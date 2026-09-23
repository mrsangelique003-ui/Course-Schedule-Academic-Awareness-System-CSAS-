using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using CourseScheduleSystem.Web.Data;
using CourseScheduleSystem.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CourseScheduleSystem.Web.Pages.Admin;

[Authorize(Roles = "Administrator")]
public class HodChatModel : PageModel
{
    private readonly ApplicationDbContext _db;

    public HodChatModel(ApplicationDbContext db) => _db = db;

    // CPs grouped by intake for the left panel
    public List<IntakeGroup> IntakeGroups { get; set; } = new();

    // Currently selected CP
    public ClassRepresentative? SelectedCp { get; set; }

    // Private conversation with the selected CP
    public List<HodMessage> Conversation { get; set; } = new();

    // Unread counts per CP id (messages sent BY the CP that HOD hasn't read)
    public Dictionary<int, int> UnreadByCp { get; set; } = new();

    // Total CP count for the header badge
    public int TotalCps => IntakeGroups.Sum(g => g.Representatives.Count);

    [BindProperty]
    public SendMessageInput Input { get; set; } = new();

    public class SendMessageInput
    {
        [Required]
        public int CpId { get; set; }

        [Required(ErrorMessage = "Please enter a message.")]
        [StringLength(2000, ErrorMessage = "Message cannot exceed 2000 characters.")]
        public string Message { get; set; } = string.Empty;
    }

    /// <summary>Groups CPs by their intake value for the sidebar.</summary>
    public class IntakeGroup
    {
        public string Intake { get; set; } = string.Empty;
        public List<ClassRepresentative> Representatives { get; set; } = new();
    }

    // ------------------------------------------------------------------ GET
    public async Task<IActionResult> OnGetAsync(int? cpId)
    {
        await LoadSidebarAsync();

        if (cpId.HasValue && cpId > 0)
        {
            SelectedCp = IntakeGroups
                .SelectMany(g => g.Representatives)
                .FirstOrDefault(r => r.Id == cpId);

            if (SelectedCp != null)
            {
                Conversation = await _db.HodMessages
                    .Where(m => m.ClassRepresentativeId == cpId)
                    .OrderBy(m => m.SentAt)
                    .ToListAsync();

                // Mark CP-sent messages as read (HOD is now reading them)
                var unread = Conversation
                    .Where(m => m.SenderType == MessageSenderType.ClassRepresentative && !m.IsRead)
                    .ToList();

                if (unread.Count > 0)
                {
                    unread.ForEach(m => m.IsRead = true);
                    await _db.SaveChangesAsync();
                }

                Input.CpId = cpId.Value;
            }
        }

        return Page();
    }

    // --------------------------------------------------------------- POST SEND
    public async Task<IActionResult> OnPostSendAsync()
    {
        await LoadSidebarAsync();

        SelectedCp = IntakeGroups
            .SelectMany(g => g.Representatives)
            .FirstOrDefault(r => r.Id == Input.CpId);

        Conversation = await _db.HodMessages
            .Where(m => m.ClassRepresentativeId == Input.CpId)
            .OrderBy(m => m.SentAt)
            .ToListAsync();

        if (!ModelState.IsValid)
            return Page();

        if (SelectedCp == null)
        {
            ModelState.AddModelError(string.Empty, "Please select a valid Class Representative.");
            return Page();
        }

        var staffIdClaim = User.FindFirstValue("StaffId");
        var hod = await _db.Administrators
            .FirstOrDefaultAsync(a => a.StaffId == staffIdClaim && a.IsActive);

        if (hod == null)
        {
            ModelState.AddModelError(string.Empty, "Could not identify the sending administrator.");
            return Page();
        }

        _db.HodMessages.Add(new HodMessage
        {
            ClassRepresentativeId = Input.CpId,
            HodId                 = hod.Id,
            Message               = Input.Message.Trim(),
            SenderType            = MessageSenderType.Hod,
            IsRead                = false,          // unread for the CP
            SentAt                = DateTime.UtcNow
        });

        await _db.SaveChangesAsync();

        return RedirectToPage(new { cpId = Input.CpId });
    }

    // --------------------------------------------------------------- HELPERS
    private async Task LoadSidebarAsync()
    {
        var allCps = await _db.ClassRepresentatives
            .Where(r => r.IsActive)
            .OrderBy(r => r.Intake)
            .ThenBy(r => r.FullName)
            .ToListAsync();

        // Unread counts: CP-sent messages not yet read by HOD
        var unreadCounts = await _db.HodMessages
            .Where(m => m.SenderType == MessageSenderType.ClassRepresentative && !m.IsRead)
            .GroupBy(m => m.ClassRepresentativeId)
            .Select(g => new { CpId = g.Key, Count = g.Count() })
            .ToListAsync();

        UnreadByCp = unreadCounts.ToDictionary(x => x.CpId, x => x.Count);

        // Group by intake
        IntakeGroups = allCps
            .GroupBy(cp => string.IsNullOrWhiteSpace(cp.Intake) ? "Unknown" : cp.Intake)
            .OrderBy(g => g.Key)
            .Select(g => new IntakeGroup
            {
                Intake          = g.Key,
                Representatives = g.ToList()
            })
            .ToList();
    }
}

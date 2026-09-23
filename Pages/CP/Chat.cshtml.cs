using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using CourseScheduleSystem.Web.Data;
using CourseScheduleSystem.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CourseScheduleSystem.Web.Pages.CP.Chat;

public class IndexModel : CourseScheduleSystem.Web.Pages.CP.CpPageModel
{
    public List<HodMessage> Messages { get; set; } = new();
    public int UnreadCount { get; set; }

    [BindProperty]
    public ReplyInput Input { get; set; } = new();

    public class ReplyInput
    {
        [Required(ErrorMessage = "Please type a message before sending.")]
        [StringLength(2000, ErrorMessage = "Message cannot exceed 2000 characters.")]
        public string Message { get; set; } = string.Empty;
    }

    public IndexModel(ApplicationDbContext context) : base(context) { }

    // ------------------------------------------------------------------ GET
    public async Task<IActionResult> OnGetAsync()
    {
        if (!IsCurrentCpValid())
            return Unauthorized();

        await LoadMessagesAsync();

        // Mark HOD-sent messages as read (CP is now reading them)
        var unread = Messages
            .Where(m => m.SenderType == MessageSenderType.Hod && !m.IsRead)
            .ToList();

        if (unread.Count > 0)
        {
            unread.ForEach(m => m.IsRead = true);
            await _context.SaveChangesAsync();
        }

        return Page();
    }

    // -------------------------------------------------------------- POST REPLY
    public async Task<IActionResult> OnPostReplyAsync()
    {
        if (!IsCurrentCpValid())
            return Unauthorized();

        await LoadMessagesAsync();

        if (!ModelState.IsValid)
            return Page();

        // We need the HOD id — use the HOD that sent the most recent message,
        // or fall back to the first active administrator
        var hodId = Messages
            .Where(m => m.SenderType == MessageSenderType.Hod)
            .OrderByDescending(m => m.SentAt)
            .Select(m => (int?)m.HodId)
            .FirstOrDefault();

        if (hodId == null)
        {
            var admin = await _context.Administrators
                .Where(a => a.IsActive)
                .OrderBy(a => a.Id)
                .FirstOrDefaultAsync();

            if (admin == null)
            {
                ModelState.AddModelError(string.Empty, "No administrator found to send the message to.");
                return Page();
            }

            hodId = admin.Id;
        }

        _context.HodMessages.Add(new HodMessage
        {
            ClassRepresentativeId = CurrentCpId,
            HodId                 = hodId.Value,
            Message               = Input.Message.Trim(),
            SenderType            = MessageSenderType.ClassRepresentative,
            IsRead                = false,          // unread for the HOD
            SentAt                = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();

        return RedirectToPage();
    }

    // --------------------------------------------------------------- HELPERS
    private async Task LoadMessagesAsync()
    {
        Messages = await _context.HodMessages
            .Where(m => m.ClassRepresentativeId == CurrentCpId)
            .OrderBy(m => m.SentAt)
            .ToListAsync();

        UnreadCount = Messages.Count(m =>
            m.SenderType == MessageSenderType.Hod && !m.IsRead);
    }
}

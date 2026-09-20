using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using CourseScheduleSystem.Web.Data;
using CourseScheduleSystem.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CourseScheduleSystem.Web.Pages.Student;

[Authorize(Roles = "Student")]
public class TechnicalSupportModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public TechnicalSupportModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public CourseScheduleSystem.Web.Models.Student? CurrentStudent { get; private set; }

    public List<SupportTicket> Tickets { get; private set; } = new();

    [BindProperty]
    public SupportTicketInput Input { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        var student = await GetCurrentStudentAsync();

        if (student == null)
        {
            return RedirectToPage("/Account/Login");
        }

        CurrentStudent = student;

        await LoadTicketsAsync(student.Id);

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var student = await GetCurrentStudentAsync();

        if (student == null)
        {
            return RedirectToPage("/Account/Login");
        }

        if (!ModelState.IsValid)
        {
            CurrentStudent = student;

            await LoadTicketsAsync(student.Id);

            return Page();
        }

        var ticket = new SupportTicket
        {
            TicketNumber = GenerateTicketNumber(),
            StudentId = student.Id,
            Subject = Input.Subject.Trim(),
            Category = Input.Category!.Value,
            Description = Input.Description.Trim(),
            Status = SupportTicketStatus.Open,
            CreatedAt = DateTime.UtcNow
        };

        _context.SupportTickets.Add(ticket);

        await _context.SaveChangesAsync();

        TempData["SupportSuccess"] =
            $"Your support request has been submitted successfully. Ticket number: {ticket.TicketNumber}";

        return RedirectToPage();
    }

    private async Task<CourseScheduleSystem.Web.Models.Student?> GetCurrentStudentAsync()
    {
        var studentIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(studentIdClaim, out var studentId))
        {
            return null;
        }

        return await _context.Students
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == studentId);
    }

    private async Task LoadTicketsAsync(int studentId)
    {
        Tickets = await _context.SupportTickets
            .AsNoTracking()
            .Where(t => t.StudentId == studentId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    private static string GenerateTicketNumber()
    {
        return $"SUP-{Guid.NewGuid():N}"[..12].ToUpperInvariant();
    }

    public static string GetCategoryName(SupportCategory category)
    {
        return category switch
        {
            SupportCategory.TechnicalIssue => "Technical Issue",
            SupportCategory.AccountAccess => "Account Access",
            SupportCategory.CourseSchedule => "Course Schedule",
            SupportCategory.Enrollment => "Enrollment",
            SupportCategory.Examination => "Examination",
            SupportCategory.Other => "Other",
            _ => "Other"
        };
    }

    public static string GetStatusName(SupportTicketStatus status)
    {
        return status switch
        {
            SupportTicketStatus.Open => "Open",
            SupportTicketStatus.InProgress => "In Progress",
            SupportTicketStatus.Resolved => "Resolved",
            SupportTicketStatus.Closed => "Closed",
            _ => "Open"
        };
    }

    public static string GetStatusClass(SupportTicketStatus status)
    {
        return status switch
        {
            SupportTicketStatus.Open => "status-open",
            SupportTicketStatus.InProgress => "status-progress",
            SupportTicketStatus.Resolved => "status-resolved",
            SupportTicketStatus.Closed => "status-closed",
            _ => "status-open"
        };
    }

    public class SupportTicketInput
    {
        [Required(ErrorMessage = "Please enter a subject.")]
        [StringLength(
            100,
            MinimumLength = 3,
            ErrorMessage = "The subject must be between 3 and 100 characters.")]
        public string Subject { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please select a support category.")]
        public SupportCategory? Category { get; set; }

        [Required(ErrorMessage = "Please describe your issue.")]
        [StringLength(
            2000,
            MinimumLength = 10,
            ErrorMessage = "The description must be between 10 and 2000 characters.")]
        public string Description { get; set; } = string.Empty;
    }
}


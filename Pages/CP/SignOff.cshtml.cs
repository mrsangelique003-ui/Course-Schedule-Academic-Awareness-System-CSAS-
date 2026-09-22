using CourseScheduleSystem.Web.Data;
using CourseScheduleSystem.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CourseScheduleSystem.Web.Pages.CP.SignOff;

public class IndexModel : CourseScheduleSystem.Web.Pages.CP.CpPageModel
{
    public List<CourseCompletion> Completions { get; set; } = new();

    public IndexModel(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<IActionResult> OnGetAsync()
    {
        if (!IsCurrentCpValid())
        {
            return Unauthorized();
        }

        Completions = await _context.CourseCompletions
            .Include(c => c.Course)
            .Include(c => c.Lecturer)
            .Where(c => c.ClassRepresentativeId == CurrentCpId)
            .OrderByDescending(c => c.SubmittedAt)
            .ToListAsync();

        return Page();
    }

    public async Task<IActionResult> OnPostConfirmAsync(int id)
    {
        if (!IsCurrentCpValid())
        {
            return Unauthorized();
        }

        var completion = await _context.CourseCompletions
            .FirstOrDefaultAsync(c =>
                c.Id == id &&
                c.ClassRepresentativeId == CurrentCpId);

        if (completion == null)
        {
            return NotFound();
        }

        if (completion.Status != CourseCompletionStatus.Submitted)
        {
            return BadRequest("This course completion cannot be confirmed.");
        }

        completion.Status = CourseCompletionStatus.Confirmed;
        completion.ConfirmedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return LocalRedirect("/CP/SignOff");
    }

    public async Task<IActionResult> OnPostReturnAsync(
        int id,
        string? remarks)
    {
        if (!IsCurrentCpValid())
        {
            return Unauthorized();
        }

        var completion = await _context.CourseCompletions
            .FirstOrDefaultAsync(c =>
                c.Id == id &&
                c.ClassRepresentativeId == CurrentCpId);

        if (completion == null)
        {
            return NotFound();
        }

        if (completion.Status != CourseCompletionStatus.Submitted)
        {
            return BadRequest("This course completion cannot be returned.");
        }

        if (string.IsNullOrWhiteSpace(remarks))
        {
            ModelState.AddModelError(
                string.Empty,
                "Remarks are required when returning a course completion.");

            return await ReloadPageAsync();
        }

        remarks = remarks.Trim();

        if (remarks.Length > 1000)
        {
            ModelState.AddModelError(
                string.Empty,
                "Remarks cannot exceed 1000 characters.");

            return await ReloadPageAsync();
        }

        completion.Status = CourseCompletionStatus.Returned;
        completion.Remarks = remarks;

        await _context.SaveChangesAsync();

        return LocalRedirect("/CP/SignOff");
    }

    private async Task<IActionResult> ReloadPageAsync()
    {
        Completions = await _context.CourseCompletions
            .Include(c => c.Course)
            .Include(c => c.Lecturer)
            .Where(c => c.ClassRepresentativeId == CurrentCpId)
            .OrderByDescending(c => c.SubmittedAt)
            .ToListAsync();

        return Page();
    }
}
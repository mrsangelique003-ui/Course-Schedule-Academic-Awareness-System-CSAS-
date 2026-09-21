using CourseScheduleSystem.Web.Data;
using CourseScheduleSystem.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CourseScheduleSystem.Web.Pages.CP.Dashboard;

public class IndexModel : CourseScheduleSystem.Web.Pages.CP.CpPageModel
{
    public ClassRepresentative? CurrentCp { get; private set; }

    public int TotalCourses { get; private set; }
    public int TotalGroups { get; private set; }
    public int PendingCompletions { get; private set; }
    public int UnreadMessages { get; private set; }

    public IndexModel(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<IActionResult> OnGetAsync()
    {
        CurrentCp = await GetCurrentCpAsync();

        if (CurrentCp == null)
        {
            return NotFound();
        }

        TotalCourses = await _context.Courses
            .CountAsync(c =>
                c.ClassRepresentatives
                    .Any(cp => cp.Id == CurrentCpId));

        TotalGroups = await _context.ClassGroups
            .CountAsync(g =>
                g.ClassRepresentativeId == CurrentCpId &&
                g.IsActive);

        PendingCompletions = await _context.CourseCompletions
            .CountAsync(c =>
                c.ClassRepresentativeId == CurrentCpId &&
                c.Status == CourseCompletionStatus.Submitted);

        UnreadMessages = await _context.HodMessages
            .CountAsync(m =>
                m.ClassRepresentativeId == CurrentCpId &&
                !m.IsRead);

        return Page();
    }
}
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
}
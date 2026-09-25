using CourseScheduleSystem.Web.Data;
using CourseScheduleSystem.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CourseScheduleSystem.Web.Pages.CP.CisCps;

public class IndexModel : CourseScheduleSystem.Web.Pages.CP.CpPageModel
{
    public List<ClassRepresentative> Representatives { get; set; } = new();

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

        Representatives = await _context.ClassRepresentatives
            .Where(cp =>
                cp.IsActive &&
                cp.Faculty.Contains("Computing"))
            .OrderBy(cp => cp.FullName)
            .ToListAsync();

        return Page();
    }
}

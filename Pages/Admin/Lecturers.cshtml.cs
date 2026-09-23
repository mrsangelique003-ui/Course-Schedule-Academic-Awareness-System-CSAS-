using CourseScheduleSystem.Web.Data;
using CourseScheduleSystem.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CourseScheduleSystem.Web.Pages.Admin;

[Authorize(Roles = "Administrator")]
public class LecturersModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public LecturersModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public List<Lecturer> Lecturers { get; set; } = new();

    public int TotalLecturers { get; set; }

    public int ActiveLecturers { get; set; }

    public int InactiveLecturers { get; set; }

    public async Task OnGetAsync()
    {
        Lecturers = await _context.Lecturers
            .AsNoTracking()
            .OrderBy(l => l.FullName)
            .ToListAsync();

        TotalLecturers = Lecturers.Count;

        ActiveLecturers = Lecturers.Count(l => l.IsActive);

        InactiveLecturers = Lecturers.Count(l => !l.IsActive);
    }
}
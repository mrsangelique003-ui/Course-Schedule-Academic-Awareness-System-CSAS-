using CourseScheduleSystem.Web.Data;
using CourseScheduleSystem.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CourseScheduleSystem.Web.Pages.Admin;

[Authorize(Roles = "Administrator")]
public class CoursesModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public CoursesModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public List<Course> Courses { get; set; } = new();

    public List<Lecturer> Lecturers { get; set; } = new();

    public int TotalCourses { get; set; }

    public int ActiveCourses { get; set; }

    public int InactiveCourses { get; set; }

    public int TotalCredits { get; set; }

    public async Task OnGetAsync()
    {
        Courses = await _context.Courses
            .AsNoTracking()
            .OrderBy(c => c.Code)
            .ToListAsync();

        Lecturers = await _context.Lecturers
            .AsNoTracking()
            .OrderBy(l => l.FullName)
            .ToListAsync();

        TotalCourses = Courses.Count;

        ActiveCourses = Courses.Count(c => c.IsActive);

        InactiveCourses = Courses.Count(c => !c.IsActive);

        TotalCredits = Courses.Sum(c => c.Credits);
    }
}
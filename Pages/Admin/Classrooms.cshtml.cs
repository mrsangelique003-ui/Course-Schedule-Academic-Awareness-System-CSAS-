using CourseScheduleSystem.Web.Data;
using CourseScheduleSystem.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CourseScheduleSystem.Web.Pages.Admin;

[Authorize(Roles = "Administrator")]
public class ClassroomsModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public ClassroomsModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public List<Room> Classrooms { get; set; } = new();

    public List<Course> Courses { get; set; } = new();

    public List<Lecturer> Lecturers { get; set; } = new();

    public List<ClassRepresentative> ClassRepresentatives { get; set; } = new();

    public int TotalClassrooms { get; set; }

    public int AvailableClassrooms { get; set; }

    public int UnavailableClassrooms { get; set; }

    public int TotalCapacity { get; set; }

    public async Task OnGetAsync()
    {
        Classrooms = await _context.Rooms
            .AsNoTracking()
            .OrderBy(r => r.Building)
            .ThenBy(r => r.RoomNumber)
            .ToListAsync();

        Courses = await _context.Courses
            .AsNoTracking()
            .Where(c => c.IsActive)
            .OrderBy(c => c.Code)
            .ToListAsync();

        Lecturers = await _context.Lecturers
            .AsNoTracking()
            .Where(l => l.IsActive)
            .OrderBy(l => l.FullName)
            .ToListAsync();

        ClassRepresentatives = await _context.ClassRepresentatives
            .AsNoTracking()
            .Where(c => c.IsActive)
            .OrderBy(c => c.FullName)
            .ToListAsync();

        TotalClassrooms = Classrooms.Count;

        AvailableClassrooms = Classrooms.Count(r => r.IsAvailable);

        UnavailableClassrooms = Classrooms.Count(r => !r.IsAvailable);

        TotalCapacity = Classrooms.Sum(r => r.Capacity);
    }
}
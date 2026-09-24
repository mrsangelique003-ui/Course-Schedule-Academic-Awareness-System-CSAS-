using CourseScheduleSystem.Web.Data;
using CourseScheduleSystem.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CourseScheduleSystem.Web.Pages.Admin;

[Authorize(Roles = "Administrator")]
public class ClassroomsModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ClassroomsModel> _logger;


public ClassroomsModel(
    ApplicationDbContext context,
    ILogger<ClassroomsModel> logger)
    {
        _context = context;
        _logger = logger;
    }

    public List<Room> Classrooms { get; set; } = new();

    public List<Course> Courses { get; set; } = new();

    public List<Lecturer> Lecturers { get; set; } = new();

    public List<ClassRepresentative> ClassRepresentatives { get; set; } = new();

    public List<ScheduleEntry> ScheduleEntries { get; set; } = new();

    public int TotalClassrooms { get; set; }

    public int AvailableClassrooms { get; set; }

    public int UnavailableClassrooms { get; set; }

    public int TotalCapacity { get; set; }

    [TempData]
    public string? SuccessMessage { get; set; }

    public async Task OnGetAsync()
    {
        await LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        Classrooms = await _context.Rooms
            .AsNoTracking()
            .OrderBy(r => r.Building)
            .ThenBy(r => r.RoomNumber)
            .ToListAsync();

        Courses = await _context.Courses
            .AsNoTracking()
            .Include(c => c.Lecturer)
            .Include(c => c.ClassRepresentatives)
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
            .Where(cr => cr.IsActive)
            .OrderBy(cr => cr.FullName)
            .ToListAsync();

        ScheduleEntries = await _context.ScheduleEntries
            .AsNoTracking()
            .Include(s => s.Course)
            .Include(s => s.Lecturer)
            .Include(s => s.ClassRepresentative)
            .Include(s => s.Room)
            .Where(s =>
                s.IsActive &&
                s.Status == ScheduleStatus.Active)
            .OrderBy(s => s.Room.Building)
            .ThenBy(s => s.Room.RoomNumber)
            .ThenBy(s => s.DayOfWeek)
            .ThenBy(s => s.StartTime)
            .ToListAsync();

        await ApplyCurrentAvailabilityAsync();

        TotalClassrooms = Classrooms.Count;

        AvailableClassrooms = Classrooms.Count(r => r.IsAvailable);

        UnavailableClassrooms =
            TotalClassrooms - AvailableClassrooms;

        TotalCapacity = Classrooms.Sum(r => r.Capacity);
    }

    private async Task ApplyCurrentAvailabilityAsync()
    {
        var today = DateTime.Today;
        var currentTime = TimeOnly.FromDateTime(DateTime.Now);
        var currentDay = DateTime.Now.DayOfWeek;

        var occupiedRoomIds = await _context.ScheduleEntries
            .AsNoTracking()
            .Where(s =>
                s.IsActive &&
                s.Status == ScheduleStatus.Active &&
                s.StartDate.Date <= today &&
                s.EndDate.Date >= today &&
                s.DayOfWeek == currentDay &&
                s.StartTime <= currentTime &&
                s.EndTime > currentTime)
            .Select(s => s.RoomId)
            .Distinct()
            .ToListAsync();

        var occupiedSet = occupiedRoomIds.ToHashSet();

        foreach (var classroom in Classrooms)
        {
            if (occupiedSet.Contains(classroom.Id))
            {
                classroom.IsAvailable = false;
            }
        }
    }


}

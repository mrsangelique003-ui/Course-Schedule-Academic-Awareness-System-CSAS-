using CourseScheduleSystem.Web.Data;
using CourseScheduleSystem.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CourseScheduleSystem.Web.Pages.Admin;

[Authorize(Roles = "Administrator")]
public class DashboardModel : PageModel
{
    private readonly ApplicationDbContext _db;


public DashboardModel(ApplicationDbContext db)
    {
        _db = db;
    }

    public string WelcomeName { get; private set; } = "Administrator";

    public int TotalStudents { get; private set; }

    public int TotalLecturers { get; private set; }

    public int TotalCourses { get; private set; }

    public int TotalClasses { get; private set; }

    public int TotalRooms { get; private set; }

    public int AvailableRooms { get; private set; }

    public int TotalEnrollments { get; private set; }

    public List<CourseEnrollmentRow> CourseEnrollments { get; private set; } = new();

    public List<TodayClassRow> TodayClasses { get; private set; } = new();

    public DateTime LastUpdated { get; private set; }

    public async Task OnGetAsync()
    {
        WelcomeName =
            User.Identity?.Name
            ?? "Administrator";

        LastUpdated = DateTime.Now;

        TotalStudents =
            await _db.Students
                .AsNoTracking()
                .CountAsync(s => s.IsActive);

        TotalLecturers =
            await _db.Lecturers
                .AsNoTracking()
                .CountAsync(l => l.IsActive);

        TotalCourses =
            await _db.Courses
                .AsNoTracking()
                .CountAsync(c => c.IsActive);

        TotalClasses =
            await _db.ScheduleEntries
                .AsNoTracking()
                .CountAsync(s =>
                    s.IsActive &&
                    s.Status == ScheduleStatus.Active);

        TotalRooms =
            await _db.Rooms
                .AsNoTracking()
                .CountAsync();

        AvailableRooms =
            await _db.Rooms
                .AsNoTracking()
                .CountAsync(r => r.IsAvailable);

        TotalEnrollments =
            await _db.Enrollments
                .AsNoTracking()
                .CountAsync(e =>
                    e.Status == EnrollmentStatus.Enrolled);

        CourseEnrollments =
            await _db.Courses
                .AsNoTracking()
                .Where(c => c.IsActive)
                .OrderBy(c => c.Code)
                .Select(c => new CourseEnrollmentRow
                {
                    CourseId = c.Id,
                    CourseCode = c.Code,
                    CourseName = c.Name,
                    LecturerName =
                        c.Lecturer != null
                            ? c.Lecturer.FullName
                            : "Not assigned",
                    Credits = c.Credits,
                    EnrollmentCount =
                        c.Enrollments.Count(e =>
                            e.Status == EnrollmentStatus.Enrolled)
                })
                .ToListAsync();

        var today = DateTime.Now.DayOfWeek;
        var currentTime = TimeOnly.FromDateTime(DateTime.Now);

        var todaySchedules =
            await _db.ScheduleEntries
                .AsNoTracking()
                .Include(s => s.Course)
                .Include(s => s.Lecturer)
                .Include(s => s.Room)
                .Where(s =>
                    s.IsActive &&
                    s.Status == ScheduleStatus.Active &&
                    s.DayOfWeek == today)
                .OrderBy(s => s.StartTime)
                .ToListAsync();

        TodayClasses =
            todaySchedules
                .Select(s =>
                {
                    var status =
                        currentTime >= s.StartTime &&
                        currentTime <= s.EndTime
                            ? "Ongoing"
                            : currentTime < s.StartTime
                                ? "Upcoming"
                                : "Completed";

                    return new TodayClassRow
                    {
                        ScheduleId = s.Id,
                        CourseCode = s.Course.Code,
                        CourseName = s.Course.Name,
                        LecturerName = s.Lecturer.FullName,
                        Room = $"{s.Room.Building} · {s.Room.RoomNumber}",
                        StartTime = s.StartTime,
                        EndTime = s.EndTime,
                        StudySession = s.StudySession.ToString(),
                        Status = status
                    };
                })
                .ToList();
    }

    public class CourseEnrollmentRow
    {
        public int CourseId { get; set; }

        public string CourseCode { get; set; } = string.Empty;

        public string CourseName { get; set; } = string.Empty;

        public string LecturerName { get; set; } = string.Empty;

        public int Credits { get; set; }

        public int EnrollmentCount { get; set; }
    }

    public class TodayClassRow
    {
        public int ScheduleId { get; set; }

        public string CourseCode { get; set; } = string.Empty;

        public string CourseName { get; set; } = string.Empty;

        public string LecturerName { get; set; } = string.Empty;

        public string Room { get; set; } = string.Empty;

        public TimeOnly StartTime { get; set; }

        public TimeOnly EndTime { get; set; }

        public string StudySession { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;
    }


}

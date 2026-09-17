using System.Security.Claims;
using CourseScheduleSystem.Web.Data;
using CourseScheduleSystem.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CourseScheduleSystem.Web.Pages;

[Authorize]
public class DashboardModel : PageModel
{
    private readonly ApplicationDbContext _db;

    public DashboardModel(ApplicationDbContext db)
    {
        _db = db;
    }

    public CourseScheduleSystem.Web.Models.Student? CurrentStudent { get; set; }

    public string UserName { get; set; } = string.Empty;

    public string UserRole { get; set; } = string.Empty;

    public int TotalEnrolledCourses { get; private set; }

    public int TotalUpcomingClasses { get; private set; }

    public int TotalCredits { get; private set; }

    public List<CourseItem> Courses { get; set; } = new();

    public List<ScheduleItemDto> ScheduleItems { get; set; } = new();

    public async Task OnGetAsync()
    {
        UserName =
            User.FindFirstValue(ClaimTypes.Name)
            ?? "User";

        UserRole =
            User.FindFirstValue(ClaimTypes.Role)
            ?? string.Empty;

        var userIdValue =
            User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdValue, out var studentId))
        {
            return;
        }

        CurrentStudent = await _db.Students
            .AsNoTracking()
            .FirstOrDefaultAsync(s =>
                s.Id == studentId &&
                s.IsActive);

        if (CurrentStudent == null)
        {
            return;
        }

        Courses = await _db.Enrollments
            .AsNoTracking()
            .Where(e =>
                e.StudentId == studentId)
            .OrderBy(e => e.Course.Code)
            .Select(e => new CourseItem
            {
                EnrollmentId = e.Id,

                CourseId = e.CourseId,

                Code = e.Course.Code,

                Name = e.Course.Name,

                Credits = e.Course.Credits,

                LecturerName =
                    e.Course.Lecturer != null &&
                    !string.IsNullOrWhiteSpace(e.Course.Lecturer.FullName)
                        ? e.Course.Lecturer.FullName
                        : "Lecturer not assigned",

                Status = e.Status
            })
            .ToListAsync();

        TotalEnrolledCourses =
            Courses.Count(c =>
                c.Status == EnrollmentStatus.Enrolled);

        TotalCredits =
            Courses
                .Where(c =>
                    c.Status == EnrollmentStatus.Enrolled)
                .Sum(c => c.Credits);

        var enrolledCourseIds =
            Courses
                .Where(c =>
                    c.Status == EnrollmentStatus.Enrolled)
                .Select(c => c.CourseId)
                .ToList();

        if (enrolledCourseIds.Count == 0)
        {
            ScheduleItems = new List<ScheduleItemDto>();
            TotalUpcomingClasses = 0;
            return;
        }

        var today = DateTime.Today.DayOfWeek;

        ScheduleItems = await _db.ScheduleEntries
            .AsNoTracking()
            .Where(s =>
                s.IsActive &&
                s.Status == ScheduleStatus.Active &&
                s.DayOfWeek == today &&
                enrolledCourseIds.Contains(s.CourseId))
            .OrderBy(s => s.StartTime)
            .Select(s => new ScheduleItemDto
            {
                Id = s.Id,

                TeacherName =
                    s.Lecturer != null &&
                    !string.IsNullOrWhiteSpace(s.Lecturer.FullName)
                        ? s.Lecturer.FullName
                        : "Lecturer not assigned",

                Subject =
                    s.Course != null &&
                    !string.IsNullOrWhiteSpace(s.Course.Name)
                        ? s.Course.Name
                        : "Course not assigned",

                CourseCode =
                    s.Course != null
                        ? s.Course.Code
                        : string.Empty,

                StartTime =
                    s.StartTime.ToString("hh:mm tt"),

                EndTime =
                    s.EndTime.ToString("hh:mm tt"),

                RoomNumber =
                    s.Room == null
                        ? "Room not assigned"
                        : string.IsNullOrWhiteSpace(s.Room.Building)
                            ? (
                                string.IsNullOrWhiteSpace(s.Room.RoomNumber)
                                    ? "Room not assigned"
                                    : s.Room.RoomNumber
                              )
                            : string.IsNullOrWhiteSpace(s.Room.RoomNumber)
                                ? s.Room.Building
                                : $"{s.Room.Building} · {s.Room.RoomNumber}",

                StudySession =
                    s.StudySession,

                Notes =
                    s.Notes,

                CardColorClass =
                    GetColorClass(
                        s.Course != null
                            ? s.Course.Name
                            : string.Empty)
            })
            .ToListAsync();

        TotalUpcomingClasses =
            ScheduleItems.Count;
    }

    private static string GetColorClass(string subject)
    {
        if (string.IsNullOrWhiteSpace(subject))
        {
            return "dash-schedule-card-green";
        }

        var value =
            subject.Trim().ToLowerInvariant();

        if (value.Contains("science"))
        {
            return "dash-schedule-card-green";
        }

        if (value.Contains("biology"))
        {
            return "dash-schedule-card-yellow";
        }

        if (value.Contains("physics"))
        {
            return "dash-schedule-card-purple";
        }

        if (value.Contains("mathematics") ||
            value.Contains("math"))
        {
            return "dash-schedule-card-blue";
        }

        return "dash-schedule-card-green";
    }

    public class CourseItem
    {
        public int EnrollmentId { get; set; }

        public int CourseId { get; set; }

        public string Code { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public int Credits { get; set; }

        public string LecturerName { get; set; } = string.Empty;

        public EnrollmentStatus Status { get; set; }
    }

    public class ScheduleItemDto
    {
        public int Id { get; set; }

        public string TeacherName { get; set; } = string.Empty;

        public string Subject { get; set; } = string.Empty;

        public string CourseCode { get; set; } = string.Empty;

        public string StartTime { get; set; } = string.Empty;

        public string EndTime { get; set; } = string.Empty;

        public string RoomNumber { get; set; } = string.Empty;

        public StudySession StudySession { get; set; }

        public string? Notes { get; set; }

        public string CardColorClass { get; set; } = string.Empty;
    }
}

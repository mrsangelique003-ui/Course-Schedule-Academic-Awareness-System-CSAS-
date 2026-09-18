using System.Security.Claims;
using CourseScheduleSystem.Web.Data;
using CourseScheduleSystem.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CourseScheduleSystem.Web.Pages;

[Authorize(Roles = "Student")]
public class DashboardModel : PageModel
{
    private readonly ApplicationDbContext _db;

public DashboardModel(ApplicationDbContext db)
    {
        _db = db;
    }

    public CourseScheduleSystem.Web.Models.Student? CurrentStudent { get; private set; }

    public string UserName { get; private set; } = string.Empty;

    public string UserRole { get; private set; } = string.Empty;

    public int TotalEnrolledCourses { get; private set; }

    public int TotalUpcomingClasses { get; private set; }

    public int TotalCredits { get; private set; }

    public List<CourseItem> Courses { get; private set; } = new();

    public List<ScheduleItemDto> ScheduleItems { get; private set; } = new();

    public async Task OnGetAsync()
    {
        UserName =
            User.FindFirstValue(ClaimTypes.Name)
            ?? "Student";

        UserRole =
            User.FindFirstValue(ClaimTypes.Role)
            ?? "Student";

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
                e.StudentId == studentId &&
                e.Status == EnrollmentStatus.Enrolled)
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
            Courses.Count;

        TotalCredits =
            Courses.Sum(c => c.Credits);

        var enrolledCourseIds =
            Courses
                .Select(c => c.CourseId)
                .Distinct()
                .ToList();

        if (enrolledCourseIds.Count == 0)
        {
            ScheduleItems = new List<ScheduleItemDto>();
            TotalUpcomingClasses = 0;
            return;
        }

        var today =
            DateTime.Today.DayOfWeek;

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
                        : !string.IsNullOrWhiteSpace(s.Room.Building) &&
                          !string.IsNullOrWhiteSpace(s.Room.RoomNumber)
                            ? $"{s.Room.Building} · {s.Room.RoomNumber}"
                            : !string.IsNullOrWhiteSpace(s.Room.RoomNumber)
                                ? s.Room.RoomNumber
                                : !string.IsNullOrWhiteSpace(s.Room.Building)
                                    ? s.Room.Building
                                    : "Room not assigned",

                StudySession =
                    s.StudySession,

                Notes =
                    s.Notes,

                CardColorClass =
                    "dash-schedule-card-default"
            })
            .ToListAsync();

        TotalUpcomingClasses =
            ScheduleItems.Count;
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

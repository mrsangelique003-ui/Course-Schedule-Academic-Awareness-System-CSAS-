using System.Security.Claims;
using CourseScheduleSystem.Web.Data;
using CourseScheduleSystem.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CourseScheduleSystem.Web.Pages.Student;

[Authorize(Roles = "Student")]
public class ScheduleModel : PageModel
{
    private readonly ApplicationDbContext _db;

    public ScheduleModel(ApplicationDbContext db)
    {
        _db = db;
    }

    public CourseScheduleSystem.Web.Models.Student? CurrentStudent { get; private set; }

    public List<ScheduleItem> ScheduleItems { get; private set; } = new();

    public int TotalClasses { get; private set; }

    public int TotalCourses { get; private set; }

    public async Task OnGetAsync()
    {
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

        var enrolledCourseIds = await _db.Enrollments
            .AsNoTracking()
            .Where(e =>
                e.StudentId == studentId &&
                e.Status == EnrollmentStatus.Enrolled)
            .Select(e => e.CourseId)
            .Distinct()
            .ToListAsync();

        TotalCourses = enrolledCourseIds.Count;

        if (enrolledCourseIds.Count == 0)
        {
            ScheduleItems = new List<ScheduleItem>();
            TotalClasses = 0;
            return;
        }

        ScheduleItems = await _db.ScheduleEntries
            .AsNoTracking()
            .Where(s =>
                s.IsActive &&
                s.Status == ScheduleStatus.Active &&
                enrolledCourseIds.Contains(s.CourseId))
            .OrderBy(s => s.DayOfWeek)
            .ThenBy(s => s.StartTime)
            .Select(s => new ScheduleItem
            {
                Id = s.Id,
                CourseId = s.CourseId,
                CourseCode = s.Course.Code,
                CourseName = s.Course.Name,
                LecturerName =
                    !string.IsNullOrWhiteSpace(s.Lecturer.FullName)
                        ? s.Lecturer.FullName
                        : "Lecturer not assigned",
                Building =
                    !string.IsNullOrWhiteSpace(s.Room.Building)
                        ? s.Room.Building
                        : "Building not assigned",
                RoomNumber =
                    !string.IsNullOrWhiteSpace(s.Room.RoomNumber)
                        ? s.Room.RoomNumber
                        : "Room not assigned",
                DayOfWeek = s.DayOfWeek,
                StartTime = s.StartTime,
                EndTime = s.EndTime,
                StudySession = s.StudySession,
                Notes = s.Notes,
                Status = s.Status
            })
            .ToListAsync();

        TotalClasses = ScheduleItems.Count;
    }

    public class ScheduleItem
    {
        public int Id { get; set; }

        public int CourseId { get; set; }

        public string CourseCode { get; set; } = string.Empty;

        public string CourseName { get; set; } = string.Empty;

        public string LecturerName { get; set; } = string.Empty;

        public string Building { get; set; } = string.Empty;

        public string RoomNumber { get; set; } = string.Empty;

        public DayOfWeek DayOfWeek { get; set; }

        public TimeOnly StartTime { get; set; }

        public TimeOnly EndTime { get; set; }

        public StudySession StudySession { get; set; }

        public string? Notes { get; set; }

        public ScheduleStatus Status { get; set; }
    }
}
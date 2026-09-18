using System.Security.Claims;
using CourseScheduleSystem.Web.Data;
using CourseScheduleSystem.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CourseScheduleSystem.Web.Pages.Student;

[Authorize(Roles = "Student")]
public class CourseDetailsModel : PageModel
{
    private readonly ApplicationDbContext _db;

    public CourseDetailsModel(ApplicationDbContext db)
    {
        _db = db;
    }

    public CourseScheduleSystem.Web.Models.Student? CurrentStudent { get; private set; }

    public CourseDetailsItem? Course { get; private set; }

    public List<ScheduleItem> Schedule { get; private set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var userIdValue =
            User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdValue, out var studentId))
        {
            return RedirectToPage("/Account/Login");
        }

        CurrentStudent = await _db.Students
            .AsNoTracking()
            .FirstOrDefaultAsync(s =>
                s.Id == studentId &&
                s.IsActive);

        if (CurrentStudent == null)
        {
            return Forbid();
        }

        var enrollment = await _db.Enrollments
            .AsNoTracking()
            .Where(e =>
                e.StudentId == studentId &&
                e.CourseId == id)
            .Select(e => new CourseDetailsItem
            {
                EnrollmentId = e.Id,
                CourseId = e.CourseId,
                Code = e.Course.Code,
                Name = e.Course.Name,
                Description = e.Course.Description,
                Credits = e.Course.Credits,
                Status = e.Status,
                LecturerName =
                    e.Course.Lecturer != null &&
                    !string.IsNullOrWhiteSpace(
                        e.Course.Lecturer.FullName)
                        ? e.Course.Lecturer.FullName
                        : "Lecturer not assigned",
                StudySession = e.Course.StudySession,
                StartDate = e.Course.StartDate,
                EndDate = e.Course.EndDate
            })
            .FirstOrDefaultAsync();

        if (enrollment == null)
        {
            return NotFound();
        }

        Course = enrollment;

        Schedule = await _db.ScheduleEntries
            .AsNoTracking()
            .Where(s =>
                s.CourseId == id &&
                s.IsActive &&
                s.Status == ScheduleStatus.Active)
            .OrderBy(s => s.DayOfWeek)
            .ThenBy(s => s.StartTime)
            .Select(s => new ScheduleItem
            {
                Id = s.Id,
                DayOfWeek = s.DayOfWeek,
                StartTime = s.StartTime,
                EndTime = s.EndTime,
                StudySession = s.StudySession,
                Building = s.Room != null
                    ? s.Room.Building
                    : "Room not assigned",
                RoomNumber = s.Room != null
                    ? s.Room.RoomNumber
                    : "Room not assigned",
                LecturerName =
                    s.Lecturer != null &&
                    !string.IsNullOrWhiteSpace(
                        s.Lecturer.FullName)
                        ? s.Lecturer.FullName
                        : "Lecturer not assigned",
                Notes = s.Notes
            })
            .ToListAsync();

        return Page();
    }

    public class CourseDetailsItem
    {
        public int EnrollmentId { get; set; }

        public int CourseId { get; set; }

        public string Code { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public int Credits { get; set; }

        public string LecturerName { get; set; } = string.Empty;

        public StudySession StudySession { get; set; }

        public EnrollmentStatus Status { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
    }

    public class ScheduleItem
    {
        public int Id { get; set; }

        public DayOfWeek DayOfWeek { get; set; }

        public TimeOnly StartTime { get; set; }

        public TimeOnly EndTime { get; set; }

        public StudySession StudySession { get; set; }

        public string Building { get; set; } = string.Empty;

        public string RoomNumber { get; set; } = string.Empty;

        public string LecturerName { get; set; } = string.Empty;

        public string? Notes { get; set; }
    }


}

using CourseScheduleSystem.Web.Data;
using CourseScheduleSystem.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CourseScheduleSystem.Web.Pages.Admin;

[Authorize(Roles = "Administrator")]
public class StudentDetailsModel : PageModel
{
    private readonly ApplicationDbContext _db;


public StudentDetailsModel(ApplicationDbContext db)
    {
        _db = db;
    }

    public StudentProfile? Student { get; private set; }

    public int TotalEnrollments { get; private set; }

    public int ActiveEnrollments { get; private set; }

    public int PendingEnrollments { get; private set; }

    public int CompletedEnrollments { get; private set; }

    public List<EnrollmentRow> Enrollments { get; private set; } = new();

    public List<ScheduleRow> Schedule { get; private set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Student = await _db.Students
            .AsNoTracking()
            .Where(s => s.Id == id)
            .Select(s => new StudentProfile
            {
                Id = s.Id,
                RegNo = s.RegNo,
                FullName = s.FullName,
                Email = s.Email,
                PhoneNumber = s.PhoneNumber,
                Department = s.Department,
                Program = s.Program,
                Level = s.Level,
                StudySession = s.StudySession,
                IsActive = s.IsActive,
                CreatedAt = s.CreatedAt
            })
            .FirstOrDefaultAsync();

        if (Student is null)
        {
            return NotFound();
        }

        var enrollmentData = await _db.Enrollments
            .AsNoTracking()
            .Where(e => e.StudentId == id)
            .Include(e => e.Course)
                .ThenInclude(c => c.Lecturer)
            .OrderByDescending(e => e.RequestedAt)
            .ToListAsync();

        TotalEnrollments = enrollmentData.Count;

        ActiveEnrollments = enrollmentData.Count(
            e => e.Status == EnrollmentStatus.Enrolled);

        PendingEnrollments = enrollmentData.Count(
            e => e.Status == EnrollmentStatus.Pending);

        CompletedEnrollments = enrollmentData.Count(
            e =>
                e.Status != EnrollmentStatus.Enrolled &&
                e.Status != EnrollmentStatus.Pending);

        var courseOccurrences = enrollmentData
            .GroupBy(e => e.CourseId)
            .ToDictionary(
                g => g.Key,
                g => g.Count());

        Enrollments = enrollmentData
            .Select(e => new EnrollmentRow
            {
                Id = e.Id,
                CourseCode = e.Course.Code,
                CourseName = e.Course.Name,
                Credits = e.Course.Credits,
                LecturerName = e.Course.Lecturer?.FullName ?? "Not assigned",
                Status = e.Status.ToString(),
                RequestedAt = e.RequestedAt,
                ApprovedAt = e.ApprovedAt,
                IsRepeated = courseOccurrences[e.CourseId] > 1
            })
            .ToList();

        var activeCourseIds = enrollmentData
            .Where(e => e.Status == EnrollmentStatus.Enrolled)
            .Select(e => e.CourseId)
            .Distinct()
            .ToList();

        if (activeCourseIds.Count > 0)
        {
            var scheduleData = await _db.ScheduleEntries
                .AsNoTracking()
                .Include(s => s.Course)
                .Include(s => s.Lecturer)
                .Include(s => s.Room)
                .Where(s =>
                    s.IsActive &&
                    s.Status == ScheduleStatus.Active &&
                    activeCourseIds.Contains(s.CourseId))
                .OrderBy(s => s.DayOfWeek)
                .ThenBy(s => s.StartTime)
                .ToListAsync();

            Schedule = scheduleData
                .Select(s => new ScheduleRow
                {
                    Id = s.Id,
                    CourseCode = s.Course.Code,
                    CourseName = s.Course.Name,
                    LecturerName = s.Lecturer.FullName,
                    Room = $"{s.Room.Building} · {s.Room.RoomNumber}",
                    Day = s.DayOfWeek.ToString(),
                    StartTime = s.StartTime,
                    EndTime = s.EndTime,
                    StudySession = s.StudySession.ToString()
                })
                .ToList();
        }

        return Page();
    }

    public class StudentProfile
    {
        public int Id { get; set; }

        public string RegNo { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;

        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }

        public string Department { get; set; } = string.Empty;

        public string? Program { get; set; }

        public string? Level { get; set; }

        public StudySession StudySession { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }
    }

    public class EnrollmentRow
    {
        public int Id { get; set; }

        public string CourseCode { get; set; } = string.Empty;

        public string CourseName { get; set; } = string.Empty;

        public int Credits { get; set; }

        public string LecturerName { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public DateTime RequestedAt { get; set; }

        public DateTime? ApprovedAt { get; set; }

        public bool IsRepeated { get; set; }
    }

    public class ScheduleRow
    {
        public int Id { get; set; }

        public string CourseCode { get; set; } = string.Empty;

        public string CourseName { get; set; } = string.Empty;

        public string LecturerName { get; set; } = string.Empty;

        public string Room { get; set; } = string.Empty;

        public string Day { get; set; } = string.Empty;

        public TimeOnly StartTime { get; set; }

        public TimeOnly EndTime { get; set; }

        public string StudySession { get; set; } = string.Empty;
    }


}

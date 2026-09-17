using System.Security.Claims;
using CourseScheduleSystem.Web.Data;
using CourseScheduleSystem.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CourseScheduleSystem.Web.Pages.Student;

[Authorize(Roles = "Student")]
public class MyCoursesModel : PageModel
{
    private readonly ApplicationDbContext _db;

    public MyCoursesModel(ApplicationDbContext db)
    {
        _db = db;
    }

    public CourseScheduleSystem.Web.Models.Student? CurrentStudent { get; private set; }

    public List<CourseItem> Courses { get; private set; } = new();

    public int TotalCourses { get; private set; }

    public int EnrolledCourses { get; private set; }

    public int PendingCourses { get; private set; }

    public int TotalCredits { get; private set; }

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
            .ToListAsync();

        TotalCourses = Courses.Count;

        EnrolledCourses = Courses.Count(c =>
            c.Status == EnrollmentStatus.Enrolled);

        PendingCourses = Courses.Count(c =>
            c.Status == EnrollmentStatus.Pending);

        TotalCredits = Courses
            .Where(c =>
                c.Status == EnrollmentStatus.Enrolled)
            .Sum(c => c.Credits);
    }

    public class CourseItem
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
}
using System.Security.Claims;
using CourseScheduleSystem.Web.Data;
using CourseScheduleSystem.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CourseScheduleSystem.Web.Pages;

[Authorize(Roles = "Student")]
public class CoursesModel : PageModel
{
    private readonly ApplicationDbContext _db;

    public CoursesModel(ApplicationDbContext db)
    {
        _db = db;
    }

    public CourseScheduleSystem.Web.Models.Student? CurrentStudent { get; set; }

    public List<CourseItem> Courses { get; set; } = new();

    public int TotalCourses { get; private set; }

    public int EnrolledCourses { get; private set; }

    public int PendingCourses { get; private set; }

    public int ClosedCourses { get; private set; }

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
                s.Id == studentId);

        if (CurrentStudent == null)
        {
            return;
        }

        Courses = await _db.Enrollments
            .AsNoTracking()
            .Where(e =>
                e.StudentId == studentId)
            .OrderByDescending(e => e.RequestedAt)
            .Select(e => new CourseItem
            {
                EnrollmentId = e.Id,
                CourseId = e.CourseId,
                Code = e.Course.Code,
                Name = e.Course.Name,
                Description = e.Course.Description,
                Credits = e.Course.Credits,
                LecturerName =
                    e.Course.Lecturer != null
                        ? e.Course.Lecturer.FullName
                        : "Not assigned",
                Status = e.Status,
                RequestedAt = e.RequestedAt,
                ApprovedAt = e.ApprovedAt
            })
            .ToListAsync();

        TotalCourses = Courses.Count;

        EnrolledCourses = Courses.Count(c =>
            c.Status == EnrollmentStatus.Enrolled);

        PendingCourses = Courses.Count(c =>
            c.Status == EnrollmentStatus.Pending);

        ClosedCourses = Courses.Count(c =>
            c.Status == EnrollmentStatus.Closed);

        TotalCredits = Courses
            .Where(c =>
                c.Status == EnrollmentStatus.Enrolled)
            .Sum(c => c.Credits);
    }

    public class CourseItem
    {
        public int EnrollmentId { get; set; }

        public int CourseId { get; set; }

        public string Code { get; set; } =
            string.Empty;

        public string Name { get; set; } =
            string.Empty;

        public string? Description { get; set; }

        public int Credits { get; set; }

        public string LecturerName { get; set; } =
            string.Empty;

        public EnrollmentStatus Status { get; set; }

        public DateTime RequestedAt { get; set; }

        public DateTime? ApprovedAt { get; set; }
    }
}
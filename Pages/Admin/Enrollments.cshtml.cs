using CourseScheduleSystem.Web.Data;
using CourseScheduleSystem.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CourseScheduleSystem.Web.Pages.Admin;

[Authorize(Roles = "Administrator")]
public class EnrollmentsModel : PageModel
{
    private readonly ApplicationDbContext _context;


public EnrollmentsModel(
    ApplicationDbContext context)
    {
        _context = context;
    }

    public List<CourseEnrollmentViewModel> Courses { get; set; } = new();

    public int TotalCourses { get; set; }

    public int TotalEnrollments { get; set; }

    public int EnrolledCount { get; set; }

    public int PendingCount { get; set; }

    public int ClosedCount { get; set; }

    public string? ErrorMessage { get; set; }

    public async Task OnGetAsync()
    {
        try
        {
            var courses =
                await _context.Courses
                    .AsNoTracking()
                    .Include(c => c.Lecturer)
                    .Include(c => c.Enrollments)
                        .ThenInclude(e => e.Student)
                    .Where(c => c.IsActive)
                    .OrderBy(c => c.Code)
                    .ToListAsync();

            TotalCourses = courses.Count;

            TotalEnrollments =
                courses.Sum(c => c.Enrollments.Count);

            EnrolledCount =
                courses.Sum(c =>
                    c.Enrollments.Count(e =>
                        e.Status == EnrollmentStatus.Enrolled));

            PendingCount =
                courses.Sum(c =>
                    c.Enrollments.Count(e =>
                        e.Status == EnrollmentStatus.Pending));

            ClosedCount =
                courses.Sum(c =>
                    c.Enrollments.Count(e =>
                        e.Status == EnrollmentStatus.Closed));

            Courses =
                courses
                    .Select(c =>
                        new CourseEnrollmentViewModel
                        {
                            Id = c.Id,
                            Code = c.Code,
                            Name = c.Name,
                            LecturerName =
                                c.Lecturer?.FullName ?? string.Empty,

                            EnrolledCount =
                                c.Enrollments.Count(e =>
                                    e.Status ==
                                    EnrollmentStatus.Enrolled),

                            Students =
                                c.Enrollments
                                    .OrderBy(e => e.Student.FullName)
                                    .Select(e =>
                                        new EnrollmentStudentViewModel
                                        {
                                            RegNo =
                                                e.Student.RegNo,

                                            FullName =
                                                e.Student.FullName,

                                            Email =
                                                e.Student.Email,

                                            Status =
                                                e.Status.ToString(),

                                            RequestedAt =
                                                e.RequestedAt,

                                            ApprovedAt =
                                                e.ApprovedAt
                                        })
                                    .ToList()
                        })
                    .ToList();
        }
        catch (Exception)
        {
            ErrorMessage =
                "The enrollment information could not be loaded.";

            Courses = new();
        }
    }

    public class CourseEnrollmentViewModel
    {
        public int Id { get; set; }

        public string Code { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string LecturerName { get; set; } = string.Empty;

        public int EnrolledCount { get; set; }

        public List<EnrollmentStudentViewModel> Students { get; set; } =
            new();
    }

    public class EnrollmentStudentViewModel
    {
        public string RegNo { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public DateTime RequestedAt { get; set; }

        public DateTime? ApprovedAt { get; set; }
    }


}

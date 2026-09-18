using System.Security.Claims;
using CourseScheduleSystem.Web.Data;
using CourseScheduleSystem.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CourseScheduleSystem.Web.Pages.Student;

[Authorize(Roles = "Student")]
public class ProfileModel : PageModel
{
    private readonly ApplicationDbContext _db;

    public ProfileModel(ApplicationDbContext db)
    {
        _db = db;
    }

    public StudentProfile? Profile { get; private set; }

    public int EnrolledCourses { get; private set; }

    public int TotalCredits { get; private set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var userIdValue =
            User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdValue, out var studentId))
        {
            return RedirectToPage("/Account/Login");
        }

        var student = await _db.Students
            .AsNoTracking()
            .FirstOrDefaultAsync(s =>
                s.Id == studentId &&
                s.IsActive);

        if (student == null)
        {
            return Forbid();
        }

        Profile = new StudentProfile
        {
            Id = student.Id,
            FullName = student.FullName,
            RegNo = student.RegNo,
            Email = student.Email,
            IsActive = student.IsActive
        };

        var enrolledCourses = await _db.Enrollments
            .AsNoTracking()
            .Where(e =>
                e.StudentId == studentId &&
                e.Status == EnrollmentStatus.Enrolled)
            .Select(e => e.Course)
            .ToListAsync();

        EnrolledCourses = enrolledCourses.Count;

        TotalCredits = enrolledCourses
            .Sum(c => c.Credits);

        return Page();
    }

    public class StudentProfile
    {
        public int Id { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string RegNo { get; set; } = string.Empty;

        public string? Email { get; set; }

        public bool IsActive { get; set; }
    }
}

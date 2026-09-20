using System.Security.Claims;
using CourseScheduleSystem.Web.Data;
using CourseScheduleSystem.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CourseScheduleSystem.Web.Pages.Student;

[Authorize(Roles = "Student")]
public class AvailableCoursesModel : PageModel
{
    private readonly ApplicationDbContext _db;


public AvailableCoursesModel(ApplicationDbContext db)
    {
        _db = db;
    }

    public CourseScheduleSystem.Web.Models.Student? CurrentStudent { get; private set; }

    public List<CourseItem> Courses { get; private set; } = new();

    public int AvailableCourseCount { get; private set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var studentId = GetStudentId();

        if (studentId == null)
        {
            return RedirectToPage("/Account/Login");
        }

        CurrentStudent = await _db.Students
            .AsNoTracking()
            .FirstOrDefaultAsync(s =>
                s.Id == studentId.Value &&
                s.IsActive);

        if (CurrentStudent == null)
        {
            return Forbid();
        }

        var now = DateTime.UtcNow;

        var enrollments = await _db.Enrollments
            .AsNoTracking()
            .Where(e => e.StudentId == studentId.Value)
            .Select(e => new
            {
                e.CourseId,
                e.Status
            })
            .ToListAsync();

        var enrollmentLookup = enrollments
            .GroupBy(e => e.CourseId)
            .ToDictionary(
                g => g.Key,
                g => g.First().Status);

        var courses = await _db.Courses
            .AsNoTracking()
            .Where(c =>
                c.IsActive &&
                c.Status == CourseStatus.Available)
            .OrderBy(c => c.Code)
            .Select(c => new CourseItem
            {
                Id = c.Id,
                Code = c.Code,
                Name = c.Name,
                Description = c.Description,
                Credits = c.Credits,
                StudySession = c.StudySession,
                LecturerName =
                    c.Lecturer != null &&
                    !string.IsNullOrWhiteSpace(c.Lecturer.FullName)
                        ? c.Lecturer.FullName
                        : "Lecturer not assigned",
                RegistrationOpenDate = c.RegistrationOpenDate,
                RegistrationCloseDate = c.RegistrationCloseDate,
                StartDate = c.StartDate,
                EndDate = c.EndDate
            })
            .ToListAsync();

        Courses = courses
            .Where(c => IsRegistrationOpen(c, now))
            .ToList();

        foreach (var course in Courses)
        {
            if (enrollmentLookup.TryGetValue(
                    course.Id,
                    out var enrollmentStatus))
            {
                course.EnrollmentStatus = enrollmentStatus;
            }
        }

        AvailableCourseCount = Courses.Count;

        return Page();
    }

    public async Task<IActionResult> OnPostEnrollAsync(int courseId)
    {
        var studentId = GetStudentId();

        if (studentId == null)
        {
            return RedirectToPage("/Account/Login");
        }

        var student = await _db.Students
            .FirstOrDefaultAsync(s =>
                s.Id == studentId.Value &&
                s.IsActive);

        if (student == null)
        {
            return Forbid();
        }

        var course = await _db.Courses
            .FirstOrDefaultAsync(c =>
                c.Id == courseId &&
                c.IsActive);

        if (course == null)
        {
            TempData["EnrollmentError"] =
                "The selected course could not be found.";

            return RedirectToPage();
        }

        var now = DateTime.UtcNow;

        if (course.Status != CourseStatus.Available)
        {
            TempData["EnrollmentError"] =
                "This course is not currently available for enrollment.";

            return RedirectToPage();
        }

        if (!IsRegistrationOpen(course, now))
        {
            TempData["EnrollmentError"] =
                GetRegistrationClosedMessage(course, now);

            return RedirectToPage();
        }

        var existingEnrollment = await _db.Enrollments
            .FirstOrDefaultAsync(e =>
                e.StudentId == studentId.Value &&
                e.CourseId == courseId);

        if (existingEnrollment != null)
        {
            if (existingEnrollment.Status == EnrollmentStatus.Enrolled)
            {
                TempData["EnrollmentInfo"] =
                    "You are already enrolled in this course.";
            }
            else if (existingEnrollment.Status == EnrollmentStatus.Pending)
            {
                TempData["EnrollmentInfo"] =
                    "You have already requested this course.";
            }
            else
            {
                TempData["EnrollmentInfo"] =
                    "You already have an enrollment record for this course.";
            }

            return RedirectToPage();
        }

        var enrollment = new Enrollment
        {
            StudentId = studentId.Value,
            CourseId = courseId,
            Status = EnrollmentStatus.Enrolled,
            RequestedAt = now,
            ApprovedAt = now
        };

        _db.Enrollments.Add(enrollment);

        try
        {
            await _db.SaveChangesAsync();

            TempData["EnrollmentSuccess"] =
                $"You have successfully enrolled in {course.Code} - {course.Name}.";
        }
        catch (DbUpdateException)
        {
            TempData["EnrollmentError"] =
                "The enrollment could not be completed. The course may have already been added to your account.";

            return RedirectToPage();
        }

        return RedirectToPage();
    }

    private int? GetStudentId()
    {
        var userIdValue =
            User.FindFirstValue(ClaimTypes.NameIdentifier);

        return int.TryParse(userIdValue, out var studentId)
            ? studentId
            : null;
    }

    private static bool IsRegistrationOpen(
        CourseItem course,
        DateTime now)
    {
        return IsRegistrationOpen(
            course.RegistrationOpenDate,
            course.RegistrationCloseDate,
            now);
    }

    private static bool IsRegistrationOpen(
        Course course,
        DateTime now)
    {
        return IsRegistrationOpen(
            course.RegistrationOpenDate,
            course.RegistrationCloseDate,
            now);
    }

    private static bool IsRegistrationOpen(
        DateTime? registrationOpenDate,
        DateTime? registrationCloseDate,
        DateTime now)
    {
        if (!registrationOpenDate.HasValue)
        {
            return false;
        }

        var openDate = registrationOpenDate.Value;

        var maximumCloseDate = openDate.AddDays(2);

        var effectiveCloseDate =
            registrationCloseDate.HasValue &&
            registrationCloseDate.Value < maximumCloseDate
                ? registrationCloseDate.Value
                : maximumCloseDate;

        return now >= openDate &&
               now <= effectiveCloseDate;
    }

    private static string GetRegistrationClosedMessage(
        Course course,
        DateTime now)
    {
        if (!course.RegistrationOpenDate.HasValue)
        {
            return "Registration for this course has not been opened yet.";
        }

        if (now < course.RegistrationOpenDate.Value)
        {
            return $"Registration opens on {course.RegistrationOpenDate.Value:dd MMM yyyy, HH:mm}.";
        }

        var maximumCloseDate =
            course.RegistrationOpenDate.Value.AddDays(2);

        var effectiveCloseDate =
            course.RegistrationCloseDate.HasValue &&
            course.RegistrationCloseDate.Value < maximumCloseDate
                ? course.RegistrationCloseDate.Value
                : maximumCloseDate;

        if (now > effectiveCloseDate)
        {
            return $"Registration for this course closed on {effectiveCloseDate:dd MMM yyyy, HH:mm}.";
        }

        return "Registration for this course is currently unavailable.";
    }

    public class CourseItem
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Credits { get; set; }
        public StudySession StudySession { get; set; }
        public string LecturerName { get; set; } = string.Empty;
        public DateTime? RegistrationOpenDate { get; set; }
        public DateTime? RegistrationCloseDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public EnrollmentStatus? EnrollmentStatus { get; set; }
    }


}

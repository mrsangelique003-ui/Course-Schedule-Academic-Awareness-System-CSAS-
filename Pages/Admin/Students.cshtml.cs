using CourseScheduleSystem.Web.Data;
using CourseScheduleSystem.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CourseScheduleSystem.Web.Pages.Admin;

[Authorize(Roles = "Administrator")]
public class StudentsModel : PageModel
{
    private readonly ApplicationDbContext _db;


public StudentsModel(ApplicationDbContext db)
    {
        _db = db;
    }

    [BindProperty(SupportsGet = true)]
    public string? Search { get; set; }

    [BindProperty(SupportsGet = true)]
    public string Status { get; set; } = "All";

    public int TotalStudents { get; private set; }

    public int ActiveStudents { get; private set; }

    public int InactiveStudents { get; private set; }

    public int StudentsWithEnrollments { get; private set; }

    public List<StudentRow> Students { get; private set; } = new();

    public async Task OnGetAsync()
    {
        var studentsQuery = _db.Students
            .AsNoTracking()
            .AsQueryable();

        TotalStudents = await _db.Students
            .AsNoTracking()
            .CountAsync();

        ActiveStudents = await _db.Students
            .AsNoTracking()
            .CountAsync(s => s.IsActive);

        InactiveStudents = TotalStudents - ActiveStudents;

        StudentsWithEnrollments = await _db.Students
            .AsNoTracking()
            .CountAsync(s =>
                s.Enrollments.Any(e =>
                    e.Status == EnrollmentStatus.Enrolled));

        if (!string.IsNullOrWhiteSpace(Search))
        {
            var searchTerm = Search.Trim().ToLower();

            studentsQuery = studentsQuery.Where(s =>
                s.RegNo.ToLower().Contains(searchTerm) ||
                s.FullName.ToLower().Contains(searchTerm) ||
                (s.Email != null &&
                 s.Email.ToLower().Contains(searchTerm)) ||
                s.Department.ToLower().Contains(searchTerm) ||
                (s.Program != null &&
                 s.Program.ToLower().Contains(searchTerm)));
        }

        Status = Status?.Trim() ?? "All";

        if (Status.Equals("Active", StringComparison.OrdinalIgnoreCase))
        {
            studentsQuery = studentsQuery.Where(s => s.IsActive);
            Status = "Active";
        }
        else if (Status.Equals("Inactive", StringComparison.OrdinalIgnoreCase))
        {
            studentsQuery = studentsQuery.Where(s => !s.IsActive);
            Status = "Inactive";
        }
        else
        {
            Status = "All";
        }

        Students = await studentsQuery
            .OrderBy(s => s.FullName)
            .Select(s => new StudentRow
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
                EnrollmentCount = s.Enrollments.Count(
                    e => e.Status == EnrollmentStatus.Enrolled)
            })
            .ToListAsync();
    }

    public class StudentRow
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

        public int EnrollmentCount { get; set; }
    }


}

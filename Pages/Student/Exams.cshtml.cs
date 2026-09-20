using CourseScheduleSystem.Web.Data;
using CourseScheduleSystem.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CourseScheduleSystem.Web.Pages.Student;

[Authorize(Roles = "Student")]
public class ExamsModel : PageModel
{
    private readonly ApplicationDbContext _db;


public ExamsModel(ApplicationDbContext db)
    {
        _db = db;
    }

    public CourseScheduleSystem.Web.Models.Student? CurrentStudent { get; private set; }

    public List<Exam> Exams { get; private set; } = new();

    public DateTime CurrentMonth { get; private set; }

    public string Search { get; private set; } = string.Empty;

    public int TotalExams { get; private set; }

    public int CatCount { get; private set; }

    public int FinalCount { get; private set; }

    public int UpcomingCount { get; private set; }

    public int CompletedCount { get; private set; }

    public async Task<IActionResult> OnGetAsync(
        int? year,
        int? month,
        string? search)
    {
        var studentIdClaim =
            User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(studentIdClaim, out var studentId))
        {
            return RedirectToPage("/Account/Login");
        }

        CurrentStudent =
            await _db.Students
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    s => s.Id == studentId && s.IsActive);

        if (CurrentStudent == null)
        {
            return RedirectToPage("/Account/Login");
        }

        var today = DateTime.Today;

        var selectedYear =
            year ?? today.Year;

        var selectedMonth =
            month ?? today.Month;

        if (selectedMonth < 1 || selectedMonth > 12)
        {
            selectedMonth = today.Month;
        }

        CurrentMonth =
            new DateTime(
                selectedYear,
                selectedMonth,
                1);

        Search =
            search?.Trim() ?? string.Empty;

        var enrolledCourseIds =
            await _db.Enrollments
                .AsNoTracking()
                .Where(e =>
                    e.StudentId == studentId &&
                    e.Status == EnrollmentStatus.Enrolled)
                .Select(e => e.CourseId)
                .ToListAsync();

        var monthStart =
            new DateTime(
                CurrentMonth.Year,
                CurrentMonth.Month,
                1);

        var monthEnd =
            monthStart.AddMonths(1);

        var query =
            _db.Exams
                .AsNoTracking()
                .Include(e => e.Course)
                .Include(e => e.Room)
                .Where(e =>
                    e.IsActive &&
                    enrolledCourseIds.Contains(e.CourseId) &&
                    e.ExamDate >= monthStart &&
                    e.ExamDate < monthEnd);

        if (!string.IsNullOrWhiteSpace(Search))
        {
            var searchTerm =
                Search.ToLower();

            query =
                query.Where(e =>
                    e.Course.Code.ToLower()
                        .Contains(searchTerm) ||
                    e.Course.Name.ToLower()
                        .Contains(searchTerm));
        }

        Exams =
            await query
                .OrderBy(e => e.ExamDate)
                .ThenBy(e => e.StartTime)
                .ThenBy(e => e.ExamType)
                .ToListAsync();

        TotalExams =
            Exams.Count;

        CatCount =
            Exams.Count(e =>
                e.ExamType == ExamType.CAT);

        FinalCount =
            Exams.Count(e =>
                e.ExamType == ExamType.Final);

        var currentDate =
            DateTime.Today;

        UpcomingCount =
            Exams.Count(e =>
                e.ExamDate.Date > currentDate ||
                (
                    e.ExamDate.Date == currentDate &&
                    e.EndTime > TimeOnly.FromDateTime(
                        DateTime.Now)
                ));

        CompletedCount =
            Exams.Count(e =>
                e.ExamDate.Date < currentDate ||
                (
                    e.ExamDate.Date == currentDate &&
                    e.EndTime <= TimeOnly.FromDateTime(
                        DateTime.Now)
                ));

        return Page();
    }

    public string GetExamStatus(Exam exam)
    {
        var today =
            DateTime.Today;

        var now =
            TimeOnly.FromDateTime(
                DateTime.Now);

        if (exam.ExamDate.Date < today)
        {
            return "Completed";
        }

        if (exam.ExamDate.Date > today)
        {
            return "Upcoming";
        }

        if (now < exam.StartTime)
        {
            return "Upcoming";
        }

        if (now >= exam.StartTime &&
            now < exam.EndTime)
        {
            return "Ongoing";
        }

        return "Completed";
    }

    public string GetExamStatusClass(Exam exam)
    {
        return GetExamStatus(exam) switch
        {
            "Ongoing" => "status-ongoing",
            "Completed" => "status-completed",
            _ => "status-upcoming"
        };
    }

    public string GetExamTypeClass(ExamType examType)
    {
        return examType switch
        {
            ExamType.CAT => "exam-cat",
            ExamType.Final => "exam-final",
            _ => "exam-default"
        };
    }

    public string GetStudySession(Exam exam)
    {
        return exam.Course.StudySession.ToString();
    }

    public string GetRoomName(Exam exam)
    {
        if (exam.Room == null)
        {
            return "Room not assigned";
        }

        return string.IsNullOrWhiteSpace(exam.Room.Building)
            ? exam.Room.RoomNumber
            : $"{exam.Room.Building} — {exam.Room.RoomNumber}";
    }

    public string GetPreviousMonthUrl()
    {
        var previousMonth =
            CurrentMonth.AddMonths(-1);

        return BuildMonthUrl(
            previousMonth.Year,
            previousMonth.Month);
    }

    public string GetNextMonthUrl()
    {
        var nextMonth =
            CurrentMonth.AddMonths(1);

        return BuildMonthUrl(
            nextMonth.Year,
            nextMonth.Month);
    }

    public string GetTodayUrl()
    {
        var today =
            DateTime.Today;

        return BuildMonthUrl(
            today.Year,
            today.Month);
    }

    private string BuildMonthUrl(
        int year,
        int month)
    {
        var queryParts =
            new List<string>
            {
            $"year={year}",
            $"month={month}"
            };

        if (!string.IsNullOrWhiteSpace(Search))
        {
            queryParts.Add(
                $"search={Uri.EscapeDataString(Search)}");
        }

        return $"?{string.Join("&", queryParts)}";
    }


}

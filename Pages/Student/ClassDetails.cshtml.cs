using System.Security.Claims;
using CourseScheduleSystem.Web.Data;
using CourseScheduleSystem.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CourseScheduleSystem.Web.Pages.Student;

[Authorize(Roles = "Student")]
public class ClassDetailsModel : PageModel
{
    private readonly ApplicationDbContext _db;


public ClassDetailsModel(ApplicationDbContext db)
    {
        _db = db;
    }

    [BindProperty(SupportsGet = true)]
    public string Session { get; set; } = "All";

    [BindProperty(SupportsGet = true)]
    public string Day { get; set; } = "All";

    [BindProperty(SupportsGet = true)]
    public string? Search { get; set; }

    public RoomDetails? Room { get; private set; }

    public List<DayGroup> WeeklyTimetable { get; private set; } = new();

    public int TotalFilteredClasses =>
        WeeklyTimetable.Sum(day => day.Classes.Count);

    public List<string> Days { get; } = new()
{
    "Monday",
    "Tuesday",
    "Wednesday",
    "Thursday",
    "Friday",
    "Saturday",
    "Sunday"
};

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var userIdValue =
            User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdValue, out var studentId))
        {
            return RedirectToPage("/Account/Login");
        }

        var studentExists = await _db.Students
            .AsNoTracking()
            .AnyAsync(s =>
                s.Id == studentId &&
                s.IsActive);

        if (!studentExists)
        {
            return RedirectToPage("/Account/Login");
        }

        Room = await _db.Rooms
            .AsNoTracking()
            .Where(r => r.Id == id)
            .Select(r => new RoomDetails
            {
                Id = r.Id,
                RoomNumber = r.RoomNumber,
                Building = r.Building,
                Capacity = r.Capacity,
                RoomType = r.RoomType,
                IsAvailable = r.IsAvailable
            })
            .FirstOrDefaultAsync();

        if (Room == null)
        {
            return Page();
        }

        Session = NormalizeSession(Session);
        Day = NormalizeDay(Day);

        var scheduleEntries = await _db.ScheduleEntries
            .AsNoTracking()
            .Where(s =>
                s.RoomId == id &&
                s.IsActive &&
                s.Status == ScheduleStatus.Active)
            .OrderBy(s => s.DayOfWeek)
            .ThenBy(s => s.StartTime)
            .Select(s => new ScheduleData
            {
                Id = s.Id,
                CourseId = s.CourseId,
                CourseCode = s.Course.Code,
                CourseName = s.Course.Name,

                LecturerName =
                    s.Lecturer != null &&
                    !string.IsNullOrWhiteSpace(s.Lecturer.FullName)
                        ? s.Lecturer.FullName
                        : "Lecturer not assigned",

                LecturerPhoneNumber =
                    s.Lecturer != null
                        ? s.Lecturer.PhoneNumber
                        : null,

                RoomNumber = s.Room.RoomNumber,
                Building = s.Room.Building,

                DayOfWeek = s.DayOfWeek,
                StartTime = s.StartTime,
                EndTime = s.EndTime,

                StudySession = s.StudySession,

                ClassRepresentativeId =
                    s.ClassRepresentativeId,

                ClassRepresentativeName =
                    s.ClassRepresentative != null &&
                    !string.IsNullOrWhiteSpace(
                        s.ClassRepresentative.FullName)
                        ? s.ClassRepresentative.FullName
                        : "Not assigned",

                ClassRepresentativeRegNo =
                    s.ClassRepresentative != null &&
                    !string.IsNullOrWhiteSpace(
                        s.ClassRepresentative.RegNo)
                        ? s.ClassRepresentative.RegNo
                        : "—",

                ClassRepresentativePhoneNumber =
                    s.ClassRepresentative != null &&
                    !string.IsNullOrWhiteSpace(
                        s.ClassRepresentative.PhoneNumber)
                        ? s.ClassRepresentative.PhoneNumber
                        : "Phone not available",

                Intake =
                    s.ClassRepresentative != null &&
                    !string.IsNullOrWhiteSpace(
                        s.ClassRepresentative.Intake)
                        ? s.ClassRepresentative.Intake
                        : "Intake not assigned",

                Level =
                    s.ClassRepresentative != null &&
                    !string.IsNullOrWhiteSpace(
                        s.ClassRepresentative.Level)
                        ? s.ClassRepresentative.Level
                        : "Level not assigned",

                Notes = s.Notes
            })
            .ToListAsync();

        var courseIds = scheduleEntries
            .Select(s => s.CourseId)
            .Distinct()
            .ToList();

        var classRepresentativeIds = scheduleEntries
            .Where(s => s.ClassRepresentativeId.HasValue)
            .Select(s => s.ClassRepresentativeId!.Value)
            .Distinct()
            .ToList();

        var classGroups = await _db.ClassGroups
            .AsNoTracking()
            .Where(g =>
                courseIds.Contains(g.CourseId) &&
                classRepresentativeIds.Contains(
                    g.ClassRepresentativeId))
            .Select(g => new ClassGroupData
            {
                CourseId = g.CourseId,
                ClassRepresentativeId =
                    g.ClassRepresentativeId,
                Intake = g.Intake,
                Level = g.Level,
                GroupLink = g.GroupLink
            })
            .ToListAsync();

        var filteredEntries = scheduleEntries
            .Where(s =>
                MatchesSession(s.StudySession) &&
                MatchesDay(s.DayOfWeek) &&
                MatchesSearch(s))
            .ToList();

        var detailItems = filteredEntries
            .Select(s =>
            {
                var groupLink = classGroups
                    .Where(g =>
                        g.CourseId == s.CourseId &&
                        s.ClassRepresentativeId.HasValue &&
                        g.ClassRepresentativeId ==
                            s.ClassRepresentativeId.Value)
                    .Where(g =>
                        string.Equals(
                            g.Intake,
                            s.Intake,
                            StringComparison.OrdinalIgnoreCase) &&
                        string.Equals(
                            g.Level,
                            s.Level,
                            StringComparison.OrdinalIgnoreCase))
                    .Select(g => g.GroupLink)
                    .FirstOrDefault();

                return new ScheduleDetailItem
                {
                    Id = s.Id,
                    CourseId = s.CourseId,
                    CourseCode = s.CourseCode,
                    CourseName = s.CourseName,
                    LecturerName = s.LecturerName,
                    LecturerPhoneNumber =
                        s.LecturerPhoneNumber,
                    RoomNumber = s.RoomNumber,
                    Building = s.Building,
                    DayOfWeek = s.DayOfWeek,
                    StartTime = s.StartTime,
                    EndTime = s.EndTime,
                    StudySession = s.StudySession,
                    Intake = s.Intake,
                    Level = s.Level,
                    ClassRepresentativeName =
                        s.ClassRepresentativeName,
                    ClassRepresentativeRegNo =
                        s.ClassRepresentativeRegNo,
                    ClassRepresentativePhoneNumber =
                        s.ClassRepresentativePhoneNumber,
                    WhatsAppGroupLink = groupLink,
                    Notes = s.Notes
                };
            })
            .ToList();

        WeeklyTimetable = BuildWeeklyTimetable(detailItems);

        return Page();
    }

    private bool MatchesSession(StudySession session)
    {
        if (Session == "All")
        {
            return true;
        }

        return string.Equals(
            session.ToString(),
            Session,
            StringComparison.OrdinalIgnoreCase);
    }

    private bool MatchesDay(DayOfWeek day)
    {
        if (Day == "All")
        {
            return true;
        }

        return string.Equals(
            day.ToString(),
            Day,
            StringComparison.OrdinalIgnoreCase);
    }

    private bool MatchesSearch(ScheduleData item)
    {
        if (string.IsNullOrWhiteSpace(Search))
        {
            return true;
        }

        var search = Search.Trim();

        return Contains(item.CourseCode, search) ||
               Contains(item.CourseName, search) ||
               Contains(item.LecturerName, search) ||
               Contains(item.Intake, search) ||
               Contains(item.Level, search) ||
               Contains(
                   item.ClassRepresentativeName,
                   search) ||
               Contains(
                   item.ClassRepresentativeRegNo,
                   search) ||
               Contains(
                   item.ClassRepresentativePhoneNumber,
                   search);
    }

    private static bool Contains(
        string? value,
        string search)
    {
        return !string.IsNullOrWhiteSpace(value) &&
               value.Contains(
                   search,
                   StringComparison.OrdinalIgnoreCase);
    }

    private static string NormalizeSession(string? session)
    {
        if (string.Equals(
                session,
                "Day",
                StringComparison.OrdinalIgnoreCase))
        {
            return "Day";
        }

        if (string.Equals(
                session,
                "Evening",
                StringComparison.OrdinalIgnoreCase))
        {
            return "Evening";
        }

        if (string.Equals(
                session,
                "Weekend",
                StringComparison.OrdinalIgnoreCase))
        {
            return "Weekend";
        }

        return "All";
    }

    private string NormalizeDay(string? day)
    {
        if (!string.IsNullOrWhiteSpace(day) &&
            Days.Any(d =>
                string.Equals(
                    d,
                    day,
                    StringComparison.OrdinalIgnoreCase)))
        {
            return Days.First(d =>
                string.Equals(
                    d,
                    day,
                    StringComparison.OrdinalIgnoreCase));
        }

        return "All";
    }

    private static List<DayGroup> BuildWeeklyTimetable(
        List<ScheduleDetailItem> items)
    {
        var dayOrder = new[]
        {
        DayOfWeek.Monday,
        DayOfWeek.Tuesday,
        DayOfWeek.Wednesday,
        DayOfWeek.Thursday,
        DayOfWeek.Friday,
        DayOfWeek.Saturday,
        DayOfWeek.Sunday
    };

        return dayOrder
            .Select(day => new DayGroup
            {
                Day = day,
                DayName = day.ToString(),
                Classes = items
                    .Where(item => item.DayOfWeek == day)
                    .OrderBy(item => item.StartTime)
                    .ThenBy(item => item.CourseCode)
                    .ToList()
            })
            .Where(group => group.Classes.Count > 0)
            .ToList();
    }

    public sealed class RoomDetails
    {
        public int Id { get; set; }

        public string RoomNumber { get; set; } =
            string.Empty;

        public string Building { get; set; } =
            string.Empty;

        public int Capacity { get; set; }

        public string? RoomType { get; set; }

        public bool IsAvailable { get; set; }
    }

    private sealed class ScheduleData
    {
        public int Id { get; set; }

        public int CourseId { get; set; }

        public string CourseCode { get; set; } =
            string.Empty;

        public string CourseName { get; set; } =
            string.Empty;

        public string LecturerName { get; set; } =
            string.Empty;

        public string? LecturerPhoneNumber { get; set; }

        public string RoomNumber { get; set; } =
            string.Empty;

        public string Building { get; set; } =
            string.Empty;

        public DayOfWeek DayOfWeek { get; set; }

        public TimeOnly StartTime { get; set; }

        public TimeOnly EndTime { get; set; }

        public StudySession StudySession { get; set; }

        public int? ClassRepresentativeId { get; set; }

        public string ClassRepresentativeName { get; set; } =
            string.Empty;

        public string ClassRepresentativeRegNo { get; set; } =
            string.Empty;

        public string ClassRepresentativePhoneNumber { get; set; } =
            string.Empty;

        public string Intake { get; set; } =
            string.Empty;

        public string Level { get; set; } =
            string.Empty;

        public string? Notes { get; set; }
    }

    private sealed class ClassGroupData
    {
        public int CourseId { get; set; }

        public int ClassRepresentativeId { get; set; }

        public string Intake { get; set; } =
            string.Empty;

        public string Level { get; set; } =
            string.Empty;

        public string GroupLink { get; set; } =
            string.Empty;
    }

    public sealed class ScheduleDetailItem
    {
        public int Id { get; set; }

        public int CourseId { get; set; }

        public string CourseCode { get; set; } =
            string.Empty;

        public string CourseName { get; set; } =
            string.Empty;

        public string LecturerName { get; set; } =
            string.Empty;

        public string? LecturerPhoneNumber { get; set; }

        public string RoomNumber { get; set; } =
            string.Empty;

        public string Building { get; set; } =
            string.Empty;

        public DayOfWeek DayOfWeek { get; set; }

        public TimeOnly StartTime { get; set; }

        public TimeOnly EndTime { get; set; }

        public StudySession StudySession { get; set; }

        public string Intake { get; set; } =
            string.Empty;

        public string Level { get; set; } =
            string.Empty;

        public string ClassRepresentativeName { get; set; } =
            string.Empty;

        public string ClassRepresentativeRegNo { get; set; } =
            string.Empty;

        public string ClassRepresentativePhoneNumber { get; set; } =
            string.Empty;

        public string? WhatsAppGroupLink { get; set; }

        public string? Notes { get; set; }

        public string SessionClass =>
            StudySession switch
            {
                StudySession.Day => "session-day",
                StudySession.Evening => "session-evening",
                StudySession.Weekend => "session-weekend",
                _ => "session-default"
            };
    }

    public sealed class DayGroup
    {
        public DayOfWeek Day { get; set; }

        public string DayName { get; set; } =
            string.Empty;

        public List<ScheduleDetailItem> Classes { get; set; } =
            new();
    }


}

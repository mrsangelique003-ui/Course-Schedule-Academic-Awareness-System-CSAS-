using CourseScheduleSystem.Web.Data;
using CourseScheduleSystem.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CourseScheduleSystem.Web.Pages.Admin;

[Authorize(Roles = "Administrator")]
public class SchedulesModel : PageModel
{
    private readonly ApplicationDbContext _context;


public SchedulesModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public DateTime Today { get; private set; }

    public TimeOnly CurrentTime { get; private set; }

    public DayOfWeek CurrentDay { get; private set; }

    public int TotalToday { get; private set; }

    public int OngoingCount { get; private set; }

    public int UpcomingCount { get; private set; }

    public int CompletedCount { get; private set; }

    public List<ScheduleViewModel> OngoingClasses { get; private set; } = new();

    public List<ScheduleViewModel> UpcomingClasses { get; private set; } = new();

    public List<ScheduleViewModel> CompletedClasses { get; private set; } = new();

    public async Task OnGetAsync()
    {
        var now = DateTime.Now;

        Today = now.Date;
        CurrentTime = TimeOnly.FromDateTime(now);
        CurrentDay = now.DayOfWeek;

        var schedules = await _context.ScheduleEntries
            .AsNoTracking()
            .Include(s => s.Course)
            .Include(s => s.Lecturer)
            .Include(s => s.ClassRepresentative)
            .Include(s => s.Room)
            .Where(s =>
                s.IsActive &&
                s.Status == ScheduleStatus.Active &&
                s.DayOfWeek == CurrentDay &&
                s.StartDate.Date <= Today &&
                s.EndDate.Date >= Today)
            .OrderBy(s => s.StartTime)
            .ToListAsync();

        var scheduleViewModels = schedules
            .Select(MapSchedule)
            .ToList();

        TotalToday = scheduleViewModels.Count;

        OngoingClasses = scheduleViewModels
            .Where(s =>
                s.StartTime <= CurrentTime &&
                s.EndTime > CurrentTime)
            .OrderBy(s => s.StartTime)
            .ToList();

        UpcomingClasses = scheduleViewModels
            .Where(s =>
                s.StartTime > CurrentTime)
            .OrderBy(s => s.StartTime)
            .ToList();

        CompletedClasses = scheduleViewModels
            .Where(s =>
                s.EndTime <= CurrentTime)
            .OrderByDescending(s => s.StartTime)
            .ToList();

        OngoingCount = OngoingClasses.Count;
        UpcomingCount = UpcomingClasses.Count;
        CompletedCount = CompletedClasses.Count;
    }

    private static ScheduleViewModel MapSchedule(
        ScheduleEntry schedule)
    {
        return new ScheduleViewModel
        {
            Id = schedule.Id,

            CourseCode =
                schedule.Course?.Code ?? "—",

            CourseName =
                schedule.Course?.Name ?? "Unknown Course",

            LecturerName =
                schedule.Lecturer?.FullName ?? "Not Assigned",

            LecturerStaffId =
                schedule.Lecturer?.StaffId ?? "—",

            Room =
                schedule.Room == null
                    ? "Not Assigned"
                    : $"{schedule.Room.Building} · {schedule.Room.RoomNumber}",

            RoomNumber =
                schedule.Room?.RoomNumber ?? "—",

            ClassRepresentative =
                schedule.ClassRepresentative?.FullName
                ?? "Not Assigned",

            RepresentativeRegNo =
                schedule.ClassRepresentative?.RegNo
                ?? "—",

            Day =
                schedule.DayOfWeek.ToString(),

            StartTime =
                schedule.StartTime,

            EndTime =
                schedule.EndTime,

            StudySession =
                schedule.StudySession.ToString(),

            Notes =
                schedule.Notes
        };
    }

    public class ScheduleViewModel
    {
        public int Id { get; set; }

        public string CourseCode { get; set; } = string.Empty;

        public string CourseName { get; set; } = string.Empty;

        public string LecturerName { get; set; } = string.Empty;

        public string LecturerStaffId { get; set; } = string.Empty;

        public string Room { get; set; } = string.Empty;

        public string RoomNumber { get; set; } = string.Empty;

        public string ClassRepresentative { get; set; } = string.Empty;

        public string RepresentativeRegNo { get; set; } = string.Empty;

        public string Day { get; set; } = string.Empty;

        public TimeOnly StartTime { get; set; }

        public TimeOnly EndTime { get; set; }

        public string StudySession { get; set; } = string.Empty;

        public string? Notes { get; set; }
    }


}

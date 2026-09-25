using CourseScheduleSystem.Web.Data;
using CourseScheduleSystem.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CourseScheduleSystem.Web.Pages.Admin;

[Authorize(Roles = "Administrator,Dean,HOD,DirectorOfQuality")]
public class ClassroomManagementModel : PageModel
{
    private readonly ApplicationDbContext _db;


public ClassroomManagementModel(ApplicationDbContext db)
    {
        _db = db;
    }

    public List<ClassCardViewModel> Classes { get; private set; } = new();

    public int TotalClasses { get; private set; }

    public int AssignedLecturers { get; private set; }

    public int AssignedRepresentatives { get; private set; }

    public int RoomsInUse { get; private set; }

    public List<string> Statuses { get; private set; } = new();

    public async Task OnGetAsync()
    {
        var now = DateTime.Now;
        var today = now.Date;
        var currentDay = now.DayOfWeek;
        var currentTime = TimeOnly.FromDateTime(now);

        var scheduleEntries = await _db.ScheduleEntries
            .AsNoTracking()
            .Include(s => s.Course)
            .Include(s => s.Room)
            .Include(s => s.Lecturer)
            .Include(s => s.ClassRepresentative)
            .OrderBy(s => s.DayOfWeek)
            .ThenBy(s => s.StartTime)
            .ThenBy(s => s.Course.Code)
            .ToListAsync();

        Classes = scheduleEntries
            .Select(s => new ClassCardViewModel
            {
                Id = s.Id,

                CourseCode = s.Course.Code,
                CourseName = s.Course.Name,

                LecturerId = s.LecturerId,
                LecturerName =
                    s.Lecturer?.FullName ?? string.Empty,
                LecturerStaffId =
                    s.Lecturer?.StaffId ?? string.Empty,

                ClassRepresentativeId =
                    s.ClassRepresentativeId,

                RepresentativeName =
                    s.ClassRepresentative?.FullName ?? string.Empty,

                RepresentativeRegNo =
                    s.ClassRepresentative?.RegNo ?? string.Empty,

                RepresentativePhone =
                    s.ClassRepresentative?.PhoneNumber ?? string.Empty,

                RoomNumber = s.Room.RoomNumber,
                Building = s.Room.Building,
                RoomCapacity = s.Room.Capacity,

                Day = s.DayOfWeek.ToString(),

                StartTime =
                    s.StartTime.ToString("HH:mm"),

                EndTime =
                    s.EndTime.ToString("HH:mm"),

                StudySession = s.StudySession,

                Status = s.Status.ToString(),

                Intake = GetCourseIntake(s),

                Level = GetCourseLevel(s),

                Notes = s.Notes,

                IsActive = s.IsActive,

                IsOccupied =
                    s.IsActive &&
                    s.Status == ScheduleStatus.Active &&
                    s.StartDate.Date <= today &&
                    s.EndDate.Date >= today &&
                    s.DayOfWeek == currentDay &&
                    s.StartTime <= currentTime &&
                    s.EndTime > currentTime
            })
            .ToList();

        TotalClasses = Classes.Count;

        AssignedLecturers = Classes
            .Count(c => c.LecturerId.HasValue);

        AssignedRepresentatives = Classes
            .Count(c => c.ClassRepresentativeId.HasValue);

        RoomsInUse = Classes
            .Select(c => $"{c.Building}|{c.RoomNumber}")
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Count();

        Statuses = Classes
            .Select(c => c.Status)
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(s => s)
            .ToList();
    }

    private static string GetCourseIntake(
        ScheduleEntry scheduleEntry)
    {
        if (scheduleEntry.ClassRepresentative != null &&
            !string.IsNullOrWhiteSpace(
                scheduleEntry.ClassRepresentative.Intake))
        {
            return scheduleEntry.ClassRepresentative.Intake;
        }

        return "—";
    }

    private static string GetCourseLevel(
        ScheduleEntry scheduleEntry)
    {
        if (scheduleEntry.ClassRepresentative != null &&
            !string.IsNullOrWhiteSpace(
                scheduleEntry.ClassRepresentative.Level))
        {
            return scheduleEntry.ClassRepresentative.Level;
        }

        return "—";
    }

    public class ClassCardViewModel
    {
        public int Id { get; set; }

        public string CourseCode { get; set; } = string.Empty;

        public string CourseName { get; set; } = string.Empty;

        public int? LecturerId { get; set; }

        public string LecturerName { get; set; } = string.Empty;

        public string LecturerStaffId { get; set; } = string.Empty;

        public int? ClassRepresentativeId { get; set; }

        public string RepresentativeName { get; set; } =
            string.Empty;

        public string RepresentativeRegNo { get; set; } =
            string.Empty;

        public string RepresentativePhone { get; set; } =
            string.Empty;

        public string RoomNumber { get; set; } = string.Empty;

        public string Building { get; set; } = string.Empty;

        public int RoomCapacity { get; set; }

        public string Day { get; set; } = string.Empty;

        public string StartTime { get; set; } = string.Empty;

        public string EndTime { get; set; } = string.Empty;

        public StudySession StudySession { get; set; }

        public string Status { get; set; } = string.Empty;

        public string Intake { get; set; } = string.Empty;

        public string Level { get; set; } = string.Empty;

        public string? Notes { get; set; }

        public bool IsActive { get; set; }

        public bool IsOccupied { get; set; }
    }


}

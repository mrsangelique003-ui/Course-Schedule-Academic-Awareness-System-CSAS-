using System.Security.Claims;
using CourseScheduleSystem.Web.Data;
using CourseScheduleSystem.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CourseScheduleSystem.Web.Pages.Student;

[Authorize(Roles = "Student")]
public class ClassroomsModel : PageModel
{
    private readonly ApplicationDbContext _db;


public ClassroomsModel(ApplicationDbContext db)
    {
        _db = db;
    }

    public CourseScheduleSystem.Web.Models.Student? CurrentStudent
    {
        get;
        private set;
    }

    public List<ClassroomItem> Classrooms { get; private set; } = new();

    public int TotalClassrooms { get; private set; }

    public int AvailableClassrooms { get; private set; }

    public int OccupiedClassrooms { get; private set; }

    public int UpcomingClasses { get; private set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var userIdValue =
            User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdValue, out var studentId))
        {
            return RedirectToPage("/Account/Login");
        }

        CurrentStudent = await _db.Students
            .AsNoTracking()
            .FirstOrDefaultAsync(s =>
                s.Id == studentId &&
                s.IsActive);

        if (CurrentStudent == null)
        {
            return Forbid();
        }

        var now = GetRwandaTime();
        var today = now.DayOfWeek;
        var currentTime = TimeOnly.FromDateTime(now);

        var rooms = await _db.Rooms
            .AsNoTracking()
            .OrderBy(r => r.Building)
            .ThenBy(r => r.RoomNumber)
            .Select(r => new ClassroomItem
            {
                Id = r.Id,
                RoomNumber = r.RoomNumber,
                Building = r.Building,
                Capacity = r.Capacity,
                RoomType = r.RoomType,
                IsAvailable = r.IsAvailable,

                ScheduleEntries = r.ScheduleEntries
                    .Where(s =>
                        s.IsActive &&
                        s.Status == ScheduleStatus.Active)
                    .OrderBy(s => s.DayOfWeek)
                    .ThenBy(s => s.StartTime)
                    .Select(s => new ScheduleItem
                    {
                        Id = s.Id,
                        CourseId = s.CourseId,

                        CourseCode =
                            s.Course.Code,

                        CourseName =
                            s.Course.Name,

                        LecturerName =
                            s.Lecturer != null &&
                            !string.IsNullOrWhiteSpace(
                                s.Lecturer.FullName)
                                ? s.Lecturer.FullName
                                : "Lecturer not assigned",

                        ClassRepresentativeName =
                            s.ClassRepresentative != null &&
                            !string.IsNullOrWhiteSpace(
                                s.ClassRepresentative.FullName)
                                ? s.ClassRepresentative.FullName
                                : "Class representative not assigned",

                        Intake =
                            s.ClassRepresentative != null &&
                            !string.IsNullOrWhiteSpace(
                                s.ClassRepresentative.Intake)
                                ? s.ClassRepresentative.Intake + " Intake"
                                : "Intake not assigned",

                        Level =
                            s.ClassRepresentative != null &&
                            !string.IsNullOrWhiteSpace(
                                s.ClassRepresentative.Level)
                                ? s.ClassRepresentative.Level
                                : "Level not assigned",

                        DayOfWeek =
                            s.DayOfWeek,

                        StartTime =
                            s.StartTime,

                        EndTime =
                            s.EndTime,

                        StudySession =
                            s.StudySession,

                        Notes =
                            s.Notes
                    })
                    .ToList()
            })
            .ToListAsync();


        foreach (var classroom in rooms)
        {
            var todayClasses = classroom.ScheduleEntries
                .Where(s =>
                    s.DayOfWeek == today)
                .OrderBy(s => s.StartTime)
                .ToList();

            classroom.TodaysClasses =
                todayClasses.Count;


            var currentClass =
                todayClasses.FirstOrDefault(s =>
                    currentTime >= s.StartTime &&
                    currentTime < s.EndTime);


            var nextClassToday =
                todayClasses.FirstOrDefault(s =>
                    s.StartTime > currentTime);


            if (currentClass != null)
            {
                classroom.CurrentClass =
                    currentClass;

                classroom.CurrentClassDate =
                    now.Date;

                classroom.CurrentClassDateLabel =
                    GetDateLabel(now.Date, now.Date);


                classroom.NextClass =
                    nextClassToday;


                if (nextClassToday != null)
                {
                    classroom.NextClassDate =
                        now.Date;

                    classroom.NextClassDateLabel =
                        GetDateLabel(
                            now.Date,
                            now.Date);
                }
                else
                {
                    var nextScheduledClass =
                        FindNextScheduledClass(
                            classroom.ScheduleEntries,
                            today,
                            currentTime,
                            now.Date);

                    if (nextScheduledClass != null)
                    {
                        classroom.NextClass =
                            nextScheduledClass.Value.Schedule;

                        classroom.NextClassDate =
                            nextScheduledClass.Value.Date;

                        classroom.NextClassDateLabel =
                            GetDateLabel(
                                nextScheduledClass.Value.Date,
                                now.Date);
                    }
                }


                classroom.Status =
                    ClassroomStatus.Occupied;

                classroom.StatusText =
                    "Currently in use";
            }
            else if (nextClassToday != null)
            {
                classroom.NextClass =
                    nextClassToday;

                classroom.NextClassDate =
                    now.Date;

                classroom.NextClassDateLabel =
                    GetDateLabel(
                        now.Date,
                        now.Date);

                classroom.Status =
                    ClassroomStatus.Upcoming;

                classroom.StatusText =
                    "Class starting soon";
            }
            else
            {
                var nextScheduledClass =
                    FindNextScheduledClass(
                        classroom.ScheduleEntries,
                        today,
                        currentTime,
                        now.Date);

                if (nextScheduledClass != null)
                {
                    classroom.NextClass =
                        nextScheduledClass.Value.Schedule;

                    classroom.NextClassDate =
                        nextScheduledClass.Value.Date;

                    classroom.NextClassDateLabel =
                        GetDateLabel(
                            nextScheduledClass.Value.Date,
                            now.Date);

                    classroom.Status =
                        ClassroomStatus.Upcoming;

                    classroom.StatusText =
                        "Upcoming class";
                }
                else if (!classroom.IsAvailable)
                {
                    classroom.Status =
                        ClassroomStatus.Unavailable;

                    classroom.StatusText =
                        "Unavailable";
                }
                else
                {
                    classroom.Status =
                        ClassroomStatus.Available;

                    classroom.StatusText =
                        "Available";
                }
            }
        }


        Classrooms = rooms;

        TotalClassrooms =
            Classrooms.Count;

        OccupiedClassrooms =
            Classrooms.Count(c =>
                c.Status ==
                ClassroomStatus.Occupied);

        AvailableClassrooms =
            Classrooms.Count(c =>
                c.Status ==
                ClassroomStatus.Available);

        UpcomingClasses =
            Classrooms.Count(c =>
                c.NextClass != null);

        return Page();
    }


    private static string GetDateLabel(
        DateTime scheduledDate,
        DateTime today)
    {
        scheduledDate = scheduledDate.Date;
        today = today.Date;

        if (scheduledDate == today)
        {
            return $"Today — {scheduledDate:dddd, dd MMMM yyyy}";
        }

        if (scheduledDate == today.AddDays(1))
        {
            return $"Tomorrow — {scheduledDate:dddd, dd MMMM yyyy}";
        }

        var daysUntilSunday =
            DayOfWeek.Sunday - today.DayOfWeek;

        if (daysUntilSunday < 0)
        {
            daysUntilSunday += 7;
        }

        var endOfWeek =
            today.AddDays(daysUntilSunday);

        if (scheduledDate > today &&
            scheduledDate <= endOfWeek)
        {
            return $"This week on — {scheduledDate:dddd, dd MMMM yyyy}";
        }

        return scheduledDate.ToString(
            "dddd, dd MMMM yyyy");
    }


    private static DateTime GetRwandaTime()
    {
        try
        {
            var rwandaTimeZone =
                TimeZoneInfo.FindSystemTimeZoneById(
                    "South Africa Standard Time");

            return TimeZoneInfo.ConvertTimeFromUtc(
                DateTime.UtcNow,
                rwandaTimeZone);
        }
        catch (TimeZoneNotFoundException)
        {
            return DateTime.UtcNow.AddHours(2);
        }
        catch (InvalidTimeZoneException)
        {
            return DateTime.UtcNow.AddHours(2);
        }
    }


    private static (
        ScheduleItem Schedule,
        DateTime Date
    )? FindNextScheduledClass(
        IEnumerable<ScheduleItem> scheduleEntries,
        DayOfWeek today,
        TimeOnly currentTime,
        DateTime todayDate)
    {
        var futureSchedules = scheduleEntries
            .Select(schedule =>
            {
                var dayOffset =
                    GetFutureDayOffset(
                        today,
                        schedule.DayOfWeek);

                if (dayOffset == 0 &&
                    schedule.StartTime <= currentTime)
                {
                    dayOffset = 7;
                }

                return new
                {
                    Schedule = schedule,
                    DayOffset = dayOffset
                };
            })
            .Where(x =>
                x.DayOffset > 0 ||
                x.Schedule.StartTime > currentTime)
            .OrderBy(x => x.DayOffset)
            .ThenBy(x => x.Schedule.StartTime)
            .FirstOrDefault();

        if (futureSchedules == null)
        {
            return null;
        }

        var scheduledDate =
            todayDate.AddDays(
                futureSchedules.DayOffset);

        return (
            futureSchedules.Schedule,
            scheduledDate
        );
    }


    private static int GetFutureDayOffset(
        DayOfWeek today,
        DayOfWeek scheduledDay)
    {
        var todayNumber =
            (int)today;

        var scheduledDayNumber =
            (int)scheduledDay;

        return
            (scheduledDayNumber -
             todayNumber +
             7) % 7;
    }


    public class ClassroomItem
    {
        public int Id { get; set; }

        public string RoomNumber { get; set; } =
            string.Empty;

        public string Building { get; set; } =
            string.Empty;

        public int Capacity { get; set; }

        public string? RoomType { get; set; }

        public bool IsAvailable { get; set; }

        public ClassroomStatus Status { get; set; }

        public string StatusText { get; set; } =
            string.Empty;

        public ScheduleItem? CurrentClass { get; set; }

        public DateTime? CurrentClassDate { get; set; }

        public string CurrentClassDateLabel { get; set; } =
            string.Empty;

        public ScheduleItem? NextClass { get; set; }

        public DateTime? NextClassDate { get; set; }

        public string NextClassDateLabel { get; set; } =
            string.Empty;

        public int TodaysClasses { get; set; }

        public List<ScheduleItem> ScheduleEntries { get; set; } =
            new();
    }


    public class ScheduleItem
    {
        public int Id { get; set; }

        public int CourseId { get; set; }

        public string CourseCode { get; set; } =
            string.Empty;

        public string CourseName { get; set; } =
            string.Empty;

        public string LecturerName { get; set; } =
            string.Empty;

        public string ClassRepresentativeName { get; set; } =
            string.Empty;

        public string Intake { get; set; } =
            string.Empty;

        public string Level { get; set; } =
            string.Empty;

        public DayOfWeek DayOfWeek { get; set; }

        public TimeOnly StartTime { get; set; }

        public TimeOnly EndTime { get; set; }

        public StudySession StudySession { get; set; }

        public string? Notes { get; set; }
    }


    public enum ClassroomStatus
    {
        Available,
        Occupied,
        Upcoming,
        Unavailable
    }


}

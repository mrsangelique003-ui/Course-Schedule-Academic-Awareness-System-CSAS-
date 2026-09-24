using CourseScheduleSystem.Web.Data;
using CourseScheduleSystem.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace CourseScheduleSystem.Web.Pages.Admin;

[Authorize(Roles = "Administrator,Dean,HOD,DirectorOfQuality")]
public class AssignClassModel : PageModel
{
    private readonly ApplicationDbContext _db;


public AssignClassModel(ApplicationDbContext db)
    {
        _db = db;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public bool IsEditMode => Input.Id.HasValue && Input.Id.Value > 0;

    public string? SuccessMessage =>
        TempData["SuccessMessage"]?.ToString();

    public List<CourseOption> Courses { get; private set; } = new();

    public List<RoomOption> Rooms { get; private set; } = new();

    public List<LecturerOption> Lecturers { get; private set; } = new();

    public List<RepresentativeOption> Representatives { get; private set; } = new();

    public LecturerOption? SelectedLecturer { get; private set; }

    public RepresentativeOption? SelectedRepresentative { get; private set; }

    public IReadOnlyList<DayOfWeek> Days { get; } =
        new[]
        {
        DayOfWeek.Monday,
        DayOfWeek.Tuesday,
        DayOfWeek.Wednesday,
        DayOfWeek.Thursday,
        DayOfWeek.Friday,
        DayOfWeek.Saturday,
        DayOfWeek.Sunday
        };

    public IReadOnlyList<StudySessionOption> StudySessions { get; } =
        new[]
        {
        new StudySessionOption(
            StudySession.Day,
            "DAY"),

        new StudySessionOption(
            StudySession.Evening,
            "EVENING"),

        new StudySessionOption(
            StudySession.Weekend,
            "WEEKEND")
        };

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id.HasValue)
        {
            var scheduleEntry =
                await _db.ScheduleEntries
                    .AsNoTracking()
                    .FirstOrDefaultAsync(s => s.Id == id.Value);

            if (scheduleEntry == null)
            {
                return NotFound();
            }

            Input = new InputModel
            {
                Id = scheduleEntry.Id,
                CourseId = scheduleEntry.CourseId,
                RoomId = scheduleEntry.RoomId,
                LecturerId = scheduleEntry.LecturerId,
                ClassRepresentativeId =
                    scheduleEntry.ClassRepresentativeId,
                DayOfWeek = scheduleEntry.DayOfWeek,
                StartTime = scheduleEntry.StartTime,
                EndTime = scheduleEntry.EndTime,
                StudySession = scheduleEntry.StudySession,
                Notes = scheduleEntry.Notes,
                IsActive = scheduleEntry.IsActive
            };
        }
        else
        {
            Input.DayOfWeek = DayOfWeek.Monday;
            Input.StartTime = new TimeOnly(8, 0);
            Input.EndTime = new TimeOnly(10, 0);
            Input.StudySession = StudySession.Day;
            Input.IsActive = true;
        }

        await LoadSelectionsAsync();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await LoadSelectionsAsync();

        ValidateInput();

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var course =
            await _db.Courses
                .FirstOrDefaultAsync(c =>
                    c.Id == Input.CourseId &&
                    c.IsActive);

        if (course == null)
        {
            ModelState.AddModelError(
                "Input.CourseId",
                "The selected course does not exist or is inactive.");

            return Page();
        }

        if (Input.LecturerId.HasValue)
        {
            var lecturerExists =
                await _db.Lecturers
                    .AsNoTracking()
                    .AnyAsync(l =>
                        l.Id == Input.LecturerId.Value &&
                        l.IsActive);

            if (!lecturerExists)
            {
                ModelState.AddModelError(
                    "Input.LecturerId",
                    "The selected lecturer does not exist or is inactive.");

                return Page();
            }
        }

        if (Input.ClassRepresentativeId.HasValue)
        {
            var representativeExists =
                await _db.ClassRepresentatives
                    .AsNoTracking()
                    .AnyAsync(cp =>
                        cp.Id == Input.ClassRepresentativeId.Value &&
                        cp.IsActive);

            if (!representativeExists)
            {
                ModelState.AddModelError(
                    "Input.ClassRepresentativeId",
                    "The selected class representative does not exist or is inactive.");

                return Page();
            }
        }

        var roomExists =
            await _db.Rooms
                .AsNoTracking()
                .AnyAsync(r =>
                    r.Id == Input.RoomId &&
                    r.IsAvailable);

        if (!roomExists)
        {
            ModelState.AddModelError(
                "Input.RoomId",
                "The selected classroom does not exist or is unavailable.");

            return Page();
        }

        var hasRoomConflict =
            await _db.ScheduleEntries
                .AsNoTracking()
                .AnyAsync(s =>
                    s.Id != Input.Id &&
                    s.RoomId == Input.RoomId &&
                    s.DayOfWeek == Input.DayOfWeek &&
                    s.IsActive &&
                    Input.StartTime < s.EndTime &&
                    Input.EndTime > s.StartTime);

        if (hasRoomConflict)
        {
            ModelState.AddModelError(
                "Input.RoomId",
                "The selected classroom is already occupied during this time.");

            return Page();
        }

        if (Input.LecturerId.HasValue)
        {
            var hasLecturerConflict =
                await _db.ScheduleEntries
                    .AsNoTracking()
                    .AnyAsync(s =>
                        s.Id != Input.Id &&
                        s.LecturerId == Input.LecturerId &&
                        s.DayOfWeek == Input.DayOfWeek &&
                        s.IsActive &&
                        Input.StartTime < s.EndTime &&
                        Input.EndTime > s.StartTime);

            if (hasLecturerConflict)
            {
                ModelState.AddModelError(
                    "Input.LecturerId",
                    "The selected lecturer is already assigned to another class during this time.");

                return Page();
            }
        }

        if (Input.ClassRepresentativeId.HasValue)
        {
            var hasRepresentativeConflict =
                await _db.ScheduleEntries
                    .AsNoTracking()
                    .AnyAsync(s =>
                        s.Id != Input.Id &&
                        s.ClassRepresentativeId ==
                        Input.ClassRepresentativeId &&
                        s.DayOfWeek == Input.DayOfWeek &&
                        s.IsActive &&
                        Input.StartTime < s.EndTime &&
                        Input.EndTime > s.StartTime);

            if (hasRepresentativeConflict)
            {
                ModelState.AddModelError(
                    "Input.ClassRepresentativeId",
                    "The selected class representative is already assigned to another class during this time.");

                return Page();
            }
        }

        if (Input.Id.HasValue && Input.Id.Value > 0)
        {
            var scheduleEntry =
                await _db.ScheduleEntries
                    .FirstOrDefaultAsync(s =>
                        s.Id == Input.Id.Value);

            if (scheduleEntry == null)
            {
                return NotFound();
            }

            scheduleEntry.CourseId = Input.CourseId;
            scheduleEntry.RoomId = Input.RoomId;
            scheduleEntry.LecturerId = Input.LecturerId;
            scheduleEntry.ClassRepresentativeId =
                Input.ClassRepresentativeId;
            scheduleEntry.DayOfWeek = Input.DayOfWeek;
            scheduleEntry.StartTime = Input.StartTime;
            scheduleEntry.EndTime = Input.EndTime;
            scheduleEntry.StudySession = Input.StudySession;
            scheduleEntry.Notes = Input.Notes;
            scheduleEntry.IsActive = Input.IsActive;
            scheduleEntry.Status = Input.IsActive
                ? ScheduleStatus.Active
                : GetInactiveStatus();
            scheduleEntry.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Class updated successfully.";

            return RedirectToPage(
                "/Admin/ClassroomManagement");
        }

        var newScheduleEntry = new ScheduleEntry
        {
            CourseId = Input.CourseId,
            RoomId = Input.RoomId,
            LecturerId = Input.LecturerId,
            ClassRepresentativeId =
                Input.ClassRepresentativeId,
            DayOfWeek = Input.DayOfWeek,
            StartTime = Input.StartTime,
            EndTime = Input.EndTime,
            StudySession = Input.StudySession,
            Status = Input.IsActive
                ? ScheduleStatus.Active
                : GetInactiveStatus(),
            Notes = Input.Notes,
            IsActive = Input.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        _db.ScheduleEntries.Add(newScheduleEntry);

        await _db.SaveChangesAsync();

        TempData["SuccessMessage"] =
            "Class assigned successfully.";

        return RedirectToPage(
            "/Admin/ClassroomManagement");
    }

    private async Task LoadSelectionsAsync()
    {
        Courses = await _db.Courses
            .AsNoTracking()
            .Where(c => c.IsActive)
            .OrderBy(c => c.Code)
            .Select(c => new CourseOption
            {
                Id = c.Id,
                Code = c.Code,
                Name = c.Name
            })
            .ToListAsync();

        Rooms = await _db.Rooms
            .AsNoTracking()
            .Where(r => r.IsAvailable)
            .OrderBy(r => r.Building)
            .ThenBy(r => r.RoomNumber)
            .Select(r => new RoomOption
            {
                Id = r.Id,
                RoomNumber = r.RoomNumber,
                Building = r.Building,
                Capacity = r.Capacity
            })
            .ToListAsync();

        Lecturers = await _db.Lecturers
            .AsNoTracking()
            .Where(l => l.IsActive)
            .OrderBy(l => l.FullName)
            .Select(l => new LecturerOption
            {
                Id = l.Id,
                FullName = l.FullName,
                StaffId = l.StaffId,
                Department = l.Department
            })
            .ToListAsync();

        Representatives = await _db.ClassRepresentatives
            .AsNoTracking()
            .Where(cp => cp.IsActive)
            .OrderBy(cp => cp.FullName)
            .Select(cp => new RepresentativeOption
            {
                Id = cp.Id,
                FullName = cp.FullName,
                RegNo = cp.RegNo,
                Email = cp.Email,
                PhoneNumber = cp.PhoneNumber,
                Department = cp.Department,
                Faculty = cp.Faculty,
                Intake = cp.Intake,
                Level = cp.Level
            })
            .ToListAsync();

        if (Input.LecturerId.HasValue)
        {
            SelectedLecturer =
                Lecturers.FirstOrDefault(
                    l => l.Id == Input.LecturerId.Value);
        }

        if (Input.ClassRepresentativeId.HasValue)
        {
            SelectedRepresentative =
                Representatives.FirstOrDefault(
                    cp => cp.Id == Input.ClassRepresentativeId.Value);
        }
    }

    private void ValidateInput()
    {
        if (Input.CourseId <= 0)
        {
            ModelState.AddModelError(
                "Input.CourseId",
                "Please select a course.");
        }

        if (Input.RoomId <= 0)
        {
            ModelState.AddModelError(
                "Input.RoomId",
                "Please select a classroom.");
        }

        if (Input.EndTime <= Input.StartTime)
        {
            ModelState.AddModelError(
                "Input.EndTime",
                "End time must be later than start time.");
        }
    }

    private static ScheduleStatus GetInactiveStatus()
    {
        var values =
            Enum.GetValues<ScheduleStatus>();

        var inactive =
            values.FirstOrDefault(
                value =>
                    value.ToString()
                        .Equals(
                            "Inactive",
                            StringComparison.OrdinalIgnoreCase));

        return inactive;
    }

    public class InputModel
    {
        public int? Id { get; set; }

        [Range(1, int.MaxValue)]
        public int CourseId { get; set; }

        [Range(1, int.MaxValue)]
        public int RoomId { get; set; }

        public int? LecturerId { get; set; }

        public int? ClassRepresentativeId { get; set; }

        public DayOfWeek DayOfWeek { get; set; }

        public TimeOnly StartTime { get; set; }

        public TimeOnly EndTime { get; set; }

        public StudySession StudySession { get; set; } =
            StudySession.Day;

        [StringLength(500)]
        public string? Notes { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class CourseOption
    {
        public int Id { get; set; }

        public string Code { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;
    }

    public class RoomOption
    {
        public int Id { get; set; }

        public string RoomNumber { get; set; } = string.Empty;

        public string Building { get; set; } = string.Empty;

        public int Capacity { get; set; }
    }

    public class LecturerOption
    {
        public int Id { get; set; }

        public string StaffId { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;

        public string Department { get; set; } = string.Empty;
    }

    public class RepresentativeOption
    {
        public int Id { get; set; }

        public string RegNo { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public string Department { get; set; } = string.Empty;

        public string Faculty { get; set; } = string.Empty;

        public string Intake { get; set; } = string.Empty;

        public string Level { get; set; } = string.Empty;
    }

    public record StudySessionOption(
        StudySession Value,
        string Label);


}

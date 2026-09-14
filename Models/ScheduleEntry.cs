namespace CourseScheduleSystem.Web.Models;

public class ScheduleEntry
{
    public int Id { get; set; }

    public int CourseId { get; set; }

    public Course Course { get; set; } = null!;

    public DayOfWeek DayOfWeek { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public StudySession StudySession { get; set; } = StudySession.Day;

    public int RoomId { get; set; }

    public Room Room { get; set; } = null!;

    public bool IsActive { get; set; } = true;

    public ICollection<RoomShift> RoomShifts { get; set; } =
        new List<RoomShift>();

    public ICollection<LecturerAttendance> LecturerAttendances { get; set; } =
        new List<LecturerAttendance>();

    public ICollection<AttendanceFlag> AttendanceFlags { get; set; } =
        new List<AttendanceFlag>();
}
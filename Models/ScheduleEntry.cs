namespace CourseScheduleSystem.Web.Models;

public class ScheduleEntry
{
    public int      Id           { get; set; }
    public int      CourseId     { get; set; }
    public Course   Course       { get; set; } = null!;

    /// <summary>Monday … Sunday</summary>
    public string   DayOfWeek    { get; set; } = string.Empty;
    public TimeOnly StartTime    { get; set; }
    public TimeOnly EndTime      { get; set; }

    /// <summary>Day | Evening | Weekend</summary>
    public string   StudySession { get; set; } = "Day";

    public int      RoomId       { get; set; }
    public Room     Room         { get; set; } = null!;

    public bool     IsActive     { get; set; } = true;
    public DateTime CreatedAt    { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt    { get; set; } = DateTime.UtcNow;

    // Navigation
    public ICollection<RoomShift>           RoomShifts           { get; set; } = new List<RoomShift>();
    public ICollection<LecturerAttendance>  LecturerAttendances  { get; set; } = new List<LecturerAttendance>();
    public ICollection<AttendanceFlag>      AttendanceFlags      { get; set; } = new List<AttendanceFlag>();
}

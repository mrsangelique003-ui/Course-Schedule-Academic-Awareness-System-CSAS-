namespace CourseScheduleSystem.Web.Models;

public class LecturerAttendance
{
    public int     Id                { get; set; }
    public int     LecturerId        { get; set; }
    public Lecturer Lecturer         { get; set; } = null!;

    public int     ScheduleEntryId   { get; set; }
    public ScheduleEntry ScheduleEntry { get; set; } = null!;

    public DateOnly SessionDate      { get; set; }

    /// <summary>Present | Absent | Late | MakeUp</summary>
    public string  AttendanceStatus  { get; set; } = "Present";
    public string? OfficialReason    { get; set; }
    public bool    MakeUpRequested   { get; set; }
    public DateTime? MakeUpScheduledAt { get; set; }
    public DateTime  RecordedAt      { get; set; } = DateTime.UtcNow;
}

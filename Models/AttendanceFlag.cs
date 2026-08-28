namespace CourseScheduleSystem.Web.Models;

public class AttendanceFlag
{
    public int    Id                { get; set; }
    public string RaisedByUserId    { get; set; } = string.Empty;
    public ApplicationUser RaisedBy { get; set; } = null!;

    public int    ScheduleEntryId   { get; set; }
    public ScheduleEntry ScheduleEntry { get; set; } = null!;

    public DateTime FlaggedAt       { get; set; } = DateTime.UtcNow;

    /// <summary>Absent | Late | Cancelled</summary>
    public string  IssueType        { get; set; } = "Absent";
    public string? Notes            { get; set; }

    /// <summary>Pending | Reviewed | Resolved</summary>
    public string  Status           { get; set; } = "Pending";
}

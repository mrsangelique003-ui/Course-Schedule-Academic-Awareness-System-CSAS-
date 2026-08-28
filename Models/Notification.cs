namespace CourseScheduleSystem.Web.Models;

public class Notification
{
    public int    Id       { get; set; }
    public string UserId   { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;

    public string  Title     { get; set; } = string.Empty;
    public string  Body      { get; set; } = string.Empty;

    /// <summary>RoomShift | Cancellation | Deadline | Announcement | System</summary>
    public string  Type      { get; set; } = "System";
    public bool    IsRead    { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ReadAt  { get; set; }
}

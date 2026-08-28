namespace CourseScheduleSystem.Web.Models;

public class Room
{
    public int     Id           { get; set; }
    public string  RoomNumber   { get; set; } = string.Empty;
    public string  Building     { get; set; } = string.Empty;
    public int     Capacity     { get; set; }
    public DateTime CreatedAt   { get; set; } = DateTime.UtcNow;

    // Navigation
    public ICollection<ScheduleEntry> ScheduleEntries { get; set; } = new List<ScheduleEntry>();
    public ICollection<RoomShift>     OriginalShifts  { get; set; } = new List<RoomShift>();
    public ICollection<RoomShift>     NewShifts       { get; set; } = new List<RoomShift>();
}

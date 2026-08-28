namespace CourseScheduleSystem.Web.Models;

public class RoomShift
{
    public int     Id                 { get; set; }
    public int     ScheduleEntryId    { get; set; }
    public ScheduleEntry ScheduleEntry { get; set; } = null!;

    public int     OriginalRoomId     { get; set; }
    public Room    OriginalRoom       { get; set; } = null!;

    public int     NewRoomId          { get; set; }
    public Room    NewRoom            { get; set; } = null!;

    public string  Reason             { get; set; } = string.Empty;
    public DateTime EffectiveAt       { get; set; }

    public string  CreatedByUserId    { get; set; } = string.Empty;
    public ApplicationUser CreatedBy  { get; set; } = null!;
    public DateTime CreatedAt         { get; set; } = DateTime.UtcNow;

    /// <summary>Pending | Approved | Rejected</summary>
    public string  Status             { get; set; } = "Pending";
}

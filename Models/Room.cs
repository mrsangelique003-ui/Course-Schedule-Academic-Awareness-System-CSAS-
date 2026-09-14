using System.ComponentModel.DataAnnotations;

namespace CourseScheduleSystem.Web.Models;

public class Room
{
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string RoomNumber { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Building { get; set; } = string.Empty;

    [Range(1, 2000)]
    public int Capacity { get; set; }

    public ICollection<ScheduleEntry> ScheduleEntries { get; set; } =
        new List<ScheduleEntry>();

    public ICollection<RoomShift> OriginalRoomShifts { get; set; } =
        new List<RoomShift>();

    public ICollection<RoomShift> NewRoomShifts { get; set; } =
        new List<RoomShift>();
}
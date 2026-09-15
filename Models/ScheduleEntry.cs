using System.ComponentModel.DataAnnotations;

namespace CourseScheduleSystem.Web.Models;

public class ScheduleEntry
{
    public int Id { get; set; }

    public int CourseId { get; set; }

    public Course Course { get; set; } = null!;

    public int LecturerId { get; set; }

    public Lecturer Lecturer { get; set; } = null!;

    public int RoomId { get; set; }

    public Room Room { get; set; } = null!;

    public DayOfWeek DayOfWeek { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public StudySession StudySession { get; set; } = StudySession.Day;

    public ScheduleStatus Status { get; set; } = ScheduleStatus.Active;

    [StringLength(500)]
    public string? Notes { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }
}
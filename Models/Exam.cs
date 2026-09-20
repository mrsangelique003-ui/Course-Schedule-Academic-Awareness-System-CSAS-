using System.ComponentModel.DataAnnotations;

namespace CourseScheduleSystem.Web.Models;

public class Exam
{
    public int Id { get; set; }


public int CourseId { get; set; }

    public Course Course { get; set; } = null!;

    public int RoomId { get; set; }

    public Room Room { get; set; } = null!;

    public ExamType ExamType { get; set; }

    public DateTime ExamDate { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public bool IsActive { get; set; } = true;

    [StringLength(500)]
    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }


}

public enum ExamType
{
    CAT,
    Final
}

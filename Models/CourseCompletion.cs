using System.ComponentModel.DataAnnotations;

namespace CourseScheduleSystem.Web.Models;

public class CourseCompletion
{
    public int Id { get; set; }

    [Required]
    public int CourseId { get; set; }

    [Required]
    public int ClassRepresentativeId { get; set; }

    [Required]
    public int LecturerId { get; set; }

    public CourseCompletionStatus Status { get; set; }

    [StringLength(1000)]
    public string? Remarks { get; set; }

    public DateTime? SubmittedAt { get; set; }

    public DateTime? ConfirmedAt { get; set; }

    // Navigation properties
    public Course Course { get; set; } = null!;

    public ClassRepresentative ClassRepresentative { get; set; } = null!;

    public Lecturer Lecturer { get; set; } = null!;
}

public enum CourseCompletionStatus
{
    Submitted,
    Confirmed,
    Returned
}
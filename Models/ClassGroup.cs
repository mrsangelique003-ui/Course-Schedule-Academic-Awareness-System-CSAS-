using System.ComponentModel.DataAnnotations;

namespace CourseScheduleSystem.Web.Models;

public class ClassGroup
{
    public int Id { get; set; }

    [Required]
    public int CourseId { get; set; }

    [Required]
    public int ClassRepresentativeId { get; set; }

    [Required]
    [StringLength(100)]
    public string GroupName { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Intake { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Level { get; set; } = string.Empty;

    [Required]
    [StringLength(500)]
    public string GroupLink { get; set; } = string.Empty;

    public bool IsPublished { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public Course Course { get; set; } = null!;

    public ClassRepresentative ClassRepresentative { get; set; } = null!;
}
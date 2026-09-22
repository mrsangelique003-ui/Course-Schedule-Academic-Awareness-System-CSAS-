using System.ComponentModel.DataAnnotations;

namespace CourseScheduleSystem.Web.Models;

public class HodMessage
{
    public int Id { get; set; }

    [Required]
    public int ClassRepresentativeId { get; set; }

    [Required]
    public int HodId { get; set; }

    [Required]
    [StringLength(2000)]
    public string Message { get; set; } = string.Empty;

    public bool IsRead { get; set; }

    public DateTime SentAt { get; set; }

    // Navigation properties
    public ClassRepresentative ClassRepresentative { get; set; } = null!;
}
using System.ComponentModel.DataAnnotations;

namespace CourseScheduleSystem.Web.Models;

public class Administrator
{
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string StaffId { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string FullName { get; set; } = string.Empty;

    [EmailAddress]
    [StringLength(256)]
    public string? Email { get; set; }

    [Phone]
    [StringLength(20)]
    public string? PhoneNumber { get; set; }

    [Required]
    [StringLength(100)]
    public string Department { get; set; } = "CIS";

    [Required]
    [StringLength(50)]
    public string Role { get; set; } = "HOD";

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Course> ManagedCourses { get; set; } =
        new List<Course>();

    public ICollection<ScheduleEntry> ManagedSchedules { get; set; } =
        new List<ScheduleEntry>();
}
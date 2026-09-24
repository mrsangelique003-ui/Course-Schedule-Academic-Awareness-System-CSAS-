using System.ComponentModel.DataAnnotations;

namespace CourseScheduleSystem.Web.Models;

public class Lecturer
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

    public bool IsActive { get; set; } = true;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    public ICollection<Course> Courses { get; set; } =
        new List<Course>();

    public ICollection<ScheduleEntry> ScheduleEntries { get; set; } =
        new List<ScheduleEntry>();


}

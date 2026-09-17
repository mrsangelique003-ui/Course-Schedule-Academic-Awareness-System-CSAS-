using System.ComponentModel.DataAnnotations;

namespace CourseScheduleSystem.Web.Models;

public class Student
{
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string RegNo { get; set; } = string.Empty;

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

    [StringLength(50)]
    public string? Program { get; set; }

    [StringLength(50)]
    public string? Level { get; set; }

    public StudySession StudySession { get; set; } = StudySession.Day;

    public bool IsActive { get; set; } = true;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Enrollment> Enrollments { get; set; } =
        new List<Enrollment>();
}
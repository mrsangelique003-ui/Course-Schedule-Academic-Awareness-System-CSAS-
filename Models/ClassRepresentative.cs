using System.ComponentModel.DataAnnotations;

namespace CourseScheduleSystem.Web.Models;

public class ClassRepresentative
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

    [StringLength(100)]
    public string? Nationality { get; set; }

    [Required]
    [StringLength(100)]
    public string Department { get; set; } = "CIS";

    public StudySession StudySession { get; set; } = StudySession.Day;

    public bool IsActive { get; set; } = true;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

    // ============================================================
    // EXISTING COURSE RELATIONSHIP
    // ============================================================

    public ICollection<Course> RepresentedCourses { get; set; } =
        new List<Course>();

    // ============================================================
    // CP MODULE
    // ============================================================

    [Required]
    [StringLength(100)]
    public string Intake { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Level { get; set; } = string.Empty;

    // Class groups managed by this Class Representative
    public ICollection<ClassGroup> ClassGroups { get; set; } =
        new List<ClassGroup>();

    // Course completion/sign-off records
    public ICollection<CourseCompletion> CourseCompletions { get; set; } =
        new List<CourseCompletion>();

    // Messages exchanged with the HOD
    public ICollection<HodMessage> HodMessages { get; set; } =
        new List<HodMessage>();
}
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

    [Required]
    [EmailAddress]
    [StringLength(256)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [Phone]
    [StringLength(20)]
    public string PhoneNumber { get; set; } = string.Empty;

    [StringLength(100)]
    public string? Nationality { get; set; }

    [Required]
    [StringLength(100)]
    [RegularExpression(
        "^(Information System and Management|Information Technology|Software Engineering|Networking|Multimedia)$",
        ErrorMessage = "Please select a valid department.")]
    public string Department { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Faculty { get; set; } = string.Empty;

    [Range(1, 3, ErrorMessage = "Year must be between 1 and 3.")]
    public int Year { get; set; }

    public StudySession StudySession { get; set; } = StudySession.Day;

    public bool IsActive { get; set; } = true;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

    public string SignatureImageData { get; set; } = string.Empty;

    public ICollection<Course> RepresentedCourses { get; set; } =
        new List<Course>();

    [Required]
    [StringLength(100)]
    [RegularExpression(
        "^(January|March|September)$",
        ErrorMessage = "Intake must be January, March, or September.")]
    public string Intake { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    [RegularExpression(
        "^Year [1-3]$",
        ErrorMessage = "Level must be Year 1, Year 2, or Year 3.")]
    public string Level { get; set; } = string.Empty;

    public ICollection<ClassGroup> ClassGroups { get; set; } =
        new List<ClassGroup>();

    public ICollection<CourseCompletion> CourseCompletions { get; set; } =
        new List<CourseCompletion>();

    public ICollection<HodMessage> HodMessages { get; set; } =
        new List<HodMessage>();

    public ICollection<ScheduleEntry> ScheduleEntries { get; set; } =
        new List<ScheduleEntry>();


}

using System.ComponentModel.DataAnnotations;

namespace CourseScheduleSystem.Web.Models;

public class Course
{
    public int Id { get; set; }

    [Required]
    [StringLength(20)]
    public string Code { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }

    [Range(1, 30)]
    public int Credits { get; set; }

    public CourseStatus Status { get; set; } = CourseStatus.Available;

    public StudySession StudySession { get; set; } = StudySession.Day;

    public int? LecturerId { get; set; }

    public Lecturer? Lecturer { get; set; }

    public int? AdministratorId { get; set; }

    public Administrator? Administrator { get; set; }

    public DateTime? RegistrationOpenDate { get; set; }

    public DateTime? RegistrationCloseDate { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<ScheduleEntry> ScheduleEntries { get; set; } =
        new List<ScheduleEntry>();

    public ICollection<ClassRepresentative> ClassRepresentatives { get; set; } =
        new List<ClassRepresentative>();

    public ICollection<Enrollment> Enrollments { get; set; } =
        new List<Enrollment>();
}
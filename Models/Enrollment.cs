namespace CourseScheduleSystem.Web.Models;

public class Enrollment
{
    public int Id { get; set; }

    public int StudentId { get; set; }

    public Student Student { get; set; } = null!;

    public int CourseId { get; set; }

    public Course Course { get; set; } = null!;

    public EnrollmentStatus Status { get; set; } =
        EnrollmentStatus.Pending;

    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;

    public DateTime? ApprovedAt { get; set; }
}
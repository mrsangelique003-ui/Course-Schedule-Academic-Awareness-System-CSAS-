namespace CourseScheduleSystem.Web.Models;

public class Enrollment
{
    public int    Id       { get; set; }
    public string UserId   { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;

    public int    CourseId { get; set; }
    public Course Course   { get; set; } = null!;

    /// <summary>Pending | Enrolled | Closed</summary>
    public string    Status     { get; set; } = "Pending";
    public DateTime  EnrolledAt { get; set; } = DateTime.UtcNow;
}

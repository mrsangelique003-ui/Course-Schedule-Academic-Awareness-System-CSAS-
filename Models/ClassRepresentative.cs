namespace CourseScheduleSystem.Web.Models;

public class ClassRepresentative
{
    public int    Id         { get; set; }
    public string UserId     { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;

    public int    CourseId   { get; set; }
    public Course Course     { get; set; } = null!;

    public bool     IsActive   { get; set; } = true;
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
}

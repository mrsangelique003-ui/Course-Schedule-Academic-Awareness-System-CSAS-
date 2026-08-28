namespace CourseScheduleSystem.Web.Models;

public class AuditLog
{
    public int     Id         { get; set; }
    public string? UserId     { get; set; }
    public ApplicationUser? User { get; set; }

    public string  Action     { get; set; } = string.Empty;
    public string  EntityType { get; set; } = string.Empty;
    public string? EntityId   { get; set; }
    public string? OldValues  { get; set; }  // JSON
    public string? NewValues  { get; set; }  // JSON
    public string? IpAddress  { get; set; }
    public string? UserAgent  { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

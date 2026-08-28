using Microsoft.AspNetCore.Identity;

namespace CourseScheduleSystem.Web.Models;

public class ApplicationUser : IdentityUser
{
    public string RegNo         { get; set; } = string.Empty;
    public string FullName      { get; set; } = string.Empty;
    public string Department    { get; set; } = "CIS";
    public string StudySession  { get; set; } = "Day";   // Day | Evening | Weekend
    public bool   IsActive      { get; set; } = true;
    public string? ProfilePhotoUrl { get; set; }
    public DateTime? LastLoginAt   { get; set; }
    public DateTime  CreatedAt     { get; set; } = DateTime.UtcNow;

    // Navigation
    public ICollection<Enrollment>           Enrollments           { get; set; } = new List<Enrollment>();
    public ICollection<ClassRepresentative>  ClassRepresentatives  { get; set; } = new List<ClassRepresentative>();
    public ICollection<AttendanceFlag>       AttendanceFlags       { get; set; } = new List<AttendanceFlag>();
    public ICollection<Notification>         Notifications         { get; set; } = new List<Notification>();
    public ICollection<AuditLog>             AuditLogs             { get; set; } = new List<AuditLog>();
    public ICollection<RoomShift>            RoomShifts            { get; set; } = new List<RoomShift>();
    public ICollection<Application>          Applications          { get; set; } = new List<Application>();
}

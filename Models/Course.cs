namespace CourseScheduleSystem.Web.Models;

public class Course
{
    public int     Id          { get; set; }
    public string  Code        { get; set; } = string.Empty;
    public string  Name        { get; set; } = string.Empty;
    public string? Description { get; set; }

    /// <summary>Available | Closed | Upcoming</summary>
    public string  Status      { get; set; } = "Available";

    public string? PhotoUrl           { get; set; }
    public string? WhatsAppGroupLink  { get; set; }

    public DateTime? StartDate              { get; set; }
    public DateTime? EndDate                { get; set; }
    public DateTime? RegistrationOpenDate   { get; set; }
    public DateTime? RegistrationCloseDate  { get; set; }
    public DateTime  CreatedAt              { get; set; } = DateTime.UtcNow;

    // FK
    public int? LecturerId { get; set; }
    public Lecturer? Lecturer { get; set; }

    // Navigation
    public ICollection<ScheduleEntry>      ScheduleEntries     { get; set; } = new List<ScheduleEntry>();
    public ICollection<Enrollment>         Enrollments         { get; set; } = new List<Enrollment>();
    public ICollection<ClassRepresentative> ClassRepresentatives { get; set; } = new List<ClassRepresentative>();
}

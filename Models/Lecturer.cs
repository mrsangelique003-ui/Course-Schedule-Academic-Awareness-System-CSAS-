namespace CourseScheduleSystem.Web.Models;

public class Lecturer
{
    public int     Id          { get; set; }
    public string  StaffId     { get; set; } = string.Empty;
    public string  FullName    { get; set; } = string.Empty;
    public string  Email       { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string  Department  { get; set; } = "CIS";
    public DateTime CreatedAt  { get; set; } = DateTime.UtcNow;

    // Navigation
    public ICollection<Course>              Courses              { get; set; } = new List<Course>();
    public ICollection<LecturerAttendance>  LecturerAttendances  { get; set; } = new List<LecturerAttendance>();
}

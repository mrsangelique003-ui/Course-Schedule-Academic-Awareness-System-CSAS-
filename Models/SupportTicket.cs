using System.ComponentModel.DataAnnotations;

namespace CourseScheduleSystem.Web.Models;

public class SupportTicket
{
    public int Id { get; set; }

    [Required]
    [StringLength(20)]
    public string TicketNumber { get; set; } = string.Empty;

    public int StudentId { get; set; }

    public Student Student { get; set; } = null!;

    [Required]
    [StringLength(100)]
    public string Subject { get; set; } = string.Empty;

    public SupportCategory Category { get; set; }

    [Required]
    [StringLength(2000)]
    public string Description { get; set; } = string.Empty;

    public SupportTicketStatus Status { get; set; } = SupportTicketStatus.Open;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }
}

public enum SupportCategory
{
    TechnicalIssue,
    AccountAccess,
    CourseSchedule,
    Enrollment,
    Examination,
    Other
}

public enum SupportTicketStatus
{
    Open,
    InProgress,
    Resolved,
    Closed
}


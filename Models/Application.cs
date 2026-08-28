namespace CourseScheduleSystem.Web.Models;

public class Application
{
    public int     Id                  { get; set; }
    public string  ReferenceNumber     { get; set; } = string.Empty;

    public int     ServiceId           { get; set; }
    public Service Service             { get; set; } = null!;

    public string  ApplicantUserId     { get; set; } = string.Empty;
    public ApplicationUser Applicant   { get; set; } = null!;

    public string? ReviewedByUserId    { get; set; }
    public ApplicationUser? ReviewedBy { get; set; }

    /// <summary>Draft | Submitted | UnderReview | Approved | Rejected | Completed</summary>
    public string  Status              { get; set; } = "Draft";
    public string? RejectionReason     { get; set; }
    public string? ReviewComments      { get; set; }

    /// <summary>Normal | High | Urgent</summary>
    public string  Priority            { get; set; } = "Normal";

    public DateTime  CreatedAt         { get; set; } = DateTime.UtcNow;
    public DateTime? SubmittedAt       { get; set; }
    public DateTime? ReviewedAt        { get; set; }
    public DateTime? CompletedAt       { get; set; }
    public DateTime? DueDate           { get; set; }

    public ICollection<Document> Documents { get; set; } = new List<Document>();
}

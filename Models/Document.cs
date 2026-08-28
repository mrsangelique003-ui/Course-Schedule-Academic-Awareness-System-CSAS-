namespace CourseScheduleSystem.Web.Models;

public class Document
{
    public int     Id                   { get; set; }
    public int?    ServiceId            { get; set; }
    public Service? Service             { get; set; }

    public int     ApplicationId        { get; set; }
    public Application Application      { get; set; } = null!;

    public string  UploadedByUserId     { get; set; } = string.Empty;
    public ApplicationUser UploadedBy   { get; set; } = null!;

    public string  Title                { get; set; } = string.Empty;
    public string? Description          { get; set; }
    public string  DocumentType         { get; set; } = string.Empty;
    public string  FilePath             { get; set; } = string.Empty;
    public string  OriginalFileName     { get; set; } = string.Empty;
    public string  ContentType          { get; set; } = string.Empty;
    public long    FileSizeBytes        { get; set; }

    /// <summary>Pending | Verified | Rejected</summary>
    public string  Status               { get; set; } = "Pending";
    public string? RejectionReason      { get; set; }
    public bool    IsRequired           { get; set; }

    public DateTime  CreatedAt          { get; set; } = DateTime.UtcNow;
    public DateTime  UploadedAt         { get; set; } = DateTime.UtcNow;
    public DateTime? VerifiedAt         { get; set; }
    public string?   VerifiedByUserId   { get; set; }
}

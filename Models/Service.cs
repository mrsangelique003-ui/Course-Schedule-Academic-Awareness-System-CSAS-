namespace CourseScheduleSystem.Web.Models;

public class Service
{
    public int     Id                  { get; set; }
    public string  Code                { get; set; } = string.Empty;
    public string  Name                { get; set; } = string.Empty;
    public string? Description         { get; set; }
    public string? Category            { get; set; }
    public string  Status              { get; set; } = "Active";
    public string? AllowedRoles        { get; set; }
    public int?    ProcessingDays      { get; set; }
    public decimal? Fee                { get; set; }
    public string? IconUrl             { get; set; }
    public bool    RequiresDocuments   { get; set; }
    public string? CreatedByUserId     { get; set; }
    public DateTime CreatedAt          { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt          { get; set; } = DateTime.UtcNow;

    public ICollection<Application> Applications { get; set; } = new List<Application>();
}

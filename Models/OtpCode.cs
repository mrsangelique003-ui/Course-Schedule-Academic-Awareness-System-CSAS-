namespace CourseScheduleSystem.Web.Models;

public class OtpCode
{
    public int     Id              { get; set; }
    public string  UserId          { get; set; } = string.Empty;
    public ApplicationUser User    { get; set; } = null!;

    public string  CodeHash        { get; set; } = string.Empty;
    public string  Purpose         { get; set; } = "Login";
    public DateTime ExpiresAt      { get; set; }
    public DateTime CreatedAt      { get; set; } = DateTime.UtcNow;
    public bool    IsUsed          { get; set; }
    public DateTime? UsedAt        { get; set; }
    public int     FailedAttempts  { get; set; }
    public string? DeliveryChannel { get; set; }
    public string? DeliveryAddress { get; set; }
}

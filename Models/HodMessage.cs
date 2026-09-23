using System.ComponentModel.DataAnnotations;

namespace CourseScheduleSystem.Web.Models;

public class HodMessage
{
    public int Id { get; set; }

    [Required]
    public int ClassRepresentativeId { get; set; }

    [Required]
    public int HodId { get; set; }

    [Required]
    [StringLength(2000)]
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Who sent this message — HOD or the Class Representative.
    /// </summary>
    public MessageSenderType SenderType { get; set; } = MessageSenderType.Hod;

    public bool IsRead { get; set; }

    public DateTime SentAt { get; set; }

    // Navigation properties
    public ClassRepresentative ClassRepresentative { get; set; } = null!;
}

/// <summary>
/// Identifies which side of the conversation sent a message.
/// </summary>
public enum MessageSenderType
{
    Hod = 0,
    ClassRepresentative = 1
}
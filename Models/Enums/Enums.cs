
namespace CourseScheduleSystem.Web.Models;

public enum CourseStatus
{
    Available,
    Closed,
    Upcoming
}

public enum EnrollmentStatus
{
    Pending,
    Enrolled,
    Closed
}

public enum ApplicationStatus
{
    Draft,
    Submitted,
    UnderReview,
    Approved,
    Rejected,
    Completed
}

public enum ApplicationPriority
{
    Normal,
    High,
    Urgent
}

public enum DocumentStatus
{
    Pending,
    Verified,
    Rejected
}

public enum RoomShiftStatus
{
    Pending,
    Approved,
    Rejected
}

public enum AttendanceIssueType
{
    Absent,
    Late,
    Cancelled
}

public enum AttendanceFlagStatus
{
    Pending,
    Reviewed,
    Resolved
}

public enum LecturerAttendanceStatus
{
    Present,
    Absent,
    Late,
    MakeUp
}

public enum NotificationType
{
    RoomShift,
    Cancellation,
    Deadline,
    Announcement,
    System
}

public enum StudySession
{
    Day,
    Evening,
    Weekend
}

public enum ServiceStatus
{
    Active,
    Inactive
}

public enum OtpPurpose
{
    Login,
    PasswordReset,
    EmailVerification,
    TwoFactorAuth
}

public enum OtpDeliveryChannel
{
    Sms,
    Email,
    WhatsApp
}

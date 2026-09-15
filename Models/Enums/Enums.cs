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

public enum StudySession
{
    Day,
    Evening,
    Weekend
}

public enum ScheduleStatus
{
    Active,
    Cancelled,
    Rescheduled
}
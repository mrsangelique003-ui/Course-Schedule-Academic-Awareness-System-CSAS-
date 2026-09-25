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

public enum CPDepartment
{
    InformationSystemAndManagement,
    InformationTechnology,
    SoftwareEngineering,
    Networking,
    Multimedia
}

public enum CPIntake
{
    January,
    March,
    September
}

public enum CPLevel
{
    Year1,
    Year2,
    Year3
}

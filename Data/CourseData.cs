namespace CourseScheduleSystem.Web.Data;

public class Course
{
    public string Code { get; set; } = "";
    public string Title { get; set; } = "";
    public string Lecturer { get; set; } = "";
    public string ScheduleTime { get; set; } = "";
    public string Venue { get; set; } = "";
    public string WhatsappGroupUrl { get; set; } = "";
    public DateTime JoinDeadline { get; set; }
}

public static class CourseData
{
    public static List<Course> Courses { get; } = new()
    {
        new Course
        {
            Code = "CSE301",
            Title = "Software Engineering",
            Lecturer = "Dr. J. Mugisha",
            ScheduleTime = "Mon 08:00 - 10:00",
            Venue = "Block A - Room 204",
            WhatsappGroupUrl = "https://chat.whatsapp.com/example1",
            JoinDeadline = DateTime.Now.AddDays(5)
        },
        new Course
        {
            Code = "CSE305",
            Title = "Database Systems",
            Lecturer = "Mrs. A. Byukusenge",
            ScheduleTime = "Tue 10:00 - 12:00",
            Venue = "Block B - Room 101",
            WhatsappGroupUrl = "https://chat.whatsapp.com/example2",
            JoinDeadline = DateTime.Now.AddDays(3)
        },
        new Course
        {
            Code = "CSE310",
            Title = "Human-Computer Interaction",
            Lecturer = "Mr. E. Niyonsenga",
            ScheduleTime = "Wed 13:00 - 15:00",
            Venue = "Block A - Room 108",
            WhatsappGroupUrl = "https://chat.whatsapp.com/example3",
            JoinDeadline = DateTime.Now.AddDays(-1)
        },
        new Course
        {
            Code = "CSE402",
            Title = "Software Architecture",
            Lecturer = "Dr. P. Habimana",
            ScheduleTime = "Thu 08:00 - 10:00",
            Venue = "Block C - Room 302",
            WhatsappGroupUrl = "https://chat.whatsapp.com/example4",
            JoinDeadline = DateTime.Now.AddDays(7)
        }
    };
}
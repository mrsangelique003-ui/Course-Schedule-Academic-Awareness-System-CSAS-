// Legacy static seed data (kept for reference — real seed data is in DbInitializer.cs)
namespace CourseScheduleSystem.Web.Data;

public class CourseSeedItem
{
    public string Code          { get; set; } = "";
    public string Title         { get; set; } = "";
    public string LecturerName  { get; set; } = "";
    public string ScheduleTime  { get; set; } = "";
    public string Venue         { get; set; } = "";
    public string WhatsappGroupUrl { get; set; } = "";
    public DateTime JoinDeadline   { get; set; }
}

public static class LegacyCourseData
{
    public static List<CourseSeedItem> Items { get; } = new()
    {
        new CourseSeedItem
        {
            Code = "CSE301", Title = "Software Engineering",
            LecturerName = "Dr. J. Mugisha", ScheduleTime = "Mon 08:00 - 10:00",
            Venue = "Block A - Room 204",
            WhatsappGroupUrl = "https://chat.whatsapp.com/example1",
            JoinDeadline = DateTime.Now.AddDays(5)
        },
        new CourseSeedItem
        {
            Code = "CSE305", Title = "Database Systems",
            LecturerName = "Mrs. A. Byukusenge", ScheduleTime = "Tue 10:00 - 12:00",
            Venue = "Block B - Room 101",
            WhatsappGroupUrl = "https://chat.whatsapp.com/example2",
            JoinDeadline = DateTime.Now.AddDays(3)
        },
        new CourseSeedItem
        {
            Code = "CSE310", Title = "Human-Computer Interaction",
            LecturerName = "Mr. E. Niyonsenga", ScheduleTime = "Wed 13:00 - 15:00",
            Venue = "Block A - Room 108",
            WhatsappGroupUrl = "https://chat.whatsapp.com/example3",
            JoinDeadline = DateTime.Now.AddDays(-1)
        },
        new CourseSeedItem
        {
            Code = "CSE402", Title = "Software Architecture",
            LecturerName = "Dr. P. Habimana", ScheduleTime = "Thu 08:00 - 10:00",
            Venue = "Block C - Room 302",
            WhatsappGroupUrl = "https://chat.whatsapp.com/example4",
            JoinDeadline = DateTime.Now.AddDays(7)
        }
    };
}

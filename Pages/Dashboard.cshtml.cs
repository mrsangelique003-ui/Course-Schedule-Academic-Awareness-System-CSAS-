using System.Security.Claims;
using CourseScheduleSystem.Web.Models; // Adjust namespace to match your project
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CourseScheduleSystem.Web.Pages
{
    [Authorize]
    public class DashboardModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public DashboardModel(ApplicationDbContext db)
        {
            _db = db;
        }

        // Stats
        public int TotalClasses { get; set; }
        public int TotalTasks { get; set; }
        public int TotalExams { get; set; }

        // Data Lists
        public List<TaskItemDto> Tasks { get; set; } = new();
        public List<ScheduleItemDto> ScheduleItems { get; set; } = new();

        public async Task OnGetAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var role = User.FindFirstValue(ClaimTypes.Role);

            // Fetch summary stats
            TotalClasses = await _db.Schedules.CountAsync();
            TotalTasks = await _db.Tasks.CountAsync();
            TotalExams = await _db.Tasks.CountAsync(t => t.IsExam);

            // Load Task Items
            Tasks = await _db.Tasks
                .Include(t => t.Teacher)
                .Select(t => new TaskItemDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    TeacherName = t.Teacher.FullName,
                    TeacherRole = t.Teacher.Title,
                    CompletedSteps = t.CompletedSteps,
                    TotalSteps = t.TotalSteps,
                    Points = t.Points,
                    ProgressPercentage = t.TotalSteps > 0 ? (t.CompletedSteps * 100) / t.TotalSteps : 0
                })
                .Take(5)
                .ToListAsync();

            // Load Today's Class Schedule
            ScheduleItems = await _db.Schedules
                .Include(s => s.Teacher)
                .Where(s => s.Date.Date == DateTime.Today)
                .OrderBy(s => s.StartTime)
                .Select(s => new ScheduleItemDto
                {
                    Id = s.Id,
                    TeacherName = s.Teacher != null ? s.Teacher.FullName : string.Empty,
                    Subject = s.Subject,
                    StartTime = s.StartTime.ToString("hh:mm tt"),
                    EndTime = s.EndTime.ToString("hh:mm tt"),
                    IsBreak = s.IsBreak,
                    CardColorClass = s.IsBreak ? "dash-schedule-card-plain" : GetColorClass(s.Subject)
                })
                .ToListAsync();
        }

        private static string GetColorClass(string subject) => subject.ToLower() switch
        {
            "science" => "dash-schedule-card-green",
            "biology" => "dash-schedule-card-yellow",
            "physics" => "dash-schedule-card-purple",
            _ => "dash-schedule-card-green"
        };

        public class TaskItemDto
        {
            public int Id { get; set; }
            public string Title { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public string TeacherName { get; set; } = string.Empty;
            public string TeacherRole { get; set; } = string.Empty;
            public int CompletedSteps { get; set; }
            public int TotalSteps { get; set; }
            public int Points { get; set; }
            public int ProgressPercentage { get; set; }
        }

        public class ScheduleItemDto
        {
            public int Id { get; set; }
            public string TeacherName { get; set; } = string.Empty;
            public string Subject { get; set; } = string.Empty;
            public string StartTime { get; set; } = string.Empty;
            public string EndTime { get; set; } = string.Empty;
            public bool IsBreak { get; set; }
            public string CardColorClass { get; set; } = string.Empty;
        }
    }
}
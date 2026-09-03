using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
namespace CourseScheduleSystem.Web.Pages.Student;
[Authorize(Roles = "Student")]
public class NotificationsModel : PageModel { public void OnGet() { } }

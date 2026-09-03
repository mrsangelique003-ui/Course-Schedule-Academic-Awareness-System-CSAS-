using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
namespace CourseScheduleSystem.Web.Pages.Student;
[Authorize(Roles = "Student")]
public class CoursesModel : PageModel { public void OnGet() { } }

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
namespace CourseScheduleSystem.Web.Pages.Student;
[Authorize(Roles = "Student")]
public class ProfileModel : PageModel { public void OnGet() { } }

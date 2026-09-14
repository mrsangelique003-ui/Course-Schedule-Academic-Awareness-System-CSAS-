using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
namespace CourseScheduleSystem.Web.Pages.Quality;
[Authorize(Roles = "DirectorOfQuality,Dean,HOD")]
public class AttendanceLogModel : PageModel { public void OnGet() { } }

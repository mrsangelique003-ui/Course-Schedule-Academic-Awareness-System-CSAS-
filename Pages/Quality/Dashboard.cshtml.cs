using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CourseScheduleSystem.Web.Pages.Quality;

[Authorize(Roles = "DirectorOfQuality")]
public class DashboardModel : PageModel
{
    public void OnGet() { }
}

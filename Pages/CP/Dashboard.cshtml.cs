using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CourseScheduleSystem.Web.Pages.CP;

// CP can edit. DirectorOfQuality, Dean, HOD get read-only view via Razor logic.
[Authorize(Roles = "CP,DirectorOfQuality,Dean,HOD")]
public class DashboardModel : PageModel
{
    public void OnGet() { }
}

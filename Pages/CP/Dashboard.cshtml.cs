using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CourseScheduleSystem.Web.Pages.CP;

[Authorize(Roles = "CP")]
public class DashboardModel : PageModel
{
    public void OnGet() { }
}

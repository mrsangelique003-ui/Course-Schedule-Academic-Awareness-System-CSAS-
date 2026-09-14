using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
namespace CourseScheduleSystem.Web.Pages.CP;
[Authorize(Roles = "CP,DirectorOfQuality,Dean,HOD")]
public class NotificationsModel : PageModel { public void OnGet() { } }

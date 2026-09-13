using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
namespace CourseScheduleSystem.Web.Pages.Dean;
[Authorize(Roles = "Dean")]
public class AuditLogModel : PageModel { public void OnGet() { } }

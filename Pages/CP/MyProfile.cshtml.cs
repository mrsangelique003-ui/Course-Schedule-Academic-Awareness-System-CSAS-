using CourseScheduleSystem.Web.Data;
using CourseScheduleSystem.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace CourseScheduleSystem.Web.Pages.CP;

public class MyProfileModel : CpPageModel
{
    public ClassRepresentative? CurrentCp { get; set; }

    public MyProfileModel(ApplicationDbContext context) : base(context) { }

    public async Task<IActionResult> OnGetAsync()
    {
        if (!IsCurrentCpValid())
            return Unauthorized();

        CurrentCp = await GetCurrentCpAsync();

        if (CurrentCp == null)
            return NotFound();

        return Page();
    }
}

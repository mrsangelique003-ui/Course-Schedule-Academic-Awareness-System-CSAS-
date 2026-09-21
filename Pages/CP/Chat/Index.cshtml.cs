using CourseScheduleSystem.Web.Data;
using CourseScheduleSystem.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CourseScheduleSystem.Web.Pages.CP.Chat;

public class IndexModel : CourseScheduleSystem.Web.Pages.CP.CpPageModel
{
    public List<HodMessage> Messages { get; set; } = new();

    public IndexModel(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<IActionResult> OnGetAsync()
    {
        if (!IsCurrentCpValid())
        {
            return Unauthorized();
        }

        Messages = await _context.HodMessages
            .Where(m => m.ClassRepresentativeId == CurrentCpId)
            .OrderBy(m => m.SentAt)
            .ToListAsync();

        return Page();
    }
}
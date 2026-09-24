using CourseScheduleSystem.Web.Data;
using CourseScheduleSystem.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CourseScheduleSystem.Web.Pages.Admin;

[Authorize(Roles = "Administrator")]
public class ClassRepresentativeDetailsModel : PageModel
{
    private readonly ApplicationDbContext _db;
    private readonly ILogger<ClassRepresentativeDetailsModel> _logger;


public ClassRepresentativeDetailsModel(
    ApplicationDbContext db,
    ILogger<ClassRepresentativeDetailsModel> logger)
    {
        _db = db;
        _logger = logger;
    }

    public ClassRepresentative? Representative { get; private set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        if (id <= 0)
        {
            return NotFound();
        }

        try
        {
            Representative = await _db.ClassRepresentatives
                .AsNoTracking()
                .FirstOrDefaultAsync(cp => cp.Id == id);

            if (Representative == null)
            {
                return NotFound();
            }

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "An error occurred while loading class representative details for ID {Id}.",
                id);

            return RedirectToPage("/Admin/ClassRepresentatives");
        }
    }


}

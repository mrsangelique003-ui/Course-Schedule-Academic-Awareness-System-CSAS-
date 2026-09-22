using CourseScheduleSystem.Web.Data;
using CourseScheduleSystem.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CourseScheduleSystem.Web.Pages.CP.ClassGroups;

public class IndexModel : CourseScheduleSystem.Web.Pages.CP.CpPageModel
{
	public List<ClassGroup> Groups { get; set; } = new();

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

		Groups = await _context.ClassGroups
			.Include(g => g.Course)
			.Where(g =>
				g.ClassRepresentativeId == CurrentCpId &&
				g.IsActive)
			.OrderBy(g => g.Course.Name)
			.ThenBy(g => g.GroupName)
			.ToListAsync();

		return Page();
	}
}
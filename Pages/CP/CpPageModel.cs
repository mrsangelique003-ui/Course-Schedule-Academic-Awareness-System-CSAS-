using CourseScheduleSystem.Web.Data;
using CourseScheduleSystem.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CourseScheduleSystem.Web.Pages.CP;

[Authorize]
public abstract class CpPageModel : PageModel
{
	protected readonly ApplicationDbContext _context;

	protected CpPageModel(ApplicationDbContext context)
	{
		_context = context;
	}

	protected int CurrentCpId
	{
		get
		{
			var claim =
				User.FindFirstValue(ClaimTypes.NameIdentifier);

			if (string.IsNullOrWhiteSpace(claim))
			{
				return 0;
			}

			return int.TryParse(claim, out var id) ? id : 0;
		}
	}

	protected async Task<ClassRepresentative?> GetCurrentCpAsync()
	{
		if (CurrentCpId <= 0)
		{
			return null;
		}

		return await _context.ClassRepresentatives
			.Include(cp => cp.RepresentedCourses)
			.FirstOrDefaultAsync(cp =>
				cp.Id == CurrentCpId &&
				cp.IsActive);
	}

	protected bool IsCurrentCpValid()
	{
		return CurrentCpId > 0;
	}
}
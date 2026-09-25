using CourseScheduleSystem.Web.Data;
using CourseScheduleSystem.Web.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CourseScheduleSystem.Web.Pages.CP;

public class DashboardModel : PageModel
{
    private readonly ApplicationDbContext _db;

    public DashboardModel(ApplicationDbContext db)
    {
        _db = db;
    }

    public ClassRepresentative? CurrentCp { get; set; }

    public int TotalCourses { get; set; }

    public int TotalGroups { get; set; }

    public int PendingCompletions { get; set; }

    public int UnreadMessages { get; set; }

    public List<ClassRepresentative> CisReps { get; set; } = new();

    public async Task OnGetAsync()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdClaim, out var cpId))
            return;

        CurrentCp = await _db.ClassRepresentatives
            .AsNoTracking()
            .Include(c => c.RepresentedCourses)
            .FirstOrDefaultAsync(c => c.Id == cpId);

        if (CurrentCp == null)
            return;

        TotalCourses = CurrentCp.RepresentedCourses.Count;

        TotalGroups = await _db.ClassGroups
            .AsNoTracking()
            .CountAsync(g => g.ClassRepresentativeId == cpId);

        PendingCompletions = await _db.CourseCompletions
            .AsNoTracking()
            .CountAsync(c =>
                c.ClassRepresentativeId == cpId &&
                c.Status == CourseCompletionStatus.Submitted);

        UnreadMessages = await _db.HodMessages
            .AsNoTracking()
            .CountAsync(m =>
                m.ClassRepresentativeId == cpId &&
                !m.IsRead);

        CisReps = await _db.ClassRepresentatives
            .AsNoTracking()
            .Where(c => c.IsActive && c.Faculty.Contains("Computing"))
            .OrderBy(c => c.FullName)
            .ToListAsync();
    }
}

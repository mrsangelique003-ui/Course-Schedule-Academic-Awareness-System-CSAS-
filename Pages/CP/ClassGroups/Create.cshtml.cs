using System.ComponentModel.DataAnnotations;
using CourseScheduleSystem.Web.Data;
using CourseScheduleSystem.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CourseScheduleSystem.Web.Pages.CP.ClassGroups;

public class CreateModel : CourseScheduleSystem.Web.Pages.CP.CpPageModel
{
    [BindProperty]
    public InputModel Input { get; set; } = new();

    public List<Course> AvailableCourses { get; set; } = new();

    public CreateModel(ApplicationDbContext context)
        : base(context)
    {
    }

    public class InputModel
    {
        [Required(ErrorMessage = "Please select a course.")]
        [Display(Name = "Course")]
        public int CourseId { get; set; }

        [Required(ErrorMessage = "Group name is required.")]
        [StringLength(100)]
        [Display(Name = "Group Name")]
        public string GroupName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Intake is required.")]
        [StringLength(100)]
        public string Intake { get; set; } = string.Empty;

        [Required(ErrorMessage = "Level is required.")]
        [StringLength(50)]
        public string Level { get; set; } = string.Empty;

        [Required(ErrorMessage = "Group link is required.")]
        [StringLength(500)]
        [Url(ErrorMessage = "Please enter a valid group link.")]
        [Display(Name = "Group Link")]
        public string GroupLink { get; set; } = string.Empty;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        if (!IsCurrentCpValid())
        {
            return Unauthorized();
        }

        await LoadCoursesAsync();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!IsCurrentCpValid())
        {
            return Unauthorized();
        }

        await LoadCoursesAsync();

        if (!ModelState.IsValid)
        {
            return Page();
        }

        // Make sure the selected course actually belongs
        // to the currently signed-in CP.
        var courseBelongsToCp = await _context.Courses
            .AnyAsync(c =>
                c.Id == Input.CourseId &&
                c.ClassRepresentatives
                    .Any(cp => cp.Id == CurrentCpId));

        if (!courseBelongsToCp)
        {
            ModelState.AddModelError(
                nameof(Input.CourseId),
                "You can only create a group for a course you represent.");

            return Page();
        }

        var groupName = Input.GroupName.Trim();
        var intake = Input.Intake.Trim();
        var level = Input.Level.Trim();
        var groupLink = Input.GroupLink.Trim();

        // Prevent duplicate active groups for the same CP/course/group.
        var duplicateExists = await _context.ClassGroups
            .AnyAsync(g =>
                g.ClassRepresentativeId == CurrentCpId &&
                g.CourseId == Input.CourseId &&
                g.IsActive &&
                g.GroupName.ToLower() == groupName.ToLower());

        if (duplicateExists)
        {
            ModelState.AddModelError(
                nameof(Input.GroupName),
                "A group with this name already exists for this course.");

            return Page();
        }

        var group = new ClassGroup
        {
            CourseId = Input.CourseId,
            ClassRepresentativeId = CurrentCpId,
            GroupName = groupName,
            Intake = intake,
            Level = level,
            GroupLink = groupLink,
            IsPublished = false,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = null
        };

        _context.ClassGroups.Add(group);

        await _context.SaveChangesAsync();

        return RedirectToPage("/CP/ClassGroups");
    }

    private async Task LoadCoursesAsync()
    {
        AvailableCourses = await _context.Courses
            .Where(c =>
                c.ClassRepresentatives
                    .Any(cp => cp.Id == CurrentCpId))
            .OrderBy(c => c.Name)
            .ToListAsync();
    }
}
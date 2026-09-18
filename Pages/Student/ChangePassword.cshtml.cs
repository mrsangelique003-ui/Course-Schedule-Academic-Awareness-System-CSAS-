using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using CourseScheduleSystem.Web.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CourseScheduleSystem.Web.Pages.Student;

[Authorize(Roles = "Student")]
public class ChangePasswordModel : PageModel
{
    private readonly ApplicationDbContext _db;
    private readonly IPasswordHasher<CourseScheduleSystem.Web.Models.Student> _passwordHasher;


public ChangePasswordModel(
    ApplicationDbContext db,
    IPasswordHasher<CourseScheduleSystem.Web.Models.Student> passwordHasher)
    {
        _db = db;
        _passwordHasher = passwordHasher;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public string? StudentName { get; private set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var userIdValue =
            User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdValue, out var studentId))
        {
            return RedirectToPage("/Account/Login");
        }

        var student = await _db.Students
            .AsNoTracking()
            .FirstOrDefaultAsync(s =>
                s.Id == studentId &&
                s.IsActive);

        if (student == null)
        {
            return Forbid();
        }

        StudentName = student.FullName;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var userIdValue =
            User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdValue, out var studentId))
        {
            return RedirectToPage("/Account/Login");
        }

        var student = await _db.Students
            .FirstOrDefaultAsync(s =>
                s.Id == studentId &&
                s.IsActive);

        if (student == null)
        {
            return Forbid();
        }

        StudentName = student.FullName;

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var verificationResult =
            _passwordHasher.VerifyHashedPassword(
                student,
                student.PasswordHash,
                Input.CurrentPassword);

        if (verificationResult == PasswordVerificationResult.Failed)
        {
            ModelState.AddModelError(
                "Input.CurrentPassword",
                "The current password is incorrect.");

            return Page();
        }

        if (Input.CurrentPassword == Input.NewPassword)
        {
            ModelState.AddModelError(
                "Input.NewPassword",
                "Your new password must be different from your current password.");

            return Page();
        }

        student.PasswordHash =
            _passwordHasher.HashPassword(
                student,
                Input.NewPassword);

        await _db.SaveChangesAsync();

        TempData["PasswordChanged"] =
            "Your password has been changed successfully.";

        return RedirectToPage("/Student/Profile");
    }

    public class InputModel
    {
        [Required(ErrorMessage = "Current password is required.")]
        [DataType(DataType.Password)]
        [Display(Name = "Current Password")]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "New password is required.")]
        [StringLength(
            100,
            MinimumLength = 8,
            ErrorMessage = "The new password must be at least 8 characters long.")]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please confirm your new password.")]
        [DataType(DataType.Password)]
        [Compare(
            "NewPassword",
            ErrorMessage = "The new passwords do not match.")]
        [Display(Name = "Confirm New Password")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

}

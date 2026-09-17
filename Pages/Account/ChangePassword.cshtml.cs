using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using CourseScheduleSystem.Web.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;


[Authorize]
public class ChangePasswordModel : PageModel
{
    private readonly ApplicationDbContext _db;

    public ChangePasswordModel(ApplicationDbContext db)
    {
        _db = db;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public string? StatusMessage { get; set; }

    public class InputModel
    {
        [Required(ErrorMessage = "Current password is required.")]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "New password is required.")]
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please confirm your new password.")]
        [DataType(DataType.Password)]
        [Compare(nameof(NewPassword), ErrorMessage = "New password and confirmation do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var role = User.FindFirstValue(ClaimTypes.Role);

        bool updated = role switch
        {
            "Administrator" => await UpdatePasswordAsync(_db.Administrators, userId),
            "Lecturer" => await UpdatePasswordAsync(_db.Lecturers, userId),
            "ClassRepresentative" => await UpdatePasswordAsync(_db.ClassRepresentatives, userId),
            "Student" => await UpdatePasswordAsync(_db.Students, userId),
            _ => false
        };

        if (!updated)
        {
            ModelState.AddModelError(string.Empty, "Current password verification failed.");
            return Page();
        }

        // Re-sign in or redirect with success message
        TempData["StatusMessage"] = "Your password has been updated successfully.";
        return RedirectToPage("/Account/ChangePassword");
    }

    private async Task<bool> UpdatePasswordAsync<T>(DbSet<T> dbSet, int id) where T : class
    {
        var entity = await dbSet.FindAsync(id);
        if (entity == null) return false;

        var hasher = new PasswordHasher<T>();
        var currentHash = (string)entity.GetType().GetProperty("PasswordHash")!.GetValue(entity)!;

        if (hasher.VerifyHashedPassword(entity, currentHash, Input.CurrentPassword) == PasswordVerificationResult.Failed)
        {
            return false;
        }

        entity.GetType().GetProperty("PasswordHash")!.SetValue(entity, hasher.HashPassword(entity, Input.NewPassword));
        await _db.SaveChangesAsync();
        return true;
    }
}
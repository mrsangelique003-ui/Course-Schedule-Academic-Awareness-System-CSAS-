using CourseScheduleSystem.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace CourseScheduleSystem.Web.Pages;

public class LoginModel : PageModel
{
    private readonly SignInManager<ApplicationUser> _signIn;
    private readonly UserManager<ApplicationUser>  _users;

    public LoginModel(
        SignInManager<ApplicationUser> signIn,
        UserManager<ApplicationUser>  users)
    {
        _signIn = signIn;
        _users  = users;
    }

    [BindProperty]
    [Required(ErrorMessage = "Registration Number is required.")]
    public string RegNo { get; set; } = string.Empty;

    [BindProperty]
    [Required(ErrorMessage = "Password is required.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [BindProperty]
    public bool RememberMe { get; set; }

    public string ErrorMessage { get; set; } = string.Empty;

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        // Support login by RegNo (stored as UserName) or email
        var user = await _users.FindByNameAsync(RegNo)
                ?? await _users.FindByEmailAsync(RegNo);

        if (user is null || !user.IsActive)
        {
            ErrorMessage = "Invalid credentials or account is inactive.";
            return Page();
        }

        var result = await _signIn.PasswordSignInAsync(
            user, Password, RememberMe, lockoutOnFailure: false);

        if (!result.Succeeded)
        {
            ErrorMessage = "Invalid Registration Number or password.";
            return Page();
        }

        // Role-based redirect
        if (await _users.IsInRoleAsync(user, "Student"))
            return RedirectToPage("/Student/Dashboard");
        if (await _users.IsInRoleAsync(user, "CP"))
            return RedirectToPage("/CP/Dashboard");
        if (await _users.IsInRoleAsync(user, "DirectorOfQuality"))
            return RedirectToPage("/Quality/Dashboard");
        if (await _users.IsInRoleAsync(user, "Dean"))
            return RedirectToPage("/Dean/Dashboard");
        if (await _users.IsInRoleAsync(user, "HOD"))
            return RedirectToPage("/Dean/Dashboard");

        return RedirectToPage("/Index");
    }
}

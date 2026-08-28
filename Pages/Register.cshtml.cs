using CourseScheduleSystem.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace CourseScheduleSystem.Web.Pages;

public class RegisterModel : PageModel
{
    private readonly UserManager<ApplicationUser>  _users;
    private readonly SignInManager<ApplicationUser> _signIn;

    public RegisterModel(
        UserManager<ApplicationUser>  users,
        SignInManager<ApplicationUser> signIn)
    {
        _users  = users;
        _signIn = signIn;
    }

    [BindProperty, Required(ErrorMessage = "Registration Number is required.")]
    public string RegNo { get; set; } = string.Empty;

    [BindProperty, Required(ErrorMessage = "Full name is required.")]
    public string FullName { get; set; } = string.Empty;

    [BindProperty, Required(ErrorMessage = "Password is required."), MinLength(6)]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [BindProperty, Required(ErrorMessage = "Please confirm your password.")]
    [DataType(DataType.Password), Compare("Password", ErrorMessage = "Passwords do not match.")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [BindProperty, Required]
    public string StudySession { get; set; } = "Day";

    public string ErrorMessage { get; set; } = string.Empty;

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        if (await _users.FindByNameAsync(RegNo) is not null)
        {
            ErrorMessage = "A student with this Registration Number already exists.";
            return Page();
        }

        var user = new ApplicationUser
        {
            UserName     = RegNo,
            RegNo        = RegNo,
            FullName     = FullName,
            Email        = $"{RegNo.ToLower()}@student.unilak.ac.rw",
            Department   = "CIS",
            StudySession = StudySession,
            IsActive     = true
        };

        var result = await _users.CreateAsync(user, Password);
        if (!result.Succeeded)
        {
            ErrorMessage = string.Join(" ", result.Errors.Select(e => e.Description));
            return Page();
        }

        await _users.AddToRoleAsync(user, "Student");
        await _signIn.SignInAsync(user, isPersistent: false);
        return RedirectToPage("/Student/Dashboard");
    }
}

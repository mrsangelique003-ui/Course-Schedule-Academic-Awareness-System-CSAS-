using System.Security.Claims;
using System.ComponentModel.DataAnnotations;
using CourseScheduleSystem.Web.Data;
using CourseScheduleSystem.Web.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CourseScheduleSystem.Web.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public LoginModel(ApplicationDbContext db)
        {
            _db = db;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public class InputModel
        {
            [Required(ErrorMessage = "Email is required.")]
            [EmailAddress(ErrorMessage = "Invalid email address.")]
            public string Email { get; set; } = string.Empty;

            [Required(ErrorMessage = "Password is required.")]
            [DataType(DataType.Password)]
            public string Password { get; set; } = string.Empty;

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        private record AccountMatch(
            int Id, string FullName, string Email, string Role, bool MustChangePassword);

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var email = Input.Email.Trim();

            var match =
                await CheckAdministratorAsync(email, Input.Password) ??
                await CheckLecturerAsync(email, Input.Password) ??
                await CheckClassRepresentativeAsync(email, Input.Password) ??
                await CheckStudentAsync(email, Input.Password);

            if (match is null)
            {
                ModelState.AddModelError(string.Empty, "Invalid email or password.");
                return Page();
            }

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, match.Id.ToString()),
                new(ClaimTypes.Name, match.FullName),
                new(ClaimTypes.Email, match.Email),
                new(ClaimTypes.Role, match.Role)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = Input.RememberMe,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
                });

            if (match.MustChangePassword)
                return RedirectToPage("/Account/ChangePassword");

            return RedirectToPage("/Dashboard");
        }

        private async Task<AccountMatch?> CheckAdministratorAsync(string email, string password)
        {
            var account = await _db.Administrators.FirstOrDefaultAsync(
                a => a.IsActive && a.Email != null && a.Email.ToLower() == email.ToLower());
            if (account is null) return null;

            var result = new PasswordHasher<Administrator>()
                .VerifyHashedPassword(account, account.PasswordHash, password);

            return result == PasswordVerificationResult.Failed ? null
                : new AccountMatch(account.Id, account.FullName, account.Email!, "Administrator", account.MustChangePassword);
        }

        private async Task<AccountMatch?> CheckLecturerAsync(string email, string password)
        {
            var account = await _db.Lecturers.FirstOrDefaultAsync(
                l => l.IsActive && l.Email != null && l.Email.ToLower() == email.ToLower());
            if (account is null) return null;

            var result = new PasswordHasher<Lecturer>()
                .VerifyHashedPassword(account, account.PasswordHash, password);

            return result == PasswordVerificationResult.Failed ? null
                : new AccountMatch(account.Id, account.FullName, account.Email!, "Lecturer", account.MustChangePassword);
        }

        private async Task<AccountMatch?> CheckClassRepresentativeAsync(string email, string password)
        {
            var account = await _db.ClassRepresentatives.FirstOrDefaultAsync(
                c => c.IsActive && c.Email != null && c.Email.ToLower() == email.ToLower());
            if (account is null) return null;

            var result = new PasswordHasher<ClassRepresentative>()
                .VerifyHashedPassword(account, account.PasswordHash, password);

            return result == PasswordVerificationResult.Failed ? null
                : new AccountMatch(account.Id, account.FullName, account.Email!, "ClassRepresentative", account.MustChangePassword);
        }

        private async Task<AccountMatch?> CheckStudentAsync(string email, string password)
        {
            var account = await _db.Students.FirstOrDefaultAsync(
                s => s.IsActive && s.Email != null && s.Email.ToLower() == email.ToLower());
            if (account is null) return null;

            var result = new PasswordHasher<Student>()
                .VerifyHashedPassword(account, account.PasswordHash, password);

            return result == PasswordVerificationResult.Failed ? null
                : new AccountMatch(account.Id, account.FullName, account.Email!, "Student", account.MustChangePassword);
        }
    }
}
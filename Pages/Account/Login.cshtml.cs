using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
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
            [Required(ErrorMessage = "Email or registration number is required.")]
            [StringLength(256)]
            [Display(Name = "Email or Registration Number")]
            public string Identifier { get; set; } = string.Empty;

            [Required(ErrorMessage = "Password is required.")]
            [DataType(DataType.Password)]
            public string Password { get; set; } = string.Empty;

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        private record AccountMatch(
            int Id,
            string FullName,
            string? Email,
            string Role,
            string? RegistrationNumber);

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var identifier = Input.Identifier.Trim();

            var match =
                await CheckAdministratorAsync(identifier, Input.Password) ??
                await CheckLecturerAsync(identifier, Input.Password) ??
                await CheckClassRepresentativeAsync(identifier, Input.Password) ??
                await CheckStudentAsync(identifier, Input.Password);

            if (match is null)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Invalid email/registration number or password.");

                return Page();
            }

            var claims = new List<Claim>
            {
                new(
                    ClaimTypes.NameIdentifier,
                    match.Id.ToString()),

                new(
                    ClaimTypes.Name,
                    match.FullName),

                new(
                    ClaimTypes.Role,
                    match.Role)
            };

            if (!string.IsNullOrWhiteSpace(match.Email))
            {
                claims.Add(
                    new Claim(
                        ClaimTypes.Email,
                        match.Email));
            }

            if (!string.IsNullOrWhiteSpace(match.RegistrationNumber))
            {
                claims.Add(
                    new Claim(
                        "RegNo",
                        match.RegistrationNumber));
            }

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = Input.RememberMe,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
                });

            return RedirectToPage("/Admin/Dashboard");
        }

        private async Task<AccountMatch?> CheckAdministratorAsync(
            string identifier,
            string password)
        {
            var account = await _db.Administrators
                .FirstOrDefaultAsync(a =>
                    a.IsActive &&
                    a.Email != null &&
                    a.Email.ToLower() == identifier.ToLower());

            if (account is null)
                return null;

            var result = new PasswordHasher<Administrator>()
                .VerifyHashedPassword(
                    account,
                    account.PasswordHash,
                    password);

            if (result == PasswordVerificationResult.Failed)
                return null;

            return new AccountMatch(
                account.Id,
                account.FullName,
                account.Email,
                "Administrator",
                null);
        }

        private async Task<AccountMatch?> CheckLecturerAsync(
            string identifier,
            string password)
        {
            var account = await _db.Lecturers
                .FirstOrDefaultAsync(l =>
                    l.IsActive &&
                    l.Email != null &&
                    l.Email.ToLower() == identifier.ToLower());

            if (account is null)
                return null;

            var result = new PasswordHasher<Lecturer>()
                .VerifyHashedPassword(
                    account,
                    account.PasswordHash,
                    password);

            if (result == PasswordVerificationResult.Failed)
                return null;

            return new AccountMatch(
                account.Id,
                account.FullName,
                account.Email,
                "Lecturer",
                null);
        }

        private async Task<AccountMatch?> CheckClassRepresentativeAsync(
            string identifier,
            string password)
        {
            var account = await _db.ClassRepresentatives
                .FirstOrDefaultAsync(c =>
                    c.IsActive &&
                    c.RegNo.ToLower() == identifier.ToLower());

            if (account is null)
                return null;

            var result = new PasswordHasher<ClassRepresentative>()
                .VerifyHashedPassword(
                    account,
                    account.PasswordHash,
                    password);

            if (result == PasswordVerificationResult.Failed)
                return null;

            return new AccountMatch(
                account.Id,
                account.FullName,
                account.Email,
                "ClassRepresentative",
                account.RegNo);
        }

        private async Task<AccountMatch?> CheckStudentAsync(
            string identifier,
            string password)
        {
            var account = await _db.Students
                .FirstOrDefaultAsync(s =>
                    s.IsActive &&
                    s.RegNo.ToLower() == identifier.ToLower());

            if (account is null)
                return null;

            var result = new PasswordHasher<CourseScheduleSystem.Web.Models.Student>()
                .VerifyHashedPassword(
                    account,
                    account.PasswordHash,
                    password);

            if (result == PasswordVerificationResult.Failed)
                return null;

            return new AccountMatch(
                account.Id,
                account.FullName,
                account.Email,
                "Student",
                account.RegNo);
        }
    }
}


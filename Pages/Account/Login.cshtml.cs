using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace CourseScheduleSystem.Web.Pages.Account
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

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

        public void OnGet()
        {
            // Optional: Add logic here if you need to do something when the page loads.
            // For example, redirect if already logged in.
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // TODO: Implement your actual authentication logic here.
            // For now, we will just simulate a successful login.

            // Example:
            // if (Input.Email == "admin@unilak.ac.rw" && Input.Password == "password")
            // {
            //     // Sign in the user
            //     return RedirectToPage("/Index");
            // }
            // else
            // {
            //     ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            //     return Page();
            // }

            // Temporary: Redirect to home page on any submission for testing
            return RedirectToPage("/Index");
        }
    }
}
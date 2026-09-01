using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace CourseScheduleSystem.Web.Pages;

public class ForgotPasswordModel : PageModel
{
    [BindProperty, Required(ErrorMessage = "Registration Number is required.")]
    public string RegNo { get; set; } = string.Empty;

    public bool Submitted { get; set; }

    public void OnGet() { }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid) return Page();
        // In production this would trigger an email/SMS reset flow.
        // For now, direct the student to contact the CIS Department admin.
        Submitted = true;
        return Page();
    }
}

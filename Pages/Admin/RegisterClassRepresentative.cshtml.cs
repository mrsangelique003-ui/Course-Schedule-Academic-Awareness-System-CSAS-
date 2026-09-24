using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using CourseScheduleSystem.Web.Data;
using CourseScheduleSystem.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CourseScheduleSystem.Web.Pages.Admin;

[Authorize(Roles = "Administrator")]
public class RegisterClassRepresentativeModel : PageModel
{
    private const string DefaultPassword = "M00del!!";


private const string FacultyName =
    "Faculty of Computing and Information Technology";

    private static readonly string[] AllowedDepartments =
    [
        "Information system and management",
    "information Technology",
    "Software Engeenering",
    "Networking",
    "Multimedia"
    ];

    private static readonly string[] AllowedIntakes =
    [
        "January",
    "March",
    "September"
    ];

    private static readonly string[] AllowedLevels =
    [
        "Year 1",
    "Year 2",
    "Year 3"
    ];

    private static readonly StudySession[] AllowedStudySessions =
    [
        StudySession.Day,
    StudySession.Evening,
    StudySession.Weekend
    ];

    private readonly ApplicationDbContext _context;
    private readonly ILogger<RegisterClassRepresentativeModel> _logger;
    private readonly PasswordHasher<ClassRepresentative> _passwordHasher;

    public RegisterClassRepresentativeModel(
        ApplicationDbContext context,
        ILogger<RegisterClassRepresentativeModel> logger)
    {
        _context = context;
        _logger = logger;
        _passwordHasher = new PasswordHasher<ClassRepresentative>();
    }

    public IReadOnlyList<string> Departments =>
        AllowedDepartments;

    public IReadOnlyList<string> Intakes =>
        AllowedIntakes;

    public IReadOnlyList<string> Levels =>
        AllowedLevels;

    public IReadOnlyList<StudySession> StudySessions =>
        AllowedStudySessions;

    [BindProperty]
    public ClassRepresentativeInput Input { get; set; } = new();

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        NormalizeInput();

        ValidateInput();

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var normalizedRegNo = Input.RegNo;

        var registrationExists =
            await _context.ClassRepresentatives
                .AsNoTracking()
                .AnyAsync(cp => cp.RegNo == normalizedRegNo);

        if (registrationExists)
        {
            ModelState.AddModelError(
                "Input.RegNo",
                $"Registration number '{normalizedRegNo}' is already registered. Please use a different registration number.");

            ModelState.AddModelError(
                string.Empty,
                $"A Class Representative with registration number '{normalizedRegNo}' already exists.");

            return Page();
        }

        var representative = new ClassRepresentative
        {
            RegNo = normalizedRegNo,
            FullName = Input.FullName,
            Email = Input.Email,
            PhoneNumber = Input.PhoneNumber,
            Nationality = NormalizeOptional(Input.Nationality),
            Department = Input.Department,
            Faculty = FacultyName,
            Year = GetYear(Input.Level),
            StudySession = Input.StudySession,
            Intake = Input.Intake,
            Level = Input.Level,
            IsActive = true,
            AssignedAt = DateTime.UtcNow,
            SignatureImageData = Input.SignatureImageData
        };

        representative.PasswordHash =
            _passwordHasher.HashPassword(
                representative,
                DefaultPassword);

        try
        {
            _context.ClassRepresentatives.Add(representative);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                $"Class Representative {representative.FullName} was registered successfully.";

            return RedirectToPage(
                "/Admin/ClassRepresentatives");
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(
                ex,
                "Database error while registering Class Representative {RegNo}.",
                normalizedRegNo);

            ModelState.AddModelError(
                string.Empty,
                "The Class Representative could not be registered because of a database error. Please try again.");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unexpected error while registering Class Representative {RegNo}.",
                normalizedRegNo);

            ModelState.AddModelError(
                string.Empty,
                "An unexpected error occurred while registering the Class Representative. Please try again.");
        }

        return Page();
    }

    private void NormalizeInput()
    {
        Input.RegNo =
            Input.RegNo?.Trim() ?? string.Empty;

        Input.FullName =
            Input.FullName?.Trim() ?? string.Empty;

        Input.Email =
            Input.Email?.Trim() ?? string.Empty;

        Input.PhoneNumber =
            Input.PhoneNumber?.Trim() ?? string.Empty;

        Input.Nationality =
            NormalizeOptional(Input.Nationality);

        Input.Department =
            Input.Department?.Trim() ?? string.Empty;

        Input.Intake =
            Input.Intake?.Trim() ?? string.Empty;

        Input.Level =
            Input.Level?.Trim() ?? string.Empty;

        Input.SignatureImageData =
            Input.SignatureImageData?.Trim() ?? string.Empty;
    }

    private void ValidateInput()
    {
        if (string.IsNullOrWhiteSpace(Input.RegNo))
        {
            ModelState.AddModelError(
                "Input.RegNo",
                "Registration number is required.");
        }

        if (string.IsNullOrWhiteSpace(Input.FullName))
        {
            ModelState.AddModelError(
                "Input.FullName",
                "Full name is required.");
        }

        if (string.IsNullOrWhiteSpace(Input.Email))
        {
            ModelState.AddModelError(
                "Input.Email",
                "Email address is required.");
        }
        else if (!IsValidEmail(Input.Email))
        {
            ModelState.AddModelError(
                "Input.Email",
                "Please enter a valid email address.");
        }

        if (string.IsNullOrWhiteSpace(Input.PhoneNumber))
        {
            ModelState.AddModelError(
                "Input.PhoneNumber",
                "Phone number is required.");
        }

        if (!AllowedDepartments.Contains(
                Input.Department,
                StringComparer.Ordinal))
        {
            ModelState.AddModelError(
                "Input.Department",
                "Please select a valid department.");
        }

        if (!AllowedIntakes.Contains(
                Input.Intake,
                StringComparer.Ordinal))
        {
            ModelState.AddModelError(
                "Input.Intake",
                "Please select a valid intake.");
        }

        if (!AllowedLevels.Contains(
                Input.Level,
                StringComparer.Ordinal))
        {
            ModelState.AddModelError(
                "Input.Level",
                "Please select Year 1, Year 2, or Year 3.");
        }

        if (!AllowedStudySessions.Contains(
                Input.StudySession))
        {
            ModelState.AddModelError(
                "Input.StudySession",
                "Please select a valid study session.");
        }

        ValidateSignature();
    }

    private void ValidateSignature()
    {
        if (string.IsNullOrWhiteSpace(Input.SignatureImageData))
        {
            ModelState.AddModelError(
                "Input.SignatureImageData",
                "A signature is required.");
            return;
        }

        if (!Input.SignatureImageData.StartsWith(
                "data:image/png;base64,",
                StringComparison.OrdinalIgnoreCase))
        {
            ModelState.AddModelError(
                "Input.SignatureImageData",
                "The signature format is invalid.");
            return;
        }

        var base64Data =
            Input.SignatureImageData[
                "data:image/png;base64,".Length..];

        if (string.IsNullOrWhiteSpace(base64Data))
        {
            ModelState.AddModelError(
                "Input.SignatureImageData",
                "A valid signature is required.");
            return;
        }

        try
        {
            var signatureBytes =
                Convert.FromBase64String(base64Data);

            if (signatureBytes.Length < 100)
            {
                ModelState.AddModelError(
                    "Input.SignatureImageData",
                    "The signature is too small or incomplete. Please sign again.");
            }
        }
        catch (FormatException)
        {
            ModelState.AddModelError(
                "Input.SignatureImageData",
                "The signature data is invalid. Please sign again.");
        }
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            var address = new MailAddress(email);

            return string.Equals(
                address.Address,
                email,
                StringComparison.OrdinalIgnoreCase);
        }
        catch (FormatException)
        {
            return false;
        }
    }

    private static int GetYear(string level)
    {
        return level switch
        {
            "Year 1" => 1,
            "Year 2" => 2,
            "Year 3" => 3,
            _ => 1
        };
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }

    public class ClassRepresentativeInput
    {
        [Required]
        [StringLength(50)]
        public string RegNo { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(256)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Phone]
        [StringLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Nationality { get; set; }

        [Required]
        public string Department { get; set; } = string.Empty;

        [Required]
        public string Intake { get; set; } = string.Empty;

        [Required]
        public string Level { get; set; } = string.Empty;

        public StudySession StudySession { get; set; } =
            StudySession.Day;

        [Required]
        public string SignatureImageData { get; set; } = string.Empty;
    }


}

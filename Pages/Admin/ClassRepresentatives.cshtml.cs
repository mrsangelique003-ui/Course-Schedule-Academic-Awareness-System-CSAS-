using System.ComponentModel.DataAnnotations;
using CourseScheduleSystem.Web.Data;
using CourseScheduleSystem.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CourseScheduleSystem.Web.Pages.Admin;

[Authorize(Roles = "Administrator")]
public class ClassRepresentativesModel : PageModel
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
        "1",
    "2",
    "3"
    ];

    private static readonly StudySession[] AllowedStudySessions =
    [
        StudySession.Day,
    StudySession.Evening,
    StudySession.Weekend
    ];

    private readonly ApplicationDbContext _db;
    private readonly ILogger<ClassRepresentativesModel> _logger;
    private readonly PasswordHasher<ClassRepresentative> _passwordHasher;

    public ClassRepresentativesModel(
        ApplicationDbContext db,
        ILogger<ClassRepresentativesModel> logger)
    {
        _db = db;
        _logger = logger;
        _passwordHasher = new PasswordHasher<ClassRepresentative>();
    }

    [BindProperty(SupportsGet = true)]
    public string? Search { get; set; }

    [BindProperty(SupportsGet = true)]
    public string Status { get; set; } = "All";

    [BindProperty(SupportsGet = true)]
    public int PageNumber { get; set; } = 1;

    [BindProperty]
    public ClassRepresentativeInput Input { get; set; } = new();

    public const int PageSize = 10;

    public int TotalRepresentatives { get; private set; }

    public int ActiveRepresentatives { get; private set; }

    public int InactiveRepresentatives { get; private set; }

    public int DepartmentCount { get; private set; }

    public int TotalPages { get; private set; }

    public bool HasPreviousPage => PageNumber > 1;

    public bool HasNextPage => PageNumber < TotalPages;

    public bool IsFiltered =>
        !string.IsNullOrWhiteSpace(Search) ||
        !Status.Equals("All", StringComparison.OrdinalIgnoreCase);

    public string? ErrorMessage { get; private set; }

    public string? SuccessMessage { get; private set; }

    public List<ClassRepresentativeRow> ClassRepresentatives { get; private set; } = [];

    public IReadOnlyList<string> Departments => AllowedDepartments;

    public IReadOnlyList<string> Intakes => AllowedIntakes;

    public IReadOnlyList<string> Levels => AllowedLevels;

    public IReadOnlyList<StudySession> StudySessions => AllowedStudySessions;

    public async Task OnGetAsync()
    {
        await LoadRepresentativesAsync();
    }

    public async Task<IActionResult> OnPostCreateAsync()
    {
        NormalizeInput();
        ClearValidationForInput();

        ValidateInput();

        if (!ModelState.IsValid)
        {
            await LoadRepresentativesAsync();
            return Page();
        }

        try
        {
            var registrationNumber = Input.RegNo;

            var existingRepresentative = await _db.ClassRepresentatives
                .AsNoTracking()
                .AnyAsync(cp => cp.RegNo == registrationNumber);

            if (existingRepresentative)
            {
                ModelState.AddModelError(
                    "Input.RegNo",
                    "A class representative with this registration number already exists.");

                await LoadRepresentativesAsync();
                return Page();
            }

            var representative = new ClassRepresentative
            {
                RegNo = registrationNumber,
                FullName = Input.FullName,
                Email = Input.Email,
                PhoneNumber = Input.PhoneNumber,
                Nationality = NormalizeOptional(Input.Nationality),
                Department = Input.Department,
                Faculty = FacultyName,
                Year = int.Parse(Input.Level),
                StudySession = Input.StudySession,
                Intake = Input.Intake,
                Level = Input.Level,
                IsActive = true,
                AssignedAt = DateTime.UtcNow,
                SignatureImageData = string.Empty
            };

            representative.PasswordHash =
                _passwordHasher.HashPassword(
                    representative,
                    DefaultPassword);

            _db.ClassRepresentatives.Add(representative);

            await _db.SaveChangesAsync();

            TempData["SuccessMessage"] =
                $"{representative.FullName} was registered successfully.";

            return RedirectToPage(
                "/Admin/ClassRepresentatives",
                new
                {
                    Search,
                    Status,
                    PageNumber
                });
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(
                ex,
                "Database error while creating class representative {RegNo}.",
                Input.RegNo);

            ModelState.AddModelError(
                string.Empty,
                "The class representative could not be registered because the database could not save the record.");

            await LoadRepresentativesAsync();
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unexpected error while creating class representative {RegNo}.",
                Input.RegNo);

            ModelState.AddModelError(
                string.Empty,
                "Something went wrong while registering the class representative.");

            await LoadRepresentativesAsync();
            return Page();
        }
    }

    public async Task<IActionResult> OnPostEditAsync()
    {
        NormalizeInput();
        ClearValidationForInput();

        if (Input.Id <= 0)
        {
            ModelState.AddModelError(
                string.Empty,
                "The selected class representative could not be identified.");

            await LoadRepresentativesAsync();
            return Page();
        }

        ValidateInput();

        if (!ModelState.IsValid)
        {
            await LoadRepresentativesAsync();
            return Page();
        }

        try
        {
            var representative = await _db.ClassRepresentatives
                .FirstOrDefaultAsync(cp => cp.Id == Input.Id);

            if (representative == null)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "The class representative could not be found. It may have already been removed.");

                await LoadRepresentativesAsync();
                return Page();
            }

            var registrationNumber = Input.RegNo;

            var duplicateRegistrationNumber =
                await _db.ClassRepresentatives
                    .AsNoTracking()
                    .AnyAsync(cp =>
                        cp.Id != Input.Id &&
                        cp.RegNo == registrationNumber);

            if (duplicateRegistrationNumber)
            {
                ModelState.AddModelError(
                    "Input.RegNo",
                    "Another class representative already uses this registration number.");

                await LoadRepresentativesAsync();
                return Page();
            }

            representative.RegNo = registrationNumber;
            representative.FullName = Input.FullName;
            representative.Email = Input.Email;
            representative.PhoneNumber = Input.PhoneNumber;
            representative.Nationality = NormalizeOptional(Input.Nationality);
            representative.Department = Input.Department;
            representative.Faculty = FacultyName;
            representative.Year = int.Parse(Input.Level);
            representative.StudySession = Input.StudySession;
            representative.Intake = Input.Intake;
            representative.Level = Input.Level;

            await _db.SaveChangesAsync();

            TempData["SuccessMessage"] =
                $"{representative.FullName} was updated successfully.";

            return RedirectToPage(
                "/Admin/ClassRepresentatives",
                new
                {
                    Search,
                    Status,
                    PageNumber
                });
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(
                ex,
                "Database error while editing class representative {Id}.",
                Input.Id);

            ModelState.AddModelError(
                string.Empty,
                "The changes could not be saved because of a database error.");

            await LoadRepresentativesAsync();
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unexpected error while editing class representative {Id}.",
                Input.Id);

            ModelState.AddModelError(
                string.Empty,
                "Something went wrong while updating the class representative.");

            await LoadRepresentativesAsync();
            return Page();
        }
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        if (id <= 0)
        {
            TempData["ErrorMessage"] =
                "The selected class representative could not be identified.";

            return RedirectToPage(
                "/Admin/ClassRepresentatives",
                new
                {
                    Search,
                    Status,
                    PageNumber
                });
        }

        try
        {
            var representative = await _db.ClassRepresentatives
                .FirstOrDefaultAsync(cp => cp.Id == id);

            if (representative == null)
            {
                TempData["ErrorMessage"] =
                    "The class representative could not be found. It may have already been removed.";

                return RedirectToPage(
                    "/Admin/ClassRepresentatives",
                    new
                    {
                        Search,
                        Status,
                        PageNumber
                    });
            }

            var representativeName = representative.FullName;

            _db.ClassRepresentatives.Remove(representative);

            await _db.SaveChangesAsync();

            TempData["SuccessMessage"] =
                $"{representativeName} was deleted successfully.";

            return RedirectToPage(
                "/Admin/ClassRepresentatives",
                new
                {
                    Search,
                    Status,
                    PageNumber
                });
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(
                ex,
                "Database error while deleting class representative {Id}.",
                id);

            TempData["ErrorMessage"] =
                "The class representative could not be deleted because other records depend on this account.";

            return RedirectToPage(
                "/Admin/ClassRepresentatives",
                new
                {
                    Search,
                    Status,
                    PageNumber
                });
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unexpected error while deleting class representative {Id}.",
                id);

            TempData["ErrorMessage"] =
                "Something went wrong while deleting the class representative.";

            return RedirectToPage(
                "/Admin/ClassRepresentatives",
                new
                {
                    Search,
                    Status,
                    PageNumber
                });
        }
    }

    private async Task LoadRepresentativesAsync()
    {
        try
        {
            Search = Search?.Trim();

            Status = Status?.Trim() ?? "All";

            if (!Status.Equals(
                    "Active",
                    StringComparison.OrdinalIgnoreCase) &&
                !Status.Equals(
                    "Inactive",
                    StringComparison.OrdinalIgnoreCase))
            {
                Status = "All";
            }
            else if (Status.Equals(
                         "Active",
                         StringComparison.OrdinalIgnoreCase))
            {
                Status = "Active";
            }
            else
            {
                Status = "Inactive";
            }

            if (PageNumber < 1)
            {
                PageNumber = 1;
            }

            var filteredQuery = _db.ClassRepresentatives
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(Search))
            {
                var searchPattern = $"%{Search}%";

                filteredQuery = filteredQuery.Where(cp =>
                    EF.Functions.Like(cp.RegNo, searchPattern) ||
                    EF.Functions.Like(cp.FullName, searchPattern) ||
                    (cp.Email != null &&
                     EF.Functions.Like(cp.Email, searchPattern)) ||
                    (cp.PhoneNumber != null &&
                     EF.Functions.Like(cp.PhoneNumber, searchPattern)) ||
                    EF.Functions.Like(cp.Department, searchPattern) ||
                    EF.Functions.Like(cp.Faculty, searchPattern) ||
                    EF.Functions.Like(cp.Intake, searchPattern) ||
                    EF.Functions.Like(cp.Level, searchPattern));
            }

            if (Status == "Active")
            {
                filteredQuery = filteredQuery
                    .Where(cp => cp.IsActive);
            }
            else if (Status == "Inactive")
            {
                filteredQuery = filteredQuery
                    .Where(cp => !cp.IsActive);
            }

            TotalRepresentatives =
                await filteredQuery.CountAsync();

            ActiveRepresentatives =
                await filteredQuery.CountAsync(cp => cp.IsActive);

            InactiveRepresentatives =
                await filteredQuery.CountAsync(cp => !cp.IsActive);

            DepartmentCount =
                await filteredQuery
                    .Select(cp => cp.Department)
                    .Distinct()
                    .CountAsync();

            TotalPages = TotalRepresentatives == 0
                ? 1
                : (int)Math.Ceiling(
                    TotalRepresentatives / (double)PageSize);

            if (PageNumber > TotalPages)
            {
                PageNumber = TotalPages;
            }

            var skip = (PageNumber - 1) * PageSize;

            ClassRepresentatives =
                await filteredQuery
                    .OrderBy(cp => cp.FullName)
                    .ThenBy(cp => cp.Id)
                    .Skip(skip)
                    .Take(PageSize)
                    .Select(cp => new ClassRepresentativeRow
                    {
                        Id = cp.Id,
                        RegNo = cp.RegNo,
                        FullName = cp.FullName,
                        Email = cp.Email,
                        PhoneNumber = cp.PhoneNumber,
                        Department = cp.Department,
                        Faculty = cp.Faculty,
                        Level = cp.Level,
                        Intake = cp.Intake,
                        StudySession = cp.StudySession,
                        IsActive = cp.IsActive
                    })
                    .ToListAsync();

            ErrorMessage =
                TempData["ErrorMessage"]?.ToString();

            SuccessMessage =
                TempData["SuccessMessage"]?.ToString();
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "An error occurred while loading class representatives.");

            ClassRepresentatives = [];

            TotalRepresentatives = 0;
            ActiveRepresentatives = 0;
            InactiveRepresentatives = 0;
            DepartmentCount = 0;
            TotalPages = 1;
            PageNumber = 1;

            ErrorMessage =
                "We could not load the class representative directory right now. Please refresh the page and try again.";
        }
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
    }

    private bool ValidateInput()
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
        else if (!new EmailAddressAttribute()
                     .IsValid(Input.Email))
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
                "Level must be 1, 2 or 3.");
        }

        if (!AllowedStudySessions.Contains(
                Input.StudySession))
        {
            ModelState.AddModelError(
                "Input.StudySession",
                "Please select a valid study session.");
        }

        return ModelState.IsValid;
    }

    private void ClearValidationForInput()
    {
        var keysToRemove = ModelState.Keys
            .Where(key =>
                key.StartsWith(
                    "Input.",
                    StringComparison.OrdinalIgnoreCase))
            .ToList();

        foreach (var key in keysToRemove)
        {
            ModelState.Remove(key);
        }
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }

    public class ClassRepresentativeInput
    {
        public int Id { get; set; }

        public string RegNo { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public string? Nationality { get; set; }

        public string Department { get; set; } = string.Empty;

        public string Intake { get; set; } = string.Empty;

        public string Level { get; set; } = string.Empty;

        public StudySession StudySession { get; set; } =
            StudySession.Day;
    }

    public class ClassRepresentativeRow
    {
        public int Id { get; set; }

        public string RegNo { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;

        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }

        public string Department { get; set; } = string.Empty;

        public string Faculty { get; set; } = string.Empty;

        public string Level { get; set; } = string.Empty;

        public string Intake { get; set; } = string.Empty;

        public StudySession StudySession { get; set; }

        public bool IsActive { get; set; }
    }


}

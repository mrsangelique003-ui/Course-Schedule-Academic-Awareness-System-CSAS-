using CourseScheduleSystem.Web.Data;
using CourseScheduleSystem.Web.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ── Database ───────────────────────────────────────────────────────────────────
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ── ASP.NET Core Identity ──────────────────────────────────────────────────────
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    options.Password.RequireDigit           = false;
    options.Password.RequireLowercase       = false;
    options.Password.RequireUppercase       = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength         = 6;
    options.SignIn.RequireConfirmedAccount  = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// ── Cookie settings (Identity configures this internally; override paths here) ─
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath         = "/Login";
    options.LogoutPath        = "/Logout";
    options.AccessDeniedPath  = "/AccessDenied";
    options.ExpireTimeSpan    = TimeSpan.FromHours(8);
    options.SlidingExpiration = true;
});

// ── Authorization policies ─────────────────────────────────────────────────────
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("StudentOnly",  p => p.RequireRole("Student"));
    options.AddPolicy("CPOnly",       p => p.RequireRole("CP"));
    options.AddPolicy("QualityOnly",  p => p.RequireRole("DirectorOfQuality"));
    options.AddPolicy("DeanHODOnly",  p => p.RequireRole("Dean", "HOD"));
    options.AddPolicy("AnyRole",      p => p.RequireRole("Student", "CP", "DirectorOfQuality", "Dean", "HOD"));
});

// ── Razor Pages ────────────────────────────────────────────────────────────────
builder.Services.AddRazorPages();

var app = builder.Build();

// ── Seed the database ──────────────────────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    await DbInitializer.SeedAsync(scope.ServiceProvider);
}

// ── Middleware pipeline ────────────────────────────────────────────────────────
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// Only redirect to HTTPS in production — avoid certificate warnings in dev
if (!app.Environment.IsDevelopment())
    app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

// ── Logout ─────────────────────────────────────────────────────────────────────
app.MapPost("/Logout", async (HttpContext ctx) =>
{
    var signInManager = ctx.RequestServices
        .GetRequiredService<Microsoft.AspNetCore.Identity.SignInManager<ApplicationUser>>();
    await signInManager.SignOutAsync();
    ctx.Response.Redirect("/Login");
}).RequireAuthorization();

app.Run();

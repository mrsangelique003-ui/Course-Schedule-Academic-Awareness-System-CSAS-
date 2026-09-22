using CourseScheduleSystem.Web.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using System.IO.Compression;

var builder = WebApplication.CreateBuilder(args);

// ---------------------------------------------------------------------------
// DATABASE
// ---------------------------------------------------------------------------
builder.Services.AddDbContextPool<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sql => sql.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)
    )
);

// ---------------------------------------------------------------------------
// PASSWORD HASHER
// ---------------------------------------------------------------------------
builder.Services.AddScoped<
    IPasswordHasher<CourseScheduleSystem.Web.Models.Student>,
    PasswordHasher<CourseScheduleSystem.Web.Models.Student>>();

// ---------------------------------------------------------------------------
// AUTHENTICATION (Cookie)
// ---------------------------------------------------------------------------
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";

        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;

        options.Cookie.Name = "CSAS.Auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.Cookie.SameSite = SameSiteMode.Lax;

        // Return 401/403 instead of redirect for API/AJAX requests
        options.Events.OnRedirectToLogin = context =>
        {
            if (context.Request.Path.StartsWithSegments("/api"))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Task.CompletedTask;
            }
            context.Response.Redirect(context.RedirectUri);
            return Task.CompletedTask;
        };

        options.Events.OnRedirectToAccessDenied = context =>
        {
            if (context.Request.Path.StartsWithSegments("/api"))
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                return Task.CompletedTask;
            }
            context.Response.Redirect(context.RedirectUri);
            return Task.CompletedTask;
        };
    });

// ---------------------------------------------------------------------------
// AUTHORIZATION
// ---------------------------------------------------------------------------
builder.Services.AddAuthorization(options =>
{
    // Named policies — keeps [Authorize(Policy = "...")] readable
    options.AddPolicy("AdminOnly", p => p.RequireRole("Admin"));
    options.AddPolicy("ClassRepOnly", p => p.RequireRole("Class Representative", "CP"));
    options.AddPolicy("HodOrAdmin", p => p.RequireRole("HOD", "Admin"));
});

// ---------------------------------------------------------------------------
// RESPONSE COMPRESSION
// ---------------------------------------------------------------------------
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();

    options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
    [
        "text/html",
        "text/css",
        "application/javascript",
        "application/json",
        "image/svg+xml",
        "font/woff2"
    ]);
});

builder.Services.Configure<BrotliCompressionProviderOptions>(
    options => options.Level = CompressionLevel.Fastest);

builder.Services.Configure<GzipCompressionProviderOptions>(
    options => options.Level = CompressionLevel.Fastest);

// ---------------------------------------------------------------------------
// CACHING
// ---------------------------------------------------------------------------
builder.Services.AddResponseCaching();
builder.Services.AddMemoryCache();

// ---------------------------------------------------------------------------
// RAZOR PAGES
// ---------------------------------------------------------------------------
builder.Services.AddRazorPages(options =>
{
    // Require authentication for every page under /CP and /Admin by default
    options.Conventions.AuthorizeFolder("/CP");
    options.Conventions.AuthorizeFolder("/Admin", "AdminOnly");

    // Pages that anonymous users must be able to reach
    options.Conventions.AllowAnonymousToFolder("/Account");
});

var app = builder.Build();

// ---------------------------------------------------------------------------
// SEED DATABASE
// ---------------------------------------------------------------------------
using (var scope = app.Services.CreateScope())
{
    await DbInitializer.SeedAsync(scope.ServiceProvider);
}

// ---------------------------------------------------------------------------
// MIDDLEWARE PIPELINE
// ---------------------------------------------------------------------------
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseResponseCompression();

app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = context =>
    {
        var headers = context.Context.Response.Headers;

        headers["Cache-Control"] = "public, max-age=31536000, immutable";
        headers["Vary"] = "Accept-Encoding";
    }
});

app.UseResponseCaching();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
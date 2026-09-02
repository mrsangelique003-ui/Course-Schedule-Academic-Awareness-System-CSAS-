using CourseScheduleSystem.Web.Data;
using CourseScheduleSystem.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using System.IO.Compression;

var builder = WebApplication.CreateBuilder(args);

// ── Database (with connection pooling + query splitting) ──────────────────────
builder.Services.AddDbContextPool<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sql => sql.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)
    )
);

// ── ASP.NET Core Identity ─────────────────────────────────────────────────────
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

// ── Cookie settings ───────────────────────────────────────────────────────────
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath         = "/Login";
    options.LogoutPath        = "/Logout";
    options.AccessDeniedPath  = "/AccessDenied";
    options.ExpireTimeSpan    = TimeSpan.FromHours(8);
    options.SlidingExpiration = true;
});

// ── Authorization policies ────────────────────────────────────────────────────
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("StudentOnly",  p => p.RequireRole("Student"));
    options.AddPolicy("CPOnly",       p => p.RequireRole("CP"));
    options.AddPolicy("QualityOnly",  p => p.RequireRole("DirectorOfQuality"));
    options.AddPolicy("DeanHODOnly",  p => p.RequireRole("Dean", "HOD"));
    options.AddPolicy("AnyRole",      p => p.RequireRole("Student", "CP", "DirectorOfQuality", "Dean", "HOD"));
});

// ── Response Compression (Brotli + Gzip) ─────────────────────────────────────
builder.Services.AddResponseCompression(opts =>
{
    opts.EnableForHttps = true;
    opts.Providers.Add<BrotliCompressionProvider>();
    opts.Providers.Add<GzipCompressionProvider>();
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[]
    {
        "text/html", "text/css", "application/javascript",
        "application/json", "image/svg+xml", "font/woff2"
    });
});
builder.Services.Configure<BrotliCompressionProviderOptions>(o => o.Level = CompressionLevel.Fastest);
builder.Services.Configure<GzipCompressionProviderOptions>(o => o.Level   = CompressionLevel.Fastest);

// ── Response Caching ──────────────────────────────────────────────────────────
builder.Services.AddResponseCaching();

// ── Memory Cache ──────────────────────────────────────────────────────────────
builder.Services.AddMemoryCache();

// ── Razor Pages ───────────────────────────────────────────────────────────────
builder.Services.AddRazorPages();

var app = builder.Build();

// ── Seed database ─────────────────────────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    await DbInitializer.SeedAsync(scope.ServiceProvider);
}

// ── Middleware pipeline ───────────────────────────────────────────────────────
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// Response compression FIRST — before static files
app.UseResponseCompression();

// Only redirect HTTPS in production
if (!app.Environment.IsDevelopment())
    app.UseHttpsRedirection();

// Static files with aggressive caching headers
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        // Cache static assets for 1 year (CSS, JS, images)
        var headers = ctx.Context.Response.Headers;
        headers["Cache-Control"] = "public, max-age=31536000, immutable";
        headers["Vary"]           = "Accept-Encoding";
    }
});

app.UseResponseCaching();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();

// ── Logout ────────────────────────────────────────────────────────────────────
app.MapPost("/Logout", async (HttpContext ctx) =>
{
    var signInManager = ctx.RequestServices
        .GetRequiredService<SignInManager<ApplicationUser>>();
    await signInManager.SignOutAsync();
    ctx.Response.Redirect("/Login");
}).RequireAuthorization();

app.Run();

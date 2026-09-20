using CourseScheduleSystem.Web.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using System.IO.Compression;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContextPool<ApplicationDbContext>(options =>
options.UseSqlServer(
builder.Configuration.GetConnectionString("DefaultConnection"),
sql => sql.UseQuerySplittingBehavior(
QuerySplittingBehavior.SplitQuery)
)
);

builder.Services.AddScoped<
IPasswordHasher<CourseScheduleSystem.Web.Models.Student>,
PasswordHasher<CourseScheduleSystem.Web.Models.Student>>();

builder.Services.AddAuthentication(
CookieAuthenticationDefaults.AuthenticationScheme)
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
});


builder.Services.AddAuthorization();

builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();


options.MimeTypes =
    ResponseCompressionDefaults.MimeTypes.Concat(
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

builder.Services.AddResponseCaching();

builder.Services.AddMemoryCache();

builder.Services.AddRazorPages();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    await DbInitializer.SeedAsync(scope.ServiceProvider);
}

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


    headers["Cache-Control"] =
        "public, max-age=31536000, immutable";

        headers["Vary"] = "Accept-Encoding";
    }


});

app.UseResponseCaching();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapRazorPages();

app.Run();

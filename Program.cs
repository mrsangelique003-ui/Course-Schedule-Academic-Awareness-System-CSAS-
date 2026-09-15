using CourseScheduleSystem.Web.Data;
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

app.MapRazorPages();

app.Run();
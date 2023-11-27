using System.Diagnostics;
using Lattice;
using Lattice.Web.Data;
using QuestPDF.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

QuestPDF.Settings.License = LicenseType.Community;

builder.Services.AddLattice();
builder.Services.AddSingleton<PreviewStorage>();
builder.Services.AddControllers();

var app = builder.Build();

// If we're running in Docker or similar, use the PORT environment variable.
var port = Environment.GetEnvironmentVariable("PORT");
if (port != null)
{
    builder.WebHost.UseUrls($"http://*:{port}");
}

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseExceptionHandler("/Error");
}

app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});
app.UseDefaultFiles();
app.UseStaticFiles();

// Run the Vite in serve mode when running in development mode.
if (app.Environment.IsDevelopment())
{
    Process.Start(new ProcessStartInfo
    {
        FileName = OperatingSystem.IsLinux() || OperatingSystem.IsMacOS() ? "bash" : "cmd",
        RedirectStandardInput = true,
        WorkingDirectory = Path.Combine(Environment.CurrentDirectory, "client")
    })!.StandardInput.WriteLine("npm run serve");
}

app.Run();
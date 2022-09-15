using System.Diagnostics;
using Lattice;
using System.Reflection;
using Lattice.Builders;
using Lattice.Web;
using Lattice.Web.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLattice();
builder.Services.AddSingleton<TempStorage>();
builder.Services.AddControllers();


var app = builder.Build();

// If we're running in Docker or similar, use the PORT environment variable.
var port = Environment.GetEnvironmentVariable("PORT");
if (port != null)
{
    builder.WebHost.UseUrls($"http://*:{port}");
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

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
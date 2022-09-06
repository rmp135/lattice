using System.Diagnostics;
using Lattice;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.RegisterAll(new[]
{
    Assembly.GetAssembly(typeof(Main))!, 
    Assembly.GetAssembly(typeof(Program))!
});

// Run the serve command when running in development mode.
if (builder.Environment.IsDevelopment())
{
    Process.Start(new ProcessStartInfo
    {
        FileName = OperatingSystem.IsLinux() || OperatingSystem.IsMacOS() ? "bash" : "cmd",
        RedirectStandardInput = true,
        WorkingDirectory = Path.Combine(Environment.CurrentDirectory, "client")
    })!.StandardInput.WriteLine("npm run serve");
}

// Add services to the container.
builder.Services.AddControllers();
    
var app = builder.Build();

if (app.Environment.IsProduction())
{
    builder.WebHost.UseUrls($"http://*:{Environment.GetEnvironmentVariable("PORT")}");
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
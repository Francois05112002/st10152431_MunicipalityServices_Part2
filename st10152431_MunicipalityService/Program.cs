using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using st10152431_MunicipalityService.Services;
using st10152431_MunicipalityService.Models;

var builder = WebApplication.CreateBuilder(args);

// ===== ADD SERVICES TO THE CONTAINER =====

// Add Razor Pages support
builder.Services.AddRazorPages();

// Add session support for user authentication
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Session timeout after 30 minutes of inactivity
    options.Cookie.HttpOnly = true; // Security: Cookie not accessible via JavaScript
    options.Cookie.IsEssential = true; // Required for GDPR compliance
    options.Cookie.Name = ".MunicipalityApp.Session"; // Custom session cookie name
});

// Register all services as SINGLETONS
// Singleton = Single instance shared across all requests (maintains data in memory)
// CRITICAL: Data structures (Dictionary, List, HashSet) persist across all user sessions
builder.Services.AddSingleton<UserService>();
builder.Services.AddSingleton<ReportService>();
builder.Services.AddSingleton<EventService>();

// Optional: Add HTTP context accessor if needed for accessing session in services
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// ===== CONFIGURE THE HTTP REQUEST PIPELINE =====

// Configure error handling
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios.
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage(); // Show detailed errors in development
}

// Redirect HTTP to HTTPS
app.UseHttpsRedirection();

// Enable static files (CSS, JS, images, etc.)
app.UseStaticFiles();

// Enable routing
app.UseRouting();

// CRITICAL: Enable session BEFORE authorization
// Order matters: Session must come before Authorization
app.UseSession();

// Enable authorization
app.UseAuthorization();

// Map Razor Pages
app.MapRazorPages();

// Run the application
app.Run();
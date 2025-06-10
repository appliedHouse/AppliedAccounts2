using AppliedAccounts.Authentication;
using AppliedAccounts.Middleware;
using AppliedAccounts.Services;
using AppliedDB;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.JSInterop;


System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddAuthenticationCore();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
//builder.Services.AddSingleton<AppUserModel>();              // Create a new instance of AppUserModel
builder.Services.AddSingleton<UserProfile>();               // Create a User Model Class.
builder.Services.AddScoped<ProtectedSessionStorage>();
builder.Services.AddScoped<AuthenticationStateProvider, UserAuthenticationStateProvider>();
builder.Services.AddScoped<ToastService>();
builder.Services.AddScoped<PrintService>();
builder.Services.AddHttpClient();

builder.Services.AddScoped<GlobalService>(sp =>
{
    // Access configuration and navigation manager
    var config = sp.GetRequiredService<IConfiguration>();
    var navManager = sp.GetRequiredService<NavigationManager>();
    var JSRuntime = sp.GetRequiredService<IJSRuntime>();
    var StateProvider = sp.GetRequiredService<AuthenticationStateProvider>();

    // Initialize GlobalService with dependencies
    var globalService = new GlobalService(config, navManager, JSRuntime, StateProvider);

    // Set the Language.ID value here
    globalService.Language.ID = 1; // Example: Setting ID to 1

    return globalService;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseMiddleware<DatabaseValidation>(); // Custom middleware to validate the database
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();

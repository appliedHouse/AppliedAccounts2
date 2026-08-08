using AppliedAccounts.Authentication;
using AppliedAccounts.Hubs;
using AppliedAccounts.Middleware;
using AppliedAccounts.Models;
using AppliedAccounts.Services;
using AppliedAccounts.Services.Menus;
using AppliedDB;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using ToastNotificationLibrary.Extensions;
using ToastNotificationLibrary.Models;
using ToastNotificationLibrary.Services;
using SQLitePCL;

System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

var builder = WebApplication.CreateBuilder(args);

Batteries.Init(); // Start SQLite Engine.

// Add services to the container.

// 1. Add SignalR with JSON protocol support
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true;
}).AddJsonProtocol(); // This adds JSON protocol

// 2. Add HTTP Client
builder.Services.AddHttpClient();

// 3. Add Authentication
builder.Services.AddAuthenticationCore();
builder.Services.AddScoped<ProtectedSessionStorage>();
builder.Services.AddScoped<UserAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider, UserAuthenticationStateProvider>();

// 4. Add Razor Pages and Blazor
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// 5. Add other services
builder.Services.AddSingleton<UserProfile>();
builder.Services.AddScoped<PrintService>();
builder.Services.AddScoped<MessagesService>();
builder.Services.AddScoped<GlobalService>();
builder.Services.AddScoped<IMenuDatabaseInitializer, MenuDatabaseInitializer>();
builder.Services.AddScoped<IMenuService, MenuService>();
builder.Services.AddScoped<ProgressService>();
builder.Services.AddScoped<ToastService>();

// 6. Add FTP Backup Service
builder.Services.AddSingleton<FTPBackupService>();
builder.Services.Configure<BackupModel>(builder.Configuration.GetSection("FTPBackup"));
builder.Services.AddHostedService<FTPBackupService>();

// 7. Add Toast Notifications
builder.Services.AddToastNotification(options =>
{
    options.DefaultDuration = 8000;
    options.DefaultPosition = ToastPosition.BottomEnd;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseMiddleware<DatabaseValidation>();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseUserDatabaseValidation();

app.MapBlazorHub();
app.MapHub<LogHub>("/loghub");
app.MapFallbackToPage("/_Host");

app.Run();
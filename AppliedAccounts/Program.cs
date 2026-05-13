using AppliedAccounts.Authentication;
using AppliedAccounts.Middleware;
using AppliedAccounts.Services;
using AppliedDB;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using SQLitePCL;
using ToastNotificationLibrary.Extensions;
using ToastNotificationLibrary.Models;


System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

var builder = WebApplication.CreateBuilder(args);

Batteries.Init();                       // Start SQLite Engine.

// Add services to the container.

builder.Services.AddHttpClient();
builder.Services.AddAuthenticationCore();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<UserProfile>();               // Create a User Model Class.
builder.Services.AddScoped<ProtectedSessionStorage>();
builder.Services.AddScoped<UserAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider, UserAuthenticationStateProvider>();
builder.Services.AddScoped<PrintService>();
builder.Services.AddScoped<MessagesService>();
builder.Services.AddScoped<GlobalService>();
builder.Services.AddToastNotification(options =>
{
    options.DefaultDuration = 3000;
    options.DefaultPosition = ToastPosition.BottomEnd;
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseMiddleware<DatabaseValidation>(); 

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseUserDatabaseValidation();         

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");


// Backup SQLite Database to local computer as Backup..
app.MapGet("/api/backup/{fileName}", async (
    string fileName,
    ISQLiteDBBackupService backupService,
    IWebHostEnvironment env) =>
{
    var dbPath = Path.Combine(
        env.WebRootPath,
        "SQLiteDB",
        fileName
    );

    try
    {
        var (data, name) = await backupService.CreateBackupAsync(dbPath);
        return Results.File(data, "application/octet-stream", name);
    }
    catch (FileNotFoundException)
    {
        return Results.NotFound("Database not found");
    }
});


app.Run();

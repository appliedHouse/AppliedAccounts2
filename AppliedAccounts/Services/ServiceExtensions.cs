using AppliedDB;
using AppliedAccounts.Authentication;
using AppliedAccounts.Services.Menus;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using ToastNotificationLibrary.Services;
using ToastNotificationLibrary.Extensions;

using ToastNotificationLibrary.Models;

namespace AppliedAccounts.Services
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // HTTP Client
            services.AddHttpClient();

            // Authentication
            services.AddAuthenticationCore();

            // Razor Pages and Blazor
            services.AddRazorPages();
            services.AddServerSideBlazor();

            // Singleton Services
            services.AddSingleton<UserProfile>();

            // Scoped Services
            services.AddScoped<ProtectedSessionStorage>();
            
            services.AddScoped<PrintService>();
            services.AddScoped<MessagesService>();
            services.AddScoped<GlobalService>();
            services.AddScoped<IMenuDatabaseInitializer, MenuDatabaseInitializer>();
            services.AddScoped<IMenuService, MenuService>();
            services.AddScoped<ProgressService>();
            services.AddScoped<ToastService>();

            // Toast Notifications
            services.AddToastNotification(options =>
            {
                options.DefaultDuration = 8000;
                options.DefaultPosition = ToastPosition.BottomEnd;
            });

            services.AddScoped<UserAuthenticationStateProvider>();
            services.AddScoped<AuthenticationStateProvider>(sp =>
                sp.GetRequiredService<UserAuthenticationStateProvider>());


            return services;
        }
    }
}
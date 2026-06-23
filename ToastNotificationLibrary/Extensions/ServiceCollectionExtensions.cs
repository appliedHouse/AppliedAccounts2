// Extensions/ServiceCollectionExtensions.cs
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToastNotificationLibrary.Services;
using ToastNotificationLibrary.Models; // Add this

namespace ToastNotificationLibrary.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddToastNotification(this IServiceCollection services)
    {
        services.TryAddScoped<IToastService, ToastService>();
        return services;
    }

    public static IServiceCollection AddToastNotification(this IServiceCollection services, Action<ToastOptions> configure)
    {
        var options = new ToastOptions();
        configure(options);
        services.AddSingleton(options);
        services.TryAddScoped<IToastService, ToastService>();
        return services;
    }
}

public class ToastOptions
{
    public int DefaultDuration { get; set; } = 8000;
    public ToastPosition DefaultPosition { get; set; } = ToastPosition.BottomEnd;
}
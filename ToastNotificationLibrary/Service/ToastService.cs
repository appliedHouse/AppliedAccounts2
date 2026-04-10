// Services/ToastService.cs
using ToastNotificationLibrary.Models;
using ToastNotificationLibrary.Extensions;

namespace ToastNotificationLibrary.Services;

public class ToastService : IToastService, IDisposable
{
    public event Action<ToastMessage>? OnShow;
    public event Action? OnHide;

    private CancellationTokenSource? _cancellationTokenSource;
    private ToastMessage? _currentToast;
    private readonly ToastOptions? _options = new ToastOptions { DefaultDuration = 10000 };

    public ToastService(IServiceProvider serviceProvider)
    {
        _options = serviceProvider.GetService(typeof(ToastOptions)) as ToastOptions;
    }

    public void ShowSuccess(string message, int duration = 3000)
        => Show(new ToastMessage(message, ToastLevel.Success, GetEffectiveDuration(duration)));

    public void ShowError(string message, int duration = 3000)
        => Show(new ToastMessage(message, ToastLevel.Error, GetEffectiveDuration(duration)));

    public void ShowWarning(string message, int duration = 3000)
        => Show(new ToastMessage(message, ToastLevel.Warning, GetEffectiveDuration(duration)));

    public void ShowInfo(string message, int duration = 3000)
        => Show(new ToastMessage(message, ToastLevel.Info, GetEffectiveDuration(duration)));

    private int GetEffectiveDuration(int duration)
    {
        // If options provided and caller used the library default (3000), prefer configured DefaultDuration
        if (_options != null && duration == 3000)
        {
            return _options.DefaultDuration;
        }

        return duration;
    }

    public void Show(ToastMessage message)
    {
        Clear();
        _currentToast = message;
        OnShow?.Invoke(message);

        if (message.Duration > 0)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            Task.Delay(message.Duration, _cancellationTokenSource.Token)
                .ContinueWith(task =>
                {
                    if (!task.IsCanceled)
                    {
                        Clear();
                    }
                }, TaskScheduler.Default);
        }
    }

    public void Clear()
    {
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource?.Dispose();
        _cancellationTokenSource = null;

        if (_currentToast != null)
        {
            _currentToast = null;
            OnHide?.Invoke();
        }
    }

    public void Dispose()
    {
        Clear();
    }
}
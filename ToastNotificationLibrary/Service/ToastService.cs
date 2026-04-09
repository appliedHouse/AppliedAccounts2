// Services/ToastService.cs
using System.Threading;
using System.Threading.Tasks;
using ToastNotificationLibrary.Models;

namespace ToastNotificationLibrary.Services;

public class ToastService : IToastService, IDisposable
{
    public event Action<ToastMessage>? OnShow;
    public event Action? OnHide;

    private CancellationTokenSource? _cancellationTokenSource;
    private ToastMessage? _currentToast;

    public void ShowSuccess(string message, int duration = 3000)
        => Show(new ToastMessage(message, ToastLevel.Success, duration));

    public void ShowError(string message, int duration = 3000)
        => Show(new ToastMessage(message, ToastLevel.Error, duration));

    public void ShowWarning(string message, int duration = 3000)
        => Show(new ToastMessage(message, ToastLevel.Warning, duration));

    public void ShowInfo(string message, int duration = 3000)
        => Show(new ToastMessage(message, ToastLevel.Info, duration));

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
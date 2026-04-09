// Services/IToastService.cs
using ToastNotificationLibrary.Models;

namespace ToastNotificationLibrary.Services;

public interface IToastService
{
    event Action<ToastMessage>? OnShow;
    event Action? OnHide;

    void ShowSuccess(string message, int duration = 3000);
    void ShowError(string message, int duration = 3000);
    void ShowWarning(string message, int duration = 3000);
    void ShowInfo(string message, int duration = 3000);
    void Show(ToastMessage message);
    void Clear();
}
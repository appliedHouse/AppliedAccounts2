// Services/IToastService.cs
using ToastNotificationLibrary.Models;

namespace ToastNotificationLibrary.Services;

public interface IToastService
{
    event Action<ToastMessage>? OnShow;
    event Action? OnHide;
    
    void ShowSuccess(string message);
    void ShowError(string message);
    void ShowWarning(string message);
    void ShowInfo(string message);
    void Show(ToastMessage message);
    void Clear();
}
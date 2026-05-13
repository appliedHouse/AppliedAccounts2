// Models/ToastMessage.cs
namespace ToastNotificationLibrary.Models;

public class ToastMessage
{
    public string Id { get; } = Guid.NewGuid().ToString();
    public string Message { get; set; }
    public ToastLevel Level { get; set; }
    public int Duration { get; set; }
    public DateTime CreatedAt { get; } = DateTime.UtcNow;
    public bool IsVisible { get; set; } = true;
    public ToastPosition Position { get; set; } = ToastPosition.BottomEnd;

    public ToastMessage(string message, ToastLevel level, int duration = 3000)
    {
        Message = message;
        Level = level;
        Duration = duration;
    }

    public ToastMessage WithPosition(ToastPosition position)
    {
        Position = position;
        return this;
    }

    public string GetCssClass() => Level switch
    {
        ToastLevel.Success => "toast-success",
        ToastLevel.Error => "toast-error",
        ToastLevel.Warning => "toast-warning",
        ToastLevel.Info => "toast-info",
        _ => "toast-secondary"
    };

    public string GetIcon() => Level switch
    {
        ToastLevel.Success => "✓",
        ToastLevel.Error => "✗",
        ToastLevel.Warning => "⚠️",
        ToastLevel.Info => "ℹ️",
        _ => "📢"
    };

    public string GetPositionClass() => Position switch
    {
        ToastPosition.TopEnd => "top-0 end-0",
        ToastPosition.TopStart => "top-0 start-0",
        ToastPosition.BottomEnd => "bottom-0 end-0",
        ToastPosition.BottomStart => "bottom-0 start-0",
        ToastPosition.TopCenter => "top-0 start-50 translate-middle-x",
        ToastPosition.BottomCenter => "bottom-0 start-50 translate-middle-x",
        _ => "bottom-0 end-0"
    };
}

public enum ToastLevel
{
    Info,
    Success,
    Warning,
    Error
}

public enum ToastPosition
{
    TopEnd,
    TopStart,
    BottomEnd,
    BottomStart,
    TopCenter,
    BottomCenter
}
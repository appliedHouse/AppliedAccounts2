namespace AppliedAccounts.Services
{

    public class ToastService
    {
        public event Action<ToastClass>? OnShowToast;
        public event Action? OnHideToast;
        public ToastClass Toasters = new();

        public void ShowToast(ToastClass toast)
        {
            toast.ShowToast = true;
            OnShowToast?.Invoke(toast);

            Task.Delay(toast.delayTime).ContinueWith(_ =>
            {
                toast.ShowToast = false;
                OnHideToast?.Invoke(); // Notify UI to hide it
            });
        }

        public async Task ShowToastAsync(ToastClass toast)
        {
            await Task.Run(() => ShowToast(toast));
        }

        public void ShowToast(ToastClass toast, string? _Message)
        {
            toast.ShowToast = true;
            toast.Message = _Message ?? toast.Message;
            OnShowToast?.Invoke(toast);

            Task.Delay(toast.delayTime).ContinueWith(_ =>
            {
                toast.ShowToast = false;
                OnHideToast?.Invoke(); // Notify UI to hide it
            });
        }
    }


}
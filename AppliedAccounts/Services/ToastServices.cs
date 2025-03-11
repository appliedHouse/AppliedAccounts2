﻿namespace AppliedAccounts.Services
{

    public class ToastService
    {
        public event Action<string, string>? OnShowToast;

        public void ShowToast(ToastClass _Toast)
        {
            OnShowToast?.Invoke(_Toast.Message, _Toast.CssClass);
        }
    }

    public class ToastClass
    {
        public string Message { get; set; }
        public string CssClass { get; set; }
        public bool ShowToast { get; set; }
        public ToastPosition Position { get; set; }
        public ToastLevel Level { get; set; }

        public ToastClass()
        {
            Position = ToastPosition.Top;

            //CssClass = GetLevel();
            

        }

        public string GetLevel()
        {
            return Level switch
            {
                ToastLevel.Success => "bg-success text-white",
                ToastLevel.Error => "bg-danger text-white",
                ToastLevel.Warning => "bg-warning text-dark",
                ToastLevel.Info => "bg-info text-dark",
                _ => "bg-secondary text-white"
            };

        }

        public string GetPosition()
        {
            return Position switch
            {
                ToastPosition.Bottom => "bottom-0 end-0 p-3",
                ToastPosition.Top => "top-0 end-0 p-3",
                _ => "bottom-0 end-0 p-3"
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
            Top,
            Bottom,
        }
    }
}
﻿namespace AppliedAccounts.Services
{
    public class ToastClass
    {
        public string Message { get; set; } = string.Empty;
        public string CssClass { get; set; }
        public bool ShowToast { get; set; }
        public int delayTime { get; set; }
        public ToastPosition Position { get; set; }
        public ToastLevel Level { get; set; }

        public ToastClass()
        {
            delayTime = 3000;
            Position = ToastPosition.Top;
            Level = ToastLevel.Info;
        }

        public ToastClass(string _Message)
        {
            Message = _Message;
            delayTime = 3000;
            Position = ToastPosition.Top;
            Level = ToastLevel.Info;
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

        public static ToastClass SaveToast { get; set; } = new()
        {
            Message = "Save ",
            Level = ToastLevel.Success,
            Position = ToastPosition.Top,
            ShowToast = true

        };
        public static ToastClass NotSaveToast { get; set; } = new()
        {
            Message = "Not Save",
            Level = ToastLevel.Warning,
            Position = ToastPosition.Top,
            ShowToast = true

        };
        public static ToastClass DeleteToast { get; set; } = new()
        {
            Message = "Delete",
            Level = ToastLevel.Warning,
            Position = ToastPosition.Bottom,
            ShowToast = true,
        };

        public static ToastClass ErrorToast { get; set; } = new()
        {
            Message = "Error",
            Level = ToastLevel.Error,
            Position = ToastPosition.Bottom,
            ShowToast = true,
        };

        public static ToastClass DownLoadToast { get; set; } = new()
        {
            Message = "File has been download",
            Level = ToastLevel.Info,
            Position = ToastPosition.Bottom,
            ShowToast = true,
            delayTime = 10000                // 10 second
        };
    }
}


namespace AppliedGlobals
{
    public class ProgressBarModel
    {
        public class ProgressReport
        {
            public int Current { get; set; }
            public int Total { get; set; }
            public string Message { get; set; } = string.Empty;
            public bool IsComplete { get; set; }
            public bool IsError { get; set; }
            public string ErrorMessage { get; set; } = string.Empty;

            public int Percentage => Total > 0 ? (int)((double)Current / Total * 100) : 0;
        }
    }
}

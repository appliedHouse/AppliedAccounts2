using AppliedGlobals;

namespace AppliedAccounts.Services
{
    public class ProgressService
    {
        public event Action<ProgressBarModel.ProgressReport> OnProgressChanged;

        public void ReportProgress(ProgressBarModel.ProgressReport report)
        {
            OnProgressChanged?.Invoke(report);
        }
    }

    
}

using AppliedAccounts.Data;
using AppliedAccounts.Models;
using AppliedDB;
using Microsoft.JSInterop;


namespace AppliedAccounts.Pages.Accounts
{
    public partial class ReceiptList
    {
        private object _FileName;

        public AppUserModel UserProfile { get; set; }
        public ReceiptListModel MyModel { get; set; }

        public bool IsWaiting { get; set; }
        public string SpinnerMessage { get; set; }
        public ReceiptList()
        {
            IsWaiting = false;
        }

        public async Task Print(ReportActionClass reportAction)
        {
            // this redirect to model print.... do not delete this method
            try
            {
                SpinnerMessage = "Report is being generated.  Wait for some while.";
                IsWaiting = true;
                await InvokeAsync(StateHasChanged);
                await MyModel.Print(reportAction);

            }
            catch (Exception error)
            {
                MyModel.MsgClass.Danger(error.Message);
            }
            finally
            {
                IsWaiting = false;
                await InvokeAsync(StateHasChanged);
            }
        }

        

    }
}

using AppliedAccounts.Data;
using AppliedAccounts.Models;
using AppliedDB;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace AppliedAccounts.Pages.Accounts
{
    public partial class BooksList
    {

        public BookListModel MyModel { get; set; } = new();
        public NavigationManager NavManager { get; set; }

        public BooksList() { }

        #region Back Page
        public void Back() { AppGlobal.NavManager.NavigateTo("/Menu/Accounts"); }
        #endregion

        #region New Voucher
        public void New()
        {
            if (MyModel.BookID > 0)
            {
                NavManager.NavigateTo($"/Accounts/Books/{MyModel.VoucherID}/{MyModel.BookID}");
            }
        }
        #endregion

        #region Refresh Page
        public async void Refresh()
        {
            MyModel.SetKeys();              // Save the current page setting in Registry 
            MyModel.Pages = new();          // Reset the page model
            await InvokeAsync(StateHasChanged);
        }
        #endregion

        #region Print
        public async Task Print(ReportActionClass reportAction)
        {
            MyModel.IsWaiting = true;
            await InvokeAsync(StateHasChanged);
            await Task.Delay(100);                  // Delay for show the message and 

            try
            {
                MyModel.VoucherID = reportAction.VoucherID;
                await Task.Run(() => { MyModel.Print(reportAction.PrintType); });
            }
            catch (Exception error)
            {
                MyModel.MsgService.Error(error);
            }

            MyModel.IsWaiting = false;
            await InvokeAsync(StateHasChanged);
            await Task.Delay(100);                  // Delay for show the message and 
        }
        #endregion

        #region DropDown Changed
        private void BookNatureChanged(long _NatureID)
        {
            MyModel.BookNatureID = _NatureID;
            MyModel.BookID = MyModel.Source.GetBookAccounts(MyModel.BookNatureID).First()?.ID ?? 0;
        }

        private void BookListChanged(long _BookID)
        {
            MyModel.BookID = _BookID; //result;
        }
        #endregion


        #region Delete record and Voucher
        private void DeleteVoucher(long VoucherID)
        {
            if (MyModel.DeleteAll(VoucherID))
            {
                Toaster.ShowInfo($"Voucher Deleted Successfully {VoucherID}");
            }
            else
            {
                Toaster.ShowError($"Voucher Deletion Failed {VoucherID}");
            }
        }
        #endregion

    }

    #region book view Model

    public class BookView
    {
        public long ID { get; set; }
        public string Vou_No { get; set; }
        public int Sr_No { get; set; }
        public DateTime Vou_Date { get; set; }
        public string Description { get; set; }
        public decimal Recevied { get; set; }
        public decimal Paid { get; set; }
        public decimal Balance { get; set; }
        public string TReceived { get; set; }
        public string TPaid { get; set; }
        public string TBalance { get; set; }
        public string Status { get; set; }

    }
    #endregion
}

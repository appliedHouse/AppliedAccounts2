using AppliedAccounts.Data;
using AppliedAccounts.Models;
using AppliedAccounts.Pages.Menu;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Runtime.CompilerServices;
using static AppliedGlobals.AppErums;




namespace AppliedAccounts.Pages.Accounts.Post
{
    public partial class Posting
    {
        public PostingViewModel MyViewModel { get; set; }
        public PostingModel MyModel { get; set; } = new();
        public string DBFile => AppGlobal.DBFile;

        public long PostingVoucherID { get; set; } = 0;
        public string PostingVoucher { get; set; } =string.Empty;


        protected override async Task OnInitializedAsync()
        {
            MyModel.Source = new(AppGlobal.AppPaths);
           
            MyModel.Source.SetKey("IsPosting", false, KeyTypes.Boolean, "Is posting is in progress..");
            MyViewModel = new();
            MyViewModel.Dt_From = AppRegistry.GetDate(DBFile, "Post_dt_From");
            MyViewModel.Dt_To = AppRegistry.GetDate(DBFile, "Post_dt_To");
            MyViewModel.PostingType = AppRegistry.GetNumber(DBFile, "Post_Type");
            
            
            await MyModel.LoadData(MyViewModel.PostingType);
        }

        #region Change Event
        private async void OnPostingTypeChanged(int value)
        {
            MyViewModel.PostingType = value;

            await MyModel.LoadData(MyViewModel.PostingType);
        }

        private void OnStatusChanged(int value)
        {

            MyViewModel.PostingStatus = value;
        }

        #endregion

        public void Refresh()
        {
            AppRegistry.SetKey(DBFile, "Post_Type", MyViewModel.PostingType, KeyTypes.Number);
            AppRegistry.SetKey(DBFile, "Post_dt_From", MyViewModel.Dt_From, KeyTypes.Date);
            AppRegistry.SetKey(DBFile, "Post_dt_To", MyViewModel.Dt_To, KeyTypes.Date);
            AppRegistry.SetKey(DBFile, "PostCash", false, KeyTypes.Boolean);    // Reset Post Cash Voucher Status
            AppRegistry.SetKey(DBFile, "PostBank", false, KeyTypes.Boolean);    // Reset Post Bank Voucher Status
            AppRegistry.SetKey(DBFile, "PostReceipt", false, KeyTypes.Boolean);

            //return RedirectToPage();
        }
        
        public void GetFilter()
        {
            string filter = string.Empty;
            if (MyViewModel.Dt_From != DateTime.MinValue && MyViewModel.Dt_To != DateTime.MinValue)
            {
                filter = $" Vou_Date >= '{MyViewModel.Dt_From:yyyy-MM-dd}' AND Vou_Date <= '{MyViewModel.Dt_To:yyyy-MM-dd}' ";
            }
            MyModel.Filter = filter;

        }


        public async Task PostVoucher(long id)
        {
            MyModel.IsPosting = true;
            StateHasChanged();

            await AppGlobal.JS.InvokeVoidAsync("showModal", "SaveVoucher");

            await MyModel.DoVoucherPosting(id, MyViewModel.PostingType);

            await AppGlobal.JS.InvokeVoidAsync("hideModal", "SaveVoucher");
            MyModel.IsPosting = false;
            StateHasChanged();
        }

       



        public class PostingViewModel
        {
            public int PostingType { get; set; }
            public int PostingStatus { get; set; }
            public DateTime Dt_From { get; set; }
            public DateTime Dt_To { get; set; }
        }

       
        

    }
}

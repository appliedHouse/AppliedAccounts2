using AppliedAccounts.Data;
using AppliedAccounts.Models.UnPost;
using Microsoft.JSInterop;
using static AppliedGlobals.AppErums;

namespace AppliedAccounts.Pages.Accounts.Post
{
    public partial class UnPost
    {
        public UnPostViewModel MyViewModel { get; set; } = new();
        public UnPostModel MyModel { get; set; } = new();
        public string DBFile => AppGlobal.DBFile;
        public long UnPostVoucherID { get; set; } = 0;
        public string UnPostVoucher { get; set; } = string.Empty;
        protected override async Task OnInitializedAsync()
        {
            MyModel.Source = new(AppGlobal.AppPaths);

            MyModel.Source.SetKey("IsPosting", false, KeyTypes.Boolean, "Is posting is in progress..");
            MyViewModel = new();
            MyViewModel.Dt_From = AppRegistry.GetDate(DBFile, "UnPost_dt_From");
            MyViewModel.Dt_To = AppRegistry.GetDate(DBFile, "UnPost_dt_To");
            MyViewModel.PostingType = AppRegistry.GetNumber(DBFile, "UnPost_Type");

            await MyModel.LoadData(MyViewModel.PostingType);
        }

        public void Refresh()
        {
            MyModel.Source.SetKey("UnPost_Type", MyViewModel.PostingType, KeyTypes.Number, "");
            MyModel.Source.SetKey("UnPost_dt_From", MyViewModel.Dt_From, KeyTypes.Date, "");
            MyModel.Source.SetKey("UnPost_dt_To", MyViewModel.Dt_To, KeyTypes.Date, "");
            MyModel.Source.SetKey("UnPostCash", false, KeyTypes.Boolean, "");    // Reset Post Cash Voucher Status
            MyModel.Source.SetKey("UnPostBank", false, KeyTypes.Boolean, "");    // Reset Post Bank Voucher Status
            MyModel.Source.SetKey("UnPostReceipt", false, KeyTypes.Boolean, "");
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


        public async Task UnPostingVoucher(long id)
        {
            MyModel.IsPosting = true;
            StateHasChanged();

            await AppGlobal.JS.InvokeVoidAsync("showModal", "SaveVoucher");

            await MyModel.DoVoucherUnPost(id, MyViewModel.PostingType);

            await AppGlobal.JS.InvokeVoidAsync("hideModal", "SaveVoucher");
            MyModel.IsPosting = false;
            StateHasChanged();
        }
    }


    public class UnPostViewModel
    {
        public int PostingType { get; set; }
        public int PostingStatus { get; set; }
        public DateTime Dt_From { get; set; }
        public DateTime Dt_To { get; set; }
    }



}

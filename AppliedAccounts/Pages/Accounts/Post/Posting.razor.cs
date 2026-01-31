using AppliedAccounts.Data;
using AppliedAccounts.Models.Posting;
using Microsoft.JSInterop;
using static AppliedAccounts.Pages.Accounts.Post.Posting;
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
            MyViewModel = new(); ;
            MyViewModel.Dt_From = AppRegistry.GetDate(DBFile, "Post_dt_From");
            MyViewModel.Dt_To = AppRegistry.GetDate(DBFile, "Post_dt_To");
            MyViewModel.PostingType = (PostingTypes)AppRegistry.GetNumber(DBFile, "Post_Type");

            MyModel.Pages.PageChanged += OnPageChangedInternal;

            await MyModel.LoadData(MyViewModel);
            await InvokeAsync(StateHasChanged);
        }

        private async void OnPageChangedInternal(int page)
        {
            if (MyViewModel is null)
                return;

            await MyModel.LoadData(MyViewModel);
            await InvokeAsync(StateHasChanged);

        }

        #region Change Event

        private async Task OnPostingTypeChanged(PostingTypes value)
        {
            MyViewModel.PostingType = value;
            await MyModel.LoadData(MyViewModel);
        }
       

        private async void OnStatusChanged(int _PostingStatus)
        {

            MyViewModel.PostingStatus = _PostingStatus;
            await MyModel.LoadData(MyViewModel);
        
        }

        #endregion

        public async void Refresh()
        {
            AppRegistry.SetKey(DBFile, "Post_Type", MyViewModel.PostingType, KeyTypes.Number);
            AppRegistry.SetKey(DBFile, "Post_dt_From", MyViewModel.Dt_From, KeyTypes.Date);
            AppRegistry.SetKey(DBFile, "Post_dt_To", MyViewModel.Dt_To, KeyTypes.Date);
            AppRegistry.SetKey(DBFile, "PostCash", false, KeyTypes.Boolean);    // Reset Post Cash Voucher Status
            AppRegistry.SetKey(DBFile, "PostBank", false, KeyTypes.Boolean);    // Reset Post Bank Voucher Status
            AppRegistry.SetKey(DBFile, "PostReceipt", false, KeyTypes.Boolean);

            MyModel.Pages = new();
            await MyModel.LoadData(MyViewModel);
            await InvokeAsync(StateHasChanged);
        }

        public async Task PostVoucher(long id)
        {
            MyModel.IsPosting = true;
            StateHasChanged();

            await AppGlobal.JS.InvokeVoidAsync("showModal", "SaveVoucher");

            await MyModel.DoVoucherPosting(id, MyViewModel);

            await AppGlobal.JS.InvokeVoidAsync("hideModal", "SaveVoucher");
            MyModel.IsPosting = false;
            StateHasChanged();
        }

        public class PostingViewModel
        {
            public PostingTypes PostingType { get; set; }
            public int PostingStatus { get; set; }
            public DateTime Dt_From { get; set; }
            public DateTime Dt_To { get; set; }
        }
    }
}

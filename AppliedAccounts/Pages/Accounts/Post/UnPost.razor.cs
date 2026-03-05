using AppliedAccounts.Data;
using AppliedAccounts.Models.Posting;
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

            MyModel.Source.SetKey("IsUnPost", false, KeyTypes.Boolean, "Is Un-post is in progress..");
            MyViewModel = new();
            MyViewModel.Dt_From = MyModel.Source.GetDate("UnPost_dt_From");
            MyViewModel.Dt_To = MyModel.Source.GetDate("UnPost_dt_To");
            MyViewModel.PostingType = (PostingTypes)MyModel.Source.GetNumber("UnPost_Type");

            MyModel.Pages.PageChanged += OnPageChangedInternal;

            await MyModel.LoadData(MyViewModel);
        }

        private async void OnPageChangedInternal(int page)
        {
            if (MyViewModel is null) { return; }
            await MyModel.LoadData(MyViewModel);
            await InvokeAsync(StateHasChanged);
        }


        public async void Refresh()
        {
            AppRegistry.SetKey(DBFile, "UnPost_Type", (int)MyViewModel.PostingType, KeyTypes.Number, "");
            AppRegistry.SetKey(DBFile, "UnPost_dt_From", MyViewModel.Dt_From, KeyTypes.Date, "");
            AppRegistry.SetKey(DBFile, "UnPost_dt_To", MyViewModel.Dt_To, KeyTypes.Date, "");
            AppRegistry.SetKey(DBFile, "UnPostCash", false, KeyTypes.Boolean, "");    // Reset Post Cash Voucher Status
            AppRegistry.SetKey(DBFile, "UnPostBank", false, KeyTypes.Boolean, "");    // Reset Post Bank Voucher Status
            AppRegistry.SetKey(DBFile, "UnPostReceipt", false, KeyTypes.Boolean, "");

            MyModel.Pages = new();

            MyModel.FilterDates[0] = MyViewModel.Dt_From;
            MyModel.FilterDates[1] = MyViewModel.Dt_To;

            await MyModel.LoadData(MyViewModel);
            await InvokeAsync(StateHasChanged);
        }

        #region Change Event
        private async void OnPostingTypeChanged(PostingTypes value)
        {
            MyViewModel.PostingType = value;
            await MyModel.LoadData(MyViewModel);
            await InvokeAsync(StateHasChanged);
        }
        
        #endregion


        

        public async Task UnPostingVoucher(long id)
        {
            MyModel.IsPosting = true;

            //StateHasChanged();

            await AppGlobal.JS.InvokeVoidAsync("showModal", "SaveVoucher");
            await MyModel.DoVoucherUnPost(id, MyViewModel.PostingType);
            await AppGlobal.JS.InvokeVoidAsync("hideModal", "SaveVoucher");
            MyModel.IsPosting = false;
            
            await MyModel.LoadData(MyViewModel);
            await InvokeAsync(StateHasChanged);





        }
    }


    public class UnPostViewModel
    {
        public PostingTypes PostingType { get; set; }
        public int PostingStatus { get; set; }
        public DateTime Dt_From { get; set; }
        public DateTime Dt_To { get; set; }
    }



}

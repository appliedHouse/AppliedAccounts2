using AppliedAccounts.Models.Posting;
using Microsoft.JSInterop;
using static AppliedGlobals.AppErums;

namespace AppliedAccounts.Pages.Accounts.Post
{
    public partial class UnPost
    {
        public UnPostViewModel MyViewModel { get; set; } = new();
        public UnPostModel MyModel { get; set; }
        public string DBFile => AppGlobals.DBFile;
        public long UnPostVoucherID { get; set; } = 0;
        public string UnPostVoucher { get; set; } = string.Empty;
        protected override async Task OnInitializedAsync()
        {
            MyModel = new(AppGlobals); ;
            MyModel.Source = new(AppGlobals.AppPaths);
            MyModel.MsgService = AppGlobals.MsgService;

            MyModel.Source.SetKey("IsUnPost", false, KeyTypes.Boolean, "Is Un-post is in progress..");
            MyViewModel = new(); ;
            MyViewModel.Dt_From = MyModel.Source.GetDate("UnPost_dt_From");
            MyViewModel.Dt_To = MyModel.Source.GetDate("UnPost_dt_To");
            MyViewModel.PostingType = (PostingTypes)MyModel.Source.GetNumber("UnPost_Type");

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

        public async void Refresh()
        {
            MyModel.MsgService.Clear();
            MyModel.Source.SetKey("UnPost_Type", MyViewModel.PostingType, KeyTypes.Number);
            MyModel.Source.SetKey("UnPost_dt_From", MyViewModel.Dt_From, KeyTypes.Date);
            MyModel.Source.SetKey("UnPost_dt_To", MyViewModel.Dt_To, KeyTypes.Date);
            MyModel.Source.SetKey("UnPostCash", false, KeyTypes.Boolean);    // Reset Post Cash Voucher Status
            MyModel.Source.SetKey("UnPostBank", false, KeyTypes.Boolean);    // Reset Post Bank Voucher Status
            MyModel.Source.SetKey("UnPostReceipt", false, KeyTypes.Boolean);

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

            await AppGlobals.JS.InvokeVoidAsync("showModal", "SaveVoucher");
            await MyModel.DoVoucherUnPost(id, MyViewModel);
            await AppGlobals.JS.InvokeVoidAsync("hideModal", "SaveVoucher");
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

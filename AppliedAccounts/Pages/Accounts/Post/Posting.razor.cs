using AppliedAccounts.Models.Posting;
using Microsoft.JSInterop;
using static AppliedGlobals.AppErums;

namespace AppliedAccounts.Pages.Accounts.Post
{
    public partial class Posting
    {
        public PostingViewModel MyViewModel { get; set; }
        public PostingModel MyModel { get; set; }
        public long PostingVoucherID { get; set; } = 0;
        public string PostingVoucher { get; set; } = string.Empty;

        #region Constructor
        protected override async Task OnInitializedAsync()
        {
            MyModel = new(AppGlobal);

            MyModel.Source.SetKey("IsPosting", false, KeyTypes.Boolean, "Is posting is in progress..");
            MyViewModel = new(); ;
            MyViewModel.Dt_From = MyModel.Source.GetDate("Post_dt_From");
            MyViewModel.Dt_To = MyModel.Source.GetDate("Post_dt_To");
            MyViewModel.PostingType = (PostingTypes)MyModel.Source.GetNumber("Post_Type");

            MyModel.Pages.PageChanged += OnPageChangedInternal;

            await MyModel.LoadData(MyViewModel);
            await InvokeAsync(StateHasChanged);
        }
        #endregion

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
            MyModel.MsgService.Clear();
            MyModel.Source.SetKey("Post_Type", MyViewModel.PostingType, KeyTypes.Number);
            MyModel.Source.SetKey("Post_dt_From", MyViewModel.Dt_From, KeyTypes.Date);
            MyModel.Source.SetKey("Post_dt_To", MyViewModel.Dt_To, KeyTypes.Date);
            MyModel.Source.SetKey("PostCash", false, KeyTypes.Boolean);    // Reset Post Cash Voucher Status
            MyModel.Source.SetKey("PostBank", false, KeyTypes.Boolean);    // Reset Post Bank Voucher Status
            MyModel.Source.SetKey("PostReceipt", false, KeyTypes.Boolean);

            //MyModel.Pages = new();
            await MyModel.LoadData(MyViewModel);
            await InvokeAsync(StateHasChanged);
        }

        public async Task PostVoucher(long id)
        {
            MyModel.IsPosting = true;
            StateHasChanged();

            await AppGlobal.JS.InvokeVoidAsync("showModal", "SaveVoucher");

            var result = await MyModel.DoVoucherPosting(id, MyViewModel);

            if(result)
            {
                
                Toaster.ShowSuccess($"Voucher posted successfully.");
            }
            else
            {
                Toaster.ShowWarning("Voucher posting failed. Please check the details and try again.");
            }



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

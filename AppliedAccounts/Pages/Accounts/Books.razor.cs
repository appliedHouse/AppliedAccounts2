using AppliedAccounts.Data;
using AppliedAccounts.Models;
using AppliedAccounts.Services;
using Microsoft.AspNetCore.Components;
using System.Data;


namespace AppliedAccounts.Pages.Accounts
{
    public partial class Books
    {
        [Parameter] public long ID { get; set; }
        [Parameter] public long BookID { get; set; }

        public AppliedGlobals.AppUserModel UserProfile { get; set; }
        public BookModel MyModel { get; set; } = new();

        public bool IsPageValid { get; set; } = true;
        public ToastClass MyToastClass { get; set; }

        public ToastClass Toast { get; set; }


        private decimal Tot_DR = 0.0M;
        private decimal Tot_CR = 0.0M;
        private string ErrorMessage = string.Empty;

        

        public Books() 
        {
        }

        public void ShowToast(ToastClass _toast)
        {
            Toast = _toast;
            ToastService.ShowToast(Toast);
        }

        public void Start()
        {
            MyModel = new(ID, BookID, AppGlobal); ;
            MyModel.ReportService = ReportService;

            if (MyModel == null) { IsPageValid = false; MsgService.Warning("Model is null"); return; }
            if (MyModel?.MyVoucher == null) { IsPageValid = false; MsgService.Warning("Voucher is null"); return; }
            if (MyModel?.MyVoucher.Master == null) { IsPageValid = false; MsgService.Warning("Voucher master data is null"); return; }
            if (MyModel?.MyVoucher.Detail == null) { IsPageValid = false; MsgService.Warning("Voucher detail data is null"); return; }
        }

        #region Drop Down Value changed events
        private void BookIDChanged(long _BookID)
        {
            BookID = _BookID;
            MyModel.MyVoucher.Master.BookID = BookID;
        }

        private void AccountIDChanged(long _ID)
        {
            MyModel.MyVoucher.Detail.COA = _ID;
            MyModel.MyVoucher.Detail.TitleAccount = MyModel.Accounts
                .Where(e => e.ID == MyModel.MyVoucher.Detail.COA)
                .Select(e => e.Title)
                .First() ?? "";
        }

        private void CompanyIDChanged(long _ID)
        {
            MyModel.MyVoucher.Detail.Company = _ID;
            MyModel.MyVoucher.Detail.TitleCompany = MyModel.Companies
                .Where(e => e.ID == MyModel.MyVoucher.Detail.Company)
                .Select(e => e.Title)
                .First() ?? "";
        }
        private void ProjectIDChanged(long _ID)
        {
            MyModel.MyVoucher.Detail.Project = _ID;
            MyModel.MyVoucher.Detail.TitleProject = MyModel.Projects
                .Where(e => e.ID == MyModel.MyVoucher.Detail.Project)
                .Select(e => e.Title)
                .First() ?? "";

        }
        private void EmployeeIDChanged(long _ID)
        {
            MyModel.MyVoucher.Detail.Employee = _ID;
            MyModel.MyVoucher.Detail.TitleEmployee = MyModel.Employees
                .Where(e => e.ID == MyModel.MyVoucher.Detail.Employee)
                .Select(e => e.Title)
                .First() ?? "";
        }
        #endregion

        #region Save
        public async void SaveAll()
        {
            var IsSaved = false;
            MyModel.MyMessage = "Saving....";
            IsSaved = await MyModel.SaveAllAsync(); // Ensure save operation completes successfully

            if (IsSaved)
            {
                MyModel.IsWaiting = false;
                MyModel.LoadData();

                if(MyModel.MyVoucher.Details.Count == 0)
                {
                    MyModel.DeleteMaster();

                    // Delete Master record if (details are all deleted / empty)  20-DEC-2025
                }



                ToastService.ShowToast(ToastClass.SaveToast, MyModel.MyVoucher.Master.Vou_No);
                await InvokeAsync(StateHasChanged);
            }
            else
            {
                MsgService.AddRange(MyModel.MsgClass);
            }

            
        }
        #endregion

        #region BackPage
        public void BackPage() { NavManager.NavigateTo("/Accounts/BooksList"); }
        #endregion

        #region Print
        private async void Print(ReportActionClass reportAction)
        {
            MyModel.MyMessage = "Please wait... The report is being generated.";
            MyModel.IsWaiting = true; await InvokeAsync(StateHasChanged);
            await Task.Run(() => { MyModel.Print(reportAction); });
            MyModel.IsWaiting = false; await InvokeAsync(StateHasChanged);
        }
        #endregion

        
    }
}

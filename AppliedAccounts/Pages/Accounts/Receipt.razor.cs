using AppliedAccounts.Data;
using AppliedAccounts.Models;
using Microsoft.AspNetCore.Components;

namespace AppliedAccounts.Pages.Accounts
{
    public partial class Receipt : ComponentBase
    {
        public ReceiptModel MyModel { get; set; }
        public bool IsPageValid { get; set; }
        public string ErrorMessage { get; set; }
        private string SpinnerMessage { get; set; }

        public Receipt()
        {
            //MyModel = new ReceiptModel(AppGlobal);
            //IsPageValid = true;
            //ErrorMessage = string.Empty;
        }

        #region DropDown Changed
        private void COAIDChanged(long _ID)
        {
            MyModel.MyVoucher.Master.COA = _ID;
            MyModel.MyVoucher.Master.TitleCOA = MyModel.PayCOA
                .Where(e => e.ID == MyModel.MyVoucher.Master.COA)
                .Select(e => e.Title)
                .First() ?? "";
        }
        private void AccountIDChanged(long _ID)
        {
            MyModel.MyVoucher.Detail.Account = _ID;
            MyModel.MyVoucher.Detail.TitleAccount = MyModel.Accounts
                .Where(e => e.ID == MyModel.MyVoucher.Detail.Account)
                .Select(e => e.Title)
                .First() ?? "";
        }
        private void PayerIDChanged(long _ID)
        {
            MyModel.MyVoucher.Master.Payer = _ID;
            MyModel.MyVoucher.Master.TitlePayer = MyModel.Companies
                .Where(e => e.ID == MyModel.MyVoucher.Master.Payer)
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

        #region Back Page
        private void BackPage()
        {
            AppGlobal.NavManager.NavigateTo("/Accounts/ReceiptList");
        }
        #endregion

        #region Save
        private async void SaveAll()
        {
            var IsSaved = await MyModel.SaveAllAsync();

            await InvokeAsync(StateHasChanged);

            if (IsSaved)
            {
                Toaster.ShowSuccess($"Successfully saved {MyModel.MyVoucher.Master.Vou_No}");
                AppGlobal.NavManager.NavigateTo($"/Accounts/Receipt/{MyModel.MyVoucher.Master.ID1}");
            }
        }
        #endregion

        #region Print

        public async Task Print(ReportActionClass reportAction)
        {
            //MyModel.IsWaiting = true;
            //await InvokeAsync(StateHasChanged);

            await MyModel.Print(reportAction);

            //MyModel.IsWaiting = false;
            //await InvokeAsync(StateHasChanged);

        }
        #endregion

    }
}

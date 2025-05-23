using AppliedAccounts.Data;
using AppliedAccounts.Models;
using AppliedAccounts.Services;
using Microsoft.AspNetCore.Components;

namespace AppliedAccounts.Pages.Accounts
{
    public partial class Receipt : ComponentBase
    {
        public ReceiptModel MyModel { get; set; }
        public bool IsPageValid { get; set; }
        public string ErrorMessage { get; set; }
        private bool IsWaiting { get; set; }
        private string SpinnerMessage { get; set; }

        public Receipt()
        {
            MyModel = new ReceiptModel(AppGlobals);
            IsPageValid = true;
            ErrorMessage = string.Empty;
        }

        #region DropDown Changed
        private void COAIDChanged(int _ID)
        {
            MyModel.MyVoucher.Master.COA = _ID;
            MyModel.MyVoucher.Master.TitleCOA = MyModel.PayCOA
                .Where(e => e.ID == MyModel.MyVoucher.Master.COA)
                .Select(e => e.Title)
                .First() ?? "";
        }

        private void PayerIDChanged(int _ID)
        {
            MyModel.MyVoucher.Master.Payer = _ID;
            MyModel.MyVoucher.Master.TitlePayer = MyModel.Companies
                .Where(e => e.ID == MyModel.MyVoucher.Master.Payer)
                .Select(e => e.Title)
                .First() ?? "";
        }

        private void AccountIDChanged(int _ID)
        {
            MyModel.MyVoucher.Detail.Account = _ID;
            MyModel.MyVoucher.Detail.TitleAccount = MyModel.Accounts
                .Where(e => e.ID == MyModel.MyVoucher.Detail.Account)
                .Select(e => e.Title)
                .First() ?? "";
        }

        private void ProjectIDChanged(int _ID)
        {
            MyModel.MyVoucher.Detail.Project = _ID;
            MyModel.MyVoucher.Detail.TitleProject = MyModel.Projects
                .Where(e => e.ID == MyModel.MyVoucher.Detail.Project)
                .Select(e => e.Title)
                .First() ?? "";
        }

        private void EmployeeIDChanged(int _ID)
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
            NavManager.NavigateTo("/Accounts/ReceiptList");
        }
        #endregion

        #region Save
        private async void SaveAll()
        {
            var IsSaved = await MyModel.SaveAllAsync();

            await InvokeAsync(StateHasChanged);

            if (IsSaved)
            {
                ToastService.ShowToast(ToastClass.SaveToast, $"Save | {MyModel.MyVoucher.Master.Vou_No}"); // show the toast
                NavManager.NavigateTo($"/Accounts/Receipt/{MyModel.MyVoucher.Master.ID1}");
            }
        }
        #endregion

        #region Print

        public async void Print(ReportActionClass reportAction)
        {
            await MyModel.Print(reportAction);
        }
        #endregion

        public void TestRecord()
        {

            try
            {
                MyModel.TestNewAsync();



            }
            catch (Exception ex)
            {
                ToastService.ShowToast(ToastClass.ErrorToast, ex.Message);
            }


        }
    }
}

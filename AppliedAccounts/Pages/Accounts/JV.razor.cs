using AppliedAccounts.Data;
using AppliedAccounts.Models.Accounts;
using AppliedAccounts.Services;

namespace AppliedAccounts.Pages.Accounts
{
    public partial class JV
    {
        public JVModel MyModel { get; set; }
        public List<JVViewModel> MyVoucher => MyModel.MyVoucher;

        public bool IsPageValid { get; set; } = true;
        public ToastClass MyToastClass { get; set; }
        public ToastClass Toast { get; set; }
        
        public JV()
        {
            //MyModel = new(AppGlobal);
            MyToastClass = new();
            Toast = new();
        }


        #region Drop Down Value changed events
        private void AccountIDChanged(long _ID)
        {
            MyModel.Transaction.COA = _ID;
            MyModel.Transaction.TitleAccount = MyModel.Accounts
                .Where(e => e.ID == MyModel.Transaction.COA)
                .Select(e => e.Title)
                .First() ?? "";
        }

        private void CompanyIDChanged(long _ID)
        {
            MyModel.Transaction.Company = _ID;
            MyModel.Transaction.TitleCompany = MyModel.Companies
                .Where(e => e.ID == MyModel.Transaction.Company)
                .Select(e => e.Title)
                .First() ?? "";
        }
        private void ProjectIDChanged(long _ID)
        {
            MyModel.Transaction.Project = _ID;
            MyModel.Transaction.TitleProject = MyModel.Projects
                .Where(e => e.ID == MyModel.Transaction.Project)
                .Select(e => e.Title)
                .First() ?? "";

        }
        private void EmployeeIDChanged(long _ID)
        {
            MyModel.Transaction.Employee = _ID;
            MyModel.Transaction.TitleEmployee = MyModel.Employees
                .Where(e => e.ID == MyModel.Transaction.Employee)
                .Select(e => e.Title)
                .First() ?? "";
        }
        #endregion

        #region Back Page
        public void BackPage()
        {
            AppGlobal.NavManager.NavigateTo("/Accounts/JVList");
        }
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

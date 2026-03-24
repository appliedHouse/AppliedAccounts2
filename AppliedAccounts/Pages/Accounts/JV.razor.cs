using AppliedAccounts.Data;
using AppliedAccounts.Models;
using AppliedAccounts.Models.Accounts;
using AppliedAccounts.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Transactions;

namespace AppliedAccounts.Pages.Accounts
{
    public partial class JV
    {
        #region Variables
        [Parameter] public string Vou_No { get; set; }
        public JVModel MyModel { get; set; }
        public List<JVViewModel> MyVoucher => MyModel.MyVoucher;
        public bool IsPageValid { get; set; } = true;
        public ToastClass MyToastClass { get; set; }
        public ToastClass Toast { get; set; }
        public MessagesService MsgService { get; set; }
        public bool CanEdit => GetCanEdit();
        private bool GetCanEdit()
        {
            if(MyModel.MyVoucher.Count == 0) return false;
            if(MyModel.Transaction.Sr_No == 1) return false;
            return true;
        }

        #endregion


        public JV()
        {
            //MyModel = new(AppGlobal);
            MyToastClass = new();
            Toast = new();
        }

        public void ShowToast(ToastClass _toast)
        {
            Toast = _toast;
            ToastService.ShowToast(Toast);
        }

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

                ToastService.ShowToast(ToastClass.SaveToast, MyModel.Vou_No);
                await InvokeAsync(StateHasChanged);
            }
            else
            {
                MsgService.AddRange(MyModel.MsgClass);
            }


        }
        #endregion

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

        #region Browse Window

        #region Dropdown Changed
        public void AccountsChanged(long _ID)
        {
            MyModel.Transaction.COA = _ID;
            MyModel.Transaction.TitleAccount = MyModel.Accounts
                .Where(e => e.ID == MyModel.Transaction.COA)
                .Select(e => e.Title)
                .First() ?? "";
        }
        public void CompanyChanged(long _ID)
        {
            MyModel.Transaction.Company = _ID;
            MyModel.Transaction.TitleCompany = MyModel.Companies
                .Where(e => e.ID == MyModel.Transaction.Company)
                .Select(e => e.Title)
                .First() ?? "";
        }

        public void EmployeeChanged(long _ID)
        {
            MyModel.Transaction.Employee = _ID;
            MyModel.Transaction.TitleEmployee = MyModel.Employees
                .Where(e => e.ID == MyModel.Transaction.Employee)
                .Select(e => e.Title)
                .First() ?? "";
        }

        public void ProjectChanged(long _ID)
        {
            MyModel.Transaction.Project = _ID;
            MyModel.Transaction.TitleProject = MyModel.Projects
                .Where(e => e.ID == MyModel.Transaction.Project)
                .Select(e => e.Title)
                .First() ?? "";
        }



        #endregion
        private void SelectedBrowse(long selectedId)
        {
            if (MyModel.BrowseClass.Type == 1) { AccountsChanged(selectedId); }
            else if (MyModel.BrowseClass.Type == 2) { CompanyChanged(selectedId); }
            else if (MyModel.BrowseClass.Type == 3) { EmployeeChanged(selectedId); }
            else if (MyModel.BrowseClass.Type == 4) { ProjectChanged(selectedId); }
        }
        public async void BrowseWindow(long _ListType)
        {
            switch (_ListType)
            {
                case 0:                                         // Nill
                    MyModel.BrowseClass = new();
                    break;

                case 1:
                    MyModel.BrowseClass = new();
                    MyModel.BrowseClass.Type = 1;
                    MyModel.BrowseClass.Heading = "Accounts";
                    MyModel.BrowseClass.Selected = MyModel.Transaction.COA;
                    MyModel.BrowseClass.BrowseList = MyModel.Accounts;
                    break;


                case 2:
                    MyModel.BrowseClass = new();
                    MyModel.BrowseClass.Type = 2;
                    MyModel.BrowseClass.Heading = "Company";
                    MyModel.BrowseClass.Selected = MyModel.Transaction.Company;
                    MyModel.BrowseClass.BrowseList = MyModel.Companies;
                    break;

                case 3:
                    MyModel.BrowseClass = new();
                    MyModel.BrowseClass.Type = 3;
                    MyModel.BrowseClass.Heading = "Employee";
                    MyModel.BrowseClass.Selected = MyModel.Transaction.Employee;
                    MyModel.BrowseClass.BrowseList = MyModel.Employees;
                    break;

                case 4:
                    MyModel.BrowseClass = new();
                    MyModel.BrowseClass.Type = 4;
                    MyModel.BrowseClass.Heading = "Project";
                    MyModel.BrowseClass.Selected = MyModel.Transaction.Project;
                    MyModel.BrowseClass.BrowseList = MyModel.Projects;
                    break;

                default:
                    MyModel.BrowseClass = new();
                    break;
            }

            await InvokeAsync(StateHasChanged);
            await AppGlobal.JS.InvokeVoidAsync("showModol", "winBrowse");

        }
        #endregion

    }
}

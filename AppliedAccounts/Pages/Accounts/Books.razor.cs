﻿using AppliedAccounts.Data;
using AppliedAccounts.Models;
using AppliedAccounts.Services;
using AppMessages;
using Microsoft.AspNetCore.Components;
using System.Data;


namespace AppliedAccounts.Pages.Accounts
{
    public partial class Books
    {
        [Parameter] public int ID { get; set; }
        [Parameter] public int BookID { get; set; }

        public AppliedGlobals.AppUserModel UserProfile { get; set; }
        public BookModel MyModel { get; set; } = new();
        public MessageClass MsgClass { get; set; }

        public bool IsPageValid { get; set; } = true;
        public ToastClass MyToastClass { get; set; }

        public ToastClass Toast { get; set; }
        public Books() { }

        public void ShowToast(ToastClass _toast)
        {
            Toast = _toast;
            ToastService.ShowToast(Toast);
        }

        public void Start()
        {
            MsgClass = new();
            MyToastClass = new();
            MyModel = new(ID, BookID, AppGlobal); ;
            //MyModel.AppGlobal = AppGlobal;
            MyModel.ReportService = ReportService;

            if (MyModel == null) { IsPageValid = false; MsgClass.Add("Model is null"); return; }
            if (MyModel?.MyVoucher == null) { IsPageValid = false; MsgClass.Add("Voucher is null"); return; }
            if (MyModel?.MyVoucher.Master == null) { IsPageValid = false; MsgClass.Add("Voucher master data is null"); return; }
            if (MyModel?.MyVoucher.Detail == null) { IsPageValid = false; MsgClass.Add("Voucher detail data is null"); return; }
        }

        #region Drop Down Value changed events
        private void BookIDChanged(int _BookID)
        {
            BookID = _BookID;
            MyModel.MyVoucher.Master.BookID = BookID;
        }

        private void AccountIDChanged(int _ID)
        {
            MyModel.MyVoucher.Detail.COA = _ID;
            MyModel.MyVoucher.Detail.TitleAccount = MyModel.Accounts
                .Where(e => e.ID == MyModel.MyVoucher.Detail.COA)
                .Select(e => e.Title)
                .First() ?? "";
        }

        private void CompanyIDChanged(int _ID)
        {
            MyModel.MyVoucher.Detail.Company = _ID;
            MyModel.MyVoucher.Detail.TitleCompany = MyModel.Companies
                .Where(e => e.ID == MyModel.MyVoucher.Detail.Company)
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

        #region Save
        public async void SaveAll()
        {
            var IsSaved = false;
            MyModel.MyMessage = "Saving....";
            IsSaved = await MyModel.SaveAllAsync(); // Ensure save operation completes successfully

            if (IsSaved)
            {
                MyModel.IsWaiting = false;
                ToastService.ShowToast(ToastClass.SaveToast, MyModel.MyVoucher.Master.Vou_No);
                await InvokeAsync(StateHasChanged);
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

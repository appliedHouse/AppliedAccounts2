using AppliedAccounts.Data;
using AppliedAccounts.Models;
using AppliedDB;
using Microsoft.AspNetCore.Components;

namespace AppliedAccounts.Pages.Accounts
{
    public partial class Books
    {
        [Parameter] public int ID { get; set; }
        [Parameter] public int NatureID { get; set; }

        public AppUserModel UserProfile { get; set; }
        public BookModel MyModel { get; set; } = new();
        public int BookID { get; set; }
        public AppMessages.MessageClass MsgClass { get; set; }

        public bool IsPageValid { get; set; } = true;

        public Books() { }

        public void Start()
        {
            MsgClass = new();
            MyModel = new(ID,UserProfile);
            

            if (MyModel == null) { IsPageValid = false; MsgClass.Add("Model is null"); return; }
            if (MyModel?.MyVoucher == null) { IsPageValid = false; MsgClass.Add("Voucher is null"); return; }
            if (MyModel?.MyVoucher.Master == null) { IsPageValid = false; MsgClass.Add("Voucher master data is null"); return; }
            if (MyModel?.MyVoucher.Detail == null) { IsPageValid = false; MsgClass.Add("Voucher detail data is null"); return; }
        }

        #region Drop Down Value changed events
        private void BookIDChanged(int _BookID)
        {
            MyModel.MyVoucher.Master.BookID = _BookID;
            
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

    }



}

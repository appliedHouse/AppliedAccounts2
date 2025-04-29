using AppliedAccounts.Data;
using AppliedAccounts.Models;
using AppliedAccounts.Models.Interface;
using AppliedAccounts.Services;
using AppliedDB;
using AppMessages;
using AppReports;
using Microsoft.Reporting.Map.WebForms.BingMaps;

//using AppliedReports;

namespace AppliedAccounts.Pages.Sale
{
    public partial class SaleInvoice
    {
        #region Variables

        public SaleInvoiceModel MyModel { get; set; } = new();
        public bool IsPageValid { get; set; }
        public string ErrorMessage { get; set; }
        private bool IsWaiting { get; set; }
        private string SpinnerMessage { get; set; }

        #endregion

        #region Constructor
        public SaleInvoice()
        {
            IsPageValid = true;
            IsWaiting = false;
            ErrorMessage = string.Empty;
        }
        #endregion

        #region DropDown List change
        public void CompanyChanged(int _NewValue)
        {
            MyModel.MyVoucher.Master.Company = _NewValue;
            MyModel.MyVoucher.Master.TitleCompany = MyModel.Companies.Where(e => e.ID == _NewValue).FirstOrDefault()!.Title ?? "";
        }
        public void EmployeeChanged(int _NewValue)
        {
            MyModel.MyVoucher.Master.Employee = _NewValue;
            MyModel.MyVoucher.Master.TitleEmployee = MyModel.Employees.Where(e => e.ID == _NewValue).FirstOrDefault()!.Title ?? "";
        }
        public void InventoryChanged(int _NewValue)
        {
            MyModel.MyVoucher.Detail.Inventory = _NewValue;
            MyModel.MyVoucher.Detail.TitleInventory = MyModel.Inventory.Where(e => e.ID == _NewValue).FirstOrDefault()!.Title ?? "";
        }
        public void TaxChanged(int _NewValue)
        {
            MyModel.MyVoucher.Detail.TaxID = _NewValue;
            MyModel.MyVoucher.Detail.TitleTaxID = MyModel.Taxes.Where(e => e.ID == _NewValue).FirstOrDefault()!.Title ?? "";
            MyModel.MyVoucher.Detail.TaxRate = MyModel.Source.SeekTaxRate(MyModel.MyVoucher.Detail.TaxID);
        }
        public void UnitChanged(int _NewValue)
        {
            MyModel.MyVoucher.Detail.Unit = _NewValue;
            MyModel.MyVoucher.Detail.TitleUnit = MyModel.Taxes.Where(e => e.ID == _NewValue).FirstOrDefault()!.Title ?? "";
        }
        #endregion


        #region Submit & Digitis
        public void Submit()
        {

        }


        public bool ShowDigits = false;
        public void Digits()
        {
            if (ShowDigits)
            {
                MyModel.Totals.NumberFormat = Format.Number;
            }
            else
            {
                MyModel.Totals.NumberFormat = Format.Digit;
            }
            //MyModel.CalculateTotal();
        }
        #endregion

      

      
        public void SaveAll()
        {
            //Model.Save();
            //Record = Model.SaleInvoiceRecord;
            //Printmodel.ReportData.ReportTable = Model.SaleInvoiceRecords.ToDataTable();
        }

        #region Delete
        public void Delete(int _Sr_No)
        {
            MyModel.MyVoucher.Detail = MyModel.MyVoucher.Details.Where(row => row.Sr_No==_Sr_No).First();
            if (MyModel.MyVoucher.Detail is not null)
            {
                MyModel.Deleted.Add(MyModel.MyVoucher.Detail);                  // Save in deleted list
                MyModel.MyVoucher.Details.Remove(MyModel.MyVoucher.Detail);     // remove from detail list
                MyModel.MsgClass.Add(AppMessages.Enums.Messages.RowDeleted);    // Set message to display after deleted.
            }

        }
        #endregion
       

        #region Home & Back Buttons
        public void GotoHome()
        {
            NavManager.NavigateTo("/");
        }

        public void BackPage()
        {
            NavManager.NavigateTo("/Sale/SaleInvoiceList");
        }

        public void TestRecord()
        {

        }

        #endregion

        #region Print
        public void Print()
        {
            //ReportModel Reportmodel = Model.Print();

        }

        public void Print(ReportType RptType)
        {

        }

        
        #endregion



    }
}
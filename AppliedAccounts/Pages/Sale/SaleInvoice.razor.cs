using AppliedAccounts.Data;
using AppliedAccounts.Models;
using AppliedAccounts.Models.Interface;
using AppliedAccounts.Services;
using AppliedDB;
using AppMessages;
using AppReports;

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

        #region New Sale Invoice
        //public void New()
        //{
        //    MyRecord = new SaleInvoiceRecord();
        //    if (MyModel.SaleInvoiceRecords.Count > 0)
        //    {
        //        var _FirstRecord = Model.SaleInvoiceRecords[0];
        //        var _MaxSrNo = Model.SaleInvoiceRecords.Max(e => e.Sr_No) + 1;

        //        Record = new();
        //        {
        //            Record.ID1 = _FirstRecord.ID1;
        //            Record.Vou_No = _FirstRecord.Vou_No;
        //            Record.Vou_Date = _FirstRecord.Vou_Date;
        //            Record.Company = _FirstRecord.Company;
        //            Record.Employee = _FirstRecord.Employee;
        //            Record.Ref_No = _FirstRecord.Ref_No;
        //            Record.Inv_No = _FirstRecord.Inv_No;
        //            Record.Inv_Date = _FirstRecord.Vou_Date;
        //            Record.Pay_Date = _FirstRecord.Pay_Date;
        //            Record.Remarks = _FirstRecord.Remarks;
        //            Record.Comments = _FirstRecord.Comments;
        //            Record.Status = "Insert";

        //            Record.ID2 = 0;
        //            Record.Sr_No = _MaxSrNo;
        //            Record.Inventory = 0;
        //            Record.Batch = "";
        //            Record.Qty = 0.00M;
        //            Record.Rate = 0.00M;
        //            Record.TaxID = 0;
        //            Record.TaxRate = 0.00M;
        //            Record.Description = "";
        //            Record.Project = 0;
        //        }

        //    }
        //    else
        //    {
        //        Record = new SaleInvoiceRecord();;
        //        Record.ID1 = 0;
        //        Record.Vou_No = "NEW";
        //        Record.Vou_Date = DateTime.Now;
        //        Record.Company = 0;
        //        Record.Employee = 0;
        //        Record.Ref_No = string.Empty;
        //        Record.Inv_No = string.Empty;
        //        Record.Inv_Date = DateTime.Now;
        //        Record.Pay_Date = DateTime.Now;
        //        //Record.Amount = 0.00M;
        //        Record.Remarks = string.Empty;
        //        Record.Comments = string.Empty;
        //        Record.Status = string.Empty;

        //        Record.ID2 = 0;
        //        Record.Sr_No = 1;
        //        Record.Inventory = 0;
        //        Record.Batch = "";
        //        Record.Qty = 0.00M;
        //        Record.Rate = 0.00M;
        //        Record.TaxID = 0;
        //        Record.TaxRate = 0.00M;
        //        Record.Description = "";
        //        Record.Project = 0;
        //    }
        //}

        //public void Edit(int _Sr_No)
        //{
        //    foreach (SaleInvoiceRecord _Record in Model.SaleInvoiceRecords)
        //    {
        //        if (_Record.Sr_No == _Sr_No)
        //        {
        //            Record = _Record;
        //            break;
        //        }
        //    }
        //}
        #endregion

      
        #region Save / Update
        //public void Update()
        //{
        //    //Model.NewRecord(Record);

        //}

        public void SaveAll()
        {
            //Model.Save();
            //Record = Model.SaleInvoiceRecord;
            //Printmodel.ReportData.ReportTable = Model.SaleInvoiceRecords.ToDataTable();
        }
        #endregion

        #region Delete
        public void Delete(int _Sr_No)
        {
            //foreach (SaleInvoiceRecord _Record in Model.SaleInvoiceRecords)
            //{
            //    if (_Record.Sr_No == _Sr_No)
            //    {
            //        _Record.Sr_No = _Record.Sr_No * -1;
            //    }
            //}
            //Model.SetTotals();

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
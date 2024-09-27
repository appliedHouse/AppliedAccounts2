using AppliedAccounts.Data;
using AppliedAccounts.Models;
//using AppliedReports;

namespace AppliedAccounts.Pages.Sale
{
    public partial class SaleInvoice
    {
        #region Variables
        public SaleInvoiceModel Model { get; set; } = new();
        public SaleInvoiceRecord Record { get; set; } = new();
        public int RecordPointer { get; set; }
        #endregion

        #region Constructor
        public SaleInvoice()
        {

        }
        #endregion

        #region Submit & Digitis
        public void Submit()
        {

        }

        public void Digits()
        {
            if (Model.ShowDigits)
            {
                Model.NumberFormat = Format.Number;
            }
            else
            {
                Model.NumberFormat = Format.Digit;
            }
            Model.SetTotals();
        }
        #endregion

        #region New Sale Invoice
        public void New()
        {
            Record = new SaleInvoiceRecord();
            if (Model.SaleInvoiceRecords.Count > 0)
            {
                var _FirstRecord = Model.SaleInvoiceRecords[0];
                var _MaxSrNo = Model.SaleInvoiceRecords.Max(e => e.Sr_No) + 1;

                Record = new();
                {
                    Record.ID1 = _FirstRecord.ID1;
                    Record.Vou_No = _FirstRecord.Vou_No;
                    Record.Vou_Date = _FirstRecord.Vou_Date;
                    Record.Company = _FirstRecord.Company;
                    Record.Employee = _FirstRecord.Employee;
                    Record.Ref_No = _FirstRecord.Ref_No;
                    Record.Inv_No = _FirstRecord.Inv_No;
                    Record.Inv_Date = _FirstRecord.Vou_Date;
                    Record.Pay_Date = _FirstRecord.Pay_Date;
                    Record.Remarks = _FirstRecord.Remarks;
                    Record.Comments = _FirstRecord.Comments;
                    Record.Status = "Insert";

                    Record.ID2 = 0;
                    Record.Sr_No = _MaxSrNo;
                    Record.Inventory = 0;
                    Record.Batch = "";
                    Record.Qty = 0.00M;
                    Record.Rate = 0.00M;
                    Record.TaxID = 0;
                    Record.TaxRate = 0.00M;
                    Record.Description = "";
                    Record.Project = 0;
                }

            }
            else
            {
                Record = new SaleInvoiceRecord();;
                Record.ID1 = 0;
                Record.Vou_No = "NEW";
                Record.Vou_Date = DateTime.Now;
                Record.Company = 0;
                Record.Employee = 0;
                Record.Ref_No = string.Empty;
                Record.Inv_No = string.Empty;
                Record.Inv_Date = DateTime.Now;
                Record.Pay_Date = DateTime.Now;
                //Record.Amount = 0.00M;
                Record.Remarks = string.Empty;
                Record.Comments = string.Empty;
                Record.Status = string.Empty;

                Record.ID2 = 0;
                Record.Sr_No = 1;
                Record.Inventory = 0;
                Record.Batch = "";
                Record.Qty = 0.00M;
                Record.Rate = 0.00M;
                Record.TaxID = 0;
                Record.TaxRate = 0.00M;
                Record.Description = "";
                Record.Project = 0;
            }
        }

        public void Edit(int _Sr_No)
        {
            foreach (SaleInvoiceRecord _Record in Model.SaleInvoiceRecords)
            {
                if (_Record.Sr_No == _Sr_No)
                {
                    Record = _Record;
                    break;
                }
            }
        }
        #endregion

        #region DropDown List change
        public void CompanyChanged(int _NewValue)
        {
            Record.Company = _NewValue;
            Record.TitleCompany = AppliedDB.DataSource.GetTitle(Model.Customers, _NewValue);
        }
        public void EmployeeChanged(int _NewValue)
        {
            Record.Employee = _NewValue;
            Record.TitleEmployee = AppliedDB.DataSource.GetTitle(Model.Employees, _NewValue);
        }
        public void InventoryChanged(int _NewValue)
        {
            Record.Inventory = _NewValue;
            Record.TitleInventory = AppliedDB.DataSource.GetTitle(Model.Inventory, _NewValue);
        }
        public void TaxChanged(int _NewValue)
        {
            Record.TaxID = _NewValue;
            Record.TitleTaxID = AppliedDB.DataSource.GetTitle(Model.Taxes, _NewValue);
            Record.TaxRate = Model.Source.SeekTaxRate(Record.TaxID);
        }
        public void UnitChanged(int _NewValue)
        {
            Record.Unit = _NewValue;
            Record.TitleUnit = AppliedDB.DataSource.GetTitle(Model.Units, _NewValue);
        }
        #endregion

        #region Save / Update
        public void Update()
        {
            Model.NewRecord(Record);

        }

        public void Save()
        {
            Model.Save();
            Record = Model.SaleInvoiceRecord;
            //Printmodel.ReportData.ReportTable = Model.SaleInvoiceRecords.ToDataTable();
        }
        #endregion

        #region Delete
        public void Delete(int _Sr_No)
        {
            foreach (SaleInvoiceRecord _Record in Model.SaleInvoiceRecords)
            {
                if (_Record.Sr_No == _Sr_No)
                {
                    _Record.Sr_No = _Record.Sr_No * -1;
                }
            }
            Model.SetTotals();

        }
        #endregion

        #region Record Navigate
        public void First()
        {
            if (Model.SaleInvoiceRecords.Count > 0)
            {
                RecordPointer = 0;
                var _ID = Model.SaleInvoiceRecords.First().ID2;
                var _Found = Model.SetSaleInvoiceRecord(_ID);
                if (_Found)
                {
                    Record = Model.SaleInvoiceRecord;
                }
            }
        }

        public void Next()
        {
            if (Model.SaleInvoiceRecords.Count > 0)
            {
                var _LastPoint = Model.SaleInvoiceRecords.Count - 1;
                RecordPointer += 1;
                if (RecordPointer > _LastPoint) { RecordPointer = 0; }
                Record = Model.SaleInvoiceRecords[RecordPointer];

            }
        }

        public void Back()
        {
            if (Model.SaleInvoiceRecords.Count > 0)
            {
                var _LastPoint = Model.SaleInvoiceRecords.Count - 1;
                RecordPointer -= 1;
                if (RecordPointer < 0) { RecordPointer = _LastPoint; }
                Record = Model.SaleInvoiceRecords[RecordPointer];

            }
        }

        public void Last()
        {
            if (Model.SaleInvoiceRecords.Count > 0)
            {
                RecordPointer = Model.SaleInvoiceRecords.Count;
                var _ID = Model.SaleInvoiceRecords.Last().ID2;
                var _Found = Model.SetSaleInvoiceRecord(_ID);
                if (_Found)
                {
                    Record = Model.SaleInvoiceRecord;
                }
            }

        }
        #endregion

        #region Home & Back Buttons
        public void GotoHome()
        {
            NavManager.NavigateTo("/");
        }

        public void GotoBack()
        {
            NavManager.NavigateTo("/Sale/SaleInvoiceList");
        }

        #endregion

        #region Print
        public void Print()
        {
            //ReportModel Reportmodel = Model.Print();

        }
        #endregion



    }
}
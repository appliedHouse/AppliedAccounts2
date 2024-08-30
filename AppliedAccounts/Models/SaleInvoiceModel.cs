using AppliedAccounts.Authentication;
using AppliedAccounts.Data;
using AppliedDB;
using AppMessages;
using AppReports;
using System.Data;
using static AppliedAccounts.Data.AppRegistry;
using static AppliedDB.VoucherTypeClass;
using MESSAGES = AppMessages.Enums.Messages;
using Tables = AppliedDB.Enums.Tables;

namespace AppliedAccounts.Models
{
    public class SaleInvoiceModel
    {
        #region Constructor
        public SaleInvoiceModel(AppUserModel _UserProfile)
        {
            UserProfile = _UserProfile;
            Source = new DataSource(UserProfile);
            View_SalesInvoice = Source.GetTable(AppliedDB.Enums.Query.SaleInvoiceView);
            Customers = Source.GetCustomers();
            Employees = Source.GetEmployees();
            Projects = Source.GetProjects();
            Inventory = Source.GetInventory();
            Taxes = Source.GetTaxes();
            Units = Source.GetUnits();
            //Report = Print();

            if (View_SalesInvoice.Rows.Count > 0)
            {
                InvoiceID = (int)View_SalesInvoice.Rows[0]["ID"];
                IsRecordLoaded = SetSaleInvoiceRecord(InvoiceID);
                //Report.ReportData.ReportTable = GetReportTable();

            }
        }

        public SaleInvoiceModel()
        {
            //Report = Print();

        }
        #endregion

        #region Variables
        public AppUserModel UserProfile { get; set; } = new();
        public DataSource Source { get; set; }
        public string InvoiceNo { get; set; } = string.Empty;
        public int InvoiceID { get; set; } = 0;
        public DataTable View_SalesInvoice { get; set; } = new();
        public SaleInvoiceRecord SaleInvoiceRecord { get; set; } = new();
        public List<SaleInvoiceRecord> SaleInvoiceRecords { get; set; } = new();
        public bool IsRecordLoaded { get; set; }
        public SaleInvoiceTotal Totals { get; set; } = new();
        public bool ShowDigits { get; set; } = true;
        public List<CodeTitle> Customers { get; set; }
        public List<CodeTitle> Employees { get; set; }
        public List<CodeTitle> Projects { get; set; }
        public List<CodeTitle> Inventory { get; set; }
        public List<CodeTitle> Taxes { get; set; }
        public List<CodeTitle> Units { get; set; }
        public AppMessages.AppMessages Messages { get; set; } = new();
        public string NumberFormat { get; set; } = Format.Number;
        public string DateFormat { get; set; } = Format.DDMMMYY;
        public ReportModel Report { get; set; } = new();
        #endregion

        #region Save
        public void Save()
        {
            if (SaleInvoiceRecords.Count > 0)
            {
                SetTotals();            // Calculate Invoice Totals Amounts
                var TB_Sale1 = Source.GetTable(Tables.BillReceivable);
                var TB_Sale2 = Source.GetTable(Tables.BillReceivable2);
                var First = SaleInvoiceRecords[0];

                DataRow Row1;
                List<DataRow> Rows2 = new();

                #region DataRows from Model Sale Recrods.

                Row1 = TB_Sale1.NewRow();

                Row1["ID"] = First.ID1;
                Row1["Vou_No"] = First.Vou_No;
                Row1["Vou_Date"] = First.Vou_Date;
                Row1["Company"] = First.Company;
                Row1["Employee"] = First.Employee;
                Row1["Inv_No"] = First.Inv_No;
                Row1["Inv_Date"] = First.Inv_Date;
                Row1["Pay_Date"] = First.Pay_Date;
                Row1["Amount"] = Totals.Tot_NetAmount;
                Row1["Description"] = First.Remarks;
                Row1["Comments"] = First.Comments;
                Row1["Status"] = VoucherStatus.Submitted;

                foreach (SaleInvoiceRecord _Record in SaleInvoiceRecords)
                {
                    DataRow Row2 = TB_Sale2.NewRow();

                    Row2["ID"] = _Record.ID2;
                    Row2["TranID"] = _Record.TranID;
                    Row2["Sr_No"] = _Record.Sr_No;
                    Row2["Inventory"] = _Record.Inventory;
                    Row2["Batch"] = _Record.Batch;
                    Row2["Qty"] = _Record.Qty;
                    Row2["Rate"] = _Record.Rate;
                    Row2["Tax"] = _Record.TaxID;
                    Row2["Tax_Rate"] = _Record.TaxRate;
                    Row2["Description"] = _Record.Description;
                    Row2["Project"] = _Record.Project;
                    Rows2.Add(Row2);
                }
                #endregion

                #region Validation of Data Rows

                var _Validate1 = Validation(Row1);
                var _Validate2 = true;
                foreach (DataRow _Row in Rows2)
                {
                    if (!Validation(_Row))
                    {
                        _Validate2 = false;
                    }
                }
                #endregion

                #region SAVE Date into Data Tables.


                if (_Validate1 && _Validate2)
                {
                    // Bill Payable Master Record 


                    if ((string)Row1["Vou_No"] == "New")
                    {
                        Row1["Vou_No"] = NewVoucherNo.GetSaleVouNo(UserProfile.DataFile);
                    }

                    string DBFile = UserProfile.DataFile;
                    CommandClass _CommandClass = new(Row1, DBFile);
                    _CommandClass.SaveChanges();

                    if (_CommandClass.Effected > 0) { Messages.Add(MESSAGES.Save); }
                    else { Messages.Add(MESSAGES.NotSave); }

                    // Bill Payable Details record
                    var _SavedRecords = 0;
                    var _SrNo = 1;
                    foreach (DataRow Row in Rows2)
                    {

                        Row["Sr_No"] = _SrNo;
                        _CommandClass = new(Row, DBFile);

                        if ((int)Row["ID"] >= 0)
                        {

                            _CommandClass.SaveChanges();
                            if (_CommandClass.Effected > 0) { Messages.Add(MESSAGES.Save); _SavedRecords += 1; _SrNo += 1; }
                            else { Messages.Add(MESSAGES.NotSave); }
                        }
                        else
                        {
                            _CommandClass.CommandDelete.Parameters["@ID"].Value = (int)Row["ID"] * -1;
                            _CommandClass.CommandDelete.ExecuteNonQuery();
                            if (_CommandClass.Effected > 0) { Messages.Add(MESSAGES.RowDeleted); _SavedRecords += 1; }
                            else { Messages.Add(MESSAGES.RowNotDeleted); }
                        }

                    }
                }
                #endregion

                #region Refresh Web Page Data

                View_SalesInvoice = Source.GetTable(Tables.view_BillReceivable);
                SaleInvoiceRecords = SetSaleInvoice(InvoiceNo);
                Report.ReportData.ReportTable = GetReportTable();

                #endregion
            }

        }
        #endregion

        #region Get Report table
        public DataTable GetReportTable()
        {
            return Source.GetTable(AppliedDB.Enums.Query.SaleInvoice, InvoiceID);
        }
        #endregion

        #region Record Add in List
        public int NewRecord(SaleInvoiceRecord NewRecord)
        {
            var _Found = false;
            foreach (SaleInvoiceRecord _Record in SaleInvoiceRecords)
            {
                if (_Record.Sr_No == NewRecord.Sr_No)
                {
                    _Found = true;
                }
            }

            if (!_Found)
            {
                NewRecord.Status = VoucherStatus.Submitted.ToString();
                NewRecord.ID1 = 0;
                NewRecord.ID2 = 0;
                SaleInvoiceRecords.Add(NewRecord);
            }
            return 0;
        }
        #endregion

        #region Get / Set Sales Invoice Record (Table to Variable)
        public bool SetSaleInvoiceRecord(int _ID)
        {
            if (View_SalesInvoice is not null)
            {
                if (View_SalesInvoice.Rows.Count > 0)
                {
                    View_SalesInvoice.DefaultView.RowFilter = $"ID2={_ID}";
                    if (View_SalesInvoice.DefaultView.Count > 0)
                    {
                        var _Row = View_SalesInvoice.DefaultView[0].Row;
                        _Row = AppliedDB.Functions.RemoveNull(_Row);

                        SaleInvoiceRecord = new();
                        SaleInvoiceRecord.ID1 = (int)_Row["ID1"];
                        SaleInvoiceRecord.ID2 = (int)_Row["ID2"];
                        SaleInvoiceRecord.Vou_No = (string)_Row["Vou_No"];
                        SaleInvoiceRecord.Ref_No = (string)_Row["Ref_No"];
                        SaleInvoiceRecord.Inv_No = (string)_Row["Inv_No"];
                        SaleInvoiceRecord.TranID = (int)_Row["TranID"];
                        SaleInvoiceRecord.Sr_No = (int)_Row["Sr_No"];
                        SaleInvoiceRecord.Inventory = (int)_Row["Inventory"];
                        SaleInvoiceRecord.Company = (int)_Row["Company"];
                        SaleInvoiceRecord.Employee = (int)_Row["Employee"];
                        SaleInvoiceRecord.Project = (int)_Row["Project"];
                        SaleInvoiceRecord.TaxID = (int)_Row["Tax"];
                        SaleInvoiceRecord.Vou_Date = (DateTime)_Row["Vou_Date"];
                        SaleInvoiceRecord.Inv_Date = (DateTime)_Row["Inv_Date"];
                        SaleInvoiceRecord.Pay_Date = (DateTime)_Row["Pay_Date"];
                        SaleInvoiceRecord.Remarks = (string)_Row["Description"];
                        SaleInvoiceRecord.Comments = (string)_Row["Comments"];
                        SaleInvoiceRecord.Description = (string)_Row["Description2"];
                        SaleInvoiceRecord.Batch = (string)_Row["Batch"];
                        SaleInvoiceRecord.Status = (string)_Row["Status"];
                        SaleInvoiceRecord.Qty = (decimal)_Row["Qty"];
                        SaleInvoiceRecord.Rate = (decimal)_Row["Rate"];
                        SaleInvoiceRecord.TaxRate = (decimal)_Row["Tax_Rate"];

                        SaleInvoiceRecord.TitleInventory = Source.SeekTitle(Tables.Inventory, (int)_Row["Inventory"]);
                        SaleInvoiceRecord.TitleEmployee = Source.SeekTitle(Tables.Employees, (int)_Row["Employee"]);
                        SaleInvoiceRecord.TitleProject = Source.SeekTitle(Tables.Project, (int)_Row["Project"]);
                        SaleInvoiceRecord.TitleTaxID = Source.SeekTitle(Tables.Taxes, (int)_Row["Tax"]);
                        SaleInvoiceRecord.TaxRate = Source.SeekTaxRate((int)_Row["Tax"]);

                        SetTotals();

                        return true;

                    }
                }
            }
            return false;
        }

        public bool GetSaleInvoiceRecord(SaleInvoiceRecord _ModelRecord)
        {
            if (_ModelRecord.ID2 > 0)
            {
                foreach (SaleInvoiceRecord _Item in SaleInvoiceRecords)
                {
                    _Item.Vou_No = _ModelRecord.Vou_No;
                    _Item.Vou_Date = _ModelRecord.Vou_Date;
                    _Item.Inv_No = _ModelRecord.Inv_No;
                    _Item.Inv_Date = _ModelRecord.Inv_Date;
                    _Item.Pay_Date = _ModelRecord.Pay_Date;
                    _Item.Ref_No = _ModelRecord.Ref_No;
                    _Item.Company = _ModelRecord.Company;
                    _Item.Employee = _ModelRecord.Employee;
                    _Item.Remarks = _ModelRecord.Remarks;
                    _Item.Comments = _ModelRecord.Comments;
                    _Item.Status = "Updated";

                    if (_Item.ID2 == _ModelRecord.ID2)
                    {
                        _Item.Inventory = _ModelRecord.Inventory;
                        _Item.Qty = _ModelRecord.Qty;
                        _Item.Rate = _ModelRecord.Rate;
                        _Item.TaxID = _ModelRecord.TaxID;
                        _Item.Project = _ModelRecord.Project;
                        _Item.Description = _ModelRecord.Description;
                    }
                }

            }

            return false;
        }
        public void GetSaleInvoice(string _InvoiceNo)
        {
            SaleInvoiceRecords = SetSaleInvoice(_InvoiceNo);
            Report.ReportData.ReportTable = SaleInvoiceRecords.ToDataTable();
            SetTotals();
        }

        public List<SaleInvoiceRecord> SetSaleInvoice(string _InvoiceNo)
        {
            List<SaleInvoiceRecord> _List = new();
            if (_InvoiceNo is not null)
            {

                InvoiceNo = _InvoiceNo;


                View_SalesInvoice.DefaultView.RowFilter = $"Vou_No='{InvoiceNo}'";
                if (View_SalesInvoice.DefaultView.Count > 0)
                {
                    foreach (DataRowView RowView in View_SalesInvoice.DefaultView)
                    {
                        var _ID = (int)RowView["ID2"];
                        if (SetSaleInvoiceRecord(_ID))
                        {

                            _List.Add(SaleInvoiceRecord);
                        }
                    }

                }
            }


            return _List;
        }


        #endregion

        #region Print
        public ReportModel Print()
        {
            #region No Record Found - No Report Print
            if (SaleInvoiceRecords.Count == 0)
            {
                Report.Messages.Add($"{DateTime.Now} No Record found to print...");
                return new();
            }

            #endregion

            ReportModel Reportmodel = new();
            string _DataSetName = "ds_SaleInvoice";
            string _ReportFileKey = "sRptName";
            string _Heading1Key = "sRptHeading1";
            string _Heading2Key = "sRptHeading2";
            ReportType _RptType = ReportType.Preview;

            try
            {
                // Generate / Obtain Report Data from Temp Table....
                var _DBFile = UserProfile.DataFile;
                var _Globals = new Globals();
                //var _TempTable = GetText(_DBFile, "stkLedData");
                //var _SourceTable = new DataTable();
                var _ReportFile = GetText(_DBFile, _ReportFileKey);
                //if (_SourceTable == null) { return new(); }

                var _Heading1 = GetText(_DBFile, _Heading1Key);
                var _Heading2 = GetText(_DBFile, _Heading2Key);

                // Input Parameters  (.rdl report file)
                Reportmodel.InputReport.FilePath = _Globals.ReportPath;
                Reportmodel.InputReport.FileName = _ReportFile;
                Reportmodel.InputReport.FileExtention = "rdl";
                // output Parameters (like pdf, excel, word, html, tiff)
                Reportmodel.OutputReport.FilePath = _Globals.PDFPath;
                Reportmodel.OutputReport.FileLink = _Globals.PDFPath;
                Reportmodel.OutputReport.FileName = _ReportFile;
                Reportmodel.OutputReport.ReportType = _RptType;
                // Reports Parameters
                Reportmodel.AddReportParameter("CompanyName", UserProfile.Company);
                Reportmodel.AddReportParameter("Heading1", _Heading1);
                Reportmodel.AddReportParameter("Heading2", _Heading2);
                Reportmodel.AddReportParameter("Footer", "Power by Applied Software House");

                Reportmodel.ReportData.DataSetName = _DataSetName;
                //Reportmodel.ReportData.ReportTable = _SourceTable; // Data Filter will apply by registry variables. FYI

                #region Temp
                //if (Reportmodel.ReportRenderAsync().Result)         // Render a report for preview or download...
                //{
                //    if (Reportmodel.OutputReport.ReportType == ReportType.HTML || Reportmodel.OutputReport.ReportType == ReportType.Preview)
                //    {
                //        ReportLink = Reportmodel.OutputReport.GetFileLink();
                //        IsShowPdf = true;
                //        return Page();
                //    }
                //    else
                //    {
                //        var FileName = $"{Reportmodel.OutputReport.FileName}{Reportmodel.OutputReport.FileExtention}";
                //        return File(Reportmodel.ReportBytes, Reportmodel.OutputReport.MimeType, FileName);
                //    }
                //}
                #endregion
            }
            catch (Exception)
            {
                Messages.Add(AppMessages.Enums.Messages.prtReportError);

            }
            return Reportmodel;
        }
        #endregion

        #region Totals
        public void SetTotals()
        {
            Totals = new();
            Totals.NumberFormat = NumberFormat;
            if (SaleInvoiceRecords.Count > 0)
            {
                foreach (var _Record in SaleInvoiceRecords)
                {
                    if (_Record.ID2 >= 0)
                    {
                        Totals.Tot_Qty += _Record.Qty;
                        Totals.Tot_Amount += _Record.Amount;
                        Totals.Tot_TaxAmount += _Record.TaxAmount;
                        Totals.Tot_NetAmount += _Record.NetAmount;
                    }
                }
            }
        }
        #endregion

        #region Validation of Record and DataRow
        public bool Validation(SaleInvoiceRecord _Record)
        {
            var _Validated = true;

            if (_Record.Qty == 0) { Messages.Add(MESSAGES.Row_QtyZero); _Validated = true; }
            if (_Record.Rate == 0) { Messages.Add(MESSAGES.Row_RateZero); _Validated = true; }
            if (_Record.Amount == 0) { Messages.Add(MESSAGES.Row_AmountZero); _Validated = true; }
            if (_Record.TaxAmount == 0) { Messages.Add(MESSAGES.Row_TaxAmountZero); _Validated = true; }
            if (_Record.Company == 0) { Messages.Add(MESSAGES.Row_CompanyIDZero); _Validated = true; }
            if (_Record.Inventory == 0) { Messages.Add(MESSAGES.Row_InventoryIDZero); _Validated = true; }
            if (_Record.TaxID == 0) { Messages.Add(MESSAGES.Row_TaxIDZero); _Validated = true; }
            if (_Record.Project == 0) { Messages.Add(MESSAGES.Row_ProjectIDZero); _Validated = true; }
            if (_Record.Remarks.Length == 0) { Messages.Add(MESSAGES.Row_NoRemarks); _Validated = true; }
            if (_Record.Description.Length == 0) { Messages.Add(MESSAGES.Row_NoDescription); _Validated = true; }

            return _Validated;
        }
        public bool Validation(DataRow _Row)
        {
            // return if row status is deleted.
            if ((int)_Row["ID"] < 0) { return true; }

            var _Validated = true;
            if (_Row.Table.TableName.Equals(Tables.BillReceivable))
            {
                if (_Row is null) { Messages.Add(MESSAGES.RowValueNull); _Validated = false; }
                if (_Row["ID"] == null) { Messages.Add(MESSAGES.ColumnIsNull); _Validated = false; }
                if (_Row["Vou_No"] == null) { Messages.Add(MESSAGES.ColumnIsNull); _Validated = false; }
                if (_Row["Vou_Date"] == null) { Messages.Add(MESSAGES.ColumnIsNull); _Validated = false; }
                if (_Row["Company"] == null) { Messages.Add(MESSAGES.ColumnIsNull); _Validated = false; }
                if (_Row["Employee"] == null) { Messages.Add(MESSAGES.ColumnIsNull); _Validated = false; }
                if (_Row["Inv_No"] == null) { Messages.Add(MESSAGES.ColumnIsNull); _Validated = false; }
                if (_Row["Inv_Date"] == null) { Messages.Add(MESSAGES.ColumnIsNull); _Validated = false; }
                if (_Row["Pay_Date"] == null) { Messages.Add(MESSAGES.ColumnIsNull); _Validated = false; }
                if (_Row["Amount"] == null) { Messages.Add(MESSAGES.ColumnIsNull); _Validated = false; }
                if (_Row["Description"] == null) { Messages.Add(MESSAGES.ColumnIsNull); _Validated = false; }

                if (_Row is null) { Messages.Add(MESSAGES.RowValueNull); _Validated = false; }
                if (_Row["ID"] == DBNull.Value) { Messages.Add(MESSAGES.ColumnDBNullValue); _Validated = false; }
                if (_Row["Vou_No"] == DBNull.Value) { Messages.Add(MESSAGES.ColumnDBNullValue); _Validated = false; }
                if (_Row["Vou_Date"] == DBNull.Value) { Messages.Add(MESSAGES.ColumnDBNullValue); _Validated = false; }
                if (_Row["Company"] == DBNull.Value) { Messages.Add(MESSAGES.ColumnDBNullValue); _Validated = false; }
                if (_Row["Employee"] == DBNull.Value) { Messages.Add(MESSAGES.ColumnDBNullValue); _Validated = false; }
                if (_Row["Inv_No"] == DBNull.Value) { Messages.Add(MESSAGES.ColumnDBNullValue); _Validated = false; }
                if (_Row["Inv_Date"] == DBNull.Value) { Messages.Add(MESSAGES.ColumnDBNullValue); _Validated = false; }
                if (_Row["Pay_Date"] == DBNull.Value) { Messages.Add(MESSAGES.ColumnDBNullValue); _Validated = false; }
                if (_Row["Amount"] == DBNull.Value) { Messages.Add(MESSAGES.ColumnDBNullValue); _Validated = false; }
                if (_Row["Description"] == DBNull.Value) { Messages.Add(MESSAGES.ColumnDBNullValue); _Validated = false; }

                if ((decimal)_Row["Qty"] == 0) { Messages.Add(MESSAGES.Row_QtyZero); _Validated = true; }
                if ((decimal)_Row["Rate"] == 0) { Messages.Add(MESSAGES.Row_RateZero); _Validated = true; }
                if ((decimal)_Row["Amount"] == 0) { Messages.Add(MESSAGES.Row_AmountZero); _Validated = true; }
                if ((decimal)_Row["TaxAmount"] == 0) { Messages.Add(MESSAGES.Row_TaxAmountZero); _Validated = true; }
                if ((decimal)_Row["Company"] == 0) { Messages.Add(MESSAGES.Row_CompanyIDZero); _Validated = true; }
                if ((decimal)_Row["Inventory"] == 0) { Messages.Add(MESSAGES.Row_InventoryIDZero); _Validated = true; }
                if ((decimal)_Row["TaxID"] == 0) { Messages.Add(MESSAGES.Row_TaxIDZero); _Validated = true; }
                if ((decimal)_Row["Project"] == 0) { Messages.Add(MESSAGES.Row_ProjectIDZero); _Validated = true; }
                if (_Row["Remarks"].ToString()?.Length == 0) { Messages.Add(MESSAGES.Row_NoRemarks); _Validated = true; }
                if ((decimal)_Row["Description"] == 0) { Messages.Add(MESSAGES.Row_NoDescription); _Validated = true; }

                if ((DateTime)_Row["Pay_Date"] < (DateTime)_Row["Inv_Date"]) { Messages.Add(MESSAGES.Sale_PayLessInvDate); }

            }

            if (_Row.Table.TableName.Equals(Tables.BillReceivable2))
            {
                if (_Row is null) { Messages.Add(MESSAGES.RowValueNull); _Validated = false; }
                if (_Row["ID"] == null) { Messages.Add(MESSAGES.ColumnIsNull); _Validated = false; }
                if (_Row["Sr_No"] == null) { Messages.Add(MESSAGES.ColumnIsNull); _Validated = false; }
                if (_Row["TranID"] == null) { Messages.Add(MESSAGES.ColumnIsNull); _Validated = false; }
                if (_Row["Inventory"] == null) { Messages.Add(MESSAGES.ColumnIsNull); _Validated = false; }
                if (_Row["Batch"] == null) { Messages.Add(MESSAGES.ColumnIsNull); _Validated = false; }
                if (_Row["Qty"] == null) { Messages.Add(MESSAGES.ColumnIsNull); _Validated = false; }
                if (_Row["Rate"] == null) { Messages.Add(MESSAGES.ColumnIsNull); _Validated = false; }
                if (_Row["Tax"] == null) { Messages.Add(MESSAGES.ColumnIsNull); _Validated = false; }
                if (_Row["TaxRate"] == null) { Messages.Add(MESSAGES.ColumnIsNull); _Validated = false; }
                if (_Row["Description"] == null) { Messages.Add(MESSAGES.ColumnIsNull); _Validated = false; }

                if (_Row["ID"] == DBNull.Value) { Messages.Add(MESSAGES.ColumnDBNullValue); _Validated = false; }
                if (_Row["Sr_No"] == DBNull.Value) { Messages.Add(MESSAGES.ColumnDBNullValue); _Validated = false; }
                if (_Row["TranID"] == DBNull.Value) { Messages.Add(MESSAGES.ColumnDBNullValue); _Validated = false; }
                if (_Row["Inventory"] == DBNull.Value) { Messages.Add(MESSAGES.ColumnDBNullValue); _Validated = false; }
                if (_Row["Batch"] == DBNull.Value) { Messages.Add(MESSAGES.ColumnDBNullValue); _Validated = false; }
                if (_Row["Qty"] == DBNull.Value) { Messages.Add(MESSAGES.ColumnDBNullValue); _Validated = false; }
                if (_Row["Rate"] == DBNull.Value) { Messages.Add(MESSAGES.ColumnDBNullValue); _Validated = false; }
                if (_Row["Tax"] == DBNull.Value) { Messages.Add(MESSAGES.ColumnDBNullValue); _Validated = false; }
                if (_Row["TaxRate"] == DBNull.Value) { Messages.Add(MESSAGES.ColumnDBNullValue); _Validated = false; }
                if (_Row["Description"] == DBNull.Value) { Messages.Add(MESSAGES.ColumnDBNullValue); _Validated = false; }


                if ((int)_Row["TranID"] == 0) { Messages.Add(MESSAGES.ColumnValueZero); _Validated = false; }
                if ((int)_Row["Inventory"] == 0) { Messages.Add(MESSAGES.ColumnValueZero); _Validated = false; }

                var __TaxRate = Source.SeekTaxRate((int)_Row["TaxID"]);
                if (__TaxRate.Equals((decimal)_Row["TaxRate"])) { Messages.Add(MESSAGES.Sale_TaxRateNotMatch); _Validated = false; }

            }

            return _Validated;

        }
        #endregion

        #region Convert decimal to String Text
        public string ToAmount(object _Object)
        {
            return Conversion.ToAmount(_Object, NumberFormat);
        }
        #endregion
    }
    public class SaleInvoiceRecord
    {
        public int ID1 { get; set; }
        public int ID2 { get; set; }
        public string Vou_No { get; set; } = string.Empty;
        public string Ref_No { get; set; } = string.Empty;
        public string Inv_No { get; set; } = string.Empty;
        public int TranID { get; set; }
        public int Sr_No { get; set; }
        public int Inventory { get; set; }
        public int Company { get; set; }
        public int Employee { get; set; }
        public int Project { get; set; }
        public int TaxID { get; set; }


        public string TitleInventory { get; set; } = string.Empty;
        public string TitleCompany { get; set; } = string.Empty;
        public string TitleEmployee { get; set; } = string.Empty;
        public string TitleProject { get; set; } = string.Empty;
        public string TitleTaxID { get; set; } = string.Empty;
        public string TitleUnit { get; set; } = string.Empty;

        public DateTime Vou_Date { get; set; }
        public DateTime Inv_Date { get; set; }
        public DateTime Pay_Date { get; set; }

        public string Remarks { get; set; } = string.Empty;
        public string Comments { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Batch { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;

        public int Unit { get; set; }
        public decimal Qty { get; set; } = 0.00M;
        public decimal Rate { get; set; } = 0.00M;
        public decimal TaxRate { get; set; } = 0.00M;
        public decimal Amount => decimal.Parse((Qty * Rate).ToString());
        public decimal TaxAmount => decimal.Parse((Amount * (TaxRate / 100)).ToString());
        public decimal NetAmount => decimal.Parse((Amount + TaxAmount).ToString());

    }
    public class SaleInvoiceTotal
    {
        public string NumberFormat { get; set; } = Format.Digit;
        public decimal Tot_Qty { get; set; } = 0.00M;
        public decimal Tot_Amount { get; set; } = 0.00M;
        public decimal Tot_TaxAmount { get; set; } = 0.00M;
        public decimal Tot_NetAmount { get; set; } = 0.00M;
    }


}

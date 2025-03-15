using Microsoft.AspNetCore.Components.Forms;
using System.Data;
using AppliedDB;
using AppliedAccounts.Models;
using AppliedAccounts.Data;
using System.Diagnostics;
using System.Text;
using Microsoft.JSInterop;



namespace AppliedAccounts.Pages.ImportData
{


    public partial class ImportSaleInvoice
    {

        #region Variables
        public AppUserModel AppUser { get; set; }
        public ImportSaleInvoiceModel Model { get; set; }
        public ImportExcelFile ImportExcel { get; set; }
        public StringBuilder MyMessage { get; set; }
        public DataSet? ExcelDataSet { get; set; }
        public DataTable? SalesData { get; set; }
        public DataTable? SalesSchema { get; set; }
        public DataTable? InvData { get; set; }
        public DataTable Sale1 { get; set; }
        public DataTable Sale2 { get; set; }
        public DataSource Source { get; set; }
        public Stopwatch stopwatch { get; set; } = new();


        public bool IsExcelLoaded { get; set; } = false;
        public bool ShowSpinner { get; set; } = false;
        public bool ShowImportButton { get; set; } = true;
        public string ExcelFileName { get; set; } = "";
        public string SpinnerMessage { get; set; } = "";



        DateTime Inv_Date = DateTime.Now;
        DateTime Due_Date = DateTime.Now;
        string RefNo = string.Empty;
        string Batch = string.Empty;
        string Inv_No = string.Empty;
        int TotalRec = 0;

        int Counter = 0;
        int Error = 0;
        int Skip = 0;


        #endregion

        #region Constructor
        //public ImportSaleInvoice() { }

        public ImportSaleInvoice(AppUserModel _AppUser)
        {
            AppUser = _AppUser;
            stopwatch.Start();
            MyMessage = new();
            Model = new();

        }
        #endregion

        #region Get Excel file from Import Model, 
        // The Excel File save on server and
        // open from Server
        // Create a Temp Database Table and move all data to this temp SQLite Database File.
        public async Task GetExcelFile(InputFileChangeEventArgs e)
        {
            try
            {
                SpinnerMessage = "Excel file is being loaded.  Wait for some while";
                ShowSpinner = true;
                ExcelFileName = e.File.Name;
                StateHasChanged();


                ImportExcel = new(e.File, AppUser);
                await ImportExcel.ImportDataAsync();
                IsExcelLoaded = true;
                MyMessage.AppendLine($"{DateTime.Now} Excel File loaded.... OK");
            }
            catch (Exception)
            {

                MyMessage.AppendLine($"{DateTime.Now} ERROR: Excel file not loaded.... ");
            }
            finally
            {
                ShowSpinner = false;
                StateHasChanged();
            }

        }
        #endregion

        #region Import Data main method

        public async Task GetImportedDataAsync()
        {
            if (IsExcelLoaded)
            {
                SpinnerMessage = "Sales invoice data is being Process...";

                ShowSpinner = true;
                ShowImportButton = false;
                stopwatch.Restart(); // Restart instead of Start to reset time

                await InvokeAsync(StateHasChanged);

                try
                {
                    await GetExcelSheetDataAsync(); // Ensure this is awaited if async
                    await GetDataTableAsync(); // Fixed method name
                }
                finally
                {
                    stopwatch.Stop();
                    var ts = stopwatch.Elapsed;
                    MyMessage.AppendLine($"{DateTime.Now} Total time spent in process: {ts.TotalSeconds} seconds");

                    ShowSpinner = false;
                    Model.ShowData = true;

                    await InvokeAsync(StateHasChanged);
                }
            }
        }


        private async Task GetExcelSheetDataAsync()
        {
            SpinnerMessage = "Sales invoice data is being Process... Gathering Data sheets";
            string _TempGUID = AppRegistry.GetText(AppUser.DataFile, "ExcelImport");
            TempDB _TempDB = new(_TempGUID + ".db");
            SalesData = await _TempDB.GetTempTableAsync("Data");
            SalesSchema = await _TempDB.GetTempTableAsync("Schema");
            InvData = await _TempDB.GetTempTableAsync("Invoice Data");
        }
        #endregion

        #region Get Data From SQLite Temp Data, 
        //Stored data from excel file to Temp
        // and here this data will call in a app as Data tables.
        public async Task GetDataTableAsync()
        {
            if (SalesData != null && SalesSchema != null && InvData != null)
            {
                ImportSaleInvoiceModel _Result = await GetDataTablesAsync();
                Model = _Result;
            }
        }




        public async Task<ImportSaleInvoiceModel> GetDataTablesAsync()
        {
            SpinnerMessage = "Sales invoice data is being Process... Gatting Data table";
            ImportSaleInvoiceModel _Result = new();
            _Result.DBFile = AppUser.DataFile;
            await GenerateInvoice();
            MyMessage.AppendLine($"{DateTime.Now} Task Completed.");

            Model.ShowData = true;

            if (Sale1 != null)
            {
                foreach (DataRow Row in Sale1.Rows)
                {
                    _Result.SaleInvoiceList.Add(Row);
                }

                if (Sale2 != null)
                {
                    foreach (DataRow Row in Sale2.Rows)
                    {
                        _Result.SaleDetailsList.Add(Row);
                    }
                }

            }
            return _Result;

        }

        #endregion

        #region Generate Invoice Master Table
        public async Task GenerateInvoice()
        {
            await Task.Run(() =>
            {
                SpinnerMessage = "Sales invoice data is being Process... Generating Invoices !!";
                MyMessage.AppendLine($"{DateTime.Now} Start Process for Generate Invoice");
                #region Error Message
                if (InvData is null || SalesData is null || SalesSchema is null)
                {
                    MyMessage.AppendLine($"{DateTime.Now} Date is not available to proceed...");
                    return;
                }
                #endregion

                Sale1 = DataSource.CloneTable(AppUser.DataFile, Enums.Tables.BillReceivable);
                Sale2 = DataSource.CloneTable(AppUser.DataFile, Enums.Tables.BillReceivable2);

                TotalRec = SalesData.Rows.Count;

                GetInvoiceData();          // Gather data from excel sheet schema for creating voucher Data parameters.
                MyMessage.Append("");


                Model.ShowData = true;
                CommandClass _CommandClass = new CommandClass();
                foreach (DataRow Row in SalesData.Rows)                 // Loop main sale invocies records. per record per invoice.
                {
                    SpinnerMessage = $"Sales invoice data is being Process... Generating Invoices {Counter}";
                    Counter++;

                    if (string.IsNullOrEmpty(Row["Code"].ToString())) { Error++; continue; }
                    if ((string)Row["Active"] != "1") { Skip++; continue; }
                    if (Counter == 1) { continue; }     // Skip record of Heading in Excel File. 


                    DataRow _Row1 = Sale1.NewRow();
                    int _CompanyID = Functions.Code2Int(AppUser.DataFile, Enums.Tables.Customers, (string)Row["Code"]);
                    int _EmployeeID = Functions.Code2Int(AppUser.DataFile, Enums.Tables.Employees, (string)Row["Employee"]);
                    decimal _Total = Conversion.ToDecimal(Row["Total"]);

                    if (_Total > 0)     // if Invoice amount is zero, skip this...
                    {
                        _Row1["ID"] = Counter;
                        _Row1["Vou_No"] = string.Concat(Batch, Counter.ToString("0000"));
                        _Row1["Vou_Date"] = Inv_Date;
                        _Row1["Company"] = _CompanyID;
                        _Row1["Employee"] = _EmployeeID;
                        _Row1["Ref_No"] = RefNo;
                        _Row1["Inv_No"] = string.Concat(Inv_No, Counter.ToString("0000")); ;
                        _Row1["Inv_Date"] = Inv_Date;
                        _Row1["Pay_Date"] = Due_Date;
                        _Row1["Amount"] = _Total;
                        _Row1["Description"] = Row["CompanyName"];
                        _Row1["Comments"] = $"Sale Invoice : {Row["CompanyName"]} for amount {_Total}"; ;
                        _Row1["Status"] = "Submitted";

                        Sale1.Rows.Add(_Row1);
                        Model.SaleInvoiceList.Add(_Row1);
                        Model.ShowData = false;

                        MyMessage.AppendLine($" # {Counter}");

                        GetInvoiceDetails(Row);                 // Generates detail record of sale invocies.
                    }
                    else
                    {
                        Model.RejectedList.Add(Row);
                    }
                }
            });
            MyMessage.AppendLine($"{DateTime.Now} End Sales Invoice details process..");
        }
        #endregion

        #region Generate Sales Invoice Details Table
        public void GetInvoiceDetails(DataRow Row)
        {
            MyMessage.AppendLine($"{DateTime.Now} Working of Sale Invoice details.");
            var Counter2 = 0;
            var Sr_No = 0;
            if (SalesSchema is not null)
            {
                foreach (DataRow Scheme in SalesSchema.Rows)
                {

                    if ((string)Scheme["Code"] != "0")
                    {
                        decimal _Amount = Conversion.ToDecimal(Row[(string)Scheme["Amount"]]);
                        if (_Amount == 0) { continue; }          // Skip if amount is zero.

                        Counter2++; Sr_No++;
                        DataRow _Row2 = Sale2.NewRow();
                        _Row2["ID"] = Counter2;
                        _Row2["Sr_No"] = Sr_No;
                        _Row2["TranID"] = Counter;
                        _Row2["Batch"] = Row["Batch"];

                        int _Inventory = Functions.Code2Int(AppUser.DataFile, Enums.Tables.Inventory, (string)Scheme["Code"]);
                        _Row2["Inventory"] = _Inventory;
                        _Row2["Batch"] = Row["Batch"];

                        if ((string)Scheme["Entry ID"] == "A")          // Amount only
                        {
                            _Row2["Qty"] = 1;
                            _Row2["Rate"] = _Amount;
                        }
                        if ((string)Scheme["Entry ID"] == "Q")          // Quantity and Rate = Amount
                        {
                            _Row2["Qty"] = Conversion.ToDecimal(Row[(string)Scheme["Qty"]]);
                            _Row2["Rate"] = Conversion.ToDecimal(Row[(string)Scheme["Rate"]]);
                        }

                        decimal _TaxID = Functions.Code2Int(AppUser.DataFile, Enums.Tables.Taxes, (string)Scheme["STax"]);
                        _Row2["Tax"] = _TaxID;
                        _Row2["Tax_Rate"] = Functions.Code2Rate(AppUser.DataFile, (int)_Row2["Tax"]);
                        _Row2["Description"] = Row[(string)Scheme["Remarks Code"]];
                        int.TryParse(Row["Project"].ToString(), out int _projectID);
                        _Row2["Project"] = _projectID;

                        Sale2.Rows.Add(_Row2);
                        Model.SaleDetailsList.Add(_Row2);
                    }
                }
            }

            var Stop = true;
        }
        #endregion

        #region Get Invoice Data from Excel file
        // Like Invoice Date, due Date, Invoice No pattern.
        // 
        public void GetInvoiceData()
        {
            MyMessage.AppendLine($"{DateTime.Now} Gathering Invoice Data");
            #region Get Invoice Date

            foreach (DataRow Row in InvData.Rows)
            {
                if (Row["Particular"].ToString() == "Invoice Date")
                {
                    Inv_Date = Conversion.ToDateTime((string)Row["Value"]);
                }
                if (Row["Particular"].ToString() == "Invoice No")
                {
                    Inv_No = (string)Row["Value"];
                }

                if (Row["Particular"].ToString() == "Due Date")
                {
                    Due_Date = Conversion.ToDateTime((string)Row["Value"]);
                }
                if (Row["Particular"].ToString() == "Batch") { Batch = (string)Row["Value"]; }
                if (Row["Particular"].ToString() == "RefNo") { RefNo = (string)Row["Value"]; }
            }
            #endregion


        }
        #endregion

        #region Dispaly invocie in Modol...  Pending.  through View button in <Table> Tag.
        private async Task ShowInvoiceModol(int ID)
        {

            Model.GetSelectedSale(ID);
            ClientName = Model.SelectedSale.First().Company;
            Voucher = $"{Model.SelectedSale.First().Vou_No} : {Model.SelectedSale.First().Vou_Date.ToString(Format.DDMMYY)}";

            Totals = new();
            foreach (var item in Model.SelectedSale)
            {
                Totals.Tot_Qty += item.Qty;
                Totals.Tot_Amount += item.Amount;
                Totals.Tot_Tax += item.TaxAmount;
                Totals.Tot_Net += item.NetAmount;
            }

            await js.InvokeVoidAsync("showModol", "ModolSaleInvoice");

        }
        #endregion

        #region Post / Save invoices to Database

        private async void Post()
        {
            ShowSpinner = true;
            await SaveAsync();
            ShowSpinner = false;
            Model.ShowData = false;
            Model.ShowMessages = true;
            await InvokeAsync(StateHasChanged);
        }

        private async Task SaveAsync()
        {
            MyMessage.Clear();
            Source = new(AppUser);
            var BillRec1 = Source.GetTable(Enums.Tables.BillReceivable);
            var BillRec2 = Source.GetTable(Enums.Tables.BillReceivable2);
            var master = BillRec1.NewRow();
            var details = new List<DataRow>();
            var Validated = true;

            foreach (var Invoice in Model.SaleInvoiceList)
            {
                MyMessage.AppendLine($"{DateTime.Now} Invoice {master["Vou_No"]} is processing...");
                master = Invoice;
                details = Model.SaleDetailsList.Where(row => (int)row["TranID"] == (int)master["ID"]).ToList();

                MyMessage.AppendLine($"{DateTime.Now} {details.Count} records found in invoice {master["Vou_No"]}");
                Validated = Validation(master);
                MyMessage.AppendLine($"{DateTime.Now} {master["Vou_No"]} is {Validated} validated");

                foreach (var Row in details)
                {

                    Validated = Validation(Row); ;
                    if (!Validated)
                    {
                        break;
                    }
                    MyMessage.AppendLine($"{DateTime.Now} {master["Vou_No"]} Serial # {Row["Sr_No"]}  is {Validated} validated");
                }

                if (Validated)
                {
                    MyMessage.AppendLine($"{DateTime.Now} {master["Vou_No"]} validated for post / save... ");
                    master["ID"] = 0;           // Set a Datarow for insert command
                    CommandClass _Commands = new(master, AppUser.DataFile);
                    var IsSaved = _Commands.SaveChanges();
                    MyMessage.AppendLine($"{DateTime.Now} {master["Vou_No"]} saved ---> {IsSaved} ");
                    var _TranID = (int)master["ID"];        // Get ID after save row in SQLite Data Table.
                    foreach (var Row in details)
                    {
                        Row["ID"] = 0;
                        Row["TranID"] = _TranID;
                        _Commands = new(Row, AppUser.DataFile);
                        await Task.Run(() => _Commands.SaveChanges());
                        MyMessage.AppendLine($"{DateTime.Now} {master["Vou_No"]} Serial # {Row["Sr_No"]} is saved ---> {IsSaved} ");
                    }
                }
                else
                {
                    MyMessage.AppendLine($"{DateTime.Now} ERROR : Sales Date is not valided to post...");
                }
            }
        }
        #endregion

        #region Validation of Sale Invoice Record ---- Pending
        private bool Validation(DataRow _Row)
        {
            return true;
        }
        #endregion

        #region show Invoices
        private void ShowInvoices()
        {
            NavManager.NavigateTo("/Sale/SaleInvoiceList");
        }
        #endregion
    }

    #region Model Class
    public class ImportSaleInvoiceModel
    {
        //public AppUserModel AppUser { get; set; }
        public DataRow SaleMaster { get; set; }
        public DataRow SaleDetails { get; set; }
        public string DBFile { get; set; }

        public List<DataRow> SaleInvoiceList { get; set; } = new();
        public List<DataRow> RejectedList { get; set; } = new();
        public List<DataRow> SaleDetailsList { get; set; } = new();
        public List<SelectedSaleInvoice> SelectedSale { get; set; } = new();

        public bool ShowData { get; set; } = false;
        public bool ShowMessages { get; set; } = false;
        public bool PostData { get; set; } = false;

        public ImportSaleInvoiceModel() { }
        public ImportSaleInvoiceModel(DataTable _SalesTable1, DataTable _SaleTable2)
        {
            SaleMaster = _SalesTable1.NewRow();
            SaleDetails = _SaleTable2.NewRow();
        }

        public List<DataRow> GetDetail(int _ID)
        {
            return SaleDetailsList.Where(a => (int)a["ID"] == _ID).ToList();
        }

        public void GetSelectedSale(int TranID)
        {
            var _master = SaleInvoiceList.Where(Row => (int)Row["ID"] == TranID).First();
            var _detail = SaleDetailsList.Where(Row => (int)Row["TranID"] == TranID).ToList();

            if (_detail.Count > 0)
            {
                SelectedSale.Clear();
                foreach (var Item in _detail)
                {
                    decimal _TaxRate = Functions.GetTaxRate(DBFile, (int)Item["Tax"]);
                    SelectedSaleInvoice _SaleInvoice = new(); ;
                    _SaleInvoice.Company = (string)_master["Description"];   //Functions.GetTitle(DBFile, Enums.Tables.Customers, (int)_master["Company"]);
                    _SaleInvoice.Vou_No = (string)_master["Vou_No"];
                    _SaleInvoice.Vou_Date = (DateTime)_master["Vou_Date"];
                    _SaleInvoice.Inventory = Functions.GetTitle(DBFile, Enums.Tables.Inventory, (int)Item["Inventory"]);
                    _SaleInvoice.Batch = (string)Item["Batch"];
                    _SaleInvoice.Qty = (decimal)Item["Qty"];
                    _SaleInvoice.Rate = (decimal)Item["Rate"];
                    _SaleInvoice.Amount = _SaleInvoice.Qty * _SaleInvoice.Rate;
                    _SaleInvoice.TaxAmount = _SaleInvoice.Amount * (_TaxRate / 100);
                    _SaleInvoice.NetAmount = _SaleInvoice.Amount + _SaleInvoice.TaxAmount;

                    SelectedSale.Add(_SaleInvoice);
                }
            }
        }
    }

    public class SelectedSaleInvoice
    {
        public string Company { get; set; } = string.Empty;
        public string Vou_No { get; set; } = string.Empty;
        public DateTime Vou_Date { get; set; }
        public string Inventory { get; set; } = string.Empty;
        public string Batch { get; set; } = string.Empty;
        public decimal Rate { get; set; }
        public decimal Qty { get; set; }
        public decimal Amount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal NetAmount { get; set; }
    }
    #endregion
}

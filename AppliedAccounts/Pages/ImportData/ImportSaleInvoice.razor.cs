using Microsoft.AspNetCore.Components.Forms;
using System.Data;
using AppliedDB;
using AppliedAccounts.Models;
using AppliedAccounts.Data;
using System.Diagnostics;
using Microsoft.JSInterop;
using AppMessages;
using Tables = AppliedDB.Enums.Tables;
using AppliedAccounts.Services;

namespace AppliedAccounts.Pages.ImportData
{
    public partial class ImportSaleInvoice
    {

        #region Variables
        public AppUserModel AppUser { get; set; }
        public ImportSaleInvoiceModel MyModel { get; set; }
        public ImportExcelFile ImportExcel { get; set; }
        public MessageClass MsgClass { get; set; }
        public DataSet? ExcelDataSet { get; set; }
        public DataTable? ClientData { get; private set; }
        public DataTable? SalesData { get; set; }
        public DataTable? SalesSchema { get; set; }
        public DataTable? InvData { get; set; }
        public DataTable Sale1 { get; set; }
        public DataTable Sale2 { get; set; }
        public DataSource Source { get; set; }
        public Stopwatch Stopwatch { get; set; } = new();
        
        public bool IsExcelLoaded { get; set; } = false;
        public bool ShowSpinner { get; set; } = false;
        public bool ShowImportButton { get; set; } = true;
        public string ExcelFileName { get; set; } = "";
        
        
        
        DateTime Inv_Date = DateTime.Now;
        DateTime Due_Date = DateTime.Now;
        string RefNo = string.Empty;
        string Batch = string.Empty;
        string Inv_No = string.Empty;

        int Error = 0;
        int Skip = 0;


        #endregion

        #region Constructor
        //public ImportSaleInvoice() { }

        public ImportSaleInvoice(AppUserModel _AppUser)
        {
            MyModel = new();
            AppUser = _AppUser;
            MyModel.IsClientUpdate = true;             // true if client info update in DB


        }
        #endregion

        #region Upload Excel file. 
        // The Excel File save on server and
        // open from Server
        // Create a Temp Database Table and move all data to this temp SQLite Database File.
        public async Task GetExcelFile(InputFileChangeEventArgs e)
        {
            try
            {
                MyModel.SpinnerMessage = "Excel file is being loaded.  Wait for some while";
                ShowSpinner = true;
                ExcelFileName = e.File.Name;
                await InvokeAsync(StateHasChanged);


                ImportExcel = new(e.File, AppUser);
                await ImportExcel.ImportDataAsync();            // ImportExcelFile.cs Function
                IsExcelLoaded = true;                           // Excel file has been loaded successfully.
                MsgClass.Add($"{DateTime.Now} Excel File loaded.... OK");
            }
            catch (Exception)
            {
                MyModel.IsError = true;
                MsgClass.Add($"{DateTime.Now} ERROR: Excel file not loaded.... ");
            }
            finally
            {
                ShowSpinner = false;
                await InvokeAsync(StateHasChanged);
            }

        }
        #endregion

        #region Import Data main method

        public async Task GetImportDataAsync()
        {
            if (IsExcelLoaded)
            {
                MyModel.SpinnerMessage = "Sales invoice data is being Process...";

                ShowSpinner = true;
                ShowImportButton = false;
                Stopwatch.Restart(); // Restart instead of Start to reset time

                await InvokeAsync(StateHasChanged);

                try
                {
                    if (!MyModel.IsError)
                    {
                        await GetExcelSheetDataAsync(); // Ensure this is awaited if async
                        await InvokeAsync(StateHasChanged);
                    }

                    if (MyModel.IsClientUpdate)
                    {
                        if (!MyModel.IsError)
                        {
                            await UpdateClientListAsync();  // Ensure all the client has been update in DB.
                            await InvokeAsync(StateHasChanged);
                        }
                    }

                    if (!MyModel.IsError)
                    {
                        await GenerateSalesInvoiceAsync();      // Fixed method name
                        await InvokeAsync(StateHasChanged);
                    }
                }
                finally
                {
                    Stopwatch.Stop();
                    var ts = Stopwatch.Elapsed;
                    MsgClass.Add($"{DateTime.Now} Total time spent in process: {ts.TotalSeconds} seconds");

                    ShowSpinner = false;
                    MyModel.ShowImportedData = true;     // display all imported sales invoices after process complete

                    await InvokeAsync(StateHasChanged);
                }
            }
        }
        #endregion

        //Sep 1
        #region Get from Temp SQLiet DB file  to Data Tables
        private async Task GetExcelSheetDataAsync()
        {
            MyModel.SpinnerMessage = "Sales invoice data is being Process... Gathering Data sheets";
            string _TempGUID = AppRegistry.GetText(AppUser.DataFile, "ExcelImport");
            TempDB _TempDB = new(_TempGUID + ".db");
            ClientData = await _TempDB.GetTempTableAsync("Clients List");
            SalesData = await _TempDB.GetTempTableAsync("Data");
            SalesSchema = await _TempDB.GetTempTableAsync("Schema");
            InvData = await _TempDB.GetTempTableAsync("Invoice Data");


            // Validate the Batch is exist in Data Table Bill Receivable
            MyModel.IsError = await BatchValidaed();
            await InvokeAsync(StateHasChanged);

        }

        // Check the Batch is already exist in Bill Receivable table.
        private async Task<bool> BatchValidaed()
        {
            var ExcelBatch = string.Empty;
            var TableBatch = string.Empty;

            if (InvData != null && InvData.Rows.Count > 0)
            {
                ExcelBatch = InvData.AsEnumerable().ToList()
                    .Where(row => row.Field<string>("Particular") == "Batch")
                    .Select(row => row.Field<string>("Value")).First() ?? "";
                if (ExcelBatch.Any()) { Batch = ExcelBatch; }
            }

            var _Text = $"SELECT DISTINCT [Ref_No] FROM [BillReceivable] WHERE [Ref_No] = '{ExcelBatch}'";
            using var _Table = await Task.Run(() => Source.GetTable(_Text));
            if (_Table.Rows.Count > 0)
            {

                MyModel.ErrorMessage = $"Batch {ExcelBatch} number is already exist in Sale Invoice. Assign an unique number";
                return true;   // pass value to IsError; Error found.
            }
            return false;      // No error found.   
        }
        #endregion

        //Step 2
        #region Update Client List
        private async Task UpdateClientListAsync()
        {
            var Log = new Dictionary<string, bool>();
            MyModel.IsProgressBar = true;

            try
            {
                if (ClientData != null && Source != null)
                {
                    MyModel.SpinnerMessage = "Sales invoice data is being Process... Customer Data Updating..";
                    var tb_Client = Source.GetTable(Tables.Customers);
                    var tb_ClientList = tb_Client.AsEnumerable().ToList();
                    var ExcelColumn = "BP Name";
                    var DataColumn = "Title";

                    MyModel.TotalRec = ClientData.Rows.Count;
                    MyModel.Counter = 0;

                    foreach (DataRow Row in ClientData.Rows)
                    {

                        var _Title = Row[ExcelColumn].ToString()?.Trim() ?? "";
                        var _RowID = tb_ClientList.Where(row => _Title == row.Field<string>(DataColumn)).Select(row => row.Field<int>("ID")).FirstOrDefault();

                        if (_RowID == 0)
                        {

                            Log.Add(_Title, false);
                        }
                        else
                        {
                            Log.Add(_Title, true);
                        }


                        // Process Bar Claculation
                        MyModel.Counter++;
                        double _Counter = double.Parse(MyModel.Counter.ToString());
                        double _TotalRec = double.Parse(MyModel.TotalRec.ToString());
                        MyModel.BarPercent = Math.Round((_Counter / _TotalRec) * 100, 2);
                        await UpdateClient(Row, _RowID);
                        await InvokeAsync(StateHasChanged);
                    }

                }
            }
            catch (Exception error)
            {
                MyModel.IsError = true;
                MyModel.ErrorMessage = error.Message;
                MsgClass.Add(error.Message);
            }

        }

        private async Task UpdateClient(DataRow _Row, int _RowID)
        {
            try
            {
                var _ClientRow = Source.GetNewRow(Tables.Customers);
                _ClientRow["ID"] = _RowID;
                _ClientRow["Code"] = _Row["BP Code"];
                _ClientRow["Title"] = _Row["BP Name"];
                _ClientRow["NTN"] = _Row["NTN"];
                _ClientRow["ContactTo"] = _Row["Contact Person"];
                _ClientRow["Email"] = _Row["Email Address"];
                _ClientRow["Address1"] = _Row["Address Name 2"];
                _ClientRow["Address2"] = _Row["Address Name 3"];
                _ClientRow["Address3"] = _Row["Street"];
                _ClientRow["City"] = _Row["City"];
                _ClientRow["Status"] = 1;

                CommandClass Commands = new(_ClientRow, Source.DBFile);
                await Task.Run(() =>
                {
                    Commands.SaveChanges();
                    MyModel.SpinnerMessage = $"{_ClientRow["Title"]} is being updated..";
                });

            }
            catch (Exception error)
            {
                MsgClass.Add(error.Message);
            }



        }
        #endregion


        // Step 3

        #region Get Sales Invocies from Data tables of Temp DB, 

        public async Task<ImportSaleInvoiceModel> GenerateSalesInvoiceAsync()
        {
            MyModel.SpinnerMessage = "Sales invoice data is being Process... Gatting Data table";
            ImportSaleInvoiceModel _Result = new();
            _Result.DBFile = AppUser.DataFile;
            MyModel.IsProgressBar = true;
            MyModel.Counter = 0;
            await GenerateInvoice();
            await InvokeAsync(StateHasChanged);

            MsgClass.Add($"{DateTime.Now} Task Completed.");

            MyModel.ShowImportedData = true;

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
            MyModel = _Result;
            return _Result;

        }

        #endregion

        #region Generate Invoice Master Table
        public async Task GenerateInvoice()
        {

            MsgClass.Add($"{DateTime.Now} Start Process for Generate Invoice");
            #region Error Message
            if (InvData is null || SalesData is null || SalesSchema is null)
            {
                MsgClass.Add($"{DateTime.Now} Date is not available to proceed...");
                return;
            }
            #endregion

            Sale1 = DataSource.CloneTable(AppUser.DataFile, Tables.BillReceivable);
            Sale2 = DataSource.CloneTable(AppUser.DataFile, Tables.BillReceivable2);

            MyModel.TotalRec = SalesData.Rows.Count;

            GetInvoiceData();          // Gather data from excel sheet schema for creating voucher Data parameters.
            MsgClass.Add($"{DateTime.Now} GetInvoiceData() Completed");

            MyModel.ShowImportedData = true;
            MyModel.IsProgressBar = true;
            CommandClass _CommandClass = new CommandClass();
            foreach (DataRow Row in SalesData.Rows)                 // Loop main sale invocies records. per record per invoice.
            {
                //SpinnerMessage = $"Sales invoice data is being Process... Generating Invoices {Counter}";
                MyModel.Counter++;
                double _Counter = double.Parse(MyModel.Counter.ToString());
                double _TotalRec = double.Parse(MyModel.TotalRec.ToString());
                MyModel.BarPercent = Math.Round((_Counter / _TotalRec) * 100, 2);

                if (string.IsNullOrEmpty(Row["Code"].ToString())) { Error++; continue; }
                if ((string)Row["Active"] != "1") { Skip++; continue; }
                if (MyModel.Counter == 1) { continue; }     // Skip record of Heading in Excel File. 


                DataRow _Row1 = Sale1.NewRow();
                int _CompanyID = AppliedDB.Functions.Code2Int(AppUser.DataFile, Tables.Customers, (string)Row["Code"]);
                int _EmployeeID = AppliedDB.Functions.Code2Int(AppUser.DataFile, Tables.Employees, (string)Row["Employee"]);
                decimal _Total = Conversion.ToDecimal(Row["Total"]);

                if (_Total > 0)     // if Invoice amount is zero, skip this...
                {
                    _Row1["ID"] = MyModel.Counter;
                    _Row1["Vou_No"] = string.Concat(Batch, MyModel.Counter.ToString("0000"));
                    _Row1["Vou_Date"] = Inv_Date;
                    _Row1["Company"] = _CompanyID;
                    _Row1["Employee"] = _EmployeeID;
                    _Row1["Ref_No"] = Batch;                // Ref No. will be save as batch
                    _Row1["Inv_No"] = string.Concat(Inv_No, MyModel.Counter.ToString("0000")); ;
                    _Row1["Inv_Date"] = Inv_Date;
                    _Row1["Pay_Date"] = Due_Date;
                    _Row1["Amount"] = _Total;
                    _Row1["Description"] = Row["CompanyName"];
                    _Row1["Comments"] = $"Sale Invoice : {Row["CompanyName"]} for amount {_Total}"; ;
                    _Row1["Status"] = "Submitted";

                    Sale1.Rows.Add(_Row1);
                    MyModel.SaleInvoiceList.Add(_Row1);
                    MyModel.ShowImportedData = false;

                    MsgClass.Add($" # {MyModel.Counter}");

                    MyModel.SpinnerMessage = $"{Row["CompanyName"]} is being generated.";
                    await GetInvoiceDetails(Row);   // Generates detail record of sale invocies.
                    await InvokeAsync(StateHasChanged);
                }
                else
                {
                    MyModel.RejectedList.Add(Row);
                }
            }
            MsgClass.Add($"{DateTime.Now} End Sales Invoice details process..");
        }
        #endregion

        #region Generate Sales Invoice Details Table
        public async Task GetInvoiceDetails(DataRow Row)
        {
            await Task.Run(() =>
            {
                MsgClass.Add($"{DateTime.Now} Working of Sale Invoice details.");
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
                            _Row2["TranID"] = MyModel.Counter;
                            _Row2["Batch"] = Batch;

                            int _Inventory = AppliedDB.Functions.Code2Int(AppUser.DataFile, Tables.Inventory, (string)Scheme["Code"]);
                            _Row2["Inventory"] = _Inventory;
                            _Row2["Batch"] = Batch;

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

                            decimal _TaxID = AppliedDB.Functions.Code2Int(AppUser.DataFile, Tables.Taxes, (string)Scheme["STax"]);
                            _Row2["Tax"] = _TaxID;
                            _Row2["Tax_Rate"] = AppliedDB.Functions.Code2Rate(AppUser.DataFile, (int)_Row2["Tax"]);
                            _Row2["Description"] = Row[(string)Scheme["Remarks Code"]];
                            int.TryParse(Row["Project"].ToString(), out int _projectID);
                            _Row2["Project"] = _projectID;

                            Sale2.Rows.Add(_Row2);
                            MyModel.SaleDetailsList.Add(_Row2);
                        }
                    }
                }
            });
            var Stop = true;
        }
        #endregion

        #region Get Invoice Data from Excel file
        // Like Invoice Date, due Date, Invoice No pattern.
        // 
        public void GetInvoiceData()
        {
            MsgClass.Add($"{DateTime.Now} Gathering Invoice Data");
            #region Get Invoice Date

            if (InvData != null)
            {
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
            }
            #endregion


        }
        #endregion

        #region Dispaly invocie in Modol...  Pending.  through View button in <Table> Tag.
        private async Task ShowInvoiceModol(int ID)
        {

            MyModel.GetSelectedSale(ID);
            ClientName = MyModel.SelectedSale.First().Company;
            Voucher = $"{MyModel.SelectedSale.First().Vou_No} : {MyModel.SelectedSale.First().Vou_Date.ToString(Format.DDMMYY)}";

            Totals = new();
            foreach (var item in MyModel.SelectedSale)
            {
                Totals.Tot_Qty += item.Qty;
                Totals.Tot_Amount += item.Amount;
                Totals.Tot_Tax += item.TaxAmount;
                Totals.Tot_Net += item.NetAmount;
            }

            await js.InvokeVoidAsync("showModol", "ModolSaleInvoice");

        }
        #endregion

        // Step 4.  Posting of Sales Invoices generated by Process

        #region Post / Save invoices to Database
        
        private async Task Post()
        {
            MyModel.IsProgressBar = true;
            MyModel.SpinnerMessage = "Sale invoices are being posted in General Ledger";
            ShowSpinner = true;
            await InvokeAsync(StateHasChanged);
            
            await SaveAsync();
            ShowSpinner = false;
            MyModel.ShowImportedData = false;
            MyModel.ShowMessages = true;
            await InvokeAsync(StateHasChanged);
        }

        

        private async Task SaveAsync()
        {
            MsgClass = new();
            Source = new(AppUser);
            var BillRec1 = Source.GetTable(Tables.BillReceivable);
            var BillRec2 = Source.GetTable(Tables.BillReceivable2);
            var master = BillRec1.NewRow();
            var details = new List<DataRow>();
            var Validated = true;

            foreach (var Invoice in MyModel.SaleInvoiceList)
            {
                MsgClass.Add($"{DateTime.Now} Invoice {master["Vou_No"]} is processing...");
                master = Invoice;
                details = MyModel.SaleDetailsList.Where(row => (int)row["TranID"] == (int)master["ID"]).ToList();

                MsgClass.Add($"{DateTime.Now} {details.Count} records found in invoice {master["Vou_No"]}");
                Validated = Validation(master);
                MsgClass.Add($"{DateTime.Now} {master["Vou_No"]} is {Validated} validated");

                foreach (var Row in details)
                {

                    Validated = Validation(Row); ;
                    if (!Validated)
                    {
                        break;
                    }
                    MsgClass.Add($"{DateTime.Now} {master["Vou_No"]} Serial # {Row["Sr_No"]}  is {Validated} validated");
                }



                if (Validated)
                {
                    MsgClass.Add($"{DateTime.Now} {master["Vou_No"]} validated for post / save... ");
                    master["ID"] = 0;           // Set a Datarow for insert command
                    CommandClass _Commands = new(master, AppUser.DataFile);
                    var IsSaved = _Commands.SaveChanges();
                    MsgClass.Add($"{DateTime.Now} {master["Vou_No"]} saved ---> {IsSaved} ");
                    var _TranID = (int)master["ID"];        // Get ID after save row in SQLite Data Table.
                    foreach (var Row in details)
                    {
                        Row["ID"] = 0;
                        Row["TranID"] = _TranID;
                        _Commands = new(Row, AppUser.DataFile);
                        await Task.Run(() =>
                        {
                            MyModel.SpinnerMessage = $"{master["Vou_No"]} is being saved.";
                            _Commands.SaveChanges();
                        });

                        MsgClass.Add($"{DateTime.Now} Serial # {Row["Sr_No"]} is saved ---> {IsSaved} ");

                        ToastService.ShowToast(ToastClass.SaveToast, $"Save | {master["Vou_No"]}"); // show the toast
                    }
                }
                else
                {
                    MsgClass.Add($"{DateTime.Now} ERROR : Sales Date is not valided to post...");
                }
            }
            
        }
        #endregion

        #region Validation of Sale Invoice Record ---- Pending
        private bool Validation(DataRow _Row)
        {
            if ( _Row == null ) {  return false; }
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

        public bool ShowImportedData { get; set; } = false;
        public bool ShowMessages { get; set; } = false;
        public bool PostData { get; set; } = false;
        public bool IsSpinner { get; set; } = false;
        public string SpinnerMessage { get; set; } = string.Empty;
        public bool IsError { get; set; } = false;
        public string ErrorMessage { get; set; } = string.Empty;
        public bool IsProgressBar { get; set; } = false;

        public int Counter { get; set; }
        public int TotalRec { get; set; }
        public double BarPercent { get; set; }
        public bool IsBatch { get; set; }
        public bool IsClientUpdate { get; set; }


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
                    decimal _TaxRate = AppliedDB.Functions.GetTaxRate(DBFile, (int)Item["Tax"]);
                    SelectedSaleInvoice _SaleInvoice = new(); ;
                    _SaleInvoice.SrNo = (int)Item["Sr_No"];
                    _SaleInvoice.Company = (string)_master["Description"];   //Functions.GetTitle(DBFile, Enums.Tables.Customers, (int)_master["Company"]);
                    _SaleInvoice.Vou_No = (string)_master["Vou_No"];
                    _SaleInvoice.Vou_Date = (DateTime)_master["Vou_Date"];
                    _SaleInvoice.Inventory = AppliedDB.Functions.GetTitle(DBFile, Tables.Inventory, (int)Item["Inventory"]);
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
        public int SrNo { get; set; }
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

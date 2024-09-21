using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Forms;
using System.Data;
using AppliedDB;
using AppliedAccounts.Models;
using AppliedAccounts.Data;
using System.Diagnostics;
using System.Text;
using Microsoft.AspNetCore.Components;



namespace AppliedAccounts.Pages.ImportData
{


    public partial class ImportSaleInvoice
    {
        [Inject] public NavigationManager NavManager { set; get; } = default!;

        public SaleInvoiceModel Model { get; set; }

        public ImportExcelFile ImportExcel { get; set; }
        public AppUserModel AppUser { get; set; }
        public DataSet? ExcelDataSet { get; set; }
        public DataTable? SalesData { get; set; }
        public DataTable? SalesSchema { get; set; }
        public DataTable? InvData { get; set; }
        public DataTable Sale1 { get; set; }
        public DataTable Sale2 { get; set; }
        DataSource Source { get; set; }


        DateTime Inv_Date = DateTime.Now;
        DateTime Due_Date = DateTime.Now;
        string Batch = string.Empty;
        string Inv_No = string.Empty;
        int TotalRec = 0;

        int Counter = 0;
        int Error = 0;
        int Skip = 0;


        public StringBuilder MyMessage { get; set; } = new();

        public bool IsExcelLoaded { get; set; } = false;
        public bool ShowSpinner { get; set; } = false;

        public Stopwatch stopwatch { get; set; } = new();

        #region Constructor
        public ImportSaleInvoice()
        {

        }

        public ImportSaleInvoice(AppUserModel _AppUser)
        {
            AppUser = _AppUser; stopwatch.Start();
        }
        #endregion

        #region Get Excel file from Import Model, 
        // The Excel File save on server and
        // open from Server
        // Create a Temp Database Table and move all data to this temp SQLite Database File.
        public void GetExcelFile(InputFileChangeEventArgs e)
        {
            try
            {
                ImportExcel = new(e.File, AppUser);
                IsExcelLoaded = true;
                MyMessage.AppendLine($"{DateTime.Now} Excel File loaded.... OK");
            }
            catch (Exception)
            {

                MyMessage.AppendLine($"{DateTime.Now} ERROR: Excel file not loaded.... ");
            }


        }
        #endregion

        public async void GetImportedDataAsync()
        {
            if (IsExcelLoaded)
            {
                ShowSpinner = true;
                stopwatch.Start();
                GetExcelSheetData();
                await GetDataTableAync();
                stopwatch.Stop();
                var ts = stopwatch.Elapsed;
                MyMessage.AppendLine($"{DateTime.Now} Total time spend in process {ts.TotalSeconds}");
                ShowSpinner = false;
                Model.ShowData = true;
                StateHasChanged();
            }
        }

        private void GetExcelSheetData()
        {
            string _TempGUID = AppRegistry.GetText(AppUser.DataFile, "ExcelImport");
            TempDB _TempDB = new(_TempGUID + ".db");
            SalesData = _TempDB.GetTempTable("Data");
            SalesSchema = _TempDB.GetTempTable("Schema");
            InvData = _TempDB.GetTempTable("Invoice Data");
        }

        #region Get Data From SQLite Temp Data, 
        //Stored data from excel file to Temp
        // and here this data will call in a app as Data tables.
        public async Task GetDataTableAync()
        {
            if (SalesData != null && SalesSchema != null && InvData != null)
            {
                SaleInvoiceModel _Result = await Task.Run(() => GetDataTables());
                Model = _Result;
            }
        }




        public SaleInvoiceModel GetDataTables()
        {
            SaleInvoiceModel _Result = new();
            GenerateInvoice();
            MyMessage.AppendLine($"{DateTime.Now} Task Completed.");

            Model.ShowData = true;

            foreach (DataRow Row in Sale1.Rows)
            {
                _Result.SaleInvoiceList.Add(Row);
            }

            foreach (DataRow Row in Sale2.Rows)
            {
                _Result.SaleDetailsList.Add(Row);
            }
            return _Result;

        }

        #endregion

        #region Generate Invoice Master Table
        public void GenerateInvoice()
        {

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

            GetInvoiceData();
            MyMessage.Append("");


            Model.ShowData = true;
            CommandClass _CommandClass = new CommandClass();
            foreach (DataRow Row in SalesData.Rows)
            {

                Counter++;

                if (string.IsNullOrEmpty(Row["Code"].ToString())) { Error++; continue; }
                if ((string)Row["Active"] != "1") { Skip++; continue; }
                if (Counter == 1) { continue; }     // Skip record of Heading in Excel File. 


                DataRow _Row1 = Sale1.NewRow();
                int _CompanyID = Functions.Code2Int(AppUser.DataFile, Enums.Tables.Customers, (string)Row["Code"]);
                int _EmployeeID = Functions.Code2Int(AppUser.DataFile, Enums.Tables.Employees, (string)Row["Employee"]);
                decimal _Total = Conversion.ToDecimal(Row["Total"]);

                _Row1["ID"] = Counter;
                _Row1["Vou_No"] = string.Concat(Batch, Counter.ToString("0000"));
                _Row1["Vou_Date"] = Inv_Date;
                _Row1["Company"] = _CompanyID;
                _Row1["Employee"] = _EmployeeID;
                _Row1["Ref_No"] = Row["Ref_No"];
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

                GetInvoiceDetails(Row);
                
            }
            
            MyMessage.AppendLine($"{DateTime.Now} End Sales Invoice details process..");
        }
        #endregion

        #region Generate Sales Invoice Details Table
        public void GetInvoiceDetails(DataRow Row)
        {
            MyMessage.AppendLine($"{DateTime.Now} Working of Sale Invoice details.");
            var Counter2 = 0;
            var Sr_No = 0;
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


                    int _Inventory = Functions.Code2Int(AppUser.DataFile, Enums.Tables.COA, (string)Row["Code"]);
                    _Row2["Inventory"] = _Inventory;
                    _Row2["Batch"] = Row["Batch"];

                    if ((string)Scheme["Entry ID"] == "A")
                    {
                        _Row2["Qty"] = 1;
                        _Row2["Rate"] = _Amount;


                    }
                    if ((string)Scheme["Entry ID"] == "Q")
                    {
                        _Row2["Qty"] = Conversion.ToDecimal(Row[(string)Scheme["Qty"]]);
                        _Row2["Rate"] = Conversion.ToDecimal(Row[(string)Scheme["Rate"]]);
                    }

                    decimal _TaxID = Functions.Code2Int(AppUser.DataFile, Enums.Tables.Taxes, (string)Scheme["STax"]);
                    _Row2["Tax"] = _TaxID;
                    _Row2["Tax_Rate"] = Functions.Code2Rate(AppUser.DataFile, (int)_Row2["Tax"]);
                    _Row2["Description"] = Row[(string)Scheme["Remarks Code"]];
                    _Row2["Project"] = Row["Project"];

                    Sale2.Rows.Add(_Row2);
                    Model.SaleDetailsList.Add(_Row2);
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
            }
            #endregion


        }
        #endregion





        private void Stop(MouseEventArgs e)
        {
            if (Model.SaleDetailsList.Count > 0)
            {
                Model.ShowData = true;
            }
            else
            {
                ShowSpinner = !Model.ShowData;
            }


            var Stop = true;
        }
        private void DisplayInvoice(MouseEventArgs e)
        {
            NavManager.NavigateTo("/InvDetails");
            //   Wroking is peending.....


        }
        private void Save()
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
                    foreach(var Row in details)
                    {
                        Row["ID"] = 0;
                        Row["TranID"] = _TranID;
                        _Commands = new(Row, AppUser.DataFile);
                        _Commands.SaveChanges();
                        MyMessage.AppendLine($"{DateTime.Now} {master["Vou_No"]} Serial # {Row["Sr_No"]} is saved ---> {IsSaved} ");
                    }
                }
                else
                {
                    MyMessage.AppendLine($"{DateTime.Now} ERROR : Sales Date is not valided to post...");
                }
            }
           
           
        }

        private bool Validation(DataRow _Row)
        {
            return true;
        }

    }




    #region Model Class
    public class SaleInvoiceModel
    {
        public DataRow SaleMaster { get; set; }
        public DataRow SaleDetails { get; set; }

        public List<DataRow> SaleInvoiceList { get; set; } = new();
        public List<DataRow> SaleDetailsList { get; set; } = new();

        public bool ShowData { get; set; } = false;
        public bool ShowMessages { get; set; } = false;
        public bool PostData { get; set; } = false;

        public SaleInvoiceModel()
        {

        }
        public SaleInvoiceModel(DataTable _SalesTable1, DataTable _SaleTable2)
        {
            SaleMaster = _SalesTable1.NewRow();
            SaleDetails = _SaleTable2.NewRow();
        }

        public List<DataRow> GetDetail(int _ID)
        {
            return SaleDetailsList.Where(a => (int)a["ID"] == _ID).ToList();
        }

    }
    #endregion
}

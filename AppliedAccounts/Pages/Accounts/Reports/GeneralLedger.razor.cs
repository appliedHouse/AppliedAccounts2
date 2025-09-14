using AppliedAccounts.Component;
using AppliedAccounts.Data;
using AppliedAccounts.Models;
using AppliedDB;
using AppMessages;
using BlazorJS;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System.Data;
using System.Drawing;
using Format = AppliedGlobals.AppValues.Format;
using KeyType = AppliedGlobals.AppErums.KeyTypes;
using MESSAGES = AppMessages.Enums.Messages;

namespace AppliedAccounts.Pages.Accounts.Reports
{
    public partial class GeneralLedger
    {
        public DataSource Source { get; set; }
        public GLModel MyModel { get; set; }
        public MessageClass MsgClass { get; set; }
        public string DBFile { get; set; }
        public bool IsPageValid { get; set; }
        public bool IsPrinting { get; set; }
        string IsPageValidMessage { get; set; } = "Page has some error. Consult to Administrator";
        NavigationManager NavManager => AppGlobal.NavManager;

        public List<CodeTitle> Accounts { get; set; }
        public List<CodeTitle> Companies { get; set; }
        public List<CodeTitle> Employees { get; set; }

        public GeneralLedger()
        {
            MyModel = new();
        }

        public void Start(AppliedGlobals.AppUserModel _UserModel)
        {
            if (_UserModel != null)
            {

                MsgClass = new();
                //UserModel = _UserModel;
                Source = new(AppGlobal.AppPaths);
                DBFile = AppGlobal.DBFile;
                Accounts = Source.GetAccounts();            // Get List of Accounts
                Companies = Source.GetCustomers();          // Get List of Costomer/Clients
                Employees = Source.GetEmployees();        // Get List of Employees


            }
            else
            {
                IsPageValid = false;
                MsgClass.Add(MESSAGES.PageIsNotValid);
            }

            GetKeys();
        }


        #region Get & Set Registry Keys 
        private void GetKeys()
        {
            MyModel.COAID = AppRegistry.GetNumber(DBFile, "GL_COA");
            MyModel.Date_From = AppRegistry.GetFrom(DBFile, "GL_COA");
            MyModel.Date_To = AppRegistry.GetTo(DBFile, "GL_COA");
            MyModel.SortBy = AppRegistry.GetText(DBFile, "GL_COA");

            MyModel.CompanyID = AppRegistry.GetNumber(DBFile, "GL_Company");
            MyModel.DtFrom_Com = AppRegistry.GetFrom(DBFile, "GL_Company");
            MyModel.DtTo_Com = AppRegistry.GetTo(DBFile, "GL_Company");

            MyModel.EmployeeID = AppRegistry.GetNumber(DBFile, "GL_Employee");
            MyModel.DtFrom_Emp = AppRegistry.GetFrom(DBFile, "GL_Employee");
            MyModel.DtTo_Emp = AppRegistry.GetTo(DBFile, "GL_Employee");
        }

        private void SetKeys()
        {
            AppRegistry.SetKey(DBFile, "GL_COA", MyModel.COAID, KeyType.Number, "General Ledger ID,From,To,Sort");
            AppRegistry.SetKey(DBFile, "GL_COA", MyModel.Date_From, KeyType.From);
            AppRegistry.SetKey(DBFile, "GL_COA", MyModel.Date_To, KeyType.To);
            AppRegistry.SetKey(DBFile, "GL_COA", MyModel.SortBy, KeyType.Text);

            AppRegistry.SetKey(DBFile, "GL_Company", MyModel.CompanyID, KeyType.Number, "Company Ledger ID,From,To,Sort");
            AppRegistry.SetKey(DBFile, "GL_Company", MyModel.DtFrom_Com, KeyType.From);
            AppRegistry.SetKey(DBFile, "GL_Company", MyModel.DtTo_Emp, KeyType.To);
            AppRegistry.SetKey(DBFile, "GL_Company", MyModel.SortBy, KeyType.Text);

            AppRegistry.SetKey(DBFile, "GL_Employee", MyModel.EmployeeID, KeyType.Number, "Company Ledger ID,From,To,Sort");
            AppRegistry.SetKey(DBFile, "GL_Employee", MyModel.DtFrom_Emp, KeyType.From);
            AppRegistry.SetKey(DBFile, "GL_Employee", MyModel.DtTo_Emp, KeyType.To);
            AppRegistry.SetKey(DBFile, "GL_Employee", MyModel.SortBy, KeyType.Text);

        }
        #endregion

        #region Refresh and BackPage

        public void Refresh()
        {
            SetKeys();
            


            //GetReportData();
            //if (_ReportData.ReportTable != null)
            //{
            //    MyModel.Ledger = _ReportData.ReportTable;
            //}
        }

        public void BackPage()
        {
            NavManager.NavigateTo("/Menu/Accounts");
        }
        #endregion

        #region Print
        public async void Print(ReportActionClass PrintAction)
        {
            MsgClass = new();           // Clear all previous messages - refresh
            IsPrinting = true;
            await InvokeAsync(StateHasChanged);

            try
            {
                SetKeys();
                await Task.Run(() =>
                {
                    ReportService = new(AppGlobal); ;
                    ReportService.ReportType = PrintAction.PrintType;
                    GetReportData();
                    CreateReportModel();

                    if (!ReportService.IsError)
                    {
                        ReportService.Print();
                    }

                });

            }
            catch (Exception error)
            {
                MsgClass.Error(error.Message);
            }

            IsPrinting = false;
            await InvokeAsync(StateHasChanged);
        }

        public async void GetReportData()
        {
            if(MyModel.COAID == 0)
            {
                ReportService.IsError = true;
                MsgClass.Add(MESSAGES.AccountIDIsZero);
                return;
            }

            var _OBDate = MyModel.Date_From.AddDays(-1).ToString(Format.YMD);
            var _DateFrom = MyModel.Date_From.ToString(Format.YMD);
            var _DateTo = MyModel.Date_To.ToString(Format.YMD);

            var _FilterOB = $"[COA] = {MyModel.COAID} AND Date([Vou_Date]) < Date('{_OBDate}')";
            var _Filter = $"[COA] = {MyModel.COAID} AND (Date([Vou_Date]) BETWEEN Date('{_DateFrom}') AND Date('{_DateTo}'))";
            var _GroupBy = "[COA]";
            var _SortBy = "[Vou_date], [Vou_no]";
            var _Query = SQLQueries.Quries.GeneralLedger(MyModel.COAID, _OBDate, _FilterOB, _GroupBy, _Filter, _SortBy);

            MyModel.PagingQuery.Query = _Query;
            MyModel.PagingQuery.Source = Source;
             
            MyModel.PagingQuery.Pages = MyModel.PagingQuery.Pages ?? new PageModel();

            DataTable _ReportTable = Source.GetTable(_Query);
            MyModel.PagingQuery.Pages.TotalRecords = _ReportTable.Rows.Count;
            DataTable _DisplayTable = await MyModel.PagingQuery.GetPageData();

            if (_ReportTable.Columns.Count == 0)
            {
                ReportService.IsError = false;
                MsgClass.Add(MESSAGES.NoRecordFound);
            }

            
            ReportService.Data.ReportTable = _ReportTable;
            ReportService.Data.DataSetName = "dsname_Ledger";

            MyModel.Ledger = _DisplayTable;
        }

        public void CreateReportModel()
        {

            var _Heading1 = $"General Ledger " + Source.SeekTitle(AppliedDB.Enums.Tables.COA, MyModel.COAID);
            var _Heading2 = $"[{MyModel.Date_From.ToString(Format.DDMMMYY)}] to [{MyModel.Date_To.ToString(Format.DDMMMYY)}] ";

            ReportService.Model.InputReport.FileName = "Ledger.rdl";
            ReportService.Model.ReportDataSource = ReportService.Data;                   // Load Reporting Data to Report Model
            ReportService.Model.OutputReport.FileName = "Ledger_" + "CompanyName";
            ReportService.Model.OutputReport.ReportType = ReportService.ReportType;
            ReportService.Model.AddReportParameter("Heading1", _Heading1);
            ReportService.Model.AddReportParameter("Heading2", _Heading2);

        }
        #endregion

        #region Open Dialog windows

        private async Task HandleClick(MouseEventArgs e)
        {
            // Prevent default using JS interop
            await AppGlobal.JS.InvokeVoidAsync("eval", "event.preventDefault()");
            opnCompanies();
        }

        public void opnCompanies()
        {
            AppGlobal.JS.AlertAsync("Open Companies Dialog Box");

        }
        #endregion
    }

    public class GLModel
    {
        public int BookID { get; set; }
        public int COAID { get; set; }
        public int CompanyID { get; set; }
        public int ProjectID { get; set; }
        public int EmployeeID { get; set; }

        public DateTime Date_From { get; set; }
        public DateTime Date_To { get; set; }
        public string SortBy { get; set; }
        public DataTable Ledger { get; set; }

        public DateTime DtFrom_Com { get; set; }            // Date for (Companies/Clients)
        public DateTime DtTo_Com { get; set; }

        public DateTime DtFrom_Emp { get; set; }            // Date for (Employees)
        public DateTime DtTo_Emp { get; set; }

        public PageQuery PagingQuery { get; set; } = new();


    }
}

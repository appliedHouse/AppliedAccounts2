using AppliedAccounts.Data;
using AppliedAccounts.Models.Interface;
using AppliedAccounts.Services;
using AppliedDB;
using AppMessages;
using AppReports;
using Microsoft.AspNetCore.Components;
using System.Data;
using MESSAGES = AppMessages.Enums.Messages;

namespace AppliedAccounts.Pages.Reporting
{
    public partial class GeneralLedger : IPrint
    {
        public AppUserModel UserModel { get; set; }
        public DataSource Source { get; set; }
        public GLModel MyModel { get; set; } 
        public MessageClass MsgClass { get; set; }
        public string DBFile { get; set; }
        public bool IsPageValid { get; set; }
        public bool IsPrinting { get; set; }
        string IsPageValidMessage { get; set; } = "Page has some error. Consult to Administrator";
        NavigationManager NavManager => AppGlobals.NavManager;

        public List<CodeTitle> Accounts { get; set; }

        public GeneralLedger()
        {
            MyModel = new();
        }

        public void Start(AppUserModel _UserModel)
        {
            if (_UserModel != null)
            {

                MsgClass = new();
                UserModel = _UserModel;
                Source = new(UserModel);
                DBFile = UserModel.DataFile;
                Accounts = Source.GetAccounts();            // Get List of Accounts


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
        }

        private void SetKeys()
        {
            AppRegistry.SetKey(DBFile, "GL_COA", MyModel.COAID, KeyType.Number, "General Ledger ID,From,To,Sort");
            AppRegistry.SetKey(DBFile, "GL_COA", MyModel.Date_From, KeyType.From);
            AppRegistry.SetKey(DBFile, "GL_COA", MyModel.Date_To, KeyType.To);
            AppRegistry.SetKey(DBFile, "GL_COA", MyModel.SortBy, KeyType.Text);
        }
        #endregion

        #region Refresh and BackPage

        public void Refresh()
        {
            SetKeys();
            ReportData _ReportData = GetReportData();
            if (_ReportData.ReportTable != null)
            {
                MyModel.Ledger = _ReportData.ReportTable;
            }
        }

        public void BackPage()
        {
            NavManager.NavigateTo("/Menu/Accounts");
        }
        #endregion

        #region Print
        public async void Print(ReportType PrintType)
        {
            
            if (MyModel.COAID > 0)
            {
                await PrintLedger(MyModel.COAID, PrintType);
            }
            else
            { MsgClass.Add(MESSAGES.COAIsNull); }
        }

        public async Task PrintLedger(int ID, ReportType PrintType)
        {
            IsPrinting = true;
            await InvokeAsync(StateHasChanged);

            SetKeys();
            await Task.Run(() =>
            {
                ReportService = new(AppGlobals); ;
                ReportService.RptType = PrintType;
                ReportService.RptData = GetReportData();
                ReportService.RptModel = CreateReportModel();
                
            });

            try
            {
                ReportService.Print();
            }
            catch (Exception)
            {
                ReportService.MyMessage = "Error....";
                MsgClass.Add(ReportService.MyMessage);
            }
            

            IsPrinting = false;
            await InvokeAsync(StateHasChanged);
        }

        public ReportData GetReportData()
        {
            var _OBDate = MyModel.Date_From.AddDays(-1).ToString(Format.YMD);
            var _DateFrom = MyModel.Date_From.ToString(Format.YMD);
            var _DateTo = MyModel.Date_To.ToString(Format.YMD);

            var _FilterOB = $"[COA] = {MyModel.COAID} AND Date([Vou_Date]) < Date('{_OBDate}')";
            var _Filter = $"[COA] = {MyModel.COAID} AND (Date([Vou_Date]) BETWEEN Date('{_DateFrom}') AND Date('{_DateTo}'))";
            var _GroupBy = "[COA]";
            var _SortBy = "[Vou_date], [Vou_no]";
            var _Query = SQLQueries.Quries.GeneralLedger(MyModel.COAID, _OBDate, _FilterOB, _GroupBy, _Filter,  _SortBy);

            DataTable _Table = Source.GetTable(_Query);

            if (_Table.Columns.Count == 0)
            {
                MsgClass.Add(MESSAGES.NoRecordFound);
                return new();
            }

            ReportData _ReportData = new()
            {
                ReportTable = _Table,
                DataSetName = "dsname_Ledger"
            };
            return _ReportData;
        }

        public ReportModel CreateReportModel()
        {
            ReportModel Report = new ReportModel();
            
            var _Heading1 = $"General Ledger " + Source.SeekTitle(AppliedDB.Enums.Tables.COA, MyModel.COAID);
            var _Heading2 = $"[{MyModel.Date_From.ToString(Format.DDMMMYY)}] to [{MyModel.Date_To.ToString(Format.DDMMMYY)}] "; 

            Report.ReportUrl = NavManager.BaseUri;

            Report.InputReport.FilePath = UserModel.ReportFolder;
            Report.InputReport.FileName = "Ledger";
            Report.InputReport.FileExtention = "rdl";

            Report.ReportDataSource = ReportService.RptData;                   // Load Reporting Data to Report Model

            Report.OutputReport.FilePath = UserModel.PDFFolder;
            Report.OutputReport.FileName = "Ledger_" + "CompanyName";
            Report.OutputReport.ReportType = ReportService.RptType;
            Report.OutputReport.ReportUrl = Report.ReportUrl;

            Report.AddReportParameter("CompanyName", UserModel.Company);
            Report.AddReportParameter("Heading1", _Heading1);
            Report.AddReportParameter("Heading2", _Heading2);
            Report.AddReportParameter("Footer", AppFunctions.ReportFooter());

            return Report;
        }
        #endregion
    }

    public class GLModel
    {
        public int BookID { get; set; }
        public int COAID { get; set; }
        public int CompanyID { get; set; }
        public int ProjectID { get; set; }
        public int EmployeeId { get; set; }

        public DateTime Date_From { get; set; }
        public DateTime Date_To { get; set; }
        public string SortBy { get; set; }
        public DataTable Ledger { get; set; }

    }
}

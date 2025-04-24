using AppliedAccounts.Data;
using AppliedAccounts.Services;
using AppliedDB;
using AppMessages;
using AppReports;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Data;
using MESSAGES = AppMessages.Enums.Messages;

namespace AppliedAccounts.Pages.Reporting
{
    public partial class GeneralLedger
    {
        public GlobalService Globals { get; set; }
        public AppUserModel UserModel { get; set; }
        public DataSource Source { get; set; }
        public PrintService ReportService { get; set; }
        public MessageClass MsgClass { get; set; }
        public int COAID { get; set; }
        public DateTime Date_From { get; set; }
        public DateTime Date_To { get; set; }
        public string SortBy { get; set; }
        public string DBFile { get; set; }
        public bool IsPageValid { get; set; }
        public bool IsPrinting { get; set; }
        string IsPageValidMessage { get; set; } = "Page has some error. Consult to Administrator";
        NavigationManager NavManager => Globals.NavManager;

        public List<CodeTitle> Accounts { get; set; }

        public GeneralLedger()
        {


            COAID = 0;
            SortBy = "";
            DBFile = "";

            // Only One DataRow ID=GL_COA for enough for all info.
            COAID = AppRegistry.GetNumber(DBFile, "GL_COA");
            Date_From = AppRegistry.GetFrom(DBFile, "GL_COA");
            Date_To = AppRegistry.GetTo(DBFile, "GL_COA");
            SortBy = AppRegistry.GetText(DBFile, "GL_COA");


        }

        public void Start(AppUserModel _UserModel)
        {
            MsgClass = new();
            UserModel = _UserModel;
            Source = new(UserModel);
            ReportSeervice = new();
            DBFile = UserModel.DataFile;

            Accounts = Source.GetAccounts();


        }

        public void BackPage()
        {
            NavManager.NavigateTo("/Menu/Accounts");
        }


        #region Print
        public async void Print(ReportType PrintType)
        {
            
            if (COAID > 0)
            {
                await Print(COAID, PrintType);
            }
            else
            { MsgClass.Add(MESSAGES.COAIsNull); }
        }

        public async Task Print(int ID, ReportType PrintType)
        {
            IsPrinting = true;
            await InvokeAsync(StateHasChanged);

            Start(UserModel);
            AppRegistry.SetKey(DBFile, "GL_COA", COAID, KeyType.Number, "General Ledger ID,From,To,Sort");
            AppRegistry.SetKey(DBFile, "GL_COA", Date_From, KeyType.From);
            AppRegistry.SetKey(DBFile, "GL_COA", Date_To, KeyType.To);
            AppRegistry.SetKey(DBFile, "GL_COA", SortBy, KeyType.Text);

            await Task.Run(() =>
            {
                ReportService = new(GlobalService); ;
                ReportService.JS = js;
                ReportService.RptType = PrintType;
                ReportService.RptData = GetReportData(ID);
                ReportService.RptModel = CreateReportModel(ID);
            });

            await ReportService.Print();
            IsPrinting = false;
            await InvokeAsync(StateHasChanged);
        }

        private ReportData GetReportData(int ID)
        {
            var _OBDate = Date_From.AddDays(-1).ToString(Format.YMD);
            var _DateFrom = Date_From.ToString(Format.YMD);
            var _DateTo = Date_To.ToString(Format.YMD);

            var _FilterOB = $"[COA] = {COAID} AND Date([Vou_Date]) < Date('{_OBDate}')";
            var _Filter = $"[COA] = {COAID} AND (Date([Vou_Date]) BETWEEN Date('{_DateFrom}') AND Date('{_DateTo}'))";
            var _GroupBy = "[COA]";
            var _SortBy = "[Vou_date], [Vou_no]";
            var _Query = SQLQueries.Quries.GeneralLedger(_OBDate, _FilterOB, _GroupBy, _Filter,  _SortBy);

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

        private ReportModel CreateReportModel(int _ID)
        {
            ReportModel Report = new ReportModel();
            
            var _Heading1 = $"General Ledger " + Source.SeekTitle(AppliedDB.Enums.Tables.COA, COAID);
            var _Heading2 = $"[{Date_From.ToString(Format.DDMMMYY)}] to [{Date_To.ToString(Format.DDMMMYY)}] "; 

            Report.ReportUrl = NavManager.BaseUri;

            Report.InputReport.FilePath = UserModel.ReportFolder;
            Report.InputReport.FileName = "Ledger";
            Report.InputReport.FileExtention = "rdl";

            Report.OutputReport.FilePath = UserModel.PDFFolder;
            Report.OutputReport.FileName = "Ledger_" + "CompanyName";
            Report.OutputReport.ReportType = ReportType.Print;
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

        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public string SortBy { get; set; }

    }
}

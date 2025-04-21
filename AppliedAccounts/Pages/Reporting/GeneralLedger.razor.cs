using AppliedAccounts.Data;
using AppliedAccounts.Services;
using AppliedDB;
using AppMessages;
using AppReports;
using System.Data;
using MESSAGES = AppMessages.Enums.Messages;

namespace AppliedAccounts.Pages.Reporting
{
    public partial class GeneralLedger
    {
        public AppUserModel UserModel { get; set; }
        public DataSource Source { get; set; }
        public PrintService ReportService { get; set; }
        public MessageClass MsgClass { get; set; }
        public int COAID { get; set; }
        public DateTime Date_From { get; set; }
        public DateTime Date_To { get; set; }
        public string SortBy { get; set; }
        public string DBFile { get; set; }

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
            UserModel = _UserModel;
            Source = new(UserModel);
        }


        public void Print(int ID, ReportType PrintType)
        {
            Start(UserModel);
            AppRegistry.SetKey(DBFile, "GL_COA", COAID, KeyType.Number, "General Ledger ID,From,To,Sort");
            AppRegistry.SetKey(DBFile, "GL_COA", Date_From, KeyType.From);
            AppRegistry.SetKey(DBFile, "GL_COA", Date_To, KeyType.To);
            AppRegistry.SetKey(DBFile, "GL_COA", SortBy, KeyType.Text);

            ReportService = new();
            ReportService.RptType = PrintType;
            ReportService.RptData = GetReportData(ID);
            ReportService.RptModel = CreateReportModel(ID);
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
            var _Query = SQLQueries.Quries.GeneralLedger(_OBDate, _FilterOB, _Filter, _GroupBy, _SortBy);

            DataTable _Table = Source.GetTable(_Query);

            if (_Table.Columns.Count == 0)
            {
                MsgClass.Add(MESSAGES.NoRecordFound);
                return new();
            }

            ReportData _ReportData = new ReportData();
            _ReportData.ReportTable = _Table;
            _ReportData.DataSetName = "dsname_Ledger";

            ReportData reportData = new ReportData();
            return reportData;
        }

        private ReportModel CreateReportModel(int _ID)
        {
            ReportModel Report = new ReportModel();

            Report.InputReport.FileName = "Ledger";
            Report.InputReport.FileExtention = "rdl";

            Report.OutputReport.FileName = "Ledger_" + "CompanyName";
            Report.OutputReport.ReportType = ReportType.Print;


            return Report;
        }
    }
}

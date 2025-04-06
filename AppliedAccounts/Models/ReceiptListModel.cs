using AppliedAccounts.Data;
using AppliedAccounts.Models.Interface;
using AppliedAccounts.Services;
using AppliedDB;
using AppMessages;
using AppReports;
using System.Data;
using System.Text;
using static AppliedDB.Enums;

namespace AppliedAccounts.Models
{
    public class ReceiptListModel : IVoucherList
    {
        public AppUserModel? UserProfile { get; set; }
        public DataSource Source { get; set; }
        public List<DataRow> DataList { get; set; }
        public List<CodeTitle> PayerList { get; set; }
        public int PayerID { get; set; }
        public Tables Table { get; set; }
        public string SearchText { get; set; }
        public MessageClass MsgClass { get; set; }
        public PrintService Printer { get; set; }

        public DateTime DT_Start { get; set; }
        public DateTime DT_End { get; set; }
        public bool PageIsValid { get; set; } = false;
        public Globals AppGlobal { get; set; }

        public ReceiptListModel(AppUserModel _AppUserModel)
        {
            UserProfile = _AppUserModel;
            Table = Tables.view_Receipts;
            Source = new DataSource(UserProfile);
            MsgClass = new();
            DT_Start = AppRegistry.GetDate(Source.DBFile, "rcptFrom");
            DT_End = AppRegistry.GetDate(Source.DBFile, "rcptTo");
            SearchText = AppRegistry.GetText(Source.DBFile, "rcptSearch");
            PayerList = Source.GetCustomers();
            DataList = LoadData();
            Printer = new();

        }

        #region Load Data
        public List<DataRow> LoadData()
        {
            using var _DataTable = Source.GetTable(SQLQueries.Quries.ReceiptList(GetFilterText()));
            return [.. _DataTable.AsEnumerable()];
        }
        #endregion

        #region Get Text for filter List
        public string GetFilterText()
        {
            var _Text = new StringBuilder();

            if (SearchText.Length > 0)
            {
                _Text.AppendLine($"[Vou_No] LIKE '%{SearchText}%' OR ");
                _Text.AppendLine($"[TitlePayer] LIKE '%{SearchText}%' OR ");
                _Text.AppendLine($"[TitleAccount] LIKE '%{SearchText}%' OR ");
                _Text.AppendLine($"[Doc_No] LIKE '%{SearchText}%' OR ");
                _Text.AppendLine($"[Ref_No] LIKE '%{SearchText}%' OR ");
                _Text.AppendLine($"Date([Vou_Date]) Between ");
                _Text.AppendLine($"'{DT_Start.ToString(Format.YMD)}' AND");
                _Text.AppendLine($"'{DT_End.ToString(Format.YMD)}' ");
                return _Text.ToString();
            }

            if(PayerID > 0)
            {
                _Text.Append($"[Payer] == {PayerID}");
                return _Text.ToString();
            }

            return string.Empty;
        }
        #endregion

        #region Refresh Data
        public void RefreshData()
        {
            DataList = LoadData();
        }
        #endregion

        #region Print
        public void Print(int _ID)
        {
            var _SourceData = Source.GetReceiptVoucher(_ID);

            ReportData rptDate = new();
            rptDate.ReportTable = Source.GetReceiptVoucher(_ID); ;
            rptDate.DataSetName = "ds_Receipt"; 

            ReportModel rptModel = new();

            rptModel.InputReport.FileName = "Receipt";
            rptModel.InputReport.FileExtention = ".rdl";
            rptModel.InputReport.FilePath = AppGlobal.ReportPath;

            rptModel.OutputReport.FileName = $"Receipt_{_ID}";
            rptModel.OutputReport.FileExtention = ".pdf";
            rptModel.OutputReport.FilePath = AppGlobal.PDFPath;
            rptModel.OutputReport.ReportType = ReportType.PDF;

            rptModel.ReportData = rptDate;

            Printer = new();
            Printer.RptData = rptDate;
            Printer.RptModel = rptModel;
            Printer.RptType = ReportType.PDF;
            Printer.RptUrl = Printer.GetReportLink();


        }

        private ReportData GetReportData()
        {
            var _Report = new ReportData();
            
            return _Report;
        }

        #endregion

    }
}

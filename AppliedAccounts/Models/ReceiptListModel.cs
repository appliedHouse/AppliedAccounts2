using AppliedAccounts.Data;
using AppliedAccounts.Models.Interface;
using AppliedAccounts.Services;
using AppliedDB;
using AppMessages;
using AppReports;
using Microsoft.AspNetCore.Components;
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
        public PrintService ReportService { get; set; }
        public AppUserModel? AppUser { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string DBFile { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public object Record { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public List<object> Records { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public decimal TotalAmount { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool SelectAll { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public NavigationManager NavManager { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

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

            if (PayerID > 0)
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
            ReportService = new()
            {
                RptData = GetReportData(_ID),              // always generate Data for report
                RptModel = CreateReportModel(_ID),         // and then generate report parameters
               

            };
            ReportService.RptType = ReportType.Preview;
            var ReportList = ReportService.GetReportLink();
            //await js.InvokeVoidAsync("downloadPDF", _FileName, ReportService.RptModel.ReportBytes);

        }

        public ReportData GetReportData(int ID)
        {
            var _Query = SQLQueries.Quries.Receipt(ID);
            var _Table = Source.GetTable(_Query);
            var _ReportData = new ReportData();

            _ReportData.ReportTable = _Table;
            _ReportData.DataSetName = "ds_receipt";

            return _ReportData;
        }

        private ReportModel CreateReportModel(int ID)
        {
            var _InvoiceNo = "Receipt";
            var _Heading1 = "Receipt";
            var _Heading2 = $"Receipt No. {_InvoiceNo}";
            var _ReportPath = UserProfile!.ReportFolder;
            var _CompanyName = UserProfile.Company;
            var _ReportFooter = AppFunctions.ReportFooter();

            ReportModel rptModel = new();

            rptModel.InputReport.FileName = $"Receipt";
            rptModel.InputReport.FileExtention = "rdl";
            rptModel.InputReport.FilePath = UserProfile!.ReportFolder;

            rptModel.OutputReport.FileName = $"Receipt_{ID}";
            rptModel.OutputReport.FileExtention = ".pdf";
            rptModel.OutputReport.FilePath = UserProfile!.PDFFolder;
            rptModel.OutputReport.ReportType = ReportType.PDF;

            rptModel.AddReportParameter("CompanyName", _CompanyName);
            rptModel.AddReportParameter("Heading1", _Heading1);
            rptModel.AddReportParameter("Heading2", _Heading2);
            rptModel.AddReportParameter("Footer", _ReportFooter);

            return rptModel;
        }

        public void Edit(int _ID)
        {
            throw new NotImplementedException();
        }

        List<object> IVoucherList.LoadData()
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}

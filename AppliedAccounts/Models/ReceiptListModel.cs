using Applied_WebApplication.Data;
using AppliedAccounts.Data;
using AppliedAccounts.Models.Interface;
using AppliedAccounts.Pages.Accounts;
using AppliedAccounts.Services;
using AppliedDB;
using AppMessages;
using AppReports;
using SQLQueries;
using System.Data;
using System.Text;
using static AppliedDB.Enums;
using MESSAGES = AppMessages.Enums.Messages;

namespace AppliedAccounts.Models
{
    public class ReceiptListModel
    {
        public GlobalService AppGlobals { get; set; }
        public AppUserModel? UserProfile { get; set; }
        public DataSource Source { get; set; }
        public List<DataRow> DataList { get; set; }
        public List<CodeTitle> PayerList { get; set; }
        public int ReceiptID { get; set; }
        public int PayerID { get; set; }
        public Tables Table { get; set; }
        public string SearchText { get; set; }
        public MessageClass MsgClass { get; set; }
        public DateTime DT_Start { get; set; }
        public DateTime DT_End { get; set; }
        public bool PageIsValid { get; set; } = false;
        public PrintService ReportService { get; set; }

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
        public async Task Print(ReportActionClass reportAction)
        {
            ReceiptID = reportAction.VoucherID;
            ReportService = new(AppGlobals); ;
            ReportService.ReportType = reportAction.PrintType;
            GetReportData();
            UpdateReportModel();

            try
            {
                ReportService.Print();
            }
            catch (Exception error)
            {
                MsgClass.Add(error.Message);
            }
        }

        public void GetReportData()
        {
            var _Query = Quries.Receipt(ReceiptID);
            var _Table = Source.GetTable(_Query);

            ReportService.Data.ReportTable = _Table;
            ReportService.Data.DataSetName = "ds_receipt";
        }

        public void UpdateReportModel()
        {
            var _InvoiceNo = "Receipt";
            var _Heading1 = "Receipt";
            var _Heading2 = $"Receipt No. {_InvoiceNo}";
            var _ReportPath = UserProfile!.ReportFolder;
            var _CompanyName = UserProfile.Company;
            var _ReportFooter = AppFunctions.ReportFooter();

            var _Amount = (decimal)ReportService.Data.ReportTable.Rows[0]["Amount"];
            var _NumInWords = new NumInWords();
            var _AmountinWord = _NumInWords.ChangeCurrencyToWords(_Amount, "SAR", "...");
            var ShowImage = false;

            ReportService.Model.InputReport.FileName = $"Receipt.rdl";
            ReportService.Model.InputReport.FilePath = UserProfile!.ReportFolder;
            ReportService.Model.ReportDataSource = ReportService.Data;
            ReportService.Model.OutputReport.FileName = $"Receipt_{ReceiptID}";
            ReportService.Model.OutputReport.FilePath = UserProfile!.PDFFolder;
            ReportService.Model.OutputReport.ReportType = ReportService.ReportType;

            ReportExtractor reportExtractor = new(ReportService.Model.InputReport.FileFullName);

            ReportService.Model.AddReportParameter("Heading1", _Heading1);
            ReportService.Model.AddReportParameter("Heading2", _Heading2);
            ReportService.Model.AddReportParameter("InWord", _AmountinWord);
            ReportService.Model.AddReportParameter("CurrencySign", AppGlobals.Currency.Sign ?? "$");
            ReportService.Model.AddReportParameter("PayerTitle", "Donor");
            ReportService.Model.AddReportParameter("ShowImages", ShowImage.ToString());
        }
        #endregion

        #region Edit
        public void Edit(int _ID)
        {
            AppGlobals.NavManager.NavigateTo($"/Accounts/Receipt/{ReceiptID}");
        }
        #endregion



    }
}

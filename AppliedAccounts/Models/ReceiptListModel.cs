using Applied_WebApplication.Data;
using AppliedAccounts.Data;
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
            SetKeys();
            DataList = LoadData();
        }
        #endregion

        #region Print
        public async Task Print(ReportActionClass reportAction)
        {
            await Task.Run(() =>
            {
                try
                {
                    SetKeys();
                    ReceiptID = reportAction.VoucherID;
                    ReportService = new(AppGlobals); ;                      // Initialize Report Service
                    ReportService.ReportType = reportAction.PrintType;      // Assign Report Type 
                    GetReportData();                                        // Report Data Source Setup
                    UpdateReportModel();                                    // Update Report Model

                    if (!ReportService.IsError) { ReportService.Print(); }  // Report Print / Preview / PDF / Excel / Word / Image / HTML
                    else
                    {
                        MsgClass.Critical(MESSAGES.rptNotValidToPrint);     // Add Error Message to Page error view if Report is not valid
                    }
                }
                catch (Exception error)
                {
                    MsgClass.Add(error.Message);
                }

            });
        }

        public void GetReportData()
        {
            var _Query = Quries.Receipt(ReceiptID);
            var _Table = Source.GetTable(_Query);

            if (_Table.Columns.Count == 0)
            {
                ReportService.IsError = true;
                MsgClass.Error(MESSAGES.rptDataTableIsNull);
            }
            else
            {
                ReportService.Data.ReportTable = _Table;
                ReportService.Data.DataSetName = "ds_receipt";
            }
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
            var _Currency = AppGlobals.Currency.Sign ?? "$";
            var _CurrencyDigit = AppGlobals.Currency.DigitTitle ?? "";
            var _AmountinWord = _NumInWords.ChangeCurrencyToWords(_Amount, _Currency, _CurrencyDigit);
            var ShowImage = false;

            ReportService.Model.InputReport.FileName = $"Receipt.rdl";
            //ReportService.Model.InputReport.FilePath = UserProfile!.ReportFolder;
            ReportService.Model.ReportDataSource = ReportService.Data;
            ReportService.Model.OutputReport.FileName = $"Receipt_{ReceiptID}";
            //ReportService.Model.OutputReport.FilePath = UserProfile!.PDFFolder;
            ReportService.Model.OutputReport.ReportType = ReportService.ReportType;
            ReportService.Model.AddReportParameter("Heading1", _Heading1);
            ReportService.Model.AddReportParameter("Heading2", _Heading2);
            ReportService.Model.AddReportParameter("InWord", _AmountinWord);
            ReportService.Model.AddReportParameter("CurrencySign", AppGlobals.Currency.Sign ?? "$");
            ReportService.Model.AddReportParameter("PayerTitle", "Donor");
            ReportService.Model.AddReportParameter("ShowImages", ShowImage.ToString());
            ReportService.Extractor = new ReportExtractor(ReportService.Model.InputReport.FileFullName);
        }
        #endregion

        #region Edit
        public void Edit(int _ID)
        {
            SetKeys();
            AppGlobals.NavManager.NavigateTo($"/Accounts/Receipt/{ReceiptID}");
        }
        #endregion

        #region Get & Set Keys
        public void SetKeys()
        {
            AppRegistry.SetKey(Source.DBFile, "rptRcptList", DT_Start, KeyType.From, "Receipt Report /Accounts/ReceiptList");
            AppRegistry.SetKey(Source.DBFile, "rptRcptList", DT_End, KeyType.To);
            AppRegistry.SetKey(Source.DBFile, "rptRcptList", SearchText, KeyType.Text);
            AppRegistry.SetKey(Source.DBFile, "rptRcptList", PayerID, KeyType.Number);
        }

        public void GetKeys()
        {
            SearchText = AppRegistry.GetText(Source.DBFile, "rptRcptList");
            DT_Start = AppRegistry.GetFrom(Source.DBFile, "rptRcptList");
            DT_End = AppRegistry.GetTo(Source.DBFile, "rptRcptList");
            PayerID = AppRegistry.GetNumber(Source.DBFile, "rptRcptList");
        }
        #endregion


    }
}

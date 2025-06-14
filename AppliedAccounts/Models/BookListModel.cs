using AppliedAccounts.Data;
using AppliedAccounts.Pages.Accounts;
using AppliedAccounts.Services;
using AppliedDB;
using AppReports;
using System.Data;
using Format = AppliedGlobals.AppValues.Format;
using Messages = AppMessages.Enums.Messages;

namespace AppliedAccounts.Models
{
    public class BookListModel  // : IVoucherList
    {
        public GlobalService AppGlobal { get; set; }
        public List<CodeTitle> BookList { get; set; }
        public List<CodeTitle> NatureAccountsList { get; set; }
        public DataSource Source { get; set; }
        public AppMessages.MessageClass MsgClass { get; set; }
        public PrintService ReportService { get; set; }
        public PageModel Pages { get; set; } = new();

        public int BookID { get; set; }
        public int BookNatureID { get; set; }
        public int VoucherID { get; set; }
        public int TotalRecord { get; set; } = 0;
        public DateTime DT_Start { get; set; }
        public DateTime DT_End { get; set; }
        public string SearchText { get; set; }
        public string BookNatureTitle = "Book Title";
        //public List<BookView> BookRecords { get; set; }
        public bool PageIsValid { get; set; } = false;
        public bool IsWaiting { get; set; } = false;

        #region Constructor
        public BookListModel()
        {

        }
        public BookListModel(int _BookID, GlobalService _AppGlobal)
        {
            AppGlobal = _AppGlobal;
            MsgClass = new();
            Source = new(AppGlobal.AppPaths);
            GetKeys();

            try
            {
                if (_BookID == 0) { BookID = 1; } else { BookID = _BookID; }
                var result = Source?.SeekValue(Enums.Tables.COA, BookID, "Nature") ?? 0;
                BookNatureID = (int)result;
                BookID = _BookID;

                NatureAccountsList =
                [
                    new() { ID = 1, Code = "01", Title = "Cash" },
                    new() { ID = 2, Code = "02", Title = "Bank" },
                ];

                PageIsValid = LoadData();
            }
            catch (Exception)
            {
                MsgClass.Add(Messages.PageIsNotValid);
            }
        }
        #endregion


        public bool LoadData()
        {
            try
            {
                LoadBookRecords(BookID);
                return false;
            }
            catch (Exception)
            {
                MsgClass.Add(Messages.DataLoadFailed);
                return false;
            }
        }

        public List<BookView> LoadBookRecords(int _BookID)     // Load List of Cash / Bank Book record in Table
        {
            if (_BookID == 0) { return []; }

            string _Date1 = DT_Start.ToString(Format.YMD);
            string _Date2 = DT_End.ToString(Format.YMD);
            string _Query = $"SELECT * FROM [view_Book]";
            decimal _OBal = 0.00M; // Opening Balance
            List<BookView> _List = [];

            // Get Opening Balance
            #region Opening Balance
            string _QueryOB = $"{_Query} WHERE Date([Vou_Date]) < '{_Date1}' AND [BookID] = {_BookID}";
            DataTable _OBalTable = Source.GetTable(_QueryOB);
            if (_OBalTable != null && _OBalTable.Rows.Count > 0)
            {
                decimal _TotDR = _OBalTable.AsEnumerable().Sum(x => x.Field<decimal>("DR"));
                decimal _TotCR = _OBalTable.AsEnumerable().Sum(x => x.Field<decimal>("CR"));
                _OBal = _TotCR - _TotDR;
                BookView _OBalRecord = new()
                {
                    ID = 0,
                    Vou_No = "OBAL",
                    Vou_Date = DT_Start,
                    Recevied = _TotCR,
                    Paid = _TotDR,
                    Balance = _OBal,
                    Description = "Opening Balance",
                    TReceived = _TotCR.ToString(Format.Digit),
                    TPaid = _TotDR.ToString(Format.Digit),
                    TBalance = _OBal.ToString(Format.Digit)
                };

                if (Pages.Current == 1)
                {
                    _List.Add(_OBalRecord);         // Add opening balance record in first page
                }
            }
            #endregion

            // Fatch Book Records between Date Range

            string _Filter = $"Date([Vou_Date]) BETWEEN '{_Date1}' AND '{_Date2}'";
            if (SearchText.Length > 0)
            {
                _Filter += $" AND ([Vou_No] LIKE '%{SearchText}%' OR [Description] LIKE '%{SearchText}%')";
            }
            string _Sorting = $"[Vou_Date], [Vou_No] ";
            string _Limit = $"LIMIT {Pages.Size} OFFSET {(Pages.Current - 1) * Pages.Size}";

            TotalRecord = Source.RecordCound(Enums.Tables.view_Book, _Filter) + 1;   // +1 for Opening Balance
            var _Data = Source.GetTable(_Query, _Filter, _Sorting + _Limit);

            if (_Data != null)
            {
                decimal _Bal = _OBal;
                decimal _DR = 0.00M;
                decimal _CR = 0.00M;

                foreach (DataRow Row in _Data.Rows)
                {
                    _DR = Row.Field<decimal>("DR");
                    _CR = Row.Field<decimal>("CR");
                    _Bal += _CR - _DR;

                    var _Record = new BookView()
                    {
                        ID = Row.Field<int>("ID1"),
                        Vou_No = Row.Field<string>("Vou_No") ?? "---",
                        Vou_Date = Row.Field<DateTime>("Vou_Date"),
                        Recevied = _CR,
                        Paid = _DR,
                        Balance = _Bal,
                        Description = Row.Field<string>("Description") ?? "",

                        TReceived = _CR.ToString(Format.Digit),
                        TPaid = _DR.ToString(Format.Digit),
                        TBalance = _Bal.ToString(Format.Digit)
                    };

                    _List.Add(_Record);
                }

                
                Pages.Refresh(TotalRecord);

                return _List;
            }
            return [];
        }
               

        #region Print
        public void Print(ReportType _ReportType)
        {

            try
            {
                ReportService = new(AppGlobal);
                ReportService.ReportType = _ReportType;
                GetReportData();
                ReportModel();                              // Add / update Report model data.
                ReportService.Print();
            }
            catch (Exception error)
            {
                MsgClass.Add(error.Message);
            }
        }

        public void GetReportData()
        {
            ReportService.Data.ReportTable = Source.GetBookVoucher(VoucherID);
            ReportService.Data.DataSetName = "ds_CashBank";   // ds_CashBank

        }

        public void ReportModel()
        {
            var _VoucherNo = ReportService.Data.ReportTable.Rows[0]["Vou_No"].ToString();
            var _Heading1 = $"General Ledger {BookNatureTitle}";
            var _Heading2 = $"Voucher {_VoucherNo}";

            ReportService.Model.ReportDataSource = ReportService.Data;                   // Load Reporting Data to Report Model

            ReportService.Model.InputReport.FileName = "CashBankBook.rdl";

            ReportService.Model.OutputReport.ReportType = ReportService.ReportType;
            ReportService.Model.OutputReport.FileName = $"Book_{_VoucherNo}" +
                $"{ReportService.Model.OutputReport.FileExt}";          // without Extention

            ReportService.Model.AddReportParameter("Heading1", _Heading1);
            ReportService.Model.AddReportParameter("Heading2", _Heading2);
            ReportService.Model.AddReportParameter("InWords", "Words");
            ReportService.Model.AddReportParameter("CurrencySign", "SAR");
            ReportService.Model.AddReportParameter("ShowImages", true.ToString());

        }
        #endregion

        #region Edit Book Voucher Re-direct to Book Page
        public void Edit(int _ID)
        {
            SetKeys();
            AppGlobal.NavManager.NavigateTo($"/Accounts/Books/{BookID}/{BookNatureID}");
        }
        #endregion

        #region Get & Set Keys
        internal void SetKeys()
        {
            if (!string.IsNullOrEmpty(Source.DBFile))
            {
                AppRegistry.SetKey(Source.DBFile, "BkNatureID", BookNatureID, KeyType.Number);
                AppRegistry.SetKey(Source.DBFile, "BkBook", BookID, KeyType.Number, "Cash / Bank BooK");
                AppRegistry.SetKey(Source.DBFile, "BkBook", DT_Start, KeyType.From);
                AppRegistry.SetKey(Source.DBFile, "BkBook", DT_End, KeyType.To);
                AppRegistry.SetKey(Source.DBFile, "BkBook", SearchText, KeyType.Text);
            }
        }

        internal void GetKeys()
        {
            if (!string.IsNullOrEmpty(Source.DBFile))
            {
                BookNatureID = AppRegistry.GetNumber(Source.DBFile, "BKNatureID");
                BookID = AppRegistry.GetNumber(Source.DBFile, "BKbook");
                DT_Start = AppRegistry.GetFrom(Source.DBFile, "BKbook");
                DT_End = AppRegistry.GetTo(Source.DBFile, "BKbook");
                SearchText = AppRegistry.GetText(Source.DBFile, "BKBook");
            }
        }
        #endregion


    }           // END
}


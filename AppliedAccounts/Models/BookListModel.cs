using AppliedDB;
using Messages = AppMessages.Enums.Messages;
using System.Data;
using AppliedAccounts.Services;
using AppliedAccounts.Data;
using AppliedAccounts.Pages.Accounts;
using AppReports;
using Windows.Services.Maps;

namespace AppliedAccounts.Models
{
    public class BookListModel  // : IVoucherList
    {
        public GlobalService AppGlobals { get; set; }
        public List<CodeTitle> BookList { get; set; }
        public List<CodeTitle> NatureAccountsList { get; set; }
        public DataSource Source { get; set; }
        public AppUserModel? UserProfile { get; set; }
        public AppMessages.MessageClass MsgClass { get; set; }
        public PrintService ReportService { get; set; }

        public int BookID { get; set; }
        public int BookNatureID { get; set; }
        public int VoucherID { get; set; }
        public DateTime DT_Start { get; set; }
        public DateTime DT_End { get; set; }
        public string SearchText { get; set; }
        public string BookNatureTitle = "Book Title";
        public List<BookView> BookRecords { get; set; }
        public bool PageIsValid { get; set; } = false;

        public BookListModel() { }
        public BookListModel(int _BookID, AppUserModel _AppUserProfile)
        {
            MsgClass = new();
            UserProfile = _AppUserProfile;
            Source = new(UserProfile);
            GetKeys();

            try
            {

                if (_BookID == 0) { BookID = 1; } else { BookID = _BookID; }


                // Get a Nature of Book.  It is Cash Book  or Bank book...
                var result = Source?.SeekValue(Enums.Tables.COA, BookID, "Nature") ?? 0;
                BookNatureID = (int)result;

                BookID = _BookID;
                UserProfile = _AppUserProfile;

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


        public bool LoadData()
        {
            try
            {
                BookRecords = LoadBookRecords(BookID);
                return true;
            }
            catch (Exception)
            {
                MsgClass.Add(Messages.DataLoadFailed);
                return false;
            }
        }


        public List<BookView> LoadBookRecords(int _BookID)     // Load List of Cash / Bank Book record in Table
        {
            var _List = new List<BookView>();
            var _Data = Source.GetBookList(_BookID);

            if (_Data != null)
            {
                decimal _Bal = 0.00M;
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
                return _List;
            }
            return [];
        }


        #region Filter on Data List showing in table
        public string GetFilterText()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Print
        public void Print(ReportType _ReportType)
        {
            
            try
            {
                ReportService = new(AppGlobals);
                ReportService.ReportType = _ReportType;
                ReportService.Data = GetReportData();
                ReportModel();                              // Add / update Report model data.
                ReportService.Print();

              



            }
            catch (Exception error)
            {
                MsgClass.Add(error.Message);
            }

        }

        public ReportData GetReportData()
        {
            ReportData reportData = new(); ;
            reportData.ReportTable = Source.GetBookVoucher(VoucherID);
            reportData.DataSetName = "ds_CashBank";   // ds_CashBank

            return reportData;
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
            AppGlobals.NavManager.NavigateTo($"/Accounts/Books/{BookID}/{BookNatureID}");
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


using AppliedAccounts.Pages.Accounts.Post;
using AppliedAccounts.Pages.Sale.Quotations;
using AppliedAccounts.Services;
using AppliedDB;
using AppMessages;
using Microsoft.AspNetCore.Components;
using Org.BouncyCastle.Crypto.Modes.Gcm;
using System.Data;
using VoucherPosting;
using static AppliedAccounts.Pages.Accounts.Post.Posting;
using static AppliedDB.Enums;
using static AppMessages.Enums;


namespace AppliedAccounts.Models.Posting
{
    public class PostingModel
    {
        [Inject] public GlobalService AppGlobal { get; set; } = default!;
        public MessageClass MsgClass { get; set; } = new();
        public List<DataListModel> DataListModelList { get; set; } = new();
        public bool IsPosting { get; set; } = false;
        public DataSource Source { get; set; }
        public string Filter { get; set; } = string.Empty;
        public PostingTypes PostType { get; set; } = 0;
        public PageModel Pages { get; set; } = new();

        public PostingModel()
        {
            if (AppGlobal != null) { Source = new(AppGlobal.AppPaths); }
           

        }

        



        #region Load Data

        public async Task LoadData(PostingViewModel PostingModel)
        {
            Source ??= new(AppGlobal.AppPaths);
            //Source = AppGlobal.Source;

            if (PostingModel.PostingType == 0) { return; }

            DateTime Date1 = PostingModel.Dt_From;
            DateTime Date2 = PostingModel.Dt_To;
            List<string> conditions = new();

            switch (PostingModel.PostingType)
            {
                // Cash Books
                case PostingTypes.CashBook:

                    if (PostingModel.PostingStatus == 1) { conditions.Add($"Status='{PostingStatus.Submitted}'"); }
                    if (PostingModel.PostingStatus == 2) { conditions.Add($"Status='{PostingStatus.Posted}'"); }

                    conditions.Add($"Date(Vou_Date) >= '{Date1:yyyy-MM-dd}'");
                    conditions.Add($"Date(Vou_Date) <= '{Date2:yyyy-MM-dd}'");

                    string Filter = string.Join(" AND ", conditions);

                    string _Limit = $"LIMIT {Pages.Size} OFFSET {(Pages.Current - 1) * Pages.Size}";

                    var _CashAccList = Source.GetTable(SQLQueries.Quries.GetCashAccounts());
                    if (_CashAccList.Rows.Count > 0)
                    {
                        var CashAccIDs = string.Join(",", _CashAccList.AsEnumerable().Select(r => r.Field<long>("ID")));
                        Filter = $"BookID IN ({CashAccIDs}) AND {Filter}";
                        //Pages.TotalRecords = Source.GetCount(Tables.Book, Filter);
                    }

                    _Limit = $"LIMIT {Pages.Size} OFFSET {(Pages.Current - 1) * Pages.Size}";

                    var _DataTableCash = Source.GetTable(Tables.Book, $"{Filter} {_Limit}");
                    DataListModelList = GetPostingTable(_DataTableCash);
                    Pages.Refresh(Source.GetCount(Tables.Book, Filter));
                    
                    break;

                // Bank Books
                case PostingTypes.BankBook:
                    conditions = new();

                    if (PostingModel.PostingStatus == 1) { conditions.Add($"Status='{PostingStatus.Submitted}'"); }
                    if (PostingModel.PostingStatus == 2) { conditions.Add($"Status='{PostingStatus.Posted}'"); }

                    conditions.Add($"Date(Vou_Date) >= '{Date1:yyyy-MM-dd}'");
                    conditions.Add($"Date(Vou_Date) <= '{Date2:yyyy-MM-dd}'");

                    Filter = string.Join(" AND ", conditions);

                    var _BankAccList = Source.GetTable(SQLQueries.Quries.GetBankAccounts());
                    if (_BankAccList.Rows.Count > 0)
                    {
                        var BankAccIDs = string.Join(",", _BankAccList.AsEnumerable().Select(r => r.Field<long>("ID")));
                        Filter = $"BookID IN ({BankAccIDs}) AND {Filter} ";
                    }

                    _Limit = $"LIMIT {Pages.Size} OFFSET {(Pages.Current - 1) * Pages.Size}";
                    var _DataTableBank = Source.GetTable(Tables.Book, $"{Filter} {_Limit}" );
                    DataListModelList = GetPostingTable(_DataTableBank);
                    Pages.Refresh(Source.GetCount(Tables.Book, Filter));

                    break;
                case PostingTypes.WriteCheques:
                    DataListModelList.Clear();
                    break;

                case PostingTypes.BillPayable:
                    DataListModelList.Clear();
                    break;

                case PostingTypes.BillReceivable:
                    DataListModelList.Clear();
                    break;

                case PostingTypes.Receipt:
                    DataListModelList.Clear();
                    break;

                case PostingTypes.Payment:
                    DataListModelList.Clear();
                    break;

                case PostingTypes.SalesReturn:
                    DataListModelList.Clear();
                    break;


                case PostingTypes.Production:
                    DataListModelList.Clear();
                    break;

                default:
                    DataListModelList.Clear();
                    break;
            }
            await Task.Delay(100);
        }

        private List<DataListModel> GetPostingTable(DataTable dataTable)
        {

            var _List = new List<DataListModel>();

            try
            {
                // Cash book
                if (dataTable.TableName == Tables.Book.ToString())
                {
                    //DataListModelList = CreatePostingTable(PostType);

                    foreach (DataRow Row in dataTable.Rows)
                    {
                        var Row1 = Source.RemoveNullValues(Row);

                        long ID = Row1.Field<long>("ID");
                        long BookID = Row1.Field<long>("BookID");

                        var _DataList = new DataListModel();
                        _DataList.ID = ID;
                        _DataList.Vou_No = Row1.Field<string>("Vou_No") ?? string.Empty;
                        _DataList.Vou_Date = Row1.Field<DateTime>("Vou_Date");
                        _DataList.Title = Source.SeekTitle(Tables.COA, BookID);
                        _DataList.DR = Row1.Field<decimal>("Amount") <= 0 ? Row1.Field<decimal>("Amount") : 0.0M;
                        _DataList.CR = Row1.Field<decimal>("Amount") > 0 ? Row1.Field<decimal>("Amount") : 0.0M;
                        _DataList.Status = Row1.Field<string>("Status") ?? "Submitted";
                        _DataList.Selected = false;

                        _List.Add(_DataList);

                    }
                }

            }
            catch (Exception ex)
            {
                MsgClass.Critical(ex.Message);
                throw;
            }

            
            return [.. _List.OrderBy(e => e.Vou_Date).ThenBy(e => e.Vou_No)];

        }

        
        #endregion


        #region Voucher Posting

        public async Task DoVoucherPosting(long _VouID, PostingViewModel PostingModel)
        {
            if (PostingModel.PostingType == 0) { return; }         // Return if type not assigned.

            // Cash Book Posting
            if (PostingModel.PostingType == PostingTypes.CashBook)
            {
                VoucherPostingModel postingModel = new();

                postingModel.MasterTable = Source.GetTable(AppliedDB.Enums.Tables.Book, $"ID={_VouID}");
                postingModel.DetailTable = Source.GetTable(AppliedDB.Enums.Tables.Book2, $"TranID={_VouID}");

                MsgClass.ClearMessages();                            // Clear all previous messages. 
                CashBook postCashBook = new(Source, postingModel);
                await postCashBook.DoCashPosting();                  // Cash Posting main method.
                if (postCashBook.PostSuccessful)
                {
                    MsgClass.Success(Messages.Save);        // add message after Save selected Vouchers.
                    LoadData(PostingModel);                    // Refresh display Data afger save voucher.
                }
                else
                {
                    MsgClass = postCashBook.MsgClass;
                }
            }

            // Bank Book Posting
            if (PostingModel.PostingType == PostingTypes.BankBook)
            {
                VoucherPostingModel postingModel = new();

                postingModel.MasterTable = Source.GetTable(AppliedDB.Enums.Tables.Book, $"ID={_VouID}");
                postingModel.DetailTable = Source.GetTable(AppliedDB.Enums.Tables.Book2, $"TranID={_VouID}");

                MsgClass.ClearMessages();                           // Clear all previous messages. 
                CashBook postBankBook = new(Source, postingModel);
                await postBankBook.DoBankPosting();                  // Bank Posting main method.
                if (postBankBook.PostSuccessful)
                {
                    MsgClass.Success(Messages.Save);        // add message after Save selected Vouchers.
                    LoadData(PostingModel);              // Refresh display Data afger save voucher.
                }
                else
                {
                    MsgClass = postBankBook.MsgClass;
                }
            }

            // Bill Receivable Posting  
            if (PostingModel.PostingType == PostingTypes.BillReceivable)
            {
                return;
            }
            return;
        }

        #endregion


        #region Model  razor page view in Table Tax

        public class DataListModel
        {
            public long ID { get; set; }
            public string Vou_No { get; set; }
            public DateTime Vou_Date { get; set; }

            public string Title { get; set; }
            public decimal DR { get; set; }
            public decimal CR { get; set; }
            public decimal Amount { get; set; }

            public string Status { get; set; }
            public bool Active { get; set; }
            public bool Selected { get; set; }

        }


        #endregion

        
    }
}

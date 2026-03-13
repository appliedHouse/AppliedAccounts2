using AppliedAccounts.Pages.Accounts.Post;
using AppliedAccounts.Services;
using AppliedDB;
using AppMessages;
using Microsoft.AspNetCore.Components;
using System.Data;
using VoucherPosting;
using static AppliedDB.Enums;
using static AppMessages.Enums;

namespace AppliedAccounts.Models.Posting
{
    public class UnPostModel
    {
        [Inject] public GlobalService AppGlobal { get; set; } = default!;
        public UnPostViewModel UnPostVM { get; set; } = new();
        public MessageClass MsgClass { get; set; } = new();
        public List<DataListModel> DataListModelList { get; set; } = new();
        public bool IsPosting { get; set; } = false;
        public DataSource Source { get; set; }
        public string Filter { get; set; } = string.Empty;
        public string Sort {get; set; } = "Vou_Date, Vou_No";
        public int PostType { get; set; } = 0;

        public PageModel Pages { get; set; } = new();

        public DateTime[] FilterDates { get; set; } = { DateTime.Now, DateTime.Now };

        public UnPostModel()
        {
            if (AppGlobal != null) { Source = new(AppGlobal.AppPaths); }
            Pages.PageChanged += OnPageChangedInternal;
        }


        private async void OnPageChangedInternal(int page)
        {
            if (UnPostVM is null) { return; }
            await LoadData();

        }

        #region Load Data
        public async Task LoadData()
        {
            await LoadData(UnPostVM);
        }


        public async Task LoadData(UnPostViewModel _UnPostVM)
        {
            Source ??= new(AppGlobal.AppPaths);
            UnPostVM = _UnPostVM;

            FilterDates[0] = UnPostVM.Dt_From;
            FilterDates[1] = UnPostVM.Dt_To;


            if (UnPostVM.PostingType == 0) { return; }

            switch (UnPostVM.PostingType)
            {
                // Cash Books
                case PostingTypes.CashBook:
                    Filter = "";
                    var _CashAccList = Source.GetTable(SQLQueries.Quries.GetCashAccounts());
                    if (_CashAccList.Rows.Count > 0)
                    {
                        var CashAccIDs = string.Join(",", _CashAccList.AsEnumerable().Select(r => r.Field<long>("ID")));
                        Filter = $"BookID IN ({CashAccIDs}) AND [Status] = 'Posted' AND ";
                        Filter += AppliedDB.Functions.GetDateFilter(FilterDates);
                        
                    }
                    var _Sort = Sort + Pages.GetLimit();            // Add pagination filter to select records / rows.
                    var _DataTableCash = Source.GetTable(Tables.Book, Filter, _Sort);
                    DataListModelList = GetPostingTable(_DataTableCash);
                    Pages.Refresh(Source.GetCount(Tables.Book, Filter));

                    break;

                // Bank Books
                case PostingTypes.BankBook:
                    Filter = "";
                    var _BankAccList = Source.GetTable(SQLQueries.Quries.GetBankAccounts());
                    if (_BankAccList.Rows.Count > 0)
                    {
                        var BankAccIDs = string.Join(",", _BankAccList.AsEnumerable().Select(r => r.Field<long>("ID")));
                        Filter = $"BookID IN ({BankAccIDs}) AND [Status] = 'Posted' AND ";
                        Filter += AppliedDB.Functions.GetDateFilter(FilterDates);
                       

                    }
                    _Sort = Sort + Pages.GetLimit();            // Add pagination filter to select records / rows.
                    var _DataTableBank = Source.GetTable(Tables.Book, Filter, _Sort);
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
            //await Task.Delay(100);
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
            return _List;

        }


        #endregion



        #region Voucher UnPost

        public async Task DoVoucherUnPost(long _VouID, PostingTypes _PostType)
        {
            if (_PostType == 0) { return; }         // Return if type not assigned.

            // Cash Book Posting
            if (_PostType == PostingTypes.CashBook)
            {
                VoucherPostingModel postingModel = new();

                postingModel.MasterTable = Source.GetTable(Tables.Book, $"ID={_VouID}");
                postingModel.DetailTable = Source.GetTable(Tables.Book2, $"TranID={_VouID}");

                MsgClass.ClearMessages();                            // Clear all previous messages. 
                CashBook postCashBook = new(Source, postingModel);
                await postCashBook.DoCashUnPost();                  // Cash Posting main method.
                if (postCashBook.UnPostSuccessful)
                {
                    MsgClass.Success(Messages.Save);        // add message after Save selected Vouchers.
                    await LoadData();              // Refresh display Data afger save voucher.
                }
                else
                {
                    MsgClass = postCashBook.MsgClass;
                }
            }

            // Bank Book Posting
            if (_PostType == PostingTypes.BankBook)
            {
                VoucherPostingModel postingModel = new();

                postingModel.MasterTable = Source.GetTable(Tables.Book, $"ID={_VouID}");
                postingModel.DetailTable = Source.GetTable(Tables.Book2, $"TranID={_VouID}");

                MsgClass.ClearMessages();                           // Clear all previous messages. 
                CashBook postBankBook = new(Source, postingModel);
                // Cash & Bank Voucher data table is same. so here using same fucntion as using for cash
                await postBankBook.DoCashUnPost();                   
                if (postBankBook.PostSuccessful)
                {
                    MsgClass.Success(Messages.Save);        // add message after Save selected Vouchers.
                    await LoadData();              // Refresh display Data afger save voucher.
                }
                else
                {
                    MsgClass = postBankBook.MsgClass;
                }
            }

            // Bill Receivable Posting  
            if (_PostType == PostingTypes.BillReceivable)
            {
                BillReceivable UnPostBillReceivable = new();

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

using AppliedAccounts.Pages.Accounts.Post;
using AppliedAccounts.Services;
using AppliedDB;
using System.Data;
using VoucherPosting;
using static AppliedAccounts.Pages.Accounts.Post.Posting;
using static AppliedDB.Enums;
using static AppMessages.Enums;

namespace AppliedAccounts.Models.Posting
{
    public class UnPostModel
    {
        public GlobalService AppGlobal { get; set; }
        public UnPostViewModel UnPostVM { get; set; } = new();
        public MessagesService MsgService { get; set; }
        public List<DataListModel> DataListModelList { get; set; } = new();
        public bool IsPosting { get; set; } = false;
        public DataSource Source { get; set; }
        public string Filter { get; set; } = string.Empty;
        public string Sort { get; set; } = "Vou_Date, Vou_No";
        public int PostType { get; set; } = 0;
        private Dictionary<long, string> _coaCache = new();
        private List<long> _cashIds;
        private List<long> _bankIds;

        public PageModel Pages { get; set; } = new();

        public DateTime[] FilterDates { get; set; } = { DateTime.Now, DateTime.Now };

        public UnPostModel(GlobalService appGlobal)
        {
            AppGlobal = appGlobal;
            Source = new(appGlobal.AppPaths);
            MsgService = appGlobal.MsgService;
        }

        public void Init()
        {
            Source ??= new DataSource(AppGlobal.AppPaths);

            if (_coaCache.Count == 0)
                LoadCOA();

            _cashIds ??= LoadAccountIds(SQLQueries.Quries.GetCashAccounts());
            _bankIds ??= LoadAccountIds(SQLQueries.Quries.GetBankAccounts());
        }

        #region Load Data
        //public async Task LoadData()
        //{
        //    await LoadData(UnPostVM);
        //}


        public async Task LoadData(UnPostViewModel model)
        {
            Init();

            if (model.PostingType == 0)
            {
                DataListModelList.Clear();
                return;
            }

            string filter = BuildFilter(model);
            string paging = BuildPaging();

            List<long> ids = model.PostingType switch
            {
                PostingTypes.CashBook => _cashIds,
                PostingTypes.BankBook => _bankIds,
                _ => null
            };

            if (ids == null || ids.Count == 0)
            {
                DataListModelList.Clear();
                return;
            }

            string finalFilter = $"BookID IN ({string.Join(",", ids)}) AND {filter} {paging}";

            var table = Source.GetTable(Tables.Book, finalFilter);

            DataListModelList = Map(table);

            Pages.Refresh(Source.GetCount(Tables.Book, $"BookID IN ({string.Join(",", ids)}) AND {filter}"));


            #region Temp
            //Source ??= new(AppGlobal.AppPaths);
            //UnPostVM = _UnPostVM;

            //FilterDates[0] = UnPostVM.Dt_From;
            //FilterDates[1] = UnPostVM.Dt_To;


            //if (UnPostVM.PostingType == 0) { return; }

            //switch (UnPostVM.PostingType)
            //{
            //    // Cash Books
            //    case PostingTypes.CashBook:
            //        Filter = "";
            //        var _CashAccList = Source.GetTable(SQLQueries.Quries.GetCashAccounts());
            //        if (_CashAccList.Rows.Count > 0)
            //        {
            //            var CashAccIDs = string.Join(",", _CashAccList.AsEnumerable().Select(r => r.Field<long>("ID")));
            //            Filter = $"BookID IN ({CashAccIDs}) AND [Status] = 'Posted' AND ";
            //            Filter += Functions.GetDateFilter(FilterDates);
            //        }
            //        var _Sort = Sort + Pages.GetLimit();            // Add pagination filter to select records / rows.
            //        var _DataTableCash = Source.GetTable(Tables.Book, Filter, _Sort);
            //        DataListModelList = GetPostingTable(_DataTableCash);
            //        Pages.Refresh(Source.GetCount(Tables.Book, Filter));

            //        break;

            //    // Bank Books
            //    case PostingTypes.BankBook:
            //        Filter = "";
            //        var _BankAccList = Source.GetTable(SQLQueries.Quries.GetBankAccounts());
            //        if (_BankAccList.Rows.Count > 0)
            //        {
            //            var BankAccIDs = string.Join(",", _BankAccList.AsEnumerable().Select(r => r.Field<long>("ID")));
            //            Filter = $"BookID IN ({BankAccIDs}) AND [Status] = 'Posted' AND ";
            //            Filter += AppliedDB.Functions.GetDateFilter(FilterDates);


            //        }
            //        _Sort = Sort + Pages.GetLimit();            // Add pagination filter to select records / rows.
            //        var _DataTableBank = Source.GetTable(Tables.Book, Filter, _Sort);
            //        DataListModelList = GetPostingTable(_DataTableBank);
            //        Pages.Refresh(Source.GetCount(Tables.Book, Filter));

            //        break;



            //case PostingTypes.WriteCheques:
            //    DataListModelList.Clear();
            //    break;

            //case PostingTypes.BillPayable:
            //    DataListModelList.Clear();
            //    break;

            //case PostingTypes.BillReceivable:
            //    DataListModelList.Clear();
            //    break;

            //case PostingTypes.Receipt:
            //    DataListModelList.Clear();
            //    break;

            //case PostingTypes.Payment:
            //    DataListModelList.Clear();
            //    break;

            //case PostingTypes.SalesReturn:
            //    DataListModelList.Clear();
            //    break;

            //case PostingTypes.Production:
            //    DataListModelList.Clear();
            //    break;


            //default:
            //    DataListModelList.Clear();
            //    break;
            //}
            #endregion


            await Task.CompletedTask;
            //await Task.Delay(100);
        }

        private string BuildFilter(UnPostViewModel model)
        {
            var conditions = new List<string>();

            if (model.PostingStatus == 1)
                conditions.Add($"Status='{PostingStatus.Submitted}'");

            if (model.PostingStatus == 2)
                conditions.Add($"Status='{PostingStatus.Posted}'");

            var from = model.Dt_From.Date;
            var to = model.Dt_To.Date.AddDays(1);

            conditions.Add($"Vou_Date >= '{from:yyyy-MM-dd HH:mm:ss}'");
            conditions.Add($"Vou_Date < '{to:yyyy-MM-dd HH:mm:ss}'");

            return string.Join(" AND ", conditions);
        }

        private string BuildPaging()
        {
            return $"ORDER BY Vou_Date, Vou_No LIMIT {Pages.Size} OFFSET {(Pages.Current - 1) * Pages.Size}";
        }

        private void LoadCOA()
        {
            var table = Source.GetTable(Tables.COA);

            _coaCache = table.AsEnumerable()
                .ToDictionary(
                    r => r.Field<long>("ID"),
                    r => r.Field<string>("Title") ?? ""
                );
        }

        private List<long> LoadAccountIds(string query)
        {
            return Source.GetTable(query)
                .AsEnumerable()
                .Select(r => r.Field<long>("ID"))
                .ToList();
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
                MsgService.Error(ex);
                throw;
            }
            return _List;

        }

        private List<DataListModel> Map(DataTable table)
        {
            var list = new List<DataListModel>(table.Rows.Count);

            foreach (DataRow row in table.Rows)
            {
                long bookId = row.Field<long>("BookID");
                decimal amount = row.Field<decimal>("Amount");

                list.Add(new DataListModel
                {
                    ID = row.Field<long>("ID"),
                    Vou_No = row.Field<string>("Vou_No") ?? "",
                    Vou_Date = row.Field<DateTime>("Vou_Date"),
                    Title = _coaCache.TryGetValue(bookId, out var title) ? title : "",
                    DR = amount <= 0 ? amount : 0,
                    CR = amount > 0 ? amount : 0,
                    Status = row.Field<string>("Status") ?? "Submitted",
                    Selected = false
                });
            }

            return list;
        }

        #endregion



        #region Voucher UnPost

        public async Task<bool> DoVoucherPosting(long vouId, UnPostViewModel model)
        {
            MsgService.Clear();

            if (model.PostingType == 0)
            {
                MsgService.Danger(Messages.PostingTypeNotDefined);
                return false;
            }

            var postingModel = new VoucherPostingModel
            {
                MasterTable = Source.GetTable(Tables.Book, $"ID={vouId}"),
                DetailTable = Source.GetTable(Tables.Book2, $"TranID={vouId}")
            };

            if (postingModel.MasterTable.Rows.Count == 0)
            {
                MsgService.Danger(Messages.PostingMasterRecordNotFound);
                return false;
            }

            if (postingModel.DetailTable.Rows.Count == 0)
            {
                MsgService.Danger(Messages.PostingDetailRecordNotFound);
                return false;
            }

            var post = new PostCashBook(Source, postingModel, MsgService.MsgClass);
            var result = false;

            switch (model.PostingType)
            {
                case PostingTypes.CashBook:
                    result = await post.DoCashUnPost();
                    break;

                case PostingTypes.BankBook:
                    result = await post.DoCashUnPost();
                    break;

                default:
                    return false;
            }

            MsgService.MsgClass.AddRange(post.MsgClass);

            if (result)
            {
                MsgService.Clear();
                MsgService.Success(Messages.Posted);
                await LoadData(model);
                return true;
            }
            else
            {
                MsgService.AddRange(post.MsgClass);
                MsgService.Critical(Messages.NotSave);
                return false;
            }
        }

        internal async Task DoVoucherUnPost(long id, PostingTypes postingType)
        {
            throw new NotImplementedException();
        }

        //public async Task<bool> DoVoucherUnPost(long _VouID, PostingTypes _PostType)
        //{
        //    if (_PostType == 0) { return false; }         // Return if type not assigned.

        //    // Cash Book Posting
        //    if (_PostType == PostingTypes.CashBook)
        //    {
        //        VoucherPostingModel postingModel = new(); ;

        //        postingModel.MasterTable = Source.GetTable(Tables.Book, $"ID={_VouID}");
        //        postingModel.DetailTable = Source.GetTable(Tables.Book2, $"TranID={_VouID}");

        //        if (postingModel.MasterTable.Rows.Count == 0)
        //        {
        //            MsgService.Warning(Messages.VoucherNotFound);
        //            return false;
        //        }

        //        if (postingModel.DetailTable.Rows.Count == 0)
        //        {
        //            MsgService.Warning(Messages.PostingDetailRecordNotFound);
        //            return false;
        //        }


        //        MsgService.Clear();                            // Clear all previous messages. 
        //        PostCashBook postCashBook = new(Source, postingModel);
        //        await postCashBook.DoCashUnPost();                  // Cash Posting main method.
        //        //if (postCashBook.UnPostSuccessful)
        //        //{
        //        //    MsgService.Success(Messages.Saved);        // add message after Save selected Vouchers.
        //        //    await LoadData();                          // Refresh display Data afger save voucher.
        //        //    return true;
        //        //}
        //        //else
        //        //{
        //        //    MsgService.MsgClass.AddRange(postCashBook.MsgClass);
        //        //    return false;
        //        //}
        //    }

        //    // Bank Book Posting
        //    if (_PostType == PostingTypes.BankBook)
        //    {
        //        VoucherPostingModel postingModel = new();

        //        postingModel.MasterTable = Source.GetTable(Tables.Book, $"ID={_VouID}");
        //        postingModel.DetailTable = Source.GetTable(Tables.Book2, $"TranID={_VouID}");

        //        if (postingModel.MasterTable.Rows.Count == 0)
        //        {
        //            MsgService.Warning(Messages.VoucherNotFound);
        //            return false;
        //        }


        //        MsgService.Clear();                           // Clear all previous messages. 
        //        PostCashBook postBankBook = new(Source, postingModel);
        //        // Cash & Bank Voucher data table is same. so here using same fucntion as using for cash
        //        await postBankBook.DoCashUnPost();
        //        if (postBankBook.PostSuccessful)
        //        {
        //            MsgService.Success(Messages.Saved);        // add message after Save selected Vouchers.
        //            await LoadData();                          // Refresh display Data afger save voucher.
        //            return true;
        //        }
        //        else
        //        {
        //            MsgService.MsgClass.AddRange(postBankBook.MsgClass);
        //            return false;
        //        }
        //    }

        //    // Bill Receivable Posting  
        //    if (_PostType == PostingTypes.BillReceivable)
        //    {
        //        PostBillReceivable UnPostBillReceivable = new();

        //        return false;
        //    }
        //    return false;
        //}

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

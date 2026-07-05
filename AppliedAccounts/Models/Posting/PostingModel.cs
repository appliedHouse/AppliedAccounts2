using AppliedAccounts.Pages.Accounts.Post;
using AppliedAccounts.Services;
using AppliedDB;
using AppMessages;
using Microsoft.AspNetCore.Components;
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
        public bool IsPosting { get; set; }
        public DataSource Source { get; set; }
        public PageModel Pages { get; set; } = new();

        private Dictionary<long, string> _coaCache = new();
        private List<long> _cashIds;
        private List<long> _bankIds;

        #region Init

        public void Init()
        {
            Source ??= new DataSource(AppGlobal.AppPaths);

            if (_coaCache.Count == 0)
                LoadCOA();

            _cashIds ??= LoadAccountIds(SQLQueries.Quries.GetCashAccounts());
            _bankIds ??= LoadAccountIds(SQLQueries.Quries.GetBankAccounts());
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

        #endregion

        #region Load Data

        public async Task LoadData(PostingViewModel model)
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

            await Task.CompletedTask;
        }

        private string BuildFilter(PostingViewModel model)
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

        #region Voucher Posting

        public async Task DoVoucherPosting(long vouId, PostingViewModel model)
        {
            if (model.PostingType == 0) return;

            var postingModel = new VoucherPostingModel
            {
                MasterTable = Source.GetTable(Tables.Book, $"ID={vouId}"),
                DetailTable = Source.GetTable(Tables.Book2, $"TranID={vouId}")
            };

            MsgClass.ClearMessages();

            var post = new CashBook(Source, postingModel);

            switch (model.PostingType)
            {
                case PostingTypes.CashBook:
                    await post.DoCashPosting();
                    break;

                case PostingTypes.BankBook:
                    await post.DoBankPosting();
                    break;

                default:
                    return;
            }

            if (post.PostSuccessful)
            {
                MsgClass.Success(Messages.Saved);
                await LoadData(model); // ✅ FIXED
            }
            else
            {
                MsgClass = post.MsgClass;
            }
        }

        #endregion

        #region Model

        public class DataListModel
        {
            public long ID { get; set; }
            public string Vou_No { get; set; }
            public DateTime Vou_Date { get; set; }
            public string Title { get; set; }
            public decimal DR { get; set; }
            public decimal CR { get; set; }
            public string Status { get; set; }
            public bool Selected { get; set; }
        }

        #endregion
    }
}

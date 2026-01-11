using AppliedAccounts.Services;
using AppliedDB;
using AppliedGlobals;
using AppMessages;
using Microsoft.AspNetCore.Components;
using System.Data;
using VoucherPosting;
using static AppMessages.Enums;


namespace AppliedAccounts.Models
{
    public class PostingModel
    {
        [Inject] public GlobalService AppGlobal { get; set; } = default!;
        public MessageClass MsgClass { get; set; } = new();
        public List<DataListModel> DataListModelList { get; set; }
        public bool IsPosting { get; set; } = false;
        public DataSource Source { get; set; }
        public string Filter { get; set; } = string.Empty;
        public int PostType { get; set; } = 0;

        public PostingModel()
        {
            if (AppGlobal != null) { Source = new(AppGlobal.AppPaths); }

        }

        #region Load Data
        public async Task LoadData(int PostingType)
        {
            Source ??= new(AppGlobal.AppPaths);

            if (PostingType == 0) { return; }

            switch (PostingType)
            {
                // Cash Books
                case 1:
                    Filter = "";
                    var _CashAccList = Source.GetTable(SQLQueries.Quries.GetCashAccounts());
                    if (_CashAccList.Rows.Count > 0)
                    {
                        var CashAccIDs = string.Join(",", _CashAccList.AsEnumerable().Select(r => r.Field<long>("ID")));
                        Filter = $"BookID IN ({CashAccIDs}) ";
                    }
                    var _DataTableCash = Source.GetTable(AppliedDB.Enums.Tables.Book, Filter);
                    DataListModelList = GetPostingTable(_DataTableCash);

                    break;

                // Bank Books
                case 2:
                    Filter = "";
                    var _BankAccList = Source.GetTable(SQLQueries.Quries.GetBankAccounts());
                    if (_BankAccList.Rows.Count > 0)
                    {
                        var BankAccIDs = string.Join(",", _BankAccList.AsEnumerable().Select(r => r.Field<long>("ID")));
                        Filter = $"BookID IN ({BankAccIDs}) ";
                    }
                    var _DataTableBank = Source.GetTable(AppliedDB.Enums.Tables.Book, Filter);
                    DataListModelList = GetPostingTable(_DataTableBank);
                    break;
                case 3:
                    DataListModelList.Clear();
                    break;

                case 4:
                    DataListModelList.Clear();
                    break;

                case 5:
                    DataListModelList.Clear();
                    break;

                case 6:
                    DataListModelList.Clear();
                    break;

                case 7:
                    DataListModelList.Clear();
                    break;

                case 8:
                    DataListModelList.Clear();
                    break;

                case 9:
                    DataListModelList.Clear();
                    break;

                case 10:
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

            // Cash book
            if (dataTable.TableName == AppliedDB.Enums.Tables.Book.ToString())
            {
                //DataListModelList = CreatePostingTable(PostType);

                foreach (DataRow Row in dataTable.Rows)
                {
                    long ID = Row.Field<long>("ID");

                    var _DataList = new DataListModel();
                    _DataList.ID = ID;
                    _DataList.Vou_No = Row.Field<string>("Vou_No") ?? string.Empty;
                    _DataList.Vou_Date = Row.Field<DateTime>("Vou_Date");
                    _DataList.Title = Source.SeekTitle(AppliedDB.Enums.Tables.COA, ID);
                    _DataList.DR = Row.Field<decimal>("Amount") <= 0 ? Row.Field<decimal>("Amount") : 0.0M;
                    _DataList.CR = Row.Field<decimal>("Amount") > 0 ? Row.Field<decimal>("Amount") : 0.0M;
                    _DataList.Status = Row.Field<string>("Status") ?? "Submitted";
                    _DataList.Selected = false;

                    _List.Add(_DataList);

                }
            }

            return _List;
        }


        #endregion


        #region Voucher Posting

        public async Task DoVoucherPosting(long _VouID, int _PostType)
        {
            VoucherPostingModel postingModel = new();

            postingModel.MasterTable = Source.GetTable(AppliedDB.Enums.Tables.Book, $"ID={_VouID}");
            postingModel.DetailTable = Source.GetTable(AppliedDB.Enums.Tables.Book2, $"TranID={_VouID}");

            if (_PostType == 0) { return; }
            if (_PostType == 1)
            {
                MsgClass.ClearMessages();                           // Clear all previous messages. 
                CashBook postCashBook = new(Source, postingModel);
                await postCashBook.PostCashBook();                  // Cash Posting main method.
                if (postCashBook.PostSuccessful)
                {
                    MsgClass.Success(Messages.Save);        // add message after Save selected Vouchers.
                    await LoadData(_PostType);              // Refresh display Data afger save voucher.
                }
                else
                {
                    MsgClass = postCashBook.MsgClass;
                }
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

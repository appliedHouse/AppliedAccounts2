using AppliedAccounts.Services;
using AppliedDB;
using AppMessages;
using Microsoft.AspNetCore.Components;
using System.Data;
using VoucherPosting;
using static AppMessages.Enums;

namespace AppliedAccounts.Models.UnPost
{
    public class UnPostModel
    {
        [Inject] public GlobalService AppGlobal { get; set; } = default!;
        public MessageClass MsgClass { get; set; } = new();
        public List<DataListModel> DataListModelList { get; set; } = new();
        public bool IsPosting { get; set; } = false;
        public DataSource Source { get; set; }
        public string Filter { get; set; } = string.Empty;
        public int PostType { get; set; } = 0;

        public UnPostModel()
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
                        Filter = $"BookID IN ({CashAccIDs}) AND [Status] = 'Posted' ";
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
                        Filter = $"BookID IN ({BankAccIDs}) AND [Status] = 'Posted' ";
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

            try
            {
                // Cash book
                if (dataTable.TableName == AppliedDB.Enums.Tables.Book.ToString())
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
                        _DataList.Title = Source.SeekTitle(AppliedDB.Enums.Tables.COA, BookID);
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



        #region Voucher Posting

        public async Task DoVoucherUnPost(long _VouID, int _PostType)
        {
            if (_PostType == 0) { return; }         // Return if type not assigned.

            // Cash Book Posting
            if (_PostType == (int)PostingTypes.CashBook)
            {
                VoucherPostingModel postingModel = new();

                postingModel.MasterTable = Source.GetTable(AppliedDB.Enums.Tables.Book, $"ID={_VouID}");
                postingModel.DetailTable = Source.GetTable(AppliedDB.Enums.Tables.Book2, $"TranID={_VouID}");

                MsgClass.ClearMessages();                            // Clear all previous messages. 
                CashBook postCashBook = new(Source, postingModel);
                await postCashBook.DoCashUnPost();                  // Cash Posting main method.
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

            // Bank Book Posting
            if (_PostType == (int)PostingTypes.BankBook)
            {
                VoucherPostingModel postingModel = new();

                postingModel.MasterTable = Source.GetTable(AppliedDB.Enums.Tables.Book, $"ID={_VouID}");
                postingModel.DetailTable = Source.GetTable(AppliedDB.Enums.Tables.Book2, $"TranID={_VouID}");

                MsgClass.ClearMessages();                           // Clear all previous messages. 
                CashBook postBankBook = new(Source, postingModel);
                await postBankBook.DoBankUnPost();                  // Bank Posting main method.
                if (postBankBook.PostSuccessful)
                {
                    MsgClass.Success(Messages.Save);        // add message after Save selected Vouchers.
                    await LoadData(_PostType);              // Refresh display Data afger save voucher.
                }
                else
                {
                    MsgClass = postBankBook.MsgClass;
                }
            }

            // Bill Receivable Posting  
            if (_PostType == (int)PostingTypes.BillReceivable)
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

        public enum PostingTypes
        {
            None = 0,
            CashBook = 1,
            BankBook = 2,
            WriteCheques = 3,
            BillPayable = 4,
            BillReceivable = 5,
            Payment = 6,
            Receipt = 7,
            JournalVoucher = 8,
            SalesReturn = 9,
            Production = 10,
        }
        #endregion
    }
}

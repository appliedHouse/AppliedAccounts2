using AppliedAccounts.Services;
using AppliedDB;
using AppMessages;
using Microsoft.AspNetCore.Components;
using System.Data;
using VoucherPosting;
using static AppMessages.Enums;

namespace AppliedAccounts.Models
{
    public class PostingModel
    {
        [Inject] GlobalService AppGlobal { get; set; } = default!;
        public MessageClass MsgClass { get; set; } = new();
        public DataTable PostTable { get; set; }
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
                case 1:
                    var _DataTable = Source.GetTable(AppliedDB.Enums.Tables.Book, Filter);
                    PostTable = GetPostingTable(_DataTable, false);          // show posted voucehr only  true

                    break;

                case 2:
                    break;
                case 3:
                    break;

                case 4:
                    break;

                case 5:
                    break;

                case 6:
                    break;

                case 7:
                    break;

                case 8:
                    break;

                case 9:
                    break;

                case 10:
                    break;


                default:
                    break;
            }




            await Task.Delay(100);
        }

        private DataTable GetPostingTable(DataTable dataTable, bool IsSubmittedOnly)
        {
            if (dataTable.TableName == AppliedDB.Enums.Tables.Book.ToString())
            {
                PostTable = CreatePostingTable(PostType);

                foreach (DataRow Row in dataTable.Rows)
                {
                    if (!IsSubmittedOnly)
                    {
                        if (Row.Field<string>("Status") != "Submitted") { continue; }
                        var ptRow = PostTable.NewRow();
                        ptRow["ID"] = Row.Field<long>("ID");
                        ptRow["Vou_Date"] = Row.Field<DateTime>("Vou_Date");
                        ptRow["Vou_No"] = Row.Field<string>("Vou_No");
                        ptRow["Title"] = Source.SeekTitle(AppliedDB.Enums.Tables.COA, Row.Field<long>("ID"));
                        ptRow["DR"] = Row.Field<decimal>("Amount") > 0 ? Row.Field<decimal>("Amount") : 0;
                        ptRow["CR"] = Row.Field<decimal>("Amount") < 0 ? Row.Field<decimal>("Amount") : 0;
                        ptRow["Status"] = Row.Field<string>("Status") ?? "Submitted";
                        ptRow["Post"] = false;
                        PostTable.Rows.Add(ptRow);
                    }
                }
            }

            return PostTable;
        }

        private DataTable CreatePostingTable(int postType)
        {
            var _Table = new DataTable();
            if (postType == 0) { _Table.Columns.Add("BookTitle"); }
            _Table.Columns.Add("ID", typeof(long));
            _Table.Columns.Add("Vou_Date", typeof(DateTime));
            _Table.Columns.Add("Vou_No", typeof(string));
            _Table.Columns.Add("Title", typeof(string));
            _Table.Columns.Add("DR", typeof(decimal));
            _Table.Columns.Add("CR", typeof(decimal));
            _Table.Columns.Add("Status", typeof(string));
            _Table.Columns.Add("Post", typeof(bool));
            return _Table;
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
                CashBook postCashBook = new(Source, postingModel);
                await postCashBook.PostCashBook();
                if (postCashBook.PostSuccessful)
                {
                    MsgClass.Success(Messages.Save);
                }
                else
                {
                    MsgClass = postCashBook.MsgClass;
                }
            }

            return;
        }

        #endregion
    }
}

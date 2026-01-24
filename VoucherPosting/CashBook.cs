using AppliedDB;
using AppMessages;
using System.Data;
using static AppliedDB.VoucherTypeClass;
using static AppMessages.Enums;


namespace VoucherPosting
{
    // Posting of Cash Voucher
    public class CashBook
    {

        public DataSource Source { get; set; }
        public VoucherPostingModel PostingData { get; set; }
        public LedgerModel LedgerData { get; set; }
        public bool PostSuccessful { get; set; } = false;
        public MessageClass MsgClass { get; set; } = new MessageClass();
        public string Action { get; set; } = string.Empty;
        public string Vou_No { get; set; }
        public long Vou_ID { get; set; }



        public CashBook(DataSource _Source, VoucherPostingModel _PostingModel)
        {
            Source = _Source;
            PostingData = _PostingModel;
            GetVouNumber();

            if (!string.IsNullOrEmpty(Vou_No))
            {
                LedgerData = new LedgerModel(Source, Vou_No);
            }
        }

        private void GetVouNumber()
        {
            if (Source == null) { return; }
            if (PostingData == null) { return; }
            if (PostingData.MasterTable == null) { return; }
            if (PostingData.MasterTable.Rows.Count == 0) { return; }
            Vou_No = PostingData?.MasterTable.Rows[0]["Vou_No"].ToString()!;
            Vou_ID = (long)PostingData?.MasterTable.Rows[0]["ID"]!;
        }

        public async Task DoCashPosting()
        {
            await PostBook();
        }

        public async Task DoBankPosting()
        {
            await PostBook();
        }


        public async Task PostBook()
        {
            MsgClass.ClearMessages();           // Clear all previous messages.

            // Validation of Voucher
            if (!PostValidate()) { return; }

            try
            {
                Source.BeginTransaction();
                DataRow _MasterRow = PostingData.MasterTable.Rows[0];
                int SrNo = 1;

                #region Master Record
                var LedgerRow = LedgerData.LedgerTable.NewRow();

                LedgerRow["ID"] = 0;
                LedgerRow["TranID"] = _MasterRow.Field<long>("ID");
                LedgerRow["Vou_Type"] = VoucherType.Cash.ToString();
                LedgerRow["Vou_Date"] = _MasterRow.Field<DateTime>("Vou_Date").Date;
                LedgerRow["Vou_No"] = _MasterRow.Field<string>("Vou_No");
                LedgerRow["SR_No"] = SrNo++;
                LedgerRow["Ref_No"] = string.IsNullOrEmpty(_MasterRow.Field<string>("Ref_No"))
                                        ? DBNull.Value
                                        : _MasterRow["Ref_No"];
                LedgerRow["BookID"] = _MasterRow.Field<long>("BookID");

                LedgerRow["COA"] = _MasterRow.Field<long>("BookID");
                LedgerRow["DR"] = _MasterRow.Field<decimal>("Amount") < 0 ? _MasterRow.Field<decimal>("Amount") : 0;
                LedgerRow["CR"] = _MasterRow.Field<decimal>("Amount") > 0 ? _MasterRow.Field<decimal>("Amount") : 0;
                LedgerRow["Customer"] = DBNull.Value;
                LedgerRow["Project"] = DBNull.Value;
                LedgerRow["Employee"] = DBNull.Value;
                LedgerRow["Description"] = _MasterRow.Field<string>("Remarks");
                LedgerRow["Comments"] = DBNull.Value;

                Source.Save(LedgerRow);
                if (!Source.IsSaved)
                {
                    MsgClass = Source.MsgClass;
                    Source.RollbackTransaction();
                    return;
                }

                #endregion

                #region Details
                foreach (DataRow Row in PostingData.DetailTable.Rows)
                {
                    LedgerRow = LedgerData.LedgerTable.NewRow();

                    LedgerRow["ID"] = 0; 
                    LedgerRow["TranID"] = _MasterRow.Field<long>("ID");
                    LedgerRow["Vou_Type"] = VoucherType.Cash.ToString();
                    LedgerRow["Vou_Date"] = _MasterRow.Field<DateTime>("Vou_Date").Date;
                    LedgerRow["Vou_No"] = _MasterRow.Field<string>("Vou_No");
                    LedgerRow["SR_No"] = SrNo++;
                    LedgerRow["Ref_No"] = _MasterRow.Field<string>("Ref_No");
                    LedgerRow["BookID"] = _MasterRow.Field<long>("BookID");
                    LedgerRow["COA"] = Row["COA"];
                    LedgerRow["DR"] = Row["DR"];
                    LedgerRow["CR"] = Row["CR"];
                    LedgerRow["Customer"] = Row["Company"];
                    LedgerRow["Project"] = Row["Project"];
                    LedgerRow["Employee"] = Row["Employee"];
                    LedgerRow["Description"] = Row["Description"];
                    LedgerRow["Comments"] = Row.Field<string>("Comments");

                    Source.Save(LedgerRow);
                    if (!Source.IsSaved)
                    {
                        MsgClass = Source.MsgClass;
                        Source.RollbackTransaction();
                        break;
                    }

                    MsgClass = Source.MsgClass;
                }
                #endregion

                #region Mark as Posted

                DataRow _PostedRow = Source.GetDataRow(AppliedDB.Enums.Tables.Book, Vou_ID);
                if (_PostedRow != null)
                {
                    _PostedRow["Status"] = "Posted";
                    Source.Save(_PostedRow);
                    Source.CommitTransaction();                 // At the end commit the transaction
                    PostSuccessful = true;
                    MsgClass.Success(Messages.TransactionCommited);
                }
                else
                {
                    PostSuccessful = false;
                    MsgClass.Success(Messages.TransactionRollback);
                    Source.RollbackTransaction();               // Otherwsie rollback
                }

                #endregion
            }
            catch (Exception ex)
            {
                Source.RollbackTransaction();
                MsgClass.Critical(ex.Message);
            }
            await Task.FromResult(true);
        }

        public bool PostValidate()
        {
            try
            {
                MsgClass.ClearMessages();

                if (Source == null) { MsgClass.Error(Messages.DataSourceIsNull); }
                if (Vou_ID == 0) { MsgClass.Error(Messages.VouNoNotDefineProperly); }
                if (string.IsNullOrEmpty(Vou_No)) { MsgClass.Error(Messages.VouNoIsNull); }
                if (Source!.GetTable(AppliedDB.Enums.Tables.Ledger, $"Vou_No='{Vou_No}'").Rows.Count > 0)
                { MsgClass.Alert(Messages.VoucherAlreadyPosted); }

                if (PostingData == null) { MsgClass.Error(Messages.PostingDataIsNull); }
                {
                    if (PostingData?.MasterTable.Rows.Count == 0) { MsgClass.Alert(Messages.PostingMasterRecordNotFound); }
                    if (PostingData?.DetailTable.Rows.Count == 0) { MsgClass.Alert(Messages.PostingDetailRecordNotFound); }

                    var VouAmount = PostingData?.MasterTable.Rows[0].Field<decimal>("Amount");
                    var SumDR = PostingData?.DetailTable.AsEnumerable().Sum(r => r.Field<decimal>("DR"));
                    var SumCR = PostingData?.DetailTable.AsEnumerable().Sum(r => r.Field<decimal>("CR"));

                    if (VouAmount != (SumDR - SumCR)) { MsgClass.Alert(Messages.AmountNotEqual); }
                }
                if (LedgerData.GetVouID() > 0) { MsgClass.Error(Messages.VoucherAlreadyPosted); }


                if (MsgClass.Count > 0)
                {
                    return false;
                }
            }
            catch (Exception error)
            {
                MsgClass.Critical(error.Message);
                return false;
            }

            return true;
        }



        #region Cash or Bank Voucher Unpost

        public async Task DoCashUnPost()
        {
            throw new NotImplementedException();
        }

        public async Task DoBankUnPost()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}

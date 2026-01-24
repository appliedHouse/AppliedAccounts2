using AppliedAccounts.Data;
using AppliedDB;
using AppMessages;
using Microsoft.Data.Sqlite;
using System.Data;
using System.Drawing;
using static AppliedDB.Enums;
using static AppliedDB.VoucherTypeClass;
using static AppMessages.Enums;

namespace AppliedAccounts.Models.Posting
{
    public class BillReceivable
    {

        public MessageClass MsgClass { get; set; }
        public DataSource Source { get; set; }


        #region Bill Receivable / Sales Invoices
        public async Task<bool> PostBillReceivable(string UserName, int id)
        {
            MsgClass = new();
            Source!.BeginTransaction();

            if (Source == null)
            {
                MsgClass.Warning(Messages.DataSourceIsNull);
                return false;
            }

            var _Filter = $"Vou_Type='{VoucherType.Receivable}' AND TranID={id}";
            var tb_Ledger = Source.GetTable(Tables.Ledger, _Filter);
            List<DataRow> VoucherRows = new();
            DataTable SaleInvoice = Source.GetTable(SQLQueries.Quries.SaleInvoice(id));

            #region Validations
            if (SaleInvoice == null)
            {
                MsgClass.Warning(Messages.SalesInvoiceIDIsNull);
            }

            if (SaleInvoice!.Rows.Count == 0)
            {
                MsgClass.Warning(Messages.SalesInvoiceRowCountZero);
            }

            if (SaleInvoice.Rows[0]["Status"].ToString() == VoucherStatus.Posted.ToString())
            {
                MsgClass.Warning(Messages.SalesInvoiceIsAlreadyPosted);

            }
            #endregion

            if (MsgClass.Count == 0)
            {
                int COA_DR = AppRegistry.GetNumber(UserName, "BRec_Receivable");
                int COA_CR = AppRegistry.GetNumber(UserName, "BRec_Stock");
                int COA_Tax = AppRegistry.GetNumber(UserName, "BRec_Tax");

                if (COA_DR == 0 || COA_CR == 0 || COA_Tax == 0)
                {
                    MsgClass.Warning(Messages.SalesInvoiceAccountNotValid);
                    return false;
                }

                int SRNO = 1;
                string Vou_No = SaleInvoice.Rows[0]["Vou_No"].ToString()!;

                #region Check the vocher is already exist in the ledger ? or not exist.

                var vw_Ledger = tb_Ledger.AsDataView();
                vw_Ledger.RowFilter = $"Vou_No='{Vou_No}'";
                if (vw_Ledger.Count > 0)
                {
                    MsgClass.Warning(Messages.SalesInvoiceVoucherAlreadyPosted);

                }
                #endregion

                if (MsgClass.Count == 0 && SalesInvoiceValidation())
                {
                    try
                    {
                        foreach (DataRow Row in SaleInvoice.Rows)
                        {


                            if (Vou_No != Row["Vou_No"].ToString())
                            {
                                MsgClass.Warning(Messages.SalesInvoiceVoucherNotMatched);
                                break;
                            }
                            var _Description = (string)Row["Inventory"] + ": " + (string)Row["Description"];

                            #region Debit Entry
                            DataRow CurrentRow = Source.GetNewRow(Tables.Ledger);
                            CurrentRow["ID"] = 0;
                            CurrentRow["TranID"] = Row["TranID"];
                            CurrentRow["Vou_Type"] = VoucherType.Receivable.ToString();
                            CurrentRow["Vou_Date"] = Row["Vou_Date"];
                            CurrentRow["Vou_No"] = Row["Vou_No"];
                            CurrentRow["SR_No"] = SRNO; SRNO += 1;
                            CurrentRow["Ref_No"] = Row["Ref_No"];
                            CurrentRow["BookID"] = 0;
                            CurrentRow["COA"] = COA_DR;
                            CurrentRow["DR"] = Row["Amount"];
                            CurrentRow["CR"] = 0;
                            CurrentRow["Customer"] = Row["CompanyID"];
                            CurrentRow["Project"] = Row["ProjectID"];
                            CurrentRow["Employee"] = Row["EmployeeID"];
                            CurrentRow["Description"] = _Description;
                            CurrentRow["Comments"] = Row["Remarks"];

                            Source.Save(CurrentRow);

                            #endregion

                            #region Tax Entry
                            if (Conversion.ToDecimal(Row["Tax_Amount"]) > 0)
                            {
                                CurrentRow = Source.GetNewRow(Tables.Ledger);
                                CurrentRow["ID"] = 0;
                                CurrentRow["TranID"] = Row["TranID"];
                                CurrentRow["Vou_Type"] = VoucherType.Receivable.ToString();
                                CurrentRow["Vou_Date"] = Row["Vou_Date"];
                                CurrentRow["Vou_No"] = Row["Vou_No"];
                                CurrentRow["SR_No"] = SRNO; SRNO += 1;
                                CurrentRow["Ref_No"] = Row["Ref_No"];
                                CurrentRow["BookID"] = 0;
                                CurrentRow["COA"] = COA_Tax;
                                CurrentRow["DR"] = Row["Tax_Amount"]; ;
                                CurrentRow["CR"] = 0;
                                CurrentRow["Customer"] = Row["CompanyID"];
                                CurrentRow["Project"] = Row["ProjectID"];
                                CurrentRow["Employee"] = Row["EmployeeID"];
                                CurrentRow["Description"] = string.Concat(Row["Tax"], ": ", Row["Description"]);
                                CurrentRow["Comments"] = Row["Remarks"];

                                Source.Save(CurrentRow);
                            }
                            #endregion

                            #region Credit Entry
                            CurrentRow = Source.GetNewRow(Tables.Ledger);
                            CurrentRow["ID"] = 0;
                            CurrentRow["TranID"] = Row["TranID"];
                            CurrentRow["Vou_Type"] = VoucherType.Receivable.ToString();
                            CurrentRow["Vou_Date"] = Row["Vou_Date"];
                            CurrentRow["Vou_No"] = Row["Vou_No"];
                            CurrentRow["SR_No"] = SRNO; SRNO += 1;
                            CurrentRow["Ref_No"] = Row["Ref_No"];
                            CurrentRow["BookID"] = 0;
                            CurrentRow["COA"] = COA_CR;
                            CurrentRow["DR"] = 0;
                            CurrentRow["CR"] = Row["Net_Amount"];
                            CurrentRow["Customer"] = Row["CompanyID"];
                            CurrentRow["Project"] = Row["ProjectID"];
                            CurrentRow["Employee"] = Row["EmployeeID"];
                            CurrentRow["Description"] = Row["Description"];
                            CurrentRow["Comments"] = Row["Remarks"];

                            Source.Save(CurrentRow);

                            #endregion
                        }
                    }
                    catch (Exception)
                    {
                        Source.RollbackTransaction();
                        MsgClass = new();
                        MsgClass.Danger(Messages.SalesInvoicePostingFailed);
                        return false;
                    }
                }
            }

            Source.CommitTransaction();
            return MsgClass.Count == 0;
        }

        private bool SalesInvoiceValidation()
        {
            var _result = true;
            return _result;
        }
        #endregion
    }
}

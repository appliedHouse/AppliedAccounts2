using AppliedAccounts.Models;
using System.Data;
using VoucherPosting;

namespace AppliedAccounts.Data.Mapping
{
    public static class MappingLedger
    {
        public static List<DataRow> FromBook(this BookModel.Voucher BookVoucher)
        {
            var ledgerRows = new List<DataRow>();

            // Create a DataTable to define the schema for Ledger
            var ledgerTable = new DataTable("Ledger");
            ledgerTable.Columns.Add("ID", typeof(long));
            ledgerTable.Columns.Add("TranID", typeof(int));
            ledgerTable.Columns.Add("Vou_Type", typeof(string));
            ledgerTable.Columns.Add("Vou_Date", typeof(DateTime));
            ledgerTable.Columns.Add("Vou_No", typeof(string));
            ledgerTable.Columns.Add("SR_NO", typeof(int));
            ledgerTable.Columns.Add("Ref_No", typeof(string));
            ledgerTable.Columns.Add("BookID", typeof(long));
            ledgerTable.Columns.Add("COA", typeof(long));
            ledgerTable.Columns.Add("DR", typeof(decimal));
            ledgerTable.Columns.Add("CR", typeof(decimal));
            ledgerTable.Columns.Add("CUSTOMER", typeof(long));
            ledgerTable.Columns.Add("EMPLOYEE", typeof(long));
            ledgerTable.Columns.Add("INVENTORY", typeof(long));
            ledgerTable.Columns.Add("PROJECT", typeof(long));
            ledgerTable.Columns.Add("Description", typeof(string));
            ledgerTable.Columns.Add("Comments", typeof(string));
            ledgerTable.Columns.Add("Status", typeof(string));

            // For each detail, create a ledger entry
            foreach (var detail in BookVoucher.Details)
            {
                var row = ledgerTable.NewRow();

                // Generate unique ID (you might want to use a different strategy)
                row["ID"] = detail.ID2 > 0 ? detail.ID2 : 0;

                row["TranID"] = detail.TranID > 0 ? detail.TranID : 0;
                row["Vou_Type"] = "Voucher"; // Or determine from context
                row["Vou_Date"] = BookVoucher.Master.Vou_Date;
                row["Vou_No"] = BookVoucher.Master.Vou_No;
                row["SR_NO"] = detail.Sr_No;
                row["Ref_No"] = BookVoucher.Master.Ref_No;
                row["BookID"] = BookVoucher.Master.BookID;
                row["COA"] = detail.COA;
                row["DR"] = detail.DR;
                row["CR"] = detail.CR;
                row["CUSTOMER"] = 0; // Set appropriately if you have customer mapping
                row["EMPLOYEE"] = detail.Employee > 0 ? detail.Employee : 0;
                row["INVENTORY"] = 0; // Set appropriately if you have inventory mapping
                row["PROJECT"] = detail.Project > 0 ? detail.Project : 0;
                row["Description"] = detail.Description ?? string.Empty;
                row["Comments"] = detail.Comments ?? string.Empty;
                row["Status"] = BookVoucher.Master.Status ?? "Active";

                ledgerRows.Add(row);
            }

            return ledgerRows;
        }


       
       
    }
}
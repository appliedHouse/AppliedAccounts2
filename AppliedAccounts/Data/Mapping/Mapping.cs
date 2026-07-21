using AppliedAccounts.Models;
using AppliedAccounts.Models.Accounts;
using AppliedAccounts.Pages.Accounts.Post;
using AppliedDB;
using System.Data;

namespace AppliedAccounts.Data.Mapping
{
    public static class Mapping
    {
        public static DataRow RemoveDBNull(this DataRow row)
        {
            foreach (DataColumn column in row.Table.Columns)
            {
                if (row[column] == DBNull.Value)
                {
                    row[column] = null;
                }
            }
            return row;
        }


        public static DataRow? ToDataRow(this COARecord rec, DataRow row)
        {
            if (rec == null) { return null; }
            row["ID"] = rec.ID;
            row["Code"] = rec.Code;
            row["Title"] = rec.Title;
            row["Class"] = rec.Class;
            row["Nature"] = rec.Nature;
            row["Notes"] = rec.Notes;
            row["OPENING_BALANCE"] = rec.OBal;
            return row;
        }

        public static DataRow ToDataRow(this COANatureRecord rec, DataRow row)
        {
            row["Id"] =    rec.ID;
            row["Code"] =  rec.Code;
            row["Title"] = rec.Title;
            return row;
        }

        public static DataRow ToDataRow(this JVViewModel rec, DataRow row)
        {
            // Convert to book Data table's Columns
            row["ID"] = rec.ID;
            row["Vou_No"] = rec.Vou_No;
            row["Vou_Date"] = rec.Vou_Date;
            row["Vou_Type"] = VoucherTypeClass.VoucherType.JV.ToString();
            row["Sr_No"] = rec.Sr_No;
            row["Ref_No"] = rec.Ref_No;
            row["BookID"] = rec.BookID;
            row["COA"] = rec.COA;
            row["DR"] = rec.DR;
            row["CR"] = rec.CR;
            row["Customer"] = rec.Company;
            row["Employee"] = rec.Employee;
            row["Inventory"] = rec.Inventory;
            row["Project"] = rec.Project;
            row["Description"] = rec.Description;
            row["Comments"] = rec.Comments;
            row["Status"] = PostingStatus.Submitted.ToString();
            return row;
        }
    }
}

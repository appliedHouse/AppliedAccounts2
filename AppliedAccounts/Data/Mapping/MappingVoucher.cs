using System.Data;
using static AppliedAccounts.Models.BookModel;

namespace AppliedAccounts.Data.Mapping
{
    public static class MappingVoucher
    {
        public static DataRow ToDataRow(this Master voucher, DataRow _row)
        {
            _row["ID"] = voucher.ID1;
            _row["BookID"] = voucher.BookID;
            _row["Vou_No"] = voucher.Vou_No;
            _row["Vou_Date"] = voucher.Vou_Date;
            _row["Amount"] = voucher.Amount;
            _row["Ref_No"] = voucher.Ref_No;
            _row["SheetNo"] = voucher.SheetNo;
            _row["Remarks"] = voucher.Ref_No;
            _row["Status"] = voucher.Status;
            return _row;
        }

        public static DataRow ToDataRow(this Detail detail, DataRow _row)
        {
            _row["ID"] = detail.ID2;
            _row["TranID"] = detail.TranID;
            _row["SR_NO"] = detail.Sr_No;
            _row["COA"] = detail.COA;
            _row["Company"] = detail.Company;
            _row["Employee"] = detail.Employee;
            _row["Project"] = detail.Project;
            _row["DR"] = detail.DR;
            _row["CR"] = detail.CR;
            _row["Description"] = detail.Description;
            _row["Comments"] = detail.Comments;

            return _row;
        }
    }
}

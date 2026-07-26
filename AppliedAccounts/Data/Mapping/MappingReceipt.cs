using System.Data;
using static AppliedAccounts.Models.ReceiptModel;

namespace AppliedAccounts.Data.Mapping
{
    public static class MappingReceipt
    {
        #region Voucher Model Mapping
        public static Voucher? ToReceiptModel(this List<DataRow> Rows)
        {
            if (Rows == null || Rows.Count == 0)
                return null;

            var modelVoucher = new Voucher
            {
                Master = new Master(),
                Details = new List<Detail>()
            };

            var IsMaster = false;

            foreach (DataRow Row in Rows)
            {
                if (!IsMaster)
                {
                    // Populate Master from the first row
                    modelVoucher.Master.ID1 = Row.Field<long>("ID1");
                    modelVoucher.Master.Vou_No = Row.Field<string>("Vou_No") ?? "";
                    modelVoucher.Master.Vou_Date = Row.Field<DateTime>("Vou_Date");
                    modelVoucher.Master.COA = Row.Field<long>("COA");
                    modelVoucher.Master.Payer = Row.Field<long>("Payer");
                    modelVoucher.Master.Ref_No = Row.Field<string>("Ref_No") ?? "";
                    modelVoucher.Master.Doc_No = Row.Field<string>("Doc_No");
                    modelVoucher.Master.Doc_Date = Row.Field<DateTime?>("Doc_Date");
                    modelVoucher.Master.Pay_Mode = Row.Field<string>("Pay_Mode") ?? "";
                    modelVoucher.Master.Amount = Row.Field<decimal>("Amount");
                    modelVoucher.Master.Remarks = Row.Field<string>("Remarks") ?? "";
                    modelVoucher.Master.Comments = Row.Field<string>("Comments") ?? "";
                    modelVoucher.Master.Status = Row.Field<string>("Status") ?? "";
                    modelVoucher.Master.TitlePayer = Row.Field<string>("TitlePayer") ?? "";
                    modelVoucher.Master.TitleCOA = Row.Field<string>("TitleCOA") ?? "";

                    IsMaster = true;
                }

                // Populate Detail for each row
                var detail = new Detail
                {
                    ID2 = Row.Field<long>("ID2"),
                    Sr_No = Row.Field<int>("Sr_No"),
                    TranID = Row.Field<long>("TranID"),
                    Ref_No = Row.Field<string>("Ref_No2") ?? "",
                    Inv_No = Row.Field<long>("Inv_No"),
                    Account = Row.Field<long>("Account"),
                    DR = Row.Field<decimal>("DR"),
                    CR = Row.Field<decimal>("CR"),
                    Employee = Row.Field<long>("Employee"),
                    Project = Row.Field<long>("Project"),
                    Description = Row.Field<string>("Description") ?? "",
                    TitleAccount = Row.Field<string>("TitleAccount") ?? "",
                    TitleProject = Row.Field<string>("TitleProject") ?? "",
                    TitleEmployee = Row.Field<string>("TitleEmployee") ?? "",
                    Action = "Found"
                };

                modelVoucher.Details.Add(detail);
            }
            return modelVoucher;
        }

        #endregion
    }
}

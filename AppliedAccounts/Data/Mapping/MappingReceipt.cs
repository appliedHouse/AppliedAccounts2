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
                var detail = new Detail();
                {
                    detail.ID2 = Row.Field<long>("ID2");
                    detail.Sr_No = Row.Field<int>("Sr_No");
                    detail.TranID = Row.Field<long>("TranID");
                    detail.Ref_No = Row.Field<string>("Ref_No2") ?? "";
                    detail.Inv_No = Row.Field<long>("Inv_No");
                    detail.Account = Row.Field<long>("Account");
                    detail.DR = Row.Field<decimal>("DR");
                    detail.CR = Row.Field<decimal>("CR");
                    detail.Employee = Row.Field<long>("Employee");
                    detail.Project = Row.Field<long>("Project");
                    detail.Description = Row.Field<string>("Description") ?? "";
                    detail.TitleAccount = Row.Field<string>("TitleAccount") ?? "";
                    detail.TitleProject = Row.Field<string>("TitleProject") ?? "";
                    detail.TitleEmployee = Row.Field<string>("TitleEmployee") ?? "";
                    detail.Action = "Found";
                };

                modelVoucher.Details.Add(detail);
            }
            return modelVoucher;
        }

        #endregion
    }
}

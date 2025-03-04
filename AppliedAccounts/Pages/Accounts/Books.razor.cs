using AppliedAccounts.Data;
using AppliedAccounts.Models;
using AppliedDB;
using Microsoft.AspNetCore.Components;



namespace AppliedAccounts.Pages.Accounts
{
    public partial class Books
    {
        [Parameter] public int ID { get; set; }
        [Parameter] public int NatureID { get; set; }

        public AppUserModel UserProfile { get; set; }
        public BookModel MyModel { get; set; } = new();

        public int BookID { get; set; }
        //public Voucher MyVoucher { get; set; }
        //public DataSource Source { get; set; }
        public MessageClass MsgClass { get; set; }



        public Books() { }

        public void Start()
        {
            MsgClass = new();
            MyModel = new(ID,UserProfile);
        }

        #region Load Data 
        //public bool LoadData(int _ID)
        //{
        //    var _Voucher = Source.GetBookVoucher(_ID).AsEnumerable().ToList();
        //    if (_Voucher.Count > 0)
        //    {
        //        MyVoucher.Master = _Voucher.Select(row => new Record()
        //        {
        //            ID1 = row.Field<int>("ID1"),
        //            Vou_No = row.Field<string>("Vou_No") ?? "",
        //            Vou_Date = row.Field<DateTime>("Vou_Date"),
        //            BookID = row.Field<int>("BookID"),
        //            Amount = row.Field<decimal>("BookID"),
        //            Ref_No = row.Field<string>("Ref_No") ?? "",
        //            SheetNo = row.Field<string>("SheetNo") ?? "",
        //            Remarks = row.Field<string>("Remarks") ?? "",
        //            Status = row.Field<string>("Status") ?? "Submitted",
        //        }).First();

        //        MyVoucher.Detail = [.. _Voucher.Select(row => new Records()
        //        {
        //            ID2 = row.Field<int>("ID2"),
        //            TranID = row.Field<int>("TranID"),
        //            Sr_No = row.Field<int>("SR_NO"),
        //            COA = row.Field<int>("COA"),
        //            Company = row.Field<int>("Company"),
        //            Employee = row.Field<int>("Employee"),
        //            Project = row.Field<int>("Project"),
        //            DR = row.Field<decimal>("DR"),
        //            CR = row.Field<decimal>("CR"),
        //            Description = row.Field<string>("Description") ?? "",
        //            Comments = row.Field<string>("Comments") ?? ""
        //        })];

        //        BookID = MyVoucher.Master.BookID;           // Assigned a book ID from voucher data.
        //    }
        //    else
        //    {
        //        MyModel.MyMessages.Add(Messages.NoRecordFound);
        //        return false;
        //    }

        //    return true;
        //}
        #endregion

    }



}

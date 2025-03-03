using AppliedAccounts.Data;
using AppliedAccounts.Models;
using AppliedDB;
using Microsoft.AspNetCore.Components;
using System.Data;
using Messages = AppMessages.Enums.Messages;



namespace AppliedAccounts.Pages.Accounts
{
    public partial class Books
    {
        
        public AppUserModel UserProfile { get; set; }
        public BookModel MyModel { get; set; } = new();
        
        public int BookID { get; set; }
        public Voucher MyVoucher { get; set; }
        public DataSource Source { get; set; }
        public MessageClass MsgClass { get; set; }
        public DateTime LastVoucherDate { get; set; }

        //public Books()
        //{
        //    Start();

        //}

        public Books()
        {
            //Start();            // Assigned initial variables and load data (Voucher)
            
        }


        public void Start()
        {
            
            MsgClass = new();

            if (UserProfile != null)
            {
                Source = new(UserProfile);
            }
            else
            {
                MsgClass.MyMessages.Add(MessageClass.GetMessage(Messages.UserProfileIsNull));
            }
            LastVoucherDate = AppRegistry.GetDate(MyModel.DBFile, "LastBookVou");

            if(ID ==0) { MyVoucher = NewVoucher(); } else { LoadData(ID); }

        }

        public bool LoadData(int _ID)
        {
            var _Voucher = Source.GetBookVoucher(_ID).AsEnumerable().ToList();
            if (_Voucher.Count > 0)
            {
                MyVoucher.Master = _Voucher.Select(row => new Record()
                {
                    ID1 = row.Field<int>("ID1"),
                    Vou_No = row.Field<string>("Vou_No") ?? "",
                    Vou_Date = row.Field<DateTime>("Vou_Date"),
                    BookID = row.Field<int>("BookID"),
                    Amount = row.Field<decimal>("BookID"),
                    Ref_No = row.Field<string>("Ref_No") ?? "",
                    SheetNo = row.Field<string>("SheetNo") ?? "",
                    Remarks = row.Field<string>("Remarks") ?? "",
                    Status = row.Field<string>("Status") ?? "Submitted",
                }).First();

                MyVoucher.Detail = [.. _Voucher.Select(row => new Records()
                {
                    ID2 = row.Field<int>("ID2"),
                    TranID = row.Field<int>("TranID"),
                    Sr_No = row.Field<int>("SR_NO"),
                    COA = row.Field<int>("COA"),
                    Company = row.Field<int>("Company"),
                    Employee = row.Field<int>("Employee"),
                    Project = row.Field<int>("Project"),
                    DR = row.Field<decimal>("DR"),
                    CR = row.Field<decimal>("CR"),
                    Description = row.Field<string>("Description") ?? "",
                    Comments = row.Field<string>("Comments") ?? ""
                })];

                BookID = MyVoucher.Master.BookID;           // Assigned a book ID from voucher data.
            }
            else
            {
                MyModel.MyMessages.Add(Messages.NoRecordFound);
                return false;
            }

            return true;
        }

        public Voucher NewVoucher()
        {
            Voucher _NewVoucher = new() { Master = new(), Detail = [] };

            _NewVoucher.Master.ID1 = 0;
            _NewVoucher.Master.Vou_No = "New";
            _NewVoucher.Master.Vou_Date = LastVoucherDate;
            _NewVoucher.Master.BookID = BookID;
            _NewVoucher.Master.Amount = 0.00M;
            _NewVoucher.Master.Ref_No = "";
            _NewVoucher.Master.SheetNo = "";
            _NewVoucher.Master.Remarks = "";
            _NewVoucher.Master.Status = "Submitted";

            _NewVoucher.Detail.Add(new Records
            {
                ID2 = 0,
                TranID = 0,
                Sr_No = 1,
                COA = 0,
                Company = 0,
                Employee = 0,
                Project = 0,
                DR = 0.00M,
                CR = 0.00M,
                Description = "",
                Comments = ""
            });

            return _NewVoucher;
        }
    }


    public class Voucher
    {
        public Record Master { get; set; }
        public List<Records> Detail { get; set; }
    }


    public class Record
    {
        public Record() { }
        public int ID1 { get; set; }
        public string Vou_No { get; set; }
        public DateTime Vou_Date { get; set; }
        public int BookID { get; set; }
        public decimal Amount { get; set; }
        public string Ref_No { get; set; }
        public string SheetNo { get; set; }
        public string Remarks { get; set; }
        public string Status { get; set; }
    }

    public class Records
    {

        public int ID2 { get; set; }
        public int TranID { get; set; }
        public int Sr_No { get; set; }
        public int COA { get; set; }
        public int Company { get; set; }
        public int Employee { get; set; }
        public int Project { get; set; }
        public decimal DR { get; set; }
        public decimal CR { get; set; }
        public string Description { get; set; }
        public string Comments { get; set; }
    }


}

using AppliedAccounts.Data;
using AppliedAccounts.Models;
using AppliedDB;
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
        public MessageClass MsgClass {get; set;}

        public Books()
        {
            Start();

        }

        public Books(int _BookID)
        {
            BookID = _BookID;
            Start();
            LoadData(BookID);
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
        }


        public bool LoadData(int _BookID)
        {
            var _Data = Source.GetBook(_BookID);



            return true;
        }

    }


    public class Voucher
    {
        public Record Master { get; set; }
        public List<Records> Detail { get; set; }
    }


    public class Record
    {
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

        public class Records { 

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

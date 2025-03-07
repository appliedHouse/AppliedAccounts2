using AppliedAccounts.Data;
using AppliedDB;
using System.Data;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using MESSAGE = AppMessages.Enums.Messages;
using Tables = AppliedDB.Enums.Tables;

namespace AppliedAccounts.Models
{
    public class BookModel
    {

        #region Variables
        public int VoucherID { get; set; }
        public int BookID { get; set; }
        public int BookNature { get; set; }
        public string BookNatureTitle { get; set; }

        public Voucher MyVoucher { get; set; }

        public List<CodeTitle> Companies { get; set; }
        public List<CodeTitle> Employees { get; set; }
        public List<CodeTitle> Projects { get; set; }
        public List<CodeTitle> Accounts { get; set; }
        public List<CodeTitle> BookList { get; set; }

        public AppUserModel? UserProfile { get; set; }
        public DataSource Source { get; set; }
        public MessageClass MsgClass { get; set; }
        public bool PageIsValid { get; set; } = false;
        public DateTime LastVoucherDate { get; set; }

        #endregion
        #region Constructor
        public BookModel()
        {

        }
        public BookModel(int _VoucherID, AppUserModel _AppUserProfile)
        {
            MsgClass = new();
            try
            {
                VoucherID = _VoucherID;
                UserProfile = _AppUserProfile;

                if (UserProfile != null)
                {
                    Source = new(UserProfile);
                    LastVoucherDate = AppRegistry.GetDate(Source.DBFile, "LastBKDate");

                    if (VoucherID == 0) { MyVoucher = NewVoucher(); }   // Create a new voucher;
                    if (VoucherID > 0)
                    {
                        LoadData();
                    }

                    BookList = Source.GetBookAccounts(BookNature);
                    Companies = Source.GetCustomers();
                    Employees = Source.GetEmployees();
                    Projects = Source.GetProjects();
                    Accounts = Source.GetAccounts();
                }
                else
                {
                    MsgClass.Add(MESSAGE.UserProfileIsNull);
                }

                PageIsValid = true;
            }
            catch (Exception)
            {



            }

        }
        #endregion

        private bool LoadData()
        {
            PageIsValid = true;
            if (Source != null)
            {
                try
                {
                    var VoucherData = Source.GetBookVoucher(VoucherID).AsEnumerable().ToList();

                    if (VoucherData != null)
                    {
                        if (VoucherData.Count > 0)
                        {
                            BookID = VoucherData.Select(row => row.Field<int>("BookID")).First();
                            BookNatureTitle = GetNatureTitle(BookID);

                            MyVoucher.Master = VoucherData!.Select(first => new Master()
                            {
                                ID1 = first.Field<int>("ID1"),
                                Vou_No = first.Field<string>("Vou_No") ?? "",
                                Vou_Date = first.Field<DateTime>("Vou_Date"),
                                BookID = first.Field<int>("BookID"),
                                Amount = first.Field<decimal>("Amount"),
                                Ref_No = first.Field<string>("Ref_No") ?? "",
                                SheetNo = first.Field<string>("SheetNo") ?? "",
                                Remarks = first.Field<string>("Remarks") ?? "",
                                Status = first.Field<string>("Status") ?? ""
                            }).First() ?? new();

                            MyVoucher.Details = [.. VoucherData.Select(row => new Detail()
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
                                Comments = row.Field<string>("Comments") ?? "",
                                action = "get",

                                TitleAccount = Accounts.Where(e=> e.ID == row.Field<int>("COA")).Select(e=> e.Title).First() ?? "",
                                TitleCompany = Companies.Where(e=> e.ID == row.Field<int>("Company")).Select(e=> e.Title).First() ?? "",
                                TitleProject = Projects.Where(e => e.ID == row.Field < int >("Project")).Select(e => e.Title).First() ?? "",
                                TitleEmployee = Employees.Where(e => e.ID == row.Field < int >("Employee")).Select(e => e.Title).First() ?? "",
                            })];
                        }
                    }
                }
                catch (Exception)
                {
                    PageIsValid = false;
                }
            }
            return PageIsValid;
        }

        private Voucher NewVoucher()
        {
            BookNatureTitle = GetNatureTitle(BookID);

            Voucher _NewVoucher = new();
            _NewVoucher.Master.ID1 = 0;
            _NewVoucher.Master.Vou_No = "New";
            _NewVoucher.Master.Vou_Date = LastVoucherDate;
            _NewVoucher.Master.BookID = BookID;
            _NewVoucher.Master.Amount = 0.00M;
            _NewVoucher.Master.Ref_No = "";
            _NewVoucher.Master.SheetNo = "";
            _NewVoucher.Master.Remarks = "";
            _NewVoucher.Master.Status = "Submitted";

            _NewVoucher.Details.Add(new Detail
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
                Comments = "",
                action = "new",

                TitleAccount = string.Empty,
                TitleCompany = string.Empty,
                TitleProject = string.Empty,
                TitleEmployee = string.Empty,



            });

            return _NewVoucher;
        }


        private string GetNatureTitle(int _BookID)
        {

            var _Title = "Unknown Book"; // Default title
            var _Nature = Source.SeekValue(Tables.COA, _BookID, "Nature");

            if (_Nature != null && int.TryParse(_Nature.ToString(), out int natureValue) && natureValue > 0)
            {
                _Title = Source.SeekTitle(Tables.COA_Nature, natureValue);
            }

            return _Title;

        }
    }


    #region Models

    public class Voucher
    {

        public Voucher()
        {
            Master = new();
            Detail = new();
            Details = [];
        }

        public Master Master { get; set; }
        public Detail Detail { get; set; }
        public List<Detail> Details { get; set; }
    }


    public class Master
    {
        public Master() { }
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

    public class Detail
    {
        public Detail() { }
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
        public string action { get; set; }

        public string TitleAccount { get; set; }
        public string TitleCompany { get; set; }
        public string TitleProject { get; set; }
        public string TitleEmployee { get; set; }

    }
    #endregion
}

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
        
        public DateTime LastVoucherDate { get; set; }
        public DateTime MinVouDate = AppRegistry.MinDate;
        public DateTime MaxVouDate { get; set; }

        #endregion
        #region Constructor
        public BookModel()
        {

        }
        public BookModel(int _VoucherID, AppUserModel _AppUserProfile)
        {
            MsgClass = new();
            MyVoucher = new();

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

                
            }
            catch (Exception)
            {
                MsgClass.Add(MESSAGE.Default);


            }

        }
        #endregion

        #region Load Data
        private bool LoadData()
        {
           
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

                            return true;
                        }
                    }
                }
                catch (Exception)
                {
                    
                }
            }
            return false; 
        }
        #endregion

        #region New Voucher
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

            _NewVoucher.Detail = NewDetail();
            

            

            return _NewVoucher;
        }
        

        private Detail NewDetail()
        {
            int _MaxSrNo = 1;
            if(MyVoucher.Details.Count > 0) { _MaxSrNo = MyVoucher.Details.Max(e => e.Sr_No) + 1; }


            var _Detail = new Detail();
            {
                _Detail.ID2 = 0;
                _Detail.TranID = 0;
                _Detail.Sr_No = _MaxSrNo;
                _Detail.COA = 0;
                _Detail.Company = 0;
                _Detail.Employee = 0;
                _Detail.Project = 0;
                _Detail.DR = 0.00M;
                _Detail.CR = 0.00M;
                _Detail.Description = "";
                _Detail.Comments = "";
                _Detail.action = "new";

                _Detail.TitleAccount = string.Empty;
                _Detail.TitleCompany = string.Empty;
                _Detail.TitleProject = string.Empty;
                _Detail.TitleEmployee = string.Empty;
            };

            return _Detail;
        }
        #endregion

        #region Get Nature title 
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
        #endregion

        #region Add, Save, Delete buttons
        public void New()
        {
            MyVoucher.Detail = NewDetail();
        }

        public void Edit(int _SrNo)
        {
            var _Detail = MyVoucher.Detail;
            MyVoucher.Detail = MyVoucher.Details.Where(e => e.Sr_No == _SrNo).First() ?? _Detail;

        }
        public void Save()
        {
            if (IsVoucherValidated())
            {
                var IsSrNo = MyVoucher.Details.Where(e => e.Sr_No == MyVoucher.Detail.Sr_No).Any();
                if (!IsSrNo)
                {
                    MyVoucher.Detail.action = "save";
                    MyVoucher.Details.Add(MyVoucher.Detail);
                }
            }
        }

        public void SaveAll()
        {

        }
        public void Remove(int _SrNo)
        {

        }

        private bool IsVoucherValidated()
        {
            bool IsValid = true;


            MsgClass = new(0);
            if(MyVoucher.Master.BookID==0) { MsgClass.Add(MESSAGE.IDIsZero); }
            if(MyVoucher.Master.Vou_No.Length==0) { MsgClass.Add(MESSAGE.VouNoNotDefine); }
            if(MyVoucher.Master.Vou_No.Length==11) { MsgClass.Add(MESSAGE.VouNoNotDefine); }
            if(MyVoucher.Master.Vou_Date< AppRegistry.MinVouDate) { MsgClass.Add(MESSAGE.VouDateLess); }
            if(MyVoucher.Master.Vou_Date> AppRegistry.MaxVouDate) { MsgClass.Add(MESSAGE.VouDateMore); }
            if(MyVoucher.Master.Remarks.Length==0) { MsgClass.Add(MESSAGE.Row_NoRemarks); }
            if(MyVoucher.Master.Status.Length==0) { MsgClass.Add(MESSAGE.Row_NoStatus); }

            if(MyVoucher.Detail.COA==0) { MsgClass.Add(MESSAGE.Row_CompanyIDZero); }
            if(MyVoucher.Detail.DR>0 && MyVoucher.Detail.CR>0) { MsgClass.Add(MESSAGE.DRnCRHaveValue); }
            if(MyVoucher.Detail.DR==0&& MyVoucher.Detail.CR==0) { MsgClass.Add(MESSAGE.DRnCRAreZero); }
            if(MyVoucher.Detail.Description.Length==0) { MsgClass.Add(MESSAGE.DescriptionIsNothing); }
            return IsValid;

        }

        #endregion
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

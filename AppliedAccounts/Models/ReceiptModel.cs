using AppliedAccounts.Data;
using AppliedAccounts.Models.Interface;
using AppliedDB;
using AppMessages;
using System.Data;
using SQLQueries;
using MESSAGE = AppMessages.Enums.Messages;

namespace AppliedAccounts.Models
{
    public class ReceiptModel : IVoucher
    {
        #region Variables
        public int ReceiptID { get; set; }
        public int[] NatureIDs { get; set; }
        public DateTime LastVoucherDate { get; set; }
        public DateTime MaxVouDate { get; set; }
        public MessageClass MsgClass { get; set; }
        public Voucher MyVoucher { get; set; }
        public List<Detail> Deleted { get; set; }
        public bool Processing { get; set; }
        public DataSource Source { get; set; }
        public List<CodeTitle> Companies { get; set; }
        public List<CodeTitle> Employees { get; set; }
        public List<CodeTitle> Projects { get; set; }
        public List<CodeTitle> Accounts { get; set; }
        public List<CodeTitle> PayCOA { get; set; }
        public List<CodeTitle> InvoiceList { get; set; }
        public string DataFile { get; set; }

        public AppUserModel? UserProfile { get; set; }
        public int Index { get; set; }
        public bool RecordFound { get; set; }
        public int Count => MyVoucher.Details.Count;

        public decimal Tot_DR { get; set; }
        public decimal Tot_CR { get; set; }


        #endregion

        #region Constructor
        public ReceiptModel() { }
        public ReceiptModel(AppUserModel _UserProfile)
        {
            UserProfile = _UserProfile;
            var query = Quries.ReceiptList;

        }
        public ReceiptModel(AppUserModel _UserProfile, int _ReceiptID)
        {
            UserProfile = _UserProfile;
            ReceiptID = _ReceiptID;
            Start(ReceiptID);
            var query = Quries.ReceiptList;

        }
        public void Start(int _ReceiptID)
        {
            if (UserProfile is null) { return; }
            Source ??= new(UserProfile);

            MsgClass = new();
            MyVoucher = new();
            LastVoucherDate = AppRegistry.GetDate(Source.DBFile, "rcptDate");

            try
            {
                ReceiptID = _ReceiptID;
                DataFile = UserProfile?.DataFile ?? "";

                if (UserProfile != null && !string.IsNullOrEmpty(DataFile))
                {
                    Source = new(UserProfile);
                    LastVoucherDate = AppRegistry.GetDate(DataFile, "LastRecDate");

                    if (ReceiptID == 0) { MyVoucher = NewVoucher(); }   // Create a new voucher;
                    if (ReceiptID > 0)
                    {
                        LoadData();
                    }

                    Companies = Source.GetCustomers();
                    Employees = Source.GetEmployees();
                    Projects = Source.GetProjects();
                    Accounts = Source.GetAccounts();
                    PayCOA = Source.GetAccounts();
                    InvoiceList = Source.GetInvoices();
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


        #region New Voucher

        private Voucher NewVoucher()
        {


            Voucher _NewVoucher = new();
            _NewVoucher.Master.ID1 = 0;
            _NewVoucher.Master.Vou_No = "New";
            _NewVoucher.Master.Vou_Date = LastVoucherDate;
            _NewVoucher.Master.COA = 0;
            _NewVoucher.Master.Payer = 0;
            _NewVoucher.Master.Ref_No = string.Empty;
            _NewVoucher.Master.Doc_No = string.Empty;
            _NewVoucher.Master.Doc_Date = DateTime.Now;
            _NewVoucher.Master.Pay_Mode = string.Empty;
            _NewVoucher.Master.Amount = 0.00M;
            _NewVoucher.Master.Remarks = string.Empty;
            _NewVoucher.Master.Comments = string.Empty;
            _NewVoucher.Master.Status = "Submitted";

            _NewVoucher.Detail = NewDetail();




            return _NewVoucher;
        }
        #endregion

        #region Edit Record
        public void Edit(int _SrNo)
        {
            var _Detail = MyVoucher.Detail;
            MyVoucher.Detail = MyVoucher.Details.Where(e => e.Sr_No == _SrNo).First() ?? _Detail;
        }
        #endregion

        #region Validation
        public bool IsVoucherValidated()
        {
            bool IsValid = true;
            MsgClass = new();

            if (MyVoucher.Master == null) { MsgClass.Add(MESSAGE.MasterRecordisNull); return false; }
            if (MyVoucher.Details == null) { MsgClass.Add(MESSAGE.DetailRecordsisNull); return false; }
            //if (MyVoucher.Details.Count == 0) { MsgClass.Add(MESSAGE.DetailRecordsAreZero); return false; }

            if (MyVoucher.Master.Vou_No.Length == 0) { MsgClass.Add(MESSAGE.VouNoNotDefine); }
            if (!MyVoucher.Master.Vou_No.ToLower().Equals("new"))
            {
                if (MyVoucher.Master.Vou_No.Length != 11) { MsgClass.Add(MESSAGE.VouNoNotDefineProperly); }
            }
            if (MyVoucher.Master.Vou_Date < AppRegistry.MinVouDate) { MsgClass.Add(MESSAGE.VouDateLess); }
            if (MyVoucher.Master.Vou_Date > AppRegistry.MaxVouDate) { MsgClass.Add(MESSAGE.VouDateMore); }
            if (MyVoucher.Master.COA == 0) { MsgClass.Add(MESSAGE.Row_COAIsZero); }
            if (MyVoucher.Master.Payer == 0) { MsgClass.Add(MESSAGE.Row_CompanyIDZero); }
            if (MyVoucher.Master.Remarks.Length == 0) { MsgClass.Add(MESSAGE.Row_NoRemarks); }
            if (MyVoucher.Master.Status.Length == 0) { MsgClass.Add(MESSAGE.Row_NoStatus); }

            if (MyVoucher.Detail.Sr_No == 0) { MsgClass.Add(MESSAGE.SerialNoIsZero); }
            if (MyVoucher.Detail.Account == 0) { MsgClass.Add(MESSAGE.Row_COAIsZero); }
            if (MyVoucher.Detail.DR > 0 && MyVoucher.Detail.CR > 0) { MsgClass.Add(MESSAGE.DRnCRHaveValue); }
            if (MyVoucher.Detail.DR == 0 && MyVoucher.Detail.CR == 0) { MsgClass.Add(MESSAGE.DRnCRAreZero); }
            if (MyVoucher.Detail.Description.Length == 0) { MsgClass.Add(MESSAGE.DescriptionIsNothing); }
            if (MsgClass.Count > 0) { IsValid = false; }

            return IsValid;
        }
        #endregion

        #region Load Data
        public bool LoadData()
        {
            if (Source != null)
            {
                try
                {
                    var VoucherData = Source.GetReceiptVoucher(ReceiptID).AsEnumerable().ToList();

                    if (VoucherData != null)
                    {
                        if (VoucherData.Count > 0)
                        {

                            MyVoucher.Master = VoucherData!.Select(first => new Master()
                            {
                                ID1 = first.Field<int>("ID1"),
                                Vou_No = first.Field<string>("Vou_No") ?? "",
                                Vou_Date = first.Field<DateTime>("Vou_Date"),
                                Payer = first.Field<int>("Payer"),
                                COA = first.Field<int>("COA"),
                                Amount = first.Field<decimal>("Amount"),
                                Ref_No = first.Field<string>("Ref_No") ?? "",
                                Remarks = first.Field<string>("Remarks") ?? "",
                                Comments = first.Field<string>("Comments") ?? "",
                                Status = first.Field<string>("Status") ?? "",

                                TitleCOA = first.Field<string>("TitleCOA") ?? "",
                                TitlePayer = first.Field<string>("TitlePayer") ?? "",
                            }).First() ?? new();

                            MyVoucher.Details = [.. VoucherData.Select(row => new Detail()
                            {
                                ID2 = row.Field<int>("ID2"),
                                TranID = row.Field<int>("TranID"),
                                Sr_No = row.Field<int>("SR_NO"),
                                Account = row.Field<int>("COA"),
                                Employee = row.Field<int>("Employee"),
                                Project = row.Field<int>("Project"),
                                DR = row.Field<decimal>("DR"),
                                CR = row.Field<decimal>("CR"),
                                Description = row.Field<string>("Description") ?? "",
                                Action = "get",

                                TitleAccount = row.Field<string>("TitleAccount") ?? "",
                                TitleProject = row.Field<string>("TitleProject") ?? "",
                                TitleEmployee = row.Field<string>("TitleEmployee") ?? "",
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

        #region New Record 
        public void New()
        {
            MyVoucher.Detail = NewDetail();
        }

        private Detail NewDetail()
        {
            int _MaxSrNo = 1;
            if (MyVoucher.Details.Count > 0) { _MaxSrNo = MyVoucher.Details.Max(e => e.Sr_No) + 1; }


            var _Detail = new Detail();
            {
                _Detail.ID2 = 0;
                _Detail.TranID = 0;
                _Detail.Sr_No = _MaxSrNo;
                _Detail.Account = 0;
                _Detail.Employee = 0;
                _Detail.Project = 0;
                _Detail.DR = 0.00M;
                _Detail.CR = 0.00M;
                _Detail.Description = "";
                _Detail.Action = "new";

                _Detail.TitleAccount = string.Empty;
                _Detail.TitleProject = string.Empty;
                _Detail.TitleEmployee = string.Empty;
            }
          ;

            return _Detail;
        }
        #endregion

        #region Remove Record
        public void Remove()
        {
            if (MyVoucher.Detail is not null)
            {
                var IsAlreadyDeleted = Deleted.Where(e => e.Sr_No == MyVoucher.Detail.Sr_No).Any();
                if (!IsAlreadyDeleted)
                {
                    Deleted.Add(MyVoucher.Detail);
                    MyVoucher.Details.Remove(MyVoucher.Detail);
                    if (MyVoucher.Details.Count > 0)
                    {
                        MyVoucher.Detail = MyVoucher.Details.First();
                    }
                    else
                    {
                        MyVoucher.Detail = NewDetail();
                    }
                }
            }
            CalculateTotal();
        }

        #endregion


        #region Save
        public void Save()
        {
            if (IsVoucherValidated())
            {
                var IsSrNo = MyVoucher.Details.Where(e => e.Sr_No == MyVoucher.Detail.Sr_No).Any();
                if (!IsSrNo)
                {
                    MyVoucher.Detail.Action = "save";
                    MyVoucher.Details.Add(MyVoucher.Detail);
                }
            }
            else
            {
                MsgClass.Add(MESSAGE.RecordNotValidated);
            }
            CalculateTotal();
        }

        public Task SaveAllAsync()
        {
            throw new NotImplementedException();
        }
        #endregion


        public void TestNew()
        {
            Voucher _NewVoucher = new();
            _NewVoucher.Master.ID1 = 0;
            _NewVoucher.Master.Vou_No = "New";
            _NewVoucher.Master.Vou_Date = LastVoucherDate;
            _NewVoucher.Master.COA = 2;
            _NewVoucher.Master.Payer = 5;
            _NewVoucher.Master.Ref_No = "Ref-12345";
            _NewVoucher.Master.Doc_No = "Doc-09886";
            _NewVoucher.Master.Doc_Date = DateTime.Now;
            _NewVoucher.Master.Pay_Mode = "Bank";
            _NewVoucher.Master.Amount = 1500.00M;
            _NewVoucher.Master.Remarks = "Testing ";
            _NewVoucher.Master.Comments = "Comments";
            _NewVoucher.Master.Status = "Submitted";

            var _SRNO = 1;
            if (MyVoucher.Details.Count > 0)
            {
                _SRNO = MyVoucher.Details.Max(e => e.Sr_No) + 1;
            }

            var _Detail = new Detail();
            {
                _Detail.ID2 = 0;
                _Detail.TranID = 0;
                _Detail.Sr_No = _SRNO;
                _Detail.Account = 15;
                _Detail.Employee = 1;
                _Detail.Project = 3;
                _Detail.Inv_No = 10;
                _Detail.Ref_No = "Master Ref-009";
                _Detail.DR = 1500.00M;
                _Detail.CR = 0.00M;
                _Detail.Description = "Description";
                _Detail.Action = "new";

                _Detail.TitleAccount = Accounts.Where(e => e.ID == _Detail.Account).Select(e => e.Title).FirstOrDefault() ?? "";
                _Detail.TitleProject = Projects.Where(e => e.ID == _Detail.Project).Select(e => e.Title).FirstOrDefault() ?? "";
                _Detail.TitleEmployee = Employees.Where(e => e.ID == _Detail.Employee).Select(e => e.Title).FirstOrDefault() ?? ""; ;
            }

            _NewVoucher.Detail = _Detail;

            MyVoucher = _NewVoucher;
        }



        #region Navigation of Records
        public void Top()
        {
            Index = 1;
            if (MyVoucher.Details.Count > 0)
            { MyVoucher.Detail = MyVoucher.Details.First(); }

        }

        public void Next()
        {
            if (MyVoucher.Details.Count > 0)
            {
                Index = MyVoucher.Details.IndexOf(MyVoucher.Detail) + 1;
                var Counter = MyVoucher.Details.Count - 1;
                if (Index > Counter) { Index = Counter; }
                MyVoucher.Detail = MyVoucher.Details[Index];
            }
        }

        public void Back()
        {
            if (MyVoucher.Details.Count > 0)
            {
                Index = MyVoucher.Details.IndexOf(MyVoucher.Detail) - 1;
                if (Index < 0) { Index = 0; }
                MyVoucher.Detail = MyVoucher.Details[Index];
            }
        }

        public void Last()
        {
            Index = MyVoucher.Details.Count - 1;
            if (MyVoucher.Details.Count > 0)
            { MyVoucher.Detail = MyVoucher.Details.Last(); }
        }
        #endregion

        #region Print
        public void Print()
        {

            //await js.InvokeVoidAsync("printPDF", "/PDFReports/Test.pdf"); // Path to your PDF
        }

        public void Remove(int _SrNo)
        {
            throw new NotImplementedException();
        }

        public void CalculateTotal()
        {
            Tot_DR = 0; Tot_CR = 0;
            if(MyVoucher.Details.Count > 0)
            {
                Tot_DR = MyVoucher.Details.Sum(e => e.DR);
                Tot_CR = MyVoucher.Details.Sum(e => e.CR);
            }
        }


        #endregion

        #region VoucherModel
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
            public int COA { get; set; }               // Amount received in this account i.e. cash or bank acc.
            public int Payer { get; set; }             // Customer, client, donner etc.
            public string Ref_No { get; set; }          // Cheque No. c
            public string? Doc_No { get; set; }          // Cheque No. or on-line transaction no.
            public DateTime? Doc_Date { get; set; }      // Cheque No. or on-line transaction no.
            public string Pay_Mode { get; set; }
            public decimal Amount { get; set; }
            public string Remarks { get; set; }
            public string Comments { get; set; }
            public string Status { get; set; }


            public string TitlePayer { get; set; }
            public string TitleCOA { get; set; }
        }

        public class Detail
        {
            public int ID2 { get; set; }
            public int Sr_No { get; set; }
            public int TranID { get; set; }
            public string Ref_No { get; set; }
            public int Inv_No { get; set; }
            public int Account { get; set; }                // Settle account against receipt amount
            public decimal DR { get; set; }
            public decimal CR { get; set; }
            public int Employee { get; set; }
            public int Project { get; set; }
            public string Description { get; set; }

            public string TitleAccount { get; set; }
            public string TitleProject { get; set; }
            public string TitleEmployee { get; set; }
            public string TitleInvoice { get; set; }
            public string Action { get; set; }
        }
        #endregion
    }
}

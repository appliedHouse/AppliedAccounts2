using AppliedAccounts.Data;
using AppliedDB;
using System.Data;
using AppMessages;
using MESSAGE = AppMessages.Enums.Messages;
using Tables = AppliedDB.Enums.Tables;
using AppliedAccounts.Models.Interface;
using Microsoft.AspNetCore.Components;
using AppliedAccounts.Services;
using AppReports;

namespace AppliedAccounts.Models
{
    public class BookModel : IVoucher
    {

        #region Variables
        public int VoucherID { get; set; }
        public int BookID { get; set; }
        public int BookNature { get; set; }
        public string BookNatureTitle { get; set; }

        public Voucher MyVoucher { get; set; }
        public List<Detail> Deleted { get; set; } = [];

        public List<CodeTitle> Companies { get; set; } = [];
        public List<CodeTitle> Employees { get; set; } = [];
        public List<CodeTitle> Projects { get; set; } = [];
        public List<CodeTitle> Accounts { get; set; } = [];
        public List<CodeTitle> BookList { get; set; } = [];

        public AppUserModel? UserProfile { get; set; }
        public DataSource Source { get; set; }
        public MessageClass MsgClass { get; set; }

        public DateTime LastVoucherDate { get; set; }
        public DateTime MinVouDate = AppRegistry.MinDate;
        public DateTime MaxVouDate { get; set; }
        public int Index { get; set; } = 0;

        public string DataFile { get; set; }

        public int Count => MyVoucher.Details.Count;

        public decimal Tot_DR { get; set; }
        public decimal Tot_CR { get; set; }
        public bool IsWaiting { get; set; }
        public bool IsSaved { get; set; }
        public NavigationManager NavManager => AppGlobals.NavManager;
        public PrintService ReportService { get; set; }
        public GlobalService AppGlobals { get; set; }

        private int CashNatureID = 0;
        private int BankNatureID = 0;


        #endregion

        #region Constructor
        public BookModel()
        {

        }
        public BookModel(int _VoucherID, int _BookID, AppUserModel _AppUserProfile)
        {
            MsgClass = new();
            MyVoucher = new();

            try
            {
                BookID = _BookID;
                VoucherID = _VoucherID;
                UserProfile = _AppUserProfile;
                DataFile = _AppUserProfile.DataFile;

                CashNatureID = AppRegistry.GetNumber(DataFile, "CashBKNature");
                BankNatureID = AppRegistry.GetNumber(DataFile, "BankBKNature");

                if (UserProfile != null)
                {
                    Source = new(UserProfile);
                    LastVoucherDate = AppRegistry.GetDate(Source.DBFile, "LastBKDate");

                    if (VoucherID == 0) { MyVoucher = NewVoucher(); }   // Create a new voucher;
                    if (VoucherID > 0)
                    {
                        LoadData();
                    }


                    Companies = Source.GetCustomers();
                    Employees = Source.GetEmployees();
                    Projects = Source.GetProjects();
                    Accounts = Source.GetAccounts();

                    var result = Source.SeekValue(Tables.COA, BookID, "Nature") ?? 0;
                    BookNature = (int)result;
                    if (BookNature > 0)
                    { BookList = Source.GetBookAccounts(BookNature); }

                    if (BookNature == CashNatureID) { BookNatureTitle = "Cash Book"; }
                    if (BookNature == BankNatureID) { BookNatureTitle = "Bank Book"; }

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
        public bool LoadData()
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

                                TitleAccount = row.Field<string>("TitleCOA") ?? "",
                                TitleCompany = row.Field<string>("TitleCompany") ?? "",
                                TitleEmployee = row.Field<string>("TitleEmployee") ?? "",
                                TitleProject = row.Field<string>("TitleProject") ?? ""


                                //TitleAccount = Accounts.Where(e=> e.ID == row.Field<int>("COA")).Select(e=> e.Title).First() ?? "",
                                //TitleCompany = Companies.Where(e=> e.ID == row.Field<int>("Company")).Select(e=> e.Title).First() ?? "",
                                //TitleProject = Projects.Where(e => e.ID == row.Field < int >("Project")).Select(e => e.Title).First() ?? "",
                                //TitleEmployee = Employees.Where(e => e.ID == row.Field < int >("Employee")).Select(e => e.Title).First() ?? "",
                            })];

                            return true;
                        }
                    }
                }
                catch (Exception error)
                {
                    MsgClass.Error(error.Message);
                }
            }
            return false;
        }
        #endregion

        #region New Voucher
        private Voucher NewVoucher()
        {


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
            if (MyVoucher.Details.Count > 0) { _MaxSrNo = MyVoucher.Details.Max(e => e.Sr_No) + 1; }


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
            }
            ;

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

        public async Task<bool> SaveAllAsync()
        {
            IsSaved = true;

            if (!IsWaiting)
            {
                IsWaiting = true;
                await Task.Run(() =>
                {
                    try
                    {
                        if (MyVoucher.Master.Vou_No.ToUpper().Equals("NEW"))
                        {
                            if (BookNature == CashNatureID)         // Cash Book Nature
                            {
                                MyVoucher.Master.Vou_No = NewVoucherNo.GetCashVoucher(UserProfile!.DataFile, MyVoucher.Master.Vou_Date);
                            }

                            if (BookNature == BankNatureID)         // Bank Book Nature
                            {
                                MyVoucher.Master.Vou_No = NewVoucherNo.GetBankVoucher(UserProfile!.DataFile, MyVoucher.Master.Vou_Date);
                            }
                        }


                        var Row1 = Source.GetNewRow(Tables.Book);

                        Row1["ID"] = MyVoucher.Master.ID1;
                        Row1["Vou_No"] = MyVoucher.Master.Vou_No;
                        Row1["Vou_Date"] = MyVoucher.Master.Vou_Date;
                        Row1["BookID"] = MyVoucher.Master.BookID;
                        Row1["Ref_No"] = MyVoucher.Master.Vou_Date;
                        Row1["SheetNo"] = MyVoucher.Master.SheetNo;
                        Row1["Remarks"] = MyVoucher.Master.Remarks;
                        Row1["Status"] = MyVoucher.Master.Status;
                        Row1["Amount"] = MyVoucher.Details.Sum(e => e.DR) - MyVoucher.Details.Sum(e => e.CR);

                        CommandClass cmdClass1 = new(Row1, Source.MyConnection);
                        cmdClass1.SaveChanges();

                        VoucherID = cmdClass1.PrimaryKeyID;
                        Row1["ID"] = VoucherID;                // Get a new Id of record after save / insert.

                        var Row2 = Source.GetNewRow(Tables.Book2);

                        foreach (var item in MyVoucher.Details)
                        {
                            Row2["ID"] = item.ID2;
                            Row2["TranID"] = VoucherID;
                            Row2["SR_NO"] = item.Sr_No;
                            Row2["COA"] = item.COA;
                            Row2["Company"] = item.Company;
                            Row2["Employee"] = item.Employee;
                            Row2["Project"] = item.Project;
                            Row2["DR"] = item.DR;
                            Row2["CR"] = item.CR;
                            Row2["Description"] = item.Description;
                            Row2["Comments"] = item.Comments;

                            CommandClass cmdClass2 = new(Row2, Source.MyConnection);
                            cmdClass2.SaveChanges();
                        }

                        IsWaiting = false;

                    }
                    catch (Exception)
                    {
                        IsSaved = false;

                    }

                });
                IsWaiting = false;

            }

            return IsSaved;
        }
        #endregion

        #region Remove
        public void Remove()
        {
            if (MyVoucher.Detail != null)
            {
                MyVoucher.Detail.action = "delete";                     // Marked record as deleted.
                Deleted.Add(MyVoucher.Detail);                          // Add a record to delete while saving.
                MyVoucher.Details.Remove(MyVoucher.Detail);             // Remove record from current list.
            }
        }

        #endregion

        #region Validation
        public bool IsVoucherValidated()
        {
            bool IsValid = true;


            MsgClass = new();
            if (MyVoucher.Master.BookID == 0) { MsgClass.Add(MESSAGE.BookIDIsZero); }
            if (MyVoucher.Master.Vou_No.Length == 0) { MsgClass.Add(MESSAGE.VouNoNotDefine); }
            if (!MyVoucher.Master.Vou_No.ToLower().Equals("new"))
            {
                if (MyVoucher.Master.Vou_No.Length != 11) { MsgClass.Add(MESSAGE.VouNoNotDefineProperly); }
            }
            if (MyVoucher.Master.Vou_Date < AppRegistry.MinVouDate) { MsgClass.Add(MESSAGE.VouDateLess); }
            if (MyVoucher.Master.Vou_Date > AppRegistry.MaxVouDate) { MsgClass.Add(MESSAGE.VouDateMore); }
            if (MyVoucher.Master.Remarks.Length == 0) { MsgClass.Add(MESSAGE.Row_NoRemarks); }
            if (MyVoucher.Master.Status.Length == 0) { MsgClass.Add(MESSAGE.Row_NoStatus); }
            if (MyVoucher.Detail.Sr_No == 0) { MsgClass.Add(MESSAGE.SerialNoIsZero); }
            if (MyVoucher.Detail.COA == 0) { MsgClass.Add(MESSAGE.Row_COAIsZero); }
            if (MyVoucher.Detail.DR > 0 && MyVoucher.Detail.CR > 0) { MsgClass.Add(MESSAGE.DRnCRHaveValue); }
            if (MyVoucher.Detail.DR == 0 && MyVoucher.Detail.CR == 0) { MsgClass.Add(MESSAGE.DRnCRAreZero); }
            if (MyVoucher.Detail.Description.Length == 0) { MsgClass.Add(MESSAGE.DescriptionIsNothing); }
            if (MsgClass.Count > 0) { IsValid = false; }

            return IsValid;

        }
        #endregion

        #region Navigation


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


        public void CalculateTotal()
        {
            Tot_DR = 0; Tot_CR = 0;
            if (MyVoucher.Details.Count > 0)
            {
                Tot_DR = MyVoucher.Details.Sum(e => e.DR);
                Tot_CR = MyVoucher.Details.Sum(e => e.CR);
            }
        }
        #endregion

        #region Print
        public async void Print(ReportActionClass _ReportAction)
        {
            try
            {
                ReportService = new(AppGlobals); ;
                ReportService.ReportType = _ReportAction.PrintType;

                await GetReportDataAsync();
                await CreateReportModelAsync();
                await ReportService.PrintAsync();

                if(ReportService.IsError)
                {
                    MsgClass.Add(ReportService.MyMessage.First(), AppMessages.Enums.Class.Danger);
                }


            }
            catch (Exception error)
            {
                MsgClass.Add(error.Message);
            }



        }

        public async Task GetReportDataAsync()
        {
            await Task.Run(() =>
            {
                ReportService.Data.ReportTable = Source.GetBookVoucher(VoucherID);
                ReportService.Data.DataSetName = "ds_CashBank";   // ds_CashBank

            });
        }

        public async Task CreateReportModelAsync()
        {
            await Task.Run(() =>
            {
                ReportService.Model.InputReport.FileName = "CashBankBook.rdl";

                ReportService.Model.ReportDataSource = ReportService.Data;
                ReportService.Model.OutputReport.FileName = $"{BookNatureTitle}-{MyVoucher.Master.Vou_No}";

                string _Heading1 = BookNatureTitle;
                string _Heading2 = $"Voucher {MyVoucher.Master.Vou_No}";

                ReportService.Model.AddReportParameter("Heading1", _Heading1);
                ReportService.Model.AddReportParameter("Heading2", _Heading2);
                ReportService.Model.AddReportParameter("InWords", "Words");
                ReportService.Model.AddReportParameter("CurrencySign", "SAR");
                ReportService.Model.AddReportParameter("ShowImages", true.ToString());
            });
        }
        #endregion

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
}
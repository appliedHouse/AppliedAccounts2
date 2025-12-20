using AppliedAccounts.Data;
using AppliedAccounts.Models.Interface;
using AppliedAccounts.Services;
using AppliedDB;
using AppMessages;
using Microsoft.AspNetCore.Components;
using System.Data;
using Windows.System.RemoteSystems;
using MESSAGE = AppMessages.Enums.Messages;
using Tables = AppliedDB.Enums.Tables;

namespace AppliedAccounts.Models
{
    public class BookModel : IVoucher
    {

        #region Variables
        public long VoucherID { get; set; }
        public long BookID { get; set; }
        public long BookNature { get; set; }
        public string BookNatureTitle { get; set; }

        public Voucher MyVoucher { get; set; }
        public List<Detail> Deleted { get; set; } = [];
        public List<CodeTitle> Companies { get; set; } = [];
        public List<CodeTitle> Employees { get; set; } = [];
        public List<CodeTitle> Projects { get; set; } = [];
        public List<CodeTitle> Accounts { get; set; } = [];
        public List<CodeTitle> BookList { get; set; } = [];
        public GlobalService AppGlobal { get; set; }
        public DataSource Source { get; set; }
        public MessageClass MsgClass { get; set; }
        public DateTime LastVoucherDate { get; set; }
        public DateTime MinVouDate = AppRegistry.MinDate;
        public DateTime MaxVouDate { get; set; }
        public int Index { get; set; } = 0;
        public string DataFile { get; set; }
        public int Count => MyVoucher.Details.Count;
        public string MyMessage { get; set; }

        public decimal Tot_DR { get; set; }
        public decimal Tot_CR { get; set; }
        public bool IsWaiting { get; set; }
        public bool IsSaved { get; set; }
        public NavigationManager NavManager => AppGlobal.NavManager;
        public PrintService ReportService { get; set; }

        private int CashNatureID = 1;           // Default Nature Id = 1 for cash
        private int BankNatureID = 2;           // Default Nature Id = 2 for cash

        public string _Deleted = "deleted";


        #endregion

        #region Constructor
        public BookModel()
        {

        }
        public BookModel(long _VoucherID, long _BookID, GlobalService _AppGlobal)
        {
            AppGlobal = _AppGlobal;
            MsgClass = new();
            MyVoucher = new();

            try
            {
                BookID = _BookID;
                VoucherID = _VoucherID;
                DataFile = AppGlobal.DBFile;

                CashNatureID = AppRegistry.GetNumber(DataFile, "CashBKNature");
                BankNatureID = AppRegistry.GetNumber(DataFile, "BankBKNature");

                if (AppGlobal.AppPaths != null)
                {
                    Source = new(AppGlobal.AppPaths);
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

                    BookNature = 1;         // Default value;

                    var result = Source.SeekValue(Tables.COA, BookID, "Nature") ?? 0;
                    if (result.GetType() != typeof(long))
                    {
                        long.TryParse(result.ToString(), out long val);
                        BookNature = val;
                    }
                    if (BookNature > 0)
                    { BookList = Source.GetBookAccounts(BookNature); }

                    if (BookNature == CashNatureID) { BookNatureTitle = "Cash Book"; }
                    if (BookNature == BankNatureID) { BookNatureTitle = "Bank Book"; }

                }
                else
                {
                    MsgClass.Alert(MESSAGE.UserProfileIsNull);
                }


            }
            catch (Exception ex)
            {
                MsgClass.Danger(ex.Message);


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
                            BookID = VoucherData.Select(row => row.Field<long>("BookID")).First();

                            MyVoucher.Master = VoucherData!.Select(first => new Master()
                            {
                                ID1 = first.Field<long>("ID1"),
                                Vou_No = first.Field<string?>("Vou_No") ?? "",
                                Vou_Date = first.Field<DateTime>("Vou_Date"),
                                BookID = first.Field<long>("BookID"),
                                Amount = first.Field<decimal>("Amount"),
                                Ref_No = first.Field<string?>("Ref_No") ?? "",
                                SheetNo = first.Field<string?>("SheetNo") ?? "",
                                Remarks = first.Field<string?>("Remarks") ?? "",
                                Status = first.Field<string?>("Status") ?? "Submitted"
                            }).First() ?? new();

                            MyVoucher.Details = [.. VoucherData.Select(row => new Detail()
                            {
                                ID2 = row.Field<long>("ID2"),
                                TranID = row.Field<long>("TranID"),
                                Sr_No = row.Field<int>("SR_NO"),
                                COA = row.Field < long?>("COA") ?? 0,
                                Company = row.Field<long?>("Company") ?? 0,
                                Employee = row.Field<long?>("Employee") ?? 0,
                                Project = row.Field<long?>("Project") ?? 0,
                                DR = row.Field<decimal>("DR"),
                                CR = row.Field<decimal>("CR"),
                                Description = row.Field<string?>("Description") ?? "",
                                Comments = row.Field<string?>("Comments") ?? "",
                                action = "get",
                                DetailGuid = Guid.NewGuid(),            // Record unique ID for delete records.

                                TitleAccount = row.Field<string?>("TitleCOA") ?? "",
                                TitleCompany = row.Field<string?>("TitleCompany") ?? "",
                                TitleEmployee = row.Field<string?>("TitleEmployee") ?? "",
                                TitleProject = row.Field<string?>("TitleProject") ?? ""

                            })];

                            if (MyVoucher.Details.Count > 0) { MyVoucher.Detail = MyVoucher.Details.First(); }

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
                _Detail.DetailGuid = Guid.NewGuid();

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

        #region New, Add / Edit 
        public void New()
        {
            MyVoucher.Detail = NewDetail();
        }

        public void Edit(int _SrNo)
        {
            var _Detail = MyVoucher.Detail;
            MyVoucher.Detail = MyVoucher.Details.Where(e => e.Sr_No == _SrNo).First() ?? _Detail;

        }



        #endregion

        #region Save
        public void Save()
        {
            if (IsTransValidated())
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
                    #region Delete transaction marked as delete
                    
                    try
                    {
                        if(MyVoucher.Details.Any(e=> e.action == _Deleted))
                        {

                        }


                    }
                    catch (Exception error)
                    {
                        MsgClass.Danger(error.Message);
                    }
                    #endregion


                    if (IsVoucherValidated())
                    {
                        try
                        {
                            var IsVoucherNumValid = true;
                            if (MyVoucher.Master.Vou_No.ToUpper().Equals("NEW"))
                            {
                                IsVoucherNumValid = false;
                                if (BookNature == CashNatureID)         // Cash Book Nature
                                {
                                    MyVoucher.Master.Vou_No = NewVoucherNo.GetCashVoucher(AppGlobal.DBFile, MyVoucher.Master.Vou_Date);
                                    IsVoucherNumValid = true;
                                }

                                else if (BookNature == BankNatureID)         // Bank Book Nature
                                {
                                    MyVoucher.Master.Vou_No = NewVoucherNo.GetBankVoucher(AppGlobal.DBFile, MyVoucher.Master.Vou_Date);
                                    IsVoucherNumValid = true;
                                }
                            }

                            if (IsVoucherNumValid)
                            {
                                var Row1 = Source.GetNewRow(Tables.Book);

                                Row1["ID"] = MyVoucher.Master.ID1;
                                Row1["Vou_No"] = MyVoucher.Master.Vou_No;
                                Row1["Vou_Date"] = MyVoucher.Master.Vou_Date;
                                Row1["BookID"] = MyVoucher.Master.BookID;
                                Row1["Ref_No"] = MyVoucher.Master.Vou_Date;
                                Row1["SheetNo"] = string.IsNullOrWhiteSpace(MyVoucher.Master.SheetNo) ? DBNull.Value : MyVoucher.Master.SheetNo;
                                Row1["Remarks"] = MyVoucher.Master.Remarks;
                                Row1["Status"] = MyVoucher.Master.Status;
                                Row1["Amount"] = MyVoucher.Details.Sum(e => e.DR) - MyVoucher.Details.Sum(e => e.CR);

                                CommandClass cmdClass1 = new(Row1, Source.MyConnection);
                                var saved1 = cmdClass1.SaveChanges();

                                if (saved1)             // if (voucher master record save successfully)
                                {
                                    VoucherID = cmdClass1.PrimaryKeyID;
                                    Row1["ID"] = VoucherID;                // Get a new Id of record after save / insert.

                                    var Row2 = Source.GetNewRow(Tables.Book2);
                                    var _NewSrNo = 1;

                                    foreach (var item in MyVoucher.Details)
                                    {
                                        if(item.action == _Deleted)
                                        {
                                            Row2["ID"] = item.ID2;
                                            CommandClass cmdClassDel = new(Row2, Source.MyConnection);
                                            var deleted = cmdClassDel.DeleteRow();
                                            if(!deleted)
                                            {
                                                MsgClass.Danger(MESSAGE.VouTransNotDeleted);
                                                // Roll Back master record
                                                IsSaved = false;
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            Row2["ID"] = item.ID2;
                                            Row2["TranID"] = VoucherID;
                                            Row2["SR_NO"] = _NewSrNo; _NewSrNo++;
                                            Row2["COA"] = item.COA;
                                            Row2["Company"] = item.Company == 0 ? DBNull.Value : item.Company;
                                            Row2["Employee"] = item.Employee == 0 ? DBNull.Value : item.Employee;
                                            Row2["Project"] = item.Project == 0 ? DBNull.Value : item.Project;
                                            Row2["DR"] = item.DR;
                                            Row2["CR"] = item.CR;
                                            Row2["Description"] = string.IsNullOrWhiteSpace(item.Description) ? DBNull.Value : item.Description;
                                            Row2["Comments"] = string.IsNullOrWhiteSpace(item.Comments) ? DBNull.Value : item.Comments;

                                            CommandClass cmdClass2 = new(Row2, Source.MyConnection);
                                            var save2 = cmdClass2.SaveChanges();
                                            if (!save2)
                                            {
                                                MsgClass.Error(MESSAGE.NotSave);
                                                var stop = true;
                                                // roll Bank master record from Data Table
                                            }
                                        }

                                        
                                    }

                                    IsWaiting = false;
                                }
                                else
                                {
                                    MsgClass.Error(MESSAGE.NotSave);
                                }
                            }
                            else
                            {
                                MsgClass.Alert(MESSAGE.VouNoNotDefineProperly);
                            }
                        }

                        catch (Exception ex)
                        {
                            IsSaved = false;
                            MsgClass.Danger(ex.Message);
                        }
                    }
                });
                IsWaiting = false;
            }
            return IsSaved;
        }


        #endregion

        #region Remove
        public void Remove(int _SrNo)
        {
            if (_SrNo > 0 && _SrNo <= MyVoucher.Details.Max(sr => sr.Sr_No))
            {
                var _Trans = MyVoucher.Details.FirstOrDefault(sr => sr.Sr_No == _SrNo);
                if (_Trans != null)
                {
                    if (MyVoucher.Master.Vou_No == "NEW")
                    {
                        //_Trans.Sr_No = _Trans.Sr_No * -1;
                        //_Trans.action = "delete";
                        //Deleted.Add(_Trans);                     // Marked record as deleted.

                        MyVoucher.Details.Remove(_Trans);

                        if (MyVoucher.Details.Count > 0)
                        {
                            var _NewSrNo = 1;
                            foreach (var trans in MyVoucher.Details)
                            {
                                trans.Sr_No = _NewSrNo; _SrNo++;
                            }
                        }
                    }
                    else
                    {   // if (voucher is exist in database.
                        // Save a record as deleted marks to Deleted at save all function.
                        _Trans.Sr_No = _Trans.Sr_No * -1;
                        _Trans.action = _Deleted;
                        Deleted.Add(_Trans);                     // Marked record as deleted.
                    }
                }
            }
            if( _SrNo < 0)
            {
                var _Trans = MyVoucher.Details.FirstOrDefault(sr => sr.Sr_No == _SrNo);
                if(_Trans != null)
                {
                    _Trans.Sr_No = _Trans.Sr_No * -1;            // Undo Removed transaction
                    _Trans.action = "get";
                }
            }

        }

        #endregion

        #region Validation
        public bool IsVoucherValidated()
        {
            MsgClass.ClearMessages();
            bool IsValid = true;

            if (MyVoucher.Master.BookID == 0) { MsgClass.Add(MESSAGE.BookIDIsZero); }
            if (MyVoucher.Master.Vou_No.Length == 0) { MsgClass.Add(MESSAGE.VouNoNotDefine); }
            if (MyVoucher.Master.Vou_Date < AppRegistry.MinVouDate) { MsgClass.Add(MESSAGE.VouDateLess); }
            if (MyVoucher.Master.Vou_Date > AppRegistry.MaxVouDate) { MsgClass.Add(MESSAGE.VouDateMore); }
            if (MyVoucher.Master.Remarks.Length == 0) { MsgClass.Add(MESSAGE.Row_NoRemarks); }
            if (MyVoucher.Master.Status.Length == 0) { MsgClass.Add(MESSAGE.Row_NoStatus); }

            foreach (var Trans in MyVoucher.Details)
            {
                if (Trans.Sr_No == 0) { MsgClass.Add(MESSAGE.SerialNoIsZero); }
                if (Trans.COA == 0) { MsgClass.Add(MESSAGE.Row_COAIsZero); }
                if (Trans.DR > 0 && Trans.CR > 0) { MsgClass.Add(MESSAGE.DRnCRHaveValue); }
                if (Trans.DR == 0 && Trans.CR == 0) { MsgClass.Add(MESSAGE.DRnCRAreZero); }
                if (string.IsNullOrEmpty(Trans.Description)) { MsgClass.Add(MESSAGE.DescriptionIsNothing); }

            }
            return IsValid;

        }

        public bool IsTransValidated()
        {
            bool IsValid = true;


            MsgClass = new();
            if (MyVoucher.Master.BookID == 0) { MsgClass.Add(MESSAGE.BookIDIsZero); }
            if (MyVoucher.Master.Vou_No.Length == 0) { MsgClass.Add(MESSAGE.VouNoNotDefine); }
            if (MyVoucher.Master.Vou_Date < AppRegistry.MinVouDate) { MsgClass.Add(MESSAGE.VouDateLess); }
            if (MyVoucher.Master.Vou_Date > AppRegistry.MaxVouDate) { MsgClass.Add(MESSAGE.VouDateMore); }
            if (MyVoucher.Master.Remarks.Length == 0) { MsgClass.Add(MESSAGE.Row_NoRemarks); }
            if (MyVoucher.Master.Status.Length == 0) { MsgClass.Add(MESSAGE.Row_NoStatus); }
            if (MyVoucher.Detail.Sr_No == 0) { MsgClass.Add(MESSAGE.SerialNoIsZero); }
            if (MyVoucher.Detail.COA == 0) { MsgClass.Add(MESSAGE.Row_COAIsZero); }
            if (MyVoucher.Detail.DR > 0 && MyVoucher.Detail.CR > 0) { MsgClass.Add(MESSAGE.DRnCRHaveValue); }
            if (MyVoucher.Detail.DR == 0 && MyVoucher.Detail.CR == 0) { MsgClass.Add(MESSAGE.DRnCRAreZero); }
            if (string.IsNullOrEmpty(MyVoucher.Detail.Description)) { MsgClass.Add(MESSAGE.DescriptionIsNothing); }
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
        public void Print(ReportActionClass _ReportAction)
        {
            try
            {
                ReportService = new(AppGlobal); ;
                ReportService.ReportType = _ReportAction.PrintType;

                GetReportDataAsync();
                CreateReportModelAsync();
                ReportService.Print();

                if (ReportService.IsError)
                {
                    MsgClass.Add(ReportService.MyMessage.First(), AppMessages.Enums.Class.Danger);
                }
            }
            catch (Exception error)
            {
                MsgClass.Add(error.Message);
            }



        }

        public void GetReportDataAsync()
        {
            ReportService.Data.ReportTable = Source.GetBookVoucher(VoucherID);
            ReportService.Data.DataSetName = "ds_CashBank";   // ds_CashBank
        }

        public void CreateReportModelAsync()
        {
            ReportService.Model.InputReport.FileName = "CashBankBook.rdl";

            ReportService.Model.OutputReport.ReportType = ReportService.ReportType;
            ReportService.Model.ReportDataSource = ReportService.Data;
            ReportService.Model.OutputReport.FileName = $"{BookNatureTitle}-{MyVoucher.Master.Vou_No}";

            string _Heading1 = BookNatureTitle;
            string _Heading2 = $"Voucher {MyVoucher.Master.Vou_No}";

            ReportService.Model.AddReportParameter("Heading1", _Heading1);
            ReportService.Model.AddReportParameter("Heading2", _Heading2);
            ReportService.Model.AddReportParameter("InWords", "Words");
            ReportService.Model.AddReportParameter("CurrencySign", "SAR");
            ReportService.Model.AddReportParameter("ShowImages", true.ToString());
        }

        public void Edit(long _ID2)
        {
            throw new NotImplementedException();
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
            public long ID1 { get; set; }
            public string Vou_No { get; set; }
            public DateTime Vou_Date { get; set; }
            public long BookID { get; set; }
            public decimal Amount { get; set; }
            public string Ref_No { get; set; }
            public string SheetNo { get; set; }
            public string Remarks { get; set; }
            public string Status { get; set; }


        }
        public class Detail
        {
            public Detail() { }
            public long ID2 { get; set; }
            public long TranID { get; set; }
            public int Sr_No { get; set; }
            public long COA { get; set; }
            public long Company { get; set; }
            public long Employee { get; set; }
            public long Project { get; set; }
            public decimal DR { get; set; }
            public decimal CR { get; set; }
            public string Description { get; set; }
            public string Comments { get; set; }
            public string action { get; set; }

            public string TitleAccount { get; set; }
            public string TitleCompany { get; set; }
            public string TitleProject { get; set; }
            public string TitleEmployee { get; set; }

            public Guid DetailGuid { get; set; }

        }
        #endregion
    }
}
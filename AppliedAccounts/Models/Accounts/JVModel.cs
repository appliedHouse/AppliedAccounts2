using AppliedAccounts.Data;
using AppliedAccounts.Models.Interface;
using AppliedAccounts.Services;
using AppliedAccounts.Pages.Accounts.Post;
using AppliedDB;
using AppMessages;
using System.Data;

using Messages = AppMessages.Enums.Messages;

namespace AppliedAccounts.Models.Accounts
{
    public class JVModel : IVoucher
    {

        #region Variables
        public List<JVViewModel> MyVoucher { get; set; }
        public JVViewModel Transaction { get; set; }
        public GlobalService AppGlobal { get; set; }
        public DateTime LastVoucherDate { get; set; }
        public DateTime MaxVouDate { get; set; }
        public MessageClass MsgClass { get; set; }
        public PrintService ReportService { get; set; }
        public DataSource Source { get; set; }
        public List<CodeTitle> Companies { get; set; }
        public List<CodeTitle> Employees { get; set; }
        public List<CodeTitle> Projects { get; set; }
        public List<CodeTitle> Accounts { get; set; }
        public BrowseModel BrowseClass { get; set; } = new();


        public int Index { get; set; }

        public int Count => MyVoucher.Count;
        public decimal TotalAmount => MyVoucher.Sum(v => v.DR);
        public bool IsBalanced => Tot_DR == Tot_CR && (Tot_DR != 0 || Tot_CR != 0);

        public decimal Tot_DR { get => MyVoucher.Sum(e => e.DR); set => MyVoucher.Sum(e => e.DR); }
        public decimal Tot_CR { get => MyVoucher.Sum(e => e.CR); set => MyVoucher.Sum(e => e.CR); }
        public bool IsWaiting { get; set; }

        public string MyMessage { get; set; }
        private NumInWords InWords { get; set; }
        public string Currency { get; set; } = "Rs.";
        public string CurrencyUnit { get; set; } = "Pessa";

        public string Vou_No { get; set; }
        public long Vou_ID { get; set; }

        public DataRow CurrentRow { get; set; }
        public DataRow NewRow { get; set; }

        #endregion

        #region Constructor
        public JVModel(GlobalService _AppGlobal, string _VouNo)
        {
            AppGlobal = _AppGlobal;
            Start(_VouNo);

        }

        public JVModel(GlobalService _AppGlobal)
        {
            AppGlobal = _AppGlobal;
            Start("New");
        }


        public void Start(string _VouNo)
        {
            Source = new(AppGlobal.AppPaths);

            MsgClass = new();
            MyVoucher = new();
            ReportService = new();

            Currency = Source.GetText("Currency");
            CurrencyUnit = Source.GetText("CurrencyUnit");
            InWords = new(Currency, CurrencyUnit);

            Companies = Source.GetCustomers();
            Employees = Source.GetEmployees();
            Projects = Source.GetProjects();
            Accounts = Source.GetAccounts();

            MyVoucher = new();
            Transaction = new();

            Vou_No = _VouNo ?? "New";
            NewRow = Source.GetNewRow(AppliedDB.Enums.Tables.Ledger);
            CurrentRow = NewRow;

            LoadData();
            GetKeys();

        }

        #endregion

        public void CalculateTotal()
        {
            var Total = Tot_DR - Tot_CR;
        }

        #region Load Data
        public bool LoadData()
        {
            try
            {



                if (string.IsNullOrWhiteSpace(Vou_No))
                {
                    MsgClass.Alert(Messages.VoucherNumberEmpty);
                    return false;
                }

                if (Vou_No == "New")
                {
                    MyVoucher = new();
                    Transaction = new();
                    Transaction.Sr_No = 1;
                    Transaction.Vou_Date = Source.GetDate("JV");
                    return true;
                }

                var table = Source.GetJV(Vou_No);

                if (table == null || table.Rows.Count == 0) { return false; }

                MyVoucher = [.. table.AsEnumerable().Select(e => new JVViewModel()
                        {
                            ID = e.Field<long>("ID"),
                            Vou_No = e.Field<string>("Vou_No") ?? "",
                            Vou_Date = e.Field<DateTime>("Vou_Date"),
                            Sr_No = e.Field<int>("Sr_No"),
                            COA = e.Field<long?>("COA") ?? 0,
                            DR = e.Field<decimal?>("DR") ?? 0.0M,
                            CR = e.Field<decimal?>("CR") ?? 0.0M,
                            Description = e.Field<string>("Description") ?? "",
                            Comments = e.Field<string>("Comments") ?? "",
                            Company = e.Field<long?>("Customer") ?? 0,
                            Project = e.Field<long?>("Project") ?? 0,
                            Employee = e.Field<long?>("Employee") ?? 0,

                            TitleCompany = e.Field<string?>("TitleCompany") ?? "",
                            TitleProject = e.Field<string?>("TitleProject") ?? "",
                            TitleAccount = e.Field<string?>("TitleAccount") ?? "",
                            TitleEmployee = e.Field<string?>("TitleEmployee") ?? "",
                        })];

                if (MyVoucher.Count > 0) { Transaction = MyVoucher.First(); return true; }
                MsgClass.Alert(Messages.VoucherNotFound); return false;
            }
            catch (Exception error)
            {
                MsgClass.Error(error.Message);
            }
            return false;
        }

        #endregion

        #region Validation
        public bool IsTransValidated()
        {
            MsgClass = new();


            if (Transaction.DR + Transaction.CR == 0) { MsgClass.Alert(Messages.VoucherAmountIsZero); }
            if (string.IsNullOrEmpty(Transaction.Description)) { MsgClass.Alert(Messages.DescriptionIsNull); }
            if (Transaction.COA.Equals(0)) { MsgClass.Alert(Messages.AccountIDIsZero); }
            if (Transaction.DR > 0 && Transaction.CR > 0) { MsgClass.Alert(Messages.DRnCRHaveValue); }

            return MsgClass.Count == 0;
        }

        public bool IsVoucherValidated()
        {
            MessageClass SaveMessages = new();
            var CurrentTransaction = Transaction.Clone();
            var _date = MyVoucher.First().Vou_Date;

            foreach (var item in MyVoucher)
            {
                if (item.Vou_Date != _date) { SaveMessages.Alert(Messages.VoucherDateNotSame); }

                if (!IsTransValidated())
                {
                    SaveMessages.AddReange(MsgClass.MessageList);
                    break;
                }
            }
            if (!Tot_DR.Equals(Tot_CR)) { SaveMessages.Alert(Messages.VoucherAmountNotEqual); }
            Transaction = CurrentTransaction;
            return SaveMessages.Count == 0;
        }


        #endregion

        #region Edit / Add
        public void Edit(long _ID2)
        {
            Transaction = MyVoucher.Where(e => e.Sr_No == _ID2).FirstOrDefault()!;
        }

        public void New()
        {
            if (IsTransValidated())
            {
                if (MyVoucher.Count > 0)
                {
                    var MaxSrNo = MyVoucher.Max(e => e.Sr_No);
                    Transaction = new()
                    {
                        Sr_No = MaxSrNo + 1,
                        Vou_No = MyVoucher.Last().Vou_No,
                        Vou_Date = MyVoucher.Last().Vou_Date,
                        Vou_Type = PostingTypes.JV.ToString(),
                        Status = PostingStatus.Submitted.ToString()
                    };
                }
            }

        }
        #endregion

        #region Navigation
        public void Top()
        {
            Index = 1;
            if (MyVoucher.Count > 0)
            {
                Transaction = MyVoucher.First();
            }
        }

        public void Next()
        {
            if (MyVoucher.Count > 0)
            {
                Index = MyVoucher.IndexOf(Transaction) + 1;
                var Counter = MyVoucher.Count - 1;
                if (Index > Counter) { Index = Counter; }
                Transaction = MyVoucher[Index];
            }
        }

        public void Back()
        {
            if (MyVoucher.Count > 0)
            {
                Index = MyVoucher.IndexOf(Transaction) - 1;
                if (Index < 0) { Index = 0; }
                Transaction = MyVoucher[Index];
            }
        }

        public void Last()
        {
            Index = MyVoucher.Count - 1;
            if (MyVoucher.Count > 0)
            {
                Transaction = MyVoucher.Last();
            }
        }
        #endregion


        #region Remove / Delete
        public void Remove(int _SrNo)
        {
            if (Transaction.ID == 0)
            {
                MyVoucher.Remove(Transaction);
            }
            else if (MyVoucher.Count > 0)
            {
                Transaction = MyVoucher.Where(e => e.Sr_No == _SrNo).First();
                Transaction.Sr_No = Transaction.Sr_No * -1;
            }
        }
        #endregion

        #region Save
        public void Save()
        {
            if (IsTransValidated())
            {
                var _Exist = MyVoucher.Where(e => e.Sr_No == Transaction.Sr_No).FirstOrDefault();
                if (_Exist == null)
                {
                    MyVoucher.Add(Transaction);
                }
            }

        }

        public async Task<bool> SaveAllAsync()
        {
            SetKey();
            try
            {
                // SELECT * FROM [Ledger] WHERE [Vou_Type]='JV' AND Vou_No='New' 
                var _Ledger = Source.GetTable(AppliedDB.Enums.Tables.Ledger, $"[Vou_Type]='{VoucherTypeClass.VoucherType.JV}' AND Vou_No='{Vou_No}'");
                if (_Ledger.Columns.Count > 0)
                {
                    // Update Voucher date must be same in all voucher list. 
                    var _date = MyVoucher.First().Vou_Date;
                    MyVoucher.ForEach(e => { e.Vou_Date = _date; });

                    if (IsVoucherValidated())
                    {


                        if (Transaction.Vou_No == "New")
                        {
                            Vou_No = NewVoucherNo.GetJournalVoucher(AppGlobal.DBFile, Transaction.Vou_Date);
                            MyVoucher.ForEach(e => e.Vou_No = Vou_No);
                        }

                        Source.BeginTransaction();

                        // Delete existing transactions if (mark as deleted) sr_no < 0
                        var _DeletedTrans = MyVoucher.Where(e => e.Sr_No < 0).ToList();
                        foreach (var _Voucher in _DeletedTrans)
                        {
                            CurrentRow = Convert2Row(_Voucher);
                            Source.Delete(CurrentRow);
                        }

                        var _SrNo = 1;

                        foreach (var Tran in MyVoucher)
                        {
                            CurrentRow = Convert2Row(Tran);
                            CurrentRow["Sr_No"] = _SrNo; _SrNo++;
                            if ((long)CurrentRow["Customer"] == 0) { CurrentRow["Customer"] = DBNull.Value; }
                            if ((long)CurrentRow["Employee"] == 0) { CurrentRow["Employee"] = DBNull.Value; }
                            if ((long)CurrentRow["Project"] == 0) { CurrentRow["Project"] = DBNull.Value; }
                            Source.Save(CurrentRow);
                        }


                        Source.CommitTransaction();
                        return await Task.FromResult(true);
                    }
                }

            }
            catch (Exception error)
            {
                MsgClass.Error(error.Message);
                Source.RollbackTransaction();

            }
            return await Task.FromResult(false);

        }

        private DataRow Convert2Row(JVViewModel _VModel)
        {
            if (CurrentRow == null) { CurrentRow = Source.GetNewRow(AppliedDB.Enums.Tables.Ledger); }

            CurrentRow["ID"] = _VModel.ID;
            CurrentRow["Vou_No"] = _VModel.Vou_No;
            CurrentRow["Vou_Date"] = _VModel.Vou_Date;
            CurrentRow["Vou_Type"] = VoucherTypeClass.VoucherType.JV.ToString();
            CurrentRow["Sr_No"] = _VModel.Sr_No;
            CurrentRow["Ref_No"] = _VModel.Ref_No;
            CurrentRow["BookID"] = DBNull.Value;
            CurrentRow["COA"] = _VModel.COA;
            CurrentRow["DR"] = _VModel.DR;
            CurrentRow["CR"] = _VModel.CR;
            CurrentRow["Customer"] = _VModel.Company;
            CurrentRow["Employee"] = _VModel.Employee;
            CurrentRow["Inventory"] = DBNull.Value;
            CurrentRow["Project"] = _VModel.Project;
            CurrentRow["Description"] = _VModel.Description;
            CurrentRow["Comments"] = DBNull.Value;
            CurrentRow["Status"] = PostingStatus.Submitted.ToString();

            return CurrentRow;
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
            ReportService.Data.ReportTable = Source.GetJV(Transaction.Vou_No)!;
            ReportService.Data.DataSetName = "ds_JV";   // ds_CashBank
        }

        public void CreateReportModelAsync()
        {
            ReportService.Model.InputReport.FileName = "JV.rdl";

            ReportService.Model.OutputReport.ReportType = ReportService.ReportType;
            ReportService.Model.ReportDataSource = ReportService.Data;
            ReportService.Model.OutputReport.FileName = $"JV-{Transaction.Vou_No}";

            string _Heading1 = "Journal Voucher";
            string _Heading2 = $"Voucher {Transaction.Vou_No}";

            ReportService.Model.AddReportParameter("Heading1", _Heading1);
            ReportService.Model.AddReportParameter("Heading2", _Heading2);
            ReportService.Model.AddReportParameter("InWords", InWords.ChangeCurrencyToWords(TotalAmount.ToString()));
            ReportService.Model.AddReportParameter("CurrencySign", Currency);
            ReportService.Model.AddReportParameter("ShowImages", true.ToString());
        }




        #endregion

        #region Get / Set Keys
        private void GetKeys()
        {
            if (Transaction != null)
            {
                Transaction.Vou_Date = Source.GetDate("JV");
            }
        }

        private void SetKey()
        {
            Source.SetKey("JV", Transaction.Vou_Date, AppliedGlobals.AppErums.KeyTypes.Date, "Journal Voucher");
        }
        #endregion
    }
}

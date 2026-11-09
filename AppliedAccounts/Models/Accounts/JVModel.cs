using AppliedAccounts.Data;
using AppliedAccounts.Data.Mapping;
using AppliedAccounts.Models.Interface;
using AppliedAccounts.Pages.Accounts.Post;
using AppliedAccounts.Services;
using AppliedDB;
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
        public MessagesService MsgService { get; set; }
        public PrintService ReportService { get; set; }
        public DataSource Source { get; set; }
        public List<CodeTitle> Companies { get; set; }
        public List<CodeTitle> Employees { get; set; }
        public List<CodeTitle> Projects { get; set; }
        public List<CodeTitle> Accounts { get; set; }
        public BrowseModel BrowseClass { get; set; } = new();


        public int Index { get; set; }

        public int Count => MyVoucher.Count;
        public decimal TotalAmount => MyVoucher.Where(e=> e.Sr_No > 0).Sum(v => v.DR);
        public bool IsBalanced => Tot_DR == Tot_CR && (Tot_DR != 0 || Tot_CR != 0);

        public decimal Tot_DR => GetDRAmount(); 
        public decimal Tot_CR => GetCRAmount(); 

       
        public bool IsWaiting { get; set; }
        public string MyMessage { get; set; }
        private NumInWords InWords { get; set; }
        public string Currency { get; set; } = "Rs.";
        public string CurrencyUnit { get; set; } = "Pessa";

        public string Vou_No { get; set; }
        public long Vou_ID { get; set; }

        public DataRow CurrentRow { get; set; }
        public DataRow NewRow { get; set; }
        decimal IVoucher.Tot_DR { get => Tot_DR; set => throw new NotImplementedException(); }
        decimal IVoucher.Tot_CR { get => Tot_CR; set => throw new NotImplementedException(); }

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

            //
            MyVoucher = new();
            ReportService = new();

            Currency = Source.GetText("Currency");
            CurrencyUnit = Source.GetText("CurrencyUnit");
            InWords = new(Currency, CurrencyUnit);

            Companies = Source.GetCustomers();
            Employees = Source.GetEmployees();
            Projects = Source.GetProjects();
            Accounts = Source.GetAccounts();

            MyVoucher = [];
            Transaction = new();

            Vou_No = _VouNo ?? "New";
            NewRow = Source.GetNewRow(Enums.Tables.Ledger);
            CurrentRow = NewRow;

            GetKeys();
            LoadData();


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
                    MsgService.Alert(Messages.VoucherNumberEmpty);
                    return false;
                }

                if (Vou_No == "New" || string.IsNullOrWhiteSpace(Vou_No))
                {
                    Transaction.Vou_No = "New";
                    Transaction.Sr_No = 1;
                    Transaction.Vou_Date = Source.GetDate("JV");
                    Transaction.Vou_Type = PostingTypes.JV.ToString();
                    Transaction.Status = PostingStatus.Submitted.ToString();
                    return true;
                }

                var table = Source.GetJV(Vou_No);

                if (table == null || table.Rows.Count == 0) { return false; }

                MyVoucher = [.. table.AsEnumerable().Select(e => new JVViewModel()
                        {
                            ID = e.Field<long>("ID"),
                            Vou_No = e.Field<string>("Vou_No") ?? "",
                            Vou_Date = e.Field<DateTime>("Vou_Date"),
                            Vou_Type = e.Field<string>("Vou_Type") ?? "",
                            Ref_No = e.Field<string>("Ref_No") ?? "",
                            Sr_No = e.Field<int>("Sr_No"),
                            COA = e.Field<long?>("COA") ?? 0,
                            DR = e.Field<decimal?>("DR") ?? 0.0M,
                            CR = e.Field<decimal?>("CR") ?? 0.0M,
                            Description = e.Field<string>("Description") ?? "",
                            Comments = e.Field<string>("Comments") ?? "",
                            Company = e.Field<long?>("Customer") ?? 0,
                            Project = e.Field<long?>("Project") ?? 0,
                            Employee = e.Field<long?>("Employee") ?? 0,
                            Status = e.Field<string>("Status") ?? "",

                            TitleCompany = e.Field<string?>("TitleCompany") ?? "",
                            TitleProject = e.Field<string?>("TitleProject") ?? "",
                            TitleAccount = e.Field<string?>("TitleAccount") ?? "",
                            TitleEmployee = e.Field<string?>("TitleEmployee") ?? "",
                        })];

                if (MyVoucher.Count > 0) { Transaction = MyVoucher.First(); return true; }
                MsgService.Alert(Messages.VoucherNotFound); return false;
            }
            catch (Exception error)
            {
                MsgService.Error(error.Message);
            }
            return false;
        }

        #endregion

        #region Validation
        public bool IsTransValidated()
        {
            //


            if (Transaction.DR + Transaction.CR == 0) { MsgService.Alert(Messages.VoucherAmountIsZero); }
            if (string.IsNullOrEmpty(Transaction.Description)) { MsgService.Alert(Messages.DescriptionIsNull); }
            if (Transaction.COA.Equals(0)) { MsgService.Alert(Messages.AccountIDIsZero); }
            if (Transaction.DR > 0 && Transaction.CR > 0) { MsgService.Alert(Messages.DRnCRHaveValue); }

            return MsgService.Count == 0 ? true : false;
        }

        public bool IsVoucherValidated()
        {
            MsgService.Clear();
            var CurrentTransaction = Transaction.Clone();
            var _date = MyVoucher.First().Vou_Date;

            foreach (var item in MyVoucher)
            {
                if (item.Vou_Date != _date) { MsgService.Alert(Messages.VoucherDateNotSame); }

                if (!IsTransValidated())
                {
                    MsgService.AddRange(MsgService.MsgClass);
                    break;
                }
            }
            if (!Tot_DR.Equals(Tot_CR)) { MsgService.Alert(Messages.VoucherAmountNotEqual); }
            Transaction = CurrentTransaction;
            return MsgService.Count == 0;
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

        public void Remove(int srNo)
        {
            var item = MyVoucher.FirstOrDefault(e => e.Sr_No == srNo);
            if (item == null) return;
            if (item.ID == 0) MyVoucher.Remove(item);          // never saved: just drop it
            else item.Sr_No = -Math.Abs(item.Sr_No);           // saved: mark for delete on Save
        }
        
        #endregion

        #region Save
        public void Save()
        {
            if (IsTransValidated())
            {
                var _Exist = MyVoucher.FirstOrDefault(e => e.Sr_No == Transaction.Sr_No);
                if (_Exist == null)
                {
                    MyVoucher.Add(Transaction);
                }
            }

        }


        public Task<bool> SaveAllAsync() => Task.FromResult(SaveVoucher());

        private bool SaveVoucher()
        {
            var isNew = Vou_No == "New";
            var inTransaction = false;
            try
            {
                var lines = MyVoucher.Where(e => e.Sr_No > 0).ToList();
                if (lines.Count > 0)
                {
                    var date = lines[0].Vou_Date;               // one date for the whole voucher
                    MyVoucher.ForEach(e => e.Vou_Date = date);
                }
                if (!IsVoucherValidated()) return false;

                if (isNew)
                {
                    Vou_No = NewVoucherNo.GetJournalVoucher(AppGlobal.DBFile, lines[0].Vou_Date);
                    MyVoucher.ForEach(e => e.Vou_No = Vou_No);
                }

                Source.BeginTransaction(); inTransaction = true;

                foreach (var deleted in MyVoucher.Where(e => e.Sr_No < 0).ToList())
                {
                    CurrentRow = Convert2Row(deleted);
                    Source.Delete(CurrentRow);
                }

                var srNo = 1;
                foreach (var tran in lines)                    // positive lines only
                {
                    CurrentRow = Convert2Row(tran);
                    CurrentRow["Sr_No"] = srNo++;
                    if ((long)CurrentRow["Customer"] == 0) CurrentRow["Customer"] = DBNull.Value;
                    if ((long)CurrentRow["Employee"] == 0) CurrentRow["Employee"] = DBNull.Value;
                    if ((long)CurrentRow["Project"] == 0) CurrentRow["Project"] = DBNull.Value;
                    Source.Save(CurrentRow);
                }

                Source.CommitTransaction(); inTransaction = false;
                SetKey();
                LoadData();                                    // reload so lines get their IDs
                return true;
            }
            catch (Exception error)
            {
                if (inTransaction) Source.RollbackTransaction();
                if (isNew) { Vou_No = "New"; MyVoucher.ForEach(e => e.Vou_No = "New"); }
                MsgService.Error(error);
            }
            return false;
        }

        private DataRow Convert2Row(JVViewModel _VModel)
        {
            if (CurrentRow == null) { CurrentRow = Source.GetNewRow(Enums.Tables.Ledger); }
            return _VModel.ToDataRow(CurrentRow);
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
                    MsgService.Danger(ReportService.MyMessage.First());
                }
            }
            catch (Exception error)
            {
                MsgService.Error(error);
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

        #region Total Debit and Credit Amount
        private decimal GetDRAmount()
        {
            if (MyVoucher != null)
            {
                return MyVoucher.Where(e => e.Sr_No > 0).Sum(e => e.DR);
            }
            return 0.00M;
        }
        private decimal GetCRAmount()
        {
            if (MyVoucher != null)
            {
                return MyVoucher.Where(e => e.Sr_No > 0).Sum(e => e.CR);
            }
            return 0.00M;
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

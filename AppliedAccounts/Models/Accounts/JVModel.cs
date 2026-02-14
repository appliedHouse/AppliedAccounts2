using Applied_WebApplication.Data;
using AppliedAccounts.Data;
using AppliedAccounts.Models.Interface;
using AppliedAccounts.Services;
using AppliedDB;
using AppMessages;

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
        public int Index { get; set; }

        public int Count => MyVoucher.Count;
        public decimal TotalAmount => MyVoucher.Sum(v => v.DR);

        public decimal Tot_DR { get => MyVoucher.Sum(e => e.DR); set => MyVoucher.Sum(e => e.DR); }
        public decimal Tot_CR { get => MyVoucher.Sum(e => e.CR); set => MyVoucher.Sum(e => e.CR); }
        public bool IsWaiting { get; set; }

        public string MyMessage { get; set; }
        private NumInWords InWords { get; set; }
        public string Currency { get; set; } = "Rs.";
        public string CurrencyUnit { get; set; } = "Pessa";

        #endregion

        public JVModel(GlobalService _AppGlobal)
        {
            AppGlobal = _AppGlobal;
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
        }


        public void CalculateTotal()
        {
            var Total = Tot_DR - Tot_CR;
        }

        #region Load Data
        public bool LoadData()
        {
            return true;
        }
        #endregion

        #region Validation
        public bool IsTransValidated()
        {
            bool _result = true;
            var Total_DR = Tot_DR;
            var Total_CR = Tot_CR;
            if (!Total_CR.Equals(Total_DR)) { _result = false; }
            if (Transaction.DR + Transaction.CR == 0) { _result = false; }
            if (string.IsNullOrEmpty(Transaction.Description)) { _result = false; }
            if (Transaction.COA.Equals(0)) { _result = false; }

            return _result;
        }

        public bool IsVoucherValidated()
        {
            bool _result = true;
            var CurrentTransaction = Transaction.Clone();
            foreach (var item in MyVoucher)
            {
                if (!IsTransValidated())
                {
                    _result = false;
                    break;
                }
            }
            if (!Tot_DR.Equals(Tot_CR)) { _result = false; }
            Transaction = CurrentTransaction;
            return _result;
        }

        
        #endregion

        #region Edit / Add
        public void Edit(long _ID2)
        {
            return;
        }

        public void New()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Navigation
        public void Top()
        {
            throw new NotImplementedException();
        }

        public void Next()
        {
            throw new NotImplementedException();
        }

        public void Back()
        {
            throw new NotImplementedException();
        }

        public void Last()
        {
            throw new NotImplementedException();
        }
        #endregion


        #region Remove / Delete
        public void Remove(int _SrNo)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Save
        public void Save()
        {
            throw new NotImplementedException();
        }

        public Task<bool> SaveAllAsync()
        {
            throw new NotImplementedException();
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
    }
}

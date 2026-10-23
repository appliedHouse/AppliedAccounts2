// SettingGeneral.razor.cs
using AppliedDB;
using Menus;
namespace AppliedAccounts.Pages.Setting
{
    public partial class SettingGeneral
    {
        public GeneralSettngModel MyModel { get; set; } = new();
        private string UserName => AppGlobal.UserID;
        private bool IsAuthorized => AppGlobal.UserRole.Equals("Administrator") ? true : false;
        private DataSource Source;

        private List<CodeTitle> NatureList;
        private List<CodeTitle> AccountList;


        protected override void OnInitialized()
        {
            Source = new(AppGlobal.AppPaths);


            NatureList = Source.GetAccNature();
            AccountList = Source.GetAccounts();

            GetValues();
        }

        private void GetValues()
        {
            MyModel.CashNatureID = Source.GetNumber("CashBkNature");
            MyModel.BankNatureID = Source.GetNumber("BankBkNature");
        }

        private void SaveAsync()
        {
            Source.SetKey("CashBkNature", MyModel.CashNatureID);
            Source.SetKey("BankBkNature", MyModel.BankNatureID);

            Toaster.ShowSuccess("Setting values saved.");

        }

        private void CashNatureValueChange(long _ID)
        {
            MyModel.CashNatureID = _ID;
            MyModel.CashNatureTitleAccount = NatureList.FirstOrDefault(e => e.ID == _ID)?.Title ?? "";
        }

        private void BankNatureValueChange(long _ID)
        {
            MyModel.BankNatureID = _ID;
            MyModel.BankNatureTitleAccount = NatureList.FirstOrDefault(e => e.ID == _ID)?.Title ?? "";
        }

        protected void Back() { AppGlobal.NavManager.GoTo(MenuID.Setting); }
    }

    public class GeneralSettngModel
    {
        public long CashNatureID { get; set; }
        public long BankNatureID { get; set; }
        public long RevenueNatureID { get; set; }
        public List<long> ClientLedgeIds { get; set; }
        public string ClientLedgerRDLC { get; set; }
        public string ReceiptRDLC { get; set; }

        public DateTime FiscalYear1 { get; set; }
        public DateTime FiscalYear2 { get; set; }
        public DateTime VoucherDate1 { get; set; }
        public DateTime VoucherDate2 { get; set; }
        public DateTime OpeningDate { get; set; }

        public string CashNatureTitleAccount { get; set; }
        public string BankNatureTitleAccount { get; set; }

        public AccountsPurchase PayableAccounts { get; set; }
        public AccountsReceivable ReceivableAccounts { get; set; }

        public List<CodeTitle> NatureList { get; set; }
        public List<CodeTitle> AccountsList { get; set; }

    }

    public class AccountsPurchase
    {
        public long Purchase { get; set; }
        public long Tax { get; set; }
        public long Payable { get; set; }
    }

    public class AccountsReceivable
    {
        public long Stock { get; set; }
        public long Tax { get; set; }
        public long Receivable { get; set; }
    }
}

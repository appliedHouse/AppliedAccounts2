using AppliedAccounts.Authentication;
using AppliedAccounts.Services;
using AppliedDB;

namespace AppliedAccounts.Pages.Setting
{
    public partial class SettingGeneral
    {
        private DataSource? Source { get; set; }
        public GeneralSettngModel MyModel { get; set; } = new();

        protected override void OnInitialized()
        {
            var _User = ((UserAuthenticationStateProvider)AuthState).AppUser;

            Source = new(AppliedUsersService.ClientConnection(_User.UserID)!);

            MyModel.NatureList = Source.GetAccNature();
            MyModel.AccountsList = Source.GetAccounts();

            MyModel.CashNatureID = Source.GetNumber("CaskBkNature");
            MyModel.BankNatureID = Source.GetNumber("BankBkNature");

        }
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

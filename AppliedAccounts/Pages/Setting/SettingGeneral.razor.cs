namespace AppliedAccounts.Pages.Setting
{
    public partial class SettingGeneral
    {
        
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

        public AccountsPurchase PayableAccounts { get; set; }
        public AccountsReceivable ReceivableAccounts { get; set; }

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

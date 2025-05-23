namespace AppliedAccounts.Pages.Sale.Quotations
{
    public partial class QuotationsList
    {
        public int VoucherNo { get; set; } = 0;
        public bool IsPageValid { get; set; } = true;
        public QuotationListModel MyModel { get; set; } = new QuotationListModel();

        public QuotationsList()
        {
            IsPageValid = false;
        }

    }
}

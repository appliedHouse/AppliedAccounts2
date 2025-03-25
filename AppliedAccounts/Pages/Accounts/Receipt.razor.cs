using AppliedAccounts.Models;

namespace AppliedAccounts.Pages.Accounts
{
    public partial class Receipt
    {
        public ReceiptModel Model { get; set; }
        public bool IsPageValid { get; set; }
        public string ErrorMessage { get; set; }    

        public Receipt()
        {
            Model = new ReceiptModel();
            IsPageValid = true;
            ErrorMessage = string.Empty;
        }
    }
}

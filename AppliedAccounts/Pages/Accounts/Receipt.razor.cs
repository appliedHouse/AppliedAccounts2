using AppliedAccounts.Models;
using Microsoft.AspNetCore.Components;

namespace AppliedAccounts.Pages.Accounts
{
    public partial class Receipt
    {
        [Parameter] public int ID { get; set; }
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

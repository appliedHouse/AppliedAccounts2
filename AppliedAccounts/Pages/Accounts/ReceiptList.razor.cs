using AppliedAccounts.Data;
using AppliedAccounts.Models;
using AppliedDB;
using System.Data;


namespace AppliedAccounts.Pages.Accounts
{
    public partial class ReceiptList
    {
        public AppUserModel UserProfile { get; set; }
        public ReceiptListModel MyModel { get; set; }

        public ReceiptList()
        {
           
        }

    }
}

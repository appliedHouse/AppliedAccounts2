using AppliedAccounts.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace AppliedAccounts.Pages.Accounts
{
    public partial class Receipt : ComponentBase
    {
        public ReceiptModel MyModel { get; set; }
        public bool IsPageValid { get; set; }
        public string ErrorMessage { get; set; }    

        public Receipt()
        {
            MyModel = new ReceiptModel();
            IsPageValid = true;
            ErrorMessage = string.Empty;
        }

        private void AccountIDChanged(int _ID)
        {
            MyModel.MyVoucher.Master.COA = _ID;
            MyModel.MyVoucher.Master.TitleCOA = MyModel.PayCOA
                .Where(e => e.ID == MyModel.MyVoucher.Master.COA)
                .Select(e => e.Title)
                .First() ?? "";
        }

        
        

    }
}

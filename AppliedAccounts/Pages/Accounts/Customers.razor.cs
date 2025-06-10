using AppliedAccounts.Models;
using AppliedAccounts.Services;

namespace AppliedAccounts.Pages.Accounts
{
    public partial class Customers
    {
        public CustomersModel MyModel { get; set; } = new();

        public Customers() { }

        public void Save()
        {
            if (MyModel.Save())
            {
                ToastService.ShowToast(ToastClass.SaveToast, "Customer saved successfully.");
            }
            else
            {
                ToastService.ShowToast(ToastClass.ErrorToast, "Failed to save customer.");
            }
        }
    }
}

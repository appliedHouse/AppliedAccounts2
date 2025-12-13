using AppliedAccounts.Authentication;
using AppliedAccounts.Models;
using AppliedAccounts.Services;

namespace AppliedAccounts.Pages.Accounts
{
    public partial class Customers
    {
        public CustomersModel MyModel { get; set; } = new();

        public Customers() { }

        protected override void OnInitialized()
        {
            var AppUserProfile = ((UserAuthenticationStateProvider)authStateProvider).AppUser;

            if (ID < 0) { IsDelete = true; ID = Math.Abs(ID); }

            if (AppUserProfile != null) { MyModel = new(AppGlobal, ID); }
            else { MyModel = new(); }
        }


        public void Delete(long ID)
        {
            if (MyModel.Delete(ID)) { AppGlobal.NavManager.NavigateTo("/CustomerList"); }
        }

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

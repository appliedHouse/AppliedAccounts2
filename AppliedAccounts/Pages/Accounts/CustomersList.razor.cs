using AppliedAccounts.Models;

namespace AppliedAccounts.Pages.Accounts
{
    public partial class CustomersList
    {
        #region Constructor
        public CustomersList() { }
        #endregion

        public CustomersModel Model { get; set; }
        private CustomerVM? SelectedCustomer { get; set; }              // View Model for Bootstrap model display.
        private bool isModalVisible { get; set; }

        protected async override Task OnInitializedAsync()
        {
            Model = new(AppGlobal);
        }

        public void Back() { AppGlobal.NavManager.NavigateTo("/Menu/Accounts"); }
        public void Add1() { AppGlobal.NavManager.NavigateTo("/Customer/0"); }
        public void Edit(long ID) { AppGlobal.NavManager.NavigateTo($"/Customer/{ID}"); }
        public void Delete(long ID) { AppGlobal.NavManager.NavigateTo($"/Customer/{ID}/true"); }

        public void Submit() { }


        #region Bootstrap Model View & close
        internal void ViewCustomer(long id)
        {
            SelectedCustomer = Model.GetRecord(id);
            isModalVisible = true;
            StateHasChanged();
        }

        internal void CloseModal()
        {
            isModalVisible = false;
            SelectedCustomer = null;
            StateHasChanged();
        }
        #endregion
    }
}

using AppliedAccounts.Models;
using Microsoft.AspNetCore.Components;

namespace AppliedAccounts.Pages.Sale
{
    public partial class Customers
    {
        [Parameter]
        public long ID { get; set; }

        [Parameter]
        public bool IsDelete { get; set; } = false;


        public string GotoBack = Menus.MenuNavigation.NavTo(Menus.MenuID.ClientList);
        public CustomersModel MyModel { get; set; }

        public Customers() { }

        protected override void OnInitialized()
        {
            MyModel = new(AppGlobal);
            MyModel.Record = MyModel.GetRecord(ID);
        }


        public void Delete()
        {
            ID = MyModel.Record.ID;
            if (MyModel.Delete(ID))
            {
                ToastService.ShowSuccess($"Successfully deleted {MyModel.Record.Title}");
                AppGlobal.NavManager.NavigateTo("/CustomerList");
            }
            else
            {
                ToastService.ShowError($"Fail to be deleted {MyModel.Record.Title}");
            }
        }

        public void Save()
        {
            if (MyModel.Save())
            {
                ToastService.ShowSuccess($"Successfully saved {MyModel.Record.Title}");
            }
            else
            {
                ToastService.ShowError($"Fail to be saved {MyModel.Record.Title}");
            }
        }
    }
}

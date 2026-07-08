using AppliedAccounts.Authentication;
using AppliedAccounts.Models;
using AppliedAccounts.Services;
using Microsoft.AspNetCore.Components;
using System.Collections.Immutable;

namespace AppliedAccounts.Pages.Accounts
{
    public partial class Customers
    {
        [Parameter]
        public long ID { get; set; }

        [Parameter]
        public bool IsDelete { get; set; } = false;

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

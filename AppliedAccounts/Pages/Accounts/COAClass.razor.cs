using AppliedAccounts.Models;
using Menus;

namespace AppliedAccounts.Pages.Accounts
{
    public partial class COAClass
    {
        public COAClassModel MyModel { get; set; } = new();
        public COAClass() { }

        public void Save()
        {
            var IsSaved = MyModel.Save();

            if (IsSaved)
            {
                Toaster.ShowSuccess($"Record {MyModel.Record.Title} has been saved Save");

            }
            else
            {
                Toaster.ShowWarning($"Record {MyModel.Record.Title} failed to  Save");
            }
        }

        public void Add()
        {
            MyModel.Add();
        }

        public void Delete(long ID)
        {
            if (MyModel.Delete(ID))
            {
                Toaster.ShowSuccess(MyModel.MyMessage);
            }
            else
            {
                Toaster.ShowWarning(MyModel.MyMessage);
            }
        }

        public void Edit(long ID)
        {
            MyModel.Edit(ID);
        }

        public void Back()
        {
            AppGlobal.NavManager.GoTo(MenuID.AccountsDictionery);
        }
    }
}

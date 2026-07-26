using AppliedAccounts.Models;
using Menus;

namespace AppliedAccounts.Pages.Accounts
{
    public partial class COANature
    {
        public COANatureModel MyModel { get; set; } = new();
        public COANature() { }

        public void Save()
        {
            var IsSaved = MyModel.Save();

            if (IsSaved) 
            {
                Toaster.ShowSuccess(MyModel.MyMessage);
            }
            else
            {
                Toaster.ShowWarning(MyModel.MyMessage);
            }
        }

        public void Add()
        {
            MyModel.Add();
        }

        public void Delete(long ID)
        {
            if(MyModel.Delete(ID))
            {
                Toaster.ShowSuccess(MyModel.MyMessage);
                MyModel.LoadData();
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

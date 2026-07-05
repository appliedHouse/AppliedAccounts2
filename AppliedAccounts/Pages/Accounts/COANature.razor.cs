using AppliedAccounts.Models;

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
                ToastService.ShowSuccess(MyModel.MyMessage);
            }
            else
            {
                ToastService.ShowWarning(MyModel.MyMessage);
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
                ToastService.ShowSuccess(MyModel.MyMessage);
                MyModel.LoadData();
            }
            else
            {
                ToastService.ShowWarning(MyModel.MyMessage);
            }
        }

        public void Edit(long ID)
        {
            MyModel.Edit(ID);
        }
    }
}

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
                ToastService.ShowSuccess($"Record {MyModel.Record.Title} has been saved Save");
            }
            else
            {
                ToastService.ShowWarning($"Record {MyModel.Record.Title} failed to  Save");
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
                ToastService.ShowSuccess($"Record {MyModel.Record.Title} has been deleted successfully");
                

            }
            else
            {
                ToastService.ShowWarning($"Record {MyModel.Record.Title} failed to Delete");
            }
        }

        public void Edit(long ID)
        {
            MyModel.Edit(ID);
        }
    }
}

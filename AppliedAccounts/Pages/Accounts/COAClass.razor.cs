using AppliedAccounts.Models;

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
            if (MyModel.Delete(ID))
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

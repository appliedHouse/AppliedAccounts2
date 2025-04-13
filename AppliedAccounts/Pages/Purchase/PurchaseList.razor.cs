using AppliedDB;

namespace AppliedAccounts.Pages.Purchase
{
    public partial class PurchaseList
    {
        public AppUserModel AppUser { get; set; }
        public Models.PurchaseListModel MyModel { get; set; }

        public void Search()
        {
            MyModel.LoadData();
        }

        public void PrintAll()
        { }

    }
}

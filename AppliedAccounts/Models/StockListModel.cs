using AppliedAccounts.Services;

namespace AppliedAccounts.Models
{
    public class StockListModel
    {
        public GlobalService AppGlobal { get; set; }
        public StockListModel(GlobalService _AppGlobal) 
        {
            AppGlobal = _AppGlobal;
        }

    public void GetKeys()
        {

        }
    }
}

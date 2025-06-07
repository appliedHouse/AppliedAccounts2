using AppliedAccounts.Data;
using AppReports;

namespace AppliedAccounts.Pages.Sale.Quotations
{
    public class QuotationListModel
    {
        public int QuotID { get; set; } = 0;

        public Filters FilterClass { get; set; } = new Filters();


    }
    public class Filters
    {
        public Filters()
        {
            From = DateTime.Now.AddDays(-30);
            To = DateTime.Now;
        }

        public Filters(string _RegistryKey, string _DBFile)
        {
            From = AppRegistry.GetFrom(_DBFile, _RegistryKey);
            From = AppRegistry.GetTo(_DBFile, _RegistryKey);
            CustomerID = AppRegistry.GetNumber(_DBFile, _RegistryKey);
            SearchText = AppRegistry.GetText(_DBFile, _RegistryKey);
        }

        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public int CustomerID { get; set; }
        public int Status { get; set; }
        public int ItemID { get; set; }
        public string SearchText { get; set; }
        public ReportType ReportTypeID { get; set; }
    }

}

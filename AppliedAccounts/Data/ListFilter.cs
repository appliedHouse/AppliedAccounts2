using System.Data;

namespace AppliedAccounts.Data
{
    public class ListFilter
    {
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public DateTime MinDate { get; set; }
        public DateTime MaxDate { get; set; }
        public string SearchText { get; set; } 
        public string GetFilterText { get; set; }
        public ListFilter(string DBFile) {

            DateFrom = DateTime.Now;
            DateTo = DateTime.Now;
            MinDate = AppRegistry.GetDate(DBFile, "Vou_DtFrom");
            MaxDate = AppRegistry.GetDate(DBFile, "Vou_DtTo");
            SearchText = string.Empty;
            GetFilterText = string.Empty;
        }

    }
}

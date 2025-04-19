using System.Data;
using System.Text;

namespace AppliedAccounts.Data
{
    public class ListFilter
    {
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public DateTime MinDate { get; set; }
        public DateTime MaxDate { get; set; }
        public string SearchText { get; set; } 
        
        public ListFilter(string DBFile) {

            DateFrom = DateTime.Now;
            DateTo = DateTime.Now;
            MinDate = AppRegistry.GetDate(DBFile, "Vou_DtFrom");
            MaxDate = AppRegistry.GetDate(DBFile, "Vou_DtTo");
            SearchText = string.Empty;
            
        }

        public void ClearSearch()
        {
            SearchText = string.Empty;
        }

        public string GetFilterText()
        {
            if(SearchText.Length == 0) { return string.Empty; }

            var _Text = new StringBuilder();

            _Text.Append(" WHERE ");
            _Text.Append("");

            return _Text.ToString();
        }



    }
}

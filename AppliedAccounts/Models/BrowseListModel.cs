using AppliedDB;
using Microsoft.AspNetCore.Components;
using System.ComponentModel;

namespace AppliedAccounts.Models
{
    public class BrowseListModel 
    {
        public BrowseListModel()
        {
            BrowseList = [];
            FilteredList = [];

            BrowseList.Add(new CodeTitle() { ID = 0, Code = "-", Title = "No Record Found" });
            //OutputValue = 0;
            Enable = false;
            SearchText = string.Empty;
        }

        

        public List<CodeTitle> BrowseList { get; set; }
        public List<CodeTitle> FilteredList { get; set; }
        public string BrowseListName { get; set; }
        public int InputValue { get; set; }
        public int OutputValue { get; set; }
        public bool Enable { get; set; } = false;
        public string SearchText { get; set; }


        public void SetBrowseList(List<CodeTitle> _browseList)
        {
            BrowseList = _browseList;
            FilteredList = _browseList;
        }

        public List<CodeTitle> FilterList(string _searchText)
        {
            SearchText = _searchText;
            var _FilteredList = FilteredList ?? [];

            if (string.IsNullOrEmpty(SearchText))
            {
                _FilteredList = BrowseList;
            }
            else
            {
                _FilteredList = [.. BrowseList.Where(x => x.Code.Contains(SearchText, StringComparison.CurrentCultureIgnoreCase) ||
                                                         x.Title.Contains(SearchText, StringComparison.CurrentCultureIgnoreCase))];
            }
            return _FilteredList;
        }

        public void ChangeFilter(ChangeEventArgs e)
        {
            SearchText = e.Value?.ToString() ?? "";
            FilteredList = FilterList(SearchText);
        }

        public void SelectValue(int _ID)
        {
            OutputValue = _ID;
        }
              

    }
}

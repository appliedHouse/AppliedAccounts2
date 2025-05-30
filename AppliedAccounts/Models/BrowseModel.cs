using AppliedDB;
using Microsoft.AspNetCore.Components;

namespace AppliedAccounts.Models
{
    public class BrowseModel
    {
        public int Type { get; set; } = 0; // 0 - Null, 1 - Class, 2 - Nature, 3 - Notes
        public string Heading { get; set; }
        public List<CodeTitle> BrowseList { get; set; }
        public List<CodeTitle> FilterList { get; set; }
        public int Selected { get; set; }
        public string SearchText { get; set; } = string.Empty;

        public BrowseModel()
        {
            Heading = string.Empty;
            BrowseList = [];
            Selected = 0;
        }

        public List<CodeTitle> GetBrowseList()
        {
            return FilterList ?? BrowseList;

        }

        public void InputHandler(ChangeEventArgs e)
        {
            var Oic = StringComparison.OrdinalIgnoreCase;
            var _Value = e.Value?.ToString() ?? string.Empty;
            var _SearchText = SearchText;
            
            if (_Value.Length > 0)
            {
                FilterList = BrowseList.Where
                                (x => 
                                x.Code.Contains(_Value, Oic) ||
                                x.Title.Contains(_Value, Oic) 
                                )
                                .ToList();
            }
            else
            {
                FilterList = BrowseList;
            }
        }

    }
}

using AppliedDB;

namespace AppliedAccounts.Models
{
    public class BrowseModel
    {
        public int Type { get; set; } = 0; // 0 - Null, 1 - Class, 2 - Nature, 3 - Notes
        public string Heading { get; set; }
        public List<CodeTitle> BrowseList { get; set; }
        public int Selected { get; set; }
        public string SearchText { get; set; } = string.Empty;

        public BrowseModel()
        {
            Heading = string.Empty;
            BrowseList = [];
            Selected = 0;
        }

        public List<CodeTitle> GetBrowseList(int _SelectedID)
        {
            

        }

    }
}

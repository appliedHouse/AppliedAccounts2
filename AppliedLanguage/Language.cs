using System.Data;

namespace AppLanguages
{
    public class Language
    {
        //private DataTable? LanguageDataTable { get; set; }
        public readonly List<DataRow> LangList = new LanguageList().LangList ;
        public List<DataRow> LanguageText { get; set; } = new();

        public int LanguageID { get; set; }
        public string Section { get; set; } = string.Empty;

        public Language(int _LanguageID, string _Section)
        {
            LanguageText = GetLanguageText(_LanguageID, _Section);
        }

        private static List<DataRow> GetLanguageText(int _LanguageID, string _Section)
        {
            List<DataRow> _List = new();
            DataTable _Table = new AppliedDB.Languages().GetLanguageText(_LanguageID, _Section);

            if (_Table.Rows.Count > 0)
            {
                foreach (DataRow _Row in _Table.Rows)
                {
                    _List.Add(_Row);
                }
                return _List;
            }
            return null;
        }

        public string GetValue(string _Key)
        {
            
            return LanguageText.Where(Row => (string)Row["Key"] == _Key.Trim()).Select(Row => (string)Row["TextValue"]).FirstOrDefault() ?? "No Value.";


            //if (LanguageDataTable is not null)
            //{
            //    LanguageDataTable.DefaultView.RowFilter = $"Key='{_Key}'";
            //    if (LanguageDataTable.DefaultView.Count >= 1)
            //    {
            //        return LanguageDataTable.DefaultView[0].Row["TextValue"].ToString() ?? "No Text.";
            //    }
            //}
            //return "No Value.";
        }

    }





}


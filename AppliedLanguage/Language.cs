using AppliedDB;
using Microsoft.Data.Sqlite;
using System.Data;

namespace AppLanguages
{
    public class Language
    {
        //private DataTable? LanguageDataTable { get; set; }
        public List<DataRow> Languages { get; set; }
        public int LanguagesCount => Languages.Count;
        public List<DataRow> LanguageText { get; set; } = new();

        public int LanguageID { get; set; }
        public string Section { get; set; } = string.Empty;
        public DataSource Source { get; set; }

        public Language(int _LanguageID, string _Section, DataSource dataSource)
        {
            Source = dataSource;
            LanguageText = GetLanguageText(_LanguageID, _Section);
            Languages = GetLanguageList();
        }

        private List<DataRow> GetLanguageText(int _LanguageID, string _Section)
        {
            List<DataRow> _List = new();
            SqliteConnection? _Connection = Source.MyConnections.GetSqliteLanguage();
            if(_Connection == null) { return null!; }

            string _Query = $"SELECT * FROM LanguageText WHERE LanguageID={_LanguageID} AND Section='{_Section}'";
            DataTable? _Table = Source.GetTable(_Query, _Connection);

            if (_Table != null && _Table.Rows.Count > 0)
            {
                foreach (DataRow _Row in _Table.Rows)
                {
                    _List.Add(_Row);
                }
                return _List;
            }
            return null!;
        }

        private List<DataRow> GetLanguageList()
        {
            List<DataRow> _List = new();
            SqliteConnection? _Connection = Source.MyConnections.GetSqliteLanguage();
            if (_Connection == null) { return null!; }

            string _Query = $"SELECT * FROM [LanguageList]";
            DataTable? _Table = Source.GetTable(_Query, _Connection);

            if (_Table != null && _Table.Rows.Count > 0)
            {
                foreach (DataRow _Row in _Table.Rows)
                {
                    _List.Add(_Row);
                }
                return _List;
            }
            return null!;
        }


        public string GetValue(string _Key)
        {
            return LanguageText.Where(Row => (string)Row["Key"] == _Key.Trim()).Select(Row => (string)Row["TextValue"]).FirstOrDefault() ?? "No Value.";
        }
    }
}


using System.Data.SQLite;
using System.Data;

namespace AppliedLanguage
{
    public static class LanguageListClass
    {
        public static Dictionary<int, string> GetLanguageList()
        {
            var _Path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Languages", "Languages.db");
            var _Connection = new SQLiteConnection($"Data Source='{_Path}'"); _Connection.Open();

            var _Command = new SQLiteCommand(_Connection); ;
            _Command.CommandText = "SELECT * FROM [LanguageList]";
            SQLiteDataAdapter _Adapter = new(_Command);
            DataSet _DataSet = new();
            DataTable _DataTable = new DataTable();

            _Adapter.Fill(_DataSet);

            var _List = new Dictionary<int, string>();
            if (_DataSet.Tables.Count > 0)
            {
                _DataTable = _DataSet.Tables[0];

                foreach (DataRow _Row in _DataTable.Rows)
                {
                    _List.Add ((int)_Row["ID"], (string)_Row["Title"]);
                }
            }

            _Connection.Dispose(); _Connection = null;
            _Command.Dispose(); _Command = null;

            return _List;
        }
    }

}


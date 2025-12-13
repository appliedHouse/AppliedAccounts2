using System.Data;
using Microsoft.Data.Sqlite;

namespace AppliedLanguage
{
    public static class LanguageListClass
    {
        public static Dictionary<int, string> GetLanguageList()
        {
            var _Path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Languages", "Languages.db");
            var _ConnectionText = $"Data Source='{_Path}'";
            var _CommandText = "SELECT * FROM [LanguageList]";

            using var _Connection = new SqliteConnection(_ConnectionText); _Connection.Open();

            using var _Command = new SqliteCommand(_CommandText, _Connection); ;
            using var _reader = _Command.ExecuteReader();
            var _DataTable = new DataTable();
            _DataTable.Load(_reader);

            var _List = new Dictionary<int, string>();

            foreach (DataRow _Row in _DataTable.Rows)
            {
                _List.Add((int)_Row["ID"], (string)_Row["Title"]);
            }

            return _List;
        }
    }

}


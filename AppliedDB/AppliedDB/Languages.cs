using System.Data;
using Microsoft.Data.Sqlite;

namespace AppliedDB
{
    public class Languages
    {
        public SqliteConnection MyConnection { get; set; }

        
        public Languages(SqliteConnection connection)
        {
            MyConnection = connection;
        }


        public DataTable GetLanguageList()
        {
            var _Connection = MyConnection;
            if (_Connection is not null)
            {
                // SELECT * FROM [LanguageList]
                _Connection.Open();
                using var _Command = new SqliteCommand($"SELECT * FROM [LanguageList]", _Connection);
                using var _reader = _Command.ExecuteReader();
                var dt = new DataTable();
                dt.Load(_reader);
                dt.TableName = "LanguageList";
                return dt;
                
            }
            return null!;

        }
        public DataTable? GetLanguageText(int _Language, string _Section)
        {
            
            if (MyConnection is not null)
            {
                // SELECT * FROM [Language records of specific language ]
                var _Filter = $"WHERE Language={_Language} AND Section IN ('{_Section}','Common')";
                using var _Command = new SqliteCommand($"SELECT * FROM [LanguageText] {_Filter}", MyConnection);
                using var _reader = _Command.ExecuteReader();
                var dt = new DataTable();
                dt.Load(_reader);
                dt.TableName = "LanguageText";
                return dt;
                
            }
            return null!;

        }

    }
}

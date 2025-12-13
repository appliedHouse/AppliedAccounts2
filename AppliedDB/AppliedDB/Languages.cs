using System.Data;
using Microsoft.Data.Sqlite;
using Windows.System.UserProfile;

namespace AppliedDB
{
    public class Languages
    {
        //public SqliteConnection LangConnection = Connections.GetLanguageConnection() ?? new();




        public static DataTable GetLanguageList()
        {
            var _Connection = Connections.GetLanguageConnection();
            if (_Connection is not null)
            {
                // SELECT * FROM [LanguageList]
                _Connection.Open();
                using var _Command = new SqliteCommand($"SELECT * FROM [LanguageList]", _Connection);
                using var _reader = _Command.ExecuteReader();
                var dt = new DataTable();
                dt.Load(_reader);
                return dt;
                //var _Adapter = new SqliteDataAdapter(_Command);
                //var _DataSet = new DataSet();
                //_Adapter.Fill(_DataSet, "LanguageList");
                //_Connection.Close();
                //if (_DataSet.Tables.Count > 0)
                //{
                //    return _DataSet.Tables[0];
                //}
            }
            return null;

        }

        public DataTable GetLanguageText(int _Language, string _Section)
        {
            var _Connection = Connections.GetLanguageConnection();
            if (_Connection is not null)
            {
                // SELECT * FROM [Language records of specific language ]
                _Connection.Open();
                var _Filter = $"WHERE Language={_Language} AND Section IN ('{_Section}','Common')";
                using var _Command = new SqliteCommand($"SELECT * FROM [LanguageText] {_Filter}", _Connection);
                using var _reader = _Command.ExecuteReader();
                var dt = new DataTable();
                dt.Load(_reader);
                return dt;
                //var _Adapter = new SqliteDataAdapter(_Command);
                //var _DataSet = new DataSet();
                //_Adapter.Fill(_DataSet, "LanguageList");
                //_Connection.Close();
                //if (_DataSet.Tables.Count > 0)
                //{
                //    return _DataSet.Tables[0];
                //}
            }
            return null;

        }

    }
}

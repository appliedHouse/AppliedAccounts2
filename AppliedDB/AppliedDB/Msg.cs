using System.Data;
using Microsoft.Data.Sqlite;

namespace AppliedDB
{
    public class Msg
    {
        public static DataTable GetMessages()
        {
            try
            {
                var _Connection = Connections.GetMessagesConnection();
                if (_Connection is not null)
                {
                    _Connection.Open();
                    using var _Command = new SqliteCommand($"SELECT * FROM [Messages]", _Connection);
                    using var _reader = _Command.ExecuteReader();

                    var _DataSet = new DataSet();
                    var _DataTable = new DataTable();

                    _DataTable.TableName = "Messages";
                    _DataTable.Load(_reader);
                    return _DataTable;
                }
            }
            catch (Exception)
            {
                return null;
            }

            return null;
        }

    }
}

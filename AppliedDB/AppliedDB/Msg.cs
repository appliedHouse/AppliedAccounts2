using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppliedDB
{
    public class Msg
    {
        public static DataTable GetMessages()
        {
            var _Connection = Connections.GetMessagesConnection();
            if (_Connection is not null)
            {
                // SELECT * FROM [Messages]
                _Connection.Open();
                var _Command = new System.Data.SQLite.SQLiteCommand($"SELECT * FROM [Messages]", _Connection);
                var _Adapter = new System.Data.SQLite.SQLiteDataAdapter(_Command);
                var _DataSet = new DataSet();
                _Adapter.Fill(_DataSet, "Messages");
                _Connection.Close();
                if (_DataSet.Tables.Count > 0)
                {
                    return _DataSet.Tables[0];
                }
            }
            return null;
        }

    }
}

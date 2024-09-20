using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppliedDB
{
    public class TempDB
    {
        private SQLiteConnection MyConnection { get; set; }
        public string TempDBFile { get; set; }
        public string TableName { get; set; }
        public DataTable TempTable { get; set; }

        public TempDB(string _TempDBFile) 
        {
            TempDBFile = _TempDBFile;
            string _DBPath = Path.Combine(Connections.GetTempDBPath(), TempDBFile);
            MyConnection = new(new SQLiteConnection($"Data Source={_DBPath}"));
        }

        public DataTable GetTempTable(string _DataName)
        {
            TableName = _DataName;
            if (MyConnection.State != ConnectionState.Open) { MyConnection.Open(); } 

            SQLiteCommand _Command = new(MyConnection);
            _Command.CommandText = $"SELECT * FROM [{TableName}]";
            DataSet _DataSet = new DataSet();
            SQLiteDataAdapter _Adapter = new(_Command);
            _Adapter.Fill(_DataSet, "SaleData");
            if (_DataSet.Tables.Count > 0)
            {
                TempTable = _DataSet.Tables[0];
                return _DataSet.Tables[0];
            }
            return null;
        }
    }
}

﻿using System.Data;
using System.Data.SQLite;

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

        public async Task<DataTable> GetTempTableAsync(string _DataTableName)
        {
            TempTable = new DataTable();
            await Task.Run(() =>
            {
                try
                {
                    TableName = _DataTableName;
                    if (MyConnection.State != ConnectionState.Open) { MyConnection.Open(); }

                    SQLiteCommand _Command = new(MyConnection);
                    _Command.CommandText = $"SELECT * FROM [{TableName}]";
                    DataSet _DataSet = new DataSet();
                    SQLiteDataAdapter _Adapter = new(_Command);
                    _Adapter.Fill(_DataSet, _DataTableName);
                    if (_DataSet.Tables.Count > 0)
                    {
                        TempTable = _DataSet.Tables[0];
                    }
                }
                catch (Exception)
                {
                    TempTable = new();
                }

            });
            return TempTable;
        }

    }
}

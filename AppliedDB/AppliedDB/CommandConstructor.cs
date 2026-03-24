using Microsoft.Data.Sqlite;
using System.Data;
using System.Text;

namespace AppliedDB
{
    public static class CommandConstructor
    {
        public static class Commands
        {
            public static SqliteCommand? Insert(DataRow CurrentRow, SqliteConnection DBConnection)
            {
                if (CurrentRow.Field<long>("ID") == 0)
                {
                    DataColumnCollection _Columns = CurrentRow.Table.Columns;

                    StringBuilder _CommandString = new();
                    string _LastColumn = _Columns[_Columns.Count - 1].ColumnName.ToString();
                    string _TableName = CurrentRow.Table.TableName;
                    string _ParameterName;
                    if (_TableName == "") { return null; }
                    _CommandString.Append("INSERT INTO [");
                    _CommandString.Append(_TableName);
                    _CommandString.Append("] VALUES (");

                    foreach (DataColumn _Column in _Columns)
                    {
                        string _ColumnName = _Column.ColumnName.ToString();
                        _CommandString.Append(string.Concat('@', _Column.ColumnName));
                        if (_ColumnName != _LastColumn)
                        { _CommandString.Append(','); }
                        else
                        { _CommandString.Append(") "); }
                    }

                    SqliteCommand _Command = new SqliteCommand(_CommandString.ToString(), DBConnection);

                    foreach (DataColumn _Column in _Columns)
                    {
                        if (_Column == null) { continue; }
                        _ParameterName = string.Concat('@', _Column.ColumnName.Replace(" ", ""));
                        _Command.Parameters.AddWithValue(_ParameterName, CurrentRow[_Column.ColumnName]);
                    }

                    //if (CurrentRow.Field<long>("ID") == 0)
                    //{
                    //    CurrentRow["ID"] = DataSource.GetMaxID(_TableName, DBConnection.ConnectionString);
                    //    _Command.Parameters["@ID"].Value = CurrentRow["ID"];
                    //}

                    return _Command;
                }
                return null;
            }
            public static SqliteCommand? Insert(DataRow CurrentRow, string DBFile)
            {
                var _Connection = Connections.GetClientConnection(DBFile);
                if (_Connection is not null)
                {
                    return Insert(CurrentRow, _Connection);
                }
                return null;
            }
            public static SqliteCommand? UpDate(DataRow CurrentRow, SqliteConnection DBConnection)
            {
                //if (CurrentRow.RowState == DataRowState.Modified)
                if (CurrentRow.Field<long>("ID") != 0)
                {
                    var _TableName = CurrentRow.Table.TableName;
                    var _Columns = CurrentRow.Table.Columns;
                    var _CommandString = new StringBuilder();
                    var _LastColumn = _Columns[_Columns.Count - 1].ColumnName.ToString();

                    _CommandString.Append(string.Concat("UPDATE [", _TableName, "] SET "));

                    foreach (DataColumn _Column in _Columns)
                    {
                        if (_Column.ColumnName == "ID") { continue; }
                        _CommandString.Append(string.Concat("[", _Column.ColumnName, "]"));
                        _CommandString.Append('=');
                        _CommandString.Append(string.Concat('@', _Column.ColumnName.Replace(" ", "")));

                        if (_Column.ColumnName == _LastColumn)
                        {
                            _CommandString.Append(string.Concat(" WHERE ID = @ID"));
                        }
                        else
                        {
                            _CommandString.Append(',');
                        }
                    }

                    SqliteCommand _Command = new SqliteCommand(_CommandString.ToString(), DBConnection);

                    foreach (DataColumn _Column in _Columns)
                    {
                        if (_Column == null) { continue; }
                        var _ParameterName = string.Concat('@', _Column.ColumnName.Replace(" ", ""));
                        _Command.Parameters.AddWithValue(_ParameterName, CurrentRow[_Column.ColumnName]);
                    }

                    return _Command;
                }
                return null;

            }
            public static SqliteCommand? UpDate(DataRow CurrentRow, string DBFile)
            {
                var _Connection = Connections.GetClientConnection(DBFile);
                if (_Connection is not null)
                {
                    return UpDate(CurrentRow, _Connection);
                }
                return null;
            }
            public static SqliteCommand? Delete(DataRow CurrentRow, SqliteConnection DBConnection)
            {
                if (CurrentRow.Field<long>("ID") != 0)
                {
                    var _TableName = CurrentRow.Table.TableName;
                    if (_TableName == "") { return null; }
                    var _Command = new SqliteCommand("", DBConnection);
                    _Command.Parameters.AddWithValue("@ID", CurrentRow["ID"]);
                    _Command.CommandText = $"DELETE FROM [{_TableName}] WHERE ID=@ID";
                    return _Command;
                }
                return null;

            }
            public static SqliteCommand? Delete(DataRow CurrentRow, string DBFile)
            {
                if (CurrentRow.Field<long>("ID") != 0)
                {
                    var _TableName = CurrentRow.Table.TableName;
                    var _Connection = Connections.GetClientConnection(DBFile);
                    using var _Command = new SqliteCommand("", _Connection);

                    if (_TableName == "")
                    {
                        return null;
                    }

                    _Command.Parameters.AddWithValue("@ID", CurrentRow["ID"]);
                    _Command.CommandText = $"DELETE FROM [{_TableName}] WHERE ID=@ID";
                    return _Command;
                }
                return null;
            }

        }
    }
}

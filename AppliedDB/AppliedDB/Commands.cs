using AppMessages;
using Microsoft.Data.Sqlite;
using System.Data;
using System.Text;
using Messages = AppMessages.Enums.Messages;

namespace AppliedDB
{
    public class Commands
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

                if(_TableName == "")
                {
                    return null;
                }

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

                CurrentRow["ID"] = DataSource.GetMaxID(_TableName, DBConnection);
                _Command.Parameters["@ID"].Value = CurrentRow["ID"];
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
            if (CurrentRow.Field<long>("ID") != 0)
            {

                var _TableName = CurrentRow.Table.TableName;
                var _Columns = CurrentRow.Table.Columns;
                //var _Command = new SqliteCommand(DBConnection);
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
                if(_TableName == "")
                {
                    return null;
                }
                var _Command = new SqliteCommand("",DBConnection);
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

                if(_TableName == "")
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
    public class CommandClass
    {
        public SqliteCommand CommandInsert { get; set; }
        public SqliteCommand CommandUpdate { get; set; }
        public SqliteCommand CommandDelete { get; set; }
        public DataRow Row { get; set; }
        public string Action { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public int Effected { get; set; } = 0;
        public long PrimaryKeyID { get; set; } = 0;
        public MessageClass MyMessages { get; set; } = new();

        #region Constructors
        public CommandClass()
        {

        }

        public CommandClass(DataRow _Row, string DBFile)
        {
            Row = _Row;

            if ((int)Row.Field<long>("ID") == 0) { Action = "Insert"; } else { Action = "Update"; }

            CommandInsert = Commands.Insert(Row, DBFile);
            CommandUpdate = Commands.UpDate(Row, DBFile);
            CommandDelete = Commands.Delete(Row, DBFile);

        }

        public CommandClass(DataRow _Row, SqliteConnection DBConnection)
        {
            Row = _Row;

            if ((int)Row.Field<long>("ID") == 0) { Action = "Insert"; } else { Action = "Update"; }

            CommandInsert = Commands.Insert(Row, DBConnection);
            CommandUpdate = Commands.UpDate(Row, DBConnection);
            CommandDelete = Commands.Delete(Row, DBConnection);

        }

        #endregion

        // Insert and Update the Row
        public bool SaveChanges()
        {
            bool result = false;
            if (Row is null) { MyMessages.Add(Messages.RowValueNull); return false; }

            if (Action == "Update")
            {
                if (CommandUpdate is not null)
                {
                    try
                    {
                        CommandUpdate.Connection.Open();
                        Effected = CommandUpdate.ExecuteNonQuery();
                        CommandUpdate.Connection.Close();
                        PrimaryKeyID = (long)CommandUpdate.Parameters["@ID"].Value;
                    }
                    catch (Exception)
                    {
                        MyMessages.Add(Messages.RowNotUpdated); return false;
                    }
                }
            }

            if (Action == "Insert")
            {
                if (CommandInsert is not null)
                {
                    try
                    {
                        CommandInsert.Connection.Open();
                        Effected = CommandInsert.ExecuteNonQuery();
                        CommandInsert.Connection.Close();
                        PrimaryKeyID = (long)CommandInsert.Parameters["@ID"].Value;
                    }
                    catch (Exception)
                    {
                        MyMessages.Danger(Messages.RowNotInserted); result = false;
                    }

                }
            }

            if (Effected == 0) { MyMessages.Alert(Messages.NotSave); result = false; }
            if (Effected > 0) { MyMessages.Add(Messages.Save); result = true; }

            return result;
        }

        // Delete the row
        public bool DeleteRow()
        {
            if (Row is not null)
            {
                Row.Table.DefaultView.RowFilter = $"ID={Row.Field<long>("ID")}";
                if (Row.Table.DefaultView.Count == 1)
                {
                    if (CommandDelete is not null)
                    {
                        CommandDelete.Connection.Open();
                        Effected = CommandDelete.ExecuteNonQuery();
                        CommandDelete.Connection.Close();
                        if (Effected > 0)
                        {
                            MyMessages.Add(Messages.RowDeleted);
                            return true;
                        }
                    }
                }
            }
            MyMessages.Add(Messages.RowNotDeleted);
            return false;
        }
    }
}

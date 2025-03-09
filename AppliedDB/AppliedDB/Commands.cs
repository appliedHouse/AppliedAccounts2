using System.Data.SQLite;
using System.Data;
using System.Text;
using Messages = AppMessages.Enums.Messages;
using AppMessages;

namespace AppliedDB
{
    public class Commands
    {
        public static SQLiteCommand? Insert(DataRow CurrentRow, SQLiteConnection DBConnection)
        {
            if ((int)CurrentRow["ID"] == 0)
            {

                DataColumnCollection _Columns = CurrentRow.Table.Columns;
                SQLiteCommand _Command = new(DBConnection);

                StringBuilder _CommandString = new();
                string _LastColumn = _Columns[_Columns.Count - 1].ColumnName.ToString();
                string _TableName = CurrentRow.Table.TableName;
                string _ParameterName;

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

                _Command.CommandText = _CommandString.ToString();

                foreach (DataColumn _Column in _Columns)
                {
                    if (_Column == null) { continue; }
                    _ParameterName = string.Concat('@', _Column.ColumnName.Replace(" ", ""));
                    _Command.Parameters.AddWithValue(_ParameterName, CurrentRow[_Column.ColumnName]);
                }

                CurrentRow["ID"] = DataSource.GetMaxID(_TableName,DBConnection);
                _Command.Parameters["@ID"].Value = CurrentRow["ID"];
                return _Command;
            }
            return null;
        }
        public static SQLiteCommand? Insert(DataRow CurrentRow, string DBFile)
        {
            var _Connection = Connections.GetClientConnection(DBFile);
            if(_Connection is not null)
            { 
            return Insert(CurrentRow, _Connection);
            }
            return null;
        }
        public static SQLiteCommand? UpDate(DataRow CurrentRow, SQLiteConnection DBConnection)
        {
            if ((int)CurrentRow["ID"] != 0)
            {

                var _TableName = CurrentRow.Table.TableName;
                var _Columns = CurrentRow.Table.Columns;
                var _Command = new SQLiteCommand(DBConnection);
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

                _Command.CommandText = _CommandString.ToString();

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
        public static SQLiteCommand? UpDate(DataRow CurrentRow, string DBFile)
        {
            var _Connection = Connections.GetClientConnection(DBFile);
            if (_Connection is not null)
            {
                return UpDate(CurrentRow, _Connection);
            }
            return null;
        }
        public static SQLiteCommand? Delete (DataRow CurrentRow, SQLiteConnection DBConnection)
        {
            if ((int)CurrentRow["ID"] != 0)
            {
                var _TableName = CurrentRow.Table.TableName;
                var _Command = new SQLiteCommand(DBConnection);
                _Command.Parameters.AddWithValue("@ID", CurrentRow["ID"]);
                _Command.CommandText = $"DELETE FROM [{_TableName}] WHERE ID=@ID";
                return _Command;
            }
            return null;

        }
        public static SQLiteCommand? Delete(DataRow CurrentRow, string DBFile)
        {
            if ((int)CurrentRow["ID"] != 0)
            {
                var _TableName = CurrentRow.Table.TableName;
                var _Command = new SQLiteCommand(Connections.GetClientConnection(DBFile));
                _Command.Parameters.AddWithValue("@ID", CurrentRow["ID"]);
                _Command.CommandText = $"DELETE FROM [{_TableName}] WHERE ID=@ID";
                return _Command;
            }
            return null;
        }

    }
    public class CommandClass
    {
        public SQLiteCommand? CommandInsert { get; set; }
        public SQLiteCommand? CommandUpdate { get; set; }
        public SQLiteCommand? CommandDelete { get; set; }
        public DataRow? Row { get; set; }
        public string Action { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public int Effected { get; set; } = 0;
        public MessageClass MyMessages { get; set; } = new();

        #region Constructors
        public CommandClass()
        {

        }

        public CommandClass(DataRow _Row, string DBFile)
        {
            Row = _Row;

            if ((int)Row["ID"] == 0) { Action = "Insert"; } else { Action = "Update"; }

            CommandInsert = Commands.Insert(Row, DBFile);
            CommandUpdate = Commands.UpDate(Row, DBFile);
            CommandDelete = Commands.Delete(Row, DBFile);

        }

        public CommandClass(DataRow _Row, SQLiteConnection DBConnection)
        {
            Row = _Row;

            if ((int)Row["ID"] == 0) { Action = "Insert"; } else { Action = "Update"; }

            CommandInsert = Commands.Insert(Row, DBConnection);
            CommandUpdate = Commands.UpDate(Row, DBConnection);
            CommandDelete = Commands.Delete(Row, DBConnection);

        }


        //public CommandClass(DataRow _Row, string DBFile, MessageClass _Messages)
        //{
        //    Row = _Row;
        //    MyMessages = _Messages;

        //    if ((int)Row["ID"] == 0) { Action = "Insert"; } else { Action = "Update"; }

        //    try
        //    {
        //        CommandInsert = Commands.Insert(Row, DBFile);
        //        CommandUpdate = Commands.UpDate(Row, DBFile);
        //        CommandDelete = Commands.Delete(Row, DBFile);
        //        //MyMessages = _Messages;

        //    }
        //    catch (Exception)
        //    {
        //        MyMessages.Add(Messages.CommendError);
        //    }



        //}
        #endregion

        // Insert and Update the Row
        public bool SaveChanges()
        {
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
                    }
                    catch (Exception)
                    {
                        MyMessages.Add(Messages.RowNotDeleted); return false;
                    }

                }
            }

            if (Effected == 0) { MyMessages.Add(Messages.NotSave); return false; }
            if (Effected > 0) { MyMessages.Add(Messages.Save); return true; }

            return false;
        }

        // Delete the row
        public bool DeleteRow()
        {
            if (Row is not null)
            {
                Row.Table.DefaultView.RowFilter = $"ID={(int)Row["ID"]}";
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
            return false;
        }
    }
}

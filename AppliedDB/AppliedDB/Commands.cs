using AppMessages;
using Microsoft.Data.Sqlite;
using System.Data;
using static AppliedDB.CommandConstructor;
using Messages = AppMessages.Enums.Messages;

namespace AppliedDB
{

    public class CommandClass
    {
        public SqliteCommand CommandInsert { get; set; } = new();
        public SqliteCommand CommandUpdate { get; set; } = new();
        public SqliteCommand CommandDelete { get; set; } = new();
        public DataRow Row { get; set; }
        public string Action { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public int Effected { get; set; } = 0;
        public long PrimaryKeyID { get; set; } = 0;
        public MessageClass MyMessages { get; set; } = new();
        public AppliedGlobals.AppValues.AppPath AppPath { get; set; }

        #region Constructors
        public CommandClass()
        {

        }

        public CommandClass(DataRow _Row, string DBFile)
        {
            Row = _Row;

            if (_Row.Field<long>("ID") == 0) { Action = "Insert"; }
            else { Action = "Update"; }

            CommandInsert = Commands.Insert(Row, DBFile)!;
            CommandUpdate = Commands.UpDate(Row, DBFile)!;
            CommandDelete = Commands.Delete(Row, DBFile)!;
        }

        public CommandClass(DataRow _Row, SqliteConnection DBConnection)
        {
            Row = _Row;

            if (_Row.Field<long>("ID") == 0) { Action = "Insert"; }
            else { Action = "Update"; }

            CommandInsert = Commands.Insert(Row, DBConnection)!;
            CommandUpdate = Commands.UpDate(Row, DBConnection)!;
            CommandDelete = Commands.Delete(Row, DBConnection)!;
        }

        public CommandClass(DataRow _Row, SqliteConnection DBConnection, SqliteTransaction Transaction)
        {
            Row = _Row;

            if (_Row.Field<long>("ID") == 0) { Action = "Insert"; }
            else { Action = "Update"; }

            CommandInsert = Commands.Insert(Row, DBConnection)!;
            CommandUpdate = Commands.UpDate(Row, DBConnection)!;
            CommandDelete = Commands.Delete(Row, DBConnection)!;

            if (CommandInsert != null) { CommandInsert.Transaction = Transaction; }
            if (CommandUpdate != null) { CommandUpdate.Transaction = Transaction; }
            if (CommandDelete != null) { CommandDelete.Transaction = Transaction; }
        }

        #endregion

        // Insert and Update the Row
        public bool SaveChanges()
        {
            if (Row is null)
            {
                MyMessages.Add(Messages.RowValueNull);
                return false;
            }

            // UPDATE
            if (Action == "Update" && CommandUpdate is not null)
            {
                try
                {
                    if (CommandUpdate.Transaction == null)
                    {
                        if (CommandUpdate.Connection!.State != ConnectionState.Open)
                            CommandUpdate.Connection.Open();
                    }

                    Effected = CommandUpdate.ExecuteNonQuery();
                    PrimaryKeyID = (long)CommandUpdate.Parameters["@ID"].Value!;

                    if (Effected > 0)
                    {
                        MyMessages.Add(Messages.Save);
                        return true;
                    }

                    MyMessages.Alert(Messages.NotSave);
                    return false;
                }
                catch (Exception ex)
                {
                    MyMessages.Critical(ex.Message);
                    return false;
                }
            }

            // INSERT
            if (Action == "Insert" && CommandInsert is not null)
            {
                try
                {
                    if (CommandInsert.Transaction == null)
                    {
                        if (CommandInsert.Connection!.State != ConnectionState.Open)
                            CommandInsert.Connection.Open();
                    }
                    // Only create savepoint if transaction exists
                    string savePoint = null!;
                    if (CommandInsert.Transaction != null)
                    {
                        savePoint = $"SP_Insert_{Guid.NewGuid():N}";
                        CommandInsert.Transaction.Save(savePoint);
                    }

                    var _Connection = CommandInsert.Connection;
                    var _Transaction = CommandInsert.Transaction;
                    var nextId = GetNextId(_Connection!, _Transaction, Row.Table.TableName);  //  (long)cmdMaxID.ExecuteScalar()!;
                    CommandInsert.Parameters["@ID"].Value = nextId;

                    Effected = CommandInsert.ExecuteNonQuery();
                    PrimaryKeyID = nextId;

                    // Release savepoint if used
                    if (savePoint != null) { CommandInsert.Transaction!.Release(savePoint); }


                    MyMessages.Success(Messages.Save);
                    return true;
                }
                catch (Exception ex)
                {
                    if (CommandInsert.Transaction != null)
                    {
                        try { CommandInsert.Transaction.Rollback(); } catch { }
                    }
                    MyMessages.Danger(ex.Message);
                    return false;
                }
            }

            return false;
        }

        public static long GetNextId(SqliteConnection connection, SqliteTransaction? transaction, string tableName)
        {
            using var cmd = connection.CreateCommand();

            // Assign transaction only if it's not null
            if (transaction != null)
                cmd.Transaction = transaction;

            // 1️⃣ Check if IdGenerator row exists
            cmd.CommandText = "SELECT LastId FROM IdGenerator WHERE TableName = @name";
            cmd.Parameters.AddWithValue("@name", tableName);
            var result = cmd.ExecuteScalar();

            if (result == null)
            {
                // 2️⃣ Row does not exist → get max ID from main table
                cmd.CommandText = $"SELECT IFNULL(MAX(ID),0) FROM [{tableName}]";
                var maxId = Convert.ToInt64(cmd.ExecuteScalar()!);

                // 3️⃣ Insert row with LastId = maxId
                cmd.CommandText = "INSERT INTO IdGenerator (TableName, LastId) VALUES (@name, @lastId)";

                // Clear parameters and add new ones to avoid duplicate parameter error
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@name", tableName);
                cmd.Parameters.AddWithValue("@lastId", maxId);
                cmd.ExecuteNonQuery();

                result = maxId;
            }

            // 4️⃣ Increment LastId atomically
            cmd.CommandText = "UPDATE IdGenerator SET LastId = LastId + 1 WHERE TableName = @name";
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@name", tableName);
            cmd.ExecuteNonQuery();

            // 5️⃣ Return new LastId
            cmd.CommandText = "SELECT LastId FROM IdGenerator WHERE TableName = @name";
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@name", tableName);

            return Convert.ToInt64(cmd.ExecuteScalar()!);
        }



        // Delete the row
        public bool DeleteRow()
        {
            if (Row == null)
            {
                MyMessages.Add(Messages.RowNotDeleted);
                return false;
            }

            if (CommandDelete is null)
            {
                MyMessages.Add(Messages.RowNotDeleted);
                return false;
            }

            try
            {
                long rowId = Row.Field<long>("ID");

                if (CommandDelete.Connection!.State != ConnectionState.Open)
                    CommandDelete.Connection.Open();

                if (CommandDelete.Parameters.Contains("@ID"))
                    CommandDelete.Parameters["@ID"].Value = rowId;

                Effected = CommandDelete.ExecuteNonQuery();

                if (Effected > 0)
                {
                    MyMessages.Add(Messages.RowDeleted);
                    return true;
                }

                MyMessages.Add(Messages.RowNotDeleted);
                return false;
            }
            catch (Exception ex)
            {
                MyMessages.Danger(ex.Message);
                return false;
            }
        }
    }
}

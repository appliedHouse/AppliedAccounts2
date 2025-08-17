using AppMessages;
using System.Data.SQLite;

namespace AppliedAccounts.Data
{
    public class Database_Patches
    {
        public AppliedDB.DataSource Source { get; set; }
        public MessageClass MsgClass { get; set; } = new();
        public MessageClass ErrorMsgClass { get; set; } = new();
        public List<bool> IsPatchApplied { get; set; } = [];
        public Database_Patches(AppliedDB.DataSource source)
        {
            Source = source;
            IsPatchApplied.Add(BillReceivable2_AddUnit());
            IsPatchApplied.Add(BillPayable2_AddUnit());
        }


        public bool BillReceivable2_AddUnit()
        {
            var _DataTable = Source.GetTable(AppliedDB.Enums.Tables.BillReceivable2);
            if (_DataTable.Columns.Contains("Unit")) return true; // Column already exists
            if(Source.MyConnection == null) { return false; }

            try
            {
                if (Source.MyConnection.State != System.Data.ConnectionState.Open) { Source.MyConnection.Open(); }
                var _CommandText = "ALTER TABLE [BillReceivable2] ADD COLUMN Unit INT;";
                var _Command = new SQLiteCommand(_CommandText, Source.MyConnection);
                int _effected = _Command.ExecuteNonQuery();
                if (_effected > 0)
                {
                    MsgClass.Add("Column 'Unit' added to [BillReceivable2] table successfully.");
                    return true;
                }
                else
                {
                    MsgClass.Danger("Column 'Unit' NOT added to [BillReceivable2] table successfully.");
                    return false;
                }
            }
            catch (Exception error)
            {
                ErrorMsgClass.Error(error.Message);
                return false;
            }
        }

        public bool BillPayable2_AddUnit()
        {
            var _DataTable = Source.GetTable(AppliedDB.Enums.Tables.BillPayable2);
            if (_DataTable.Columns.Contains("Unit")) return true; // Column already exists
            if (Source.MyConnection == null) { return false; }

            try
            {
                if (Source.MyConnection.State != System.Data.ConnectionState.Open) { Source.MyConnection.Open(); }
                var _CommandText = "ALTER TABLE [BillPayable2] ADD COLUMN Unit INT;";
                var _Command = new SQLiteCommand(_CommandText, Source.MyConnection);
                int _effected = _Command.ExecuteNonQuery();
                if (_effected > 0)
                {
                    MsgClass.Add("Column 'Unit' added to [BillPayable2] table successfully.");
                    return true;
                }
                else
                {
                    MsgClass.Danger("Column 'Unit' NOT added to BillPayable2 table successfully.");
                    return false;
                }
            }
            catch (Exception error)
            {
                ErrorMsgClass.Error(error.Message);
                return false;
            }
        }
    }
}

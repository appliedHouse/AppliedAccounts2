using AppliedAccounts.Services;
using Microsoft.Data.Sqlite;

namespace AppliedAccounts.Data
{
    public class DataBaseClass
    {
        public SqliteConnection UsersConnection { get; set; }
        public SqliteConnection ClientConnection { get; set; }
        public SqliteConnection LanguageConnection { get; set; }
        public SqliteConnection MessageConnection { get; set; }
        public SqliteConnection SystemConnection { get; set; }
        public SqliteConnection SessionConnection { get; set; }

        public DataBaseClass(GlobalService _AppGlobal)
        {
            AppliedDB.Connections _Connection = new(_AppGlobal.AppPaths);
        }

        public static SqliteConnection? GetMessagesConnection() { return null; }
        public static SqliteConnection? GetLanguageConnection() { return null; }
        public static SqliteConnection? GetClientConnection(string _DBFile) { return null; }
    }
}

using System.Data.SQLite;
using AppliedAccounts.Services;
using AppliedGlobals;

namespace AppliedAccounts.Data
{
    public class DataBaseClass
    {
        public SQLiteConnection UsersConnection { get; set; }
        public SQLiteConnection ClientConnection { get; set; }
        public SQLiteConnection LanguageConnection { get; set; }
        public SQLiteConnection MessageConnection { get; set; }
        public SQLiteConnection SystemConnection { get; set; }
        public SQLiteConnection SessionConnection { get; set; }

        public DataBaseClass(GlobalService _AppGlobals)
        {
            AppliedDB.Connections _Connection = new(_AppGlobals.AppPaths);
        }

        public static SQLiteConnection? GetMessagesConnection() { return null; }
        public static SQLiteConnection? GetLanguageConnection() { return null; }
        public static SQLiteConnection? GetClientConnection(string _DBFile) { return null; }
    }
}

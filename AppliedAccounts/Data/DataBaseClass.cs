using System.Data.SQLite;

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

        public DataBaseClass(AppliedDB.AppUserModel _UserProfile)
        {
            AppliedDB.Connections _Connection = new(_UserProfile);

        }


        public static SQLiteConnection? GetMessagesConnection() { return null; }
        public static SQLiteConnection? GetLanguageConnection() { return null; }
        public static SQLiteConnection? GetClientConnection(string _DBFile) { return null; }
    }
}

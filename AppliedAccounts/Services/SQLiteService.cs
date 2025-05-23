using System.Data.SQLite;

namespace AppliedAccounts.Services
{
    public class SQLiteService
    {
        public AppliedDB.Connections MyConnections { get; set; }
        public SQLiteService(AppliedDB.AppPaths _AppPaths)
        {
            //MyConnections = new(_AppPaths);
        }

        public SQLiteConnection? GetSQLiteConnection(SQLiteConnectionType _ConnectionType)
        {
            switch (_ConnectionType)
            {
                case SQLiteConnectionType.Users:
                    return MyConnections.GetSQLiteUsers();
                case SQLiteConnectionType.Client:
                    return MyConnections.GetSQLiteClient();
                case SQLiteConnectionType.Language:
                    return MyConnections.GetSQLiteLanguage();
                case SQLiteConnectionType.Message:
                    return MyConnections.GetSQLiteMessage();
                case SQLiteConnectionType.System:
                    return MyConnections.GetSQLiteSystem();
                case SQLiteConnectionType.Session:
                    return MyConnections.GetSQLiteSession();
            }
            return null;
        }

        // Changed the accessibility of the enum to public to fix CS0051
        public enum SQLiteConnectionType
        {
            Users,
            Client,
            Language,
            Message,
            System,
            Session
        }
    }
}

using AppliedGlobals;
using Microsoft.Data.Sqlite;

namespace AppliedAccounts.Services
{
    public class SqliteService
    {
        public AppliedDB.Connections MyConnections { get; set; }
        public SqliteService(AppValues.AppPath _AppPaths)
        {
            MyConnections = new(_AppPaths);
        }

        public SqliteConnection? GetSqliteConnection(SqliteConnectionType _ConnectionType)
        {
            switch (_ConnectionType)
            {
                case SqliteConnectionType.Users:
                    return MyConnections.GetSqliteUsers();
                case SqliteConnectionType.Client:
                    return MyConnections.GetSqliteClient();
                case SqliteConnectionType.Language:
                    return MyConnections.GetSqliteLanguage();
                case SqliteConnectionType.Message:
                    return MyConnections.GetSqliteMessage();
                case SqliteConnectionType.System:
                    return MyConnections.GetSqliteSystem();
                case SqliteConnectionType.Session:
                    return MyConnections.GetSqliteSession();
            }
            return null;
        }

        // Changed the accessibility of the enum to public to fix CS0051
        public enum SqliteConnectionType
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

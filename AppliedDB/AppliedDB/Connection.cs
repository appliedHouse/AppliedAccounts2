using AppMessages;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppliedDB
{
    public class Connections : IConnections
    {
        public SQLiteConnection? UsersConnection { get; set; } = GetUsersConnection();
        public SQLiteConnection? ClientConnection { get; set; } = new();
        public SQLiteConnection? MessageConnection { get; set; } = GetMessagesConnection();
        public SQLiteConnection? LanguageConnection { get; set; } = GetLanguageConnection();
        public static string BasePath => Directory.GetCurrentDirectory();
        public static string RootPath = "wwwroot";
        public static string ClientPath = "SQliteDB";
        public static string MessagePath = "Messages";
        public static string LanguagePath = "Languages";
        public static string DB_Users = "AppliedUsers2.db";
        public static string DB_Messages = "Messages.db";
        public static string DB_Language = "Languages.db";
        public static string DB_Client { get; set; } = string.Empty;

        public Connections()
        {
            
        }

        public Connections(AppUserModel UserProfile)
        {
            DB_Client = UserProfile.DataFile;
        }

        public static SQLiteConnection? GetUsersConnection()
        {
            var DBFile = Path.Combine(BasePath, RootPath, ClientPath, "AppliedUsers2.db");
            if (File.Exists(DBFile))
            {
                return GetClientConnection(DBFile);
            }
            return null;
        }
        public static SQLiteConnection? GetMessagesConnection()
        {
            var DBFile = Path.Combine(BasePath, RootPath, MessagePath, "Messages.db");
            if (File.Exists(DBFile))
            {
                return GetClientConnection(DBFile);
            }
            return null;
        }

        public static SQLiteConnection? GetLanguageConnection()
        {
            var DBFile = Path.Combine(BasePath, RootPath, LanguagePath, "Languages.db");
            if (File.Exists(DBFile))
            {
                return GetClientConnection(DBFile);
            }
            return null;
        }
        public static SQLiteConnection? GetClientConnection(string _DBFile)
        {

            var DBFile = Path.Combine(BasePath, RootPath, ClientPath, _DBFile);
            if (File.Exists(DBFile))
            {
                try
                {
                    SQLiteConnection _Connection = new();
                    _Connection.ConnectionString = $"Data Source={DBFile}";
                    return _Connection;
                }
                catch (Exception)
                {
                    // Error handling code type here....
                }
            }
            return null;
        }

    }

    public interface IConnections
    {
        public SQLiteConnection? UsersConnection { get; set; }
        public SQLiteConnection? MessageConnection { get; set; }
    }
}

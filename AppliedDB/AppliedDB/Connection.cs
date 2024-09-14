using System.Data.SQLite;

namespace AppliedDB
{
    public class Connections : IDisposable
    {
        //public SQLiteConnection? UsersConnection { get; set; }
        //public SQLiteConnection? ClientConnection => GetSQLiteClient();
        //public SQLiteConnection? MessageConnection { get; set; }
        //public SQLiteConnection? LanguageConnection { get; set; }
        //public SQLiteConnection? SystemConnection { get; set; }
        //public SQLiteConnection? SessionConnection { get; set; }
        public static string BasePath => Directory.GetCurrentDirectory();
        public static string RootPath { get; set; } = "";
        public static string UsersPath { get; set; } = "";
        public static string ClientPath { get; set; } = "";
        public static string ImagesPath { get; set; } = "";
        public static string MessagePath { get; set; } = "";
        public static string LanguagePath { get; set; } = "";
        public static string ReportPath { get; set; } = "";
        public static string PDFPath { get; set; } = "";
        public static string SystemPath { get; set; } = "";
        public static string SessionPath { get; set; } = "";
        public static string TempDBPath { get; set; } = GetTempDBPath();


        public static string DB_Users = "AppliedUsers2.db";
        public static string DB_Message = "Messages.db";
        public static string DB_Language = "Languages.db";
        public static string DB_System = "System.db";
        public static string DB_Client { get; set; } = string.Empty;
        public static string DB_Session { get; set; } = string.Empty;

        public static AppUserModel AppUserProfile { get; set; }
        

        public Connections() { }
        public Connections(AppUserModel _UserProfile)
        {
            AppUserProfile = _UserProfile;
            DB_Client = AppUserProfile.DataFile;
            DB_Session = AppUserProfile.Session;

            RootPath = AppUserProfile.RootFolder;
            UsersPath = AppUserProfile.UsersFolder;
            ClientPath = AppUserProfile.ClientFolder;
            ImagesPath = AppUserProfile.ImagesFolder;
            LanguagePath = AppUserProfile.LanguageFolder;
            MessagePath = AppUserProfile.MessageFolder;
            ReportPath = AppUserProfile.ReportFolder;
            PDFPath = AppUserProfile.PDFFolder;
            SystemPath = AppUserProfile.SystemFolder;
            SessionPath = AppUserProfile.SessionFolder;
            TempDBPath = AppUserProfile.TempDBFolder;

            //UsersConnection = GetSQLiteConnection(Path.Combine(BasePath, RootPath, UsersPath, DB_Users));
            //ClientConnection = GetSQLiteConnection(Path.Combine(BasePath, RootPath, ClientPath, DB_Client));
            //LanguageConnection = GetSQLiteConnection(Path.Combine(BasePath, RootPath, LanguagePath, DB_Language));
            //MessageConnection = GetSQLiteConnection(Path.Combine(BasePath, RootPath, MessagePath, DB_Message));
            //SystemConnection = GetSQLiteConnection(Path.Combine(BasePath, RootPath, SystemPath, DB_System));
            //SessionConnection = GetSQLiteConnection(Path.Combine(BasePath, RootPath, SessionPath, DB_Session));

        }
        #region Connection non static
        public SQLiteConnection? GetSQLiteUsers()
        {
            return GetSQLiteConnection(Path.Combine(BasePath, RootPath, ClientPath, DB_Client));
        }

        public SQLiteConnection? GetSQLiteClient()
        {
            return GetSQLiteConnection(Path.Combine(BasePath, RootPath, ClientPath, DB_Client));
        }

        public SQLiteConnection? GetSQLiteLanguage()
        {
            return GetSQLiteConnection(Path.Combine(BasePath, RootPath, ClientPath, DB_Language));
        }

        public SQLiteConnection? GetSQLiteMessage()
        {
            return GetSQLiteConnection(Path.Combine(BasePath, RootPath, ClientPath, DB_Message));
        }

        public SQLiteConnection? GetSQLiteSystem()
        {
            return GetSQLiteConnection(Path.Combine(BasePath, RootPath, ClientPath, DB_System));
        }

        public SQLiteConnection? GetSQLiteSession()
        {
            return GetSQLiteConnection(Path.Combine(BasePath, RootPath, ClientPath, DB_Session));
        }

        public static string GetTempDBPath()
        {
            return Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "SQLiteTemp");
        }

        public static string GetExcelPath()
        {
            return Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ExcelFiles");
        }

        #endregion

        #region Static Connection by default values.
        public static SQLiteConnection? GetUsersConnection()
        {
            var DBFile = Path.Combine(BasePath, "wwwroot", "SQLiteDB", DB_Users);
            return GetSQLiteConnection(DBFile);
            //if (File.Exists(DBFile))
            //{
            //    return GetClientConnection(DBFile);
            //}
            //return null;
        }
        public static SQLiteConnection? GetMessagesConnection()
        {
            var DBFile = Path.Combine(BasePath, "wwwroot", "Messages", DB_Message);
            return GetSQLiteConnection(DBFile);
            //if (File.Exists(DBFile))
            //{
            //    return GetClientConnection(DBFile);
            //}
            //return null;
        }
        public static SQLiteConnection? GetLanguageConnection()
        {
            var DBFile = Path.Combine(BasePath, "wwwroot", "Languages", DB_Language);
            return GetSQLiteConnection(DBFile);
            //if (File.Exists(DBFile))
            //{
            //    return GetClientConnection(DBFile);
            //}
            //return null;
        }
        public static SQLiteConnection? GetClientConnection(string _DBFile)
        {
            var DBFile = Path.Combine(BasePath, "wwwroot", "SQLiteDB", _DBFile);
            return GetSQLiteConnection(DBFile);
        }
        public static SQLiteConnection? GetSQLiteConnection(string _UsersDBFile)
        {

            if (File.Exists(_UsersDBFile))
            {
                try
                {
                    SQLiteConnection _Connection = new(); ;
                    _Connection.ConnectionString = $"Data Source={_UsersDBFile}";
                    return _Connection;
                }
                catch (Exception)
                {
                    // Error handling code type here....
                }
            }
            return null;

        }
        #endregion
        public void Dispose()
        {
            //UsersConnection?.Dispose();
            //ClientConnection?.Dispose();
            //MessageConnection?.Dispose();
            //LanguageConnection?.Dispose();
            //SystemConnection?.Dispose();
            //SessionConnection?.Dispose();
        }
    }

    //public interface IConnections
    //{
    //    public SQLiteConnection? UsersConnection { get; set; }
    //    public SQLiteConnection? ClientConnection { get; set; }
    //    public SQLiteConnection? MessageConnection { get; set; }
    //    public SQLiteConnection? LanguageConnection { get; set; }
    //    public SQLiteConnection? SystemConnection { get; set; }
    //    public SQLiteConnection? SessionConnection { get; set; }
    //}
}

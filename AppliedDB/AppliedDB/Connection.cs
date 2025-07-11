using AppliedGlobals;
using System.Data.SQLite;

namespace AppliedDB
{
    public class Connections : IDisposable
    {
        public static AppValues.AppPath AppPaths { get; set; }
        public static string AppPath => Directory.GetCurrentDirectory();
        public static string BaseUrl { get; set; }
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
        public Connections(AppValues.AppPath _AppPaths)
        {
            AppPaths = _AppPaths;
            BaseUrl = _AppPaths.BaseUri;
            RootPath = _AppPaths.RootPath;
            UsersPath = _AppPaths.UsersPath;
            ClientPath = _AppPaths.ClientPath;
            ImagesPath = _AppPaths.ImagesPath;
            MessagePath = _AppPaths.MessagesPath;
            LanguagePath = _AppPaths.LanguagesPath;
            ReportPath = _AppPaths.ReportPath;
            PDFPath = _AppPaths.PDFPath;
            SystemPath = _AppPaths.SystemPath;
            SessionPath = _AppPaths.SessionPath;
            TempDBPath = _AppPaths.DBTempPath;
            DB_Client = _AppPaths.DBFile;
            DB_Session = _AppPaths.DBFile;
        }

        #region Connection non static
        public SQLiteConnection? GetSQLiteUsers()
        {
            return GetSQLiteConnection(Path.Combine(AppPath, RootPath, ClientPath, DB_Client));
        }

        public SQLiteConnection? GetSQLiteClient()
        {
            return GetSQLiteConnection(Path.Combine(AppPath, RootPath, ClientPath, DB_Client));
        }

        public SQLiteConnection? GetSQLiteLanguage()
        {
            return GetSQLiteConnection(Path.Combine(AppPath, RootPath, ClientPath, DB_Language));
        }

        public SQLiteConnection? GetSQLiteMessage()
        {
            return GetSQLiteConnection(Path.Combine(AppPath, RootPath, ClientPath, DB_Message));
        }

        public SQLiteConnection? GetSQLiteSystem()
        {
            return GetSQLiteConnection(Path.Combine(AppPath, RootPath, ClientPath, DB_System));
        }

        public SQLiteConnection? GetSQLiteSession()
        {
            return GetSQLiteConnection(Path.Combine(AppPath, RootPath, ClientPath, DB_Session));
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
            var DBFile = Path.Combine(AppPath, "wwwroot", "SQLiteDB", DB_Users);
            return GetSQLiteConnection(DBFile);
            //if (File.Exists(DBFile))
            //{
            //    return GetClientConnection(DBFile);
            //}
            //return null;
        }
        public static SQLiteConnection? GetMessagesConnection()
        {
            var DBFile = Path.Combine(AppPath, "wwwroot", "Messages", DB_Message);
            return GetSQLiteConnection(DBFile);
            //if (File.Exists(DBFile))
            //{
            //    return GetClientConnection(DBFile);
            //}
            //return null;
        }
        public static SQLiteConnection? GetLanguageConnection()
        {
            var DBFile = Path.Combine(AppPath, "wwwroot", "Languages", DB_Language);
            return GetSQLiteConnection(DBFile);
            //if (File.Exists(DBFile))
            //{
            //    return GetClientConnection(DBFile);
            //}
            //return null;
        }
        public static SQLiteConnection? GetClientConnection(string _DBFile)
        {
            var DBFile = Path.Combine(AppPath, "wwwroot", "SQLiteDB", _DBFile);
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

}

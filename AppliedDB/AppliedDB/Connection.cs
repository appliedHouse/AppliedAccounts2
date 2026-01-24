using AppliedGlobals;
using Microsoft.Data.Sqlite;

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

        public SqliteConnection? GetSqliteUsers()
        {
            return GetSqliteConnection(Path.Combine(AppPath, RootPath, ClientPath, DB_Client));
        }

        public SqliteConnection? GetSqliteClient()
        {
            return GetSqliteConnection(Path.Combine(AppPath, RootPath, ClientPath, DB_Client));
        }

        public SqliteConnection? GetSqliteLanguage()
        {
            return GetSqliteConnection(Path.Combine(AppPath, RootPath, ClientPath, DB_Language));
        }

        public SqliteConnection? GetSqliteMessage()
        {
            return GetSqliteConnection(Path.Combine(AppPath, RootPath, ClientPath, DB_Message));
        }

        public SqliteConnection? GetSqliteSystem()
        {
            return GetSqliteConnection(Path.Combine(AppPath, RootPath, ClientPath, DB_System));
        }

        public SqliteConnection? GetSqliteSession()
        {
            return GetSqliteConnection(Path.Combine(AppPath, RootPath, ClientPath, DB_Session));
        }

        public static string GetTempDBPath()
        {
            return Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "SqliteTemp");
        }

        public static string GetExcelPath()
        {
            return Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ExcelFiles");
        }

        #endregion

        #region Static Connection by default values.
        public static SqliteConnection? GetUsersConnection()
        {
            var DBFile = Path.Combine(AppPath, "wwwroot", "SqliteDB", DB_Users);
            return GetSqliteConnection(DBFile);
            //if (File.Exists(DBFile))
            //{
            //    return GetClientConnection(DBFile);
            //}
            //return null;
        }
        public static SqliteConnection? GetMessagesConnection()
        {
            var DBFile = Path.Combine(AppPath, "wwwroot", "Messages", DB_Message);
            return GetSqliteConnection(DBFile);
            //if (File.Exists(DBFile))
            //{
            //    return GetClientConnection(DBFile);
            //}
            //return null;
        }
        public static SqliteConnection? GetLanguageConnection()
        {
            var DBFile = Path.Combine(AppPath, "wwwroot", "Languages", DB_Language);
            return GetSqliteConnection(DBFile);
      
        }
        public static SqliteConnection? GetClientConnection(string _DBFile)
        {
            var DBFile = Path.Combine(AppPath, "wwwroot", "SqliteDB", _DBFile);
            return GetSqliteConnection(DBFile);
        }
        public static SqliteConnection? GetSqliteConnection(string _UsersDBFile)
        {

            if (File.Exists(_UsersDBFile))
            {
                try
                {
                    SqliteConnection _Connection = new(); ;
                    _Connection.ConnectionString = $"Data Source={_UsersDBFile}";
                    return _Connection;
                }
                catch (Exception ex)
                {
                    // Error handling code type here....
                }
            }
            return null;
        }

        public static SqliteConnection? GetSqliteConnectionbyString(string _ConnectionString)
        {

            if (!string.IsNullOrEmpty(_ConnectionString))
            {
                try
                {
                    SqliteConnection _Connection = new(); ;
                    _Connection.ConnectionString = _ConnectionString;
                    return _Connection;
                }
                catch (Exception ex)
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

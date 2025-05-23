using AppliedAccounts.Authentication;
using AppliedDB;
using System.Data.SQLite;
using System.Data;
using AppliedAccounts.Services;

namespace AppliedAccounts.Pages.Users
{
    public partial class Login
    {
        GlobalService AppGlobals { get; set; } = new();
        private AppUserModel MyModel = new();
        bool IsLogin { get; set; } = true;
        bool IsError { get; set; } = false;
        bool IsUserFound { get; set; } = false;
        string ErrorMessage { get; set; }
        int LanguageID { get; set; } = 1;                       // Default Language is 1 = English

        public async void Submit()
        {
            IsUserFound = false;
            UserProfile AppUser = GetUserProfile(MyModel);

            if (IsUserFound)
            {
                bool IsUser = AppUser.Profile.UserID.Equals(MyModel.UserID);
                bool IsPSW = AppUser.Profile.Password.Equals(MyModel.Password);

                if (IsUser && IsPSW)     // Validate the User Id and Password are equal.
                {
                    var _newGUID = Guid.NewGuid();
                    var userAuthStateProvider = (UserAuthonticationStateProvider)authStateProvider;
                    await userAuthStateProvider.UpdateAuthonticateState(new UserSession
                    {
                        UserName = AppUser.Profile.UserID,
                        Role = AppUser.Profile.Role,
                        DisplayName = AppUser.Profile.DisplayName,
                        Designation = AppUser.Profile.Designation,
                        Email = AppUser.Profile.UserEmail,
                        SQLiteFile = AppUser.Profile.DataFile,
                        CompanyName = AppUser.Profile.Company,
                        PIN = "0000",
                        SessionGuid = _newGUID,
                        LanguageID = LanguageID,

                        // Default Application Foler
                        RootFolder = AppGlobals.AppPaths.RootPath ?? "wwwroot",
                        ClientsFolder = AppGlobals.AppPaths.ClientPath ?? "SQLiteDB",
                        UsersFolder = AppGlobals.AppPaths.UsersPath ?? "SQLiteDB",
                        ReportFolder = AppGlobals.AppPaths.ReportPath ?? "Reports",
                        LanguageFolder = AppGlobals.AppPaths.LanguagesPath ?? "Languages",
                        MessageFolder = AppGlobals.AppPaths.MessagesPath ?? "Messages",
                        ImageFolder = AppGlobals.AppPaths.ImagesPath ?? "Images",
                        PDFFolder = AppGlobals.AppPaths.PDFPath ?? "PDFReport",
                        SystemFolder = AppGlobals.AppPaths.SystemPath ?? "System",
                        SessionFolder = AppGlobals.AppPaths.SessionPath ?? "Sessions",

                    });

                    NavManager.NavigateTo("/", true);

                }
                else
                {
                    IsLogin = false;
                }
            }
            else
            {
                IsError = true;
                ErrorMessage = $"User: {MyModel.UserID} not exist.";
            }
        }
        public void ReLoad()
        {
            IsLogin = true;
            return;
        }
        private UserProfile GetUserProfile(AppUserModel _UserModel)
        {
            var _UserProfile = new UserProfile();
            try
            {
                var _Profile = new AppUserModel();
                var _UserID = _UserModel.UserID;
                if (_UserModel != null)
                {
                    var UsersDBFile = Path.Combine(
                        AppGlobals.AppPaths.FirstPath,
                        AppGlobals.AppPaths.RootPath,
                        AppGlobals.AppPaths.UsersPath, "AppliedUsers2.db");

                    var _CommandText = $"SELECT * FROM [Users] WHERE [UserID] = '{_UserID}'";
                    var _Connection = Connections.GetSQLiteConnection(UsersDBFile); _Connection?.Open();
                    SQLiteCommand _Command = new(_CommandText, _Connection);
                    SQLiteDataAdapter _Adapter = new(_Command);
                    DataSet _DataSet = new();
                    _Adapter.Fill(_DataSet, "Users");
                    _Connection?.Close();

                    if (_DataSet.Tables.Count == 1)
                    {
                        if (_DataSet.Tables[0].Rows.Count > 0)
                        {
                            var _UserData = _DataSet.Tables[0].Rows[0];
                            if (_UserData != null)
                            {
                                _UserProfile.Profile = new()
                                {
                                    UserID = _UserData["UserID"].ToString() ?? "",
                                    Password = _UserData["Password"].ToString() ?? "",
                                    DisplayName = _UserData["DisplayName"].ToString() ?? "",
                                    Designation = _UserData["Designation"].ToString() ?? "",
                                    UserEmail = _UserData["UserEmail"].ToString() ?? "",
                                    Role = _UserData["Role"].ToString() ?? "",
                                    LastLogin = _UserData["LastLogin"].ToString() ?? "",
                                    Session = Guid.NewGuid().ToString(),
                                    DataFile = _UserData["DataFile"].ToString() ?? "",
                                    Company = _UserData["Company"].ToString() ?? "",
                                };
                                IsUserFound = true;
                            }
                        }
                        else
                        {
                            IsLogin = false;
                            ErrorMessage = "User Not Found...";
                            //NavManager.NavigateTo("/AppError");
                        }
                    }
                }
            }
            catch (Exception error)
            {
                IsError = true;
                ErrorMessage = error.Message;
            }


            return _UserProfile;
        }
    }
}

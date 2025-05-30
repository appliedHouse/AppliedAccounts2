using AppliedAccounts.Authentication;
using AppliedAccounts.Services;
using AppliedDB;
using System.Data;
using System.Data.SQLite;

namespace AppliedAccounts.Pages.Users
{
    public partial class Login
    {
        
        private AppliedGlobals.AppUserModel MyModel = new();
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
                    var userAuthStateProvider = (UserAuthenticationStateProvider)authStateProvider;
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
                        //RootFolder = AppGlobal.AppPaths.RootPath ?? "wwwroot",
                        //ClientsFolder = AppGlobal.AppPaths.ClientPath ?? "SQLiteDB",
                        //UsersFolder = AppGlobal.AppPaths.UsersPath ?? "SQLiteDB",
                        //ReportFolder = AppGlobal.AppPaths.ReportPath ?? "Reports",
                        //LanguageFolder = AppGlobal.AppPaths.LanguagesPath ?? "Languages",
                        //MessageFolder = AppGlobal.AppPaths.MessagesPath ?? "Messages",
                        //ImageFolder = AppGlobal.AppPaths.ImagesPath ?? "Images",
                        //PDFFolder = AppGlobal.AppPaths.PDFPath ?? "PDFReport",
                        //SystemFolder = AppGlobal.AppPaths.SystemPath ?? "System",
                        //SessionFolder = AppGlobal.AppPaths.SessionPath ?? "Sessions",

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
        private UserProfile GetUserProfile(AppliedGlobals.AppUserModel _UserModel)
        {
            var _UserProfile = new UserProfile();
            try
            {
                var _Profile = new AppliedGlobals.AppUserModel();
                var _UserID = _UserModel.UserID;
                if (_UserModel != null)
                {
                    var UsersDBFile = Path.Combine(
                        AppGlobal.AppPaths.FirstPath,
                        AppGlobal.AppPaths.RootPath,
                        AppGlobal.AppPaths.UsersPath, "AppliedUsers2.db");

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
                                    Address1 = _UserData["Address1"].ToString() ?? "",
                                    Address2 = _UserData["Address2"].ToString() ?? "",
                                    City = _UserData["City"].ToString() ?? "",
                                    Country = _UserData["Country"].ToString() ?? "",
                                    Contact = _UserData["Contact"].ToString() ?? ""
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

﻿using AppliedAccounts.Authentication;
using AppliedDB;
using System.Data.SQLite;
using System.Data;
using AppliedAccounts.Data;
using System.Data.Entity;

namespace AppliedAccounts.Pages.Users
{
    public partial class Login
    {
        Globals AppGlobals { get; set; } = new();
        private AppUserModel Model = new();
        string NavToHome { get; set; } = string.Empty;
        bool IsLogin { get; set; } = true;
        int LanguageID { get; set; } = 1;                       // Default Language is 1 = English

        public async void Submit()
        {
            UserProfile AppUser = GetUserProfile(Model);


            bool IsUser = AppUser.Profile.UserID.Equals(Model.UserID);
            bool IsPSW = AppUser.Profile.Password.Equals(Model.Password);

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

                // This will create a SQL Lite Database Table for local Session
                // Values could be store and retrive

                // var _Path = Path.Combine(AppUser.Profile.DataPath, "System");
                // AppliedAccounts.Data.AppLocalDBSession _SessionDB = new(_Path, _newGUID);

                // END.


                NavManager.NavigateTo("/", true);

            }
            else
            {
                IsLogin = false;
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
                        if ((DataRow)_UserData != null)
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
                        }
                    }
                    else
                    {
                        NavManager.NavigateTo("/AppError");
                    }
                }
            }

            return _UserProfile;
        }
    }
}

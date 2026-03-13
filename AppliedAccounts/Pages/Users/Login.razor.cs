using AppliedAccounts.Authentication;
using AppliedAccounts.Middleware;
using AppliedDB;
using Microsoft.Data.Sqlite;
using System;
using System.Data;

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
        private static readonly Lock _lock = new();

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
                    var _UserData = new UserSession();

                    _UserData.UserName = AppUser.Profile.UserID;
                    _UserData.Role = AppUser.Profile.Role;
                    _UserData.DisplayName = AppUser.Profile.DisplayName;
                    _UserData.Designation = AppUser.Profile.Designation;
                    _UserData.Email = AppUser.Profile.UserEmail;
                    _UserData.SqliteFile = AppUser.Profile.DataFile;
                    _UserData.CompanyName = AppUser.Profile.Company;
                    _UserData.PIN = "0000";
                    _UserData.SessionGuid = _newGUID;
                    _UserData.LanguageID = LanguageID;

                    bool IsDBFileValid = false;
                    await userAuthStateProvider.UpdateAuthonticateState(_UserData);
                    IsDBFileValid = await UserDatabaseFileValidateAsync(AppUser.Profile.DataFile);

                    if(IsDBFileValid)
                    {
                        NavManager.NavigateTo("/", true);
                    }
                    else
                    {
                        NavManager.NavigateTo("/DBNotValidate", true);
                    }


                    

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


        private async Task<bool> UserDatabaseFileValidateAsync(string dataFile)
        {
            try
            {
                lock (_lock)
                {
                    if (string.IsNullOrEmpty(dataFile))
                        throw new Exception("User DataFile not assigned.");

                    var dbPath = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot",
                        "SqliteDB",
                        dataFile
                    );

                    if (!File.Exists(dbPath))
                    {
                        return false;
                    }
                        

                    // Try opening a connection
                    using var conn = new SqliteConnection($"Data Source={dbPath}");
                    conn.Open(); // Will throw if invalid/corrupt
                    return true;

                }
            }
            catch
            {
                return false;
            }
        }

        public void ReLoad()
        {
            IsLogin = true;
            return;
        }
        private UserProfile GetUserProfile(AppliedGlobals.AppUserModel userModel)
        {
            var userProfile = new UserProfile();

            try
            {
                // Validate input
                if (userModel == null)
                {
                    IsLogin = false;
                    ErrorMessage = "User model cannot be null";
                    return userProfile;
                }

                if (string.IsNullOrWhiteSpace(userModel.UserID))
                {
                    IsLogin = false;
                    ErrorMessage = "User ID cannot be empty";
                    return userProfile;
                }

                var usersDBFile = Path.Combine(
                    AppGlobal.AppPaths.FirstPath,
                    AppGlobal.AppPaths.RootPath,
                    AppGlobal.AppPaths.UsersPath,
                    "AppliedUsers2.db");

                // Verify database file exists
                if (!File.Exists(usersDBFile))
                {
                    IsLogin = false;
                    ErrorMessage = "User database not found";
                    return userProfile;
                }

                const string commandText = "SELECT * FROM [Users] WHERE [UserID] = @UserID";

                using var connection = new SqliteConnection($"Data Source={usersDBFile}");
                connection.Open();

                using var command = new SqliteCommand(commandText, connection);
                command.Parameters.AddWithValue("@UserID", userModel.UserID);

                using var reader = command.ExecuteReader();
                var dataTable = new DataTable();
                dataTable.Load(reader);

                // Check if we got any results
                if (dataTable.Rows.Count == 0)
                {
                    IsLogin = false;
                    ErrorMessage = "User not found";
                    return userProfile;
                }

                var userData = dataTable.Rows[0];

                // Create user profile with null checks
                userProfile.Profile = new AppliedGlobals.AppUserModel
                {
                    UserID = GetSafeString(userData["UserID"]),
                    Password = GetSafeString(userData["Password"]),
                    DisplayName = GetSafeString(userData["DisplayName"]),
                    Designation = GetSafeString(userData["Designation"]),
                    UserEmail = GetSafeString(userData["UserEmail"]),
                    Role = GetSafeString(userData["Role"]),
                    LastLogin = GetSafeString(userData["LastLogin"]),
                    Session = Guid.NewGuid().ToString(),
                    DataFile = GetSafeString(userData["DataFile"]),
                    Company = GetSafeString(userData["Company"]),
                    Address1 = GetSafeString(userData["Address1"]),
                    Address2 = GetSafeString(userData["Address2"]),
                    City = GetSafeString(userData["City"]),
                    Country = GetSafeString(userData["Country"]),
                    Contact = GetSafeString(userData["Contact"])
                };

                IsUserFound = true;
                IsLogin = true; // Assuming login should be true when user is found
            }
            catch (SqliteException sqlEx)
            {
                IsError = true;
                ErrorMessage = $"Database error: {sqlEx.Message}";
                // Log the exception here if you have logging
            }
            catch (Exception ex)
            {
                IsError = true;
                ErrorMessage = $"Unexpected error: {ex.Message}";
                // Log the exception here if you have logging
            }

            return userProfile;
        }

        // Helper method for safe string conversion
        private static string GetSafeString(object value)
        {
            return value?.ToString()?.Trim() ?? string.Empty;
        }
    }
}

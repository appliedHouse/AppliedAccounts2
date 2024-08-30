using AppliedAccounts.Authentication;
using AppliedDB;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Security.Claims;

namespace AppliedBlazorApp.Authentication
{
    public class UserAuthonticationStateProvider : AuthenticationStateProvider
    {
        private readonly ProtectedSessionStorage _sessionStorage;
        private readonly ClaimsPrincipal AnyOne = new(new ClaimsIdentity());
        public AppUserModel AppUser = new AppUserModel();
        public IEnumerable<Claim> Claims { get; set; }

        public UserAuthonticationStateProvider(ProtectedSessionStorage sessionStorage)
        {
            _sessionStorage = sessionStorage;
            Claims = new List<Claim>();
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var userSessionStorageResult = await _sessionStorage.GetAsync<UserSession>("UserSession");
                var userSession = userSessionStorageResult.Success ? userSessionStorageResult.Value : null;
                if (userSession == null)
                {
                    return await Task.FromResult(new AuthenticationState(AnyOne));
                }
                else
                {
                    var ClaimPrincipal = new ClaimsPrincipal(new ClaimsIdentity(
                        new List<Claim>
                            {
                                new (ClaimTypes.Name, userSession.UserName),
                                new (ClaimTypes.Role, userSession.Role),
                                new (ClaimTypes.Email, userSession.Email),
                                new ("DBFile",userSession.SQLiteFile),
                                new ("Company",userSession.CompanyName),
                                new ("Designation",userSession.Designation),
                                new ("DisplayName",userSession.DisplayName),
                                new ("PIN",userSession.PIN),
                                new ("LanguageID",userSession.LanguageID.ToString()),
                                
                                new ("RootFolder",userSession.RootFolder.ToString()),
                                new ("UsersFolder",userSession.UsersFolder.ToString()),
                                new ("ClientsFolder",userSession.ClientsFolder.ToString()),  // Data base Folder
                                new ("ReportFolder",userSession.ReportFolder.ToString()),
                                new ("PDFFolder",userSession.PDFFolder.ToString()),
                                new ("MessageFolder",userSession.MessageFolder.ToString()),
                                new ("LanguageFolder",userSession.LanguageFolder.ToString()),
                                new ("ImageFolder",userSession.ImageFolder.ToString()),
                                new ("SystemFolder",userSession.SystemFolder.ToString()),
                                new ("SessionFolder",userSession.SessionFolder.ToString()),
                            }, "AppliedAuth"));

                    var _Result = await Task.FromResult(new AuthenticationState(ClaimPrincipal));
                    GetAppUser(_Result);

                    Claims = _Result.User.Claims.ToList();
                    return _Result;
                }
            }
            catch (Exception)
            {
                return await Task.FromResult(new AuthenticationState(AnyOne));
            }


        }

        public async Task UpdateAuthonticateState(UserSession? userSession)
        {
            ClaimsPrincipal claimsPrincipal;
            if (userSession != null)
            {
                await _sessionStorage.SetAsync("UserSession", userSession);
                claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
                            {
                                new (ClaimTypes.Name, userSession.UserName),
                                new (ClaimTypes.Role, userSession.Role),
                                new (ClaimTypes.Email, userSession.Email),
                                new ("DBFile",userSession.SQLiteFile),
                                new ("Company",userSession.CompanyName),
                                new ("Designation",userSession.Designation),
                                new ("DisplayName",userSession.DisplayName),
                                new ("PIN",userSession.PIN),
                                new ("LanguageID",userSession.LanguageID.ToString()),

                                new ("RootFolder",userSession.RootFolder.ToString()),
                                new ("UsersFolder",userSession.UsersFolder.ToString()),
                                new ("ClientsFolder",userSession.ClientsFolder.ToString()),  // Data base Folder
                                new ("ReportFolder",userSession.ReportFolder.ToString()),
                                new ("PDFFolder",userSession.PDFFolder.ToString()),
                                new ("MessageFolder",userSession.MessageFolder.ToString()),
                                new ("LanguageFolder",userSession.LanguageFolder.ToString()),
                                new ("ImageFolder",userSession.ImageFolder.ToString()),
                                new ("SystemFolder",userSession.SystemFolder.ToString()),
                                new ("SessionFolder",userSession.SessionFolder.ToString()),
                            }, "AppliedAuth"));
            }
            else
            {
                await _sessionStorage.DeleteAsync("UserSession");
                claimsPrincipal = AnyOne;
            }
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
        }


        public void GetAppUser(AuthenticationState _AuthState)
        {
            var claims = _AuthState.User.Identities.First().Claims.ToList();
            AppUser = new AppUserModel();
            {
                AppUser.UserID = _AuthState.User.Identity?.Name ?? "";
                AppUser.Password = "";
                AppUser.DisplayName = claims?.FirstOrDefault(x => x.Type.Equals("DisplayName", StringComparison.OrdinalIgnoreCase))?.Value ?? "";
                AppUser.Designation = claims?.FirstOrDefault(x => x.Type.Equals("Designation", StringComparison.OrdinalIgnoreCase))?.Value ?? "";
                AppUser.UserEmail = claims?.FirstOrDefault(x => x.Type.Equals("UserEmail", StringComparison.OrdinalIgnoreCase))?.Value ?? "";
                AppUser.Role = claims?.FirstOrDefault(x => x.Type.Equals("Role", StringComparison.OrdinalIgnoreCase))?.Value ?? "";
                AppUser.LastLogin = claims?.FirstOrDefault(x => x.Type.Equals("Lastlogin", StringComparison.OrdinalIgnoreCase))?.Value ?? "";
                AppUser.DataFile = claims?.FirstOrDefault(x => x.Type.Equals("DBFile", StringComparison.OrdinalIgnoreCase))?.Value ?? "";
                AppUser.Company = claims?.FirstOrDefault(x => x.Type.Equals("Company", StringComparison.OrdinalIgnoreCase))?.Value ?? "";
                AppUser.PIN = claims?.FirstOrDefault(x => x.Type.Equals("PIN", StringComparison.OrdinalIgnoreCase))?.Value ?? "";
                AppUser.LanguageID = int.Parse(claims?.FirstOrDefault(x => x.Type.Equals("LanguageID", StringComparison.OrdinalIgnoreCase))?.Value ?? "");


                // Default values
                AppUser.RootFolder = claims?.FirstOrDefault(x => x.Type.Equals("RootFolder", StringComparison.OrdinalIgnoreCase))?.Value ?? "SQLiteDB";
                AppUser.UsersFolder = claims?.FirstOrDefault(x => x.Type.Equals("UsersFolder", StringComparison.OrdinalIgnoreCase))?.Value ?? "SQLiteDB";
                AppUser.ClientFolder = claims?.FirstOrDefault(x => x.Type.Equals("ClientFolder", StringComparison.OrdinalIgnoreCase))?.Value ?? "SQLiteDB";
                AppUser.ReportFolder = claims?.FirstOrDefault(x => x.Type.Equals("ReportFolder", StringComparison.OrdinalIgnoreCase))?.Value ?? "Reports";
                AppUser.PDFFolder = claims?.FirstOrDefault(x => x.Type.Equals("PDFFolder", StringComparison.OrdinalIgnoreCase))?.Value ?? "PDFReports";
                AppUser.MessageFolder = claims?.FirstOrDefault(x => x.Type.Equals("MessageFolder", StringComparison.OrdinalIgnoreCase))?.Value ?? "Messages";
                AppUser.LanguageFolder = claims?.FirstOrDefault(x => x.Type.Equals("LanguageFolder", StringComparison.OrdinalIgnoreCase))?.Value ?? "Languages";
                AppUser.ImagesFolder = claims?.FirstOrDefault(x => x.Type.Equals("ImageFolder", StringComparison.OrdinalIgnoreCase))?.Value ?? "Images";
                AppUser.SystemFolder = claims?.FirstOrDefault(x => x.Type.Equals("SystemFolder", StringComparison.OrdinalIgnoreCase))?.Value ?? "System";
                AppUser.SessionFolder = claims?.FirstOrDefault(x => x.Type.Equals("SessionFolder", StringComparison.OrdinalIgnoreCase))?.Value ?? "Sessions";
            };


        }


    }
}

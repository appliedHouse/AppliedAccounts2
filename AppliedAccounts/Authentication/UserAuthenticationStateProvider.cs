using AppliedGlobals;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Security.Claims;


namespace AppliedAccounts.Authentication
{
    public class UserAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ProtectedSessionStorage _sessionStorage;
        private readonly ClaimsPrincipal AnyOne = new(new ClaimsIdentity());
        private readonly NavigationManager _navManager;
        public AppUserModel AppUser = new AppUserModel();
        public IEnumerable<Claim> Claims { get; set; }

        public UserAuthenticationStateProvider(ProtectedSessionStorage sessionStorage, NavigationManager NavManager)
        {
            _sessionStorage = sessionStorage;
            _navManager = NavManager;
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
                    var ClaimPrincipal = CreatePrincipal(userSession);
                    var _Result = await Task.FromResult(new AuthenticationState(ClaimPrincipal));
                    GetAppUser(_Result);

                    Claims = [.. _Result.User.Claims];
                    return _Result;
                }
            }
            catch (Exception)
            {
                return await Task.FromResult(new AuthenticationState(AnyOne));
            }
        }

        public async Task UpdateAuthenticateState(UserSession? userSession)
        {
            ClaimsPrincipal claimsPrincipal;
            if (userSession != null)
            {
                await _sessionStorage.SetAsync("UserSession", userSession);
                claimsPrincipal = CreatePrincipal(userSession);
                
            }
            else
            {
                await _sessionStorage.DeleteAsync("UserSession");
                claimsPrincipal = AnyOne;
            }


            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
        }


        private ClaimsPrincipal CreatePrincipal(UserSession user)
        {
            var claims = new List<Claim>
            {
                new (ClaimTypes.Name, user.UserName),
                new (ClaimTypes.Role, user.Role),
                new (ClaimTypes.Email, user.Email),
                new ("DBFile", user.SqliteFile),
                new ("Company", user.CompanyName),
                new ("Designation", user.Designation),
                new ("DisplayName", user.DisplayName),
                new ("PIN", user.PIN),
                new ("LanguageID", user.LanguageID.ToString()),
                new ("Session", user.SessionGuid.ToString()),

                new ("AppPath", _navManager.BaseUri),
                new ("RootFolder", user.RootFolder.ToString()),
                new ("UsersFolder", user.UsersFolder.ToString()),
                new ("ClientsFolder", user.ClientsFolder.ToString()),  // Data base Folder
                new ("ReportFolder", user.ReportFolder.ToString()),
                new ("PDFFolder", user.PDFFolder.ToString()),
                new ("MessageFolder", user.MessageFolder.ToString()),
                new ("LanguageFolder", user.LanguageFolder.ToString()),
                new ("ImageFolder", user.ImageFolder.ToString()),
                new ("SystemFolder", user.SystemFolder.ToString()),
                new ("SessionFolder", user.SessionFolder.ToString()),
                new ("TempDBFolder", user.TempDBFolder.ToString())
            };

            return new ClaimsPrincipal(
                new ClaimsIdentity(claims, "AppliedAuth"));
        }


        public void GetAppUser(AuthenticationState authState)
        {
            var claims = authState.User.Claims.ToList();

            string GetClaim(string type) =>
                claims.FirstOrDefault(c => c.Type.Equals(type, StringComparison.OrdinalIgnoreCase))?.Value ?? "";

            AppUser = new AppUserModel
            {
                UserID = authState.User.Identity?.Name ?? "",
                Password = "",
                DisplayName = GetClaim("DisplayName"),
                Designation = GetClaim("Designation"),
                UserEmail = GetClaim(ClaimTypes.Email),
                Role = GetClaim(ClaimTypes.Role),
                LastLogin = GetClaim("LastLogin"),
                DataFile = GetClaim("DBFile"),
                Company = GetClaim("Company"),
                PIN = GetClaim("PIN"),
                Session = GetClaim("Session"),
                LanguageID = int.TryParse(GetClaim("LanguageID"), out var language) ? language : 0
            };
        }

        public async Task LogoutAsync()
        {
            await UpdateAuthenticateState(null);
        }

        public enum UserRolls
        {
            Administrator = 1,
            Manager = 2,
            User = 3,
            Guest = 4,
        }

    }
}

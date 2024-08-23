using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Security.Claims;

namespace AppliedAccounts2.Authentication
{
    public class UserAuthonticationStateProvider
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
                                new Claim(ClaimTypes.Name, userSession.UserName),
                                new Claim(ClaimTypes.Role, userSession.Role),
                                new Claim(ClaimTypes.Email, userSession.Email),
                                new Claim("DBFile",userSession.SQliteFile),
                                new Claim("Company",userSession.CompanyName),
                                new Claim("Designation",userSession.Designation),
                                new Claim("DisplayName",userSession.DisplayName),
                                new Claim("PIN",userSession.PIN),
                                new Claim("LanguageID",userSession.LanguageID.ToString())
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
                    new ("SQLiteFile", userSession.SQliteFile),
                    new ("Company", userSession.CompanyName),
                    new ("SessionID", Guid.NewGuid().ToString()),
                    new ("DisplayName", userSession.DisplayName),
                    new ("Designation", userSession.Designation),
                    new ("Email", userSession.Designation),
                    new ("PIN", userSession.PIN),
                    new ("LanguageID", userSession.LanguageID.ToString())
                }));
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
                AppUser.UserID = _AuthState.User.Identity.Name;
                AppUser.Password = "";
                AppUser.DisplayName = claims?.FirstOrDefault(x => x.Type.Equals("DisplayName", StringComparison.OrdinalIgnoreCase))?.Value;
                AppUser.Designation = claims?.FirstOrDefault(x => x.Type.Equals("Designation", StringComparison.OrdinalIgnoreCase))?.Value;
                AppUser.UserEmail = claims?.FirstOrDefault(x => x.Type.Equals("UserEmail", StringComparison.OrdinalIgnoreCase))?.Value;
                AppUser.Role = claims?.FirstOrDefault(x => x.Type.Equals("Role", StringComparison.OrdinalIgnoreCase))?.Value;
                AppUser.LastLogin = claims?.FirstOrDefault(x => x.Type.Equals("Lastlogin", StringComparison.OrdinalIgnoreCase))?.Value;
                AppUser.Session = "";
                AppUser.DataFile = claims?.FirstOrDefault(x => x.Type.Equals("DBFile", StringComparison.OrdinalIgnoreCase))?.Value;
                AppUser.Company = claims?.FirstOrDefault(x => x.Type.Equals("Company", StringComparison.OrdinalIgnoreCase))?.Value;
                AppUser.PIN = claims?.FirstOrDefault(x => x.Type.Equals("PIN", StringComparison.OrdinalIgnoreCase))?.Value;
                AppUser.LanguageID = int.Parse(claims?.FirstOrDefault(x => x.Type.Equals("LanguageID", StringComparison.OrdinalIgnoreCase))?.Value);
            };


        }

    }
}

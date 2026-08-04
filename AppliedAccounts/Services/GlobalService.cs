using AppliedAccounts.Authentication;
using AppliedDB;
using AppliedGlobals;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.JSInterop;
using Org.BouncyCastle.Utilities.Collections;
using static AppliedGlobals.AppValues;

namespace AppliedAccounts.Services
{
    public class GlobalService : IDisposable
    {
        public readonly IConfiguration Config;
        public readonly NavigationManager NavManager;
        public readonly IJSRuntime JS;
        public readonly ILogger<GlobalService> MyLogger;

        public Connections Connections;
        public ProtectedSessionStorage AppStore;

        public AppPath AppPaths { get; set; } = new();
        public AuthorClass Author { get; set; } = new();
        public AppUserModel Client { get; set; } = new();
        public LanguageClass Language { get; set; } = new();
        public CurrencyClass Currency { get; set; } = new();
        public Format Format { get; set; } = new();
        public PrintReport Reporting { get; set; } = new();
        public string DBFile => AppPaths.DBFile;
        public string UserID = string.Empty;
        public string UserRole = string.Empty;

        public event Action? OnInitialized;
        public event Action? OnLanguageChanged;
        public MessagesService MsgService { get; set; }

        private readonly UserAuthenticationStateProvider _authStateProvider;
        private bool _isInitialized = false;
        private readonly SemaphoreSlim _initLock = new SemaphoreSlim(1, 1);

        #region Constructor

        public GlobalService(
            IConfiguration _Config,
            NavigationManager _NavManager,
            IJSRuntime _JS,
            UserAuthenticationStateProvider _StateProvider,
            ILogger<GlobalService> _logger,
            ProtectedSessionStorage _sessionStorage)
        {
            Config = _Config;
            NavManager = _NavManager;
            JS = _JS;
            _authStateProvider = _StateProvider;
            MyLogger = _logger;
            MsgService = new(_Config);
            AppStore = _sessionStorage;

            _ = InitializeAsync();
            InitializeStaticProperties();
        }

        private async Task InitializeAsync()
        {
            await _initLock.WaitAsync();
            try
            {
                if (_isInitialized) return;

                var authState = await _authStateProvider.GetAuthenticationStateAsync();

                Client = _authStateProvider.AppUser ?? new AppUserModel();

                AppPaths.DBFile = Client.DataFile;
                UserID = Client.UserID;
                UserRole = Client.Role;

                var databaseConfig = new DatabaseConfig();
                var connectionLogger = MyLogger as ILogger<Connections> ??
                    LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<Connections>();
                Connections = new Connections(AppPaths, connectionLogger);

                _isInitialized = true;

                OnInitialized?.Invoke();
            }
            catch (Exception ex)
            {
                MyLogger.LogError(ex, "Error initializing GlobalService");
            }
            finally
            {
                _initLock.Release();
            }
        }

        private void InitializeStaticProperties()
        {
            AppPaths.BaseUri = NavManager.BaseUri;
            AppPaths.FirstPath = Directory.GetCurrentDirectory();
            AppPaths.RootPath = Config.GetValue<string>("Paths:RootPath") ?? "wwwroot";

            AppPaths.SystemPath = Config.GetValue<string>("Paths:SystemPath") ?? "System";
            AppPaths.ImagesPath = Config.GetValue<string>("Paths:ImagesPath") ?? "Images";
            AppPaths.ReportPath = Config.GetValue<string>("Paths:ReportPath") ?? "Reports";
            AppPaths.LanguagesPath = Config.GetValue<string>("Paths:LanguagesPath") ?? "Languages";
            AppPaths.MessagesPath = Config.GetValue<string>("Paths:MessagesPath") ?? "Messages";
            AppPaths.PDFPath = Config.GetValue<string>("Paths:PDFPath") ?? "PDFReports";
            AppPaths.ClientPath = Config.GetValue<string>("Paths:ClientPath") ?? "SqliteDB";
            AppPaths.UsersPath = Config.GetValue<string>("Paths:UsersPath") ?? "SqliteDB";
            AppPaths.DBTempPath = Config.GetValue<string>("Paths:DBTempPath") ?? "SqliteTemp";
            AppPaths.SessionPath = Config.GetValue<string>("Paths:SessionPath") ?? "Sessions";
            AppPaths.ExcelFilesPath = Config.GetValue<string>("Paths:ExcelFilesPath") ?? "ExcelFiles";

            Author = new()
            {
                Company = Config.GetValue<string>("Author:Company") ?? "",
                Address1 = Config.GetValue<string>("Author:Address1") ?? "",
                Address2 = Config.GetValue<string>("Author:Address2") ?? "",
                City = Config.GetValue<string>("Author:City") ?? "",
                Country = Config.GetValue<string>("Author:Country") ?? "",
                Contact = Config.GetValue<string>("Author:Contact") ?? "",
                Email = Config.GetValue<string>("Author:Email") ?? "",
                Url = Config.GetValue<string>("Author:Url") ?? "",
                Url2 = Config.GetValue<string>("Author:Url2") ?? "",
            };

            Language = new()
            {
                ID = Config.GetValue<int>("Language:ID"),
                Sign = Config.GetValue<string>("Language:Sign"),
                Title = Config.GetValue<string>("Language:Title"),
            };

            Currency = new()
            {
                ID = Config.GetValue<int>("Currency:ID"),
                Sign = Config.GetValue<string>("Currency:Sign") ?? "",
                Title = Config.GetValue<string>("Currency:Title") ?? "",
                Format = Config.GetValue<string>("Currency:Format") ?? "",
                Units = Config.GetValue<string>("Currency:Units") ?? "",
            };

            Reporting = new()
            {
                ReportFooter = Config.GetValue<string>("Report:ReportFooter") ?? "",
                ReportTitle = Config.GetValue<string>("Report:ReportTitle") ?? "",
                ReportLogo = Config.GetValue<string>("Report:ReportLogo") ?? "",
            };

            AppStore.SetAsync("AppPaths", AppPaths);
            AppStore.SetAsync("Author", Author);
            AppStore.SetAsync("Language", Language);
            AppStore.SetAsync("Currency", Currency);
            AppStore.SetAsync("Reporting", Reporting);  

        }

        #endregion

      
        // Public method to ensure initialization
        public async Task EnsureInitializedAsync()
        {
            if (!_isInitialized)
            {
                await InitializeAsync();
            }
        }

        // Refresh user data after login/logout
        public async Task RefreshUserDataAsync()
        {
            var authState = await _authStateProvider.GetAuthenticationStateAsync();
            Client = _authStateProvider.AppUser ?? new AppUserModel();

            // Update paths if needed
            AppPaths.DBFile = Client.DataFile;
            UserID = Client.UserID;
            UserRole = Client.Role;
        }

        #region MinDate and MaxDate
        public DateTime MinDate() => GetMinDate();

        private DateTime GetMinDate()
        {
            var _result = new DateTime(2000, 1, 1);
            try
            {
                if (_isInitialized && Connections != null)
                {
                    DataSource Source = new(AppPaths);
                    _result = Source.GetDate("MinDate");
                }
            }
            catch (Exception ex)
            {
                MyLogger?.LogError(ex, "Error getting MinDate");
                _result = new DateTime(2000, 1, 1);
            }
            return _result;
        }

        public DateTime MaxDate() => GetMaxDate();

        private DateTime GetMaxDate()
        {
            var _result = new DateTime(2030, 12, 31);
            try
            {
                if (_isInitialized && Connections != null)
                {
                    DataSource Source = new(AppPaths);
                    _result = Source.GetDate("MaxDate");
                }
            }
            catch (Exception ex)
            {
                MyLogger?.LogError(ex, "Error getting MaxDate");
                _result = new DateTime(2030, 12, 31);
            }
            return _result;
        }
        #endregion

        #region Language Management
        public void SetLanguage(int id)
        {
            if (Language.ID == id)
                return;

            Language.ID = id;
            OnLanguageChanged?.Invoke();
        }

        #endregion

        #region Protected Session Storage

        public async Task StoreSaveAsync<T>(string key, T value)
        {
            await AppStore.SetAsync(key, value!);
        }

        public async Task<T?> StoreLoadAsync<T>(string key)
        {
            var result = await AppStore.GetAsync<T>(key);
            return result.Success ? result.Value : default;
        }

        public async Task<T> StoreLoadAsync<T>(string key, T defaultValue)
        {
            var result = await AppStore.GetAsync<T>(key);
            return result.Success ? result.Value! : defaultValue;
        }

        public async Task<bool> StoreExistsAsync(string key)
        {
            var result = await AppStore.GetAsync<object>(key);
            return result.Success;
        }

        public async Task StoreRemoveAsync(string key)
        {
            await AppStore.DeleteAsync(key);
        }

        public async Task StoreClearAsync(params string[] keys)
        {
            foreach (var key in keys)
            {
                await AppStore.DeleteAsync(key);
            }
        }

        #endregion

        #region Dispose Pattern
        public void Dispose()
        {
            _initLock?.Dispose();
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
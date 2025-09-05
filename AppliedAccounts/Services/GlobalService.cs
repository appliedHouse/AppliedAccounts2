using AppliedAccounts.Authentication;
using AppliedGlobals;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using static AppliedGlobals.AppValues;

namespace AppliedAccounts.Services
{
    public class GlobalService
    {
        public readonly IConfiguration Config;
        public readonly NavigationManager NavManager;
        public readonly IJSRuntime JS;

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

        public GlobalService() { }

        public GlobalService(IConfiguration _Config, NavigationManager _NavManager, IJSRuntime _JS, AuthenticationStateProvider _StateProvider)
        {
            Config = _Config;
            NavManager = _NavManager;
            JS = _JS;

            Client = ((UserAuthenticationStateProvider)_StateProvider).AppUser;

            AppPaths.DBFile = Client.DataFile;
            UserID = Client.UserID;
            UserRole = Client.Role;

            AppPaths.BaseUri = NavManager.BaseUri;
            AppPaths.FirstPath = Directory.GetCurrentDirectory();
            AppPaths.RootPath = Config.GetValue<string>("Paths:RootPath") ?? "wwwroot";

            AppPaths.SystemPath = Config.GetValue<string>("Paths:SystemPath") ?? "System";
            AppPaths.ImagesPath = Config.GetValue<string>("Paths:ImagesPath") ?? "Images";
            AppPaths.ReportPath = Config.GetValue<string>("Paths:ReportPath") ?? "Reports";
            AppPaths.LanguagesPath = Config.GetValue<string>("Paths:LanguagesPath") ?? "Languages";
            AppPaths.MessagesPath = Config.GetValue<string>("Paths:MessagesPath") ?? "Messages";
            AppPaths.PDFPath = Config.GetValue<string>("Paths:PDFPath") ?? "PDFReports";
            AppPaths.ClientPath = Config.GetValue<string>("Paths:ClientPath") ?? "SQLiteDB";
            AppPaths.UsersPath = Config.GetValue<string>("Paths:UsersPath") ?? "SQLiteDB";
            AppPaths.DBTempPath = Config.GetValue<string>("Paths:DBTempPath") ?? "SQLiteTemp";
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
                Sign = Config.GetValue<string>("Currency:Sign"),
                Title = Config.GetValue<string>("Currency:Title"),
                Format = Config.GetValue<string>("Currency:Format"),
            };

            Reporting = new()
            {
                ReportFooter = Config.GetValue<string>("Report:ReportFooter") ?? "",
                ReportTitle = Config.GetValue<string>("Report:ReportTitle") ?? "",
                ReportLogo = Config.GetValue<string>("Report:ReportLogo") ?? "",
            };

        }

    }
}


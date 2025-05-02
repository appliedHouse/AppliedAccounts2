using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace AppliedAccounts.Services
{
    public class GlobalService
    {
        public readonly IConfiguration Config;
        public readonly NavigationManager NavManager;
        public readonly IJSRuntime JS;

        public AppPath AppPaths { get; set; } = new();
        public AuthorClass Author { get; set; } = new();
        public LanguageClass Language { get; set; } = new();
        public CurrencyClass Currency { get; set; } = new();
        public Format AppFormat { get; set; } = new();

        public GlobalService() { }

        public GlobalService(IConfiguration _Config, NavigationManager _NavManager, IJSRuntime _JS)
        {
            Config = _Config;
            NavManager = _NavManager;
            JS = _JS;

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


        }
    }

    public class AppPath
    {
        public string BaseUri { get; set; } = string.Empty;
        public string FirstPath { get; set; } = string.Empty;
        public string RootPath { get; set; } = string.Empty;
        public string ReportPath { get; set; } = string.Empty;
        public string UsersPath { get; set; } = string.Empty;
        public string ClientPath { get; set; } = string.Empty;
        public string PDFPath { get; set; } = string.Empty;
        public string DBTempPath { get; set; } = string.Empty;
        public string SystemPath { get; set; } = string.Empty;
        public string ImagesPath { get; set; } = string.Empty;
        public string LanguagesPath { get; set; } = string.Empty;
        public string MessagesPath { get; set; } = string.Empty;
        public string SessionPath { get; set; } = string.Empty;

    }


    #region Author Class
    public class AuthorClass
    {
        public string Company { get; set; } = string.Empty;
        public string Address1 { get; set; } = string.Empty;
        public string Address2 { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string Contact { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string Url2 { get; set; } = string.Empty;

    }
    #endregion

    #region Language Class
    public class LanguageClass
    {
        public int ID { get; set; }
        public string? Sign { get; set; }
        public string? Title { get; set; }
    }
    #endregion

    #region Currency Class
    public class CurrencyClass
    {
        public int ID { get; set; }
        public string? Sign { get; set; }
        public string? Title { get; set; }
        public string? Format { get; set; }
    }
    #endregion

    #region Formats
    public class Format
    {
        public static string Number { get; set; } = string.Empty;
        public static string Currency { get; set; } = string.Empty;
        public static string Decimal { get; set; } = string.Empty;

        public static string DDMMYY { get; set; } = "dd-MM-yy";
        public static string DDMMMYYYY { get; set; } = "dd-MMM-yyyy";
        public static string DDMMMYY { get; set; } = "dd-MMM-yy";
        public static string YMD { get; set; } = "yyyy-MM-dd";

        public static string Amount { get; set; } = "#,##0";
        public static string Digit { get; set; } = "#,##0.00";
        public static string Digit4 { get; set; } = "#,##0.0000";
        public static string Digit6 { get; set; } = "#,##0.000000";
    }
    #endregion

    #region Printing Reports
    public class PrintReports
    {
        public string BaseUrl { get; set; }
        public string ReportFooter { get; set; }
        public string ReportTitle { get; set; }
        public string ReportLogo { get; set; }
    }
    #endregion
}


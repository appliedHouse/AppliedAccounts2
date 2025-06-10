

namespace AppliedGlobals
{
    public class AppValues
    {
        public AppPath Paths { get; set; } = new();
        public AuthorClass Author { get; set; } = new();
        public ClientClass Client { get; set; } = new();
        public LanguageClass Language { get; set; } = new();
        public CurrencyClass Currency { get; set; } = new();
        public Format Formats { get; set; } = new();
        public PrintReport Report { get; set; } = new();

        public AppValues() { }

        #region Paths
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
            public string DBFile { get; set; } = string.Empty;

        }

        #endregion

        #region Client Data
        public class ClientClass
        {
            public string DisplayName { get; set; } = string.Empty;
            public string Name { get; set; } = string.Empty;
            public string Address1 { get; set; } = string.Empty;
            public string Address2 { get; set; } = string.Empty;
            public string City { get; set; } = string.Empty;
            public string Country { get; set; } = string.Empty;
            public string Contact { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string Url { get; set; } = string.Empty;
            public string PIN { get; set; } = string.Empty;
            public DateTime LoginTime { get; set; } = DateTime.Now;

        }
        #endregion

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
            public int? Digits { get; set; }
            public string? DigitTitle { get; set; }
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
        public class PrintReport
        {
            public string ReportTitle { get; set; }
            public string ReportFooter { get; set; }
            public string ReportLogo { get; set; }          // Link of logi file   jpg, png, bmp etc
        }
        #endregion
    }
}

namespace AppliedAccounts.Authentication
{
    public class UserSession
    {
        public string UserName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string SQLiteFile { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Designation { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PIN { get; set; } = string.Empty;
        public int LanguageID { get; set; }
        public Guid SessionGuid { get; set; }
        public string AppPath { get; set; } = string.Empty;
        public string RootFolder { get; set; } = string.Empty;
        public string UsersFolder { get; set; } = string.Empty;
        public string ClientsFolder { get; set; } = string.Empty;
        public string ReportFolder { get; set; } = string.Empty;
        public string PDFFolder { get; set; } = string.Empty;
        public string MessageFolder { get; set; } = string.Empty;
        public string LanguageFolder { get; set; } = string.Empty;
        public string ImageFolder { get; set; } = string.Empty;
        public string SystemFolder { get; set; } = string.Empty;
        public string SessionFolder { get; set; } = string.Empty;
        public string TempDBFolder { get; set; } = string.Empty;

        public UserSession()
        {

            PIN = "0000";
            SessionGuid = Guid.NewGuid();
            LanguageID = 1;                         // Default Language 1 is English.
        }

    }
}

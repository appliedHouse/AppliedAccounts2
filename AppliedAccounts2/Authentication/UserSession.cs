namespace AppliedAccounts2.Authentication
{
    public class UserSession
    {
        public string UserName { get; set; }
        public string Role { get; set; }
        public string SQliteFile { get; set; }
        public string CompanyName { get; set; }
        public string DisplayName { get; set; }
        public string Designation { get; set; }
        public string Email { get; set; }
        public string PIN { get; set; }
        public int LanguageID { get; set; }

        public UserSession()
        {
            UserName = string.Empty;
            Role = string.Empty;
            SQliteFile = string.Empty;
            CompanyName = string.Empty;
            DisplayName = string.Empty;
            Designation = string.Empty;
            Email = string.Empty;
            PIN = "0000";
            LanguageID = 1;                         // Default Language 1 is English.
        }

    }
}

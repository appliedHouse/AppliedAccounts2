using System.ComponentModel.DataAnnotations;

namespace AppliedGlobals
{
    public class AppUserModel
    {
        [Required]
        public string UserID { get; set; } = string.Empty;
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Designation { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public string LastLogin { get; set; } = string.Empty;
        public string Session { get; set; } = string.Empty;
        public string DataFile { get; set; } = string.Empty;
        public string DataPath { get; set; } = string.Empty;
        public string Company { get; set; } = string.Empty;
        public string PIN { get; set; } = "0000";
        public bool IsSaved { get; set; }
        public int LanguageID { get; set; } = 1;                // Default Language English.


        public string BasePath { get; set; } = string.Empty;
        public string RootFolder { get; set; } = string.Empty;
        public string ClientFolder { get; set; } = string.Empty;
        public string UsersFolder { get; set; } = string.Empty;
        public string ReportFolder { get; set; } = string.Empty;
        public string PDFFolder { get; set; } = string.Empty;
        public string MessageFolder { get; set; } = string.Empty;
        public string LanguageFolder { get; set; } = string.Empty;
        public string ImagesFolder { get; set; } = string.Empty;
        public string SystemFolder { get; set; } = string.Empty;
        public string SessionFolder { get; set; } = string.Empty;
        public string TempDBFolder { get; set; } = string.Empty;
    }
}

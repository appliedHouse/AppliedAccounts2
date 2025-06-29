﻿using System.ComponentModel.DataAnnotations;

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

        public DateTime LoginTime { get; set; }
        public string LastLogin { get; set; } = string.Empty;
        public string Session { get; set; } = string.Empty;
        public string DataFile { get; set; } = string.Empty;
        //public string DataPath { get; set; } = string.Empty;

        public string Company { get; set; } = string.Empty;
        public string Address1 { get; set; } = string.Empty;
        public string Address2 { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string Contact { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string PIN { get; set; } = "0000";

        public bool RememberMe { get; set; }
        public int LanguageID { get; set; } = 1;                // Default Language English.
    }
}

namespace AppliedAccounts.Models
{
    public class BackupModel
    {
        public string FtpHost { get; set; }
        public string FtpUsername { get; set; }
        public string FtpPasswordHash { get; set; }
        public string FtpRemotePath { get; set; }
        public bool FtpUseSSL { get; set; }
        public int ScheduleHour { get; set; }
        public int ScheduleMinute { get; set; }
    }
}

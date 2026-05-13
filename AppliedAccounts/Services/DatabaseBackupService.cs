using AppliedAccounts.Services;
using System.IO.Compression;

// Backup Service that copy all DB file from applied.com.pk to jahangir.com


public class DatabaseBackupService : BackgroundService
{
    private readonly ILogger<DatabaseBackupService> _logger;
    private readonly string _dbFolder = @"C:\inetpub\wwwroot\AppliedDB"; // Production DB path
    private readonly string _backupFolder = @"C:\BackupTemp"; // Temporary backup path
    private readonly GlobalService AppGlobals;
    private readonly BackupServerModel BackupModel;

    public DatabaseBackupService(ILogger<DatabaseBackupService> logger, GlobalService _Global)
    {
        _logger = logger;
        AppGlobals = _Global;

        BackupModel = new BackupServerModel();
        BackupModel.Domain = "";
        BackupModel.UserName = "";
        BackupModel.Password = "";


    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var now = DateTime.Now;
                // Run daily at 2 AM
                if (now.Hour == 2 && now.Minute == 0)
                {
                    await BackupDatabases();
                    // Wait 1 minute so it does not run multiple times
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                }

                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in database backup service");
            }
        }
    }

    private async Task BackupDatabases()
    {
        // Create temp folder
        Directory.CreateDirectory(_backupFolder);

        var files = Directory.GetFiles(_dbFolder, "*.db");
        string zipFile = Path.Combine(_backupFolder, $"Backup_{DateTime.Now:yyyyMMdd}.zip");

        using (var archive = ZipFile.Open(zipFile, ZipArchiveMode.Create))
        {
            foreach (var file in files)
            {
                archive.CreateEntryFromFile(file, Path.GetFileName(file));
            }
        }

        // Upload zip to backup server via SFTP or FTP
        await UploadBackup(zipFile);

        // Clean up
        File.Delete(zipFile);
    }

    private Task UploadBackup(string filePath)
    {
        return Task.Run(() =>
        {
            using (var sftp = new Renci.SshNet.SftpClient("www.jahangir.com", "username", "password"))
            {
                sftp.Connect();
                using var fs = new FileStream(filePath, FileMode.Open);
                sftp.UploadFile(fs, "/home/backupuser/sqlite_backup/" + Path.GetFileName(filePath));
                sftp.Disconnect();
            }
        });
    }

    private class BackupServerModel
    {
        public string Domain { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }

}

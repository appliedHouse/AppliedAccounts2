using System.Net;
using System.IO.Compression;

public class BackupByFTP : BackgroundService
{
    private readonly ILogger<DatabaseBackupService> _logger;
    private readonly string _dbFolder = @"C:\inetpub\wwwroot\AppliedDB"; // Your SQLite DB folder
    private readonly string _backupTemp = @"C:\BackupTemp"; // Temporary backup folder
    private readonly string _ftpHost = "ftp.jahangir.com"; // FTP server
    private readonly string _ftpUser = "ftpusername";       // FTP username
    private readonly string _ftpPass = "ftppassword";       // FTP password

    public BackupByFTP(ILogger<DatabaseBackupService> logger)
    {
        _logger = logger;
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
                    BackupDatabases();
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

    private void BackupDatabases()
    {
        Directory.CreateDirectory(_backupTemp);

        // Copy DB files to temp folder
        var files = Directory.GetFiles(_dbFolder, "*.db");
        string zipFile = Path.Combine(_backupTemp, $"Backup_{DateTime.Now:yyyyMMdd}.zip");

        // Create ZIP
        using (var archive = ZipFile.Open(zipFile, ZipArchiveMode.Create))
        {
            foreach (var file in files)
            {
                archive.CreateEntryFromFile(file, Path.GetFileName(file));
            }
        }

        // Upload ZIP via FTP
        UploadFileToFtp(zipFile);

        // Clean up
        File.Delete(zipFile);
    }

    private void UploadFileToFtp(string localFile)
    {
        string ftpFilePath = $"ftp://{_ftpHost}/sqlite_backup/{Path.GetFileName(localFile)}";

        var request = (FtpWebRequest)WebRequest.Create(ftpFilePath);
        request.Method = WebRequestMethods.Ftp.UploadFile;
        request.Credentials = new NetworkCredential(_ftpUser, _ftpPass);

        byte[] fileContents = File.ReadAllBytes(localFile);
        request.ContentLength = fileContents.Length;

        using (var requestStream = request.GetRequestStream())
        {
            requestStream.Write(fileContents, 0, fileContents.Length);
        }

        using (var response = (FtpWebResponse)request.GetResponse())
        {
            _logger.LogInformation($"Upload File Complete, status {response.StatusDescription}");
        }
    }
}

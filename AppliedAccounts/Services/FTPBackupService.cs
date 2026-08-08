using AppliedAccounts.Hubs;
using AppliedAccounts.Models;
using AppliedCrypto;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using System.IO.Compression;
using System.Net;

namespace AppliedAccounts.Services
{
    public class FTPBackupService : BackgroundService
    {
        private readonly ILogger<FTPBackupService> _logger;
        private readonly IOptions<BackupModel> _settings;
        private readonly IHubContext<LogHub> _hubContext;
        private readonly string _sourceFolder;
        private readonly string _tempFolder;
        private readonly int _hour;
        private readonly int _minute;
        private readonly string _encryptionPassphrase;

        public FTPBackupService(
            ILogger<FTPBackupService> logger,
            IOptions<BackupModel> settings,
            IHubContext<LogHub> hubContext)
        {
            _logger = logger;
            _settings = settings;
            _hubContext = hubContext;

            _sourceFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "SQLiteDB");
            _tempFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "SqliteTemp");
            _hour = settings.Value.ScheduleHour;
            _minute = settings.Value.ScheduleMinute;
            _encryptionPassphrase = "Applied";

            _logger.LogInformation($"Backup of Database files service start.");
            _logger.LogInformation($"Backup Domain {_settings.Value.FtpHost}");
            _logger.LogInformation($"Backup folder {_settings.Value.FtpRemotePath}");
        }

        private async Task SendLogAsync(string message, string level = "Info")
        {
            try
            {
                await _hubContext.Clients.All.SendAsync("ReceiveLog", message, level, DateTime.Now);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send log via SignalR");
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var now = DateTime.Now;
                    var nextRun = new DateTime(now.Year, now.Month, now.Day, _hour, _minute, 0);
                    if (nextRun <= now) nextRun = nextRun.AddDays(1);

                    var delay = nextRun - now;
                    await SendLogAsync($"Next backup scheduled at {nextRun:HH:mm:ss}", "Info");
                    _logger.LogInformation("Next backup scheduled at {NextRun}", nextRun);

                    await Task.Delay(delay, stoppingToken);

                    if (!stoppingToken.IsCancellationRequested)
                    {
                        await BackupDatabases();
                    }
                }
                catch (Exception ex)
                {
                    await SendLogAsync($"Error in backup service: {ex.Message}", "Error");
                    _logger.LogError(ex, "Error in FTP backup service");
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                }
            }
        }

        private async Task BackupDatabases()
        {
            try
            {
                await SendLogAsync("═══════════════════════════════════", "Info");
                await SendLogAsync("🔄 Starting database backup...", "Info");
                _logger.LogInformation("Starting database backup...");

                if (!Directory.Exists(_sourceFolder))
                {
                    await SendLogAsync($"❌ Database directory does not exist: {_sourceFolder}", "Error");
                    return;
                }

                var files = Directory.GetFiles(_sourceFolder, "*.db");
                if (files.Length == 0)
                {
                    await SendLogAsync($"⚠️ No database files found in {_sourceFolder}", "Warning");
                    return;
                }

                await SendLogAsync($"📁 Found {files.Length} database file(s) to backup", "Info");

                Directory.CreateDirectory(_tempFolder);

                var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                var zipFileName = $"Backup_{timestamp}.zip";
                var zipFilePath = Path.Combine(_tempFolder, zipFileName);

                var tempCopyDir = Path.Combine(_tempFolder, $"temp_{Guid.NewGuid():N}");
                Directory.CreateDirectory(tempCopyDir);

                try
                {
                    // Copy files with retry logic
                    int copiedCount = 0;
                    foreach (var file in files)
                    {
                        var fileName = Path.GetFileName(file);
                        var destFile = Path.Combine(tempCopyDir, fileName);
                        await CopyFileWithRetryAsync(file, destFile);
                        copiedCount++;
                        await SendLogAsync($"📄 Copied {fileName} ({copiedCount}/{files.Length})", "Info");
                        _logger.LogInformation("Copied {File} to temp location", fileName);
                    }

                    await SendLogAsync($"📦 Creating zip archive: {zipFileName}", "Info");
                    _logger.LogInformation("Creating zip archive: {ZipPath}", zipFilePath);

                    // Create zip with progress
                    using (var archive = ZipFile.Open(zipFilePath, ZipArchiveMode.Create))
                    {
                        var tempFiles = Directory.GetFiles(tempCopyDir, "*.db");
                        int addedCount = 0;
                        foreach (var file in tempFiles)
                        {
                            var entryName = Path.GetFileName(file);
                            archive.CreateEntryFromFile(file, entryName, CompressionLevel.Optimal);
                            addedCount++;
                            await SendLogAsync($"📦 Added {entryName} to zip ({addedCount}/{tempFiles.Length})", "Info");
                            _logger.LogInformation("Added {File} to zip", entryName);
                        }
                    }

                    var zipSize = new FileInfo(zipFilePath).Length;
                    await SendLogAsync($"✅ Zip file created: {zipFileName} ({zipSize / 1024:N0} KB)", "Success");

                    await SendLogAsync($"☁️ Uploading to FTP server...", "Info");
                    await UploadBackup(zipFilePath);

                    await SendLogAsync($"✅ Backup completed successfully! File: {zipFileName}", "Success");

                    // Cleanup local zip
                    File.Delete(zipFilePath);
                    await SendLogAsync($"🧹 Local zip file cleaned up", "Info");
                }
                finally
                {
                    if (Directory.Exists(tempCopyDir))
                    {
                        Directory.Delete(tempCopyDir, true);
                        await SendLogAsync($"🧹 Temporary directory cleaned up", "Info");
                    }
                }

                await SendLogAsync("═══════════════════════════════════", "Success");
                await SendLogAsync("✅ Backup process completed successfully", "Success");
                _logger.LogInformation("Database backup completed successfully.");
            }
            catch (Exception ex)
            {
                await SendLogAsync($"❌ Backup failed: {ex.Message}", "Error");
                _logger.LogError(ex, "Backup process failed");
                throw;
            }
        }

        private async Task CopyFileWithRetryAsync(string sourcePath, string destPath)
        {
            const int maxAttempts = 5;
            const int initialDelay = 200;

            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {
                try
                {
                    using var sourceStream = new FileStream(
                        sourcePath,
                        FileMode.Open,
                        FileAccess.Read,
                        FileShare.ReadWrite);
                    using var destStream = File.Create(destPath);
                    await sourceStream.CopyToAsync(destStream);
                    return;
                }
                catch (IOException) when (attempt < maxAttempts - 1)
                {
                    var delay = initialDelay * (int)Math.Pow(2, attempt);
                    var fileName = Path.GetFileName(sourcePath);
                    await SendLogAsync($"⏳ File {fileName} is locked, retrying in {delay}ms... (Attempt {attempt + 1}/{maxAttempts})", "Warning");
                    _logger.LogWarning("File {File} is locked, retrying in {Delay}ms (Attempt {Attempt}/{MaxAttempts})",
                        fileName, delay, attempt + 1, maxAttempts);
                    await Task.Delay(delay);
                }
            }
        }

        private async Task UploadBackup(string localFilePath)
        {
            try
            {
                var host = _settings.Value.FtpHost;
                var username = _settings.Value.FtpUsername;
                var encryptedPassword = _settings.Value.FtpPasswordHash;
                var remotePath = _settings.Value.FtpRemotePath.TrimEnd('/');
                var useSsl = _settings.Value.FtpUseSSL;

                if (string.IsNullOrEmpty(_encryptionPassphrase))
                {
                    throw new InvalidOperationException("Encryption passphrase is not configured");
                }

                string password;
                try
                {
                    password = CryptoHelper.MyDecrypt(encryptedPassword, _encryptionPassphrase);
                    await SendLogAsync($"🔐 Password decrypted successfully", "Info");
                }
                catch (Exception ex)
                {
                    await SendLogAsync($"❌ Failed to decrypt FTP password: {ex.Message}", "Error");
                    throw;
                }

                var remoteFileName = Path.GetFileName(localFilePath);

                var uriBuilder = new UriBuilder
                {
                    Scheme = "ftp",
                    Host = host,
                    Path = $"{remotePath}/{remoteFileName}".Replace('\\', '/')
                };

                var remoteUri = uriBuilder.Uri;

                await SendLogAsync($"☁️ Uploading to: {remoteUri}", "Info");
                _logger.LogInformation("Uploading {LocalFile} to {RemoteUri}", localFilePath, remoteUri);

                var request = (FtpWebRequest)WebRequest.Create(remoteUri);
                request.Method = WebRequestMethods.Ftp.UploadFile;
                request.Credentials = new NetworkCredential(username, password);
                request.UseBinary = true;
                request.UsePassive = true;
                request.EnableSsl = useSsl;
                request.KeepAlive = false;
                request.Timeout = 300000;

                if (useSsl)
                {
                    ServicePointManager.ServerCertificateValidationCallback +=
                        (sender, certificate, chain, sslPolicyErrors) => true;
                }

                // Upload with progress tracking
                var fileInfo = new FileInfo(localFilePath);
                long totalBytes = fileInfo.Length;
                long bytesUploaded = 0;

                using var fileStream = File.OpenRead(localFilePath);
                using var requestStream = await request.GetRequestStreamAsync();

                byte[] buffer = new byte[8192];
                int bytesRead;
                int lastProgress = 0;

                await SendLogAsync($"📤 Uploading {remoteFileName} ({totalBytes / 1024:N0} KB)...", "Info");

                while ((bytesRead = await fileStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    await requestStream.WriteAsync(buffer, 0, bytesRead);
                    bytesUploaded += bytesRead;

                    var progress = (int)((bytesUploaded * 100) / totalBytes);
                    if (progress % 10 == 0 && progress != lastProgress && progress > 0)
                    {
                        await SendLogAsync($"📤 Upload progress: {progress}%", "Info");
                        lastProgress = progress;
                    }
                }

                requestStream.Close();

                using var response = (FtpWebResponse)await request.GetResponseAsync();
                await SendLogAsync($"✅ Upload completed successfully! Status: {response.StatusDescription}", "Success");
                _logger.LogInformation("Upload completed. Status: {Status}", response.StatusDescription);
            }
            catch (WebException ex)
            {
                var ftpResponse = ex.Response as FtpWebResponse;
                if (ftpResponse != null)
                {
                    await SendLogAsync($"❌ FTP Error: {ftpResponse.StatusCode} - {ftpResponse.StatusDescription}", "Error");
                }
                else
                {
                    await SendLogAsync($"❌ FTP Error: {ex.Message}", "Error");
                }
                _logger.LogError(ex, "FTP upload failed for {LocalFile}", localFilePath);
                throw;
            }
            catch (Exception ex)
            {
                await SendLogAsync($"❌ Upload failed: {ex.Message}", "Error");
                _logger.LogError(ex, "FTP upload failed for {LocalFile}", localFilePath);
                throw;
            }
        }

        // Manual trigger for backup
        public async Task PerformBackupAsync()
        {
            await BackupDatabases();
        }
    }
}
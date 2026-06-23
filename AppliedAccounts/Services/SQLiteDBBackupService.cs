using Microsoft.Data.Sqlite;

namespace AppliedAccounts.Services
{
    public interface ISQLiteDBBackupService
    {
        Task<(byte[] Data, string FileName)> CreateBackupAsync(string SQLiteFile);
    }

    

    public class SQLiteDBBackupService : ISQLiteDBBackupService
    {
        private readonly IWebHostEnvironment _env;

        public SQLiteDBBackupService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<(byte[] Data, string FileName)> CreateBackupAsync(string SQLiteFile)
        {
            var dbPath = Path.Combine(_env.WebRootPath, "SQLiteDB", SQLiteFile);

            if (!File.Exists(dbPath))
            {
                throw new FileNotFoundException("Database not found");
            }

            var tempPath = Path.GetTempFileName();

            using (var source = new SqliteConnection($"Data Source={dbPath}"))
            using (var dest = new SqliteConnection($"Data Source={tempPath}"))
            {
                source.Open();
                dest.Open();
                source.BackupDatabase(dest);
            }

            var bytes = await File.ReadAllBytesAsync(tempPath);
            File.Delete(tempPath);

            var fileName = $"{SQLiteFile}_backup_{DateTime.Now:yyyyMMddHHmm}.db";
            
            return (bytes, fileName);           // send data to program.cs MapGet()
        }
    }
}
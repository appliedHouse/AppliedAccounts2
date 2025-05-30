namespace AppReports
{
    interface IOutputReport
    {
        public string FileFullName { get; }
        public string OutputFileName { get; }
        public string FileExtention { get; }
        public string FileLink { get; }
        public bool IsFileExist { get; }

    }

    public class OutputReport
    {
        public string ReportUrl { get; set; }       // like http://localhist:xxx/
        public string BasePath { get; set; } = Directory.GetCurrentDirectory();
        public string RootPath { get; set; } = "wwwroot";
        public string FilePath { get; set; } = string.Empty;            // file path after wwwroot
        public string FileName { get; set; } = string.Empty;            // file Name
        
        
        public ReportType ReportType { get; set; } = ReportType.Preview;
        public string MimeType { get; set; } = string.Empty;
        public FileStream FileStream { get; set; }
        
        public bool IsFileExist { get => File.Exists(FileFullName); }
        public string FileFullName { get => GetFullName(); }
        public string OutputFileName { get => GetFileName(); }
        public string FileExt { get => ReportExtention.Get(ReportType); }       // File Extention get dynamically
        public string FileLink { get => GetFileLink(); }                         // Create a lick to display file         

        #region Constructor
        public OutputReport() { }

        public OutputReport(string _FilePath, string _FileName)
        {
            FilePath = _FilePath;
            FileName = _FileName;
        }
        #endregion

        private string GetFullName()
        {
            var _Extention = ReportExtention.Get(ReportType); 

            if (FilePath.Length > 0 && FileName.Length > 0 && _Extention.Length > 0)
            {
                return Path.Combine(GetFileFolder(), FileName);

            }
            return string.Empty;
        }

        private string GetFileName()
        {
            var _Extention = ReportExtention.Get(ReportType);

            if (FilePath.Length > 0 && FileName.Length > 0 && _Extention.Length > 0)
            {
                return Path.Combine(FileName);

            }
            return string.Empty;
        }
        public string GetFileFolder()
        {
            var _FirstPath = Directory.GetCurrentDirectory();
            var _FileFolder = Path.Combine(_FirstPath, RootPath, FilePath);

            if (!Directory.Exists(_FileFolder))
            {
                Directory.CreateDirectory(_FileFolder);
            }
            return _FileFolder;
        }

        public string GetFileLink()
        {
            return $"{ReportUrl}/{FilePath}/{FileName}";
        }


        

    }
}


namespace AppReports
{
    class Save
    {
        public string FileFullName { get; set; }
        public byte[] ReportBytes { get; set; }
        public FileStream FileStream { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;

        public bool SaveReport()
        {

            var _FileName = FileFullName;
            if (_FileName.Length > 0)
            {
                if (File.Exists(_FileName))
                {
                    File.Delete(_FileName);
            
                }
                using (FileStream fstream = new FileStream(_FileName, FileMode.Create))
                {
                    fstream.Write(ReportBytes, 0, ReportBytes.Length);
                    FileStream = fstream;
                }
            }
            else
            {
                ErrorMessage = "File not save.";
            }

            return false;
        }
    }
}

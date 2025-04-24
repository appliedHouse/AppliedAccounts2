using Microsoft.AspNetCore.Components;
using Microsoft.Reporting.NETCore;
using System.Data;



namespace AppReports
{
    public class ReportModel
    {
        #region Variables
        public List<string> Messages { get; set; } = new();
        public InputReport InputReport { get; set; }
        public OutputReport OutputReport { get; set; }
        public ReportData ReportData { get; set; }
        public byte[] ReportBytes { get; set; }
        public bool IsReportRendered { get; set; } = false;
        public string ReportUrl { get; set; } = string.Empty;
        public string ReportPath { get; set; } = string.Empty;

        public List<ReportParameter> ReportParameters { get; set; }
        //public bool Render => ReportRender();
        private readonly string DateTimeFormat = "yyyy-MM-dd [hh:mm:ss]";
        private string DateTimeNow => DateTime.Now.ToString(DateTimeFormat);
        
        #endregion

        #region Constructor
        public ReportModel()
        {
            Messages = [];
            InputReport = new InputReport();
            OutputReport = new OutputReport();
            ReportData = new ReportData();
            ReportParameters = [];
            ReportBytes = [];

            Messages.Add($"{DateTimeNow}: Report Class Started.");
            ReportPath ??= "PDFReports";
            ReportUrl ??= "http://localhost/";

            InputReport.BasePath = Directory.GetCurrentDirectory();
            OutputReport.ReportUrl = ReportUrl;

        }


        #endregion

        #region Report Render
        public bool ReportRender(ReportType rptType)
        {
            
            OutputReport.ReportType = rptType;
            return ReportRender();
        }


        public bool ReportRender()
        {
            IsReportRendered = false;
            Messages.Add($"{DateTimeNow}: Report rendering started");
            Messages.Add($"{DateTimeNow}: Report Base URL {ReportUrl}");
            Messages.Add($"{DateTimeNow}: Report ReportPath {ReportPath}");

            if (ReportParameters.Count == 0) { DefaultParameters.GetDefaultParameters(); }
            if (InputReport.IsFileExist)
            {
                Messages.Add($"{DateTimeNow}: Report file found {InputReport.FileFullName}");

                var _ReportType = OutputReport.ReportType;
                Messages.Add($"{DateTimeNow}: Report Type is {OutputReport.ReportType}");

                OutputReport.MimeType = ReportMime.GetReportMime(_ReportType);
                Messages.Add($"{DateTimeNow}: Report MimeType is {OutputReport.MimeType}");

                //OutputReport.FileExtention = OutputReport.GetFileExtention(_ReportType);
                Messages.Add($"{DateTimeNow}: Report File Extention is {OutputReport.FileExtention}");

                //string lastPart = Path.GetFileName(Path.GetDirectoryName(OutputReport.FilePath)) ?? "OutputPath";
                //OutputReport.FileLink = GetFileLink();
                Messages.Add($"{DateTimeNow}: Report File Full Name is {OutputReport.FileFullName}");
                Messages.Add($"{DateTimeNow}: Report File download link is {OutputReport.FileLink}");

                var _ReportFile = InputReport.FileFullName;
                var _FileType = RenderFormat.GetRenderFormat(_ReportType);
                var _ReportStream = new StreamReader(_ReportFile);
                Messages.Add($"{DateTimeNow}: {_ReportFile} is read as stream.");

                LocalReport report = new();
                report.ReportPath = _ReportFile;
                report.ReportEmbeddedResource = report.ReportPath;

                report.LoadReportDefinition(_ReportStream);
                report.DataSources.Add(ReportData.DataSource);
                report.SetParameters(ReportParameters);

                ReportBytes = report.Render(_FileType);
                Messages.Add($"{DateTimeNow}: Report Render bytes are {ReportBytes.Count()}");

                if (OutputReport.ReportType != ReportType.Print)
                {

                    if (ReportBytes.Length > 0) { SaveReport(); }
                    else
                    {
                        Messages.Add($"{DateTimeNow}: ERROR: Report length is reporting zero");
                    }
                    
                }
                Messages.Add($"{DateTimeNow}: Report rendering completed at {DateTimeNow}");
                IsReportRendered = true;
            }
            else
            {
                Messages.Add($"{DateTimeNow}: Report file NOT found {InputReport.FileFullName}");
            }
            return IsReportRendered;
        }

        //private string GetFileLink()
        //{
        //    return OutputReport.FileFullName;
        //}

        public async Task<bool> ReportRenderAsync()
        {
            IsReportRendered = false;
            await Task.Run(() =>
            {
                IsReportRendered = ReportRender();    
            });
            return IsReportRendered;
        }
        #endregion

        #region Report Save / Stream 
        public bool SaveReport()
        {
            Messages.Add($"{DateTimeNow}: Report {ReportBytes.Length} btyes count.");
            Messages.Add($"{DateTimeNow}: Report saving start at {DateTimeNow}");

            var _FileName = OutputReport.FileFullName;
            if (_FileName.Length > 0)
            {
                if (File.Exists(_FileName))
                {
                    Messages.Add($"{DateTimeNow}: File {_FileName} already exist.");
                    File.Delete(_FileName);
                    Messages.Add($"{DateTimeNow}: File {_FileName} Deleted.");

                }
                using (FileStream fstream = new FileStream(_FileName, FileMode.Create))
                {
                    Messages.Add($"{DateTimeNow}: Report File streamed.");
                    fstream.Write(ReportBytes, 0, ReportBytes.Length);
                    OutputReport.FileStream = fstream;
                    Messages.Add($"{DateTimeNow}: Report saved sucessfully");
                    Messages.Add($"{DateTimeNow}: Created a file {_FileName}");

                    //ReportUrl = $"{OutputReport.FilePath}/{OutputReport.FileName}{OutputReport.FileExtention}";

                }
            }
            else
            {
                Messages.Add($"{DateTimeNow}: Report NOT saved.");
            }

            return false;
        }
        #endregion


        public void AddReportParameter(string Key, string Value)
        {
            var _Parameter = new ReportParameter(Key, Value);
            ReportParameters.Add(_Parameter);
            Messages.Add($"{DateTimeNow}: Report Parameter add {Key} => {Value}");
        }

    }

    public class InputReport
    {
        public string FilePath { get; set; } = string.Empty;            // Path after wwwroot
        public string FileName { get; set; } = string.Empty;            // File name
        public string FileExtention { get; set; } = string.Empty;       // Extention without dot
        public string BasePath { get; set; } = Directory.GetCurrentDirectory();
        public string RootPath { get; set; } = "wwwroot";

        public string FileFullName => GetFullName();
        public bool IsFileExist => GetFileExist();


        private bool GetFileExist()
        {
            if (File.Exists(FileFullName)) { return true; }
            { return false; }
        }

        private string GetFullName()
        {
            if (FilePath.Length > 0 && FileName.Length > 0 && FileExtention.Length > 0)
            {
                return Path.Combine(BasePath, RootPath, FilePath, FileName + "." + FileExtention);
            }
            return string.Empty;
        }
    }
    public class OutputReport
    {
        public string ReportUrl { get; set; }       // like http://localhist:xxx/
        public string BasePath { get; set; } = Directory.GetCurrentDirectory();
        public string RootPath { get; set; } = "wwwroot";
        public string FilePath { get; set; } = string.Empty;            // file path after wwwroot
        public string FileName { get; set; } = string.Empty;            // file Name
        public string FileExtention => GetFileExtention(ReportType);       // File Extention get dynamically
        public string FileLink => GetFileLink();                         // Create a lick to display file         
        public ReportType ReportType { get; set; } = ReportType.Preview;
        public string MimeType { get; set; } = string.Empty;
        public FileStream FileStream { get; set; }
        public bool IsFileExist => File.Exists(FileFullName);
        public string FileFullName => GetFullName();
        public string FileFolder => GetFileFolder();
        public string OutputFileName => GetFileName();

        private string GetFullName()
        {
            var _Extention = GetFileExtention(ReportType);

            if (FilePath.Length > 0 && FileName.Length > 0 && _Extention.Length > 0)
            {
                return Path.Combine(GetFileFolder(),FileName+FileExtention);
                
            }
            return string.Empty;
        }

        private string GetFileName()
        {
            var _Extention = GetFileExtention(ReportType);

            if (FilePath.Length > 0 && FileName.Length > 0 && _Extention.Length > 0)
            {
                return Path.Combine(FileName + FileExtention);

            }
            return string.Empty;
        }
        public string GetFileFolder()
        {
            var _FileFolder = Path.Combine(BasePath, RootPath, FilePath);

            if(!Directory.Exists(_FileFolder))
            {
                Directory.CreateDirectory(_FileFolder);
            }
            return _FileFolder;
        }

        public string GetFileLink()
        {
            return $"{ReportUrl}/{FilePath}/{FileName}{FileExtention}";
        }


        public static string GetFileExtention(ReportType _ReportType)
        {
            if (_ReportType == ReportType.Preview) { return ".pdf"; }
            if (_ReportType == ReportType.PDF) { return ".pdf"; }
            if (_ReportType == ReportType.HTML) { return ".html"; }
            if (_ReportType == ReportType.Word) { return ".docx"; }
            if (_ReportType == ReportType.Excel) { return ".xlsx"; }
            if (_ReportType == ReportType.Image) { return ".tiff"; }
            return "";
        }

    }
    public class ReportData
    {
        //public string SQLQuery { get; set; } = string.Empty;
        public DataTable ReportTable { get; set; } = new();
        public string DataSetName { get; set; } = string.Empty;
        public ReportDataSource DataSource => GetReportDataSource();

        private ReportDataSource GetReportDataSource()
        {
            if (DataSetName.Length > 0 && ReportTable != null)
            {
                return new ReportDataSource(DataSetName, ReportTable);
            }
            return new ReportDataSource();
        }
    }

}

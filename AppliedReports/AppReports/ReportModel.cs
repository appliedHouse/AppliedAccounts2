using Microsoft.Reporting.NETCore;
using System.Data;

namespace AppReports
{
    public class ReportModel
    {
        #region Variables
        public List<string> Messages { get; set; }
        public InputReport InputReport { get; set; }
        public OutputReport OutputReport { get; set; }
        public ReportData ReportDataSource { get; set; }
        public byte[] ReportBytes { get; set; }
        public bool IsReportRendered { get; set; } = false;
        public string ErrorMessage { get; set; } = string.Empty;
        public string ReportTitle { get; set; } = string.Empty;
        public ReportExtractor Extractor { get; set; }

        public List<ReportParameter> ReportParameters { get; set; }
        private string DateTimeNow => DateTime.Now.ToString("yyyy-MM-dd [hh:mm:ss]");

        #endregion

        #region Constructor
        public ReportModel()
        {

            InputReport = new InputReport();
            OutputReport = new OutputReport();
            ReportDataSource = new ReportData();

            Messages = [];
            ReportParameters = [];
            ReportBytes = [];

            Messages.Add($"{DateTimeNow}: Report Class Started.");

            InputReport.BasePath = Directory.GetCurrentDirectory();
            OutputReport.BasePath = Directory.GetCurrentDirectory();

            Messages.Add($"{DateTimeNow}: InputReport.BasePath {InputReport.BasePath}");
            Messages.Add($"{DateTimeNow}: OutputReport.BasePath {OutputReport.ReportUrl}");
        }


        #endregion

        #region Report Render

        public bool ReportRender(ReportType rptType)
        {
            Messages.Add($"{DateTimeNow}: Report rendering started");

            OutputReport.ReportType = rptType;
            IsReportRendered = false;

            try
            {
                Messages.Add($"{DateTimeNow} Input Base Path {InputReport.BasePath}");
                Messages.Add($"{DateTimeNow} Input Root Path {InputReport.RootPath}");
                Messages.Add($"{DateTimeNow} Input File Path {InputReport.FilePath}");
                Messages.Add($"{DateTimeNow} Input Full Name {InputReport.FileFullName}");
                Messages.Add($"{DateTimeNow} Input File Found {InputReport.IsFileExist}");

                Messages.Add($"{DateTimeNow}: Output Report URL {OutputReport.ReportUrl}");
                Messages.Add($"{DateTimeNow}: Output Report Path {OutputReport.FilePath}");
                Messages.Add($"{DateTimeNow}: Output File Full Name is {OutputReport.FileFullName}");
                Messages.Add($"{DateTimeNow}: Output File download link is {OutputReport.FileLink}");
                Messages.Add($"{DateTimeNow}: OutPut MimeType is {OutputReport.MimeType}");
                Messages.Add($"{DateTimeNow}: OutPut File Extention is {OutputReport.FileExt}");
                Messages.Add($"{DateTimeNow}: OutPut Report Type {OutputReport.ReportType}");

                Messages.Add($"{DateTimeNow}: Report DataSet Name is {ReportDataSource.DataSetName}");
                Messages.Add($"{DateTimeNow}: Report DataSet Name is {ReportDataSource.DataSource.Name}");

                Messages.Add($"{DateTimeNow}: Report Parameters Count {ReportParameters.Count}");
                foreach (var _Parameter in ReportParameters)
                {
                    Messages.Add($"{DateTimeNow}: Report Parameter {_Parameter.Name} => {_Parameter.Values[0]}");
                }



                if (ReportParameters.Count == 0)
                {
                    Messages.Add("Report Parameters found zero.");

                }
                if (InputReport.IsFileExist)
                {

                    Messages.Add($"{DateTimeNow}: Report Type is {OutputReport.ReportType}");

                    OutputReport.MimeType = ReportMime.Get(OutputReport.ReportType);
                    
                    var _ReportFile = InputReport.FileFullName;
                    var _DataSource = ReportDataSource.DataSource;
                    var _ReportStream = new StreamReader(_ReportFile);
                    var _Parameters = ReportParameters;
                    var _RenderFormat = RenderFormat.Get(OutputReport.ReportType);
                    Messages.Add($"{DateTimeNow}: {_ReportFile} is read as stream.");

                    // Report Generating.....
                    LocalReport report = new(); ;
                    //report.ReportEmbeddedResource = _ReportFile;
                    report.LoadReportDefinition(_ReportStream);
                    report.DataSources.Add(_DataSource);
                    report.DisplayName = OutputReport.FileName;
                    if (IsParametersValid())
                    {
                        report.SetParameters(_Parameters);
                        report.Refresh();

                        // Report Rendering ....
                        ReportBytes = report.Render(_RenderFormat) ?? [];
                        Messages.Add($"{DateTimeNow}: Report Render bytes are {ReportBytes.Length}");
                        Messages.Add($"{DateTimeNow}: Report rendering completed at {DateTimeNow}");
                        IsReportRendered = true;
                    }
                    else
                    {
                        if(!ReportParameters.Equals(Extractor.MyParameters.Count))
                        {
                            Messages.Add($"{DateTimeNow}: Parameters {ReportParameters.Count} != {Extractor.MyParameters.Count} ");
                        }

                        Messages.Add($"{DateTimeNow}: Report {ReportParameters.Count} Parameters are not valid");
                    }


                }
                else
                {
                    ErrorMessage = $"{DateTimeNow}: Report file NOT found {InputReport.FileFullName}";
                    Messages.Add(ErrorMessage);
                }

            }
            catch (Exception error)
            {
                ErrorMessage = error.Message;
                IsReportRendered = false;

            }
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
                }
            }
            else
            {
                Messages.Add($"{DateTimeNow}: Report NOT saved.");
            }

            return false;
        }
        #endregion


        #region Add Parameters
        public void AddReportParameter(string Key, string Value)
        {
            if (InputReport.IsFileExist)
            {
                Extractor ??= new(InputReport.FileFullName);
                var _Parameter = new ReportParameter(Key, Value);
                if (Extractor.IsExistParameter(Key))
                {
                    ReportParameters.Add(_Parameter);
                    Messages.Add($"{DateTimeNow}: Report Parameter add {Key} => {Value}");
                }
                else
                {
                    Messages.Add($"{DateTimeNow}: Report Parameter {Key} => {Value} is not part of RDL report ");
                }
            }
        }

        public bool IsParametersValid()
        {
            // Check all the variable are generated for the report, if not match send false;

            if (Extractor.MyParameters.Count != ReportParameters.Count)
                return false;

            return Extractor.MyParameters.All(myParam => ReportParameters.Any(reportParam =>
            string.Equals(myParam.Name, reportParam.Name, StringComparison.OrdinalIgnoreCase)));
        }



        #endregion
    }



}

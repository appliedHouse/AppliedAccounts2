namespace AppReports
{
    class ReportExtention
    {
        public static string Get(ReportType _ReportType)
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
}

namespace AppReports
{
    public static class RenderFormat
    {
        public static string GetRenderFormat(ReportType _ReportType)
        {
            if (_ReportType == ReportType.Preview) { return "PDF"; }
            if (_ReportType == ReportType.PDF) { return "PDF"; }
            if (_ReportType == ReportType.HTML) { return "HTML5"; }
            if (_ReportType == ReportType.Word) { return "WORDOPENXML"; }
            if (_ReportType == ReportType.Excel) { return "EXCELOPENXML"; }
            if (_ReportType == ReportType.Image) { return "IMAGE"; }
            return "";
        }
    }
}

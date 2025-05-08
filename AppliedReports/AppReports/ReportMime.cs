namespace AppReports
{
    public static class ReportMime
    {
        public static string Get(ReportType reportType) => reportType switch
        {
            ReportType.PDF => "application/pdf",
            ReportType.HTML => "text/html",
            ReportType.Word => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            ReportType.Excel => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            ReportType.Image => "image/tiff",
            _ => "application/pdf"
        };


    }
}

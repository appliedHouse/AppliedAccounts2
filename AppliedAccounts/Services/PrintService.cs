using AppReports;

namespace AppliedAccounts.Services
{
    public class PrintService
    {
        public ReportParameters rptParameters { get; set; }
        public ExportReport rptExport { get; set; }


        public PrintService()
        {

        }
        public PrintService(ReportParameters rptParameters)
        {
            this.rptParameters = rptParameters;
            rptExport = new(rptParameters);

        }

        public void Print()
        {

        }


        public void Preview()
        {

        }

        public void Export(ReportType rptType)
        {

        }

        public enum DownloadOption
        {
            displayPDF,
            downloadFile
        }
    }
}

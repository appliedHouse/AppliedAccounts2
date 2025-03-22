using AppReports;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using AppliedAccounts.Data;

namespace AppliedAccounts.Services
{
    public class PrintService
    {
        public ReportData RptData { get; set; }
        public ReportModel RptModel { get; set; }
        public ReportType RptType { get; set; }
        public string RptUrl { get; set; }
        public string JSOption { get; set; }


        public PrintService()
        {

        }


        public void Preview()
        {
            //js.InvokeVoidAsync("displayPDF", RptModel.OutputReport.FileFullName);

        }

        public void Export(ReportType rptType)
        {

        }

        private string RenderReport()
        {
            RptModel.ReportData = RptData;          // Set Report Data to print in report.

            if (RptModel.ReportData != null)
            {
                if (RptModel.ReportRender())
                {
                    if (RptType == ReportType.Preview)
                    { JSOption = DownloadOption.displayPDF.ToString(); }
                    else
                    { JSOption = DownloadOption.downloadFile.ToString(); }
                    return JSOption;
                }
            }
            return "";
        }

        internal string GetReportLink()
        {
            RptModel.OutputReport.ReportType = RptType;
            JSOption = RenderReport();
            RptUrl = string.Concat(RptModel.OutputReport.FileLink);
            return RptUrl;
        }

        public enum DownloadOption
        {
            displayPDF,
            downloadFile,
        }
    }
}

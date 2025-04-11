using AppReports;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace AppliedAccounts.Services
{
    public class PrintService
    {
        public ReportData RptData { get; set; }
        public ReportModel RptModel { get; set; }
        public ReportType RptType { get; set; }
        public string RptUrl { get; set; }
        public string JSOption { get; set; }
        public IJSRuntime JS { get; set; }
        public NavigationManager NavManager { get; set; }
        public string DownLoadPath = "PDFReports";

        public PrintService()
        {
        }

        public byte[] Generate()
        {
            RptModel.ReportData = RptData;          // Set Report Data to print in report.
            if (RptModel.ReportData != null)
            {
                if (RptModel.ReportRender())
                {
                    // In the Process of ReportRender, ReportBytes are generated.
                    return RptModel.ReportBytes;
                }
            }
            return [];
        }


        private string RenderReport()
        {
            RptModel.ReportData = RptData;          // Set Report Data to print in report.

            if (RptModel.ReportData != null)
            {
                if (RptModel.ReportRender())
                {
                    if (RptType == ReportType.Preview)
                    { JSOption = "displayPDF"; }
                    else
                    { JSOption = "downloadFile"; }
                    return JSOption;
                }
            }
            return "";
        }

        public string GetReportLink()
        {
            RptModel.OutputReport.ReportType = RptType;
            JSOption = RenderReport();
            RptUrl = string.Concat(RptModel.OutputReport.FileLink);
            return RptUrl;
        }


        public void Preview()
        {
            RptModel.ReportRender(ReportType.Preview);
            JS.InvokeVoidAsync("DisplayPDF", RptModel.ReportBytes);
        }

        internal void PDF()
        {
            RptModel.ReportRender(ReportType.PDF);
            JS.InvokeVoidAsync("downloadPDF", RptModel.OutputReport.FileName, RptModel.ReportBytes);
        }

        internal void Excel()
        {
            RptModel.ReportRender(ReportType.Excel);

            var FileLink = $"{NavManager.BaseUri}{DownLoadPath}/{RptModel.OutputReport.FileName}{RptModel.OutputReport.FileExtention}";
            NavManager.NavigateTo(FileLink, forceLoad: true);
        }

        internal void Word()
        {
            RptModel.ReportRender(ReportType.Word);

            var FileLink = $"{NavManager.BaseUri}{DownLoadPath}/{RptModel.OutputReport.FileName}{RptModel.OutputReport.FileExtention}";
            NavManager.NavigateTo(FileLink, forceLoad: true);
        }

        internal void Image()
        {
            RptModel.ReportRender(ReportType.Image);
            var FileLink = $"{NavManager.BaseUri}{DownLoadPath}/{RptModel.OutputReport.FileName}{RptModel.OutputReport.FileExtention}";
            NavManager.NavigateTo(FileLink, forceLoad: true);
            //JS.InvokeVoidAsync("open", FileLink, "_blank");
        }

        internal void HTML()
        {
            RptModel.ReportRender(ReportType.HTML);
            var FileLink = $"{NavManager.BaseUri}{DownLoadPath}/{RptModel.OutputReport.FileName}{RptModel.OutputReport.FileExtention}";
            JS.InvokeVoidAsync("open", FileLink, "_blank");
        }
    }
}

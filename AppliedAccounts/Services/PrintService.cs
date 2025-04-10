using AppliedAccounts.Pages.Testing;
using AppReports;
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

        public PrintService()
        {
        }


        public void Preview()
        {
            
            RptModel.ReportRender(ReportType.Preview);
            JS.InvokeVoidAsync("DisplayPDF", RptModel.ReportBytes);
        }


        public void Export(ReportType rptType)
        {
            switch (rptType)    
            {
                case ReportType.Print:
                    break;
                case ReportType.Preview:
                    JS.InvokeVoidAsync("displayPDF", RptModel.OutputReport.FileFullName);
                    break;
                case ReportType.PDF:
                    break;
                case ReportType.Excel:
                    break;
                case ReportType.Word:
                    break;
                case ReportType.Image:
                    break;
                case ReportType.HTML:
                    break;
                default:
                    break;
            }

            
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
                    { JSOption = DownloadOption.displayPDF.ToString(); }
                    else
                    { JSOption = DownloadOption.downloadFile.ToString(); }
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

        internal void PDF()
        {
            throw new NotImplementedException();
        }

        internal void Excel()
        {
            throw new NotImplementedException();
        }

        internal void Word()
        {
            throw new NotImplementedException();
        }

        internal void Image()
        {
            throw new NotImplementedException();
        }

        internal void HTML()
        {
            throw new NotImplementedException();
        }

        public enum DownloadOption
        {
            displayPDF,
            downloadFile,
        }
    }
}

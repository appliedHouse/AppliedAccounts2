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
        public GlobalService Config { get; set; }
        public NavigationManager NavManager { get; set; }
        
        public string ReportUrl { get; set; } = string.Empty;

        public PrintService(GlobalService _Config)
        {
            Config = _Config;
            NavManager = Config.NavManager;
            JS = Config.JS;
            ReportUrl = $"{Config.AppPaths.BaseUri}{Config.AppPaths.PDFPath}";
        }

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

        public async Task Print()
        {
            RptModel.ReportData = RptData;
            RptModel.ReportRender(ReportType.Print);
            string rptBytes64 = Convert.ToBase64String(RptModel.ReportBytes);
            await JS.InvokeVoidAsync("printer", rptBytes64);
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

            var FileLink = $"{ReportUrl}/{RptModel.OutputReport.FileName}{RptModel.OutputReport.FileExtention}";
            NavManager.NavigateTo(FileLink, forceLoad: true);
        }

        internal void Word()
        {
            RptModel.ReportRender(ReportType.Word);

            var FileLink = $"{ReportUrl}/{RptModel.OutputReport.FileName}{RptModel.OutputReport.FileExtention}";
            NavManager.NavigateTo(FileLink, forceLoad: true);
        }

        internal void Image()
        {
            RptModel.ReportRender(ReportType.Image);
            var FileLink = $"{ReportUrl}/{RptModel.OutputReport.FileName}{RptModel.OutputReport.FileExtention}";
            NavManager.NavigateTo(FileLink, forceLoad: true);
            //JS.InvokeVoidAsync("open", FileLink, "_blank");
        }

        internal void HTML()
        {
            RptModel.ReportRender(ReportType.HTML);
            var FileLink = $"{ReportUrl}/{RptModel.OutputReport.FileName}{RptModel.OutputReport.FileExtention}";
            JS.InvokeVoidAsync("open", FileLink, "_blank");
        }
    }
}

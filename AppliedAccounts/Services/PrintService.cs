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
        public string MyMessage { get; set; }
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

        #region Temp
        //public byte[] Generate()
        //{

        //    RptModel.ReportData = RptData;          // Set Report Data to print in report.
        //    if (RptModel.ReportData != null)
        //    {
        //        if (RptModel.ReportRender())
        //        {
        //            // In the Process of ReportRender, ReportBytes are generated.
        //            return RptModel.ReportBytes;
        //        }
        //    }
        //    return [];
        //}


        //private string RenderReport()
        //{
        //    RptModel.ReportData = RptData;          // Set Report Data to print in report.

        //    if (RptModel.ReportData != null)
        //    {
        //        if (RptModel.ReportRender())
        //        {
        //            if (RptType == ReportType.Preview)
        //            { JSOption = "displayPDF"; }
        //            else
        //            { JSOption = "downloadFile"; }
        //            return JSOption;
        //        }
        //    }
        //    return "";
        //}

        //public string GetReportLink()
        //{
        //    RptModel.OutputReport.ReportType = RptType;
        //    JSOption = RenderReport();
        //    RptUrl = string.Concat(RptModel.OutputReport.FileLink);
        //    return RptUrl;
        //}
        #endregion
        public async void Print()
        {
            switch (RptType)
            {
                case ReportType.Print: await Printer(); break;
                case ReportType.Preview: await Preview(); break;
                case ReportType.PDF: await PDF(); break;
                case ReportType.Excel: await Excel(); break;
                case ReportType.Word: await Word(); break;
                case ReportType.Image: await Image(); break;
                case ReportType.HTML: await HTML(); break;
                default: await Preview(); break;
            }
        }



        #region Option (Type) of Printing Of reports. Print,Preview,PDF, Excel.... 
        public async Task Printer()
        {
            RptModel.PrintData = RptData;
            bool IsRendered = RptModel.ReportRender(ReportType.Print);
            if (IsRendered)
            {
                string rptBytes64 = Convert.ToBase64String(RptModel.ReportBytes);
                await JS.InvokeVoidAsync("printer", rptBytes64);
            }
            else
            {
                MyMessage = "Report has some error";
            }

        }
        public async Task Preview()
        {
            if(RptModel.ReportRender(ReportType.Preview))
            {
                await JS.InvokeVoidAsync("DisplayPDF", RptModel.ReportBytes);
            }
            else
            {
                MyMessage = RptModel.Messages.Last();
            }
        }
        public async Task PDF()
        {
            if (RptModel.ReportRender(ReportType.PDF))
            {
                await JS.InvokeVoidAsync("downloadFile", 
                      RptModel.OutputReport.FileName, 
                      RptModel.ReportBytes,
                      RptModel.OutputReport.MimeType);
            }
            else
            {
                MyMessage = RptModel.Messages.Last();
            }
        }
        public async Task Excel()
        {
            if (RptModel.ReportRender(ReportType.Excel))
            {
                await JS.InvokeVoidAsync("downloadFile",
                      RptModel.OutputReport.FileName,
                      RptModel.ReportBytes,
                      RptModel.OutputReport.MimeType);
            }
            else
            {
                MyMessage = RptModel.Messages.Last();
            }
        }
        public async Task Word()
        {
            if (RptModel.ReportRender(ReportType.Word))
            {
                await JS.InvokeVoidAsync("downloadFile",
                      RptModel.OutputReport.FileName,
                      RptModel.ReportBytes,
                      RptModel.OutputReport.MimeType);
            }
            else
            {
                MyMessage = RptModel.Messages.Last();
            }
        }
        public async Task Image()
        {
            if (RptModel.ReportRender(ReportType.Image))
            {
                await JS.InvokeVoidAsync("downloadFile",
                      RptModel.OutputReport.FileName,
                      RptModel.ReportBytes,
                      RptModel.OutputReport.MimeType);
            }
            else
            {
                MyMessage = RptModel.Messages.Last();
            }


            //var FileLink = $"{ReportUrl}/{RptModel.OutputReport.FileName}{RptModel.OutputReport.FileExtention}";
            //NavManager.NavigateTo(FileLink, forceLoad: true);
            //JS.InvokeVoidAsync("open", FileLink, "_blank");
        }
        public async Task HTML()
        {
            if (RptModel.ReportRender(ReportType.HTML))
            {
                await JS.InvokeVoidAsync("downloadFile",
                      RptModel.OutputReport.FileName,
                      RptModel.ReportBytes,
                      RptModel.OutputReport.MimeType);
            }
            else
            {
                MyMessage = RptModel.Messages.Last();
            }

            //var FileLink = $"{ReportUrl}/{RptModel.OutputReport.FileName}{RptModel.OutputReport.FileExtention}";
            //JS.InvokeVoidAsync("open", FileLink, "_blank");
        }
        #endregion
    }
}

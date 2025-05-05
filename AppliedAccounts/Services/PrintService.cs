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
        public bool IsError { get; set; } = false;

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

        #region Print a Report
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

        public void Print(ReportType reportType)
        {
            RptType = reportType;
            Print();
        }
        #endregion

        #region Option (Type) of Printing Of reports. Print,Preview,PDF, Excel.... 
        public async Task Printer()
        {
            try
            {
                RptModel.ReportDataSource = RptData;
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
            catch (Exception error)
            {

                IsError = true;
                MyMessage = error.Message;
            }


        }
        public async Task Preview()
        {
            try
            {
                if (RptModel.ReportRender(ReportType.Preview))
                {
                    await JS.InvokeVoidAsync("DisplayPDF", RptModel.ReportBytes);
                }
                else
                {
                    MyMessage = RptModel.Messages.Last();
                }
            }
            catch (Exception error)
            {

                IsError = true;
                MyMessage = error.Message;
            }
        }
        public async Task PDF()
        {
            try
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
            catch (Exception error)
            {

                IsError = true;
                MyMessage = error.Message;
            }
        }
        public async Task Excel()
        {
            try
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
            catch (Exception error)
            {

                IsError = true;
                MyMessage = error.Message;
            }
        }
        public async Task Word()
        {
            try
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
            catch (Exception error)
            {

                IsError = true;
                MyMessage = error.Message;
            }
        }
        public async Task Image()
        {

            if (RptModel.ReportRender(ReportType.Image))
            {
                await JS.InvokeVoidAsync("DisplayFile",
                    RptModel.ReportBytes,
                    RptModel.OutputReport.MimeType);
            }
            else
            {
                MyMessage = RptModel.Messages.Last();
            }

        }
        public async Task HTML()
        {
            try
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
            }
            catch (Exception error)
            {

                IsError = true;
                MyMessage = error.Message;
            }

        }
        #endregion
    }
}

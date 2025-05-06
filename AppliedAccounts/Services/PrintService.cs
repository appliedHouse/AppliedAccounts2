using AppliedDB;
using AppMessages;
using AppReports;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Microsoft.Reporting.NETCore;

namespace AppliedAccounts.Services
{
    public class PrintService
    {
        public IJSRuntime JS { get; set; }
        public GlobalService Config { get; set; }
        public NavigationManager NavManager { get; set; }
        public AppUserModel? UserProfile { get; set; }


        public ReportData Data { get; set; }
        public ReportModel Model { get; set; }
        public ReportType ReportType { get; set; }
        
        
        public MessageClass MsgClass { get; set; }
        public bool IsError { get; set; } = false;
        public string MyMessage { get; set; }

        public PrintService(GlobalService _Config)
        {
            Config = _Config;
            NavManager = Config.NavManager;
            JS = Config.JS;

            Model = new();

            Model.InputReport.RootPath = Config.AppPaths.RootPath;
            Model.InputReport.FilePath = Config.AppPaths.ReportPath;
            
            Model.OutputReport.BasePath = NavManager.BaseUri;
            Model.OutputReport.RootPath = Config.AppPaths.RootPath;
            Model.OutputReport.FilePath = Config.AppPaths.PDFPath;

            Model.ReportParameters =
            [
                new ReportParameter("CompanyName", Config.Reporting.ReportTitle ),
                new ReportParameter("Footer", Config.Reporting.ReportFooter)
            ];
        }

        public PrintService()
        {
        }

        #region Print a Report
        public async void Print()
        {

            switch (Model.OutputReport.ReportType)
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
            Model.OutputReport.ReportType = reportType;
            Print();
        }
        #endregion

        #region Option (Type) of Printing Of reports. Print,Preview,PDF, Excel.... 
        public async Task Printer()
        {
            try
            {
                Model.ReportDataSource = Data;
                bool IsRendered = Model.ReportRender(ReportType.Print);
                if (IsRendered)
                {
                    string rptBytes64 = Convert.ToBase64String(Model.ReportBytes);
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

            if(Model.ErrorMessage.Length > 0)
            {
                MsgClass.Danger(Model.ErrorMessage);
            }


        }
        public async Task Preview()
        {
            try
            {
                if (Model.ReportRender(ReportType.Preview))
                {
                    await JS.InvokeVoidAsync("DisplayPDF", Model.ReportBytes);
                }
                else
                {
                    MyMessage = Model.Messages.Last();
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
                if (Model.ReportRender(ReportType.PDF))
                {
                    await JS.InvokeVoidAsync("downloadFile",
                          Model.OutputReport.FileName,
                          Model.ReportBytes,
                          Model.OutputReport.MimeType);
                }
                else
                {
                    MyMessage = Model.Messages.Last();
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
                if (Model.ReportRender(ReportType.Excel))
                {
                    await JS.InvokeVoidAsync("downloadFile",
                          Model.OutputReport.FileName,
                          Model.ReportBytes,
                          Model.OutputReport.MimeType);
                }
                else
                {
                    MyMessage = Model.Messages.Last();
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
                if (Model.ReportRender(ReportType.Word))
                {
                    await JS.InvokeVoidAsync("downloadFile",
                          Model.OutputReport.FileName,
                          Model.ReportBytes,
                          Model.OutputReport.MimeType);
                }
                else
                {
                    MyMessage = Model.Messages.Last();
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

            if (Model.ReportRender(ReportType.Image))
            {
                await JS.InvokeVoidAsync("DisplayFile",
                    Model.ReportBytes,
                    Model.OutputReport.MimeType);
            }
            else
            {
                MyMessage = Model.Messages.Last();
            }

        }
        public async Task HTML()
        {
            try
            {
                if (Model.ReportRender(ReportType.HTML))
                {
                    await JS.InvokeVoidAsync("downloadFile",
                          Model.OutputReport.FileName,
                          Model.ReportBytes,
                          Model.OutputReport.MimeType);
                }
                else
                {
                    MyMessage = Model.Messages.Last();
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

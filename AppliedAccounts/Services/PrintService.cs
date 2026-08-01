using AppReports;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Microsoft.Reporting.NETCore;

namespace AppliedAccounts.Services
{
    public class PrintService
    {
        public IJSRuntime JS { get; set; }
        public GlobalService AppGlobals { get; set; }
        public NavigationManager NavManager { get; set; }
        public AppliedGlobals.AppUserModel? UserProfile { get; set; }

        public ReportData Data { get; set; }
        public ReportModel Model { get; set; }
        public ReportType ReportType { get; set; }
        public ReportExtractor Extractor { get; set; }

        public bool IsError { get; set; } = false;
        public List<string> MyMessage { get; set; } = new();
        public MessagesService MsgService { get; set; }


        public PrintService(GlobalService appGlobals)
        {
            if (appGlobals is not null)
            {

                AppGlobals = appGlobals;
                NavManager = AppGlobals.NavManager;
                JS = AppGlobals.JS;
                MsgService = AppGlobals.MsgService;

                Data = new();
                Model = new();

                Model.InputReport.RootPath = AppGlobals.AppPaths.RootPath;
                Model.InputReport.FilePath = AppGlobals.AppPaths.ReportPath;

                Model.OutputReport.BasePath = NavManager.BaseUri;
                Model.OutputReport.RootPath = AppGlobals.AppPaths.RootPath;
                Model.OutputReport.FilePath = AppGlobals.AppPaths.PDFPath;

                AppGlobals.Reporting.ReportTitle = AppGlobals.Client.DisplayName;

                if (string.IsNullOrEmpty(AppGlobals.Reporting.ReportTitle)) { AppGlobals.Reporting.ReportTitle = "APPLIED SOFTWARE HOUSE"; }
                if (string.IsNullOrEmpty(AppGlobals.Reporting.ReportFooter)) { AppGlobals.Reporting.ReportFooter = "APPLIED ACCOUNTS"; }


                Model.ReportParameters =
                [
                    new ReportParameter("CompanyName", AppGlobals.Reporting.ReportTitle ),
                    new ReportParameter("Footer", AppGlobals.Reporting.ReportFooter)
                ];
            }
        }

        public PrintService()
        {
        }

        #region Print a Report

        public async Task<bool> PrintAsync()
        {

            Model.OutputReport.ReportType = ReportType;

            if (Data == null && Model.ReportDataSource != null)
            {
                Data = Model.ReportDataSource;
            }
            else if (Data != null && Model.ReportDataSource == null)
            {
                Model.ReportDataSource = Data;
            }

            //Model.ReportDataSource = Data; // Set the data source for the report

            IsError = ReportValidate();

            if (!IsError)
            {
                switch (ReportType)
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
                return true;
            }
            return false;
        }

        public void Print()
        {
            try
            {
                Task.Run(async () =>
                {
                    var _result = await PrintAsync();  
                });
            }
            catch (Exception ex) // ← Catches the actual exception
            {
                MsgService.Error($"Error: {ex.Message}");
                Console.WriteLine($"Caught: {ex.Message}");
            }
        }

        #endregion

        #region Report Validation
        public bool ReportValidate()
        {
            bool result = false;
            Extractor = new(Model.InputReport.FileFullName);
            MsgService.Clear();

            if (!Model.IsParametersValid())
            {
                result = true;
                MyMessage.Add("The report parameters are not aligned with the report requirements.");
                MsgService.Critical(MyMessage.Last());
            }


            if (Data != null)
            {
                if (Data.DataSetName != Extractor.DataSetName)
                {
                    result = true;
                    MyMessage.Add("The report Dataset name is not aligned with the report requirements.");
                    MsgService.Critical(MyMessage.Last());
                }
            }
            else
            {
                result = true;
                MyMessage.Add("The report Dataset name is not assign. Value is null");
                MsgService.Critical(MyMessage.Last());

            }

            return result;
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
                    MyMessage.Add(Model.ErrorMessage);
                }
            }
            catch (Exception error)
            {

                IsError = true;
                MyMessage.Add(error.Message);
            }

            if (Model.ErrorMessage.Length > 0)
            {
                MyMessage.Add(Model.ErrorMessage);
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
                    MyMessage.Add(Model.ErrorMessage);
                }
            }
            catch (Exception error)
            {

                IsError = true;
                MyMessage.Add(error.Message);
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
                    MyMessage.Add(Model.ErrorMessage);
                }
            }
            catch (Exception error)
            {

                IsError = true;
                MyMessage.Add(error.Message);
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
                    MyMessage.Add(Model.ErrorMessage);
                }
            }
            catch (Exception error)
            {

                IsError = true;
                MyMessage.Add(error.Message);
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
                    MyMessage.Add(Model.ErrorMessage);
                }
            }
            catch (Exception error)
            {

                IsError = true;
                MyMessage.Add(error.Message);
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
                MyMessage.Add(Model.ErrorMessage);
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
                    MyMessage.Add(Model.ErrorMessage);
                }
            }
            catch (Exception error)
            {

                IsError = true;
                MyMessage.Add(error.Message);
            }

        }
        #endregion
    }
}

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
        public AppliedGlobals.AppUserModel? UserProfile { get; set; }

        public ReportData Data { get; set; }
        public ReportModel Model { get; set; }
        public ReportType ReportType { get; set; }
        public ReportExtractor Extractor { get; set; }

        public bool IsError { get; set; } = false;
        public List<string> MyMessage { get; set; } = new();


        public PrintService(GlobalService _Config)
        {
            if (_Config is not null)
            {

                Config = _Config;
                NavManager = Config.NavManager;
                JS = Config.JS;

                Data = new();
                Model = new();

                Model.InputReport.RootPath = Config.AppPaths.RootPath;
                Model.InputReport.FilePath = Config.AppPaths.ReportPath;

                Model.OutputReport.BasePath = NavManager.BaseUri;
                Model.OutputReport.RootPath = Config.AppPaths.RootPath;
                Model.OutputReport.FilePath = Config.AppPaths.PDFPath;

                if (string.IsNullOrEmpty(Config.Reporting.ReportTitle)) { Config.Reporting.ReportTitle = "APPLIED SOFTWARE HOUSE"; }
                if (string.IsNullOrEmpty(Config.Reporting.ReportFooter)) { Config.Reporting.ReportFooter = "APPLIED ACCOUNTS"; }


                Model.ReportParameters =
                [
                    new ReportParameter("CompanyName", Config.Reporting.ReportTitle ),
                new ReportParameter("Footer", Config.Reporting.ReportFooter)
                ];
            }
        }

        public PrintService()
        {
        }

        #region Print a Report

        public async Task PrintAsync()
        {
            IsError = ReportValidate();

            Model.OutputReport.ReportType = ReportType;
            Model.ReportDataSource = Data; // Set the data source for the report

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
            }

        }

        public async void Print()
        {
            IsError = ReportValidate();

            Model.OutputReport.ReportType = ReportType;
            Model.ReportDataSource = Data; // Set the data source for the report

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
            }
        }

        #endregion

        #region Report Validation
        public bool ReportValidate()
        {
            bool result = false;
            Extractor = new(Model.InputReport.FileFullName);

            if (!Model.IsParametersValid())
            {
                result = true;
                MyMessage.Add("Report Parameters are not equal with report.");
            }

            if (Data.DataSetName != Extractor.DataSetName)
            {
                result = true;
                MyMessage.Add("Report Dataset Name is not matched with report.");
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

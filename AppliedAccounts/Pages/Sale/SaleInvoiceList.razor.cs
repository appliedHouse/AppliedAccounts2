using AppliedAccounts.Data;
using AppliedAccounts.Services;
using AppliedDB;
using AppReports;
using BlazorJS;
using Microsoft.JSInterop;
using System.Data;

//using static AppliedAccounts.Services.PrintService;
using MESSAGE = AppMessages.Enums.Messages;


namespace AppliedAccounts.Pages.Sale
{
    public partial class SaleInvoiceList
    {

        public AppUserModel AppUser { get; set; }
        public Models.SaleInvoiceListModel Model { get; set; }
        public ReportModel PrintClass { get; set; }
        private bool IsPrinted { get; set; } = false;
        private bool IsPrinting { get; set; } = false;
        private List<string> PrintedReports { get; set; } = new();
        public PrintService ReportService { get; set; }
        public Globals MyGlobals { get; set; }
        public string PrintingMessage { get; set; }

        public SaleInvoiceList()
        {

        }


        #region Delete
        public async void Delete(int ID)
        {

            await Task.Delay(1000);
            // Add code here to delete sales invocies.
        }
        #endregion
        #region Edit
        public void Edit(int ID)
        {
            NavManager.NavigateTo($"/Sale/SaleInvoice/{ID}");
        }
        #endregion
        #region Select All and Select One
        public void SelectAll()
        {
            Model.SelectAll = !Model.SelectAll;
            Model.Records?.ForEach(item => item.IsSelected = Model.SelectAll);
            //StateHasChanged();
        }

        public void SelectOne(int _ID)
        {
            var item = Model.Records.Where(a => a.Id == _ID).First();
            item.IsSelected = !item.IsSelected;
            //foreach (var item in Model.Records)
            //{
            //    if(item.Id == _ID)
            //    {
            //        item.IsSelected = !item.IsSelected;
            //    }
            //}
        }
        #endregion

        #region Sales Invoice report print -- Print -- Print All - 

        public async void PrintAll()
        {
            IsPrinting = true;
            await InvokeAsync(StateHasChanged);
            PrintingMessage = "Priting Start.";

            foreach (var item in Model.Records)
            {
                if (item.IsSelected)
                {
                    PrintingMessage = $"Sales invoice for {Model.Record.TitleCustomer} is being printed.";
                    await InvokeAsync(StateHasChanged);
                    await Print(item.Id);
                }
            }
            IsPrinting = false;
            await InvokeAsync(StateHasChanged);
        }

        public async Task Print(int ID)
        {
            //await InvokeAsync(StateHasChanged);
            Model.Record = Model.Records.Where(row => row.Id == ID).First();
            var _Batch = Model.Record.Ref_No;
            var _Title = Model.Record.TitleCustomer.Replace(".", "_"); // Replace dot with _ for file name correction.
            var _FileName = $"{_Batch}_{_Title}";

            ReportService.RptData = GetReportData(ID);              // always generate Data for report
            ReportService.RptModel = CreateReportModel(ID);         // and then generate report parameters
            ReportService.RptType = ReportType.Preview;
            var ReportList = ReportService.GetReportLink();
            string base64String = Convert.ToBase64String(ReportService.RptModel.ReportBytes);

            await js.InvokeVoidAsync("printer", base64String);
            
        }
        private ReportData GetReportData(int ID)
        {
            ReportData _Result = new();
            string _Query = ReportQuery.SaleInvoiceQuery($"[B2].[TranID]={ID}");
            var _DataTable = DataSource.GetDataTable(AppUser.DataFile, _Query, "ReportData");

            _Result.ReportTable = _DataTable;
            _Result.DataSetName = "ds_SaleInvoice";

            return _Result;
        }

        private ReportModel CreateReportModel(int _ID)
        {

            ReportModel _Reportmodel = new();
            try
            {
                var _InvoiceNo = "INV-Testing";
                var _Heading1 = "Sales Invoice";
                var _Heading2 = $"Invoice No. {_InvoiceNo}";
                var _ReportPath = AppUser.ReportFolder;
                var _ReportOption = ReportType.Excel;
                var _CompanyName = AppUser.Company;
                var _ReportFooter = AppFunctions.ReportFooter();

                _Reportmodel.ReportUrl = NavManager.BaseUri;

                // Input Parameters  (.rdl report file)
                _Reportmodel.InputReport.FilePath = _ReportPath;
                _Reportmodel.InputReport.FileName = "CDCInv";
                _Reportmodel.InputReport.FileExtention = "rdl";
                
                // output Parameters (like pdf, excel, word, html, tiff)
                _Reportmodel.OutputReport.FilePath = AppUser.PDFFolder;
                _Reportmodel.OutputReport.FileName = "SaleInvoice_" + _ID.ToString("0000");
                _Reportmodel.OutputReport.ReportType = ReportType.Preview;
                _Reportmodel.OutputReport.ReportType = _ReportOption;
                _Reportmodel.OutputReport.ReportUrl = _Reportmodel.ReportUrl;
                // Reports Parameters
                _Reportmodel.AddReportParameter("CompanyName", _CompanyName);
                _Reportmodel.AddReportParameter("Heading1", _Heading1);
                _Reportmodel.AddReportParameter("Heading2", _Heading2);
                _Reportmodel.AddReportParameter("Footer", _ReportFooter);

                //var _SaveAs = _Reportmodel.OutputReport.FileFullName;
                //_Reportmodel.OutputReport.FileName = _SaveAs;
            }
            catch (Exception)
            {
                Model.MsgClass.Add(MESSAGE.Default);
            }

            return _Reportmodel;
        }
        #endregion


    }
}

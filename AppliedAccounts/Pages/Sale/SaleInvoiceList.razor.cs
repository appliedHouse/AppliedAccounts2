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
        public async void Edit(int ID)
        {
            await Task.Delay(1000);
            // Add code here to delete sales invocies.
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
            var ReportLisk = ReportService.GetReportLink();
            await js.InvokeVoidAsync("downloadPDF", _FileName, ReportService.RptModel.ReportBytes);
            
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



                // Input Parameters  (.rdl report file)
                _Reportmodel.InputReport.FilePath = _ReportPath;
                _Reportmodel.InputReport.FileName = "CDCInv";
                _Reportmodel.InputReport.FileExtention = "rdl";
                // output Parameters (like pdf, excel, word, html, tiff)
                _Reportmodel.OutputReport.FilePath = AppUser.PDFFolder + "\\";
                _Reportmodel.OutputReport.FileLink = "";
                _Reportmodel.OutputReport.ReportType = _ReportOption;
                // Reports Parameters
                _Reportmodel.AddReportParameter("CompanyName", _CompanyName);
                _Reportmodel.AddReportParameter("Heading1", _Heading1);
                _Reportmodel.AddReportParameter("Heading2", _Heading2);
                _Reportmodel.AddReportParameter("Footer", _ReportFooter);

                //var _InvNo = _Reportmodel.ReportData.ReportTable.Rows[0]["Vou_No"].ToString() ?? _ID.ToString("0000");
                //var _RecNo = ""; // _Reportmodel.ReportData.DataSource.Records.First().Ref_No;
                //var _SaveAs = _Reportmodel.OutputReport.FileName = $"{_RecNo}INV-{_InvNo}";

                var _SaveAs = "Test";

                _Reportmodel.OutputReport.FileName = _SaveAs;




                //        string _RecNo = Model.Records.First().Ref_No;
                //var _SourceData = new DataTable();          // Inject a Data Table for print.

                //_Reportmodel.ReportData.DataSetName = "ds_SaleInvoice";
                //_Reportmodel.ReportData.ReportTable = _SourceData;
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

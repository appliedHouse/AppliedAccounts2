using AppliedAccounts.Data;
using AppliedAccounts.Services;
using AppliedDB;
using AppReports;
using Microsoft.JSInterop;
using MESSAGE = AppMessages.Enums.Messages;


namespace AppliedAccounts.Pages.Purchase
{
    public partial class PurchaseList
    {
        public AppUserModel AppUser { get; set; }
        public Models.PurchaseListModel MyModel { get; set; }
        public PrintService ReportService { get; set; }
        private bool IsPrinted { get; set; } = false;
        private bool IsPrinting { get; set; } = false;
        public string PrintingMessage { get; set; }

        public void Search()
        {
            MyModel.LoadData();
        }

        #region Edit
        public void Edit(int ID)
        {
            NavManager.NavigateTo($"/Purchase/Purchased/{ID}");
        }
        #endregion

        #region Delete
        public async void Delete(int ID)
        {

            await Task.Delay(1000);
            // Add code here to delete sales invocies.
        }
        #endregion

        #region Select All and Select One
        public void SelectAll()
        {
            MyModel.SelectAll = !MyModel.SelectAll;
            MyModel.Records?.ForEach(item => item.IsSelected = MyModel.SelectAll);
            //StateHasChanged();
        }

        public void SelectOne(int _ID)
        {
            var item = MyModel.Records.Where(a => a.ID1 == _ID).First();
            item.IsSelected = !item.IsSelected;
            
        }
        #endregion
        #region Sales Invoice report print -- Print -- Print All - 

        public async void PrintAll()
        {
            IsPrinting = true;
            await InvokeAsync(StateHasChanged);
            PrintingMessage = "Priting Start.";

            foreach (var item in MyModel.Records)
            {
                if (item.IsSelected)
                {
                    PrintingMessage = $"Sales invoice for {MyModel.Record.SupplierTitle} is being printed.";
                    await InvokeAsync(StateHasChanged);
                    await Print(item.ID1);
                }
            }
            IsPrinting = false;
            await InvokeAsync(StateHasChanged);
        }

        public async Task Print(int ID)
        {
            //await InvokeAsync(StateHasChanged);
            MyModel.Record = MyModel.Records.Where(row => row.ID1 == ID).First();
            var _Batch = MyModel.Record.Ref_No;
            var _Title = MyModel.Record.SupplierTitle.Replace(".", "_"); // Replace dot with _ for file name correction.
            var _FileName = $"{_Batch}_{_Title}";

            ReportService.RptData = GetReportData(ID);              // always generate Data for report
            ReportService.RptModel = CreateReportModel(ID);         // and then generate report parameters
            ReportService.RptType = ReportType.Preview;
            //var ReportList = ReportService.GetReportLink();
            await js.InvokeVoidAsync("downloadPDF", _FileName, ReportService.RptModel.ReportBytes);

        }
        private ReportData GetReportData(int ID)
        {
            ReportData _Result = new();
            string _Query = ReportQuery.SaleInvoiceQuery($"[B2].[TranID]={ID}");
            var _DataTable = DataSource.GetDataTable(AppUser.DataFile, _Query, "ReportData");

            _Result.ReportTable = _DataTable;
            _Result.DataSetName = "ds_PurchaseInvoice";

            return _Result;
        }

        private ReportModel CreateReportModel(int _ID)
        {

            ReportModel _Reportmodel = new();
            try
            {
                var _InvoiceNo = "INV-Testing";
                var _Heading1 = "Purchase Invoice";
                var _Heading2 = $"Invoice No. {_InvoiceNo}";
                var _ReportPath = AppUser.ReportFolder;
                var _ReportOption = ReportType.Excel;
                var _CompanyName = AppUser.Company;
                var _ReportFooter = AppFunctions.ReportFooter();

                // Input Parameters  (.rdl report file)
                _Reportmodel.InputReport.FilePath = _ReportPath;
                _Reportmodel.InputReport.FileName = "PurchasedInvoice";
                _Reportmodel.InputReport.FileExtention = "rdl";
                // output Parameters (like pdf, excel, word, html, tiff)
                _Reportmodel.OutputReport.FilePath = AppUser.PDFFolder + "\\";
                //_Reportmodel.OutputReport.FileLink = "";
                _Reportmodel.OutputReport.ReportType = _ReportOption;
                // Reports Parameters
                _Reportmodel.AddReportParameter("CompanyName", _CompanyName);
                _Reportmodel.AddReportParameter("Heading1", _Heading1);
                _Reportmodel.AddReportParameter("Heading2", _Heading2);
                _Reportmodel.AddReportParameter("Footer", _ReportFooter);

                var _SaveAs = "Test";

                _Reportmodel.OutputReport.FileName = _SaveAs;
            }
            catch (Exception)
            {
                MyModel.MsgClass.Add(MESSAGE.Default);
            }

            return _Reportmodel;
        }
        #endregion

    }
}

using AppliedAccounts.Data;
using AppliedDB;
using AppReports;
using Microsoft.JSInterop;
using System.Data;
using MESSAGE = AppMessages.Enums.Messages;


namespace AppliedAccounts.Pages.Sale
{
    public partial class SaleInvoiceList
    {
        public AppUserModel AppUser { get; set; }
        public Models.SaleInvoiceListModel Model { get; set; }
        public ReportModel PrintClass { get; set; }

        public SaleInvoiceList()
        {
        }

        public async void Print(int ID)
        {
            if (PrintClass is not null)
            {
                PrintClass.OutputReport.ReportType = ReportType.PDF;
                PrintClass.ReportData = GetReportData(ID);
                PrintClass.ReportRender();


                await js.InvokeVoidAsync("displayPDF", PrintClass.OutputReport.FileLink);
                //await js.InvokeVoidAsync("downloadFile", PrintClass.OutputReport.FileLink);
            }

        }

        public async void PrintAll()
        {
            await Task.Delay(1000);

        }

        public async void Delete(int ID)
        {

            await Task.Delay(1000);
            // Add code here to delete sales invocies.
        }

        public async void Edit(int ID)
        {
            await Task.Delay(1000);
            // Add code here to delete sales invocies.
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


        #region Create a Class for print report
        public ReportModel CreateReportModel()
        {
            ReportModel _Reportmodel = new ReportModel();
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
                _Reportmodel.OutputReport.FileName = "SaleInvoice";
                _Reportmodel.OutputReport.ReportType = _ReportOption;
                // Reports Parameters
                _Reportmodel.AddReportParameter("CompanyName", _CompanyName);
                _Reportmodel.AddReportParameter("Heading1", _Heading1);
                _Reportmodel.AddReportParameter("Heading2", _Heading2);
                _Reportmodel.AddReportParameter("Footer", _ReportFooter);

                var _SourceData = new DataTable();          // Inject a Data Table for perint.

                _Reportmodel.ReportData.DataSetName = "ds_SaleInvoice";
                _Reportmodel.ReportData.ReportTable = _SourceData;
            }
            catch (Exception)
            {
                Model.MyMessages.Add(MESSAGE.Default);
            }

            return _Reportmodel;
        }
        #endregion
    }
}

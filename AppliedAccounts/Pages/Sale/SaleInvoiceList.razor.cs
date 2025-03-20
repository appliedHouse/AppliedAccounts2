using AppliedAccounts.Data;
using AppliedDB;
using AppReports;
using Microsoft.JSInterop;
using System.Data;
using static AppliedAccounts.Services.PrintService;
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


        public SaleInvoiceList()
        {
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
            PrintedReports = new();
            await Task.Run(() =>
            {
                foreach (var item in Model.Records)
                {
                    if (item.IsSelected)
                    {
                        Print(item.Id, downloadOption.downloadFile);

                    }

                }
            });
            IsPrinting = false;
            IsPrinted = true;
            await InvokeAsync(() => StateHasChanged());
        }

        public void Print(int ID)
        {
            //PrintingService.Print("docs/sample.pdf")

            Print(ID, downloadOption.displayPDF);
                        
        }

        public async void Print(int ID, downloadOption Option)
        {
            IsPrinted = false;
            if (PrintClass is not null)
            {
                PrintClass.OutputReport.ReportType = ReportType.PDF;
                PrintClass.ReportData = GetReportData(ID);

                if (PrintClass.ReportData.ReportTable.Rows.Count > 0)
                {
                    string _InvNo = PrintClass.ReportData.ReportTable.Rows[0]["Vou_No"].ToString() ?? ID.ToString("0000");
                    string _RecNo = Model.Records.First().Ref_No;

                    PrintClass.OutputReport.FileName = $"{_RecNo}INV-{_InvNo}";
                    PrintClass.ReportRender();
                    PrintedReports.Add(PrintClass.OutputReport.FileFullName);
                    
                    if(Option == DownloadOption.displayPDF)
                    {
                        //await PrintingService.Print(PrintClass.OutputReport.FileLink);
                    }
                    else
                    {
                        await js.InvokeVoidAsync(Option.ToString(), PrintClass.OutputReport.FileLink);
                    }

                }
            }
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

        private ReportModel CreateReportModel()
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

                var _SourceData = new DataTable();          // Inject a Data Table for print.

                _Reportmodel.ReportData.DataSetName = "ds_SaleInvoice";
                _Reportmodel.ReportData.ReportTable = _SourceData;
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

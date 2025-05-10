using AppliedAccounts.Data;
using AppliedAccounts.Services;
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
        public Models.SaleInvoiceListModel MyModel { get; set; }
        public ReportModel PrintClass { get; set; }
        private bool IsPrinted { get; set; } = false;
        private bool IsPrinting { get; set; } = false;
        private List<string> PrintedReports { get; set; } = new();
        public PrintService ReportService { get; set; }
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
            MyModel.SelectAll = !MyModel.SelectAll;
            MyModel.Records?.ForEach(item => item.IsSelected = MyModel.SelectAll);
            //StateHasChanged();
        }

        public void SelectOne(int _ID)
        {
            var item = MyModel.Records.Where(a => a.Id == _ID).First();
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

        public async void PrintOnePDF()
        {
            IsPrinting = true;
            await InvokeAsync(StateHasChanged);
            PrintingMessage = "Priting Start.";
            List<int> SaleInvoiceIDList = new List<int>();


            foreach (var item in MyModel.Records)
            {
                if (item.IsSelected) { SaleInvoiceIDList.Add(item.Id); }
            }
            if (SaleInvoiceIDList.Count > 0)
            {
                try
                {
                    var _Text = DateTime.Now.ToString(Format.YMD);
                    var _RandomNo = (new Random()).Next(1000, 9999);
                    var _FileName = $"SalesInvoice_{_Text}_{_RandomNo}";

                    ReportService.Data = GetReportDataOnePDF(SaleInvoiceIDList);              // always generate Data for report
                    ReportService.Model = CreateReportModelOnePDF();                          // and then generate report parameters
                    ReportService.ReportType = ReportType.Preview;
                    //var ReportList = ReportService.GetReportLink();
                    var rptArray = ReportService.Model.ReportBytes;
                    var rptMime = ReportService.Model.OutputReport.MimeType;
                    var rptFile = $"{MyModel.Record.Ref_No}_{MyModel.Record.TitleCustomer}";
                    await js.InvokeVoidAsync("downloadFile", _FileName, rptArray, rptMime);
                }
                catch (Exception error)
                {
                    MyModel.MsgClass.Add(error.Message);
                }
            }



            IsPrinting = false;
            await InvokeAsync(StateHasChanged);
        }

        public async void PrintAll()
        {
            IsPrinting = true;
            await InvokeAsync(StateHasChanged);
            PrintingMessage = "Priting Start.";

            foreach (var item in MyModel.Records)
            {
                if (item.IsSelected)
                {
                    PrintingMessage = $"Sales invoice for {MyModel.Record.TitleCustomer} is being printed.";
                    await InvokeAsync(StateHasChanged);
                    //await DownLoadPrint(item.Id);
                }
            }
            IsPrinting = false;
            await InvokeAsync(StateHasChanged);
        }

        public async Task Print(ReportActionClass reportAction)
        {
            MyModel.VoucherID = reportAction.VoucherID;

            try
            {
                IsPrinting = true;
                await InvokeAsync(StateHasChanged);
                MyModel.Record = MyModel.Records.Where(row => row.Id == MyModel.VoucherID).First();
                var _Batch = MyModel.Record.Ref_No;
                var _Title = MyModel.Record.TitleCustomer.Replace(".", "_"); // Replace dot with _ for file name correction.
                var _FileName = $"{_Batch}_{_Title}";
                var _VoucherNo = MyModel.Record.Vou_No ?? "NoVoucher";

                ReportService = new(AppGlobals);
                GetReportData();              // always generate Data for report
                UpdateReportModel();         // and then generate report parameters
                ReportService.Print();

                IsPrinting = false;
                IsPrinted = true;
                await InvokeAsync(StateHasChanged);

            }
            catch (Exception error)
            {
                MyModel.MsgClass.Add(error.Message);
            }

        }




        private void GetReportData()
        {
            string _Query = ReportQuery.SaleInvoiceQuery($"[TranID]={MyModel.VoucherID}");
            var _DataTable = DataSource.GetDataTable(AppUser.DataFile, _Query, "ReportData");

            ReportService.Data.ReportTable = _DataTable;
            ReportService.Data.DataSetName = "ds_SaleInvoice";

           
        }

        private ReportData GetReportDataOnePDF(List<int> _SaleInvoiceIDList)
        {
            ReportData _Result = new();
            var _Filter = string.Join(",", _SaleInvoiceIDList);

            string _Query = ReportQuery.SaleInvoiceQuery($"[TranID] in ({_Filter})");
            var _DataTable = DataSource.GetDataTable(AppUser.DataFile, _Query, "ReportData");

            ReportService.Data.ReportTable = _DataTable;
            ReportService.Data.DataSetName = "ds_SaleInvoice";

            return _Result;
        }

        private void UpdateReportModel()
        {
                var _Heading1 = "Sales Invoice";
                var _Heading2 = $"Invoice No. {MyModel.Record.Vou_No}";

                ReportService.Model.InputReport.FileName = "CDCInv.rdl";
                ReportService.Model.OutputReport.FileName = "SaleInvoice_" + new Random().Next(10000,99999).ToString();
                ReportService.Model.AddReportParameter("Heading1", _Heading1);
                ReportService.Model.AddReportParameter("Heading2", _Heading2);
            
        }

        private ReportModel CreateReportModelOnePDF()
        {

            ReportModel _Reportmodel = new();
            try
            {
                var _InvoiceNo = "INV-Testing";
                var _Heading1 = "Sales Invoice";
                var _Heading2 = $"Invoice No. {_InvoiceNo}";
                var _ReportOption = ReportType.Excel;


                // Input Parameters  (.rdl report file)
                _Reportmodel.InputReport.FileName = "CDCInvOnePDF.rdl";

                // output Parameters (like pdf, excel, word, html, tiff)
                _Reportmodel.OutputReport.FileName = "SaleInvoice_" + (new Random()).Next(1000, 9999).ToString();
                _Reportmodel.OutputReport.ReportType = _ReportOption;
                // Reports Parameters
                _Reportmodel.AddReportParameter("Heading1", _Heading1);
                _Reportmodel.AddReportParameter("Heading2", _Heading2);

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

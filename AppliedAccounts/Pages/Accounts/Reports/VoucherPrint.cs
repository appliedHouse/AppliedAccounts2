using AppliedAccounts.Data;
using AppliedAccounts.Services;
using AppliedDB;
using AppReports;

namespace AppliedAccounts.Pages.Accounts.Reports
{
    public class VoucherPrint
    {
        public PrintService ReportService { get; set; } = new();
        public ReportModel Model { get; set; } = new();
        public GlobalService AppGlobal { get; set; }

        public long VoucherID { get; set; }  
        public string VoucherNo { get; set; }
        public DataSource Source { get; set; }
        public bool IsPrinting { get; set; } = false;
        public bool IsError { get; set; } = false;


        #region Constructor
        public VoucherPrint()
        {

        }
        
        public VoucherPrint(ReportActionClass reportAction, GlobalService appGlobal)
        {
            AppGlobal = appGlobal;
            VoucherID = reportAction.VoucherID;
            Source = new(AppGlobal.AppPaths);
            ReportService.JS = AppGlobal.JS;
        }
        
        #endregion

        public async Task PrintAsync()
        {
            IsPrinting = true;                                   // Flag for show printing status in UI
            ReportService.Model = await GetReportModelAsync();   // Get the report model asynchronously 
            ReportService.IsError = IsError;                     // Set the error status in the report service
            if (!ReportService.IsError)
            {
                ReportService.Print();
            }
        }
        
        public void Print()
        {
            IsPrinting = true;                                   // Flag for show printing status in UI
            ReportService.Model = GetReportModelAsync().Result;  // Get the report model asynchronously 
            ReportService.IsError = IsError;                     // Set the error status in the report service
            ReportService.Data = ReportService.Model.ReportDataSource;
            if (!ReportService.IsError)
            {
                ReportService.Print();
            }
        }


        public async Task<ReportModel> GetReportModelAsync()
        {
            try
            {
                Model ??= new();
                Model.InputReport.FileName = "Voucher.rdl";
                Model.OutputReport.FileName = VoucherNo;
                Model.OutputReport.ReportType = ReportService.ReportType;

                GetVoucherData();
                GetVoucherParameters();

                if (IsError)
                {
                    Model.ErrorMessage = $"No data found for Voucher No: {VoucherNo}";
                    return null!;
                }
            }
            catch (Exception error)
            {
                Model.ErrorMessage = $"ERROR: {error.Message}";
            }

            return Model;
        }

        private void GetVoucherParameters()
        {
            Model.AddReportParameter("CompanyName", AppGlobal.Client.DisplayName);
            Model.AddReportParameter("Heading1", "General Voucher");
            Model.AddReportParameter("Heading2", $"Voucher - {VoucherNo}");
            Model.AddReportParameter("Footer", AppGlobal.Reporting.ReportFooter);
        }

        private void GetVoucherData()
        {
            //Source = new DataSource(AppGlobal.AppPaths);
            var _Query = string.Empty;

            if (VoucherNo == null)
            {
                if(VoucherID == 0)
                {
                    Model.ReportDataSource.ReportTable = null!;
                    Model.ReportDataSource.DataSetName = null!;
                    return;
                }
                else
                {
                    
                    _Query = $"SELECT [Voucher_No] FROM Ledger WHERE id = {VoucherID}";
                    VoucherNo = (string)Source.SeekValue(Enums.Tables.Ledger, VoucherID, "Vou_No")! ?? "";
                }
            }


            ////SELECT * FROM [Ledger] WHERE [Vou_No] = 'JV2606-0005' ORDER BY [Sr_No]
            _Query = $"SELECT * FROM [Ledger] WHERE [Vou_No] = '{VoucherNo}' ORDER BY [Sr_No]";
            var _Data = Source.GetTable(_Query);
            if (_Data.Rows.Count > 0)
            {
                Model.ReportDataSource.ReportTable = _Data;
                Model.ReportDataSource.DataSetName = "ds_Voucher";
            }
            else
            {
                IsError = true;
            }
        }
    }
}

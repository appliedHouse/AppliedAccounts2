using AppliedAccounts.Services;
using AppliedDB;
using AppReports;
using Microsoft.Reporting.NETCore;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

namespace AppliedAccounts.Pages.Accounts.Reports
{
    public class VoucherPrint
    {
        public PrintService ReportService { get; set; }
        public ReportModel Model { get; set; } = new();
        public GlobalService AppGlobal { get; set; }
       
        public string VoucherNo { get; set; } = "Voucher";
        public DataSource Source { get; set; }
        public bool IsPrinting { get; set; } = false;
        public bool IsError { get; set; } = false;


        #region Constructor
        public VoucherPrint()
        {

        }
        
        public VoucherPrint(GlobalService appGlobal)
        {
            AppGlobal = appGlobal;
            ReportService = new(AppGlobal);
        }

        public VoucherPrint(GlobalService appGlobal, ReportType reportType)
        {
            AppGlobal = appGlobal;
            ReportService = new(AppGlobal)
            {
                ReportType = reportType
            };
        }

        public VoucherPrint(GlobalService appGlobal, ReportType reportType, string voucherNo)
        {
            AppGlobal = appGlobal;
            ReportService = new(AppGlobal)
            {
                ReportType = reportType
            };

            VoucherNo = voucherNo;
        }
        #endregion

        public async Task Print()
        {
            IsPrinting = true;                                   // Flag for show printing status in UI
            ReportService.Model = await GetReportModelAsync();  // Get the report model asynchronously 
            ReportService.IsError = IsError;                       // Set the error status in the report service
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
            Model.AddReportParameter("Heading1", "General Voucher");
            Model.AddReportParameter("Heading2", $"Voucher - {VoucherNo}");
        }

        private void GetVoucherData()
        {
            Source = new DataSource(AppGlobal.AppPaths);
            string _Query = $"SELECT * FROM [Ledger] WHERE [Vou_No] = '{VoucherNo}'";
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

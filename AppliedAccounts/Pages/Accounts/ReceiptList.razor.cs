using AppliedAccounts.Models;
using AppliedDB;
using Microsoft.JSInterop;


namespace AppliedAccounts.Pages.Accounts
{
    public partial class ReceiptList
    {
        private object _FileName;

        public AppUserModel UserProfile { get; set; }
        public ReceiptListModel MyModel { get; set; }

        public bool IsWaiting { get; set; }
        public string SpinnerMessage { get; set; }
        public ReceiptList()
        {

            IsWaiting = false;
        }

        public async Task Print(int ID)
        {
            try
            {
                SpinnerMessage = "Report is being generated.  Wait some wile.";
                IsWaiting = true;
                await Task.Run(()=> { MyModel.Print(ID); }); // Generate Report

                var reportModel = MyModel.ReportService.RptModel;

                if (reportModel?.ReportBytes?.Length > 0)
                {
                    var reportName = reportModel.OutputReport.FileName + ID.ToString();
                    var reportBytes = reportModel.ReportBytes;

                    await js.InvokeVoidAsync("downloadPDF", reportName, reportBytes);
                }
            }
            catch (Exception ex)
            {
                // Log or display error
                Console.WriteLine($"Print error: {ex.Message}");
            }
            finally
            {
                IsWaiting = false;
                await InvokeAsync(StateHasChanged);
            }
        }

        public async void PrintPreview(int ID)
        {
            try
            {
                SpinnerMessage = "Report is being generated.  Wait some wile.";
                IsWaiting = true;
                await Task.Run(() => { MyModel.Print(ID); }); // Generate Report
                
            }
            catch (Exception ex)
            {
                // Log or display error
                Console.WriteLine($"Print error: {ex.Message}");
            }
            finally
            {
                IsWaiting = false;
                await InvokeAsync(StateHasChanged);
                PrintService.Preview();
            }

        }

    }
}

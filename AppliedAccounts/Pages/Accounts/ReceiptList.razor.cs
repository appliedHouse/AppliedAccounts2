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
                SpinnerMessage = "Report is being generated.  Wait for some while.";
                IsWaiting = true;

                await Task.Run(() =>
                {
                    MyModel.Print(ID);
                    MyModel.ReportService.JS = js;                              // Inject js into Print Servce
                    MyModel.ReportService.NavManager = NavManager;              // Inject NavManager into Print Service
                });

                var reportModel = MyModel.ReportService.RptModel;

                if (reportModel?.ReportBytes?.Length > 0)
                {
                    //var reportName = reportModel.OutputReport.FileName + ID.ToString();
                    var base64 = Convert.ToBase64String(reportModel.ReportBytes);
                    await js.InvokeVoidAsync("printer", base64);

                    //await js.InvokeVoidAsync("downloadPDF", reportName, reportBytes);
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
                SpinnerMessage = "Report is being generated.  Wait for some while.";
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

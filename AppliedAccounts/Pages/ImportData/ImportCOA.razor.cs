using AppliedAccounts.Models;
using Microsoft.AspNetCore.Components.Forms;

namespace AppliedAccounts.Pages.ImportData
{
    public partial class ImportCOA
    {

        public TestModel MyModel { get; set; } 
        public ImportExcelFile ImportCOAModel { get; set; }
        public bool ShowSpinner { get; set; } = false;
        public string ExcelFileName { get; set; } = "";



        public ImportCOA()
        {
            MyModel = new TestModel(AppGlobal);
        }

        public async Task GetExcelFile(InputFileChangeEventArgs e)
        {
            ShowSpinner = true;
            ExcelFileName = e.File.Name;
            MyModel.SpinnerMessage = $"Loading Excel file: [{e.File.Name}]. Please wait...";
            await InvokeAsync(StateHasChanged);

            await Task.Delay(100); // Simulate delay for spinner
            ImportCOAModel = new ImportExcelFile(e.File, AppGlobal);
            await ImportCOAModel.ImportDataAsync();            // ImportExcelFile.cs Function

            ShowSpinner = false;
            IsExcelLoaded = true;                              // Excel file has been loaded successfully.


            
        }
    }
}

using AppliedAccounts.Component;
using AppliedAccounts.Models;
using AppliedAccounts.Models.Import;
using Microsoft.AspNetCore.Components.Forms;
using System.Data;
using MESSAGES = AppMessages.Enums.Messages;

namespace AppliedAccounts.Pages.ImportData
{
    public partial class ImportStock
    {

        public ImportStockModel MyModel { get; set; } = new();
        public ImportExcelFile ImportCOAModel { get; set; }
        public string SpinnerMessage { get; set; } = string.Empty;
        public string SpinnerType { get; set; }

        public async Task GetExcelFile(InputFileChangeEventArgs e)
        {
            MyModel.ExcelFileName = e.File.Name;
            SpinnerMessage = $"Loading Excel file: [{e.File.Name}]. Please wait...";
            await InvokeAsync(StateHasChanged);

            await Task.Delay(100); // Simulate delay for spinner
            ImportCOAModel = new ImportExcelFile(e.File, AppGlobal, "ImportStock");
            await ImportCOAModel.ImportDataAsync();            // ImportExcelFile.cs Function

            SpinnerMessage = $"Excel file: [{e.File.Name}] has been loaded sucessfully";
            SpinnerType = "success";
            MyModel.IsExcelLoaded = true;      // Excel file has been loaded successfully.

            Step1 = false;

            MyModel.LoadImportedData();
            Step2 = true;
        }

        public List<DataRow> GetFilteredData(string _TableName)
        {
            MyModel.Pages ??= new();
            if (MyModel.ImportedData.Count == 0)
            {
                MyModel.MsgService.Error(MESSAGES.NoRecordFound);
                return [];
            }
            return [.. MyModel.ImportedData.Skip(MyModel.Pages.Current).Take(MyModel.Pages.Size)];                // Copy Imported Data to Filter Data
        }

        
    }
}

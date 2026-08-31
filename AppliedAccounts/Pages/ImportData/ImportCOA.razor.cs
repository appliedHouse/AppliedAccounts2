using AppliedAccounts.Models;
using Microsoft.AspNetCore.Components.Forms;
using System.Data;
using MESSAGES = AppMessages.Enums.Messages;

namespace AppliedAccounts.Pages.ImportData
{
    public partial class ImportCOA
    {

        public ImportCOAModel MyModel { get; set; } = new();
        public ImportExcelFile ImportCOAModel { get; set; }
        public bool ShowSpinner { get; set; } = false;
        public string SpinnerMessage { get; set; } = "Loading... Please wait...";

        public async Task GetExcelFile(InputFileChangeEventArgs e)
        {
            ShowSpinner = true;
            MyModel.IsExcelLoaded = false;
            MyModel.ExcelFileName = e.File.Name;
            SpinnerMessage = $"Loading Excel file: [{e.File.Name}]. Please wait...";
            await InvokeAsync(StateHasChanged);

            await Task.Delay(100); // Simulate delay for spinner
            ImportCOAModel = new ImportExcelFile(e.File, AppGlobal, "ImportCOA");
            await ImportCOAModel.ImportDataAsync();            // ImportExcelFile.cs Function

            ShowSpinner = false;
            MyModel.IsExcelLoaded = true;                              // Excel file has been loaded successfully.
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

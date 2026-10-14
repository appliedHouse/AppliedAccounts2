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
        public ImportExcelFile ImportModel { get; set; }
        public string SpinnerMessage { get; set; } = string.Empty;
        public string SpinnerType { get; set; }

        public async Task GetExcelFile(InputFileChangeEventArgs e)
        {
            try
            {
                var file = e.File;


                MyMessage = string.Empty;
                Step1 = false;
                Step2 = true;

                MyModel.ExcelFileName = file.Name;
                SpinnerMessage = $"Loading Excel file: [{file.Name}]. Please wait...";
                //await InvokeAsync(StateHasChanged);

                //await Task.Delay(100); // Simulate delay for spinner
                ImportModel = new ImportExcelFile(file, AppGlobal, "ImportStock");
                await ImportModel.ImportDataAsync();            // ImportExcelFile.cs Function

                if (ImportModel.IsImported)
                {
                    Step2 = false;
                    Step3 = true;

                    SpinnerMessage = $"Excel file: [{e.File.Name}] has been loaded sucessfully";
                    SpinnerType = "success";
                    MyModel.IsExcelLoaded = true;      // Excel file has been loaded successfully.

                    MyModel.LoadImportedData();
                    //await InvokeAsync(StateHasChanged);
                }
                else
                {
                    Step1 = false;
                    Step2 = false;
                    Step3 = false;
                    Step4 = false;
                    StepError = true;

                    SpinnerMessage = "Data Import has error. Check Excel Data File or contect to administrator";
                    SpinnerMessage += ImportModel.MyMessage;
                    SpinnerType = "Danger";
                    //await InvokeAsync(StateHasChanged);

                }

            }
            catch (Exception error)
            {
                MsgService.Error(error);
            }
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

        public async Task ImportInDB()
        {
            Step3 = false;
            Step4 = true;
            await InvokeAsync(StateHasChanged);

            SpinnerMessage = "Inventory Unit of Measure is being exported to Database Tables...";
            MyModel.ImportUOM();
            await InvokeAsync(StateHasChanged);

            SpinnerMessage = "Inventory Packing is being exported to Database Tables...";
            MyModel.ImportPacking();
            await InvokeAsync(StateHasChanged);

            SpinnerMessage = "Inventory Size is being exported to Database Tables...";
            MyModel.ImportSize();
            await InvokeAsync(StateHasChanged);

            SpinnerMessage = "Inventory Category is being exported to Database Tables...";
            MyModel.ImportCategory();
            await InvokeAsync(StateHasChanged);

            SpinnerMessage = "Inventory Sub Category is being exported to Database Tables...";
            MyModel.ImportSubCategory();
            await InvokeAsync(StateHasChanged);

            SpinnerMessage = "Inventory items is being exported to Database Tables...";
            MyModel.ImportInventory();
            await InvokeAsync(StateHasChanged);


            MsgService.AddRange(MyModel.MsgService);
            await InvokeAsync(StateHasChanged);

        }

    }
}

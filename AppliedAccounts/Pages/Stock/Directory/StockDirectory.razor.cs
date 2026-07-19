using AppliedDB;
using AppMessages;
using Menus;
using System.Data;
using AppliedAccounts.Models;
using static AppliedDB.Enums;

namespace AppliedAccounts.Pages.Stock.Directory
{
    public partial class StockDirectory
    {
        public MessageClass MsgClass { get; set; } = new();

        public CodeTitleModel MyModel { get; set; } = new();
        public DataSource Source { get; set; }
        public string Filter { get; set; }
        public string Sort { get; set; }
        public bool EditMode { get; set; } 
        public bool IsDeleted { get; set; } 

        public List<CodeTitle> StockDirectoryList { get; set; }

        private readonly Dictionary<string, (Tables Table, string Title)> _tableMap = new()
        {
            ["Inv_Category"] = (Tables.Inv_Category, "Stock Category"),
            ["Inv_SubCategory"] = (Tables.Inv_SubCategory, "Stock Sub Category"),
            ["Inv_Packing"] = (Tables.Inv_Packing, "Stock Packing"),
            ["Inv_Size"] = (Tables.Inv_Size, "Stock Item Size"),
            ["Inv_UOM"] = (Tables.Inv_UOM, "Stock Unit of Measurement")
        };

        private async Task GetBackPath() 
        {
            EditMode = false;
            IsDeleted = false;
            AppGlobal.NavManager.NavigateTo($"/Stock/Directory/{TableName}");
            await Task.CompletedTask;
        }

        public void LoadData(string tableName)
        {
            
            Source ??= new(AppGlobal.AppPaths);

            if (_tableMap.TryGetValue(tableName, out var mapping))
            {
                StockDirectoryList = Source.GetCodeTitle(mapping.Table, "Title");
                SubHeadingTitle = mapping.Title;
            }
        }

        public async Task New()
        {
            EditMode = true;
            MyModel = new CodeTitleModel()
            {
                ID = 0,
                Code = string.Empty,
                Title = string.Empty
            };
            await InvokeAsync(StateHasChanged);
        }
        public async Task Edit(long _ID, bool? isDelete = false)
        {
            EditMode = true;
            IsDeleted = isDelete ?? false;
            var _data = StockDirectoryList.FirstOrDefault(e => e.ID == _ID);
            if (_data != null)
            {
                MyModel.ID = _data.ID;
                MyModel.Code = _data.Code;
                MyModel.Title = _data.Title;
            }

            await InvokeAsync(StateHasChanged);
        }
        public async Task Delete(long _ID)
        {
            
            var _ExistingRow = GetExistingRow(_ID);
            if (_ExistingRow != null)
            {
                IsDeleted = await Source.DeleteAsync(_ExistingRow);   // if delete is successful, IsDeleted will be false, otherwise true
                if (IsDeleted) 
                {
                    LoadData(TableName!);
                    IsDeleted = false;
                    EditMode = false;
                }
            }
        }

        private DataRow? GetExistingRow(long _ID)
        {
            if (MyModel.ID > 0)
            {
                string _Query = $"SELECT * FROM {TableName} WHERE ID = {_ID}";
                var Row = Source.GetDataRow(_Query);
                return Row;
            }
            return null;
        }

        #region Save Methods
        public void Save()
        {
            EditMode = false;
            InvokeAsync(StateHasChanged);

            if (Enum.TryParse<Tables>(TableName, out var table))
            {
                var _Row = Source.GetNewRow(table);
                _Row["ID"] = MyModel.ID;
                _Row["Code"] = MyModel.Code;
                _Row["Title"] = MyModel.Title;

                Source.Save(_Row);
                if (Source.IsSaved)
                {
                    LoadData(TableName);
                    ToastService.ShowSuccess($"'{MyModel.Title}' has been saved successfully!");
                }
            }
            else
            {
                MsgClass.Critical(AppMessages.Enums.Messages.DataTableNotFound);
            }
        }

        #endregion

        

    }
}



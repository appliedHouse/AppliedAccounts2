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
        public bool EditMode { get; set; } = false;
        public bool IsDeleted { get; set; } = false;

        public List<CodeTitle> StockDirectoryList { get; set; }

        private readonly Dictionary<string, (Tables Table, string Title)> _tableMap = new()
        {
            ["Inv_Category"] = (Tables.Inv_Category, "Stock Category"),
            ["Inv_SubCategory"] = (Tables.Inv_SubCategory, "Stock Sub Category"),
            ["Inv_Packing"] = (Tables.Inv_Packing, "Stock Packing"),
            ["Inv_Size"] = (Tables.Inv_Size, "Stock Item Size"),
            ["Inv_UOM"] = (Tables.Inv_UOM, "Stock Unit of Measurement")
        };

        private string GetBackPath() 
        {
            EditMode = false;
            IsDeleted = false;

            string _result = string.Empty;
            //if(TableName == "Inv_Category") { _result = NavigationPaths.StockCategory(); }
            //else if (TableName == "Inv_SubCategory") { _result = NavigationPaths.StockSubCategory(); }
            //else if (TableName == "Inv_Packing") { _result = NavigationPaths.StockPacking(); }
            //else if (TableName == "Inv_Size") { _result = NavigationPaths.StockSize(); }
            //else if (TableName == "Inv_UOM") { _result = NavigationPaths.StockUOM(); }
            return _result;
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
            IsDeleted = true;
            EditMode = true;
            var _ExistingRow = GetExistingRow(_ID);
            if (_ExistingRow != null)
            {
                await Source.DeleteAsync(_ExistingRow);
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



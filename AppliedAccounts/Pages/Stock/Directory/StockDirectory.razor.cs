using AppliedDB;
using AppMessages;
using Microsoft.AspNetCore.Components;
using System.Data;
using static AppliedDB.Enums;

// This page is all direcotries of Stock. Only Table name is passed to this page and it will load the data accordingly.

namespace AppliedAccounts.Pages.Stock.Directory
{
    public partial class StockDirectory
    {
        [Parameter] public string? TableName { get; set; }
        public MessageClass MsgClass { get; set; } = new();

        public CodeTitleModel MyModel { get; set; } = new();
        public DataSource Source { get; set; }
        public string Filter { get; set; }
        public string Sort { get; set; }
        public bool EditMode { get; set; }
        public bool IsDelete { get; set; }


        public List<CodeTitle> StockDirectoryList { get; set; }

        protected override void OnParametersSet()
        {
            if (!string.IsNullOrEmpty(TableName))
            {
                LoadData(TableName);
            }
        }

        public void LoadData(string _TableName)
        {
            var _Sort = "Title";
            Source ??= new(AppGlobal.AppPaths);

            switch (_TableName)
            {
                case "Inv_Category":
                    StockDirectoryList = Source.GetCodeTitle(Tables.Inv_Category, _Sort);
                    SubHeadingTitle = "Stock Category";
                    break;
                case "Inv_SubCategory":
                    StockDirectoryList = Source.GetCodeTitle(Tables.Inv_SubCategory, _Sort);
                    SubHeadingTitle = "Stock Sub Category";
                    break;
                case "Inv_Packing":
                    StockDirectoryList = Source.GetCodeTitle(Tables.Inv_Packing, _Sort);
                    SubHeadingTitle = "Stock Packing";
                    break;
                case "Inv_Size":
                    StockDirectoryList = Source.GetCodeTitle(Tables.Inv_Size, _Sort);
                    SubHeadingTitle = "Stock Item Size";
                    break;
                case "Inv_UOM":
                    StockDirectoryList = Source.GetCodeTitle(Tables.Inv_UOM, _Sort);
                    SubHeadingTitle = "Stock Unit of Measurement";
                    break;
                default:
                    break;
            }
        }

        public void Add()
        {
            EditMode = true;
            MyModel.ID = 0;
            MyModel.Code = string.Empty;
            MyModel.Title = string.Empty;
        }

        public void EditForm(long _ID, bool isDelete)
        {
            IsDelete = isDelete;
            EditMode = true;
            var _data = StockDirectoryList.FirstOrDefault(e => e.ID == _ID);
            if (_data != null)
            {
                MyModel.ID = _data.ID;
                MyModel.Code = _data.Code;
                MyModel.Title = _data.Title;
            }
        }
        public void Delete()
        {
            Source.MsgClass.ClearMessages();
            MsgClass.ClearMessages();

            EditMode = false;
            IsDelete = false;

            if (MyModel.ID > 0)
            {
                if (Enum.TryParse<Tables>(TableName, out var table))
                {
                    var _Row = Source.GetNewRow(table);
                    _Row["ID"] = MyModel.ID;
                    _Row["Code"] = MyModel.Code;
                    _Row["Title"] = MyModel.Title;

                    if (Source.Delete(_Row))
                    {
                        LoadData(TableName);
                        ToastService.ShowSuccess($"{MyModel.Title} successfully Deleted.");
                        return;
                    }
                    else
                    {
                        ToastService.ShowWarning($"{MyModel.Title} fail to be Delete!");
                    }
                }
                else
                {
                    MsgClass.Critical(AppMessages.Enums.Messages.DataTableNotFound);
                }

                MsgClass.AddReange(Source.MsgClass);

            }

            

        }

        #region Save Methods
        public void Save()
        {
            EditMode = false;

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

                    ToastService.ShowSuccess($"{MyModel.Title} successfully Saved.");
                }
                else
                {
                    ToastService.ShowWarning($"{MyModel.Title} fail to be saved.!");
                }
            }
            else
            {
                MsgClass.Critical(AppMessages.Enums.Messages.DataTableNotFound);
            }

            InvokeAsync(StateHasChanged);
        }

        #endregion

        public void BackPage() { AppGlobal.NavManager.NavigateTo("/Menu/Stock/Dictionery"); }

    }

    public class CodeTitleModel
    {
        public long ID { set; get; }
        public string Code { set; get; }
        public string Title { set; get; }
    }


}



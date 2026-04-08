using AppliedDB;
using AppMessages;
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

        }

        public bool Edit(long _ID)
        {
            EditMode = true;
            var _data = StockDirectoryList.Where(e => e.ID == _ID).FirstOrDefault();
            if(_data != null)
            {
                MyModel.ID = _data.ID;
                MyModel.Code = _data.Code;
                MyModel.Title = _data.Title;
            }

            return true;
        }
        public bool Delete(long _ID) { EditMode = true; return true; }
        public void Save() { EditMode = false; InvokeAsync(StateHasChanged); }
        public void BackPage() { AppGlobal.NavManager.NavigateTo("/Menu/Stock"); }

    }

    public class CodeTitleModel
    {
        public long ID { set; get; }
        public string Code { set; get; }
        public string Title { set; get; }
    }


}



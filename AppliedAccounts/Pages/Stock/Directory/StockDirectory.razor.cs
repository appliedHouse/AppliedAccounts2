using AppliedDB;
using AppMessages;
using static AppliedDB.Enums;

namespace AppliedAccounts.Pages.Stock.Directory
{
    public partial class StockDirectory
    {
        public MessageClass MsgClass { get; set; }
        public DataSource Source { get; set; }
        public CodeTitle MyModel { get; set; }
        public string Filter { get; set; }
        public string Sort { get; set; }
        public bool EditMode { get; set; } = false;

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

        public bool Delete(long _ID) { return true; }
        public bool Edit(long _ID)
        {
            EditMode = true;
            MyModel= StockDirectoryList.FirstOrDefault(x => x.ID == _ID)!;
            return true;
        }

        public void Save()
        {

            EditMode = false;
            //return true;

        }

    }

}



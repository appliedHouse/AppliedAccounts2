using AppliedDB;
using AppMessages;
using System.Data;

namespace AppliedAccounts.Pages.Stock.Directory
{
    public partial class StockDirectory
    {
        public MessageClass MsgClass { get; set; }
        public DataSource Source { get; set; }
        public string Filter { get; set; }
        public string Sort { get; set; }
       
        public List<StockDirectoryVM> StockDirectoryList { get; set; }

        public StockDirectory()
        {
            Source = new DataSource(AppGlobal.AppPaths);

            if(TableName == null)
            {
                MsgClass = new MessageClass();
                //MsgClass.Critical(Enums.Messages.DataTableNotFound);
            }
        }


        public void LoadData()
        {
            var _Query = $"SELECT * FROM {TableName}";
            var _Filter = string.Empty ;
            var _Sort = string.Empty ;

            var _Data = Source.GetTable(_Query, _Filter, _Sort).AsEnumerable().ToList();
            foreach (var item in _Data)
            {
                var _item = new StockDirectoryVM();
               

            }
            
        }
    }

    public class StockDirectoryVM
    {

        public long ID { get; set; }
        public string Code { get; set; }
        public string Title { get; set; }


        
    }
}



using AppliedAccounts.Services;
using AppliedAccounts.Data;
using AppliedDB;
using AppMessages;
using System.Data;
using Tables = AppliedDB.Enums.Tables;
using MESSAGE = AppMessages.Enums.Messages;

namespace AppliedAccounts.Models
{
    public class StockListModel
    {
        public GlobalService AppGlobal { get; set; }
        public DataSource Source { get; set; }
        public MessageClass MsgClass { get; set; }
        public int RecordID { get; set; } = 0; // Current record ID
        public RecordModel Record { get; set; } = new(); // List of stock records
        public List<RecordModel> Records { get; set; } = new(); // List of stock records
        public List<RecordModel> FilterRecords { get; set; } = new(); // List of stock records

        public List<CodeTitle> Category { get; set; } = new();
        public List<CodeTitle> SubCategory { get; set; } = new();
        public List<CodeTitle> Packing { get; set; } = new();
        public List<CodeTitle> UOM { get; set; } = new();
        public List<CodeTitle> Size { get; set; } = new();
        public string SearchText { get; set; }

        public BrowseModel BrowseClass { get; set; } = new();

        public StockListModel(GlobalService _AppGlobal)
        {
            AppGlobal = _AppGlobal;
            Source = new(AppGlobal.AppPaths);
            MsgClass = new();
            GetKeys();
            LoadData();
        }

        public void LoadData()
        {
            var _DataList = Source.GetTable(Tables.Inventory);
            Category = Source.GetCodeTitle(Tables.Inv_Category, "Title");
            Packing = Source.GetCodeTitle(Tables.Inv_Packing, "Title");
            SubCategory = Source.GetCodeTitle(Tables.Inv_SubCategory, "Title");
            UOM = Source.GetCodeTitle(Tables.Inv_UOM, "Title");
            Size = Source.GetCodeTitle(Tables.Inv_Size, "Title");
            Records = [.. Source.GetTable(SQLQueries.Quries.Inventory()).AsEnumerable().Select(row => Row2Record(row))];
            FilterRecords = Records;
        }

        #region Add, Edit & Delete
        public void Add()
        {
            Record = new RecordModel
            {
                ID = 0,
                Code = string.Empty,
                Title = string.Empty,
                Qty_Packing = 1,
                Packing = 0,
                UOM = 0,
                Size = 0,
                SubCategory = 0,
                Notes = string.Empty
            };

        }

        public void Edit(int ID)
        {
            try
            {
                Record = Records.FirstOrDefault(r => r.ID == ID) ?? new RecordModel();
                RecordID = Record.ID;
            }
            catch (Exception error)
            {
                MsgClass.Add(error.Message);
            }
        }

        public bool Delete()
        {
            return Source.Delete(Tables.Inventory, Record2Row());

        }
        #endregion

        #region DataRow to Record and Record to DataRow
        private DataRow Record2Row()
        {
            var row = Source.GetNewRow(Tables.Inventory);
            row["ID"] = Record.ID;
            row["Code"] = Record.Code;
            row["Title"] = Record.Title;
            row["Qty_Packing"] = Record.Qty_Packing;
            row["Packing"] = Record.Packing;
            row["UOM"] = Record.UOM;
            row["Size"] = Record.Size;
            row["Category"] = Record.Category;
            row["SubCategory"] = Record.SubCategory;
            row["Notes"] = Record.Notes;

            return row;
        }
        private RecordModel Row2Record(DataRow row)
        {
            AppliedDB.Functions.RemoveNull(row);

            return new RecordModel
            {
                ID = row.Field<int>("ID"),
                Code = row.Field<string>("Code") ?? string.Empty,
                Title = row.Field<string>("Title") ?? string.Empty,
                Qty_Packing = row.Field<int>("Qty_Packing"),
                Packing = row.Field<int>("Packing"),
                UOM = row.Field<int>("UOM"),
                Size = row.Field<int>("Size"),
                SubCategory = row.Field<int>("SubCategory"),
                Notes = row.Field<string>("Notes")!,
                TitleCategory = row.Field<string>("TitleCategory")!,
                TitleSubCategory = row.Field<string>("TitleSubCategory")!,
                TitleUOM = row.Field<string>("TitleUOM")!,
                TitleSize = row.Field<string>("TitleSize")!,
                TitlePacking = row.Field<string>("TitlePacking")!,
            };
        }
        #endregion

        #region Save
        public bool Save()
        {
            var _Row = Record2Row();
            var _Saved = Source.Save(Tables.Inventory, _Row);
            if(!_Saved)
            {
                MsgClass = Source.MyCommands.MyMessages;
            }
            return _Saved;


        }
        #endregion

        #region Get & Set Registry Keys
        public void GetKeys()
        {

        }

        public void SetKeys()
        {

        }
        #endregion

        #region RecordModel
        public class RecordModel
        {
            public int ID { get; set; }
            public string Code { get; set; } = string.Empty;
            public string Title { get; set; } = string.Empty;
            public int Qty_Packing { get; set; }
            public int Packing { get; set; }
            public int UOM { get; set; }
            public int Size { get; set; }
            public int SubCategory { get; set; }
            public int Category { get; set; }
            public string Notes { get; set; }

            public virtual string TitlePacking { get; set; }
            public virtual string TitleUOM { get; set; }
            public virtual string TitleSubCategory { get; set; }
            public virtual string TitleCategory { get; set; }
            public virtual string TitleSize { get; set; }

        }
        #endregion

        #region Search
        public void Search()
        {
            var _SearchText = SearchText;
            if (string.IsNullOrWhiteSpace(_SearchText))
            {
                FilterRecords = Records;
            }
            else
            {
                var Oic = StringComparison.OrdinalIgnoreCase;
                FilterRecords = [.. Records.Where(r =>
                    r.Code.Contains(_SearchText, Oic) ||
                    r.Title.Contains(_SearchText, Oic) ||
                    r.Notes.Contains(_SearchText, Oic) ||
                    r.TitlePacking.Contains(_SearchText, Oic) ||
                    r.TitleUOM.Contains(_SearchText, Oic) ||
                    r.TitleSubCategory.Contains(_SearchText, Oic) ||
                    r.TitleCategory.Contains(_SearchText, Oic)
                )];
            }
        }

        public void ClearSearch()
        {
            SearchText = string.Empty;
            FilterRecords = Records;
        }
        #endregion

    }

}

using AppliedAccounts.Services;
using AppliedAccounts.Data;
using AppliedDB;
using AppMessages;
using System.Data;
using Tables = AppliedDB.Enums.Tables;
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

        public List<CodeTitle> Category { get; set; } = new();
        public List<CodeTitle> SubCategory { get; set; } = new();
        public List<CodeTitle> Packing { get; set; } = new();
        public List<CodeTitle> UOM { get; set; } = new();
        public List<CodeTitle> Size { get; set; } = new();

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
        }


        private void Record2Row()
        {
            DataRow row = Source.GetNewRow(AppliedDB.Enums.Tables.Inventory);
            row["ID"] = Record.ID;
            row["Code"] = Record.Code;
            row["Title"] = Record.Title;
            row["Qty_Packing"] = Record.Qty_Packing;
            row["Packing"] = Record.Packing;
            row["UOM"] = Record.UOM;
            row["Size"] = Record.Size;
            row["SubCategory"] = Record.SubCategory;
            row["Notes"] = Record.Notes;
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
            Record = Records.FirstOrDefault(r => r.ID == ID) ?? new RecordModel();
            RecordID = ID;
            if (RecordID > 0)
            {
                Record2Row();
            }
            else
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

        }

        public void Delete(int ID)
        {
        }
        #endregion

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

        public void Save()
        {
        }


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

    }

}

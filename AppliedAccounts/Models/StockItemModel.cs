using AppliedAccounts.Data;
using AppliedDB;
using System.Data;
using Tables = AppliedDB.Enums.Tables;

namespace AppliedAccounts.Models
{
    public class StockItemModel
    {
        public int ID { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public int PackingQty { get; set; }
        public int Packing { get; set; }
        public int UOM { get; set; }
        public int StockCategory { get; set; }
        public string Description { get; set; } = string.Empty;

        public virtual DataTable TB_Stock { get; set; } = new();
        public virtual DataRow CurrentRow { get; set; }
        public virtual List<Dictionary<int, string>> ListPacking { get; set; } = new();
        public virtual List<Dictionary<int, string>> ListUOM { get; set; } = new();
        public virtual List<Dictionary<int, string>> ListCategories { get; set; } = new();
        public virtual int SelectedPacking { get; set; } = 1;
        public virtual string SelectedUOM { get; set; } = string.Empty;
        public virtual string SelectedCategory { get; set; } = string.Empty;

        public virtual int StockID { get; set; }
        public virtual string DataFile { get; set; } = string.Empty;
        public virtual AppMessages.MessageClass MsgClass { get; set; } = new();

        #region Constructor
        public StockItemModel() { CurrentRow = TB_Stock.NewRow(); }
        public StockItemModel(string _DataFile)
        {
            DataFile = _DataFile;
            TB_Stock = DataSource.GetDataTable(DataFile, Tables.Inventory);
            ListPacking = DataSource.GetDataList(DataFile, Tables.Inv_Packing);
            ListUOM = DataSource.GetDataList(DataFile, Tables.Inv_UOM);
            ListCategories = DataSource.GetDataList(DataFile, Tables.Inv_SubCategory);

            if (TB_Stock.Rows.Count > 0) { CurrentRow = TB_Stock.Rows[0]; } else { CurrentRow = TB_Stock.NewRow(); }
        }
        public StockItemModel(string _DataFile, int? _StockID)
        {
            DataFile = _DataFile;
            TB_Stock = DataSource.GetDataTable(DataFile, Tables.Inventory);
            ListPacking = DataSource.GetDataList(DataFile, Tables.Inv_Packing);
            ListUOM = DataSource.GetDataList(DataFile, Tables.Inv_UOM);
            ListCategories = DataSource.GetDataList(DataFile, Tables.Inv_SubCategory);

            if (TB_Stock.Rows.Count > 0) { CurrentRow = TB_Stock.Rows[0]; } else { CurrentRow = TB_Stock.NewRow(); }

            _StockID ??= 0; StockID = (int)_StockID;
            if (StockID == 0) { GetNewStock(); }
            if (StockID > 0) { GetStockData(StockID); }
        }
        #endregion

        #region Get Stock Data
        public void GetStockData(int _StockID)
        {
            if (TB_Stock == null) { return; }

            StockID = _StockID;
            TB_Stock.DefaultView.RowFilter = $"ID={StockID}";
            if (TB_Stock.DefaultView.Count == 1)
            {
                CurrentRow = TB_Stock.DefaultView[0].Row;
                ID = (int)CurrentRow["ID"];
                Code = (string)CurrentRow["Code"];
                Title = (string)CurrentRow["Title"];
                PackingQty = (int)CurrentRow["Qty_Packing"];
                Packing = (int)CurrentRow["Packing"];
                UOM = (int)CurrentRow["UOM"];
                StockCategory = (int)CurrentRow["SubCategory"];
                Description = (string)CurrentRow["Notes"];
            }
        }
        public void GetNewStock()
        {
            CurrentRow = TB_Stock.NewRow();

            ID = 0;
            Code = string.Empty;
            Title = string.Empty;
            PackingQty = 0;
            Packing = 0;
            UOM = 0;
            StockCategory = 0;
            Description = string.Empty;
        }
        #endregion

        #region Save
        public void Save()
        {
            MsgClass.ClearMessages();

            CurrentRow["ID"] = ID;
            CurrentRow["Code"] = Code;
            CurrentRow["Title"] = Title;
            CurrentRow["Qty_Packing"] = PackingQty;
            CurrentRow["Packing"] = Packing;
            CurrentRow["UOM"] = UOM;
            CurrentRow["SubCategory"] = StockCategory;
            CurrentRow["Notes"] = Description;
            AppFunctions.Save(DataFile, TB_Stock, CurrentRow);
            MsgClass.Add(string.Concat([Code, ",", Title, " : Add Record"]));



        }
        #endregion
    }
}


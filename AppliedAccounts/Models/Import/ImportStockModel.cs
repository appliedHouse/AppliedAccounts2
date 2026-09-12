using AppliedAccounts.Services;
using Microsoft.Data.Sqlite;
using System.Data;
using AppliedDB;
using static AppliedDB.Enums;

namespace AppliedAccounts.Models.Import
{
    public class ImportStockModel
    {
        public GlobalService AppGlobal { get; set; }
        public string ExcelFileName { get; set; } = "";
        public bool IsExcelLoaded { get; set; } = false;
        public bool IsDataLoaded { get; set; } = false;
        public string ExcelImportRegistry { get; set; }
        public DataSource Source { get; set; }

        SqliteConnection TempConnection { get; set; }

        public int SelectedTable { get; set; }
        public List<DataRow> ImportedData { get; set; } = [];
        public List<DataRow> FilterData { get; set; } = [];
        public MessagesService MsgService { get; set; }
        public PageModel Pages { get; set; } = new();

        public List<string> TableNames =>
            [
                Tables.Inventory.ToString(),
                Tables.Inv_Category.ToString(),
                Tables.Inv_SubCategory.ToString(),
                Tables.Inv_UOM.ToString(),
                Tables.Inv_Size.ToString(),
                Tables.Inv_Packing.ToString()
            ];

        private enum tbName
        {
            Inventory,Category,SubCategory,UOM,Size,Packing
        }
        public ImportStockModel() { }
        public ImportStockModel(GlobalService _AppGlobal, MessagesService msgService)
        {
            if (_AppGlobal is not null)
            {
                MsgService = msgService;
                AppGlobal = _AppGlobal;
                ExcelImportRegistry = "ImportStock";              // Get a GUID of DB from registry data
                Source = new(_AppGlobal.AppPaths);
            }
        }

        public void LoadImportedData()
        {
            try
            {
                IsDataLoaded = false;
                string selectedTable = TableNames[0];                                        // Inventory
                string _Path = Source.MyConnections.GetTempDBPath();                         // Connections.GetTempDBPath();                               // Temp DB Path
                string _File = Source.GetText(ExcelImportRegistry);                          // Imported DB File for Stock
                string _ImportDBPath = Path.Combine(_Path, _File + ".db");                   // Connection string Path
                                                                                             //SqliteConnection _TempDBConnection = new($"Data Source={_ImportDBPath}");
                TempConnection = new($"Data Source={_ImportDBPath}");
                ImportedData = [.. DataSource.GetDataTable(selectedTable, TempConnection).AsEnumerable()];
                IsDataLoaded = true;       // Data Successcully loaded.
            }
            catch (Exception error)
            {
                MsgService.Error(error);
            }
        }

        public void ImportInDB()
        {
            if (!ImportUOM())
            {
                MsgService.Critical("Importing of Inventory Unit of Measure has an error..");
                return;
            }

            if (!ImportSize())
            {
                MsgService.Critical("Importing of Inventory Size has an error..");
                return;
            }

            if (!ImportCategory())
            {
                MsgService.Critical("Importing of Inventory Cateogry has an error..");
                return;
            }

            if (!ImportSubCategory())
            {
                MsgService.Critical("Importing of Inventory Sub Category has an error..");
                return;
            }

            if (!ImportPacking())
            {
                MsgService.Critical("Importing of Inventory Packing has an error..");
                return;
            }
        }

        #region Import Tables
        private bool ImportUOM()
        {
            try
            {
                string selectedTable = TableNames[(int)tbName.UOM];            // UOM
                var ImportedUOM = DataSource.GetDataTable(selectedTable, TempConnection);
                var TargetTable = Source.GetTable(Tables.Inv_UOM);
                var TargetList = Source.GetTable(Tables.Inv_UOM).AsEnumerable();
                var NewRow = TargetTable.NewRow();

                if (ImportedUOM.TableName != selectedTable)
                {
                    return false;
                }

                foreach (DataRow Row in ImportedUOM.Rows)
                {
                    NewRow["ID"] = 0;

                    if (TargetList.Any(e => e.Field<long>("ID") == Row.Field<long>("ID")))
                    { NewRow["ID"] = Row["ID"]; }

                    NewRow["Code"] = Row["Code"];
                    NewRow["Title"] = Row["Title"];
                    Source.Save(NewRow);
                }
                return true;
            }
            catch (Exception error)
            {
                MsgService.Error(error);
                return false;
            }


        }
        private bool ImportSize()
        {
            try
            {
                string selectedTable = TableNames[(int)tbName.Size];            // Size
                var ImportedUOM = DataSource.GetDataTable(selectedTable, TempConnection);
                var TargetTable = Source.GetTable(Tables.Inv_Size);
                var TargetList = Source.GetTable(Tables.Inv_Size).AsEnumerable();
                var NewRow = TargetTable.NewRow();

                if (ImportedUOM.TableName != selectedTable)
                {
                    return false;
                }

                foreach (DataRow Row in ImportedUOM.Rows)
                {
                    NewRow["ID"] = 0;

                    if (TargetList.Any(e => e.Field<long>("ID") == Row.Field<long>("ID")))
                    { NewRow["ID"] = Row["ID"]; }

                    NewRow["Code"] = Row["Code"];
                    NewRow["Title"] = Row["Title"];
                    Source.Save(NewRow);
                }
                return true;
            }
            catch (Exception error)
            {
                MsgService.Error(error);
                return false;
            }


        }
        private bool ImportPacking()
        {
            try
            {
                string selectedTable = TableNames[(int)tbName.Packing];            // UOM
                var ImportedUOM = DataSource.GetDataTable(selectedTable, TempConnection);
                var TargetTable = Source.GetTable(Tables.Inv_Packing);
                var TargetList = Source.GetTable(Tables.Inv_Packing).AsEnumerable();
                var NewRow = TargetTable.NewRow();

                if (ImportedUOM.TableName != selectedTable)
                {
                    return false;
                }

                foreach (DataRow Row in ImportedUOM.Rows)
                {
                    NewRow["ID"] = 0;

                    if (TargetList.Any(e => e.Field<long>("ID") == Row.Field<long>("ID")))
                    { NewRow["ID"] = Row["ID"]; }

                    NewRow["Code"] = Row["Code"];
                    NewRow["Title"] = Row["Title"];
                    Source.Save(NewRow);
                }
                return true;
            }
            catch (Exception error)
            {
                MsgService.Error(error);
                return false;
            }


        }
        private bool ImportCategory()
        {
            try
            {
                string selectedTable = TableNames[(int)tbName.Category];            // UOM
                var ImportedUOM = DataSource.GetDataTable(selectedTable, TempConnection);
                var TargetTable = Source.GetTable(Tables.Inv_Category);
                var TargetList = Source.GetTable(Tables.Inv_Category).AsEnumerable();
                var NewRow = TargetTable.NewRow();

                if (ImportedUOM.TableName != selectedTable)
                {
                    return false;
                }

                foreach (DataRow Row in ImportedUOM.Rows)
                {
                    NewRow["ID"] = 0;

                    if (TargetList.Any(e => e.Field<long>("ID") == Row.Field<long>("ID")))
                    { NewRow["ID"] = Row["ID"]; }

                    NewRow["Code"] = Row["Code"];
                    NewRow["Title"] = Row["Title"];
                    Source.Save(NewRow);
                }
                return true;
            }
            catch (Exception error)
            {
                MsgService.Error(error);
                return false;
            }


        }
        private bool ImportSubCategory()
        {
            try
            {
                string selectedTable = TableNames[(int)tbName.SubCategory];            // UOM
                var ImportedUOM = DataSource.GetDataTable(selectedTable, TempConnection);
                var TargetTable = Source.GetTable(Tables.Inv_SubCategory);
                var TargetList = Source.GetTable(Tables.Inv_SubCategory).AsEnumerable();
                var NewRow = TargetTable.NewRow();

                if (ImportedUOM.TableName != selectedTable)
                {
                    return false;
                }

                foreach (DataRow Row in ImportedUOM.Rows)
                {
                    NewRow["ID"] = 0;

                    if (TargetList.Any(e => e.Field<long>("ID") == Row.Field<long>("ID")))
                    { NewRow["ID"] = Row["ID"]; }

                    NewRow["Code"] = Row["Code"];
                    NewRow["Title"] = Row["Title"];
                    Source.Save(NewRow);
                }
                return true;
            }
            catch (Exception error)
            {
                MsgService.Error(error);
                return false;
            }


        }
        #endregion
    }



}

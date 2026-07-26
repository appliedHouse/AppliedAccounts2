using AppliedAccounts.Data;
using AppliedAccounts.Services;
using AppliedDB;
using AppMessages;
using System;
using System.Data;
using Microsoft.Data.Sqlite;

namespace AppliedAccounts.Models
{
    public class ImportCOAModel
    {
        public GlobalService AppGlobal { get; set; }
        public string ExcelFileName { get; set; } = "";
        public bool IsExcelLoaded { get; set; } = false;
        public string ExcelImportRegistry { get; set; }
        public DataSource Source { get; set; }
        public List<string> TableNames { get; } = ["COA", "Nature", "Notes"];
        public string SelectedTable { get; set; }
        public List<DataRow> ImportedData { get; set; } = [];
        public List<DataRow> FilterData { get; set; } = [];
        public MessagesService MsgService { get; set; } 
        public PageModel Pages { get; set; } = new();


        public ImportCOAModel() { }
        public ImportCOAModel(GlobalService _AppGlobal, MessagesService msgService)
        {
            if (_AppGlobal is not null)
            {
                MsgService = msgService;
                AppGlobal = _AppGlobal;
                ExcelImportRegistry = "ImportCOA";              // Get a GUID of DB from registry data
                Source = new(_AppGlobal.AppPaths);
                SelectedTable = TableNames[0];
            }
        }


        public void LoadImportedData()
        {

            string _Path = Source.MyConnections.GetTempDBPath();                         // Connections.GetTempDBPath();                               // Temp DB Path
            string _File = Source.GetText(ExcelImportRegistry);                          // Imported DB File for COA
            string _ImportDBPath = Path.Combine(_Path, _File + ".db");                   // Connection string Path
            SqliteConnection _TempDBConnection = new($"Data Source={_ImportDBPath}");
            ImportedData = [.. DataSource.GetDataTable(SelectedTable, _TempDBConnection).AsEnumerable()];
            IsExcelLoaded = true;       // Data Successcully loaded.
        }
    }
}

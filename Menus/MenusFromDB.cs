using System.Data;
using Microsoft.Data.Sqlite;

namespace Menus
{
    public class MenusFromDB
    {
        public static List<MenuItem> Get()
        {
            return Get2();                  // Temporary return data for development


            try
            {
                var _Menus = new List<MenuItem>();
                var _dbPath = $"{Directory.GetCurrentDirectory()}/wwwroot/System/MenusDB.db";
                var _ConnectionString = $"Data Source={_dbPath}";

                if(!File.Exists(_dbPath))
                {
                    CreateMenuDB _CreateMenuDB = new(_dbPath);
                    _CreateMenuDB.EnsureDatabaseExists();
                }

                var _Connection = new SqliteConnection(_ConnectionString); _Connection.Open();

                var _CommandText = "SELECT * FROM Menus";
                using var _Command = new SqliteCommand(_CommandText, _Connection);
                using var _reader = _Command.ExecuteReader();
                var _DataTable = new DataTable();
                _DataTable.Load(_reader);

                var _Menu1 = new MenuItem();

                foreach (DataRow _Row in _DataTable.Rows)
                {
                    _Menu1 = new MenuItem(); ;
                    _Menu1.ID = (int)_Row.Field<long>("ID");
                    _Menu1.Title = _Row.Field<string>("Title") ?? "";
                    _Menu1.Active = _Row.Field<long>("Active") == 1;
                    _Menu1.Icon = _Row.Field<string>("Icon") ?? "";
                    _Menu1.Level = (int)_Row.Field<long>("Level");
                    _Menu1.ParentID = (int)_Row.Field<long>("ParentID");
                    _Menu1.NavigateTo = _Row.Field<string>("NavigateTo") ?? "";

                    _Menus.Add(_Menu1);
                }
                return _Menus;
            }
            catch (Exception)
            {
                return Get2();
            }
        }

        public static List<MenuItem> Get2()
        {
            var _Menus = new List<MenuItem>();
            _Menus = MenuData.GetDefaultMenus();
            return _Menus;
        }
    }
        
}

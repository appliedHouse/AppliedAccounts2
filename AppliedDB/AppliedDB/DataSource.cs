using System.Data;
using System.Data.SQLite;
using System.Text;
using Tables = AppliedDB.Enums.Tables;
using Query = AppliedDB.Enums.Query;

namespace AppliedDB
{
    public class DataSource
    {
        public AppUserModel? UserProfile { get; set; }
        public SQLiteConnection? MyConnection { get; set; }
        public SQLiteCommand? MyCommand { get; set; }
        public string DBFile => GetDataFile();


        #region Constructor
        public DataSource(AppUserModel _UserProfile)
        {
            UserProfile = _UserProfile;
            if (UserProfile.DataFile.Length > 0)
            {
                MyConnection = Connections.GetClientConnection(UserProfile.DataFile);
                if (MyConnection is not null)
                {
                    MyCommand = new SQLiteCommand(MyConnection);
                }
            }
        }
        #endregion

        #region Get Table
        public DataTable GetTable(Tables _Table)
        {
            if (MyCommand is not null)
            {
                MyCommand.CommandText = $"SELECT * FROM [{_Table}]";
                return GetDataTable(_Table, MyCommand);
            }
            return null;
        }
        public DataTable GetTable(Tables _Table, string _Filter)
        {
            if (MyCommand is not null)
            {
                var _Text = new StringBuilder();
                _Text.Append($"SELECT * FROM[{_Table}] ");
                if (!string.IsNullOrEmpty(_Filter)) { _Text.Append($"WHERE {_Filter} "); }

                MyCommand.CommandText = _Text.ToString(); ;
                return GetDataTable(_Table, MyCommand);
            }
            return null;
        }
        public DataTable GetTable(Tables _Table, string _Filter, string _Sort)
        {
            if (MyCommand is not null)
            {
                var _Text = new StringBuilder();
                _Text.Append($"SELECT * FROM[{_Table}] ");
                if (!string.IsNullOrEmpty(_Filter)) { _Text.Append($"WHERE {_Filter} "); }
                if (!string.IsNullOrEmpty(_Sort)) { _Text.Append($"ORDER BY {_Sort}"); }


                MyCommand.CommandText = _Text.ToString();
                return GetDataTable(_Table, MyCommand);
            }
            return null;
        }
        public DataTable GetTable(Query _SQLQuery)
        {


            try
            {
                if (MyConnection is not null)
                {

                    var _Query = SQLQuery.GetQuery(_SQLQuery);
                    if (_Query.QueryText.Length > 0)
                    {
                        MyConnection.Open();
                        var _Command = new SQLiteCommand(_Query.QueryText, MyConnection);
                        SQLiteDataAdapter _Adapter = new(_Command);
                        DataSet _DataSet = new();
                        _Adapter.Fill(_DataSet, _Query.TableName);
                        MyConnection.Close();

                        if (_DataSet.Tables.Count == 1)
                        {
                            return _DataSet.Tables[0];
                        }
                    }
                }
                return null;
            }
            catch (Exception)
            {
                return new DataTable();
            }
        }
        public DataTable GetTable(Query _SQLQuery, int _ID)
        {
            try
            {
                if (MyConnection is not null)
                {
                    MyConnection.Open();
                    var _Query = SQLQuery.GetQuery(_SQLQuery);
                    var _Command = new SQLiteCommand(_Query.QueryText, MyConnection);
                    _Command.Parameters.AddWithValue("@ID", _ID);
                    SQLiteDataAdapter _Adapter = new(_Command);
                    DataSet _DataSet = new();
                    _Adapter.Fill(_DataSet, _Query.TableName);
                    MyConnection.Close();
                    if (_DataSet.Tables.Count == 1)
                    {
                        return _DataSet.Tables[0];
                    }

                }
                return null;
            }
            catch (Exception)
            {

                return new DataTable();
            }
        }

        private DataTable GetDataTable(Tables _Table, SQLiteCommand _Command)
        {
            try
            {
                if (_Command.Connection is not null)
                {
                    _Command.Connection.Open();
                    string TableName = _Table.ToString();
                    SQLiteDataAdapter _Adapter = new(_Command);
                    DataSet _DataSet = new();
                    _Adapter.Fill(_DataSet, TableName);
                    _Command.Connection.Close();

                    if (_DataSet.Tables.Count == 1)
                    {
                        return _DataSet.Tables[0];
                    }

                }

            }
            catch (Exception)
            {

                return new DataTable();
            }
            return null;
        }
        #endregion

        #region Get Messages Table
        public static DataTable Messages()
        {
            var DefaultLanguage = 1;
            return Messages(DefaultLanguage);
        }
        public static DataTable Messages(int _Language)
        {

            var _Connection = Connections.GetMessagesConnection();
            if (_Connection is not null)
            {

                _Connection.Open();
                var _Command = new SQLiteCommand($"SELECT * FROM [Messages] WHERE Language={_Language}", _Connection);
                var _Adapter = new SQLiteDataAdapter(_Command);
                var _DataSet = new DataSet();
                _Adapter.Fill(_DataSet, "Messages");
                _Connection.Close();
                if (_DataSet.Tables.Count > 0)
                {
                    return _DataSet.Tables[0];
                }
            }
            return null;
        }
        #endregion

        #region Get List
        public static List<DataRow> GetList(string DBFile, Tables _Table)
        {
            var Table = GetDataTable(DBFile, _Table);
            return Table.AsEnumerable().ToList();
        }
        public static List<DataRow> GetList(string DBFile, Query Query)
        {
            QueryClass _QueryClass = SQLQuery.GetQuery(Query);
            var Table = GetDataTable(DBFile, _QueryClass.QueryText, _QueryClass.TableName);
            if (Table is not null)
            {
                return Table.AsEnumerable().ToList();
            }
            return new();
        }

        public List<DataRow> GetList(Query Query)
        {
            if (UserProfile is not null)
            {
                QueryClass _QueryClass = SQLQuery.GetQuery(Query);
                var Table = GetDataTable(UserProfile.DataFile, _QueryClass.QueryText, _QueryClass.TableName);
                if (Table is not null)
                {
                    return Table.AsEnumerable().ToList();
                }
            }
            return new();
        }

        #endregion

        #region Seek
        public DataRow Seek(Tables _Table, int ID)
        {

            var _DataTable = GetTable(_Table);
            if (_DataTable is not null)
            {
                var _DataRow = _DataTable.NewRow();
                _DataTable.DefaultView.RowFilter = $"ID={ID}";
                if (_DataTable.DefaultView.Count == 1)
                {
                    _DataRow = _DataTable.DefaultView[0].Row;
                }
                else
                {
                    _DataRow = _DataTable.NewRow();
                }

                _DataTable.Dispose();
                return _DataRow;
            }
            return null;
        }


        public string SeekTitle(Tables _Table, int ID)
        {
            var _DataTable = GetTable(_Table);
            if (_DataTable is not null)
            {
                var _Title = string.Empty;
                _DataTable.DefaultView.RowFilter = $"ID={ID}";
                if (_DataTable.DefaultView.Count == 1)
                {
                    _Title = _DataTable.DefaultView[0]["Title"].ToString();
                }
                _DataTable.Dispose();
                if (_Title is null) { return string.Empty; }
                return _Title;
            }
            return "";
        }

        public decimal SeekTaxRate(int ID)
        {
            var _TaxRate = 0.00M;
            var _DataRow = Seek(Tables.Taxes, ID);

            if (_DataRow != null)
            {
                _TaxRate = (decimal)_DataRow["Rate"];
            }
            _DataRow = null;

            return _TaxRate;

        }
        #endregion

        #region Get Code title List of various.
        public List<CodeTitle> GetCustomers()
        {
            return GetCustomers("Title");
        }
        public List<CodeTitle> GetCustomers(string? _Sort)
        {
            _Sort ??= "Title";
            var _Table = GetTable(Tables.Customers, "", "Title");
            if (_Table is not null)
            {
                var _CodeTitle = new CodeTitle();
                var _CodeTitleList = new List<CodeTitle>();

                if (_Table.Rows.Count > 0)
                {
                    foreach (DataRow Row in _Table.Rows)
                    {
                        if (Row["ID"] == null) { Row["ID"] = 0; }
                        if (Row["Code"] == null) { Row["Code"] = string.Empty; }
                        if (Row["Title"] == null) { Row["Title"] = string.Empty; }

                        _CodeTitle = new();
                        _CodeTitle.ID = (int)Row["ID"];
                        _CodeTitle.Code = (string)Row["Code"];
                        _CodeTitle.Title = (string)Row["Title"];

                        _CodeTitleList.Add(_CodeTitle);
                    }
                }

                _Table.Dispose(); _Table = null;
                return _CodeTitleList;
            }

            return new();
        }
        public List<CodeTitle> GetEmployees()
        {
            return GetEmployees("Title");
        }
        public List<CodeTitle> GetEmployees(string? _Sort)
        {
            _Sort ??= "Title";
            var _Table = GetTable(Tables.Employees, "", "Title");
            if (_Table is not null)
            {
                var _CodeTitle = new CodeTitle();
                var _CodeTitleList = new List<CodeTitle>();

                if (_Table.Rows.Count > 0)
                {
                    foreach (DataRow Row in _Table.Rows)
                    {
                        if (Row["ID"] == null) { Row["ID"] = 0; }
                        if (Row["Code"] == null) { Row["Code"] = string.Empty; }
                        if (Row["Title"] == null) { Row["Title"] = string.Empty; }

                        _CodeTitle = new();
                        _CodeTitle.ID = (int)Row["ID"];
                        _CodeTitle.Code = (string)Row["Code"];
                        _CodeTitle.Title = (string)Row["Title"];

                        _CodeTitleList.Add(_CodeTitle);
                    }
                }

                _Table.Dispose(); _Table = null;
                return _CodeTitleList;
            }
            return new();
        }
        public List<CodeTitle> GetProjects()
        {
            return GetProjects("Title");
        }
        public List<CodeTitle> GetProjects(string? _Sort)
        {
            _Sort ??= "Title";
            var _Table = GetTable(Tables.Project, "", "Title");
            if (_Table is not null)
            {
                var _CodeTitle = new CodeTitle();
                var _CodeTitleList = new List<CodeTitle>();

                if (_Table.Rows.Count > 0)
                {
                    foreach (DataRow Row in _Table.Rows)
                    {
                        if (Row["ID"] == null) { Row["ID"] = 0; }
                        if (Row["Code"] == null) { Row["Code"] = string.Empty; }
                        if (Row["Title"] == null) { Row["Title"] = string.Empty; }

                        _CodeTitle = new();
                        _CodeTitle.ID = (int)Row["ID"];
                        _CodeTitle.Code = (string)Row["Code"];
                        _CodeTitle.Title = (string)Row["Title"];

                        _CodeTitleList.Add(_CodeTitle);
                    }
                }

                _Table.Dispose(); _Table = null;
                return _CodeTitleList;
            }

            return new();

        }
        public List<CodeTitle> GetAccounts()
        {
            return GetAccounts("Title");
        }
        public List<CodeTitle> GetAccounts(string? _Sort)
        {
            _Sort ??= "Title";
            var _Table = GetTable(Tables.COA, "", "Title");
            if (_Table is not null)
            {
                var _CodeTitle = new CodeTitle();
                var _CodeTitleList = new List<CodeTitle>();

                if (_Table.Rows.Count > 0)
                {
                    foreach (DataRow Row in _Table.Rows)
                    {
                        if (Row["ID"] == null) { Row["ID"] = 0; }
                        if (Row["Code"] == null) { Row["Code"] = string.Empty; }
                        if (Row["Title"] == null) { Row["Title"] = string.Empty; }

                        _CodeTitle = new();
                        _CodeTitle.ID = (int)Row["ID"];
                        _CodeTitle.Code = (string)Row["Code"];
                        _CodeTitle.Title = (string)Row["Title"];

                        _CodeTitleList.Add(_CodeTitle);
                    }
                }

                _Table.Dispose(); _Table = null;
                return _CodeTitleList;
            }

            return new();

        }
        public List<CodeTitle> GetInventory()
        {
            return GetInventory("Title");
        }
        public List<CodeTitle> GetInventory(string? _Sort)
        {
            _Sort ??= "Title";
            var _Table = GetTable(Tables.Inventory, "", "Title");
            if (_Table is not null)
            {
                var _CodeTitle = new CodeTitle();
                var _CodeTitleList = new List<CodeTitle>();

                if (_Table.Rows.Count > 0)
                {
                    foreach (DataRow Row in _Table.Rows)
                    {
                        if (Row["ID"] == null) { Row["ID"] = 0; }
                        if (Row["Code"] == null) { Row["Code"] = string.Empty; }
                        if (Row["Title"] == null) { Row["Title"] = string.Empty; }

                        _CodeTitle = new();
                        _CodeTitle.ID = (int)Row["ID"];
                        _CodeTitle.Code = (string)Row["Code"];
                        _CodeTitle.Title = (string)Row["Title"];

                        _CodeTitleList.Add(_CodeTitle);
                    }
                }

                _Table.Dispose(); _Table = null;
                return _CodeTitleList;
            }

            return new();

        }
        public List<CodeTitle> GetTaxes()
        {
            return GetTaxes("Title");
        }
        public List<CodeTitle> GetTaxes(string? _Sort)
        {
            _Sort ??= "Title";
            var _Table = GetTable(Tables.Taxes, "", "Title");
            if (_Table is not null)
            {
                var _CodeTitle = new CodeTitle();
                var _CodeTitleList = new List<CodeTitle>();

                if (_Table.Rows.Count > 0)
                {
                    foreach (DataRow Row in _Table.Rows)
                    {
                        if (Row["ID"] == null) { Row["ID"] = 0; }
                        if (Row["Code"] == null) { Row["Code"] = string.Empty; }
                        if (Row["Title"] == null) { Row["Title"] = string.Empty; }

                        _CodeTitle = new();
                        _CodeTitle.ID = (int)Row["ID"];
                        _CodeTitle.Code = (string)Row["Code"];
                        _CodeTitle.Title = (string)Row["Title"];

                        _CodeTitleList.Add(_CodeTitle);
                    }
                }

                _Table.Dispose(); _Table = null;
                return _CodeTitleList;
            }

            return new();
        }
        public List<CodeTitle> GetUnits()
        {
            return GetUnits("Title");
        }
        public List<CodeTitle> GetUnits(string? _Sort)
        {
            _Sort ??= "Title";
            var _Table = GetTable(Tables.Inv_UOM, "", "Title");
            if (_Table is not null)
            {
                var _CodeTitle = new CodeTitle();
                var _CodeTitleList = new List<CodeTitle>();

                if (_Table.Rows.Count > 0)
                {
                    foreach (DataRow Row in _Table.Rows)
                    {
                        if (Row["ID"] == null) { Row["ID"] = 0; }
                        if (Row["Code"] == null) { Row["Code"] = string.Empty; }
                        if (Row["Title"] == null) { Row["Title"] = string.Empty; }

                        _CodeTitle = new();
                        _CodeTitle.ID = (int)Row["ID"];
                        _CodeTitle.Code = (string)Row["Code"];
                        _CodeTitle.Title = (string)Row["Title"];

                        _CodeTitleList.Add(_CodeTitle);
                    }
                }

                _Table.Dispose(); _Table = null;
                return _CodeTitleList;
            }

            return new();

        }
        #endregion

        #region Get Title form Code Title List
        public string GetTitle(List<CodeTitle> _List, int _ID)
        {
            foreach (CodeTitle Item in _List)
            {
                if (Item.ID.Equals(_ID))
                {
                    return Item.Title;
                }

            }
            return string.Empty;
        }
        #endregion

        #region Static DataTable Methods
        public static DataTable GetDataTable(string DBFile, Tables _Table)
        {
            string TableName = _Table.ToString();
            return GetDataTable(DBFile, TableName);
        }
        public static DataTable GetDataTable(string DBFile, string _Table)
        {
            if (DBFile == null) { return new(); }
            if (DBFile.Length == 0) { return new(); }
            //========================================================


            try
            {
                string TableName = _Table.ToString();
                var _CommandText = $"SELECT * FROM [{TableName}]";
                var _Connection = Connections.GetClientConnection(DBFile);
                _Connection.Open();
                SQLiteCommand _Command = new(_CommandText, _Connection);
                SQLiteDataAdapter _Adapter = new(_Command);
                DataSet _DataSet = new();
                _Adapter.Fill(_DataSet, TableName);
                _Connection.Close();

                if (_DataSet.Tables.Count == 1)
                {
                    _Command.Dispose();
                    _Adapter.Dispose();
                    return _DataSet.Tables[0];
                }
                else
                {
                    return new DataTable();
                }
            }
            catch (SQLiteException)
            {
                return new DataTable();
            }

            catch (Exception)
            {

                return new DataTable();
            }

        }
        public static DataTable GetDataTable(string DBFile, string _Query, string _TableName)
        {
            if (DBFile == null) { return new(); }
            if (DBFile.Length == 0) { return new(); }
            //========================================================

            DataTable _DataTable;
            var _CommandText = _Query;
            var _Connection = Connections.GetClientConnection(DBFile);
            if (_Connection is not null)
            {

                _Connection.Open();
                SQLiteCommand _Command = new(_CommandText, _Connection);
                SQLiteDataAdapter _Adapter = new(_Command);
                DataSet _DataSet = new();
                _Adapter.Fill(_DataSet, _TableName);
                _Connection.Close();

                if (_DataSet.Tables.Count == 1)
                {
                    _DataTable = _DataSet.Tables[0];
                }
                else
                {
                    _DataTable = new DataTable();
                }

                _Command.Dispose();
                _Adapter.Dispose();
                _DataSet.Dispose();

                return _DataTable;
            }
            return null;
        }
        public static List<Dictionary<int, string>> GetDataList(string DBFile, Tables _Table)
        {
            if (DBFile == null) { return new(); }
            if (DBFile.Length == 0) { return new(); }
            //========================================================

            List<Dictionary<int, string>> _List = new();
            try
            {
                string TableName = _Table.ToString();
                var _CommandText = $"SELECT * FROM [{TableName}]";
                var _Connection = Connections.GetClientConnection(DBFile);
                if (_Connection is not null)
                {

                    _Connection.Open();
                    SQLiteCommand _Command = new(_CommandText, _Connection);
                    SQLiteDataAdapter _Adapter = new(_Command);
                    DataSet _DataSet = new();
                    _Adapter.Fill(_DataSet, TableName);
                    _Connection.Close();

                    if (_DataSet.Tables.Count == 1)
                    {
                        var _DataTable = _DataSet.Tables[0];

                        foreach (DataRow Row in _DataTable.Rows)
                        {
                            Dictionary<int, string> _item = new() { { (int)Row["id"], (string)Row["Title"] } };
                            _List.Add(_item);
                        }

                        _Command.Dispose();
                        _Adapter.Dispose();
                        _DataSet.Dispose();

                        return _List;
                    }
                    else
                    {
                        return _List;
                    }
                }
                return null;
            }
            catch (Exception)
            {

                return _List;
            }
        }
        public static int GetMaxID(string DBFile, string _Table)
        {
            DataTable _DataTable = GetDataTable(DBFile, _Table);
            if (_DataTable.Rows.Count == 0) { return 1; }
            int _MaxID = (int)_DataTable.Compute("MAX(ID)", "") + 1;
            _DataTable.Dispose();
            return _MaxID;

        }
        #endregion

        #region Delete Row
        public bool Delete(Tables _Table, DataRow _Row)
        {
            // Mode this codes to CommandClass...


            //var SQLCommands = new CommandClass(_Row, DBFile);
            //var Deleted = SQLCommands.CommandDelete?.ExecuteNonQuery();
            //if (Deleted is not null)
            //{
            //    if (Deleted > 0)
            //    {
            //        return true;
            //    }
            //}
            return false;
        }
        #endregion

        #region Get NewRow of Table
        public static DataRow GetNewRow(string DBFile, Tables _Table)
        {
            var _DataTable = GetDataTable(DBFile, _Table);
            if (_DataTable is not null)
            {
                var _NewRow = _DataTable.NewRow();
                _DataTable.Dispose();
                return _NewRow;
            }
            return null;
        }


        public DataRow GetNewRow(Tables _Table)
        {
            return GetNewRow(DBFile, _Table);
        }

        #endregion

        #region Get Data File Name
        private string GetDataFile()
        {
            if (UserProfile is not null)
            {
                return UserProfile.DataFile;
            }
            return "";
        }

        #endregion
    }
    public class CodeTitle
    {
        public int ID { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
    }
}

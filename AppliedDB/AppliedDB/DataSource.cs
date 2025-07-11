using AppliedGlobals;
using System.Data;
using System.Data.SQLite;
using System.Text;
using static AppliedDB.Enums;
using static AppliedGlobals.AppErums;
using Tables = AppliedDB.Enums.Tables;

namespace AppliedDB
{
    public class DataSource
    {
        public AppValues.AppPath AppPaths { get; set; }
        public SQLiteConnection MyConnection { get; set; }
        public SQLiteCommand MyCommand { get; set; }
        public CommandClass MyCommands { get; set; } = new();
        public string DBFile => GetDataFile();
        public string ErrorMessage { get; set; }


        #region Constructor

        public DataSource(AppValues.AppPath _AppPaths)
        {
            AppPaths = _AppPaths;
            var _Connection = new Connections(AppPaths);
            MyConnection = _Connection.GetSQLiteClient()!;               // Get a connection of Client

            if (MyConnection is not null)
            {
                MyCommand = new SQLiteCommand(MyConnection);
            }
        }


        //public DataSource(AppUserModel _UserProfile)
        //{
        //    UserProfile = _UserProfile;
        //    var _Connection = new Connections(_UserProfile);
        //    MyConnection = _Connection.GetSQLiteClient()!;               // Get a connection of Client

        //    if (MyConnection is not null)
        //    {
        //        MyCommand = new SQLiteCommand(MyConnection);
        //    }

        //}

        #endregion

        #region Get Table

        #region Get Table by AppliedDB.Enum.Tables
        public async Task<DataTable> GetTableAsync(Tables _Table)
        {
            if (MyCommand is not null)
            {
                MyCommand.CommandText = $"SELECT * FROM [{_Table}]";
                return await Task.Run(() => GetDataTable(_Table, MyCommand)); ;
            }
            return new DataTable();
        }
        public DataTable GetTable(Tables _Table)
        {
            if (MyCommand is not null)
            {
                MyCommand.CommandText = $"SELECT * FROM [{_Table}]";
                return GetDataTable(_Table, MyCommand);
            }
            return new DataTable();
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
            return new DataTable();
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
            return new DataTable();
        }
        #endregion

        #region Get Table by AppliedDB.Enum.Query
        public DataTable GetTable(Query _SQLQuery)
        {

            try
            {
                if (MyConnection is not null)
                {
                    var _Query = SQLQuery.GetQuery(_SQLQuery);
                    if (!string.IsNullOrWhiteSpace(_Query.QueryText))
                    {
                        if (MyConnection.State != ConnectionState.Open) { MyConnection.Open(); }
                        using var _Command = new SQLiteCommand(_Query.QueryText, MyConnection);
                        using var _Adapter = new SQLiteDataAdapter(_Command);
                        using var _DataSet = new DataSet();

                        _Adapter.Fill(_DataSet, _Query.TableName);
                        if (MyConnection.State == ConnectionState.Open) { MyConnection.Close(); }

                        if (_DataSet.Tables.Count == 1)
                        {
                            _Adapter.Dispose();
                            _Command.Dispose();
                            return _DataSet.Tables[0];
                        }
                    }
                }
                return new DataTable();
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
                    if (MyConnection.State != ConnectionState.Open) { MyConnection.Open(); }
                    var _Query = SQLQuery.GetQuery(_SQLQuery);
                    using var _Command = new SQLiteCommand(_Query.QueryText, MyConnection);
                    _Command.Parameters.AddWithValue("@ID", _ID);
                    using var _Adapter = new SQLiteDataAdapter(_Command);
                    using var _DataSet = new DataSet();
                    _Adapter.Fill(_DataSet, _Query.TableName);
                    if (MyConnection.State == ConnectionState.Open) { MyConnection.Close(); }
                    if (_DataSet.Tables.Count == 1)
                    {
                        return _DataSet.Tables[0];
                    }

                }
                return new DataTable();
            }
            catch (Exception)
            {

                return new DataTable();
            }
        }
        #endregion

        #region Get Table by string
        public DataTable GetTable(string _SQLQuery)
        {
            return GetTable(_SQLQuery, "", "");
        }
        public DataTable GetTable(string _SQLQuery, string _Filter)
        {
            return GetTable(_SQLQuery, _Filter, "");
        }
        public DataTable GetTable(string _SQLQuery, string _Filter, string _Sort)
        {
            try
            {
                if (!string.IsNullOrEmpty(_SQLQuery))
                    if (MyConnection is not null)
                    {
                        if (MyConnection.State != ConnectionState.Open) { MyConnection.Open(); }
                        var _Command = new SQLiteCommand(_SQLQuery, MyConnection);
                        if (!string.IsNullOrEmpty(_Filter)) { _Command.CommandText += $" WHERE {_Filter}"; }
                        if (!string.IsNullOrEmpty(_Sort)) { _Command.CommandText += $" ORDER BY {_Sort}"; }
                        var _Adapter = new SQLiteDataAdapter(_Command);
                        var _DataSet = new DataSet();
                        _Adapter.Fill(_DataSet, (Guid.NewGuid()).ToString());
                        if (MyConnection.State == ConnectionState.Open) { MyConnection.Close(); }
                        if (_DataSet.Tables.Count == 1)
                        {
                            return _DataSet.Tables[0];
                        }
                    }
                return new DataTable();
            }
            catch (Exception)
            {
                return new DataTable();
            }
        }
        #endregion

        #region Get Table Static

        public static DataTable GetDataTable(Tables _Table, SQLiteCommand _Command)
        {
            try
            {
                if (_Command.Connection is not null)
                {
                    if (_Command.Connection.State != ConnectionState.Open) { _Command.Connection.Open(); }
                    string TableName = _Table.ToString();
                    using var _Adapter = new SQLiteDataAdapter(_Command);
                    using var _DataSet = new DataSet();
                    _Adapter.Fill(_DataSet, TableName);
                    if (_Command.Connection.State == ConnectionState.Open) { _Command.Connection.Close(); }

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
            return new DataTable(); ;
        }
        public static DataTable GetDataTable(Tables _Table, SQLiteConnection _Connection)
        {
            return GetDataTable(_Table.ToString(), _Connection);

        }
        public static DataTable GetDataTable(string _Table, SQLiteConnection _Connection)
        {
            try
            {
                if (_Connection is not null)
                {
                    if (_Connection.State != ConnectionState.Open) { _Connection.Open(); }
                    string TableName = _Table.ToString();
                    string CommText = $"SELECT * FROM [{_Table}]";
                    using var _Adapter = new SQLiteDataAdapter(CommText, _Connection);
                    using var _DataSet = new DataSet();
                    _Adapter.Fill(_DataSet, TableName);
                    if (_Connection.State == ConnectionState.Open) { _Connection.Close(); }

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
            return new DataTable();

        }
        public static DataTable GetQueryTable(string _SQLQuery, SQLiteConnection _Connection)
        {
            try
            {
                if (_Connection is not null)
                {
                    if (!string.IsNullOrEmpty(_SQLQuery))
                    {

                        if (_Connection.State != ConnectionState.Open) { _Connection.Open(); }
                        //var _Query = SQLQuery.GetQuery(_SQLQuery);
                        var _Command = new SQLiteCommand(_SQLQuery, _Connection);
                        using var _Adapter = new SQLiteDataAdapter(_Command);
                        using var _DataSet = new DataSet();
                        _Adapter.Fill(_DataSet, (new Guid()).ToString());
                        if (_Connection.State == ConnectionState.Open) { _Connection.Close(); }
                        if (_DataSet.Tables.Count == 1)
                        {
                            return _DataSet.Tables[0];
                        }
                    }
                }
                return new DataTable();
            }
            catch (Exception)
            {

                return new DataTable();
            }
        }

        public static DataTable GetDataTable(Tables _Table, SQLiteConnection _Connection, string _Filter)
        {
            try
            {
                if (_Connection is not null)
                {

                    var _Query = $"SELECT * FROM {_Table} ";
                    if (_Filter.Length > 0) { _Query += $"WHERE {_Filter}"; }

                    var _Command = new SQLiteCommand(_Query, _Connection);
                    using var _Adapter = new SQLiteDataAdapter(_Command);
                    using var _DataSet = new DataSet();

                    if (_Connection.State != ConnectionState.Open) { _Connection.Open(); }
                    _Adapter.Fill(_DataSet, _Table.ToString());
                    if (_Connection.State == ConnectionState.Open) { _Connection.Close(); }
                   
                    if (_DataSet.Tables.Count == 1)
                    {
                        return _DataSet.Tables[0];
                    }
                }
                return new DataTable();
            }
            catch (Exception)
            {

                return new DataTable();
            }
        }
        #endregion
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
                if (_Connection.State != ConnectionState.Open) { _Connection.Open(); }
                using var _Command = new SQLiteCommand($"SELECT * FROM [Messages] WHERE Language={_Language}", _Connection);
                using var _Adapter = new SQLiteDataAdapter(_Command);
                using var _DataSet = new DataSet();
                _Adapter.Fill(_DataSet, "Messages");
                if (_Connection.State == ConnectionState.Open) { _Connection.Close(); }
                if (_DataSet.Tables.Count > 0)
                {
                    return _DataSet.Tables[0];
                }
            }
            return new DataTable();
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
            var _QueryClass = SQLQuery.GetQuery(Query);
            var Table = GetDataTable(DBFile, _QueryClass.QueryText, _QueryClass.TableName);
            if (Table is not null)
            {
                return Table.AsEnumerable().ToList();
            }
            return new();
        }

        public List<DataRow> GetList(Query Query)
        {
            if (AppPaths is not null)
            {
                var _QueryClass = SQLQuery.GetQuery(Query);
                var Table = GetDataTable(AppPaths.DBFile, _QueryClass.QueryText, _QueryClass.TableName);
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
            if (ID > 0)
            {
                var _DataTable = GetTable(_Table).AsEnumerable().ToList();
                if (_DataTable.Count > 0)
                {
                    var _DataRow = _DataTable.Where(rows => rows.Field<int>("ID") == ID).First();
                    return _DataRow;
                }
            }
            return null;

        }

        public List<DataRow> List(Tables _Table, int ID)
        {
            var _DataTable = GetTable(_Table).AsEnumerable().ToList();
            var _DataRow = _DataTable.Where(rows => rows.Field<int>("ID") == ID).ToList();
            return _DataRow;
        }

        public object? SeekValue(Tables _Table, int _ID, string _column)
        {
            // _Table  => Table Enums.table
            // _ID     => ID primary key for search record
            // _column => Column Name for search value

            var TableList = GetTable(_Table).AsEnumerable().ToList().Where(rows => rows.Field<int>("ID") == _ID).SingleOrDefault();

            if (TableList != null)
            {
                return TableList[_column];

            }

            return null;

        }

        public string SeekTitle(Tables _Table, int ID)
        {
            DataTable _DataTable = GetTable(_Table);

            if (_DataTable is null)
                return string.Empty;

            try
            {
                var _Title = _DataTable.AsEnumerable()
                       .Where(row => row.Field<int>("ID") == ID)
                       .Select(row => row.Field<string>("Title"))
                       .First() ?? string.Empty;

                return _Title;
            }
            finally
            {
                _DataTable.Dispose();
            }

        }

        public decimal SeekTaxRate(int ID)
        {
            var _TaxRate = 0.00M;
            var _DataRow = Seek(Tables.Taxes, ID);

            if (_DataRow != null)
            {
                _TaxRate = (decimal)_DataRow["Rate"];
            }


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
            return GetCodeTitle(Tables.Customers, _Sort);
        }
        public List<CodeTitle> GetEmployees()
        {
            return GetEmployees("Title");
        }
        public List<CodeTitle> GetEmployees(string? _Sort)
        {
            _Sort ??= "Title";
            return GetCodeTitle(Tables.Employees, _Sort);
        }
        public List<CodeTitle> GetProjects()
        {
            return GetProjects("Title");
        }
        public List<CodeTitle> GetProjects(string? _Sort)
        {
            _Sort ??= "Title";
            return GetCodeTitle(Tables.Project, _Sort);
        }
        public List<CodeTitle> GetAccounts()
        {
            return GetAccounts("Title");
        }
        public List<CodeTitle> GetAccounts(string? _Sort)
        {
            _Sort ??= "Title";
            return GetCodeTitle(Tables.COA, _Sort);
        }
        public List<CodeTitle> GetInventory()
        {
            return GetInventory("Title");
        }
        public List<CodeTitle> GetInventory(string? _Sort)
        {
            _Sort ??= "Title";
            return GetCodeTitle(Tables.Inventory, _Sort);
        }
        public List<CodeTitle> GetTaxes()
        {
            return GetTaxes("Title");
        }
        public List<CodeTitle> GetTaxes(string? _Sort)
        {
            _Sort ??= "Title";
            return GetCodeTitle(Tables.Taxes, _Sort);
        }
        public List<CodeTitle> GetUnits()
        {
            return GetCodeTitle(Tables.Inv_UOM, "Title");
        }
        public List<CodeTitle> GetUnits(string? _Sort)
        {
            _Sort ??= "Title";
            return GetCodeTitle(Tables.Inv_UOM, _Sort);
        }
        public List<CodeTitle> GetInvoices()
        {
            // Generate Unpaid invoices to show in Receipt Page..... it is pending now.
            return new();
        }
        public List<CodeTitle> GetAccClass()
        {
            return GetCodeTitle(Tables.COA_Class, "Title");
        }
        public List<CodeTitle> GetAccClass(string? _Sort)
        {
            _Sort ??= "Title";
            return GetCodeTitle(Tables.COA_Class, _Sort);
        }
        public List<CodeTitle> GetAccNature()
        {
            return GetCodeTitle(Tables.COA_Nature, "Title");
        }
        public List<CodeTitle> GetAccNature(string? _Sort)
        {
            _Sort ??= "Title";
            return GetCodeTitle(Tables.COA_Nature, _Sort);
        }
        public List<CodeTitle> GetAccNotes()
        {
            return GetCodeTitle(Tables.COA_Notes, "Title");
        }
        public List<CodeTitle> GetAccNotes(string? _Sort)
        {
            _Sort ??= "Title";
            return GetCodeTitle(Tables.COA_Notes, _Sort);
        }


        #region Getting Code and Title for all tables
        public List<CodeTitle> GetCodeTitle(Tables _DataTable, string? _Sort)
        {
            // General method for obtain Code and Title for all direcotrues. like account,client, cusotmer, class, nature etc...
            _Sort ??= "Title";
            var _Table = GetTable(_DataTable, "", "Title");
            if (_Table is not null)
            {
                var _CodeTitle = new CodeTitle();
                var _CodeTitleList = new List<CodeTitle>();

                if (_Table.Rows.Count > 0)
                {
                    _CodeTitleList.Add(new CodeTitle()
                    {
                        ID = 0,
                        Code = "Top",
                        Title = "Select...."
                    });

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


        public static List<CodeTitle> GetCodeTitle(string _Table, SQLiteConnection DBConnection)
        {
            var _Sort = "Title";
            var _DataTable = GetDataTable(_Table, DBConnection);
            _DataTable.DefaultView.Sort = _Sort;
            if (_Table is not null)
            {
                var _CodeTitle = new CodeTitle();
                var _CodeTitleList = new List<CodeTitle>();

                if (_DataTable.Rows.Count > 0)
                {
                    _CodeTitleList.Add(new CodeTitle()
                    {
                        ID = 0,
                        Code = "Top",
                        Title = "Select...."
                    });

                    foreach (DataRow Row in _DataTable.DefaultView.ToTable().Rows)
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

                _DataTable.Dispose(); _Table = null;
                return _CodeTitleList;
            }

            return new();

        }
        #endregion

        #region Get Title form Code Title List
        public static string GetTitle(List<CodeTitle> _List, int _ID)
        {
            var _Title = _List.Where(l => l.ID == _ID).Select(l => l.Title).ToString();
            if (_Title is null) { return string.Empty; }
            return _Title;
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
                if (_Connection != null)
                {
                    if (_Connection.State != ConnectionState.Open) { _Connection.Open(); }
                    SQLiteCommand _Command = new(_CommandText, _Connection);
                    SQLiteDataAdapter _Adapter = new(_Command);
                    DataSet _DataSet = new();
                    _Adapter.Fill(_DataSet, TableName);
                    if (_Connection.State == ConnectionState.Open) { _Connection.Close(); }

                    if (_DataSet.Tables.Count == 1)
                    {
                        _Command.Dispose();
                        _Adapter.Dispose();
                        return _DataSet.Tables[0];
                    }
                }
            }
            catch (Exception)
            {

            }

            return new DataTable();
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

                if (_Connection.State != ConnectionState.Open) { _Connection.Open(); }
                SQLiteCommand _Command = new(_CommandText, _Connection);
                SQLiteDataAdapter _Adapter = new(_Command);
                DataSet _DataSet = new();
                _Adapter.Fill(_DataSet, _TableName);
                if (_Connection.State == ConnectionState.Open) { _Connection.Close(); }

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

                    if (_Connection.State != ConnectionState.Open) { _Connection.Open(); }
                    SQLiteCommand _Command = new(_CommandText, _Connection);
                    SQLiteDataAdapter _Adapter = new(_Command);
                    DataSet _DataSet = new();
                    _Adapter.Fill(_DataSet, TableName);
                    if (_Connection.State == ConnectionState.Open) { _Connection.Close(); }

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

        #endregion

        #region Maximum ID of the Table


        public static int GetMaxID(string DBFile, string _Table)
        {
            DataTable _DataTable = GetDataTable(DBFile, _Table);
            if (_DataTable.Rows.Count == 0) { return 1; }
            int _MaxID = (int)_DataTable.Compute("MAX(ID)", "") + 1;
            _DataTable.Dispose();
            return _MaxID;

        }

        public static int GetMaxID(string _Table, SQLiteConnection DBConnection)
        {
            DataTable _DataTable = GetDataTable(_Table, DBConnection);
            if (_DataTable.Rows.Count == 0) { return 1; }
            int _MaxID = (int)_DataTable.Compute("MAX(ID)", "") + 1;
            _DataTable.Dispose();
            return _MaxID;

        }
        #endregion

        #region Delete Row
        public bool Delete(Tables _Table, DataRow _Row)
        {
            var _DataTable = GetTable(_Table);
            var _NewRow = _DataTable.NewRow();
            var _RowArray = _Row.ItemArray;

            _NewRow.ItemArray = _RowArray;

            MyCommands = new(_NewRow, MyConnection);
            return MyCommands.DeleteRow();
        }
        #endregion

        #region Get NewRow of Table
        public static DataRow GetNewRow(string DBFile, Tables _Table)
        {
            using var _DataTable = GetDataTable(DBFile, _Table);
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
            if (AppPaths is not null)
            {
                return AppPaths.DBFile;
            }
            return "";
        }

        #endregion

        #region Close Table
        public DataTable CloneTable(Tables _Table)
        {
            return GetDataTable(DBFile, _Table).Clone();
        }

        public static DataTable CloneTable(string _DBFile, Tables _Table)
        {
            return GetDataTable(_DBFile, _Table).Clone();
        }
        #endregion

        #region Get Cash or Bank Book
        public DataTable GetBookVoucher(int ID)
        {
            DataTable _Table = new DataTable();
            if (AppPaths is not null)
            {
                var _filter = $"ID1 = {ID}";
                var _Query = SQLQuery.View_Book(_filter);         // Get Records from Book and Book2 table.
                _Table = GetTable(_Query);
            }
            return _Table;
        }


        public DataTable GetBookList(int BookID)
        {
            DataTable _Table = new DataTable();
            if (AppPaths is not null)
            {
                var _Query = SQLQuery.View_Book($"BookID = {BookID}");
                _Table = GetTable(_Query);
            }
            return _Table;
        }

        public DataTable GetBookList(int BookID, string Filter)
        {
            DataTable _Table = new DataTable();
            if (AppPaths is not null)
            {
                string _SQLQuery = SQLQuery.View_Book($"BookID = {BookID} ");
                string _Sort = "Vou_Date DESC, ID DESC";

                _Table = GetTable(_SQLQuery,Filter, _Sort);
            }
            return _Table;
        }



        public List<CodeTitle> GetBookAccounts(int NatureID)
        {
            // Get Book Account list from COA.
            if (NatureID > 0)
            {
                return GetTable(Tables.COA, $"Nature={NatureID}", "Title").AsEnumerable().ToList().
                    Select(rows => new CodeTitle
                    {
                        ID = rows.Field<int>("ID"),
                        Code = rows.Field<string>("Code") ?? "",
                        Title = rows.Field<string>("Title") ?? ""
                    }).ToList();
            }
            List<CodeTitle> EmptyList = new();
            EmptyList.Add(new CodeTitle { ID = 0, Code = "", Title = "No Records" });
            return EmptyList;

        }


        #endregion

        #region Get Receipt Voucher
        public DataTable GetReceiptVoucher(int receiptID)
        {
            if (receiptID > 0)
            {
                var QueryText = $"SELECT * FROM [view_Receipts] WHERE [TranID] = {receiptID}";
                using var _Table = GetDataTable(DBFile, QueryText, "view_Receipt");
                if (_Table != null && _Table.Columns.Count > 0)
                {
                    return _Table;
                }
            }
            return new DataTable();
        }

        public DataTable GetReceiptList(string _Filter)
        {
            var _Table = GetTable(SQLQueries.Quries.ReceiptList(_Filter));
            _Table ??= new DataTable();
            return _Table;
        }

        #endregion

        #region Geting a DB Directory()



        public static Dictionary<int, string> GetDirectory(string _DirectoryName, string DBFile)
        {
            var _Connection = Connections.GetClientConnection(DBFile);
            var _Dictionary = new Dictionary<int, string> { { 0, "Select..." } };

            var _Query = $"SELECT * FROM [Directories] WHERE [Directory] = '{_DirectoryName}'";
            var _Table = GetDataTable(_Query, _Connection!);
            if (_Table is not null)
            {

                if (_Table.Rows.Count > 0)
                {
                    foreach (DataRow Row in _Table.Rows)
                    {
                        _Dictionary.Add((int)Row["Key"], (string)Row["Value"]);
                    }
                }
            }
            return _Dictionary;
        }

        public Dictionary<int, string> GetDirectory(string _DirectoryName)
        {
            var _Connection = Connections.GetClientConnection(DBFile);
            var _Dictionary = new Dictionary<int, string> { { 0, "Select..." } };

            var _Query = $"SELECT * FROM [Directories] WHERE [Directory] = '{_DirectoryName}'";
            var _Table = GetDataTable(_Query, MyConnection);
            if (_Table is not null)
            {

                if (_Table.Rows.Count > 0)
                {
                    foreach (DataRow Row in _Table.Rows)
                    {
                        _Dictionary.Add((int)Row["Key"], (string)Row["Value"]);
                    }
                }
            }
            return _Dictionary;
        }


        #endregion

        #region Save
        public void Save(DataRow newRow)
        {
            MyCommands = new(newRow, MyConnection);
            MyCommands.SaveChanges();
        }

        public bool Save(Tables _Table, DataRow newRow)
        {
            MyCommands = new(newRow, MyConnection);
            return MyCommands.SaveChanges();
        }
        #endregion

        #region Get Registry Keys

        public void SetKey(string Key, object KeyValue, KeyTypes keytype, string _Title)
        {
            Registry _Registry = new(MyConnection, DBFile);
            _Registry.SetKey(Key, KeyValue, keytype, _Title);
        }

        public async Task SetKeyAsync(string Key, object KeyValue, KeyTypes keytype, string _Title)
        {
            await Task.Run(() =>
            {
                Registry _Registry = new(MyConnection, DBFile);
                _Registry.SetKey(Key, KeyValue, keytype, _Title);
            });
        }

        public string GetText(string Key)
        {
            Registry _Registry = new(MyConnection, DBFile);
            return (string)_Registry.GetKey(Key, KeyTypes.Text);
        }
        public int GetNumber(string Key)
        {
            Registry _Registry = new(MyConnection, DBFile);
            return (int)_Registry.GetKey(Key, KeyTypes.Number);
        }

        public DateTime GetDate(string Key)
        {
            Registry _Registry = new(MyConnection, DBFile);
            return (DateTime)_Registry.GetKey(Key, KeyTypes.Date);
        }

        public bool GetBoolean(string Key)
        {
            Registry _Registry = new(MyConnection, DBFile);
            return (bool)_Registry.GetKey(Key, KeyTypes.Boolean);
        }

        public DateTime GetFrom(string Key)
        {
            Registry _Registry = new(MyConnection, DBFile);
            return (DateTime)_Registry.GetKey(Key, KeyTypes.From);
        }

        public DateTime GetTo(string Key)
        {
            Registry _Registry = new(MyConnection, DBFile);
            return (DateTime)_Registry.GetKey(Key, KeyTypes.To);
        }

        public DateTime[] GetFromTo(string Key)
        {
            DateTime[] _Dates = [GetFrom(Key), GetTo(Key)];
            return _Dates;
        }

        #endregion

        public int RecordCound(Tables _Table, string _Filter)
        {
            string _Query = $"SELECT COUNT(*) FROM {_Table} WHERE {_Filter}";
            using var command = new SQLiteCommand(_Query, MyConnection);
            if(MyConnection.State != ConnectionState.Open) { MyConnection.Open(); }
            var result = Convert.ToInt32(command.ExecuteScalar()); MyConnection.Close();
            return result;
        }
    }

    public class CodeTitle
    {
        public int ID { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
    }
}

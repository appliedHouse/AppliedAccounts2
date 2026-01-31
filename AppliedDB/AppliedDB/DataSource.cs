using AppliedGlobals;
using AppMessages;
using Microsoft.Data.Sqlite;
using System.Data;
using System.Text;
using static AppliedDB.Enums;
using static AppliedGlobals.AppErums;
using Tables = AppliedDB.Enums.Tables;

namespace AppliedDB
{
    public class DataSource : IDisposable
    {
        public AppValues.AppPath AppPaths { get; set; }
        public SqliteConnection MyConnection { get; set; }
        public SqliteConnection MyConnection2 { get; set; }
        public SqliteCommand MyCommand { get; set; }
        public CommandClass MyCommands { get; set; } = new();
        public string DBFile => GetDataFile();
        public string ErrorMessage { get; set; }
        public bool IsSaved { get; set; } = false;
        public MessageClass MsgClass { get; set; } = new();

        private SqliteTransaction? _transaction;


        #region Constructor

        public DataSource(AppValues.AppPath _AppPaths)
        {
            AppPaths = _AppPaths;
            var _Connection = new Connections(AppPaths);
            MyConnection = _Connection.GetSqliteClient()!;               // Get a connection of Client
            MyConnection2 = _Connection.GetSqliteClient()!;              // Get a connection of Client

            if (MyConnection is not null)
            {
                MyCommand = new SqliteCommand("", MyConnection);
            }
        }

        public DataSource()
        {
        }


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
                        using var _Command = new SqliteCommand(_Query.QueryText, MyConnection);

                        using var reader = _Command.ExecuteReader();
                        var dt = new DataTable();
                        dt.Load(reader);

                        return dt;
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
                    var _Query = SQLQuery.GetQuery(_SQLQuery);
                    if (!string.IsNullOrWhiteSpace(_Query.QueryText))
                    {
                        if (MyConnection.State != ConnectionState.Open) { MyConnection.Open(); }
                        using var _Command = new SqliteCommand(_Query.QueryText, MyConnection);
                        _Command.Parameters.AddWithValue("@ID", _ID);

                        using var reader = _Command.ExecuteReader();
                        var dt = new DataTable();
                        dt.Load(reader);

                        return dt;
                    }


                    //if (MyConnection.State != ConnectionState.Open) { MyConnection.Open(); }
                    //var _Query = SQLQuery.GetQuery(_SQLQuery);
                    //using var _Command = new SqliteCommand(_Query.QueryText, MyConnection);
                    //_Command.Parameters.AddWithValue("@ID", _ID);
                    //using var _Adapter = new SqliteDataAdapter(_Command);
                    //using var _DataSet = new DataSet();
                    //_Adapter.Fill(_DataSet, _Query.TableName);
                    //if (MyConnection.State == ConnectionState.Open) { MyConnection.Close(); }
                    //if (_DataSet.Tables.Count == 1)
                    //{
                    //    return _DataSet.Tables[0];
                    //}

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

        public async Task<DataTable> GetTableAsync(string _SQLQuery)
        {
            var _Table = await Task.Run(() => GetTable(_SQLQuery, "", ""));
            return _Table;

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
                        if (!string.IsNullOrWhiteSpace(_SQLQuery))
                        {
                            if (MyConnection.State != ConnectionState.Open) { MyConnection.Open(); }
                            using var _Command = new SqliteCommand(_SQLQuery, MyConnection);
                            if (!string.IsNullOrEmpty(_Filter)) { _Command.CommandText += $" WHERE {_Filter}"; }
                            if (!string.IsNullOrEmpty(_Sort)) { _Command.CommandText += $" ORDER BY {_Sort}"; }
                            using var reader = _Command.ExecuteReader();

                            var dt = GetDataTableExtention(reader);
                            dt.TableName = ExtractTableNameFromQuery(_SQLQuery);
                            return dt;
                        }
                    }
                return new DataTable();
            }
            catch (Exception ex)
            {
                MsgClass.Critical(ex.Message);
                return new DataTable();
            }
        }
        #endregion

        #region Get Table for List as per paging model
        public static DataTable GetPageQuery(PageQuery _PageQuery)
        {
            return _PageQuery.GetPageData().Result;
        }
        #endregion


        #region Get Table Static

       
        public static DataTable GetDataTable(Tables _Table, SqliteCommand _Command)
        {
            try
            {
                if (_Command.Connection is not null)
                {
                    if (_Command.Connection.State != ConnectionState.Open) { _Command.Connection.Open(); }
                    string TableName = _Table.ToString();

                    using var reader = _Command.ExecuteReader();
                    var dt = GetDataTableExtention(reader);
                    dt.TableName = _Table.ToString();

                    return dt;
                }

            }
            catch (Exception)
            {

                return new DataTable();
            }
            return new DataTable(); ;
        }
        public static DataTable GetDataTable(Tables _Table, SqliteConnection _Connection)
        {
            return GetDataTable(_Table.ToString(), _Connection);

        }
        public static DataTable GetDataTable(string _Table, SqliteConnection _Connection)
        {
            try
            {
                if (_Connection is not null)
                {
                    if (_Connection.State != ConnectionState.Open) { _Connection.Open(); }
                    string TableName = _Table.ToString();
                    string CommText = $"SELECT * FROM [{_Table}]";

                    using var _Command = new SqliteCommand(CommText, _Connection);
                    using var reader = _Command.ExecuteReader();
                    var dt = GetDataTableExtention(reader); // new DataTable();
                    //dt.Load(reader);
                    dt.TableName = _Table;
                    return dt;

                }

            }
            catch (Exception)
            {

                return new DataTable();
            }
            return new DataTable();

        }
        public static DataTable GetQueryTable(string _SQLQuery, SqliteConnection _Connection)
        {
            try
            {
                if (_Connection is not null)
                {
                    if (!string.IsNullOrEmpty(_SQLQuery))
                    {
                        if (_Connection.State != ConnectionState.Open) { _Connection.Open(); }
                        var _Command = new SqliteCommand(_SQLQuery, _Connection);
                        using var reader = _Command.ExecuteReader();
                        var dt = GetDataTableExtention(reader);
                        dt.TableName = ExtractTableNameFromQuery(_SQLQuery);
                        return dt;
                    }
                }
                return new DataTable();
            }
            catch (Exception)
            {

                return new DataTable();
            }
        }
        public static DataTable GetDataTable(Tables table, SqliteConnection connection, string filter)
        {
            try
            {
                if (connection == null)
                    return new DataTable();

                string query = $"SELECT * FROM {table}";
                if (!string.IsNullOrWhiteSpace(filter))
                    query += $" WHERE {filter}";

                using var command = new SqliteCommand(query, connection);
                if (connection.State != ConnectionState.Open)
                    connection.Open();

                using var reader = command.ExecuteReader();
                var dt = GetDataTableExtention(reader);

                if(dt.Rows.Count == 0) { dt.Load(reader); }             // try again to load data directly. if not found...


                dt.TableName = table.ToString();
                return dt;
            }
            catch
            {
                return new DataTable();
            }
            finally
            {
                if (connection?.State == ConnectionState.Open)
                    connection.Close();
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
                using var _Command = new SqliteCommand($"SELECT * FROM [Messages] WHERE Language={_Language}", _Connection);
                using var _reader = _Command.ExecuteReader();
                var dt = new DataTable();
                dt.Load(_reader);

                return dt;

                //using var _Adapter = new SqliteDataAdapter(_Command);
                //using var _DataSet = new DataSet();
                //_Adapter.Fill(_DataSet, "Messages");
                //if (_Connection.State == ConnectionState.Open) { _Connection.Close(); }
                //if (_DataSet.Tables.Count > 0)
                //{
                //    return _DataSet.Tables[0];
                //}
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

        #region Get Data Row
        public DataRow? GetDataRow(Tables _Table, long ID)
        {
            var _Connection = Connections.GetSqliteConnection(MyConnection.DataSource)!;
            var _DataTable =  GetDataTable(_Table, _Connection,$"ID={ID}");
            if(_DataTable.Rows.Count > 0)
            {
                DataRow row = _DataTable.Rows[0];
                row.AcceptChanges();                    // Add here to Rowstate must be unchanged
                return row;
            }
            return null;
        }

        #endregion

        #region Seek
        public DataRow Seek(Tables _Table, long ID)
        {
            var table = GetTable(_Table);       // <-- real DataTable
            var list = table.AsEnumerable().ToList();

            if (ID >= 0 && list.Count > 0)
            {
                var row = list
                    .FirstOrDefault(r => r.Field<long>("ID") == ID);

                if (row != null)
                    return row;
            }

            // return an empty row with same schema
            return table.NewRow();
        }

        public List<DataRow> List(Tables _Table, long ID)
        {
            var _DataTable = GetTable(_Table).AsEnumerable().ToList();
            var _DataRow = _DataTable.Where(rows => rows.Field<long>("ID") == ID).ToList();
            return _DataRow;
        }

        public object? SeekValue(Tables _Table, long _ID, string _column)
        {
            // _Table  => Table Enums.table
            // _ID     => ID primary key for search record
            // _column => Column Name for search value

            var _DataRow = GetTable(_Table).AsEnumerable().ToList().Where(rows => rows.Field<long>("ID") == _ID).SingleOrDefault();

            if (_DataRow != null)
            {
                return _DataRow.Field<object>(_column);
            }

            return null;

        }

        public string SeekTitle(Tables _Table, long ID)
        {
            if (_Table == 0 || ID == 0) { return string.Empty; }

            DataTable _DataTable = GetTable(_Table);

            if (_DataTable is null) { return string.Empty; }

            try
            {
                var _Title = _DataTable.AsEnumerable()
                       .Where(row => row.Field<long>("ID") == ID)
                       .Select(row => row.Field<string>("Title"))
                       .First() ?? string.Empty;

                return _Title;
            }
            finally
            {
                _DataTable.Dispose();
            }

        }

        public decimal SeekTaxRate(long ID)
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
                        _CodeTitle.ID = (int)Row.Field<long>("ID");
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


        public static List<CodeTitle> GetCodeTitle(string _Table, SqliteConnection DBConnection)
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
                        _CodeTitle.ID = (int)Row.Field<long>("ID");
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
        public static DataTable GetDataTable(string DBFile, Tables _Table, string _Filter)
        {
            string Query = $"SELECT * FROM [{_Table}] WHERE Code='{_Filter}'";
            return GetDataTable(DBFile, Query, _Table.ToString());
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
                    using var _Command = new SqliteCommand(_CommandText, _Connection);
                    using var _reader = _Command.ExecuteReader();
                    var schemaTable = _reader.GetSchemaTable();
                    var dt = GetDataTableExtention(_reader);
                    dt.TableName = _Table;
                    return dt;

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
                using var _Command = new SqliteCommand(_CommandText, _Connection);
                using var _reader = _Command.ExecuteReader();
                var dt = GetDataTableExtention(_reader);
                dt.TableName = _TableName;
                return dt;
            }
            return null;
        }
        public static List<Dictionary<long, string>> GetDataList(string DBFile, Tables _Table)
        {
            if (DBFile == null) { return new(); }
            if (DBFile.Length == 0) { return new(); }
            //========================================================

            List<Dictionary<long, string>> _List = new();
            try
            {
                string _TableName = _Table.ToString();
                var _CommandText = $"SELECT * FROM [{_TableName}]";
                var _Connection = Connections.GetClientConnection(DBFile);
                if (_Connection is not null)
                {

                    if (_Connection.State != ConnectionState.Open) { _Connection.Open(); }
                    using var _Command = new SqliteCommand(_CommandText, _Connection);
                    using var _reader = _Command.ExecuteReader();
                    var dt = GetDataTableExtention(_reader);

                    foreach (DataRow Row in dt.Rows)
                    {
                        Dictionary<long, string> _item = new() { { (int)Row.Field<long>("ID"), (string)Row["Title"] } };
                        _List.Add(_item);
                    }
                    return _List;
                }
                else
                {
                    return _List;
                }

            }
            catch (Exception)
            {
                return _List;
            }
        }

        #endregion

        #region Maximum ID of the Table

        public static long GetMaxID(string _Table, string ConnectionString)
        {
            // Create this function due to command.transaction issue.
            // Create a new connection without transaction to avoid error in transaction mode.
            var _Connection = Connections.GetSqliteConnectionbyString(ConnectionString);
            DataTable _DataTable = GetDataTable(_Table,_Connection!);
            if (_DataTable.Rows.Count == 0) { return 1; }
            long _MaxID = (long)_DataTable.Compute("MAX(ID)", "") + 1;
            _DataTable.Dispose();
            return _MaxID;

        }

        public static long GetMaxID(string _Table, SqliteConnection DBConnection)
        {
            DataTable _DataTable = GetDataTable(_Table, DBConnection);
            if (_DataTable.Rows.Count == 0) { return 1; }
            long _MaxID = (long)_DataTable.Compute("MAX(ID)", "") + 1;
            _DataTable.Dispose();
            return _MaxID;

        }

        public static long GetMaxID(Tables _Table, SqliteConnection DBConnection)
        {
            DataTable _DataTable = GetDataTable(_Table, DBConnection);
            if (_DataTable.Rows.Count == 0) { return 1; }
            long _MaxID = (long)_DataTable.Compute("MAX(ID)", "") + 1;
            _DataTable.Dispose();
            return _MaxID;

        }

        public long GetMaxID(Tables _Table)
        {
            var _Text = $"SELECT Max(ID) AS MaxID FROM[{_Table}]";
            var _DataTable = GetTable(_Text);
            if (_DataTable.Rows.Count == 0) { return 1; }
            long _MaxID = _DataTable.Rows[0].Field<long>("MaxID") ;
            _DataTable.Dispose();
            return _MaxID + 1;
        }
        #endregion

        #region Delete Row
        //public bool Delete(Tables _Table, DataRow _Row)
        //{
        //    var _DataTable = GetTable(_Table);
        //    var _NewRow = _DataTable.NewRow();
        //    var _RowArray = _Row.ItemArray;

        //    _NewRow.ItemArray = _RowArray;

        //    MyCommands = new(_NewRow, MyConnection);
        //    return MyCommands.DeleteRow();
        //}

        //public bool Delete(DataRow _Row)
        //{
        //    if (MyCommands.CommandDelete != null) { MyCommands.CommandDelete.Transaction = _transaction; }

        //    var IsDeleted = false;
        //    MyCommands = new(_Row, MyConnection);
        //    var result = MyCommands.DeleteRow();
        //    if (result)
        //    {
        //        IsDeleted = true;
        //    }
        //    return IsDeleted;
        //}

        public bool Delete(DataRow _Row)
        {
            // Create commands FIRST
            MyCommands = new(_Row, MyConnection);

            // THEN attach the transaction
            if (_transaction != null && MyCommands.CommandDelete != null)
            {
                MyCommands.CommandDelete.Transaction = _transaction;
            }

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
        public DataTable GetBookVoucher(long ID)
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


        public DataTable GetBookList(long BookID)
        {
            DataTable _Table = new DataTable();
            if (AppPaths is not null)
            {
                var _Query = SQLQuery.View_Book($"BookID = {BookID}");
                _Table = GetTable(_Query);
            }
            return _Table;
        }

        public DataTable GetBookList(long BookID, string Filter)
        {
            DataTable _Table = new DataTable();
            if (AppPaths is not null)
            {
                string _SQLQuery = SQLQuery.View_Book($"BookID = {BookID} ");
                string _Sort = "Vou_Date DESC, ID DESC";

                _Table = GetTable(_SQLQuery, Filter, _Sort);
            }
            return _Table;
        }



        public List<CodeTitle> GetBookAccounts(long NatureID)
        {
            // Get Book Account list from COA.
            if (NatureID > 0)
            {
                return GetTable(Tables.COA, $"Nature={NatureID}", "Title").AsEnumerable().ToList().
                    Select(rows => new CodeTitle
                    {
                        ID = rows.Field<long>("ID"),
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
        public DataTable GetReceiptVoucher(long receiptID)
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
            IsSaved = false;
            if (MyConnection.State != ConnectionState.Open) { MyConnection.Open(); }

            MyCommands = new(newRow, MyConnection);
            if (_transaction != null)
            {
                if(MyCommands.CommandInsert != null) { MyCommands.CommandInsert.Transaction = _transaction; }
                if (MyCommands.CommandUpdate != null) { MyCommands.CommandUpdate.Transaction = _transaction; }
                if (MyCommands.CommandDelete != null) { MyCommands.CommandDelete.Transaction = _transaction; }
            }

            var result = MyCommands.SaveChanges();
            if (result)
            {
                MsgClass.Success(AppMessages.Enums.Messages.Save);
                IsSaved = true;
            }
            else
            {
                MsgClass.Error(AppMessages.Enums.Messages.RecordNotSaved);
            }
        }

        // Depreciated....... NOT IN USE... 14-Dec-2025.
        public bool Save(Tables _Table, DataRow newRow)
        {
            MyCommands = new(newRow, MyConnection);
            return MyCommands.SaveChanges();
        }
        #endregion

        #region Get Registry Keys

        public void SetKey(string Key, object KeyValue, KeyTypes keytype, string _Title)
        {
            if (MyConnection.State != ConnectionState.Open) { MyConnection.Open(); }

            DataTable TB_Registry = GetTable(Tables.Registry, $"Code = '{Key}'");
            DataRow CurrentRow;
            //var SQLAction = string.Empty;

            if (TB_Registry.Rows.Count == 1)
            {
                //SQLAction = "Update";
                CurrentRow = TB_Registry.DefaultView[0].Row;
                CurrentRow.AcceptChanges();
            }
            else
            {
                //SQLAction = "Insert";
                CurrentRow = TB_Registry.NewRow();
                CurrentRow["ID"] = GetMaxID(Tables.Registry, MyConnection);
            }

            CurrentRow["Code"] = Key;
            CurrentRow["Title"] = _Title;
            CurrentRow["UserName"] = DBFile;
            switch (keytype)
            {
                case KeyTypes.Number:
                    CurrentRow["nValue"] = KeyValue;
                    break;
                case KeyTypes.Currency:
                    CurrentRow["mValue"] = KeyValue;
                    break;
                case KeyTypes.Date:
                    CurrentRow["dValue"] = KeyValue;
                    break;
                case KeyTypes.Boolean:
                    CurrentRow["bValue"] = KeyValue;
                    break;
                case KeyTypes.Text:
                    CurrentRow["cValue"] = KeyValue;
                    break;
                case KeyTypes.From:
                    CurrentRow["From"] = KeyValue;
                    break;
                case KeyTypes.To:
                    CurrentRow["To"] = KeyValue;
                    break;
                default:
                    break;
            }

            var cmd = new CommandClass(CurrentRow, MyConnection);
            cmd.SaveChanges();

            MyConnection.Close();
        }

        public object GetKey(string Key, KeyTypes keytype)
        {

            object ReturnValue;
            var Registry = GetTable(Tables.Registry, $" WHERE Code = '{Key}'");

            if (Registry.Rows.Count == 1)
            {
                DataRow Row = Registry.Rows[0];
                ReturnValue = keytype switch
                {
                    KeyTypes.Number => Row["nValue"],
                    KeyTypes.Currency => Row["mValue"],
                    KeyTypes.Boolean => Row["bValue"],
                    KeyTypes.Date => Row["dValue"],
                    KeyTypes.Text => Row["cValue"],
                    _ => string.Empty
                };
            }
            else
            {
                ReturnValue = keytype switch
                {
                    KeyTypes.Number => 0,
                    KeyTypes.Currency => 0.00,
                    KeyTypes.Boolean => false,
                    KeyTypes.Date => DateTime.Now,
                    KeyTypes.Text => string.Empty,
                    _ => string.Empty
                };
            }

            if (ReturnValue == DBNull.Value) { return string.Empty; }
            return ReturnValue;
        }

        // Depreciated 29-Jan-2026
        #region Set Key Depreciated
        public void SetKey1(string Key, object KeyValue, KeyTypes keytype, string _Title)
        {

            Registry _Registry = new(MyConnection, DBFile);
            _Registry.SetKey(Key, KeyValue, keytype, _Title);
        }
        #endregion

        public async Task SetKeyAsync(string Key, object KeyValue, KeyTypes keytype, string _Title)
        {
            await Task.Run(() =>
            {
                SetKey(Key, KeyValue, keytype, _Title);
            });
        }

        public string GetText(string Key)
        {
            //Registry _Registry = new(MyConnection, DBFile);
            return (string)GetKey(Key, KeyTypes.Text);
        }
        public int GetNumber(string Key)
        {
            //Registry _Registry = new(MyConnection, DBFile);
            return (int)GetKey(Key, KeyTypes.Number);
        }

        public DateTime GetDate(string Key)
        {
            //Registry _Registry = new(MyConnection, DBFile);
            return (DateTime)GetKey(Key, KeyTypes.Date);
        }

        public bool GetBoolean(string Key)
        {
            //Registry _Registry = new(MyConnection, DBFile);
            return (bool)GetKey(Key, KeyTypes.Boolean);
        }

        public DateTime GetFrom(string Key)
        {
            //Registry _Registry = new(MyConnection, DBFile);
            return (DateTime)GetKey(Key, KeyTypes.From);
        }

        public DateTime GetTo(string Key)
        {
            //Registry _Registry = new(MyConnection, DBFile);
            return (DateTime)GetKey(Key, KeyTypes.To);
        }

        public DateTime[] GetFromTo(string Key)
        {
            DateTime[] _Dates = [GetFrom(Key), GetTo(Key)];
            return _Dates;
        }

        #endregion

        #region Count Record
        public int RecordCound(Tables _Table, string _Filter)
        {
            string _Query = $"SELECT COUNT(*) FROM {_Table} WHERE {_Filter}";
            using var command = new SqliteCommand(_Query, MyConnection);
            if (MyConnection.State != ConnectionState.Open) { MyConnection.Open(); }
            var result = Convert.ToInt32(command.ExecuteScalar()); MyConnection.Close();
            return result;
        }
        #endregion

        #region Data Table Extention
        public static DataTable GetDataTableExtention(SqliteDataReader _reader)
        {
            var schemaTable = _reader.GetSchemaTable();
            var dt = new DataTable();

            if (schemaTable != null)
            {
                foreach (DataRow row in schemaTable.Rows)
                {
                    string _ColumnName = row["ColumnName"].ToString() ?? string.Empty;
                    Type _DataType = (Type)row["DataType"];
                    string _TypeName = (string)row["DataTypeName"];

                    if (_TypeName.ToUpper() == "INT") { _DataType = typeof(int); }
                    else if (_TypeName.ToUpper() == "INT64") { _DataType = typeof(long); }
                    else if (_TypeName.ToUpper() == "DECIMAL") { _DataType = typeof(decimal); }
                    else if (_TypeName.ToUpper() == "DATETIME") { _DataType = typeof(DateTime); }
                    else if (_TypeName.ToUpper() == "NVARCHAR") { _DataType = typeof(string); }
                    else if (_TypeName.ToUpper() == "BOOLEAN") { _DataType = typeof(bool); }

                    string upperColumnName = _ColumnName.ToUpper();
                    if (upperColumnName == "ID" ||
                        upperColumnName == "ID1" ||
                        upperColumnName == "ID2" ||
                        upperColumnName == "TRANID")
                    {
                        _DataType = typeof(long);
                    }
                    else if (upperColumnName == "SR_NO")
                    {
                        _DataType = typeof(int);
                    }

                    dt.Columns.Add(_ColumnName, _DataType);
                }

                var _Stop = true;


                while (_reader.Read())
                {
                    DataRow newRow = dt.NewRow();

                    for (int i = 0; i < _reader.FieldCount; i++)
                    {
                        string columnName = _reader.GetName(i);

                        if (_reader.IsDBNull(i))
                        {
                            newRow[columnName] = DBNull.Value;
                        }
                        else
                        {
                            // Get the value and handle type conversions
                            object value = _reader.GetValue(i);
                            Type columnType = dt.Columns[columnName].DataType;

                            // Convert if needed
                            if (value.GetType() != columnType)
                            {
                                try
                                {
                                    if (columnType == typeof(DateTime) && value is string)
                                    {
                                        if (DateTime.TryParse(value.ToString(), out DateTime dtValue))
                                            value = dtValue;
                                    }
                                    else if (columnType == typeof(decimal) && value is double)
                                    {
                                        value = Convert.ToDecimal(value);
                                    }
                                    else if (columnType == typeof(long) && value is long)
                                    {
                                        // Already correct type
                                    }
                                    else
                                    {
                                        value = Convert.ChangeType(value, columnType);
                                    }
                                }
                                catch
                                {
                                    // Keep original value if conversion fails
                                }
                            }

                            newRow[columnName] = value;
                        }
                    }

                    dt.Rows.Add(newRow);
                }
            }

            return dt;

            //while (_reader.Read())
            //    {
            //        DataRow newRow = dt.NewRow();

            //        for (int i = 0; i < _reader.FieldCount; i++)
            //        {
            //            string columnName = _reader.GetName(i);
            //            Type columnType = dt.Columns[columnName].DataType;

            //            if (_reader.IsDBNull(i)) { newRow[columnName] = DBNull.Value; }
            //            else { newRow[columnName] = _reader.GetValue(i); }


            //        }
            //        dt.Rows.Add(newRow);

            //    }
            //}

            //return dt;
        }

        private static string ExtractTableNameFromQuery(string sqlQuery)
        {
            if (string.IsNullOrWhiteSpace(sqlQuery))
                return "UnknownTable";

            // Normalize the query
            var query = sqlQuery.Trim();

            // Check if it's a SELECT query
            if (query.StartsWith("SELECT", StringComparison.OrdinalIgnoreCase))
            {
                // Find the FROM clause
                var fromIndex = query.IndexOf(" FROM ", StringComparison.OrdinalIgnoreCase);
                if (fromIndex > 0)
                {
                    var fromPart = query.Substring(fromIndex + 6); // Skip " FROM "

                    // Find the next keyword (WHERE, ORDER BY, GROUP BY, etc.)
                    var endIndex = fromPart.IndexOfAny(new[] { ' ', '\t', '\n', '\r', ';' });
                    if (endIndex > 0)
                    {
                        return fromPart.Substring(0, endIndex).Trim();
                    }
                    return fromPart.Trim().Split(' ')[0].Trim();
                }
            }
            return "QueryResult";
        }

        #endregion

        #region SQL Transaction Methods
        public void BeginTransaction()
        {
            if (MyConnection.State != ConnectionState.Open)
                MyConnection.Open();

            _transaction = MyConnection.BeginTransaction();
        }

        public void CommitTransaction()
        {
            _transaction?.Commit();
            _transaction = null;
        }

        public void RollbackTransaction()
        {
            _transaction?.Rollback();
            _transaction = null;
        }
        #endregion

        public DataRow RemoveNullValues(DataRow row)
        {
            
            foreach (DataColumn column in row.Table.Columns)
            {
                if (row.IsNull(column))
                {
                    Type columnType = column.DataType;

                    row[column] = columnType switch
                    {
                        Type t when t == typeof(int) => 0,
                        Type t when t == typeof(short) => (short)0,      // Int16
                        Type t when t == typeof(long) => 0L,              // Int64
                        Type t when t == typeof(string) => string.Empty,
                        Type t when t == typeof(decimal) => 0.0M,
                        Type t when t == typeof(float) => 0.0f,
                        Type t when t == typeof(bool) => false,
                        Type t when t == typeof(double) => 0.0,
                        Type t when t == typeof(byte) => (byte)0,
                        Type t when t == typeof(char) => '\0',
                        Type t when t == typeof(DateTime) => DateTime.MinValue,
                        Type t when t == typeof(Guid) => Guid.Empty,
                        Type t when t.IsValueType => Activator.CreateInstance(t), // Default value for other value types
                        _ => null // For reference types that aren't strings
                    };
                }
            }

            return row;
        }

        #region Disposed
        public void Dispose()
        {
            try
            {
                _transaction?.Dispose();
                _transaction = null;

                MyCommand?.Dispose();
                MyCommand = null!;

                if (MyConnection?.State == ConnectionState.Open)
                    MyConnection.Close();

                MyConnection?.Dispose();
                MyConnection = null!;

                if (MyConnection2?.State == ConnectionState.Open)
                    MyConnection2.Close();

                MyConnection2?.Dispose();
                MyConnection2 = null!;
            }
            catch
            {
                // NEVER throw from Dispose
            }
        }

        public int GetCount(Tables book, string filter)
        {
            using var cmd = MyConnection.CreateCommand();
            cmd.CommandText = $"SELECT COUNT(*) FROM {book} WHERE {filter}";

            if (cmd.Connection.State != ConnectionState.Open)
                cmd.Connection.Open();

            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        #endregion
    }






    public class CodeTitle
    {
        public long ID { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
    }
}

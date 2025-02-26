using System.Data;
using System.Data.SQLite;
using System.Text;
using AppliedDB;
using static AppliedDB.Enums;

namespace AppliedAccounts.Data
{
    public class CreateDatabase
    {
        public Connections ConnectionClass { get; set; }
        public SQLiteConnection MyConnection { get; set; }
        public List<DataRow> TableList { get; set; }
        public UserProfile AppUser { get; set; }
        public AppUserModel UserModel { get; set; }
        private string TableName { get; set; }
        private string UserName { get; set; }
        private string DBFile { get; set; }
        private List<string> MyMessages { get; set; }

        public CreateDatabase(AppUserModel _UserModel)
        {
            UserModel = _UserModel;
            MyMessages = new List<string>();
            ConnectionClass = new(UserModel);

            MyConnection = ConnectionClass.GetSQLiteUsers() ?? new();
            GetTableNames();
            CreateTables();
        }




        public void GetTableNames()
        {
            var tableNames = new List<string>();
            if (MyConnection.State != System.Data.ConnectionState.Open) { MyConnection.Open(); }
            var CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name NOT LIKE 'sqlite_%';";
            var _Table = AppliedDB.DataSource.GetQueryTable(CommandText, MyConnection);
            TableList = _Table.AsEnumerable().ToList();
        }

        #region Create Tables


        public void CreateTables()
        {
            var _SQLQuery = $"SELECT name FROM sqlite_master WHERE type in('table', 'view') ORDER BY 1";
            var _TableView = DataSource.GetDataTable(DBFile, _SQLQuery).AsDataView();
            var _TablesList = Enum.GetValues(typeof(Tables)).Cast<Tables>().ToList();

            foreach (Tables _Table in _TablesList)
            {
                if (_Table.ToString().StartsWith("tmp")) { continue; }
                MyMessages.Append($"Crerated Table {_Table}");
                CreateTable(_Table);
            }
        }
        #endregion


        //========================================================================= CREATE
        #region Create DataTable into Source Data

        public void CreateTable(Tables _Table)
        {
            #region return if table exist
            var _TableName = _Table.ToString();
            var _CommandText = $"SELECT count(name) FROM sqlite_master WHERE type in('table', 'view') AND name ='{_TableName}'";
            var _Command = new SQLiteCommand(_CommandText, MyConnection);
            if (!MyConnection.State.Equals(ConnectionState.Open)) { MyConnection.Open(); }
            long TableExist = (long)_Command.ExecuteScalar();
            if (TableExist > 0) { return; }
            #endregion

            switch (_Table)
            {
                case Tables.Registry:
                    break;
                case Tables.COA:
                    break;
                case Tables.COA_Nature:
                    break;
                case Tables.COA_Class:
                    break;
                case Tables.COA_Notes:
                    break;
                case Tables.CashBook:
                    break;

                case Tables.BankBook:
                    _CommandText = BankBook();
                    //BankBook(UserName);
                    break;
                case Tables.WriteCheques:
                    break;
                case Tables.Taxes:
                    break;
                case Tables.ChequeTranType:
                    break;
                case Tables.ChequeStatus:
                    break;
                case Tables.TaxTypeTitle:
                    break;
                case Tables.BillPayable:
                    break;
                case Tables.BillPayable2:
                    break;
                case Tables.view_Purchased:
                    _CommandText = Purchased();
                    break;
                case Tables.view_Sold:
                    _CommandText = Sold();
                    break;
                case Tables.Production:
                    _CommandText = Production1();
                    _CommandText = Production2();
                    break;
                case Tables.view_Production:
                    _CommandText = ProductionView();
                    break;
                case Tables.TB:
                    break;
                case Tables.BillReceivable:
                    break;
                case Tables.BillReceivable2:
                    break;
                case Tables.SaleReturn:
                    _CommandText = SaleReturn();
                    break;
                case Tables.view_BillReceivable:
                    break;
                case Tables.OBALCompany:
                    break;
                case Tables.JVList:
                    break;
                case Tables.Customers:
                    break;
                case Tables.City:
                    break;
                case Tables.Country:
                    break;
                case Tables.Project:
                    break;
                case Tables.Employees:
                    break;
                case Tables.Directories:
                    _CommandText = Directories();
                    _CommandText = DirectoriesINSERT();
                    break;
                case Tables.Inventory:
                    break;
                case Tables.Inv_Category:
                    break;
                case Tables.Inv_SubCategory:
                    break;
                case Tables.Inv_Packing:
                    break;
                case Tables.Inv_UOM:
                    break;
                case Tables.StockCategory:
                    _CommandText = StockCategory();
                    break;
                case Tables.StockInHand:
                    _CommandText = StockInHand();
                    break;
                case Tables.OBALStock:
                    break;
                case Tables.BOMProfile:
                    break;
                case Tables.BOMProfile2:
                    break;
                case Tables.StockPositionData:
                    _CommandText = StockPositionData();
                    break;
                case Tables.StockPosition:
                    _CommandText = StockPosition();
                    break;
                case Tables.StockPositionSUM:
                    _CommandText = StockPositionSUM();
                    break;
                case Tables.Ledger:
                    break;
                case Tables.view_Ledger:
                    break;
                case Tables.CashBookTitles:
                    break;
                case Tables.VouMax_JV:
                    break;
                case Tables.VouMax:
                    break;
                case Tables.PostCashBook:
                    break;
                case Tables.PostBankBook:
                    break;
                case Tables.PostWriteCheque:
                    break;
                case Tables.PostBillReceivable:
                    break;
                case Tables.PostBillPayable:
                    break;
                case Tables.PostPayments:
                    break;
                case Tables.PostReceipts:
                    break;
                case Tables.UnpostCashBook:
                    break;
                case Tables.UnpostBillPayable:
                    break;
                case Tables.fun_BillPayableAmounts:
                    break;
                case Tables.fun_BillPayableEntry:
                    break;
                case Tables.TempLedger:
                    break;
                case Tables.Chk_BillReceivable1:
                    _CommandText = Chk_BillReceivable1();
                    break;
                case Tables.Chk_BillReceivable2:
                    _CommandText = Chk_BillReceivable2();
                    break;
                case Tables.Receipts:
                    _CommandText = Receipts();
                    break;



                default:
                    break;
            }


            try
            {
                if (!MyConnection.State.Equals(ConnectionState.Open)) { MyConnection.Open(); }

                _Command = new(_CommandText, MyConnection);
                var rowsAffected = _Command.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    MyMessages.Add($"Table {_TableName} created successfully.");
                }
                else
                {
                    MyMessages.Add($"Table {_TableName} NOT created (Failed).");
                }

            }
            catch (Exception e)
            {
                MyMessages.Add($"Error: {e.Message}");
            }

        }




        #endregion



        /// <Queries>
        /// Start SQL Query Text  //////////////////////////////////////////////////////////////////////
        /// Start SQL Query Text  //////////////////////////////////////////////////////////////////////
        /// </summary>




        #region Sale Return
        public string SaleReturn()
        {
            var _Text = new StringBuilder();
            _Text.AppendLine("CREATE TABLE[SaleReturn] (");
            _Text.AppendLine("[ID] INT PRIMARY KEY NOT NULL UNIQUE, ");
            _Text.AppendLine("[Vou_No] TEXT(12) NOT NULL UNIQUE, ");
            _Text.AppendLine("[Vou_Date] DATETIME NOT NULL, ");
            _Text.AppendLine("[TranID] INT NOT NULL UNIQUE REFERENCES[BillReceivable2]([ID]), ");
            _Text.AppendLine("[QTY] DECIMAL NOT NULL DEFAULT 0, ");
            _Text.AppendLine("[Status] TEXT(12) NOT NULL DEFAULT Submitted)");
            return _Text.ToString();
        }
        #endregion
        #region Bank Book
        public string BankBook()
        {
            var Text = new StringBuilder();
            Text.AppendLine("CREATE TABLE[BankBook](");
            Text.AppendLine("[ID] INT NOT NULL UNIQUE,");
            Text.AppendLine("[Vou_Date] DATETIME NOT NULL, ");
            Text.AppendLine("[Vou_No] TEXT(10) NOT NULL,");
            Text.AppendLine("[BookID] INT NOT NULL, ");
            Text.AppendLine("[COA] INT NOT NULL, ");
            Text.AppendLine("[Ref_No] NVARCHAR(10), ");
            Text.AppendLine(" [Sheet_No] NVARCHAR(12), ");
            Text.AppendLine("[DR] DECIMAL NOT NULL, ");
            Text.AppendLine("[CR] DECIMAL NOT NULL,");
            Text.AppendLine("[Customer] INT,");
            Text.AppendLine("[Employee] INT, ");
            Text.AppendLine("[Project] INT, ");
            Text.AppendLine("[Description] NVARCHAR(60) NOT NULL,");
            Text.AppendLine("[Comments] NVARCHAR(500), ");
            Text.AppendLine("[Status] NVARCHAR(10) NOT NULL DEFAULT Submitted);");
            return Text.ToString();

        }
        #endregion

        #region Directories
        public string Directories()
        {
            var Text = new StringBuilder();
            Text.AppendLine("CREATE TABLE[Directories](");
            Text.AppendLine("[ID] INT PRIMARY KEY NOT NULL UNIQUE, ");
            Text.AppendLine("[Directory] NVARCHAR NOT NULL,");
            Text.AppendLine("[Key] INT NOT NULL, ");
            Text.AppendLine("[Value] NVARCHAR NOT NULL); ");

            return Text.ToString();

        }

        public string DirectoriesINSERT()
        {
            //DataTableClass _TableClass = new DataTableClass(UserName, Tables.Directories);
            //SQLiteCommand _Command = new(ConnectionClass.AppConnection(UserName));
            //string[] Queries = new string[4];

            //Queries[0] = "INSERT INTO [Directories] VALUES (1, 'CompanyStatus', 1, 'Customer')";
            //Queries[1] = "INSERT INTO [Directories] VALUES (2, 'CompanyStatus', 2, 'Supplier');";
            //Queries[2] = "INSERT INTO [Directories] VALUES (3, 'CompanyStatus', 3, 'Vendor');";
            //Queries[3] = "INSERT INTO [Directories] VALUES (4, 'CompanyStatus', 4, 'Customer / Vendor');";

            //foreach (string Query in Queries)
            //{
            //    _Command.CommandText = Query;
            //    _Command.ExecuteNonQuery();
            //}
            return "";
        }

        #endregion


        #region Stock Position Data
        public string StockPositionData()
        {
            AppRegistry.SetKey(UserName, "QueryMessage", string.Empty, KeyType.Text);
            var Text = new StringBuilder();
            Text.AppendLine("CREATE VIEW [StockPositionData] AS ");
            Text.AppendLine(SQLQuery.StockPositionData(""));
            return Text.ToString();


        }

        public string StockPosition()
        {
            AppRegistry.SetKey(UserName, "QueryMessage", string.Empty, KeyType.Text);
            var Text = new StringBuilder();
            Text.AppendLine("CREATE VIEW [StockPosition] AS ");
            Text.AppendLine(SQLQuery.StockPosition("UserName"));
            return Text.ToString();
        }

        public string StockPositionSUM()
        {
            AppRegistry.SetKey(UserName, "QueryMessage", string.Empty, KeyType.Text);
            var Text = new StringBuilder();
            Text.AppendLine("CREATE VIEW [StockPositionSUM] AS ");
            Text.AppendLine(SQLQuery.StockPositionSUM(UserName));
            return Text.ToString();
        }

        #endregion

        #region Cheak Bill Receivable

        public string Chk_BillReceivable1()
        {
            AppRegistry.SetKey(UserName, "QueryMessage", string.Empty, KeyType.Text);
            var Text = new StringBuilder();
            Text.AppendLine("CREATE VIEW [Chk_BillReceivable1] AS ");
            Text.AppendLine(SQLQuery.Chk_BillReceivable1());
            return Text.ToString();
        }


        public string Chk_BillReceivable2()
        {

            AppRegistry.SetKey(UserName, "QueryMessage", string.Empty, KeyType.Text);
            var Text = new StringBuilder();
            Text.AppendLine("CREATE VIEW [Chk_BillReceivable2] AS ");
            Text.AppendLine(SQLQuery.Chk_BillReceivable2());


            return Text.ToString();
        }
        #endregion

        #region Purchased

        private string Purchased()
        {

            AppRegistry.SetKey(UserName, "QueryMessage", string.Empty, KeyType.Text);
            var Text = new StringBuilder();
            Text.AppendLine("CREATE VIEW [view_Purchased] AS ");
            Text.AppendLine(SQLQuery.view_Purchased());


            return Text.ToString();

        }
        #endregion

        #region Sold
        private string Sold()
        {
            AppRegistry.SetKey(UserName, "QueryMessage", string.Empty, KeyType.Text);
            var Text = new StringBuilder();
            Text.AppendLine("CREATE VIEW [view_Sold] AS ");
            Text.AppendLine(SQLQuery.view_Sold());
            return Text.ToString();
        }
        #endregion

        #region Production

        private string Production1()
        {
            AppRegistry.SetKey(UserName, "QueryMessage", string.Empty, KeyType.Text);
            var Text = new StringBuilder();
            Text.AppendLine("CREATE TABLE[Production]( ");
            Text.AppendLine("[ID] INT PRIMARY KEY NOT NULL UNIQUE,");
            Text.AppendLine("[Vou_No] TEXT(10) NOT NULL UNIQUE, ");
            Text.AppendLine("[Vou_Date] DATETIME NOT NULL, ");
            Text.AppendLine("[Batch] NVARCHAR(25) NOT NULL, ");
            Text.AppendLine("[Remarks] NVARCHAR, ");
            Text.AppendLine("[Comments] NVARCHAR),");
            Text.AppendLine("[Status] TEXT(12) NOT NULL DEFAULT Submitted); ");

            return Text.ToString();
        }

        private string Production2()
        {
            AppRegistry.SetKey(UserName, "QueryMessage", string.Empty, KeyType.Text);
            var Text = new StringBuilder();
            Text.AppendLine("CREATE TABLE[Production2](");
            Text.AppendLine("[ID] INT PRIMARY KEY NOT NULL UNIQUE,");
            Text.AppendLine("[TranID] INT NOT NULL REFERENCES[Production]([ID]), ");
            Text.AppendLine("[Stock] INT NOT NULL REFERENCES[Inventory]([ID]), ");
            Text.AppendLine("[Flow] TEXT(3) NOT NULL, ");
            Text.AppendLine("[Qty] DECIMAL NOT NULL, ");
            Text.AppendLine("[UOM] DECIMAL NOT NULL, ");
            Text.AppendLine("[Rate] DECIMAL NOT NULL, ");
            Text.AppendLine("[Remarks] NVARCHAR(100));");

            return Text.ToString();
        }

        private string ProductionView()
        {
            AppRegistry.SetKey(UserName, "QueryMessage", string.Empty, KeyType.Text);
            var Text = new StringBuilder();
            Text.AppendLine("CREATE VIEW [view_Production] AS ");
            Text.AppendLine(SQLQuery.View_Production());
            return Text.ToString();
        }

        #endregion

        #region Accounts (COA)
        private string COA_Map()
        {
            var Text = new StringBuilder();
            Text.AppendLine("CREATE TABLE [COA_Map] (");
            Text.AppendLine("[ID] INT PRIMARY KEY NOT NULL UNIQUE,");
            Text.AppendLine("[COA] INT NOT NULL UNIQUE REFERENCES [COA] ([ID]), ");
            Text.AppendLine("[Stock] INT NOT NULL REFERENCES [Inventory] ([ID]));");
            return Text.ToString();
        }
        #endregion

        #region Stock Category View
        private string StockCategory()
        {
            var Text = new StringBuilder();
            Text.AppendLine("CREATE VIEW [StockCategory] AS ");
            Text.AppendLine("SELECT ");
            Text.AppendLine("[S].[ID],");
            Text.AppendLine("[S].[Code],");
            Text.AppendLine("[S].[Title],");
            Text.AppendLine("[S].[Category],");
            Text.AppendLine("[C].[Code] [CatCode],");
            Text.AppendLine("[C].[Title] [CatTitle]");
            Text.AppendLine("FROM [Inv_SubCategory] [S]");
            Text.AppendLine("LEFT JOIN [Inv_Category] [C] ");
            Text.AppendLine("ON [C].[ID] = [S].[Category]");
            return Text.ToString();
        }

        #endregion

        #region Stock in Hand
        private string StockInHand()
        {
            var Text = new StringBuilder();
            Text.AppendLine("CREATE TABLE [StockInHand] (");
            Text.AppendLine("[StockID] INT, ");
            Text.AppendLine("[GTitle]  NVARCHAR(100),");
            Text.AppendLine("[Vou_No]  NVARCHAR(15),");
            Text.AppendLine("[Vou_Date] DATETIME,");
            Text.AppendLine("[Title] NVARCHAR(100),");
            Text.AppendLine("[PRQty] DECIMAL,");
            Text.AppendLine("[PRAmount] DECIMAL,");
            Text.AppendLine("[SLQty] DECIMAL,");
            Text.AppendLine("[SLAmount] DECIMAL,");
            Text.AppendLine("[PDQty] DECIMAL,");
            Text.AppendLine("[PQAmount] DECIMAL,");
            Text.AppendLine("[NetQty] DECIMAL,");
            Text.AppendLine("[NetAmount] DECIMAL,");
            Text.AppendLine("[AvgRate] DECIMAL,");
            Text.AppendLine("[SoldCost] DECIMAL)");
            return Text.ToString();

        }

        #endregion

        #region Receipts
        public string Receipts()
        {
            var _Text = new StringBuilder();
            _Text.AppendLine("CREATE TABLE[Receipts](");
            _Text.AppendLine("[ID] INT PRIMARY KEY,");
            _Text.AppendLine("[Vou_No] TEXT(10), ");
            _Text.AppendLine("[Vou_Date] DATE NOT NULL, ");
            _Text.AppendLine("[Ref_No]  NVARCHAR(12), ");
            _Text.AppendLine("[COA] INT, ");
            _Text.AppendLine("[COACash] INT, ");
            _Text.AppendLine("[Payer] INT NOT NULL, ");
            _Text.AppendLine("[Project] INT NOT NULL, ");
            _Text.AppendLine("[Employee] INT, ");
            _Text.AppendLine("[Amount] DECIMAL NOT NULL, ");
            _Text.AppendLine("[Description] NVARCHAR NOT NULL,");
            _Text.AppendLine("[Status] NVARCHAR(10) NOT NULL);");
            return _Text.ToString();
        }
        #endregion


    }
}

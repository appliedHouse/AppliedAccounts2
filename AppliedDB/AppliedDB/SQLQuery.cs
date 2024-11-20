using System.Text;
using static AppliedDB.Enums;

namespace AppliedDB
{
    public class SQLQuery
    {
        #region Sales Invoice / Sale Invoice List
        public static string SaleInvoice()
        {
            var Text = new StringBuilder();
            Text.Append("SELECT ");
            Text.Append("[B2].[TranID],");
            Text.Append("[B1].[Vou_No], ");
            Text.Append("[B1].[Vou_Date], ");
            Text.Append("[B1].[Company] AS [CompanyID], ");
            Text.Append("[C].[Code] AS [Code], ");
            Text.Append("[C].[NTN] AS [NTN], ");
            Text.Append("[C].[Title] AS [Company], ");
            Text.Append("[C].[Title] AS [Company], ");
            Text.Append("[B1].[Employee] AS [EmployeeID], ");
            Text.Append("[E].[Title] AS [Employee], ");
            Text.Append("[B2].[Project] AS [ProjectID], ");
            Text.Append("[P].[Title] AS [Project], ");
            Text.Append("[B1].[Ref_No], ");
            Text.Append("[B1].[Inv_No], ");
            Text.Append("[B1].[Inv_Date], ");
            Text.Append("[B1].[Pay_Date], ");
            Text.Append("[B1].[Description], ");
            Text.Append("[B2].[Sr_No], ");
            Text.Append("[B2].[Inventory] AS [InventoryID], ");
            Text.Append("[I].[Title] AS [Inventory], ");
            Text.Append("[B2].[Batch], ");
            Text.Append("[B2].[Qty], ");
            Text.Append("[B2].[Rate], ");
            Text.Append("[B2].[Qty] * [B2].[Rate] AS [Amount], ");
            Text.Append("[T].[Title] AS [Tax], ");
            Text.Append("[T].[Rate] AS [Tax_Rate], ");
            Text.Append("([T].[Rate] / 100) AS [Tax_Rate2], ");
            Text.Append("CAST([B2].[Qty] * [B2].[Rate] AS FLOAT) AS [Amount],");
            Text.Append("(CAST([B2].[Qty] * [B2].[Rate] AS FLOAT) *");
            Text.Append("CAST([T].[Rate] AS FLOAT))/ 100 AS [Tax_Amount],");
            Text.Append("CAST([B2].[Qty] * [B2].[Rate] AS FLOAT) +");
            Text.Append("(CAST([B2].[Qty] * [B2].[Rate] AS FLOAT) *");
            Text.Append("CAST([T].[Rate] AS FLOAT))/ 100 AS [Net_Amount],");
            Text.Append("[B2].[Description] AS [Remarks], ");
            Text.Append("[C].[Address1], ");
            Text.Append("[C].[Address2], ");
            Text.Append("[C].[City] || ' ' || [C].[State] || ' ' || [C].[Country] AS [Address3], ");
            Text.Append("[C].[Phone], ");
            Text.Append("[B1].[Status] ");
            Text.Append("FROM [BillReceivable] [B1] ");
            Text.Append("LEFT JOIN[BillReceivable2] [B2] ON[B2].[TranID] = [B1].[ID] ");
            Text.Append("LEFT JOIN[Customers] [C] ON[C].[ID] = [B1].[Company] ");
            Text.Append("LEFT JOIN[Employees] [E] ON[E].[ID] = [B1].[Employee] ");
            Text.Append("LEFT JOIN[Project] [P] ON[P].[ID] = [B2].[Project] ");
            Text.Append("LEFT JOIN[Inventory] [I] ON[I].[ID] = [B2].[Inventory] ");
            Text.Append("LEFT JOIN[Taxes] [T] ON[T].[ID] = [B2].[Tax] ");
            Text.Append("WHERE TranID=@ID");
            return Text.ToString();

        }
        public static string SaleInvoiceList()
        {
            var _Text = new StringBuilder();
            _Text.Append("SELECT ");
            _Text.Append("[BR].[ID],");
            _Text.Append("[BR].[Vou_No],");
            _Text.Append("[BR].[Ref_No],");
            _Text.Append("[BR].[Vou_Date],");
            _Text.Append("[BR].[Inv_Date],");
            _Text.Append("[BR].[Pay_Date],");
            _Text.Append("[C].[Title] As[Company],");
            _Text.Append("[E].[Title] As[Salesman],");
            _Text.Append("[C].[City] As[City],");
            _Text.Append("[BR].[Amount],");
            _Text.Append("[BR].[Description],");
            _Text.Append("[BR].[Status]");
            _Text.Append("FROM [BillReceivable] [BR]");
            _Text.Append("LEFT JOIN[Customers] [C] ON [C].[ID] = [BR].[Company]");
            _Text.Append("LEFT JOIN[Employees] [E] ON [C].[ID] = [BR].[Employee]");

            return _Text.ToString();
        }
        #endregion

        #region Purchase Invoice / Purchase Invoice List
        public static string PurchaseInvoice()
        {
            var Text = new StringBuilder();
            Text.Append("SELECT ");
            Text.Append("[B2].[TranID],");
            Text.Append("[B1].[Vou_No], ");
            Text.Append("[B1].[Vou_Date], ");
            Text.Append("[B1].[Company] AS [CompanyID], ");
            Text.Append("[C].[Code] AS [Code], ");
            Text.Append("[C].[NTN] AS [NTN], ");
            Text.Append("[C].[Title] AS [Company], ");
            Text.Append("[C].[Title] AS [Company], ");
            Text.Append("[B1].[Employee] AS [EmployeeID], ");
            Text.Append("[E].[Title] AS [Employee], ");
            Text.Append("[B2].[Project] AS [ProjectID], ");
            Text.Append("[P].[Title] AS [Project], ");
            Text.Append("[B1].[Ref_No], ");
            Text.Append("[B1].[Inv_No], ");
            Text.Append("[B1].[Inv_Date], ");
            Text.Append("[B1].[Pay_Date], ");
            Text.Append("[B1].[Description], ");
            Text.Append("[B2].[Sr_No], ");
            Text.Append("[B2].[Inventory] AS [InventoryID], ");
            Text.Append("[I].[Title] AS [Inventory], ");
            Text.Append("[B2].[Batch], ");
            Text.Append("[B2].[Qty], ");
            Text.Append("[B2].[Rate], ");
            Text.Append("[B2].[Qty] * [B2].[Rate] AS [Amount], ");
            Text.Append("[T].[Title] AS [Tax], ");
            Text.Append("[T].[Rate] AS [Tax_Rate], ");
            Text.Append("([T].[Rate] / 100) AS [Tax_Rate2], ");
            Text.Append("CAST([B2].[Qty] * [B2].[Rate] AS FLOAT) AS [Amount],");
            Text.Append("(CAST([B2].[Qty] * [B2].[Rate] AS FLOAT) *");
            Text.Append("CAST([T].[Rate] AS FLOAT))/ 100 AS [Tax_Amount],");
            Text.Append("CAST([B2].[Qty] * [B2].[Rate] AS FLOAT) +");
            Text.Append("(CAST([B2].[Qty] * [B2].[Rate] AS FLOAT) *");
            Text.Append("CAST([T].[Rate] AS FLOAT))/ 100 AS [Net_Amount],");
            Text.Append("[B2].[Description] AS [Remarks], ");
            Text.Append("[C].[Address1], ");
            Text.Append("[C].[Address2], ");
            Text.Append("[C].[City] || ' ' || [C].[State] || ' ' || [C].[Country] AS [Address3], ");
            Text.Append("[C].[Phone], ");
            Text.Append("[B1].[Status] ");
            Text.Append("FROM [BillPayable] [B1] ");
            Text.Append("LEFT JOIN[BillPayable2] [B2] ON[B2].[TranID] = [B1].[ID] ");
            Text.Append("LEFT JOIN[Customers] [C] ON[C].[ID] = [B1].[Company] ");
            Text.Append("LEFT JOIN[Employees] [E] ON[E].[ID] = [B1].[Employee] ");
            Text.Append("LEFT JOIN[Project] [P] ON[P].[ID] = [B2].[Project] ");
            Text.Append("LEFT JOIN[Inventory] [I] ON[I].[ID] = [B2].[Inventory] ");
            Text.Append("LEFT JOIN[Taxes] [T] ON[T].[ID] = [B2].[Tax] ");
            Text.Append("WHERE TranID=@ID");
            return Text.ToString();

        }
        public static string PurchaseInvoiceList()
        {
            var _Text = new StringBuilder();
            _Text.Append("SELECT ");
            _Text.Append("[BP].[ID],");
            _Text.Append("[BP].[Vou_No],");
            _Text.Append("[BP].[Vou_Date],");
            _Text.Append("[BP].[Inv_Date],");
            _Text.Append("[BP].[Pay_Date],");
            _Text.Append("[C].[Title] As[Company],");
            _Text.Append("[E].[Title] As[Employee],");
            _Text.Append("[C].[City] As[City],");
            _Text.Append("[BP].[Amount],");
            _Text.Append("[BP].[Description],");
            _Text.Append("[BP].[Status]");
            _Text.Append("FROM [BillPayable] [BP]");
            _Text.Append("LEFT JOIN[Customers] [C] ON [C].[ID] = [BP].[Company]");
            _Text.Append("LEFT JOIN[Employees] [E] ON [C].[ID] = [BP].[Employee]");

            return _Text.ToString();
        }
        #endregion

        #region Chart of Accounts
        public static string COAList()
        {
            var _Text = new StringBuilder();
            _Text.Append("SELECT");
            _Text.Append("[COA].*,");
            _Text.Append("[C].[Title] [TitleClass],");
            _Text.Append("[T].[Title] [TitleNature],");
            _Text.Append("[N].[Title] [TitleNote]");
            _Text.Append("FROM [COA]");
            _Text.Append("LEFT JOIN[COA_Class]  [C] ON[C].[ID] = [COA].[Class]");
            _Text.Append("LEFT JOIN[COA_Nature] [T] ON[T].[ID] = [COA].[Nature]");
            _Text.Append("LEFT JOIN[COA_Notes]  [N] ON[N].[ID] = [COA].[Notes]");

            return _Text.ToString();
        }
        #endregion

        #region Customers & CustomerList
        public static string Customers()
        {
            var _Text = new StringBuilder();
            _Text.Append("SELECT * FROM [Customers]");
            return _Text.ToString();
        }
        public static string CustomersList()
        {
            var _Text = new StringBuilder();
            _Text.Append("SELECT * FROM [Customers]");
            return _Text.ToString();
        }
        #endregion

        #region COA Class
        public static string COAClassList()
        {
            var _Text = new StringBuilder();
            _Text.Append("SELECT * FROM [COA_Class]");
            return _Text.ToString();
        }
        #endregion

        #region COA Nature
        public static string COANatureList()
        {
            var _Text = new StringBuilder();
            _Text.Append("SELECT * FROM [COA_Nature]");
            return _Text.ToString();
        }
        #endregion

        #region COA Notes
        public static string COANotesList()
        {
            var _Text = new StringBuilder();
            _Text.Append("SELECT * FROM [COA_Notes]");
            return _Text.ToString();
        }
        #endregion

        #region View Purchase Invoice
        public static string ViewPurchaseInvoice()
        {
            var _Text = new StringBuilder();
            _Text.Append("SELECT ");
            _Text.Append("[BP1].[ID] AS[ID],");
            _Text.Append("[BP1].[ID] AS[ID1],");
            _Text.Append("[BP1].[Vou_No],");
            _Text.Append("[BP1].[Vou_Date],");
            _Text.Append("[BP1].[Company],");
            _Text.Append("[BP1].[Employee],");
            _Text.Append("[BP1].[Ref_No],");
            _Text.Append("[BP1].[Inv_No],");
            _Text.Append("[BP1].[Inv_Date],");
            _Text.Append("[BP1].[Inv_Date],");
            _Text.Append("[BP1].[Pay_Date],");
            _Text.Append("[BP1].[Amount],");
            _Text.Append("[BP1].[Description],");
            _Text.Append("[BP1].[Comments],");
            _Text.Append("[BP1].[Status],");
            _Text.Append("[BP2].[Sr_No],");
            _Text.Append("[BP2].[ID] AS[ID2],");
            _Text.Append("[BP2].[TranID],");
            _Text.Append("[BP2].[Inventory],");
            _Text.Append("[BP2].[Batch],");
            _Text.Append("[BP2].[Qty],");
            _Text.Append("[BP2].[Rate],");
            _Text.Append("[BP2].[Tax],");
            _Text.Append("[BP2].[Tax_Rate],");
            _Text.Append("[BP2].[Description] AS[Description2],");
            _Text.Append("[BP2].[Project]");
            _Text.Append("FROM [BillPayable] [BP1]");
            _Text.Append("LEFT JOIN [BillPayable2] [BP2] ON [BP1].[ID] = [BP2].[TranID];");

            return _Text.ToString();


        }
        #endregion

        #region View Sale Invoice
        public static string ViewSaleInvoice()
        {
            var _Text = new StringBuilder();
            _Text.Append("SELECT ");
            _Text.Append("[BP1].[ID] AS[ID],");
            _Text.Append("[BP1].[ID] AS[ID1],");
            _Text.Append("[BP1].[Vou_No],");
            _Text.Append("[BP1].[Vou_Date],");
            _Text.Append("[BP1].[Company],");
            _Text.Append("[BP1].[Employee],");
            _Text.Append("[BP1].[Ref_No],");
            _Text.Append("[BP1].[Inv_No],");
            _Text.Append("[BP1].[Inv_Date],");
            _Text.Append("[BP1].[Inv_Date],");
            _Text.Append("[BP1].[Pay_Date],");
            _Text.Append("[BP1].[Amount],");
            _Text.Append("[BP1].[Description],");
            _Text.Append("[BP1].[Comments],");
            _Text.Append("[BP1].[Status],");
            _Text.Append("[BP2].[Sr_No],");
            _Text.Append("[BP2].[ID] AS[ID2],");
            _Text.Append("[BP2].[TranID],");
            _Text.Append("[BP2].[Inventory],");
            _Text.Append("[BP2].[Batch],");
            _Text.Append("[BP2].[Qty],");
            _Text.Append("[BP2].[Rate],");
            _Text.Append("[BP2].[Tax],");
            _Text.Append("[BP2].[Tax_Rate],");
            _Text.Append("[BP2].[Description] AS[Description2],");
            _Text.Append("[BP2].[Project]");
            _Text.Append("FROM [BillReceivable] [BP1]");
            _Text.Append("LEFT JOIN [BillReceivable2] [BP2] ON [BP1].[ID] = [BP2].[TranID];");

            return _Text.ToString();


        }
        #endregion

        #region Cash or Bank Book from Ledger

        public static string BookLedger(int BookID)
        {
            if (BookID > 1)
            {
                var _Text = new StringBuilder();
                _Text.AppendLine("SELECT ");
                _Text.AppendLine("[BookID],[Vou_Type],[Vou_Date],[Vou_No],[Description],");
                _Text.AppendLine("[DR],[CR]");
                _Text.AppendLine("0.00 AS [BAL]");
                _Text.AppendLine("[Customer]");
                _Text.AppendLine("[Customers].[Title] AS [CustomerTitle],");
                _Text.AppendLine("[Project],");
                _Text.AppendLine("[Project].[Title] As [ProjectTitle],");
                _Text.AppendLine("'' AS [Status]");
                _Text.AppendLine("FROM [Ledger]");
                _Text.AppendLine("LEFT JOIN [Customers] ON [Ledger].[Customer] = [Customers].[ID]");
                _Text.AppendLine("LEFT JOIN [Project]   ON [Ledger].[Project]  = [Project].[ID];");
                _Text.AppendLine($"WHERE [Ledger].[BookID] = {BookID}");

                return _Text.ToString();
            }
            return string.Empty;
        }

        public static string CashBook() { return string.Empty; }
        public static string BankBook() { return string.Empty; }


        #endregion

        public static QueryClass GetQuery(Query _SQLQuery)
        {
            if (_SQLQuery.Equals(Query.SaleInvoice)) { return new QueryClass { QueryText = SaleInvoice(), TableName = "SaleInvoice" }; }
            if (_SQLQuery.Equals(Query.SaleInvoiceList)) { return new QueryClass { QueryText = SaleInvoiceList(), TableName = "SaleInvoiceList" }; }
            if (_SQLQuery.Equals(Query.SaleInvoiceView)) { return new QueryClass { QueryText = ViewSaleInvoice(), TableName = "SaleInvoiceView" }; }
            if (_SQLQuery.Equals(Query.PurchaseInvoice)) { return new QueryClass { QueryText = PurchaseInvoice(), TableName = "PurchaseInvoice" }; }
            if (_SQLQuery.Equals(Query.PurchaseInvoiceList)) { return new QueryClass { QueryText = PurchaseInvoiceList(), TableName = "PurchaseInvoiceList" }; }
            if (_SQLQuery.Equals(Query.PurchaseInvoiceView)) { return new QueryClass { QueryText = ViewPurchaseInvoice(), TableName = "PurchaseInvoiceView" }; }
            if (_SQLQuery.Equals(Query.COAList)) { return new QueryClass { QueryText = COAList(), TableName = "COAList" }; }
            if (_SQLQuery.Equals(Query.Customers)) { return new QueryClass { QueryText = Customers(), TableName = Tables.Customers.ToString() }; }
            if (_SQLQuery.Equals(Query.CustomersList)) { return new QueryClass { QueryText = CustomersList(), TableName = "CustomersList" }; }
            if (_SQLQuery.Equals(Query.COAClassList)) { return new QueryClass { QueryText = COAClassList(), TableName = Tables.COA_Class.ToString() }; }
            if (_SQLQuery.Equals(Query.COANatureList)) { return new QueryClass { QueryText = COANatureList(), TableName = Tables.COA_Nature.ToString() }; }
            if (_SQLQuery.Equals(Query.COANotesList)) { return new QueryClass { QueryText = COANotesList(), TableName = Tables.COA_Notes.ToString() }; }

            return new QueryClass();
        }

    }
    public class QueryClass
    {
        public string QueryText { get; set; } = string.Empty;
        public string TableName { get; set; } = string.Empty;
    }

}

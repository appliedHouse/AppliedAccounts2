using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SQLQueries
{
    public class Quries
    {
        #region Receipt
        public static string Receipt(int _ReceiptID)
        {
            var _Text = new StringBuilder();
            _Text.AppendLine("SELECT [R].*,[R2].*,");
            _Text.AppendLine("[C].[Title] AS [PayerTitle],");
            _Text.AppendLine("[E].[Title] AS [EmployeeTitle], ");
            _Text.AppendLine("[A].[Title] AS [AccountTitle], ");
            _Text.AppendLine("[P].[Title] As [ProjectTitle]");
            _Text.AppendLine("FROM [Receipt2] [R2]");
            _Text.AppendLine("LEFT JOIN [Receipt]   [R] ON [R].[ID] = [R2].[TranID]");
            _Text.AppendLine("LEFT JOIN [COA]       [A] ON [A].[ID] = [R].[COA]");
            _Text.AppendLine("LEFT JOIN [Project]   [P] ON [P].[ID] = [R2].[Project]");
            _Text.AppendLine("LEFT JOIN [Employees] [E] ON [E].[ID] = [R2].[Employee]");
            _Text.AppendLine("LEFT JOIN [Customers] [C] ON [C].[ID] = [R].[Payer]");
            _Text.AppendLine($"WHERE [R].[ID] = {_ReceiptID}");
            return _Text.ToString();
        }

        public static string ReceiptList(string _Filter)
        {
            var _Text = new StringBuilder();
            _Text.AppendLine("SELECT ");
            _Text.AppendLine("[R].*,");
            _Text.AppendLine("[A].[Title] AS[TitleAccount],");
            _Text.AppendLine("[C].[Title] AS[TitlePayer]");
            _Text.AppendLine("FROM[Receipt][R]");
            _Text.AppendLine("LEFT JOIN[COA] [A] ON[A].[ID] = [R].[COA]");
            _Text.AppendLine("LEFT JOIN[Customers] [C] ON[C].[ID] = [R].[Payer]");
            _Text.AppendLine("");
            if (!string.IsNullOrEmpty(_Filter))
            {
                _Text.AppendLine("WHERE " + _Filter);
            }
            return _Text.ToString();
        }

        public static string Receipts()
        {
            var _Text = new StringBuilder();
            _Text.AppendLine("SELECT * FROM [Receipts]");
            return _Text.ToString();
        }

        #endregion

        #region Sale Invoice / Bill Receivable
        public static string SaleInvoice(int _ID)
        {
            var _Text = new StringBuilder();
            _Text.AppendLine("SELECT * FROM (");
            _Text.AppendLine("SELECT [B2].[TranID],");
            _Text.AppendLine("[B1].[Vou_No],");
            _Text.AppendLine("[B1].[Vou_Date],");
            _Text.AppendLine("[C].[Title] AS[Company],");
            _Text.AppendLine("[E].[Title] AS[Employee],");
            _Text.AppendLine("[P].[Title] AS[Project],");
            _Text.AppendLine("[B1].[Ref_No],");
            _Text.AppendLine("[B1].[Inv_No],");
            _Text.AppendLine("[B1].[Inv_Date],");
            _Text.AppendLine("[B1].[Pay_Date],");
            _Text.AppendLine("[B1].[Description],");
            _Text.AppendLine("[B2].[Sr_No],");
            _Text.AppendLine("[I].[Title] AS[Inventory],");
            _Text.AppendLine("[B2].[Batch],");
            _Text.AppendLine("[B2].[Unit],");
            _Text.AppendLine("[U].[Title] AS [UnitTitle],");
            _Text.AppendLine("[B2].[Qty] AS [Qty],");
            _Text.AppendLine("[B2].[Rate],");
            _Text.AppendLine("[B2].[Qty] * [B2].[Rate] AS[Amount],");
            _Text.AppendLine("[T].[Rate] AS[Tax_Rate],");
            _Text.AppendLine("([B2].[Qty] * [B2].[Rate]) * [T].[Rate] AS[Tax_Amount],");
            _Text.AppendLine("([B2].[Qty] * [B2].[Rate]) + (([B2].[Qty] * [B2].[Rate]) * [T].[Rate]) AS[Net_Amount],");
            _Text.AppendLine("[B2].[Description] AS[Remarks], [C].[Address1], [C].[Address2], [C].[City] || ' ' || [C].[State] || ' ' || [C].[Country] AS[Address3], ");
            _Text.AppendLine("[C].[Phone]");
            _Text.AppendLine("FROM [BillReceivable] [B1] ");
            _Text.AppendLine("LEFT JOIN [BillReceivable2] [B2] ON [B2].[TranID] = [B1].[ID] ");
            _Text.AppendLine("LEFT JOIN [Customers]        [C] ON [C].[ID]      = [B1].[Company] ");
            _Text.AppendLine("LEFT JOIN [Employees]        [E] ON [E].[ID]      = [B1].[Employee] ");
            _Text.AppendLine("LEFT JOIN [Inv_UOM]          [U] ON [U].[ID]      = [B2].[Unit] ");
            _Text.AppendLine("LEFT JOIN [Project]          [P] ON [P].[ID]      = [B2].[Project] ");
            _Text.AppendLine("LEFT JOIN [Inventory]        [I] ON [I].[ID]      = [B2].[Inventory] ");
            _Text.AppendLine("LEFT JOIN [Taxes]            [T] ON [T].[ID]      = [B2].[Tax]");
            _Text.AppendLine(") AS [SaleInvoice] ");
            _Text.AppendLine($"WHERE [TranID] = {_ID}");
            return _Text.ToString();
        }
        #endregion

        #region View of Purchased Invoice
        public static string ViewPurchaseInvoice(string _Filter)
        {
            var _Text = new StringBuilder();
            _Text.AppendLine("SELECT * FROM ( ");
            _Text.AppendLine("SELECT ");
            _Text.AppendLine("[BP1].[ID] AS [ID1],");
            _Text.AppendLine("[BP1].[Vou_No],");
            _Text.AppendLine("[BP1].[Vou_Date],");
            _Text.AppendLine("[BP1].[Company],");
            _Text.AppendLine("[C].[Title] AS [CompanyTitle],");
            _Text.AppendLine("[BP1].[Employee],");
            _Text.AppendLine("[E].[Title] AS [EmployeeTitle],");
            _Text.AppendLine("[BP1].[Ref_No],");
            _Text.AppendLine("[BP1].[Inv_No],");
            _Text.AppendLine("[BP1].[Inv_Date],");
            _Text.AppendLine("[BP1].[Pay_Date],");
            _Text.AppendLine("[BP1].[Amount],");
            _Text.AppendLine("[BP1].[Description] AS [Remarks],");
            _Text.AppendLine("[BP1].[Comments],");
            _Text.AppendLine("[BP1].[Status],");
            _Text.AppendLine("[BP2].[ID] AS [ID2],");
            _Text.AppendLine("[BP2].[Sr_No],");
            _Text.AppendLine("[BP2].[TranID],");
            _Text.AppendLine("[BP2].[Inventory],");
            _Text.AppendLine("[I].[Title] AS [InventoryTitle],");
            _Text.AppendLine("[BP2].[Batch],");
            _Text.AppendLine("[BP2].[Qty],");
            _Text.AppendLine("[BP2].[Rate],");
            _Text.AppendLine("[BP2].[Tax],");
            _Text.AppendLine("[BP2].[Tax_Rate],");
            _Text.AppendLine("[BP2].[Description],");
            _Text.AppendLine("[BP2].[Project]");
            _Text.AppendLine("FROM [BillPayable] [BP1]");
            _Text.AppendLine("LEFT JOIN [BillPayable2] [BP2] ON [BP1].[ID] = [BP2].[TranID]");
            _Text.AppendLine("LEFT JOIN [Customers]      [C] ON   [C].[ID] = [BP1].[Company]");
            _Text.AppendLine("LEFT JOIN [Employees]      [E] ON   [E].[ID] = [BP1].[Employee]");
            _Text.AppendLine("LEFT JOIN [Inventory]      [I] ON   [I].[ID] = [BP2].[Inventory]");
            _Text.AppendLine("LEFT JOIN [Taxes]          [T] ON   [T].[ID] = [BP2].[Tax]");
            _Text.AppendLine(" ) [Purchased] ");

            if (!string.IsNullOrEmpty(_Filter))
            {
                _Text.AppendLine("WHERE " + _Filter);
            }

            return _Text.ToString();
        }
        #endregion

        #region General Ledger
        public static string GeneralLedger(int COAID, string OBDate, string FilterOB, string GroupBy, string Filter, string OrderBy)
        {
            var _Text = new StringBuilder();
            var _Text1 = new StringBuilder();           // Opening Balance
            var _Text2 = new StringBuilder();           // Ledger
            var _Text3 = new StringBuilder();           // Combine Opening and Ledger

            // Ledger Opening Balance One DataRow
            _Text1.AppendLine("SELECT");
            _Text1.AppendLine("0 AS[Vou_No],");
            _Text1.AppendLine($"Date('{OBDate}') AS[Vou_Date],");
            _Text1.AppendLine("IIF([BAL] > 0, [BAL], 0) AS[DR],");
            _Text1.AppendLine("IIF([BAL] < 0, ABS([BAL]), 0) AS[CR],");
            _Text1.AppendLine("'Opening Balance...' AS[Description],");
            _Text1.AppendLine($"{COAID} AS[COA],");
            _Text1.AppendLine("0 AS[Customer],");
            _Text1.AppendLine("0 AS[Project],");
            _Text1.AppendLine("0 AS[Employee],");
            _Text1.AppendLine("0 AS[Inventory]");
            _Text1.AppendLine("FROM(SELECT");
            _Text1.AppendLine("SUM([DR]) AS[DR],");
            _Text1.AppendLine("SUM([CR]) AS[CR],");
            _Text1.AppendLine("SUM([DR] - [CR]) AS[BAL]");
            _Text1.AppendLine("FROM [Ledger]");
            _Text1.AppendLine($"WHERE {FilterOB}) ");
            _Text1.AppendLine($"GROUP BY {GroupBy} ");

            // Ledger DataRow from Start to End Date
            _Text2.AppendLine("SELECT");
            _Text2.AppendLine("[Ledger].[Vou_No],");
            _Text2.AppendLine("[Ledger].[Vou_Date],");
            _Text2.AppendLine("[Ledger].[DR],");
            _Text2.AppendLine("[Ledger].[CR],");
            _Text2.AppendLine("[Ledger].[Description],");
            _Text2.AppendLine("[Ledger].[COA],");
            _Text2.AppendLine("[Ledger].[Customer],");
            _Text2.AppendLine("[Ledger].[Project],");
            _Text2.AppendLine("[Ledger].[Employee],");
            _Text2.AppendLine("[Ledger].[Inventory]");
            _Text2.AppendLine("FROM [Ledger]");
            _Text2.AppendLine($"WHERE {Filter} ");


            _Text3.AppendLine("SELECT [L].*,");
            _Text3.AppendLine("[A].[TITLE] AS [AccountTitle],");
            _Text3.AppendLine("[C].[TITLE] AS [CompanyName],");
            _Text3.AppendLine("[E].[TITLE] AS [EmployeeName],");
            _Text3.AppendLine("[P].[TITLE] AS [ProjectTitle],");
            _Text3.AppendLine("[I].[TITLE] AS [StockTitle]");
            _Text3.AppendLine($"FROM(");

            _Text3.AppendLine(_Text1.ToString());
            _Text3.AppendLine("UNION ALL");
            _Text3.AppendLine(_Text2.ToString());


            _Text3.AppendLine(") AS [L]");
            _Text3.AppendLine("LEFT JOIN [COA]       [A] ON [A].[ID] = [L].[COA]");
            _Text3.AppendLine("LEFT JOIN [Customers] [C] ON [C].[ID] = [L].[CUSTOMER]");
            _Text3.AppendLine("LEFT JOIN [Employees] [E] ON [E].[ID] = [L].[EMPLOYEE]");
            _Text3.AppendLine("LEFT JOIN [Project]   [P] ON [P].[ID] = [L].[PROJECT]");
            _Text3.AppendLine("LEFT JOIN [Inventory] [I] ON [I].[ID] = [L].[INVENTORY]");
            if (OrderBy.Length > 0) { _Text3.AppendLine($"ORDER BY {OrderBy}"); }

            return _Text3.ToString();
        }

        #endregion

        #region View Sales Invoice (Bill Receivable)
        public static string BillReceivable(string Filter)
        {
            var _Text = new StringBuilder();
            _Text.AppendLine("SELECT* FROM(");
            _Text.AppendLine("SELECT ");
            _Text.AppendLine("[B1].[ID] AS[ID],");
            _Text.AppendLine("[B1].[ID] AS[ID1],");
            _Text.AppendLine("[B1].[Vou_No],");
            _Text.AppendLine("[B1].[Vou_Date],");
            _Text.AppendLine("[B1].[Company],");
            _Text.AppendLine("[B1].[Employee],");
            _Text.AppendLine("[B1].[Ref_No],");
            _Text.AppendLine("[B1].[Inv_No],");
            _Text.AppendLine("[B1].[Inv_Date],");
            _Text.AppendLine("[B1].[Pay_Date],");
            _Text.AppendLine("[B1].[Amount],");
            _Text.AppendLine("[B1].[Description] AS [Remarks],");
            _Text.AppendLine("[B1].[Comments],");
            _Text.AppendLine("[B1].[Status],");
            _Text.AppendLine("[B2].[ID] AS[ID2],");
            _Text.AppendLine("[B2].[Sr_No],");
            _Text.AppendLine("[B2].[TranID],");
            _Text.AppendLine("[B2].[Inventory],");
            _Text.AppendLine("[B2].[Batch],");
            _Text.AppendLine("[B2].[Unit],");
            _Text.AppendLine("[B2].[Qty],");
            _Text.AppendLine("[B2].[Rate],");
            _Text.AppendLine("[B2].[Tax],");
            _Text.AppendLine("[B2].[Tax_Rate],");
            _Text.AppendLine("[B2].[Description] AS [Description],");
            _Text.AppendLine("[B2].[Project],");
            _Text.AppendLine("[C].[Title] AS [TitleSupplier],");
            _Text.AppendLine("[E].[Title] AS [TitleEmployee],");
            _Text.AppendLine("[I].[Title] AS [TitleStock],");
            _Text.AppendLine("[U].[Title] AS [TitleUnit],");
            _Text.AppendLine("[P].[Title] As [TitleProject],");
            _Text.AppendLine("[T].[Title] As [TitleTax]");
            _Text.AppendLine("FROM [BillReceivable] [B1]");
            _Text.AppendLine("LEFT JOIN [BillReceivable2][B2] ON [B1].[ID] = [B2].[TranID]");
            _Text.AppendLine("LEFT JOIN [Customers]      [C]  ON  [C].[ID] = [B1].[Company]");
            _Text.AppendLine("LEFT JOIN [Employees]      [E]  ON  [E].[ID] = [B1].[Employee]");
            _Text.AppendLine("LEFT JOIN [Inventory]      [I]  ON  [I].[ID] = [B2].[Inventory]");
            _Text.AppendLine("LEFT JOIN [Inv_UOM]        [U]  ON  [U].[ID] = [B2].[Unit]");
            _Text.AppendLine("LEFT JOIN [Project]        [P]  ON  [P].[ID] = [B2].[Project]");
            _Text.AppendLine("LEFT JOIN [Taxes]          [T]  ON  [T].[ID] = [B2].[Tax]");
            _Text.AppendLine(") AS [SalesInvoice]");
            if (Filter.Length > 0)
            {
                _Text.AppendLine($"WHERE {Filter}");
            }
            return _Text.ToString();
        }

        #endregion

        #region Chart of Accounts CRUD
        public static string COA()
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

        #region Inventory
        public static string Inventory()
        {
            var _Text = new StringBuilder();
            _Text.AppendLine("SELECT [I].*,");
            _Text.AppendLine("[P].[Title] [TitlePacking],");
            _Text.AppendLine("[U].[Title] [TitleUOM],");
            _Text.AppendLine("[S].[Title] [TitleSubCategory],");
            _Text.AppendLine("[C].[Title] [TitleCategory],");
            _Text.AppendLine("[Z].[Title] [TitleSize]");
            _Text.AppendLine("FROM [Inventory] [I]");
            _Text.AppendLine("LEFT JOIN [Inv_Packing]     [P] ON [P].[ID] = [I].[Packing]");
            _Text.AppendLine("LEFT JOIN [Inv_UOM]         [U] ON [U].[ID] = [I].[UOM]");
            _Text.AppendLine("LEFT JOIN [Inv_SubCategory] [S] ON [S].[ID] = [I].[SubCategory]");
            _Text.AppendLine("LEFT JOIN [Inv_Category]    [C] ON [C].[ID] = [S].[Category]");
            _Text.AppendLine("LEFT JOIN [Inv_Size]        [Z] ON [Z].[ID] = [I].[Size]");
            return _Text.ToString();

        }
        #endregion

        #region Revenue Graph
        public static string RevenueGraph(string _Batch)
        {
            var _Text = new StringBuilder();
            _Text.AppendLine("SELECT");
            _Text.AppendLine("[A].[Inventory],");
            _Text.AppendLine("[I].[Title] AS [Title],");
            _Text.AppendLine("SUM(ROUND([A].[Qty] * [A].[Rate])) AS [Amount]");
            _Text.AppendLine("FROM (");
            _Text.AppendLine($"SELECT * FROM [view_BillReceivable] WHERE [Ref_No] = '{_Batch}'");
            _Text.AppendLine(") [A]");
            _Text.AppendLine("LEFT JOIN [Inventory] [I] ON[I].[ID] = [A].[Inventory]");
            _Text.AppendLine("GROUP BY [A].[Inventory]");
            return _Text.ToString();

        }

        public static string BatchesForGraph()
        {
            return "SELECT [Ref_No] AS [Batch] FROM [BillReceivable] GROUP BY [Ref_No] ORDER BY [Ref_No] DESC LIMIT 5;";
        }

        public static string Ledger2(string _FilterOB, string _Filter, string _Groupby, string OBDate, string _OrderBy)
        {
            var _Text1 = new StringBuilder();           // Opening Balance
            var _Text2 = new StringBuilder();           // Ledger
            var _Text3 = new StringBuilder();           // Combine Opening and Ledger

            // Ledger Opening Balance One DataRow
            _Text1.AppendLine("SELECT");
            _Text1.AppendLine("0 AS[Vou_No],");
            _Text1.AppendLine($"Date('{OBDate}') AS[Vou_Date],");
            _Text1.AppendLine("IIF([BAL] > 0, [BAL], 0) AS[DR],");
            _Text1.AppendLine("IIF([BAL] < 0, ABS([BAL]), 0) AS[CR],");
            _Text1.AppendLine("'Opening Balance...' AS[Description],");
            _Text1.AppendLine("0 AS[COA],");
            _Text1.AppendLine("0 AS[Customer],");
            _Text1.AppendLine("0 AS[Project],");
            _Text1.AppendLine("0 AS[Employee],");
            _Text1.AppendLine("0 AS[Inventory]");
            _Text1.AppendLine("FROM(SELECT");
            _Text1.AppendLine("SUM([DR]) AS[DR],");
            _Text1.AppendLine("SUM([CR]) AS[CR],");
            _Text1.AppendLine("SUM([DR] - [CR]) AS[BAL]");
            _Text1.AppendLine("FROM [Ledger]");
            _Text1.AppendLine($"WHERE {_FilterOB}) ");
            _Text1.AppendLine($"GROUP BY {_Groupby} ");

            // Ledger DataRow from Start to End Date
            _Text2.AppendLine("SELECT");
            _Text2.AppendLine("[Ledger].[Vou_No],");
            _Text2.AppendLine("[Ledger].[Vou_Date],");
            _Text2.AppendLine("[Ledger].[DR],");
            _Text2.AppendLine("[Ledger].[CR],");
            _Text2.AppendLine("[Ledger].[Description],");
            _Text2.AppendLine("[Ledger].[COA],");
            _Text2.AppendLine("[Ledger].[Customer],");
            _Text2.AppendLine("[Ledger].[Project],");
            _Text2.AppendLine("[Ledger].[Employee],");
            _Text2.AppendLine("[Ledger].[Inventory]");
            _Text2.AppendLine("FROM[Ledger]");
            _Text2.AppendLine($"WHERE {_Filter} ");


            _Text3.AppendLine("SELECT[L].*,");
            _Text3.AppendLine("[A].[TITLE] AS[AccountTitle],");
            _Text3.AppendLine("[C].[TITLE] AS[CompanyName],");
            _Text3.AppendLine("[E].[TITLE] AS[EmployeeName],");
            _Text3.AppendLine("[P].[TITLE] AS[ProjectTitle],");
            _Text3.AppendLine("[I].[TITLE] AS[StockTitle]");
            _Text3.AppendLine($"FROM(");

            _Text3.AppendLine(_Text1.ToString());
            _Text3.AppendLine("UNION ALL");
            _Text3.AppendLine(_Text2.ToString());


            _Text3.AppendLine(") AS[L]");
            _Text3.AppendLine("LEFT JOIN[COA]       [A] ON[A].[ID] = [L].[COA]");
            _Text3.AppendLine("LEFT JOIN[Customers] [C] ON[C].[ID] = [L].[CUSTOMER]");
            _Text3.AppendLine("LEFT JOIN[Employees] [E] ON[E].[ID] = [L].[EMPLOYEE]");
            _Text3.AppendLine("LEFT JOIN[Project]   [P] ON[P].[ID] = [L].[PROJECT]");
            _Text3.AppendLine("LEFT JOIN[Inventory] [I] ON[I].[ID] = [L].[INVENTORY]");
            if (_OrderBy.Length > 0) { _Text3.AppendLine($"ORDER BY {_OrderBy}"); }

            return _Text3.ToString();

        }
        #endregion

        #region Trial Balance
        public static string TrialBalance(string _Filter, string _OrderBy)
        {
            var Text = new StringBuilder();
            Text.AppendLine("SELECT * FROM (");
            Text.AppendLine("SELECT [Ledger].[COA], [COA].[Code], [COA].[Title], ");
            Text.AppendLine("SUM([Ledger].[DR]) AS [DR], ");
            Text.AppendLine("SUM([Ledger].[CR]) AS [CR], ");
            Text.AppendLine("SUM([Ledger].[DR] - [Ledger].[CR]) AS [BAL] ");
            Text.AppendLine("FROM [Ledger] ");
            Text.AppendLine("LEFT JOIN[COA] ON[COA].[ID] = [Ledger].[COA] ");
            if (_Filter.Length > 0) { Text.AppendLine($" WHERE {_Filter} "); }
            Text.AppendLine("GROUP BY [COA] ");
            Text.AppendLine(") WHERE BAL <> 0 ");
            if (_OrderBy.Length > 0) { Text.AppendLine($" ORDER BY {_OrderBy}"); }

            return Text.ToString();
        }
        #endregion

    }
}

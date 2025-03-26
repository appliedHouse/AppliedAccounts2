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

        public static string BookLedger(int _BookID)
        {
            // Cash or Bank record from ledger table.
            if (_BookID > 0)
            {
                //CashBook / BankBook Books Data from Ledger
                // Select * from [LEDGER] WHERE bookID = _BookID


                //var _Text = new StringBuilder();
                //_Text.AppendLine($"BookID = {BookID}");
                //return View_Book(_Text.ToString()); //   _Text.ToString();
            }
            return string.Empty;
        }

        public static string BookView(int BookID)
        {
            // Cash and Bank record from Data table  view view_Book
            if (BookID > 0)
            {
                var _Text = new StringBuilder();
                _Text.AppendLine("SELECT * FROM [view_Book] ");
                _Text.AppendLine($"WHERE BookID = {BookID}");
                return _Text.ToString(); //   _Text.ToString();
            }
            return string.Empty;
        }

        //public static string BookVoucher(int _ID)
        //{
        //    if (_ID > 0)
        //    {
        //        var _Text = new StringBuilder();
        //        _Text.AppendLine($"TranID = {_ID}");
        //        //return View_Book(_Text.ToString()); //   _Text.ToString();
        //    }
        //    return string.Empty;
        //}

        #endregion

        #region Production View
        public static string View_Production()
        {
            var Text = new StringBuilder();
            Text.Append("SELECT ");
            Text.Append("[P1].[ID] AS ID1,[P1].[Vou_no],[P1].[Vou_Date],[P1].[Batch],[P1].[Remarks],[P1].[Comments], ");
            Text.Append("[P2].[ID] AS ID2,[P2].[TranID],[P2].[Flow],[P2].[Stock],[P2].[Qty],[P2].[Rate],");
            Text.Append("([P2].[Qty] * [P2].[Rate]) AS [Amount],");
            Text.Append("[P2].[Remarks] AS [Remarks2], ");
            Text.Append("[I].[Title] AS [StockTitle], ");
            Text.Append("[U].[Code] As [UnitTag], [U].[Title] As [UnitTitle], ");
            Text.Append("IIF (LENGTH ([L].[Vou_No]) > 0, 'Posted', 'Submitted') AS [Status] ");
            Text.Append("FROM [Production2] [P2]");
            Text.Append("LEFT JOIN [Production] [P1] ON [P2].[TranID] = [P1].[ID] ");
            Text.Append("LEFT JOIN [Inventory]  [I]  ON [I].[ID]      = [P2].[Stock] ");
            Text.Append("LEFT JOIN [Inv_UOM]    [U]  ON [U].[ID]      = [I].[UOM] ");
            Text.Append("LEFT JOIN [Ledger]     [L]  ON [L].[Vou_No]  = [P1].[Vou_No] ");

            return Text.ToString();
        }
        #endregion

        #region View Sold
        public static string view_Sold()
        {
            var Text = new StringBuilder();

            Text.Append("SELECT [B1].[Vou_No], ");
            Text.Append("[B1].[Vou_Date], ");
            Text.Append("[B2].[Inventory], ");
            Text.Append("[I].[Title] AS [StockTitle],");
            Text.Append("[B2].[Qty] * -1 AS [Qty], ");
            Text.Append("[B2].[Rate] * -1 AS [Rate], ");
            Text.Append("[B2].[Qty] *[B2].[Rate] * -1 AS [Amount], ");
            Text.Append("[T].[Rate] *-1 AS [TaxRate], ");
            Text.Append("CAST(([B2].[Qty] *[B2].[Rate]*-1) * ([T].[Rate]*-1) AS Float) AS [TaxAmount] ");
            Text.Append("FROM [BillReceivable] [B1] ");
            Text.Append("LEFT JOIN [BillReceivable2] [B2] ON [B2].[TranID] = [B1].[ID] ");
            Text.Append("LEFT JOIN [Taxes] [T] ON [T].[ID] = [B2].[Tax]; ");
            Text.Append("LEFT JOIN [Inventory] [I] ON [I].[ID] = [B2].[Inventory]; ");

            return Text.ToString();
        }
        #endregion

        #region Bill Payable Combine (Purchased) View
        public static string view_Purchased()
        {
            var Text = new StringBuilder();

            Text.Append("SELECT [B1].[Vou_No], [B1].[Vou_Date], ");
            Text.Append("[B2].[Inventory], [B2].[Qty], [B2].[Rate], [B2].[Qty] *[B2].[Rate] AS [Amount], ");
            Text.Append("[T].[Rate] AS [TaxRate], ");
            Text.Append("CAST(([B2].[Qty] *[B2].[Rate]) * [T].[Rate] AS Float) AS [TaxAmount] ");
            Text.Append("FROM [BillPayable] [B1] ");
            Text.Append("LEFT JOIN [BillPayable2] [B2] ON [B2].[TranID] = [B1].[ID] ");
            Text.Append("LEFT JOIN [Taxes] [T] ON [T].[ID] = [B2].[Tax]; ");

            return Text.ToString();
        }
        #endregion

        #region  Check Bill Receivable
        public static string Chk_BillReceivable1()
        {
            // Query show record which has BillReceivable record by not any records in BillReceivable2
            var Text = new StringBuilder();
            Text.Append("SELECT* FROM (");
            Text.Append("SELECT ");
            Text.Append("[B1].[ID] AS[ID1],");
            Text.Append("[B2].[ID] AS[ID2],");
            Text.Append("[B2].[TranID],");
            Text.Append("[B1].[Vou_No],");
            Text.Append("[B1].[Vou_Date] ");
            Text.Append("FROM (SELECT* FROM [BillReceivable] WHERE [Status]= 'Posted') AS [B1]");
            Text.Append(" LEFT JOIN [BillReceivable2] [B2] ON [B2].[TranID] = [B1].[ID]");
            Text.Append(") ");
            Text.Append(" WHERE[ID2] IS NULL");


            return Text.ToString();
        }

        public static string Chk_BillReceivable2()
        {
            var Text = new StringBuilder();
            Text.Append("SELECT");
            Text.Append("[B].[Vou_No][Bill_VNo],");
            Text.Append("[B].[Vou_Date],");
            Text.Append("[B].[Company],");
            Text.Append("[B].[Ref_No],");
            Text.Append("[B].[Amount],");
            Text.Append("[B].[Status],");
            Text.Append("[L].[Vou_No][Led_VNo],");
            Text.Append("[L].[DR],");
            Text.Append("[L].[CR]");

            Text.Append("FROM[BillReceivable][B]");
            Text.Append("LEFT JOIN(SELECT* FROM [Ledger] WHERE Vou_Type = 'Receivable') [L] ON[L].[TranID] = [B].[ID]");
            Text.Append("WHERE[L].[TRanID] IS NULL and[B].[Status] = 'Posted'");

            return Text.ToString();
        }
        #endregion

        #region Stock Position Data (In Hand)
        public static string StockPosition(string Filter)
        {
            var Text = new StringBuilder();
            Text.Append("SELECT * FROM ");
            Text.Append($"({StockPositionData(Filter)})");
            return Text.ToString();
        }
        public static string StockPositionData(string Filter)
        {
            var Text = new StringBuilder();
            Text.Append("SELECT * FROM (");
            Text.Append("SELECT * FROM ( SELECT ");
            Text.Append("'PURCHASED' AS [TRAN],");
            Text.Append("[B1].[Vou_No],");
            Text.Append("[B1].[Vou_Date],");
            Text.Append("[B2].[Inventory],");
            Text.Append("[B2].[Qty],");
            Text.Append("[B2].[Rate],");
            Text.Append("[B2].[Qty] * [B2].[Rate] AS [Amount],");
            Text.Append("[T].[Rate] AS [TaxRate],");
            Text.Append("([B2].[Qty] * [B2].[Rate]) * [T].[Rate] AS [TaxAmount],");
            Text.Append("([B2].[Qty] * [B2].[Rate]) + (([B2].[Qty] * [B2].[Rate]) * [T].[Rate]) AS [NetAmount] ");
            Text.Append("FROM [BillPayable] [B1] ");
            Text.Append("LEFT JOIN [BillPayable2] [B2] ON [B1].[ID] = [B2].[TranID] ");
            Text.Append("LEFT JOIN Taxes [T] On [T].[ID] = [B2].[Tax] ");
            Text.Append(") AS [Purchased] ");
            Text.Append(" UNION ");
            Text.Append("SELECT * FROM ");
            Text.Append("(SELECT ");
            Text.Append("'SOLD' AS [TRAN], ");
            Text.Append("[B1].[Vou_No], ");
            Text.Append("[B1].[Vou_Date], ");
            Text.Append("[B2].[Inventory], ");
            Text.Append("[B2].[Qty], ");
            Text.Append("[B2].[Rate], ");
            Text.Append("[B2].[Qty] * [B2].[Rate] AS [Amount], ");
            Text.Append("[T].[Rate] AS [TaxRate], ");
            Text.Append("([B2].[Qty] * [B2].[Rate]) * [T].[Rate] AS [TaxAmount], ");
            Text.Append("([B2].[Qty] * [B2].[Rate]) + (([B2].[Qty] * [B2].[Rate]) * [T].[Rate]) AS [NetAmount] ");
            Text.Append("FROM [BillReceivable2] [B2] ");
            Text.Append("LEFT JOIN [BillReceivable] [B1] ON [B1].[ID] = [B2].[TranID] ");
            Text.Append("LEFT JOIN Taxes [T] On [T].[ID] = [B2].[Tax] ");
            Text.Append(") AS [Sold] ");
            Text.Append("UNION ");
            Text.Append("SELECT * FROM ");
            Text.Append("(SELECT ");
            Text.Append("'SRETURN' AS [TRAN], ");
            Text.Append("[SR].[Vou_No], ");
            Text.Append("[SR].[Vou_Date], ");
            Text.Append("[B2].[Inventory], ");
            Text.Append("[SR].[Qty], ");
            Text.Append("[B2].[Rate], ");
            Text.Append("[B2].[Qty] * [B2].[Rate] AS [Amount], ");
            Text.Append("[T].[Rate] AS [TaxRate], ");
            Text.Append("([B2].[Qty] * [B2].[Rate]) * [T].[Rate] AS [TaxAmount], ");
            Text.Append("([B2].[Qty] * [B2].[Rate]) + (([B2].[Qty] * [B2].[Rate]) * [T].[Rate]) AS [NetAmount] ");
            Text.Append("FROM [SaleReturn] [SR] ");
            Text.Append("LEFT JOIN BillReceivable2 [B2] ON [B2].[ID] = [SR].[TranID] ");
            Text.Append("LEFT JOIN BillReceivable   [B1] ON [B1].[ID] = [B2].[TranID] ");
            Text.Append("LEFT JOIN Taxes                 [T]   ON [T].[ID]   = [B2].[Tax] ");
            Text.Append(") AS [SRETURN] ) ");
            if (string.IsNullOrEmpty(Filter)) { Text.Append($" WHERE {Filter}"); }
            return Text.ToString();


        }
        public static string StockPositionSUM(string Filter)
        {
            var Text = new StringBuilder();
            Text.Append("SELECT * FROM(");
            Text.Append("SELECT ");
            Text.Append("[Inventory],");
            Text.Append("SUM(QTY) AS [PQty],");
            Text.Append("SUM(Amount) AS [PAmount],");
            Text.Append("SUM(TaxAmount) AS [PTaxAmount],");
            Text.Append("SUM(TaxAmount) AS [PNetAmount],");
            Text.Append("0.00 AS [SQty],");
            Text.Append("0.00 AS [SAmount],");
            Text.Append("0.00 AS [STaxAmount],");
            Text.Append("0.00 AS [SNetAmount],");
            Text.Append("0.00 AS [SRQty],");
            Text.Append("0.00 AS [SRAmount],");
            Text.Append("0.00 AS [SRTaxAmount],");
            Text.Append("0.00 AS [SRNetAmount]");
            Text.Append($"FROM ({StockPositionData(Filter)}) WHERE [Tran] = 'PURCHASED'");
            Text.Append("GROUP BY [Inventory]");
            Text.Append(") AS [P] ");
            Text.Append("UNION ");
            Text.Append("SELECT * FROM(");
            Text.Append("SELECT ");
            Text.Append("[Inventory],");
            Text.Append("0.00 AS [PQty],");
            Text.Append("0.00 AS [PAmount],");
            Text.Append("0.00 AS [PTaxAmount],");
            Text.Append("0.00 AS [PNetAmount],");
            Text.Append("SUM(QTY) AS [SQty],");
            Text.Append("SUM(Amount) AS [SAmount],");
            Text.Append("SUM(TaxAmount) AS [STaxAmount],");
            Text.Append("SUM(TaxAmount) AS [SNetAmount],");
            Text.Append("0.00 AS [SRQty],");
            Text.Append("0.00 AS [SRAmount],");
            Text.Append("0.00 AS [SRTaxAmount],");
            Text.Append("0.00 AS [SRNetAmount]");
            Text.Append($"FROM ({StockPositionData(Filter)}) WHERE [Tran] = 'SOLD'");
            Text.Append("GROUP BY [Inventory]");
            Text.Append(") AS [S] ");
            Text.Append("UNION ");
            Text.Append("SELECT * FROM (");
            Text.Append("SELECT ");
            Text.Append("[Inventory],");
            Text.Append("0.00 AS [PQty],");
            Text.Append("0.00 AS [PAmount],");
            Text.Append("0.00 AS [PTaxAmount],");
            Text.Append("0.00 AS [PNetAmount],");
            Text.Append("0.00 AS [SQty],");
            Text.Append("0.00 AS [SAmount],");
            Text.Append("0.00 AS [STaxAmount],");
            Text.Append("0.00 AS [SNetAmount],");
            Text.Append("SUM(QTY) AS [SRRQty],");
            Text.Append("SUM(Amount) AS [SRAmount],");
            Text.Append("SUM(TaxAmount) AS [SRTaxAmount],");
            Text.Append("SUM(NetAmount) AS [SRNetAmount]");
            Text.Append($"FROM ({StockPositionData(Filter)}) WHERE [Tran] = 'SRETURN'");
            Text.Append("GROUP BY [Inventory]");
            Text.Append(") AS [SR]");
            return Text.ToString();
            //ok

        }

        #endregion

        #region Book (Cash * Bank)
        public static string Book()
        {
            return "SELECT * FROM [Book]";
        }

        private static string Book2()
        {
            return "SELECT * FROM [Book2]";
        }

        public static string View_Book(string _Filter)
        {
            var _Text = new StringBuilder();
            _Text.AppendLine("SELECT * FROM [View_Book]");
            if(!string.IsNullOrEmpty(_Filter))
            {
                _Text.AppendLine($" WHERE {_Filter}");
            }

            return _Text.ToString();
        }
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
            if (_SQLQuery.Equals(Query.View_Production)) { return new QueryClass { QueryText = View_Production(), TableName = Tables.view_Production.ToString() }; }
            if (_SQLQuery.Equals(Query.View_Sold)) { return new QueryClass { QueryText = view_Sold(), TableName = Tables.view_Sold.ToString() }; }
            if (_SQLQuery.Equals(Query.Chk_BillReceivable1)) { return new QueryClass { QueryText = Chk_BillReceivable1(), TableName = Tables.Chk_BillReceivable1.ToString() }; }
            if (_SQLQuery.Equals(Query.Chk_BillReceivable2)) { return new QueryClass { QueryText = Chk_BillReceivable2(), TableName = Tables.Chk_BillReceivable2.ToString() }; }
            if (_SQLQuery.Equals(Query.Book)) { return new QueryClass { QueryText = Book(), TableName = Tables.Book.ToString() }; }
            if (_SQLQuery.Equals(Query.Book2)) { return new QueryClass { QueryText = Book2(), TableName = Tables.Book2.ToString() }; }
            

            return new QueryClass();
        }

        public static QueryClass GetQuery1(Query _SQLQuery, string Filter)
        {
            if (_SQLQuery.Equals(Query.StockPosition)) { return new QueryClass { QueryText = StockPosition(Filter), TableName = Tables.StockPosition.ToString() }; }
            if (_SQLQuery.Equals(Query.StockPositionData)) { return new QueryClass { QueryText = StockPosition(Filter), TableName = Tables.StockPosition.ToString() }; }
            if (_SQLQuery.Equals(Query.StockPositionSUM)) { return new QueryClass { QueryText = StockPositionSUM(Filter), TableName = Tables.StockPositionSUM.ToString() }; }
            if (_SQLQuery.Equals(Query.View_Book)) { return new QueryClass { QueryText = View_Book(Filter), TableName = Tables.view_Book.ToString() }; }
            return new QueryClass();
        }



        public class QueryClass
        {
            public string QueryText { get; set; } = string.Empty;
            public string TableName { get; set; } = string.Empty;
        }

    }
}

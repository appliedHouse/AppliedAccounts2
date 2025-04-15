using System.Text;

namespace SQLQueries
{
    public static class Quries
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
            
            if(!string.IsNullOrEmpty(_Filter))
            {
                _Text.AppendLine("WHERE " + _Filter);
            }

            return _Text.ToString();
        }
        #endregion

    }
}

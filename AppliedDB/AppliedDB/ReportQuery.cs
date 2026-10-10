using System.Text;

namespace AppliedDB
{
    public class ReportQuery
    {
        public static string SaleInvoiceQuery(string _Filter)
        {
            var _Text = new StringBuilder();
            _Text.Append("SELECT * FROM ( ");
            _Text.Append("SELECT [B2].[TranID], ");
            _Text.Append("[B1].[Vou_No],");
            _Text.Append("[B1].[Vou_Date], ");
            _Text.Append("[C].[Code] AS [Code], ");
            _Text.Append("[C].[NTN] AS [NTN], ");
            _Text.Append("[C].[Title] AS [Company], ");
            _Text.Append("[E].[Title] AS [Employee], ");
            _Text.Append("[P].[Title] AS [Project], ");
            _Text.Append("[B1].[Ref_No], ");
            _Text.Append("[B1].[Inv_No], ");
            _Text.Append("[B1].[Inv_Date], ");
            _Text.Append("[B1].[Pay_Date], ");
            _Text.Append("[B1].[Description], ");
            _Text.Append("[B2].[Sr_No], ");
            _Text.Append("[B2].[Inventory]");
            _Text.Append("[I].[Code] AS [InventoryCode], ");
            _Text.Append("[I].[Title] AS [InventoryTitle], ");
            _Text.Append("[B2].[Batch], ");
            _Text.Append("[B2].[Qty] AS [Qty],");
            _Text.Append("[B2].[Rate], ");
            _Text.Append("[B2].[Qty] * [B2].[Rate] AS [Amount], ");
            _Text.Append("[T].[Rate] AS [Tax_Rate], ");
            _Text.Append("([B2].[Qty] * [B2].[Rate]) * [T].[Rate] AS [Tax_Amount], ");
            _Text.Append("([B2].[Qty] * [B2].[Rate]) + (([B2].[Qty] * [B2].[Rate]) * [T].[Rate]) AS [Net_Amount], ");
            _Text.Append("[B2].[Description] AS [Remarks], [C].[Address1], [C].[Address2], [C].[City] || ' ' || [C].[State] || ' ' || [C].[Country] AS[Address3], ");
            _Text.Append("[C].[Phone]");
            _Text.Append("FROM [BillReceivable] [B1]");
            _Text.Append("LEFT JOIN [BillReceivable2] [B2] ON [B2].[TranID] = [B1].[ID]");
            _Text.Append("LEFT JOIN [Customers] [C] ON [C].[ID] = [B1].[Company]");
            _Text.Append("LEFT JOIN [Employees] [E] ON [E].[ID] = [B1].[Employee]");
            _Text.Append("LEFT JOIN [Project] [P] ON [P].[ID] = [B2].[Project] LEFT JOIN [Inventory] [I] ON [I].[ID] = [B2].[Inventory] LEFT JOIN [Taxes] [T] ON[T].[ID] = [B2].[Tax] ");
            _Text.Append(") AS [SalesInvoice] ");
            _Text.Append($"WHERE {_Filter} ");

            return _Text.ToString();

        }

        public static string SalesBatchQuery(string _Filter)
        {
            var _Text = new StringBuilder();
            _Text.Append("SELECT * FROM ( ");
            _Text.Append("SELECT ");
            _Text.Append("[B1].[ID] AS [ID], ");
            _Text.Append("[B1].[ID] AS [ID1], ");
            _Text.Append("[B1].[Vou_No], ");
            _Text.Append("[B1].[Vou_Date], ");
            _Text.Append("[B1].[Company], ");
            _Text.Append("[C].[Code] As [CompanyCode], ");
            _Text.Append("[C].[Title] As [CompanyTitle], ");
            _Text.Append("[B1].[Employee], ");
            _Text.Append("[B1].[Ref_No], ");
            _Text.Append("[B1].[Inv_No], ");
            _Text.Append("[B1].[Inv_Date], ");
            _Text.Append("[B1].[Pay_Date], ");
            _Text.Append("[B1].[Description], ");
            _Text.Append("[B1].[Comments], ");
            _Text.Append("[B1].[Status], ");
            _Text.Append("[B2].[ID] AS [ID2], ");
            _Text.Append("[B2].[Sr_No], ");
            _Text.Append("[B2].[TranID], ");
            _Text.Append("[I].[Code] [InventoryCode], ");
            _Text.Append("[B2].[Inventory], ");
            _Text.Append("[B2].[Batch], ");
            _Text.Append("[B2].[Qty], ");
            _Text.Append("[B2].[Rate], ");
            _Text.Append("[B2].[Tax], ");
            _Text.Append("[B2].[Qty] * [B2].[Rate] As [Amount], ");
            _Text.Append("[B2].[Tax_Rate], ");
            _Text.Append("([B2].[Qty] * [B2].[Rate]) * ([B2].[Tax_Rate] / 100.0) As [Tax_Amount], ");
            _Text.Append("([B2].[Qty] * [B2].[Rate]) + ([B2].[Qty] * [B2].[Rate]) * ([B2].[Tax_Rate] / 100.0) As [Net_Amount], ");
            _Text.Append("[B2].[Description] AS [Description2], ");
            _Text.Append("[B2].[Project] ");
            _Text.Append("FROM [BillReceivable] [B1] ");
            _Text.Append("LEFT JOIN [BillReceivable2] [B2] ON [B1].[ID] = [B2].[TranID] ");
            _Text.Append("LEFT JOIN [Customers] [C] ON [C].[ID] = [B1].[Company] ");
            _Text.Append("LEFT JOIN [Inventory] [I] ON [I].[ID] = [B2].[Inventory] ");
            _Text.Append(") AS [SalesBatch] ");
            _Text.Append($"WHERE {_Filter} ");

            return _Text.ToString();
        }
    }
}
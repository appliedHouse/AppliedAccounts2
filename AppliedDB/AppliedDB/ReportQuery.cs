using System.Text;

namespace AppliedDB
{
    public class ReportQuery
    {
        public static string SaleInvoiceQuery(string _Filter)
        {
            var _Text = new StringBuilder();

            _Text.AppendLine("SELECT * FROM ( ");

            _Text.AppendLine("SELECT [B2].[TranID], ");
            _Text.AppendLine("[B1].[Vou_No],");
            _Text.AppendLine("[B1].[Vou_Date], ");
            _Text.AppendLine("[C].[Code] AS [Code], ");
            _Text.AppendLine("[C].[NTN] AS [NTN], ");
            _Text.AppendLine("[C].[Title] AS [Company], ");
            _Text.AppendLine("[E].[Title] AS [Employee], ");
            _Text.AppendLine("[P].[Title] AS [Project], ");
            _Text.AppendLine("[B1].[Ref_No], ");
            _Text.AppendLine("[B1].[Inv_No], ");
            _Text.AppendLine("[B1].[Inv_Date], ");
            _Text.AppendLine("[B1].[Pay_Date], ");
            _Text.AppendLine("[B1].[Description], ");
            _Text.AppendLine("[B2].[Sr_No], ");

            _Text.AppendLine("[SC].[Code] AS [SCatCode], ");
            _Text.AppendLine("[SC].[Title] AS [SCatTitle], ");

            _Text.AppendLine("[B2].[Inventory],");
            _Text.AppendLine("[I].[Code] AS [InventoryCode], ");
            _Text.AppendLine("[I].[Title] AS [InventoryTitle], ");
            _Text.AppendLine("[B2].[Batch], ");
            _Text.AppendLine("[B2].[Qty] AS [Qty],");
            _Text.AppendLine("[B2].[Rate], ");
            _Text.AppendLine("[B2].[Qty] * [B2].[Rate] AS [Amount], ");
            _Text.AppendLine("[T].[Rate] AS [Tax_Rate], ");
            _Text.AppendLine("([B2].[Qty] * [B2].[Rate]) * [T].[Rate] AS [Tax_Amount], ");
            _Text.AppendLine("([B2].[Qty] * [B2].[Rate]) + (([B2].[Qty] * [B2].[Rate]) * [T].[Rate]) AS [Net_Amount], ");
            _Text.AppendLine("[B2].[Description] AS [Remarks], [C].[Address1], [C].[Address2], [C].[City] || ' ' || [C].[State] || ' ' || [C].[Country] AS [Address3], ");
            _Text.AppendLine("[C].[Phone] ");

            _Text.AppendLine("FROM [BillReceivable] [B1]");
            _Text.AppendLine("LEFT JOIN [BillReceivable2] [B2] ON [B2].[TranID] = [B1].[ID]");
            _Text.AppendLine("LEFT JOIN [Customers] [C] ON [C].[ID] = [B1].[Company]");
            _Text.AppendLine("LEFT JOIN [Employees] [E] ON [E].[ID] = [B1].[Employee]");
            _Text.AppendLine("LEFT JOIN [Project] [P] ON [P].[ID] = [B2].[Project]");
            _Text.AppendLine("LEFT JOIN [Inventory] [I] ON [I].[ID] = [B2].[Inventory]");
            _Text.AppendLine("LEFT JOIN [Taxes] [T] ON [T].[ID] = [B2].[Tax]");
            _Text.AppendLine("LEFT JOIN [Inv_SubCategory] [SC] ON [SC].[ID] = [B2].[Inventory]");

            _Text.AppendLine(") AS [SalesInvoice] ");
            _Text.AppendLine($"WHERE {_Filter} ");

            return _Text.ToString();
        }

        public static string SalesBatchQuery(string _Filter)
        {
            var _Text = new StringBuilder();
            _Text.AppendLine("SELECT * FROM ( ");
            _Text.AppendLine("SELECT ");
            _Text.AppendLine("[B1].[ID] AS [ID], ");
            _Text.AppendLine("[B1].[ID] AS [ID1], ");
            _Text.AppendLine("[B1].[Vou_No], ");
            _Text.AppendLine("[B1].[Vou_Date], ");
            _Text.AppendLine("[B1].[Company], ");
            _Text.AppendLine("[C].[Code] As [CompanyCode], ");
            _Text.AppendLine("[C].[Title] As [CompanyTitle], ");
            _Text.AppendLine("[B1].[Employee], ");
            _Text.AppendLine("[B1].[Ref_No], ");
            _Text.AppendLine("[B1].[Inv_No], ");
            _Text.AppendLine("[B1].[Inv_Date], ");
            _Text.AppendLine("[B1].[Pay_Date], ");
            _Text.AppendLine("[B1].[Description], ");
            _Text.AppendLine("[B1].[Comments], ");
            _Text.AppendLine("[B1].[Status], ");
            _Text.AppendLine("[B2].[ID] AS [ID2], ");
            _Text.AppendLine("[B2].[Sr_No], ");
            _Text.AppendLine("[B2].[TranID], ");
            _Text.AppendLine("[I].[Code] [InventoryCode], ");
            _Text.AppendLine("[B2].[Inventory], ");
            _Text.AppendLine("[B2].[Batch], ");
            _Text.AppendLine("[B2].[Qty], ");
            _Text.AppendLine("[B2].[Rate], ");
            _Text.AppendLine("[B2].[Tax], ");
            _Text.AppendLine("[B2].[Qty] * [B2].[Rate] As [Amount], ");
            _Text.AppendLine("[B2].[Tax_Rate], ");
            _Text.AppendLine("([B2].[Qty] * [B2].[Rate]) * ([B2].[Tax_Rate] / 100.0) As [Tax_Amount], ");
            _Text.AppendLine("([B2].[Qty] * [B2].[Rate]) + ([B2].[Qty] * [B2].[Rate]) * ([B2].[Tax_Rate] / 100.0) As [Net_Amount], ");
            _Text.AppendLine("[B2].[Description] AS [Description2], ");
            _Text.AppendLine("[B2].[Project] ");
            _Text.AppendLine("FROM [BillReceivable] [B1] ");
            _Text.AppendLine("LEFT JOIN [BillReceivable2] [B2] ON [B1].[ID] = [B2].[TranID] ");
            _Text.AppendLine("LEFT JOIN [Customers] [C] ON [C].[ID] = [B1].[Company] ");
            _Text.AppendLine("LEFT JOIN [Inventory] [I] ON [I].[ID] = [B2].[Inventory] ");
            _Text.AppendLine(") AS [SalesBatch] ");
            _Text.AppendLine($"WHERE {_Filter} ");

            return _Text.ToString();
        }
    }
}
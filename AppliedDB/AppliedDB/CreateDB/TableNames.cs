namespace AppliedDB.CreateDB
{
    internal class TableNames
    {
        public static string[] GetTableNames()
        {
            string[] tableNames = new string[]
            {
                "BankBook",
                "BillPayable",
                "BillPayable2",
                "BillReceivable",
                "BillReceivable2",
                "BOMProfile",
                "BOMProfile2",
                "Book",
                "Book2",
                "CashBook",
                "ChequeStatus",
                "ChequeTranType",
                "City",
                "COA",
                "COA_CLASS",
                "COA_Map",
                "COA_NATURE",
                "COA_NOTES",
                "Country",
                "Customers",
                "Directories",
                "Employees",
                "FinishedGoods",
                "IdGenerator",
                "Inv_Category",
                "Inventory",
                "Inv_Packing",
                "Inv_Size",
                "Inv_SubCategory",
                "Inv_UOM",
                "Inv_Price",
                "Ledger",
                "OBALCompany",
                "OBALStock",
                "Production2",
                "Profile",
                "Project",
                "Receipt",
                "Receipt2",
                "Receipts",
                "Registry",
                "Role",
                "SaleReturn",
                "StockInHand",
                "Taxes",
                "WriteCheques",
                // Data Views
                "view_Book",
                "view_BillPayable",
                "view_BillReceivable",
                "view_Ledger",
                "view_Purchased",
                "view_sold",
                "view_Receipts"
            };
            return tableNames;
        }
    }
}

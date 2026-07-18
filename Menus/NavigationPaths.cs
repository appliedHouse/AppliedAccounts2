namespace Menus
{
    public static class NavigationPaths
    {
        // ==========================================
        // DYNAMIC PATHS - NOT IN MENU DATA
        // These have parameters or are dynamic
        // ==========================================

        // Sales - Customer (with ID parameter)
        public static string Customer(long id = 0, bool isDelete = false)
        {
            return isDelete ? $"/Sales/Customer/{id}/true" : $"/Sales/Customer/{id}";
        }

        public static string CustomerAdd() => "/Sales/Customer/0";
        public static string CustomerEdit(long id) => $"/Sales/Customer/{id}";
        public static string CustomerDelete(long id) => $"/Sales/Customer/{id}/true";

        // Sales - Invoice
        public static string Invoice(long id = 0) => $"/Sales/Invoice/{id}";
        public static string InvoiceAdd() => "/Sales/Invoice/0";
        public static string InvoiceEdit(long id) => $"/Sales/Invoice/{id}";
        public static string InvoiceDelete(long id) => $"/Sales/Invoice/{id}/true";

        // Purchase
        public static string PurchaseOrder(long id = 0) => $"/Purchase/Order/{id}";
        public static string PurchaseOrderAdd() => "/Purchase/Order/0";
        public static string PurchaseOrderEdit(long id) => $"/Purchase/Order/{id}";
        public static string PurchaseOrderDelete(long id) => $"/Purchase/Order/{id}/true";

        // Stock - Items
        public static string StockItem(long id = 0) => $"/Stock/Item/{id}";
        public static string StockItemAdd() => "/Stock/Item/0";
        public static string StockItemEdit(long id) => $"/Stock/Item/{id}";
        public static string StockItemDelete(long id) => $"/Stock/Item/{id}/true";

        // Reports
        public static string Report(string reportName, long id = 0) => $"/Reports/{reportName}/{id}";
        public static string ReportSales(long id = 0) => $"/Reports/Sales/{id}";
        public static string ReportPurchases(long id = 0) => $"/Reports/Purchases/{id}";
        public static string ReportInventory(long id = 0) => $"/Reports/Inventory/{id}";

        // User Management
        public static string User(long id = 0) => $"/Admin/User/{id}";
        public static string UserAdd() => "/Admin/User/0";
        public static string UserEdit(long id) => $"/Admin/User/{id}";
        public static string UserDelete(long id) => $"/Admin/User/{id}/true";

        // Stock Dictionery
        public static string StockCategory() => "/Stock/Directory/Inv_Category";
        public static string StockSubCategory() => "/Stock/Directory/Inv_SubCategory";
        public static string StockPacking() => "/Stock/Directory/nv_Packing";
        public static string StockSize() => "/Stock/Directory/Inv_Size";
        public static string StockUOM() => "/Stock/Directory/Inv_UOM";
    }
}
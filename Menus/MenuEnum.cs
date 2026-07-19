namespace Menus
{
    public enum MenuID
    {
        // Main Menu
        Home = 1,
        Account = 2,
        Sale = 3,
        Stock = 4,
        HR = 5,
        Admin = 6,
        Taxation = 7,
        Logout = 8,

        // Sub Menu Home
        Setting = 11,
        Config = 12,
        Import = 13,

        // Sub Menu Accounts
        AccountsDictionery = 21,
        Transaction = 22,
        AccountsReports = 23,
        Balances = 24,
        Post = 25,
        Unpost = 26,

        // Sub Menu Sale
        SaleDictionery = 31,
        SaleTransaction = 32,
        SaleReports = 33,

        // Sub Menu Stock
        StockDictionery = 41,
        Production = 42,
        StockReports = 43,

        // Sub Menu HR
        Employees = 51,
        Department = 52,
        Payroll = 53,

        // Sub Menu Admin
        Circulars = 61,
        Events = 62,
        Attendence = 63,

        // Sub Menu Taxation
        InputTax = 71,
        OutputTax = 72,
        FBRReturn = 73,
        SRBReturn = 74,

        // Accounts - Dictionery
        COA = 2101,
        COANature = 2102,
        COAClass = 2103,
        COANotes = 2104,
        Projects = 2105,

        // Accounts - Transaction
        CashBook = 2201,
        Bankbook = 2202,
        Payment = 2203,
        ChqReturn = 2204,
        Receivables = 2205,
        Payables = 2206,
        Receipts = 2207,
        JVoucher = 2208,
        Vouchers = 2209,

        // Accounts - Reports
        Ledger = 2301,
        TrialBalance = 2302,
        ExpenseSheet = 2303,
        Receivable = 2304,
        Payable = 2305,
        ProfitLoss = 2306,
        BalanceSheet = 2307,

        // Sale - Dictionery
        ClientList = 3101,

        // Stock - Dictionery
        Inventory = 4101,
        Category = 4102,
        SubCategory = 4103,
        Packing = 4104,
        Size = 4105,
        Measurement = 4106
    }
}
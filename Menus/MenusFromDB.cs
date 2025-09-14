
namespace Menus
{
    public class MenusFromDB
    {
    internal static List<MenuItem> Get()
        {
            var _Menus = new List<MenuItem>(); ;

            _Menus.Add(new MenuItem { ID = 1, Title = "Home", Active = true, Icon = "bi bi-house", Level = 1, ParentID = 0, NavigateTo = "/" });
            _Menus.Add(new MenuItem { ID = 2, Title = "Account", Active = true, Icon = "bi-calculator", Level = 1, ParentID = 0, NavigateTo = "/Account" });
            _Menus.Add(new MenuItem { ID = 3, Title = "Sale", Active = true, Icon = "bi bi-receipt-cutoff", Level = 1, ParentID = 0 , NavigateTo = "/Sale" });
            _Menus.Add(new MenuItem { ID = 4, Title = "Stock", Active = true, Icon = "bi bi-box-seam", Level = 1, ParentID = 0, NavigateTo = "/Stock" });
            _Menus.Add(new MenuItem { ID = 5, Title = "HR", Active = true, Icon = "bi bi-person-arms-up", Level = 1, ParentID = 0, NavigateTo = "/HR" });
            _Menus.Add(new MenuItem { ID = 6, Title = "Admin", Active = true, Icon = "bi bi-person-lines-fill", Level = 1, ParentID = 0, NavigateTo = "/Admin" });
            _Menus.Add(new MenuItem { ID = 7, Title = "Logout", Active = true, Icon = "bi bi-door-closed text-danger", Level = 1, ParentID = 0, NavigateTo = "/Logout" });
            
            _Menus.Add(new MenuItem { ID = 11, Title = "Setting", Active = true, Icon = "bi bi-gear", Level = 2, ParentID = 1, NavigateTo = "/Home/Setting" });
            _Menus.Add(new MenuItem { ID = 12, Title = "Config", Active = true, Icon = "bi bi-sliders", Level = 2, ParentID = 1, NavigateTo = "/Home/Config" });
            _Menus.Add(new MenuItem { ID = 13, Title = "Import", Active = true, Icon = "bi bi-database-down", Level = 2, ParentID = 1, NavigateTo = "/Home/Import" });
            
            _Menus.Add(new MenuItem { ID = 21, Title = "Dictionery", Active = true, Icon = "bi bi-journal-code", Level = 2, ParentID = 2, NavigateTo = "/Accounts/Dictionery" });
            _Menus.Add(new MenuItem { ID = 22, Title = "Transaction", Active = true, Icon = "bi bi-stack-overflow", Level = 2, ParentID = 2, NavigateTo = "/Menu/Accounts" });
            _Menus.Add(new MenuItem { ID = 23, Title = "Reports", Active = true, Icon = "bi bi-printer", Level = 2, ParentID = 2, NavigateTo = "/Menu/Reports" });
            _Menus.Add(new MenuItem { ID = 24, Title = "Balances", Active = true, Icon = "bi bi-wallet2", Level = 2, ParentID = 2, NavigateTo = "" });
            _Menus.Add(new MenuItem { ID = 25, Title = "Post", Active = true, Icon = "bi bi-send", Level = 2, ParentID = 2, NavigateTo = "" });
            _Menus.Add(new MenuItem { ID = 26, Title = "Unpost", Active = true, Icon = "bi bi-send-dash", Level = 2, ParentID = 2, NavigateTo = "" });

            _Menus.Add(new MenuItem { ID = 31, Title = "Dictionery", Active = true, Icon = "bi bi-journal-code", Level = 2, ParentID = 3, NavigateTo = "" });
            _Menus.Add(new MenuItem { ID = 32, Title = "Transaction", Active = true, Icon = "bi bi-stack-overflow", Level = 2, ParentID = 3, NavigateTo = "/Menu/SaleCDC" });
            _Menus.Add(new MenuItem { ID = 33, Title = "Reports", Active = true, Icon = "bi bi-printer", Level = 2, ParentID = 3, NavigateTo = "" });
            
            _Menus.Add(new MenuItem { ID = 41, Title = "Dictionery", Active = true, Icon = "bi bi-journal-code", Level = 2, ParentID = 4, NavigateTo = "" });
            _Menus.Add(new MenuItem { ID = 42, Title = "Production", Active = true, Icon = "bi bi-bricks", Level = 2, ParentID = 4, NavigateTo = "" });
            _Menus.Add(new MenuItem { ID = 43, Title = "Reports", Active = true, Icon = "bi bi-printer", Level = 2, ParentID = 4, NavigateTo = "" });
            
            _Menus.Add(new MenuItem { ID = 51, Title = "Employees", Active = true, Icon = "bi bi-person-fill", Level = 2, ParentID = 5, NavigateTo = "" });
            _Menus.Add(new MenuItem { ID = 52, Title = "Department", Active = true, Icon = "bi bi-building", Level = 2, ParentID = 5, NavigateTo = "" });
            _Menus.Add(new MenuItem { ID = 53, Title = "Posting", Active = true, Icon = "bi bi-person-badge", Level = 2, ParentID = 5, NavigateTo = "" });
            
            _Menus.Add(new MenuItem { ID = 61, Title = "Circulars", Active = true, Icon = "bi bi-megaphone", Level = 2, ParentID = 6, NavigateTo = "" });
            _Menus.Add(new MenuItem { ID = 62, Title = "Events", Active = true, Icon = "bi bi-calendar3", Level = 2, ParentID = 6, NavigateTo = "" });
            _Menus.Add(new MenuItem { ID = 63, Title = "Attendence", Active = true, Icon = "bi bi-clock", Level = 2, ParentID = 6, NavigateTo = "" });


            // Accounts - Dictionery
            
            _Menus.Add(new MenuItem { ID = 2101, Title = "Accounts", Active = true, Icon = "bi bi-book", Level = 3, ParentID = 21, NavigateTo = "/Accounts/COA" });
            _Menus.Add(new MenuItem { ID = 2102, Title = "Nature", Active = true, Icon = "bi bi-tags", Level = 3, ParentID = 21, NavigateTo = "/Accounts/COANature" });
            _Menus.Add(new MenuItem { ID = 2103, Title = "Class", Active = true, Icon = "bi bi-layers", Level = 3, ParentID = 21, NavigateTo = "/Accounts/COAClass" });
            _Menus.Add(new MenuItem { ID = 2104, Title = "Notes", Active = true, Icon = "bi bi-journal-text", Level = 3, ParentID = 21, NavigateTo = "/Accounts/COANotes" });

            // Accounts - Trancaction
            _Menus.Add(new MenuItem { ID = 2201, Title = "Cash Book", Active = true, Icon = "bi bi-cash-coin", Level = 3, ParentID = 22, NavigateTo = "/Accounts/BooksList/1" });
            _Menus.Add(new MenuItem { ID = 2202, Title = "Bank book", Active = true, Icon = "bi bi-bank", Level = 3, ParentID = 22, NavigateTo = "/Accounts/BooksList/2" });
            _Menus.Add(new MenuItem { ID = 2203, Title = "Payment", Active = true, Icon = "bi bi-credit-card-2-front", Level = 3, ParentID = 22, NavigateTo = "" });
            _Menus.Add(new MenuItem { ID = 2204, Title = "Chq. Return", Active = true, Icon = "bi bi-arrow-90deg-down", Level = 3, ParentID = 22, NavigateTo = "" });
            _Menus.Add(new MenuItem { ID = 2205, Title = "Receivables", Active = true, Icon = "bi bi-file-text", Level = 3, ParentID = 22, NavigateTo = "" });
            _Menus.Add(new MenuItem { ID = 2206, Title = "Payables", Active = true, Icon = "bi bi-file-earmark-post", Level = 3, ParentID = 22, NavigateTo = "" });
            _Menus.Add(new MenuItem { ID = 2207, Title = "Receipts", Active = true, Icon = "bi bi-receipt", Level = 3, ParentID = 22, NavigateTo = "" });
            _Menus.Add(new MenuItem { ID = 2208, Title = "J.Voucher", Active = true, Icon = "bi bi-journal", Level = 3, ParentID = 22, NavigateTo = "" });
            _Menus.Add(new MenuItem { ID = 2209, Title = "Vouchers", Active = true, Icon = "bi bi-journals", Level = 3, ParentID = 22, NavigateTo = "" });

            // Accounts - Reports
            _Menus.Add(new MenuItem { ID = 2301, Title = "Ledger", Active = true, Icon = "bi bi-journals", Level = 3, ParentID = 23, NavigateTo = "/Accounts/Reports/Ledgers" });
            _Menus.Add(new MenuItem { ID = 2302, Title = "Trial Balance", Active = true, Icon = "bi bi-table", Level = 3, ParentID = 23, NavigateTo = "/Accounts/Reports/TrialBalance" });
            _Menus.Add(new MenuItem { ID = 2303, Title = "Expense Sheet", Active = true, Icon = "bi bi-file-spreadsheet", Level = 3, ParentID = 23, NavigateTo = "/Accounts/Reports/ExpenseSheet" });
            _Menus.Add(new MenuItem { ID = 2304, Title = "Receivable", Active = true, Icon = "bi bi-file-text", Level = 3, ParentID = 23, NavigateTo = "/Accounts/Reports/ReceivablePayable/1" });
            _Menus.Add(new MenuItem { ID = 2305, Title = "Payable", Active = true, Icon = "bi bi-file-earmark-post", Level = 3, ParentID = 23, NavigateTo = "/Accounts/Reports/ReceivablePayable/2" });
            _Menus.Add(new MenuItem { ID = 2305, Title = "Profit & Loss", Active = true, Icon = "bi bi-coin", Level = 3, ParentID = 23, NavigateTo = "/Accounts/Reports/ProfitnLoss" });
            _Menus.Add(new MenuItem { ID = 2305, Title = "Balanse Sheet", Active = true, Icon = "bi bi-grid-1x2", Level = 3, ParentID = 23, NavigateTo = "/Accounts/Reports/BalanceSheet" });



            return _Menus;
        }
    }

    public enum AppMenu
    {
        Home = 1,
        Logout = 7
    }
}

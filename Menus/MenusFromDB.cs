
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
            
            _Menus.Add(new MenuItem { ID = 21, Title = "Dictionery", Active = true, Icon = "bi bi-journal-code", Level = 2, ParentID = 2, NavigateTo = "" });
            _Menus.Add(new MenuItem { ID = 22, Title = "Transaction", Active = true, Icon = "bi bi-stack-overflow", Level = 2, ParentID = 2, NavigateTo = "/Menu/Accounts" });
            _Menus.Add(new MenuItem { ID = 23, Title = "Reports", Active = true, Icon = "bi bi-printer", Level = 2, ParentID = 2, NavigateTo = "/Menu/Reports" });
            _Menus.Add(new MenuItem { ID = 24, Title = "Balances", Active = true, Icon = "bi bi-wallet2", Level = 2, ParentID = 2, NavigateTo = "" });
            _Menus.Add(new MenuItem { ID = 25, Title = "Post", Active = true, Icon = "bi bi-send", Level = 2, ParentID = 2, NavigateTo = "" });
            _Menus.Add(new MenuItem { ID = 26, Title = "Unpost", Active = true, Icon = "bi bi-send-dash", Level = 2, ParentID = 2, NavigateTo = "" });

            _Menus.Add(new MenuItem { ID = 31, Title = "Dictionery", Active = true, Icon = "bi bi-journal-code", Level = 2, ParentID = 3, NavigateTo = "" });
            _Menus.Add(new MenuItem { ID = 32, Title = "Transaction", Active = true, Icon = "bi bi-stack-overflow", Level = 2, ParentID = 3, NavigateTo = "" });
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


            // Accounts - Trancaction
            _Menus.Add(new MenuItem { ID = 2201, Title = "Cash Book", Active = true, Icon = "bi bi-cash-coin", Level = 3, ParentID = 22, NavigateTo = "/Accounts/BooksList" });
            _Menus.Add(new MenuItem { ID = 2202, Title = "Bank book", Active = true, Icon = "bi bi-bank", Level = 3, ParentID = 22, NavigateTo = "/Accounts/BooksList" });
            _Menus.Add(new MenuItem { ID = 2203, Title = "Payment", Active = true, Icon = "bi bi-credit-card-2-front", Level = 3, ParentID = 22, NavigateTo = "" });
            _Menus.Add(new MenuItem { ID = 2204, Title = "Chq. Return", Active = true, Icon = "bi bi-arrow-90deg-down", Level = 3, ParentID = 22, NavigateTo = "" });
            _Menus.Add(new MenuItem { ID = 2205, Title = "Receivables", Active = true, Icon = "bi bi-file-text", Level = 3, ParentID = 22, NavigateTo = "" });
            _Menus.Add(new MenuItem { ID = 2206, Title = "Payables", Active = true, Icon = "bi bi-file-earmark-post", Level = 3, ParentID = 22, NavigateTo = "" });
            _Menus.Add(new MenuItem { ID = 2207, Title = "Receipts", Active = true, Icon = "bi bi-receipt", Level = 3, ParentID = 22, NavigateTo = "" });
            _Menus.Add(new MenuItem { ID = 2208, Title = "J.Voucher", Active = true, Icon = "bi bi-journal", Level = 3, ParentID = 22, NavigateTo = "" });
            _Menus.Add(new MenuItem { ID = 2209, Title = "Vouchers", Active = true, Icon = "bi bi-journals", Level = 3, ParentID = 22, NavigateTo = "" });



            return _Menus;
        }
    }
}

using AppliedAccounts.Services;
using AppliedDB;
using AppMessages;
using Microsoft.AspNetCore.Components;
using static AppliedDB.Enums;

namespace AppliedAccounts.Models.Interface
{

    public interface IVoucherList
    {
        AppliedGlobals.AppUserModel? AppUser { get; set; }
        DataSource Source { get; set; }
        string DBFile { get; set; }
        object Record { get; set; } //  SalesRecord Record or PurchaseRecord  etc.
        List<object> Records { get; set; } //List<Record> Records { get; set; }

        string SearchText { get; set; }
        MessageClass MsgClass { get; set; }

        Tables Table { get; set; }                              // Name of Table for fatch Data
        decimal TotalAmount { get; set; } //= 0.00M;
        bool SelectAll { get; set; } //= false;

        PrintService Printer { get; set; }
        DateTime DT_Start { get; set; }
        DateTime DT_End { get; set; }
        bool PageIsValid { get; set; }
        NavigationManager NavManager { get; set; }

        string GetFilterText(); //{ return string.Empty; }
        void Print(int _ID);
        void Edit(int _ID);

        List<object> LoadData(); // { return []; }
    }
}

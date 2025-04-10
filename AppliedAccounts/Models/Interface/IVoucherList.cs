using AppliedAccounts.Services;
using AppliedDB;
using AppMessages;
using System.Data;
using static AppliedDB.Enums;

namespace AppliedAccounts.Models.Interface
{

    public interface IVoucherList
    {
        AppUserModel? UserProfile { get; set; }
        DataSource Source { get; set; }
        List<DataRow> DataList { get; set; }
        Tables Table { get; set; }
        string SearchText { get; set; }
        MessageClass MsgClass { get; set; }
        PrintService Printer { get; set; }
        DateTime DT_Start { get; set; }
        DateTime DT_End { get; set; }
        bool PageIsValid { get; set; }

        string GetFilterText(); //{ return string.Empty; }

        void Print(int _ID);
        
        List<DataRow> LoadData(); // { return []; }
    }
}

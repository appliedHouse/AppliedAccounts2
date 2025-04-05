using AppliedDB;
using AppMessages;
using System.Data;
using static AppliedDB.Enums;

namespace AppliedAccounts.Models.Interface
{
    public class IVoucherList
    {
        AppUserModel? UserProfile { get; set; }
        DataSource Source { get; set; }
        List<DataRow> DataList { get; set; }
        Tables Table { get; set; }
        string SearchText { get; set; }
        MessageClass MsgClass { get; set; }
        DateTime DT_Start { get; set; }
        DateTime DT_End { get; set; }
        bool PageIsValid { get; set; } = false;

        string GetFilterText() { return string.Empty; }

        static List<DataRow> LoadData() { return []; }

    }
}

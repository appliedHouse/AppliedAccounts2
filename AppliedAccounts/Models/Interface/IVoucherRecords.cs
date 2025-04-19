using AppliedAccounts.Data;
using AppliedDB;
using AppMessages;
using Microsoft.AspNetCore.Components;

namespace AppliedAccounts.Models.Interface
{
    public interface IVoucherRecords<T> where T : class
    {
        AppUserModel AppUser { get; set; }
        DataSource Source { get; set; }
        NavigationManager NavManager { get; set; }
        string DBFile { get; set; }
        ListFilter FilterClass { get; set; }
        List<T> Records { get; set; }
        MessageClass MsgClass { get; set; }
        AppliedDB.Enums.Tables Table { get; set; }
        List<T> LoadData();
        void Print(int ID);
        void Edit(int ID);
        
        
    }
}

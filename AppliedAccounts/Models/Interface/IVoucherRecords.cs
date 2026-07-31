using AppliedAccounts.Data;
using AppliedAccounts.Services;
using AppliedDB;
using Microsoft.AspNetCore.Components;

namespace AppliedAccounts.Models.Interface
{
    public interface IVoucherRecords<T> where T : class
    {
        DataSource Source { get; set; }
        NavigationManager NavManager { get; set; }
        ListFilter FilterClass { get; set; }
        List<T> Records { get; set; }
        MessagesService MsgService { get; set; }
        Enums.Tables Table { get; set; }
        List<T> LoadData();
        void Print(long ID);
        void Edit(long ID);


    }
}

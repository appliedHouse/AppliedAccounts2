using AppliedAccounts.Data;
using AppliedAccounts.Models.Interface;
using AppliedAccounts.Services;
using AppliedDB;
using AppMessages;
using Microsoft.AspNetCore.Components;
using System.Data;

namespace AppliedAccounts.Models
{
    public class PurchaseListModel : IVoucherRecords<PurchaseRecord>
    {
        public AppUserModel AppUser { get; set; }
        public DataSource Source { get; set; }
        public NavigationManager NavManager { get; set; }
        public string DBFile { get; set; }
        public ListFilter Filter { get; set; }
        public List<PurchaseRecord> Records { get; set; }
        
        public MessageClass MsgClass { get; set; }
        public AppliedDB.Enums.Tables Table { get; set; }

        public PurchaseListModel(AppUserModel _AppUser)
        {
            AppUser = _AppUser;
        }

        public string GetFilterText()
        {
            throw new NotImplementedException();
        }

        public void Print(int _ID)
        {
            throw new NotImplementedException();
        }

        public void Edit(int _ID)
        {
            NavManager!.NavigateTo($"/Purchase/Purchased/{_ID}");
        }
        public List<PurchaseRecord> LoadData()
        {
            throw new NotImplementedException();
        }
        public Paging Pages { get; set; } = new();
    }

        public class PurchaseRecord
        {
            public int ID { get; set; }
            public string Vou_No { get; set; }
            public DateTime Vou_Date { get; set; }
            public string Batch { get; set; }
            public int SupplierID { get; set; }
            public string SupplierTitle { get; set; }
            public string Gross { get; set; }
            public string TaxRate { get; set; }
            public string TaxAmount { get; set; }
            public string NetAmount { get; set; }
            public bool IsSelected { get; set; } = false;
        }
    
    
}

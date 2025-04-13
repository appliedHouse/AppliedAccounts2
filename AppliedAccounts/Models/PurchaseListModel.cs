using AppliedAccounts.Data;
using AppliedAccounts.Models.Interface;
using AppliedAccounts.Services;
using AppliedDB;
using AppMessages;
using Microsoft.AspNetCore.Components;
using System.Data;
using System.Text;

namespace AppliedAccounts.Models
{
    public class PurchaseListModel : IVoucherRecords<PurchaseRecord>
    {
        public AppUserModel AppUser { get; set; }
        public DataSource Source { get; set; }
        public NavigationManager NavManager { get; set; }
        public string DBFile { get; set; }
        public ListFilter FilterClass { get; set; }
        public List<PurchaseRecord> Records { get; set; }
        
        public MessageClass MsgClass { get; set; }
        public AppliedDB.Enums.Tables Table { get; set; }

        public PurchaseListModel(AppUserModel _AppUser)
        {
            AppUser = _AppUser;
            DBFile = AppUser.DataFile;
            Source = new DataSource(DBFile);
            MsgClass = new();
            FilterClass = new(DBFile);
            Table = AppliedDB.Enums.Tables.view_BillPayable;
            LoadData();
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
            
            using var _Table = Source.GetTable(SQLQueries.Quries.ViewPurchaseInvoice(FilterClass.GetFilterText()));


            foreach(DataRow item in _Table.Rows)
            {
                var _Record = new PurchaseRecord
                {
                    ID = item.Field<int>("ID"),
                    Vou_No = item.Field<string>("Vou_No") ?? "",
                    Vou_Date = item.Field<DateTime>("Vou_Date"),
                    Batch = item.Field<string>("Batch") ?? "",
                    SupplierID = item.Field<int>("SupplierID"),
                    SupplierTitle = item.Field<string>("SupplierTitle") ?? "",
                //    Gross = item.Field<decimal>("Gross"),
                //    TaxRate = item.Field<decimal>("TaxRate"),
                //    TaxAmount = item.Field<decimal>("TaxAmount"),
                //    NetAmount = item.Field<decimal>("NetAmount")
                };
                Records.Add(_Record);
            }

            return Records;

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

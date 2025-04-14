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
            Source = new DataSource(AppUser);
            MsgClass = new();
            FilterClass = new(DBFile);
            Table = AppliedDB.Enums.Tables.view_BillPayable;
            Records = LoadData();
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
            var _result = new List<PurchaseRecord>();


            using var _Table = Source.GetTable(SQLQueries.Quries.ViewPurchaseInvoice(FilterClass.GetFilterText()));


            foreach (DataRow item in _Table.Rows)
            {
                var _Record = new PurchaseRecord
                {
                    ID1 = item.Field<int>("ID1"),
                    Vou_No = item.Field<string>("Vou_No") ?? "",
                    Vou_Date = item.Field<DateTime>("Vou_Date"),
                    SupplierID = item.Field<int>("Company"),
                    Inv_No = item.Field<string>("Inv_No") ?? "",
                    Inv_Date = item.Field<DateTime>("Inv_Date"),
                    Pay_Date = item.Field<DateTime>("Pay_Date"),
                    Amount = item.Field<decimal>("Amount"),
                    Remarks = item.Field<string>("Remarks") ?? "",
                    Comments = item.Field<string>("comments") ?? "",
                    Status = item.Field<string>("Status") ?? "",
                    ID2 = item.Field<int>("ID2"),
                    Sr_No = item.Field<int>("Sr_No"),
                    TranID = item.Field<int>("TranID"),
                    Inventory = item.Field<int>("Inventory"),
                    Batch = item.Field<string>("Batch") ?? "",
                    Qty = item.Field<decimal>("Qty"),
                    Rate = item.Field<decimal>("Rate"),
                    
                    TaxID = item.Field<int>("Tax"),
                    TaxRate = item.Field<decimal>("Tax_Rate"),
                    Description = item.Field<string>("Description") ?? "",
                    Project = item.Field<int>("Project"),

                };
                _result.Add(_Record);
            }

            return _result;

        }
        public Paging Pages { get; set; } = new();
    }

    public class PurchaseRecord
    {
        public int ID1 { get; set; }
        public string Vou_No { get; set; }
        public DateTime Vou_Date { get; set; }
        public string Inv_No { get; set; }
        public DateTime Inv_Date { get; set; }
        public DateTime Pay_Date { get; set; }
        public string Ref_No { get; set; }
        public int SupplierID { get; set; }
        public string SupplierTitle { get; set; }
        public decimal Amount { get; set; }
        public string Remarks { get; set; }
        public string Comments { get; set; }
        public string Status { get; set; }
        public int ID2 { get; set; }
        public int Sr_No { get; set; }
        public int TranID { get; set; }
        public int Inventory { get; set; }
        public string Batch { get; set; }
        public decimal Qty { get; set; }
        public decimal Rate { get; set; }
        public decimal Gross => Qty * Rate;
        
        public int TaxID { get; set; }
        public decimal TaxRate { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal NetAmount => Gross + TaxAmount;
        public string Description { get; set; }
        public int Project { get; set; }

        /// <summary>
        /// Other DB
        /// </summary>
        public bool IsSelected { get; set; } = false;           // Selected for Print All  
    }


}

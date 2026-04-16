using AppliedAccounts.Data;
using AppliedAccounts.Models.Interface;
using AppliedAccounts.Services;
using AppliedDB;
using AppMessages;
using Microsoft.AspNetCore.Components;
using System.Data;
using static AppliedDB.Enums;

namespace AppliedAccounts.Models
{
    public class PurchaseListModel : IVoucherRecords<PurchaseRecord>
    {
        public GlobalService AppGlobal { get; set; }
        public DataSource Source { get; set; }
        public NavigationManager NavManager { get; set; }
        public ListFilter FilterClass { get; set; }
        public List<PurchaseRecord> Records { get; set; }
        public PurchaseRecord Record { get; set; }
        public MessageClass MsgClass { get; set; }
        public AppliedDB.Enums.Tables Table { get; set; }
        public bool SelectAll { get; set; }

        public PurchaseListModel(GlobalService _AppGlobal)
        {
            AppGlobal = _AppGlobal;
            Source = new(AppGlobal.AppPaths);
            MsgClass = new();
            FilterClass = new(AppGlobal.DBFile);
            Table = Tables.view_BillPayable;
            Records = LoadData();
        }

        public void Print(long _ID)
        {
            throw new NotImplementedException();
        }

        public void Edit(long _ID)
        {
            NavManager!.NavigateTo($"/Purchase/Purchased/{_ID}");
        }
        public List<PurchaseRecord> LoadData()
        {
            var _result = new List<PurchaseRecord>();


            using var _Table = Source.GetTable(SQLQueries.Quries.ViewPurchaseInvoice(FilterClass.GetFilterText()));


            foreach (DataRow item in _Table.Rows)
            {
                var qty = item.GetDecimal("Qty");
                var rate = item.GetDecimal("Rate");
                var taxRate = item.GetDecimal("Tax_Rate");

                var _Record = new PurchaseRecord
                {
                    ID1 = item.GetInt64("ID1"),
                    Vou_No = item.GetString("Vou_No"),
                    Vou_Date = item.GetDate("Vou_Date"),

                    SupplierID = item.GetInt64("Company"),
                    SupplierTitle = item.GetString("CompanyTitle"),

                    Inv_No = item.GetString("Inv_No"),
                    Inv_Date = item.GetDate("Inv_Date"),
                    Pay_Date = item.GetDate("Pay_Date"),

                    Amount = item.GetDecimal("Amount"),

                    Remarks = item.GetString("Remarks"),
                    Comments = item.GetString("Comments"),
                    Status = item.GetString("Status"),

                    ID2 = item.GetInt64("ID2"),
                    Sr_No = item.GetInt32("Sr_No"),
                    TranID = item.GetInt64("TranID"),
                    Inventory = item.GetInt64("Inventory"),

                    Batch = item.GetString("Batch"),
                    Qty = qty,
                    Rate = rate,

                    TaxID = item.GetInt64("Tax"),
                    TaxRate = taxRate,
                    TaxAmount = (qty * rate * taxRate) / 100,

                    Description = item.GetString("Description"),
                    Project = item.GetInt64("Project"),

                    Ref_No = item.GetString("Ref_No")
                };
                _result.Add(_Record);
            }

            return _result;

        }
        public Paging Pages { get; set; } = new();
    }

    public class PurchaseRecord
    {
        public long ID1 { get; set; }
        public string Vou_No { get; set; }
        public DateTime Vou_Date { get; set; }
        public string Inv_No { get; set; }
        public DateTime Inv_Date { get; set; }
        public DateTime Pay_Date { get; set; }
        public string Ref_No { get; set; }
        public long SupplierID { get; set; }
        public string SupplierTitle { get; set; }
        public decimal Amount { get; set; }
        public string Remarks { get; set; }
        public string Comments { get; set; }
        public string Status { get; set; }
        public long ID2 { get; set; }
        public int Sr_No { get; set; }
        public long TranID { get; set; }
        public long Inventory { get; set; }
        public string Batch { get; set; }
        public decimal Qty { get; set; }
        public decimal Rate { get; set; }
        public decimal Gross => Qty * Rate;

        public long TaxID { get; set; }
        public decimal TaxRate { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal NetAmount => Gross + TaxAmount;
        public string Description { get; set; }
        public long Project { get; set; }

        /// <summary>
        /// Other DB
        /// </summary>
        public bool IsSelected { get; set; } = false;           // Selected for Print All  
    }


}

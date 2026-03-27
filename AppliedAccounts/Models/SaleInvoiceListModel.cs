using AppliedAccounts.Services;
using AppliedDB;
using AppMessages;
using AppReports;
using Microsoft.AspNetCore.Components;
using System.Data;
using static AppliedDB.Enums;

namespace AppliedAccounts.Models
{
    public class SaleInvoiceListModel
    {
        [Inject] public GlobalService AppGlobal { get; set; } = default!;
        public DataSource Source { get; set; }
        public string DBFile { get; set; } = string.Empty;
        public SalesRecord Record { get; set; } = new();
        public List<SalesRecord> Records { get; set; } = new();
        public DataTable Data { get; set; } = new();
        public PageModel Pages { get; set; } = new();

        public MessageClass MsgClass { get; set; } = new();
        public decimal TotalAmount { get; set; } = 0.00M;
        public bool SelectAll { get; set; }
        public long VoucherID { get; set; }
        public string SearchText { get; set; } = string.Empty;
        public string Filter { get; set; } = string.Empty;


        #region Constructor
        public SaleInvoiceListModel(GlobalService _AppGlobal) 
        {
            AppGlobal = _AppGlobal;
            Source = new(AppGlobal.AppPaths);
            if(AppGlobal.Client.UserID == "CDC")
            {
                Pages.Size = 500;                   // if user id CDC shows max 500 records in one page
            }
            //LoadData();
        }
        
        #endregion

        #region Load Data

       

        public void LoadData()
        {
            //Source ??= new(AppGlobal.AppPaths);
            var _Query = SQLQuery.SaleInvoiceList();
            var _Sort = "Vou_Date, Vou_No";

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                string[] columns =
                {
                    "Company",
                    "Salesman",
                    "City",
                    "Description",
                    "Vou_No",
                    "Vou_Date",
                    "Inv_Date",
                    "Pay_Date"
                };

                Filter = string.Join(" OR ", columns.Select(c => $"{c} like '%{SearchText}%'"));
            }
            else
            {
                Filter = string.Empty;
            }
            
            //Pages.TotalRecords = Source.RecordCound(Tables.BillReceivable, Filter) + 1;
            Data = Source.GetTable(_Query, Filter, _Sort + Pages.GetLimit());
            Records = [.. Data.AsEnumerable().Select(row => GetRecord(row))]; 
            Pages.Refresh(Source.RecordCount(_Query, Filter));

        }
        #endregion

        #region Get Records by Row & ID
        private SalesRecord GetRecord(DataRow _Row)
        {
            _Row = AppliedDB.Functions.RemoveNull(_Row);
            SalesRecord _Record = new();
            {
                _Record.Id = (long)_Row["ID"];
                _Record.Vou_No = (string)_Row["Vou_No"];
                _Record.Ref_No = (string)_Row["Ref_No"];
                //_Record.Batch = (string)_Row["Batch"];
                _Record.Vou_Date = (DateTime)_Row["Vou_Date"];
                _Record.Inv_Date = (DateTime)_Row["Inv_Date"];
                _Record.Pay_Date = (DateTime)_Row["Pay_Date"];
                _Record.TitleCustomer = (string)_Row["Company"];
                _Record.TitleSalesman = (string)_Row["Salesman"];
                _Record.City = (string)_Row["City"];
                _Record.Amount = (decimal)_Row["Amount"];
                _Record.Description = (string)_Row["Description"];

            }
            return _Record;
        }

        public SalesRecord GetRecord(long _ID)
        {

            foreach (SalesRecord _Record in Records)
            {
                if (_Record.Id == _ID)
                {
                    Record = _Record;
                    return _Record;
                }
            }
            return new();
        }
        #endregion

        #region Add
        public void Add()
        {
            Record = new SalesRecord();
        }
        #endregion

        #region Edit
        public void Edit(long _ID)
        {
            GetRecord(_ID);
        }
        #endregion

        #region Search
        public async void Search()
        {
            
            LoadData();
            Pages.Current = 1;
            Pages.Refresh();
        }

        public async void ClearText()
        {
            SearchText = string.Empty;
            LoadData();
            Pages.Refresh();
            
        }
        #endregion

    }

    public class SalesRecord
    {
        public long Id { get; set; } = 0;
        public string Vou_No { get; set; } = string.Empty;
        public string Ref_No { get; set; } = string.Empty;
        public DateTime Vou_Date { get; set; } = DateTime.Now;
        public DateTime Inv_Date { get; set; } = DateTime.Now;
        public DateTime Pay_Date { get; set; } = DateTime.Now;
        public string TitleCustomer { get; set; } = string.Empty;
        public string TitleSalesman { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; } = 0.00M;
        public ReportModel Report { get; set; } = new();
        public bool IsSelected { get; set; } = false;       // Record is selected for bulk print.

    }



}

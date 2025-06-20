﻿using AppliedAccounts.Data;
using AppliedAccounts.Services;
using AppliedDB;
using AppMessages;
using AppReports;
using System.Data;

namespace AppliedAccounts.Models
{
    public class SaleInvoiceListModel
    {
        public GlobalService AppGlobal { get; set; }
        public DataSource Source { get; set; }
        public string DBFile { get; set; } = string.Empty;
        public SalesRecord Record { get; set; } = new();
        public List<SalesRecord> Records { get; set; } = new();
        public List<DataRow> Data { get; set; } = new();

        public MessageClass MsgClass { get; set; } = new();
        public decimal TotalAmount { get; set; } = 0.00M;
        public bool SelectAll { get; set; }
        public int VoucherID { get; set; }
        public string SearchText { get; set; } = string.Empty;
        #region Constructor
        public SaleInvoiceListModel() { }
        public SaleInvoiceListModel(GlobalService _AppGlobal)
        {
            AppGlobal = _AppGlobal;
            Source = new(AppGlobal.AppPaths);
            LoadData();
        }
        #endregion

        #region Load Data
        private void LoadData()
        {
            Data = Source.GetList(AppliedDB.Enums.Query.SaleInvoiceList);
            Records = GetFilterRecords();
        }
        #endregion


        #region Filter Records
        private List<SalesRecord> GetFilterRecords()
        {
            var _FilterRecords = new List<SalesRecord>();

            if (SearchText.Length > 0)
            {
                _FilterRecords = Data.AsEnumerable().Where(row =>
                (row.Field<string>("Vou_No") ?? string.Empty)!.Contains(SearchText) ||
                AppFunctions.Date2Text(row.Field<DateTime>("Vou_Date"))!.Contains(SearchText) ||
                AppFunctions.Date2Text(row.Field<DateTime>("Inv_Date"))!.Contains(SearchText) ||
                AppFunctions.Date2Text(row.Field<DateTime>("Pay_Date"))!.Contains(SearchText) ||
                (row.Field<string>("Company") ?? string.Empty).Contains(SearchText) ||
                (row.Field<string>("City") ?? string.Empty).Contains(SearchText) ||
                (row.Field<string>("Salesman") ?? string.Empty).Contains(SearchText) ||
                (row.Field<string>("Ref_No") ?? string.Empty).Contains(SearchText)
                ).Select(row => GetRecord(row)).ToList();

            }

            if (SearchText.Length == 0)
            {
                _FilterRecords = [.. Data.AsEnumerable().ToList().Select(GetRecord)];
                //_FilterRecords = Data.AsEnumerable().ToList().Select(row => GetRecord(row)).ToList();
            }

            TotalAmount = _FilterRecords.Sum(row => row.Amount);


            return _FilterRecords;
        }

        #endregion

        #region Get Records by Row & ID
        private SalesRecord GetRecord(DataRow _Row)
        {
            _Row = AppliedDB.Functions.RemoveNull(_Row);
            SalesRecord _Record = new();
            {
                _Record.Id = (int)_Row["ID"];
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

        public SalesRecord GetRecord(int _ID)
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
        public void Edit(int _ID)
        {
            GetRecord(_ID);
        }
        #endregion

        #region Search
        public void Search()
        {
            Records = GetFilterRecords();

        }

        public void ClearText()
        {
            SearchText = string.Empty;
            Records = GetFilterRecords();
        }
        #endregion

    }

    public class SalesRecord
    {
        public int Id { get; set; } = 0;
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

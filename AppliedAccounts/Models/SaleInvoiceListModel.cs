using AppliedAccounts.Data;
using AppliedDB;
using AppReports;
using System.Data;

namespace AppliedAccounts.Models
{
    public class SaleInvoiceListModel
    {
        public AppUserModel AppUser { get; set; }
        public DataSource? Source { get; set; }
        public string DBFile { get; set; } = string.Empty;
        public SalesRecord Record { get; set; } = new();
        public List<SalesRecord> Records { get; set; } = new();
        public List<DataRow> Data { get; set; } = new();
        public string SearchText { get; set; } = string.Empty;
        public AppMessages.MessageClass MsgClass { get; set; } = new();
        public decimal TotalAmount { get; set; } = 0.00M;
        public bool SelectAll { get; set; }
        

        #region Constructor
        public SaleInvoiceListModel() { }
        public SaleInvoiceListModel(AppUserModel UserProfile)
        {
            AppUser = UserProfile;
            DBFile = AppUser.DataFile;
            Source = new(AppUser);
            Data = Source.GetList(Enums.Query.SaleInvoiceList);
            Records = GetFilterRecords();
        }
        #endregion

        #region Filter Records
        private List<SalesRecord> GetFilterRecords()
        {
            var _FilterRecords = new List<SalesRecord>();
            TotalAmount = 0.00M;

            foreach (DataRow _Row in Data)
            {
                if (SearchText.Length == 0)
                {
                    TotalAmount = TotalAmount + (decimal)_Row["Amount"];
                    _FilterRecords.Add(GetRecord(_Row));
                }
                else
                {
                    var IsSearch = false;
                    if (_Row["Vou_No"].ToString().Contains(SearchText)) { IsSearch = true; }
                    if (AppFunctions.Date2Text(_Row["Vou_Date"]).Contains(SearchText)) { IsSearch = true; }
                    if (AppFunctions.Date2Text(_Row["Inv_Date"]).Contains(SearchText)) { IsSearch = true; }
                    if (AppFunctions.Date2Text(_Row["Pay_Date"]).Contains(SearchText)) { IsSearch = true; }
                    if (_Row["Company"].ToString().Contains(SearchText)) { IsSearch = true; }
                    if (_Row["City"].ToString().Contains(SearchText)) { IsSearch = true; }
                    if (_Row["Salesman"].ToString().Contains(SearchText)) { IsSearch = true; }
                    if (_Row["Ref_No"].ToString().Contains(SearchText)) { IsSearch = true; }

                    if (IsSearch)
                    {
                        _FilterRecords.Add(GetRecord(_Row));
                        TotalAmount = TotalAmount + (decimal)_Row["Amount"];
                    }
                }
            }
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

using AppliedAccounts.Data;
using AppliedDB;
using AppReports;
using System.Data;

namespace AppliedAccounts.Models
{
    public class PurchaseInvoiceListModel
    {
        public AppUserModel? AppUser { get; set; }
        public DataSource? Source { get; set; }
        public string DBFile { get; set; } = string.Empty;
        public PurchaseRecord Record { get; set; } = new();
        public List<PurchaseRecord> Records { get; set; } = new();
        public List<PurchaseRecord> DisplayRecords { get; set; } = new();
        public List<DataRow> Data { get; set; } = new();
        public string SearchText { get; set; } = string.Empty;
        public AppMessages.AppMessages MyMessages { get; set; } = new();
        public decimal TotalAmount { get; set; } = 0.00M;
        public int Page { get; set; } = 1;
        public int Pages { get; set; } = 0;
        public int PerPage { get; set; } = 0;

        #region Constructor
        public PurchaseInvoiceListModel() { }
        public PurchaseInvoiceListModel(AppUserModel UserProfile) 
        {
            AppUser = UserProfile;
            DBFile = AppUser.DataFile;
            Source = new(AppUser);
            Data = Source.GetList(Enums.Query.PurchaseInvoiceList);
            Records = GetFilterRecords();

            Pages = (int)Math.Ceiling((float)Records.Count / PerPage );

            if (Records.Count > 0)
            {
                Page = 1;
                DisplayRecords = Records.Skip(0).Take(20).ToList();
            }


        }

        #endregion

        #region Filter Records
        private List<PurchaseRecord> GetFilterRecords()
        {
            var _FilterRecords = new List<PurchaseRecord>();
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
                    if (_Row["Company"].ToString().ToUpper().Contains(SearchText.ToUpper())) { IsSearch = true; }
                    if (_Row["City"].ToString().ToUpper().Contains(SearchText.ToUpper())) { IsSearch = true; }
                    if (_Row["Employee"].ToString().ToUpper().Contains(SearchText.ToUpper())) { IsSearch = true; }

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
        private PurchaseRecord GetRecord(DataRow _Row)
        {
            _Row = AppliedDB.Functions.RemoveNull(_Row);
            PurchaseRecord _Record = new();
            {
                _Record.ID = (int)_Row["ID"];
                _Record.Vou_No = (string)_Row["Vou_No"];
                _Record.Vou_Date = (DateTime)_Row["Vou_Date"];
                _Record.Inv_Date = (DateTime)_Row["Inv_Date"];
                _Record.Pay_Date = (DateTime)_Row["Pay_Date"];
                _Record.TitleCustomer = (string)_Row["Company"];
                _Record.TitleEmployee = (string)_Row["Employee"];
                _Record.City = (string)_Row["City"];
                _Record.Amount = (decimal)_Row["Amount"];
                _Record.Description = (string)_Row["Description"];

            }
            return _Record;
        }

        public PurchaseRecord GetRecord(int _ID)
        {
            var _Record = new PurchaseRecord();

            if (_ID == 0) { if (Records.Count > 0) { Record = Records.First(); } }
            else
            {

                foreach (PurchaseRecord _Item in Records)
                {
                    if (_Item.ID == _ID)
                    {
                        _Record = _Item;
                    }
                }
            }
            Record = _Record;
            return _Record;
        }
        #endregion

        #region Add
        public void Add()
        {
            Record = new PurchaseRecord();
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
        #endregion

        
    }

    public class PurchaseRecord
    {
        public int ID { get; set; } = 0;
        public string Vou_No { get; set; } = string.Empty;
        public DateTime Vou_Date { get; set; } = DateTime.Now;
        public DateTime Inv_Date { get; set; } = DateTime.Now;
        public DateTime Pay_Date { get; set; } = DateTime.Now;
        public string TitleCustomer { get; set; } = string.Empty;
        public string TitleEmployee { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; } = 0.00M;
        public ReportModel Report { get; set; } = new();

    }
}

using AppliedAccounts.Services;
using AppliedDB;
using AppliedGlobals;
using AppMessages;
using System.ComponentModel.DataAnnotations;
using System.Data;
using static AppliedDB.Enums;

namespace AppliedAccounts.Models
{
    public class CustomersModel
    {
        public GlobalService AppGlobal { get; set; }
        public DataSource? Source { get; set; }
        public string DBFile { get; set; } = string.Empty;
        public CustomerRecord Record { get; set; } = new();
        public List<CustomerRecord> Records { get; set; } = [];
        public List<DataRow> Data { get; set; } = new();
        public DataRow? MyDataRow { get; set; }
        public MessageClass MsgClass { get; set; }
        public bool RecordNotFound { get; set; } = false;
        public string SearchText { get; set; } = string.Empty;


        #region Constructor
        public CustomersModel() { }
        
        public CustomersModel(GlobalService _AppGlobal)
        {
            AppGlobal = _AppGlobal;
            MsgClass = new();
            Source = new(AppGlobal.AppPaths);
            Data = Source.GetList(Query.CustomersList);
            MyDataRow = Source.Seek(Tables.Customers, 0);

            if (Data is not null) { Records = GetFilterRecords(""); }


            if (MyDataRow is null) { RecordNotFound = true; Record = new CustomerRecord(); }
            else
            {
                Record = GetRecord(MyDataRow);
            }
        }
        public CustomersModel(GlobalService _AppGlobal, int ID)
        {
            AppGlobal = _AppGlobal;
            //AppUser = UserProfile;
            //DBFile = AppUser.DataFile;
            Source = new(AppGlobal.AppPaths);
            MyDataRow = Source.Seek(Tables.Customers, ID);
            Record = GetRecord(MyDataRow);
        }
        #endregion

        #region Get Record and DataRow
        private CustomerRecord GetRecord(DataRow _Row)
        {
            _Row = AppliedDB.Functions.RemoveNull(_Row);
            CustomerRecord _Record = new();
            {
                _Record.ID = (int)_Row["ID"];
                _Record.Code = (string)_Row["Code"];
                _Record.Title = (string)_Row["Title"];
                _Record.Address1 = (string)_Row["Address1"];
                _Record.Address2 = (string)_Row["Address2"];
                _Record.City = (string)_Row["City"];
                _Record.Country = (string)_Row["Country"];
                _Record.Phone = (string)_Row["Phone"];
                _Record.Mobile = (string)_Row["Mobile"];
                _Record.Email = (string)_Row["Email"];
                _Record.NTN = (string)_Row["NTN"];
                _Record.CNIC = (string)_Row["CNIC"];
                _Record.Notes = (string)_Row["Notes"];

            }
            return _Record;
        }

        public CustomerRecord GetRecord(int _ID)
        {

            foreach (CustomerRecord _Record in Records)
            {
                if (_Record.ID == _ID)
                {
                    Record = _Record;
                    return _Record;
                }
            }
            return new();
        }

        private DataRow GetDataRow(CustomerRecord _Record)
        {
            DataRow _DataRow = MyDataRow.Table.NewRow();
            _DataRow["Id"] = _Record.ID;
            _DataRow["Code"] = _Record.Code;
            _DataRow["Title"] = _Record.Title;
            _DataRow["Address1"] = _Record.Address1;
            _DataRow["Address2"] = _Record.Address2;
            _DataRow["City"] = _Record.City;
            _DataRow["Country"] = _Record.Country;
            _DataRow["Phone"] = _Record.Phone;
            _DataRow["Mobile"] = _Record.Mobile;
            _DataRow["Email"] = _Record.Email;
            _DataRow["NTN"] = _Record.NTN;
            _DataRow["CNIC"] = _Record.CNIC;
            return _DataRow;
        }
        #endregion

        #region Filter List
        private List<CustomerRecord> GetFilterRecords(string _Filter)
        {
            List<CustomerRecord> _FilterRecords = new List<CustomerRecord>();
            foreach (DataRow _Row in Data)
            {
                if (SearchText.Length == 0)
                {
                    _FilterRecords.Add(GetRecord(_Row));
                }
                else
                {
                    var IsSearch = false;
                    if (_Row["Code"].ToString().Contains(SearchText)) { IsSearch = true; }
                    if (_Row["Title"].ToString().Contains(SearchText)) { IsSearch = true; }
                    if (_Row["Address1"].ToString().Contains(SearchText)) { IsSearch = true; }
                    if (_Row["Address2"].ToString().Contains(SearchText)) { IsSearch = true; }
                    if (_Row["City"].ToString().Contains(SearchText)) { IsSearch = true; }
                    if (_Row["Country"].ToString().Contains(SearchText)) { IsSearch = true; }

                    if (IsSearch) { _FilterRecords.Add(GetRecord(_Row)); }

                }
            }
            return _FilterRecords;
        }
        #endregion

        #region Delete
        public bool Delete(int _ID)
        {
            //MyMessages = MessageClass.Messages;
            MyDataRow = Source!.Seek(Tables.Customers, _ID);
            if (MyDataRow is not null)
            {
                if ((int)MyDataRow["ID"] == _ID)
                {
                    var _Commands = new CommandClass(MyDataRow, Source.DBFile);
                    _Commands.DeleteRow();
                    if (_Commands.Effected > 0)
                    {
                        MsgClass = _Commands.MyMessages;
                        Data = Source.GetList(Query.CustomersList);
                        if (Data is not null) { Records = GetFilterRecords(""); }
                        return true;
                    }
                }
            }
            return false;
        }
        #endregion

        #region Save
        internal bool Save()
        {
            var _NewRow = GetDataRow(Record);
            if (Validate(_NewRow))
            {
                var _Commands = new CommandClass(_NewRow, DBFile);

                return _Commands.SaveChanges();
            }
            return false;
        }

        private bool Validate(DataRow _Row)
        {
            if (_Row["ID"] == null) { MsgClass.Alert(AppMessages.Enums.Messages.IDIsNull); }
            if (_Row["Code"] == null) { MsgClass.Alert(AppMessages.Enums.Messages.CodeIsNull); }
            if (_Row["Title"] == null) { MsgClass.Alert(AppMessages.Enums.Messages.TitleIsNull); }
            if (_Row["City"] == null) { MsgClass.Alert(AppMessages.Enums.Messages.CityIsZero); }

            if (((string)_Row["Code"]).Length == 0) { MsgClass.Alert(AppMessages.Enums.Messages.CodeIsZero); }
            if (((string)_Row["Title"]).Length == 0) { MsgClass.Alert(AppMessages.Enums.Messages.TitleIsZero); }
            if (((string)_Row["City"]).Length == 0) { MsgClass.Alert(AppMessages.Enums.Messages.CityIsZero); }

            return true;
        }
        #endregion

        #region Add
        public void Add()
        {
            Record = new CustomerRecord();
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
            Records = GetFilterRecords(SearchText);
        }
        #endregion
    }

    public class CustomerRecord
    {
        [Required]
        public int ID { get; set; }
        [Required]
        public string Code { get; set; } = string.Empty;
        [Required]
        public string Title { get; set; } = string.Empty;
        [Required]
        public string Address1 { get; set; } = string.Empty;
        public string Address2 { get; set; } = string.Empty;
        [Required]
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        [Phone]
        public string Phone { get; set; } = string.Empty;
        [Phone]
        public string Mobile { get; set; } = string.Empty;
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        public string NTN { get; set; } = string.Empty;
        public string CNIC { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;

    }

}

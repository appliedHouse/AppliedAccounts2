using AppliedDB;
using System.Data;
using static AppliedDB.Enums;
using MESSAGE = AppMessages.Enums.Messages;

namespace AppliedAccounts.Models
{
    public class COANatureModel
    {
        public AppUserModel? AppUser { get; set; }
        public DataSource? Source { get; set; }
        public string DBFile { get; set; } = string.Empty;
        public COANatureRecord Record { get; set; } = new();
        public List<COANatureRecord> Records { get; set; } = new();
        public List<DataRow> Data { get; set; } = new();

        public int CountRecord => Records.Count;
        public int Count => Data.Count;
        public AppMessages.MessageClass MsgClass { get; set; } = new();
        public string SearchText { get; set; } = string.Empty;
        public bool IsDeleted { get; set; } = false;

        #region Constructor
        public COANatureModel() { }
        public COANatureModel(AppUserModel _UserProfile) 
        {
            AppUser = _UserProfile;
            DBFile = AppUser.DataFile;
            Source = new(AppUser);
            Data = Source.GetList(Query.COANatureList);
            Records = GetFilterRecords(string.Empty);
            if (Records.Count > 0) { Record = Records.First(); } else { Record = new COANatureRecord(); }

        }
        #endregion

        #region Filter List
        private List<COANatureRecord> GetFilterRecords(string _Filter)
        {
            List<COANatureRecord> _FilterRecords = new();
            foreach (DataRow _Row in Data)
            {
                if (SearchText.Length == 0)
                {
                    _FilterRecords.Add(GetRecord(_Row));
                }
                else
                {
                    if (_Row["Code"].ToString().Contains(SearchText)) { _FilterRecords.Add(GetRecord(_Row)); }
                    if (_Row["Title"].ToString().Contains(SearchText)) { _FilterRecords.Add(GetRecord(_Row)); }
                }
            }
            return _FilterRecords;
        }
        #endregion

        #region Set Record and Data Row
        private COANatureRecord GetRecord(DataRow _Row)
        {
            COANatureRecord _Record = new();
            {
                _Record.ID = (int)_Row["ID"];
                _Record.Code = (string)_Row["Code"];
                _Record.Title = (string)_Row["Title"];

            }
            return _Record;
        }
        public COANatureRecord GetRecord(int _ID)
        {
            var _Record = new COANatureRecord();

            if (_ID == 0) { if (Records.Count > 0) { Record = Records.First(); } }
            else
            {

                foreach (COANatureRecord _Item in Records)
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
        private DataRow GetDataRow(COANatureRecord _Record)
        {
            DataRow _DataRow;
            if (Data.Count == 0)
            {
                _DataRow = DataSource.GetNewRow(DBFile, Tables.COA_Nature);
            }
            else
            {
                _DataRow = Data.First();
            }

            _DataRow["Id"] = _Record.ID;
            _DataRow["Code"] = _Record.Code;
            _DataRow["Title"] = _Record.Title;
            return _DataRow;

        }
        #endregion

        #region Add
        public void Add()
        {
            Record = new COANatureRecord();
        }
        #endregion

        #region Edit
        public void Edit(int _ID)
        {
            GetRecord(_ID);
        }
        #endregion

        #region Delete
        public bool Delete(int _ID)
        {
            GetRecord(_ID);
            IsDeleted = true;
            return true;
        }

        public bool DeleteRow(int _ID)
        {
            GetRecord(_ID);
            IsDeleted = false;
            //MyMessages = MessageClass.Messages;
            var _DeleteRow = DataSource.GetNewRow(DBFile, Tables.COA_Nature);

            if (_DeleteRow is not null)
            {
                _DeleteRow["ID"] = Record.ID;
                _DeleteRow["Code"] = Record.Code;
                _DeleteRow["Title"] = Record.Title;

                var _Commands = new CommandClass(_DeleteRow, DBFile);
                var _result = _Commands.DeleteRow();
                if (_result)
                {
                    // Refrest data from database table.
                    Data = Source!.GetList(Query.COANatureList);
                    Records = GetFilterRecords(string.Empty);
                    GetRecord(0);
                    return _result;
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
                if (_Commands.CommandInsert is not null || _Commands.CommandUpdate is not null)
                {
                    var _result = _Commands.SaveChanges();
                    if (_result)
                    {
                        // Refresh Data
                        Data = Source!.GetList(Query.COANatureList);
                        Records = GetFilterRecords(string.Empty);
                    }
                }
                else
                {
                    MsgClass.Add(MESSAGE.SQLQueryIsNull);
                }
            }

            return false;
        }
        #endregion

        #region Validate
        private bool Validate(DataRow _Row)
        {
            var _Validated = true;
            if (_Row["ID"] is null) { _Validated = false; MsgClass.Add(MESSAGE.IDIsNull); }
            if (_Row["Code"] is null) { _Validated = false; MsgClass.Add(MESSAGE.CodeIsNull); }
            if (_Row["Title"] is null) { _Validated = false; MsgClass.Add(MESSAGE.ColumnIsNull); }

            if (_Row["Code"].ToString()!.Length == 0) { _Validated = false; MsgClass.Add(MESSAGE.CodeIsZero); }
            if (_Row["Title"].ToString()!.Length == 0) { _Validated = false; MsgClass.Add(MESSAGE.TitleIsZero); }
            return _Validated;
        }
        #endregion

    }

    public class COANatureRecord
    {
        public int ID { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;

    }
}

using AppliedAccounts.Services;
using AppliedDB;
using System.Data;
using static AppliedDB.Enums;
using static AppMessages.Enums;

namespace AppliedAccounts.Models
{
    public class COAClassModel
    {
        //public AppliedGlobals.AppUserModel? AppUser { get; set; }
        public GlobalService AppGlobal { get; set; }
        public DataSource? Source { get; set; }
        public string DBFile => Source!.DBFile;
        public COAClassRecord Record { get; set; } = new();
        public List<COAClassRecord> Records { get; set; } = new();
        public List<DataRow> Data { get; set; } = new();

        public int CountRecord => Records.Count;
        public int Count => Data.Count;
        public AppMessages.MessageClass MsgClass { get; set; } = new();
        public string SearchText { get; set; } = string.Empty;
        public bool IsDeleted { get; set; } = false;
        public string MyMessage { get; private set; }


        #region Constructor
        public COAClassModel() { }
        public COAClassModel(GlobalService _AppGlobal)
        {
            AppGlobal = _AppGlobal;
            Source = new(AppGlobal.AppPaths);
            LoadData();
        }
        #endregion

        #region Load Data
        public void LoadData()
        {
            Source ??= new(AppGlobal.AppPaths);
            Data = Source.GetList(Query.COAClassList);
            Records = GetFilterRecords(string.Empty);
            if (Records.Count > 0) { Record = Records.First(); } else { Record = new COAClassRecord(); }
        }
        #endregion


        #region Filter List
        private List<COAClassRecord> GetFilterRecords(string _Filter)
        {
            List<COAClassRecord> _FilterRecords = [];
            string searchLower = SearchText?.ToLower() ?? string.Empty;

            foreach (DataRow _Row in Data)
            {
                if (string.IsNullOrEmpty(searchLower))
                {
                    _FilterRecords.Add(GetRecord(_Row));
                }
                else
                {
                    string code = _Row["Code"]?.ToString()?.ToLower() ?? string.Empty;
                    string title = _Row["Title"]?.ToString()?.ToLower() ?? string.Empty;

                    if (code.Contains(searchLower) || title.Contains(searchLower))
                    {
                        _FilterRecords.Add(GetRecord(_Row));
                    }
                }
            }
            return _FilterRecords;
        }
        #endregion

        #region Set Record and Data Row
        private COAClassRecord GetRecord(DataRow _Row)
        {
            COAClassRecord _Record = new();
            {
                _Record.ID = (long)_Row["ID"];
                _Record.Code = (string)_Row["Code"];
                _Record.Title = (string)_Row["Title"];

            }
            return _Record;
        }
        public void GetRecord(long _ID)
        {
            if (Records.Count > 0)
            {
                Record = Records.FirstOrDefault(e => e.ID == _ID) ?? Records.First();
            }
            else
            {
                Record = new COAClassRecord();
            }
        }

        private DataRow GetDataRow(COAClassRecord _Record)
        {
            DataRow _DataRow;
            if (Data.Count == 0)
            {
                _DataRow = DataSource.GetNewRow(DBFile, Tables.COA_Class);
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
            Record = new COAClassRecord();
        }
        #endregion

        #region Edit
        public void Edit(long _ID)
        {
            GetRecord(_ID);
        }
        #endregion

        #region Delete
        public bool Delete(long _ID)
        {
            MsgClass.ClearMessages();
            var _DeleteRow = Source!.GetDataRow(Tables.COA_Class, _ID);

            if (_DeleteRow is not null)
            {
                var _result = Source.Delete(_DeleteRow);
                LoadData();
                MyMessage = $"Record {_DeleteRow["Title"]} has been deleted sucessfully.";
                return _result;
            }
            MyMessage = $"Record {_DeleteRow!["Title"]} failed to be deleted.";
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
                        Data = Source!.GetList(Query.COAClassList);
                        Records = GetFilterRecords(string.Empty);
                        return true;
                    }
                }
                else
                {
                    MsgClass.Add(Messages.SQLQueryIsNull);
                }
            }

            return false;
        }
        #endregion

        #region Validate

        private bool Validate(DataRow _Row)
        {
            var _Validated = true;
            if (_Row["ID"] is null) { _Validated = false; MsgClass.Add(Messages.IDIsNull); }
            if (_Row["Code"] is null) { _Validated = false; MsgClass.Add(Messages.CodeIsNull); }
            if (_Row["Title"] is null) { _Validated = false; MsgClass.Add(Messages.ColumnIsNull); }

            if (_Row["Code"].ToString()!.Length == 0) { _Validated = false; MsgClass.Add(Messages.CodeIsZero); }
            if (_Row["Title"].ToString()!.Length == 0) { _Validated = false; MsgClass.Add(Messages.TitleIsZero); }
            return _Validated;
        }
        #endregion
    }

    public class COAClassRecord
    {
        public long ID { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
    }

}

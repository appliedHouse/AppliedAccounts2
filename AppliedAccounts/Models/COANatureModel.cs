using AppliedAccounts.Services;
using AppliedAccounts.Data.Mapping;
using AppliedDB;
using System.Data;
using static AppliedDB.Enums;
using MESSAGE = AppMessages.Enums.Messages;

namespace AppliedAccounts.Models
{
    public class COANatureModel
    {
        public GlobalService AppGlobal { get; set; }
        public DataSource? Source { get; set; }
        public string DBFile => Source!.DBFile ?? "" ;
        public COANatureRecord Record { get; set; } = new();
        public List<COANatureRecord> Records { get; set; } = new();
        public List<DataRow> Data { get; set; } = new();

        public int CountRecord => Records.Count;
        public int Count => Data.Count;
        public AppMessages.MessageClass MsgClass { get; set; } = new();
        public string SearchText { get; set; } = string.Empty;
        public bool IsDeleted { get; set; } = false;
        public string MyMessage { get; private set; }

        #region Constructor
        public COANatureModel() { }
        public COANatureModel(GlobalService _AppGlobal)
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
            Data = Source.GetList(Query.COANatureList);
            Records = GetFilterRecords(string.Empty);
            if (Records.Count > 0) { Record = Records.First(); } else { Record = new COANatureRecord(); }
        }
        #endregion



        #region Filter List
        private List<COANatureRecord> GetFilterRecords(string _Filter)
        {
            List<COANatureRecord> _FilterRecords = [];
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
        private COANatureRecord GetRecord(DataRow _Row)
        {


            COANatureRecord _Record = new();
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
                Record = new COANatureRecord();
            }
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

            return _Record.ToDataRow(_DataRow);

        }
        #endregion

        #region Add
        public void Add()
        {
            Record = new COANatureRecord();
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
            var _DeleteRow = Source!.GetDataRow(Tables.COA_Nature, _ID);

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
                        Data = Source!.GetList(Query.COANatureList);
                        Records = GetFilterRecords(string.Empty);
                        MyMessage = $"Record {Record.Title} has been saved successfully";
                        return true;
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
        public long ID { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;

    }
}

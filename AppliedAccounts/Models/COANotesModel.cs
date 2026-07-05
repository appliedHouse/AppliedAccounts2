using AppliedAccounts.Services;
using AppliedDB;
using System.Data;
using static AppliedDB.Enums;
using MESSAGE = AppMessages.Enums.Messages;

namespace AppliedAccounts.Models
{
    public class COANotesModel
    {
        public GlobalService AppGlobal { get; set; }
        public DataSource? Source { get; set; }
        public string DBFile => Source!.DBFile;
        public COANotesRecord Record { get; set; } = new();
        public List<COANotesRecord> Records { get; set; } = new();
        public List<DataRow> Data { get; set; } = new();

        public int CountRecord => Records.Count;
        public int Count => Data.Count;
        public AppMessages.MessageClass MsgClass { get; set; } = new();
        public string SearchText { get; set; } = string.Empty;
        public bool IsDeleted { get; set; } = false;
        public string MyMessage { get; set; } = "No Message";

        #region Constructor
        public COANotesModel() { }
        public COANotesModel(GlobalService _AppGlobal)
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
            Data = Source.GetList(Query.COANotesList);
            Records = GetFilterRecords(string.Empty);
            if (Records.Count > 0) { Record = Records.First(); } else { Record = new COANotesRecord(); }
        }
        #endregion


        #region Filter List
        private List<COANotesRecord> GetFilterRecords(string _Filter)
        {
            List<COANotesRecord> _FilterRecords = [];
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
        private COANotesRecord GetRecord(DataRow _Row)
        {
            COANotesRecord _Record = new();
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
                Record = new COANotesRecord();
            }
        }
        private DataRow GetDataRow(COANotesRecord _Record)
        {
            DataRow _DataRow;
            if (Data.Count == 0)
            {
                _DataRow = DataSource.GetNewRow(DBFile, Tables.COA_Notes);
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
            Record = new COANotesRecord();
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
            var _DeleteRow = Source!.GetDataRow(Tables.COA_Notes, _ID);

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
                        Data = Source!.GetList(Query.COANotesList);
                        Records = GetFilterRecords(string.Empty);
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
            if (_Row["ID"] is null)
            {
                MsgClass.Add(MESSAGE.IDIsNull);
                return false;
            }

            if (_Row["Code"] is null)
            {
                MsgClass.Add(MESSAGE.CodeIsNull);
                return false;
            }

            if (string.IsNullOrWhiteSpace(_Row["Code"].ToString()))
            {
                MsgClass.Add(MESSAGE.CodeIsZero);
                return false;
            }

            if (_Row["Title"] is null)
            {
                MsgClass.Add(MESSAGE.ColumnIsNull);
                return false;
            }

            if (string.IsNullOrWhiteSpace(_Row["Title"].ToString()))
            {
                MsgClass.Add(MESSAGE.TitleIsZero);
                return false;
            }

            return true;
        }
        #endregion

    }

    public class COANotesRecord
    {
        public long ID { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;

    }
}

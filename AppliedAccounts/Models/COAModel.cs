using AppliedAccounts.Data;
using AppliedDB;
using System.Data;
using static AppliedDB.Enums;
using static AppMessages.Enums;

namespace AppliedAccounts.Models
{
    public class COAModel
    {
        #region Valiables
        public AppUserModel? AppUser { get; set; }
        public DataSource? Source { get; set; }
        public string DBFile { get; set; } = string.Empty;
        public COARecord Record { get; set; } = new();
        public List<COARecord> Records { get; set; } = new();
        public List<DataRow> Data { get; set; } = new();

        public int CountRecord => Records.Count;
        public int Count => Data.Count;

        public List<Dictionary<int, string>> ClassList { get; set; } = new();
        public List<Dictionary<int, string>> NatureList { get; set; } = new();
        public List<Dictionary<int, string>> NotesList { get; set; } = new();

        public string SelectedClass { get; set; } = "Select..";
        public string SelectedNature { get; set; } = "Select..";
        public string SelectedNotes { get; set; } = "Select..";
        public string SearchText { get; set; } = string.Empty;
        public AppMessages.AppMessages MyMessages { get; set; } = new();

        #endregion

        #region Constructor
        public COAModel() { }
        public COAModel(AppUserModel UserProfile)
        {
            AppUser = UserProfile;
            DBFile = AppUser.DataFile;
            Source = new(AppUser);
            Data = Source.GetList(Query.COAList);
            Records = GetFilterRecords();

            if (Count > 0) { Record = Records.First(); } else { Record = new COARecord(); }

            ClassList = DataSource.GetDataList(DBFile, Tables.COA_Class);
            NatureList = DataSource.GetDataList(DBFile, Tables.COA_Nature);
            NotesList = DataSource.GetDataList(DBFile, Tables.COA_Notes);
            MyMessages = MessageClass.Messages;
        }
        #endregion

        #region Get Record and DataRow
        private COARecord GetRecord(DataRow _Row)
        {
            _Row = AppliedDB.Functions.RemoveNull(_Row);
            COARecord _Record = new();
            {
                _Record.ID = (int)_Row["ID"];
                _Record.Code = (string)_Row["Code"];
                _Record.Title = (string)_Row["Title"];
                _Record.Class = (int)_Row["Class"];
                _Record.Nature = (int)_Row["Nature"];
                _Record.Notes = (int)_Row["Notes"];

                _Record.TitleClass = (string)_Row["TitleClass"];
                _Record.TitleNature = (string)_Row["TitleNature"];
                _Record.TitleNote = (string)_Row["TitleNote"];
            }
            return _Record;
        }

        public COARecord GetRecord(int _ID)
        {
            var _Record = new COARecord();

            if (_ID == 0) { if (Records.Count > 0) { Record = Records.First(); } }
            else
            {
                foreach (COARecord _Item in Records)
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

        private DataRow GetDataRow(COARecord _Record)
        {
            DataRow _DataRow;
            if (Data.Count == 0)
            {
                _DataRow = DataSource.GetNewRow(DBFile, Tables.COA);
            }
            else
            {
                _DataRow = Data.First();
            }

            _DataRow["Id"] = _Record.ID;
            _DataRow["Code"] = _Record.Code;
            _DataRow["Title"] = _Record.Title;
            _DataRow["Class"] = _Record.Class;
            _DataRow["Nature"] = _Record.Nature;
            _DataRow["Notes"] = _Record.Notes;
            _DataRow["TitleClass"] = _Record.TitleClass;
            _DataRow["TitleNature"] = _Record.TitleNature;
            _DataRow["TitleNote"] = _Record.TitleNote;
            return _DataRow;
        }
        #endregion

        #region Filter List
        private List<COARecord> GetFilterRecords()
        {
            List<COARecord> _FilterRecords = new List<COARecord>();
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
                    if (_Row["TitleClass"].ToString().Contains(SearchText)) { _FilterRecords.Add(GetRecord(_Row)); }
                    if (_Row["TitleNature"].ToString().Contains(SearchText)) { _FilterRecords.Add(GetRecord(_Row)); }
                    if (_Row["TitleNote"].ToString().Contains(SearchText)) { _FilterRecords.Add(GetRecord(_Row)); }
                }
            }
            return _FilterRecords;
        }
        #endregion

        #region Delete
        public bool Delete(int _ID)
        {
            MyMessages = MessageClass.Messages;
            var _DeleteRow = DataSource.GetNewRow(DBFile, Tables.COA);

            if (_DeleteRow is not null)
            {
                _DeleteRow["ID"] = Record.ID;
                _DeleteRow["Code"] = Record.Code;
                _DeleteRow["Title"] = Record.Title;
                _DeleteRow["Class"] = Record.Class;
                _DeleteRow["Nature"] = Record.Nature;
                _DeleteRow["Notes"] = Record.Notes;

                var _Commands = new CommandClass(_DeleteRow, DBFile);
                return _Commands.DeleteRow();
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
                var _Commands = new CommandClass(_NewRow, DBFile, MyMessages);
                return _Commands.SaveChanges();
            }
                        
            return false;
        }
        #endregion

        #region Add
        public void Add()
        {
            Record = new COARecord();
        }
        #endregion

        #region Edit
        public void Edit(int _ID)
        {
            Record = GetRecord(_ID);
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




        #region Validate
        private bool Validate(DataRow _Row)
        {
            var _Validated = true;
            if (_Row["ID"] is null) { _Validated = false; MyMessages.Add(Messages.IDIsNull); }
            if (_Row["Code"] is null) { _Validated = false; MyMessages.Add(Messages.CodeIsNull); }
            if (_Row["Title"] is null) { _Validated = false; MyMessages.Add(Messages.TitleIsNull); }
            if (_Row["Class"] is null) { _Validated = false; MyMessages.Add(Messages.ColumnIsNull); }
            if (_Row["Nature"] is null) { _Validated = false; MyMessages.Add(Messages.ColumnIsNull); }
            if (_Row["Notes"] is null) { _Validated = false; MyMessages.Add(Messages.ColumnIsNull); }

            if (_Row["Code"].ToString().Length == 0) { _Validated = false; MyMessages.Add(Messages.CodeIsZero); }
            if (_Row["Title"].ToString().Length == 0) { _Validated = false; MyMessages.Add(Messages.TitleIsZero); }
            if (_Row["Class"].ToString().Length == 0) { _Validated = false; MyMessages.Add(Messages.AccClassZero); }
            if (_Row["Nature"].ToString().Length == 0) { _Validated = false; MyMessages.Add(Messages.AccNatureZero); }
            if (_Row["Notes"].ToString().Length == 0) { _Validated = false; MyMessages.Add(Messages.AccNotesZero); }

            if (_Row["Code"].ToString().Length != 6) { _Validated = false; MyMessages.Add(Messages.CodeLength6); }


            return _Validated;
        }
        #endregion

    }

    public class COARecord
    {
        public int ID { get; set; } = 0;
        public string Code { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public int Class { get; set; } = 0;
        public int Nature { get; set; } = 0;
        public int Notes { get; set; } = 0;
        public decimal OBal { get; set; } = 0.00M;

        public virtual string? TitleClass { get; set; }
        public virtual string? TitleNature { get; set; }
        public virtual string? TitleNote { get; set; }


    }

}

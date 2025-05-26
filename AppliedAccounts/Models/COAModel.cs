using AppliedAccounts.Services;
using AppliedDB;
using System.Data;
using static AppliedDB.Enums;
using MESSAGES = AppMessages.Enums.Messages;

namespace AppliedAccounts.Models
{
    public class COAModel
    {
        #region Valiables
        public GlobalService AppGlobals { get; set; }
        public DataSource? Source { get; set; }
        public string DBFile { get; set; } = string.Empty;
        public COARecord Record { get; set; } = new();
        public List<COARecord> Records { get; set; } = new();
        public List<DataRow> Data { get; set; } = new();

        public int CountRecord => Records.Count;
        public int Count => Data.Count;

        public List<CodeTitle> ClassList { get; set; } = new();
        public List<CodeTitle> NatureList { get; set; } = new();
        public List<CodeTitle> NotesList { get; set; } = new();

        public string SelectedClass { get; set; } = "Select..";
        public string SelectedNature { get; set; } = "Select..";
        public string SelectedNotes { get; set; } = "Select..";
        public string SearchText { get; set; } = string.Empty;
        public AppMessages.MessageClass MsgClass { get; set; } = new();
        public BrowseModel BrowseClass { get; set; } = new();

        #endregion

        #region Constructor
        public COAModel() { }
        public COAModel(GlobalService _AppGlobals)
        {
            AppGlobals = _AppGlobals;
            Source = new(AppGlobals.AppPaths);
            Data = Source.GetTable(SQLQueries.Quries.COA()).AsEnumerable().ToList();
            Records = GetFilterRecords();

            ClassList = Source.GetAccClass();
            NatureList = Source.GetAccNature();
            NotesList = Source.GetAccNotes();

            if (Count > 0) { Record = Records.First(); } else { Record = new COARecord(); }

            // = MessageClass.Messages;
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
            var OIC = StringComparison.OrdinalIgnoreCase;

            var filteredData = Data.AsEnumerable()
                .Where(row =>
                    (row.Field<string>("Code")?.Contains(SearchText, OIC) ?? false)
                  || (row.Field<string>("Title")?.Contains(SearchText, OIC) ?? false)
                  || (row.Field<string>("TitleClass")?.Contains(SearchText, OIC) ?? false)
                  || (row.Field<string>("TitleNature")?.Contains(SearchText, OIC) ?? false)
                  || (row.Field<string>("TitleNote")?.Contains(SearchText, OIC) ?? false))
                .Select(GetRecord)
                .ToList();

            return filteredData;


        }
        #endregion

        #region Delete
        public bool Delete(int _ID)
        {
            //MyMessages = MessageClass.Messages;
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
            var _NewRow = Source!.GetNewRow(Tables.COA);

            _NewRow["ID"] = Record.ID;
            _NewRow["Code"] = Record.Code;
            _NewRow["Title"] = Record.Title;
            _NewRow["Class"] = Record.Class;
            _NewRow["Nature"] = Record.Nature;
            _NewRow["Notes"] = Record.Notes;
            _NewRow["OPENING_BALANCE"] = Record.OBal;

            if (Validate(_NewRow))
            {
                Source!.Save(_NewRow);
                return true;

                //var _Commands = new CommandClass(_NewRow, DBFile);
                //return _Commands.SaveChanges();
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
            if (_Row["ID"] is null) { _Validated = false; MsgClass.Add(MESSAGES.IDIsNull); }
            if (_Row["Code"] is null) { _Validated = false; MsgClass.Add(MESSAGES.CodeIsNull); }
            if (_Row["Title"] is null) { _Validated = false; MsgClass.Add(MESSAGES.TitleIsNull); }
            if (_Row["Class"] is null) { _Validated = false; MsgClass.Add(MESSAGES.ClassIsNull); }
            if (_Row["Nature"] is null) { _Validated = false; MsgClass.Add(MESSAGES.NatureIsNull); }
            if (_Row["Notes"] is null) { _Validated = false; MsgClass.Add(MESSAGES.NotesIsNull); }

            if (_Row["Code"].ToString()?.Length == 0) { _Validated = false; MsgClass.Add(MESSAGES.CodeIsZero); }
            if (_Row["Title"].ToString()?.Length == 0) { _Validated = false; MsgClass.Add(MESSAGES.TitleIsZero); }
            if (_Row["Class"].ToString()?.Length == 0) { _Validated = false; MsgClass.Add(MESSAGES.AccClassZero); }
            if (_Row["Nature"].ToString()?.Length == 0) { _Validated = false; MsgClass.Add(MESSAGES.AccNatureZero); }
            if (_Row["Notes"].ToString()?.Length == 0) { _Validated = false; MsgClass.Add(MESSAGES.AccNotesZero); }
            if (_Row["Code"].ToString()?.Length != 6) { _Validated = false; MsgClass.Add(MESSAGES.CodeLength6); }


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

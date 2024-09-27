using Microsoft.AspNetCore.Components;
using AppliedAccounts.Data;
using AppliedDB;
using AppLanguages;
using System.ComponentModel.DataAnnotations;
using System.Data;
using MESSAGE = AppMessages.Enums.Messages;


namespace AppliedAccounts.Models
{


    public class BooksModel
    {
        #region Variables

        [Inject] public NavigationManager NavManager { set; get; } = default!;
        [Inject] public Globals AppGlobals { get; set; } = default!;
        [Inject] public IConfiguration Appconfig { get; set; } = default!;

        public string? Vou_No { get; set; }
        public int BookID { get; set; }
        public string? TitleBook { get; set; }

        public AppUserModel? UserProfile { get; set; }
        public CashRecord? Record { get; set; }
        public List<CashRecord>? Records { get; set; }

        public List<CodeTitle> Companies { get; set; }
        public List<CodeTitle> Employees { get; set; }
        public List<CodeTitle> Projects { get; set; }
        public List<CodeTitle> Accounts { get; set; }

        public DataTable? TB_Book { get; set; }
        public DataRow? CurrentRow { get; set; }
        public DataSource? Source { get; set; }
        public AppMessages.AppMessages MyMessages { get; set; } = MessageClass.Messages;

        //public virtual decimal Amount => DR - CR;
        public virtual bool FoundRow { get; set; } = false;
        public virtual bool IsModelValid { get; set; }
        public virtual LangPack LangClass { get; set; }




        #endregion

        #region Constructor


        public BooksModel()
        {
            NavManager.NavigateTo("/");


        }

        public BooksModel(AppUserModel _UserProfile, int _BookID, string _Vou_No, int _LangID)
        {

            _LangID = 1;

            UserProfile = _UserProfile;
            Vou_No = _Vou_No;
            BookID = _BookID;
            Source = new(UserProfile);
            LangClass = new(_LangID, "Book");
            LoadData();

        }
        #endregion

        #region Load Data
        public bool LoadData()
        {
            if (UserProfile is not null)
            {
                try
                {
                    var DBFile = UserProfile.DataFile.ToString();
                    if (string.IsNullOrEmpty(Vou_No)) { Vou_No = "New"; }

                    if (Vou_No == "New")
                    {
                        Record = NewRecord();
                        Records = new();

                    }
                    else
                    {
                        var _Query = $"SELECT * FROM [CashBook] WHERE [Vou_No] = '{Vou_No}'";
                        TB_Book = DataSource.GetDataTable(DBFile, _Query, "Voucher");
                        Row2Rec(0);

                    }

                    Companies = Source.GetCustomers();
                    Employees = Source.GetEmployees();
                    Projects = Source.GetProjects();
                    Accounts = Source.GetAccounts();
                    TitleBook = DataSource.GetTitle(Accounts, BookID);

                    return true;
                }
                catch (Exception)
                {
                    MyMessages.Add(MESSAGE.DataNotLoaded);
                    return false;
                }

            }
            return false;
        }
        #endregion

        #region Get Row
        private DataRow GetDataRow(int _ID)
        {
            FoundRow = false;
            DataRow _CurrentRow;
            if (TB_Book is not null)
            {
                TB_Book.DefaultView.RowFilter = $"ID={_ID}";
                if (TB_Book.DefaultView.Count > 0)
                {
                    FoundRow = true;
                    _CurrentRow = TB_Book.DefaultView[0].Row;
                }
                else
                {
                    _CurrentRow = TB_Book.NewRow();
                }
            }
            else
            {
                // if Book is null
                TB_Book = new();
                _CurrentRow = TB_Book.NewRow();
            }

            return _CurrentRow;
        }
        private DataRow GetRowIndex(int _Index)
        {
            FoundRow = false;
            DataRow _CurrentRow;
            if (TB_Book != null)
            {

                if (TB_Book.Rows.Count == 1)
                {
                    FoundRow = true;
                    _CurrentRow = TB_Book.Rows[_Index];
                }
                else
                {
                    _CurrentRow = TB_Book.NewRow();
                }
            }
            else
            {
                TB_Book = new();
                _CurrentRow = TB_Book.NewRow();
            }
            return _CurrentRow;
        }
        #endregion

        #region Record -> Row --> Record
        public void Rec2Row()
        {
            if (TB_Book is not null)
            {
                CurrentRow = TB_Book.NewRow();
            }
            else
            {
                CurrentRow = Source.GetNewRow(Enums.Tables.CashBook);
            }


            CurrentRow["ID"] = Record.ID;
            CurrentRow["Vou_No"] = Record.Vou_No;
            CurrentRow["Vou_Date"] = Record.Vou_Date;
            CurrentRow["BookID"] = Record.BookID;
            CurrentRow["COA"] = Record.COA;
            CurrentRow["Ref_No"] = Record.Ref_No;
            CurrentRow["Sheet_No"] = Record.Sheet_No;
            CurrentRow["DR"] = Record.DR;
            CurrentRow["CR"] = Record.CR;
            CurrentRow["Customer"] = Record.Company;
            CurrentRow["Employee"] = Record.Employee;
            CurrentRow["Project"] = Record.Project;
            CurrentRow["Description"] = Record.Description;
            CurrentRow["Comments"] = Record.Comments;
            CurrentRow["Status"] = Record.Status;

        }
        public void Row2Rec(int _ID)
        {
            CurrentRow = GetDataRow(_ID);
            if (FoundRow)
            {
                Record = new();
                {
                    Record.ID = (int)CurrentRow["ID"];
                    Record.Vou_No = (string)CurrentRow["Vou_No"];
                    Record.Vou_Date = (DateTime)CurrentRow["Vou_Date"];
                    Record.BookID = (int)CurrentRow["BookID"];
                    Record.COA = (int)CurrentRow["COA"];
                    Record.Ref_No = (string)CurrentRow["Ref_No"];
                    Record.Sheet_No = (string)CurrentRow["Sheet_No"];
                    Record.DR = (decimal)CurrentRow["DR"];
                    Record.CR = (decimal)CurrentRow["CR"];
                    Record.Company = (int)CurrentRow["Customer"];
                    Record.Project = (int)CurrentRow["Project"];
                    Record.Employee = (int)CurrentRow["Employee"];
                    Record.Description = (string)CurrentRow["Description"];
                    Record.Comments = (string)CurrentRow["Comments"];
                    Record.Status = (string)CurrentRow["Status"];
                }
            }
            else
            {
                Record = NewRecord();
            }
        }
        #endregion

        #region New Cash Record / Empty Record
        public CashRecord NewRecord()
        {
            CashRecord _Record = new(); ;
            _Record.ID = 0;
            _Record.Vou_No = Vou_No ?? "New";
            _Record.Vou_Date = DateTime.Today;
            _Record.BookID = BookID;
            _Record.COA = 0;
            _Record.Ref_No = string.Empty;
            _Record.Sheet_No = string.Empty;
            _Record.DR = 0.00M;
            _Record.CR = 0.00M;
            _Record.Company = 0;
            _Record.Project = 0;
            _Record.Employee = 0;
            _Record.Description = string.Empty;
            _Record.Comments = string.Empty;
            _Record.Status = "Add";

            return _Record;
        }
        #endregion

        #region Update, Save, New Record
        internal void Update()
        {

            Rec2Row();              // Convert page Variables into row for validation
            if (CurrentRow is not null && Record is not null)
            {
                if (Validate(CurrentRow))
                {
                    Records.Add(Record);
                    IsModelValid = true;
                }
                else
                {
                    IsModelValid = false;
                }
            }
        }
        #endregion

        #region Validate
        private bool Validate(DataRow _Row)
        {
            var _Validated = true;

            if (_Row["ID"] is null) { _Validated = false; MyMessages.Add(MESSAGE.IDIsNull); }
            if (_Row["Vou_No"] is null) { _Validated = false; MyMessages.Add(MESSAGE.VouNoIsNull); }
            if (_Row["BookID"] is null) { _Validated = false; MyMessages.Add(MESSAGE.BookIDIsNull); }
            if (_Row["COA"] is null) { _Validated = false; MyMessages.Add(MESSAGE.COAIsNull); }

            if (_Row["Customer"] is null) { _Validated = false; MyMessages.Add(MESSAGE.CustomerIDIsNull); }
            if (_Row["Employee"] is null) { _Validated = false; MyMessages.Add(MESSAGE.EmployeeIDIsNull); }

            if (_Row["Project"] is null) { _Validated = false; MyMessages.Add(MESSAGE.ProjectIDIsNull); }


            if (_Row["Description"] is null) { _Validated = false; MyMessages.Add(MESSAGE.DescriptionIsNull); }


            if (_Row["DR"] is null) { _Validated = false; MyMessages.Add(MESSAGE.DRIsNull); }
            if (_Row["CR"] is null) { _Validated = false; MyMessages.Add(MESSAGE.CRIsNull); }

            if (_Validated == false) { return _Validated; }    // return false if found null value in the above objects.

            if ((int)_Row["BookID"] == 0) { _Validated = false; MyMessages.Add(MESSAGE.BookIDIsZero); }
            if (string.IsNullOrEmpty(_Row["Vou_No"].ToString())) { _Validated = false; MyMessages.Add(MESSAGE.VouNoNotDefine); }
            if ((int)_Row["COA"] == 0) { _Validated = false; MyMessages.Add(MESSAGE.AccountIDIsZero); }
            if ((int)_Row["Customer"] == 0) { _Validated = false; MyMessages.Add(MESSAGE.CustomerIDIsZero); }
            if ((int)_Row["Employee"] == 0) { _Validated = false; MyMessages.Add(MESSAGE.EmployeeIDIsZero); }
            if ((int)_Row["Project"] == 0) { _Validated = false; MyMessages.Add(MESSAGE.ProjectIDIsZero); }

            if ((decimal)_Row["DR"] > 0 && (decimal)_Row["CR"] > 0) { _Validated = false; MyMessages.Add(MESSAGE.DRnCRHaveValue); }
            if ((decimal)_Row["DR"] == 0 && (decimal)_Row["CR"] == 0) { _Validated = false; MyMessages.Add(MESSAGE.DRnCRAreZero); }
            if ((decimal)_Row["DR"] < 0) { _Validated = false; MyMessages.Add(MESSAGE.DRIsNegative); }
            if ((decimal)_Row["DR"] < 0) { _Validated = false; MyMessages.Add(MESSAGE.CRIsNegative); }

            if (string.IsNullOrEmpty(_Row["Description"].ToString())) { _Validated = false; MyMessages.Add(MESSAGE.DescriptionIsNothing); }

            return _Validated;
        }
        #endregion

        #region Check Validity of Model
        public bool GetModelValidation()
        {
            var _Result = true;
            if (UserProfile is null) { _Result = false; MyMessages.Add(MESSAGE.UserProfileIsNull); }
            if (Vou_No is null) { _Result = false; MyMessages.Add(MESSAGE.VouNoNotDefine); }
            if (BookID == 0) { _Result = false; MyMessages.Add(MESSAGE.BookIDIsZero); }
            if (Record is null) { _Result = false; MyMessages.Add(MESSAGE.RecordIsNull); }
            if (Records is null) { _Result = false; MyMessages.Add(MESSAGE.RecordsAreNull); }
            if (Source is null) { _Result = false; MyMessages.Add(MESSAGE.DataSourceIsNull); }
            if (Source.MyConnection is null) { _Result = false; MyMessages.Add(MESSAGE.ConnectionsIsNull); }
            if (Companies is null) { _Result = false; MyMessages.Add(MESSAGE.CompanyListIsNull); }
            if (Employees is null) { _Result = false; MyMessages.Add(MESSAGE.EmployeeListIsNull); }
            if (Projects is null) { _Result = false; MyMessages.Add(MESSAGE.ProjectListIsNull); }
            if (Accounts is null) { _Result = false; MyMessages.Add(MESSAGE.AccountListIsNull); }
            //if (TB_Book is null) { _Result = false; MyMessages.Add(MESSAGE.BookTableIsNull); }
            //if (CurrentRow is null) { _Result = false; MyMessages.Add(MESSAGE.CurrentRowIsNull); }
            IsModelValid = _Result;
            return _Result;

        }
        #endregion

        #region Get Language Pack
        public LangPack GetLangPack(int _LangID)
        {
            return new(_LangID, "Book");
        }


        #endregion


    }

    #region Record & Language Class 

    public class CashRecord
    {
        [Required]
        public int ID { get; set; }
        [Required]
        public DateTime Vou_Date { get; set; } = DateTime.Now;
        [Required]
        public string Vou_No { get; set; } = string.Empty;
        public int BookID { get; set; }
        public int COA { get; set; }
        public string Ref_No { get; set; } = string.Empty;
        public string Sheet_No { get; set; } = string.Empty;

        [Range(1, Int32.MaxValue)]
        public decimal DR { get; set; }
        [Range(1, Int32.MaxValue)]
        public decimal CR { get; set; }
        public int Company { get; set; }
        public int Employee { get; set; }
        public int Project { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Comments { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;

        public virtual int SelectedBookID { get; set; }
        public virtual decimal Amount => (DR - CR);


        public string TitleAccount { get; set; } = string.Empty;
        public string TitleProject { get; set; } = string.Empty;
        public string TitleCompany { get; set; } = string.Empty;
        public string TitleEmployee { get; set; } = string.Empty;


    }

    public class LangPack
    {
        public int LangID { get; set; }  // Current Language ID
        public int LangInt => (int)LangID;  // Current Language ID
        public string? LangSection { get; set; }

        public string VouNo { get; set; } = string.Empty;
        public string VouDate { get; set; } = string.Empty;
        public string RefNo { get; set; } = string.Empty;
        public string ExpSheet { get; set; } = string.Empty;
        public string Company { get; set; } = string.Empty;
        public string Account { get; set; } = string.Empty;
        public string Employee { get; set; } = string.Empty;
        public string Project { get; set; } = string.Empty;
        public string Paid { get; set; } = string.Empty;
        public string Received { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Comments { get; set; } = string.Empty;
        public string Update { get; set; } = string.Empty;
        public string Add { get; set; } = string.Empty;
        public string New { get; set; } = string.Empty;
        public string Delete { get; set; } = string.Empty;
        public string Save { get; set; } = string.Empty;


        public LangPack(int _LangID, string _Section)
        {
            LangID = _LangID;
            LangSection = _Section;
            GetLangPack();
        }

        //public Dictionary<int, string> LangList => LanguageListClass.GetLanguageList();

        public void GetLangPack()
        {
            LangSection ??= "Common";

            Language? _Language = new(LangID, LangSection);

            if (_Language != null)
            {

                VouNo = _Language.GetValue("VouNo");
                VouDate = _Language.GetValue("VouDate");
                RefNo = _Language.GetValue("RefNo");
                ExpSheet = _Language.GetValue("ExpSheet");
                Company = _Language.GetValue("Company");
                Account = _Language.GetValue("Account");
                Employee = _Language.GetValue("Employee");
                Project = _Language.GetValue("Project");
                Paid = _Language.GetValue("Paid");
                Received = _Language.GetValue("Received");
                Description = _Language.GetValue("Description");
                Comments = _Language.GetValue("Comments");
                Update = _Language.GetValue("Update");
                Add = _Language.GetValue("Add");
                New = _Language.GetValue("New");
                Delete = _Language.GetValue("Delete");
                Save = _Language.GetValue("Save");
                _Language = null;
            }


        }


    }
    #endregion
}
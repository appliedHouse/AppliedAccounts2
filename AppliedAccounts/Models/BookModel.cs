using AppliedDB;
using AppMessages;
using System.ComponentModel.DataAnnotations;
using MESSAGE = AppMessages.Enums.Messages;
using Tables = AppliedDB.Enums.Tables;

namespace AppliedAccounts.Models
{
    public class BookModel
    {

        #region Variables
        public int BookID { get; set; }
        public int BookNature { get; set; }
        public string BookName { get; set; }
        public string DBFile { get; set; }

        public BookRecord Record { get; set; }
        public List<BookRecord>? Records { get; set; }
        public List<CodeTitle> Companies { get; set; }
        public List<CodeTitle> Employees { get; set; }
        public List<CodeTitle> Projects { get; set; }
        public List<CodeTitle> Accounts { get; set; }
        public List<CodeTitle> BookList { get; set; }

        public AppUserModel? UserProfile { get; set; }
        public DataSource Source { get; set; }
        public bool IsDataLoaded { get; set; }
        public AppMessages.AppMessages MyMessages { get; set; }

        #endregion
        #region Constructor
        public BookModel() 
        {
            MyMessages = new();
        }
        public BookModel(int _BookID, AppUserModel _AppUserProfile)
        {
            BookID = _BookID;
            UserProfile = _AppUserProfile;
            if (UserProfile != null)
            {
                Source = new(UserProfile);
                IsDataLoaded = LoadData(Source);
            };

            MyMessages = new();
        }
        #endregion

        private bool LoadData(DataSource source)
        {
            MyMessages = new();
            var _IsDataLoaded = true;
            if (BookID == 0) { _IsDataLoaded = false; }
            if (UserProfile == null) { _IsDataLoaded = false; }
            if (source == null) { _IsDataLoaded = false; }
            if (!_IsDataLoaded) { MyMessages.Add(MESSAGE.DataNotLoaded); return _IsDataLoaded; }

            //BookNature = (int)(Source.SeekValue(Tables.COA, BookID, "Nature") ?? 0);
            //if (BookNature == 0) { _IsDataLoaded = false; }
            if (!_IsDataLoaded) { MyMessages.Add(MESSAGE.BookIDIsZero); return _IsDataLoaded; }

            BookList = Source.GetBookAccounts(BookNature);
            Companies = Source.GetCustomers();
            Employees = Source.GetEmployees();
            Projects = Source.GetProjects();
            Accounts = Source.GetAccounts();

            return _IsDataLoaded;
        }

    }

    public class BookRecord
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

        [Range(1, 999999999)]
        public decimal DR { get; set; }
        [Range(1, 999999999)]
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
}

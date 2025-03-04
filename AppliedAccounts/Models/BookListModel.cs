using AppliedAccounts.Data;
using AppliedDB;
using Messages = AppMessages.Enums.Messages;
namespace AppliedAccounts.Models
{
    public class BookListModel
    {
        public List<CodeTitle> BookList { get; set; }
        public List<CodeTitle> NatureAccountsList { get; set; }
        public DataSource Source { get; set; }
        public AppUserModel? UserProfile { get; set; }
        public MessageClass MsgClass { get; set; }

        public int BookID { get; set; }
        public int BookNatureID { get; set; }
        public int SelectedVoucherID { get; set; }
        public DateTime DT_Start { get; set; }
        public DateTime DT_End { get; set; }
        public string SearchText { get; set; }
        public string BookTitle = "Book Title";
        public bool PageIsValid = false;


        public BookListModel() { }
        public BookListModel(int _BookID, AppUserModel _AppUserProfile)
        {
            MsgClass = new();

            try
            {
                if (_BookID == 0) { BookID = 1; } else { BookID = _BookID; }

                var result = Source?.SeekValue(Enums.Tables.COA, BookID, "Nature") ?? 0;
                BookNatureID = (int)result;

                BookID = _BookID;
                UserProfile = _AppUserProfile;
                if (UserProfile != null)
                {
                    Source = new(UserProfile);

                }
                
                NatureAccountsList =
                [
                    new() { ID = 1, Code = "01", Title = "Cash" },
                    new() { ID = 2, Code = "02", Title = "Bank" },
                ];

                PageIsValid = true;
            }
            catch (Exception)
            {
                MsgClass.Add(Messages.PageIsNotValid);

            }


            
        }



    }           // END
}


using AppliedDB;
using Messages = AppMessages.Enums.Messages;
using AppliedAccounts.Models.Interface;
using System.Data;
using AppliedAccounts.Services;
using Microsoft.AspNetCore.Components;
using AppliedAccounts.Data;

namespace AppliedAccounts.Models
{
    public class BookListModel  // : IVoucherList
    {
        public List<CodeTitle> BookList { get; set; }
        public List<CodeTitle> NatureAccountsList { get; set; }
        public DataSource Source { get; set; }
        public AppUserModel? UserProfile { get; set; }
        public AppMessages.MessageClass MsgClass { get; set; }

        public int BookID { get; set; }
        public int BookNatureID { get; set; }
        public int SelectedVoucherID { get; set; }
        public DateTime DT_Start { get; set; }
        public DateTime DT_End { get; set; }
        public string SearchText { get; set; }
        public string BookTitle = "Book Title";
        public bool PageIsValid { get; set; } = false;
        public List<DataRow> DataList { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Enums.Tables Table { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public PrintService Printer { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public AppUserModel? AppUser { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        
        public object Record { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public List<object> Records { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public decimal TotalAmount { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool SelectAll { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public NavigationManager NavManager { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public BookListModel() { }
        public BookListModel(int _BookID, AppUserModel _AppUserProfile)
        {
            MsgClass = new();
            UserProfile = _AppUserProfile;
            Source = new(UserProfile);
            GetKeys();

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

        public List<DataRow> LoadData() { return []; }

        public string GetFilterText()
        {
            throw new NotImplementedException();
        }

        public void Print(int _ID)
        {
            SetKeys();
            throw new NotImplementedException();
        }

        public void Edit(int _ID)
        {
            SetKeys();
            NavManager.NavigateTo($"/Accounts/Books/{BookID}/{BookNatureID}");
        }




        #region Get & Set Keys
        internal void SetKeys()
        {
            if (!string.IsNullOrEmpty(Source.DBFile))
            {
                AppRegistry.SetKey(Source.DBFile, "BkNatureID", BookNatureID, KeyType.Number);
                AppRegistry.SetKey(Source.DBFile, "BkBook", BookID, KeyType.Number, "Cash / Bank BooK");
                AppRegistry.SetKey(Source.DBFile, "BkBook", DT_Start, KeyType.From);
                AppRegistry.SetKey(Source.DBFile, "BkBook", DT_End, KeyType.To);
                AppRegistry.SetKey(Source.DBFile, "BkBook", SearchText, KeyType.Text);
            }
        }

        internal void GetKeys()
        {
            if (!string.IsNullOrEmpty(Source.DBFile))
            {
                BookNatureID = AppRegistry.GetNumber(Source.DBFile, "BKNatureID");
                BookID = AppRegistry.GetNumber(Source.DBFile, "BKbook");
                DT_Start = AppRegistry.GetFrom(Source.DBFile, "BKbook");
                DT_End = AppRegistry.GetTo(Source.DBFile, "BKbook");
                SearchText = AppRegistry.GetText(Source.DBFile, "BKBook");
            }
        }
        #endregion


    }           // END
}


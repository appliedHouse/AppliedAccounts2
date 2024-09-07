using AppliedAccounts.Models;

namespace AppliedAccounts.Pages.Accounts
{
    interface Temp
    {
        string ErrorMessage { get; set; }
    }

    public partial class COA1 : Temp
    {
        public COAModel Model { get; set; } = new();
        public bool IsPageValid { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public COA1() { }

        public bool GetPageIsValid()
        {
            var _Valid = true;
            if( Model.AppUser is null) { _Valid = false; ErrorMessage = "User not define properly."; }
            if( Model.Records is null) { _Valid = false; ErrorMessage = "Records not found"; }
            if( Model.Record is null) { _Valid = false; ErrorMessage = "Account Class List is empty"; }
            if( Model.NatureList is null) { _Valid = false; ErrorMessage = "Account Nature List is empty"; }
            if( Model.NotesList is null) { _Valid = false; ErrorMessage = "Financial Notes List is empty"; }
            if( Model.MyMessages is null) { _Valid = false; ErrorMessage = "Message Class is null"; }
            return _Valid;
        }

    }
}





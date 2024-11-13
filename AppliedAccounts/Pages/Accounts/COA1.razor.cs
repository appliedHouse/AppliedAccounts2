using AppLanguages;
using AppliedAccounts.Models;
using System.Reflection.Metadata;

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
        public LanguagePack LangPack { get; set; }

        public int LangID = 2;
        public string Section = "COA";



        public COA1()
        {
            GetLangPack(LangID, Section);


        }

        public bool GetPageIsValid()
        {
            var _Valid = true;
            if (Model.AppUser is null) { _Valid = false; ErrorMessage = "User not define properly."; }
            if (Model.Records is null) { _Valid = false; ErrorMessage = "Records not found"; }
            if (Model.Record is null) { _Valid = false; ErrorMessage = "Account Class List is empty"; }
            if (Model.NatureList is null) { _Valid = false; ErrorMessage = "Account Nature List is empty"; }
            if (Model.NotesList is null) { _Valid = false; ErrorMessage = "Financial Notes List is empty"; }
            if (Model.MyMessages is null) { _Valid = false; ErrorMessage = "Message Class is null"; }
            return _Valid;
        }

        public void GetLangPack(int _LangID, string _Section)
        {
            if (_Section == null) { return; }

            AppLanguages.Language _Language = new(_LangID, _Section);

            _Language.GetValue("Search");
            LangPack = new();

            LangPack.Search = _Language.GetValue("Search");

        }



        public class LanguagePack
        {
            public LanguagePack() { }
            public string Search { get; set; } = string.Empty;



        }
    }
}





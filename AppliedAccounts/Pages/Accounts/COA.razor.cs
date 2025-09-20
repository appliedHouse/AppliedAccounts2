using AppliedAccounts.Models;
using Microsoft.JSInterop;

namespace AppliedAccounts.Pages.Accounts
{


    public partial class COA
    {
        public COAModel MyModel { get; set; } = new();
        public bool IsPageValid { get; set; }

        public COA()
        {

        }

        public bool GetPageIsValid()
        {
            var _Valid = true;

            if (MyModel.Records is null) { _Valid = false; MyModel.MsgClass.Add("Records not found"); }
            if (MyModel.Record is null) { _Valid = false; MyModel.MsgClass.Add("Account Class List is empty"); }
            if (MyModel.NatureList is null) { _Valid = false; MyModel.MsgClass.Add("Account Nature List is empty"); }
            if (MyModel.NotesList is null) { _Valid = false; MyModel.MsgClass.Add("Financial Notes List is empty"); }
            return _Valid;
        }
        protected void Back() { AppGlobal.NavManager.NavigateTo("/Menu/Accounts"); }
        public async void Add()
        {
            MyModel.Add();
            await AppGlobal.JS.InvokeVoidAsync("showAcordion", "accordionRecordDisplay");

            //Model.Add();
        }
        public async void Edit(int ID)
        {
            MyModel.Edit(ID);
            await AppGlobal.JS.InvokeVoidAsync("showAcordion", "accordionRecordDisplay");
        }

    }
}




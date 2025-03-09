using AppliedAccounts.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;


namespace AppliedAccounts.Pages.Accounts
{


    public partial class COA1
    {
        public COAModel Model { get; set; } = new();
        public bool IsPageValid { get; set; }
        //public string ErrorMessage { get; set; } = string.Empty;
        public COA1() { }

        public bool GetPageIsValid()
        {
            var _Valid = true;
            if (Model.AppUser is null) { _Valid = false; Model.MsgClass.Add("User not define properly."); }
            if (Model.Records is null) { _Valid = false; Model.MsgClass.Add("Records not found"); }
            if (Model.Record is null) { _Valid = false; Model.MsgClass.Add("Account Class List is empty"); }
            if (Model.NatureList is null) { _Valid = false; Model.MsgClass.Add("Account Nature List is empty"); }
            if (Model.NotesList is null) { _Valid = false; Model.MsgClass.Add("Financial Notes List is empty"); }
            return _Valid;
        }

        protected void Back() { NavManager.NavigateTo("/Menu/Accounts"); }

        public async void Add()
        {
            Model.Add();
            await js.InvokeVoidAsync("showAcordion", "accordionRecordDisplay");

            //Model.Add();
        }

        public async void Edit(int ID)
        {
            Model.Edit(ID);
            await js.InvokeVoidAsync("showAcordion", "accordionRecordDisplay");
        }

    }
}





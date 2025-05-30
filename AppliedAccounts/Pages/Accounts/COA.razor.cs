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


        #region DropDown Changed
        private void ClassChanged(int _ID)
        {
            MyModel.Record.Class = _ID;
            MyModel.Record.TitleClass = MyModel.ClassList
                .Where(e => e.ID == MyModel.Record.Class)
                .Select(e => e.Title)
                .First() ?? "";
        }
        private void NatureChanged(int _ID)
        {
            MyModel.Record.Nature = _ID;
            MyModel.Record.TitleNature = MyModel.NatureList
                .Where(e => e.ID == MyModel.Record.Nature)
                .Select(e => e.Title)
                .First() ?? "";
        }
        private void NotesChanged(int _ID)
        {
            MyModel.Record.Notes = _ID;
            MyModel.Record.TitleNote = MyModel.NotesList
                .Where(e => e.ID == MyModel.Record.Notes)
                .Select(e => e.Title)
                .First() ?? "";
        }
        private void SelectedBrowse(int selectedId)
        {
            if (MyModel.BrowseClass.Type == 1) { ClassChanged(selectedId); }
            else if (MyModel.BrowseClass.Type == 2) { NatureChanged(selectedId); }
            else if (MyModel.BrowseClass.Type == 3) { NotesChanged(selectedId); }
        }
        public async void BrowseWindow(int _ListType)
        {
            switch (_ListType)
            {
                case 0:                                         // Nill
                    MyModel.BrowseClass = new();
                    break;

                case 1:
                    MyModel.BrowseClass = new();
                    MyModel.BrowseClass.Type = 1;
                    MyModel.BrowseClass.Heading = "Account Class";
                    MyModel.BrowseClass.Selected = MyModel.Record.Class;
                    MyModel.BrowseClass.BrowseList = MyModel.ClassList;
                    break;


                case 2:
                    MyModel.BrowseClass = new();
                    MyModel.BrowseClass.Type = 2;
                    MyModel.BrowseClass.Heading = "Account Nature";
                    MyModel.BrowseClass.Selected = MyModel.Record.Nature;
                    MyModel.BrowseClass.BrowseList = MyModel.NatureList;
                    break;

                case 3:
                    MyModel.BrowseClass = new();
                    MyModel.BrowseClass.Type = 3;
                    MyModel.BrowseClass.Heading = "Account Notes";
                    MyModel.BrowseClass.Selected = MyModel.Record.Notes;
                    MyModel.BrowseClass.BrowseList = MyModel.NotesList;
                    break;

                default:
                    MyModel.BrowseClass = new();
                    break;
            }

            await InvokeAsync(StateHasChanged);
            await AppGlobal.JS.InvokeVoidAsync("showModol", "winBrowse");

        }

        #endregion
    }
}





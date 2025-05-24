using AppliedAccounts.Models;
using Microsoft.JSInterop;

namespace AppliedAccounts.Pages.Accounts
{


    public partial class COA
    {
        public COAModel MyModel { get; set; } = new();
        public bool IsPageValid { get; set; }
        private bool showPopup = false;
        
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

        protected void Back() { AppGlobals.NavManager.NavigateTo("/Menu/Accounts"); }

        public async void Add()
        {
            MyModel.Add();
            await AppGlobals.JS.InvokeVoidAsync("showAcordion", "accordionRecordDisplay");

            //Model.Add();
        }

        public async void Edit(int ID)
        {
            MyModel.Edit(ID);
            await AppGlobals.JS.InvokeVoidAsync("showAcordion", "accordionRecordDisplay");
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

        private void ShowClassPopup()
        {
            showPopup = true;
            // Optionally: Load AllClasses if not loaded
        }

        private void SelectClass(int selectedId)
        {
            MyModel.Record.Class = selectedId;
            showPopup = false;
        }

       

        public async Task BrowseWindow(string _SelectList)             
        {
            // Browse a windows that display list of Account Class to select any one and assign to dropdown
            await Task.Delay(100);
            // MyBrowseModel.BrowseListName = "COA";
            // await InvokeAsync(StateHasChanged);
            // await Task.Delay(1000);
            // await js.InvokeVoidAsync("showModol", "browseCodeTitle");
        }

        #endregion
    }
}





using AppliedAccounts.Models;
using Microsoft.JSInterop;
using Menus;



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
        protected void Back() { AppGlobal.NavManager.GoTo(MenuID.AccountsDictionery); }
        public async void Add()
        {
            MyModel.Add();
            await AppGlobal.JS.InvokeVoidAsync("showAcordion", "accordionRecordDisplay");

            //Model.Add();
        }
        public async void Edit(long ID)
        {
            MyModel.Edit(ID);
            await AppGlobal.JS.InvokeVoidAsync("showAcordion", "accordionRecordDisplay");
        }

        public async void Save()
        {
            var IsSaved = await Task.Run(MyModel.Save);
            if(IsSaved)
            {
                await InvokeAsync(StateHasChanged);
                MyModel.MsgClass.Success(AppMessages.Enums.Messages.Saved);
                ToastService.ShowSuccess($"Successfully saved {MyModel.Record.Title}");
            }
            else
            {
                ToastService.ShowError($"Failed to save {MyModel.Record.Title}");
            }
        }

        public async void Delete(long ID)
        {
            var IsDeleted = MyModel.Delete(ID);
            if (IsDeleted)
            {
                MyModel.MsgClass.Success(AppMessages.Enums.Messages.Delete);
                ToastService.ShowSuccess(MyModel.MyMessage);
                
                MyModel.LoadData();
                MyModel.GetFirstRecord();
            }
            else
            {
                ToastService.ShowError(MyModel.MyMessage);
            }
        }


        #region DropDown Changes
        public void ClassChanged(long _NewValue)
        {
            MyModel.Record.Class = _NewValue;
            MyModel.Record.TitleClass = MyModel.ClassList.First(e => e.ID == _NewValue).Title ?? "";
        }

        public void NatureChanged(long _NewValue)
        {
            MyModel.Record.Nature = _NewValue;
            MyModel.Record.TitleNature = MyModel.NatureList.First(e => e.ID == _NewValue).Title ?? "";
        }

        public void NotesChanged(long _NewValue)
        {
            MyModel.Record.Notes = _NewValue;
            MyModel.Record.TitleNote = MyModel.NotesList.First(e => e.ID == _NewValue).Title ?? "";
        }
        #endregion


    }
}




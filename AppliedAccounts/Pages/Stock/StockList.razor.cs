using AppliedAccounts.Models;
using AppliedAccounts.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace AppliedAccounts.Pages.Stock
{
    public partial class StockList
    {
        public StockListModel MyModel { get; set; }
        public bool ShowList { get; set; }                  //play List or Edit form
        public bool ShowForm { get; set; }                  //play List or Edit form
       

        public void BackPage()
        {
            ShowList = !ShowList;
            ShowForm = !ShowForm;
        }


        #region Add, Edit, Delete & Save
        public void Add()
        {
            ShowList = false;
            ShowForm = true;
            MyModel.Add();
        }
        public void Edit(int ID)
        {
            ShowList = false;
            ShowForm = true;
            MyModel.Edit(ID);

        }
        public async void Delete(int? ID)
        {
            if (MyModel.Delete())
            {
                ShowList = true;
                ShowForm = false;
                await Toaster.ShowToastAsync(ToastClass.DeleteToast);
            }
        }
        public async void Save()
        {
            if (MyModel.Save())
            {
                ShowList = true;
                ShowForm = false;
                await Toaster.ShowToastAsync(ToastClass.SaveToast);
            }
        }

        #endregion

        #region Dropdown Changed
        public void CategoryChanged(int _ID)
        {
            MyModel.Record.Category = _ID;
            MyModel.Record.TitleCategory = MyModel.Category
                .Where(e => e.ID == MyModel.Record.Category)
                .Select(e => e.Title)
                .First() ?? "";
        }

        public void SubCategoryChanged(int _ID)
        {
            MyModel.Record.SubCategory = _ID;
            MyModel.Record.TitleSubCategory = MyModel.SubCategory
                .Where(e => e.ID == MyModel.Record.SubCategory)
                .Select(e => e.Title)
                .First() ?? "";
        }

        public void PackingChanged(int _ID)
        {
            MyModel.Record.Packing = _ID;
            MyModel.Record.TitlePacking = MyModel.Packing
                .Where(e => e.ID == MyModel.Record.Packing)
                .Select(e => e.Title)
                .First() ?? "";
        }

        public void SizeChanged(int _ID)
        {
            MyModel.Record.Size = _ID;
            MyModel.Record.TitleSize = MyModel.Size
                .Where(e => e.ID == MyModel.Record.Size)
                .Select(e => e.Title)
                .First() ?? "";
        }



        #endregion

        #region Browse Window
        private void SelectedBrowse(int selectedId)
        {
            if (MyModel.BrowseClass.Type == 1) { CategoryChanged(selectedId); }
            else if (MyModel.BrowseClass.Type == 2) { SubCategoryChanged(selectedId); }
            else if (MyModel.BrowseClass.Type == 3) { PackingChanged(selectedId); }
            else if (MyModel.BrowseClass.Type == 4) { SizeChanged(selectedId); }
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
                    MyModel.BrowseClass.Heading = "Category";
                    MyModel.BrowseClass.Selected = MyModel.Record.Category;
                    MyModel.BrowseClass.BrowseList = MyModel.Category;
                    break;


                case 2:
                    MyModel.BrowseClass = new();
                    MyModel.BrowseClass.Type = 2;
                    MyModel.BrowseClass.Heading = "Sub Category";
                    MyModel.BrowseClass.Selected = MyModel.Record.SubCategory;
                    MyModel.BrowseClass.BrowseList = MyModel.SubCategory;
                    break;

                case 3:
                    MyModel.BrowseClass = new();
                    MyModel.BrowseClass.Type = 3;
                    MyModel.BrowseClass.Heading = "Packing";
                    MyModel.BrowseClass.Selected = MyModel.Record.Packing;
                    MyModel.BrowseClass.BrowseList = MyModel.Packing;
                    break;

                case 4:
                    MyModel.BrowseClass = new();
                    MyModel.BrowseClass.Type = 4;
                    MyModel.BrowseClass.Heading = "Size";
                    MyModel.BrowseClass.Selected = MyModel.Record.Size;
                    MyModel.BrowseClass.BrowseList = MyModel.Size;
                    break;

                default:
                    MyModel.BrowseClass = new();
                    break;
            }

            await InvokeAsync(StateHasChanged);
            await AppGlobal.JS.InvokeVoidAsync("showModol", "winBrowse");

        }
        #endregion

        #region Search
        public void Search()
        {

        }
        #endregion

    }
}

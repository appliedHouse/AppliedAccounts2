using AppliedAccounts2.Models;
using AppliedDB;
using System.Data;

namespace AppliedAccounts2.Pages.Accounts
{
    public partial class CashBook
    {

        public BooksModel? Model { get; set; }
        public AppUserModel? Profile { get; set; }
        public string Booktitle { get; set; } = string.Empty;
        public LangPack? LangClass => Model.LangClass;

        #region Constructor
        public CashBook() { }
        #endregion

        #region Submit
        public void Submit() { }
        #endregion

        #region DropDown Change events

        public void CompanyChanged(int _NewValue)
        {
            Model.Record.Company = _NewValue;
            Model.Record.TitleEmployee = Model.Source.GetTitle(Model.Companies, _NewValue);
        }

        public void EmployeeChanged(int _NewValue)
        {
            Model.Record.Employee = _NewValue;
            Model.Record.TitleEmployee = Model.Source.GetTitle(Model.Employees, _NewValue);
        }

        public void ProjectChanged(int _NewValue)
        {
            Model.Record.Project = _NewValue;
            Model.Record.TitleProject = Model.Source.GetTitle(Model.Projects, _NewValue);
        }

        public void AccountChanged(int _NewValue)
        {
            Model.Record.COA = _NewValue;
            Model.Record.TitleAccount = Model.Source.GetTitle(Model.Accounts, _NewValue);
        }

        public void LanguageChanged(int _NewValue)
        {
            Model.LangClass = new((Languages)_NewValue, "Book");
        }

        #endregion
        

        #region New
        public void New()
        {
            if (Model.Record.Status == "New")
            {
                


            }
        }
        #endregion

        #region Delete
        public void Delete()
        {
            
        
        
        }
        #endregion

        #region Save
        public void Save()
        { }
        #endregion

        #region Validate
        private bool Validate(DataRow _Row)
        {

            return true;

        }
        #endregion


        #region Reload after error messages shows
        private void Refresh(Microsoft.AspNetCore.Components.Web.MouseEventArgs e)
        {
            Model.IsModelValid = true;
        }
        #endregion
    }
}

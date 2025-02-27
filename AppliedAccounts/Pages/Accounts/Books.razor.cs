using AppliedAccounts.Models;
using AppliedDB;
using System.Linq.Expressions;

namespace AppliedAccounts.Pages.Accounts
{
    public partial class Books
    {

        public AppUserModel userProfile { get; set; }
        public BookModel MyModel { get; set; } = new();
        public List<CodeTitle> BookList { get; set; }
        public int BookNature { get; set; }



        public Books()
        {
            MyModel = new(0, userProfile ?? new());
        }


        public void Back() { NavManager.NavigateTo("/Menu/Accounts"); }

        public void GetBookList(int _BookNature)
        {
            MyModel.Source = new(userProfile);
            BookList = MyModel.Source.GetBookAccounts(_BookNature);

        }


        #region Debit and Credit Amount format
        //protected string FormatDR
        //{
        //    get => Model.Record.DR.ToString("N2");
        //    set
        //    {
        //        if (decimal.TryParse(value, out var parsedValue))
        //        {
        //            Model.Record.DR = parsedValue; // Parse the value back
        //        }
        //    }
        //}

        //protected string FormatCR
        //{
        //    get => Model.Record.CR.ToString("N2");
        //    set
        //    {
        //        if (decimal.TryParse(value, out var parsedValue))
        //        {
        //            Model.Record.CR = parsedValue; // Parse the value back
        //        }
        //    }
        //}
        #endregion
    }

    public enum ChooseBook
    {
        Cash = 1,
        Bank = 2
    }
}

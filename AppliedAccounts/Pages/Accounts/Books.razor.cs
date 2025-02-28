using AppliedAccounts.Models;
using AppliedDB;
using System.Data;
using System.Linq.Expressions;

namespace AppliedAccounts.Pages.Accounts
{
    public partial class Books
    {

        public AppUserModel UserProfile { get; set; }
        public BookModel MyModel { get; set; } = new();
        public List<CodeTitle> BookList { get; set; } = new();
        public int BookNature { get; set; }
        public DateTime DT_Start { get; set; }
        public DateTime DT_End { get; set; }
        public string SearchText { get; set; }
        
        public BookRec BookRecord {get; set;}
        public List<BookRec> BookRecords {get; set;}

        public Books()
        {
            MyModel = new(0, UserProfile ?? new());
           

        }


        public void Back() { NavManager.NavigateTo("/Menu/Accounts"); }

        public List<CodeTitle> GetBookList(int _BookNature)
        {
            MyModel.Source = new(UserProfile);
            var _BookList = MyModel.Source.GetBookAccounts(_BookNature) ?? new(); 
            return _BookList;

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

        public string GetTitle(List<CodeTitle> _List, int _Value)
        {
            if (_List.Count == 0) { return string.Empty; }
            if (_List is null) { return string.Empty; }
            return _List.Where(x => x.ID == _Value).Select(x => x.Title).First();
        }
    }

    public class BookRec
    {
        public int Vou_No { get; set; }
        public DateTime Vou_Date { get; set; }
        public int Description { get; set; }
        public string Recevied { get; set; }
        public decimal Paid { get; set; }
        public decimal Balance { get; set; }
    }

}

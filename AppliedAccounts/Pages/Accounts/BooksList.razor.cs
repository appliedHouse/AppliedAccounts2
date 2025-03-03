using AppliedAccounts.Models;
using AppliedDB;
using Microsoft.AspNetCore.Components;
using System.Data;

namespace AppliedAccounts.Pages.Accounts
{
    public partial class BooksList
    {

        public AppUserModel UserProfile { get; set; }
        public BookModel MyModel { get; set; } = new();
        public List<CodeTitle> BookList { get; set; } = new();
        public int BookNature { get; set; }
        public DateTime DT_Start { get; set; }
        public DateTime DT_End { get; set; }
        public string SearchText { get; set; }

        public BookView BookRecord { get; set; }
        public List<BookView> BookRecords { get; set; }

        

        public BooksList()
        {
            MyModel = new(0, UserProfile ?? new());


        }

        List<BookView> LoadBookRecords(int _BookID)
        {
            var _List = new List<BookView>();
            var _Data = MyModel.Source.GetBook(_BookID);

            if (_Data != null)
            {
                decimal _Bal = 0.00M, _DR = 0.00M, _CR = 0.00M;
                foreach (DataRow Row in _Data.Rows)
                {
                    _DR = Row.Field<decimal>("DR");
                    _CR = Row.Field<decimal>("CR");
                    _Bal += _CR - _DR;

                    var _Record = new BookView()
                    {
                        ID = Row.Field<int>("ID"),
                        Vou_No = Row.Field<string>("Vou_No") ?? "---",
                        Vou_Date = Row.Field<DateTime>("Vou_No"),
                        Recevied = _CR,
                        Paid = _DR,
                        Balance = _Bal,
                        Description = Row.Field<string>("Description") ?? "",
                        txtRecevied = _CR.ToString("###,###,###.##"),
                        txtPaid = _CR.ToString("###,###,###.##"),
                        txtBalance = _Bal.ToString("###,###,###.##")
                    };

                    _List.Add(_Record);
                }
                return _List;
            }
            return [];
        }

        public void Back() { NavManager.NavigateTo("/Menu/Accounts"); }

        public List<CodeTitle> GetBookList(int _BookNature)
        {
            MyModel.Source = new(UserProfile);
            var _BookList = MyModel.Source.GetBookAccounts(_BookNature) ?? new();
            return _BookList;

        }

        public void New()
        {
            NavManager.NavigateTo($"/Accounts/Books/{0}");
        }


        public void Refresh()
        {

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


    public class BookView
    {
        public int ID { get; set; }
        public string Vou_No { get; set; }
        public DateTime Vou_Date { get; set; }
        public string Description { get; set; }
        public decimal Recevied { get; set; }
        public decimal Paid { get; set; }
        public decimal Balance { get; set; }
        public string txtRecevied { get; set; }
        public string txtPaid { get; set; }
        public string txtBalance { get; set; }

    }

}

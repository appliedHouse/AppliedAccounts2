using AppliedAccounts.Models;
using AppliedAccounts.Services;
using AppliedDB;
using System.Data;

namespace AppliedAccounts.Pages.Accounts
{
    public partial class BooksList
    {

        public AppUserModel UserProfile { get; set; }
        public BookListModel MyModel { get; set; } = new();
        public BookView BookRecord { get; set; }
        public List<BookView> BookRecords { get; set; }

        public BooksList()
        {
            //MyModel = new(0, UserProfile ?? new());
           
        }

        List<BookView> LoadBookRecords(int _BookID)     // Load List of Cash / Bank Book record in Table
        {
            var _List = new List<BookView>();
            var _Data = MyModel.Source.GetBookList(_BookID);

            if (_Data != null)
            {
                decimal _Bal = 0.00M;
                decimal _DR = 0.00M;
                decimal _CR = 0.00M;

                foreach (DataRow Row in _Data.Rows)
                {
                    _DR = Row.Field<decimal>("DR");
                    _CR = Row.Field<decimal>("CR");
                    _Bal += _CR - _DR;

                    var _Record = new BookView()
                    {
                        ID = Row.Field<int>("ID1"),
                        Vou_No = Row.Field<string>("Vou_No") ?? "---",
                        Vou_Date = Row.Field<DateTime>("Vou_Date"),
                        Recevied = _CR,
                        Paid = _DR,
                        Balance = _Bal,
                        Description = Row.Field<string>("Description") ?? "",
                     
                        TReceived = _CR.ToString(Format.Digit),
                        TPaid = _DR.ToString(Format.Digit),
                        TBalance = _Bal.ToString(Format.Digit)
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
            //MyModel.Source = new(UserProfile);
            var _BookList = MyModel.Source.GetBookAccounts(_BookNature) ?? new();
            return _BookList;
        }

        public void New()
        {
            if (MyModel.BookID > 0)
            {
                NavManager.NavigateTo($"/Accounts/Books/{MyModel.SelectedVoucherID}/{MyModel.BookID}");
            }
        }

        public void Refresh()
        {
            MyModel.SetKeys();
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
        public string TReceived { get; set; }
        public string TPaid { get; set; }
        public string TBalance { get; set; }

    }

}

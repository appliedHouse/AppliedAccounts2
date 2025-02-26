
namespace AppliedAccounts.Pages.Accounts
{
    public partial class CashBankBook 
    {

        public CashBankBook()
        { }


        public void Save(string Vou_No) { }
        public void Add(int ID) { }
        public void Edit(int ID) { }
        public void Delete(int ID) { }
        public void Print(int ID) { }
        public void Post(int ID) { }
        public void Back() { NavManager.NavigateTo("/Menu/Accounts"); }

        protected void CRFocusOut()
        {
            if (Model.Record.CR > 0)
            {
                Model.Record.DR = 0.00M;
            }

        }
        protected void DRFocusOut()
        {
            if (Model.Record.DR > 0)
            {
                Model.Record.CR = 0.00M;
            }

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
}

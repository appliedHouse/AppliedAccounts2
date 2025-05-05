using AppliedAccounts.Data;
using AppliedAccounts.Models;
using AppliedAccounts.Models.Interface;
using AppliedDB;
using AppReports;
using Microsoft.AspNetCore.Components;
using System.Data;


namespace AppliedAccounts.Pages.Accounts
{
    public partial class BooksList 
    {

        public AppUserModel UserProfile { get; set; }
        public BookListModel MyModel { get; set; } = new();
        public NavigationManager NavManager { get; set; }

        public BooksList() { }

        public void Back() { AppGlobals.NavManager.NavigateTo("/Menu/Accounts"); }

        public void New()
        {
            if (MyModel.BookID > 0)
            {
                NavManager.NavigateTo($"/Accounts/Books/{MyModel.VoucherID}/{MyModel.BookID}");
            }
        }

        public void Refresh()
        {
            MyModel.SetKeys();
        }


        public string GetTitle(List<CodeTitle> _List, int _Value)
        {
            if (_List.Count == 0) { return string.Empty; }
            if (_List is null) { return string.Empty; }
            return _List.Where(x => x.ID == _Value).Select(x => x.Title).First();
        }

        public void Print(ReportActionClass reportAction)
        {
            try
            {
                MyModel.ReportService = new();
                MyModel.VoucherID = reportAction.VoucherID;
                MyModel.ReportService.RptData = MyModel.GetReportData();
                MyModel.ReportService.RptModel = MyModel.CreateReportModel();
                MyModel.ReportService.Print(reportAction.PrintType);
            }
            catch (Exception)
            {
                MyModel.MsgClass.Add(AppMessages.Enums.Messages.prtReportError);

            }

            
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

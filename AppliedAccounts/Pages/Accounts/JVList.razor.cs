using AppliedAccounts.Data;
using AppliedDB;
using AppliedGlobals;
using AppMessages;
using AppReports;
using System.Data;
using System.Text;

namespace AppliedAccounts.Pages.Accounts
{
    public partial class JVList
    {
        public DataSource Source { get; set; }
        public JVListViewModel MyModel { get; set; } = new();
        public List<JVListDataModel> JVItems { get; set; } = new();
        public MessageClass MsgClass { get; set; } = new();
        public PageModel Pages { get; set; } = new();

        public JVList()
        {

        }

        #region Load Data
        public void LoadData()
        {
            try
            {
                Source ??= new DataSource(AppGlobal.AppPaths);
                string _Query = SQLQueries.Quries.JournalVoucherList(GetFilter());
                DataTable _Table = Source.GetTable(_Query);
                if (_Table.Rows.Count > 0)
                {
                    JVItems = [.. (from DataRow _Row in _Table.Rows
                           select new JVListDataModel()
                           {
                               Vou_No = _Row.Field<string>("Vou_No") ?? "",
                               Vou_Date = _Row.Field<DateTime?>("Vou_Date") ?? DateTime.MinValue,
                               Vou_Type = _Row.Field<string>("Vou_Type") ?? "",
                               Ref_No = _Row.Field<string>("Ref_No") ?? "",
                               DR = _Row.Field<decimal?>("DR") ?? 0,
                               CR = _Row.Field<decimal?>("CR") ?? 0,
                               Description = _Row.Field<string>("Description") ?? "",
                               Comments = _Row.Field<string>("Comments") ?? "",
                               Status = _Row.Field<string>("Status") ?? "",
                           })];
                }
            }
            catch (Exception error)
            {
                MsgClass.Error(error.Message);
            }
        }

        private string GetFilter()
        {
            StringBuilder _Text = new();
            _Text.AppendLine($"Vou_Type='{VoucherTypeClass.VoucherType.JV}' AND ");
            _Text.AppendLine($"Vou_Date >= '{MyModel.Date1:yyyy-MM-dd}' AND");
            _Text.AppendLine($"Vou_Date <= '{MyModel.Date2:yyyy-MM-dd}'");

            if (MyModel.IsSearch)
            {
                _Text.AppendLine($" AND (Vou_No LIKE '%{MyModel.SearchText}%' OR Description LIKE '%{MyModel.SearchText}%') ");
            }
            _Text.AppendLine($" ORDER BY [Vou_Date], [Vou_No] ");
            _Text.AppendLine($"LIMIT {Pages.Size} OFFSET {(Pages.Current - 1) * Pages.Size}");

            return _Text.ToString();
        }

        #endregion

        #region Refresh Page
        public async void Refresh()
        {
            SetKeys();                           // Save the current page setting in Registry 
            Pages = new();                       // Reset the page model
            await InvokeAsync(StateHasChanged);
        }
        #endregion

        #region New Voucher
        public void New()
        {
            AppGlobal.NavManager.NavigateTo($"/Accounts/JV");
        }
        #endregion


        #region Print
        public async Task Print(ReportActionClass reportAction)
        {
            MyModel.IsWaiting = true;
            await InvokeAsync(StateHasChanged);
            await Task.Delay(100);                  // Delay for show the message and 

            try
            {
                //MyModel.VoucherID = reportAction.VoucherID;
                //await Task.Run(() => { MyModel.Print(reportAction.PrintType); });
            }
            catch (Exception)
            {
                //MyModel.MsgClass.Add(AppMessages.Enums.Messages.prtReportError);
            }

            MyModel.IsWaiting = false;
            await InvokeAsync(StateHasChanged);
            await Task.Delay(100);                  // Delay for show the message and 
        }
        #endregion

        #region Keys
        public void SetKeys()
        {
            Source.SetKey("JVList", MyModel.Date1, AppErums.KeyTypes.From, "JV List View Model");
            Source.SetKey("JVList", MyModel.Date2, AppErums.KeyTypes.To, "JV List View Model");
            Source.SetKey("JVList", MyModel.SearchText, AppErums.KeyTypes.Text, "JV List View Model");
        }

        public void GetKeys()
        {
            MyModel.Date1 = Source.GetFrom("JVList");
            MyModel.Date2 = Source.GetTo("JVList");
            MyModel.SearchText = Source.GetText("JVList");
        }
        #endregion
    }

    public class JVListViewModel
    {
        public DateTime Date1 { get; set; }
        public DateTime Date2 { get; set; }
        public string SearchText { get; set; } = string.Empty;
        public bool IsWaiting { get; set; }

        public bool IsSearch => SearchText.Length > 0;

    }

    public class JVListDataModel
    {
        public long ID { get; set; }
        public string Vou_No { get; set; }
        public DateTime Vou_Date { get; set; }
        public string Vou_Type { get; set; }
        public int Sr_No { get; set; }
        public string Ref_No { get; set; }
        public long COA { get; set; }
        public decimal DR { get; set; }
        public decimal CR { get; set; }
        public long Company { get; set; }
        public long Employee { get; set; }
        public long Project { get; set; }
        public string Description { get; set; }
        public string Comments { get; set; }
        public string Status { get; set; }

        public string TitleAccount { get; set; }
        public string TitleCompany { get; set; }
        public string TitleEmployee { get; set; }
        public string TitleProject { get; set; }

    }

}

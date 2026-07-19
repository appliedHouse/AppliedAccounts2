using AppliedAccounts.Data;
using AppliedAccounts.Pages.Accounts.Reports;
using AppliedAccounts.Services;
using AppliedDB;
using AppliedGlobals;
using AppMessages;
using AppReports;
using System.Data;

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
            // subscribe to page change events so UI refreshes when user navigates pages
            Pages.PageChanged += OnPageChanged;
        }
        private void OnPageChanged(int newPage)
        {
            LoadData();
            InvokeAsync(StateHasChanged);
        }

        #region Load Data
        public void LoadData()
        {
            try
            {
                Source ??= new(AppGlobal.AppPaths);
                DataTable _Table = Source.GetTable(GetFilter());

                if (_Table != null && _Table.Rows.Count > 0)
                {
                    JVItems = [.. (from DataRow _Row in _Table.Rows
                           select new JVListDataModel()
                           {
                               ID = _Row.Field<long>("ID"),
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
            if (MyModel.Date1 > MyModel.Date2)
            {
                // Correct Date Order if it is dis-order.
                (MyModel.Date2, MyModel.Date1) = (MyModel.Date1, MyModel.Date2);
            }


            // Build base filter (without ORDER BY / LIMIT)
            var baseFilter = new System.Text.StringBuilder();
            baseFilter.AppendLine($"Vou_Type='{VoucherTypeClass.VoucherType.JV}' AND ");
            baseFilter.AppendLine($"Vou_Date >= '{MyModel.Date1.QueryDate()}' AND");
            baseFilter.AppendLine($"Vou_Date <= '{MyModel.Date2.QueryDate()}'");
            if (MyModel.IsSearch)
            {
                baseFilter.AppendLine($" AND (Vou_No LIKE '%{MyModel.SearchText}%' OR Description LIKE '%{MyModel.SearchText}%') ");
            }

            // Get full (unpaged) set to calculate total records for paging
            string fullQuery = SQLQueries.Quries.JournalVoucherList(baseFilter.ToString());
            var fullTable = Source.GetTable(fullQuery);
            int totalRecords = fullTable?.Rows.Count ?? 0;

            // Refresh paging model
            Pages.Refresh(totalRecords);

            // Build paged query by appending ORDER BY and LIMIT/OFFSET after the base query
            string pagedQuery = fullQuery + $" ORDER BY [Vou_Date], [Vou_No] LIMIT {Pages.Size} OFFSET {(Pages.Current - 1) * Pages.Size}";

            return pagedQuery;
        }

        #endregion

        #region Refresh Page
        public async Task Refresh()
        {
            SetKeys();                           // Save the current page setting in Registry 
            LoadData();
            await InvokeAsync(StateHasChanged);
        }

        public async Task Clear()
        {
            MyModel.SearchText = string.Empty;
            await Refresh();
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

            try
            {
                MyModel.VoucherNo = (string)Source.SeekValue(AppliedDB.Enums.Tables.Ledger, reportAction.VoucherID, "Vou_No")!;
                if (!string.IsNullOrEmpty(MyModel.VoucherNo))
                {
                    VoucherPrint PrintClass = new(AppGlobal, reportAction.PrintType, MyModel.VoucherNo);
                    if (!PrintClass.IsError)
                    {
                        await PrintClass.Print();
                    }
                }
                else
                {
                    MsgClass.Warning(AppMessages.Enums.Messages.VoucherNotFound);
                }


            }
            catch (Exception)
            {
                MsgClass.Add(AppMessages.Enums.Messages.prtReportError);
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
        public long VoucherID { get; internal set; }
        public string VoucherNo { get; internal set; }
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

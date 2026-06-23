using AppliedAccounts.Data;
using AppliedDB;
using AppMessages;
using AppReports;
using Microsoft.AspNetCore.Components.Forms;
using System.Data;
using System.Net;
using static AppliedGlobals.AppErums;
using static AppliedGlobals.AppValues;


namespace AppliedAccounts.Pages.Accounts.Reports
{
    public partial class TrialBalance
    {
        public string DBFile { get; set; }
        public TBModel MyModel { get; set; }
        public List<TBListModel> AccList { get; set; }
        public DataSource Source { get; set; }
        private EditContext? EditContext;
        bool IsPageValidate = false;
        bool IsPrinting = false;

        public MessageClass MsgClass { get; set; } = new MessageClass();

        #region Dictionaries for Dropdowns
        public Dictionary<int, string> OptionTypes { get; set; } = new Dictionary<int, string>
        {
            { 1, "All" },
            { 2, "Upto Date" },
            { 3, "Opening" },
        };
        public Dictionary<int, string> TB_Options { get; set; } = new Dictionary<int, string>
        {
            {1, "Trial Balance" },
            {2, "Project Balances" },
            {3, "Compnay Balances" },
            {4, "Employee Balances" }
        };
        #endregion

        public TrialBalance()
        {

        }

        protected override async Task OnInitializedAsync()
        {
            MyModel = new();
            EditContext = new EditContext(MyModel);
            EditContext.OnFieldChanged += async (sender, e) => await DisplayList();
            Source = new(AppGlobal.AppPaths);
            DBFile = AppGlobal.DBFile;

            MyModel.Companies = Source.GetCustomers();
            MyModel.Employees = Source.GetEmployees();
            MyModel.Projects = Source.GetProjects();

            _ = GetKeys();
            await DisplayList();
        }

        #region Display List

        private async Task DisplayList()
        {
            try
            {
                DataTable reportTable = MyModel.TB_Type switch
                {
                    1 => await TB_All(),
                    2 => await TB_DatesAsync(MyModel.TB_From, MyModel.TB_To),
                    3 => await TBOB_DataAsync(),
                    _ => await TB_All()
                };

                if (reportTable == null || reportTable.Rows.Count == 0)
                {
                    AccList = new List<TBListModel>();
                    return;
                }

                AccList = reportTable.AsEnumerable()
                    .Select(row => new TBListModel
                    {
                        ID = row.Field<long>("ID"),
                        Code = row.Field<string>("Code") ?? "",
                        Title = row.Field<string>("Title") ?? "",
                        DR = row.Field<decimal>("DR"),
                        CR = row.Field<decimal>("CR")
                    })
                    .ToList();


                if (AccList.Count > 0)
                {
                    IsPageValidate = true;
                }
                else
                {
                    IsPageValidate = false;
                }

            }
            catch (Exception error)
            {
                MsgClass.Error(error.Message);
            }
            await InvokeAsync(StateHasChanged);
        }
        #endregion

        #region Print Trial Balance
        public async Task Print(ReportActionClass PrintAction)
        {
            MsgClass = new();           // Clear all previous messages - refresh
            IsPrinting = true;

            await InvokeAsync(StateHasChanged);
            await Task.Delay(100);

            try
            {
                _ = SetKeys();
                ReportService = new(AppGlobal); ;
                ReportService.ReportType = PrintAction.PrintType;
                await CreateReportModel();
                if (!ReportService.IsError)
                {
                    ReportService.Print();
                    MsgClass = ReportService.MsgClass;
                }

            }
            catch (Exception error)
            {
                MsgClass.Error(error.Message);
            }

            IsPrinting = false;
            await InvokeAsync(StateHasChanged);
        }

        public async Task<ReportData> GetReportData()
        {
            var _Result = new ReportData();

            DataTable reportTable = MyModel.TB_Option switch
            {
                1 => await TB_All(),
                2 => await TB_DatesAsync(MyModel.TB_From, MyModel.TB_To),
                3 => await TBOB_DataAsync(),
                _ => new DataTable()
            };

            _Result.DataSetName = "dset_TB";
            _Result.ReportTable = reportTable;


            ReportService.Data = _Result;
            return _Result;
        }

        public async Task CreateReportModel()
        {

            var _Heading1 = $"Trial Balance ";
            var _Heading2 = $"[{MyModel.TB_From.ToString(Format.DDMMMYY)}] to [{MyModel.TB_To.ToString(Format.DDMMMYY)}] ";
            var _CompanyName = ReportService.Config.Client.Company.Replace(" ", "_");

            ReportService.Model.InputReport.FileName = "TB.rdl";
            ReportService.Model.ReportDataSource = await GetReportData();                   // Load Reporting Data to Report Model
            ReportService.Model.OutputReport.FileName = "TB_" + _CompanyName;
            ReportService.Model.OutputReport.ReportType = ReportService.ReportType;
            ReportService.Model.AddReportParameter("Heading1", _Heading1);
            ReportService.Model.AddReportParameter("Heading2", _Heading2);

        }
        #endregion

        #region Get Data as per report option 

        public async Task<DataTable> TB_All()
        {
            string _Filter = string.Empty;
            string _Query = string.Empty;
            string _OrderBy = "Code";

            if (MyModel.TB_Type == 1)                // All Records
            {
                _Filter = string.Empty;
            }
            else if (MyModel.TB_Type == 2)           // From Date to Date
            {
                _Filter = $"Date(Vou_Date) >= Date({MyModel.TB_From.QueryDate()}) AND Date(Vou_Date) <= Date({MyModel.TB_To.QueryDate()})";
            }
            else if (MyModel.TB_Type == 3)           // Opening Balances
            {
                _Filter = $"[Vou_Type] = 'OpeningBalance'";
            }

            DataTable _Table;

            if (MyModel.TB_Option == 1)
            {
                _Query = SQLQueries.Quries.TrialBalance(_Filter, _OrderBy);
            }
            else if (MyModel.TB_Option == 2)
            {
                _Query = SQLQueries.Quries.TBProject(0, _Filter, _OrderBy);
            }

            else if (MyModel.TB_Option == 3)
            {

            }

            else if (MyModel.TB_Option == 4)
            {

            }



            _Table = await Source.GetTableAsync(_Query);
            return _Table;
        }

        public async Task<DataTable> TBOB_DataAsync()
        {
            DataTable _Table;
            DateTime OBalDate = AppRegistry.GetDate(DBFile, "OBDate");
            var _Date = AppRegistry.YMD(OBalDate);
            var _Filter = $"Date([Ledger].[Vou_Date]) = Date('{_Date}')";
            var _OrderBy = "Code";
            var _Query = SQLQueries.Quries.TrialBalance(_Filter, _OrderBy);
            _Table = await Source.GetTableAsync(_Query);
            return _Table;
        }

        public async Task<DataTable> TB_DatesAsync(DateTime Date1, DateTime Date2)
        {
            DataTable _Table;
            var _Date1 = AppRegistry.YMD(Date1);
            var _Date2 = AppRegistry.YMD(Date2);

            var _Filter = $"Date(Vou_Date) >= '{_Date1}' AND Date(Vou_Date) <= '{_Date2}'";
            var _OrderBy = "Code";
            var _Query = SQLQueries.Quries.TrialBalance(_Filter, _OrderBy);
            _Table = await Source.GetTableAsync(_Query);
            return _Table;
        }

        #endregion

        #region Set and Get Keys
        public async Task SetKeys()
        {
            Source.SetKey("TB", MyModel.TB_From, KeyTypes.From, "Trial Balance");
            Source.SetKey("TB", MyModel.TB_To, KeyTypes.To);
            Source.SetKey("TB_Option", MyModel.TB_Option, KeyTypes.Number);
            Source.SetKey("TB_Type", MyModel.TB_Type, KeyTypes.Number);
        }

        public async Task GetKeys()
        {
            MyModel.TB_From = Source.GetFrom("TB");
            MyModel.TB_To = Source.GetTo("TB");
            MyModel.TB_Option = Source.GetNumber("TB_Option");
            MyModel.TB_Type = Source.GetNumber("TB_Type");
        }
        #endregion


        #region Print Ledger
        public void PrintLedger()
        {
            MsgClass.Add("Printing of Ledger is being generated...");
        }
        #endregion

        #region Drop Down Value changed events



        private async Task CompanyIDChanged(long _ID)
        {
            MyModel.CompanyID = _ID;
            MyModel.TitleCompany = MyModel.Companies
                .Where(e => e.ID == MyModel.CompanyID)
                .Select(e => e.Title)
                .First() ?? "";

            await DisplayList();
        }
        private async Task ProjectIDChanged(long _ID)
        {
            MyModel.ProjectID = _ID;
            MyModel.TitleProject = MyModel.Projects
                .Where(e => e.ID == MyModel.ProjectID)
                .Select(e => e.Title)
                .First() ?? "";

            await DisplayList();
        }
        private async Task EmployeeIDChanged(long _ID)
        {
            MyModel.EmployeeID = _ID;
            MyModel.TitleEmployee = MyModel.Employees
                .Where(e => e.ID == MyModel.EmployeeID)
                .Select(e => e.Title)
                .First() ?? "";
            await DisplayList();
        }
        #endregion
    }
}
public class TBModel
{

    public DateTime TB_From { get; set; }
    public DateTime TB_To { get; set; }
    public int TB_Option { get; set; }
    public int TB_Type { get; set; }
    public ReportActionClass MyReportClass { get; set; }

    public long ProjectID { get; set; }
    public long CompanyID { get; set; }
    public long EmployeeID { get; set; }

    public List<CodeTitle> Companies { get; set; } = [];
    public List<CodeTitle> Employees { get; set; } = [];
    public List<CodeTitle> Projects { get; set; } = [];

    public string TitleProject { get; set; }
    public string TitleCompany { get; set; }
    public string TitleEmployee { get; set; }

}

public class TBListModel
{
    public long ID { get; set; }
    public string Code { get; set; }
    public string Title { get; set; }
    public decimal DR { get; set; }
    public decimal CR { get; set; }
    public decimal Amount => DR - CR;

}






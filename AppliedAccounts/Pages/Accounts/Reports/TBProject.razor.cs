using AppliedAccounts.Data;
using AppliedDB;
using AppMessages;
using AppReports;
using System.Data;
using static AppliedGlobals.AppErums;

namespace AppliedAccounts.Pages.Accounts.Reports
{
    public partial class TBProject
    {
        public TBProjectModel MyModel { get; set; }
        public DataSource Source { get; set; }
        public MessageClass MsgClass { get; set; } = new MessageClass();

        bool IsPrinting = false;



        #region Print
        #region Print Trial Balance
        public async void Print(ReportActionClass PrintAction)
        {
            MsgClass = new();           // Clear all previous messages - refresh
            IsPrinting = true;
            await InvokeAsync(StateHasChanged); await Task.Delay(100);

            try
            {
                SetKeys();
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
        #endregion
        #endregion

        public async Task CreateReportModel()
        {
            var _Heading1 = $"Trial Balance (Project) ";
            var _Heading2 = $"[{MyModel.DateFrom.QueryDate()} to [{MyModel.DateTo.QueryDate()}] ";
            var _CompanyName = ReportService.Config.Client.Company.Replace(" ", "_");

            ReportService.Model.InputReport.FileName = "TB.rdl";
            ReportService.Model.ReportDataSource = await GetReportData();                   // Load Reporting Data to Report Model
            ReportService.Model.OutputReport.FileName = "TB_" + _CompanyName;
            ReportService.Model.OutputReport.ReportType = ReportService.ReportType;
            ReportService.Model.AddReportParameter("Heading1", _Heading1);
            ReportService.Model.AddReportParameter("Heading2", _Heading2);

        }

        public async Task<ReportData> GetReportData()
        {
            var _Result = new ReportData();

            DataTable _ReportTable = MyModel.TB_Option
            switch
            {
                1 => await TB_All(),
                2 => TB_Dates(MyModel.DateFrom, MyModel.DateTo),
                3 => TBOB_Data(),
                _ => new()
            };

            _Result.DataSetName = "dset_TB";
            _Result.ReportTable = _ReportTable;

            ReportService.Data = _Result;
            return _Result;
        }

        #region Get Data as per report option 

        public async Task<DataTable> TB_All()
        {
            DataTable _Table;
            var _OrderBy = "Code";
            var _Query = SQLQueries.Quries.TBProject(MyModel.Project,string.Empty, _OrderBy);
            _Table = await Source.GetTableAsync(_Query);
            return _Table;
        }

        public DataTable TBOB_Data()
        {
            DataTable _Table;
            DateTime OBalDate = Source.GetDate("OBDate");
            var _Filter = $"Date([Ledger].[Vou_Date]) = Date('{OBalDate.QueryDate()}')";
            var _OrderBy = "Code";
            var _Query = SQLQueries.Quries.TBProject(MyModel.Project, _Filter, _OrderBy);
            _Table = Source.GetTable(_Query);
            return _Table;
        }

        public DataTable TB_Dates(DateTime Date1, DateTime Date2)
        {
            DataTable _Table;

            var _Filter = $"Date(Vou_Date) >= '{Date1.QueryDate()}' AND Date(Vou_Date) <= '{Date2.QueryDate()}'";
            var _OrderBy = "Code";
            var _Query = SQLQueries.Quries.TBProject(MyModel.Project, _Filter, _OrderBy);
            _Table = Source.GetTable(_Query);
            return _Table;
        }

        #endregion

        #region Set and Get Keys
        public void SetKeys()
        {
            Source.SetKey("TBP_Account", MyModel.Project,KeyTypes.Number, "Trial Balance Project");
            Source.SetKey("TBP_Project", MyModel.Account,KeyTypes.Number);
            Source.SetKey("TBP_DtFrom", MyModel.DateFrom,KeyTypes.Date);
            Source.SetKey("TBP_DtFrom", MyModel.DateTo,KeyTypes.Date);
        }

        public void GetKeys()
        {
            Source.GetKey("TBP_Account", KeyTypes.Number);
            Source.GetKey("TBP_Project", KeyTypes.Number);
            Source.GetKey("TBP_DtFrom", KeyTypes.Date);
            Source.GetKey("TBP_DtFrom", KeyTypes.Date);
        }
        #endregion

        #region Dropdown value changed events
        private void ProjectIDChanged(long _ID)
        {
            MyModel.Project = _ID;
            MyModel.TitleProject = MyModel.Projects
                .Where(e => e.ID == MyModel.Project)
                .Select(e => e.Title)
                .First() ?? "";

        }

        private void AccountIDChanged(long _ID)
        {
            MyModel.Account = _ID;
            MyModel.TitleAccount = MyModel.Accounts
                .Where(e => e.ID == MyModel.Account)
                .Select(e => e.Title)
                .First() ?? "";
        }
        #endregion
    }

    public class TBProjectModel
    {
        public long Project { get; set; }
        public long Account { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public int TB_Option { get; set; }
        public ReportActionClass MyReportClass { get; set; }
        public Dictionary<int, string> OptionTypes { get; set; } = new Dictionary<int, string>
        {
            { 1, "All" },
            { 2, "Upto Date" },
            { 3, "Opening" },
        };

        public List<CodeTitle> Projects { get; set; } = [];
        public List<CodeTitle> Accounts { get; set; } = [];

        public string TitleProject { get; set; } = "";
        public string TitleAccount { get; set; } = "";

        public int SelectedOptionType { get; set; } = 1; // For binding


    }
}

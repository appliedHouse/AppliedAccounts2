using AppliedAccounts.Data;
using AppliedDB;
using AppMessages;
using AppReports;
using System.Data;
using static AppliedGlobals.AppValues;
using static AppliedGlobals.AppErums;


namespace AppliedAccounts.Pages.Accounts.Reports
{
    public partial class TrialBalance
    {
        public string DBFile { get; set; }
        public TBModel MyModel { get; set; }
        public DataSource Source { get; set; }

        public MessageClass MsgClass { get; set; } = new MessageClass();

        public TrialBalance()
        {

        }

        protected override void OnInitialized()
        {
            Source = new(AppGlobal.AppPaths);
            DBFile = AppGlobal.DBFile;

            MyModel = new();
            GetKeys();
        }

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

        public async Task<ReportData> GetReportData()
        {
            var _Result = new ReportData();

            DataTable _ReportTable = MyModel.TB_Option
            switch
            {
                1 => await TB_All(),
                2 => TB_Dates(MyModel.TB_From, MyModel.TB_To),
                3 => TBOB_Data(),
                _ => new()
            };

            _Result.DataSetName = "dset_TB";
            _Result.ReportTable = _ReportTable;

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
            DataTable _Table;
            var _Filter = string.Empty;
            var _OrderBy = "Code";
            var _Query = SQLQueries.Quries.TrialBalance(_Filter, _OrderBy);
            _Table = await Source.GetTableAsync(_Query);
            return _Table;
        }

        public DataTable TBOB_Data()
        {
            DataTable _Table;
            DateTime OBalDate = AppRegistry.GetDate(DBFile, "OBDate");
            var _Date = AppRegistry.YMD(OBalDate);
            var _Filter = $"Date([Ledger].[Vou_Date]) = Date('{_Date}')";
            var _OrderBy = "Code";
            var _Query = SQLQueries.Quries.TrialBalance(_Filter, _OrderBy);
            _Table = Source.GetTable(_Query);
            return _Table;
        }

        public DataTable TB_Dates(DateTime Date1, DateTime Date2)
        {
            DataTable _Table;
            var _Date1 = AppRegistry.YMD(Date1);
            var _Date2 = AppRegistry.YMD(Date2);

            var _Filter = $"Date(Vou_Date) >= '{_Date1}' AND Date(Vou_Date) <= '{_Date2}'";
            var _OrderBy = "Code";
            var _Query = SQLQueries.Quries.TrialBalance(_Filter, _OrderBy);
            _Table = Source.GetTable(_Query);
            return _Table;
        }

        #endregion


        #region Set and Get Keys
        public void SetKeys()
        {
            AppRegistry.SetKey(DBFile, "TrialBalance", MyModel.TB_From, KeyTypes.From, "Trial Balance");
            AppRegistry.SetKey(DBFile, "TrialBalance", MyModel.TB_To, KeyTypes.To);
            AppRegistry.SetKey(DBFile, "TrialBalance", MyModel.TB_Option, KeyTypes.Number);
        }

        public void GetKeys()
        {
            MyModel.TB_From = AppRegistry.GetFrom(DBFile, "TrialBalance");
            MyModel.TB_To = AppRegistry.GetTo(DBFile, "TrialBalance");
            MyModel.TB_Option = AppRegistry.GetNumber(DBFile, "TrialBalance");
        }
        #endregion
    }

    public class TBModel
    {

        public DateTime TB_From { get; set; }
        public DateTime TB_To { get; set; }
        public int TB_Option { get; set; }
        public Dictionary<int, string> OptionTypes { get; set; } = new Dictionary<int, string>
        {
            { 1, "All" },
            { 2, "Upto Date" },
            { 3, "Opening" },
        };

        public ReportActionClass MyReportClass { get; set; }


    }
}

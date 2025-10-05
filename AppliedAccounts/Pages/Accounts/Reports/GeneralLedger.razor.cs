using AppliedAccounts.Data;
using AppliedAccounts.Models;
using AppliedDB;
using AppMessages;
using AppReports;
using Microsoft.AspNetCore.Components;
using System.Data;
using System.Security.Cryptography.X509Certificates;
using Format = AppliedGlobals.AppValues.Format;
using KeyType = AppliedGlobals.AppErums.KeyTypes;
using MESSAGES = AppMessages.Enums.Messages;

namespace AppliedAccounts.Pages.Accounts.Reports
{
    public partial class GeneralLedger
    {
        public DataSource Source { get; set; }
        public GLModel MyModel { get; set; }
        public MessageClass MsgClass { get; set; }
        public string DBFile { get; set; }
        public bool IsPageValid { get; set; }
        public bool IsPrinting { get; set; }
        string IsPageValidMessage { get; set; } = "Page has some error. Consult to Administrator";
        NavigationManager NavManager => AppGlobal.NavManager;

        public GeneralLedger()
        {
            MyModel = new();
        }

        public void Start(AppliedGlobals.AppUserModel _UserModel)
        {
            if (_UserModel != null)
            {
                MsgClass = new();
                Source = new(AppGlobal.AppPaths);
                DBFile = AppGlobal.DBFile;
                MyModel.AccountList = Source.GetAccounts();
                MyModel.CompanyList = Source.GetCustomers();
                MyModel.EmployeeList = Source.GetEmployees();
                MyModel.ProjectList = Source.GetProjects();

            }
            else
            {
                IsPageValid = false;
                MsgClass.Add(MESSAGES.PageIsNotValid);
            }

            GetKeys();
        }

        #region Get & Set Registry Keys 
        private void GetKeys()
        {
            MyModel.COAID = AppRegistry.GetNumber(DBFile, "GL_COA");
            MyModel.Date_From = AppRegistry.GetFrom(DBFile, "GL_COA");
            MyModel.Date_To = AppRegistry.GetTo(DBFile, "GL_COA");
            MyModel.SortBy = AppRegistry.GetText(DBFile, "GL_COA");

            MyModel.CompanyID = AppRegistry.GetNumber(DBFile, "GL_Company");
            MyModel.DtFrom_Com = AppRegistry.GetFrom(DBFile, "GL_Company");
            MyModel.DtTo_Com = AppRegistry.GetTo(DBFile, "GL_Company");

            MyModel.EmployeeID = AppRegistry.GetNumber(DBFile, "GL_Employee");
            MyModel.DtFrom_Emp = AppRegistry.GetFrom(DBFile, "GL_Employee");
            MyModel.DtTo_Emp = AppRegistry.GetTo(DBFile, "GL_Employee");

            MyModel.ProjectID = AppRegistry.GetNumber(DBFile, "GL_Project");
            MyModel.DtFrom_Prj = AppRegistry.GetFrom(DBFile, "GL_Project");
            MyModel.DtTo_Prj = AppRegistry.GetTo(DBFile, "GL_Project");
        }

        private void SetKeys()
        {
            AppRegistry.SetKey(DBFile, "GL_COA", MyModel.COAID, KeyType.Number, "General Ledger ID,From,To,Sort");
            AppRegistry.SetKey(DBFile, "GL_COA", MyModel.Date_From, KeyType.From);
            AppRegistry.SetKey(DBFile, "GL_COA", MyModel.Date_To, KeyType.To);
            AppRegistry.SetKey(DBFile, "GL_COA", MyModel.SortBy, KeyType.Text);

            AppRegistry.SetKey(DBFile, "GL_Company", MyModel.CompanyID, KeyType.Number, "Company Ledger ID,From,To,Sort");
            AppRegistry.SetKey(DBFile, "GL_Company", MyModel.DtFrom_Com, KeyType.From);
            AppRegistry.SetKey(DBFile, "GL_Company", MyModel.DtTo_Com, KeyType.To);
            AppRegistry.SetKey(DBFile, "GL_Company", MyModel.SortBy, KeyType.Text);

            AppRegistry.SetKey(DBFile, "GL_Employee", MyModel.EmployeeID, KeyType.Number, "Company Ledger ID,From,To,Sort");
            AppRegistry.SetKey(DBFile, "GL_Employee", MyModel.DtFrom_Emp, KeyType.From);
            AppRegistry.SetKey(DBFile, "GL_Employee", MyModel.DtTo_Emp, KeyType.To);
            AppRegistry.SetKey(DBFile, "GL_Employee", MyModel.SortBy, KeyType.Text);

            AppRegistry.SetKey(DBFile, "GL_Project", MyModel.ProjectID, KeyType.Number, "Project Ledger ID,From,To,Sort");
            AppRegistry.SetKey(DBFile, "GL_Project", MyModel.DtFrom_Prj, KeyType.From);
            AppRegistry.SetKey(DBFile, "GL_Project", MyModel.DtTo_Prj, KeyType.To);
            AppRegistry.SetKey(DBFile, "GL_Project", MyModel.SortBy, KeyType.Text);

        }
        #endregion

        #region Refresh and BackPage

        public void Refresh()
        {
            SetKeys();
        }

        public void BackPage()
        {
            NavManager.NavigateTo("/Menu/Accounts");
        }
        #endregion

        #region Print
        public async void Print(ReportActionClass PrintAction)
        {
            MsgClass = new();           // Clear all previous messages - refresh
            IsPrinting = true;
            await InvokeAsync(StateHasChanged);

            try
            {
                SetKeys();
                await Task.Run(() =>
                {
                    ReportService = new(AppGlobal); ;
                    ReportService.ReportType = PrintAction.PrintType;
                    CreateReportModel();
                    if (!ReportService.IsError)
                    {
                        ReportService.Print();
                    }

                });

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

            if (MyModel.COAID == 0)
            {
                ReportService.IsError = true;
                MsgClass.Add(MESSAGES.AccountIDIsZero);
                return _Result;
            }

            var _OBDate = MyModel.Date_From.AddDays(-1).ToString(Format.YMD);
            var _DateFrom = MyModel.Date_From.ToString(Format.YMD);
            var _DateTo = MyModel.Date_To.ToString(Format.YMD);

            var _FilterOB = $"[COA] = {MyModel.COAID} AND Date([Vou_Date]) < Date('{_OBDate}')";
            var _Filter = $"[COA] = {MyModel.COAID} AND (Date([Vou_Date]) BETWEEN Date('{_DateFrom}') AND Date('{_DateTo}'))";
            var _GroupBy = "[COA]";
            var _SortBy = "[Vou_date], [Vou_no]";
            var _Query = SQLQueries.Quries.GeneralLedger(MyModel.COAID, _OBDate, _FilterOB, _GroupBy, _Filter, _SortBy);

            MyModel.PagingQuery.Query = _Query;
            MyModel.PagingQuery.Source = Source;

            MyModel.PagingQuery.Pages = MyModel.PagingQuery.Pages ?? new PageModel();

            DataTable _ReportTable = Source.GetTable(_Query);
            MyModel.PagingQuery.Pages.TotalRecords = _ReportTable.Rows.Count;
            DataTable _DisplayTable = await MyModel.PagingQuery.GetPageData();

            if (_ReportTable.Columns.Count == 0)
            {
                ReportService.IsError = false;
                MsgClass.Add(MESSAGES.NoRecordFound);
            }

            _Result.DataSetName = "dsname_Ledger";
            _Result.ReportTable = _ReportTable;

            ReportService.Data = _Result;

            MyModel.Ledger = _DisplayTable;

            return _Result; 
        }

        public async void CreateReportModel()
        {

            var _Heading1 = $"General Ledger " + Source.SeekTitle(AppliedDB.Enums.Tables.COA, MyModel.COAID);
            var _Heading2 = $"[{MyModel.Date_From.ToString(Format.DDMMMYY)}] to [{MyModel.Date_To.ToString(Format.DDMMMYY)}] ";

            ReportService.Model.InputReport.FileName = "Ledger.rdl";
            ReportService.Model.ReportDataSource = await GetReportData();                   // Load Reporting Data to Report Model
            ReportService.Model.OutputReport.FileName = "Ledger_" + "CompanyName";
            ReportService.Model.OutputReport.ReportType = ReportService.ReportType;
            ReportService.Model.AddReportParameter("Heading1", _Heading1);
            ReportService.Model.AddReportParameter("Heading2", _Heading2);

        }
        #endregion

        #region Print Company
        public async void PrintCompany(ReportActionClass PrintAction)
        {
            MsgClass = new();           // Clear all previous messages - refresh
            IsPrinting = true;
            await InvokeAsync(StateHasChanged);
            
            try
            {
                SetKeys();
                ReportService = new(AppGlobal); ;
                ReportService.ReportType = PrintAction.PrintType;
                ReportService.IsError = await CreateReportModel_Company();
                if (ReportService.IsError)
                {
                    ReportService.Print();
                }

            }
            catch (Exception error)
            {
                MsgClass.Error(error.Message);
            }
            
            IsPrinting = false;
            await InvokeAsync(StateHasChanged);
        }

        private async Task<bool> CreateReportModel_Company()
        {
            var _CompanyName = Source.SeekTitle(AppliedDB.Enums.Tables.Customers, MyModel.CompanyID);
            var _Heading1 = $"Company Ledger " + _CompanyName;
            var _Heading2 = $"[{MyModel.DtFrom_Com.ToString(Format.DDMMMYY)}] to [{MyModel.DtTo_Com.ToString(Format.DDMMMYY)}] ";
            var _ReportName = "CompanyGL2.rdl";
            ReportService.Model.InputReport.FileName = _ReportName;

            if (File.Exists(ReportService.Model.InputReport.FileFullName))
            {
                //ReportService.Model = new();
                ReportService.Model.InputReport.FileName = _ReportName;
                ReportService.Model.OutputReport.FileName = "CompanyGL_" + _CompanyName.Replace(" ", "_");
                ReportService.Model.OutputReport.ReportType = ReportService.ReportType;
                ReportService.Model.AddReportParameter("Heading1", _Heading1);
                ReportService.Model.AddReportParameter("Heading2", _Heading2);
                ReportService.Model.ReportDataSource = await GetReportData_Company();  //ReportService.Data;  // Load Reporting Data to Report Model
                ReportService.IsError = false;
            }
            else
            {
                ReportService.IsError = true;
                MsgClass.Error(MESSAGES.rptRDLCNotExist + " " + ReportService.Model.InputReport.FileFullName);
                return false;
            }
            return true;
        }

        private async Task<ReportData> GetReportData_Company()
        {
            var _Result = new ReportData();

            if (MyModel.CompanyID == 0)
            {
                ReportService.IsError = true;
                MsgClass.Add(MESSAGES.AccountIDIsZero);
                return _Result;
            }

            var _OBDate = MyModel.DtFrom_Com.AddDays(-1).ToString(Format.YMD);
            var _DateFrom = MyModel.DtFrom_Com.ToString(Format.YMD);
            var _DateTo = MyModel.DtTo_Com.ToString(Format.YMD);

            var _Nature = AppRegistry.GetText(AppGlobal.DBFile, "CompanyGLs");
            var _FilterOB = $"[Customer] = {MyModel.CompanyID} AND  [COA] IN ({_Nature}) AND Date([Vou_Date]) < Date('{_DateFrom}')";
            var _Filter = $"[Customer] = {MyModel.CompanyID} AND  [COA] IN ({_Nature}) AND (Date([Vou_Date]) BETWEEN Date('{_DateFrom}') AND Date('{_DateTo}'))";
            var _GroupBy = "[Customer]";
            var _SortBy = "[Vou_date], [Vou_no]";

            var _Query = SQLQueries.Quries.Ledger2(_FilterOB, _Filter, _GroupBy, _OBDate, _SortBy);

            MyModel.PagingQuery.Query = _Query;
            MyModel.PagingQuery.Source = Source;

            MyModel.PagingQuery.Pages = MyModel.PagingQuery.Pages ?? new PageModel();

            DataTable _ReportTable = Source.GetTable(_Query);
            MyModel.PagingQuery.Pages.TotalRecords = _ReportTable.Rows.Count;
            DataTable _DisplayTable = await MyModel.PagingQuery.GetPageData();

            if (_ReportTable.Columns.Count == 0)
            {
                ReportService.IsError = false;
                MsgClass.Add(MESSAGES.NoRecordFound);
            }

            _Result.ReportTable = _ReportTable;
            _Result.DataSetName = "dsname_CompanyGL";

            ReportService.Data.ReportTable = _Result.ReportTable;
            ReportService.Data.DataSetName = _Result.DataSetName;
            MyModel.Ledger = _DisplayTable;

            return _Result;

        }
        #endregion

        #region Print Employee
        public async void PrintEmployee(ReportActionClass PrintAction)
        {
            MsgClass = new();           // Clear all previous messages - refresh
            IsPrinting = true;
            await InvokeAsync(StateHasChanged);
            try
            {
                SetKeys();
                ReportService = new(AppGlobal); ;
                ReportService.ReportType = PrintAction.PrintType;
                ReportService.IsError = await CreateReportModel_Employee();
                if (ReportService.IsError)
                {
                    ReportService.Print();
                }
            }
            catch (Exception error)
            {
                MsgClass.Error(error.Message);
            }
            IsPrinting = false;
            await InvokeAsync(StateHasChanged);
        }

        private async Task<bool> CreateReportModel_Employee()
        {
            var _EmployeeName = Source.SeekTitle(AppliedDB.Enums.Tables.Employees, MyModel.EmployeeID);
            var _Heading1 = $"Employee Ledger " + _EmployeeName;
            var _Heading2 = $"[{MyModel.DtFrom_Emp.ToString(Format.DDMMMYY)}] to [{MyModel.DtTo_Emp.ToString(Format.DDMMMYY)}] ";
            var _ReportName = "EmployeeGL.rdl";
            ReportService.Model.InputReport.FileName = _ReportName;

            if (File.Exists(ReportService.Model.InputReport.FileFullName))
            {
                //ReportService.Model = new();
                ReportService.Model.InputReport.FileName = _ReportName;
                ReportService.Model.OutputReport.FileName = "EmployeeGL_" + _EmployeeName.Replace(" ", "_");
                ReportService.Model.OutputReport.ReportType = ReportService.ReportType;
                ReportService.Model.AddReportParameter("Heading1", _Heading1);
                ReportService.Model.AddReportParameter("Heading2", _Heading2);
                ReportService.Model.ReportDataSource = await GetReportData_Employee();  //ReportService.Data;  // Load Reporting Data to Report Model
                ReportService.IsError = false;
            }
            else
            {
                ReportService.IsError = true;
                MsgClass.Error(MESSAGES.rptRDLCNotExist + " " + ReportService.Model.InputReport.FileFullName);
                return false;
            }
            return true;
        }

        private async Task<ReportData> GetReportData_Employee()
        {
            var _Result = new ReportData();

            if (MyModel.EmployeeID == 0)
            {
                ReportService.IsError = true;
                MsgClass.Add(MESSAGES.EmployeeIDIsZero);
                return _Result;
            }

            var _OBDate = MyModel.DtFrom_Emp.AddDays(-1).ToString(Format.YMD);
            var _DateFrom = MyModel.DtFrom_Emp.ToString(Format.YMD);
            var _DateTo = MyModel.DtTo_Emp.ToString(Format.YMD);

            var _Nature = AppRegistry.GetText(AppGlobal.DBFile, "GLp_Nature");
            var _FilterOB = $"[Employee] = {MyModel.EmployeeID} AND  [COA] IN ({_Nature}) AND Date([Vou_Date]) < Date('{_DateFrom}')";
            var _Filter = $"[Employee] = {MyModel.EmployeeID} AND  [COA] IN ({_Nature}) AND (Date([Vou_Date]) BETWEEN Date('{_DateFrom}') AND Date('{_DateTo}'))";
            var _GroupBy = "[Employee]";
            var _SortBy = "[Vou_date], [Vou_no]";

            var _Query = SQLQueries.Quries.Ledger2(_FilterOB, _Filter, _GroupBy, _OBDate, _SortBy);

            MyModel.PagingQuery.Query = _Query;
            MyModel.PagingQuery.Source = Source;

            MyModel.PagingQuery.Pages = MyModel.PagingQuery.Pages ?? new PageModel();

            DataTable _ReportTable = Source.GetTable(_Query);
            MyModel.PagingQuery.Pages.TotalRecords = _ReportTable.Rows.Count;
            DataTable _DisplayTable = await MyModel.PagingQuery.GetPageData();

            if (_ReportTable.Columns.Count == 0)
            {
                ReportService.IsError = false;
                MsgClass.Add(MESSAGES.NoRecordFound);
            }

            _Result.ReportTable = _ReportTable;
            _Result.DataSetName = "ds_EmployeeGL";

            ReportService.Data.ReportTable = _Result.ReportTable;
            ReportService.Data.DataSetName = _Result.DataSetName;
            MyModel.Ledger = _DisplayTable;

            return _Result;
        }
        #endregion

        #region Print Project
        public async void PrintProject(ReportActionClass PrintAction)
        {
            MsgClass = new();           // Clear all previous messages - refresh
            IsPrinting = true;
            await InvokeAsync(StateHasChanged); await Task.Delay(100);
            try
            {
                SetKeys();
                ReportService = new(AppGlobal); ;
                ReportService.ReportType = PrintAction.PrintType;
                ReportService.IsError = await CreateReportModel_Project();
                if (ReportService.IsError)
                {
                    ReportService.Print();
                }
            }
            catch (Exception error)
            {
                MsgClass.Error(error.Message);
            }
            
            IsPrinting = false;
            await InvokeAsync(StateHasChanged);
        }
        private async Task<bool> CreateReportModel_Project() {
            var _ProjectName = Source.SeekTitle(AppliedDB.Enums.Tables.Project, MyModel.ProjectID);
            var _Heading1 = $"Project Ledger {_ProjectName}";
            var _Heading2 = $"[{MyModel.DtFrom_Prj.ToString(Format.DDMMMYY)}] to [{MyModel.DtTo_Prj.ToString(Format.DDMMMYY)}] ";
            var _ReportName = "GLProject.rdl";   // Dataset ds_Project
            ReportService.Model.InputReport.FileName = _ReportName;

            if (File.Exists(ReportService.Model.InputReport.FileFullName))
            {
                //ReportService.Model = new();
                ReportService.Model.InputReport.FileName = _ReportName;
                ReportService.Model.OutputReport.FileName = "ProjectGL_" + _ProjectName.Replace(" ", "_");
                ReportService.Model.OutputReport.ReportType = ReportService.ReportType;
                ReportService.Model.AddReportParameter("Heading1", _Heading1);
                ReportService.Model.AddReportParameter("Heading2", _Heading2);
                ReportService.Model.ReportDataSource = await GetReportData_Project();  //ReportService.Data;  // Load Reporting Data to Report Model
                ReportService.IsError = false;
            }
            else
            {
                ReportService.IsError = true;
                MsgClass.Error(MESSAGES.rptRDLCNotExist + " " + ReportService.Model.InputReport.FileFullName);
                return false;
            }
            return true;
        }
        public async Task<ReportData> GetReportData_Project() 
        {
            var _Result = new ReportData();

            if (MyModel.ProjectID == 0)
            {
                ReportService.IsError = true;
                MsgClass.Add(MESSAGES.ProjectIDIsZero);
                return _Result;
            }

            var _OBDate = MyModel.DtFrom_Prj.AddDays(-1).ToString(Format.YMD);
            var _DateFrom = MyModel.DtFrom_Prj.ToString(Format.YMD);
            var _DateTo = MyModel.DtTo_Prj.ToString(Format.YMD);

            var _Nature = AppRegistry.GetText(AppGlobal.DBFile, "GLp_Nature");
            var _FilterOB = $"[Project] = {MyModel.ProjectID} AND  [COA] IN ({_Nature}) AND Date([Vou_Date]) < Date('{_DateFrom}')";
            var _Filter = $"[Project] = {MyModel.ProjectID} AND  [COA] IN ({_Nature}) AND (Date([Vou_Date]) BETWEEN Date('{_DateFrom}') AND Date('{_DateTo}'))";
            var _GroupBy = "[Project]";
            var _SortBy = "[Vou_date], [Vou_no]";

            var _Query = SQLQueries.Quries.Ledger2(_FilterOB, _Filter, _GroupBy, _OBDate, _SortBy);

            MyModel.PagingQuery.Query = _Query;
            MyModel.PagingQuery.Source = Source;

            MyModel.PagingQuery.Pages = MyModel.PagingQuery.Pages ?? new PageModel();

            DataTable _ReportTable = Source.GetTable(_Query);
            MyModel.PagingQuery.Pages.TotalRecords = _ReportTable.Rows.Count;
            DataTable _DisplayTable = await MyModel.PagingQuery.GetPageData();

            if (_ReportTable.Columns.Count == 0)
            {
                ReportService.IsError = false;
                MsgClass.Add(MESSAGES.NoRecordFound);
            }

            _Result.ReportTable = _ReportTable;
            _Result.DataSetName = "ds_Project";

            ReportService.Data.ReportTable = _Result.ReportTable;
            ReportService.Data.DataSetName = _Result.DataSetName;
            MyModel.Ledger = _DisplayTable;

            return _Result;

        }
        #endregion

        public class GLModel
        {
            public int BookID { get; set; }
            public int COAID { get; set; }
            public int CompanyID { get; set; }
            public int ProjectID { get; set; }
            public int EmployeeID { get; set; }

            public DateTime Date_From { get; set; }
            public DateTime Date_To { get; set; }
            public string SortBy { get; set; }
            public DataTable Ledger { get; set; }

            public DateTime DtFrom_Com { get; set; }            // Date for (Companies/Clients)
            public DateTime DtTo_Com { get; set; }
           

            public DateTime DtFrom_Emp { get; set; }            // Date for (Employees)
            public DateTime DtTo_Emp { get; set; }
            
            public DateTime DtFrom_Prj { get; set; }            // Date for (Projects
            public DateTime DtTo_Prj { get; set; }

            public PageQuery PagingQuery { get; set; } = new();

            public BrowseModel BrowseClass { get; set; } = new();

            public List<CodeTitle> AccountList { get; set; } = [];
            public List<CodeTitle> CompanyList { get; set; } = [];
            public List<CodeTitle> EmployeeList { get; set; } = [];
            public List<CodeTitle> ProjectList { get; set; } = [];

            public string TitleCOA { get; set; }
            public string TitleCompany { get; set; }
            public string TitleEmployee { get; set; }

        }
    }
}

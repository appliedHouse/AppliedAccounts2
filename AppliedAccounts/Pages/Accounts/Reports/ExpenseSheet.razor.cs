using AppliedAccounts.Data;
using SQLQueries;
using Microsoft.AspNetCore.Components;
using System.Data;
using AppliedDB;

namespace AppliedAccounts.Pages.Accounts.Reports
{
    public partial class ExpenseSheet
    {
        #region Valiables
        public ReportActionClass RptBtns { get; set; } = new();
        public ExpenseSheetModel MyModel { get; set; } = new();

        public DataSource Source { get; set; }
        public string SearchText = "";
        public string SelectedSheetNo = "";
        public DataTable ExpenseTable { get; set; } = new();
        public List<CodeTitle> SheetTable { get; set; } = new();
        public int sr { get; set; }
        private decimal SearchDR = 0;
        private decimal SearchCR = 0;

        private DataRow SelectedRow { get; set; }
        private bool IsModalOpen { get; set; } = false;

        #endregion


        #region Constructor
        public ExpenseSheet()
        {
           //Source = new(AppGlobal.AppPaths);
           //LoadSheetList();
        }
        #endregion

        internal void LoadSheetList()
        {
            try
            {
                SheetTable = [..AppGlobal.Source.GetTable(Quries.ExpenseSheetList())
                    .AsEnumerable()
                    .Select(r => new CodeTitle
                    {
                        Code = r["SheetNo"]?.ToString() ?? "",
                        Title = r["SheetNo"]?.ToString() ?? ""
                    })];


                if (SheetTable.Count > 0)
                {
                    SelectedSheetNo = SheetTable[0].Code;
                    LoadExpenseSheet();
                }

            }
            catch (Exception ex)
            {
                ToastService.ShowError(ex.Message);
            }
        }

        private void LoadExpenseSheet()
        {
            try
            {
                ExpenseTable = AppGlobal.Source.GetTable(Quries.ExpenseSheetData(SelectedSheetNo));
            }
            catch (Exception ex)
            {
                ToastService.ShowError(ex.Message);
            }
        }

        private void ClearSearch()
        {
            SearchText = "";
        }

        private DataRow[] SearchResult()
        {
            sr = 0;
            SearchDR = 0;
            SearchCR = 0;
            
            DataRow[] Rows;
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                Rows = ExpenseTable.Select();
            }
            else
            {
                Rows = ExpenseTable.Select($@"
                                Vou_No LIKE '%{SearchText}%'
                                OR Ref_No LIKE '%{SearchText}%'
                                OR TitleCOA LIKE '%{SearchText}%'
                                OR TitleCompany LIKE '%{SearchText}%'
                                OR Description LIKE '%{SearchText}%'
                            ");
            }

            foreach (var item in Rows)
            {
                SearchDR += Convert.ToDecimal(item["DR"]);
                SearchCR += Convert.ToDecimal(item["CR"]);
            }
            return Rows;
        }

        public void Print(ReportActionClass PrintAction)
        {
            try
            {

                ReportService = new(AppGlobal); ;
                ReportService.ReportType = PrintAction.PrintType;
                CreateReportModel().Wait();
                ReportService.Print();

            }
            catch (Exception ex)
            {
                ToastService.ShowError(ex.Message);
            }
        }

        public async void PrintSummary(ReportActionClass PrintAction)
        {
            try
            {
                ReportService = new(AppGlobal); ;

                ReportService.ReportType = PrintAction.PrintType;

                await CreateSummaryReportModel();

                ReportService.Print();
            }
            catch (Exception ex)
            {
                ToastService.ShowError(ex.Message);
            }
        }

        public async Task CreateReportModel()
        {
            try
            {

                DataTable ReportTable = ExpenseTable.Copy();
                ReportService.Data.DataSetName = "ds_ExpenseSheet";
                ReportService.Data.ReportTable = ReportTable;
                ReportService.Model.InputReport.FileName = "ExpenseSheet.rdl";
                ReportService.Model.OutputReport.FileName = "ExpenseSheet";
                ReportService.Model.OutputReport.ReportType = ReportService.ReportType;
                ReportService.Model.AddReportParameter("Heading1", "Expense Sheet Report");
                ReportService.Model.AddReportParameter("Heading2", $"Sheet No : {SelectedSheetNo}");

            }
            catch (Exception ex)
            {
                ToastService.ShowError(ex.Message);
            }
        }
        public async Task CreateSummaryReportModel()
        {
            try
            {

                DataTable SummaryTable = AppGlobal.Source.GetTable(Quries.ExpenseSheetSummary(SelectedSheetNo));
                ReportService.Data.DataSetName = "ds_ExpenseGroup";
                ReportService.Data.ReportTable = SummaryTable;
                ReportService.Model.InputReport.FileName = "ExpenseSheetGroup.rdl";
                ReportService.Model.OutputReport.FileName = "ExpenseSheetSummary";
                ReportService.Model.OutputReport.ReportType = ReportService.ReportType;
                ReportService.Model.AddReportParameter("Heading1", "PROJECT SUMMARY EXPENSES SHEET");
                ReportService.Model.AddReportParameter("Heading2", $"Project Sheet # {SelectedSheetNo}");

            }
            catch (Exception ex)
            {
                ToastService.ShowError(ex.Message);
            }
        }

        #region Dropdown Changed Events
        private void OnSheetChanged(string sheetNo)
        {
            MyModel.SelectedSheetNo = sheetNo;
            SelectedSheetNo = sheetNo;
            LoadExpenseSheet();
        }
        #endregion

        // Helper method to display selected sheet text
        private string GetSelectedSheetText()
        {
            if (string.IsNullOrEmpty(MyModel?.SelectedSheetNo))
                return null!;

            var sheet = SheetTable?.FirstOrDefault(s => s.Code == MyModel.SelectedSheetNo);
            return sheet?.Title ?? MyModel.SelectedSheetNo;
        }

        private void SelectSheet(string code)
        {
            if (!string.IsNullOrEmpty(code))
            {
                MyModel.SelectedSheetNo = code;
                SelectedSheetNo = code;
                LoadExpenseSheet();
                StateHasChanged();
            }
        }



        private void OpenPreviewModal(DataRow row)
        {
            SelectedRow = row;
            IsModalOpen = true;
            StateHasChanged();
        }

        // Method to close modal
        private void CloseModal()
        {
            IsModalOpen = false;
            SelectedRow = null!;
            StateHasChanged();
        }

        // Helper method to safely get value from DataRow
        private string GetStringValue(DataRow row, string columnName)
        {
            return row?.Table?.Columns?.Contains(columnName) == true
                ? row[columnName]?.ToString() ?? string.Empty
                : string.Empty;
        }

        private decimal GetDecimalValue(DataRow row, string columnName)
        {
            if (row?.Table?.Columns?.Contains(columnName) == true && row[columnName] != DBNull.Value)
            {
                return Convert.ToDecimal(row[columnName]);
            }
            return 0;
        }

        private DateTime GetDateTimeValue(DataRow row, string columnName)
        {
            if (row?.Table?.Columns?.Contains(columnName) == true && row[columnName] != DBNull.Value)
            {
                return Convert.ToDateTime(row[columnName]);
            }
            return DateTime.Now;
        }

        public class ExpenseSheetModel
        {
            public string SelectedSheetNo { get; set; }
            public string SearchText { get; set; }
        }
    }
}


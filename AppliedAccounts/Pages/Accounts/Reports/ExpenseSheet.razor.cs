using AppliedAccounts.Data;
using SQLQueries;
using Microsoft.AspNetCore.Components;
using System.Data;
using AppliedDB;

namespace AppliedAccounts.Pages.Accounts.Reports
{
    public partial class ExpenseSheet
    {
        public ReportActionClass RptBtn { get; set; }
        public ExpenseSheetModel MyModel { get; set; } = new();

        public string SearchText = "";
        public string SelectedSheetNo = "";
        public DataTable ExpenseTable { get; set; } = new();
        public List<CodeTitle> SheetTable { get; set; } = new();

        internal void LoadSheets()
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
                ReportService = new(AppGlobal);

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


        public class ExpenseSheetModel
        {
            public string SelectedSheetNo { get; set; }
            public string SearchText { get; set; }
        }
    }
}


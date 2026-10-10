using AppliedDB;
using System.Data;

namespace AppliedAccounts.Pages.Sale.Reports
{
    public class SalesPivotSheet
    {
        public DataSource Source { get; set; }
        public DataTable? BatchData { get; set; }
        public DataTable? PivotData { get; set; }
        public List<string> InventoryCodes { get; set; } = new();
        public List<string> Messages { get; set; } = new();

        public SalesPivotSheet(DataSource dataSource, string batch)
        {
            Messages.Add("Sales Pivot Sheet Process Start.");

            Source = dataSource;
            BatchData = GetBatch(batch);

            if (BatchData == null)
            {
                Messages.Add($"Batch {batch} returned no data.");
                return;
            }

            Messages.Add("Sales Pivot Data Completed.");
            Messages.Add($"Total {BatchData.Rows.Count} records found.");

            CreatePivotTable();

            if (PivotData == null)
            {
                Messages.Add("Sales Pivot Data is null.");
            }
            else
            {
                Messages.Add("Sales Pivot Data completed successfully.");
                Messages.Add($"Sales Pivot Data has {PivotData.Rows.Count} records.");
            }
        }

        private DataTable? GetBatch(string batchNo)
        {
            string query = ReportQuery.SalesBatchQuery(batchNo);
            return Source.GetTable(query);
        }

        private List<string> GetDistinctCodes(DataTable batch)
        {
            return batch.AsEnumerable()
                        .Select(r => r.Field<string>("Code"))
                        .Where(c => !string.IsNullOrWhiteSpace(c))
                        .Distinct()
                        .OrderBy(c => c)
                        .ToList()!;
        }

        private void CreatePivotTable()
        {
            if (BatchData == null) return;


            #region Creating Pivot Table

            PivotData = new DataTable("SalesPivot");

            PivotData.Columns.Add("Sr_No", typeof(int));
            PivotData.Columns.Add("CompanyCode", typeof(string));   // renamed: no clash with dynamic "Code" columns
            PivotData.Columns.Add("Company", typeof(long));
            PivotData.Columns.Add("CompanyTitle", typeof(string));

            // Discover dynamic columns from the SOURCE data
            InventoryCodes = GetDistinctCodes(BatchData);

            foreach (var code in InventoryCodes)
            {
                if (!PivotData.Columns.Contains(code))
                    PivotData.Columns.Add(code, typeof(decimal));
            }

            PivotData.Columns.Add("Total", typeof(decimal));

            var PivotList = PivotData.AsEnumerable().ToList();

            #endregion

            var _SrNo = 1;

            foreach (DataRow row in BatchData.Rows)
            {
                long _CompanyID = row.Field<long?>("Company") ?? 0L;
                long _StockID = row.Field<long?>("Inventory") ?? 0L;

                string? _InvCode = row.Field<string>("InventoryCode");
                string _StockCode;

                if (string.IsNullOrWhiteSpace(_InvCode))
                {
                    _StockCode = (_StockID + 10000).ToString("D6");    // "010001", "010056"
                }
                else
                {
                    _StockCode = _InvCode.Trim();
                }

                var BatchRow = PivotList.FirstOrDefault(e => e.Field<long>("Company") == _CompanyID);

                if (BatchRow == null)
                {
                    var NewRow = PivotList.ToDataTable().NewRow();
                    NewRow["Sr_No"] = _SrNo; _SrNo++;
                    NewRow["Company"] = row["Company"];
                    NewRow["CompanyCode"] = row["Companycode"];
                    NewRow["CompanyTitle"] = row["CompanyTitle"];
                    NewRow[_StockCode] = row["Net_Amount"];
                    PivotList.Add(NewRow);
                }
                else
                {
                    BatchRow[_StockCode] = row["New_Amount"];
                }

            }

            PivotData = PivotList.ToDataTable();

        }

        public DataTable? GetBatchData() => BatchData;
    }
}
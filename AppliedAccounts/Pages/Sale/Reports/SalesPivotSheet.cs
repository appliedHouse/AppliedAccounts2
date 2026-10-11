using AppliedDB;
using AppliedExcel.CDC;
using System.Data;


namespace AppliedAccounts.Pages.Sale.Reports
{
    public class SalesPivotSheet
    {
        #region Variables
        private DataSource Source { get; set; }
        private DataTable? BatchData { get; set; }
        private DataTable? PivotData { get; set; }
        private List<string> InventoryCodes { get; set; } = new();
        public List<string> Messages { get; set; } = new();
        public DataTable? GetBatchData() => BatchData;
        public DataTable? GetPivotData() => PivotData;
        private byte[] PivotBytes { get; set; }
        #endregion

        #region Constructor
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
        #endregion

        private DataTable? GetBatch(string batchNo)
        {
            string filter = $"[Batch]='{batchNo}'";
            string query = ReportQuery.SalesBatchQuery(filter);
            var _DataTable = Source.GetTable(query);
            _DataTable.TableName = "BatchData";
            return _DataTable;
        }

        private List<string> GetDistinctCodes(DataTable batch)
        {
            try
            {
                return batch.AsEnumerable()
                        .Select(r => r.Field<string>("InventoryCode"))
                        .Where(c => !string.IsNullOrWhiteSpace(c))
                        .Distinct()
                        .OrderBy(c => c)
                        .ToList()!;
            }
            catch (Exception error)
            {
                Messages.Add(error.Message);
            }
            return [];
        }

        private void CreatePivotTable()
        {
            if (BatchData == null) return;

            try
            {


                #region Creating Pivot Table

                PivotData = new DataTable("SalesPivot");

                PivotData.Columns.Add("Sr_No", typeof(int));
                PivotData.Columns.Add("Company", typeof(long));
                PivotData.Columns.Add("CompanyCode", typeof(string));
                PivotData.Columns.Add("CompanyTitle", typeof(string));

                InventoryCodes = GetDistinctCodes(BatchData);

                foreach (var code in InventoryCodes)
                {
                    if (!PivotData.Columns.Contains(code))
                        PivotData.Columns.Add(code, typeof(decimal));
                }

                PivotData.Columns.Add("Total", typeof(decimal));

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

                    var BatchRow = PivotData.AsEnumerable().ToList().FirstOrDefault(e => e.Field<long>("Company") == _CompanyID);

                    // Helper once, outside the loop
                    var _NetAmount = row["Net_Amount"];
                    decimal.TryParse(_NetAmount.ToString(), out decimal lineAmount);

                    if (BatchRow == null)
                    {
                        // ---- New company: create pivot row ----
                        var NewRow = PivotData.NewRow();

                        NewRow["Sr_No"] = _SrNo++;
                        NewRow["Company"] = row["Company"];
                        NewRow["CompanyCode"] = row["CompanyCode"];
                        NewRow["CompanyTitle"] = row["CompanyTitle"];

                        // Initialize ALL dynamic columns to 0 (not DBNull)
                        foreach (string code in InventoryCodes)
                            NewRow[code] = 0m;

                        // Set this line's amount
                        NewRow[_StockCode] = lineAmount;
                        NewRow["Total"] = lineAmount;

                        PivotData.Rows.Add(NewRow);
                    }
                    else
                    {
                        // ---- Existing company: accumulate ----
                        decimal existing = BatchRow.Field<decimal?>(_StockCode) ?? 0m;
                        BatchRow[_StockCode] = existing + lineAmount;

                        decimal currentTotal = BatchRow.Field<decimal?>("Total") ?? 0m;
                        BatchRow["Total"] = currentTotal + lineAmount;
                    }

                }
            }
            catch (Exception error)
            {
                Messages.Add(error.Message);
            }
        }

        public byte[] GetPivotBytes()
        {
            if(PivotData != null)
            {
                if (PivotData.Rows.Count > 0)
                {
                    var ExcelClass = new SalesPivot(PivotData);
                    PivotBytes = ExcelClass.ToExcel();
                    if(PivotBytes != null)
                    {
                        if(PivotBytes.Length > 0)
                        {
                            return PivotBytes;
                        }
                    }
                }
            }
            return null!;
        }
    }
}


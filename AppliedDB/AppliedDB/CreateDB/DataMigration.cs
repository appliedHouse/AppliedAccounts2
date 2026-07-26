using AppliedGlobals;
using System.Data;
using System.Text;

namespace AppliedDB.CreateDB
{
	public class DataMigration
	{
        private DataSource Source { get; set; }
        //private DataTable CashBook { get; set; }
        //private DataTable BankBook { get; set; }
        public string DBFile { get; set; }
        public StringBuilder Log { get; set; } = new StringBuilder();
        public DataMigration(DataSource source)           
		{
            Source = source;
        }

        public async Task Cash2BookAsync(IProgress<ProgressBarModel.ProgressReport> progress = null)
        {
            var CashBook = Source.GetTable(Enums.Tables.CashBook);
            var _Book1 = Source.GetTable(Enums.Tables.Book);
            var _Book2 = Source.GetTable(Enums.Tables.Book2);

            var totalRecords = CashBook.Rows.Count;
            var processedCount = 0;
            var duplicateCount = 0;
            var batchSize = 100; // Adjust based on performance

            if (totalRecords == 0)
            {
                progress?.Report(new ProgressBarModel.ProgressReport
                {
                    Current = 100,
                    Total = 100,
                    Message = "No records to process",
                    IsComplete = true
                });
                return;
            }

            // Get existing Vou_No for faster lookup
            var existingVouNos = new HashSet<string>(
                _Book1.AsEnumerable().Select(r => r.Field<string>("Vou_No"))
            );

            // Process in batches
            for (int i = 0; i < totalRecords; i += batchSize)
            {
                var batch = CashBook.Rows.Cast<DataRow>().Skip(i).Take(batchSize).ToList();

                // Start a transaction for this batch
                Source.BeginTransaction();
                //using var transaction = Source.BeginTransaction();

                try
                {
                    foreach (var _Row in batch)
                    {
                        var vouNo = _Row.Field<string>("Vou_No");

                        if (!existingVouNos.Contains(vouNo))
                        {
                            var _NewRow1 = _Book1.NewRow();
                            var _NewRow2 = _Book2.NewRow();

                            _NewRow1["ID"] = 0;
                            _NewRow1["BookID"] = _Row["BookID"];
                            _NewRow1["Vou_No"] = vouNo;
                            _NewRow1["Vou_Date"] = _Row["Vou_Date"];
                            _NewRow1["Amount"] = ((decimal)_Row["DR"] - (decimal)_Row["CR"]);
                            _NewRow1["Ref_No"] = _Row["Ref_No"];
                            _NewRow1["SheetNo"] = _Row["Sheet_No"];
                            _NewRow1["Remarks"] = _Row["Description"];
                            _NewRow1["Status"] = _Row["Status"];

                            _NewRow2["ID"] = 0;
                            _NewRow2["TranId"] = _Row["ID"];
                            _NewRow2["Sr_No"] = 1;
                            _NewRow2["COA"] = _Row["COA"];
                            _NewRow2["Company"] = _Row["Customer"];
                            _NewRow2["Employee"] = _Row["Employee"];
                            _NewRow2["Project"] = _Row["Project"];
                            _NewRow2["DR"] = _Row["DR"];
                            _NewRow2["CR"] = _Row["CR"];
                            _NewRow2["Description"] = _Row["Description"];
                            _NewRow2["Comments"] = _Row["Comments"];

                            Source.Save(_NewRow1);
                            _NewRow2["TranId"] = Source.MyCommands.PrimaryKeyID;
                            Source.Save(_NewRow2);
                        }
                        else
                        {
                            duplicateCount++;
                        }

                        processedCount++;

                        // Report progress every 10 records
                        if (processedCount % 10 == 0 || processedCount == totalRecords)
                        {
                            progress?.Report(new ProgressBarModel.ProgressReport
                            {
                                Current = Math.Min(processedCount, totalRecords),
                                Total = totalRecords,
                                Message = $"Processing record {processedCount} of {totalRecords}... (Duplicates skipped: {duplicateCount})"
                            });
                        }
                    }

                    Source.CommitTransaction();
                }
                catch
                {
                    Source.RollbackTransaction();
                    throw;
                }

                // Allow UI to update between batches
                await Task.Delay(1);
            }

            progress?.Report(new ProgressBarModel.ProgressReport
            {
                Current = totalRecords,
                Total = totalRecords,
                Message = $"Completed! Processed {processedCount - duplicateCount} records, skipped {duplicateCount} duplicates",
                IsComplete = true
            });
        }

        public async Task Bank2BookAsync(IProgress<ProgressBarModel.ProgressReport> progress = null)
        {
            var BankBook = Source.GetTable(Enums.Tables.BankBook);
            var _Book1 = Source.GetTable(Enums.Tables.Book);
            var _Book2 = Source.GetTable(Enums.Tables.Book2);

            var totalRecords = BankBook.Rows.Count;
            var processedCount = 0;
            var duplicateCount = 0;
            var batchSize = 100; // Adjust based on performance

            if (totalRecords == 0)
            {
                progress?.Report(new ProgressBarModel.ProgressReport
                {
                    Current = 100,
                    Total = 100,
                    Message = "No records to process",
                    IsComplete = true
                });
                return;
            }

            // Get existing Vou_No for faster lookup
            var existingVouNos = new HashSet<string>(
                _Book1.AsEnumerable().Select(r => r.Field<string>("Vou_No"))
            );

            // Process in batches
            for (int i = 0; i < totalRecords; i += batchSize)
            {
                var batch = BankBook.Rows.Cast<DataRow>().Skip(i).Take(batchSize).ToList();

                // Start a transaction for this batch
                Source.BeginTransaction();
                //using var transaction = Source.BeginTransaction();

                try
                {
                    foreach (var _Row in batch)
                    {
                        var vouNo = _Row.Field<string>("Vou_No");

                        if (!existingVouNos.Contains(vouNo))
                        {
                            var _NewRow1 = _Book1.NewRow();
                            var _NewRow2 = _Book2.NewRow();

                            _NewRow1["ID"] = 0;
                            _NewRow1["BookID"] = _Row["BookID"];
                            _NewRow1["Vou_No"] = vouNo;
                            _NewRow1["Vou_Date"] = _Row["Vou_Date"];
                            _NewRow1["Amount"] = ((decimal)_Row["DR"] - (decimal)_Row["CR"]);
                            _NewRow1["Ref_No"] = _Row["Ref_No"];
                            _NewRow1["SheetNo"] = _Row["Sheet_No"];
                            _NewRow1["Remarks"] = _Row["Description"];
                            _NewRow1["Status"] = _Row["Status"];

                            _NewRow2["ID"] = 0;
                            _NewRow2["TranId"] = _Row["ID"];
                            _NewRow2["Sr_No"] = 1;
                            _NewRow2["COA"] = _Row["COA"];
                            _NewRow2["Company"] = _Row["Customer"];
                            _NewRow2["Employee"] = _Row["Employee"];
                            _NewRow2["Project"] = _Row["Project"];
                            _NewRow2["DR"] = _Row["DR"];
                            _NewRow2["CR"] = _Row["CR"];
                            _NewRow2["Description"] = _Row["Description"];
                            _NewRow2["Comments"] = _Row["Comments"];

                            Source.Save(_NewRow1);
                            _NewRow2["TranId"] = Source.MyCommands.PrimaryKeyID;
                            Source.Save(_NewRow2);
                        }
                        else
                        {
                            duplicateCount++;
                        }

                        processedCount++;

                        // Report progress every 10 records
                        if (processedCount % 10 == 0 || processedCount == totalRecords)
                        {
                            progress?.Report(new ProgressBarModel.ProgressReport
                            {
                                Current = Math.Min(processedCount, totalRecords),
                                Total = totalRecords,
                                Message = $"Processing record {processedCount} of {totalRecords}... (Duplicates skipped: {duplicateCount})"
                            });
                        }
                    }

                    Source.CommitTransaction();
                }
                catch
                {
                    Source.RollbackTransaction();
                    throw;
                }

                // Allow UI to update between batches
                await Task.Delay(1);
            }

            progress?.Report(new ProgressBarModel.ProgressReport
            {
                Current = totalRecords,
                Total = totalRecords,
                Message = $"Completed! Processed {processedCount - duplicateCount} records, skipped {duplicateCount} duplicates",
                IsComplete = true
            });
        }

    }
}

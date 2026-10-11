using ClosedXML.Excel;
using System.Data;

namespace AppliedExcel.CDC
{
    public class SalesPivot
    {
        public DataTable? PivotData { get; set;  }

        public SalesPivot(DataTable? _table)
        {
            PivotData = _table;
        }

        public byte[] ToExcel(string sheetName = "Sheet1")
        {
            if (PivotData == null) return Array.Empty<byte>();

            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add(sheetName);

            // Header
            for (int c = 0; c < PivotData.Columns.Count; c++)
            {
                var cell = ws.Cell(1, c + 1);
                cell.Value = PivotData.Columns[c].ColumnName;
                cell.Style.Font.Bold = true;
                cell.Style.Fill.BackgroundColor = XLColor.LightGray;
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            }

            // Rows
            for (int r = 0; r < PivotData.Rows.Count; r++)
            {
                for (int c = 0; c < PivotData.Columns.Count; c++)
                {
                    var value = PivotData.Rows[r][c];
                    var cell = ws.Cell(r + 2, c + 1);

                    if (value == null || value == DBNull.Value) continue;

                    if (value is decimal dec)
                    {
                        cell.Value = dec;
                        cell.Style.NumberFormat.Format = "#,##0.00;(#,##0.00)";
                    }
                    else if (value is long l)
                    {
                        cell.Value = l;
                    }
                    else if (value is int i)
                    {
                        cell.Value = i;
                    }
                    else
                    {
                        cell.Value = value.ToString();
                    }
                }
            }

            // Formatting
            ws.Column(1).Width = 6;    // Sr_No
            ws.Column(2).Width = 12;   // Company
            ws.Column(3).Width = 14;   // CompanyCode
            ws.Column(4).Width = 32;   // CompanyTitle
            ws.Columns(5, PivotData.Columns.Count).Width = 12;

            ws.SheetView.FreezeRows(1);
            ws.RangeUsed()?.SetAutoFilter();

            using var ms = new MemoryStream();
            wb.SaveAs(ms);
            return ms.ToArray();
        }
    }
}

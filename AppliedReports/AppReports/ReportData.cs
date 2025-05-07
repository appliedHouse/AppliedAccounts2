using Microsoft.Reporting.NETCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppReports
{
    interface IReportData
    {
        ReportDataSource DataSource { get; }
    }

    public class ReportData : IReportData
    {
        public DataTable ReportTable { get; set; } = new();
        public string DataSetName { get; set; } = string.Empty;
        public int Count => ReportTable.Rows.Count;

        public ReportDataSource DataSource => GetReportDataSource();


        public ReportData() { }
        public ReportData(DataTable _Table, string _DataSet)
        {
            ReportTable = _Table;
            DataSetName = _DataSet;
        }

        private ReportDataSource GetReportDataSource()
        {
            if (DataSetName.Length > 0 && ReportTable != null)
            {
                return new ReportDataSource(DataSetName, ReportTable);
            }
            return new ReportDataSource();
        }
    }
}

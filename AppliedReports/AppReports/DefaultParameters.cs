using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Reporting.NETCore;


namespace AppReports
{
    internal class DefaultParameters
    {
        public static IEnumerable<ReportParameter> GetDefaultParameters()
        {
            List<ReportParameter> _Parameters = new List<ReportParameter>
            {
                new ReportParameter("CompanyName", "Applied Software House"),
                new ReportParameter("Heading1", "Heading "),
                new ReportParameter("Heading2", "Sub Heading"),
                new ReportParameter("Footer", "Powered by Applied Software House")
            };

            return _Parameters;
        }
    }
}

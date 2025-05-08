using Microsoft.Reporting.NETCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AppReports
{
    public class ReportExtractor
    {
        public List<ReportParameterClass> MyParameters { get; set; }

        public string RDL_File { get; set; }
        public string DataSetName => GetDataSetNames().FirstOrDefault() ?? "";

        public ReportExtractor(string _RDLFile)
        {
            RDL_File = _RDLFile;
            MyParameters = GetParameters(RDL_File);
        }
        public bool IsExistParameter(string value)
        {
            return MyParameters.Any(p => p.Name.Equals(value, StringComparison.OrdinalIgnoreCase));
        }

        public static List<ReportParameterClass> GetParameters(string rdlFilePath)
        {
            XNamespace ns = "http://schemas.microsoft.com/sqlserver/reporting/2016/01/reportdefinition";
            var xdoc = XDocument.Load(rdlFilePath);
            var parameters = new List<ReportParameterClass>();

            foreach (var param in xdoc.Descendants(ns + "ReportParameter"))
            {
                parameters.Add(new ReportParameterClass
                {
                    Name = param.Attribute("Name")?.Value ?? "",
                    DataType = param.Element(ns + "DataType")?.Value ?? "",
                    Prompt = param.Element(ns + "Prompt")?.Value ?? ""
                });
            }

            return parameters;
        }

       

        private List<string> GetDataSetNames()
        {
            XNamespace ns = "http://schemas.microsoft.com/sqlserver/reporting/2016/01/reportdefinition";
            var xdoc = XDocument.Load(RDL_File);
            var datasetNames = new List<string>();

            foreach (var dataset in xdoc.Descendants(ns + "DataSet"))
            {
                var nameAttr = dataset.Attribute("Name");
                if (nameAttr != null)
                {
                    datasetNames.Add(nameAttr.Value);
                }
            }

            return datasetNames;
        }
    }
    public class ReportParameterClass
    {
        public string Name { get; set; }
        public string DataType { get; set; }
        public string Prompt { get; set; }
    }
}

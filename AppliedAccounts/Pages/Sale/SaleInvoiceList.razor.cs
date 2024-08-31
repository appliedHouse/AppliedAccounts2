using AppliedAccounts.Data;
using AppMessages;
//using AppliedReports;
using System.Data;


namespace AppliedAccounts.Pages.Sale
{
    public partial class SaleInvoiceList
    {
        public Models.SaleInvoiceListModel Model { get; set; } = new();

        //public ReportModel CreateReportModel()
        //{
        //    //ReportModel _Reportmodel = new ReportModel();
        //    try
        //    {

        //        var _InvoiceNo = "INV-Testing";
        //        var _Heading1 = "Sales Invoice";
        //        var _Heading2 = $"Invoice No. {_InvoiceNo}";
        //        var _ReportPath = "";
        //        var _ReportOption = ReportType.Preview;
        //        var _CompanyName = Model.AppUser.Company;
        //        var _ReportFooter = AppFunctions.ReportFooter();

        //        // Input Parameters  (.rdl report file)
        //        _Reportmodel.InputReport.FilePath = _ReportPath;
        //        _Reportmodel.InputReport.FileName = "SaleInvoice";
        //        _Reportmodel.InputReport.FileExtention = "rdl";
        //        // output Parameters (like pdf, excel, word, html, tiff)
        //        _Reportmodel.OutputReport.FilePath = "";
        //        _Reportmodel.OutputReport.FileLink = "";
        //        _Reportmodel.OutputReport.FileName = "SaleInvoice";
        //        _Reportmodel.OutputReport.ReportType = _ReportOption;
        //        // Reports Parameters
        //        _Reportmodel.AddReportParameter("CompanyName", _CompanyName);
        //        _Reportmodel.AddReportParameter("Heading1", _Heading1);
        //        _Reportmodel.AddReportParameter("Heading2", _Heading2);
        //        _Reportmodel.AddReportParameter("Footer", _ReportFooter);

        //        var _SourceData = new DataTable();          // Inject a Data Table for perint.

        //        _Reportmodel.ReportData.DataSetName = "ds_SaleInvoice";
        //        _Reportmodel.ReportData.ReportTable = _SourceData;
        //    }
        //    catch (Exception)
        //    {
        //        Model.MyMessages.Add(MessagesEnums.Messages.Default);
        //    }

        //    return _Reportmodel;
        //}
    }
}

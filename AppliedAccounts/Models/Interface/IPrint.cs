using AppReports;

namespace AppliedAccounts.Models.Interface
{
    public interface IPrint
    {

        void Print(ReportType PrintType);
        ReportData GetReportData();
        ReportModel CreateReportModel();
    }
}

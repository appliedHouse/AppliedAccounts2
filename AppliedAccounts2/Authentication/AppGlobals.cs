namespace AppliedAccounts2.Authentication
{
    public class AppGlobals
    {
        public string CompanyName { get; set; } = "Applied Software House";
        public string Address { get; set; } = "Federal B Area, Karachi, Pakistan.";
        public string ContactNo { get; set; } = "+92 (336) 24 54 230";
        public string MobileNo { get; set; } = "+966 (55) 028 9497";
        public string EmialAddress { get; set; } = "info@Appliedhouse.com";
        public string GoogleMapID { get; set; } = "GXDRFDS";

        public string ReportPath { get; set; } = ".\\wwwroot\\Reports\\";
        public string ReportExtention { get; set; } = "rdl";
        public string PrintedReportsPath { get; set; } = ".\\wwwroot\\Printed\\";
        public string PrintedReportsPathLink { get; set; } = "/Printed/";
        public string ReportFooter { get; set; } = "Powered by Applied Software House";
    }
}

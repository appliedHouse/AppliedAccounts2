
using Microsoft.AspNetCore.Components.Forms;
using System.Data;
using AppliedDB;
using AppliedAccounts.Models;

namespace AppliedAccounts.Pages.ImportData
{
    public partial class ImportSaleInvoice
    {

        public ImportExcelFile ImportExcel { get; set; }
        public AppUserModel AppUser { get; set; }
        public DataSet? ExcelDataSet { get; set; }
        public string MyMessage {  get; set; }

        public ImportSaleInvoice()
        {
            

        }

        public void GetExcelFile(InputFileChangeEventArgs e)
        {
            ImportExcel = new(e.File, AppUser);
        }

    }
}


using Microsoft.AspNetCore.Components.Forms;

namespace AppliedDB
{
    class ReadExcel
    {
        public string FilePath { get; set; }
        public IBrowserFile browserFile { get; set; }


        public void ReadExcelFile(IBrowserFile _BrowseFile)
        {
            browserFile = _BrowseFile;

        }


    }
}

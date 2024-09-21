
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

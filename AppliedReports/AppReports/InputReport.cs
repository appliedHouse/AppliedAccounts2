using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppReports
{
    interface IInputReport
    {
        public string FileFullName { get; }
        public bool IsFileExist { get; }
    }


    public class InputReport : IInputReport
    {
        public string FilePath { get; set; } = string.Empty;            // Path after wwwroot
        public string FileName { get; set; } = string.Empty;            // File name
        public string BasePath { get; set; } = Directory.GetCurrentDirectory();
        public string RootPath { get; set; } = "wwwroot";

        public string FileFullName { get => GetFullName(); } 
        public bool IsFileExist { get => GetFileExist(); }
       

        public InputReport() { }
        public InputReport(string _FilePath, string _FileName) 
        {
            FilePath = _FilePath;
            FileName = _FileName;
        }

        public string GetFileExtention()
        {
            if(FileName.Length > 0)
            {
                return Path.GetExtension(FileName);
            }
            return string.Empty;
        }

        public bool GetFileExist()
        {
            if (File.Exists(FileFullName)) { return true; }
            { return false; }
        }

        public string GetFullName()
        {
            if (FileName.Length > 0)
            {
                return Path.Combine(BasePath, RootPath, FilePath, FileName);
            }
            return string.Empty;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nucleus.Service.Support.Utils
{
    public static class ExcelReader
    {
        public static bool GetRowCount(string fileLocation, string serverName, out int rowCount)
        {
           return UIAutomation.Framework.Utils.ExcelReader.GetRowCount(fileLocation, serverName, out rowCount);
        }

        public static bool GetDownloadedFileHeader(string fileLocation, string serverName, string workSheetName, out string downloadedFileHeader)
        {
            return UIAutomation.Framework.Utils.ExcelReader.GetDownloadedFileHeader(fileLocation, serverName, workSheetName, out downloadedFileHeader);
            
        }

        public static bool GetDownloadFileWorkSheets(string fileLocation, string serverName, out List<string> downloadFileWorkSheets)
        {
            return UIAutomation.Framework.Utils.ExcelReader.GetDownloadFileWorkSheets(fileLocation, serverName, out downloadFileWorkSheets);

        }

     
    }
}

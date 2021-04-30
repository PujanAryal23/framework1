using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Configuration;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System.Collections.Generic;
using System.Data.SqlClient;
using NPOI.XSSF.UserModel;
using UIAutomation.Framework.Core.Driver;

namespace UIAutomation.Framework.Utils
{
    public static class ExcelReader
    {
        public static bool GetRowCount(string fileLocation, string serverName, out int rowCount)
        {
            rowCount = -1;
            NetworkConnection network = null;
            if (serverName.Length != 0)
            {
                network = new NetworkConnection(serverName, ConfigurationManager.AppSettings["ProxyUserName"],
                    ConfigurationManager.AppSettings["ProxyPassword"], ConfigurationManager.AppSettings["ProxyDomain"]);
                network.ConnectToServer();
            }
            if (File.Exists(fileLocation))
            {
                Console.WriteLine("File Location: " + fileLocation);

                using (var document = new StreamReader(File.OpenRead(fileLocation)))
                {
                    string lines = document.ReadToEnd();
                    var line = lines.Split('\r');
                    rowCount = line.Count() - 1;
                    document.Close();
                }

                if (network != null)
                {
                    network.DisConnectToServer();
                }
                return true;
            }
            if (network != null)
            {
                network.DisConnectToServer();
            }
            return false;
        }

        public static bool GetDownloadedFileHeader(string fileLocation, string serverName, string currentSheet, out string downloadedFileHeader)
        {
            NetworkConnection network = null;
            downloadedFileHeader = "";
            if (serverName.Length != 0)
            {
                network = new NetworkConnection(serverName, ConfigurationManager.AppSettings["ProxyUserName"],
                    ConfigurationManager.AppSettings["ProxyPassword"], ConfigurationManager.AppSettings["ProxyDomain"]);
                network.ConnectToServer();
            }
            if (File.Exists(fileLocation))
            {

                HSSFWorkbook hssfwb;
                using (FileStream file = new FileStream(fileLocation, FileMode.Open, FileAccess.Read))
                {
                    hssfwb = new HSSFWorkbook(file);
                }

                ISheet sheet = hssfwb.GetSheet(currentSheet);

                if (currentSheet == "ClientAppealsSummary")
                    downloadedFileHeader = sheet.GetRow(0).GetCell(1).StringCellValue;
                else if (currentSheet == "AnalystAppealsSummary")
                    downloadedFileHeader = sheet.GetRow(1).GetCell(2).StringCellValue;

                if (network != null)
                {
                    network.DisConnectToServer();
                }
                return true;
            }
            if (network != null)
            {
                network.DisConnectToServer();
            }
            return false;

        }

        public static bool GetDownloadFileWorkSheets(string fileLocation, string serverName, out List<string> downloadFileWorkSheets)
        {

            NetworkConnection network = null;
            downloadFileWorkSheets = new List<string>();
            if (serverName.Length != 0)
            {
                network = new NetworkConnection(serverName, ConfigurationManager.AppSettings["ProxyUserName"],
                    ConfigurationManager.AppSettings["ProxyPassword"], ConfigurationManager.AppSettings["ProxyDomain"]);
                network.ConnectToServer();
            }
            //fileLocation = "\\172.27.5.169:v.nucleus.hcinsight.net\\api\\clients\\SMTST\\ciwDocDownloads\\GetDocument";
            if (File.Exists(fileLocation))
            {
                XSSFWorkbook workBook;

                using (FileStream file = new FileStream(fileLocation, FileMode.Open, FileAccess.Read))
                {

                    workBook = new XSSFWorkbook(file);
                }

                for (int i = 0; i <= 12; i++)
                {
                    downloadFileWorkSheets.Add(workBook.GetSheetAt(i).SheetName);
                }
                return true;
            }
            if (network != null)
            {
                network.DisConnectToServer();
            }
            return false;

        }

        public static void ReadExcelSheetValue(string fileName, string sheetName, int startrow, int endrow, out List<string> headerList, out List<List<string>> excelExportList, out string name, 
            bool isclientname = false, bool skipMergedCells = true,bool username=false)
        {
            XSSFWorkbook Workbook;
            List<string> exportedDatarow;
            excelExportList = new List<List<string>>();
            name = string.Empty;

            using (FileStream file = new FileStream(@"C:/Users/i11143/Downloads/" + fileName, FileMode.Open, FileAccess.Read))
            {
                Workbook = new XSSFWorkbook(file);
            }

            ISheet sheet = Workbook.GetSheet(sheetName);

            for (int row = startrow; row < sheet.LastRowNum - endrow; row++)
            {
                IRow rowindex = sheet.GetRow(row);
                exportedDatarow = new List<string>();
                foreach (var cell in rowindex.Cells)
                {
                    if (cell.IsMergedCell && skipMergedCells && cell.StringCellValue == "" )
                        continue;

                    if (cell.CellType == CellType.Numeric)
                        if (DateUtil.IsCellDateFormatted(cell))
                            exportedDatarow.Add(cell.DateCellValue.ToString());

                        else
                            exportedDatarow.Add(cell.NumericCellValue.ToString());

                    else
                        exportedDatarow.Add(cell.StringCellValue);
                }
                excelExportList.Add(exportedDatarow);
            }

            if (isclientname)
            {
                name = sheet.GetRow(1).GetCell(1).StringCellValue;
            }

            if (username)
            {
                name = sheet.GetRow(rownum: 0).GetCell(1).StringCellValue;
            }

            headerList = excelExportList[0];
            excelExportList.RemoveAt(0);
        }

        public static void ReadExcelSheetValueWithMergedCell(string fileName, string sheetName, int startrow, int endrow, out List<string> mergedCellHeader, out List<List<string>> excelExportList)
        {
            XSSFWorkbook Workbook;
            List<string> exportedDatarow;
            excelExportList = new List<List<string>>();

            using (FileStream file = new FileStream(@"C:/Users/i11143/Downloads/" + fileName, FileMode.Open, FileAccess.Read))


            {
                Workbook = new XSSFWorkbook(file);
            }


            ISheet sheet = Workbook.GetSheet(sheetName);

            for (int row = startrow; row < sheet.LastRowNum - endrow; row++)
            {
                IRow rowindex = sheet.GetRow(row);
                exportedDatarow = new List<string>();
                foreach (var cell in rowindex.Cells)
                {

                    if (cell.CellType == CellType.Numeric)
                        if (DateUtil.IsCellDateFormatted(cell))
                            exportedDatarow.Add(cell.DateCellValue.ToString());

                        else
                            exportedDatarow.Add(cell.NumericCellValue.ToString());

                    else
                        exportedDatarow.Add(cell.StringCellValue);
                    exportedDatarow = exportedDatarow.Where(x => !string.IsNullOrEmpty(x)).ToList();
                }

                excelExportList.Add(exportedDatarow);
                excelExportList = excelExportList.Where(x => x.Count != 0).ToList();
            }

            mergedCellHeader = excelExportList[1];



        }

        public static int GetLastColOfExcel(string fileName, string sheetName)
        {

            XSSFWorkbook Workbook;
            using (FileStream file = new FileStream(@"C:/Users/i11143/Downloads/" + fileName, FileMode.Open, FileAccess.Read))
            {
                Workbook = new XSSFWorkbook(file);
            }
            ISheet sheet = Workbook.GetSheet(sheetName);
            IRow rowindex = sheet.GetRow(1);
            return rowindex.LastCellNum;

        }



        public static void DeleteExcelFileIfAlreadyExists(string fileName)
        {
            try
            {
                if (fileName.Contains(".xlsx"))
                {
                    var filestream = new FileStream(@"C:/Users/i11143/Downloads/" + fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    filestream.Close();
                    File.Delete(@"C:/Users/i11143/Downloads/" + fileName);
                }
                else
                {
                    var filestream = new FileStream(@"C:/Users/i11143/Downloads/" + fileName + ".xlsx", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    filestream.Close();
                    File.Delete(@"C:/Users/i11143/Downloads/" + fileName + ".xlsx");
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("File to be deleted does not exist");
            }
        }

        public static void DeleteFileIfAlreadyExists(string fileName)
        {
            var fileInfo = new FileInfo(@"C:/Users/i11143/Downloads/" + fileName);

            if (fileInfo.Exists)
            {
                var filestream = new FileStream(@"C:/Users/i11143/Downloads/" + fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                filestream.Close();
                File.Delete(@"C:/Users/i11143/Downloads/" + fileName);
            }
        }
    }
}




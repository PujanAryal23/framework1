using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Microsoft.Office.Interop.Excel;

namespace NewRelicTestFramework
{
    public class Excel
    {
        public static void ExportErrorReportToExcel(IList<ErrorsList> ErrorList, BackgroundWorker backgroundWorker,Environment env)
        {
            Application xlApp = new Microsoft.Office.Interop.Excel.Application();

            if (xlApp == null)
            {
                Console.WriteLine("EXCEL could not be started. Check that your office installation and project references are correct.");
                return;
            }
            xlApp.Visible = true;

            Workbook wb = xlApp.Workbooks.Add(XlWBATemplate.xlWBATWorksheet);
            Worksheet ws = (Worksheet)wb.Worksheets[1];

            if (ws == null)
            {
                Console.WriteLine("Worksheet could not be created. Check that your office installation and project references are correct.");
            }

            ws.Columns["D"].ColumnWidth = 73.71;
            ws.Columns["B"].ColumnWidth = 24.14;
            ws.Columns["C"].ColumnWidth = 34.14;
            ws.Cells[1, 1] = "Total Count : " + ErrorList.Sum(c => c.ErrorCount).ToString();
            ws.Cells[2, 1] = "New";
            ws.Cells[2, 2] = "Issue";
            ws.Cells[2, 3] = "Source";
            ws.Cells[2, 4] = "Description";
            ws.Cells[2, 5] = "Error Count";
            ws.Cells[2, 6] = "Status";
            ws.Cells[2, 7] = "Note";
            ws.Cells[2, 8] = "Link";

            ws.Range["A1", "H1"].Font.Bold = true;
            ws.Range["A1", "H1"].VerticalAlignment = XlVAlign.xlVAlignCenter;

            int index = 1;
            int process = ErrorList.Count;
            int row = 3;
            foreach (var error in ErrorList)
            {
                backgroundWorker.ReportProgress(index++ * 100 / process);
                int column = 1;
                ws.Cells[row, column++].value = error.IsNew;
                ws.Cells[row, column++].value = error.Issue;
                ws.Cells[row, column++].value = error.Source;
                ws.Cells[row, column++].value = error.Description;
                ws.Cells[row, column++].value = error.ErrorCount;
                ws.Cells[row, column++].value = error.Status;
                ws.Cells[row, column++].value = error.Note;
                ws.Hyperlinks.Add(ws.Cells[row, column++], error.ErrorPermalink, Type.Missing, Type.Missing, "New Relic");
                row++;

            }
            
            string path= System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile)+"\\downloads";
            var date = DateTime.Now;
            string name = String.Format("{0}\\ErrorReport_{1} {2}-{3}-{4}.xls",path,env, date.Month, date.Day, date.Year);
            xlApp.Visible = false;            
            wb.SaveAs(name, XlFileFormat.xlWorkbookDefault, Type.Missing, Type.Missing, false, false, XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            wb.Close();
        }

        public static void ExportTransactionReportToExcel(IList<TransactionTraces> slowTransactionList, BackgroundWorker backgroundWorker,Environment env)
        {

            Application xlApp = new Microsoft.Office.Interop.Excel.Application();

            if (xlApp == null)
            {
                Console.WriteLine("EXCEL could not be started. Check that your office installation and project references are correct.");
                return;
            }
            xlApp.Visible = true;

            Workbook wb = xlApp.Workbooks.Add(XlWBATemplate.xlWBATWorksheet);
            Worksheet ws = (Worksheet)wb.Worksheets[1];

            if (ws == null)
            {
                Console.WriteLine("Worksheet could not be created. Check that your office installation and project references are correct.");
            }

            ws.Columns["D"].ColumnWidth = 73.71;
            ws.Columns["B"].ColumnWidth = 24.14;
            ws.Columns["C"].ColumnWidth = 34.14;
            ws.Cells[1, 1] = "Issue Type";
            ws.Cells[1, 2] = "SlowPage";
            ws.Cells[1, 3] = "Count";
            ws.Cells[1, 4] = "Notes";

            ws.Range["A1", "H1"].Font.Bold = true;
            ws.Range["A1", "H1"].VerticalAlignment = XlVAlign.xlVAlignCenter;


            int index = 1;
            int process = slowTransactionList.Count;
            int row = 2;
            foreach (var transaction in slowTransactionList)
            {
                backgroundWorker.ReportProgress(index++ * 100 / process);
                int column = 1;
                ws.Cells[row, column++].value = transaction.IssueType;
                ws.Cells[row, column++].value = transaction.SlowPage;
                ws.Cells[row, column++].value = transaction.Count;
                ws.Cells[row, column++].value = transaction.Notes;
                row++;

            }
            string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile) + "\\downloads";
            var date = DateTime.Now;
            string name = String.Format("{0}\\TransationReport{1} {2}-{3}-{4}.xls", path, env, date.Month, date.Day, date.Year);
            
            xlApp.Visible = false;
            wb.SaveAs(name, XlFileFormat.xlWorkbookDefault, Type.Missing, Type.Missing, false, false, XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            wb.Close();
        }

    }
}

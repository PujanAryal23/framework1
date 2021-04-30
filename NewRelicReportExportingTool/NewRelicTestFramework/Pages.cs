using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewRelicTestFramework.Repositories;

namespace NewRelicTestFramework
{
    public static class Pages
    {

        static string Url = "http://newrelic.com";
        public static void GenerateProductionErrorReport(DateTime ReportDate)
        {
            Browser.GenerateProductionErrorReport(Url, ReportType.Error, Environment.PROD, ReportDate);
        }

        public static void GenerateUATErrorReport(DateTime ReportDate)
        {
            Browser.GenerateUATErrorReport(Url, ReportType.Error, Environment.UAT, ReportDate);
        }

        public static void GenerateProdTransactionReport(DateTime ReportDate)
        {
            Browser.GenerateProdTransactionReport(Url, ReportType.Transaction, Environment.PROD, ReportDate);
        }

        public static void GenerateUATTransactionReport(DateTime ReportDate)
        {
            Browser.GenerateUATTransactionReport(Url, ReportType.Transaction, Environment.UAT, ReportDate);
        }


        public static void ExportProdErrorReportToExcel()
        {
            //var errorList = ErrorReportRepository.GetErrorReport(ReportType.Error, Environment.PROD);
           // Excel.ExportErrorReportToExcel(errorList);
        }

        public static void ExportUATErrorReportToExcel()
        {
            //var errorList = ErrorReportRepository.GetErrorReport(ReportType.Error, Environment.UAT);
            //Excel.ExportErrorReportToExcel(errorList);
        }


        public static void ExportProdTransactionReportToExcel()
        {
            //var transactionList = TransactionReportRepository.GetTransactionReport(ReportType.Transaction, Environment.PROD);
            //Excel.ExportTransactionReportToExcel(transactionList);
        }

        public static void ExportUATTransactionReportToExcel()
        {
            //List<TransactionTraces> transactionList = TransactionReportRepository.GetTransactionReport(ReportType.Transaction, Environment.UAT);
            //Excel.ExportTransactionReportToExcel(transactionList);

        }

       
    }

}
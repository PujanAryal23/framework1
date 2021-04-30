using NewRelicTestFramework.DataBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewRelicTestFramework.Common;
using System.Windows.Forms;

namespace NewRelicTestFramework.Repositories
{
    public class TransactionReportRepository
    {
        public static void SaveTransactionReport(IList<TransactionTraces> transactionList, ReportType errorType, Environment production, string permalink, DateTime reportDate)
        {
            try
            {
                var sqlList = new List<string>();
                var seq = DatabaseConnection.GetSequence("SEQ_REPORTCODE");
                var sqlstring = @"INSERT INTO NEWRELIC_REPORT (REPORTSEQ,PERMALINK,ENVIRONMENT,REPORT_TYPE,TOTALCOUNT,CREATE_DATE,REPORT_DATE) VALUES ";
                sqlstring += String.Format("({0},'{1}','{2}','{3}',{4},'{5}','{6}')", seq, permalink, production, errorType.GetStringValue(), 0, DateTime.Now.ToString("dd-MMM-yyyy"), reportDate.ToString("dd-MMM-yyyy"));
                sqlList.Add(sqlstring);
                foreach (var transaction in transactionList)
                {
                    var tranSeq = DatabaseConnection.GetSequence("SEQ_TRANSACTIONSEQ");
                    var insertErrorSql = @"INSERT INTO NEWRELIC_TRANSACTION_REPORT (REPORTSEQ,TRANSACTIONSEQ,ISSUE_TYPE,PAGE,COUNT,NOTES) VALUES";

                    insertErrorSql += String.Format("({0},{1},'{2}','{3}',{4},'{5}')", Convert.ToInt64(seq), Convert.ToInt64(tranSeq), transaction.IssueType, transaction.SlowPage, transaction.Count, transaction.Notes);
                    sqlList.Add(insertErrorSql);
                }
                DatabaseConnection.ExecuteMultipleQueries(sqlList);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " : Database cannot be connected!!!", "Error");
                throw;
            }
        }

        public static List<TransactionTraces> GetTransactionReport(ReportType reportType, Environment environment, object createDate)
        {
            var transactionList = new List<TransactionTraces>();
            try
            {
                var _reportType = reportType.GetStringValue();
                var sqlString = string.Format(@"SELECT * FROM NEWRELIC_TRANSACTION_REPORT WHERE REPORTSEQ IN (SELECT REPORTSEQ FROM NEWRELIC_REPORT WHERE REPORT_TYPE='{0}' AND ENVIRONMENT='{1}' AND REPORT_DATE='{2}')", _reportType, environment, createDate);
                transactionList = DatabaseConnection.GetTable(sqlString).AsEnumerable()
                    .Select(x => new TransactionTraces()
                    {
                        SlowPage = x.Field<string>("PAGE"),
                        Count = (int)x.Field<long>("COUNT"),
                        IssueType = x.Field<string>("ISSUE_TYPE"),
                        Notes = x.Field<string>("NOTES")
                    })
                    .ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " : Database cannot be connected!!!", "Error");
                throw ex;
            }
            return transactionList;
        }
    }
}
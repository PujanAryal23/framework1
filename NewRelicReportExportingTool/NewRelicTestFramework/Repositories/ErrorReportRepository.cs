using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewRelicTestFramework.DataBase;
using NewRelicTestFramework.Common;
using System.Windows.Forms;

namespace NewRelicTestFramework.Repositories
{
    public static class ErrorReportRepository
    {
        public static void SaveErrorReport(IList<ErrorsList> errorList, ReportType errorType, Environment production, string permalink, DateTime reportDate)
        {
            try
            {
                var totalCount = errorList.Sum(c => c.ErrorCount);
                var sqlList = new List<string>();
                var seq = DatabaseConnection.GetSequence("SEQ_REPORTCODE");
                var sqlstring = @"INSERT INTO NEWRELIC_REPORT (REPORTSEQ,PERMALINK,ENVIRONMENT,REPORT_TYPE,TOTALCOUNT,CREATE_DATE,REPORT_DATE) VALUES ";
                sqlstring += String.Format("({0},'{1}','{2}','{3}',{4},'{5}','{6}')", seq, permalink, production, errorType.GetStringValue(), totalCount, DateTime.Now.ToString("dd-MMM-yyyy"), reportDate.ToString("dd-MMM-yyyy"));
                sqlList.Add(sqlstring);
                foreach (var error in errorList)
                {
                    var errSeq = DatabaseConnection.GetSequence("SEQ_ERRORSEQ");
                    var insertErrorSql = @"INSERT INTO NEWRELIC_ERROR_REPORT (REPORTSEQ,ERRORSEQ,ISNEW,ISSUE,ERROR_SOURCE,DESCRIPTION,ERROR_COUNT,STATUS,NOTES,BEGIN_DATE,END_DATE,PERMALINK) VALUES";

                    insertErrorSql += String.Format("({0},{1},'{2}','{3}','{4}','{5}',{6},'{7}','{8}','{9}','{10}','{11}')", Convert.ToInt64(seq), Convert.ToInt64(errSeq), error.IsNew, error.Issue, error.Source, error.Description, error.ErrorCount, error.Status, error.Note, error.BeginDate, error.EndDate, error.ErrorPermalink);
                    sqlList.Add(insertErrorSql);
                }
                DatabaseConnection.ExecuteMultipleQueries(sqlList);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " : Database cannot be connected!!!", "Error");

            }
            finally
            {
                DatabaseConnection.CloseConnection();
            }

        }

        public static List<ErrorsList> GetErrorReport(ReportType reportType, Environment environment, object createDate)
        {
            var errorList = new List<ErrorsList>();
            try
            {
                var _reportType = StringEnumerator.GetStringValue(reportType);
                var sqlString = String.Format(@"SELECT * FROM NEWRELIC_ERROR_REPORT WHERE REPORTSEQ IN (SELECT REPORTSEQ FROM NEWRELIC_REPORT WHERE REPORT_TYPE='{0}' AND ENVIRONMENT='{1}' AND REPORT_DATE='{2}')", _reportType, environment, createDate);
                errorList = DatabaseConnection.GetTable(sqlString).AsEnumerable()
                   .Select(x => new ErrorsList()
                   {
                       IsNew = x.Field<string>("ISNEW"),
                       Description = x.Field<string>("DESCRIPTION"),
                       Status = x.Field<string>("STATUS"),
                       Issue = x.Field<string>("ISSUE"),
                       Source = x.Field<string>("ERROR_SOURCE"),
                       ErrorPermalink = x.Field<string>("PERMALINK"),
                       Note = x.Field<string>("NOTES"),
                       ErrorCount = (int)x.Field<long>("ERROR_COUNT")
                   }).OrderByDescending(c => c.ErrorCount)
                   .ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " : Database cannot be connected!!!", "Error");
                throw;
            }
            return errorList;
        }

        public static List<string> GetReportDates()
        {
            try
            {
                string sqlstring = @"  SELECT DISTINCT REPORT_DATE FROM NEWRELIC_REPORT ORDER BY REPORT_DATE";
                var dateList = DatabaseConnection.GetTable(sqlstring).AsEnumerable()
                    .Select(x => x.Field<DateTime>("REPORT_DATE").ToString("dd-MMM-yy"))
                    .ToList();
                return dateList;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " : Database cannot be connected!!!", "Error");
                return new List<string>();
            }
        }

        public static List<NewRelicReports> GetReports()
        {
            var reportList = new List<NewRelicReports>();
            using (var con = DatabaseConnection.GetConnection())
            {                
                var sqlString = String.Format(@"SELECT * FROM NEWRELIC_REPORT");
                reportList = DatabaseConnection.GetTable(sqlString).AsEnumerable()
                   .Select(x => new NewRelicReports()
                   {
                       ReportType=x.Field<string>("REPORT_TYPE"),
                       Environments=x.Field<string>("ENVIRONMENT"),
                       ReportDate = x.Field<DateTime>("REPORT_DATE")
                   })
                   .ToList();
            }
            return reportList;
        }
    }
}
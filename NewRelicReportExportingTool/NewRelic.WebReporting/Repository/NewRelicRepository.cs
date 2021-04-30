using System;
using System.Collections.Generic;
using NewRelic.WebReporting.Models;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace NewRelic.WebReporting.Repository
{
    public class NewRelicRepository
    {
        public List<Reports> GetReports()
        {
            List<Reports> reports = new List<Reports>();
            using (var connection = DatabaseConnection.GetConnection())
            {
                try
                {
                    OracleCommand command = new OracleCommand("SELECT * FROM NEWRELIC_REPORT", connection);
                    command.CommandType = CommandType.Text;
                    OracleDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        reports.Add(new Reports
                        {
                            ReportSeq = reader["REPORTSEQ"] as int?,
                            Permalink = reader["PERMALINK"].ToString(),
                            Environment = reader["ENVIRONMENT"].ToString(),
                            ReportType = reader["REPORT_TYPE"].ToString(),
                            TotalCount = reader["TOTALCOUNT"] as int?,
                            CreateDate = reader["CREATE_DATE"].ToString()
                        });
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    connection.Close();

                }
            }
            return reports;
        }
        public List<ErrorsList> GetErrorsList()
        {
            List<ErrorsList> errorsList = new List<ErrorsList>();
            using (var connection = DatabaseConnection.GetConnection())
            {
                try
                {
                    OracleCommand command = new OracleCommand("SELECT * FROM NEWRELIC_ERROR_REPORT", connection);
                    command.CommandType = CommandType.Text;
                    OracleDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        errorsList.Add(new ErrorsList
                        {
                            ReportSeq = reader["REPORTSEQ"] as int?,
                            ErrorSeq = reader["ERRORSEQ"] as int?,
                            IsNew = reader["ISNEW"].ToString(),
                            Issue = reader["ISSUE"].ToString(),
                            ErrorSource = reader["ERROR_SOURCE"].ToString(),
                            Description = reader["DESCRIPTION"].ToString(),
                            ErrorCount = reader["ERROR_COUNT"] as int?,
                            Status = reader["STATUS"].ToString(),
                            Notes = reader["NOTES"].ToString(),
                            BeginDate = reader["BEGIN_DATE"].ToString(),
                            EndDate = reader["END_DATE"].ToString(),
                            Permalink = reader["PERMALINK"].ToString()
                        });
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    connection.Close();
                }
            }
            return errorsList;
        }
        public List<TransactionTraces> GetTransactionTraces()
        {
            List<TransactionTraces> transactionTraces = new List<TransactionTraces>();
            using (var connection = DatabaseConnection.GetConnection())
            {
                try
                {
                    OracleCommand command = new OracleCommand("SELECT * FROM NEWRELIC_TRANSACTION_REPORT", connection);
                    command.CommandType = CommandType.Text;
                    OracleDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        transactionTraces.Add(new TransactionTraces
                        {
                            ReportSeq = reader["REPORTSEQ"] as int?,
                            TransactionSeq = reader["TRANSACTIONSEQ"] as int?,
                            IssueType = reader["ISSUE_TYPE"].ToString(),
                            Page = reader["PAGE"].ToString(),
                            Count = reader["COUNT"] as int?,
                            Notes = reader["NOTES"].ToString(),
                        });
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    connection.Close();
                }
            }
            return transactionTraces;
        }
    }
}
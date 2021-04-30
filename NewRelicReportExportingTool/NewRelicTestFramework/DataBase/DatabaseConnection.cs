using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Configuration;

namespace NewRelicTestFramework.DataBase
{
    public static class DatabaseConnection
    {
        private static OracleConnection _connection;

        private static void OpenConnection()
        {
            try
            {
                if (_connection.State != ConnectionState.Open)
                    _connection.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public static void CloseConnection()
        {
            try
            {
                if (_connection.State == ConnectionState.Open)
                    _connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        public static OracleConnection GetConnection()
        {
            _connection = new OracleConnection(GetConnectionString());
            OpenConnection();
            return _connection;
        }

        private static string GetConnectionString()
        {
            try
            {
                var oracleServer = new OracleConnectionStringBuilder();
                var dataSource = ConfigurationManager.AppSettings["OracleDataSource"];
                var userId = ConfigurationManager.AppSettings["OracleUserId"];
                var password = ConfigurationManager.AppSettings["OraclePassword"];
                oracleServer.DataSource = dataSource;
                oracleServer.UserID = userId;
                oracleServer.Password = password;
                return oracleServer.ToString();
            }
            catch (Exception ex)
            {

                throw ex;
            }


        }

        public static DataTable GetTable(string sqlstring)
        {
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            using (var con = GetConnection())
            {
                OracleCommand command = new OracleCommand(sqlstring, con);
                command.CommandType = CommandType.Text;
                OracleDataAdapter dataAdapter = new OracleDataAdapter(command);
                dataAdapter.Fill(ds);
            }
            dt = ds.Tables[0];
            return dt;

        }

        public static long GetSequence(string sequenceName)
        {
            string sql = string.Format(" SELECT {0}.NEXTVAL FROM dual", sequenceName);
            return GetSingleValue(sql);
        }


        public static long GetSingleValue(string sqlString)
        {
            using (var con = GetConnection())
            {
                OracleCommand command = new OracleCommand(sqlString, con);
                command.CommandType = CommandType.Text;
                return Convert.ToInt64(command.ExecuteScalar());
            }
        }

        public static void ExecuteQuery(string sqlstring)
        {
            using (var con = GetConnection())
            {
                OracleCommand command;
                OracleTransaction transaction;
                command = new OracleCommand(sqlstring, con);
                // Start a local transaction
                transaction = con.BeginTransaction(IsolationLevel.ReadCommitted);
                // Assign transaction object for a pending local transaction
                command.Transaction = transaction;
                command.CommandType = CommandType.Text;
                try
                {
                    command.ExecuteNonQuery();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    CloseConnection();
                }
            }
        }

        public static void ExecuteMultipleQueries(IList<string> queries)
        {
            var sqlBuilder = new StringBuilder("BEGIN ");
            sqlBuilder.Append(string.Join("; ", queries));
            sqlBuilder.Append("; END;");
            using (var con = GetConnection())
            {
                var command = new OracleCommand(sqlBuilder.ToString(), _connection);
                var transaction = _connection.BeginTransaction(IsolationLevel.ReadCommitted);
                command.Transaction = transaction;
                command.CommandType = CommandType.Text;
                try
                {
                    command.ExecuteNonQuery();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }
    }
}
using System;
using System.Data;
using System.Web;
using Oracle.ManagedDataAccess.Client;
using System.Configuration;

namespace NewRelic.WebReporting.Repository
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
            var connectionString = new OracleConnectionStringBuilder();
            connectionString.DataSource = ConfigurationManager.AppSettings["OracleDataSource"];
            connectionString.UserID = ConfigurationManager.AppSettings["OracleUserId"];
            connectionString.Password = ConfigurationManager.AppSettings["OraclePassword"];
            return connectionString.ToString();
        }
    }
}
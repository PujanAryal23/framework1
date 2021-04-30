using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;

namespace UIAutomation.Framework.Database
{
    public class NewDatabaseConnection : IDatabaseConnection
    {
        private OracleConnection _connection;

        #region CONSTRUCTOR
        public NewDatabaseConnection()
        {
            _connection = new OracleConnection(GetConnectionString());
        }
        #endregion
        public void OpenConnection()
        {
            try
            {
                if (ConfigurationManager.AppSettings["OracleUserId"] == "TEST") return;
                if (_connection.State != ConnectionState.Open)
                    _connection.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void CloseConnection()
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

        public OracleConnection GetConnection()
        {
            OpenConnection();
            return _connection;
        }


        public string GetConnectionString()
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

    }
}
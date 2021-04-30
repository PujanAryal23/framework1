using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Oracle.ManagedDataAccess.Client;

namespace UIAutomation.Framework.Database
{
    public class DatabaseConnection
    {
        private static OracleConnection _connection;

        static DatabaseConnection()
        {
            _connection = new OracleConnection(GetConnectionString());
        }

        private static void OpenConnection()
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


        public static string GetConnectionString()
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




        //public static void Connect()
        //{
        //    string DataSource = ConfigurationManager.AppSettings["OracleDataSource"];
        //    string UserId = ConfigurationManager.AppSettings["OracleUserId"];
        //    string Password = ConfigurationManager.AppSettings["OraclePassword"];
        //    Console.WriteLine(DataSource + "    " + UserId + "    " + Password);
        //    OracleConnection _connection = new OracleConnection();
        //    OracleConnectionStringBuilder _oracleServer = new OracleConnectionStringBuilder();
        //    try
        //    {
        //        _oracleServer.DataSource = DataSource;
        //        _oracleServer.UserID = UserId;
        //        _oracleServer.Password = Password;
        //        string sqlCommand = "select * from smtst.hciflag where claseq=1479275 and   clasub=0";
        //        _connection = new OracleConnection(_oracleServer.ToString());
        //        OracleCommand command = new OracleCommand(sqlCommand, _connection);
        //        command.CommandType = CommandType.Text;
        //        _connection.Open();

        //        OracleDataAdapter dr = new OracleDataAdapter(command);
        //        DataSet ds = new DataSet();
        //        dr.Fill(ds);
        //        Console.WriteLine(ds.Tables[0].Rows[0][0].ToString());

        //    }
        //    finally
        //    {
        //        if (_connection.State == ConnectionState.Open)
        //        {
        //            Console.WriteLine("open");
        //            _connection.Close();
        //            _connection.Dispose();
        //        }
        //    }
        //}

    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;

namespace UIAutomation.Framework.Database
{
    public class NewOracleStatementexecutor : IOracleStatementExecutor
    {
        public DataSet ObjData;
        private readonly IDatabaseConnection _dbConnection;

        #region CONSTRUCTOR
        public NewOracleStatementexecutor(IDatabaseConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        #endregion

        public List<string> GetTableSingleColumn(string sqlstring, int col = 0)
        {
            ObjData = new DataSet();
            try
            {
                var con = _dbConnection.GetConnection();
                var command = new OracleCommand(sqlstring, con);
                command.CommandType = CommandType.Text;
                var dataAdapter = new OracleDataAdapter(command);
                dataAdapter.Fill(ObjData);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            if (ObjData == null || ObjData.Tables.Count == 0 || ObjData.Tables[0].Rows.Count == 0)
                return null;
            return (ObjData.Tables[0]).Select().Where(x => !x.IsNull(0)).CopyToDataTable().AsEnumerable()
                    .Select(x => x[col].ToString()).ToList();


        }

        public long GetSingleValue(string sqlstring)
        {
            long count = 0;
            try
            {
                var con = _dbConnection.GetConnection();
                OracleCommand command = new OracleCommand(sqlstring, con);
                command.CommandType = CommandType.Text;
                count = Convert.ToInt64(command.ExecuteScalar());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return count;
        }

        public string GetSingleStringValue(string sqlstring)
        {
            string value = "";
            try
            {
                var con = _dbConnection.GetConnection();
                OracleCommand command = new OracleCommand(sqlstring, con);
                command.CommandType = CommandType.Text;
                value = command.ExecuteScalar().ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return value;
        }

        public string GetFileBlobValue(string sqlstring)
        {
            byte[] value;
            var logoImage = " ";
            try
            {
                var con = _dbConnection.GetConnection();
                OracleCommand command = new OracleCommand(sqlstring, con);
                command.CommandType = CommandType.Text;
                value = (byte[])command.ExecuteScalar();
                logoImage = (value != null) ? string.Format("data:image/gif;base64,{0}", Convert.ToBase64String(value)) : null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return logoImage;
        }

        public IEnumerable<DataRow> GetCompleteTable(string sqlstring)
        {
            ObjData = new DataSet();
            try
            {
                var con = _dbConnection.GetConnection();
                var command = new OracleCommand(sqlstring, con);
                command.CommandType = CommandType.Text;
                var dataAdapter = new OracleDataAdapter(command);
                dataAdapter.Fill(ObjData, "TableObject");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            if (ObjData == null || ObjData.Tables.Count == 0 || ObjData.Tables[0].Rows.Count == 0)
                return null;
            else
                return (ObjData.Tables["TableObject"]).Select()
                          .Where(x => !x.IsNull(0))
                          .CopyToDataTable()
                          .AsEnumerable();

            //return (_objData.Tables[0]).AsEnumerable()
            //    .Select(x => x[0].ToString()).ToList();
        }

        public IEnumerable<DataRow> GetCompleteTableWithNullValues(string sqlstring)
        {
            ObjData = new DataSet();
            try
            {
                var con = _dbConnection.GetConnection();
                var command = new OracleCommand(sqlstring, con);
                command.CommandType = CommandType.Text;
                var dataAdapter = new OracleDataAdapter(command);
                dataAdapter.Fill(ObjData, "TableObject");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return (ObjData.Tables["TableObject"]).Select()
                    .CopyToDataTable()
                    .AsEnumerable();

        }
        public void ExecuteQuery(string sqlstring)
        {
            try
            {
                var con = _dbConnection.GetConnection();
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

            }

            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }
            finally
            {
                _dbConnection.CloseConnection();
            }
        }

        public void ExecuteQueryAsync(string sqlstring)
        {

            Task.Factory.StartNew(
                () =>
                {
                    try
                    {
                        var con = _dbConnection.GetConnection();
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




                    }

                    catch (Exception ex)
                    {

                        Console.WriteLine(ex.Message);
                    }

                });
        }

        public void CloseConnection()
        {
            _dbConnection.CloseConnection();
        }

    }
}


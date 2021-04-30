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
    public interface IDatabaseConnection
    {
        #region PUBLIC METHODS
        void OpenConnection();
        void CloseConnection();
        OracleConnection GetConnection();
        string GetConnectionString();
        #endregion
    }
}
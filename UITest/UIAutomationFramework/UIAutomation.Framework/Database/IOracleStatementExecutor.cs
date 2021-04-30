using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;

namespace UIAutomation.Framework.Database
{
    public interface IOracleStatementExecutor
    {
        List<string> GetTableSingleColumn(string sqlstring, int col = 0);

        long GetSingleValue(string sqlstring);

        string GetSingleStringValue(string sqlstring);

        string GetFileBlobValue(string sqlstring);

        IEnumerable<DataRow> GetCompleteTable(string sqlstring);

        IEnumerable<DataRow> GetCompleteTableWithNullValues(string sqlstring);

        void ExecuteQuery(string sqlstring);

        void ExecuteQueryAsync(string sqlstring);

        void CloseConnection();

    }
}
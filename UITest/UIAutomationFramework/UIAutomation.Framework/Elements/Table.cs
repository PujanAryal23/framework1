using System.Linq;
using System.Collections.Generic;
using OpenQA.Selenium;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Driver;

namespace UIAutomation.Framework.Elements
{
    /// <summary>
    /// Provides a convenience method for retrieving values from an HTML table element.
    /// </summary>
    public class Table : BaseElement
    {
        /// <summary>
        /// Initializes a new instance of the Table class.
        /// </summary>
        public Table(bool cache, How select, string selector)
            : base(cache, select, selector, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the Table class.
        /// </summary>
        public Table(bool cache, How select, string selector, ISearchContext context)
            : base(cache, select, selector, context)
        {
        }

        /// <summary>
        /// Gets a list of header names of a specified table.
        /// </summary>
        /// <param name="index">The number of rows to skip.</param>
        /// <returns>A list of header names.</returns>
        public List<string> GetHeader(int index = 0)
        {
            var header = SiteDriver.FindElement("thead", How.TagName, ProxyWebElement);
            var row = SiteDriver.FindElements("tr", How.TagName, header).ElementAt(index);
            var headers = SiteDriver.FindElements("th", How.TagName, row);
            headers = headers.Where(col => col.Text != "");
            return headers.Select(col => col.Text).ToList();
        }

        /// <summary>
        /// Gets a list of values from a specified row.
        /// </summary>
        /// <param name="index">The specified index of a row.</param>
        /// <returns>The list of values from the row</returns>
        public List<string> GetRow(int index)
        {
            var body = SiteDriver.FindElement("tbody", How.TagName, ProxyWebElement);
            var row = SiteDriver.FindElements("tr", How.TagName, body).ElementAt(index);
            var cols = SiteDriver.FindElements("td", How.TagName, row);
            return cols.Select(col => col.Text).ToList();
        }

        public List<string> GetColumn(int index)
        {
            var body = SiteDriver.FindElement("tbody", How.TagName, ProxyWebElement);
            var rows = SiteDriver.FindElements("tr", How.TagName, body);
            return rows.Select(x => SiteDriver.FindElements("td", How.TagName, x).Count() > index ?
                    SiteDriver.FindElements("td", How.TagName, x).ElementAt(index).Text : " ").Where(y => y != " ").ToList();
        }

        /// <summary>
        /// Gets a row and its columns values.
        /// </summary>
        /// <param name="skip">The number of rows to skip.</param>
        /// <returns>The key value pairs of a row and its column values.</returns>
        public Dictionary<int, List<string>> GetRows(int skip = 0)
        {
            return GetTableCols(GetTableRows(skip));
        }

        /// <summary>
        /// Gets a number of rows count.
        /// </summary>
        /// <param name="skip">The number of rows to skip.</param>
        /// <returns>Total rows count.</returns>
        public int GetRowCount(int skip = 0)
        {
            return GetTableRows(skip).Count();
        }

        /// <summary>
        /// Gets a list of column values from a particular row having the element is present.
        /// </summary>
        /// <param name="select">The element to check whether it presents or not in a row.</param>
        /// <param name="selector">The locating mechanism to use.</param>
        /// <param name="skip">The number of rows to skip.</param>
        /// <returns>The list of column values.</returns>
        public List<string> GetRowDataFor(string select, How selector, int skip = 0)
        {
            var result = new List<string>();
            for (int i = 1; i <= GetTableRows(skip).Count();i++)
            {
                var row = SiteDriver.FindElement(string.Format(".//tr[{0}]", i), How.XPath, ProxyWebElement);
                if (!SiteDriver.IsElementPresent(select, selector, row)) continue;
                var tablecols = SiteDriver.FindElements("td", How.TagName, row);
                result = tablecols.Select(col => col.Text).ToList();
                break;
            }
            return result;
        }

        /// <summary>
        /// Gets a single value for a specified row and column inex.
        /// </summary>
        /// <param name="rowIndex">The specified index of a row.</param>
        /// <param name="colIndex">The specified index of a column.</param>
        /// <returns>A single data.</returns>
        public string GetRowData(int rowIndex, int colIndex)
        {
            return GetRowDataFor(rowIndex, colIndex);
        }

        /// <summary>
        /// Returns a <see cref="Table"/> as row.
        /// </summary>
        /// <param name="rowIndex">The specified index of a row.</param>
        /// <returns>This Table.</returns>
        public Table GetRowFor(int rowIndex)
        {
            var body = SiteDriver.FindElement<Table>("tbody", How.TagName, this);
            return SiteDriver.FindElement<Table>(string.Format(".//tr[{0}]", rowIndex), How.XPath, body);
        }

        /// <summary>
        /// Gets a single value for a specified row contans and column inex.
        /// </summary>
        /// <param name="rowContains">The specified locating path of a row.</param>
        /// <param name="colIndex">The specified index of a column.</param>
        /// <returns>A single data.</returns>
        public string GetRowData(string rowContains, int colIndex)
        {
            return GetRowDataFor(rowContains, colIndex);
        }

        /// <summary>
        /// Gets a single value for a specified row contans and column inex.
        /// </summary>
        /// <param name="rowIndex">The specified index of a row.</param>
        /// <param name="colContains">The specified locating path of a column.</param>
        /// <returns>A single data.</returns>
        public string GetRowData(int rowIndex, string colContains)
        {
            return GetRowDataFor(rowIndex, colContains);
        }

        /// <summary>
        /// Gets a single value for a specified row and column contans.
        /// </summary>
        /// <param name="rowContains">The specified locating path of a row.</param>
        /// <param name="colContains">The specified locating path of a column.</param>
        /// <returns>A single data.</returns>
        public string GetRowData(string rowContains, string colContains)
        {
            return GetRowDataFor(rowContains, colContains);
        }

        /// <summary>
        /// Gets a list of Table as row.
        /// </summary>
        /// <param name="skip">The number of rows to skip.</param>
        /// <returns>The multiple tables.</returns>
        private IEnumerable<IWebElement> GetTableRows(int skip = 0)
        {
            var body = SiteDriver.FindElement("tbody", How.TagName, this.ProxyWebElement);
            return SiteDriver.FindElements("tr", How.TagName, body).Skip(skip);
        }

        /// <summary>
        /// Gets a key values pair of a Table.
        /// </summary>
        /// <param name="rows">A list of Table as rows.</param>
        /// <returns>A key value pairs of index and column values.</returns>
        private Dictionary<int, List<string>> GetTableCols(IEnumerable<IWebElement> rows)
        {
            var index = 0;
            var result = new Dictionary<int, List<string>>();
            foreach (var row in rows)
            {
                var cols = SiteDriver.FindElements("td", How.TagName, row);
                var list = cols.Select(col => col.Text).ToList();
                result[index++] = list;
            }
            return result;
        }

        /// <summary>
        /// Get data from particular row and column
        /// </summary>
        /// <param name="row">A locating path of a row.</param>
        /// <param name="column">A locating path of a column.</param>
        /// <returns>A single data.</returns>
        private string GetRowDataFor(object row, object column)
        {
            return SiteDriver.FindElement(string.Format(".//tbody/tr[{0}]/td[{1}]",row, column), How.XPath, ProxyWebElement).Text;
        }

        public bool IsCheckMarkPresent(object row, object column)
        {
            return SiteDriver.IsElementPresent(string.Format(".//tbody/tr[{0}]/td[{1}]/img", row, column), How.XPath, ProxyWebElement);
        }
    }
}

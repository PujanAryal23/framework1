using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Legacy.Service.PageObjects.Product;
using Legacy.Service.PageServices.Base;
using Legacy.Service.PageServices.Default;
using Legacy.Service.Support.Enum;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Core.Navigation;
using UIAutomation.Framework.Elements;

namespace Legacy.Service.PageServices.Product
{
    public class BatchStatisticsReportPage : DefaultPage
    {
        private BatchStatisticsReportPageObjects _batchStatisticsReportPage;

        public BatchStatisticsReportPage(INavigator navigator, BatchStatisticsReportPageObjects batchListPage)
            : base(navigator, batchListPage)
        {
            _batchStatisticsReportPage = (BatchStatisticsReportPageObjects) PageObject;
        }


        public override IPageService GoBack()
        {
            Navigator.Back();
           Console.WriteLine("Navigated back to Batch List Page");
            if (Navigator.CurrentUrl.StartsWith(PageObject.PageUrl))
                return this;
            return new BatchListPage(Navigator, new BatchListPageObjects());
        }

        /// <summary>
        /// Get row title of table
        /// </summary>
        /// <param name="tableIndex"></param>
        /// <returns></returns>
        public IList<string> GetRowTitleOfTable(string tableIndex)
        {
            return
                SiteDriver.FindElements(
                    string.Format(BatchStatisticsReportPageObjects.SpanTblRowTitleListXPath, tableIndex), How.XPath,
                    "Text");
        }

        /// <summary>
        /// Get headers in table
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public IList<string> GetHeadersOfTable(string index)
        {
            return
                SiteDriver.FindElements(string.Format(BatchStatisticsReportPageObjects.SpanTblHeaderListXPath, index),
                                        How.XPath, "Text").Where(x => x != " ")
                    .Select(s => s.Contains("\r\n") ? s.Replace("\r\n", " ") : s).ToList();
        }
    }
}

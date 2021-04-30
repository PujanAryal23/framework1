using System;
using System.Collections.Generic;
using System.Linq;
using Legacy.Service.PageObjects.Product;
using Legacy.Service.PageServices.Base;
using Legacy.Service.PageServices.Default;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Core.Navigation;
using UIAutomation.Framework.Elements;
using Legacy.Service.PageObjects.Default;
using UIAutomation.Framework.Utils;
using Legacy.Service.Support.Enum;

namespace Legacy.Service.PageServices.Product
{
    public class BatchListPage : DefaultPage
    {
        private BatchListPageObjects _batchListPage;

        public BatchListPage(INavigator navigator, DefaultPageObjects batchListPage)
            : base(navigator, batchListPage)
        {
            // Just for performance!
            _batchListPage = (BatchListPageObjects)PageObject;
        }


        public override IPageService GoBack()
        {
            Navigator.Back();
            if (Navigator.CurrentUrl.StartsWith(PageObject.PageUrl))
                return this;
            return new PhysicianClaimInsightPage(Navigator, new PhysicianClaimInsightPageObjects());
        }

        /// <summary>
        /// Click on stats link 
        /// </summary>
        /// <returns></returns>
        public BatchStatisticsReportPage ClickOnStatsLink()
        {
            var batchStatisticsReport = Navigator.Navigate<BatchStatisticsReportPageObjects>(() =>
                                {
                                    SiteDriver.FindElement<Link>(string.Format(BatchListPageObjects.StatsLinkXPath, 2), How.XPath).Click();
                                    Console.WriteLine("Navigated to Batch Statistics Report Page");
                                });
            return new BatchStatisticsReportPage(Navigator, batchStatisticsReport);
        }

        public FlaggedClaimPage ClickOnBatchIdLink(string batchId)
        {
            var flaggedClaim = Navigator.Navigate<FlaggedClaimPageObjects>(() =>
                                {
                                    SiteDriver.FindElement<Link>(string.Format(BatchListPageObjects.BatchIdLinkXPath, batchId), How.XPath).Click();
                                    Console.WriteLine(string.Format("Click on Batch Id '{0}'.", batchId));
                                    
                                });
            return new FlaggedClaimPage(Navigator, flaggedClaim);
        }

        public BatchListPage GetCurrentPage(out int currentPage)
        {
            string currentPageLinkText = string.Empty;
            _batchListPage = Navigator.Navigate<BatchListPageObjects>(() =>
                                                                          {
                                                                              currentPageLinkText = _batchListPage.CurrentPageLnk.Text;
                                                                          });
            currentPage = Convert.ToInt16(currentPageLinkText);
            return new BatchListPage(Navigator, _batchListPage);
        }

        public IEnumerable<string> GetListOfPageLinkNo()
        {
            return SiteDriver.FindElements(BatchListPageObjects.PageLnkListXPath, How.XPath, "Text");
        }

        public BatchListPage ClickOnPageLink(string pageNo)
        {
            _batchListPage = Navigator.Navigate(() =>
                                                    {
                                                        if (!_batchListPage.CurrentPageLnk.Text.Equals(pageNo))
                                                        {
                                                            SiteDriver.FindElement<Link>(string.Format(BatchListPageObjects.PageLnkToClickTemplate, pageNo), How.XPath).Click();
                                                            Console.Out.WriteLine("Click on Page Number : {0} ", pageNo);
                                                            
                                                        }
                                                    },
                                                () => new BatchListPageObjects(ProductPageUrlEnum.BatchMenu.GetStringValue()));
            return new BatchListPage(Navigator, _batchListPage);
        }

        public new BatchListPage ClickOnBackButton()
        {
            _batchListPage = Navigator.Navigate<BatchListPageObjects>(() =>
                                                                          {
                                                                              _batchListPage.BackButton.Click();
                                                                              Console.Out.WriteLine("Clicked on Back Button");
                                                                          });
            return new BatchListPage(Navigator, _batchListPage);

        }

    }
}

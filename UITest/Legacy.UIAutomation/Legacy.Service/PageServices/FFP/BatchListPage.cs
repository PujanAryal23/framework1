using System;
using Legacy.Service.PageObjects.FFP;
using Legacy.Service.PageServices.Base;
using Legacy.Service.PageServices.Default;
using Legacy.Service.Support.Utils;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Core.Navigation;
using Legacy.Service.PageObjects.Default;
using UIAutomation.Framework.Elements;
using Legacy.Service.Support.Enum;

namespace Legacy.Service.PageServices.FFP
{
    public class BatchListPage : DefaultPage
    {
        #region PRIVATE/PUBLIC FIELDS
		
        private BatchListPageObjects _batchListPage;
        
        #endregion

        #region CONSTRUCTORS

        public BatchListPage(INavigator navigator, DefaultPageObjects batchListPage)
            : base(navigator, batchListPage)
        {
            _batchListPage = (BatchListPageObjects)PageObject;
        }
        
        #endregion

        #region PUBLIC METHODS

        public override IPageService GoBack()
        {
            Navigator.Back();
            if (Navigator.CurrentUrl.StartsWith(PageObject.PageUrl))
                return this;
            return new FraudFinderProPage(Navigator, new FraudFinderProPageObjects());
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

        public BatchListPage ClickOnPageLink(string pageNo,out bool isPostBack)
        {
            bool isClicked = false;
            _batchListPage = Navigator.Navigate(() =>
            {
                 if (!_batchListPage.CurrentPageLnk.Text.Equals(pageNo))
                 {
                     {
                         SiteDriver.FindElement<Link>(
                             string.Format(BatchListPageObjects.PageLnkToClickTemplate, pageNo), How.XPath).Click();
                         SiteDriver.WaitForPageToLoad();
                         SiteDriver.WaitToLoadNew(2000);
                         Console.Out.WriteLine("Click on Page Number : {0} ", pageNo);
                         isClicked = true;
                     }
                 }},
                                                () => new BatchListPageObjects(FraudFinderProPageUrlEnum.BatchMenu.GetStringValue()));
            isPostBack = isClicked;
            
            return new BatchListPage(Navigator, _batchListPage);
        }

        public BatchListPage GetCurrentPage(out string pageNo,bool isClicked = false)
        {
            string currentPageNo="";
            _batchListPage = Navigator.Navigate(() =>
                                                        currentPageNo = CurrentPageUrl.Substring(CurrentPageUrl.IndexOf("Page=")).Trim(),
                                                    () => isClicked? new BatchListPageObjects(FraudFinderProPageUrlEnum.BatchMenu.GetStringValue()) : new BatchListPageObjects(FraudFinderProPageUrlEnum.Batchmenu.GetStringValue()));
            pageNo = currentPageNo;
            return new BatchListPage(Navigator, _batchListPage);
        }

        public new BatchListPage ClickOnBackButton()
        {
            _batchListPage = Navigator.Navigate<BatchListPageObjects>(() =>
            {
                _batchListPage.BackButton.Click();
                Console.Out.WriteLine(
                    "Clicked on Back Button");
            });
            return new BatchListPage(Navigator, _batchListPage);

        }

        #endregion
    }
}

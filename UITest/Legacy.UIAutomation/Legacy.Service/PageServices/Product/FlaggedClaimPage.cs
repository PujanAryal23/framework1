using System;
using System.Collections.Generic;
using Legacy.Service.PageObjects.Product;
using Legacy.Service.PageServices.Base;
using Legacy.Service.PageServices.Default;
using Legacy.Service.Support;
using Legacy.Service.Support.Enum;
using Legacy.Service.Support.Utils;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Core.Navigation;
using UIAutomation.Framework.Elements;

namespace Legacy.Service.PageServices.Product
{
    public class FlaggedClaimPage : DefaultPage
    {
        private FlaggedClaimPageObjects _flaggedClaimPage;

        public FlaggedClaimPage(INavigator navigator, FlaggedClaimPageObjects flaggedClaimPage)
            : base(navigator, flaggedClaimPage)
        {

            _flaggedClaimPage = (FlaggedClaimPageObjects)PageObject;
        }

        public override IPageService GoBack()
        {
            Navigator.Back();
            if (Navigator.CurrentUrl.StartsWith(PageObject.PageUrl))
                return this;
            return new BatchListPage(Navigator, new BatchListPageObjects());
        }

        /// <summary>
        /// Click on claim status to navigate to claim history
        /// </summary>
        /// <param name="claimSeq"></param>
        /// <returns></returns>
        public ClaimHistoryPage ClickOnClaimStatusOfClaimSequence(string claimSeq)
        {
            int indexOfStatus = 8;
            if (StartLegacy.Product.Equals(ProductEnum.PCI))
                indexOfStatus = 8;
            else if (StartLegacy.Product.Equals(ProductEnum.DCI))
                indexOfStatus = 7;
            var claimHistoryPage = Navigator.Navigate<ClaimHistoryPageObjects>(() =>
                                     {
                                        var indexOfClaimSeq = SiteDriver.FindElements(FlaggedClaimPageObjects.ClaimSeqXPath, How.XPath, "Text").FindIndex(x => x == claimSeq) + 2;
                                        SiteDriver.FindElement<Link>(string.Format(FlaggedClaimPageObjects.ClaimStatusXPathTemplate, indexOfClaimSeq, indexOfStatus), How.XPath).Click();
                                        Console.WriteLine(string.Format("Click on claim status of claim sequence '{0}'", claimSeq));
                                        SiteDriver.WaitForIe(3000);
                                        SiteDriver.SwitchWindowByTitle(PageTitleEnum.ClaimHistory.GetStringValue());
                                    });
            
            return new ClaimHistoryPage(Navigator, claimHistoryPage); 
        }

        public ClaimSummaryPage ClickOnClaimSequence(string claimSeq)
        {
          
            var claimSummaryPage = Navigator.Navigate<ClaimSummaryPageObjects>(() =>
                                {
                                    SiteDriver.FindElement<Link>(string.Format(FlaggedClaimPageObjects.ClaimSeqXPathTemplate,claimSeq), How.XPath).Click();
                                    Console.WriteLine(string.Format("Click on claim sequence '{0}'", claimSeq));
                                });
            SiteDriver.SwitchFrame("View");
            return new ClaimSummaryPage(Navigator, claimSummaryPage);
        }

        public List<int> GetColumnValue(int colIndex)
        {
            var originalValues = new List<int>();
            for (int i = 2; i < 100; i++)
            {
                try
                {
                    originalValues.Add(Convert.ToInt32(SiteDriver.FindElement<TextLabel>(string.Format(FlaggedClaimPageObjects.ClaimNoXPath, i), How.XPath).Text)); 
                }
                catch (Exception)
                {
                    break;
                }
            }
            return originalValues;
        }

        public void ClickClaimNoAscendingArrow()
        {
            _flaggedClaimPage.ClaimNoAscendingArrow.Click();
            SiteDriver.WaitToLoadNew(2000);
            Console.WriteLine("Clicked claim no ascending arrow");
        }

        public void ClickClaimNoDescendingArrow()
        {
            _flaggedClaimPage.ClaimNoDescendingArrow.Click();
            
            Console.WriteLine("Clicked claim no descending arrow");
        }
    }
}

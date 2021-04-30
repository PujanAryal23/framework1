using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using Legacy.Service.PageObjects.Product;
using Legacy.Service.PageServices.Base;
using Legacy.Service.PageServices.Default;
using Legacy.Service.Support.Environment;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Common.Constants;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Core.Navigation;
using UIAutomation.Framework.Elements;
using UIAutomation.Framework.Utils;

namespace Legacy.Service.PageServices.Product
{
    public class SearchClearedPage : DefaultPage
    {
        private SearchClearedPageObjects _searchClearedPage;

        public SearchClearedPage(INavigator navigator, SearchClearedPageObjects searchClearedPage)
            : base(navigator, searchClearedPage)
        {
            // Just for performance!
            _searchClearedPage = (SearchClearedPageObjects)PageObject;
        }


        public override IPageService GoBack()
        {
            Navigator.Back();
            if (Navigator.CurrentUrl.StartsWith(PageObject.PageUrl))
                return this;
            return new PhysicianClaimInsightPage(Navigator, new PhysicianClaimInsightPageObjects());
        }

        public SearchClearedPage SearchByBatchId(string batchId)
        {
            if (string.Compare(EnvironmentManager.Instance.Browser, BrowserConstants.Chrome, StringComparison.OrdinalIgnoreCase) == 0)
            {
                JavaScriptExecutor.ExecuteClick(SearchClearedPageObjects.BatchIdDropDownId, How.Id);
                SiteDriver.FindElement<CustomButton>(
                    string.Format(SearchClearedPageObjects.BatchIdDropDownXPathTemplate, batchId), How.XPath).Click();
            }
            else
            {
                _searchClearedPage.BatchIdDropDown.Click();
                _searchClearedPage.BatchIdDropDown.SetText(batchId);
            }
            
            Console.WriteLine(string.Format("Search by Batch Id '{0}'", batchId));
            return ClickSearchButton();
        }

        public SearchClearedPage ClickSearchButton()
        {
            _searchClearedPage.SearchButton.Click();
            Console.WriteLine("Click Search Button");
            
            return new SearchClearedPage(Navigator, new SearchClearedPageObjects());
        }

        public SearchClearedPage ClickAscendingArrowOfReleaseDateColumn()
        {
            _searchClearedPage.ReleaseDateAscendingArrow.Click();
            Console.WriteLine("Clicked ascending arrow of Release Date Column.");
            
            return new SearchClearedPage(Navigator, new SearchClearedPageObjects());
        }

        public DateTime GetReleaseDateOfFirstRow()
        {
            return
                Convert.ToDateTime(_searchClearedPage.FirstRowReleaseDate.Text);
        }
    }
}

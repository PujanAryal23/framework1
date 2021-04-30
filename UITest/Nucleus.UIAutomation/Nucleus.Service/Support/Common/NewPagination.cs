using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nucleus.Service.PageObjects.Claim;
using Nucleus.Service.PageServices.Base.Default;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Base;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Elements;
using UIAutomation.Framework.Utils;

namespace Nucleus.Service.Support.Common
{
    public class NewPagination
    {

        public const string LoadMoreCssLocator = "div.load_more_data span";
        public const string LinkableLoadMoreCssLocator = "div.load_more_data span";
        public const string WorkingAjaxMessageCssLocator = "div.small_loading";
        private readonly ISiteDriver SiteDriver;
        private readonly IJavaScriptExecutors JavaScriptExecutor;

        #region Constructor

        public NewPagination(ISiteDriver siteDriver, IJavaScriptExecutors javaScriptExecutors)
        {
            SiteDriver = siteDriver;
            JavaScriptExecutor = javaScriptExecutors;
        }
        

        #endregion
        public bool IsWorkingAjaxMessagePresent()
        {
            return SiteDriver.IsElementPresent(WorkingAjaxMessageCssLocator, How.CssSelector);
        }

        public void WaitForWorkingAjaxMessage()
        {
            SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
            SiteDriver.WaitForPageToLoad();
        }

        public void ClickOnLoadMore()
        {
            JavaScriptExecutor.ExecuteClick(LoadMoreCssLocator, How.CssSelector);
            WaitForWorkingAjaxMessage();
        }

        public string GetLoadMoreText()
        {
            return SiteDriver.FindElement(LoadMoreCssLocator, How.CssSelector).Text;
        }

        public bool IsLoadMoreLinkable()
        {
            return SiteDriver.IsElementPresent(LinkableLoadMoreCssLocator, How.CssSelector);
        }

    }
}

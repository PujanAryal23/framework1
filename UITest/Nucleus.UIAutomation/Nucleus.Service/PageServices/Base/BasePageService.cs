using System;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Base;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Core.Navigation;
using UIAutomation.Framework.Elements;
using UIAutomation.Framework.Utils;

namespace Nucleus.Service.PageServices.Base
{
    public abstract class BasePageService : IPageService
    {
        protected PageBase PageObject;

        private readonly INavigator _navigator;

        protected BasePageService(INavigator navigator, PageBase page)
        {
            _navigator = navigator;
            PageObject = (PageBase)SiteDriver.InitializePageElement(page);
        }

        public string CurrentPageUrl
        {
            get { return SiteDriver.Url; }
        }

        public string CurrentPageTitle
        {
            get { return SiteDriver.Title; }
        }

        public string PageUrl
        {
            get { return PageObject.PageUrl; }
        }

        public string PageTitle
        {
            get { return PageObject.PageTitle; }
        }

        protected INavigator Navigator
        {
            get { return _navigator; }
        }

        protected void SendKeys(String strKey, String select,How selector)
        {
            JavaScriptExecutor.SendKeys(strKey,select,selector);
        }

        protected void Clear(String select, How selector)
        {
            JavaScriptExecutor.Clear(select, selector);
        }
    }
}

using System;
using Nucleus.Service.Support.Environment;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Base;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Core.Navigation;
using UIAutomation.Framework.Database;
using UIAutomation.Framework.Elements;
using UIAutomation.Framework.Utils;

namespace Nucleus.Service.PageServices.Base
{
    public abstract class NewBasePageService : IPageService
    {
        protected NewPageBase PageObject;

        private readonly INewNavigator _navigator;
        private readonly ISiteDriver _siteDriver;
        private readonly IJavaScriptExecutors _javaScriptExecutors;
        private readonly IEnvironmentManager _environmentManager;
        private readonly IBrowserOptions _browserOptions;
        private readonly IOracleStatementExecutor _executor;
       
        public NewBasePageService(INewNavigator navigator, NewPageBase page, ISiteDriver siteDriver,IJavaScriptExecutors javaScriptExecutors, IEnvironmentManager environmentManager, IBrowserOptions browserOptions, IOracleStatementExecutor executor)
        {
            _navigator = navigator;
            _siteDriver = siteDriver;
            _javaScriptExecutors = javaScriptExecutors;
            _environmentManager = environmentManager;
            _browserOptions = browserOptions;
            PageObject = (NewPageBase)_siteDriver.InitializePageElement(page);
            _executor = executor;
        }

        public string CurrentPageUrl
        {
            get { return _siteDriver.Url; }
        }

        public string CurrentPageTitle
        {
            get { return _siteDriver.Title; }
        }

        public string PageUrl
        {
            get { return PageObject.PageUrl; }
        }

        public string PageTitle
        {
            get { return PageObject.PageTitle; }
        }

        protected INewNavigator Navigator
        {
            get { return _navigator; }
        }

        protected ISiteDriver SiteDriver
        {
            get { return _siteDriver; }
        }

        protected IOracleStatementExecutor Executor
        {
            get { return _executor; }
        }
        public void CaptureScreenShot(string testName)
        {
            SiteDriver.CaptureScreenShot(testName);
        }
        void PageRefresh()
        {
            SiteDriver.Refresh();
        }


        protected IJavaScriptExecutors JavaScriptExecutor
        {
            get { return _javaScriptExecutors; }
        }

        protected IEnvironmentManager EnvironmentManager
        {
            get { return _environmentManager; }
        }

        protected IBrowserOptions BrowserOptions
        {
            get { return _browserOptions; }
        }
       
        protected void SendKeys(String strKey, String select, How selector)
        {
            JavaScriptExecutor.SendKeys(strKey, select, selector);
        }

        protected void Clear(String select, How selector)
        {
            JavaScriptExecutor.Clear(select, selector);
        }

        public void WaitForCondition(Func<bool> f, int milliSec = 0) =>
            SiteDriver.WaitForCondition(f, milliSec);
    }
}
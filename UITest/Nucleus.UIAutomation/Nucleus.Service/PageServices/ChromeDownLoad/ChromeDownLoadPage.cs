using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using Nucleus.Service.PageObjects.ChromeDownLoad;
using Nucleus.Service.PageObjects.Dashboard;
using Nucleus.Service.PageObjects.Default;
using Nucleus.Service.PageObjects.Invoice;
using Nucleus.Service.PageObjects.Provider;
using Nucleus.Service.PageServices.Base;
using Nucleus.Service.Support.Environment;
using UIAutomation.Framework.Common;
using Nucleus.Service.PageServices.Base.Default;
using Nucleus.Service.PageServices.Dashboard;
using Nucleus.Service.PageServices.Provider;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Core.Navigation;
using UIAutomation.Framework.Utils;
using Nucleus.Service.PageServices.QuickLaunch;
using Nucleus.Service.PageObjects.QuickLaunch;
//using Nucleus.Service.PageServices.Invoice;
using UIAutomation.Framework.Elements;
using Nucleus.Service.PageServices.Claim;
using Nucleus.Service.PageObjects.Claim;
using Nucleus.Service.PageServices.Invoice;
using UIAutomation.Framework.Database;

namespace Nucleus.Service.PageServices.ChromeDownLoad
{
    public class ChromeDownLoadPage : NewBasePageService
    {
        #region PRIVATE FIELDS

        private ChromeDownLoadPageObjects _chromeDownLoad;

        #endregion

        #region CONSTRUCTOR

        public ChromeDownLoadPage(INewNavigator navigator, ChromeDownLoadPageObjects chromeDownLoad, ISiteDriver siteDriver, IJavaScriptExecutors javaScriptExecutors, IEnvironmentManager environmentManager, IBrowserOptions browserOptions, IOracleStatementExecutor executor)
            : base(navigator, chromeDownLoad, siteDriver, javaScriptExecutors, environmentManager, browserOptions, executor)
        {
            _chromeDownLoad = (ChromeDownLoadPageObjects)PageObject;
        }
        #endregion

        #region PUBLIC METHODS

        public string GetFileLocation()
        {
            if (string.Compare(ConfigurationManager.AppSettings["TestBrowser"], "edge", StringComparison.OrdinalIgnoreCase) == 0)
                return GetFileLocationForEdge();
            SiteDriver.WaitForCondition(JavaScriptExecutor.IsDownloadedFilePresent);
            CaptureScreenShot("Failed reason for Failing Dashboard Download Test");
            return JavaScriptExecutor.GetDownloadedFilename().GetAttribute("href").Substring(8);
            //SiteDriver.WaitForCondition(() => JavaScriptExecutor.GetDownloadedFilename());
            // return SiteDriver.FindElement(ChromeDownLoadPageObjects.FileLocationCssLocator,How.CssSelector).GetAttribute("href").Substring(8);
        }

        public string GetFileName()
        {
            if (string.Compare(ConfigurationManager.AppSettings["TestBrowser"],"edge",StringComparison.OrdinalIgnoreCase) == 0)
                return GetFileNameForEdge();
            SiteDriver.WaitForCondition(JavaScriptExecutor.IsDownloadedFilePresent);
            CaptureScreenShot("Failed reason for Failing Dashboard Download Test");
            return JavaScriptExecutor.GetDownloadedFilename().Text;
            // SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(ChromeDownLoadPageObjects.FileLocationCssLocator, How.CssSelector));
            //  return SiteDriver.FindElement(ChromeDownLoadPageObjects.FileLocationCssLocator, How.CssSelector).Text;

        }

        public string GetFileNameForEdge()
        {
            SiteDriver.WaitForCondition(()=> SiteDriver.IsElementPresent(ChromeDownLoadPageObjects.FileNameForEdgeCssLocator,How.CssSelector));
            return SiteDriver.FindElement(ChromeDownLoadPageObjects.FileNameForEdgeCssLocator, How.CssSelector)
                .Text;
        }

        public string GetFileLocationForEdge()
        {
            SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(ChromeDownLoadPageObjects.FileLocationForEdgeCssLocator, How.CssSelector));
            return SiteDriver.FindElement(ChromeDownLoadPageObjects.FileLocationForEdgeCssLocator, How.CssSelector)
                .Text;
        }

        public List<string> GetFileNameList()
        {
            SiteDriver.WaitForCondition(JavaScriptExecutor.IsDownloadedFilePresent);
            var length = JavaScriptExecutor.GetDownloadedFilenameList();
            var list = new List<string>();
            for (var i = 0; i < length; i++)
            {
                list.Add(JavaScriptExecutor.GetDownloadedFilename(i).Text);
            }
            return list;
        }

        public T ClickBrowserBackButton<T>(string url = null)
        {
            var target = typeof(T);
            Action action;

            if (url != null)
                action = () => SiteDriver.Open(url);
            else
                action = SiteDriver.Back;
            if (typeof(ProviderSearchPage) == target)
            {

                PageObject = Navigator.Navigate<ProviderSearchPageObjects>(action);
            }
            else
            if (typeof(ClaimsDetailPage) == target)
            {
                PageObject = Navigator.Navigate<ClaimsDetailPageObjects>(action);
            }
            else if (typeof(COBClaimsDetailPage) == target)
            {
               PageObject = Navigator.Navigate<COBClaimsDetailpageObject>(action);
            }
            else if (typeof(COBAppealsDetailPage) == target)
            {
                PageObject = Navigator.Navigate<COBAppealsDetailPageObjects>(action);
            }
            else if (typeof(AppealsDetailPage) == target)
            {
                PageObject = Navigator.Navigate<AppealsDetailPageObjects>(action);
            }

            else if (typeof(DashboardLogicRequestsDetailsPage) == target)
            {
                PageObject = Navigator.Navigate<DashboardLogicRequestsDetailPageObjects>(action);
            }
            else if (typeof(QuickLaunchPage) == target)
            {
                PageObject = Navigator.Navigate<QuickLaunchPageObjects>(action);
            }
            else if (typeof(InvoiceSearchPage) == target)
            {
               PageObject = Navigator.Navigate<InvoiceSearchPageObjects>(action);
            }
            else if (typeof(LogicSearchPage) == target)
            {
                PageObject = Navigator.Navigate<LogicSearchPageObjects>(action);
            }
            else if (typeof(ClaimSearchPage) == target)
            {
                PageObject = Navigator.Navigate<ClaimSearchPageObjects>(action);
            }
            else if (typeof(ProviderSearchPage) == target)
            {
                PageObject = Navigator.Navigate<ProviderSearchPageObjects>(action);
            }
            else if (typeof(SuspectProvidersPage) == target)
            {
                PageObject = Navigator.Navigate<SuspectProvidersPageObjects>(action);
            }
            else if (typeof(COBClaimsDetailPage) == target)
            {
                PageObject = Navigator.Navigate<COBClaimsDetailpageObject>(action);
            }
            return (T)Activator.CreateInstance(target, Navigator, PageObject, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public string ClickOnDownloadAndGetFileName(string url = null)
        {
            var fileName = GetFileName();
            SiteDriver.WaitForCondition(() =>
            {
                if (fileName == "")
                {
                    fileName = GetFileName();
                    return false;
                }
                else
                    return true;
            }, 5000);
            return fileName;

        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nucleus.Service.PageObjects.ChromeDownLoad;
using Nucleus.Service.PageObjects.Dashboard;
using Nucleus.Service.PageServices.Base;
using Nucleus.Service.PageServices.Base.Default;
using Nucleus.Service.PageServices.ChromeDownLoad;
using Nucleus.Service.Support.Environment;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Core.Navigation;
using UIAutomation.Framework.Database;
using UIAutomation.Framework.Elements;
using UIAutomation.Framework.Utils;

namespace Nucleus.Service.PageServices.Dashboard
{
    public class COBAppealsDetailPage : NewDefaultPage
    {
        #region PRIVATE FIELDS

        private COBAppealsDetailPageObjects _cobAppealsDetailPage;
        

        #endregion

        #region CONSTRUCTOR

        public COBAppealsDetailPage(INewNavigator navigator, COBAppealsDetailPageObjects cobAppealsDetailPage, ISiteDriver siteDriver, IJavaScriptExecutors javaScriptExecutors, IEnvironmentManager environmentManager, IBrowserOptions browserOptions, IOracleStatementExecutor executor)
            : base(navigator, cobAppealsDetailPage, siteDriver, javaScriptExecutors, environmentManager, browserOptions, executor)
        {
            _cobAppealsDetailPage = (COBAppealsDetailPageObjects)PageObject;

        }

        #endregion

        #region PUBLIC METHODS
        public string GetCOBAppealsDetailPageHeader()
        {
            return SiteDriver.FindElement(COBAppealsDetailPageObjects.COBAppealsDetailPageheaderXPath, How.XPath).Text;
        }

        public string GoToDownloadPageAndGetFileName()
        {
            var fileName = ChromeDownLoadPage.ClickOnDownloadAndGetFileName();
            ChromeDownLoadPage.ClickBrowserBackButton<COBAppealsDetailPage>();
            SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(COBAppealsDetailPageObjects.COBAppealsDetailPageheaderXPath, How.XPath));
            SiteDriver.WaitForPageToLoad();
            return fileName;

        }


        #endregion

    }
}

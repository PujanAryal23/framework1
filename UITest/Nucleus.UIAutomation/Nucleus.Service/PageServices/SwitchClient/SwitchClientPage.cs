using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Nucleus.Service.PageObjects.Default;
using Nucleus.Service.PageObjects.QuickLaunch;
using Nucleus.Service.PageObjects.SwitchClient;
using Nucleus.Service.PageServices.Base;
using Nucleus.Service.PageServices.QuickLaunch;
using Nucleus.Service.SqlScriptObjects.SwitchClient;
using Nucleus.Service.Support.Common.Constants;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Environment;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Core.Navigation;
using UIAutomation.Framework.Database;
using UIAutomation.Framework.Utils;
using UIAutomation.Framework.Elements;
using static System.String;

namespace Nucleus.Service.PageServices.SwitchClient
{
    public class SwitchClientPage : NewBasePageService
    {
        #region PRIVATE FIELDS

        private readonly SwitchClientPageObjects _switchClientPage;
        private QuickLaunchPageObjects _quickLaunch;

        #endregion

        #region CONSTRUCTOR

        public SwitchClientPage(INewNavigator navigator, SwitchClientPageObjects switchClientPage, ISiteDriver siteDriver, IJavaScriptExecutors javaScriptExecutors, IEnvironmentManager environmentManager, IBrowserOptions browserOptions, IOracleStatementExecutor executor)
            : base(navigator, switchClientPage, siteDriver, javaScriptExecutors, environmentManager, browserOptions, executor)
        {
            _switchClientPage = (SwitchClientPageObjects)PageObject;
        }

        #endregion

        #region PUBLIC METHODS

        //public QuickLaunchPage SwitchClientTo(ClientEnum clientName, bool isCorePage=false,bool isPopup=false)
        //{
        //    var oldHeaderName =
        //        SiteDriver.FindElement(DefaultPageObjects.PageHeaderCssLocator, How.CssSelector).Text;
        //    _quickLaunch = Navigator.Navigate<QuickLaunchPageObjects>(() =>
        //    {
        //        EnvironmentManager.CaptureScreenShot("Dashbord_issue_popup");
        //        Console.WriteLine(
        //            $"Current Time={DateTime.Now} and Last Time Used for {clientName}={GetLastTimeUsedFromDatabase(EnvironmentManager.Username, clientName)}");
        //        if (isCorePage)
        //        {
        //            JavaScriptExecutors.ClickJQueryByText(
        //                string.Format(SwitchClientPageObjects.SwitchClientTemplateCore,clientName.ToString(), clientName.GetStringValue()),
        //                clientName.GetStringValue());
                    
        //            if (isPopup)
        //            {
        //                SiteDriver.WaitForCondition(() =>Navigator.IsAlertBoxPresent(), 10000);
        //                Navigator.AcceptAlertBox();
        //            }
        //            SiteDriver.WaitForCondition(() =>
        //                SiteDriver.IsElementPresent(DefaultPageObjects.WorkingAjaxMessageCssLocator, How.CssSelector),5000);
        //            SiteDriver.WaitForCondition(() =>
        //                !SiteDriver.IsElementPresent(DefaultPageObjects.WorkingAjaxMessageCssLocator, How.CssSelector));
        //            SiteDriver.WaitForPageToLoad();
        //            SiteDriver.WaitForCondition(() =>
        //                SiteDriver.FindElement(DefaultPageObjects.PageHeaderCssLocator, How.CssSelector).Text != oldHeaderName);
        //        }
        //        else                   
        //            SiteDriver.FindElement(string.Format(SwitchClientPageObjects.SwitchClientXPathTemplate, clientName, clientName.GetStringValue()), How.XPath).Click();
        //        SiteDriver.WaitForPageToLoad();
        //        StringFormatter.PrintMessage($"Switch client to : {clientName}");
        //        Console.WriteLine(
        //            $"Current Time={DateTime.Now} and Last Time Used for {clientName}={GetLastTimeUsedFromDatabase(EnvironmentManager.Username, clientName)}");
        //    });
        //    return new QuickLaunchPage(Navigator,_quickLaunch);
        //}

        //public string GetLastTimeUsedFromDatabase(string userId, ClientEnum clientEnum) =>
        //    _executor.GetSingleStringValue(Format(SwitchClientSqlScriptObject.GetLastTimeUsed, userId, clientEnum));

        //public List<string> GetSwitchClientList()
        //{
        //    return SiteDriver.FindElements(SwitchClientPageObjects.SwitchClientListCssSelector, How.CssSelector,
        //        "Text");
        //}

        #endregion
    }
}

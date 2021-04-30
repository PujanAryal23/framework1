using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using Microsoft.SqlServer.Server;
using Nucleus.Service.PageObjects.HelpCenter;
using Nucleus.Service.PageServices.Base.Default;
using Nucleus.Service.SqlScriptObjects.HelpCenter;
using Nucleus.Service.Support.Environment;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Core.Navigation;
using UIAutomation.Framework.Database;
using UIAutomation.Framework.Elements;
using UIAutomation.Framework.Utils;
using static System.String;

namespace Nucleus.Service.PageServices.HelpCenter
{
    public class HelpCenterPage : NewDefaultPage
    {
        #region PRIVATE FIELDS

        private HelpCenterPageObjects _helpCenterPage;
        
        #endregion

        #region PUBLIC PROPERTIES

        public HelpCenterPageObjects HelpCenter
        {
            get { return _helpCenterPage; }
        }

        #endregion

        #region CONSTRUCTOR

        public HelpCenterPage(INewNavigator navigator, HelpCenterPageObjects helpCenterPage, ISiteDriver siteDriver, IJavaScriptExecutors javaScriptExecutors, IEnvironmentManager environmentManager, IBrowserOptions browserOptions, IOracleStatementExecutor executor)
            : base(navigator, helpCenterPage, siteDriver, javaScriptExecutors, environmentManager, browserOptions, executor)
        {
            _helpCenterPage = (HelpCenterPageObjects)PageObject;
        }

        #endregion

        #region PUBLIC METHODS

        public void ClickFormsTab()
        {
            SiteDriver.FindElement(HelpCenterPageObjects.FormsTabId, How.Id).Click();
        }

        public void ClickSystemTab()
        {
            SiteDriver.FindElement(HelpCenterPageObjects.SystemTabId, How.Id).Click();
        }

        public void ClickReleaseTab()
        {
            SiteDriver.FindElement(HelpCenterPageObjects.ReleaseTabId, How.Id).Click();
        }

        public bool IsFraudPreventionQuickStartGuideLinkValid()
        {
            return SiteDriver.IsUrlValid(SiteDriver.FindElement(HelpCenterPageObjects.FraudPreventionQuickStartGuideLinkId,How.Id).GetAttribute("href"));
        }

        public bool IsClaimsEditingQuickStartGuideLinkValid()
        {
            return SiteDriver.IsUrlValid(SiteDriver.FindElement(HelpCenterPageObjects.ClaimsEditingQuickStartGuideLinkId, How.Id).GetAttribute("href"));
        }

        public bool IsFraudPreventionOnlineHelpLinkValid()
        {
            return SiteDriver.IsUrlValid(SiteDriver.FindElement(HelpCenterPageObjects.FraudPreventionOnlineHelpLinkId,How.Id).GetAttribute("href"));
        }

        public bool IsClaimsEditingOnlineHelpLinkValid()
        {
            return SiteDriver.IsUrlValid(SiteDriver.FindElement(HelpCenterPageObjects.ClaimsEditingOnlineHelpLinkId,How.Id).GetAttribute("href"));
        }

        public bool IsNewUserRequestFormLinkValid()
        {
            return SiteDriver.IsUrlValid(SiteDriver.FindElement(HelpCenterPageObjects.NewUserRequestFormLinkId,How.Id).GetAttribute("href"));
        }

        public bool IsUserTerminationRequestFormLinkValid()
        {
            return SiteDriver.IsUrlValid(SiteDriver.FindElement(HelpCenterPageObjects.UserTerminationRequestFormLinkId,How.Id).GetAttribute("href"));
        }

        public bool IsUserReportRequestFormLinkValid()
        {
            return SiteDriver.IsUrlValid(SiteDriver.FindElement(HelpCenterPageObjects.UserReportRequestFormLinkId,How.Id).GetAttribute("href"));
        }

        public bool IsClientCustomizationFormLinkValid()
        {
            return SiteDriver.IsUrlValid(SiteDriver.FindElement(HelpCenterPageObjects.ClientCustomizationFormLinkId,How.Id).GetAttribute("href"));
        }

        public bool IsFraudPreventionEobFlagDescLinkValid()
        {
            return SiteDriver.IsUrlValid(SiteDriver.FindElement(HelpCenterPageObjects.FraudPreventionEobFlagDescLinkId,How.Id).GetAttribute("href"));
        }

        public bool IsClaimsEditingEobFlagDescLinkValid()
        {
            return SiteDriver.IsUrlValid(SiteDriver.FindElement(HelpCenterPageObjects.ClaimsEditingEobFlagDescLinkId,How.Id).GetAttribute("href"));
        }

        public bool IsRulesEngineReleaseNotesLinkValid()
        {
            return SiteDriver.IsUrlValid(SiteDriver.FindElement(HelpCenterPageObjects.RulesEngineReleaseNotesLinkId,How.Id).GetAttribute("href"));
        }

        public bool IsFormHeaderPresent() =>
            SiteDriver.IsElementPresent(HelpCenterPageObjects.FormHeaderCssSelector, How.CssSelector);

        public bool IsDownloadIconPresentByFormName(string formName) =>
            JavaScriptExecutor.IsElementPresent(string.Format(HelpCenterPageObjects.DownloadIconByFormNameCssTemplate,
                formName));

        public bool IsSystemInformationPresent() =>
            SiteDriver.IsElementPresent(HelpCenterPageObjects.SystemInformationHeaderCssSelector, How.CssSelector);

        public string GetHelpDeskHeader() => SiteDriver
            .FindElement(HelpCenterPageObjects.HelpDeskHeaderCssSelector, How.CssSelector).Text;

        public List<string> GetHelpDeskHeadersList() =>
            JavaScriptExecutor.FindElements(HelpCenterPageObjects.HelpDeskInfoSectionHeadersCssSelector, How.CssSelector, "Text");

        public List<string> GetInfoSectionDescriptionByHeader(string header) => JavaScriptExecutor.FindElements(
                string.Format(HelpCenterPageObjects.InfoSectionDescriptionXPathTemplate, header), How.XPath, "Text")
            .Select(x => x.Replace("\r\n", " ")).ToList();

        public int GetCountOfHolidays(string header) =>
            JavaScriptExecutor.FindElementsCount(Format(HelpCenterPageObjects.HolidaysListCssSelectorTemplate, header));
        public string GetAdobeAcrobatReaderText() => 
        JavaScriptExecutor.FindElement(HelpCenterPageObjects.AdobeTextCssSelector).GetAttribute("outerText");

        public void ClickAdobeReader() =>
            SiteDriver.FindElement(HelpCenterPageObjects.AdobeAcrobatReaderCssSelector, How.CssSelector).Click();

        public string ClickDownloadIconByHeaderAndGetFilename(string header)
        {
            JavaScriptExecutor
                .FindElement(string.Format(HelpCenterPageObjects.DownloadIconByFormNameCssTemplate, header)).Click();
            var downloadPage = NavigateToChromeDownLoadPage();
            var fileName = downloadPage.GetFileName();

            SiteDriver.WaitForCondition(() =>
            {
                if (fileName == "")
                {
                    fileName = downloadPage.GetFileName();
                    return false;
                }
                else
                    return true;
            }, 5000);

            return fileName;
        }

        public List<string> GetHolidaysForCurrentYearFromDb() =>
            Executor.GetTableSingleColumn(HelpCenterSqlScriptObjects.GetHolidaysForCurrentYear);

        public void ClickOnCotivitiUrl() =>
            JavaScriptExecutor.FindElement(HelpCenterPageObjects.CotivitiUrlCssSelector).Click();

        #endregion
    }
}

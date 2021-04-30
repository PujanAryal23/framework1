using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Nucleus.Service.PageObjects.Appeal;
using Nucleus.Service.PageObjects.Claim;
using Nucleus.Service.PageServices.Base;
using Nucleus.Service.PageServices.Base.Default;
using Nucleus.Service.PageServices.Claim;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Environment;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Core.Navigation;
using UIAutomation.Framework.Database;
using UIAutomation.Framework.Elements;
using UIAutomation.Framework.Utils;
using Extensions = Nucleus.Service.Support.Utils.Extensions;

namespace Nucleus.Service.PageServices.Appeal
{
    public class AppealLetterPage : NewDefaultPage
    {
        #region PRIVATE FIELDS

        private AppealLetterPageObjects _appealLetterPage;

        #endregion

        #region CONSTRUCTOR

        public AppealLetterPage(INewNavigator navigator, AppealLetterPageObjects appealLetterPage, ISiteDriver siteDriver, IJavaScriptExecutors javaScriptExecutors, IEnvironmentManager environmentManager, IBrowserOptions browserOptions, IOracleStatementExecutor executor)
            : base(navigator, appealLetterPage, siteDriver, javaScriptExecutors, environmentManager, browserOptions, executor)
        {
            _appealLetterPage = (AppealLetterPageObjects)PageObject;
        }

        #endregion

        #region PUBLIC METHODS

        public void ClickOnDownloadPdf()
        {
             SiteDriver.FindElement(AppealLetterPageObjects.DownloadPdfClass, How.ClassName).Click();
        }

        public string GetAppealLetterPageHeader()
        {
            return SiteDriver.FindElement(AppealLetterPageObjects.PageHeaderCssLocator, How.CssSelector).Text;
        }

        public string ClickOnClaimNoAndGetPageHeader()
        {
            JavaScriptExecutor.ExecuteClick(AppealLetterPageObjects.ClaimNoCSSSelector, How.CssSelector);
            Console.WriteLine("Clicked on claim sequence, navigating to claim action");
            SiteDriver.WaitForCondition(() => SiteDriver.SwitchWindowByUrl("claims"));
            SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(ClaimActionPageObjects.ClaimActionPageTitleCss, How.CssSelector));
            var pageHeader = GetPageHeader();
            SiteDriver.CloseWindow();
            SiteDriver.WaitForCondition(() => SiteDriver.SwitchWindowByTitle(PageTitleEnum.AppealLetter.GetStringValue()));
            return pageHeader;

        }

        /// <summary>
        /// Close note popup page and back to new appeal manager
        /// </summary>
        public AppealManagerPage CloseLetterPopUpPageAndBackToAppealManager()
        {
            var appealAction = Navigator.Navigate<AppealManagerPageObjects>(() =>
            {
                SiteDriver.CloseWindow();
                SiteDriver.SwitchWindowByTitle(Extensions.GetStringValue(PageTitleEnum.AppealManager));
            });
            return new AppealManagerPage(Navigator, appealAction, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        /// <summary>
        /// Close note popup page and back to new appeal search
        /// </summary>
        public AppealSearchPage CloseLetterPopUpPageAndBackToAppealSearch()
        {
            var appealAction = Navigator.Navigate<AppealSearchPageObjects>(() =>
            {
                SiteDriver.CloseWindow();
                SiteDriver.SwitchWindowByTitle(Extensions.GetStringValue(PageTitleEnum.AppealSearch));
            });
            return new AppealSearchPage(Navigator, appealAction, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        
        /// <summary>
        /// Close note popup page and back to appeal summary page
        /// </summary>
        public AppealSummaryPage CloseLetterPopUpPageAndBackToAppealSummary()
        {
            var appealAction = Navigator.Navigate<AppealSummaryPageObjects>(() =>
            {
                SiteDriver.CloseWindow();
                SiteDriver.SwitchWindowByTitle(Extensions.GetStringValue(PageTitleEnum.AppealSummary));
            });
            return new AppealSummaryPage(Navigator, appealAction, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        /// <summary>
        /// Close note popup page
        /// </summary>
        public void CloseLetterPopUpPageAndSwitchToNewClaimActionPage()
        {
            SiteDriver.CloseWindow();
            SiteDriver.SwitchWindowByTitle(Extensions.GetStringValue(PageTitleEnum.ClaimAction));
        }

        public string GetReviewDate()
        {
            return SiteDriver.FindElement(AppealLetterPageObjects.ReviewDateControlId, How.Id).Text;
        }

        public string GetAppealLetterClosing()
        {
            return SiteDriver.FindElement(AppealLetterPageObjects.AppealLetterClosingId, How.Id).Text;
        }
        public string GetReviewDisclaimer()
        {
            return SiteDriver.FindElement(AppealLetterPageObjects.ReviewDisclaimerId, How.Id).Text;
        }

        public List<string> GetDocTypeInAppealLetterBody(bool popUp = false)
        {
            if (popUp)
                return SiteDriver.FindDisplayedElementsText(AppealActionPageObjects.DocumentTypeInLetterBody, How.CssSelector);
            else
                return SiteDriver.FindDisplayedElementsText(AppealActionPageObjects.GetDocumentTypeInAppealLetter, How.CssSelector);

        }
        public string GetAppealLetterFullDetail()
        {
            return SiteDriver.FindElement(AppealLetterPageObjects.AppealLetterFullDetailClassName, How.Id).Text;
        }
        public List<string> GetFooterDetail()
        {
            var list = JavaScriptExecutor.FindElements(AppealLetterPageObjects.FooterCssSelector, How.CssSelector, "Text");
            return list;
        }

        public bool IsIroPhrasePresent()
        {
            return SiteDriver.IsElementPresent(AppealLetterPageObjects.IroPhraseId, How.Id);
        }

        public bool IsIroCareOfPresent()
        {
            return SiteDriver.IsElementPresent(AppealLetterPageObjects.IroCareOfId, How.Id);
        }

        public bool IsIroDeptNamePresent()
        {
            return SiteDriver.IsElementPresent(AppealLetterPageObjects.IroDeptNameId, How.Id);
        }

        public bool IsIroStreetPresent()
        {
            return SiteDriver.IsElementPresent(AppealLetterPageObjects.IroStreetId, How.Id);
        }

        public bool IsIroSuitePresent()
        {
            return SiteDriver.IsElementPresent(AppealLetterPageObjects.IroSuiteId, How.Id);
        }

        public bool IsIroCityPresent()
        {
            return SiteDriver.IsElementPresent(AppealLetterPageObjects.IroCityId, How.Id);
        }

        public bool IsIroStatePresent()
        {
            return SiteDriver.IsElementPresent(AppealLetterPageObjects.IroStateId, How.Id);
        }

        public bool IsIroZipPresent()
        {
            return SiteDriver.IsElementPresent(AppealLetterPageObjects.IroZipId, How.Id);
        }

        public bool IsIroPhonePresent()
        {
            return SiteDriver.IsElementPresent(AppealLetterPageObjects.IroPhoneId, How.Id);
        }

        public bool IsIroTollFreePresent()
        {
            return SiteDriver.IsElementPresent(AppealLetterPageObjects.IroTollFreeId, How.Id);
        }

        public bool IsIroFaxPresent()
        {
            return SiteDriver.IsElementPresent(AppealLetterPageObjects.IroFaxId, How.Id);
        }

        public bool IsIroEmailPresent()
        {
            return SiteDriver.IsElementPresent(AppealLetterPageObjects.IroEmailId, How.Id);
        }

        public bool IsIroWebSite1Present()
        {
            return SiteDriver.IsElementPresent(AppealLetterPageObjects.IroWebSite1Id, How.Id);
        }

        public bool IsIroWebSite2Present()
        {
            return SiteDriver.IsElementPresent(AppealLetterPageObjects.IroWebSite2Id, How.Id);
        }

        public int GetCountOfAppealClaimDetails()
        {
            return SiteDriver.FindElementsCount(AppealLetterPageObjects.AppealClaimDetailsSectionCssSelector,
                How.CssSelector);
        }

        public List<string> GetListOfLinesByAppealClaimDetailNumber(int appealClaimDetailNumber)
        {
            return JavaScriptExecutor.FindElements(string.Format(AppealLetterPageObjects.LineNumbersListFromAppealLetterByAppealClaimDetailsNumber, 
                    appealClaimDetailNumber), How.XPath, "Text");
        }

        public List<string> GetListOfNotesByAppealClaimDetailNumber(int appealClaimDetailNumber)
        {
            return JavaScriptExecutor.FindElements(string.Format(AppealLetterPageObjects.NotesListFromAppealLetterByAppealClaimDetailsNumber,
                appealClaimDetailNumber), How.XPath, "Text");
        }

       
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nucleus.Service.PageObjects.Claim;
using Nucleus.Service.PageServices.Base;
using Nucleus.Service.Support.Common.Constants;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Environment;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Core.Navigation;
using UIAutomation.Framework.Database;
using UIAutomation.Framework.Elements;
using UIAutomation.Framework.Utils;
using Extensions = Nucleus.Service.Support.Utils.Extensions;

namespace Nucleus.Service.PageServices.Claim
{
    public class FlagPopupPage : NewBasePageService
    {
         #region PRIVATE FIELDS

        private FlagPopupPageObjects _flagPopupPage;
        private string _currentWindowHandle;

        #endregion

        #region PUBLIC PROPERTIES

        public new string PageTitle { get { return _flagPopupPage.PageTitle; } }
        #endregion

        #region CONSTRUCTOR

        public FlagPopupPage(INewNavigator navigator, FlagPopupPageObjects flagPopupPage, ISiteDriver siteDriver, IJavaScriptExecutors javaScriptExecutors, IEnvironmentManager environmentManager, IBrowserOptions browserOptions, IOracleStatementExecutor executor)
            : base(navigator, flagPopupPage, siteDriver, javaScriptExecutors, environmentManager, browserOptions, executor)
        {
            _flagPopupPage = (FlagPopupPageObjects)PageObject;
        }

        #endregion

        #region PUBLIC METHODS

        public string GetTextValueWithInTag(int row = 3, string tag = ">br")
        {
            return SiteDriver.FindElement(string.Format(FlagPopupPageObjects.ToolTipContentTemplate, row, tag), How.CssSelector).Text.Trim();
        }

        public string GetTextValueWithinSpanTag(int row)
        {
            var list = JavaScriptExecutor
                .FindElements(string.Format(FlagPopupPageObjects.ToolTipContentTemplate, row, ">span"),
                    How.CssSelector,"Text");
            return list[0].TrimEnd() +" "+ list[1].TrimEnd();
        }

        public string GetPopupHeaderText()
        {
            return SiteDriver.FindElement(FlagPopupPageObjects.PopupHeaderTextCssSelector, How.CssSelector).Text.Trim();
        }

        public string GetTextContent(int row=1,int rowSpan=1)
        {
            return SiteDriver.FindElement(string.Format(FlagPopupPageObjects.ToolTipContentTemplate, row, ">span:nth-of-type(" + rowSpan + ")"), How.CssSelector).Text.Trim();
        }

        public string GetTextValueinLiTag(int row)
        {
            return SiteDriver.FindElement(string.Format(FlagPopupPageObjects.ToolTipContentTemplate, row, ""), How.CssSelector).Text.Trim();
        }

      

        public static void AssignPageTitle(string title)
        {
            NewPopupCodePageObjects.AssignPageTitle = title;
        }

        public int CountWindowHandles(string title)
        {
            return SiteDriver.WindowHandles.Count(handle => SiteDriver.GetHandleByTitle(title).Equals(handle));
        }

        public ClaimHistoryPage SwitchBackToPatientClaimPage()
        {
            var patientClaimHistoryPageObject = Navigator.Navigate<ClaimHistoryPageObjects>(() =>
            {
                SiteDriver.SwitchWindowByTitle(Extensions.GetStringValue(PageTitleEnum.ClaimHistoryPopup));
                StringFormatter.PrintMessage("Switch back to Patient Claim History Page.");
            });
            return new ClaimHistoryPage(Navigator, patientClaimHistoryPageObject, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public ClaimHistoryPage ClosePopup(string popupName)
        {

            var patientClaimHistoryPageObject = Navigator.Navigate<ClaimHistoryPageObjects>(() =>
            {
               // SiteDriver.SwitchWindowByTitle(popupName);
                SiteDriver.CloseWindow();
                SiteDriver.SwitchWindowByTitle(Extensions.GetStringValue(PageTitleEnum.ClaimHistoryPopup));
                StringFormatter.PrintMessage("Close popup and switch back to Patient Claim History Page.");
            });
            return new ClaimHistoryPage(Navigator, patientClaimHistoryPageObject, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public ClaimActionPage SwitchBackToNewClaimActionPage()
        {
            var newClaimActionPageObject = Navigator.Navigate<ClaimActionPageObjects>(() =>
            {
                SiteDriver.SwitchWindowByTitle(Extensions.GetStringValue(PageTitleEnum.ClaimAction));
                StringFormatter.PrintMessage("Switch back to New Claim Action Page.");
            });
            return new ClaimActionPage(Navigator, newClaimActionPageObject, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }
        public ClaimActionPage ClosePopupOnNewClaimActionPage(string popupName)
        {

            var newClaimActionPageObject = Navigator.Navigate<ClaimActionPageObjects>(() =>
            {
                SiteDriver.SwitchWindowByTitle(popupName);
                SiteDriver.CloseWindow();
                SiteDriver.SwitchWindowByTitle(Extensions.GetStringValue(PageTitleEnum.ClaimAction));
                StringFormatter.PrintMessage("Close popup and switch back to New Claim Action Page.");
            });
            return new ClaimActionPage(Navigator, newClaimActionPageObject, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public ClaimHistoryPage ClosePopupOnPatientPreAuthHistory(string popupName)
        {
            var patientClaimHistoryPageObject = Navigator.Navigate<ClaimHistoryPageObjects>(() =>
            {
                // SiteDriver.SwitchWindowByTitle(popupName);
                SiteDriver.CloseWindow();
                SiteDriver.SwitchWindowByTitle(Extensions.GetStringValue(PageTitleEnum.ClaimHistoryPopup));
                StringFormatter.PrintMessage("Close popup and switch back to Patient Claim History Page.");
            });
            return new ClaimHistoryPage(Navigator, patientClaimHistoryPageObject, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public bool IsFlagTitleBold()
        {
            return SiteDriver
                .FindElement(NewPopupCodePageObjects.PopupHeaderTextCssSelector, How.CssSelector)
                .GetAttribute("class").Contains("bold");
        }

        #endregion
    }
}

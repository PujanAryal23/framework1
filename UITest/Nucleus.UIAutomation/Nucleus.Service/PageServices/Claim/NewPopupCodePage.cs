using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Nucleus.Service.PageObjects.Appeal;
using Nucleus.Service.PageObjects.Claim;
using Nucleus.Service.PageObjects.PreAuthorization;
using Nucleus.Service.PageServices.Appeal;
using Nucleus.Service.PageServices.Base;
using Nucleus.Service.PageServices.PreAuthorization;
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
    public class NewPopupCodePage : NewBasePageService
    {
         #region PRIVATE FIELDS

        private NewPopupCodePageObjects _newPopCodePage;
        private string _currentWindowHandle;

        #endregion

        #region PUBLIC PROPERTIES

        public new string PageTitle { get { return _newPopCodePage.PageTitle; } }
        #endregion

        #region CONSTRUCTOR

        public NewPopupCodePage(INewNavigator navigator, NewPopupCodePageObjects newPopupCodePage, ISiteDriver siteDriver, IJavaScriptExecutors javaScriptExecutors, IEnvironmentManager environmentManager, IBrowserOptions browserOptions, IOracleStatementExecutor executor)
            : base(navigator, newPopupCodePage, siteDriver, javaScriptExecutors, environmentManager, browserOptions, executor)
        {
            _newPopCodePage = (NewPopupCodePageObjects)PageObject;
        }
        
        #endregion


        #region PUBLIC METHODS

        public string GetTextValueWithInBrTag(int row=3)
        {
            return SiteDriver.FindElement(string.Format(NewPopupCodePageObjects.ToolTipContentTemplate, row, ">br"), How.CssSelector).Text.Trim();
        }

        public string GetPopupHeaderText()
        {
            return SiteDriver.FindElement(NewPopupCodePageObjects.PopupHeaderTextCssSelector, How.CssSelector).Text.Trim();
        }

        public string GetTextContent(int row=1,int rowSpan=1)
        {
            return SiteDriver.FindElement(string.Format(NewPopupCodePageObjects.ToolTipContentTemplate, row, ">span:nth-of-type("+rowSpan+")"), How.CssSelector).Text.Trim();
        }

        public string GetTextValueinLiTag(int row)
        {
            return SiteDriver.FindElement(string.Format(NewPopupCodePageObjects.ToolTipContentTemplate, row, ""), How.CssSelector).Text.Trim();
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

        public AppealActionPage ClosePopupOnAppealActionPage(string popupName)
        {
            var newClaimActionPageObject = Navigator.Navigate<AppealActionPageObjects>(() =>
            {
                SiteDriver.SwitchWindowByTitle(popupName);
                SiteDriver.CloseWindow();
                SiteDriver.SwitchWindowByTitle(Extensions.GetStringValue(PageTitleEnum.AppealAction));
                StringFormatter.PrintMessage("Close popup and switch back to New Appeal Action Page.");
            });
            return new AppealActionPage(Navigator, newClaimActionPageObject, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public AppealSummaryPage ClosePopupOnBackAppealSummaryPage(string popupName)
        {

            var appealSummary = Navigator.Navigate<AppealSummaryPageObjects>(() =>
            {
                SiteDriver.SwitchWindowByTitle(popupName);
                SiteDriver.CloseWindow();
                SiteDriver.SwitchWindowByTitle(Extensions.GetStringValue(PageTitleEnum.AppealSummary));
                StringFormatter.PrintMessage("Close popup and switch back to Appeal Summary Page.");
            });
            return new AppealSummaryPage(Navigator, appealSummary, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public string GetPopUpCurrentUrl()
        {
            return SiteDriver.Url;
        }

        public string GetProcCodeDescriptionFromPopUp()
        {
            var desc = SiteDriver.FindElement(NewPopupCodePageObjects.PopUpDescriptionXPath, How.XPath).GetAttribute("innerText");
            var str = "";
            if (desc.Length > 0)
            {
                int m = desc.IndexOf("\n") + 1;
                str = desc.Substring(m);
                str = str.Replace(System.Environment.NewLine, string.Empty);
            }
            return str;
        }
        
        public PreAuthActionPage ClosePopupOnPreAuthActionPage(string popupName)
        {
            var preAuthActionPageObject = Navigator.Navigate<PreAuthActionPageObjects>(() =>
            {
                SiteDriver.SwitchWindowByTitle(popupName);
                SiteDriver.CloseWindow();
                SiteDriver.SwitchWindowByTitle(Extensions.GetStringValue(PageTitleEnum.PreAuthorization));
                StringFormatter.PrintMessage("Close popup and switch back to Pre Auth Action Page.");
            });
            return new PreAuthActionPage(Navigator, preAuthActionPageObject, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }
        
        
        public ClaimHistoryPage ClosePopupOnPatientPreAuthHistory(string popupName)
        {
            var claimHistoryPageObject = Navigator.Navigate<ClaimHistoryPageObjects>(() =>
            {
                SiteDriver.SwitchWindowByTitle(popupName);
                SiteDriver.CloseWindow();
                SiteDriver.SwitchWindowByTitle(Extensions.GetStringValue(PageTitleEnum.PatientPreAuthHistory));
                StringFormatter.PrintMessage("Close popup and switch back to Patient Pre Auth History Page.");
            });
            return new ClaimHistoryPage(Navigator, claimHistoryPageObject, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }
        
        #endregion
    }
}

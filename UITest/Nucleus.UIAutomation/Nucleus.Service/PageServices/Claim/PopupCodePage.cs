using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nucleus.Service.PageObjects.Claim;
using Nucleus.Service.PageServices.Base;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Navigation;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Elements;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Common.Constants;
using Nucleus.Service.Support.Environment;
using UIAutomation.Framework.Database;
using UIAutomation.Framework.Utils;
using Extensions = Nucleus.Service.Support.Utils.Extensions;

namespace Nucleus.Service.PageServices.Claim
{
    public class PopupCodePage : NewBasePageService
    {
        #region PRIVATE FIELDS

        private PopupCodePageObjects _popCodePage;
        private string _currentWindowHandle;

        #endregion

        #region PUBLIC PROPERTIES

        public new string PageTitle { get { return _popCodePage.PageTitle; } }
        #endregion

        #region CONSTRUCTOR

        public PopupCodePage(INewNavigator navigator, PopupCodePageObjects diagnosisCodePage, ISiteDriver siteDriver, IJavaScriptExecutors javaScriptExecutors, IEnvironmentManager environmentManager, IBrowserOptions browserOptions, IOracleStatementExecutor executor)
            : base(navigator, diagnosisCodePage, siteDriver, javaScriptExecutors, environmentManager, browserOptions, executor)
        {
            _popCodePage = (PopupCodePageObjects)PageObject;
        }

        #endregion

        #region PUBLIC METHODS

        public string GetTextValueWithInPTag()
        {
            return SiteDriver.FindElement(string.Format(PopupCodePageObjects.ToolTipContentTemplate, 3, "/p"), How.XPath).Text.Trim();
        }

        public string GetPopupHeaderText()
        {
            return SiteDriver.FindElement(PopupCodePageObjects.PopupHeaderTextXPath, How.XPath).Text.Trim();
        }

        public string GetTextValue()
        {
            return SiteDriver.FindElement(string.Format(PopupCodePageObjects.ToolTipContentTemplate, 1, ""), How.XPath).Text.Trim();
        }

        public static void AssignPageTitle(string title)
        {
            PopupCodePageObjects.AssignPageTitle = title;
        }

        public int CountWindowHandles(string title)
        {
            return SiteDriver.WindowHandles.Count(handle => SiteDriver.GetHandleByTitle(title).Equals(handle));
        }

        public ClaimActionPage SwitchBackToNewClaimActionPage()
        {
            var newClaimActionPageObject = Navigator.Navigate<ClaimActionPageObjects>(() =>
                {
                    SiteDriver.SwitchWindowByTitle(PageTitleEnum.ClaimAction.GetStringValue());
                    StringFormatter.PrintMessage("Switch back to New Claim Action Page.");
                });
            return new ClaimActionPage(Navigator, newClaimActionPageObject, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public ClaimActionPage ClosePopup(string popupName)
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

        #endregion
    }
}

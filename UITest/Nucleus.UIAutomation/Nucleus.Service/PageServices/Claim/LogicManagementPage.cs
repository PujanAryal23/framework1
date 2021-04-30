using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nucleus.Service.PageObjects.Claim;
using Nucleus.Service.PageServices.Base;
using Nucleus.Service.SqlScriptObjects.Logic;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Common;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Core.Navigation;
using UIAutomation.Framework.Elements;
using UIAutomation.Framework.Utils;
using UIAutomation.Framework.Common.Constants;
using Nucleus.Service.Support.Environment;
using UIAutomation.Framework.Database;

namespace Nucleus.Service.PageServices.Claim
{
    public class LogicManagementPage : NewBasePageService
    {
        #region PRIVATE PROPERTIES

        private readonly LogicManagementPageObjects _logicManagementPage;

        #endregion

        #region CONSTRUCTOR

        public LogicManagementPage(INewNavigator navigator, LogicManagementPageObjects claimActionPage, ISiteDriver siteDriver, IJavaScriptExecutors javaScriptExecutors, IEnvironmentManager environmentManager, IBrowserOptions browserOptions, IOracleStatementExecutor executor)
            : base(navigator, claimActionPage, siteDriver, javaScriptExecutors, environmentManager, browserOptions, executor)
        {
            _logicManagementPage = (LogicManagementPageObjects)PageObject;
        }

        #endregion


        #region DB methods    

        #endregion

        #region PUBLIC METHODS

        /// <summary>
        /// Create new logic request
        /// </summary>
        /// <param name="notesText"></param>
        public void CreateNewLogicRequest(string notesText)
        {
            try
            {
                SiteDriver.FindElement(LogicManagementPageObjects.LogicMessageTextAreaXPath, How.XPath).SendKeys(notesText);
                SiteDriver.FindElement(LogicManagementPageObjects.SaveBtnXPath, How.XPath).Click();
            }
            catch
            {
                SiteDriver.FindElement(LogicManagementPageObjects.LogicMessageTextAreaXPath, How.XPath).SendKeys(notesText);
                JavaScriptExecutor.ExecuteClick(LogicManagementPageObjects.SaveBtnXPath, How.XPath);
            }
            SiteDriver.WaitToLoadNew(1000);
        }

        /// <summary>
        /// Close logic management page and wait
        /// </summary>
        public void CloseLogicManagementPageAndWait()
        {
            SiteDriver.CloseWindow(_logicManagementPage.OriginalWindowHandle);
        }

        /// <summary>
        /// Close data popup adn switch to original window
        /// </summary>
        /// <param name="pageTitleEnum"></param>
        public ClaimActionPage CloseDataPopupAndSwitchToOrignalWindow(PageTitleEnum pageTitleEnum)
        {
            var claimActionPage = Navigator.Navigate<ClaimActionPageObjects>(() =>
            {
                SiteDriver.CloseWindow();
                Console.WriteLine("Close Logic Management Page");
                SiteDriver.SwitchWindowByTitle(pageTitleEnum.GetStringValue());
            });
            return new ClaimActionPage(Navigator, claimActionPage, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        /// <summary>
        /// Close data popup adn switch to original window
        /// </summary>
        public ClaimActionPage CloseDataPopupAndSwitchToOrignalWindow()
        {
            var newClaimAction = Navigator.Navigate<ClaimActionPageObjects>(() =>
            {
                SiteDriver.CloseWindow();
                SiteDriver.SwitchWindowByTitle(PageTitleEnum.ClaimAction.GetStringValue());
            });
            return new ClaimActionPage(Navigator, newClaimAction, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public string GetAssignedTo()
        {
            return SiteDriver.FindElement(LogicManagementPageObjects.AssignedToXPath, How.XPath).Text;
        }


        public bool LogicRequestList()
        {
            return SiteDriver.IsElementPresent(LogicManagementPageObjects.LogicRequestListCssLocator, How.CssSelector, 5000);
        }

        public bool IsLogicManagerPageTitlePresent()
        {
            return SiteDriver.IsElementPresent(LogicManagementPageObjects.PageTitleCssLocator, How.CssSelector);
        }

        public bool IsLogicLogicFormForSelectedFlagOpenAndPresent(string flag)
        {
            return SiteDriver.IsElementPresent(string.Format(LogicManagementPageObjects.CreateLogicFormXpathTemplateForFlagSelected, flag), How.XPath);
        }

        public void SelectDropDownListValueByLabel(int value, bool directSelect = true)
        {
            SiteDriver.FindElement<Section>(LogicManagementPageObjects.SelectDropDownCssSelector, How.CssSelector).ClickWithOutWait();

            SiteDriver.FindElement<Section>(string.Format(LogicManagementPageObjects.DropDownListForStatusCssTemplate, value), How.CssSelector).ClickWithOutWait();
            SiteDriver.FindElement<Section>(LogicManagementPageObjects.SelectDropDownCssSelector, How.CssSelector).ClickWithOutWait();

        }


        public string GetSelectedDropDownListValueByLabel(string label)
        {
            return SiteDriver.FindElement(string.Format(LogicManagementPageObjects.DropdownListDefaultSelectedValueXpath, label), How.XPath).Text;
        }


        public void ClickOnButtonByButtonName(string flag, string value = "Save", int row = 1)
        {
            JavaScriptExecutor.ClickJQuery(string.Format(LogicManagementPageObjects.SaveUpdateButtonForGivenFlagRowXpathTemplate, flag, value, row));
            Console.WriteLine(value + " Button Clicked");
        }

        public void ClickOnCanelLinkForFlagAndRow(string flag, int row = 1)
        {
            JavaScriptExecutor.ClickJQuery(string.Format(LogicManagementPageObjects.CancelButtonXpathTemplateForFlagAndRow, flag, row));
            Console.WriteLine("Cancel Button Clicked for flag: " + flag + " of " + row + "st/nth row");
        }

        public bool IsReplyIconPresent(string flag, string date)
        {
            return JavaScriptExecutor.IsElementPresent(string.Format(LogicManagementPageObjects.ReplyIconCssSelector, flag, date));
        }

        public bool IsEditIconPresent(string flag, string date)
        {
            return JavaScriptExecutor.IsElementPresent(string.Format(LogicManagementPageObjects.EditIconCssSelector, flag, date));
        }

        public void ClickOnEditIcon(string flag, string date)
        {
            JavaScriptExecutor.ClickJQuery(string.Format(LogicManagementPageObjects.EditIconCssSelector, flag, date));
        }

        public void ClickOnReplyIcon(string flag, string date)
        {
            JavaScriptExecutor.ClickJQuery(string.Format(LogicManagementPageObjects.ReplyIconCssSelector, flag, date));
        }

        public void ClickOnUpdateButton()
        {
            SiteDriver.FindElementAndClickOnCheckBox(LogicManagementPageObjects.UpdateLogicNoteButtonXpath, How.XPath);
        }

        public string GetUpdatedLogicNoteText(string name)
        {
            var logicNote = SiteDriver.FindElement(LogicManagementPageObjects.UpdatedLogicNoteXpath, How.XPath).Text;
            return logicNote;
        }



        #endregion
    }
}

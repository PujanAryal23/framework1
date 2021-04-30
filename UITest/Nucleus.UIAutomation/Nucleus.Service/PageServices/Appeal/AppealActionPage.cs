using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Permissions;
using System.Security.Policy;
using System.Windows.Forms;
using Nucleus.Service.PageObjects.Appeal;
using Nucleus.Service.PageObjects.Claim;
using Nucleus.Service.PageServices.Base.Default;
using Nucleus.Service.PageServices.Claim;
using Nucleus.Service.Support.Common.Constants;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Menu;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Core.Navigation;
using UIAutomation.Framework.Elements;
using UIAutomation.Framework.Utils;
using Nucleus.Service.PageObjects.Default;
using Nucleus.Service.PageObjects.Login;
using Nucleus.Service.PageServices.Login;
using Nucleus.Service.SqlScriptObjects.Appeal;
using Nucleus.Service.SqlScriptObjects.EditSettingsManager;
using Nucleus.Service.SqlScriptObjects.Settings;
using Nucleus.Service.Support.Common;
using Nucleus.Service.Support.Environment;
using UIAutomation.Framework.Database;
using static System.Console;
using static System.String;
using Keys = OpenQA.Selenium.Keys;

namespace Nucleus.Service.PageServices.Appeal
{
    public class AppealActionPage : NewDefaultPage
    {
        
          #region PRIVATE PROPERTIES

        private AppealActionPageObjects _appealActionPage;
        private BillActionPage _billAction;
        private ClaimActionPage _claimAction;
        private AppealSearchPage _appealSearch;
        //private SideBarPanelSearch _sideBarPanelSearch;
        public bool IsNewClaimAction = false;
        public string ClaimSequenceData =  Empty;
        public string ClaimActionPageTitle =  Empty;
        private readonly string _originalWindow;
        private readonly SubMenu _subMenu;
       
        #endregion

        #region CONSTRUCTOR

        public AppealActionPage(INewNavigator navigator, AppealActionPageObjects newAppealActionPage, ISiteDriver siteDriver, IJavaScriptExecutors javaScriptExecutors, IEnvironmentManager environmentManager, IBrowserOptions browserOptions, IOracleStatementExecutor executor,bool handlePopup=true)
            : base(navigator, newAppealActionPage, siteDriver, javaScriptExecutors, environmentManager, browserOptions, executor)
        {
            _appealActionPage = (AppealActionPageObjects)PageObject;
            //_sideBarPanelSearch = new SideBarPanelSearch(new CalenderPage(SiteDriver), SiteDriver, JavaScriptExecutor);
            _subMenu = new SubMenu(SiteDriver, JavaScriptExecutor);
            _originalWindow = SiteDriver.CurrentWindowHandle;
            if(handlePopup)
                HandleAutomaticallyOpenedActionPopup();
            
        }

        #endregion

        #region database interaction

        public List<string> GetAppealReasonCodesFromDB(string product, string action)
        {
            var listOfReasonCodesFromDB = Executor.GetTableSingleColumn(Format(AppealSqlScriptObjects.GetAppealReasonCodesSqlScript, product, action)).ToList();
            return listOfReasonCodesFromDB;
        }

        public List<List<string>> GetExpectedConsultantRationalesList()
        {
            var list =
                Executor.GetCompleteTable(AppealSqlScriptObjects.ConsultantRationales);
            return list.Select(row => row.ItemArray.Select(x => x.ToString()).ToList()).ToList();
        }

        public string AncillarySettingValuesForFOTandFREfromDB(string FOTorFRE)
        {
            var ancillaryValue = Executor.GetSingleStringValue(Format(EditSettingsManagerSqlScriptObjects.GetAncillarySettingValueForFOTandFRE,
                FOTorFRE == "FOT" ? "FOTVAL" : "FREVAL"));

            return ancillaryValue;
        }


        #endregion

        //public SideBarPanelSearch GetSideBarPanelSearch
        //{
        //    get { return _sideBarPanelSearch; }

        //}

        public ClaimActionPage CloseAppealActionWindow()
        {
            var newClaimAction = Navigator.Navigate<ClaimActionPageObjects>(() =>
            {
                SiteDriver.CloseWindow();
                SiteDriver.SwitchWindowByUrl(PageUrlEnum.ClaimAction.GetStringValue());
            });
            return new ClaimActionPage(Navigator, newClaimAction, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);

        }

        public bool IsAppealProcessingHistory()
        {
            return SiteDriver.Title.Equals(PageTitleEnum.AppealProcessingHistory.GetStringValue());
        }

        public bool IsAppealNotePagePresent()
        {
            return SiteDriver.Title.Equals(PageTitleEnum.AppealNotes.GetStringValue());
        }

        public int GetWindowHandlesCount()
        {
            return SiteDriver.WindowHandles.Count;
        }

        public ClaimActionPage SwitchClaimActionPopup(string url)
        {
            //ClaimActionPopup.AssignUrl(claimSeq);
            var newClaimAction = Navigator.Navigate<ClaimActionPageObjects>(() =>
            {
                SiteDriver.SwitchWindowByUrl(url);
                SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(DefaultPageObjects.PageHeaderCssLocator, How.CssSelector),10000);
                StringFormatter.PrintMessage("Switch Claim Action Popup.");
            });
            return new ClaimActionPage(Navigator, newClaimAction, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public AppealActionPage SwitchBackToAppealActionPage()
        {
            var newAppealAction = Navigator.Navigate<AppealActionPageObjects>(() =>
            {
                SiteDriver.SwitchWindow(_originalWindow);
                StringFormatter.PrintMessage("Switch back to New Appeal Action Page.");
            });
            return new AppealActionPage(Navigator, newAppealAction, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor, false);
        }

        public AppealActionPage ClosePopupOnAppealActionPage(string popupUrl)
        {

            var newAppealAction = Navigator.Navigate<AppealActionPageObjects>(() =>
            {
                SiteDriver.SwitchWindowByUrl(popupUrl);
                if(SiteDriver.CurrentWindowHandle!=_originalWindow)
                    SiteDriver.CloseWindow();
                SiteDriver.SwitchWindow(_originalWindow);
                StringFormatter.PrintMessage("Close popup and switch back to New Appeal Action Page.");
            });
            return new AppealActionPage(Navigator, newAppealAction, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor, false);
        }

        public int GetAppealLockCountByUser(string username)
        {
            return (int)Executor.GetSingleValue(string.Format(AppealSqlScriptObjects.AppealCountByUser, username));
        }

       
        #region Adjust appeal line


        public List<string> GetEditFlagLineValueByFlag(string flag)
        {
            return
                JavaScriptExecutor.FindElements(
                     Format(AppealActionPageObjects.EditFlagLineValueListByFlagCssTemplate, flag), "Text");
        }

        public int GetDeletedFlagLineCount()
        {
            return SiteDriver.FindElementsCount(AppealActionPageObjects.DeletedFlagLinesXpathTemplate, How.XPath);
        }

        public int GetNotDeletedFlagLineCount(string linNo)
        {
            return SiteDriver.FindElementsCount( Format(AppealActionPageObjects.FlagLinesXpathTemplate, linNo), How.XPath);
        }


        public List<string> GetEditFlagLineLeByFlag(string flag)
        {
            return
                JavaScriptExecutor.FindElements(
                     Format(AppealActionPageObjects.EditFlagLineLabelListByFlagCssTemplate, flag), "Text");
        }

        public List<string> GetFlagListOnAdjustAppealLine()
        {
            return
                JavaScriptExecutor.FindElements(
                    AppealActionPageObjects.FlagListAdjustAppealLineCssLocator, "Text");
        }

        public bool IsDeletedFlagPresentByFlag(string flag)
        {
            return
                JavaScriptExecutor.IsElementPresent(
                     Format(AppealActionPageObjects.EditFlagLineByDeletedFlagCssTemplate, flag));
        }
        public bool IsDeletedFlagPresentByRow(int row)
        {
            return
                JavaScriptExecutor.IsElementPresent(
                     Format(AppealActionPageObjects.EditFlagLineByDeletedRowCssTemplate, row));
        }
        public bool IsEditIconPresentByRow(int row)
        {
            return
                SiteDriver.IsElementPresent(
                     Format(AppealActionPageObjects.EditIconAdjustAppealLineByRowCssTemplate, row),How.CssSelector);
        }

        public bool IsDeleteIconPresentByRow(int row,bool checkDisabled=false)
        {
            if (checkDisabled)
            {
                var a=SiteDriver.FindElement(
                         Format(AppealActionPageObjects.DeleteIconAdjustAppealLineByRowCssTemplate, 2),
                        How.CssSelector);
                return a.Enabled;
            }
            return
                SiteDriver.IsElementPresent(
                     Format(AppealActionPageObjects.DeleteIconAdjustAppealLineByRowCssTemplate, row), How.CssSelector);
        }

        public bool IsRestoreIconPresentDisabledByRow(int row,bool disable=false)
        {
            if (disable)
                return SiteDriver.FindElement(
                     Format(AppealActionPageObjects.RestoreIconAdjustAppealLineByRowCssTemplate, row),
                    How.CssSelector).GetAttribute("Class").Contains("is_disabled");
            return
                SiteDriver.IsElementPresent(
                     Format(AppealActionPageObjects.RestoreIconAdjustAppealLineByRowCssTemplate, row), How.CssSelector);
        }

        public bool IsApproveIconDisabled()
        {
                return SiteDriver.FindElement(AppealActionPageObjects.approveIconCssLocator, How.CssSelector)
                .GetAttribute("class").Contains("is_disabled");
            
        }

        public bool IsSwitchIconPresentByRow(int row)
        {
            return
                SiteDriver.IsElementPresent(
                     Format(AppealActionPageObjects.SwitchIconAdjustAppealLineByRowCssTemplate, row), How.CssSelector);
        }

        public bool IsSwitchDeleteIconDisabledByRow(int row=1,bool disable=false)
        {
            if (disable)
                return SiteDriver.FindElement(
                     Format(AppealActionPageObjects.DeleteIconAdjustAppealLineByRowCssTemplate, row),
                    How.CssSelector).GetAttribute("Class").Contains("is_disabled");

            return SiteDriver.FindElement(
                     Format(AppealActionPageObjects.SwitchIconAdjustAppealLineByRowCssTemplate, row),
                    How.CssSelector).GetAttribute("Class").Contains("is_disabled");
         

        }

        public void ClickOnSwitchIconByRow(int row = 1)
        {
            JavaScriptExecutor.ClickJQuery( Format(AppealActionPageObjects.SwitchIconAdjustAppealLineByRowCssTemplate, row));
        }

        public void ClickOnDeleteIconByRow(int row = 1)
        {
            JavaScriptExecutor.ClickJQuery(Format(AppealActionPageObjects.DeleteIconAdjustAppealLineByRowCssTemplate, row));
        }

        public int GetEditFlagsRowCount()
        {
            return SiteDriver.FindElementsCount(AppealActionPageObjects.EditFlagLineCssLocator, How.CssSelector);
        }

        public void ClickOnEditFlagByFlag(string flag)
        {
            JavaScriptExecutor.ClickJQuery( Format(AppealActionPageObjects.EditIconAdjustAppealLineByFlagCssTemplate, flag));
        }

        public void ClickOnSwitchFlagByFlag(string flag)
        {
            JavaScriptExecutor.ClickJQuery( Format(AppealActionPageObjects.SwitchIconAdjustAppealLineByFlagCssTemplate, flag));
        }

        public bool IsEditFlagIconEnabled(string flag)
        {
            return JavaScriptExecutor.IsElementPresent(
                 Format(AppealActionPageObjects.EditIconAdjustAppealLineByFlagCssTemplate, flag) +
                ".is_active");

        }

        public bool IsDeleteFlagIconEnabled(string flag)
        {
            return JavaScriptExecutor.IsElementPresent(
                 Format(AppealActionPageObjects.DeleteIconAdjustAppealLineByFlagCssTemplate, flag) +
                ".is_active");

        }
        public bool IsSwitchIconPresentByFlag(string flag)
        {
            return JavaScriptExecutor.IsElementPresent(
                 Format(AppealActionPageObjects.SwitchIconAdjustAppealLineByFlagCssTemplate, flag));
        }
        public bool IsEditFlagDivPresent()
        {
            return SiteDriver.IsElementPresent(AppealActionPageObjects.EditFlagDivCssLocator, How.CssSelector);
        }

        public bool IsInputFieldPresentByLabel(string label)
        {
            return SiteDriver.IsElementPresent(
                 Format(AppealActionPageObjects.InputFieldByLabelXPathTemplate, label), How.XPath);

        }
        public void SetInputFieldByLabel(string label,string value,bool pressTabKey=false)
        {
            SiteDriver.FindElement( Format(AppealActionPageObjects.InputFieldByLabelXPathTemplate,label),How.XPath).ClearElementField();
            SiteDriver.FindElement(Format(AppealActionPageObjects.InputFieldByLabelXPathTemplate, label), How.XPath).SendKeys(value);
            if(pressTabKey)
                SiteDriver.FindElement( Format(AppealActionPageObjects.InputFieldByLabelXPathTemplate, label), How.XPath).SendKeys(Keys.Tab);
        }

        public string GetInputFieldByLabel(string label)
        {
          return   SiteDriver.FindElement( Format(AppealActionPageObjects.InputFieldByLabelXPathTemplate, label), How.XPath)
                .GetAttribute("value");
        }

        public void ClickDeleteIcononEditFlag()
        {
            JavaScriptExecutor.ExecuteClick(AppealActionPageObjects.LineDeleteCssLocator,How.CssSelector);
        }
        public bool IsLineDeleteRestoreIconDisabled(bool restore=false)
        {
            if (restore)
            {
               return SiteDriver.FindElement(
                    AppealActionPageObjects.LineRestoreCssLocator,How.CssSelector).GetAttribute("Class").Contains("is_disabled");
            }
            return SiteDriver.FindElement(
                AppealActionPageObjects.LineDeleteCssLocator, How.CssSelector).GetAttribute("Class").Contains("is_disabled");
        }

        public void ClickOnLabel(string label)
        {
            var element = SiteDriver.FindElement(Format("//label[text()='{0}']", label), How.XPath);
            SiteDriver.ElementToBeClickable(element);
            SiteDriver.WaitToLoadNew(500);
            element.Click();
        }

        public void ClickOnCancelLinkEditFlag()
        {
            var element = SiteDriver.FindElement(AppealActionPageObjects.CancelLinkEditFlagCssLocator, How.CssSelector);
            SiteDriver.ElementToBeClickable(element);
            SiteDriver.WaitToLoadNew(500);
            element.Click();
        }

        public bool IsDeleteAppealLineIconPresentByRow(int row=1)
        {
            return
                SiteDriver.IsElementPresent(
                     Format(AppealActionPageObjects.AppealLineDeleteIconCssLocator, row+1), How.CssSelector);
        }

        public bool IsDeleteAppealLineIconEnabled()
        {
            return SiteDriver.IsElementPresent(AppealActionPageObjects.EnabledAppealLineDeleteIconsCssSelector,How.CssSelector);
        }

        public void ClickOnDeleteAppealLineIcon(int row=1)
        {
            var element =
                SiteDriver.FindElement(Format(AppealActionPageObjects.AppealLineDeleteIconCssLocator, row + 1),
                    How.CssSelector);
            SiteDriver.ElementToBeClickable(element);
            SiteDriver.WaitToLoadNew(500);
            element.Click();
        }
        public string GetDeleteAppealLineIconTooltip(int row)
        {
            return
                SiteDriver.FindElement(
                     Format(AppealActionPageObjects.AppealLineDeleteIconListElementsCssLocator, row+1), How.CssSelector).GetAttribute("title");
        }

        public int GetCountOfAppealLine()
        {
            return SiteDriver.FindElementsCount(AppealActionPageObjects.AppealLineListXpath, How.XPath);
        }

       
        #endregion



        public string GetCurrentUrl()
        {
            return SiteDriver.Url;
        }

        /// <summary>
        /// Get text from Alert Box
        /// </summary>
        /// <returns></returns>
        public string GetAlertBoxText()
        {
            return Navigator.GetAlertBoxText();
        }

        /// <summary>
        /// Checks if Alert is present
        /// </summary>
        /// <returns></returns>
        public bool IsAlertBoxPresent()
        {
            return Navigator.IsAlertBoxPresent();
        }
        
        /// <summary>
        /// Accepts the alert
        /// </summary>
        public void AcceptAlertBoxIfPresent()
        {
            if (IsAlertBoxPresent())
            {
                Navigator.AcceptAlertBox();
                 WriteLine("Alert Box Accepted");
            }
            else
            {
                 WriteLine("Alert Box wasn't found.");
            }
        }

        /// <summary>
        /// Dismisses the alert
        /// </summary>
        public void DismissAlertBoxIfPresent()
        {
            if (IsAlertBoxPresent())
            {
                Navigator.DismissAlertBox();
                 WriteLine("Alert Box Dismissed");
            }
            else
            {
                 WriteLine("Alert Box wasn't found.");
            }
        }

        /// <summary>
        /// Click on Menu Option
        /// </summary>
        /// <param name="menuOption"></param>
        public void ClickOnMenuSpan(String menuOption)
        {
            var menuToClick = HeaderMenu.GetElementLocatorTemplateSpan(menuOption);
            JavaScriptExecutor.ExecuteClick(menuToClick, How.XPath);
             WriteLine("Navigated to {0}", menuOption);
        }

        /// <summary>
        /// Click on Menu Option
        /// </summary>
        /// <param name="submenuName"></param>
        public void ClickOnMenu(String submenuName)
        {
            JavaScriptExecutor.ExecuteClick( Format(SubMenu.SubMenuByTextXPathTemplate,submenuName), How.XPath);
             WriteLine("Clicked Sub Menu to {0}", submenuName);
        }

        public void ClickOnlyDashboardIcon()
        {

            var element = SiteDriver.FindElement(AppealActionPageObjects.DashboardButtonCssLocator, How.CssSelector);
            SiteDriver.ElementToBeClickable(element);
            SiteDriver.WaitToLoadNew(500);
            element.Click();
            WriteLine("Clicked on Dashboard Button");
        }


        public void ClickOnlyQuickLaunchIcon()
        {
            var element = SiteDriver.FindElement(NewDefaultPageObjects.QuickLaunchButtonXPath, How.XPath);
            SiteDriver.ElementToBeClickable(element);
            SiteDriver.WaitToLoadNew(500);
            element.Click();
            WriteLine("Clicked on Dashboard Button");
        }

        public void ClickOnlyHelpIcon()
        {
            var element = SiteDriver.FindElement(AppealActionPageObjects.DashboardButtonCssLocator, How.CssSelector);
            SiteDriver.ElementToBeClickable(element);
            SiteDriver.WaitToLoadNew(500);
            element.Click();
            WriteLine("Clicked on Help Button");
        }

        public void ClickOnlySearchIcon()
        {
            JavaScriptExecutor.ExecuteClick(AppealActionPageObjects.SearchIconCssLocator, How.CssSelector);
        }

        /// <summary>
        /// return all submenu 
        /// </summary>
        /// <returns></returns>
        public List<string> GetAllSubMenuList()
        {
           return _subMenu.GetAllSubMenuList();
        }

        #region PUBLIC METHODS
        /// <summary>
        ///A claim action popup is opened when a page is redirected to appeal action if there is at least one claim line,
        ///this popup is closed by switching handle to appeal action page .
        /// </summary>
        public void HandleAutomaticallyOpenedActionPopup(int currentlyOpenTabs=1)
        {
            SiteDriver.WaitForCondition(() => SiteDriver.WindowHandles.Count > currentlyOpenTabs, 7000);
            if (SiteDriver.WindowHandles.Count > currentlyOpenTabs)
            {
                string originalWindow = SiteDriver.CurrentWindowHandle;
                

                SiteDriver.WaitForCondition(() =>
                {
                     SiteDriver.SwitchWindow(SiteDriver.WindowHandles[SiteDriver.WindowHandles.Count-1]); 
                    if (
                         Compare(SiteDriver.Title, PageTitleEnum.ClaimAction.GetStringValue(),
                            StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        _claimAction = new ClaimActionPage(Navigator, new ClaimActionPageObjects(), SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
                        IsNewClaimAction = true;
                        ClaimSequenceData = _claimAction.GetClaimSequence();
                        ClaimActionPageTitle = SiteDriver.Title;
                        SiteDriver.CloseWindowAndSwitchTo(originalWindow);
                         WriteLine("Automatically opened New Claim Action page closed");
                        return true;
                    }
                    if (
                         Compare(SiteDriver.Title, PageTitleEnum.BillAction2.GetStringValue(),
                            StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        _billAction = new BillActionPage(Navigator, new BillActionPageObjects(), SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
                        IsNewClaimAction = false;
                        ClaimSequenceData = _billAction.GetClaSeqTextLabel();
                        ClaimActionPageTitle = SiteDriver.Title;
                        SiteDriver.CloseWindowAndSwitchTo(originalWindow);
                         WriteLine(
                            "Automatically opened New Bill Action page closed");
                        return true;
                    }

                    return false;
                });
            }

            if (SiteDriver.WindowHandles.Count > currentlyOpenTabs)
            {
                try
                {
                    SiteDriver.SwitchWindow(SiteDriver.WindowHandles[SiteDriver.WindowHandles.Count - 1]); 
                    if (!SiteDriver.Title.Equals(PageTitleEnum.ClaimAction.GetStringValue())
                        && !SiteDriver.Title.Equals(PageTitleEnum.BillAction2.GetStringValue()))
                    {
                         WriteLine(
                            "Expected Title: 'Nucleus: Bill Action' or 'Nucleus: Claim Action' Actual Title: '" +
                            SiteDriver.Title + "'");
                        ClaimActionPageTitle = SiteDriver.Title;
                        SiteDriver.CloseWindowAndSwitchTo(_originalWindow);
                         WriteLine("Automatically opened page closed");
                    }
                }
                catch
                {
                    SiteDriver.SwitchWindow(_originalWindow);
                }
            }
        }


        #region disabled_icon_validation

        public bool IsInputFieldDisabledByLabel(string label)
        {
            var val=   SiteDriver.IsElementPresent( Format(AppealActionPageObjects.DisabledInputFieldByLabelXPathTemplate,label), How.XPath);
            return val;
        }

        public bool IsTextAreaDisabledByLabel(string label)
        {
            return SiteDriver.IsElementPresent( Format(AppealActionPageObjects.EditAppealLineTextAreaXpathTemplate, label), How.XPath);

        }

        public bool IsIframeEditorDisabledByLabel(string label)
        {
            bool isEditable = true;
            SiteDriver.SwitchFrameByXPath( Format(AppealActionPageObjects.EditAppealLineIframeEditorXpathTemplate, label));
            if (SiteDriver.FindElement("body", How.CssSelector).GetAttribute("contenteditable") == "true")
            {
                isEditable = false;
            }
            SiteDriver.SwitchBackToMainFrame();
            return isEditable;
        }

        public bool IsEditIconDisabled()
        {
            return SiteDriver.IsElementPresent(AppealActionPageObjects.DisabledEditIconCssSelector, How.CssSelector);
        }
        public bool IsAppealNoteIconDisabled()
        {
            return SiteDriver.IsElementPresent(AppealActionPageObjects.DisabledNoteIconCssSelector, How.CssSelector);
        }
        public bool IsAppealLetterIconDisabled()
        {
            return SiteDriver.IsElementPresent(AppealActionPageObjects.DisabledLetterIconCssSelector, How.CssSelector);
        }
        public bool IsAppealDraftIconDisabled()
        {
            return SiteDriver.IsElementPresent(AppealActionPageObjects.DisabledAppealDraftIconCssSelector, How.CssSelector);
        }
        public bool IsVisibleToClientIconDisabled()
        {
            return SiteDriver.IsElementPresent(AppealActionPageObjects.DisabledVisibleToClientCheckBoxCssLocator, How.CssSelector);
        }
        public bool IsAppealDocumentIconDisabled()
        {
            return SiteDriver.IsElementPresent(AppealActionPageObjects.DisabledAppealDocumentCssSelector, How.CssSelector);
        }

        public bool IsSaveDraftButtonDisabled()
        {
            return SiteDriver.IsElementPresent(AppealActionPageObjects.DisabledSaveDraftButton, How.XPath);
        }

        public bool IsAppealLetterIconEnabled()
        {
            return SiteDriver.IsElementPresent(AppealActionPageObjects.EnabledLetterIconCssSelector, How.CssSelector);
        }
        public bool IsAppealDraftIconEnabled()
        {
            return SiteDriver.IsElementPresent(AppealActionPageObjects.EnabledAppealDraftIconCssSelector, How.CssSelector);
        }
        public bool IsAppealLock()
        {
            return SiteDriver.IsElementPresent(AppealActionPageObjects.LockedIconCssSelector, How.CssSelector);
        }

        public bool IsAppealLocked(string appealseq, string userID)
        {
            var lockedAppealList = Executor.GetTableSingleColumn(Format(AppealSqlScriptObjects.AppealLock, userID));

            return lockedAppealList.Any(x => x.Contains(appealseq));

        }

        public string GetAppealLockToolTip()
        {
            return SiteDriver.FindElement(AppealActionPageObjects.LockedIconCssSelector, How.CssSelector).GetAttribute("Title");
        }

        public bool IsSearchIconDisabled()
        {
            return
                SiteDriver.FindElement(AppealActionPageObjects.SearchIconXPath, How.XPath)
                    .GetAttribute("class")
                    .Contains("is_disabled");
        }

        public bool IsMoreOptionDisabled()
        {
            return
                SiteDriver.FindElement(AppealActionPageObjects.MoreOptionsListCssLocator, How.CssSelector)
                    .GetAttribute("class")
                    .Contains("is_disabled");
        }
        #endregion



        public AppealActionPage SwitchClientAndNavigateAppealAction(string clientToSwitch,string preValue,string currentValue)
        {
            var chromeDownLoad =
                Navigator.Navigate<AppealActionPageObjects>(
                    () =>
                        SiteDriver.Open(CurrentPageUrl.Replace(ClientEnum.SMTST.ToString(), clientToSwitch).Replace(preValue, currentValue)));
            WaitForWorking();
            return new AppealActionPage(Navigator, chromeDownLoad,SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        #region Complete
        public bool IsCompleteAppealIconPresent()
        {
            return SiteDriver.IsElementPresent(AppealActionPageObjects.approveIconCssLocator, How.CssSelector);
        }
        #endregion

        #region AppealEmail
        public void ClickonEmailIcon()
        {
            var element = SiteDriver.FindElement(AppealActionPageObjects.EmailIconCssLocator, How.CssSelector);
            SiteDriver.ElementToBeClickable(element);
            SiteDriver.WaitToLoadNew(500);
            element.Click();
            SiteDriver.WaitForCondition(IsAppealEmailFormPresent);
        }

        public bool IsAppealEmailFormPresent()
        {
            return SiteDriver.IsElementPresent(AppealActionPageObjects.AppealEmailFormCssLocator, How.CssSelector);
        }

        public void SetToEmailInfo(string to)
        {
            SiteDriver.FindElement(AppealActionPageObjects.ToInputFieldCssLocator, How.CssSelector).ClearElementField();
            SiteDriver.FindElement(AppealActionPageObjects.ToInputFieldCssLocator, How.CssSelector).SendKeys(to);
        }

        public void SetAdditionalCCEmailInfo(string to)
        {
            SiteDriver.FindElement(AppealActionPageObjects.AdditionalCCInputFieldCssLocator, How.CssSelector).ClearElementField();
            SiteDriver.FindElement(AppealActionPageObjects.AdditionalCCInputFieldCssLocator, How.CssSelector).SendKeys(to);
        }

        public string GetToEmailInfo()
        {
            return SiteDriver.FindElement(AppealActionPageObjects.ToInputFieldCssLocator, How.CssSelector)
                 .GetAttribute("value");
        }

        public string GetAdditionalCCEmailInfo()
        {
            return SiteDriver.FindElement(AppealActionPageObjects.AdditionalCCInputFieldCssLocator, How.CssSelector)
                 .GetAttribute("value");
        }

        public string GetEmailValue()
        {
            return SiteDriver.FindElement(AppealActionPageObjects.EmailValueXPath, How.XPath).Text;
        }

        public string GetClientCCValue()
        {
            return SiteDriver.FindElement(AppealActionPageObjects.ClientCCValueXPath, How.XPath).Text;
        }

        public string GetTextMessage()
        {
            return
                SiteDriver.FindElement(AppealActionPageObjects.TextMessageCssLocator, How.CssSelector).Text.Replace("\r\n", "");
        }

        public void SetNote(String note)
        {
            SiteDriver.SwitchFrameByClass("cke_wysiwyg_frame cke_reset");
            var element = SiteDriver.FindElement("body", How.CssSelector);
            SiteDriver.ElementToBeClickable(element);
            SiteDriver.WaitToLoadNew(500);
            element.Click();
            element.ClearElementField();
            SiteDriver.WaitTillClear(element);
            element.SendKeys(note);
             WriteLine("Note set to {0}", note);
            SiteDriver.SwitchBackToMainFrame();

        }

        public string GetNote()
        {
            SiteDriver.SwitchFrameByClass("cke_wysiwyg_frame cke_reset");
            var note = SiteDriver.FindElement("body", How.CssSelector).Text;
            SiteDriver.SwitchBackToMainFrame();
            return note;
        }

        public void ClickOnCancelLinkOnAppealEmail()
        {
            JavaScriptExecutor.ExecuteClick(AppealActionPageObjects.CancelAppealEmailLinkCssLocator, How.CssSelector);
            
        }
        public void ClickOnSendEmail()
        {
            JavaScriptExecutor.ExecuteClick(AppealActionPageObjects.SendEmailButtonXPath, How.XPath);
            SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
            
        }

        public bool IsAppealEmailEnabled()
        {
            return
                !SiteDriver.FindElement(AppealActionPageObjects.EnabledDisabledAppealEmailXPath, How.XPath)
                    .GetAttribute("class")
                    .Contains("is_disabled");
        }

        public bool IsManageAppealEmailIconPresent()
        {
            return SiteDriver.IsElementPresent(AppealActionPageObjects.EmailIconCssLocator,How.CssSelector);
        }

        public string GetAppealEmailTitle()
        {
            return
                SiteDriver.FindElement(AppealActionPageObjects.EnabledDisabledAppealEmailXPath, How.XPath)
                    .GetAttribute("title");
        }
        #endregion

        #region AppealLetter_AppealMail

        public void ClickOnAppealLetter()
        {
            if (!IsAppealLetterSection())
            {
                var element = SiteDriver.FindElement(AppealActionPageObjects.LetterIconIconCssLocator, How.CssSelector);
                SiteDriver.ElementToBeClickable(element);
                SiteDriver.WaitToLoadNew(500);
                element.Click();
                SiteDriver.WaitForCondition(IsAppealLetterSection);
            }
            else
            {
                var element = SiteDriver.FindElement(AppealActionPageObjects.LetterIconIconCssLocator, How.CssSelector);
                SiteDriver.ElementToBeClickable(element);
                SiteDriver.WaitToLoadNew(500);
                element.Click();
                SiteDriver.WaitForCondition(()=>!IsAppealLetterSection());
            }
            
        }

        public bool IsAppealLetterSection()
        {
            return SiteDriver.IsElementPresent(AppealActionPageObjects.AppealLetterSectionCssLocator, How.CssSelector);
        }

        public void ClickOnApproveIcon()
        {
            var element = SiteDriver.FindElement(AppealActionPageObjects.approveIconCssLocator, How.CssSelector);
            SiteDriver.ElementToBeClickable(element);
            SiteDriver.WaitToLoadNew(500);
            element.Click();

        }
        public void ClickOkOnConfirmationModalWithoutWait()
        {
            var element = SiteDriver.FindElement(NewDefaultPageObjects.OkConfirmationCssSelector, How.CssSelector);
            SiteDriver.ElementToBeClickable(element);
            SiteDriver.WaitToLoadNew(500);
            element.Click();
            //WaitForWorking();
            WriteLine("Ok Button is Clicked");
        }

        public void ClickOnRefreshIcon()
        {
            var element = SiteDriver.FindElement(AppealActionPageObjects.RefreshIconCssLocator, How.CssSelector);
            SiteDriver.ElementToBeClickable(element);
            SiteDriver.WaitToLoadNew(500);
            element.Click();
        }

        public bool IsRefreshIconPresent()
        {
            return SiteDriver.IsElementPresent(AppealActionPageObjects.RefreshIconCssLocator, How.CssSelector);
        }

        public string GetLetterBody()
        {
            var body =
                SiteDriver.FindElement(AppealActionPageObjects.LetterBodyCssLocator, How.CssSelector).Text;
            return body;
        }

        public List<string> GetDocTypeInAppealLetterBody(bool popUp=false)
        {
          if(popUp)
              return SiteDriver.FindDisplayedElementsText(AppealActionPageObjects.DocumentTypeInLetterBody, How.CssSelector);
          else
            return  SiteDriver.FindDisplayedElementsText(AppealActionPageObjects.GetDocumentTypeInAppealLetter, How.CssSelector);
            
        }

        public string GetLetterBodyForDCIAppealLetter()
        {
            var body =
                SiteDriver.FindElement(AppealActionPageObjects.LetterBodyForDCICssLocator, How.CssSelector).Text;
            return body;
        }

        public string GetAppealLetterClaimLineContent(int row)
        {
           return SiteDriver.FindElement(
                 Format(AppealActionPageObjects.AppealLetterClaimLineXpathTemplate, row), How.XPath).Text;
           
        }

        public string GetAppealLetterFooterText()
        {
            return SiteDriver.FindElement(AppealActionPageObjects.AppealLetterFooterXpath, How.XPath)
                .Text;
        }

        #endregion

        #region AppealInformation

        public bool IsAppealInformationByLabel(string label)
        {
            return SiteDriver.IsElementPresent(
                 Format(AppealActionPageObjects.AppealInfoByLabelXPathTemplate, label), How.XPath);
        }

        public bool IsEditAppealInputDivPresentByLabel(string label)
        {
            return SiteDriver.IsElementPresent( Format(AppealActionPageObjects.EditAppealInputDivByLabelXPathTemplate,label),
                How.XPath);
        }

        public string GetAppealInformationByLabel(string label)
        {
            return SiteDriver.FindElement(
                 Format(AppealActionPageObjects.AppealInfoByLabelXPathTemplate, label), How.XPath).Text.Trim();
        }

        public string GetAppealInformationToolTipByLabel(string label)
        {
            return SiteDriver.FindElement(
                 Format(AppealActionPageObjects.AppealInfoByLabelXPathTemplate, label), How.XPath).GetAttribute("title").Trim();
        }

        public bool IsEllipsisPresentAppealInformationByLabel(string label)
        {
           return SiteDriver.FindElement(
                 Format(AppealActionPageObjects.AppealEllipsisByLabelXPathTemplate, label), How.XPath)
                .GetCssValue("text-overflow") == "ellipsis";
        }
        #endregion

        public void ClickOnSaveButton()
        {
            var element = SiteDriver.FindElement(AppealActionPageObjects.SaveButtonXPath, How.XPath);
            SiteDriver.ElementToBeClickable(element);
            SiteDriver.WaitToLoadNew(500);
            element.Click();
            WaitForWorking();
        }

        public void ClickOnEditIcon()
        {
            var element = SiteDriver.FindElement(AppealActionPageObjects.EditIconCssLocator, How.CssSelector);
            SiteDriver.ElementToBeClickable(element);
            SiteDriver.WaitToLoadNew(500);
            element.Click();
            WaitForWorking();
        }
        public void SelectEditAppealFieldDropDownListByLabel(string label, string value)
        {
            var element = SiteDriver.FindElement(
                Format(AppealActionPageObjects.EditAppealInputFieldByLabelXPathTemplate, label), How.XPath);
            element.ClearElementField();
            SiteDriver.WaitTillClear(element);
            SiteDriver.ElementToBeClickable(element);
            SiteDriver.WaitToLoadNew(500);
            element.Click();
            element.SendKeys(value);
            JavaScriptExecutor.ExecuteClick(
                 Format(AppealActionPageObjects.EditAppealDropDownListValueByLabelXpathTemplate, label, value), How.XPath);
             WriteLine("{0} <{1}> selected", label, value);
        }

        public List<String> GetEditAppealInputListByLabel(string label)
        {
            JavaScriptExecutor.ExecuteClick(
                 Format(AppealActionPageObjects.EditAppealInputFieldByLabelXPathTemplate, label), How.XPath);
            JavaScriptExecutor.ExecuteMouseOver(
                Format(AppealActionPageObjects.EditAppealInputFieldByLabelXPathTemplate, label), How.XPath);
            var list = JavaScriptExecutor.FindElements(
                Format(AppealActionPageObjects.EditAppealDropDownListByLabelXpathTemplate, label), How.XPath,
                "Text");
            JavaScriptExecutor.ExecuteMouseOut(
               Format(AppealActionPageObjects.EditAppealInputFieldByLabelXPathTemplate, label), How.XPath);

            list.RemoveAll(IsNullOrEmpty);
            return list;
        }
        public bool IsClinicallyDeletedFlagPresentByLineNo(string lineNo,string flag)
        {
            return
                SiteDriver.IsElementPresent(
                     Format(AppealActionPageObjects.FlagByLineNoXPathTemplate, lineNo, flag), How.XPath);
        }

        

        public bool IsRedInvalidInActionResult()
        {
            return SiteDriver.IsElementPresent(AppealActionPageObjects.RedInvalidInActionResultCssLocator,
                How.CssSelector);
        }

        public bool IsPayIconEnabled(int claimRow=2,int lineRow=2)
        {
            if (IsAppealResultTypeHiddenInAppealAction(claimRow, lineRow))
                ClickOnAppealResultTypeEllipses(claimRow, lineRow);
            return SiteDriver.IsElementPresent(
                 Format(AppealActionPageObjects.PayIconCssTemplate, claimRow, lineRow)+ ".is_active", How.CssSelector);
        }
        public bool IsPayIconDisabled(int claimRow = 2, int lineRow = 2)
        {
            return SiteDriver.IsElementPresent(
                 Format(AppealActionPageObjects.PayIconCssTemplate, claimRow, lineRow) + ".is_disabled", How.CssSelector);
        }

        public bool IsDenyIconEnabled(int claimRow = 2, int lineRow = 2)
        {
            if (IsAppealResultTypeHiddenInAppealAction(claimRow, lineRow))
                ClickOnAppealResultTypeEllipses(claimRow, lineRow);
            return SiteDriver.IsElementPresent(
                 Format(AppealActionPageObjects.DenyIconCssTemplate, claimRow, lineRow) + ".is_active", How.CssSelector);
        }

        public bool IsAppealResultTypeHiddenInAppealAction(int claimRow = 2, int lineRow = 2)
        {
            return SiteDriver.IsElementPresent(
                Format(AppealActionPageObjects.AppealResultTypeCssTemplate, claimRow, lineRow) + ".is_hidden", How.CssSelector);
        }

        public void ClickOnAppealResultTypeEllipses(int claimRow = 2, int lineRow = 2)
        {
           var element = SiteDriver.FindElement(
                Format(AppealActionPageObjects.AppealResultTypeEllipsesCssTemplate, claimRow, lineRow),
                How.CssSelector);
            SiteDriver.ElementToBeClickable(element);
            SiteDriver.WaitToLoadNew(500);
            element.Click();
        }

        public bool IsAppealResultTypeEllipsesDisabledInAppealAction(int claimRow = 2, int lineRow = 2)
        {
            return SiteDriver.IsElementPresent(
                Format(AppealActionPageObjects.AppealResultTypeEllipsesCssTemplate, claimRow, lineRow) + ".is_disabled", How.CssSelector);
        }

        public bool IsDenyIconDisabled(int claimRow = 2, int lineRow = 2)
        {
            return SiteDriver.IsElementPresent(
                 Format(AppealActionPageObjects.DenyIconCssTemplate, claimRow, lineRow) + ".is_disabled", How.CssSelector);
        }

        public bool IsAdjustIconEnabled(int claimRow = 2, int lineRow = 2)
        {
            if (IsAppealResultTypeHiddenInAppealAction(claimRow, lineRow))
                ClickOnAppealResultTypeEllipses(claimRow, lineRow);
            return SiteDriver.IsElementPresent(
                 Format(AppealActionPageObjects.AdjustIconCssTemplate, claimRow, lineRow) + ".is_active", How.CssSelector);
        }
        public bool IsAdjustIconDisabled(int claimRow = 2, int lineRow = 2)
        {
            return SiteDriver.IsElementPresent(
                 Format(AppealActionPageObjects.AdjustIconCssTemplate, claimRow, lineRow) + ".is_disabled", How.CssSelector);
        }

        public bool IsNoDocsIconEnabled(int claimRow = 2, int lineRow = 2)
        {
            if (IsAppealResultTypeHiddenInAppealAction(claimRow, lineRow))
                ClickOnAppealResultTypeEllipses(claimRow, lineRow);
            return SiteDriver.IsElementPresent(
                 Format(AppealActionPageObjects.NoDocsIconCssTemplate, claimRow, lineRow) + ".is_active", How.CssSelector);
        }
        public bool IsNoDocsIconDisabled(int claimRow = 2, int lineRow = 2)
        {
            return SiteDriver.IsElementPresent(
                 Format(AppealActionPageObjects.NoDocsIconCssTemplate, claimRow, lineRow) + ".is_disabled", How.CssSelector);
        }
       

        public void ClickOnSaveDraftButton(int row=1)
        {
            JavaScriptExecutor.ExecuteClick( Format(AppealActionPageObjects.SaveDraftButtonXPath,row+1),How.XPath);
            WaitForWorking();
        }

        public string GetPayIconBackGroundColor()
        {
            var color= SiteDriver.FindElement( Format(AppealActionPageObjects.PayIconCssTemplate, 2, 2),
                How.CssSelector).GetCssValue("background");
            return color;
        }

        public string GetDenyIconBackGroundColor()
        {
            var color = SiteDriver.FindElement( Format(AppealActionPageObjects.DenyIconCssTemplate, 2, 2),
                How.CssSelector).GetCssValue("background");
            return color;
        }

        public string GetNoDocsIconBackGroundColor()
        {
            var color = SiteDriver.FindElement( Format(AppealActionPageObjects.NoDocsIconCssTemplate, 2, 2),
                How.CssSelector).GetCssValue("background");
            return color;
        }

        public bool IsVisibleToClientCheckBoxChecked()
        {
            return SiteDriver.FindElement(AppealActionPageObjects.VisibleToClientCssLocator, How.CssSelector).Displayed && (SiteDriver.FindElement(AppealActionPageObjects.VisibleToClientCssLocator, How.CssSelector)
                .GetAttribute("checked"))=="true";
        }

        public string GetEditAppealLineTextAreaByHeader(string header)
        {
            return SiteDriver.FindElement(
                 Format(AppealActionPageObjects.EditAppealLineTextAreaXpathTemplate, header), How.XPath)
                .GetAttribute("value");
        }

        public string GetEditAppealLineIframeEditorByHeader(string header)
        {
            SiteDriver.SwitchFrameByXPath( Format(AppealActionPageObjects.EditAppealLineIframeEditorXpathTemplate, header));
            var text = SiteDriver.FindElement("body", How.CssSelector).Text;
            SiteDriver.SwitchBackToMainFrame();
            return text;
        }

        public List<string> GetAllEditAppealLineIframeEditorByHeader( string header, int skipLine =2)
        {
            List<string> list = new List<string>();
            for (int i = 1; i <= GetAppealLineCount(); i++)
            {
                if (skipLine == 1)
                {
                    skipLine++;
                    continue;
                }

                SiteDriver.SwitchFrameByXPath(
                     Format(AppealActionPageObjects.EditAppealLineIframeEditorByRowXpathTemplate, i,
                        header));
                var text = SiteDriver.FindElement("body", How.CssSelector).Text;
                list.Add(text);
                SiteDriver.SwitchBackToMainFrame();
            }

            
            return list;
        }

        public int GetAppealLineCount()
        {
            return SiteDriver.FindElementsCount(AppealActionPageObjects.AppealLineListXpath, How.XPath);
        }

        public string GetEditAppealLineIframeEditorByHeaderAndAppealLineNo(string claSeqInAppealLine, string header, string appealLineNo)
        {
            SiteDriver.SwitchFrameByXPath( Format(AppealActionPageObjects.EditAppealLineIframeEditorByAppealLineNoAndAppealLineClaSeqXpathTemplate, 
                claSeqInAppealLine, header, appealLineNo ));
            var text = SiteDriver.FindElement("body", How.CssSelector).Text;
            SiteDriver.SwitchBackToMainFrame();
            return text;
        }
        public void SetEditAppealLineTextAreaByHeader(string header,string text)
        {
            if (text.Length == 4001)
            {
                JavaScriptExecutor.SendKeys(text.Substring(0, 3990),
                             Format(AppealActionPageObjects.EditAppealLineTextAreaXpathTemplate, header),
                            How.XPath);//selenium couldnot perform sendkeys for long text which causes hung issue so javascript implemented
                
                 SiteDriver.FindElement(
                             Format(AppealActionPageObjects.EditAppealLineTextAreaXpathTemplate, header),
                            How.XPath)
                            .SendKeys(text.Substring(0, 11));
            }
            else
            {
                SiteDriver.FindElement(
                        Format(AppealActionPageObjects.EditAppealLineTextAreaXpathTemplate, header), How.XPath).ClearElementField();
                SiteDriver.FindElement(
                        Format(AppealActionPageObjects.EditAppealLineTextAreaXpathTemplate, header), How.XPath).SendKeys(text);
            }

        }
        /// <summary>
        /// Since RTE automatically preserve 7 charcater for p tag i.e.,<p></p> count=7
        /// </summary>
        /// <param name="header"></param>
        /// <param name="text"></param>
        public void SetEditAppealLineIfrmeEditorByHeader(string header, string text)
        {
            SiteDriver.SwitchFrameByXPath( Format(AppealActionPageObjects.EditAppealLineIframeEditorXpathTemplate, header));
            SendValuesOnTextArea(header, text);
        }


        public bool IsEditAppealLineFormEditable(string header)
        {

            SiteDriver.SwitchFrameByXPath(
                 Format(AppealActionPageObjects.EditAppealLineIframeEditorXpathTemplate, header));
            bool isEditable = SiteDriver.IsElementPresent("body.cke_editable", How.CssSelector);
            SiteDriver.SwitchBackToMainFrame();
            return isEditable;
        }

        public bool IsReasonCodeInProperFormat()
        {
            JavaScriptExecutor.ExecuteClick(AppealActionPageObjects.ReasonCodeInputCssLocator, How.CssSelector);
            var reasonCodeList = JavaScriptExecutor.FindElements(AppealActionPageObjects.ReasonCodeListCssLocator,
                 "Text");
            reasonCodeList.RemoveAt(0);
            JavaScriptExecutor.ExecuteMouseOut(AppealActionPageObjects.ReasonCodeInputCssLocator, How.CssSelector);
            return reasonCodeList.All(s => s.Contains('-'));
        }

        public List<string> GetReasonCodeList()
        {
            JavaScriptExecutor.ExecuteClick(AppealActionPageObjects.ReasonCodeInputCssLocator, How.CssSelector);
            var reasonCodeList = JavaScriptExecutor.FindElements(AppealActionPageObjects.ReasonCodeListCssLocator,
                 "Text");
            reasonCodeList.RemoveAt(0);
            JavaScriptExecutor.ExecuteMouseOut(AppealActionPageObjects.ReasonCodeInputCssLocator, How.CssSelector);
            return reasonCodeList;
        }

        public List<string> GetConsultantRationalesList()
        {
            JavaScriptExecutor.ExecuteClick(AppealActionPageObjects.ConsultantRationalesInputXPath, How.XPath);
            var consultantRationales = JavaScriptExecutor.FindElements(AppealActionPageObjects.ConsultantRationalesListXPath, How.XPath,
                "Text");
            consultantRationales.RemoveAt(0);
            JavaScriptExecutor.ExecuteMouseOut(AppealActionPageObjects.ConsultantRationalesInputXPath, How.XPath);
            return consultantRationales;
        }

        public void SetConsultantRationales(string consultantRationales)
        {
            JavaScriptExecutor.ExecuteClick(AppealActionPageObjects.ConsultantRationalesInputXPath, How.XPath);
            var element = SiteDriver.FindElement(AppealActionPageObjects.ConsultantRationalesInputXPath,
                How.XPath);
            element.ClearElementField();
            SiteDriver.WaitTillClear(element);
            SiteDriver.WaitToLoadNew(200);
            element.SendKeys(consultantRationales);
            if (!IsNullOrEmpty(consultantRationales))
            {
                element = SiteDriver.FindElement(
                    Format(AppealActionPageObjects.ConsultantRationalesValueXPath, consultantRationales),How.XPath);
                SiteDriver.ElementToBeClickable(element);
                SiteDriver.WaitToLoadNew(500);
                element.Click();
            }
            else
                JavaScriptExecutor.ExecuteClick(AppealActionPageObjects.ConsultantRationalesListXPath, How.XPath);
            JavaScriptExecutor.ExecuteMouseOut(AppealActionPageObjects.ConsultantRationalesInputXPath, How.XPath);
        }

        public bool IsConsultantRationalePresent()
        {
            return SiteDriver.IsElementPresent(AppealActionPageObjects.ConsultantRationalesInputXPath, How.XPath);
        }
        public void SetReasonCodeList(string reasonCode)
        {
            JavaScriptExecutor.ExecuteClick(AppealActionPageObjects.ReasonCodeInputCssLocator, How.CssSelector);
            var element = SiteDriver.FindElement(AppealActionPageObjects.ReasonCodeInputCssLocator,
                How.CssSelector);
            element.ClearElementField();
            SiteDriver.WaitTillClear(element);
            SiteDriver.WaitToLoadNew(200);
            element.SendKeys(reasonCode);

            if (!IsNullOrEmpty(reasonCode))
            {
                 element = SiteDriver.FindElement(
                    Format(AppealActionPageObjects.ReasonCodeListXpathTemplate, reasonCode),
                    How.XPath);
                SiteDriver.ElementToBeClickable(element);
                SiteDriver.WaitToLoadNew(500);
                element.Click();
            }
            else
                JavaScriptExecutor.ExecuteClick(AppealActionPageObjects.ReasonCodeListCssLocator, How.CssSelector);
            //JavaScriptExecutor.ExecuteMouseOut(AppealActionPageObjects.ReasonCodeInputCssLocator, How.CssSelector);
        }

        public string GetReasonCodeInput()
        {
            return
                SiteDriver.FindElement(AppealActionPageObjects.ReasonCodeInputCssLocator, How.CssSelector)
                    .GetAttribute("value");
        }

        public List<string> GetAllReasonCodeInput()
        {
            return SiteDriver.FindElementsAndGetAttribute("value", AppealActionPageObjects.ReasonCodeInputCssLocator,
                How.CssSelector);
        }

        public void WaitForWorking()
        {
            
            SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
            SiteDriver.WaitForPageToLoad();
            
        }
       

        public void WaitToLoadPageErrorPopupModal()
        {
            SiteDriver.WaitForCondition(IsPageErrorPopupModalPresent);
        }
        


        public void ClickOnCancelSaveDraft()
        {
            JavaScriptExecutor.ExecuteClick(AppealActionPageObjects.CancelSaveDraftXPath, How.XPath);
        }
        public void ClickOnCancelLink()
        {
            JavaScriptExecutor.ExecuteClick(AppealActionPageObjects.CancelLinkCssLocator,How.CssSelector);
        }

        public void ClickOnAppealLineCancel(bool ifMultipleOpen=false, string linNo="")
        {
            if (!ifMultipleOpen)
            {
                var element = SiteDriver.FindElement(AppealActionPageObjects.AppealLineCancelLinkXPathTemplate,
                    How.XPath);
                SiteDriver.ElementToBeClickable(element);
                SiteDriver.WaitToLoadNew(500);
                element.Click();
            }
            else
            {
                var element =
                    SiteDriver.FindElement(
                        Format(AppealActionPageObjects.AppealLineCancelLinkByLinNoXPathTemplate, linNo), How.XPath);
                SiteDriver.ElementToBeClickable(element);
                SiteDriver.WaitToLoadNew(500);
                element.Click();
            }
        }

        public string GetEditAppealLineHeaderText()
        {
            return
                SiteDriver.FindElement(AppealActionPageObjects.EditAppealHeaderValueCssLocator,
                    How.CssSelector).Text;
        }

        public void ClickOnPayIcon()
        {
            JavaScriptExecutor.ExecuteClick( Format(AppealActionPageObjects.PayIconCssTemplate,2,2),How.CssSelector);
            SiteDriver.WaitForCondition(IsEditAppealLineIframeSectionPresent);
        }

        public void ClickOnDenyIcon(int row = 2)
        {
            JavaScriptExecutor.ExecuteClick( Format(AppealActionPageObjects.DenyIconCssTemplate, 2, row), How.CssSelector);
            SiteDriver.WaitForCondition(IsEditAppealLineIframeSectionPresent);
        }

        public void ClickOnAdjustIcon(int row=2)
        {
            JavaScriptExecutor.ExecuteClick( Format(AppealActionPageObjects.AdjustIconCssTemplate, 2,row), How.CssSelector);
            SiteDriver.WaitForCondition(IsEditAppealLineIframeSectionPresent);
        }


        public void ClickOnHelpIcon(int row = 2)
        {
            JavaScriptExecutor.ExecuteClick(Format(AppealActionPageObjects.AppealHelpIconCssTemplate, 2, row), How.CssSelector);
        }

        public void ClickOnAdjustIconByFlagAndClaimSequence(string claimSequence, string flag)
        {
            JavaScriptExecutor.ExecuteClick(
                 Format(AppealActionPageObjects.AdjustIconXPathTemplateByFlagAndClaimSequence, claimSequence,
                    flag), How.XPath);
            SiteDriver.WaitForCondition(IsEditAppealLineIframeSectionPresent);
        }

        public void ClickOnAdjustAppealLineByLineNo(string lineNo)
        {
            JavaScriptExecutor.ExecuteClick(Format(AppealActionPageObjects.AdjustAppealLineByLineNoXpathTemplate, lineNo), How.XPath);
            SiteDriver.WaitForCondition(IsEditAppealLineIframeSectionPresent);
        }

        public void ClickOnNoDocsIcon()
        {
            JavaScriptExecutor.ExecuteClick( Format(AppealActionPageObjects.NoDocsIconCssTemplate, 2, 2), How.CssSelector);
            SiteDriver.WaitForCondition(IsEditAppealLineIframeSectionPresent);
        }

        public bool IsEditAppealLineSectionPresent()
        {
            return SiteDriver.IsElementPresent(AppealActionPageObjects.EditAppealLineSectionCssLocator,
                How.CssSelector);
        }

        public bool IsEditAppealLineIframeSectionPresent()
        {
            return SiteDriver.IsElementPresent(
                 Format(AppealActionPageObjects.EditAppealLineIframeEditorXpathTemplate, "Rationale"),
                How.XPath);
        }
        public List<String> GetMoreOptionsList()
        {
            return JavaScriptExecutor.FindElements(AppealActionPageObjects.MoreOptionsListCssLocator, How.CssSelector, "Text");
        }

        /// <summary>
        /// Click on View Note Icon
        /// </summary>
        /// <returns>AppealNotePage</returns>
        //public AppealNotePage ClickOnViewNoteIcon()
        //{
        //    var appealNotePage = Navigator.Navigate<AppealNotePageObjects>(() =>
        //    {
        //        var element = SiteDriver.FindElement(AppealActionPageObjects.notesIconCssLocator, How.CssSelector);
        //        SiteDriver.ElementToBeClickable(element);
        //        SiteDriver.WaitToLoadNew(500);
        //        element.Click();
        //        WriteLine("Clicked View Notes Icon");
        //        SiteDriver.WaitForCondition(() => SiteDriver.SwitchWindowByTitle(PageTitleEnum.AppealNotes.GetStringValue()));
        //        SiteDriver.WaitForPageToLoad();
        //        SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(AppealNotePageObjects.NoteFormDivCssLocator,How.CssSelector));
        //    });
        //    return new AppealNotePage(Navigator, appealNotePage, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        //}

        /// <summary>
        /// Click on View Note Icon
        /// </summary>
        /// <returns>AppealNotePage</returns>
        //public AppealNotePage ClickOnAddNoteIcon()
        //{
        //    var appealNotePage = Navigator.Navigate<AppealNotePageObjects>(() =>
        //    {
        //        var element = SiteDriver.FindElement(AppealActionPageObjects.addNotesIconCssLocator, How.CssSelector);
        //        SiteDriver.ElementToBeClickable(element);
        //        SiteDriver.WaitToLoadNew(500);
        //        element.Click();
        //        WriteLine("Clicked Add Note Icon");
        //        SiteDriver.WaitForCondition(() => SiteDriver.SwitchWindowByTitle(PageTitleEnum.AppealNotes.GetStringValue()));
        //        SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(AppealNotePageObjects.NoteFormDivCssLocator, How.CssSelector));
        //        SiteDriver.WaitForPageToLoad();
                
        //    });
        //    return new AppealNotePage(Navigator, appealNotePage, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        //}

        //public bool IsAppealNoteIconPresent()
        //{
        //    return SiteDriver.FindElement(AppealActionPageObjects.notesIconCssLocator, How.CssSelector).Displayed;
        //}

        //public bool IsAddAppealNoteIconPresent()
        //{
        //    return SiteDriver.FindElement(AppealActionPageObjects.addNotesIconCssLocator, How.CssSelector).Displayed;
        //}

        public string GetProviderByClaimGroup(int claimGroupRow = 2)
        {
            return
                SiteDriver.FindElement(
                     Format(AppealActionPageObjects.ProviderValueCssTemplate, claimGroupRow), How.CssSelector).Text;
        }
        public string GetSpecialtyByClaimGroup(int claimGroupRow = 2)
        {
            return
                SiteDriver.FindElement(
                     Format(AppealActionPageObjects.SpecialtyValueCssTemplate, claimGroupRow), How.CssSelector).Text;
        }
       

        /// <summary>
        /// CLick on appeal Processing Hx
        /// </summary>
        /// <returns></returns>
        public AppealProcessingHistoryPage ClickAppealProcessingHx()
        {
            var appealprocessingHx = Navigator.Navigate<AppealProcessingHistoryPageObjects>(() =>
            {
                JavaScriptExecutor.ExecuteClick(AppealActionPageObjects.AppealProcessingHxLinkXpath, How.XPath);
                 WriteLine("Clicked Appeal Proccessing Link");
                SiteDriver.WaitForCondition(
                    () => SiteDriver.SwitchWindowByTitle(PageTitleEnum.AppealProcessingHistory.GetStringValue()));
            });
            return new AppealProcessingHistoryPage(Navigator, appealprocessingHx, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        /// <summary>
        /// CLick on appeal  Hx
        /// </summary>
        /// <returns></returns>
        //public AppealHistoryPage ClickAppealHistory()
        //{
        //    var appealHx = Navigator.Navigate<AppealHistoryPageObjects>(() =>
        //    {
        //        JavaScriptExecutor.ExecuteClick(NewAppealActionPageObjects.AppealHxLinkXpath, How.XPath);
        //         WriteLine("Clicked Appeal Proccessing Link");
        //        SiteDriver.WaitForCondition(
        //            () => SiteDriver.SwitchWindowByTitle(PageTitleEnum.AppealHistory.GetStringValue()));
        //    });
        //    return new AppealHistoryPage(Navigator, appealHx);
        //}

        public void ClickMoreOption()
        {
            var element = SiteDriver.FindElement(AppealActionPageObjects.MoreOptionCssLocator, How.CssSelector);
            SiteDriver.ElementToBeClickable(element);
            SiteDriver.WaitToLoadNew(500);
            element.Click();
        }

        public List<int> GetClaimSequenceList()
        {
            return JavaScriptExecutor.FindElements(AppealActionPageObjects.ClaimSequenceValueListCssLocator, How.CssSelector, "text").Select(s => s.Substring(0, s.LastIndexOf('-'))).ToList().Select(int.Parse).ToList();
        }

        public string GetLineNo(int claimGroupRow=2,int claimRow=2)
        {
            return
                SiteDriver.FindElement(
                     Format(AppealActionPageObjects.LineNoCssTemplate, claimGroupRow, claimRow), How.CssSelector).Text;
        }
        public List<string> GetFlagList(int claimGroupRow=2,int claimRow=2)
        {
           var list =
               SiteDriver.FindElement(
                    Format(AppealActionPageObjects.FlagListValueCssTemplate, claimGroupRow, claimRow),
                   How.CssSelector).Text.Replace("\r\n", "").Replace(" ","");
             return
                new List<string>(list.Split(',')); 
        }
        public string GetAppealPay(int claimGroupRow = 2, int claimRow=2)
        {
            if (IsAppealResultTypeHiddenInAppealAction(claimGroupRow, claimRow))
                ClickOnAppealResultTypeEllipses(claimGroupRow, claimRow);
            return
                SiteDriver.FindElement(
                     Format(AppealActionPageObjects.PayIconCssTemplate, claimGroupRow, claimRow) + ".is_active", How.CssSelector).Text;
        }

        public string GetAppealDeny(int claimGroupRow = 2, int claimRow=2)
        {
            if (IsAppealResultTypeHiddenInAppealAction(claimGroupRow, claimRow))
                ClickOnAppealResultTypeEllipses(claimGroupRow, claimRow);
            return
                SiteDriver.FindElement(
                     Format(AppealActionPageObjects.DenyIconCssTemplate, claimGroupRow, claimRow) + ".is_active", How.CssSelector).Text;
        }

        public string GetAppealAdjust(int claimGroupRow = 2, int claimRow=2)
        {
            if (IsAppealResultTypeHiddenInAppealAction(claimGroupRow, claimRow))
                ClickOnAppealResultTypeEllipses(claimGroupRow, claimRow);
            return
                SiteDriver.FindElement(
                     Format(AppealActionPageObjects.AdjustIconCssTemplate, claimGroupRow, claimRow) + ".is_active", How.CssSelector).Text;
        }

        public string GetAppealNoDocs(int claimGroupRow = 2, int claimRow=2)
        {
            if (IsAppealResultTypeHiddenInAppealAction(claimGroupRow, claimRow))
                ClickOnAppealResultTypeEllipses(claimGroupRow, claimRow);
            return
                SiteDriver.FindElement(
                     Format(AppealActionPageObjects.NoDocsIconCssTemplate, claimGroupRow, claimRow) + ".is_active", How.CssSelector).Text;
        }

        public string GetDateOfService(int claimGroupRow = 2, int claimRow=2)
        {
            return
                SiteDriver.FindElement(
                     Format(AppealActionPageObjects.DateOfServiceCssTemplate, claimGroupRow, claimRow), How.CssSelector).Text;
        }

        public string GetRevCode(int claimGroupRow = 2, int claimRow=2)
        {
            return
                SiteDriver.FindElement(
                     Format(AppealActionPageObjects.RevCodeCssTemplate, claimGroupRow, claimRow), How.CssSelector).Text;
        }

        public NewPopupCodePage ClickOnRevenueCodeandSwitch(string title,int claimGroupRow = 2, int claimRow=2)
        {
            JavaScriptExecutor.ExecuteClick(
                 Format(AppealActionPageObjects.RevCodeCssTemplate, claimGroupRow, claimRow), How.CssSelector);
            return SwitchToPopupCodePage(title);
        }

        public string GetModifier(int claimGroupRow = 2, int claimRow=2)
        {
            return
                SiteDriver.FindElement(
                     Format(AppealActionPageObjects.ModifierCssTemplate, claimGroupRow, claimRow), How.CssSelector).Text;
        }

        public string GetProcCode(int claimGroupRow = 2, int claimRow=2)
        {
            return
                SiteDriver.FindElement(
                     Format(AppealActionPageObjects.ProcCodeCssTemplate, claimGroupRow, claimRow), How.CssSelector).Text;
        }

        public NewPopupCodePage ClickOnProcCodeandSwitch(string title, int claimGroupRow = 2, int claimRow=2)
        {
            JavaScriptExecutor.ExecuteClick(
                 Format(AppealActionPageObjects.ProcCodeCssTemplate, claimGroupRow, claimRow), How.CssSelector);
            return SwitchToPopupCodePage(title);
        }

        public string GetProcDescription(int claimGroupRow = 2, int claimRow=2)
        {
            return
                SiteDriver.FindElement(
                     Format(AppealActionPageObjects.ProcDescriptionCssTemplate, claimGroupRow, claimRow), How.CssSelector).Text;
        }

        public string GetProcDescriptionToolTip(int claimGroupRow = 2, int claimRow=2)
        {
            SiteDriver.MouseOver( Format(AppealActionPageObjects.ProcDescriptionCssTemplate, claimGroupRow, claimRow), How.CssSelector);
            
            return
                SiteDriver.FindElement(
                     Format(AppealActionPageObjects.ProcDescriptionCssTemplate, claimGroupRow, claimRow), How.CssSelector).GetAttribute("title");
        }

        public string GetUnits(int claimGroupRow = 2, int claimRow=2)
        {
            return
               SiteDriver.FindElement(
                     Format(AppealActionPageObjects.UnitsCssTemplate, claimGroupRow, claimRow), How.CssSelector).Text;
        }

        public string GetSource(int claimGroupRow = 2, int claimRow=2)
        {
            return
                SiteDriver.FindElement(
                     Format(AppealActionPageObjects.SourceCssTemplate, claimGroupRow, claimRow), How.CssSelector).Text;
        }

        public string GetSourceToolTip(int claimGroupRow = 2, int claimRow=2)
        {
            return
                SiteDriver.FindElement(
                     Format(AppealActionPageObjects.SourceCssTemplate, claimGroupRow, claimRow), How.CssSelector).GetAttribute("title");
        }

        public Double GetSugPaid(int claimGroupRow = 2, int claimRow=2)
        {
            return
                Double.Parse(SiteDriver.FindElement(
                     Format(AppealActionPageObjects.SugPaidCssTemplate, claimGroupRow, claimRow),
                    How.CssSelector).Text.Replace('$', ' ').Trim());
        }

        public void SetSugPaid(string value,int claimGroupRow = 2, int claimRow = 2)
        {
            var element = SiteDriver.FindElement(
                 Format(AppealActionPageObjects.SugPaidCssTemplate, claimGroupRow, claimRow),
                How.CssSelector);
            element.ClearElementField();
            element.SendKeys(value);
        }
        public string GetTrigCode(int claimGroupRow = 2, int claimRow=2)
        {
            return
                SiteDriver.FindElement(
                     Format(AppealActionPageObjects.TrigCodeCssTemplate, claimGroupRow, claimRow), How.CssSelector).Text;
        }

        public NewPopupCodePage ClickOnTrigCodeandSwitch(string title, int claimGroupRow = 2, int claimRow=2)
        {
            JavaScriptExecutor.ExecuteClick(
                 Format(AppealActionPageObjects.TrigCodeCssTemplate, claimGroupRow, claimRow), How.CssSelector);
            return SwitchToPopupCodePage(title);
        }

        public string GetTrigSpec(int claimGroupRow = 2, int claimRow=2)
        {
            return
                SiteDriver.FindElement(
                     Format(AppealActionPageObjects.TrigSpecCssTemplate, claimGroupRow, claimRow), How.CssSelector).Text;
        }

        public string GetTrigDOS(int claimGroupRow = 2, int claimRow=2)
        {
            return
                SiteDriver.FindElement(
                     Format(AppealActionPageObjects.TrigDOSCssTemplate, claimGroupRow, claimRow), How.CssSelector).Text;
        }

        public string GetAppealLevel(int claimGroupRow = 2, int claimRow=2)
        {
            return
                SiteDriver.FindElement(
                     Format(AppealActionPageObjects.AppealLevelCssTemplate, claimGroupRow, claimRow), How.CssSelector).Text;
        }

        public bool IsAppealLevelPresent(int claimGroupRow = 2, int claimRow = 2)
        {
            return
                SiteDriver.IsElementPresent(
                     Format(AppealActionPageObjects.AppealLevelCssTemplate, claimGroupRow, claimRow), How.CssSelector);
        }



        public NewPopupCodePage SwitchToPopupCodePage(string title)
        {
            var popupCode = Navigator.Navigate<NewPopupCodePageObjects>(() =>
            {
                SiteDriver.WaitForCondition(() => SiteDriver.SwitchWindowByTitle(title));
                NewPopupCodePage.AssignPageTitle(title);

                 WriteLine("Switch To " + title + " Page");
            });
            return new NewPopupCodePage(Navigator, popupCode, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public AppealActionPage ClosePopup(string popupName)
        {

            var appealAction = Navigator.Navigate<AppealActionPageObjects>(() =>
            {
                // SiteDriver.SwitchWindowByTitle(popupName);
                SiteDriver.CloseWindow();
                SiteDriver.SwitchWindowByTitle(PageTitleEnum.ClaimHistoryPopup.GetStringValue());
                StringFormatter.PrintMessage("Close popup and switch back to New Appeal Action Page.");
            });
            return new AppealActionPage(Navigator, appealAction, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }
        


        public string GetPageHeader()
        {
            return SiteDriver.FindElement(AppealActionPageObjects.PageHeaderCssLocator, How.CssSelector).Text;
        }

        public string GetRecordReview()
        {
            return
                SiteDriver.FindElement(AppealActionPageObjects.RecordReviewLabelCssLocator,
                    How.CssSelector).Text;
        }

        //public string GetNoteIconTitle()
        //{
        //    return SiteDriver.FindElement(AppealActionPageObjects.notesIconCssLocator, How.CssSelector).GetAttribute("title");
        //}

        public string GetDocumentIconTitle()
        {
            return SiteDriver.FindElement(AppealActionPageObjects.documentsIconCssLocator, How.CssSelector).GetAttribute("title");
        }

        public string GetApproveIconTitle()
        {
            return SiteDriver.FindElement(AppealActionPageObjects.approveIconCssLocator, How.CssSelector).GetAttribute("title");
        }

        public string GetLetterIconTitle()
        {
            return SiteDriver.FindElement(AppealActionPageObjects.LetterIconIconCssLocator, How.CssSelector).GetAttribute("title");
        }
        public string GetSaveDraftIconTitle()
        {
            return SiteDriver.FindElement(AppealActionPageObjects.savedraftIconCssLocator, How.CssSelector).GetAttribute("title");
        }

        public string GetSearchIconTitle()
        {
            return SiteDriver.FindElement(AppealActionPageObjects.SearchIconCssLocator, How.CssSelector).GetAttribute("title");
        }

        public string GetMoreOptionIconTitle()
        {
            return SiteDriver.FindElement(AppealActionPageObjects.MoreOptionCssLocator, How.CssSelector).GetAttribute("title");
        }

        public string GetAppealCategory()
        {
            return SiteDriver.FindElement( Format(AppealActionPageObjects.AppealRowValueCssTemplate,1, 2),
                How.CssSelector).Text;
        }

        public string GetAppealStatus()
        {
            return SiteDriver.FindElement(Format(AppealActionPageObjects.AppealRowValueCssTemplate, 2, 2),
                How.CssSelector).Text;
        }

        public string GetClientNotes() => SiteDriver
            .FindElement(Format(AppealActionPageObjects.AppealRowValueCssTemplate, 2, 4), How.CssSelector).Text;

        public string GetPrimaryReviewer()
        {
            return SiteDriver.FindElement( Format(AppealActionPageObjects.AppealRowValueCssTemplate,1, 3),
                How.CssSelector).Text;
        }

        public string GetAssignedTo()
        {
            return SiteDriver.FindElement( Format(AppealActionPageObjects.AppealRowValueCssTemplate,1, 4),
                How.CssSelector).Text;
        }

        public string GetDueDate()
        {
            return SiteDriver.FindElement( Format(AppealActionPageObjects.AppealRowValueCssTemplate, 1, 5),
                How.CssSelector).Text;
        }

        public string GetStatus()
        {
            return SiteDriver.FindElement( Format(AppealActionPageObjects.AppealRowValueCssTemplate,2, 2),
                How.CssSelector).Text;
        }

        public string GetExternalDocumentId()
        {
            return SiteDriver.FindElement( Format(AppealActionPageObjects.AppealRowValueCssTemplate,2, 3),
                How.CssSelector).Text;
        }

        public string GetAppealSequence()
        {
            return SiteDriver.FindElement(AppealActionPageObjects.AppealSeqCssLocator, How.CssSelector).Text;
        }

        public AppealSearchPage ClickOnSearchIconToNavigateAppealSearchPage()
        {
            JavaScriptExecutor.ExecuteClick(AppealActionPageObjects.SearchIconCssLocator, How.CssSelector);
            WaitForWorkingAjaxMessage(20000);
            if(IsPageErrorPopupModalPresent())ClickOkCancelOnConfirmationModal(true);
            if (GetPageHeader() == "Appeal Search" && !SiteDriver.IsElementPresent(AppealSearchPageObjects.FindAppealSectionCssLocator, How.CssSelector) && !SiteDriver.IsElementPresent(AppealSearchPageObjects.SearchIconInActiveXpath, How.CssSelector))
            {
                JavaScriptExecutor.ExecuteClick(AppealSearchPageObjects.SearchIconCssLocator, How.CssSelector);
                JavaScriptExecutor.ExecuteClick(AppealSearchPageObjects.SearchIconCssLocator, How.CssSelector);
            }
            SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(AppealSearchPageObjects.FindAppealSectionCssLocator, How.CssSelector));
            SiteDriver.WaitToLoadNew(300);
            if(IsWorkingAjaxMessagePresent())
                RefreshPage();
            return new AppealSearchPage(Navigator, new AppealSearchPageObjects(), SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);

        }

        /// <summary>
        /// Navigate to claim action page
        /// </summary>
        /// <returns>claim action page</returns>
        public ClaimActionPage ClickOnClaimSequenceAndSwitchWindow(int row=2)
        {
            var claimActionPopup = Navigator.Navigate<ClaimActionPageObjects>(() =>
            {
                var element = SiteDriver.FindElement(Format(AppealActionPageObjects.ClaimSequenceValueCssTemplate, row),
                    How.CssSelector);
                SiteDriver.ElementToBeClickable(element);
                SiteDriver.WaitToLoadNew(500);
                element.Click();
                WriteLine("Clicked on claim sequence, navigating to claim action");
                SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
                SiteDriver.SwitchWindow(SiteDriver.WindowHandles[1]);
                SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(ClaimActionPageObjects.ClaimActionPageTitleCss, How.CssSelector));
                SiteDriver.WaitForPageToLoad();
            });
            return new ClaimActionPage(Navigator, claimActionPopup, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public ClaimActionPage ClickOnClaimSequenceSwitchToClaimAction(int row=2)
        {
            var claimActionPopup = Navigator.Navigate<ClaimActionPageObjects>(() =>
            {
                var element = SiteDriver.FindElement(Format(AppealActionPageObjects.ClaimSequenceValueCssTemplate, row),
                    How.CssSelector);
                SiteDriver.ElementToBeClickable(element);
                SiteDriver.WaitToLoadNew(500);
                element.Click();
                WriteLine("Clicked on Claim Sequence: ");
                SiteDriver.WaitForCondition(() => SiteDriver.SwitchWindowByUrl("claim_lines"));
                SiteDriver.WaitForCondition(
                    () => SiteDriver.IsElementPresent(DefaultPageObjects.PageHeaderCssLocator, How.CssSelector));
            });
            return new ClaimActionPage(Navigator, claimActionPopup, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        
        public void CloseAnyPopupIfExist()
        {
            if (SiteDriver.WindowHandles.Count > 1)
            {
                for (var i=1 ;i < SiteDriver.WindowHandles.Count;i++)
                {
                    if (!_originalWindow.Equals(SiteDriver.CurrentWindowHandle))
                    {
                        SiteDriver.CloseWindow();
                        SiteDriver.SwitchWindowByTitle(PageTitleEnum.AppealAction.GetStringValue());
                    }
                }
            }
        }

        public void CloseAllExistingPopupIfExist()
        {
            while (SiteDriver.WindowHandles.Count != 1)
            {
                SiteDriver.SwitchWindow(SiteDriver.WindowHandles[1]);
                if (!_originalWindow.Equals(SiteDriver.CurrentWindowHandle))
                {
                    SiteDriver.CloseWindow();
                    SiteDriver.SwitchWindowByTitle(PageTitleEnum.AppealAction.GetStringValue());
                }
            }
        }
        public bool IsAppealDocumentIconPresent()
        {
            return
                SiteDriver.IsElementPresent(AppealActionPageObjects.AppealDocumentIconXpath, How.XPath);
        }
        public bool IsAppealDocumentSelected()
        {
            return
                SiteDriver.FindElementAndGetClassAttribute(AppealActionPageObjects.AppealDocumentIconXpath, How.XPath)
                    .Any(str => str.Contains("is_selected")); //.Contains("is_selected");
        }

        public bool IsAppealDocumentSectionPresent()
        {
            return
                SiteDriver.IsElementPresent(AppealActionPageObjects.AppealDocuemntSectionXpath, How.XPath);
        }

        public bool IsAppealLetterSectionPresent()
        {
            return
                SiteDriver.IsElementPresent(AppealActionPageObjects.AppealLetterSectionXpath, How.XPath);
        }

        public void ClickOnAppealDocsIcon(bool open)
        {
            JavaScriptExecutor.ExecuteClick(AppealActionPageObjects.AppealDocumentIconXpath, How.XPath);
            if (open) SiteDriver.WaitForCondition(IsAppealDocumentSectionPresent);
        }

        public int AppealDocumentListCount()
        {
            return SiteDriver.FindElementsCount(AppealActionPageObjects.AppealDocumentListCssLocator, How.CssSelector);
        }

        public List<string> AppealDocumentListByDate()
        {
            var list = JavaScriptExecutor.FindElements(AppealActionPageObjects.AppealDocumentListByDateCssLocator, How.CssSelector, "Text");
            //for (var i = 0; i < list.Count; i++)
            //{
            //    list[i] = Regex.Replace(list[i], @"\r\n?|\n", "");
            //}
            list.RemoveAll( IsNullOrEmpty);
            return list;
        }
        public bool IsListSortedInDescendingOrder( )
        {
            return AppealDocumentListByDate().Select(DateTime.Parse).ToList().IsInDescendingOrder();
        }

        public string GetAppealDocInfo(int row,int innerRow ,int col)
        {
            return SiteDriver.FindElement( Format(AppealActionPageObjects.AppealDocumentInfoCssLocator, row, innerRow,col),
                How.CssSelector).Text;
        }

        public string GetAppealDocInfoToolTip(int row, int innerRow, int col)
        {
            return SiteDriver.FindElement( Format(AppealActionPageObjects.AppealDocumentInfoCssLocator, row, innerRow,col),
                How.CssSelector).GetAttribute("title");
        }
        public void ClickOnDocumentToViewAndStayOnPage(int docrow)
        {
            var element =
                SiteDriver.FindElement(Format(AppealActionPageObjects.AppealDocumentInfoCssLocator, docrow, 1, 1),
                    How.CssSelector);
            SiteDriver.ElementToBeClickable(element);
            SiteDriver.WaitToLoadNew(500);
            element.Click();
            SiteDriver.SwitchWindow(SiteDriver.WindowHandles[SiteDriver.WindowHandles.Count-1]);
        }

        public string GetOpenedDocumentText()
        {
            return SiteDriver.FindElement("body>pre", How.CssSelector).Text;
        }

        /// <summary>
        /// Close note popup page and back to appeal summary page
        /// </summary>
        public AppealActionPage CloseDocumentTabPageAndBackToAppealAction()
        {
            var newAppealAction= Navigator.Navigate<AppealActionPageObjects>(() =>
            {
                SiteDriver.CloseWindow();
                SiteDriver.SwitchWindowByTitle(PageTitleEnum.AppealAction.GetStringValue());
            });
            return new AppealActionPage(Navigator, newAppealAction,SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor,false);
        }

        public AppealSummaryPage SwitchToAppealSummary()
        {
            return new AppealSummaryPage(Navigator, new AppealSummaryPageObjects(), SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public AppealSearchPage SwitchToAppealSearch()
        {
            return new AppealSearchPage(Navigator, new AppealSearchPageObjects(), SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public void CompleteAppeals(string appealSeq=null, string reasonCode=null, string rationale=null, bool handlePopUp=false,bool isCompleted=false,bool isAction=true)
        {
            if(isAction)     
                ClickOnDenyIcon();
            if (handlePopUp)
            {
               if (IsPageErrorPopupModalPresent())
                 ClickOkCancelOnConfirmationModal(true);
            }
            if (!isCompleted) { 
                SetReasonCodeList(reasonCode);
                SetEditAppealLineIfrmeEditorByHeader("Rationale", rationale);
            }
            ClickOnAppealLetter();
            ClickOnApproveIcon();
            ClickOkCancelOnConfirmationModal(true);
            WaitForWorking();
        }

        public void CloseCurrentWindowAndSwitchToOriginal()
        {
            var handles = SiteDriver.WindowHandles.ToList();
            SiteDriver.CloseWindowAndSwitchTo(handles[0]);
        }

        

        public void DeleteAppealLineByLinNo(string claseq, string clasub, string linNo)
        {
            var query =  Format(AppealSqlScriptObjects.DeleteAppealLineByLinNo, claseq, clasub, linNo);
            Executor.ExecuteQueryAsync(query);
          
        }

        public List<string> GetListOfHolidays(DateTime start,DateTime end)
        {
            return Executor.GetTableSingleColumn(Format(AppealSqlScriptObjects.GetHolidays,start.ToString("dd-MMM-yyyy"),end.ToString("dd-MMM-yyyy")));
        }


        public bool IsPayIconSelected(int col = 2, int row = 2)
        {
            return SiteDriver.IsElementPresent(
                 Format(AppealActionPageObjects.PayIconCssTemplate, col, row + ".is_enabled"),
                How.CssSelector);
        }

        public bool IsDenyIconSelected(int col = 2, int row = 2)
        {
            return SiteDriver.IsElementPresent(
                 Format(AppealActionPageObjects.DenyIconCssTemplate, col, row) + ".is_enabled",
                How.CssSelector);
        }

        public bool IsAllPayIconSelected(int skipLine = 2)
        {
            if (skipLine == 1)
                return SiteDriver.FindElementsAndGetAttributeByTwoClass("badge", "is_enabled",
                    AppealActionPageObjects.AllPayIconCssTemplate,
                    How.CssSelector).Skip(skipLine).All(x => x.Equals(true));
            return SiteDriver.FindElementsAndGetAttributeByTwoClass("badge", "is_enabled",
                AppealActionPageObjects.AllPayIconCssTemplate,
                How.CssSelector).All(x => x.Equals(true));

        }

        public bool IsAllDenyIconSelected(int skipLine=2)
        {
            if(skipLine==1)
                return SiteDriver.FindElementsAndGetAttributeByTwoClass("badge", "is_enabled",
                AppealActionPageObjects.AllDenyIconCssTemplate,
                How.CssSelector).Skip(skipLine).All(x => x.Equals(true));
            return SiteDriver.FindElementsAndGetAttributeByTwoClass("badge", "is_enabled",
                AppealActionPageObjects.AllDenyIconCssTemplate,
                How.CssSelector).All(x => x.Equals(true));
        }

        public bool IsAdjustIconSelected(int col = 2, int row = 2)
        {
            return SiteDriver.IsElementPresent(
                 Format(AppealActionPageObjects.AdjustIconCssTemplate, col, row) + ".is_enabled",
                How.CssSelector);
        }

        public void MaximizeWindow()
        {
            SiteDriver.WebDriver.Manage().Window.Maximize();
        }
        public void DeleteAppealAuditHistory(int apseq1 = 493889, int apseq2 = 494775)
        {
            var query =  Format(AppealSqlScriptObjects.DeleteAppealAuditHistory, apseq1, apseq2);
            Executor.ExecuteQueryAsync(query);
        }

        public void ClickOnCopyAllButton(int section = 2)
        {
            var element = SiteDriver.FindElement(Format(AppealActionPageObjects.CopyAllButtonXpath, section),
                How.XPath);
            SiteDriver.ElementToBeClickable(element);
            SiteDriver.WaitToLoadNew(500);
            element.Click();
        }


        public int GetAppealLineFormCount()
        {
            return SiteDriver.FindElementsCount(AppealActionPageObjects.AppealLineFormCssLocator, How.CssSelector);
        }

        public bool IsCopyAllButtonPresent(int section=2)
        {
            return SiteDriver.IsElementPresent( Format(AppealActionPageObjects.CopyAllButtonXpath, section),How.XPath);
        }
        public void ClickOnSaveAppealDraftButton()
        {
            var element = SiteDriver.FindElement(AppealActionPageObjects.savedraftIconCssLocator, How.CssSelector);
            SiteDriver.ElementToBeClickable(element);
            SiteDriver.WaitToLoadNew(500);
            element.Click();
            WriteLine("Clicked on Save Appeal Draft button");
            WaitForWorking();

        }
        public List<string> GetStatusAndTypeFromRealTimeQueue(string claseq)
        {
            var list = Executor.GetCompleteTable(Format(AppealSqlScriptObjects.GetAppeaStatusFromRealTimeQueue,
                claseq));

            return list.Select(row => row.ItemArray.Select(x => x.ToString()).ToList()).ToList()[0];

        }


        public bool IsAppealLineFormOpened(int section = 2)
        {
            return SiteDriver.IsElementPresent(Format(AppealActionPageObjects.AppealLineFormByLineCssLocator,section),How.CssSelector);
        }

        public string GetAppealLevelFromDb(string claseq, ClientEnum client)
        {
            var claimSeq = claseq.Split('-');
            return Executor.GetSingleStringValue(
                string.Format(AppealSqlScriptObjects.GetAppealLevel, claimSeq[0], claimSeq[1], client));


        }

        public bool IsAppealActionHeaderForMRRAppealTypePresent() =>
            JavaScriptExecutor.IsElementPresent(AppealActionPageObjects.AppealActionHeaderForMRRAppealTypeCssSelector);

        #endregion

        #region PRIVATE METHODS



        #endregion

        #region Appeal Help Section
        public bool IsAppealHelpSectionPresentToTheRight() =>
            SiteDriver.IsElementPresent(AppealActionPageObjects.AppealHelpSectionXPath,How.XPath);

        public bool IsAppealHelpfulHintsTextPresentInAppealHelpSection() =>
            SiteDriver.IsElementPresent(AppealActionPageObjects.AppealHelpfulHintsTextXPath,How.XPath);

        public bool IsAppealRationaleDocumentSectionPresentInAppealHelpSection() =>
            SiteDriver.IsElementPresent(AppealActionPageObjects.AppealRationaleDocumentXPath,How.XPath);

        public string GetAppealHelpfulHintsTextInAppealHelpSection(bool noData = false)
        {
            if (noData)
                return SiteDriver.FindElement(AppealActionPageObjects.GetAppealHelpfulHintsTextNoDataXPath, How.XPath).Text;
            SiteDriver.SwitchFrameByXPath(Format(AppealActionPageObjects.GetAppealHelpfulHintsTextXPath));
            var text = SiteDriver.FindElement("body", How.CssSelector).Text;
            SiteDriver.SwitchBackToMainFrame();
            return text;
        }

        public string GetAppealRationaleDocumentNameInAppealHelpSection(bool noData = false)
        {
            if(noData)
                return SiteDriver.FindElement(AppealActionPageObjects.AppealRationaleDocumentFileNameNoDataXPath, How.XPath).Text;
            return SiteDriver.FindElement(AppealActionPageObjects.AppealRationaleDocumentFileNameXPath, How.XPath).Text;
        }
        
        public void ClickOnDocumentNameToOpenInNewTab() =>
            SiteDriver.FindElement(AppealActionPageObjects.AppealRationaleDocumentFileNameXPath, How.XPath).Click();
        
        #endregion


    }
}


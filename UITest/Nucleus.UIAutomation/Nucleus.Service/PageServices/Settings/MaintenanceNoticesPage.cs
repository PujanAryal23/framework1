using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using Nucleus.Service.PageObjects.Appeal;
using Nucleus.Service.PageObjects.Claim;
using Nucleus.Service.PageObjects.Login;
using Nucleus.Service.PageObjects.Settings;
using Nucleus.Service.PageServices.Appeal;
using Nucleus.Service.PageServices.Base.Default;
using Nucleus.Service.PageServices.Login;
using Nucleus.Service.Support.Common;
using Nucleus.Service.Support.Environment;
using OpenQA.Selenium;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Navigation;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Elements;
using Nucleus.Service.Support.Menu;
using UIAutomation.Framework.Database;
using UIAutomation.Framework.Utils;

namespace Nucleus.Service.PageServices.Settings
{
    public class MaintenanceNoticesPage : NewDefaultPage
    {
        #region PRIVATE FIELDS

        private MaintenanceNoticesPageObjects _maintenanceNoticesPage;

        #endregion

        #region PUBLIC PROPERTIES

        public MaintenanceNoticesPageObjects MaintenanceNotices
        {
            get { return _maintenanceNoticesPage; }
        }

        #endregion

        #region CONSTRUCTOR

        public MaintenanceNoticesPage(INewNavigator navigator, MaintenanceNoticesPageObjects maintenanceNoticesPage, ISiteDriver siteDriver, IJavaScriptExecutors javaScriptExecutors, IEnvironmentManager environmentManager, IBrowserOptions browserOptions, IOracleStatementExecutor executor)
            : base(navigator, maintenanceNoticesPage, siteDriver, javaScriptExecutors, environmentManager, browserOptions, executor
            )
        {
            _maintenanceNoticesPage = (MaintenanceNoticesPageObjects)PageObject;
        }

        #endregion

        #region calender

        public void SetDate(string date)
        {
            var splitDate = date.Split('/');

            var month = "";

            switch (splitDate[0])
            {
                case "01":
                    month = "January";
                    break;

                case "02":
                    month = "February";
                    break;

                case "03":
                    month = "March";
                    break;

                case "04":
                    month = "April";
                    break;

                case "05":
                    month = "May";
                    break;

                case "06":
                    month = "June";
                    break;

                case "07":
                    month = "July";
                    break;

                case "08":
                    month = "August";
                    break;

                case "09":
                    month = "September";
                    break;

                case "10":
                    month = "October";
                    break;

                case "11":
                    month = "November";
                    break;

                case "12":
                    month = "December";
                    break;
            }



            SiteDriver.FindElement(MaintenanceNoticesPageObjects.YearCssLocator, How.CssSelector).Click();
            SiteDriver.FindElement(string.Format(MaintenanceNoticesPageObjects.YearValueXPathTemplate, splitDate[2]), How.XPath).Click();
            SiteDriver.FindElement(MaintenanceNoticesPageObjects.MonthCssLocator, How.CssSelector).Click();
            SiteDriver.FindElement(string.Format(MaintenanceNoticesPageObjects.MonthValueXPathTemplate, month), How.XPath).Click();
            SiteDriver.FindElement(string.Format(MaintenanceNoticesPageObjects.DayValueXPathTemplate, splitDate[1]), How.XPath).Click();


        }

        public void SetTime(string time)
        {
            var splitTime = time.Split(':');
            SiteDriver.FindElement(MaintenanceNoticesPageObjects.MinuteCssLocator, How.CssSelector).Click();
            SiteDriver.FindElement(string.Format(MaintenanceNoticesPageObjects.MinutesValueXPathTemplate, splitTime[1]), How.XPath).Click();
            SiteDriver.FindElement(MaintenanceNoticesPageObjects.HourCssLocator, How.CssSelector).Click();
            SiteDriver.FindElement(string.Format(MaintenanceNoticesPageObjects.HourValueXPathTemplate, splitTime[0]), How.XPath).Click();


        }

        #endregion calender

        public bool IsAddNewCategoryCodeSectionDisplayed()
        {
            if (SiteDriver.IsElementPresent(MaintenanceNoticesPageObjects.ScheduleMaintenanceNoticeSectionXPath,
                How.XPath))
                return SiteDriver.FindElement(MaintenanceNoticesPageObjects.ScheduleMaintenanceNoticeSectionXPath,
                    How.XPath).Displayed;
            return false;
        }

        public string GetPreviewMessage()
        {
            return
                SiteDriver.FindElement(MaintenanceNoticesPageObjects.PreviewMessageCssLocator, How.CssSelector)
                    .Text;
        }

        public string GetPreviewMessageDateTime()
        {

            JavaScriptExecutor.ExecuteClick("body", How.CssSelector);

            return
                SiteDriver.FindElement(MaintenanceNoticesPageObjects.PreviewMessageDateTimeCssLocator, How.CssSelector)
                    .Text;

        }


        public bool IsEditIconPresent()
        {
            return SiteDriver.IsElementPresent(MaintenanceNoticesPageObjects.EditIconCssTemplate, How.CssSelector);
        }
        public bool IsEditIconEnabled(int row = 1)
        {
            return SiteDriver.IsElementPresent(string.Format(MaintenanceNoticesPageObjects.EditIconEnabledCssTemplate, row), How.CssSelector);
        }

        public bool IsEditIconDisabled(int row = 1)
        {
            return SiteDriver.IsElementPresent(string.Format(MaintenanceNoticesPageObjects.EditIconDisabledCssTemplate, row), How.CssSelector);
        }

        public bool IsDeleteIconPresent(int row = 1)
        {
            return SiteDriver.IsElementPresent(string.Format(MaintenanceNoticesPageObjects.DeleteIconCssTemplate, row), How.CssSelector);
        }
        public bool IsDeleteIconEnabled(int row = 1)
        {
            return SiteDriver.IsElementPresent(string.Format(MaintenanceNoticesPageObjects.DeleteIconEnabledCssTemplate, row), How.CssSelector);
        }

        public bool IsDeleteIconDisabled(int row = 1)
        {
            return SiteDriver.IsElementPresent(MaintenanceNoticesPageObjects.DeleteIconDisabledCssLocator, How.CssSelector);
        }
        public string GetDisplayLabel(int row = 1)
        {
            return SiteDriver.FindElement(String.Format(MaintenanceNoticesPageObjects.DisplayLabelCsssLocator, row), How.CssSelector).Text;
        }

        public string GetDisplayValue(int row = 1)
        {
            return SiteDriver.FindElement(String.Format(MaintenanceNoticesPageObjects.DisplayValueCssTemplate, row), How.CssSelector).Text;
        }

        public string GetMaintenanceLabel(int row = 1)
        {
            return SiteDriver.FindElement(String.Format(MaintenanceNoticesPageObjects.MaintenanceLabelCssLocator, row), How.CssSelector).Text;
        }

        public string GetMaintenanceValue(int row = 1)
        {
            return SiteDriver.FindElement(String.Format(MaintenanceNoticesPageObjects.MaintenanceValueCssTemplate, row), How.CssSelector).Text;
        }

        public string GetLastModifiedUserLabel(int row = 1)
        {
            return SiteDriver.FindElement(String.Format(MaintenanceNoticesPageObjects.LastModifiedUserLabelCssLocator, row), How.CssSelector).Text;
        }

        public string GetLastModifiedUserValue(int row = 1)
        {
            return SiteDriver.FindElement(String.Format(MaintenanceNoticesPageObjects.LastModifiedUserValueCssTemplate, row), How.CssSelector).Text;
        }

        public string GetLastModifiedDateLabel(int row = 1)
        {
            return SiteDriver.FindElement(String.Format(MaintenanceNoticesPageObjects.LastModifiedDateLabelCssLocator, row), How.CssSelector).Text;
        }

        public string GetLastModifiedDateValue(int row = 1)
        {
            return SiteDriver.FindElement(String.Format(MaintenanceNoticesPageObjects.LastModifiedDateValueCssTemplate, row), How.CssSelector).Text;
        }

        public bool IsMaintenanceNoticesSubMenuPresent()
        {
            return SiteDriver.IsElementPresent(GetSubMenu.GetSubMenuLocator(SubMenu.MaintenanceNotices), How.XPath);
        }

        public string GetPageInsideTitle()
        {
            return
                SiteDriver.FindElement(MaintenanceNoticesPageObjects.PageInsideTitleCssLocator,
                    How.CssSelector).Text;
        }

        public bool IsListOfNotificationPresent()
        {
            return SiteDriver.IsElementPresent(MaintenanceNoticesPageObjects.GridRowCssLocator, How.CssSelector);
        }
        public bool IsPanelOpen()
        {
            return SiteDriver.IsElementPresent(MaintenanceNoticesPageObjects.RightsidePanelCssTempalte, How.CssSelector);
        }



        /// <summary>
        /// Get result grid row count
        /// </summary>
        /// <returns></returns>
        public int GetResultGridRowCount()
        {
            return SiteDriver.FindElementsCount(MaintenanceNoticesPageObjects.GridRowCssLocator, How.CssSelector);
        }
        public string GetDisplayValueForGivenRow(int rowId)
        {
            return SiteDriver.FindElement(String.Format(MaintenanceNoticesPageObjects.DisplayValueCssTemplate, rowId), How.CssSelector).Text;
        }
        public string GetStatusValueForGivenRow(int rowId)
        {
            return SiteDriver.FindElement(String.Format(MaintenanceNoticesPageObjects.NotificationStatusCssLocator, rowId), How.CssSelector).Text;
        }

        public void ClickOnNotificationRow(int row)
        {
            JavaScriptExecutor.ExecuteClick(string.Format(MaintenanceNoticesPageObjects.NotificationRowCssTemplate, row), How.CssSelector);
            SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());

            Console.WriteLine("Click on Notification Row {0}", row);
        }
        public bool IsWorkingAjaxMessagePresent()
        {
            return SiteDriver.IsElementPresent(MaintenanceNoticesPageObjects.WorkingAjaxMessageCssLocator, How.CssSelector);
        }

        public string GetNotificationDateRangeLabelForRightPanel()
        {
            return SiteDriver.FindElement(MaintenanceNoticesPageObjects.NotificationDateRangePanelCssTemplate, How.CssSelector).Text;
        }

        public string GetNotificationDateRangeValueForRightPanel()
        {
            return SiteDriver.FindElement(MaintenanceNoticesPageObjects.NotificationDateRangePanelValueCssTemplate, How.CssSelector).Text;
        }

        public string GetMaintenanceDateRangeLabelForRightPanel()
        {
            return SiteDriver.FindElement(MaintenanceNoticesPageObjects.MaintenanceDateRangePanelCssTemplate, How.CssSelector).Text;
        }

        public string GetMaintenanceDateRangeValueForRightPanel()
        {
            return SiteDriver.FindElement(MaintenanceNoticesPageObjects.MaintenanceDateRangePanelValueCssTemplate, How.CssSelector).Text;
        }

        public string GetLastModifiedUserLabelForRightPanel()
        {
            return SiteDriver.FindElement(MaintenanceNoticesPageObjects.LastModifiedUserPanelCssTemplate, How.CssSelector).Text;
        }

        public string GetLastModifiedUserValueForRightPanel()
        {
            return SiteDriver.FindElement(MaintenanceNoticesPageObjects.LastModifiedUserPanelValueCssTemplate, How.CssSelector).Text;
        }

        public string GetLastModifiedDateLabelForRightPanel()
        {
            return SiteDriver.FindElement(MaintenanceNoticesPageObjects.LastModifiedDatePanelCssTemplate, How.CssSelector).Text;
        }

        public string GetLastModifiedDateValueForRightPanel()
        {
            return SiteDriver.FindElement(MaintenanceNoticesPageObjects.LastModifiedDatePanelValueCssTemplate, How.CssSelector).Text;
        }
        public string GetNotificationStatusLabelForRightPanel()
        {
            return SiteDriver.FindElement(MaintenanceNoticesPageObjects.NotificationStatusPanelCssTemplate, How.CssSelector).Text;
        }

        public string GetNotificationStatusValueForRightPanel()
        {
            return SiteDriver.FindElement(MaintenanceNoticesPageObjects.NotificationStatusPanelValueCssTemplate, How.CssSelector).Text;
        }

        public string GetNotificationMessageLabelForRightPanel()
        {
            return SiteDriver.FindElement(MaintenanceNoticesPageObjects.NotificationMessagePanelCssTemplate, How.CssSelector).Text;
        }
        public bool IsNotificationMessagePresentInRightPanel()
        {
            return SiteDriver.IsElementPresent(MaintenanceNoticesPageObjects.NotificationMessagePanelValueCssTemplate, How.CssSelector);
        }

        public void ClickOnDeleteIcon(int row)
        {
            JavaScriptExecutor.ExecuteClick(string.Format(MaintenanceNoticesPageObjects.DeleteIconEnabledCssTemplate, row), How.CssSelector);
            Console.WriteLine("Delete Icon Clicked on Row<{0}>", row);
        }



        public string GetConfirmationMessage()
        {
            return
                SiteDriver.FindElement(
                    MaintenanceNoticesPageObjects.PopupModalContentDivId, How.Id).Text;
        }

        public void ClickOnCancelOnConfirmationModal()
        {
            SiteDriver.FindElement(MaintenanceNoticesPageObjects.CancelConfirmationCssSelector,How.CssSelector).Click();
            Console.WriteLine("Cancel Button is Clicked");
        }

        public void ClickOnOkButtonOnConfirmationModal()
        {
            SiteDriver.FindElement(MaintenanceNoticesPageObjects.OkConfirmationCssSelector, How.CssSelector).Click();
            Console.WriteLine("Ok Button is Clicked");
        }

        public void ClickOnAddNotice()
        {
            JavaScriptExecutor.ExecuteClick((MaintenanceNoticesPageObjects.AddNoticeCssSelector), How.CssSelector);
        }

        public void SetNote(String note)
        {
            SiteDriver.SwitchFrameByClass("cke_wysiwyg_frame cke_reset");
            var element = SiteDriver.FindElement("body", How.CssSelector);
            SiteDriver.ElementToBeClickable(element);
            SiteDriver.WaitToLoadNew(500);
            element.Click();
            element.ClearElementField();
            element.SendKeys(Keys.Escape);
            element.SendKeys(note);
            Console.WriteLine("Note set to {0}", note);
            SiteDriver.SwitchBackToMainFrame();


        }

        public string GetNote()
        {
            SiteDriver.SwitchFrameByClass("cke_wysiwyg_frame cke_reset");
            var note = SiteDriver.FindElement("body", How.CssSelector).Text;
            Console.WriteLine("Note set to {0}", note);
            SiteDriver.SwitchBackToMainFrame();
            return note;
        }

        public void ClickOnDisplayDateRangeFrom()
        {
            JavaScriptExecutor.ExecuteClick(MaintenanceNoticesPageObjects.DisplayDateRangeFromCssSelector, How.CssSelector);
        }

        public void ClickOnDisplayDateRangeTo()
        {
            JavaScriptExecutor.ExecuteClick(MaintenanceNoticesPageObjects.DisplayDateRangeToCssSelector, How.CssSelector);
        }

        public void ClickOnMaintenanceDateRangeFrom()
        {
            JavaScriptExecutor.ExecuteClick(MaintenanceNoticesPageObjects.MaintenanceDateRangeFromCssSelector, How.CssSelector);
        }

        public void ClickOnMaintenanceDateRangeTo()
        {

            JavaScriptExecutor.ExecuteClick(MaintenanceNoticesPageObjects.MaintenanceDateRangeToCssSelector, How.CssSelector);
        }

        public void SetDisplayDateFrom(string date)
        {
            var element = SiteDriver.FindElement(MaintenanceNoticesPageObjects.DisplayDateRangeFromCssSelector,
                How.CssSelector);
            element.ClearElementField();
            SiteDriver.WaitTillClear(element);
            SiteDriver.WaitToLoadNew(200);

            element.SendKeys(date);
        }

        public string GetDisplayDateFrom()
        {
            return SiteDriver.FindElement(MaintenanceNoticesPageObjects.DisplayDateRangeFromCssSelector, How.CssSelector).GetAttribute("value");

        }

        public void SetDisplayDateTo(string date)
        {
            var element = SiteDriver.FindElement(MaintenanceNoticesPageObjects.DisplayDateRangeToCssSelector,
                How.CssSelector);

            element.ClearElementField();
            SiteDriver.WaitTillClear(element);
            SiteDriver.WaitToLoadNew(200);

            element.SendKeys(date);
        }

        public string GetDisplayDateTo()
        {
            return SiteDriver.FindElement(MaintenanceNoticesPageObjects.DisplayDateRangeToCssSelector,
                 How.CssSelector).GetAttribute("value");
        }

        public void SetMaintenanceDateRangeFrom(string date)
        {
            var element = SiteDriver.FindElement(MaintenanceNoticesPageObjects.MaintenanceDateRangeFromCssSelector,
                How.CssSelector);
            element.ClearElementField();
            SiteDriver.WaitTillClear(element);
            SiteDriver.WaitToLoadNew(200);

           element.SendKeys(date);
        }

        public string GetMaintenanceDateRangeFrom()
        {
            return SiteDriver.FindElement(MaintenanceNoticesPageObjects.MaintenanceDateRangeFromCssSelector,
                 How.CssSelector).GetAttribute("value");
        }

        public void SetMaintenanceDateRangeTo(string date)
        {

            var element = SiteDriver.FindElement(MaintenanceNoticesPageObjects.MaintenanceDateRangeToCssSelector,
                How.CssSelector);
            element.ClearElementField();
            SiteDriver.WaitTillClear(element);
            SiteDriver.WaitToLoadNew(200);
            element.SendKeys(date);

        }

        public string GetMaintenanceDateRangeTo()
        {
            return SiteDriver.FindElement(MaintenanceNoticesPageObjects.MaintenanceDateRangeToCssSelector,
                 How.CssSelector).GetAttribute("value");
        }

        public void ClickOnScheduleNotice()
        {
            JavaScriptExecutor.ExecuteClick("body", How.CssSelector);
            JavaScriptExecutor.ExecuteClick(MaintenanceNoticesPageObjects.ScheduleButtonCssSelector, How.CssSelector);

        }

        public void ClickOnCancelScheduleNotice()
        {
            JavaScriptExecutor.ExecuteClick(MaintenanceNoticesPageObjects.CancelScheduleCssSelector, How.CssSelector);
        }

        public void WaitForWorking()
        {
            SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
            SiteDriver.WaitForCondition(IsListOfNotificationPresent);
            SiteDriver.WaitForPageToLoad();

        }
        public string GetNotificationMessageLValueInRightPanel()
        {
            return SiteDriver.FindElement(MaintenanceNoticesPageObjects.NotificationMessagePanelValueCssTemplate, How.CssSelector).Text;
        }


        /// <summary>
        /// Get count of active status
        /// </summary>
        /// <returns></returns>
        public int GetActiveNotificationCount()
        {
            return SiteDriver.FindElementsCount(MaintenanceNoticesPageObjects.ActiveNotificationCountCssLocator, How.CssSelector);
        }

        public bool IsMaintenaceNoticePresent()
        {
            return SiteDriver.IsElementPresent(MaintenanceNoticesPageObjects.MaintenancenoticeContainerCssLocator, How.CssSelector);
        }

        public string GetMaintenaceNoticeFromLoginPage()
        {
            return
                SiteDriver.FindElement(MaintenanceNoticesPageObjects.MaintenancenoticeContainerCssLocator,
                    How.CssSelector).Text;
        }
        public string GetMaintenaceNoticeDescriptionFromLoginPage()
        {
            return
                 SiteDriver.FindElement(
                     MaintenanceNoticesPageObjects.MaintenanceContainerNoticeDescriptionCssLocator,
                     How.CssSelector).Text;


        }

        public string GetMaintenaceNoticeTimeFrameFromLoginPage()
        {
            return
                SiteDriver.FindElement(MaintenanceNoticesPageObjects.MaintenanceContainerTimeFrameCssLocator,
                    How.CssSelector).Text;
        }

        public string GetAddScheduleHederLabel()
        {
            return
                SiteDriver.FindElement(MaintenanceNoticesPageObjects.AddScheduleHeaderLabelCssSelector,
                    How.CssSelector).Text;
        }

        public bool IsPopupModalPresent()
        {
            return SiteDriver.IsElementPresent(MaintenanceNoticesPageObjects.PopupModalCloseId, How.Id);
        }

        public void ClickOnClosePopupModal()
        {
            var element = SiteDriver.FindElement(MaintenanceNoticesPageObjects.PopupModalCloseId, How.Id);
            SiteDriver.ElementToBeClickable(element);
            SiteDriver.WaitToLoadNew(500);
            element.Click();
            Console.WriteLine("Closed the modal popup");

        }

        public string GetPopupModalMessage()
        {
            return SiteDriver.FindElement(MaintenanceNoticesPageObjects.PopupModalContentDivId, How.Id).Text;
        }

        public bool IsInvalidInputPresentForDisplayDateRangeLabel()
        {
            bool firstCondition =
                SiteDriver.IsElementPresent(MaintenanceNoticesPageObjects.DisplayDateRangeFromCssSelector + ".invalid", How.CssSelector)
                &&
                SiteDriver.IsElementPresent(MaintenanceNoticesPageObjects.DisplayDateRangeToCssSelector + ".invalid",
                  How.CssSelector);

            var secondCondition = false;

            if (firstCondition)
                secondCondition =
                SiteDriver.FindElement(MaintenanceNoticesPageObjects.DisplayDateRangeFromCssSelector + ".invalid", How.CssSelector)
                    .GetCssValue("box-shadow").Contains("rgb(255, 0, 0)")
                &&
                SiteDriver.FindElement(MaintenanceNoticesPageObjects.DisplayDateRangeToCssSelector + ".invalid", How.CssSelector)
                      .GetCssValue("box-shadow").Contains("rgb(255, 0, 0)");

            return firstCondition && secondCondition;
        }

        public bool IsInvalidInputPresentForMaintenanceDateLabel()
        {
            bool firstCondition =
                SiteDriver.IsElementPresent(MaintenanceNoticesPageObjects.MaintenanceDateRangeFromCssSelector + ".invalid", How.CssSelector)
                &&
                SiteDriver.IsElementPresent(MaintenanceNoticesPageObjects.MaintenanceDateRangeToCssSelector + ".invalid", How.CssSelector);

            var secondCondition = false;

            if (firstCondition)
                secondCondition =
                    SiteDriver.FindElement(MaintenanceNoticesPageObjects.MaintenanceDateRangeFromCssSelector + ".invalid", How.CssSelector)
                        .GetCssValue("box-shadow").Contains("rgb(255, 0, 0)")
                    &&
                    SiteDriver.FindElement(MaintenanceNoticesPageObjects.MaintenanceDateRangeToCssSelector + ".invalid", How.CssSelector)
                        .GetCssValue("box-shadow").Contains("rgb(255, 0, 0)");

            return firstCondition && secondCondition;
        }

        public string GetToolTipOnInvalidInputOnDate(int row = 1, int col = 1)
        {
            if (row == 1)
            {
                return
                    SiteDriver.FindElement(col == 1 ? MaintenanceNoticesPageObjects.DisplayDateRangeFromCssSelector : MaintenanceNoticesPageObjects.DisplayDateRangeToCssSelector,
                        How.CssSelector).GetAttribute("title");
            }
            else
            {
                return
                    SiteDriver.FindElement(col == 1 ? MaintenanceNoticesPageObjects.MaintenanceDateRangeFromCssSelector : MaintenanceNoticesPageObjects.MaintenanceDateRangeToCssSelector,
                        How.CssSelector).GetAttribute("title");
            }
        }


        public void ClickOnEditDefaultMessageLink()
        {
            if (SiteDriver.IsElementPresent(MaintenanceNoticesPageObjects.EditDefaultMessageCssLocator, How.CssSelector))
            {
                JavaScriptExecutor.ExecuteClick(MaintenanceNoticesPageObjects.EditDefaultMessageCssLocator, How.CssSelector);
            }

        }

        public string GetThreeBusinessDayLabelMessage()
        {
            return
                SiteDriver.FindElement(MaintenanceNoticesPageObjects.ThreeBusinessDayLabelCssSelector,
                    How.CssSelector).Text;
        }

        public string GetDiplayDateRangelabelTooltip()
        {
            return
                SiteDriver.FindElement(MaintenanceNoticesPageObjects.DiplayDateRangelabelCssSelector,
                    How.CssSelector).GetAttribute("title");
        }
        public void ClickonEditIcon()
        {
            JavaScriptExecutor.ExecuteClick(MaintenanceNoticesPageObjects.EditIconEnabledCssLocator, How.CssSelector);
            SiteDriver.WaitForCondition(IsEditFormPresent);
        }

        public bool IsEditFormPresent()
        {
            return SiteDriver.IsElementPresent(MaintenanceNoticesPageObjects.EditFormCssLocator, How.CssSelector);
        }


        public void SetDisplayDateFromOnEditSection(string date)
        {
            if (date != "")
            {
                var element = SiteDriver.FindElement(
                    MaintenanceNoticesPageObjects.DisplayDateRangeFromOnEditSectionFieldCssLocator,
                    How.CssSelector);
                element.ClearElementField();
                SiteDriver.WaitTillClear(element);
                SiteDriver.WaitToLoadNew(200);
                element.SendKeys(date);
                Console.WriteLine("Display Date From Set to " + date);
            }
            else
            {
                var element = SiteDriver.FindElement(
                    MaintenanceNoticesPageObjects.DisplayDateRangeFromOnEditSectionFieldCssLocator,
                    How.CssSelector);
                element.ClearElementField();
                SiteDriver.WaitTillClear(element);
                SiteDriver.WaitToLoadNew(200);
                Console.WriteLine("Display Date From Cleared");
            }
        }


        public void SetDisplayDateToOnEditSection(string date)
        {
            if (date != "")
            {
                var element = SiteDriver.FindElement(
                    MaintenanceNoticesPageObjects.DisplayDateRangeToOnEditSectionFieldCssLocator,
                    How.CssSelector);
                element.ClearElementField();
                SiteDriver.WaitTillClear(element);
                SiteDriver.WaitToLoadNew(200);
                element.SendKeys(date);
                Console.WriteLine("Display Date To Set to " + date);
            }
            else
            {
                var element = SiteDriver.FindElement(
                    MaintenanceNoticesPageObjects.DisplayDateRangeToOnEditSectionFieldCssLocator,
                    How.CssSelector);
                element.ClearElementField();
                SiteDriver.WaitTillClear(element);
                SiteDriver.WaitToLoadNew(200);
                Console.WriteLine("Display Date To Cleared");
            }
        }

        public void SetMaintenanceDateRangeFromOnEditSection(string date)
        {
            if (date != "")
            {
                var element = SiteDriver.FindElement(
                    MaintenanceNoticesPageObjects.MaintenanceDateRangeFromOnEditSectionFieldCssLocator,
                    How.CssSelector);
                element.ClearElementField();
                SiteDriver.WaitTillClear(element);
                SiteDriver.WaitToLoadNew(200);
                element.SendKeys(date);
                Console.WriteLine("Maintenance Date From Set to " + date);
            }
            else
            {
                var element = SiteDriver.FindElement(
                    MaintenanceNoticesPageObjects.MaintenanceDateRangeFromOnEditSectionFieldCssLocator,
                    How.CssSelector);
                element.ClearElementField();
                SiteDriver.WaitTillClear(element);
                SiteDriver.WaitToLoadNew(200);
                Console.WriteLine("Maintenance Date From Cleared");
            }
        }

        public void SetMaintenanceDateRangeToOnEditSection(string date)
        {
            if (date != "")
            {
                var element = SiteDriver.FindElement(
                    MaintenanceNoticesPageObjects.MaintenanceDateRangeToOnEditSectionFieldCssLocator,
                    How.CssSelector);
                element.ClearElementField();
                SiteDriver.WaitTillClear(element);
                SiteDriver.WaitToLoadNew(200);
                element.SendKeys(date);
                Console.WriteLine("Maintenance Date To Set to " + date);
            }
            else
            {
                var element = SiteDriver.FindElement(
                    MaintenanceNoticesPageObjects.MaintenanceDateRangeToOnEditSectionFieldCssLocator,
                    How.CssSelector);
                element.ClearElementField();
                SiteDriver.WaitTillClear(element);
                SiteDriver.WaitToLoadNew(200);
                Console.WriteLine("Maintenance Date To Cleared");
            }
        }

        public void ClickonDefaultMessage()
        {
            JavaScriptExecutor.ExecuteClick(MaintenanceNoticesPageObjects.DefaultMessageOnEditSectionFieldCssLocator, How.CssSelector);
            WaitForWorking();
            Console.WriteLine("Clicked on Default Message");
        }
        public void ClickOnSaveButton()
        {
            var element = SiteDriver.FindElement("body", How.CssSelector);
            SiteDriver.ElementToBeClickable(element);
            SiteDriver.WaitToLoadNew(500);
            element.Click();
            JavaScriptExecutor.ExecuteClick(MaintenanceNoticesPageObjects.SaveButtonCssLocator, How.CssSelector);
            Console.WriteLine("Clicked on Save Button");

        }

        public void ClickOnCancelButton()
        {
            JavaScriptExecutor.ExecuteClick(MaintenanceNoticesPageObjects.CancelButtonCssLocator, How.CssSelector);
            Console.WriteLine("Clicked on Cancel Button");
            WaitForWorking();
        }


        public void ClickOnDefaultMessageLinkOnEditSection()
        {
            JavaScriptExecutor.ExecuteClick(MaintenanceNoticesPageObjects.DefaultMessageOnEditSectionFieldCssLocator, How.CssSelector);
        }

    }
}

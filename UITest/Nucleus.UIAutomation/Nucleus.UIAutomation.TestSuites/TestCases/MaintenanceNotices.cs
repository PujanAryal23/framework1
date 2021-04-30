using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Nucleus.Service.Data;
using Nucleus.Service.PageServices.Settings;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Utils;
using Nucleus.UIAutomation.TestSuites.Base;
using Nucleus.Service.Support.Common.Constants;
using NUnit.Framework;
using UIAutomation.Framework.Core.Driver;

namespace Nucleus.UIAutomation.TestSuites.TestCases
{
    public class MaintenanceNotices : NewAutomatedBase
    {
        #region PRIVATE FIELDS

        private MaintenanceNoticesPage _maintenanceNotices;

        #endregion

        #region OVERRIDE METHODS
        protected override void ClassInit()
        {
            try
            {
                base.ClassInit();
                _maintenanceNotices = QuickLaunch.NavigateToMaintenanceNotices();
                var dateTimeFrom = DateTime.Now;
                var dateTimeTo = DateTime.Now.AddDays(20);
                CreateMaintenanceNotice(dateTimeFrom.ToString("MM/dd/yyyy"), dateTimeTo.ToString("MM/dd/yyyy"),
                    dateTimeFrom.ToString("MM/dd/yyyy 00:00"), dateTimeTo.ToString("MM/dd/yyyy 00:00"));
            }
            catch (Exception)
            {
                if (StartFlow != null)
                    StartFlow.Dispose();
                throw;
            }
        }


        protected override void TestCleanUp()
        {
            base.TestCleanUp();
            if (string.Compare(UserType.CurrentUserType, UserType.HCIADMIN, StringComparison.OrdinalIgnoreCase) != 0)
            {
                _maintenanceNotices = _maintenanceNotices.Logout().LoginAsHciAdminUser().NavigateToMaintenanceNotices();
            }
        }
        #endregion

        #region PROTECTED PROPERTIES

        protected override string FullyQualifiedClassName
        {
            get
            {
                return GetType().FullName;
            }
        }
        #endregion

        #region TEST SUITES

        [Test, Category("SmokeTest")] //TANT-204
        public void Verify_red_highlight_in_required_fields_in_add_maintenance_notice()
        {
            StringFormatter.PrintMessageTitle("Verify whether required fields are highlighted red if left empty");
            _maintenanceNotices.ClickOnAddNotice();

            _maintenanceNotices.ClickOnEditDefaultMessageLink();
            _maintenanceNotices.SetNote("");
            _maintenanceNotices.ClickOnScheduleNotice();
            _maintenanceNotices.ClickOnClosePopupModal();

            _maintenanceNotices.IsInvalidInputPresentOnNote().ShouldBeTrue("Red highlight must be shown around 'Message' field" +
                                                                                           "when left empty");
            
            _maintenanceNotices.IsInvalidInputPresentForDisplayDateRangeLabel().ShouldBeTrue("Display Date Range field should be highlighted" +
                                                                                                "red when left empty");
            
            _maintenanceNotices.IsInvalidInputPresentForMaintenanceDateLabel().ShouldBeTrue("Maintenance Date field shoule be highlighted red" +
                                                                                              "if left empty");
        }

    
        [Test]//us39306
        public void Verify_all_required_columns_with_appropriate_labels_appear()
        {
            StringFormatter.PrintMessageTitle(" Checking  Presence of list ");
            _maintenanceNotices.IsListOfNotificationPresent().ShouldBeTrue("List of notices is present");
            StringFormatter.PrintMessageTitle(" Checking  if panel is not open ");
            _maintenanceNotices.IsPanelOpen().ShouldBeFalse("Panel open should be false");
            StringFormatter.PrintMessageTitle(" Checking  Icons ");
            _maintenanceNotices.IsEditIconPresent().ShouldBeTrue("Edit Icon is present");
            _maintenanceNotices.IsDeleteIconPresent().ShouldBeTrue("Delete Icon is present");
            StringFormatter.PrintMessageTitle(" Checking Column Labels ");
            _maintenanceNotices.GetDisplayLabel().ShouldBeEqual("Display:", "Display label is present");
            _maintenanceNotices.GetMaintenanceLabel().ShouldBeEqual("Maintenance:", "Maintenance label is present");
            _maintenanceNotices.GetLastModifiedUserLabel().ShouldBeEqual("LM User:", "LM User label is present");
            _maintenanceNotices.GetLastModifiedDateLabel().ShouldBeEqual("LM Date:", "LM Date label is present");

            StringFormatter.PrintMessageTitle(" Checking Format of Values");
            VerifyThatDateRangeIsInCorrectFormat(_maintenanceNotices.GetDisplayValue());
            VerifyThatDateTimeRangeIsInCorrectFormat(_maintenanceNotices.GetMaintenanceValue());
            VerifyThatNameIsInCorrectFormat(_maintenanceNotices.GetLastModifiedUserValue());

            StringFormatter.PrintMessageTitle(" Checking for status verification ");
            var gridRowCount = _maintenanceNotices.GetActiveNotificationCount();
            StringFormatter.PrintLineBreak();
            StringFormatter.PrintMessageTitle(string.Format("No of Rows in the grid: <{0}>", gridRowCount));
            if (gridRowCount == 1)
            {
                var dateTimeFrom = DateTime.Now.AddDays(1);
                var dateTimeTo = DateTime.Now.AddDays(21);
                CreateMaintenanceNotice(dateTimeFrom.ToString("MM/dd/yyyy"), dateTimeTo.ToString("MM/dd/yyyy"),
                    dateTimeFrom.ToString("MM/dd/yyyy 00:00"), dateTimeTo.ToString("MM/dd/yyyy 00:00"),true);
            }
            gridRowCount = _maintenanceNotices.GetActiveNotificationCount();
            for (var row = 1; row < gridRowCount; row++)
                {
                    var firstrowdate = _maintenanceNotices.GetDisplayValueForGivenRow(row)
                        .Split(new[] {"-"}, StringSplitOptions.None);
                    var firstrowstartdate = Convert.ToDateTime(firstrowdate[0]);
                    var firstrowenddate = Convert.ToDateTime(firstrowdate[1]);

                    var secondrowdate = _maintenanceNotices.GetDisplayValueForGivenRow(row + 1)
                        .Split(new[] {"-"}, StringSplitOptions.None);
                    var secondrowstartdate = Convert.ToDateTime(secondrowdate[0]);
                    var secondrowenddate = Convert.ToDateTime(secondrowdate[1]);
                    (firstrowstartdate >= secondrowstartdate || firstrowenddate >= secondrowenddate).ShouldBeTrue(
                        "Maintenance list is sorted with the most recent changes at the top");
                    VerifyStatusForDaterange(_maintenanceNotices.GetDisplayValueForGivenRow(row),
                        _maintenanceNotices.GetStatusValueForGivenRow(row),row);
                }

            
        }


        [Test] //US39305
        public void Verify_navigation_of_maintenance_notices()
        {
            _maintenanceNotices.IsMaintenanceNoticesSubMenuPresent().ShouldBeTrue("Maintenance Notices submenu present for authorized user.");
            _maintenanceNotices.GetPageInsideTitle().ShouldBeEqual("Maintenance Notices", "Correct Page Title displayed inside the page.");
            _maintenanceNotices.Logout().LoginAsClientUser();
            _maintenanceNotices.IsMaintenanceNoticesSubMenuPresent().ShouldBeFalse("Maintenance Notices submenu present for client user.");
            _maintenanceNotices.Logout().LoginAsUserHavingNoManageCategoryAuthority();
            _maintenanceNotices.IsMaintenanceNoticesSubMenuPresent().ShouldBeFalse("Maintenance Notices submenu present for unauthorized user.");
        }

        [Test]//US39308
        public void Verify_that_panel_opens_on_right_side()
        {
            _maintenanceNotices.ClickOnNotificationRow(1);
            _maintenanceNotices.IsPanelOpen().ShouldBeTrue("Right sided panel is open and present.");
            StringFormatter.PrintMessageTitle(" Checking details being displayed ");
            _maintenanceNotices.GetNotificationDateRangeLabelForRightPanel().ShouldBeEqual("Notification Date range:", "Notification Date range label is present");
            _maintenanceNotices.GetMaintenanceDateRangeLabelForRightPanel().ShouldBeEqual("Maintenance Date/Time range:", "Maintenance Date/Time range label is present");
            _maintenanceNotices.GetLastModifiedUserLabelForRightPanel().ShouldBeEqual("LM User:", "Last modified user label is present");
            _maintenanceNotices.GetLastModifiedDateLabelForRightPanel().ShouldBeEqual("LM Date:", "Last modified date label is present");
            _maintenanceNotices.GetNotificationStatusLabelForRightPanel().ShouldBeEqual("Status:", "Status label is present");
            _maintenanceNotices.GetNotificationMessageLabelForRightPanel().ShouldBeEqual("Scheduled Notification Message:", "Notification Message label is present");
            _maintenanceNotices.IsNotificationMessagePresentInRightPanel().ShouldBeTrue("Notification Message is present");
            _maintenanceNotices.GetDisplayValue()
                .ShouldBeEqual(_maintenanceNotices.GetNotificationDateRangeValueForRightPanel(),
                    "Notification Date Range value of grid matches that of panel");
            _maintenanceNotices.GetMaintenanceValue()
               .ShouldBeEqual(_maintenanceNotices.GetMaintenanceDateRangeValueForRightPanel(),
                   "Maintenance Date Range value of grid matches that of panel");
            _maintenanceNotices.GetLastModifiedUserValue()
               .ShouldBeEqual(_maintenanceNotices.GetLastModifiedUserValueForRightPanel(),
                   "Last modified user value of grid matches that of panel");
            _maintenanceNotices.GetLastModifiedDateValue()
               .ShouldBeEqual(_maintenanceNotices.GetLastModifiedDateValueForRightPanel(),
                   "Last modified  Date Range of grid matches that of panel");
            var status = _maintenanceNotices.GetStatusValueForGivenRow(1);
            switch (status)
            {
                case "A":
                    _maintenanceNotices.GetNotificationStatusValueForRightPanel()
               .ShouldBeEqual("Active",
                   "Status of notice of grid matches that of panel.");
                    break;
                case "C":
                    _maintenanceNotices.GetNotificationStatusValueForRightPanel()
               .ShouldBeEqual("Cancelled",
                   "Status of notice of grid matches that of panel.");
                    break;
                case "I":
                    _maintenanceNotices.GetNotificationStatusValueForRightPanel()
               .ShouldBeEqual("Inactive",
                   "Status of notice of grid matches that of panel.");
                    break;
                
            }
        }

        [Test]//us393111
        public void Verfiy_maintenance_notice_displays_on_login_page()
        {
            var totalActiveStatus = _maintenanceNotices.GetActiveNotificationCount();
            if (totalActiveStatus > 1)
            {
                var smallestDate = Convert.ToDateTime(_maintenanceNotices.GetDisplayValueForGivenRow(totalActiveStatus)
                    .Split(new[] { "-" }, StringSplitOptions.None)[0]);
                var requiredRow = totalActiveStatus;
                for (var row = totalActiveStatus; row>=1; row--)
                {
                    var currentrowdate = _maintenanceNotices.GetDisplayValueForGivenRow(row)
                      .Split(new[] { "-" }, StringSplitOptions.None);
                    var currentrowstartdate = Convert.ToDateTime(currentrowdate[0]);
                    if (smallestDate > currentrowstartdate)
                    {
                        requiredRow = row;
                        smallestDate = currentrowstartdate;
                    }
                }
                _maintenanceNotices.ClickOnNotificationRow(requiredRow); 
            }
            else
            {
                _maintenanceNotices.ClickOnNotificationRow(1);
            }

            _maintenanceNotices.IsPanelOpen().ShouldBeTrue("Right sided panel is open and present.");
            _maintenanceNotices.IsNotificationMessagePresentInRightPanel().ShouldBeTrue("Notification Message is present");
            var maintenanceMessage = _maintenanceNotices.GetNotificationMessageLValueInRightPanel().Replace(System.Environment.NewLine, "")+ " MT.";//since the msessage will have MT by default later and removing line break
             var maintenanceDateRange = _maintenanceNotices.GetMaintenanceDateRangeValueForRightPanel().Split(new[] { "-" }, StringSplitOptions.None);
            var maintenanceStartDate = Convert.ToDateTime(maintenanceDateRange[0]);
            var maintenanceEndDate = Convert.ToDateTime(maintenanceDateRange[1]);
            var todayDate = DateTime.Now;
            try
            {


                var loginpage = _maintenanceNotices.Logout();
                _maintenanceNotices.IsMaintenaceNoticePresent()
                    .ShouldBeTrue("Maintenance notice is present in the login page");
                var loginmaintenanceMessage =
                    _maintenanceNotices.GetMaintenaceNoticeDescriptionFromLoginPage()
                        .Replace(System.Environment.NewLine, "")
                        .Replace(" MT ", " ");
                
                (loginmaintenanceMessage)
                    .ShouldBeEqual(maintenanceMessage,
                        "Maintenance notice description equals the notice description present for the nearest display date message.");

                Console.WriteLine("Checking if the time frame lies in maintenance time frame");
                if (maintenanceStartDate <= todayDate && todayDate <= maintenanceEndDate)
                //checking i login is allowed during maintenance time frame
                {
                    Console.WriteLine("Trying to login during the maintenance time frame");
                    loginpage.LoginAsHciAdminUser();
                    CheckTestClientAndSwitch();
                    _maintenanceNotices = QuickLaunch.NavigateToMaintenanceNotices();
                    Console.WriteLine("Successfully login during the maintenance time frame");
                }
            }
            finally
            {
                if (_maintenanceNotices.CurrentPageTitle== PageTitleEnum.Login.GetStringValue())
                {
                    CurrentPage = QuickLaunch = Login.LoginAsHciAdminUser();
                    _maintenanceNotices = QuickLaunch.NavigateToMaintenanceNotices();
                }
            }
        }
        [Test]//US39310
        public void Verify_that_cancel_scheduled_maintenance_notification()
        {
            var flagForCancelNotification = true;
            var flagForCancelClick = true;
            var noticeRowCount = _maintenanceNotices.GetResultGridRowCount();
            
            for (var row = 1; row <= noticeRowCount; row++)
            {
                
                _maintenanceNotices.IsDeleteIconPresent(row).ShouldBeTrue("Delete Icon (X) Present");
                if (_maintenanceNotices.GetStatusValueForGivenRow(row) == "I")
                {
                    _maintenanceNotices.IsDeleteIconDisabled(row)
                        .ShouldBeTrue("Deleted Icon (X) is disabled for inactive notifications");
                    if (_maintenanceNotices.GetStatusValueForGivenRow(row + 1) == "I")
                        break;
                }
                if (_maintenanceNotices.GetStatusValueForGivenRow(row) == "C" && flagForCancelNotification)
                {
                    _maintenanceNotices.GetStatusValueForGivenRow(row - 1)
                        .ShouldBeEqual("A", "Cancelled notifications are below the Active scheduled notifications");
                    flagForCancelNotification = false;
                }
                if (_maintenanceNotices.IsEditIconEnabled() && flagForCancelClick)
                {
                    var beforeStatus = _maintenanceNotices.GetStatusValueForGivenRow(row);
                    _maintenanceNotices.ClickOnDeleteIcon(row);
                    if(_maintenanceNotices.IsPopupModalPresent())
                        _maintenanceNotices.ClickOnCancelOnConfirmationModal();
                    _maintenanceNotices.GetStatusValueForGivenRow(row).ShouldBeEqual(beforeStatus, "No changes occur");
                    flagForCancelClick = false;
                }


            }
        }

        [Test]//US39309
        public void Verify_proper_validation_on_edit_scheduled_maintenance_notification()
        {
            StringFormatter.PrintMessageTitle("Checking  Edit Icon present");
            var totalRow = _maintenanceNotices.GetResultGridRowCount();
            try
            {
               // _maintenanceNotices.IsEditIconPresent().ShouldBeTrue("Edit Icon is present");
                _maintenanceNotices.IsEditIconEnabled().ShouldBeTrue("Edit Icon is present and enabled");
                _maintenanceNotices.ClickonEditIcon();
                _maintenanceNotices.IsEditFormPresent()
                    .ShouldBeTrue("Edit Maintenance Notice Section is displayed below the selected record");

                Console.WriteLine("Checking validation status for incorrect display start date");
                _maintenanceNotices.SetDisplayDateFromOnEditSection("!@#$%");
                VerifyEditValidationMessage("Validation message displayed when incorrect Display start Date ");
                VerfiyDisplayDateToolTip("Tooltip displayed when incorrect Display start Date");
                Console.WriteLine("Checking validation status for empty display start date");
                _maintenanceNotices.SetDisplayDateFromOnEditSection("");
                VerifyEditValidationMessage("Validation message displayed when  Display start Date is Empty ");
                VerfiyDisplayDateToolTip("Tooltip displayed when empty Display start Date");
                Console.WriteLine("Checking validation status for incorrect end display date");
                _maintenanceNotices.SetDisplayDateToOnEditSection("!@#$%");
                VerifyEditValidationMessage("Validation message displayed when incorrect Display End Date");
                VerfiyDisplayDateToolTip("Tooltip displayed when incorrect Display End Date");
                Console.WriteLine("Checking validation status for empty display End date");
                _maintenanceNotices.SetDisplayDateToOnEditSection("");
                VerifyEditValidationMessage("Validation message displayed when  Display End Date Empty ");
                VerfiyDisplayDateToolTip("Tooltip displayed when empty Display End Date");


                _maintenanceNotices.SetMaintenanceDateRangeFromOnEditSection("");
                VerifyEditValidationMessage("Validation message displayed when  Maintenance Date From is Empty ");
                VerfiyMaintenanceDateToolTip("Tooltip displayed when empty Maintenance start Date");
                Console.WriteLine("Checking validation status for incorrect  maintenance date");
                _maintenanceNotices.SetMaintenanceDateRangeFromOnEditSection("!@#$%");
                VerifyEditValidationMessage("Validation message displayed when incorrect Maintenance start Date");
                VerfiyMaintenanceDateToolTip("Tooltip displayed when incorrect maintenance start Date");
                _maintenanceNotices.SetMaintenanceDateRangeToOnEditSection("");
                VerifyEditValidationMessage("Validation message displayed when  Maintenance Date to is Empty ");
                VerfiyMaintenanceDateToolTip("Tooltip displayed when empty Maintenance Date To");
                _maintenanceNotices.SetMaintenanceDateRangeToOnEditSection("!@#$%");
                VerifyEditValidationMessage("Validation message displayed when incorrect Maintenance End Date");
                VerfiyMaintenanceDateToolTip("Tooltip displayed when incorrect maintenance end Date");
            
                Console.WriteLine("Checking validation status for daterange less than 3 business days");
                _maintenanceNotices.SetDisplayDateFromOnEditSection(DateTime.Now.ToString("MM'/'dd'/'yyyy"));
                _maintenanceNotices.SetDisplayDateToOnEditSection(DateTime.Now.AddDays(1).ToString("MM'/'dd'/'yyyy"));
                VerifyEditValidationMessage("Validation message displayed when  Display Date range is less than 3 business days ");
                VerfiyDisplayDateToolTip("Tooltip displayed when Display date range is less than 3 business days");

                _maintenanceNotices.SetDisplayDateFromOnEditSection(DateTime.Now.AddDays(3).ToString("MM'/'dd'/'yyyy")); 
                VerifyEditValidationMessage("Validation message displayed when  Display Begin Date is greater than End Date ");
                
                _maintenanceNotices.GetToolTipOnInvalidInputOnDate(1)
                    .ShouldBeEqual(
                        "The Display Date Range begin date must be earlier than or equal to the Display Date Range end date.",
                        "Proper Tooltip Shown When Display  Begin Date is greater than End Date");
                _maintenanceNotices.GetToolTipOnInvalidInputOnDate(1,2)
                    .ShouldBeEqual(
                        "The Display Date Range begin date must be earlier than or equal to the Display Date Range end date.",
                        "Proper Tooltip Shown When Display  Begin Date is greater than End Date");
                Console.WriteLine("Checking validation status for  Display Date range greater than maintenance date");
                _maintenanceNotices.SetDisplayDateFromOnEditSection(DateTime.Now.AddDays(1).ToString());
                _maintenanceNotices.SetDisplayDateToOnEditSection(DateTime.Now.AddDays(5).ToString());
                _maintenanceNotices.SetMaintenanceDateRangeFromOnEditSection(DateTime.Now.ToString());
                _maintenanceNotices.SetMaintenanceDateRangeToOnEditSection(DateTime.Now.ToString()); 
                VerifyEditValidationMessage("Validation message displayed when  Display Date range is greater than maintenance date ");
                
                _maintenanceNotices.GetToolTipOnInvalidInputOnDate(2)
                    .ShouldBeEqual(
                        "Display date cannot be greater than maintenance date",
                        "Proper Tooltip Shown When Display  Date is greater than maintenance date");
                _maintenanceNotices.GetToolTipOnInvalidInputOnDate(2,2)
                    .ShouldBeEqual(
                        "Display date cannot be greater than maintenance date",
                        "Proper Tooltip Shown When Display  Date is greater than maintenance date");
                Console.WriteLine("Checking validation status for  blank note");
               // _maintenanceNotices.ClickOnEditDefaultMessageLink();
                _maintenanceNotices.SetNote("");
                VerifyEditValidationMessage("Validation message displayed when note is empty ");
                _maintenanceNotices.GetInvalidTooltipMessageOnNote()
                    .ShouldBeEqual(
                        "Message is required",
                        "Proper Tooltip Shown When Note is blank");
                _maintenanceNotices.ClickOnDefaultMessageLinkOnEditSection();
                SetNoticeInPast();
                Console.WriteLine("Checking if cancel setting dispay date range in past discards changes");
                _maintenanceNotices.ClickOnCancelOnConfirmationModal();
                _maintenanceNotices.IsEditFormPresent().ShouldBeTrue("Changes are discarded"); //no changes made

                Console.WriteLine("Checking cancel link validation");
                _maintenanceNotices.ClickOnCancelScheduleNotice();
                _maintenanceNotices.ClickOnCancelOnConfirmationModal();
                _maintenanceNotices.IsEditFormPresent().ShouldBeTrue("Cancel new changes confirmation discarded"); //no changes made
                _maintenanceNotices.ClickOnCancelScheduleNotice();
                _maintenanceNotices.ClickOnOkButtonOnConfirmationModal();
                _maintenanceNotices.GetResultGridRowCount()
                    .ShouldBeEqual(totalRow, "Total Maintenance Row Count should be same as before");
                _maintenanceNotices.IsEditFormPresent().ShouldBeFalse("Cancel new changes confirmation approved");
               
             }

            finally
            {
                if (_maintenanceNotices.IsPopupModalPresent())
                    _maintenanceNotices.ClickOnClosePopupModal();
                if (_maintenanceNotices.IsEditFormPresent())
                {
                    _maintenanceNotices.ClickOnCancelScheduleNotice();
                    _maintenanceNotices.ClickOnOkButtonOnConfirmationModal();
                }


            }
            
        }

        private void SetNoticeInPast()
        {
            Console.WriteLine("Checking validation message for notice scheduled in past");
            _maintenanceNotices.SetDisplayDateFromOnEditSection(DateTime.Now.AddDays(-5).ToString("MM/dd/yyyy"));
            _maintenanceNotices.SetDisplayDateToOnEditSection(DateTime.Now.AddDays(-1).ToString("MM/dd/yyyy"));
            _maintenanceNotices.SetMaintenanceDateRangeFromOnEditSection(DateTime.Now.AddDays(-3).ToString("MM/dd/yyyy HH:mm"));
            _maintenanceNotices.SetMaintenanceDateRangeToOnEditSection(DateTime.Now.AddDays(-2).ToString("MM/dd/yyyy HH:mm"));
           // 
            _maintenanceNotices.ClickOnSaveButton();
            _maintenanceNotices.WaitForCondition(()=>_maintenanceNotices.IsPopupModalPresent());
            _maintenanceNotices.GetPopupModalMessage().ShouldBeEqual(
                "The Display Date Range is set in the past. Do you wish to proceed?", "Proper Validation Message when past date");
        }

        [Test]//us39309
        public void Verify_edit_schedule_maintenance_notification()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            IDictionary<string, string> paramLists = DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);

            var note = paramLists["Note"];
            var maxNote = paramLists["MaxNote"];

            StringFormatter.PrintMessageTitle("Verifying Edit Scheduled Maintenance Notice ");
            var dateTimeFrom = DateTime.Now;
            var dateTimeTo = DateTime.Now.AddDays(5);
            try
            {
                var totalRow = _maintenanceNotices.GetResultGridRowCount();
                if (_maintenanceNotices.IsEditIconEnabled()) { 
                _maintenanceNotices.IsEditIconEnabled().ShouldBeTrue("Edit Icon is present and enabled");
                _maintenanceNotices.ClickonEditIcon();
                _maintenanceNotices.IsEditFormPresent()
                    .ShouldBeTrue("Edit Maintenance Notice Section is displayed below the selected record");
                _maintenanceNotices.ClickOnDisplayDateRangeFrom();
                _maintenanceNotices.SetDate(dateTimeFrom.ToString("MM/d/yyyy"));
                _maintenanceNotices.ClickOnDisplayDateRangeTo();
                _maintenanceNotices.SetDate(dateTimeTo.ToString("MM/d/yyyy"));

                    var preMaintenanceTimeFrom = _maintenanceNotices.GetMaintenanceDateRangeFrom().Split(' ')[1];
                    var preMaintenanceTimeTo = _maintenanceNotices.GetMaintenanceDateRangeTo().Split(' ')[1];

                _maintenanceNotices.ClickOnMaintenanceDateRangeFrom();
                _maintenanceNotices.SetTime(dateTimeFrom.ToString("HH:mm"));
                if (dateTimeFrom.ToString("H:mm").Split(':')[0] != preMaintenanceTimeFrom.Split(':')[0])
                    _maintenanceNotices.ClickOnMaintenanceDateRangeFrom();
                _maintenanceNotices.SetDate(dateTimeFrom.ToString("MM/d/yyyy"));

                _maintenanceNotices.ClickOnMaintenanceDateRangeTo();
                _maintenanceNotices.SetTime(dateTimeTo.ToString("HH:mm"));
                if (dateTimeTo.ToString("HH:mm").Split(':')[0] != preMaintenanceTimeTo.Split(':')[0])
                    _maintenanceNotices.ClickOnMaintenanceDateRangeTo();
                _maintenanceNotices.SetDate(dateTimeTo.ToString("MM/d/yyyy"));

                _maintenanceNotices.GetDisplayDateFrom()
                    .ShouldBeEqual(dateTimeFrom.ToString("MM/dd/yyyy"),
                        "Verfication of Display Date From Selected From Calender ");
                _maintenanceNotices.GetDisplayDateTo()
                    .ShouldBeEqual(dateTimeTo.ToString("MM/dd/yyyy"),
                        "Verfication of Display Date To Selected From Calender ");
                _maintenanceNotices.GetMaintenanceDateRangeFrom()
                    .ShouldBeEqual(dateTimeFrom.ToString("MM/dd/yyyy H:mm"),
                        "Verfication of Maintenance Date/Time From Selected From Calender ");
                _maintenanceNotices.GetMaintenanceDateRangeTo()
                    .ShouldBeEqual(dateTimeTo.ToString("MM/dd/yyyy H:mm"),
                        "Verfication of Display Date/Time From Selected From Calender ");

                _maintenanceNotices.ClickOnEditDefaultMessageLink();//click on default message
                _maintenanceNotices.GetNote().ShouldBeEqual(note, "Verify Default Note Message");
                _maintenanceNotices.ClickOnEditDefaultMessageLink();//set focus on note
                _maintenanceNotices.SetNote(maxNote);
                _maintenanceNotices.SetDisplayDateFrom(dateTimeFrom.ToString("MM/dd/yyyy"));
                _maintenanceNotices.SetDisplayDateTo(dateTimeTo.ToString("MM/dd/yyyy"));
                _maintenanceNotices.GetDiplayDateRangelabelTooltip().ShouldBeEqual("The maintenance notice will be displayed during this time period",
                    "Verify Dispaly Date Range Tooltip");
               
                _maintenanceNotices.ClickOnScheduleNotice();
                _maintenanceNotices.ClickOnClosePopupModal();
                _maintenanceNotices.GetInvalidTooltipMessageOnNote()
                    .ShouldBeEqual(
                        "Message must be less than 200 characters in length.",
                        "Proper Tooltip Shown When Note is more than 200 character");
                _maintenanceNotices.ClickOnEditDefaultMessageLink();
                _maintenanceNotices.GetNote().ShouldBeEqual(note, "Verify Default Note Message when click on Default Message");



                _maintenanceNotices.SetMaintenanceDateRangeFrom(dateTimeFrom.ToString("MM/dd/yyyy"));
                _maintenanceNotices.SetMaintenanceDateRangeTo(dateTimeTo.ToString("MM/dd/yyyy"));

                _maintenanceNotices.GetPreviewMessage().Split('\r')[0].ShouldBeEqual(note, "Preview Message Verfication");
                _maintenanceNotices.GetPreviewMessageDateTime()
                    .ShouldBeEqual(dateTimeFrom.Date.ToString("dddd, MMMM d, yyyy h:mm tt") + " to "
                    + dateTimeTo.Date.ToString("dddd, MMMM d, yyyy h:mm tt"), "Maintenance Date Time is displayed in bold?");

                _maintenanceNotices.ClickOnCancelScheduleNotice();
                _maintenanceNotices.GetConfirmationMessage()
                    .ShouldBeEqual("Any unsaved changes will be discarded. Do you wish to proceed?",
                        "Confirmation Message verification");
                _maintenanceNotices.ClickOnOkButtonOnConfirmationModal();
                _maintenanceNotices.GetResultGridRowCount()
                    .ShouldBeEqual(totalRow, "Total Maintenance Row Count should be same as before");
                }

            }
            finally
            {
                if (_maintenanceNotices.IsPopupModalPresent())
                    _maintenanceNotices.ClickOnClosePopupModal();
                if (_maintenanceNotices.IsEditFormPresent())
                {
                    _maintenanceNotices.ClickOnCancelScheduleNotice();
                    _maintenanceNotices.ClickOnOkButtonOnConfirmationModal();
                }

            }
        }

        [Test]//us39307
        public void Verify_proper_validation_message_shown_on_add_maintenance_notices()
        {
            StringFormatter.PrintMessageTitle("Verifying Validation Message");
            try
            {
                _maintenanceNotices.ClickOnAddNotice();
                _maintenanceNotices.ClickOnScheduleNotice();
                _maintenanceNotices.GetPopupModalMessage().ShouldBeEqual(
                    "Invalid or missing data must be resolved before the notice can be scheduled.","Proper Validation Message when any field is blank");
                _maintenanceNotices.ClickOnClosePopupModal();

                _maintenanceNotices.GetToolTipOnInvalidInputOnDate(1)
                    .ShouldBeEqual(
                        "Maintenance notices must be displayed for a minimum of three business days (excluding holidays).",
                        "Proper Tooltip Shown When Display Date is Blank");
                _maintenanceNotices.GetToolTipOnInvalidInputOnDate(1,2)
                    .ShouldBeEqual(
                        "Maintenance notices must be displayed for a minimum of three business days (excluding holidays).",
                        "Proper Tooltip Shown When Display Date is Blank");

                _maintenanceNotices.GetToolTipOnInvalidInputOnDate(2)
                    .ShouldBeEqual(
                        "Maintenance Date and Time is required.",
                        "Proper Tooltip Shown When Maintenance Date is Blank");
                _maintenanceNotices.GetToolTipOnInvalidInputOnDate(2,2)
                    .ShouldBeEqual(
                        "Maintenance Date and Time is required.",
                        "Proper Tooltip Shown When Maintenance Date is Blank");

                _maintenanceNotices.SetDisplayDateFrom(DateTime.Now.ToString("MM/dd/yyyy"));
                _maintenanceNotices.SetDisplayDateTo(DateTime.Now.AddDays(1).ToString("MM/dd/yyyy"));
                _maintenanceNotices.ClickOnScheduleNotice();
                _maintenanceNotices.ClickOnClosePopupModal();
                _maintenanceNotices.GetToolTipOnInvalidInputOnDate(1)
                    .ShouldBeEqual(
                        "Maintenance notices must be displayed for a minimum of three business days (excluding holidays).",
                        "Proper Tooltip Shown When Display Date is less than 3 business days");
                _maintenanceNotices.GetToolTipOnInvalidInputOnDate(1,2)
                    .ShouldBeEqual(
                        "Maintenance notices must be displayed for a minimum of three business days (excluding holidays).",
                        "Proper Tooltip Shown When Display Date is less than 3 business days");
                _maintenanceNotices.SetDisplayDateFrom(DateTime.Now.AddDays(3).ToString("MM/dd/yyyy"));
                _maintenanceNotices.ClickOnScheduleNotice();
                _maintenanceNotices.ClickOnClosePopupModal();
                _maintenanceNotices.GetToolTipOnInvalidInputOnDate(1)
                    .ShouldBeEqual(
                        "The Display Date Range begin date must be earlier than or equal to the Display Date Range end date.",
                        "Proper Tooltip Shown When Display  Begin Date is greater than End Date");
                _maintenanceNotices.GetToolTipOnInvalidInputOnDate(1,2)
                    .ShouldBeEqual(
                        "The Display Date Range begin date must be earlier than or equal to the Display Date Range end date.",
                        "Proper Tooltip Shown When Display  Begin Date is greater than End Date");

                _maintenanceNotices.SetDisplayDateFrom(DateTime.Now.AddDays(1).ToString("MM/dd/yyyy"));
                _maintenanceNotices.SetDisplayDateTo(DateTime.Now.AddDays(5).ToString("MM/dd/yyyy"));
                _maintenanceNotices.SetMaintenanceDateRangeFrom(DateTime.Now.ToString("MM/dd/yyyy HH:mm"));
                _maintenanceNotices.SetMaintenanceDateRangeTo(DateTime.Now.ToString("MM/dd/yyyy HH:mm"));
                _maintenanceNotices.ClickOnScheduleNotice();
                _maintenanceNotices.ClickOnClosePopupModal();
                _maintenanceNotices.GetToolTipOnInvalidInputOnDate(2)
                    .ShouldBeEqual(
                        "Display date cannot be greater than maintenance date",
                        "Proper Tooltip Shown When Display  Date is greater than maintenance date");
                _maintenanceNotices.GetToolTipOnInvalidInputOnDate(2,2)
                    .ShouldBeEqual(
                        "Display date cannot be greater than maintenance date",
                        "Proper Tooltip Shown When Display  Date is greater than maintenance date");
                _maintenanceNotices.ClickOnEditDefaultMessageLink();
                _maintenanceNotices.SetNote("");
                _maintenanceNotices.ClickOnScheduleNotice();
                _maintenanceNotices.ClickOnClosePopupModal();
                _maintenanceNotices.GetInvalidTooltipMessageOnNote()
                    .ShouldBeEqual(
                        "Message is required",
                        "Proper Tooltip Shown When Note is blank");
                _maintenanceNotices.ClickOnEditDefaultMessageLink();
                _maintenanceNotices.SetDisplayDateFrom(DateTime.Now.AddDays(-5).ToString("MM/dd/yyyy"));
                _maintenanceNotices.SetDisplayDateTo(DateTime.Now.AddDays(-1).ToString("MM/dd/yyyy"));
                _maintenanceNotices.SetMaintenanceDateRangeFrom(DateTime.Now.AddDays(-5).ToString("MM/dd/yyyy HH:mm"));
                _maintenanceNotices.SetMaintenanceDateRangeTo(DateTime.Now.AddDays(-2).ToString("MM/dd/yyyy HH:mm"));

                _maintenanceNotices.ClickOnScheduleNotice();
                _maintenanceNotices.GetPopupModalMessage().ShouldBeEqual(
                    "One or more of the dates selected occurs in the past. Please select different dates in order to continue.", "Proper Validation Message when past date");
                _maintenanceNotices.ClickOnClosePopupModal();
            }
            finally
            {
                if (_maintenanceNotices.IsPopupModalPresent())
                    _maintenanceNotices.ClickOnClosePopupModal();
                if (_maintenanceNotices.IsAddNewCategoryCodeSectionDisplayed())
                {
                    _maintenanceNotices.ClickOnCancelScheduleNotice();
                    _maintenanceNotices.ClickOnOkButtonOnConfirmationModal();
                }
            }
        }

        [Test]//us39307
        public void Verify_add_schedule_maintenance_notifications()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            IDictionary<string, string> paramLists = DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);

            var note = paramLists["Note"];
            var maxNote = paramLists["MaxNote"]; 
            
            StringFormatter.PrintMessageTitle("Verifying Add Schedule Maintenance Notice Form ");
            var dateTimeFrom = DateTime.Now;
            var dateTimeTo = DateTime.Now.AddDays(5);
            try
            {
                var totalRow = _maintenanceNotices.GetResultGridRowCount();
                _maintenanceNotices.ClickOnAddNotice();
                _maintenanceNotices.GetAddScheduleHederLabel().ShouldBeEqual("Schedule Maintenance Notice", "Add Schedule Maintenance Notice label");
                _maintenanceNotices.GetThreeBusinessDayLabelMessage()
                    .ShouldBeEqual(
                        "System maintenance notices must be displayed for a minimum of three business days (excluding holidays).",
                        "Three Business Days Message");

                _maintenanceNotices.ClickOnDisplayDateRangeFrom();
                _maintenanceNotices.SetDate(dateTimeFrom.ToString("MM/d/yyyy"));
                _maintenanceNotices.ClickOnDisplayDateRangeTo();
                _maintenanceNotices.SetDate(dateTimeTo.ToString("MM/d/yyyy"));

                _maintenanceNotices.ClickOnMaintenanceDateRangeFrom();
                _maintenanceNotices.SetTime(dateTimeFrom.ToString("HH:mm"));
                if (dateTimeFrom.ToString("HH:mm").Split(':')[0]!="00")
                    _maintenanceNotices.ClickOnMaintenanceDateRangeFrom();
                _maintenanceNotices.SetDate(dateTimeFrom.ToString("MM/d/yyyy"));

                _maintenanceNotices.ClickOnMaintenanceDateRangeTo();
                _maintenanceNotices.SetTime(dateTimeTo.ToString("HH:mm"));
                if (dateTimeTo.ToString("HH:mm").Split(':')[0] != "00")
                    _maintenanceNotices.ClickOnMaintenanceDateRangeTo();
                _maintenanceNotices.SetDate(dateTimeTo.ToString("MM/d/yyyy"));

                _maintenanceNotices.GetDisplayDateFrom()
                    .ShouldBeEqual(dateTimeFrom.ToString("MM/dd/yyyy"),
                        "Verfication of Display Date From Selected From Calender ");
                _maintenanceNotices.GetDisplayDateTo()
                    .ShouldBeEqual(dateTimeTo.ToString("MM/dd/yyyy"),
                        "Verfication of Display Date To Selected From Calender ");
                _maintenanceNotices.GetMaintenanceDateRangeFrom()
                    .ShouldBeEqual(dateTimeFrom.ToString("MM/dd/yyyy H:mm"),
                        "Verfication of Maintenance Date/Time From Selected From Calender ");
                _maintenanceNotices.GetMaintenanceDateRangeTo()
                    .ShouldBeEqual(dateTimeTo.ToString("MM/dd/yyyy H:mm"),
                        "Verfication of Maintenance Date/Time To Selected From Calender ");

                _maintenanceNotices.GetNote().ShouldBeEqual(note, "Verify Default Note Message");
                _maintenanceNotices.ClickOnEditDefaultMessageLink();
                _maintenanceNotices.SetNote(maxNote);
                _maintenanceNotices.SetDisplayDateFrom(dateTimeFrom.ToString("MM/dd/yyyy"));
                _maintenanceNotices.SetDisplayDateTo(dateTimeTo.ToString("MM/dd/yyyy"));
                _maintenanceNotices.GetDiplayDateRangelabelTooltip().ShouldBeEqual("The maintenance notice will be displayed during this time period",
                    "Verify Dispaly Date Range Tooltip");
                
                _maintenanceNotices.ClickOnScheduleNotice();
                _maintenanceNotices.ClickOnClosePopupModal();
                _maintenanceNotices.GetInvalidTooltipMessageOnNote()
                    .ShouldBeEqual(
                        "Message must be less than 200 characters in length.",
                        "Proper Tooltip Shown When Note is more than 200 character");
                _maintenanceNotices.ClickOnEditDefaultMessageLink();
                _maintenanceNotices.GetNote().ShouldBeEqual(note, "Verify Default Note Message when click on Default Message");



                _maintenanceNotices.SetMaintenanceDateRangeFrom(dateTimeFrom.ToString("MM/dd/yyyy"));
                _maintenanceNotices.SetMaintenanceDateRangeTo(dateTimeTo.ToString("MM/dd/yyyy"));

                _maintenanceNotices.GetPreviewMessage().Split('\r')[0].ShouldBeEqual(note, "Preview Message Verfication");
                _maintenanceNotices.GetPreviewMessageDateTime()
                    .ShouldBeEqual(dateTimeFrom.Date.ToString("dddd, MMMM d, yyyy h:mm tt") + " to "
                    + dateTimeTo.Date.ToString("dddd, MMMM d, yyyy h:mm tt"), "Maintenance Date Time is displayed in bold?");

                _maintenanceNotices.ClickOnCancelScheduleNotice();
                _maintenanceNotices.GetConfirmationMessage()
                    .ShouldBeEqual("Any unsaved changes will be discarded. Do you wish to proceed?",
                        "Confirmation Message verification");
                _maintenanceNotices.ClickOnOkButtonOnConfirmationModal();
                _maintenanceNotices.GetResultGridRowCount()
                    .ShouldBeEqual(totalRow, "Total Maintenance Row Count should be same as before");


                

            }
            finally
            {
                if (_maintenanceNotices.IsPopupModalPresent())
                    _maintenanceNotices.ClickOnClosePopupModal();
                if (_maintenanceNotices.IsAddNewCategoryCodeSectionDisplayed())
                {
                    _maintenanceNotices.ClickOnCancelScheduleNotice();
                    _maintenanceNotices.ClickOnOkButtonOnConfirmationModal();
                }

            }
        }

       

        #endregion

        #region PRIVATE METHODS

       
        private void CreateMaintenanceNotice(string displayDateFrom, string displayDateTo, string maintenanceDateFrom,
            string maintenanceDateTo,bool add=false)
        {
            var count= _maintenanceNotices.GetActiveNotificationCount();
            if (count != 0 && !add) return;
            _maintenanceNotices.ClickOnAddNotice();
            _maintenanceNotices.SetDisplayDateFrom(displayDateFrom);
            _maintenanceNotices.SetDisplayDateTo(displayDateTo);
            _maintenanceNotices.SetMaintenanceDateRangeFrom(maintenanceDateFrom);
            _maintenanceNotices.SetMaintenanceDateRangeTo(maintenanceDateTo);
            _maintenanceNotices.ClickOnEditDefaultMessageLink();
            _maintenanceNotices.ClickOnScheduleNotice();
            _maintenanceNotices.WaitForWorking();
            
        }

        private void VerifyThatDateRangeIsInCorrectFormat(string dateRange)
        {
            Regex.IsMatch(dateRange, @"^(0[1-9]|1[0-2])\/(0[1-9]|[1-2][0-9]|3[0-1])\/([0-9]{4})-(0[1-9]|1[0-2])\/(0[1-9]|[1-2][0-9]|3[0-1])\/([0-9]{4})$").ShouldBeTrue("The Date Range value '" + dateRange + "' is in format dd/mm/yyyy-dd/mm/yyyy");
        }
        private void VerifyThatDateTimeRangeIsInCorrectFormat(string dateTimeRange)
        {
            Regex.IsMatch(dateTimeRange, @"(0[1-9]|1[0-2])\/(0[1-9]|[1-2][0-9]|3[0-1])\/([0-9]{4})\s(1?[0-9]|2[0-3]):[0-5][0-9]-(0[1-9]|1[0-2])\/(0[1-9]|[1-2][0-9]|3[0-1])\/([0-9]{4})\s(1?[0-9]|2[0-3]):[0-5][0-9]$")
                .ShouldBeTrue("The Date Range value '" + dateTimeRange + "' is in format dd/mm/yyyy-dd/mm/yyyy");
        }

        //<First Name> <Last Name> 
        private void VerifyThatNameIsInCorrectFormat(string name)
        {
            Regex.IsMatch(name, @"^(\S+ )+\S+$").ShouldBeTrue("The Name '" + name + "' is in format XXX XXX ");
        }

        /// <summary>
        /// for upcoming notifications status should be A elseit can be C or I. A=active, C= cncelled , I = inactive, and edit and delete privileages should be accordingly
        /// </summary>
        /// <param name="dateRange"></param>
        /// <param name="status"></param>
        /// <param name="row"></param>
        /// <returns></returns>

        private void VerifyStatusForDaterange(string dateRange, string status,int row=1)
        {
            var date = dateRange.Split(new[] { "-" }, StringSplitOptions.None);
            var startdate = DateTime.Parse(date[0]);
            var enddate = DateTime.Parse(date[1]);
            if (DateTime.Now.Date >startdate.Date && DateTime.Now.Date > enddate.Date)
            {
                Console.WriteLine("Status expected value I , actual value " + status);
                (status.Equals("I")).ShouldBeTrue("Status expected value I , actual value " + status);
                if (status.Equals("I"))
                {
                    _maintenanceNotices.IsEditIconDisabled(row).ShouldBeTrue("Edit Icon is present and disabled");
                    _maintenanceNotices.IsDeleteIconDisabled(row).ShouldBeTrue("Delete Icon is present and disabled");
                }
            }
            else
            {
                Console.WriteLine("Status expected value either A or C, actual value " + status);
                (status.Equals("A") || status.Equals("C")).ShouldBeTrue("Status expected value to be either A or C, actual value " + status);
                if (status.Equals("A"))
                {
                    _maintenanceNotices.IsEditIconEnabled(row).ShouldBeTrue("Edit Icon is present and enabled");
                    _maintenanceNotices.IsDeleteIconEnabled(row).ShouldBeTrue("Delete Icon is present and enabled");
                }
                if (status.Equals("C"))
                {
                    _maintenanceNotices.IsEditIconDisabled(row).ShouldBeTrue("Edit Icon is present and disabled");
                    _maintenanceNotices.IsDeleteIconDisabled(row).ShouldBeTrue("Delete Icon is present and disabled");
                }


            }
        }

        private string AppendMT(string strSource, string strStart, string strEnd)
        {
            int Start, End;
            if (strSource.Contains(strStart) && strSource.Contains(strEnd))
            {
                Start = strSource.IndexOf(strStart, 0) + strStart.Length;
                End = strSource.IndexOf(strEnd, Start);
                return strSource.Substring(0, Start) + " MT " + strSource.Substring(End);
            }
            else
            {
                return "";
            }
        }
        private void VerifyEditValidationMessage(string title)
        {
            _maintenanceNotices.ClickOnSaveButton();
            _maintenanceNotices.GetPopupModalMessage().ShouldBeEqual(
                "Invalid or missing data must be resolved before the record can be saved.", title);
                _maintenanceNotices.ClickOnClosePopupModal();
        }

        private void VerfiyDisplayDateToolTip(string title)
        {
            _maintenanceNotices.GetToolTipOnInvalidInputOnDate(1).ShouldBeEqual(
                "Maintenance notices must be displayed for a minimum of three business days (excluding holidays).", title);
            _maintenanceNotices.GetToolTipOnInvalidInputOnDate(1,2).ShouldBeEqual(
                "Maintenance notices must be displayed for a minimum of three business days (excluding holidays).", title);
        }
        private void VerfiyMaintenanceDateToolTip(string title)
        {
            
                _maintenanceNotices.GetToolTipOnInvalidInputOnDate(2).ShouldBeEqual(
                "Maintenance Date and Time is required.", title);
                _maintenanceNotices.GetToolTipOnInvalidInputOnDate(2,2).ShouldBeEqual(
                    "Maintenance Date and Time is required.", title);
        }
        #endregion
    }
}

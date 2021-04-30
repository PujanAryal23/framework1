using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Nucleus.Service.Data;
using static System.String;
using Nucleus.Service.PageServices.Claim;
using Nucleus.Service.PageServices.PreAuthorization;
using Nucleus.Service.PageServices.Settings.User;
using Nucleus.Service.Support.Common.Constants;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Environment;
using Nucleus.UIAutomation.TestSuites.Base;
using Nucleus.UIAutomation.TestSuites.Common;
using NUnit.Framework;
using UIAutomation.Framework.Utils;
using Nucleus.Service.Support.Common;

namespace Nucleus.UIAutomation.TestSuites.TestCases.ClientUser
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class PreAuthActionClient
    {
        #region Old Sequential Code Commented Out


        /*#region PRIVATE FIELDS

        private PreAuthActionPage _preAuthAction;
        private PreAuthSearchPage _preAuthSearch;
        private PreAuthProcessingHistoryPage _preAuthProcessingHistory;
        private ClaimHistoryPage _claimHistory;
        private ProfileManagerPage _profileManager;
        private ClaimActionPage _claimAction;
        private string _authSeq;

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

        #region OVERRIDE METHODS

        protected override void ClassInit()
        {
            try
            {
                base.ClassInit();
                _preAuthSearch = QuickLaunch.NavigateToPreAuthSearch();
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
            if (_preAuthSearch.IsPageErrorPopupModalPresent())
                _preAuthSearch.ClosePageError();

            if (string.Compare(UserType.CurrentUserType, UserType.CLIENT, StringComparison.OrdinalIgnoreCase) != 0)
            {
                automatedBase.CurrentPage = QuickLaunch = _preAuthAction.Logout().LoginAsClientUser();
                _preAuthSearch = QuickLaunch.NavigateToPreAuthSearch();

            }
            else if (automatedBase.CurrentPage.GetPageHeader() != PageHeaderEnum.PreAuthSearch.GetStringValue())
            {
                automatedBase.CurrentPage.NavigateToPreAuthSearch();
                Console.Out.WriteLine("Navigate to Pre-Auth Search Page");
            }
            _preAuthSearch.GetSideBarPanelSearch.OpenSidebarPanel();
            _preAuthSearch.GetSideBarPanelSearch.ClickOnClearLink();
            _preAuthSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                PreAuthQuickSearchEnum.AllPreAuths.GetStringValue());
            base.TestCleanUp();
        }

        protected override void ClassCleanUp()
        {

            try
            {
                _preAuthAction.CloseDatabaseConnection();
                if (automatedBase.CurrentPage.IsQuickLaunchIconPresent())
                    _preAuthAction.ClickOnQuickLaunch();

            }
            finally
            {
                base.ClassCleanUp();
            }

        }

        #endregion*/

        #endregion

        protected string FullyQualifiedClassName
        {
            get
            {
                return GetType().FullName;
            }
        }

        #region TEST SUITES

        [Test] //CAR-2440(CAR-2638)
        public void Verify_create_and_manage_logic_on_flag_line_client()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                PreAuthActionPage _preAuthAction;
                PreAuthSearchPage _preAuthSearch;

                _preAuthSearch = automatedBase.CurrentPage.NavigateToPreAuthSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var authSeq = paramLists["PreAuth"];

                StringFormatter.PrintMessageTitle("Delete Logic From Database");
                _preAuthSearch.DeleteLogicFromDatabase(authSeq);

                _preAuthAction = _preAuthSearch.SearchByAuthSequenceAndNavigateToAuthAction(authSeq);

                StringFormatter.PrintMessageTitle("Create New Logic");
                _preAuthAction.IsLogicPlusIconDisplayed(1, 1)
                    .ShouldBeTrue("Lplus Icon should be displayed for No Logic");
                _preAuthAction.ClickOnAddLogicIconByRow(1, 1);
                _preAuthAction.IsLogicFormTextPresent("Create Logic")
                    .ShouldBeTrue("Create Logic form should be displayed");

                StringFormatter.PrintMessage("Verification that text is required to start conversation");
                _preAuthAction.ClickSaveButton();
                _preAuthAction.IsPageErrorPopupModalPresent()
                    .ShouldBeTrue("Error message pops up when Logic is created without entering any text");
                _preAuthAction.GetPageErrorMessage().ShouldBeEqual("Enter a message to create Logic request.",
                    "Popup Error for Empty Message Reply");
                _preAuthAction.ClosePageError();

                StringFormatter.PrintMessage("Verification that user can type upto 500 characters");
                _preAuthAction.AddLogicMessageTextarea(Concat(Enumerable.Repeat("test ", 101)));
                _preAuthAction.GetLogicMessageTextarea().Length
                    .ShouldBeEqual(500, "User can free text up to 500 characters");

                StringFormatter.PrintMessage("Creation of logic");
                _preAuthAction.AddLogicMessageTextarea("Test Logic From Client User");
                _preAuthAction.ClickSaveButton();

                StringFormatter.PrintMessage("Validation after logic creation");
                var actualLogicMessage =
                    Regex.Split(_preAuthAction.GetRecentRightLogicMessageByRow(1), "\r\n").ToList();
                actualLogicMessage[0].ShouldBeEqual(_preAuthAction.GetLoggedInUserFullName(),
                    "Reply User Full Name should be same");
                actualLogicMessage[1].IsDateTimeWithoutSecInFormat()
                    .ShouldBeTrue("Is Date Time is in Correct Format?");
                actualLogicMessage[2].ShouldBeEqual("Test Logic From Client User", "Is Logic Message Same?");
                _preAuthAction.GetSideWindow.GetValueByLabel("Status").ShouldBeEqual(
                    LogicStatusEnum.Open.GetStringValue(),
                    $"Status should be {LogicStatusEnum.Open.GetStringValue()} when logic is created");
                _preAuthAction.GetSideWindow.GetValueByLabel("Assigned to").ShouldBeEqual(
                    "Cotiviti", $"Assigned to should be Cotiviti");

                StringFormatter.PrintMessageTitle("Verification of Cancel Button");
                VerificationOfLogicCancelFunctionality(_preAuthAction, "Cancel Test Logic Reply From Internal User", "Cotiviti");

                #region Verification of reply and close functionality

                StringFormatter.PrintMessageTitle(
                    "Verification of Reply and Close functionalities for Internal user");
                _preAuthAction.Logout().LoginAsHciAdminUser().NavigateToPreAuthSearch()
                    .SearchByAuthSequenceAndNavigateToAuthAction(authSeq, false);
                _preAuthAction.IsInternalLogicIconByRowPresent(1, 1)
                    .ShouldBeTrue("Logic Icon should be Assigned to Internal - filled state");
                //_preAuthAction.ClickOnLogicIcon(1, 1, "HCIUSER");

                StringFormatter.PrintMessage("Verification of presence of buttons, form and values");
                _preAuthAction.GetPrimaryButtonText()
                    .ShouldBeEqual("Reply", "Is 'Reply' Button Present?");
                _preAuthAction.GetSideWindow.GetSecondaryButtonName()
                    .ShouldBeEqual("Close Logic", "Is 'Close Logic' Button Present?");
                _preAuthAction.IsLogicFormTextPresent("Reply To Logic")
                    .ShouldBeTrue("Create Logic form should be displayed");
                _preAuthAction.GetSideWindow.GetValueByLabel("Assigned to").ShouldBeEqual(
                    "Cotiviti", $"Assigned to should be Cotiviti");
                _preAuthAction.GetSideWindow.GetValueByLabel("Status").ShouldBeEqual(
                    LogicStatusEnum.Open.GetStringValue(),
                    $"Status should be {LogicStatusEnum.Open.GetStringValue()} when logic is created");

                StringFormatter.PrintMessage("Verification that text is required to reply logic");
                _preAuthAction.ClickSaveButton();
                _preAuthAction.IsPageErrorPopupModalPresent()
                    .ShouldBeTrue("Error message pops up when logic is replied with empty text");
                _preAuthAction.GetPageErrorMessage().ShouldBeEqual("Enter a message to create Logic request.",
                    "Popup Error for Empty Message Reply");
                _preAuthAction.ClosePageError();

                StringFormatter.PrintMessage("Verification that 500 characters are allowed to reply a logic");
                _preAuthAction.AddLogicMessageTextarea(Concat(Enumerable.Repeat("TEST ", 101)));
                _preAuthAction.GetLogicMessageTextarea().Length
                    .ShouldBeEqual(500, "User can free text up to 500 characters");

                StringFormatter.PrintMessageTitle("Verification of Cancel Button");
                VerificationOfLogicCancelFunctionality(_preAuthAction, "Cancel Test Logic Reply From Cotiviti User", "Cotiviti");

                StringFormatter.PrintMessageTitle("Verification that text from other user will be left justified");
                _preAuthAction.AddLogicMessageTextarea("Logic Reply From Cotiviti User");
                _preAuthAction.ClickSaveButton();
                var actualLeftLogicMessage =
                    Regex.Split(_preAuthAction.GetRecentLeftLogicMessage(), "\r\n").ToList();
                actualLeftLogicMessage.ShouldBeEqual(actualLogicMessage,
                    "Other User Type Logic Should be on left side");

                StringFormatter.PrintMessage("Verification that text from same user will be right justified");
                actualLogicMessage =
                    Regex.Split(_preAuthAction.GetRecentRightLogicMessageByRow(2), "\r\n").ToList();
                actualLogicMessage[0].ShouldBeEqual(_preAuthAction.GetLoggedInUserFullName(),
                    "Reply User Full Name should be same");
                actualLogicMessage[1].IsDateTimeWithoutSecInFormat()
                    .ShouldBeTrue("Is Date Time is in Correct Format?");
                actualLogicMessage[2].ShouldBeEqual($"Logic Reply From Cotiviti User", "Is Logic Message Same?");
                _preAuthAction.GetSideWindow.GetValueByLabel("Status").ShouldBeEqual(
                    LogicStatusEnum.Open.GetStringValue(),
                    $"Status should be {LogicStatusEnum.Open.GetStringValue()} when logic is created");

                StringFormatter.PrintMessage(
                    "Verification that user will be able to add another message and reply");
                _preAuthAction.AddLogicMessageTextarea("Additional Logic Reply From Cotiviti User");
                _preAuthAction.ClickSaveButton();
                actualLeftLogicMessage =
                    Regex.Split(_preAuthAction.GetRecentRightLogicMessageByRow(3), "\r\n").ToList();
                actualLeftLogicMessage[2].ShouldBeEqual("Additional Logic Reply From Cotiviti User",
                    "User can add another message and reply");

                StringFormatter.PrintMessage("Verification of Close functionality");
                _preAuthAction.GetSideWindow.ClickOnSecondaryButton();
                _preAuthAction.IsPageErrorPopupModalPresent()
                    .ShouldBeFalse("User able to Close Logic without message");
                _preAuthAction.IsLogicWindowDisplayByRowPresent(1, 1)
                    .ShouldBeTrue("Logic Form Should still be open when logic is replied");
                _preAuthAction.GetSideWindow.GetValueByLabel("Status").ShouldBeEqual(
                    LogicStatusEnum.Closed.GetStringValue(),
                    $"Status should be {LogicStatusEnum.Closed.GetStringValue()} when logic is closed");
                _preAuthAction.GetSideWindow.GetValueByLabel("Assigned to").ShouldBeEqual(
                    "Client", "Assigned to should be remain unchanged when the logic is closed");

                #endregion

                StringFormatter.PrintMessage("Verification that user will be able to reopen the logic");
                _preAuthAction.AddLogicMessageTextarea("New Logic Reply From Client User");
                _preAuthAction.ClickSaveButton();
                _preAuthAction.GetSideWindow.GetValueByLabel("Assigned to").ShouldBeEqual(
                    "Client", "Assigned to should be Client");
                _preAuthAction.GetSideWindow.GetValueByLabel("Status").ShouldBeEqual(
                    LogicStatusEnum.Open.GetStringValue(),
                    $"Status should be {LogicStatusEnum.Closed.GetStringValue()} when logic is closed");
                _preAuthAction.ClickCancel();
            }
        }

        [Test] //CAR-1393(CAR-1515)
        public void Verify_flag_color_functionality()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                PreAuthActionPage _preAuthAction;
                PreAuthSearchPage _preAuthSearch;

                _preAuthSearch = automatedBase.CurrentPage.NavigateToPreAuthSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;

                var _authSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName,
                    "AuthSequence", "Value");
                _preAuthAction = _preAuthSearch.SearchByAuthSequenceAndNavigateToAuthAction(_authSeq, false);
                _preAuthAction.GetBoldOrNormalColorFlagList().ShouldCollectionBeEquivalent(
                    _preAuthAction.GetFlagListByClientdone(_authSeq), "Is Bold Red Color display for CLIDONE='F'");
                _preAuthAction.GetBoldOrNormalColorFlagList(false).ShouldCollectionBeEquivalent(
                    _preAuthAction.GetFlagListByClientdone(_authSeq, true),
                    "Is Non Bold Red Color display for CLIDONE='T'");
            }
        }

        [Test] //CAR - 268(CAR-1706)
        public void Verify_Lock_Funtionality_Client()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                PreAuthActionPage _preAuthAction;
                PreAuthSearchPage _preAuthSearch;

                _preAuthSearch = automatedBase.CurrentPage.NavigateToPreAuthSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var lockedAuthSequence = paramLists["LockedAuthSequence"];
                var unlockedAuthSequence = paramLists["UnlockedAuthSequence"];
                var providerSequence = paramLists["ProviderSequence"];
                try
                {
                    _preAuthSearch.UpdateStatusOfAuthSeqFromDatabase("V", unlockedAuthSequence);
                    _preAuthSearch.DeleteLockByPreAuthSeq(lockedAuthSequence);
                    _preAuthSearch.Logout().LoginAsClientUser1().NavigateToPreAuthSearch();
                    automatedBase.CurrentPage = _preAuthAction =
                        _preAuthSearch.SearchByAuthSequenceAndNavigateToAuthAction(lockedAuthSequence, false);
                    _preAuthAction.IsLockIconPresent().ShouldBeFalse("Is Lock icon present?");

                    var lockedby = _preAuthAction.GetLoggedInUserFullName();
                    var pageUrl = automatedBase.CurrentPage.CurrentPageUrl;

                    StringFormatter.PrintMessage("Logging in from another user to verify lock in Pre-Auth Search page");

                    lockedby.DoesNameContainsOnlyFirstWithLastname()
                        .ShouldBeTrue("Does Name contain first name and last name?");
                    _preAuthSearch.IsLockIconPresentAndGetLockIconToolTipMessage(lockedAuthSequence, pageUrl)
                        .ShouldBeEqual($"Pre-Auth is currently locked by {lockedby}");

                    StringFormatter.PrintMessage("Verification of Lock funtionalities in Pre-Auth Action page");

                    _preAuthAction = _preAuthSearch.NaviateToPreAuthActionPage(1, 2, false, false);
                    _preAuthAction.CloseCurrentWindowAndSwitchToOriginal(1);
                    _preAuthAction.IsLockIconPresent().ShouldBeTrue("Lock icon should be present");
                    _preAuthAction.GetToolTipOfLockIcon().ShouldBeEqual(
                        $"Pre-Auth is opened in view mode. It is currently locked by {lockedby}");
                    _preAuthAction.IsApproveIconEnabled().ShouldBeFalse("Is Approve icon enabled?");
                    _preAuthAction.IsTransferIconDisabled().ShouldBeTrue("Is transfer icon disabled?");
                    _preAuthAction.IsEditFlagIconDisabled().ShouldBeTrue("Is Edit flag icon disabled?");
                    //_preAuthAction.IsEditDentalRecordIconDisabled().ShouldBeTrue("Is Edit Dental Record icon disabled");
                    _preAuthSearch = _preAuthAction.ClickOnReturnToPreAuthSearch();
                    _preAuthSearch.WaitForPageToLoadWithSideBarPanel();
                    _preAuthSearch.GetSideBarPanelSearch.ClickOnClearLink();
                    _preAuthSearch.SetProviderSeq(providerSequence);
                    _preAuthSearch.ClickOnFindButton();
                    _preAuthAction.WaitForPageToLoadWithSideBarPanel();

                    StringFormatter.PrintMessage(
                        "Verification of locked auth sequence being skipped on clicking next icon");

                    int lockedAuthSeqCount = _preAuthSearch.GetCountOfLockedAuthSequences();
                    int unlockedAuthSeqCount =
                        (_preAuthSearch.GetGridViewSection.GetGridRowCount()) - lockedAuthSeqCount;
                    var lockExcludedPreAuthList =
                        _preAuthSearch.GetSearchResultUptoRow(unlockedAuthSeqCount, true).Keys;

                    _preAuthAction = _preAuthSearch.ClickOnFirstUnlockedPreAuthToNavigateToPreAuthActionPage();
                    _preAuthAction.CloseCurrentWindowAndSwitchToOriginal(1);

                    var nextAuthSeqList = ClickNextAndGetAuthSeqList(_preAuthAction, 1, unlockedAuthSeqCount - 1);

                    nextAuthSeqList.ShouldCollectionBeEqual(lockExcludedPreAuthList,
                        "When users select the Next button on the pre-auth, locked pre-auths will be skipped");
                    _preAuthAction.ClickOnNextIcon();
                    _preAuthAction.WaitForPageToLoadWithSideBarPanel();
                    _preAuthSearch.GetPageHeader().ShouldBeEqual(PageHeaderEnum.PreAuthSearch.GetStringValue(),
                        "Page should direct to PreAuth Search page");

                    StringFormatter.PrintMessage(
                        "Verification that locked auth sequence gets skipped on clicking approve icon");

                    _preAuthSearch.ClickOnFilterOptionListRow(1);

                    var resultList = _preAuthSearch
                        .GetSearchResultUptoRow(_preAuthSearch.GetGridViewSection.GetGridRowCount()).Keys.ToList();

                    _preAuthSearch.ClickLastUnlockedAuthSeq(1);
                    _preAuthAction.ClickOnApproveIcon();
                    _preAuthAction.WaitForStaticTime(3000);
                    _preAuthAction.CloseCurrentWindowAndSwitchToOriginal(1);
                    _preAuthAction.GetPreAuthDetailValueByTitle("Auth Seq").ShouldBeEqual(resultList[1],
                        "On clicking approve button the user is navigated to top unlocked auth seq");
                    _preAuthAction.ClickOnReturnToPreAuthSearch();
                    _preAuthAction.WaitForPageToLoadWithSideBarPanel();
                    _preAuthSearch.GetPageHeader().ShouldBeEqual(PageHeaderEnum.PreAuthSearch.GetStringValue(),
                        "Page should direct to PreAuth Search page");
                }

                finally
                {
                    _preAuthSearch.UpdateStatusOfAuthSeqFromDatabase("U", unlockedAuthSequence);
                    _preAuthSearch.DeleteLockByPreAuthSeq(lockedAuthSequence);
                    _preAuthSearch.CloseCurrentWindowAndSwitchToOriginal(0);
                    automatedBase.CurrentPage.NavigateToPreAuthSearch();
                    automatedBase.CurrentPage.RefreshPage();
                }
            }
        }

        [Test] // CAR-1089
        public void Verify_Approve_Functionality_Client()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                PreAuthActionPage _preAuthAction;
                PreAuthSearchPage _preAuthSearch;

                _preAuthSearch = automatedBase.CurrentPage.NavigateToPreAuthSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var clientUnreviewedAuthSequence = paramLists["ClientUnreviewedAuthSequence"];
                var cotivitiUnreviewedAuthSequence = paramLists["CotivitiUnreviewedAuthSequence"];
                var providerSequence = paramLists["ProviderSequence"];
                var processingHistoryPageHeader = automatedBase.DataHelper
                    .GetMappingData(FullyQualifiedClassName, "processingHistoryHeader").Values.ToList();
                try
                {
                    _preAuthSearch.UpdateStatusOfAuthSeqFromDatabase("V", clientUnreviewedAuthSequence);
                    _preAuthSearch.UpdateHciDoneOrClientDoneToFalseFromDatabase("clidone",
                        clientUnreviewedAuthSequence);

                    StringFormatter.PrintMessage(
                        "Verification that approve icon is disabled for preauth seq with status Cotiviti Unreviewed");
                    _preAuthAction =
                        _preAuthSearch.SearchByAuthSequenceAndNavigateToAuthAction(cotivitiUnreviewedAuthSequence);
                    _preAuthAction.GetPreAuthDetailValueByTitle("Status").ShouldBeEqual(
                        StatusEnum.CotivitiUnreviewed.GetStringDisplayValue(), "Status should be Cotiviti Unreviewed");
                    _preAuthAction.IsApproveIconEnabled().ShouldBeFalse("Is Approve icon enabled?");

                    StringFormatter.PrintMessage(
                        "Verification of user being able to use filter and sort options, approve icon being enabled for status Client unreviewed and user should be prevented from double clicking the icon ");

                    _preAuthAction.NavigateToPreAuthSearch().GetSideBarPanelSearch.ClickOnClearLink();
                    _preAuthSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Provider Sequence", providerSequence);
                    _preAuthSearch.GetSideBarPanelSearch.ClickOnFindButton();
                    _preAuthSearch.WaitForWorkingAjaxMessage();
                    var authSeqResultList = _preAuthAction.GetGridViewSection.GetGridListValueByCol();

                    _preAuthSearch.ClickOnFilterOptionListRow(1);
                    _preAuthSearch.IsListStringSortedInAscendingOrder(2)
                        .ShouldBeTrue("Is list sorted in ascending order after sorting it by auth seq?");
                    _preAuthSearch.ClickOnFilterOptionListRow(1);

                    var username = _preAuthAction.GetLoggedInUserFullName();
                    _preAuthAction = _preAuthSearch.NaviateToPreAuthActionPage(2, 2);
                    var previousStatusOfPreAuthSeq = _preAuthAction.GetPreAuthDetailValueByTitle("Status");

                    _preAuthAction.GetPreAuthDetailValueByTitle("Auth Seq").ShouldBeEqual(authSeqResultList[1],
                        "Auth seq should be displayed in order as displayed in search result");
                    previousStatusOfPreAuthSeq.ShouldBeEqual(StatusEnum.ClientUnreviewed.GetStringDisplayValue());
                    _preAuthAction.IsApproveIconEnabled().ShouldBeTrue("Is Approve icon enabled?");
                    _preAuthAction.ClickApproveAndCheckIfApproveIconIsDisabled()
                        .ShouldBeTrue("Is Approve icon disabled?");

                    StringFormatter.PrintMessage(
                        "Verification that user will be moved to first unlocked pre-auth at the top of the list id there are additional record");

                    _preAuthAction.GetPreAuthDetailValueByTitle("Auth Seq").ShouldBeEqual(authSeqResultList[0],
                        "Auth seq should be displayed in order as displayed in search result");
                    _preAuthAction.GetPreAuthDetailValueByTitle("Status")
                        .ShouldBeEqual(StatusEnum.ClientUnreviewed.GetStringDisplayValue());
                    _preAuthAction.IsApproveIconEnabled().ShouldBeTrue("Is Approve icon enabled?");

                    StringFormatter.PrintMessage(
                        "Verification that Pre-Auth Status will be closed when there are no flags that require Client Review and User will be navigated back to Pre-Auth Search page when there are no additional records");

                    _preAuthAction.ClickOnApproveIcon();
                    _preAuthAction.WaitForPageToLoadWithSideBarPanel();
                    _preAuthSearch.GetPageHeader().ShouldBeEqual(PageHeaderEnum.PreAuthSearch.GetStringValue(),
                        "When there is no additional records in the list, user will be navigated back to Pre-Auth search page");

                    StringFormatter.PrintMessage(
                        "Verification when approve icon is selected, CLIDONE = T, status will update to Closed and approve icon is disabled for Closed auth seq");

                    _preAuthAction.NavigateToPreAuthSearch()
                        .SearchByAuthSequenceAndNavigateToAuthAction(
                            clientUnreviewedAuthSequence.Split(',').ToList()[0]);

                    var status = _preAuthAction.GetPreAuthDetailValueByTitle("Status");

                    _preAuthAction.IsAllCliDONETrue(clientUnreviewedAuthSequence)
                        .ShouldBeTrue("Are all CliDone set to true after approving the preauthseq?");
                    _preAuthAction.GetPreAuthDetailValueByTitle("Status").ShouldBeEqual(
                        StatusEnum.Closed.GetStringDisplayValue(),
                        $"Status should be {StatusEnum.Closed.GetStringDisplayValue()} when approved by client ");
                    _preAuthAction.IsApproveIconEnabled().ShouldBeFalse("Is approve icon enabled?");

                    StringFormatter.PrintMessage(
                        "Verification when approve icon is selected, a pre-auth record with Date, Username, status and notes will be displayed");

                    PreAuthProcessingHistoryPage preAuthProcessingHistory =
                        _preAuthAction.ClickOnPreAuthProcessingHistoryAndSwitch();
                    preAuthProcessingHistory.GetPreAuthHistoryAuthSeq()
                        .ShouldBeEqual(clientUnreviewedAuthSequence.Split(',').ToList()[0],
                            "Auth sequences should match");
                    preAuthProcessingHistory.GetPreAuthProcessingHistoryHeader()
                        .ShouldCollectionBeEqual(processingHistoryPageHeader, "Column headers should match");
                    preAuthProcessingHistory.GetPreAuthProcessingHistoryModifiedDate(1, 1)
                        .ShouldBeEqual(DateTime.Now.Date.ToString("MM/dd/yyyy"), "Dates should match");
                    preAuthProcessingHistory.GetPreAuthProcessingHistoryGridValueByRowCol(1, 2)
                        .ShouldBeEqual(username, "Username should match");
                    preAuthProcessingHistory.GetPreAuthProcessingHistoryGridValueByRowCol(1, 3)
                        .ShouldBeEqual(status, "Changed status should match");
                    preAuthProcessingHistory.GetPreAuthProcessingHistoryGridValueByRowCol(1, 4).ShouldBeEqual(
                        $"Previous status = {previousStatusOfPreAuthSeq}", "Notes should match");
                }

                finally
                {
                    _preAuthSearch.UpdateStatusOfAuthSeqFromDatabase("V", clientUnreviewedAuthSequence);
                    _preAuthSearch.UpdateHciDoneOrClientDoneToFalseFromDatabase("clidone", clientUnreviewedAuthSequence);
                }
            }

        }

        [Test] //CAR-1516(CAR-1394) ,CAR-1590
        public void Verify_Flag_Audit_History_specifications_client()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                PreAuthActionPage _preAuthAction;
                PreAuthSearchPage _preAuthSearch;

                _preAuthSearch = automatedBase.CurrentPage.NavigateToPreAuthSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var lineNumberList = paramLists["LineNumber"].Split(',').ToList();
                var flagList = paramLists["Flag"].Split(',').ToList();
                var labelsList = automatedBase.DataHelper
                    .GetMappingData(FullyQualifiedClassName, "labels").Values.ToList();
                var authSequence = paramLists["AuthSequence"];
                var authSequenceWithAudit = paramLists["AuthSequenceWithAudit"];

                _preAuthAction = _preAuthSearch.SearchByAuthSequenceAndNavigateToAuthAction(authSequenceWithAudit);

                try
                {

                    _preAuthSearch.DeleteFlagAuditHistoryFromDatabaseByPreAuthSeq(authSequence);

                    StringFormatter.PrintMessage("Verification for auth id with flag audit history record and modification date");
                    _preAuthAction.GetHeaderOfBottomRightSection()
                        .ShouldBeEqual("Line Items", "Lines items should be displayed");
                    _preAuthAction.IsLineItemOrFlagAuditHistoryIconPresent("Flag Audit History")
                        .ShouldBeTrue("Is Flag Audit History Icon displayed?");
                    _preAuthAction.ClickOnLineItemOrFlagAuditHistoryIconByTitle("Flag Audit History");
                    _preAuthAction.GetHeaderOfBottomRightSection()
                        .ShouldBeEqual("Flag Audit History", "Flag Audit History should be displayed");
                    _preAuthAction.GetFlagAuditRecordCount()
                        .ShouldBeGreater(0, "Flag audit history records should be displayed");
                    _preAuthAction.GetLinenumberOrFlagFromFlagAuditRecord(1, "FlaggedLines")
                        .ShouldCollectionBeEqual(lineNumberList, "Line numbers should match");
                    _preAuthAction.DoesLineNumberStartsWith1(1, "FlaggedLines")
                        .ShouldBeTrue("Does Line number should start with 1?");
                    _preAuthAction.IsLineNumberOfFlagAuditHistorySorted("FlaggedLines")
                        .ShouldBeTrue("Line Numbers should be in ascending order");
                    _preAuthAction.ScrollToLastDiv("FlaggedLines");
                    _preAuthAction.GetLinenumberOrFlagFromFlagAuditRecord(2, "FlaggedLines")
                        .ShouldCollectionBeEqual(flagList, "Flags should be present");
                    _preAuthAction.GetFlagAuditHistoryActionCount(1)
                        .ShouldBeGreater(0, "Audit actions should be present");
                    _preAuthAction.IsFlagAuditHistoryModDateSorted(1)
                        .ShouldBeTrue("Are Actions should be shown in descending order by Mod Date?");

                    StringFormatter.PrintMessage("Verification for no audit records for flag audit history");
                    _preAuthAction.NavigateToPreAuthSearch().SearchByAuthSequenceAndNavigateToAuthAction(authSequence);
                    _preAuthAction.ClickOnLineItemOrFlagAuditHistoryIconByTitle("Flag Audit History");
                    _preAuthAction.IsFlagAuditsPresent().ShouldBeFalse("Flag audit records should not be present");
                    _preAuthAction.GetNoFlagAuditRecordMessage().ShouldBeEqual(
                        "No flag audit records exist for this pre-auth.", "No audit record message should match");


                    #region CAR-1590

                    StringFormatter.PrintMessage(
                        "This is to verify that on clearing the notes, the notes will be deleted as expected");
                    var notes = "This is for testing on clearing the notes entered the notes will clear";

                    EditFlag(_preAuthAction, "1", "1", 3, "", notes, notes: true, istitle: false);

                    _preAuthAction.GetSideWindow.ClearIFrame("Note");
                    _preAuthAction.ClickSaveButton();
                    _preAuthAction.WaitForWorking();
                    _preAuthAction.ClickOnLineItemOrFlagAuditHistoryIconByTitle("Line Items");
                    _preAuthAction.ClickOnLineItemOrFlagAuditHistoryIconByTitle("Flag Audit History");
                    _preAuthAction.GetFlagAuditHistoryValue(1, 2, 3, 1)
                        .ShouldBeNullorEmpty("On clearing the notes, label note should not display anything");

                    #endregion

                    StringFormatter.PrintMessage(
                        "Verification of labels and formats of record shown in flag audit history and action Delete");
                    var notesValue = "Delete";
                    EditFlag(_preAuthAction, "1", "1", 3, notesValue, notesValue, client: false, notes: true);
                    var reasonCode = _preAuthAction.GetSideWindow.GetDropDownListDefaultValue("Reason Code");

                    _preAuthAction.ClickSaveButton();
                    _preAuthAction.WaitForWorking();
                    _preAuthAction.ClickOnLineItemOrFlagAuditHistoryIconByTitle("Line Items");
                    _preAuthAction.ClickOnLineItemOrFlagAuditHistoryIconByTitle("Flag Audit History");
                    _preAuthAction.GetFlagAuditHistoryRecordLabels(1, 2)
                        .ShouldCollectionBeEqual(labelsList, "Labels should match");
                    labelsList.Contains("Client Display:").ShouldBeFalse("Is Client Display: present?");
                    _preAuthAction.IsModDateFormatCorrect(1, 2, 1, 1)
                        .ShouldBeTrue("Is Date should be in MM/DD/YYYY H:MM:SS AM/PM format?");
                    _preAuthAction.GetFlagAuditHistoryValue(1, 2, 1, 2).DoesNameContainsOnlyFirstWithLastname()
                        .ShouldBeTrue("Does name contain of Firstname and lastname?");
                    _preAuthAction.GetFlagAuditHistoryValue(1, 2, 2, 2)
                        .ShouldBeEqual(reasonCode, "Reason code should match");
                    _preAuthAction.GetFlagAuditHistoryValue(1, 2, 2, 1)
                        .ShouldBeEqual("Delete", "Action performed should be shown");
                    _preAuthAction.GetFlagAuditHistoryValue(1, 2, 3, 1)
                        .ShouldBeEqual(notesValue, "Notes entered should match");

                    StringFormatter.PrintMessage(
                        "Verification of client display being Yes whent visible to client checkbox is checked");
                    var inputNotesValue = "client test";

                    EditFlag(_preAuthAction, "1", "1", 3, "Restore", inputNotesValue, client: false, notes: true);
                    _preAuthAction.ClickSaveButton();
                    _preAuthAction.WaitForWorking();
                    _preAuthAction.GetFlagAuditHistoryValue(1, 2, 3, 1)
                        .ShouldBeEqual(inputNotesValue, "Notes entered should match");

                    StringFormatter.PrintMessage("Verification of Action 'Note'");
                    EditFlag(_preAuthAction, "1", "1", 3, "", "This is for validation of action Note", client: false, notes: true, istitle: false);
                    _preAuthAction.ClickSaveButton();
                    _preAuthAction.WaitForWorking();
                    _preAuthAction.GetFlagAuditHistoryValue(1, 2, 2, 1)
                        .ShouldBeEqual("Note", "Action performed should be shown");

                    StringFormatter.PrintMessage("To verify user can toggle back to line items");
                    _preAuthAction.IsLineItemOrFlagAuditHistoryIconPresent("Line Items")
                        .ShouldBeTrue("Is Line items icon should be present on the left of Flag audit history icon?");
                    _preAuthAction.ClickOnLineItemOrFlagAuditHistoryIconByTitle("Line Items");
                    _preAuthAction.GetHeaderOfBottomRightSection()
                        .ShouldBeEqual("Line Items", "Lines items should be displayed");
                }

                finally
                {
                    //This is to handle if the test gets aborted before the flag has been restored, delete icon will get disabled at this condition
                    // and the test begins with flag deletion which leads to failure in case the delete icon is disabled
                    _preAuthAction.ClickOnEditFlagIconByLineNoAndRow();
                    var isIconDisabled = _preAuthAction.IsDeleteRestoreIconDisabled("Delete");
                    if (isIconDisabled)
                    {
                        _preAuthAction.SelectReasonCode(3);
                        _preAuthAction.ClickOnDeleteOrRestoreIcon("Restore");
                        _preAuthAction.ClickSaveButton();
                    }

                    _preAuthAction.NavigateToPreAuthSearch();
                }
            }
        }

        [Test] // CAR-915 (CAR-1387) + CAR-2888(CAR-2870) + CAR-CAR-2887(CAR-2846)
        public void Verify_Pre_Auth_Action_Upper_Left_Quadrant()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                PreAuthActionPage _preAuthAction;
                PreAuthSearchPage _preAuthSearch;

                _preAuthSearch = automatedBase.CurrentPage.NavigateToPreAuthSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var _authSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "AuthSequence", "Value");
                _preAuthAction = _preAuthSearch.SearchByAuthSequenceAndNavigateToAuthAction(_authSeq);
                var valuesFromDB = _preAuthAction.GetUpperLeftQuadrantValuesFromDB(_authSeq);
                var preAuthSeq = valuesFromDB[0];
                var reviewType = valuesFromDB[1];
                var patientName = valuesFromDB[2];
                var patSeq = valuesFromDB[3];
                var patDOB = valuesFromDB[4];
                var prvSeq = valuesFromDB[5];
                var prvName = valuesFromDB[6];
                var TIN = valuesFromDB[7];
                var state = valuesFromDB[8];
                var groupID = valuesFromDB[9];
                var planID = valuesFromDB[10];
                var patNum = valuesFromDB[11];
                var preAuthID = valuesFromDB[12];
                var imageId = valuesFromDB[13];

                StringFormatter.PrintMessageTitle("Verify Upper Left Quadrant Data Values");
                _preAuthAction.GetUpperLeftQuadrantValueByLabel("Auth Seq").ShouldBeEqual(preAuthSeq, "Auth Seq");
                _preAuthAction.GetUpperLeftQuadrantValueByLabel("Review Type").ShouldBeEqual(reviewType, "Auth Seq");
                _preAuthAction.GetUpperLeftQuadrantValueByLabel("Patient Name").ShouldBeEqual(patientName, "Auth Seq");
                _preAuthAction.GetUpperLeftQuadrantValueByLabel("Patient Seq").ShouldBeEqual(patSeq, "Patient Seq");
                _preAuthAction.GetUpperLeftQuadrantValueByLabel("DOB").ShouldBeEqual(patDOB, "DOB");
                _preAuthAction.GetUpperLeftQuadrantValueByLabel("Status").ShouldBeEqual("New", "Status");
                _preAuthAction.GetUpperLeftQuadrantValueByLabel("Provider Seq").ShouldBeEqual(prvSeq, "Provider Seq");
                _preAuthAction.GetUpperLeftQuadrantValueByLabel("Provider Name")
                    .ShouldBeEqual(prvName, "Provider Name");
                _preAuthAction.GetUpperLeftQuadrantValueByLabel("TIN").ShouldBeEqual(TIN, "TIN");
                _preAuthAction.GetUpperLeftQuadrantValueByLabel("State").ShouldBeEqual(state, "State");
                _preAuthAction.GetUpperLeftQuadrantValueByLabel("Group").ShouldBeEqual(groupID, "Group");
                _preAuthAction.GetUpperLeftQuadrantValueByLabel("Plan").ShouldBeEqual(planID, "Plan");
                _preAuthAction.GetUpperLeftQuadrantValueByLabel("Pat No").ShouldBeEqual(patNum, "Pat No");
                _preAuthAction.GetUpperLeftQuadrantValueByLabel("Pre-Auth ID").ShouldBeEqual(preAuthID, "Pre-Auth ID");
                _preAuthAction.GetUpperLeftQuadrantValueByLabel("Image ID").ShouldBeEqual(imageId, "Image ID");
            }
        }

        [Test] //CAR-1431(CAR-1351)
        public void Verify_flagged_line_data_points()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                PreAuthActionPage _preAuthAction;
                PreAuthSearchPage _preAuthSearch;

                _preAuthSearch = automatedBase.CurrentPage.NavigateToPreAuthSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var authSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "AuthSequence", "Value");
                var authSeqWithTriggerInfo = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName,
                    "AuthSequenceWithTriggerInfo", "Value");
                _preAuthSearch.DeleteFlagAndAuditByAuthSeqFlagsLinNo(authSeq, "'DELE'", "2");
                _preAuthSearch.DeleteFlagAndAuditByAuthSeqFlagsLinNo(authSeq, "'DELE'", "3");
                _preAuthAction = _preAuthSearch.SearchByAuthSequenceAndNavigateToAuthAction(authSeq);

                _preAuthAction.IsFlaggedLineRowDivPresentByLineNo(1)
                    .ShouldBeTrue("Is Flag Div Row Present for having flag?");
                _preAuthAction.IsFlaggedLineRowDivPresentByLineNo(3)
                    .ShouldBeFalse("Is Flag Div Row Present for not having flag?");

                _preAuthAction.GetFlagListByDivList(isInternalUser: false)
                    .ShouldCollectionBeEqual(_preAuthAction.GetClientFlagList(authSeq),
                        "Is Flag List Equals?");

                _preAuthAction.IsApproveIconPresent().ShouldBeTrue("Is Approve Icon Display?");
                //_preAuthAction.IsTransferIconPresent().ShouldBeTrue("Is Transfer Approve Icon Display?");
                _preAuthAction.IsHistoryIconPresent().ShouldBeTrue("Is History Icon Display?");
                _preAuthAction.IsNextIconPresent().ShouldBeTrue("Is Next Icon Display?");

                _preAuthAction.GetFlagLineNumberList().IsInAscendingOrder()
                    .ShouldBeTrue("Flagged Line should display ascending by line number");

                _preAuthAction.GetFlagLineLevelHeaderDetailValue(col: 2).IsDateInFormat()
                    .ShouldBeTrue("Is DOS is in Date format?");

                var procCode = _preAuthAction.GetFlagLineLevelHeaderDetailValue(col: 3);
                var procCodePage = _preAuthAction.ClickOnProcCodeAndSwitch("CDT Code", procCode);
                procCodePage.GetTextValueinLiTag(1)
                    .ShouldBeEqual(string.Concat("Code: ", procCode), "Is Correct popup displayed?");
                _preAuthAction.CloseAnyTabIfExist();

                _preAuthAction.GetFlagLineLevelHeaderDetailValue(col: 4)
                    .ShouldBeEqual(_preAuthAction.GetProcDesc(procCode, "DEN"), "Is Proc Short Description Equals?");

                //TN,TS and OC value are manually added
                _preAuthAction.GetFlagLineLevelHeaderDetailValue(col: 5).ShouldNotBeEmpty("Is TN Value Displayed?");
                _preAuthAction.GetFlagLineLevelHeaderDetailValue(col: 6).ShouldNotBeEmpty("Is TS Value Displayed?");
                _preAuthAction.GetFlagLineLevelHeaderDetailValue(col: 7).ShouldNotBeEmpty("Is OC Value Displayed?");

                _preAuthAction.GetFlagLineLevelHeaderDetailLabel(col: 5).ShouldBeEqual("TN:", "Is TN Label Displayed?");
                _preAuthAction.GetFlagLineLevelHeaderDetailLabel(col: 6).ShouldBeEqual("TS:", "Is TS Label Displayed?");
                _preAuthAction.GetFlagLineLevelHeaderDetailLabel(col: 7).ShouldBeEqual("OC:", "Is OC Label Displayed?");

                _preAuthAction.GetFlagLineLevelHeaderDetailLabel(row: 2, col: 2)
                    .ShouldBeEqual("Sug Paid:", "Is TN Label Displayed?");
                _preAuthAction.GetFlagLineLevelHeaderDetailLabel(row: 2, col: 3)
                    .ShouldBeEqual("Savings:", "Is TS Label Displayed?");
                _preAuthAction.GetFlagLineLevelHeaderDetailLabel(row: 2, col: 4)
                    .ShouldBeEqual("Scen:", "Is Scenario Label Displayed?");

                _preAuthAction.GetFlagLineLevelHeaderDetailValue(row: 2, col: 4)
                    .ShouldBeEqual("A", "Is OC Label Displayed?");


                _preAuthAction.ClickOnReturnToPreAuthSearch()
                    .SearchByAuthSequenceAndNavigateToAuthAction(authSeqWithTriggerInfo);

                var flag = _preAuthAction.GetFlagRowDetailValue(col: 2);
                automatedBase.CurrentPage.CaptureScreenShot("Failed reason for Failing PreAuthAction Test");
                var flagPopupPage =
                    _preAuthAction.ClickOnFlagandSwitch("Flag Information - " + flag, isInternalUser: false);
                flagPopupPage.GetPopupHeaderText()
                    .ShouldBeEqual("Flag - " + flag, "Is Correct Popup Page displayed?", true);
                _preAuthAction.CloseAnyTabIfExist();

                _preAuthAction.IsFlagRowEditIconPresent().ShouldBeTrue("Is Edit Icon Dislayed in Flag Row?");
                _preAuthAction.IsFlagRowAddLogicIconPresent().ShouldBeTrue("Is Logic Icon Dislayed in Flag Row?");

                _preAuthAction.GetFlagRowDetailLabel().ShouldBeEqual("Source:", "Is TN Label Display?");

                _preAuthAction.GetClientDataSourceValue().ShouldBeEqual("HCI Clinical Policy Committee Guidelines",
                    "Is Source Value Display?");

                var actualflagList = _preAuthAction.GetFlagListByDivList(isInternalUser: false);

                _preAuthAction
                    .GetFlagOrderSequenceListByFlag("'" + string.Join(",", actualflagList).Replace(",", "','") + "'")
                    .ShouldCollectionBeEqual(actualflagList,
                        "Is Flags are displayed ascending order by flag order from Edit Table?");
            }
        }

        [Test] //CAR-267
        public void Verify_Navigation_To_Pre_Auth_Action_And_Template_Client()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                PreAuthActionPage _preAuthAction;
                PreAuthSearchPage _preAuthSearch;

                _preAuthSearch = automatedBase.CurrentPage.NavigateToPreAuthSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var quadrants = paramLists["QuadrantTitles"].Split(',').ToList();
                var toolTipTexts = paramLists["ToolTipValues"].Split(',').ToList();

                _preAuthSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                    PreAuthQuickSearchClientEnum.DocumentsNeeded.GetStringValue());


                _preAuthSearch.GetSideBarPanelSearch.ClickOnFindButton();
                _preAuthSearch.WaitForWorkingAjaxMessage();

                var listBeforeSearch = _preAuthSearch.GetGridViewSection.GetGridAllRowData();

                StringFormatter.PrintMessage("Template verification");

                _preAuthAction = _preAuthSearch.ClickOnGridByRowColToPreAuthActionPage(1, 2);
                _preAuthAction.GetQuadrantHeaderTitleList().ShouldCollectionBeEqual(quadrants,
                    "Pre-Auth Action page is divided into 4 quadrant template");

                _preAuthAction.IsSearchIconPresent().ShouldBeTrue("Search icon should be present");
                _preAuthAction.GetSearchIconToolTip()
                    .ShouldBeEqual(toolTipTexts[0], "Tooltip values should match");
                _preAuthAction.IsMoreOptionsIconPresent().ShouldBeTrue("More optoins icon should be present");
                _preAuthAction.GetMoreOptionIconToolTip().ShouldBeEqual(toolTipTexts[1], "Tooltip values should match");

                StringFormatter.PrintMessage(
                    "Verification of Pre-Auth Search page being retained on clicking search icon");

                _preAuthAction.ClickOnReturnToPreAuthSearch();
                _preAuthSearch.GetPageHeader().ShouldBeEqual(PageHeaderEnum.PreAuthSearch.GetStringValue(),
                    "PreAuthSearch page should be retained on clicking return to pre-auth search ");
                _preAuthSearch.GetGridViewSection.GetGridAllRowData()
                    .ShouldCollectionBeEqual(listBeforeSearch, "Previous list should be retained");

            }
        }

        [Test] //CAR-262
        public void Verify_Line_item_data_points_for_client()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                PreAuthActionPage _preAuthAction;
                PreAuthSearchPage _preAuthSearch;

                _preAuthSearch = automatedBase.CurrentPage.NavigateToPreAuthSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var authSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "AuthSequence",
                    "Value");
                _preAuthAction = _preAuthSearch.SearchByAuthSequenceAndNavigateToAuthAction(authSeq);
                var lineDetails = _preAuthAction.GetLineDetailsValuesFromDB(authSeq);
                string procCodeDescShort;
                string procCodeDescDetail;
                List<string> flagList = new List<string>();

                try
                {
                    var lineNoList = _preAuthAction.GetLineNumberValues();
                    lineNoList.IsInAscendingOrder().ShouldBeTrue("Line Number Should be in Ascending Order.");
                    var flagNoList = _preAuthAction.GetFlagLineNumberList();
                    for (int j = 1; j <= flagNoList.Count; j++)
                    {
                        var flag = _preAuthAction.GetFlagListByDivList(j, false)[0];
                        flagList.Add(flag);
                    }


                    _preAuthAction.GetQuadrantHeaderTitleList()[3]
                        .ShouldBeEqual("Line Items", "Quadrant title should be equal to Line Items.");
                    var lineFlagList = _preAuthAction.GetLineItemsValueListByRowColumn();
                    int k = 0;
                    foreach (var flagNo in flagNoList)
                    {
                        lineFlagList[Convert.ToInt32(flagNo) - 1]
                            .ShouldBeEqual(flagList[k], "Top Flag are shown if the line are flagged.");
                        k++;
                    }

                    var dosList = _preAuthAction.GetLineItemsValueListByRowColumn(1, 3);
                    var procCodeList = _preAuthAction.GetLineItemsValueListByRowColumn(1, 4);
                    var tnList = _preAuthAction.GetLineItemsValueListByRowColumn(3, 2);
                    var tsList = _preAuthAction.GetLineItemsValueListByRowColumn(3, 3);
                    var ocList = _preAuthAction.GetLineItemsValueListByRowColumn(3, 4);

                    for (int i = 0; i < lineNoList.Count; i++)
                    {
                        procCodeDescShort = _preAuthAction.GetProcDesc(lineDetails[i][2], "DEN");
                        procCodeDescDetail = _preAuthAction.GetProcCodeDescriptionFromDB(lineDetails[i][2]);
                        _preAuthAction.GetLineNumberValues()[i].ShouldBeEqual(lineDetails[i][0],
                            string.Format("Line number should be equal to {0}.", lineDetails[i][0]));
                        dosList[i].ShouldBeEqual(Convert.ToDateTime(lineDetails[i][1]).ToString("MM/dd/yyyy"),
                            string.Format("Date of Service value should be equal to {0}.",
                                Convert.ToDateTime(lineDetails[i][1]).ToString("MM/dd/yyyy")));
                        procCodeList[i].ShouldBeEqual(lineDetails[i][2],
                            String.Format("Proc Code value should be equal to {0}.", lineDetails[i][2]));

                        _preAuthAction.GetProcCodeDescription()[i].ShouldBeEqual(procCodeDescShort,
                            string.Format("Proc Code Description value should be equal to {0}.",
                                procCodeDescShort));
                        NewPopupCodePage _popupCode =
                            _preAuthAction.ClickOnLineDetailsProcCodeAndSwitch("CDT Code", lineDetails[i][2], i + 1);
                        _popupCode.GetProcCodeDescriptionFromPopUp().ShouldBeEqual(procCodeDescDetail,
                            string.Format("Proc Code Description should consist {0}.", procCodeDescDetail));

                        _preAuthAction = _popupCode.ClosePopupOnPreAuthActionPage("CDT Code - " + lineDetails[i][2]);
                        _preAuthAction.GetLineItemsValueByLabel("Billed:")[i].ShouldBeEqual(
                            Convert.ToInt32(lineDetails[i][3]).ToString("C"),
                            string.Format("Billed value should be equal to {0}.",
                                Convert.ToInt32(lineDetails[i][3]).ToString("C")));
                        _preAuthAction.GetLineItemsValueByLabel("Allowed:")[i].ShouldBeEqual(
                            Convert.ToInt32(lineDetails[i][4]).ToString("C"),
                            string.Format("Allowed value should be equal to {0}.",
                                Convert.ToInt32(lineDetails[i][4]).ToString("C")));
                        _preAuthAction.GetLineItemsValueByLabel("Adj Paid:")[i].ShouldBeEqual(
                            Convert.ToInt32(lineDetails[i][4]).ToString("C"),
                            string.Format("Adj Paid value should be equal to {0}.",
                                Convert.ToInt32(lineDetails[i][4]).ToString("C")));
                        _preAuthAction.GetLineItemsValueByLabel("Scen:")[i].ShouldBeEqual(lineDetails[i][5],
                            string.Format("Scenario value should be equal to {0}.", lineDetails[i][5]));
                        tnList[i].ShouldBeEqual(lineDetails[i][6],
                            string.Format("TN value should be equal to {0}.", lineDetails[i][6]));
                        tsList[i].ShouldBeEqual(lineDetails[i][7],
                            string.Format("TS value should be equal to {0}.", lineDetails[i][7]));
                        ocList[i].ShouldBeEqual(lineDetails[i][8],
                            string.Format("OC value should be equal to {0}.", lineDetails[i][8]));
                    }
                }

                finally
                {
                    _preAuthAction.CloseAnyTabIfExist();
                }
            }

        }

        [Test] //CAR-1096 (CAR-1514) 
        public void Verify_Delete_Restore_Flag_Option_Is_Not_Available_To_Users_Without_Manage_Edits_Privilege_Client()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                PreAuthActionPage _preAuthAction;
                PreAuthSearchPage _preAuthSearch;

                _preAuthSearch = automatedBase.CurrentPage.NavigateToPreAuthSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramsList =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var preAuthSeqWithFlags = paramsList["AuthSequenceWithFlags"];

                StringFormatter.PrintMessageTitle(
                    "Verifying that Edit Flag icons should be disabled for users without the 'Manage Edits' privilege");
                _preAuthAction = automatedBase.CurrentPage.Logout().LoginAsClientUserWithoutManageEditsPrivilege()
                    .NavigateToPreAuthSearch().SearchByAuthSequenceAndNavigateToAuthAction(preAuthSeqWithFlags, false);

                _preAuthAction.GetCountOfEnabledEditFlagIcons().ShouldBeEqual(0,
                    "Edit Flag icons should be disabled for users without 'Manage Edits' privilege ");
            }
        }

        [Test] //CAR-1096 (CAR-1514)
        public void Verify_Delete_Restore_Flag_In_Pre_Auth_Action_Page_For_Client_Users()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                PreAuthActionPage _preAuthAction;
                PreAuthSearchPage _preAuthSearch;

                _preAuthSearch = automatedBase.CurrentPage.NavigateToPreAuthSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramsList =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var preAuthSeqWithFlags = paramsList["AuthSequenceWithFlags"];

                // Setting any deleted flags to undeleted state and removing Flag Audit History from the DB
                _preAuthSearch.UnDeleteAnyFlagByPreAuthSeq(preAuthSeqWithFlags);
                _preAuthSearch.DeleteFlagAuditHistoryFromDatabaseByPreAuthSeq(preAuthSeqWithFlags);

                _preAuthAction = _preAuthSearch.SearchByAuthSequenceAndNavigateToAuthAction(preAuthSeqWithFlags, false);

                StringFormatter.PrintMessageTitle("Verifying clicking on the 'Edit' Pencil Icon");
                _preAuthAction.GetEditFlagToolTipByFlagNo()
                    .ShouldBeEqual("Edit Flag", "Is tooltip message for 'Edit Flag' as expected ?");

                _preAuthAction.ClickOnEditFlagIconByLineNoAndRow();
                _preAuthAction.IsEditFlagFormSectionPresent()
                    .ShouldBeTrue("Is Edit Flag form section present once edit flag icon is clicked ?");

                _preAuthAction.IsIconInFlaggedLinesSectionActive("Approve")
                    .ShouldBeFalse("Is 'Approve' icon disabled when edit flag icon is clicked? ");
                _preAuthAction.IsIconInFlaggedLinesSectionActive("Transfer")
                    .ShouldBeFalse("Is 'Transfer' icon disabled when edit flag icon is clicked?");
                _preAuthAction.IsIconInFlaggedLinesSectionActive("Next")
                    .ShouldBeFalse("Is 'Next' icon disabled when edit flag icon is clicked?");

                StringFormatter.PrintMessage("Verifying 'Delete Flag' icon in flag row");
                // Verifying delete icon is accessible when there are no deleted flags in the pre-auth
                _preAuthAction.GetCountOfDeletedFlags().ShouldBeEqual(0, "Get count of deleted flags in the pre-auth");
                _preAuthAction.ClickOnDeleteOrRestoreIcon("Delete");
                _preAuthAction.GetEditFlaggedLinesFormSectionHeader().ShouldBeEqual("Delete Flag",
                    "Is form header 'Delete Flag' after clicking on delete icon");
                _preAuthAction.ClickSaveButton();
                _preAuthAction.IsPageErrorPopupModalPresent()
                    .ShouldBeTrue("Is an error popup present when 'Reason Code' is not selected?");
                _preAuthAction.GetPageErrorMessage()
                    .ShouldBeEqual("Reason Code is required before the record can be saved.");
                _preAuthAction.ClosePageError();

                _preAuthAction.GetSideWindow.GetDropDownListDefaultValue("Reason Code")
                    .ShouldBeEqual(" ", "Is the default 'Reason Code' value null ?");

                var listOfReasonCodesFromDropdown =
                    _preAuthAction.GetSideWindow.GetAvailableDropDownList("Reason Code");
                var listOfReasonCodesFromDB = _preAuthAction.GetReasonCodeListFromDB(false);

                listOfReasonCodesFromDropdown.Remove("");
                listOfReasonCodesFromDropdown.ShouldCollectionBeEqual(listOfReasonCodesFromDB,
                    "Does the 'Reason Code' dropdown contain correct items reason codes ?");

                StringFormatter.PrintMessageTitle("Verifying 'Note' field in the edit flag form");
                _preAuthAction.IsVisibleToClientIconPresent()
                    .ShouldBeFalse("Is 'Visible To Client' checkbox present ?");
                _preAuthAction.GetSideWindow.IsAsertiskPresent("Note")
                    .ShouldBeFalse("Is 'Note' field a required field ?");
                _preAuthAction.ClickCancel();

                StringFormatter.PrintMessageTitle("Verifying Delete/Restore of Flags");
                var flags = paramsList["Flags"].Split(';').ToList();
                var flagSavings = paramsList["FlagSavings"].Split(';').ToList();

                VerifyDeleteRestoreOfFlags(_preAuthAction, preAuthSeqWithFlags, 3, flags[0]);
                VerifyTopFlagOrderAndSavings(_preAuthAction, 3, flags, flagSavings);
                VerificationOfFlagAuditHistory(_preAuthAction, 3, 2, paramsList);

                VerifyDeleteRestoreOfFlags(_preAuthAction, preAuthSeqWithFlags, 3, flags[0], false);
                VerifyTopFlagOrderAndSavings(_preAuthAction, 3, flags, flagSavings, false);
                VerificationOfFlagAuditHistory(_preAuthAction, 3, 2, paramsList, false);

            }
        }

        [Test] //CAR-1096 (CAR-1514)
        public void Verify_Cancel_Button_In_Delete_Restore_Flag_In_Pre_Auth_Action_Page_For_Client_Users()
        {

            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                PreAuthActionPage _preAuthAction;
                PreAuthSearchPage _preAuthSearch;

                _preAuthSearch = automatedBase.CurrentPage.NavigateToPreAuthSearch();
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramsList =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var preAuthSeq = paramsList["PreAuthSeq"];
                var flag = paramsList["Flag"];

                // Removing any flag deletes and Audit history from the Pre-Auth before continuing with the test
                _preAuthSearch.UnDeleteAnyFlagByPreAuthSeq(preAuthSeq);
                _preAuthSearch.DeleteFlagAuditHistoryFromDatabaseByPreAuthSeq(preAuthSeq);

                StringFormatter.PrintMessage("Verifying Cancel button while performing Edit Action");
                _preAuthAction = _preAuthSearch.SearchByAuthSequenceAndNavigateToAuthAction(preAuthSeq);

                _preAuthAction.ClickOnLineItemOrFlagAuditHistoryIconByTitle("Flag Audit History");
                var originalAuditHistoryCount = _preAuthAction.GetFlagAuditHistoryActionCount(1);

                _preAuthAction.ClickOnEditFlagIconByLineNoAndRow();
                _preAuthAction.SelectReasonCode(2);
                _preAuthAction.InputNotes("Test Note");
                _preAuthAction.GetSideWindow.Cancel();

                _preAuthAction.GetFlagAuditHistoryActionCount(1).ShouldBeEqual(originalAuditHistoryCount,
                    "Is Flag Audit History the same after Edit Form is cancelled?");

                StringFormatter.PrintMessage("Verifying Cancel button while performing Delete Action");
                _preAuthAction.ClickOnEditFlagIconByLineNoAndRow();
                _preAuthAction.ClickOnDeleteOrRestoreIcon("Delete");
                _preAuthAction.SelectReasonCode(2);
                _preAuthAction.InputNotes("Test Note");
                _preAuthAction.GetSideWindow.Cancel();

                _preAuthAction.GetFlagAuditHistoryActionCount(1).ShouldBeEqual(originalAuditHistoryCount,
                    "Is Flag Audit History the same after Edit Form is cancelled?");
                _preAuthAction.IsFlagRowDeletedPresent()
                    .ShouldBeFalse(
                        "Is Flag deleted when Cancel button is clicked from Edit Form during Delete Action?");

                StringFormatter.PrintMessage("Verifying Cancel button on Restore Action");
                // Deleting the flag first to check for Restore
                _preAuthAction.DeleteRestoreFlagByLineNoAndFlagName(1, flag);
                _preAuthAction.ClickOnEditFlagIconByLineNoAndRow();
                _preAuthAction.ClickOnDeleteOrRestoreIcon("Restore");
                _preAuthAction.SelectReasonCode(2);
                _preAuthAction.InputNotes("Test Note");
                _preAuthAction.GetSideWindow.Cancel();

                _preAuthAction.GetCountOfDeletedFlags()
                    .ShouldBeEqual(1,
                        "Is Flag still in deleted state when Cancel button is clicked from Edit Form during Restore Action?");
            }
        }

        [Test] //CAR-1609 (CAR-1523)
        [Order(2)]
        public void Verify_Next_Button_In_Pre_Auth_Action_Page()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                PreAuthActionPage _preAuthAction;
                PreAuthSearchPage _preAuthSearch;

                _preAuthSearch = automatedBase.CurrentPage.NavigateToPreAuthSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;

                _preAuthSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                    PreAuthQuickSearchEnum.AllPreAuths.GetStringValue());
                _preAuthSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Status",
                    StatusEnum.ClientUnreviewed.GetStringDisplayValue());
                _preAuthSearch.ClickOnFindButton();
                var totalSearchResultCount = _preAuthSearch.GetGridViewSection.GetGridRowCount();

                var preAuthSearchResultDetails = _preAuthSearch.GetSearchResultUptoRow(4, true);

                _preAuthAction = _preAuthSearch.ClickOnFirstUnlockedPreAuthToNavigateToPreAuthActionPage();

                StringFormatter.PrintMessageTitle(
                    "Verifying whether 'Next' icon is disabled while 'Working' message is displayed");
                var preAuthFlagDetails = VerifyNextPreAuthDetailsAndReturnFlagDetails(_preAuthAction, preAuthSearchResultDetails);
                _preAuthSearch.GetGridViewSection.GetGridRowCount().ShouldBeEqual(totalSearchResultCount,
                    "Search result is unchaged when 'Next' is used to" +
                    " navigate across the Pre-Auths");

                foreach (KeyValuePair<string, List<string>> keyValuePair in preAuthFlagDetails)
                {
                    _preAuthSearch.ClickOnAuthSeqToNavigateToPreAuthActionPage(keyValuePair.Key);
                    _preAuthSearch.WaitForWorking();
                    _preAuthAction.GetCountOfDeletedFlags().ToString().ShouldBeEqual(keyValuePair.Value[0],
                        "Flag status should be unchanged when 'Next' icon " +
                        "is clicked");
                    _preAuthAction.GetUpperLeftQuadrantValueByLabel("Status").ShouldBeEqual(keyValuePair.Value[1],
                        "Pre-Auth status should remain unchanged after " +
                        "'Next' icon is clicked");

                    _preAuthAction.ClickOnReturnToPreAuthSearch();
                }

                StringFormatter.PrintMessageTitle(
                    "Verifying whether the 'Next' icon shows correct results for sorted Pre-Auth search results");

                _preAuthSearch.ClickOnFilterOptionListRow(1);

                var preAuthSearchResultDetailsSortedByAuthSeq = _preAuthSearch.GetSearchResultUptoRow(4, true);
                _preAuthAction = _preAuthSearch.ClickOnFirstUnlockedPreAuthToNavigateToPreAuthActionPage();

                VerifyNextPreAuthDetailsAndReturnFlagDetails(_preAuthAction, preAuthSearchResultDetailsSortedByAuthSeq);
                _preAuthSearch.GetGridViewSection.GetGridRowCount().ShouldBeEqual(totalSearchResultCount,
                    "Search result should remain unchanged");

                StringFormatter.PrintMessageTitle("Verifying 'Next' icon when searched for a single record");
                _preAuthSearch.GetSideBarPanelSearch.ClickOnClearLink();
                _preAuthAction =
                    _preAuthSearch.SearchByAuthSequenceAndNavigateToAuthAction(
                        preAuthSearchResultDetailsSortedByAuthSeq.Keys.ElementAt(0), false);
                _preAuthAction.ClickOnNextIcon();
                _preAuthSearch.WaitForPageToLoadWithSideBarPanel();

                automatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(
                    PageHeaderEnum.PreAuthSearch.GetStringValue(),
                    "Does clicking on 'Next' icon after reaching the end of a list return to Pre-Auth Search Page ? ");
                _preAuthSearch.GetGridViewSection.GetGridRowCount().ShouldBeEqual(1, "Searched Result is retained");
                
            }
        }

        [Test] //CAR-1087 (CAR-1705)
        public void Verify_pre_auth_processing_history_pop_up_for_client_user()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                PreAuthActionPage _preAuthAction;
                PreAuthSearchPage _preAuthSearch;
                PreAuthProcessingHistoryPage _preAuthProcessingHistory;

                _preAuthSearch = automatedBase.CurrentPage.NavigateToPreAuthSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramsList = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var preAuthSeq = paramsList["PreAuthSeq"];
                var columnHeaderList = paramsList["ColumnHeaderList"].Split(';').ToList();
                var expectedAuditData = paramsList["AuditData"].Split(';').ToList();
                _preAuthSearch.DeleteHistoryAndUpdateStatusByAuthSeq(preAuthSeq, "21-DEC-19",
                    StatusEnum.ClientUnreviewed.GetStringValue());
                _preAuthAction = _preAuthSearch.SearchByAuthSequenceAndNavigateToAuthAction(preAuthSeq, false);

                var expectedValueList = _preAuthAction.PreAuthProcessingHistoryList(preAuthSeq);
                try
                {
                    _preAuthProcessingHistory = _preAuthAction.ClickOnPreAuthProcessingHistoryAndSwitch();
                    _preAuthAction.IsPreAuthProcessingHistoryOpen()
                        .ShouldBeTrue("Is Pre-Auth Processing History popup opened?");
                    _preAuthProcessingHistory.GetPageHeader()
                        .ShouldBeEqual(PageHeaderEnum.PreAuthProcessingHistory.GetStringValue());
                    _preAuthProcessingHistory.GetColumnHeaderList().ShouldCollectionBeEqual(columnHeaderList,
                        "Pre-Auth Processing History grid column header should be correct");
                    var rowCount = _preAuthProcessingHistory.GetRowCount();

                    for (int i = 1; i <= rowCount; i++)
                    {
                        string expectedStatus;
                        if (expectedValueList[i - 1][2] == "X")
                            expectedStatus = "Editor Analyzed";
                        else
                        {
                            var status = expectedValueList[i - 1][2].TryParse<StatusEnum>();
                            expectedStatus = status.GetStringDisplayValue();
                        }

                        _preAuthProcessingHistory.GetGridValueByRowCol(i).ShouldBeEqual(
                            DateTime.Parse(expectedValueList[i - 1][0]).ToString(), "Date should in date time format");
                        _preAuthProcessingHistory.GetGridValueByRowCol(i, 2).ShouldBeEqual(expectedValueList[i - 1][1],
                            "Users <First Name> <Last Name> should be displayed. ");
                        _preAuthProcessingHistory.GetGridValueByRowCol(i, 3).ShouldBeEqual(expectedStatus,
                            "Status of Pre-Auth at the time of action should be displayed");
                        _preAuthProcessingHistory.GetGridValueByRowCol(i, 4)
                            .ShouldBeEqual(expectedValueList[i - 1][3], "Note should be displayed");
                    }


                    _preAuthAction =
                        _preAuthProcessingHistory.CloseClaimProcessingHistoryPageAndSwitchToPreAuthActionPage();
                    StringFormatter.PrintMessage("Verify Processing History after approval of Pre-Auth");
                    _preAuthSearch = _preAuthAction.ClickOnApproveIconAndNavigateToPreAuthSearchPage();
                    var t = DateTime.ParseExact(_preAuthAction.GetSystemDateFromDatabase(), "MM/dd/yyyy hh:mm:ss tt",
                        CultureInfo.InvariantCulture);
                    _preAuthSearch.GetSideBarPanelSearch.WaitSideBarPanelOpen();
                    _preAuthAction = _preAuthSearch.ClickOnGridByRowColToPreAuthActionPage(1);
                    _preAuthProcessingHistory = _preAuthAction.ClickOnPreAuthProcessingHistoryAndSwitch();
                    DateTime.ParseExact(_preAuthProcessingHistory.GetGridValueByRowCol(), "M/d/yyyy h:m:s tt",
                        CultureInfo.InvariantCulture).AssertDateRange(t.AddSeconds(-2), t.AddSeconds(2),
                        "Current Date And Time Stamp Must Display");
                    _preAuthProcessingHistory.GetGridValueByRowCol(1, 2)
                        .ShouldBeEqual(expectedAuditData[0], "User Name");
                    _preAuthProcessingHistory.GetGridValueByRowCol(1, 3).ShouldBeEqual(expectedAuditData[1], "Status");
                    _preAuthProcessingHistory.GetGridValueByRowCol(1, 4)
                        .ShouldBeEqual(expectedAuditData[2], "Notes");

                }
                finally
                {
                    _preAuthAction.CloseAnyPopupIfExist();
                }
            }
        }

        [Test] //CAR-1457 (CAR-1703) + CAR-3139 + CV-3532 (CV-3681)
        public void Verify_transfer_pre_auth_for_client_users()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                PreAuthActionPage _preAuthAction;
                PreAuthSearchPage _preAuthSearch;
                PreAuthProcessingHistoryPage _preAuthProcessingHistory;

                _preAuthSearch = automatedBase.CurrentPage.NavigateToPreAuthSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var testData = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var memberID = testData["Member_Id"];
                var provSeq = testData["ProvSeq"];
                var preAuthIDClosedStatus = testData["PreAuthIDClosedStatus"];
                var preAuthSeq = testData["PreAuth"];
                var preAuthForSingleSearch = testData["PreAuthForSingleSearch"];
                var preAuthForWhichTransferIconIsDisabled = testData["PreAuthForWhichTransferIconIsDisabled"];
                var statusForWhichTransferIconIsDisabled =
                    testData["StatusForWhichTransferIconIsDisabled"].Split(';').ToList();

                var orderOfStatusAssignment = new List<string>
                {
                    StatusEnum.DocumentsRequested.GetStringDisplayValue()
                };

                _preAuthSearch.UpdateStatusOfAuthSeqFromDatabase("B", preAuthSeq);
                _preAuthSearch.UpdateStatusOfAuthSeqFromDatabase("B", preAuthForSingleSearch);

                StringFormatter.PrintMessageTitle(
                    "Verifying 'Transfer' icon is disabled for statuses other than Docs Required,Docs Requested and Closed");
                foreach (var status in statusForWhichTransferIconIsDisabled)
                {
                    _preAuthSearch.UpdateStatusOfAuthSeqFromDatabase($"{status}",
                        preAuthForWhichTransferIconIsDisabled);

                    _preAuthAction =
                        _preAuthSearch.SearchByAuthSequenceAndNavigateToAuthAction(
                            preAuthForWhichTransferIconIsDisabled, false);
                    var currentStatus = _preAuthAction.GetUpperLeftQuadrantValueByLabel("Status");
                    _preAuthAction.IsTransferIconEnabled()
                        .ShouldBeFalse($"Transfer Icon should be disabled for {currentStatus}");
                    _preAuthAction.ClickOnReturnToPreAuthSearch();
                    _preAuthSearch.GetSideBarPanelSearch.ClickOnClearLink();
                }

                StringFormatter.PrintMessage("Searching for the pre-auths");
                _preAuthSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                    PreAuthQuickSearchEnum.AllPreAuths.GetStringValue());
                _preAuthSearch.SetInputFieldByInputLabel("Member ID", memberID);
                _preAuthSearch.SetInputFieldByInputLabel("Provider Sequence", provSeq);
                _preAuthSearch.ClickOnFindButton();

                var preAuthSeqListFromGrid = _preAuthSearch
                    .GetSearchResultUptoRow(_preAuthSearch.GetGridViewSection.GetGridRowCount(), true).Keys
                    .ToList();
                _preAuthAction = _preAuthSearch.ClickOnAuthSeqToNavigateToPreAuthActionPage(preAuthSeq);
                _preAuthAction.WaitForWorking();

                var preAuthStatus = _preAuthAction.GetUpperLeftQuadrantValueByLabel("Status");

                StringFormatter.PrintMessage("Verifying presence of Transfer Form");
                _preAuthAction.ClickOnTransferIcon();
                _preAuthAction.IsTransferPreAuthFormPresent()
                    .ShouldBeTrue(
                        "The Transfer Pre-Auth form section should be present once Transfer button is clicked");

                StringFormatter.PrintMessage("Verification of the required fields in the Transfer Form");
                _preAuthAction.GetSideWindow.IsAsertiskPresent("Status")
                    .ShouldBeTrue("Status should be a required field");
                _preAuthAction.GetSideWindow.IsAsertiskPresent("Note")
                    .ShouldBeFalse("Note should not be a required field");

                StringFormatter.PrintMessageTitle(
                    "Verification of the max number of characters in the 'Note' field");
                var noteText = new string('a', 501);
                _preAuthAction.SetNoteInTransferPreAuth(noteText);
                var noteInTransferTextArea = _preAuthAction.GetNoteFromTransferPreAuth();
                noteInTransferTextArea.Length.ShouldBeEqual(500,
                    "The Note field should allow only upto 500 characters");

                StringFormatter.PrintMessage("Verification of the cancel button in the Transfer form");
                _preAuthAction.ClickCancel();
                _preAuthAction.IsTransferPreAuthFormPresent()
                    .ShouldBeFalse("Clicking on 'Cancel' should close the 'Transfer Pre-Auth' form");
                _preAuthAction.GetUpperLeftQuadrantValueByLabel("Status")
                    .ShouldBeEqual(preAuthStatus, "Pre-Auth Status should remain unchanged if Cancel is clicked");

                _preAuthAction.ClickOnTransferIcon();

                var currentPreAuthSeq = _preAuthAction.GetUpperLeftQuadrantValueByLabel("Auth Seq");
                var statusList = _preAuthAction.GetSideWindow.GetAvailableDropDownList("Status");
                statusList.Remove(preAuthStatus);

                StringFormatter.PrintMessageTitle("Verification of the Status dropdown field in the Transfer form");
                VerifyStatusDropdownInTransferForm(_preAuthAction, preAuthStatus, statusList);

                StringFormatter.PrintMessageTitle(
                    "Verification of the Transfer functionality for list of pre-auths in the pre-auth search page");
                VerifyTransferFunctionality(preAuthSeqListFromGrid, currentPreAuthSeq, orderOfStatusAssignment);

                StringFormatter.PrintMessageTitle(
                    "Verification Transfer functionality for single pre-auth in pre-auth search page");
                _preAuthSearch = _preAuthAction.ClickOnReturnToPreAuthSearch();
                _preAuthSearch.GetSideBarPanelSearch.ClickOnClearLink();
                _preAuthAction = _preAuthSearch.SearchByAuthSequenceAndNavigateToAuthAction(preAuthForSingleSearch);

                VerifyTransferFunctionality(null, preAuthForSingleSearch,
                    new List<string>() {StatusEnum.DocumentReview.GetStringDisplayValue()}, false);

                #region CV-3532 (CV-3681)
                StringFormatter.PrintMessageTitle("Verification of the Status dropdown field in the Transfer form when current status is closed");
                _preAuthAction.ClickOnReturnToPreAuthSearch();
                _preAuthSearch.GetSideBarPanelSearch.ClickOnClearLink();
                _preAuthSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                    PreAuthQuickSearchEnum.AllPreAuths.GetStringValue());
                _preAuthSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Auth Sequence", preAuthIDClosedStatus);
                _preAuthSearch.ClickOnFindButton();
                _preAuthSearch.WaitForWorkingAjaxMessage();
                _preAuthAction = _preAuthSearch.ClickOnAuthSeqToNavigateToPreAuthActionPage(preAuthIDClosedStatus);
                preAuthStatus = _preAuthAction.GetUpperLeftQuadrantValueByLabel("Status");
                _preAuthAction.ClickOnTransferIcon();
                statusList = _preAuthAction.GetSideWindow.GetAvailableDropDownList("Status");
                statusList.Remove(preAuthStatus);
                VerifyStatusDropdownInTransferForm(_preAuthAction, preAuthStatus, statusList);
                #endregion



                #region LOCAL METHOD

                void VerifyTransferFunctionality(List<string> preAuthList, string currentPreAuth, List<string> listOfStatus, bool multiplePreAuthsInSearchGrid = true)

                {
                    if (multiplePreAuthsInSearchGrid)
                    {
                        if (!_preAuthAction.IsTransferIconPresent())
                            _preAuthAction.ClickOnTransferIcon();

                        List<string> newStatusList;
                        string newCurrentStatus;


                        foreach (var status in listOfStatus)
                        {
                            _preAuthAction.ClickOnTransferIcon();
                            _preAuthAction.SelectStatusDropDownValue(status);
                            _preAuthAction.SetNoteInTransferPreAuth($"Test Note for {status}");
                            _preAuthAction.ClickSaveButton();
                            _preAuthAction.WaitForWorking();
                            _preAuthAction.CloseAnyTabIfExist();

                            _preAuthAction.GetUpperLeftQuadrantValueByLabel("Auth Seq")
                                .ShouldBeEqual(preAuthList[preAuthList.IndexOf(currentPreAuth) + 1], "User should be directed to the next pre-auth in the list");

                            _preAuthSearch = _preAuthAction.NavigateToPreAuthSearch();

                            StringFormatter.PrintMessage("Verifying whether the recently changed status can be searched from the Pre-Auth Search page");
                            _preAuthSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Status", status);
                            _preAuthSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Auth Sequence", currentPreAuth);
                            _preAuthSearch.ClickOnFindButton();
                            _preAuthSearch.GetSideBarPanelSearch.IsNoMatchingRecordFoundMessagePresent()
                                .ShouldBeFalse("User is able to find the pre-auth by filtering by the newly assigned status");
                            _preAuthSearch.GetGridViewSection.GetValueInGridByColRow(8)
                                .ShouldBeEqual(status, "Status should be updated to the newly set status in Pre-Auth Search grid");

                            _preAuthSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Status", "");
                            _preAuthSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Auth Sequence", "");
                            _preAuthSearch.ClickOnFindButton();

                            _preAuthAction = _preAuthSearch.ClickOnAuthSeqToNavigateToPreAuthActionPage(currentPreAuth);

                            newCurrentStatus = _preAuthAction.GetUpperLeftQuadrantValueByLabel("Status");
                            newCurrentStatus.ShouldBeEqual(status, "The pre-auth should be assigned the new status after the transfer process");
                            _preAuthProcessingHistory = _preAuthAction.ClickOnPreAuthProcessingHistoryAndSwitch();

                            _preAuthProcessingHistory.GetGridValueByRowCol(1, 4).Contains($"Test Note for {status}")
                                .ShouldBeTrue("Is the entered note being displayed correctly in Pre-Auth Processing History");

                            _preAuthProcessingHistory.CloseClaimProcessingHistoryPageAndSwitchToPreAuthActionPage();

                            _preAuthAction.ClickOnTransferIcon();
                            newStatusList = _preAuthAction.GetSideWindow.GetAvailableDropDownList("Status");
                            newStatusList.Remove(newCurrentStatus);
                            VerifyStatusDropdownInTransferForm(_preAuthAction, newCurrentStatus, newStatusList);
                        }
                    }

                    else
                    {
                        if (!_preAuthAction.IsTransferIconPresent())
                            _preAuthAction.ClickOnTransferIcon();

                        List<string> newStatusList;
                        string newCurrentStatus;

                        foreach (var status in listOfStatus)
                        {
                            _preAuthAction.ClickOnTransferIcon();
                            _preAuthAction.SelectStatusDropDownValue(status);
                            _preAuthAction.SetNoteInTransferPreAuth($"Test Note for {status}");
                            _preAuthAction.ClickSaveButton();
                            _preAuthAction.WaitForWorking();
                            _preAuthAction.CloseAnyTabIfExist();

                            automatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.PreAuthSearch.GetStringValue(),
                                    "User should be directed back to Pre-Auth Search page if there is just one search result");

                            _preAuthSearch.GetGridViewSection.GetValueInGridByColRow(8)
                                .ShouldBeEqual(status, "Status should be updated to the newly set status in Pre-Auth Search grid");
                            _preAuthAction = _preAuthSearch.ClickOnAuthSeqToNavigateToPreAuthActionPage(currentPreAuth);

                            newCurrentStatus = _preAuthAction.GetUpperLeftQuadrantValueByLabel("Status");
                            newCurrentStatus.ShouldBeEqual(status, "The pre-auth should be assigned the new status after the transfer process");
                            _preAuthProcessingHistory = _preAuthAction.ClickOnPreAuthProcessingHistoryAndSwitch();

                            _preAuthProcessingHistory.GetGridValueByRowCol(1, 4).Contains($"Test Note for {status}")
                                .ShouldBeTrue("Is the entered note being displayed correctly in Pre-Auth Processing History");

                            _preAuthProcessingHistory.CloseClaimProcessingHistoryPageAndSwitchToPreAuthActionPage();
                        }
                    }
                }
                #endregion

            }
        }


        [Test]//CAR-1794(CAR-1661)
        public void Verify_access_Patient_Claim_History_from_PreAuth_Action_for_client_user()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                PreAuthActionPage _preAuthAction = null;
                PreAuthSearchPage _preAuthSearch;
                ClaimHistoryPage _claimHistory;

                _preAuthSearch = automatedBase.CurrentPage.NavigateToPreAuthSearch();
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);

                var preAuthID = paramLists["PreAuthID"];
                var preAuthSeq = paramLists["PreAuthSeq"];
                List<string> patprovTitle = new List<string>(2) {"Patient Seq", "Provider Seq"};
                List<string> patientHeader = new List<string>(2) {"PatientSequence", "ProviderSequence"};


                _preAuthSearch.UpdateStatusOfAuthSeqFromDatabase("V", preAuthSeq);
                _preAuthSearch.UpdateHciDoneOrClientDoneToFalseFromDatabase("clidone", preAuthSeq);

                try
                {
                    _preAuthSearch.GetSideBarPanelSearch.SelectDropDownListByIndex("Quick Search", 1);
                    _preAuthSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Pre-Auth ID", preAuthID);
                    _preAuthSearch.GetSideBarPanelSearch.ClickOnFindButton();
                    _preAuthSearch.WaitForWorkingAjaxMessage();
                    _preAuthAction = _preAuthSearch.ClickOnGridByRowColToPreAuthActionPage(2, 2, false);
                    _preAuthAction.GetWindowHandlesCount()
                        .ShouldBeEqual(1, "Patient Claim History Popup should not display");
                    _preAuthAction.GetHistoryIconToolTip().ShouldBeEqual("Patient Claim History");
                    var patprovseq = _preAuthAction.GetPatientProviderDetails(patprovTitle);
                    _preAuthAction.ClickOnHistoryIconToNavigateToPatientClaimHistoryPage();
                    _preAuthAction.GetWindowHandlesCount()
                        .ShouldBeEqual(2, "Patient Claim History Popup should display");
                    _claimHistory = _preAuthAction.SwitchToPatientClaimHistory();
                    _claimHistory.IsProviderHistoryTabSelected()
                        .ShouldBeTrue("ProviderHistory tab should be selected by default.");
                    var claimhistorypatprovseq = _claimHistory.GetPatientHistoryHeaderDetails(patientHeader);
                    _claimHistory.IsDataAvailableinProviderHistoryTab()
                        .ShouldBeTrue("Claims should be available in Provider History tab");
                    patprovseq.ShouldCollectionBeEqual(claimhistorypatprovseq, "List should match");
                    _preAuthAction = _claimHistory.SwitchBackToPreAuthActionPage();
                    _preAuthAction.ClickOnApproveIcon();
                    _preAuthAction.WaitForStaticTime(3000);
                    var patprovseq1 = _preAuthAction.GetPatientProviderDetails(patprovTitle);
                    _preAuthAction.GetWindowHandlesCount()
                        .ShouldBeEqual(2, "Patient Claim History Popup should not close");
                    _preAuthAction.ClickOnHistoryIconToNavigateToPatientClaimHistoryPage(true);
                    _preAuthAction.GetWindowHandlesCount().ShouldBeEqual(2,
                        "Existing Patient Claim History should be replaced by new one");
                    _claimHistory = _preAuthAction.SwitchToPatientClaimHistory(true);
                    var claimhistorypatprovseq1 = _claimHistory.GetPatientHistoryHeaderDetails(patientHeader);
                    _claimHistory.GetEmptyMessageText().ShouldBeEqual("No claims found.");
                    patprovseq1.ShouldCollectionBeEqual(claimhistorypatprovseq1,
                        "Patient and Provider Sequence of Pre-Auth Action page and Patient Claim History should match");
                    _preAuthAction = _claimHistory.SwitchBackToPreAuthActionPage();
                }
                finally
                {
                    _preAuthAction.CloseLastWindow();
                    _preAuthSearch.UpdateStatusOfAuthSeqFromDatabase("V", preAuthSeq);
                    _preAuthSearch.UpdateHciDoneOrClientDoneToFalseFromDatabase("clidone", preAuthSeq);

                }
            }
        }

        [Test] //CAR-1097
        [Order(1)]
        public void Verify_view_patient_claim_hx_in_pre_auth_action_for_client_user()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                PreAuthActionPage _preAuthAction = null;
                PreAuthSearchPage _preAuthSearch;

                _preAuthSearch = automatedBase.CurrentPage.NavigateToPreAuthSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var authSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "AuthSequence", "Value");
                var preAuthHistoryLabel = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName,
                    "HistoryLabel", "Value").Split(';').ToList();
                var proc = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                    "procCode",
                    "Value");
                var flag = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                    "flag", "Value");

                _preAuthSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                    "All Pre-Auths");
                _preAuthSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Auth Sequence", authSeq);
                _preAuthSearch.GetSideBarPanelSearch.ClickOnFindButton();
                _preAuthSearch.WaitForWorking();
                _preAuthAction = _preAuthSearch.ClickOnAuthSeqToNavigateToPreAuthActionPage(authSeq);

                var expectedValueList = _preAuthAction.PatientPreAuthHistoryList(authSeq);
                var expectedSummaryDetail = _preAuthAction.PatientPreAuthHistoryTotalList(authSeq);

                var totalBilled = expectedValueList.Select(r => decimal.Parse(Regex.Split(r[9], @"[^0-9\.]+")[1]))
                    .Sum();
                var totalAllowed = expectedValueList.Select(r => decimal.Parse(Regex.Split(r[10], @"[^0-9\.]+")[1]))
                    .Sum();
                var totalAdjPaid = expectedValueList.Select(r => decimal.Parse(Regex.Split(r[11], @"[^0-9\.]+")[1]))
                    .Sum();
                var totalSugPaid = expectedValueList.Select(r => decimal.Parse(Regex.Split(r[12], @"[^0-9\.]+")[1]))
                    .Sum();
                var totalSavings = expectedValueList.Select(r => decimal.Parse(Regex.Split(r[13], @"[^0-9\.]+")[1]))
                    .Sum();

                var expectedDetailList = expectedValueList.Select(r => string.Join(" ", r.ToArray())).ToList();
                var claimPatientHistory = _preAuthAction.ClickOnPreAuthClaimProcessingHistoryAndSwitch();
                claimPatientHistory.ClickOnPreAuthIconAndNavigateToPatientPreAuthHistory();
                claimPatientHistory.GetPreAuthHistoryTableLabel().ShouldCollectionBeEqual(preAuthHistoryLabel,
                    "The PreAuth History Table should contain following labels.");

                var actualDetailList = claimPatientHistory.GetPatientPreAuthHistoryRowValue();
                actualDetailList = actualDetailList.Select(x => Regex.Replace(x, "\r\n", " ")).ToList();

                expectedDetailList.Select(s=> s.Replace(" ","")).ShouldCollectionBeEqual(actualDetailList.Select(s=> s.Replace(" ","")), "Verification of Row details");

                var summaryDetails = claimPatientHistory.GetPatientPreAuthHistorySummary()
                    .Select(x => Regex.Replace(x, "\r\n", " ")).ToList();
                var expectedSummaryDetails =
                    expectedSummaryDetail[0][0] + " Auth Seq " + expectedSummaryDetail[0][1] + " Status " +
                    expectedSummaryDetail[0][2] + " $" + totalBilled + " $" + totalAllowed +
                    " $" + totalAdjPaid + " $" + totalSugPaid + " $" + totalSavings;
                summaryDetails[0].ShouldBeEqual(expectedSummaryDetails, "Verify Summary Details");

                var newPopupCode = claimPatientHistory.ClickProcCode("CDT Code", proc, 1);
                newPopupCode.GetProcCodeDescriptionFromPopUp().ShouldBeEqual(
                    Regex.Replace(expectedSummaryDetail[0][6], @"\s+", " "),
                    $"Proc Code Description should be equal to {expectedSummaryDetail[0][6]}.");
                claimPatientHistory =
                    newPopupCode.ClosePopupOnPatientPreAuthHistory("CDT Code" + proc);
                var flagPopupCode = claimPatientHistory.ClickFlag(flag);
                flagPopupCode.IsFlagTitleBold().ShouldBeTrue("Flag Title Should be bold black.");
                flagPopupCode.ClosePopupOnPatientPreAuthHistory("Flag" + flag);
                
            }
        }


        [NonParallelizable]
        [Test] //TE - 811 + CAR-3133 [CAR-3262]
        public void Verify_upload_new_document_in_pre_auth_action_for_Client_User()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                PreAuthActionPage _preAuthAction;
                PreAuthSearchPage _preAuthSearch;

                _preAuthSearch = automatedBase.CurrentPage.NavigateToPreAuthSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> testData =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var preAuthSequence = testData["PreAuthSequence"];
                var auditDate = testData["AuditDate"];
                var fileToUpload = testData["FileToUpload"];
                
                var expectedSelectedFileTypeList = new List<string>()
                    {
                        DentalAppealDocTypeEnum.ChartNotes.GetStringValue(), 
                        DentalAppealDocTypeEnum.Images.GetStringValue()
                    };

                var expectedSelectedFileType = $"{DentalAppealDocTypeEnum.ChartNotes.GetStringValue()},{DentalAppealDocTypeEnum.Images.GetStringValue()}";
                   
                var userName = _preAuthSearch.GetLoggedInUserFullName();
                var expectedDocumentHeaderText = testData["DocumentHeaderText"];

                List<string> expectedFileTypeList = new List<string>();
                foreach (DentalAppealDocTypeEnum fileTypeEnum in Enum.GetValues(typeof(DentalAppealDocTypeEnum)))
                {
                    var preAuthFileType = fileTypeEnum.GetStringValue();
                    expectedFileTypeList.Add(preAuthFileType);
                }

                _preAuthSearch.DeletePreAuthDocumentRecord(preAuthSequence, auditDate);

                _preAuthSearch.RefreshPage();

                _preAuthAction = _preAuthSearch.SearchByAuthSequenceAndNavigateToAuthAction(preAuthSequence, false);
                var status = _preAuthAction.GetPreAuthStatus();

                _preAuthAction.GetTitleOfUpperRightQuadrant()
                    .ShouldBeEqual("Documents", "Upper Quadrant Title Should Be Documents");

                _preAuthAction.GetDocumentUploadFormTitle()
                    .ShouldBeEqual(expectedDocumentHeaderText, "Form Header Should Match");
                _preAuthAction.IsUploadNewDocumentFormPresent()
                    .ShouldBeTrue("Upload Document Form Should Be Open By Default");
                _preAuthAction.IsAddIconDisabledInDocumentUploadForm().ShouldBeTrue("Add Icon Should Be Disabled");
                _preAuthAction.ClickOnCancelLinkOnUploadDocumentForm();

                _preAuthAction.IsUploadNewDocumentFormPresent().ShouldBeFalse("Upload Document Form Should Be Closed");
                _preAuthAction.IsAddIconDisabledInDocumentUploadForm().ShouldBeFalse("Add Icon Should Be Enabled");

                _preAuthAction.ClickOnAddIconDocumentUploadForm();

                Console.WriteLine("Verify Maximum character in description");
                var descp = new string('b', 105);
                _preAuthAction.GetFileUploadPage.SetFileUploaderFieldValue("Description", descp);
                _preAuthAction.GetFileUploadPage.GetFileUploaderFieldValue(2)
                    .Length.ShouldBeEqual(100, "Character length should not exceed more than 100 in Description");
                _preAuthAction.GetFileUploadPage.IsAddFileButtonDisabled()
                    .ShouldBeTrue(
                        "Add file button should be disabled (unless atleast a file has been uploaded) must be true?");
                _preAuthAction.GetFileUploadPage.ClickOnSaveUploadBtn();
                _preAuthAction.IsPageErrorPopupModalPresent()
                    .ShouldBeTrue("Page error pop up should be present if no file type is selected");
                _preAuthAction.GetPageErrorMessage()
                    .ShouldBeEqual("At least one file must be uploaded before the changes can be saved.",
                        "Expected error message on zero file type selection");
                _preAuthAction.ClosePageError();


                /* --------- add single file only since multiple file upload with selenium is successfull locally only -----*/
                _preAuthAction.GetFileUploadPage.AddFileForUpload(fileToUpload.Split(';')[0]);
                _preAuthAction.GetFileUploadPage.IsAddFileButtonDisabled()
                    .ShouldBeFalse(
                        "Add file button should be enabled (when atleast a file has been uploaded) must be false?");
                _preAuthAction.GetFileUploadPage.ClickOnAddFileBtn();
                _preAuthAction.IsPageErrorPopupModalPresent()
                    .ShouldBeTrue("Page error pop up should be present if no file type is selected");
                _preAuthAction.GetPageErrorMessage()
                    .ShouldBeEqual("At least one Document Type selection is required before the files can be added.",
                        "Expected error message on zero file type selection");
                _preAuthAction.ClosePageError();

                //CAR-3133 [CAR-3262]
                _preAuthAction.GetFileUploadPage.GetAvailableFileTypeList()
                .ShouldCollectionBeEqual(expectedFileTypeList, "File Type List Equal");
                _preAuthAction.GetFileUploadPage.SetFileTypeListVlaue(expectedSelectedFileTypeList[0]);
                _preAuthAction.GetFileUploadPage.GetPlaceHolderValue()
                    .ShouldBeEqual(expectedSelectedFileTypeList[0], "File Type Text");
                _preAuthAction.GetFileUploadPage.SetFileTypeListVlaue(expectedSelectedFileTypeList[1]);
                _preAuthAction.GetFileUploadPage.GetPlaceHolderValue()
                    .ShouldBeEqual("Multiple values selected", "File Type Text when multiple value selected");
                _preAuthAction.GetFileUploadPage.GetSelectedFileTypeList()
                    .ShouldCollectionBeEqual(expectedSelectedFileTypeList, "Selected File List Equal");
                _preAuthAction.GetFileUploadPage.SetFileUploaderFieldValue("Description", "Test Description");
                _preAuthAction.GetFileUploadPage.GetSelectedFilesValue()
                    .ShouldCollectionBeEqual(fileToUpload.Split(';').ToList(),
                        "Expected multiples files list is present");
                _preAuthAction.GetFileUploadPage.ClickOnAddFileBtn();
                _preAuthAction.GetFileUploadPage.ClaimFileToUploadDocumentValue(1, 2)
                    .ShouldBeEqual(fileToUpload.Split(';')[0], "Document correct and present under files to upload");
                _preAuthAction.GetFileUploadPage.ClaimFileToUploadDocumentValue(1, 3)
                    .ShouldBeEqual("Test Description",
                        "Document Description is correct and present under files to upload.");
                _preAuthAction.GetFileUploadPage.ClaimFileToUploadDocumentValue(1, 4)
                    .ShouldBeEqual("Multiple File Types", "Document Type text when multiple File Types is selected.");
                _preAuthAction.GetFileUploadPage.GetFileToUploadTooltipValue(1, 4)
                    .ShouldBeEqual(expectedSelectedFileType, "Document Type correct and present under files to upload");
                _preAuthAction.IsPageErrorPopupModalPresent()
                    .ShouldBeFalse(
                        "Page error pop up should not be present if no description is set as its not a required field");


                StringFormatter.PrintMessage("Verify Duplicate Files Can Not Be Added");
                _preAuthAction.GetFileUploadPage.SetFileTypeListVlaue(expectedSelectedFileTypeList[1]);
                _preAuthAction.GetFileUploadPage.SetFileUploaderFieldValue("Description", descp);
                _preAuthAction.GetFileUploadPage.AddFileForUpload(fileToUpload.Split(';')[0]);
                _preAuthAction.GetFileUploadPage.ClickOnAddFileBtn();

                _preAuthAction.IsPageErrorPopupModalPresent()
                    .ShouldBeTrue("Page error pop up should be present if no file type is selected");
                _preAuthAction.GetPageErrorMessage()
                    .ShouldBeEqual("Duplicate file is not added in the list.");
                _preAuthAction.ClosePageError();

                _preAuthAction.GetFileUploadPage.ClickOnSaveUploadBtn();
                _preAuthAction.GetFileUploadPage.IsDocumentDivPresent().ShouldBeTrue("Document List are present");
                _preAuthAction.IsUploadNewDocumentFormPresent()
                    .ShouldBeFalse("Upload New document div should be closed after uploading document.");
                _preAuthAction.GetFileUploadPage.IsFollowingAppealDocumentPresent(fileToUpload.Split(';')[0])
                    .ShouldBeTrue("Uploaded Document is listed");
                var expectedDate = _preAuthAction.GetFileUploadPage.AppealDocumentsListAttributeValue(1, 1, 3);


                StringFormatter.PrintMessageTitle(
                    "Validation of Audit Record to display in pre auth Processing History after document upload.");
                _preAuthAction.IsPreAuthAuditAddedForDocumentUpload(preAuthSequence)
                    .ShouldBeTrue("Claim Audit for Document Upload is added in database ");
                PreAuthProcessingHistoryPage preAuthProcessingHistory =
                    _preAuthAction.ClickOnPreAuthProcessingHistoryAndSwitch();
                var date = preAuthProcessingHistory.GetGridValueByRowCol(1, 1);
                // date.IsDateTimeInFormat().ShouldBeTrue("Upload Date time is in correct format");
                Convert.ToDateTime(date).Date
                    .ShouldBeEqual(Convert.ToDateTime(expectedDate).Date, "Correct Audit Date is displayed.");
                preAuthProcessingHistory.GetGridValueByRowCol(1, 3)
                    .ShouldBeEqual(status, " Correct Status is displayed.");
                var currentUser = preAuthProcessingHistory.GetGridValueByRowCol(1, 2);
                currentUser.DoesNameContainsOnlyFirstWithLastname()
                    .ShouldBeTrue("Username Should be in <FirstName> <LastName> (userid)");
                currentUser.ShouldBeEqual(userName, "Username Should be in <FirstName> <LastName> (userid)");
                preAuthProcessingHistory.GetGridValueByRowCol(1, 4)
                    .ShouldBeEqual("Document Uploaded", "Correct Note is displayed.");
                _preAuthAction.CloseAnyPopupIfExist();
                var existingDocCount = _preAuthAction.GetFileUploadPage.DocumentCountOfFileList();
                _preAuthAction.GetFileUploadPage.ClickOnClaimDocumentUploadIcon();
                _preAuthAction.GetFileUploadPage.IsDocumentUploadSectionPresent()
                    .ShouldBeTrue("Claim Document Uploader Section is displayed");
                _preAuthAction.GetFileUploadPage.SetFileTypeListVlaue(DentalAppealDocTypeEnum.ClaimImage.GetStringValue());
                _preAuthAction.GetFileUploadPage.AddFileForUpload(fileToUpload.Split(';')[0]);
                _preAuthAction.GetFileUploadPage.ClickOnAddFileBtn();
                var filesToUploadCount = _preAuthAction.GetFileUploadPage.GetFilesToUploadCount();
                _preAuthAction.GetFileUploadPage.ClaimFileToUploadDocumentValue(1, 2)
                    .ShouldBeEqual(fileToUpload.Split(';')[0], "Document correct and present under files to upload");
                _preAuthAction.GetFileUploadPage.ClickOnDeleteIconInFilesToUpload(1);
                _preAuthAction.GetFileUploadPage.GetFilesToUploadCount()
                    .ShouldBeLess(filesToUploadCount, "Document had been removed.");
                _preAuthAction.GetFileUploadPage.ClickOnCancelBtn();
                // selecting cancel closed form and discards added files
                _preAuthAction.GetFileUploadPage.IsDocumentDivPresent().ShouldBeTrue("Document List are present");
                _preAuthAction.GetFileUploadPage.DocumentCountOfFileList()
                    .ShouldBeEqual(existingDocCount,
                        "Added Files has been discarded and has not been associated to appeal");
            }
        }


        [Test, Category("Working")] //TE-811
        public void Verify_Multiple_Document_Can_Be_Uploaded_In_Pre_Auth_Action_for_Internal_User()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                PreAuthActionPage _preAuthAction;
                PreAuthSearchPage _preAuthSearch;

                _preAuthSearch = automatedBase.CurrentPage.NavigateToPreAuthSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> testData =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var preAuthSequence = testData["PreAuthSequence"];
                var fileToUpload = testData["FileToUpload"].Split(';');
                var fileType = testData["FileType"].Split(',');
                var description = testData["Description"].Split(';');
                var auditDate = testData["AuditDate"];
                _preAuthSearch.DeletePreAuthDocumentRecord(preAuthSequence, auditDate);
                _preAuthAction = _preAuthSearch.SearchByAuthSequenceAndNavigateToAuthAction(preAuthSequence, false);
                _preAuthAction.UploadPreAuthDocument(fileType[0], description[0], fileToUpload[0]);
                var filesToUploadCount = _preAuthAction.GetFileUploadPage.GetFilesToUploadCount();
                _preAuthAction.UploadPreAuthDocument(fileType[0], description[1], fileToUpload[1]);
                _preAuthAction.GetFileUploadPage.GetFilesToUploadCount().ShouldBeGreater(filesToUploadCount,
                    "Multiple Files Should Be Added Prior To Save");
                _preAuthAction.GetFileUploadPage.ClickOnSaveUploadBtn();
                StringFormatter.PrintMessage("Verify Database Values For Uploaded Document");
                var docUploadDataFromDb = _preAuthAction.GetDocumentUploadInformationFromDb(preAuthSequence);
                _preAuthAction.GetFileUploadPage.DocumentCountOfFileList().ShouldBeEqual(docUploadDataFromDb.Count,
                    " Uploaded Document Count Should Match");
                int i = 1;
                foreach (var row in docUploadDataFromDb)
                {
                    _preAuthAction.UploadedDocumentValueByRowColumn(i, 1, 2)
                        .ShouldBeEqual(row[0], "File Name Should Match");
                    Convert.ToDateTime(_preAuthAction.UploadedDocumentValueByRowColumn(i, 1, 4))
                        .ShouldBeEqual(Convert.ToDateTime(row[1]), "Date Should Match");
                    _preAuthAction.UploadedDocumentValueByRowColumn(i, 2, 2)
                        .ShouldBeEqual(row[2], "Description Should Match");
                    i++;
                }
            }
        }

        [Test] //TE-811
        public void Verify_view_delete_of_uploaded_documents_in_Pre_Auth_action_page_for_Client_User()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                PreAuthActionPage _preAuthAction;
                PreAuthSearchPage _preAuthSearch;

                _preAuthSearch = automatedBase.CurrentPage.NavigateToPreAuthSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var preAuthSequence = paramLists["PreAuthSequence"];
                var fileToUpload = paramLists["FileToUpload"].Split(';');
                var docDescription = paramLists["DocDescription"];
                var auditDate = paramLists["AuditDate"];
                
                _preAuthSearch.DeletePreAuthDocumentRecord(preAuthSequence, auditDate);
                _preAuthAction = _preAuthSearch.SearchByAuthSequenceAndNavigateToAuthAction(preAuthSequence, false);

                List<string> expectedSelectedFileTypeList = new List<string>();
                foreach (DentalAppealDocTypeEnum fileTypeEnum in Enum.GetValues(typeof(DentalAppealDocTypeEnum)))
                {
                    var preAuthFileType = fileTypeEnum.GetStringValue();
                    expectedSelectedFileTypeList.Add(preAuthFileType);
                }

                expectedSelectedFileTypeList.Sort();

                var existingDocCount = _preAuthAction.GetFileUploadPage.DocumentCountOfFileList();
                var preAuthStatus = _preAuthAction.GetPreAuthStatus();
                _preAuthAction.GetFileUploadPage.SetFileUploaderFieldValue("Description", docDescription);
                _preAuthAction.GetFileUploadPage.SetFileTypeListVlaue("All");

                _preAuthAction.GetFileUploadPage.AddFileForUpload(fileToUpload[0]);
                _preAuthAction.GetFileUploadPage.ClickOnAddFileBtn();
                _preAuthAction.GetFileUploadPage.ClickOnSaveUploadBtn();
                _preAuthAction.GetFileUploadPage.DocumentCountOfFileList()
                    .ShouldBeEqual(++existingDocCount, "New document has been added");
                _preAuthAction.GetFileUploadPage.IsFollowingAppealDocumentPresent(fileToUpload[0])
                    .ShouldBeTrue("Uploaded Document is listed");
                Console.WriteLine("Verify the Status of the claim after the document is uploaded");
                _preAuthAction.GetPreAuthStatus()
                    .ShouldBeEqual(preAuthStatus, "Verified Claim Status after Document Upload");

                _preAuthAction.IsPreAuthDocumentDeleteIconPresent()
                    .ShouldBeFalse("Pre Auth Document Delete Icon should present");

                _preAuthAction.GetFileUploadPage.AppealDocumentsListAttributeValue(1, 1, 1)
                    .ShouldBeEqual(fileToUpload[0], "Document filename is displayed");
                _preAuthAction.GetFileUploadPage.AppealDocumentsListAttributeValue(1, 1, 3)
                    .IsDateTimeInFormat()
                    .ShouldBeTrue("Document date is displayed and in proper format");
                _preAuthAction.GetFileUploadPage.AppealDocumentsListAttributeValue(1, 2, 1)
                    .ShouldBeEqual(docDescription, "Document Description is displayed");

                _preAuthAction.GetFileUploadPage.GetFileTypeAttributeListVaues(1, 1, 2)
                    .ShouldCollectionBeEqual(expectedSelectedFileTypeList, "Document fileType is displayed");
                _preAuthAction.GetFileUploadPage.GetFileTypeAttributeListToolTip(1, 1, 2)
                    .ShouldCollectionBeEqual(expectedSelectedFileTypeList, "Document fileType tooltip is displayed");
                
                _preAuthAction.GetFileUploadPage.IsEllipsisPresentInFileType(1)
                    .ShouldBeTrue("Is Ellipsis Present for lengthy values");

                _preAuthAction.ClickOnDocumentToViewAndStayOnPage(1); //window opens to view appeal document 
                _preAuthAction.GetOpenedDocumentText().ShouldBeEqual("test test", "document detail");
                _preAuthAction.CloseDocumentTabPageAndBackToPreAuthAction();

                _preAuthAction.IsAuditAddedForDocumentDownload(preAuthSequence)
                    .ShouldBeTrue("Audit for Document Download is added in database ");
            }
        }

        [Test] //TE-812
        public void Verify_hyperlink_from_TN_to_Tooth_history()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                PreAuthActionPage _preAuthAction;
                PreAuthSearchPage _preAuthSearch;
                ClaimHistoryPage _claimHistory;

                _preAuthSearch = automatedBase.CurrentPage.NavigateToPreAuthSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                _preAuthAction =
                    _preAuthSearch.SearchByAuthSequenceAndNavigateToAuthAction(paramLists["PreAuthSequence"], false);
                var toothRecordsAssociatedWithTNValueFromDb =
                    _preAuthAction.GetToothRecordsForTNValue(paramLists["PatSequence"], paramLists["ToothNo"]);
                _preAuthAction.IsTnHyperlinkPresent().ShouldBeTrue("Is TN value shown as hyperlink in line items?");
                _claimHistory = _preAuthAction.ClickOnTnLinkByRowToNavigateToPatientClaimHistoryPage();
                _preAuthAction.GetWindowHandlesCount()
                    .ShouldBeEqual(2, "Patient Claim History Popup Page Should Be Opened");
                _claimHistory.IsSelectedToothNumberCorrect(paramLists["ToothNo"])
                    .ShouldBeTrue($"Is TN {paramLists["ToothNo"]} selected?");

                StringFormatter.PrintMessage(
                    "Verify tooth records are filtered showing records associated with the TN value");
                int i = 1;
                foreach (var row in toothRecordsAssociatedWithTNValueFromDb)
                {
                    _claimHistory.GetPatientDentalHistoryData(i.ToString(), "1")
                        .ShouldBeEqual(row[0], "First Column should consist the value for DOS.");
                    _claimHistory.GetPatientDentalHistoryData(i.ToString(), "2").ShouldBeEqual(row[1],
                        "Second Column should consist the value for Proc.");
                    _claimHistory.GetPatientDentalHistoryData(i.ToString(), "3").ShouldBeEqual(
                        row[2].Replace("E SERIES", "..."),
                        "Third Column should consist the value for Proc Description.");
                    _claimHistory.GetPatientDentalHistoryData(i.ToString(), "4")
                        .ShouldBeEqual(row[3], "Fourth Column should consist the value for TN.");
                    _claimHistory.GetPatientDentalHistoryData(i.ToString(), "5")
                        .ShouldBeEqual(row[4], "Fifth Column should consist the value for TS.");
                    _claimHistory.GetPatientDentalHistoryData(i.ToString(), "6")
                        .ShouldBeEqual(row[5], "Sixth Column should consist the value for OC.");
                    _claimHistory.GetPatientDentalHistoryData(i.ToString(), "7").ShouldBeEqual(row[6],
                        "Seventh Column should consist the value for Allowed.");
                    _claimHistory.GetPatientDentalHistoryData(i.ToString(), "8").ShouldBeEqual(row[7],
                        "Eight Column should consist the value for Flag.");
                    i++;
                }

                _preAuthAction = _claimHistory.SwitchBackToPreAuthActionPage();
                _preAuthAction.CloseAnyTabIfExist();
            }

        }

        [Test] //TE-752
        public void Verify_red_badge_over_the_preauth_icon_represents_the_number_of_preauth_records_existing_for_patient_client()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                PreAuthActionPage _preAuthAction;
                PreAuthSearchPage _preAuthSearch;

                _preAuthSearch = automatedBase.CurrentPage.NavigateToPreAuthSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestExtensions.TestName);
                _preAuthSearch.UpdateProductStatusForClient(ProductEnum.DCA.ToString(), ClientEnum.SMTST.ToString(),
                    true);

                try
                {
                    StringFormatter.PrintMessage("Verify Pre-Auth icon is present for DCA Active Client");
                    _preAuthAction =
                        _preAuthSearch.SearchByAuthSequenceAndNavigateToAuthAction(paramLists["PreAuthSequence"],
                            false);
                    var preAuthCount = _preAuthAction.GetPreAuthCountFromDatabase(paramLists["PatSequence"]);
                    var claimPatientHistory = _preAuthAction.ClickOnPreAuthClaimProcessingHistoryAndSwitch();
                    claimPatientHistory.ClickOnPreAuthIconAndNavigateToPatientPreAuthHistory();
                    claimPatientHistory.IsPreAuthIconPresentInHeaderLevel().ShouldBeTrue("Is Pre Auth Icon Present ?");

                    StringFormatter.PrintMessage("Verify Red Badge Is Shown If Pre-Auth Record Is Present");
                    claimPatientHistory.IsRedBadgePresentOverPreAuthIcon()
                        .ShouldBeTrue("Is Red Badge Present Over Pre Auth Icon ?");
                    claimPatientHistory.GetPreAuthCountOnRedBadge()
                        .ShouldBeEqual(preAuthCount, "Pre Auth Count Should Match");

                    //StringFormatter.PrintMessage("Verify Badge Is Not Shown If No Pre-Auths Exist");
                    //claimPatientHistory.SwitchBackToPreAuthActionPage();
                    //_preAuthAction.CloseAnyPopupIfExist();
                    //_preAuthAction.ClickOnReturnToPreAuthSearch();
                    //_preAuthSearch.SearchByAuthSequenceAndNavigateToAuthAction(paramLists["PreAuthWithoutAnyRecord"], false);
                    //claimPatientHistory = _preAuthAction.ClickOnPreAuthClaimProcessingHistoryAndSwitch();
                    //claimPatientHistory.ClickOnPreAuthIconAndNavigateToPatientPreAuthHistory();
                    //claimPatientHistory.IsPreAuthIconPresentInHeaderLevel().ShouldBeTrue("Is Pre Auth Icon Present ?");
                    //claimPatientHistory.IsRedBadgePresentOverPreAuthIcon().ShouldBeFalse("Is Red Badge Present Over Pre Auth Icon ?");

                    StringFormatter.PrintMessage("Verify Pre Auth Icon Is Not Present For DCA Inactive client");
                    _preAuthSearch.UpdateProductStatusForClient(ProductEnum.DCA.ToString(), ClientEnum.SMTST.ToString(),
                        false);
                    claimPatientHistory.SwitchBackToPreAuthActionPage();
                    _preAuthAction.CloseAnyTabIfExist();
                    claimPatientHistory = _preAuthAction.ClickOnPreAuthClaimProcessingHistoryAndSwitch();
                    claimPatientHistory.IsPreAuthIconPresentInHeaderLevel().ShouldBeFalse("Is Pre Auth Icon Present ?");
                    _preAuthAction.CloseAnyTabIfExist();
                }
                finally
                {
                    _preAuthSearch.UpdateProductStatusForClient(ProductEnum.DCA.ToString(), ClientEnum.SMTST.ToString(),
                        true);
                }
            }
        }

        [Test]//TE-810 + CAR-2887(CAR-2846)
        public void Verify_creation_of_PreAuth_Notes()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                PreAuthActionPage _preAuthAction;
                PreAuthSearchPage _preAuthSearch;

                _preAuthSearch = automatedBase.CurrentPage.NavigateToPreAuthSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                const string notes = "Test Note for PreAuth";
                var authSeq = paramLists["PreAuthSeq"];
                var lengthyNote = new string('a', 4494);


                _preAuthSearch.DeletePreAuthNote(authSeq);
                _preAuthAction = _preAuthSearch.SearchByAuthSequenceAndNavigateToAuthAction(authSeq, false);
                var username = _preAuthAction.GetLoggedInUserFullName();


                StringFormatter.PrintMessage("verify validation message correct?");
                _preAuthAction.IsAddNoteIconPresent().ShouldBeTrue("note icon present?");
                _preAuthAction.ClickonAddNoteIcon();
                _preAuthAction.IsNoteContainerPresent().ShouldBeTrue("Notes container Displayed?");
                _preAuthAction.GetPreAuthNoteType().ShouldBeEqual("Pre-Auth", "Displayed note type correct?");


                StringFormatter.PrintMessageTitle("Verify Validation and Cancel link working?");
                _preAuthAction.ClickOnSaveButtonInNoteEditorByRow(1);
                _preAuthAction.IsPageErrorPopupModalPresent().ShouldBeTrue("validation for note message displayed?");
                _preAuthAction.GetPageErrorMessage()
                    .ShouldBeEqual("Invalid or missing data must be resolved before record can be saved.");
                _preAuthAction.ClosePageError();

                _preAuthAction.SetLengthyNoteToVisbleTextarea("note",lengthyNote, handlePopup:false);
                _preAuthAction.IsPageErrorPopupModalPresent().ShouldBeTrue($"{lengthyNote.Length - 1} values allowed");
                _preAuthAction.GetPageErrorMessage().ShouldBeEqual("Note value is too long.",
                    "Is validation message for lengthy note Equals?");
                _preAuthAction.ClosePageError();

                _preAuthAction.ClickOnAddNoteCancelLink();
                _preAuthAction.IsNoteContainerPresent()
                    .ShouldBeFalse("Notes container Displayed after clicking cancel?");

                StringFormatter.PrintMessageTitle("Addition of New note");
                _preAuthAction.ClickonAddNoteIcon();
                _preAuthAction.InputNotes(notes);
                _preAuthAction.WaitForStaticTime(3000);
                _preAuthAction.ClickonAddNoteSaveButton();
                _preAuthAction.GetNoteListCount().ShouldBeEqual(1, "Any notes created?");
                _preAuthAction.IsRedBadgeNoteIndicatorPresent()
                    .ShouldBeTrue("Red badge icon present with the note count?");
                _preAuthAction.NoOfPreAauthNotes().ShouldBeEqual("1", "Number of notes correct?");
                VerifySavedPreAuthNotes(_preAuthAction, notes, username);
            }
        }


        [Test] //TE-810
        public void Verify_Edit_Of_existing_PreAuth_Notes()
        {

            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                PreAuthActionPage _preAuthAction;
                PreAuthSearchPage _preAuthSearch;

                _preAuthSearch = automatedBase.CurrentPage.NavigateToPreAuthSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var editNotes = "Edit Test Note for PreAuth";
                var authSeq = paramLists["PreAuthSeq"];
                var nonCurrentUser = paramLists["UserName"];

                StringFormatter.PrintMessage("Verify edit of existing notes");
                _preAuthAction = _preAuthSearch.SearchByAuthSequenceAndNavigateToAuthAction(authSeq);
                _preAuthAction.ClickNotesIcon();

                StringFormatter.PrintMessage("Verify details of existing notes");
                var notesListDetailFromDb = _preAuthAction.GetNotesDetailFromDb(authSeq);
                for (var i = 0; i <= notesListDetailFromDb.Count - 1; i++)
                {
                    _preAuthAction.GetGridNoteDetailByCol(i + 1, 2)
                        .ShouldBeEqual(notesListDetailFromDb[i][0], "note type correct?");
                    _preAuthAction.GetGridNoteDetailByCol(i + 1, 4)
                        .ShouldBeEqual(notesListDetailFromDb[i][1], "note creator correct?");
                    _preAuthAction.GetGridNoteDetailByCol(i + 1, 5)
                        .ShouldBeEqual(notesListDetailFromDb[i][2], "note created date correct?");

                }


                StringFormatter.PrintMessage("verify edit of existing notes");
                _preAuthAction.ClickOnEditIconOnNotesByRow(1);
                _preAuthAction.IsNoteEditFormDisplayedByRow(1).ShouldBeTrue("Note edit form displayed?");
                var notes = _preAuthAction.GetInputValueOfNotes();
                if (notes == editNotes)
                    editNotes = "New Edit Test Note for PreAuth";
                _preAuthAction.ClickOnCancelButtonInNoteEditorByRow(1);
                _preAuthAction.IsNoteEditFormDisplayedByRow(1).ShouldBeFalse("Note edit form displayed?");
                _preAuthAction.ClickOnEditIconOnNotesByRow(1);
                _preAuthAction.GetInputValueOfNotes().ShouldBeEqual(notes, "Verify saved Note displayed?");
                _preAuthAction.InputNotes(editNotes);
                _preAuthAction.WaitForStaticTime(3000);
                _preAuthAction.ClickOnSaveButtonInNoteEditorByRow(1);
                VerifySavedPreAuthNotes(_preAuthAction, editNotes, _preAuthAction.GetLoggedInUserFullName());


                StringFormatter.PrintMessage("Verify caret sign and check notes is not editable");
                _preAuthAction.ClickOnCollapseIconOnNotesByRow(2);
                _preAuthAction.IsNoteEditFormDisplayedByRow(2)
                    .ShouldBeTrue("Clicking on Expand icon again must close note form.");
                _preAuthAction.IsNoteFormEditableByName(nonCurrentUser)
                    .ShouldBeFalse("Notes form must not be editable");
            }
        }

        //[Test] //TE-877
        public void Verify_Deleted_Flags_Are_Visible_To_Client_Users()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                PreAuthActionPage _preAuthAction;
                PreAuthSearchPage _preAuthSearch;

                _preAuthSearch = automatedBase.CurrentPage.NavigateToPreAuthSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var authSeq = paramLists["PreAuthSeq"];
                var authSeqWithSystemDeletedFlag = paramLists["PreAuthSeqWithSystemDeletedFlag"];
                var flagSeq = paramLists["FlagSeq"];
                var deletedFlag = paramLists["DeletedFlag"];
                var systemDeletedFlag = paramLists["SystemDeletedFlag"];
                StringFormatter.PrintMessage("Delete flag by Internal user");
                _preAuthSearch.DeleteRestorePreAuthFlagByInternalUser(flagSeq);
                try
                {
                    _preAuthAction = _preAuthSearch.SearchByAuthSequenceAndNavigateToAuthAction(authSeq, false);
                    StringFormatter.PrintMessage("Verify Deleted Flag Is Present In Flagged Lines For Client User");
                    _preAuthAction.IsDeletedFlagPresentByFlagName(deletedFlag)
                        .ShouldBeTrue("Deleted Flag Should Be Present In Pre Auth Action Page");

                    StringFormatter.PrintMessage("Verify Audit Record For Deleted Flag Is Present for Client User");
                    _preAuthAction.ClickOnLineItemOrFlagAuditHistoryIconByTitle("Flag Audit History");
                    _preAuthAction.IsAuditPresentForDeletedFlag(deletedFlag)
                        .ShouldBeTrue("Audit Should Be Present In Flag Audit History For Deleted Flag");

                    StringFormatter.PrintMessage(
                        "Verify System Deleted Flag Is Not Visible In Flagged Lines For Client User");
                    _preAuthAction.ClickOnReturnToPreAuthSearch()
                        .SearchByAuthSequenceAndNavigateToAuthAction(authSeqWithSystemDeletedFlag);
                    _preAuthAction.IsDeletedFlagPresentByFlagName(systemDeletedFlag)
                        .ShouldBeFalse("System Deleted Flag Should Not Be Present In Pre Auth Action Page");
                    _preAuthAction.ClickOnLineItemOrFlagAuditHistoryIconByTitle("Flag Audit History");
                    _preAuthAction.IsAuditPresentForDeletedFlag(systemDeletedFlag)
                        .ShouldBeFalse("Audit Should Not Be Present In Flag Audit History For Deleted Flag");

                }
                finally
                {
                    _preAuthSearch.DeleteRestorePreAuthFlagByInternalUser(flagSeq, false);
                }
            }
        }

        [Test] //CAR-2957(CAR-2928)
        public void Verify_flag_description_in_flag_Description_pop_up_preAuth_Client()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                PreAuthActionPage _preAuthAction;
                PreAuthSearchPage _preAuthSearch;

                _preAuthSearch = automatedBase.CurrentPage.NavigateToPreAuthSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                string preAuthSeq = paramLists["PreAuthSequence"];
                string flagType = paramLists["FlagType"];
                string clientAutoReviewValue = paramLists["ClientAutoReviewValue"];
                string flagDescriptionValue = paramLists["FlagDescriptionValue"];

                _preAuthAction = _preAuthSearch.SearchByAuthSequenceAndNavigateToAuthAction(preAuthSeq);

                var flag = _preAuthAction.GetFlagRowDetailValue(col: 2);
                var flagShortDescription = _preAuthAction.GetEOBMessageFromDatabase(flag, "S");
                var flagPopupPage =
                    _preAuthAction.ClickOnFlagandSwitch("Flag Information - " + flag, isInternalUser: false);
                flagPopupPage.GetPopupHeaderText().ShouldBeEqual($"Flag - {flag}", "Popup header should match");
                flagPopupPage.GetTextValueinLiTag(1)
                    .ShouldBeEqual(flagShortDescription, "Short flag description should match");
                flagPopupPage.GetTextValueWithInTag(tag: "").Replace("\r\n", " ")
                    .ShouldBeEqual(flagType, "Flag Type should match");
                flagPopupPage.GetTextValueWithinSpanTag(4)
                    .ShouldBeEqual(clientAutoReviewValue, "Client Auto Review value should match");
                flagPopupPage.GetTextValueWithInTag(5, ">span")
                    .ShouldBeEqual(flagDescriptionValue, "Flag description label should be present");
                flagPopupPage.GetTextValueWithInTag(5, "").Replace($"{flagDescriptionValue}\r\n", "")
                    .ShouldBeEqual(flagShortDescription, "Flag description should match");
            }
        }


        [Test] //CAR-3083[CAR-3029]
        public void Verify_EOB_message_when_sug_code_is_added_preAuth_Client()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                PreAuthActionPage _preAuthAction;
                PreAuthSearchPage _preAuthSearch;

                _preAuthSearch = automatedBase.CurrentPage.NavigateToPreAuthSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                string preAuthSeq = paramLists["PreAuthSequence"];
                string sugCode = paramLists["SugCode"];
                var lineNo = paramLists["LineNo"];
                var flag = paramLists["Flag"];

                _preAuthAction = _preAuthSearch.SearchByAuthSequenceAndNavigateToAuthAction(preAuthSeq);
                var message = _preAuthAction.GetEOBMessageFromDatabase(flag);
                var lineProcCodeDesc = _preAuthAction.GetProcDescriptionTextOfByLineNo();
                var lineProcCode = _preAuthAction.GetProcDescriptionByLineNo();
                var editSugProcCodeDesc = _preAuthAction.GetShortProcCodeDescriptionFromDB(sugCode);
                var expectedMessage = String.Format(message, lineNo, lineProcCode,
                    lineProcCodeDesc,
                    sugCode, editSugProcCodeDesc).Trim();
                _preAuthAction.GetEobMessageByLineNumber()
                    .ShouldBeEqual(expectedMessage, "EOB message should match");
            }
        }

        [Test] //CAR-3178(CAR-3107)
        [Author("Pujan Aryal")]
        public void Verify_Trigger_Proc_Code_And_Description_Is_Shown_In_EOB_Message()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                PreAuthSearchPage preAuthSearch = automatedBase.CurrentPage.NavigateToPreAuthSearch();
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                string preAuthSeq = paramLists["PreAuthSequence"];
                var lineNo = paramLists["LineNo"];
                var trigLineNo = paramLists["TrigLineNo"];
                var lineNoNoValueFound = paramLists["LineNoForNoValueFound"];
                var trigLineNoNoValueFound = paramLists["TrigLineNoForNoValueFound"];
                var flags = paramLists["Flags"].Split(',').ToList();

                StringFormatter.PrintMessage("Verify the EOB message shows the correct information if trigger line proc code and it's description exist");
                PreAuthActionPage preAuthAction = preAuthSearch.SearchByAuthSequenceAndNavigateToAuthAction(preAuthSeq);
                var message = preAuthAction.GetEOBMessageFromDatabase(flags[0]);
                var lineProcCodeDesc = preAuthAction.GetProcDescriptionTextOfByLineNo();
                var trigProcCodeDesc = preAuthAction.GetProcDescriptionTextOfByLineNo(2);
                var trigProcCode = preAuthAction.GetTriggerProcCodeFromDatabaseByLinNoAndFlag(preAuthSeq,lineNo,flags[0]);
                var lineProcCode = preAuthAction.GetProcDescriptionByLineNo();
                var expectedMessage = Format(message, lineNo, lineProcCode, lineProcCodeDesc, trigProcCode, trigProcCodeDesc,trigLineNo).Trim();

                preAuthAction.GetEobMessageByLineNumber().ShouldBeEqual(expectedMessage, "EOB message should match");

                StringFormatter.PrintMessage("Verify if trigger line proc code description does not exist, 'No value found' will be shown for the description");
                message = preAuthAction.GetEOBMessageFromDatabase(flags[0]);
                lineProcCodeDesc = preAuthAction.GetProcDescriptionTextOfByLineNo(4);
                trigProcCode = preAuthAction.GetTriggerProcCodeFromDatabaseByLinNoAndFlag(preAuthSeq, lineNoNoValueFound, flags[0]);
                trigProcCodeDesc = preAuthAction.GetProcDescriptionTextOfByLineNo(5);

                if (trigProcCodeDesc=="")
                {
                    trigProcCodeDesc = "No value found";
                }

                lineProcCode = preAuthAction.GetProcDescriptionByLineNo(4);
                expectedMessage = Format(message, lineNoNoValueFound, lineProcCode, lineProcCodeDesc, trigProcCode, trigProcCodeDesc, trigLineNoNoValueFound).Trim();
                preAuthAction.GetEobMessageByLineNumber(4).ShouldBeEqual(expectedMessage, "EOB message should match when triggered line proc code description does not exist");
            }
        }

        [Test] //CAR-3200(CAR-3183)
        [Author("Pujan Aryal")]
        [NonParallelizable]
        public void Verify_TN_TS_And_OC_Are_Shown_In_The_Patient_Pre_Auth_History_Pop_Up()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                PreAuthSearchPage _preAuthSearch = automatedBase.QuickLaunch.NavigateToPreAuthSearch();
                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestName);
                string preAuthSeq = paramLists["PreAuthSequence"];
                var toothInfo = paramLists["ToothInfo"].Split(',').ToList();
                _preAuthSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                    "All Pre-Auths");
                PreAuthActionPage _preAuthAction =
                    _preAuthSearch.SearchByAuthSequenceAndNavigateToAuthAction(preAuthSeq);
                var tnDescription = _preAuthAction.GetShortProcCodeDescriptionFromDB(toothInfo[0], "DTOO");
                var ocDescription = _preAuthAction.GetShortProcCodeDescriptionFromDB(toothInfo[2], "ORAL");

                try
                {
                    var claimHistory = _preAuthAction.ClickOnPreAuthClaimProcessingHistoryAndSwitch();
                    claimHistory.WaitForStaticTime(2000);
                    claimHistory.ClickOnPreAuthIconAndNavigateToPatientPreAuthHistory();

                    StringFormatter.PrintMessage("Verify For every pre-auth line record TN,TS and OC columns are present");
                    for (int i = 1; i <= claimHistory.GetPatientPreAuthHistoryRowCount(); i++)
                    {
                        claimHistory.IsColumnNamePresentByRow("TN", i);
                        claimHistory.IsColumnNamePresentByRow("TS", i);
                        claimHistory.IsColumnNamePresentByRow("OC", i);
                    }

                    StringFormatter.PrintMessage("Verify values shown in the TN and OC columns will have tool-tips showing the descriptions.");
                    claimHistory.MouseOverPatientPreAuthTableAndGetToolTipString(2, 5).ShouldBeEqual(tnDescription, "Value shown in the TN column will have tool-tip showing the description");
                    claimHistory.MouseOverPatientPreAuthTableAndGetToolTipString(2, 7).ShouldBeEqual(ocDescription, "Value shown OC column will have tool-tip showing the description");

                    StringFormatter.PrintMessage("Verify When values are modified through Pre-Auth Action, the new values will be reflected in the history page view.");
                    claimHistory.GetPatientPreAuthHistoryTableValueByRowAndColumn(2, 5).ShouldBeEqual(toothInfo[0], "TN Value modified through Pre-Auth Action, should be reflected in the history page view.");
                    claimHistory.GetPatientPreAuthHistoryTableValueByRowAndColumn(2, 6).ShouldBeEqual(toothInfo[1], "TS Value modified through Pre-Auth Action, should be reflected in the history page view.");
                    claimHistory.GetPatientPreAuthHistoryTableValueByRowAndColumn(2, 7).ShouldBeEqual(toothInfo[2], "OC Value modified through Pre-Auth Action, should be reflected in the history page view.");

                    StringFormatter.PrintMessage("Verify scroll bar is present on Patient Tooth History view");
                    claimHistory.ClickDentalHistoryTab();
                    claimHistory.ResizeWindow(1200, 764);
                    claimHistory.IsVerticalScrollBarPresentInPatientToothHistoryTab().ShouldBeTrue("Verify scroll bar is present on Patient Tooth History view");

                }
                finally
                {
                    _preAuthAction.CloseAnyPopupIfExist();
                }
            }
        }

        #endregion

        #region PRIVATE METHODS



        public void VerifySavedPreAuthNotes(PreAuthActionPage _preAuthAction, string note, string username, int row = 1)
        {
            StringFormatter.PrintMessage("Verify created notes:");
            _preAuthAction.GetGridNoteDetailByRowandCol(row, 2).ShouldBeEqual("Pre-Auth", "Correct claim type recorded?");
            _preAuthAction.GetGridNoteDetailByRowandCol(row, 4).ShouldBeEqual(username,
                "Correct username recorded?");
            _preAuthAction.GetGridNoteDetailByRowandCol(row, 5).ShouldBeEqual(_preAuthAction.CurrentDateInMstStandard(null), "Correct date recorded?");
            _preAuthAction.ClickOnEditIconOnNotesByRow(row);
            _preAuthAction.GetInputValueOfNotes().ShouldBeEqual(note, "Is Note Equal?");
            _preAuthAction.ClickOnCancelButtonInNoteEditorByRow(row);

        }
       
        private void VerifyStatusDropdownInTransferForm(PreAuthActionPage _preAuthAction, object currentStatus,
            List<string> currentStatusList)
        {
            #region Status List Declaration And Initialization

            Dictionary<string, string> documentsRequestedList = new Dictionary<string, string>
            {
                [StatusEnum.DocumentReview.GetStringDisplayValue()] = "Documents are ready for Cotiviti review"
            };

            Dictionary<string, string> closedList = new Dictionary<string, string>
            {
                [StatusEnum.DocumentsRequested.GetStringDisplayValue()] = "Documents have been requested from the provider",
                [StatusEnum.DocumentReview.GetStringDisplayValue()] = "Documents are ready for Cotiviti review"
            };

            Dictionary<string, string> documentsRequiredList = new Dictionary<string, string>
            {
                [StatusEnum.DocumentsRequested.GetStringDisplayValue()] = "Documents have been requested from the provider",
                [StatusEnum.DocumentReview.GetStringDisplayValue()] = "Documents are ready for Cotiviti review"
            };
            #endregion

            List<string> tooltipList;

            switch (currentStatus)
            {
                case String status when (status == StatusEnum.DocumentsRequired.GetStringDisplayValue()):
                    currentStatusList.ShouldCollectionBeEqual(documentsRequiredList.Select(s => s.Key).ToList(), $"Is the status list correct when current status is : {currentStatus} ?");
                    tooltipList = _preAuthAction.GetStatusToolTipInPreAuthTransferForm();
                    tooltipList.RemoveAt(0);
                    tooltipList.ShouldCollectionBeEqual(documentsRequiredList.Select(s => s.Value).ToList(), "Is Tooltip message correct for all status ? ");
                    break;

                case String status when (status == StatusEnum.DocumentsRequested.GetStringDisplayValue()):
                    currentStatusList.ShouldCollectionBeEqual(documentsRequestedList.Select(s => s.Key).ToList(), $"Is the status list correct when current status is : {currentStatus} ?");
                    tooltipList = _preAuthAction.GetStatusToolTipInPreAuthTransferForm();
                    tooltipList.RemoveAt(0);
                    tooltipList.ShouldCollectionBeEqual(documentsRequestedList.Select(s => s.Value).ToList(), "Is Tooltip message correct for all status ? ");
                    break;
                case String status when (status == StatusEnum.Closed.GetStringDisplayValue()):
                    currentStatusList.ShouldCollectionBeEqual(closedList.Select(s => s.Key).ToList(), $"Is the status list correct when current status is : {currentStatus} ?");
                    tooltipList = _preAuthAction.GetStatusToolTipInPreAuthTransferForm();
                    tooltipList.RemoveAt(0);
                    tooltipList.ShouldCollectionBeEqual(closedList.Select(s => s.Value).ToList(), "Is Tooltip message correct for all status ? ");
                    break;
                default:
                    throw new AssertionException($" Current Status {currentStatus} is invalid ");
            }
        }

        private Dictionary<string, List<string>> VerifyNextPreAuthDetailsAndReturnFlagDetails(
            PreAuthActionPage _preAuthAction, Dictionary<string, List<string>> expectedPreAuthDetailsFromSearchPage)
        {
            var flagDetails = new Dictionary<string, List<string>>();

            int count;
            for (count = 0; count < 3; count++)
            {
                var currentPreAuthSeq = expectedPreAuthDetailsFromSearchPage.Keys.ElementAt(count + 1);
                _preAuthAction.ClickOnNextIconAndCheckIfNextIconIsDisabled().ShouldBeTrue("Is 'Next' icon disabled while 'Working' message is displayed ?");
                _preAuthAction.WaitForWorkingAjaxMessage();

                var preAuthSeqFromPreAuthActionPage = _preAuthAction.GetUpperLeftQuadrantValueByLabel("Auth Seq");
                preAuthSeqFromPreAuthActionPage.ShouldBeEqual(currentPreAuthSeq);

                var preAuthDetails = expectedPreAuthDetailsFromSearchPage[preAuthSeqFromPreAuthActionPage];

                _preAuthAction.GetUpperLeftQuadrantValueByLabel("Review Type").ShouldBeEqual(preAuthDetails[0], "Is 'Review Type' as expected when 'Next' icon is clicked ?");
                _preAuthAction.GetUpperLeftQuadrantValueByLabel("Patient Seq").ShouldBeEqual(preAuthDetails[1], "Is the 'Patient Seq' Seq as expected when 'Next' icon is clicked ?");
                _preAuthAction.GetUpperLeftQuadrantValueByLabel("Provider Seq").ShouldBeEqual(preAuthDetails[2], "Is the 'Provider Seq' as expected when 'Next' icon is clicked ?");
                _preAuthAction.GetUpperLeftQuadrantValueByLabel("State").ShouldBeEqual(preAuthDetails[4], "Is the 'State' as expected when 'Next' icon is clicked ?");
                _preAuthAction.GetUpperLeftQuadrantValueByLabel("Status").ShouldBeEqual(preAuthDetails[5], "Is the 'Status' as expected when 'Next' icon is clicked ?");

                var countOfDeletedFlags = _preAuthAction.GetCountOfDeletedFlags().ToString();
                var preAuthStatus = _preAuthAction.GetUpperLeftQuadrantValueByLabel("Status");

                flagDetails.Add(currentPreAuthSeq, new List<string>()
                {
                    countOfDeletedFlags , preAuthStatus
                });

            }
            _preAuthAction.ClickOnReturnToPreAuthSearch();
            return flagDetails;
        }

        private void VerifyTopFlagOrderAndSavings(PreAuthActionPage _preAuthAction, int lineNo, List<string> flagsList,
            List<string> flagSavingsList, bool delete = true)
        {
            var flagToBeDeletedOrRestored = flagsList[0];
            var updatedTopFlag = flagsList[1];
            var savingForDeletedOrRestoredFlag = flagSavingsList[0];
            var savingsForUpdatedTopFlag = flagSavingsList[1];

            _preAuthAction.ClickOnLineItemOrFlagAuditHistoryIconByTitle("Line Items");

            if (delete)
            {
                StringFormatter.PrintMessage("Verifying deleted top flag is not being shown as top flag");
                var topFlag = _preAuthAction.GetFlagRowDetailValue(lineNo, 1, 2);
                topFlag.ShouldNotBeEqual(flagToBeDeletedOrRestored, "Top flag should not be shown as the top flag in Flagged Lines container once the flag is deleted");
                topFlag.ShouldBeEqual(updatedTopFlag, "Is top flag adjusted so that the next top flag on the line is displayed first?");

                StringFormatter.PrintMessage("Verifying flag order is updated once top flag is deleted");
                int i = 0;
                for (; i < 2; i++)
                {
                    _preAuthAction.GetFlagRowDetailValue(lineNo, i + 1, 2).ShouldBeEqual(flagsList[i + 1]);
                }

                _preAuthAction.GetFlagRowDetailValue(lineNo, 3, 2).ShouldBeEqual(flagsList[0]);
                var listOfFlagsInLineItems = _preAuthAction.GetLineItemsValueListByRowColumn(1, 2);

                StringFormatter.PrintMessage("Verifying whether top flag adjusted automatically in Line Items once flag is deleted");
                listOfFlagsInLineItems[2].ShouldBeEqual(updatedTopFlag, "Is top flag adjusted automatically in Line Items once flag is deleted ?");

                _preAuthAction.GetFlagLineLevelHeaderDetailValue(lineNo, 2, 3).ShouldBeEqual(savingsForUpdatedTopFlag);

            }

            else
            {
                StringFormatter.PrintMessage("Verifying restored top flag is being shown as top flag");
                var topFlag = _preAuthAction.GetFlagRowDetailValue(lineNo, 1, 2);
                topFlag.ShouldBeEqual(flagToBeDeletedOrRestored, "Top flag should be shown as the top flag in Flagged Lines container once the flag is restored");

                StringFormatter.PrintMessage("Verifying flag order is updated once top flag is restored");
                int i = 0;
                for (; i < 3; i++)
                {
                    _preAuthAction.GetFlagRowDetailValue(lineNo, i + 1, 2).ShouldBeEqual(flagsList[i]);
                }

                var listOfFlagsInLineItems = _preAuthAction.GetLineItemsValueListByRowColumn(1, 2);

                StringFormatter.PrintMessage("Verifying whether top flag adjusted automatically in Line Items once flag is restored");
                listOfFlagsInLineItems[2].ShouldBeEqual(flagToBeDeletedOrRestored, "Is top flag adjusted automatically in Line Items once flag is deleted ?");

                _preAuthAction.GetFlagLineLevelHeaderDetailValue(lineNo, 2, 3).ShouldBeEqual(savingForDeletedOrRestoredFlag);
            }

        }

        private void VerifyDeleteRestoreOfFlags(PreAuthActionPage _preAuthAction, string authSeq, int lineNo,
            string flagName, bool delete = true)
        {
            // Storing the initial Savings value before either deleting or restoring
            if (delete)
            {
                StringFormatter.PrintMessage("Verifying Deleting a flag");
                _preAuthAction.DeleteRestoreFlagByLineNoAndFlagName(lineNo, flagName);
                var hciDoneAndCliDoneFromDB = _preAuthAction.GetHciDoneAndCliDoneValuesByAuthSeqLinNoFlagName(authSeq, lineNo, flagName);
                hciDoneAndCliDoneFromDB[0][0].ShouldBeEqual(hciDoneAndCliDoneFromDB[0][1]).ShouldBeEqual("T", "Are HciDone and CliDone = 'T' after deleting flag?");

                var countOfFlagsInDiv = _preAuthAction.GetFlagListByDivList(3).Count;
                _preAuthAction.IsFlagRowDeletedPresent(3, countOfFlagsInDiv).ShouldBeTrue("Is Flag row deleted after deleting the flag ?");
            }

            else
            {
                StringFormatter.PrintMessage("Verifying Restoring a flag");
                _preAuthAction.DeleteRestoreFlagByLineNoAndFlagName(lineNo, flagName, false);
                var hciDoneAndCliDoneFromDB = _preAuthAction.GetHciDoneAndCliDoneValuesByAuthSeqLinNoFlagName(authSeq, lineNo, flagName);
                hciDoneAndCliDoneFromDB[0][0].ShouldBeEqual(hciDoneAndCliDoneFromDB[0][1].ShouldBeEqual("T"));
            }
        }

        private void VerificationOfFlagAuditHistory(PreAuthActionPage _preAuthAction, int lineNo, int auditRecordRow,
            IDictionary<string, string> paramsList, bool delete = true)
        {
            var listCounter = 0;
            IDictionary<string, string> flagAuditHistoryDetails = new Dictionary<string, string>();
            var modifiedBy = paramsList["ModifiedBy"];
            var userType = paramsList["UserType"];
            var deleteActionText = paramsList["DeleteActionText"];
            var restoreActionText = paramsList["RestoreActionText"];
            var reasonCode = paramsList["ReasonCode"];

            List<string> listOfFlagAuditHistoryLabels = new List<string>(new string[]
            {
                "Mod Date",
                "By",
                "User Type",
                "Action",
                "Reason Code",
                "Notes"
            });

            _preAuthAction.ClickOnLineItemOrFlagAuditHistoryIconByTitle("Flag Audit History");
            _preAuthAction.WaitForStaticTime(1500);

            for (int i = 1; i < 4; i++)
            {
                if (i == 2)
                {
                    for (int j = 1; j < 3; j++)
                    {
                        flagAuditHistoryDetails.Add(listOfFlagAuditHistoryLabels[listCounter], _preAuthAction.GetFlagAuditHistoryValue(lineNo, auditRecordRow, i, j));
                        listCounter++;
                    }
                    continue;
                }

                // Stores the 'Notes' value from the Flag Audit History to the IDictionary
                if (i == 3)
                {
                    flagAuditHistoryDetails.Add(listOfFlagAuditHistoryLabels[listCounter], _preAuthAction.GetFlagAuditHistoryValue(lineNo, auditRecordRow, i, 1));
                    break;
                }

                for (int j = 1; j < 4; j++)
                {
                    flagAuditHistoryDetails.Add(listOfFlagAuditHistoryLabels[listCounter], _preAuthAction.GetFlagAuditHistoryValue(lineNo, auditRecordRow, i, j));
                    listCounter++;
                }
            }

            flagAuditHistoryDetails["Mod Date"].Contains(DateTime.Now.ToString("MM/dd/yyyy")).ShouldBeTrue("Is 'Mod Date' being correctly displayed?");
            flagAuditHistoryDetails["By"].Contains(modifiedBy).ShouldBeTrue("Is 'By' being correctly displayed?");
            flagAuditHistoryDetails["User Type"].Contains(userType).ShouldBeTrue("Is 'User Type' being correctly displayed?");
            flagAuditHistoryDetails["Action"].Contains(delete ? deleteActionText : restoreActionText).ShouldBeTrue("Is 'Action' being correctly displayed?");
            flagAuditHistoryDetails["Reason Code"].Contains(reasonCode).ShouldBeTrue("Is 'Reason Code' being correctly displayed?");
            flagAuditHistoryDetails["Notes"].Contains(delete ? "Delete Note" : "Restore Note").ShouldBeTrue("Is 'Notes' being correctly displayed?");
        }

        private void EditFlag(PreAuthActionPage _preAuthAction, string lineNo, string row, int reasoncode, string title,
            string note = "", bool client = false, bool notes = false, bool istitle = true)
        {
            _preAuthAction.ClickOnEditFlagIconByLineNoAndRow(lineNo, row);
            

            if (istitle)
            {
                _preAuthAction.ClickOnDeleteOrRestoreIcon(title);
            }
         

            if (client)
            {
                _preAuthAction.ClickVisibleToClientIcon();
            }

            if (notes)
            {
                _preAuthAction.InputNotes(note);
            }
            _preAuthAction.SelectReasonCode(reasoncode);

        }

        private List<string> ClickNextAndGetAuthSeqList(PreAuthActionPage _preAuthAction, int row, int count)
        {
            List<string> authSeqList = new List<string>();
            authSeqList.Add(_preAuthAction.GetPreAuthDetailValueByTitle("Auth Seq"));
            for (int i = 0; i <= count - 1; i++)
            {
                _preAuthAction.ClickOnNextIcon();
                _preAuthAction.WaitForStaticTime(3000);
                _preAuthAction.CloseCurrentWindowAndSwitchToOriginal(row);
                authSeqList.Add(_preAuthAction.GetPreAuthDetailValueByTitle("Auth Seq"));

            }

            return authSeqList;
        }

        private void VerificationOfLogicCancelFunctionality(PreAuthActionPage _preAuthAction, string msg, string user)
        {
            _preAuthAction.AddLogicMessageTextarea(msg);
            _preAuthAction.ClickCancel();
            _preAuthAction.ClickOnLogicIconWithLogicByRow(1, 1);
            _preAuthAction.GetLogicMessageTextarea().ShouldBeNullorEmpty("Message Box Should be Null or Empty");
            _preAuthAction.GetSideWindow.GetValueByLabel("Assigned to").ShouldBeEqual(
                user,
                $"Assigned to should be {user}");
            _preAuthAction.GetSideWindow.GetValueByLabel("Status").ShouldBeEqual(
                LogicStatusEnum.Open.GetStringValue(),
                $"Status should be {LogicStatusEnum.Open.GetStringValue()} ");
        }

        #endregion
    }
}

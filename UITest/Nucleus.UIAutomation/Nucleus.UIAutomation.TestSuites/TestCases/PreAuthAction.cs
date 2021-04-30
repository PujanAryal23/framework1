using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using static System.String;
using System.Text.RegularExpressions;
using Nucleus.Service.Data;
using Nucleus.Service.PageServices.Claim;
using Nucleus.Service.PageServices.PreAuthorization;
using Nucleus.Service.PageServices.QuickLaunch;
using Nucleus.Service.PageServices.Settings.User;
using Nucleus.Service.Support.Common.Constants;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Environment;
using Nucleus.UIAutomation.TestSuites.Base;
using Nucleus.UIAutomation.TestSuites.Common;
using NUnit.Framework;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Utils;
using Extensions = Nucleus.Service.Support.Utils.Extensions;
using Nucleus.Service.Support.Common;

namespace Nucleus.UIAutomation.TestSuites.TestCases
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class PreAuthAction
    {

        //#region PRIVATE FIELDS

        //private UserProfileSearchPage _newUserProfileSearch;
        //private PreAuthActionPage _preAuthAction;
        //private PreAuthSearchPage _preAuthSearch;
        //private PreAuthProcessingHistoryPage _preAuthProcessingHistory;
        //private ProfileManagerPage _profileManager;
        //private ClaimHistoryPage _claimHistory;
        //private QuickLaunchPage _quickLaunch;


        //#endregion

        #region PROTECTED PROPERTIES

        protected string FullyQualifiedClassName
        {
            get { return GetType().FullName; }
            }

        #endregion

        //#region OVERRIDE METHODS

        //protected override void ClassInit()
        //{
        //    try
        //    {
        //        base.ClassInit();
        //        //_authSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, "AuthSequence", "AuthSequence", "Value");
        //        _preAuthSearch = QuickLaunch.NavigateToPreAuthSearch();
        //    }
        //    catch (Exception)
        //    {
        //        if (StartFlow != null)
        //            StartFlow.Dispose();
        //        throw;
        //    }
        //}

        //protected override void TestCleanUp()
        //{
        //    if (_preAuthSearch.IsPageErrorPopupModalPresent())
        //        _preAuthSearch.ClosePageError();

        //    if (string.Compare(UserType.CurrentUserType, UserType.HCIADMIN, StringComparison.OrdinalIgnoreCase) != 0)
        //    {
        //        CurrentPage = QuickLaunch = CurrentPage.Logout().LoginAsHciAdminUser();
        //        _preAuthSearch = QuickLaunch.NavigateToPreAuthSearch();

        //    }
        //    else if (CurrentPage.GetPageHeader() != PageHeaderEnum.PreAuthSearch.GetStringValue())
        //    {
        //        CurrentPage.NavigateToPreAuthSearch();
        //        _preAuthSearch.GetSideBarPanelSearch.ClickOnClearLink();
        //        _preAuthSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
        //            PreAuthQuickSearchEnum.OutstandingPreAuths.GetStringValue());

        //    }
        //    else
        //    {
        //        _preAuthSearch.GetSideBarPanelSearch.ClickOnClearLink();
        //        _preAuthSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
        //            PreAuthQuickSearchEnum.OutstandingPreAuths.GetStringValue());
        //    }
        //    base.TestCleanUp();
        //}

        //protected override void ClassCleanUp()
        //{
        //    try
        //    {
        //        _preAuthAction.CloseDatabaseConnection();


        //    }
        //    finally
        //    {
        //        base.ClassCleanUp();
        //    }

        //}

        //#endregion

        #region TEST SUITES

        [Test] //CAR-2442[CAR-2693] + CAR-2689(CAR-2731)
        public void Verify_preauth_logics_are_getting_displayed_in_logic_search()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                PreAuthSearchPage _preAuthSearch;
                PreAuthActionPage _preAuthAction;
                _preAuthSearch = automatedBase.QuickLaunch.NavigateToPreAuthSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var authSeq = paramLists["PreAuth"];
                var flag = paramLists["Flag"];

                StringFormatter.PrintMessageTitle("Delete Previous Logics From Database");
                _preAuthSearch.DeleteLogicFromDatabase(authSeq);

                StringFormatter.PrintMessageTitle(
                    "Creating new logics on the pre-auth to verify the behavior in logic search");

                _preAuthAction = _preAuthSearch.SearchByAuthSequenceAndNavigateToAuthAction(authSeq, false);
                var internalFullName = _preAuthAction.GetLoggedInUserFullName();
                var sysdate = DateTime.ParseExact(_preAuthAction.GetSystemDateFromDatabase(), "MM/dd/yyyy hh:mm:ss tt",
                    CultureInfo.InvariantCulture);

                StringFormatter.PrintMessage("Create a new Logic");
                _preAuthAction.ClickOnAddLogicIconByRow(1, 1);
                _preAuthAction.AddLogicMessageTextarea("Test Logic From Internal User");
                _preAuthAction.ClickSaveButton();
                _preAuthAction.ClickCancel();

                StringFormatter.PrintMessage("Create a Second Logic");
                _preAuthAction.ClickOnAddLogicIconByRow(1, 2);
                _preAuthAction.AddLogicMessageTextarea("Another Test Logic From Internal User");
                _preAuthAction.ClickSaveButton();

                StringFormatter.PrintMessageTitle(
                    "Verifying whether Pre-Auth logics are getting displayed in Logic Search for Client Users");
                var _logicSearch = _preAuthAction.Logout().LoginAsClientUser().NavigateToLogicSearch();
                _logicSearch.GetGridViewSection.GetGridListValueByCol(3).ShouldContain(authSeq,
                    "Logic is displayed when the Quick Search option is" +
                    $"'{LogicQuickSearchTypeEnum.AllOpenLogics.GetStringValue()}'");
                _logicSearch.GetGridViewSection.GetGridListValueByCol(3).Where(x => x.Contains(authSeq)).ToList().Count
                    .ShouldBeEqual(2,
                        "There should be 2 rows shown for the two different logics created by the internal user");

                StringFormatter.PrintMessage(
                    $"Verifying filtering the data by Quick Search option '{LogicQuickSearchTypeEnum.DCIOpenLogics.GetStringValue()}'");
                _logicSearch.SideBarPanelSearch.SetDateField("Create Date Range", DateTime.Now.ToString("MM/dd/yyyy"),
                    1);
                _logicSearch.SideBarPanelSearch.SetDateField("Create Date Range", DateTime.Now.ToString("MM/dd/yyyy"),
                    2);
                _logicSearch.SideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                    LogicQuickSearchTypeEnum.DCIOpenLogics.GetStringValue());
                _logicSearch.SideBarPanelSearch.ClickOnFindButton();
                _logicSearch.WaitForWorking();
                _logicSearch.GetGridViewSection.GetGridListValueByCol(3).ShouldContain(authSeq,
                    "Logic is displayed when the Quick Search option is"
                    + $"'{LogicQuickSearchTypeEnum.DCIOpenLogics.GetStringValue()}'");

                _preAuthAction = _logicSearch.ClickOnAuthSeqToNavigatePreAuthActionPage(authSeq);
                _preAuthAction.GetSideWindow.ClickOnSecondaryButton();

                #region CAR-2689

                _logicSearch = _preAuthAction.ClickOnSearchIconToReturnToLogicSearchPage();
                _logicSearch.GetPageHeader().ShouldBeEqual(PageHeaderEnum.LogicSearch.GetStringValue(),
                    "Page should be navigated to Logic Search");

                #endregion

                _logicSearch.SideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                    LogicQuickSearchTypeEnum.DCIOpenLogics.GetStringValue());
                _logicSearch.SideBarPanelSearch.SetDateField("Create Date Range", DateTime.Now.ToString("MM/dd/yyyy"),
                    1);
                _logicSearch.SideBarPanelSearch.SetDateField("Create Date Range", DateTime.Now.ToString("MM/dd/yyyy"),
                    2);
                _logicSearch.SideBarPanelSearch.ClickOnFindButton();
                _logicSearch.WaitForWorking();

                _logicSearch.GetGridViewSection.GetGridListValueByCol(3).Where(x => x.Contains(authSeq)).ToList().Count
                    .ShouldBeEqual(1,
                        "Only open logics should be displayed in the result for client users");

                VerifyPrimaryDataInGrid();

                _preAuthAction = _logicSearch.ClickOnAuthSeqToNavigatePreAuthActionPage(authSeq);
                _preAuthAction.AddLogicMessageTextarea("Test Reply");
                _preAuthAction.ClickSaveButton();

                StringFormatter.PrintMessageTitle(
                    "Verifying whether Pre-Auth logics are getting displayed in Logic Search for Internal Users");
                _logicSearch = _preAuthAction.Logout().LoginAsHciAdminUser().NavigateToLogicSearch();
                _logicSearch.SideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                    LogicQuickSearchTypeEnum.DCIOpenLogics.GetStringValue());
                _logicSearch.SideBarPanelSearch.SetDateField("Create Date Range", DateTime.Now.ToString("MM/dd/yyyy"),
                    1);
                _logicSearch.SideBarPanelSearch.SetDateField("Create Date Range", DateTime.Now.ToString("MM/dd/yyyy"),
                    2);
                _logicSearch.SideBarPanelSearch.ClickOnFindButton();
                _logicSearch.WaitForWorking();

                _logicSearch.GetGridViewSection.GetGridListValueByCol(3).Where(x => x.Contains(authSeq)).ToList().Count
                    .ShouldBeEqual(1,
                        "Only open logics should be displayed in the result for internal users");

                VerifyPrimaryDataInGrid(false);

                #region LOCAL METHODS

                void VerifyPrimaryDataInGrid(bool clientUser = true)
                {
                    _logicSearch.GetGridViewSection.GetValueInGridByColRow().ShouldBeEqual(ClientEnum.SMTST.ToString(),
                        "Client code should be shown in the result grid");
                    _logicSearch.GetGridViewSection.GetValueInGridByColRow(3).ShouldBeEqual(authSeq,
                        "Pre-auth seq should be shown in the result grid");
                    _logicSearch.GetGridViewSection.GetValueInGridByColRow(4).ShouldBeEqual(flag,
                        "Flag on which the logic is created should be shown in the result grid");
                    _logicSearch.GetGridViewSection.GetValueInGridByColRow(5).ShouldBeEqual(
                        clientUser ? "Client" : "Cotiviti",
                        "Assigned to value should be shown in the result grid");
                    _logicSearch.GetGridViewSection.GetValueInGridByColRow(6).ShouldBeEqual("Open",
                        "Logic status should be displayed in the result grid");

                    if (clientUser)
                        _logicSearch.GetGridViewSection.GetValueInGridByColRow(7).ShouldBeEqual(internalFullName,
                            "Created By user should be displayed in the result grid");

                    //_logicSearch.GetGridViewSection.GetValueInGridByColRow(8).ShouldContain(DateTime.Now.ToString("MM/dd/yyyy"),
                    //  "Last Response Date should be shown in the grid");

                    DateTime.ParseExact(_logicSearch.GetGridViewSection.GetValueInGridByColRow(clientUser ? 8 : 7),
                        "MM/dd/yyyy h:m:s tt",
                        CultureInfo.InvariantCulture).AssertDateRange(sysdate.AddMinutes(-2),
                        sysdate.AddMinutes(2), "Last Response Date should be shown in the grid");
                }

                #endregion
            }
        }

        [Test] //CAR-2440(CAR-2638)
        public void Verify_create_and_manage_logic_on_flag_line()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                PreAuthSearchPage _preAuthSearch;
                PreAuthActionPage _preAuthAction;
                _preAuthSearch = automatedBase.QuickLaunch.NavigateToPreAuthSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var authSeq = paramLists["PreAuthSeq"];

                StringFormatter.PrintMessageTitle("Delete Logic From Database");
                _preAuthSearch.DeleteLogicFromDatabase(authSeq);
                _preAuthAction = _preAuthSearch.SearchByAuthSequenceAndNavigateToAuthAction(authSeq, false);
                try
                {
                    StringFormatter.PrintMessageTitle("Create New Logic");
                    _preAuthAction.IsLogicPlusIconDisplayed(1, 1)
                        .ShouldBeTrue("Lplus Icon should be displayed for No Logic");
                    _preAuthAction.ClickOnAddLogicIconByRow(1, 1);
                    _preAuthAction.IsLogicFormTextPresent("Create Logic")
                        .ShouldBeTrue("Create Logic form should be displayed");

                    StringFormatter.PrintMessage("Verification that text is required to start conversation");
                    _preAuthAction.ClickSaveButton();
                    _preAuthAction.IsPageErrorPopupModalPresent().ShouldBeTrue("Popup Error for Empty Message Reply");
                    _preAuthAction.GetPageErrorMessage().ShouldBeEqual("Enter a message to create Logic request.",
                        "Error message has correct warning text");
                    _preAuthAction.ClosePageError();

                    StringFormatter.PrintMessage("Verification that the user can type upto 500 characters as message");
                    _preAuthAction.AddLogicMessageTextarea(Concat(Enumerable.Repeat("test ", 101)));
                    _preAuthAction.GetLogicMessageTextarea().Length
                        .ShouldBeEqual(500, "user can free text up to 500 characters");

                    StringFormatter.PrintMessage("Creation of logic");
                    _preAuthAction.AddLogicMessageTextarea("Test Logic From Internal User");
                    _preAuthAction.ClickSaveButton();

                    StringFormatter.PrintMessage("Validation after logic creation");
                    var actualLogicMessage =
                        Regex.Split(_preAuthAction.GetRecentRightLogicMessageByRow(1), "\r\n").ToList();
                    actualLogicMessage[0].ShouldBeEqual(_preAuthAction.GetLoggedInUserFullName(),
                        "Reply User Full Name should be same");
                    actualLogicMessage[1].IsDateTimeWithoutSecInFormat()
                        .ShouldBeTrue("Is Date Time is in Correct Format?");
                    actualLogicMessage[2].ShouldBeEqual("Test Logic From Internal User", "Is Logic Message Same?");
                    _preAuthAction.GetSideWindow.GetValueByLabel("Status").ShouldBeEqual(
                        LogicStatusEnum.Open.GetStringValue(),
                        $"Status should be {LogicStatusEnum.Open.GetStringValue()} when logic is created");
                    _preAuthAction.GetSideWindow.GetValueByLabel("Assigned to").ShouldBeEqual(
                        "Client", $"Assigned to should be Client");

                    StringFormatter.PrintMessageTitle("Verification of Cancel Button");
                    VerificationOfLogicCancelFunctionality("Cancel Test Logic Reply From Internal User", "Client");

                    #region Verification of reply and close functionality

                    StringFormatter.PrintMessageTitle(
                        "Verification of Reply and Close functionalities for Client user");

                    _preAuthAction.Logout().LoginAsClientUser().NavigateToPreAuthSearch()
                        .SearchByAuthSequenceAndNavigateToAuthAction(authSeq);
                    _preAuthAction.IsClientLogicIconByRowPresent(1, 1)
                        .ShouldBeTrue("Logic Icon should be Assigned to Client - unfilled state");
                    //_preAuthAction.ClickOnLogicIcon(1, 1, "CLIENT");


                    StringFormatter.PrintMessage("Verification of presence of buttons, form and values");
                    _preAuthAction.GetPrimaryButtonText()
                        .ShouldBeEqual("Reply", "Is 'Reply' Button Present?");
                    _preAuthAction.GetSideWindow.GetSecondaryButtonName()
                        .ShouldBeEqual("Close Logic", "Is 'Close Logic' Button Present?");
                    _preAuthAction.IsLogicFormTextPresent("Reply To Logic")
                        .ShouldBeTrue("Create Logic form should be displayed");
                    _preAuthAction.GetSideWindow.GetValueByLabel("Assigned to")
                        .ShouldBeEqual("Client", "Assigned to should be Client");
                    _preAuthAction.GetSideWindow.GetValueByLabel("Status").ShouldBeEqual(
                        LogicStatusEnum.Open.GetStringValue(),
                        $"Status should be {LogicStatusEnum.Open.GetStringValue()} when logic is created");

                    StringFormatter.PrintMessage("Verification that text is required to reply logic");
                    _preAuthAction.ClickSaveButton();
                    _preAuthAction.IsPageErrorPopupModalPresent()
                        .ShouldBeTrue("Error popup should be present when replied with empty text");
                    _preAuthAction.GetPageErrorMessage().ShouldBeEqual("Enter a message to create Logic request.",
                        "Popup Error for Empty Message Reply");
                    _preAuthAction.ClosePageError();

                    StringFormatter.PrintMessage("Verification that 500 characters are allowed");
                    _preAuthAction.AddLogicMessageTextarea(Concat(Enumerable.Repeat("TEST ", 101)));
                    _preAuthAction.GetLogicMessageTextarea().Length
                        .ShouldBeEqual(500, "User can free text up to 500 characters");

                    StringFormatter.PrintMessageTitle("Verification of Cancel Button");
                    VerificationOfLogicCancelFunctionality("Cancel Test Logic Reply From Client User", "Client");

                    StringFormatter.PrintMessageTitle(
                        "Verification that text from opposite user will be left-justified");
                    _preAuthAction.AddLogicMessageTextarea("Logic Reply From Client User");
                    _preAuthAction.ClickSaveButton();
                    var actualLeftLogicMessage =
                        Regex.Split(_preAuthAction.GetRecentLeftLogicMessage(), "\r\n").ToList();
                    actualLeftLogicMessage.ShouldBeEqual(actualLogicMessage,
                        "Other User Type Logic Should be on left side");
                    _preAuthAction.IsLogicFormTextPresent("Reply To Logic")
                        .ShouldBeTrue("Logic form should remain open even after a logic is replied");

                    StringFormatter.PrintMessage("Verification that text from same user will be right-justified");
                    actualLogicMessage =
                        Regex.Split(_preAuthAction.GetRecentRightLogicMessageByRow(2), "\r\n").ToList();
                    actualLogicMessage[0].ShouldBeEqual(_preAuthAction.GetLoggedInUserFullName(),
                        "Reply User Full Name should be same");
                    actualLogicMessage[1].IsDateTimeWithoutSecInFormat()
                        .ShouldBeTrue("Is Date Time is in Correct Format?");
                    actualLogicMessage[2].ShouldBeEqual($"Logic Reply From Client User", "Is Logic Message Same?");
                    _preAuthAction.GetSideWindow.GetValueByLabel("Status").ShouldBeEqual(
                        LogicStatusEnum.Open.GetStringValue(),
                        $"Status should be {LogicStatusEnum.Open.GetStringValue()} when logic is created");

                    StringFormatter.PrintMessage(
                        "Verification that user will be able to add another message and reply");
                    _preAuthAction.AddLogicMessageTextarea("Additional Logic Reply From Client User");
                    _preAuthAction.ClickSaveButton();
                    actualLeftLogicMessage =
                        Regex.Split(_preAuthAction.GetRecentRightLogicMessageByRow(3), "\r\n").ToList();
                    actualLeftLogicMessage[2].ShouldBeEqual("Additional Logic Reply From Client User",
                        "User can add another message and reply");

                    StringFormatter.PrintMessage("Verification of Close functionality");
                    _preAuthAction.GetSideWindow.ClickOnSecondaryButton();
                    _preAuthAction.IsPageErrorPopupModalPresent()
                        .ShouldBeFalse("User able to Close Logic without entering a message");
                    _preAuthAction.IsLogicWindowDisplayByRowPresent(1, 1)
                        .ShouldBeTrue("Logic Form Should remain open even after logic is closed");
                    _preAuthAction.GetSideWindow.GetValueByLabel("Status").ShouldBeEqual(
                        LogicStatusEnum.Closed.GetStringValue(),
                        $"Status should be {LogicStatusEnum.Closed.GetStringValue()} when logic is closed");
                    _preAuthAction.GetSideWindow.GetValueByLabel("Assigned to").ShouldBeEqual(
                        "Cotiviti", "Assigned to should be remain unchanged when the logic is closed");

                    #endregion

                    StringFormatter.PrintMessage("Verification that user will be able to reopen the logic");
                    _preAuthAction.AddLogicMessageTextarea("New Logic Reply From Client User");
                    _preAuthAction.ClickSaveButton();
                    _preAuthAction.GetSideWindow.GetValueByLabel("Assigned to").ShouldBeEqual(
                        "Cotiviti", "Assigned to should be Cotiviti");
                    _preAuthAction.GetSideWindow.GetValueByLabel("Status").ShouldBeEqual(
                        LogicStatusEnum.Open.GetStringValue(),
                        $"Status should be {LogicStatusEnum.Closed.GetStringValue()} when logic is closed");
                    _preAuthAction.ClickCancel();
                }

                finally
                {
                    _preAuthAction.Logout().LoginAsHciAdminUser().NavigateToPreAuthSearch();
                }

                void VerificationOfLogicCancelFunctionality(string msg, string user)
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
            }
        }


        [Test] //CAR-1702(CAR-1574) + TE-802 + CAR-3030 [CAR-3082]
        [Retrying(Times = 3)]
        public void Verify_add_flag_functionality()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                PreAuthSearchPage _preAuthSearch;
                PreAuthActionPage _preAuthAction;
                _preAuthSearch = automatedBase.QuickLaunch.NavigateToPreAuthSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var _authSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName,
                    "AuthSequence", "Value");
                var flags = automatedBase.DataHelper
                    .GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName, "Flags", "Value")
                    .Split(';').ToList();
                var flag = flags[0];
                var flag2 = flags[1];
                var flag1 = flags[2];

                _preAuthSearch.DeleteFlagAndAuditByAuthSeqFlagsLinNo(_authSeq, "'" + flag + "'" + ",'" + flag2 + "'",
                    "1");
                _preAuthSearch.DeleteFlagAuditByAuthSeqFlagsLinNo(_authSeq, "'" + flag1 + "'", "1");
                _preAuthAction =
                    _preAuthSearch.SearchByAuthSequenceAndNavigateToAuthAction(_authSeq, false, popup: false);
                _preAuthAction.IsPatientClaimHistoryPresent().ShouldBeTrue(
                    "PreAuth Processing history pop up present on initial navigation to prea auth action page?");
                _preAuthAction.CloseAnyPopupIfExist();

                var preTopFlag = _preAuthAction.GetLineItemValueByDivRowColumn(1, 1, 2);
                if (string.IsNullOrEmpty(preTopFlag))
                {
                    EditFlag("1", "1", 2, "", "TEST", false, true, false);
                    _preAuthAction.ClickSaveButton();
                    _preAuthAction.WaitForWorking();
                }

                double.TryParse(_preAuthAction.GetLineItemValueByDivRowColumn(1, 2, 4), NumberStyles.Currency,
                    CultureInfo.CurrentCulture.NumberFormat, out var adjPaid);

                double.TryParse(_preAuthAction.GetFlagLineLevelHeaderDetailValue(1, 2, 2), NumberStyles.Currency,
                    CultureInfo.CurrentCulture.NumberFormat, out var sugPaidBefore);
                _preAuthAction.GetFlagLineLevelHeaderDetailValue(1, 2, 3).ShouldBeEqual(
                    (adjPaid - sugPaidBefore).ToString("C"),
                    $"Since {flag1} is top flag,Savings should be calculated based on the top flag.");

                var expectedflagList = _preAuthAction.GetFlagListByDivList();
                expectedflagList.Insert(0, flag); //flag with higher rank is push to top of list
                var sugPaidAfter = sugPaidBefore + 5;
                _preAuthAction.ClickOnAddFlagIcon();
                _preAuthAction.ClickOnLinesOnSelectLines();
                _preAuthAction.GetSideWindow.FillInputBox("Sug Paid", sugPaidAfter.ToString(), true);
                _preAuthAction.GetSideWindow.SelectDropDownListValueByLabel("Flag", flag);
                _preAuthAction.GetSideWindow.FillIFrame("Note", "ADD FLAG TEST");
                _preAuthAction.ClickSaveButton();
                _preAuthAction.WaitForWorking();
                _preAuthAction.IsPatientClaimHistoryPresent()
                    .ShouldBeFalse("PreAuth Processing history pop up present?");

                _preAuthAction.GetFlagRowDetailValue(1, 1, 4).ShouldBeEqual("MANUAL", "source");
                _preAuthAction.GetLineItemValueByDivRowColumn(1, 1, 2).ShouldBeEqual(flag, "flag");
                _preAuthAction.GetFlagListByDivList()
                    .ShouldCollectionBeEqual(expectedflagList,
                        "New Flag List should be match with higher rank flag at top");
                _preAuthAction.GetFlagLineLevelHeaderDetailValue(1, 2, 2).ShouldNotBeEqual(sugPaidBefore.ToString("C"),
                    "Sug paid should be changed to current top flag's value");
                _preAuthAction.GetFlagLineLevelHeaderDetailValue(1, 2, 2).ShouldBeEqual(sugPaidAfter.ToString("C"),
                    $"Since {flag} is top flag,Sug Paid should be updated with value entered in add flag form ");

                _preAuthAction.GetFlagLineLevelHeaderDetailValue(1, 2, 3).ShouldBeEqual(
                    (adjPaid - sugPaidAfter).ToString("C"),
                    $"Since {flag} is top flag,Savings should be updated according to top flag ");

                _preAuthAction.ClickOnAddFlagIcon();
                _preAuthAction.ClickOnLinesOnSelectLines();
                _preAuthAction.GetSideWindow.SelectDropDownListValueByLabel("Flag", flag2);
                _preAuthAction.ClickVisibleToClientIcon();
                _preAuthAction.ClickSaveButton();
                _preAuthAction.WaitForWorking();
                _preAuthAction.IsPatientClaimHistoryPresent()
                    .ShouldBeFalse("PreAuth Processing history pop up present?");

                expectedflagList.Insert(expectedflagList.Count, flag2);
                _preAuthAction.GetLineItemValueByDivRowColumn(1, 1, 2).ShouldBeEqual(flag,
                    $"Top Flag should be unchanged and remain same as {flag}");
                _preAuthAction.GetFlagListByDivList()
                    .ShouldCollectionBeEqual(expectedflagList, "New Flag  should be added");
                _preAuthAction.GetFlagLineLevelHeaderDetailValue(1, 2, 2)
                    .ShouldBeEqual(sugPaidAfter.ToString("C"), "sug paid should be same as that of top flag");
                _preAuthAction.GetFlagLineLevelHeaderDetailValue(1, 2, 3).ShouldBeEqual(
                    (adjPaid - sugPaidAfter).ToString("C"),
                    $"Since {flag} is top flag,Savings should be updated with value entered in add flag form ");

                StringFormatter.PrintMessage("Verify Flag audit records for newly added lines");
                _preAuthAction.ClickOnLineItemOrFlagAuditHistoryIconByTitle("Flag Audit History");
                //Convert.ToDateTime(_preAuthAction.GetFlagAuditHistoryByLinNoFlagLabel(1, flag, "Mod Date")).Date
                //    .ShouldBeEqual(DateTime.Now.Date, "Date should be in MM/DD/YYYY H:MM:SS AM/PM format");
                _preAuthAction.GetFlagAuditHistoryByLinNoFlagLabel(1, flag, "By")
                    .DoesNameContainsOnlyFirstWithLastname()
                    .ShouldBeTrue("Does name consist of Firstname and lastname of user?");
                _preAuthAction.GetFlagAuditHistoryByLinNoFlagLabel(1, flag, "User Type")
                    .ShouldBeEqual("Internal", "User Type should match");
                _preAuthAction.GetFlagAuditHistoryByLinNoFlagLabel(1, flag, "Action")
                    .ShouldBeEqual("Add", "Action performed should be shown");
                _preAuthAction.GetFlagAuditHistoryByLinNoFlagLabel(1, flag, "Reason Code")
                    .ShouldBeEqual("", "Reason code should match");
                _preAuthAction.GetFlagAuditHistoryByLinNoFlagLabel(1, flag, "Client Display")
                    .ShouldBeEqual("No", "Client Display should match");
                _preAuthAction.GetFlagAuditHistoryByLinNoFlagLabel(1, flag, "Notes")
                    .ShouldBeEqual("ADD FLAG TEST", "Note should match");

                _preAuthAction.GetFlagAuditHistoryByLinNoFlagLabel(1, flag2, "Client Display").ShouldBeEqual("Yes",
                    "Client Display should be 'Yes' when Visible to Client checkbox is selected");

                void EditFlag(string lineNo, string row, int reasoncode, string title, string note = "",
                    bool client = false, bool notes = false, bool istitle = true)
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
            }
        }

        [Test] //CAR - 268(CAR-1706)
        public void Verify_Lock_Funtionality()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                PreAuthSearchPage _preAuthSearch;
                PreAuthActionPage _preAuthAction;
                _preAuthSearch = automatedBase.QuickLaunch.NavigateToPreAuthSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var lockedAuthSequence = paramLists["LockedAuthSequence"];
                var unlockedAuthSequence = paramLists["UnlockedAuthSequence"];
                var providerSequence = paramLists["ProviderSequence"];
                try
                {
                    _preAuthSearch.UpdateStatusOfAuthSeqFromDatabase("U", unlockedAuthSequence);
                    _preAuthSearch.DeleteLockByPreAuthSeq(lockedAuthSequence);
                    _preAuthSearch.Logout().LoginAsHciAdminUser4().NavigateToPreAuthSearch();
                    automatedBase.CurrentPage = _preAuthAction =
                        _preAuthSearch.SearchByAuthSequenceAndNavigateToAuthAction(lockedAuthSequence, false);
                    _preAuthAction.IsLockIconPresent().ShouldBeFalse("Lock icon should not be present");

                    //var lockedby = _preAuthAction.GetUsername();
                    var lockedby = _preAuthAction.GetLoggedInUserFullName();
                    var pageUrl = automatedBase.CurrentPage.CurrentPageUrl;

                    StringFormatter.PrintMessage(
                        "Logging in as other user and verifying the Lock in Pre-auth Search page");

                    lockedby.DoesNameContainsOnlyFirstWithLastname()
                        .ShouldBeTrue("Does name contain first name and last name?");
                    _preAuthSearch.IsLockIconPresentAndGetLockIconToolTipMessage(lockedAuthSequence, pageUrl, false)
                        .ShouldBeEqual($"Pre-Auth is currently locked by {lockedby}");
                    _preAuthAction = _preAuthSearch.NaviateToPreAuthActionPage(1, 2, false, false);

                    StringFormatter.PrintMessage("Verification of Lock functionalities on Pre-Auth Action page");

                    _preAuthAction.CloseCurrentWindowAndSwitchToOriginal(1);
                    _preAuthAction.IsLockIconPresent().ShouldBeTrue("Is Lock icon present?");
                    _preAuthAction.GetToolTipOfLockIcon().ShouldBeEqual(
                        $"Pre-Auth is opened in view mode. It is currently locked by {lockedby}");
                    _preAuthAction.IsApproveIconEnabled().ShouldBeFalse("Is Approve icon enabled?");
                    _preAuthAction.IsAddIconEnabled().ShouldBeFalse("Is Add icon enabled?");
                    _preAuthAction.IsTransferIconDisabled().ShouldBeTrue("Is transfer icon disabled?");
                    _preAuthAction.IsEditFlagIconDisabled().ShouldBeTrue("Is Edit flag icon disabled?");
                    //_preAuthAction.IsEditDentalRecordIconDisabled().ShouldBeTrue("Is Edit Dental Record icon disabled");
                    _preAuthSearch = _preAuthAction.ClickOnReturnToPreAuthSearch();
                    _preAuthSearch.WaitForPageToLoadWithSideBarPanel();
                    _preAuthSearch.GetSideBarPanelSearch.ClickOnClearLink();
                    _preAuthSearch.SetProviderSeq(providerSequence);
                    _preAuthSearch.ClickOnFindButton();
                    _preAuthAction.WaitForPageToLoadWithSideBarPanel();

                    StringFormatter.PrintMessage("Verification of locked auth seq being skipped on clicking Next icon");

                    int lockedAuthSeqCount = _preAuthSearch.GetCountOfLockedAuthSequences();
                    int unlockedAuthSeqCount =
                        (_preAuthSearch.GetGridViewSection.GetGridRowCount()) - lockedAuthSeqCount;
                    var lockExcludedPreAuthList =
                        _preAuthSearch.GetSearchResultUptoRow(unlockedAuthSeqCount, true).Keys;

                    _preAuthAction = _preAuthSearch.ClickOnFirstUnlockedPreAuthToNavigateToPreAuthActionPage();
                    _preAuthAction.CloseCurrentWindowAndSwitchToOriginal(1);

                    var nextAuthSeqList = ClickNextAndGetAuthSeqList(1, unlockedAuthSeqCount - 1);

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
                    automatedBase.CurrentPage.ClickOnQuickLaunch().NavigateToPreAuthSearch();
                    automatedBase.CurrentPage.RefreshPage();
                }

                List<string> ClickNextAndGetAuthSeqList(int row, int count)
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
            }
        }

        [Test] //CAR-1393(CAR-1515)
        public void Verify_flag_color_functionality()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                PreAuthSearchPage _preAuthSearch;
                PreAuthActionPage _preAuthAction;
                _preAuthSearch = automatedBase.QuickLaunch.NavigateToPreAuthSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var _authSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName,
                    "AuthSequence", "Value");
                _preAuthAction = _preAuthSearch.SearchByAuthSequenceAndNavigateToAuthAction(_authSeq, false);
                _preAuthAction.GetBoldOrNormalColorFlagList().ShouldCollectionBeEquivalent(
                    _preAuthAction.GetFlagListByHcidone(_authSeq), "Is Bold Red Color display for HCIDONE='F'");
                _preAuthAction.GetBoldOrNormalColorFlagList(false).ShouldCollectionBeEquivalent(
                    _preAuthAction.GetFlagListByHcidone(_authSeq, true),
                    "Is Non Bold Red Color display for HCIDONE='T'");
            }
        }

        [Test] // CAR-1089(CAR-1613) + CAR-3233 [CAR-3261]
        public void Verify_Approve_Functionality()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                PreAuthSearchPage _preAuthSearch;
                PreAuthActionPage _preAuthAction;
                _preAuthSearch = automatedBase.QuickLaunch.NavigateToPreAuthSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var authsequences = paramLists["AuthSequence"];
                var providerSequence = paramLists["ProviderSequence"];
                var processingHistoryPageHeader = automatedBase.DataHelper
                    .GetMappingData(FullyQualifiedClassName, "processingHistoryHeader").Values.ToList();
                try
                {
                    StringFormatter.PrintMessage("Reverting pre-auth status and hcidone in DB");
                    _preAuthSearch.UpdateStatusOfAuthSeqFromDatabase("U", authsequences);
                    _preAuthSearch.UpdateHciDoneOrClientDoneToFalseFromDatabase("hcidone", authsequences);
                    _preAuthSearch.UpdateStatusOfAuthSeqFromDatabase("S", paramLists["AuthSeqWithConsultantComplete"].Split(',').ToList()[0]);
                    _preAuthSearch.UpdateStatusOfAuthSeqFromDatabase("K", paramLists["AuthSeqWithConsultantComplete"].Split(',').ToList()[1]);
                    _preAuthSearch.UpdateHciDoneOrClientDoneToFalseFromDatabase("hcidone", paramLists["AuthSeqWithConsultantComplete"]);

                    StringFormatter.PrintMessage("Verification of user being able to use filter and sort options, approve icon being enabled for status Client unreviewed and user should be prevented from double clicking the icon ");

                    _preAuthSearch.GetSideBarPanelSearch.SelectDropDownListByIndex("Quick Search", 1);
                    _preAuthSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Provider Sequence", providerSequence);
                    _preAuthSearch.GetSideBarPanelSearch.ClickOnFindButton();
                    _preAuthSearch.WaitForWorkingAjaxMessage();
                    _preAuthSearch.IsListStringSortedInDescendingOrder(2)
                        .ShouldBeTrue("Is Auth Seq listed in descending order?");

                    var authSeqResultList = _preAuthSearch.GetGridViewSection.GetGridListValueByCol();

                    _preAuthSearch.ClickOnFilterOptionListRow(1);
                    _preAuthSearch.IsListStringSortedInAscendingOrder(2)
                        .ShouldBeTrue("Is list sorted in ascending order after sorting it by auth seq?");
                    _preAuthSearch.ClickOnFilterOptionListRow(1);

                    StringFormatter.PrintMessage("Verification that approve icon is enabled for status Cotiviti Unreviewed and prevented from double clicking");
                    _preAuthAction = _preAuthSearch.NaviateToPreAuthActionPage(2, 2);
                    var username = _preAuthAction.GetLoggedInUserFullName();

                    _preAuthAction.GetPreAuthDetailValueByTitle("Auth Seq")
                        .ShouldBeEqual(authSeqResultList[1], "Auth seq should match");
                    _preAuthAction.GetPreAuthDetailValueByTitle("Status")
                        .ShouldBeEqual(StatusEnum.CotivitiUnreviewed.GetStringDisplayValue());
                    _preAuthAction.IsApproveIconEnabled().ShouldBeTrue("Is Approve icon enabled?");
                    _preAuthAction.ClickApproveAndCheckIfApproveIconIsDisabled()
                        .ShouldBeTrue("Is Approve icon disabled?");
                    _preAuthAction.CloseLastWindow();

                    StringFormatter.PrintMessage("Verification that user will be moved to first unlocked pre-auth in the list of result set from Pre-Auth Search, user will be navigated back to Pre-Auth Search page when there are no additional records and HCIDONE = T after clicking approve");
                    var previousStatusOfPreAuthSeq = _preAuthAction.GetPreAuthDetailValueByTitle("Status");
                    _preAuthAction.GetPreAuthDetailValueByTitle("Auth Seq").ShouldBeEqual(authSeqResultList[0],
                        "First unlocked preauth seq should be displayed");
                    previousStatusOfPreAuthSeq.ShouldBeEqual(StatusEnum.CotivitiUnreviewed.GetStringDisplayValue(),
                        $"Status should be {StatusEnum.CotivitiUnreviewed.GetStringDisplayValue()}");
                    _preAuthAction.IsApproveIconEnabled().ShouldBeTrue("Is Approve icon enabled?");
                    _preAuthAction.ClickOnApproveIcon();
                    _preAuthAction.WaitForPageToLoadWithSideBarPanel();
                    _preAuthSearch.GetPageHeader().ShouldBeEqual(PageHeaderEnum.PreAuthSearch.GetStringValue(),
                        "When there is no additional records in the list, user will be navigated back to Pre-Auth search page");
                    _preAuthAction.IsAllHCIDONETrue(authsequences)
                        .ShouldBeTrue("Are all HCIDone set to true after approving the preauthseq?");

                    StringFormatter.PrintMessage("Verification for status being set to Client Unreviewed on approving the preauth seq if there are flags that contain Client review and approve icon is disabled for status Client Unreviewed");
                    _preAuthAction.NavigateToPreAuthSearch()
                        .SearchByAuthSequenceAndNavigateToAuthAction(authsequences.Split(',').ToList()[0]);
                    _preAuthAction.GetPreAuthDetailValueByTitle("Status").ShouldBeEqual(
                        StatusEnum.ClientUnreviewed.GetStringDisplayValue(),
                        $"Status should be {StatusEnum.ClientUnreviewed.GetStringDisplayValue()} when there is active flag");
                    _preAuthAction.IsApproveIconEnabled().ShouldBeFalse("Is Approve icon enabled?");

                    StringFormatter.PrintMessage("Verification that Pre-Auth Status will be closed when there are no flags that require Client Review and approve icon is disabled for status Closed");
                    _preAuthAction.NavigateToPreAuthSearch()
                        .SearchByAuthSequenceAndNavigateToAuthAction(authsequences.Split(',').ToList()[1]);
                    var status = _preAuthAction.GetPreAuthDetailValueByTitle("Status");
                    status.ShouldBeEqual(StatusEnum.Closed.GetStringDisplayValue(),
                        $"Status Should be {StatusEnum.Closed.GetStringDisplayValue()} when all flag is not active");
                    _preAuthAction.IsApproveIconEnabled().ShouldBeFalse("Is Approve icon enabled?");

                    StringFormatter.PrintMessage("Verification when approve icon is selected, a pre-auth record with Date, Username, status and notes will be displayed");

                    var preAuthProcessingHistory = _preAuthAction.ClickOnPreAuthProcessingHistoryAndSwitch();
                    preAuthProcessingHistory.GetPreAuthHistoryAuthSeq()
                        .ShouldBeEqual(authsequences.Split(',').ToList()[1], "Auth sequence should match");
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
                    _preAuthAction.CloseAnyPopupIfExist();

                    //CAR-3233 [CAR-3261]
                    StringFormatter.PrintMessageTitle("Verifying whether Approve icon is enabled for pre-auths with State/Cotiviti Consultant Complete status");

                    _preAuthSearch = _preAuthAction.NavigateToPreAuthSearch();
                    _preAuthSearch.GetSideBarPanelSearch.ClickOnClearLink();

                    var preAuthsWithConsultantComplete = paramLists["AuthSeqWithConsultantComplete"].Split(',').ToList();
                    var statusList = new List<string>()
                    {
                        StatusEnum.StateConsultantComplete.GetStringDisplayValue(),
                        StatusEnum.CotivitiConsultantComplete.GetStringDisplayValue()
                    };

                    for (int count = 0; count < 2; count++)
                    {
                        _preAuthSearch.SearchByAuthSequenceAndNavigateToAuthAction(preAuthsWithConsultantComplete[count], false );

                        _preAuthAction.GetPreAuthStatus().ShouldBeEqual(statusList[count] , $"Pre-auth status should be {statusList[count]}");
                        _preAuthAction.IsApproveIconEnabled().ShouldBeTrue($"Approve icon should be enabled when pre-auth status is {statusList[count]}");
                        _preAuthAction.ClickOnApproveIconAndNavigateToPreAuthSearchPage();
                        _preAuthAction.WaitForCondition(()=> automatedBase.CurrentPage.GetPageHeader().Equals(PageHeaderEnum.PreAuthSearch.GetStringValue()) );
                        _preAuthAction.WaitForCondition(() => _preAuthSearch.GetGridViewSection.GetValueInGridByColRow(8).Equals(StatusEnum.ClientUnreviewed.GetStringDisplayValue()));

                        _preAuthSearch.GetGridViewSection.GetValueInGridByColRow(8)
                            .ShouldBeEqual(StatusEnum.ClientUnreviewed.GetStringDisplayValue(),
                                "User should be allowed to approve the pre-auth and the pre-auth moves to correct status after the approval");
                    }
                }

                finally
                {
                    StringFormatter.PrintMessageTitle("Finally block reverting the hcidone and pre-auth status in DB");
                    _preAuthSearch.UpdateStatusOfAuthSeqFromDatabase("U", authsequences);
                    _preAuthSearch.UpdateHciDoneOrClientDoneToFalseFromDatabase("hcidone", authsequences);

                    _preAuthSearch.UpdateStatusOfAuthSeqFromDatabase("S", paramLists["AuthSeqWithConsultantComplete"].Split(',').ToList()[0]);
                    _preAuthSearch.UpdateStatusOfAuthSeqFromDatabase("K", paramLists["AuthSeqWithConsultantComplete"].Split(',').ToList()[1]);
                    _preAuthSearch.UpdateHciDoneOrClientDoneToFalseFromDatabase("hcidone", paramLists["AuthSeqWithConsultantComplete"]);
                }
            }
        }

        [Test]//CAR-1612(CAR-1091)
        [Retrying(Times = 3)]
        public void Verify_add_flag_and_select_lines_validation()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                PreAuthSearchPage _preAuthSearch;
                PreAuthActionPage _preAuthAction;
                _preAuthSearch = automatedBase.QuickLaunch.NavigateToPreAuthSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var quadrant1 = paramLists["QuadrantTitles1"].Split(',').ToList();
                var quadrant2 = paramLists["QuadrantTitles2"].Split(',').ToList();
                var authSequence = paramLists["AuthSequence"];
                var expectedSelectedList = new List<List<string>>
                {
                    paramLists["SelectedLines1"].Split(';').ToList(), paramLists["SelectedLines2"].Split(';').ToList()
                };
                try
                {
                    _preAuthAction = _preAuthSearch.SearchByAuthSequenceAndNavigateToAuthAction(authSequence, false);
                    _preAuthAction.GetQuadrantHeaderTitleList().ShouldCollectionBeEqual(quadrant1,
                        "Pre-Auth Action page is divided into 4 quadrant template");
                    _preAuthAction.ClickOnAddFlagIcon();
                    _preAuthAction.GetQuadrantHeaderTitleList().ShouldCollectionBeEqual(quadrant2,
                        "Pre-Auth Action page is divided into 4 quadrant template but quadrant 3 and 4 should changed");

                    _preAuthAction.IsAddFlagSectionPresent()
                        .ShouldBeTrue("Is Add Flag to Selected Lines Section Display?");
                    _preAuthAction.GetMessageBelowSelectedLines().ShouldBeEqual(
                        "Click on the desired line(s) from the Select Lines list",
                        "Is Message Below Selected Lines Equals?");

                    StringFormatter.PrintMessageTitle("Verify All Components of Add Flag Section should disabled.");
                    _preAuthAction.GetSideWindow.IsDropDownBoxDisabled("Flag")
                        .ShouldBeTrue("Flag Dropdown should disabled.");
                    _preAuthAction.GetSideWindow.IsTextBoxDisabled("Sug Paid")
                        .ShouldBeTrue("Sug Paid should disabled.");
                    _preAuthAction.GetSideWindow.IsCheckboxDisabledByLabel("Visible To Client")
                        .ShouldBeTrue("Visible to Client checkbox should disabled.");
                    _preAuthAction.GetSideWindow.IsIFrameDisabled("Note")
                        .ShouldBeTrue("Note Text Area should disabled.");
                    _preAuthAction.IsSaveButtonDisabled().ShouldBeTrue("Save Button should disabled.");
                    _preAuthAction.IsCancelButtonEnabled().ShouldBeTrue("Cancel Link should not be disabled.");

                    StringFormatter.PrintMessageTitle("Verify Select All Checkbox functionality");
                    _preAuthAction.GetSideWindow.ClickOnCheckBoxByLabel("Select All Lines");
                    _preAuthAction.IsAllLinesSelected().ShouldBeTrue("Is All Lines Selected?");
                    _preAuthAction.GetSideWindow.ClickOnCheckBoxByLabel("Select All Lines");
                    _preAuthAction.IsAllLinesSelected(false).ShouldBeTrue("Is All Lines deSelected?");

                    StringFormatter.PrintMessageTitle(
                        "Verify All Components of Add Flag Section should enabled when any claim line selected.");
                    _preAuthAction.ClickOnLinesOnSelectLines();
                    _preAuthAction.GetSideWindow.IsDropDownBoxDisabled("Flag")
                        .ShouldBeFalse("Flag Dropdown should be enabled.");
                    _preAuthAction.GetSideWindow.IsTextBoxDisabled("Sug Paid")
                        .ShouldBeFalse("Sug Paid should be enabled.");
                    _preAuthAction.GetSideWindow.IsCheckboxDisabledByLabel("Visible To Client")
                        .ShouldBeFalse("Visible to Client checkbox should be enabled.");
                    _preAuthAction.GetSideWindow.IsCheckboxCheckedByLabel("Visible To Client")
                        .ShouldBeFalse("Visible to Client checkbox should unchecked.");
                    _preAuthAction.GetSideWindow.IsIFrameDisabled("Note")
                        .ShouldBeFalse("Note Text Area should be enabled.");
                    _preAuthAction.IsSaveButtonDisabled().ShouldBeFalse("Save Button should be enabled.");
                    _preAuthAction.IsCancelButtonEnabled().ShouldBeTrue("Cancel Link should be enabled.");

                    StringFormatter.PrintMessageTitle("Verify Sug Paid should not greater than adj paid");
                    _preAuthAction.GetSideWindow.GetInputFieldText("Sug Paid")
                        .ShouldBeEqual("$0.00", "Is default value of Sug Paid Equal?");
                    var adjPaid =
                        Convert.ToDouble(_preAuthAction.GetValuesOnLinesOnSelectLines(5).Replace('$', ' ').Trim());
                    _preAuthAction.GetSideWindow.FillInputBox("Sug Paid", (adjPaid + 1).ToString("0.00"), true);
                    _preAuthAction.IsPageErrorPopupModalPresent()
                        .ShouldBeTrue("Is Popup Present for Sug Paid greater than Adj Paid?");
                    _preAuthAction.GetPageErrorMessage().ShouldBeEqual(
                        string.Format(
                            "Suggested Paid amount of ${0:0.00} cannot exceed the line Adjusted Paid amount of ${1:0.00}.",
                            adjPaid + 1, adjPaid), "Is Popup message Equals?");
                    _preAuthAction.ClosePageError();

                    StringFormatter.PrintMessageTitle(
                        "Verify Sug Paid should be disabled if more than one line is selected");
                    _preAuthAction.ClickOnLinesOnSelectLines(2);
                    _preAuthAction.GetSideWindow.IsTextBoxDisabled("Sug Paid").ShouldBeTrue("Is Sug Paid Disabled?");
                    _preAuthAction.ClickOnLinesOnSelectLines(2);
                    _preAuthAction.GetSideWindow.IsTextBoxDisabled("Sug Paid").ShouldBeFalse("Is Sug Paid Disabled?");
                    _preAuthAction.ClickOnLinesOnSelectLines(2);

                    StringFormatter.PrintMessageTitle("Verify Selected line should be in proper format");
                    _preAuthAction.GetValuesListOnLinesOnSelectedLines(3).ShouldCollectionBeEqual(
                        _preAuthAction.GetValuesListOnLinesOnSelectLines(3, true),
                        "Is Selected Lines present below the Selected Lines");
                    var selectedLines =
                        _preAuthAction.GetAllLinesListOnSelectedLines().Select(x => Regex.Split(x, "\r\n").ToList())
                            .ToList();
                    selectedLines.ShouldCollectionBeEqual(expectedSelectedList, "Is Selected List in Proper Format?");

                    StringFormatter.PrintMessageTitle("Verify All Components of Add Flag Section");
                    var expectedFlagList = _preAuthAction.GetCommonSql.GetDCIFlag();
                    _preAuthAction.GetSideWindow.GetAvailableDropDownList("Flag")
                        .ShouldCollectionBeEqual(expectedFlagList, "Is Flag List Equals?");
                    _preAuthAction.GetSideWindow.SelectDropDownListValueByLabel("Flag", expectedFlagList[0], false);
                    _preAuthAction.GetSideWindow.SelectDropDownListValueByLabel("Flag", expectedFlagList[1]);
                    _preAuthAction.GetSideWindow.SelectedDropDownOptionsByLabel("Flag")
                        .ShouldBeEqual(expectedFlagList[1], "Is User can select only single Flag?");
                    _preAuthAction.GetSideWindow.IsAsertiskPresent("Flag").ShouldBeTrue("Value should required");

                    StringFormatter.PrintMessageTitle(
                        "Verify All Components of Add Flag Section should be in default state when cancel link is clicked");
                    _preAuthAction.ClickCancel();
                    _preAuthAction.ClickOnAddFlagIcon();
                    _preAuthAction.ClickOnLinesOnSelectLines();
                    _preAuthAction.GetSideWindow.GetInputFieldText("Sug Paid")
                        .ShouldBeEqual("$0.00", "Is default value of Sug Paid Equal?");
                    _preAuthAction.GetSideWindow.GetDropDownInputFieldByLabel("Flag")
                        .ShouldBeEqual("Please Select", "Is Flag set to default value?");
                    _preAuthAction.GetSideWindow.Cancel();

                }
                finally
                {
                    _preAuthSearch.NavigateToPreAuthSearch();
                }
            }
        }

        [Test] //CAR-1516(CAR-1394) ,CAR-1590
        public void Verify_Flag_Audit_History_specifications()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                PreAuthSearchPage _preAuthSearch;
                PreAuthActionPage _preAuthAction;
                _preAuthSearch = automatedBase.QuickLaunch.NavigateToPreAuthSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var lineNumber = paramLists["LineNumber"].Split(',').ToList();
                var flag = paramLists["Flag"].Split(',').ToList();
                var labels = automatedBase.DataHelper
                    .GetMappingData(FullyQualifiedClassName, "labels").Values.ToList();
                var authSequence = paramLists["AuthSequence"];
                var authSequenceWithAudit = paramLists["AuthSequenceWithAudit"];
                _preAuthSearch.DeleteFlagAuditHistoryFromDatabaseByPreAuthSeq(authSequence);
                _preAuthAction =
                    _preAuthSearch.SearchByAuthSequenceAndNavigateToAuthAction(authSequenceWithAudit, false);
                try
                {
                    StringFormatter.PrintMessage(
                        "Verification for auth id with flag audit history record and modification date");
                    _preAuthAction.GetHeaderOfBottomRightSection()
                        .ShouldBeEqual("Line Items", "Is Lines items displayed?");
                    _preAuthAction.IsLineItemOrFlagAuditHistoryIconPresent("Flag Audit History")
                        .ShouldBeTrue("Is Flag Audit History Icon Display?");
                    _preAuthAction.ClickOnLineItemOrFlagAuditHistoryIconByTitle("Flag Audit History");
                    _preAuthAction.GetHeaderOfBottomRightSection()
                        .ShouldBeEqual("Flag Audit History", "Flag Audit History should be displayed");
                    _preAuthAction.GetFlagAuditRecordCount()
                        .ShouldBeGreater(0, "Flag audit history records should be displayed");
                    _preAuthAction.GetLinenumberOrFlagFromFlagAuditRecord(1, "FlagAuditHistory")
                        .ShouldCollectionBeEqual(lineNumber, "Line numbers should match");
                    _preAuthAction.DoesLineNumberStartsWith1(1, "FlaggedLines")
                        .ShouldBeTrue("Does Line number should start with 1?");
                    _preAuthAction.IsLineNumberOfFlagAuditHistorySorted("FlagAuditHistory")
                        .ShouldBeTrue("Are Line Numbers in an ascending order?");
                    _preAuthAction.GetLinenumberOrFlagFromFlagAuditRecord(2, "FlagAuditHistory")
                        .ShouldCollectionBeEqual(flag, "Flags should be present");
                    _preAuthAction.GetFlagAuditHistoryActionCount(1)
                        .ShouldBeGreater(0, "Audit actions should be present");
                    _preAuthAction.IsFlagAuditHistoryModDateSorted(1)
                        .ShouldBeTrue("Are Actions should be shown in descending order by Mod Date?");

                    StringFormatter.PrintMessage("Verification for no audit records for flag audit history");
                    _preAuthAction.NavigateToPreAuthSearch()
                        .SearchByAuthSequenceAndNavigateToAuthAction(authSequence, false);
                    _preAuthAction.ClickOnLineItemOrFlagAuditHistoryIconByTitle("Flag Audit History");
                    _preAuthAction.IsFlagAuditsPresent().ShouldBeFalse("Flag audit records should not be present");
                    _preAuthAction.GetNoFlagAuditRecordMessage().ShouldBeEqual(
                        "No flag audit records exist for this pre-auth.", "No audit record message should match");

                    #region CAR-1590

                    StringFormatter.PrintMessage(
                        "This is to verify that on clearing the notes, the notes will be deleted as expected");
                    var notes = "This is for testing on clearing the notes entered the notes will clear";

                    EditFlag("1", "1", 2, "", notes, notes: true, istitle: false);
                    _preAuthAction.GetSideWindow.ClearIFrame("Note");
                    _preAuthAction.ClickSaveButton();
                    _preAuthAction.WaitForWorking();
                    _preAuthAction.ClickOnLineItemOrFlagAuditHistoryIconByTitle("Line Items");
                    _preAuthAction.ClickOnLineItemOrFlagAuditHistoryIconByTitle("Flag Audit History");
                    _preAuthAction.GetFlagAuditHistoryValue(1, 2, 3, 1)
                        .ShouldBeNullorEmpty("On clearing the notes, label note should not display anything");

                    #endregion

                    StringFormatter.PrintMessage(
                        "Verification of labels and formats of record shown in flag audit history and action 'Delete'");
                    var notesValue = "Delete";
                    EditFlag("1", "1", 2, notesValue, notesValue, notes: true);
                    var reasonCode = _preAuthAction.GetSideWindow.GetDropDownListDefaultValue("Reason Code");
                    _preAuthAction.ClickSaveButton();
                    _preAuthAction.WaitForWorking();
                    _preAuthAction.ClickOnLineItemOrFlagAuditHistoryIconByTitle("Flag Audit History");
                    _preAuthAction.GetFlagAuditHistoryRecordLabels(1, 2)
                        .ShouldCollectionBeEqual(labels, "Labels should match");
                    _preAuthAction.IsModDateFormatCorrect(1, 2, 1, 1)
                        .ShouldBeTrue("Date should be in MM/DD/YYYY H:MM:SS AM/PM format");
                    _preAuthAction.GetFlagAuditHistoryValue(1, 2, 1, 2).DoesNameContainsOnlyFirstWithLastname()
                        .ShouldBeTrue("Does name consist of Firstname and lastname of user?");
                    _preAuthAction.GetFlagAuditHistoryValue(1, 2, 2, 2)
                        .ShouldBeEqual(reasonCode, "Reason code should match");
                    _preAuthAction.GetFlagAuditHistoryValue(1, 2, 2, 1)
                        .ShouldBeEqual("Delete", "Action performed should be shown");

                    StringFormatter.PrintMessage(
                        "Verification of client display being No when visible to client checkbox is unchecked");

                    _preAuthAction.GetFlagAuditHistoryValue(1, 2, 2, 3).ShouldBeEqual("No",
                        "Client display should be No if the visible to client check box is not checked");


                    _preAuthAction.GetFlagAuditHistoryValue(1, 2, 3, 1)
                        .ShouldBeEqual(notesValue, "Notes entered should match");
                    var inputNotesValue = "This is from internal";
                    StringFormatter.PrintMessage(
                        "Verification of client display being Yes when visible to client checkbox is checked and action 'Restore'");
                    EditFlag("1", "2", 2, "Restore", inputNotesValue, true, true);


                    _preAuthAction.ClickSaveButton();
                    _preAuthAction.WaitForWorking();


                    _preAuthAction.GetFlagAuditHistoryValue(1, 2, 2, 3).ShouldBeEqual("Yes",
                        "Client display should be Yes if the visible to client check box is checked");

                    _preAuthAction.GetFlagAuditHistoryValue(1, 2, 3, 1)
                        .ShouldBeEqual(inputNotesValue, "Notes entered should match");

                    StringFormatter.PrintMessage(
                        "Verification of internal user's flag audit visible to client if visible to client is selected");
                    _preAuthSearch = _preAuthAction.Logout().LoginAsClientUser()
                        .NavigateToPreAuthSearch();
                    _preAuthAction = _preAuthSearch.SearchByAuthSequenceAndNavigateToAuthAction(authSequence);
                    _preAuthAction.ClickOnLineItemOrFlagAuditHistoryIconByTitle("Flag Audit History");


                    //Commented as a part of CAR-3068
                    //StringFormatter.PrintMessage(
                    //    "Note field is present when internal user has selected the visible to client checkbox for client users");
                    //_preAuthAction.GetFlagAuditHistoryValue(1, 2, 1, 3).ShouldBeEqual("Internal");
                    //_preAuthAction.IsNotePresent(1, 2).ShouldBeTrue("Is Label Note present?");
                    //_preAuthAction.GetFlagAuditHistoryValue(1, 2, 3, 1).ShouldBeEqual(inputNotesValue,
                    //    "Notes input by internal user with visible to client selected should be visible to client users");

                    //StringFormatter.PrintMessage(
                    //    "Note field is not displayed when visible to client checkbox is unchecked by internal user to client users");
                    //_preAuthAction.GetFlagAuditHistoryValue(1, 3, 1, 3).ShouldBeEqual("Internal");
                    //_preAuthAction.IsNotePresent(1, 3).ShouldBeFalse("Is Note label present?");

                    var clientInputNotesValue = "This is for client";
                    StringFormatter.PrintMessage(
                        "Verification of flag audit of client user display to internal and Action 'Note'");
                    EditFlag("1", "1", 2, "", clientInputNotesValue, false, true, false);

                    _preAuthAction.ClickSaveButton();
                    _preAuthSearch = _preAuthAction.Logout().LoginAsHciAdminUser().NavigateToPreAuthSearch();
                    _preAuthAction = _preAuthSearch.SearchByAuthSequenceAndNavigateToAuthAction(authSequence, false);
                    _preAuthAction.ClickOnLineItemOrFlagAuditHistoryIconByTitle("Flag Audit History");
                    _preAuthAction.GetFlagAuditHistoryValue(1, 2, 1, 3)
                        .ShouldBeEqual("Client", "User type should be client");
                    _preAuthAction.GetFlagAuditHistoryValue(1, 2, 2, 1)
                        .ShouldBeEqual("Note", "Action performed should be shown");
                    _preAuthAction.GetFlagAuditHistoryValue(1, 2, 3, 1)
                        .ShouldBeEqual(clientInputNotesValue,
                            "Internal user should see notes entered by both user type");

                    StringFormatter.PrintMessage(
                        "Verification of user being able to toggle the view back to Line Items");
                    _preAuthAction.IsLineItemOrFlagAuditHistoryIconPresent("Line Items")
                        .ShouldBeTrue("Is Line items icon should be present on the left of Flag audit history icon?");
                    _preAuthAction.ClickOnLineItemOrFlagAuditHistoryIconByTitle("Line Items");
                    _preAuthAction.GetHeaderOfBottomRightSection()
                        .ShouldBeEqual("Line Items", "Lines items should be displayed");
                }

                finally
                {
                    //This is to handle if the test gets aborted before the flag has been restored, delete icon will get disbaled at this condition
                    // and the test begins with flag deletion which leads to failure in case the delete icon is disabled
                    _preAuthAction.ClickOnEditFlagIconByLineNoAndRow();
                    var isIconDisabled = _preAuthAction.IsDeleteRestoreIconDisabled("Delete");
                    if (isIconDisabled)
                    {
                        _preAuthAction.SelectReasonCode(2);
                        _preAuthAction.ClickOnDeleteOrRestoreIcon("Restore");
                        _preAuthAction.ClickSaveButton();
                    }

                    _preAuthAction.NavigateToPreAuthSearch();
                }

                void EditFlag(string lineNo, string row, int reasoncode, string title, string note = "",
                    bool client = false, bool notes = false, bool istitle = true)
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
            }
        }

        [Test] // CAR-915 (CAR-1387) + + CAR-2888(CAR-2870) + CAR-CAR-2887(CAR-2846)
        public void Verify_Pre_Auth_Action_Upper_Left_Quadrant()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                PreAuthSearchPage _preAuthSearch;
                PreAuthActionPage _preAuthAction;
                _preAuthSearch = automatedBase.QuickLaunch.NavigateToPreAuthSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var _authSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName,
                    "AuthSequence", "Value");
                _preAuthAction = _preAuthSearch.SearchByAuthSequenceAndNavigateToAuthAction(_authSeq, false);

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
                _preAuthAction.GetUpperLeftQuadrantValueByLabel("Image ID")
                    .ShouldBeEqual(imageId, "Image ID");
            }
        }

        [Test] //CAR-1432(CAR-1088)
        public void Verify_flagged_line_data_points()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                PreAuthSearchPage _preAuthSearch;
                PreAuthActionPage _preAuthAction;
                _preAuthSearch = automatedBase.QuickLaunch.NavigateToPreAuthSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var authSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName,
                    "AuthSequence", "Value");
                var authSeqWithTriggerInfo = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName,
                    "AuthSequenceWithTriggerInfo", "Value");
                _preAuthSearch.DeleteFlagAndAuditByAuthSeqFlagsLinNo(authSeq, "'DELE'", "2");
                _preAuthSearch.DeleteFlagAndAuditByAuthSeqFlagsLinNo(authSeq, "'DELE'", "3");
                _preAuthAction = _preAuthSearch.SearchByAuthSequenceAndNavigateToAuthAction(authSeq, false);

                _preAuthAction.IsFlaggedLineRowDivPresentByLineNo(1)
                    .ShouldBeTrue("Is Flag Div Row Present for having flag?");
                _preAuthAction.IsFlaggedLineRowDivPresentByLineNo(3)
                    .ShouldBeFalse("Is Flag Div Row Present for not having flag?");

                _preAuthAction.GetFlagListByDivList()
                    .ShouldCollectionBeEqual(_preAuthAction.GetAllFlagList(authSeq),
                        "Is Deleted Flag List Equals?");

                _preAuthAction.GetFlagListByDivList(2)
                    .ShouldCollectionBeEquivalent(_preAuthAction.GetAllFlagList(authSeq, 2),
                        "Is System Deleted Flag List Equals?");

                _preAuthAction.IsApproveIconPresent().ShouldBeTrue("Is Approve Icon Display?");
                _preAuthAction.IsAddFlagIconPresent().ShouldBeTrue("Is Add Flag Icon Display?");
                _preAuthAction.IsTransferIconPresent().ShouldBeTrue("Is Transfer Approve Icon Display?");
                _preAuthAction.IsHistoryIconPresent().ShouldBeTrue("Is History Icon Display?");
                _preAuthAction.IsNextIconPresent().ShouldBeTrue("Is Next Icon Display?");

                _preAuthAction.GetFlagLineNumberList().IsInAscendingOrder()
                    .ShouldBeTrue("Flagged Line should display ascending by line number");

                _preAuthAction.GetFlagLineLevelHeaderDetailValue(col: 2).IsDateInFormat()
                    .ShouldBeTrue("Is DOS is in Date format?");

                var procCode = _preAuthAction.GetFlagLineLevelHeaderDetailValue(col: 3);
                var procCodePage = _preAuthAction.ClickOnProcCodeAndSwitch("CDT Code", procCode);
                procCodePage.GetTextValueinLiTag(1)
                    .ShouldBeEqual(string.Concat("Code: ", procCode), "Is Correct popup display?");
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
                    .ShouldBeEqual("Scen:", "Is OC Label Displayed?");

                _preAuthAction.GetFlagLineLevelHeaderDetailValue(row: 2, col: 4)
                    .ShouldBeEqual("A", "Is Scenario Value Displayed?");

                _preAuthAction.ClickOnReturnToPreAuthSearch()
                    .SearchByAuthSequenceAndNavigateToAuthAction(authSeqWithTriggerInfo);

                var trigProcCode = _preAuthAction.GetFlagRowDetailValue(col: 5);
                var trigProcCodePage = _preAuthAction.ClickOnProcCodeAndSwitch("CDT Code", trigProcCode);
                trigProcCodePage.GetTextValueinLiTag(1)
                    .ShouldBeEqual(string.Concat("Code: ", trigProcCode), "Is Correct popup display?");
                _preAuthAction.CloseAnyTabIfExist();

                var flag = _preAuthAction.GetFlagRowDetailValue(col: 3);
                var flagPopupPage = _preAuthAction.ClickOnFlagandSwitch("Flag Information - " + flag);
                flagPopupPage.GetPopupHeaderText().ShouldBeEqual("Flag - " + flag, "Is Correct Popup Page display?");
                _preAuthAction.CloseAnyTabIfExist();


                _preAuthAction.IsFlagRowEditIconPresent().ShouldBeTrue("Is Edit Icon Display in Flag Row?");
                _preAuthAction.IsFlagRowAddLogicIconPresent().ShouldBeTrue("Is Logic Icon Displayed in Flag Row?");


                _preAuthAction.GetFlagRowDetailLabel(col: 4).ShouldBeEqual("S:", "Is TN Label Displayed?");
                _preAuthAction.GetFlagRowDetailLabel(col: 7).ShouldBeEqual("TL:", "Is TS Label Displayed?");

                _preAuthAction.GetFlagRowDetailValue(col: 4).ShouldNotBeEmpty("Is Source Value Displayed?");
                var triggerProcCode = _preAuthAction.GetFlagRowDetailValue(col: 5);
                triggerProcCode.ShouldNotBeEmpty("Is Trigger Proc Value Displayed?");
                _preAuthAction.GetFlagRowDetailValue(col: 6)
                    .ShouldBeEqual(_preAuthAction.GetProcDesc(triggerProcCode, "DEN"), "Is Triger Proc Value Display?");
                //_preAuthAction.GetFlagRowDetailValue(col: 7).ShouldNotBeEmpty("Is Triger Auth Sequence Value Display?");
                _preAuthAction.GetFlagRowDetailValue(col: 7).ShouldNotBeEmpty("Is Trigger Line No Value Display?");


                var actualflagList = _preAuthAction.GetFlagListByDivList();
                _preAuthAction
                    .GetFlagOrderSequenceListByFlag("'" + string.Join(",", actualflagList).Replace(",", "','") + "'")
                    .ShouldCollectionBeEqual(actualflagList,
                        "Is Flags are displayed ascending order by flag order from Edit Table?");
            }
        }


        [Test] //CAR-267
        public void Verify_Navigation_To_Pre_Auth_Action_And_Template()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                PreAuthSearchPage _preAuthSearch;
                PreAuthActionPage _preAuthAction;
                _preAuthSearch = automatedBase.QuickLaunch.NavigateToPreAuthSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var quadrants = paramLists["Quadrants"].Split(',').ToList();
                var toolTipTexts = paramLists["ToolTips"].Split(',').ToList();

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
        public void Verify_Line_item_data_points()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                PreAuthSearchPage _preAuthSearch;
                PreAuthActionPage _preAuthAction;
                _preAuthSearch = automatedBase.QuickLaunch.NavigateToPreAuthSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var authSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName,
                    "AuthSequence",
                    "Value");
                _preAuthAction = _preAuthSearch.SearchByAuthSequenceAndNavigateToAuthAction(authSeq, false);
                var lineDetails = _preAuthAction.GetLineDetailsValuesFromDB(authSeq);
                string procCodeDescShort;
                string procCodeDescDetail;
                List<string> flagList = new List<string>();

                var lineNoList = _preAuthAction.GetLineItemsValueListByRowColumn(1, 1);
                lineNoList.IsInAscendingOrder().ShouldBeTrue("Line Number Should be in Ascending Order.");
                var flagNoList = _preAuthAction.GetFlagLineNumberList();
                for (int j = 1; j <= flagNoList.Count; j++)
                {
                    var flag = _preAuthAction.GetFlagListByDivList(j)[0];
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
                    lineNoList[i].ShouldBeEqual(lineDetails[i][0],
                        string.Format("Line number should be equal to {0}.", lineDetails[i][0]));
                    dosList[i].ShouldBeEqual(Convert.ToDateTime(lineDetails[i][1]).ToString("MM/dd/yyyy"),
                        string.Format("Date of Service value should be equal to {0}.",
                            Convert.ToDateTime(lineDetails[i][1]).ToString("MM/dd/yyyy")));
                    procCodeList[i].ShouldBeEqual(lineDetails[i][2],
                        string.Format("Proc Code value should be equal to {0}.", lineDetails[i][2]));

                    _preAuthAction.GetProcCodeDescription()[i].ShouldBeEqual(procCodeDescShort,
                        string.Format("Proc Code Description value should be equal to {0}.",
                            procCodeDescShort));
                    NewPopupCodePage _popupCode =
                        _preAuthAction.ClickOnLineDetailsProcCodeAndSwitch("CDT Code", lineDetails[i][2], i + 1);
                    _popupCode.GetProcCodeDescriptionFromPopUp().Contains(procCodeDescDetail)
                        .ShouldBeTrue(string.Format("Proc Code Descripton should consist {0}.", procCodeDescDetail));
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
        }

        [Test] //CAR-1096 (CAR-1514) 
        public void Verify_Delete_Restore_Flag_Option_Is_Not_Available_To_Users_Without_Manage_Edits_Privilege()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                PreAuthSearchPage _preAuthSearch;
                PreAuthActionPage _preAuthAction;
                _preAuthSearch = automatedBase.QuickLaunch.NavigateToPreAuthSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramsList =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var preAuthSeqWithFlags = paramsList["AuthSequenceWithFlags"];

                StringFormatter.PrintMessageTitle(
                    "Verifying that Edit Flag icons should be disabled for users without the 'Manage Edits' privilege");
                _preAuthAction = automatedBase.CurrentPage.Logout().LoginAsUserHavingNoManageEditAuthority()
                    .NavigateToPreAuthSearch()
                    .SearchByAuthSequenceAndNavigateToAuthAction(preAuthSeqWithFlags, false);

                _preAuthAction.GetCountOfEnabledEditFlagIcons().ShouldBeEqual(0,
                    "Edit Flag icons should be disabled for users without 'Manage Edits' privilege ");
            }
        }
        
       [Test] //CAR-1096 (CAR-1514) + CAR-3201(CAR-3182)
       [Author("Shreya Shrestha")]
       [Retrying(Times = 3)]
        public void Verify_Delete_Restore_Flag_In_Pre_Auth_Action_Page()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                PreAuthSearchPage _preAuthSearch;
                PreAuthActionPage _preAuthAction;
                _preAuthSearch = automatedBase.QuickLaunch.NavigateToPreAuthSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramsList =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var preAuthSeqWithFlags = paramsList["AuthSequenceWithFlags"];
                
                // Setting any deleted flags to undeleted state and removing Flag Audit History from the DB
                _preAuthSearch.UnDeleteAnyFlagByPreAuthSeq(preAuthSeqWithFlags);
                _preAuthSearch.UpdateHciDoneOrClientDoneToFalseFromDatabase("clidone", preAuthSeqWithFlags);
                _preAuthSearch.DeleteFlagAuditHistoryFromDatabaseByPreAuthSeq(preAuthSeqWithFlags);

                _preAuthAction = _preAuthSearch.SearchByAuthSequenceAndNavigateToAuthAction(preAuthSeqWithFlags, false);
                var flagList = _preAuthAction.GetLineItemsValueListByRowColumn();
                var rnd = new Random();
                var lineNum = rnd.Next(1, flagList.Count);

                #region CAR-3201(CAR-3182)
                StringFormatter.PrintMessage("Verification of presence of quick delete icon");
                _preAuthAction.IsQuickDeleteIconPresent().ShouldBeTrue("Is quick delete icon present?");
                _preAuthAction.IsAllCliDONETrue(preAuthSeqWithFlags).ShouldBeFalse("Is flag marked done?");
                _preAuthAction.ClickQuickDeleteIcon();

                StringFormatter.PrintMessage("Verification of all flags being deleted and struck through");
                _preAuthAction.AreAllFlagsDeleted().ShouldBeTrue("All the flags are deleted?");
                _preAuthAction.AreFlagsStruckThrough().ShouldBeTrue("Are all flags struck through?");

                StringFormatter.PrintMessage("Verification of flag audit history");
                VerificationOfFlagAuditHistory(lineNum, 2, paramsList,quickDeleteReasonCode:true, quickDelete:true);

                StringFormatter.PrintMessage("Verification that flags are set to done on clicking quick delete icon");
                _preAuthAction.IsAllCliDONETrue(preAuthSeqWithFlags).ShouldBeTrue("Is flag marked done?");

                StringFormatter.PrintMessage("Verification that quick delete icon is replaced by quick restore icon after clicking quick delete");
                _preAuthAction.IsQuickDeleteIconPresent().ShouldBeFalse("Is quick delete icon present?");
                _preAuthAction.IsQuickRestoreIconPresent().ShouldBeTrue("Is quick restore icon present?");

                StringFormatter.PrintMessage("Verification of quick restore");
                _preAuthAction.ClickQuickRestoreIcon();
                _preAuthAction.AreAllFlagsDeleted().ShouldBeFalse("All the flags are deleted?");
                _preAuthAction.AreFlagsStruckThrough().ShouldBeFalse("Are all flags struck through?");

                StringFormatter.PrintMessage("Verification of flag audit history for quick restore icon");
                VerificationOfFlagAuditHistory(lineNum, 2, paramsList,false, true, true);

                StringFormatter.PrintMessage("Resetting flags to default configuration");
                _preAuthSearch.DeleteFlagAuditHistoryFromDatabaseByPreAuthSeq(preAuthSeqWithFlags);
                _preAuthSearch.UpdateHciDoneOrClientDoneToFalseFromDatabase("clidone", preAuthSeqWithFlags);
                _preAuthAction.RefreshPage(false);
                _preAuthAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();

                #endregion

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
                //Verifying delete icon is accessible when there are no deleted flags in the pre-auth
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
                var listOfReasonCodesFromDB = _preAuthAction.GetReasonCodeListFromDB();

                listOfReasonCodesFromDropdown.Remove("");
                listOfReasonCodesFromDropdown.ShouldCollectionBeEqual(listOfReasonCodesFromDB,
                    "Does the 'Reason Code' dropdown contain correct items reason codes ?");

                StringFormatter.PrintMessageTitle("Verifying 'Note' field in the edit flag form");
                _preAuthAction.IsVisibleToClientIconChecked()
                    .ShouldBeFalse("Is 'Visible To Client' checkbox checked by default?");
                _preAuthAction.ClickVisibleToClientIcon();
                _preAuthAction.IsVisibleToClientIconChecked().ShouldBeTrue("Is 'Visible to Client' checkbox checked? ");
                _preAuthAction.GetSideWindow.IsAsertiskPresent("Note")
                    .ShouldBeFalse("Is 'Note' field a required field ?");
                _preAuthAction.ClickCancel();

                StringFormatter.PrintMessageTitle("Verifying Delete/Restore of Flags");
                var flags = paramsList["Flags"].Split(';').ToList();
                var flagSavings = paramsList["FlagSavings"].Split(';').ToList();

                VerifyDeleteRestoreOfFlags(preAuthSeqWithFlags, 3, flags[0]);
                VerifyTopFlagOrderAndSavings(3, flags, flagSavings);
                VerificationOfFlagAuditHistory(3, 2, paramsList);

                VerifyDeleteRestoreOfFlags(preAuthSeqWithFlags, 3, flags[0], false);
                VerifyTopFlagOrderAndSavings(3, flags, flagSavings, false);
                VerificationOfFlagAuditHistory(3, 2, paramsList, false);

                StringFormatter.PrintMessageTitle("Verification Color of System Deleted Flags");
                var authSeqWithSysDeletedFlag = paramsList["AuthSeqWithSysDeletedFlag"];
                _preAuthSearch = _preAuthAction.ClickOnReturnToPreAuthSearch();
                _preAuthAction = _preAuthSearch.SearchByAuthSequenceAndNavigateToAuthAction(authSeqWithSysDeletedFlag, false);
                _preAuthAction.GetFlagTextColor(1, 1, 3).Contains("rgba(0, 0, 0, 1)")
                    .ShouldBeTrue("System Deleted Flag Name should be shown in Black");

                void VerificationOfFlagAuditHistory(int lineNo, int auditRecordRow, IDictionary<string, string> paramList, bool delete = true, bool quickDeleteReasonCode = false, bool quickDelete = false)
                {
                    var listCounter = 0;
                    IDictionary<string, string> flagAuditHistoryDetails = new Dictionary<string, string>();
                    var modifiedBy = paramList["ModifiedBy"];
                    var userType = paramList["UserType"];
                    var deleteActionText = paramList["DeleteActionText"];
                    var restoreActionText = paramList["RestoreActionText"];
                    var reasonCode = paramList["ReasonCode"];
                    var noClientDisplay = paramList["NoClientDisplay"];
                    var note = paramList["Note"];

                    List<string> listOfFlagAuditHistoryLabels = new List<string>(new string[]
                    {
                "Mod Date",
                "By",
                "User Type",
                "Action",
                "Reason Code",
                "Client Display",
                "Notes"
                    });

                    _preAuthAction.ClickOnLineItemOrFlagAuditHistoryIconByTitle("Flag Audit History");

                    for (int i = 1; i < 4; i++)
                    {
                        // Stores the 'Notes' value from the Flag Audit History to the IDictionary
                        if (i == 3)
                        {
                            flagAuditHistoryDetails.Add(listOfFlagAuditHistoryLabels[listCounter],
                                _preAuthAction.GetFlagAuditHistoryValue(lineNo, auditRecordRow, i, 1));
                            break;
                        }

                        // Stores all values except 'Notes' value from the Flag Audit History to the IDictionary
                        for (int j = 1; j < 4; j++)
                        {
                            flagAuditHistoryDetails.Add(listOfFlagAuditHistoryLabels[listCounter],
                                _preAuthAction.GetFlagAuditHistoryValue(lineNo, auditRecordRow, i, j));
                            listCounter++;
                        }
                    }

                    flagAuditHistoryDetails["Mod Date"].Contains(DateTime.Now.ToString("MM/dd/yyyy")).ShouldBeTrue("Is 'Mod Date' being correctly displayed?");
                    flagAuditHistoryDetails["By"].Contains(modifiedBy).ShouldBeTrue("Is 'By' being correctly displayed?");
                    flagAuditHistoryDetails["User Type"].Contains(userType).ShouldBeTrue("Is 'User Type' being correctly displayed?");
                    flagAuditHistoryDetails["Action"].Contains(delete ? deleteActionText : restoreActionText).ShouldBeTrue("Is 'Action' being correctly displayed?");
                    flagAuditHistoryDetails["Reason Code"].Contains(quickDeleteReasonCode? "DEN 9 - Review By Analyst" : reasonCode).ShouldBeTrue("Is 'Reason Code' being correctly displayed?");
                    flagAuditHistoryDetails["Client Display"].Contains(noClientDisplay).ShouldBeTrue("Is 'Client Display' being correctly displayed?");
                    flagAuditHistoryDetails["Notes"].Contains(quickDelete? "" : delete ? "Delete Note" : "Restore Note").ShouldBeTrue("Is 'Notes' being correctly displayed?");
                }

                void VerifyTopFlagOrderAndSavings(int lineNo, List<string> flagsList, List<string> flagSavingsList,
                    bool delete = true)
                {
                    var flagToBeDeletedOrRestored = flagsList[0];
                    var updatedTopFlag = flagsList[1];
                    var savingForDeletedOrRestoredFlag = flagSavingsList[0];
                    var savingsForUpdatedTopFlag = flagSavingsList[1];

                    _preAuthAction.ClickOnLineItemOrFlagAuditHistoryIconByTitle("Line Items");
                    _preAuthAction.ScrollToLastDiv("FlaggedLines");
                    if (delete)
                    {
                        StringFormatter.PrintMessage("Verifying deleted top flag is not being shown as top flag");
                        var topFlag = _preAuthAction.GetFlagRowDetailValue(lineNo, 1, 3);
                        topFlag.ShouldNotBeEqual(flagToBeDeletedOrRestored,
                            "Top flag should not be shown as the top flag in Flagged Lines container once the flag is deleted");
                        topFlag.ShouldBeEqual(updatedTopFlag,
                            "Is top flag adjusted so that the next top flag on the line is displayed first?");

                        StringFormatter.PrintMessage("Verifying flag order is updated once top flag is deleted");
                        int i = 0;
                        for (; i < 3; i++)
                        {
                            _preAuthAction.GetFlagRowDetailValue(lineNo, i + 1, 3).ShouldBeEqual(flagsList[i + 1]);
                        }

                        _preAuthAction.GetFlagRowDetailValue(lineNo, 4, 3).ShouldBeEqual(flagsList[0]);
                        var listOfFlagsInLineItems = _preAuthAction.GetLineItemsValueListByRowColumn(1, 2);

                        StringFormatter.PrintMessage(
                            "Verifying whether top flag adjusted automatically in Line Items once flag is deleted");
                        listOfFlagsInLineItems[2].ShouldBeEqual(updatedTopFlag,
                            "Is top flag adjusted automatically in Line Items once flag is deleted ?");

                        _preAuthAction.GetFlagLineLevelHeaderDetailValue(lineNo, 2, 3)
                            .ShouldBeEqual(savingsForUpdatedTopFlag);

                    }

                    else
                    {
                        StringFormatter.PrintMessage("Verifying restored top flag is being shown as top flag");
                        var topFlag = _preAuthAction.GetFlagRowDetailValue(lineNo, 1, 3);
                        topFlag.ShouldBeEqual(flagToBeDeletedOrRestored,
                            "Top flag should be shown as the top flag in Flagged Lines container once the flag is restored");

                        StringFormatter.PrintMessage("Verifying flag order is updated once top flag is restored");
                        int i = 0;
                        for (; i < 4; i++)
                        {
                            _preAuthAction.GetFlagRowDetailValue(lineNo, i + 1, 3).ShouldBeEqual(flagsList[i]);
                        }

                        var listOfFlagsInLineItems = _preAuthAction.GetLineItemsValueListByRowColumn(1, 2);

                        StringFormatter.PrintMessage(
                            "Verifying whether top flag adjusted automatically in Line Items once flag is restored");
                        listOfFlagsInLineItems[2].ShouldBeEqual(flagToBeDeletedOrRestored,
                            "Is top flag adjusted automatically in Line Items once flag is deleted ?");

                        _preAuthAction.GetFlagLineLevelHeaderDetailValue(lineNo, 2, 3)
                            .ShouldBeEqual(savingForDeletedOrRestoredFlag);
                    }

                }

                void VerifyDeleteRestoreOfFlags(string authSeq, int lineNo, string flagName, bool delete = true)
                {
                    _preAuthAction.ScrollToLastDiv("FlaggedLines");
                    // Storing the initial Savings value before either deleting or restoring
                    if (delete)
                    {
                        StringFormatter.PrintMessage("Verifying Deleting a flag");
                        _preAuthAction.DeleteRestoreFlagByLineNoAndFlagName(lineNo, flagName);

                        var hciDoneAndCliDoneFromDB =
                            _preAuthAction.GetHciDoneAndCliDoneValuesByAuthSeqLinNoFlagName(authSeq, lineNo, flagName);
                        hciDoneAndCliDoneFromDB[0][0].ShouldBeEqual(hciDoneAndCliDoneFromDB[0][1])
                            .ShouldBeEqual("T", "Are HciDone and CliDone = 'T' after deleting flag?");

                        var countOfFlagsInDiv = _preAuthAction.GetFlagListByDivList(3).Count;
                        _preAuthAction.IsFlagRowDeletedPresent(3, countOfFlagsInDiv)
                            .ShouldBeTrue("Is Flag row deleted after deleting the flag ?");

                        StringFormatter.PrintMessage(
                            "Verifying flag detail row has a strikethrough after it is deleted");
                        for (int i = 3; i < 8; i++)
                            _preAuthAction.IsStrikeThroughPresentInFlagDetail(3, countOfFlagsInDiv, i)
                                .ShouldBeTrue("Is flag detail striked through ?");
                    }

                    else
                    {
                        StringFormatter.PrintMessage("Verifying Restoring a flag");
                        _preAuthAction.DeleteRestoreFlagByLineNoAndFlagName(lineNo, flagName, false);
                        _preAuthAction.WaitForStaticTime(2000);

                        var hciDoneAndCliDoneFromDB =
                            _preAuthAction.GetHciDoneAndCliDoneValuesByAuthSeqLinNoFlagName(authSeq, lineNo, flagName);
                        hciDoneAndCliDoneFromDB[0][0].ShouldBeEqual("T", "Is HCIDONE = 'T' after restoring the flag ?");
                        hciDoneAndCliDoneFromDB[0][1].ShouldBeEqual("F", "Is CLIDONE = 'F' after restoring the flag ?");
                    }
                }

            }
        }

        [Test] //CAR-1096 (CAR-1514)
        public void Verify_Cancel_Button_In_Delete_Restore_Flag_In_Pre_Auth_Action_Page()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                PreAuthSearchPage _preAuthSearch;
                PreAuthActionPage _preAuthAction;
                _preAuthSearch = automatedBase.QuickLaunch.NavigateToPreAuthSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramsList =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var preAuthSeq = paramsList["PreAuthSeq"];
                var flag = paramsList["Flag"];

                // Removing any flag deletes and Audit history from the Pre-Auth before continuing with the test
                _preAuthSearch.UnDeleteAnyFlagByPreAuthSeq(preAuthSeq);
                _preAuthSearch.DeleteFlagAuditHistoryFromDatabaseByPreAuthSeq(preAuthSeq);

                StringFormatter.PrintMessage("Verifying Cancel button while performing Edit Action");
                _preAuthAction = _preAuthSearch.SearchByAuthSequenceAndNavigateToAuthAction(preAuthSeq, false);

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
        public void Verify_Next_Button_In_Pre_Auth_Action_Page()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                PreAuthSearchPage _preAuthSearch;
                PreAuthActionPage _preAuthAction;
                _preAuthSearch = automatedBase.QuickLaunch.NavigateToPreAuthSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                try
                {
                    _preAuthSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                        PreAuthQuickSearchEnum.OutstandingPreAuths.GetStringValue());
                    _preAuthSearch.ClickOnFindButton();
                    _preAuthSearch.WaitForWorking();
                    var totalSearchResultCount = _preAuthSearch.GetGridViewSection.GetGridRowCount();
                    var preAuthSearchResultDetails = _preAuthSearch.GetSearchResultUptoRow(4, true);

                    _preAuthAction = _preAuthSearch.ClickOnFirstUnlockedPreAuthToNavigateToPreAuthActionPage();

                    StringFormatter.PrintMessageTitle(
                        "Verifying whether 'Next' icon is disabled while 'Working' message is displayed");
                    var preAuthFlagDetails = VerifyNextPreAuthDetailsAndReturnFlagDetails(preAuthSearchResultDetails);
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

                    VerifyNextPreAuthDetailsAndReturnFlagDetails(preAuthSearchResultDetailsSortedByAuthSeq);
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
                finally
                {
                    if (automatedBase.CurrentPage.GetPageHeader() != PageHeaderEnum.PreAuthSearch.GetStringValue())
                        automatedBase.CurrentPage.NavigateToPreAuthSearch();
                    //Clear sort and filter options
                    _preAuthSearch.ClickOnFilterOptionListRow(7);

                    if (!_preAuthSearch.GetSideBarPanelSearch.IsSideBarPanelOpen())
                        _preAuthSearch.ClickOnSidebarIcon();

                    _preAuthSearch.GetSideBarPanelSearch.ClickOnClearLink();
                }

                Dictionary<string, List<string>> VerifyNextPreAuthDetailsAndReturnFlagDetails(
                    Dictionary<string, List<string>> expectedPreAuthDetailsFromSearchPage, int window = 0)
                {
                    var flagDetails = new Dictionary<string, List<string>>();

                    int count;
                    for (count = 0; count < 3; count++)
                    {
                        var nextPreAuthSeq = expectedPreAuthDetailsFromSearchPage.Keys.ElementAt(count + 1);
                        _preAuthAction.ClickOnNextIconAndCheckIfNextIconIsDisabled()
                            .ShouldBeTrue("Is 'Next' icon disabled while 'Working' message is displayed ?");
                        _preAuthAction.WaitForWorkingAjaxMessage();
                        _preAuthAction.CloseCurrentWindowAndSwitchToOriginal(window);

                        var preAuthSeqFromPreAuthActionPage =
                            _preAuthAction.GetUpperLeftQuadrantValueByLabel("Auth Seq");
                        preAuthSeqFromPreAuthActionPage.ShouldBeEqual(nextPreAuthSeq,
                            "Is the Pre-Auth Seq as expected when 'Next' icon is clicked ?");

                        var preAuthDetails = expectedPreAuthDetailsFromSearchPage[preAuthSeqFromPreAuthActionPage];

                        _preAuthAction.GetUpperLeftQuadrantValueByLabel("Review Type").ShouldBeEqual(preAuthDetails[0],
                            "Is 'Review Type' as expected when 'Next' icon is clicked ?");
                        _preAuthAction.GetUpperLeftQuadrantValueByLabel("Patient Seq").ShouldBeEqual(preAuthDetails[1],
                            "Is the 'Patient Seq' Seq as expected when 'Next' icon is clicked ?");
                        _preAuthAction.GetUpperLeftQuadrantValueByLabel("Provider Seq").ShouldBeEqual(preAuthDetails[2],
                            "Is the 'Provider Seq' as expected when 'Next' icon is clicked ?");
                        _preAuthAction.GetUpperLeftQuadrantValueByLabel("State").ShouldBeEqual(preAuthDetails[4],
                            "Is the 'State' as expected when 'Next' icon is clicked ?");
                        _preAuthAction.GetUpperLeftQuadrantValueByLabel("Status").ShouldBeEqual(preAuthDetails[5],
                            "Is the 'Status' as expected when 'Next' icon is clicked ?");

                        var countOfDeletedFlags = _preAuthAction.GetCountOfDeletedFlags().ToString();
                        var preAuthStatus = _preAuthAction.GetUpperLeftQuadrantValueByLabel("Status");

                        flagDetails.Add(nextPreAuthSeq, new List<string>()
                {
                    countOfDeletedFlags , preAuthStatus
                });

                    }

                    _preAuthAction.ClickOnReturnToPreAuthSearch();
                    return flagDetails;
                }
            }
        }

        [Test] //CAR-1087 (CAR-1705)
        public void Verify_pre_auth_processing_history_pop_up()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                PreAuthSearchPage _preAuthSearch;
                PreAuthActionPage _preAuthAction;
                PreAuthProcessingHistoryPage _preAuthProcessingHistory;
                _preAuthSearch = automatedBase.QuickLaunch.NavigateToPreAuthSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramsList =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var preAuthSeq = paramsList["PreAuthSeq"];
                var columnHeaderList = paramsList["ColumnHeaderList"].Split(';').ToList();
                var expectedAuditData = paramsList["AuditData"].Split(';').ToList();
                _preAuthSearch.DeleteHistoryAndUpdateStatusByAuthSeq(preAuthSeq, "06-JAN-19",
                    StatusEnum.CotivitiUnreviewed.GetStringValue());
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

        [Test] //CAR-1457 (CAR-1703)
        public void Verify_transfer_pre_auth()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                PreAuthSearchPage _preAuthSearch;
                PreAuthActionPage _preAuthAction;
                PreAuthProcessingHistoryPage _preAuthProcessingHistory;

                _preAuthSearch = automatedBase.QuickLaunch.NavigateToPreAuthSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var testData = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var memberID = testData["Member_Id"];
                var provSeq = testData["ProvSeq"];
                var preAuthSeq = testData["PreAuth"];
                var preAuthWithDocsReviewStatus = testData["PreAuthWithDocsReviewStatus"];
                var preAuthWithDocsRequestedStatus = testData["PreAuthWithDocsRequestedStatus"];
                var preAuthsForWhichTransferIconIsDisabled =
                    testData["PreAuthsForWhichTransferIconIsDisabled"].Split(';').ToList();

                var orderOfStatusAssignment = new List<string>
                {
                    StatusEnum.CotivitiConsultantRequired.GetStringDisplayValue(),
                    StatusEnum.CotivitiConsultantComplete.GetStringDisplayValue(),
                    StatusEnum.StateConsultantRequired.GetStringDisplayValue(),
                    StatusEnum.StateConsultantComplete.GetStringDisplayValue(),
                    StatusEnum.DocumentsRequired.GetStringDisplayValue()
                };

                _preAuthSearch.UpdateStatusOfAuthSeqFromDatabase("U", preAuthSeq);
                _preAuthSearch.UpdateStatusOfAuthSeqFromDatabase("D", preAuthWithDocsReviewStatus);
                _preAuthSearch.UpdateStatusOfAuthSeqFromDatabase("R", preAuthWithDocsRequestedStatus);

                try
                {
                    StringFormatter.PrintMessageTitle(
                        "Verification Transfer Icon will disabled for Client Unreviewed or Closed");
                    foreach (var preAuth in preAuthsForWhichTransferIconIsDisabled)
                    {
                        _preAuthAction = _preAuthSearch.SearchByAuthSequenceAndNavigateToAuthAction(preAuth, false);
                        var status = _preAuthAction.GetUpperLeftQuadrantValueByLabel("Status");
                        _preAuthAction.IsTransferIconEnabled()
                            .ShouldBeFalse($"Transfer Icon should be disabled for {status}");
                        _preAuthAction.ClickOnReturnToPreAuthSearch();
                        _preAuthSearch.GetSideBarPanelSearch.ClickOnClearLink();
                    }

                    StringFormatter.PrintMessageTitle(
                        "Verification Transfer Icon and drop down list of status for preauth having different status");
                    _preAuthSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                        PreAuthQuickSearchEnum.AllPreAuths.GetStringValue());
                    _preAuthSearch.SetInputFieldByInputLabel("Member ID", memberID);
                    _preAuthSearch.SetInputFieldByInputLabel("Provider Sequence", provSeq);
                    _preAuthSearch.ClickOnFindButton();
                    _preAuthSearch.WaitForWorking();

                    var preAuthSeqListFromGrid = _preAuthSearch
                        .GetSearchResultUptoRow(_preAuthSearch.GetGridViewSection.GetGridRowCount(), true).Keys
                        .ToList();
                    _preAuthAction = _preAuthSearch.ClickOnAuthSeqToNavigateToPreAuthActionPage(preAuthSeq);
                    _preAuthAction.WaitForWorking();

                    var preAuthStatus = _preAuthAction.GetUpperLeftQuadrantValueByLabel("Status");

                    StringFormatter.PrintMessageTitle("Validation of Transfer Form");
                    _preAuthAction.ClickOnTransferIcon();
                    _preAuthAction.IsTransferPreAuthFormPresent()
                        .ShouldBeTrue(
                            "The Transfer Pre-Auth form section should be present once Transfer button is clicked");

                    _preAuthAction.GetSideWindow.IsAsertiskPresent("Status")
                        .ShouldBeTrue("Status should be a required field");
                    _preAuthAction.GetSideWindow.IsAsertiskPresent("Note")
                        .ShouldBeFalse("Note should not be a required field");

                    var noteText = new string('a', 501);
                    _preAuthAction.SetNoteInTransferPreAuth(noteText);
                    var noteInTransferTextArea = _preAuthAction.GetNoteFromTransferPreAuth();
                    noteInTransferTextArea.Length.ShouldBeEqual(500,
                        "The Note field should allow only upto 500 characters");

                    _preAuthAction.ClickCancel();
                    _preAuthAction.IsTransferPreAuthFormPresent()
                        .ShouldBeFalse("Clicking on 'Cancel' should close the 'Transfer Pre-Auth' form");
                    _preAuthAction.GetUpperLeftQuadrantValueByLabel("Status")
                        .ShouldBeEqual(preAuthStatus, "Pre-Auth Status should remain unchanged if Cancel is clicked");

                    _preAuthAction.ClickOnTransferIcon();

                    var currentPreAuthSeq = _preAuthAction.GetUpperLeftQuadrantValueByLabel("Auth Seq");
                    var statusList = _preAuthAction.GetSideWindow.GetAvailableDropDownList("Status");
                    statusList.Remove(preAuthStatus);

                    VerifyTransferFunctionality(preAuthSeqListFromGrid, currentPreAuthSeq, orderOfStatusAssignment);

                    _preAuthSearch = _preAuthAction.ClickOnReturnToPreAuthSearch();
                    _preAuthSearch.GetSideBarPanelSearch.ClickOnClearLink();
                    _preAuthAction =
                        _preAuthSearch.SearchByAuthSequenceAndNavigateToAuthAction(preAuthWithDocsReviewStatus, false);

                    _preAuthAction.ClickOnTransferIcon();
                    statusList = _preAuthAction.GetSideWindow.GetAvailableDropDownList("Status");
                    statusList.Remove(StatusEnum.DocumentReview.GetStringDisplayValue());
                    VerifyStatusDropdownInTransferForm(StatusEnum.DocumentReview.GetStringDisplayValue(), statusList);

                    _preAuthSearch = _preAuthAction.ClickOnReturnToPreAuthSearch();
                    _preAuthSearch.GetSideBarPanelSearch.ClickOnClearLink();
                    _preAuthAction =
                        _preAuthSearch.SearchByAuthSequenceAndNavigateToAuthAction(preAuthWithDocsRequestedStatus);
                    VerifyTransferFunctionality(null, preAuthWithDocsRequestedStatus,
                        new List<string>() { statusList[0] },
                        false);
                }

                finally
                {
                    automatedBase.CurrentPage.NavigateToPreAuthSearch();
                    if (!_preAuthSearch.GetSideBarPanelSearch.IsSideBarPanelOpen())
                        _preAuthSearch.ClickOnSidebarIcon();
                    _preAuthSearch.GetSideBarPanelSearch.ClickOnClearLink();
                }

                void VerifyTransferFunctionality(List<string> preAuthList, string currentPreAuth,
                    List<string> statusList, bool multiplePreAuthsInSearchGrid = true)
                {
                    if (multiplePreAuthsInSearchGrid)
                    {
                        if (!_preAuthAction.IsTransferIconPresent())
                            _preAuthAction.ClickOnTransferIcon();

                        List<string> newStatusList;
                        string newCurrentStatus;

                        foreach (var status in statusList)
                        {
                            _preAuthAction.ClickOnTransferIcon();
                            _preAuthAction.SelectStatusDropDownValue(status);
                            _preAuthAction.SetNoteInTransferPreAuth($"Test Note for {status}");
                            _preAuthAction.ClickSaveButton();
                            _preAuthAction.WaitForWorking();
                            _preAuthAction.CloseAnyTabIfExist();

                            _preAuthAction.GetUpperLeftQuadrantValueByLabel("Auth Seq")
                                .ShouldBeEqual(preAuthList[preAuthList.IndexOf(currentPreAuth) + 1],
                                    "User should be directed to the next pre-auth in the list");

                            _preAuthSearch = _preAuthAction.NavigateToPreAuthSearch();

                            StringFormatter.PrintMessage(
                                "Verifying whether the recently changed status can be searched from the Pre-Auth Search page");
                            _preAuthSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Status", status);
                            _preAuthSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Auth Sequence", currentPreAuth);
                            _preAuthSearch.ClickOnFindButton();
                            _preAuthSearch.WaitForWorking();
                            _preAuthSearch.GetSideBarPanelSearch.IsNoMatchingRecordFoundMessagePresent()
                                .ShouldBeFalse(
                                    "User is able to find the pre-auth by filtering by the newly assigned status");
                            _preAuthSearch.GetGridViewSection.GetValueInGridByColRow(8)
                                .ShouldBeEqual(status,
                                    "Status should be updated to the newly set status in Pre-Auth Search grid");

                            _preAuthSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Status", "");
                            _preAuthSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Auth Sequence", "");
                            _preAuthSearch.ClickOnFindButton();
                            _preAuthSearch.WaitForWorking();

                            _preAuthAction = _preAuthSearch.ClickOnAuthSeqToNavigateToPreAuthActionPage(currentPreAuth);

                            newCurrentStatus = _preAuthAction.GetUpperLeftQuadrantValueByLabel("Status");
                            newCurrentStatus.ShouldBeEqual(status,
                                "The pre-auth should be assigned the new status after the transfer process");
                            _preAuthProcessingHistory = _preAuthAction.ClickOnPreAuthProcessingHistoryAndSwitch();

                            _preAuthProcessingHistory.GetGridValueByRowCol(1, 4).Contains($"Test Note for {status}")
                                .ShouldBeTrue(
                                    "Is the entered note being displayed correctly in Pre-Auth Processing History");

                            _preAuthProcessingHistory.CloseClaimProcessingHistoryPageAndSwitchToPreAuthActionPage();

                            _preAuthAction.ClickOnTransferIcon();
                            newStatusList = _preAuthAction.GetSideWindow.GetAvailableDropDownList("Status");
                            newStatusList.Remove(newCurrentStatus);
                            VerifyStatusDropdownInTransferForm(newCurrentStatus, newStatusList);
                        }
                    }

                    else
                    {
                        if (!_preAuthAction.IsTransferIconPresent())
                            _preAuthAction.ClickOnTransferIcon();

                        List<string> newStatusList;
                        string newCurrentStatus;

                        foreach (var status in statusList)
                        {
                            _preAuthAction.ClickOnTransferIcon();
                            _preAuthAction.SelectStatusDropDownValue(status);
                            _preAuthAction.SetNoteInTransferPreAuth($"Test Note for {status}");
                            _preAuthAction.ClickSaveButton();
                            _preAuthAction.WaitForWorking();
                            _preAuthAction.CloseAnyTabIfExist();

                            automatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(
                                PageHeaderEnum.PreAuthSearch.GetStringValue(),
                                    "User should be directed back to Pre-Auth Search page if there is just one search result");

                            StringFormatter.PrintMessage(
                                "Verifying whether the recently changed status can be searched from the Pre-Auth Search page");
                            _preAuthSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Status", status);
                            _preAuthSearch.ClickOnFindButton();
                            _preAuthSearch.WaitForWorking();
                            _preAuthSearch.GetSideBarPanelSearch.IsNoMatchingRecordFoundMessagePresent()
                                .ShouldBeFalse(
                                    "User is able to find the pre-auth by filtering by the newly assigned status");

                            _preAuthSearch.GetGridViewSection.GetValueInGridByColRow(8)
                                .ShouldBeEqual(status,
                                    "Status should be updated to the newly set status in Pre-Auth Search grid");
                            _preAuthAction = _preAuthSearch.ClickOnAuthSeqToNavigateToPreAuthActionPage(currentPreAuth);

                            newCurrentStatus = _preAuthAction.GetUpperLeftQuadrantValueByLabel("Status");
                            newCurrentStatus.ShouldBeEqual(status,
                                "The pre-auth should be assigned the new status after the transfer process");
                            _preAuthProcessingHistory = _preAuthAction.ClickOnPreAuthProcessingHistoryAndSwitch();

                            _preAuthProcessingHistory.GetGridValueByRowCol(1, 4).Contains($"Test Note for {status}")
                                .ShouldBeTrue(
                                    "Is the entered note being displayed correctly in Pre-Auth Processing History");

                            _preAuthProcessingHistory.CloseClaimProcessingHistoryPageAndSwitchToPreAuthActionPage();

                            _preAuthAction.ClickOnTransferIcon();
                            newStatusList = _preAuthAction.GetSideWindow.GetAvailableDropDownList("Status");
                            newStatusList.Remove(newCurrentStatus);
                            VerifyStatusDropdownInTransferForm(newCurrentStatus, newStatusList);
                        }
                    }
                }

                void VerifyStatusDropdownInTransferForm(object currentStatus, List<string> currentStatusList)
                {
                    #region Status List Declaration And Initialization

                    Dictionary<string, string> cotivitiUnreviewedList = new Dictionary<string, string>
                    {
                        [StatusEnum.CotivitiConsultantRequired.GetStringDisplayValue()] =
                            "Requires Cotiviti consultant review",
                        [StatusEnum.StateConsultantRequired.GetStringDisplayValue()] =
                            "Requires Cotiviti state consultant review",
                        [StatusEnum.DocumentsRequired.GetStringDisplayValue()] =
                            "Documents are required from the client/provider"
                    };

                    Dictionary<string, string> cotivitiConsultantRequiredList = new Dictionary<string, string>
                    {
                        [StatusEnum.CotivitiConsultantComplete.GetStringDisplayValue()] =
                            "Consultant review is complete",
                        [StatusEnum.DocumentsRequired.GetStringDisplayValue()] =
                            "Documents are required from the client/provider"
                    };

                    Dictionary<string, string> stateConsultantRequiredList = new Dictionary<string, string>
                    {
                        [StatusEnum.StateConsultantComplete.GetStringDisplayValue()] = "Consultant review is complete",
                        [StatusEnum.DocumentsRequired.GetStringDisplayValue()] =
                            "Documents are required from the client/provider"
                    };

                    Dictionary<string, string> otherStatusList = new Dictionary<string, string>
                    {
                        [StatusEnum.CotivitiConsultantRequired.GetStringDisplayValue()] =
                            "Requires Cotiviti consultant review",
                        [StatusEnum.StateConsultantRequired.GetStringDisplayValue()] =
                            "Requires Cotiviti state consultant review",
                        [StatusEnum.DocumentsRequired.GetStringDisplayValue()] =
                            "Documents are required from the client/provider",
                        [StatusEnum.CotivitiUnreviewed.GetStringDisplayValue()] =
                            "Remove pend status and complete review"
                    };

                    #endregion

                    List<string> tooltipList;

                    switch (currentStatus)
                    {
                        case String str when str == StatusEnum.CotivitiUnreviewed.GetStringDisplayValue():
                            currentStatusList.ShouldCollectionBeEqual(
                                cotivitiUnreviewedList.Select(s => s.Key).ToList(),
                                $"Is the status list correct when current status is : {currentStatus} ?");
                            tooltipList = _preAuthAction.GetStatusToolTipInPreAuthTransferForm();
                            tooltipList.RemoveAt(0);
                            tooltipList.ShouldCollectionBeEqual(cotivitiUnreviewedList.Select(s => s.Value).ToList(),
                                "Is Tooltip message correct for all status ? ");
                            break;

                        case String str when str == StatusEnum.CotivitiConsultantRequired.GetStringDisplayValue():
                            currentStatusList.ShouldCollectionBeEqual(
                                cotivitiConsultantRequiredList.Select(s => s.Key).ToList(),
                                $"Is the status list correct when current status is : {currentStatus} ?");
                            tooltipList = _preAuthAction.GetStatusToolTipInPreAuthTransferForm();
                            tooltipList.RemoveAt(0);
                            tooltipList.ShouldCollectionBeEqual(
                                cotivitiConsultantRequiredList.Select(s => s.Value).ToList(),
                                "Is Tooltip message correct for all status ? ");
                            break;

                        case String str when str == StatusEnum.StateConsultantRequired.GetStringDisplayValue():
                            currentStatusList.ShouldCollectionBeEqual(
                                stateConsultantRequiredList.Select(s => s.Key).ToList(),
                                $"Is the status list correct when current status is : {currentStatus} ?");
                            tooltipList = _preAuthAction.GetStatusToolTipInPreAuthTransferForm();
                            tooltipList.RemoveAt(0);
                            tooltipList.ShouldCollectionBeEqual(
                                stateConsultantRequiredList.Select(s => s.Value).ToList(),
                                "Is Tooltip message correct for all status ? ");
                            break;

                        case String str when str == StatusEnum.CotivitiConsultantRequired.GetStringDisplayValue():
                            currentStatusList.ShouldCollectionBeEqual(
                                cotivitiConsultantRequiredList.Select(s => s.Key).ToList(),
                                $"Is the status list correct when current status is : {currentStatus} ?");
                            tooltipList = _preAuthAction.GetStatusToolTipInPreAuthTransferForm();
                            tooltipList.RemoveAt(0);
                            tooltipList.ShouldCollectionBeEqual(
                                cotivitiConsultantRequiredList.Select(s => s.Value).ToList(),
                                "Is Tooltip message correct for all status ? ");
                            break;

                        case String str when str == StatusEnum.DocumentsRequired.GetStringDisplayValue() ||
                                    str == StatusEnum.DocumentReview.GetStringDisplayValue() ||
                                    str == StatusEnum.DocumentsRequested.GetStringDisplayValue() ||
                                    str == StatusEnum.StateConsultantComplete.GetStringDisplayValue() ||
                                    str == StatusEnum.CotivitiConsultantComplete.GetStringDisplayValue():

                            currentStatusList.ShouldCollectionBeEqual(otherStatusList.Select(s => s.Key).ToList(),
                                $"Is the status list correct when current status is : {currentStatus} ?");
                            tooltipList = _preAuthAction.GetStatusToolTipInPreAuthTransferForm();
                            tooltipList.RemoveAt(0);
                            tooltipList.ShouldCollectionBeEqual(otherStatusList.Select(s => s.Value).ToList(),
                                "Is Tooltip message correct for all status ? ");
                            break;

                        default:
                            throw new AssertionException($" Current Status {currentStatus} is invalid ");
                    }
                }
            }
        }

        [Test] //CAR-1097 + CAR-3214 [CAR-3237]
        public void Verify_view_patient_claim_hx_in_pre_auth_action()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                PreAuthSearchPage _preAuthSearch;
                PreAuthActionPage _preAuthAction;
                _preAuthSearch = automatedBase.QuickLaunch.NavigateToPreAuthSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var authSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName,
                    "AuthSequence", "Value");
                var preAuthHistoryLabel = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName,
                    "HistoryLabel", "Value").Split(';').ToList();
                var proc = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                    "procCode",
                    "Value");
                var flag = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                    "flag",
                    "Value");
                try
                {
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

                    expectedDetailList.Select(s => s.Replace(" ", ""))
                        .ShouldCollectionBeEqual(actualDetailList.Select(s => s.Replace(" ", "")),
                            "Verification of Row details");

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

                    //CAR-3214 [CAR-3237]
                    StringFormatter.PrintMessageTitle("Verifying toggling between Pre-Auth tab and other tabs");
                    ToggleBetweenPreAuthIconAndOtherIcons();

                    void ToggleBetweenPreAuthIconAndOtherIcons()
                    {
                        var rnd = new Random();

                        int totalToggles = rnd.Next(4, 8);
                        var iconsToClick = new Dictionary<string, string>()
                        {
                            ["Provider History"] = "Patient Claim History",
                            ["Patient Dx"] = "Patient Diagnosis History",
                            ["Dental"] = "Patient Tooth History"
                        };

                        var preAuthPageHeader = "Patient Pre-Auth History";
                        claimPatientHistory.ClickOnClaimHistoryIconByName("Pre-Auth", preAuthPageHeader);

                        StringFormatter.PrintMessageTitle($"Toggling between Pre-Auth tab and other tabs for {totalToggles} times");
                        for (int count = 1; count <= totalToggles; count++)
                        {
                            var randomIconName = iconsToClick.ElementAt(rnd.Next(0, 3)).Key;
                            var pageHeader = iconsToClick[randomIconName];

                            claimPatientHistory.ClickOnClaimHistoryIconByName(randomIconName, pageHeader);
                            claimPatientHistory.GetPopupPageTitle().ShouldBeEqual(pageHeader,
                                $"User should be correctly navigated to the {pageHeader} view when the {randomIconName} icon is clicked");

                            claimPatientHistory.ClickOnClaimHistoryIconByName("Pre-Auth", preAuthPageHeader);
                            claimPatientHistory.GetPopupPageTitle().ShouldBeEqual(preAuthPageHeader,
                                $"User should be correctly navigated to the {pageHeader} view when the {randomIconName} icon is clicked");
                        }
                    }
                }
                finally
                {
                    _preAuthSearch.CloseAnyTabIfExist();
                }
            }
        }

        [Test]//CAR-1794(CAR-1661)
        public void Verify_access_Patient_Claim_History_from_PreAuth_Action()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                PreAuthSearchPage _preAuthSearch;
                PreAuthActionPage _preAuthAction;
                UserProfileSearchPage _newUserProfileSearch;
                ClaimHistoryPage _claimHistory;
                _preAuthSearch = automatedBase.QuickLaunch.NavigateToPreAuthSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);

                var preAuthID = paramLists["PreAuthID"];
                var preAuthSeq = paramLists["PreAuthSeq"].Split(';').ToList();
                var patprovTitle = new List<string>(2) { "Patient Seq", "Provider Seq" };
                var patientHeader = new List<string>(2) { "PatientSequence", "ProviderSequence" };

                _preAuthSearch.UpdateStatusOfAuthSeqFromDatabase("U", preAuthSeq[0]);
                _preAuthSearch.UpdateHciDoneOrClientDoneToFalseFromDatabase("hcidone", preAuthSeq[0]);
                try
                {

                    StringFormatter.PrintMessageTitle("When 'Auto Display Patient Claim History' option is selected");

                    _newUserProfileSearch = _preAuthSearch.NavigateToNewUserProfileSearch();
                    _newUserProfileSearch.SearchUserByNameOrId(
                        new List<string> {automatedBase.EnvironmentManager.Username}, true);
                    _newUserProfileSearch.ClickOnGridRowByUserIdToOpenUserSettingSideView(automatedBase
                        .EnvironmentManager.Username);

                    _newUserProfileSearch.ClickOnEditIcon();

                    _newUserProfileSearch
                        .IsRadioButtonOnOffByLabel(UserPreferencesEnum.AutoDisplayPtClHx.GetStringValue(),
                            true)
                        .ShouldBeTrue($"{UserPreferencesEnum.AutoDisplayPtClHx.GetStringValue()}");


                    _newUserProfileSearch.SearchUserByNameOrId(
                        new List<string>
                            {automatedBase.EnvironmentManager.HCIUserWithOnlyDciWorklistAuthorityAndNoOtherAuthority},
                        true);
                    _newUserProfileSearch.ClickOnGridRowByUserIdToOpenUserSettingSideView(automatedBase
                        .EnvironmentManager
                        .HCIUserWithOnlyDciWorklistAuthorityAndNoOtherAuthority);

                    _newUserProfileSearch.ClickOnEditIcon();

                    _newUserProfileSearch
                        .IsRadioButtonOnOffByLabel(UserPreferencesEnum.AutoDisplayPtClHx.GetStringValue(),
                            false)
                        .ShouldBeTrue($"{UserPreferencesEnum.AutoDisplayPtClHx.GetStringValue()}");

                    _preAuthSearch = _newUserProfileSearch.NavigateToPreAuthSearch();
                    _preAuthSearch.GetSideBarPanelSearch.SelectDropDownListByIndex("Quick Search", 1);
                    _preAuthSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Pre-Auth ID", preAuthID);
                    _preAuthSearch.GetSideBarPanelSearch.ClickOnFindButton();
                    _preAuthSearch.WaitForWorkingAjaxMessage();
                    _preAuthAction = _preAuthSearch.ClickOnGridByRowColToPreAuthActionPage(2, 2, false);
                    _preAuthAction.GetWindowHandlesCount()
                        .ShouldBeEqual(2, "Patient Claim History Popup should display");
                    var patprovseq = _preAuthAction.GetPatientProviderDetails(patprovTitle);
                    _claimHistory = _preAuthAction.SwitchToPatientClaimHistory(true);
                    _claimHistory.IsProviderHistoryTabSelected()
                        .ShouldBeTrue("ProviderHistory tab should be selected by default.");
                    var claimhistorypatprovseq = _claimHistory.GetPatientHistoryHeaderDetails(patientHeader);

                    StringFormatter.PrintMessageTitle("Message displayed when there are no claims");
                    _claimHistory.GetEmptyMessageText().ShouldBeEqual("No claims found.");

                    _preAuthAction = _claimHistory.SwitchBackToPreAuthActionPage();

                    patprovseq.ShouldCollectionBeEqual(claimhistorypatprovseq,
                        "Patient and Provider Sequence of Pre-Auth Action page and Patient Claim History should match");

                    StringFormatter.PrintMessageTitle(
                        "Existing patient claim history pop up is replaced by new patient claim history");
                    _preAuthAction.ClickOnApproveIcon();
                    _preAuthAction.WaitForStaticTime(3000);
                    var patprovseq1 = _preAuthAction.GetPatientProviderDetails(patprovTitle);
                    _claimHistory = _preAuthAction.SwitchToPatientClaimHistory(true);
                    _claimHistory.IsProviderHistorySelected()
                        .ShouldBeTrue("Provider History should be selected by default");
                    var claimhistorypatprovseq1 = _claimHistory.GetPatientHistoryHeaderDetails(patientHeader);
                    patprovseq1.ShouldCollectionBeEqual(claimhistorypatprovseq1,
                        "Patient and Provider Sequence of Pre-Auth Action page and Patient Claim History should match");
                    _preAuthAction = _claimHistory.SwitchBackToPreAuthActionPage();
                    _preAuthAction.CloseLastWindow();


                    _preAuthSearch.Logout().LoginAsHCIUserwithONLYDCIWorklistauthority();

                    _preAuthSearch = automatedBase.CurrentPage.NavigateToPreAuthSearch();
                    _preAuthSearch.GetSideBarPanelSearch.SelectDropDownListByIndex("Quick Search", 1);
                    _preAuthSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Auth Sequence", preAuthSeq[1]);
                    _preAuthSearch.GetSideBarPanelSearch.ClickOnFindButton();
                    _preAuthSearch.WaitForWorkingAjaxMessage();
                    _preAuthAction = _preAuthSearch.ClickOnGridByRowColToPreAuthActionPage(1, 2, false);
                    _preAuthAction.GetWindowHandlesCount()
                        .ShouldBeEqual(1, "Auto Patient Claim History Popup should not display");
                    _preAuthAction.GetHistoryIconToolTip().ShouldBeEqual("Patient Claim History");
                    var patprovseq2 = _preAuthAction.GetPatientProviderDetails(patprovTitle);
                    _preAuthAction.ClickOnHistoryIconToNavigateToPatientClaimHistoryPage();
                    _preAuthAction.GetWindowHandlesCount()
                        .ShouldBeEqual(2, "Patient Claim History Popup should display");
                    _claimHistory = _preAuthAction.SwitchToPatientClaimHistory();
                    var claimhistorypatprovseq2 = _claimHistory.GetPatientHistoryHeaderDetails(patientHeader);
                    _claimHistory.IsDataAvailableinProviderHistoryTab()
                        .ShouldBeTrue("Claims should be available in Provider History tab");
                    patprovseq2.ShouldCollectionBeEqual(claimhistorypatprovseq2, "List should match");
                    _preAuthAction = _claimHistory.SwitchBackToPreAuthActionPage();

                }
                finally
                {
                    _preAuthSearch.CloseLastWindow();
                    _preAuthSearch.UpdateStatusOfAuthSeqFromDatabase("U", preAuthSeq[0]);
                    _preAuthSearch.UpdateHciDoneOrClientDoneToFalseFromDatabase("hcidone", preAuthSeq[0]);
                }
            }
        }


        [Test] //TE - 811 + CAR-3133 [CAR-3262]
        public void Verify_upload_new_document_in_pre_auth_action_for_Internal_User()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                try
                {
                    PreAuthSearchPage _preAuthSearch;
                    PreAuthActionPage _preAuthAction;
                    _preAuthSearch = automatedBase.QuickLaunch.NavigateToPreAuthSearch();
                    TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                    IDictionary<string, string> testData =
                        automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                    var preAuthSequence = testData["PreAuthSequence"];
                    var auditDate = testData["AuditDate"];
                    var fileToUpload = testData["FileToUpload"];
                    var fileType = testData["FileType"];
                    var expectedSelectedFileTypeList = testData["SelectedFileList"].Split(',').ToList();
                    var expectedSelectedFileType = testData["SelectedFileList"];

                    List<string> expectedFileTypeList = new List<string>();
                    foreach (DentalAppealDocTypeEnum fileTypeEnum in Enum.GetValues(typeof(DentalAppealDocTypeEnum)))
                    {
                        var preAuthFileType = fileTypeEnum.GetStringValue();
                        expectedFileTypeList.Add(preAuthFileType);
                    }

                    var expectedDocumentHeaderText = testData["DocumentHeaderText"];
                    var currentUser = _preAuthSearch.GetLoggedInUserFullName();

                    _preAuthSearch.DeletePreAuthDocumentRecord(preAuthSequence, auditDate);
                    _preAuthAction = _preAuthSearch.SearchByAuthSequenceAndNavigateToAuthAction(preAuthSequence, false);
                    var status = _preAuthAction.GetPreAuthStatus();

                    _preAuthAction.GetTitleOfUpperRightQuadrant()
                        .ShouldBeEqual("Documents", "Upper Quadrant Title Should Be Documents");

                    _preAuthAction.GetDocumentUploadFormTitle()
                        .ShouldBeEqual(expectedDocumentHeaderText, "Form Header Should Match");
                    _preAuthAction.IsUploadNewDocumentFormPresent()
                        .ShouldBeTrue("Upload Document Form Should Be Open By Default");
                    _preAuthAction.IsAddIconDisabledInDocumentUploadForm().ShouldBeTrue("Add Icon Should Be Disabled");


                    StringFormatter.PrintMessage("Verify cancel link behaviour");
                    _preAuthAction.ClickOnCancelLinkOnUploadDocumentForm();
                    _preAuthAction.IsUploadNewDocumentFormPresent().ShouldBeFalse("Upload Document Form Should Be Closed");
                    _preAuthAction.IsAddIconDisabledInDocumentUploadForm().ShouldBeFalse("Add Icon Should Be Enabled");


                    StringFormatter.PrintMessage("Verify Validation Message fro upload document");
                    _preAuthAction.ClickOnAddIconDocumentUploadForm();

                    StringFormatter.PrintMessage("Verify Maximum character in description");
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
                    _preAuthAction.GetFileUploadPage.ClickOnAddFileBtn();
                    _preAuthAction.IsPageErrorPopupModalPresent()
                        .ShouldBeFalse(
                            "Page error pop up should not be present if no description is set as its not a required field");

                    StringFormatter.PrintMessage("Verify details of added file correct");
                    _preAuthAction.GetFileUploadPage.ClaimFileToUploadDocumentValue(1, 2)
                        .ShouldBeEqual(fileToUpload.Split(';')[0],
                            "Documen Nmae  correct and present under files to upload");
                    _preAuthAction.GetFileUploadPage.ClaimFileToUploadDocumentValue(1, 3)
                        .ShouldBeEqual("Test Description",
                            "Document Description is correct and present under files to upload.");
                    _preAuthAction.GetFileUploadPage.ClaimFileToUploadDocumentValue(1, 4)
                        .ShouldBeEqual("Multiple File Types", "Document Type text when multiple File Types is selected.");
                    _preAuthAction.GetFileUploadPage.GetFileToUploadTooltipValue(1, 4)
                        .ShouldBeEqual(expectedSelectedFileType,
                            "Verify tool tip should display name of all selected file types");

                    var filesToUploadCount = _preAuthAction.GetFileUploadPage.GetFilesToUploadCount();

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

                    _preAuthAction.GetFileUploadPage.GetFilesToUploadCount()
                        .ShouldBeEqual(filesToUploadCount, "count should be same");
                    _preAuthAction.GetFileUploadPage.ClickOnSaveUploadBtn();
                    _preAuthAction.GetFileUploadPage.IsDocumentDivPresent().ShouldBeTrue("Document List are present");
                    _preAuthAction.IsUploadNewDocumentFormPresent()
                        .ShouldBeFalse("Upload New document form should be closed after uploading document.");
                    _preAuthAction.IsPreAuthDocumentPresent(fileToUpload.Split(';')[0])
                        .ShouldBeTrue("Uploaded Document is listed and document Name correct");
                    var expectedDate = _preAuthAction.UploadedDocumentValueByRowColumn(1, 1, 4);

                    StringFormatter.PrintMessageTitle(
                        "Validation of Audit Record to display in pre auth Processing History after document upload.");
                    _preAuthAction.IsPreAuthAuditAddedForDocumentUpload(preAuthSequence)
                        .ShouldBeTrue("Claim Audit for Document Upload is added in database ");

                    PreAuthProcessingHistoryPage preAuthProcessingHistory =
                        _preAuthAction.ClickOnPreAuthProcessingHistoryAndSwitch();
                    var date = preAuthProcessingHistory.GetGridValueByRowCol(1, 1);
                    //date.IsDateTimeInFormat().ShouldBeTrue("Upload Date time is in correct format");
                    Convert.ToDateTime(date).Date
                        .ShouldBeEqual(Convert.ToDateTime(expectedDate).Date, "Correct Audit Date is displayed.");
                    preAuthProcessingHistory.GetGridValueByRowCol(1, 3)
                        .ShouldBeEqual(status, " Correct Status is displayed.");



                    preAuthProcessingHistory.GetGridValueByRowCol(1, 2).ShouldBeEqual(currentUser);
                    currentUser.DoesNameContainsOnlyFirstWithLastname()
                        .ShouldBeTrue("Username Should be in <FirstName> <LastName>");
                    preAuthProcessingHistory.GetGridValueByRowCol(1, 4)
                        .ShouldBeEqual("Document Uploaded", "Correct Note is displayed.");
                    _preAuthAction.CloseAnyPopupIfExist();

                    StringFormatter.PrintMessage("verify delete of added files");
                    var existingDocCount = _preAuthAction.GetFileUploadPage.DocumentCountOfFileList();
                    _preAuthAction.GetFileUploadPage.ClickOnClaimDocumentUploadIcon();
                    _preAuthAction.GetFileUploadPage.IsDocumentUploadSectionPresent()
                        .ShouldBeTrue("Document Uploader Section is displayed");
                    _preAuthAction.GetFileUploadPage.SetFileTypeListVlaue(fileType);
                    _preAuthAction.GetFileUploadPage.AddFileForUpload(fileToUpload.Split(';')[0]);
                    _preAuthAction.GetFileUploadPage.ClickOnAddFileBtn();
                    filesToUploadCount = _preAuthAction.GetFileUploadPage.GetFilesToUploadCount();
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
                            "Added Files has been discarded ");
                }
                catch (Exception e)
                {
                    
                }
            }
        }

        [Test, Category("Working")] //TE-811
        public void Verify_Multiple_Document_Can_Be_Uploaded_In_Pre_Auth_Action_for_Internal_User()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                PreAuthSearchPage _preAuthSearch;
                PreAuthActionPage _preAuthAction;
                _preAuthSearch = automatedBase.QuickLaunch.NavigateToPreAuthSearch();

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
                UploadPreAuthDocument(fileType[0], description[0], fileToUpload[0]);
                var filesToUploadCount = _preAuthAction.GetFileUploadPage.GetFilesToUploadCount();
                UploadPreAuthDocument(fileType[0], description[1], fileToUpload[1]);
                _preAuthAction.WaitForStaticTime(4000);
                _preAuthAction.GetFileUploadPage.GetFilesToUploadCount()
                    .ShouldBeEqual(filesToUploadCount + 1, "Multiple Files Should Be Added Prior To Save");
                _preAuthAction.GetFileUploadPage.ClickOnSaveUploadBtn();
                StringFormatter.PrintMessage("Verify Database Values For Uploaded Document");
                var docUploadDataFromDb = _preAuthAction.GetDocumentUploadInformationFromDb(preAuthSequence);
                _preAuthAction.GetFileUploadPage.DocumentCountOfFileList()
                    .ShouldBeEqual(docUploadDataFromDb.Count, " Uploaded Document Count Should Match");
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

                void UploadPreAuthDocument(string fType, string desc, string fileName)
                {
                    if (!_preAuthAction.IsUploadNewDocumentFormPresent())
                    {
                        _preAuthAction.ClickOnAddIconDocumentUploadForm();
                    }

                    _preAuthAction.GetFileUploadPage.SetFileTypeListVlaue(fType);
                    _preAuthAction.GetFileUploadPage.SetFileUploaderFieldValue("Description", desc);
                    _preAuthAction.GetFileUploadPage.AddFileForUpload(fType);
                    _preAuthAction.WaitForStaticTime(5000);
                    _preAuthAction.GetFileUploadPage.ClickOnAddFileBtn();
                    _preAuthAction.WaitForStaticTime(5000);
                    _preAuthAction.GetFileUploadPage.WaitForWorkingAjaxMessage();
                }
            }
        }

        [Test] //TE-811
        public void Verify_view_delete_of_uploaded_documents_in_Pre_Auth_action_page_for_Internal_User()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                PreAuthSearchPage _preAuthSearch;
                PreAuthActionPage _preAuthAction;
                _preAuthSearch = automatedBase.QuickLaunch.NavigateToPreAuthSearch();
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var preAuthSequence = paramLists["PreAuthSequence"];
                var fileToUpload = paramLists["FileToUpload"].Split(';');
                
                var docDescription = paramLists["DocDescription"];
                var currentUser = _preAuthSearch.GetLoggedInUserFullName();
                var userType = paramLists["UserType"];
                var auditDate = paramLists["AuditDate"];

                List<string> expectedSelectedFileTypeList = new List<string>();
                foreach (DentalAppealDocTypeEnum fileTypeEnum in Enum.GetValues(typeof(DentalAppealDocTypeEnum)))
                {
                    var preAuthFileType = fileTypeEnum.GetStringValue();
                    expectedSelectedFileTypeList.Add(preAuthFileType);
                }

                expectedSelectedFileTypeList.Sort();

                _preAuthSearch.DeletePreAuthDocumentRecord(preAuthSequence, auditDate);
                _preAuthAction = _preAuthSearch.SearchByAuthSequenceAndNavigateToAuthAction(preAuthSequence, false);
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
                StringFormatter.PrintMessage("Verify the Status of the claim after the document is uploaded");
                _preAuthAction.GetPreAuthStatus()
                    .ShouldBeEqual(preAuthStatus, "Verified Claim Status after Document Upload");
                _preAuthAction.IsPreAuthDocumentDeleteIconPresent()
                    .ShouldBeTrue("Pre Auth Document Delete Icon should present");
                _preAuthAction.GetFileUploadPage.AppealDocumentsListAttributeValue(1, 1, 2)
                    .ShouldBeEqual(fileToUpload[0], "Document filename is displayed");
                _preAuthAction.GetFileUploadPage.AppealDocumentsListAttributeValue(1, 1, 4)
                    .IsDateTimeInFormat()
                    .ShouldBeTrue("Document date is displayed and in proper format");
                _preAuthAction.GetFileUploadPage.AppealDocumentsListAttributeValue(1, 2, 2)
                    .ShouldBeEqual(docDescription, "Document Description is displayed");
                _preAuthAction.GetFileUploadPage.GetFileTypeAttributeListVaues(1, 1, 3)
                    .ShouldCollectionBeEqual(expectedSelectedFileTypeList, "Document fileType is displayed");
                _preAuthAction.GetFileUploadPage.GetFileTypeAttributeListToolTip(1, 1, 3)
                    .ShouldCollectionBeEqual(expectedSelectedFileTypeList,
                        "Document fileType tooltip is displayed");
                _preAuthAction.GetFileUploadPage.IsEllipsisPresentInFileType(1)
                    .ShouldBeTrue("Is Ellipsis Present for lengthy values");

                StringFormatter.PrintMessage("Verify notes document opens in a new tab on clicking the file name and audit is recorded");
                _preAuthAction.ClickOnDocumentToViewAndStayOnPage(2); //window opens to view appeal document 
                _preAuthAction.GetOpenedDocumentText().ShouldBeEqual("test test", "document detail");
                _preAuthAction.CloseDocumentTabPageAndBackToPreAuthAction();
                _preAuthAction.IsAuditAddedForDocumentDownload(preAuthSequence)
                    .ShouldBeTrue("Audit for Document Download is added in database ");

                StringFormatter.PrintMessage("Verify document delete");
                _preAuthAction.GetFileUploadPage.ClickOnDeleteFileBtn(fileToUpload[0]);

                if (_preAuthAction.IsPageErrorPopupModalPresent())
                    _preAuthAction.GetPageErrorMessage()
                        .ShouldBeEqual("The selected document will be deleted. Do you wish to proceed?");

                _preAuthAction.ClickOkCancelOnConfirmationModal(false);

                _preAuthAction.GetFileUploadPage.DocumentCountOfFileList()
                    .ShouldBeEqual(existingDocCount, "All the documents are still intact and listed");
                _preAuthAction.GetFileUploadPage.ClickOnDeleteFileBtn(fileToUpload[0]);
                _preAuthAction.ClickOkCancelOnConfirmationModal(true);
                _preAuthAction.GetFileUploadPage.DocumentCountOfFileList()
                    .ShouldBeEqual(existingDocCount - 1, "Deleted document hasnt been listed.");
                _preAuthAction.GetFileUploadPage.IsFollowingAppealDocumentPresent(fileToUpload[0])
                    .ShouldBeFalse("Deleted Document isn't present");
                _preAuthAction.IsDocumentDeletedInDB(preAuthSequence).ShouldBeTrue("Deleted column set to True?");

                StringFormatter.PrintMessage("Verify the Status of the pre auth after the document is deleted");
                _preAuthAction.GetPreAuthStatus().ShouldBeEqual
                    (preAuthStatus, "Verified Claim Status Before Document Deletion");

                StringFormatter.PrintMessageTitle("Validation of Audit Record to display in claim Processing History after document upload.");
                _preAuthAction.IsAuditAddedForDocumentDelete(preAuthSequence)
                    .ShouldBeTrue("Audit for Document Delete is added in database ");
                PreAuthProcessingHistoryPage preAuthProcessingHistory =
                    _preAuthAction.ClickOnPreAuthProcessingHistoryAndSwitch();
                var date = preAuthProcessingHistory.GetGridValueByRowCol();
                Convert.ToDateTime(date).Date
                    .ShouldBeEqual(_preAuthAction.CurrentDateTimeInMst(DateTime.UtcNow).Date,
                        "Correct Audit Date is displayed.");
                preAuthProcessingHistory.GetGridValueByRowCol(1, 3)
                    .ShouldBeEqual(preAuthStatus, " Correct Status is displayed.");
                preAuthProcessingHistory.GetGridValueByRowCol(1, 2)
                    .ShouldBeEqual(currentUser, "recorded user name correct?");
                currentUser.DoesNameContainsOnlyFirstWithLastname()
                    .ShouldBeTrue("Username Should be in <FirstName> <LastName> (userid)");
                preAuthProcessingHistory.GetGridValueByRowCol(1, 4)
                    .ShouldBeEqual("Document Deleted", "Correct Note is displayed.");
                _preAuthAction.CloseAnyPopupIfExist();
            }
        }

        [Test] //TE-812
        public void Verify_hyperlink_from_TN_to_Tooth_history()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                PreAuthSearchPage _preAuthSearch;
                PreAuthActionPage _preAuthAction;
                ClaimHistoryPage _claimHistory;

                _preAuthSearch = automatedBase.QuickLaunch.NavigateToPreAuthSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                _preAuthAction =
                    _preAuthSearch.SearchByAuthSequenceAndNavigateToAuthAction(paramLists["PreAuthSequence"], false);
                var toothRecordsAssociatedWithTNValueFromDb =
                    _preAuthAction.GetToothRecordsForTNValue(paramLists["PatSequence"], paramLists["ToothNo"]);
                _preAuthAction.IsTnHyperlinkPresent().ShouldBeTrue("Is TN hyperlink");
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
        public void
            Verify_red_badge_over_the_preauth_icon_represents_the_number_of_preauth_records_existing_for_patient()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                PreAuthSearchPage _preAuthSearch;
                PreAuthActionPage _preAuthAction;
                _preAuthSearch = automatedBase.QuickLaunch.NavigateToPreAuthSearch();

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
                    claimPatientHistory.IsPreAuthIconPresentInHeaderLevel().ShouldBeTrue("Is Pre Auth Icon Present ?");
                    claimPatientHistory.ClickOnPreAuthIconAndNavigateToPatientPreAuthHistory();


                    StringFormatter.PrintMessage("Verify Red Badge Is Shown If Pre-Auth Record Is Present");
                    claimPatientHistory.IsRedBadgePresentOverPreAuthIcon()
                        .ShouldBeTrue("Is Red Badge Present Over Pre Auth Icon ?");
                    claimPatientHistory.GetPreAuthCountOnRedBadge()
                        .ShouldBeEqual(preAuthCount, "Pre Auth Count Should Match");

                    StringFormatter.PrintMessage("Verify Pre Auth Icon Is Not Present For DCA Inactive client");
                    _preAuthSearch.UpdateProductStatusForClient(ProductEnum.DCA.ToString(), ClientEnum.SMTST.ToString(),
                        false);
                    claimPatientHistory.SwitchBackToPreAuthActionPage();
                    _preAuthAction.CloseAnyTabIfExist();
                    _preAuthAction.ClickOnPreAuthClaimProcessingHistoryAndSwitch();
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
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                PreAuthSearchPage _preAuthSearch;
                PreAuthActionPage _preAuthAction;
                _preAuthSearch = automatedBase.QuickLaunch.NavigateToPreAuthSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                const string notes = "Test Note for PreAuth";
                var authSeq = paramLists["PreAuthSeq"];
                var lengthyNote = new string('a', 4494);


                _preAuthSearch.DeletePreAuthNote(authSeq);
                _preAuthAction = _preAuthSearch.SearchByAuthSequenceAndNavigateToAuthAction(authSeq, false);
                var username = _preAuthAction.GetLoggedInUserFullName();


                StringFormatter.PrintMessageTitle("Verify Note Container Details");
                _preAuthAction.IsAddNoteIconPresent().ShouldBeTrue("note icon present?");
                _preAuthAction.ClickonAddNoteIcon();
                _preAuthAction.IsNoteContainerPresent().ShouldBeTrue("Notes container Displayed?");
                _preAuthAction.GetPreAuthNoteType().ShouldBeEqual("Pre-Auth", "Displayed note type correct?");
                _preAuthAction.GetPreAuthNoteUserName().ShouldBeEqual(username, "Displayed User Name Correct ?");
                _preAuthAction.IsVisibleToClientIconChecked()
                    .ShouldBeTrue("Visible To Client Should Be Checked By Default");


                StringFormatter.PrintMessageTitle("Verify Validation and Cancel link working?");
                _preAuthAction.ClickOnSaveButtonInNoteEditorByRow(1);
                _preAuthAction.IsPageErrorPopupModalPresent().ShouldBeTrue("validation for note message displayed?");
                _preAuthAction.GetPageErrorMessage()
                    .ShouldBeEqual("Invalid or missing data must be resolved before record can be saved.");
                _preAuthAction.ClosePageError();

                _preAuthAction.SetLengthyNoteToVisbleTextarea("note",lengthyNote, handlePopup: false);
                _preAuthAction.IsPageErrorPopupModalPresent().ShouldBeTrue($"{lengthyNote.Length - 1} values allowed");
                _preAuthAction.GetPageErrorMessage().ShouldBeEqual("Note value is too long.",
                    "Is validation message for lengthy note Equals?");
                _preAuthAction.ClosePageError();

                _preAuthAction.ClickOnAddNoteCancelLink();
                _preAuthAction.IsNoteContainerPresent()
                    .ShouldBeFalse("Notes container Displayed after clicking cancel?");
                _preAuthAction.ClickonAddNoteIcon();

                StringFormatter.PrintMessageTitle("Addition of New note with visible to client=false");
                AddNotes();
                _preAuthAction.ClickonAddNoteSaveButton();
                _preAuthAction.GetNoteListCount().ShouldBeEqual(1, "Any notes created?");
                _preAuthAction.IsRedBadgeNoteIndicatorPresent()
                    .ShouldBeTrue("Red badge icon present with the note count?");
                _preAuthAction.NoOfPreAauthNotes().ShouldBeEqual("1", "Number of notes correct?");
                VerifySavedPreAuthNotes(notes, username);

                StringFormatter.PrintMessage("Addition of New note with visible to client=true");
                _preAuthAction.clickPreauthNoteSectionAddNoteIcon();
                AddNotes(2, true);
                _preAuthAction.ClickonAddNoteSaveButton();

                StringFormatter.PrintMessage("Verify only visible to client notes displayed to client user");
                _preAuthSearch = _preAuthAction.Logout().LoginAsClientUser().NavigateToPreAuthSearch();
                _preAuthAction = _preAuthSearch.SearchByAuthSequenceAndNavigateToAuthAction(authSeq);
                _preAuthAction.ClickNotesIcon();
                _preAuthAction.GetNoteListCount().ShouldBeEqual(1, "Any notes created?");
                VerifySavedPreAuthNotes(notes, username, internalUser: false);

                void AddNotes(int row = 1, bool clientVisibility = false)
                {
                    _preAuthAction.InputNotes(notes);
                    // _preAuthAction.IsVisibleToClientIconChecked();
                    if (clientVisibility)
                        _preAuthAction.CheckVisibleToClientInNoteEditorByRow(row);


                }

                void VerifySavedPreAuthNotes(string note, string uname, int row = 1, bool visibleToClient = true,
                    bool internalUser = true)
                {
                    StringFormatter.PrintMessage("Verify created notes:");
                    _preAuthAction.GetGridNoteDetailByRowandCol(row, 2)
                        .ShouldBeEqual("Pre-Auth", "Correct claim type recorded?");
                    _preAuthAction.GetGridNoteDetailByRowandCol(row, 4).ShouldBeEqual(uname,
                        "Correct username recorded?");
                    _preAuthAction.GetGridNoteDetailByRowandCol(row, 5)
                        .ShouldBeEqual(_preAuthAction.CurrentDateInMstStandard(null), "Correct date recorded?");
                    if (internalUser)
                    {
                        if (visibleToClient)
                            _preAuthAction.IsVisibletoClientIconPresentByRow(row)
                                .ShouldBeTrue("Is Visible to client check mark present?");
                        else
                        {
                            _preAuthAction.IsVisibletoClientIconPresentByRow(row)
                                .ShouldBeFalse("Is Visible to client check mark present?");
                        }

                        _preAuthAction.ClickOnEditIconOnNotesByRow(row);
                        _preAuthAction.GetInputValueOfNotes().ShouldBeEqual(note, "Is Note Equal?");
                        _preAuthAction.ClickOnCancelButtonInNoteEditorByRow(row);
                    }

                }
            }
        }

        [Test] //TE-810
        public void Verify_Edit_Of_existing_PreAuth_Notes()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                PreAuthSearchPage _preAuthSearch;
                PreAuthActionPage _preAuthAction;
                _preAuthSearch = automatedBase.QuickLaunch.NavigateToPreAuthSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var editNotes = "Edit Test Note for PreAuth";
                var authSeq = paramLists["PreAuthSeq"];
                var nonCurrentUser = paramLists["UserName"];
                var currentUser = _preAuthSearch.GetLoggedInUserFullName();

                StringFormatter.PrintMessage("Verify edit of existing notes");
                _preAuthAction = _preAuthSearch.SearchByAuthSequenceAndNavigateToAuthAction(authSeq, false);
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
                _preAuthAction.IsPencilIconPresentByName(currentUser)
                    .ShouldBeTrue("Pencil Icon Should Be Present For The Notes Created By Currently Logged In User");
                _preAuthAction.ClickOnEditIconOnNotesByRow(2);
                _preAuthAction.IsNoteEditFormDisplayedByRow(2).ShouldBeTrue("Note edit form displayed?");
                var notes = _preAuthAction.GetInputValueOfNotes();
                if (notes == editNotes)
                    editNotes = "New Edit Test Note for PreAuth";
                AddNotes(editNotes);
                _preAuthAction.ClickOnCancelButtonInNoteEditorByRow(2);
                _preAuthAction.IsNoteEditFormDisplayedByRow(2).ShouldBeFalse("Note edit form displayed?");
                _preAuthAction.ClickOnEditIconOnNotesByRow(2);
                _preAuthAction.GetInputValueOfNotes().ShouldBeEqual(notes, "Verify saved Note displayed?");
                AddNotes(editNotes);
                _preAuthAction.ClickOnSaveButtonInNoteEditorByRow(2);
                VerifySavedPreAuthNotes(editNotes, _preAuthAction.GetLoggedInUserFullName(), row: 2);


                StringFormatter.PrintMessage("Verify caret sign and check notes is not editable");
                _preAuthAction.IsCarrotIconPresentByName(nonCurrentUser)
                    .ShouldBeTrue("Carrot Icon Should Be Present For Notes Created By Non Current User");
                _preAuthAction.ClickOnCollapseIconOnNotesByRow(1);
                _preAuthAction.IsNoteEditFormDisplayedByRow(1)
                    .ShouldBeTrue("Clicking on Expand icon again must close note form.");
                _preAuthAction.IsNoteFormEditableByName(nonCurrentUser)
                    .ShouldBeFalse("Notes form must not be editable");

                void AddNotes(string note, int row = 1, bool clientVisibility = false)
                {
                    _preAuthAction.InputNotes(note);
                    // _preAuthAction.IsVisibleToClientIconChecked();
                    if (clientVisibility)
                        _preAuthAction.CheckVisibleToClientInNoteEditorByRow(row);
                }

                void VerifySavedPreAuthNotes(string note, string username, int row = 1, bool visibleToClient = true,
                    bool internalUser = true)
                {
                    StringFormatter.PrintMessage("Verify created notes:");
                    _preAuthAction.GetGridNoteDetailByRowandCol(row, 2)
                        .ShouldBeEqual("Pre-Auth", "Correct claim type recorded?");
                    _preAuthAction.GetGridNoteDetailByRowandCol(row, 4).ShouldBeEqual(username,
                        "Correct username recorded?");
                    _preAuthAction.GetGridNoteDetailByRowandCol(row, 5)
                        .ShouldBeEqual(_preAuthAction.CurrentDateInMstStandard(null), "Correct date recorded?");
                    if (internalUser)
                    {
                        if (visibleToClient)
                            _preAuthAction.IsVisibletoClientIconPresentByRow(row)
                                .ShouldBeTrue("Is Visible to client check mark present?");
                        else
                        {
                            _preAuthAction.IsVisibletoClientIconPresentByRow(row)
                                .ShouldBeFalse("Is Visible to client check mark present?");
                        }

                        _preAuthAction.ClickOnEditIconOnNotesByRow(row);
                        _preAuthAction.GetInputValueOfNotes().ShouldBeEqual(note, "Is Note Equal?");
                        _preAuthAction.ClickOnCancelButtonInNoteEditorByRow(row);
                    }

                }
            }
        }


        [Test] //CAR-2957(CAR-2928)
        public void Verify_flag_description_in_flag_Description_pop_up_preAuth()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                PreAuthSearchPage _preAuthSearch;
                PreAuthActionPage _preAuthAction;
                _preAuthSearch = automatedBase.QuickLaunch.NavigateToPreAuthSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                string preAuthSeq = paramLists["PreAuthSequence"];
                string flagType = paramLists["FlagType"];
                string cotivitiAutoReviewValue = paramLists["CotivitiAutoReviewValue"];
                string clientAutoReviewValue = paramLists["ClientAutoReviewValue"];
                string flagDescriptionValue = paramLists["FlagDescriptionValue"];

                _preAuthSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                    PreAuthQuickSearchEnum.AllPreAuths.GetStringValue());
                _preAuthAction = _preAuthSearch.SearchByAuthSequenceAndNavigateToAuthAction(preAuthSeq);

                try
                {
                    var flag = _preAuthAction.GetFlagRowDetailValue(col: 3);
                    var flagShortDescription = _preAuthAction.GetEOBMessageFromDatabase(flag, "S");

                    var flagPopupPage =
                        _preAuthAction.ClickOnFlagandSwitch("Flag Information - " + flag);
                    flagPopupPage.GetPopupHeaderText().ShouldBeEqual($"Flag - {flag}", "Popup header should match");
                    flagPopupPage.GetTextValueinLiTag(1)
                        .ShouldBeEqual(flagShortDescription, "Short flag description should match");
                    flagPopupPage.GetTextValueWithInTag(tag: "").Replace("\r\n", " ")
                        .ShouldBeEqual(flagType, "Flag Type should match");
                    flagPopupPage.GetTextValueWithinSpanTag(4).ShouldBeEqual(cotivitiAutoReviewValue,
                        "Cotiviti Auto review value should match");
                    flagPopupPage.GetTextValueWithinSpanTag(5)
                        .ShouldBeEqual(clientAutoReviewValue, "Client Auto Review value should match");
                    flagPopupPage.GetTextValueWithInTag(6, ">span")
                        .ShouldBeEqual(flagDescriptionValue, "Flag description label should be present");
                    flagPopupPage.GetTextValueWithInTag(6, "").Replace($"{flagDescriptionValue}\r\n", "")
                        .ShouldBeEqual(flagShortDescription, "Flag description should match");
                }
                finally
                {
                    _preAuthAction.CloseAnyTabIfExist();
                    _preAuthAction.NavigateToPreAuthSearch();
                }
            }
        }

        [Test] //CAR-3147[CAR-3068] + TE-877
        public void Verify_Flag_Lines_Deleted_By_Internal_Users_Are_Hidden_To_Client_Users()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                PreAuthSearchPage _preAuthSearch;
                PreAuthActionPage _preAuthAction;
                _preAuthSearch = automatedBase.QuickLaunch.NavigateToPreAuthSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                string preAuthSeq = paramLists["PreAuthSequence"];
                string flag1 = paramLists["Flags"].Split(',')[0];
                string flagSequence = paramLists["FlagSequence"];
                var authSeqWithSystemDeletedFlag = paramLists["PreAuthSeqWithSystemDeletedFlag"];
                var systemDeletedFlag = paramLists["SystemDeletedFlag"];
                _preAuthSearch.DeleteFlagAndAuditByAuthSeqFlagsLinNo(preAuthSeq, "'"+flag1+"'", "1");
                _preAuthSearch.DeleteRestorePreAuthFlagByInternalUser(flagSequence, false);
                _preAuthSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                    PreAuthQuickSearchEnum.AllPreAuths.GetStringValue());
                _preAuthAction = _preAuthSearch.SearchByAuthSequenceAndNavigateToAuthAction(preAuthSeq);

                try
                {
                    if (_preAuthAction.IsLockIconPresent())
                    {
                        _preAuthSearch.DeleteLockByPreAuthSeq(preAuthSeq);
                        _preAuthAction.NavigateToPreAuthSearch()
                            .SearchByAuthSequenceAndNavigateToAuthAction(preAuthSeq);
                    }

                    _preAuthAction.ClickOnAddFlagIcon();
                    _preAuthAction.ClickOnLinesOnSelectLines();
                    _preAuthAction.GetSideWindow.SelectDropDownListValueByLabel("Flag", flag1);
                    _preAuthAction.GetSideWindow.FillIFrame("Note", "ADD FLAG");
                    _preAuthAction.ClickVisibleToClientIcon(true);
                    _preAuthAction.ClickSaveButton();
                    _preAuthAction.WaitForWorking();

                    StringFormatter.PrintMessage("Perform Note Action"); 
                    EditFlag("1","2",2,"","Note Action",true,true,false);
                    
                    StringFormatter.PrintMessage("Perform Delete Action");
                    EditFlag("1", "2", 3, "Delete", "Delete Action", true, true);
                    
                    StringFormatter.PrintMessage("Perform Restore Action");
                    EditFlag("1", "2", 3, "Restore", "Restore Action", true, true);
                    
                    StringFormatter.PrintMessage("Perform Delete Action"); 
                    EditFlag("1", "2", 3, "Delete", "Delete Action", true, true);

                    StringFormatter.PrintMessageTitle("Logging in as Client user and performing the validations");
                    _preAuthSearch = _preAuthAction.Logout().LoginAsClientUser()
                        .NavigateToPreAuthSearch();
                    _preAuthAction = _preAuthSearch.SearchByAuthSequenceAndNavigateToAuthAction(preAuthSeq);
                    _preAuthAction.ClickOnLineItemOrFlagAuditHistoryIconByTitle("Flag Audit History");

                    StringFormatter.PrintMessage(
                        "Verifying Deleted Flag Should Not Be Present In Pre Auth Action Page Client View");
                    _preAuthAction.IsDeletedFlagPresentByFlagName(flag1)
                        .ShouldBeFalse("Deleted Flag Should Not Be Present In Pre Auth Action Page Client View");

                    StringFormatter.PrintMessage(
                        "Verifying if all flags are deleted by an internal user, nothing will be shown in the flagged lines quadrant for the client view.");
                    _preAuthSearch.DeleteRestorePreAuthFlagByInternalUser(flagSequence);
                    _preAuthAction.RefreshPage(false);
                    _preAuthAction.IsNoDataAvailableMessagePresentInFlaggedLine()
                        .ShouldBeTrue(
                            "If all flags are deleted by an internal user, nothing will be shown in the flagged lines quadrant for the client view.");

                    StringFormatter.PrintMessage(
                        "Verify Flag Audit History Records where user type  is internal and action except ADD are hidden in client view");
                    _preAuthAction.IsFlagAuditPresentByInternalUserTypeAndAction("Add", flag1)
                        .ShouldBeTrue("Add Audit Should Be Present in client view");
                    _preAuthAction.IsFlagAuditPresentByInternalUserTypeAndAction("Delete", flag1)
                        .ShouldBeFalse("Delete Audit by internal user Should Not Be Present in client view");
                    _preAuthAction.IsFlagAuditPresentByInternalUserTypeAndAction("Restore", flag1)
                        .ShouldBeFalse("Restore Audit by internal user Should Not Be Present in client view");
                    _preAuthAction.IsFlagAuditPresentByInternalUserTypeAndAction("Note", flag1)
                        .ShouldBeFalse("Note Audit by internal user Should Be Not Present in client view");

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
                    StringFormatter.PrintMessageTitle("Executing the Finally block");
                    _preAuthSearch.DeleteFlagAndAuditByAuthSeqFlagsLinNo(preAuthSeq, "'" + flag1 + "'",
                        "1");
                    _preAuthSearch.DeleteRestorePreAuthFlagByInternalUser(flagSequence,false);
                }

                #region Local Method

                void EditFlag(string lineNo, string row, int reasoncode, string title, string note = "",
                    bool client = false, bool notes = false, bool istitle = true)
                {
                    _preAuthAction.ClickOnEditFlagIconByLineNoAndRow(lineNo, row);

                    if (istitle)
                        _preAuthAction.ClickOnDeleteOrRestoreIcon(title);

                    if (client)
                        _preAuthAction.ClickVisibleToClientIcon();

                    if (notes)
                        _preAuthAction.InputNotes(note);

                    _preAuthAction.SelectReasonCode(reasoncode);
                    _preAuthAction.ClickSaveButton();
                    _preAuthAction.WaitForWorking();
                }

                #endregion
            }
        }

        [Test] //CAR-3187(CAR-3181)
        public void Verify_Edit_TN_TS_OC()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                PreAuthSearchPage _preAuthSearch = automatedBase.QuickLaunch.NavigateToPreAuthSearch();
                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestName);
                string preAuthSeq = paramLists["PreAuthSequence"];
                var toothInfo = paramLists["ToothInfo"].Split(',').ToList();
                var invalidToothInfo = paramLists["InvalidToothInfo"].Split(',').ToList();
                var toothInfoWithError = paramLists["ToothInfoWithError"].Split(',').ToList();
                _preAuthSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search", "All Pre-Auths");
                PreAuthActionPage _preAuthAction = _preAuthSearch.SearchByAuthSequenceAndNavigateToAuthAction(preAuthSeq);
                _preAuthAction.DeleteToothInfoForPreAuth(preAuthSeq);

                try
                {
                    _preAuthAction.IsEditIconPresentInClaimLine().ShouldBeTrue("Edit icon present?");
                    _preAuthAction.ClickOnEditIconOnLine();
                    _preAuthAction.SetToothInformationInLineBynoandLabel("TN", toothInfo[0]);
                    _preAuthAction.SetToothInformationInLineBynoandLabel("TS", toothInfo[1]);
                    _preAuthAction.SetToothInformationInLineBynoandLabel("OC", toothInfo[2]);
                    _preAuthAction.ClickOnSaveEditIconByLineNo();
                    _preAuthAction.WaitForWorking();
                    var processingHistoryPage = _preAuthAction.ClickOnPreAuthProcessingHistoryAndSwitch();
                    var audit = processingHistoryPage.GetPreAuthProcessingHistoryGridValueByRowCol(1, 4);
                    _preAuthAction =
                        processingHistoryPage.CloseClaimProcessingHistoryPageAndSwitchToPreAuthActionPage();
                    var note = _preAuthAction.PreAuthProcessingHistoryList(preAuthSeq)[0][3].Trim();
                    note.ShouldBeEqual(
                        $"TN changed from no value to {toothInfo[0]}. TS changed from no value to {toothInfo[1]}. OC changed from no value to {toothInfo[2]}.",
                        "Note set correct?");
                    audit.ShouldBeEqual(
                        $"TN changed from no value to {toothInfo[0]}. TS changed from no value to {toothInfo[1]}. OC changed from no value to {toothInfo[2]}.",
                        "Note set correct?");

                    _preAuthAction.GetToothInfoForPreAuth(preAuthSeq)
                        .ShouldCollectionBeEqual(toothInfo, "tooth info saved correctly?");
                    ClaimHistoryPage _claimHistory =
                        _preAuthAction.ClickOnTnLinkByRowToNavigateToPatientClaimHistoryPage();
                    _claimHistory.IsToothNumberSelected("permanent", "toothInfo[0]", 12)
                        .ShouldBeTrue("tooth number selected?");
                    _preAuthAction.CloseAnyPopupIfExist();
                    
                    StringFormatter.PrintMessage("Verify length restriction");
                    _preAuthAction.ClickOnEditIconOnLine();
                    _preAuthAction.SetToothInformationInLineBynoandLabel("TN", invalidToothInfo[0]);
                    _preAuthAction.SetToothInformationInLineBynoandLabel("TS", invalidToothInfo[1]);
                    _preAuthAction.SetToothInformationInLineBynoandLabel("OC", invalidToothInfo[2]);
                    _preAuthAction.GetToothInformationInLineBynoandLabel("TN").Length
                        .ShouldBeEqual(2, "Maximum 2 digit allowed");
                    _preAuthAction.GetToothInformationInLineBynoandLabel("TS").Length
                        .ShouldBeEqual(5, "Maximum 2 alphanumeric value allowed");
                    _preAuthAction.GetToothInformationInLineBynoandLabel("OC").Length
                        .ShouldBeEqual(2, "Maximum 2 alphabet allowed");
                    _preAuthAction.ClickOnCancelEditIconByLineNo();
                    
                    StringFormatter.PrintMessage("Verify error message when invalid data in entered");
                    _preAuthAction.ClickOnEditIconOnLine();
                    _preAuthAction.SetToothInformationInLineBynoandLabel("TN", toothInfoWithError[0]);
                    _preAuthAction.ClickOnSaveEditIconByLineNo();
                    _preAuthAction.IsPageErrorPopupModalPresent().ShouldBeTrue("Error present?");
                    _preAuthAction.GetPageErrorMessage()
                        .ShouldBeEqual($"{toothInfoWithError[0]} is not a valid TN.", "error message correct?");
                    _preAuthAction.ClosePageError();
                    _preAuthAction.ClickOnCancelEditIconByLineNo();

                    _preAuthAction.ClickOnEditIconOnLine();
                    _preAuthAction.SetToothInformationInLineBynoandLabel("OC", toothInfoWithError[1]);
                    _preAuthAction.ClickOnSaveEditIconByLineNo();
                    _preAuthAction.GetPageErrorMessage()
                        .ShouldBeEqual($"{toothInfoWithError[1]} is not a valid OC.", "error message correct?");
                    _preAuthAction.ClosePageError();
                }
                finally
                {
                    _preAuthAction.DeleteToothInfoForPreAuth(preAuthSeq);
                }
            }
        }

        [Test] //CAR-3279(CAR-3135)
        [Author("Shreya Pradhan")]
        public void Verify_Addition_Of_Triggerline_While_Adding_Flag()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                PreAuthSearchPage _preAuthSearch = automatedBase.QuickLaunch.NavigateToPreAuthSearch();
                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestName);
                var _authSeq = paramLists["authseq"];
                var flag = paramLists["flag"];
                _preAuthSearch.DeleteFlagAndAuditByAuthSeqFlagsLinNo(_authSeq, "'" + flag + "'", "1");
                PreAuthActionPage _preAuthAction =
                    _preAuthSearch.SearchByAuthSequenceAndNavigateToAuthAction(_authSeq, false, popup: true);
                var lines = _preAuthAction.GetLineDetailsValuesFromDB(_authSeq).Select(x => x[0]).ToList();
                _preAuthAction.ClickOnAddFlagIcon();
                _preAuthAction.GetSideWindow.IsDropDownBoxDisabled("Trigger Line").ShouldBeTrue("Trigger Line dropdown should be disabled when both line and flag are not selected");

                _preAuthAction.ClickOnLinesOnSelectLines();
                _preAuthAction.GetSideWindow.IsDropDownBoxDisabled("Trigger Line").ShouldBeTrue("Trigger Line dropdown should be disabled when a single line is selected but flag is not selected");
                _preAuthAction.ClickCancel();

                _preAuthAction.ClickOnAddFlagIcon();
                _preAuthAction.GetSideWindow.ClickOnCheckBoxByLabel("Select All Lines");
                _preAuthAction.GetSideWindow.SelectDropDownListValueByLabel("Flag", flag);
                _preAuthAction.GetSideWindow.IsDropDownBoxDisabled("Trigger Line").ShouldBeTrue("Trigger Line dropdown should be disabled when multiple lines and flag selected");
                _preAuthAction.ClickCancel();

                _preAuthAction.ClickOnAddFlagIcon();
                _preAuthAction.ClickOnLinesOnSelectLines();
                _preAuthAction.GetSideWindow.SelectDropDownListValueByLabel("Flag", flag);
                _preAuthAction.GetSideWindow.IsDropDownBoxDisabled("Trigger Line").ShouldBeFalse("Trigger Line dropdown should be enabled when a single line and flag is selected");
                _preAuthAction.GetSideWindow.GetDropDownList("Trigger Line").ShouldCollectionBeEquivalent(lines.Skip(1), "Trigger Line Drop down menu should show the line numbers of all the available pre-auth lines on the pre-auth except the current selected line.");
                _preAuthAction.GetSideWindow.SelectDropDownListValueByLabel("Trigger Line", lines[1]);
                _preAuthAction.GetSideWindow.FillIFrame("Note", "ADD FLAG TEST");
                _preAuthAction.ClickSaveButton();
                _preAuthAction.WaitForWorking();
                _preAuthAction.GetFlagDetailValueBylabelandFlag(flag, "TL").ShouldBeEqual(lines[1], "When saved, TL on the flag line should be populated with the selected Trigger Line value.");

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
                _preAuthAction.DeleteToothInfoForPreAuth(preAuthSeq);
                var tnDescription = _preAuthAction.GetShortProcCodeDescriptionFromDB(toothInfo[0], "DTOO");
                var ocDescription = _preAuthAction.GetShortProcCodeDescriptionFromDB(toothInfo[2], "ORAL");

                try
                {
                    _preAuthAction.ClickOnEditIconOnLine();
                    _preAuthAction.SetToothInformationInLineBynoandLabel("TN", toothInfo[0]);
                    _preAuthAction.SetToothInformationInLineBynoandLabel("TS", toothInfo[1]);
                    _preAuthAction.SetToothInformationInLineBynoandLabel("OC", toothInfo[2]);
                    _preAuthAction.ClickOnSaveEditIconByLineNo();
                    _preAuthAction.WaitForWorking();
                    _preAuthAction.WaitForStaticTime(1000);
                    var claimHistory = _preAuthAction.ClickOnPreAuthClaimProcessingHistoryAndSwitch();
                    claimHistory.WaitForStaticTime(2000);
                    claimHistory.ClickOnPreAuthIconAndNavigateToPatientPreAuthHistory();

                    StringFormatter.PrintMessage("Verify For every pre-auth line record TN,TS and OC columns are present");
                    for (int i = 1; i <= claimHistory.GetPatientPreAuthHistoryRowCount(); i++)
                    {
                        claimHistory.IsColumnNamePresentByRow("TN",i);
                        claimHistory.IsColumnNamePresentByRow("TS", i);
                        claimHistory.IsColumnNamePresentByRow("OC", i);
                    }

                    StringFormatter.PrintMessage("Verify values shown in the TN and OC columns will have tool-tips showing the descriptions.");
                    claimHistory.MouseOverPatientPreAuthTableAndGetToolTipString(1, 5).ShouldBeEqual(tnDescription, "Value shown in the TN column will have tool-tip showing the description");
                    claimHistory.MouseOverPatientPreAuthTableAndGetToolTipString(1, 7).ShouldBeEqual(ocDescription, "Value shown OC column will have tool-tip showing the description");

                    StringFormatter.PrintMessage("Verify When values are modified through Pre-Auth Action, the new values will be reflected in the history page view.");
                    claimHistory.GetPatientPreAuthHistoryTableValueByRowAndColumn(1, 5).ShouldBeEqual(toothInfo[0], "TN Value modified through Pre-Auth Action, should be reflected in the history page view.");
                    claimHistory.GetPatientPreAuthHistoryTableValueByRowAndColumn(1, 6).ShouldBeEqual(toothInfo[1], "TS Value modified through Pre-Auth Action, should be reflected in the history page view.");
                    claimHistory.GetPatientPreAuthHistoryTableValueByRowAndColumn(1, 7).ShouldBeEqual(toothInfo[2], "OC Value modified through Pre-Auth Action, should be reflected in the history page view.");

                    StringFormatter.PrintMessage("Verify scroll bar is present on Patient Tooth History view");
                    claimHistory.ClickDentalHistoryTab();
                    claimHistory.ResizeWindow(1200, 750);
                    claimHistory.IsVerticalScrollBarPresentInPatientToothHistoryTab().ShouldBeTrue("Verify scroll bar is present on Patient Tooth History view");

                }
                finally
                {
                    _preAuthAction.CloseAnyPopupIfExist();
                    _preAuthAction.DeleteToothInfoForPreAuth(preAuthSeq);
                }
            }
        }

    }
}

#endregion


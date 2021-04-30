using System;
using System.Collections.Generic;
using System.Diagnostics;
using Nucleus.Service.Data;
using System.Linq;
using System.Text.RegularExpressions;
using Nucleus.Service.PageServices.Claim;
using Nucleus.Service.PageServices.Settings.User;
using Nucleus.Service.PageServices.QA;
using Nucleus.Service.Support.Common.Constants;
using Nucleus.UIAutomation.TestSuites.Base;
using NUnit.Framework;
using UIAutomation.Framework.Utils;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Environment;
using System.Text;
using Nucleus.Service.PageServices.Base.Default;
using Nucleus.Service.SqlScriptObjects.Common_SQL;
using Nucleus.Service.Support.Menu;
using Nucleus.UIAutomation.TestSuites.Common;

namespace Nucleus.UIAutomation.TestSuites.TestCases
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class QaClaimSearch
    {
        #region PRIVATE FIELDS
        //private QaClaimSearchPage _qaClaimSearch;
        //private ProfileManagerPage _profileManager;
        //private List<string> _analystUserList;
        //private List<string> _assignedClientList;
        //private CommonValidations _commonValidations;
        //private UserProfileSearchPage _newUserProfileSearch;
        #endregion

        //#region OVERRIDE METHODS
        //protected override void ClassInit()
        //{
        //    try
        //    {
        //       base.ClassInit();
        //        _qaClaimSearch = QuickLaunch.NavigateToQaClaimSearch();
        //        _commonValidations = new CommonValidations(CurrentPage);
        //        // RetrieveListFromDatabase();
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
        //    base.TestCleanUp();
        //    if(CurrentPage.IsPageErrorPopupModalPresent())
        //        CurrentPage.ClosePageError();
        //    if (string.Compare(UserType.CurrentUserType, UserType.HCIADMIN, StringComparison.OrdinalIgnoreCase) != 0)
        //    {
        //        _qaClaimSearch = _qaClaimSearch.Logout().LoginAsHciAdminUser().NavigateToQaClaimSearch();
        //    }
        //    else if (CurrentPage.GetPageHeader() != PageHeaderEnum.QaClaimSearch.GetStringValue())
        //    {
        //        CurrentPage.NavigateToQaClaimSearch();
        //    }
        //    else
        //    {
        //        _qaClaimSearch.GetSideBarPanelSearch.OpenSidebarPanel();
        //        _qaClaimSearch.ClickOnClearButton();
        //        _qaClaimSearch.SelectOutstandingQaClaims();
        //        _qaClaimSearch.ClickOnFindButton();
        //    }          
        //}
        //protected override void ClassCleanUp()
        //{
        //    try
        //    {
        //        _qaClaimSearch.CloseDbConnection();
        //    }

        //    finally
        //    {
        //        base.ClassCleanUp();
        //    }
        //}
        //#endregion

        #region PROTECTED PROPERTIES

        //protected string GetType().FullName
        //{
        //    get
        //    {
        //        return GetType().FullName;
        //    }
        //}
        #endregion

        #region DBinteraction methods
        //private void RetrieveListFromDatabase()
        //{
        //    _analystUserList = _qaClaimSearch.GetAnalystList();
        //    _assignedClientList = _qaClaimSearch.GetAssignedClientListFromDB(EnvironmentManager.HciAdminUsername);

        //}

        #endregion
        #region TEST SUITES

        [Test] //CAR-3019(CAR-2947)
        [Order(11)]
        public void Verify_Restriction_Filter_and_Restriction_icon()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                var TestName = new StackFrame(true).GetMethod().Name;
                QaClaimSearchPage _qaClaimSearch = automatedBase.QuickLaunch.NavigateToQaClaimSearch();
                UserProfileSearchPage _newUserProfileSearch;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(GetType().FullName, TestName);
                var assignedRestrictionClaimList =
                    _qaClaimSearch.GetSearchResultsForAssignedRestriction(automatedBase.EnvironmentManager.Username);

                StringFormatter.PrintMessage("Verification if Restriction filter is present");
                _qaClaimSearch.GetSideBarPanelSearch
                    .IsSearchInputFieldDisplayedByLabel(QaClaimSearchFiltersEnum.Restrictions.ToString())
                    .ShouldBeTrue("Is Restrictions filter present?");
                _qaClaimSearch.GetSideBarPanelSearch
                    .GetInputValueByLabel(QaClaimSearchFiltersEnum.Restrictions.ToString())
                    .ShouldBeEqual("All", "'All' should be the default selection in Restriction dropdown");

                StringFormatter.PrintMessage("Navigating to User Profile Page to verify assigned restrictions");
                _newUserProfileSearch = automatedBase.CurrentPage.NavigateToNewUserProfileSearch();
                _newUserProfileSearch.SearchUserByNameOrId(new List<string> { automatedBase.EnvironmentManager.Username}, true);
                _newUserProfileSearch.ClickOnGridRowByUserIdToOpenUserSettingSideView(automatedBase.EnvironmentManager.Username);
                _newUserProfileSearch.ClickOnUserSettingTabByTabName(UserSettingsTabEnum.Clients.GetStringValue());

                var assignedRestrictionList = _newUserProfileSearch.GetAvailableAssignedList(3, false);

                StringFormatter.PrintMessage(
                    "Navigating to User Profile Page to verify assigned restrictions for user with no restrictions");
                _newUserProfileSearch.SearchUserByNameOrId(
                    new List<string> { automatedBase.EnvironmentManager.HciClaimViewRestrictionUsername}, true);
                _newUserProfileSearch.ClickOnGridRowByUserIdToOpenUserSettingSideView(automatedBase.EnvironmentManager
                    .HciClaimViewRestrictionUsername);
                _newUserProfileSearch.ClickOnUserSettingTabByTabName(UserSettingsTabEnum.Clients.GetStringValue());
                _newUserProfileSearch.GetAvailableAssignedList(3, false)
                    .ShouldCollectionBeEmpty("Restriction should not be assigned");

                StringFormatter.PrintMessage(
                    "Navigating to QA Claim Search with Outstanding Claims and All restriction filter");
                _qaClaimSearch = _newUserProfileSearch.NavigateToQaClaimSearch();
                _qaClaimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Analyst", paramLists["Analyst"]);
                _qaClaimSearch.GetSideBarPanelSearch.ClickOnFindButton();
                _qaClaimSearch.WaitForWorking();

                StringFormatter.PrintMessage("Verifying if the assigned restrictions match");
                _qaClaimSearch.GetSideBarPanelSearch
                    .GetAvailableDropDownList(QaClaimSearchFiltersEnum.Restrictions.ToString()).Skip(2)
                    .ShouldCollectionBeEqual(assignedRestrictionList, "Do assigned restrictions match?");
                List<string> defaultValues = _qaClaimSearch.GetSideBarPanelSearch
                    .GetAvailableDropDownList(QaClaimSearchFiltersEnum.Restrictions.ToString()).Take(2).ToList();

                StringFormatter.PrintMessage("Verification for Outstanding Claims filter with All restrictions");
                _qaClaimSearch.GetGridViewSection.GetGridListValueByCol(3)
                    .ShouldCollectionBeEquivalent(
                        _qaClaimSearch.GetQAClaimSearchResultsForAllRestrictionFilterForOutstandingQADb(
                            automatedBase.EnvironmentManager.Username),
                        "Search results match?");

                StringFormatter.PrintMessage("Verification for Outstanding Claims filter with No restrictions");
                SearchByRestrictionAndCheckValue("Restrictions", defaultValues[1],
                    _qaClaimSearch.GetQAClaimSearchResultsForNoRestrictionFilterForOutstandingQADb(automatedBase.EnvironmentManager
                        .Username));
                _qaClaimSearch.IsRestrictionIconPresentInSearchResult().ShouldBeFalse("Is restriction icon present?");

                StringFormatter.PrintMessage("Verification for Outstanding Claims filter for assigned restrctions");
                for (int i = 0; i < assignedRestrictionList.Count; i++)
                {
                    SearchByRestrictionAndCheckValue("Restrictions", assignedRestrictionList[i],
                        assignedRestrictionClaimList[i]);
                    _qaClaimSearch.IsRestrictionIconPresentInSearchResult()
                        .ShouldBeTrue($"Is {assignedRestrictionList[i]} icon present?");
                    _qaClaimSearch.GetRestrictionIconCountInSearchResult().ShouldBeEqual(
                        _qaClaimSearch.GetGridViewSection.GetGridRowCount(),
                        $"Is {assignedRestrictionList[i]} icon shown for all rows?");

                    _qaClaimSearch.GetRestrictionTooltip().ShouldBeEqual(
                        $"This claim is allowed to be viewed only by users that have the {assignedRestrictionList[i]} access.  Please contact management if you are viewing this claim in error.",
                        $"Tooltip for {assignedRestrictionList[i]} restriction should match");
                }

                StringFormatter.PrintMessage("Verifying for All Qa Claims filter with All restrictions");
                _qaClaimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                    QaClaimQuickSearchTypeEnum.AllQaClaims.GetStringValue());
                SearchByRestrictionAndCheckValue("Restrictions", defaultValues[0],
                    _qaClaimSearch.GetQAClaimsAllClaimsAllRestrcitionSearchResultDb(automatedBase.EnvironmentManager.Username));

                StringFormatter.PrintMessage("Verifying for All Qa Claims filter with No restrictions");
                SearchByRestrictionAndCheckValue("Restrictions", defaultValues[1],
                    _qaClaimSearch.GetQAClaimsAllClaimNoRestrcitionSearchResultDb(automatedBase.EnvironmentManager.Username));
                _qaClaimSearch.IsRestrictionIconPresentInSearchResult().ShouldBeFalse("Is restriction icon present?");

                StringFormatter.PrintMessage("Verifying for All Qa Claims filter with assigned restrictions");
                for (int i = 0; i < assignedRestrictionList.Count; i++)
                {
                    SearchByRestrictionAndCheckValue("Restrictions", assignedRestrictionList[i],
                        assignedRestrictionClaimList[i]);
                    _qaClaimSearch.IsRestrictionIconPresentInSearchResult()
                        .ShouldBeTrue($"Is {assignedRestrictionList[i]} icon present?");
                    _qaClaimSearch.GetRestrictionIconCountInSearchResult().ShouldBeEqual(
                        _qaClaimSearch.GetGridViewSection.GetGridRowCount(),
                        $"Is {assignedRestrictionList[i]} icon shown for all rows?");

                    _qaClaimSearch.GetRestrictionTooltip().ShouldBeEqual(
                        $"This claim is allowed to be viewed only by users that have the {assignedRestrictionList[i]} access.  Please contact management if you are viewing this claim in error.",
                        $"Tooltip for {assignedRestrictionList[i]} restriction should match");
                }

                _qaClaimSearch.GetSideBarPanelSearch.ClickOnClearLink();

                StringFormatter.PrintMessage(
                    "Verify if the user has no restriction available then only default values are displayed in the restriction list");
                _qaClaimSearch.Logout().LoginAsClaimViewRestrictionUser();

                _qaClaimSearch = automatedBase.CurrentPage.NavigateToQaClaimSearch();
                _qaClaimSearch.GetSideBarPanelSearch.GetAvailableDropDownList("Restrictions")
                    .ShouldBeEqual(defaultValues,
                        "Only default values available when no restriction is assigned ?");

                void SearchByRestrictionAndCheckValue(string label, string value, List<string> valueToCompare)
                {
                    _qaClaimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel(label, value);
                    _qaClaimSearch.GetSideBarPanelSearch.ClickOnFindButton();
                    _qaClaimSearch.WaitForWorking();
                    _qaClaimSearch.GetGridViewSection.GetGridListValueByCol(3)
                        .ShouldCollectionBeEquivalent(
                            valueToCompare, "Search results should match");
                }
            }
        }

        [Test, Category("SmokeTestDeployment")]//TANT-105
        public void Verify_SidebarPanel_and_Navigation_TO_Claim_Action()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                QaClaimSearchPage _qaClaimSearch = automatedBase.QuickLaunch.NavigateToQaClaimSearch();
                var sortOption = automatedBase.DataHelper.GetMappingData(GetType().FullName, "QA_Claim_Search_Sorting_Options")
                    .Values.ToList();

                StringFormatter.PrintMessage("Verify open and close of QA claims search Panel");
                _qaClaimSearch.GetSideBarPanelSearch.IsSideBarPanelOpen()
                    .ShouldBeTrue("Side bar panel  open by default?");
                _qaClaimSearch.GetSideBarPanelSearch.ClickOnToggleSidebarPanelButton();
                _qaClaimSearch.GetSideBarPanelSearch.IsSideBarPanelOpen().ShouldBeFalse("Side bar panel closed?");

                StringFormatter.PrintMessage("Verify Sorting options ");
                _qaClaimSearch.IsSortIconPresent().ShouldBeTrue("Sort icon Present?");
                _qaClaimSearch.GetFilterOptionList().ShouldCollectionBeEqual(sortOption, "Sort options equal?");
                _qaClaimSearch.ClickOnFilterOption();

                StringFormatter.PrintMessage("Verify Navigation to Claim Action Page from Qa Claim Search");
                if (!_qaClaimSearch.GetSideBarPanelSearch.IsSideBarPanelOpen())
                    _qaClaimSearch.GetSideBarPanelSearch.ClickOnToggleSidebarPanelButton();
                _qaClaimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Restrictions", "No Restriction");
                _qaClaimSearch.GetSideBarPanelSearch.ClickOnFindButton();
                _qaClaimSearch.WaitForWorking();
                var claimseq = _qaClaimSearch.GetGridViewSection
                    .GetValueInGridByColRow(3);
                var newClaimAction = _qaClaimSearch.ClickOnClaimSequenceAndNavigateToNewClaimActionPage(claimseq);
                newClaimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                newClaimAction.GetClaimSequence().ShouldBeEqual(claimseq, "Is claim Sequence same?");
                _qaClaimSearch = newClaimAction.ClickClaimSearchIconAndNavigateToQaClaimSearchPage();
            }
        }

        [Test] //US66659
        [Order(11)]
        public void View_And_Edit_QA_Fail_Notes()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                var TestName = new StackFrame(true).GetMethod().Name;
                QaClaimSearchPage _qaClaimSearch = automatedBase.QuickLaunch.NavigateToQaClaimSearch();
                var analyst = automatedBase.DataHelper.GetSingleTestData(GetType().FullName, TestName, "Analyst",
                    "Value");
                var client = automatedBase.DataHelper.GetSingleTestData(GetType().FullName, TestName, "Client",
                    "Value");
                var claseq = automatedBase.DataHelper.GetSingleTestData(GetType().FullName, TestName,
                    "ClaimSequence", "Value");
                var reasonCode = automatedBase.DataHelper
                    .GetSingleTestData(GetType().FullName, TestName, "ReasonCode", "Value")
                    .Split(',').ToList();
                var errorDescription = automatedBase.DataHelper
                    .GetSingleTestData(GetType().FullName, TestName, "ErrorDescription", "Value")
                    .Split(',').ToList();
                SearchClaimSeqForAnalystAndClient(analyst.Split('(')[0].Trim(), client, _qaClaimSearch);
                if (_qaClaimSearch.GetQaResultStatusByClaimSequence(claseq) == "Passed")
                {
                    var newClaimAction = _qaClaimSearch.ClickOnClaimSequenceAndNavigateToNewClaimActionPage(claseq);
                    newClaimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                    //newClaimAction.ClickWorkListIcon();
                    ClickClaimActionQaFail_ClickOnEditOnClaimQaAndSelect_RecordQaErrorDescription(newClaimAction,
                        reasonCode[0], errorDescription[0]);
                    newClaimAction.SaveRecordQAErrorAndNavigateToNextClaim();
                    _qaClaimSearch = newClaimAction.ClickClaimSearchIconAndNavigateToQaClaimSearchPage();
                    _qaClaimSearch.GetQaResultStatusByClaimSequence(claseq)
                        .ShouldBeEqual("Failed", "QA status updated to QA Fail.");
                }

                SearchClaimSeqForAnalystAndClient(analyst.Split('(')[0].Trim(), client, _qaClaimSearch);
                if (_qaClaimSearch.GetQaResultStatusByClaimSequence(claseq) == "Failed")
                {
                    var newClaimAction = _qaClaimSearch.ClickOnClaimSequenceAndNavigateToNewClaimActionPage(claseq);
                    newClaimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                    //newClaimAction.ClickWorkListIcon();
                    ClickClaimActionQaFail_ClickOnEditOnClaimQaAndSelect_RecordQaErrorDescription(newClaimAction,
                        reasonCode[1], errorDescription[1]);
                    newClaimAction.ClickClaimQaCancelLink();
                    if (newClaimAction.IsPageErrorPopupModalPresent())
                    {
                        newClaimAction.ClosePageError();
                    }

                    ClickOnEditOnClaimQaAndSelect_RecordQaErrorDescription(newClaimAction, "", "");
                    newClaimAction.ClickSaveRecordQAError();
                    if (newClaimAction.IsPageErrorPopupModalPresent())
                    {
                        newClaimAction.ClosePageError();
                    }

                    Select_RecordQaErrorDescription(newClaimAction, reasonCode[2], errorDescription[2]);
                    newClaimAction.ClickClaimQaCancelLink();
                    if (newClaimAction.IsPageErrorPopupModalPresent())
                    {
                        newClaimAction.ClickOkCancelOnConfirmationModal(false);
                    }

                    newClaimAction.ClickClaimQaCancelLink();
                    if (newClaimAction.IsPageErrorPopupModalPresent())
                    {
                        newClaimAction.ClickOkCancelOnConfirmationModal(true);
                    }

                    ClickOnEditOnClaimQaAndSelect_RecordQaErrorDescription(newClaimAction, reasonCode[2],
                        errorDescription[2]);
                    newClaimAction.SaveRecordQAErrorAndNavigateToNextClaim();
                    _qaClaimSearch = newClaimAction.ClickClaimSearchIconAndNavigateToQaClaimSearchPage();
                    newClaimAction = _qaClaimSearch.ClickOnClaimSequenceAndNavigateToNewClaimActionPage(claseq);
                    newClaimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                    //newClaimAction.ClickWorkListIcon();
                    var claimProcessingHistory = newClaimAction.ClickOnClaimProcessingHistoryAndSwitch();
                    var dateTime = claimProcessingHistory.GetClaimHistoryTableDataValueByRowCol(2, 1);
                    Convert.ToDateTime(dateTime).Date.ShouldBeEqual(DateTime.Now.Date, "Is Reviewed Date equal?");
                    claimProcessingHistory.GetClaimHistoryTableDataValueByRowCol(2, 2)
                        .ShouldBeEqual(ClaimAuditActionEnum.QaFail.GetStringValue(), "Verify Action");
                    claimProcessingHistory.GetClaimHistoryTableDataValueByRowCol(2, 4)
                        .ShouldBeEqual(analyst, "Verification of QA Reviewer should be in proper format");
                    claimProcessingHistory.GetClaimHistoryTableDataValueByRowCol(2, 5)
                        .ShouldBeEqual("Internal", "User Type should Internal.");
                    claimProcessingHistory.GetClaimHistoryTableDataValueByRowCol(2, 6)
                        .ShouldBeEqual("Reviewed", "Status should equal to current claim status.");
                    newClaimAction = claimProcessingHistory.CloseClaimProcessingHistoryPageAndSwitchToClaimActionPage();
                    _qaClaimSearch = newClaimAction.ClickClaimSearchIconAndNavigateToQaClaimSearchPage();
                    newClaimAction = _qaClaimSearch.ClickOnClaimSequenceAndNavigateToNewClaimActionPage(claseq);
                    ClosePatientClaimHisotryAndClickClaimActionQaPass(newClaimAction, false);
                    _qaClaimSearch = newClaimAction.ClickClaimSearchIconAndNavigateToQaClaimSearchPage();
                    _qaClaimSearch.GetQaResultStatusByClaimSequence(claseq)
                        .ShouldBeEqual("Failed", "QA Fail status should retain.");
                    newClaimAction = _qaClaimSearch.ClickOnClaimSequenceAndNavigateToNewClaimActionPage(claseq);
                    ClosePatientClaimHisotryAndClickClaimActionQaPass(newClaimAction, true);
                    newClaimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                    _qaClaimSearch = newClaimAction.ClickClaimSearchIconAndNavigateToQaClaimSearchPage();
                    _qaClaimSearch.GetQaResultStatusByClaimSequence(claseq)
                        .ShouldBeEqual("Passed", "QA Fail status should be updated to QA Pass.");
                }
            }
        }

        [Test, Category("CommonTableDependent")] //US66655
        [Order(7)]
        public void Verify_QA_Pass_result()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                var TestName = new StackFrame(true).GetMethod().Name;
                QaClaimSearchPage _qaClaimSearch = automatedBase.QuickLaunch.NavigateToQaClaimSearch();

                var analyst = automatedBase.DataHelper.GetSingleTestData(GetType().FullName, TestName, "Analyst",
                    "Value");
                var claseq = automatedBase.DataHelper
                    .GetSingleTestData(GetType().FullName, TestName, "ClaimSequence", "Value")
                    .Split(',').ToList();
                var client = automatedBase.DataHelper.GetSingleTestData(GetType().FullName, TestName, "Client",
                    "Value");
                var reasonCode = automatedBase.DataHelper.GetSingleTestData(GetType().FullName, TestName,
                    "ReasonCode", "Value");
                var errorDescription = automatedBase.DataHelper.GetSingleTestData(GetType().FullName, TestName,
                    "ErrorDescription", "Value");
                _qaClaimSearch.UpdateDeleteQaClaimReviewStatusForClaim(claseq[0]);
                SearchClaimSeqForAnalystAndClient(analyst.Split('(')[0].Trim(), client, _qaClaimSearch);
                var newClaimAction = _qaClaimSearch.ClickOnClaimSequenceAndNavigateToNewClaimActionPage(claseq[0]);
                newClaimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                //newClaimAction.ClickWorkListIcon();
                var claimStatus = newClaimAction.GetClaimStatus();
                newClaimAction.ClickClaimQaPassAndWaitForNextClaim();
                _qaClaimSearch = newClaimAction.ClickClaimSearchIconAndNavigateToQaClaimSearchPage();
                _qaClaimSearch.ClickOnClaimSequenceAndNavigateToNewClaimActionPage(claseq[0]);
                newClaimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                newClaimAction.CheckQaDoneStatus().ShouldBeTrue("Verification of QA Done status should updated.");
                newClaimAction.GetClaimStatus().ShouldBeEqual(claimStatus, "Claim Status should be same.");
                var claimProcessingHistory = newClaimAction.ClickOnClaimProcessingHistoryAndSwitch();
                var dateTime = claimProcessingHistory.GetClaimHistoryTableDataValueByRowCol(2, 1);
                Convert.ToDateTime(dateTime).Date.ShouldBeEqual(DateTime.Now.Date, "Is Reviewed Date equal?");
                claimProcessingHistory.GetClaimHistoryTableDataValueByRowCol(2, 2)
                    .ShouldBeEqual(ClaimAuditActionEnum.QaPass.GetStringValue(), "Verify Action");
                claimProcessingHistory.GetClaimHistoryTableDataValueByRowCol(2, 4)
                    .ShouldBeEqual(analyst, "Verification of QA Reviewer should be in proper format");
                claimProcessingHistory.GetClaimHistoryTableDataValueByRowCol(2, 5)
                    .ShouldBeEqual("Internal", "User Type should Internal.");
                claimProcessingHistory.GetClaimHistoryTableDataValueByRowCol(2, 6)
                    .ShouldBeEqual("Reviewed", "Status should equal to current claim status.");
                newClaimAction = claimProcessingHistory.CloseClaimProcessingHistoryPageAndSwitchToClaimActionPage();
                _qaClaimSearch = newClaimAction.ClickClaimSearchIconAndNavigateToQaClaimSearchPage();
                if (_qaClaimSearch.GetQaResultStatusByClaimSequence(claseq[1]) != "Failed")
                {
                    newClaimAction = _qaClaimSearch.ClickOnClaimSequenceAndNavigateToNewClaimActionPage(claseq[1]);
                    newClaimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                    //newClaimAction.ClickWorkListIcon();
                    newClaimAction.ClickClaimActionQaFail();
                    newClaimAction.ClickonOnEditOnClaimQaByRow();
                    newClaimAction.SelectRecordQaErrorDropdown("Reason Code", reasonCode);
                    newClaimAction.SelectRecordQaErrorDropdown("Error Description", errorDescription);
                    newClaimAction.SaveRecordQAErrorAndNavigateToNextClaim();
                    _qaClaimSearch = newClaimAction.ClickClaimSearchIconAndNavigateToQaClaimSearchPage();
                }

                newClaimAction = _qaClaimSearch.ClickOnClaimSequenceAndNavigateToNewClaimActionPage(claseq[1]);
                ClosePatientClaimHisotryAndClickClaimActionQaPass(newClaimAction, false);
                _qaClaimSearch = newClaimAction.ClickClaimSearchIconAndNavigateToQaClaimSearchPage();
                _qaClaimSearch.GetQaResultStatusByClaimSequence(claseq[1])
                    .ShouldBeEqual("Failed", "QA Fail status should retain.");
                newClaimAction = _qaClaimSearch.ClickOnClaimSequenceAndNavigateToNewClaimActionPage(claseq[1]);
                ClosePatientClaimHisotryAndClickClaimActionQaPass(newClaimAction, true);
                newClaimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                _qaClaimSearch = newClaimAction.ClickClaimSearchIconAndNavigateToQaClaimSearchPage();
                _qaClaimSearch.GetQaResultStatusByClaimSequence(claseq[1])
                    .ShouldBeEqual("Passed", "QA Fail status should updated to QA Pass.");
            }
        }       

        [Test] //US66619+US66650
        [Order(1)]
        [Retrying(Times = 3)]
        public void Verify_searched_grid_display_result()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                var TestName = new StackFrame(true).GetMethod().Name;
                QaClaimSearchPage _qaClaimSearch = automatedBase.QuickLaunch.NavigateToQaClaimSearch();

                var analyst = automatedBase.DataHelper.GetSingleTestData(GetType().FullName, TestName, "Analyst",
                    "Value");
                _qaClaimSearch.SelectAllQaClaims();
                _qaClaimSearch.ClickOnFindButton();
                _qaClaimSearch.GetListValueOfGrid(4).Select(DateTime.Parse).ToList().IsInAscendingOrder()
                    .ShouldBeTrue("Are records in Descending Order(oldest dates first)");
                _qaClaimSearch.SelectSmtstClient();
                _qaClaimSearch.SelectAnalyst(analyst);
                _qaClaimSearch.ClickOnFindButton();

                _qaClaimSearch
                    .IsClaimAuditRowPresentForQaPassByClaseq(_qaClaimSearch.GetClaimSequenceForQaPassResultIcon())
                    .ShouldBeTrue("Verificaiton of QA Pass Icon with audit record in database for that claim");
                _qaClaimSearch
                    .IsClaimAuditRowPresentForQaFailByClaseq(_qaClaimSearch.GetClaimSequenceForQaFailResultIcon())
                    .ShouldBeTrue("Verificaiton of QA Fail Icon with audit record in database for that claim");
                _qaClaimSearch
                    .IsClaimAuditRowPresentForNoQaResultByClaseq(_qaClaimSearch.GetClaimSequenceForNoQaResultIcon())
                    .ShouldBeFalse("Verificaiton of No QA Result with audit record in database for that claim");

                _qaClaimSearch.GetGridViewSection.IsLabelPresentByCol()
                    .ShouldBeFalse("Label should not be displayed for Client Code");
                _qaClaimSearch.GetGridViewSection.GetValueInGridByColRow().Length
                    .ShouldBeGreater(0,
                        "Client Code should be present and character Length should be greater than zero");

                _qaClaimSearch.GetGridViewSection.IsLabelPresentByCol(3)
                    .ShouldBeFalse("Label should not be displayed for Claim Sequence");

                _qaClaimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("QA Result",
                    ClaimAuditActionEnum.QaPass.GetStringValue());
                _qaClaimSearch.ClickOnFindButton();

                var claseq = _qaClaimSearch.GetGridViewSection.GetValueInGridByColRow(3);

                var newClaimAction = _qaClaimSearch.ClickOnClaimSequenceAndNavigateToNewClaimActionPage(claseq);
                newClaimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                var qaToolTipMessage = newClaimAction.GetQaAuditIconTooltip();
                newClaimAction.GetPageHeader().ShouldBeEqual("Claim Action",
                    "Page should redirect to New Claim Action Page");
                newClaimAction.GetClaimSequence().ShouldBeEqual(claseq, "Is Associated Claim page open?");
                newClaimAction.GetWindowHandlesCount()
                    .ShouldBeEqual(1,
                        "New Claim Action Page should open as an existing page and not as a pop out page");

                _qaClaimSearch = newClaimAction.ClickClaimSearchIconAndNavigateToQaClaimSearchPage();
                _qaClaimSearch.GetPageHeader().ShouldBeEqual(PageHeaderEnum.QaClaimSearch.GetStringValue(),
                    "Page should Redirect to QA Claim Search Page");

                _qaClaimSearch.GetGridViewSection.GetLabelInGridByColRow(4)
                    .ShouldBeEqual("" +
                                   "Approve" +
                                   " Date:", "Verification of Approve Date Label");
                _qaClaimSearch.GetGridViewSection.GetValueInGridByColRow(4).IsDateInFormat()
                    .ShouldBeTrue("Is Approve Date in correct format");

                _qaClaimSearch.GetGridViewSection.IsLabelPresentByCol(5)
                    .ShouldBeFalse("Label should not be displayed for Client Code");
                _qaClaimSearch.GetGridViewSection.GetValueInGridByColRow(5).DoesNameContainsOnlyFirstWithLastname()
                    .ShouldBeTrue("Analyst Name should be in <FirstName> <LastName> format");

                _qaClaimSearch.GetGridViewSection.GetLabelInGridByColRow(6)
                    .ShouldBeEqual("QA Review Date:", "Verification of QA Review Date Label");
                _qaClaimSearch.GetGridViewSection.GetValueInGridByColRow(6).IsDateInFormat()
                    .ShouldBeTrue("Is Approve Date in correct format");
                qaToolTipMessage.ShouldBeEqual("QA Done", "Is Claim in QA Ready Mode for Blank QA Review Date ?");
                var expectedTotalQaReviewClaimCount = _qaClaimSearch.GetTotalQaReviewClaimCount();
                _qaClaimSearch.ClickOnClearButton();
                _qaClaimSearch.SelectAllQaClaims();
                _qaClaimSearch.ClickOnFindButton();
                var loadMoreValue = _qaClaimSearch.GetPagination.GetLoadMoreText();
                loadMoreValue.ShouldBeEqual(
                    string.Format("Viewing 25 of {0} (Load More)", expectedTotalQaReviewClaimCount),
                    "Verification of Load More text");
                _qaClaimSearch.GetPagination.ClickOnLoadMore();
                loadMoreValue = _qaClaimSearch.GetPagination.GetLoadMoreText();
                var numbers = Regex.Split(loadMoreValue, @"\D+").Where(s => s != String.Empty)
                    .Select(m => int.Parse(m.Trim())).ToList();
                numbers[0].ShouldBeGreater(25, "Load More Text Count should Updated");
                numbers[1].ShouldBeEqual(expectedTotalQaReviewClaimCount, "Total Count Should be same");
                _qaClaimSearch.GetGridViewSection.IsVerticalScrollBarPresentInGridSection()
                    .ShouldBeTrue("Vertical Scrollbar should Present");
                _qaClaimSearch.SelectOutstandingQaClaims();
                _qaClaimSearch.SelectClient(ClientEnum.SMTST.ToString());
                _qaClaimSearch.ClickOnFindButton();
                //_qaClaimSearch.GetGridViewSection.IsVerticalScrollBarPresentInGridSection()
                //    .ShouldBeFalse("Vertical Scrollbar should not Present");
                claseq = _qaClaimSearch.GetGridViewSection.GetValueInGridByColRow(3);
                newClaimAction = _qaClaimSearch.ClickOnClaimSequenceAndNavigateToNewClaimActionPage(claseq);
                newClaimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                qaToolTipMessage = newClaimAction.GetQaAuditIconTooltip();
                _qaClaimSearch = newClaimAction.ClickClaimSearchIconAndNavigateToQaClaimSearchPage();
                _qaClaimSearch.GetGridViewSection.GetValueInGridByColRow(6)
                    .ShouldBeNullorEmpty("Is Reviwed Date Null?");
                qaToolTipMessage.ShouldBeEqual("QA Ready", "Is Claim in QA Ready Mode for Blank QA Review Date ?");
            }
        }

        //[Test]
        //public void Verify_navigation_and_security()
        //{
        //    _profileManager = QuickLaunch.NavigateToProfileManager();
        //    _profileManager.ClickOnPrivileges();
        //    _profileManager.IsAuthorityAssigned("QA Manager").ShouldBeTrue("'QA Manager' authority is assigned.");
        //    _profileManager.NavigateToQaClaimSearch();
        //    _qaClaimSearch.IsQaClaimSearchSubMenuPresent().ShouldBeTrue("QA Claim Search submenu present for authorized user.");
        //    _qaClaimSearch.GetPageInsideTitle().ShouldBeEqual(PageHeaderEnum.QaClaimSearch.GetStringValue(), "Correct Page Title displayed inside the page.");
        //    _qaClaimSearch.GetSideBarPanelSearch.IsSideBarPanelOpen().ShouldBeTrue("Filter Panel present.");
            
        //    _qaClaimSearch.Logout().LoginAsClientUser();
        //    _qaClaimSearch.IsQaClaimSearchSubMenuPresent().ShouldBeFalse("QA Claim Search submenu absent for client user.");
        //    _qaClaimSearch.Logout().LoginAsUserHavingNoManageCategoryAuthority();//uiautomation3
        //    _profileManager = QuickLaunch.NavigateToProfileManager();
        //    _profileManager.ClickOnPrivileges();
        //    _profileManager.IsAuthorityAssigned("QA Manager").ShouldBeTrue("'QA Manager' authority is assigned with read only");
        //    _profileManager.ClickOnQuickLaunch();
        //    _qaClaimSearch.IsQaClaimSearchSubMenuPresent().ShouldBeFalse("QA Claim Search submenu absent for read only authority.");
        //    _qaClaimSearch.Logout().LoginAsUserHavingNoAnyAuthority();//uiautomation4
        //    _qaClaimSearch.IsQaClaimSearchSubMenuPresent().ShouldBeFalse("QA Claim Search submenu absent for unauthorized user.");
        //}

        [Test, Category("Acceptance")] //CAR-3184 [CAR-3314]
        [Author("Shyam Bhattarai")] 
        public void Verification_Of_Lock_Icon_In_Qa_Claim_Search_Page()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists = automatedBase.DataHelper.GetTestData(GetType().FullName, TestName);
                var approveDates = paramLists["approveDates"].Split(';').ToList();
                var claseqs = paramLists["claseqs"].Split(';').ToList();
                var analyst = paramLists["analyst"];
                var lockIconTooltip = paramLists["lockIconTooltip"];

                QaClaimSearchPage _qaClaimSearch = automatedBase.QuickLaunch.NavigateToQaClaimSearch();

                try
                {
                    StringFormatter.PrintMessageTitle("Opening the claim from QA Claim Search page to lock the claim");
                    _qaClaimSearch.SelectAllQaClaims();
                    _qaClaimSearch.SelectAnalyst(analyst);
                    _qaClaimSearch.SelectClient(ClientEnum.SMTST.ToString());
                    _qaClaimSearch.GetSideBarPanelSearch.SetDateField("Approve Date", approveDates[0], 1);
                    _qaClaimSearch.GetSideBarPanelSearch.SetDateField("Approve Date", approveDates[0], 1);
                    _qaClaimSearch.ClickOnFindButton();

                    var claimAction = _qaClaimSearch.ClickOnClaimSequenceAndNavigateToNewClaimActionPage(claseqs[0]);
                    claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();

                    var url = _qaClaimSearch.CurrentPageUrl;

                    StringFormatter.PrintMessageTitle(
                        "Logging in as a different user to verify whether the claim has been locked");
                    claimAction =
                        _qaClaimSearch.SwitchToOpenNewClaimActionByUrl(url,
                            automatedBase.EnvironmentManager.HciAdminUsername1);

                    claimAction.IsLocked()
                        .ShouldBeTrue("Claim should be locked when opened via the Qa Claim Search page by another user");
                    _qaClaimSearch = claimAction.NavigateToQaClaimSearch();

                    _qaClaimSearch.SelectAllQaClaims();
                    _qaClaimSearch.SelectAnalyst(analyst);
                    _qaClaimSearch.SelectClient(ClientEnum.SMTST.ToString());
                    _qaClaimSearch.GetSideBarPanelSearch.SetDateField("Approve Date", approveDates[0], 1);
                    _qaClaimSearch.GetSideBarPanelSearch.SetDateField("Approve Date", approveDates[0], 2);
                    _qaClaimSearch.ClickOnFindButton();

                    _qaClaimSearch.GetGridViewSection.IsLockIconPresentInGrid()
                        .ShouldBeTrue("Lock icon should be shown in Qa Claim Search grid");
                    _qaClaimSearch.GetGridViewSection.GetLockToolTipText()
                        .ShouldBeEqual(lockIconTooltip, "Lock Tooltip text should be correct");

                    StringFormatter.PrintMessageTitle("Logging in as the same user to remove the lock from the claim");
                    _qaClaimSearch = _qaClaimSearch
                        .SwitchToOpenNewClaimActionByUrl(url, automatedBase.EnvironmentManager.HciAdminUsername)
                        .NavigateToQaClaimSearch();

                    SearchInQaClaimSearchPage(approveDates[1]);
                    claimAction = _qaClaimSearch.ClickOnClaimSequenceAndNavigateToNewClaimActionPage(claseqs[1]);
                    claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();

                    StringFormatter.PrintMessageTitle(
                        "Logging in as a different user to verify whether the claim lock has been removed");
                    _qaClaimSearch = _qaClaimSearch
                        .SwitchToOpenNewClaimActionByUrl(url, automatedBase.EnvironmentManager.HciAdminUsername1)
                        .NavigateToQaClaimSearch();
                    SearchInQaClaimSearchPage(approveDates[0]);

                    _qaClaimSearch.GetGridViewSection.IsLockIconPresentInGrid().ShouldBeFalse(
                        "Lock should be removed once the user locking the claim has opened another claim from Qa Claim Search Page");

                    #region LOCAL METHOD

                    void SearchInQaClaimSearchPage(string dateOfApproval)
                    {
                        _qaClaimSearch.SelectAllQaClaims();
                        _qaClaimSearch.SelectAnalyst(analyst);
                        _qaClaimSearch.SelectClient(ClientEnum.SMTST.ToString());
                        _qaClaimSearch.GetSideBarPanelSearch.SetDateField("Approve Date", dateOfApproval, 1);
                        _qaClaimSearch.GetSideBarPanelSearch.SetDateField("Approve Date", dateOfApproval, 2);
                        _qaClaimSearch.ClickOnFindButton();
                    }

                    #endregion
                }
                finally
                {
                    StringFormatter.PrintMessageTitle("Running the finally block. Removing the claim locks from the DB");
                    _qaClaimSearch.GetCommonSql.RemoveQaClaimLockFromDB(claseqs[0]);
                    _qaClaimSearch.GetCommonSql.RemoveQaClaimLockFromDB(claseqs[1]);
                }
            }
        }

        [Test] //US66651
        
        public void Validation_of_filter_panel_and_expected_plus_default_value_of_filter_panel_input_fields()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                QaClaimSearchPage _qaClaimSearch = automatedBase.QuickLaunch.NavigateToQaClaimSearch();
                List<string> _analystUserList = _qaClaimSearch.GetAnalystList();
                List<string> _assignedClientList;
                var expectedQuickSearchList =
                    automatedBase.DataHelper.GetMappingData(GetType().FullName, "QuickSearch").Values.ToList();
                _analystUserList = _qaClaimSearch.GetAnalystList();
                var qAResult = automatedBase.DataHelper.GetMappingData(GetType().FullName, "QAResult").Values.ToList();
                _qaClaimSearch.GetSideBarPanelSearch.IsSideBarPanelOpen()
                    .ShouldBeTrue("QA Claim Search Panel on sidebar is open by default when user lands on page.");
                _qaClaimSearch.ClickOnToggleFindAnalystPanelButton();
                _qaClaimSearch.GetSideBarPanelSearch.IsSideBarPanelOpen()
                    .ShouldBeFalse("QA Claim Search Panel on sidebar is hidden when toggle button is clicked.");
                _qaClaimSearch.ClickOnToggleFindAnalystPanelButton();
                StringFormatter.PrintMessageTitle(
                    "Verifying Default value behaviour for QA Claim search Filter input panel fields");
                ValidateDefaultValueForQaClaimSearch(QaClaimQuickSearchTypeEnum.OutstandingQaClaims.GetStringValue(), _qaClaimSearch);
                var qsDropDownList = _qaClaimSearch.GetSideBarPanelSearch.GetAvailableDropDownList("Quick Search");
                qsDropDownList.Count.ShouldBeGreater(0, "List of Quick Search is greater than zero.");
                qsDropDownList.ShouldCollectionBeEqual(expectedQuickSearchList, "Quick Search List is As Expected");
                _qaClaimSearch.GetSideBarPanelSearch.GetInputValueByLabelPlaceholder("Analyst")
                    .ShouldBeEqual("Select Analyst", "Placeholder value of Analyst is as expected");
                ValidateDropDownFoDefaultValueAndExpectedList("Analyst", _analystUserList,_qaClaimSearch);
                _analystUserList[0].DoesNameContainsOnlyFirstWithLastname()
                    .ShouldBeTrue("Analyst should be in proper format of <firstname> <lastname>");
                _qaClaimSearch.GetSideBarPanelSearch.GetInputValueByLabelPlaceholder("Client")
                    .ShouldBeEqual("Select Client", "Placeholder value of Client is as expected");
                _assignedClientList = _qaClaimSearch.GetAssignedClientList(automatedBase.EnvironmentManager.HciAdminUsername);
                ValidateDropDownFoDefaultValueAndExpectedList("Client", _assignedClientList, _qaClaimSearch);
                ValidateDropDownFoDefaultValueAndExpectedList("QA Result", qAResult, _qaClaimSearch, true);
                _qaClaimSearch.GetSideBarPanelSearch.GetInputValueByLabelPlaceholder("QA Reviewer")
                    .ShouldBeEqual("Select QA Reviewer", "Placeholder value of QA Reviewer is as expected");
                ValidateDropDownFoDefaultValueAndExpectedList("QA Reviewer", _analystUserList, _qaClaimSearch);
                _analystUserList[0].DoesNameContainsOnlyFirstWithLastname()
                    .ShouldBeTrue("QA Reviewer should be in proper format of <firstname> <lastname>");
                ValidateQaClaimSearchDateFields("Approve Date", _qaClaimSearch);
                ValidateQaClaimSearchDateFields("QA Review Date", _qaClaimSearch);
                _qaClaimSearch.ClickOnClearButton();
                StringFormatter.PrintMessageTitle(
                    "Validate For default value of QA Claim Search filter panel fields, advanced filter remains open and Quick Search defaults to all");
                //default values for all should retain but quick search value defaults to all
                ValidateDefaultValueForQaClaimSearch(QaClaimQuickSearchTypeEnum.AllQaClaims.GetStringValue(), _qaClaimSearch);
            }
        }

        [Test] //US66651
        public void Validation_of_filter_panel_and_search_result_for_basic_search_filter()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                QaClaimSearchPage _qaClaimSearch = automatedBase.QuickLaunch.NavigateToQaClaimSearch();
                _qaClaimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                    QaClaimQuickSearchTypeEnum.OutstandingQaClaims.GetStringValue(), false);
                _qaClaimSearch.ClickOnFindButton();
                var loadMoreValue = _qaClaimSearch.GetPagination.GetLoadMoreText();
                var numbers = Regex.Split(loadMoreValue, @"\D+").Where(s => s != String.Empty)
                    .Select(m => int.Parse(m.Trim())).ToList();
                var count = numbers[1] % 25 == 0 ? numbers[1] / 25 - 1 : numbers[1] / 25;
                for (var i = 0; i < count; i++)
                {
                    _qaClaimSearch.GetPagination.ClickOnLoadMore();
                }

                _qaClaimSearch.GetGridViewSection.GetGridListValueByColAndSort(3).ShouldCollectionBeEqual(
                    _qaClaimSearch.GetOutstandingQaClaimsList(),
                    "All claims flagged for QA review for all users and all clients awaiting QA review is shown.");
                _qaClaimSearch.GetGridViewSection.IsListIconPresentInGridForClassName("qa_pass").ShouldBeFalse(
                    "Outstanding Qa claims filter option doesnot show grid result for claims with pass status as well");
                _qaClaimSearch.GetGridViewSection.IsListIconPresentInGridForClassName("qa_fail").ShouldBeFalse(
                    "Outstanding Qa claims filter options doesnot show s grid result for claims with fail status");
                _qaClaimSearch.SearchByAnalyst("Test Automation");
                _qaClaimSearch.GetGridViewSection.GetGridListValueByColAndSort(3).ShouldCollectionBeEqual(
                    _qaClaimSearch.GetQaClaimsListForGivenFilterForOutstandingStatus("Analyst",
                        automatedBase.EnvironmentManager.HciAdminUsername),
                    "All claims flagged for QA review for all Test Automation user and all clients awaiting QA review is shown.");
                _qaClaimSearch.GetGridViewSection.DoesGridListValueByColHasValue("Test Automation", 5)
                    .ShouldBeTrue("QA Claim search by analyst filters the result ");
                _qaClaimSearch.SearchByAnalyst("Test Admin");
                _qaClaimSearch.GetSideBarPanelSearch.IsNoMatchingRecordFoundMessagePresent()
                    .ShouldBeTrue("If no results are found a message will be shown.");
                _qaClaimSearch.GetSideBarPanelSearch.GetNoMatchingRecordFoundMessage()
                    .ShouldBeEqual("No matching records were found.");
                _qaClaimSearch.ClickOnClearButton();
                _qaClaimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Client",
                    ClientEnum.SMTST.ToString(), false); //type ahead functionality
                _qaClaimSearch.ClickOnClearButton();
                StringFormatter.PrintMessageTitle(
                    "Verify selecting clear button will not execute search by verifying presence of no data message is grid list view and absence of no data message in filter panel.");
                _qaClaimSearch.GetGridViewSection.IsNoDataMessagePresentInLeftSection().ShouldBeTrue(
                    "Search was not executed and no data available message is present on grid view should be true");
                _qaClaimSearch.GetSideBarPanelSearch.IsNoMatchingRecordFoundMessagePresent()
                    .ShouldBeFalse("Search was not executed and no record found message present should be false");
                _qaClaimSearch.SelectClientAndFind(ClientEnum.SMTST.ToString());

                loadMoreValue = _qaClaimSearch.GetPagination.GetLoadMoreText();
                numbers = Regex.Split(loadMoreValue, @"\D+").Where(s => s != String.Empty)
                    .Select(m => int.Parse(m.Trim())).ToList();
                count = numbers[1] % 25 == 0 ? numbers[1] / 25 - 1 : numbers[1] / 25;
                for (var i = 0; i < count; i++)
                {
                    _qaClaimSearch.GetPagination.ClickOnLoadMore();
                }

                _qaClaimSearch.GetGridViewSection.GetGridListValueByColAndSort(3).ShouldCollectionBeEqual(
                    _qaClaimSearch.GetQaClaimsListForGivenFilterForOutstandingStatus("Client",
                        ClientEnum.SMTST.ToString()),
                    "All claims flagged for QA review for all user and SMTST client and awaiting QA review is shown.");
                _qaClaimSearch.GetGridViewSection.DoesGridListValueByColHasValue(ClientEnum.SMTST.ToString())
                    .ShouldBeTrue("QA Claim search by client filters the result ");
            }
        }

        [Test] //US66651
        [Order(2)]
        public void Validation_of_filter_panel_and_search_result_for_advance_search_filter()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                var TestName = new StackFrame(true).GetMethod().Name;
                QaClaimSearchPage _qaClaimSearch = automatedBase.QuickLaunch.NavigateToQaClaimSearch();
                var approveDateRange = automatedBase.DataHelper.GetSingleTestData(GetType().FullName, TestName,
                    "ApproveDateRange", "Value");
                var reviewDateRange = automatedBase.DataHelper.GetSingleTestData(GetType().FullName, TestName,
                    "QaReviewDateRange", "Value");
                _qaClaimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                    QaClaimQuickSearchTypeEnum.AllQaClaims.GetStringValue(), false); //type ahead functionality
                _qaClaimSearch.ClickOnFindButton();
                _qaClaimSearch.ClickOnFilterOptionList("Sort By Client");
                _qaClaimSearch.GetGridViewSection.IsListIconPresentInGridForClassName("qa_pass")
                    .ShouldBeTrue("All Qa claims filter option shows grid result for claims with pass status as well");
                _qaClaimSearch.GetGridViewSection.IsListIconPresentInGridForClassName("qa_fail")
                    .ShouldBeTrue("All Qa claims filter options shows grid result for claims with fail status");
                _qaClaimSearch.IsAwaitingReviewListWithNoIconPresentInGrid()
                    .ShouldBeTrue("All Qa claims filter option shows grid result for claims with awaiting status");
                _qaClaimSearch.GetSideBarPanelSearch.SetDateField("Approve Date", approveDateRange, 1);
                _qaClaimSearch.ClickOnFindButton();
                _qaClaimSearch.GetGridViewSection.GetGridListValueByColAndSort(3).ShouldCollectionBeEqual(
                    _qaClaimSearch.GetQaClaimsListForGivenFilterForAllStatus("Approve Date", approveDateRange),
                    "All claims flagged for QA review is filtered for given approve date range.");
                _qaClaimSearch.GetGridViewSection.DoesGridListValueByColHasValue(approveDateRange, 4)
                    .ShouldBeTrue("QA Claim search by approve date filters the result ");
                _qaClaimSearch.ClickOnClearButton();
                _qaClaimSearch.GetSideBarPanelSearch.SetDateField("QA Review Date", reviewDateRange, 1);
                _qaClaimSearch.ClickOnFindButton();
                _qaClaimSearch.GetGridViewSection.GetGridListValueByColAndSort(3).ShouldCollectionBeEqual(
                    _qaClaimSearch.GetQaClaimsListForGivenFilterForAllStatus("Review Date", reviewDateRange),
                    "All claims flagged for QA review is filtered for given review date range.");
                _qaClaimSearch.GetGridViewSection.DoesGridListValueByColHasValue(reviewDateRange, 6)
                    .ShouldBeTrue("QA Claim search by approve date filters the result ");
                _qaClaimSearch.ClickOnClearButton();
                _qaClaimSearch.SelectClientAndFind(ClientEnum.SMTST.ToString()); //to get less result 
                _qaClaimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("QA Result", "QA Fail", false);
                _qaClaimSearch.ClickOnFindButton();
                _qaClaimSearch.GetGridViewSection.GetGridListValueByColAndSort(3).ShouldCollectionBeEqual(
                    _qaClaimSearch.GetQaClaimsListForGivenFilterForAllStatus("ClientAndStatus",
                        ClientEnum.SMTST.ToString(), "F"),
                    "All claims flagged for QA review is filtered for given Qa result type.");
                _qaClaimSearch.GetGridViewSection.IsListIconPresentInGridForClassName("qa_pass").ShouldBeFalse(
                    "QA result set to fail filter option shows grid result for claims with pass status should be false");
                _qaClaimSearch.GetGridViewSection.IsListIconPresentInGridForClassName("qa_fail").ShouldBeTrue(
                    "QA result set to fail filter option shows grid result for claims with pass status should be true");
                _qaClaimSearch.IsAwaitingReviewListWithNoIconPresentInGrid().ShouldBeFalse(
                    "QA result set to fail filter option shows grid result for claims with pass status should be false");
                _qaClaimSearch.ClickOnClearButton();
                _qaClaimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("QA Reviewer", "Test Automation",
                    false);
                _qaClaimSearch.ClickOnFindButton();
                _qaClaimSearch.GetGridViewSection.GetGridListValueByColAndSort(3).ShouldCollectionBeEqual(
                    _qaClaimSearch.GetQaClaimsListForGivenFilterForAllStatus("QA Reviewer",
                        automatedBase.EnvironmentManager.HciAdminUsername),
                    "All claims flagged for QA review for all Test Automation user and all clients awaiting QA review is shown.");
            }
        }

        [Test] //US66652
        [Order(10)]
        public void Verify_sort_option_and_sorted_search_results()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                QaClaimSearchPage _qaClaimSearch = automatedBase.QuickLaunch.NavigateToQaClaimSearch();
                var sortOption = automatedBase.DataHelper.GetMappingData(GetType().FullName, "QA_Claim_Search_Sorting_Options")
                    .Values.ToList();
                _qaClaimSearch.GetFilterOptionList()
                    .ShouldCollectionBeEqual(sortOption, "Verification of Sort Options");
                _qaClaimSearch.IsSortIconPresent().ShouldBeTrue("Sort Icon is Present");
                var defaultClientList = _qaClaimSearch.GetGridViewSection.GetGridListValueByCol();
                var defaultAnalystList = _qaClaimSearch.GetGridViewSection.GetGridListValueByCol(5);
                var defaultClaimSequenceList = _qaClaimSearch.GetGridViewSection.GetGridListValueByCol(3);
                ValidateListSorted(2, sortOption[0], "Client", _qaClaimSearch);
                ValidateListSorted(5, sortOption[1], "Analyst", _qaClaimSearch);
                VerifyClearSort(sortOption[2], defaultClientList, defaultAnalystList, defaultClaimSequenceList, _qaClaimSearch);
            }
        }

        [Test, Category("CommonTableDependent")] //US66653
        [Order(6)]
        public void Verify_flagging_of_claims_when_user_approves_a_claim_on_claim_action_based_on_QA_manager_settings()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(2))
            {
                var TestName = new StackFrame(true).GetMethod().Name;
                QaClaimSearchPage _qaClaimSearch = automatedBase.QuickLaunch.NavigateToQaClaimSearch();
                var claimSequences = automatedBase.DataHelper
                    .GetSingleTestData(GetType().FullName, TestName, "ClaimSequences", "Value")
                    .Split(';');
                var claimSequenceInDailyQc = automatedBase.DataHelper.GetSingleTestData(GetType().FullName,
                    TestName, "ClaimSequenceInDailyQC", "Value");
                var analyst = automatedBase.DataHelper.GetSingleTestData(GetType().FullName, TestName, "Analyst",
                    "Value");
                var date = automatedBase.DataHelper.GetSingleTestData(GetType().FullName, TestName, "Date",
                    "Value");
                try
                {
                    var qaManager = _qaClaimSearch.NavigateToAnalystManager();
                    var newBeginDate = DateTime.Now.AddDays(-1).ToString("MM/dd/yyyy");
                    var newEndDate = DateTime.Now.AddDays(2).ToString("MM/dd/yyyy");
                    qaManager.ActivateAnalystForGivenDateRangeAndTarget(newBeginDate, newEndDate, "1", "2", analyst);
                    qaManager.GetGridValueByRowCol(1, 2).Split('-')[0].Trim()
                        .ShouldBeEqual(newBeginDate, "Changes has been  applied.");
                    StringFormatter.PrintMessageTitle(
                        "Verification of Flagging of claims: should flag upto max setting, should not flag claim in daily qc");
                    foreach (var claseq in claimSequences)
                    {
                        qaManager.DeleteQaAuditRecordOfClaimApproveAndChangeClaimStatus(claseq, date);
                    }

                    qaManager.DeleteQaAuditRecordOfClaimApproveAndChangeClaimStatus(claimSequenceInDailyQc, date);
                    qaManager.ResetUserQaCounterToDefault(automatedBase.EnvironmentManager.HciAdminUsername2);

                    automatedBase.CurrentPage.RefreshPage();

                    var newClaimSearch = _qaClaimSearch.NavigateToClaimSearch();
                    ApproveClaimsInClaimActionAndReturnToNewClaimSearch(newClaimSearch, claimSequenceInDailyQc);
                    foreach (var claseq in claimSequences)
                    {
                        ApproveClaimsInClaimActionAndReturnToNewClaimSearch(newClaimSearch, claseq);
                    }

                    automatedBase.CurrentPage = _qaClaimSearch = newClaimSearch.NavigateToQaClaimSearch();
                    _qaClaimSearch.SelectOutstandingQaClaims();
                    _qaClaimSearch.SearchByAnalyst(analyst);
                    _qaClaimSearch.IsClaimSeqAlreadySelectedForDailyQc(claimSequences[0])
                        .ShouldBeFalse("Claim sequence: " + claimSequences[0] +
                                       " has not been selected for daily QC already.");
                    _qaClaimSearch.IsClaimSeqAlreadySelectedForDailyQc(claimSequences[1])
                        .ShouldBeFalse("Claim sequence: " + claimSequences[1] +
                                       " has not been selected for daily QC already.");
                    _qaClaimSearch.IsClaimSeqAlreadySelectedForDailyQc(claimSequences[2])
                        .ShouldBeFalse("Claim sequence: " + claimSequences[2] +
                                       " has not been selected for daily QC already.");
                    _qaClaimSearch.IsClaimSeqAlreadySelectedForDailyQc(claimSequenceInDailyQc)
                        .ShouldBeTrue("Claim sequence: " + claimSequenceInDailyQc +
                                      " has been selected for daily QC already.");
                    _qaClaimSearch.IsClaimSequencePresentInGridPanel(claimSequences[0])
                        .ShouldBeTrue("Record of QA flagging of claims should be present for first approved claim: " +
                                      claimSequences[0]);
                    _qaClaimSearch.IsClaimSequencePresentInGridPanel(claimSequences[1])
                        .ShouldBeTrue("Record of QA flagging of claims should be present for second approved claim: " +
                                      claimSequences[1]);
                    _qaClaimSearch.IsClaimSequencePresentInGridPanel(claimSequences[2])
                        .ShouldBeFalse(
                            "Record of QA flagging of claims should not be present for claim approved when max claim flag is crossed: " +
                            claimSequences[2]);
                    _qaClaimSearch.IsClaimSequencePresentInGridPanel(claimSequenceInDailyQc)
                        .ShouldBeFalse(
                            "Record of QA flagging of claims should not be present for claim selected for daily qc: " +
                            claimSequenceInDailyQc);
                }

                finally
                {

                }
            }
        }

        [Test] //US66654, US66656
        [Order(8)]
        public void Verify_QA_result_icons_and_status_indicators_in_New_Claim_Action()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                QaClaimSearchPage _qaClaimSearch = automatedBase.QuickLaunch.NavigateToQaClaimSearch();
                //QA Claim Search - Outstanding Claims (QA Ready)
                _qaClaimSearch.SelectQuickSearch(QaClaimQuickSearchTypeEnum.OutstandingQaClaims.GetStringValue());
                _qaClaimSearch.SelectClientAndFind(ClientEnum.SMTST.ToString());
                var firstOutstandingClaimSeq = _qaClaimSearch.GetGridViewSection.GetValueInGridByColRow(3);
                var newClaimAction =
                    _qaClaimSearch.ClickOnClaimSequenceAndNavigateToNewClaimActionPage(firstOutstandingClaimSeq);
                newClaimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                //newClaimAction.ClickWorkListIcon();
                newClaimAction.IsQaAuditIconInReviewMode().ShouldBeTrue("QA Ready icon present");
                newClaimAction.GetQaAuditIconTooltip().ShouldBeEqual(QAAuditIconEnum.QaReady.GetStringValue(),
                    "QA Ready tooltip message present");
                newClaimAction.IsQaPassIconPresent().ShouldBeTrue("QA Pass icon present");
                newClaimAction.GetQaPassIconTooltip()
                    .ShouldBeEqual(ClaimAuditActionEnum.QaPass.GetStringValue(), "QA Pass Icon Tooltip");
                newClaimAction.IsQaFailIconPresent().ShouldBeTrue("QA Fail icon present");
                newClaimAction.GetQaFailIconTooltip()
                    .ShouldBeEqual(ClaimAuditActionEnum.QaFail.GetStringValue(), "QA Fail Icon Tooltip");
                _qaClaimSearch = newClaimAction.ClickClaimSearchIconAndNavigateToQaClaimSearchPage();

                //QA Claim Search - All QA Claims - QA Pass
                _qaClaimSearch.SelectAllQaClaims();
                _qaClaimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("QA Result",
                    ClaimAuditActionEnum.QaPass.GetStringValue());
                _qaClaimSearch.SelectClientAndFind(ClientEnum.SMTST.ToString());
                var firstQAPassClaimSeq = _qaClaimSearch.GetGridViewSection.GetValueInGridByColRow(3);
                newClaimAction =
                    _qaClaimSearch.ClickOnClaimSequenceAndNavigateToNewClaimActionPage(firstQAPassClaimSeq);
                newClaimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                newClaimAction.IsQaAuditIconInLockedMode().ShouldBeTrue("QA Done icon present");
                newClaimAction.GetQaAuditIconTooltip().ShouldBeEqual(QAAuditIconEnum.QaDone.GetStringValue(),
                    "QA Done tooltip message present");
                newClaimAction.IsQaPassIconPresent().ShouldBeTrue("QA Pass icon present");
                newClaimAction.GetQaPassIconTooltip()
                    .ShouldBeEqual(ClaimAuditActionEnum.QaPass.GetStringValue(), "QA Pass Icon Tooltip");
                newClaimAction.IsQaFailIconPresent().ShouldBeTrue("QA Fail icon present");
                newClaimAction.GetQaFailIconTooltip()
                    .ShouldBeEqual(ClaimAuditActionEnum.QaFail.GetStringValue(), "QA Fail Icon Tooltip");
                _qaClaimSearch = newClaimAction.ClickClaimSearchIconAndNavigateToQaClaimSearchPage();

                //QA Claim Search - All QA Claims - QA Fail
                _qaClaimSearch.SelectAllQaClaims();
                _qaClaimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("QA Result",
                    ClaimAuditActionEnum.QaFail.GetStringValue());
                _qaClaimSearch.SelectClientAndFind(ClientEnum.SMTST.ToString());
                var firstQAFailClaimSeq = _qaClaimSearch.GetGridViewSection.GetValueInGridByColRow(3);
                newClaimAction =
                    _qaClaimSearch.ClickOnClaimSequenceAndNavigateToNewClaimActionPage(firstQAFailClaimSeq);
                newClaimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                newClaimAction.IsQaAuditIconInLockedMode().ShouldBeTrue("QA Done icon present");
                newClaimAction.GetQaAuditIconTooltip().ShouldBeEqual(QAAuditIconEnum.QaDone.GetStringValue(),
                    "QA Done tooltip message present");
                newClaimAction.IsQaPassIconPresent().ShouldBeTrue("QA Pass icon present");
                newClaimAction.GetQaPassIconTooltip()
                    .ShouldBeEqual(ClaimAuditActionEnum.QaPass.GetStringValue(), "QA Pass Icon Tooltip");
                newClaimAction.IsQaFailIconPresent().ShouldBeTrue("QA Fail icon present");
                newClaimAction.GetQaFailIconTooltip()
                    .ShouldBeEqual(ClaimAuditActionEnum.QaFail.GetStringValue(), "QA Fail Icon Tooltip");
                _qaClaimSearch = newClaimAction.ClickClaimSearchIconAndNavigateToQaClaimSearchPage();

                //User having no QA Manager Authority
                StringFormatter.PrintMessageTitle("Log in as user having no QA Manager Authority");
                var claimSearch = _qaClaimSearch.Logout().LoginAsUserHavingNoAnyAuthority()
                    .NavigateToClaimSearch();
                newClaimAction =
                    claimSearch.SearchByClaimSequenceToNavigateToClaimActionPage(firstOutstandingClaimSeq);
                newClaimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                newClaimAction.IsQaAuditIconPresent().ShouldBeFalse("QA icon present");
                newClaimAction.IsQaPassIconPresent().ShouldBeFalse("QA Pass icon present");
                newClaimAction.IsQaFailIconPresent().ShouldBeFalse("QA Fail icon present");
                _qaClaimSearch = newClaimAction.Logout().LoginAsHciAdminUser().NavigateToQaClaimSearch();
            }
        }

        [Test, Category("CommonTableDependent")]//US66657
        [Retrying(Times = 3)]
        public void Verify_Qa_Fail_and_complete_result_with_form_validation()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                var TestName = new StackFrame(true).GetMethod().Name;
                QaClaimSearchPage _qaClaimSearch = automatedBase.QuickLaunch.NavigateToQaClaimSearch();
                var analyst = automatedBase.DataHelper.GetSingleTestData(GetType().FullName, TestName, "Analyst",
                    "Value");
                var claseq = automatedBase.DataHelper.GetSingleTestData(GetType().FullName, TestName,
                    "ClaimSequence", "Value");
                var reasonCode = automatedBase.DataHelper.GetSingleTestData(GetType().FullName, TestName,
                    "ReasonCode", "Value");
                var errorDescription = automatedBase.DataHelper.GetSingleTestData(GetType().FullName, TestName,
                    "ErrorDescription", "Value");
                const string testNote = "This is a Test Note";

                Console.WriteLine("Delete Existing QA Result from database");

                _qaClaimSearch.UpdateDeleteQaClaimReviewStatusForClaim(claseq);
                _qaClaimSearch.SelectAllQaClaims();
                _qaClaimSearch.SelectClient(ClientEnum.SMTST.ToString());
                _qaClaimSearch.SearchByAnalyst(analyst.Split('(')[0].Trim());

                StringFormatter.PrintMessageTitle("Verification of QA Result Icon for Informational and others");
                SaveRecordQaErrorandNavigateToQaClaimSearchPage(claseq, _qaClaimSearch, 1, reasonCode, errorDescription, testNote);
                _qaClaimSearch.GetQaResultStatusByClaimSequence(claseq).ShouldBeEqual("Passed",
                    "QA Result should pass if Error Description is selected to <Informational>");
                SaveRecordQaErrorandNavigateToQaClaimSearchPage(claseq, _qaClaimSearch, 2, reasonCode, "");
                _qaClaimSearch.GetQaResultStatusByClaimSequence(claseq).ShouldBeEqual("Failed",
                    "QA Result should fail if one line has Error Description other than <Informational>");

                StringFormatter.PrintMessageTitle("Verificatio of QA Claim Line");
                var newClaimAction = _qaClaimSearch.ClickOnClaimSequenceAndNavigateToNewClaimActionPage(claseq);
                newClaimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                //newClaimAction.ClickWorkListIcon();
                VerificationOfClaimProcessingHistoryDetails(newClaimAction, analyst);
                newClaimAction.DeleteClaimAuditRecordExceptAddAndApprove(claseq);
                if (!newClaimAction.IsDeleteIconPresentOnFlaggedLinesRowsByRow())
                    newClaimAction.ClickOnRestoreIconOnFlaggedLinesRowsByRow(claseq);
                if (!newClaimAction.IsDeleteIconPresentOnFlaggedLinesRowsByRow(2))
                    newClaimAction.ClickOnRestoreIconOnFlaggedLinesRowsByRow(claseq, 2);

                var topFlag1 = newClaimAction.GetTopFlagByRow();
                var topFlag2 = newClaimAction.GetTopFlagByRow(2);
                var procCode = newClaimAction.GetProcCodeOnClaimLine();
                var procDescription = newClaimAction.GetProcDescriptionOnClaimLine();
                newClaimAction.ClickClaimActionQaFail();
                newClaimAction.GetClaimQaRowValueByCol().ShouldBeEqual("1", "Verification of Line No");
                newClaimAction.IsEditIconPresentInClaimQaByRow().ShouldBeTrue("Verification of Pencil Icon");
                newClaimAction.IsNoteIconPresentInClaimQaByRow().ShouldBeTrue("Verification of Note Icon");
                newClaimAction.GetClaimQaRowValueByCol(4).ShouldBeEqual(procCode, "Verification of Line No");
                newClaimAction.GetClaimQaRowValueByCol(5).ShouldBeEqual(procDescription, "Verification of Line No");
                newClaimAction.GetClaimQaRowValueByCol(3)
                    .ShouldBeEqual(topFlag1, "Top Flag should equal to claim lines top flag");
                newClaimAction.GetClaimQaRowValueByCol(3, 2)
                    .ShouldBeEqual(topFlag2, "Top Flag should equal to claim lines top flag");

                StringFormatter.PrintMessageTitle(
                    "Verification of Flag Icon in QA Claim List for delete and restore flag");
                newClaimAction.ClickOnDeleteIconOnFlaggedLinesRowsByRow(claseq, 2, true);
                newClaimAction.GetClaimQaRowValueByCol(3, 2)
                    .ShouldBeEqual("", "Top Flag should should empty when all flag delete on specific flag lines",
                        true);
                newClaimAction.ClickOnRestoreIconOnFlaggedLinesRowsByRow(claseq, 2);

                newClaimAction.GetClaimQaRowValueListByCol().IsInAscendingOrder()
                    .ShouldBeTrue("Is Claim Qa Rows are in Ascending Order by Line Number");

                StringFormatter.PrintMessageTitle("Verification of Reason Code List, Type ahead functionality");
                var expectedReasonCodeListList = newClaimAction.GetExpectedQaErrorReasonCodeList();
                newClaimAction.ClickonOnEditOnClaimQaByRow();
                newClaimAction.GetRecordQaErrorDropdownList("Reason Code")
                    .ShouldCollectionBeEqual(expectedReasonCodeListList,
                        "Reason Code should equal and sorted to the database value");
                newClaimAction.IsTypeAheadForRecordQaErrorDropdown("Reason Code", reasonCode)
                    .ShouldBeTrue("Verification of Type Ahead functionality");

                StringFormatter.PrintMessageTitle(
                    "Verification of Error Description List which is static predefined list");
                var expectedErrorDescriptionList = newClaimAction.GetExpectedErrorDescriptionList();
                newClaimAction.GetRecordQaErrorDropdownList("Error Description")
                    .ShouldCollectionBeEqual(expectedErrorDescriptionList,
                        "Verification of Error Description List");

                StringFormatter.PrintMessageTitle("Verification of Empty Reason Code and validation popoup message");
                newClaimAction.SelectRecordQaErrorDropdown("Reason Code", "");
                newClaimAction.ClickSaveRecordQAError();
                newClaimAction.IsPageErrorPopupModalPresent()
                    .ShouldBeTrue("Validation popup should exist when trying to save without Reason Code");
                newClaimAction.GetPageErrorMessage()
                    .ShouldBeEqual("Some values are missing. Please complete the required fields.",
                        "Verificaiton of Popup Messagae");
                newClaimAction.ClosePageError();

                StringFormatter.PrintMessageTitle("Verification of Overrides Note for Maximum character");
                var note = new StringBuilder(2500).Insert(0, "This is  test note. ", 125).ToString();
                newClaimAction.SetOverrideNoteInNoteByRow(note + 'a');
                newClaimAction.GetOverrideNoteInNoteByRow().ShouldBeEqual(note, "Verification of Maximum character");

                StringFormatter.PrintMessageTitle(
                    "Verification of Each Cancel button of Claim QA rows and verify changes should discard");
                newClaimAction.ScrollToQaClaimListByRow(2);
                newClaimAction.ClickonOnEditOnClaimQaByRow(2);
                newClaimAction.ClickCancelButtonOnRecordQaError();
                newClaimAction.ClickCancelButtonOnRecordQaError(2);
                newClaimAction.IsRecordQaErrorSectionPresent().ShouldBeFalse("Record QA Error Form should close");
                newClaimAction.IsRecordQaErrorSectionPresent(2)
                    .ShouldBeFalse("Second Record QA Error Form should close");
                newClaimAction.ClickonOnEditOnClaimQaByRow();
                VerifyQaErrorRecordForm(newClaimAction, reasonCode, errorDescription, testNote,
                    "Reason Code should discard", "Error Description should discard",
                    "Override Notes should discard");


                StringFormatter.PrintMessageTitle(
                    "Verification of Form Cancel link and behaviour to Record QA Error form");
                Select_RecordQaErrorDescription(newClaimAction, expectedReasonCodeListList[3],
                    expectedErrorDescriptionList[3], "This is another note");
                newClaimAction.ClickClaimQaCancelLink();
                newClaimAction.IsPageErrorPopupModalPresent()
                    .ShouldBeTrue("Is Confirmation of Discard changes popup appears");
                newClaimAction.GetPageErrorMessage()
                    .ShouldBeEqual("Unsaved changes will be discarded. Do you wish to proceed?",
                        "Verification of Confirmation Message");
                newClaimAction.ClickOkCancelOnConfirmationModal(false);

                VerifyQaErrorRecordForm(newClaimAction, expectedReasonCodeListList[3], expectedErrorDescriptionList[3],
                    "This is another note",
                    "Reason Code should retain", "Error Description should retain",
                    "Override Notes should retain");

                newClaimAction.ClickClaimQaCancelLink();
                newClaimAction.ClickOkCancelOnConfirmationModal(true);

                newClaimAction.IsRecordQaErrorSectionPresent().ShouldBeFalse("Record QA Error Form should close");
                newClaimAction.ClickonOnEditOnClaimQaByRow();
                VerifyQaErrorRecordForm(newClaimAction, reasonCode, errorDescription, testNote,
                    "Reason Code should discard", "Error Description should discard",
                    "Override Notes should discard");
                newClaimAction.ClickClaimQaCancelLink();
                newClaimAction.ClickOkCancelOnConfirmationModal(true);

                StringFormatter.PrintMessageTitle(
                    "Verify that other user can view QA Fail notes created by other another user");
                newClaimAction.Logout().LoginAsHciAdminUser1().NavigateToClaimSearch()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(claseq);
                //newClaimAction.ClickWorkListIcon();
                newClaimAction.ClickClaimActionQaFail();
                newClaimAction.ClickonOnEditOnClaimQaByRow();
                VerifyQaErrorRecordForm(newClaimAction, reasonCode, errorDescription, testNote,
                    "Other User can view Reason Code", "Other User can view Error Description",
                    "Other User can view Overrides note");
            }
        }

        [Test, Category("CommonTableDependent")] //US66658 + CAR-3177 (CAR-3189)
        [Order(9)]
        public void Validate_user_is_able_to_advance_through_QA_claim_worklist_for_review()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                var TestName = new StackFrame(true).GetMethod().Name;
                QaClaimSearchPage _qaClaimSearch = automatedBase.QuickLaunch.NavigateToQaClaimSearch();
                var claimSequences = automatedBase.DataHelper
                    .GetSingleTestData(GetType().FullName, TestName, "ClaimSequences", "Value")
                    .Split(';');
                var analyst = automatedBase.DataHelper.GetSingleTestData(GetType().FullName, TestName, "Analyst",
                    "Value");
                var reasonCode = automatedBase.DataHelper.GetSingleTestData(GetType().FullName, TestName,
                    "ReasonCode", "Value");
                var errorDescription = automatedBase.DataHelper.GetSingleTestData(GetType().FullName, TestName,
                    "ErrorDescription", "Value");

                Random rnd = new Random();

                List<string> qaActions = new List<string>()
                {
                    "Pass",
                    "Fail"
                };
                StringFormatter.PrintMessage("Reverting the test data");
                foreach (var claseq in claimSequences)
                {
                    _qaClaimSearch.UpdateDeleteQaClaimReviewStatusForClaim(claseq);
                    _qaClaimSearch.DeleteClaimLock(claseq);
                }

                StringFormatter.PrintMessageTitle("Performing QA Actions on the claims to verify that the user is navigated to the subsequent claims");
                _qaClaimSearch.SelectOutstandingQaClaims();
                _qaClaimSearch.SelectSmtstClient();
                _qaClaimSearch.SearchByAnalyst(analyst);
                var listOfClaimsInGrid = _qaClaimSearch.GetGridViewSection.GetGridListValueByCol(3);
                var newClaimAction = _qaClaimSearch.ClickOnClaimSequenceAndNavigateToNewClaimActionPage(listOfClaimsInGrid[0]);
                newClaimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();

                newClaimAction.ClickClaimQaPassAndWaitForNextClaim();
                listOfClaimsInGrid[1].ShouldBeEqual(newClaimAction.GetClaimSequence(),
                    "After reviewing claim, page should automatically navigates to next claim in the list");

                var randomQaAction = qaActions[rnd.Next(qaActions.Count)];

                if (randomQaAction == "Fail")
                {
                    ClickClaimActionQaFail_ClickOnEditOnClaimQaAndSelect_RecordQaErrorDescription(newClaimAction, reasonCode, errorDescription);
                    newClaimAction.SaveRecordQAErrorAndNavigateToNextClaim();
                }
                else
                    newClaimAction.ClickClaimQaPassAndWaitForNextClaim();

                listOfClaimsInGrid[2].ShouldBeEqual(newClaimAction.GetClaimSequence(),
                    $"After reviewing claim and setting QA {randomQaAction}, page should automatically navigates to next claim in the list");
                _qaClaimSearch = newClaimAction.ClickClaimQaPassAndNavigateToQaClaimSearchPage();
                _qaClaimSearch.GetPageHeader().ShouldBeEqual(PageHeaderEnum.QaClaimSearch.GetStringValue(),
                    "User should taken back to QA Claim search after last claim of list is approved");

                StringFormatter.PrintMessageTitle("Verifying the QA reviewed and locked claims are skipped");
                _qaClaimSearch.SelectAllQaClaims();
                _qaClaimSearch.ClickOnFindButton();

                var claseqToBeAltered = listOfClaimsInGrid[1];

                foreach (var claseq in claimSequences)
                {
                    if (claseq != claseqToBeAltered)
                        _qaClaimSearch.UpdateDeleteQaClaimReviewStatusForClaim(claseq);
                }

                _qaClaimSearch.ClickOnFindButton();

                StringFormatter.PrintMessage("Verifying that already QA reviewed claims are skipped when the user performs QA actions on a claim");
                _qaClaimSearch.ClickOnClaimSequenceAndNavigateToNewClaimActionPage(listOfClaimsInGrid[0]);
                newClaimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                newClaimAction.ClickClaimQaPassAndWaitForNextClaim();

                listOfClaimsInGrid[2].ShouldBeEqual(newClaimAction.GetClaimSequence(),
                    "After reviewing claim, page should automatically navigates to next claim in the list and skip the claim which is already reviewed");

                newClaimAction.ClickClaimSearchIconAndNavigateToQaClaimSearchPage();
                var claimQaStatus = _qaClaimSearch.GetQaResultStatusByClaimSequence(listOfClaimsInGrid[0]);
                claimQaStatus.ShouldBeEqual("Passed",
                    $"Claim QA status should be correctly changed to {claimQaStatus}");

                StringFormatter.PrintMessage("Reverting the test data");
                foreach (var claseq in claimSequences)
                {
                    _qaClaimSearch.UpdateDeleteQaClaimReviewStatusForClaim(claseq);
                }

                newClaimAction.GetCommonSql.InsertLockForClaimByClaimSeqAndUserId(listOfClaimsInGrid[1],
                    automatedBase.EnvironmentManager.HciAdminUsername1);

                _qaClaimSearch.ClickOnFindButton();

                StringFormatter.PrintMessage("Verifying that locked claims are skipped when the user performs QA actions on a claim");
                _qaClaimSearch.ClickOnClaimSequenceAndNavigateToNewClaimActionPage(listOfClaimsInGrid[0]);
                newClaimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                newClaimAction.ClickClaimQaPassAndWaitForNextClaim();

                listOfClaimsInGrid[2].ShouldBeEqual(newClaimAction.GetClaimSequence(),
                    "After reviewing claim, page should automatically navigates to next claim in the list and skip the claim which is locked");
            }
        }

        [Test] //CAR-728
        [Order(3)]
        public void Verify_find_button_is_disabled_when_search_is_active()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                QaClaimSearchPage _qaClaimSearch = automatedBase.QuickLaunch.NavigateToQaClaimSearch();
                _qaClaimSearch.GetSideBarPanelSearch.OpenSidebarPanel();
                _qaClaimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search", "All QA Claims");
                _qaClaimSearch.ClickFindAndCheckIfFindButtonIsDisabled()
                    .ShouldBeTrue("Find Button Should be disabled while the search is active.");
                _qaClaimSearch.WaitForWorkingAjaxMessage();
                _qaClaimSearch.GetGridViewSection.GetGridRowCount()
                    .ShouldBeGreater(0, "Search results should be displayed");
                _qaClaimSearch.CheckIfFindButtonIsEnabled()
                    .ShouldBeTrue("Find Button Should be enabled once the search is complete.");
            }
        }

        [Test] // TE-803
        [NonParallelizable]
        public void Verify_when_user_deletes_all_flags_on_QA_Ready_Claims_alert_is_not_shown_and_users_are_allowed_to_leave_claim_action_page()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                var TestName = new StackFrame(true).GetMethod().Name;
                QaClaimSearchPage _qaClaimSearch = automatedBase.QuickLaunch.NavigateToQaClaimSearch();
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(GetType().FullName, TestName);
                var analyst = paramLists["Analyst"];
                ClaimActionPage newClaimAction = null;
                try
                {
                    StringFormatter.PrintMessage("Verification for Claims with QA Ready State");
                    newClaimAction = SearchByAnalystAndClientToNavigateToNewClaimActionPage(analyst, _qaClaimSearch);
                    //If Restore All Flag icon is present instead, click on it to change to get Delete All.
                    if (newClaimAction.IsRestoreButtonPresent())
                        newClaimAction.ClickOnRestoreAllFlagsIcon();
                    newClaimAction.ClickOnDeleteAllFlagsIcon();
                    var claSeq = newClaimAction.GetCurrentClaimSequence();

                    //Navigate to dashboard
                    newClaimAction.ClickOnlyDashboardIcon();
                    VerifyPageHeaderAndErrorMessage(PageHeaderEnum.Dashboard.GetStringValue(), _qaClaimSearch);
                    newClaimAction.IsClaimLockedByClaimSequence(ClientEnum.SMTST.ToString(), claSeq.Split('-')[0],
                        claSeq.Split('-')[1],
                        automatedBase.EnvironmentManager.HciAdminUsername3).ShouldBeFalse($"Is claim {claSeq} locked?");

                    automatedBase.CurrentPage.NavigateToQaClaimSearch();
                    SearchByAnalystAndClientToNavigateToNewClaimActionPage(analyst, _qaClaimSearch);

                    //Navigate to Appeal Search
                    newClaimAction.NavigateToAppealSearch();
                    VerifyPageHeaderAndErrorMessage(PageHeaderEnum.AppealSearch.GetStringValue(), _qaClaimSearch);
                    newClaimAction.IsClaimLockedByClaimSequence(ClientEnum.SMTST.ToString(), claSeq.Split('-')[0],
                        claSeq.Split('-')[1],
                        automatedBase.EnvironmentManager.HciAdminUsername3).ShouldBeFalse($"Is claim {claSeq} locked?");

                    automatedBase.CurrentPage.NavigateToQaClaimSearch();
                    SearchByAnalystAndClientToNavigateToNewClaimActionPage(analyst, _qaClaimSearch);

                    //Navigate to claim search using search icon
                    newClaimAction.ClickClaimSearchIcon();
                    VerifyPageHeaderAndErrorMessage(PageHeaderEnum.QaClaimSearch.GetStringValue(), _qaClaimSearch);
                    newClaimAction.IsClaimLockedByClaimSequence(ClientEnum.SMTST.ToString(), claSeq.Split('-')[0],
                        claSeq.Split('-')[1],
                        automatedBase.EnvironmentManager.HciAdminUsername3).ShouldBeFalse($"Is claim {claSeq} locked?");
                }

                finally
                {
                    if (automatedBase.CurrentPage.GetPageHeader() != PageHeaderEnum.ClaimAction.GetStringValue())
                    {
                        SearchByAnalystAndClientToNavigateToNewClaimActionPage(analyst, _qaClaimSearch);
                    }

                    if (newClaimAction.IsRestoreButtonPresent())
                        newClaimAction.ClickOnRestoreAllFlagsIcon();

                }
            }
        }

        [Test] //TE-833
        [Order(4)]
        [NonParallelizable]
        public void Verify_Claim_Lock_Is_Released_When_User_Navigates_To_Next_Claim_Using_QAPass()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                var TestName = new StackFrame(true).GetMethod().Name;
                QaClaimSearchPage _qaClaimSearch = automatedBase.QuickLaunch.NavigateToQaClaimSearch();
                var claseq = automatedBase.DataHelper.GetSingleTestData(GetType().FullName, TestName, "claseq",
                    "Value");
                var sortOption = automatedBase.DataHelper.GetMappingData(GetType().FullName, "QA_Claim_Search_Sorting_Options")
                    .Values.ToList();

                _qaClaimSearch.UpdateQAClaimStatus(claseq);
                try
                {
                    _qaClaimSearch.Logout().LoginAsHciAdminUser5();
                    automatedBase.CurrentPage = _qaClaimSearch = automatedBase.CurrentPage.NavigateToQaClaimSearch();
                    _qaClaimSearch.SelectOutstandingQaClaims();
                    _qaClaimSearch.SelectSmtstClient();
                    _qaClaimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Restrictions","No Restriction");
                    _qaClaimSearch.ClickOnFindButton();
                    _qaClaimSearch.WaitForWorking();
                    _qaClaimSearch.ClickOnFilterOptionList(sortOption[1]);
                    var newClaimAction = _qaClaimSearch.ClickOnClaimSequenceAndNavigateToNewClaimActionPage(claseq);
                    newClaimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                    newClaimAction.IsClaimLocked(ClientEnum.SMTST.ToString(), claseq.Split('-')[0],
                        claseq.Split('-')[1],
                        automatedBase.EnvironmentManager.HciAdminUsername5).ShouldBeTrue($"Is claim {claseq} locked?");
                    newClaimAction.ClickClaimActionQaPass();
                    newClaimAction.WaitForWorkingAjaxMessage();
                    newClaimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                    newClaimAction.GetClaimSequence()
                        .ShouldNotBeEqual(claseq, "Navigation successful to next qa claim");
                    newClaimAction.IsClaimLocked(ClientEnum.SMTST.ToString(), claseq.Split('-')[0],
                        claseq.Split('-')[1],
                        automatedBase.EnvironmentManager.HciAdminUsername5).ShouldBeFalse($"Is claim {claseq} locked?");
                    _qaClaimSearch = newClaimAction.ClickClaimSearchIconAndNavigateToQaClaimSearchPage();
                }
                finally
                {

                }
            }
        }

        [Test,]//TE-834
        
        public void Verify_QA_Claim_Pass_Fail_Icons_Are_Disabled_When_Claim_Is_Locked_By_User()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                var TestName = new StackFrame(true).GetMethod().Name;
                QaClaimSearchPage _qaClaimSearch = automatedBase.QuickLaunch.NavigateToQaClaimSearch();
                var claimSeq = automatedBase.DataHelper.GetSingleTestData(GetType().FullName, TestName,
                    "ClaimSequence", "Value");
                var claimSeqWithAppealLock = automatedBase.DataHelper.GetSingleTestData(GetType().FullName,
                    TestName,
                    "ClaimSeqWithAppealLock", "Value");
                _qaClaimSearch.DeleteClaimLock(claimSeq);
                try
                {
                    StringFormatter.PrintMessage("Verify Qa Pass/Fail Icon Is Disabled When Locked By Another User");
                    _qaClaimSearch.Logout().LoginAsHciAdminUser5();
                    automatedBase.CurrentPage = _qaClaimSearch = automatedBase.CurrentPage.NavigateToQaClaimSearch();
                    _qaClaimSearch.SelectOutstandingQaClaims();
                    _qaClaimSearch.SelectSmtstClient();
                    _qaClaimSearch.ClickOnFindButton();
                    var newClaimAction = _qaClaimSearch.ClickOnClaimSequenceAndNavigateToNewClaimActionPage(claimSeq);
                    newClaimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                    automatedBase.CurrentPage = newClaimAction;
                    StringFormatter.PrintMessage(
                        string.Format("Open claim aciton page for claim sequence: {0} with direct URL ", claimSeq));
                    var newClaimActionUrl = automatedBase.CurrentPage.CurrentPageUrl;
                    newClaimAction =
                        _qaClaimSearch.SwitchToOpenNewClaimActionByUrl(newClaimActionUrl,
                            automatedBase.EnvironmentManager.HciAdminUsername);
                    newClaimAction.IsClaimLocked()
                        .ShouldBeTrue("Claim should be locked when it is in view mode by another user");
                    newClaimAction.GetLockIConTooltip()
                        .ShouldBeEqual(
                            "This claim has been opened in view mode. It is currently locked by Test Automation5 (ui_automation5).",
                            "Is Lock Message Equal?");
                    newClaimAction.IsQaPassFailIconDisabled()
                        .ShouldBeTrue("Qa Pass/Fail Icon Should Be Disabled When Locked By Other User");

                    StringFormatter.PrintMessage(
                        "Verify Qa Pass/Fail Icon Is Enabled When Appeal Lock Is Present For The Claim");
                    automatedBase.CurrentPage = _qaClaimSearch = automatedBase.CurrentPage.NavigateToQaClaimSearch();
                    //newClaimAction.ClickClaimSearchIcon();
                    _qaClaimSearch.SelectOutstandingQaClaims();
                    _qaClaimSearch.SelectSmtstClient();
                    _qaClaimSearch.ClickOnFindButton();
                    newClaimAction =
                        _qaClaimSearch.ClickOnClaimSequenceAndNavigateToNewClaimActionPage(claimSeqWithAppealLock);
                    newClaimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                    newClaimAction.IsClaimLocked()
                        .ShouldBeTrue("Claim should be locked when appeal is present for the claim");
                    newClaimAction.GetLockIConTooltip()
                        .ShouldBeEqual(
                            "Claim is locked. You cannot modify claims linked to an appeal. Adjustments must be made using Appeal Action.",
                            "Is Lock Message Equal?");
                    newClaimAction.IsQaPassFailIconDisabled()
                        .ShouldBeFalse("Qa Pass/Fail icon should be enabled when claim is locked for appeal");



                }
                finally
                {

                    _qaClaimSearch.DeleteClaimLock(claimSeq);
                }
            }
        }

        [Test, Category("Acceptance")]//TANT-1198
        public void Verify_No_Ui_Distortion_In_Claim_Search()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                QaClaimSearchPage _qaClaimSearch = automatedBase.QuickLaunch.NavigateToQaClaimSearch();
                _qaClaimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Client", ClientEnum.SMTST.ToString());
                _qaClaimSearch.GetSideBarPanelSearch.ClickOnFindButton();
                var claseq = _qaClaimSearch.GetGridViewSection.GetGridListValueByCol(3);
                ClaimActionPage claimAction = _qaClaimSearch.ClickOnClaimSequenceAndNavigateToNewClaimActionPage(claseq[0]);
                claimAction.CloseAllExistingPopupIfExist();
                ClaimSearchPage claimSearch = claimAction.NavigateToClaimSearch(true);
                if (!claimSearch.GetSideBarPanelSearch.IsSideBarPanelOpen())
                    claimSearch.GetSideBarPanelSearch.OpenSidebarPanel();
                claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",ClaimQuickSearchTypeEnum.FlaggedClaims.GetStringValue());
                claimSearch.GetSideBarPanelSearch.ClickOnFindButton();
                claimSearch.WaitForWorking();
                claimSearch.GetGridViewSection.GetGridListValueByCol(3).ShouldCollectionBeNotEmpty("flag information available?");
            }
        }

        [Test] //CAR-2993(CAR-2980)
        [Retrying(Times = 3)]
        [Order(5)]
        public void Verify_navigation_and_security_of_QaClaimSearch()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                QaClaimSearchPage _qaClaimSearch = automatedBase.QuickLaunch.NavigateToQaClaimSearch();
                CommonValidations _commonValidations = new CommonValidations(automatedBase.CurrentPage);
                _commonValidations.ValidateSecurityAndNavigationOfAPage(HeaderMenu.Qa,
                    new List<string> {SubMenu.QaClaimSearch, SubMenu.QaAppealSearch},
                    RoleEnum.QAAnalyst.GetStringValue(),
                    new List<string>
                        {PageHeaderEnum.QaClaimSearch.GetStringValue(), PageHeaderEnum.QaAppealSearch.GetStringValue()},
                    automatedBase.Login.LoginAsUserHavingNoAnyAuthority, new[] {"Test4", "Automation4"});
                _qaClaimSearch.Logout().LoginAsClientUser();
                _qaClaimSearch.IsQaClaimSearchSubMenuPresent()
                    .ShouldBeFalse("Qa Claim Search submenu present for client user.");
                _qaClaimSearch.IsQaAppealSearchSubMenuPresent()
                    .ShouldBeFalse("Qa Appeal Search submenu present for client user.");
            }
        }


        #endregion


        #region Private functions

        private ClaimActionPage SearchByAnalystAndClientToNavigateToNewClaimActionPage(string analyst, QaClaimSearchPage _qaClaimSearch)
        {
            _qaClaimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Analyst", analyst);
            _qaClaimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Client", ClientEnum.SMTST.ToString());
            _qaClaimSearch.GetSideBarPanelSearch.ClickOnFindButton();
            _qaClaimSearch.WaitForWorkingAjaxMessage();
            var claseq = _qaClaimSearch.GetGridViewSection.GetValueInGridByColRow(3);
            var newClaimAction = _qaClaimSearch.ClickOnClaimSequenceAndNavigateToNewClaimActionPage(claseq);
            newClaimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
            return newClaimAction;

        }
        private void VerifyPageHeaderAndErrorMessage(string pageHeader, NewDefaultPage CurrentPage)
        {
            CurrentPage.IsAlertBoxPresent().ShouldBeFalse("Popup should not be displayed notifying confirmation for leaving.");
            CurrentPage.GetPageHeader().ShouldBeEqual(pageHeader, $"User should be navigated to {pageHeader} Page");
        }

        private static void VerifyQaErrorRecordForm(ClaimActionPage newClaimAction,string reasonCode,string errorDescription,string note,string reasonCodeMessage,string errorDescriptionMessasge,string noteMessage)
        {
            newClaimAction.GetSelectedRecordQaErrorDropDownValue("Reason Code").ShouldBeEqual(reasonCode, reasonCodeMessage);
            newClaimAction.GetSelectedRecordQaErrorDropDownValue("Error Description")
                .ShouldBeEqual(errorDescription, errorDescriptionMessasge);
            newClaimAction.GetOverrideNoteInNoteByRow().ShouldBeEqual(note,
                noteMessage);
        }

        private void ClickClaimActionQaFail_ClickOnEditOnClaimQaAndSelect_RecordQaErrorDescription(ClaimActionPage newClaimAction, string reasonCode, string errorDescription)
        {
            newClaimAction.ClickClaimActionQaFail();
            ClickOnEditOnClaimQaAndSelect_RecordQaErrorDescription(newClaimAction, reasonCode, errorDescription);
        }
        private void ClickOnEditOnClaimQaAndSelect_RecordQaErrorDescription(ClaimActionPage newClaimAction, string reasonCode, string errorDescription)
        {
            newClaimAction.ClickonOnEditOnClaimQaByRow();
            Select_RecordQaErrorDescription(newClaimAction, reasonCode, errorDescription);
        }
        private void Select_RecordQaErrorDescription(ClaimActionPage newClaimAction, string reasonCode, string errorDescription,string note="")
        {
            newClaimAction.SelectRecordQaErrorDropdown("Reason Code", reasonCode);
            newClaimAction.SelectRecordQaErrorDropdown("Error Description", errorDescription);
            if (note != "")
                newClaimAction.SetOverrideNoteInNoteByRow(note);
        }
        private void VerificationOfClaimProcessingHistoryDetails(ClaimActionPage newClaimAction,string analyst)
        {
            var claimProcessingHistory = newClaimAction.ClickOnClaimProcessingHistoryAndSwitch();
            var dateTime = claimProcessingHistory.GetClaimHistoryTableDataValueByRowCol(2, 1);
            Convert.ToDateTime(dateTime).Date.ShouldBeEqual(DateTime.Now.Date, "Is Reviewed Date equal?");
            claimProcessingHistory.GetClaimHistoryTableDataValueByRowCol(2, 2)
                .ShouldBeEqual(ClaimAuditActionEnum.QaFail.GetStringValue(), "Verify Action");
            claimProcessingHistory.GetClaimHistoryTableDataValueByRowCol(2, 4)
                .ShouldBeEqual(analyst, "Verification of QA Reviewer should be in proper format");
            claimProcessingHistory.GetClaimHistoryTableDataValueByRowCol(2, 5)
                .ShouldBeEqual("Internal", "User Type should Internal.");
            claimProcessingHistory.GetClaimHistoryTableDataValueByRowCol(2, 6)
                .ShouldBeEqual("Unreviewed", "Status should equal to current claim status.");
            claimProcessingHistory.CloseClaimProcessingHistoryPageAndSwitchToClaimActionPage();
        }

        private void SaveRecordQaErrorandNavigateToQaClaimSearchPage(string claseq, QaClaimSearchPage _qaClaimSearch,int row = 1, string reasonCode = "QUA 5 - QA Error", string errorDescription = "Should have left edit",string note="")
        {
            var isCurrentClaimIsLastClaim = _qaClaimSearch.IsCurrentClaimSequenceLastClaimSequence(claseq);
            var newClaimAction = _qaClaimSearch.ClickOnClaimSequenceAndNavigateToNewClaimActionPage(claseq);
            newClaimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
            //newClaimAction.ClickWorkListIcon();
            newClaimAction.ClickClaimActionQaFail();
            newClaimAction.ClickonOnEditOnClaimQaByRow(row);
            newClaimAction.SelectRecordQaErrorDropdown("Reason Code",reasonCode);
            newClaimAction.SelectRecordQaErrorDropdown("Error Description", errorDescription);
            if(note!="")
                newClaimAction.SetOverrideNoteInNoteByRow(note);
            if (isCurrentClaimIsLastClaim)
                newClaimAction.SaveRecordQaErrorAndNavigateToQaClaimSearchPage();
            else
            {
                newClaimAction.SaveRecordQAErrorAndNavigateToNextClaim();
                _qaClaimSearch = newClaimAction.ClickClaimSearchIconAndNavigateToQaClaimSearchPage();
            }
            if (note == "")
                newClaimAction.IsPageErrorPopupModalPresent().ShouldBeFalse("Popup Should not display if note is empty");
            if (errorDescription == "")
                newClaimAction.IsPageErrorPopupModalPresent().ShouldBeFalse("Popup Should not display if Error Description is empty");

        }

        private void ClosePatientClaimHisotryAndClickClaimActionQaPass(ClaimActionPage newClaimAction, bool value)
        {
            newClaimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
            //newClaimAction.ClickWorkListIcon();
            newClaimAction.ClickClaimActionQaPass();
            newClaimAction.ClickOkCancelOnConfirmationModal(value);
        }
        
        private void VerifyClearSort(string sortOption, IList<string> defaultClientList, IList<string> defaultAnalystList, IList<string> defaultClaimSequenceList, QaClaimSearchPage _qaClaimSearch)
        {            
            _qaClaimSearch.ClickOnFilterOptionList(sortOption);
            _qaClaimSearch.GetGridViewSection.GetGridListValueByCol()
                .ShouldCollectionBeEqual(defaultClientList, "Client are in default order.");
            _qaClaimSearch.GetGridViewSection.GetGridListValueByCol(5)
                .ShouldCollectionBeEqual(defaultAnalystList, "Analyst Users are in default order.");
            _qaClaimSearch.GetGridViewSection.GetGridListValueByCol(3)
                .ShouldCollectionBeEqual(defaultClaimSequenceList, "Claim Sequences are in default order.");
        }

        private void ValidateListSorted(int col, string sortOption, string colName, QaClaimSearchPage _qaClaimSearch)
        {
            _qaClaimSearch.ClickOnFilterOptionList(sortOption);
            var list = _qaClaimSearch.GetGridViewSection.GetGridListValueByCol(col);
            _qaClaimSearch.IsListInAscendingOrder(list)
                .ShouldBeTrue(string.Format("<{0}>  are in ascending order", colName));
            _qaClaimSearch.ClickOnFilterOptionList(sortOption);
            list = _qaClaimSearch.GetGridViewSection.GetGridListValueByCol(col);
            _qaClaimSearch.IsListInDescendingOrder(list)
                .ShouldBeTrue(string.Format("<{0}>  are in descending order", colName)); ;
        }

        //private void ValidateClientListSorted(int col, string sortOption)
        //{
        //    _qaClaimSearch.ClickOnFilterOptionList(sortOption);
        //    List<string> clientList = _qaClaimSearch.GetGridViewSection.GetGridListValueByCol(col);
        //    Assert.IsTrue(_qaClaimSearch.IsListInAscendingOrder(clientList), "Client List are in ascending order");
        //    _qaClaimSearch.ClickOnFilterOptionList(sortOption);
        //    clientList = _qaClaimSearch.GetGridViewSection.GetGridListValueByCol(col);
        //    Assert.IsTrue(_qaClaimSearch.IsListInDescendingOrder(clientList), "Client List are in descending order");
        //}

        private void ValidateDropDownFoDefaultValueAndExpectedList(string label, IList<string> collectionToEqual, QaClaimSearchPage _qaClaimSearch, bool qAResult=false)
        {
            var actualDropDownList = _qaClaimSearch.GetSideBarPanelSearch.GetAvailableDropDownList(label);
            actualDropDownList.Count.ShouldBeGreater(0, "List of " + label + " is greater than zero.");
            if (collectionToEqual != null)
            {
                actualDropDownList.RemoveAll(item => item == "");
                actualDropDownList.ShouldCollectionBeEqual(collectionToEqual, label + " List is As Expected");
            }
            if (!qAResult) actualDropDownList.IsInAscendingOrder().ShouldBeTrue(label + " should be sorted in alphabetical order.");
            _qaClaimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel(label, actualDropDownList[0],false); //check for type ahead functionality
            _qaClaimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel(label, actualDropDownList[1]);
            _qaClaimSearch.GetSideBarPanelSearch.GetInputValueByLabel(label).ShouldBeEqual(actualDropDownList[1], "User can select only a single option");
        }

        private void ValidateForPopUpMessage(string popUpMessgae, string expectedError, QaClaimSearchPage _qaClaimSearch)
        {
            _qaClaimSearch.WaitToLoadPageErrorPopupModal();
            _qaClaimSearch.IsPageErrorPopupModalPresent()
                .ShouldBeTrue(popUpMessgae);
            _qaClaimSearch.GetPageErrorMessage()
                .ShouldBeEqual(expectedError,
                    popUpMessgae);
            _qaClaimSearch.ClosePageError();
        }

        private void ValidateQaClaimSearchDateFields(string fieldName, QaClaimSearchPage _qaClaimSearch)
        {
            _qaClaimSearch.GetDateRangeInputFieldsCount(fieldName)
                .ShouldBeEqual(2, "A beginning and end date range filters are present");
            var beginDate = DateTime.Now.AddDays(-10).ToString("MM/d/yyyy");
            var endDate = DateTime.Now.AddMonths(3).ToString("MM/d/yyyy");
            _qaClaimSearch.GetSideBarPanelSearch.SetDateFieldFrom(fieldName, beginDate);
            var approveDate = _qaClaimSearch.GetSideBarPanelSearch.GetDateFieldTo(fieldName);
            approveDate.ShouldBeEqual(Convert.ToDateTime(beginDate).ToString("MM/dd/yyyy"),
                "Date populated in the beginning date is automatically populated in the end date field");
            approveDate.IsDateInFormat()
                .ShouldBeTrue("Date should be in MM/DD/YYYY format");
            _qaClaimSearch.GetSideBarPanelSearch.SetDateFieldTo(fieldName, endDate);
            _qaClaimSearch.ClickOnFindButton();
            ValidateForPopUpMessage("Popup Displayed when date range exceeds 3 months",
                "Date range selected must not exceed 3 months.", _qaClaimSearch);
        }

        private void ValidateDefaultValueForQaClaimSearch(string quickSearchValue, QaClaimSearchPage _qaClaimSearch)
        {
            StringFormatter.PrintMessageTitle("Validate For default value of QA Claim Search filter panel fields");
            _qaClaimSearch.GetSideBarPanelSearch.GetInputValueByLabel("Quick Search")
                .ShouldBeEqual(quickSearchValue, "Default value is as expected");
            _qaClaimSearch.SelectQuickSearch(QaClaimQuickSearchTypeEnum.AllQaClaims.GetStringValue());
            _qaClaimSearch.GetSideBarPanelSearch.GetInputValueByLabel("Analyst")
               .ShouldBeNullorEmpty("Analyst value defaults to blank ");
            _qaClaimSearch.GetSideBarPanelSearch.GetInputValueByLabel("Client")
                .ShouldBeNullorEmpty("Client value defaults to blank ");
            _qaClaimSearch.GetSideBarPanelSearch.GetInputValueByLabel("QA Result")
                .ShouldBeEqual("All", "Default value is as expected");
            _qaClaimSearch.GetSideBarPanelSearch.GetInputValueByLabel("QA Reviewer")
                .ShouldBeNullorEmpty("QA Reviewer value defaults to blank ");
            _qaClaimSearch.GetSideBarPanelSearch.GetDateFieldTo("Approve Date")
                .ShouldBeNullorEmpty("Approve End Date field value defaults to blank ");
            _qaClaimSearch.GetSideBarPanelSearch.GetDateFieldFrom("Approve Date")
                .ShouldBeNullorEmpty("Approve From Date field value defaults to blank ");
            _qaClaimSearch.GetSideBarPanelSearch.GetDateFieldFrom("QA Review Date")
                .ShouldBeNullorEmpty("QA Review From Date field value defaults to blank ");
            _qaClaimSearch.GetSideBarPanelSearch.GetDateFieldTo("QA Review Date")
               .ShouldBeNullorEmpty("QA Review End Date field value defaults to blank ");
        }

        private void ApproveClaimsInClaimActionAndReturnToNewClaimSearch(ClaimSearchPage claimSearch, string claimSequence)
        {
            var newClaimAction = claimSearch.SearchByClaimSequenceToNavigateToClaimActionPage(claimSequence);
            newClaimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
            newClaimAction.ClickOnApproveButton();
            newClaimAction.WaitForWorkingAjaxMessage();
        }
    
        private void SearchClaimSeqForAnalystAndClient(string analyst, string client, QaClaimSearchPage _qaClaimSearch)
        {
            _qaClaimSearch.SelectAllQaClaims();
            _qaClaimSearch.SelectAnalyst(analyst);
            switch (client)
            {
                case "CVTY":
                    _qaClaimSearch.SelectCvtyClient();
                    break;
                case "PEHP":
                    _qaClaimSearch.SelectPehpClient();
                    break;
                //case "RPE":
                //    _qaClaimSearch.SelectRpeClient();
                //    break;
                case "TTREE":
                    _qaClaimSearch.SelectTtreeClient();
                    break;
                //case "SMTST":
                default:
                    _qaClaimSearch.SelectSmtstClient();
                    break;
            }
            _qaClaimSearch.ClickOnFindButton();
        }

        #endregion

    }
    
}

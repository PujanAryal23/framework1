using System;
using System.Collections.Generic;
using System.Linq;
using Nucleus.Service.Data;
using Nucleus.Service.PageServices.Claim;
using Nucleus.Service.Support.Common.Constants;
using Nucleus.Service.Support.Enum;
using Nucleus.UIAutomation.TestSuites.Base;
using NUnit.Framework;
using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;
using Nucleus.Service.PageServices.Settings;
using UIAutomation.Framework.Core.Driver;
using Nucleus.Service.PageServices.Appeal;
using Nucleus.Service.PageServices.Base.Default;
using Nucleus.Service.PageServices.QuickLaunch;
using UIAutomation.Framework.Utils;
using Nucleus.Service.Support.Common;
using Nucleus.Service.Support.Environment;
using static System.String;

namespace Nucleus.UIAutomation.TestSuites.TestCases.ClientUser
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class ClaimActionClient2
    {
        //#region PRIVATE FIELDS

        //private ClaimActionPage _claimAction;
        //private ClaimSearchPage _claimSearch;
        //private _claimAction.GetFileUploadPage _claimAction.GetFileUploadPage;
        //private AppealManagerPage _appealManager;
        //private string _claseq;
        //private string _claseq2;

        //#endregion

        #region PROTECTED PROPERTIES

        protected string FullyQualifiedClassName
        {
            get
            {
                return GetType().FullName;
            }
        }

        #endregion

        //#region OVERRIDE METHODS

        //protected override void ClassInit()
        //{
        //    try
        //    {
        //        UserLoginIndex = 2;
        //        base.ClassInit();
        //        _claseq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, "ClaimSequenceForClient", "ClaimSequence", "Value");
        //        _claseq2 = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, "ClaimSequence2ForClient", "ClaimSequence", "Value");
        //        _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(_claseq);
        //        _claimAction.GetFileUploadPage = _claimAction.GetFileUploadPage;

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

        //    if (string.Compare(UserType.CurrentUserType, UserType.CLIENT, StringComparison.OrdinalIgnoreCase) != 0)
        //    {
        //        CurrentPage = automatedBase.QuickLaunch = _claimAction.Logout().LoginAsClientUser4();
        //        _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(_claseq);
        //    }
        //    if (!CurrentPage.IsCurrentClientAsExpected(EnvironmentManager.TestClient))
        //    {
        //        CheckTestClientAndSwitch();
        //        _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(_claseq);
        //    }
        //    if (CurrentPage.GetPageHeader() != "Claim Action")
        //    {

        //        Console.Out.WriteLine("Back to automatedBase.QuickLaunch page");
        //        _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(_claseq);
        //        Console.Out.WriteLine("Finally Navigate to CV Claim Work List");
        //    }
        //    else if (_claimAction.IsWorkingAjaxMessagePresent())
        //    {
        //        _claimAction.Refresh();
        //        _claimAction.WaitForWorkingAjaxMessage();
        //    }
        //    base.TestCleanUp();

        //}

        //protected override void ClassCleanUp()
        //{
        //    try
        //    {
        //        _claimAction.CloseDatabaseConnection();
        //        //if (CurrentPage.IsQuickLaunchIconPresent())
        //        //    _claimAction.GoToQuickLaunch();
        //    }

        //    finally
        //    {
        //        base.ClassCleanUp();
        //    }
        //}

        //#endregion

        #region TEST SUITES

        [Test] //CAR-3260(CAR-3207)
        [Author("ShreyaS")]
        public void Verify_lock_icon_for_dental_review_client()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(2))
            {
                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestName);
                var claSeqs = paramLists["ClaimSeqs"].Split(',').ToList();
                ClaimSearchPage _claimSearch = automatedBase.CurrentPage.NavigateToClaimSearch();
                _claimSearch.GetCommonSql.GetAppealTypeByClaseqDb(claSeqs[0]).ShouldBeEqual("D", "Appeal type should be Dental");

                StringFormatter.PrintMessage("Verification of lock icon for Dental Appeal in Claim action page");
                ClaimActionPage _claimAction = _claimSearch.SearchByClaimSequenceToNavigateToClaimActionPage(claSeqs[0]);
                _claimAction.IsLocked().ShouldBeFalse("Lock icon should not be present for Dental appeal in Claim Action page");
                
                StringFormatter.PrintMessage("Verification of lock icon for Dental Appeal in Claim Search page");
                _claimAction.ClickOnBrowserBack();
                _claimSearch.IsClaimLockPresentForClaimSequence(claSeqs[0]).ShouldBeFalse("Lock icon should not be present for Dental appeal in Claim Search page");

                StringFormatter.PrintMessage("Verification of lock icon for non-dental Appeal in Claim Action page");
                _claimSearch.SearchByClaimSequenceToNavigateToClaimActionPage(claSeqs[1]);
                _claimSearch.GetCommonSql.GetAppealTypeByClaseqDb(claSeqs[1]).ShouldBeEqual("A", "Appeal type should be Appeal");
                _claimAction.IsLocked().ShouldBeTrue("Lock icon should be present for Non-Dental appeal in Claim Action page");

                StringFormatter.PrintMessage("Verification of lock icon for non-dental Appeal in Claim search page");
                _claimAction.ClickOnBrowserBack();
                _claimSearch.IsClaimLockPresentForClaimSequence(claSeqs[1]).ShouldBeTrue("Lock icon should be present for Non-Dental appeal in Claim Search page");

                StringFormatter.PrintMessage("Verification of lock icon for a claim with both dental and non-dental Appeal in Claim search page");
                _claimSearch.SearchByClaimSequenceToNavigateToClaimActionPage(claSeqs[2]);
                _claimAction.ClickOnViewAppealIcon();
                _claimAction.GetListOfAppealTypesInRows().ShouldCollectionBeEqual(new List<string>() { "Appeal", "Dental Review" }, "Claim Should Have Appeals with both Dental and Non-Dental type");
                _claimAction.IsLocked().ShouldBeTrue("Lock icon should be present for a claim with both dental and non-dental Appeal in Claim search page");

                StringFormatter.PrintMessage("Verification of lock icon for a claim with both dental and non-dental Appeal in Claim search page");
                _claimAction.ClickOnBrowserBack();
                _claimSearch.IsClaimLockPresentForClaimSequence(claSeqs[2]).ShouldBeTrue("Lock icon should be present for a claim with both dental and non-dental Appeal in Claim search page");
            }
        }

        [Test] //CAR-2957(CAR-2928)
        public void Verify_flag_description_in_flag_Description_pop_up_client()
        {
            using (var automatedBase = new AutomatedBaseClientParallel(2))
            {
                ClaimActionPage _claimAction = null;
                ClaimSearchPage _claimSearch;
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                string flag = paramLists["Flag"];
                string popupHeader = paramLists["PopupHeader"];
                string flagType = paramLists["FlagType"];
                string clientAutoReviewValue = paramLists["ClientAutoReviewValue"];
                string flagDescriptionValue = paramLists["FlagDescriptionValue"];
                try
                {
                    _claimSearch = automatedBase.QuickLaunch.NavigateToClaimSearch();
                    _claimSearch.GetSideBarPanelSearch.ClickOnClearLink();
                    _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                        ClaimQuickSearchTypeEnum.FlaggedClaims.GetStringValue());
                    _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Flag", flag);
                    _claimSearch.GetSideBarPanelSearch.ClickOnFindButton();
                    _claimSearch.WaitForWorking();
                    _claimAction = _claimSearch.ClickOnClaimSequenceOfSearchResult(1);
                    _claimAction.WaitForPageToLoad();
                    _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                    var flagShortDescription = _claimAction.GetEOBMessageFromDatabase(flag, "S");
                    var flagLongDescription = Regex.Replace(_claimAction.GetEOBMessageFromDatabase(flag, "G"), @"\s+", " ");
                    var _flagPopup = _claimAction.ClickOnFlagandSwitch($"Flag Information - {flag}", flag);
                    _flagPopup.GetPopupHeaderText().ShouldBeEqual(popupHeader, "Popup header should match");
                    _flagPopup.GetTextValueinLiTag(1)
                        .ShouldBeEqual(flagShortDescription, "Short flag description should match");
                    _flagPopup.GetTextValueWithInTag(tag: "").Replace("\r\n", " ")
                        .ShouldBeEqual(flagType, "Flag Type should match");
                    _flagPopup.GetTextValueWithinSpanTag(4)
                        .ShouldBeEqual(clientAutoReviewValue, "Client Auto Review value should match");
                    _flagPopup.GetTextValueWithInTag(5, ">span")
                        .ShouldBeEqual(flagDescriptionValue, "Flag description label should be present");
                    _flagPopup.GetTextValueWithInTag(5, "").Replace($"{flagDescriptionValue}\r\n", "")
                        .ShouldBeEqual(flagLongDescription, "Long description should match");
                    _claimAction = _flagPopup.ClosePopupOnNewClaimActionPage(flag);
                }

                finally
                {
                    _claimAction.CloseAnyPopupIfExist();
                }
            }

        }

        [Test] //CAR-2432(CAR-2569)
        [NonParallelizable]
        public void Verify_logic_pend_functionality_client()
        {
            using (var automatedBase = new AutomatedBaseClientParallel(2))
            {
                ClaimActionPage _claimAction = null;
                ClaimSearchPage _claimSearch;
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var providerNameForClaseq = paramLists["ProviderNameForClaseq"];
                var claimNo = paramLists["ClaimNo"];
                List<string> claseq = null;

                try
                {
                    StringFormatter.PrintMessage("Creation of new logic");
                    _claimSearch = automatedBase.QuickLaunch.NavigateToClaimSearch();
                    _claimSearch.UpdateClaimStatusToUnreviewedFromDatabase(claimNo);
                    if (!_claimSearch.GetSideBarPanelSearch.GetTopHeaderName().Equals("Find Claim"))
                        _claimSearch.ClickOnFindClaimIcon();
                    _claimSearch.GetSideBarPanelSearch.ClickOnClearLink();
                    _claimSearch.SearchByClaimNmber(claimNo);
                    _claimAction = _claimSearch.ClickOnClaimSequenceOfSearchResult(1);
                    claseq = _claimAction.GetClaimsByClaimNoforClientUserFromDatabase(claimNo);
                    if(_claimAction.GetClaimSequence()!= claseq[0])
                        claseq = claseq.OrderBy(x => x.Split('-')[0]).ToList();
                    _claimAction.GetClaimStatus().ShouldBeEqual(StatusEnum.ClientUnreviewed.GetStringDisplayValue(),
                        "Initial status should be Client Unreviewed");
                    _claimAction.GetClaimSequence().ShouldBeEqual(claseq[0], "Claim Sequence should match");
                    _claimAction.ClickOnAddLogicIconByRow(1);
                    _claimAction.AddLogicMessageTextarea("Test Logic From Client User");
                    _claimAction.GetSideWindow.Save();
                    _claimAction.WaitForWorking();

                    StringFormatter.PrintMessage(
                        "Verification that page navigates to next ClaimSeq in the search result list after logic creation");
                    _claimAction.GetClaimSequence().ShouldBeEqual(claseq[1],
                        "On creating the logic, it should transfer to another claim sequence in the list");

                    StringFormatter.PrintMessage(
                        "Verification that page navigates to Claim Search page when next ClaimSeq is not available after logic creation");
                    _claimAction.ClickOnAddLogicIconByRow(1);
                    _claimAction.AddLogicMessageTextarea("Test Logic From Client User");
                    _claimAction.GetSideWindow.Save();
                    _claimAction.WaitForWorking();
                    _claimSearch.GetPageHeader().ShouldBeEqual(PageHeaderEnum.ClaimSearch.GetStringValue(),
                        "User should be navigated to Claim Search page after creating the logic when there is no other claim sequence in the list");

                    StringFormatter.PrintMessage(
                        "Verification that status changes to Logic Pend after creation of Logic");
                    _claimSearch.GetGridViewSection.GetValueInGridByColRow(12).ShouldBeEqual(
                        ClaimSubStatusTypeEnum.LogicPend.GetStringValue(),
                        "Status should change to logic pend after logic creation");
                    _claimSearch.GetGridViewSection.GetValueInGridByColRow(12, 2).ShouldBeEqual(
                        ClaimSubStatusTypeEnum.LogicPend.GetStringValue(),
                        "Status should change to logic pend after logic creation");

                    StringFormatter.PrintMessage("Verification of assigned to value");
                    _claimSearch.GetGridViewSection.ClickOnGridRowByRowInClaimSearch();
                    _claimSearch.GetAssignedTo().ShouldBeEqual(_claimSearch.GetLoggedInUserFullName(),
                        "Claim should be assigned to user who created the logic");

                    StringFormatter.PrintMessage("Verification Of Claim Audit History");
                    var rowToBeClicked =
                        _claimSearch.GetGridViewSection.GetGridRowNumberByColAndLabel(providerNameForClaseq, 6);
                    _claimSearch.GetGridViewSection.ClickOnGridByRowCol(rowToBeClicked, 2);

                    var t = DateTime.ParseExact(_claimAction.GetSystemDateFromDatabase(), "MM/dd/yyyy hh:mm:ss tt",
                        CultureInfo.InvariantCulture);

                    ClaimProcessingHistoryPage claimProcessingHistory =
                        _claimAction.ClickOnClaimProcessingHistoryAndSwitch();
                    DateTime.ParseExact(claimProcessingHistory.GetClaimHistoryTableDataValueByRowCol(1, 1),
                        "M/d/yyyy h:m:s tt", CultureInfo.InvariantCulture).AssertDateRange(t.AddSeconds(-5), t,
                        "Modified Date And Time Stamp should match");
                    claimProcessingHistory.GetClaimHistoryTableDataValueByRowCol(1, 6).ShouldBeEqual(
                        ClaimSubStatusTypeEnum.LogicPend.GetStringValue(), "Claim Status should be Logic Pend");
                    _claimAction = claimProcessingHistory.CloseClaimProcessingHistoryPageAndSwitchToClaimActionPage();

                    StringFormatter.PrintMessage(
                        "Verification that status can be changed from Logic Pend to other status using Transfer button");
                    _claimAction.ClickOnTransferButton();
                    _claimAction.SelectStatusCode(ClaimSubStatusTypeEnum.DocumentationRequired.GetStringValue());
                    _claimAction.ClickOnSaveButton();

                    StringFormatter.PrintMessage(
                        "Verification that status from Logic Pend to other status after approving the claim sequence");
                    _claimAction.ClickOnApproveButton();
                    var rowForApprovedClaim =
                        _claimSearch.GetGridViewSection.GetGridRowNumberByColAndLabel(providerNameForClaseq, 6);
                    var rowForAnotherClaim = rowForApprovedClaim == 1 ? 2 : 1;
                    _claimSearch.GetGridViewSection.GetValueInGridByColRow(12, rowForApprovedClaim).ShouldBeEqual(
                        ClaimSubStatusTypeEnum.DocumentationRequired.GetStringValue(),
                        $"Status should change to {ClaimSubStatusTypeEnum.DocumentationRequired.GetStringValue()} after claim is transferred");
                    _claimSearch.GetGridViewSection.GetValueInGridByColRow(12, rowForAnotherClaim).ShouldNotBeEqual(
                        ClaimSubStatusTypeEnum.LogicPend.GetStringValue(),
                        "Status should change after approving claim sequence of Logic pend status");

                }

                finally
                {
                    StringFormatter.PrintMessageTitle("Running the 'finally' block");
                    _claimAction.DeleteLogicNoteFromDatabase(claseq[0]);
                    _claimAction.DeleteLogicNoteFromDatabase(claseq[1]);
                    _claimAction.UpdateClaimStatus(claseq[0], ClientEnum.SMTST.ToString(),
                        StatusEnum.CotivitiUnreviewed.GetStringValue(), 'T');
                    _claimAction.UpdateClaimStatus(claseq[1], ClientEnum.SMTST.ToString(),
                        StatusEnum.CotivitiUnreviewed.GetStringValue(), 'T');
                }
            }
        }

        [Test]//US65703
        public void Verify_that_the_add_notes_icon_will_be_displayed_on_Claim_Action_Page_if_notes_are_not_present_for_Client_User()
        {
            using (var automatedBase = new AutomatedBaseClientParallel(2))
            {
                ClaimActionPage _claimAction;
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var claimSeqWithoutNote = paramLists["ClaimSequenceWithoutNote"];
                _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(claimSeqWithoutNote);
                _claimAction.IsAddNoteIndicatorPresent().ShouldBeTrue("Is Add note indicator present");
                _claimAction.ClickOnClaimNotes();
                var noOfNotes = _claimAction.GetNoteListCount();
                (noOfNotes > 0).ShouldBeFalse("The given claim sequence has notes should be false.");
            }

        }

        // [Test]//US14402
        public void Verify_that_the_view_notes_icon_will_be_displayed_on_Claim_Action_Page_if_notes_are_present_for_Client_User()
        {
            using (var automatedBase = new AutomatedBaseClientParallel(2))
            {
                ClaimActionPage _claimAction;
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                string claimSeqWithNote = paramLists["ClaimSequenceWithNote"];
                _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(claimSeqWithNote);
                NotesPopupPage claimNotePage = null;
                _claimAction.IsViewNoteIndicatorPresent().ShouldBeTrue("Is view note indicator present");
                try
                {
                    claimNotePage = _claimAction.ClickOnClaimNotes(claimSeqWithNote);
                    int noOfNotes = claimNotePage.GetRowCount();
                    (noOfNotes > 0).ShouldBeTrue("The given claim sequence has notes");

                }
                finally
                {
                    if (claimNotePage != null)
                        claimNotePage.ClosePopupNoteAndSwitchToNewClaimActionPage();
                }
            }

        }



        [Test]//US33626
        public void Verify_that_client_fraud_analysts_can_delete_restore_PCI_flags()
        {
            using (var automatedBase = new AutomatedBaseClientParallel(100))
            {
                ClaimActionPage _claimAction;
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestExtensions.TestName);
                var claimSeqWithFfpAndPciFlags = paramLists["ClaimSequenceWithPciAndFfpFlags"];
                var ffpFlagPairs = automatedBase.DataHelper
                    .GetTestData(FullyQualifiedClassName, TestExtensions.TestName, "FfpFlag").ToList();
                var pciFlagPairs = automatedBase.DataHelper
                    .GetTestData(FullyQualifiedClassName, TestExtensions.TestName, "PciFlag").ToList();
                _claimAction = automatedBase.Login.LoginAsClientUserHavingFfpEditOfPciFlagsAuthority().NavigateToClaimSearch()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(claimSeqWithFfpAndPciFlags);
                RestoreDeletedFlagsBySpecificLineEdit(ffpFlagPairs.Concat(pciFlagPairs),_claimAction);

                StringFormatter.PrintMessageTitle(" Delete All Flags on the claim ");
                var countDeletedFlagsBefore = GetCountOfDeletedFlagsBySpecificLineEdit(ffpFlagPairs,_claimAction);
                _claimAction.ClickOnDeleteAllFlagsIcon();
                var countDeletedFlagsAfter = GetCountOfDeletedFlagsBySpecificLineEdit(ffpFlagPairs,_claimAction);
                (countDeletedFlagsAfter - countDeletedFlagsBefore).ShouldBeEqual(ffpFlagPairs.Count,
                    "Deleted Flags Count");
                VerifyGivenFlagsDeleteIconDisabled(pciFlagPairs,_claimAction);
                RestoreDeletedFlagsBySpecificLineEdit(ffpFlagPairs,_claimAction);

                StringFormatter.PrintMessageTitle(" Delete All Flags on flagged Line ");
                countDeletedFlagsBefore = GetCountOfDeletedFlagsBySpecificLineEdit(ffpFlagPairs,_claimAction);
                _claimAction.ClickOnDeleteAllFlagsIconOnFlagLine();
                countDeletedFlagsAfter = GetCountOfDeletedFlagsBySpecificLineEdit(ffpFlagPairs,_claimAction);
                (countDeletedFlagsAfter - countDeletedFlagsBefore).ShouldBeEqual(ffpFlagPairs.Count,
                    "Deleted Flags Count");
                VerifyGivenFlagsDeleteIconDisabled(pciFlagPairs,_claimAction);
                RestoreDeletedFlagsBySpecificLineEdit(ffpFlagPairs,_claimAction);

                StringFormatter.PrintMessageTitle(" Delete FFP Flag ");
                countDeletedFlagsBefore = GetCountOfDeletedFlagsBySpecificLineEdit(ffpFlagPairs,_claimAction);
                _claimAction.ClickOnDeleteIconFlagLevelForLineEdit(ffpFlagPairs[0].Key, ffpFlagPairs[0].Value);
                countDeletedFlagsAfter = GetCountOfDeletedFlagsBySpecificLineEdit(ffpFlagPairs,_claimAction);
                (countDeletedFlagsAfter - countDeletedFlagsBefore).ShouldBeEqual(1,
                    "Deleted Flags Count");
                VerifyGivenFlagsDeleteIconDisabled(pciFlagPairs,_claimAction);
                RestoreDeletedFlagsBySpecificLineEdit(ffpFlagPairs,_claimAction);

                StringFormatter.PrintMessageTitle(" Delete CV Flag ");
                countDeletedFlagsBefore = GetCountOfDeletedFlagsBySpecificLineEdit(pciFlagPairs,_claimAction);
                _claimAction.ClickOnDeleteIconFlagLevelForLineEdit(pciFlagPairs[0].Key, pciFlagPairs[0].Value);
                countDeletedFlagsAfter = GetCountOfDeletedFlagsBySpecificLineEdit(pciFlagPairs,_claimAction);
                (countDeletedFlagsAfter - countDeletedFlagsBefore).ShouldBeEqual(1,
                    "Deleted Flags Count");
                RestoreDeletedFlagsBySpecificLineEdit(pciFlagPairs,_claimAction);
            }
        }


        [Test, Category("OnDemand")] // + TANT- 189
        [NonParallelizable]
        [Retrying(Times=3)]
        public void Verify_access_to_original_claim_data_for_different_PHI_authority_and_setting()
        {
            using (var automatedBase = new AutomatedBaseClientParallel(2))
            {
                ClaimActionPage _claimAction = null;
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var claseq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, 
                    "ClaimSequenceForClient", "ClaimSequence", "Value");
                var claseq2 = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    "ClaimSequence2ForClient", "ClaimSequence", "Value");
                _claimAction =
                    automatedBase.QuickLaunch.NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(claseq);
                try
                {
                    Console.WriteLine("Client profile, PHI accessibilty being set to Allow all.");
                    _claimAction.GetCommonSql.UpdateSpecificClientDataInDB("client_user_phi_access = 'A'",
                        ClientEnum.SMTST.ToString());
                    _claimAction.RefreshPage(false);

                    Console.WriteLine("Client user <uiautomation_cl> with view no phi authority");
                    _claimAction.ClickMoreOption();
                    _claimAction.IsOriginalClaimDataLinkPresent().ShouldBeTrue("Original Data Claim Link is present.");
                    _claimAction.Logout().LoginAsClientUserHavingFfpEditOfPciFlagsAuthority();
                    _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch()
                        .SearchByClaimSequenceToNavigateToClaimActionPage(claseq2);
                    Console.WriteLine("Client user <uiautomation_cl2> with view phi authority");
                    _claimAction.ClickMoreOption();
                    _claimAction.IsOriginalClaimDataLinkPresent().ShouldBeTrue("Original Data Claim Link is present.");

                    Console.WriteLine(
                        "Client profile, PHI accessibility being set to Allow only with PHI view accessibility.");
                    _claimAction.GetCommonSql.UpdateSpecificClientDataInDB("client_user_phi_access = 'U'",
                        ClientEnum.SMTST.ToString());

                    _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch()
                        .SearchByClaimSequenceToNavigateToClaimActionPage(claseq2);
                    Console.WriteLine("Client user <uiautomation_cl2> with view phi authority");
                    _claimAction.ClickMoreOption();
                    _claimAction.IsOriginalClaimDataLinkPresent().ShouldBeTrue("Original Data Claim Link is present.");

                    automatedBase.CurrentPage = automatedBase.QuickLaunch.Logout().LoginAsClientUser4();
                    _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch()
                        .SearchByClaimSequenceToNavigateToClaimActionPage(claseq);
                    Console.WriteLine("Client user <uiautomation_cl> with view no phi authority");
                    _claimAction.ClickMoreOption();
                    _claimAction.IsOriginalClaimDataLinkPresent().ShouldBeFalse("Original Data Claim Link is present.");

                    Console.WriteLine("Client profile, PHI accessibilty being set to Do not Allow .");
                    _claimAction.GetCommonSql.UpdateSpecificClientDataInDB("client_user_phi_access = 'D'",
                        ClientEnum.SMTST.ToString());


                    _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch()
                        .SearchByClaimSequenceToNavigateToClaimActionPage(claseq);
                    _claimAction.WaitForStaticTime(1000);
                    Console.WriteLine("Client user <uiautomation_cl> with view no phi authority");
                    _claimAction.ClickMoreOption();
                    _claimAction.IsOriginalClaimDataLinkPresent().ShouldBeFalse("Original Data Claim Link is present.");

                    _claimAction.Logout().LoginAsClientUserHavingFfpEditOfPciFlagsAuthority();
                    _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch()
                        .SearchByClaimSequenceToNavigateToClaimActionPage(claseq2);
                    Console.WriteLine("Client user <uiautomation_cl2> with view phi authority");
                    _claimAction.ClickMoreOption();
                    _claimAction.IsOriginalClaimDataLinkPresent().ShouldBeFalse("Original Data Claim Link is present.");

                }
                finally
                {
                    StringFormatter.PrintMessageTitle("Finally Block. \n Change accessibility setting to default");
                    _claimAction.GetCommonSql.UpdateSpecificClientDataInDB("client_user_phi_access = 'U'",
                        ClientEnum.SMTST.ToString());
                }
            }
        }


        //[Test,Category("Acceptance")] //US50647 
        public void Verify_that_Original_claim_data_page_pops_up_with_presence_of_HCILINSEQ_column_when_click_on_original_claim_data()
        {
            using (var automatedBase = new AutomatedBaseClientParallel(100))
            {
                ClaimActionPage _claimAction = null;
                var claseq2 = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    "ClaimSequence2ForClient", "ClaimSequence", "Value");
                automatedBase.Login.LoginAsClientUserHavingFfpEditOfPciFlagsAuthority().NavigateToClaimSearch()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(claseq2);
                try
                {
                    Console.WriteLine("Client user <uiautomation_cl2> with view phi authority");
                    _claimAction.ClickMoreOption();
                    var originalClaimData = _claimAction.ClickOnOriginalClaimDataAndSwitch();
                    originalClaimData.IsColumnPresentByColumnName("HCILINSEQ")
                        .ShouldBeTrue("Is HCILINSEQ Column Present");

                }
                finally
                {
                    _claimAction.CloseAllExistingPopupIfExist();
                }
            }
        }

        [Test]//US51982 + CAR-3031[CAR-3061]
        public void Verify_that_client_user_able_to_view_appropriate_audit_records_for_switch_from_another_claim_approved_and_delete()
        {
            using (var automatedBase = new AutomatedBaseClientParallel(2))
            {
                ClaimActionPage _claimAction;
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> testData = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestExtensions.TestName);
                var claimSeq = testData["ClaimSequence"];
                var addBySwitchAuditDetail = testData["AddBySwitchAuditDetail"].Replace("\\r\\n", "\r\n");
                var approvedAuditDetail = testData["ApprovedAuditDetail"].Replace("\\r\\n", "\r\n");
                var deleteAuditDetail = testData["DeleteAuditDetail"].Replace("\\r\\n", "\r\n");
                var restoreAuditDetail = testData["RestoreAuditDetail"].Replace("\\r\\n", "\r\n");
                var noteAudtDetail = testData["NoteAudtDetail"].Replace("\\r\\n", "\r\n");
                _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(claimSeq);
                _claimAction.ClickOnFirstFlagLineOfFlaggedLinesDiv();
                _claimAction.WaitForWorkingAjaxMessage();
                _claimAction.WaitForStaticTime(4000);
                var flagDetailsList = _claimAction.GetFlagAuditList();
                var totalRow = flagDetailsList.Count - 1;
                flagDetailsList[totalRow - 1]
                    .ShouldBeEqual(addBySwitchAuditDetail, "Add Flag by Switching by System", true);
                flagDetailsList[totalRow - 3].ShouldBeEqual(approvedAuditDetail, "Approved Audit Verification", true);
                flagDetailsList[totalRow - 4].ShouldBeEqual(restoreAuditDetail, "Restore Audit Verification", true);
                flagDetailsList[totalRow - 5].ShouldBeEqual(deleteAuditDetail, "Delete Audit Verification", true);
                flagDetailsList[totalRow - 6].ShouldBeEqual(noteAudtDetail, "Note Audit Verifiction", true);
            }

        }


        [Test]//US53199
        public void Validate_EOB_message_displayed_for_fre_flag()
        {
            using (var automatedBase = new AutomatedBaseClientParallel(2))
            {
                ClaimActionPage _claimAction = null;
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> testData = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestExtensions.TestName);
                var claimSeq = testData["ClaimSequence"];
                var claimSeqWithTriggerLine = testData["ClaimSequenceWithTriggerLine"];
                var flag = testData["Flag"];
                var message = testData["Message"];
                var triggerClaimAltClaimNoWithLink = testData["TriggerClaimAltClaimNoWithLink"];
                var triggerClaimAltClaimNoWithOutLink = testData["TriggerClaimAltClaimNoWithOutLink"];
                var triggerClimSequence = testData["TriggerClimSequence"];
                _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(claimSeq);
                try
                {
                    var lineProcCodeDesc = _claimAction.GetProcDescriptionTextOfFlaggedLines();
                    var lineProcCode = _claimAction.GetProcCodeForRow(1);
                    var lineNo = _claimAction.GetFlagLineValue(1);

                    _claimAction.ClickOnFirstEditFlag();
                    var editSugunit = _claimAction.GetSugUnits();
                    var expectedDesp = String.Format(message, lineProcCode,
                        lineProcCodeDesc, lineNo,
                        editSugunit, triggerClaimAltClaimNoWithOutLink);
                    _claimAction.GetFlagListForClaimLine(1)[0].ShouldBeEqual(flag, "Expected flag is FRE");
                    _claimAction.GetFlagLineDetailsRowColWise(1, 3)
                        .ShouldBeEqual(expectedDesp, "FRE EOB message displayed as expected.");
                    _claimAction.IsTriggerAltClaimOnFlagLinesClickable(1)
                        .ShouldBeFalse(
                            "TriggerAltClaimNo should not be a hyperlink when trigger claim is the current claim");

                    SearchByClaimSeqFromWorkListForClientUser(claimSeqWithTriggerLine,_claimAction);

                    lineProcCodeDesc = _claimAction.GetProcDescriptionTextOfFlaggedLines();
                    lineProcCode = _claimAction.GetProcCodeForRow(1);
                    lineNo = _claimAction.GetFlagLineValue(1);

                    _claimAction.ClickOnEditIconFlagLevelForLineEdit(lineNo, flag);
                    editSugunit = _claimAction.GetSugUnits();
                    expectedDesp = String.Format(message, lineProcCode,
                        lineProcCodeDesc, lineNo,
                        editSugunit, triggerClaimAltClaimNoWithLink);
                    _claimAction.GetFlagListForClaimLine(1)[1].ShouldBeEqual(flag, "Expected flag is FRE");
                    _claimAction.GetFlagLineDetailsRowColWise(2, 3)
                        .ShouldBeEqual(expectedDesp, "FRE EOB message displayed as expected.");

                    _claimAction.IsTriggerAltClaimOnFlagLinesClickable(2)
                        .ShouldBeTrue(
                            "TriggerAltClaimNo should not be a hyperlink when trigger claim is the current claim");
                    _claimAction.GetClaimSequenceFromClaimActionPopupOnClickTriggerAltClaimNo(triggerClimSequence, 2)
                        .ShouldBeEqual(triggerClimSequence, "Proper Trigger Claim popup Should display");
                }
                finally
                {
                    _claimAction.CloseAllExistingPopupIfExist();
                    if (_claimAction.IsPageErrorPopupModalPresent())
                        _claimAction.ClosePageError();
                }
            }

        }

        [Test]//US57573
        public void Validate_EOB_message_displayed_for_RDS_flag()
        {
            using (var automatedBase = new AutomatedBaseClientParallel(2))
            {
                ClaimActionPage _claimAction;
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> testData = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestExtensions.TestName);
                var claimSeq = testData["ClaimSequence"];
                var flag = testData["Flag"];
                var message = testData["Message"];
                var triggerLineProcCode = testData["TriggerLineProcCode"];
                var triggerLineProcCodeDesc = testData["TriggerLineProcCodeDesc"];
                var triggerClaim = testData["TriggerClaim"];
                var triggerLineNo = testData["TriggerLineNo"];
                _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(claimSeq);
                var lineProcCodeDesc = _claimAction.GetProcDescriptionTextOfFlaggedLines();
                var lineProcCode = _claimAction.GetProcCodeForRow(1);
                var expectedEobMessage = string.Format(message, lineProcCode, lineProcCodeDesc, triggerLineProcCode,
                    triggerLineProcCodeDesc, triggerClaim, triggerLineNo);
                _claimAction.GetEOBMessage(flag)
                    .ShouldBeEqual(expectedEobMessage, string.Format("{0} EOB message displayed as expected.", flag));
            }
        }


        [Test]//US58177
        public void Verify_that_sug_code_should_empty_for_UNL_or_COS_flag_in_flag_details()
        {
            using (var automatedBase = new AutomatedBaseClientParallel(2))
            {
                ClaimActionPage _claimAction = null;
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var claimSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "ClaimSequence", "Value");
                var COSEditEOBMessage = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "COSEditEOBMessage", "Value");
                var UNLEditEOBMessage = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "UNLEditEOBMessage", "Value");
                _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(claimSeq);
                const string flagUNL = "UNL";
                const string flagCOS = "COS";

                try
                {
                    _claimAction.ClickOnFlagLineByFlag(flagUNL);
                    _claimAction.GetClaimFlagAuditHistoryHeaderDetailByFlagAndLabel(flagUNL, "Sug Code")
                        .ShouldBeNullorEmpty(string.Format("Sug Code should empty for <{0}> flag", flagUNL));
                    _claimAction.ClickOnFlagLineByFlag(flagCOS);
                    _claimAction.GetClaimFlagAuditHistoryHeaderDetailByFlagAndLabel(flagCOS, "Sug Code")
                        .ShouldBeNullorEmpty(string.Format("Sug Code should empty for <{0}> flag", flagCOS));

                    StringFormatter.PrintMessageTitle(
                        "Verify sug proc should disabled and for any flag that does not allow Sug Procs when trying to modify flag");
                    _claimAction.ClickOnEditIconFlagLevelForLineEdit(1, flagUNL);
                    _claimAction.IsSugCodeTextFieldEnabled()
                        .ShouldBeFalse(string.Format("Is Sug Code disabled for <{0}> flag", flagUNL));
                    _claimAction.ClickOnEditIconFlagLevelForLineEdit(1, flagCOS);
                    _claimAction.IsSugCodeTextFieldEnabled()
                        .ShouldBeFalse(string.Format("Is Sug Code disabled for <{0}> flag", flagCOS));
                    _claimAction.ClickOnCancelLink();
                    _claimAction.GetEOBMessage(flagCOS)
                        .ShouldBeEqual(COSEditEOBMessage,
                            string.Format("{0} EOB message displayed as expected.", flagCOS));
                    _claimAction.GetEOBMessage(flagUNL)
                        .ShouldBeEqual(UNLEditEOBMessage,
                            string.Format("{0} EOB message displayed as expected.", flagUNL));
                }
                finally
                {
                    if (_claimAction.IsEditFlagAreaPresent())
                        _claimAction.ClickOnCancelLink();
                }
            }
        }


        [Test]//US67025
        public void Verify_view_delete_of_uploaded_documents_in_Claim_action_page_for_Client_User()
        {
            using (var automatedBase = new AutomatedBaseClientParallel(2))
            {
                ClaimActionPage _claimAction = null;
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var claimSequenceToViewDocs = paramLists["ClaimSequence"];
                var fileToUpload = paramLists["FileToUpload"].Split(';');
                var expectedSelectedFileTypeList =
                    paramLists["SelectedFileList"].Split(',').Select(x => x.Trim()).ToList();
                var docDescription = paramLists["DocDescription"];
                var auditDate = paramLists["AuditDate"];
                _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(claimSequenceToViewDocs);
                expectedSelectedFileTypeList.Sort();
                _claimAction.ClickOnClaimDocuments();
                var existingDocCount = _claimAction.GetFileUploadPage.DocumentCountOfFileList();
                var claimStatus = _claimAction.GetClaimStatus();

                try
                {

                    _claimAction.GetFileUploadPage.SetFileUploaderFieldValue("Description", docDescription);
                    _claimAction.GetFileUploadPage.SetFileTypeListVlaue("All");
                    _claimAction.GetFileUploadPage.AddFileForUpload(fileToUpload[0]);
                    _claimAction.GetFileUploadPage.ClickOnAddFileBtn();
                    _claimAction.GetFileUploadPage.ClickOnSaveUploadBtn();
                    _claimAction.GetFileUploadPage.IsDocumentDivPresent().ShouldBeTrue("Document List are present");
                    _claimAction.GetFileUploadPage.DocumentCountOfFileList()
                        .ShouldBeGreater(existingDocCount, "New document has been added");
                    _claimAction.GetFileUploadPage.IsFollowingAppealDocumentPresent(fileToUpload[0])
                        .ShouldBeTrue("Uploaded Document is listed");

                    Console.WriteLine("Verify the Status of the claim after the document is uploaded");
                    _claimAction.GetClaimStatus()
                        .ShouldBeEqual(claimStatus, "Verified Claim Status after Document Upload");

                    _claimAction.IsClaimDocumentDeleteIconPresent()
                        .ShouldBeFalse("Claim Document Delete Icon should not be present");

                    _claimAction.GetFileUploadPage.AppealDocumentsListAttributeValue(1, 1, 1)
                        .ShouldBeEqual(fileToUpload[0], "Document filename is displayed");
                    _claimAction.GetFileUploadPage.AppealDocumentsListAttributeValue(1, 1, 3)
                        .IsDateTimeInFormat()
                        .ShouldBeTrue("Document date is displayed and in proper format");
                    _claimAction.GetFileUploadPage.AppealDocumentsListAttributeValue(1, 2, 1)
                        .ShouldBeEqual(docDescription, "Document Description is displayed");

                    _claimAction.GetFileUploadPage.GetFileTypeAttributeListVaues(1, 1, 2)
                        .ShouldCollectionBeEqual(expectedSelectedFileTypeList, "Document fileType is displayed");
                    _claimAction.GetFileUploadPage.GetFileTypeAttributeListToolTip(1, 1, 2)
                        .ShouldCollectionBeEqual(expectedSelectedFileTypeList,
                            "Document fileType tooltip is displayed");

                    _claimAction.ClickOnDocumentToViewAndStayOnPage(1); //window opens to view  document 
                    _claimAction.GetOpenedDocumentText().ShouldBeEqual("test test", "document detail");
                    _claimAction.CloseDocumentTabPageAndBackToNewClaimAction();

                    Console.WriteLine("Verify the Status of the claim after the document is opened and closed");
                    _claimAction.GetClaimStatus().ShouldBeEqual
                        (claimStatus, "Verified Claim Status Before Document Deletion");
                }
                finally
                {
                    _claimAction.DeleteClaimDocumentRecord(claimSequenceToViewDocs, auditDate);
                }


            }

        }

        [Test]//US67025
        public void Verify_scroll_bar_is_displayed_in_Claim_Document_quadrant_if_the_list_goes_beyond_the_size_in_Claim_action_page_for_Client_User()
        {
            using (var automatedBase = new AutomatedBaseClientParallel(2))
            {
                ClaimActionPage _claimAction;
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var claimSequenceToVerifyScrollBar = automatedBase.DataHelper.GetSingleTestData
                    (FullyQualifiedClassName, TestExtensions.TestName, "ClaimSequenceWithDocs", "Value");
                _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(claimSequenceToVerifyScrollBar);
                _claimAction.ClickOnClaimDocuments();
                _claimAction.IsVerticalScrollBarPresentInClaimDocumentSection()
                    .ShouldBeTrue(
                        "Scrollbar Should display in Claim Documents Section when the list of documents extends out of the view");

            }
        }


        [Test]//US67012
        public void Verify_that_the_add_document_icon_will_be_displayed_on_Claim_Action_Page_if_documents_are_not_present_for_client_User()
        {
            using (var automatedBase = new AutomatedBaseClientParallel(2))
            {
                ClaimActionPage _claimAction;
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var claimSequenceWithoutDocs = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "ClaimSequenceWithoutDocs", "Value");
                _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(claimSequenceWithoutDocs);
                _claimAction.IsAddDocumentIconPresent().ShouldBeTrue("Is Add Document Icon present in Q2 quadrant?");
                _claimAction.IsRedBadgeDocumentIconPresent()
                    .ShouldBeFalse("Is Red badge present in upload Document icon?");
                _claimAction.ClickOnClaimDocuments();
                _claimAction.IsUploadDocumentContainerPresent().ShouldBeTrue("Upload Document Form should be present");
                _claimAction.IsUploadNewDocumentFormPresent()
                    .ShouldBeTrue("Is Claim Document container With Upload New Claim Document Form Present?");
                _claimAction.GetFileUploadPage.DocumentCountOfFileList()
                    .ShouldBeEqual(0, "There should not be any document for no docs claim");
            }
        }

        [Test]//US67012
        public void Verify_red_badge_over_the_documents_icon_represents_the_number_of_claim_documents_added_in_claim()
        {
            using (var automatedBase = new AutomatedBaseClientParallel(2))
            {
                ClaimActionPage _claimAction = null;
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var claimSequenceWithDocs = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "ClaimSequenceWithDocs", "Value");
                _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(claimSequenceWithDocs);
                var expectedClaimDocsCount = _claimAction.TotalCountofClaimDocsFromDatabase(claimSequenceWithDocs);
                _claimAction.GetClaimDocsCountInBadge().ShouldBeEqual(expectedClaimDocsCount,
                    "Red Badge text should equal to claim docs count from database");

                _claimAction.ClickOnClaimDocuments();
                _claimAction.IsUploadDocumentContainerPresent().ShouldBeTrue("Is Upload Document form present?");
                _claimAction.IsUploadNewDocumentFormPresent()
                    .ShouldBeFalse("Upload New Document form should not open when claim docs already exist.");
                _claimAction.GetFileUploadPage.DocumentCountOfFileList().ShouldBeEqual(
                    Convert.ToInt32(expectedClaimDocsCount), "Is Document count equal to database value");
                _claimAction.GetFileUploadPage.GetAddedDocumentList()
                    .IsInDescendingOrder()
                    .ShouldBeTrue("Most recenet uploaded files are on top");
                _claimAction.GetFileUploadPage.IsAddDocumentIconEnabled().ShouldBeTrue("Is Add Document icon enabled?");
                _claimAction.GetFileUploadPage.ClickOnClaimDocumentUploadIcon();
                _claimAction.GetFileUploadPage.IsAddDocumentIconDisabled()
                    .ShouldBeTrue("Is Add Document icon disabled? ");
                _claimAction.ClickOnUploadDocumentCancelBtn();
                _claimAction.IsUploadNewDocumentFormPresent()
                    .ShouldBeFalse("Add Document Form should not be present after clicking the Cancel button");
                _claimAction.IsUploadDocumentContainerPresent()
                    .ShouldBeTrue("Claim Document Container should be present");
            }
        }

        [Test]//US67012
        public void Verify_upload_new_claim_document_in_claim_action_for_client_User()
        {
            using (var automatedBase = new AutomatedBaseClientParallel(2))
            {
                ClaimActionPage _claimAction = null;
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> testData = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestExtensions.TestName);
                var claimSequence = testData["ClaimSequence"];
                var auditDate = testData["AuditDate"];
                var fileToUpload = testData["FileToUpload"];
                var fileType = testData["FileType"];
                var expectedSelectedFileTypeList = testData["SelectedFileList"].Split(',').ToList();
                var expectedSelectedFileType = testData["SelectedFileList"];
                var expectedFileTypeList = automatedBase.DataHelper
                    .GetMappingData(FullyQualifiedClassName, "File_Type_List").Values.ToList();
                var userName = testData["UserName"];
                var userType = testData["UserType"];
                var expectedDocumentHeaderText = testData["DocumentHeaderText"];
                _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(claimSequence);
                try
                {
                    _claimAction.GetClaimDocumentIconToolTip().ShouldBeEqual("Claim Documents",
                        "Expected claim document uploader icon tooltip is present");
                    _claimAction.ClickOnClaimDocuments();
                    _claimAction.WaitForWorkingAjaxMessage();
                    _claimAction.GetTopRightComponentTitle()
                        .ShouldBeEqual("Claim Documents", "Correct Claim Document Header is displayed.");
                    _claimAction.GetFileUploadPage.DocumentUploadHeaderText().ShouldBeEqual(expectedDocumentHeaderText,
                        "Expected Document Upload Header is displayed.");
                    _claimAction.GetFileUploadPage.IsClaimDocumentUploadEnabled()
                        .ShouldBeFalse("Is Add Document icon enabled?");
                    _claimAction.GetFileUploadPage.ClickOnCancelBtn();
                    _claimAction.IsUploadNewDocumentFormPresent()
                        .ShouldBeFalse("Add Document Form should not be present after clicking the Cancel button");
                    _claimAction.GetFileUploadPage.ClickOnClaimDocumentUploadIcon();
                    //_claimAction.GetFileUploadPage.IsCancelButtonDisabled().ShouldBeTrue("Is Cancel Button disabled?");
                    Console.WriteLine("Verify Maximum character in description");
                    var descp = new string('b', 105);
                    _claimAction.GetFileUploadPage.SetFileUploaderFieldValue("Description", descp);
                    _claimAction.GetFileUploadPage.GetFileUploaderFieldValue(2)
                        .Length.ShouldBeEqual(100, "Character length should not exceed more than 100 in Description");
                    _claimAction.GetFileUploadPage.IsAddFileButtonDisabled()
                        .ShouldBeTrue(
                            "Add file button should be disabled (unless atleast a file has been uploaded) must be true?");
                    _claimAction.GetFileUploadPage.ClickOnSaveUploadBtn();
                    _claimAction.IsPageErrorPopupModalPresent()
                        .ShouldBeTrue("Page error pop up should be present if no file type is selected");
                    _claimAction.GetPageErrorMessage()
                        .ShouldBeEqual("At least one file must be uploaded before the changes can be saved.",
                            "Expected error message on zero file type selection");
                    _claimAction.ClosePageError();

                    /* --------- add single file only since multiple file upload with selenium is successfull locally only -----*/
                    _claimAction.GetFileUploadPage.AddFileForUpload(fileToUpload.Split(';')[0]);
                    _claimAction.GetFileUploadPage.IsAddFileButtonDisabled()
                        .ShouldBeFalse(
                            "Add file button should be enabled (when atleast a file has been uploaded) must be false?");
                    _claimAction.GetFileUploadPage.ClickOnAddFileBtn();
                    _claimAction.IsPageErrorPopupModalPresent()
                        .ShouldBeTrue("Page error pop up should be present if no file type is selected");
                    _claimAction.GetPageErrorMessage()
                        .ShouldBeEqual(
                            "At least one Document Type selection is required before the files can be added.",
                            "Expected error message on zero file type selection");
                    _claimAction.ClosePageError();
                    _claimAction.GetFileUploadPage.GetAvailableFileTypeList()
                        .ShouldCollectionBeEqual(expectedFileTypeList, "File Type List Equal");
                    _claimAction.GetFileUploadPage.SetFileTypeListVlaue(expectedSelectedFileTypeList[0]);
                    _claimAction.GetFileUploadPage.GetPlaceHolderValue()
                        .ShouldBeEqual("Provider Letter", "File Type Text");
                    _claimAction.GetFileUploadPage.SetFileTypeListVlaue(expectedSelectedFileTypeList[1]);
                    _claimAction.GetFileUploadPage.GetPlaceHolderValue()
                        .ShouldBeEqual("Multiple values selected", "File Type Text when multiple value selected");
                    _claimAction.GetFileUploadPage.GetSelectedFileTypeList()
                        .ShouldCollectionBeEqual(expectedSelectedFileTypeList, "Selected File List Equal");
                    _claimAction.GetFileUploadPage.SetFileUploaderFieldValue("Description", "Test Description");
                    _claimAction.GetFileUploadPage.GetSelectedFilesValue()
                        .ShouldCollectionBeEqual(fileToUpload.Split(';').ToList(),
                            "Expected multiples files list is present");
                    _claimAction.GetFileUploadPage.ClickOnAddFileBtn();
                    _claimAction.GetFileUploadPage.ClaimFileToUploadDocumentValue(1, 2)
                        .ShouldBeEqual(fileToUpload.Split(';')[0],
                            "Document correct and present under files to upload");
                    _claimAction.GetFileUploadPage.ClaimFileToUploadDocumentValue(1, 3)
                        .ShouldBeEqual("Test Description",
                            "Document Description is correct and present under files to upload.");
                    _claimAction.GetFileUploadPage.ClaimFileToUploadDocumentValue(1, 4)
                        .ShouldBeEqual("Multiple File Types",
                            "Document Type text when multiple File Types is selected.");
                    _claimAction.GetFileUploadPage.GetFileToUploadTooltipValue(1, 4)
                        .ShouldBeEqual(expectedSelectedFileType,
                            "Document Type correct and present under files to upload");
                    _claimAction.IsPageErrorPopupModalPresent()
                        .ShouldBeFalse(
                            "Page error pop up should not be present if no description is set as its not a required field");
                    _claimAction.GetFileUploadPage.ClickOnSaveUploadBtn();
                    _claimAction.GetFileUploadPage.IsDocumentDivPresent().ShouldBeTrue("Document List are present");
                    _claimAction.IsUploadNewDocumentFormPresent()
                        .ShouldBeFalse("Upload New document div should be closed after uploading document.");
                    _claimAction.GetFileUploadPage.IsFollowingAppealDocumentPresent(fileToUpload.Split(';')[0])
                        .ShouldBeTrue("Uploaded Document is listed");
                    var expectedDate = _claimAction.GetFileUploadPage.AppealDocumentsListAttributeValue(1, 1, 3);

                    StringFormatter.PrintMessageTitle(
                        "Validation of Audit Record to display in claim Processing History after document upload.");
                    _claimAction.IsClaimAuditAddedForDocumentUpload(claimSequence)
                        .ShouldBeTrue("Claim Audit for Document Upload is added in database ");
                    ClaimProcessingHistoryPage claimProcessingHistory =
                        _claimAction.ClickOnClaimProcessingHistoryAndSwitch();
                    var date = claimProcessingHistory.GetClaimHistoryTableDataValueByRowCol(1, 1);
                    date.IsDateTimeInFormat().ShouldBeTrue("Upload Date time is in correct format");
                    var actualDate = Convert.ToDateTime(date);
                    var expectDate = Convert.ToDateTime(expectedDate);
                    actualDate.AddSeconds(-actualDate.Second)
                        .ShouldBeEqual(expectDate.AddSeconds(-expectDate.Second), "Correct Audit Date is displayed.");
                    claimProcessingHistory.GetClaimHistoryTableDataValueByRowCol(1, 2)
                        .ShouldBeEqual("Document Upload", " Correct Audit Action is displayed.");
                    var currentUser = claimProcessingHistory.GetClaimHistoryTableDataValueByRowCol(1, 4);
                    currentUser.DoesNameContainsOnlyFirstWithLastname()
                        .ShouldBeTrue("Username Should be in <FirstName> <LastName> (userid)");
                    currentUser.ShouldBeEqual(userName, "Username Should be in <FirstName> <LastName> (userid)");
                    claimProcessingHistory.GetClaimHistoryTableDataValueByRowCol(1, 5)
                        .ShouldBeEqual(userType, "Correct User Type is diplayed.");
                    claimProcessingHistory.GetClaimHistoryTableDataValueByRowCol(1, 6)
                        .ShouldBeEqual("Unreviewed", "Correct Claim Status is displayed.");
                    _claimAction.CloseAllExistingPopupIfExist();
                    var existingDocCount = _claimAction.GetFileUploadPage.DocumentCountOfFileList();
                    _claimAction.GetFileUploadPage.ClickOnClaimDocumentUploadIcon();
                    _claimAction.GetFileUploadPage.IsDocumentUploadSectionPresent()
                        .ShouldBeTrue("Claim Document Uploader Section is displayed");
                    _claimAction.GetFileUploadPage.SetFileTypeListVlaue(fileType);
                    _claimAction.GetFileUploadPage.AddFileForUpload(fileToUpload.Split(';')[0]);
                    _claimAction.GetFileUploadPage.ClickOnAddFileBtn();
                    var filesToUploadCount = _claimAction.GetFileUploadPage.GetFilesToUploadCount();
                    _claimAction.GetFileUploadPage.ClaimFileToUploadDocumentValue(1, 2)
                        .ShouldBeEqual(fileToUpload.Split(';')[0],
                            "Document correct and present under files to upload");
                    _claimAction.GetFileUploadPage.ClickOnDeleteIconInFilesToUpload(1);
                    _claimAction.GetFileUploadPage.GetFilesToUploadCount()
                        .ShouldBeLess(filesToUploadCount, "Document had been removed.");
                    _claimAction.GetFileUploadPage.ClickOnCancelBtn();

                    // selecting cancel closed form and discards added files
                    _claimAction.GetFileUploadPage.IsDocumentDivPresent().ShouldBeTrue("Document List are present");
                    _claimAction.GetFileUploadPage.DocumentCountOfFileList()
                        .ShouldBeEqual(existingDocCount,
                            "Added Files has been discarded and has not been associated to appeal");
                    _claimAction.CloseAnyPopupIfExist();
                }
                finally
                {
                    _claimAction.DeleteClaimDocumentRecord(claimSequence, auditDate);
                }
            }
        }


        [Test] //US69379
        public void Validation_test_to_create_appeal_from_new_claim_action_for_client_unreviewed_claims()
        {
            using (var automatedBase = new AutomatedBaseClientParallel(2))
            {
                ClaimActionPage _claimAction = null;
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> testData = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestExtensions.TestName);
                var claimSeq = testData["ClaimSequence"];
                var disabledTooltip = testData["ToolTipForCUrDisabledAppealType"];
                _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(claimSeq);
                _claimAction.GetClaimStatus().ShouldBeEqual(ClaimStatusTypeEnum.ClientUnreviewed.GetStringValue(),
                        "Status of claim is as required: client unreviewed.");
                var appealCreator = _claimAction.ClickOnCreateAppealIcon();
                _claimAction.WaitForPageToLoad();
                appealCreator.GetPageHeader().ShouldBeEqual(PageHeaderEnum.AppealCreator.GetStringValue(),
                    "Page header text equals Appeal creator");
                appealCreator.IsRespectiveAppealTypeSelected("R")
                    .ShouldBeTrue("Record Review should be selected by default for Client Unreviewed claims");
                appealCreator.IsAppealTypeDisabled(AppealType.Appeal.GetStringValue())
                    .ShouldBeTrue("Appeal type as Appeal should be disabled for Client Unreviewed claims");
                appealCreator.GetToolTipForDisabledAppealType(AppealType.Appeal.GetStringValue())
                    .ShouldBeEqual(disabledTooltip,
                        "Tool tip message for client Unreviewed claims record type selection");
                appealCreator.ClickOnCancelBtn();
                _claimAction.GetPageHeader().ShouldBeEqual(PageHeaderEnum.ClaimAction.GetStringValue(),
                    "Page returns to claim action");

            }
        }

        [Test] //US69379
        public void Validation_test_to_create_appeal_from_new_claim_action_for_client_reviewed_claims()
        {
            using (var automatedBase = new AutomatedBaseClientParallel(2))
            {
                ClaimActionPage _claimAction = null;
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> testData = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestExtensions.TestName);
                var claimSeq = testData["ClaimSequence"];
                var disabledTooltipWithAppeal = testData["ToolTipForClaimWithAppealProcess"];
                var lockedClaimMessageTooltip = testData["LockedClaimMessage"];
                _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(claimSeq);
                DeletePreviousAppeals(claimSeq,automatedBase.CurrentPage,_claimAction); //to delete appeal created
                try
                {
                    var appealCreator = _claimAction.ClickOnCreateAppealIcon();
                    appealCreator.WaitForPageToLoad();
                    appealCreator.GetPageHeader().ShouldBeEqual(PageHeaderEnum.AppealCreator.GetStringValue(),
                        "Page header text equals Appeal creator");
                    appealCreator.IsRespectiveAppealTypeSelected(AppealType.Appeal.GetStringValue())
                        .ShouldBeTrue("Appeal type record should be selected by default for Client reviewed claims");
                    appealCreator.IsAppealTypeDisabled(AppealType.Appeal.GetStringValue())
                        .ShouldBeFalse("Appeal type as Appeal should not be disabled for client reviewed claims");
                    appealCreator.SelectRecordReviewRecordType();
                    appealCreator.IsRespectiveAppealTypeSelected("R")
                        .ShouldBeTrue("Record Review type should be selectable for Client reviewed claims");
                    appealCreator.SelectAppealRecordType();
                    appealCreator.IsRespectiveAppealTypeSelected(AppealType.Appeal.GetStringValue())
                        .ShouldBeTrue("Appeal record type should be selectable for Client reviewed claims");
                    appealCreator.SelectClaimLine();
                    appealCreator.CreateAppeal(ProductEnum.CV.GetStringValue(), "XYZ123");
                    _claimAction.GetPageHeader().ShouldBeEqual(PageHeaderEnum.ClaimAction.GetStringValue(),
                        "Page should return to claim action after appeal is created.");
                    _claimAction.IsAddAppealIconDisabled()
                        .ShouldBeTrue("Create appeal icon should be disabled for claims having appeals");
                    _claimAction.GetToolTipMessageDisabledCreateAppealIcon().ShouldBeEqual(disabledTooltipWithAppeal,
                        "Tooltip of Create Appeal Icon for Claim with having appeals should be equal");
                    _claimAction.IsClaimLocked().ShouldBeTrue("Claim Lock Icon Present?");
                    var tooltip = _claimAction.GetLockIConTooltip();
                    tooltip.ShouldBeEqual(lockedClaimMessageTooltip, "ToolTip Message:");

                }
                finally
                {
                    DeletePreviousAppeals(claimSeq, automatedBase.CurrentPage, _claimAction); //to delete appeal created
                }
            }

        }


       [Test, Category("OnDemand")] //CAR-278 + TANT-189
       [NonParallelizable]
        public void Verify_Modification_of_Core_Edits_Claims()
        {
            using (var automatedBase = new AutomatedBaseClientParallel(2))
            {
                ClaimActionPage _claimAction = null;
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var claseqWithCoreFlagsClaim = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "claseqWithCoreFlagsClaim", "Value");
                _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(claseqWithCoreFlagsClaim);
                try
                {
                    StringFormatter.PrintMessageTitle(
                        $"Disabling {ConfigurationSettingsEnum.ModifyCoreFlag.GetStringValue()}");
                    _claimAction.GetCommonSql.UpdateSpecificClientDataInDB("CAN_MODIFY_CORE_FLAGS='F'",
                        ClientEnum.SMTST.ToString());

                    StringFormatter.PrintMessageTitle(
                        Format(
                            $"Verifying users cannot modify claims on core flags when {ConfigurationSettingsEnum.ModifyCoreFlag.GetStringValue()} is disabled"));
                    _claimAction.RefreshPage(false);
                    _claimAction.IsDeleteIconDisabledByLinenoAndRow(1, 1).ShouldBeTrue(
                        Format(
                            $"Delete icon for core flag should be disabled when {ConfigurationSettingsEnum.ModifyCoreFlag.GetStringValue()} is Disabled"));
                    _claimAction.IsRestoreIconDisabledByLinenoAndRow(1, 2).ShouldBeTrue(
                        Format(
                            $"Restore icon for core flag should be disabled when {ConfigurationSettingsEnum.ModifyCoreFlag.GetStringValue()} is Disabled"));
                    _claimAction.IsDeleteRestoreIconRowByRowDisabled(1).ShouldBeTrue(
                        Format(
                            $"Delete icon for all flags on line should be disabled when {ConfigurationSettingsEnum.ModifyCoreFlag.GetStringValue()} is Disabled"));
                    _claimAction.IsDeleteRestoreAllFlagsIconCssLocator().ShouldBeTrue(
                        Format(
                            $"Delete icon for all flags  should be disabled when {ConfigurationSettingsEnum.ModifyCoreFlag.GetStringValue()} is Disabled"));

                    StringFormatter.PrintMessageTitle(
                        Format($"Enabling {ConfigurationSettingsEnum.ModifyCoreFlag.GetStringValue()}"));
                    _claimAction.GetCommonSql.UpdateSpecificClientDataInDB("CAN_MODIFY_CORE_FLAGS='T'",
                        ClientEnum.SMTST.ToString());

                    StringFormatter.PrintMessageTitle(
                        Format(
                            $"Verifying users can modify claim on core flags when {ConfigurationSettingsEnum.ModifyCoreFlag.GetStringValue()} is Enabled"));
                    _claimAction.RefreshPage(false);

                    _claimAction.IsDeleteIconDisabledByLinenoAndRow(1, 1)
                        .ShouldBeFalse(Format(
                            $"Delete icon for core flags should not be disabled when {ConfigurationSettingsEnum.ModifyCoreFlag.GetStringValue()} Enabled"));
                    _claimAction.ClickOnEditIconFlagLevelForLineEdit(1, "FRE");
                    _claimAction.IsEditFlagDeleteRestoreButtonDisabled()
                        .ShouldBeFalse(Format(
                            $"Normal Delete icon for core flags should not be disabled when {ConfigurationSettingsEnum.ModifyCoreFlag.GetStringValue()} Enabled"));
                    _claimAction.ClickOnCancelLink();
                    _claimAction.ClickOnEditIconFlagLevelForLineEdit(1, "PFOT");

                    _claimAction.IsEditFlagDeleteRestoreButtonDisabled(true)
                        .ShouldBeFalse(Format($"Normal Restore icon for core flags should not be disabled when " +
                                              $"{ConfigurationSettingsEnum.ModifyCoreFlag.GetStringValue()} Enabled"));

                    _claimAction.ClickOnCancelLink();
                    _claimAction.IsRestoreIconDisabledByLinenoAndRow(1, 2).ShouldBeFalse(
                        Format(
                            $"Restore icon for core flags should not be disabled when {ConfigurationSettingsEnum.ModifyCoreFlag.GetStringValue()} Enabled"));
                    _claimAction.IsDeleteIconRowByRowDisabled(1).ShouldBeFalse(Format(
                        $"Delete icon for all flags on line should be enabled when {ConfigurationSettingsEnum.ModifyCoreFlag.GetStringValue()} is Enabled"));
                    _claimAction.IsDeleteAllFlagsPresent(true).ShouldBeTrue(Format(
                        $"Delete icon for all flags on line should be enabled when {ConfigurationSettingsEnum.ModifyCoreFlag.GetStringValue()} is Enabled"));
                }
                finally
                {
                    StringFormatter.PrintMessageTitle(
                        $"Finally Block. \n Reverting {ConfigurationSettingsEnum.ModifyCoreFlag.GetStringValue()} to False");
                    _claimAction.GetCommonSql.UpdateSpecificClientDataInDB("CAN_MODIFY_CORE_FLAGS='F'",
                        ClientEnum.SMTST.ToString());
                }
            }
        }

        // [Test]//CAR-423 + CAR-736
        public void Verify_cvp_flag_details_container_upon_selecting_flagged_line()
        {
            using (var automatedBase = new AutomatedBaseClientParallel(2))
            {
                ClaimActionPage _claimAction = null;
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> testData = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestExtensions.TestName);
                var claimSeq = testData["ClaimSequence"];
                var claimSequenceHavingDefenseCodeOutOfRange = testData["ClaimSequenceHavingDefenseCodeOutOfRange"];
                var flagSeq = testData["FlagSeq"].Split(';').ToList();
                var cvpFlag = testData["CVPFlag"].Split(';').ToList();
                _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(claimSeq);
                _claimAction.ClickOnFlagLineByFlag(cvpFlag[0]);
                _claimAction.IsDefenseCodePresent(flagSeq[0]).ShouldBeTrue("Is defense code present?");
                _claimAction.IsCVPFlagContainerPresent().ShouldBeTrue(
                    "Selecting flag line of CVP flag must display a new container above existing flag audit information");
                _claimAction.GetFlagDetailDefenseRationale()
                    .ShouldBeEqual(_claimAction.GetDefenseRationaleOfFlagseqBasedOnEffectiveDateAndDOS(flagSeq[0]),
                        "Defense Rationale must be equal to database value");
                _claimAction.GetFlagDetailLineNumberForCvpFlag().ShouldBeEqual(
                    _claimAction.GetLineNoOfSelectedFlaggedLine(),
                    "Line no displayed for CVP flag container on Flag detail section should be equal to selected flagged line no");
                _claimAction.GetFlagDetailEditForCvpFlag().ShouldBeEqual(_claimAction.GetEditOfSelectedFlaggedLine(),
                    "Line no displayed for CVP flag container on Flag detail section should be equal to selected flagged line no");

                _claimAction.ClickOnFlagLineByFlag("FRE");
                _claimAction.IsCVPFlagContainerPresent().ShouldBeFalse("Is Flag Detail for CVP flag present?");

                StringFormatter.PrintMessage(
                    "Verifying Flagged line with defense code but DOS doesn't lie between effective and expire date of defense code");
                SearchByClaimSeqFromWorkListForClientUser(claimSequenceHavingDefenseCodeOutOfRange,_claimAction);
                _claimAction.ClickOnFlagLineByFlag(cvpFlag[1]);
                _claimAction.IsDefenseCodePresent(flagSeq[1]).ShouldBeTrue("Is Defense Code Present");
                _claimAction.IsCVPFlagContainerPresent().ShouldBeFalse(
                    "Selecting flag line of CVP flag must not display a new container above existing flag audit information");
                _claimAction.GetDefenseRationaleOfFlagseqBasedOnEffectiveDateAndDOS(flagSeq[1])
                    .ShouldBeNullorEmpty("DOS of flagged line doesn't lie effective and expire date of defense code ");
            }

        }

        [Test, Category("SchemaDependent")]//CAR-423 + CAR-736 + CAR 984 +CAR-781
        public void Verify_CVP_defense_information_in_flag_details_for_non_batch_client()
        {
            using (var automatedBase = new AutomatedBaseClientParallel(2))
            {
                ClaimActionPage _claimAction = null;
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var claimSequence = paramLists["ClaimSequence"];
                var claimSeqWithNoEditText = paramLists["ClaimSequenceWithNoEditText"];
                var claimSequenceHavingDefenseCodeOutOfRange = paramLists["ClaimSequenceHavingDefenseCodeOutOfRange"];
                var flagSeq = paramLists["FlagSeq"];
                List<string> flagList = paramLists["Flag"].Split(';').ToList();
             
                StringFormatter.PrintMessageTitle("Verify edit text for a claim with a different Trigger claseq");
                automatedBase.CurrentPage.ClickOnSwitchClient().SwitchClientTo(ClientEnum.RPE, true);
                _claimAction = automatedBase.CurrentPage.NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(claimSequence);

                Verify_labels_and_values_in_the_flag_details_for_particular_flag(claimSequence, flagList[0],_claimAction);

                StringFormatter.PrintMessageTitle("Verify EOB for a claim with no edit text");
                SearchByClaimSeqFromWorkListForClientUser(claimSeqWithNoEditText,_claimAction);
                _claimAction.GetEOBMessage(flagList[1])
                    .ShouldBeEqual(string.Format(_claimAction.GetEOBMessageFromDatabase(flagList[1]), "1", "15922",
                        "REMOVAL OF TAIL BONE ULCER"));

                StringFormatter.PrintMessage(
                    "Verifying Flagged line with defense code but DOS doesn't lie between effective and expire date of defense code");
                SearchByClaimSeqFromWorkListForClientUser(claimSequenceHavingDefenseCodeOutOfRange,_claimAction);
                _claimAction.ClickOnFlagLineByFlag(flagList[2]);
                _claimAction.IsDefenseCodePresent(flagSeq).ShouldBeTrue("Is Defense Code Present");
                _claimAction.IsCVPFlagContainerPresent().ShouldBeTrue(
                    "Selecting flag line of CVP flag must  display a new container above existing flag audit information");
                _claimAction
                    .GetDefenseRationaleOfFlagseqBasedOnEffectiveDateAndDOS(claimSequenceHavingDefenseCodeOutOfRange)
                    .ShouldBeNullorEmpty("DOS of flagged line doesn't lie effective and expire date of defense code ");
            }
        }

        [Test]//CAR 1140
        public void Verify_triggerclaim_hyperlink_for_flag_that_has_triggerclaim_onthesameline_differentlinesameclaim_difflinediffclaim()
        {
            using (var automatedBase = new AutomatedBaseClientParallel(2))
            {
                ClaimActionPage _claimAction;
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var claimSequenceWithSamTrigClasSeqOnSameLine = paramLists["ClaimSequence1"];
                var claimSequence = paramLists["ClaimSequence2"];
                var triggerClaSeq = paramLists["TriggerClaSeq"];

                StringFormatter.PrintMessageTitle(
                    "Verify No Trigger Claim No Exists on Flagged Line for having Same Trigger Claseq with same line");
                _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch().
                    SearchByClaimSequenceToNavigateToClaimActionPage(claimSequenceWithSamTrigClasSeqOnSameLine);
                _claimAction.IsTriggerAltClaimOnFlagLinesClickable(2)
                    .ShouldBeFalse("Is Trriger Claim No display for same trigger claseq with same line");

                StringFormatter.PrintMessageTitle(
                    "Verify Claim Number is hyperlink or not based on Diffter Trriger Claseq/Same Claseq with different line");
                SearchByClaimSeqFromWorkListForClientUser(claimSequence,_claimAction);
                _claimAction.IsTriggerAltClaimOnFlagLinesClickable(1)
                    .ShouldBeTrue(
                        "TriggerAltClaimNo should be a hyperlink when trigger claim is NOT the current claim");
                _claimAction.GetClaimSequenceFromClaimActionPopupOnClickTriggerAltClaimNo(triggerClaSeq, 1)
                    .ShouldBeEqual(triggerClaSeq, "Hyperlink opens to a different trigger claseq");
                _claimAction.IsTriggerAltClaimOnFlagLinesClickable(2)
                    .ShouldBeFalse("Is Trriger Claim No hyperlink for same trigger claseq with different line");
            }

        }

        [Test] //CAR-1799(CAR-1098)
        public void Verify_Pre_Auth_Action_Popup_version()
        {
            using (var automatedBase = new AutomatedBaseClientParallel(2))
            {
                ClaimActionPage _claimAction = null;
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var claimSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "ClaimSequence", "Value");
                _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(claimSeq);
                try
                {
                    var _claimHistory = _claimAction.ClickOnHistoryButtonAndSwitchToPatientClaimHistoryPage();
                    _claimHistory.IsPreAuthIconPresentInHeaderLevel().ShouldBeTrue("Is Pre Auth Icon Display?");
                    var openedWindowCountBefore = _claimHistory.GetWindowHandlesCount();

                    var patientPreAuthHx = _claimHistory.ClickOnPreAuthIconAndNavigateToPatientPreAuthHistory();
                    patientPreAuthHx.PageTitle.ShouldBeEqual(PageTitleEnum.PatientPreAuthHistory.GetStringValue(),
                        $"Page Should redirect to {PageHeaderEnum.PatientPreAuthHistory.GetStringValue()}");
                    patientPreAuthHx.GetPopupPageTitle().ShouldBeEqual(
                        PageHeaderEnum.PatientPreAuthHistory.GetStringValue(),
                        $"Popup page should be {PageHeaderEnum.PatientPreAuthHistory.GetStringValue()}");
                    var preAuthSeq = patientPreAuthHx.GetAuthSeq();
                    var preAuthActionPopup = patientPreAuthHx.ClickOnAuthSeqLinkAndNavigateToPreAuthActionPopup();
                    preAuthActionPopup.GetWindowHandlesCount().ShouldBeEqual(openedWindowCountBefore,
                        "Pre-Auth Action page should replace the Patient Claim History window");

                    StringFormatter.PrintMessageTitle("Verification of Pre Auth Action Popup Version");
                    preAuthActionPopup.GetUpperLeftQuadrantValueByLabel("Auth Seq")
                        .ShouldBeEqual(preAuthSeq, "Auth Seq should be displayed correctly");
                    preAuthActionPopup.IsNucleusHeaderPresent().ShouldBeFalse("Nucleus Header Should not display");
                    preAuthActionPopup.IsReturnToSearchIconEnabled()
                        .ShouldBeFalse("Return To Search Icon should disabled");
                    preAuthActionPopup.IsApproveIconEnabled().ShouldBeFalse("Approve Icon should disabled");
                    preAuthActionPopup.IsTransferIconDisabled().ShouldBeTrue("Transfer Icon should disabled");
                    preAuthActionPopup.IsNextIconDisabled().ShouldBeTrue("Next Icon should disabled");
                    preAuthActionPopup.IsAllEditIconDisabled()
                        .ShouldBeTrue("All Edit Flag Icon in flagged lines should be disabled");
                    //preAuthActionPopup.IsEditDentalRecordIconDisabled().ShouldBeTrue("Edit Line Item Icon should disabled");

                }
                finally
                {
                    _claimAction.CloseAnyTabIfExist();
                }
            }

        }

        [Test] //+ CAR-2650(CAR-2622)
        [NonParallelizable]
        public void Verify_that_change_in_sug_unit_change_only_client_sug_unit()
        {
            using (var automatedBase = new AutomatedBaseClientParallel(2))
            {
                ClaimActionPage _claimAction;
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var claimSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName,
                    "ClaimSequence", "Value");
                var flag = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                    "Flag",
                    "Value");
                var reasonCode = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName,
                    "ReasonCode", "Value");
                _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(claimSeq);
                var rnd = new Random();
                var preSugUnits = _claimAction.GetInternalAndClientSugUnitsByClaseqAndFlag(claimSeq, flag);
                var sugUnits = rnd.Next(2, 590);
                while (preSugUnits[0] == sugUnits || preSugUnits[1] == sugUnits)
                {
                    sugUnits = rnd.Next(2, 590);
                }
                var flagLine = _claimAction.GetFlagLineValue(2);
                _claimAction.ClickOnClaimDollarDetailsIcon();
                _claimAction.WaitForStaticTime(200);
                var allowed = _claimAction.GetAllowedValueByLine(flagLine);
                var units = _claimAction.GetUnitValueOnFlag(2);
                _claimAction.ClickOnEditIconFlagLevelForLineEdit(300, flag);
                _claimAction.EnterSugUnits(sugUnits.ToString());
                _claimAction.SelectReasonCode(reasonCode);
                _claimAction.GetSugPaid()
                    .ShouldBeEqual(Math.Round(allowed / units * sugUnits, 2, MidpointRounding.AwayFromZero),
                        "Sug Paid calculation is correct after modification of Sug Units instantly in input field");
                _claimAction.SaveFlag(reasonCode);
                var newSugUnits = _claimAction.GetInternalAndClientSugUnitsByClaseqAndFlag(claimSeq, flag);
                newSugUnits[0].ShouldBeEqual(preSugUnits[0], "HCISUGUNITS should not be changed");
                newSugUnits[1].ShouldNotBeEqual(preSugUnits[1], "CLISUGUNITS should be changed");
                newSugUnits[1].ShouldBeEqual(sugUnits, "CLISUGUNITS should  be equal to changed value");
            }

        }

        #endregion

        #region Private Methods

        void SearchByClaimSeqFromWorkListForClientUser(string claimSeq, ClaimActionPage _claimAction)
        {
            _claimAction.WaitForIe(3000);
            if (_claimAction.IsPageHeaderPresent())
            {
                if (!_claimAction.IsWorkListControlDisplayed())
                    _claimAction.ClickWorkListIcon();
            }
            _claimAction.WaitForStaticTime(2000);
            if (!_claimAction.IsWorkListControlDisplayed())
                _claimAction.ClickWorkListIcon();
            _claimAction.ClickSearchIcon()
            .SearchByClaimSequence(claimSeq);
            //_claimAction.ClickWorkListIcon();
        }

        void RestoreDeletedFlagsBySpecificLineEdit(IEnumerable<KeyValuePair<string, string>> lineEditFlagPairs,ClaimActionPage _claimAction)
        {
            foreach (var lineEditFlagPair in lineEditFlagPairs)
            {
                _claimAction.RestoreSpecificLineEditFlagByClient(lineEditFlagPair.Key, lineEditFlagPair.Value);
            }
        }


        int GetCountOfDeletedFlagsBySpecificLineEdit(IEnumerable<KeyValuePair<string, string>> lineEditFlagPairs,ClaimActionPage _claimAction)
        {

            return
                lineEditFlagPairs.Sum(
                        lineEditFlagPair =>
                            _claimAction.GetDeletedLineFlagCountForClient(lineEditFlagPair.Key, lineEditFlagPair.Value));

        }

        void VerifyGivenFlagsDeleteIconDisabled(IEnumerable<KeyValuePair<string, string>> lineEditFlagPairs,ClaimActionPage _claimAction)
        {
            foreach (var lineEditFlagPair in lineEditFlagPairs)
            {
                _claimAction.IsDeleteFlagDisabledForLineEdit(lineEditFlagPair.Key, lineEditFlagPair.Value)
                    .ShouldBeTrue("Line: " + lineEditFlagPair.Key + ", Edit: " + lineEditFlagPair.Value + " is Disabled");
            }
        }


        private void DeletePreviousAppeals(string claimSeq,NewDefaultPage CurrentPage,ClaimActionPage _claimAction)
        {
            AppealManagerPage _appealManager = null;
            if (CurrentPage.GetPageHeader() != "Appeal Manager")
            {
                _appealManager = _claimAction.Logout().LoginAsHciAdminUser().NavigateToAppealManager();

            }
            _appealManager.DeleteAppealsAssociatedWithClaim(claimSeq);
            _claimAction.Logout().LoginAsClientUser4().NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(claimSeq);
        }

        private void Verify_labels_and_values_in_the_flag_details_for_particular_flag(string claimSeq, string flag,ClaimActionPage _claimAction)
        {
            var expectedFlagDetails = _claimAction.GetFlagDetailsList(ClientEnum.RPE.ToString(), claimSeq);
            _claimAction.ClickOnFlagLineByFlag(flag);
            _claimAction.IsCVPFlagContainerPresent().ShouldBeTrue(
                "Selecting flag line of flag must display a new container above existing flag audit information");
            _claimAction.GetFlagDetailLineNumberForCvpFlag().ShouldBeEqual(
                _claimAction.GetLineNoOfSelectedFlaggedLine(),
                "Line no displayed for flag container on Flag detail section should be equal to selected flagged line no");
            _claimAction.IsRuleIdLabelPresent().ShouldBeTrue("Rule ID label should be present");
            _claimAction.GetFlagDetailsLabel(1).ShouldBeEqual("Author:");
            _claimAction.GetFlagDetailsLabel(2).ShouldBeEqual("Source:");
            _claimAction.GetFlagDetailsLabel(3).ShouldBeEqual("Defense Rationale:");
            _claimAction.GetFlagDetailsValueFirstRow(3)
                .ShouldBeEqual(
                    expectedFlagDetails[0],
                    "Rule ID should match with the database");
            _claimAction.GetFlagDetailsValueFirstRow(2).ShouldBeEqual(flag, "Flag values should match ");
            _claimAction.GetFlagDetailsValueSecondRow(1)
                .ShouldBeEqual(expectedFlagDetails[1],"Author Value should match with the database");
            _claimAction.GetFlagDetailsValueSecondRow(2)
                .ShouldBeEqual(expectedFlagDetails[2],"Source Value should match with the database");
            _claimAction.GetFlagDetailsValueSecondRow(3)
                .ShouldBeEqual(_claimAction.GetDefenseRationaleOfFlagseqBasedOnEffectiveDateAndDOS(claimSeq),
                    "Defense Rationale Value should match with the database");
            _claimAction.GetEOBMessage(flag)
                .AssertIsContained(expectedFlagDetails[3], string.Format("{0} EOB message displayed as expected.", flag));

        }

        #endregion
    }
}

using Nucleus.Service.Data;
using Nucleus.Service.PageObjects.Claim;
using Nucleus.Service.PageServices.Appeal;
using Nucleus.Service.PageServices.Claim;
using Nucleus.Service.PageServices.Settings;
using Nucleus.Service.PageServices.Settings.User;
using Nucleus.Service.Support.Common.Constants;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Environment;
using Nucleus.Service.Support.Menu;
using Nucleus.UIAutomation.TestSuites.Base;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Nucleus.Service.PageObjects.Default;
using Nucleus.Service.PageServices.Base.Default;
using Nucleus.Service.PageServices.QuickLaunch;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Utils;
using static System.String;

namespace Nucleus.UIAutomation.TestSuites.TestCases
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class ClaimAction3
    {
        //#region PRIVATE FIELDS


        //private ClaimActionPage _claimAction;
        //private AppealActionPage _appealAction;
        //private NewPopupCodePage _newpopupCodePage;
        //private ClientSearchPage _clientSearch;

        //private ClaimSearchPage _claimSearch;
        //private ProfileManagerPage _profileManager;
        //private string claimSeq;
        //#endregion

        //#region PROTECTED PROPERTIES

        protected string FullyQualifiedClassName
        {
            get
            {
                return GetType().FullName;
            }
        }
        //#endregion

        //#region OVERRIDE METHODS

        //protected override void ClassInit()
        //{
        //    try
        //    {
        //        UserLoginIndex = 3;
        //        base.ClassInit();
        //        TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
        //        IDictionary<string, string> paramLists = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
        //        claimSeq = paramLists["ClaimSequence"];
        //        //automatedBase.QuickLaunch.Logout().LoginAsHciAdminUser3();                
        //        _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(claimSeq);
        //        _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
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
        //    if (_claimAction.IsClaimLocked())
        //        Console.WriteLine("Claim is Locked!");
        //    if (_claimAction.IsPageErrorPopupModalPresent())
        //        _claimAction.ClosePageError();

        //    if (_claimAction.IsClaimProcessingHistoryOpen())
        //        _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();

        //    base.TestCleanUp();
        //    if (string.Compare(UserType.CurrentUserType, UserType.HCIADMIN3, StringComparison.OrdinalIgnoreCase) != 0)
        //    {
        //        _claimAction = _claimAction.Logout()
        //            .LoginAsHciAdminUser3().NavigateToClaimSearch()
        //            .SearchByClaimSequenceToNavigateToClaimActionPage(claimSeq);
        //    }
        //    if (!CurrentPage.IsCurrentClientAsExpected(EnvironmentManager.TestClient))
        //    {
        //        CheckTestClientAndSwitch();
        //    }
        //    else if (_claimAction.IsWorkingAjaxMessagePresent())
        //    {
        //        _claimAction.Refresh();
        //        _claimAction.WaitForWorkingAjaxMessage();
        //        _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();

        //    }

        //    if (CurrentPage.GetPageHeader().Equals(PageHeaderEnum.ClaimAction.GetStringValue()))
        //    {

        //    }
        //    else if(CurrentPage.GetPageHeader().Equals(PageHeaderEnum.ClaimSearch.GetStringValue()))
        //    {
        //        _claimSearch.SearchByClaimSequenceToNavigateToClaimActionPage(claimSeq,true);
        //    }
        //    else {
        //        CurrentPage = _claimAction = CurrentPage.NavigateToClaimSearch()
        //            .SearchByClaimSequenceToNavigateToClaimActionPage(claimSeq,true);
        //    }



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

        [Test, Category("SmokeTestDeployment")] //TANT-82
        public void Verify_lower_left_quadrant_icons_from_worklist()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(3))
            {
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                ClaimActionPage _claimAction;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                string claimSeq = paramLists["ClaimSeq"];
                string flag = paramLists["Flag"];
                _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(claimSeq);
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();

                _claimAction.ClickOnEditAllFlagsIcon();
                _claimAction.IsEditFlagAreaPresent().ShouldBeTrue("Is Edit all flags form expanded?");

                StringFormatter.PrintMessage(
                    "Verifying saving with reason code empty in edit all flags should result in error");
                _claimAction.ClickOnSaveButton();
                _claimAction.ClosePageError();
                _claimAction.IsInvalidInputPresentByLabel("Reason Code").ShouldBeTrue(
                    "Reason Code Field should be surrounded with red highlight" +
                    "to show that invalid input is entered");
                _claimAction.ClickOnCancelLink();

                StringFormatter.PrintMessage(
                    "Verifying saving with reason code empty in edit all flags on a line should result in error");
                _claimAction.ClickEditAllFlagOnLineByLineNo();
                _claimAction.ClickOnSaveButton();
                _claimAction.ClosePageError();
                _claimAction.IsInvalidInputPresentByLabel("Reason Code").ShouldBeTrue(
                    "Reason Code Field should be surrounded with red highlight" +
                    "to show that invalid input is entered");
                _claimAction.ClickOnCancelLink();

                _claimAction.ClickAddIconButton();
                _claimAction.IsAddFlagPanelPresent().ShouldBeTrue("Is Add Flag form present?");
                _claimAction.ClickOnFlagLevelCancelLink();

                _claimAction.ClickOnTransferButton();
                _claimAction.IsTransferClaimsWidgetDisplayed().ShouldBeTrue("Is Transfer Claim Form displayed?");
                _claimAction.ClickOnSaveButton();
                _claimAction.ClosePageError();
                _claimAction.IsInvalidInputPresentByLabel("Status").ShouldBeTrue(
                    "Status Field should be surrounded with red highlight" +
                    "to show that invalid input is entered");
                _claimAction.ClickOnCancelLink();

                _claimAction.ClickOnTransferApproveButton();
                _claimAction.IsTransferClaimsWidgetDisplayed()
                    .ShouldBeTrue("Is Transfer/Approve Claim form displayed?");
                _claimAction.ClickOnSaveButton();
                _claimAction.ClosePageError();
                _claimAction.IsInvalidInputPresentByLabel("Status").ShouldBeTrue(
                    "Status Field should be surrounded with red highlight" +
                    "to show that invalid input is entered");
                _claimAction.ClickOnCancelLink();

                StringFormatter.PrintMessage("Verify flag level icons");
                _claimAction.ClickOnEditForGivenFlag(flag);
                _claimAction.IsFlagLevelEditFlagAreaPresent().ShouldBeTrue("Is Edit all flags form expanded?");
                _claimAction.ClickOnSaveButton();
                _claimAction.ClosePageError();
                _claimAction.IsInvalidInputPresentByLabel("Reason Code").ShouldBeTrue(
                    "Reason Code Field should be surrounded with red highlight" +
                    "to show that invalid input is entered");
                _claimAction.ClickOnCancelLink();

                for (var i = 0; i < 2; i++)
                {
                    if (_claimAction.IsClaimLocked())
                        _claimAction.RemoveLock();
                    if (_claimAction.IsDeleteIconEnabledByLinenoAndRow(1, 1))
                    {
                        _claimAction.ClickOnDeleteIconOnFlagByLineNoAndRow(claimSeq);
                        _claimAction.WaitForWorking();
                        _claimAction.IsFlaggedLineDeletedByLine(1, 1)
                            .ShouldBeTrue("Flagged Line Should Be Deleted", true);
                    }
                    else
                    {
                        _claimAction.ClickOnRestoreIconOnFlaggedLinesRowsByRow(claimSeq);
                        _claimAction.IsFlaggedLineNotDeletedByLine(1, 1)
                            .ShouldBeTrue("Flagged Line Should Be Restored", true);
                    }
                }

                _claimAction.ClickOnLogicIconByFlag(flag);
                _claimAction.IsLogicWindowDisplay().ShouldBeTrue("Create Logic form should be displayed");
                _claimAction.GetSideWindow.Cancel();
            }

        }


        [Test, Category("SmokeTestDeployment")] //TANT-82
        public void Verify_that_delete_all_flag_and_restore_all_flag_clicked_work_properly()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(3))
            {
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                ClaimActionPage _claimAction;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                string claimSeq = paramLists["ClaimSequence"];
                string claimSeq1 = paramLists["ClaimSequence1"];
                string reasonCode = paramLists["ReasonCode"];
                _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(claimSeq1);
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                bool errorPagePresent = false;
                try
                {
                    if (_claimAction.IsDeleteAllFlagsPresent())
                    {
                        int flagCount = _claimAction.GetCountOfAllFlags();
                        int deletedFlagCount = _claimAction.GetCountOfDeletedFlags();
                        Console.WriteLine("Before flags are deleted.");
                        Console.WriteLine(string.Format("All flags count: '{0}'\nDeleted flags count '{1}'", flagCount,
                            deletedFlagCount));
                        flagCount += deletedFlagCount;
                        _claimAction.ClickOnEditAllFlagsIcon();
                        _claimAction.ClickDeleteButtonOfEditAllFlagsSection();
                        _claimAction.SelectReasonCodeInEditAllFlag(reasonCode);
                        _claimAction.ClickOnSaveButton();
                        errorPagePresent = _claimAction.IsPageErrorPopupModalPresent();
                        if (errorPagePresent)
                        {
                            _claimAction.ClosePageError();
                            SearchByClaimSeqFromWorkList(claimSeq,_claimAction);

                        }

                        Console.WriteLine("After flags are deleted.");
                        deletedFlagCount = _claimAction.GetCountOfDeletedFlags();
                        Console.WriteLine(string.Format("All flags count: '{0}'\nDeleted flags count '{1}'",
                            _claimAction.GetCountOfAllFlags(), deletedFlagCount));
                        (deletedFlagCount == flagCount).ShouldBeTrue(
                            "Deleted flags count are equal to previous all flags count and deleted flags count");
                        _claimAction.ClickOnEditAllFlagsIcon();
                        _claimAction.ClickRestoreButtonOfEditAllFlagsSection();
                        _claimAction.SelectReasonCodeInEditAllFlag(reasonCode);
                        _claimAction.ClickOnSaveButton();

                        if (_claimAction.IsPageErrorPopupModalPresent())
                        {
                            _claimAction.ClosePageError();
                            _claimAction.RemoveLock();
                            _claimAction.ClickOnSaveButton();
                        }

                        Console.WriteLine("After flags are restored.");
                        int restoreFlagCount = _claimAction.GetCountOfAllFlags();
                        Console.WriteLine("Restored flags count: '{0}'\nDeleted flags count '{1}'", restoreFlagCount,
                            _claimAction.GetCountOfDeletedFlags());
                        (deletedFlagCount == restoreFlagCount).ShouldBeTrue(
                            "Restored flags count are equal to previous deleted flags count");
                    }
                    else
                    {
                        int flagCount = _claimAction.GetCountOfAllFlags();
                        int deletedFlagCount = _claimAction.GetCountOfDeletedFlags();
                        Console.WriteLine("Before flags are restored.");
                        Console.WriteLine("All flags count: '{0}'\nDeleted flags count '{1}'", flagCount,
                            deletedFlagCount);
                        _claimAction.ClickOnEditAllFlagsIcon();
                        _claimAction.ClickRestoreButtonOfEditAllFlagsSection();
                        _claimAction.SelectReasonCodeInEditAllFlag(reasonCode);
                        _claimAction.ClickOnSaveButton();
                        errorPagePresent = _claimAction.IsPageErrorPopupModalPresent();
                        if (_claimAction.IsPageErrorPopupModalPresent())
                        {
                            _claimAction.ClosePageError();
                            SearchByClaimSeqFromWorkList(claimSeq,_claimAction);
                        }

                        Console.WriteLine("After flags are restored.");
                        int restoreFlagCount = _claimAction.GetCountOfAllFlags();
                        Console.WriteLine("Restored flags count: '{0}'\nDeleted flags count '{1}'", restoreFlagCount,
                            _claimAction.GetCountOfDeletedFlags());
                        (deletedFlagCount == restoreFlagCount).ShouldBeTrue(
                            "Restored flags count are equal to previous deleted flags count");
                        _claimAction.ClickOnEditAllFlagsIcon();
                        _claimAction.ClickDeleteButtonOfEditAllFlagsSection();
                        _claimAction.SelectReasonCodeInEditAllFlag(reasonCode);
                        _claimAction.ClickOnSaveButton();

                        if (_claimAction.IsPageErrorPopupModalPresent())
                        {
                            _claimAction.ClosePageError();
                            _claimAction.RemoveLock();
                            _claimAction.ClickOnSaveButton();
                        }

                        Console.WriteLine("After flags are deleted.");
                        deletedFlagCount = _claimAction.GetCountOfDeletedFlags();
                        Console.WriteLine("All flags count: '{0}'\nDeleted flags count '{1}'",
                            _claimAction.GetCountOfAllFlags(), deletedFlagCount);
                        (deletedFlagCount == restoreFlagCount).ShouldBeTrue(
                            "Deleted flags count are equal to previous all flags count");
                    }
                }
                finally
                {
                    if (_claimAction.IsPageErrorPopupModalPresent())
                    {
                        _claimAction.ClosePageError();
                        if (_claimAction.IsDisabled(ClaimActionPage.ActionItems.DeleteOrRestore))
                            _claimAction.RemoveLock();
                        if (_claimAction.IsRestoreButtonPresent())
                            _claimAction.ClickOnRestoreAllFlagsIcon();
                        this.AssertFail("Error page popup is present");
                    }

                    //If Restore All Flag icon is present instead, click on it to change to get Delete All.
                    if (_claimAction.IsDisabled(ClaimActionPage.ActionItems.DeleteOrRestore))
                        _claimAction.RemoveLock();
                    if (_claimAction.IsRestoreButtonPresent())
                        _claimAction.ClickOnRestoreAllFlagsIcon();
                }
            }

        }


        [Test, Category("SmokeTestDeployment")]//US31605 + TANT-82
        public void Verify_that_viewing_flag_detail_for_system_deleted_flags_edit_appears_with_strikethrough_and_customization_is_populated()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(3))
            {
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                ClaimActionPage _claimAction;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                string claimSeq = paramLists["ClaimSequence"];
                _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(claimSeq);
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                StringFormatter.PrintLineBreak();
                _claimAction.ClickSystemDeletedFlagIcon();
                _claimAction.ClickOnSystemDeletedFlagLine();
                _claimAction.IsSystemDeletedFlagPresentInFlagDetailSection()
                    .ShouldBeTrue("System Deleted Flag Is Present");
                _claimAction
                    .GetCustomizationPopulatedLabel()
                    .ShouldBeEqual("Cust:", "Customization Label");
                (_claimAction
                        .GetCustomizationPopulatedField().Length > 0)
                    .ShouldBeTrue("Customization Field is not Empty.");
            }
        }


        [Test, Category("SchemaDependent")]//CAR-1410(CAR-1858)
        public void Verify_CVP_defense_information_in_flag_details_for_non_batch_client()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(3))
            {
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                ClaimActionPage _claimAction;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var claimSequence = paramLists["ClaimSequence"];
                var claimSequenceHavingDefenseCodeOutOfRange = paramLists["ClaimSequenceHavingDefenseCodeOutOfRange"];
                var flagSeq = paramLists["FlagSeq"];

                List<string> flagList = paramLists["Flag"].Split(';').ToList();

                StringFormatter.PrintMessageTitle("Verify edit text for a claim with a different Trigger claseq");
                automatedBase.CurrentPage.ClickOnSwitchClient().SwitchClientTo(ClientEnum.RPE, true);
                _claimAction = automatedBase.CurrentPage.NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(claimSequence);
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                if (_claimAction.IsPageErrorPopupModalPresent())
                {
                    _claimAction.ClosePageError();
                }

                Verify_labels_and_values_in_the_flag_details_for_particular_flag(claimSequence, flagList[0],_claimAction);


                StringFormatter.PrintMessage(
                    "Verifying Flagged line with defense code but DOS doesn't lie between effective and expire date of defense code");
                SearchByClaimSeqFromWorkList(claimSequenceHavingDefenseCodeOutOfRange,_claimAction);
                _claimAction.ClickOnFlagLineByFlag(flagList[2]);
                _claimAction.IsDefenseCodePresent(flagSeq).ShouldBeTrue("Is Defense Code Present");
                _claimAction.IsCVPFlagContainerPresent().ShouldBeTrue(
                    "Selecting flag line of CVP flag must  display a new container above existing flag audit information");
                _claimAction
                    .GetDefenseRationaleOfFlagseqBasedOnEffectiveDateAndDOS(claimSequenceHavingDefenseCodeOutOfRange)
                    .ShouldBeNullorEmpty("DOS of flagged line doesn't lie effective and expire date of defense code ");
            }
        }

        [Test] //CAR-2432(CAR-2569)
        [NonParallelizable]
        public void Verify_logic_pend_functionality()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(3))
            {
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                ClaimActionPage _claimAction;
                ClaimSearchPage _claimSearch;
                var claimToBeApproved = paramLists["ClaimToBeApproved"];
                var claimNo = paramLists["ClaimNo"];
                _claimAction = automatedBase.CurrentPage.NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage("1458018-0");
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                var claseq = _claimAction.GetClaimsByClaimNoFromDatabase(claimNo);
                _claimAction.DeleteLogicNoteFromDatabase(claseq[0]);
                _claimAction.DeleteLogicNoteFromDatabase(claseq[1]);
                _claimAction.UpdateClaimStatus(claseq[0], ClientEnum.SMTST.ToString(),
                    StatusEnum.CotivitiUnreviewed.GetStringValue());
                _claimAction.UpdateClaimStatus(claseq[1], ClientEnum.SMTST.ToString(),
                    StatusEnum.CotivitiUnreviewed.GetStringValue());

                try
                {
                    StringFormatter.PrintMessage("Creation of new logic");
                    _claimSearch = _claimAction.ClickClaimSearchIcon();
                    _claimSearch.GetSideBarPanelSearch.ClickOnClearLink();
                    _claimSearch.SearchByClaimNmber(claimNo);
                    _claimSearch.GetGridViewSection.ClickOnGridByRowCol(1, 2);
                    _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                    _claimAction.GetClaimStatus().ShouldBeEqual(StatusEnum.CotivitiUnreviewed.GetStringDisplayValue(),
                        "Initial status should be Cotiviti Unreviewed");
                    _claimAction.GetClaimSequence().ShouldBeEqual(claseq[0], "Claim Sequence should match");
                    _claimAction.ClickOnAddLogicIconByRow(1);
                    _claimAction.AddLogicMessageTextarea("Test Logic From Internal User");
                    _claimAction.GetSideWindow.Save();
                    _claimAction.WaitForWorking();
                    _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();

                    StringFormatter.PrintMessage(
                        "Verification that page navigates to next ClaimSeq in the search result list after logic creation");
                    _claimAction.GetClaimSequence().ShouldBeEqual(claseq[1],
                        "On creating the logic, it should transfer to another claim sequence in the list");

                    StringFormatter.PrintMessage(
                        "Verification that page navigates to Claim Search page when next ClaimSeq is not available after logic creation");
                    _claimAction.ClickOnAddLogicIconByRow(1);
                    _claimAction.AddLogicMessageTextarea("Test Logic From Internal User");
                    _claimAction.GetSideWindow.Save();
                    _claimAction.WaitForWorking();
                    _claimAction.WaitForStaticTime(200);
                    _claimSearch.GetPageHeader().ShouldBeEqual(PageHeaderEnum.ClaimSearch.GetStringValue(),
                        "User should be navigated to Claim Search page after creating the logic when there is no other claim sequence in the list");

                    StringFormatter.PrintMessage(
                        "Verification that status changes to Logic Pend after creation of Logic");
                    _claimSearch.GetGridViewSection.GetValueInGridByColRow(8).ShouldBeEqual(
                        ClaimSubStatusTypeEnum.LogicPend.GetStringValue(),
                        "Status should change to logic pend after logic creation");
                    _claimSearch.GetGridViewSection.GetValueInGridByColRow(8, 2).ShouldBeEqual(
                        ClaimSubStatusTypeEnum.LogicPend.GetStringValue(),
                        "Status should change to logic pend after logic creation");

                    StringFormatter.PrintMessage("Verification of assigned to value");
                    _claimSearch.GetGridViewSection.ClickOnGridRowByValue(claseq[0]);
                    _claimSearch.GetAssignedTo().ShouldBeEqual(_claimSearch.GetLoggedInUserFullName(),
                        "Claim should be Assigned to the user who created the logic");

                    StringFormatter.PrintMessage("Verification Of Claim Audit History");
                    var rowToBeClicked =
                        _claimSearch.GetGridViewSection.GetGridRowNumberByColAndLabel(claimToBeApproved);
                    _claimSearch.GetGridViewSection.ClickOnGridByRowCol(rowToBeClicked, 2);
                    _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();

                    var t = DateTime.ParseExact(_claimAction.GetSystemDateFromDatabase(), "MM/dd/yyyy hh:mm:ss tt",
                        CultureInfo.InvariantCulture);

                    ClaimProcessingHistoryPage claimProcessingHistory =
                        _claimAction.ClickOnClaimProcessingHistoryAndSwitch();
                    DateTime.ParseExact(claimProcessingHistory.GetClaimHistoryTableDataValueByRowCol(1, 1),
                        "M/d/yyyy h:m:s tt", CultureInfo.InvariantCulture).AssertDateRange(t.AddMinutes(-1),
                        t.AddMinutes(1),
                        "Modified Date And Time Stamp should match");
                    claimProcessingHistory.GetClaimHistoryTableDataValueByRowCol(1, 6).ShouldBeEqual(
                        ClaimSubStatusTypeEnum.LogicPend.GetStringValue(), "Claim Status should be Logic Pend");
                    _claimAction = claimProcessingHistory.CloseClaimProcessingHistoryPageAndSwitchToClaimActionPage();

                    StringFormatter.PrintMessage(
                        "Verification that status can be changed from Logic Pend to other status using Transfer button");
                    _claimAction.ClickOnTransferButton();
                    _claimAction.SelectStatusCode(ClaimSubStatusTypeEnum.DocumentationRequired.GetStringValue());
                    _claimAction.ClickOnSaveButton();
                    _claimAction.WaitForWorking();

                    StringFormatter.PrintMessage(
                        $"Verification that status from Logic Pend to '{ClaimSubStatusTypeEnum.DocumentationRequired.GetStringValue()}' " +
                        "status after approving the claim sequence");
                    _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                    _claimAction.ClickOnApproveButton();
                    var rowOfApprovedClaim =
                        _claimSearch.GetGridViewSection.GetGridRowNumberByColAndLabel(claimToBeApproved);
                    _claimSearch.GetGridViewSection.GetValueInGridByColRow(8, rowOfApprovedClaim).ShouldBeEqual(
                        ClaimSubStatusTypeEnum.DocumentationRequired.GetStringValue(),
                        $"Status should change to '{ClaimSubStatusTypeEnum.DocumentationRequired.GetStringValue()}' after the claim is transferred");
                    _claimSearch.GetGridViewSection.GetValueInGridByColRow(8, rowOfApprovedClaim == 1 ? 1 : 2)
                        .ShouldNotBeEqual(ClaimSubStatusTypeEnum.LogicPend.GetStringValue(),
                            "Status should change after approving claim sequence of Logic pend status");

                }
                finally
                {
                    _claimAction.DeleteLogicNoteFromDatabase(claseq[0]);
                    _claimAction.DeleteLogicNoteFromDatabase(claseq[1]);
                    _claimAction.UpdateClaimStatus(claseq[0], ClientEnum.SMTST.ToString(),
                        StatusEnum.CotivitiUnreviewed.GetStringValue());
                    _claimAction.UpdateClaimStatus(claseq[1], ClientEnum.SMTST.ToString(),
                        StatusEnum.CotivitiUnreviewed.GetStringValue());
                }
            }
        }

        [Test]//CAR-1796(CAR-1407)
        [NonParallelizable]
        public void Verify_reply_close_logic_functionality()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(3))
            {
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var claSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "ClaimSequence", "Value");
                ClaimActionPage _claimAction;
                _claimAction = automatedBase.CurrentPage.NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage("1458018-0");
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();

                try
                {
                    #region Delete Logic From Database and Create New Logic
                    StringFormatter.PrintMessageTitle("Delete Logic From Database and Create New Logic");
                    _claimAction.DeleteLogicNoteFromDatabase(claSeq);
                    SearchByClaimSeqFromWorkList(claSeq,_claimAction);
                    _claimAction.IsLogicPlusIconDisplayed(1)
                        .ShouldBeTrue("Lplus Icon should be displayed for No Logic");
                    _claimAction.ClickOnAddLogicIconByRow(1);
                    _claimAction.IsLogicFormText("Create Logic").ShouldBeTrue("Create Logic form should be displayed");
                    _claimAction.AddLogicMessageTextarea("Test Logic From Internal User");
                    _claimAction.GetSideWindow.Save();
                    _claimAction.WaitForWorking();

                    _claimAction.WaitForStaticTime(500);
                    _claimAction.SearchByClaimSequence(claSeq);
                    _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();

                    _claimAction.ClickOnLogicIconWithLogicByRow(1);
                    var actualLogicMessage = Regex.Split(_claimAction.GetRecentRightLogicMessage(), "\r\n").ToList();
                    _claimAction.GetSideWindow.Cancel();

                    _claimAction.ClickOnAddLogicIconByRow(2);
                    _claimAction.WaitForStaticTime(500);
                    _claimAction.AddLogicMessageTextarea("Test Logic From Internal User For Closed");
                    _claimAction.WaitForStaticTime(200);
                    _claimAction.GetSideWindow.Save();
                    _claimAction.WaitForWorking();
                    _claimAction.WaitForStaticTime(500);

                    _claimAction.SearchByClaimSequence(claSeq);
                    _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();

                    _claimAction.ClickOnLogicIconWithLogicByRow(2);

                    _claimAction.GetSideWindow.ClickOnSecondaryButton();
                    _claimAction.WaitForWorking();
                    _claimAction.IsLogicWindowDisplayByRowPresent(2)
                        .ShouldBeTrue("Logic Form Should display when logic is closed");

                    #endregion

                    _claimAction.Logout().LoginAsClientUser().NavigateToClaimSearch()
                        .SearchByClaimSequenceToNavigateToClaimActionPage(claSeq);
                    _claimAction.IsLogicWindowDisplay()
                        .ShouldBeTrue("Logic Window display by default when assigned to client user");

                    var assignedTo = "Client";

                    for (var i = 0; i < 2; i++)
                    {
                        #region Logic Form Functionality for Closed Status

                        StringFormatter.PrintMessageTitle("Verification of Logic Form Functionality for Closed Status");

                        _claimAction.IsLogicWindowDisplayByRowPresent(2)
                            .ShouldBeFalse("Reply Logic Form Should not display for Closed Status");
                        if (i == 0)
                        {
                            _claimAction.IsClientLogicIconByRowPresent(2)
                                .ShouldBeTrue("Logic Icon should be Assigned to Client - unfilled state");
                            _claimAction.ClickOnLogicIconWithLogicByRow(2);
                            _claimAction.IsLogicWindowDisplayByRowPresent(2)
                                .ShouldBeTrue(
                                    "Reply Logic Form Should display for Closed Status when click on Logic Icon");
                            _claimAction.ClickOnLogicIconWithLogicByRow(2, true);

                        }
                        else
                        {
                            _claimAction.IsInternalLogicIconByRowPresent(2)
                                .ShouldBeTrue(
                                    $"Logic Icon should be Assigned to {CompanyEnum.companyName.GetStringValue()} - filled state");
                            _claimAction.ClickOnLogicIconWithLogicByRow(2);
                            _claimAction.IsLogicWindowDisplayByRowPresent(2)
                                .ShouldBeTrue(
                                    "Reply Logic Form Should display for Closed Status when click on Logic Icon");
                            _claimAction.ClickOnLogicIconWithLogicByRow(2, true);
                        }

                        #endregion

                        #region Validation of Logic Form

                        StringFormatter.PrintMessageTitle("Validation of Logic Form");

                        _claimAction.IsLogicFormText("Reply To Logic")
                            .ShouldBeTrue("Create Logic form should be displayed");
                        _claimAction.GetSideWindow.GetValueByLabel("Assigned to").ShouldBeEqual(
                            assignedTo,
                            $"Assigned to should be {assignedTo}");
                        _claimAction.GetSideWindow.GetValueByLabel("Status").ShouldBeEqual(
                            LogicStatusEnum.Open.GetStringValue(),
                            $"Status should be {LogicStatusEnum.Open.GetStringValue()} when logic is created");
                        _claimAction.GetSideWindow.Save();
                        _claimAction.GetPageErrorMessage().ShouldBeEqual("Enter a message to create Logic request.",
                            "Popup Error for Empty Message Reply");
                        _claimAction.ClosePageError();
                        _claimAction.AddLogicMessageTextarea(Concat(Enumerable.Repeat("TEST ", 101)));
                        _claimAction.GetLogicMessageTextarea().Length
                            .ShouldBeEqual(500, "User can free text up to 500 characters");
                        _claimAction.GetSideWindow.GetPrimaryButtonName()
                            .ShouldBeEqual("Reply", "Is 'Reply' Button Present?");
                        _claimAction.GetSideWindow.GetSecondaryButtonName()
                            .ShouldBeEqual("Close Logic", "Is 'Close Logic' Button Present?");

                        StringFormatter.PrintMessageTitle("Verification of Cancel Button");
                        _claimAction.AddLogicMessageTextarea($"Cancel Test Logic Reply From {assignedTo} User");
                        _claimAction.GetSideWindow.Cancel();
                        _claimAction.ClickOnLogicIconWithLogicByRow(1);
                        _claimAction.GetLogicMessageTextarea()
                            .ShouldBeNullorEmpty("Message Box Should be Null or Empty");
                        _claimAction.GetSideWindow.GetValueByLabel("Assigned to").ShouldBeEqual(
                            assignedTo,
                            $"Assigned to should be {assignedTo}");
                        _claimAction.GetSideWindow.GetValueByLabel("Status").ShouldBeEqual(
                            LogicStatusEnum.Open.GetStringValue(),
                            $"Status should be {LogicStatusEnum.Open.GetStringValue()} ");

                        #endregion

                        #region Verification of Reply and CLosed Button

                        StringFormatter.PrintMessageTitle("Verification of Reply and Closed Button");
                        _claimAction.AddLogicMessageTextarea($"Logic Reply From {assignedTo} User");
                        _claimAction.GetSideWindow.Save();
                        _claimAction.WaitForWorking();
                        var actualLeftLogicMessage =
                            Regex.Split(_claimAction.GetRecentLeftLogicMessage(), "\r\n").ToList();
                        actualLeftLogicMessage.ShouldBeEqual(actualLogicMessage,
                            "Other User Type Logic Should be on left side");
                        actualLogicMessage = Regex.Split(_claimAction.GetRecentRightLogicMessage(), "\r\n").ToList();

                        actualLogicMessage[0].ShouldBeEqual(_claimAction.GetLoggedInUserFullName(),
                            "Reply User Full Name should be same");
                        actualLogicMessage[1].IsDateTimeWithoutSecInFormat()
                            .ShouldBeTrue("Is Date Time is in Correct Format?");
                        actualLogicMessage[2]
                            .ShouldBeEqual($"Logic Reply From {assignedTo} User", "Is Logic Message Same?");
                        var newReplyAssignedTo = assignedTo;
                        assignedTo = assignedTo == "Client" ? CompanyEnum.companyName.GetStringValue() : "Client";

                        _claimAction.GetSideWindow.GetValueByLabel("Assigned to").ShouldBeEqual(
                            assignedTo,
                            $"Assigned to should be {assignedTo}");
                        _claimAction.GetSideWindow.GetValueByLabel("Status").ShouldBeEqual(
                            LogicStatusEnum.Open.GetStringValue(),
                            $"Status should be {LogicStatusEnum.Open.GetStringValue()} when logic is created");
                        _claimAction.GetSideWindow.ClickOnSecondaryButton();
                        _claimAction.GetSideWindow.GetValueByLabel("Status").ShouldBeEqual(
                            LogicStatusEnum.Closed.GetStringValue(),
                            $"Status should be {LogicStatusEnum.Closed.GetStringValue()} when logic is closed");
                        _claimAction.IsPageErrorPopupModalPresent()
                            .ShouldBeFalse("User able to Close Logic without message");
                        _claimAction.IsLogicWindowDisplay()
                            .ShouldBeTrue("Logic Window display by default when assigned to client user");
                        _claimAction.AddLogicMessageTextarea($"New Logic Reply From {newReplyAssignedTo} User");
                        _claimAction.GetSideWindow.Save();
                        _claimAction.WaitForWorking();
                        _claimAction.IsLogicWindowDisplayByRowPresent(1)
                            .ShouldBeTrue("Logic Form Should display when logic is replied");
                        actualLogicMessage = Regex.Split(_claimAction.GetRecentRightLogicMessage(), "\r\n").ToList();
                        _claimAction.GetSideWindow.Cancel();

                        #endregion


                        if (i != 0) continue;
                        _claimAction.ClickOnLogicIconWithLogicByRow(2);
                        _claimAction.AddLogicMessageTextarea("Test Logic From Client User For Closed");
                        _claimAction.GetSideWindow.Save();
                        _claimAction.WaitForWorking();
                        _claimAction.GetSideWindow.ClickOnSecondaryButton();
                        _claimAction.IsLogicWindowDisplayByRowPresent(2)
                            .ShouldBeTrue("Logic Form Should display when logic is closed");

                        _claimAction.Logout().LoginAsHciAdminUser3().NavigateToClaimSearch()
                            .SearchByClaimSequenceToNavigateToClaimActionForClientSwitch(claSeq);

                    }
                }

                finally
                {
                    if (_claimAction.IsLogicWindowDisplay())
                        _claimAction.GetSideWindow.Cancel();
                }
            }
        }




        [Test, Category("SmokeTest")]//US37264
        public void Verify_switched_flag_line_from_one_claim_to_another_and_back_to_previous_claim_retaining_units()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(3))
            {
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var claimSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "ClaimSequence", "Value");
                var flag = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                    "Flag", "Value");
                ClaimActionPage _claimAction;
                _claimAction = automatedBase.CurrentPage.NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(claimSeq);
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();

                //Get Trigger Line # from current line in which switch operation perform
                var triggerLine = _claimAction.GetTriggerLineByFlag(flag);
                var unit = _claimAction.GetUnitByFlag(flag);

                //Click switch flag icon having flag=UNB
                _claimAction.ClickSwitchEditIconByFlag(flag);
                _claimAction.GetUnitByLineFlag(triggerLine, flag)
                    .ShouldBeEqual(unit, "Unit is same after switch operation");
                triggerLine = _claimAction.GetTriggerLineByFlag(flag);

                //Click switch flag icon having flag=UNB
                _claimAction.ClickSwitchEditIconByFlag(flag);
                _claimAction.GetUnitByLineFlag(triggerLine, flag)
                    .ShouldBeEqual(unit, "Unit is same after switch back to the previous claim");
            }
        }



        [Test, Category("NewClaimAction3")]//39864
        public void Verify_that_Logic_Manager_default_view_appears_when_user_access_claim_action_from_the_logic_search_page()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(3))
            {
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var claimSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "ClaimSequence", "Value");
                ClaimActionPage _claimAction;
                ClaimSearchPage claimSearch = null;
                _claimAction = automatedBase.CurrentPage.NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(claimSeq);
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                var logicSearch = _claimAction.NavigateToLogicSearch();
                _claimAction = logicSearch.SearchByClaimSequenceToNavigateToNewClaimActionPage(claimSeq);
                _claimAction.IsLogicWindowDisplay()
                    .ShouldBeTrue("Logic window should be present");
            }
        }




        [Test, Category("NewClaimAction3")]//us39873(Ref Story US55877:- sug paid logic updated) + CAR-2650(CAR-2622)
        [Retrying(Times = 3)]
        public void Verify_that_sug_paid_and_savings_updated_when_sug_units_modified()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(3))
            {
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var claimSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "ClaimSequence", "Value");
                var flag1 = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                    "flag1", "Value");
                //var flag2 = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName, "flag2", "Value");
                var reasonCode = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "ReasonCode", "Value");
                ClaimActionPage _claimAction;
                _claimAction = automatedBase.CurrentPage.NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(claimSeq);
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                var rnd = new Random();
                var flagLine = _claimAction.GetFlagLineValue(1);
                try
                {
                    var preSugUnits = _claimAction.GetInternalAndClientSugUnitsByClaseqAndFlag(claimSeq, flag1);
                    var sugUnits1 = rnd.Next(86, 170);
                    while (preSugUnits[0] == sugUnits1 || preSugUnits[1] == sugUnits1)
                    {
                        sugUnits1 = rnd.Next(86, 170);
                    }

                    _claimAction.ClickOnDollarIcon();
                    var allowed = _claimAction.GetAllowedValueByLine(flagLine);
                    var units = _claimAction.GetUnitValueOnFlag(1);
                    var adjPaid = _claimAction.GetAdjPaidValueByLine(flagLine);
                    _claimAction.ClickOnEditIconFlagLevelForLineEdit(flagLine, flag1);
                    _claimAction.EnterSugUnits(sugUnits1.ToString());
                    _claimAction.SelectReasonCode(reasonCode);
                    _claimAction.GetSugPaid()
                        .ShouldBeEqual(Math.Round(allowed / units * sugUnits1, 2, MidpointRounding.AwayFromZero),
                            "Sug Paid calculation is correct after modification of Sug Units instantly in input field");
                    _claimAction.SaveFlag(reasonCode);
                    var newSugUnits = _claimAction.GetInternalAndClientSugUnitsByClaseqAndFlag(claimSeq, flag1);
                    newSugUnits[0].ShouldNotBeEqual(preSugUnits[0], "HCISUGUNITS should be changed");
                    newSugUnits[1].ShouldNotBeEqual(preSugUnits[1], "CLISUGUNITS should be changed");
                    newSugUnits[0].ShouldBeEqual(newSugUnits[1], "HCISUGUNITS and CLISUGUNITS should be same");
                    //var sugUnits2 = rnd.Next(86, 170);
                    //_claimAction.ClickOnEditIconFlagLevelForLineEdit(flagLine, flag2);
                    //_claimAction.EnterSugUnits(sugUnits2.ToString());
                    //_claimAction.SelectReasonCode(reasonCode);
                    //_claimAction.GetSugPaid()
                    //    .ShouldBeEqual(Math.Round(allowed / units * sugUnits2, 2, MidpointRounding.AwayFromZero),
                    //        "Sug Paid calculation is correct after modification of Sug Units instantly in input field");
                    //_claimAction.SaveFlag(reasonCode);
                    var sugPaid = _claimAction.GetLineSugPaidValue(flag1);
                    var savings = _claimAction.GetLineSavingsValue(flag1);
                    var expectedSaving1 =
                        Math.Round(adjPaid - Math.Round(allowed / units * sugUnits1, 2, MidpointRounding.AwayFromZero),
                            2,
                            MidpointRounding.AwayFromZero);
                    //var expectedSaving2 =
                    //    Math.Round(adjPaid - Math.Round(allowed / units * sugUnits2, 2, MidpointRounding.AwayFromZero), 2,
                    //        MidpointRounding.AwayFromZero);
                    //var totaExpectedSavings = Math.Round(expectedSaving1 + expectedSaving2, 2, MidpointRounding.AwayFromZero);
                    var totaExpectedSavings = Math.Round(expectedSaving1, 2, MidpointRounding.AwayFromZero);
                    var expectedSugPaid = Math.Round(allowed / units * sugUnits1, 2, MidpointRounding.AwayFromZero);
                    sugPaid.ShouldBeEqual(expectedSugPaid,
                        "Sug Paid calculation is correct after modification of Sug Units");
                    savings.ShouldBeEqual(totaExpectedSavings,
                        "Savings calculation is correct after modification of Sug Units");
                    sugUnits1 = rnd.Next(10, 85);
                    _claimAction.ClickOnEditIconFlagLevelForLineEdit(flagLine, flag1);
                    _claimAction.EnterSugUnits(sugUnits1.ToString());
                    _claimAction.SelectReasonCode(reasonCode);
                    _claimAction.SaveFlag(reasonCode);
                    //sugUnits2 = rnd.Next(10, 85);
                    //_claimAction.ClickOnEditIconFlagLevelForLineEdit(flagLine, flag2);
                    //_claimAction.EnterSugUnits(sugUnits2.ToString());
                    //_claimAction.SelectReasonCode(reasonCode);
                    //_claimAction.SaveFlag(reasonCode);
                    sugPaid = _claimAction.GetLineSugPaidValue(flag1);
                    savings = _claimAction.GetLineSavingsValue(flag1);
                    expectedSaving1 =
                        Math.Round(adjPaid - Math.Round(allowed / units * sugUnits1, 2, MidpointRounding.AwayFromZero),
                            2,
                            MidpointRounding.AwayFromZero);
                    //expectedSaving2 =
                    //    Math.Round(adjPaid - Math.Round(allowed / units * sugUnits2, 2, MidpointRounding.AwayFromZero), 2,
                    //        MidpointRounding.AwayFromZero);
                    //totaExpectedSavings = Math.Round(expectedSaving1 + expectedSaving2, 2, MidpointRounding.AwayFromZero);
                    totaExpectedSavings = Math.Round(expectedSaving1, 2, MidpointRounding.AwayFromZero);
                    if (totaExpectedSavings >= adjPaid)
                        //    sugPaid.ShouldEqual(0, "Sug Paid should be zero when Saving amount is greater than Adj Paid amount");
                        savings.ShouldBeEqual(adjPaid,
                            "Saving amount should equal to Adj Paid amount when Saving amount is greater than Adj Paid amount");
                }
                finally
                {
                    if (_claimAction.IsPageErrorPopupModalPresent())
                        _claimAction.ClosePageError();
                }
            }
        }


        [Test, Category("NewClaimAction3")]
        [Retrying(Times = 3)]
        public void Verify_switched_edit_is_deleted_from_one_line_and_added_to_another_claim_changes_appear_on_claim_without_having_to_refresh()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(3))
            {
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IList<string> flagLines = new List<string>(2);
                string claimSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "ClaimSequence", "Value");
                ClaimActionPage _claimAction;
                _claimAction = automatedBase.CurrentPage.NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(claimSeq);
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                _claimAction.DeleteAllDeletedFlagsByClaSeq(claimSeq);
                //Get Trigger Line # from current line in which switch operation perform
                string triggerLine = _claimAction.GetTriggerLine();

                //Get current line # in which switch operation perform
                string switchedEdit = _claimAction.GetCurrentEditToSwitch();

                //Get edit in which switch operation perform
                string currentLineNum = _claimAction.GetCurrentLineNumToSwitch();

                Console.Out.WriteLine("Current Line # : {0} \nEdit : {1} \nTL : {2}", currentLineNum, switchedEdit,
                    triggerLine);

                //Count deleted line before switch edit 
                int deletedLineCount =
                    _claimAction.GetDeletedLineCountWithSwitchOperation(currentLineNum, switchedEdit);

                Console.Out.WriteLine("Deleted FlagLineCount before Switch of a current line : {0}", deletedLineCount);

                //Count edit line at line triggered by current line #
                int countEditAtTriggerLine =
                    _claimAction.GetEditedLineCountAtTriggerLineNumber(triggerLine, switchedEdit);

                Console.Out.WriteLine("Edit Line Count : {0} at TL : {1}", countEditAtTriggerLine, triggerLine);

                StringFormatter.PrintMessageTitle(
                    "Switch Flag, Count Deleted Flag Line, Added To Flag Trigger Line And Disabled Switch Icon ");

                //Click switch flag icon and verify deleted edit line with same flag in same line
                _claimAction.ClickSwitchEditIcon().GetDeletedLineCountWithSwitchOperation(currentLineNum, switchedEdit)
                    .ShouldBeEqual(deletedLineCount + 1, "Deleted Flag Line Count after switch operation");

                //Verify edit is added to trigger line
                _claimAction.GetEditedLineCountAtTriggerLineNumber(triggerLine, switchedEdit).ShouldBeEqual(
                    countEditAtTriggerLine + 1,
                    string.Format("Added Flag Line Count at Trigger Line # {0}", triggerLine));

                _claimAction.IsSwitchDeleted(currentLineNum).ShouldBeTrue(
                    string.Format("Disabled switch icon after switch at Current Line # {0}", currentLineNum));
                StringFormatter.PrintLineBreak();
            }
        }


        [Test, Category("NewClaimAction3")]//US9606
        [Retrying(Times = 3)]
        public void Verify_that_when_user_deletes_or_restores_the_MPR_edit_then_the_PS_edit_should_be_deleted_or_restored()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(3))
            {
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                string claimSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "ClaimSequence", "Value");
                ClaimActionPage _claimAction;
                _claimAction = automatedBase.CurrentPage.NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(claimSeq);
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                string trigLineNumForPsEdit = _claimAction.GetTriggerLineForEditMpr();
                DeleteOrRestoreMprEditThenPsEditShouldBeDeletedOrRestored(trigLineNumForPsEdit,_claimAction);
            }
        }


        [Test, Category("NewClaimAction3")]//US31625 + TE-803
        [NonParallelizable]
        public void Verify_when_user_deletes_all_flags_on_a_claim_Next_icon_is_disabled_and_alert_is_shown_trying_to_leave()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(3))
            {
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                string claimSeq = paramLists["ClaimSequence"];
                ClaimActionPage _claimAction;
                ClaimSearchPage _claimSearch;
                _claimAction = automatedBase.CurrentPage.NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(claimSeq);
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();

                try
                {
                    //If Restore All Flag icon is present instead, click on it to change to get Delete All.
                    if (_claimAction.IsRestoreButtonPresent())
                        _claimAction.ClickOnRestoreAllFlagsIcon();

                    //_claimAction.ClickOnDeleteAllFlagsIcon();

                    //Next Button should be disabled
                    _claimAction.ClickOnDeleteAllFlagsIconAndNextIconIsDisabled().ShouldBeTrue("Next button disabled?");

                    //Menu Option (IE Incompatible)
                    if (string.Compare(automatedBase.EnvironmentManager.Browser, "IE", StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        Console.WriteLine("Menu Option skipped for IE");
                    }
                    else
                    {
                        _claimAction.ClickOnMenu(SubMenu.AppealSearch);
                        //CheckApplicationAlertMessageThenDismiss();
                        CheckApplicationAlertMessageThenDismiss(_claimAction);
                    }

                    //Appeal Creator Icon
                    //_claimAction.ClickOnlyCreateAppealIcon();
                    //CheckApplicationAlertMessageThenDismiss();

                    //Dashboard Icon
                    _claimAction.ClickOnlyDashboardIcon();
                    CheckApplicationAlertMessageThenDismiss(_claimAction);
                    _claimAction.ClickOnPageFooter();
                    _claimAction.IsClaimLockedByClaimSequence(ClientEnum.SMTST.ToString(), claimSeq.Split('-')[0],
                        claimSeq.Split('-')[1],
                        automatedBase.EnvironmentManager.HciAdminUsername3).ShouldBeTrue($"Is claim {claimSeq} locked?");

                    //TE-803
                    //Search Icon
                    _claimAction.ClickClaimSearchIcon();
                    CheckApplicationAlertMessageThenDismiss(_claimAction);
                    _claimAction.IsClaimLockedByClaimSequence(ClientEnum.SMTST.ToString(), claimSeq.Split('-')[0],
                        claimSeq.Split('-')[1],
                        automatedBase.EnvironmentManager.HciAdminUsername3).ShouldBeTrue($"Is claim {claimSeq} locked?");

                    //Search From Claim Action
                    if (!_claimAction.IsWorkListControlDisplayed())
                        _claimAction.ClickWorkListIcon();
                    _claimAction.ClickSearchIcon().SearchByClaimSequence("12345");
                    CheckApplicationAlertMessageThenDismiss(_claimAction);
                    _claimAction.IsClaimLockedByClaimSequence(ClientEnum.SMTST.ToString(), claimSeq.Split('-')[0],
                        claimSeq.Split('-')[1],
                        automatedBase.EnvironmentManager.HciAdminUsername3).ShouldBeTrue($"Is claim {claimSeq} locked?");

                    //Create Worklist From Claim Action
                    if (!_claimAction.IsWorkListControlDisplayed())
                        _claimAction.ClickWorkListIcon();
                    _claimAction.ToggleWorkListToPci();
                    _claimAction.ClickOnCreateButton();
                    CheckApplicationAlertMessageThenDismiss(_claimAction);
                    _claimAction.IsClaimLockedByClaimSequence(ClientEnum.SMTST.ToString(), claimSeq.Split('-')[0],
                        claimSeq.Split('-')[1],
                        automatedBase.EnvironmentManager.HciAdminUsername3).ShouldBeTrue($"Is claim {claimSeq} locked?");

                    //Help Icon
                    _claimAction.ClickOnlyHelpIcon();
                    CheckApplicationAlertMessageThenDismiss(_claimAction);
                    _claimAction.IsClaimLockedByClaimSequence(ClientEnum.SMTST.ToString(), claimSeq.Split('-')[0],
                        claimSeq.Split('-')[1],
                        automatedBase.EnvironmentManager.HciAdminUsername3).ShouldBeTrue($"Is claim {claimSeq} locked?");

                    //Browser Back Button
                    _claimAction.WaitForStaticTime(500);
                    _claimAction.ClickOnBrowserBackButtonToClaimSearch();
                    CheckApplicationAlertMessageThenDismiss(_claimAction);
                    _claimAction.IsClaimLockedByClaimSequence(ClientEnum.SMTST.ToString(), claimSeq.Split('-')[0],
                        claimSeq.Split('-')[1],
                        automatedBase.EnvironmentManager.HciAdminUsername3).ShouldBeTrue($"Is claim {claimSeq} locked?");


                    _claimAction.ClickOnRestoreAllFlagsIcon();
                    _claimSearch = _claimAction.ClickClaimSearchIcon();
                    _claimSearch.IsClaimLocked(ClientEnum.SMTST.ToString(), claimSeq.Split('-')[0],
                        claimSeq.Split('-')[1],
                        automatedBase.EnvironmentManager.HciAdminUsername3).ShouldBeFalse($"Is claim {claimSeq} locked?");
                }
                finally
                {
                    //Restore All Flags at last if present
                    if (_claimAction.IsRestoreButtonPresent())
                        _claimAction.ClickOnRestoreAllFlagsIcon();
                }
            }
        }

        [Test] //TE-803
        public void Verify_when_user_deletes_all_flags_on_QAReady_Claims_alert_is_not_shown_and_users_are_allowed_to_leave_claim_action_page()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(3))
            {
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var claimSeqQAReadyState = paramLists["ClaimSequenceQAReadyState"];
                var claimSeqFromQAClaimSearch = paramLists["ClaimSequenceFromQAClaimSearch"];
                ClaimActionPage _claimAction;
                ClaimSearchPage _claimSearch;
                _claimAction = automatedBase.CurrentPage.NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(claimSeqQAReadyState);
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();

                try
                {
                    StringFormatter.PrintMessage("Verification for Claims with QA Ready State");
                    //If Restore All Flag icon is present instead, click on it to change to get Delete All.
                    if (_claimAction.IsRestoreButtonPresent())
                        _claimAction.ClickOnRestoreAllFlagsIcon();
                    _claimAction.ClickOnDeleteAllFlagsIcon();

                    //Navigate to dashboard
                    _claimAction.NavigateToDashboard();
                    VerifyPageHeaderAndErrorMessage(PageHeaderEnum.Dashboard.GetStringValue(),automatedBase.CurrentPage);
                    _claimAction.IsClaimLockedByClaimSequence(ClientEnum.SMTST.ToString(),
                            claimSeqQAReadyState.Split('-')[0], claimSeqQAReadyState.Split('-')[1],
                            automatedBase.EnvironmentManager.HciAdminUsername3)
                        .ShouldBeFalse($"Is claim {claimSeqQAReadyState} locked?");
                    _claimAction = automatedBase.CurrentPage.NavigateToClaimSearch()
                        .SearchByClaimSequenceToNavigateToClaimActionPage(claimSeqQAReadyState, true);

                    //Navigate to Appeal Search
                    _claimAction.NavigateToAppealSearch();
                    VerifyPageHeaderAndErrorMessage(PageHeaderEnum.AppealSearch.GetStringValue(),automatedBase.CurrentPage);
                    _claimAction.IsClaimLockedByClaimSequence(ClientEnum.SMTST.ToString(),
                            claimSeqQAReadyState.Split('-')[0], claimSeqQAReadyState.Split('-')[1],
                            automatedBase.EnvironmentManager.HciAdminUsername3)
                        .ShouldBeFalse($"Is claim {claimSeqQAReadyState} locked?");
                    _claimAction = automatedBase.CurrentPage.NavigateToClaimSearch()
                        .SearchByClaimSequenceToNavigateToClaimActionPage(claimSeqQAReadyState, true);

                    //Navigate to Claim Search clicking on claim search icon
                    _claimSearch = _claimAction.ClickClaimSearchIcon();
                    VerifyPageHeaderAndErrorMessage(PageHeaderEnum.ClaimSearch.GetStringValue(),automatedBase.CurrentPage);
                    _claimAction.IsClaimLockedByClaimSequence(ClientEnum.SMTST.ToString(),
                            claimSeqQAReadyState.Split('-')[0], claimSeqQAReadyState.Split('-')[1],
                            automatedBase.EnvironmentManager.HciAdminUsername3)
                        .ShouldBeFalse($"Is claim {claimSeqQAReadyState} locked?");

                    _claimAction = automatedBase.CurrentPage.NavigateToClaimSearch()
                        .SearchByClaimSequenceToNavigateToClaimActionPage(claimSeqQAReadyState);

                    // //Search From Claim Action
                    // if (!_claimAction.IsWorkListControlDisplayed())
                    //     _claimAction.ClickWorkListIcon();
                    // _claimAction.ClickSearchIcon().SearchByClaimSequence(claimSeqFromQAClaimSearch);
                    // // VerifyPageHeaderAndErrorMessage(PageHeaderEnum.ClaimAction.GetStringValue());
                    // _claimAction.IsClaimLockedByClaimSequence(ClientEnum.SMTST.ToString(), claimSeqQAReadyState.Split('-')[0], claimSeqQAReadyState.Split('-')[1],
                    //EnvironmentManager.HciAdminUsername3).ShouldBeFalse($"Is claim {claimSeqQAReadyState} locked?");
                    // _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                    // _claimAction = CurrentPage.NavigateToClaimSearch()
                    //     .SearchByClaimSequenceToNavigateToClaimActionPage(claimSeqFromQAClaimSearch);

                    //Create Worklist From Claim Action
                    if (!_claimAction.IsWorkListControlDisplayed())
                        _claimAction.ClickWorkListIcon();
                    _claimAction.ToggleWorkListToPci();
                    _claimAction.ClickOnCreateButton();
                    _claimAction.IsClaimLockedByClaimSequence(ClientEnum.SMTST.ToString(),
                        claimSeqQAReadyState.Split('-')[0], claimSeqQAReadyState.Split('-')[1],
                        automatedBase.EnvironmentManager.HciAdminUsername3).ShouldBeFalse($"Is claim {claimSeqQAReadyState} locked?");
                    _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                    _claimAction = automatedBase.CurrentPage.NavigateToClaimSearch()
                        .SearchByClaimSequenceToNavigateToClaimActionPage(claimSeqQAReadyState);

                    //Browser Back Button
                    _claimAction.WaitForStaticTime(500);
                    _claimSearch = _claimAction.ClickOnBrowserBackButtonToClaimSearch();
                    VerifyPageHeaderAndErrorMessage(PageHeaderEnum.ClaimSearch.GetStringValue(),automatedBase.CurrentPage);
                    _claimAction.IsClaimLockedByClaimSequence(ClientEnum.SMTST.ToString(),
                        claimSeqQAReadyState.Split('-')[0], claimSeqQAReadyState.Split('-')[1],
                        automatedBase.EnvironmentManager.HciAdminUsername3).ShouldBeFalse($"Is claim {claimSeqQAReadyState} locked?");
                    _claimAction = _claimSearch.ClickOnPreviouslyViewedClaimSequenceLinkByRow(1);

                    // StringFormatter.PrintMessage("Verification for claims in QA Claim Search Page");
                    // SearchByClaimSeqFromWorkList(claimSeqFromQAClaimSearch);
                    // if (_claimAction.IsRestoreButtonPresent())
                    //     _claimAction.ClickOnRestoreAllFlagsIcon();
                    // _claimAction.ClickOnDeleteAllFlagsIcon();

                    // //Navigate to Appeal Search
                    // _claimAction.NavigateToAppealSearch();
                    // VerifyPageHeaderAndErrorMessage(PageHeaderEnum.AppealSearch.GetStringValue());
                    // _claimAction.IsClaimLockedByClaimSequence(ClientEnum.SMTST.ToString(),
                    //         claimSeqFromQAClaimSearch.Split('-')[0], claimSeqFromQAClaimSearch.Split('-')[1],
                    //         EnvironmentManager.HciAdminUsername3)
                    //     .ShouldBeFalse($"Is claim {claimSeqFromQAClaimSearch} locked?");
                    // _claimAction = CurrentPage.NavigateToClaimSearch()
                    //     .SearchByClaimSequenceToNavigateToClaimActionPage(claimSeqFromQAClaimSearch);

                    // //Navigate to dashboard
                    // _claimAction.ClickOnlyDashboardIcon();
                    // VerifyPageHeaderAndErrorMessage(PageHeaderEnum.Dashboard.GetStringValue());
                    // _claimAction.IsClaimLockedByClaimSequence(ClientEnum.SMTST.ToString(),
                    //         claimSeqFromQAClaimSearch.Split('-')[0], claimSeqFromQAClaimSearch.Split('-')[1],
                    //         EnvironmentManager.HciAdminUsername3)
                    //     .ShouldBeFalse($"Is claim {claimSeqFromQAClaimSearch} locked?");
                    // _claimAction = CurrentPage.NavigateToClaimSearch()
                    //     .SearchByClaimSequenceToNavigateToClaimActionPage(claimSeqFromQAClaimSearch);

                    // //Navigate to Claim Search clicking on claim search icon
                    // _claimSearch = _claimAction.ClickClaimSearchIcon();
                    // VerifyPageHeaderAndErrorMessage(PageHeaderEnum.ClaimSearch.GetStringValue());
                    // _claimAction.IsClaimLockedByClaimSequence(ClientEnum.SMTST.ToString(),
                    //         claimSeqFromQAClaimSearch.Split('-')[0], claimSeqFromQAClaimSearch.Split('-')[1],
                    //         EnvironmentManager.HciAdminUsername3)
                    //     .ShouldBeFalse($"Is claim {claimSeqFromQAClaimSearch} locked?");


                    // //Search From Claim Action
                    // if (!_claimAction.IsWorkListControlDisplayed())
                    //     _claimAction.ClickWorkListIcon();
                    // _claimAction.ClickSearchIcon().SearchByClaimSequence(claimSeqQAReadyState);
                    // // VerifyPageHeaderAndErrorMessage(PageHeaderEnum.ClaimAction.GetStringValue());
                    // _claimAction.IsClaimLockedByClaimSequence(ClientEnum.SMTST.ToString(), claimSeqFromQAClaimSearch.Split('-')[0], claimSeqFromQAClaimSearch.Split('-')[1],
                    //EnvironmentManager.HciAdminUsername3).ShouldBeFalse($"Is claim {claimSeqFromQAClaimSearch} locked?");
                    // _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                    // _claimAction = CurrentPage.NavigateToClaimSearch()
                    //     .SearchByClaimSequenceToNavigateToClaimActionPage(claimSeqFromQAClaimSearch);

                    // //Create Worklist From Claim Action
                    // if (!_claimAction.IsWorkListControlDisplayed())
                    //     _claimAction.ClickWorkListIcon();
                    // _claimAction.ToggleWorkListToPci();
                    // _claimAction.ClickOnCreateButton();
                    // _claimAction.IsClaimLockedByClaimSequence(ClientEnum.SMTST.ToString(), claimSeqFromQAClaimSearch.Split('-')[0], claimSeqFromQAClaimSearch.Split('-')[1],
                    //EnvironmentManager.HciAdminUsername3).ShouldBeFalse($"Is claim {claimSeqFromQAClaimSearch} locked?");
                    // _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                    // _claimAction = CurrentPage.NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(claimSeqFromQAClaimSearch);


                    // //Browser Back Button
                    // _claimAction.WaitForStaticTime(500);
                    // _claimSearch = _claimAction.ClickOnBrowserBackButtonToClaimSearch();
                    // VerifyPageHeaderAndErrorMessage(PageHeaderEnum.ClaimSearch.GetStringValue());
                    // _claimAction.IsClaimLockedByClaimSequence(ClientEnum.SMTST.ToString(), claimSeqFromQAClaimSearch.Split('-')[0], claimSeqFromQAClaimSearch.Split('-')[1],
                    //EnvironmentManager.HciAdminUsername3).ShouldBeFalse($"Is claim {claimSeqFromQAClaimSearch} locked?");
                    // _claimAction = _claimSearch.ClickOnPreviouslyViewedClaimSequenceLinkByRow(1);
                    // _claimAction.ClickOnRestoreAllFlagsIcon();

                }
                finally
                {
                    _claimAction.RestoreAllFlagByClaimSequence(claimSeqFromQAClaimSearch);
                    _claimAction.RestoreAllFlagByClaimSequence(claimSeqQAReadyState);
                }
            }
        }

        [Test, Category("NewClaimAction3")]
        public void Verify_when_clicking_on_pencil_on_flag_level_sug_unit_sug_paid_and_sug_code_appear_enabled()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(3))
            {
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                string flag = paramLists["Flag"];
                string claimSeq = paramLists["ClaimSequence"];
                ClaimActionPage _claimAction;
                _claimAction = automatedBase.CurrentPage.NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(claimSeq);
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                StringFormatter.PrintLineBreak();
                if (!_claimAction.IsDeleteAllFlagsPresent())
                {
                    _claimAction.ClickOnEditAllFlagsIcon();
                    _claimAction.SelectReasonCode("ACC 1 - Accept Response To Logic Request");
                    _claimAction.ClickRestoreButtonOfEditAllFlagsSection();
                    _claimAction.ClickOnSaveButton();
                    Console.WriteLine("Restored flags.");
                }

                _claimAction.ClickOnEditForGivenFlag(flag);
                _claimAction.IsSugCodeTextFieldEnabled().ShouldBeFalse("Sug Code Text Field Enabled ?");
                _claimAction.IsSugUnitTextFieldEnabled().ShouldBeFalse("Sug Unit Text Field Enabled ?");
                _claimAction.IsSugPaidTextFieldEnabled().ShouldBeTrue("Sug Paid Text Field Enabled ?");
                _claimAction.ClickOnCancelEditFlagLink();
                StringFormatter.PrintLineBreak();
            }
        }



        [Test, Category("NewClaimAction3")]
        [Retrying(Times = 3)]
        public void Verify_delete_or_restore_the_single_flag_works_properly()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(3))
            {
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                string reasonCode = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "ReasonCode", "Value");

                string claimSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "ClaimSequence", "Value");
                ClaimActionPage _claimAction;
                _claimAction = automatedBase.CurrentPage.NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(claimSeq);
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                DeleteOrRestoreLineFlag(reasonCode,_claimAction);
            }
        }

        //[Test, Category("NewClaimAction3")]//US45708
        //public void Verify_that_audit_detail_should_update_without_reload_the_page_when_click_on_delete_flag()
        //{
        //    TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
        //    string claimSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName, "ClaimSequence", "Value");
        //    SearchByClaimSeqFromWorkList(claimSeq);
        //    try
        //    {
        //        if (_claimAction.IsRestoreFlagIconEnabled())
        //        {
        //            _claimAction.ClickOnRestoreFlagIcon();

        //        }
        //        _claimAction.ClickOnDeleteFlagIcon();
        //        _claimAction.ClickOnFirstFlagLineOfFlaggedLinesDiv();
        //        _claimAction.GetModifiedDateTextInAuditTrial().Split(' ')[0].ShouldBeEqual(
        //            DateTime.Now.Date.ToString("MM/dd/yyyy"), "Audit Date Should Today");
        //        _claimAction.GetActionTextInAuditTrial().ShouldBeEqual("Delete", "Action Should Deleted");

        //    }
        //    finally
        //    {
        //        if (_claimAction.IsRestoreFlagIconEnabled())
        //        {
        //            _claimAction.ClickOnRestoreFlagIcon();

        //        }
        //    }


        //}

        [Test, Category("NewClaimAction3"), Category("SchemaDependent")]//US58177
        public void Verify_that_sug_code_should_empty_for_UNL_or_COS_flag_in_flag_details()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(3))
            {
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                string claimSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "ClaimSequence", "Value");
                //SearchByClaimSeqFromWorkList(claimSeq);
                const string flagUNL = "UNL";
                const string flagCOS = "COS";
                ClaimActionPage _claimAction;
                automatedBase.CurrentPage.ClickOnSwitchClient().SwitchClientTo(ClientEnum.CVTY, true);
                _claimAction = automatedBase.CurrentPage.NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(claimSeq);
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();

                try
                {
                    _claimAction.ClickOnFlagLineByFlag(flagUNL);
                    _claimAction.GetClaimFlagAuditHistoryHeaderDetailByFlagAndLabel(flagUNL, "Sug Code")
                        .ShouldBeNullorEmpty(string.Format("Sug Code should empty for <{0}> flag", flagUNL));
                    _claimAction.ClickOnFlagLineByFlag(flagCOS);
                    _claimAction.GetClaimFlagAuditHistoryHeaderDetailByFlagAndLabel(flagCOS, "Sug Code")
                        .ShouldBeNullorEmpty(string.Format("Sug Code should empty for <{0}> flag", flagCOS));

                    StringFormatter.PrintMessageTitle(
                        "Verify sug proc should disabled and for any flag that does not allow Sug Procs when trying to add flag");
                    _claimAction.ClickAddIconButton();

                    _claimAction.SelectClaimLineToAddFlag();
                    _claimAction.SelectAddInFlagAdd(flagUNL);
                    _claimAction.IsSugCodeTextFieldEnabled()
                        .ShouldBeFalse(string.Format("Is Sug Code disabled for <{0}> flag", flagUNL));


                    _claimAction.SelectAddInFlagAdd(flagCOS);
                    _claimAction.IsSugCodeTextFieldEnabled()
                        .ShouldBeFalse(string.Format("Is Sug Code disabled for <{0}> flag", flagCOS));
                    _claimAction.ClickOnFlagLevelCancelLink();

                    StringFormatter.PrintMessageTitle(
                        "Verify sug proc should disabled and for any flag that does not allow Sug Procs when trying to modify flag");
                    _claimAction.ClickOnEditIconFlagLevelForLineEdit(1, flagUNL);
                    _claimAction.IsSugCodeTextFieldEnabled()
                        .ShouldBeFalse(string.Format("Is Sug Code disabled for <{0}> flag", flagUNL));


                    _claimAction.ClickOnEditIconFlagLevelForLineEdit(1, flagCOS);
                    _claimAction.IsSugCodeTextFieldEnabled()
                        .ShouldBeFalse(string.Format("Is Sug Code disabled for <{0}> flag", flagCOS));
                    _claimAction.ClickOnCancelLink();
                }
                finally
                {
                    if (_claimAction.IsAddFlagPanelPresent())
                        _claimAction.ClickOnFlagLevelCancelLink();
                    else if (_claimAction.IsEditFlagAreaPresent())
                        _claimAction.ClickOnCancelLink();

                }
            }
        }


        [Test, Category("NewClaimAction3")] //US65038
        public void Validate_medical_code_desc_popup_under_trigger_line()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(3))
            {
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var claimSeq = paramLists["ClaimSequence"];
                var mvcPageUrl = paramLists["MVCPageUrl"];
                ClaimActionPage _claimAction;
                NewPopupCodePage _newpopupCodePage;
                _claimAction = automatedBase.CurrentPage.NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(claimSeq);
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                try
                {
                    _claimAction.ClickAddIconButton();
                    _claimAction.SelectClaimLineToAddFlag("2");
                    _claimAction.SelectAddInFlagAdd("UNB");
                    _newpopupCodePage = _claimAction.ClickOnTriggerClaimLineResultForUnknownProcCodeByRow(1);
                    var popupUrl = _newpopupCodePage.GetPopUpCurrentUrl();
                    popupUrl.ShouldNotContain("title", "Url doesnot have title Query String parameter");

                    popupUrl.AssertIsContained(mvcPageUrl,
                        string.Format("Procedure code pop up link points to mvc page <{0}>", mvcPageUrl));
                    popupUrl.ShouldNotContain("Code=Unknown",
                        "MVC page query string parameter has code value. Unknown displayed should be false.");
                    popupUrl.AssertIsContained("Code=00000",
                        "MVC page query string parameter has code value. Code 00000 displayed is true.");
                    _newpopupCodePage.GetTextValueinLiTag(1).ShouldBeEqual(("Code: Unknown"),
                        "Code displayed is same value as in type of page header and query string parameter is ignored.");
                    _newpopupCodePage.GetTextValueinLiTag(2).ShouldBeEqual(
                        string.Concat("Type:", _newpopupCodePage.GetPopupHeaderText()),
                        "Type is same as page header of the pop up.");

                    _claimAction = _newpopupCodePage.ClosePopupOnNewClaimActionPage("Unknown");


                }
                finally
                {
                    _claimAction.CloseAllExistingPopupIfExist();
                    _claimAction.ClickOnFlagLevelCancelLink();
                }
            }
        }

        [Test, Category("NewClaimAction3")] //US65040
        public void Validate_medical_revenue_code_desc_popup_under_claim_line_for_unknown_code()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(3))
            {
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var claimSeq = paramLists["ClaimSequence"];
                var mvcPageUrl = paramLists["MVCPageUrl"];
                var revCodeDesc = paramLists["RevCodeDescription"];
                ClaimActionPage _claimAction;
                NewPopupCodePage _newpopupCodePage;
                _claimAction = automatedBase.CurrentPage.NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(claimSeq);
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                try
                {
                    StringFormatter.PrintMessageTitle("Verify Rev Code popup in New Claim Action Page");
                    var revCode = _claimAction.GetValueOfClaimLineByRowAndLabel(3, "R:");
                    _newpopupCodePage = _claimAction.ClickOnRevCodeOfClaimLineByRow(3, "Unknown");
                    var popupUrl = _newpopupCodePage.GetPopUpCurrentUrl();
                    popupUrl.ShouldNotContain("title", "Url doesnot have title Query String parameter");
                    popupUrl.AssertIsContained(mvcPageUrl,
                        string.Format("Revenue code pop up url has mvc page link <{0}> ", mvcPageUrl));
                    popupUrl.ShouldNotContain("code=Unknown",
                        "MVC page query string parameter has code value. Unknown displayed should not display");
                    popupUrl.AssertIsContained(string.Format("code={0}", revCode),
                        string.Format("MVC page query string parameter has code value. Code {0} displayed is true.",
                            revCode));
                    _newpopupCodePage.GetTextValueinLiTag(1)
                        .ShouldBeEqual("Code: Unknown",
                            "Code displayed is same value as in type of page header and query string parameter is ignored.");
                    _newpopupCodePage.GetTextValueinLiTag(2)
                        .ShouldBeEqual(string.Concat("Type: ", _newpopupCodePage.GetPopupHeaderText()),
                            "Type is same as page header of the pop up.");
                    _newpopupCodePage.GetTextValueinLiTag(3)
                        .ShouldBeEqual(string.Concat("Description\r\n", revCodeDesc), "Description");
                    _claimAction = _newpopupCodePage.ClosePopupOnNewClaimActionPage(revCode);

                    StringFormatter.PrintMessageTitle("Verify Rev Code popup in Patient Claim History Page");
                    _claimAction.ClickPatientHistoryIcon();
                    var claimHistoryPage = _claimAction.SwitchToPatientClaimHistory();
                    claimHistoryPage.ClickOnRevenueCodeByRevCodeAndSwitch("Revenue Code - Unknown", revCode);
                    popupUrl = _newpopupCodePage.GetPopUpCurrentUrl();
                    popupUrl.AssertIsContained(mvcPageUrl,
                        string.Format("Revenue code pop up link contains mvc page link <{0}>", mvcPageUrl));
                    popupUrl.ShouldNotContain("code=Unknown",
                        "MVC page query string parameter has code value. <Unknown> dispalyed should be false.");
                    popupUrl.AssertIsContained("code=" + revCode,
                        string.Format("MVC page query string parameter has code value. Code <{0}> displayed is true.",
                            revCode));
                    _newpopupCodePage.GetTextValueinLiTag(1).ShouldBeEqual("Code: Unknown", "Code");
                    _newpopupCodePage.GetTextValueinLiTag(2)
                        .ShouldBeEqual(string.Concat("Type: ", _newpopupCodePage.GetPopupHeaderText()),
                            "Type is same as page header of the pop up.");
                    _newpopupCodePage.GetTextValueinLiTag(3)
                        .ShouldBeEqual(string.Concat("Description\r\n", revCodeDesc), "Description");
                    _newpopupCodePage.ClosePopup("Revenue Code - Unknown");
                    claimHistoryPage.SwitchToNewClaimActionPage(true);
                }
                finally
                {
                    _claimAction.CloseAllExistingPopupIfExist();
                }
            }
        }

        [Test, Category("NewClaimAction3")] //US65042
        public void Validate_medical_revenue_code_desc_popup_under_claim_line_for_known_code()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(3))
            {
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var claimSeq = paramLists["ClaimSequence"];
                var mvcPageUrl = paramLists["MVCPageUrl"];
                ClaimActionPage _claimAction;
                NewPopupCodePage _newpopupCodePage;
                _claimAction = automatedBase.CurrentPage.NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(claimSeq);
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();

                try
                {
                    StringFormatter.PrintMessageTitle("Verify Rev Code popup in New Claim Action Page");
                    var revCode = _claimAction.GetValueOfClaimLineByRowAndLabel(3, "R:");
                    var revCodeDesc = _claimAction.GetLongProcCodeDescription(revCode, "REV");
                    _newpopupCodePage = _claimAction.ClickOnRevCodeOfClaimLineByRow(3, revCode);
                    var popupUrl = _newpopupCodePage.GetPopUpCurrentUrl();
                    popupUrl.ShouldNotContain("title", "Url doesnot have title Query String parameter");

                    popupUrl.AssertIsContained(mvcPageUrl,
                        string.Format("Revenue code pop up link points to mvc page <{0}>", mvcPageUrl));
                    _newpopupCodePage.GetTextValueinLiTag(1).ShouldBeEqual(string.Concat("Code: ", revCode), "Code");
                    _newpopupCodePage.GetTextValueinLiTag(2).ShouldBeEqual(
                        string.Concat("Type: ", _newpopupCodePage.GetPopupHeaderText()),
                        "Type is same as page header of the pop up.");
                    _newpopupCodePage.GetTextValueinLiTag(3)
                        .ShouldBeEqual(string.Concat("Description\r\n", revCodeDesc), "Description");

                    _claimAction = _newpopupCodePage.ClosePopupOnNewClaimActionPage(revCode);

                    StringFormatter.PrintMessageTitle("Verify Rev Code popup in Patient Claim History Page");
                    _claimAction.ClickPatientHistoryIcon();
                    var claimHistoryPage = _claimAction.SwitchToPatientClaimHistory();
                    claimHistoryPage.ClickOnRevenueCodeByRevCodeAndSwitch("Revenue Code - " + revCode, revCode);
                    popupUrl = _newpopupCodePage.GetPopUpCurrentUrl();

                    popupUrl.AssertIsContained(mvcPageUrl,
                        string.Format("Revenue code pop up link points to mvc page <{0}>", mvcPageUrl));
                    _newpopupCodePage.GetTextValueinLiTag(1).ShouldBeEqual(string.Concat("Code: ", revCode), "Code");
                    _newpopupCodePage.GetTextValueinLiTag(2).ShouldBeEqual(
                        string.Concat("Type: ", _newpopupCodePage.GetPopupHeaderText()),
                        "Type is same as page header of the pop up.");
                    _newpopupCodePage.GetTextValueinLiTag(3)
                        .ShouldBeEqual(string.Concat("Description\r\n", revCodeDesc), "Description");
                    _newpopupCodePage.ClosePopup("Revenue Code - " + revCode);
                    claimHistoryPage.SwitchToNewClaimActionPage(true);
                }
                finally
                {
                    _claimAction.CloseAllExistingPopupIfExist();
                }
            }
        }

        [Test, Category("NewClaimAction3"), Category("OnDemand")]//US68636
        [NonParallelizable]
        public void Verify_Switch_Setting_restricts_Switch_action_in_the_Claim_Action_page()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(3))
            {
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var claimSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "ClaimSequence", "Value");
                var flag = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                    "Flag", "Value");
                ClaimActionPage _claimAction = null;
                try
                {
                    StringFormatter.PrintMessageTitle(
                        $"Updating {ConfigurationSettingsEnum.NonReverseFlag.GetStringValue()} to 'NO'");
                    automatedBase.CurrentPage.GetCommonSql.UpdateSpecificClientDataInDB("allow_add_switch = 'D'",
                        ClientEnum.SMTST.ToString());
                    automatedBase.CurrentPage.RefreshPage(false);
                    automatedBase.CurrentPage.IsRoleAssigned<UserProfileSearchPage>(new List<string> {automatedBase.EnvironmentManager.Username},
                        RoleEnum.ClaimsManager.GetStringValue()).ShouldBeFalse(
                        $"Is {AuthorityAssignedEnum.HCIUserCanAddEditsToClientHistoryClaims.GetStringValue()}  present for current user<{EnvironmentManager.Username}>");
                    _claimAction = automatedBase.CurrentPage.NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(claimSeq);
                    _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                    _claimAction.ClickOnEditIconFlagLevelForLineEdit(1, flag);
                    _claimAction.ClickOnSwitchIconInEditFlagSectionInClaimLine();
                    _claimAction.WaitForWorkingAjaxMessage();
                    _claimAction.IsPageErrorPopupModalPresent()
                        .ShouldBeTrue("Pop up error message should be present");
                    _claimAction.GetPageErrorMessage()
                        .ShouldBeEqual("The flag cannot be switched. This client does not allow flags to be added.");
                    _claimAction.ClosePageError();
                }

                finally
                {
                    StringFormatter.PrintMessageTitle(
                        $"Finally Block \n Updating {ConfigurationSettingsEnum.NonReverseFlag.GetStringValue()} to 'YES'");
                    _claimAction?.GetCommonSql.UpdateSpecificClientDataInDB("allow_add_switch = 'A'",
                        ClientEnum.SMTST.ToString());
                }
            }
        }

        [Test, Category("NewClaimAction3")] //US69376
        public void Validate_note_character_limit_in_claim_action_for_edit_add_transfer_transferApprove_notes()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(3))
            {
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var claimSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName,
                    "ClaimSequence", "Value");
                var notes = new string('a', 1994);
                ClaimActionPage _claimAction;
                _claimAction = automatedBase.CurrentPage.NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(claimSeq);
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();

                StringFormatter.PrintMessage("Validate character limit for note field for Edit Flag");
                _claimAction.ClickOnEditIconFlagLevelForLineEdit(1, "UPA");
                ValidateCharacterLimitOnNotes(notes,_claimAction);
                StringFormatter.PrintMessage("Validate character limit for note field for Edit All flags On Line");
                _claimAction.ClickEditAllFlagsOnLineButton();
                ValidateCharacterLimitOnNotes(notes,_claimAction);
                StringFormatter.PrintMessage("Validate character limit for note field for Edit All flags On Claim");
                _claimAction.ClickOnEditAllFlagsIcon();
                ValidateCharacterLimitOnNotes(notes,_claimAction);
                StringFormatter.PrintMessage("Validate character limit for note field for Transfer Claim");
                _claimAction.ClickOnTransferButton();
                ValidateCharacterLimitOnNotes(notes,_claimAction);
                StringFormatter.PrintMessage("Validate character limit for note field for Transfer/Approve Claim");
                _claimAction.ClickOnTransferApproveButton();
                ValidateCharacterLimitOnNotes(notes,_claimAction);
                StringFormatter.PrintMessage("Validate character limit for note field for Add Flag ");
                _claimAction.ClickAddIconButton();
                _claimAction.SelectClaimLineToAddFlag();
                _claimAction.SelectAddInFlagAdd("AADD");
                _claimAction.SetLengthyNoteToVisbleTextarea("flag",notes, true);
                _claimAction.GetNoteOfVisibleTextarea(true).Length.ShouldBeEqual(1993,
                    "Character limit should be 1993 or less, where 7 characters are separated for <p></p> tag.");
                _claimAction.ClickOnFlagLevelCancelLink();
            }
        }


        [Test] //US69709
        public void Verify_rule_repository_notes_are_appearing_for_appropriate_flags()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(3))
            {
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramsLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);

                var claimSeqList = paramsLists["ClaimSequence"].Split(';').ToList();
                var flagSeqList = paramsLists["FlagSequence"].Split(';').ToList();
                ClaimActionPage _claimAction;
                _claimAction = automatedBase.CurrentPage.NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(claimSeqList[0]);
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                _claimAction.ClickOnFirstFlagLineOfFlaggedLinesDiv();
                _claimAction.IsRuleNoteFieldPresent().ShouldBeTrue("Rule Note field is displayed");

                StringFormatter.PrintMessageTitle(
                    "Verifying Rule notes are getting displayed for the expected flags and not for others");
                foreach (var claimSequence in claimSeqList)
                {
                    SearchByClaimSeqFromWorkList(claimSequence,_claimAction);
                    _claimAction.WaitForWorking();

                    var countOfFlags = _claimAction.GetCountOfTotalFlagsAssociatedToAClaim(claimSequence);

                    //Loops through and clicks all the flags associated with the claim. Verifies Rule Note display for required flags.
                    for (var j = 0; j < countOfFlags; j++)
                    {
                        var flagCodeinFlaggedLines = _claimAction.GetFlagCodeFromFlaggedLines(j + 1);

                        if (flagSeqList.Contains(flagCodeinFlaggedLines))
                        {
                            _claimAction.ClickOnFlagDivInFlaggedLinesByRow(j + 1);
                            var ruleNoteFromUI = _claimAction.GetFlagDetailsTextByLabel("Rule Note:");
                            var fetchedRuleNoteFromDB =
                                _claimAction.GetRuleNoteForGivenClaimSeqFromDB(claimSequence, flagCodeinFlaggedLines);
                            ruleNoteFromUI.ShouldBeEqual(fetchedRuleNoteFromDB,
                                "The rule note is getting displayed for claim : " + claimSequence + " " +
                                "and flag : " + flagCodeinFlaggedLines);
                        }
                        else
                        {
                            _claimAction.ClickOnFlagDivInFlaggedLinesByRow(j + 1);
                            _claimAction.GetFlagDetailsTextByLabel("Rule Note:")
                                .ShouldBeNullorEmpty("Rule Note is correctly shown as empty for Flag : " +
                                                     flagCodeinFlaggedLines);
                        }
                    }
                }
            }
        }

        [Test] //CAR 185
        public void Verify_PCI_coder_worklist_authority_and_menu_option()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(3))
            {
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramsLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                ClaimActionPage _claimAction;

                if (Compare(UserType.CurrentUserType, UserType.HCIADMIN3, StringComparison.OrdinalIgnoreCase) != 0)
                    automatedBase.CurrentPage = automatedBase.CurrentPage.Logout().LoginAsHciAdminUser3();


                automatedBase.CurrentPage.IsRoleAssigned<UserProfileSearchPage>(new List<string> {automatedBase.EnvironmentManager.Username},
                    RoleEnum.CoderWorkList.GetStringValue()).ShouldBeTrue(
                    $"Is CV Coder Work List present for current user<{EnvironmentManager.Username}>");

                automatedBase.CurrentPage.IsRoleAssigned<QuickLaunchPage>(new List<string> {paramsLists["UserID"]},
                    RoleEnum.CoderWorkList.GetStringValue()).ShouldBeFalse(
                    $"Is CV Coder Work List present for client user<{paramsLists["UserID"]}>");

                automatedBase.QuickLaunch.IsPciCoderClaimsSubMenuPresent()
                    .ShouldBeTrue("CV Coder Claims should be present when authority is assigned.");
                _claimAction = automatedBase.CurrentPage.NavigateToCVCodersClaim();
                var loopCount = 0;
                while (!_claimAction.IsElementPresent(ClaimActionPageObjects.NoDataAvailableXPath, How.XPath) &&
                       loopCount <= 3)
                {
                    CheckForClaimAction(ClaimStatusTypeEnum.CotivitiUnreviewed.GetStringValue(),_claimAction);
                    loopCount++;
                }
            }
        }

        [Test, Category("OnDemand")]//CAR-277
        [NonParallelizable]
        public void Verify_switch_icon_for_core_flag_for_different_client_setting()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(3))
            {
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramsLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                ClaimActionPage _claimAction = null;
                ClaimSearchPage _claimSearch;
                AppealActionPage _appealAction = null;
                _claimAction = automatedBase.CurrentPage.NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage("1458018-0");
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                var flagWithoutTriggerClaim = paramsLists["FlagWithoutTriggerClaim"];
                var flagWithTriggerClaim = paramsLists["FlagWithTriggerClaim"];
                var claseqWithoutTriggerClaim = paramsLists["ClaseqWithoutTriggerClaim"];
                var claseqWithTriggerClaim1 = paramsLists["ClaseqWithTriggerClaim1"];
                var claseqWithTriggerClaim2 = paramsLists["ClaseqWithTriggerClaim2"];
                var claseqWithAppealAndTriggerClaim1 = paramsLists["ClaseqWithAppealAndTriggerClaim1"];
                var claseqWithAppealAndTriggerClaim2 = paramsLists["ClaseqWithAppealAndTriggerClaim2"];
                var userFirstName = paramsLists["FirstName"];
                var userLastName = paramsLists["LastName"];
                var expectedPopupMessage1 =
                    "The flag cannot be switched. This client does not allow flags to be added to reviewed claims.";
                var expectedPopupMessage2 =
                    "This client does not allow flags to be switched on claims. Click OK to override and continue with switch.";
                var optionChecked = false;
                try
                {
                    _claimAction.RevertScriptForSwitchFunctionalityCoreFlag(
                        claseqWithTriggerClaim1, claseqWithTriggerClaim2, claseqWithoutTriggerClaim, "R");

                    _claimAction.RevertScriptForSwitchFunctionalityCoreFlag(
                        claseqWithAppealAndTriggerClaim1, claseqWithAppealAndTriggerClaim2, claseqWithoutTriggerClaim,
                        "R");

                    var countOfCoreFlag = _claimAction.GetCountOfTotalFlagsAssociatedToAClaim(claseqWithTriggerClaim1);
                    var countOfCoreFlagForAppeal =
                        _claimAction.GetCountOfTotalFlagsAssociatedToAClaim(claseqWithAppealAndTriggerClaim1);

                    if (countOfCoreFlagForAppeal == 0)
                    {
                        var temp = claseqWithAppealAndTriggerClaim2;
                        claseqWithAppealAndTriggerClaim2 = claseqWithAppealAndTriggerClaim1;
                        claseqWithAppealAndTriggerClaim1 = temp;
                    }

                    if (countOfCoreFlag == 0)
                    {
                        var temp = claseqWithTriggerClaim2;
                        claseqWithTriggerClaim2 = claseqWithTriggerClaim1;
                        claseqWithTriggerClaim1 = temp;
                    }

                    automatedBase.CurrentPage.IsRoleAssigned<UserProfileSearchPage>(new List<string> {automatedBase.EnvironmentManager.Username},
                        RoleEnum.ClaimsManager.GetStringValue()).ShouldBeFalse(
                        $"Is {AuthorityAssignedEnum.HCIUserCanAddEditsToClientHistoryClaims.GetStringValue()} and {AuthorityAssignedEnum.ModifyAutoReviewedFlags.GetStringValue()} present for current user<{automatedBase.EnvironmentManager.Username}>");

                    automatedBase.CurrentPage.IsRoleAssigned<UserProfileSearchPage>(new List<string> {userFirstName, userLastName},
                        RoleEnum.ClaimsManager.GetStringValue(), false).ShouldBeTrue(
                        $"Is {AuthorityAssignedEnum.HCIUserCanAddEditsToClientHistoryClaims.GetStringValue()} present for current user<{userFirstName} {userLastName}>");

                    _claimSearch = automatedBase.CurrentPage.NavigateToClaimSearch();

                    StringFormatter.PrintMessage(
                        $"Setting {ConfigurationSettingsEnum.ModifyCoreFlag.GetStringValue()} to T and " +
                        $"{ConfigurationSettingsEnum.NonReverseFlag.GetStringValue()} to 'YES - only to client unreviewed claims'");

                    automatedBase.CurrentPage.GetCommonSql.UpdateSpecificClientDataInDB(
                        "CAN_MODIFY_CORE_FLAGS = 'T', allow_add_switch = 'C'", ClientEnum.SMTST.ToString());
                    automatedBase.CurrentPage.RefreshPage(false);

                    optionChecked = true;

                    VerifyEnableDisabledSwitchIconAndPopupMessageInAppealAction(claseqWithAppealAndTriggerClaim1,
                        claseqWithAppealAndTriggerClaim2, flagWithTriggerClaim,automatedBase.CurrentPage,_claimAction,_appealAction);

                    _claimAction = automatedBase.CurrentPage.NavigateToClaimSearch()
                        .SearchByClaimSequenceToNavigateToClaimActionPage(claseqWithoutTriggerClaim, true);

                    StringFormatter.PrintMessageTitle(
                        "Verify Switch Icon for claims without Trigger Claim and Status as Client Reviewed");

                    VerifyEnableDisabledSwitchIconAndPopupMessageInClaimAction(null, flagWithoutTriggerClaim,_claimAction,
                        expectedPopupMessage1);

                    StringFormatter.PrintMessageTitle(
                        "Verify Switch Icon for claims with Trigger Claim and Status as Client Reviewed");
                    VerifyEnableDisabledSwitchIconAndPopupMessageInClaimAction(claseqWithTriggerClaim1,
                        flagWithTriggerClaim,_claimAction, expectedPopupMessage1);

                    StringFormatter.PrintMessageTitle(
                        "Verify Switch Icon for claims with Trigger Claim and Status as Client Reviewed, having authority of" +
                        AuthorityAssignedEnum.HCIUserCanAddEditsToClientHistoryClaims.GetStringValue());
                    var newClaimSearch = automatedBase.CurrentPage.Logout().LoginAsHciAdminUser()
                        .NavigateToClaimSearch();

                    _claimAction =
                        newClaimSearch.SearchByClaimSequenceToNavigateToClaimActionPage(claseqWithTriggerClaim1);
                    VerifyEnableDisabledSwitchIconAndPopupMessageInClaimAction(null, flagWithTriggerClaim,_claimAction,
                        expectedPopupMessage2,true);

                    StringFormatter.PrintMessageTitle(
                        "Verify Switch Icon for claims with Trigger Claim and Status as Client UnReviewed");
                    _claimAction.RevertScriptForSwitchFunctionalityCoreFlag(
                        claseqWithTriggerClaim1, claseqWithTriggerClaim2, claseqWithoutTriggerClaim, "U");

                    newClaimSearch = _claimAction.Logout().LoginAsHciAdminUser3()
                        .NavigateToClaimSearch();

                    _claimAction =
                        newClaimSearch.SearchByClaimSequenceToNavigateToClaimActionPage(claseqWithTriggerClaim2);
                    VerifyEnableDisabledSwitchIconAndPopupMessageInClaimAction(null, flagWithTriggerClaim, _claimAction,null, true,
                        false);

                    StringFormatter.PrintMessageTitle(
                        "Verify Switch Icon for claims with Trigger Claim and  Allow Add/Switch Flag is Yes");
                    _claimAction.RevertScriptForSwitchFunctionalityCoreFlag(
                        claseqWithTriggerClaim1, claseqWithTriggerClaim2, claseqWithoutTriggerClaim, "R");
                    _claimAction.RevertScriptForSwitchFunctionalityCoreFlag(
                        claseqWithAppealAndTriggerClaim1, claseqWithAppealAndTriggerClaim2, claseqWithoutTriggerClaim,
                        "R");

                    StringFormatter.PrintMessage(
                        $"Setting {ConfigurationSettingsEnum.NonReverseFlag.GetStringValue()} to 'YES'");

                    _claimAction.GetCommonSql.UpdateSpecificClientDataInDB("allow_add_switch = 'A'",
                        ClientEnum.SMTST.ToString());
                    _claimAction.RefreshPage(false);
                    _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();

                    SearchByClaimSeqFromWorkList(claseqWithoutTriggerClaim, _claimAction,true);

                    VerifyEnableDisabledSwitchIconAndPopupMessageInClaimAction(claseqWithTriggerClaim1,
                        flagWithTriggerClaim,_claimAction, null, true, false);

                    StringFormatter.PrintMessage(
                        $"Setting {ConfigurationSettingsEnum.NonReverseFlag.GetStringValue()} to 'NO'");
                    _claimAction.GetCommonSql.UpdateSpecificClientDataInDB("allow_add_switch = 'D'",
                        ClientEnum.SMTST.ToString());
                    _claimAction.RefreshPage(false);
                    _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();

                    SearchByClaimSeqFromWorkList(claseqWithoutTriggerClaim,_claimAction, true);

                    VerifyEnableDisabledSwitchIconAndPopupMessageInClaimAction(claseqWithTriggerClaim2,
                        flagWithTriggerClaim,_claimAction, isDisabled: true);

                    VerifyEnableDisabledSwitchIconAndPopupMessageInAppealAction(claseqWithAppealAndTriggerClaim2, null,
                        flagWithTriggerClaim, automatedBase.CurrentPage,_claimAction,_appealAction,true);

                    _claimAction.RevertScriptForSwitchFunctionalityCoreFlag(
                        claseqWithTriggerClaim1, claseqWithTriggerClaim2, claseqWithoutTriggerClaim, "R");

                    StringFormatter.PrintMessageTitle(
                        "Verify Switch Icon for claims with Trigger Claim and unchecked the Allow All Users To Modify Core Flags ");

                    StringFormatter.PrintMessage(
                        $"Setting {ConfigurationSettingsEnum.ModifyCoreFlag.GetStringValue()} to F and {ConfigurationSettingsEnum.NonReverseFlag.GetStringValue()} to 'YES'");

                    _claimAction.GetCommonSql.UpdateSpecificClientDataInDB(
                        "CAN_MODIFY_CORE_FLAGS = 'F', allow_add_switch = 'A'", ClientEnum.SMTST.ToString());

                    optionChecked = false;

                    automatedBase.CurrentPage.NavigateToClaimSearch()
                        .SearchByClaimSequenceToNavigateToClaimActionPage(claseqWithoutTriggerClaim, true);

                    _claimAction.RefreshPage(false);
                    _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();

                    VerifyEnableDisabledSwitchIconAndPopupMessageInClaimAction(null, flagWithTriggerClaim,_claimAction,
                        isDisabled: true);
                }

                finally
                {
                    StringFormatter.PrintMessageTitle("Finally Block \n Reverting client configurations");
                    /*if (automatedBase.CurrentPage.GetPageHeader().Equals(PageHeaderEnum.AppealAction.GetStringValue()))
                        _appealAction.ClickOnSearchIconToNavigateAppealSearchPage();*/
                    if (optionChecked)
                    {
                        StringFormatter.PrintMessage(
                            $"Setting {ConfigurationSettingsEnum.ModifyCoreFlag.GetStringValue()} to F and {ConfigurationSettingsEnum.NonReverseFlag.GetStringValue()} to 'YES'");
                        _claimAction.GetCommonSql.UpdateSpecificClientDataInDB(
                            "CAN_MODIFY_CORE_FLAGS = 'F', allow_add_switch = 'A'", ClientEnum.SMTST.ToString());
                    }

                    automatedBase.CurrentPage.CloseAnyTabIfExist();
                }
            }
        }

        [Test,Category("NewClaimAction3")]//CAR-1518
        public void Verify_page_error_modal_popup_doesnot_shows_error_message()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(3))
            {
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var claimSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName,
                    "ClaimSequence", "Value");

                ClaimActionPage _claimAction;
                _claimAction = automatedBase.CurrentPage.NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(claimSeq);
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                _claimAction.RestoreAllFlagByClaimSequence(claimSeq);
                _claimAction.DeleteClaimAuditRecordExceptAdd(claimSeq);
                _claimAction.RefreshPage(false);
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();

                for (var i = 0; i < 2; i++)
                {
                    _claimAction.ClickEditAllFlagsOnLineButton();
                    var action = _claimAction.IsDeleteAllFlagIconPresentOnLine() ? "Delete" : "Restore";
                    _claimAction.ClickAndSaveOnEditAllFlagOnTheLineIcon();
                    if (_claimAction.IsPageErrorPopupModalPresent())
                    {
                        _claimAction.GetPageErrorMessage()
                            .Contains(string.Format("An error occurred trying to {0} Flags", action))
                            .ShouldBeFalse("Popup Should not display error message");
                        _claimAction.ClosePageError();
                    }
                    else
                        Console.WriteLine("No Popup and Issue display");
                }
            }
        }


        [Test] //TE-803
        public void Verify_Navigating_Away_From_Claim_Action_Page_Claim_Lock_Is_Released()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(3))
            {
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var claSeq = paramLists["ClaimSequence"].Split(',');
                ClaimActionPage _claimAction;
                ClaimSearchPage _claimSearch;
                _claimAction = automatedBase.CurrentPage.NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(claSeq[0]);
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();

                //Navigate to dashboard
                _claimAction.NavigateToDashboard();
                VerifyPageHeaderAndErrorMessage(PageHeaderEnum.Dashboard.GetStringValue(),automatedBase.CurrentPage);
                _claimAction.IsClaimLockedByClaimSequence(ClientEnum.SMTST.ToString(),
                        claSeq[0].Split('-')[0], claSeq[0].Split('-')[1],
                        automatedBase.EnvironmentManager.HciAdminUsername3)
                    .ShouldBeFalse($"Is claim {claSeq[0]} locked?");
                _claimAction = automatedBase.CurrentPage.NavigateToClaimSearch()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(claSeq[0]);

                //Navigate to Appeal Search
                _claimAction.NavigateToAppealSearch();
                VerifyPageHeaderAndErrorMessage(PageHeaderEnum.AppealSearch.GetStringValue(),automatedBase.CurrentPage);
                _claimAction.IsClaimLockedByClaimSequence(ClientEnum.SMTST.ToString(),
                        claSeq[0].Split('-')[0], claSeq[0].Split('-')[1],
                        automatedBase.EnvironmentManager.HciAdminUsername3)
                    .ShouldBeFalse($"Is claim {claSeq[0]} locked?");
                _claimAction = automatedBase.CurrentPage.NavigateToClaimSearch()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(claSeq[0]);

                //Navigate to Claim Search clicking on claim search icon
                _claimSearch = _claimAction.ClickClaimSearchIcon();
                VerifyPageHeaderAndErrorMessage(PageHeaderEnum.ClaimSearch.GetStringValue(),automatedBase.CurrentPage);
                _claimAction.IsClaimLockedByClaimSequence(ClientEnum.SMTST.ToString(),
                        claSeq[0].Split('-')[0], claSeq[0].Split('-')[1],
                        automatedBase.EnvironmentManager.HciAdminUsername3)
                    .ShouldBeFalse($"Is claim {claSeq[0]} locked?");

                _claimAction = _claimSearch
                    .SearchByClaimSequenceToNavigateToClaimActionPage(claSeq[0]);

                //Search From Claim Action
                if (!_claimAction.IsWorkListControlDisplayed())
                    _claimAction.ClickWorkListIcon();
                _claimAction.ClickSearchIcon().SearchByClaimSequence(claSeq[1]);
                _claimAction.IsClaimLockedByClaimSequence(ClientEnum.SMTST.ToString(), claSeq[0].Split('-')[0],
                    claSeq[0].Split('-')[1],
                    automatedBase.EnvironmentManager.HciAdminUsername3).ShouldBeFalse($"Is claim {claSeq[0]} locked?");
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                if (!_claimAction.IsWorkListControlDisplayed())
                    _claimAction.ClickWorkListIcon();
                _claimAction = _claimAction.ClickSearchIcon().SearchByClaimSequence(claSeq[0]);
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();


                //Create Worklist From Claim Action
                if (!_claimAction.IsWorkListControlDisplayed())
                    _claimAction.ClickWorkListIcon();
                _claimAction.ToggleWorkListToPci();
                _claimAction.ClickOnCreateButton();
                _claimAction.IsClaimLockedByClaimSequence(ClientEnum.SMTST.ToString(), claSeq[0].Split('-')[0],
                    claSeq[0].Split('-')[1],
                    automatedBase.EnvironmentManager.HciAdminUsername3).ShouldBeFalse($"Is claim {claSeq[0]} locked?");
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                if (!_claimAction.IsWorkListControlDisplayed())
                    _claimAction.ClickWorkListIcon();

                _claimAction = automatedBase.CurrentPage.NavigateToClaimSearch()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(claSeq[0]);

                //Browser Back Button

                _claimSearch = _claimAction.ClickOnBrowserBackButtonToClaimSearch();
                _claimAction.WaitForStaticTime(500);
                VerifyPageHeaderAndErrorMessage(PageHeaderEnum.ClaimSearch.GetStringValue(),automatedBase.CurrentPage);
                _claimAction.IsClaimLockedByClaimSequence(ClientEnum.SMTST.ToString(), claSeq[0].Split('-')[0],
                    claSeq[0].Split('-')[1],
                    automatedBase.EnvironmentManager.HciAdminUsername3).ShouldBeFalse($"Is claim {claSeq} locked?");
                _claimSearch.ClickOnFindClaimIcon();
                _claimAction = _claimSearch
                    .SearchByClaimSequenceToNavigateToClaimActionPage(claSeq[0]);


                //Next Button In Claim Action
                _claimAction.ClickOnNextButton();
                _claimAction.IsClaimLockedByClaimSequence(ClientEnum.SMTST.ToString(), claSeq[0].Split('-')[0],
                    claSeq[0].Split('-')[1],
                    automatedBase.EnvironmentManager.HciAdminUsername3).ShouldBeFalse($"Is claim {claSeq[0]} locked?");
            }
        }

        #endregion

        #region PRIVATE METHODS

        private void VerifyPageHeaderAndErrorMessage(string pageHeader,NewDefaultPage CurrentPage)
        {
            CurrentPage.IsAlertBoxPresent().ShouldBeFalse("Popup should not be displayed notifying confirmation for leaving.");
            CurrentPage.GetPageHeader().ShouldBeEqual(pageHeader, $"User should be navigated to {pageHeader} Page");
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
                .ShouldBeEqual(expectedFlagDetails[1], "Author Value should match with the database");
            _claimAction.GetFlagDetailsValueSecondRow(2)
                .ShouldBeEqual(expectedFlagDetails[2], "Source Value should match with the database");
            _claimAction.GetFlagDetailsValueSecondRow(3)
                .ShouldBeEqual(_claimAction.GetDefenseRationaleOfFlagseqBasedOnEffectiveDateAndDOS(claimSeq),
                    "Defense Rationale Value should match with the database");
            
        }

        private void VerifyEnableDisabledSwitchIconAndPopupMessageInClaimAction(string claseq, string flag,ClaimActionPage _claimAction, string popupMessage = null, bool isSwitchable = false, bool isPopup = true, bool isDisabled = false)
        {
            if (!string.IsNullOrEmpty(claseq))
                SearchByClaimSeqFromWorkList(claseq, _claimAction,false);
            _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
            if (isDisabled)
            {
                _claimAction.IsSwitchEditIconByFlagEnabled(flag)
                    .ShouldBeFalse("Is Switch Icon for Core Flag Enabled");
                return;
            }
            _claimAction.ClickSwitchEditIconByFlag(flag, false);
            if (isPopup)
            {
                _claimAction.IsPageErrorPopupModalPresent().ShouldBeTrue("Is Popup Modal display?");
                _claimAction.GetPageErrorMessage().ShouldBeEqual(popupMessage, "Is Message Equal?");
            }

            if (isSwitchable)
            {
                if (isPopup) _claimAction.ClickOkCancelOnConfirmationModal(true);
                _claimAction.GetCountOfDeletedFlags().ShouldBeEqual(1, "Flag should switched");
                return;
            }
            _claimAction.ClosePageError();


        }

        private void VerifyEnableDisabledSwitchIconAndPopupMessageInAppealAction(string claseq1, string claseq2, string flag,NewDefaultPage CurrentPage, ClaimActionPage _claimAction,AppealActionPage _appealAction, bool isDisabled = false)
        {
            if (!CurrentPage.GetPageHeader().Equals(PageHeaderEnum.AppealAction.GetStringValue()))
            {
                _appealAction = _claimAction.NavigateToAppealSearch()
                    .FindByClaimSequenceToNavigateAppealAction(claseq1);
            }
            //_appealAction.HandleAutomaticallyOpenedActionPopup(2);
            _appealAction.ClickOnAdjustIconByFlagAndClaimSequence(claseq1, flag);
            if (isDisabled)
            {
                _appealAction.IsSwitchDeleteIconDisabledByRow().ShouldBeTrue("Is Switch Icon disabled?");
                _appealAction.ClickOnSearchIconToNavigateAppealSearchPage();
                return;
            }

            _appealAction.IsSwitchDeleteIconDisabledByRow().ShouldBeFalse("Is Switch Icon disabled?");
            _appealAction.ClickOnSwitchFlagByFlag(flag);
            _appealAction.WaitForStaticTime(1000);
            _appealAction.IsPageErrorPopupModalPresent().ShouldBeTrue("Is Popup Modal display?");

            _appealAction.ClickOkCancelOnConfirmationModal(true);
            _appealAction.ClickOnAdjustIconByFlagAndClaimSequence(claseq2, flag);
            _appealAction.IsSwitchIconPresentByFlag(flag).ShouldBeTrue("Flag should switched");
            _appealAction.GetDeletedFlagLineCount().ShouldBeEqual(1, "Deleted Flag should be Equal");
            _appealAction.ClickOnSearchIconToNavigateAppealSearchPage();

        }


        private void CheckForClaimAction(string cotivitiUn,ClaimActionPage _claimAction)
        {
            _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
            _claimAction.GetClaimStatus().ShouldBeEqual(cotivitiUn, "Claim status of CV Coder claims should equal to " + cotivitiUn);
            var claseq = _claimAction.GetClaimSequence();
            _claimAction.IsCoderReviewByClaimSequenceTrue(claseq)
                .ShouldBeTrue("Is Coder_Review set to True for displayed claim?");
            _claimAction.IsFlagPciProduct(claseq)
                .ShouldBeTrue("Is atleast one flag a CV Product?");
            if (!_claimAction.IsElementPresent(ClaimActionPageObjects.NoDataAvailableXPath, How.XPath))
            {
                _claimAction.ClickOnNextButton();
            }
        }

        void SearchByClaimSeqFromWorkList(string claimSeq,ClaimActionPage _claimAction, bool handlePopup = true)
        {
            if (_claimAction.IsPageHeaderPresent())
            {
                if (!_claimAction.IsWorkListControlDisplayed())
                    _claimAction.ClickWorkListIcon();
            }
            _claimAction.ClickSearchIcon()
            .SearchByClaimSequence(claimSeq);
            if (handlePopup)
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
            _claimAction.WaitForCondition(() => !_claimAction.IsWorkListControlDisplayed());
        }

        void HandleAutomaticallyOpenedPatientClaimHistoryPopupFor(object obj,ClaimActionPage _claimAction)
        {
            if (obj is ClaimActionPage)
            {
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
            }
        }

        void DeleteOrRestoreLineFlag(string reasonCode,ClaimActionPage _claimAction)
        {
            try
            {
                bool action = _claimAction.IsFlagDeletable();
                string workItem = !action ? "Delete" : "Restore";

                int expectedDeletedFlags = _claimAction.GetCountOfDeletedFlags();
                int expectedRestoredFlags = _claimAction.GetCountOfRestoreFlags();

                StringFormatter.PrintMessageTitle(workItem + " Single Flag");
                string flag = _claimAction.ClickOnFirstEditFlag(action); 
                _claimAction.ClickOnWorkButton(action);
                _claimAction.SelectReasonCode(reasonCode);
                _claimAction.ClickOnSaveButton();

                if (action)
                {

                    (_claimAction.GetCountOfRestoreFlags() - expectedRestoredFlags).ShouldBeEqual(1, "Count of Restored Flags");
                }
                else
                {
                    (_claimAction.GetCountOfDeletedFlags() - expectedDeletedFlags).ShouldBeEqual(1,
                                                                                                  "Count of Deleted Flags");
                }

                StringFormatter.PrintMessage(workItem + " the single flag : " + flag + ".");
                StringFormatter.PrintLineBreak();
            }
            finally
            {
                bool isErrorModalPresent = _claimAction.IsPageErrorPopupModalPresent();
                if (isErrorModalPresent)
                {
                    _claimAction.ClosePageError();

                    this.AssertFail("Error page popup is present");
                }

            }
        }

        private void DeleteOrRestoreMprEditThenPsEditShouldBeDeletedOrRestored(string triggLineNum,ClaimActionPage _claimAction)
        {
            bool action = _claimAction.IsMprEditDeleted();
            string workItem = action ? "Restores" : "Deletes";

            StringFormatter.PrintMessageTitle("When the user " + workItem + " the MPR Edit ");
            _claimAction.ClickMprEditDelOrRestoreAction();

            if (action)
            {
                (!_claimAction.IsPsEditDeleted(triggLineNum)).ShouldBeTrue(" PS Edit Should be Restored");
            }
            else
            {
                (_claimAction.IsPsEditDeleted(triggLineNum)).ShouldBeTrue("PS Edit Should be Deleted");
            }
            StringFormatter.PrintLineBreak();
        }

        void CheckAlertMessageThenDismiss(ClaimActionPage _claimAction)
        {
            _claimAction.IsAlertBoxPresent().ShouldBeTrue("Pop Up should be displayed notifying confirmation for leaving.");
            _claimAction.DismissAlertBoxIfPresent();
        }

        void CheckApplicationAlertMessageThenDismiss(ClaimActionPage _claimAction)
        {
            _claimAction.GetPageErrorMessage()
                .ShouldBeEqual("Claims that have been modified must be APPROVED prior to leaving the page.",
                    "Correct Warning Message Displayed?");
            _claimAction.ClosePageError();
        }


        void ReloadBrowserIfPageErrorPopupAppearsWhenTransferClaim(ClaimActionPage _claimAction,string statusCode = "Documentation Requested")
        {
            if (_claimAction.IsPageErrorPopupModalPresent())
            {
                _claimAction.Refresh();
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                _claimAction.ClickOnTransferButton();
                _claimAction.SelectStatusCode(statusCode);
                _claimAction.ClickOnSaveButton();
            }
        }

        private void ValidateCharacterLimitOnNotes(string notes,ClaimActionPage _claimAction)
        {
            _claimAction.SetLengthyNoteToVisbleTextarea("note",notes);
            _claimAction.GetNoteOfVisibleTextarea().Length.ShouldBeEqual(1993,
                "Character limit should be 1993 or less, where 7 characters are separated for <p></p> tag.");
            _claimAction.ClickOnCancelLink();
        }

        #endregion
    }
}
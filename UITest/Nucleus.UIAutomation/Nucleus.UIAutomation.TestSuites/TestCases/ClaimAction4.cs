using System;
using System.Collections.Generic;
using System.Linq;
using Nucleus.Service.Data;
using NUnit.Framework;
using Nucleus.Service.PageServices.Claim;
using Nucleus.Service.Support.Common.Constants;
using Nucleus.Service.Support.Environment;
using Nucleus.UIAutomation.TestSuites.Base;
using System.Diagnostics;
using System.Security.Claims;
using Nucleus.Service.PageServices.Appeal;
using Nucleus.Service.PageServices.Base.Default;
using Nucleus.Service.PageServices.Settings;
using Nucleus.Service.PageServices.Settings.User;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Utils;
using Nucleus.Service.Support.Common;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Menu;
using System.Text.RegularExpressions;

namespace Nucleus.UIAutomation.TestSuites.TestCases
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class ClaimAction4 
    {
    //    #region PRIVATE FIELDS

        
    //    private ClaimActionPage _claimAction;
    //    private NewPopupCodePage _newpopupCodePage;
    //    private ProfileManagerPage _profileManager;
    //    AppealActionPage _appealAction = null;
    //    AppealManagerPage _appealManager = null;
    //    AppealSearchPage _appealSearch = null;
    //    private string _claimSequence = string.Empty;
        
    //    #endregion

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
        //        UserLoginIndex = 4;
        //        base.ClassInit();
        //        //QuickLaunch.Logout().LoginAsHciAdminUser4();
        //        _claimSequence = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, "ClassInit", "ClaimSequence", "Value");
        //        _claimAction = QuickLaunch.NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(_claimSequence,false);
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

        //    base.TestCleanUp();
        //    if (string.Compare(UserType.CurrentUserType, UserType.HCIADMIN4, StringComparison.OrdinalIgnoreCase) != 0)
        //    {
        //        _claimAction = _claimAction.Logout()
        //            .LoginAsHciAdminUser4().ClickOnSwitchClient().SwitchClientTo(automatedBase.EnvironmentManager.TestClient).NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(_claimSequence,false);
        //        HandleAutomaticallyOpenedPatientClaimHistoryPopupFor(_claimAction);
        //    }
        //    else if (!CurrentPage.IsCurrentClientAsExpected(automatedBase.EnvironmentManager.TestClient))
        //        {
        //            CheckTestClientAndSwitch();
        //            _claimAction = QuickLaunch.NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(_claimSequence);
        //    }
        //    else if (!CurrentPage.GetPageHeader().Equals(PageHeaderEnum.ClaimAction.GetStringValue()))
        //    {
        //        CurrentPage = _claimAction = CurrentPage.NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(_claimSequence);
        //        HandleAutomaticallyOpenedPatientClaimHistoryPopupFor(_claimAction);
        //    }
        //    else if (_claimAction.IsWorkingAjaxMessagePresent())
        //    {
        //        _claimAction.Refresh();
        //        _claimAction.WaitForWorkingAjaxMessage();
        //        _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();

        //    }

        //}

        //protected override void ClassCleanUp()
        //{
        //    try
        //    {
        //        _claimAction.CloseDatabaseConnection();

        //    }

        //    finally
        //    {
        //        base.ClassCleanUp();
        //    }
        //}

        //#endregion

        #region TEST SUITES

        [Test, Category("Acceptance")] //CV-8765
        [Author("Shyam Bhattarai")]
        public void Verify_Internal_Reviewer_Can_Add_Flag_When_Adj_Paid_Is_Zero()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(4))
            {
                ClaimActionPage _claimAction;
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;

                var paramList = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);

                var claseq = paramList["claseq"];
                var cobFlagsToBeAdded = paramList["cobFlagsToBeAdded"].Split(';').ToList();

                StringFormatter.PrintMessage("Reverting the claim status to Cotiviti Unreviewed");
                automatedBase.CurrentPage.GetCommonSql.UpdateClaimStatusFromDB("U", claseq);

                _claimAction = automatedBase.CurrentPage.NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(claseq, true);

                var cobFlags = _claimAction.GetCommonSql.GetFlagListBasedOnProductDb("C");
                var allFlags =
                    _claimAction.GetCommonSql.GetAllFlagsForAllActiveProductsByClient(ClientEnum.SMTST.ToString());

                try
                {
                    StringFormatter.PrintMessage("Verifying the initial data before starting the test");

                    var totalClaimLines = _claimAction.GetCountOfClaimLines();

                    totalClaimLines.ShouldBeEqual(3, "The claim should have a total of three Claim Lines");

                    double.Parse(_claimAction.GetClaimLineDetailsValueByLineNo(1, 2, 6).TrimStart('$')).ShouldBeEqual(0, "Claim Line 1 should have Adj Paid as zero");

                    (double.Parse(_claimAction.GetClaimLineDetailsValueByLineNo(3, 2, 6).TrimStart('$')) > 0).ShouldBeTrue("Claim Line 3 should not have Adj Paid as zero");

                    StringFormatter.PrintMessageTitle("Verifying multiple cases for adding a flag");

                    StringFormatter.PrintMessage("Verifying when selected claim line has Adj Paid = 0");
                    _claimAction.ClickAddIconButton();
                    _claimAction.SelectClaimLineToAddFlag(1.ToString());
                    _claimAction.WaitForCondition(() => _claimAction.GetActiveClaimLineCount() == 1);
                    _claimAction.WaitForStaticTime(5000);
                    _claimAction.GetActiveClaimLineCount().ShouldBeEqual(1,
                            "Analyst should be able to select the claim line with Adj Paid = 0");

                    VerifyFlagListAndSavingTheFlag(cobFlagsToBeAdded[0], true, 1);

                    StringFormatter.PrintMessage("Verifying for the claim line where Adj paid is not equals to 0");
                    _claimAction.ClickAddIconButton();
                    _claimAction.SelectClaimLineToAddFlag(1.ToString());
                    _claimAction.WaitForCondition(() => _claimAction.GetActiveClaimLineCount() == 1);
                    _claimAction.WaitForStaticTime(5000);
                    _claimAction.GetSideWindow.GetDropDownList("Flag").ShouldCollectionBeEqual(allFlags,
                        "All flags should be available when Adj Paid is not equals to 0");

                    StringFormatter.PrintMessage("Verifying when at least one selected claim lines have Adj Paid = 0");
                    _claimAction.ClickAddIconButton();
                    _claimAction.ClickSelectAllLinesIcon();

                    _claimAction.WaitForCondition(()=> _claimAction.GetActiveClaimLineCount() == 3);
                    _claimAction.WaitForStaticTime(3000);

                    VerifyFlagListAndSavingTheFlag(cobFlagsToBeAdded[1], false, 3);

                    #region LOCAL METHOD

                    void VerifyFlagListAndSavingTheFlag(string flagToBeAdded, bool adjPaidIsZero, int linno)
                    {
                        _claimAction.GetFlagListFromFlagDropdownInAddFlagForm().ShouldCollectionBeEqual(cobFlags,
                                "Analyst should only be allowed to add COB flags when a claim line with Ajd Paid = 0 is selected");

                            _claimAction.GetSideWindow.SelectDropDownListValueByLabel("Flag", flagToBeAdded);
                            _claimAction.GetSideWindow.Save(waitForWorkingMessage: true);

                            var addedFlag = _claimAction.GetFlagListForClaimLine(linno);
                            (addedFlag.Count == 1 && addedFlag[0] == flagToBeAdded).ShouldBeTrue("The flag should be added correctly");
                    }
                    #endregion
                }
                finally
                {
                    StringFormatter.PrintMessageTitle(
                        "Finally Block : Deleting the added flags and claim audit from the DB");
                    foreach (var flag in cobFlagsToBeAdded)
                    {
                        _claimAction.DeleteFlagLineFromDatabaseByClaimSeqAndFlag(claseq, flag);
                    }
                }
            }
        }

        [Test, Category("NewClaimAction4")] //US65698
        public void Verify_appeal_icon_for_deleted_appeal_in_patient_claim_history_page()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(4))
            {
                ClaimActionPage _claimAction;
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var claimSequenceWithAppeal = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "ClaimSequenceWithAppeal", "Value");
                var claimSequenceWithDeletedAppeal = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "ClaimSequenceWithDeletedAppeal", "Value");
                
                _claimAction = automatedBase.CurrentPage.NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(claimSequenceWithAppeal, true);
                
                ClaimHistoryPage patientHistory = null;
                try
                {
                    patientHistory = _claimAction.ClickOnHistoryButtonAndSwitchToPatientClaimHistoryPage();
                    patientHistory.IsAppealIconPresent(claimSequenceWithAppeal)
                        .ShouldBeTrue("Appeal Icon is present for open status");
                    patientHistory.SwitchToNewClaimActionPage(true);
                    SearchByClaimSeqFromWorkList(claimSequenceWithDeletedAppeal);
                    patientHistory = _claimAction.ClickOnHistoryButtonAndSwitchToPatientClaimHistoryPage();
                    _claimAction.GetDeletedAppealCountByClaseq(claimSequenceWithDeletedAppeal)
                        .ShouldBeEqual(1, "Previous Deleted Appeal Count");
                    patientHistory.IsAppealIconPresent(claimSequenceWithDeletedAppeal)
                        .ShouldBeFalse("Appeal Icon should not present for previous deleted appeal");
                }
                finally
                {
                    if (patientHistory != null)
                        patientHistory.SwitchToNewClaimActionPage(true);
                    _claimAction.ClickClaimSearchIcon();
                }

                void SearchByClaimSeqFromWorkList(string claimSeq)
                {
                    if (_claimAction.IsPageHeaderPresent())
                    {
                        if (!_claimAction.IsWorkListControlDisplayed())
                            _claimAction.ClickWorkListIcon();
                    }
                    _claimAction.ClickSearchIcon()
                        .SearchByClaimSequence(claimSeq);
                    _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                    //_claimAction.ClickWorkListIcon();
                    _claimAction.WaitForCondition(() => !_claimAction.IsWorkListControlDisplayed(), 3000);
                }
            }
        }

        [Test, Category("NewClaimAction4")] //US58227
        public void Verify_claim_action_poup_should_display_when_click_on_claim_sequence_on_patient_claim_history_page()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(4))
            {

                ClaimActionPage _claimAction;
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var claimSequence = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                    "ClaimSequence", "Value");
                var claseqOnClaimHistory = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "ClaseqOnClaimHistory", "Value");
                var headerList = automatedBase.DataHelper
                    .GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName, "HeaderList", "Value")
                    .Split(';').ToList();
                _claimAction = automatedBase.CurrentPage.NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(claimSequence, true);
                //SearchByClaimSeqFromWorkList(claimSequence);
                ClaimHistoryPage patientHistory = null;
                try
                {
                    patientHistory = _claimAction.ClickOnHistoryButtonAndSwitchToPatientClaimHistoryPage();
                    var claimActionPopup = patientHistory.ClickOnClaimSequence(claseqOnClaimHistory);
                    claimActionPopup.GetPageHeader().ShouldBeEqual(PageHeaderEnum.ClaimAction.GetStringValue());
                    claimActionPopup.GetHeaderList()
                        .ShouldCollectionBeEqual(headerList, "Header in Claim Action present");
                }
                finally
                {
                    if (patientHistory != null)
                        patientHistory.SwitchToNewClaimActionPage(true);
                    _claimAction.CloseAnyPopupIfExist();
                    _claimAction.ClickClaimSearchIcon();
                }
            }
        }

        [Test, Category("NewClaimAction4")] //US58266 + TANT-82 (Verification of 'Next' icon)
        public void Verify_that_Patient_Claim_Hx_Popup_should_replaced_previously_opened_Patient_Claim_Hx_popup()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(4))
            {
                ClaimActionPage _claimAction;
                _claimAction = automatedBase.CurrentPage.NavigateToCVClaimsWorkList(); //reset view
                try
                {
                    _claimAction.GetWindowHandlesCount()
                        .ShouldBeEqual(2, "Patient Claim Hx display before going to next claim");
                    var patientClaimHx = _claimAction.SwitchToPatientClaimHistory();
                    var preClaimHxUrl = patientClaimHx.CurrentPageUrl;
                    _claimAction.SwitchBackToNewClaimActionPage();
                    _claimAction = _claimAction.ClickOnNextButton();
                    _claimAction.GetWindowHandlesCount()
                        .ShouldBeEqual(2, "Patient Claim Hx Popup should replaced by next patient claim Hx");
                    patientClaimHx = _claimAction.SwitchToPatientClaimHistory();
                    patientClaimHx.CurrentPageUrl.ShouldNotBeEqual(preClaimHxUrl,
                        "Patient Claim Hx Popup should replaced by next patient claim Hx by verifying url");
                    patientClaimHx.SwitchToNewClaimActionPage(true);
                }
                finally
                {
                    _claimAction.CloseAllExistingPopupIfExist();
                }
            }
        }


        [Test, Category("NewClaimAction4")]//US50652
        [NonParallelizable]
        public void verify_claim_should_unlock_either_by_approving_or_leaves_that_claim()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(4))
            {
                ClaimActionPage _claimAction;

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var claimSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                    "ClaimSequence", "Value");
                _claimAction = automatedBase.CurrentPage.NavigateToClaimSearch()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(claimSeq, true);
                try
                {
                    //SearchByClaimSeqFromWorkList(claimSeq);
                    StringFormatter.PrintMessageTitle(
                        "Verification of Additional Locked Claims Lists when claim is approved");
                    if (_claimAction.IsLocked())
                        _claimAction.RemoveLock();
                    if (_claimAction.IsApproveButtonDisabled())
                    {
                        _claimAction.ClickOnTransferButton();
                        _claimAction.SelectStatusCode("Documentation Requested");
                        _claimAction.ClickOnSaveButton();
                        ReloadBrowserIfPageErrorPopupAppearsWhenTransferClaim();
                        _claimAction.ClickOnReturnToClaim(claimSeq);
                        _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                        _claimAction.ClickWorkListIcon();

                    }

                    _claimAction.ClickOnApproveButton();
                    _claimAction.IsClaimLockedByClaimSequence(ClientEnum.SMTST.ToString(), claimSeq.Split('-')[0],
                            claimSeq.Split('-')[1],
                            automatedBase.EnvironmentManager.HciAdminUsername4)
                        .ShouldBeFalse($"Is claim {claimSeq} locked?");
                    _claimAction.WaitForCondition(_claimAction.IsBlankPagePresent);
                    _claimAction.IsEmptyMessageOnAdditionalLockedClaimsPresent()
                        .ShouldBeTrue("No Data Available message should present in Additional Locked Claims", true);
                    _claimAction.ClickOnReturnToClaim(claimSeq);
                    _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();



                    StringFormatter.PrintMessageTitle(
                        "Verification of Additional Locked Claims Lists when user leaves claim");
                    _claimAction.ClickClaimSearchIcon();
                    _claimAction.WaitForCondition(_claimAction.IsBlankPagePresent);
                    _claimAction.IsEmptyMessageOnAdditionalLockedClaimsPresent()
                        .ShouldBeTrue("No Data Available message should present in Additional Locked Claims");
                    _claimAction.ClickOnReturnToClaim(claimSeq);
                    _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                    _claimAction.ClickWorkListIcon();

                }
                finally
                {
                    if (_claimAction.IsBlankPagePresent())
                    {
                        _claimAction.ClickOnReturnToClaim(claimSeq);
                        _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                        _claimAction.ClickWorkListIcon();
                    }

                    if (_claimAction.IsApproveButtonDisabled())
                    {
                        StringFormatter.PrintMessageTitle("Restored to previous status");
                        _claimAction.ClickOnTransferButton();
                        _claimAction.SelectStatusCode("Documentation Requested");
                        _claimAction.ClickOnSaveButton();
                        _claimAction.ClickOnReturnToClaim(claimSeq);
                        _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                        _claimAction.ClickWorkListIcon();

                    }

                    if (_claimAction.IsWorkListControlDisplayed())
                        _claimAction.ClickWorkListIcon();
                }

                void ReloadBrowserIfPageErrorPopupAppearsWhenTransferClaim(string statusCode = "Documentation Requested")
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
            }
        }


        [Test, Category("SmokeTest"), Category("SmokeTestDeployment")] //us39866 +  TANT-82
        public void Verify_that_claim_status_changed_when_claim_is_approved()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(4))
            {

                ClaimActionPage _claimAction;
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var claimSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                    "ClaimSequence", "Value");
                var claimSeq1 = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                    "ClaimSequence1", "Value");
                _claimAction = automatedBase.CurrentPage.NavigateToClaimSearch()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(claimSeq, true);
                //SearchByClaimSeqFromWorkList(claimSeq);

                try
                {
                    if (_claimAction.IsRestoreButtonPresent())
                        _claimAction.ClickOnRestoreAllFlagsIcon();

                    _claimAction.ClickOnDeleteAllFlagsIcon();
                    if (_claimAction.IsPageErrorPopupModalPresent())
                    {
                        _claimAction.ClosePageError();
                        SearchByClaimSeqFromWorkList(claimSeq1);
                        if (_claimAction.IsRestoreButtonPresent())
                            _claimAction.ClickOnRestoreAllFlagsIcon();

                        _claimAction.ClickOnDeleteAllFlagsIcon();
                    }

                    _claimAction.ClickOnApproveButton();
                    _claimAction.ClickOnBrowserBack();
                    _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                    _claimAction.IsApproveButtonDisabled().ShouldBeTrue("Approve Icon disabled?");
                    _claimAction.IsNextButtonDisabled().ShouldBeTrue("Next Button disabled?");
                    _claimAction.GetClaimStatus()
                        .ShouldBeEqual(ClaimStatusTypeEnum.CotivitiReviewed.GetStringValue(),
                            "Claim Status should be Cotiviti Reviewed after approve");
                    _claimAction.ClickOnRestoreAllFlagsIcon();
                    //_claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                    _claimAction.IsApproveButtonEnabled().ShouldBeTrue("Approve Button Enabled after resotre flag");
                    _claimAction.GetClaimStatus()
                        .ShouldBeEqual(ClaimStatusTypeEnum.CotivitiUnreviewed.GetStringValue(),
                            "Claim Status should be Cotiviti Reviewed after approve");

                }
                finally
                {

                    if (_claimAction.IsBlankPagePresent())
                        _claimAction.ClickOnBrowserBack();

                    var restoreButton = _claimAction.IsRestoreButtonPresent();
                    var approveButton = _claimAction.IsApproveButtonDisabled();
                    if (restoreButton)
                    {
                        _claimAction.ClickOnRestoreAllFlagsIcon();

                        if (approveButton)
                            _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                    }

                    if (_claimAction.IsWorkListControlDisplayed())
                        _claimAction.ClickWorkListIcon();
                }

                void SearchByClaimSeqFromWorkList(string claimSequence)
                {
                    if (_claimAction.IsPageHeaderPresent())
                    {
                        if (!_claimAction.IsWorkListControlDisplayed())
                            _claimAction.ClickWorkListIcon();
                    }
                    _claimAction.ClickSearchIcon()
                        .SearchByClaimSequence(claimSequence);
                    _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                    //_claimAction.ClickWorkListIcon();
                    _claimAction.WaitForCondition(() => !_claimAction.IsWorkListControlDisplayed(), 3000);
                }
            }
        }

        [Test, Category("NewClaimAction4"), Category("AppealDependent")] //US45712
        public void
            Verify_create_appeal_enabled_only_when_Status_is_Client_Reviewed_or_Unreviewed_with_tool_tip_message_for_disabled_create_icon()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(4))
            {
                ClaimActionPage _claimAction;
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var clientReviewedClaimSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "ClientReviewedClaimSeq", "Value");
                var clientUnreviewedClaimSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "ClientUnreviewedClaimSeq", "Value");
                var claimSeqList = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName, "ClaimSequenceList",
                            "Value")
                        .Split(',')
                        .ToList();
                var lockedClaimSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                    "LockedClaimSeq", "Value");
                var closedAppealClaimSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "ClosedAppealClaimSeq", "Value");
                var completeAppealClaimSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "CompleteAppealClaimSeq", "Value");

                _claimAction = automatedBase.CurrentPage.NavigateToClaimSearch()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(clientReviewedClaimSeq, true);


                //SearchByClaimSeqFromWorkList(_claimAction, clientReviewedClaimSeq);
                _claimAction.IsAddAppealIconEnabled()
                    .ShouldBeTrue("Create Appeal Icon Should be enabled for claim status having client Reviewed");
                SearchByClaimSeqFromWorkList(clientUnreviewedClaimSeq);
                _claimAction.IsAddAppealIconEnabled()
                    .ShouldBeTrue("Create Appeal Icon Should be enabled for claim status having client Unreviewed");
                SearchByClaimSeqFromWorkList(closedAppealClaimSeq);
                _claimAction.IsAddAppealIconEnabled()
                    .ShouldBeTrue(
                        "Create Appeal Icon Should be enabled for claim status having client Reviewed and existing appeal is closed");
                SearchByClaimSeqFromWorkList(completeAppealClaimSeq);
                _claimAction.IsAddAppealIconEnabled()
                    .ShouldBeTrue(
                        "Create Appeal Icon Should be enabled for claim status having client Reviewed and existing appeal is complete");


                // Commented out since clients with DCA enabled can now add appeals claims with any status
                /*
                StringFormatter.PrintMessageTitle("Verification of tooltip of disabled create appeal icon");
                Random rand = new Random();
                for (var i = 0; i < 3; i++)
                {
                    SearchByClaimSeqFromWorkList(claimSeqList[rand.Next(0, 10)]);
                    var status = _claimAction.GetClaimStatus();
                    _claimAction.IsAddAppealIconDisabled()
                        .ShouldBeTrue(string.Format("Create Appeal Should disabled for claims having status<{0}> that has not been released to the client", status));
                    _claimAction.GetToolTipMessageDisabledCreateAppealIcon()
                        .ShouldBeEqual(
                            "Appeals cannot be added to a claim  that has not been released to the client.",
                            "Tooltip Message for Unreviwed Claims");
    
                }
                */

                SearchByClaimSeqFromWorkList(lockedClaimSeq);
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                _claimAction.IsAddAppealIconDisabled()
                    .ShouldBeTrue(
                        "Create Appeal Should disabled for claims that has appeal whose status is not complete or closed");
                _claimAction.GetToolTipMessageDisabledCreateAppealIcon()
                    .ShouldBeEqual(
                        "Appeals cannot be opened for claims that have an existing appeal in process.",
                        "Tooltip Message for claim having appeal");

                #region Private Method
                void SearchByClaimSeqFromWorkList(string claimSeq)
                {
                    if (_claimAction.IsPageHeaderPresent())
                    {
                        if (!_claimAction.IsWorkListControlDisplayed())
                            _claimAction.ClickWorkListIcon();
                    }
                    _claimAction.ClickSearchIcon()
                        .SearchByClaimSequence(claimSeq);
                    _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                    //_claimAction.ClickWorkListIcon();
                    _claimAction.WaitForCondition(() => !_claimAction.IsWorkListControlDisplayed(), 3000);
                }
                #endregion
            }
        }

        [Test, Category("NewClaimAction4")]
        public void Verify_that_no_work_list_exists_then_clicking_Approve_Transfer_TransferApprove_directs_user_to_blank_page_with_work_list_panel_open()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(4))
            {

                ClaimActionPage _claimAction;
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                string claimSeq = paramLists["ClaimSequence"];
                var _claimSequence = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, "ClassInit", "ClaimSequence", "Value");

                _claimAction = automatedBase.CurrentPage.NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(claimSeq, true);

                _claimAction.ClickWorkListIcon().ClickWorkListIconWithinWorkList();
                _claimAction.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Batch ID", paramLists["BatchId"]);
                _claimAction.ClickOnCreateButton();
                if (_claimAction.WindowHandles.Count > 1)
                    HandleAutomaticallyOpenedPatientClaimHistoryPopupFor(_claimAction, _claimAction);
                _claimAction.ClickSearchIcon().SearchByClaimSequence(claimSeq);
                HandleAutomaticallyOpenedPatientClaimHistoryPopupFor(_claimAction, _claimAction);
                _claimAction.ClickWorkListIcon();
                try
                {
                    if (!_claimAction.IsApproveButtonDisabled())
                    {
                        _claimAction.ClickOnApproveButton();
                        _claimAction.IsBlankPagePresent().ShouldBeTrue("New Claim Action page is blank.", true);
                        _claimAction.IsWorkListControlDisplayed().ShouldBeTrue("WorkList panel is displayed.", true);
                        _claimAction.ClickOnReturnToClaim(claimSeq);
                        HandleAutomaticallyOpenedPatientClaimHistoryPopupFor(_claimAction, _claimAction);
                        _claimAction.ClickWorkListIcon();
                    }

                    _claimAction.ClickOnTransferButton();
                    _claimAction.SelectStatusCode("Documentation Requested");
                    _claimAction.ClickOnSaveButton();
                    ReloadBrowserIfPageErrorPopupAppearsWhenTransferClaim(_claimAction);
                    _claimAction.IsBlankPagePresent().ShouldBeTrue("New Claim Action page is blank.", true);
                    _claimAction.IsWorkListControlDisplayed().ShouldBeTrue("WorkList panel is displayed.", true);
                    _claimAction.ClickOnReturnToClaim(claimSeq);
                    HandleAutomaticallyOpenedPatientClaimHistoryPopupFor(_claimAction, _claimAction);
                    _claimAction.ClickWorkListIcon();
                    if (!_claimAction.IsApproveButtonDisabled())
                    {
                        _claimAction.ClickOnTransferApproveButton();
                        _claimAction.SelectStatusCode("Documentation Requested");
                        _claimAction.ClickOnSaveButton();
                        ReloadBrowserIfPageErrorPopupAppearsWhenTransferClaim(_claimAction);
                        _claimAction.IsBlankPagePresent().ShouldBeTrue("New Claim Action page is blank.", true);
                        _claimAction.IsWorkListControlDisplayed().ShouldBeTrue("WorkList panel is displayed.", true);
                    }
                }

                finally
                {
                    bool isblank = _claimAction.IsBlankPagePresent();
                    bool isWorkListPresent = _claimAction.IsWorkListControlDisplayed();
                    if (isblank && !isWorkListPresent)
                    {
                        _claimAction = _claimAction.NavigateToClaimSearch()
                            .SearchByClaimSequenceToNavigateToClaimActionPage(_claimSequence);
                    }
                    else
                    {
                        if (!isWorkListPresent)
                            _claimAction.ClickWorkListIcon();
                        _claimAction.ClickSearchIcon().SearchByClaimSequence(_claimSequence)
                            .HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                        _claimAction.ClickWorkListIcon();
                    }
                }
            }
        }

        [Test, Category("NewClaimAction4")] //US31621
        [NonParallelizable]
        // [Retrying (Times = 2)]
        public void Verify_that_when_user_clicks_on_delete_or_restore_icons_Approve_icon_is_disabled_as_Working_message_is_shown()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(4))
            {
                ClaimActionPage _claimAction;
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestExtensions.TestName);
                string claimSeq = paramLists["ClaimSequence"];
                _claimAction = automatedBase.CurrentPage.NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(claimSeq, true);

                try
                {


                    //If Restore All Flag icon is present, click on it to change to get Delete All.
                    if (_claimAction.IsRestoreButtonPresent())
                        _claimAction.ClickOnRestoreAllFlagsIconNoWait();
                    //else
                    //    _claimAction.ClickOnDeleteAllFlagsIconNoWait();

                    _claimAction.ClickOnDeleteAllFlagsIconAndIsWorkingIconPresent()
                        .ShouldBeTrue("Working Message Present?");
                    _claimAction.Wait();

                    if (_claimAction.IsRestoreButtonPresent())
                        _claimAction.ClickOnRestoreAllFlagsIconNoWait();
                    //else
                    //    _claimAction.ClickOnDeleteAllFlagsIconNoWait();

                    _claimAction.ClickOnDeleteAllFlagsIconAndIsApprovedIconDisabled()
                        .ShouldBeTrue("Approve Icon is Disabled?");
                    _claimAction.Wait();
                }
                finally
                {
                    //Restore flags before exit
                    if (_claimAction.IsRestoreButtonPresent())
                        _claimAction.ClickOnRestoreAllFlagsIcon();
                }
            }
        }

        [Test, Category("NewClaimAction4")] //US53177
        public void Verify_user_allowed_to_enter_alphanumeric_character_without_popup_in_sug_code_in_add_flag_section()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(4))
            {
                ClaimActionPage _claimAction;
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var claimSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                    "ClaimSequence", "Value");
                _claimAction = automatedBase.CurrentPage.NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(claimSeq, true);
                try
                {
                    _claimAction.ClickAddIconButton();
                    _claimAction.SelectClaimLineToAddFlag();
                    _claimAction.SelectAddInFlagAdd("REB");
                    _claimAction.EnterSugCode("Test1");
                    _claimAction.IsPageErrorPopupModalPresent()
                        .ShouldBeTrue("Page error popup present for Invalid Sug Code?");
                    _claimAction.ClosePageError();
                    _claimAction.EnterSugCode("D9211");
                    _claimAction.IsPageErrorPopupModalPresent()
                        .ShouldBeFalse("Page error popup present for Valid Sug Code?");
                }
                finally
                {
                    _claimAction.ClickOnFlagLevelCancelLink();
                }
            }
        }

        //[Test, Category("NewClaimAction4"),Category("Acceptance")] //US50647 
        public void
            Verify_that_Original_claim_data_page_pops_up_with_presence_of_HCILINSEQ_column_when_click_on_original_claim_data()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(4))
            {
                ClaimActionPage _claimAction;
                var _claimSequence = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, "ClassInit", "ClaimSequence", "Value");
                _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(_claimSequence, true);
                
                try
                {
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

        [Test, Category("NewClaimAction4")] //US53174
        public void
            Verify_sug_code_on_flag_detail_is_clickable_and__popup_page_display_corect_information_for_CPT_and_HCPCS_code()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(4))
            {
                ClaimActionPage _claimAction;
                NewPopupCodePage _newpopupCodePage;
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var claimSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                    "ClaimSequence", "Value");
                var CPTData = automatedBase.DataHelper
                    .GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName, "CPTData", "Value").Split(';');
                var HCPCSData = automatedBase.DataHelper
                    .GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName, "HCPCSData", "Value")
                    .Split(';');

                _claimAction = automatedBase.CurrentPage.NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(claimSeq, true);
                try
                {
                    _claimAction.ClickOnFlagLineByFlag(HCPCSData[0]);
                    _newpopupCodePage = _claimAction.ClickOnSugCodeLinkOnFlagDetails(HCPCSData[2], HCPCSData[1]);
                    _newpopupCodePage.GetPopupHeaderText().ShouldBeEqual(HCPCSData[2], "Popup Header Text");
                    _newpopupCodePage.GetTextValueinLiTag(1)
                        .ShouldBeEqual(string.Concat("Code: ", HCPCSData[1]), "Code");
                    _newpopupCodePage.GetTextValueinLiTag(2)
                        .ShouldBeEqual(string.Concat("Type: ", HCPCSData[2]), "Type");
                    _newpopupCodePage.GetTextValueinLiTag(3)
                        .ShouldBeEqual(string.Concat("Description\r\n", HCPCSData[3]), "Description");
                    _claimAction = _newpopupCodePage.ClosePopupOnNewClaimActionPage(HCPCSData[1]);

                    _claimAction.ClickOnFlagLineByFlag(CPTData[0]);
                    _newpopupCodePage = _claimAction.ClickOnSugCodeLinkOnFlagDetails(CPTData[2], CPTData[1]);
                    _newpopupCodePage.GetPopupHeaderText().ShouldBeEqual(CPTData[2], "Popup Header Text");
                    _newpopupCodePage.GetTextValueinLiTag(1).ShouldBeEqual(string.Concat("Code: ", CPTData[1]), "Code");
                    _newpopupCodePage.GetTextValueinLiTag(2).ShouldBeEqual(string.Concat("Type: ", CPTData[2]), "Type");
                    _newpopupCodePage.GetTextValueinLiTag(3)
                        .ShouldBeEqual(string.Concat("Description\r\n", CPTData[3], ";", CPTData[4]), "Description");
                    _claimAction = _newpopupCodePage.ClosePopupOnNewClaimActionPage(CPTData[1]);

                }
                finally
                {
                    _claimAction.CloseAnyPopupIfExist();
                    _claimAction.NavigateToQaClaimSearch();
                }
            }
        }

        //[Test, Category("NewClaimAction4")]//US57581
        public void Verify_claim_flag_audit_history_header_detail_with_audit_detail()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(4))
            {
                ClaimActionPage _claimAction = null;
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var claimSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                    "ClaimSequence", "Value");
                var headerInfo = automatedBase.DataHelper
                    .GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName, "HeaderDetails", "Value")
                    .Split(';');
                var reasonCode = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                    "ReasonCode", "Value");
                var flagSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                    "FlagSequence", "Value");

                _claimAction.DeleteClaimAuditRecordFromDatabase(flagSeq, claimSeq, headerInfo[0]);

                _claimAction = automatedBase.CurrentPage.NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(claimSeq);
                _claimAction.GetClaimFlagAuditHistoryIconTooltip()
                    .ShouldBeEqual("Claim Flag Audit History", "Tooltip of Claim Flag Audit History Icon");
                _claimAction.ClickOnClaimFlagAuditHistoryIcon();


                _claimAction.IsClaimAuditHistoryDivPresentByLineNoAndFlag(headerInfo[1], headerInfo[0])
                    .ShouldBeFalse("Audit Record should not be present.");

                _claimAction.ClickOnEditIconFlagLevelForLineEdit(Convert.ToInt32(headerInfo[0]), headerInfo[1]);
                _claimAction.SelectReasonCode(reasonCode);
                _claimAction.SetNoteToVisbleTextarea("Test");
                _claimAction.SaveFlag(reasonCode);

                _claimAction.IsClaimAuditHistoryDivPresentByLineNoAndFlag(headerInfo[1], headerInfo[0])
                    .ShouldBeTrue("Audit Record should be updated automatically.");

                _claimAction.GetClaimFlagAuditHistorysDetailListByFlagAndLabel(headerInfo[2], "Mod Date")
                    .Select(DateTime.Parse)
                    .ToList()
                    .IsInDescendingOrder()
                    .ShouldBeTrue("Recent Date should be on top");

                _claimAction.GetFlagOnClaimFlagAuditHistoryByRow(3)
                    .ShouldBeEqual(headerInfo[1], "Flag should be displayed");
                _claimAction.GetLineNoOnClaimFlagAuditHistoryByRow(3)
                    .ShouldBeEqual(headerInfo[0], "Line No should be displayed");
                _claimAction.GetClaimFlagAuditHistoryHeaderDetailByFlagAndLabel(headerInfo[2], "Sug Code")
                    .ShouldBeEqual(headerInfo[3], "Sug Code should be displayed");
                _claimAction.GetClaimFlagAuditHistoryHeaderDetailByFlagAndLabel(headerInfo[1], "Cust")
                    .ShouldBeEqual(headerInfo[4], "Cust(i.e.,prepep) should be displayed");
                _claimAction.GetClaimFlagAuditHistoryHeaderDetailByFlagAndLabel(headerInfo[1], "Addl")
                    .ShouldBeEqual(headerInfo[5], "Addl(i.e., comment) should be displayed");



                Convert.ToDateTime(
                        _claimAction.GetClaimFlagAuditHistoryDetailByFlagAndRowAndLabelAndLineNo(headerInfo[1],
                            "Mod Date", headerInfo[0])).ToString("MM/dd/yyyy")
                    .IsDateInFormat()
                    .ShouldBeTrue("Is Mod Date in mm/dd/yyyy format");
                _claimAction
                    .GetClaimFlagAuditHistoryDetailByFlagAndRowAndLabelAndLineNo(headerInfo[1], "By", headerInfo[0])
                    .DoesNameContainsOnlyFirstWithLastname()
                    .ShouldBeTrue("Modified By Should be in <FirstName> <LastName>");
                _claimAction
                    .GetClaimFlagAuditHistoryDetailByFlagAndRowAndLabelAndLineNo(headerInfo[1], "Action", headerInfo[0])
                    .ShouldNotBeNull("Action Should not null");
                _claimAction
                    .GetClaimFlagAuditHistoryDetailByFlagAndRowAndLabelAndLineNo(headerInfo[1], "Type", headerInfo[0])
                    .ShouldNotBeNull("Action Should not null");
                _claimAction
                    .GetClaimFlagAuditHistoryDetailByFlagAndRowAndLabelAndLineNo(headerInfo[1], "Code", headerInfo[0])
                    .ShouldNotBeNull("Action Should not null");
                _claimAction
                    .GetClaimFlagAuditHistoryDetailByFlagAndRowAndLabelAndLineNo(headerInfo[1], "Client Display",
                        headerInfo[0])
                    .AssertIsContainedInList(new List<string> { "Yes", "No" }, "Action Should not null");
                _claimAction
                    .GetClaimFlagAuditHistoryDetailByFlagAndRowAndLabelAndLineNo(headerInfo[1], "Notes", headerInfo[0])
                    .Length.ShouldBeGreaterOrEqual(0, "Notes may present or can be null");
            }
        }

        //[Test, Category("Working")]
        //public void Verify_eyeball_icon_with_tooltip_display_for_provider_is_on_review_both_client_cotiviti()
        //{

        //    var prvSeqBothActionReview = "98680";
        //    var prvSeqCotivitiReviewOnly = "73472";

        //}

        [Test, Category("NewClaimAction4")] //US53176
        [NonParallelizable]
        public void Verify_claim_should_not_lock_when_flag_is_added_and_continue_to_modify_or_approve_the_claim()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(4))
            {
                ClaimActionPage _claimAction;

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var claimSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                    "ClaimSequence", "Value");
                var triggerClaSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                    "TriggerClaSeq", "Value");
                var reasonCode = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                    "ReasonCode", "Value");
                var flag = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName, "Flag",
                    "Value");
                var source = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName, "Source",
                    "Value");
                _claimAction = automatedBase.CurrentPage.NavigateToClaimSearch()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(claimSeq, true);
                
                try
                {
                    _claimAction.DeleteFlagLineFromDatabaseByClaimSeqAndFlag(claimSeq, flag);
                    _claimAction.ClickAddIconButton();
                    _claimAction.SelectClaimLineToAddFlag();
                    _claimAction.SelectAddInFlagAdd(flag);
                    _claimAction.WaitForWorkingAjaxMessage();
                    _claimAction.SelectFlagSourceInAddFlag(source);
                    _claimAction.SelectTriggerClaimLineOnAddFlag(triggerClaSeq);
                    _claimAction.SetNote("New Flag Added");
                    _claimAction.EnterSugUnits("1");
                    _claimAction.EnterSugPaid("1");
                    _claimAction.ClickCheckBox();
                    _claimAction.ClickOnSaveEditButton();
                    _claimAction.WaitForWorkingAjaxMessage();
                    _claimAction.IsFlagLinePresentByFlag(flag)
                        .ShouldBeTrue(string.Format("New Flag <{0}> should added", flag));
                    _claimAction.Sleep(15000);
                    _claimAction.IsClaimLocked().ShouldBeFalse("Claim Should not lock when flag is added upto 15 sec");

                    StringFormatter.PrintMessageTitle("User able to edit/approve claim after addition of new flag");
                    _claimAction.GetCommonSql.InsertLockForClaimByClaimSeqAndUserId(claimSeq, automatedBase.EnvironmentManager.Username);
                    _claimAction.RefreshPage(false);
                    _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                    _claimAction.ClickOnEditIconFlagLevelForLineEdit(1, flag);
                    _claimAction.EnterSugPaid("0");
                    _claimAction.SelectReasonCode(reasonCode);
                    _claimAction.SaveFlag(reasonCode);

                    _claimAction.ClickOnEditIconFlagLevelForLineEdit(1, flag);
                    _claimAction.GetSugPaid()
                        .ShouldBeEqual(Math.Round(0.00, 2, MidpointRounding.AwayFromZero),
                            "User able to modify flag after flag is added in flagged lines");
                    _claimAction.ClickOnCancelEditFlagLink();


                    _claimAction.ClickOnApproveButton();
                    _claimAction.WaitForBlankPage();
                    _claimAction.ClickOnReturnToClaim(claimSeq);
                    _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();

                }
                finally
                {
                    if (_claimAction.IsBlankPagePresent())
                    {
                        _claimAction.ClickOnBrowserBack();
                        _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                    }

                    if (_claimAction.IsWorkListControlDisplayed())
                        _claimAction.ClickWorkListIcon();

                }
            }
        }

        [Test, Category("NewClaimAction4")]//US67012
        public void Verify_that_the_add_document_icon_will_be_displayed_on_Claim_Action_Page_if_documents_are_not_present_for_Cotiviti_User()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(4))
            {
                ClaimActionPage _claimAction;
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var claimSequenceWithoutDocs = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "ClaimSequenceWithoutDocs", "Value");
                _claimAction = automatedBase.CurrentPage.NavigateToClaimSearch()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(claimSequenceWithoutDocs, true);

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

        [Test, Category("NewClaimAction4")]//US67012
        public void Verify_red_badge_over_the_documents_icon_represents_the_number_of_claim_documents_added_in_claim()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(4))
            {
                ClaimActionPage _claimAction;
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var claimSequenceWithDocs = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "ClaimSequenceWithDocs", "Value");
                _claimAction = automatedBase.CurrentPage.NavigateToClaimSearch()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(claimSequenceWithDocs, true);
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
                    .ShouldBeTrue("Most recent uploaded files are on top");
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

        [Test, Category("NewClaimAction4")]//US67012
        //[Retrying(Times = 2)]
        public void Verify_upload_new_claim_document_in_claim_action_for_Cotiviti_User()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(4))
            {
                ClaimActionPage _claimAction;

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> testData = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestExtensions.TestName);

                var claimSequence = testData["ClaimSequence"];
                var auditDate = testData["AuditDate"];
                var fileToUpload = testData["FileToUpload"];
                var fileType = testData["FileType"];
                var expectedSelectedFileTypeList = testData["SelectedFileList"].Split(',').ToList();
                var expectedSelectedFileType = testData["SelectedFileList"];
                var expectedFileTypeList =
                    automatedBase.DataHelper.GetMappingData(FullyQualifiedClassName, "File_Type_List").Values.ToList();
                var userName = testData["UserName"];
                var userType = testData["UserType"];
                var expectedDocumentHeaderText = testData["DocumentHeaderText"];

                automatedBase.CurrentPage.DeleteClaimDocumentRecord(claimSequence, auditDate);
                _claimAction = automatedBase.CurrentPage.NavigateToClaimSearch()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(claimSequence, true);

                var claimDocsCount = Convert.ToInt64(_claimAction.GetClaimDocsCountInBadge());
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
                //_claimAction.GetFileUploadPage.IsCancelButtonDisabled().ShouldBeFalse("Is Cancel Button disabled?");
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
                //_claimAction.GetFileUploadPage.AddFileForUpload(fileToUpload.Split(';'));


                /* --------- add single file only since multiple file upload with selenium is successfull locally only -----*/
                _claimAction.GetFileUploadPage.AddFileForUpload(fileToUpload.Split(';')[0]);
                _claimAction.GetFileUploadPage.IsAddFileButtonDisabled()
                    .ShouldBeFalse(
                        "Add file button should be enabled (when atleast a file has been uploaded) must be false?");
                _claimAction.GetFileUploadPage.ClickOnAddFileBtn();
                _claimAction.IsPageErrorPopupModalPresent()
                    .ShouldBeTrue("Page error pop up should be present if no file type is selected");
                _claimAction.GetPageErrorMessage()
                    .ShouldBeEqual("At least one Document Type selection is required before the files can be added.",
                        "Expected error message on zero file type selection");
                _claimAction.ClosePageError();
                _claimAction.GetFileUploadPage.GetAvailableFileTypeList()
                    .ShouldCollectionBeEqual(expectedFileTypeList, "File Type List Equal");
                _claimAction.GetFileUploadPage.SetFileTypeListVlaue(expectedSelectedFileTypeList[0]);
                _claimAction.GetFileUploadPage.GetPlaceHolderValue()
                    .ShouldBeEqual(expectedSelectedFileTypeList[0], "File Type Text");
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
                    .ShouldBeEqual(fileToUpload.Split(';')[0], "Document correct and present under files to upload");
                _claimAction.GetFileUploadPage.ClaimFileToUploadDocumentValue(1, 3)
                    .ShouldBeEqual("Test Description",
                        "Document Description is correct and present under files to upload.");
                _claimAction.GetFileUploadPage.ClaimFileToUploadDocumentValue(1, 4)
                    .ShouldBeEqual("Multiple File Types", "Document Type text when multiple File Types is selected.");
                _claimAction.GetFileUploadPage.GetFileToUploadTooltipValue(1, 4)
                    .ShouldBeEqual(expectedSelectedFileType, "Document Type correct and present under files to upload");
                _claimAction.IsPageErrorPopupModalPresent()
                    .ShouldBeFalse(
                        "Page error pop up should not be present if no description is set as its not a required field");
                _claimAction.GetFileUploadPage.ClickOnSaveUploadBtn();
                _claimAction.GetFileUploadPage.IsDocumentDivPresent().ShouldBeTrue("Document List are present");
                _claimAction.IsUploadNewDocumentFormPresent()
                    .ShouldBeFalse("Upload New document div should be closed after uploading document.");
                _claimAction.GetFileUploadPage.IsFollowingAppealDocumentPresent(fileToUpload.Split(';')[0])
                    .ShouldBeTrue("Uploaded Document is listed");
                var expectedDate = _claimAction.GetFileUploadPage.AppealDocumentsListAttributeValue(1, 1, 4);
                _claimAction.GetClaimDocsCountInBadge()
                    .ShouldBeEqual((claimDocsCount + 1).ToString(), "Claim Doc badge is updated.");
                StringFormatter.PrintMessageTitle(
                    "Validation of Audit Record to display in claim Processing History after document upload.");
                _claimAction.IsClaimAuditAddedForDocumentUpload(claimSequence)
                    .ShouldBeTrue("Claim Audit for Document Upload is added in database ");
                ClaimProcessingHistoryPage claimProcessingHistory =
                    _claimAction.ClickOnClaimProcessingHistoryAndSwitch();
                var date = claimProcessingHistory.GetClaimHistoryTableDataValueByRowCol(1, 1);
                // date.IsDateTimeInFormat().ShouldBeTrue("Upload Date time is in correct format");
                Convert.ToDateTime(date).Date
                    .ShouldBeEqual(Convert.ToDateTime(expectedDate).Date, "Correct Audit Date is displayed.");
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
                    .ShouldBeEqual(fileToUpload.Split(';')[0], "Document correct and present under files to upload");
                _claimAction.GetFileUploadPage.ClickOnDeleteIconInFilesToUpload(1);
                _claimAction.GetFileUploadPage.GetFilesToUploadCount()
                    .ShouldBeLess(filesToUploadCount, "Document had been removed.");
                _claimAction.GetFileUploadPage.ClickOnCancelBtn();
                // selecting cancel closed form and discards added files
                _claimAction.GetFileUploadPage.IsDocumentDivPresent().ShouldBeTrue("Document List are present");
                _claimAction.GetFileUploadPage.DocumentCountOfFileList()
                    .ShouldBeEqual(existingDocCount,
                        "Added Files has been discarded and has not been associated to appeal");
            }
        }

        [Test, Category("NewClaimAction4")]//US67025
        //[Retrying(Times = 2)]
        public void Verify_view_delete_of_uploaded_documents_in_Claim_action_page_for_Cotiviti_User()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(4))
            {
                ClaimActionPage _claimAction;
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var claimSequenceToViewDeleteDocs = paramLists["ClaimSequence"];
                var fileToUpload = paramLists["FileToUpload"].Split(';');
                var expectedSelectedFileTypeList =
                    paramLists["SelectedFileList"].Split(',').Select(x => x.Trim()).ToList();
                var docDescription = paramLists["DocDescription"];
                var userName = paramLists["UserName"];
                var userType = paramLists["UserType"];
                var auditDate = paramLists["AuditDate"];
                expectedSelectedFileTypeList.Sort();

                _claimAction = automatedBase.CurrentPage.NavigateToClaimSearch()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(claimSequenceToViewDeleteDocs, true);
                _claimAction.DeleteClaimDocumentRecord(claimSequenceToViewDeleteDocs, auditDate);
                _claimAction.ClickOnClaimDocuments();

                //if (_claimAction.GetFileUploadPage.IsFollowingAppealDocumentPresent(fileToUpload[0]))
                //{
                //    Console.WriteLine("Delete document if previously run test failed to delete uploaded document");
                //    _claimAction.GetFileUploadPage.ClickOnDeleteFileBtn(fileToUpload[0]);                
                //    _claimAction.ClickOkCancelOnConfirmationModal(true);
                //    _claimAction.DeleteClaimDocumentRecord(claimSequenceToViewDeleteDocs, auditDate);
                //    _claimAction.ClickOnClaimDocuments();
                //}


                var existingDocCount = _claimAction.GetFileUploadPage.DocumentCountOfFileList();
                var claimStatus = _claimAction.GetClaimStatus();
                _claimAction.GetFileUploadPage.SetFileUploaderFieldValue("Description", docDescription);
                _claimAction.GetFileUploadPage.SetFileTypeListVlaue("All");

                _claimAction.GetFileUploadPage.AddFileForUpload(fileToUpload[0]);
                _claimAction.GetFileUploadPage.ClickOnAddFileBtn();
                _claimAction.GetFileUploadPage.ClickOnSaveUploadBtn();
                _claimAction.GetFileUploadPage.DocumentCountOfFileList()
                    .ShouldBeEqual(++existingDocCount, "New document has been added");
                _claimAction.GetFileUploadPage.IsFollowingAppealDocumentPresent(fileToUpload[0])
                    .ShouldBeTrue("Uploaded Document is listed");
                Console.WriteLine("Verify the Status of the claim after the document is uploaded");
                _claimAction.GetClaimStatus().ShouldBeEqual(claimStatus, "Verified Claim Status after Document Upload");

                _claimAction.ClickOnDocumentToViewAndStayOnPage(2);//window opens to view appeal document 
                _claimAction.GetOpenedDocumentText().ShouldBeEqual("test test", "document detail");
                _claimAction.CloseDocumentTabPageAndBackToNewClaimAction();

                _claimAction.GetFileUploadPage.AppealDocumentsListAttributeValue(1, 1, 2)
                    .ShouldBeEqual(fileToUpload[0], "Document filename is displayed");
                _claimAction.GetFileUploadPage.AppealDocumentsListAttributeValue(1, 1, 4)
                    .IsDateTimeInFormat()
                    .ShouldBeTrue("Document date is displayed and in proper format");
                _claimAction.GetFileUploadPage.AppealDocumentsListAttributeValue(1, 2, 2)
                    .ShouldBeEqual(docDescription, "Document Description is displayed");

                _claimAction.GetFileUploadPage.GetFileTypeAttributeListVaues(1, 1, 3)
                    .ShouldCollectionBeEqual(expectedSelectedFileTypeList, "Document fileType is displayed");
                _claimAction.GetFileUploadPage.GetFileTypeAttributeListToolTip(1, 1, 3)
                    .ShouldCollectionBeEqual(expectedSelectedFileTypeList, "Document fileType tooltip is displayed");



                _claimAction.GetFileUploadPage.IsEllipsisPresentInFileType(1)
                    .ShouldBeTrue("Is Ellipsis Present for lengthy values");

                _claimAction.ClickOnDocumentToViewAndStayOnPage(2); //window opens to view appeal document 
                _claimAction.GetOpenedDocumentText().ShouldBeEqual("test test", "document detail");
                _claimAction.CloseDocumentTabPageAndBackToNewClaimAction();

                _claimAction.IsClaimAuditAddedForDocumentDownload(claimSequenceToViewDeleteDocs)
                    .ShouldBeTrue("Claim Audit for Document Download is added in database ");


                _claimAction.GetFileUploadPage.ClickOnDeleteFileBtn(fileToUpload[0]);

                if (_claimAction.IsPageErrorPopupModalPresent())
                    _claimAction.GetPageErrorMessage()
                        .ShouldBeEqual("The selected document will be deleted. Do you wish to proceed?");
                _claimAction.ClickOkCancelOnConfirmationModal(false);

                _claimAction.GetFileUploadPage.DocumentCountOfFileList()
                    .ShouldBeEqual(existingDocCount, "All the documents are still entact and listed");
                _claimAction.GetFileUploadPage.ClickOnDeleteFileBtn(fileToUpload[0]);
                _claimAction.ClickOkCancelOnConfirmationModal(true);
                _claimAction.GetFileUploadPage.DocumentCountOfFileList()
                    .ShouldBeLessOrEqual(existingDocCount, "Deleted document hasnt been listed.");

                _claimAction.GetFileUploadPage.IsFollowingAppealDocumentPresent(fileToUpload[0])
                    .ShouldBeFalse("Deleted Document isn't present");

                Console.WriteLine("Verify the Status of the claim after the document is deleted");
                _claimAction.GetClaimStatus().ShouldBeEqual
                    (claimStatus, "Verified Claim Status Before Document Deletion");

                StringFormatter.PrintMessageTitle(
                    "Validation of Audit Record to display in claim Processing History after document upload.");

                _claimAction.IsClaimAuditAddedForDocumentDelete(claimSequenceToViewDeleteDocs)
                    .ShouldBeTrue("Claim Audit for Document Upload is added in database ");
                ClaimProcessingHistoryPage claimProcessingHistory =
                    _claimAction.ClickOnClaimProcessingHistoryAndSwitch();
                claimProcessingHistory.GetClaimHistoryTableDataValueByRowCol(1, 2)
                    .ShouldBeEqual("Document Delete", " Correct Audit Action is displayed.");
                claimProcessingHistory.GetClaimHistoryTableDataValueByRowCol(1, 4)
                    .DoesNameContainsOnlyFirstWithLastname()
                    .ShouldBeTrue("Username Should be in <FirstName> <LastName> (userid)");
                claimProcessingHistory.GetClaimHistoryTableDataValueByRowCol(1, 4).ShouldBeEqual(userName,
                    "Username Should be in <FirstName> <LastName> (userid)");
                claimProcessingHistory.GetClaimHistoryTableDataValueByRowCol(1, 5)
                    .ShouldBeEqual(userType, "Correct User Type is diplayed.");
                claimProcessingHistory.GetClaimHistoryTableDataValueByRowCol(1, 6)
                    .ShouldBeEqual("Unreviewed", "Correct Claim Status is displayed.");
                _claimAction.CloseAllExistingPopupIfExist();
            }
        }

        [Test, Category("NewClaimAction4")]//US67025
        public void Verify_scroll_bar_is_displayed_in_Claim_Document_quadrant_if_the_list_goes_beyond_the_size_in_Claim_action_page_for_Cotiviti_User()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(4))
            {
                ClaimActionPage _claimAction;
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var claimSequenceToVerifyScrollBar = automatedBase.DataHelper.GetSingleTestData
                    (FullyQualifiedClassName, TestExtensions.TestName, "ClaimSequenceWithDocs", "Value");
                _claimAction = automatedBase.CurrentPage.NavigateToClaimSearch()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(claimSequenceToVerifyScrollBar, true);
                _claimAction.ClickOnClaimDocuments();
                _claimAction.IsVerticalScrollBarPresentInClaimDocumentSection()
                    .ShouldBeTrue(
                        "Scrollbar Should display in Claim Documents Section when the list of documents extends out of the view");
            }
        }

        [Test, Category("NewClaimAction4")]//US67974
        [NonParallelizable]
        public void Verify_auto_approved_flags_message_while_deleting_all_flag_for_claims_with_and_without_AutoApproved_Flags()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(4))
            {
                ClaimActionPage _claimAction;
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var claimSequenceWithAAFlag = paramLists["ClaimSequenceWithAAFlag"];
                var claimSequenceWithoutAAFlag = paramLists["ClaimSequenceWithoutAAFlag"];
                var flaseq = paramLists["FlagSeq"];
                var lino = paramLists["lino"];

                _claimAction = automatedBase.CurrentPage.NavigateToClaimSearch()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(claimSequenceWithAAFlag, true);
                _claimAction.DeleteClaimAuditOnly(claimSequenceWithAAFlag, "22-NOV-2017");
                _claimAction.DeleteClaimAuditOnly(claimSequenceWithoutAAFlag, "22-NOV-2017");
                _claimAction.DeleteClaimAuditRecordFromDatabase(flaseq, claimSequenceWithoutAAFlag, lino);
                _claimAction.DeleteLineFlagAuditByClaimSequence(claimSequenceWithAAFlag, ClientEnum.SMTST.ToString());
                _claimAction.RestoreClaimQAAuditByClaimSequence(claimSequenceWithAAFlag);
                _claimAction.UpdateStatusAndRestoreFlags(claimSequenceWithoutAAFlag, ClientEnum.SMTST.ToString());
                _claimAction.UpdateStatusAndRestoreFlags(claimSequenceWithAAFlag, ClientEnum.SMTST.ToString());
                try
                {

                    if (_claimAction.IsRestoreButtonPresent())
                        _claimAction.ClickOnRestoreAllFlagsIcon();
                    _claimAction.ClickOnDeleteAllFlagsIcon(false);
                    _claimAction.IsPageErrorPopupModalPresent().ShouldBeTrue(
                        "The AA flags warning message should  be shown when there is at least one flag that the user has the product authority to delete, that is also auto-approved. ");
                    _claimAction.GetPageErrorMessage().ShouldBeEqual(
                        "At least one auto-reviewed flag is present on this claim. Do you wish to delete all flags?");
                    _claimAction.ClickOnErrorMessageOkButton();
                    _claimAction.ClickOnApproveButton();
                    _claimAction.WaitForStaticTime(500);

                    SearchByClaimSeqFromWorkList(claimSequenceWithoutAAFlag);
                    _claimAction.ClickOnDeleteAllFlagsIcon();
                    _claimAction.IsPageErrorPopupModalPresent().ShouldBeFalse(
                        "The AA flags warning message should not be shown when there is no Auto Approved flags on claim. ");
                    _claimAction.ClickOnRestoreAllFlagsIcon();
                }
                finally
                {
                    if (_claimAction.IsPageErrorPopupModalPresent())
                        _claimAction.ClosePageError();
                    if (_claimAction.IsRestoreButtonPresent())
                        _claimAction.ClickOnRestoreAllFlagsIcon();
                    SearchByClaimSeqFromWorkList(claimSequenceWithAAFlag);
                    if (_claimAction.IsRestoreButtonPresent())
                        _claimAction.ClickOnRestoreAllFlagsIcon();

                }

                void SearchByClaimSeqFromWorkList(string claimSeq)
                {
                    if (_claimAction.IsPageHeaderPresent())
                    {
                        if (!_claimAction.IsWorkListControlDisplayed())
                            _claimAction.ClickWorkListIcon();
                    }
                    _claimAction.ClickSearchIcon()
                        .SearchByClaimSequence(claimSeq);
                    _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                    //_claimAction.ClickWorkListIcon();
                    _claimAction.WaitForCondition(() => !_claimAction.IsWorkListControlDisplayed(), 3000);
                }
            }
        }

        [Test, Category("NewClaimAction4")] //US67973
        public void Verify_Trigger_Claims_Shown_For_FOT_Edit()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(4))
            {
                ClaimActionPage _claimAction;

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var claimSequenceWithFOTFlag = paramLists["ClaimSequnceWithFOTFlagDetails"];

                _claimAction = automatedBase.CurrentPage.NavigateToClaimSearch()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(claimSequenceWithFOTFlag, true);
               
                var triggerClaimsFromDB = _claimAction.GetTriggerClaimsFromDB(claimSequenceWithFOTFlag);

                try
                {
                    _claimAction.ClickOnFlagLineByFlag("FOT");
                    var TriggerClaimsNoComma =
                        _claimAction.GetTriggerClaimsList().Select(x => x.Replace(",", "")).ToList();
                    TriggerClaimsNoComma.Count.ShouldBeGreater(0,
                        "Trigger Claims Value should be populated as Comma Separated?");
                    triggerClaimsFromDB[0].Split(',').ToList()
                        .ShouldBeEqual(TriggerClaimsNoComma, "Trigger Claims List should be as expected?");

                    _claimAction.ClickOnTriggerClaimOnFlagDetailsAndGetClaSeq().ShouldBeEqual(TriggerClaimsNoComma[0],
                        "Claseq for page opened through HyperLink Press should equal to claseq: " +
                        TriggerClaimsNoComma[0]);

                    _claimAction.SwitchBackToNewClaimActionPage()
                        .ShouldNotBeNull("Pop up browser should not replace existing page?");

                }
                finally
                {
                    _claimAction.SwitchBackToNewClaimActionPage();
                }
            }
        }

        [Test]//CAR-12(CAR-190)
        public void Verify_claim_work_list_with_PCI_RN_Worklist_functionality()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(4))
            {
                ClaimActionPage _claimAction;

                const string restriction = "Offshore";
                var expectedClaimWorkList = automatedBase.DataHelper.GetMappingData(FullyQualifiedClassName, "claim_work_list").Values
                    .ToList();
                var expectedClaimWorkListSidebarpanel = automatedBase.DataHelper
                    .GetMappingData(FullyQualifiedClassName, "claim_work_list_sidebarpanel").Values.ToList();

                var _claimSequence = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, "ClassInit", "ClaimSequence", "Value");

                _claimAction = automatedBase.CurrentPage.NavigateToClaimSearch()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(_claimSequence, true);

                try
                {
                    _claimAction.GetSecondarySubMenuOptionList(HeaderMenu.Claim, SubMenu.ClaimWorkList)
                        .ShouldCollectionBeEqual(expectedClaimWorkList, "Is SubMenu Under Claim Work List Equal?");
                    _claimAction.GetSideBarPanelSearch.OpenSidebarPanel();
                    _claimAction.GetSideBarPanelSearch.GetOptionListOnArrowDownIcon()
                        .ShouldCollectionBeEqual(expectedClaimWorkListSidebarpanel,
                            "Is SubMenu Under Claim Work List Equal?");
                    _claimAction.ToggleWorkListToPciRN();
                    _claimAction.ClickOnCreateButton();
                    _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                    _claimAction.IsCoderReviewByClaimSequenceNull(_claimAction.GetClaimSequence())
                        .ShouldBeTrue("Is Coder_Review Null for display claim?");
                    _claimAction.GetSideBarPanelSearch.OpenSidebarPanel();
                    _claimAction.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Claim Restrictions",
                        restriction);
                    _claimAction.ClickOnCreateButton();
                    if (_claimAction.GetPageHeader() == PageHeaderEnum.ClaimAction.GetStringValue())
                    {
                        _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                        _claimAction.GetSideBarPanelSearch.OpenSidebarPanel();
                        _claimAction.DoesClaimContainRespectiveRestriction(_claimAction)
                            .ShouldBeTrue("Verify Claim Restriction Rules");
                    }

                    _claimAction.IsRoleAssigned<ClaimSearchPage>(
                        new List<string> {automatedBase.EnvironmentManager.Username},
                        RoleEnum.ClinicalValidationAnalyst.GetStringValue()).ShouldBeTrue(
                        "Is CV RN Work List present for current user:" + automatedBase.EnvironmentManager.Username);
                }
                finally
                {
                    automatedBase.QuickLaunch = _claimAction.Logout().LoginAsHciAdminUser4();
                    _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch()
                        .SearchByClaimSequenceToNavigateToClaimActionPage(_claimSequence);
                    _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                }
            }
        }

        [Test, Category("AppealDependent"), Category("OnDemand")] //US45712 + TANT-189
        public void Verify_create_appeal_enabled_only_when_Status_is_Client_Reviewed_or_Unreviewed_with_tool_tip_message_for_disabled_create_icon_and_DCI_disabled()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(4))
            {
                ClaimActionPage _claimAction;

                var _claimSequence = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, "ClassInit", "ClaimSequence", "Value");

                _claimAction = automatedBase.CurrentPage.NavigateToClaimSearch()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(_claimSequence, true);

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var clientReviewedClaimSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "ClientReviewedClaimSeq", "Value");
                var clientUnreviewedClaimSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "ClientUnreviewedClaimSeq", "Value");
                var claimSeqList =
                    automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName, "ClaimSequenceList",
                            "Value")
                        .Split(',')
                        .ToList();
                var lockedClaimSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                    "LockedClaimSeq", "Value");
                var closedAppealClaimSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "ClosedAppealClaimSeq", "Value");
                var completeAppealClaimSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "CompleteAppealClaimSeq", "Value");

                try
                {
                    StringFormatter.PrintMessageTitle($"Setting {ProductEnum.DCA} to off for {ClientEnum.SMTST}");
                    _claimAction.GetCommonSql.UpdateSpecificClientDataInDB("DCI_ACTIVE = 'F'",
                        ClientEnum.SMTST.ToString());
                    _claimAction.RefreshPage(false);

                    SearchByClaimSeqFromWorkList(clientReviewedClaimSeq);
                    _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                    _claimAction.IsAddAppealIconEnabled()
                        .ShouldBeTrue("Create Appeal Icon Should be enabled for claim status having client Reviewed");
                    SearchByClaimSeqFromWorkList(clientUnreviewedClaimSeq);
                    _claimAction.IsAddAppealIconEnabled()
                        .ShouldBeTrue("Create Appeal Icon Should be enabled for claim status having client Unreviewed");
                    SearchByClaimSeqFromWorkList(closedAppealClaimSeq);
                    _claimAction.IsAddAppealIconEnabled()
                        .ShouldBeTrue(
                            "Create Appeal Icon Should be enabled for claim status having client Reviewed and existing appeal is closed");
                    SearchByClaimSeqFromWorkList(completeAppealClaimSeq);
                    _claimAction.IsAddAppealIconEnabled()
                        .ShouldBeTrue(
                            "Create Appeal Icon Should be enabled for claim status having client Reviewed and existing appeal is complete");

                    StringFormatter.PrintMessageTitle("Verification of tooltip of disabled create appeal icon");
                    Random rand = new Random();

                    for (var i = 0; i < 3; i++)
                    {
                        SearchByClaimSeqFromWorkList(claimSeqList[rand.Next(0, 10)]);
                        _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                        var status = _claimAction.GetClaimStatus();
                        _claimAction.IsAddAppealIconDisabled()
                            .ShouldBeTrue(string.Format(
                                "Create Appeal Should disabled for claims having status<{0}> that has not been released to the client",
                                status));
                        _claimAction.GetToolTipMessageDisabledCreateAppealIcon()
                            .ShouldBeEqual(
                                "Appeals cannot be added to a claim  that has not been released to the client.",
                                "Tooltip Message for Unreviwed Claims");
                    }

                    SearchByClaimSeqFromWorkList(lockedClaimSeq);
                    _claimAction.IsAddAppealIconDisabled()
                        .ShouldBeTrue(
                            "Create Appeal Should disabled for claims that has appeal whose status is not complete or closed");
                    _claimAction.GetToolTipMessageDisabledCreateAppealIcon()
                        .ShouldBeEqual(
                            "Appeals cannot be opened for claims that have an existing appeal in process.",
                            "Tooltip Message for claim having appeal");
                }

                finally
                {
                    StringFormatter.PrintMessageTitle(
                        $"Finally Block \n Reverting {ProductEnum.DCA} to ON for {ClientEnum.SMTST}");
                    _claimAction.GetCommonSql.UpdateSpecificClientDataInDB("DCI_ACTIVE = 'T'",
                        ClientEnum.SMTST.ToString());
                }

                #region  Private Method
                void SearchByClaimSeqFromWorkList(string claimSeq)
                {
                    if (_claimAction.IsPageHeaderPresent())
                    {
                        if (!_claimAction.IsWorkListControlDisplayed())
                            _claimAction.ClickWorkListIcon();
                    }
                    _claimAction.ClickSearchIcon()
                        .SearchByClaimSequence(claimSeq);
                    _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                    //_claimAction.ClickWorkListIcon();
                    _claimAction.WaitForCondition(() => !_claimAction.IsWorkListControlDisplayed(), 3000);
                }
                #endregion
            }
        }

        [Test] //CAR-983 (CAR-1111)
        [Retrying(Times = 2)]
        public void Verify_Dental_appeals_are_editable_in_popup_from_claim_action()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(4))
            {
                ClaimActionPage _claimAction;
                AppealManagerPage _appealManager;
                AppealActionPage _appealAction;
                AppealSearchPage _appealSearch;
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestExtensions.TestName);
                var flag = paramLists["Flag"];
                var dentalAppealClaimSeq = paramLists["DentalAppealClaimSeq"];
                var adjustReasonCode = paramLists["AdjustReasonCode"];
                var editFlagReasonCode = paramLists["EditFlagReasonCode"];
                var consultantRationales = paramLists["ConsultantRationales"];
                const string rationale = "Test Rationale";
                const string summary = "Test Summary";
                const string note = "Test Note";
                _claimAction = automatedBase.CurrentPage.NavigateToClaimSearch()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(dentalAppealClaimSeq, true);
                _claimAction.ClickOnViewAppealIcon();
                _claimAction.GetAppealTypeByRow(1).ShouldBeEqual("Dental Review");
                var appealSeq = _claimAction.GetAppealSequence();
                var statusBefore = _claimAction.GetAppealStatus(appealSeq);
                if (statusBefore == AppealStatusEnum.Complete.GetStringDisplayValue() ||
                    statusBefore == AppealStatusEnum.Closed.GetStringDisplayValue())
                {
                    _appealManager = _claimAction.NavigateToAppealManager();
                    _appealManager.SearchByClaimSequence(dentalAppealClaimSeq);
                    _appealManager.ClickOnEditIcon();
                    _appealManager.ChangeAppealStatus(AppealStatusEnum.New.GetStringDisplayValue());
                    var newClaimSearch = _appealManager.NavigateToClaimSearch();
                    _claimAction =
                        newClaimSearch
                            .SearchByClaimSequenceToNavigateToClaimActionForClientSwitch(dentalAppealClaimSeq, true);
                    _claimAction.ClickOnViewAppealIcon();
                    statusBefore = _claimAction.GetAppealStatus(appealSeq);
                }

                try
                {
                    _appealAction = _claimAction.ClickOnAppealSequence(appealSeq);
                    _appealAction.HandleAutomaticallyOpenedActionPopup();
                    _appealAction.GetPageHeader().ShouldBeEqual("Appeal Action", "Page Title");
                    StringFormatter.PrintMessage(
                        "Verifying all button are enabled in Appeal Action popup for dental review appeal type");
                    _appealAction.IsEditIconDisabled().ShouldBeFalse("Edit Icon should be enabled");
                    _appealAction.IsPayIconEnabled().ShouldBeTrue("Is Pay Icon Enabled?");
                    _appealAction.IsDenyIconEnabled().ShouldBeTrue("Is Deny Icon Enabled?");
                    _appealAction.IsAdjustIconEnabled().ShouldBeTrue("Is Adjust Icon Enabled?");
                    _appealAction.IsNoDocsIconEnabled().ShouldBeTrue("Is No Docs Icon Enabled?");
                    _appealAction.IsAppealDraftIconEnabled().ShouldBeTrue("Is Save Appeal Draft icon Enabled?");
                    _appealAction.IsAppealLetterIconEnabled().ShouldBeTrue("Is Generate Appeal Letter Icon enabled?");
                    _appealAction.IsSaveDraftButtonDisabled().ShouldBeFalse("Is Save draft button enabled?");
                    StringFormatter.PrintMessage("Verifying edit in Appeal Action popup");
                    var statusAfter = statusBefore == AppealStatusEnum.New.GetStringDisplayValue()
                        ? AppealStatusEnum.HCIConsultantRequired.GetStringDisplayValue()
                        : AppealStatusEnum.New.GetStringDisplayValue();
                    _appealAction.ClickOnEditIcon();
                    _appealAction.SelectEditAppealFieldDropDownListByLabel("Status", statusAfter);
                    _appealAction.ClickOnSaveButton();
                    if (statusAfter == AppealStatusEnum.HCIConsultantRequired.GetStringDisplayValue())
                    {
                        _appealSearch = _appealAction.SwitchToAppealSearch();
                        _appealAction =
                            _appealSearch.SearchByAppealSequenceNavigateToAppealAction(appealSeq.Split('-')[0]);
                    }

                    _appealAction.GetStatus().ShouldBeEqual(statusAfter);
                    _claimAction = _appealAction.CloseAppealActionWindow();
                    _claimAction.Refresh();
                    _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                    _claimAction.ClickOnViewAppealIcon();
                    _claimAction.GetAppealStatus(appealSeq).ShouldBeEqual(statusAfter);
                    StringFormatter.PrintMessage("Verifying editable Appeal Line form and completing appeal");
                    _appealAction = _claimAction.ClickOnAppealSequence(appealSeq);
                    _appealAction.HandleAutomaticallyOpenedActionPopup();
                    if (!_appealAction.IsDenyIconSelected())
                        _appealAction.ClickOnDenyIcon();
                    else
                        _appealAction.ClickOnPayIcon();
                    if (_appealAction.IsPageErrorPopupModalPresent())
                        _appealAction.ClickOkCancelOnConfirmationModal(true);
                    _appealAction.WaitForStaticTime(300);
                    _appealAction.IsEditAppealLineFormEditable("Rationale")
                        .ShouldBeTrue("Is Rationale editor Editable?");
                    _appealAction.IsEditAppealLineFormEditable("Summary").ShouldBeTrue("Is Summary editor editable?");
                    var reasonList = _appealAction.GetReasonCodeList();
                    _appealAction.SetReasonCodeList(reasonList[0]);
                    _appealAction.GetReasonCodeInput()
                        .ShouldBeEqual(reasonList[0], "User is able to select and get Reason Code");
                    _appealAction.SetEditAppealLineTextAreaByHeader("Note", note);
                    _appealAction.SetEditAppealLineIfrmeEditorByHeader("Rationale", rationale);
                    _appealAction.SetConsultantRationales(consultantRationales);
                    _appealAction.SetEditAppealLineIfrmeEditorByHeader("Summary", summary);
                    _appealAction.ClickOnSaveDraftButton();
                    StringFormatter.PrintMessage("Editing Flag lines");
                    _appealAction.ClickOnAdjustIcon();
                    if (_appealAction.IsPageErrorPopupModalPresent())
                        _appealAction.ClickOkCancelOnConfirmationModal(true);
                    _appealAction.MaximizeWindow();
                    _appealAction.WaitForStaticTime(300);
                    _appealAction.IsEditFlagIconEnabled(flag).ShouldBeTrue("Is edit flag icon enabled?");
                    _appealAction.IsDeleteFlagIconEnabled(flag).ShouldBeTrue("Is delete flag icon enabled?");
                    _appealAction.ClickOnEditFlagByFlag(flag);
                    _appealAction.SetReasonCodeList(editFlagReasonCode);
                    _appealAction.ClickOnSaveButton();
                    _appealAction.SetReasonCodeList(adjustReasonCode);
                    _appealAction.SetEditAppealLineIfrmeEditorByHeader("Rationale", rationale);
                    _appealAction.SetEditAppealLineIfrmeEditorByHeader("Summary", summary);
                    _appealAction.SetConsultantRationales(consultantRationales);
                    _appealAction.SetEditAppealLineTextAreaByHeader("Note", note);
                    _appealAction.ClickOnSaveDraftButton();
                    _appealAction.ClickOnAppealLetter();
                    _appealAction.ClickOnApproveIcon();
                    _appealAction.ClickOkCancelOnConfirmationModal(true);
                    _appealAction.WaitForWorking();
                    _claimAction = _appealAction.CloseAppealActionWindow();
                    _claimAction.Refresh();
                    _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                    _claimAction.IsAddAppealIconEnabled()
                        .ShouldBeTrue("Appeal creator icon must be enabled when associated appeal is completed");
                    _appealSearch = _claimAction.NavigateToAppealSearch();
                    _appealSearch.SelectAllAppeals();
                    _appealSearch.SelectSMTST();
                    var _appealSummary = _appealSearch.SearchByClaimSequenceInCotivitiUser(dentalAppealClaimSeq);
                    VerifyThatNameIsInCorrectFormat(
                        _appealSummary.GetAppealLineDetailsAuditValueByLabel("Last Modified User"),
                        "Last Modified User for given appeal.");
                    VerifyThatNameIsInCorrectFormat(
                        _appealSummary.GetAppealLineDetailsAuditValueByLabel("Completed By"),
                        "Completed By displayed for given appeal");
                    VerifyThatDateIsInCorrectFormat(_appealSummary.GetAppealLineDetailsAuditValueByLabel("Completed"),
                        "Completed  displayed for given appeal.");
                    _appealSummary.GetAppealLineDetailsAuditValueByLabel("Reason Code")
                        .ShouldBeEqual(adjustReasonCode, "Reason displayed for given appeal.");
                    VerifyThatReasonCodeIsInCorrectFormat(adjustReasonCode, "reason Code is properly formatted");
                    _appealSummary.GetAppealLineDetailsAuditValueByLabel("Result")
                        .ShouldBeEqual("Adjust", "Result displayed for given appeal.");
                    _appealSummary.GetAppealLineDetailsAuditValueByLabelInDiv("Notes:")
                        .ShouldBeEqual(note, "Notes displayed for given appeal.");
                    _appealSummary.ClickOnDiplayRationaleLink("Rationale");
                    _appealSummary.GetAppealLineDetailsAuditValueByLabel("Rationale")
                        .ShouldBeEqual(rationale, "Rationale displayed for given appeal.");
                    _appealSummary.GetAppealLineDetailsAuditValueByLabel("Summary")
                        .ShouldBeEqual(summary, "Summary displayed for given appeal.");
                    _appealManager = _appealAction.NavigateToAppealManager();
                    _appealManager.SearchByAppealSequence(appealSeq.Split('-')[0]);
                    _appealManager.ClickOnEditIcon();
                    _appealManager.ChangeAppealStatus(AppealStatusEnum.HCIConsultantRequired.GetStringDisplayValue());
                }
                finally
                {
                    _claimAction.CloseAllExistingPopupIfExist();
                }
            }
        }

#endregion

    #region PRIVATE METHODS

        void SearchByClaimSeqFromWorkList(ClaimActionPage _claimAction, string claimSeq)
        {
            if (_claimAction.IsPageHeaderPresent())
            {
                if (!_claimAction.IsWorkListControlDisplayed())
                    _claimAction.ClickWorkListIcon();
            }
            _claimAction.ClickSearchIcon()
            .SearchByClaimSequence(claimSeq);
            _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
            //_claimAction.ClickWorkListIcon();
            _claimAction.WaitForCondition(() => !_claimAction.IsWorkListControlDisplayed(), 3000);
        }

        void HandleAutomaticallyOpenedPatientClaimHistoryPopupFor(ClaimActionPage _claimAction, object obj)
        {
            if (obj is ClaimActionPage)
            {
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
            }
        }

        void DeleteOrRestoreLineFlag(ClaimActionPage _claimAction, string reasonCode)
        {
            try
            {
                bool action = _claimAction.IsFlagDeletable();
                string workItem = !action ? "Delete" : "Restore";

                int expectedDeletedFlags = _claimAction.GetCountOfDeletedFlags();
                int expectedRestoredFlags = _claimAction.GetCountOfRestoreFlags();

                StringFormatter.PrintMessageTitle(workItem + " Single Flag");
                string flag = _claimAction.ClickOnFirstEditFlag(action);
                _claimAction.SelectReasonCode(reasonCode);
                _claimAction.ClickOnWorkButton(action);
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

        private void DeleteOrRestoreMprEditThenPsEditShouldBeDeletedOrRestored(ClaimActionPage _claimAction, string triggLineNum)
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
            _claimAction.GetAlertBoxText().Contains("Claims that have been modified must be APPROVED prior to leaving the page.")
                .ShouldBeTrue("Correct Warning Message Displayed?");
            _claimAction.DismissAlertBoxIfPresent();
        }

        void CheckApplicationAlertMessageThenDismiss(ClaimActionPage _claimAction)
        {
            _claimAction.GetPageErrorMessage()
                .ShouldBeEqual("Claims that have been modified must be APPROVED prior to leaving the page.",
                    "Correct Warning Message Displayed?");
            _claimAction.ClosePageError();
        }

        void ReloadBrowserIfPageErrorPopupAppearsWhenTransferClaim(ClaimActionPage _claimAction,
            string statusCode = "Documentation Requested")
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
        private void VerifyThatNameIsInCorrectFormat(string name, string message)
        {
            Regex.IsMatch(name, @"^(\S+ )+\S+$").ShouldBeTrue(message + " '" + name + "' is in format XXX XXX ");
        }

        private void VerifyThatDateIsInCorrectFormat(string date, string dateName)
        {
            Regex.IsMatch(date, @"^(0[1-9]|1[0-2])\/(0[1-9]|1\d|2\d|3[01])\/(19|20)\d{2}$").ShouldBeTrue("The " + dateName + " Date'" + date + "' is in format MM/DD/YYYY");
        }

        private void VerifyThatReasonCodeIsInCorrectFormat(string reasonCode, string message)
        {
            Regex.IsMatch(reasonCode, @"^([a-zA-Z0-9\s]+)+ (-)+([a-zA-Z0-9\s]+)").ShouldBeTrue(message + " '" + reasonCode + "' is in format XXX - XXX ");
        }
            #endregion
    }
}
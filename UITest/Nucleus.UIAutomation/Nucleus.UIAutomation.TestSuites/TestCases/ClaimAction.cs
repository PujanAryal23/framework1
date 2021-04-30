using System;
using System.Collections.Generic;
using System.Linq;
using Nucleus.Service.Data;
using Nucleus.Service.PageServices.Appeal;
using Nucleus.Service.Support.Enum;
using NUnit.Framework;
using Nucleus.Service.PageServices.Claim;
using Nucleus.Service.Support.Common.Constants;
using Nucleus.Service.Support.Environment;
using Nucleus.UIAutomation.TestSuites.Base;
using System.Diagnostics;
using Nucleus.Service.PageServices.QA;
using Nucleus.Service.PageServices.QuickLaunch;
using Nucleus.Service.PageServices.Settings;
using Nucleus.Service.PageServices.Settings.User;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Utils;
using Nucleus.Service.Support.Menu;
using Nucleus.UIAutomation.TestSuites.Common;



namespace Nucleus.UIAutomation.TestSuites.TestCases
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class ClaimAction 
    {
        /* #region PRIVATE FIELDS

         private ProfileManagerPage _profileManager;
         private UserProfileSearchPage _newUserProfileSearch;
         private ClaimActionPage _claimAction;
         private string _claimSequenceWithAppeal = string.Empty;
         private LogicSearchPage _logicSearch;
         private ClientSearchPage _clientSearch;

         private string _claimSeq;
         private ClaimProcessingHistoryPage _claimProcessingHistory;
         private ClaimHistoryPage _claimHistory;
         private CommonValidations _commonValidation;
         private readonly string _dciWorkListPrivilege = RoleEnum.DCIAnalyst.GetStringValue();
         private ClaimSearchPage _claimSearch;
         private QaClaimSearchPage _qaClaimSearch;
         private InvoiceDataPage _invoicedataPage;



                 #region OVERRIDE METHODS

         protected override void ClassInit()
         {
            try
             {
                 UserLoginIndex = 1;
                 base.ClassInit();
                 _claimSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, "ClassInit", "ClaimSequence", "Value");
                 //QuickLaunch.Logout().LoginAsHciAdminUser1();
                 _claimAction = QuickLaunch.NavigateToClaimSearch()
                     .SearchByClaimSequenceToNavigateToClaimActionPage(_claimSeq);
                 _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                 _commonValidation = new CommonValidations(CurrentPage);
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
             if (_claimAction.IsClaimLocked())
                 Console.WriteLine("Claim is Locked!");
             if (_claimAction.IsPageErrorPopupModalPresent())
                 _claimAction.ClosePageError();


             base.TestCleanUp();
             if (string.Compare(UserType.CurrentUserType, UserType.HCIADMIN1, StringComparison.OrdinalIgnoreCase) != 0)
             {
                 _claimAction = _claimAction.Logout()
                     .LoginAsHciAdminUser1().ClickOnSwitchClient().SwitchClientTo(automatedBase.EnvironmentManager.TestClient).
                     NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(_claimSeq);
             }
             else if (!CurrentPage.IsCurrentClientAsExpected(automatedBase.EnvironmentManager.TestClient))
             {
                 CheckTestClientAndSwitch();
                 _claimAction = QuickLaunch.NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(_claimSeq);
             }
             else if (!CurrentPage.GetPageHeader().Equals(PageHeaderEnum.ClaimAction.GetStringValue()))
             {
                 CurrentPage = _claimAction = CurrentPage.NavigateToClaimSearch().
                     SearchByClaimSequenceToNavigateToClaimActionPage(_claimSeq);
                 HandleAutomaticallyOpenedPatientClaimHistoryPopupFor(_claimAction);
             }

             else if (_claimAction.IsWorkingAjaxMessagePresent())
             {
                 _claimAction.Refresh();
                 _claimAction.WaitForWorkingAjaxMessage();
                 _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();

             }
         }

         protected override void ClassCleanUp()
         {
             try
             {
                 _claimAction.CloseDatabaseConnection();
                 //_claimAction.Logout();
                 //if (CurrentPage.IsQuickLaunchIconPresent())
                 //    _claimAction.GoToQuickLaunch();
             }

             finally
             {
                 base.ClassCleanUp();
             }
         }

         #endregion

         #endregion */

        #region PROTECTED PROPERTIES

        protected string FullyQualifiedClassName
        {
            get
            {
                return GetType().FullName;
            }
        }
        #endregion



        #region TEST SUITES

        [Test] //CAR-3253(CAR-3297)
        [Author("Shyam Bhattarai")]
        public void Verify_Default_Reason_Code_For_COB_Flags()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var paramList = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var claseq = paramList["claseq"];
                var reasonCode = paramList["reasonCode"];
                
                ClaimActionPage _claimAction = automatedBase.CurrentPage.NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(claseq, true);

                try
                {
                    var deleteFlagActions = new Dictionary<string, Action>()
                    {
                        ["Delete all Flags on Claim"] = () => { _claimAction.ClickOnDeleteAllFlagsIcon(); },
                        ["Delete all flags in line"] = () => { _claimAction.ClickOnDeleteAllFlagsIconOnFlagLine(); },
                        ["Delete particular flag"] = () => { _claimAction.ClickOnDeleteIconOnFlagByLineNoAndRow(claseq); }
                    };

                    foreach (var deleteAction in deleteFlagActions)
                    {
                        StringFormatter.PrintMessageTitle($"Performing {deleteAction.Key}");
                        deleteAction.Value();
                        _claimAction.ClickOnClaimFlagAuditHistoryIcon();

                        StringFormatter.PrintMessage("Verifying the Claim Flag Audit History window to verify the default reason code");
                        _claimAction.GetFlagAuditDetailByRowCol(3, 1).ShouldBeEqual(reasonCode);
                        _claimAction.ClickOnRestoreAllFlagsIcon();
                    }
                }
                finally
                {
                    StringFormatter.PrintMessage("Final Block : Deleting the audit records and restoring all the flags");
                    _claimAction.DeleteClaimAuditRecordFromDatabaseByClaseq(claseq, "1");

                    _claimAction.RestoreAllFlagByClaimSequence(claseq);
                }
            }
        }


        [Test, Category("OnDemand")] // CAR-3110(CAR-3097)
        [NonParallelizable]
        public void Verify_Decision_Point_and_Reason_in_Flag_details_for_PPM_claims()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(1))
            {
                ClaimActionPage _claimAction;
                var _claimSeq =
                    automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, "ClassInit", "ClaimSequence",
                        "Value");
                //QuickLaunch.Logout().LoginAsHciAdminUser1();
                _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(_claimSeq);
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();

                 var TestName = new StackFrame(true).GetMethod().Name;
                var paramList = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestName);
                var claSeq = paramList["ClaimSequence"];
                var flag = paramList["Flag"];
                var recReasonCodeAndDpKeyFromDb = _claimAction.GetRecReasonCodeAndDpKeyFromDb(claSeq);

                Random rnd = new Random();
                var processingTypes = new List<string>
                {
                    "R",
                    "PR",
                    "PB"
                };
                var processingType = processingTypes[rnd.Next(processingTypes.Count)];
                StringFormatter.PrintMessage($"Changing the processing type of RPE to {processingType}");
                _claimAction.GetCommonSql.UpdateProcessingTypeOfClient(processingType, ClientEnum.RPE.ToString());

                try
                {
                    StringFormatter.PrintMessage("Switching to RPE client");
                    automatedBase.CurrentPage.ClickOnSwitchClient().SwitchClientTo(ClientEnum.RPE);

                    _claimAction = automatedBase.CurrentPage.NavigateToClaimSearch()
                        .SearchByClaimSequenceToNavigateToClaimActionPage(claSeq);
                    _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                    if (_claimAction.IsPageErrorPopupModalPresent())
                        _claimAction.ClosePageError();
                    _claimAction.ClickOnFlagLineByLineNoWithFlag(1, flag);

                    if (processingType.Equals(processingTypes[0]))
                    {
                        StringFormatter.PrintMessageTitle(
                            "Verifying whether for client with processing type neither PCA Batch nor PCA Realtime, " +
                            "Reason and Decision points should not be displayed");

                        _claimAction.IsFlagDetailLabelPresentByLabelName("Reason Code:")
                            .ShouldBeFalse("Reason code should not be present for non PCA batch or realtime client");
                        _claimAction.IsFlagDetailLabelPresentByLabelName("Decision Point:")
                            .ShouldBeFalse("Decision point should not be present for non PCA batch or realtime client");
                    }

                    else
                    {
                        StringFormatter.PrintMessageTitle(
                            "Verifying whether for client with processing type either PCA Batch or PCA Realtime, " +
                            "Reason and Decision points should be displayed");

                        _claimAction.IsFlagDetailLabelPresentByLabelName("Reason Code:")
                            .ShouldBeTrue("Reason Code be present for PCA batch or realtime client");
                        _claimAction.IsFlagDetailLabelPresentByLabelName("Decision Point:")
                            .ShouldBeTrue("Decision Point should be present for PCA batch or realtime client");

                        StringFormatter.PrintMessage("Verifying Reason and Decision points against the DB");
                        _claimAction.GetFlagDetailsDataPointValueByUlAndLiNumber(2, 1)
                            .ShouldBeEqual(recReasonCodeAndDpKeyFromDb[0], "Reason code should match");
                        _claimAction.GetFlagDetailsDataPointValueByUlAndLiNumber(2, 2)
                            .ShouldBeEqual(recReasonCodeAndDpKeyFromDb[1], "Decision Point should match");
                    }
                }
                finally
                {
                    StringFormatter.PrintMessageTitle(
                        "Finally block : Reverting client to SMTST and the processing type for RPE");

                    if (_claimAction.IsPageErrorPopupModalPresent())
                        _claimAction.ClosePageError();

                    if (!_claimAction.IsDefaultTestClientForEmberPage(ClientEnum.SMTST))
                        _claimAction.ClickOnSwitchClient().SwitchClientTo(ClientEnum.SMTST, true);

                    _claimAction.GetCommonSql.UpdateProcessingTypeOfClient(processingTypes[0],
                        ClientEnum.RPE.ToString());
                }
            }
        }

        [Test, Category("OnDemand")] // CAR-2913(CAR-2866)
        [NonParallelizable]
        public void Verify_no_record_for_Real_time_claim_queue_record_if_claim_is_going_to_QC()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(1))
            {
                ClaimActionPage _claimAction = null;
                var TestName = new StackFrame(true).GetMethod().Name;
                var paramList = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestName);
                var claSeq = paramList["ClaimSequence"];

                try
                {
                    StringFormatter.PrintMessage("Logging in and verifying as user without QA Analyst Role for first Approval");

                    _claimAction = automatedBase.QuickLaunch.Logout().LoginWithDefaultSuspectProvidersPage().SwitchClientTo(ClientEnum.RPE).NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(claSeq, true);
                    _claimAction.GetClaimStatus().ShouldBeEqual(StatusEnum.CotivitiUnreviewed.GetStringDisplayValue(),
                        "Status should be Cotiviti Unreviewed");
                    _claimAction.ClickOnApproveButton();


                    StringFormatter.PrintMessage("Verifying the same user cannot approve for the second time");
                    _claimAction.ClickOnClaimSequenceOfSearchResult(1);
                    _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                    _claimAction.IsApproveButtonDisabled().ShouldBeTrue("Is Approve button disabled?");

                    StringFormatter.PrintMessage(
                        "Verifying the record is not written on first approval by the user without QA Analyst role");
                    _claimAction.GetRecordCountOfRealTimeClaimQueueAuditFromDb(claSeq).ShouldBeEqual(0,
                        "No Record should be written on first Approval by the user without QA Analyst role");

                    StringFormatter.PrintMessage("Logging in from the user with QA Analyst role");
                    _claimAction.Logout().LoginAsHciAdminUser1().SwitchClientTo(ClientEnum.RPE).NavigateToClaimSearch()
                        .SearchByClaimSequenceToNavigateToClaimActionPage(claSeq, true);
                    _claimAction.GetClaimStatus().ShouldBeEqual(StatusEnum.CotivitiUnreviewed.GetStringDisplayValue(),
                        "After first Approval, status should not change");
                    _claimAction.ClickOnApproveButton();
                    _claimAction.GetGridViewSection.GetValueInGridByColRow(9)
                        .ShouldBeEqual(StatusEnum.ClientUnreviewed.GetStringDisplayValue(),
                            "Status should change after second approval by user with QA Analyst role");
                    _claimAction.GetRecordCountOfRealTimeClaimQueueAuditFromDb(claSeq).ShouldBeEqual(1,
                        "After second approval by the user with QA Analyst role is a record should be written");

                }

                finally
                {
                    _claimAction.DeleteRealTimeClaimQueueRecordFromDb(claSeq);
					_claimAction.DeleteRealTimeClaimQueueRecordAuditFromDb(claSeq);
                    _claimAction.UpdateClaimStatus(claSeq, ClientEnum.RPE.ToString(),
                        StatusEnum.CotivitiUnreviewed.GetStringValue());
                    _claimAction.DeleteClaimAuditRecordByClientAndClaseqFromDb(ClientEnum.RPE.ToString(), claSeq);
                    _claimAction.UpdateAuditCompletedAndCompletedDateFromDb(ClientEnum.RPE.ToString(), "F", "null",
                        claSeq);
                    _claimAction.SwitchClientTo(ClientEnum.SMTST);
                }
            }
        }

        [Test, Category("OnDemand")] //CAR-2649(CAR-2546)
        [NonParallelizable]
        public void Verify_claim_lock_for_CVP_and_PCA_clients()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(1))
            {
                ClaimActionPage _claimAction;
                ClientSearchPage _clientSearch;
                var _claimSeq =
                    automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, "ClassInit", "ClaimSequence",
                        "Value");
                //QuickLaunch.Logout().LoginAsHciAdminUser1();
                _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(_claimSeq);
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestName);

                var claSeq = paramLists["ClaimSequence"];
                Random rnd = new Random();
                var processingTypes = new List<string>
                {
                    "B",
                    "C",
                    "PB",
                    "PR",
                    "R"
                };
                var processingType = processingTypes[rnd.Next(processingTypes.Count)];
                try
                {
                    StringFormatter.PrintMessage($"Changing the processing type of RPE to {processingType}");
                    _claimAction.GetCommonSql.UpdateProcessingTypeOfClient(processingType, ClientEnum.RPE.ToString());
                    automatedBase.CurrentPage.ClickOnSwitchClient().SwitchClientTo(ClientEnum.RPE, true);
                    _claimAction = automatedBase.CurrentPage.NavigateToClaimSearch()
                        .SearchByClaimSequenceToNavigateToClaimActionPage(claSeq);
                    _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                    _claimAction.GetClaimStatus().ShouldBeEqual(ClaimStatusTypeEnum.ClientReviewed.GetStringValue(),
                        "Status should be Client Reviewed");
                    if (processingType == "B")
                    {
                        _claimAction.IsPageErrorPopupModalPresent()
                            .ShouldBeFalse("Is error pop-up present for Batch ");
                        _claimAction.IsDisabled(ClaimActionPage.ActionItems.DeleteOrRestore)
                            .ShouldBeFalse("Delete/Restore is disabled at Claim level?");
                        _claimAction.IsDeleteIconDisabledByLineNumber()
                            .ShouldBeFalse("Is delete icon disabled at Line level?");
                        _claimAction.IsDeleteIconEnabledByLinenoAndRow(1, 1)
                            .ShouldBeTrue("Is delete icon enabled at flag level?");
                        _claimAction.IsDisabled(ClaimActionPage.ActionItems.Approve)
                            .ShouldBeTrue("Approve icon for internal user with Client reviewed status is disabled");
                        _claimAction.IsDisabled(ClaimActionPage.ActionItems.Edit)
                            .ShouldBeFalse("Edit is disabled at Claim level?");
                        _claimAction.IsEditIconDisabledByLineno(1)
                            .ShouldBeFalse("Is edit icon disabled at line level");
                        _claimAction.IsEditIconEnabledByLinenoAndRow(1, 1)
                            .ShouldBeTrue("Is edit icon enabled at flag level");
                        _claimAction.IsAddIconDisabledInFlaggedLine().ShouldBeFalse("Is add icon disabled?");
                        _claimAction.IsTrasnferApproveIconDisabled()
                            .ShouldBeTrue("Approve icon for internal user with Client reviewed status is disabled");

                    }
                    else
                    {
                        _claimAction.GetPageErrorMessage()
                            .ShouldBeEqual(
                                "Claims that have been returned to the claim payment system cannot be modified.",
                                "Error pop-up should be present");
                        _claimAction.ClosePageError();
                        _claimAction.IsDisabled(ClaimActionPage.ActionItems.DeleteOrRestore)
                            .ShouldBeTrue("Delete/Restore is disabled at Claim level?");
                        _claimAction.IsDeleteIconDisabledByLineNumber()
                            .ShouldBeTrue("Is delete icon disabled at Line level?");
                        _claimAction.IsDeleteIconDisabledByLinenoAndRow(1, 1)
                            .ShouldBeTrue("Is delete icon disabled at flag level?");
                        _claimAction.IsDisabled(ClaimActionPage.ActionItems.Approve)
                            .ShouldBeTrue("Approve is disabled?");
                        _claimAction.IsDisabled(ClaimActionPage.ActionItems.Edit)
                            .ShouldBeTrue("Edit is disabled at Claim level?");
                        _claimAction.IsEditIconDisabledByLineno(1)
                            .ShouldBeTrue("Is edit icon disabled at line level");
                        _claimAction.IsEditIconDisabledByLinenoAndRow(1, 1)
                            .ShouldBeTrue("Is edit icon disabled at flag level");
                        _claimAction.IsAddIconDisabledInFlaggedLine().ShouldBeTrue("Is add icon disabled?");
                        _claimAction.IsTrasnferApproveIconDisabled()
                            .ShouldBeTrue("Is Transfer/Approve icon disabled?");
                    }
                }
                finally

                {
                    _claimAction.GetCommonSql.UpdateProcessingTypeOfClient(processingTypes[4],
                        ClientEnum.RPE.ToString());
                    _claimAction.ClickOnSwitchClient().SwitchClientTo(ClientEnum.SMTST);
                }

            }
        }

        [Test, Category("OnDemand"), Category("NewClaimAction1")] //CAR-507
        [NonParallelizable]
        public void Verify_Presence_Of_AdjPaid_And_Mod_When_DCI_Is_The_Only_Active_Product()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(1))
            {
                ClaimActionPage _claimAction = null;

                var TestName = new StackFrame(true).GetMethod().Name;
                var claimSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestName,
                    "ClaimSequence", "Value");
                try
                {
                    _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch()
                        .SearchByClaimSequenceToNavigateToClaimActionPage(claimSeq);
                    _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                    _claimAction.UpdateProductTypeToInactiveByClientCodeFromDb(ClientEnum.SMTST.ToString());
                    _claimAction.Refresh();
                    _claimAction.DoesClaimLineContainAdjPaidandMod()
                        .ShouldBeFalse("Mod and Adj Paid should not be present");
                }
                finally
                {
                    _claimAction.UpdateProductTypeToActiveByClientCodeFromDb(ClientEnum.SMTST.ToString());
                    _claimAction.Refresh();
                }
            }
        }

        [Test, Category("OnDemand")] //CAR-2814(CAR-2738) + CAR-2894 (CAR-2852)
        [NonParallelizable]
        [Retrying(Times=3)]
        public void Verify_only_edit_flag_icon_is_enabled_for_CVP_PCA_clients_with_client_reviewed_Status()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(1))
            {
                ClaimActionPage _claimAction;
                ClientSearchPage _clientSearch = null;
                QaClaimSearchPage _qaClaimSearch;
                var _claimSeq =
                    automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, "ClassInit", "ClaimSequence",
                        "Value");
                //QuickLaunch.Logout().LoginAsHciAdminUser1();
                _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(_claimSeq);
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestName);
                var claSeq = paramLists["ClaimSequence"].Split(',').ToList();
                var clientUnreviewedClaimSeq = paramLists["ClientUnreviewedClaimSequence"];
                var currentUser = paramLists["UserName"];
                var flag = paramLists["Flag"].Split(',').ToList();
                var flagSeq = paramLists["FlagSeq"].Split(',').ToList();
                var reasonCode = paramLists["ReasonCode"];
                var note = paramLists["Note"];
                var lineNum = Convert.ToInt16(paramLists["LineNum"]);
                var reason = paramLists["Reason"];
                Random rnd = new Random();
                var processingTypes = new List<string>
                {
                    "B",
                    "C",
                    "PB",
                    "PR",
                    "R"
                };
                var processingType = processingTypes[rnd.Next(processingTypes.Count)];

                StringFormatter.PrintMessage("Switching client to RPE");
                _claimAction.ClickClaimSearchIcon().ClickOnSwitchClient().SwitchClientTo(ClientEnum.RPE, true);
                _claimAction.DeleteClaimAuditRecordFromDatabase(flagSeq[0], claSeq[0], lineNum.ToString(),
                    ClientEnum.RPE.ToString());
                _claimAction.DeleteClaimAuditRecordFromDatabase(flagSeq[1], claSeq[1], lineNum.ToString(),
                    ClientEnum.RPE.ToString());
                _claimAction.DeleteClaimAuditRecordFromDatabase(flagSeq[2], claSeq[2], lineNum.ToString(),
                    ClientEnum.RPE.ToString());
                _claimAction.UpdateAuditCompletedInClaimQaAuditFromDatabase(ClientEnum.RPE.ToString(), "F",
                    claSeq[1]);

                try
                {
                    _clientSearch = automatedBase.CurrentPage.NavigateToClientSearch();

                    StringFormatter.PrintMessage("Verification for claim with status Client Reviewed");
                    foreach (var claimSeq in claSeq)
                    {
                        _claimAction.GetCommonSql.UpdateProcessingTypeOfClient(
                            processingTypes[rnd.Next(processingTypes.Count)], ClientEnum.RPE.ToString());

                        //CAR-2894 (CAR-2852)
                        if (claimSeq.Equals(claSeq[2]))
                        {
                            _qaClaimSearch = _claimAction.NavigateToQaClaimSearch();
                            _qaClaimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                                "All QA Claims");
                            _qaClaimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Client",
                                ClientEnum.RPE.ToString());
                            _qaClaimSearch.GetSideBarPanelSearch.ClickOnFindButton();
                            _qaClaimSearch.WaitForWorking();
                            _claimAction = _qaClaimSearch.ClickOnClaimSequenceAndNavigateToNewClaimActionPage(claimSeq);
                            _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();

                        }

                        else
                        {
                            _claimAction.NavigateToClaimSearch()
                                .SearchByClaimSequenceToNavigateToClaimActionPage(claimSeq, true);
                        }

                        _claimAction.GetClaimStatus().ShouldBeEqual(
                            ClaimStatusTypeEnum.ClientReviewed.GetStringValue(),
                            "Status should be Client Reviewed");

                        if (processingType.Equals("B"))
                        {
                            _claimAction.IsIconEnabledByLineNoFlagNameAndIconName(lineNum, flag[0], "small_edit_icon")
                                .ShouldBeTrue("Is edit icon for non auto-approved flag enabled?");
                            _claimAction.IsIconEnabledByLineNoFlagNameAndIconName(lineNum, flag[0], "small_delete_icon")
                                .ShouldBeTrue("Is delete flag icon disabled?");
                            _claimAction.IsIconEnabledByLineNoFlagNameAndIconName(lineNum, flag[0], "small_switch_icon")
                                .ShouldBeTrue("Is switch flag icon enabled?");
                            _claimAction.ClickOnEditIconFlagLevelForLineEdit(lineNum, flag[0]);
                            _claimAction.IsReasonCodeEnabled().ShouldBeTrue("Is reason code enabled?");
                            _claimAction.IsEditFlagNoteTextAreaEnabled().ShouldBeTrue("Is text area enabled?");
                            _claimAction.IsEditFlagDeleteRestoreButtonDisabled()
                                .ShouldBeFalse("Is delete button disabled?");
                            _claimAction.IsSugPaidTextFieldEnabled().ShouldBeTrue("Is Sug Paid text area enabled?");
                            _claimAction.ClickOnCancelEditFlagLink();
                        }

                        else
                        {
                            _claimAction.ClosePageError();
                            _claimAction.IsIconEnabledByLineNoFlagNameAndIconName(lineNum, flag[0], "small_edit_icon")
                                .ShouldBeTrue("Is edit icon for non auto-approved flag enabled?");
                            _claimAction
                                .IsIconEnabledByLineNoFlagNameAndIconName(lineNum, flag[0], "small_delete_icon")
                                .ShouldBeFalse("Is delete flag icon disabled?");
                            _claimAction
                                .IsIconEnabledByLineNoFlagNameAndIconName(lineNum, flag[0], "small_switch_icon")
                                .ShouldBeFalse("Is switch flag icon enabled?");
                            _claimAction.ClickOnEditIconFlagLevelForLineEdit(lineNum, flag[0]);
                            _claimAction.IsReasonCodeEnabled().ShouldBeTrue("Is reason code enabled?");
                            _claimAction.IsEditFlagNoteTextAreaEnabled().ShouldBeTrue("Is text area enabled?");
                            _claimAction.IsEditFlagDeleteRestoreButtonDisabled()
                                .ShouldBeTrue("Is delete button disabled?");
                            _claimAction.IsEditFlagDeleteRestoreButtonDisabled(true)
                                .ShouldBeTrue("Is restore button disabled?");
                            _claimAction.IsSugUnitTextFieldEnabled().ShouldBeFalse("Is Sug Unit text area enabled?");
                            _claimAction.IsSugPaidTextFieldEnabled().ShouldBeFalse("Is Sug Paid text area enabled?");
                            _claimAction.IsSugCodeTextFieldEnabled().ShouldBeFalse("Is Sug Code text area enabled?");
                            _claimAction.SelectReasonCode(reasonCode);
                            _claimAction.SetNoteInEditFlagNoteField(note);
                            _claimAction.GetSideWindow.Save(waitForWorkingMessage: true);
                            _claimAction.ClickOnClaimFlagAuditHistoryIcon();
                            _claimAction.GetClaimFlagAuditHistoryValueByLabel(currentUser, "Reason Code")
                                .ShouldBeEqual(reasonCode, "User should be able to modify Reason Code");
                            _claimAction.WaitForStaticTime(200);
                            _claimAction.GetClaimFlagAuditHistoryValueByLabel(currentUser, "Notes")
                                .ShouldBeEqual(note, "User should be able to set note");
                            _claimAction.GetClaimFlagAuditHistoryValueByLabel(currentUser, "Action")
                                .ShouldBeEqual(reason, "Action Should be Reason");

                            StringFormatter.PrintMessage("For Auto-Approve flags all the icons should be disabled");

                            _claimAction.IsIconEnabledByLineNoFlagNameAndIconName(lineNum, flag[1], "small_edit_icon")
                                .ShouldBeFalse("Is edit icon for non auto-approved flag enabled?");
                            _claimAction
                                .IsIconEnabledByLineNoFlagNameAndIconName(lineNum, flag[1], "small_delete_icon")
                                .ShouldBeFalse("Is delete flag icon disabled?");
                            _claimAction
                                .IsIconEnabledByLineNoFlagNameAndIconName(lineNum, flag[1], "small_switch_icon")
                                .ShouldBeFalse("Is switch flag icon enabled?");
                            _claimAction.DeleteClaimAuditRecordFromDatabase(
                                claimSeq.Equals(claSeq[0]) ? flagSeq[0] :
                                claimSeq.Equals(claSeq[1]) ? flagSeq[1] : flagSeq[2], claimSeq, lineNum.ToString(),
                                ClientEnum.RPE.ToString());

                            StringFormatter.PrintMessage(
                                "Verification for claim with status other than Client Reviewed");
                            _claimAction.NavigateToClaimSearch()
                                .SearchByClaimSequenceToNavigateToClaimActionPage(clientUnreviewedClaimSeq, true);
                            _claimAction.GetClaimStatus().ShouldBeEqual(
                                StatusEnum.ClientUnreviewed.GetStringDisplayValue(),
                                $"Status should be {StatusEnum.ClientUnreviewed.GetStringValue()}");
                            _claimAction.IsIconEnabledByLineNoFlagNameAndIconName(lineNum, flag[2], "small_edit_icon")
                                .ShouldBeTrue("Is edit icon for non auto-approved flag enabled?");
                            _claimAction.IsIconEnabledByLineNoFlagNameAndIconName(lineNum, flag[2], "small_delete_icon")
                                .ShouldBeTrue("Is delete flag icon disabled?");
                            _claimAction.IsIconEnabledByLineNoFlagNameAndIconName(lineNum, flag[2], "small_switch_icon")
                                .ShouldBeTrue("Is switch flag icon enabled?");
                            _claimAction.ClickOnEditIconFlagLevelForLineEdit(lineNum, flag[2]);
                            _claimAction.IsReasonCodeEnabled().ShouldBeTrue("Is reason code enabled?");
                            _claimAction.IsEditFlagNoteTextAreaEnabled().ShouldBeTrue("Is text area enabled?");
                            _claimAction.IsEditFlagDeleteRestoreButtonDisabled()
                                .ShouldBeFalse("Is delete button disabled?");
                            _claimAction.IsSugPaidTextFieldEnabled().ShouldBeTrue("Is Sug Paid text area enabled?");
                        }
                    }
                }
                finally
                {
                    StringFormatter.PrintMessageTitle("Inside the Finally block of the test");

                    _claimAction.DeleteClaimAuditRecordFromDatabase(flagSeq[0], claSeq[0], lineNum.ToString(), ClientEnum.RPE.ToString());
                    _claimAction.DeleteClaimAuditRecordFromDatabase(flagSeq[1], claSeq[1], lineNum.ToString(), ClientEnum.RPE.ToString());
                    _claimAction.DeleteClaimAuditRecordFromDatabase(flagSeq[2], claSeq[2], lineNum.ToString(), ClientEnum.RPE.ToString());
                    _claimAction.UpdateAuditCompletedInClaimQaAuditFromDatabase(ClientEnum.RPE.ToString(), "F", claSeq[1]);
                    _claimAction.GetCommonSql.UpdateProcessingTypeOfClient(ProcessingType.R.ToString(), ClientEnum.RPE.ToString());
                    _claimAction.ClickOnSwitchClient().SwitchClientTo(ClientEnum.SMTST);
                }
            }
        }

        [Test, Category("OnDemand")] //TE-763
        [NonParallelizable]
        public void Verify_Visible_To_Client_Is_Selected_By_Default_For_Clients_With_DCI_Active()

        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(1))
            {
                ClaimActionPage _claimAction = null;



                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestName);
                string claimSeq = paramLists["ClaimSequence"];
                string flagName = paramLists["FlagName"];


                try
                {
                    StringFormatter.PrintMessage(
                        "Verify Visible To Client Is Selected By Default For Clients with DCA Active");
                    _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch()
                        .SearchByClaimSequenceToNavigateToClaimActionPage(claimSeq);
                    _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                    _claimAction.UpdateProductStatusForClient(ProductEnum.DCA.ToString(), ClientEnum.SMTST.ToString(),
                        true);
                    _claimAction.Refresh();

                    VerifyVisibleToClientSelectedByDefault(_claimAction, true, flagName);

                    StringFormatter.PrintMessage(
                        "Verify Visible To Client Is Not Selected By Default For Clients that do not have DCA Active");
                    _claimAction.UpdateProductStatusForClient(ProductEnum.DCA.ToString(), ClientEnum.SMTST.ToString());

                    _claimAction.Refresh();
                    _claimAction.WaitForWorkingAjaxMessage();
                    _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                    VerifyVisibleToClientSelectedByDefault(_claimAction, false, flagName);

                }
                finally
                {
                    _claimAction.UpdateProductStatusForClient(ProductEnum.DCA.ToString(), ClientEnum.SMTST.ToString(),
                        true);

                }

            }
        }

        [Test, Category("OnDemand")] //TE-769
        [NonParallelizable]
        public void Verify_Pended_Substatus_List_For_Clients_With_DCI_Active_Transfer_Form()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(1))
            {
                ClaimActionPage _claimAction;
                var _claimSeq =
                    automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, "ClassInit", "ClaimSequence", "Value");
                //QuickLaunch.Logout().LoginAsHciAdminUser1();
                _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(_claimSeq);
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestName);
                string claimSeq = paramLists["ClaimSequence"];
                string pendedStatus = paramLists["PendedStatus"];
                var statusListForDCIActiveFromDb =
                    _claimAction.GetAssociatedClaimSubStatusForInternalUser(ClientEnum.SMTST.ToString());
                var statusListForDCIInactiveFromDb =
                    _claimAction.GetAssociatedClaimSubStatusForInternalUserWithDCIInactive(ClientEnum.SMTST.ToString());
                _claimAction.UpdateProductStatusForClient(ProductEnum.DCA.ToString(), ClientEnum.SMTST.ToString(),
                    true);
                _claimAction.UpdateClaimStatus(claimSeq, ClientEnum.SMTST.ToString(), "U");
                try
                {
                    StringFormatter.PrintMessage(
                        "Verify Status List In Transfer Claim Form For Clients with DCA Active");
                    SearchByClaimSeqFromWorkList(_claimAction, claimSeq);
                    _claimAction.ClickOnTransferButton();
                    VerifyStatusListFromDatabase(_claimAction, statusListForDCIActiveFromDb);

                    StringFormatter.PrintMessage(
                        "Verify Status List In Transfer/Approve Claim Form For Clients with DCA Active");
                    _claimAction.ClickOnTransferApproveButton();
                    VerifyStatusListFromDatabase(_claimAction, statusListForDCIActiveFromDb);


                    StringFormatter.PrintMessage(
                        "Verify Status List In Transfer Claim Form For Clients with DCA InActive");
                    _claimAction.UpdateProductStatusForClient(ProductEnum.DCA.ToString(), ClientEnum.SMTST.ToString());
                    _claimAction.Refresh();
                    _claimAction.WaitForWorkingAjaxMessage();
                    _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                    _claimAction.ClickOnTransferButton();
                    VerifyStatusListFromDatabase(_claimAction, statusListForDCIInactiveFromDb);

                    StringFormatter.PrintMessage(
                        "Verify Status List In Transfer/Approve Claim Form For Clients with DCA InActive");
                    _claimAction.ClickOnTransferApproveButton();
                    VerifyStatusListFromDatabase(_claimAction, statusListForDCIInactiveFromDb);

                    StringFormatter.PrintMessage("Verify Status Is Present In Primary Data");
                    _claimAction.UpdateProductStatusForClient(ProductEnum.DCA.ToString(), ClientEnum.SMTST.ToString(),
                        true);
                    _claimAction.Refresh();
                    _claimAction.WaitForWorkingAjaxMessage();
                    _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                    _claimAction.ClickOnTransferButton();
                    _claimAction.SelectStatusCode(pendedStatus);
                    _claimAction.ClickOnSaveButton();
                    ReloadBrowserIfPageErrorPopupAppearsWhenTransferClaim(_claimAction, pendedStatus);
                    SearchByClaimSeqFromWorkList(_claimAction, claimSeq);
                    _claimAction.GetClaimStatus().ShouldBeEqual(pendedStatus, "Claim Status Should Match");
                    var claimSearch = _claimAction.ClickClaimSearchIcon();
                    claimSearch.GetGridViewSection.GetValueInGridByColRow(8)
                        .ShouldBeEqual(pendedStatus, "Status should be present in primary data.");


                }
                finally
                {
                    _claimAction.UpdateProductStatusForClient(ProductEnum.DCA.ToString(), ClientEnum.SMTST.ToString(),
                        true);
                    _claimAction.UpdateClaimStatus(claimSeq, ClientEnum.SMTST.ToString(), "U");
                }

            }

        }

        [Test, Category("OnDemand")] //CAR-3102(CAR-3021)
        [Retry(3)]
        [Author("Pujan Aryal")]
        public void Verify_For_PCA_And_CVP_Real_Time_Clients_DX_Codes_Shown_Have_Dx_Lvl_is_L()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(1))
            {
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                string claimSeq = paramLists["ClaimSequence"];
                string lineNo = paramLists["LineNo"];
                var dxCodes = paramLists["DxCodes"].Split(',').ToList();
                Random rnd = new Random();
                var processingTypes = new List<string>()
                {
                    "R",
                    "PR"
                };

                ClaimActionPage _claimAction;
                var _claimSeq =
                    automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, "ClassInit", "ClaimSequence",
                        "Value");
                _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(_claimSeq);
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();

                try
                {
                    automatedBase.CurrentPage.ClickOnSwitchClient().SwitchClientTo(ClientEnum.RPE, true);
                    _claimAction.GetCommonSql.UpdateProcessingTypeOfClient(
                        processingTypes[rnd.Next(processingTypes.Count)], ClientEnum.RPE.ToString());
                    _claimAction = automatedBase.CurrentPage.NavigateToClaimSearch()
                        .SearchByClaimSequenceToNavigateToClaimActionPage(claimSeq);
                    _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                    if (_claimAction.IsPageErrorPopupModalPresent())
                        _claimAction.ClosePageError();
                    _claimAction.ClickOnLineDetailsSection();
                    _claimAction.WaitForStaticTime(1000);
                    StringFormatter.PrintMessage("Verify Dx Code Is Visible Only When Dx_lvl value is L");
                    foreach (var dxcode in dxCodes)
                    {
                        string dxLvlValue = _claimAction.GetDxLvlValueFromDatabaseByDxCode(dxcode, claimSeq, lineNo);
                        if (dxLvlValue == "L")
                            _claimAction.IsDxCodePresentByLabel(dxcode)
                                .ShouldBeTrue("Dx Code with dx lvl value L should be shown");
                        else
                            _claimAction.IsDxCodePresentByLabel(dxcode)
                                .ShouldBeFalse("Dx Code with dx lvl value other than L should not be shown");
                    }
                }
                finally
                {
                    _claimAction.GetCommonSql.UpdateProcessingTypeOfClient(processingTypes[0],
                        ClientEnum.RPE.ToString());
                    _claimAction.ClickOnSwitchClient().SwitchClientTo(ClientEnum.SMTST, true);
                }
            }
        }

        [Test, Category("SchemaDependent")] //CAR-2535 (CAR-2639) //TE-912
        public void Verify_flag_audit_action_driver_for_sweep_action()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(1))
            {
                ClaimActionPage _claimAction;
                var _claimSeq =
                    automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, "ClassInit", "ClaimSequence", "Value");
                //QuickLaunch.Logout().LoginAsHciAdminUser1();
                _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(_claimSeq);
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
               var TestName = new StackFrame(true).GetMethod().Name;
                var paramList = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestName);
                var singleLineSweepFlagClaseq = paramList["SweepClaseq"].Split(',')[0];
                var multiLineSweepFlagClaseq = paramList["SweepClaseq"].Split(',')[1];
                try
                {
                    _claimAction.SwitchClientAndNavigateClaimAction(ClientEnum.CVTY.GetStringDisplayValue(),
                        _claimAction.GetClaimSequence().Split('-')[0],
                        paramList["claSeq"]);
                    _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                    if (_claimAction.IsPageErrorPopupModalPresent()) _claimAction.ClosePageError();
                    StringFormatter.PrintMessageTitle("Verifying the Type in Flag Audit History");
                    _claimAction.ClickOnClaimFlagAuditHistoryIcon();
                    _claimAction.GetActionTypeInAuditTrial()
                        .ShouldBeEqual("Sweep", "The action driver should display 'Sweep' instead of 'CVP Sweep'");
                    StringFormatter.PrintMessage(
                        "Verify that all flags and flag audits are displayed for internal user if all flags are deleted from sweep");

                    _claimAction.ClickWorkListIcon().ClickSearchIcon().SearchByClaimSequence(singleLineSweepFlagClaseq);
                    _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                    _claimAction.ClickOnClaimFlagAuditHistoryIcon();
                    _claimAction.GetActionTypeInAuditTrial()
                        .ShouldBeEqual("HCI Review Sweep", "The action driver should display 'HCI Review Sweep'");


                    StringFormatter.PrintMessage(
                        "Verify that all flags and flag audits are displayed for internal user if any flag is deleted from sweep");
                    _claimAction.ClickWorkListIcon().ClickSearchIcon().SearchByClaimSequence(multiLineSweepFlagClaseq);
                    _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                    _claimAction.ClickOnClaimFlagAuditHistoryIcon();
                    _claimAction.GetActionTypeInAuditTrial()
                        .ShouldBeEqual("Sweep", "flag with sweep audit should be displayed for internal user");

                    StringFormatter.PrintMessage(
                        "Verify that no flags are displayed for client user if all flags are deleted from sweep");
                    _claimAction.Logout().LoginAsClientUser().ClickOnSwitchClient()
                        .SwitchClientTo(ClientEnum.CVTY);
                    _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch()
                        .SearchByClaimSequenceToNavigateToClaimActionPage(singleLineSweepFlagClaseq);
                    _claimAction.IsEmptyFlagLineMessageAvailable().ShouldBeTrue("sweep flag displayed?");



                    StringFormatter.PrintMessage(
                        "Verify that no flags are displayed for client user if all flags are deleted from sweep");
                    _claimAction.ClickWorkListIcon().ClickSearchIcon().SearchByClaimSequence(multiLineSweepFlagClaseq);
                    _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                    _claimAction.IsFlagLinePresentByFlag(paramList["SweepFlag"])
                        .ShouldBeFalse("Flag deleted with sweep should not be displayed");

                }
                finally
                {
                    automatedBase.CurrentPage.ClickOnSwitchClient().SwitchClientTo(ClientEnum.SMTST, true);
                }

            }
        }


        //[Test, Category("NewClaimAction1")]//US10536
        //public void Verify_that_TIN_History_and_Patient_DX_radiobutton_option_is_present()
        //{
        //   var TestName = new StackFrame(true).GetMethod().Name;
        //    ClaimHistoryPage claimHistory = null;
        //    try
        //    {
        //        claimHistory = _claimAction.ClickOnHistoryButtonAndSwitchToProviderHistoryPopUp();
        //        claimHistory.ClickOnExtendedClaimHx();
        //        claimHistory.MouseOverTinHistory();
        //        claimHistory.IsTinHistoryRadioButtonPresentInExtendedHistoryPage().ShouldBeTrue("Tin History RadioButton Present?");
        //        claimHistory.MouseOverPatientDX();
        //        claimHistory.IsPatientDxRadioButtonPresentInExtendedHistoryPage().ShouldBeTrue("Patient Dx RadioButton Present?");
        //    }
        //    finally
        //    {
        //        if (claimHistory != null && !_claimAction.IsProviderHistoryClose())
        //        {
        //            claimHistory.SwitchToNewClaimActionPage(true);
        //        }

        //    }
        //}


        [Test] //CAR-2441 [CAR-2571]
        public void Verify_scanned_image_id_field_for_internal_user()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(1))
            {
                ClaimActionPage _claimAction;
                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestName);

                var claSeqWithImageId = paramLists["ClaimSeqWithImageId"];
                var expectedImageId = paramLists["ImageId"];
                var claSeqWithoutImageId = paramLists["ClaimSeqWithoutImageId"];
                var claimSeqForDCIInactiveClient = paramLists["ClaimSeqForDCIInactiveClient"];
                _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(claSeqWithImageId);
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
               

                try
                {
                    
                    StringFormatter.PrintMessageTitle("Verification of the Image ID field");
                    _claimAction.IsImageIdFieldPresent()
                        .ShouldBeTrue("The Image ID field should be present for the client for which DCA is active");
                    _claimAction.GetImageId().ShouldBeEqual(expectedImageId, "Correct Image ID should be displayed");

                    StringFormatter.PrintMessage(
                        "Verifying whether Image ID field is empty for claims that do not have an image id");
                    SearchByClaimSeqFromWorkList(_claimAction,claSeqWithoutImageId);
                    _claimAction.GetImageId()
                        .ShouldBeNullorEmpty(
                            "Image ID field should not show any value for claims which do not have an Image ID");

                    StringFormatter.PrintMessage(
                        "Verifying whether the Image ID field is not shown for DCA inactive clients");
                    _claimAction.SwitchClientAndNavigateClaimAction(ClientEnum.TTREE.ToString(),
                        claSeqWithoutImageId.Split('-')[0], claimSeqForDCIInactiveClient.Split('-')[0]);
                    _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                    _claimAction.IsImageIdFieldPresent()
                        .ShouldBeFalse("Image ID field should not be present for DCA Inactive clients");
                }
                finally
                {
                    if (_claimAction.IsPageErrorPopupModalPresent())
                        _claimAction.ClosePageError();
                    if (!automatedBase.CurrentPage.IsCurrentClientAsExpected(automatedBase.EnvironmentManager.TestClient))
                        CheckTestClientAndSwitch(automatedBase);
                }
            }
        }

        [Test] //CAR-2208 [CAR-2503]
        public void Verify_COB_approve_flags_for_internal_user()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(1))
            {
                ClaimActionPage _claimAction;
                var _claimSeq =
                    automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, "ClassInit", "ClaimSequence", "Value");
                //QuickLaunch.Logout().LoginAsHciAdminUser1();
                _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(_claimSeq);
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
               var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestName);

                var claimSeq = paramLists["ClaimSeq"];
                var cobFlags = paramLists["COB_Flags"].Split(',').ToList();
                var claimSeqForUnassignedFlag = paramLists["ClaimSeqForUnassignedFlag"];
                var unassignedProductFlag = paramLists["UnassignedProductFlag"];

                try
                {
                    _claimAction.UpdateStatusAndRestoreFlags(claimSeq, ClientEnum.SMTST.ToString());
                    _claimAction.UpdateClaimStatus(claimSeq, ClientEnum.SMTST.ToString(), "U");
                    _claimAction.GetCommonSql.UpdateHciDoneOrClientDoneToFalseFromDatabase(ClientEnum.SMTST.ToString(),
                        "hcidone", claimSeq, $"'{cobFlags[0]}', '{cobFlags[1]}'");

                    _claimAction.GetCommonSql.UpdateHciDoneOrClientDoneToFalseFromDatabase(ClientEnum.SMTST.ToString(),
                        "hcidone", claimSeqForUnassignedFlag, "'" + cobFlags[0] + "'");

                    //StringFormatter.PrintMessageTitle("Verifying user privileges and authorities are correct");
                    //_profileManager = CurrentPage.NavigateToProfileManager();
                    //_profileManager.ClickOnPrivileges();
                    //_profileManager.IsReadWriteAssigned(AuthoritiesEnum.COB.GetStringValue()).
                    //    ShouldBeTrue($"User should have Read/Write access for {AuthoritiesEnum.COB.GetStringValue()}");
                    //_profileManager.IsReadWriteAssigned(AuthoritiesEnum.ManageEdits.GetStringValue()).
                    //    ShouldBeTrue($"User should have Read/Write access for {AuthoritiesEnum.ManageEdits.GetStringValue()}");


                    StringFormatter.PrintMessage(
                        "Verification of COB RW and Manage Edits RW authority for internal user");

                    _claimAction.IsRoleAssigned<UserProfileSearchPage>(new List<string> {automatedBase.EnvironmentManager.Username},
                        RoleEnum.COBAuditor.GetStringValue()).ShouldBeTrue(
                        $"Is COB present for current user<{automatedBase.EnvironmentManager.Username}>");
                    automatedBase.CurrentPage.IsAvailableAssignedRowPresent(1, RoleEnum.ClaimsProcessor.GetStringValue())
                        .ShouldBeTrue(
                            $"Is Manage Edits present for current user<{automatedBase.EnvironmentManager.Username}>");

                    _claimAction
                        .IsRoleAssigned<UserProfileSearchPage>(
                            new List<string> {automatedBase.EnvironmentManager.HciUserWithReadOnlyManageCategory},
                            RoleEnum.COBAuditor.GetStringValue()).ShouldBeTrue(
                            $"Is COB present for current user<{automatedBase.EnvironmentManager.ClientUserName}>");

                    automatedBase.CurrentPage.IsAvailableAssignedRowPresent(1, RoleEnum.FCIAnalyst.GetStringValue()).ShouldBeFalse(
                        $"Is FCI present for current user<{automatedBase.EnvironmentManager.Username}>");

                    automatedBase.CurrentPage.IsAvailableAssignedRowPresent(1, RoleEnum.ClaimsProcessor.GetStringValue())
                        .ShouldBeTrue(
                            $"Is Manage Edits present for current user<{automatedBase.EnvironmentManager.Username}>");



                    //_newUserProfileSearch.SideBarPanelSearch.OpenSidebarPanel();
                    //_newUserProfileSearch.SearchUserByNameOrId(new List<string>{ automatedBase.EnvironmentManager.HciUserWithReadOnlyManageCategory }, true);
                    //_profileManager = _newUserProfileSearch.ClickonUserNameToNavigateProfilemanagereUsingUserId(automatedBase.EnvironmentManager.Instance
                    //    .HciUserWithReadOnlyManageCategory);



                    //_profileManager.ClickOnPrivileges();
                    //_profileManager.IsAuthorityAssigned(ProductEnum.FCI.ToString())
                    //    .ShouldBeFalse($"{ProductEnum.FCI.ToString()} is not assigned to {automatedBase.EnvironmentManager.HciUserWithReadOnlyManageCategory}");
                    //_profileManager.IsReadWriteAssigned(ProductEnum.COB.ToString())
                    //    .ShouldBeTrue($"{ProductEnum.COB.ToString()} is assigned to {automatedBase.EnvironmentManager.HciUserWithReadOnlyManageCategory} " +
                    //                  "with Read/Write authority");
                    //_profileManager.IsReadWriteAssigned(AuthoritiesEnum.ManageEdits.GetStringValue()).
                    //    ShouldBeTrue($"{automatedBase.EnvironmentManager.HciUserWithReadOnlyManageCategory} should have Read/Write access for {AuthoritiesEnum.ManageEdits.GetStringValue()}");

                    automatedBase.CurrentPage.NavigateToClaimSearch().SearchByClaimSequence(claimSeq);
                    _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();

                    StringFormatter.PrintMessageTitle("Verifying effects of approving a claim with COB flags");
                    _claimAction.GetClaimStatus().ShouldBeEqual(ClaimStatusTypeEnum.CotivitiUnreviewed.GetStringValue(),
                        $"The claim status should be {ClaimStatusTypeEnum.CotivitiUnreviewed.GetStringValue()} when at least one of the " +
                        "flags do not have HCIDONE = true");

                    _claimAction.ClickOnApproveButton();
                    _claimAction.SearchByClaimSequence(claimSeq);
                    _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();

                    StringFormatter.PrintMessage("Verifying whether COB flags are getting set to hcidone true");
                    _claimAction.GetHciDoneAndCliDoneValuesByClaSeqLinNoFlagName(ClientEnum.SMTST.ToString(), "hcidone",
                            claimSeq, 1, cobFlags[0])[0][0]
                        .ShouldBeEqual("T",
                            $"Hcidone should be set to 'T' after approving for the COB flag : {cobFlags[0]}");
                    _claimAction.GetHciDoneAndCliDoneValuesByClaSeqLinNoFlagName(ClientEnum.SMTST.ToString(), "hcidone",
                            claimSeq, 1, cobFlags[1])[0][0]
                        .ShouldBeEqual("T",
                            $"Hcidone should be set to 'T' after approving for the COB flag : {cobFlags[1]}");

                    StringFormatter.PrintMessage("Verifying whether claim status is getting updated correctly");
                    _claimAction.GetClaimStatus().ShouldBeEqual(ClaimStatusTypeEnum.CotivitiReviewed.GetStringValue(),
                        $"Claim status should be updated to {ClaimStatusTypeEnum.CotivitiReviewed.GetStringValue()} when all flags have hcidone = 'T'");

                    StringFormatter.PrintMessage(
                        "Verifying whether the product flag to which the user does not have access to is not approved");
                    _claimAction.Logout().LoginAsUserHavingManageCategoryReadOnlyAuthority();

                    automatedBase.CurrentPage.NavigateToClaimSearch().SearchByClaimSequence(claimSeqForUnassignedFlag);
                    _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();

                    var claimStatusBeforeApproval = _claimAction.GetClaimStatus();
                    claimStatusBeforeApproval.ShouldBeEqual(ClaimStatusTypeEnum.CotivitiUnreviewed.GetStringValue(),
                        $"Claim should be in '{ClaimStatusTypeEnum.CotivitiUnreviewed.GetStringValue()}' status when at least one flag is not reviewed");

                    _claimAction.ClickOnApproveButton();
                    _claimAction.SearchByClaimSequence(claimSeqForUnassignedFlag);
                    _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();

                    _claimAction.GetClaimStatus().ShouldBeEqual(claimStatusBeforeApproval,
                        $"Claim status should not change since {unassignedProductFlag} is not approved because it is not assigned to the user");

                    _claimAction.GetHciDoneAndCliDoneValuesByClaSeqLinNoFlagName(ClientEnum.SMTST.ToString(), "hcidone",
                            claimSeqForUnassignedFlag, 1, unassignedProductFlag)[0][0]
                        .ShouldBeEqual("F", "Product flags to which the user does not have access to is not approved");
                    _claimAction.GetHciDoneAndCliDoneValuesByClaSeqLinNoFlagName(ClientEnum.SMTST.ToString(), "hcidone",
                            claimSeqForUnassignedFlag, 1, cobFlags[0])[0][0]
                        .ShouldBeEqual("T",
                            "COB flag which is assigned to the user should be approved with hcidone set to 'T'");
                }

                finally
                {
                    StringFormatter.PrintMessageTitle("Revert hcidone status to 'F' for the flags");
                    _claimAction.UpdateStatusAndRestoreFlags(claimSeq, ClientEnum.SMTST.ToString());
                    _claimAction.GetCommonSql.UpdateHciDoneOrClientDoneToFalseFromDatabase(ClientEnum.SMTST.ToString(),
                        "hcidone", claimSeq, $"'{cobFlags[0]}', '{cobFlags[1]}'");

                    _claimAction.GetCommonSql.UpdateHciDoneOrClientDoneToFalseFromDatabase(ClientEnum.SMTST.ToString(),
                        "hcidone", claimSeqForUnassignedFlag, "'" + cobFlags[0] + "'");
                }
            }
        }

        [Test, Category("CommonTableDependent")] // CAR-2136(CAR-2454)
        public void Verify_COB_Product_Authorities()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(1))
            {
                ClaimActionPage _claimAction;
                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestName);
                var claimSeq = paramLists["ClaimSequence"];
                var flag = paramLists["Flag"];
                var lineNoFrom = paramLists["LineNumberFrom"];
                var lineNoTo = paramLists["LineNumberTo"];
                _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(claimSeq);
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                _claimAction.DeleteAllDeletedFlagsByClaSeq(claimSeq);

                try
                {
                    _claimAction.AddRoleForAuthorities(automatedBase.EnvironmentManager.Username,
                        RoleEnum.COBAuditor.GetStringValue());
                    _claimAction.UpdateRWRORoleForAuthorities(automatedBase.EnvironmentManager.Username,
                        RoleEnum.ClaimsReadOnly.GetStringValue(), RoleEnum.ClaimsProcessor.GetStringValue());

                    StringFormatter.PrintMessage(
                        "Verification of COB RW and Manage Edits RW authority for internal user");

                    _claimAction.IsRoleAssigned<UserProfileSearchPage>(
                        new List<string> {automatedBase.EnvironmentManager.Username},
                        RoleEnum.COBAuditor.GetStringValue()).ShouldBeTrue(
                        $"Is COB present for current user<{automatedBase.EnvironmentManager.Username}>");
                    automatedBase.CurrentPage
                        .IsAvailableAssignedRowPresent(1, RoleEnum.ClaimsProcessor.GetStringValue())
                        .ShouldBeTrue(
                            $"Is Manage Edits present for current user<{automatedBase.EnvironmentManager.Username}>");

                    _claimAction
                        .IsRoleAssigned<UserProfileSearchPage>(
                            new List<string>
                                {automatedBase.EnvironmentManager.HCIUserWithReadOnlyAccessToAllAuthorites},
                            RoleEnum.COBAuditor.GetStringValue(), isAssigned: false).ShouldBeTrue(
                            $"Is COB present for current user<{automatedBase.EnvironmentManager.HCIUserWithReadOnlyAccessToAllAuthorites}>");
                    automatedBase.CurrentPage.IsAvailableAssignedRowPresent(1, RoleEnum.ClaimsReadOnly.GetStringValue())
                        .ShouldBeTrue(
                            $"Is Manage Edits present for current user<{automatedBase.EnvironmentManager.HCIUserWithReadOnlyAccessToAllAuthorites}>");


                    _claimAction = automatedBase.CurrentPage.NavigateToClaimSearch()
                        .SearchByClaimSequenceToNavigateToClaimActionPage(claimSeq);
                    _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();

                    var lineNum = _claimAction.GetFlaggedLineDetails();

                    StringFormatter.PrintMessage(
                        "Verification of delete/restore/switch flag icon functionality for user with RW authority for both COB and Manage edits");
                    _claimAction.ClickOnDeleteIconOnFlagByLineNoAndRow(claimSeq);
                    _claimAction.IsFlaggedLineDeletedByLine(1, 1).ShouldBeTrue("Flagged Line Should Be Deleted", true);
                    _claimAction.ClickOnRestoreIconOnFlagByLineNoAndRow(claimSeq);
                    _claimAction.IsFlaggedLineDeletedByLine(1, 1)
                        .ShouldBeFalse("Flagged Line Should Be Restored", true);
                    if (lineNum == "2")
                    {
                        _claimAction.ClickOnSwitchIconByCurrentLineNoAndFlagAndTriggerLineNo(lineNoTo,
                            flag, lineNoFrom);
                        _claimAction
                            .IsDeletedSwitchIconPresentAfterFlagSwitchedByCurrentLineNoAndFlagAndTriggerLineNo(lineNoTo,
                                flag, lineNoFrom)
                            .ShouldBeTrue("Current Flag Should Deleted");
                        _claimAction
                            .IsSwitchIconPresentForFlagToBeSwitchByCurrentLineNoAndFlagAndTriggerLineNo(lineNoFrom,
                                flag,
                                lineNoTo)
                            .ShouldBeTrue("Switched Flag should present in Trigger Line");
                    }
                    else
                    {
                        _claimAction.ClickOnSwitchIconByCurrentLineNoAndFlagAndTriggerLineNo(lineNoFrom, flag,
                            lineNoTo);
                        _claimAction
                            .IsDeletedSwitchIconPresentAfterFlagSwitchedByCurrentLineNoAndFlagAndTriggerLineNo(
                                lineNoFrom,
                                flag, lineNoTo)
                            .ShouldBeTrue("Current Flag Should Deleted");

                        _claimAction
                            .IsSwitchIconPresentForFlagToBeSwitchByCurrentLineNoAndFlagAndTriggerLineNo(lineNoTo, flag,
                                lineNoFrom)
                            .ShouldBeTrue("Switched Flag should present in Trigger Line");

                    }

                    _claimAction.DeleteAllDeletedFlagsByClaSeq(claimSeq);

                    StringFormatter.PrintMessage("Setting COB to RO");
                    _claimAction.RemoveRoleForAuthorities(automatedBase.EnvironmentManager.Username,
                        RoleEnum.COBAuditor.GetStringValue());
                    _claimAction.WaitForStaticTime(5000);

                    StringFormatter.PrintMessage(
                        "Verification of delete/restore/switch flag icon functionality for user with COB RO and Manage edits RW authority");
                    _claimAction.RefreshPage(false);
                    _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                    _claimAction.IsDeleteIconDisabledByLinenoAndRow(1, 1).ShouldBeTrue("Is Delete icon disabled?");
                    _claimAction.IsSwitchIconDisabledByLineNumAndRowNum().ShouldBeTrue("Is Switch Icon disabled?");

                    StringFormatter.PrintMessage("Setting Manage edits to RO and COB RW");
                    _claimAction.UpdateRWRORoleForAuthorities(automatedBase.EnvironmentManager.Username,
                        RoleEnum.ClaimsProcessor.GetStringValue(), RoleEnum.ClaimsReadOnly.GetStringValue());
                    _claimAction.AddRoleForAuthorities(automatedBase.EnvironmentManager.Username,
                        RoleEnum.COBAuditor.GetStringValue());

                    StringFormatter.PrintMessage(
                        "Verification of delete/restore/switch flag icon functionality for user with COB RW and Manage edits RO authority");
                    _claimAction.RefreshPage(false);
                    _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                    _claimAction.IsDeleteIconDisabledByLinenoAndRow(1, 1).ShouldBeTrue("Is Delete icon disabled?");
                    _claimAction.IsSwitchIconDisabledByLineNumAndRowNum().ShouldBeTrue("Is Switch Icon disabled?");

                    StringFormatter.PrintMessage("Setting Manage edits to RW");
                    _claimAction.UpdateRWRORoleForAuthorities(automatedBase.EnvironmentManager.Username,
                        RoleEnum.ClaimsReadOnly.GetStringValue(), RoleEnum.ClaimsProcessor.GetStringValue());

                    StringFormatter.PrintMessage(
                        "Verification of delete/restore/switch flag icon functionality for user with RO authority for both COB and Manage edits");
                    _claimAction.Logout().LoginAsHCIUserWithReadOnlyAccessToAllAuthorities();
                    _claimAction = automatedBase.CurrentPage.NavigateToClaimSearch()
                        .SearchByClaimSequenceToNavigateToClaimActionPage(claimSeq);
                    _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                    _claimAction.IsDeleteIconDisabledByLinenoAndRow(1, 1).ShouldBeTrue("Is Delete icon disabled?");
                    _claimAction.IsSwitchIconDisabledByLineNumAndRowNum().ShouldBeTrue("Is Switch Icon disabled?");
                    _claimAction.ClickClaimSearchIcon();
                }
                finally
                {
                    _claimAction.AddRoleForAuthorities(automatedBase.EnvironmentManager.Username,
                        RoleEnum.COBAuditor.GetStringValue());
                    _claimAction.UpdateRWRORoleForAuthorities(automatedBase.EnvironmentManager.HciAdminUsername1,
                        RoleEnum.ClaimsReadOnly.GetStringValue(), RoleEnum.ClaimsProcessor.GetStringValue());

                    _claimAction.RemoveRoleForAuthorities(automatedBase.EnvironmentManager.HCIUserWithReadOnlyAccessToAllAuthorites,
                        RoleEnum.COBAuditor.GetStringValue());
                    _claimAction.UpdateRWRORoleForAuthorities(
                        automatedBase.EnvironmentManager.HCIUserWithReadOnlyAccessToAllAuthorites,
                        RoleEnum.ClaimsProcessor.GetStringValue(), RoleEnum.ClaimsReadOnly.GetStringValue());
                }

            }
        }

        [Test] //CAR-1884(CAR-2306)
        public void Verify_presence_of_sug_Modifier_in_flag_details()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(1))
            {
                ClaimActionPage _claimAction;
                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestName);
                var claimSeq = paramLists["ClaSeq"];
                var flag = paramLists["Flag"];
                

                _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(claimSeq);
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                var sugModifierList = _claimAction.GetSugModifiersByClaimSeqAndEditFlagFromDatabase(claimSeq, flag);

               StringFormatter.PrintMessage("Verification of 'Line Details' section when Flagged Line is selected");
                _claimAction.ClickOnFlaggedLinesLineByLineNumberAndRow(1, 1);
                _claimAction.GetLabelByDivNumAndRowAndCol(3, 1, 2).ShouldBeEqual("Sug Code:");
                _claimAction.GetLabelByDivNumAndRowAndCol(3, 1, 3).ShouldBeEqual("Sug Modifier:",
                    "Sug Modifier should be placed after Sug Code");
                _claimAction.GetLabelByDivNumAndPreceedingSiblingOfLineDetail(3, "Sug Code:")
                    .ShouldBeEqual("Sug Modifier:", "");
                _claimAction.GetLabelOfLineDetailsByDivNumRowAndCol(3, 1, 1)
                    .ShouldBeEqual(flag, $"Flag name should be {flag}");
                _claimAction.GetValueOfLineDetailsDataPointByDivNumAndTitle(3, "Sug Modifier").Split(',').ToList()
                    .ShouldCollectionBeEqual(sugModifierList,
                        "Sug modifier should be comma separated in order sug mod1, mod2, mod3, mod4");

                StringFormatter.PrintMessage("Verification when Flag is selected");
                _claimAction.ClickOnFlagLineByFlag(flag);
                _claimAction.GetFlagDetailLabelByPreceedingLabel("Sug Code:").ShouldBeEqual("Sug Modifier:",
                    "Sug Modifier should be placed after Sug Code");
                _claimAction.GetFlagDetailsValueFirstRow(4).Split(',').ToList().ShouldCollectionBeEqual(sugModifierList,
                    "Sug modifier should be comma separated in order sug mod1, mod2, mod3, mod4");
            }
        }

        [Test] // CAR-1110(CAR-846) + CAR-1145(CAR-1131)
        [Retrying(Times = 1)]
        public void Verify_add_flag_functionality()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(1))
            {
                ClaimActionPage _claimAction;
                ClaimProcessingHistoryPage _claimProcessingHistory;
                
                
               var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestName);
                var claimSequence = paramLists["ClaimSequence"];
                var flag = paramLists["Flag"].Split(',');
                var flagSource = paramLists["Source"];
                var triggerClaimSeq = paramLists["TriggerClaimSequence"];
                var claSeqWithAdjPaidZero = paramLists["ClaSeqWithAdjPaidZero"];
                const string sourceValue = "Manual/CL";
                const string action = "Line Edit";
                _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(claSeqWithAdjPaidZero);
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                try
                {
                    #region CAR-1145(CAR-1131)

                    StringFormatter.PrintMessageTitle(
                        "Verification of Claim lines with Adj Paid 0 should not be selected when 'Select All Lines' checkbox is selected");

                    var listOfClaimLinesWithAdjPaidZero =
                        _claimAction.GetClaimLineNoByLabelAndValueCssLocator("Adj Paid", "$0.00");

                    _claimAction.ClickAddIconButton();
                    _claimAction.ClickSelectAllLinesIcon();
                    var listOfSelectedClaimLineIndexValue = _claimAction.GetSelectedClaimLineIndexValue();

                    foreach (var claimLineWithAdjPaidZero in listOfClaimLinesWithAdjPaidZero)
                    {
                        listOfSelectedClaimLineIndexValue.ShouldNotContain(claimLineWithAdjPaidZero,
                            "Claim Lines with Adj Paid : '$0.00' should not be auto-selected when 'Select All Lines' checkbox is checked while adding the claim lines ");
                    }

                    #endregion CAR-1145(CAR-1131)

                    _claimAction.DeleteFlagLineFromDatabaseByClaimSeqAndFlag(claimSequence, flag[1]);
                    SearchByClaimSeqFromWorkList(_claimAction, claimSequence);
                    var listOfClaimLines = _claimAction.GetClaimLinesIndexValue();
                    _claimAction.ClickAddIconButton();
                    _claimAction.WaitForCondition(() => _claimAction.IsAddFlagPanelPresent());

                    _claimAction.IsFlagDropDownDisabledInAddFlag()
                        .ShouldBeTrue("The flag drop down is disabled when no claim line is selected");
                    StringFormatter.PrintMessageTitle("Verification of Select/DeSelect Claim Lines ");
                    _claimAction.SelectClaimLineToAddFlag();
                    _claimAction.GetSelectedClaimLineIndexValue()
                        .ShouldBeEqual(_claimAction.GetIndexOfSelectedLinesInFlaggedLine(),
                            "Selected Flagged Line Should be same as selected line in claim lines");
                    _claimAction.ClickOnSelectedClaimLinesOnSelectedLinesByRow(1);
                    _claimAction.GetIndexOfSelectedLinesInFlaggedLine()
                        .ShouldCollectionBeEmpty(
                            "There should not be any Claim Lines should be present at Selected Claim Lines");
                    StringFormatter.PrintMessageTitle(
                        "Verification of Input Field based on selection of single Claim lines");
                    _claimAction.IsFlagDropDownDisabledInAddFlag()
                        .ShouldBeTrue("Is Flag Drop Down disabled when not a single claim line selected?");
                    _claimAction.SelectClaimLineToAddFlag();

                    _claimAction.WaitForCondition(() => !_claimAction.IsFlagDropDownDisabledInAddFlag());

                    _claimAction.SelectFlagInAddFlag(flag[0]);
                    _claimAction.WaitForWorking();
                    _claimAction.IsSugUnitTextFieldEnabled().ShouldBeTrue("Sug Unit text field should be enabled");
                    _claimAction.IsSugPaidTextFieldEnabled().ShouldBeTrue("Sug Paid text field should be enabled");
                    _claimAction.SelectFlagInAddFlag(flag[1]);
                    _claimAction.WaitForWorking();
                    _claimAction.IsSugPaidTextFieldEnabled().ShouldBeTrue("Sug Paid text field should be enabled");
                    _claimAction.IsSugCodeTextFieldEnabled().ShouldBeTrue("Sug Code text field should be enabled");
                    StringFormatter.PrintMessageTitle(
                        "Verification of Input Field based on selection of multiple Claim lines");
                    _claimAction.IsFlagDropDownDisabledInAddFlag()
                        .ShouldBeFalse("The flag drop down is enabled when claim line is selected");
                    _claimAction.SelectClaimLineToAddFlag("2");
                    _claimAction.GetFlagVlaueAddValueSection().ShouldBeNullorEmpty("No flag should be selected");
                    _claimAction.IsSugUnitTextFieldEnabled().ShouldBeFalse("Sug Unit text field should be disabled");
                    _claimAction.IsSugPaidTextFieldEnabled().ShouldBeFalse("Sug Paid text field should be disabled");
                    _claimAction.IsSugCodeTextFieldEnabled().ShouldBeFalse("Sug Code text field should be disabled");
                    _claimAction.GetSelectedClaimLineIndexValue().ShouldCollectionBeEqual(
                        _claimAction.GetIndexOfSelectedLinesInFlaggedLine(), "The indices should be 1 and 2");
                    StringFormatter.PrintMessageTitle("Verification of checkbox of Select All ");
                    _claimAction.IsSelectAllLinesIconPresentInClaimLine()
                        .ShouldBeTrue("Select All Line icon should be present");
                    _claimAction.ClickSelectAllLinesIcon();
                    var listOfSelectedClaimLines = _claimAction.GetSelectedClaimLineIndexValue();
                    listOfSelectedClaimLines.ShouldCollectionBeEqual(
                        _claimAction.GetIndexOfSelectedLinesInFlaggedLine(),
                        "Is All Claim Lines selected when Select All Checkbox checked");
                    listOfSelectedClaimLines.ShouldCollectionBeEqual(listOfClaimLines,
                        "All claim Line Should be Selected");
                    _claimAction.ClickSelectAllLinesIcon();
                    _claimAction.GetIndexOfSelectedLinesInFlaggedLine()
                        .ShouldCollectionBeEmpty(
                            "There should not be any Claim Lines should be present at Selected Claim Lines when unchecked Select All Checkbox");
                    StringFormatter.PrintMessageTitle("Verification of Newly Added Flag on each claim lines");
                    _claimAction.ClickSelectAllLinesIcon();
                    _claimAction.SelectFlagInAddFlag(flag[1]);
                    _claimAction.WaitForWorking();
                    _claimAction.SelectFlagSourceInAddFlag(flagSource);
                    _claimAction.SelectTriggerClaimLineOnAddFlag(triggerClaimSeq);
                    _claimAction.ClickOnSaveEditButton();
                    var count = _claimAction.GetFlagLineManaulSourceCount(flag[1]);
                    for (var i = 1; i < count + 1; i++)
                    {
                        _claimAction.GetSourceOfFlaggedLine(i, flag[1])
                            .ShouldBeEqual(sourceValue, "Source value should be Manual");
                    }
                }
                finally
                {
                    _claimAction.DeleteFlagLineFromDatabaseByClaimSeqAndFlag(claimSequence, flag[1]);
                }
            }
        }

        [Test, Category("NewClaimAction1")] //US10549
        public void Verify_that_AltClaimNo_is_displayed_in_patient_claim_history()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(1))
            {
                ClaimActionPage _claimAction;
                ClaimProcessingHistoryPage _claimProcessingHistory;
                var _claimSeq =
                    automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, "ClassInit", "ClaimSequence", "Value");
                //QuickLaunch.Logout().LoginAsHciAdminUser1();
                _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(_claimSeq);
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
               var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestName);
                string claimSequence = paramLists["ClaimSequence"];
                SearchByClaimSeqFromWorkList(_claimAction,claimSequence);
                _claimAction.ClickPatientHistoryIcon();
                ClaimHistoryPage claimHistoryPage = null;
                try
                {
                    claimHistoryPage = _claimAction.SwitchToPatientClaimHistory();
                    claimHistoryPage.GetAltClaimNoOfClaimSequence(paramLists["ClaimSequence"]).ShouldBeEqual(
                        paramLists["AltClaimNo"], "Alt Claim No");
                }
                finally
                {
                    if (claimHistoryPage != null)
                        claimHistoryPage.SwitchToNewClaimActionPage(true);
                }
            }
        }

        [Test, Category("NewClaimAction1")]
        public void
            Verify_that_line_with_adj_paid_values_that_are_less_than_or_equal_to_zero_dollar_donot_appear_in_list_of_trigger_lines()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(1))
            {
                ClaimActionPage _claimAction;
               
               var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestName);
                string claimSequence = paramLists["ClaimSequence"];
                _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(claimSequence);
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                
                try
                {
                    _claimAction.ClickAddIconButton();
                    _claimAction.SelectClaimLineToAddFlag(paramLists["DenyLine"]);
                    _claimAction.SelectAddInFlagAdd("FUD");
                    _claimAction.GetAdjustedPaidFromTriggerClaimLineSection().All(x => x > 0).ShouldBeTrue(
                        "All Adjusted Paid appear in list of trigger lines are not less than or equal to zero.");
                }
                finally
                {
                    _claimAction.ClickOnFlagLevelCancelLink();
                }
            }

        }

        [Test, Category("NewClaimAction1")]
        public void Verify_that_user_selected_line_doesnot_appear_in_list_of_trigger_lines()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(1))
            {
                ClaimActionPage _claimAction;
               
                
               var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestName);
                string claimSequence = paramLists["ClaimSequence"];
                _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(claimSequence);
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                try
                {
                    _claimAction.ClickAddIconButton();
                    _claimAction.SelectClaimLineToAddFlag(paramLists["DenyLine"]);
                    _claimAction.SelectAddInFlagAdd("ADD");
                    foreach (string triggerClaimSequence in _claimAction.GetTriggerClaimSequenceList())
                    {
                        triggerClaimSequence.ShouldNotBeTheSameAs(claimSequence,
                            "Claim Sequence should not be same as Trigger Claim Sequence.");
                    }
                }
                finally
                {
                    _claimAction.ClickOnFlagLevelCancelLink();
                }
            }
        }

        [Test, Category("NewClaimAction1"), Category("IE10NonCompatible")]
        public void Verify_ICD_10_codes_appear_in_patient_claim_history()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(1))
            {
                ClaimActionPage _claimAction;
               
               var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestName);
                string claimSequence = paramLists["ClaimSequence"];
                _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(claimSequence);
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                ClaimHistoryPage patientHistory = null;
                try
                {
                    patientHistory = _claimAction.ClickOnHistoryButtonAndSwitchToPatientClaimHistoryPage();
                    patientHistory.MouseOverDxCode(paramLists["TableIndex"], paramLists["RowIndex"]);
                    patientHistory.GetValueOfDxCodeInPopup(paramLists["DxCodeIndex"])
                        .ShouldBeEqual(paramLists["DxCode"], "ICD 10 Codes", true);
                    //patientHistory.ClickOnExtendedClaimHx();
                    //patientHistory.MouseOverDxCodeOfExtendedClaimHx(paramLists["ExtendedHistoryRowIndex"]);
                    //patientHistory.GetValueOfDxCodeInPopupOfExtendedClaimHx(paramLists["DxCodeIndex"]).ShouldEqual(
                    //    paramLists["DxCodeOfExtendedClaimHx"], "ICD 10 Codes", true);
                }
                finally
                {
                    if (patientHistory != null)
                        patientHistory.SwitchToNewClaimActionPage(true).ClickWorkListIcon();
                }
            }
        }

        [Test, Category("NewClaimAction1")]
        public void Verify_Modifier_explanation_appears_in_Line_Details_and_Flag_Details()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(1))
            {
                ClaimActionPage _claimAction;
               
                //QuickLaunch.Logout().LoginAsHciAdminUser1();
               
               var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestName);
                string claimSequence = paramLists["ClaimSequence"];
                _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(claimSequence);
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                if (_claimAction.IsModifierExplanationValuePresentInFlaggedLinesOfFirstDiv())
                {
                    Console.WriteLine("Modifier Explanation is present in Flagged Lines.");
                    _claimAction.ClickOnLineDetailsSection();
                    _claimAction.IsModifierExplanationPresentInLinesDetails()
                        .ShouldBeTrue("Modifier Explanation is present in Line Details.", true);
                    Console.WriteLine("Modifier Explanation: " + _claimAction.GetModifierExplanationValue());
                }
                else
                {
                    Console.WriteLine("Modifier Explanation is not present in Flagged Lines.");
                    _claimAction.ClickOnLineDetailsSection();
                    _claimAction.IsModifierExplanationPresentInLinesDetails()
                        .ShouldBeFalse("Modifier Explanation is not present in Line Details.", true);
                }

                if (_claimAction.IsModifierExplanationValuePresentInFirstFlagRowOfFirstDiv())
                {
                    Console.WriteLine("Modifier Explanation is present in First Flag line.");
                    _claimAction.ClickOnFirstFlagLineOfFlaggedLinesDiv();
                    _claimAction.IsModifierExplanationPresentInFlagDetails()
                        .ShouldBeTrue("Modifier Explanation is present in Flag Details.", true);
                    Console.WriteLine(
                        "Modifier Explanation: " + _claimAction.GetModifierExplanationValueOfFlagDetails());
                }
                else
                {
                    Console.WriteLine("Modifier Explanation is not present in First Flag Line.");
                    _claimAction.ClickOnFirstFlagLineOfFlaggedLinesDiv();
                    _claimAction.IsModifierExplanationPresentInFlagDetails()
                        .ShouldBeFalse("Modifier Explanation is present in Flag Details.", true);
                }
            }
        }

        [Test, Category("NewClaimAction1")] //US31605
        public void
            Claims_with_system_deleted_flags_loads_with_flags_not_displayed_in_Flagged_Lines_container_by_default_flag_with_X_is_enabled_and_clicking_on_this_icon_displays_system_deleted_flags()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(1))
            {
                ClaimActionPage _claimAction;
                
                
               var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestName);
                string claimSeq = paramLists["ClaimSequence"];


                _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(claimSeq);
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                _claimAction.IsFlagWithXIsEnabled()
                    .ShouldBeTrue("Flag Symbol With X Is Displayed");
                _claimAction
                    .IsSystemDeletedFlagPresentInFlagLinesSection()
                    .ShouldBeFalse("System Deleted Flag Is Not Present");
                _claimAction.ClickSystemDeletedFlagIcon();
                _claimAction.IsSystemDeletedFlagPresentInFlagLinesSection()
                    .ShouldBeTrue("System Deleted Flag Is Present");
            }
        }


        [Test, Category("SmokeTest")]
        public void Verify_that_Provider_Claim_History_page_pops_up_when_page_is_loaded()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(1))
            {
                ClaimActionPage _claimAction = null;
              
                
               var TestName = new StackFrame(true).GetMethod().Name;
                try
                {
                    _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch()
                        .SearchByClaimSequenceToNavigateToClaimActionPage("1458925-0");
                    //_claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                    _claimAction.IsProviderHistoryPopUpPresent()
                        .ShouldBeTrue("Patient Claim History with Provider History is enabled.");
                }
                finally
                {
                    _claimAction.ClickWorkListIcon();
                }
            }
        }

        [Test, Category("NewClaimAction1")]
        public void Verify_that_when_begin_dos_is_different_from_end_dos_text_is_red_and_tooltip_appears()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(1))
            {
                ClaimActionPage _claimAction;
                
               
               var TestName = new StackFrame(true).GetMethod().Name;
                var claimSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestName,
                    "ClaimSequence", "Value");
                _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(claimSeq);
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                string tooltipValue, color;
                tooltipValue = _claimAction.GetDosTooltipValue();
                color = _claimAction.GetTextColorOfDosValue();
                var beginDos = _claimAction.GetDosValue();
                (!beginDos.Equals(tooltipValue)).ShouldBeTrue(string.Format("BeginDos: '{0}' Differ EndDos: '{1}'",
                    beginDos, tooltipValue));
                color.ShouldBeEqual(
                    string.Compare(automatedBase.EnvironmentManager.Browser, "IE", StringComparison.OrdinalIgnoreCase) == 0
                        ? "rgba(255,0,0,1)"
                        : "rgba(255, 0, 0, 1)", "Color Of DOS Text Is Red");
            }
        }

        [Test, Category("NewClaimAction1")]
        public void Verify_that_proper_comment_is_shown_when_no_dx_code_is_present()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(1))
            {
                ClaimActionPage _claimAction;
                
               var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestName);
                string claimSequence = paramLists["ClaimSequenceWithNoDxCodes"];
                _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(claimSequence);
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                _claimAction.GetNoDxCodePresentMesssage().ShouldBeEqual(
                    "There are currently no DX Codes available for this claim", "Dx code message");
            }
        }

        [Test, Category("NewClaimAction1")]
        public void
            Verify_that_when_claimAction_loads_the_default_view_in_the_lower_right_quadrant_is_the_claim_lines_view()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(1))
            {
                ClaimActionPage _claimAction;
                var _claimSeq =
                    automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, "ClassInit", "ClaimSequence", "Value");
                //QuickLaunch.Logout().LoginAsHciAdminUser1();
                _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(_claimSeq);
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
               var TestName = new StackFrame(true).GetMethod().Name;
                string labelValue = _claimAction.GetValueOfLowerRightQuadrant();
                labelValue.ShouldBeEqual("Claim Lines", "Label of Lower Right Quadrant");
            }
        }

        [Test, Category("NewClaimAction1")]
        public void Verify_the_delete_icon_is_disabled_for_user_not_having_manage_edit_authority()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(1))
            {
                ClaimActionPage _claimAction;
                
               
               var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestName);
                string claimSeq = paramLists["ClaimSequence"];
                _claimAction = automatedBase.QuickLaunch.Logout().LoginAsUserHavingNoManageEditAuthority().NavigateToClaimSearch()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(claimSeq);
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
               // HandleAutomaticallyOpenedPatientClaimHistoryPopupFor(_claimAction);
                _claimAction.IsEditFlagsIconEnabled().ShouldBeFalse("Delete Icon Enabled?");
            }
        }

        [Test, Category("SmokeTest")]
        public void Verify_that_entering_an_invalid_sug_code_gives_page_error_popup()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(1))
            {
                ClaimActionPage _claimAction;
                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestName);
                string flag = paramLists["Flag"];
                string invalidSugCode = paramLists["InvalidSugCode"];

                _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(paramLists["ClaimSequence"]);
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
              
               StringFormatter.PrintLineBreak();
                var modalPresent = false;
                try
                {
                    _claimAction.ClickOnEditForGivenFlag(flag);
                    _claimAction.EnterSugCode(invalidSugCode);
                    //_claimAction.ClickOnSugLabel();
                    _claimAction.WaitForWorkingAjaxMessage();
                    (modalPresent = _claimAction.IsPageErrorPopupModalPresent()).ShouldBeTrue(
                        "Page Error Modal popup present ?");
                    _claimAction.GetPageErrorMessage().ShouldBeEqual("Sug Code must be valid.", "Message Content");
                }
                finally
                {
                    if (modalPresent)
                        _claimAction.ClosePageError();
                    _claimAction.ClickOnCancelEditFlagLink();
                    StringFormatter.PrintLineBreak();
                }
            }
        }

        [Test, Category("SmokeTest"), Category("AppealDependent")]
        public void
            Verify_that_claim_with_appeal_is_locked_for_Cotiviti_user_lock_icon_and_tooltip_should_appear_indicating_claim_is_linked_to_an_appeal()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(1))
            {
                ClaimActionPage _claimAction;
                
              var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestName);

                _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(paramLists["ClaimSequenceWithAppeal"]);
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();

                _claimAction.IsClaimLocked().ShouldBeTrue("Claim Lock Icon Present?");
                string tooltip = _claimAction.GetLockIConTooltip();
                tooltip.ShouldBeEqual(
                    "Claim is locked. You cannot modify claims linked to an appeal. Adjustments must be made using Appeal Action.",
                    "ToolTip Message:");
            }
        }

        [Test, Category("NewClaimAction1"), Category("AppealDependent")]
        public void Verify_that_claim_with_appeal_where_appeal_status_is_complete_or_closed_has_Add_Appeal_Enabled()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(1))
            {
                ClaimActionPage _claimAction;
                
               
               var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestName);
                string claimSequenceWithAppeal = paramLists["ClaimSequenceWithAppealStatusClosedOrComplete"];
                _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(claimSequenceWithAppeal);
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
               _claimAction.IsAddAppealIconEnabled().ShouldBeTrue("Add Appeal Icon Enabled?");
            }
        }

        [Test, Category("NewClaimAction1"), Category("AppealDependent")]
        public void
            Verify_that_claim_with_appeal_where_appeal_status_is_NOT_complete_or_closed_has_Add_Appeal_Disabled()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(1))
            {
                ClaimActionPage _claimAction;
                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestName);
                string claimSequenceWithAppeal = paramLists["ClaimSequenceWithAppealStatusNotClosedOrComplete"];

                _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(claimSequenceWithAppeal);
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
               
               
                _claimAction.IsAddAppealIconDisabled().ShouldBeTrue("Add Appeal Icon Disabled?");
            }
        }

        [Test, Category("SmokeTest"), Category("AppealDependent")]
        public void
            Verify_that_for_locked_claim_with_appeal_edit_delete_restore_approve_approveornext_next_transfer_drop_down_and_addorview_documents_are_disabled()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(1))
            {
                ClaimActionPage _claimAction;
                string _claimSequenceWithAppeal = string.Empty;
                
               
               var TestName = new StackFrame(true).GetMethod().Name;
                _claimSequenceWithAppeal = string.IsNullOrEmpty(_claimSequenceWithAppeal)
                    ? automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, "ClaimSequenceWithAppeal", "ClaimSequence",
                        "Value")
                    : _claimSequenceWithAppeal;
                _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(_claimSequenceWithAppeal);
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                StringFormatter.PrintMessageTitle("Are Disabled");
                if (_claimAction.IsLocked())
                {
                    _claimAction.IsDisabled(ClaimActionPage.ActionItems.Edit).ShouldBeTrue("Edit is disabled");
                    _claimAction.IsDisabled(ClaimActionPage.ActionItems.DeleteOrRestore)
                        .ShouldBeTrue("Delete/Restore is disabled");
                    _claimAction.IsDisabled(ClaimActionPage.ActionItems.Approve).ShouldBeTrue("Approve is disabled");
                    _claimAction.IsDisabled(ClaimActionPage.ActionItems.AddViewDocuments)
                        .ShouldBeTrue("Add/View Documents is disabled");
                }
                else
                {
                    this.AssertFail("Claim is not locked");
                }
            }
        }

        [Test, Category("NewClaimAction1"), Category("AppealDependent")]
        public void Verify_clicking_on_Appeal_Sequence_opens_up_Appeal_Action()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(1))
            {
                ClaimActionPage _claimAction;
               
               
               var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestName);
                string claimSeq = paramLists["ClaimSequence"];
                string appealSeq = paramLists["AppealSequence"];
                _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(claimSeq);
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                _claimAction.ClickOnViewAppealIcon();
                AppealActionPage appealAction = null;
                try
                {
                    appealAction = _claimAction.ClickOnAppealSequence(appealSeq);
                    appealAction.GetPageHeader().ShouldBeEqual("Appeal Action", "Page Title");
                }
                finally
                {
                    if (appealAction != null)
                    {
                        _claimAction = appealAction.CloseAppealActionWindow();
                    }
                }
            }
        }



        [Test, Category("NewClaimAction1"), Category("SchemaDependent")] //TE-671

            public void Verify_clicking_on_Appeal_Sequence_opens_up_Appeal_Action_in_a_Separate_Tab_For_Dental_Appeal()
            {
                using (var automatedBase = new NewAutomatedBaseParallelRun(1))
                {
                    ClaimActionPage _claimAction;
                   
                   
               var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestName);
                var claimSeq = paramLists["ClaimSequence"];
                _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(claimSeq);
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();

                try
                {
                    StringFormatter.PrintMessage("verify that Appeal option in a pop up from claim action ");
                    
                    _claimAction.ClickOnViewAppealIcon();
                    _claimAction.GetAppealTypeByRow(2).ShouldBeEqual(AppealType.Appeal.GetStringDisplayValue(),
                        "correct appeal type displayed?");
                    AppealSummaryPage appealSummary =
                        _claimAction.ClickOnAppealSequenceToOpenAppealSummarypopupByAppealType(2);
                    appealSummary.GetPageHeader()
                        .ShouldBeEqual(PageTitleEnum.AppealSummary.GetStringDisplayValue(), "Page Title");
                    appealSummary.IsNucleusHeaderPresent().ShouldBeFalse("Nucleus Header should not be present");
                    appealSummary.IsWindowOpenedAsTab().ShouldBeFalse("Is appeal Summary opened in separate tab?");
                    var windowCount = appealSummary.GetWindowHandlesCount();


                    StringFormatter.PrintMessage("verify that Record Review appeal opens in pop up from claim action");
                    appealSummary.SwitchToClaimActionByUrl();
                    _claimAction.GetAppealTypeByRow(3).ShouldBeEqual(AppealType.RecordReview.GetStringDisplayValue(),
                        "correct appeal type displayed?");
                    appealSummary = _claimAction.ClickOnAppealSequenceToOpenAppealSummarypopupByAppealType(3);
                    appealSummary.GetPageHeader()
                        .ShouldBeEqual(PageTitleEnum.AppealSummary.GetStringDisplayValue(), "Page Title");
                    appealSummary.IsNucleusHeaderPresent().ShouldBeFalse("Nucleus Header should not be present");
                    appealSummary.IsWindowOpenedAsTab().ShouldBeFalse("Is appeal Summary opened in separate tab?");
                    appealSummary.GetWindowHandlesCount().ShouldBeEqual(windowCount, "appeal action reloaded");


                    StringFormatter.PrintMessage(
                        "verify that Dental Appeal Summary page should open in a new tab from claim action page");
                    appealSummary.SwitchToClaimActionByUrl();
                    _claimAction.GetAppealTypeByRow(4).ShouldBeEqual(AppealType.DentalReview.GetStringDisplayValue(),
                        "correct appeal type displayed?");
                    appealSummary = _claimAction.ClickOnAppealSequenceToOpenAppealSummarypopupByAppealType(4);
                    appealSummary.GetPageHeader()
                        .ShouldBeEqual(PageTitleEnum.AppealSummary.GetStringDisplayValue(), "Page Title");
                    appealSummary.IsNucleusHeaderPresent().ShouldBeTrue("Nucleus Header should be present");
                    appealSummary.GetWindowHandlesCount()
                        .ShouldBeEqual(windowCount + 1, "Windows count should be increased");
                    appealSummary.IsWindowOpenedAsTab().ShouldBeTrue("Is appeal Summary opened in separate tab?");

                    _claimAction = appealSummary.CloseAppealSummaryWindow();

                    StringFormatter.PrintMessage(
                        "verify that Dental Appeal Action page should open in a new tab from claim action page");
                    appealSummary.SwitchToClaimActionByUrl();
                    _claimAction.GetAppealTypeByRow().ShouldBeEqual(AppealType.DentalReview.GetStringDisplayValue(),
                        "correct appeal type displayed?");
                    AppealActionPage appealAction = _claimAction.ClickOnAppealSequenceByRow();
                    appealAction.HandleAutomaticallyOpenedActionPopup();
                    appealAction.GetPageHeader()
                        .ShouldBeEqual(PageTitleEnum.AppealAction.GetStringDisplayValue(), "Page Title");
                    appealAction.IsNucleusHeaderPresent().ShouldBeTrue("Nucleus Header should be present");
                    appealAction.GetWindowHandlesCount()
                        .ShouldBeEqual(windowCount + 1, "Windows count should be increased");
                    appealAction.IsWindowOpenedAsTab().ShouldBeTrue("Is appeal Action opened in separate tab?");

                }
                finally
                {
                    _claimAction.CloseAllExistingPopupIfExist();

                }
            }
        }

        [Test, Category("NewClaimAction1"), Category("AppealDependent")]
        public void Verify_user_can_click_on_Appeal_letter_if_appeal_is_closed_or_complete_to_open_up_appeal_letter()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(1))
            {
                ClaimActionPage _claimAction;
                
                
               var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestName);
                string claimSeq = paramLists["ClaimSequence"];
                string appealSeq = paramLists["AppealSequence"];
                _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(claimSeq);
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                _claimAction.ClickOnViewAppealIcon();
                AppealLetterPage appealLetter = null;
                try
                {
                    string status = _claimAction.GetAppealStatus(appealSeq);
                    Console.WriteLine(appealSeq + " status: " + status);
                    if (status.Equals("Complete") || status.Equals("Closed"))
                    {
                        appealLetter = _claimAction.ClickOnAppealLetter(appealSeq);
                        appealLetter.PageTitle.ShouldBeEqual(appealLetter.CurrentPageTitle, "Page Title");
                    }
                    else
                    {
                        _claimAction.AppealLetterIsPresent(appealSeq).ShouldBeFalse("Appeal Letter Link Is Present");
                    }
                }
                finally
                {
                    if (appealLetter != null)
                    {
                        appealLetter.CloseLetterPopUpPageAndSwitchToNewClaimActionPage();
                    }
                }
            }
        }

        [Test, Category("NewClaimAction1"), Category("AppealDependent")]
        public void
            Verify_that_if_an_Appeal_exists_on_a_claim_then_badge_appears_on_View_appeal_icon_indicating_number_of_Appeals_or_Records_reviews_associated_with_the_claim()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(1))
            {
                ClaimActionPage _claimAction;
               
               
               var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestName);
                string claimSeq = paramLists["ClaimSequence"];
                _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(claimSeq);
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
              
                int appealCountShownInButton = _claimAction.GetAppealCountShownInAppealButton();
                _claimAction.ClickOnViewAppealIcon()
                    .GetAppealSequenceCount().ShouldBeEqual(appealCountShownInButton, "Appeal Sequence Count", true);
            }
        }

        [Test, Category("NewClaimAction1"), Category("AppealDependent")]
        public void
            Verify_that_if_no_appeal_exists_then_View_Appeal_icon_appears_with_no_badge_clicking_on_icon_shows_appeal_not_present_message()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(1))
            {
                ClaimActionPage _claimAction;
                
               
               var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestName);
                string claimSeq = paramLists["ClaimSequenceWithNoAppeal"];

                _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(claimSeq);
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
               
                _claimAction.IsBadgeIconPresentInAppeal().ShouldBeFalse("View Appeal Icon present with badge ?");
                _claimAction.ClickOnViewAppealIcon().GetTopRightComponentTitle()
                    .ShouldBeEqual("Appeals", "Top Right component title");
                StringFormatter.PrintMessage(_claimAction.GetEmptyAppealSectionMessage());
            }
        }

        [Test, Category("NewClaimAction1")]
        public void
            Verify_that_if_there_are_no_flags_on_the_claim_then_there_are_currently_no_flagged_Lines_available_for_this_claim_message_is_shown()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(1))
            {
                ClaimActionPage _claimAction;
               
              
               var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestName);
                string claimSeq = paramLists["ClaimSequence"];

                _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(claimSeq);
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                _claimAction.IsEmptyFlagLineMessageAvailable()
                    .ShouldBeTrue("There are currently no flagged lines available for this claim");
            }
        }

        [Test, Category("NewClaimAction1")] //US37263
        public void Verify_that_when_click_on_next_blank_page_appear_after_claim_is_search_in_worklist_panel()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(1))
            {
                ClaimActionPage _claimAction;
                
                
               var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestName);
                var claimSeq = paramLists["ClaimSequence"];
                _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(claimSeq);
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                try
                {
                    _claimAction.ClickOnNextButton();
                    _claimAction.IsBlankPagePresent().ShouldBeTrue("Blank page present?");
                    _claimAction.IsWorkListControlDisplayed().ShouldBeTrue("Work List panel present");
                }
                finally
                {
                    _claimAction.SearchByClaimSequence(claimSeq);
                    _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                    _claimAction.ClickWorkListIcon();
                }

            }
        }



        [Test, Category("NewClaimAction1")] //us39867
        public void Verify_that_DX_Code_date_should_changed_when_user_moves_from_claim_to_claim()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(1))
            {
                ClaimActionPage _claimAction;
                var _claimSeq =
                    automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, "ClassInit", "ClaimSequence", "Value");
                //QuickLaunch.Logout().LoginAsHciAdminUser1();
                _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(_claimSeq);
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                var dxCodeDate = _claimAction.GetFirstOrignialDxDate();
                var preClaimSeq = _claimAction.GetClaimSequence();
                _claimAction = _claimAction.NavigateToCVClaimsWorkList();
                for (var i = 1; i <= 5; i++)
                {
                    _claimAction.ClickOnNextButton();
                    _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                    Console.WriteLine("compare claim Sequence between current <{0}> and previous <{1}>",
                        _claimAction.GetClaimSequence(), preClaimSeq);
                    var newDXCodeDate = _claimAction.GetFirstOrignialDxDate();
                    if (dxCodeDate == newDXCodeDate)
                    {
                        _claimAction.Refresh();
                        _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                        _claimAction.WaitForCondition(JavaScriptExecutor.ExecuteJqueryStatus);
                        _claimAction.Wait();
                        _claimAction.GetFirstOrignialDxDate().ShouldBeEqual(newDXCodeDate,
                            "DX Code Date should be same after reload the page");
                        break;
                    }

                    _claimAction.GetFirstOrignialDxDate()
                        .ShouldNotBeEqual(dxCodeDate, "DX Code Date should be different in next claim");
                    dxCodeDate = newDXCodeDate;
                    preClaimSeq = _claimAction.GetClaimSequence();
                    StringFormatter.PrintMessageTitle("Iteration <" + i + ">");
                }
            }
        }

        [Test, Category("NewClaimAction1")] //us39868
        public void
            Verify_that_next_button_should_disabled_and_worklist_panel_should_not_open_when_user_moves_from_claim_to_claim()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(1))
            {
                ClaimActionPage _claimAction;
                var TestName = new StackFrame(true).GetMethod().Name;
                var claimSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestName,
                    "ClaimSequence", "Value");
                var _claimSeq =
                    automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, "ClassInit", "ClaimSequence", "Value");
                //QuickLaunch.Logout().LoginAsHciAdminUser1();
                _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(_claimSeq);
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
              
                _claimAction = _claimAction.NavigateToCVClaimsWorkList();
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                _claimAction.WaitForCondition(() => !_claimAction.IsNextButtonDisabled());
                _claimAction.ClickOnNextButton();
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                _claimAction.WaitForCondition(() => !_claimAction.IsNextButtonDisabled());
                var nextButton = _claimAction.ClickOnNextButtonWithoutWaitAndIsNextButtonDisabled();
                var workListControl = _claimAction.IsWorkListControlDisplayed();
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                nextButton.ShouldBeTrue("Next Button is disabled");
                workListControl.ShouldBeFalse("Work List Section Should not open");
            }
        }

        [Test, Category("SmokeTest")]
        public void Verify_that_click_on_flag_and_verify_that_Flag_Details_appear_in_lower_right_quadrant()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(1))
            {
                ClaimActionPage _claimAction;
               
              
               var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestName);
                string claimSeq = paramLists["ClaimSequence"];
                _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(claimSeq);
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                _claimAction.IsFlagDetailDivDisplayed().ShouldBeFalse("Flag Details div displayed?");
                _claimAction.ClickOnFlagDetailsSection();
                _claimAction.IsFlagDetailDivDisplayed().ShouldBeTrue("Flag Details div displayed?");
                _claimAction.ClickOnLinesIcon();
            }
        }


        [Test, Category("NewClaimAction1"), Category("AppealDependent")] //US48460
        public void Verify_when_url_changes_to_access_different_claims_client_session_should_update()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(1))
            {
                ClaimActionPage _claimAction;
                var _claimSeq =
                    automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, "ClassInit", "ClaimSequence", "Value");
                //QuickLaunch.Logout().LoginAsHciAdminUser1();
                _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(_claimSeq);
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
               var TestName = new StackFrame(true).GetMethod().Name;
                var claimSeqSMTST = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestName,
                    "ClaimSequenceSMTST", "Value");
                var claimSeqPEHP = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestName,
                    "ClaimSequencePEHP", "Value");
                var appealSeqPEHP = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestName,
                    "AppealSequencePEHP", "Value");
                var flag = automatedBase.DataHelper
                    .GetSingleTestData(FullyQualifiedClassName, TestName, "Flag", "Value").Split(';')
                    .ToList();
                var procCode = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestName,
                    "ProcedureCode", "Value");
                SearchByClaimSeqFromWorkList(_claimAction,claimSeqSMTST);
                try
                {
                    _claimAction.SwitchClientAndNavigateClaimAction(ClientEnum.PEHP.ToString(),
                        claimSeqSMTST.Split('-')[0], claimSeqPEHP.Split('-')[0]);
                    _claimAction.WaitToLoadPatientClaimHistoryPopUp();
                    _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                    _claimAction.GetPageHeader()
                        .ShouldBeEqual("Bill Action", "Claim Action Should changed to Bill Action");
                    _claimAction.GetClaimSequence().ShouldBeEqual(claimSeqPEHP,
                        "Bill Sequence Should updated in order to verify Claim Information");
                    _claimAction.GetFlagListForClaimLine(1)
                        .ShouldCollectionBeEqual(flag, "Flag Should Equal in order to verify Flagged Lines");
                    _claimAction.GetProcCodeOnClaimLine()
                        .ShouldBeEqual(procCode, "Procedure Code Should Equal in order to verify Claim Lines Details");
                    _claimAction.ClickOnViewAppealIcon();
                    _claimAction.GetAppealSequence()
                        .ShouldBeEqual(appealSeqPEHP, "Appeal Sequence Should Equal in order to verify Appeal Section");
                }
                finally
                {
                    if (!automatedBase.CurrentPage.IsDefaultTestClientForEmberPage(ClientEnum.SMTST))
                    {
                        _claimAction.ClickOnSwitchClient()
                            .SwitchClientTo(ClientEnum.SMTST, true)
                            .NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(_claimSeq);
                        _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                    }
                }
            }
        }

        [Test] //CAR-206, CAR-627 + CAR-2434, CAR-2570
        public void Verify_Dental_data_points_alignment_and_allow_users_to_modify_dental_records()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(1))
            {
                ClaimActionPage _claimAction;
                ClaimHistoryPage _claimHistory;
             
               
               var TestName = new StackFrame(true).GetMethod().Name;
                var claimSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestName,
                    "ClaimSequence", "Value").Split(',').ToList();
                var title = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestName,
                    "Title", "Value").Split(',').ToList();
                var notes = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestName,
                    "Notes", "Value");
                var claimLineValues = automatedBase.DataHelper
                    .GetSingleTestData(FullyQualifiedClassName, TestName, "ClaimLineValues", "Value")
                    .Split(',').ToList();
                var claimProcessingHistoryDetails = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestName, "ClaimProcessingHistoryDetails", "Value").Split(':').ToList();
                var expectedDataPoints = automatedBase.DataHelper.GetMappingData(FullyQualifiedClassName, "Dental_data_points").Values
                    .ToList();
                const string invalidTn = "40";
                int inputfieldNumber = 0;
                var oldvalue = "";
                _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(claimSeq[0]);
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                _claimAction.DeleteClaimAuditOnly(claimSeq[1], "01-AUG-2019");
                try
                {
                    
                    _claimHistory = _claimAction.ClickOnHistoryButtonAndSwitchToPatientClaimHistoryPage();
                    _claimHistory.IsBackgroundColorDiff().ShouldBeTrue("Background color should be different");
                    _claimHistory.GetDentalDataPoints()
                        .ShouldCollectionBeEqual(expectedDataPoints, "Data points should be in order");
                    _claimHistory.IsDOSSorted()
                        .ShouldBeTrue(
                            "Claims should be shown in order of DOS, most recent claims should be at the top");
                    _claimAction = _claimHistory.SwitchToNewClaimActionPage(true);
                    SearchByClaimSeqFromWorkList(_claimAction,claimSeq[1]);
                    var listDataPointsFlagLineBeforeEdit = _claimAction.GetDentalDataList(title, "flagged_line");
                    var listDataPointsClaimLineBeforeEdit = _claimAction.GetDentalDataList(title, "claim_line");
                    _claimAction.ClickClaimLineDentalEditIcon();
                    _claimAction.IsEditClaimLineFieldPresent().ShouldBeTrue("Edit Dental Claim Line should present");
                    _claimAction.GetDentalInputDataList(title).ShouldCollectionBeEqual(
                        listDataPointsClaimLineBeforeEdit,
                        "Input Data should equal to display value");
                    _claimAction.InputEditDentalDataLength(title[0], invalidTn);
                    _claimAction.ClickSaveButton();
                    _claimAction.GetPageErrorMessage()
                        .ShouldBeEqual(string.Format("{0} is not a valid TN.", invalidTn));
                    _claimAction.ClosePageError();
                    _claimAction.ClickOnCancelLink();
                    _claimAction.GetDentalDataList(title, "flagged_line").ShouldCollectionBeEqual(
                        listDataPointsFlagLineBeforeEdit,
                        "No changes will be saved on cancelling edit in Flagged Lines");
                    _claimAction.GetDentalDataList(title, "claim_line").ShouldCollectionBeEqual(
                        listDataPointsClaimLineBeforeEdit,
                        "No changes will be saved on cancelling edit in Claim Lines");
                    var longDescOfTNFromDB =
                        _claimAction.GetLongDescOfToothNoFromDb(listDataPointsFlagLineBeforeEdit[0]);
                    var longDescOfOCFromDB =
                        _claimAction.GetLongDescOfOralCavityFromDb(listDataPointsFlagLineBeforeEdit[2]);
                    _claimAction.GetDentalDataToolTipText("flagged_line", title[0])
                        .ShouldBeEqual(longDescOfTNFromDB, "Tooltip values should match");
                    _claimAction.GetDentalDataToolTipText("flagged_line", title[2])
                        .ShouldBeEqual(longDescOfOCFromDB, "Tooltip values should match");
                    _claimAction.GetDentalDataToolTipText("claim_line", title[0])
                        .ShouldBeEqual(longDescOfTNFromDB, "Tooltip values should match");
                    _claimAction.GetDentalDataToolTipText("claim_line", title[2])
                        .ShouldBeEqual(longDescOfOCFromDB, "Tooltip values should match");
                    _claimAction.ClickClaimLineDentalEditIcon();
                    foreach (var t in title)
                    {
                        _claimAction.InputEditDentalDataLength(t, "");
                    }

                    _claimAction.ClickSaveButton();
                    _claimAction.IsEditClaimLineFieldPresent()
                        .ShouldBeFalse("Edit Dental Claim Line should not display");
                    foreach (var t in title)
                    {
                        _claimAction.GetDentalData("claim_line", t)
                            .ShouldBeNullorEmpty(string.Format("Empty Data can be saved for {0}", t));
                    }

                    _claimAction.ClickClaimLineDentalEditIcon();
                    _claimAction.InputEditDentalDataLength(title[0],
                            listDataPointsFlagLineBeforeEdit[0] == "05" ? "222" : "055")
                        .ShouldBeEqual(2, "User can type upto 2 alphanumeric value for TN");
                    _claimAction.InputEditDentalDataLength(title[1],
                            listDataPointsFlagLineBeforeEdit[1] == "ABCDE" ? "wXyZaB" : "AbCDeF")
                        .ShouldBeEqual(5, "User can type upto 5 alphanumeric value for TS");
                    _claimAction.InputEditDentalDataLength(title[2],
                            listDataPointsFlagLineBeforeEdit[2] == "UR" ? "UrL" : "UlR")
                        .ShouldBeEqual(2, "User can type upto 2 alphanumeric value for OC");
                    var editDentalRecordValues =
                        _claimAction.GetDentalInputDataList(title).Select(x => x.ToUpper()).ToList();
                    _claimAction.ClickSaveButton();
                    _claimAction.GetDentalDataList(title, "flagged_line").ShouldCollectionBeEqual(
                        editDentalRecordValues,
                        "Dental data should match with edited record");
                    _claimAction.GetDentalDataList(title, "claim_line").ShouldCollectionBeEqual(editDentalRecordValues,
                        "Dental data of claim line and flag line after edit should match");
                    _claimHistory = _claimAction.ClickOnHistoryButtonAndSwitchToPatientClaimHistoryPage();
                    editDentalRecordValues.ShouldCollectionBeEqual(
                        _claimHistory.GetDentalDataValuesFromPatHistoryPage(),
                        "The edited values should be reflected to Patient Claim History Page");
                    _claimHistory.GetDentalDataPointsValue().ShouldCollectionBeEqual(
                        _claimHistory.GetDataBaseValueFromDb(claimSeq[1].Split('-')[0]),
                        "Data points values should match");
                    _claimHistory.GetProcDesc(_claimHistory.GetValueInGridByCol(8), "DEN")
                        .ShouldBeEqual(_claimHistory.MouseOverAndGetToolTipString(10), "The descriptions shoud match");
                    _claimAction = _claimHistory.SwitchToNewClaimActionPage(true);
                    VerificationOfClaimProcessingHistoryDetails(_claimAction, claimProcessingHistoryDetails[0],
                        claimProcessingHistoryDetails[1], claimProcessingHistoryDetails[2],
                        claimProcessingHistoryDetails[3],
                        string.Format(claimProcessingHistoryDetails[4], editDentalRecordValues[0],
                            editDentalRecordValues[1], editDentalRecordValues[2]));
                    foreach (var t in title)
                    {
                        _claimAction.ClickClaimLineDentalEditIcon();
                        oldvalue = _claimAction.GetDentalData("claim_line", t);
                        _claimAction.InputEditDentalDataLength(t, claimLineValues[inputfieldNumber]);
                        _claimAction.ClickOnSaveButton();
                        VerificationOfClaimProcessingHistoryDetails(_claimAction, claimProcessingHistoryDetails[0],
                            claimProcessingHistoryDetails[1], claimProcessingHistoryDetails[2],
                            claimProcessingHistoryDetails[3],
                            string.Format(notes, t,
                                oldvalue, claimLineValues[inputfieldNumber]));
                        inputfieldNumber++;
                    }

                }
                finally
                {
                    _claimAction.CloseAnyPopupIfExist();
                }
            }
        }

        [Test, Category("NewClaimAction1")] // CAR-507
        public void Verify_addition_of_Dental_details_in_Flag_and_Claim_lines()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(1))
            {
                ClaimActionPage _claimAction;
             
               
                
               var TestName = new StackFrame(true).GetMethod().Name;
                var claimSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestName,
                    "ClaimSequence", "Value");
                var title = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestName,
                    "Title", "Value").Split(',').ToList();
                var toolTipValue = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestName,
                    "ToolTipValue", "Value").Split(',').ToList();
               
                var listDataPointsFlagLine = new List<string>();
                var listDataPointsClaimLine = new List<string>();
                _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(claimSeq);
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                _claimAction.DoesClaimLineContainAdjPaidandMod().ShouldBeTrue("Mod and Adj Paid should be present");
                var expectedDetalDataPoint = _claimAction.GetDentalDataPointsValuesFromDb(claimSeq);
                foreach (var t in title)
                {
                    _claimAction.IsDentalElementPresent("flagged_line", t)
                        .ShouldBeTrue("Is Dental Element Present at Flagged Line?");
                    _claimAction.IsDentalElementPresent("claim_line", t)
                        .ShouldBeTrue("Is Dental Element Present at Claim Line?");
                    listDataPointsFlagLine.Add(_claimAction.GetDentalData("flagged_line", t));
                    listDataPointsClaimLine.Add(_claimAction.GetDentalData("claim_line", t));
                }

                StringFormatter.PrintMessageTitle("Verify Dental Data in Flagged Lines");
                listDataPointsFlagLine.ShouldCollectionBeEqual(expectedDetalDataPoint, "Dental data should match");
                _claimAction.GetDentalDataToolTipText("flagged_line", title[0]).ShouldBeEqual(
                    _claimAction.GetLongDescOfToothNoFromDb(toolTipValue[0]), "Tooltip values should match");
                _claimAction.GetDentalDataToolTipText("flagged_line", title[2]).ShouldBeEqual(
                    _claimAction.GetLongDescOfOralCavityFromDb(toolTipValue[1]), "Tooltip values should match");
                _claimAction.IsDigitOfLengthTwo("flagged_line", title[0]).ShouldBeTrue("TN should be 2 digit value");
                StringFormatter.PrintMessageTitle("Verify Dental Data in Claim Lines");
                listDataPointsClaimLine.ShouldCollectionBeEqual(expectedDetalDataPoint, "Dental data should match");
                _claimAction.GetDentalDataToolTipText("claim_line", title[0]).ShouldBeEqual(
                    _claimAction.GetLongDescOfToothNoFromDb(toolTipValue[0]), "Tooltip values should match");
                _claimAction.GetDentalDataToolTipText("claim_line", title[2]).ShouldBeEqual(
                    _claimAction.GetLongDescOfOralCavityFromDb(toolTipValue[1]), "Tooltip values should match");
                _claimAction.IsDigitOfLengthTwo("claim_line", title[0]).ShouldBeTrue("TN should be 2 digit value");
            }
        }

        

        [Test] //CAR- 275(CAR-897)
        public void Verify_DCI_Claims_Worklist()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(1))
            {
                ClaimActionPage _claimAction;
                var _claimSeq =
                    automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, "ClassInit",
                        "ClaimSequence", "Value");
                var TestName = new StackFrame(true).GetMethod().Name;
                var expectedClaimWorkList = automatedBase.DataHelper
                    .GetMappingData(FullyQualifiedClassName, "claim_work_list").Values
                    .ToList();
                var expectedWorkListFilters = automatedBase.DataHelper
                    .GetMappingData(FullyQualifiedClassName, "Dci_worklist_filter_options").Values.ToList();
                var expectedClaimStatus =
                    automatedBase.DataHelper.GetMappingData(FullyQualifiedClassName, "Dci_claim_status").Values
                        .ToList();
                const string flag = "DCCI";
                const string batchId = "SCAC_HIST_21";
                //QuickLaunch.Logout().LoginAsHciAdminUser1();
                _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(_claimSeq);
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();

                try
                {
                    _claimAction.GetSecondarySubMenuOptionList(HeaderMenu.Claim, SubMenu.ClaimWorkList)
                        .ShouldCollectionBeEqual(expectedClaimWorkList,
                            "Is SubMenu DCA Under Claim Work List Equal?");
                    _claimAction.NavigateToDciClaimsWorkList();
                    _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                    _claimAction.GetClaimStatus()
                        .ShouldBeEqual(ClaimStatusTypeEnum.CotivitiUnreviewed.GetStringValue(),
                            "Status should be Cotiviti Unreviewed");
                    _claimAction.IsDCIProductFlagPresentInDCIWorklist()
                        .ShouldBeTrue("Dental Flag should be present");
                    _claimAction.ClickWorkListIcon();
                    _claimAction.GetSideBarPanelSearch.GetOptionListOnArrowDownIcon();
                    _claimAction.IsDCIWorkListPresentAtTheBottom()
                        .ShouldBeTrue("DCA worklist should be present at the bottom");
                    _claimAction.GetDciWorklistFilters().ShouldCollectionBeEqual(expectedWorkListFilters,
                        "Worklist filters should be Claim Status, Flag and Batch ID");
                    _claimAction.GetSideBarPanelSearch.ClickOnToggleIcon("Claim Status");
                    _claimAction.GetDciClaimStatus().ShouldCollectionBeEqual(expectedClaimStatus,
                        "Claim status should be Unreviewed and Pended");
                    _claimAction.GetSideBarPanelSearch.ClickOnToggleIcon("Claim Status");
                    _claimAction.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Flag", flag);
                    _claimAction.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Batch ID", batchId, false);
                    _claimAction.ClickOnCreateButton();
                    _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                    _claimAction.GetBatchIdInClaimDetails().ShouldBeEqual(batchId, "Batch IDs must match");
                    //_claimAction.GetFlagListForClaimLine(3)[0].ShouldBeEqual(flag, "Flags must match");
                    _claimAction.GetAvailableFlagList().ShouldContain(flag, "Flags must match");
                }
                finally
                {
                    if (!_claimAction.IsWorkListControlDisplayed())
                        _claimAction.ClickWorkListIcon();
                    _claimAction.ToggleWorkListToPci();
                    SearchByClaimSeqFromWorkList(_claimAction, _claimSeq);
                }
            }
        }

        [Test] //CAR- 275
        [NonParallelizable]
        public void Validate_Security_And_Navigation_Of_DCI_Claim_WorkList_Page()
        {
            
            using (var automatedBase = new NewAutomatedBaseParallelRun(1))
            {
                ClaimActionPage _claimAction;
                CommonValidations _commonValidation = new CommonValidations(automatedBase.CurrentPage);
                string _dciWorkListPrivilege = RoleEnum.DCIAnalyst.GetStringValue();


                _commonValidation.ValidateSecurityAndNavigationOfAPage(HeaderMenu.Claim,
				                new List<string> { SubMenu.ClaimWorkList,SubMenu.DciClaimsWorkList },
				                _dciWorkListPrivilege,
				                new List<string> { PageHeaderEnum.ClaimAction.GetStringValue() },
				                automatedBase.Login.LoginAsUserHavingNoAnyAuthority, new[] { "Test4", "Automation4" });

                
            }
        }

        [Test] //CAR-845(CAR-1340)
        public void Verify_ability_to_edit_a_flag_note()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(1))
            {
                ClaimActionPage _claimAction = null;
               
               var TestName = new StackFrame(true).GetMethod().Name;
                var claimSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestName,
                    "ClaimSequence", "Value");
                var username = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestName,
                    "UserName", "Value");
                var clientDisplay = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestName,
                    "ClientDisplay", "Value");
                var note = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestName, "Note",
                    "Value");
                string _clientDisplay, _note;
                try
                {
                    _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch()
                        .SearchByClaimSequenceToNavigateToClaimActionPage(claimSeq);
                    _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                    _claimAction.ClickOnClaimFlagAuditHistoryIcon();
                    _claimAction.IsFlagAuditHistoryEditNoteIconPresent("ui automation_3")
                        .ShouldBeFalse("Edit icon should not be present for the notes the users did not create.");
                    _claimAction.IsFlagAuditHistoryEditNoteIconPresent(username);
                    _clientDisplay = _claimAction.GetClaimFlagAuditHistoryValueByLabel(username, "Client Display");
                    _note = _claimAction.GetClaimFlagAuditHistoryValueByLabel(username, "Notes");
                    clientDisplay = _clientDisplay == "No" ? "Yes" : "No";
                    note = _note == "Test Note" ? "Test Note1" : "Test Note";
                    _claimAction.EditFlagNoteAndClientDisplay(clientDisplay, note, username, false);
                    _claimAction.GetClaimFlagAuditHistoryValueByLabel(username, "Client Display")
                        .ShouldBeEqual(_clientDisplay, "Clicking on Cancel will not save the changes.");
                    _claimAction.GetClaimFlagAuditHistoryValueByLabel(username, "Notes")
                        .ShouldBeEqual(_note, "Clicking on Cancel will not save the changes.");
                    _claimAction.EditFlagNoteAndClientDisplay(clientDisplay, note, username, true);
                    _claimAction.GetClaimFlagAuditHistoryValueByLabel(username, "Client Display")
                        .ShouldBeEqual(clientDisplay, "Clicking on Save will save the changes.");
                    _claimAction.GetClaimFlagAuditHistoryValueByLabel(username, "Notes")
                        .ShouldBeEqual(note, "Clicking on Save will save the changes.");
                }
                finally
                {
                    _claimAction.CloseAnyPopupIfExist();
                }
            }
        }




        [Test] //TE-643
        public void Verify_Original_Claim_Data_Pop_Up()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(1))
            {
                ClaimActionPage _claimAction;
                var _claimSeq =
                    automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, "ClassInit", "ClaimSequence", "Value");
                //QuickLaunch.Logout().LoginAsHciAdminUser1();
                _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(_claimSeq);
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                try
                {
                    _claimAction.GetSideBarPanelSearch.OpenSidebarPanel();
                    SearchByClaimSeqFromWorkList(_claimAction,_claimSeq);
                    _claimAction.ClickMoreOption();
                    var originalClaimDataPage = _claimAction.ClickOnOriginalClaimDataAndSwitch();
                    originalClaimDataPage.GetPageHeader()
                        .ShouldBeEqual("Original Claim Data", "Page Header Should Match");
                    originalClaimDataPage.GetClaimSequenceInHeader()
                        .ShouldBeEqual(_claimSeq, "Claim Sequence Should Match");
                    var (originalClaimDataFromDb, columnNames) =
                        originalClaimDataPage.GetOriginalClaimDataFromDatabase(_claimSeq);
                    originalClaimDataPage.GetColumnNames().ShouldBeEqual(columnNames, "Column Names Should Match");
                    int i = 1;
                    foreach (var row in originalClaimDataFromDb)
                    {
                        originalClaimDataPage.GetOriginalClaimDataValuesByRow(i)
                            .ShouldBeEqual(row, "Data Should Match");
                        i++;
                    }
                }
                finally
                {
                    _claimAction.CloseAllExistingPopupIfExist();
                }


            }
        }

        [Test] // TE-755
        public void Verify_Dental_Profiler_Icon_displayed()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(1))
            {
                ClaimActionPage _claimAction;
                var _claimSeq =
                    automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, "ClassInit", "ClaimSequence", "Value");
                //QuickLaunch.Logout().LoginAsHciAdminUser1();
                _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(_claimSeq);
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
               var TestName = new StackFrame(true).GetMethod().Name;
                var claimSeqWithIcon = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestName,
                    "ClaimSequence1", "Value");
                var claimSeqWithoutIcon = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestName,
                    "ClaimSequence2", "Value");

                try
                {


                    StringFormatter.PrintMessage(
                        "Verify Dental icon is displayed icon is  Displayed for provider where dental profile review is set to true");
                    _claimAction.GetSideBarPanelSearch.OpenSidebarPanel();
                    SearchByClaimSeqFromWorkList(_claimAction,claimSeqWithIcon);
                    ValidateDentalProfilerIcon(_claimAction,claimSeqWithIcon);

                    StringFormatter.PrintMessage(
                        "Verify Dental icon is not Displayed for provider where dental profile review is set to false");

                    _claimAction.GetSideBarPanelSearch.OpenSidebarPanel();
                    SearchByClaimSeqFromWorkList(_claimAction,claimSeqWithoutIcon.Split(',')[0]);
                    ValidateDentalProfilerIcon(_claimAction,claimSeqWithoutIcon.Split(',')[0]);

                    StringFormatter.PrintMessage(
                        "verify Dental icon should not be displayed for client with DCA product disabled");
                    if (_claimAction.GetProductStatusForClient(ProductEnum.DCA.ToString(), ClientEnum.TTREE.ToString()))
                        _claimAction.UpdateProductStatusForClient(ProductEnum.DCA.ToString(),
                            ClientEnum.TTREE.ToString());
                    _claimAction = automatedBase.CurrentPage.ClickOnSwitchClient().SwitchClientTo(ClientEnum.TTREE, true)
                        .NavigateToClaimSearch()
                        .SearchByClaimSequenceToNavigateToClaimActionPage(claimSeqWithoutIcon.Split(',')[1]);
                    ValidateDentalProfilerIcon(_claimAction,claimSeqWithoutIcon.Split(',')[1], false);
                }
                finally
                {
                    if (!automatedBase.CurrentPage.IsDefaultTestClientForEmberPage(ClientEnum.SMTST))
                    {
                        _claimAction.ClickOnSwitchClient()
                            .SwitchClientTo(ClientEnum.SMTST, true)
                            .NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(_claimSeq);
                        _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                    }

                }
            }
        }


        [Test] //CAR-3054(CAR-2966)
        public void Verify_Invoice_Data_In_claimAction_Page()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(1))
            {
                ClaimActionPage _claimAction;
                InvoiceDataPage _invoicedataPage;
                var _claimSeq =
                    automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, "ClassInit", "ClaimSequence", "Value");
                //QuickLaunch.Logout().LoginAsHciAdminUser1();
                _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(_claimSeq);
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
               var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestName);
                var claseq = paramLists["Claseq"];
                List<string> ProductList = new List<string> {"FAC", "DENT", "FIRST"};
                int i = 2, j = 0;

                if (!_claimAction.IsWorkListControlDisplayed())
                    _claimAction.ClickWorkListIcon();
                _claimAction.SearchByClaimSequence(claseq);
                _claimAction.WaitForWorking();
                _claimAction.CloseAllExistingPopupIfExist();
                _invoicedataPage = _claimAction.ClickOnInvoiceData();

                var generalInvoiceDataFromDb = _invoicedataPage.GetGeneralInvoiceDataFromDb(claseq);
                var invoiceHeaderLabel = _invoicedataPage.GetInvoiceheaderlabel();
                var invoiceLabels = _invoicedataPage.GetInvoiceLabel();
                var products = _invoicedataPage.GetInvoiceGridProducts();

                StringFormatter.PrintMessage("Verify Header label value displayed and correct? ");
                invoiceHeaderLabel.ShouldCollectionBeEqual(paramLists["InvoiceHeaderlabel"].Split(',').ToList(),
                    "Headers displayed correct?");

                _invoicedataPage.GetInvoiceheaderBylabel(invoiceHeaderLabel[0])
                    .ShouldBeEqual(claseq.Split('-')[0], $"{invoiceHeaderLabel[0]} correct?");
                _invoicedataPage.GetInvoiceheaderBylabel(invoiceHeaderLabel[1])
                    .ShouldBeEqual(generalInvoiceDataFromDb[0], $"{invoiceHeaderLabel[1]} value correct?");

                _invoicedataPage.GetGroupValue()
                    .ShouldBeEqual(generalInvoiceDataFromDb[1], "Group value displayed correct?");


                StringFormatter.PrintMessage(
                    "Verify general invoice content Label displayed and corresponding value correct");
                invoiceLabels.ShouldCollectionBeEqual(paramLists["InvoiceDataLabels"].Split(',').ToList(),
                    "Invoice Data labels Correct?");
                _invoicedataPage.GetInvoiceGridHeader()
                    .ShouldCollectionBeEqual(paramLists["GridHeader"].Split(',').ToList(),
                        "Headers of invoice grid correct?");

                foreach (var label in invoiceLabels)
                {
                    _invoicedataPage.GetInvoiceValueBylabel(label).ShouldBeEqual(generalInvoiceDataFromDb[i].Trim(),
                        $"correct value displayed for {label}?");
                    i++;
                }

                StringFormatter.PrintMessage("verify invoice grid data Specific correct ");

                products.ShouldCollectionBeEqual(paramLists["Product"].Split(',').ToList(),
                    "Products displayed correct?");
                foreach (var product in products)
                {
                    _invoicedataPage.GetProductSpecificInvoiceDataFromDb(ProductList[j], claseq)
                        .ShouldCollectionBeEqual(_invoicedataPage.GetInvoiceGridValueByProduct(product),
                            "product specific data correct?");
                    j++;
                }

                _claimAction = _invoicedataPage.CloseInvoiceDataPageAndBackToClaimActionPage();

            }
        }

        [Test] //   TE-768
        [NonParallelizable]
        public void Verify_Default_Reason_code_for_when_DCI_Flag_Is_Deleted()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(1))
            {
                ClaimActionPage _claimAction;
                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestName);
                var claimSeq = paramLists["ClaSeq"];
                var switchClaseq = paramLists["switchClaseq"];
                var userAction = paramLists["Action"].Split(',').ToList();
                var flag = paramLists["Flag"];
               
                _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(claimSeq);
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
              

                _claimAction.RestoreDeletedFlagsFromDB(claimSeq.Split('-')[0], switchClaseq.Split('-')[0]);
                _claimAction.DeleteLineFlagAuditByClaimSequence(claimSeq, ClientEnum.SMTST.ToString());
                _claimAction.DeleteLineFlagAuditByClaimSequence(switchClaseq, ClientEnum.SMTST.ToString());
                _claimAction.Refresh();
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();

                StringFormatter.PrintMessage("Verify default reason code when flag is deleted");
                var flagList = _claimAction.GetFlagListForClaimLine(1);
                _claimAction.ClickOnDeleteIconOnFlagByLineNoAndRow(claimSeq);
                VerifyReasonCodeForFLags(_claimAction,new List<string> {flagList[0]}, userAction[0]);
                _claimAction.ClickOnRestoreIconOnFlagByLineNoAndRow(claimSeq, row: 2);
                VerifyReasonCodeForFLags(_claimAction,new List<string> {flagList[0]}, userAction[1]);


                StringFormatter.PrintMessage("Verify default reason code when all flags on the line are deleted");
                _claimAction.ClickOnDeleteAllFlagsIconOnFlagLine();
                VerifyReasonCodeForFLags(_claimAction,flagList, userAction[0]);
                _claimAction.ClickOnRestoreIconOnFlaggedLinesRowsByRow(claimSeq);
                VerifyReasonCodeForFLags(_claimAction,flagList, userAction[1]);


                StringFormatter.PrintMessage("Verify reason code verified when all flags are deleted");
                _claimAction.ClickOnDeleteAllFlagsIcon();
                VerifyReasonCodeForFLags(_claimAction,flagList, userAction[0]);

                StringFormatter.PrintMessage("verify default reason code for switch flag ");
                SearchByClaimSeqFromWorkList(_claimAction,switchClaseq);
                _claimAction.ClickSwitchEditIconByFlag(flag);
                VerifyReasonCodeForFLags(_claimAction,new List<string> {flag}, userAction[0], "2");

            }
        }

        [Test] //TE-752
        public void
            Verify_red_badge_over_the_preauth_icon_represents_the_number_of_preauth_records_existing_for_patient()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(1))
            {
                ClaimActionPage _claimAction;
                var _claimSeq =
                    automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, "ClassInit", "ClaimSequence", "Value");
                //QuickLaunch.Logout().LoginAsHciAdminUser1();
                _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(_claimSeq);
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
               var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestName);
                _claimAction.UpdateProductStatusForClient(ProductEnum.DCA.ToString(), ClientEnum.SMTST.ToString(),
                    true);

                try
                {
                    StringFormatter.PrintMessage("Verify Pre-Auth icon is present for DCA Active Client");
                    SearchByClaimSeqFromWorkList(_claimAction,paramLists["ClaimSequence"]);
                    var preAuthCount = _claimAction.GetPreAuthCountFromDatabase(paramLists["PatSequence"]);
                    var claimPatientHistory = _claimAction.ClickOnHistoryButtonAndSwitchToPatientClaimHistoryPage();
                    claimPatientHistory.ClickOnPreAuthIconAndNavigateToPatientPreAuthHistory();
                    claimPatientHistory.IsPreAuthIconPresentInHeaderLevel().ShouldBeTrue("Is Pre Auth Icon Present ?");

                    StringFormatter.PrintMessage("Verify Red Badge Is Shown If Pre-Auth Record Is Present");
                    claimPatientHistory.IsRedBadgePresentOverPreAuthIcon()
                        .ShouldBeTrue("Is Red Badge Present Over Pre Auth Icon ?");
                    claimPatientHistory.GetPreAuthCountOnRedBadge()
                        .ShouldBeEqual(preAuthCount, "Pre Auth Count Should Match");

                    StringFormatter.PrintMessage("Verify Badge Is Not Shown If No Pre-Auths Exist");
                    claimPatientHistory.SwitchBackToPreAuthActionPage();
                    _claimAction.CloseAnyPopupIfExist();

                    SearchByClaimSeqFromWorkList(_claimAction,paramLists["ClaimSequenceWithoutPreAuth"]);
                    claimPatientHistory = _claimAction.ClickOnHistoryButtonAndSwitchToPatientClaimHistoryPage();
                    claimPatientHistory.ClickOnPreAuthIconAndNavigateToPatientPreAuthHistory();
                    claimPatientHistory.IsPreAuthIconPresentInHeaderLevel().ShouldBeTrue("Is Pre Auth Icon Present ?");
                    claimPatientHistory.IsRedBadgePresentOverPreAuthIcon()
                        .ShouldBeFalse("Is Red Badge Present Over Pre Auth Icon ?");

                    StringFormatter.PrintMessage("Verify Pre Auth Icon Is Not Present For DCA Inactive client");
                    _claimAction.UpdateProductStatusForClient(ProductEnum.DCA.ToString(), ClientEnum.SMTST.ToString(),
                        false);
                    claimPatientHistory.SwitchBackToPreAuthActionPage();
                    _claimAction.CloseAnyTabIfExist();
                    _claimAction.ClickOnHistoryButtonAndSwitchToPatientClaimHistoryPage();
                    claimPatientHistory.IsPreAuthIconPresentInHeaderLevel().ShouldBeFalse("Is Pre Auth Icon Present ?");
                    _claimAction.CloseAnyPopupIfExist();
                }
                finally
                {
                    _claimAction.UpdateProductStatusForClient(ProductEnum.DCA.ToString(), ClientEnum.SMTST.ToString(),
                        true);
                }

            }
        }

        [Test, Category("SchemaDependent")] //TE-916

        public void Verify_Appeals_Are_displayed_By_Default_For_DCI_Clients()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(1))
            {
                ClaimActionPage _claimAction;
                ClaimSearchPage _claimSearch;
                var _claimSeq =
                    automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, "ClassInit", "ClaimSequence", "Value");
                //QuickLaunch.Logout().LoginAsHciAdminUser1();
                _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(_claimSeq);
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
               var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestName);
                try
                {
                    StringFormatter.PrintMessage(
                        "Verify DX codes view are displayed by default when products other than DCA are enabled");
                    _claimAction.GetProductStatusForClient(ProductEnum.DCA.ToString(), ClientEnum.SMTST.ToString())
                        .ShouldBeTrue("DCA product active for SMTST client");
                    _claimAction.GetDefaultSecondaryView().ShouldBeEqual("Dx Codes", "DX codes displayed by default");

                    StringFormatter.PrintMessage(
                        "Verify Appeals are displayed by default when only DCA product is enabled");
                    _claimAction.EnableOnlyDCIForClient(ClientEnum.TTREE.ToString());

                    _claimSearch = _claimAction.ClickOnSwitchClient().SwitchClientTo(ClientEnum.TTREE, true)
                        .NavigateToClaimSearch();
                    _claimAction =
                        _claimSearch.SearchByClaimSequenceToNavigateToClaimActionPage(paramLists["ClaimSequence"]);
                    _claimAction.GetDefaultSecondaryView().ShouldBeEqual("Appeals",
                        "Appeals view displayed by default in top right quarter");

                    StringFormatter.PrintMessage(
                        "Verify when appeal icon is disabled for client,It has no affect on internal user");
                    _claimAction.UpdateHideAppeal(true, ClientEnum.TTREE.ToString());
                    _claimAction.Refresh();
                    _claimAction.GetDefaultSecondaryView().ShouldBeEqual("Appeals",
                        "Appeals view displayed by default in top right quarter");

                }
                finally
                {
                    _claimAction.RestoreProductForClients(ClientEnum.TTREE.ToString());
                    _claimAction.UpdateHideAppeal(false, ClientEnum.TTREE.ToString());
                }

            }
        }

        [Test] //CAR-2952 (CAR-2884)
        [Retrying(Times = 2)]
        public void Verify_reason_codes_for_internal_user()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(1))
            {
                ClaimActionPage _claimAction;
                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestName);
                string claimSeq = paramLists["ClaimSequence"];
                string linSeq = paramLists["LineSequence"];
                var userAction = paramLists["UserAction"].Split(',').ToList();
                var ffpFlagProductPairs =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestName, "FfpFlag");
                var cvFlagProductPairs =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestName, "CvFlag");
                var dciFlagProductPairs =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestName, "DciFlag");
                var fciFlagProductPairs =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestName, "FciFlag");
                var cobFlagPairs = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestName, "CobFlag");
                List<Dictionary<string, string>> flagProductList = new List<Dictionary<string, string>>()
                {
                    ffpFlagProductPairs,
                    cvFlagProductPairs,
                    dciFlagProductPairs,
                    fciFlagProductPairs,
                    cobFlagPairs
                };

                _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(claimSeq);
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
              
                _claimAction.RestoreParticularFlagsByClaimSequence(claimSeq);

                try
                {
                    //SearchByClaimSeqFromWorkListForClientUser(_claimAction,claimSeq);
                    StringFormatter.PrintMessage("Verify reason codes for Edit All Flags on the Claim option");
                    StringFormatter.PrintMessage("Verify reason codes for No Delete/Restore Action");
                    _claimAction.ClickOnEditAllFlagsIcon();
                    VerifyReasonCodesBasedOnAction(_claimAction,userAction[2]);
                    StringFormatter.PrintMessage("Verify reason codes for Delete Action");
                    _claimAction.ClickDeleteButtonOfEditAllFlagsSection();
                    VerifyReasonCodesBasedOnAction(_claimAction,userAction[0]);
                    StringFormatter.PrintMessage("Verify reason codes for Restore Action");
                    _claimAction.ClickRestoreButtonOfEditAllFlagsSection();
                    VerifyReasonCodesBasedOnAction(_claimAction, userAction[1]);
                    _claimAction.ClickOnCancelLink();

                    StringFormatter.PrintMessage("Verify reason codes for Edit All Flags on the Line Button");
                    StringFormatter.PrintMessage("Verify reason codes for No Delete/Restore Action");
                    _claimAction.ClickEditAllFlagsOnLineButton();
                    VerifyReasonCodesBasedOnAction(_claimAction, userAction[2], false);
                    StringFormatter.PrintMessage("Verify reason codes for Delete Action");
                    _claimAction.ClickDeleteButtonOfEditAllFlagsOnLineSection();
                    VerifyReasonCodesBasedOnAction(_claimAction, userAction[0], false);
                    StringFormatter.PrintMessage("Verify reason codes for Restore Action");
                    _claimAction.ClickRestoreButtonOfEditAllFlagsOnLineSection();
                    VerifyReasonCodesBasedOnAction(_claimAction, userAction[1], false);
                    _claimAction.ClickOnCancelLink();

                    StringFormatter.PrintMessage("Verify reason codes for particular flag of each product");
                    foreach (var flagProductPair in flagProductList)
                    {
                        StringFormatter.PrintMessage("Verify reason codes for No Delete/Restore Action");
                        _claimAction.ClickOnEditIconFlagLevelForLineEdit(2, flagProductPair.Values.FirstOrDefault());
                        VerifyReasonCodesBasedOnProductFlagAndAction(_claimAction,flagProductPair, userAction[2]);
                        StringFormatter.PrintMessage("Verify reason codes for Delete Action");
                        _claimAction.ClickOnDeleteIconFlagLevelForLineEdit("2", flagProductPair.Values.First());
                        _claimAction.ClickOnEditIconFlagLevelForLineEdit(2, flagProductPair.Values.FirstOrDefault());
                        _claimAction.ClickRestoreButtonOfEditAllFlagsOnLineSection();
                        VerifyReasonCodesBasedOnProductFlagAndAction(_claimAction,flagProductPair, userAction[1]);
                        StringFormatter.PrintMessage("Verify reason codes for Restore Action");
                        _claimAction.RestoreSpecificLineEditFlagByClient("2", flagProductPair.Values.First(), internalUser:true);
                        _claimAction.ClickOnEditIconFlagLevelForLineEdit(2, flagProductPair.Values.FirstOrDefault());
                        _claimAction.ClickDeleteButtonOfEditAllFlagsOnLineSection();
                        VerifyReasonCodesBasedOnProductFlagAndAction(_claimAction,flagProductPair, userAction[0]);
                        _claimAction.ClickOnCancelLink();
                    }

                }
                finally
                {
                    _claimAction.RestoreParticularFlagsByClaimSequence(claimSeq);
                }

            }
        }

        [Test] //CAR-3028 [CAR-2942] + CAR-3234(CAR-3194)
        [Retry(3)]
        [Author("PujanA + ShreyaS")]
        public void Verify_Dx_Code_Values_And_Descriptions_And_NDC_Value_In_Line_Details()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(1))
            {
                ClaimActionPage _claimAction;
                var _claimSeq =
                    automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, "ClassInit", "ClaimSequence", "Value");
                //QuickLaunch.Logout().LoginAsHciAdminUser1();
                _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(_claimSeq);
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
               var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestName);
                string claimSeq = paramLists["ClaimSequence"];
                string linNo = paramLists["LineNo"];

                var expectedDataFromDb = _claimAction.GetDxCodeValuesAndDescriptionsFromDb(claimSeq.Split('-')[0],
                    claimSeq.Split('-')[1], linNo);
                var expectedNDCValueFromDb = _claimAction.GetNDCValueByClaimSeqFromDb(claimSeq.Split('-')[0],
                    claimSeq.Split('-')[1]);

                SearchByClaimSeqFromWorkListForClientUser(_claimAction,claimSeq);
                _claimAction.WaitForStaticTime(1000);
                _claimAction.ClickOnLineDetailsSection();
                _claimAction.WaitForStaticTime(1000);
                _claimAction.ScrollToLastPosition();

                _claimAction.GetTotalDxCodesCountFromUI()
                    .ShouldBeEqual(expectedDataFromDb.Count, "Dx Count should match");

                StringFormatter.PrintMessage("Verifying the DX Code values and descriptions in line details");
                for (int i = 0; i < _claimAction.GetTotalDxCodesCountFromUI(); i++)
                {
                    _claimAction.GetDxCodeLabelByRow(i + 1)
                        .ShouldBeEqual(expectedDataFromDb[i][0], "Label Should Match");
                    _claimAction.GetDxCodeValuesByRowColumn(i + 1, 2)
                        .ShouldBeEqual(expectedDataFromDb[i][1], "Code Value Should Match");
                    _claimAction.GetDxCodeValuesByRowColumn(i + 1, 3)
                        .ShouldBeEqual(expectedDataFromDb[i][2], "Version Value Should Match");
                    _claimAction.GetDxCodeValuesByRowColumn(i + 1, 4)
                        .ShouldBeEqual(expectedDataFromDb[i][3], "Code Description Should Match");
                }

                #region CAR-3234(CAR-3194)
                StringFormatter.PrintMessage("Verification of NDC value");
                _claimAction.IsDataPointLabelOfLineDetailByLabelPresent("NDC:").ShouldBeTrue("Is NDC label present?");
                _claimAction.GetDataPointValueOfLineDetailsByLabel("NDC:")
                    .ShouldBeEqual(expectedNDCValueFromDb, "NDC values should match");
                #endregion

            }
        }

        [Test]//CAR-3103(CAR-3048)
        public  void Verify_FlagNotes_Icon_In_Claim_Action_Page()
        {
           

            using (var automatedBase = new NewAutomatedBaseParallelRun(1))
            {
                ClaimActionPage _claimAction;
                 var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestName);
                string claimSeq = paramLists["ClaimSequence"];
                List<string> flagNoteslabel = paramLists["NotesLabel"].Split(',').ToList();
                var _claimSeq =
                    automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, "ClassInit", "ClaimSequence", "Value");
                _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(_claimSeq);
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();

                try
                {
                    SearchByClaimSeqFromWorkListForClientUser(_claimAction, claimSeq);
                    _claimAction.IsClaimFlagNoteDisplayed().ShouldBeTrue("Claim Flag note icon displayed?");
                    _claimAction.ClickOnClaimFlagNotesIcon();
                    _claimAction.GetClaimFlagNotesIconTooltip().ShouldBeEqual("Claim Flag Notes", "tooltip correct?");
                    var flagDetails = _claimAction.GetClaimFlagNotesFromDb(claimSeq);
                    int j = 1;
                    foreach (var flag in flagDetails)
                    {
                        string editflag = flag[1];
                        _claimAction.GetFlagAndLineNoFromFlagNotesSection(j, 1).ShouldBeEqual(flag[0]);
                        _claimAction.GetFlagAndLineNoFromFlagNotesSection(j, 2).ShouldBeEqual(flag[1]);
                        if (flag[4] == "")
                            _claimAction.GetNoflagMessage(editflag)
                                .ShouldBeEqual("No flag notes available.", "No Flag message correct?");
                        else
                        {
                            int i = 2;
                            _claimAction.GetFlagdetailabel(editflag)
                                .ShouldCollectionBeEquivalent(flagNoteslabel, $"label correct for {editflag}");
                            foreach (var label in flagNoteslabel)
                            {
                                _claimAction.GetClaimFlagNoteDetailListByFlagAndLabel(editflag, label)
                                    .ShouldBeEqual(flag[i], $"{label} data correct?");
                                i++;
                            }
                        }

                        j++;
                    }

                    StringFormatter.PrintMessage("verify claim flag notes icon is not present for client with DCA Disabled");
                    automatedBase.QuickLaunch = _claimAction.SwitchClientTo(ClientEnum.RPE);
                    _claimAction = automatedBase.QuickLaunch.NavigateToCVClaimsWorkList();
                    _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                    _claimAction.IsClaimFlagNoteDisplayed().ShouldBeFalse("Claim flag notes should not be displayed");
                }
                finally
                {
                    _claimAction.CloseAnyPopupIfExist();
                    _claimAction.SwitchClientTo(ClientEnum.SMTST);
                }
            }
        }

        [Test,Category("Acceptance")]//CV-8766(CV-3355)
        [Author("ShreyaP")]
        public void Verify_Confidence_Score_In_FlagDetails()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(1))
            {
                ClaimActionPage _claimAction;
                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestName);
                string _claimSeq = paramLists["claseq"];
               
                _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(_claimSeq);
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                string probabilityScore ;
                string score;

                try
                {                    
                    var flags = _claimAction.GetAllFlagsFromWorklist();
                    foreach (var flag in flags)
                    {
                        _claimAction.ClickOnFlagLineByFlag(flag);
                        probabilityScore = _claimAction.GetFlagDetailsByLabel("Probability Score");
                        score = (_claimAction.GetConfidenceScoreForFlage(_claimSeq,flag));
                        calculatePredictionPercentage(score).ShouldBeEqual(probabilityScore,
                                $"probability score for {flag} correct?");                       
                        
                    }                 
                
                }
                finally
                {
                    _claimAction.CloseAnyPopupIfExist();
                    
                }
            }
        }

        #endregion

        #region PRIVATE METHODS

        public string calculatePredictionPercentage(string score)
        {
            if(score.Equals(""))
                return String.Empty;
            else
            {
                float actualScore = Convert.ToSingle(score);
                var confidencePercentage = actualScore > 0.5 ? actualScore * 100 : (1 - actualScore) * 100;
                string probabilityprediction = actualScore > 0.5 ? "D" : "A";
                return probabilityprediction + " - " + confidencePercentage+"%";
            }
            

        }
        void SearchByClaimSeqFromWorkListForClientUser(ClaimActionPage _claimAction,string claimSeq)
        {
            _claimAction.WaitForIe(3000);
            if (_claimAction.IsPageHeaderPresent())
            {
                if (!_claimAction.IsWorkListControlDisplayed())
                    _claimAction.ClickWorkListIcon();
            }
            _claimAction.WaitForStaticTime(2000);
            _claimAction.ClickSearchIcon()
                .SearchByClaimSequence(claimSeq);
            //_claimAction.ClickWorkListIcon();s
            _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
        }

        private void VerifyReasonCodesBasedOnAction(ClaimActionPage _claimAction,string action, bool claimlineoption = true)
        {
            List<string> expectedReasonCodes;
            if (claimlineoption)
                expectedReasonCodes = _claimAction.GetExpectedReasonCodesForAllFlagsOnClaimOption(action);
            else
                expectedReasonCodes = _claimAction.GetExpectedReasonCodesForAllFlagsOnTheLine(action);

            expectedReasonCodes.Insert(0, "");
            _claimAction.GetEditReasonCodeOptions()
                .ShouldCollectionBeEqual(expectedReasonCodes, "Edit Reason Code Options should match");

        }

        private void VerifyReasonCodesBasedOnProductFlagAndAction(ClaimActionPage _claimAction,Dictionary<string, string> flagProductPair, string action)
        {
            StringFormatter.PrintMessage($"Verify Reason Codes for {flagProductPair.Values.FirstOrDefault()} flag of {flagProductPair.Keys.FirstOrDefault()} product");
            List<string> expectedReasonCodesForAParticularFlag = _claimAction.GetExpectedReasonCodesForAParticularFlag(flagProductPair.Keys.FirstOrDefault(), action);
            expectedReasonCodesForAParticularFlag.Insert(0, "");
            _claimAction.GetEditReasonCodeOptions()
                .ShouldCollectionBeEqual(expectedReasonCodesForAParticularFlag, "Edit Reason Code Options");

        }

        void VerifyStatusListFromDatabase(ClaimActionPage _claimAction,List<string> statusListFromDb)
        {
            var statusList = _claimAction.GetStatusListFromDropdown();
            statusList.RemoveAt(0);
            statusList.ShouldCollectionBeEqual(statusListFromDb, "Status List Should Match");
        }

        void VerifyVisibleToClientSelectedByDefault(ClaimActionPage _claimAction,bool selected,string flagName)
        {
            _claimAction.ClickOnEditAllFlagsIcon();
            if (selected)
                _claimAction.IsVisibleToclientSelectedInAllFlagsSection().ShouldBeTrue("Visible To Client Should Be Selected in  Claim Level Edit formBy Default");
            else
                _claimAction.IsVisibleToclientSelectedInAllFlagsSection().ShouldBeFalse("Visible To Client Not Should Be Selected in Claim Level Edit form By Default");
            
            _claimAction.ClickEditAllFlagsOnLineButton();
            if (selected)
                _claimAction.IsVisibleToclientSelectedInEditAllFlagLineSection().ShouldBeTrue("Visible To Client Should Be Selected in Line Level Edit form By Default");
            else
                _claimAction.IsVisibleToclientSelectedInEditAllFlagLineSection().ShouldBeFalse("Visible To Client Not Should Be Selected in Line Level Edit form By Default");
            
            _claimAction.ClickOnEditIconFlagLevelForLineEdit(1, flagName);
            if (selected)
                _claimAction.IsVisibleToclientSelectedInFlagLineSection().ShouldBeTrue("Visible To Client Should Be Selected in Flag Level Edit form By Default");
            else
                _claimAction.IsVisibleToclientSelectedInFlagLineSection().ShouldBeFalse("Visible To Client Not Should Be Selected in Flag Level Edit form By Default");
            _claimAction.ClickOnCancelLink();
        }


        private void VerifyReasonCodeForFLags(ClaimActionPage _claimAction,List<string> FLaggedLines, string action, string linNo = "1")
        {
            _claimAction.ClickOnClaimFlagAuditHistoryIcon();
            foreach (string flag in FLaggedLines)
            {
                var reasoncode = _claimAction.GetClaimFlagAuditHistoryDetailByFlagAndRowAndLabelAndLineNo(flag, "Reason Code", linNo);
                _claimAction.GetClaimFlagAuditHistoryDetailByFlagAndRowAndLabelAndLineNo(flag, "Action", linNo)
                    .ShouldBeEqual(action);
                if (_claimAction.IsFlagDental(flag))
                {
                    reasoncode.ShouldBeEqual("DEN 9 - Review By Analyst");

                }
                else
                {
                    reasoncode.ShouldBeEqual("COD 1 - Clinical Judgment");
                }


            }

        }


        private void ValidateDentalProfilerIcon(ClaimActionPage _claimAction,string claseq,bool dci_active=true)
        {
            
            if (dci_active && _claimAction.GetDentalProfilerReview(claseq.Split('-')[0], claseq.Split('-')[1]).Equals("T"))
            {
                _claimAction.IsDentalProfilerIconPresent().ShouldBeTrue("Dental Profiler Icon present?");
                _claimAction.GetTooltipMessageForDentalIcon().ShouldBeEqual("Provider Specific Review","Is Tooltip correct?");

            }
            else

            _claimAction.IsDentalProfilerIconPresent().ShouldBeFalse("Dental Profiler Icon present?");


        }

        private void VerificationOfClaimProcessingHistoryDetails(ClaimActionPage newClaimAction, string action,string analyst,string userType, string status, string notes)
        {
            var claimProcessingHistory = newClaimAction.ClickOnClaimProcessingHistoryAndSwitch();
            var dateTime = claimProcessingHistory.GetClaimHistoryTableDataValueByRowCol(1, 1);
            Convert.ToDateTime(dateTime).Date.ShouldBeEqual(DateTime.Now.Date, "Is Reviewed Date equal?");
            claimProcessingHistory.GetClaimHistoryTableDataValueByRowCol(1, 2)
                .ShouldBeEqual(action, "Action should be Line Edit");
            claimProcessingHistory.GetClaimHistoryTableDataValueByRowCol(1, 4)
                .ShouldBeEqual(analyst, "Verification of QA Reviewer should be in proper format.");
            claimProcessingHistory.GetClaimHistoryTableDataValueByRowCol(1, 5)
                .ShouldBeEqual(userType, "User Type should match.");
            claimProcessingHistory.GetClaimHistoryTableDataValueByRowCol(1, 6)
                .ShouldBeEqual(status, "Status should equal to current claim status.");
            claimProcessingHistory.GetClaimHistoryTableDataValueByRowCol(1, 10)
                .ShouldBeEqual(notes, "Notes on which values have been changed should be shown");
            claimProcessingHistory.CloseClaimProcessingHistoryPageAndSwitchToClaimActionPage();
        }

        void SearchByClaimSeqFromWorkList(ClaimActionPage _claimAction,string claimSeq,bool handlePopup=true)
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
            _claimAction.WaitForCondition(() => !_claimAction.IsWorkListControlDisplayed(),3000);
        }

        void HandleAutomaticallyOpenedPatientClaimHistoryPopupFor(ClaimActionPage _claimAction,object obj)
        {
            if (obj is ClaimActionPage)
            {
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
            }
        }

        void DeleteOrRestoreLineFlag(ClaimActionPage _claimAction,string reasonCode)
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
        

        private void DeleteOrRestoreMprEditThenPsEditShouldBeDeletedOrRestored(ClaimActionPage _claimAction,string triggLineNum)
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

        void CheckTestClientAndSwitch(NewAutomatedBaseParallelRun automatedBase)
        {
            if (!automatedBase.QuickLaunch.IsDefaultTestClientForEmberPage(
                automatedBase.EnvironmentManager.TestClient))
            {
                automatedBase.CurrentPage.ClickOnSwitchClient().SwitchClientTo(automatedBase.EnvironmentManager.TestClient);
            }

        }

        #endregion

    }
}

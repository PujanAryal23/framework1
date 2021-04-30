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
using System.Security.Policy;
using Nucleus.Service.PageServices.Settings;
using UIAutomation.Framework.Core.Driver;
using Nucleus.Service.PageServices.Appeal;
using Nucleus.Service.PageServices.Base.Default;
using Nucleus.Service.PageServices.QuickLaunch;
using Nucleus.Service.PageServices.Settings.User;
using UIAutomation.Framework.Utils;
using Nucleus.Service.Support.Environment;
using Nucleus.Service.Support.Menu;
using Nucleus.UIAutomation.TestSuites.Common;

namespace Nucleus.UIAutomation.TestSuites.TestCases.ClientUser
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class ClaimActionClient
    {
        //#region PRIVATE FIELDS

        //private ClaimActionPage _claimAction;
        //private ClaimSearchPage _claimSearch;
        //private ClientSearchPage _clientSearch;
        //private ProfileManagerPage _profileManager;
        //private NewPopupCodePage _newpopupCodePage;
        //private AppealManagerPage _appealManager;
        //private ClaimHistoryPage _claimHistory;
        //private AppealActionPage _newppealaction;
        //private AppealSearchPage _appealSearch;
        //private CommonValidations _commonValidation;
        //private readonly string _dciWorkListPrivilege = RoleEnum.DCIAnalyst.GetStringValue();
        private string _claseq = "1465863-0";
        private string _claseq2 = "1465866-0";
        private string _claseq3 = "1465867-0";
       
        //#endregion

        #region PROTECTED PROPERTIES

        public string FullyQualifiedClassName
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
        //        base.ClassInit();
        //        _claseq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, "ClaimSequenceForClient", "ClaimSequence", "Value");
        //        _claseq2 = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, "ClaimSequence2ForClient", "ClaimSequence", "Value");
        //        _claseq3 = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, "ClaimSequence3ForClient", "ClaimSequence", "Value");
        //        _claseq3 = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, "ClaimSequence4ForClient", "ClaimSequence", "Value");
        //         _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(_claseq);
        //        _commonValidation = new CommonValidations(automatedBase.CurrentPage);
        //  }
        //    catch (Exception )
        //    {
        //        if (StartFlow != null)
        //            StartFlow.Dispose();
        //        throw ;
        //    }
        //}

        //protected override void TestCleanUp()
        //{

        //    if (string.Compare(UserType.CurrentUserType, UserType.CLIENT, StringComparison.OrdinalIgnoreCase) != 0)
        //    {
        //        automatedBase.CurrentPage = automatedBase.QuickLaunch = _claimAction.Logout().LoginAsClientUser();
        //        _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(_claseq);
        //    }
        //    if (!automatedBase.CurrentPage.IsCurrentClientAsExpected(automatedBase.EnvironmentManager.TestClient))
        //    {
        //        CheckTestClientAndSwitch();
        //        _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(_claseq);
        //    }
        //    if (automatedBase.CurrentPage.GetPageHeader() != "Claim Action")
        //    {
        //        _claimAction = automatedBase.CurrentPage.NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(_claseq);
        //        Console.Out.WriteLine("Finally Navigate to CV Claim Work List");
        //    }
        //    else if (_claimAction.IsWorkingAjaxMessagePresent())
        //    {
        //        _claimAction.Refresh();
        //        _claimAction.WaitForWorkingAjaxMessage();
        //    }

        //    if(automatedBase.CurrentPage.IsPageErrorPopupModalPresent())
        //        automatedBase.CurrentPage.ClosePageError();

        //    base.TestCleanUp();

        //}

        //protected override void ClassCleanUp()
        //{
        //    try
        //    {
        //        _claimAction.CloseDatabaseConnection();
        //        //if (automatedBase.CurrentPage.IsautomatedBase.QuickLaunchIconPresent())
        //        //    _claimAction.GoToautomatedBase.QuickLaunch();

        //    }
        //    finally
        //    {
        //        base.ClassCleanUp();
        //    }

        //}

        //#endregion

        #region TEST SUITES
        [Test, Category("Review")] //CAR-3234(CAR-3194)
        [Retry(3)]
        [Author("ShreyaS")]
        public void Verify_NDC_Value_In_Line_Details_client()
        {
            using (var automatedBase = new AutomatedBaseClientParallel(1))
            {
                ClaimActionPage _claimAction;
                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestName);
                string claimSeq = paramLists["ClaimSequence"];
                _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(claimSeq);

                var expectedNDCValueFromDb = _claimAction.GetNDCValueByClaimSeqFromDb(claimSeq.Split('-')[0],
                    claimSeq.Split('-')[1]);

                _claimAction.ClickOnLineDetailsSection();
                _claimAction.WaitForStaticTime(1000);
                _claimAction.ScrollToLastPosition();

                StringFormatter.PrintMessage("Verification of NDC value");
                _claimAction.IsDataPointLabelOfLineDetailByLabelPresent("NDC:").ShouldBeTrue("Is NDC label present?");
                _claimAction.GetDataPointValueOfLineDetailsByLabel("NDC:")
                    .ShouldBeEqual(expectedNDCValueFromDb, "NDC values should match");
            }
        }

        [Test, Category("OnDemand")] // CAR-3110(CAR-3097)
        public void Verify_Decision_Point_and_Reason_in_Flag_details_for_PPM_claims_client()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                ClaimActionPage _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(_claseq);
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var paramList = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
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
                    automatedBase.CurrentPage.ClickOnSwitchClient().SwitchClientTo(ClientEnum.RPE, typeAhead: true);

                    _claimAction = automatedBase.CurrentPage.NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(claSeq);
                    if (_claimAction.IsPageErrorPopupModalPresent())
                        _claimAction.ClosePageError();
                    _claimAction.ClickOnFlagLineByLineNoWithFlag(1, flag, 2);

                    if (processingType.Equals(processingTypes[0]))
                    {
                        StringFormatter.PrintMessageTitle("Verifying whether for client with processing type neither PCA Batch nor PCA Realtime, " +
                                                          "Reason and Decision points should not be displayed");

                        _claimAction.IsFlagDetailLabelPresentByLabelName("Reason Code:").ShouldBeFalse("Reason code should not be present for non PCA batch or realtime client");
                        _claimAction.IsFlagDetailLabelPresentByLabelName("Decision Point:").ShouldBeFalse("Decision point should not be present for non PCA batch or realtime client");
                    }

                    else
                    {
                        StringFormatter.PrintMessageTitle("Verifying whether for client with processing type either PCA Batch or PCA Realtime, " +
                                                          "Reason and Decision points should be displayed");

                        _claimAction.IsFlagDetailLabelPresentByLabelName("Reason Code:").ShouldBeTrue("Reason Code be present for PCA batch or realtime client");
                        _claimAction.IsFlagDetailLabelPresentByLabelName("Decision Point:").ShouldBeTrue("Decision Point should be present for PCA batch or realtime client");

                        StringFormatter.PrintMessage("Verifying Reason and Decision points against the DB");
                        _claimAction.GetFlagDetailsDataPointValueByUlAndLiNumber(2, 1)
                            .ShouldBeEqual(recReasonCodeAndDpKeyFromDb[0], "Reason code should match");
                        _claimAction.GetFlagDetailsDataPointValueByUlAndLiNumber(2, 2)
                            .ShouldBeEqual(recReasonCodeAndDpKeyFromDb[1], "Decision Point should match");
                    }

                }
                finally
                {
                    StringFormatter.PrintMessageTitle("Finally block : Reverting client to SMTST and the processing type for RPE");

                    if (_claimAction.IsPageErrorPopupModalPresent())
                        _claimAction.ClosePageError();

                    if (!_claimAction.IsDefaultTestClientForEmberPage(ClientEnum.SMTST))
                        _claimAction.ClickOnSwitchClient().SwitchClientTo(ClientEnum.SMTST, true);

                    _claimAction.GetCommonSql.UpdateProcessingTypeOfClient(processingTypes[0], ClientEnum.RPE.ToString());
                }
            }
        }

        [Test] //CAR-2441 [CAR-2571]
        public void Verify_scanned_image_id_field_for_client_user()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
               
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);

                var claSeqWithImageId = paramLists["ClaimSeqWithImageId"];
                var expectedImageId = paramLists["ImageId"];
                var claSeqWithoutImageId = paramLists["ClaimSeqWithoutImageId"];
                var claimSeqForDCIInactiveClient = paramLists["ClaimSeqForDCIInactiveClient"];
                ClaimActionPage _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(claSeqWithImageId);

                try
                {
                    StringFormatter.PrintMessageTitle("Verification of the Image ID field");
                    _claimAction.IsImageIdFieldPresent().ShouldBeTrue("The Image ID field should be present for the client for which DCA is active");
                    _claimAction.GetImageId().ShouldBeEqual(expectedImageId, "Correct Image ID should be displayed");

                    StringFormatter.PrintMessage("Verifying whether Image ID field is empty for claims that do not have an image id");

                    SearchByClaimSeqFromWorkList(claSeqWithoutImageId, _claimAction,false);
                    _claimAction.GetImageId().ShouldBeNullorEmpty("Image ID field should not show any value for claims which do not have an Image ID");

                    StringFormatter.PrintMessage("Verifying whether the Image ID field is not shown for DCA inactive clients");
                    _claimAction.SwitchClientAndNavigateClaimAction(ClientEnum.TTREE.ToString(),
                        claSeqWithoutImageId.Split('-')[0], claimSeqForDCIInactiveClient.Split('-')[0]);
                    _claimAction.IsImageIdFieldPresent().ShouldBeFalse("Image ID field should not be present for DCA Inactive clients");
                }
                finally
                {
                    if (!automatedBase.CurrentPage.IsCurrentClientAsExpected(automatedBase.EnvironmentManager.TestClient))
                        CheckTestClientAndSwitch(automatedBase.QuickLaunch,automatedBase.EnvironmentManager,automatedBase.CurrentPage);
                }

            }
        }

        [Test] //CAR-2208 [CAR-2503] + CAR-3078[CAR-3109]
        public void Verify_COB_approve_flags_for_client_user()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                ClaimActionPage _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(_claseq);
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var claimSeq = paramLists["ClaimSeq"];
                var cobFlag = paramLists["COB_Flag"];
                var claimSeqForUnassignedFlag = paramLists["ClaimSeqForUnassignedFlag"];
                var unassignedProductFlag = paramLists["UnassignedProductFlag"];
                var assignedProductFlag = paramLists["AssignedProductFlag"];
                var assignedAuthority = paramLists["AssignedAuthority"];
                var claimSeqForAssignedFlag = paramLists["ClaimSeqForAssignedFlag"];

                try
                {
                    _claimAction.UpdateStatusAndRestoreFlags(claimSeq, ClientEnum.SMTST.ToString());
                    _claimAction.UpdateStatusAndRestoreFlags(claimSeqForUnassignedFlag, ClientEnum.SMTST.ToString());
                    _claimAction.UpdateStatusAndRestoreFlags(claimSeqForAssignedFlag, ClientEnum.SMTST.ToString());
                    _claimAction.GetCommonSql.UpdateHciDoneOrClientDoneToFalseFromDatabase(ClientEnum.SMTST.ToString(),
                        "clidone", claimSeq, $"'{cobFlag}'");
                    _claimAction.GetCommonSql.UpdateHciDoneOrClientDoneToFalseFromDatabase(ClientEnum.SMTST.ToString(),
                        "clidone", claimSeqForUnassignedFlag, "'" + cobFlag + "'");
                    _claimAction.GetCommonSql.UpdateHciDoneOrClientDoneToFalseFromDatabase(ClientEnum.SMTST.ToString(),
                        "clidone", claimSeqForAssignedFlag, "'" + cobFlag + "'");

                    StringFormatter.PrintMessageTitle("Verifying user privileges and authorities are correct");
                    automatedBase.CurrentPage.Logout().LoginAsHciAdminUser();

                    StringFormatter.PrintMessage(
                        "Verification of COB RW and Manage Edits RW authority for client user");

                    _claimAction.IsRoleAssigned<UserProfileSearchPage>(
                        new List<string> {automatedBase.EnvironmentManager.ClientUserName},
                        RoleEnum.COBAuditor.GetStringValue()).ShouldBeTrue(
                        $"Is COB present for current user<{automatedBase.EnvironmentManager.ClientUserName}>");
                    automatedBase.CurrentPage.IsAvailableAssignedRowPresent(1, RoleEnum.ClaimsProcessor.GetStringValue())
                        .ShouldBeTrue(
                            $"Is Manage Edits present for current user<{automatedBase.EnvironmentManager.ClientUserName}>");

                    _claimAction
                        .IsRoleAssigned<UserProfileSearchPage>(new List<string> {automatedBase.EnvironmentManager.ClientUsername7},
                            RoleEnum.FCIAnalyst.GetStringValue()).ShouldBeFalse(
                            $"Is FCI present for current user<{automatedBase.EnvironmentManager.ClientUsername7}>");
                    automatedBase.CurrentPage.IsAvailableAssignedRowPresent(1, RoleEnum.COBAuditor.GetStringValue()).ShouldBeTrue(
                        $"Is COB present for current user<{automatedBase.EnvironmentManager.ClientUserName}>");
                    automatedBase.CurrentPage.IsAvailableAssignedRowPresent(1, RoleEnum.ClaimsProcessor.GetStringValue())
                        .ShouldBeTrue(
                            $"Is Manage Edits present for current user<{automatedBase.EnvironmentManager.ClientUserName}>");

                    #region CAR-3078[CAR-3109]

                    StringFormatter.PrintMessage(
                        "Verify Approve COB authority is added and COB product authority is removed from COB Auditor Role");
                    var roleManager = automatedBase.CurrentPage.NavigateToRoleManager();
                    roleManager.ClickEditIconByRoleNameAndUserType("COB Auditor", "Client");
                    roleManager.GetAssignedAuthoritiesList().ShouldCollectionBeEqual(
                        new List<string>() {assignedAuthority},
                        "Authority Should Match");

                    #endregion

                    automatedBase.CurrentPage.Logout().LoginAsClientUser().NavigateToClaimSearch().SearchByClaimSequence(claimSeq);

                    #region CAR-3078[CAR-3109]

                    StringFormatter.PrintMessageTitle("Verifying delete and edit buttons are disabled for COB flags");
                    _claimAction.IsApproveButtonEnabled().ShouldBeTrue("Is Approve Button Enabled ?");
                    _claimAction.IsTransferApproveButtonEnabled().ShouldBeTrue("Is Transfer Approve Button Enabled ?");
                    _claimAction.IsDeleteIconDisabledByLineNumber(1).ShouldBeTrue("Is Delete Icon Disabled in Line ?");
                    _claimAction.IsEditIconDisabledByLineno(1).ShouldBeTrue("Is Edit Icon Disabled in Line ?");
                    _claimAction.IsDeleteIconDisabledByLinenoAndRow(1, 1)
                        .ShouldBeTrue("Is delete icon disabled on flag level ?");
                    _claimAction.IsEditIconDisabledByLinenoAndRow(1, 1)
                        .ShouldBeTrue("Is edit icon disabled on flag level ?");
                    _claimAction.IsDeleteAllFlagsIconDisabled()
                        .ShouldBeTrue("Is delete icon on claim level disabled ?");
                    _claimAction.IsEditFlagsIconEnabled().ShouldBeFalse("Is edit icon on claim level enabled ?");

                    #endregion

                    StringFormatter.PrintMessageTitle("Verifying effects of approving a claim with COB flags");
                    _claimAction.GetClaimStatus().ShouldBeEqual(ClaimStatusTypeEnum.ClientUnreviewed.GetStringValue(),
                        $"The claim status should be {ClaimStatusTypeEnum.ClientUnreviewed.GetStringValue()} when at least one of the " +
                        "flags do not have CLIDONE = true");

                    _claimAction.ClickOnApproveButton();
                    _claimAction.SearchByClaimSequence(claimSeq);

                    StringFormatter.PrintMessage("Verifying whether COB flags are getting set to clidone true");
                    _claimAction.GetHciDoneAndCliDoneValuesByClaSeqLinNoFlagName(ClientEnum.SMTST.ToString(), "clidone",
                            claimSeq, 1, cobFlag)[0][0]
                        .ShouldBeEqual("T",
                            $"Clidone should be set to 'T' after approving for the COB flag : {cobFlag}");

                    StringFormatter.PrintMessage("Verifying whether claim status is getting updated correctly");
                    _claimAction.GetClaimStatus().ShouldBeEqual(ClaimStatusTypeEnum.ClientReviewed.GetStringValue(),
                        $"Claim status should be updated to {ClaimStatusTypeEnum.ClientReviewed.GetStringValue()} when all flags have clidone = 'T'");

                    StringFormatter.PrintMessage(
                        "Verifying whether the product flag to which the user does not have access to is not approved");
                    _claimAction.Logout().LoginAsClientUser7().NavigateToClaimSearch()
                        .SearchByClaimSequence(claimSeqForUnassignedFlag);

                    var claimStatusBeforeApproval = _claimAction.GetClaimStatus();
                    claimStatusBeforeApproval.ShouldBeEqual(ClaimStatusTypeEnum.ClientUnreviewed.GetStringValue(),
                        $"Claim should be in '{ClaimStatusTypeEnum.ClientUnreviewed.GetStringValue()}' status when at least one flag is not reviewed");

                    _claimAction.ClickOnApproveButton();
                    _claimAction.SearchByClaimSequence(claimSeqForUnassignedFlag);

                    _claimAction.GetClaimStatus().ShouldBeEqual(claimStatusBeforeApproval,
                        $"Claim status should not change since {unassignedProductFlag} is not approved because it is not assigned to the user");

                    _claimAction.GetHciDoneAndCliDoneValuesByClaSeqLinNoFlagName(ClientEnum.SMTST.ToString(), "clidone",
                            claimSeqForUnassignedFlag, 1, unassignedProductFlag)[0][0]
                        .ShouldBeEqual("F", "Product flags to which the user does not have access to is not approved");
                    _claimAction.GetHciDoneAndCliDoneValuesByClaSeqLinNoFlagName(ClientEnum.SMTST.ToString(), "clidone",
                            claimSeqForUnassignedFlag, 1, cobFlag)[0][0]
                        .ShouldBeEqual("T",
                            "COB flag which is assigned to the user should be approved with clidone set to 'T'");

                    #region CAR-3078[CAR-3109]

                    StringFormatter.PrintMessage(
                        "Verifying whether the product flag to which the user have access to other than COB is approved");
                    _claimAction.ClickWorkListIcon();
                    _claimAction.ClickSearchIcon();
                    _claimAction.SearchByClaimSequence(claimSeqForAssignedFlag);
                    _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                    _claimAction.GetClaimStatus().ShouldBeEqual(ClaimStatusTypeEnum.ClientUnreviewed.GetStringValue(),
                        $"Claim should be in '{ClaimStatusTypeEnum.ClientUnreviewed.GetStringValue()}' status when at least one flag is not reviewed");

                    _claimAction.ClickOnApproveButton();
                    _claimAction.SearchByClaimSequence(claimSeqForAssignedFlag);

                    _claimAction.GetClaimStatus().ShouldBeEqual(ClaimStatusTypeEnum.ClientReviewed.GetStringValue(),
                        $"Claim status should change since {claimSeqForAssignedFlag} is also approved because it is assigned to the user");

                    _claimAction.GetHciDoneAndCliDoneValuesByClaSeqLinNoFlagName(ClientEnum.SMTST.ToString(), "clidone",
                            claimSeqForAssignedFlag, 1, assignedProductFlag)[0][0]
                        .ShouldBeEqual("T", "Product flags to which the user does not have access to is not approved");
                    _claimAction.GetHciDoneAndCliDoneValuesByClaSeqLinNoFlagName(ClientEnum.SMTST.ToString(), "clidone",
                            claimSeqForAssignedFlag, 1, cobFlag)[0][0]
                        .ShouldBeEqual("T",
                            "COB flag which is assigned to the user should be approved with clidone set to 'T'");

                    #endregion
                }



                finally
                {
                    StringFormatter.PrintMessageTitle("Revert clidone status to 'F' for the flags");
                    _claimAction.UpdateStatusAndRestoreFlags(claimSeq, ClientEnum.SMTST.ToString());
                    _claimAction.GetCommonSql.UpdateHciDoneOrClientDoneToFalseFromDatabase(ClientEnum.SMTST.ToString(),
                        "clidone", claimSeq, $"'{cobFlag}'");

                    _claimAction.GetCommonSql.UpdateHciDoneOrClientDoneToFalseFromDatabase(ClientEnum.SMTST.ToString(),
                        "clidone", claimSeqForUnassignedFlag, "'" + cobFlag + "'");
                    _claimAction.GetCommonSql.UpdateHciDoneOrClientDoneToFalseFromDatabase(ClientEnum.SMTST.ToString(),
                        "clidone", claimSeqForAssignedFlag, "'" + cobFlag + "'");
                }
            }
        }

        //[Test, Category("CommonTableDependent"),Category("Working")] // CAR-2136(CAR-2454)
        public void Verify_COB_Product_Authorities_client()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                ClaimActionPage _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(_claseq);
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestExtensions.TestName);
                var claimSeq = paramLists["ClaimSequence"];
                var userId = paramLists["UserId"].Split(',').ToList();
                _claimAction.DeleteAllDeletedFlagsByClaSeq(claimSeq);

                try
                {

                    _claimAction.AddRoleForAuthorities(userId[0], RoleEnum.COBAuditor.GetStringValue(), false);
                    _claimAction.UpdateRWRORoleForAuthorities(userId[0], RoleEnum.ClaimsReadOnly.GetStringValue(),
                        RoleEnum.ClaimsProcessor.GetStringValue(), false);

                    StringFormatter.PrintMessage(
                        "Verification of COB RW and Manage Edits RW authority for client user");
                    _claimAction.Logout().LoginAsHciAdminUser();
                    _claimAction
                        .IsRoleAssigned<UserProfileSearchPage>(new List<string> {userId[0]},
                            RoleEnum.COBAuditor.GetStringValue()).ShouldBeTrue(
                            $"Is COB present for current user<{userId[0]}>");
                    automatedBase.CurrentPage.IsAvailableAssignedRowPresent(1, RoleEnum.ClaimsProcessor.GetStringValue())
                        .ShouldBeTrue(
                            $"Is Manage Edits present for current user<{userId[0]}>");

                    StringFormatter.PrintMessage(
                        "Verification of COB RO and Manage Edits RO for client user with read only authority");
                    _claimAction.IsRoleAssigned<UserProfileSearchPage>(new List<string> {userId[1]},
                        RoleEnum.COBAuditor.GetStringValue(), isAssigned: false).ShouldBeTrue(
                        $"Is COB present for current user<{userId[1]}>");
                    automatedBase.CurrentPage.IsAvailableAssignedRowPresent(1, RoleEnum.ClaimsReadOnly.GetStringValue()).ShouldBeTrue(
                        $"Is Manage Edits present for current user<{userId[1]}>");

                    //_claimAction.UpdateRWROAccessToAuthorities(1, userId[0], AuthoritiesEnum.ManageEdits.GetStringValue());
                    //_claimAction.UpdateRWROAccessToAuthorities(1, userId[0], AuthoritiesEnum.COB.ToString());

                    //StringFormatter.PrintMessage("Verification of COB RW and Manage Edits RW authority for client user");
                    //_profileManager = _claimAction.Logout().LoginAsHciAdminUser().NavigateToNewUserProfileSearch().ClickonUserNameToNavigateProfilemanagereUsingUserId(userId[0]);
                    //_profileManager.ClickOnPrivileges();
                    //_profileManager.IsReadWriteAssigned(AuthoritiesEnum.COB.ToString()).ShouldBeTrue("Is COB read/write authority present?");
                    //_profileManager.IsReadWriteAssigned(AuthoritiesEnum.ManageEdits.GetStringValue())
                    //    .ShouldBeTrue("Is Manage edits read/write authority present?");

                    //StringFormatter.PrintMessage("Verification of COB RO and Manage Edits RO for client user with read only authority");
                    //_profileManager.NavigateToNewUserProfileSearch().ClickonUserNameToNavigateProfilemanagereUsingUserId(userId[1]);
                    //_profileManager.ClickOnPrivileges();
                    //_profileManager.IsReadOnlyAssgined(AuthoritiesEnum.COB.ToString()).ShouldBeTrue("Is COB read/write authority present?");
                    //_profileManager.IsReadOnlyAssgined(AuthoritiesEnum.ManageEdits.GetStringValue())
                    //    .ShouldBeTrue("Is Manage edits read/write authority present?");

                    StringFormatter.PrintMessage(
                        "Verification of delete/restore/switch flag icon and functionality for client user with COB RW and Manage Edits RW authority");
                    _claimAction = automatedBase.CurrentPage.Logout().LoginClientUserWithDefaultSuspectProvidersPage()
                        .NavigateToClaimSearch()
                        .SearchByClaimSequenceToNavigateToClaimActionPage(claimSeq);
                    _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                    _claimAction.ClickOnDeleteIconOnFlagByLineNoAndRow(claimSeq);
                    _claimAction.WaitForWorking();
                    _claimAction.IsFlaggedLineDeletedByLine(1, 1).ShouldBeTrue("Flagged Line Should Be Deleted", true);
                    _claimAction.ClickOnRestoreIconOnFlagByLineNoAndRow(claimSeq);
                    _claimAction.WaitForWorking();
                    _claimAction.IsFlaggedLineDeletedByLine(1, 1)
                        .ShouldBeFalse("Flagged Line Should Be Restored", true);
                    _claimAction.IsSwitchIconNotDisplayedByLineNumberAndRowNum().ShouldBeTrue("Is Switch icon hidden?");

                    _claimAction.DeleteAllDeletedFlagsByClaSeq(claimSeq);

                    StringFormatter.PrintMessage("Changing COB authority to RO for client user");
                    _claimAction.RemoveRoleForAuthorities(userId[0], RoleEnum.COBAuditor.GetStringValue(), false);

                    StringFormatter.PrintMessage(
                        "Verification of delete/restore/switch flag icon and functionality for client user with COB RO and Manage Edits RW authority");
                    _claimAction.RefreshPage(false);

                    _claimAction.IsDeleteIconDisabledByLinenoAndRow(1, 1).ShouldBeTrue("Is Delete icon disabled?");
                    _claimAction.IsSwitchIconNotDisplayedByLineNumberAndRowNum().ShouldBeTrue("Is Switch icon hidden?");

                    StringFormatter.PrintMessage("Setting Manage edits to RO and COB RW");
                    _claimAction.UpdateRWRORoleForAuthorities(userId[0], RoleEnum.ClaimsProcessor.GetStringValue(),
                        RoleEnum.ClaimsReadOnly.GetStringValue(), false);
                    _claimAction.AddRoleForAuthorities(userId[0], RoleEnum.COBAuditor.GetStringValue(), false);

                    StringFormatter.PrintMessage(
                        "Verification of delete/restore/switch flag icon and functionality for client user with COB RW and Manage Edits RO authority");
                    _claimAction.RefreshPage(false);
                    _claimAction.IsDeleteIconDisabledByLinenoAndRow(1, 1).ShouldBeTrue("Is Delete icon disabled?");
                    _claimAction.IsSwitchIconNotDisplayedByLineNumberAndRowNum().ShouldBeTrue("Is Switch icon hidden?");

                    //StringFormatter.PrintMessage("Settings the authorities to RW for both COB and Manage edits");
                    //_claimAction.UpdateRWROAccessToAuthorities(1, userId[0], AuthoritiesEnum.ManageEdits.GetStringValue());

                    StringFormatter.PrintMessage(
                        "Verification of delete/restore/switch flag icon and functionality for client user with COB RO and Manage Edits RO authority");
                    _claimAction.Logout().LoginAsClientUserWithReadOnlyAccessToAllAuthorities();
                    _claimAction = automatedBase.CurrentPage.NavigateToClaimSearch()
                        .SearchByClaimSequenceToNavigateToClaimActionPage(claimSeq);
                    _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                    _claimAction.IsDeleteIconDisabledByLinenoAndRow(1, 1).ShouldBeTrue("Is Delete icon disabled?");
                    _claimAction.IsSwitchIconNotDisplayedByLineNumberAndRowNum().ShouldBeTrue("Is Switch icon hidden?");

                }
                finally
                {
                    _claimAction.AddRoleForAuthorities(userId[0], RoleEnum.COBAuditor.GetStringValue(), false);
                    _claimAction.UpdateRWRORoleForAuthorities(userId[0], RoleEnum.ClaimsReadOnly.GetStringValue(),
                        RoleEnum.ClaimsProcessor.GetStringValue(), false);
                }
            }

        }

        [Test] //CAR-1884(CAR-2306)
        public void Verify_presence_of_sug_Modifier_in_flag_details_for_client()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                ClaimActionPage _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(_claseq);
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestExtensions.TestName);
                var claimSeq = paramLists["ClaimSeq"];
                var flag = paramLists["Flag"];
                var sugModifierList = _claimAction.GetSugModifiersByClaimSeqAndEditFlagFromDatabase(claimSeq, flag);

                SearchByClaimSeqFromWorkList(claimSeq,_claimAction);

                StringFormatter.PrintMessage("Verification of 'Flag Details' when a particular flag is selected");
                _claimAction.ClickOnFlagLineByFlag(flag);
                _claimAction.GetFlagDetailLabelByPreceedingLabel("Sug Code:").ShouldBeEqual("Sug Modifier:",
                    "Sug Modifier should be placed after Sug Code");
                _claimAction.GetFlagDetailsDataPointValueByUlAndLiNumber(1, 4).Split(',').ToList()
                    .ShouldCollectionBeEqual(sugModifierList,
                        "Sug modifier should be comma separated in order : SUGGESTED_M1, SUGGESTED_M2, SUGGESTED_M3, SUGGESTED_M4");
            }
        }

        [Test]//CAR-1518
        public void Verify_page_error_modal_popup_doesnot_shows_error_message()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                ClaimActionPage _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(_claseq);
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var claimSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName,
                    "ClaimSequence", "Value");

                _claimAction.RestoreAllFlagByClaimSequence(claimSeq);
                _claimAction.DeleteClaimAuditRecordExceptAdd(claimSeq);
                SearchByClaimSeqFromWorkListForClientUser(claimSeq,_claimAction);
                for (var i = 0; i < 2; i++)
                {
                    var action = _claimAction.IsDeleteAllFlagIconPresentOnLine() ? "Delete" : "Restore";
                    _claimAction.ClickAndSaveOnEditAllFlagOnTheLineIcon("RPE 1 - RPE 1 Short");
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

        [Test] //CAR- 275(CAR-897)
        public void Verify_DCI_Claims_Worklist()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                ClaimActionPage _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(_claseq);
                var expectedClaimWorkList = automatedBase.DataHelper
                    .GetMappingData(FullyQualifiedClassName, "claim_work_list").Values.ToList();
                var expectedWorkListFilters = automatedBase.DataHelper
                    .GetMappingData(FullyQualifiedClassName, "Dci_worklist_filter_options").Values.ToList();
                var expectedClaimStatus =
                    automatedBase.DataHelper.GetMappingData(FullyQualifiedClassName, "Dci_claim_status").Values
                        .ToList();
                const string flag = "DCCI";
                const string batchId = "SCAC_HIST_21";

                _claimAction.GetSecondarySubMenuOptionList(HeaderMenu.Claim, SubMenu.ClaimWorkList)
                    .ShouldCollectionBeEqual(expectedClaimWorkList, "Is SubMenu DCA Under Claim Work List Equal?");
                _claimAction.NavigateToDciClaimsWorkList();
                _claimAction.GetClaimStatus()
                    .ShouldBeEqual(ClaimStatusTypeEnum.ClientUnreviewed.GetStringValue(),
                        "Status should be Client Unreviewed");
                _claimAction.IsDCIProductFlagPresentInDCIWorklist().ShouldBeTrue("Dental Flag should be present");
                if (!_claimAction.GetSideBarPanelSearch.IsSideBarPanelOpen())
                {
                    _claimAction.ClickWorkListIcon();
                }

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
                _claimAction.GetBatchIdInClaimDetails().ShouldBeEqual(batchId, "Batch IDs must match");
                _claimAction.GetFlagListForClaimLine(1)[0].ShouldBeEqual(flag, "Flags must match");

            }
        }

        [Test,]  //CAR- 275
        public void Validate_Security_And_Navigation_Of_DCI_Claim_WorkList_Page()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                CommonValidations _commonValidation = new CommonValidations(automatedBase.CurrentPage);
                string _dciWorkListPrivilege = RoleEnum.DCIAnalyst.GetStringValue();
                _commonValidation.ValidateSecurityAndNavigationOfAPage(HeaderMenu.Claim,
                            new List<string> { SubMenu.ClaimWorkList, SubMenu.DciClaimsWorkList },
                            _dciWorkListPrivilege, new List<string> { PageHeaderEnum.ClaimAction.GetStringValue() },

                            automatedBase.Login.LoginAsClientUserWithNoAnyAuthorityAndRedirectToQuickLaunch,
                            new[] { "uiautomation", "noauthority" }, automatedBase.Login.LoginAsClientUser);
            }
        }

        [Test, Category("SmokeTest")]
        public void Verify_that_if_client_setting_for_client_dot_client_user_phi_access_equal_to_U_and_user_does_not_have_authority_to_view_phi_then_user_does_not_see_client_fields_designated_as_Phi_and_does_not_see_original_claim_data_link()
        {
            using (var automatedBase = new AutomatedBaseClientParallel(100))
            {
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                automatedBase.QuickLaunch = automatedBase.Login.LoginAsAppealClientUser();
                CheckTestClientAndSwitch(automatedBase.QuickLaunch, automatedBase.EnvironmentManager,
                    automatedBase.CurrentPage);
                ClaimActionPage _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(_claseq2);
                try
                {
                    Console.WriteLine(
                        "Client user <Appluser_cl> with no view phi authority cannot see client field <HCIRUN> designated as PHI.");
                    _claimAction.IsHciRunLabelPresent().ShouldBeFalse("Hci Run Label is present.");
                    _claimAction.IsHciRunValuePresent().ShouldBeFalse("Hci Run Value is present.");
                    _claimAction.ClickMoreOption();
                    _claimAction.IsOriginalClaimDataLinkPresent().ShouldBeFalse("Original Data Claim Link is present.");
                }
                finally
                {
                    _claimAction.Logout().LoginAsClientUser().NavigateToClaimSearch()
                        .SearchByClaimSequenceToNavigateToClaimActionPage(_claseq);
                }
            }
        }

        [Test]
        public void Verify_that_if_client_setting_for_client_dot_client_user_phi_access_equal_to_U_and_user_has_authority_to_view_phi_then_user_see_client_fields_designated_as_Phi()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                ClaimActionPage _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(_claseq);
                Console.WriteLine("Client user <uiautomation_cl> with  view phi authority can see client field <HCIRUN> designated as PHI.");
                _claimAction.IsHciRunLabelPresent().ShouldBeTrue("Hci Run Label is present.");
                _claimAction.IsHciRunValuePresent().ShouldBeTrue("Hci Run Value is present.");
            }
        }

        //[Test]
        //public void Verify_a_if_client_setting_for_client_dot_client_user_phi_access_equal_to_U_and_user_does_not_have_authority_to_view_phi_then_user_does_not_see_original_claim_data_link()
        //{
        //    TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
        //    Console.WriteLine("Client user <uiautomation_cl> with no view phi authority");
        //    try
        //    {
        //        _claimAction.ClickMoreOption();
        //        _claimAction.IsOriginalClaimDataLinkPresent().ShouldBeFalse("Original Data Claim Link is present.");
        //    }
        //    finally
        //    {
        //        _claimAction.ClickMoreOption();
        //    }
        //}




        // [Test]//US14402
        public void Verify_that_the_view_notes_icon_will_be_displayed_on_Claim_Action_Page_if_notes_are_present_for_Client_User()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                string claimSeqWithNote = paramLists["ClaimSequenceWithNote"];
                ClaimActionPage _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(claimSeqWithNote);
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
        [Test]//US65703 + US65739 + CAR-2043
        public void Verify_red_badge_over_the_note_icon_represents_the_number_of_notes_that_exist_for_client_user()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                ClaimActionPage _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(_claseq);
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var claimSequenceWithBothClaimAndPrvNotes = paramLists["ClaimSequenceWithBothClaimAndPrvNotes"];
                var claimSequenceWithPrvNotesOnly = paramLists["ClaimSequenceWithPrvNotesOnly"];
                var prvseq = paramLists["PrvSeq"];
                var patseq = paramLists["PatSeq"];
                var totalClaimNoteCount =
                    _claimAction.TotalCountofClaimAndPatientNotes(claimSequenceWithBothClaimAndPrvNotes, patseq);
                var totalNoteCount =
                    Convert.ToInt32(_claimAction.TotalCountOfNotes(claimSequenceWithBothClaimAndPrvNotes, prvseq,
                        patseq));
                var totalProviderNoteCount = Convert.ToInt32(_claimAction.TotalCountofProviderNotes(prvseq));
                SearchByClaimSeqFromWorkListForClientUser(claimSequenceWithBothClaimAndPrvNotes,_claimAction);
                _claimAction.IsRedBadgeNoteIndicatorPresent().ShouldBeTrue("Is red badge note indicator present");
                _claimAction.NoOfClaimNotes().ShouldBeEqual(totalClaimNoteCount,
                    "Red Badge shows Claim notes only and not Provider notes");
                _claimAction.ClickOnClaimNotes();
                _claimAction.GetNoteListCount().ShouldBeEqual(totalNoteCount,
                    "Notes list should display both claim and provider notes");

                SearchByClaimSeqFromWorkListForClientUser(claimSequenceWithPrvNotesOnly,_claimAction);
                _claimAction.ClickOnClaimNotes();
                _claimAction.SelectNoteTypeInHeader("Type", "Provider");
                _claimAction.GetNoteListCount()
                    .ShouldBeEqual(totalProviderNoteCount, "Note list displays only Provider notes");
                _claimAction.IsRedBadgeNoteIndicatorPresent()
                    .ShouldBeFalse("red badge note indicator not present even though provider notes are present");
                _claimAction.IsAddNoteIndicatorPresent()
                    .ShouldBeFalse("Add note indicator not present even though proivder notes are present");
            }
        }

        [Test]//US31606 TE-751 + CAR-2952 (CAR-2884)
        [NonParallelizable]
        public void Verify_reason_codes_for_client_user()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                ClaimActionPage _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(_claseq);
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestExtensions.TestName);
                string claimSeq = paramLists["ClaimSequence"];
                string linSeq = paramLists["LineSequence"];
                var userAction = paramLists["UserAction"].Split(',').ToList();
                var ffpFlagProductPairs =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName, "FfpFlag");
                var cvFlagProductPairs =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName, "CvFlag");
                var dciFlagProductPairs =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName, "DciFlag");
                var fciFlagProductPairs =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName, "FciFlag");
                //var cobFlagPairs = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName, "CobFlag");
                List<Dictionary<string, string>> flagProductList = new List<Dictionary<string, string>>()
                {
                    ffpFlagProductPairs,
                    cvFlagProductPairs,
                    dciFlagProductPairs,
                    fciFlagProductPairs,
                    //cobFlagPairs
                };

                _claimAction.RestoreParticularFlagsByClaimSequence(claimSeq);

                try
                {
                    SearchByClaimSeqFromWorkListForClientUser(claimSeq,_claimAction);
                    StringFormatter.PrintMessage("Verify reason codes for Edit All Flags on the Claim option");
                    _claimAction.ClickOnEditAllFlagsIcon();
                    _claimAction.IsVisibleToclientPresentInAllFlagsSection()
                        .ShouldBeFalse("Visible to client checkbox displayed?");

                    StringFormatter.PrintMessage("Verify reason codes for No Delete/Restore Action");
                    VerifyReasonCodesBasedOnAction(userAction[2],_claimAction);
                    StringFormatter.PrintMessage("Verify reason codes for Delete Action");
                    _claimAction.ClickDeleteButtonOfEditAllFlagsSection();
                    VerifyReasonCodesBasedOnAction(userAction[0],_claimAction);
                    StringFormatter.PrintMessage("Verify reason codes for Restore Action");
                    _claimAction.ClickRestoreButtonOfEditAllFlagsSection();
                    VerifyReasonCodesBasedOnAction(userAction[1],_claimAction);
                    _claimAction.ClickOnCancelLink();

                    StringFormatter.PrintMessage("Verify reason codes for Edit All Flags on the Line Button");
                    _claimAction.ClickEditAllFlagsOnLineButton();
                    StringFormatter.PrintMessage("Verify reason codes for No Delete/Restore Action");
                    VerifyReasonCodesBasedOnAction(userAction[2], _claimAction,false);
                    StringFormatter.PrintMessage("Verify reason codes for Delete Action");
                    _claimAction.ClickDeleteButtonOfEditAllFlagsOnLineSection();
                    VerifyReasonCodesBasedOnAction(userAction[0], _claimAction,false);
                    StringFormatter.PrintMessage("Verify reason codes for Restore Action");
                    _claimAction.ClickRestoreButtonOfEditAllFlagsOnLineSection();
                    VerifyReasonCodesBasedOnAction(userAction[1], _claimAction,false);
                    _claimAction.ClickOnCancelLink();

                    StringFormatter.PrintMessage("Verify reason codes for particular flag of each product");
                    foreach (var flagProductPair in flagProductList)
                    {
                        _claimAction.ClickOnEditIconFlagLevelForLineEdit(2, flagProductPair.Values.FirstOrDefault());
                        StringFormatter.PrintMessage("Verify reason codes for No Delete/Restore Action");
                        VerifyReasonCodesBasedOnProductFlagAndAction(flagProductPair, userAction[2],_claimAction);
                        StringFormatter.PrintMessage("Verify reason codes for Delete Action");
                        _claimAction.ClickOnDeleteIconFlagLevelForLineEdit("2", flagProductPair.Values.First());
                        _claimAction.ClickOnEditIconFlagLevelForLineEdit(2, flagProductPair.Values.FirstOrDefault());
                        _claimAction.ClickRestoreButtonOfEditAllFlagsOnLineSection();
                        VerifyReasonCodesBasedOnProductFlagAndAction(flagProductPair, userAction[1],_claimAction);
                        StringFormatter.PrintMessage("Verify reason codes for Restore Action");
                        _claimAction.RestoreSpecificLineEditFlagByClient("2", flagProductPair.Values.First());
                        _claimAction.ClickOnEditIconFlagLevelForLineEdit(2, flagProductPair.Values.FirstOrDefault());
                        _claimAction.ClickDeleteButtonOfEditAllFlagsOnLineSection();
                        VerifyReasonCodesBasedOnProductFlagAndAction(flagProductPair, userAction[0],_claimAction);
                        _claimAction.ClickOnCancelLink();
                    }

                }
                finally
                {
                    _claimAction.RestoreParticularFlagsByClaimSequence(claimSeq);
                }
            }

        }


        [Test]//US31605 + CAR-3156(CAR-3144)
        public void Verify_system_deleted_flags_visibility_for_client_users()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                ClaimActionPage _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(_claseq);
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestExtensions.TestName);
                string claimSeqWithoutSystemDeletedFlags = paramLists["ClaimSequenceWithoutSystemDeletedFlags"];
                string claimSequenceWithSystemDeletedCVFlags = paramLists["ClaimSequenceWithSystemDeletedCVFlags"];
                string claimSequenceWithSystemDeletedCOBFlags = paramLists["ClaimSequenceWithSystemDeletedCOBFlags"];
                string claimSeqWithSystemDeletedPrototypeEdit =
                    paramLists["ClaimSequenceWithSystemDeletedPrototypeEdit"];
                var claimSequenceWithSystemDeletedPrePeps =
                    paramLists["ClaimSequenceWithSystemDeletedPrePeps"].Split(',').ToList();
                var prePeps = paramLists["PrePeps"].Split(',').ToList();
                var edits = paramLists["Edits"].Split(',').ToList();

                StringFormatter.PrintMessage("Verification for claseq without System Deleted Flags");
                VerifyPresenceOfSystemDeletedFlag(claimSeqWithoutSystemDeletedFlags);

                StringFormatter.PrintMessage("Verification for claseq with system deleted CV flags");
                SearchByClaimSeqFromWorkListForClientUser(claimSequenceWithSystemDeletedCVFlags,_claimAction);
                _claimAction.IsViewSystemDeletedFlagIconEnabled()
                    .ShouldBeTrue("View System Deleted Flags icon is Enabled");

                StringFormatter.PrintMessage("Verification of system deleted flag details");
                _claimAction.ClickSystemDeletedFlagIcon();
                _claimAction.IsSystemDeletedFlagPresentInFlagLinesSection()
                    .ShouldBeTrue("System Deleted Flags is displayed in Flagged Lines section");
                _claimAction.ClickOnSystemDeletedFlagLine();
                _claimAction.IsFlagAuditTrailInfoPresent().ShouldBeFalse("Audit Trail Information is displayed");
                _claimAction.IsCustomizationPopulatedValueVisible().ShouldBeTrue("'Cust' value is displayed");

                #region CAR-3156(CAR-3144)

                StringFormatter.PrintMessage("Verification for claseq with system deleted COB flags");
                _claimAction.GetProductOfSystemDeletedFlagByClaseqDb(claimSequenceWithSystemDeletedCOBFlags)
                    .ShouldBeEqual("C",
                        $"COB system deleted flag is present for the {claimSequenceWithSystemDeletedCOBFlags}");
                VerifyPresenceOfSystemDeletedFlag(claimSequenceWithSystemDeletedCOBFlags);

                StringFormatter.PrintMessage("Verification for claseq with system deleted Prototype(8888) flags");
                _claimAction.GetEditTypeOfSystemDeletedFlagByClaseqDb(claimSeqWithSystemDeletedPrototypeEdit)
                    .ShouldBeEqual("P",
                        $"Prototype system deleted flag is present for the {claimSeqWithSystemDeletedPrototypeEdit}");

                StringFormatter.PrintMessage(
                    "Verification for PrePeps starting with 0 and 1 and not starting with 0 or 1");
                for (int i = 0; i < claimSequenceWithSystemDeletedPrePeps.Count; i++)
                {
                    _claimAction.GetCountOfSystemDeletedFlagByEditClaseqPrepepDb(edits[i],
                        claimSequenceWithSystemDeletedPrePeps[i], prePeps[i]).ShouldBeGreaterOrEqual("1",
                        $"System deleted flags in present for {claimSequenceWithSystemDeletedPrePeps[i]} and {prePeps[i]}");
                    SearchByClaimSeqFromWorkListForClientUser(claimSequenceWithSystemDeletedPrePeps[i],_claimAction);
                    if (i == 2)
                    {
                        _claimAction.IsViewSystemDeletedFlagIconEnabled().ShouldBeTrue(
                            $"View System Deleted Flags icon is Enabled for PrePeps not starting with 0 or 1: {prePeps[i]}");
                        _claimAction.ClickSystemDeletedFlagIcon();
                        _claimAction.ClickOnSystemDeletedFlagLine();
                        _claimAction.GetCustomizationPopulatedField().ShouldBeEqual(prePeps[2],
                            "For PrePeps not starting with 0 or 1, system deleted flags should be displayed");
                    }
                    else
                    {
                        _claimAction.IsViewSystemDeletedFlagIconEnabled().ShouldBeFalse(
                            $"View System Deleted Flags icon is Disabled for PrePeps not starting with 0 or 1: {prePeps[i]}");
                    }

                }

                #endregion

                #region Local Methods

                void VerifyPresenceOfSystemDeletedFlag(string claimSeq)
                {
                    SearchByClaimSeqFromWorkListForClientUser(claimSeq,_claimAction);
                    _claimAction.IsViewSystemDeletedFlagIconEnabled()
                        .ShouldBeFalse("View System Deleted Flags icon is Enabled");
                }

                #endregion
            }
        }



        [Test, Category("AppealDependent")]//US45712 + CAR-3143
        public void Verify_create_appeal_enabled_only_when_Status_is_Client_Reviewed_or_Unreviewed_with_tool_tip_message_for_disabled_create_icon()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                ClaimActionPage _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(_claseq);
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var clientReviewedClaimSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "ClientReviewedClaimSeq", "Value");
                var clientUnreviewedClaimSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "ClientUnreviewedClaimSeq", "Value");
                var claimSeqList =
                    automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                            "ClaimSequenceList", "Value")
                        .Split(',')
                        .ToList();
                var claimSeqListNotReleasedToClient =
                    automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                            "ClaimSequenceListNotReleasedToClient", "Value")
                        .Split(',')
                        .ToList();
                var lockedClaimSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "LockedClaimSeq", "Value");
                var closedAppealClaimSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "ClosedAppealClaimSeq", "Value");
                var completeAppealClaimSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "CompleteAppealClaimSeq", "Value");


                SearchByClaimSeqFromWorkListForClientUser(clientReviewedClaimSeq,_claimAction);
                _claimAction.IsAddAppealIconEnabled()
                    .ShouldBeTrue("Create Appeal Icon Should enabled for claim status having client Reviewed");
                SearchByClaimSeqFromWorkListForClientUser(clientUnreviewedClaimSeq,_claimAction);
                _claimAction.IsAddAppealIconEnabled()
                    .ShouldBeTrue("Create Appeal Icon Should be enabled for claim status having client Unreviewed");
                SearchByClaimSeqFromWorkListForClientUser(closedAppealClaimSeq,_claimAction);
                _claimAction.IsAddAppealIconEnabled()
                    .ShouldBeTrue(
                        "Create Appeal Icon Should be enabled for claim status having client Reviewed and existing appeal is closed");
                SearchByClaimSeqFromWorkListForClientUser(completeAppealClaimSeq,_claimAction);
                _claimAction.IsAddAppealIconEnabled()
                    .ShouldBeTrue(
                        "Create Appeal Icon Should be enabled for claim status having client Reviewed and existing appeal is complete");



                Random rand = new Random();
                StringFormatter.PrintMessageTitle("Verification of tooltip of disabled create appeal icon");

                //Commented as part of change in CAR-3143

                //for (var i = 0; i < 3; i++)
                //{
                //    SearchByClaimSeqFromWorkListForClientUser(claimSeqListNotReleasedToClient[rand.Next(0, 10)]);
                //    var status = _claimAction.GetClaimStatus();
                //    _claimAction.IsAddAppealIconDisabled()
                //        .ShouldBeTrue(string.Format("Create Appeal Should be disabled for claims having status<{0}> that has not been released to the client", status));
                //    _claimAction.GetToolTipMessageDisabledCreateAppealIcon()
                //        .ShouldBeEqual(
                //            "An appeal cannot be added to a claim that is currently in process with Cotiviti.",
                //            "Tooltip Message for Unreviwed Claims");

                //}

                for (var i = 0; i < 3; i++)
                {

                    SearchByClaimSeqFromWorkListForClientUser(claimSeqList[rand.Next(0, 9)],_claimAction);
                    var status = _claimAction.GetClaimStatus();
                    _claimAction.IsAddAppealIconDisabled()
                        .ShouldBeTrue(
                            string.Format(
                                "Create Appeal Should be disabled for claims having status<{0}> that has been released to the client but not Reviewed/UnReviewed",
                                status));
                    _claimAction.GetToolTipMessageDisabledCreateAppealIcon()
                        .ShouldBeEqual(
                            "Appeals cannot be added to a claim  that is not in Client Reviewed or Unreviewed status.",
                            "Tooltip Message for status not Reviewed/Unreviwed for Relaesed Claims");

                }

                SearchByClaimSeqFromWorkListForClientUser(lockedClaimSeq,_claimAction);
                _claimAction.IsAddAppealIconDisabled()
                    .ShouldBeTrue(
                        "Create Appeal Should be disabled for claims that has appeal which status is not complete or closed");
                _claimAction.GetToolTipMessageDisabledCreateAppealIcon()
                    .ShouldBeEqual(
                        "Appeals cannot be opened for claims that have an existing appeal in process.",
                        "Tooltip Message for claim having appeal");
            }
        }




        [Test, Category("AppealDependent")] //US50641
        public void Verify_for_appeal_summary_pop_up_document_delete_will_be_enabled_or_disabled_based_on_appeal_status()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> testData = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestExtensions.TestName);
                var claimSeq = testData["ClaimSequence"];
                var appealSeqList = testData["AppealSequence"].Split(';'); //closed,compelte/new
                var fileType = testData["FileType"];
                var fileName = testData["FileName"];

                ClaimActionPage _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(claimSeq);
                _claimAction.ClickOnViewAppealIcon();
                AppealSummaryPage appealSummary = null;
                AppealProcessingHistoryPage appealHx = null;
                try
                {
                    automatedBase.CurrentPage = appealSummary =
                        _claimAction.ClickOnAppealSequenceToOpenAppealSummarypopup(appealSeqList[0]);
                    appealSummary.GetStatusValue().ShouldBeEqual("Closed",
                        "Confirming that we are checking for appeals with closed status");
                    appealSummary.IsAppealDocumentDeleteDisabled()
                        .ShouldBeTrue("For closed complete appeals delete file should be disabled.");
                    appealSummary.IsAppealDocumentUploadDisabled()
                        .ShouldBeTrue("For closed complete appeals upload file should be disabled.");
                    _claimAction.CloseAnyPopupIfExist();
                    automatedBase.CurrentPage = appealSummary =
                        _claimAction.ClickOnAppealSequenceToOpenAppealSummarypopup(appealSeqList[1]);
                    appealSummary.GetStatusValue().ShouldBeEqual("Complete",
                        "Confirming that we are checking for appeals with compelte status");
                    appealSummary.IsAppealDocumentDeleteDisabled()
                        .ShouldBeTrue("For closed complete appeals delete file should be disabled.");
                    appealSummary.IsAppealDocumentUploadDisabled()
                        .ShouldBeTrue("For closed complete appeals upload file should be disabled.");
                    _claimAction.CloseAnyPopupIfExist();
                    automatedBase.CurrentPage = appealSummary =
                        _claimAction.ClickOnAppealSequenceToOpenAppealSummarypopup(appealSeqList[2]);
                    appealSummary.GetStatusValue().ShouldBeEqual("New",
                        "Confirming that we are checking for appeals with new status");
                    var status = appealSummary.GetStatusValue();
                    appealSummary.UploadDocumentFromAppealSummary(fileType, fileName,
                        "new file test doc", 1);
                    appealSummary.ClickOnDeleteFileBtn(2);

                    if (appealSummary.IsPageErrorPopupModalPresent())
                        appealSummary.GetPageErrorMessage()
                            .ShouldBeEqual("The selected document will be deleted. Do you wish to proceed?");
                    appealSummary.ClickOkCancelOnConfirmationModal(false);
                    appealSummary.IsAppealDocumentDivPresent()
                        .ShouldBeTrue("All the two documents are still entact and listed.");
                    appealSummary.ClickOnDeleteFileBtn(1);
                    appealSummary.ClickOkCancelOnConfirmationModal(true);
                    appealSummary.WaitForWorkingAjaxMessage();
                    appealSummary.GetStatusValue()
                        .ShouldBeEqual(status, "Status Should be same after delete button click");
                    appealSummary.ClickMoreOption();

                    appealHx = appealSummary.ClickAppealProcessingHx();
                    appealHx.GetAppealAuditGridTableDataValue(1, 3)
                        .ShouldBeEqual("DeleteDoc", "Action Should be Delete"); //Status
                    _claimAction.CloseAllExistingPopupIfExist();
                }
                finally
                {
                    if (_claimAction.IsPageErrorPopupModalPresent()) _claimAction.ClosePageError();
                    _claimAction.CloseAnyPopupIfExist();
                }
            }
        }



        [Test] //US53174
        public void Verify_sug_code_on_flag_detail_is_clickable_and__popup_page_display_corect_information_for_CPT_and_HCPCS_code()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var claimSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "ClaimSequence", "Value");
                var CPTData = automatedBase.DataHelper
                    .GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName, "CPTData", "Value").Split(';');
                var HCPCSData = automatedBase.DataHelper
                    .GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName, "HCPCSData", "Value")
                    .Split(';');
                ClaimActionPage _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(claimSeq);

                try
                {
                    _claimAction.ClickOnFlagLineByFlag(HCPCSData[0]);
                    var _newpopupCodePage = _claimAction.ClickOnSugCodeLinkOnFlagDetails(HCPCSData[2], HCPCSData[1]);
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
                }
            }
        }


        [Test]//US57581
        public void Verify_claim_flag_audit_history_audit_detail_visible_for_releaed_claims_only()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                ClaimActionPage _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(_claseq);
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var claimSeqRelToClient = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName,
                    "ClaimSeqRelToClient", "Value");
                var claimSeqNotRelToClient = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName,
                    "ClaimSeqNotRelToClient", "Value"); //Cotiviti Unreivewed
                var flagForRelToClient = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName,
                    "FlagForRelToClient", "Value");
                var flagForNotRelToClient = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName,
                    "FlagForNotRelToClient", "Value");
                var lineNo = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName,
                    "LineNo", "Value");
                var reasonCode = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "ReasonCode", "Value");
                var flagSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "FlagSequence", "Value");
                var headerInfo = automatedBase.DataHelper
                    .GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName, "HeaderDetails", "Value")
                    .Split(';');
                //lineno,flag1,flag2,sugcode,cust,addl


                _claimAction.DeleteClaimAuditRecordFromDatabase(flagSeq, claimSeqRelToClient, lineNo);

                SearchByClaimSeqFromWorkListForClientUser(claimSeqRelToClient,_claimAction);

                _claimAction.GetClaimFlagAuditHistoryIconTooltip()
                    .ShouldBeEqual("Claim Flag Audit History", "Tooltip of Claim Flag Audit History Icon");
                _claimAction.ClickOnClaimFlagAuditHistoryIcon();

                List<DateTime> modifiedDateList = _claimAction
                    .GetClaimFlagAuditHistorysDetailListByFlagAndLabel(headerInfo[1], "Mod Date")
                    .Select(DateTime.Parse)
                    .ToList();

                modifiedDateList.IsInDescendingOrder()
                    .ShouldBeTrue("Recent Date should be on top");




                _claimAction.GetFlagOnClaimFlagAuditHistoryByRow(1).ShouldBeEqual(headerInfo[1], "Flag should display");

                _claimAction.GetLineNoOnClaimFlagAuditHistoryByRow(1)
                    .ShouldBeEqual(headerInfo[0], "Line No should display");
                _claimAction.IsRuleNoteFieldPresent()
                    .ShouldBeFalse("Rule Note field is not displayed to the client user");
                //_claimAction.GetClaimFlagAuditHistoryHeaderDetailByFlagAndLabel(headerInfo[2], "Sug Code")
                //    .ShouldBeEqual(headerInfo[3], "Sug Code should display");
                //_claimAction.GetClaimFlagAuditHistoryHeaderDetailByFlagAndLabel(headerInfo[1], "Cust")
                //    .ShouldBeEqual(headerInfo[4], "Cust(i.e.,prepep) should display");
                //_claimAction.GetClaimFlagAuditHistoryHeaderDetailByFlagAndLabel(headerInfo[1], "Addl")
                //    .ShouldBeEqual(headerInfo[5], "Addl(i.e., comment) should display");

                _claimAction.IsClaimAuditHistoryDivPresentByLineNoAndFlag(flagForRelToClient, lineNo)
                    .ShouldBeFalse("Audit Record should not present.");

                _claimAction.ClickOnEditIconFlagLevelForLineEdit(Convert.ToInt32(lineNo), flagForRelToClient);
                _claimAction.SelectReasonCode(reasonCode);
                _claimAction.SetNoteToVisbleTextarea("Test");
                _claimAction.SaveFlagAndClickFlagAudiHistory(reasonCode, Convert.ToInt32(lineNo), flagForRelToClient,
                    claimSeqRelToClient);

                _claimAction.IsClaimAuditHistoryDivPresentByLineNoAndFlag(flagForRelToClient, lineNo)
                    .ShouldBeTrue("Audit Record should updated automatically.", true);


                _claimAction.GetClaimFlagAuditHistoryDetailByFlagAndRowAndLabelAndLineNo(headerInfo[1], "By",
                        headerInfo[0], modifiedDateList.Count)
                    .ShouldBeEqual("Test Automation", "Audit Record should  display for Realsed Client.");



                Convert.ToDateTime(
                        _claimAction.GetClaimFlagAuditHistoryDetailByFlagAndRowAndLabelAndLineNo(flagForRelToClient,
                            "Mod Date", lineNo)).ToString("MM/dd/yyyy")
                    .IsDateInFormat()
                    .ShouldBeTrue("Is Mod Date in mm/dd/yyyy format");
                _claimAction
                    .GetClaimFlagAuditHistoryDetailByFlagAndRowAndLabelAndLineNo(flagForRelToClient, "By", lineNo)
                    .DoesNameContainsOnlyFirstWithLastname()
                    .ShouldBeTrue("Modified By Should be in <FirstName> <LastName>");
                _claimAction
                    .GetClaimFlagAuditHistoryDetailByFlagAndRowAndLabelAndLineNo(flagForRelToClient, "Action", lineNo)
                    .ShouldNotBeNull("Action Should not null");
                _claimAction
                    .GetClaimFlagAuditHistoryDetailByFlagAndRowAndLabelAndLineNo(flagForRelToClient, "Type", lineNo)
                    .ShouldNotBeNull("Action Should not null");
                _claimAction
                    .GetClaimFlagAuditHistoryDetailByFlagAndRowAndLabelAndLineNo(flagForRelToClient, "Code", lineNo)
                    .ShouldNotBeNull("Action Should not null");

                _claimAction
                    .GetClaimFlagAuditHistoryDetailByFlagAndRowAndLabelAndLineNo(flagForRelToClient, "Notes", lineNo)
                    .Length.ShouldBeGreaterOrEqual(0, "Notes may present or can be null");

                SearchByClaimSeqFromWorkListForClientUser(claimSeqNotRelToClient,_claimAction);
                _claimAction.ClickOnClaimFlagAuditHistoryIcon();


                _claimAction.IsClaimAuditHistoryDivPresentByLineNoAndFlag(flagForNotRelToClient, lineNo)
                    .ShouldBeFalse("Is Audit Record display for Not Realsed Client.");
            }

        }



        [Test]//US65704 + US65703
        public void Verify_that_Visible_to_client_option_is_not_available_client_user()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var claimSeq = paramLists["ClaimSequence"];
                var userName = paramLists["UserName"];
                ClaimActionPage _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(claimSeq);
                _claimAction.ClickOnClaimNotes();
                _claimAction.IsNoteContainerPresent()
                    .ShouldBeTrue("Note Container must display after clicking note icon.");
                _claimAction.IsVisibletoClientIconPresentByName("Clare Andujar Cohen")
                    .ShouldBeFalse("Visible to Client icon(Tick mark) must not be present for client user.");
                _claimAction.ClickOnEditIconOnNotesByName(userName);
                _claimAction.IsVisibleToClientCheckboxPresentInNoteEditorByName(userName)
                    .ShouldBeFalse("Visible to Client checkbox not available to client user in Edit Note form. ");
                _claimAction.ClickonAddNoteIcon();
                _claimAction.IsVisibleToClientCheckboxPresentInAddNoteForm()
                    .ShouldBeFalse("Visible To Client option not available to client user in Add Note form");
            }
        }

        [Test]//US65704 + US65703
        public void Verify_that_client_user_cannot_view_Notes_with_visible_to_client_false()
        {
            using (var automatedBase = new AutomatedBaseClientParallel(100))
            {
                var testName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, testName);
                var claimSeq = paramLists["ClaimSequence"];
                var internalUser = paramLists["InternalUser"];
                automatedBase.QuickLaunch = automatedBase.Login.LoginAsHciAdminUser2();
                ClaimActionPage _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(claimSeq);
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                _claimAction.ClickOnClaimNotes();

                if (_claimAction.IsVisibletoClientIconPresentByName(internalUser))
                {
                    _claimAction.ClickOnEditIconOnNotesByName(internalUser);
                    _claimAction
                        .ClickVisibleToClientCheckboxInNoteEditorByName(internalUser); //set visible to client false
                    _claimAction.ClickOnSaveButtonInNoteEditorByName(internalUser);
                }

                var notesCountForInternalUser = _claimAction.GetNoteListCount();

                _claimAction.Logout().LoginAsClientUser();
                _claimAction =
                    automatedBase.QuickLaunch.NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(_claseq);
                SearchByClaimSeqFromWorkListForClientUser(claimSeq,_claimAction);
                _claimAction.ClickOnClaimNotes();
                _claimAction.GetNoteListCount().ShouldBeEqual(notesCountForInternalUser - 1,
                    "Client can view only those notes with visible to client option set to true");
            }
        }


        [Test] //US69376 + TE- 751
        public void Validate_note_character_limit_in_claim_action_for_edit_add_transfer_transferApprove_notes_and_visible_to_client_is_not_present_for_client()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var claimSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName,
                    "ClaimSequence", "Value");
                var notes = new string('a', 1994);
                ClaimActionPage _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(claimSeq);
                StringFormatter.PrintMessage("Validate character limit for note field for Edit Flag");
                _claimAction.ClickOnEditIconFlagLevelForLineEdit(1, "DREC");
                _claimAction.IsVisibleToclientPresentInFlagLineSection()
                    .ShouldBeFalse("visible to client check box displayed?");
                ValidateMaxCharacterLimitOnNotes(notes,_claimAction);
                StringFormatter.PrintMessage("Validate character limit for note field for Edit All flags On Line");
                _claimAction.ClickEditAllFlagsOnLineButton();
                if (_claimAction.IsPageErrorPopupModalPresent()) _claimAction.ClickOkCancelOnConfirmationModal(true);
                ValidateMaxCharacterLimitOnNotes(notes,_claimAction);
                _claimAction.IsVisibleToclientPresentInEditAllFlagLineSection()
                    .ShouldBeFalse("visible to client displayed?");
                StringFormatter.PrintMessage("Validate character limit for note field for Edit All flags On Claim");
                _claimAction.ClickOnEditAllFlagsIcon();
                _claimAction.IsVisibleToclientPresentInAllFlagsSection()
                    .ShouldBeFalse("visible to client check box displayed?");
                if (_claimAction.IsPageErrorPopupModalPresent()) _claimAction.ClickOkCancelOnConfirmationModal(true);
                ValidateMaxCharacterLimitOnNotes(notes,_claimAction);
                StringFormatter.PrintMessage("Validate character limit for note field for Transfer Claim");
                _claimAction.ClickOnTransferButton();
                ValidateMaxCharacterLimitOnNotes(notes,_claimAction);
                StringFormatter.PrintMessage("Validate character limit for note field for Transfer/Approve Claim");
                _claimAction.ClickOnTransferApproveButton();
                ValidateMaxCharacterLimitOnNotes(notes,_claimAction);
            }
        }


        [Test]//US67974 
        [NonParallelizable]
        public void Verify_auto_approved_flags_message_while_deleting_all_flag_for_claims_with_and_without_AutoApproved_Flags()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                ClaimActionPage _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(_claseq);
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var claimSequenceWithAAFlag = paramLists["ClaimSequenceWithAAFlag"];
                var claimSequenceWithoutAAFlag = paramLists["ClaimSequenceWithoutAAFlag"];
                var flaseq = paramLists["FlagSeq"];
                var lino = paramLists["lino"];

                _claimAction.DeleteClaimAuditOnlyByClient(claimSequenceWithAAFlag, "01-JUN-2019",
                    ClientEnum.SMTST.ToString());
                _claimAction.DeleteClaimAuditOnlyByClient(claimSequenceWithoutAAFlag, "01-JUN-2019",
                    ClientEnum.SMTST.ToString());
                _claimAction.DeleteClaimAuditRecordFromDatabaseByCleint(flaseq, claimSequenceWithoutAAFlag, lino,
                    ClientEnum.SMTST.ToString());
                _claimAction.DeleteLineFlagAuditByClaimSequence(claimSequenceWithAAFlag, ClientEnum.SMTST.ToString());
                _claimAction.UpdateStatusAndRestoreFlags(claimSequenceWithoutAAFlag, ClientEnum.SMTST.ToString());
                _claimAction.UpdateStatusAndRestoreFlags(claimSequenceWithAAFlag, ClientEnum.SMTST.ToString());

                try
                {
                    SearchByClaimSeqFromWorkListForClientUser(claimSequenceWithAAFlag,_claimAction);
                    _claimAction.WaitForStaticTime(2000);
                    _claimAction.ClickOnDeleteAllFlagsIcon(false, false);
                    _claimAction.IsPageErrorPopupModalPresent().ShouldBeTrue(
                        "The AA flags warning message should  be shown when there is at least one flag that the user has the product authority to delete, that is also auto-approved. ");
                    _claimAction.GetPageErrorMessage().ShouldBeEqual(
                        "At least one auto-reviewed flag is present on this claim. Do you wish to delete all flags?");
                    _claimAction.ClickOnErrorMessageOkButton();
                    _claimAction.WaitForWorkingAjaxMessage();

                    _claimAction.RemoveLock();
                    _claimAction.ClickOnApproveButton();
                    _claimAction.WaitForStaticTime(500);

                    SearchByClaimSeqFromWorkListForClientUser(claimSequenceWithoutAAFlag,_claimAction);
                    _claimAction.ClickOnDeleteAllFlagsIcon();
                    _claimAction.IsPageErrorPopupModalPresent().ShouldBeFalse(
                        "The AA flags warning message should not be shown when there is no Auto Approved flags on claim. ");

                }
                finally
                {
                    if (_claimAction.IsPageErrorPopupModalPresent())
                        _claimAction.ClosePageError();
                    if (_claimAction.IsRestoreButtonPresent())
                        _claimAction.ClickOnRestoreAllFlagsIcon();
                    SearchByClaimSeqFromWorkListForClientUser(claimSequenceWithAAFlag,_claimAction);
                    if (_claimAction.IsRestoreButtonPresent())
                        _claimAction.ClickOnRestoreAllFlagsIcon();
                    SearchByClaimSeqFromWorkListForClientUser(_claseq,_claimAction);
                }
            }
        }
        

        [Test] //us43143
        public void Verify_appeal_status_and_expected_claim_lock_messages_for_different_appeal_status()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                ClaimActionPage _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(_claseq);
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var claimSequenceList = paramLists["ClaimSequence"].Split(';');
                var claimWithOpenappeal = claimSequenceList[0];
                var claimWithNewappeal = claimSequenceList[1];
                var claimWithDocsWaitingappeal = claimSequenceList[2];
                var claimWithCompleteappeal = claimSequenceList[3];
                var claimWithClosedappeal = claimSequenceList[4];
                var lockMessage = paramLists["LockMessage"];

                VerifyLockMessageWithDisabledCreateAppealIcon(claimWithOpenappeal,
                    AppealStatusEnum.Open.GetStringDisplayValue(), lockMessage,_claimAction);
                VerifyLockMessageWithDisabledCreateAppealIcon(claimWithNewappeal,
                    AppealStatusEnum.New.GetStringDisplayValue(), lockMessage,_claimAction);
                VerifyLockMessageWithDisabledCreateAppealIcon(claimWithDocsWaitingappeal,
                    AppealStatusEnum.DocumentsWaiting.GetStringDisplayValue(), lockMessage,_claimAction);
                VerifyLockMessageWithEnabledCreateAppealIcon(claimWithCompleteappeal,
                    AppealStatusEnum.Complete.GetStringDisplayValue(),_claimAction);
                VerifyLockMessageWithEnabledCreateAppealIcon(claimWithClosedappeal,
                    AppealStatusEnum.Closed.GetStringDisplayValue(),_claimAction);
            }

        }

        [Test] //CAR-206
        public void Verify_Dental_data_points_alignment_client()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var claimSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName,
                    "ClaimSequence", "Value").Split(',').ToList();
                //var claimseq2 = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                //    "ClaimSequence2", "Value");
                var expectedDataPoints = automatedBase.DataHelper
                    .GetMappingData(FullyQualifiedClassName, "Dental_data_points_client").Values.ToList();
                ClaimActionPage _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(claimSeq[0]);
                try
                {
                    var _claimHistory = _claimAction.ClickOnHistoryButtonAndSwitchToPatientClaimHistoryPage();
                    _claimHistory.IsBackgroundColorDiff().ShouldBeTrue("Background color should be different");
                    _claimHistory.GetDentalDataPoints()
                        .ShouldCollectionBeEqual(expectedDataPoints, "Data points should be in order");
                    _claimHistory.IsDOSSorted()
                        .ShouldBeTrue(
                            "Claims should be shown in order of DOS, most recent claims should be at the top");
                    _claimAction = _claimHistory.SwitchToNewClaimActionPage(true);
                    SearchByClaimSeqFromWorkListForClientUser(claimSeq[1],_claimAction);
                    _claimHistory = _claimAction.ClickOnHistoryButtonAndSwitchToPatientClaimHistoryPage();
                    _claimHistory.GetDentalDataPointsValue().ShouldCollectionBeEqual(
                        _claimHistory.GetDataBaseValueFromDb(claimSeq[1]), "Data points values should match");
                    _claimHistory.GetProcDesc(_claimHistory.GetValueInGridByCol(8), "DEN")
                        .ShouldBeEqual(_claimHistory.MouseOverAndGetToolTipString(10), "The descriptions shoud match");
                    _claimAction = _claimHistory.SwitchToNewClaimActionPage(true);

                }
                finally
                {
                    _claimAction.CloseAnyPopupIfExist();

                }
            }
        }

        [Test] // CAR-507
        public void Verify_addition_of_Dental_details_in_Flag_and_Claim_lines_Client()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var claimSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName,
                    "ClaimSequence", "Value");
                var title = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                    "Title", "Value").Split(',').ToList();
                var toolTipValue = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName,
                    "ToolTipValue", "Value").Split(',').ToList();
                ClaimActionPage _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(claimSeq);
                var expectedDetalDataPoint = _claimAction.GetDentalDataPointsValuesFromDb(claimSeq);
                var listDataPointsFlagLine = new List<string>();
                var listDataPointsClaimLine = new List<string>();
                _claimAction.DoesClaimLineContainAdjPaidandMod().ShouldBeTrue("Mod and Adj Paid should be present");

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

        [Test, Category("OnDemand")] //CAR-507
        public void Verify_Presence_Of_AdjPaid_And_Mod_When_DCI_Is_The_Only_Active_Product_client()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                ClaimActionPage _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(_claseq);
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var claimSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName,
                    "ClaimSequence", "Value");

                try
                {
                    _claimAction.UpdateProductTypeToInactiveByClientCodeFromDb(ClientEnum.SMTST.ToString());
                    _claimAction.Refresh();
                    _claimAction.WaitForWorkingAjaxMessage();
                    SearchByClaimSeqFromWorkListForClientUser(claimSeq,_claimAction);
                    _claimAction.DoesClaimLineContainAdjPaidandMod()
                        .ShouldBeFalse("Mod and Adj Paid should not be present");
                }

                finally
                {
                    _claimAction.UpdateProductTypeToActiveByClientCodeFromDb(ClientEnum.SMTST.ToString());
                    _claimAction.Refresh();
                    _claimAction.WaitForWorkingAjaxMessage();
                }
            }
        }



        [Test] //CAR-845(CAR-1340)
        public void Verify_inability_to_edit_a_flag_note_by_client_user()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var claimSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "ClaimSequence", "Value");
                var username = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "UserName", "Value");
                ClaimActionPage _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(claimSeq);
                try
                {
                    _claimAction.ClickOnClaimFlagAuditHistoryIcon();
                    _claimAction.IsFlagAuditHistoryEditNoteIconPresent(username)
                        .ShouldBeFalse("Edit icon should not be present for the notes in case of client users.");
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
            using (var automatedBase = new AutomatedBaseClientParallel(100))
            {
                automatedBase.QuickLaunch = automatedBase.Login.LoginAsClientUser2();
                ClaimActionPage _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(_claseq);
                _claimAction.ClickMoreOption();
                _claimAction.IsOriginalClaimDataLinkPresent()
                    .ShouldBeTrue("Original Claim Data Link should Be Present");
                var originalClaimDataPage = _claimAction.ClickOnOriginalClaimDataAndSwitch();
                originalClaimDataPage.GetPageHeader()
                    .ShouldBeEqual("Original Claim Data", "Page Header Should Match");
                originalClaimDataPage.GetClaimSequenceInHeader()
                    .ShouldBeEqual(_claseq, "Claim Sequence Should Match");
                var (originalClaimDataFromDb, columnNames) =
                    originalClaimDataPage.GetOriginalClaimDataFromDatabase(_claseq);
                originalClaimDataPage.GetColumnNames().ShouldBeEqual(columnNames, "Column Names Should Match");
                int i = 1;
                foreach (var row in originalClaimDataFromDb)
                {
                    originalClaimDataPage.GetOriginalClaimDataValuesByRow(i)
                        .ShouldBeEqual(row, "Data Should Match");
                    i++;
                }
                _claimAction.CloseAllExistingPopupIfExist();
            }
        }

        [Test, Category("NewClaimAction1"), Category("SchemaDependent")] //TE-671
        public void Verify_clicking_on_Appeal_Sequence_opens_up_Appeal_Action_in_a_Separate_Tab_For_Dental_Appeal_For_clientUser()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var claimSeq = paramLists["ClaimSequence"];
                ClaimActionPage _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(claimSeq);
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
                    appealSummary.IsClientNucleusHeaderPresent().ShouldBeFalse("Nucleus Header should not be present");
                    appealSummary.IsWindowOpenedAsTab().ShouldBeFalse("Is appeal Summary opened in separate tab?");
                    var windowCount = appealSummary.GetWindowHandlesCount();


                    StringFormatter.PrintMessage("verify that Record Review appeal opens in pop up from claim action");
                    appealSummary.SwitchToClaimActionByUrl();
                    _claimAction.GetAppealTypeByRow(3).ShouldBeEqual(AppealType.RecordReview.GetStringDisplayValue(),
                        "correct appeal type displayed?");
                    appealSummary = _claimAction.ClickOnAppealSequenceToOpenAppealSummarypopupByAppealType(3);
                    appealSummary.GetPageHeader()
                        .ShouldBeEqual(PageTitleEnum.AppealSummary.GetStringDisplayValue(), "Page Title");
                    appealSummary.IsClientNucleusHeaderPresent().ShouldBeFalse("Nucleus Header should not be present");
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
                    appealSummary.IsClientNucleusHeaderPresent().ShouldBeTrue("Nucleus Header should be present");
                    appealSummary.GetWindowHandlesCount()
                        .ShouldBeEqual(windowCount + 1, "Windows count should be increased");
                    appealSummary.IsWindowOpenedAsTab().ShouldBeTrue("Is appeal Summary opened in separate tab?");

                    _claimAction = appealSummary.CloseAppealSummaryWindow();

                }
                finally
                {
                    _claimAction.CloseAllExistingPopupIfExist();
                }
            }
        }

        [Test]// TE-755
        public void Verify_Dental_Profiler_Icon_Displayed_For_Client_User()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var claimSeqWithIcon = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "ClaimSequence1", "Value");
                var claimSeqWithoutIcon = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "ClaimSequence2", "Value");
                ClaimActionPage _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(claimSeqWithIcon);
                try
                {
                    StringFormatter.PrintMessage(
                        "Verify Dental icon is displayed icon is  Displayed for provider where dental profile review is set to true");
                    ValidateDentalProfilerIcon(claimSeqWithIcon,_claimAction);

                    StringFormatter.PrintMessage(
                        "Verify Dental icon is not Displayed for provider where dental profile review is set to false");

                    _claimAction.GetSideBarPanelSearch.OpenSidebarPanel();
                    SearchByClaimSeqFromWorkList(claimSeqWithoutIcon.Split(',')[0],_claimAction);
                    ValidateDentalProfilerIcon(claimSeqWithoutIcon.Split(',')[0],_claimAction);

                    StringFormatter.PrintMessage(
                        "verify Dental icon should not be displayed for client with DCA product disabled");
                    if (_claimAction.GetProductStatusForClient(ProductEnum.DCA.ToString(), ClientEnum.TTREE.ToString()))
                        _claimAction.UpdateProductStatusForClient(ProductEnum.DCA.ToString(),
                            ClientEnum.TTREE.ToString());
                    _claimAction = automatedBase.CurrentPage.ClickOnSwitchClient().SwitchClientTo(ClientEnum.TTREE, true)
                        .NavigateToClaimSearch()
                        .SearchByClaimSequenceToNavigateToClaimActionPage(claimSeqWithoutIcon.Split(',')[1]);
                    ValidateDentalProfilerIcon(claimSeqWithoutIcon.Split(',')[1], _claimAction,false);
                }
                finally
                {
                    if (!automatedBase.CurrentPage.IsDefaultTestClientForEmberPage(ClientEnum.SMTST))
                    {
                        _claimAction.ClickOnSwitchClient()
                            .SwitchClientTo(ClientEnum.SMTST, true)
                            .NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(_claseq);
                        _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                    }

                }
            }
        }



        [Test]//TE-768
        [NonParallelizable]
        public void Verify_Default_Reason_code_for_when_DCI_Flag_Is_Deleted_For_Client_User()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                ClaimActionPage _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(_claseq);
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestExtensions.TestName);
                var claimSeq = paramLists["ClaSeq"];
                var userAction = paramLists["Action"].Split(',').ToList();

                _claimAction.RestoreAllFlagByClaimSequence(claimSeq);
                _claimAction.DeleteLineFlagAuditByClaimSequence(claimSeq, ClientEnum.SMTST.ToString());

                StringFormatter.PrintMessage("Verify default reason code when flag is deleted");
                SearchByClaimSeqFromWorkList(claimSeq,_claimAction);
                var flagList = _claimAction.GetFlagListForClaimLine(1);
                _claimAction.ClickOnDeleteIconOnFlagByLineNoAndRow(claimSeq, row: 2);

                VerifyReasonCodeForFLags(new List<string> {flagList[1]}, userAction[0],_claimAction);
                _claimAction.ClickOnRestoreIconOnFlagByLineNoAndRow(claimSeq, row: 2);
                VerifyReasonCodeForFLags(new List<string> {flagList[1]}, userAction[1],_claimAction);

                StringFormatter.PrintMessage("Verify default reason code when all flags on the line are deleted");
                _claimAction.ClickOnDeleteAllFlagsIconOnFlagLine();
                VerifyReasonCodeForFLags(flagList, userAction[0],_claimAction);
                _claimAction.ClickOnRestoreIconOnFlaggedLinesRowsByRow(claimSeq);
                VerifyReasonCodeForFLags(flagList, userAction[1],_claimAction);

                StringFormatter.PrintMessage("Verify reason code verified when all flags are deleted");
                _claimAction.ClickOnDeleteAllFlagsIcon();
                VerifyReasonCodeForFLags(flagList, userAction[0],_claimAction);
            }


        }

        [Test, Category("NewClaimAction3")]//TE-803
        public void Verify_when_user_deletes_all_flags_on_a_claim_Next_icon_is_disabled_and_alert_is_shown_trying_to_leave()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                ClaimActionPage _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(_claseq);
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                string claimSeq = paramLists["ClaimSequence"];
                _claimAction.RestoreAllFlagByClaimSequence(claimSeq);
                SearchByClaimSeqFromWorkList(claimSeq,_claimAction);


                try
                {

                    //If Restore All Flag icon is present instead, click on it to change to get Delete All.
                    if (_claimAction.IsRestoreButtonPresent())
                        _claimAction.ClickOnRestoreAllFlagsIcon();

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
                        CheckApplicationAlertMessageThenDismiss(_claimAction);
                    }

                    //Dashboard Icon
                    _claimAction.ClickOnlyDashboardIcon();
                    CheckApplicationAlertMessageThenDismiss(_claimAction);
                    _claimAction.IsClaimLockedByClaimSequence(ClientEnum.SMTST.ToString(), claimSeq.Split('-')[0],
                        claimSeq.Split('-')[1],
                        automatedBase.EnvironmentManager.ClientUserName).ShouldBeTrue($"Is claim {claimSeq} locked?");

                    _claimAction.ClickOnPageFooter();

                    //Search Icon
                    _claimAction.ClickClaimSearchIcon();
                    CheckApplicationAlertMessageThenDismiss(_claimAction);
                    _claimAction.IsClaimLockedByClaimSequence(ClientEnum.SMTST.ToString(), claimSeq.Split('-')[0],
                        claimSeq.Split('-')[1],
                        automatedBase.EnvironmentManager.ClientUserName).ShouldBeTrue($"Is claim {claimSeq} locked?");

                    //Search From Claim Action
                    if (!_claimAction.IsWorkListControlDisplayed())
                        _claimAction.ClickWorkListIcon();
                    _claimAction.ClickSearchIcon().SearchByClaimSequence("12345");
                    CheckApplicationAlertMessageThenDismiss(_claimAction);
                    _claimAction.IsClaimLockedByClaimSequence(ClientEnum.SMTST.ToString(), claimSeq.Split('-')[0],
                        claimSeq.Split('-')[1],
                        automatedBase.EnvironmentManager.ClientUserName).ShouldBeTrue($"Is claim {claimSeq} locked?");

                    //Create Worklist From Claim Action
                    if (!_claimAction.IsWorkListControlDisplayed())
                        _claimAction.ClickWorkListIcon();
                    _claimAction.ToggleWorkListToPci();
                    _claimAction.ClickOnCreateButton();
                    CheckApplicationAlertMessageThenDismiss(_claimAction);
                    _claimAction.IsClaimLockedByClaimSequence(ClientEnum.SMTST.ToString(), claimSeq.Split('-')[0],
                        claimSeq.Split('-')[1],
                        automatedBase.EnvironmentManager.ClientUserName).ShouldBeTrue($"Is claim {claimSeq} locked?");


                    //Help Icon
                    _claimAction.ClickOnlyHelpIcon();
                    CheckApplicationAlertMessageThenDismiss(_claimAction);
                    _claimAction.IsClaimLockedByClaimSequence(ClientEnum.SMTST.ToString(), claimSeq.Split('-')[0],
                        claimSeq.Split('-')[1],
                        automatedBase.EnvironmentManager.ClientUserName).ShouldBeTrue($"Is claim {claimSeq} locked?");

                    //Browser Back Button
                    _claimAction.WaitForStaticTime(500);
                    _claimAction.ClickOnBrowserBackButtonToClaimSearch();
                    CheckApplicationAlertMessageThenDismiss(_claimAction);
                    _claimAction.IsClaimLockedByClaimSequence(ClientEnum.SMTST.ToString(), claimSeq.Split('-')[0],
                        claimSeq.Split('-')[1],
                        automatedBase.EnvironmentManager.ClientUserName).ShouldBeTrue($"Is claim {claimSeq} locked?");

                    _claimAction.ClickOnRestoreAllFlagsIcon();
                    var _claimSearch = _claimAction.ClickClaimSearchIcon();
                    _claimSearch.IsClaimLocked(ClientEnum.SMTST.ToString(), claimSeq.Split('-')[0],
                        claimSeq.Split('-')[1],
                        automatedBase.EnvironmentManager.HciAdminUsername3).ShouldBeFalse($"Is claim {claimSeq} locked?");


                }
                finally
                {
                    _claimAction.RestoreAllFlagByClaimSequence(claimSeq);
                }

            }
        }

        [Test] //TE-752
        public void Verify_red_badge_over_the_preauth_icon_represents_the_number_of_preauth_records_existing_for_patient_client()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                ClaimActionPage _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(_claseq);
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestExtensions.TestName);
                _claimAction.UpdateProductStatusForClient(ProductEnum.DCA.ToString(), ClientEnum.SMTST.ToString(),
                    true);

                try
                {
                    StringFormatter.PrintMessage("Verify Pre-Auth icon is present for DCA Active Client");
                    SearchByClaimSeqFromWorkList(paramLists["ClaimSequence"],_claimAction);
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

                    SearchByClaimSeqFromWorkList(paramLists["ClaimSequenceWithoutPreAuth"],_claimAction);
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
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                ClaimActionPage _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(_claseq);
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestExtensions.TestName);
                try
                {
                    StringFormatter.PrintMessage(
                        "Verify DX codes view are displayed by default when products other than DCA are enabled");
                    _claimAction.GetProductStatusForClient("DCI", ClientEnum.SMTST.ToString())
                        .ShouldBeTrue("DCA product active for SMTST client");
                    _claimAction.Refresh();
                    _claimAction.GetDefaultSecondaryView().ShouldBeEqual("Dx Codes", "DX codes  displayed by default");

                    StringFormatter.PrintMessage(
                        "Verify Appeals view are displayed by default when only DCA is enabled");
                    _claimAction.EnableOnlyDCIForClient(ClientEnum.TTREE.ToString());

                    var _claimSearch = _claimAction.ClickOnSwitchClient().SwitchClientTo(ClientEnum.TTREE, true)
                        .NavigateToClaimSearch();
                    _claimAction =
                        _claimSearch.SearchByClaimSequenceToNavigateToClaimActionPage(paramLists["ClaimSequence"]);
                    _claimAction.GetDefaultSecondaryView()
                        .ShouldBeEqual("Appeals", "Appeals view displayed by default");


                    StringFormatter.PrintMessage(
                        "Verify when appeal icon is disabled for client, Dx codes is displayed by default");
                    _claimAction.UpdateHideAppeal(true, ClientEnum.TTREE.ToString());
                    _claimAction.Refresh();
                    _claimAction.GetDefaultSecondaryView()
                        .ShouldBeEqual("Dx Codes", "Dx code view displayed by default");
                }
                finally
                {
                    _claimAction.RestoreProductForClients(ClientEnum.TTREE.ToString());
                    _claimAction.UpdateHideAppeal(false, ClientEnum.TTREE.ToString());
                }
            }

        }

        [Test] //CAR-3103(CAR-3048)
        public void Verify_FlagNotes_Icon_In_Claim_Action_Page()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                string claimSeq = paramLists["ClaimSequence"];
                List<string> flagNoteslabel = paramLists["NotesLabel"].Split(',').ToList();
                ClaimActionPage _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(claimSeq);
                try
                {
                    _claimAction.IsClaimLineNotesEnabled().ShouldBeTrue("Claim Flag note icon enabled?");

                    _claimAction.GetClaimFlagNotesIconTooltip().ShouldBeEqual("Claim Flag Notes", "tooltip correct?");
                    var flagDetails = _claimAction.GetClaimFlagNotesFromDb(claimSeq, false);
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

                    StringFormatter.PrintMessage("Verification user can swap between icons");
                    _claimAction.ClickClaimLineIconInClaimLines();
                    _claimAction.IsClaimLineNotesEnabled().ShouldBeFalse("Claim Flag note icon enabled?");

                    StringFormatter.PrintMessage(
                        "verify claim flag notes icon is not present for client with DCA Disabled");
                    automatedBase.QuickLaunch = _claimAction.ClickOnSwitchClient().SwitchClientTo(ClientEnum.RPE);
                    _claimAction = automatedBase.QuickLaunch.NavigateToCVClaimsWorkList();
                    _claimAction.IsClaimFlagNoteDisplayed().ShouldBeFalse("Claim flag notes should not be displayed");
                }
                finally
                {
                    _claimAction.SwitchClientTo(ClientEnum.SMTST);
                }
            }
        }


        [Test, Category("OnDemand")] //CAR-3102(CAR-3021)
        [Author("Pujan Aryal")]
        public void Verify_For_PCA_And_CVP_Real_Time_Clients_DX_Codes_Shown_Have_Dx_Lvl_is_L()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                ClaimActionPage _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(_claseq);
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                string claimSeq = paramLists["ClaimSequence"];
                string lineNo = paramLists["LineNo"];
                var dxCodes = paramLists["DxCodes"].Split(',').ToList();
                Random rnd = new Random();
                var processingTypes = new List<string>()
                {
                    "R",
                    "PR"
                };

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

        [Test] //CAR-3122(CAR-3105)
        public void Verify_Scource_Displayed_For_Flagged_Lines()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                string claimSeq = paramLists["ClaimSequence"];
                ClaimActionPage _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(claimSeq);
                var flagList = _claimAction.GetAllFlagsFromWorklist();
                foreach (var flag in flagList)
                {
                    if (_claimAction.IsFlagDental(flag))
                    {
                        _claimAction.IsSourcePresentForFlag(flag).ShouldBeFalse("Source value Present?");
                    }
                    else
                    {
                        _claimAction.IsSourcePresentForFlag(flag).ShouldBeTrue("Source value Present?");
                    }
                }
            }
        }

        #endregion

        #region Private Methods

        void CheckApplicationAlertMessageThenDismiss(ClaimActionPage _claimAction)
        {
            _claimAction.GetPageErrorMessage()
                .ShouldBeEqual("Claims that have been modified must be APPROVED prior to leaving the page.",
                    "Correct Warning Message Displayed?");
            _claimAction.ClosePageError();
        }

        private void VerifyReasonCodesBasedOnAction(string action, ClaimActionPage _claimAction,bool claimlineoption = true)
        {
            List<string> expectedReasonCodes;
            if (claimlineoption)
                expectedReasonCodes = _claimAction.GetExpectedReasonCodesForAllFlagsOnClaimOption(action,false);
            else
                expectedReasonCodes = _claimAction.GetExpectedReasonCodesForAllFlagsOnTheLine(action,false);

            expectedReasonCodes.Insert(0, "");
            _claimAction.GetEditReasonCodeOptions()
                .ShouldCollectionBeEqual(expectedReasonCodes, "Edit Reason Code Options should match");

        }

        private void VerifyReasonCodesBasedOnProductFlagAndAction(Dictionary<string,string> flagProductPair, string action,ClaimActionPage _claimAction)
        {
            StringFormatter.PrintMessage($"Verify Reason Codes for {flagProductPair.Values.FirstOrDefault()} flag of {flagProductPair.Keys.FirstOrDefault()} product");
            List<string> expectedReasonCodesForAParticularFlag = _claimAction.GetExpectedReasonCodesForAParticularFlag(flagProductPair.Keys.FirstOrDefault(),action,false);
            expectedReasonCodesForAParticularFlag.Insert(0, "");
            _claimAction.GetEditReasonCodeOptions()
                .ShouldCollectionBeEqual(expectedReasonCodesForAParticularFlag, "Edit Reason Code Options");

        }

        private void VerifyReasonCodeForFLags(List<string> FLaggedLines,string action, ClaimActionPage _claimAction,string linNo="1")
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


        private void ValidateDentalProfilerIcon(string claseq, ClaimActionPage _claimAction,bool dci_active = true)
        {
            if (dci_active && _claimAction.GetDentalProfilerReview(claseq.Split('-')[0], claseq.Split('-')[1])
                    .Equals("T"))
            {
                _claimAction.IsDentalProfilerIconPresent().ShouldBeTrue("Dental Profiler Icon present?");
                _claimAction.GetTooltipMessageForDentalIcon()
                    .ShouldBeEqual("Provider Specific Review", "Is Tooltip correct?");

            }
            else

                _claimAction.IsDentalProfilerIconPresent().ShouldBeFalse("Dental Profiler Icon present?");


        }

        void SearchByClaimSeqFromWorkList(string claimSeq, ClaimActionPage _claimAction,bool handlePopup = true)
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
            //_claimAction.ClickWorkListIcon();
            _claimAction.WaitForCondition(() => !_claimAction.IsWorkListControlDisplayed(), 3000);
        }

        private void VerifyLockMessageWithDisabledCreateAppealIcon(string claimSeq,string status,string lockMessage,ClaimActionPage _claimAction)
        {
            _claimAction.ClickWorkListIcon();
            _claimAction.SearchByClaimSequence(claimSeq);
            _claimAction.ClickOnViewAppealIcon();
            _claimAction.GetAppealStatusByRow(1).ShouldBeEqual(status,
                string.Format("Status of appeal for claim should  be equal to {0}",status));

            _claimAction.IsAddAppealIconDisabled()
                .ShouldBeTrue(string.Format("Add appeal icon should be disabled for claims with {0} appeal.", status));
            _claimAction.IsClaimLocked()
                .ShouldBeTrue(string.Format("Claim lock icon should be present for claims with {0} appeal.", status));
            _claimAction.GetLockIConTooltip()
                .ShouldBeEqual(lockMessage, "Lock icon tool tip should be present and equal");
        }

        protected void CheckTestClientAndSwitch(QuickLaunchPage quickLaunch,IEnvironmentManager environmentManager,NewDefaultPage currentPage)
        {

            if (!quickLaunch.IsDefaultTestClientForEmberPage(
                environmentManager.TestClient))
            {
                currentPage.ClickOnSwitchClient().SwitchClientTo(environmentManager.TestClient);
            }

        }

        private void VerifyLockMessageWithEnabledCreateAppealIcon(string claimSeq, string status,ClaimActionPage _claimAction)
        {
            _claimAction.ClickWorkListIcon();
            _claimAction.SearchByClaimSequence(claimSeq);
            _claimAction.ClickOnViewAppealIcon();
            _claimAction.GetAppealStatusByRow(1).ShouldBeEqual(status,
                string.Format("Status of appeal for claim should  be equal to {0}", status));
            _claimAction.IsAddAppealIconEnabled()
                .ShouldBeTrue(string.Format("Add appeal icon should be enabled for claims with {0} appeal.", status));
            if(status!=AppealStatusEnum.Closed.GetStringDisplayValue())
                _claimAction.IsClaimLocked()
                .ShouldBeTrue(string.Format("Claim lock icon should be present for claims with {0} appeal.", status));
        }

        void SearchByClaimSeqFromWorkListForClientUser(string claimSeq,ClaimActionPage _claimAction)
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
            //_claimAction.ClickWorkListIcon();
        }

        

        void SearchByClaimSeqFromWorkListForInternalUser(string claimSeq,ClaimActionPage _claimAction)
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
            _claimAction.WaitForCondition(() => !_claimAction.IsWorkListControlDisplayed());
        }


        private void ValidateMaxCharacterLimitOnNotes(string notes,ClaimActionPage _claimAction)
        {
            _claimAction.SetLengthyNoteToVisbleTextarea("note",notes);
            _claimAction.GetNoteOfVisibleTextarea().Length.ShouldBeEqual(1993,
                "Character limit should be 1993 or less, where 7 characters are separated for <p></p> tag.");
            _claimAction.ClickOnCancelLink();
        }


       

        #endregion
    }
}

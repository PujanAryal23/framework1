using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using Nucleus.Service.Data;
using Nucleus.Service.PageObjects.Claim;
using Nucleus.Service.PageServices.Appeal;
using Nucleus.Service.PageServices.Claim;
using Nucleus.Service.Support.Common.Constants;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Environment;
using Nucleus.UIAutomation.TestSuites.Base;
using NUnit.Framework;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Utils;
using Nucleus.Service.PageServices.QuickLaunch;

namespace Nucleus.UIAutomation.TestSuites.TestCases
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class ClaimAction2
    {
        #region Old Non-Parallel Code Commented Out
        /*
        #region PRIVATE FIELDS


        private ClaimActionPage _claimAction;
        private AppealManagerPage _appealManager;
        private string claimSeq;

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
                UserLoginIndex = 2;
                base.ClassInit();
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                claimSeq = paramLists["ClaimSequence"];
                //automatedBase.QuickLaunch.Logout().LoginAsHciAdminUser2();
                _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(claimSeq);
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
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
            if (string.Compare(UserType.CurrentUserType, UserType.HCIADMIN2, StringComparison.OrdinalIgnoreCase) != 0)
            {
                _claimAction = _claimAction.Logout()
                    .LoginAsHciAdminUser2().ClickOnSwitchClient().SwitchClientTo(EnvironmentManager.TestClient).
                    NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(claimSeq);
            }
            else if (!automatedBase.CurrentPage.GetPageHeader().Equals(PageHeaderEnum.ClaimAction.GetStringValue()))
            {
                automatedBase.CurrentPage = _claimAction = automatedBase.CurrentPage.NavigateToClaimSearch().
                    SearchByClaimSequenceToNavigateToClaimActionPage(claimSeq);
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
                //if (automatedBase.CurrentPage.IsautomatedBase.QuickLaunchIconPresent())
                //    _claimAction.GoToautomatedBase.QuickLaunch();
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

        [Test] //CAR-3260(CAR-3207)
        [Author("ShreyaS")]
        public void Verify_lock_icon_for_dental_review()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(2))
            {
                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestName);
                var claSeqs = paramLists["ClaimSeqs"].Split(',').ToList();
                ClaimSearchPage _claimSearch = automatedBase.CurrentPage.NavigateToClaimSearch();
                _claimSearch.GetCommonSql.GetAppealTypeByClaseqDb(claSeqs[0]).ShouldBeEqual("D","Appeal type should be Dental");

                StringFormatter.PrintMessage("Verification of lock icon for Dental Appeal in Claim action page");
                ClaimActionPage _claimAction = _claimSearch.SearchByClaimSequenceToNavigateToClaimActionPage(claSeqs[0],true);
                _claimAction.IsLocked().ShouldBeFalse("Lock icon should not be present for Dental appeal in Claim Action page");
                _claimAction.ClickOnBrowserBack();

                StringFormatter.PrintMessage("Verification of lock icon for Dental Appeal in Claim Search page");
                _claimSearch.IsClaimLockPresentForClaimSequence(claSeqs[0]).ShouldBeFalse("Lock icon should not be present for Dental appeal in Claim Search page");

                StringFormatter.PrintMessage("Verification of lock icon for non-dental Appeal in Claim Action page");
                _claimSearch.SearchByClaimSequenceToNavigateToClaimActionPage(claSeqs[1],true);
                _claimSearch.GetCommonSql.GetAppealTypeByClaseqDb(claSeqs[1]).ShouldBeEqual("A", "Appeal type should be Appeal");
                _claimAction.IsLocked().ShouldBeTrue("Lock icon should be present for Non-Dental appeal in Claim Action page");

                StringFormatter.PrintMessage("Verification of lock icon for non-dental Appeal in Claim search page");
                _claimAction.ClickOnBrowserBack();
                _claimSearch.IsClaimLockPresentForClaimSequence(claSeqs[1]).ShouldBeTrue("Lock icon should be present for Non-Dental appeal in Claim Search page");

                StringFormatter.PrintMessage("Verification of lock icon for a claim with both dental and non-dental Appeal in Claim search page");
                _claimSearch.SearchByClaimSequenceToNavigateToClaimActionPage(claSeqs[2],true);
                _claimAction.ClickOnViewAppealIcon();
                _claimAction.GetListOfAppealTypesInRows().ShouldCollectionBeEqual(new List<string>() { "Appeal", "Dental Review" }, "Claim Should Have Appeals with both Dental and Non-Dental type");
                _claimAction.IsLocked().ShouldBeTrue("Lock icon should be present for a claim with both dental and non-dental Appeal in Claim search page");

                StringFormatter.PrintMessage("Verification of lock icon for a claim with both dental and non-dental Appeal in Claim search page");
                _claimAction.ClickOnBrowserBack();
                _claimSearch.IsClaimLockPresentForClaimSequence(claSeqs[2]).ShouldBeTrue("Lock icon should be present for a claim with both dental and non-dental Appeal in Claim search page");
            }
        }

        [Test] //CAR-3241(CAR-3227)
        [Author("ShreyaS")]
        [NonParallelizable]
        public void Verify_DCI_flag_Note_visibility_to_client()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(2))
            {
                ClaimActionPage _claimAction;
                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestName);
                var claSeq = paramLists["ClaimSeq"].Split(',').ToList();
                var flag = paramLists["Flag"];
                var _claimSearch = automatedBase.CurrentPage.NavigateToClaimSearch();
                _claimAction = _claimSearch.SearchByClaimSequenceToNavigateToClaimActionPage(claSeq[0], true);

                try
                {
                    StringFormatter.PrintMessage("Deleting line flag audit");
                    _claimAction.DeleteLineFlagAuditByClaimSequence(claSeq[0], ClientEnum.SMTST.ToString());
                    _claimAction.DeleteLineFlagAuditByClaimSequence(claSeq[1], ClientEnum.SMTST.ToString());

                    StringFormatter.PrintMessage("Verification of adding note before releasing claim to client");
                    _claimAction.UpdateClaimStatus(claSeq[0], ClientEnum.SMTST.ToString(), "U");
                    _claimAction.Refresh();
                    _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                    SetReasonNote();
                    _claimAction.UpdateClaimStatus(claSeq[0], ClientEnum.SMTST.ToString(), "U", 'T');

                    StringFormatter.PrintMessageTitle("Verification of adding note after releasing claim to client");
                    _claimAction.ClickWorkListIcon().ClickonFindOption().SearchByClaimSequence(claSeq[1]);
                    _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                    SetReasonNote();

                    StringFormatter.PrintMessage("Logging in as client user");
                    _claimSearch = _claimAction.Logout().LoginAsClientUser().NavigateToClaimSearch();
                    _claimAction = _claimSearch.SearchByClaimSequenceToNavigateToClaimActionPage(claSeq[0]);
                    StringFormatter.PrintMessageTitle("Verification notes added prior to releasing claim to client is visible in client view");
                    VerifyDCIFlagNotes();
                    _claimAction.ClickWorkListIcon().ClickonFindOption().SearchByClaimSequence(claSeq[1]);
                    StringFormatter.PrintMessageTitle("Verification notes added after releasing claim to client is visible in client view");
                    VerifyDCIFlagNotes();

                }

                finally
                {
                    StringFormatter.PrintMessage("Deleting line flag audit");
                    _claimAction.DeleteLineFlagAuditByClaimSequence(claSeq[0], ClientEnum.SMTST.ToString());
                    _claimAction.DeleteLineFlagAuditByClaimSequence(claSeq[1], ClientEnum.SMTST.ToString());
                    _claimAction.UpdateClaimStatus(claSeq[0], ClientEnum.SMTST.ToString(), "U");
                }

                #region Private Methods
                void VerifyDCIFlagNotes()
                {
                    _claimAction.ClickOnClaimFlagNotesIcon();
                    _claimAction.GetClaimFlagNoteDetailListByFlagAndLabel(flag, "Notes").ShouldBeEqual("DCI flag note", "Flag note added by internal user should be visible to client in Claim Flag notes");
                    _claimAction.ClickOnClaimFlagAuditHistoryIcon();
                    _claimAction.GetClaimFlagAuditHistoryDetailByFlagAndRowAndLabelAndLineNo(flag, "Action", "1")
                        .ShouldBeEqual("Reason", "Action should be reason");
                    _claimAction.GetClaimFlagAuditHistoryDetailByFlagAndRowAndLabelAndLineNo(flag, "User Type", "1")
                        .ShouldBeEqual("Internal", "Note should be added by Internal user");
                    _claimAction.GetClaimFlagAuditHistoryDetailByFlagAndRowAndLabelAndLineNo(flag, "Notes", "1")
                        .ShouldBeEqual("DCI flag note", "Note added by internal user should be visible to client users when Reason action is performed");
                }
                void SetReasonNote()
                {
                    _claimAction.ClickOnEditForGivenFlag(flag);
                    _claimAction.SelectReasonCode("ACC 1 - Accept Response To Logic Request");
                    _claimAction.WaitForStaticTime(3000);
                    _claimAction.SetNoteInEditFlagNoteField("DCI flag note");
                    _claimAction.WaitForStaticTime(1000);
                    _claimAction.GetSideWindow.Save();
                    _claimAction.WaitForWorking();
                }
                #endregion
            }
        }

        [Test, Category("NewClaimAction2")] //US50620
        public void Verify_CIU_Referral_will_be_viewable_in_provider_details_on_every_claim_for_that_provider_which_created_via_provider_action()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(2))
            {
                ClaimActionPage _claimAction;

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var claimSequenceList = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "ClaimSequenceList", "Value");

                var _claimSearch = automatedBase.CurrentPage.NavigateToClaimSearch();


                foreach (var claimSequence in claimSequenceList.Split(';'))
                {
                    _claimAction = _claimSearch.SearchByClaimSequenceToNavigateToClaimActionPage(claimSequence);
                    
                    _claimAction.ClickonProviderDetailsIcon();
                    _claimAction.GetCiuReferralRecordRowCount()
                        .ShouldBeEqual(3,
                            "CIU Referral Should Display and equal to 3 due to 2 are created from Provider Action and 1 from Claim Action");

                    _claimAction.ClickClaimSearchIcon();
                }
            }
        }


        [Test, Category("NewClaimAction2")] //us39865
        public void Verify_that_client_specified_data_points_3rd_row_in_the_top_left_quadrant_when_user_moving_from_claim_to_claim()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(2))
            {
                ClaimActionPage _claimAction;

                _claimAction = automatedBase.CurrentPage.NavigateToCVClaimsWorkList();

                var preClaimSeq = _claimAction.GetClaimSequence();
                var preHciRun = _claimAction.GetHciRunValue();
                var preHciVoid = _claimAction.GetHciVoidValue();
                var preProvSpec = _claimAction.GetProvSpecValue();
                var prePrvoName = _claimAction.GetProvNameValue();

                for (var i = 1; i <= 5; i++)
                {
                    _claimAction.ClickOnNextButton();
                    _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                    StringFormatter.PrintMessageTitle("Iteration <" + i + ">");
                    Console.WriteLine("compare claim Sequence between current <{0}> and previous <{1}>",
                        _claimAction.GetClaimSequence(), preClaimSeq);

                    var newHciRun = _claimAction.GetHciRunValue();
                    var newHciVoid = _claimAction.GetHciVoidValue();
                    var newProvSpec = _claimAction.GetProvSpecValue();
                    var newProvName = _claimAction.GetProvNameValue();

                    if (newHciRun == preHciRun && newHciVoid == preHciVoid && newProvSpec == preProvSpec &&
                        newProvName == prePrvoName)
                    {
                        _claimAction.Refresh();
                        _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                        _claimAction.WaitForCondition(JavaScriptExecutor.ExecuteJqueryStatus);
                        _claimAction.Wait();
                        _claimAction.GetHciRunValue()
                            .ShouldBeEqual(newHciRun, "HCIRUN should be same after reload the page");
                        _claimAction.GetHciVoidValue()
                            .ShouldBeEqual(newHciVoid, "HCIVOID  should be same after reload the page");
                        _claimAction.GetProvSpecValue().ShouldBeEqual(newProvSpec,
                            "PROV SPEC should be same after reload the page");
                        _claimAction.GetProvNameValue().ShouldBeEqual(newProvName,
                            "PROV NAME should be same after reload the page");
                        break;
                    }

                    Console.WriteLine("\n Previous Data and New Data : \n <HCIRUN>:<{0}>\t<{1}> \n <HCIVOID>:<{2}>\t<{3}> \n <ProvSpec>:<{4}>\t<{5}> \n <ProvName>:<{6}>\t<{7}>",
                        preHciRun, newHciRun, preHciVoid, newHciVoid, preProvSpec, newProvSpec, prePrvoName,
                        newProvName);


                    preClaimSeq = _claimAction.GetClaimSequence();

                }
            }
        }



        [Test, Category("NewClaimAction2")]
        public void Verify_that_if_trigger_claim_is_pended_and_user_clicks_on_switch_edit_appropriate_message_is_shown()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(2))
            {
                ClaimActionPage _claimAction;

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                string claimSeq = paramLists["ClaimSequence"];

                _claimAction = automatedBase.CurrentPage.NavigateToClaimSearch()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(claimSeq);

                try
                {
                    _claimAction.ClickSwitchEditIcon();
                    _claimAction.IsPageErrorPopupModalPresent().ShouldBeTrue("Page error popup present?");
                    _claimAction.GetPageErrorMessage()
                        .ShouldBeEqual(
                            "The flag cannot be switched. The trigger claim is currently in a pended state.");
                    _claimAction.ClosePageError();
                }
                finally
                {
                    if (_claimAction.IsPageErrorPopupModalPresent())
                        _claimAction.ClosePageError();
                }
            }
        }



        [Test, Category("NewClaimAction2")] //US8583
        public void
            Verify_Original_Dx_dates_appear_correctly_in_Dx_Code_container_when_original_date_not_equal_DOS_on_lines()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(2))
            {
                ClaimActionPage _claimAction;
                
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> param =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                string claimSeq = param["ClaimSequence"];
                string originalDxDate = param["OriginalDxDate"];

                _claimAction = automatedBase.CurrentPage.NavigateToClaimSearch()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(claimSeq);

                StringFormatter.PrintMessageTitle("Verify Original DX Dates appear correctly");
                _claimAction.ClickOnViewDxCodesIcon();
                _claimAction.GetFirstOrignialDxDate().ShouldBeEqual(originalDxDate, "Original Dx Date");
                StringFormatter.PrintLineBreak();
                StringFormatter.PrintMessageTitle("Original Dx Date not equal to DOS on claim lines");
                _claimAction.GetFirstEndDosOnClaimLine().ShouldNotBeTheSameAs(originalDxDate,
                    "Original Dx Date should not be same as Dos on line");
                StringFormatter.PrintLineBreak();
            }
        }


        [Test, Category("NewClaimAction2")] //US9606
        public void Verify_that_PS_flag_is_added_to_the_trigger_line_should_appear_grey_and_user_should_not_be_able_to_delete_or_restore_the_PS_edit_line()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(2))
            {
                ClaimActionPage _claimAction;
                
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> param =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                string claimSeq = param["ClaimSequence"];

                _claimAction = automatedBase.CurrentPage.NavigateToClaimSearch()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(claimSeq);

                /* PS flag is added to trigger line should appear grey in color*/
                StringFormatter.PrintMessageTitle("PS flag is added to trigger line should appear grey in color");
                string trigLineNumForPsEdit = _claimAction.GetTriggerLineForEditMpr();
                _claimAction.GetEditPsColor(trigLineNumForPsEdit)
                    .ShouldBeEqual("rgba(128, 128, 128, 1)", "PS edit color should appear grey");
                StringFormatter.PrintLineBreak();

                /*User should not be able to be able to delete or restore the PS edit line*/
                StringFormatter.PrintMessageTitle("User should not be able to delete or restore the PS edit line");
                string status = _claimAction.GetStatusOfLineAction(trigLineNumForPsEdit)[1];
                Console.Out.WriteLine(status);

                status.Contains("is_disabled")
                    .ShouldBeTrue("User should not be able to be able to delete or restore the PS edit line");
                StringFormatter.PrintLineBreak();
            }
        }



        [Test, Category("NewClaimAction2")]
        public void Verify_proc_description_appears_on_Flagged_lines_Claim_lines_and_Claim_Dollars_Details()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(2))
            {
                ClaimActionPage _claimAction;
                
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                string claimSeq = paramLists["ClaimSequence"];
                string procDescFlaggedLines = paramLists["ProcDescFlaggedLines"];
                string procDescFlagRow = paramLists["ProcDescFlagRow"];
                string procDescClaimLines = paramLists["ProcDescClaimLines"];
                string procDescDollarDetails = paramLists["ProcDescDollarDetails"];

                _claimAction = automatedBase.CurrentPage.NavigateToClaimSearch()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(claimSeq);
                
                try
                {
                    _claimAction.IsProcDescPresentInFlaggedLines()
                        .ShouldBeTrue("Proc Description is present in Flagged Lines.");
                    _claimAction.GetProcDescriptionTextOfFlaggedLines()
                        .ShouldBeEqual(procDescFlaggedLines, "Proc Desc Text in Flagged Linse");
                    _claimAction.IsFlagDescPresentInFlaggedLinesDiv()
                        .ShouldBeTrue("Flag Description is present in Flag row of Flagged Lines.");
                    _claimAction.GetFlagDescriptionText().ShouldBeEqual(procDescFlagRow, "Flag Desc Text");
                    _claimAction.IsProcDescPresentInClaimLinesDiv()
                        .ShouldBeTrue("Proc Description is present in Claim Lines.");
                    _claimAction.GetProcDescriptionTextOfClaimLinesDiv().ShouldBeEqual(procDescClaimLines,
                        "Proc Desc Text in Claim Lines");
                    _claimAction.ClickOnDollarIcon();
                    _claimAction.IsProcDescPresentInClaimDollarDetailsDiv()
                        .ShouldBeTrue("Proc Description is present in Claim Dollar Details.");
                    _claimAction.GetProcDescriptionTextOfClaimDollarDetailsDiv().ShouldBeEqual(procDescDollarDetails,
                        "Proc Desc Text in Dollar Details");
                }
                finally
                {
                    if (_claimAction.IsClaimLinesPresent())
                        _claimAction.ClickOnLinesIcon();
                }
            }
        }

        //  [Test, Category("NewClaimAction2")]//US14402
        public void
            Verify_that_the_view_notes_icon_will_be_displayed_on_Claim_Action_Page_if_notes_are_present_for_Cotiviti_User()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(2))
            {
                ClaimActionPage _claimAction;
                
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                string claimSeqWithNote = paramLists["ClaimSequenceWithNote"];

                _claimAction = automatedBase.CurrentPage.NavigateToClaimSearch()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(claimSeqWithNote);
                
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

        [Test, Category("NewClaimAction2")] //US65703 + CAR2043
        public void
            Verify_that_the_add_notes_icon_will_be_displayed_on_Claim_Action_Page_if_notes_are_not_present_for_Cotiviti_User()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(2))
            {
                ClaimActionPage _claimAction;
                
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var claimSeqWithoutNote = paramLists["ClaimSequenceWithoutNote"];

                _claimAction = automatedBase.CurrentPage.NavigateToClaimSearch()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(claimSeqWithoutNote);

                _claimAction.IsAddNoteIndicatorPresent().ShouldBeTrue("Is Add note indicator present");
                _claimAction.IsRedBadgeNoteIndicatorPresent().ShouldBeFalse("Badge indicator should not be present");
                _claimAction.ClickOnClaimNotes();
                _claimAction.IsAddNoteFormPresent().ShouldBeTrue("Add Note Form should be present");
                _claimAction.IsAddIconDisabled().ShouldBeTrue("Add Note Indicator should be disabled.");
                var noOfNotes = _claimAction.GetNoteListCount();
                (noOfNotes > 0).ShouldBeFalse("The given claim sequence has notes should be false.");
                _claimAction.ClickOnAddNoteCancelLink();
                _claimAction.IsAddNoteFormPresent()
                    .ShouldBeFalse("Add Note Form should not be present after clicking the Cancel button");
                _claimAction.IsNoteContainerPresent().ShouldBeFalse("Note Container should not be present");

            }
        }

        [Test, Category("NewClaimAction2")]//US65703 + US65739 + CAR2043
        public void Verify_red_badge_over_the_note_icon_represents_the_number_of_claim_type_notes_only_that_exist()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(2))
            {
                ClaimActionPage _claimAction;
                
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var claimSequenceWithBothClaimAndPrvNotes = paramLists["ClaimSequenceWithBothClaimAndPrvNotes"];
                var claimSequenceWithPrvNotesOnly = paramLists["ClaimSequenceWithPrvNotesOnly"];
                var prvseq = paramLists["PrvSeq"];
                var patseq = paramLists["PatSeq"];

                _claimAction = automatedBase.CurrentPage.NavigateToClaimSearch()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(claimSequenceWithBothClaimAndPrvNotes);

                var totalClaimNoteCount =
                    _claimAction.TotalCountofClaimAndPatientNotes(claimSequenceWithBothClaimAndPrvNotes, patseq);
                var totalNoteCount =
                    Convert.ToInt32(_claimAction.TotalCountOfNotes(claimSequenceWithBothClaimAndPrvNotes, prvseq,
                        patseq));
                var totalProviderNoteCount = Convert.ToInt32(_claimAction.TotalCountofProviderNotes(prvseq));

                _claimAction.IsRedBadgeNoteIndicatorPresent().ShouldBeTrue("Is red badge note indicator present");
                _claimAction.NoOfClaimNotes().ShouldBeEqual(totalClaimNoteCount,
                    "Red Badge shows Claim and Patient notes only and not Provider notes");
                _claimAction.ClickOnClaimNotes();
                _claimAction.GetNoteListCount().ShouldBeEqual(totalNoteCount,
                    "Notes list should display both claim and provider notes");
                SearchByClaimSeqFromWorkList(_claimAction, claimSequenceWithPrvNotesOnly);
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

        [Test, Category("NewClaimAction2")] //US65703 +CAR-2392(CAR2043)
        public void Verify_note_container_display_upon_note_icon_click()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(2))
            {
                ClaimActionPage _claimAction;
                
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var claimSeqWithNote = paramLists["ClaimSequenceWithNote"];
                var prvseq = paramLists["PrvSeq"];
                var patseq = paramLists["PatSeq"];
                var expectedNoteTypeList =
                    automatedBase.DataHelper.GetMappingData(FullyQualifiedClassName, "Note_type").Values.ToList();
                var userNameList =
                    automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                            "UserNameList", "Value")
                        .Split(';')
                        .ToList();
                var currentUser = userNameList[0];
                var nonCurrentUser1 = userNameList[1];
                var nonCurrentUser2 = userNameList[2];
                var expectedOutputList =
                    automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                            "ExpectedOutputList", "Value")
                        .Split(';')
                        .ToList();
                var claim = expectedOutputList[0];
                var provider = expectedOutputList[1];
                var subType = expectedOutputList[2];
                var subTypeTrigCond = expectedOutputList[3];
                var patient = expectedOutputList[4];

                _claimAction = automatedBase.CurrentPage.NavigateToClaimSearch()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(claimSeqWithNote);

                var totalClaimNoteCount = Convert.ToInt32(_claimAction.TotalCountofClaimNotes(claimSeqWithNote));
                var totalProviderNoteCount = Convert.ToInt32(_claimAction.TotalCountofProviderNotes(prvseq));
                var totalPatientNoteCount = Convert.ToInt32(_claimAction.TotalCountofProviderNotes(patseq));
                var totalNoteCount = Convert.ToInt32(_claimAction.TotalCountOfNotes(claimSeqWithNote, prvseq, patseq));

                _claimAction.ClickOnClaimNotes();
                _claimAction.IsNoteContainerPresent()
                    .ShouldBeTrue("Note Container must display after clicking note icon.");
                _claimAction.GetNoteListCount().ShouldBeEqual(totalNoteCount,
                    "Notes list should display both claim and provider notes");

                StringFormatter.PrintMessageTitle(
                    "Verification of Note Type in Notes Container Header");
                _claimAction.GetAvailableDropDownListInNoteType("Type")
                    .ShouldCollectionBeEqual(expectedNoteTypeList, "Note Type List");
                _claimAction.GetDefaultValueOfNoteTypeOnHeader("Type")
                    .ShouldBeEqual("All", "Default selection of Note Type. ");

                StringFormatter.PrintMessageTitle(
                    "Verification of Notes Record and its values when All Note Type selected");

                var totalRow = _claimAction.GetNoteListCount();
                _claimAction.GetNoteRecordByRowColumn(2, totalRow - 355).ShouldBeEqual(patient, "Patient Note Present");
                _claimAction.GetNoteRecordByRowColumn(2, totalRow - 354).ShouldBeEqual(claim, "Claim Note Present");
                _claimAction.GetNoteRecordByRowColumn(2, totalRow - 352)
                    .ShouldBeEqual(provider, "Provider Note Present");
                _claimAction.GetNoteRecordByRowColumn(3, totalRow - 350)
                    .ShouldBeEqual(subType, "Provider Note with sub type Referral Detail");
                _claimAction.GetNoteRecordByRowColumn(3, totalRow - 349).ShouldBeEqual(subTypeTrigCond,
                    "Provider Note with sub type equal to condition for which that provider has triggered. ");
                _claimAction.GetNoteRecordByRowColumn(4, totalRow - 354)
                    .DoesNameContainsOnlyFirstWithLastname()
                    .ShouldBeTrue("Created By Should be in <First Name><Last Name>");


                _claimAction.IsPencilIconPresentByName(currentUser)
                    .ShouldBeTrue("Pencil Icon must be Present when creator of Note is user itself. ");
                _claimAction.IsCarrotIconPresentByName(currentUser)
                    .ShouldBeFalse("Carrot Icon must not be Present when the note was created by the current user.");

                _claimAction.IsCarrotIconPresentByName(nonCurrentUser1).ShouldBeTrue(
                    "Carrot Icon must be Present when the note was created by a user other than the current user.");
                _claimAction.IsPencilIconPresentByName(nonCurrentUser1)
                    .ShouldBeFalse("Pencil Icon must not be Present when creator of Note is not user itself. ");
                _claimAction.GetVisibleToClientTooltipInNotesList().ShouldBeEqual("Visible to Client");

                StringFormatter.PrintMessageTitle(
                    "Verification of Search By Patient Notes");
                _claimAction.SelectNoteTypeInHeader("Type", "Patient");
                var list = _claimAction.GetNoteRecordListByColumn();
                var distinctList = list.Distinct().ToList();
                distinctList[0].ShouldBeEqual("Patient", "Only Patient Notes should be displayed.");
                distinctList.Count.ShouldBeEqual(1, "Distinct List Length");
                list.Count.ShouldBeEqual(totalPatientNoteCount,
                    "Patient note type count equal to that in the database");

                StringFormatter.PrintMessageTitle(
                    "Verification of Search By Claim Notes");
                _claimAction.SelectNoteTypeInHeader("Type", "Claim");
                list = _claimAction.GetNoteRecordListByColumn();
                distinctList = list.Distinct().ToList();
                distinctList[0].ShouldBeEqual("Claim", "Only claim Notes should be displayed.");
                distinctList.Count.ShouldBeEqual(1, "Distinct List Length");
                list.Count.ShouldBeEqual(totalClaimNoteCount, "Claim note type count equal to that in the database");

                StringFormatter.PrintMessageTitle(
                    "Verification of Search By Provider Notes");
                _claimAction.SelectNoteTypeInHeader("Type", "Provider");
                list = _claimAction.GetNoteRecordListByColumn();
                distinctList = list.Distinct().ToList();
                distinctList[0].ShouldBeEqual("Provider", "Only claim Notes should be displayed.");
                distinctList.Count.ShouldBeEqual(1, "Distinct List Length");
                list.Count.ShouldBeEqual(totalProviderNoteCount,
                    "Provider note type count equal to that in the database");

                _claimAction.IsVerticalScrollBarPresentInNoteSection()
                    .ShouldBeTrue(
                        "Scrollbar Should display in Notes Section when the list of note records extends out of the view");

                StringFormatter.PrintMessageTitle("Verification of ability to expand multiple forms");
                _claimAction.SelectNoteTypeInHeader("Type", "All");
                _claimAction.ClickOnExpandIconOnNotesByName(nonCurrentUser1);
                _claimAction.IsNoteEditFormDisplayedByName(nonCurrentUser1)
                    .ShouldBeTrue("Notes form must be expanded.");
                _claimAction.ClickOnExpandIconOnNotesByName(nonCurrentUser2);
                _claimAction.IsNoteEditFormDisplayedByName(nonCurrentUser2)
                    .ShouldBeTrue("Notes form must be expanded.");
                _claimAction.IsNoteEditFormDisplayedByName(nonCurrentUser1)
                    .ShouldBeTrue("Notes form in row 2 is still present.");
                _claimAction.GetNoteEditFormCount().ShouldBeGreater(1, "User is able to view multiple forms");

                StringFormatter.PrintMessageTitle(
                    "Click Notes Icon again to collapse Notes container");
                _claimAction.ClickOnClaimNotes();
                _claimAction.IsNoteContainerPresent()
                    .ShouldBeFalse("Note Container must collapse after clicking note icon again.");

            }
        }

        [Test, Category("NewClaimAction2")] //US65704 + CAR2043
        public void Verify_note_edit_and_view_functionality()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(2))
            {
                ClaimActionPage _claimAction;
               
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var claimSeq = paramLists["ClaimSequence"];
                var updatedNoteText = paramLists["UpdatedNoteText"];
                var userNameList =
                    automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                            "UserNameList", "Value")
                        .Split(';')
                        .ToList();
                var currentUser = userNameList[0];
                var nonCurrentUser = userNameList[1];
                _claimAction = automatedBase.CurrentPage.NavigateToClaimSearch()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(claimSeq);

                _claimAction.ClickOnClaimNotes();

                StringFormatter.PrintMessageTitle("Verification of Edit note");
                _claimAction.ClickOnEditIconOnNotesByName(currentUser);
                _claimAction.IsNoteEditFormDisplayedByName(currentUser).ShouldBeTrue("Notes Edit form displayed");
                _claimAction.IsNoteFormEditableByName(currentUser)
                    .ShouldBeTrue("Notes can be edited by current user only ");
                _claimAction.SetNoteInNoteEditorByName(updatedNoteText, currentUser, false);
                _claimAction.ClickOnSaveButtonInNoteEditorByName(currentUser);

                StringFormatter.PrintMessage("Updating Note");
                _claimAction.ClickOnEditIconOnNotesByName(currentUser);
                _claimAction.GetNoteInNoteEditorByName(currentUser)
                    .ShouldBeEqual(updatedNoteText, "Updated Note text displayed in notes form");
                int totalRow = _claimAction.GetNoteListCount();
                _claimAction.GetNoteRecordByRowColumn(5, totalRow - 1)
                    .ShouldBeEqual(DateTime.Now.ToString("MM/dd/yyyy"), "Modified date in Note list must be updated.");

                var noteText = new string('a', 3994);
                _claimAction.SetNoteInNoteEditorByName(noteText, currentUser, true);
                _claimAction.GetNoteInNoteEditorByName(currentUser).Length.ShouldBeEqual(3993, "Note Text max length");
                _claimAction.SetNoteInNoteEditorByName(string.Empty, currentUser, false); //set empty note
                _claimAction.IsInvalidInputPresentOnNote().ShouldBeTrue("Empty Note warning displayed");
                _claimAction.GetInvalidTooltipMessageOnNote()
                    .ShouldBeEqual("Note is required before the record can be saved.", "Empty Note warning tooltip.");
                _claimAction.ClickOnSaveButtonInNoteEditorByName(currentUser);
                StringFormatter.PrintMessage("Validation popup displayed");
                _claimAction.IsPageErrorPopupModalPresent().ShouldBeTrue("Notes can't be save when notes is empty.");
                _claimAction.GetPageErrorMessage()
                    .ShouldBeEqual("Invalid or missing data must be resolved before record can be saved.");
                _claimAction.ClosePageError();

                StringFormatter.PrintMessageTitle("Verification of Cancel note");
                _claimAction.SetNoteInNoteEditorByName("Cancel Note Text", currentUser, false);
                _claimAction.IsInvalidInputPresentOnNote()
                    .ShouldBeFalse("Empty Note warning should remove after enter text in text area.");
                _claimAction.ClickOnCancelButtonInNoteEditorByName(currentUser);
                _claimAction.IsNoteEditFormDisplayedByName(currentUser)
                    .ShouldBeFalse("Notes Edit form must be collasped");
                _claimAction.ClickOnEditIconOnNotesByName(currentUser);
                _claimAction.GetNoteInNoteEditorByName(currentUser).ShouldBeEqual(updatedNoteText,
                    "Notes must not be updated when Cancel button is clicked.");
                _claimAction.ClickOnCancelButtonInNoteEditorByName(currentUser);

                StringFormatter.PrintMessage("Verify Visible to CLient icon");
                VerifyVisibleToClientIconOnNoteRow(_claimAction, currentUser);

                StringFormatter.PrintMessage("Verification of Empty Notes Message.");
                _claimAction.SelectNoteTypeInHeader("Type", "Provider");
                _claimAction.IsNoNotesMessageDisplayed().ShouldBeTrue("No Notes Message displayed.");
                _claimAction.GetNoNotesMessage().ShouldBeEqual("There are no notes available");
                _claimAction.SelectNoteTypeInHeader("Type", "All");

                StringFormatter.PrintMessageTitle("Verification of Carrot Icon on each note record.");
                _claimAction.ClickOnExpandIconOnNotesByName(nonCurrentUser);
                _claimAction.IsNoteEditFormDisplayedByName(nonCurrentUser).ShouldBeTrue("Notes form must be expanded.");
                _claimAction.IsNoteFormEditableByName(nonCurrentUser).ShouldBeFalse("Notes form must not be editable");
                _claimAction.ClickOnCollapseIconOnNotesByName(nonCurrentUser);
                _claimAction.IsNoteEditFormDisplayedByName(nonCurrentUser)
                    .ShouldBeFalse("Clicking on Expand icon again must close note form.");


            }
        }

        [Test, Category("NewClaimAction2")] //US65705 + CAR2043
        public void Verify_create_new_Claim_Provider_note()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(2))
            {
                ClaimActionPage _claimAction;
                
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var claimSeqWithNote = paramLists["ClaimSequenceWithNote"];
                var currentUserFullName = paramLists["UserFullName"];

                _claimAction = automatedBase.CurrentPage.NavigateToClaimSearch()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(claimSeqWithNote);

                _claimAction.DeleteClaimNotesofNoteTypeClaimOnly(claimSeqWithNote,
                    automatedBase.EnvironmentManager.HciAdminUsername2);
                _claimAction.RefreshPage(false);

                var noOfNotes = Convert.ToInt32(_claimAction.NoOfClaimNotes());
                _claimAction.NoOfClaimNotes().ShouldBeEqual(noOfNotes.ToString(), "Number of Notes present in Claim");
                _claimAction.IsAddNoteIndicatorPresent().ShouldBeTrue("Add Note Icon Present");
                _claimAction.ClickOnClaimNotes();
                _claimAction.ClickonAddNoteIcon();
                _claimAction.IsAddNoteFormPresent()
                    .ShouldBeTrue("Add Note Form must display after clicking note icon.");
                _claimAction.IsAddIconDisabled().ShouldBeTrue("Add Icon Should be disabled");
                _claimAction.IsVisibleToClientChecked().ShouldBeTrue("Visible To Client should be checked");
                _claimAction.ClickVisibleToClient();
                _claimAction.IsVisibleToClientChecked().ShouldBeFalse("Visible To Client should be unchecked");
                _claimAction.ClickVisibleToClient();
                _claimAction.GetNameLabel().AssertIsContained(currentUserFullName, "Name of the current User");

                _claimAction.SelectNoteType("Patient");
                _claimAction.IsSubTypeDisabled().ShouldBeTrue("SubType is disabled");
                _claimAction.ClickonAddNoteSaveButton();
                _claimAction.IsPageErrorPopupModalPresent().ShouldBeTrue("Pop up message should be present");
                _claimAction.ClosePageError();
                _claimAction.IsInvalidInputPresentOnNote().ShouldBeTrue("Note Indicator should be present");
                _claimAction.GetInvalidTooltipMessageOnNote()
                    .ShouldBeEqual("Note is required before the record can be saved.", "Note tooltip warning message");
                _claimAction.ClickVisibleToClient();
                _claimAction.SetAddNote("note");
                _claimAction.ClickOnAddNoteCancelLink();

                _claimAction.ClickOnClaimNotes();
                _claimAction.SelectNoteType("Provider");
                _claimAction.IsSubTypeDisabled().ShouldBeFalse("SubType is enabled");
                _claimAction.SelectNoteSubType("Alert");
                _claimAction.ClickonAddNoteSaveButton();
                _claimAction.IsPageErrorPopupModalPresent().ShouldBeTrue("Pop up message should be present");
                _claimAction.ClosePageError();
                _claimAction.IsInvalidInputPresentOnNote().ShouldBeTrue("Note Indicator should be present");
                _claimAction.GetInvalidTooltipMessageOnNote()
                    .ShouldBeEqual("Note is required before the record can be saved.", "Note tooltip warning message");


                _claimAction.SelectNoteType("Claim");
                _claimAction.IsSubTypeDisabled().ShouldBeTrue("SubType is disabled");
                _claimAction.SetAddNote("note");
                _claimAction.ClickonAddNoteSaveButton();
                _claimAction.WaitForWorkingAjaxMessage();
                _claimAction.NoOfClaimNotes()
                    .ShouldBeEqual((noOfNotes + 1).ToString(), "Number of Notes present in Claim");
                _claimAction.ClickOnClaimNotes();
                _claimAction.IsNoteContainerPresent()
                    .ShouldBeFalse("Note Container must collapse after clicking note icon again.");

            }
        }


        [Test, Category("NewClaimAction2")] //US25689 +TE-963
        public void Verify_provider_details_with_no_badge()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(2))
            {
                ClaimActionPage _claimAction;
                
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                string claimSeqWithoutProviderDetailBadge = paramLists["ClaimSequenceWithNoProviderDetailBadge"];
                string providerSeq = paramLists["provSeq"];

                _claimAction = automatedBase.CurrentPage.NavigateToClaimSearch()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(claimSeqWithoutProviderDetailBadge);

                var specialty = _claimAction.GetProviderSpecFromDB(claimSeqWithoutProviderDetailBadge);
                var adjSpec = _claimAction.GetSpecialtyFromDB(providerSeq);
                
                _claimAction.IsProviderDetailsIconPresent().ShouldBeTrue("Is Provider Detail Icon Present?");
                _claimAction.IsBadgePresentInProviderDetailIcon()
                    .ShouldBeFalse("Is Badge present in Provider Detail Icon?");
                //when user clicks on provider icon, provider detail container is shown
                _claimAction.ClickonProviderDetailsIcon().IsProviderDetailsViewDivDisplayed()
                    .ShouldBeTrue("Provider Details Div shown on click?");
                _claimAction.GetContainerViewVerticalRowLabel(1).ShouldBeEqual("Specialty", "Specialty Label-");
                _claimAction.GetContainerViewVerticalRowLabel(2).ShouldBeEqual("Address", "Address Label-");
                _claimAction.GetContainerViewVerticalRowLabel(3).ShouldBeEqual("Name", "Name Label-");
                _claimAction.GetContainerViewVerticalRowLabel(4).ShouldBeEqual("TIN", "TIN Label-");
                //verify value matches with that in provider table in the left


                _claimAction.GetProviderSpecialty().Split('-')[0].Trim()
                    .ShouldBeEqual(adjSpec, "specialty correct from DB?");
                _claimAction.GetContainerViewVerticalRowValue(1).Split('-')[0].Trim()
                    .ShouldBeEqual(specialty, "Provider Specialty");
                _claimAction.GetContainerViewVerticalRowValue(1).Split('-')[0].Trim()
                    .ShouldBeEqual(_claimAction.GetProvierDetailSpecialtyFromDB(providerSeq), "Provider Specialty");
                _claimAction.GetContainerViewVerticalRowSecondValue(2)
                    .ShouldBeEqual(_claimAction.GetProviderZip(), "Provider Zip");
                _claimAction.GetContainerViewVerticalRowValue(3)
                    .ShouldBeEqual(_claimAction.GetProviderName(), "Provider Name");
                _claimAction.GetContainerViewVerticalRowValue(4)
                    .ShouldBeEqual(_claimAction.GetProviderTin(), "Provider TIN");

                //verify specialty in flagged lines and claim lines
                _claimAction.GetFlaggedLineDetails(1, 1, 9)
                    .ShouldBeEqual(specialty, "specialty in flagged line correct?");
                _claimAction.GetClaimLineDetailsValueByLineNo(1, 2, 7)
                    .ShouldBeEqual(specialty, "specialty in claim line correct?");

                var patientclaimHistory = _claimAction.ClickOnHistoryButtonAndSwitchToPatientClaimHistoryPage();
                patientclaimHistory.GetPatientClaimHistoryData(1, 1, 4)
                    .ShouldBeEqual(specialty, "specialty same in patient claim history?");
                _claimAction = patientclaimHistory.SwitchToNewClaimActionPage(true);
            }
        }


        [Test, Category("NewClaimAction2")] //US25235
        public void
            Verify_provider_detail_container_is_displayed_on_clicking_provider_detail_icon_and_verify_the_container_view_vertical_rows()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(2))
            {
                ClaimActionPage _claimAction;
                var _claimSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, "ClassInit",
                    "ClaimSequence", "Value");
                _claimAction = automatedBase.QuickLaunch.NavigateToClaimSearch()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(_claimSeq, true);

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IList<string> expectedContainerViewVerticalRowsList = automatedBase.DataHelper
                    .GetMappingData(FullyQualifiedClassName, "Set_of_provider_detail_container_view_vertical_rows")
                    .Values.ToList();
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                string claimSeqWithProviderDetailBadge = paramLists["ClaimSequenceWithProviderDetailBadge"];
                if (!_claimAction.GetCurrentClaimSequence().Equals(claimSeqWithProviderDetailBadge))
                {
                    SearchByClaimSeqFromWorkList(_claimAction, claimSeqWithProviderDetailBadge);
                }

                //when user clicks on provider icon, provider detail container is shown
                _claimAction.ClickonProviderDetailsIcon().IsProviderDetailsViewDivDisplayed()
                    .ShouldBeTrue("Provider Details Div shown on click?");
                expectedContainerViewVerticalRowsList.ShouldCollectionBeEqual(
                    _claimAction.GetContainerViewVerticalRows(),
                    "Container View Vertical Rows are equal:");
                //all rows will be in collapsed state when containder view is dispalyed.
                _claimAction.AreAllRowsCollapsedInProviderDetailsContainerView()
                    .ShouldBeTrue("Are all rows in Provider Details container collapsed in default");

            }
        }



        [Test, Category("NewClaimAction2")] //US25237+US69249 + TANT-82
        public void
            Verify_switch_functionality_when_a_flag_is_swithed_to_or_switched_from_another_line_on_a_same_claim()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(2))
            {
                ClaimActionPage _claimAction;
               
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var claimSeq = paramLists["ClaimSequence"];
                var flag = paramLists["Flag"];
                var lineNoFrom = paramLists["LineNumberFrom"];
                var lineNoTo = paramLists["LineNumberTo"];

                _claimAction = automatedBase.CurrentPage.NavigateToClaimSearch()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(claimSeq);

                _claimAction.RefreshPage(false);

                _claimAction.DeleteAllDeletedFlagsByClaSeq(claimSeq);
               
                if (_claimAction.IsSwitchIconPresentForFlagToBeSwitchByCurrentLineNoAndFlagAndTriggerLineNo(lineNoFrom,
                    flag, lineNoTo))
                {
                    _claimAction.ClickOnSwitchIconByCurrentLineNoAndFlagAndTriggerLineNo(lineNoFrom,
                        flag, lineNoTo);
                    _claimAction
                        .IsDeletedSwitchIconPresentAfterFlagSwitchedByCurrentLineNoAndFlagAndTriggerLineNo(lineNoFrom,
                            flag, lineNoTo)
                        .ShouldBeTrue("Current Flag Should Deleted");

                    _claimAction
                        .IsSwitchIconPresentForFlagToBeSwitchByCurrentLineNoAndFlagAndTriggerLineNo(lineNoTo, flag,
                            lineNoFrom)
                        .ShouldBeTrue("Switched Flag should present in Trigger Line");
                }
                else
                {
                    _claimAction.ClickOnSwitchIconByCurrentLineNoAndFlagAndTriggerLineNo(lineNoTo, flag, lineNoFrom);
                    _claimAction
                        .IsDeletedSwitchIconPresentAfterFlagSwitchedByCurrentLineNoAndFlagAndTriggerLineNo(lineNoTo,
                            flag, lineNoFrom)
                        .ShouldBeTrue("Current Flag Should Deleted");
                    _claimAction
                        .IsSwitchIconPresentForFlagToBeSwitchByCurrentLineNoAndFlagAndTriggerLineNo(lineNoFrom, flag,
                            lineNoTo)
                        .ShouldBeTrue("Switched Flag should present in Trigger Line");
                }
            }
        }

        [Test, Category("NewClaimAction2")]//US25237+US69249
        public void Verify_switch_functionality_when_a_flag_is_swithed_to_or_switched_from_another_line_on_different_claim()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(2))
            {
                ClaimActionPage _claimAction;
               
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var fromClaimSeq = paramLists["FromClaimSequence"];
                var toClaimSeq = paramLists["ToClaimSequence"];
                var lineNoFrom = paramLists["LineNumberFrom"];
                var lineNoTo = paramLists["LineNumberTo"];

                _claimAction = automatedBase.CurrentPage.NavigateToClaimSearch()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(fromClaimSeq);

                _claimAction.RefreshPage(false);

                _claimAction.DeleteAllDeletedFlagsByClaSeq(fromClaimSeq);
                _claimAction.DeleteAllDeletedFlagsByClaSeq(toClaimSeq);

                if (_claimAction.IsSwitchIconPresentForFlagToBeSwitchByCurrentLineNoAndTriggerClaseqAndTriggerLineNo(
                    lineNoFrom, toClaimSeq, lineNoTo))
                {
                    _claimAction.ClickOnSwitchIconByCurrentLineNoAndTriggerClaSeqAndTriggerLineNo(lineNoFrom,
                        toClaimSeq, lineNoTo);
                    _claimAction
                        .IsDeletedSwitchIconPresentAfterFlagSwitchedByCurrentLineNoAndTriggerClaseqAndTriggerLineNo(
                            lineNoFrom, toClaimSeq, lineNoTo)
                        .ShouldBeTrue("Current Flag Should Deleted");
                    SearchByClaimSeqFromWorkList(_claimAction, toClaimSeq);
                    _claimAction
                        .IsSwitchIconPresentForFlagToBeSwitchByCurrentLineNoAndTriggerClaseqAndTriggerLineNo(lineNoTo,
                            fromClaimSeq, lineNoFrom)
                        .ShouldBeTrue("Switched Flag should present in Trigger Claim");
                }
                else
                {
                    SearchByClaimSeqFromWorkList(_claimAction, toClaimSeq);
                    _claimAction.ClickOnSwitchIconByCurrentLineNoAndTriggerClaSeqAndTriggerLineNo(lineNoTo,
                        fromClaimSeq, lineNoFrom);
                    _claimAction
                        .IsDeletedSwitchIconPresentAfterFlagSwitchedByCurrentLineNoAndTriggerClaseqAndTriggerLineNo(
                            lineNoTo, fromClaimSeq, lineNoFrom)
                        .ShouldBeTrue("Current Flag Should Deleted");
                    SearchByClaimSeqFromWorkList(_claimAction, fromClaimSeq);
                    _claimAction
                        .IsSwitchIconPresentForFlagToBeSwitchByCurrentLineNoAndTriggerClaseqAndTriggerLineNo(lineNoFrom,
                            toClaimSeq, lineNoTo)
                        .ShouldBeTrue("Switched Flag should present in Trigger Claim");
                }

            }

        }



        [Test, Category("NewClaimAction2")]
        public void
            Verify_that_tooltip_appears_when_user_hovers_over_POS_value_in_Flagged_lines_Lines_details_Claim_lines_and_Flag_Details()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(2))
            {
                ClaimActionPage _claimAction;
                
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                string claimSeq = paramLists["ClaimSequence"];

                _claimAction = automatedBase.CurrentPage.NavigateToClaimSearch()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(claimSeq);

                try
                {
                    (!string.IsNullOrEmpty(_claimAction.GetTitleAttributeValueOfPosOfFlaggedLines())).ShouldBeTrue(
                        "POS Tooltip of Flagged Lines is present.");
                    (!string.IsNullOrEmpty(_claimAction.GetTitleAttributeValueOfPosOfClaimLines())).ShouldBeTrue(
                        "POS Tooltip of Claim Lines is present.");
                    _claimAction.ClickOnFirstFlagLineOfFlaggedLinesDiv();
                    if (_claimAction.IsTriggerClaimPresentInFirstFlagLine())
                    {
                        Console.WriteLine("Trigger Claim is present in first flag line.");
                        (!string.IsNullOrEmpty(_claimAction.GetTitleAttributeValueOfPosOfFlagDetails())).ShouldBeTrue(
                            "POS Tooltip of Flag Detail is present.");
                    }
                    else
                    {
                        Console.WriteLine("Trigger Claim is not present in first flag line");
                        _claimAction.IsPosPresentInFlagDetails().ShouldBeFalse("POS is not present in flag detail.");
                    }

                    _claimAction.ClickOnLineDetailsSection();
                    if (_claimAction.IsTriggerClaimsPresentInFirstLineDetails())
                    {
                        Console.WriteLine("Trigger Claims is present in first line details.");
                        (!string.IsNullOrEmpty(_claimAction.GetTitleAttributeValueOfPosOfLineDetails())).ShouldBeTrue(
                            "POS Tooltip of Line Detail is present.");
                    }
                    else
                    {
                        Console.WriteLine("Trigger Claim is not present in first line details.");
                        _claimAction.IsPosPresentInLineDetails().ShouldBeFalse("POS is not present in line detail.");
                    }
                }
                finally
                {
                    if (_claimAction.IsClaimLinesPresent())
                        _claimAction.ClickOnLinesIcon();
                }
            }
        }

        [Test, Category("NewClaimAction2")]
        public void
            Verify_that_Click_on_the_Dollar_symbol_in_the_Flag_Details_view_loads_the_Claim_Dollar_Details_view()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(2))
            {
                ClaimActionPage _claimAction;
                
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                string claimSeq = paramLists["ClaimSequence"];

                _claimAction = automatedBase.CurrentPage.NavigateToClaimSearch()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(claimSeq);
                
                _claimAction.ClickOnFlagDetailsSection();
                _claimAction.ClickOnDollarIcon();
                string labelValue = _claimAction.GetValueOfLowerRightQuadrantClaimDollar();
                _claimAction.ClickOnLinesIcon();
                labelValue.ShouldBeEqual("Claim Dollar Details", "Label of Lower Right Quadrant");
            }
        }

        [Test, Category("NewClaimAction2")]
        public void Verify_that_click_on_the_lines_icon_in_the_Flag_Details_view_loads_the_Claim_Lines_view()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(2))
            {
                ClaimActionPage _claimAction;
               
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                string claimSeq = paramLists["ClaimSequence"];

                _claimAction = automatedBase.CurrentPage.NavigateToClaimSearch()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(claimSeq);
                
                _claimAction.ClickOnFlagDetailsSection();
                _claimAction.ClickOnLinesIcon();
                _claimAction.GetValueOfLowerRightQuadrant()
                    .ShouldBeEqual("Claim Lines", "Label of Lower Right Quadrant");
            }
        }



        [Test, Category("NewClaimAction2"), Category("AppealDependent")] //US44850
        public void Verify_behaviour_of_search_icon_and_find_claims()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(2))
            {
                ClaimActionPage _claimAction;
               
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var claimSeq = paramLists["ClaimSequence"];
                var claimNo = paramLists["ClaimNo"];
                _claimAction = automatedBase.CurrentPage.NavigateToCVClaimsWorkList();
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                try
                {

                    if (_claimAction.IsPageHeaderPresent())
                    {
                        if (!_claimAction.IsWorkListControlDisplayed())
                            _claimAction.ClickWorkListIcon();
                    }

                    _claimAction.ClickSearchIcon();
                    _claimAction.IsClaimSearchPanelPresent().ShouldBeTrue("Claim Search Panel is present");
                    _claimAction.GetClaimSequenceInFindClaimSection()
                        .ShouldBeNullorEmpty("Claim Sequence Text Box should be blank by default");
                    _claimAction.GetClaimNoInFindClaimSection()
                        .ShouldBeNullorEmpty("Claim No Text Box should be blank by default");
                    _claimAction.GetAlternateClaimNoLabelTitleInFindPanel()
                        .ShouldBeEqual("Claim No", "title of alternate claim # equals claim no ");
                    _claimAction.SetClaimSequenceInFindClaimSection("asdf");
                    _claimAction.GetPageErrorMessage()
                        .ShouldBeEqual(
                            "Only numbers allowed.", "Poup Message when claim seq in non numeric");
                    Console.WriteLine("Error displayed when non numeric value entered in claim seq");
                    _claimAction.ClosePageError();

                    _claimAction.SetClaimNoInFindClaimSection("a1");
                    _claimAction.IsPageErrorPopupModalPresent()
                        .ShouldBeFalse("No error pop up open when claim no is alphanumeric. Is error present?");
                    _claimAction.GetClaimSequenceInFindClaimSection()
                        .ShouldBeNullorEmpty(
                            "Claim Sequence Text Box should be automatically cleared when claim no is entered");
                    _claimAction.SetClaimSequenceInFindClaimSection("1");
                    _claimAction.GetClaimNoInFindClaimSection()
                        .ShouldBeNullorEmpty(
                            "Claim No Text Box should be automatically empty when claim sequence is filled");

                    _claimAction.ClickOnClearLinkOnFindSection();
                    _claimAction.ClickOnFindButton();
                    _claimAction.IsPageErrorPopupModalPresent().ShouldBeTrue("Page Error Present");
                    _claimAction.GetPageErrorMessage()
                        .ShouldBeEqual(
                            "Search cannot be initiated without any criteria entered.",
                            "Poup Message with no values in Claim Seq and Claim No");
                    Console.WriteLine("Error displayed when search initiated without any criteria");
                    _claimAction.ClosePageError();

                    _claimAction.SearchByClaimSequence("1");
                    _claimAction.GetEmptyMessage()
                        .ShouldBeEqual("No matching records were found.", "No Match Record Message");
                    _claimAction.IsListOfNextClaimsInWorklistSectionDisplayed()
                        .ShouldBeTrue("List of worklist present");
                    _claimAction.SearchByClaimSequence(claimSeq);
                    _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                    _claimAction.ClickWorkListIcon();
                    _claimAction.IsNoDataTextInNextClaimsInWorklistSectionDisplayed()
                        .ShouldBeTrue("Previous Work List abandoded");
                    _claimAction.GetNoDataTextInNextClaimsInWorklistSectionDisplayed()
                        .ShouldBeEqual("No Data Available",
                            "Previous Work List abandoded with no data available message");

                    _claimAction.SearchByClaimNoAndStayOnSearch(claimNo);
                    //_claimAction.GetSearchlistComponentItemValue(1, 3)
                    //    .ShouldEqual(claimNo, "Client's claim no  displayed");
                    _claimAction.GetSearchlistComponentItemLabel(1, 5).ShouldBeEqual("Prov:", "Provider label present");
                    _claimAction.GetSearchlistComponentItemValue(1, 5)
                        .ShouldBeEqual("55087", "Provider Seq ");
                    _claimAction.GetSearchlistComponentItemValue(1, 3).ShouldBeEqual("PAY", "Flag displayed");
                    _claimAction.GetSearchlistComponentTooltipValue(1, 3)
                        .ShouldBeEqual("PAY", "Flag tooltip displayed");
                    //_claimAction.GetSearchlistComponentItemLabel(1, 6).ShouldBeEqual("Savings:", "Saving label present");
                    _claimAction.GetSearchlistComponentItemValue(1, 7)
                        .ShouldBeEqual("$50.00", "Saving value displayed");
                    //_claimAction.GetSearchlistComponentItemLabel(1, 7).ShouldEqual("S:", "Claim Status label present");
                    _claimAction.GetSearchlistComponentItemValue(1, 8)
                        .ShouldBeEqual("Client Reviewed", "Claim Status displayed");
                    //_claimAction.GetSearchlistComponentItemLabel(1, 8)
                    //    .ShouldEqual("Adj:", "Adjustment status label present");
                    //_claimAction.GetSearchlistComponentItemValue(1, 8).ShouldEqual("O", "Adjustment Status displayed");
                    _claimAction.GetAppealLevelBadgeOnSearchResult(3)
                        .ShouldBeEqual("2", "Appeal level badge present as an indicator");
                    var searchList = _claimAction.GetClaimSeqListOnSearchResult();
                    searchList.ShouldCollectionBeSorted(true, "Iterations are list with most recent claim at top");
                    _claimAction.ClickOnClaimSequenceOfSearchResult(1);
                    _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                    for (var i = 0; i < searchList.Count; i++)
                    {
                        _claimAction.GetClaimSequence()
                            .ShouldBeEqual(searchList[i], "Claim Should Open sequentially as searched claim list");
                        _claimAction = _claimAction.ClickOnNextButton();
                        if (i == searchList.Count - 1)
                            break;
                        Console.WriteLine("Claim action opened for claim: " + _claimAction.GetClaimSequence(),
                            System.Environment.NewLine);
                        _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                    }

                    _claimAction.GetPageHeader()
                        .ShouldBeEqual(PageHeaderEnum.ClaimSearch.GetStringValue(),
                            "Page Should redirect to Claim Search after visiting each claim of search result through next button");
                    _claimAction.ClickOnClaimSequenceOfSearchResult(1);
                    _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                    _claimAction.IsClaimSearchIconPresent()
                        .ShouldBeTrue("New Claim Search Icon is present beside work list icon");
                    _claimAction.ClickClaimSearchIcon();
                    var omnipresentSearchList = _claimAction.GetClaimSeqListOnSearchResult();
                    searchList.ShouldCollectionBeEqual(omnipresentSearchList,
                        "View has returned to previous search result list.");
                    _claimAction.ClickOnClaimSequenceOfSearchResult(1);
                    _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                }
                finally
                {
                    if (_claimAction.GetPageHeader() == PageHeaderEnum.ClaimSearch.GetStringValue())
                    {
                        _claimAction = _claimAction.NavigateToClaimSearch()
                            .SearchByClaimSequenceToNavigateToClaimActionPage(claimSeq);
                        _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                    }

                    if (_claimAction.IsWorkListControlDisplayed())
                        _claimAction.ClickWorkListIcon();
                }
            }
        }

        [Test, Category("NewClaimAction2")] //us45706 and us45707
        public void Verify_ciu_creation_and_referral_loading()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(2))
            {
                ClaimActionPage _claimAction;
               
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramlLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                _claimAction = automatedBase.CurrentPage.NavigateToCVClaimsWorkList();
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                _claimAction.IsProviderDetailsViewDivDisplayed()
                    .ShouldBeFalse("Provider Details Div should be lazy loaded and so should not display?");
                _claimAction.ClickonProviderDetailsIcon();
                _claimAction.IsProviderDetailsViewDivDisplayed().ShouldBeTrue("Provider Details Div shown on click?");
                _claimAction.ClickOnNextButton();
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                _claimAction.GetTopRightComponentTitle()
                    .ShouldBeEqual("Dx Codes", "Top Right component title defaults to dx codes ");
                SearchByClaimSeqFromWorkList(_claimAction, paramlLists["ClaimSequence"]);
                _claimAction.GetLastRowLastColLabelForClaimDetails().ShouldBeEqual("PROV_NAME",
                    "CIU referral icon not displayed next to provider currentUser");
                _claimAction.ClickonProviderDetailsIcon().IsProviderDetailsViewDivDisplayed()
                    .ShouldBeTrue("Provider Details Div shown on click?");
                _claimAction.ClickonAddCiuReferralIcon();
                if (String.IsNullOrEmpty(_claimAction.GetCiuReferralFormInputValueGeneric("Phone Number")))
                    _claimAction.SetCiuReferralFormInputValueGeneric("Phone Number", "12345");
                _claimAction.GenericSelectSearchDropDownListValue("Pattern Category", paramlLists["Pattern"]);
                _claimAction.SetSearchInputFieldForCiu("Proc Code", paramlLists["ProcCode"]);
                _claimAction.SetNote(paramlLists["IdentifiedPattern"]);
                _claimAction.ClickonSaveCiuReferralIcon();
                if (_claimAction.IsPageErrorPopupModalPresent())
                {
                    _claimAction.ClosePageError();
                    _claimAction.ClickonSaveCiuReferralIcon();
                }

                _claimAction.ClickOnDeleteReferralBtn(1);
                _claimAction.ClickOkCancelOnConfirmationModal(true);
            }
        }


        [Test, Category("NewClaimAction2")]//US45716
        public void Verify_dx_code_quadrant_auto_sized_to_accomodate_dx_codes()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(2))
            {
                ClaimActionPage _claimAction;
               

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                string claimSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "ClaimSequence", "Value");

                _claimAction = automatedBase.CurrentPage.NavigateToClaimSearch()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(claimSeq);

                var dxlistFirst = _claimAction.GetListOfDxCodeList();
                _claimAction = _claimAction.NavigateToCVClaimsWorkList();
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                _claimAction.ClickOnNextButton();
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                var dxlistSecond = _claimAction.GetListOfDxCodeList();
                if (dxlistFirst.Count.Equals(dxlistSecond.Count))
                {
                    dxlistFirst.ShouldNotBeTheSameAs(dxlistSecond, "Dx code quadrant adjusts to present dx list");
                    int i = 0;
                    do
                    {
                        var dxlistA = _claimAction.GetListOfDxCodeList();
                        _claimAction.ClickOnNextButton();
                        _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                        var dxlistB = _claimAction.GetListOfDxCodeList();
                        if (dxlistA.Count.Equals(dxlistB.Count)) i++;
                        else i = 0;
                    } while (i > 0);

                }

                _claimAction = automatedBase.CurrentPage.NavigateToClaimSearch()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(claimSeq);

                var dxlist = _claimAction.GetListOfDxCodeList();
                dxlist.Count.ShouldBeEqual(7, "Dx code quadrant accomodates more than 4 dx codes.");

            }
        }

        [Test, Category("NewClaimAction2")] //US69252
        public void Verify_users_cannot_delete_core_flags()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(2))
            {
                ClaimActionPage _claimAction;
                
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var claimSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName,
                    "ClaimSequence", "Value");
                var coreFlagList = automatedBase.DataHelper
                    .GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName, "CoreFlags", "Value")
                    .Split(';').ToList();
                var flagSequences = automatedBase.DataHelper
                    .GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName, "Flagseqs", "Value")
                    .Split(';').ToList();

                _claimAction = automatedBase.CurrentPage.NavigateToClaimSearch()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(claimSeq);

                foreach (var flagseq in flagSequences)
                {
                    for (var i = 1; i <= 2; i++)
                        _claimAction.DeleteClaimAuditRecordFromDatabase(flagseq, claimSeq, i.ToString());
                }

                _claimAction.RestoreAllFlagByClaimSequence(claimSeq);
                _claimAction.RefreshPage(false);

                try
                {
                    _claimAction.IsEditIconDisabledByLinenoAndRow(2, 2)
                        .ShouldBeTrue("Is Flag Level Edit Icon disabled for Core flag?");
                    _claimAction.IsEditIconEnabledByLinenoAndRow(1, 1)
                        .ShouldBeTrue("Is Flag Level Edit Icon enabled for non Core flag?");
                    _claimAction.IsDeleteIconDisabledByLinenoAndRow(2, 2)
                        .ShouldBeTrue("Is Flag Level Delete Icon disabled for Core flag?");
                    _claimAction.IsDeleteIconEnabledByLinenoAndRow(1, 1)
                        .ShouldBeTrue("Is Flag Level Delete Icon enabled for non Core flag?");

                    StringFormatter.PrintMessageTitle(" Delete All Flags on the claim ");
                    _claimAction.ClickOnDeleteAllFlagsIcon();
                    _claimAction.IsFlaggedLineDeletedByLine(1, 1)
                        .ShouldBeTrue("Non core flag is deleted when Delete all Flag on claim icon is clicked");
                    VerifyCoreFlagNotDeleted(_claimAction, 2, 1, coreFlagList[0]);
                    VerifyCoreFlagNotDeleted(_claimAction, 3, 1, coreFlagList[1]);
                    _claimAction.ClickOnRestoreAllFlagsIcon();

                    StringFormatter.PrintMessageTitle(" Delete All Flags on the line ");
                    _claimAction.ClickOnDeleteIconOnFlaggedLinesRowsByRow(claimSeq, 2);
                    _claimAction.IsFlaggedLineDeletedByLine(2, 2)
                        .ShouldBeTrue("Non core flag is deleted when Delete all Flags on Line icon is clicked");
                    VerifyCoreFlagNotDeleted(_claimAction, 2, 1, coreFlagList[0]);
                    _claimAction.ClickOnRestoreIconOnFlaggedLinesRowsByRow(claimSeq, 2);

                    _claimAction.IsDeleteIconRowByRowDisabled(3).ShouldBeTrue(
                        "Delete icon on Line Level should be disabled if that line contains only Core flag");
                    _claimAction.IsDeleteIconRowByRowDisabled(1).ShouldBeFalse(
                        "Delete icon on Line Level should be disabled if that line contains non Core flags also");
                }
                finally
                {
                    if (!_claimAction.IsDeleteAllFlagsPresent())
                        _claimAction.ClickOnRestoreAllFlagsIcon();
                    for (var i = 1; i <= 3; i++)
                    {
                        if (!_claimAction.IsDeleteIconPresentOnFlaggedLinesRowsByRow(i))
                            _claimAction.ClickOnRestoreIconOnFlaggedLinesRowsByRow(claimSeq, i);
                    }
                }
            }
        }


        [Test, Category("NewClaimAction2")] //US69251
        public void Verify_CIU_referrals_creation_and_validation()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(2))
            {
                ClaimActionPage _claimAction;
                
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramlLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                const string alphaNumeric = "all2";

                _claimAction = automatedBase.CurrentPage.NavigateToClaimSearch()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(paramlLists["ClaimSequence"]);

                var patternCategory = paramlLists["Pattern"].Split(';');
                var expectedPatternCategoryList =
                    automatedBase.DataHelper
                        .GetMappingData(FullyQualifiedClassName, "Pattern_cattegory_for_ciu_referral").Values.ToList();

                _claimAction.ClickonProviderDetailsIcon().IsProviderDetailsViewDivDisplayed()
                    .ShouldBeTrue("Is Provider Details Div shown on click?");
                if (!_claimAction.IsNoCiuReferralMessagePresent())
                {
                    var ciuCount = _claimAction.GetCiuReferralRecordRowCount();
                    for (var i = 1; i < ciuCount; i++)
                    {
                        _claimAction.ClickOnDeleteReferralBtn(i);
                        _claimAction.ClickOkCancelOnConfirmationModal(true);
                    }
                }

                try
                {
                    _claimAction.ClickonAddCiuReferralIcon();

                    StringFormatter.PrintMessage("Valdiate error message when required fields are empty");
                    ValidateRequiredFieldsAndTheirDefaultValues(_claimAction, patternCategory[0]);

                    StringFormatter.PrintMessage("Validate phone number must be 10 digit");
                    ValidateFieldIsRequiredAndErrorMessagePopUp(_claimAction, "12", patternCategory[0], "123", "tests",
                        "Phone Number");

                    var procString = new string('a', 1002);
                    var noteString = new string('a', 2010);
                    _claimAction.SetCiuReferralFormInputValueGeneric(alphaNumeric, "Phone Number");

                    StringFormatter.PrintMessage("Click tab key to remove focus from phone number field");
                    _claimAction.SendTabKeyOnPhoneNumberToFocusExt();
                    _claimAction.GetCiuReferralFormInputValueGeneric("Phone Number")
                        .AssertIsNotContained(alphaNumeric.Where(char.IsLetter).ToArray(),
                            "Phone number should only accept numeric values.");
                    _claimAction.SetCiuReferralFormInputValueGeneric("01234567890", "Phone Number");

                    StringFormatter.PrintMessage("Click tab key to remove focus from phone number field");
                    _claimAction.SendTabKeyOnPhoneNumberToFocusExt();
                    _claimAction.GetCiuReferralFormInputValueGeneric("Phone Number").ShouldBeEqual("012-345-6789",
                        "Phone Number should allow only 10 digit value");
                    _claimAction.SetCiuReferralFormInputValueGeneric(alphaNumeric + "44a", "Ext");
                    _claimAction.GetCiuReferralFormInputValueGeneric("Ext")
                        .ShouldBeEqual(alphaNumeric, "Ext should be editable.");
                    _claimAction.GetCiuReferralFormInputValueGeneric("Ext").Length
                        .ShouldBeEqual(4, "Ext should accept only 4 characters.");
                    _claimAction.GenericSelectSearchDropDownListValue("Pattern Category", patternCategory[0]);
                    _claimAction.GetSideBarPanelSearch.GetInputAttributeValueByLabel("Pattern Category", "placeholder")
                        .ShouldBeEqual(patternCategory[0], "Pattern Category value should equal " + patternCategory[0]);
                    _claimAction.GenericSelectSearchDropDownListValue("Pattern Category", patternCategory[1]);
                    _claimAction.GetSideBarPanelSearch.GetInputAttributeValueByLabel("Pattern Category", "placeholder")
                        .ShouldBeEqual("Multiple values selected", "Pattern Category allows multiple value selected");
                    _claimAction.SetLengthyValueInCiuReferralFormInputValueGeneric(procString, "Proc Code", 1000);
                    _claimAction.GetCiuReferralFormInputValueGeneric("Proc Code")
                        .Length.ShouldBeLessOrEqual(1000, "Proc Code accepts upto 1000 characters");
                    _claimAction.SetLengthyNoteToCiuReferral("CIU Referral",noteString,false);
                    _claimAction.IsPageErrorPopupModalPresent()
                        .ShouldBeTrue("When Identified Pattern is long error message should pop up.");
                    _claimAction.GetPageErrorMessage()
                        .ShouldBeEqual("Identified Pattern value is too long.",
                            "Error pop up message should be present when Identified Pattern is long");
                    _claimAction.ClosePageError();
                    _claimAction.GetNoteOfCiuReferral()
                        .Length.ShouldBeLessOrEqual(1893,
                            "Identified Pattern shuold be less or equal to 2000 characters");
                    _claimAction.SetCiuReferralFormInputValueGeneric(alphaNumeric + "!@", "Proc Code");
                    _claimAction.GetCiuReferralFormInputValueGeneric("Proc Code")
                        .ShouldBeEqual(alphaNumeric + "!@", "Proc Code should accept alpha Numeric values.");
                    _claimAction.GenericSelectSearchDropDownListValue("Pattern Category", "Clear");
                    var listedOptionsList =
                        _claimAction.GetSideBarPanelSearch.GetMultiSelectAvailableDropDownList("Pattern Category");
                    listedOptionsList.ShouldCollectionBeEqual(expectedPatternCategoryList,
                        "Pattern Category List is as Expected");
                    _claimAction.ClickOnCancelCiuReferral();
                    _claimAction.IsProviderDetailsViewDivDisplayed()
                        .ShouldBeTrue("Provider Details Div should be  displayed once ciu referral is cancelled");
                    _claimAction.IsNoCiuReferralMessagePresent()
                        .ShouldBeTrue("Clicking on cancel button discard any changes made to CIU Referral container");
                    CreateCiuReferral(_claimAction, paramlLists["PhoneNo"], null, patternCategory[0],
                        paramlLists["IdentifiedPattern"],
                        paramlLists["ProcCode"]);
                    if (_claimAction.IsPageErrorPopupModalPresent())
                    {
                        _claimAction.ClosePageError();
                        _claimAction.ClickonSaveCiuReferralIcon();
                    }

                    _claimAction.GetCiuReferralRecordRowCount()
                        .ShouldBeGreaterOrEqual(1, "Ciu record should be created.");
                    _claimAction.ClickOnDeleteReferralBtn(1);
                    _claimAction.ClickOkCancelOnConfirmationModal(false);
                    _claimAction.GetCiuReferralRecordRowCount()
                        .ShouldBeGreaterOrEqual(1,
                            "Ciu record should not be deleted when cancel is clicked on delete confirmation.");
                    _claimAction.ClickOnDeleteReferralBtn(1);
                    _claimAction.ClickOkCancelOnConfirmationModal(true);
                    _claimAction.IsNoCiuReferralMessagePresent()
                        .ShouldBeTrue(
                            "CIU referral should be deleted by clicking on delete icon and confirming delete");
                }
                finally
                {
                    if (!_claimAction.IsProviderDetailsViewDivDisplayed())
                        _claimAction.ClickonProviderDetailsIcon();
                    if (_claimAction.IsCreateCiuReferralSectionDisplayed()) _claimAction.ClickOnCancelCiuReferral();
                    if (!_claimAction.IsNoCiuReferralMessagePresent())
                    {
                        var ciuCount = _claimAction.GetCiuReferralRecordRowCount();
                        for (var i = 1; i < ciuCount; i++)
                        {
                            _claimAction.ClickOnDeleteReferralBtn(i);
                            _claimAction.ClickOkCancelOnConfirmationModal(true);
                        }
                    }
                }
            }
        }


        [Test, Category("NewClaimAction2")] //US69251
        public void Verify_CIU_referrals_details_Records()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(2))
            {
                ClaimActionPage _claimAction;
                

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var claSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName,
                    "ClaimSequence", "Value");
                const string phoneNo = "231-456-7890";
                const string procCode = "1231890";
                const string category = "RE";
                const string pattern = "Test Identified Pattern\r\nTest Identified Pattern\r\nTest Identified Pattern";

                _claimAction = automatedBase.CurrentPage.NavigateToClaimSearch()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(claSeq);

                _claimAction.ClickonProviderDetailsIcon().IsProviderDetailsViewDivDisplayed()
                    .ShouldBeTrue("Provider Details Div shown on click?");
                _claimAction.GetCiuReferralRecordRowCount()
                    .ShouldBeGreaterOrEqual(1, "Ciu record should be present.");

                StringFormatter.PrintMessageTitle("Verification of CIU Record Detail Value");
                var createdDateList = _claimAction.GetCiuCreatedDateList();
                createdDateList.IsInDescendingOrder().ShouldBeTrue("Created Date Should be in Descending Order");
                createdDateList[0].IsDateInFormat().ShouldBeTrue("The Record Created Date is in format MM/DD/YYYY");
                ;
                _claimAction.GetCiuReferralDetailsByRowLabel(1, "By:").DoesNameContainsOnlyFirstWithLastname()
                    .ShouldBeTrue("Referral Created By is in format XXX XXX (XXX)");
                _claimAction.GetCiuReferralDetailsByRowLabel(1, "Claim Seq:")
                    .ShouldBeEqual(claSeq,
                        "CIU Referral Created from Claim Action should display corresponding Claim Sequence");
                _claimAction.GetCiuReferralDetailsByRowLabel(1, "Ph:")
                    .ShouldBeEqual(phoneNo, "Phone No Should dispalyed according to Created By");
                _claimAction.GetCiuReferralDetailsByRowLabel(1, "Type:")
                    .ShouldBeEqual("Research", "Type Should Equal");
                _claimAction.GetCiuReferralDetailsByRowLabel(1, "Proc Code:")
                    .ShouldBeEqual(procCode, "Type Should Equal");
                _claimAction.GetCiuReferralDetailsByRowLabel(1, "Category:")
                    .ShouldBeEqual(category, "Category Abbreviation should Equal and separated by comma");
                _claimAction.GetCiuReferralDetailsByRowLabel(1, "Pattern")
                    .ShouldBeEqual(pattern, "Pattern should dispaly entire text");
                _claimAction.GetCiuReferralToolTipDayByRowLabel(1, "Pattern")
                    .ShouldBeEqual(Regex.Replace(pattern, "\r\n", ""),
                        "Pattern Tooltip should equal and should not contain next line");

            }
        }

        [Test, Category("NewClaimAction2")] //US69379
        public void Validation_test_to_create_appeal_from_new_claim_action_for_client_reviewed_claims()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(2))
            {
                ClaimActionPage _claimAction;
                
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> testData = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestExtensions.TestName);
                var claimSeq = testData["ClaimSequence"];
                var disabledTooltipWithAppeal = testData["ToolTipForClaimWithAppealProcess"];
                var lockedClaimMessageTooltip = testData["LockedClaimMessage"];

                _claimAction = automatedBase.CurrentPage.NavigateToClaimSearch()
                    .SearchByClaimSequenceToNavigateToClaimActionPage(claimSeq);

                DeletePreviousAppeals(automatedBase, _claimAction, claimSeq); //to delete appeal created
                
                _claimAction.RefreshPage(false);

                try
                {
                    AppealCreatorPage appealCreator = _claimAction.ClickOnCreateAppealIcon();
                    appealCreator.GetPageHeader().ShouldBeEqual(PageHeaderEnum.AppealCreator.GetStringValue(),
                        "Page header should equals");
                    appealCreator.IsRespectiveAppealTypeSelected(AppealType.DentalReview.GetStringValue())
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
                    _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
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
                    DeletePreviousAppeals(automatedBase, _claimAction, claimSeq); //to delete appeal created
                    if (_claimAction.GetPageHeader() != PageHeaderEnum.ClaimAction.GetStringValue())
                    {
                        _claimAction.NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(claimSeq);
                        ;
                    }
                }
            }
        }

        [Test, Category("NewClaimAction2")] //CAR-3046(CAR-3265)
        [Author("Pujan Aryal")]
        [NonParallelizable]
        public void Verify_Bulk_Update_Of_Sug_Code_While_Adding_Flag_For_Multiple_Lines()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(2))
            {
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var claimSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                    "ClaimSequence", "Value");
                var flag = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                    "Flag", "Value");
                var flagSource = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                    "FlagSource", "Value");
                var sugCode = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                    "SugCode", "Value");
                ClaimActionPage claimAction = automatedBase.CurrentPage.NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(claimSeq, true);
                try
                {
                    var lineCount = claimAction.GetCountOfClaimLines();
                    claimAction.ClickAddIconButton();
                    claimAction.ClickSelectAllLinesIcon();
                    claimAction.SelectAddInFlagAdd(flag);
                    claimAction.WaitForWorking();
                    claimAction.WaitForStaticTime(200);
                    claimAction.SelectFlagSourceInAddFlag(flagSource);
                    claimAction.WaitForStaticTime(200);
                    claimAction.IsSugCodeTextFieldEnabled().ShouldBeTrue("When adding a flag and more than one line is selected, if the flag allows the Sug Code, user will be able to enter a Sug Code value.");
                    claimAction.EnterSugCode(sugCode);
                    claimAction.WaitForStaticTime(200);
                    claimAction.ClickOnSaveEditButton();
                    claimAction.IsPageErrorPopupModalPresent().ShouldBeFalse("User should be allowed to perform bulk edit of Sug Code.");

                    StringFormatter.PrintMessage("Verify when save is selected and the flag is added to the selected lines, the Sug Code will also be added for each line selected and will be shown in Flag Details.");
                    for(int i = 1;i<=lineCount;i++)
                    {
                        claimAction.ClickOnFlagLineByLineNoWithFlag(i,flag);
                        claimAction.GetFlagDetailsByLabel("Sug Code").ShouldBeEqual(sugCode, "Sug Code Should Be saved in flag details");
                    }
                }
                finally
                {
                    StringFormatter.PrintMessage("Delete Added Flags in Finally Block");
                    claimAction.DeleteFlagLineFromDatabaseByClaimSeqAndFlag(claimSeq, flag);
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
            _claimAction.WaitForCondition(() => !_claimAction.IsWorkListControlDisplayed(),3000);
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

        void ReloadBrowserIfPageErrorPopupAppearsWhenTransferClaim(ClaimActionPage _claimAction, string statusCode = "Documentation Requested")
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

        

        private void VerifyVisibleToClientIconOnNoteRow(ClaimActionPage _claimAction, string currentUser)
        {
            _claimAction.ClickOnEditIconOnNotesByName(currentUser);
            if (_claimAction.IsVisibleToClientInNoteEditorSelectedByName(currentUser))
            {
                _claimAction.IsVisibletoClientIconPresentByName(currentUser).ShouldBeTrue("Visible to Client icon present");
                _claimAction.ClickVisibleToClientCheckboxInNoteEditorByName(currentUser);
                _claimAction.ClickOnSaveButtonInNoteEditorByName(currentUser);
                _claimAction.IsVisibletoClientIconPresentByName(currentUser).ShouldBeFalse("Visible to Client icon absent");
            }
            else
            {
                _claimAction.IsVisibletoClientIconPresentByName(currentUser).ShouldBeFalse("Visible to Client icon absent");
                _claimAction.ClickVisibleToClientCheckboxInNoteEditorByName(currentUser);
                _claimAction.ClickOnSaveButtonInNoteEditorByName(currentUser);
                _claimAction.IsVisibletoClientIconPresentByName(currentUser).ShouldBeTrue("Visible to Client icon present");
            }
        }

        private void VerifyCoreFlagNotDeleted(ClaimActionPage _claimAction, int linno, int row, string coreFlag)
        {
            _claimAction.IsFlaggedLineNotDeletedByLine(linno, row).ShouldBeTrue(
                string.Format("Core flag : {0} must not be deleted",
                    coreFlag));
        }


        private void CreateCiuReferral(ClaimActionPage _claimAction, string phoneNo = "231-456-7890", string ext = null,
             string patternCategory = "All", string identifiedPattern = "Test Pattern", string procCode = "1234")
        {
            _claimAction.ClickonAddCiuReferralIcon();
            if (ext != null)
                _claimAction.SetInputFieldOnCreateCiuReferralByLabel("Ext", ext);
            _claimAction.SetInputFieldOnCreateCiuReferralByLabel("Phone", phoneNo);
            _claimAction.InsertIdentifiedPattern(identifiedPattern);
            _claimAction.SetInputFieldOnCreateCiuReferralByLabel("Proc", procCode);
            _claimAction.SelectPatternCategory(patternCategory);
            _claimAction.ClickonSaveCiuReferralIcon();
            _claimAction.WaitForWorkingAjaxMessage();
        }

        private void ValidateRequiredFieldsAndTheirDefaultValues(ClaimActionPage _claimAction, string patternCategory)
        {
            _claimAction.SetCiuReferralFormInputValueGeneric("", "Phone Number");
            _claimAction.GetCiuReferralFormInputValueGeneric("Pattern Category").ShouldBeNullorEmpty("Pattern Category should be null by default.");
            _claimAction.GetCiuReferralFormInputValueGeneric("Proc Code").ShouldBeNullorEmpty("Proc Code should be null by default.");
            _claimAction.GetNoteOfCiuReferral().ShouldBeNullorEmpty("Note should be null by default.");
            _claimAction.ClickonSaveCiuReferralIcon();
            _claimAction.IsPageErrorPopupModalPresent().ShouldBeTrue("When required fields are missing a pop up message should be displayed");
            _claimAction.GetPageErrorMessage()
                .ShouldBeEqual("Invalid or missing data must be resolved before record can be saved.",
                    "Error pop up message should be as expected.");
            _claimAction.ClosePageError();
            _claimAction.GetCountOfInvalidRed()
                .ShouldBeEqual(4, "Red Exclamation Icon for all four required fields should be present and count should equal to 4");
            StringFormatter.PrintMessage("Validate identified pattern is a required field");
            const string phone = "1234567890";
            ValidateFieldIsRequiredAndErrorMessagePopUp(_claimAction, phone, patternCategory, "123", "", "note");
            StringFormatter.PrintMessage("Validate phone number is a required field");
            ValidateFieldIsRequiredAndErrorMessagePopUp(_claimAction, "", patternCategory, "123", "tests", "Phone Number");
            StringFormatter.PrintMessage("Validate proc code is a required field");
            ValidateFieldIsRequiredAndErrorMessagePopUp(_claimAction, phone, patternCategory, "", "test", "Proc Code");
            StringFormatter.PrintMessage("Validate pattern category is a required field");
            ValidateFieldIsRequiredAndErrorMessagePopUp(_claimAction, phone, "Clear", "123", "test", "Pattern Category");
            
            
            
        }

        private void ValidateFieldIsRequiredAndErrorMessagePopUp(ClaimActionPage _claimAction, string phoneNo,
            string patternCategory, string procCode, string note, string message)
        {
            _claimAction.SetCiuReferralFormInputValueGeneric( phoneNo,"Phone Number");
            _claimAction.SetCiuReferralFormInputValueGeneric(procCode, "Proc Code");
            _claimAction.GenericSelectSearchDropDownListValue("Pattern Category", patternCategory);
            _claimAction.SetNoteToCiuReferral(note);
            _claimAction.ClickonSaveCiuReferralIcon();
            _claimAction.IsPageErrorPopupModalPresent().ShouldBeTrue("When required fields are missing a pop up message should be displayed");
            _claimAction.GetPageErrorMessage()
                .ShouldBeEqual("Invalid or missing data must be resolved before record can be saved.",
                    "Error pop up message should be present when " + message + " field is empty.");
            _claimAction.ClosePageError();
            switch (message)
            {
                case "note":
                    _claimAction.IsInvalidInputPresentOnNoteByLabel("Identified Pattern")
                                .ShouldBeTrue("Red Exclamation Icon should be present when note is empty");
                    _claimAction.GetInvalidTooltipMessageOnNote()
                .ShouldBeEqual("Identified Pattern value is required before the record can be saved.",
                    "Error pop up message should be present when " + message + " field is empty.");
                    break;
                case "Phone Number":
                    _claimAction.IsInvalidInputPresentByLabel(message)
                        .ShouldBeTrue("Red Exclamation Icon should be present when phone no is empty");
                    _claimAction.GetInvalidInputToolTipByLabel(message)
                .ShouldBeEqual("10 digit Phone number is required before the record can be saved.",
                    "Error pop up message should be present when " + message + " field is empty.");
                    break;
                case "Proc Code":
                    _claimAction.IsInvalidInputPresentByLabel(message)
                        .ShouldBeTrue("Red Exclamation Icon should be present when proc code is empty");
                    _claimAction.GetInvalidInputToolTipByLabel(message)
                .ShouldBeEqual("Proc Code value is required before the record can be saved.",
                    "Error pop up message should be present when " + message + " field is empty.");
                    break;
                case "Pattern Category":
                    _claimAction.IsInvalidInputPresentByLabel(message)
                        .ShouldBeTrue("Red Exclamation Icon should be present when Pattern Category is empty");
                    break;
            }
        }
        private void DeletePreviousAppeals(NewAutomatedBaseParallelRun automatedBase, ClaimActionPage _claimAction, string claimSeq)
        {
            AppealManagerPage _appealManager;
            automatedBase.CurrentPage = _appealManager = automatedBase.CurrentPage.NavigateToAppealManager();
            _appealManager.DeleteAppealsAssociatedWithClaim(claimSeq);
            automatedBase.CurrentPage =
                     _claimAction = _appealManager.ClickOnQuickLaunch().NavigateToClaimSearch().SearchByClaimSequenceToNavigateToClaimActionPage(claimSeq); 
            _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
        }
        public void IsClaimRestrictiontooltipAsExpected(string restriction, string actual)
        {
            var expected =
                string.Format(
                    "This claim is allowed to be viewed only by users that have the {0} access. Please contact management if you are viewing this claim in error.",
                    restriction);
            actual.ShouldBeEqual(expected, "Expected tool tip message should be present for restricted claim, accessible through access assigned");
        }


        #endregion
    }
}

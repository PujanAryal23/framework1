using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using Nucleus.Service.Data;
using Nucleus.Service.PageServices.Appeal;
using Nucleus.Service.PageServices.Claim;
using Nucleus.Service.PageServices.Settings;
using Nucleus.Service.PageServices.Settings.User;
using Nucleus.Service.Support.Common.Constants;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Environment;
using Nucleus.UIAutomation.TestSuites.Base;
using NUnit.Framework;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Utils;

namespace Nucleus.UIAutomation.TestSuites.TestCases
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class AppealAction2
    {
        //#region PRIVATE FIELDS


        //private AppealSearchPage _appealSearch;
        //private AppealCreatorPage _appealCreator;
        //private AppealActionPage _appealAction;
        //private NewPopupCodePage _newPopupCode;
        //private AppealProcessingHistoryPage _appealProcessingHx;

        //private AppealManagerPage _appealManager;
        //private AppealSummaryPage _appealSummary;
        //private ProfileManagerPage _userProfileManager;
        //private ProfileManagerPage _profileManager;
        //private ClientSearchPage _clientSearch;


        //#endregion

        //#region OVERRIDE METHODS
        //protected override void ClassInit()
        //{
        //    try
        //    {
        //        base.ClassInit();
        //        automatedBase.CurrentPage = _appealSearch = automatedBase.QuickLaunch.NavigateToAppealSearch();
        //    }
        //    catch (Exception)
        //    {
        //        if (StartFlow != null)
        //            StartFlow.Dispose();
        //        throw;
        //    }
        //}
        //protected override void TestInit()
        //{
        //    base.TestInit();
        //    automatedBase.CurrentPage = _appealSearch;
        //}

        //protected override void TestCleanUp()
        //{
        //    automatedBase.CurrentPage.CloseAnyPopupIfExist();
        //    if (automatedBase.CurrentPage.GetPageHeader().Equals(PageHeaderEnum.AppealAction.GetStringValue()))
        //    {
        //        automatedBase.CurrentPage = _appealSearch = _appealAction.ClickOnSearchIconToNavigateAppealSearchPage();
        //    }
        //    if (!automatedBase.CurrentPage.GetPageHeader().Equals(PageHeaderEnum.AppealSearch.GetStringValue()))
        //    {
        //        automatedBase.CurrentPage = _appealSearch = automatedBase.CurrentPage.NavigateToAppealSearch();
        //    }

        //    _appealSearch.ClearAll();
        //    if (string.Compare(UserType.CurrentUserType, UserType.HCIADMIN, StringComparison.OrdinalIgnoreCase) != 0)
        //    {
        //        _appealSearch = _appealSearch.Logout().LoginAsHciAdminUser().NavigateToAppealSearch();
        //    }
        //    if (!automatedBase.CurrentPage.IsDefaultTestClientForEmberPage(automatedBase.EnvironmentManager.TestClient))
        //    {
        //        automatedBase.CurrentPage.ClickOnSwitchClient().SwitchClientTo(automatedBase.EnvironmentManager.TestClient);
        //        automatedBase.CurrentPage = _appealSearch = automatedBase.CurrentPage.NavigateToAppealSearch();
        //    }
        //    base.TestCleanUp();

        //}


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

        #region TEST SUITES


        [Test]//US53192
        public void Verify_that_view_document_icon_enabled_with_user_can_view_file_when_appeal_is_locked_by_other_user()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealSearchPage _appealSearch;
                AppealActionPage _appealAction;

                _appealSearch = automatedBase.QuickLaunch.NavigateToAppealSearch();
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var claseq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                    "ClaimSequence", "Value");

                try
                {

                    automatedBase.CurrentPage =
                        _appealSearch =
                            _appealSearch.Logout().LoginAsHciAdminUser4().NavigateToAppealSearch();

                    _appealSearch.SelectAllAppeals();
                    _appealSearch.SelectClientSmtst();
                    automatedBase.CurrentPage = _appealAction = _appealSearch.SearchByClaimSequenceToGoAppealAction(claseq, true);

                    var appealActionUrl = automatedBase.CurrentPage.CurrentPageUrl;
                    AppealActionPage newAppealAction =
                        _appealSearch.SwitchTabAndOpenAppealActionByUrl(appealActionUrl);
                    automatedBase.CurrentPage = newAppealAction;
                    newAppealAction.IsAppealLock().ShouldBeTrue("Appeal Should be Locked");
                    if (!_appealAction.IsAppealDocumentSectionPresent())
                        _appealAction.ClickOnAppealDocsIcon(true);
                    newAppealAction.ClickOnDocumentToViewAndStayOnPage(1); //window opens to view appeal document 
                    newAppealAction.GetOpenedDocumentText().ShouldBeEqual("TEST123", "document detail");
                    newAppealAction.CloseDocumentTabPageAndBackToAppealAction();
                    newAppealAction.ClickOnSearchIconToNavigateAppealSearchPage();
                }
                finally
                {
                    _appealSearch.CloseAnyPopupIfExist();
                }
            }
        }

        [Test]//US53191
        public void Verify_that_Claim_Action_Popup_should_display_when_navigate_to_Appeal_Action_with_claim_action_popup_replaced_previously_opened_claim()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealSearchPage _appealSearch;
                AppealActionPage _appealAction;

                _appealSearch = automatedBase.QuickLaunch.NavigateToAppealSearch();
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var claimSequence = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                    "ClaimSequence", "Value");
                var claimSequenceNext = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                    "ClaimSequenceNext", "Value");
                //IList<string> popups = new List<string>();
                _appealSearch.SelectAllAppeals();
                _appealSearch.SelectClientSmtst();
                try
                {
                    automatedBase.CurrentPage =
                        _appealAction =
                            _appealSearch.SearchByClaimSequenceToGoAppealAction(claimSequence, false);
                    _appealAction.GetWindowHandlesCount()
                        .ShouldBeEqual(2, "Claim Action Popup Should display when New Appeal Action Page Landed");
                    var popupUrl1 = PageUrlEnum.ClaimAction.GetStringValue() + "/" + claimSequence.Split('-')[0] + "/" +
                                    claimSequence.Split('-')[1];
                    _appealAction.SwitchClaimActionPopup(popupUrl1);

                    _appealAction.CurrentPageUrl.Contains(popupUrl1)
                        .ShouldBeTrue("Claim Action Popup should display");

                    //popups.Add(popupUrl1);
                    _appealAction.SwitchBackToAppealActionPage();
                    automatedBase.CurrentPage = _appealSearch = _appealAction.ClickOnSearchIconToNavigateAppealSearchPage();
                    automatedBase.CurrentPage =
                        _appealAction =
                            _appealSearch.SearchByClaimSequenceToGoAppealAction(claimSequenceNext, false);
                    var popupUrl2 = PageUrlEnum.ClaimAction.GetStringValue() + "/" + claimSequenceNext.Split('-')[0] +
                                    "/" + claimSequenceNext.Split('-')[1];
                    var claimActionPopup = _appealAction.SwitchClaimActionPopup(popupUrl2);
                    claimActionPopup.CurrentPageUrl.Contains(claimSequenceNext.Split('-')[0])
                        .ShouldBeTrue("Claim Action poup should replaced previous opened popup");
                    _appealAction.GetWindowHandlesCount()
                        .ShouldBeEqual(2, "Claim Action Poup Should replaced and total page count should be 2");
                    //popups.Add(popupUrl2);
                    _appealAction = _appealAction.ClosePopupOnAppealActionPage(popupUrl2);
                    _appealAction.SwitchBackToAppealActionPage();
                }
                finally
                {

                }
            }
        }

       
        [Test]//US50651//no reply
        [Retrying(Times = 3)]
        public void Verify_that_page_should_navigate_to_next_unlocked_appeal_of_searched_list_when_appeal_is_completed()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealSearchPage _appealSearch;
                AppealActionPage _appealAction;
                AppealManagerPage _appealManager;

                _appealSearch = automatedBase.QuickLaunch.NavigateToAppealSearch();
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var appealSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                    "AppealSeq", "Value");
                var reasonCode = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                    "ReasonCode", "Value");

                var rationale = "Test Deny" + DateTime.Now;

                automatedBase.CurrentPage.RefreshPage();
                automatedBase.CurrentPage = _appealAction = _appealSearch.NavigateToMyAppeals();
                _appealAction.WaitForWorking();

                try
                {
                    automatedBase.CurrentPage = _appealSearch = _appealAction.ClickOnSearchIconToNavigateAppealSearchPage();
                    var appealSeqList = _appealSearch.GetAppealSequenceList();
                    if (appealSeqList.Count != 0 && !appealSeqList.Contains(appealSeq))
                    {
                        ChangeStatusToNew(appealSeq);
                        _appealAction = _appealSearch.NavigateToMyAppeals();
                        automatedBase.CurrentPage = _appealSearch = _appealAction.ClickOnSearchIconToNavigateAppealSearchPage();
                    }

                    _appealSearch.ClickOnAdvancedSearchFilterIconForMyAppeal();
                    _appealSearch.SelectSearchDropDownListValue("Type", "Appeal");
                    _appealSearch.SelectSearchDropDownListValue("Client", ClientEnum.SMTST.ToString());
                    _appealSearch.ClickOnFindButton();
                    _appealSearch.WaitForWorkingAjaxMessage();

                    appealSeqList = _appealSearch.GetUnlockAppealSequenceListWithStatusNew();
                    automatedBase.CurrentPage = _appealAction =
                        _appealSearch.ClickOnAppealSequenceToNavigateAppealActionPageByAppealSequence(appealSeq);
                    if (string.IsNullOrEmpty(_appealAction.GetReasonCodeInput()))
                        _appealAction.SetReasonCodeList(reasonCode);
                    if (string.IsNullOrEmpty(_appealAction.GetEditAppealLineIframeEditorByHeader("Rationale")))
                        _appealAction.SetEditAppealLineIfrmeEditorByHeader("Rationale", rationale);
                    _appealAction.ClickOnAppealLetter();
                    _appealAction.ClickOnApproveIcon();
                    _appealAction.ClickOkCancelOnConfirmationModal(true);
                    _appealAction.WaitForWorking();
                    _appealAction.HandleAutomaticallyOpenedActionPopup();
                    if (appealSeqList.IndexOf(appealSeq) == 0)
                        _appealAction.GetAppealSequence().ShouldBeEqual(
                            appealSeqList[appealSeqList.IndexOf(appealSeq) + 1],
                            "Page Should redirect to next appeal sequence at the top of the list in New status that is not locked.");
                    else
                        _appealAction.GetAppealSequence().ShouldBeEqual(appealSeqList[0],
                            "Page Should redirect to next appeal sequence at the top of the list in New status that is not locked.");

                }
                finally
                {
                    automatedBase.CurrentPage.CloseAnyPopupIfExist();
                    if (automatedBase.CurrentPage.GetPageHeader().Equals(PageHeaderEnum.AppealAction.GetStringValue()))
                    {
                        automatedBase.CurrentPage = _appealSearch = _appealAction.ClickOnSearchIconToNavigateAppealSearchPage();
                    }

                    if (!automatedBase.CurrentPage.GetPageHeader().Equals(PageHeaderEnum.AppealSearch.GetStringValue()))
                    {
                        automatedBase.CurrentPage = _appealSearch = automatedBase.CurrentPage.NavigateToAppealSearch();
                    }

                    ChangeStatusToNew(appealSeq);
                }

                void ChangeStatusToNew(string appealSequence)
                {
                    automatedBase.CurrentPage = _appealManager = _appealSearch.NavigateToAppealManager();
                    _appealManager.SelectDropDownListbyInputLabel("Quick Search", "All Appeals");
                    _appealManager.SetInputFieldByInputLabel("Appeal Sequence", appealSequence);
                    _appealManager.ClickOnFindButton();
                    _appealManager.WaitForWorking();
                    _appealManager.ClickOnEditIcon();
                    _appealManager.SelectDropDownListbyInputLabel("Status", "New");
                    _appealManager.SetNote("UINOTE");
                    _appealManager.ClickOnSaveButton();
                    _appealManager.WaitForWorking();
                    automatedBase.CurrentPage = _appealSearch = _appealManager.NavigateToAppealSearch();
                }
            }
        }

        [Test]//US50608
        public void Verify_proper_display_client_notes_with_tooltip()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealSearchPage _appealSearch;
                AppealActionPage _appealAction;

                _appealSearch = automatedBase.QuickLaunch.NavigateToAppealSearch();
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var claimSeqWithNote = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                    "ClaimSeqWithNote", "Value");
                var claimSeqWithOutNote = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                    "ClaimSeqWithOutNote", "Value");
                const string clientNote =
                    "Verification of Client Note having product PhysicianClaimInsight and Created by Team Automation for Client Note Validation with ellipsis.";

                _appealSearch.SelectAllAppeals();
                _appealSearch.SelectClientSmtst();
                try
                {
                    automatedBase.CurrentPage =
                        _appealAction = _appealSearch.SearchByClaimSequenceToGoAppealAction(claimSeqWithNote);
                    _appealAction.IsEllipsisPresentAppealInformationByLabel("Client")
                        .ShouldBeTrue("Ellipsis Should displayed");
                    _appealAction.GetAppealInformationByLabel("Client")
                        .ShouldBeEqual(clientNote, "Verification of Client Note");
                    _appealAction.GetAppealInformationToolTipByLabel("Client")
                        .ShouldBeEqual(clientNote, "Verification of Client Note Tooltip");
                    automatedBase.CurrentPage = _appealSearch = _appealAction.ClickOnSearchIconToNavigateAppealSearchPage();
                    _appealSearch.SelectAllAppeals();
                    _appealSearch.SelectClientSmtst();
                    automatedBase.CurrentPage = _appealAction =
                        _appealSearch.SearchByClaimSequenceToGoAppealAction(claimSeqWithOutNote);
                    _appealAction.GetAppealInformationByLabel("Client").ShouldBeNullorEmpty("Client Note Should Empty");
                    _appealAction.GetAppealInformationToolTipByLabel("Client")
                        .ShouldBeNullorEmpty("Client Note Tooltip Should Empty");
                }
                finally
                {
                }
            }
        }

     
      
       
        [Test]//US45713
        public void Verify_based_on_product_of_top_flag_result_icon_enable_only_those_user_having_read_write_authority()
        {

            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealSearchPage _appealSearch;
                AppealActionPage _appealAction;

                _appealSearch = automatedBase.QuickLaunch.NavigateToAppealSearch();
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var claimSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                    "ClaimSequence", "Value");
                _appealSearch.SelectAllAppeals();
                _appealSearch.SelectClientSmtst();
                try
                {
                    automatedBase.CurrentPage = _appealAction = _appealSearch.SearchByClaimSequenceToGoAppealAction(claimSeq);
                    StringFormatter.PrintMessageTitle("Verification of Result Iocns for Read/Write Authority");
                    _appealAction.IsPayIconEnabled(2, 4).ShouldBeTrue("Pay Icon Enabled");
                    _appealAction.IsDenyIconEnabled(2, 4).ShouldBeTrue("Deny Icon Enabled");
                    _appealAction.IsAdjustIconEnabled(2, 4).ShouldBeTrue("Adjust Icon Enabled");
                    _appealAction.IsNoDocsIconEnabled(2, 4).ShouldBeTrue("No Docs Icon Enabled");

                    StringFormatter.PrintMessageTitle("Verification of Result Iocns for Read Only Authority");
                    automatedBase.CurrentPage = _appealSearch = _appealAction.ClickOnSearchIconToNavigateAppealSearchPage().Logout()
                        .LoginAsUserHavingNoManageEditAuthority()
                        .NavigateToAppealSearch();
                    _appealSearch.SelectAllAppeals();
                    _appealSearch.SelectClientSmtst();
                    automatedBase.CurrentPage = _appealAction = _appealSearch.SearchByClaimSequenceToGoAppealAction(claimSeq);
                    _appealAction.IsAppealResultTypeEllipsesDisabledInAppealAction(2, 4).ShouldBeTrue("Pay,Deny,Adjust,No Docs Icons Disabled");
                    automatedBase.CurrentPage = _appealSearch = _appealAction.ClickOnSearchIconToNavigateAppealSearchPage();
                    _appealSearch = _appealSearch.Logout().LoginAsHciAdminUser().NavigateToAppealSearch();
                }
                finally
                {


                }
            }

        }

        [Test]//US45713
        public void Verify_default_value_for_rationale_and_summary_for_record_review_and_appeal()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealSearchPage _appealSearch;
                AppealActionPage _appealAction;

                _appealSearch = automatedBase.QuickLaunch.NavigateToAppealSearch();
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var claimSeqRecordReview = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "ClaimSeqRecordReview", "Value");
                var claimSeqBlankPayDeny = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "ClaimSeqBlankPayDeny", "Value");
                _appealSearch.SelectAllAppeals();
                _appealSearch.SelectClientSmtst();
                try
                {
                    automatedBase.CurrentPage = _appealAction =
                        _appealSearch.SearchByClaimSequenceToGoAppealAction(claimSeqRecordReview);
                    _appealAction.ClickOnPayIcon();
                    _appealAction.GetEditAppealLineIframeEditorByHeader("Rationale")
                        .ShouldBeEqual("Record Review: Pay",
                            "Rationale  should automatically populated for Record Review");
                    _appealAction.GetEditAppealLineIframeEditorByHeader("Summary")
                        .ShouldBeNullorEmpty(
                            "Summary should blank for Record Review");

                    _appealAction.ClickOnCancelSaveDraft();
                    _appealAction.ClickOkCancelOnConfirmationModal(true);

                    _appealAction.ClickOnDenyIcon();

                    _appealAction.GetEditAppealLineIframeEditorByHeader("Rationale")
                        .ShouldBeEqual("Record Review: Deny",
                            "Rationale Text Areas should automatically populated for Record Review");


                    _appealAction.GetEditAppealLineIframeEditorByHeader("Summary")
                        .ShouldBeNullorEmpty(
                            "Summary should blank for Record Review");
                    _appealAction.ClickOnCancelSaveDraft();
                    _appealAction.ClickOkCancelOnConfirmationModal(true);

                    automatedBase.CurrentPage = _appealSearch = _appealAction.ClickOnSearchIconToNavigateAppealSearchPage();

                    _appealSearch.SelectAllAppeals();
                    _appealSearch.SelectClientSmtst();
                    automatedBase.CurrentPage = _appealAction =
                        _appealSearch.SearchByClaimSequenceToGoAppealAction(claimSeqBlankPayDeny);
                    _appealAction.ClickOnPayIcon();
                    _appealAction.GetEditAppealLineIframeEditorByHeader("Rationale")
                        .ShouldBeNullorEmpty(
                            "Rationale should blank when pre-defined text exists");
                    _appealAction.GetEditAppealLineIframeEditorByHeader("Summary")
                        .ShouldBeNullorEmpty(
                            "Summary should blank when pre-defined text exists");

                    _appealAction.ClickOnCancelSaveDraft();
                    _appealAction.ClickOkCancelOnConfirmationModal(true);

                    _appealAction.ClickOnDenyIcon();
                    _appealAction.GetEditAppealLineIframeEditorByHeader("Rationale")
                        .ShouldBeNullorEmpty(
                            "Rationale should blank when pre-defined text exists");
                    _appealAction.GetEditAppealLineIframeEditorByHeader("Summary")
                        .ShouldBeNullorEmpty(
                            "Summary should blank when pre-defined text exists");
                    _appealAction.ClickOnCancelSaveDraft();
                    _appealAction.ClickOkCancelOnConfirmationModal(true);
                }
                finally
                {

                }
            }
        }

        [Test]//US45713
        [Retrying(Times = 3)]
        public void Verify_appeal_Line_values_when_there_is_saved_draft()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealSearchPage _appealSearch;
                AppealActionPage _appealAction;

                _appealSearch = automatedBase.QuickLaunch.NavigateToAppealSearch();
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var claimSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                    "ClaimSequence", "Value");
                var denyReasonCode = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                    "DenyReasonCode", "Value");
                var payReasonCode = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                    "PayReasonCode", "Value");
                const string rationale = "Test Rationale";
                const string summary = "Test Summary";
                const string note = "Test Note";
                AppealProcessingHistoryPage appealProcessingHistory = null;
                _appealSearch.SelectAllAppeals();
                _appealSearch.SelectClientSmtst();
                try
                {
                    automatedBase.CurrentPage = _appealAction = _appealSearch.SearchByClaimSequenceToGoAppealAction(claimSeq);
                    _appealAction.ClickOnCancelSaveDraft();
                    _appealAction.ClickOkCancelOnConfirmationModal(true);
                    _appealAction.IsEditAppealLineSectionPresent()
                        .ShouldBeTrue("Edit Appeal Line Section Should not close for already saved draft");
                    _appealAction.IsVisibleToClientCheckBoxChecked()
                        .ShouldBeFalse("Visible to Client Should not checked by default");
                    _appealAction.GetEditAppealLineTextAreaByHeader("Note")
                        .ShouldNotBeEmpty("Note should displayed for note that is already added");

                    _appealAction.ClickOnPayIcon();
                    if (_appealAction.IsPageErrorPopupModalPresent())
                    {
                        _appealAction.ClickOkCancelOnConfirmationModal(true);
                    }

                    _appealAction.SetReasonCodeList(payReasonCode);
                    _appealAction.SetEditAppealLineIfrmeEditorByHeader("Rationale", rationale);
                    _appealAction.SetEditAppealLineIfrmeEditorByHeader("Summary", summary);
                    _appealAction.SetEditAppealLineTextAreaByHeader("Note", note);
                    _appealAction.ClickOnSaveDraftButton();
                    automatedBase.CurrentPage = _appealSearch = _appealAction.ClickOnSearchIconToNavigateAppealSearchPage();
                    _appealSearch.SelectAllAppeals();
                    _appealSearch.SelectClientSmtst();
                    automatedBase.CurrentPage = _appealAction = _appealSearch.SearchByClaimSequenceToGoAppealAction(claimSeq);
                    _appealAction.GetPayIconBackGroundColor()
                        .AssertIsContained("rgb(105, 168, 66)", "Pay Color is Green");
                    _appealAction.GetReasonCodeInput()
                        .ShouldBeEqual(payReasonCode,
                            "Reason Code Should saved");
                    _appealAction.GetEditAppealLineIframeEditorByHeader("Rationale")
                        .ShouldBeEqual(rationale, "Rationale should saved");
                    _appealAction.GetEditAppealLineIframeEditorByHeader("Summary")
                        .ShouldBeEqual(summary, "Summary should saved");
                    _appealAction.GetEditAppealLineTextAreaByHeader("Note")
                        .ShouldBeEqual(note, "Note should saved");

                    StringFormatter.PrintMessageTitle("Validation Audit Record Entry in Appeal Processing History");

                    _appealAction.ClickMoreOption();

                    appealProcessingHistory = _appealAction.ClickAppealProcessingHx();
                    var createdDate = DateTime.Now.Date.ToString("M/d/yyyy");
                    int n = 1;
                    do
                    {
                        if (appealProcessingHistory.GetAppealAuditGridTableDataValue(n, 3) == "Save")
                            break;
                        n++;
                    } while (n < 5);

                    appealProcessingHistory.GetAppealAuditGridTableDataValue(n, 1).Split(' ')[0]
                        .ShouldBeEqual(createdDate, "Modified Date :");
                    VerifyThatNameIsInCorrectWithUserNameFormat(
                        appealProcessingHistory.GetAppealAuditGridTableDataValue(n, 2), "Modified By ");
                    appealProcessingHistory.GetAppealAuditGridTableDataValue(n, 3).ShouldBeEqual("Save", "Action :");
                    appealProcessingHistory.GetAppealAuditGridTableDataValue(n, 4).ShouldBeEqual("Pay", "Result");
                    appealProcessingHistory.GetAppealAuditGridTableDataValue(n, 5)
                        .ShouldBeEqual(payReasonCode.Split('-')[0].Trim(), "Reason:");

                    appealProcessingHistory.GetAppealAuditGridTableDataValue(n, 6).ShouldBeEqual("", "Status :");
                    appealProcessingHistory.GetAppealAuditGridTableDataValue(n, 7)
                        .ShouldBeEqual(claimSeq, "Claim Sequence");
                    appealProcessingHistory.GetAppealAuditGridTableDataValue(n, 8).ShouldBeEqual("1", "Line No :");
                    appealProcessingHistory.GetAppealAuditGridTableDataValue(n, 9).ShouldBeEqual("", "Assigned To ");
                    appealProcessingHistory.GetAppealAuditGridTableDataValue(n, 10)
                        .ShouldBeEqual("", "Primary Reviewer ");
                    appealProcessingHistory.GetAppealAuditGridTableDataValue(n, 11).ShouldBeEqual("", "Billable Time");
                    appealProcessingHistory.GetAppealAuditGridTableDataValue(n, 12).ShouldBeEqual("", "DueDate ");
                    appealProcessingHistory.GetAppealAuditGridTableDataValue(n, 13).ShouldBeEqual("", "Priority :");
                    appealProcessingHistory.GetAppealAuditGridTableDataValue(n, 14).ShouldBeEqual("", "Product :");
                    appealProcessingHistory.GetAppealAuditGridTableDataValue(n, 15).ShouldBeEqual("", "Comments :");

                    appealProcessingHistory.CloseAppealProcessingHistoryPageBackToAppealActionPage();

                    _appealAction.ClickOnDenyIcon();
                    if (_appealAction.IsPageErrorPopupModalPresent())
                    {
                        _appealAction.ClickOkCancelOnConfirmationModal(true);
                    }

                    _appealAction.ClickOnSaveDraftButton();
                    automatedBase.CurrentPage = _appealSearch = _appealAction.ClickOnSearchIconToNavigateAppealSearchPage();
                    _appealSearch.SelectAllAppeals();
                    _appealSearch.SelectClientSmtst();
                    automatedBase.CurrentPage = _appealAction = _appealSearch.SearchByClaimSequenceToGoAppealAction(claimSeq);
                    _appealAction.GetDenyIconBackGroundColor()
                        .AssertIsContained("rgb(200, 32, 39)", "Deny Color is Red");

                    StringFormatter.PrintMessageTitle(
                        "Validation For approval/complete appeal when reason code and rationale text box empty");
                    _appealAction.SetReasonCodeList(denyReasonCode);
                    _appealAction.SetEditAppealLineIfrmeEditorByHeader("Rationale", "");
                    _appealAction.ClickOnAppealLetter();
                    _appealAction.ClickOnApproveIcon();
                    _appealAction.IsPageErrorPopupModalPresent().ShouldBeTrue("Popup Modal Should Open");
                    _appealAction.GetPageErrorMessage()
                        .ShouldBeEqual(
                            "A Result, Reason Code, and Rationale are required for all appeal lines in order to complete this appeal.",
                            "Error Message When Rationale is Empty");
                    _appealAction.ClosePageError();
                    _appealAction.IsInvalidInputPresentOnNoteByLabel("Rationale")
                        .ShouldBeTrue("Exclamation Icon Present in Rationale");

                    _appealAction.SetReasonCodeList("");
                    _appealAction.SetEditAppealLineIfrmeEditorByHeader("Rationale", rationale);
                    _appealAction.ClickOnApproveIcon();
                    _appealAction.IsPageErrorPopupModalPresent().ShouldBeTrue("Popup Modal Should Open");
                    _appealAction.GetPageErrorMessage()
                        .ShouldBeEqual(
                            "A Result, Reason Code, and Rationale are required for all appeal lines in order to complete this appeal.",
                            "Error Message When Rationale is Empty");
                    _appealAction.ClosePageError();
                    _appealAction.IsInvalidInputPresentByLabel("Reason Code")
                        .ShouldBeTrue("Eclamation Icon Present in Reason Code");
                    _appealAction.ClickOnAppealLetter();

                }
                finally
                {

                }

                void VerifyThatNameIsInCorrectWithUserNameFormat(string name, string message)
                {
                    Regex.IsMatch(name, @"^(\S+ )+\S+ +\(+\S+\)+$").ShouldBeTrue(message + " Name '" + name + "' is in format XXX XXX (XXX)");
                }
            }
        }

        [Test]//US45713
        public void Verify_proper_validation_appeal_line_field_with_switching_pay_deny_result_icon()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealSearchPage _appealSearch;
                AppealActionPage _appealAction;

                _appealSearch = automatedBase.QuickLaunch.NavigateToAppealSearch();
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var claimSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                    "ClaimSequence", "Value");

                _appealSearch.SelectAllAppeals();
                _appealSearch.SelectClientSmtst();
                try
                {
                    automatedBase.CurrentPage = _appealAction = _appealSearch.SearchByClaimSequenceToGoAppealAction(claimSeq);
                    var payReasonCodeList = _appealAction.GetAppealReasonCodesFromDB("F", "P");
                    var denyReasonCodeList = _appealAction.GetAppealReasonCodesFromDB("F", "D");
                    _appealAction.ClickOnPayIcon();
                    VerifyProperValidationEditAppealLine("Pay", payReasonCodeList);

                    var rationale = _appealAction.GetEditAppealLineIframeEditorByHeader("Rationale");
                    var summary = _appealAction.GetEditAppealLineIframeEditorByHeader("Summary");
                    var note = _appealAction.GetEditAppealLineTextAreaByHeader("Note");
                    _appealAction.ClickOnDenyIcon();

                    _appealAction.IsPageErrorPopupModalPresent()
                        .ShouldBeTrue("Modal Should open when click on different result icon");
                    _appealAction.GetPageErrorMessage()
                        .ShouldBeEqual("Selected line contains appeal details. Do you want to override them?",
                            "Modal Message");
                    _appealAction.ClickOkCancelOnConfirmationModal(false);

                    _appealAction.GetReasonCodeInput()
                        .ShouldBeEqual(payReasonCodeList[0],
                            "Reason Code Should not changed when click on cancel on confirmation");
                    _appealAction.GetEditAppealLineIframeEditorByHeader("Rationale")
                        .ShouldBeEqual(rationale, "Rationale should not changed when click on cancel on confirmation");
                    _appealAction.GetEditAppealLineIframeEditorByHeader("Summary")
                        .ShouldBeEqual(summary, "Summary should not changed when click on cancel on confirmation");
                    _appealAction.GetEditAppealLineTextAreaByHeader("Note")
                        .ShouldBeEqual(note, "Note should not changed when click on cancel on confirmation");

                    _appealAction.ClickOnDenyIcon();
                    _appealAction.ClickOkCancelOnConfirmationModal(true);
                    _appealAction.GetReasonCodeInput().ShouldBeNullorEmpty("Reason Code Should not selected");
                    _appealAction.GetEditAppealLineIframeEditorByHeader("Rationale")
                        .ShouldNotBeEqual(rationale, "Rationale should refresh after switching different result");
                    _appealAction.GetEditAppealLineIframeEditorByHeader("Summary")
                        .ShouldNotBeEqual(summary, "Summary should refresh after switching different result");
                    _appealAction.GetEditAppealLineTextAreaByHeader("Note")
                        .ShouldNotBeEqual(note, "Note should refresh after switching different result");


                    VerifyProperValidationEditAppealLine("Deny", denyReasonCodeList);
                    _appealAction.ClickOnCancelSaveDraft();
                    _appealAction.ClickOkCancelOnConfirmationModal(true);


                }
                finally
                {

                }

                void VerifyProperValidationEditAppealLine(string result, IList<string> reasonCodeList)
                {
                    const string rationale = "Test Rationale New";
                    const string summary = "Test Summary New";
                    const string note = "Test Note New";
                    StringFormatter.PrintMessageTitle(string.Format("Verify Proper Validation when result {0} selected", result));
                    if (result == "No Documents")
                    {
                        _appealAction.GetEditAppealLineHeaderText()
                            .ShouldBeEqual(result, "Verification of Edit Header Text");
                        _appealAction.GetReasonCodeInput().ShouldBeEqual(reasonCodeList[3], String.Format("Reason Code {0} Should selected", reasonCodeList[3]));
                    }
                    else
                    {
                        _appealAction.GetEditAppealLineHeaderText()
                            .ShouldBeEqual(string.Format("{0} Appeal Line", result), "Verification of Edit Header Text");
                        _appealAction.GetReasonCodeInput().ShouldBeNullorEmpty("Reason Code Should not selected");
                    }
                    ValidateReasonCode(reasonCodeList);
                    if (result == "No Documents")
                    {
                        _appealAction.GetEditAppealLineIframeEditorByHeader("Rationale")
                           .ShouldBeEqual("Test Rationale for Source-CL and Flag-AADD",
                               "Rationale Text Areas should automatically populated from rationales saved in Appeal Rationale Manager");
                        _appealAction.GetEditAppealLineIframeEditorByHeader("Summary")
                            .ShouldBeEqual(
                                "Cotiviti has either not received documentation for this appeal or it is incomplete. We are unable to make an appeal recommendation without complete documentation. If new documentation becomes available, please create a new appeal for this claim.",
                                "Summary Text Areas should automatically populated for Appeal Type");
                    }
                    else
                    {
                        _appealAction.GetEditAppealLineIframeEditorByHeader("Rationale")
                            .ShouldBeEqual("Test Rationale",
                                "Rationale Text Areas should automatically populated from rationales saved in Appeal Rationale Manager");
                        _appealAction.GetEditAppealLineIframeEditorByHeader("Summary")
                            .ShouldBeEqual(string.Format("Test Rationale {0}", result),
                                "Summary Text Areas should automatically populated from rationales saved in Appeal Rationale Manager");
                    }
                    var rationaleText = new string('a', 4494);
                    _appealAction.SetEditAppealLineIfrmeEditorByHeader("Rationale", rationaleText);
                    _appealAction.GetEditAppealLineIframeEditorByHeader("Rationale")
                        .Length.ShouldBeEqual(4493, "Rationale Text Area Max Length");
                    _appealAction.SetEditAppealLineIfrmeEditorByHeader("Summary", rationaleText);
                    _appealAction.GetEditAppealLineIframeEditorByHeader("Summary")
                        .Length.ShouldBeEqual(4493, "Summary Text Area Max Length");
                    _appealAction.SetEditAppealLineIfrmeEditorByHeader("Rationale", rationale);
                    _appealAction.SetEditAppealLineIfrmeEditorByHeader("Summary", summary);
                    _appealAction.SetEditAppealLineTextAreaByHeader("Note", note);
                    _appealAction.ClickOnCancelSaveDraft();
                    _appealAction.IsPageErrorPopupModalPresent().ShouldBeTrue("Modal Popup Should Present when click on cancel link");
                    _appealAction.GetPageErrorMessage()
                        .ShouldBeEqual("Any unsaved changes will be discarded. Do you wish to proceed?");
                    _appealAction.ClickOkCancelOnConfirmationModal(false);
                    _appealAction.GetEditAppealLineIframeEditorByHeader("Rationale")
                        .ShouldBeEqual(rationale, "Rationale should not changed when click on cancel on confirmation");
                    _appealAction.GetEditAppealLineIframeEditorByHeader("Summary")
                        .ShouldBeEqual(summary, "Summary should not changed when click on cancel on confirmation");
                    _appealAction.GetEditAppealLineTextAreaByHeader("Note")
                        .ShouldBeEqual(note, "Note should not changed when click on cancel on confirmation");
                }
                void ValidateReasonCode(IList<string> reasonCodeList)
                {
                    var actualReasonCodeList = _appealAction.GetReasonCodeList();
                    actualReasonCodeList.ShouldCollectionBeEqual(reasonCodeList, "Reason Code Should appropriate List");
                    actualReasonCodeList.IsInAscendingOrder().ShouldBeTrue("Reason Code List is in Alphabetical Order");
                    _appealAction.SetReasonCodeList(reasonCodeList[1]);
                    _appealAction.SetReasonCodeList(reasonCodeList[0]);
                    _appealAction.GetReasonCodeInput().ShouldBeEqual(reasonCodeList[0], "Only single value selection allowed for reason code");
                }
            }
        }

        [NonParallelizable]
        [Test, Category("OnDemand")]
        
        public void Verify_auto_generate_appeal_email_setting_with_appeal_email_form_info_and_verify_audit_entry_in_appeal_processing_history_after_email_sent()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealSearchPage _appealSearch;
                AppealActionPage _appealAction;
                ClientSearchPage _clientSearch;
                AppealSummaryPage _appealSummary = null;
                AppealProcessingHistoryPage _appealProcessingHx;

                _appealSearch = automatedBase.QuickLaunch.NavigateToAppealSearch();
                _appealSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search", AppealQuickSearchTypeEnum.OutstandingDCIAppeals.GetStringValue());
                _appealSearch.GetSideBarPanelSearch.ClickOnFindButton();
                _appealSearch.WaitForWorking();

                _appealAction = _appealSearch.ClickOnAppealSequenceToNavigateAppealActionPageByRow(1);
                _appealSearch = _appealAction.ClickOnSearchIconToNavigateAppealSearchPage();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var ClaimSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                    "DenyClaimSequenceForAppealAction", "Value");
                var createdByFirstName = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                    "CreatedByFirstNameOnly", "Value");
                var emailInternal = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                    "EmailInternal", "Value");
                var emailClient = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                    "EmailClient", "Value");
                var dueDate = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName, "DueDate",
                    "Value");

                var denyClaimSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                    "DenyClaimSequence", "Value");
                var denyAppealSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                    "DenyAppealSequence", "Value");
                var payAppealSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                    "PayAppealSequence", "Value");
                var payClaimSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                    "PayClaimSequence", "Value");
                var createdByFirstNameOnlyForAppealSummary = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "CreatedByFirstNameOnlyForAppealSummary", "Value");
                var sendemailSettingValue = true;


                try
                {
                    _appealSearch.UpdateAppealStatusToNew(ClaimSeq);
                    _appealSearch.GetCommonSql.UpdateSpecificClientDataInDB("AUTO_GENERATE_APPEAL_EMAIL='N'", ClientEnum.SMTST.ToString());
                    _appealSearch.RefreshPage();
                    sendemailSettingValue = false;
                    _appealSearch.SelectAllAppeals();
                    _appealSearch.SelectClientSmtst();
                    automatedBase.CurrentPage = _appealAction = _appealSearch.SearchByClaimSequenceToGoAppealAction(ClaimSeq);
                    _appealAction.ClickOnAppealLetter();
                    _appealAction.IsAppealEmailEnabled().ShouldBeFalse("Appeal Email Icon is enabled");
                    _appealAction.GetAppealEmailTitle()
                        .ShouldBeEqual("This client does not receive appeal emails.",
                            "Tooltip of Disabled Appeal Email");
                    _appealAction.ClickOnSearchIconToNavigateAppealSearchPage();

                    _appealSearch.SelectAllAppeals();
                    _appealSearch.SelectClientSmtst();
                    automatedBase.CurrentPage = _appealSummary = _appealSearch.SearchByClaimSequenceInCotivitiUser(denyClaimSeq);
                    _appealSummary.IsAppealEmailEnabled().ShouldBeFalse("Appeal Email Icon is enabled");
                    _appealSummary.GetAppealEmailTitle()
                        .ShouldBeEqual("This client does not receive appeal emails.",
                            "Tooltip of Disabled Appeal Email");

                    _appealSummary.ClickOnSearchIconToNavigateAppealSearchPage();

                    automatedBase.CurrentPage = _clientSearch = automatedBase.QuickLaunch.NavigateToClientSearch();
                    
                    _clientSearch.SearchByClientCodeToNavigateToClientProfileViewPage(ClientEnum.SMTST.ToString(),
                        ClientSettingsTabEnum.Product.GetStringValue());
                    _clientSearch.ClickOnRadioButtonByLabel(ProductAppealsEnum.SendAppealEmail.GetStringValue(), true);

                    var clientCC =
                        _clientSearch.GetTextAreaValueByLabel(ProductAppealsEnum.AppealEmailCC.GetStringValue());
                    _clientSearch.GetSideWindow.Save(waitForWorkingMessage: true);
                    sendemailSettingValue = true;
                    automatedBase.CurrentPage = _appealSearch = _clientSearch.NavigateToAppealSearch();
                    _appealSearch.SelectAllAppeals();
                    _appealSearch.SelectClientSmtst();
                    automatedBase.CurrentPage = _appealAction = _appealSearch.SearchByClaimSequenceToGoAppealAction(ClaimSeq);
                    _appealAction.ClickOnAppealLetter();
                    _appealAction.IsAppealEmailEnabled().ShouldBeTrue("Appeal Email Icon Should be enabled");

                    var appealSeq = _appealAction.GetAppealSequence();

                    _appealAction.IsRefreshIconPresent().ShouldBeTrue("Restore Icon Present");
                    _appealAction.ClickonEmailIcon();
                    _appealAction.IsRefreshIconPresent().ShouldBeFalse("Restore Icon Present");
                    _appealAction.GetToEmailInfo().ShouldBeEqual(createdByFirstName, "First Name should Equal");
                    _appealAction.GetClientCCValue().ShouldBeEqual(clientCC, "Client CC should equal");
                    _appealAction.SetAdditionalCCEmailInfo("test@verishealth.com");
                    _appealAction.GetAdditionalCCEmailInfo()
                        .ShouldBeEqual("test@verishealth.com", "Additional CC is editable and should equal");
                    _appealAction.SetAdditionalCCEmailInfo("");
                    _appealAction.SetToEmailInfo("Test");
                    _appealAction.GetToEmailInfo()
                        .ShouldBeEqual("Test", "To Email Input Field should same after modified");
                    _appealAction.GetEmailValue()
                        .ShouldBeEqual(emailInternal, "Email Should equal to who create appeal");
                    _appealAction.SetNote("UINOTE");
                    _appealAction.GetNote().ShouldBeEqual("UINOTE", "Note should equal to after modified");


                    _appealAction.ClickOnPayIcon();
                    if (_appealAction.IsPageErrorPopupModalPresent())
                    {
                        _appealAction.ClickOkCancelOnConfirmationModal(true);
                    }

                    _appealAction.GetTextMessage()
                        .ShouldBeEqual(
                            string.Format(
                                "To view our response to the appeal, click on the link below.Overturned/Adjusted: Appeal Seq {0}Please let us know if you have any questions regarding this appeal.Regards,Cotiviti Client Services",
                                appealSeq),
                            "Text Message Should Equal");

                    _appealAction.ClickOnDenyIcon();
                    if (_appealAction.IsPageErrorPopupModalPresent())
                    {
                        _appealAction.ClickOkCancelOnConfirmationModal(true);
                    }

                    _appealAction.GetTextMessage()
                        .ShouldBeEqual(
                            string.Format(
                                "To view our response to the appeal, click on the link below.Upheld: Appeal Seq {0}Please let us know if you have any questions regarding this appeal.Regards,Cotiviti Client Services",
                                appealSeq),
                            "Text Message Should Equal");

                    /// if (string.IsNullOrEmpty(_newAppealAction.GetReasonCodeInput()))
                    _appealAction.SetReasonCodeList("RRD - Record Review Deny");
                    // if (string.IsNullOrEmpty(_newAppealAction.GetEditAppealLineIframeEditorByHeader("Rationale")))
                    _appealAction.SetEditAppealLineIfrmeEditorByHeader("Rationale", "Test Rationale");


                    _appealAction.ClickOnApproveIcon();
                    _appealAction.IsPageErrorPopupModalPresent()
                        .ShouldBeTrue("Popup Should Present when approve button clicked");
                    _appealAction.GetPageErrorMessage()
                        .ShouldBeEqual("The appeal will be completed. Do you wish to continue?");
                    _appealAction.ClickOkCancelOnConfirmationModal(true);

                    var createdDate = DateTime.Now.Date.ToString("M/d/yyyy");
                    automatedBase.CurrentPage = _appealSummary = _appealSearch.SearchByClaimSequenceInCotivitiUser(ClaimSeq);
                    _appealSummary.ClickMoreOption();
                    _appealProcessingHx = _appealSummary.ClickAppealProcessingHx();

                    var row = 1;
                    while (row <= 5)
                    {
                        if (_appealProcessingHx.GetAppealAuditGridTableDataValue(row, 3) == "Email")
                            break;
                        row++;
                    }

                    _appealProcessingHx.GetAppealAuditGridTableDataValue(row, 1).Split(' ')[0]
                        .ShouldBeEqual(createdDate, "Modified Date :"); //modifiedDate
                    VerifyThatNameIsInCorrectWithUserNameFormat(
                        _appealProcessingHx.GetAppealAuditGridTableDataValue(row, 2), "Modified By");

                    _appealProcessingHx.GetAppealAuditGridTableDataValue(row, 3)
                        .ShouldBeEqual("Email", "Action :"); //Action

                    _appealProcessingHx.GetAppealAuditGridTableDataValue(row, 6)
                        .ShouldBeEqual("Complete", "Status :"); //Status
                    _appealProcessingHx.GetAppealAuditGridTableDataValue(row, 7)
                        .ShouldBeEqual("", "Claim Sequence"); //Claim Seq
                    _appealProcessingHx.GetAppealAuditGridTableDataValue(row, 8)
                        .ShouldBeEqual("", "Line No :"); //Line #
                    VerifyThatNameIsInCorrectWithUserNameFormat(
                        _appealProcessingHx.GetAppealAuditGridTableDataValue(row, 9), "Assigned To");

                    VerifyThatNameIsInCorrectWithUserNameFormat(
                        _appealProcessingHx.GetAppealAuditGridTableDataValue(row, 10), "Primary Reviewer"); //PR 

                    _appealProcessingHx.GetAppealAuditGridTableDataValue(row, 12)
                        .ShouldBeEqual(dueDate, "DueDate Should be as expected."); //Duedate # 
                    _appealProcessingHx.GetAppealAuditGridTableDataValue(row, 13)
                        .ShouldBeEqual("Normal", "Priority :"); //Priority
                    _appealProcessingHx.GetAppealAuditGridTableDataValue(row, 14)
                        .ShouldBeEqual("F", "Product :"); //Product
                    _appealProcessingHx.GetAppealAuditGridTableDataValue(row, 15)
                        .ShouldBeEqual("UINOTE", "Comments :"); //Comments
                    _appealProcessingHx.CloseAppealProcessingHistoryAndBackToAppealSummary();

                    _appealSummary.ClickOnSearchIconToNavigateAppealSearchPage();

                    _appealSearch.SelectAllAppeals();
                    _appealSearch.SelectClientSmtst();
                    automatedBase.CurrentPage = _appealSummary = _appealSearch.SearchByClaimSequenceInCotivitiUser(denyClaimSeq);
                    _appealSummary.IsAppealEmailEnabled().ShouldBeTrue("Appeal Email Icon Should enabled");

                    _appealSummary.ClickonEmailIcon();
                    _appealSummary.GetToEmailInfo().ShouldBeEqual(createdByFirstNameOnlyForAppealSummary,
                        "First Name should Equal");
                    _appealSummary.GetClientCCValue().ShouldBeEqual(clientCC, "Client CC should equal");
                    _appealSummary.SetAdditionalCCEmailInfo("test@verishealth.com");
                    _appealSummary.GetAdditionalCCEmailInfo()
                        .ShouldBeEqual("test@verishealth.com", "Additional CC is editable and should equal");
                    _appealSummary.SetAdditionalCCEmailInfo("");
                    _appealSummary.SetToEmailInfo("Test");
                    _appealSummary.GetToEmailInfo()
                        .ShouldBeEqual("Test", "To Email Input Field should same after modified");
                    _appealSummary.GetEmailValue()
                        .ShouldBeEqual(emailClient, "Email Should equal to who create appeal");
                    _appealSummary.SetNote("UINOTE");
                    _appealSummary.GetNote().ShouldBeEqual("UINOTE", "Note should equal to after modified");
                    _appealSummary.SetNote("");
                    _appealSummary.GetTextMessage()
                        .ShouldBeEqual(
                            string.Format(
                                "To view our response to the appeal, click on the link below.Upheld: Appeal Seq {0}Please let us know if you have any questions regarding this appeal.Regards,Cotiviti Client Services",
                                denyAppealSeq),
                            "Text Message Should Equal");
                    _appealSummary.ClickOnCancelLinkOnAppealEmail();
                    _appealSummary.IsPageErrorPopupModalPresent().ShouldBeTrue("Confiramtion Modal Should Present");
                    _appealSummary.GetPageErrorMessage()
                        .ShouldBeEqual("Your changes will be discarded. Do you wish to proceed?",
                            "Popup Message Should Equal");
                    _appealSummary.ClickOkCancelOnConfirmationModal(false);
                    _appealSummary.IsAppealEmailFormPresent()
                        .ShouldBeTrue("Appeal Email Form dispalyed after cancel button clicked");
                    _appealSummary.ClickOnCancelLinkOnAppealEmail();
                    _appealSummary.IsPageErrorPopupModalPresent().ShouldBeTrue("Confiramtion Modal Should Present");
                    _appealSummary.GetPageErrorMessage()
                        .ShouldBeEqual("Your changes will be discarded. Do you wish to proceed?",
                            "Popup Message Should Equal");
                    _appealSummary.ClickOkCancelOnConfirmationModal(true);
                    automatedBase.CurrentPage = _appealSearch = _appealSummary.ClickOnSearchIconToNavigateAppealSearchPage();
                    _appealSearch.SelectAllAppeals();
                    _appealSearch.SelectClientSmtst();
                    automatedBase.CurrentPage = _appealSummary = _appealSearch.SearchByClaimSequenceInCotivitiUser(payClaimSeq);
                    _appealSummary.ClickonEmailIcon();
                    _appealSummary.GetTextMessage()
                        .ShouldBeEqual(
                            string.Format(
                                "To view our response to the appeal, click on the link below.Overturned/Adjusted: Appeal Seq {0}Please let us know if you have any questions regarding this appeal.Regards,Cotiviti Client Services",
                                payAppealSeq),
                            "Text Message Should Equal");
                    _appealSummary.ClickOnSendEmail();
                    _appealSummary.IsAppealEmailFormPresent()
                        .ShouldBeFalse("Appeal Email Form dispalyed after email sent without note");

                    var dueDateFromAppealSummary =
                        DateTime.Parse(_appealSummary.GetAppealDetails(1, 3)).ToString("M/d/yyyy");
                    _appealSummary.ClickMoreOption();
                    _appealProcessingHx = _appealSummary.ClickAppealProcessingHx();

                    row = 1;
                    while (row <= 4)
                    {
                        if (_appealProcessingHx.GetAppealAuditGridTableDataValue(row, 3) == "Email")
                        {
                            break;
                        }

                        row++;


                    }

                    _appealProcessingHx.GetAppealAuditGridTableDataValue(row, 1).Split(' ')[0]
                        .ShouldBeEqual(createdDate, "Modified Date :"); //modifiedDate
                    VerifyThatNameIsInCorrectWithUserNameFormat(
                        _appealProcessingHx.GetAppealAuditGridTableDataValue(row, 2), "Modified By");

                    _appealProcessingHx.GetAppealAuditGridTableDataValue(row, 3)
                        .ShouldBeEqual("Email", "Action :"); //Action

                    _appealProcessingHx.GetAppealAuditGridTableDataValue(row, 6)
                        .ShouldBeEqual("Complete", "Status :"); //Status
                    _appealProcessingHx.GetAppealAuditGridTableDataValue(row, 7)
                        .ShouldBeEqual("", "Claim Sequence"); //Claim Seq
                    _appealProcessingHx.GetAppealAuditGridTableDataValue(row, 8)
                        .ShouldBeEqual("", "Line No :"); //Line #
                    VerifyThatNameIsInCorrectWithUserNameFormat(
                        _appealProcessingHx.GetAppealAuditGridTableDataValue(row, 9), "Assigned To");

                    VerifyThatNameIsInCorrectWithUserNameFormat(
                        _appealProcessingHx.GetAppealAuditGridTableDataValue(row, 10), "Primary Reviewer"); //PR 

                    _appealProcessingHx.GetAppealAuditGridTableDataValue(row, 12)
                        .ShouldBeEqual(dueDateFromAppealSummary, "DueDate Should be Empty"); //Duedate # 
                    _appealProcessingHx.GetAppealAuditGridTableDataValue(row, 13)
                        .ShouldBeEqual("Normal", "Priority :"); //Priority
                    _appealProcessingHx.GetAppealAuditGridTableDataValue(row, 14)
                        .ShouldBeEqual("F", "Product :"); //Product
                    _appealProcessingHx.GetAppealAuditGridTableDataValue(row, 15)
                        .ShouldBeEqual("UINOTE", "Comments :"); //Comments
                    _appealProcessingHx.CloseAppealProcessingHistoryPageToAppealSummaryPage();

                }
                finally
                {
                    if (!sendemailSettingValue)
                    {
                        _appealAction.GetCommonSql.UpdateSpecificClientDataInDB("AUTO_GENERATE_APPEAL_EMAIL='Y'", ClientEnum.SMTST.ToString());
                    }
                    else
                    {
                        if (automatedBase.CurrentPage.GetPageHeader() == PageHeaderEnum.AppealAction.GetStringValue())
                            _appealAction.ClickOnSearchIconToNavigateAppealSearchPage();
                        if (automatedBase.CurrentPage.GetPageHeader() == PageHeaderEnum.AppealSummary.GetStringValue())
                            _appealSummary.ClickOnSearchIconToNavigateAppealSearchPage();
                    }

                    _appealSearch.UpdateAppealStatusToNew(ClaimSeq);

                }

                void VerifyThatNameIsInCorrectWithUserNameFormat(string name, string message)
                {
                    Regex.IsMatch(name, @"^(\S+ )+\S+ +\(+\S+\)+$").ShouldBeTrue(message + " Name '" + name + "' is in format XXX XXX (XXX)");
                }
            }
        }

        [NonParallelizable]
        [Test,Category("OnDemand")] //TE-753
        public void Verify_Email_Is_Disabled_For_Dental_Review_Appeals()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealSearchPage _appealSearch;
                AppealActionPage _appealAction;
                AppealProcessingHistoryPage _appealProcessingHx;
                AppealSummaryPage _appealSummary;

                _appealSearch = automatedBase.QuickLaunch.NavigateToAppealSearch();
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var claimSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                    "ClaimSequence", "Value");
                var date = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName, "AuditDate",
                    "Value");
                var sendEmailSettingValue = true;
                _appealSearch.UpdateAppealStatusToNew(claimSeq);
                _appealSearch.DeleteAppealAuditByClaseq(claimSeq, date);

                try
                {
                    StringFormatter.PrintMessage(
                        "Verify Manage Appeal Email Icon Is Not Present Regardless Of Client Setting");
                    _appealSearch.GetCommonSql.UpdateSpecificClientDataInDB("AUTO_GENERATE_APPEAL_EMAIL='N'", ClientEnum.SMTST.ToString());
                    _appealSearch.RefreshPage();
                    sendEmailSettingValue = false;
                    _appealSearch.SelectAllAppeals();
                    _appealSearch.SelectClientSmtst();
                    automatedBase.CurrentPage = _appealAction = _appealSearch.SearchByClaimSequenceToGoAppealAction(claimSeq);
                    _appealAction.ClickOnAppealLetter();
                    _appealAction.IsManageAppealEmailIconPresent().ShouldBeFalse(
                        "Appeal Email Icon should not be present when send appeal email setting is turned off");
                    _appealAction.ClickOnlySearchIcon();
                    if (_appealAction.IsPageErrorPopupModalPresent())
                    {
                        _appealAction.ClickOkCancelOnConfirmationModal(true);
                    }

                    _appealAction.GetCommonSql.UpdateSpecificClientDataInDB("AUTO_GENERATE_APPEAL_EMAIL='Y'", ClientEnum.SMTST.ToString());
                    sendEmailSettingValue = true;
                    _appealSearch = automatedBase.CurrentPage.NavigateToAppealSearch();
                    automatedBase.CurrentPage.RefreshPage();
                    _appealSearch.SelectAllAppeals();
                    _appealSearch.SelectClientSmtst();
                    automatedBase.CurrentPage = _appealAction = _appealSearch.SearchByClaimSequenceToGoAppealAction(claimSeq);
                    _appealAction.ClickOnAppealLetter();
                    _appealAction.IsManageAppealEmailIconPresent().ShouldBeFalse(
                        "Appeal Email Icon should not be present when send appeal email setting is turned on");


                    StringFormatter.PrintMessage(
                        "Verify Email Icon Is Not Present In Appeal Summary Page and Email Audit Is Not Present In Appeal Processing History Page");
                    _appealAction.ClickOnDenyIcon();
                    if (_appealAction.IsPageErrorPopupModalPresent())
                    {
                        _appealAction.ClickOkCancelOnConfirmationModal(true);
                    }

                    _appealAction.SetReasonCodeList("RRD - Record Review Deny");
                    _appealAction.SetEditAppealLineIfrmeEditorByHeader("Rationale", "Test Rationale");
                    _appealAction.ClickOnApproveIcon();
                    _appealAction.IsPageErrorPopupModalPresent()
                        .ShouldBeTrue("Popup Should Present when approve button clicked");
                    _appealAction.GetPageErrorMessage()
                        .ShouldBeEqual("The appeal will be completed. Do you wish to continue?");
                    _appealAction.ClickOkCancelOnConfirmationModal(true);
                    _appealSearch.SelectAllAppeals();
                    _appealSearch.SelectClientSmtst();
                    automatedBase.CurrentPage = _appealSummary = _appealSearch.SearchByClaimSequenceInCotivitiUser(claimSeq);
                    _appealSummary.IsManageAppealEmailIconPresent()
                        .ShouldBeFalse("Appeal Email Icon Should Not Be Present");
                    _appealSummary.ClickMoreOption();
                    _appealProcessingHx = _appealSummary.ClickAppealProcessingHx();
                    _appealProcessingHx.GetAppealAuditGridTableDataValuesByColumn(3).Distinct()
                        .ShouldNotContain("Email", "Appeal Email Audit should not be recorded");
                    _appealProcessingHx.CloseAppealProcessingHistoryPageToAppealSummaryPage();

                }
                finally
                {
                    if (!sendEmailSettingValue)
                    {
                        _appealSearch.GetCommonSql.UpdateSpecificClientDataInDB("AUTO_GENERATE_APPEAL_EMAIL='Y'", ClientEnum.SMTST.ToString());
                    }

                    _appealSearch.UpdateAppealStatusToNew(claimSeq);
                }

            }
        }


        [Test]
        public void Verify_documents_presence_in_appeal_action()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealSearchPage _appealSearch;
                AppealActionPage _appealAction;

                _appealSearch = automatedBase.QuickLaunch.NavigateToAppealSearch();
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> testData = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestExtensions.TestName);
                _appealSearch.SelectAllAppeals();
                _appealSearch.SelectClientSmtst();
                var expectedDocInfo = testData["DocInfo"].Split(';');
                StringFormatter.PrintMessageTitle("Verify Appeal Document Details when documents present");
                automatedBase.CurrentPage =
                    _appealAction =
                        _appealSearch.SearchByClaimSequenceToGoAppealAction(testData["ClaimSequenceWithDocs"]);
                _appealAction.IsAppealDocumentIconPresent().ShouldBeTrue("Appeal documents icon present");
                _appealAction.IsAppealDocumentSelected()
                    .ShouldBeTrue("Appeal documents displayed when docs present");
                _appealAction.IsAppealDocumentSectionPresent()
                    .ShouldBeTrue("Appeal documents list section should be open");
                _appealAction.ClickOnAppealDocsIcon(false);
                _appealAction.IsAppealDocumentSectionPresent()
                    .ShouldBeFalse("Appeal documents list open should be false");
                _appealAction.ClickOnAppealDocsIcon(true);
                _appealAction.AppealDocumentListCount()
                    .ShouldBeGreater(0, "List of documents associated displayed");
                _appealAction.IsListSortedInDescendingOrder()
                    .ShouldBeTrue("Appeal documents listed with most recent on top");
                for (int i = 0; i < _appealAction.AppealDocumentListCount(); i++)
                {
                    var singleDocInfo = expectedDocInfo[i].Split(',');
                    _appealAction.GetAppealDocInfo(i + 1, 1, 1)
                        .ShouldBeEqual(singleDocInfo[0], "Filename for appeal doc");
                    _appealAction.GetAppealDocInfoToolTip(i + 1, 1, 1)
                        .ShouldBeEqual(singleDocInfo[0], "Filename for appeal doc in tool tip");
                    _appealAction.GetAppealDocInfo(i + 1, 1, 2)
                        .ShouldBeEqual(singleDocInfo[1], "File type for appeal doc");
                    _appealAction.GetAppealDocInfoToolTip(i + 1, 1, 2).ShouldBeEqual(singleDocInfo[1],
                        "File type for appeal doc in tool tip");
                    VerifyDateStampIsInCorrectFormat(_appealAction.GetAppealDocInfo(i + 1, 1, 3),
                        "Date time stamp of file uploaded.");
                    _appealAction.GetAppealDocInfo(i + 1, 2, 1)
                        .ShouldBeEqual(singleDocInfo[2], "File description for appeal doc");
                    _appealAction.GetAppealDocInfoToolTip(i + 1, 2, 1).ShouldBeEqual(singleDocInfo[2],
                        "File description for appeal doc in tool tip");
                }

                _appealAction.ClickOnDocumentToViewAndStayOnPage(1); //window opens to view appeal document 
                _appealAction.CloseDocumentTabPageAndBackToAppealAction();
                _appealAction.ClickOnAppealLetter();
                _appealAction.IsAppealDocumentSectionPresent()
                    .ShouldBeFalse("Appeal documents list open should be false");
                _appealAction.IsAppealLetterSectionPresent()
                    .ShouldBeTrue("Appeal letter section open should be true");

                StringFormatter.PrintMessageTitle("Verify Appeal Document Section when no documents present");
                automatedBase.CurrentPage = _appealSearch = _appealAction.ClickOnSearchIconToNavigateAppealSearchPage();
                _appealSearch.SelectAllAppeals();
                _appealSearch.SelectClientSmtst();
                automatedBase.CurrentPage =
                    _appealAction =
                        _appealSearch.SearchByClaimSequenceToGoAppealAction(testData["ClaimSequenceWithOutDocs"]);
                (!_appealAction.IsAppealDocumentSectionPresent()).ShouldBeTrue(
                    "Appeal documents list open should not display when no document present");
                void VerifyDateStampIsInCorrectFormat(string date, string message)
                {
                    // string[] format = new string[] { "mm/dd/yyyy h:mm:ss tt" };
                    DateTime datetime;

                    DateTime.TryParse(date, out datetime).ShouldBeTrue(message + " '" + date + "' is date time stamp");

                }
            }
        }
       
        [Test]//US65045
        public void Verify_known_code_in_appeal_action_for_revenue_and_proc_code()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealSearchPage _appealSearch;
                AppealActionPage _appealAction;
                NewPopupCodePage _newPopupCode;

                _appealSearch = automatedBase.QuickLaunch.NavigateToAppealSearch();
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> testData =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var claimSequence = testData["ClaimSequence"];
                var revCodeDescription = testData["RevCodeDescription"];
                var procCodeDescription = testData["ProcCodeDescription"];
                _appealSearch.SelectAllAppeals();
                _appealSearch.SelectClientSmtst();
                try
                {
                    automatedBase.CurrentPage = _appealAction = _appealSearch.SearchByClaimSequenceToGoAppealAction(claimSequence);
                    var revCode = _appealAction.GetRevCode();
                    _newPopupCode = _appealAction.ClickOnRevenueCodeandSwitch("Revenue Code - " + revCode);
                    var popupUrl = _newPopupCode.GetPopUpCurrentUrl();
                    popupUrl.ShouldNotContain("title", "Url doesnot have title Query String parameter");
                    popupUrl.AssertIsContained(PageUrlEnum.MvcUrl.GetStringValue(),
                        string.Format("Procedure code pop up link points to mvc page <{0}>",
                            PageUrlEnum.MvcUrl.GetStringValue()));
                    popupUrl.ShouldNotContain("Code=Unknown",
                        "MVC page query string parameter has code value. Unknown displayed should be false.");
                    popupUrl.AssertIsContained("code=" + revCode,
                        "MVC page query string parameter has code value. Code " + revCode + " displayed is true.");
                    _newPopupCode.GetTextValueinLiTag(1).ShouldBeEqual(("Code: " + revCode),
                        "Code displayed is same value as in type of page header and query string parameter is ignored.");
                    _newPopupCode.GetTextValueinLiTag(2).ShouldBeEqual(
                        string.Concat("Type: ", _newPopupCode.GetPopupHeaderText()),
                        "Type is same as page header of the pop up.");
                    _newPopupCode.GetTextValueinLiTag(3).ShouldBeEqual(
                        string.Concat("Description\r\n", revCodeDescription),
                        "Description is same as in the xml file.");
                    _appealAction = _newPopupCode.ClosePopupOnAppealActionPage("Revenue Code - " + revCode);
                    var procCode = _appealAction.GetProcCode();
                    _newPopupCode = _appealAction.ClickOnProcCodeandSwitch("HCPCS Code - " + procCode);
                    popupUrl = _newPopupCode.GetPopUpCurrentUrl();
                    popupUrl.ShouldNotContain("title", "Url doesnot have title Query String parameter");
                    popupUrl.AssertIsContained(PageUrlEnum.MvcUrl.GetStringValue(),
                        string.Format("Procedure code pop up link points to mvc page <{0}>",
                            PageUrlEnum.MvcUrl.GetStringValue()));
                    popupUrl.ShouldNotContain("Code=Unknown",
                        "MVC page query string parameter has code value. Unknown displayed should be false.");
                    popupUrl.AssertIsContained("code=" + procCode,
                        "MVC page query string parameter has code value. Code " + procCode + " displayed is true.");
                    _newPopupCode.GetTextValueinLiTag(1).ShouldBeEqual(("Code: " + procCode),
                        "Code displayed is same value as in type of page header and query string parameter is ignored.");
                    _newPopupCode.GetTextValueinLiTag(2).ShouldBeEqual(
                        string.Concat("Type: ", _newPopupCode.GetPopupHeaderText()),
                        "Type is same as page header of the pop up.");
                    _newPopupCode.GetTextValueinLiTag(3).ShouldBeEqual(
                        string.Concat("Description\r\n", procCodeDescription),
                        "Description is same as in the xml file.");
                    _appealAction = _newPopupCode.ClosePopupOnAppealActionPage("HCPCS Code - " + procCode);
                }
                finally
                {

                }
            }
        }

        [Test]//US65629
        public void Verify_unknown_code_in_appeal_action()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealSearchPage _appealSearch;
                AppealActionPage _appealAction;
                NewPopupCodePage _newPopupCode;

                _appealSearch = automatedBase.QuickLaunch.NavigateToAppealSearch();
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> testData =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var claimSequence = testData["ClaimSequence"];
                var codeDescription = testData["CodeDescription"];
                _appealSearch.SelectAllAppeals();
                _appealSearch.SelectClientSmtst();
                try
                {
                    automatedBase.CurrentPage = _appealAction = _appealSearch.SearchByClaimSequenceToGoAppealAction(claimSequence);
                    var revCode = _appealAction.GetRevCode();
                    _newPopupCode = _appealAction.ClickOnRevenueCodeandSwitch("Revenue Code - Unknown");
                    var popupUrl = _newPopupCode.GetPopUpCurrentUrl();
                    popupUrl.ShouldNotContain("title", "Url doesnot have title Query String parameter");
                    popupUrl.AssertIsContained(PageUrlEnum.MvcUrl.GetStringValue(),
                        string.Format("Procedure code pop up link points to mvc page <{0}>",
                            PageUrlEnum.MvcUrl.GetStringValue()));
                    popupUrl.ShouldNotContain("Code=Unknown",
                        "MVC page query string parameter has code value. Unknown displayed should be false.");
                    popupUrl.AssertIsContained("code=" + revCode,
                        "MVC page query string parameter has code value. Code " + revCode + " displayed is true.");
                    _newPopupCode.GetTextValueinLiTag(1).ShouldBeEqual(("Code: Unknown"),
                        "Code displayed is same value as in type of page header and query string parameter is ignored.");
                    _newPopupCode.GetTextValueinLiTag(2).ShouldBeEqual(
                        string.Concat("Type: ", _newPopupCode.GetPopupHeaderText()),
                        "Type is same as page header of the pop up.");
                    _newPopupCode.GetTextValueinLiTag(3)
                        .ShouldBeEqual(string.Concat("Description\r\n", codeDescription),
                            "Description is as the expected");
                    _appealAction = _newPopupCode.ClosePopupOnAppealActionPage("Revenue Code - Unknown");
                    var procCode = _appealAction.GetProcCode();
                    _newPopupCode = _appealAction.ClickOnProcCodeandSwitch("- Unknown");
                    popupUrl = _newPopupCode.GetPopUpCurrentUrl();
                    popupUrl.ShouldNotContain("title", "Url doesnot have title Query String parameter");
                    popupUrl.AssertIsContained(PageUrlEnum.MvcUrl.GetStringValue(),
                        string.Format("Procedure code pop up link points to mvc page <{0}>",
                            PageUrlEnum.MvcUrl.GetStringValue()));
                    popupUrl.ShouldNotContain("Code=Unknown",
                        "MVC page query string parameter has code value. Unknown displayed should be false.");
                    popupUrl.AssertIsContained("code=" + procCode,
                        "MVC page query string parameter has code value. Code " + procCode + " displayed is true.");
                    _newPopupCode.GetTextValueinLiTag(1).ShouldBeEqual(("Code: Unknown"),
                        "Code displayed is same value as in type of page header and query string parameter is ignored.");
                    _newPopupCode.GetTextValueinLiTag(2).ShouldBeEqual(("Type:"), "Type is empty.");
                    _newPopupCode.GetTextValueinLiTag(3)
                        .ShouldBeEqual(string.Concat("Description\r\n", codeDescription),
                            "Description is as the expected.");
                    _appealAction = _newPopupCode.ClosePopupOnAppealActionPage("- Unknown");
                }
                finally
                {

                }
            }
        }



        [Test]//US69656
        public void Verify_Appeal_results_buttons_are_disabled_for_locked_appeal()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealSearchPage _appealSearch;
                AppealActionPage _appealAction;

                _appealSearch = automatedBase.QuickLaunch.NavigateToAppealSearch();
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var claimSequence = automatedBase.DataHelper
                    .GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName, "ClaimSequence", "Value");

                automatedBase.CurrentPage = automatedBase.QuickLaunch = _appealSearch.Logout().LoginAsHciAdminUser5();
                automatedBase.CheckTestClientAndSwitch();
                automatedBase.CurrentPage = _appealSearch = automatedBase.QuickLaunch.NavigateToAppealSearch();
                _appealSearch.DeleteAppealLockByClaimSeq(claimSequence);
                try
                {
                    _appealSearch.SelectAllAppeals();
                    _appealSearch.SelectClientSmtst();
                    automatedBase.CurrentPage = _appealAction = _appealSearch.SearchByClaimSequenceToGoAppealAction(claimSequence, true);
                    StringFormatter.PrintMessage(
                        string.Format("Open appeal aciton page for claim sequence: {0} with direct URL ",
                            claimSequence));
                    var newAppealActionUrl = automatedBase.CurrentPage.CurrentPageUrl;
                    _appealAction =
                        _appealSearch.SwitchToOpenAppealActionByUrlForAdmin(newAppealActionUrl);

                    _appealAction.IsAppealLock()
                        .ShouldBeTrue("Appeal should be locked when it is in view mode by another user");
                    _appealAction.GetAppealLockToolTip()
                        .ShouldBeEqual(
                            "This appeal is currently locked for editing by Test Automation5",
                            "Is Lock Message Equal?");
                    _appealAction.IsAppealResultTypeEllipsesDisabledInAppealAction().ShouldBeTrue("Is Deny,Pay,Adjust,No Docs icons disabled?");
                    _appealAction.CloseCurrentWindowAndSwitchToOriginal();
                }
                finally
                {

                }
            }
        }

        [Test,Category("AppealDependent")] // CAR-142 CAR-32
        [Retrying(Times = 3)]
        public void Verify_Switching_Auto_Approved_Flags()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(2))
            {
                AppealSearchPage _appealSearch;
                AppealActionPage _appealAction;
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var appealSequence = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                    "AppealSeqWithAAFlag", "Value");
                var claimSequence = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                    "ClaSeqWithAppealWithAAFlag", "Value").Split('-')[0];
                var claimSub = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                    "ClaSeqWithAppealWithAAFlag", "Value").Split('-')[1];

                var isShiftComplete = false;
                var countOfUndeletedFlagsInDestinationFlagLine = 0;
                var trigLineForSelectedFlag = "";
                var currentLineForSwitchedFlag = "";

                StringFormatter.PrintMessageTitle(
                    "Verifying whether the user has authority to modify auto-reviewed flags");
                automatedBase.CurrentPage.IsRoleAssigned<AppealSearchPage>(new List<string> {automatedBase.EnvironmentManager.Username},
                    RoleEnum.ClaimsManager.GetStringValue()).ShouldBeFalse(
                    $"Is Modifying auto-reviewed flags present for current user<{automatedBase.EnvironmentManager.Username}>");

                StringFormatter.PrintMessageTitle(
                    "Verifying flag switch is allowed to the user inspite of lacking the privilege to modifyauto-reviewed flags");
                _appealSearch = automatedBase.CurrentPage.NavigateToAppealSearch();
                automatedBase.CurrentPage = _appealAction =
                    _appealSearch.SearchByAppealSequenceNavigateToAppealAction(appealSequence);

                var appealLineCount = _appealAction.GetCountOfAppealLine();

                try
                {
                    for (var i = 0; i < appealLineCount; i++)
                    {
                        _appealAction.ClickOnAdjustIcon(i + 2);

                        if (!_appealAction.IsSwitchDeleteIconDisabledByRow())
                        {
                            var flag = _appealAction.GetFlagListOnAdjustAppealLine()[0];
                            var flagLineValueList = _appealAction.GetEditFlagLineValueByFlag(flag);

                            //TL value for the flag for which the switch icon is enabled
                            trigLineForSelectedFlag = flagLineValueList[9];

                            //Trigger Line Number for the current Flag line
                            currentLineForSwitchedFlag = _appealAction.GetLineNo(2, i + 2);

                            //Counting deleted flags in the destination flag line where the switched flag would get moved to.
                            countOfUndeletedFlagsInDestinationFlagLine =
                                ReturnCountOfNotDeletedFlagsByLinNo(trigLineForSelectedFlag);

                            var countOfDeletedFlagLinesBeforeSwitchFlag = _appealAction.GetDeletedFlagLineCount();

                            StringFormatter.PrintMessage("Clicking the switch flag icon");
                            _appealAction.ClickOnSwitchIconByRow();
                            _appealAction.ClickOkCancelOnConfirmationModal(true);
                            if (_appealAction.IsPageErrorPopupModalPresent()) //handle claim lock issue if appears
                            {
                                _appealAction.ClosePageError();
                                _appealAction.ClickOnClaimSequenceAndSwitchWindow();
                                _appealAction.CloseAnyPopupIfExist();
                                _appealAction.ClickOnSwitchIconByRow();
                                _appealAction.ClickOkCancelOnConfirmationModal(true);

                            }

                            isShiftComplete = true;
                            var countOfDeletedFlagLinesAfterSwitchFlag = _appealAction.GetDeletedFlagLineCount();
                            countOfDeletedFlagLinesAfterSwitchFlag.ShouldBeEqual(
                                countOfDeletedFlagLinesBeforeSwitchFlag + 1,
                                "Deleted Flag count should increase by 1 after switching");

                            _appealAction.IsSwitchDeleteIconDisabledByRow(1)
                                .ShouldBeTrue("The switch icon is disabled once the flag is switched");
                            _appealAction.ClickOnAppealLineCancel();
                            _appealSearch.ClickOkCancelOnConfirmationModal(true);
                            break;
                        }

                        _appealAction.ClickOnAppealLineCancel();
                        _appealSearch.ClickOkCancelOnConfirmationModal(true);
                    }

                    _appealAction.ClickOnAdjustAppealLineByLineNo(trigLineForSelectedFlag);
                    _appealAction.GetNotDeletedFlagLineCount(trigLineForSelectedFlag)
                        .ShouldBeEqual(countOfUndeletedFlagsInDestinationFlagLine + 1);

                }

                finally
                {
                    if (isShiftComplete)
                        _appealAction.DeleteAppealLineByLinNo(claimSequence, claimSub, currentLineForSwitchedFlag);
                }

                int ReturnCountOfNotDeletedFlagsByLinNo(string linNo)
                {
                    _appealAction.ClickOnAdjustAppealLineByLineNo(linNo);

                    var countOfNotDeletedFlags = _appealAction.GetNotDeletedFlagLineCount(linNo);
                    _appealAction.ClickOnAppealLineCancel(true, linNo);
                    _appealSearch.ClickOkCancelOnConfirmationModal(true);

                    return countOfNotDeletedFlags;
                }
            }
        }

        [NonParallelizable]
        [Test, Category("OnDemand")] //CAR-278
        public void Verify_Modification_of_Core_Edits_Claims_And_Appeals()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealSearchPage _appealSearch;
                AppealActionPage _appealAction;

                _appealSearch = automatedBase.QuickLaunch.NavigateToAppealSearch();
                _appealSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",AppealQuickSearchTypeEnum.OutstandingAppeals.GetStringValue());
                _appealSearch.GetSideBarPanelSearch.ClickOnFindButton();
                _appealSearch.WaitForWorking();

                _appealAction = _appealSearch.ClickOnAppealSequenceToNavigateAppealActionPageByRow(1);
                _appealSearch = _appealAction.ClickOnSearchIconToNavigateAppealSearchPage();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;

                var claseqwithCoreFlags = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                    "claseqwithCoreFlags", "Value");
                var claseqWithCoreFlagsClaim = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "claseqWithCoreFlagsClaim", "Value");
                bool needsToRefresh = false;

                var coreFlagsCanBeModifiedLabel = ConfigurationSettingsEnum.ModifyCoreFlag.GetStringValue();

                var _newClientSearch = automatedBase.CurrentPage.SwitchTabAndNavigateToQuickLaunchPage().NavigateToClientSearch();

                try
                {
                    StringFormatter.PrintMessageTitle($"Disabling '{coreFlagsCanBeModifiedLabel}'");
                    _newClientSearch.SearchByClientCodeToNavigateToClientProfileViewPage(ClientEnum.SMTST.ToString(),
                        ClientSettingsTabEnum.Configuration.GetStringValue());

                    if (!_newClientSearch.IsRadioButtonOnOffByLabel(coreFlagsCanBeModifiedLabel))
                    {
                        _newClientSearch.GetSideWindow.Save();
                        _newClientSearch.GetSideWindow.ClickOnEditIcon();
                        needsToRefresh = true;
                    }

                    StringFormatter.PrintMessageTitle(
                        $"Verifying users cannot modify appeal on core flags when {coreFlagsCanBeModifiedLabel} is Disabled");

                    automatedBase.CurrentPage.SwitchTab(automatedBase.CurrentPage.GetCurrentWindowHandle());

                    if (needsToRefresh)
                        automatedBase.CurrentPage.RefreshPage(false);

                    _appealAction =
                        _appealSearch.FindByClaimSequenceToNavigateAppealAction(claseqwithCoreFlags, false);
                    _appealAction.WaitForWorking();
                    _appealAction.HandleAutomaticallyOpenedActionPopup(2);

                    _appealAction.ClickOnAdjustAppealLineByLineNo("2");
                    _appealAction.IsSwitchDeleteIconDisabledByRow(1, true).ShouldBeTrue(
                        $"Quick Delete icon for core flag should be disabled when '{coreFlagsCanBeModifiedLabel}' is Disabled");
                    _appealAction.IsRestoreIconPresentDisabledByRow(3, true).ShouldBeTrue(
                        $"Restore icon for core flag should be disabled when '{coreFlagsCanBeModifiedLabel}' is Disabled");
                    _appealAction.ClickOnAppealLineCancel();
                    _appealAction.WaitForWorking();
                    _appealAction.ClickOkCancelOnConfirmationModal(true);


                    StringFormatter.PrintMessageTitle(
                        $"Verifying users cannot modify claims on core flags when {coreFlagsCanBeModifiedLabel} is disabled");
                    var _claimAction = automatedBase.CurrentPage.NavigateToClaimSearch()
                        .SearchByClaimSequenceToNavigateToClaimActionPage(claseqWithCoreFlagsClaim);
                    _claimAction.RemoveLock();
                    _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup(2);

                    _claimAction.IsDeleteIconDisabledByLinenoAndRow(1, 1).ShouldBeTrue(
                        $"Delete icon for core flag should be disabled when '{coreFlagsCanBeModifiedLabel}' is Disabled");
                    _claimAction.IsRestoreIconDisabledByLinenoAndRow(1, 2).ShouldBeTrue(
                        $"Restore icon for core flag should be disabled when '{coreFlagsCanBeModifiedLabel}' is Disabled");
                    _claimAction.IsDeleteRestoreIconRowByRowDisabled(1).ShouldBeTrue(
                        $"Delete icon for all flags on line should be disabled when '{coreFlagsCanBeModifiedLabel}' is Disabled");
                    _claimAction.IsDeleteRestoreAllFlagsIconCssLocator().ShouldBeTrue(
                        $"Delete icon for all flags  should be disabled when '{coreFlagsCanBeModifiedLabel}' is Disabled");

                    StringFormatter.PrintMessageTitle($"Enabling '{coreFlagsCanBeModifiedLabel}'");
                    _claimAction.SwitchTab(_claimAction.GetCurrentWindowHandle());

                    if (!_newClientSearch.IsRadioButtonOnOffByLabel(coreFlagsCanBeModifiedLabel))
                    {
                        _newClientSearch.ClickOnRadioButtonByLabel(coreFlagsCanBeModifiedLabel);
                        _newClientSearch.GetSideWindow.Save();
                        _newClientSearch.GetSideWindow.ClickOnEditIcon();
                    }

                    _claimAction.SwitchTab(_claimAction.GetCurrentWindowHandle());
                    StringFormatter.PrintMessageTitle(
                        $"Verifying users can modify claims on core flags when '{coreFlagsCanBeModifiedLabel}' is Enabled");
                    _claimAction.Refresh();
                    _claimAction.RemoveLock();
                    _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup(2);

                    _claimAction.IsDeleteIconDisabledByLinenoAndRow(1, 1).ShouldBeFalse(
                        $"Delete icon for core flags should not be disabled when {coreFlagsCanBeModifiedLabel} is Enabled");
                    _claimAction.ClickOnEditIconFlagLevelForLineEdit(1, "FRE");
                    _claimAction.IsEditFlagDeleteRestoreButtonDisabled()
                        .ShouldBeFalse(
                            $"Normal Delete icon for core flags should not be disabled when '{coreFlagsCanBeModifiedLabel}' is Enabled");
                    _claimAction.ClickOnCancelLink();
                    _claimAction.ClickOnEditIconFlagLevelForLineEdit(1, "PFOT");

                    _claimAction.IsEditFlagDeleteRestoreButtonDisabled(true)
                        .ShouldBeFalse(
                            $"Normal Restore icon for core flags should not be disabled when '{coreFlagsCanBeModifiedLabel}' is Enabled");

                    _claimAction.ClickOnCancelLink();
                    _claimAction.IsRestoreIconDisabledByLinenoAndRow(1, 2).ShouldBeFalse(
                        $"Restore icon for core flags should not be disabled when {coreFlagsCanBeModifiedLabel} is Enabled");
                    _claimAction.IsDeleteIconRowByRowDisabled(1).ShouldBeFalse(
                        $"Delete icon for all flags on line should be enabled when {coreFlagsCanBeModifiedLabel} is Enabled");
                    _claimAction.IsDeleteAllFlagsPresent(true).ShouldBeTrue(
                        $"Delete icon for all flags on line should be enabled when {coreFlagsCanBeModifiedLabel} is Enabled");


                    StringFormatter.PrintMessageTitle(
                        $"Verifying users can modify appeals on core flags when '{coreFlagsCanBeModifiedLabel}' is Enabled");
                    _appealAction = _claimAction.NavigateToAppealSearch()
                        .FindByClaimSequenceToNavigateAppealAction(claseqwithCoreFlags, false);
                    _appealAction.WaitForWorking();
                    _appealAction.HandleAutomaticallyOpenedActionPopup(2);

                    _appealAction.ClickOnAdjustAppealLineByLineNo("2");
                    _appealAction.IsSwitchDeleteIconDisabledByRow(1, true).ShouldBeFalse(
                        $"Quick Delete icon for core flag should be enabled when '{coreFlagsCanBeModifiedLabel}' is Enabled");
                    _appealAction.ClickOnEditFlagByFlag("UNB");
                    _appealAction.IsLineDeleteRestoreIconDisabled().ShouldBeFalse(
                        $"Normal Delete icon for core flag should be enabled when '{coreFlagsCanBeModifiedLabel}' is Enabled");
                    _claimAction.ClickOnCancelLink();
                    _appealAction.ClickOnEditFlagByFlag("MEX");
                    _appealAction.IsLineDeleteRestoreIconDisabled(true).ShouldBeFalse(
                        $"Normal Restore icon for core flag should be enabled when {coreFlagsCanBeModifiedLabel} is Enabled");
                    _claimAction.ClickOnCancelLink();
                    _appealAction.IsRestoreIconPresentDisabledByRow(3, true).ShouldBeFalse(
                        $"Restore icon for core flag should be enabled when {coreFlagsCanBeModifiedLabel} is Enabled");

                }

                finally
                {
                    if (automatedBase.CurrentPage.GetPageHeader() == _appealAction.GetPageHeader())
                        _appealAction.SwitchTab(_appealAction.GetCurrentWindowHandle());

                    _newClientSearch.ClickOnRadioButtonByLabel(coreFlagsCanBeModifiedLabel, false);
                    _newClientSearch.GetSideWindow.Save();

                    _newClientSearch.GetCommonSql.UpdateRevertCanModifyCoreFlags(ClientEnum.SMTST.ToString());

                    _newClientSearch.SwitchTab(_newClientSearch.GetCurrentWindowHandle(), true);
                }
            }
        }

        [Test] //CAR-853(CAR-823) + CAR-1114(CAR-1069)
        public void Verify_DCI_Appeals_Consultant_Workflow()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealSearchPage _appealSearch;
                AppealActionPage _appealAction;
                AppealManagerPage _appealManager;
                AppealCreatorPage _appealCreator;
                AppealProcessingHistoryPage _appealProcessingHx;

                _appealSearch = automatedBase.QuickLaunch.NavigateToAppealSearch();
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var paramsList = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);

                // claseqList[0] : Claseq Not Requiring State Review
                // claseqList[1] : Claseq Requiring State Review        
                var claseqList = paramsList["claSeqs"].Split(';').ToList();

                var statusNotRequiringStateReview = paramsList["statusNotRequiringStateReview"].Split(';').ToList();
                var statusRequiringStateReview = paramsList["statusRequiringStateReview"].Split(';').ToList();
                var appealSeqList = new List<string>();
                try
                {
                    _appealSearch.DoesClaimRequireStateReview(claseqList[0])
                        .ShouldNotBeEqual(1,
                            string.Format("Claseq {0} should not require State Review", claseqList[0]));
                    _appealSearch.DoesClaimRequireStateReview(claseqList[1])
                        .ShouldBeEqual(1, string.Format("Claseq {0} should require State Review", claseqList[1]));

                    StringFormatter.PrintMessage(
                        "Delete any prior appeals that may have been associated with the claims");
                    _appealManager = automatedBase.CurrentPage.NavigateToAppealManager();
                    foreach (var claseq in claseqList)
                    {
                        _appealManager.SearchByClaimSequence(claseq);
                        if (_appealManager.GetSearchResultCount() == 0 || _appealManager.IsNoDataMessagePresent())
                            continue;
                        automatedBase.CurrentPage = _appealManager = automatedBase.CurrentPage.NavigateToAppealManager();
                        _appealManager.DeleteAppealsAssociatedWithClaim(claseq); //to delete appeal created
                    }

                    StringFormatter.PrintMessageTitle("Creating Dental appeals on the claims");
                    _appealCreator = automatedBase.CurrentPage.NavigateToAppealCreator();
                    foreach (var claseq in claseqList)
                    {
                        _appealCreator.SearchByClaimSequence(claseq);
                        _appealCreator.SelectClaimLine();
                        _appealCreator.CreateAppeal(ProductEnum.DCA.GetStringValue(), "DocID", "");
                    }

                    StringFormatter.PrintMessageTitle(
                        "Verification of Consultant status values in edit form of Appeal Action Page");
                    _appealSearch = _appealCreator.NavigateToAppealSearch();

                    foreach (var claseq in claseqList)
                    {
                        _appealAction = _appealSearch.FindByClaimSequenceToNavigateAppealAction(claseq);
                        appealSeqList.Add(_appealAction.GetAppealSequence());
                        _appealAction.ClickOnEditIcon();

                        var statusCollection = claseq == claseqList[0]
                            ? statusNotRequiringStateReview
                            : statusRequiringStateReview;

                        StringFormatter.PrintMessageTitle(
                            "Verification of saving the consultant status on the appeals and checking whether the values are being saved properly");
                        int i = 0;
                        for (; i < 2; i++)
                        {
                            var statusList =
                                _appealAction.GetSideBarPanelSearch.GetAvailableDropDownList("Status");

                            statusList.ShouldContain(statusCollection[i], string.Format(
                                "'{0}' is present in the status dropdown in edit form of Appeal Action.",
                                statusCollection[i]));

                            StringFormatter.PrintMessage(
                                "Verifying the absence of Consultant Complete status before Consultant Required status is selected");
                            // CAR-1069 : The absence of Consultant Complete status before Consultant Required status is selected is tested in this if-block
                            if (i == 0)
                            {
                                statusList.ShouldNotContain(statusCollection[i + 1], string.Format(
                                    "'{0}' is not present in the status dropdown in edit form of Appeal Action.",
                                    statusCollection[i + 1]));
                            }

                            _appealAction.SelectEditAppealFieldDropDownListByLabel("Status", statusCollection[i]);
                            _appealAction.ClickOnSaveButton();
                            _appealSearch.GetGridViewSection.GetValueInGridByColRow(11)
                                .ShouldBeEqual(statusCollection[i]);
                            _appealSearch.ClickOnAppealSequenceToNavigateAppealActionPageByRow(1);
                            _appealAction.GetPageHeader()
                                .ShouldBeEqual(PageHeaderEnum.AppealAction.GetStringValue(),
                                    string.Format("Appeal opens up in Appeal Action page for appeal with status '{0}'",
                                        statusCollection[i]));

                            _appealAction.GetStatus().ShouldBeEqual(statusCollection[i],
                                "Status is being correctly saved after being edited in Appeal Action edit form");
                            _appealProcessingHx = _appealAction.ClickAppealProcessingHx();
                            _appealProcessingHx.GetAppealAuditGridTableDataValue(1, 6)
                                .ShouldBeEqual(statusCollection[i],
                                    "Appeal Processing History page contains the updated status");
                            _appealAction =
                                _appealProcessingHx.CloseAppealProcessingHistoryPageBackToAppealActionPage();
                            _appealAction.ClickOnEditIcon();
                        }

                        _appealSearch = _appealAction.ClickOnSearchIconToNavigateAppealSearchPage();
                    }

                    StringFormatter.PrintMessageTitle(
                        "Verification of whether the DCA appeals are part of the search result for 'Outstanding Appeals' in Appeal Search Page");
                    foreach (var appealSeq in appealSeqList)
                    {
                        _appealSearch.SelectOutstandingAppeals();
                        _appealSearch.SelectClientSmtst();
                        _appealAction = _appealSearch.SearchByAppealSequenceNavigateToAppealAction(appealSeq);
                        _appealAction.ClickOnSearchIconToNavigateAppealSearchPage();
                        _appealSearch.GetSearchResultByRowCol(1, 3)
                            .ShouldBeEqual(appealSeq, "The appeal sequence is a part of Outstanding Appeals ");
                    }
                }

                finally
                {
                    _appealManager = automatedBase.CurrentPage.NavigateToAppealManager();
                    foreach (var claseq in claseqList)
                    {
                        _appealManager.SearchByClaimSequence(claseq);
                        if (_appealManager.GetSearchResultCount() == 0 || _appealManager.IsNoDataMessagePresent())
                            continue;
                        automatedBase.CurrentPage = _appealManager = automatedBase.CurrentPage.NavigateToAppealManager();
                        _appealManager.DeleteAppealsAssociatedWithClaim(claseq); //to delete appeal created
                    }
                }

            }
        }
        #endregion

    }


}



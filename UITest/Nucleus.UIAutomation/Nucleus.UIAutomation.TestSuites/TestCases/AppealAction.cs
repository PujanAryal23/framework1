using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using Nucleus.Service.Data;
using Nucleus.Service.PageObjects.Appeal;
using Nucleus.Service.PageServices.Appeal;
using Nucleus.Service.PageServices.Claim;
using Nucleus.Service.PageServices.Settings;
using Nucleus.Service.PageServices.Settings.User;
using Nucleus.Service.Support.Common.Constants;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Environment;
using Nucleus.UIAutomation.TestSuites.Base;
using NUnit.Framework;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Core.Navigation;
using UIAutomation.Framework.Utils;
using static System.Console;
using static System.String;

namespace Nucleus.UIAutomation.TestSuites.TestCases
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class AppealAction 
    {
        //#region PRIVATE FIELDSs


        //private AppealSearchPage _appealSearch;
        //private AppealActionPage _appealAction;
        //private NewPopupCodePage _newPopupCode;
        //private AppealProcessingHistoryPage _appealProcessingHx;
        
        //private AppealManagerPage _appealManager;
        //private AppealSummaryPage _appealSummary;
        //private ClientSearchPage _clientSearch;
        //private ClaimActionPage _claimAction;


        //#endregion

        //#region OVERRIDE METHODS
        //protected override void ClassInit()
        //{
        //    try
        //    {
        //        base.ClassInit();
        //        automatedBase.CurrentPage = _appealSearch = QuickLaunch.NavigateToAppealSearch();
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
        //    if (Compare(UserType.CurrentUserType, UserType.HCIADMIN, StringComparison.OrdinalIgnoreCase) != 0)
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

        [Test] //CAR-3266(CAR-3246)
        public void Verify_save_draft_saved_all_fields()
        {

            const string rationale = "Test Rationale";
            const string summary = "Test Summary";
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealSearchPage _appealSearch = automatedBase.QuickLaunch.NavigateToAppealSearch();
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var claimSeq = paramLists["ClaimSequence"];
                var reasonCode = paramLists["ReasonCode"];
                
                _appealSearch.RevertSaveAppealDraft(claimSeq);

                 AppealActionPage _appealAction = _appealSearch.FindByClaimSequenceToNavigateAppealAction(claimSeq);
                 _appealAction.ClickOnDenyIcon();
                 _appealAction.SetReasonCodeList(reasonCode);
                 _appealAction.SetEditAppealLineIfrmeEditorByHeader("Rationale", rationale);
                 _appealAction.SetEditAppealLineIfrmeEditorByHeader("Summary", summary);
                 _appealAction.ClickOnSaveAppealDraftButton();
                 _appealAction.GetReasonCodeInput().ShouldBeEqual(reasonCode, "Reason Code Should retain after saving draft");
                 _appealAction.GetEditAppealLineIframeEditorByHeader("Rationale").ShouldBeEqual(rationale, "Rationale Should retain after saving draft");
                _appealAction.GetEditAppealLineIframeEditorByHeader("Summary").ShouldBeEqual(summary,"Summary Should retain after saving draft");

            }
        }

        [Test] //CAR-2885 (CAR-2953)
        public void Verify_appeals_have_correct_reason_codes()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealSearchPage _appealSearch;
                AppealActionPage _appealAction;

                _appealSearch = automatedBase.QuickLaunch.NavigateToAppealSearch();
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var appealList = paramLists["AppealSequences"].Split(';').ToList();
                var appealResultTypesList = new[] {"D", "P", "A", "N"};

                var productsAndAppeals = new Dictionary<string, string>()
                {
                    ["C"] = appealList[0], // COB
                    ["D"] = appealList[1], // DCA
                    ["U"] = appealList[2], // FCI
                    ["R"] = appealList[3], // FFP
                    ["N"] = appealList[4], // NEG
                    ["F"] = appealList[5], // CV
                };

                try
                {
                    foreach (KeyValuePair<string, string> productAppealPair in productsAndAppeals)
                    {
                        var appealSeqForProduct = productAppealPair.Value;
                        var product = productAppealPair.Key;

                        StringFormatter.PrintMessageTitle($"Verifying the reason codes for product '{product}'");
                        _appealSearch.SelectSMTST();
                        _appealAction =
                            _appealSearch.SearchByAppealSequenceNavigateToAppealAction(appealSeqForProduct);
                        string appealResultAction = "";

                        foreach (var appealResultType in appealResultTypesList)
                        {
                            StringFormatter.PrintMessage(
                                $"Verifying the reason codes for {product} product and appeal result type {appealResultType}");
                            switch (appealResultType)
                            {
                                case "D":
                                    _appealAction.ClickOnDenyIcon();
                                    appealResultAction = "D";
                                    break;
                                case "P":
                                    _appealAction.ClickOnPayIcon();
                                    appealResultAction = "P";
                                    break;
                                case "A":
                                    _appealAction.ClickOnAdjustIcon();
                                    appealResultAction = "A";
                                    break;
                                case "N":
                                    _appealAction.ClickOnNoDocsIcon();
                                    appealResultAction = "D";
                                    break;
                            }

                            if (_appealAction.IsPageErrorPopupModalPresent())
                                _appealAction.ClickOkCancelOnConfirmationModal(true);

                            var listOfReasonCodesFromUi = _appealAction.GetReasonCodeList();
                            var listOfReasonCodesFromDB =
                                _appealAction.GetAppealReasonCodesFromDB(product, appealResultAction);

                            listOfReasonCodesFromUi.ShouldBeEqual(listOfReasonCodesFromDB,
                                "List of reason codes should be correct");
                        }

                        _appealAction.ClickOnCancelSaveDraft();
                        _appealAction.WaitToLoadPageErrorPopupModal();
                        _appealAction.ClickOkCancelOnConfirmationModal(true);
                        _appealAction.ClickOnSearchIconToNavigateAppealSearchPage();
                    }
                }
                finally
                {
                    if (automatedBase.CurrentPage.IsPageErrorPopupModalPresent())
                        automatedBase.CurrentPage.ClickOkCancelOnConfirmationModal(true);
                }
            }
        }

        [Test] //CAR-1798 (CAR-1101)
        public void Verify_copy_all_button_functionality_in_result_reasoncode_rationale_and_summary()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealSearchPage _appealSearch;
                AppealActionPage _appealAction;

                _appealSearch = automatedBase.QuickLaunch.NavigateToAppealSearch();
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var claimSeq = paramLists["ClaimSequence"].Split(';').ToList();
                var claimSeqWithSingleLineAppealed = paramLists["ClaimSeqWithSingleLineAppealed"];
                var reasonCode = paramLists["ReasonCode"].Split(';').ToList();
                var rationale = paramLists["Rationale"].Split(';').ToList();
                var summary = paramLists["Summary"].Split(';').ToList();
                _appealSearch.RevertSaveAppealDraft(claimSeq[0]);
                _appealSearch.RevertSaveAppealDraft(claimSeq[1]);
                _appealAction = _appealSearch.FindByClaimSequenceToNavigateAppealAction(claimSeq[0]);
                _appealAction.ClickOnPayIcon();
                _appealAction.IsCopyAllButtonPresent().ShouldBeTrue("Is Copy All Button Present?");
                _appealAction.SetReasonCodeList(reasonCode[0]);
                _appealAction.SetEditAppealLineIfrmeEditorByHeader("Rationale", rationale[0]);
                _appealAction.SetEditAppealLineIfrmeEditorByHeader("Summary", summary[0]);
                _appealAction.ClickOnCopyAllButton();
                _appealAction.GetAppealLineCount().ShouldBeEqual(_appealAction.GetAppealLineFormCount(),
                    "All Appeal Line form should be opened");
                ValidateCopiedResults(rationale[0], summary[0], reasonCode[0], AppealResult.Pay);
                StringFormatter.PrintMessage("Verify popup when there are already values selected in the other lines");
                _appealAction.SetEditAppealLineIfrmeEditorByHeader("Rationale", rationale[1]);
                _appealAction.ClickOnCopyAllButton();
                _appealAction.IsPageErrorPopupModalPresent().ShouldBeTrue(
                    "If there are already values selected in the other lines, a message should be shown to the user");
                _appealAction.GetPageErrorMessage().ShouldBeEqual(
                    "Result, Reason Code, Rationale text and Summary text will be copied from this appeal line to all other appeal lines. Some values may be overwritten. Do you wish to continue?");
                _appealAction.ClickOkCancelOnConfirmationModal(false);
                _appealAction.GetAllEditAppealLineIframeEditorByHeader("Rationale").Skip(1)
                    .All(x => x.Equals(rationale[0]))
                    .ShouldBeTrue("Changed rationale value must not be copied to other appeal line forms");
                StringFormatter.PrintMessage("Switch Appeal Result and verify copy all");
                _appealAction.ClickOnDenyIcon();
                if (_appealAction.IsPageErrorPopupModalPresent())
                {
                    _appealAction.ClickOkCancelOnConfirmationModal(true);
                }

                _appealAction.IsCopyAllButtonPresent(3).ShouldBeTrue("Is Copy All Button Present?");
                _appealAction.SetReasonCodeList(reasonCode[1]);
                _appealAction.SetEditAppealLineIfrmeEditorByHeader("Rationale", rationale[1]);
                _appealAction.SetEditAppealLineIfrmeEditorByHeader("Summary", summary[1]);
                _appealAction.ClickOnCopyAllButton();
                _appealAction.IsPageErrorPopupModalPresent().ShouldBeTrue(
                    "If there are already values selected in the other lines, a message should be shown to the user");
                _appealAction.GetPageErrorMessage().ShouldBeEqual(
                    "Result, Reason Code, Rationale text and Summary text will be copied from this appeal line to all other appeal lines. Some values may be overwritten. Do you wish to continue?");
                _appealAction.ClickOkCancelOnConfirmationModal(true);
                ValidateCopiedResults(rationale[1], summary[1], reasonCode[1], AppealResult.Deny);
                StringFormatter.PrintMessage("Verify Save Appeal Draft saves Copy All values");
                _appealAction.ClickOnSaveAppealDraftButton();
                _appealSearch = _appealAction.ClickOnSearchIconToNavigateAppealSearchPage();
                _appealAction = _appealSearch.ClickOnAppealSequenceToNavigateAppealActionPageByRow(1);
                _appealAction.GetAppealLineCount().ShouldBeEqual(_appealAction.GetAppealLineFormCount(),
                    "All Appeal Line form should be opened");
                ValidateCopiedResults(rationale[1], summary[1], reasonCode[1], AppealResult.Deny);
                StringFormatter.PrintMessage("Verify Complete Appeal works for Copy All values");
                _appealAction.ClickOnAppealLetter();
                _appealAction.ClickOnApproveIcon();
                _appealAction.IsPageErrorPopupModalPresent()
                    .ShouldBeTrue("Is Ok cancel popup displayed on Complete button click?");
                _appealAction.ClosePageError();
                StringFormatter.PrintMessage("Verify Copy All button is not visible for appeal with single line");
                _appealAction.ClickOnSearchIconToNavigateAppealSearchPage();
                _appealAction =
                    _appealSearch.FindByClaimSequenceToNavigateAppealAction(claimSeqWithSingleLineAppealed);
                _appealAction.ClickOnAdjustIcon();
                _appealAction.IsCopyAllButtonPresent()
                    .ShouldBeFalse("Copy All Button should not be present for appeal with single line");
                StringFormatter.PrintMessage("Verify copy all button for user without pci product authority");
                _appealAction.Logout().LoginAsHciUserHCIUserWithoutPCIProductAuthority().NavigateToAppealSearch();
                _appealAction = _appealSearch.FindByClaimSequenceToNavigateAppealAction(claimSeq[1]);
                _appealAction.ClickOnDenyIcon(3);
                _appealAction.SetReasonCodeList(reasonCode[1]);
                _appealAction.SetEditAppealLineIfrmeEditorByHeader("Rationale", rationale[1]);
                _appealAction.SetEditAppealLineIfrmeEditorByHeader("Summary", summary[1]);
                _appealAction.ClickOnCopyAllButton(3);
                _appealAction.IsAppealLineFormOpened()
                    .ShouldBeFalse("Appeal Line with no product edit authority should not be opened");
                _appealAction.GetAppealLineFormCount().ShouldBeEqual(2, "Other appeal line should be opened.");
                StringFormatter.PrintMessage("Verify values are copied to all opened appeal line form");
                ValidateCopiedResults(rationale[1], summary[1], reasonCode[1], AppealResult.Deny, 1);
                _appealAction.ClickOnSaveAppealDraftButton();
                _appealAction.IsDenyIconSelected()
                    .ShouldBeFalse("Result should not be copied to line with no product authority");

                void ValidateCopiedResults(string rationaleValue, string summaryValue, string reasonCodeValue, AppealResult result, int skipRow = 2)
                {
                    _appealAction.GetAllEditAppealLineIframeEditorByHeader("Rationale", skipRow).All(x => x.Equals(rationaleValue))
                        .ShouldBeTrue("Rationale Value should be copied to all appeal Line form");
                    _appealAction.GetAllEditAppealLineIframeEditorByHeader("Summary", skipRow).All(x => x.Equals(summaryValue))
                        .ShouldBeTrue("Summary Value should be copied to all appeal Line form");
                    _appealAction.GetAllReasonCodeInput().All(x => x.Equals(reasonCodeValue))
                        .ShouldBeTrue("Reason Code should be copied to all appeal Line form");
                    if (result.ToString().Equals("Deny"))
                        _appealAction.IsAllDenyIconSelected(skipRow)
                            .ShouldBeTrue("Deny Result should be copied to all appeal Line form");
                    else if (result.ToString().Equals("Pay"))
                        _appealAction.IsAllPayIconSelected(skipRow)
                            .ShouldBeTrue("Pay Result should be copied to all appeal Line form");
                }
            }
        }

        [Test] //CAR-953 +TE-800
        public void Verify_DCI_values_in_appeal_letter_and_consultant_name_displayed_on_completion_of_review_with_consultant_value_saved()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealSearchPage _appealSearch;
                AppealActionPage _appealAction;
                ClaimActionPage _claimAction;

                _appealSearch = automatedBase.QuickLaunch.NavigateToAppealSearch();
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var claimSeq = paramLists["ClaimSeq"];
                var appealSeq = paramLists["AppealSeq"];
                var footerContentBeforeSavingConsultant = paramLists["FooterContentBeforeSavingConsultant"];
                var dentalDataLabel = paramLists["DentalDataLabel"].Split(',');
                var line1Content = paramLists["Line1Content"];
                var statusValue = paramLists["StatusValue"];
                const string classname = "claim_line";
                const string label = "Status";
                const string footerContentAfterSavingConsultant = "Reviewed By: Test Automation";

                _appealSearch.UpdateAppealConsultantAndStatusInAppeal(appealSeq);
                _appealAction = _appealSearch.FindByClaimSequenceToNavigateAppealAction(claimSeq);
                _appealAction.ClickOnAppealLetter();
                _appealAction.GetAppealLetterFooterText().ShouldNotContain(footerContentAfterSavingConsultant,
                    "The detail regarding the user who reviewed by appeal should not be present");
                _appealAction.GetAppealLetterFooterText()
                    .ShouldBeEqual(footerContentBeforeSavingConsultant, "Verify Footer Message");

                _claimAction = _appealAction.ClickOnClaimSequenceAndSwitchWindow();
                _claimAction.MaximizeWindow();
                var line4TNValue = _claimAction.GetDentalData(classname, dentalDataLabel[0], 4);
                var line4TSValue = _claimAction.GetDentalData(classname, dentalDataLabel[1], 4);
                var line4OCValue = _claimAction.GetDentalData(classname, dentalDataLabel[2], 4);
                var line5TNValue = _claimAction.GetDentalData(classname, dentalDataLabel[0], 5);
                var line5TSValue = _claimAction.GetDentalData(classname, dentalDataLabel[1], 5);
                _appealAction.CloseAnyPopupIfExist();

                StringFormatter.PrintMessage(
                    "Verify TN, TS, OC with no values are not present on the appeal letter claim line");

                _appealAction.GetAppealLetterClaimLineContent(1)
                    .ShouldNotContain(dentalDataLabel[0], "TN should not be present");
                _appealAction.GetAppealLetterClaimLineContent(1).ShouldBeEqual(line1Content,
                    "Line number and code number should be present");

                StringFormatter.PrintMessage(
                    "Verify TN, TS, OC with values are present on the appeal letter claim line");

                _appealAction.GetAppealLetterClaimLineContent(4)
                    .ShouldBeEqual(
                        Format("Line 4, Code D2150, TN {0}, TS {1}, OC {2}", line4TNValue, line4TSValue,
                            line4OCValue), "The TN, TS, OC values should be shown");

                StringFormatter.PrintMessage(
                    "Verify if one of the dental value is not present, the value will not appear on appeal letter claim line");

                _appealAction.GetAppealLetterClaimLineContent(5)
                    .ShouldBeEqual(Format("Line 5, Code D2140, TN {0}, TS {1}", line5TNValue, line5TSValue),
                        "The TN, TS values should be shown, OC value should not be present");
                _appealAction.GetAppealLetterClaimLineContent(5)
                    .ShouldNotContain("OC", "The OC value should not be displayed");

                StringFormatter.PrintMessage(
                    "Verify Reviewed By:Consultant name is displayed below Please let us know if you have any questions regarding this appeal text: ");
                _appealAction.ClickOnEditIcon();
                _appealAction.SelectEditAppealFieldDropDownListByLabel(label, statusValue);
                _appealAction.ClickOnSaveButton();
                _appealSearch.ClickOnFindButton();
                _appealAction.HandleAutomaticallyOpenedActionPopup();
                _appealAction.ClickOnAppealLetter();
                _appealAction.GetAppealLetterFooterText().AssertIsContained(footerContentAfterSavingConsultant,
                    "Detail of the consultant who reviewed the appeal must be present below Please let us know if you have any questions regarding this appeal.");

            }
            //388578, 1461582
        }


        [Test] //CAR-859
        public void Verify_consultant_rationales_and_new_pay_code_for_DCI_product()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealSearchPage _appealSearch;
                AppealActionPage _appealAction;

                _appealSearch = automatedBase.QuickLaunch.NavigateToAppealSearch();
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var claimSequence = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                    "ClaimSequence", "Value"); //"1462250-0";
                var reasonCode = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                    "ReasonCode", "Value");
                _appealAction = _appealSearch.FindByClaimSequenceToNavigateAppealAction(claimSequence);
                var clickActions = new List<Action>
                {
                    () => { _appealAction.ClickOnPayIcon(); },
                    () => { _appealAction.ClickOnAdjustIcon(); },
                    () => { _appealAction.ClickOnNoDocsIcon(); },
                    () => { _appealAction.ClickOnDenyIcon(); }
                };
                var expectedConsultantRationales = _appealAction.GetExpectedConsultantRationalesList();
                var expectedId = expectedConsultantRationales.Select(x => x[0].Replace("  ", " ")).ToList();
                var expectedRationale = expectedConsultantRationales.Select(x => x[1].Replace("  ", " ")).ToList();
                Random number = new Random();
                foreach (var clickAction in clickActions)
                {
                    var index1 = number.Next(0, expectedConsultantRationales.Count - 1);
                    var index2 = number.Next(0, expectedConsultantRationales.Count - 1);


                    clickAction();
                    if (_appealAction.IsPageErrorPopupModalPresent())
                    {
                        _appealAction.ClickOkCancelOnConfirmationModal(true);
                    }

                    _appealAction.GetConsultantRationalesList().ShouldCollectionBeEqual(expectedId,
                        "Consultant Rationale List should be Equal");
                    _appealAction.SetConsultantRationales(expectedId[index1]);
                    _appealAction.GetEditAppealLineIframeEditorByHeader("Rationale").ShouldBeEqual(
                        expectedRationale[index1],
                        "Rationale should auto populate when consultant rationale is selected");
                    _appealAction.SetConsultantRationales(expectedId[index2]);
                    _appealAction.GetEditAppealLineIframeEditorByHeader("Rationale").ShouldBeEqual(
                        expectedRationale[index2],
                        "Rationale should update when other consultant rationale is selected");
                    if (_appealAction.IsDenyIconSelected())
                    {
                        _appealAction.IsInputFieldPresentByLabel("New Pay Code")
                            .ShouldBeTrue("New Pay Code should display when the DENY result is selected");
                    }
                    else
                    {
                        _appealAction.IsInputFieldPresentByLabel("New Pay Code")
                            .ShouldBeFalse("Is New Pay Code displayed?");

                    }
                }

                _appealAction.SetInputFieldByLabel("New Pay Code", "Test12", true);
                _appealAction.GetInputFieldByLabel("New Pay Code").Length.ShouldBeEqual(5,
                    "New Pay Code field should allow alphanumeric values upto 5 digits");
                _appealAction.GetInputFieldByLabel("New Pay Code").ShouldBeEqual("Test1".ToUpper(),
                    "Value entered in New Pay Code should be auto-capitalized");
                _appealAction.GetEditAppealLineIframeEditorByHeader("Summary").ShouldBeEqual(
                    "Documentation does support reporting of TEST1.",
                    "Rationale should auto populate when consultant rationale is selected");

                StringFormatter.PrintMessage("Verify Consultant Rationales and New Pay Code fields are not mandatory");
                _appealAction.SetInputFieldByLabel("New Pay Code", "", true);
                _appealAction.SelectEditAppealFieldDropDownListByLabel("Reason Code", reasonCode);
                _appealAction.SetConsultantRationales("");
                _appealAction.SetEditAppealLineIfrmeEditorByHeader("Rationale", "TEST");
                _appealAction.ClickOnAppealLetter();
                _appealAction.ClickOnApproveIcon();
                //this will verify that pay code does not require to complete the appeal
                _appealAction.ClickOkCancelOnConfirmationModal(false);
            }
        }

        [Test]//US58183
        public void Verify_page_should_redirect_to_appeal_search_when_completes_the_appeal_by_entering_appeal_action_via_direct_url_for_empty_worklist()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealSearchPage _appealSearch;
                AppealActionPage _appealAction;
                AppealManagerPage _appealManager;

                _appealSearch = automatedBase.QuickLaunch.NavigateToAppealSearch();
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var appealSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                    "AppealSequence", "Value");
                try
                {
                    automatedBase.CurrentPage =
                        _appealSearch =
                            _appealSearch.Logout().LoginAsHciAdminUser4().NavigateToAppealSearch();
                    UpdateStatus("0", appealSeq);
                    var url = PageUrlEnum.AppealAction.GetStringValue() + "/" + appealSeq + "/appeal_action";
                    automatedBase.CurrentPage = _appealAction = _appealSearch.SwitchToNavigateAppealActionViaUrl(url);
                    _appealAction.ClickOnAppealLetter();
                    _appealAction.ClickOnApproveIcon();
                    _appealAction.ClickOkCancelOnConfirmationModal(true);
                    _appealAction.WaitForWorking();
                    automatedBase.CurrentPage.GetPageHeader().ShouldBeEqual("Appeal Search", "Page Should Redirect to Appeal Search");
                    automatedBase.CurrentPage.PageUrl.ShouldBeEqual(
                        automatedBase.EnvironmentManager.ApplicationUrl + PageUrlEnum.AppealSearch.GetStringValue(),
                        "Page Url Should be Appeal Search");
                    automatedBase.CurrentPage = _appealSearch;
                }
                finally
                {

                }

                void UpdateStatus(string claSeq = "0", string appealSequence = "0", string status = "New")
                {
                    automatedBase.CurrentPage = _appealManager = _appealSearch.NavigateToAppealManager();
                    _appealManager.SelectDropDownListbyInputLabel("Quick Search", "All Appeals");
                    _appealManager.SelectDropDownListbyInputLabel("Client", ClientEnum.SMTST.ToString());
                    if (claSeq != "0")
                        _appealManager.SetInputFieldByInputLabel("Claim Sequence", claSeq);
                    else
                        _appealManager.SetInputFieldByInputLabel("Appeal Sequence", appealSequence);
                    _appealManager.ClickOnFindButton();
                    _appealManager.WaitForWorking();
                    _appealManager.ClickOnEditIcon();
                    _appealManager.SelectDropDownListbyInputLabel("Status", status);
                    _appealManager.SetNote("UINOTE");
                    _appealManager.ClickOnSaveButton();
                    _appealManager.WaitForWorking();
                    automatedBase.CurrentPage = _appealSearch = _appealManager.NavigateToAppealSearch();
                }
            }
        }

        [Test]//US58183
        public void Verify_page_should_redirect_to_appeal_action_when_completes_the_appeal_by_entering_appeal_action_via_direct_url_for_having_worklistact()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealSearchPage _appealSearch;
                AppealActionPage _appealAction;

                _appealSearch = automatedBase.QuickLaunch.NavigateToAppealSearch();
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var appealSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                    "AppealSequence", "Value");
                var claimSequence = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                    "ClaimSequence", "Value");
                try
                {
                    _appealSearch.UpdateAppealStatusToNew(claimSequence);
                    _appealSearch.RefreshPage(); //to reset the search result
                    var url = PageUrlEnum.AppealAction.GetStringValue() + "/" + appealSeq + "/appeal_action";
                    automatedBase.CurrentPage = _appealAction = _appealSearch.SwitchToNavigateAppealActionViaUrl(url);
                    _appealAction.ClickOnAppealLetter();
                    _appealAction.ClickOnApproveIcon();
                    _appealAction.ClickOkCancelOnConfirmationModal(true);
                    _appealAction.WaitForWorking();
                    _appealAction.HandleAutomaticallyOpenedActionPopup();
                    _appealAction.GetAppealSequence()
                        .ShouldNotBeEqual(appealSeq, "Page Should redirect to next appeal sequence");
                    automatedBase.CurrentPage.GetPageHeader().ShouldBeEqual("Appeal Action", "Page Should Redirect to Appeal Action");
                    automatedBase.CurrentPage.PageUrl.ShouldNotContain(appealSeq,
                        "Page Url Should be Appeal Action and does not contains previous appeal sequence");
                    automatedBase.CurrentPage = _appealSearch = _appealAction.ClickOnSearchIconToNavigateAppealSearchPage();
                }
                finally
                {


                }
            }
        }

       [Retry(3)]
        [Test]//US53198 + CAR-2650(CAR-2622)
        public void Verify_that_adjust_edit_line_flag_details()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(1))
            {
                AppealActionPage _appealAction;
                AppealSearchPage _appealSearch = automatedBase.QuickLaunch.NavigateToAppealSearch();
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var claimSequence = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                    "ClaimSequence", "Value");
                var ReasonCodeList = automatedBase.DataHelper.GetMappingData(FullyQualifiedClassName, "delete_restore_reason_code")
                    .Values.ToList();
                var triggerClaimSequence = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "TriggeredClaimSequence", "Value");
                var flag = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName, "Flag",
                    "Value");
                var flagSugCodeEnabled = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                    "FlagWithSugCodeEnabled", "Value");
                var deletedFlag = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                    "DeletedFlag", "Value");
                var source = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName, "Source",
                    "Value");
                var trigCode = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                    "TrigCode", "Value");
                var trigDescription = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                    "TrigDescription", "Value");
                var triggerLine = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                    "TriggerLine", "Value");
                var modifier = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                    "Modifier", "Value");
                var sugUnitsExceed = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                    "SugUnitExceed", "Value");
                var SugPaid = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName, "SugPaid",
                    "Value");
                var fciFlag = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName, "FCIFlag",
                    "Value");
                var expectedModifiedBy = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                    "ModifiedBy", "Value");
                var rnd = new Random();
                _appealSearch.SelectAllAppeals();
                _appealSearch.SelectClientSmtst();
                automatedBase.CurrentPage =
                    _appealAction =
                        _appealSearch.SearchByClaimSequenceToGoAppealAction(claimSequence);
                var flagValue = _appealAction.AncillarySettingValuesForFOTandFREfromDB(flag);
                _appealAction.HandleAutomaticallyOpenedActionPopup();
                _appealAction.ClickOnAdjustIcon();
                _appealAction.IsEditAppealLineSectionPresent().ShouldBeTrue("Adjust Appeal Line displayed");

                StringFormatter.PrintMessageTitle("Verify Flag Details");
                var flagLineValueList = _appealAction.GetEditFlagLineValueByFlag(flag);
                VerifyThatDateIsInCorrectFormat(flagLineValueList[3], "Trigged Date");
                flagLineValueList[4].ShouldBeEqual(flag, "Flag should present and equal");
                flagLineValueList[5].ShouldBeEqual(source, "Source Value should present and equal");
                flagLineValueList[6].ShouldBeEqual(trigCode, "Trig Code should present and equal");
                flagLineValueList[7].ShouldBeEqual(trigDescription,
                    "Trig Code Short Description should be present and equal");
                flagLineValueList[8]
                    .ShouldBeEqual(triggerClaimSequence, "Trigger Claim Should be present and equal");
                flagLineValueList[9].ShouldBeEqual(triggerLine, "Trigger Line number should be present and equal");
                flagLineValueList[10].ShouldBeEqual(modifier, "Modifier should be present and equal");
                Convert.ToInt32(flagLineValueList[11])
                    .ShouldBeGreater(0, "Trigger Units should be present and Greater than zero");

                StringFormatter.PrintMessageTitle("Verify Appeal/Claim Line Flags with audit record");
                var newSugPaid = rnd.Next(1, Convert.ToInt32(SugPaid));

                _appealAction.ClickOnEditFlagByFlag(flag);
                var temp = Int32.Parse(_appealAction.GetInputFieldByLabel("Sug Paid").Replace('$', ' ').Trim()
                    .Split('.')[0]);
                newSugPaid = newSugPaid == temp
                    ? (newSugPaid + 1)
                    : newSugPaid;
                _appealAction.SetInputFieldByLabel("Sug Paid",
                    newSugPaid.ToString());
                _appealAction.SetReasonCodeList(ReasonCodeList[2]);
                _appealAction.ClickOnSaveButton();

                ClaimActionPage newClaimAction = _appealAction.ClickOnClaimSequenceAndSwitchWindow();
                var flagList = newClaimAction.GetFlagListWithDeletedFlagForLine(1).ToList();

                newClaimAction.ClickOnClaimFlagAuditHistoryIcon();
                var modifiedDate = newClaimAction.GetModifiedDateTextInAuditTrial().Split(' ')[0];
                var modifiedBy = newClaimAction.GetModifiedByInAuditTrial();
                var action = newClaimAction.GetActionTextInAuditTrial();
                var type = newClaimAction.GetActionTypeInAuditTrial();
                var code = newClaimAction.GetCodeInAuditTrial();

                newClaimAction.ClickSystemDeletedFlagIcon();
                var flagWithSystemDeletedList = newClaimAction.GetFlagListWithDeletedFlagForLine(1).ToList();
                newClaimAction.ClickOnDollarIcon();
                var allowed = 0.0;
                switch (flagValue)
                {
                    case "Billed":
                        allowed = newClaimAction.GetBilledValueByLine(1);
                        break;
                    case "Adj Paid":
                        allowed = newClaimAction.GetAdjPaidValueByLine(1);
                        break;
                    default:
                        allowed = newClaimAction.GetAllowedValueByLine(1);
                        break;
                }

                var totalSugUnits = newClaimAction.GetUnitValueOnFlag(1);
                var adjSugPaid = newClaimAction.GetAdjPaidValueByLine(1);
                newClaimAction.DeleteClaimAuditRecordExceptAdd(claimSequence);
                _appealAction.CloseAnyPopupIfExist();

                VerifyThatDateIsInCorrectFormat(modifiedDate, "Audit Date Should be in proper format");
                //modifiedDate.ShouldBeEqual(DateTime.Now.Date.ToString("MM/dd/yyyy"), "Modified Date");
                modifiedBy.ShouldBeEqual(expectedModifiedBy, "Modifiled By should be equal");
                action.ShouldBeEqual("Suggested Paid", "Action should be equal");
                type.ShouldBeEqual("Appeal", "Action Type should be equal");
                code.ShouldBeEqual("ADD - Added edit after appeal", "Reason Code should be equal");

                var allFlagList = _appealAction.GetFlagListOnAdjustAppealLine();

                allFlagList.ShouldCollectionBeEqual(flagList,
                    "Flag Should be ordered based on claim action and equal");
                allFlagList.Count.ShouldBeEqual(flagWithSystemDeletedList.Count - 1,
                    "System Deleted Flag Should not display in Appeal Action");
                _appealAction.IsDeletedFlagPresentByFlag(deletedFlag)
                    .ShouldBeTrue("Deleted Flag should be present and deleted with strike line");



                StringFormatter.PrintMessageTitle("Verify Modify flag Details");

                for (var i = 1; i <= _appealAction.GetEditFlagsRowCount(); i++)
                {
                    StringFormatter.PrintMessageTitle($"Verification of Icon in row={i}");
                    _appealAction.IsEditIconPresentByRow(i).ShouldBeTrue("Edit Icon should be present and enabled");

                    if (_appealAction.IsDeletedFlagPresentByRow(i))
                        _appealAction.IsRestoreIconPresentDisabledByRow(i).ShouldBeTrue("Restore Icon present?");
                    else
                        _appealAction.IsDeleteIconPresentByRow(i).ShouldBeTrue("Deleted Icon present?");

                    _appealAction.IsSwitchIconPresentByRow(i).ShouldBeTrue("Switch Icon present?");

                }

                _appealAction.ClickOnEditFlagByFlag(flag);
                _appealAction.IsEditFlagDivPresent().ShouldBeTrue("Edit Flag displayed?");
                var prevSugUnits = _appealAction.GetInputFieldByLabel("Sug Units");

                _appealAction.SetInputFieldByLabel("Sug Units", sugUnitsExceed);
                _appealAction.WaitToLoadPageErrorPopupModal();
                _appealAction.IsPageErrorPopupModalPresent()
                    .ShouldBeTrue("Sug Units Validation for greater value than billed units");
                _appealAction.GetPageErrorMessage()
                    .ShouldBeEqual("Suggested Units cannot be greater than the units billed on the line.");
                _appealAction.ClosePageError();

                _appealAction.SetInputFieldByLabel("Sug Units", "");
                _appealAction.SelectEditAppealFieldDropDownListByLabel("Reason Code", ReasonCodeList[1]);
                _appealAction.ClickOnSaveButton();
                _appealAction.WaitToLoadPageErrorPopupModal();
                _appealAction.IsPageErrorPopupModalPresent().ShouldBeTrue("Sug Paid Empty Validation");
                _appealAction.GetPageErrorMessage()
                    .ShouldBeEqual("Suggested Unit is required before the record can be saved.",
                        "Sug Paid empty validation message");
                _appealAction.ClosePageError();

                _appealAction.SetInputFieldByLabel("Sug Units", prevSugUnits);

                var modifiedUnits = rnd.Next(86, 170);
                _appealAction.SetInputFieldByLabel("Sug Units", modifiedUnits.ToString());
                _appealAction.ClickOnLabel("Sug Units");
                Double.Parse(_appealAction.GetInputFieldByLabel("Sug Paid").Replace('$', ' ').Trim())
                    .ShouldBeEqual(
                        Math.Round(allowed / totalSugUnits * modifiedUnits, 2, MidpointRounding.AwayFromZero),
                        "Sug Paid calculation is correct after modification of Sug Units instantly in input field");

                var sugPaid = Convert.ToDouble(adjSugPaid.ToString().Replace('$', ' ').Trim()) + 1;

                _appealAction.SetInputFieldByLabel("Sug Paid", sugPaid.ToString());
                _appealAction.ClickOnSaveButton();
                _appealAction.WaitToLoadPageErrorPopupModal();
                _appealAction.IsPageErrorPopupModalPresent()
                    .ShouldBeTrue(
                        "Sug Paid validation for exceed line Adjusted Paid");
                _appealAction.GetPageErrorMessage()
                    .ShouldBeEqual(
                        Format(
                            "Suggested Paid amount of ${0} cannot exceed the line Adjusted Paid amount of ${1}.",
                            sugPaid.ToString("F"), adjSugPaid.ToString("F")),
                        "Proper validation message for exceed line Adjusted Paid");
                _appealAction.ClosePageError();

                ValidateReasonCode(ReasonCodeList);
                _appealAction.ClickOnCancelLinkEditFlag();
                _appealAction.ClickOnEditFlagByFlag(flag);
                var preSugUnits = newClaimAction.GetInternalAndClientSugUnitsByClaseqAndFlag(claimSequence, flag);
                var sugUnits = rnd.Next(2, Convert.ToInt32(SugPaid));
                while (preSugUnits[0] == sugUnits || preSugUnits[1] == sugUnits)
                {
                    sugUnits = rnd.Next(2, Convert.ToInt32(SugPaid));
                }

                _appealAction.SetInputFieldByLabel("Sug Units", sugUnits.ToString());
                _appealAction.SelectEditAppealFieldDropDownListByLabel("Reason Code", ReasonCodeList[1]);
                _appealAction.ClickOnSaveButton();
                var newSugUnits = newClaimAction.GetInternalAndClientSugUnitsByClaseqAndFlag(claimSequence, flag);
                newSugUnits[0].ShouldNotBeEqual(preSugUnits[0], "HCISUGUNITS should be changed");
                newSugUnits[1].ShouldNotBeEqual(preSugUnits[1], "CLISUGUNITS should be changed");
                newSugUnits[0].ShouldBeEqual(newSugUnits[1], "HCISUGUNITS and CLISUGUNITS should be same");
                newClaimAction = _appealAction.ClickOnClaimSequenceAndSwitchWindow();
                var claimFlagUnit = newClaimAction.GetUnitByFlag(flag);
                _appealAction.CloseAnyPopupIfExist();
                claimFlagUnit.ShouldBeEqual(sugUnits.ToString(),
                    "Sug Unit on Flag should update to Claim Action as well");
                _appealAction.ClickOnEditFlagByFlag(flag);
                _appealAction.ClickDeleteIcononEditFlag();
                StringFormatter.PrintMessageTitle(
                    "Sug Units, Sug Paid and Sug Code should be disabled after clicking line delete icon");
                _appealAction.IsInputFieldDisabledByLabel("Sug Units").ShouldBeTrue("Sug Units Should disabled");
                _appealAction.IsInputFieldDisabledByLabel("Sug Paid").ShouldBeTrue("Sug Paid Should disabled");
                _appealAction.ClickOnCancelLinkEditFlag();

                _appealAction.ClickOnEditFlagByFlag(flagSugCodeEnabled);
                _appealAction.ClickDeleteIcononEditFlag();
                _appealAction.IsInputFieldDisabledByLabel("Sug Code").ShouldBeTrue("Sug Code Should disabled");
                _appealAction.ClickOnCancelLinkEditFlag();

                StringFormatter.PrintMessageTitle(
                    "Verification of allow sug paid greader than adj paid for FCI flag with allow sug paid exceed than adj paid");
                _appealAction.ClickOnEditFlagByFlag(fciFlag);
                //_newAppealAction.SetInputFieldByLabel("Sug Paid", sugPaid.ToString());
                _appealAction.ClickOnSaveButton();
                _appealAction.WaitToLoadPageErrorPopupModal();
                _appealAction.IsPageErrorPopupModalPresent()
                    .ShouldBeTrue(
                        "Reason Code should select when trying to modify a flag");
                _appealAction.GetPageErrorMessage()
                    .ShouldBeEqual(
                        "Reason code or note is required before the record can be saved.",
                        "Validation for empty reason code when trying to modify a flag");
                _appealAction.ClosePageError();
                _appealAction.SetReasonCodeList(ReasonCodeList[1]);

                _appealAction.ClickOnSaveButton();
                _appealAction.IsPageErrorPopupModalPresent()
                    .ShouldBeFalse(
                        "FCI with allow_sugpaid_exceed_adjpaid allow update sug paid with greater than adjpaid");
                _appealAction.IsEditFlagDivPresent().ShouldBeFalse("Edit flag div should close when flag updated");


                void VerifyThatDateIsInCorrectFormat(string date, string message, bool singleDigit = false)
                {
                    if (singleDigit)
                        Regex.IsMatch(date, @"^([1-9]|1[0-2])\/([1-9]|1\d|2\d|3[01])\/(19|20)\d{2}$").ShouldBeTrue(message + " '" + date + "' is in format M/D/YYYY");
                    else
                        Regex.IsMatch(date, @"^(0[1-9]|1[0-2])\/(0[1-9]|1\d|2\d|3[01])\/(19|20)\d{2}$").ShouldBeTrue(message + " '" + date + "' is in format MM/DD/YYYY");

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

        [Test, Category("Working")] //US50605
        //ccannot move further due to result icon is not disabled
        public void Verify_that_the_lock_icon_with_appeal_in_view_state_when_appeal_is_locked_by_different_user()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealSearchPage _appealSearch;
                AppealSummaryPage _appealSummary;

                _appealSearch = automatedBase.QuickLaunch.NavigateToAppealSearch();
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var claimSeq =
                    "388315-0"; // automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,"ClaimSequence", "Value");
                const string lockTooltip = "This appeal is currently locked for editing by uiautomation_client client";
                try
                {
                    automatedBase.CurrentPage =
                        _appealSearch =
                            _appealSearch.Logout().LoginAsClientUser().NavigateToAppealSearch();
                    _appealSearch.SelectAllAppeals();
                    _appealSearch.SetClaimSequence(claimSeq);
                    _appealSearch.ClickOnFindButton();
                    _appealSearch.WaitForWorkingAjaxMessage();

                    var newAppealSearchUrl = automatedBase.CurrentPage.CurrentPageUrl;
                    automatedBase.CurrentPage = _appealSummary =
                        _appealSearch.ClickOnAppealSequenceByStatusOrAppealToGoAppealSummaryPage();
                    var appealSummaryUrl = automatedBase.CurrentPage.CurrentPageUrl;
                    AppealActionPage newAppealAction =
                        _appealSummary.SwitchTabAndOpenAppealActionByUrl(newAppealSearchUrl + '/' +
                                                                         appealSummaryUrl.Split('/')[8] +
                                                                         "/appeal_action");
                    automatedBase.CurrentPage = newAppealAction;
                    newAppealAction.WaitForWorking();
                    newAppealAction.IsInputFieldDisabledByLabel("Reason")
                        .ShouldBeTrue("Reason Code Should disabled when Appeal is Locked");
                    newAppealAction.IsIframeEditorDisabledByLabel("Rationale")
                        .ShouldBeTrue("Reason Code Should disabled when Appeal is Locked");
                    newAppealAction.IsIframeEditorDisabledByLabel("Summary")
                        .ShouldBeTrue("Reason Code Should disabled when Appeal is Locked");
                    newAppealAction.IsTextAreaDisabledByLabel("Note")
                        .ShouldBeTrue("Reason Code Should disabled when Appeal is Locked");
                    newAppealAction.IsEditIconDisabled()
                        .ShouldBeTrue("Reason Code Should disabled when Appeal is Locked");
                    newAppealAction.IsAppealNoteIconDisabled()
                        .ShouldBeTrue("Reason Code Should disabled when Appeal is Locked");
                    newAppealAction.IsAppealLetterIconDisabled()
                        .ShouldBeTrue("Reason Code Should disabled when Appeal is Locked");
                    newAppealAction.IsAppealDraftIconDisabled()
                        .ShouldBeTrue("Reason Code Should disabled when Appeal is Locked");
                    newAppealAction.IsVisibleToClientIconDisabled()
                        .ShouldBeTrue("Reason Code Should disabled when Appeal is Locked");
                    newAppealAction.IsAppealDocumentIconDisabled()
                        .ShouldBeTrue("Appeal Document Should disabled when Appeal is Locked");
                    newAppealAction.IsAppealLock().ShouldBeTrue("Appeal Should Locked");
                    newAppealAction.GetAppealLockToolTip()
                        .ShouldBeEqual(lockTooltip,
                            "Tooltip of Lock Icon");

                    automatedBase.CurrentPage = _appealSummary = newAppealAction.SwitchOriginalAppealSummaryCotivitiUserTab();
                    automatedBase.CurrentPage =
                        _appealSearch =
                            _appealSearch.Logout().LoginAsHciAdminUser().NavigateToAppealSearch();

                }
                finally
                {
                    automatedBase.CurrentPage.SwitchOriginalAppealSummaryCotivitiUserTab();

                }
            }
        }

        [Test]//US48460
        [Retrying(Times = 3)]
        public void Verify_client_session_should_update_when_url_changes_to_access_different_appeals()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealSearchPage _appealSearch;
                AppealActionPage _appealAction;

                _appealSearch = automatedBase.QuickLaunch.NavigateToAppealSearch();
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var claimSeqSMTST = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName, "ClaimSequenceSMTST", "Value");
                var claimSeqRPE = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName, "ClaimSequenceRPE", "Value");
                var appealSeqSMTST = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName, "AppealSequenceSMTST", "Value");
                var appealSeqRPE = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName, "AppealSequenceRPE", "Value");
                var category = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName, "Category", "Value");
                var procCode = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName, "ProcCode", "Value");
                _appealSearch.UpdateAppealStatusToIncomplete(appealSeqRPE);
                _appealSearch.SelectAllAppeals();
                _appealSearch.SelectClientSmtst();

                automatedBase.CurrentPage =
                    _appealAction = _appealSearch.SearchByClaimSequenceToGoAppealAction(claimSeqSMTST.Split('-')[0]);
                _appealAction.SwitchClientAndNavigateAppealAction(ClientEnum.RPE.ToString(), appealSeqSMTST, appealSeqRPE);
                _appealAction.GetAppealSequence().ShouldBeEqual(appealSeqRPE, "Appeal Sequence Should Changed");
                _appealAction.GetAppealInformationByLabel("Claim Sequence")
                    .ShouldBeEqual(claimSeqRPE, "Claim Sequence Should Change");
                _appealAction.GetAppealInformationByLabel("Category").ShouldBeEqual(category, "Category Should equal");
                _appealAction.GetProcCode().ShouldBeEqual(procCode, "Proc Code Should Equal");
                _appealAction.IsDefaultTestClientForEmberPage(ClientEnum.RPE);
            }
        }

        //[Test] //US48450
        public void Verify_proper_validation_when_trying_to_complete_appeal_with_primary_reviewer_should_updated_without_deleting_core_flag_after_appeal_is_completed()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealSearchPage _appealSearch;
                AppealActionPage _appealAction;
                AppealSummaryPage _appealSummary;

                _appealSearch = automatedBase.QuickLaunch.NavigateToAppealSearch();
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var claimSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                    "ClaimSequenceWithAction", "Value");
                var claimSeqWithoutAction = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "ClaimSequenceWithOutAction", "Value");
                var denyReasonCode = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                    "DenyReasonCode", "Value");
                var payReasonCode = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                    "PayReasonCode", "Value");
                const string rationale = "Test Rationale";
                AppealProcessingHistoryPage appealProcessingHistory = null;
                _appealSearch.SelectAllAppeals();
                _appealSearch.SelectClientSmtst();
                try
                {
                    automatedBase.CurrentPage = _appealAction =
                        _appealSearch.SearchByClaimSequenceToGoAppealAction(claimSeqWithoutAction);
                    _appealAction.IsCompleteAppealIconPresent()
                        .ShouldBeFalse("Complete Appeal or Approve Icon Should displayed");

                    _appealAction.ClickOnAppealLetter();
                    _appealAction.IsCompleteAppealIconPresent()
                        .ShouldBeTrue("Complete Appeal or Approve Icon Should displayed");

                    ValidatePopupMessageWhenAppealisApprovedForDifferentCondition(
                        "Validation for when reason code,rationale and Result not selected", "", "");
                    _appealAction.ClickOnDenyIcon();
                    ValidatePopupMessageWhenAppealisApprovedForDifferentCondition(
                        "Validation for when reason code not selected", "", rationale);
                    ValidatePopupMessageWhenAppealisApprovedForDifferentCondition("Validation for for empty rationale",
                        denyReasonCode, "");
                    _appealAction.ClickOnAppealLetter();
                    automatedBase.CurrentPage = _appealSearch = _appealAction.ClickOnSearchIconToNavigateAppealSearchPage();
                    _appealSearch.SelectAllAppeals();
                    _appealSearch.SelectClientSmtst();
                    automatedBase.CurrentPage = _appealAction = _appealSearch.SearchByClaimSequenceToGoAppealAction(claimSeq);
                    _appealAction.ClickOnEditIcon();
                    _appealAction.SelectEditAppealFieldDropDownListByLabel("Primary Reviewer", "Ganesh Shrestha");
                    _appealAction.ClickOnSaveButton();

                    _appealAction.ClickOnAppealLetter();
                    _appealAction.ClickOnApproveIcon();
                    _appealAction.IsPageErrorPopupModalPresent()
                        .ShouldBeTrue("Confirmation popup should present in order to complete appeal");
                    _appealAction.GetPageErrorMessage()
                        .ShouldBeEqual("The appeal will be completed. Do you wish to continue?");
                    _appealAction.ClickOkCancelOnConfirmationModal(false);
                    _appealAction.ClickOnApproveIcon();
                    _appealAction.ClickOkCancelOnConfirmationModal(true);
                    automatedBase.CurrentPage.PageUrl.ShouldBeEqual(_appealSearch.PageUrl, "Page Should Redirect to Appeal Search");
                    automatedBase.CurrentPage = _appealSearch;
                    _appealSearch.GetSearchResultByRowCol(1, 11)
                        .ShouldBeEqual("Complete", "Status Should display Complete");
                    _appealSummary = _appealSearch.SearchByClaimSequenceInCotivitiUser(claimSeq);
                    _appealSummary.ClickOnEditIcon();
                    _appealSummary.SelectEditAppealFieldDropDownListByLabel("Status", "New");
                    _appealSummary.ClickOnSaveButtonOnEditAppeal();
                    automatedBase.CurrentPage = _appealAction;
                    _appealAction.GetAppealInformationByLabel("Primary Reviewer")
                        .ShouldBeEqual("Test Automation", "Primary Review Should Updated");
                    ClaimActionPage newClaimAction = _appealAction.ClickOnClaimSequenceAndSwitchWindow();
                    newClaimAction.IsFlagDeletedForLineEdit("1", "ADD")
                        .ShouldBeFalse("Core Flag Should not deleted for pay");
                    newClaimAction.IsFlagDeletedForLineEdit("1", "FRE")
                        .ShouldBeTrue("Clinically Validated Flag Should  deleted for pay");
                    newClaimAction.IsFlagDeletedForLineEdit("4", "FRE")
                        .ShouldBeFalse("Clinically Validated Flag Should not deleted for deny");

                    newClaimAction.ClickOnFlagLineByLineNoWithFlag(1, "FRE");
                    VerifyDateStampIsInCorrectFormat(newClaimAction.GetFlagAuditDetailByRowCol(1, 1),
                        "Modified Date is in correct ");
                    VerifyThatNameIsInCorrectFormat(newClaimAction.GetFlagAuditDetailByRowCol(1, 2), "Modified By");
                    newClaimAction.GetFlagAuditDetailByRowCol(2, 1).ShouldBeEqual("Delete", "Action should delete");
                    newClaimAction.GetFlagAuditDetailByRowCol(2, 2).ShouldBeEqual("Appeal", "Type should Appeal");
                    newClaimAction.IsFlagDeletedInFlagAuditDetails()
                        .ShouldBeTrue("Is Clinically Deleted Flag should underline strike in Flag Audit Details");

                    _appealAction.CloseAnyPopupIfExist();


                    _appealAction.IsClinicallyDeletedFlagPresentByLineNo("1", "ADD")
                        .ShouldBeTrue("Core Flag Should not deleted for pay");
                    _appealAction.IsClinicallyDeletedFlagPresentByLineNo("1", "FRE")
                        .ShouldBeFalse("Clinically Deleted Flag Should deleted for pay");
                    _appealAction.IsClinicallyDeletedFlagPresentByLineNo("4", "FRE")
                        .ShouldBeTrue("Clinically Deleted Flag Should not deleted for Deny");



                    _appealAction.ClickMoreOption();

                    appealProcessingHistory = _appealAction.ClickAppealProcessingHx();
                    var createdDate = DateTime.Now.Date.ToString("MM/dd/yyyy");
                    int n = 3;
                    do
                    {
                        if (appealProcessingHistory.GetAppealAuditGridTableDataValue(n, 3) == "Complete")
                            break;
                        n++;
                    } while (n < 10);

                    appealProcessingHistory.GetAppealAuditGridTableDataValue(n, 1).Split(' ')[0]
                        .ShouldBeEqual(createdDate, "Modified Date :");
                    VerifyThatNameIsInCorrectWithUserNameFormat(
                        appealProcessingHistory.GetAppealAuditGridTableDataValue(n, 2), "Modified By ");
                    appealProcessingHistory.GetAppealAuditGridTableDataValue(n, 3)
                        .ShouldBeEqual("Complete", "Action :");



                    appealProcessingHistory.GetAppealAuditGridTableDataValue(n, 6)
                        .ShouldBeEqual("Complete", "Status :");

                    VerifyThatNameIsInCorrectWithUserNameFormat(
                        appealProcessingHistory.GetAppealAuditGridTableDataValue(n, 9), "Assigned To ");
                    VerifyThatNameIsInCorrectWithUserNameFormat(
                        appealProcessingHistory.GetAppealAuditGridTableDataValue(n, 9), "Primary Reviewer ");

                    appealProcessingHistory.GetAppealAuditGridTableDataValue(n, 11).ShouldBeEqual("0", "Billable Time");
                    VerifyThatDateIsInCorrectFormat(appealProcessingHistory.GetAppealAuditGridTableDataValue(n, 12),
                        "DueDate ");
                    appealProcessingHistory.GetAppealAuditGridTableDataValue(n, 13)
                        .ShouldBeEqual("Normal", "Priority :");
                    appealProcessingHistory.GetAppealAuditGridTableDataValue(n, 14).ShouldBeEqual("F", "Product :");
                    appealProcessingHistory.GetAppealAuditGridTableDataValue(n, 15)
                        .ShouldBeEqual("UINOTE", "Comments :");

                    n = 3;
                    var flag = false;
                    do
                    {

                        if (appealProcessingHistory.GetAppealAuditGridTableDataValue(n, 4) == "Pay")
                        {
                            appealProcessingHistory.GetAppealAuditGridTableDataValue(n, 5)
                                .ShouldBeEqual(payReasonCode.Split('-')[0].Trim(), "Reason:");
                            appealProcessingHistory.GetAppealAuditGridTableDataValue(n, 7)
                                .ShouldBeEqual(claimSeq, "Claim Sequence");
                            appealProcessingHistory.GetAppealAuditGridTableDataValue(n, 8)
                                .ShouldBeEqual("1", "Line No :");
                            if (flag)
                                break;
                            flag = true;
                        }

                        if (appealProcessingHistory.GetAppealAuditGridTableDataValue(n, 4) == "Deny")
                        {
                            appealProcessingHistory.GetAppealAuditGridTableDataValue(n, 5)
                                .ShouldBeEqual(denyReasonCode.Split('-')[0].Trim(), "Reason:");
                            appealProcessingHistory.GetAppealAuditGridTableDataValue(n, 7)
                                .ShouldBeEqual(claimSeq, "Claim Sequence");
                            appealProcessingHistory.GetAppealAuditGridTableDataValue(n, 8)
                                .ShouldBeEqual("4", "Line No :");
                            if (flag)
                                break;
                            flag = true;
                        }

                        n++;
                    } while (n < 10);

                    appealProcessingHistory.CloseAppealProcessingHistoryPageBackToAppealActionPage();

                }
                finally
                {
                    //if (automatedBase.CurrentPage.Equals(typeof(AppealSummaryPage)) ||
                    //    automatedBase.CurrentPage.CurrentPageUrl.Contains("appeal_summary"))
                    //{
                    //    if (_appealSummary == null)
                    //        automatedBase.CurrentPage = _appealSummary = _appealAction.SwitchToAppealSummary();
                    //    if (!_appealSummary.IsEditAppealFormDisplayed())
                    //        _appealSummary.ClickOnEditIcon();
                    //    _appealSummary.SelectEditAppealFieldDropDownListByLabel("Status", "New");
                    //    _appealSummary.ClickOnSaveButtonOnEditAppeal();
                    //    automatedBase.CurrentPage = _appealAction;
                    //}

                    //if (automatedBase.CurrentPage.Equals(typeof(AppealActionPage)))
                    //{
                    //    _appealAction.CloseAnyPopupIfExist();
                    //    if (_appealAction.IsAppealLetterSection())
                    //        _appealAction.ClickOnAppealLetter();
                    //    automatedBase.CurrentPage = _appealSearch = _appealAction.ClickOnSearchIconToNavigateAppealSearchPage();
                    //}
                }

                void ValidatePopupMessageWhenAppealisApprovedForDifferentCondition(string message, string reasonCode, string rationaleValue)
                {
                    if (!(IsNullOrEmpty(reasonCode) && IsNullOrEmpty(rationale)))
                    {
                        _appealAction.SetReasonCodeList(reasonCode);
                        _appealAction.SetEditAppealLineIfrmeEditorByHeader("Rationale", rationaleValue);
                    }
                    _appealAction.ClickOnApproveIcon();
                    _appealAction.IsPageErrorPopupModalPresent().ShouldBeTrue(message);
                    _appealAction.GetPageErrorMessage()
                        .ShouldBeEqual(
                            "A Result, Reason Code, and Rationale are required for all appeal lines in order to complete this appeal.");
                    _appealAction.ClosePageError();

                    if (IsNullOrEmpty(reasonCode) && IsNullOrEmpty(rationaleValue))
                        _appealAction.IsRedInvalidInActionResult()
                            .ShouldBeTrue("Eclamation Icon Present in Action Result");
                    else if (IsNullOrEmpty(rationaleValue))
                        _appealAction.IsInvalidInputPresentByLabel("Rationale")
                            .ShouldBeTrue("Eclamation Icon Present in Rationale");

                    else if (IsNullOrEmpty(reasonCode))
                        _appealAction.IsInvalidInputPresentByLabel("Reason Code")
                            .ShouldBeTrue("Eclamation Icon Present in Reason Code");

                }

                void VerifyDateStampIsInCorrectFormat(string date, string message)
                {
                    // string[] format = new string[] { "mm/dd/yyyy h:mm:ss tt" };
                    DateTime datetime;

                    DateTime.TryParse(date, out datetime).ShouldBeTrue(message + " '" + date + "' is date time stamp");

                }

                void VerifyThatDateIsInCorrectFormat(string date, string message, bool singleDigit = false)
                {
                    if (singleDigit)
                        Regex.IsMatch(date, @"^([1-9]|1[0-2])\/([1-9]|1\d|2\d|3[01])\/(19|20)\d{2}$").ShouldBeTrue(message + " '" + date + "' is in format M/D/YYYY");
                    else
                        Regex.IsMatch(date, @"^(0[1-9]|1[0-2])\/(0[1-9]|1\d|2\d|3[01])\/(19|20)\d{2}$").ShouldBeTrue(message + " '" + date + "' is in format MM/DD/YYYY");

                }

                void VerifyThatNameIsInCorrectWithUserNameFormat(string name, string message)
                {
                    Regex.IsMatch(name, @"^(\S+ )+\S+ +\(+\S+\)+$").ShouldBeTrue(message + " Name '" + name + "' is in format XXX XXX (XXX)");
                }

                void VerifyThatNameIsInCorrectFormat(string name, string message)
                {
                    Regex.IsMatch(name, @"^(\S+ )+\S+$").ShouldBeTrue(message + " '" + name + "' is in format XXX XXX ");
                }
            }
        }

       

      

        //[Test]
        //public void Verify_two_state_note_icon_with_more_options()
        //{
        //    TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
        //    var ClaimSequenceWithNote = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName, "ClaimSequenceWithNote", "Value");
        //    var ClaimSequenceWithoutNote = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName, "ClaimSequenceWithoutNote", "Value");
        //    var moreOptionsList = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName, "MoreOptionsList", "Value").Split(',');
        //    AppealNotePage appealNotePage = null;
        //    AppealProcessingHistoryPage appealProcessingHx = null;
        //    //AppealHistoryPage appealHx = null;
        //    _appealSearch.SelectAllAppeals();
        //    _appealSearch.SelectClientSmtst();

        //    try
        //    {
        //        automatedBase.CurrentPage =
        //            _appealAction = _appealSearch.SearchByClaimSequenceToGoAppealAction(ClaimSequenceWithNote);
        //        _appealAction.IsAppealNoteIconPresent().ShouldBeTrue("View Appeal Note Present");
        //        //appealNotePage = _appealAction.ClickOnViewNoteIcon();
        //        //appealNotePage.IsNoteRowPresent().ShouldBeTrue("Note Present");
        //        //appealNotePage.GetnoteTypeValue().ShouldBeEqual("Appeal", "Note Type Value");
        //        //appealNotePage.IsNoteFormDivPresent().ShouldBeTrue("Add Note Form Present");
        //        //automatedBase.CurrentPage = _appealAction = appealNotePage.CloseNotePopUpPageSwitchToAppealAction();
        //        automatedBase.CurrentPage = _appealSearch = _appealAction.ClickOnSearchIconToNavigateAppealSearchPage();
        //        automatedBase.CurrentPage =
        //           _appealAction = _appealSearch.SearchByClaimSequenceToGoAppealAction(ClaimSequenceWithoutNote);
        //        _appealAction.IsAddAppealNoteIconPresent().ShouldBeTrue("Add  Appeal Note Present");
        //        appealNotePage = _appealAction.ClickOnAddNoteIcon();
        //        appealNotePage.IsNoteRowPresent().ShouldBeFalse("Note should not Present");
        //        appealNotePage.IsNoteFormDivPresent().ShouldBeTrue("Add Note Form Present");


        //        automatedBase.CurrentPage = _appealAction = appealNotePage.CloseNotePopUpPageSwitchToAppealAction();
        //        _appealAction.ClickMoreOption();
        //        _appealAction.GetMoreOptionsList()
        //            .ShouldCollectionBeEqual(moreOptionsList, "More Options List Should Equal");

        //        appealProcessingHx = _appealAction.ClickAppealProcessingHx();
        //        appealProcessingHx.GetPageHeader().ShouldBeEqual("Appeal Processing History", "Appeal Processing Page Should Open");
        //        appealProcessingHx.CloseAppealProcessingHistoryPageBackToAppealActionPage();

        //        //appealHx = _newAppealAction.ClickAppealHistory();
        //        //appealHx.GetPageHeader().ShouldBeEqual("Appeal History", "Appeal Processing Page Should Open");
        //        // appealHx.IsProvSameCodesChecked().ShouldBeTrue("Prov/Same Codes Should checked by default");
        //        //appealHx.CloseAppealHistoryPageBackToAppealActionPage();



        //    }
        //    finally
        //    {
                
        //    }

        //}

        [Test]//US41377 + CAR-3121(CAR-3063)
        public void Verify_that_appeal_data_with_claim_line_data_are_populated()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealSearchPage _appealSearch;
                AppealActionPage _appealAction;
                NewPopupCodePage _newPopupCode;

                _appealSearch = automatedBase.QuickLaunch.NavigateToAppealSearch();
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var claimSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                    "ClaimSequence", "Value");
                var trigSpeciality = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                    "TrigSpeciality", "Value");
                var appealLevel = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                    "AppealLevel", "Value");
                var modifiedBy = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                    "ModifiedBy", "Value");
                var action = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName, "Action",
                    "Value");
                var status = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName, "Status",
                    "Value");
                var priority = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                    "Priority", "Value");
                var product = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName, "Product",
                    "Value");
                var note = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName, "Note",
                    "Value");
                var claimSeqForProvider = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                    "ClaimSeqForProvider", "Value");
                var appealSequenceMRR = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                    "AppealSequenceMRR", "Value");
                var claimSequenceMRR = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                    "ClaimSequenceMRR", "Value");
                _appealSearch.SelectAllAppeals();
                _appealSearch.SelectClientSmtst();
                _appealSearch.SetClaimSequenceForCotivitiUser(claimSeq);
                _appealSearch.ClickOnFindButton();
                _appealSearch.WaitForWorkingAjaxMessage();

                try
                {
                    automatedBase.CurrentPage =
                        _appealAction = _appealSearch.ClickOnAppealSequenceByStatusOrAppealToGoAppealAction();
                    StringFormatter.PrintMessageTitle(
                        "Verification Audit Entry in Appeal Processing Page when landed in New Appeal Action Page");
                    _appealAction.ClickMoreOption();

                    AppealProcessingHistoryPage _appealProcessingHistory = _appealAction.ClickAppealProcessingHx();
                    var createdDate = DateTime.Now.Date.ToString("M/d/yyyy");
                    _appealProcessingHistory.GetAppealAuditGridTableDataValue(1, 1).Split(' ')[0]
                        .ShouldBeEqual(createdDate, "Modified Date :"); //modifiedDate
                    _appealProcessingHistory.GetAppealAuditGridTableDataValue(1, 2)
                        .ShouldBeEqual(modifiedBy, "Modified By:"); //modifiedBy
                    VerifyThatNameIsInCorrectWithUserNameFormat(
                        _appealProcessingHistory.GetAppealAuditGridTableDataValue(1, 2), "Modified By ");
                    _appealProcessingHistory.GetAppealAuditGridTableDataValue(1, 3)
                        .ShouldBeEqual(action, "Action :"); //Action
                    _appealProcessingHistory.GetAppealAuditGridTableDataValue(1, 4)
                        .ShouldBeEqual("", "Result"); //Result
                    _appealProcessingHistory.GetAppealAuditGridTableDataValue(1, 5)
                        .ShouldBeEqual("", "Reason:"); //Line #

                    _appealProcessingHistory.GetAppealAuditGridTableDataValue(1, 6)
                        .ShouldBeEqual(status, "Status :"); //Status
                    _appealProcessingHistory.GetAppealAuditGridTableDataValue(1, 7)
                        .ShouldBeEqual("", "Claim Sequence"); //Claim Seq
                    _appealProcessingHistory.GetAppealAuditGridTableDataValue(1, 8)
                        .ShouldBeEqual("", "Line No :"); //Line #
                    VerifyThatNameIsInCorrectWithUserNameFormat(
                        _appealProcessingHistory.GetAppealAuditGridTableDataValue(1, 9), "Assigned To "); //Assigned to
                    VerifyThatNameIsInCorrectWithUserNameFormat(
                        _appealProcessingHistory.GetAppealAuditGridTableDataValue(1, 10), "Primary Reviewer "); //PR 
                    _appealProcessingHistory.GetAppealAuditGridTableDataValue(1, 11)
                        .ShouldBeEqual("", "Billable Time"); //Billable  #
                    VerifyThatDateIsInCorrectFormat(
                        _appealProcessingHistory.GetAppealAuditGridTableDataValue(1, 12), "DueDate ",
                        true); //Duedate # 

                    _appealProcessingHistory.GetAppealAuditGridTableDataValue(1, 13)
                        .ShouldBeEqual(priority, "Priority :"); //Priority
                    _appealProcessingHistory.GetAppealAuditGridTableDataValue(1, 14)
                        .ShouldBeEqual(product, "Product :"); //Product
                    _appealProcessingHistory.GetAppealAuditGridTableDataValue(1, 15)
                        .ShouldBeEqual(note, "Comments :"); //Comments
                    _appealProcessingHistory.CloseAppealProcessingHistoryPageBackToAppealActionPage();


                    ClaimActionPage newClaimAction = _appealAction.ClickOnClaimSequenceAndSwitchWindow();
                    var modifier = newClaimAction.GetClaimLineDetailsValueByLineNo(3, 2, 4);
                    var units = newClaimAction.GetClaimLineDetailsValueByLineNo(1, 2, 5);
                    var procDescription = newClaimAction.GetClaimLineDetailsValueByLineNo(1, 1, 5);
                    var dateOfService = newClaimAction.GetClaimLineDetailsValueByLineNo(1, 1, 3);
                    var sugPaid = newClaimAction.GetLineSugPaidValue("UNB");
                    var trigDOS = newClaimAction.GetTrigDOS();
                    var source = newClaimAction.GetSourceByFlag("UNB");

                    var uniqueFlagList =
                        newClaimAction.GetFlagListForClaimLine(1).Distinct()
                            .ToList(); //get only unique flag list for first claim line of claimAction
                    _appealAction.CloseAnyPopupIfExist();
                    _appealAction.GetFlagList().ShouldCollectionBeEqual(uniqueFlagList,
                        "Unique flags are present per line and flag order matches that of Claim Action");

                    _appealAction.GetClaimSequenceList()
                        .IsInAscendingOrder()
                        .ShouldBeTrue("Is Claim Seq in Selected Lines Column in ascending order");

                    _appealAction.GetLineNo().ShouldBeEqual("1", "Line No is displayed");
                    _appealAction.GetAppealDeny().ShouldBeEqual("Deny", "Deny Icon is displayed");
                    _appealAction.GetAppealPay().ShouldBeEqual("Pay", "Pay Icon is displayed");
                    _appealAction.GetAppealAdjust().ShouldBeEqual("Adjust", "Adjust Icon is displayed");
                    _appealAction.GetAppealNoDocs().ShouldBeEqual("No Docs", "No Docs Icon is displayed");
                    _appealAction.GetDateOfService().ShouldBeEqual(dateOfService, "Date of Service ");

                    var revCode = _appealAction.GetRevCode(2, 4);
                    _newPopupCode = _appealAction.ClickOnRevenueCodeandSwitch("Revenue Code - " + revCode, 2, 4);
                    _newPopupCode.GetPopupHeaderText().ShouldBeEqual("Revenue Code", "Header Text");
                    _newPopupCode.GetTextContent().ShouldBeEqual("Code:", "Code Text");
                    _newPopupCode.GetTextContent(2).ShouldBeEqual("Type:", "Type Text");
                    _newPopupCode.GetTextContent(3).ShouldBeEqual("Description", "Description Text");
                    _newPopupCode.ClosePopupOnAppealActionPage("Revenue Code - " + revCode);
                    _appealAction.GetModifier(2, 4).ShouldBeEqual(modifier, "Modifier for the flagged proc code");

                    var procCode = _appealAction.GetProcCode();
                    _newPopupCode = _appealAction.ClickOnProcCodeandSwitch("CPT Code - " + procCode);
                    _newPopupCode.GetPopupHeaderText().ShouldBeEqual("CPT Code", "Header Text");
                    _newPopupCode.GetTextContent().ShouldBeEqual("Code:", "Code Text");
                    _newPopupCode.GetTextContent(2).ShouldBeEqual("Type:", "Type Text");
                    _newPopupCode.GetTextContent(3).ShouldBeEqual("Description", "Description Text");
                    _newPopupCode.ClosePopupOnAppealActionPage("CPT Code - " + procCode);


                    _appealAction.GetProcDescription()
                        .ShouldBeEqual(procDescription,
                            "Proc Description equal to short description that corresponds to the procedure code on the line");
                    _appealAction.GetProcDescriptionToolTip()
                        .ShouldBeEqual(procDescription, "Proc Description Tooltip displayed");
                    _appealAction.GetUnits()
                        .ShouldBeEqual(units, "Units Should equal to reported for the flagged proc code");
                    _appealAction.GetSource().ShouldBeEqual(source,
                        "Source should equal to Source associated with the top flag on the line");
                    _appealAction.GetSourceToolTip().ShouldBeEqual(source, "Source Tooltip is displayed");
                    _appealAction.GetSugPaid().ShouldBeEqual(sugPaid, "Sug Paid amount associated with the claim line");

                    var trigCode = _appealAction.GetTrigCode();
                    _newPopupCode = _appealAction.ClickOnTrigCodeandSwitch("CPT Code - " + trigCode);
                    _newPopupCode.GetPopupHeaderText().ShouldBeEqual("CPT Code", "Header Text");
                    _newPopupCode.GetTextContent().ShouldBeEqual("Code:", "Code Text");
                    _newPopupCode.GetTextContent(2).ShouldBeEqual("Type:", "Type Text");
                    _newPopupCode.GetTextContent(3).ShouldBeEqual("Description", "Description Text");
                    _newPopupCode.ClosePopupOnAppealActionPage("CPT Code - " + trigCode);

                    _appealAction.GetTrigSpec()
                        .ShouldBeEqual(trigSpeciality, "Specialty associated with the trigger claim line");
                    _appealAction.GetTrigDOS()
                        .ShouldBeEqual(trigDOS,
                            "Trig date of service associated with the trigger claim line for the top flag");
                    _appealAction.GetAppealLevel().ShouldBeEqual(appealLevel, "Appeal level should equal and red");
                    _appealAction.IsAppealLevelPresent(2, 3)
                        .ShouldBeFalse("Appeal Level should  not displayed for appeal level 1");

                    automatedBase.CurrentPage = _appealSearch = _appealAction.ClickOnSearchIconToNavigateAppealSearchPage();
                    _appealSearch.SelectAllAppeals();
                    _appealSearch.SelectClientSmtst();
                    automatedBase.CurrentPage = _appealAction =
                        _appealSearch.SearchByClaimSequenceToGoAppealAction(claimSeqForProvider);
                    _appealAction.GetProviderByClaimGroup().ShouldNotBeEmpty("Provider Name should displayed");
                    _appealAction.GetSpecialtyByClaimGroup().ShouldNotBeEmpty("Specialty should displayed");

                    #region CAR-3121(CAR-3063)
                    _appealAction.ClickOnSearchIconToNavigateAppealSearchPage();
                    _appealSearch.SelectAllAppeals();
                    _appealSearch.SelectClientSmtst();
                    _appealSearch.SearchByAppealSequence(appealSequenceMRR);
                    _appealAction.IsAppealLevelPresent()
                        .ShouldBeTrue("Appeal Level should be displayed for MRR Appeal");
                    _appealAction.GetAppealLevel().ShouldBeEqual(_appealAction.GetAppealLevelFromDb(claimSequenceMRR, ClientEnum.SMTST),"Appeal Level Should Match");
                    #endregion

                }
                finally
                {
                }

                void VerifyThatDateIsInCorrectFormat(string date, string message, bool singleDigit = false)
                {
                    if (singleDigit)
                        Regex.IsMatch(date, @"^([1-9]|1[0-2])\/([1-9]|1\d|2\d|3[01])\/(19|20)\d{2}$").ShouldBeTrue(message + " '" + date + "' is in format M/D/YYYY");
                    else
                        Regex.IsMatch(date, @"^(0[1-9]|1[0-2])\/(0[1-9]|1\d|2\d|3[01])\/(19|20)\d{2}$").ShouldBeTrue(message + " '" + date + "' is in format MM/DD/YYYY");

                }

                void VerifyThatNameIsInCorrectWithUserNameFormat(string name, string message)
                {
                    Regex.IsMatch(name, @"^(\S+ )+\S+ +\(+\S+\)+$").ShouldBeTrue(message + " Name '" + name + "' is in format XXX XXX (XXX)");
                }
            }
        }

        [Test] //us41381
        public void Verify_that_header_level_icon_with_appropriate_title_appears_and_appeal_information_value()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealSearchPage _appealSearch;
                AppealActionPage _appealAction;

                _appealSearch = automatedBase.QuickLaunch.NavigateToAppealSearch();
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var claimSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                    "ClaimSequence", "Value");
                var claimSeqForRecordReview = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "ClaimSeqForRecordReview", "Value");
                _appealSearch.SelectAllAppeals();
                _appealSearch.SelectClientSmtst();
                _appealSearch.SetClaimSequenceForCotivitiUser(claimSeq);
                _appealSearch.ClickOnFindButton();
                _appealSearch.WaitForWorkingAjaxMessage();

                try
                {
                    var appealSeq = _appealSearch.GetAppealSequenceByStatus();
                    automatedBase.CurrentPage =
                        _appealAction = _appealSearch.ClickOnAppealSequenceByStatusOrAppealToGoAppealAction();
                    //_appealAction.GetNoteIconTitle().ShouldBeEqual("View Appeal Notes", "Note Icon Title");
                    _appealAction.GetDocumentIconTitle().ShouldBeEqual("View Appeal Documents", "Documents Icon Title");
                    _appealAction.GetLetterIconTitle()
                        .ShouldBeEqual("Generate Appeal Letter", "Complete Appeal Icon Title");
                    //_AppealAction.GetSaveDraftIconTitle().ShouldEqual("Save Appeal Draft", "Note Icon Title");
                    _appealAction.GetSearchIconTitle()
                        .ShouldBeEqual("Return to Appeal Search", "Save Appeal Draft Icon Title");
                    _appealAction.GetMoreOptionIconTitle().ShouldBeEqual("More Options", "More Options Icon Title");

                    _appealAction.GetAppealCategory().ShouldNotBeEmpty("Category Id Should Present");
                    _appealAction.GetAppealSequence()
                        .ShouldBeEqual(appealSeq, "Appeal Sequence Should Present and Should Equal");
                    VerifyThatNameIsInCorrectFormat(_appealAction.GetPrimaryReviewer(), "Primary Reviewer");
                    VerifyThatNameIsInCorrectFormat(_appealAction.GetAssignedTo(), "Assigned To");
                    _appealAction.GetStatus().ShouldBeEqual("New", "Status Should Present and <New>");
                    _appealAction.GetExternalDocumentId().ShouldNotBeEmpty("External Doc Id should present");

                    automatedBase.CurrentPage = _appealSearch = _appealAction.ClickOnSearchIconToNavigateAppealSearchPage();

                    automatedBase.CurrentPage = _appealAction =
                        _appealSearch.SearchByClaimSequenceToGoAppealAction(claimSeqForRecordReview);
                    _appealAction.GetRecordReview()
                        .ShouldBeEqual("Record Review", "Record Review Should Present and Equal");


                }
                finally
                {
                }

                void VerifyThatNameIsInCorrectFormat(string name, string message)
                {
                    Regex.IsMatch(name, @"^(\S+ )+\S+$").ShouldBeTrue(message + " '" + name + "' is in format XXX XXX ");
                }
            }
        }

       
        [Test]//TE-800
        public void Verify_view_appeal_letter_information_details()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealSearchPage _appealSearch;
                AppealActionPage _appealAction;

                _appealSearch = automatedBase.QuickLaunch.NavigateToAppealSearch();
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var claSeqHavingDocument = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "ClaSeqHavingDocument", "Value");
                var claSeqNotHavingDocument = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "ClaSeqNotHavingDocument", "Value");
                var lineNo = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName, "LineNo",
                    "Value");
                var cptCodes = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                    "CPTCodes", "Value");
                var fileTypeList = automatedBase.DataHelper
                    .GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName, "FileTypeList", "Value")
                    .Split(',').ToList();
                try
                {
                    _appealSearch.SelectAllAppeals();
                    _appealSearch.SelectClientSmtst();
                    automatedBase.CurrentPage = _appealAction =
                        _appealSearch.SearchByClaimSequenceToGoAppealAction(claSeqNotHavingDocument);

                    _appealAction.IsConsultantRationalePresent()
                        .ShouldBeFalse("Is Consultant Rationale Present for CV product?");
                    _appealAction.ClickOnAppealLetter();
                    var letterText = _appealAction.GetLetterBody();
                    letterText.AssertIsContained(
                        "We have reviewed the claim(s) listed below in response to your request. Our review considered the following documents:",
                        "Title of Letter");
                    letterText.AssertIsContained("There were no document types associated with this request.",
                        "Message when No Document upload");
                    letterText.AssertIsContained("The results for each line are stated below:", "Title of result");
                    letterText.AssertIsContained(Format("Claim Sequence: {0}", claSeqNotHavingDocument), "Claim Seq");
                    letterText.AssertIsContained(Format("Line {0}, Code {1}", lineNo, cptCodes),
                        "Line number and associated CPT codes");
                    letterText.AssertIsContained(_appealAction.GetEditAppealLineIframeEditorByHeader("Rationale"),
                        "Rationale Text");
                    letterText.AssertIsContained(_appealAction.GetEditAppealLineIframeEditorByHeader("Summary"),
                        "Summary Text");
                    letterText.AssertIsContained("We recommend reimbursement for the code(s) listed above.",
                        "Closing Statement for Pay Action");

                    _appealAction.ClickOnDenyIcon();
                    if (_appealAction.IsPageErrorPopupModalPresent())
                    {
                        _appealAction.ClickOkCancelOnConfirmationModal(true);
                    }

                    _appealAction.IsConsultantRationalePresent()
                        .ShouldBeFalse("Is Consultant Rationale Present for CV product?");
                    _appealAction.IsInputFieldPresentByLabel("New Pay Code")
                        .ShouldBeFalse("Is New Pay Code display when the DENY result is selected for CV product?");
                    _appealAction.GetLetterBody()
                        .ShouldBeEqual(letterText, "Letter Message Should same before refresh after action changed");
                    _appealAction.ClickOnRefreshIcon();
                    letterText = _appealAction.GetLetterBody();
                    letterText.AssertIsContained("We do not recommend reimbursement for the code(s) listed above.",
                        "Closing Statement for Deny Action");

                    _appealAction.ClickOnAdjustIcon();
                    if (_appealAction.IsPageErrorPopupModalPresent())
                    {
                        _appealAction.ClickOkCancelOnConfirmationModal(true);
                    }

                    _appealAction.IsConsultantRationalePresent()
                        .ShouldBeFalse("Is Consultant Rationale Present for CV product?");
                    _appealAction.ClickOnRefreshIcon();
                    letterText = _appealAction.GetLetterBody();
                    letterText.Contains("reimbursement").ShouldBeFalse("Closing Statement should not displayed");

                    _appealAction.ClickOnNoDocsIcon();

                    if (_appealAction.IsPageErrorPopupModalPresent())
                    {
                        _appealAction.ClickOkCancelOnConfirmationModal(true);
                    }

                    _appealAction.IsConsultantRationalePresent()
                        .ShouldBeFalse("Is Consultant Rationale Present for CV product?");
                    _appealAction.ClickOnRefreshIcon();
                    letterText = _appealAction.GetLetterBody();
                    letterText.Contains("reimbursement").ShouldBeFalse("Closing Statement should not displayed");

                    automatedBase.CurrentPage = _appealSearch = _appealAction.ClickOnSearchIconToNavigateAppealSearchPage();
                    _appealSearch.SelectAllAppeals();
                    _appealSearch.SelectClientSmtst();
                    automatedBase.CurrentPage = _appealAction =
                        _appealSearch.SearchByClaimSequenceToGoAppealAction(claSeqHavingDocument);
                    _appealAction.ClickOnAppealLetter();
                    letterText = _appealAction.GetLetterBody();
                    foreach (var fileType in fileTypeList)
                    {
                        letterText.AssertIsContained(fileType, "File Type List Should Display");
                    }

                    _appealAction.ClickOnAppealLetter();
                    automatedBase.CurrentPage = _appealSearch = _appealAction.ClickOnSearchIconToNavigateAppealSearchPage();

                }
                finally
                {

                }
            }
        }
        
        [NonParallelizable]
        [Test, Category("OnDemand")]//US58221 + TANT-189
        public void Verify_switch_flag_enabled_or_disabled_based_on_allow_switch_flag_on_appeal_action_checkbox_in_client_setting()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealSearchPage _appealSearch;
                AppealActionPage _appealAction;
                ClientSearchPage _clientSearch;

                _appealSearch = automatedBase.QuickLaunch.NavigateToAppealSearch();
                _appealSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search", AppealQuickSearchTypeEnum.OutstandingDCIAppeals.GetStringValue());
                _appealSearch.GetSideBarPanelSearch.ClickOnFindButton();
                _appealSearch.WaitForWorking();

                _appealAction = _appealSearch.ClickOnAppealSequenceToNavigateAppealActionPageByRow(1);
                _appealSearch = _appealAction.ClickOnSearchIconToNavigateAppealSearchPage();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var claimSequence = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                    "ClaimSequence", "Value");

                automatedBase.CurrentPage = _clientSearch =
                    _appealSearch.NavigateToClientSearch();
                try
                {
                    
                    _clientSearch.SearchByClientCodeToNavigateToClientProfileViewPage(ClientEnum.SMTST.ToString(),
                        ClientSettingsTabEnum.Configuration.GetStringValue());
                    var list = _clientSearch.GetDropDownListForClientSettingsByLabel(ConfigurationSettingsEnum
                        .NonReverseFlag.GetStringValue());
                    var rnd = new Random();
                    var index = rnd.Next(0, list.Count - 1);
                    var option = list[index];
                    _clientSearch.SelectDropDownListForClientSettingsByLabel(
                        ConfigurationSettingsEnum.NonReverseFlag.GetStringValue(), option);
                    StringFormatter.PrintMessageTitle(
                        Format(
                            "Verify Switch Icon should not depend  on Allow Add/Switch Flags On Claim Action for <{0}>",
                            option));
                    var active = false;
                    for (var i = 0; i < 2; i++)
                    {
                        _clientSearch.ClickOnRadioButtonByLabel(
                            ConfigurationSettingsEnum.AppealActionSwitchFlag.GetStringValue(), active);
                        _clientSearch.GetSideWindow.Save(waitForWorkingMessage: true);
                        automatedBase.CurrentPage = _appealSearch = _clientSearch.NavigateToAppealSearch();
                        automatedBase.CurrentPage =
                            _appealAction =
                                _appealSearch.FindByClaimSequenceToNavigateAppealAction(claimSequence, false);
                        _appealAction.ClickOnAdjustIcon();
                        if (active)
                            _appealAction.IsSwitchDeleteIconDisabledByRow().ShouldBeFalse("Is Switch Icon Disabled?");
                        else
                            _appealAction.IsSwitchDeleteIconDisabledByRow().ShouldBeTrue("Is Switch Icon Disabled?");

                        automatedBase.CurrentPage = _appealSearch = _appealAction.ClickOnSearchIconToNavigateAppealSearchPage();
                        automatedBase.CurrentPage = _clientSearch =
                            _appealSearch.NavigateToClientSearch();
                        _clientSearch.SearchByClientCodeToNavigateToClientProfileViewPage(ClientEnum.SMTST.ToString(),
                            ClientSettingsTabEnum.Configuration.GetStringValue());
                        active = true;
                    }
                }

                finally
                {
                    if (automatedBase.CurrentPage.GetPageHeader() == "Appeal Action")
                        automatedBase.CurrentPage = _appealSearch = _appealAction.ClickOnSearchIconToNavigateAppealSearchPage();
                    if (automatedBase.CurrentPage.GetPageHeader() == "Appeal Search")
                        automatedBase.CurrentPage = _clientSearch = _appealSearch.NavigateToClientSearch();
                    _clientSearch.SearchByClientCodeToNavigateToClientProfileViewPage(ClientEnum.SMTST.ToString());
                    if (automatedBase.CurrentPage.GetPageHeader() != "Client Search")
                        automatedBase.CurrentPage = _clientSearch = automatedBase.CurrentPage.NavigateToClientSearch();
                    _clientSearch.SearchByClientCodeToNavigateToClientProfileViewPage(ClientEnum.SMTST.ToString(),
                        ClientSettingsTabEnum.Configuration.GetStringValue());
                    _clientSearch.SelectDropDownListForClientSettingsByLabel(
                        ConfigurationSettingsEnum.NonReverseFlag.GetStringValue(), "YES");
                    _clientSearch.ClickOnRadioButtonByLabel(ConfigurationSettingsEnum.AppealActionSwitchFlag
                        .GetStringValue());
                    _clientSearch.GetSideWindow.Save(waitForWorkingMessage: true);
                    automatedBase.CurrentPage = _appealSearch = _clientSearch.NavigateToAppealSearch();
                    _appealSearch.CloseAnyPopupIfExist();
                }
            }
        }

        [Test] //US66663
        public void Verify_functionality_to_delete_appeal_line_in_appeal_action()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealSearchPage _appealSearch;
                AppealActionPage _appealAction;
                AppealManagerPage _appealManager;
                AppealProcessingHistoryPage _appealProcessingHx;

                _appealSearch = automatedBase.QuickLaunch.NavigateToAppealSearch();
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var claimSequence = automatedBase.DataHelper
                    .GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName, "ClaimSequence", "Value")
                    .Split(';');

                _appealSearch.SelectAllAppeals();
                _appealSearch.SelectClientSmtst();
                automatedBase.CurrentPage = _appealAction = _appealSearch.SearchByClaimSequenceToGoAppealAction(claimSequence[1]);

                StringFormatter.PrintMessageTitle("Verification of delete icon for single appeal line");
                _appealAction.ClickOnDeleteAppealLineIcon();
                _appealAction.IsPageErrorPopupModalPresent()
                    .ShouldBeTrue("When delete icon on appeal is clicked, a confirmation pop up  should be shown.");
                _appealAction.GetPageErrorMessage()
                    .ShouldBeEqual("Line cannot be deleted. An appeal must have at least one line.",
                        "Page error message when trying to delete single appeal line is as expected and equal");
                _appealAction.ClosePageError();
                _appealAction.ClickOnSearchIconToNavigateAppealSearchPage();
                if (_appealAction.IsPageErrorPopupModalPresent())
                    _appealAction.ClickOkCancelOnConfirmationModal(true);
                var appealCreator = _appealSearch.NavigateToAppealCreator();
                automatedBase.CurrentPage = appealCreator;
                appealCreator.SearchByClaimSequenceForLockedClaim(claimSequence[0]);
                if (appealCreator.IsClaimLocked())
                {
                    automatedBase.CurrentPage = _appealManager = appealCreator.NavigateToAppealManager();

                    _appealManager.DeleteAppealsAssociatedWithClaim(claimSequence[0]); //to delete appeal created
                    appealCreator = _appealManager.NavigateToAppealCreator();
                    appealCreator.SearchByClaimSequence(claimSequence[0]);
                }

                appealCreator.ClickOnClaimLevelSection("Select Appeal Lines");
                appealCreator.CreateAppealForClaimSequence("Coding Validation", false, "12345");
                automatedBase.CurrentPage = _appealSearch = appealCreator.NavigateToAppealSearch();
                _appealSearch.SelectAllAppeals();
                _appealSearch.SelectClientSmtst();
                automatedBase.CurrentPage = _appealAction = _appealSearch.SearchByClaimSequenceToGoAppealAction(claimSequence[0]);
                var countAppealLine = _appealAction.GetCountOfAppealLine();

                for (var i = 1; i <= countAppealLine; i++)
                {
                    _appealAction.IsDeleteAppealLineIconPresentByRow(i)
                        .ShouldBeTrue(Format("Is Delete Icon present at row=<{0}>?", i));
                    _appealAction.GetDeleteAppealLineIconTooltip(i)
                        .ShouldBeEqual("Delete Appeal Line",
                            Format("Tooltip for delete appeal icon should equal at row=<{0}>", i));
                }

                _appealAction.ClickOnDeleteAppealLineIcon();
                _appealAction.IsPageErrorPopupModalPresent()
                    .ShouldBeTrue("When delete icon on appeal is clicked, a confirmation pop up should be shown.");
                _appealAction.GetPageErrorMessage()
                    .ShouldBeEqual("The line will be deleted from the appeal. Do you want to continue?",
                        "Confirmattion pop up message should be as expected and equal.");

                StringFormatter.PrintMessageTitle("Verification of OK/Cancel button on message pop up");

                _appealAction.ClickOkCancelOnConfirmationModal(false);
                _appealAction.GetCountOfAppealLine().ShouldBeEqual(countAppealLine,
                    "Appeal line should equal when cancel button is clicked");
                _appealAction.ClickOnDeleteAppealLineIcon();

                _appealAction.ClickOkCancelOnConfirmationModal(true);
                _appealAction.GetCountOfAppealLine().ShouldBeEqual(countAppealLine - 1,
                    "Appeal line should decreased by 1 when ok button is clicked");
                _appealAction.ClickMoreOption();
                _appealProcessingHx = _appealAction.ClickAppealProcessingHx();

                _appealProcessingHx.GetAppealAuditGridTableDataValue(1, 1).Split(' ')[0]
                    .ShouldBeEqual(DateTime.Now.Date.ToString("M/d/yyyy"), "Modified Date :");

                VerifyThatNameIsInCorrectWithUserNameFormat(
                    _appealProcessingHx.GetAppealAuditGridTableDataValue(1, 2), "Is Modified By equal?");

                _appealProcessingHx.GetAppealAuditGridTableDataValue(1, 3)
                    .ShouldBeEqual("RemoveLine", "Action Should equal"); //Action
                _appealProcessingHx.GetAppealAuditGridTableDataValue(1, 8)
                    .ShouldBeEqual("1", "Delete Line should be same"); //Line #
                _appealProcessingHx.CloseAppealProcessingHistoryPageBackToAppealActionPage();

                void VerifyThatNameIsInCorrectWithUserNameFormat(string name, string message)
                {
                    Regex.IsMatch(name, @"^(\S+ )+\S+ +\(+\S+\)+$").ShouldBeTrue(message + " Name '" + name + "' is in format XXX XXX (XXX)");
                }
            }
        }

        [NonParallelizable]
        [Test, Category("OnDemand")]//US68890 + TE-189
        public void Verify_completed_appeal_is_set_to_Closed_status_automatically_when_Close_Client_Appeals_client_setting_is_set_true()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealSearchPage _appealSearch;
                AppealActionPage _appealAction;
                ClientSearchPage _clientSearch;
                AppealManagerPage _appealManager;

                _appealSearch = automatedBase.QuickLaunch.NavigateToAppealSearch();
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var claimSequence = automatedBase.DataHelper
                    .GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName, "ClaimSequence", "Value");
                var reasonCode = automatedBase.DataHelper
                    .GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName, "ReasonCode", "Value");
                var rationale = "Test Deny" + DateTime.Now;

                try
                {
                    automatedBase.CurrentPage = _clientSearch = automatedBase.CurrentPage.NavigateToClientSearch();
                    _clientSearch.SearchByClientCodeToNavigateToClientProfileViewPage(ClientEnum.SMTST.ToString(),
                        ClientSettingsTabEnum.Product.GetStringValue());
                    _clientSearch.ClickAutoCloseAppealRadioButton(true);

                    _clientSearch.IsRadioButtonOnOffByLabel(ProductAppealsEnum.AutoCloseAppeals.GetStringValue())
                        .ShouldBeTrue("Is Close Client Appeal radio button set to ON?");
                    _clientSearch.GetInfoHelpTooltipByLabel(ProductAppealsEnum.AutoCloseAppeals.GetStringValue())
                        .ShouldBeEqual("Enabling will automatically set appeals to CLOSED status.",
                            "Correct Tooltip is displayed.");

                    automatedBase.CurrentPage = _appealManager = automatedBase.CurrentPage.NavigateToAppealManager();
                    _appealManager.SearchByClaimSequence(claimSequence, ClientEnum.SMTST.ToString());
                    _appealManager.ChangeStatusOfAppealByRowSelection(); //changed status to new

                    automatedBase.CurrentPage = _appealSearch = _appealManager.NavigateToAppealSearch();
                    _appealSearch.SelectAllAppeals();
                    _appealSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Client",
                        ClientEnum.SMTST.ToString());
                    automatedBase.CurrentPage = _appealAction =
                        _appealSearch.SearchByClaimSequenceToGoAppealAction(claimSequence);

                    _appealAction.ClickOnDenyIcon();
                    if (IsNullOrEmpty(_appealAction.GetReasonCodeInput()))
                        _appealAction.SetReasonCodeList(reasonCode);
                    if (IsNullOrEmpty(_appealAction.GetEditAppealLineIframeEditorByHeader("Rationale")))
                        _appealAction.SetEditAppealLineIfrmeEditorByHeader("Rationale", rationale);
                    _appealAction.ClickOnAppealLetter();
                    _appealAction.ClickOnApproveIcon();
                    _appealAction.ClickOkCancelOnConfirmationModal(true);
                    _appealAction.WaitForWorking();

                    _appealSearch.GetSearchResultByColRow(11).ShouldBeEqual(
                        AppealStatusEnum.Closed.GetStringDisplayValue(),
                        "Completed appeal should Closed automatically");
                }
                finally
                {
                    _appealSearch.GetCommonSql.UpdateSpecificClientDataInDB("CLOSE_CLIENT_APPEALS='F'", ClientEnum.SMTST.ToString());
                }
            }
        }


        [Test] //US69646
        [Retrying(Times = 3)]
        public void Validate_appeal_line_results_icon_is_enabled_only_if_their_is_authority()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealSearchPage _appealSearch;
                AppealActionPage _appealAction;

                _appealSearch = automatedBase.QuickLaunch.NavigateToAppealSearch();
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var claimSequence = automatedBase.DataHelper
                    .GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName, "ClaimSequence", "Value");
                try
                {
                    var topFlagAndProduct = _appealSearch.GetTopFlagAndProductForClaimSequence(claimSequence);
                    var topflag = topFlagAndProduct[0][0];
                    var productTypeForFlag = topFlagAndProduct[0][1];

                    _appealSearch.IsRoleAssigned<UserProfileSearchPage>(new List<string> {automatedBase.EnvironmentManager.Username},
                        RoleEnum.ClaimsProcessor.GetStringValue()).ShouldBeTrue(
                        $"Is Manage Edits present for current user<{automatedBase.EnvironmentManager.Username}>");

                    productTypeForFlag.ShouldBeEqual("F",
                        "Top flag corresponds to  CV product, where F equals product type CV ");
                    automatedBase.CurrentPage.IsAvailableAssignedRowPresent(1, RoleEnum.AppealsProcessor.GetStringValue())
                        .ShouldBeTrue(
                            $"Is Manage Appeals present for current user<{automatedBase.EnvironmentManager.Username}>");
                    automatedBase.CurrentPage.IsAvailableAssignedRowPresent(1, RoleEnum.ClinicalValidationAnalyst.GetStringValue())
                        .ShouldBeTrue(
                            $"Is CV present for current user<{automatedBase.EnvironmentManager.Username}>");

                    _appealSearch.IsRoleAssigned<UserProfileSearchPage>(new List<string> {"Test 7", "Automation"},
                        RoleEnum.ClaimsProcessor.GetStringValue(), false).ShouldBeTrue(
                        $"Is Manage Edits present for current user< Test 7 Automation >");

                    productTypeForFlag.ShouldBeEqual("F",
                        "Top flag corresponds to  CV product, where F equals product type CV ");
                    automatedBase.CurrentPage.IsAvailableAssignedRowPresent(1, RoleEnum.AppealsProcessor.GetStringValue())
                        .ShouldBeTrue(
                            $"Is Manage Appeals present for current user< Test 7 Automation >");
                    automatedBase.CurrentPage.IsAvailableAssignedRowPresent(1, RoleEnum.ClinicalValidationAnalyst.GetStringValue())
                        .ShouldBeFalse(
                            $"Is CV present for current user< Test 7 Automation >");


                    automatedBase.CurrentPage.NavigateToAppealSearch();
                    _appealSearch.SelectAllAppeals();
                    _appealSearch.SelectClientSmtst();
                    automatedBase.CurrentPage =
                        _appealAction = _appealSearch.SearchByClaimSequenceToGoAppealAction(claimSequence, true);
                    _appealAction.IsDenyIconEnabled()
                        .ShouldBeTrue(
                            "Deny Appeal result icon should be enabled for user with product authority to top flag on the line.");
                    _appealAction.IsPayIconEnabled()
                        .ShouldBeTrue(
                            "Pay Appeal result icon should be enabled for user with product authority to top flag on the line.");
                    _appealAction.IsAdjustIconEnabled()
                        .ShouldBeTrue(
                            "Adjust Appeal result icon should be enabled for user with product authority to top flag on the line.");
                    _appealAction.IsNoDocsIconEnabled()
                        .ShouldBeTrue(
                            "No Docs Appeal result icon should be enabled for user with product authority to top flag on the line.");

                    automatedBase.CurrentPage = _appealSearch = _appealSearch.Logout().LoginAsHciUserWithManageEditAuthority()
                        .NavigateToAppealSearch();
                    _appealSearch.SelectAllAppeals();
                    _appealSearch.SelectClientSmtst();
                    automatedBase.CurrentPage =
                        _appealAction = _appealSearch.SearchByClaimSequenceToGoAppealAction(claimSequence, true);
                    _appealAction.GetFlagList().ShouldContain(topflag,
                        Format("Flag list should contain top flag {0} in appeal line", topflag));
                    _appealAction.IsAppealResultTypeEllipsesDisabledInAppealAction()
                        .ShouldBeTrue(
                            "Deny,Pay,Adjust and No Docs Appeal result icons should be Disabled for user with no product authority to top flag on the line.");
                    _appealAction.ClickOnSearchIconToNavigateAppealSearchPage();
                }
                finally
                {
                    _appealSearch.Logout();
                }
            }
        }

        [Test] //CAR-925(CAR-774) + TE-889
        [Category("EdgeNonCompatible")]
        public void Validate_Dental_Appeal_Letters()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealSearchPage _appealSearch;
                AppealSummaryPage _appealSummary;

                _appealSearch = automatedBase.QuickLaunch.NavigateToAppealSearch();
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var paramsList = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var claseqList =
                    paramsList["ClaseqList"].Split(';').ToList(); // Three claseqs for each appeal type = 'A', 'R', 'D' 
                var claseqWithoutConsultantInfo = paramsList["ClaseqWithoutConsultantInfo"];
                var dentalAppealLetterContent = paramsList["DentalAppealLetterContent"].Split(';').ToList();
                var reviewerAddressRegex = paramsList["ReviewerAddressRegex"].Split(';').ToList();
                var reviewerAddressRegexIfConsultantPresent =
                    paramsList["ReviewerAddressRegexIfConsultantPresent"].Split(';').ToList();

                try
                {
                    foreach (var claseq in claseqList)
                    {
                        _appealSummary = _appealSearch.FindByClaimSequenceToNavigateAppealummary(claseq);

                        var appealLetter = _appealSummary.ClickAppealLetter();

                        StringFormatter.PrintMessageTitle(
                            "Verifying the contents of Appeal Letter which has consultant information");
                        ValidateAppealLetterContentForDCI(dentalAppealLetterContent);
                        ValidateAppealLetterContentForDCI(reviewerAddressRegex, regex: true);
                        ValidateAppealLetterContentForDCI(reviewerAddressRegexIfConsultantPresent, regex: true);

                        StringFormatter.PrintMessageTitle(
                            "Verifying downloading the Appeal Letter in PDF format and verifying it's contents");
                        var fileName = _appealSummary.ClickOnDownloadPDFAndGetFileName(appealLetter);
                        ValidatePDFContents(fileName, dentalAppealLetterContent);
                        ValidatePDFContents(fileName, reviewerAddressRegex, regex: true);
                        ValidatePDFContents(fileName, reviewerAddressRegexIfConsultantPresent, regex: true);
                        _appealSummary.CloseAnyPopupIfExist();
                        _appealSearch = _appealSummary.ClickOnSearchIconToNavigateAppealSearchPage();
                    }

                    StringFormatter.PrintMessageTitle(
                        "Verifying the contents of Appeal Letter which does not have consultant information");
                    _appealSummary =
                        _appealSearch.FindByClaimSequenceToNavigateAppealummary(claseqWithoutConsultantInfo);
                    _appealSummary.ClickAppealLetter();
                    ValidateAppealLetterContentForDCI(reviewerAddressRegexIfConsultantPresent, true, false);
                    _appealSummary.CloseLetterPopUpPage();
                }

                finally
                {

                }

                void ValidateAppealLetterContentForDCI(List<string> appealLetterContent, bool regex = false, bool isConsultantPresent = true)
                {
                    var letterText = _appealSummary.GetLetterBodyForDCIAppealLetter();

                    if (isConsultantPresent)
                    {
                        if (!regex)
                            foreach (var content in appealLetterContent)
                                letterText.AssertIsContained(content, "Is First Paragraph Prsent in Appeal Letter?");
                        else
                            foreach (var content in appealLetterContent)
                                Regex.IsMatch(letterText, content).ShouldBeTrue(Format("{0} Is present in the appeal letter", content));
                    }
                    else
                    {
                        if (!regex)
                            foreach (var content in appealLetterContent)
                                letterText.AssertIsNotContained(content, "Is First Paragraph Prsent in Appeal Letter?");
                        else
                            foreach (var content in appealLetterContent)
                                Regex.IsMatch(letterText, content).ShouldBeFalse(Format("{0} Is not present in the appeal letter", content));
                    }
                }

                void ValidatePDFContents(string fileName, List<string> appealLetterContent, bool regex = false)
                {
                    using (PdfReader reader = new PdfReader("C:/Users/uiautomation/Downloads/" + fileName))
                    {
                        StringBuilder text = new StringBuilder();
                        ITextExtractionStrategy Strategy = new LocationTextExtractionStrategy();

                        string page = "";

                        for (int i = 1; i <= reader.NumberOfPages; i++)
                            page += PdfTextExtractor.GetTextFromPage(reader, i, Strategy);

                        if (!regex)
                            foreach (var content in appealLetterContent)
                            {
                                var pageTrim = Regex.Replace(page, @"\s+", "");
                                var contentTrim = Regex.Replace(content, @"\s+", "");
                                pageTrim.AssertIsContained(contentTrim, "Is Appeal Letter Paragraph Present in PDF Document?");
                            }
                        else
                            foreach (var content in appealLetterContent)
                                Regex.IsMatch(page, content).ShouldBeTrue(Format("{0} Is present in the appeal letter", content));
                    }
                }
            }
        }


        [Test] //CAR-260 + CAR-1137
        public void Verify_reminder_message_for_DCI_Appeals_for_some_states()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealSearchPage _appealSearch;
                AppealActionPage _appealAction;
                AppealProcessingHistoryPage _appealProcessingHx;

                _appealSearch = automatedBase.QuickLaunch.NavigateToAppealSearch();
                _appealSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search", AppealQuickSearchTypeEnum.OutstandingAppeals.GetStringValue());
                _appealSearch.GetSideBarPanelSearch.ClickOnFindButton();
                _appealSearch.WaitForWorking();

                _appealAction = _appealSearch.ClickOnAppealSequenceToNavigateAppealActionPageByRow(1);
                _appealSearch = _appealAction.ClickOnSearchIconToNavigateAppealSearchPage();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var paramsList = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var claseqList = paramsList["ClaseqList"].Split(';').ToList();

                int i = 0;
                foreach (string claSeq in claseqList)
                {
                    _appealSearch.SelectAllAppeals();
                    _appealSearch.SelectClientSmtst();
                    _appealSearch.SetClaimSequenceForCotivitiUser(claSeq);
                    _appealSearch.ClickOnFindButton();
                    _appealSearch.WaitForWorking();
                    if (i == 0)
                    {
                        automatedBase.CurrentPage = _appealAction =
                            _appealSearch.ClickOnAppealSequenceToNavigateAppealActionPageByRow(1);
                    }
                    else
                    {
                        _appealAction.HandleAutomaticallyOpenedActionPopup();
                    }

                    _appealAction.ClickOnEditIcon();
                    _appealAction.SelectEditAppealFieldDropDownListByLabel("Status", "State Consultant Required");
                    _appealAction.ClickOnSaveButton();
                    automatedBase.CurrentPage = _appealAction =
                        _appealSearch.ClickOnAppealSequenceToNavigateAppealActionPageByRow(1);
                    _appealAction.DeleteAppealAuditHistory();
                    var primaryReviewer = _appealAction.GetPrimaryReviewer();
                    var assignedTo = _appealAction.GetAssignedTo();
                    var status = _appealAction.GetStatus();
                    _appealAction.ClickOnDenyIcon();
                    _appealAction.IsDenyIconSelected().ShouldBeTrue("Deny Icon Should be selected.");
                    _appealAction.ClickOnEditIcon();
                    _appealAction.SelectEditAppealFieldDropDownListByLabel("Assigned to", "Test Automation5");
                    _appealAction.SelectEditAppealFieldDropDownListByLabel("Status", "State Consultant Complete");
                    _appealAction.ClickOnSaveButton();
                    if (i == 0)
                    {
                        _appealAction.IsPageErrorPopupModalPresent().ShouldBeTrue(
                            "Reminder message should pop up for the deny case for state requiring provider details.");
                        _appealAction.GetPageErrorMessage().ShouldBeEqual(
                            "Reminder! UT guidelines require that the provider is contacted prior to an adverse appeal determination. Has the provider been contacted?");
                        _appealAction.ClickOkCancelOnConfirmationModal(false);
                        _appealAction.GetPrimaryReviewer().ShouldBeEqual(primaryReviewer,
                            Format("Primary Reviewer should be {0}.", primaryReviewer));
                        _appealAction.GetAssignedTo()
                            .ShouldBeEqual(assignedTo, Format("Assigned to should be {0}.", assignedTo));
                        _appealAction.GetStatus().ShouldBeEqual(status, Format("Status should be {0}.", status));
                        _appealAction.ClickOnSaveButton();
                        _appealAction.ClickOkCancelOnConfirmationModal(true);
                        automatedBase.CurrentPage = _appealAction =
                            _appealSearch.ClickOnAppealSequenceToNavigateAppealActionPageByRow(1);
                        _appealAction.GetAssignedTo()
                            .ShouldBeEqual("Test Automation5", "Assigned to should be Test Automation5.");
                        _appealAction.GetStatus().ShouldBeEqual("State Consultant Complete",
                            "Status should be State Consultant Complete.");
                        _appealProcessingHx = _appealAction.ClickAppealProcessingHx();
                        _appealProcessingHx.GetAppealAuditGridTableDataValue(2, 3)
                            .ShouldBeEqual("Edit");
                        _appealProcessingHx.GetAppealAuditGridTableDataValue(3, 3)
                            .ShouldBeEqual("ConsultantContact");
                        _appealAction = _appealProcessingHx.CloseAppealProcessingHistoryPageBackToAppealActionPage();
                    }

                    i++;
                    _appealAction.NavigateToAppealSearch();
                    if (_appealAction.IsPageErrorPopupModalPresent())
                    {
                        _appealAction.ClickOkCancelOnConfirmationModal(true);
                    }
                }
            }
        }


        [Test] //CAR-1214 (CAR-1000) + TE-558
        public void Verify_MVC_Pop_Up_For_Appeal_Letter()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealSearchPage _appealSearch;
                AppealActionPage _appealAction;

                _appealSearch = automatedBase.QuickLaunch.NavigateToAppealSearch();
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var paramsList = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var appealSeqWithAppealLinesOnSameClaim = paramsList["AppealSeqWithAppealLinesOnSameClaim"];
                var appealSeqWithAppealLinesOnDifferentClaims = paramsList["AppealSeqWithAppealLinesOnDifferentClaims"];
                var paidRationaleNote = paramsList["PaidRationaleNote"];
                var paidSummaryNote = paramsList["PaidSummaryNote"];
                var adjustedRationaleNote = paramsList["AdjustedRationaleNote"];
                var adjustedSummaryNote = paramsList["AdjustedSummaryNote"];

                try
                {
                    _appealSearch.UpdateAppealStatusToIncomplete(appealSeqWithAppealLinesOnSameClaim);
                    _appealSearch.UpdateAppealStatusToIncomplete(appealSeqWithAppealLinesOnDifferentClaims);

                    _appealAction =
                        _appealSearch.SearchByAppealSequenceNavigateToAppealAction(appealSeqWithAppealLinesOnSameClaim);

                    StringFormatter.PrintMessage(Format("Completing the Appeal {0} to view its Appeal Letter",
                        appealSeqWithAppealLinesOnSameClaim));
                    _appealAction.ClickOnAppealLetter();
                    var letterPreviewBody = _appealAction.GetLetterBody().Split('\r').ToList();
                    letterPreviewBody = letterPreviewBody.Select(s => s.Replace("\n", "")).ToList();
                    letterPreviewBody.RemoveAll(s => s == "");

                    #region TE-558

                    _appealAction.ClickOnApproveIcon();
                    _appealAction.ClickOkOnConfirmationModalWithoutWait();
                    StringFormatter.PrintMessage("Verify Approve Icon Is Disabled");
                    _appealAction.IsApproveIconDisabled().ShouldBeTrue("Is Approve Icon Disabled ?");
                    _appealAction.WaitForWorking();
                    _appealAction.WaitForStaticTime(200);

                    #endregion

                    var appealLetterPage = _appealSearch.ClickOnAppealLetter(1);

                    StringFormatter.PrintMessage(Format(
                        "Verifying the Claim Line details in the Appeal Letter for appeal : {0}",
                        appealSeqWithAppealLinesOnSameClaim));
                    appealLetterPage.GetCountOfAppealClaimDetails().ShouldBeEqual(2,
                        "Appeal Line Details with same information and action should not be shown " +
                        "in separate fields in the Appeal Letter");

                    StringFormatter.PrintMessage(Format(
                        "Verifying the Claim Line details in the Appeal Letter for appeal {0}",
                        appealSeqWithAppealLinesOnSameClaim));
                    var firstAppealLineDetails = appealLetterPage.GetListOfLinesByAppealClaimDetailNumber(1);
                    firstAppealLineDetails.ShouldContain("Line 1, Code 29806", "Proc Details for Line 1");
                    firstAppealLineDetails.ShouldContain("Line 2, Code 29807", "Proc Details for Line 2");

                    var firstAppealLineNotesDetails = appealLetterPage.GetListOfNotesByAppealClaimDetailNumber(1);
                    firstAppealLineNotesDetails.ShouldContain(paidRationaleNote, "Is Rationale Note Present?");
                    firstAppealLineNotesDetails.ShouldContain(paidSummaryNote, "Is Summary Note Present?");

                    var secondAppealLineDetails = appealLetterPage.GetListOfLinesByAppealClaimDetailNumber(2);
                    secondAppealLineDetails.ShouldContain("Line 3, Code 29819", "Proc Details for Line 3");

                    var secondAppealLineNotesDetails = appealLetterPage.GetListOfNotesByAppealClaimDetailNumber(2);
                    secondAppealLineNotesDetails.ShouldContain(adjustedRationaleNote, "Is Rationale Note Present?");
                    secondAppealLineNotesDetails.ShouldContain(adjustedSummaryNote, "Is Summary Note Present?");
                    appealLetterPage.CloseLetterPopUpPageAndBackToAppealSearch();

                    StringFormatter.PrintMessageTitle(
                        "Verifying whether flag details in the preview pane are present in the Appeal Letter");
                    firstAppealLineDetails.CollectionShouldBeSubsetOf(letterPreviewBody,
                        "Is Appeal Line Details Same to the Appeal Action?");
                    firstAppealLineNotesDetails.CollectionShouldBeSubsetOf(letterPreviewBody,
                        "Is Appeal Line Note Details Same to the Appeal Action?");

                    secondAppealLineDetails.CollectionShouldBeSubsetOf(letterPreviewBody,
                        "Is Appeal Line Details Same to the Appeal Action?");
                    secondAppealLineNotesDetails.CollectionShouldBeSubsetOf(letterPreviewBody,
                        "Is Appeal Line Note Details Same to the Appeal Action?");

                    StringFormatter.PrintMessageTitle(
                        "Verification of the Appeal Letter contents for appeals with multiple appeal lines on Different Claims And with" +
                        "Different Results");
                    _appealAction =
                        _appealSearch.SearchByAppealSequenceNavigateToAppealAction(
                            appealSeqWithAppealLinesOnDifferentClaims);

                    StringFormatter.PrintMessage(Format("Completing the Appeal {0} to view its Appeal Letter",
                        appealSeqWithAppealLinesOnDifferentClaims));
                    _appealAction.ClickOnAppealLetter();
                    var letterPreviewBodyForMultipleClaimLines = _appealAction.GetLetterBody().Split('\r').ToList();
                    letterPreviewBodyForMultipleClaimLines =
                        letterPreviewBodyForMultipleClaimLines.Select(s => s.Replace("\n", "")).ToList();
                    letterPreviewBodyForMultipleClaimLines.RemoveAll(s => s == "");

                    _appealAction.ClickOnApproveIcon();
                    _appealAction.ClickOkCancelOnConfirmationModal(true);
                    _appealAction.WaitForWorking();

                    StringFormatter.PrintMessage(Format(
                        "Verifying the Claim Line details in the Appeal Letter for appeal : {0}",
                        appealSeqWithAppealLinesOnDifferentClaims));
                    appealLetterPage = _appealSearch.ClickOnAppealLetter(1);
                    appealLetterPage.GetCountOfAppealClaimDetails().ShouldBeEqual(2,
                        "Appeal Line Details with same information and action should not be shown " +
                        "in separate fields in the Appeal Letter");

                    StringFormatter.PrintMessage(Format(
                        "Verifying the Claim Line details in the Appeal Letter for appeal {0}",
                        appealSeqWithAppealLinesOnDifferentClaims));
                    firstAppealLineDetails = appealLetterPage.GetListOfLinesByAppealClaimDetailNumber(1);
                    firstAppealLineDetails.ShouldContain("Line 1, Code J0135", "Proc Details for Line 1");

                    firstAppealLineNotesDetails = appealLetterPage.GetListOfNotesByAppealClaimDetailNumber(1);
                    firstAppealLineNotesDetails.ShouldContain(paidRationaleNote, "Is Rationale Note Present?");
                    firstAppealLineNotesDetails.ShouldContain(paidSummaryNote, "Is Summary Note Present?");

                    secondAppealLineDetails = appealLetterPage.GetListOfLinesByAppealClaimDetailNumber(2);
                    secondAppealLineDetails.ShouldContain("Line 1, Code J0135", "Line 3 Proc Details");
                    secondAppealLineDetails.ShouldContain("Line 1, Code J0135", "Line 3 Proc Details");

                    secondAppealLineNotesDetails = appealLetterPage.GetListOfNotesByAppealClaimDetailNumber(2);
                    secondAppealLineNotesDetails.ShouldContain(adjustedRationaleNote, "");
                    secondAppealLineNotesDetails.ShouldContain(adjustedSummaryNote, "");
                    appealLetterPage.CloseLetterPopUpPageAndBackToAppealSearch();

                    StringFormatter.PrintMessageTitle(
                        "Verifying whether flag details in the preview pane are present in the Appeal Letter");
                    firstAppealLineDetails.CollectionShouldBeSubsetOf(letterPreviewBodyForMultipleClaimLines,
                        "Is Appeal Line  Details Same to the Appeal Action ? ");
                    firstAppealLineNotesDetails.CollectionShouldBeSubsetOf(letterPreviewBodyForMultipleClaimLines,
                        "Is Appeal Line Note Details Same to the Appeal Action?");
                    secondAppealLineDetails.CollectionShouldBeSubsetOf(letterPreviewBodyForMultipleClaimLines,
                        "Is Appeal Line Details Same to the Appeal Action?");
                    secondAppealLineNotesDetails.CollectionShouldBeSubsetOf(letterPreviewBodyForMultipleClaimLines,
                        "Is Appeal Line Note Details Same to the Appeal Action?");
                }

                finally
                {

                }
            }
        }

        [Test, Category("SchemaDependent")] //TE-743
        public void Verify_Claim_Action_Is_Unlocked_For_State_Or_Cotiviti_Consultant_Complete_Appeals()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealSearchPage _appealSearch;
                AppealActionPage _appealAction;

                _appealSearch = automatedBase.QuickLaunch.NavigateToAppealSearch();
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestExtensions.TestName);
                var claimSequence = paramLists["ClaimSequence"].Split(',').ToList();
                ClaimActionPage _claimAction = null;


                try
                {
                    int j = 0;
                    foreach (AppealType appealType in Enum.GetValues(typeof(AppealType)))
                    {
                        StringFormatter.PrintMessage(
                            $"Verify Claim Is Unlocked In Claim Action Popup For Consultant Complete Appeal and {appealType} appeal type");
                        _appealSearch.DeleteAppealLockByClaimSeq(claimSequence[j]);
                        _appealSearch.DeleteClaimLock(claimSequence[j]);
                        _appealSearch.SelectAllAppeals();
                        _appealSearch.SelectClientSmtst();

                        automatedBase.CurrentPage = _appealAction =
                            _appealSearch.SearchByClaimSequenceToGoAppealAction(claimSequence[j], true);
                        _appealSearch = _appealAction.ClickOnSearchIconToNavigateAppealSearchPage();
                        _appealSearch.GetGridViewSection.GetValueInGridByColRow(6, 1)
                            .ShouldBeEqual(appealType.GetStringValue(), "Verify Appeal Type");
                        _appealAction = _appealSearch.ClickOnAppealSequenceToNavigateAppealActionPageByRow(1);

                        _appealAction.GetAppealStatus().ShouldBeEqual("State Consultant Complete",
                            $"Verify Appeal Status for {appealType}");
                        _claimAction = _appealAction.ClickOnClaimSequenceSwitchToClaimAction();
                        _claimAction.IsClaimLocked().ShouldBeFalse("Claim should be unlocked");
                        _appealAction.CloseAnyPopupIfExist();
                        _appealAction.ClickOnSearchIconToNavigateAppealSearchPage();
                        j++;
                    }


                    StringFormatter.PrintMessage("Verify Lock Is Present When Locked By Other User");
                    automatedBase.CurrentPage = automatedBase.QuickLaunch = _appealSearch.Logout().LoginAsHciAdminUser5();
                    automatedBase.CheckTestClientAndSwitch();
                    automatedBase.CurrentPage = _appealSearch = automatedBase.QuickLaunch.NavigateToAppealSearch();
                    _appealSearch.SelectAllAppeals();
                    _appealSearch.SelectClientSmtst();
                    automatedBase.CurrentPage = _appealAction =
                        _appealSearch.SearchByClaimSequenceToGoAppealAction(claimSequence[1], true);
                    StringFormatter.PrintMessage(
                        Format("Open appeal action for claim sequence: {0} with direct URL ", claimSequence[1]));
                    var newAppealActionUrl = automatedBase.CurrentPage.CurrentPageUrl;
                    _appealAction =
                        _appealSearch.SwitchToOpenAppealActionByUrlForAdmin(newAppealActionUrl);

                    _appealAction.IsAppealLock()
                        .ShouldBeTrue("Appeal should be locked when it is in view mode by another user");
                    _appealAction.GetAppealLockToolTip()
                        .ShouldBeEqual(
                            "This appeal is currently locked for editing by Test Automation5",
                            "Is Lock Message Equal?");
                    _claimAction = _appealAction.ClickOnClaimSequenceSwitchToClaimAction();
                    _claimAction.IsClaimLocked().ShouldBeTrue("Claim should be locked");
                    _claimAction.GetLockIConTooltip()
                        .ShouldBeEqual(
                            "This claim has been opened in view mode. It is currently locked by Test Automation5 (ui_automation5).",
                            "Is Lock Message Equal?");

                    _appealAction.CloseAnyPopupIfExist();
                    _appealAction.CloseCurrentWindowAndSwitchToOriginal();
                    _appealAction.CloseAnyTabIfExist();



                    StringFormatter.PrintMessage(
                        "Verify Claim Is Locked In Claim Action Page For Claims With Dental Consultant Review Complete Appeal");
                    var claimSearchPage = automatedBase.CurrentPage.NavigateToClaimSearch();
                    _claimAction =
                        claimSearchPage.SearchByClaimSequenceToNavigateToClaimActionPage(claimSequence[1], true);
                    _claimAction.IsClaimLocked().ShouldBeTrue("Claim Should Be Locked");
                    _claimAction.GetLockIConTooltip()
                        .ShouldBeEqual(
                            "Claim is locked. You cannot modify claims linked to an appeal. Adjustments must be made using Appeal Action.");

                }
                finally
                {
                    automatedBase.CurrentPage =
                        _appealSearch =
                            _appealSearch.Logout().LoginAsHciAdminUser().NavigateToAppealSearch();
                }
            }
        }

        [Test] //TE-866
        public void Verify_Delete_Line_Icon_Is_Enabled_Only_For_Users_Having_Appeal_Manager_Authority()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealSearchPage _appealSearch;
                AppealActionPage _appealAction;

                _appealSearch = automatedBase.QuickLaunch.NavigateToAppealSearch();
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestExtensions.TestName);
                var claimSequence = paramLists["ClaimSequence"];

                StringFormatter.PrintMessage(
                    "Verify Delete Icon Is Enabled For User With Appeal Manager Read/Write Authority");

                _appealSearch.IsRoleAssigned<UserProfileSearchPage>(new List<string> {automatedBase.EnvironmentManager.Username},
                    RoleEnum.OperationsManager.GetStringValue()).ShouldBeTrue(
                    $"Is Appeal Manager present for current user<{automatedBase.EnvironmentManager.Username}>");

                _appealSearch
                    .IsRoleAssigned<AppealSearchPage>(
                        new List<string> {automatedBase.EnvironmentManager.HCIUserWithReadOnlyAccessToAllAuthorites},
                        RoleEnum.OperationsManager.GetStringValue()).ShouldBeFalse(
                        $"Is Appeal Manager Read only present for current user<{automatedBase.EnvironmentManager.HCIUserWithReadOnlyAccessToAllAuthorites}>");


                _appealSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Client",
                    ClientEnum.SMTST.ToString());
                _appealAction =
                    _appealSearch.SearchByClaimSequenceToGoAppealAction(claimSequence);
                _appealAction.IsDeleteAppealLineIconEnabled().ShouldBeTrue("Delete Appeal Line Icon Should Be Enabled");



                StringFormatter.PrintMessage(
                    "Verify Delete Icon Is Disabled For User Without Appeal Manager Authority");
                automatedBase.CurrentPage.Logout().LoginAsHCIUserWithReadOnlyAccessToAllAuthorities();

                automatedBase.CurrentPage.NavigateToAppealSearch();
                _appealSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Client",
                    ClientEnum.SMTST.ToString());
                _appealAction =
                    _appealSearch.SearchByClaimSequenceToGoAppealAction(claimSequence);
                _appealAction.IsDeleteAppealLineIconEnabled()
                    .ShouldBeFalse("Delete Appeal Line Icon Should Be Disabled");
            }
        }

        [Test] //CAR-2891(CAR-2853)
        public void Verify_DCI_Appeals_With_Type_Record_And_Dental_Reviews_Are_Set_To_Closed_When_Completed()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealSearchPage _appealSearch;
                AppealActionPage _appealAction;
                AppealSummaryPage _appealSummary;

                _appealSearch = automatedBase.QuickLaunch.NavigateToAppealSearch();
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestExtensions.TestName);
                var claimSequence = paramLists["ClaimSequence"].Split(',').ToList();
                var reasonCode = paramLists["ReasonCode"];
                var rationale = paramLists["Rationale"];
                var date = paramLists["Date"];
                int j = 0;

                foreach (AppealType appealType in Enum.GetValues(typeof(AppealType)))
                {
                    StringFormatter.PrintMessage(
                        $"Verify behaviour for {appealType.GetStringDisplayValue()} appeal type");
                    _appealSearch.UpdateAppealStatusToNew(claimSequence[j]);
                    _appealSearch.DeleteAppealAuditByClaseq(claimSequence[j], date);
                    _appealSearch.SelectAllAppeals();
                    _appealSearch.SelectClientSmtst();

                    automatedBase.CurrentPage = _appealAction =
                        _appealSearch.SearchByClaimSequenceToGoAppealAction(claimSequence[j], true);
                    _appealSearch = _appealAction.ClickOnSearchIconToNavigateAppealSearchPage();
                    _appealSearch.GetGridViewSection.GetValueInGridByColRow(6, 1)
                        .ShouldBeEqual(appealType.GetStringValue(), "Verify Appeal Type");
                    _appealSearch.GetAppealProductByClaSeqFromDb(claimSequence[j])
                        .ShouldBeEqual(ProductEnum.DCA.GetStringDisplayValue(), "Is Appeal product DCA ?");
                    _appealAction = _appealSearch.ClickOnAppealSequenceToNavigateAppealActionPageByRow(1);

                    _appealAction.CompleteAppeals(handlePopUp: true, reasonCode: reasonCode, rationale: rationale);

                    _appealSearch.GetGridViewSection.GetValueInGridByColRow(11, 1)
                        .ShouldBeEqual(
                            (appealType == AppealType.Appeal)
                                ? AppealStatusEnum.Complete.GetStringDisplayValue()
                                : AppealStatusEnum.Closed.GetStringDisplayValue(),
                            "Appeal status should be closed dental and record reviews appeal types only.");
                    _appealSummary = _appealSearch.ClickOnAppealSequenceByStatusOrAppealToGoAppealSummaryPage(
                        (appealType == AppealType.Appeal)
                            ? AppealStatusEnum.Complete.GetStringDisplayValue()
                            : AppealStatusEnum.Closed.GetStringDisplayValue());
                    var appealProcessingHistory = _appealSummary.ClickAppealProcessingHx();
                    if (appealType == AppealType.Appeal)
                    {
                        appealProcessingHistory.GetAppealAuditGridTableDataValuesByColumn(6).ShouldNotContain(
                            AppealStatusEnum.Closed.GetStringDisplayValue(),
                            $"Appeal Status should not be automatically set to closed and closed audit should not be recorded for {appealType} appeal type");
                        appealProcessingHistory.GetAppealAuditGridTableDataValue(1, 6).ShouldBeEqual(
                            AppealStatusEnum.Complete.GetStringDisplayValue(),
                            "Latest Appeal Status Should Be Complete.");
                    }
                    else
                    {
                        appealProcessingHistory.GetAppealAuditGridTableDataValuesByColumn(6)
                            .CollectionShouldBeSupersetOf(
                                new List<string>()
                                {
                                    AppealStatusEnum.Closed.GetStringDisplayValue(),
                                    AppealStatusEnum.Complete.GetStringDisplayValue()
                                },
                                $"Appeal Record for both Complete and closed should  be recorded for {appealType} appeal type");
                        appealProcessingHistory.GetAppealAuditGridTableDataValue(1, 6).ShouldBeEqual(
                            AppealStatusEnum.Closed.GetStringDisplayValue(), "Latest Appeal Status Should Be Closed.");
                    }

                    _appealSummary.CloseAnyPopupIfExist();
                    _appealSummary.ClickOnSearchIconToNavigateAppealSearchPage();
                    j++;
                }
            }
        }

        [Test,Category("OnDemand")] //CAR-3199(CAR-3142)
        public void Verify_Appeal_Status_And_Type_Real_Time_Claims_Queue()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealSearchPage _appealSearch;
                AppealActionPage _appealAction;
                
                _appealSearch = automatedBase.QuickLaunch.NavigateToAppealSearch();
                 var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestName);
                var claimSeq = paramLists["ClaimSeq"];
                var appealSeq = paramLists["AppealSeq"];
                var appealType = paramLists["Type"];
                var reasonCode = "RRD - Record Review Deny";
                _appealSearch.DeleteAppealsFromRealTimeQueue(claimSeq.Split(',')[0]);
                _appealSearch.DeleteAppealsFromRealTimeQueue(claimSeq.Split(',')[1]);
                _appealSearch.UpdateAppealStatusToIncomplete(appealSeq);
                var processingType = new List<string> { "PB","PR" };
                var AppealType = new List<string> {"A", "R", "M", "D"};
                try
                {
                    for (int i = 0; i <processingType.Count; i++)
                    {
                        StringFormatter.PrintMessage($"Updating processing type of SMTST to {processingType}");
                        var random = new Random();
                        var type = AppealType[random.Next(AppealType.Count)];
                        _appealSearch.GetCommonSql.UpdateProcessingTypeOfClient(processingType[i], ClientEnum.SMTST.ToString());
                        _appealSearch.UpdateAppealType(type, appealSeq.Split(',')[i]);
                        _appealAction =
                            _appealSearch.SearchByAppealSequenceNavigateToAppealAction(appealSeq.Split(',')[i]);

                        StringFormatter.PrintMessage("Setting appeal to status of complete");
                        _appealAction.CompleteAppeals(appealSeq, reasonCode, "rationale", handlePopUp: true);
                        _appealAction.WaitForStaticTime(5000);
                        StringFormatter.PrintMessage("Verifying the entries in realtime_claim_queue_pca table");
                        var appealInfo = _appealAction.GetStatusAndTypeFromRealTimeQueue(claimSeq.Split(',')[i]);
                        appealInfo[0].ShouldBeEqual(type);
                        appealInfo[1].ShouldBeEqual(AppealStatusEnum.Complete.GetStringValue());
                    }
                }
                finally
                {
                    StringFormatter.PrintMessage("Resetting processing type of SMTST to Batch");
                    _appealSearch.GetCommonSql.UpdateProcessingTypeOfClient("B", ClientEnum.SMTST.ToString());
                }
            }
        }

        #endregion
    }
}
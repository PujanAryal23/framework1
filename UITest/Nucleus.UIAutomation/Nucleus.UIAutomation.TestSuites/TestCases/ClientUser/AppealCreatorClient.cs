using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Nucleus.Service.Data;
using Nucleus.Service.PageServices.Appeal;
using Nucleus.Service.Support.Common.Constants;
using Nucleus.UIAutomation.TestSuites.Base;
using NUnit.Framework;
using Nucleus.Service.PageServices.Claim;
using System.Text.RegularExpressions;
using Nucleus.Service.PageServices.QuickLaunch;
using Nucleus.Service.PageServices.Settings;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Environment;
using UIAutomation.Framework.Utils;

namespace Nucleus.UIAutomation.TestSuites.TestCases.ClientUser
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class AppealCreatorClient
    {
        //#region PRIVATE FIELDS

        //private AppealCreatorPage _appealCreator;
        //private AppealSummaryPage _appealSummary;
        //private ClaimActionPage _claimAction;
        //private AppealSearchPage _appealSearch;
        //private AppealActionPage _appealAction;
        //private AppealProcessingHistoryPage _appealProcessingHx;
        //private AppealManagerPage _appealManager;
        //private QuickLaunchPage _quickLaunch;
        //private ClientSearchPage _clientSearch;
        //#endregion

        //#region OVERRIDE METHODS
        //protected override void ClassInit()
        //{
        //    try
        //    {
        //        base.ClassInit();
        //        automatedBase.CurrentPage = _appealCreator = QuickLaunch.NavigateToAppealCreator();

        //    }
        //    catch (Exception)
        //    {
        //        if (StartFlow != null)
        //            StartFlow.Dispose();
        //        throw;
        //    }
        //}


        //protected override void ClassCleanUp()
        //{
        //    try
        //    {
        //        //if (automatedBase.CurrentPage.IsQuickLaunchIconPresent())
        //        //    _appealCreator.GoToQuickLaunch();
        //    }

        //    finally
        //    {
        //        base.ClassCleanUp();
        //    }

        //}

        //protected override void TestInit()
        //{
        //    base.TestInit();
        //    automatedBase.CurrentPage = _appealCreator;
        //}

        //protected override void TestCleanUp()
        //{
        //    base.TestCleanUp();
        //    if (string.Compare(UserType.CurrentUserType, UserType.CLIENT, StringComparison.OrdinalIgnoreCase) != 0)
        //    {
        //        _appealCreator = _appealCreator.Logout().LoginAsClientUser().NavigateToAppealCreator();

        //    }

        //    if (automatedBase.CurrentPage.GetPageHeader() != PageHeaderEnum.ClaimSearch.GetStringValue())
        //    {
        //        automatedBase.CurrentPage = _appealCreator = QuickLaunch.NavigateToAppealCreator();
        //    }
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

        [Test] //CAR-3120(CAR-3064)
        public void Verify_creation_of_MRR_Appeal_type_client()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                AppealCreatorPage _appealCreator;
                ClaimSearchPage _claimSearch;
                ClaimActionPage _claimAction;
                AppealManagerPage _appealManager;
                AppealCategoryManagerPage _appealCategoryManager;

                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> testData = automatedBase.DataHelper.GetTestData(GetType().FullName,
                    TestName);
                var claSeq = testData["ClaimSequences"].Split(',').ToList();
                var appealType = "Medical Record Review";
                var categoryId = testData["CategoryId"];
                var proc = testData["Proc"];
                var analyst = testData["Analyst"];
                var userId = testData["UserId"];
                Random rnd = new Random();
                var products = new List<string>()
                {
                    "Coding Validation",
                    "FacilityClaim Insight",
                    "FraudFinder Pro"
                };
                var product = products[rnd.Next(products.Count)];
                string[] newAppealData =
                {
                    categoryId, proc, proc, "", product, "", "", analyst,
                    "For MRR appeal creation", ""
                };
                try
                {
                    StringFormatter.PrintMessage("Setting 'enable_medical_record_reviews' to T");
                    if (automatedBase.CurrentPage.GetCommonSql.GetSpecificClientDataFromDb("enable_medical_record_reviews",
                        ClientEnum.SMTST.ToString()) == "F")
                    {
                        automatedBase.CurrentPage.GetCommonSql.UpdateSpecificClientDataInDB("enable_medical_record_reviews='T'",
                            ClientEnum.SMTST.ToString());
                        automatedBase.CurrentPage.RefreshPage(false);
                    }

                    StringFormatter.PrintMessage("Creating Appeal Category");
                    _appealCategoryManager =
                        automatedBase.CurrentPage.Logout().LoginAsHciAdminUser().NavigateToAppealCategoryManager();
                    _appealCategoryManager.CreateAppealCategory(newAppealData, categoryOrder: false);

                    StringFormatter.PrintMessageTitle($"Navigating to Claim Action page via {claSeq}");
                    _claimSearch = automatedBase.CurrentPage.Logout().LoginAsClientUser().NavigateToClaimSearch();

                    StringFormatter.PrintMessage("Deleting Analyst's PTO from Db");
                    _claimSearch.GetCommonSql.DeletePTOByUserIdFromDb(userId);

                    _claimAction = _claimSearch.SearchByClaimSequenceToNavigateToClaimActionPage(claSeq[0]);

                    StringFormatter.PrintMessage($"Navigating to Appeal Creator page via {claSeq} claim sequence");
                    _appealCreator = _claimAction.ClickOnCreateAppealIcon();
                    var expectedDueDate = _appealCreator.CalculateAndGetAppealDueDateFromDatabase("0");

                    StringFormatter.PrintMessage(
                        "Verifying if MRR appeal type is present when enable medical record reviews is T");

                    _appealCreator.IsMedicalRecordReviewButtonPresent().ShouldBeTrue(
                        "For Enable Medical Record Review = T, Medical record review button should be present");
                    _appealCreator.GetMedicalRecordReviewToolTip().ShouldBeEqual("Medical Record Review",
                        "Tooltip should match for MRR appeal type");
                    _appealCreator.ClickMedicalRecordReviewButton();
                    _appealCreator.GetCreateAppealColumnHeaderText().ShouldBeEqual("Create Medical Record Review",
                        "Form header of Medical Record Review Appeal type should match");
                    _appealCreator.IsUrgentCheckboxDisabled()
                        .ShouldBeTrue("Urgent checkbox should be disabled when appeal type 'M' is selected ");

                    StringFormatter.PrintMessage("Verifying creation of MRR appeal");
                    _appealCreator.SelectClaimLine();
                    _appealCreator.CreateAppeal(product, "DocID", "", "M");

                    StringFormatter.PrintMessage("Verification after creating MRR appeal");
                    _claimAction.IsAddAppealIconDisabled().ShouldBeTrue(
                        "Add appeal icon should be disabled after Appeal has been created");
                    VerifyAppealDataAppealAfterSave(_claimAction, product, appealType);

                    StringFormatter.PrintMessage(
                        "Verifying if MRR appeal type is present when enable medical record reviews is F");
                    _appealCreator.GetCommonSql.UpdateSpecificClientDataInDB("enable_medical_record_reviews='F'",
                        ClientEnum.SMTST.ToString());
                    automatedBase.CurrentPage.NavigateToAppealCreator().SearchByClaimSequence(claSeq[1]);
                    _appealCreator.RefreshPage(false);
                    _appealCreator.GetCommonSql.GetSpecificClientDataFromDb("enable_medical_record_reviews",
                        ClientEnum.SMTST.ToString()).ShouldBeEqual("F", "Enable Medical Record Review should be F");
                    _appealCreator.IsMedicalRecordReviewButtonPresent()
                        .ShouldBeFalse("Is Medical Record Review Button present?");

                    StringFormatter.PrintMessage("Logging in as HCI user for verifying analyst and categoryId");
                    _appealManager =
                        _appealCreator.Logout().LoginAsHciAdminUser().NavigateToAppealManager();
                    _appealManager.SearchByClaimSequence(claSeq[0]);
                    _appealManager.GetGridViewSection.GetValueInGridByColRow(7)
                        .ShouldBeEqual(categoryId, $"Category id should be {categoryId}");
                    _appealManager.GetGridViewSection.GetValueInGridByColRow(8)
                        .ShouldBeEqual(analyst, $"Analyst should be {analyst}");
                    _appealManager.GetGridViewSection.GetValueInGridByColRow(12)
                        .ShouldBeEqual(expectedDueDate, $"Expected date due date should be {expectedDueDate}");
                }

                finally
                {
                    StringFormatter.PrintMessage("Clearing up the appeal created");
                    if (!UserType.CurrentUserType.Equals(UserType.HCIADMIN))
                    {
                        automatedBase.CurrentPage.Logout().LoginAsHciAdminUser().NavigateToAppealManager();
                    }

                    _appealManager = automatedBase.CurrentPage.NavigateToAppealManager();
                    _appealManager.SearchByClaimSequence(claSeq[0]);
                    _appealManager.DeleteAppealsAssociatedWithClaim(claSeq);

                    StringFormatter.PrintMessage("Enabling Medical Record Type for SMTST");
                    automatedBase.CurrentPage.GetCommonSql.UpdateSpecificClientDataInDB(
                        "enable_medical_record_reviews='T'", ClientEnum.SMTST.ToString());

                    StringFormatter.PrintMessage("Deleting the Appeal Category created");
                    _appealCategoryManager = _appealManager.NavigateToAppealCategoryManager();
                    if (!_appealCategoryManager.GetSideBarPanelSearch.IsSideBarPanelOpen())
                        _appealCategoryManager.GetSideBarPanelSearch.OpenSidebarPanel();
                    if (_appealCategoryManager.GetSideBarPanelSearch.GetAvailableDropDownList("Category ID").Contains(categoryId))
                    {
                        _appealCategoryManager.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Category ID", categoryId);
                        _appealCategoryManager.GetSideBarPanelSearch.ClickOnFindButton();
                        _appealCategoryManager.WaitForWorking();
                        _appealCategoryManager.GetGridViewSection.ClickEditDeleteIconByValueAndIcon(categoryId, "Delete Record");
                        _appealCategoryManager.ClickOkCancelOnConfirmationModal(true);
                    }
                }
            }
        }


        [Test] //US41372
        public void Verify_appeal_creator_claim_search_check_for_menu_option()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                AppealCreatorPage _appealCreator;
                _appealCreator = automatedBase.CurrentPage.NavigateToAppealCreator();
                automatedBase.CurrentPage.Logout().LoginAsClientUserWithNoAnyAuthority();
                _appealCreator.IsAppealMenuPresent()
                    .ShouldBeFalse("Appeal Creator not visible to client user with no authority of manage appeals");
                _appealCreator.IsAppealCreatorQuicklaunchTilePresent()
                    .ShouldBeFalse("Appeal Creator Tile is absent to client user with no authority of manage appeals");
                _appealCreator.Logout().LoginAsClientUserWithReadOnlyAccessToAllAuthorities();
                _appealCreator.IsAppealMenuPresent()
                    .ShouldBeTrue(
                        "Appeal Creator should visible to client user with read only authority of manage appeals");
            }
        }

        [Test, Category("AppealDependent")] //US41372   
        public void Verify_appeal_creator_claim_search_for_client_user()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                AppealCreatorPage _appealCreator;
                _appealCreator = automatedBase.CurrentPage.NavigateToAppealCreator();
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestExtensions.TestName);
                var lockedAppealCs = paramLists["LockedAppealCS"];
                var lockedClaimMessage = paramLists["LockedClaimMessage"];
                _appealCreator.SearchByClaimSequence(lockedAppealCs);
                _appealCreator.GetClaimActionLockToolTip()
                    .ShouldBeEqual(lockedClaimMessage,
                        "Checking Claim Lock message for locked claim."); //verify lock message for appeal not comlete or closed
            }
        }

        [Test, Category("AppealDependent")] //US41372
        public void Verify_client_user_can_add_appeal_to_closed_completed_appeals()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                AppealCreatorPage _appealCreator;
                _appealCreator = automatedBase.CurrentPage.NavigateToAppealCreator();
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var claimSeq = paramLists["ClaimSeq"];
                _appealCreator.SearchByClaimSequence(claimSeq);
                _appealCreator.GetPageHeader()
                    .ShouldBeEqual(PageHeaderEnum.AppealCreator.GetStringValue(),
                        "Page Should Navigate to Appeal creator  for creating new appeal page client user with appeals closed /completed.");
                _appealCreator.ClickOnCancelBtn();
            }
        }


        [Test] //US50646
        public void Verify_presence_of_sub_status_of_pended_claim()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                AppealCreatorPage _appealCreator;
                _appealCreator = automatedBase.CurrentPage.NavigateToAppealCreator();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var claimSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                    "ClaimSequence", "Value");

                automatedBase.CurrentPage = _appealCreator.SearchByClaimSequenceForLockedClaim(claimSeq);
                _appealCreator.GetSearchlistComponentItemValue(1, 7)
                    .ShouldBeEqual("Pending Agreement", "Sub Status Should present for pended claims");
            }
        }


        [Test, Category("AppealDependent")] //US50615
        public void Verify_tooltip_with_appeal_creator_icon_is_enabled_or_disabled_for_different_claim_status()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                AppealCreatorPage _appealCreator;
                ClaimActionPage _claimAction;
                _appealCreator = automatedBase.CurrentPage.NavigateToAppealCreator();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> testData = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestExtensions.TestName);
                var claimSeq = testData["ClaimSeq"].Split(',');
                var claimNo = testData["ClaimNo"];
                var lockTooltip = testData["ToolTipForClientUnreleasedClient"];

                try
                {

                    _appealCreator.SearchByClaimNoAndStayOnSearch(claimNo);

                    StringFormatter.PrintMessageTitle(
                        "Verifying that appeal creator is enabled for Client Unreviewed claims");
                    _appealCreator.GetSearchlistComponentItemValueByClaimseq(claimSeq[0], 7)
                        .ShouldBeEqual("Client Unreviewed", "Status of claim is client unreviewed.");
                    _appealCreator.IsAppealCreateIconDisabledlookByClaimSeq(claimSeq[0])
                        .ShouldBeFalse("Ceate Appeal Icon is enabled?");
                    Console.WriteLine(
                        "Verifying that all previous appeals has status closed or complete for Client Unreviewed");
                    CheckClaimSequenceStatusOfAppeal(claimSeq[0], true);

                    StringFormatter.PrintMessageTitle(
                        "Verifying that appeal creator is enabled for Client Reviewed claims");
                    _appealCreator.GetSearchlistComponentItemValueByClaimseq(claimSeq[1], 7)
                        .ShouldBeEqual("Client Reviewed", "Status of claim is client reviewed.");
                    _appealCreator.IsAppealCreateIconDisabledlookByClaimSeq(claimSeq[0])
                        .ShouldBeFalse("Ceate Appeal Icon is enabled?");
                    Console.WriteLine("Verifying that all previous appeals has status closed or complete");
                    CheckClaimSequenceStatusOfAppeal(claimSeq[1], true);

                    StringFormatter.PrintMessageTitle(
                        "Verifying that appeal creator is disabled for Client Reviewed claims with previous appeals not closed or complete");
                    _appealCreator.SearchByClaimSequenceForLockedClaim(claimSeq[2]);
                    _appealCreator.IsAppealCreateIconDisabledlookByClaimSeq(claimSeq[2])
                        .ShouldBeTrue("Ceate Appeal Icon is disabled?");
                    _appealCreator.GetAppealDisabledToolTipValue(claimSeq[2])
                        .ShouldBeEqual(testData["ToolTipForClaimWithAppealProcess"],
                            "Tooltip of Create Appeal Icon for Claim with having appeals not closed or completed");
                    Console.WriteLine("Verifying that all previous appeals are not in status of closed or complete");
                    CheckClaimSequenceStatusOfAppeal(claimSeq[2], false);

                    StringFormatter.PrintMessageTitle(
                        "Verifying that appeal creator is disabled for Client Unreviewed claims with previous appeals not closed or complete");
                    _appealCreator.SearchByClaimSequenceForLockedClaim(claimSeq[3]);
                    _appealCreator.IsAppealCreateIconDisabledlookByClaimSeq(claimSeq[3])
                        .ShouldBeTrue("Ceate Appeal Icon is disabled?");
                    _appealCreator.GetAppealDisabledToolTipValue(claimSeq[3])
                        .ShouldBeEqual(testData["ToolTipForClaimWithAppealProcess"],
                            "Tooltip of Create Appeal Icon for Claim with having appeals not closed or completed");
                    Console.WriteLine("Verifying that all previous appeals has status closed or complete");
                    CheckClaimSequenceStatusOfAppeal(claimSeq[3], false);

                    StringFormatter.PrintMessageTitle(
                        "Verifying that appeal creator is disabled for those claims that are not Client Reviewed or Client Unreviewed");
                    _appealCreator.SearchByClaimSequenceForLockedClaim(claimSeq[4]);
                    _appealCreator.IsAppealCreateIconDisabledlookByClaimSeq(claimSeq[4])
                        .ShouldBeTrue("Ceate Appeal Icon is disabled?");
                    Regex regex = new Regex("[ ]{2,}", RegexOptions.None);
                    var lockTooltipValue =
                        regex.Replace(_appealCreator.GetAppealDisabledToolTipValue(claimSeq[4]), " ");
                    lockTooltipValue.ShouldBeEqual(lockTooltip, "Tooltip message of create appeal icon");
                }

                finally
                {
                    if (_appealCreator.IsPageErrorPopupModalPresent()) _appealCreator.ClosePageError();
                    _appealCreator.CloseAnyPopupIfExist();
                    if (!automatedBase.CurrentPage.Equals(typeof(AppealCreatorPage)))
                    {
                        automatedBase.CurrentPage.NavigateToAppealCreator();
                    }
                }

                void CheckClaimSequenceStatusOfAppeal(string claSeq, bool checkTrueFalse)
                {
                    _claimAction = _appealCreator.ClickOnClaimSequenceOfAppealCreatorSearchByClaimSeq(claSeq);
                    _claimAction.ClickOnViewAppealIcon();
                    var countOfCloseCompletedAppeals = 0;
                    var appealList = _claimAction.GetListOfAppealSequenceInRows();
                    foreach (var appealSeq in appealList)
                    {
                        var status = _claimAction.GetAppealStatus(appealSeq);
                        if (checkTrueFalse)
                            (status.Equals("Closed") || status.Equals("Complete")).ShouldBeTrue("All previous appeals are in status of closed or complete");
                        else if (status.Equals("Closed") || status.Equals("Complete")) countOfCloseCompletedAppeals++;
                        //(status.Equals("Closed") || status.Equals("Complete")).ShouldBeFalse(
                        //    "All previous appeals are not in status of closed or complete");
                    }
                    countOfCloseCompletedAppeals.ShouldBeLess(appealList.Count, "All Previous appeals are not closed or completed.");
                    _appealCreator.CloseAnyPopupIfExist();
                }
            }
        }

        [Test] //US50616
        public void Verify_status_of_claim_when_creating_RR_or_Appeal_type_appeals()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                AppealCreatorPage _appealCreator;
                ClaimActionPage _claimAction;

                _appealCreator = automatedBase.CurrentPage.NavigateToAppealCreator();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> testData = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestExtensions.TestName);
                var claimSeq = testData["ClaimSeq"].Split(',');
                var disabledTooltip = testData["ToolTipForCUrDisabledAppealType"];
                _appealCreator.SearchByClaimSequence(claimSeq[0]);
                try
                {
                    _claimAction = _appealCreator.ClickOnClaimSequenceAndSwitchWindow();
                    _claimAction.GetClaimStatus().ShouldBeEqual("Client Unreviewed",
                        "Status of claim is as required: client unreviewed.");
                    _appealCreator.CloseAnyPopupIfExist();
                    _appealCreator.IsRespectiveAppealTypeSelected("R")
                        .ShouldBeTrue("Record Review selected by default for Client unreviewed claims");
                    _appealCreator.IsAppealTypeDisabled("A")
                        .ShouldBeTrue("Appeal typpe as Appeal is disabled for client unreviewed claims");
                    _appealCreator.GetToolTipForDisabledAppealType("A")
                        .ShouldBeEqual(disabledTooltip,
                            "Tool tip message for client unreviewed claims record type selection");
                    _appealCreator.ClickOnCancelBtn();
                    if (_appealCreator.IsPageErrorPopupModalPresent())
                        _appealCreator.ClickOnOkButtonOnConfirmationModal();
                    _appealCreator.SearchByClaimSequence(claimSeq[1]);
                    _claimAction = _appealCreator.ClickOnClaimSequenceAndSwitchWindow();
                    _claimAction.GetClaimStatus().ShouldBeEqual("Client Reviewed",
                        "Status of claim is as required: client Reviewed.");
                    _appealCreator.CloseAnyPopupIfExist();
                    _appealCreator.IsRespectiveAppealTypeSelected("A")
                        .ShouldBeTrue("Appeal type record selected by default for Client reviewed claims");
                    _appealCreator.IsAppealTypeDisabled("A")
                        .ShouldBeFalse("Appeal type as Appeal should not be disabled for client reviewed claims");
                    _appealCreator.SelectAppealRecordType();
                    _appealCreator.IsRespectiveAppealTypeSelected("A")
                        .ShouldBeTrue("Appeal record type should be selectable for Client reviewed claims");
                    _appealCreator.SelectRecordReviewRecordType();
                    _appealCreator.IsRespectiveAppealTypeSelected("R")
                        .ShouldBeTrue("Record Review type should be selectable for Client reviewed claims");
                    _appealCreator.ClickOnCancelBtn();
                    if (_appealCreator.IsPageErrorPopupModalPresent())
                        _appealCreator.ClickOnOkButtonOnConfirmationModal();
                }

                finally
                {
                    if (_appealCreator.IsPageErrorPopupModalPresent()) _appealCreator.ClosePageError();
                    _appealCreator.CloseAnyPopupIfExist();
                    if (!automatedBase.CurrentPage.Equals(typeof(AppealCreatorPage)))
                    {
                        automatedBase.CurrentPage.NavigateToAppealCreator();
                    }
                }
            }
        }

        [Test,Category("Upload_Document")]//US50641
        public void Verify_for_appeal_summary_pop_up_on_search_document_delete_will_be_enabled_or_disabled_based_on_appeal_status()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                AppealCreatorPage _appealCreator;
                AppealSummaryPage _appealSummary;
                AppealProcessingHistoryPage _appealProcessingHx;
                AppealActionPage _appealAction;
                AppealSearchPage _appealSearch;
                AppealManagerPage _appealManager;

                _appealCreator = automatedBase.CurrentPage.NavigateToAppealCreator();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> testData = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestExtensions.TestName);
                var claimSeq = testData["ClaimSequence"];
                var fileType = testData["FileType"];
                var fileName = testData["FileName"];
                _appealCreator.SearchByClaimSequence(claimSeq);
                var hasAppeal = false;
                if (_appealCreator.GetPageHeader() == "Appeal Creator")
                {
                    hasAppeal = _appealCreator.IsAppealLevelPresent(1);
                    _appealCreator.ClickOnCancelBtn();
                }


                if (_appealCreator.IsClaimActionLockToolTipPresent(1)
                ) //check for appeals with status close/compelte too along with new appeals
                {
                    //also check if appeal previously created on this appeal hasnt been deleted due to test fail or other reason
                    DeletePreviousAppeals(claimSeq);
                    _appealCreator.SearchByClaimSequence(claimSeq);
                }
                else
                    _appealCreator.ClickOnCreateAppealIcon(1);

                _appealCreator.ClickOnClaimLine(1);
                _appealCreator.SelectProduct("Coding Validation");
                _appealCreator.SelectAppealRecordType();
                _appealCreator.AddFileForUpload(fileType, 1, fileName);
                var appealRecentlyCreated = "";
                //_appealCreator.AddFileForUpload(testData["FileType2"], 2, testData["FileName2"]);
                try
                {
                    _appealCreator.ClickOnSaveBtn();
                    _appealCreator.WaitForWorking();
                    _appealCreator.GetPageHeader()
                        .ShouldBeEqual("Claim Search", "Back to search page of appeal creator");
                    appealRecentlyCreated = _appealCreator.GetRecentlyCreatedAppealSequence();
                    automatedBase.CurrentPage = _appealSummary =
                        _appealCreator.ClickOnAppealToOpenAppealSummaryAndSwitch(appealRecentlyCreated);

                    var status = _appealSummary.GetStatusValue();
                    _appealSummary.UploadDocumentFromAppealSummary(testData["FileType2"], testData["FileName2"],
                        "new file test doc", 1);
                    _appealSummary.GetStatusValue()
                        .ShouldBeEqual("New", "Status Should be same after delete button click");
                    _appealSummary.ClickOnDeleteFileBtn(1);

                    if (_appealSummary.IsPageErrorPopupModalPresent())
                        _appealSummary.GetPageErrorMessage()
                            .ShouldBeEqual("The selected document will be deleted. Do you wish to proceed?");
                    _appealSummary.ClickOkCancelOnConfirmationModal(false);
                    _appealSummary.IsAppealDocumentDivPresent()
                        .ShouldBeTrue("All the two documents are still entact and listed.");
                    _appealSummary.ClickOnDeleteFileBtn(1);
                    _appealSummary.ClickOkCancelOnConfirmationModal(true);
                    _appealSummary.GetStatusValue()
                        .ShouldBeEqual(status, "Status Should be same after delete button click");
                    _appealSummary.ClickMoreOption();

                    _appealProcessingHx = _appealSummary.ClickAppealProcessingHx();
                    _appealProcessingHx.GetAppealAuditGridTableDataValue(1, 3)
                        .ShouldBeEqual("DeleteDoc", "Action Should be Delete"); //Status
                    // _appealProcessingHx.CloseAppealProcessingHistoryPageToAppealSummaryPage();
                    _appealCreator.CloseAllExistingPopupIfExist();

                    automatedBase.CurrentPage =
                        _appealSearch = _appealCreator.Logout().LoginAsHciAdminUser().NavigateToAppealSearch();
                    _appealSearch.SelectAllAppeals();
                    _appealSearch.SelectClientSmtst();
                    automatedBase.CurrentPage =
                        _appealAction = _appealSearch.SearchByClaimSequenceToGoAppealAction(claimSeq, true);
                    _appealAction.CompleteAppeals(claimSeq, testData["ReasonCode"], "test deny ");
                    automatedBase.CurrentPage =
                        _appealCreator = _appealSearch.Logout().LoginAsClientUser().NavigateToAppealCreator();
                    automatedBase.CurrentPage = _appealSummary =
                        _appealCreator.ClickOnAppealToOpenAppealSummaryAndSwitch(appealRecentlyCreated);
                    _appealSummary.GetStatusValue()
                        .ShouldBeEqual("Complete", "Status Should be same after cancel button click");
                    _appealSummary.IsAppealDocumentDeleteDisabled()
                        .ShouldBeTrue("For closed complete appeals delete file should be disabled.");
                    _appealSummary.IsAppealDocumentUploadDisabled()
                        .ShouldBeTrue("For closed complete appeals upload file should be disabled.");

                    Console.WriteLine("Next step is to closed appeals");

                    _appealSearch = _appealSummary.ClickOnApproveIconToNavigateAppealSearchPage();
                    _appealSummary = _appealSearch.ReturnToPreviouslyViewedAppeal();
                    _appealCreator.SwitchToPopUpWindow();
                    _appealSummary.GetStatusValue().ShouldBeEqual("Closed",
                        "Confirming that we are checking for appeals with closed status");
                    _appealSummary.IsAppealDocumentDeleteDisabled()
                        .ShouldBeTrue("For closed complete appeals delete file should be disabled.");
                    _appealSummary.IsAppealDocumentUploadDisabled()
                        .ShouldBeTrue("For closed complete appeals upload file should be disabled.");
                    _appealCreator.CloseAnyPopupIfExist();
                    automatedBase.CurrentPage = _appealCreator;
                }
                finally
                {
                    if (_appealCreator.IsPageErrorPopupModalPresent()) _appealCreator.ClosePageError();
                    _appealCreator.CloseAnyPopupIfExist();
                    if (_appealCreator.GetPageHeader() != "Claim Search")
                        automatedBase.CurrentPage.NavigateToAppealCreator();
                    _appealCreator.SearchByClaimSequenceForLockedClaim(claimSeq);
                    if (_appealCreator.GetPageHeader() == "Appeal Creator")
                    {
                        hasAppeal = _appealCreator.IsAppealLevelPresent(1);
                        _appealCreator.ClickOnCancelBtn();
                    }

                    if (_appealCreator.IsClaimActionLockToolTipPresent(1) || hasAppeal)
                        DeletePreviousAppeals(claimSeq);

                    if (string.Compare(UserType.CurrentUserType, UserType.CLIENT, StringComparison.OrdinalIgnoreCase) !=
                        0)
                        _appealCreator.Logout().LoginAsClientUser().NavigateToAppealCreator();

                }

                void DeletePreviousAppeals(string claSeq)
                {
                    automatedBase.CurrentPage =
                        _appealManager = _appealCreator.Logout().LoginAsHciAdminUser().NavigateToAppealManager();
                    _appealManager.DeleteAppealsAssociatedWithClaim(claSeq);
                    automatedBase.CurrentPage =
                        _appealCreator = _appealCreator.Logout().LoginAsClientUser().NavigateToAppealCreator();
                }
            }

        }


        [Test] //US69376
        public void Validate_note_character_limit_in_appeal_creator_client()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                AppealCreatorPage _appealCreator;
                _appealCreator = automatedBase.CurrentPage.NavigateToAppealCreator();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var claimSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                    "ClaimSequence", "Value");
                _appealCreator.SearchByClaimSequence(claimSeq);
                _appealCreator.GetPageHeader().ShouldBeEqual("Appeal Creator", "Appeal creator page opened for SMTST.");
                var notes = new string('a', 4494);
                _appealCreator.SetLengthyNote("note", notes);
                _appealCreator.GetNote().Length.ShouldBeEqual(4493,
                    "Character limit should be 4494 or less, where 7 characters are separated for <p></p> tag.");
                _appealCreator.ClickOnCancelBtn();
            }
        }


        [Test] //CAR-766
        public void Verify_full_facility_name_displays_on_appeal_creator()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                AppealCreatorPage _appealCreator;
                _appealCreator = automatedBase.CurrentPage.NavigateToAppealCreator();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var claimSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                    "ClaimSequence", "Value");
                _appealCreator.SearchByClaimSequence(claimSeq);
                var claimNo = _appealCreator.GetClaimNoValue();
                var prvName =
                    _appealCreator.GetCommonSql.GetFullPrvNameForGivenClaimNo(
                        claimNo);
                _appealCreator.GetProviderNameValue().ShouldBeEqual(prvName,
                    "Full Facility Name/ Provider name for given claim seq is present and should be equal to " +
                    prvName);
                _appealCreator.ClickOnCancelBtn();
                _appealCreator.SearchByClaimNoAndStayOnSearch(claimNo);
                _appealCreator.GetGridViewSection.GetValueInGridByColRow(col: 4).ShouldBeEqual(prvName,
                    "Full Facility Name/ Provider name for given claim seq is present and should be equal to " +
                    prvName);
            }
        }

        [Test] //CAR-830 + CAR-2898(CAR-2850)
        [NonParallelizable]
        public void Verify_appeals_with_DCI_product()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                AppealManagerPage _appealManager;
                ClientSearchPage _clientSearch;
                AppealCreatorPage _appealCreator;
                AppealSearchPage _appealSearch;
                AppealSummaryPage _appealSummary;
                AppealActionPage _appealAction;

                _appealCreator = automatedBase.CurrentPage.NavigateToAppealCreator();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var claseq = paramLists["ClaimSequence"].Split(',').ToList();
                var expectedCategoryId = "";
                var expectedAnalyst = new List<string>();

                var AppealDueDateCalculationType = new List<string> { "Business", "Calendar" };

                _appealManager = _appealCreator.Logout().LoginAsHciAdminUser().NavigateToAppealManager();

                _appealCreator.DeleteAppealsOnClaim(claseq[0].Split('-')[0]);
                _appealCreator.DeleteAppealsOnClaim(claseq[1].Split('-')[0]);
                _appealCreator.DeleteAppealsOnClaim(claseq[2].Split('-')[0]);

                var user = UserType.CurrentUserType;
                var _appealCategoryManager = _appealManager.NavigateToAppealCategoryManager();
                _appealCategoryManager.CreateAppealCategoryForMultipleAnalystForDci(
                    new List<string>() { paramLists["Analyst"] });
                _appealCategoryManager.ClickOnAppealRowCategoryByProductProcCodeTrigCodePresent("DCA", "NA", "NA");
                expectedCategoryId = _appealCategoryManager.GetCategoryIdByProductProcCodeTrigCodePresent("DCA", "NA", "NA");
                expectedAnalyst = _appealCategoryManager.GetAnalystListOnAnalystAssignment();

                try
                {
                    foreach (var type in AppealDueDateCalculationType)
                    {
                        var turnAroundTime = new List<string>();
                        var expectedDueDate = new List<string>();
                        if (UserType.CurrentUserType != user)
                            automatedBase.CurrentPage.Logout().LoginAsHciAdminUser();
                        _clientSearch = automatedBase.CurrentPage.NavigateToClientSearch();
                        _clientSearch.SearchByClientCodeToNavigateToClientProfileViewPage(ClientEnum.SMTST.ToString(), ClientSettingsTabEnum.Product.GetStringValue());
                        _clientSearch.SelectCalculationType(type);
                        _clientSearch.GetSideWindow.Save();
                        StringFormatter.PrintMessage($"updated Appeal calculation type to {type}");
                        _clientSearch.GetAppealDayTypeFromDB()
                            .ShouldBeEqual(type == "Business" ? "B" : "C", "Appeal Type Equal?");

                        turnAroundTime = _clientSearch.GetAppealDueDatesInputFieldsByLabel(ProductEnum.DCA.ToString());

                        foreach (var time in turnAroundTime)
                        {
                            expectedDueDate.Add(_appealCreator.CalculateAndGetAppealDueDateFromDatabase(time, type));
                        }
                        _appealCreator = _clientSearch.Logout().LoginAsClientUser().NavigateToAppealCreator();

                        StringFormatter.PrintMessage("Create and verify appeal for DCA product and various Appeal types");

                        CreateAppealAsPerAppealType(claseq[0],"A", expectedDueDate[1],true);
                        CreateAppealAsPerAppealType(claseq[1], "A", expectedDueDate[2]);
                        CreateAppealAsPerAppealType(claseq[2], "R", expectedDueDate[0]);
                        ValidateDentalAppeal(claseq[0], expectedDueDate[1], expectedAnalyst, expectedCategoryId, "A");
                        ValidateDentalAppeal(claseq[1], expectedDueDate[2], expectedAnalyst, expectedCategoryId, "A");
                        ValidateDentalAppeal(claseq[2], expectedDueDate[0], expectedAnalyst, expectedCategoryId, "RR");
                    }
                }
                finally
                {
                    _appealCreator.Logout().LoginAsHciAdminUser().NavigateToAppealManager();

                    _appealCreator.DeleteAppealsOnClaim(claseq[0].Split('-')[0]);
                    _appealCreator.DeleteAppealsOnClaim(claseq[1].Split('-')[0]);
                    _appealCreator.DeleteAppealsOnClaim(claseq[2].Split('-')[0]);

                    _appealCreator.GetCommonSql.UpdateDCIAppealDayTypeDb("B");
                    StringFormatter.PrintMessage("updated Appeal calculation type to Business");
                }

                void CreateAppealAsPerAppealType(string claimSeq, string appealType, string expectedDueDate, bool urgent = false)
                {
                    if (!automatedBase.CurrentPage.GetPageHeader().Equals("Appeal Creator"))
                        _appealCreator.NavigateToAppealCreator();
                    _appealCreator.SearchByClaimSequence(claimSeq);
                    _appealCreator.SelectClaimLine();
                    _appealCreator.CreateAppeal(ProductEnum.DCA.GetStringValue(), "DocID", "", appealType, urgent:urgent);
                    _appealSearch = _appealCreator.NavigateToAppealSearch();
                    _appealSearch.SelectAllAppeals();
                    _appealSearch.SelectClientSmtst();
                    automatedBase.CurrentPage = _appealSummary = _appealSearch.SearchByClaimSequence(claimSeq);
                    _appealSummary.GetAppealDetailFieldPresentByLabel("Due Date").ShouldBeEqual(expectedDueDate);
                }

                void ValidateDentalAppeal(string claimseq, string expectedDueDate, List<string> expectedAnalysts, string expectedCatId, string appealType)
                {
                    if (UserType.CurrentUserType != user)
                        automatedBase.CurrentPage.Logout().LoginAsHciAdminUser();
                    _appealSearch = automatedBase.CurrentPage.NavigateToAppealSearch();
                    _appealSearch.SelectAllAppeals();
                    _appealSearch.SelectClientSmtst();
                    _appealAction = _appealSearch.SearchByClaimSequenceToGoAppealAction(claimseq);
                    _appealAction.GetPrimaryReviewer().ShouldBeEqual(expectedAnalysts[0].Split('(')[0].Trim(),
                        "Primary Reviewer should be assigned per Appeal Category Assignment");
                    _appealAction.GetAssignedTo().ShouldBeEqual(expectedAnalysts[0].Split('(')[0].Trim(),
                        "Assigned To should be assigned per Appeal Category Assignment");
                    _appealAction.GetAppealCategory().ShouldBeEqual(expectedCatId,
                        "Category ID should be assigned per Appeal Category Assignment");
                    _appealAction.GetDueDate().ShouldBeEqual(expectedDueDate,
                        "Due Date should calculated Based on Client Setting values for the appeal type");
                    _appealSearch = _appealAction.ClickOnSearchIconToNavigateAppealSearchPage();
                    _appealSearch.GetGridViewSection.GetValueInGridByColRow(6)
                        .ShouldBeEqual(appealType);
                    _appealManager = _appealSearch.NavigateToAppealManager();
                    _appealManager.DeleteAppealsAssociatedWithClaim(claimseq);
                }
            }
        }


        [Test] //CAR-2998(CAR-2925)
        public void Verify_Appeal_Due_Date_Calculation_Settings_For_Calendar_Holiday_Options()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                AppealCreatorPage _appealCreator;
                ClientSearchPage _clientSearch;
                AppealSearchPage _appealSearch;
                AppealSummaryPage _appealSummary;

                _appealCreator = automatedBase.CurrentPage.NavigateToAppealCreator();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var claseq = paramLists["ClaimSequence"];
                var excludeHolidayOptions = new List<string> {"Cotiviti", "Client", "Both"};
                Random rnd = new Random();
                var excludeHolidayOption = excludeHolidayOptions[rnd.Next(excludeHolidayOptions.Count)];
                try
                {   
                        var turnAroundTime = new List<string>();
                        var expectedDueDate = new List<string>();
                        automatedBase.CurrentPage.Logout().LoginAsHciAdminUser();
                        _clientSearch = automatedBase.CurrentPage.NavigateToClientSearch();
                        _clientSearch.SearchByClientCodeToNavigateToClientProfileViewPage(ClientEnum.SMTST.ToString(),
                            ClientSettingsTabEnum.Product.GetStringValue());
                        _clientSearch
                            .SelectCalculationType("Business"); // To Reset Exclude Holidays After Each Iteration
                        _clientSearch.SelectCalculationType("Calendar");
                        if (excludeHolidayOption == "Both")
                        {
                            _clientSearch.SelectHolidayOption(excludeHolidayOptions[0]);
                            _clientSearch.SelectHolidayOption(excludeHolidayOptions[1]);
                        }
                        else
                        {
                            _clientSearch.SelectHolidayOption(excludeHolidayOption);
                        }

                        _clientSearch.GetSideWindow.Save();
                        StringFormatter.PrintMessage($"Updated Holiday option type to {excludeHolidayOption}");
                        _clientSearch.IsHolidayOptionSetToTrueInDatabase(excludeHolidayOption)
                            .ShouldBeTrue($"Holiday Option Should Be Set To True In Database for {excludeHolidayOption}");
                        turnAroundTime = _clientSearch.GetAppealDueDatesInputFieldsByLabel(ProductEnum.DCA.ToString());
                        foreach (var time in turnAroundTime)
                        {
                            expectedDueDate.Add(
                                _appealCreator.CalculateAndGetAppealDueDateFromDatabase(time, "Calendar"));
                        }

                        _appealCreator = _clientSearch.Logout().LoginAsClientUser().NavigateToAppealCreator();

                        StringFormatter.PrintMessage("Create and verify appeal due date for various appeal types");
                        foreach (Enum appealType in Enum.GetValues(typeof(AppealType)))
                        {
                            if (appealType.GetStringValue() == "D")
                                continue;
                            _appealCreator.SearchByClaimSequence(claseq);
                            _appealCreator.SelectClaimLine();
                            if (appealType.GetStringValue() == "A")
                            {
                                StringFormatter.PrintMessage("Verify Appeal Due Date For Appeal type and urgent");
                                _appealCreator.CreateAppeal(ProductEnum.DCA.GetStringValue(), "DocID", "",
                                    appealType.GetStringValue(), urgent: true);
                                _appealSearch = _appealCreator.NavigateToAppealSearch();
                                _appealSearch.SelectAllAppeals();
                                _appealSearch.SelectClientSmtst();
                                automatedBase.CurrentPage = _appealSummary = _appealSearch.SearchByClaimSequence(claseq);
                                _appealSummary.GetAppealDetailFieldPresentByLabel("Due Date")
                                    .ShouldBeEqual(expectedDueDate[1]);
                                _appealSummary.NavigateToAppealCreator();

                                _appealCreator.Logout().LoginAsHciAdminUser().NavigateToAppealManager();
                                _appealCreator.DeleteAppealsOnClaim(claseq.Split('-')[0]);
                                _appealCreator.Logout().LoginAsClientUser().NavigateToAppealCreator();

                                StringFormatter.PrintMessage("Verify Appeal Due Date For Appeal type");
                                _appealCreator.SearchByClaimSequence(claseq);
                                _appealCreator.SelectClaimLine();
                                _appealCreator.CreateAppeal(ProductEnum.DCA.GetStringValue(), "DocID", "",
                                    appealType.GetStringValue());
                                _appealSearch = _appealCreator.NavigateToAppealSearch();
                                _appealSearch.SelectAllAppeals();
                                _appealSearch.SelectClientSmtst();
                                automatedBase.CurrentPage = _appealSummary = _appealSearch.SearchByClaimSequence(claseq);
                                _appealSummary.GetAppealDetailFieldPresentByLabel("Due Date")
                                    .ShouldBeEqual(expectedDueDate[2]);
                                _appealSummary.NavigateToAppealCreator();

                                _appealCreator.Logout().LoginAsHciAdminUser().NavigateToAppealManager();
                                _appealCreator.DeleteAppealsOnClaim(claseq.Split('-')[0]);
                                _appealCreator.Logout().LoginAsClientUser().NavigateToAppealCreator();
                            }
                            else
                            {
                                StringFormatter.PrintMessage("Verify Appeal Due Date For Record Review Appeal type");
                                _appealCreator.CreateAppeal(ProductEnum.DCA.GetStringValue(), "DocID", "",
                                    appealType.GetStringValue());
                                _appealSearch = _appealCreator.NavigateToAppealSearch();
                                _appealSearch.SelectAllAppeals();
                                _appealSearch.SelectClientSmtst();
                                automatedBase.CurrentPage = _appealSummary = _appealSearch.SearchByClaimSequence(claseq);
                                _appealSummary.GetAppealDetailFieldPresentByLabel("Due Date")
                                    .ShouldBeEqual(expectedDueDate[0]);
                                _appealSummary.NavigateToAppealCreator();

                                _appealCreator.Logout().LoginAsHciAdminUser().NavigateToAppealManager();
                                _appealCreator.DeleteAppealsOnClaim(claseq.Split('-')[0]);
                                _appealCreator.Logout().LoginAsClientUser().NavigateToAppealCreator();
                            }
                        }

                }
                finally
                {
                    _appealCreator.Logout().LoginAsHciAdminUser().NavigateToAppealManager();
                    _appealCreator.DeleteAppealsOnClaim(claseq.Split('-')[0]);
                    _appealCreator.GetCommonSql.UpdateDCIAppealDayTypeDb("B");
                    StringFormatter.PrintMessage("updated Appeal calculation type to Business");
                }
            }
        }

        [Test, Category("SmokeTestDeployment")] //TANT-94
        public void Verify_Appeal_Creator_Page_on_Popup()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                AppealCreatorPage _appealCreator;
                _appealCreator = automatedBase.CurrentPage.NavigateToAppealCreator();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var claimNo = paramLists["ClaimNumber"];
                var fileType = paramLists["FileType"];
                var fileName = paramLists["FileName"];
                const string col1 = "Select Appeal Lines";
                const string col2 = "Selected Lines";

                _appealCreator.SearchByClaimNoAndStayOnSearch(claimNo);
                var claimSequence = _appealCreator.GetEnableClaimSequence();
                var claimActionPopup =
                    _appealCreator.ClickOnClaimSequenceOfAppealCreatorSearchByClaimSeq(claimSequence);
                var appealCreatorPopup = claimActionPopup.ClickOnCreateAppealIcon();
                appealCreatorPopup.ClickOnClaimLine(1);
                var list = appealCreatorPopup.GetSelectedClaimLinesByHeader(col1);
                list.Count.ShouldBeEqual(1, "Is Claim Line Selected?");
                appealCreatorPopup.GetSelectedClaimLinesByHeader(col2).ShouldCollectionBeEqual(list,
                    "Is claim Line Equal on Select Appeal Lines and Selected Lines");
                AddFileForUpload(fileType, 1, fileName);
                var claimActionPopUp = appealCreatorPopup.ClickOnClaimSequenceAndSwitchWindow();
                claimActionPopUp.GetPageHeader().ShouldBeEqual("Claim Action");
                claimActionPopUp.SwitchBack();
                appealCreatorPopup.ClickOnCancelBtn();
                if (appealCreatorPopup.IsPageErrorPopupModalPresent())
                    appealCreatorPopup.ClickOnOkButtonOnConfirmationModal();
                _appealCreator.CloseAnyTabIfExist();

                void AddFileForUpload(string fileTyp, int row, string fileNm)
                {
                    _appealCreator.PassGivenFileNameFilePathForDocumentUpload(fileNm);
                    _appealCreator.SetFileTypeListVlaue(fileTyp);
                    _appealCreator.SetAppealCreatorFieldValue("Description", "test appeal doc");
                    _appealCreator.ClickOnAddFileBtn();
                    _appealCreator.WaitForStaticTime(1500);
                    _appealCreator.IsFileToUploadPresent().ShouldBeTrue("File document present for uploading ");
                    _appealCreator.FileToUploadDocumentValue(row, 2)
                        .ShouldBeEqual(fileNm, "Document correct and present");

                }
            }
        }

        [Test] //TE-915 + CAR-3133 [CAR-3262]
        public void Verify_Creation_Of_Appeal_With_External_DocumentId_And_File_Type()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                AppealCreatorPage _appealCreator;
                _appealCreator = automatedBase.QuickLaunch.NavigateToAppealCreator();
                    
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var claseq = paramLists["ClaimSequence"].Split(',').ToList();
                var docId = paramLists["DocID"];
                //var reasoncode = paramLists["ReasonCode"];
                List<string> appealDocType = new List<string>();
                appealDocType.Add(DentalAppealDocTypeEnum.ClaimImage.GetStringValue());
                appealDocType.Add(DentalAppealDocTypeEnum.ChartNotes.GetStringValue());

                List<string> expectedFileTypeList = new List<string>();
                foreach (DentalAppealDocTypeEnum fileTypeEnum in Enum.GetValues(typeof(DentalAppealDocTypeEnum)))
                {
                    var preAuthFileType = fileTypeEnum.GetStringValue();
                    expectedFileTypeList.Add(preAuthFileType);
                }

                try
                {
                    _appealCreator.Logout().LoginAsHciAdminUser().NavigateToAppealManager();
                    foreach (var claimSequence in claseq)
                    {
                        _appealCreator.DeleteAppealsOnClaim(claimSequence);
                        _appealCreator.DeleteAppealDocument(claimSequence.Replace("-", ""));
                    }
                    _appealCreator.Logout().LoginAsClientUser().NavigateToAppealCreator();
                    _appealCreator.SearchByClaimSequence(claseq[0]);
                    _appealCreator.SelectClaimLine();

                    //CAR-3133 [CAR-3262]
                    _appealCreator.SelectProduct(ProductEnum.DCA.GetStringValue());
                    _appealCreator.GetAvailableFileTypeList().ShouldCollectionBeEqual(expectedFileTypeList, "File Types list is correctly displayed for DCA product");

                    _appealCreator.CreateAppeal(ProductEnum.DCA.GetStringValue(), docId, appealType: "A",
                        fileType: DentalAppealDocTypeEnum.ClaimImage.GetStringValue());
                    _appealCreator.GetAppealDocumentType(claseq[0].Replace("-", ""))
                        .ShouldBeEqual(new List<int>(){ (int)DentalAppealDocTypeEnum.ClaimImage}, "Value correctly stored?");

                    _appealCreator.SearchByClaimSequence(claseq[1]);
                    _appealCreator.SelectClaimLine();
                    _appealCreator.CreateAppealWithMultipleDocType(ProductEnum.DCA.GetStringValue(), docId,
                        appealType: "A",
                        fileType: appealDocType);
                    _appealCreator.GetAppealDocumentType(claseq[1].Replace("-", ""))
                        .ShouldCollectionBeEquivalent(
                            new List<int> {(int)DentalAppealDocTypeEnum.ClaimImage, (int)DentalAppealDocTypeEnum.ChartNotes},
                            "Value correctly stored?");
                }
                finally
                {
                    _appealCreator.Logout().LoginAsHciAdminUser().NavigateToAppealManager();
                    foreach (var claimSequence in claseq)
                    {
                        _appealCreator.DeleteAppealsOnClaim(claimSequence);
                        _appealCreator.DeleteAppealDocument(claimSequence.Replace("-", ""));
                    }
                }
            }
        }

        #endregion


        #region Private methods 

        private void VerifyAppealDataAppealAfterSave(ClaimActionPage _claimAction, string product, string appealType)
        {
            
            AppealSearchPage _appealSearch;
            AppealSummaryPage _appealSummary;
            
            StringFormatter.PrintMessage($"Verifying the {product} appeal data from New Claim Action after it is saved ");
            _claimAction.ClickOnViewAppealIcon();
            var appealSeq = _claimAction.GetAppealSequence().Split('-')[0];
            _claimAction.GetAppealStatusByRow(1).ShouldBeEqual("New", "Appeal Status is showing correctly as 'New' in Claim Action Page");
            _claimAction.GetAppealTypeByRow().ShouldBeEqual(appealType, $"Appeal Type is showing up as '{appealType}' in Claim Action Page");

            StringFormatter.PrintMessageTitle(" Performing appeal search by selecting 'Type' as one of the search filters ");
            _appealSearch = _claimAction.NavigateToAppealSearch();
            _appealSearch.ClickOnAdvancedSearchFilterIcon(true);
            _appealSearch.SelectClientSmtst();
            _appealSearch.SetInputFieldByInputLabel("Status", "New");
            _appealSearch.SelectDropDownListbyInputLabel("Type", appealType);
            _appealSummary = _appealSearch.SearchByAppealSequenceAndNavigateToAppealSummaryPage(appealSeq, clearAll:false);

            _appealSummary.GetPageHeader().ShouldBeEqual("Appeal Summary", $"The {product} appeal can be searched using the Appeal Type filter");

            StringFormatter.PrintMessage($"Verifying the {product} appeal data from Appeal Action after it is saved ");
            _appealSummary.GetStatusValue().ShouldBeEqual("New", "Appeal Status should be 'New' ");
            _appealSummary.GetAppealType().ShouldBeEqual(appealType, $"Appeal type should dbe {appealType}");
        }
        //private void AddFileForUpload(string fileType, int row, string fileName)
        //{
        //    _appealCreator.NavigateToAppealCreator();
        //    _appealCreator.PassGivenFileNameFilePathForDocumentUpload(fileName);
        //    _appealCreator.SetFileTypeListVlaue(fileType);
        //    _appealCreator.SetAppealCreatorFieldValue("Description", "test appeal doc");
        //    _appealCreator.ClickOnAddFileBtn();
        //    _appealCreator.WaitForStaticTime(1500);
        //    _appealCreator.IsFileToUploadPresent().ShouldBeTrue("File document present for uploading ");
        //    _appealCreator.FileToUploadDocumentValue(row, 2)
        //        .ShouldBeEqual(fileName, "Document correct and present");

        //}
        //private void ValidateDentalAppeal(string claseq, string expectedDueDate, List<string> expectedAnalyst,
        //    string expectedCategoryId, Enum appealType)
        //{
        //    _appealSearch = _appealCreator.NavigateToAppealSearch();
        //    _appealSearch.SelectAllAppeals();
        //    _appealSearch.SelectClientSmtst();
        //    automatedBase.CurrentPage = _appealSummary = _appealSearch.SearchByClaimSequence(claseq);
        //    _appealSummary.GetAppealDetailFieldPresentByLabel("Due Date").ShouldBeEqual(expectedDueDate);
        //    _appealSearch = _appealSummary.Logout().LoginAsHciAdminUser().NavigateToAppealSearch();
        //    _appealSearch.SelectAllAppeals();
        //    _appealSearch.SelectClientSmtst();
        //    _appealAction = _appealSearch.SearchByClaimSequenceToGoAppealAction(claseq);
        //    _appealAction.GetPrimaryReviewer().ShouldBeEqual(expectedAnalyst[0].Split('(')[0].Trim(),
        //        "Primary Reviewer should be assigned per Appeal Category Assignment");
        //    _appealAction.GetAssignedTo().ShouldBeEqual(expectedAnalyst[0].Split('(')[0].Trim(),
        //        "Assigned To should be assigned per Appeal Category Assignment");
        //    _appealAction.GetAppealCategory().ShouldBeEqual(expectedCategoryId,
        //        "Category ID should be assigned per Appeal Category Assignment");
        //    _appealAction.GetDueDate().ShouldBeEqual(expectedDueDate,
        //        "Due Date should calculated Based on Client Setting values for the appeal type");
        //    _appealSearch = _appealAction.ClickOnSearchIconToNavigateAppealSearchPage();
        //    _appealSearch.GetGridViewSection.GetValueInGridByColRow(6)
        //        .ShouldBeEqual(appealType.GetStringValue());
        //    _appealManager = _appealSearch.NavigateToAppealManager();
        //    _appealManager.DeleteAppealsAssociatedWithClaim(claseq);
        //    _appealCreator = _appealSearch.Logout().LoginAsClientUser().NavigateToAppealCreator();
        //}

        //private void CheckClaimSequenceStatusOfAppeal(string claimSeq, bool checkTrueFalse)
        //{
        //    _claimAction = _appealCreator.ClickOnClaimSequenceOfAppealCreatorSearchByClaimSeq(claimSeq);
        //    _claimAction.ClickOnViewAppealIcon();
        //    var countOfCloseCompletedAppeals = 0;
        //    var appealList = _claimAction.GetListOfAppealSequenceInRows();
        //    foreach (var appealSeq in appealList)
        //    {
        //        var status = _claimAction.GetAppealStatus(appealSeq);
        //        if (checkTrueFalse)
        //            (status.Equals("Closed") || status.Equals("Complete")).ShouldBeTrue("All previous appeals are in status of closed or complete");
        //        else if (status.Equals("Closed") || status.Equals("Complete")) countOfCloseCompletedAppeals++;
        //        //(status.Equals("Closed") || status.Equals("Complete")).ShouldBeFalse(
        //        //    "All previous appeals are not in status of closed or complete");
        //    }
        //    countOfCloseCompletedAppeals.ShouldBeLess(appealList.Count, "All Previous appeals are not closed or completed.");
        //    _appealCreator.CloseAnyPopupIfExist();
        //}


        //private void DeletePreviousAppeals(string claimSeq)
        //{
        //    automatedBase.CurrentPage =
        //        _appealManager = _appealCreator.Logout().LoginAsHciAdminUser().NavigateToAppealManager();
        //    _appealManager.DeleteAppealsAssociatedWithClaim(claimSeq);
        //    automatedBase.CurrentPage =
        //        _appealCreator = _appealCreator.Logout().LoginAsClientUser().NavigateToAppealCreator();
        //}

        #endregion
    }
}

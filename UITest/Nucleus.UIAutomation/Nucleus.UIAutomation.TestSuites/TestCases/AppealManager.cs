using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using Nucleus.Service.Data;
using Nucleus.Service.PageServices.QuickLaunch;
using Nucleus.Service.PageServices.Settings.User;
using Nucleus.Service.Support.Enum;
using Nucleus.UIAutomation.TestSuites.Base;
using Nucleus.Service.PageServices.Appeal;
using Nucleus.Service.PageServices.Claim;
using Nucleus.Service.PageServices.QA;
using Nucleus.Service.Support.Common.Constants;
using NUnit.Framework;
using UIAutomation.Framework.Utils;
using Nucleus.Service.Support.Common;
using Nucleus.Service.Support.Environment;
using Nucleus.Service.PageServices.Settings;
using Nucleus.Service.SqlScriptObjects.Common_SQL;
using Nucleus.Service.Support.Menu;
using Nucleus.UIAutomation.TestSuites.Common;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Database;

namespace Nucleus.UIAutomation.TestSuites.TestCases
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class AppealManager 
    {
        #region PRIVATE FIELDS

        /*private AppealManagerPage _appealManager;
        private ProfileManagerPage _profileManager;
        private automatedBase.QuickLaunchPage _automatedBase.QuickLaunch;
        private AppealCreatorPage _appealCreator;
        private AppealActionPage _appealAction;
        private ClaimActionPage _claimAction;
        private AppealSummaryPage _appealSummary;
        private ClientSearchPage _clientSearch;
        private QaAppealSearchPage _qaAppealSearch;
        private FileUploadPage FileUploadPage;

        */

        /*private List<string> _appealAssignableUserList;
        private List<string> _cvtyPlanList ;
        private List<string> _pehpPlanList;
        private List<string> _smtstPlanList;
        private List<string> _ttreePlanList;
        private List<string> _assignedClientList;
        private List<string> _getActiveProductList;
        private List<string> _lineOfBusinessList;*/
        
        //private readonly string _operationalManager = RoleEnum.OperationsManager.GetStringValue();
        //private CommonValidations _commonValidations;


        #endregion

        #region OVERRIDE METHODS
        /*protected override void ClassInit()
        {
            try
            {
                base.ClassInit();
                _appealManager = automatedBase.QuickLaunch.NavigateToAppealManager();
                 FileUploadPage = _appealManager.GetFileUploadPage;
                _commonValidations = new CommonValidations(automatedBase.CurrentPage);
                RetrieveListFromDatabase();
            }
            catch (Exception)
            {
                if (StartFlow != null)
                    StartFlow.Dispose();

                throw;
            }
        }*/
        /*protected override void TestInit()
        {
            base.TestInit();
            automatedBase.CurrentPage = _appealManager;
        }*/
        /*protected override void ClassCleanUp()
        {
            try
            {

            }

            finally
            {
                base.ClassCleanUp();
            }
        }*/

        /*protected override void TestCleanUp()
        {
            if (string.Compare(UserType.CurrentUserType, UserType.HCIADMIN, StringComparison.OrdinalIgnoreCase) != 0)
            {
                _appealManager = _appealManager.Logout().LoginAsHciAdminUser().NavigateToAppealManager();
            }

            if (!automatedBase.CurrentPage.IsCurrentClientAsExpected(automatedBase.EnvironmentManager.TestClient))
            {
                CheckTestClientAndSwitch();
                _appealManager = automatedBase.CurrentPage.NavigateToAppealManager();
            }

            if (_appealManager.GetPageHeader() != "Appeal Manager")
            {
                automatedBase.CurrentPage.NavigateToAppealManager();
            }
            _appealManager.ClickOnSidebarIcon();
            _appealManager.ClearAll();
            _appealManager.ClickOnAdvancedSearchFilterIcon(false);
            _appealManager.SelectAppealsDueToday();
            base.TestCleanUp();
        }*/


        #endregion

        #region DBinteraction methods
        /*private void RetrieveListFromDatabase(AppealManagerPage _appealManager, NewAutomatedBaseParallelRun automatedBase)
        {
            _appealAssignableUserList = _appealManager.GetPrimaryReviewerAssignedToList();
            _cvtyPlanList = _appealManager.GetAssociatedPlansForClient(ClientEnum.CVTY.GetStringDisplayValue());
            _pehpPlanList = _appealManager.GetAssociatedPlansForClient(ClientEnum.PEHP.ToString());
            _smtstPlanList = _appealManager.GetAssociatedPlansForClient(ClientEnum.SMTST.ToString());
            _ttreePlanList = _appealManager.GetAssociatedPlansForClient(ClientEnum.TTREE.GetStringDisplayValue());

            _assignedClientList = _appealManager.GetAssignedClientList(automatedBase.EnvironmentManager.HciAdminUsername);            
            _assignedClientList.Insert(0,"All");

            _getActiveProductList = _appealManager.GetCommonSql.GetExpectedProductListFromDatabase(ClientEnum.SMTST.ToString());
            _lineOfBusinessList = _appealManager.GetCommonSql.GetLineOfBusiness();
            _appealManager.CloseDbConnection();
        }*/
       


        #endregion

        #region PROTECTED PROPERTIES
        protected string FullyQualifiedClassName
        {
            get { return GetType().FullName; }
        }
        #endregion

        #region TEST SUITES

        [Test] //CAR-2992(CAR-2962) + CAR-3121(CAR-3063)
        public void Verify_appeal_level_count_should_be_unique_to_that_client()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealManagerPage _appealManager;
                automatedBase.CurrentPage = _appealManager = automatedBase.QuickLaunch.NavigateToAppealManager();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var claimSequence = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "ClaimSequence", "Value");
                _appealManager.SearchByClaimSequence(claimSequence);
                #region CAR-3121(CAR-3063)
                _appealManager.GetSearchResultListByCol(6).Distinct().ShouldCollectionBeEquivalent(new List<string> { "A", "MRR" }, "List should contain MRR that have been created on that claim.");
                _appealManager.IsAppealLevelBadgeValuePresentForMRRAppealType().ShouldBeTrue("Badge should show the count of appeal types of appeal and MRR that have been created on that claim.");
                #endregion
                _appealManager.GetAppealLevelBadgeValue().Max().ShouldBeEqual(
                    _appealManager.GetAppealLevel(claimSequence, ClientEnum.SMTST),
                    $"Is Correct Appeal Level display for {ClientEnum.SMTST}");
                _appealManager.SearchByClaimSequence(claimSequence, ClientEnum.RPE.ToString());
                _appealManager.GetAppealLevelBadgeValue().Max().ShouldBeEqual(
                    _appealManager.GetAppealLevel(claimSequence, ClientEnum.RPE),
                    $"Is Correct Appeal Level display for {ClientEnum.RPE}");
            }
        }

        [Test,Category("AppealManager1")] //US61341
        public void Verify_single_appeal_edit_with_proper_validation()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealManagerPage _appealManager;
                automatedBase.CurrentPage = _appealManager = automatedBase.QuickLaunch.NavigateToAppealManager();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var claimSequence = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName,
                    "ClaimSequence", "Value");
                var lockBy = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "LockedBy",
                    "Value");
                var expectedAppealTypeList =
                    automatedBase.DataHelper.GetMappingData(FullyQualifiedClassName, "Appeal_type").Values.ToList();
                var appealStatus =
                    automatedBase.DataHelper.GetMappingData(FullyQualifiedClassName, "appeal_status").Values.ToList();
                expectedAppealTypeList.RemoveAt(0);
                var expectedPriorityList =
                    automatedBase.DataHelper.GetMappingData(FullyQualifiedClassName, "Appeal_priority").Values.ToList();
                expectedPriorityList.RemoveAt(0);
                _appealManager.DeleteAppealLockByClaimSeq(claimSequence);
                _appealManager.UpdateAppealStatusToNew(claimSequence);
                _appealManager.SearchByClaimSequence(claimSequence);

                var status = _appealManager.GetSearchResultByColRow(11);
                var appealSeq = _appealManager.GetSearchResultByColRow(4);

                if (status == AppealStatusEnum.Open.GetStringDisplayValue()
                    || status == AppealStatusEnum.Complete.GetStringDisplayValue()
                    || status == AppealStatusEnum.Closed.GetStringDisplayValue())
                {
                    _appealManager.ChangeStatusOfListedAppeal(AppealStatusEnum.New.GetStringDisplayValue(), appealSeq);
                    status = AppealStatusEnum.New.GetStringDisplayValue();
                }

                appealStatus.Remove(status);
                appealStatus.Remove(AppealStatusEnum.Open.GetStringDisplayValue());
                appealStatus.Remove(AppealStatusEnum.Complete.GetStringDisplayValue());
                appealStatus.Remove(AppealStatusEnum.Closed.GetStringDisplayValue());
                appealStatus.Remove(AppealStatusEnum.MentorReview.GetStringDisplayValue());
                Random rnd = new Random();
                var newStatus = appealStatus[rnd.Next(appealStatus.Count)];



                var appealType = _appealManager.GetSearchResultByColRow(6);
                var appealPriority = _appealManager.IsUrgentIconPresent(1);
                var dueDate = _appealManager.GetSearchResultByColRow(3);



                var newAppealType = appealType == AppealType.Appeal.GetStringValue()
                    ? AppealType.RecordReview.GetStringDisplayValue()
                    : AppealType.Appeal.GetStringDisplayValue();

                var newDueDate = Convert.ToDateTime(dueDate).AddDays(-2).ToString("MM/d/yyyy");


                _appealManager.IsAppealLockIconPresent(appealSeq).ShouldBeFalse("Appeal Should not be locked");

                _appealManager.ClickOnAppealSequenceSwitchToAppealAction(appealSeq);
                var url = automatedBase.CurrentPage.CurrentPageUrl;

                _appealManager.CloseAnyPopupIfExistInAppealManager();
                _appealManager.SwitchToOpenAppealActionByUrlForAdmin(url, 5);

                _appealManager.SwitchBackToAppealManagerPage();
                _appealManager.ClickOnFindButton();
                _appealManager.WaitForWorking();

                _appealManager.IsEditIconDisabledByAppealSequence(appealSeq)
                    .ShouldBeTrue("Edit Icon Should be disabled");
                _appealManager.GetToolTipOfDisabledEditIconByAppealSequence(appealSeq)
                    .ShouldBeEqual("Modifications cannot be made to an appeal that is locked",
                        "Tooltip of disabled Edit Icon");

                _appealManager.GetAppealLockIconTooltip(appealSeq)
                    .ShouldBeEqual(string.Format("This appeal is locked by {0}", lockBy),
                        "Verify Appeal Icon is locked and Tooltip Message");
                _appealManager.SwitchBackToAppealActionPage().ClickOnSearchIconToNavigateAppealSearchPage();
                _appealManager.SwitchBackToAppealManagerPage();
                _appealManager.ClickOnFindButton();
                _appealManager.WaitForWorking();
                _appealManager.IsAppealLockIconPresent(appealSeq).ShouldBeFalse("Appeal Should not be Locked");


                _appealManager.ClickOnEditIcon();
                _appealManager.IsEditFormDisplayed().ShouldBeTrue("Edit Appeal Information Section display?");

                var isAppealTypeIsAppeal = appealType == AppealType.Appeal.GetStringValue();
                _appealManager.GetInputValueInDropDownListInEditAppealByLabel("Appeal Type")
                    .ShouldBeEqual(
                        isAppealTypeIsAppeal
                            ? AppealType.Appeal.GetStringDisplayValue()
                            : AppealType.RecordReview.GetStringDisplayValue(), "Appeal Type Selected Value");

                _appealManager.GetInputValueInDropDownListInEditAppealByLabel("Appeal Priority")
                    .ShouldBeEqual(
                        appealPriority
                            ? PriorityEnum.Urgent.GetStringValue()
                            : PriorityEnum.Normal.GetStringValue(), "Appeal Priority Selected Value");

                _appealManager.GetAvailableDropDownListInEditAppealByLabel("Appeal Type")
                    .ShouldCollectionBeEqual(expectedAppealTypeList, "Appeal Type List");

                var isAppealPriorityDisabled = _appealManager.IsInputFieldForRespectiveLabelDisabled("Priority");

                if (isAppealPriorityDisabled)
                {
                    _appealManager.SelectDropDownListbyInputLabel("Appeal Type",
                        AppealType.Appeal.GetStringDisplayValue());
                    _appealManager.SendTabKeyInDueDate();
                    _appealManager.ClosePageError();
                }

                _appealManager.GetAvailableDropDownListInEditAppealByLabel("Appeal Priority")
                    .ShouldCollectionBeEquivalent(expectedPriorityList, "Appeal Priority List");

                if (isAppealPriorityDisabled)
                {
                    _appealManager.SelectDropDownListbyInputLabel("Appeal Type",
                        AppealType.RecordReview.GetStringDisplayValue());
                    _appealManager.SendTabKeyInDueDate();
                    _appealManager.ClosePageError();
                }


                for (var i = 0; i < 2; i++)
                {
                    _appealManager.SelectDropDownListbyInputLabel("Appeal Type",
                        isAppealTypeIsAppeal
                            ? AppealType.RecordReview.GetStringDisplayValue()
                            : AppealType.Appeal.GetStringDisplayValue());


                    VerifyDueDateFunctionality("appeal type", _appealManager);

                    if (isAppealTypeIsAppeal)
                    {
                        _appealManager.IsInputFieldForRespectiveLabelDisabled("Priority")
                            .ShouldBeTrue("Priority should be disabled when appeal type set to <Record Review>");
                        _appealManager.GetInputValueInDropDownListInEditAppealByLabel("Appeal Priority")
                            .ShouldBeEqual(PriorityEnum.Normal.GetStringValue(),
                                "Appeal Priority should be Normal when appeal type set to Record Revieew");
                        _appealManager.IsInvalidInputPresentByLabel("Appeal Priority")
                            .ShouldBeTrue("Exclamation Icon display?");
                        _appealManager.GetInvalidInputToolTipByLabel("Appeal Priority")
                            .ShouldBeEqual("Priority cannot be modified for a record review.",
                                "Tooltip message of Exclamation Icon in Appeal Priority field");
                    }
                    else
                    {
                        _appealManager.IsInputFieldForRespectiveLabelDisabled("Priority")
                            .ShouldBeFalse("Priority shouldnot be disabled when appeal type set to <Appeal>");

                    }

                    isAppealTypeIsAppeal = !isAppealTypeIsAppeal;
                }


                if (_appealManager.IsInputFieldForRespectiveLabelDisabled("Priority"))
                {
                    _appealManager.SelectDropDownListbyInputLabel("Appeal Type",
                        AppealType.Appeal.GetStringDisplayValue());
                }

                //for (var i = 0; i < 2; i++)
                //{
                //    _appealManager.SelectDropDownListbyInputLabel("Appeal Priority",
                //        appealPriority
                //            ? PriorityEnum.Normal.GetStringValue()
                //            : PriorityEnum.Urgent.GetStringValue());

                //    VerifyDueDateFunctionality("priority");
                //    appealPriority = !appealPriority;
                //}

                _appealManager.SetDueDateInEditAppealForm(Convert.ToDateTime(dueDate).AddDays(2).ToString("MM/d/yyyy"),
                    false);
                _appealManager.WaitToLoadPageErrorPopupModal();
                _appealManager.GetPageErrorMessage()
                    .ShouldBeEqual(
                        "The due date selected is greater than the current due date. Do you wish to proceed?",
                        "Popup Messaage show when date that is greater than the existing date");
                _appealManager.ClickOkCancelOnConfirmationModal(true);
                _appealManager.ClearDueDate();
                _appealManager.ClickOnSaveButton();
                _appealManager.WaitToLoadPageErrorPopupModal();
                _appealManager.GetPageErrorMessage()
                    .ShouldBeEqual("Updates cannot be initiated without any criteria entered.",
                        "Popup message when no criteria is entered in the fields");
                _appealManager.ClosePageError();


                _appealManager.SelectDropDownListbyInputLabel("Appeal Type", newAppealType);
                _appealManager.SendTabKeyInDueDate();
                _appealManager.ClosePageError();
                if (newAppealType == AppealType.Appeal.GetStringDisplayValue())
                    _appealManager.SelectDropDownListbyInputLabel("Appeal Priority",
                        PriorityEnum.Urgent.GetStringValue());


                _appealManager.SetDueDateInEditAppealForm(newDueDate);
                _appealManager.SelectDropDownListbyInputLabel("Status", newStatus);


                _appealManager.ClickOnCancelLink();

                StringFormatter.PrintMessageTitle(
                    "Verify Appeal Row when cancel button is clicked and should discard the changes");
                _appealManager.IsEditFormDisplayed().ShouldBeFalse("Edit form panel should close.");


                _appealManager.GetSearchResultByColRow(3)
                    .ShouldBeEqual(Convert.ToDateTime(dueDate).ToString("MM/dd/yyyy"), "Due Date should not update");
                _appealManager.GetSearchResultByColRow(6)
                    .ShouldBeEqual(
                        appealType, "Appeal Type should not change");

                if (appealPriority)
                    _appealManager.IsUrgentIconPresent(1).ShouldBeTrue("Urgent Icon Should display");
                else
                    _appealManager.IsUrgentIconPresent(1).ShouldBeFalse("Urgent Icon Should not display");

                _appealManager.GetSearchResultByColRow(11)
                    .ShouldBeEqual(status, "Status Should display previous value");

                _appealManager.ClickOnEditIcon();
                _appealManager.SelectDropDownListbyInputLabel("Appeal Type", newAppealType);
                _appealManager.SendTabKeyInDueDate();
                _appealManager.ClosePageError();
                if (newAppealType == AppealType.Appeal.GetStringDisplayValue())
                    _appealManager.SelectDropDownListbyInputLabel("Appeal Priority",
                        PriorityEnum.Urgent.GetStringValue());


                _appealManager.SetDueDateInEditAppealForm(newDueDate);
                _appealManager.SelectDropDownListbyInputLabel("Status", newStatus);

                _appealManager.ClickOnSaveButton();
                _appealManager.WaitForWorking();
                StringFormatter.PrintMessageTitle("Verify Appeal Row when save the changes");
                _appealManager.GetSearchResultByColRow(3)
                    .ShouldBeEqual(Convert.ToDateTime(newDueDate).ToString("MM/dd/yyyy"), "Due Date should update");
                _appealManager.GetSearchResultByColRow(6)
                    .ShouldBeEqual(
                        newAppealType == AppealType.Appeal.GetStringDisplayValue()
                            ? AppealType.Appeal.GetStringValue()
                            : AppealType.RecordReview.GetStringValue(), "Appeal Type should update");

                if (newAppealType == AppealType.Appeal.GetStringDisplayValue())
                    _appealManager.IsUrgentIconPresent(1).ShouldBeTrue("Urgent Icon Should display");
                else
                    _appealManager.IsUrgentIconPresent(1).ShouldBeFalse("Urgent Icon Should display");
                _appealManager.GetSearchResultByColRow(11).ShouldBeEqual(newStatus, "Status Should Updated");
            }
        }
       
      //[Test] //US61341 +DE6082
        [Test, Category("AppealManager1")] //US61341 + DE6082 + US69657 + CAR-3121(CAR-3063)
        public void Verify_search_result_are_correct()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealManagerPage _appealManager;
                AppealActionPage _appealAction;
                AppealSummaryPage _appealSummary;
                ClaimActionPage _claimAction;

                automatedBase.CurrentPage = _appealManager = automatedBase.QuickLaunch.NavigateToAppealManager();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var claimSequence = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "ClaimSequence", "Value");
                var claimSequence1 = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "ClaimSequence1", "Value");
                _appealManager.SearchByClaimSequence(claimSequence);

                var appealSeq = _appealManager.GetSearchResultByColRow(4, 5);
                var status = _appealManager.GetSearchResultByColRow(11, 5);

                StringFormatter.PrintMessageTitle("Verification of Appeal display result");
                AppealLetterPage appealLetter = null;

                _appealManager.IsEditIconDisplayed().ShouldBeTrue("Edit Icon Present?");
                _appealManager.IsDeleteIconDisplayed().ShouldBeTrue("Delete Icon Present?");

                if (_appealManager.IsAppealLockIconPresent(appealSeq))
                {
                    _appealManager.ClickOnAppealSequenceSwitchToAppealAction(appealSeq);
                    _appealManager.ClosePopupWindowAndSwitchBackToAppealManagerPage();
                    _appealManager.ClickOnFindButton();
                    _appealManager.WaitForWorking();
                }

                if (status != AppealStatusEnum.New.GetStringDisplayValue())
                    _appealManager.ChangeStatusOfListedAppeal(AppealStatusEnum.New.GetStringDisplayValue(), appealSeq);

                _appealManager.GetSearchResultByColRow(11).ShouldBeEqual("Complete", "Status Should display Complete");
                _appealManager.IsBlackColorDueDatePresent()
                    .ShouldBeTrue("Black Color Due Date Present for Complete Status");
                _appealManager.GetSearchResultByColRow(11, 4).ShouldBeEqual("Closed", "Status Should display Closed");
                _appealManager.IsBlackColorDueDatePresent(4)
                    .ShouldBeTrue("Black Color Due Date Present for Closed Status");

                _appealManager.ChangeDueDateOfAppeal(appealSeq, DateTime.Now.AddDays(-1).ToString("MM/d/yyyy"));
                _appealManager.IsBoldRedColorDueDatePresent()
                    .ShouldBeTrue(
                        "Bold Red Color Due Date Present for due dates that are previous to the current date");

                _appealManager.ChangeDueDateOfAppeal(appealSeq, DateTime.Now.ToString("MM/d/yyyy"));
                _appealManager.IsNonBoldOnlyRedColorDueDatePresent()
                    .ShouldBeTrue(
                        "Non Bold Only Red Color Due Date Present for due dates that are equal to the current date");

                _appealManager.ChangeDueDateOfAppeal(appealSeq, DateTime.Now.AddDays(1).ToString("MM/d/yyyy"));
                _appealManager.IsOrangeColorDueDatePresent()
                    .ShouldBeTrue(
                        "Orange Color Due Date Present for due dates that are for tomorrows date to the current date");

                _appealManager.ChangeDueDateOfAppeal(appealSeq, DateTime.Now.AddDays(2).ToString("MM/d/yyyy"));
                _appealManager.IsBlackColorDueDatePresent()
                    .ShouldBeTrue("Black Color Due Date Present for Due dates that are after tomorrows");
                _appealManager.GetSearchResultByColRow(3, 5).IsDateInFormat()
                    .ShouldBeTrue("Due Date is in dd/mm/yyyy format");

                _appealManager.ChangeStatusOfListedAppeal(AppealStatusEnum.Open.GetStringDisplayValue(), appealSeq);
                _appealManager.GetSearchResultByColRow(11, 5)
                    .ShouldBeEqual(AppealStatusEnum.Open.GetStringDisplayValue(), "Status Should display Open");

                _appealManager.ChangeStatusOfListedAppeal(AppealStatusEnum.New.GetStringDisplayValue(), appealSeq);
                _appealManager.GetSearchResultByColRow(11, 5)
                    .ShouldBeEqual(AppealStatusEnum.New.GetStringDisplayValue(), "Status Should display New");

                _appealManager.ChangeStatusOfListedAppeal(AppealStatusEnum.DocumentsWaiting.GetStringDisplayValue(),
                    appealSeq);
                _appealManager.GetSearchResultByColRow(11, 5).ShouldBeEqual(
                    AppealStatusEnum.DocumentsWaiting.GetStringDisplayValue(),
                    "Status Should display Documents Awaiting");
                _appealManager.ChangeStatusOfListedAppeal(AppealStatusEnum.New.GetStringDisplayValue(), appealSeq);

                _appealManager.IsUrgentIconPresent(1).ShouldBeFalse("Urgent Icon should not display");

                _appealManager.IsUrgentIconPresent(3).ShouldBeTrue("Urgent Icon should not display");

                _appealManager.GetPayValueByRow(1)
                    .ShouldBeEqual(AppealResult.Pay.GetStringValue(), "Adjust Icon Should Present");
                _appealManager.GetDenyValueByRow(2)
                    .ShouldBeEqual(AppealResult.Deny.GetStringValue(), "Deny Icon Should Present");
                _appealManager.GetAdjustValueByRow(3)
                    .ShouldBeEqual(AppealResult.Adjust.GetStringValue(), "Pay Icon Should Present");
                _appealManager.GetNoDocsValueByRow(4)
                    .ShouldBeEqual(AppealResult.NoDocs.GetStringValue(), "No Docs Icon Should Present");

                _appealManager.IsAppealLetterIconPrsentByRow(1)
                    .ShouldBeTrue("Appeal Letter Icon Present for status Complete");
                _appealManager.IsAppealLetterIconPrsentByRow(4)
                    .ShouldBeTrue("Appeal Letter Icon Present for status Closed");
                _appealManager.IsAppealLetterIconPrsentByRow(5)
                    .ShouldBeFalse("Appeal Icon should not Present for status New");


                appealLetter = _appealManager.ClickOnAppealLetter(1);
                appealLetter.PageTitle.ShouldBeEqual(appealLetter.CurrentPageTitle, "Appeal Letter Popup");
                automatedBase.CurrentPage = _appealManager = appealLetter.CloseLetterPopUpPageAndBackToAppealManager();

                var appealList = _appealManager.GetSearchResultListByCol(4, false);
                appealList.Select(x => x).Where(string.IsNullOrEmpty)
                    .ToList()
                    .Count.ShouldBeEqual(0, "Appeal Sequence should not empty");

                automatedBase.CurrentPage =
                    _appealAction = _appealManager.ClickOnAppealSequenceSwitchToAppealAction(appealSeq);
                _appealAction.GetPageHeader()
                    .ShouldBeEqual("Appeal Action", "Appeal Action Page open for New status");
                _appealAction.IsEditIconDisabled().ShouldBeTrue("Page should open in View Mode");
                _claimAction = _appealAction.ClickOnClaimSequenceSwitchToClaimAction();
                _claimAction.GetPageHeader()
                    .ShouldBeEqual("Claim Action", "Claim Action popup opened from Appeal Action popup.");
                _appealManager.CloseAnyPopupIfExistInAppealManager();

                automatedBase.CurrentPage = _appealSummary =
                    _appealManager.ClickOnAppealSequenceSwitchToAppealSummary(appealList[1]);
                _appealSummary.GetPageHeader()
                    .ShouldBeEqual("Appeal Summary", "Appeal Summary Page open for other than New status");
                _appealSummary.IsEditIconDisabled().ShouldBeTrue("Page should open in View Mode");
                _claimAction = _appealSummary.ClickOnClaimSequenceSwitchToClaimAction();
                _claimAction.GetPageHeader()
                    .ShouldBeEqual("Claim Action", "Claim Action popup opened from Appeal Action popup.");
                _appealManager.CloseAnyPopupIfExistInAppealManager();

                automatedBase.CurrentPage = _appealSummary =
                    _appealManager.ClickOnAppealSequenceSwitchToAppealSummary(appealList[2]);
                _appealSummary.GetPageHeader()
                    .ShouldBeEqual("Appeal Summary", "Appeal Summary Page open for other than New status");
                ;
                _appealSummary.GetAppealSequenceOnHeader()
                    .ShouldBeEqual(appealList[2], "Popup Page should should update");

                _appealManager.SwitchBackToAppealManagerPage();
                automatedBase.CurrentPage =
                    _appealAction = _appealManager.ClickOnAppealSequenceSwitchToAppealAction(appealSeq);
                _appealManager.WaitForWorkingAjaxMessage();
                _appealAction.GetPageHeader()
                    .ShouldBeEqual("Appeal Action", "Appeal Action Page open for New status");
                _appealManager.GetWindowHandlesCount().ShouldBeEqual(2, "Poup Page should replace previous one");
                _appealManager.ClosePopupWindowAndSwitchBackToAppealManagerPage();

                _appealManager.GetSearchResultByColRow(5, 5)
                    .ShouldBeEqual(ClientEnum.SMTST.ToString(), "Client Code Should display");
                _appealManager.GetSearchResultByColRow(6, 5)
                    .ShouldBeEqual(AppealType.Appeal.GetStringValue(), "Appeal Type Should display");
                _appealManager.GetSearchResultByColRow(6).ShouldBeEqual(AppealType.RecordReview.GetStringValue(),
                    "Appeal Type Should display");
                _appealManager.GetSearchResultByColRow(7, 5).ShouldNotBeEmpty("Category Id Should display");
                _appealManager.GetSearchResultByColRow(8)
                    .DoesNameContainsOnlyFirstWithLastname()
                    .ShouldBeTrue("Primary Reviewer Should be in <First Name><Last Name>");

                var firstDenyCode = _appealManager.GetSearchResultByColRow(9);
                Regex.IsMatch(firstDenyCode, @"^([a-zA-Z0-9_-]){5,5}$")
                    .ShouldBeTrue("The First Deny value '" + firstDenyCode + "' is in alphanumeric format XXXXX");
                var firstPayCode = _appealManager.GetSearchResultByColRow(10);
                Regex.IsMatch(firstPayCode, @"^([a-zA-Z0-9_-]){5,5}$")
                    .ShouldBeTrue("The First Pay value '" + firstPayCode + "' is in alphanumeric format XXXXX");
                _appealManager.IsAppealLevelBadgeIconPresent().ShouldBeTrue("Appeal Level Badge icon is present");
                _appealManager.IsGreyAppealLevelBadgeIconPresent().ShouldBeTrue("Appeal level Badge is Gre.");
                List<string> badgeList = _appealManager.GetAppealLevelBadgeValue();
                foreach (var badge in badgeList)
                {
                    Int32.Parse(badge).ShouldBeGreater(1,
                        "A number badge on the line is present for appeals that are a level 2 or higher only. ");
                }


                _appealManager.SearchByClaimSequence(claimSequence1);
                var createDate = _appealManager.GetSearchResultByColRow(12);
                createDate.IsDateInFormat().ShouldBeTrue("Is Create Date in proper format");
                var createDateToolTip = _appealManager.GetToolTipOfSearchResultByColRow(12);
                createDateToolTip.IsDateTimeInFormat().ShouldBeTrue("Is Create Date ToolTip is in Date Time Format?");
                Convert.ToDateTime(createDateToolTip).Date
                    .ShouldBeEqual(Convert.ToDateTime(createDate).Date, "Is Create Date value and Tooltip are same?");

                _appealManager.GetSearchResultByColRow(13)
                    .ShouldBeEqual("D", "Is Product Abbreviation for DCA Correct?");
                _appealManager.GetSearchResultByColRow(13, 2)
                    .ShouldBeEqual("U", "Is Product Abbreviation for FCI Correct?");
                _appealManager.GetSearchResultByColRow(13, 3)
                    .ShouldBeEqual("R", "Is Product Abbreviation for FFP Correct?");
                _appealManager.GetSearchResultByColRow(13, 4)
                    .ShouldBeEqual("F", "Is Product Abbreviation for CV Correct?");
                _appealManager.GetToolTipOfSearchResultByColRow(13)
                    .ShouldBeEqual(ProductEnum.DCA.ToString(), "ToolTip for DCA");
                _appealManager.GetToolTipOfSearchResultByColRow(13, 2)
                    .ShouldBeEqual(ProductEnum.FCI.ToString(), "ToolTip for FCI");
                _appealManager.GetToolTipOfSearchResultByColRow(13, 3)
                    .ShouldBeEqual(ProductEnum.FFP.ToString(), "ToolTip for FFP");
                _appealManager.GetToolTipOfSearchResultByColRow(13, 4)
                    .ShouldBeEqual(ProductEnum.CV.ToString(), "ToolTip for CV");

                #region CAR-3121(CAR-3063)
                StringFormatter.PrintMessage("Verify Search Result when Medical Record Review is selected");
                _appealManager.ClickOnClearLink();
                _appealManager.ClickOnAdvancedSearchFilterIcon(true);
                _appealManager.SelectDropDownListbyInputLabel("Client",ClientEnum.SMTST.ToString());
                _appealManager.SelectDropDownListbyInputLabel("Type", "Medical Record Review");
                _appealManager.SetDateFieldFrom("Create Date", "09/20/2020");
                _appealManager.SetDateFieldTo("Create Date", "09/26/2020");
                _appealManager.ClickOnFindButton();
                _appealManager.WaitForWorkingAjaxMessage();
                _appealManager.GetSearchResultListByCol(4).ShouldCollectionBeEquivalent(_appealManager.GetMedicalRecordReviewAppealsFromDatabase(), "Results should yield appeal records that are of the Medical Record Review type");
                #endregion
            }
        }

        [Test, Category("AppealManager1")] //US61458 + TE-107
        public void Verify_the_secondary_details_for_an_appeal()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealManagerPage _appealManager;
                AppealSummaryPage _appealSummary;

                automatedBase.CurrentPage = _appealManager = automatedBase.QuickLaunch.NavigateToAppealManager();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestExtensions.TestName);
                var claimSequence = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "ClaimSequence", "Value");
                _appealManager.SearchByClaimSequence(claimSequence);
                _appealManager.ClickOnSearchListRow(1);
                _appealManager.WaitForWorkingAjaxMessage();
                _appealManager.IsAppealDetailSectionOpen().ShouldBeTrue("Appeal Details section should be opened");

                _appealManager.GetAppealDetailsLabel(1, 1)
                    .ShouldBeEqual("Product:", "Appeal details Product label present");
                _appealManager.GetAppealDetailsContentValue(1, 1)
                    .ShouldBeEqual(paramLists["Product"], "Appeal details product value present");

                _appealManager.GetAppealDetailsLabel(1, 2)
                    .ShouldBeEqual("Flags:", "Appeal details Product label present");
                _appealManager.GetAppealDetailsContentValue(1, 2)
                    .ShouldBeEqual(paramLists["Flag"], "Appeal details product value present");

                _appealManager.GetAppealDetailsLabel(2, 1)
                    .ShouldBeEqual("Claim Count:", "Appeal details ClaimCount label present");
                _appealManager.GetAppealDetailsContentValue(2, 1)
                    .ShouldBeEqual(paramLists["ClaimCount"], "Appeal details Claim Count value present");

                _appealManager.GetAppealDetailsLabel(2, 2)
                    .ShouldBeEqual("Prov Seq:", "Appeal details Prov Seq label present");
                _appealManager.GetAppealDetailsContentValue(2, 2)
                    .ShouldBeEqual(paramLists["ProvSeq"], "Appeal details Prov Seq value present");

                _appealManager.GetAppealDetailsLabel(3, 1)
                    .ShouldBeEqual("Create Date:", "Appeal details Create Date label present");
                _appealManager.GetAppealDetailsContentValue(3, 1).IsDateInFormat()
                    .ShouldBeTrue("Create Date is in dd / mm / yyyy format");

                _appealManager.GetAppealDetailsLabel(3, 2)
                    .ShouldBeEqual("Created By:", "Appeal details Created By label present");
                _appealManager.GetAppealDetailsContentValue(3, 2).DoesNameContainsOnlyFirstWithLastname()
                    .ShouldBeTrue("Created By Should be in <First Name><Last Name>");

                _appealManager.GetAppealDetailsLabel(4, 1)
                    .ShouldBeEqual("Complete Date:", "Appeal details Complete Date label present");
                _appealManager.GetAppealDetailsContentValue(4, 1).IsDateInFormat()
                    .ShouldBeTrue("Create Date is in dd / mm / yyyy format");

                _appealManager.GetAppealDetailsLabel(4, 2)
                    .ShouldBeEqual("Assigned To:", "Appeal details Assigned To label present");
                _appealManager.GetAppealDetailsContentValue(4, 2)
                    .DoesNameContainsOnlyFirstWithLastname()
                    .ShouldBeTrue("Assigned To Should be in <First Name><Last Name>");

                _appealManager.GetAppealDetailsLabel(5, 1).ShouldBeEqual("Client Notes:",
                    "Appeal details Client Notes label present in 4th row");
                _appealManager.GetAppealDetailsContentValue(5, 1)
                    .ShouldBeEqual(paramLists["ClientNote"], "Client Notes should be correct");
                _appealSummary =
                    _appealManager.ClickOnAppealSequenceSwitchToAppealSummary(
                        _appealManager.GetSearchResultByColRow(4, 1));
                var clientNote = _appealSummary.GetAppealDetails(3, 1);

                _appealManager.CloseAnyPopupIfExistInAppealManager();
                _appealManager.GetAppealDetailsContentValue(5, 1).ShouldBeEqual(clientNote,
                    "Client Note displayed in appeal Detail should be same as that displayed in Appeal Summary page.");

                _appealManager.IsVerticalScrollBarPresentInClientNoteSection()
                    .ShouldBeFalse("Scroll Bar should not display for short note");

                _appealManager.ClickOnSearchListRow(2);

                _appealManager.IsVerticalScrollBarPresentInClientNoteSection()
                    .ShouldBeTrue("Scroll Bar should  display for lengthy note");
            }
        }

        
        [Test, Category("AppealManager1")]//US45657
        public void Verify_that_the_clear_filters_clears_all_filters()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealManagerPage _appealManager;
                automatedBase.CurrentPage = _appealManager = automatedBase.QuickLaunch.NavigateToAppealManager();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestExtensions.TestName);
                try
                {
                    _appealManager.ClickOnAdvancedSearchFilterIcon(true);
                    _appealManager.SelectAllAppeals();
                    _appealManager.SetInputFieldByInputLabel("Appeal Sequence", paramLists["AppealSequence"]);
                    _appealManager.SelectDropDownListbyInputLabel("Client", ClientEnum.SMTST.ToString());
                    _appealManager.SetInputFieldByInputLabel("Claim Sequence", paramLists["ClaimSequence"]);
                    _appealManager.SetInputFieldByInputLabel("Claim No", paramLists["ClaimNo"]);
                    _appealManager.SetInputFieldByInputLabel("Appeal #", paramLists["AppealNo"]);
                    _appealManager.SelectDropDownListbyInputLabel("Product", paramLists["Product"]);
                    _appealManager.SelectSearchDropDownListForMultipleSelectValue("Plan", paramLists["Plan"]);
                    _appealManager.SelectSearchDropDownListForMultipleSelectValue("Line Of Business",
                        paramLists["LineOfBusiness"]);
                    _appealManager.SelectDropDownListbyInputLabel("Status", paramLists["Status"]);
                    _appealManager.SelectSearchDropDownListForMultipleSelectValue("Primary Reviewer",
                        paramLists["PrimaryReviewer"]);
                    _appealManager.SelectSearchDropDownListForMultipleSelectValue("Assigned To",
                        paramLists["AssignedTo"]);
                    _appealManager.SelectSearchDropDownListForMultipleSelectValue("Category", paramLists["Category"]);
                    _appealManager.SelectDropDownListbyInputLabel("Type", paramLists["Type"]);
                    _appealManager.SelectDropDownListbyInputLabel("Priority", paramLists["Priority"]);
                    _appealManager.SetDateFieldFrom("Due Date", DateTime.Now.ToString("MM/d/yyyy"));
                    _appealManager.SetDateFieldFrom("Create Date", DateTime.Now.ToString("MM/d/yyyy"));
                    _appealManager.SetDateFieldFrom("Complete Date", DateTime.Now.ToString("MM/d/yyyy"));
                    _appealManager.SetInputFieldByInputLabel("First Deny Code", paramLists["FirstDenyCode"]);
                    _appealManager.SetInputFieldByInputLabel("First Pay Code", paramLists["FirstPayCode"]);
                    _appealManager.SetInputFieldByInputLabel("Provider Sequence", paramLists["ProviderSequence"]);
                    _appealManager.SelectDropDownListbyInputLabel("Appeal Level", paramLists["AppealLevel"]);
                    _appealManager.ClearAll();
                    StringFormatter.PrintMessageTitle("Verify Clear Filter clears all the search filters");
                    _appealManager.GetInputValueByLabel("Appeal Sequence").ShouldBeEqual("", "Appeal Sequence");
                    _appealManager.GetInputValueByLabel("Client").ShouldBeEqual("All", "Client");
                    _appealManager.GetInputValueByLabel("Claim Sequence").ShouldBeEqual("", "Claim Sequence");
                    _appealManager.GetInputValueByLabel("Claim No").ShouldBeEqual("", "Claim No");
                    _appealManager.GetInputValueByLabel("Appeal #").ShouldBeEqual("", "Appeal No");
                    _appealManager.GetInputValueByLabel("Product").ShouldBeEqual("All", "Product");
                    _appealManager.GetInputValueByLabel("Status").ShouldBeEqual("All", "Status");
                    _appealManager.GetInputValueByLabel("Primary Reviewer").ShouldBeEqual("", "Primary Reviewer");
                    _appealManager.GetInputValueByLabel("Assigned To").ShouldBeEqual("", "Assigned To");
                    _appealManager.GetInputValueByLabel("Category").ShouldBeEqual("", "Category");
                    _appealManager.GetInputValueByLabel("Type").ShouldBeEqual("All", "Type");
                    _appealManager.GetInputValueByLabel("Priority").ShouldBeEqual("All", "Priority");
                    _appealManager.GetDateFieldFrom("Due Date").ShouldBeEqual("", "Due Date From");
                    _appealManager.GetDateFieldTo("Due Date").ShouldBeEqual("", "Due Date To");
                    _appealManager.GetDateFieldFrom("Create Date").ShouldBeEqual("", "Create Date From");
                    _appealManager.GetDateFieldTo("Create Date").ShouldBeEqual("", "Create Date To");
                    _appealManager.GetDateFieldFrom("Complete Date").ShouldBeEqual("", "Complete Date From");
                    _appealManager.GetDateFieldTo("Complete Date").ShouldBeEqual("", "Complete Date To");
                    _appealManager.GetInputValueByLabel("First Deny Code").ShouldBeEqual("", "First Deny Code");
                    _appealManager.GetInputValueByLabel("First Pay Code").ShouldBeEqual("", "First Pay Code");
                    _appealManager.GetInputValueByLabel("Provider Sequence").ShouldBeEqual("", "Provider Sequence");
                    _appealManager.GetInputValueByLabel("Appeal Level").ShouldBeEqual("All", "Appeal Level");

                    StringFormatter.PrintMessageTitle("Verify Clear Filter does not work for Quick Search");
                    _appealManager.SelectAppealsDueToday();
                    _appealManager.ClearAll();
                    _appealManager.GetInputValueByLabel("Quick Search")
                        .ShouldBeEqual(AppealQuickSearchTypeEnum.AppealsDueToday.GetStringValue(), "Quick Search");

                }
                finally
                {
                    _appealManager.ClickOnAdvancedSearchFilterIcon(false);
                    _appealManager.ClearAll();
                }
            }
        }

        [Test, Category("AppealManager1")]//US61340
        public void Verify_proper_search_validation_with_real_time_field_validation()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealManagerPage _appealManager;
                automatedBase.CurrentPage = _appealManager = automatedBase.QuickLaunch.NavigateToAppealManager();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var product = paramLists["Product"];
                var client = ClientEnum.SMTST.ToString();
                var status = paramLists["Status"];
                var type = paramLists["Type"];
                var priority = paramLists["Priority"];
                var appealLevel = paramLists["AppealLevel"];
                var firstDenyCode = paramLists["FirstDenyCode"];
                var firstPayCode = paramLists["FirstPayCode"];
                var invalidAppealSeq = paramLists["InvalidAppealSeq"];
                try
                {
                    _appealManager.ClickOnAdvancedSearchFilterIcon(true);
                    _appealManager.SelectAllAppeals();
                    _appealManager.ClickOnFindButton();
                    _appealManager.WaitForWorkingAjaxMessage();
                    _appealManager.IsPageErrorPopupModalPresent()
                        .ShouldBeTrue("Popup Displayed when search criteria is not entered");
                    _appealManager.GetPageErrorMessage()
                        .ShouldBeEqual("Search cannot be initiated without any criteria entered.",
                            "Popup Message when search criteria is not entered");
                    _appealManager.ClosePageError();

                    ValidateFieldErrorMessageForComboBox("Product", product,
                        "Search cannot be initiated with Product only. A date range, Appeal Seq, Claim Seq, Appeal #, Provider Seq, or Reviewer search criteria is required.", _appealManager);

                    ValidateFieldErrorMessageForComboBox("Client", client,
                        "Search cannot be initiated with Client only. A date range, Status, Appeal Seq, Claim Seq, Appeal #, Provider Seq or Reviewer is required.", _appealManager);

                    ValidateFieldErrorMessageForComboBox("Status", status,
                        "Search cannot be initiated for all appeals in a status of Closed. Please select a different Status, a Date Range, Reviewer or Provider Seq.", _appealManager);

                    ValidateFieldErrorMessageForComboBox("Type", type,
                        "Search cannot be initiated with Type only. A date range, Client, Category, Status, Provider Seq, or Reviewer search criteria is required.", _appealManager);

                    ValidateFieldErrorMessageForComboBox("Priority", priority,
                        "Search cannot be initiated with Priority only. A date range, Client, Category, Status, Provider Seq, or Reviewer search criteria is required.", _appealManager);

                    ValidateFieldErrorMessageForComboBox("Appeal Level", appealLevel,
                        "Search cannot be initiated with Appeal Level only. A date range, Client, Category, Status, Provider Seq, or Reviewer search criteria is required.", _appealManager);

                    ValidateFieldErrorMessageForTextField("First Deny Code", firstDenyCode,
                        "Search cannot be initiated with First Deny Code only. A date range, Client, Category, Status, Provider Seq, or Reviewer search criteria is required.", _appealManager);

                    ValidateFieldErrorMessageForTextField("First Pay Code", firstPayCode,
                        "Search cannot be initiated with First Pay Code only. A date range, Client, Category, Status, Provider Seq, or Reviewer search criteria is required", _appealManager);

                    _appealManager.SetAppealSequence(invalidAppealSeq);
                    _appealManager.ClickOnFindButton();
                    _appealManager.WaitForWorkingAjaxMessage();
                    _appealManager.GetNoMatchingRecordFoundMessage()
                        .ShouldBeEqual("No matching records were found.", "No matching record found Message");

                    ValidateFieldErrorMessageForDateRange("Due Date",
                        "Search cannot be initiated for date ranges greater than 3 months.", _appealManager);
                    ValidateFieldErrorMessageForDateRange("Create Date",
                        "Search cannot be initiated for date ranges greater than 3 months.", _appealManager);
                    ValidateFieldErrorMessageForDateRange("Complete Date",
                        "Search cannot be initiated for date ranges greater than 3 months.", _appealManager);
                }
                finally
                {
                    _appealManager.ClickOnAdvancedSearchFilterIcon(false);
                }
            }
        }

        [Test, Category("AppealManager1")]//US61340 + us69626
        public void Verify_sorting_of_appeal_search_result_for_different_sort_options()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealManagerPage _appealManager;
                automatedBase.CurrentPage = _appealManager = automatedBase.QuickLaunch.NavigateToAppealManager();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var filterOptions =
                    automatedBase.DataHelper.GetMappingData(FullyQualifiedClassName, TestExtensions.TestName).Values
                        .ToList();
                var claimNo = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "ClaimNo", "Value");
                try
                {

                    _appealManager.ClickOnAdvancedSearchFilterIcon(true);

                    StringFormatter.PrintMessageTitle("Verify if due date are same urgent appeal should display first");
                    _appealManager.SearchByClaimNo(claimNo);
                    _appealManager.GetSearchResultByColRow(3)
                        .ShouldBeEqual(_appealManager.GetSearchResultByColRow(3, 2),
                            "Row 1 and Row 2 has same due date");
                    _appealManager.IsUrgentIconPresent(1)
                        .ShouldBeTrue("if due date are same urgent appeal should be listed first than normal");
                    _appealManager.IsUrgentIconPresent(2)
                        .ShouldBeFalse("second appeal having same due date has urgent icon");
                    _appealManager.ClearAll();

                    StringFormatter.PrintMessageTitle("Verify Sorting for differnt option");
                    _appealManager.SelectAllAppeals();
                    _appealManager.SetDateFieldFrom("Create Date", "02/1/2016");
                    _appealManager.SetDateFieldTo("Create Date", "03/1/2016");
                    //_appealManager.SelectSMTST();
                    _appealManager.SendEnterKeysOnTextFieldByLabel();
                    _appealManager.WaitForWorkingAjaxMessage();
                    _appealManager.GetAppealSearchResultRowCount()
                        .ShouldBeGreater(0, "Appeal Search Result found when hit <Enter> key");

                    _appealManager.IsListDateSortedInAscendingOrder(3)
                        .ShouldBeTrue("Due Date should Sorted in Ascending Order by default");

                    _appealManager.GetFilterOptionList().ShouldCollectionBeEqual(filterOptions, "Filter Options Lists");

                    ValidateAppealSearchRowSorted(5, 8, "Client", _appealManager);
                    ValidateAppealSearchRowSorted(7, 2, "Category", _appealManager);
                    ValidateAppealSearchRowSorted(8, 3, "Analyst", _appealManager);
                    ValidateAppealSearchRowSorted(9, 4, "First Deny Code", _appealManager);
                    ValidateAppealSearchRowSorted(11, 6, "Status", _appealManager);
                    ValidateAppealSearchRowSorted(12, 7, "Create Date", _appealManager);


                    _appealManager.ClickOnFilterOptionListRow(1);
                    _appealManager.IsListDateSortedInDescendingOrder(3)
                        .ShouldBeTrue("Record Should Be Sorted Descending Order By Due Date");
                    _appealManager.ClickOnFilterOptionListRow(1);
                    _appealManager.IsListDateSortedInAscendingOrder(3)
                        .ShouldBeTrue("Record Should Be Sorted Ascending Order By Due Date");

                    _appealManager.ClickOnFilterOptionListRow(5);
                    _appealManager.GetUrgentList()
                        .IsInAscendingOrder()
                        .ShouldBeTrue("Appeal Search Row should sorted by Priority");
                    _appealManager.ClickOnFilterOptionListRow(5);
                    _appealManager.GetUrgentList()
                        .IsInDescendingOrder()
                        .ShouldBeTrue("Appeal Search Row should sorted by Priority");

                }
                finally
                {
                    _appealManager.ClickOnAdvancedSearchFilterIcon(false);
                    _appealManager.ClickOnFilterOptionListRow(9);
                }
            }
        }

        [Test, Category("AppealManager1")]//US61340
        public void Verify_sidebar_icon_functionality_with_default_value_of_quiick_search_and_functionality_of_each_value_of_quick()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealManagerPage _appealManager;
                automatedBase.CurrentPage = _appealManager = automatedBase.QuickLaunch.NavigateToAppealManager();

                try
                {
                    _appealManager.RefreshPage();
                    _appealManager.IsSideBarPanelOpen()
                        .ShouldBeTrue("Find Appeal Panel will be open when user lands on the page initially.");
                    _appealManager.IsSideBarIconPresent().ShouldBeTrue("Sidebar Icon Present?");
                    _appealManager.ClickOnSidebarIcon(true);
                    _appealManager.WaitToOpenCloseWorkList();
                    _appealManager.IsSideBarPanelOpen()
                        .ShouldBeFalse("Find Appeal Panel should close when sidebar icon is clicked");
                    _appealManager.ClickOnSidebarIcon();
                    _appealManager.WaitToOpenCloseWorkList(false);
                    _appealManager.IsSideBarPanelOpen()
                        .ShouldBeTrue("Find Appeal Panel should be open when sidebar icon is clicked again.");

                    _appealManager.GetInputValueByLabel("Quick Search")
                        .ShouldBeEqual(AppealQuickSearchTypeEnum.AppealsDueToday.GetStringValue(),
                            "Defalut value of Quick Search");
                    _appealManager.GetSearchResultCount()
                        .ShouldBeEqual(0, "Search will not auto-not execute and search row should be empty");
                    _appealManager.GetAvailableDropDownList("Quick Search").ShouldNotContain("My Appeals",
                        "Appeal manager page doesn't have 'My appeals' search option.");

                    _appealManager.ClickOnAdvancedSearchFilterIcon(true);
                    Verify_search_result_for_different_value_of_quick_search(
                        AppealQuickSearchTypeEnum.AppealsDueToday.GetStringValue(), _appealManager);
                    Verify_search_result_for_different_value_of_quick_search(
                        AppealQuickSearchTypeEnum.AllUrgentAppeals.GetStringValue(), _appealManager);
                    Verify_search_result_for_different_value_of_quick_search(
                        AppealQuickSearchTypeEnum.AllRecordReviews.GetStringValue(), _appealManager);
                    Verify_search_result_for_different_value_of_quick_search(
                        AppealQuickSearchTypeEnum.OutstandingAppeals.GetStringValue(), _appealManager);

                }
                finally
                {
                    _appealManager.ClickOnAdvancedSearchFilterIcon(false);
                }
            }
        }

        [Test, Category("AppealManager1")] //US61339
        public void Verify_navigation_and_security_of_appeal_manager()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealManagerPage _appealManager;
                string _operationalManager = RoleEnum.OperationsManager.GetStringValue();
                CommonValidations _commonValidations = new CommonValidations(automatedBase.CurrentPage);

                automatedBase.CurrentPage = _appealManager = automatedBase.QuickLaunch.NavigateToAppealManager();

                _commonValidations.ValidateSecurityAndNavigationOfAPage(HeaderMenu.Manager,
                    new List<string> {SubMenu.AppealManager, SubMenu.AppealRationaleManager},
                    _operationalManager,

                    new List<string>
                    {
                        PageHeaderEnum.AppealManager.GetStringValue(),
                        PageHeaderEnum.AppealRationaleManager.GetStringValue()
                    },
                    automatedBase.Login.LoginAsUserHavingNoAnyAuthority, new[] {"Test4", "Automation4"});

                automatedBase.QuickLaunch = _appealManager.Logout().LoginAsClientUser();
                automatedBase.QuickLaunch.IsAppealManagerSubMenuPresent()
                    .ShouldBeFalse("Appeal Manager submenu present for client user.");
            }
        }

        [Test] //TE-887
        public void Verify_MentorAppeal_Opens_In_Appeal_Action()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealManagerPage _appealManager;
                AppealActionPage _appealAction;

                automatedBase.CurrentPage = _appealManager = automatedBase.QuickLaunch.NavigateToAppealManager();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestExtensions.TestName);
                var StatusList = paramLists["AppealStatus"].Split(',').ToList();
                _appealManager.GetSideBarPanelSearch.ClickOnAdvancedSearchFilterIcon(true);
                _appealManager.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search", "All Appeals");
                _appealManager.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Status",
                    AppealStatusEnum.MentorReview.GetStringDisplayValue());
                _appealManager.SelectSMTST();
                _appealManager.ClickOnFindButton();
                _appealManager.WaitForWorkingAjaxMessage();
                _appealManager.GetAppealSearchResultRowCount().ShouldBeGreater(0,
                    "Appeal with mentor review status should be populated in the grid");
                _appealManager.GetGridViewSection.GetValueInGridByColRow(11)
                    .ShouldBeEqual("Mentor", "correct appeal status displayed in primary data?");
                _appealAction = _appealManager.ClickOnAppealSequenceByStatusOrAppealToGoAppealAction("Mentor");
                automatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.AppealAction.GetStringValue(),
                    $"Is {PageHeaderEnum.AppealAction.GetStringValue()} page open?");
                _appealAction.GetAppealStatus().ShouldBeEqual(AppealStatusEnum.MentorReview.GetStringDisplayValue(),
                    "Status Should Match");
                _appealAction.CloseAnyPopupIfExist();
            }
        }

//        [NonParallelizable]
        [Test] //CAR-2869(CAR-2985)
        public void Validate_3_column_Appeal_Manager_Form()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealManagerPage _appealManager;
                
                automatedBase.CurrentPage = _appealManager = automatedBase.QuickLaunch.NavigateToAppealManager();
                List<string> _appealAssignableUserList = _appealManager.GetPrimaryReviewerAssignedToList();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestExtensions.TestName);
                var appealStatus = automatedBase.DataHelper.GetMappingData(FullyQualifiedClassName, "appeal_status")
                    .Values.ToList();
                var claimNo = paramLists["ClaimNo"];
                var primaryReviewer = paramLists["PrimaryReviewer"];
                var assignedToReviewer = paramLists["AssignedToReviewer"];
                try
                {
                    _appealManager.RefreshPage();
                    _appealManager.SearchByClaimNo(claimNo);
                    _appealManager.ClickOnHeaderEditIcon();

                    #region Validate Primary Reviewer and Assigned To Dropdown Fields

                    StringFormatter.PrintMessageTitle("Verifying the Primary Reviewer and Assigned To List");
                    _appealManager.GetSideWindow.IsDropDownPresentByLabel("Primary Reviewer")
                        .ShouldBeTrue("Is the Primary Reviewer dropdown field present?");
                    _appealManager.GetSideWindow.IsDropDownPresentByLabel("Assigned To")
                        .ShouldBeTrue("Is the Assigned To dropdown field present?");

                    StringFormatter.PrintMessage(
                        "Verifying dropdown values in Primary Reviewer and Assigned To fields");
                    var primaryReviewerList = _appealManager.GetSideBarPanelSearch
                        .GetAvailableDropDownList("Primary Reviewer")
                        .Where(s => s != "").ToList();
                    var assignedToList = _appealManager.GetSideBarPanelSearch.GetAvailableDropDownList("Assigned To")
                        .Where(s => s != "").ToList();

                    primaryReviewerList = primaryReviewerList.Select(s => s.Remove(s.LastIndexOf('(')).Trim()).ToList();
                    assignedToList = assignedToList.Select(s => s.Remove(s.LastIndexOf('(')).Trim()).ToList();
                    primaryReviewerList.ShouldCollectionBeEquivalent(_appealAssignableUserList.Select(s => s.Trim()),
                        "Primary Reviewer list should show correct analysts");
                    assignedToList.ShouldCollectionBeEquivalent(_appealAssignableUserList.Select(s => s.Trim()),
                        "Assigned To list should show correct analysts");

                    #endregion

                    #region Validate Status Dropdown Field

                    StringFormatter.PrintMessageTitle("Validating the Status dropdown field");
                    _appealManager.GetSideWindow.IsDropDownPresentByLabel("Status")
                        .ShouldBeTrue("Is the status dropdown field present?");

                    _appealManager.GetSideWindow.SelectDropDownValue("Status", "New");
                    _appealManager.GetSideWindow.SelectDropDownValue("Status", "Open");
                    _appealManager.GetSideWindow.GetInputValueByLabel("Status")
                        .ShouldBeEqual("Open", "User can select only 1 status option at a time");

                    StringFormatter.PrintMessage("Verifying the dropdown values in 'Status' fields");
                    var statusListInDropdown = _appealManager.GetSideBarPanelSearch.GetAvailableDropDownList("Status");
                    statusListInDropdown = statusListInDropdown.Where(s => s != "").ToList();
                    statusListInDropdown.ShouldCollectionBeEquivalent(appealStatus,
                        "Status dropdown should show all the status options");

                    #endregion

                    _appealManager.ClickOnCancelLink();
                    _appealManager.ClickOnHeaderEditIcon();
                    _appealManager.ClickOnSelectAllCheckBox();

                    #region Form Validation

                    StringFormatter.PrintMessageTitle("Verifying error popup when information is missing in the form");
                    _appealManager.SetNote("Test Info");
                    _appealManager.ClickOnSaveButton();
                    _appealManager.IsPageErrorPopupModalPresent().ShouldBeTrue(
                        "Error popup should be shown when the form is saved without entering either Status or Reviewer information");
                    _appealManager.GetPageErrorMessage()
                        .ShouldBeEqual("Missing data must be entered before the appeals can be updated.");
                    _appealManager.ClosePageError();

                    StringFormatter.PrintMessage(
                        "Verifying when only Primary Reviewer is entered and Assigned To is not entered");
                    _appealManager.SelectDropDownListbyInputLabel3Column("Primary Reviewer", primaryReviewer);
                    _appealManager.SelectDropDownListbyInputLabel3Column("Assigned To", "");
                    _appealManager.ClickOnSaveButton();
                    _appealManager.IsPageErrorPopupModalPresent()
                        .ShouldBeTrue(
                            "Error popup should be shown when only Primary Reviewer is selected and not Assigned To");
                    _appealManager.GetPageErrorMessage()
                        .ShouldBeEqual("Missing data must be entered before the appeals can be updated.");

                    _appealManager.ClosePageError();
                    _appealManager.SelectDropDownListbyInputLabel3Column("Status", "Open");
                    _appealManager.ClickOnSaveButton();
                    _appealManager.IsPageErrorPopupModalPresent()
                        .ShouldBeTrue(
                            "Error popup should be shown when only Primary Reviewer and Status is selected and not Assigned To");
                    _appealManager.GetPageErrorMessage()
                        .ShouldBeEqual("Missing data must be entered before the appeals can be updated.");
                    _appealManager.ClosePageError();

                    StringFormatter.PrintMessage(
                        "Verifying when only Assigned To is entered and Primary Reviewer is not entered");
                    _appealManager.SelectDropDownListbyInputLabel3Column("Assigned To", assignedToReviewer);
                    _appealManager.SelectDropDownListbyInputLabel3Column("Primary Reviewer", "");
                    _appealManager.ClickOnSaveButton();
                    _appealManager.IsPageErrorPopupModalPresent().ShouldBeTrue(
                        "Error popup should be shown when only Assigned To reviewer is selected and not Primary Reviewer");
                    _appealManager.GetPageErrorMessage()
                        .ShouldBeEqual("Missing data must be entered before the appeals can be updated.");
                    _appealManager.ClosePageError();

                    _appealManager.ClickOnCancelLink();

                    #endregion

                    #region Verify Saving the Form

                    StringFormatter.PrintMessageTitle(
                        "Verifying either Status or Primary Reviewer and Assigned To fields are entered");
                    _appealManager.ClickOnSearchListRow(1);

                    if (_appealManager.GetAppealDetailsValueByLabel("Assigned To") ==
                        assignedToReviewer.Split('(')[0].Trim())
                    {
                        SwapPrimaryAndAssignedToReviewers();
                    }

                    _appealManager.ClickOnHeaderEditIcon();
                    _appealManager.ClickOnSelectAllCheckBox();

                    _appealManager.SelectDropDownListbyInputLabel3Column("Primary Reviewer", primaryReviewer);
                    _appealManager.SelectDropDownListbyInputLabel3Column("Assigned To", assignedToReviewer);
                    _appealManager.ClickOnSaveButton();
                    _appealManager.WaitToReturnAppealManagerSearchPage();
                    _appealManager.CurrentPageUrl.ShouldBeEqual(automatedBase.CurrentPage.PageUrl,
                        "Page Should redirect to Appeal Manager Search Page when only Primary Reviewer and Assigned To are selected");
                    _appealManager.IsPageErrorPopupModalPresent()
                        .ShouldBeFalse("There should not be any popup present");

                    StringFormatter.PrintMessage("Verifying saving the form by entering only Status");
                    Random rnd = new Random();
                    var randomStatus = appealStatus[rnd.Next(appealStatus.Count)];
                    appealStatus.Remove(randomStatus);

                    _appealManager.ClickOnHeaderEditIcon();
                    _appealManager.ClickOnSelectAllCheckBox();

                    _appealManager.SelectDropDownListbyInputLabel3Column("Status", randomStatus);
                    _appealManager.ClickOnSaveButton();
                    _appealManager.WaitToReturnAppealManagerSearchPage();
                    _appealManager.CurrentPageUrl.ShouldBeEqual(automatedBase.CurrentPage.PageUrl,
                        "Page Should redirect to Appeal Manager Search Page when only Status is selected");
                    _appealManager.IsPageErrorPopupModalPresent()
                        .ShouldBeFalse("There should not be any popup present");

                    var totalRow = _appealManager.GetSearchResultCount();
                    for (var j = 0; j < totalRow; j++)
                    {
                        if (randomStatus == "Mentor Review")
                            _appealManager.GetGridViewSection.GetValueInGridByColRow(11, j + 1)
                                .ShouldBeEqual("Mentor", "Status should match");
                        else
                            _appealManager.GetGridViewSection.GetValueInGridByColRow(11, j + 1)
                                .ShouldBeEqual(randomStatus, "Status should match");
                        _appealManager.ClickOnSearchListRow(j + 1);
                        _appealManager.WaitForWorkingAjaxMessage();
                        _appealManager.GetAppealDetailsValueByLabel("Assigned To")
                            .ShouldBeEqual(assignedToReviewer.Split('(')[0].Trim(),
                                "Assigned To should remain unchanged");
                        _appealManager.GetSearchResultByColRow(8)
                            .ShouldBeEqual(primaryReviewer, "Primary Reviewer should remain unchanged");
                    }

                    StringFormatter.PrintMessageTitle(
                        "Verifying assigning appeal and changing status to only some of the appeals in bulk selection");
                    var appealList = _appealManager.GetSearchResultListByCol(4);
                    var originalPrimaryReviewer = primaryReviewer;
                    var originalAssignedTo = assignedToReviewer;
                    SwapPrimaryAndAssignedToReviewers();

                    _appealManager.ClickOnHeaderEditIcon();
                    _appealManager.ClickOnSelectAllCheckBox();
                    _appealManager.ClickOnSelectedAppealListIn3ColumnPageByAppeal(appealList[0]);

                    _appealManager.SelectDropDownListbyInputLabel3Column("Primary Reviewer", primaryReviewer);
                    _appealManager.SelectDropDownListbyInputLabel3Column("Assigned To", assignedToReviewer);
                    _appealManager.SelectDropDownListbyInputLabel3Column("Status", appealStatus[0]);
                    _appealManager.ClickOnSaveButton();

                    for (int count = 0; count < appealList.Count; count++)
                    {
                        var expectedStatus = count == 0 ? randomStatus : appealStatus[0];
                        var expectedPrimaryReviewer = count == 0
                            ? originalPrimaryReviewer
                            : primaryReviewer;
                        var expectedAssignedTo = count == 0
                            ? originalAssignedTo
                            : assignedToReviewer;

                        if (count == 0)
                            StringFormatter.PrintMessage(
                                "Verifying appeal 'Status' and 'Primary Reviewer/Assigned To' values for appeal which was not a part of bulk assignment");

                        if (expectedStatus == "Mentor Review")
                            _appealManager.GetGridViewSection.GetValueInGridByColRow(11, count + 1)
                                .ShouldBeEqual("Mentor",
                                    "For the appeal that was not selected in the bulk selection, the status should remain unchanged");
                        else
                            _appealManager.GetGridViewSection.GetValueInGridByColRow(11, count + 1)
                                .ShouldBeEqual(expectedStatus,
                                    "For the appeal that was not selected in the bulk selection, the status should remain unchanged");
                        _appealManager.ClickOnSearchListRowByAppealSequence(appealList[count]);
                        _appealManager.WaitForWorkingAjaxMessage();
                        _appealManager.GetAppealDetailsValueByLabel("Assigned To")
                            .ShouldBeEqual(expectedAssignedTo.Split('(')[0].Trim(), "Assigned To be correct");
                        _appealManager.GetSearchResultByColRow(8, count + 1)
                            .ShouldBeEqual(expectedPrimaryReviewer, "Primary Reviewer should be correct");
                    }

                    #endregion

                }
                finally
                {
                    if (_appealManager.IsPageErrorPopupModalPresent())
                    {
                        _appealManager.ClosePageError();
                        _appealManager.ClickOnCancelLink();
                    }

                    _appealManager.ClickOnSidebarIcon();
                }

                void SwapPrimaryAndAssignedToReviewers()
                {
                    var temp = assignedToReviewer;
                    assignedToReviewer = primaryReviewer;
                    primaryReviewer = temp;
                }
            }
        }

        [Test] //US58234+US69633 + CAR-2869(CAR-2985)
        public void
            Verify_that_there_will_be_no_error_when_reassigned_appeal_in_the_3_column_Appeal_Manager_and_should_redirected_to_Appeal_Manager_search_page()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealManagerPage _appealManager;
                automatedBase.CurrentPage = _appealManager = automatedBase.QuickLaunch.NavigateToAppealManager();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var claimNo = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "ClaimNo", "Value");
                var primaryReviewer = "Test Automation (uiautomation)";
                var assignedTo = "ui automation_1 (uiautomation_1)";
                var appealStatus = automatedBase.DataHelper.GetMappingData(FullyQualifiedClassName, "appeal_status")
                    .Values.ToList();
                try
                {
                    _appealManager.RefreshPage();
                    _appealManager.IsModifyAppealsIconDisabled()
                        .ShouldBeTrue("Initially Modify Appeals icon must be disabled");
                    _appealManager.SearchByClaimNo(claimNo);
                    _appealManager.IsModifyAppealsIconEnabled()
                        .ShouldBeTrue("Modify Appeals icon must be enabled when search result is displayed.");
                    var totalRow = _appealManager.GetSearchResultCount();

                    _appealManager.ClickOnSearchListRow(1);
                    _appealManager.WaitForWorkingAjaxMessage();
                    //changed the assigned to's value if it is same to existing assigned to's value so we can make updates with change
                    if (_appealManager.GetAppealDetailsValueByLabel("Assigned To") == assignedTo.Split('(')[0].Trim())
                    {
                        var temp = assignedTo;
                        assignedTo = primaryReviewer;
                        primaryReviewer = temp;
                    }

                    _appealManager.ClickOnSidebarIcon();
                    _appealManager.IsHeaderEditIconPresent().ShouldBeTrue("Is Edit Multiple Icon Present?");
                    for (var i = 0; i < 3; i++)
                    {
                        StringFormatter.PrintMessageTitle(string.Format("Reassigment attempt=<{0}>", i + 1));
                        _appealManager.ClickOnHeaderEditIcon();
                        if (i == 0)
                        {
                            var selectAppealList = _appealManager.GetUnSelectedListIn3ColumnPage();
                            _appealManager.ClickOnSelectAllCheckBox();
                            _appealManager.GetSelectedListIn3ColumnPage("Select Appeals")
                                .ShouldCollectionBeEqual(selectAppealList, "Is All Appeal Selected?");
                            _appealManager.GetSelectedListIn3ColumnPage("Selected Appeals")
                                .ShouldCollectionBeEqual(selectAppealList, "Is All Appeal Selected?");
                            _appealManager.ClickOnSelectAllCheckBox(false);
                            _appealManager.GetSelectedListIn3ColumnPage("Selected Appeals")
                                .ShouldCollectionBeEmpty("Is Selected List Removed from Selected Appeals");
                            _appealManager.ClickOnSelectAppealListIn3ColumnPageByRow();
                            _appealManager.GetSelectedListIn3ColumnPage("Select Appeals")
                                .ShouldCollectionBeEqual(
                                    _appealManager.GetSelectedListIn3ColumnPage("Selected Appeals"),
                                    "Is Single Appeal Selected?");
                            _appealManager.ClickOnSelectedAppealListIn3ColumnPageByRow();
                            _appealManager.GetSelectedListIn3ColumnPage("Selected Appeals")
                                .ShouldCollectionBeEmpty("Is Selected List Removed from Selected Appeals");
                        }

                        Random rnd = new Random();
                        var newStatus = appealStatus[rnd.Next(appealStatus.Count)];
                        appealStatus.Remove(newStatus);

                        _appealManager.ClickOnSelectAllCheckBox();

                        //CAR-2869(CAR-2985)
                        StringFormatter.PrintMessage("Randomly assigning a status to the appeals in bulk");
                        _appealManager.GetSideWindow.SelectDropDownValue("Status", newStatus);

                        _appealManager.SelectDropDownListbyInputLabel3Column("Primary Reviewer", primaryReviewer);
                        var pr = _appealManager.GetInputValueByLabel("Primary Reviewer");
                        _appealManager.GetInputValueByLabel("Assigned To").ShouldBeEqual(
                            _appealManager.GetInputValueByLabel("Primary Reviewer"),
                            "Assigned To field must be auto populated with same selection as of Primary Reviewer.");
                        _appealManager.SelectDropDownListbyInputLabel3Column("Assigned To", assignedTo);
                        _appealManager.SetNote("UITEST");
                        _appealManager.GetNote().ShouldBeEqual("UITEST", "Is Note Editable?");
                        _appealManager.ClickOnSaveButton();
                        _appealManager.WaitToReturnAppealManagerSearchPage();
                        _appealManager.CurrentPageUrl.ShouldBeEqual(automatedBase.CurrentPage.PageUrl,
                            "Page Should redirect to Appeal Manager Search Page");
                        _appealManager.IsPageErrorPopupModalPresent()
                            .ShouldBeFalse("There should not be any popup present");
                        for (var j = 0; j < totalRow; j++)
                        {

                            if (newStatus == "Mentor Review")
                                _appealManager.GetGridViewSection.GetValueInGridByColRow(11, j + 1)
                                    .ShouldBeEqual("Mentor", "");
                            else
                                _appealManager.GetGridViewSection.GetValueInGridByColRow(11, j + 1)
                                    .ShouldBeEqual(newStatus, "");
                            _appealManager.ClickOnSearchListRow(j + 1);
                            _appealManager.WaitForWorkingAjaxMessage();
                            _appealManager.GetAppealDetailsValueByLabel("Assigned To")
                                .ShouldBeEqual(assignedTo.Split('(')[0].Trim(), "Assigned To Should get updated");
                            _appealManager.GetSearchResultByColRow(8)
                                .ShouldBeEqual(primaryReviewer, "Primary Reviewer Should get updated");
                        }

                        _appealManager.ClickOnSidebarIcon();
                        //changed the assigned to in order to verify update
                        var temp = assignedTo;
                        assignedTo = primaryReviewer;
                        primaryReviewer = temp;
                    }
                }

                finally
                {
                    if (_appealManager.IsPageErrorPopupModalPresent())
                    {
                        _appealManager.ClosePageError();
                        _appealManager.ClickOnCancelLink();
                    }

                    _appealManager.ClickOnSidebarIcon();
                }
            }
        }

        [Test,Category("Upload_Document")]//US58220
        public void Verify_document_upload_is_allowed_for_appeal_in_any_status()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealManagerPage _appealManager;

                automatedBase.CurrentPage = _appealManager = automatedBase.QuickLaunch.NavigateToAppealManager();
                FileUploadPage FileUploadPage = _appealManager.GetFileUploadPage;

                TestExtensions.TestName
                    = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> testData = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestExtensions.TestName);
                var claimSeq = testData["ClaimSequence"];
                var appealSeq = testData["AppealSequence"].Split(';');
                var statusCompleteClosed = new List<String> {"Complete", "Closed"};
                var fileToUpload = testData["FileToUpload"];
                var fileToVerify = testData["FileNameValidation"].Split(';');
                var fileType = testData["FileType"];
                var docDescription = testData["DocDescription"];
                var expectedSelectedFileTypeList = testData["SelectedFileList"].Split(',').ToList();
                var expectedFileTypeList = automatedBase.DataHelper
                    .GetMappingData(FullyQualifiedClassName, "File_Type_List").Values.ToList();

                try
                {
                    _appealManager.SearchByClaimSequence(claimSeq);
                    _appealManager.GetSearchResultCount().ShouldBeGreaterOrEqual(1, "Search result is present");
                    _appealManager.ClickOnSearchListRowByAppealSequence(appealSeq[0]);
                    _appealManager.IsAppealDetailSectionOpen().ShouldBeTrue("Appeal details is Open");
                    _appealManager.IsNoDataMessagePresent()
                        .ShouldBeTrue("For no documents case, no data message is present");
                    _appealManager.GetNoDataMessageText().ShouldBeEqual("No Data Available", "No doucment message");

                    _appealManager.ClickOnSearchListRowByAppealSequence(appealSeq[1]);
                    var currStatus = _appealManager.GetSearchResultByAppealSeq(appealSeq[1], 11);
                    StringFormatter.PrintMessageTitle(string.Format("Verify Add/Delete Document Icon for status <{0}>",
                        currStatus));
                    _appealManager.IsAppealDocumentUploadEnabled()
                        .ShouldBeFalse("Appeal Document Upload Should be Disabled for" + currStatus);
                    _appealManager.IsAppealDocumentDeleteDisabled()
                        .ShouldBeTrue("Appeal Document Delete Should be Disabled for" + currStatus);

                    StringFormatter.PrintMessageTitle("Changed the status and verify Add/Delete Document Icon.");
                    var newStatus = (currStatus.Equals(statusCompleteClosed[0])
                        ? statusCompleteClosed[1]
                        : statusCompleteClosed[0]);
                    _appealManager.ChangeStatusOfListedAppeal(newStatus, appealSeq[1]);
                    //Due to Audit Update Issue we have to click other row
                    _appealManager.ClickOnSearchListRowByAppealSequence(appealSeq[0]);
                    _appealManager.ClickOnSearchListRowByAppealSequence(appealSeq[1]);
                    _appealManager.IsAppealDocumentUploadEnabled()
                        .ShouldBeFalse("Appeal Document Upload Should be Disabled for" + newStatus);
                    _appealManager.IsAppealDocumentDeleteDisabled()
                        .ShouldBeTrue("Appeal Document Delete Should be Disabled for" + newStatus);

                    FileUploadPage.AppealDocumentsListAttributeValue(1, 1, 2)
                        .ShouldBeEqual(fileToVerify[0], "Document filename is displayed");
                    FileUploadPage.AppealDocumentsListAttributeValue(1, 1, 3)
                        .ShouldBeEqual(fileType, "Document fileType is displayed");
                    FileUploadPage.GetAppealDocumentAttributeToolTip(1, 1, 3)
                        .ShouldBeEqual(fileType, "Document fileType tooltip is displayed");
                    FileUploadPage.AppealDocumentsListAttributeValue(1, 1, 4)
                        .IsDateTimeInFormat()
                        .ShouldBeTrue("Document date is displayed and in proper format");
                    FileUploadPage.AppealDocumentsListAttributeValue(1, 2, 2)
                        .ShouldBeEqual(docDescription, "Document Description is displayed");

                    StringFormatter.PrintMessageTitle(
                        "Verify Upload Docuemnt for the status other than complete or closed");

                    _appealManager.ClickOnSearchListRowByAppealSequence(appealSeq[2]);
                    _appealManager.IsNoDataMessagePresent()
                        .ShouldBeFalse("For presence of documents case, no data message should not be shown");
                    FileUploadPage.IsAppealDocumentUploadEnabled()
                        .ShouldBeTrue("Appeal Document Upload Should be Enabled for non complete or unclosed");
                    FileUploadPage.GetAppealDocumentUploadIconToolTip()
                        .ShouldBeEqual("Upload Appeal Document", "Expected appeal upload icon tooltip is present");

                    Console.WriteLine("Verify Maximum character in description");
                    FileUploadPage.ClickOnAppealDocumentUploadIcon();
                    FileUploadPage.IsDocumentUploadSectionPresent()
                        .ShouldBeTrue("Appeal Document Uploader Section is displayed");
                    var descp = new string('a', 105);
                    FileUploadPage.SetFileUploaderFieldValue("Description", descp);
                    FileUploadPage.GetFileUploaderFieldValue(3)
                        .Length.ShouldBeEqual(100, "Character length should not exceed more than 100 in Description");

                    FileUploadPage.IsAddFileButtonDisabled()
                        .ShouldBeTrue(
                            "Add file button should be disabled (unless atleast a file has been uploaded) must be true?");
                    FileUploadPage.AddFileForUpload(fileToUpload.Split(';')[0]);
                    FileUploadPage.IsAddFileButtonDisabled()
                        .ShouldBeFalse(
                            "Add file button should be enabled (when atleast a file has been uploaded) must be false?");

                    FileUploadPage.ClickOnAddFileBtn();
                    _appealManager.IsPageErrorPopupModalPresent()
                        .ShouldBeTrue("Page error pop up should be present if no file type is selected");
                    _appealManager.GetPageErrorMessage()
                        .ShouldBeEqual(
                            "At least one Document Type selection is required before the files can be added.",
                            "Expected error message on zero file type selection");
                    _appealManager.ClosePageError();

                    FileUploadPage.SetFileUploaderFieldValue("Description", " ");
                    FileUploadPage.GetAvailableFileTypeList()
                        .ShouldCollectionBeEqual(expectedFileTypeList, "File Type List Equal");
                    FileUploadPage.SetFileTypeListVlaue(expectedSelectedFileTypeList[0]);
                    FileUploadPage.GetPlaceHolderValue().ShouldBeEqual("Provider Letter", "File Type Text");
                    FileUploadPage.SetFileTypeListVlaue(expectedSelectedFileTypeList[1]);
                    FileUploadPage.GetPlaceHolderValue()
                        .ShouldBeEqual("Multiple values selected", "File Type Text when multiple value selected");
                    FileUploadPage.GetSelectedFileTypeList()
                        .ShouldCollectionBeEqual(expectedSelectedFileTypeList, "Selected File List Equal");
                    FileUploadPage.ClickOnAddFileBtn();
                    _appealManager.IsPageErrorPopupModalPresent()
                        .ShouldBeFalse(
                            "Page error pop up should not be present if no description is set as its not a required field");
                    /* ---------Comment the below part  since multiple file upload with selenium is successfull locally only -----*/
                    //FileUploadPage.AddFileForUpload(fileToUpload.Split(';'));
                    //FileUploadPage.GetSelectedFilesValue()
                    //    .ShouldCollectionBeEqual(fileToUpload.Split(';').ToList(), "Expected multiples files list is present");
                }
                finally
                {
                    automatedBase.CurrentPage.NavigateToAppealManager();
                }
            }
        }

        [Test, Category("Upload_Document")] //US58220 part 2
        public void Verify_document_upload_and_delete_is_allowed_for_appeal_in_any_status()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealManagerPage _appealManager;
                automatedBase.CurrentPage = _appealManager = automatedBase.QuickLaunch.NavigateToAppealManager();

                FileUploadPage FileUploadPage = _appealManager.GetFileUploadPage;

                TestExtensions.TestName
                    = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> testData = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestExtensions.TestName);
                var claimSeq = testData["ClaimSequence"];
                var appealSeq = testData["AppealSequence"];
                var statusCompleteClosed = new List<String> {"Complete", "Closed"};
                var fileToUpload = testData["FileToUpload"].Split(';');
                var fileType = testData["SelectedFileList"];
                var appealstatusList = automatedBase.DataHelper.GetMappingData(FullyQualifiedClassName, "appeal_status")
                    .Values.ToList();


                _appealManager.SearchByClaimSequence(claimSeq);
                _appealManager.ClickOnSearchListRowByAppealSequence(appealSeq);
                var currStatusSeq = _appealManager.GetSearchResultByAppealSeq(appealSeq, 11);
                _appealManager.IsAppealDocumentUploadEnabled()
                    .ShouldBeTrue("Appeal Document Upload Should be Enabled for" + currStatusSeq + " appeals");
                _appealManager.IsAppealDocumentDeleteDisabled()
                    .ShouldBeFalse("Appeal Document Delete Should not be Disabled for" + currStatusSeq + " appeals");
                appealstatusList.Remove(statusCompleteClosed[0]);
                appealstatusList.Remove(statusCompleteClosed[1]);
                appealstatusList.Remove(currStatusSeq);
                foreach (var appealStats in appealstatusList)
                {
                    _appealManager.ChangeStatusOfListedAppeal(appealStats, appealSeq);
                    _appealManager.IsAppealDocumentUploadEnabled()
                        .ShouldBeTrue("Appeal Document Upload Should be Enabled for" + appealStats + " appeals");
                    _appealManager.IsAppealDocumentDeleteDisabled()
                        .ShouldBeFalse("Appeal Document Delete Should not be Disabled for" + appealStats + " appeals");
                }
                if (FileUploadPage.IsFollowingAppealDocumentPresent(fileToUpload[0]))
                {
                    Console.WriteLine("delete document if previously run test failed to delete uploaded document");
                    FileUploadPage.ClickOnDeleteFileBtn(fileToUpload[0]);
                    _appealManager.ClickOkCancelOnConfirmationModal(true);
                }

                var existingDocCount = FileUploadPage.DocumentCountOfFileList();

                /*validation of add file (bug awaiting to be resolved */
                //FileUploadPage.AddNoFileForUpload();
                //FileUploadPage.ClickOnAddFileBtn();
                //_appealManagerPage.IsPageErrorPopupModalPresent().ShouldBeTrue("Page error pop up should be present if no file is selected");
                //_appealManagerPage.GetPageErrorMessage()
                //    .ShouldEqual(noDocErrorMessage, "Expected error message on no file selection");
                //_appealManagerPage.ClosePageError();

                try
                {
                    FileUploadPage.ClickOnAppealDocumentUploadIcon();
                    FileUploadPage.IsDocumentUploadSectionPresent()
                        .ShouldBeTrue("Appeal Document Uploader Section is displayed");
                    FileUploadPage.SetFileTypeListVlaue(fileType);
                    FileUploadPage.AddFileForUpload(fileToUpload[0]);
                    FileUploadPage.ClickOnAddFileBtn();
                    FileUploadPage.FileToUploadDocumentValue(1, 2)
                        .ShouldBeEqual(fileToUpload[0], "Document correct and present under files to upload");
                    FileUploadPage.ClickOnCancelBtn();
                    //selecting cancel closed form and discards added files 
                    FileUploadPage.IsDocumentDivPresent().ShouldBeTrue("Document List are present");

                    FileUploadPage.DocumentCountOfFileList()
                        .ShouldBeEqual(existingDocCount,
                            "Added Files has been discarded and has not been associated to appeal");
                    FileUploadPage.ClickOnAppealDocumentUploadIcon();
                    FileUploadPage.IsDocumentUploadSectionPresent()
                        .ShouldBeTrue("Appeal Document Uploader Section is displayed");
                    FileUploadPage.SetFileTypeListVlaue(fileType);
                    FileUploadPage.AddFileForUpload(fileToUpload[0]);
                    FileUploadPage.ClickOnAddFileBtn();
                    FileUploadPage.ClickOnSaveUploadBtn();
                    FileUploadPage.IsDocumentDivPresent().ShouldBeTrue("Document List are present");
                    FileUploadPage.DocumentCountOfFileList()
                        .ShouldBeGreater(existingDocCount, "New document has been added");
                    FileUploadPage.IsFollowingAppealDocumentPresent(fileToUpload[0])
                        .ShouldBeTrue("Uploaded Document is listed");
                    existingDocCount = FileUploadPage.DocumentCountOfFileList();

                    //awaiting bug resolve of save functionality
                    //updatedDueDate.ShouldNotEqual(dueDate,
                    //    "Due date " + updatedDueDate +
                    //    " should be based on document upload date and updated due date equal to old " + dueDate +
                    //    " should be false.");
                    //_appealSummary.GetAppealDetails(1, 4).ShouldEqual("New", "Status should be udpated to new");
                    //FileUploadPage.GetAddedAppealDocumentList().IsInAscendingOrder().ShouldBeTrue("Most recenet uploaded files are on top");

                    currStatusSeq = _appealManager.GetSearchResultByAppealSeq(appealSeq, 11);
                    FileUploadPage.ClickOnDeleteFileBtn(fileToUpload[0]);

                    if (_appealManager.IsPageErrorPopupModalPresent())
                        _appealManager.GetPageErrorMessage()
                            .ShouldBeEqual("The selected document will be deleted. Do you wish to proceed?");
                    _appealManager.ClickOkCancelOnConfirmationModal(false);

                    FileUploadPage.DocumentCountOfFileList()
                        .ShouldBeEqual(existingDocCount, "All the two documents are still entact and listed");
                    FileUploadPage.ClickOnDeleteFileBtn(fileToUpload[0]);

                    if (_appealManager.IsPageErrorPopupModalPresent())
                        _appealManager.GetPageErrorMessage()
                            .ShouldBeEqual("The selected document will be deleted. Do you wish to proceed?");
                    _appealManager.ClickOkCancelOnConfirmationModal(false);

                    FileUploadPage.DocumentCountOfFileList()
                        .ShouldBeEqual(existingDocCount, "All the two documents are still entact and listed");
                    FileUploadPage.ClickOnDeleteFileBtn(fileToUpload[0]);
                    _appealManager.ClickOkCancelOnConfirmationModal(true);
                    FileUploadPage.DocumentCountOfFileList()
                        .ShouldBeLessOrEqual(existingDocCount, "Deleted document hasnt been listed.");
                    FileUploadPage.IsFollowingAppealDocumentPresent(fileToUpload[0])
                        .ShouldBeFalse("Deleted Document isn't present");
                    _appealManager.GetSearchResultByAppealSeq(appealSeq, 11)
                        .ShouldBeEqual(currStatusSeq,
                            "Status should not be changed and should be same after deleting file");
                }
                finally
                {
                    if (FileUploadPage.IsFollowingAppealDocumentPresent(fileToUpload[0]))
                    {
                        FileUploadPage.ClickOnDeleteFileBtn(fileToUpload[0]);
                        _appealManager.ClickOkCancelOnConfirmationModal(true);
                    }
                }
            }
        }

        [Test] //us58233 to do awaiting DE5617 story to complete
        public void Verify_product_update_of_an_appeal_is_allowed()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealManagerPage _appealManager;
                AppealActionPage _appealAction;

                automatedBase.CurrentPage = _appealManager = automatedBase.QuickLaunch.NavigateToAppealManager();
                List<string> _getActiveProductList = _appealManager.GetCommonSql.GetExpectedProductListFromDatabase(ClientEnum.SMTST.ToString());

                TestExtensions.TestName
                    = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> testData = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestExtensions.TestName);
                var claimSeq = testData["ClaimSequence"];
                var appealSeq = testData["AppealSequence"];
                var product = testData["Product"].Split(';');
                var productMappingData =
                    automatedBase.DataHelper.GetMappingData(FullyQualifiedClassName, "Product_Type_List");

                
                _getActiveProductList.Sort();
                var activeProducts = _getActiveProductList.Select(s =>
                    s.Replace("FraudFinder Pro", ProductEnum.FFP.GetStringValue())).ToList();

                AppealProcessingHistoryPage appealProcessingHistory = null;
                try
                {
                    _appealManager.SearchByClaimSequence(claimSeq);
                    _appealManager.ClickOnSearchListRowByAppealSequence(appealSeq);
                    var currProduct = _appealManager.GetSearchResultByColRow(13);
                    var pciTrue = currProduct.Equals("F");

                    _appealManager.ClickOnEditIconByAppealRowSelector(appealSeq);
                    _appealManager.GetAvailableDropDownListInEditAppealByLabel("Product")
                        .ShouldCollectionBeEquivalent(activeProducts, "Product list is as expected?");

                    _appealManager.GetInputValueByLabel("Product")
                        .ShouldBeEqual(productMappingData[currProduct],
                            "Default value is equal to the Current Product value");

                    var newProductValue = (!pciTrue) ? product[0] : product[1];
                    _appealManager.SelectDropDownListbyInputLabel("Product", (!pciTrue) ? product[1] : product[0]);
                    _appealManager.SelectDropDownListbyInputLabel("Product", newProductValue);

                    _appealManager.SelectedDropDownOptionsByLabel("Product")
                        .ShouldNotBeEqual((!pciTrue) ? product[1] : product[0], "Only single value can be selected.");
                    _appealManager.GetInputValueByLabel("Product")
                        .ShouldBeEqual(newProductValue, "Only single value can be selected.");

                    _appealManager.ClickOnCancelLink();
                    _appealManager.IsEditFormDisplayed().ShouldBeFalse("Edit form panel should close.");
                    _appealManager.GetSearchResultByColRow(13)
                        .ShouldBeEqual(currProduct,
                            "No changes made and record hasnt been updated when cancel is clicked.");
                    _appealManager.ClickOnEditIconByAppealRowSelector(appealSeq);
                    _appealManager.SelectDropDownListbyInputLabel("Product", newProductValue);

                    _appealManager.ClickOnSaveButton();
                    _appealManager.WaitForWorkingAjaxMessage();
                    _appealManager.IsEditFormDisplayed().ShouldBeFalse("Edit form panel should close.");

                    _appealManager.GetSearchResultByColRow(13)
                        .ShouldNotBeEqual(currProduct,
                            "Changes has been made and record has been updated when cancel is clicked.");
                    automatedBase.CurrentPage =
                        _appealAction = _appealManager.ClickOnAppealSequenceSwitchToAppealAction(appealSeq);
                    _appealAction.ClickMoreOption();
                    appealProcessingHistory = _appealAction.ClickAppealProcessingHx();
                    var n = 1;
                    do
                    {
                        if (appealProcessingHistory.GetAppealAuditGridTableDataValue(n, 3) == "Edit")
                            break;
                        n++;
                    } while (n < 5);

                    appealProcessingHistory.GetAppealAuditGridTableDataValue(n, 3)
                        .ShouldBeEqual("Edit", "Action Should be Edit");
                }
                finally
                {
                    _appealManager.CloseAnyPopupIfExist();
                    if (_appealManager.IsPageErrorPopupModalPresent())
                        _appealManager.ClosePageError();
                }
            }
        }

        [Test] //US65696
        public void Verify_appeal_details_section_is_updated_after_changing_status_to_Complete()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealManagerPage _appealManager;
                automatedBase.CurrentPage = _appealManager = automatedBase.QuickLaunch.NavigateToAppealManager();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestExtensions.TestName);
                var claimseq = paramLists["ClaimSequence"];
                try
                {
                    _appealManager.SearchByClaimSequence(claimseq);
                    var appealSeq = _appealManager.GetSearchResultByColRow(4);
                    _appealManager.ClickOnSearchListRowByAppealSequence(appealSeq);
                    _appealManager.ClickOnEditIconByAppealRowSelector(appealSeq);
                    StringFormatter.PrintMessage("Change appeal status to Complete");
                    _appealManager.ChangeAppealStatus(AppealStatusEnum.Complete.GetStringDisplayValue());
                    _appealManager.GetAppealDetailsContentValue("Complete Date")
                        .ShouldBeEqual(DateTime.Now.ToString("MM/dd/yyyy"),
                            "Appeal details must display updated Complete date");
                    StringFormatter.PrintMessage("Change appeal status to back to Open");
                    _appealManager.ClickOnEditIconByAppealRowSelector(appealSeq);
                    _appealManager.ChangeAppealStatus(AppealStatusEnum.Open.GetStringDisplayValue());
                }
                finally
                {
                    _appealManager.CloseAnyPopupIfExistInAppealManager();

                }
            }
        }

        [Test] //us61343
        public void Validate_delete_functionality_for_appeals()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealManagerPage _appealManager;
                AppealCreatorPage _appealCreator;
                automatedBase.CurrentPage = _appealManager = automatedBase.QuickLaunch.NavigateToAppealManager();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var claimSeq = paramLists["ClaimSequence"];

                try
                {

                    _appealManager.SearchByClaimSequence(claimSeq);
                    if (_appealManager.GetSearchResultCount() != 0 && !_appealManager.IsNoDataMessagePresent())
                    {

                        automatedBase.CurrentPage =
                            _appealManager = automatedBase.CurrentPage.NavigateToAppealManager();
                        _appealManager.DeleteAppealsAssociatedWithClaim(claimSeq); //to delete appeal created
                        automatedBase.CurrentPage = _appealCreator = _appealManager.NavigateToAppealCreator();
                        _appealCreator.SearchByClaimSequence(claimSeq);
                    }

                    automatedBase.CurrentPage = _appealCreator = _appealManager.NavigateToAppealCreator();
                    _appealCreator.SearchByClaimSequence(claimSeq);
                    if (_appealCreator.IsClaimLocked() && _appealCreator.GetPageHeader() == "Claim Search")
                    {
                        automatedBase.CurrentPage =
                            _appealManager = automatedBase.CurrentPage.NavigateToAppealManager();
                        _appealManager.DeleteAppealsAssociatedWithClaim(claimSeq); //to delete appeal created
                        automatedBase.CurrentPage = _appealCreator = _appealManager.NavigateToAppealCreator();
                        _appealCreator.SearchByClaimSequence(claimSeq);
                    }

                    CreateAppeal(_appealCreator);
                    automatedBase.CurrentPage = _appealManager = automatedBase.CurrentPage.NavigateToAppealManager();
                    _appealManager.ChangeStatusOfAppeal(claimSeq, null, "Complete");
                    automatedBase.CurrentPage = _appealCreator = _appealManager.NavigateToAppealCreator();
                    _appealCreator.SearchByClaimSequence(claimSeq);
                    CreateAppeal(_appealCreator);
                    automatedBase.CurrentPage = _appealManager = automatedBase.CurrentPage.NavigateToAppealManager();
                    _appealManager.SearchByClaimSequence(claimSeq);

                    var appealList = _appealManager.GetSearchResultListByCol(4);

                    _appealManager.ClickOnDeleteIconByRowSelector(1);
                    _appealManager.GetPageErrorMessage()
                        .ShouldBeEqual(
                            "The selected appeal will be permanently deleted. Click Ok to proceed or Cancel.");
                    _appealManager.ClickOkCancelOnConfirmationModal(false);
                    _appealManager.GetSearchResultCount()
                        .ShouldBeEqual(appealList.Count, "The record hasnot been deleted.");

                    Console.WriteLine("Deleting the appeal hence created for the test");
                    _appealManager.ClickOnDeleteIconByRowSelector(1);
                    _appealManager.ClickOkCancelOnConfirmationModal(true);
                    _appealManager.GetSearchResultCount()
                        .ShouldBeEqual(appealList.Count - 1, "The record has been deleted.");

                    Console.WriteLine("Deleting the second appeal hence created for the test");
                    _appealManager.ClickOnDeleteIconByRowSelector(1);
                    _appealManager.ClickOkCancelOnConfirmationModal(true);
                    _appealManager.GetSearchResultCount()
                        .ShouldBeEqual(appealList.Count - 2, "The record has been deleted.");

                    _appealManager.GetCountOfDeletedAppealByAppealSequenceList(appealList)
                        .ShouldBeEqual(appealList.Count.ToString(),
                            "Deleted Appeal Should exist in database as status=<R>");
                }
                finally
                {
                    if (_appealManager.IsPageErrorPopupModalPresent())
                        _appealManager.ClosePageError();

                }
            }
        }


        [Test, Category("Working")] //US61999 + TE-887
        public void Validation_of_advance_search_filters_on_appeal_search_panel()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealManagerPage _appealManager;
                AppealActionPage _appealAction;
                automatedBase.CurrentPage = _appealManager = automatedBase.QuickLaunch.NavigateToAppealManager();

                List<string> _cvtyPlanList = _appealManager.GetAssociatedPlansForClient(ClientEnum.CVTY.GetStringDisplayValue());
                List<string> _pehpPlanList = _appealManager.GetAssociatedPlansForClient(ClientEnum.PEHP.ToString());
                List<string> _smtstPlanList = _appealManager.GetAssociatedPlansForClient(ClientEnum.SMTST.ToString());
                List<string> _ttreePlanList = _appealManager.GetAssociatedPlansForClient(ClientEnum.TTREE.GetStringDisplayValue());
                List<string> _lineOfBusinessList = _appealManager.GetCommonSql.GetLineOfBusiness();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var expectedProductTypeList = automatedBase.DataHelper
                    .GetMappingData(FullyQualifiedClassName, "product_type").Values.ToList();
                var expectedAppealLevelList = automatedBase.DataHelper
                    .GetMappingData(FullyQualifiedClassName, "appeal_level").Values.ToList();
                var expectedAppealTypeList = automatedBase.DataHelper
                    .GetMappingData(FullyQualifiedClassName, "Appeal_type").Values.ToList();
                var expectedAppealPriorityList = automatedBase.DataHelper
                    .GetMappingData(FullyQualifiedClassName, "Appeal_priority").Values.ToList();
                var expectedStatusList = automatedBase.DataHelper
                    .GetMappingData(FullyQualifiedClassName, "appeal_status_list").Values.ToList();

                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                _appealManager.IsAdvancedSearchFilterIconDispalyed()
                    .ShouldBeTrue("Advanced search filter icon displayed should be true.");
                _appealManager.ClickOnAdvancedSearchFilterIcon(true);

                StringFormatter.PrintMessageTitle("Verification of Product Drop down field");
                ValidateSingleDropDownForDefaultValueAndExpectedList("Product", expectedProductTypeList, _appealManager);

                StringFormatter.PrintMessageTitle(
                    "Verification of Visibility of Input Fields when Client is not selected");
                _appealManager.GetInputValueByLabel("Client").ShouldBeEqual("All", "Client value defaults to All");
                _appealManager.IsSearchInputFieldDispalyedByLabel("Plan")
                    .ShouldBeFalse("On non Specific Client selection, Plan visibility should be false");
                _appealManager.IsSearchInputFieldDispalyedByLabel("Line Of Business")
                    .ShouldBeFalse("On Non Specific Client selection, LOB visibility should be false");
                _appealManager.IsInputFieldForRespectiveLabelDisabled("Provider Sequence")
                    .ShouldBeTrue("On Specific Client selection, Provider Sequence's disabled state should be true?");
                _appealManager.GetInputAttributeValueByLabel("Provider Sequence", "placeholder")
                    .ShouldBeEqual("Please Select Client", " Provider Sequence ask to select client");

                _appealManager.SelectSMTST();
                _appealManager.IsSearchInputFieldDispalyedByLabel("Plan")
                    .ShouldBeTrue("On Specific Client selection,Plan is visible");
                _appealManager.IsSearchInputFieldDispalyedByLabel("Line Of Business")
                    .ShouldBeTrue("On Specific Client selection, LOB visibility should be true");
                _appealManager.IsInputFieldForRespectiveLabelDisabled("Provider Sequence")
                    .ShouldBeFalse("On Specific Client selection, Provider Sequence's disabled state should be false?");
                _appealManager.GetInputValueByLabel("Provider Sequence")
                    .ShouldBeNullorEmpty("Provider Sequence value defaults to blank on client selection");
                _appealManager.SetInputFieldByInputLabel("Provider Sequence", paramLists["ProviderSeq"]);

                StringFormatter.PrintMessageTitle("Plan field Validation");
                ValidateMultipleDropDownForDefaultValueAndExpectedList("Plan", _smtstPlanList, _appealManager);
                _appealManager.SelectDropDownListbyInputLabel("Client", ClientEnum.PEHP.ToString());
                ValidateMultipleDropDownForDefaultValueAndExpectedList("Plan", _pehpPlanList, _appealManager);
                _appealManager.SelectDropDownListbyInputLabel("Client", ClientEnum.CVTY.GetStringDisplayValue());
                ValidateMultipleDropDownForDefaultValueAndExpectedList("Plan", _cvtyPlanList, _appealManager);
                _appealManager.SelectDropDownListbyInputLabel("Client", ClientEnum.TTREE.ToString());
                ValidateMultipleDropDownForDefaultValueAndExpectedList("Plan", _ttreePlanList, _appealManager);

                StringFormatter.PrintMessageTitle("LOB field Validation");
                ValidateFieldSupportingMultipleValues("Line Of Business", _lineOfBusinessList, _appealManager);

                StringFormatter.PrintMessageTitle("Primary Reviewer field Validation");
                ValidatePrimaryReviewerAndAssignedTo("Primary Reviewer", _appealManager, automatedBase);

                StringFormatter.PrintMessageTitle("Assigned To field Validation");
                ValidatePrimaryReviewerAndAssignedTo("Assigned To", _appealManager, automatedBase);

                StringFormatter.PrintMessageTitle("Category Field field Validation");
                ValidateCategoryField("Category", paramLists["Category"].Split(';'), _appealManager);

                ValidateSingleDropDownForDefaultValueAndExpectedList("Type", expectedAppealTypeList, _appealManager, false);
                ValidateSingleDropDownForDefaultValueAndExpectedList("Priority", expectedAppealPriorityList, _appealManager, false);

                _appealManager.GetAvailableDropDownList("Appeal Level")
                    .ShouldCollectionBeEqual(expectedAppealLevelList, "Appeal Level List is As Expected");
                _appealManager.GetInputValueByLabel("Appeal Level")
                    .ShouldBeEqual("All", "Appeal Level value defaults to All");

                _appealManager.SelectAllAppeals();

                StringFormatter.PrintMessageTitle("Validate Date Field");
                ValidateDateRangePickerBehaviour("Due Date", _appealManager);
                ValidateDateRangePickerBehaviour("Create Date", _appealManager);
                ValidateDateRangePickerBehaviour("Complete Date", _appealManager);

                _appealManager.GetInputValueByLabel("First Deny Code")
                    .ShouldBeNullorEmpty("First Deny Code value defaults to blank");
                _appealManager.GetInputValueByLabel("First Pay Code")
                    .ShouldBeNullorEmpty("First Pay Code value defaults to blank");

                _appealManager.SetInputFieldByInputLabel("First Deny Code", "0!0a");
                _appealManager.GetPageErrorMessage()
                    .ShouldBeEqual("Only alphanumerics allowed.",
                        "Popup Error message is shown when non alphanumeric character is passed to First Deny Code.");
                _appealManager.ClosePageError();

                ValidateAlphaNumeric("First Pay Code", "Only alphanumerics allowed.", "0!2a", _appealManager);
                ValidateAlphaNumeric("First Deny Code", "Only alphanumerics allowed.", "0!2a", _appealManager);


                _appealManager.ClearAll();
                _appealManager.SelectAllAppeals();
                _appealManager.SelectSMTST();

                StringFormatter.PrintMessageTitle("Status Field  Validation");
                ValidateSingleDropDownForDefaultValueAndExpectedList("Status", expectedStatusList, _appealManager, false);

                _appealManager.SetInputFieldByInputLabel("Claim Sequence", paramLists["ClaimSeq"]);

                var firstDeny = paramLists["FirstDeny"];
                var firstPay = paramLists["FirstPay"];
                _appealManager.SetInputFieldByInputLabel("First Deny Code", firstDeny);
                _appealManager.SetInputFieldByInputLabel("First Pay Code", firstPay);
                _appealManager.ClickOnFindButton();
                _appealManager.WaitForWorkingAjaxMessage();
                _appealManager.GetSearchResultByColRow(9).ShouldBeEqual(firstDeny);
                _appealManager.GetSearchResultByColRow(10).ShouldBeEqual(firstPay);


                Console.WriteLine(
                    "Navigate to appeal action to confirm first deny and first pay vode value is the value of deny proc code and trigger code for the first appealed line.");
                automatedBase.CurrentPage =
                    _appealAction =
                        _appealManager.ClickOnAppealSequenceSwitchToAppealAction(
                            _appealManager.GetSearchResultByColRow(4));
                _appealAction.GetProcCode()
                    .ShouldBeEqual(firstDeny,
                        "Appeal manager search result yields appeals that have first appealed line with first deny value: " +
                        firstDeny + " as the deny proc code.");
                _appealAction.GetTrigCode()
                    .ShouldBeEqual(firstPay,
                        "Appeal manager search result yields appeals that have first appealed line with first pay value: " +
                        firstPay + "  as the deny trigger code.");
                _appealManager.CloseAnyPopupIfExist();
            }
        }

        [Test] //US61556
        public void Validation_of_basic_search_filters_on_appeal_search_panel()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealManagerPage _appealManager;
                automatedBase.CurrentPage = _appealManager = automatedBase.QuickLaunch.NavigateToAppealManager();

                List<string> _assignedClientList = _appealManager.GetAssignedClientList(automatedBase.EnvironmentManager.HciAdminUsername);
                _assignedClientList.Insert(0, "All");

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestExtensions.TestName);

                var filtersList = paramLists["FiltersList"].Split(';');
                var defaultFilterValue = paramLists["DefaultValue"].Split(';');
                var expectedClientSearchList = _assignedClientList;
                var client = ClientEnum.SMTST.ToString();
                var claseq = paramLists["ClaimSeq"];

                _appealManager.RefreshPage();
                _appealManager.GetPageHeader()
                    .ShouldBeEqual(PageHeaderEnum.AppealManager.GetStringValue(),
                        "Successfully navigated to Appeal Manager");
                _appealManager.IsNoDataMessagePresentInLeftSection()
                    .ShouldBeTrue("A blank page is displayed in the Appeal Manager page");

                _appealManager.SelectDropDownListbyInputLabel("Quick Search",
                    AppealQuickSearchTypeEnum.AllAppeals.GetStringValue());
                _appealManager.ClickOnAdvancedSearchFilterIcon(true);
                _appealManager.FilterCountByLabel().ShouldBeEqual(filtersList.Length, "No filters are hidden");
                ValidateNoFiltersArePreApplied(filtersList, defaultFilterValue, _appealManager);
                _appealManager.ClickOnAdvancedSearchFilterIcon(false);
                _appealManager.FilterCountByLabel().ShouldBeEqual(5, "Basic filters for All Appeals are shown");
                _appealManager.SelectDropDownListbyInputLabel("Quick Search",
                    AppealQuickSearchTypeEnum.AppealsDueToday.GetStringValue());
                _appealManager.FilterCountByLabel().ShouldBeEqual(5, "Basic filters for Appeals Due Today are shown");
                _appealManager.SelectDropDownListbyInputLabel("Quick Search",
                    AppealQuickSearchTypeEnum.AllUrgentAppeals.GetStringValue());
                _appealManager.FilterCountByLabel().ShouldBeEqual(5, "Basic filters for All Urgent Appeals are shown");
                _appealManager.SelectDropDownListbyInputLabel("Quick Search",
                    AppealQuickSearchTypeEnum.AllRecordReviews.GetStringValue());
                _appealManager.FilterCountByLabel().ShouldBeEqual(5, "Basic filters for All Record Reviews are shown");
                _appealManager.SelectDropDownListbyInputLabel("Quick Search",
                    AppealQuickSearchTypeEnum.OutstandingAppeals.GetStringValue());
                _appealManager.FilterCountByLabel().ShouldBeEqual(5, "Basic filters for Outstanding Appeals are shown");
                ValdiateForDefaultValueOfBasicFilter(_appealManager);

                _appealManager.GetAvailableDropDownList("Client")
                    .ShouldCollectionBeEqual(expectedClientSearchList, "Client Search List As Expected");
                _appealManager.SelectDropDownListbyInputLabel("Client", client);
                _appealManager.GetInputValueByLabel("Client")
                    .ShouldBeEqual(client, "Client Search bears type-ahead functionality");
                _appealManager.IsInputFieldForRespectiveLabelDisabled("Claim Sequence")
                    .ShouldBeFalse("On No Specific Client selection, Claim Sequence should not be disabled. Is False?");
                _appealManager.IsInputFieldForRespectiveLabelDisabled("Claim No")
                    .ShouldBeFalse("On No Specific Client selection, Claim No should not be disabled. Is False?");
                ValidateAlphaNumeric("Claim No", "Only alphanumerics allowed.", "0!2a", _appealManager);
                _appealManager.SetInputFieldByInputLabel("Claim No", paramLists["50CharacterClaimNo"]);
                _appealManager.GetInputValueByLabel("Claim No").Count()
                    .ShouldBeEqual(50, "Claim No should not be greater than 50 character");
                _appealManager.ClickOnClearLink();
                _appealManager.WaitForStaticTime(1000);
                ValdiateForDefaultValueOfBasicFilter(_appealManager);
                _appealManager.GetInputValueByLabel("Quick Search")
                    .ShouldBeEqual("Outstanding Appeals", "Quick search option has not been cleared");
                StringFormatter.PrintMessageTitle(
                    "Validate For claim sequence, search can be initiated without clasub");
                _appealManager.SelectDropDownListbyInputLabel("Quick Search",
                    AppealQuickSearchTypeEnum.AllAppeals.GetStringValue());
                _appealManager.SelectDropDownListbyInputLabel("Client", client);
                _appealManager.SetInputFieldByInputLabel("Claim Sequence", claseq);
                _appealManager.ClickOnFindButton();
                _appealManager.WaitForWorkingAjaxMessage();
                _appealManager.GetAppealSearchResultRowCount().ShouldBeGreater(0,
                    "Claim sequence does not require exact value, claimseq without clasub can be used.");
                var appealSeq = _appealManager.GetSearchResultByColRow(4);
                _appealManager.ClickOnClearLink();
                _appealManager.GetInputValueByLabel("Quick Search")
                    .ShouldBeEqual(AppealQuickSearchTypeEnum.AllAppeals.GetStringValue(),
                        "Quick search option has not been cleared");
                _appealManager.SelectDropDownListbyInputLabel("Quick Search", "All Appeals");
                _appealManager.SetInputFieldByInputLabel("Appeal Sequence", appealSeq + "123");
                _appealManager.ClickOnFindButton();
                _appealManager.WaitForWorkingAjaxMessage();
                _appealManager.GetNoMatchingRecordFoundMessage()
                    .ShouldBeEqual("No matching records were found.", "No matching record found Message");
                _appealManager.SetInputFieldByInputLabel("Appeal Sequence", appealSeq);
                _appealManager.ClickOnFindButton();
                _appealManager.WaitForWorkingAjaxMessage();
                _appealManager.GetAppealSearchResultRowCount()
                    .ShouldBeGreater(0, "Appeal sequence requires exact value");
                _appealManager.GetSearchResultByColRow(4).ShouldBeEqual(appealSeq,
                    "Only exact appeal sequence value retrieves correct result");
                _appealManager.ClickOnClearLink();

            }
        }

        [NonParallelizable]
        [Test, Category("OnDemand")] //US68890
        public void Verify_completed_appeal_is_set_to_Closed_status_automatically_when_Close_Client_Appeals_client_setting_is_set_true()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealManagerPage _appealManager;
                ClientSearchPage _clientSearch;
                automatedBase.CurrentPage = _appealManager = automatedBase.QuickLaunch.NavigateToAppealManager();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var claimSequence = automatedBase.DataHelper
                    .GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName, "ClaimSequence", "Value");

                try
                {
                    automatedBase.CurrentPage = _clientSearch = automatedBase.QuickLaunch.NavigateToClientSearch();
                    _clientSearch.SearchByClientCodeToNavigateToClientProfileViewPage(ClientEnum.SMTST.ToString(),
                        ClientSettingsTabEnum.Product.GetStringValue());
                    _clientSearch.ClickAutoCloseAppealRadioButton(true);
                    _clientSearch.IsRadioButtonOnOffByLabel(ProductAppealsEnum.AutoCloseAppeals.GetStringValue())
                        .ShouldBeTrue(
                            $"Is {ProductAppealsEnum.AutoCloseAppeals.GetStringValue()} radio button selected?");

                    automatedBase.CurrentPage = _appealManager = automatedBase.CurrentPage.NavigateToAppealManager();
                    _appealManager.SearchByClaimSequence(claimSequence, ClientEnum.SMTST.ToString());
                    _appealManager.ChangeStatusOfAppealByRowSelection(); //changed status to new
                    _appealManager.ChangeStatusOfAppealByRowSelection(1,
                        AppealStatusEnum.Complete.GetStringDisplayValue());
                    _appealManager.GetSearchResultByColRow(11).ShouldBeEqual(
                        AppealStatusEnum.Closed.GetStringDisplayValue(),
                        "Completed appeal should Closed automatically");
                }
                finally
                {
                    _clientSearch = automatedBase.CurrentPage.NavigateToClientSearch();
                    _clientSearch.SearchByClientCodeToNavigateToClientProfileViewPage(ClientEnum.SMTST.ToString(),
                        ClientSettingsTabEnum.Product.GetStringValue());
                    _clientSearch.ClickAutoCloseAppealRadioButton(false);
                }
            }
        }

        [Test, Category("Upload_Document")] //US69597
        public void Verify_the_refresh_of_an_appeal_record_of_status_Open_when_a_document_is_uploaded()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealManagerPage _appealManager;
                AppealCreatorPage _appealCreator;
                
                automatedBase.CurrentPage = _appealManager = automatedBase.QuickLaunch.NavigateToAppealManager();
                FileUploadPage FileUploadPage = _appealManager.GetFileUploadPage;

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestExtensions.TestName);
                var claimSeq = paramLists["ClaimSequence"];
                var fileToUpload = paramLists["FileToUpload"].Split(';');
                var fileType = paramLists["SelectedFileList"];

                _appealManager.DeleteAppealsAssociatedWithClaim(claimSeq.Split('-')[0]);
                //_appealManager.DeleteAppealsOnClaim(claimSeq.Replace("-", ""));
                _appealCreator = _appealManager.NavigateToAppealCreator();
                _appealCreator.SearchByClaimSequence(claimSeq);
                CreateAppeal(_appealCreator);
                automatedBase.CurrentPage = _appealManager = automatedBase.CurrentPage.NavigateToAppealManager();

                _appealManager.SearchByClaimSequence(claimSeq);
                _appealManager.GetSearchResultByColRow(11)
                    .ShouldBeEqual(AppealStatusEnum.Open.GetStringDisplayValue(),
                        "Status of the appeal should be Open");
                _appealManager.ClickOnSearchListRow(1);

                try
                {
                    FileUploadPage.ClickOnAppealDocumentUploadIcon();
                    FileUploadPage.IsDocumentUploadSectionPresent()
                        .ShouldBeTrue("Appeal Document Uploader Section is displayed");
                    FileUploadPage.SetFileTypeListVlaue(fileType);
                    FileUploadPage.AddFileForUpload(fileToUpload[0]);
                    FileUploadPage.ClickOnAddFileBtn();
                    FileUploadPage.ClickOnSaveUploadBtn();
                    FileUploadPage.IsFollowingAppealDocumentPresent(fileToUpload[0])
                        .ShouldBeTrue("Uploaded Document is listed");

                    _appealManager.GetSearchResultByColRow(3).ShouldNotBeEmpty("Due date field should not be empty");
                    _appealManager.GetSearchResultByColRow(8)
                        .ShouldNotBeEmpty("Primary Reviewer field should not be empty");
                    _appealManager.GetSearchResultByColRow(9)
                        .ShouldNotBeEmpty("First Deny Code field should not be empty");
                    _appealManager.GetSearchResultByColRow(10)
                        .ShouldNotBeEmpty("First Pay  Code field should not be empty");
                    _appealManager.GetSearchResultByColRow(11)
                        .ShouldBeEqual(AppealStatusEnum.New.GetStringDisplayValue(),
                            "Status of the appeal should change to New");
                }
                finally
                {
                    if (FileUploadPage.IsFollowingAppealDocumentPresent(fileToUpload[0]))
                    {
                        Console.WriteLine("delete document if previously run test failed to delete uploaded document");
                        FileUploadPage.ClickOnDeleteFileBtn(fileToUpload[0]);
                        _appealManager.ClickOkCancelOnConfirmationModal(true);
                    }
                }
            }
        }

        [Test, Category("AppealManager1")] //US69632
        public void Search_for_claim_by_alt_claim_no()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealManagerPage _appealManager;
                
                automatedBase.CurrentPage = _appealManager = automatedBase.QuickLaunch.NavigateToAppealManager();
                
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var claimNo = automatedBase.DataHelper
                    .GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName, "AltClaimNo", "Value");
                _appealManager.GetSideBarPanelSearch.GetInputValueByLabel("Client")
                    .ShouldBeEqual("All", "All client is selected");
                _appealManager.GetSideBarPanelSearch.IsInputFieldForRespectiveLabelDisabled("Claim No")
                    .ShouldBeTrue("Claim no should be disabled until specific client is selected");
                _appealManager.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search", "All Appeals");
                _appealManager.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Client",
                    ClientEnum.SMTST.ToString());
                _appealManager.GetSideBarPanelSearch.IsInputFieldForRespectiveLabelDisabled("Claim No")
                    .ShouldBeFalse("Claim no should be enabled after specific client is selected");
                _appealManager.GetSideBarPanelSearch.SetInputFieldByLabel("Claim No", "!Asd12");
                _appealManager.IsPageErrorPopupModalPresent()
                    .ShouldBeTrue("Claim no should only accept alphanumeric character as input");
                _appealManager.ClosePageError();
                _appealManager.GetSideBarPanelSearch.SetInputFieldByLabel("Claim No", "Asd12");
                _appealManager.ClickOnFindButton();
                _appealManager.WaitForWorkingAjaxMessage();
                _appealManager.IsPageErrorPopupModalPresent()
                    .ShouldBeFalse("Claim no should accept alphanumeric character as input");
                _appealManager.GetNoMatchingRecordFoundMessage()
                    .ShouldBeEqual("No matching records were found.", "No matching record found Message");
                _appealManager.GetSideBarPanelSearch.SetInputFieldByLabel("Claim No", claimNo);
                _appealManager.ClickOnFindButton();
                _appealManager.WaitForWorkingAjaxMessage();
                _appealManager.GetSearchResultCount().ShouldBeGreater(1, "Searching by claimno should execute result");
            }
        }

        [Test, Category("AppealManager1")] //CAR-354
        public void Validate_appeal_status_updates_when_docs_uploaded_to_docs_waiting_appeal_status()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealManagerPage _appealManager;
                AppealActionPage _appealAction;
                
                automatedBase.CurrentPage = _appealManager = automatedBase.QuickLaunch.NavigateToAppealManager();
                FileUploadPage FileUploadPage = _appealManager.GetFileUploadPage;

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> testData = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestExtensions.TestName);
                var claimSeq = testData["ClaimSequence"];
                var fileType = testData["FileType"];
                var fileName = testData["FileName"];
                try
                {
                    StringFormatter.PrintMessage(
                        "Reset appeal to default i.e default status is Docs awaiting with no documents");
                    _appealManager.ResetAppealToDocumentWaiting(claimSeq);
                    _appealManager.SearchByClaimSequence(claimSeq, ClientEnum.SMTST.ToString());
                    _appealManager.ClickOnSearchListRow(1);
                    _appealManager.GetSearchResultByColRow(11).ShouldBeEqual(
                        AppealStatusEnum.DocumentsWaiting.GetStringDisplayValue(),
                        "Current status of appeal should be document awaiting");
                    var dueDate = _appealManager.GetSearchResultByColRow(3);
                    _appealManager.GetSearchResultByColRow(7).ShouldBeNullorEmpty("Category Should Be Null");
                    _appealManager.GetSearchResultByColRow(8).ShouldBeNullorEmpty("Primary Reviewer should Be Null");

                    _appealManager.GetAppealDetailsContentValue(4, 2).ShouldBeNullorEmpty("Assigned To Should be Null");
                    StringFormatter.PrintMessage("Upload document to appeal");
                    FileUploadPage.UploadDocument(fileType, fileName,
                        "new file test doc", 1);
                    FileUploadPage.ClickOnSaveUploadBtn();
                    _appealManager.WaitForWorkingAjaxMessage();
                    StringFormatter.PrintMessage("Validate appeal status is updated to New");
                    _appealManager.GetSearchResultByColRow(11)
                        .ShouldBeEqual(AppealStatusEnum.New.GetStringDisplayValue(),
                            "Current status of appeal should be updated to new after document is uploaded.");


                    _appealManager.GetSearchResultByColRow(3).ShouldNotBeEqual(dueDate, "Due Date should be updated");
                    _appealManager.GetSearchResultByColRow(7).ShouldNotBeEmpty("Category Should be assigned");
                    _appealManager.GetSearchResultByColRow(8).ShouldNotBeEmpty("Primary Reviewer should be assigned");
                    _appealManager.ClickOnSearchListRow(1);
                    _appealManager.GetAppealDetailsContentValue(4, 2)
                        .ShouldNotBeEmpty("Assigned To Should be assigned");

                    _appealAction =
                        _appealManager.ClickOnAppealSequenceSwitchToAppealAction(
                            _appealManager.GetSearchResultByColRow(4));

                    StringFormatter.PrintMessage("Validate Appeal Audit record");
                    _appealAction.ClickMoreOption();
                    var appealProcessingHistory = _appealAction.ClickAppealProcessingHx();

                    appealProcessingHistory.GetAppealAuditGridTableDataValue(1, 6)
                        .ShouldBeEqual(AppealStatusEnum.New.GetStringDisplayValue(), "Status audit");
                    appealProcessingHistory.GetAppealAuditGridTableDataValue(1, 9)
                        .ShouldNotBeEmpty("Assigned To audit");
                    appealProcessingHistory.GetAppealAuditGridTableDataValue(1, 10)
                        .ShouldNotBeEmpty("Primary Reviewer audit");
                    appealProcessingHistory.GetAppealAuditGridTableDataValue(1, 12)
                        .ShouldNotBeEmpty("Due date must be assigned");

                }
                finally
                {
                    _appealManager.CloseAnyPopupIfExist();
                }
            }
        }

        [Test] //TE-925 
        public void verify_Appeals_With_Mentor_Review_Status_Are_Getting_Displayed_In_Search_Result()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealManagerPage _appealManager;

                automatedBase.CurrentPage = _appealManager = automatedBase.QuickLaunch.NavigateToAppealManager();

                StringFormatter.PrintMessage("Verify Mentor Review status is displayed for Urgent Appeals");
                _appealManager.SelectDropDownListbyInputLabel("Quick Search",
                    AppealQuickSearchTypeEnum.AllUrgentAppeals.GetStringValue());
                _appealManager.GetSideBarPanelSearch.ClickOnFindButton();
                _appealManager.WaitForWorking();
                while (_appealManager.GetGridViewSection.IsLoadMorePresent())
                {
                    _appealManager.GetGridViewSection.ClickLoadMore();
                }


                _appealManager.GetGridViewSection.GetGridListValueByCol(4)
                    .ShouldCollectionBeEquivalent(
                        _appealManager.GetUrgentAppealsList(automatedBase.EnvironmentManager.Username),
                        "Appeals equal?");

                StringFormatter.PrintMessage("Verify Mentor Review status is displayed for All Record Reviews");
                _appealManager.SelectDropDownListbyInputLabel("Quick Search",
                    AppealQuickSearchTypeEnum.AllRecordReviews.GetStringValue());
                _appealManager.GetSideBarPanelSearch.ClickOnFindButton();
                _appealManager.WaitForWorking();
                while (_appealManager.GetGridViewSection.IsLoadMorePresent())
                {
                    _appealManager.GetGridViewSection.ClickLoadMore();
                }

                _appealManager.GetGridViewSection.GetGridListValueByCol(4)
                    .ShouldCollectionBeEquivalent(
                        _appealManager.GetRecordReviewsList(automatedBase.EnvironmentManager.Username),
                        "Appeals equal?");


                StringFormatter.PrintMessage("Verify Mentor Review status is displayed for Appeals Due Today");
                _appealManager.SelectDropDownListbyInputLabel("Quick Search",
                    AppealQuickSearchTypeEnum.AppealsDueToday.GetStringValue());
                _appealManager.SelectDropDownListbyInputLabel("Client", ClientEnum.SMTST.ToString());
                _appealManager.GetSideBarPanelSearch.ClickOnAdvancedSearchFilterIcon(true);
                _appealManager.SelectDropDownListbyInputLabel("Type", "Record Review");
                _appealManager.GetSideBarPanelSearch.ClickOnFindButton();
                _appealManager.WaitForWorking();

                while (_appealManager.GetGridViewSection.IsLoadMorePresent())
                {
                    _appealManager.GetGridViewSection.ClickLoadMore();
                }

                _appealManager.GetGridViewSection.GetGridListValueByCol(4)
                    .ShouldCollectionBeEquivalent(
                        _appealManager.GetAppealsDueTodayList(automatedBase.EnvironmentManager.Username),
                        "Appeals equal?");

            }
        }

        [Test] //CAR-728
        public void Verify_find_button_is_disabled_when_search_is_active()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealManagerPage _appealManager;

                automatedBase.CurrentPage = _appealManager = automatedBase.QuickLaunch.NavigateToAppealManager();

                _appealManager.SelectDropDownListbyInputLabel("Quick Search", "Outstanding Appeals");
                _appealManager.ClickFindAndCheckIfFindButtonIsDisabled()
                    .ShouldBeTrue("Find Button Should be disabled while the search is active.");
                _appealManager.WaitForWorkingAjaxMessage();
                _appealManager.GetGridViewSection.GetGridRowCount()
                    .ShouldBeGreater(0, "Search results should be displayed");
                _appealManager.CheckIfFindButtonIsEnabled()
                    .ShouldBeTrue("Find Button Should be enabled once the search is complete.");
            }
        }

        [Test] //TE-749
        [Order(1)]
        public void Verify_Outstanding_DCI_Appeals_Quick_Search_Option_In_Appeal_Manager()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealManagerPage _appealManager;

                automatedBase.CurrentPage = _appealManager = automatedBase.QuickLaunch.NavigateToAppealManager();

                List<string> _assignedClientList = _appealManager.GetAssignedClientList(automatedBase.EnvironmentManager.HciAdminUsername);
                _assignedClientList.Insert(0, "All");

                List<string> _smtstPlanList = _appealManager.GetAssociatedPlansForClient(ClientEnum.SMTST.ToString());

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestExtensions.TestName);

                var basicSearchFilterList = automatedBase.DataHelper
                    .GetMappingData(FullyQualifiedClassName, "basic_filters_outstanding_dci_appeals").Values.ToList();
                var expectedAppealLevelList = automatedBase.DataHelper
                    .GetMappingData(FullyQualifiedClassName, "appeal_level").Values.ToList();
                var expectedAppealTypeList = automatedBase.DataHelper
                    .GetMappingData(FullyQualifiedClassName, "Appeal_type").Values.ToList();
                var expectedAppealPriorityList = automatedBase.DataHelper
                    .GetMappingData(FullyQualifiedClassName, "Appeal_priority").Values.ToList();
                var expectedClientSearchList = _assignedClientList;
                var advancedFiltersList = paramLists["FiltersList"].Split(';');
                var defaultFilterValue = paramLists["DefaultValue"].Split(';');
                var client = ClientEnum.SMTST.ToString();
                var outstandingDCIAppealsFromDb =
                    _appealManager.GetOutstandingDCIAppealsFromDb(automatedBase.EnvironmentManager.HciAdminUsername);
                var claseq = paramLists["ClaimSeq"];
                var lineOfBusinessList = _appealManager.GetLineOfBusinessFromDb();
                
                StringFormatter.PrintMessage("Verify Basic Search Filter For Outstanding DCA Appeals");
                _appealManager.SelectDropDownListbyInputLabel("Quick Search",
                    AppealQuickSearchTypeEnum.OutstandingDCIAppeals.GetStringValue());
                _appealManager.FilterCountByLabel()
                    .ShouldBeEqual(5, "Basic filters for Outstanding DCA Appeals are shown");
                _appealManager.GetSideBarPanelSearch.GetSearchFiltersList()
                    .ShouldCollectionBeEqual(basicSearchFilterList, "Search Filters", true);
                ValdiateForDefaultValueOfBasicFilter(_appealManager);
                _appealManager.GetAvailableDropDownList("Client")
                    .ShouldCollectionBeEqual(expectedClientSearchList, "Client Search List As Expected");
                _appealManager.SelectDropDownListbyInputLabel("Client", client);
                _appealManager.GetInputValueByLabel("Client")
                    .ShouldBeEqual(client, "Client Search bears type-ahead functionality");
                _appealManager.IsInputFieldForRespectiveLabelDisabled("Claim Sequence")
                    .ShouldBeFalse("On No Specific Client selection, Claim Sequence should not be disabled. Is False?");
                _appealManager.IsInputFieldForRespectiveLabelDisabled("Claim No")
                    .ShouldBeFalse("On No Specific Client selection, Claim No should not be disabled. Is False?");
                ValidateAlphaNumeric("Claim No", "Only alphanumerics allowed.", "0!2a", _appealManager);
                _appealManager.SetInputFieldByInputLabel("Claim No", paramLists["50CharacterClaimNo"]);
                _appealManager.GetInputValueByLabel("Claim No").Count()
                    .ShouldBeEqual(50, "Claim No should not be greater than 50 character");
                _appealManager.ClickOnClearLink();
                _appealManager.WaitForStaticTime(200);
                ValdiateForDefaultValueOfBasicFilter(_appealManager);
                _appealManager.GetInputValueByLabel("Quick Search")
                    .ShouldBeEqual(AppealQuickSearchTypeEnum.OutstandingDCIAppeals.GetStringValue(),
                        "Quick search option has not been cleared");
                StringFormatter.PrintMessageTitle(
                    "Validate For claim sequence, search can be initiated without clasub");
                _appealManager.ClickOnClearLink();
                _appealManager.SelectDropDownListbyInputLabel("Client", client);
                _appealManager.SetInputFieldByInputLabel("Claim Sequence", claseq);
                _appealManager.ClickOnFindButton();
                _appealManager.WaitForWorkingAjaxMessage();
                _appealManager.GetAppealSearchResultRowCount().ShouldBeGreater(0,
                    "Claim sequence does not require exact value, claimseq without clasub can be used.");

                StringFormatter.PrintMessage("Verify Advanced Search Filter For Outstanding DCA Appeals");
                _appealManager.ClickOnClearLink();
                _appealManager.SelectDropDownListbyInputLabel("Quick Search",
                    AppealQuickSearchTypeEnum.OutstandingDCIAppeals.GetStringValue());
                _appealManager.ClickOnAdvancedSearchFilterIcon(true);
                _appealManager.FilterCountByLabel().ShouldBeEqual(advancedFiltersList.Length, "No filters are hidden");
                _appealManager.GetSideBarPanelSearch.GetSearchFiltersList()
                    .ShouldCollectionBeEqual(advancedFiltersList, "Search Filters", true);
                ValidateNoFiltersArePreApplied(advancedFiltersList, defaultFilterValue, _appealManager);

                StringFormatter.PrintMessageTitle(
                    "Verification of Visibility of Input Fields when Client is not selected");
                _appealManager.GetInputValueByLabel("Client").ShouldBeEqual("All", "Client value defaults to All");
                _appealManager.IsSearchInputFieldDispalyedByLabel("Plan")
                    .ShouldBeFalse("On non Specific Client selection, Plan visibility should be false");
                _appealManager.IsSearchInputFieldDispalyedByLabel("Line Of Business")
                    .ShouldBeFalse("On Non Specific Client selection, LOB visibility should be false");
                _appealManager.IsInputFieldForRespectiveLabelDisabled("Provider Sequence")
                    .ShouldBeTrue("On Specific Client selection, Provider Sequence's disabled state should be true?");
                _appealManager.GetInputAttributeValueByLabel("Provider Sequence", "placeholder")
                    .ShouldBeEqual("Please Select Client", " Provider Sequence ask to select client");

                _appealManager.SelectSMTST();
                _appealManager.IsSearchInputFieldDispalyedByLabel("Plan")
                    .ShouldBeTrue("On Specific Client selection,Plan is visible");
                _appealManager.IsSearchInputFieldDispalyedByLabel("Line Of Business")
                    .ShouldBeTrue("On Specific Client selection, LOB visibility should be true");
                _appealManager.IsInputFieldForRespectiveLabelDisabled("Provider Sequence")
                    .ShouldBeFalse("On Specific Client selection, Provider Sequence's disabled state should be false?");
                _appealManager.GetInputValueByLabel("Provider Sequence")
                    .ShouldBeNullorEmpty("Provider Sequence value defaults to blank on client selection");

                StringFormatter.PrintMessageTitle("Plan field Validation");
                ValidateMultipleDropDownForDefaultValueAndExpectedList("Plan", _smtstPlanList, _appealManager);

                StringFormatter.PrintMessageTitle("LOB field Validation");
                ValidateFieldSupportingMultipleValues("Line Of Business", lineOfBusinessList, _appealManager);

                StringFormatter.PrintMessageTitle("Primary Reviewer field Validation");
                ValidatePrimaryReviewerAndAssignedTo("Primary Reviewer", _appealManager, automatedBase);

                StringFormatter.PrintMessageTitle("Assigned To field Validation");
                ValidatePrimaryReviewerAndAssignedTo("Assigned To", _appealManager, automatedBase);

                StringFormatter.PrintMessageTitle("Category Field Validation");
                ValidateCategoryField("Category", paramLists["Category"].Split(';'), _appealManager);

                ValidateSingleDropDownForDefaultValueAndExpectedList("Type", expectedAppealTypeList, _appealManager, false);
                ValidateSingleDropDownForDefaultValueAndExpectedList("Priority", expectedAppealPriorityList, _appealManager, false);

                _appealManager.GetAvailableDropDownList("Appeal Level")
                    .ShouldCollectionBeEqual(expectedAppealLevelList, "Appeal Level List is As Expected");
                _appealManager.GetInputValueByLabel("Appeal Level")
                    .ShouldBeEqual("All", "Appeal Level value defaults to All");

                StringFormatter.PrintMessageTitle("Validate Date Field");
                ValidateDateRangePickerBehaviour("Due Date", _appealManager);
                ValidateDateRangePickerBehaviour("Create Date", _appealManager);
                ValidateDateRangePickerBehaviour("Complete Date", _appealManager);

                _appealManager.GetInputValueByLabel("First Deny Code")
                    .ShouldBeNullorEmpty("First Deny Code value defaults to blank");
                _appealManager.GetInputValueByLabel("First Pay Code")
                    .ShouldBeNullorEmpty("First Pay Code value defaults to blank");

                ValidateAlphaNumeric("First Pay Code", "Only alphanumerics allowed.", "0!2a", _appealManager);
                ValidateAlphaNumeric("First Deny Code", "Only alphanumerics allowed.", "0!2a", _appealManager);

                StringFormatter.PrintMessage("Verify Results For Outstanding Appeals");
                _appealManager.ClickOnClearLink();
                _appealManager.ClickOnFindButton();
                _appealManager.WaitForWorkingAjaxMessage();
                var nonOutstandingStatus = new List<String>
                {
                    AppealStatusEnum.Closed.GetStringValue(),
                    AppealStatusEnum.Complete.GetStringValue(),
                    AppealStatusEnum.DocumentsWaiting.GetStringValue(),
                    AppealStatusEnum.Open.GetStringValue(),
                    AppealStatusEnum.None.GetStringValue()
                };
                _appealManager.GetGridViewSection.GetGridRowCount()
                    .ShouldBeGreater(0, "Search Result should be returned.");
                _appealManager.GetSearchResultListByCol(11)
                    .Distinct().ToList().ShouldCollectionBeNotEqual(nonOutstandingStatus,
                        "Not OutStanding Status should not display");
                _appealManager.IsSearchInputFieldDispalyedByLabel("Status")
                    .ShouldBeFalse("Status display for Outstanding DCA Appeals");
                _appealManager.GetSearchResultListByCol(4)
                    .ShouldCollectionBeEquivalent(outstandingDCIAppealsFromDb, "Data Should Match");

            }
        }

        [Test, Category("AppealDependent")] //CAR-2692:CAR-2409
        public void Verify_Users_To_Select_Appeals_To_Be_Added_To_QA_Appeal_Search()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealManagerPage _appealManager;
                QaAppealSearchPage _qaAppealSearch;
                automatedBase.CurrentPage = _appealManager = automatedBase.QuickLaunch.NavigateToAppealManager();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestExtensions.TestName);
                var appealsNotEligibleForQA = paramLists["AppealNotEligibleForQA"].Split(';');
                var appealEligibleForQA = paramLists["AppealEligibleForQA"];
                var claSeq = paramLists["ClaSeq"];
                _appealManager.DeleteAppealForQAReviewByAppealSeqFromDb(appealEligibleForQA.ToString());
                try
                {
                    _appealManager.RefreshPage();
                    _appealManager.IsMoveAppealsToQaDisabled()
                        .ShouldBeTrue(
                            "Move Appeals to QA icon should be disabled while there are no appeals to move to qa.");
                    _appealManager.GetTooltipMoveAppealsToQA().ShouldBeEqual("Move appeals to QA",
                        "The icon should have tooltip value Move appeals to QA.");
                    SearchUsingAppealSeq(appealsNotEligibleForQA[0], _appealManager);
                    _appealManager.MoveAppealsToQa();
                    _appealManager.IsPageErrorPopupModalPresent()
                        .ShouldBeTrue("A pop up message should be shown to the user.");
                    _appealManager.GetPageErrorMessage()
                        .ShouldBeEqual("None of the appeals selected are eligible for QA.");
                    _appealManager.ClosePageError();
                    _appealManager.ClickOnClearLink();
                    _appealManager.SelectDropDownListbyInputLabel("Quick Search",
                        AppealQuickSearchTypeEnum.AllAppeals.GetStringValue());
                    _appealManager.SelectSMTST();
                    _appealManager.SetClaimSequence(claSeq);
                    _appealManager.ClickOnFindButton();
                    _appealManager.WaitForWorkingAjaxMessage();
                    _appealManager.IsMoveAppealsToQaDisabled()
                        .ShouldBeFalse(
                            "Move Appeals to QA icon should be enabled for users having QA Manager Authority.");
                    _appealManager.MoveAppealsToQa();
                    _appealManager.IsPageErrorPopupModalPresent()
                        .ShouldBeTrue("A pop up message should be shown to the user.");
                    _appealManager.GetPageErrorMessage()
                        .Contains("appeal(s) will be moved to QA Appeal Search. Do you wish to continue?")
                        .ShouldBeTrue(
                            "The message should be shown for confirmation of moving the appleals to the QA Review.");
                    _appealManager.ClickOnCancelLink();
                    _appealManager.IsPageErrorPopupModalPresent()
                        .ShouldBeFalse("The message should be closed and no changes should be made.");
                    _appealManager.ClickOnClearLink();
                    SearchUsingAppealSeq(appealEligibleForQA, _appealManager);
                    _appealManager.MoveAppealsToQa();
                    _appealManager.IsPageErrorPopupModalPresent()
                        .ShouldBeTrue("A pop up message should be shown to the user.");
                    _appealManager.ClickOkCancelOnConfirmationModal(true);
                    _appealManager.MoveAppealsToQa();
                    _appealManager.GetPageErrorMessage()
                        .ShouldBeEqual("None of the appeals selected are eligible for QA.");
                    _appealManager.ClosePageError();
                    _qaAppealSearch = _appealManager.NavigateToQaAppealSearch();
                    _qaAppealSearch.SelectOutstandingQaAppeals();
                    _qaAppealSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Analyst", "Test Automation");
                    _qaAppealSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Client",
                        ClientEnum.SMTST.ToString());
                    _qaAppealSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Restrictions",
                        "No Restriction");
                    _qaAppealSearch.ClickOnFindButton();
                    _qaAppealSearch.ClickOnLoadMore();
                    _qaAppealSearch.GetGridViewSection.GetGridListValueByCol(3).ShouldContain(appealEligibleForQA,
                        "Appeals moved to QA from Appeal Manager should be listed in QA Appeal Search.");
                    _qaAppealSearch.NavigateToAppealManager();
                }
                finally
                {
                    _appealManager.CloseAnyPopupIfExist();
                }
            }
        }

        [Test] //TE-926
        public void Verify_Results_For_Advanced_Filters()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealManagerPage _appealManager;

                automatedBase.CurrentPage = _appealManager = automatedBase.QuickLaunch.NavigateToAppealManager();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestExtensions.TestName);
                _appealManager.GetSideBarPanelSearch.ClickOnAdvancedSearchFilterIcon(true);
                _appealManager.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                    AppealQuickSearchTypeEnum.AllAppeals.GetStringValue());
                _appealManager.GetSideBarPanelSearch.SelectMultipleValuesInMultiSelectDropdownList("Primary Reviewer",
                    paramLists["Analysts"].Split(',').ToList());
                _appealManager.GetSideBarPanelSearch.SelectMultipleValuesInMultiSelectDropdownList("Assigned To",
                    paramLists["Analysts"].Split(',').ToList());
                _appealManager.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Status",
                    AppealStatusEnum.New.ToString());
                _appealManager.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Type", paramLists["Appealtype"]);
                _appealManager.GetSideBarPanelSearch.ClickOnFindButton();
                _appealManager.WaitForWorking();
                _appealManager.GetGridViewSection.ClickLoadMore();
                var appealresult = _appealManager.GetAppealsUsingAnalysts(paramLists["Userid"].Split(',')[0],
                    paramLists["Userid"].Split(',')[1], automatedBase.EnvironmentManager.Username);
                _appealManager.GetGridViewSection.GetGridListValueByCol(4)
                    .ShouldCollectionBeEquivalent(appealresult, "appeal result correct?");
            }
        }

        #endregion

        #region Private Methods

        private void SearchUsingAppealSeq(string appealSeq, AppealManagerPage _appealManager)
        {
            _appealManager.SelectDropDownListbyInputLabel("Quick Search", AppealQuickSearchTypeEnum.AllAppeals.GetStringValue());
            _appealManager.SelectSMTST();
            _appealManager.SetAppealSequence(appealSeq);
            _appealManager.ClickOnFindButton();
            _appealManager.WaitForWorkingAjaxMessageForBothDisplayAndHide();
        }

        private void CreateAppeal(AppealCreatorPage _appealCreator)
        {
            _appealCreator.ClickOnClaimLine(1);
            _appealCreator.SelectAppealRecordType();
            _appealCreator.SelectProduct(ProductEnum.CV.GetStringValue());
            _appealCreator.ClickOnSaveBtn();
            _appealCreator.WaitForWorking();
        }

        private void ValidateAppealSearchRowSorted(int col, int sortOptionRow, string colName, AppealManagerPage _appealManager)
        {
            _appealManager.ClickOnFilterOptionListRow(sortOptionRow);
            _appealManager.IsListStringSortedInAscendingOrder(col)
                .ShouldBeTrue(string.Format("{0} Should be sorted in Ascending Order", colName));
            _appealManager.ClickOnFilterOptionListRow(sortOptionRow);
            _appealManager.IsListStringSortedInDescendingOrder(col)
                .ShouldBeTrue(string.Format("{0} Should be sorted in Descending Order", colName));
        }
        
        private void ValidateFieldErrorMessageForComboBox(string fieldName, string value, string message, AppealManagerPage _appealManager)
        {
            _appealManager.SelectDropDownListbyInputLabel(fieldName, value);
            _appealManager.IsInvalidInputPresentByLabel(fieldName).ShouldBeTrue($"'{fieldName}' should be surrounded by red highlight");

            _appealManager.GetFieldErrorIconTooltipMessage(fieldName)
                .ShouldBeEqual(
                    message,
                    string.Format("Field Error Tooltip Message When <{0}> is selected only", fieldName));
            _appealManager.ClickOnFindButton();
            _appealManager.GetPageErrorMessage()
                    .ShouldBeEqual("Invalid or missing data must be resolved before search can be initiated.",
                        "Popup Message  when attempting to search when exclamation icon present");
            _appealManager.ClosePageError();
            _appealManager.ClearAll();
        }

        private void ValidateFieldErrorMessageForTextField(string fieldName,  string value, string message, AppealManagerPage _appealManager)
        {
            _appealManager.SetInputFieldByInputLabel(fieldName, value);

            _appealManager.GetFieldErrorIconTooltipMessage(fieldName)
                .ShouldBeEqual(
                    message,
                    string.Format("Field Error Tooltip Message When <{0}> is set", fieldName));
            _appealManager.ClickOnFindButton();
            _appealManager.GetPageErrorMessage()
                    .ShouldBeEqual("Invalid or missing data must be resolved before search can be initiated.",
                        "Popup Message  when attempting to search when exclamation icon present");
            _appealManager.ClosePageError();
            _appealManager.ClearAll();
        }

        private void ValidateFieldErrorMessageForDateRange(string fieldName, string message, AppealManagerPage _appealManager)
        {
            _appealManager.SetDateFieldFrom(fieldName, DateTime.Now.ToString("MM/d/yyyy"));
            _appealManager.SetDateFieldTo(fieldName, DateTime.Now.AddMonths(3).AddDays(1).ToString("MM/d/yyyy"));
            _appealManager.GetFieldErrorIconTooltipMessage(fieldName)
                .ShouldBeEqual(
                    message,
                    string.Format("Field Error Tooltip Message When <{0}> range greater than 3 months", fieldName));
            _appealManager.ClickOnFindButton();
            _appealManager.GetPageErrorMessage()
                    .ShouldBeEqual("Invalid or missing data must be resolved before search can be initiated.",
                        "Popup Message  when attempting to search when exclamation icon present");
            _appealManager.ClosePageError();
            _appealManager.ClearAll();
        }

        private void Verify_search_result_for_different_value_of_quick_search(string quickSearch, AppealManagerPage _appealManager)
        {
            _appealManager.SelectDropDownListbyInputLabel("Quick Search", quickSearch);
            _appealManager.SelectSMTST();
            _appealManager.ClickOnFindButton();
            _appealManager.WaitForWorking();
            var loadMoreValue = _appealManager.GetLoadMoreText();
            var numbers = Regex.Split(loadMoreValue, @"\D+").Where(s => s != String.Empty).Select(m => int.Parse(m.Trim())).ToList();
            var count = numbers[1] % 25 == 0 ? numbers[1] / 25 - 1 : numbers[1] / 25;
            for (var i = 0; i < count; i++)
            {
                _appealManager.ClickOnLoadMore();
            }

            var list = _appealManager.GetSearchResultListByCol(11)
                      .Distinct().ToList();
            
            if (quickSearch == AppealQuickSearchTypeEnum.AppealsDueToday.GetStringValue())
            {
                list.Count.ShouldBeEqual(2, "Only two types of status and that should be New and Mentor");
                list.ShouldCollectionBeEquivalent(new List<string>() { AppealStatusEnum.New.GetStringDisplayValue(), AppealStatusEnum.MentorReview.GetStringDisplayValue().Split(' ')[0] },
                    "Status should New and Mentor for " + quickSearch);

                _appealManager.IsSearchInputFieldDispalyedByLabel("Status").ShouldBeFalse("Status display for " + quickSearch);
                _appealManager.IsSearchInputFieldDispalyedByLabel("Due Date").ShouldBeFalse("Status display for " + quickSearch);
            }
            else if (quickSearch == AppealQuickSearchTypeEnum.AllUrgentAppeals.GetStringValue())
            {
                _appealManager.GetUrgentList()
                    .Distinct()
                    .ToList()
                    .ShouldNotContain(false, "All Appeals should be Urgent(Exclamation Icon)");
                list.Count.ShouldBeEqual(2, "Only two types of status and that should be New and Mentor");
                list.ShouldCollectionBeEquivalent(new List<string>(){AppealStatusEnum.New.GetStringDisplayValue(), AppealStatusEnum.MentorReview.GetStringDisplayValue().Split(' ')[0]},
                    "Status should New and Mentor for " + quickSearch);
                _appealManager.IsSearchInputFieldDispalyedByLabel("Status").ShouldBeFalse("Status display for " + quickSearch);
                _appealManager.IsSearchInputFieldDispalyedByLabel("Priority").ShouldBeFalse("Status display for " + quickSearch);
            }

            else if (quickSearch == AppealQuickSearchTypeEnum.AllRecordReviews.GetStringValue())
            {

                list.Count.ShouldBeEqual(2, "Only two types of status and that should be New and Mentor");
                list.ShouldCollectionBeEquivalent(new List<string>() { AppealStatusEnum.New.GetStringDisplayValue(), AppealStatusEnum.MentorReview.GetStringDisplayValue().Split(' ')[0] },
                    "Status should New and Mentor for " + quickSearch);
                var list1 = _appealManager.GetSearchResultListByCol(6)
                  .Distinct().ToList();
                list1.Count.ShouldBeEqual(1, "Only one type of Type='Record Review'");
                list1[0].ShouldBeEqual(AppealType.RecordReview.GetStringValue(),
                    "Status should New for " + quickSearch);
                _appealManager.IsSearchInputFieldDispalyedByLabel("Status").ShouldBeFalse("Status display for " + quickSearch);
                _appealManager.IsSearchInputFieldDispalyedByLabel("Type").ShouldBeFalse("Status display for " + quickSearch);
            }

            else if (quickSearch == AppealQuickSearchTypeEnum.OutstandingAppeals.GetStringValue())
            {
                var nonOutstandingStatus = new List<String>
                    {
                        AppealStatusEnum.Closed.GetStringValue(),
                        AppealStatusEnum.Complete.GetStringValue(),
                        AppealStatusEnum.DocumentsWaiting.GetStringValue(),
                        AppealStatusEnum.Open.GetStringValue(),
                        AppealStatusEnum.None.GetStringValue()
                    };

                list.ShouldCollectionBeNotEqual(nonOutstandingStatus,
                        "Not OutStanding Status should not display");
                _appealManager.IsSearchInputFieldDispalyedByLabel("Status").ShouldBeFalse("Status display for " + quickSearch);

            }
        }

        private void ValidateFieldSupportingMultipleValues(string label, IList<string> expectedDropDownList, AppealManagerPage _appealManager)
        {
            ValidateMultipleDropDownForDefaultValueAndExpectedList(label, expectedDropDownList, _appealManager);
            _appealManager.SelectSearchDropDownListForMultipleSelectValue(label,
                expectedDropDownList[0]);
            _appealManager.GetInputAttributeValueByLabel(label, "placeholder")
                .ShouldBeEqual(expectedDropDownList[0], label + "  single value selected");
            _appealManager.SelectSearchDropDownListForMultipleSelectValue(label,
                expectedDropDownList[1]);
            _appealManager.GetInputAttributeValueByLabel(label, "placeholder")
                .ShouldBeEqual("Multiple values selected", label + "  multiple value selected");
        }

        private void ValidateCategoryField(string label, IList<string> selectValue, AppealManagerPage _appealManager)
        {
            var actualDropDownList = _appealManager.GetAvailableDropDownList(label,false);
            actualDropDownList.Count.ShouldBeGreater(0, "List of " + label + " is greater than zero.");

            _appealManager.IsListInAscendingOrder(actualDropDownList).ShouldBeTrue(label + " should be sorted in alphabetical order.");
             _appealManager.GetInputAttributeValueByLabel(label, "placeholder")
                .ShouldBeEqual("Select one or more", label + " value defaults to 'select one or more'");
             _appealManager.SelectSearchDropDownListForMultipleSelectValue(label,
                selectValue[0]);
            _appealManager.GetInputAttributeValueByLabel(label, "placeholder")
                .ShouldBeEqual(selectValue[0], label + "  single value selected");
            _appealManager.SelectSearchDropDownListForMultipleSelectValue(label,
                selectValue[1]);
            _appealManager.GetInputAttributeValueByLabel(label, "placeholder")
                .ShouldBeEqual("Multiple values selected", label + "  multiple value selected");
        }

        private void ValidatePrimaryReviewerAndAssignedTo(string label, AppealManagerPage _appealManager, NewAutomatedBaseParallelRun automatedBase)
        {
            List<string> _appealAssignableUserList = _appealManager.GetPrimaryReviewerAssignedToList();

            _appealManager.GetSideBarPanelSearch.GetInputAttributeValueByLabel(label, "placeholder")
                .ShouldBeEqual("Select one or more", label + " value defaults to 'select one or more'");

            var reqDropDownList = _appealManager.GetSideBarPanelSearch.GetMultiSelectAvailableDropDownList(label);
            reqDropDownList.Remove("All");
            reqDropDownList.Remove("Clear");
            if (reqDropDownList.Count != 0)
            {
                /*  check for first , last and randomly generated index */
                var randm = new Random();
                var i = randm.Next(1, reqDropDownList.Count - 2);
                Console.WriteLine("Verify the first element of the list for the proper format.");
                //check the first element of list
                reqDropDownList[0].DoesNameContainsNameWithUserName()
                    .ShouldBeTrue(label + " should be in proper format of <firstname> <lastname> (user id)");
                //check the last element of list
                Console.WriteLine("Verify the last element of the list for the proper format.");
                reqDropDownList[reqDropDownList.Count - 1].DoesNameContainsNameWithUserName()
                    .ShouldBeTrue(label + " should be in proper format of <firstname> <lastname> (user id)");
                Console.WriteLine("Verify the random element of the list for the proper format.");
                //check the random element
                reqDropDownList[i].DoesNameContainsNameWithUserName()
                    .ShouldBeTrue(label + " should be in proper format of <firstname> <lastname> (user id)");
            }
            
            var searchList = reqDropDownList.Select(s => s.Substring(0, s.LastIndexOf('('))).ToList(); 
            searchList.ShouldCollectionBeEqual(_appealAssignableUserList, label + " List As Expected and sorted");
           
        }

        private void ValidateSingleDropDownForDefaultValueAndExpectedList(string label, IList<string> collectionToEqual,
            AppealManagerPage _appealManager, bool order = true)
        {
            var actualDropDownList = _appealManager.GetAvailableDropDownList(label);
            actualDropDownList.Count.ShouldBeGreater(0, "List of " + label + " is greater than zero.");
            if (collectionToEqual != null)
                actualDropDownList.ShouldCollectionBeEqual(collectionToEqual, label + " List As Expected");
            if (order)
            {
                actualDropDownList.Remove("All");
                actualDropDownList.IsInAscendingOrder().ShouldBeTrue(label + " should be sorted in alphabetical order.");
            }
            _appealManager.GetInputValueByLabel(label).ShouldBeEqual("All", label + " value defaults to All");
        }

        private void ValidateMultipleDropDownForDefaultValueAndExpectedList(string label,
            IList<string> collectionToEqual, AppealManagerPage _appealManager)
        {
            var listedOptionsList = _appealManager.GetAvailableDropDownList(label,false);
            listedOptionsList.Count.ShouldBeGreater(0, "List of " + label + " is greater than zero.");
            _appealManager.GetMultiSelectListedDropDownList(label).Contains("All")
                .ShouldBeTrue(
                    "A value of all displayed at the top of the list, followed by options sorted alphabetically.");
            listedOptionsList.ShouldCollectionBeEqual(collectionToEqual, label + " List As Expected");
            listedOptionsList.Remove("All");
            listedOptionsList.IsInAscendingOrder().ShouldBeTrue(label + " should be sorted in alphabetical order.");
           _appealManager.GetInputAttributeValueByLabel(label, "placeholder")
                .ShouldBeEqual("Select one or more", label + " value defaults to 'select one or more'");
        }

        private void ValidateDateRangePickerBehaviour(string label, AppealManagerPage _appealManager)
        {
            _appealManager.GetDateFieldPlaceholder(label, 1).ShouldBeEqual("00/00/0000", "Date range picker for " + label + " (from) default value");
            _appealManager.GetDateFieldPlaceholder(label, 1).ShouldBeEqual("00/00/0000", "Date range picker for " + label + " (to) default value");
            _appealManager.SetInputFieldByInputLabel(label, DateTime.Now.ToString("MM/d/yyyy")); //check numeric value can be typed
            _appealManager.GetDateFieldFrom(label).ShouldBeEqual(DateTime.Now.ToString("MM/d/yyyy"), label + " Checks numeric value is accepted");
            _appealManager.SetInputFieldByInputLabel(label, "");
            _appealManager.SetDateFieldFrom(label, DateTime.Now.ToString("MM/d/yyyy"));
            _appealManager.GetDateFieldTo(label).ShouldBeEqual(_appealManager.GetDateFieldFrom(label), label + " From value populated in To field as well.");
            _appealManager.SetDateFieldTo(label, DateTime.Now.AddMonths(3).AddDays(2).ToString("MM/d/yyyy"));
        }

        private void ValidateAlphaNumeric(string label, string message, string character,
            AppealManagerPage _appealManager)
        {
            _appealManager.SetInputFieldByInputLabel(label, character);

            _appealManager.GetPageErrorMessage()
                .ShouldBeEqual(message,
                    "Popup Error message is shown when unexpected " + character + "is passed to " + label);
            _appealManager.ClosePageError();
        }

        private void VerifyDueDateFunctionality(string label, AppealManagerPage _appealManager)
        {
            _appealManager.GetInputValueInDropDownListInEditAppealByLabel("Due Date")
                    .ShouldBeNullorEmpty("Due Date should Clear");
            _appealManager.IsCursorInDueDate().ShouldBeTrue("Cursor should be in Due Date");
            _appealManager.SendTabKeyInDueDate();
            _appealManager.WaitToLoadPageErrorPopupModal();
            _appealManager.IsPageErrorPopupModalPresent()
                .ShouldBeTrue(
                    "Popup should display When user attemps to navigate away from due date field without entering a valid date");
            _appealManager.GetPageErrorMessage()
                .ShouldBeEqual(string.Format("A valid due date must be indicated for this appeal when changing the {0}.", label),
                    "When user attemps to navigate away from due date field without entering a valid date");
            _appealManager.ClosePageError();
        }

        private void ValidateNoFiltersArePreApplied(string[] filtersList, string[] defaultFilterValue,
            AppealManagerPage _appealManager)
        {
            if(_appealManager.IsAdvancedSearchFilterIconDispalyed()) _appealManager.ClickOnAdvancedSearchFilterIcon(true);
            for (var i = 1; i < filtersList.Length; i++)
            {
                _appealManager.GetInputValueByLabel(filtersList[i]).ShouldBeEqual(defaultFilterValue[i]);
            }
            
        }

        private void ValdiateForDefaultValueOfBasicFilter(AppealManagerPage _appealManager)
        {
            StringFormatter.PrintMessageTitle("Validate For default behaviour of basic filter");
            _appealManager.GetInputValueByLabel("Appeal Sequence")
                .ShouldBeNullorEmpty("Appeal Sequence value defaults to blank");
            _appealManager.GetInputValueByLabel("Claim Sequence")
                .ShouldBeNullorEmpty("Claim Sequence value defaults to blank");
            _appealManager.GetInputValueByLabel("Claim No")
                .ShouldBeNullorEmpty("Claim No value defaults to blank ");

            _appealManager.GetInputValueByLabel("Client").ShouldBeEqual("All", "Client Search default filter equals All");

            _appealManager.IsInputFieldForRespectiveLabelDisabled("Claim Sequence")
                .ShouldBeTrue("On No Specific Client selection, Claim Sequence should be disabled. Is True?");
            _appealManager.IsInputFieldForRespectiveLabelDisabled("Claim No")
                .ShouldBeTrue("On No Specific Client selection, Claim No should be disabled. Is True?");
        }     

        #endregion
    }
}


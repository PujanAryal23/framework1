using System;
using System.Linq;
using System.Text.RegularExpressions;
using Nucleus.Service.PageServices.QA;
using Nucleus.Service.PageServices.Settings.User;
using Nucleus.Service.Support.Common.Constants;
using Nucleus.UIAutomation.TestSuites.Base;
using NUnit.Framework;
using UIAutomation.Framework.Utils;
using System.Diagnostics;
using Nucleus.Service.Data;
using System.Collections.Generic;
using Nucleus.Service.PageServices.Claim;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Environment;
using Nucleus.Service.Support.Menu;
using Nucleus.UIAutomation.TestSuites.Common;
using Org.BouncyCastle.Crypto.Parameters;
using UIAutomation.Framework.Core.Driver;

namespace Nucleus.UIAutomation.TestSuites.TestCases
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class QaManager
    {
        /* #region PRIVATE FIELDS
         private QaManagerPage _qaManager;
         private ProfileManagerPage _profileManager;
         private List<string> _analystUserList;
         private CommonValidations _commonValidations;
         private UserProfileSearchPage _userProfileSearch;
         #endregion

         #region OVERRIDE METHODS
         protected override void ClassInit()
         {
             try
             {
                 base.ClassInit();
                 _qaManager = QuickLaunch.NavigateToAnalystManager();
                 _commonValidations = new CommonValidations(CurrentPage);
                 RetrieveListFromDatabase();
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
             base.TestCleanUp();
             if (!CurrentPage.IsCurrentClientAsExpected(EnvironmentManager.TestClient))
             {
                 CheckTestClientAndSwitch();
             }

             if (string.Compare(UserType.CurrentUserType, UserType.HCIADMIN, StringComparison.OrdinalIgnoreCase) != 0)
             {
                 _qaManager = _qaManager.Logout().LoginAsHciAdminUser().NavigateToAnalystManager();
             }

             if (CurrentPage.GetPageHeader() != PageHeaderEnum.QAManager.GetStringValue())
             {
                 _qaManager = CurrentPage.NavigateToAnalystManager();
             }

             else
             {
                 _qaManager.ClickOnSidebarIcon();
                 _qaManager.ClickOnClearButton();
                 _qaManager.ClickOnFindButton();
             }

         }

         protected override void ClassCleanUp()
         {
             try
             {
                 _qaManager.CloseDbConnection();
             }

             finally
             {
                 base.ClassCleanUp();
             }
         }
         #endregion

         #region DBinteraction methods
         private void RetrieveListFromDatabase()
         {
             _analystUserList = _qaManager.GetAssignedToList();

         }

         #endregion*/

        #region PROTECTED PROPERTIES

        //protected string GetType().FullName
        //{
        //    get
        //    {
        //        return GetType().FullName;
        //    }
        //}

        #endregion

        #region TEST SUITES
        [Test, Category("SmokeTestDeployment")] //CAR-3088 (CAR-3149) + TANT-303
        public void Verify_Appeal_QA_Settings_Section()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                QaManagerPage _qaManager;
                automatedBase.CurrentPage = _qaManager = automatedBase.QuickLaunch.NavigateToAnalystManager();
                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramList =
                    automatedBase.DataHelper.GetTestData(GetType().FullName, TestName);
                var qaAppealTypes = paramList["QaAppealTypes"].Split(';').ToList();
                var userId = paramList["UserId"];
                var analyst = paramList["Analyst"];
                _qaManager.SearchByAnalyst(analyst);
                _qaManager.GetGridViewSection.ClickOnGridRowByRow();

                StringFormatter.PrintMessageTitle("Verification of the Appeal QA Settings form section");
                _qaManager.IsAppealQaSettingsHeaderPresent()
                    .ShouldBeTrue("The form header should be correctly displayed");
                _qaManager.IsAppealQaSettingsFormSectionPresent()
                    .ShouldBeTrue("Appeal QA Settings form section should be present");

                foreach (var qaAppealType in qaAppealTypes)
                {
                    _qaManager.SelectAppealQaOptionFromDropdown(qaAppealType);
                    if (qaAppealType == "No Appeal QA")
                    {
                        _qaManager.IsCheckBoxOptionDisplayedInAppealQaSettings("Specify Clients")
                            .ShouldBeFalse(
                                $"Specify Clients checkbox should not be displayed for {qaAppealType}");
                        _qaManager.IsCheckBoxOptionDisplayedInAppealQaSettings("Specify Categories")
                            .ShouldBeFalse(
                                $"Specify Categories checkbox should not be displayed for {qaAppealType}");
                    }
                    else if (qaAppealType == "Trainee QA" || qaAppealType == "Ongoing QA")
                    {
                        _qaManager.IsCheckBoxOptionDisplayedInAppealQaSettings("Specify Clients")
                            .ShouldBeTrue($"Specify Clients checkbox displayed for {qaAppealType}");
                        _qaManager.IsCheckBoxOptionDisplayedInAppealQaSettings("Specify Categories")
                            .ShouldBeTrue($"Specify categories checkbox displayed for {qaAppealType}");

                        VerifyClientAndCategoryFields();
                    }
                }

                #region LOCAL METHODS

                void VerifyClientAndCategoryFields()
                {
                    StringFormatter.PrintMessage("Verifying the Available Clients transfer component");
                    ClickCheckboxAndVerifyTransferComponent("Clients");

                    StringFormatter.PrintMessage("Verifying the Available Categories transfer component");
                    ClickCheckboxAndVerifyTransferComponent("Categories");

                    StringFormatter.PrintMessage("Unchecking the Specify Clients and Specify Categories checkboxes");
                    _qaManager.GetSideWindow.CheckOrUncheckByLabel("Specify Clients", false);
                    _qaManager.GetSideWindow.CheckOrUncheckByLabel("Specify Categories", false);

                    _qaManager.IsAvailableAssignedClientsComponentPresentByLabel("Available Clients")
                        .ShouldBeFalse("Available Clients transfer component should be hidden once Specify Clients is unchecked");
                    _qaManager.IsAvailableAssignedClientsComponentPresentByLabel("Available Categories")
                        .ShouldBeFalse("Available Categories transfer component should be hidden once Specify Clients is unchecked");
                }

                void ClickCheckboxAndVerifyTransferComponent(string transferComponentLabel)
                {
                    StringFormatter.PrintMessage($"Clicking Specify {transferComponentLabel} checkbox");
                    _qaManager.GetSideWindow.CheckOrUncheckByLabel($"Specify {transferComponentLabel}");

                    _qaManager.GetSideWindow.IsCheckboxCheckedByLabel($"Specify {transferComponentLabel}")
                        .ShouldBeTrue($"Specify {transferComponentLabel} checkbox should be checked");
                    _qaManager.IsAvailableAssignedClientsComponentPresentByLabel($"Available {transferComponentLabel}")
                        .ShouldBeTrue($"Available {transferComponentLabel} transfer component should be present when 'Specify {transferComponentLabel}' is checked");

                    //Moving all the Selected Clients to Available Clients column for testing
                    if (_qaManager.GenericGetAvailableAssignedList($"Selected {transferComponentLabel}").Count > 0)
                        _qaManager.GenericTransferAll($"Selected {transferComponentLabel}");

                    StringFormatter.PrintMessage($"Assigning all {transferComponentLabel}");
                    var availableList = _qaManager.GenericGetAvailableAssignedList($"Available {transferComponentLabel}");
                    _qaManager.GenericTransferAll($"Available {transferComponentLabel}");
                    _qaManager.GenericGetAvailableAssignedList($"Available {transferComponentLabel}")
                        .ShouldCollectionBeEmpty($"Available {transferComponentLabel} side should be empty when all {transferComponentLabel} are selected");

                    var assignedList = _qaManager.GenericGetAvailableAssignedList($"Selected {transferComponentLabel}");
                    assignedList
                        .ShouldCollectionBeEqual(availableList, $"All of the {transferComponentLabel} should be present in assigned clients side");

                    StringFormatter.PrintMessage($"Deselecting all {transferComponentLabel}");
                    _qaManager.GenericTransferAll($"Selected {transferComponentLabel}");
                    _qaManager.GenericGetAvailableAssignedList($"Selected {transferComponentLabel}")
                        .ShouldCollectionBeEmpty($"{transferComponentLabel} side should be empty when all the {transferComponentLabel} are deselected");

                    _qaManager.GenericGetAvailableAssignedList($"Available {transferComponentLabel}")
                        .ShouldCollectionBeEqual(availableList, $"All the deselected {transferComponentLabel} should appear in Available {transferComponentLabel} side");

                }
                #endregion
            }
        }

        [Test] //CAR-3016(CAR-3006)
        public void Verify_analyst_manager_accessibility()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                QaManagerPage _qaManager;

                automatedBase.CurrentPage =
                    _qaManager = automatedBase.QuickLaunch.NavigateToAnalystManager();
                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramList =
                    automatedBase.DataHelper.GetTestData(GetType().FullName, TestName);
                var userId = paramList["UserID"];
                var roles = paramList["Roles"].Split(',').ToList();
                try
                {
                    StringFormatter.PrintMessage("Addition of Appeal Category Read Only Role");
                    _qaManager.AddRoleForAuthorities(userId, roles[0]);

                    StringFormatter.PrintMessage("Verifying when user has Appeal Category Read Only role");
                    _qaManager.Logout().LoginAsHCIUserwithONLYDCIWorklistauthority().NavigateToAnalystManager();
                    _qaManager.GetGridViewSection.ClickOnGridRowByRow();
                    _qaManager.IsAnalystManagerFormIconPresentByTitle("Add PTO")
                        .ShouldBeTrue("Is Add PTO settings present?");
                    _qaManager.IsAnalystManagerFormIconPresentByTitle("Appeal Category Assignments")
                        .ShouldBeFalse("Is Appeal Category Assignments settings present?");
                    _qaManager.IsAnalystManagerFormIconPresentByTitle("Modify QA settings")
                        .ShouldBeFalse("Is Analyst QA settings present?");
                    _qaManager.IsAnalystManagerFormReadOnly()
                        .ShouldBeTrue("Is Appeal Category Assignment form read only?");
                    _qaManager.IsEditIconPresent().ShouldBeFalse("Is Edit icon present?");

                    StringFormatter.PrintMessage("Assigning only QA Analyst Role");
                    _qaManager.UpdateRoleForUser(userId, roles[0], roles[1]);
                    RefreshAndNavigateToAnalystManager();

                    StringFormatter.PrintMessage("Verification for user with only QA Analyst role");
                    _qaManager.IsAnalystManagerFormIconPresentByTitle("Add PTO")
                        .ShouldBeFalse("Is Add PTO settings present?");
                    _qaManager.IsAnalystManagerFormIconPresentByTitle("Appeal Category Assignments")
                        .ShouldBeFalse("Is Appeal Category Assignments settings present?");
                    _qaManager.IsAnalystManagerFormIconPresentByTitle("Modify QA settings")
                        .ShouldBeTrue("Is Analyst QA settings present?");
                    _qaManager.GetSelectedIconName()
                        .ShouldBeEqual("Modify QA settings", "'Modify QA settings icon should be selected'");
                    _qaManager.IsAnalystManagerFormReadOnly().ShouldBeFalse("Is Qa Settings form readonly?");

                    StringFormatter.PrintMessage("Assigning only Operation Manager role");
                    _qaManager.UpdateRoleForUser(userId, roles[1], roles[2]);
                    RefreshAndNavigateToAnalystManager();

                    StringFormatter.PrintMessage("Verification for user with only Operation Manager role");
                    _qaManager.IsAnalystManagerFormIconPresentByTitle("Add PTO")
                        .ShouldBeTrue("Is Add PTO settings present?");
                    _qaManager.IsAnalystManagerFormIconPresentByTitle("Appeal Category Assignments")
                        .ShouldBeTrue("Is Add Category Assignments present?");
                    _qaManager.IsAnalystManagerFormIconPresentByTitle("Modify QA settings")
                        .ShouldBeFalse("Is Analyst QA settings present?");
                    _qaManager.GetSelectedIconName()
                        .ShouldBeEqual("Add PTO", "'Add PTO settings icon should be selected'");
                    _qaManager.GetAnalystManagerFormTitle().ShouldBeEqual("Analyst Availability",
                        "Analyst Availability form should be opened by default");
                    _qaManager.IsEditIconDisabled()
                        .ShouldBeFalse("Is Edit icon disabled for the user with only Operation Manager role?");
                    _qaManager.ClickAnalystManagerIconByColNum(2);

                    _qaManager.WaitForCondition(() =>
                    {
                        return _qaManager.IsAnalystManagerFormPresentByFormName("Appeal Category Assignments");
                    });

                    _qaManager.IsAnalystManagerFormReadOnly()
                        .ShouldBeFalse("Is Appeal Category Assignment form read only?");


                    StringFormatter.PrintMessage("Adding QA Analyst role");
                    _qaManager.AddRoleForAuthorities(userId, roles[1]);

                    StringFormatter.PrintMessage(
                        "Verification for user with both Operation Manager and QA Analyst role");
                    RefreshAndNavigateToAnalystManager();
                    _qaManager.GetSelectedIconName()
                        .ShouldBeEqual("Modify QA settings", "'Modify QA settings icon should be selected'");
                    _qaManager.ClickAnalystManagerIconByColNum(1);
                    _qaManager.IsEditIconDisabled()
                        .ShouldBeFalse(
                            "Is Edit icon disabled for the user with Operation Manager role and Qa Analyst role but no Nucleus Admin role?");

                }

                finally
                {
                    StringFormatter.PrintMessage("Removing added roles");
                    foreach (var role in roles)
                    {
                        _qaManager.RemoveRoleForAuthorities(userId, role);
                    }
                }

                void RefreshAndNavigateToAnalystManager()
                {
                    _qaManager.WaitForStaticTime(7000);
                    _qaManager.RefreshPage();
                    _qaManager.NavigateToAnalystManager();
                    _qaManager.GetGridViewSection.ClickOnGridRowByRow();
                }

            }
        }

        [Test, Category("SmokeTestDeployment")] //TANT-302
        public void Verify_claim_qa_settings_form()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                QaManagerPage _qaManager;
                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramList =
                    automatedBase.DataHelper.GetTestData(GetType().FullName, TestName);

                var analyst = paramList["Analyst"];
                var reviewClaimCount = paramList["ReviewClaimCount"];
                var maxPerDayClaimCount = paramList["MaxPerDayClaimCount"];
                var percentClaims = paramList["PercentClaims"];

                automatedBase.CurrentPage = _qaManager = automatedBase.QuickLaunch.NavigateToAnalystManager();

                _qaManager.SearchByAnalyst(analyst);
                _qaManager.GetGridViewSection.ClickOnGridRowByRow();

                StringFormatter.PrintMessageTitle("Verification of the Claim QA Settings form section");
                _qaManager.IsSectionWithHeaderNamePresent("Claim QA Settings")
                    .ShouldBeTrue("Claim QA Settings form should be shown when the analyst is selected from the grid");

                _qaManager.IsQcBusinessDaysOnlyOptionPresent().ShouldBeTrue("QC Business days only checkbox should be present");

                _qaManager.IsDaterangePresent().ShouldBeTrue("Date range field should be displayed");

                var beginDate = DateTime.Now.AddDays(-1).ToString("MM/dd/yyyy");
                var endDate = DateTime.Now.ToString("MM/dd/yyyy");

                _qaManager.SetDateWithoutUsingCalender("Date Range", beginDate, true);
                _qaManager.SetDateWithoutUsingCalender("Date Range", endDate, false);

                _qaManager.GetBeginDate().ShouldBeEqual(beginDate, "Entered begin date should be displayed correctly");
                _qaManager.GetEndDate().ShouldBeEqual(endDate, "Entered end date should be displayed correctly");

                if (_qaManager.GetSelectedClientCount() == 0)
                {
                    _qaManager.SetClient(ClientEnum.SMTST.ToString());
                    _qaManager.GetClient().ShouldBeEqual(ClientEnum.SMTST.ToString());
                }

                _qaManager.SetClaimsCount(reviewClaimCount);
                _qaManager.SetMaxClaimsCount(maxPerDayClaimCount);

                _qaManager.GetClaimsCount().ShouldBeEqual(reviewClaimCount);
                _qaManager.GetMaxClaimsCount().ShouldBeEqual(maxPerDayClaimCount);

                _qaManager.GetPercentOfClaims()
                    .ShouldBeEqual(percentClaims, "Percent of Claims should be calculated correctly");

                _qaManager.IsQaQcRadioButtonsPresentByLabel("QA").ShouldBeTrue("QA Radio button should be present in the form");
                _qaManager.IsQaQcRadioButtonsPresentByLabel("QC").ShouldBeTrue("QC Radio button should be present in the form");

                _qaManager.SelectRadioButtonOptionByLabel("QA");
                _qaManager.IsQaQcRadioButtonSelectedByLabel("QA").ShouldBeTrue("Radio button should be selectable");

                _qaManager.SelectRadioButtonOptionByLabel("QC");
                _qaManager.IsQaQcRadioButtonSelectedByLabel("QC").ShouldBeTrue("Radio button should be selectable");
                

            }
        }

        [Test, Category("SmokeTestDeployment")] //TANT-104
        public void Verify_Find_Analyst_panel_and_QA_Target_Icon()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                QaManagerPage _qaManager;

                automatedBase.CurrentPage =
                    _qaManager = automatedBase.QuickLaunch.NavigateToAnalystManager();
                StringFormatter.PrintMessage("Verify Find Analyst Panel");
                _qaManager.IsFindAnalystppealSectionPresent()
                    .ShouldBeTrue("Find Analyst panel on sidebar is open by default when user lands on page.");
                _qaManager.ClickOnSidebarIcon(true);
                _qaManager.WaitToOpenCloseWorkList();
                _qaManager.IsFindAnalystppealSectionPresent()
                    .ShouldBeFalse("Find Analyst panel on sidebar is hidden when toggle button is clicked.");
                _qaManager.ClickOnSidebarIcon();
                _qaManager.WaitToOpenCloseWorkList(false);

                //StringFormatter.PrintMessage("Verify Qa Details Form Presented");
                //_qaManager.ClickOnGridRowToOpenSideWindow();
                //_qaManager.IsEditFormDisplayed()
                //    .ShouldBeTrue("An edit form opens below the record for editing should be true.");
                //_qaManager.ClickOnCancelLink();
            }
        }

        [Test] //US66660
        [Retry(3)]
        public void Verify_qa_target_results_history_details()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                QaManagerPage _qaManager;

                automatedBase.CurrentPage =
                    _qaManager = automatedBase.QuickLaunch.NavigateToAnalystManager();
                var TestName = new StackFrame(true).GetMethod().Name;
                var analyst = automatedBase.DataHelper.GetSingleTestData(GetType().FullName,
                    TestName, "Analyst", "Value");
                _qaManager.SearchByAnalyst(analyst);
                _qaManager.WaitForWorkingAjaxMessage();
                _qaManager.ClickOnSearchListRow(1);
                _qaManager.ClickOnSidebarIcon(true);
                var createdDate = _qaManager.GetQaTargetHistoryDetailsContentValueByLabelAndRow("Date:");
                createdDate.IsDateInFormat().ShouldBeTrue("Date should be in MM/DD/YYYY format");
                var claSeqWithClientList =
                    _qaManager.GetQaReviewClaimByUser(automatedBase.EnvironmentManager.HciAdminUsername, createdDate);
                StringFormatter.PrintMessageTitle(
                    "Verification of  Date in QA Target Results corresponds ot the date that claims are flagged for QA(approve date)");
                foreach (var list in claSeqWithClientList)
                {
                    _qaManager.IsCreateDateEqualsToCorrespondsClaimApproveDate(list[0], list[1], createdDate)
                        .ShouldBeTrue(string.Format("Claseq <{0}> for client <{1}> should be Approved on <{2}>",
                            list[0], list[1], createdDate));
                }

                var totalFlaggedClaims =
                    Convert.ToInt32(_qaManager.GetQaTargetHistoryDetailsContentValueByLabelAndRow("Total Claims:"));
                totalFlaggedClaims.ShouldBeEqual(
                    _qaManager.GetExpectedTotalFlaggedClaimsCount(automatedBase.EnvironmentManager.HciAdminUsername, createdDate),
                    "Total Flagged Claims should equal to database");
                var totalPassedClaims =
                    Convert.ToInt32(_qaManager.GetQaTargetHistoryDetailsContentValueByLabelAndRow("Total Passed:"));
                totalPassedClaims.ShouldBeEqual(
                    _qaManager.GetExpectedTotalPassedClaimsCount(automatedBase.EnvironmentManager.HciAdminUsername, createdDate),
                    "Total Passed Claims should equal to database");
                _qaManager.GetQaTargetHistoryDetailsContentValueByLabelAndRow("QA Score:").ShouldBeEqual(
                    Math.Round(totalPassedClaims * 100.0 / totalFlaggedClaims, 1, MidpointRounding.AwayFromZero) + "%",
                    "QA Score Percentage calculation should correct and display in proper format");
                _qaManager.IsCreatedDateSortedInDescendingOrder().ShouldBeTrue("Is Created Date in descending order?");
            }
        }

        [Test] //CAR-2976(CAR-2786)
        [Order(1)]
        public void Verify_current_appeal_category_assignments_by_analyst()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                QaManagerPage _qaManager;

                automatedBase.CurrentPage =
                    _qaManager = automatedBase.QuickLaunch.NavigateToAnalystManager();
                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramList =
                    automatedBase.DataHelper.GetTestData(GetType().FullName, TestName);
                var analysts = paramList["Analysts"].Split(',').ToList();
                var formTitle = paramList["FormTitle"];
                var titles = paramList["Titles"].Split(',').ToList();
                var recordLessMessage = paramList["RecordLessMessage"];
                var userSeq = paramList["UserSeq"];
                var labels = paramList["Labels"].Split(',').ToList();
                var columnNames = paramList["ColumnNames"].Split(',').ToList();
                var currentNonRestrictedClaimsAssignmentsDb =
                    _qaManager.GetCurrentRestrictedAndNonRestrictedClaimsAssignmentsByAnalystsFromDb(columnNames[1],
                        userSeq);
                var currentRestrictedClaimsAssignmentsDb =
                    _qaManager.GetCurrentRestrictedAndNonRestrictedClaimsAssignmentsByAnalystsFromDb(columnNames[0],
                        userSeq);

                StringFormatter.PrintMessage("Navigating by analyst without claims assignments");
                _qaManager.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Analyst", analysts[0]);
                _qaManager.GetSideBarPanelSearch.ClickOnFindButton();
                _qaManager.GetGridViewSection.ClickOnGridRowByRow();
                _qaManager.ClickAnalystManagerIconByColNum(2);
                _qaManager.WaitForSpinner();

                StringFormatter.PrintMessage("Specification verification");
                _qaManager.IsAnalystManagerFormPresentByFormName(formTitle)
                    .ShouldBeTrue("Is Appeal Category Assignments form present?");
                _qaManager.IsAnalystManagerIconPresentByColNum(2)
                    .ShouldBeTrue("Is Appeal Category Assignments icon present?");
                _qaManager.GetTitleOfAnalystManagerIconByColNum(2)
                    .ShouldBeEqual(formTitle, "Tool tip value should match");
                _qaManager.IsAppealFormPresent().ShouldBeTrue("Is Add Category Assignment form present?");
                _qaManager.IsAnalystManagerTitlePresentByTitleName(titles[0])
                    .ShouldBeTrue("Is Add Category Assignment title present?");
                _qaManager.IsAnalystManagerTitlePresentByTitleName(titles[1])
                    .ShouldBeTrue("Is Category ID title present?");
                _qaManager.IsDropdownPresentByLabel(titles[1]).ShouldBeTrue("Is Category ID drop down present?");
                _qaManager.IsAnalystManagerTitlePresentByTitleName(titles[2])
                    .ShouldBeTrue("Is Current Non-Restricted Claims Assignments title present?");
                _qaManager.IsAnalystManagerTitlePresentByTitleName(titles[3])
                    .ShouldBeTrue("Is Current Restricted Claims Assignments title present?");
                _qaManager.IsDownCaretIconPresentByLabel(titles[2])
                    .ShouldBeTrue("Is down caret icon present for Current Non-Restricted Claims Assignments?");
                _qaManager.IsAnalystManagerTitlePresentByTitleName(titles[3])
                    .ShouldBeTrue("Is down caret icon present for Current Restricted Claims Assignments?");

                StringFormatter.PrintMessage("Verification of no records for analyst without claims assignments");
                _qaManager.GetClaimsAssignmentsNoRecordsByLabel(1)
                    .ShouldBeEqual(recordLessMessage,
                        "Records are not present for Analyst without non restricted claims assignments");
                _qaManager.GetClaimsAssignmentsNoRecordsByLabel(2)
                    .ShouldBeEqual(recordLessMessage,
                        "Records are not present for Analyst without restricted claims assignments");
                _qaManager.ClickDownCaretIconByLabel(titles[2]);
                _qaManager.IsRightCaretIconPresentByLabel(titles[2])
                    .ShouldBeTrue("Right caret icon should be displayed after clicking Down caret icon");
                _qaManager.ClickDownCaretIconByLabel(titles[3]);
                _qaManager.IsRightCaretIconPresentByLabel(titles[3])
                    .ShouldBeTrue("Right caret icon should be displayed after clicking Down caret icon");


                StringFormatter.PrintMessage(
                    "Navigation to Appeal Category Assignment by analyst with claims assignments");
                _qaManager.ClickOnSidebarIcon();
                _qaManager.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Analyst", analysts[1]);
                _qaManager.GetSideBarPanelSearch.ClickOnFindButton();
                _qaManager.GetGridViewSection.ClickOnGridRowByRow();
                _qaManager.ClickAnalystManagerIconByColNum(2);

                StringFormatter.PrintMessage("Verification that categories are listed in ascending order");
                _qaManager.GetListOfClaimsAssignmentParticularValueByTitleRowAndCol(titles[2], 1, 2)
                    .IsInAscendingOrder()
                    .ShouldBeTrue(
                        "Are categories listed in ascending order for Current Non-Restricted Claims Assignments?");
                _qaManager.GetListOfClaimsAssignmentParticularValueByTitleRowAndCol(titles[3], 1, 2)
                    .IsInAscendingOrder()
                    .ShouldBeTrue(
                        "Are categories listed in ascending order for Current Restricted Claims Assignments?");

                StringFormatter.PrintMessage("Verification of presence of Delete icon");
                _qaManager.IsDeleteIconPresentByTitleAndLineNumber(titles[2], 1)
                    .ShouldBeTrue("Is delete icon present?");

                StringFormatter.PrintMessage("Verification of presence of Proc code and Trig code labels");
                _qaManager.GetClaimsAssignmentsRecordLabelByLabelLineNoRowCol(titles[2], 1, 2)
                    .ShouldCollectionBeEqual(labels, "Proc and Trig labels should be present");

                StringFormatter.PrintMessage(
                    "Verification of claims assignments list for Current Non-Restricted Claims Assignments");
                for (int i = 1; i <= _qaManager.GetCountOfClaimsAssignmentsRecordRowsByLabel(titles[2]); i++)
                {
                    _qaManager.GetClaimsAssignmentsListByLabelAndRow(titles[2], i)
                        .ShouldCollectionBeEqual(currentNonRestrictedClaimsAssignmentsDb[i - 1], "Data correct?");
                }

                StringFormatter.PrintMessage(
                    "Verification of claims assignments list for Current Restricted Claims Assignments");
                for (int i = 1; i <= _qaManager.GetCountOfClaimsAssignmentsRecordRowsByLabel(titles[3]); i++)
                {
                    _qaManager.GetClaimsAssignmentsListByLabelAndRow(titles[3], i)
                        .ShouldCollectionBeEqual(currentRestrictedClaimsAssignmentsDb[i - 1], "Data correct?");
                }

            }
        }

        [Test] //CAR-2977(CAR-2948)
        [NonParallelizable]
        public void Verify_Addition_And_Deletion_Appeal_Category_Assignments_By_Analyst()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                QaManagerPage _qaManager;

                automatedBase.CurrentPage =
                    _qaManager = automatedBase.QuickLaunch.NavigateToAnalystManager();
                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramList =
                    automatedBase.DataHelper.GetTestData(GetType().FullName, TestName);
                var categoryCodesFromDatabase = _qaManager.GetCategoryIdsFromDatabase();
                var restrictionOptions = paramList["RestrictionOptions"].Split(',').ToList();
                var categoryList = paramList["CategoryList"].Split(',').ToList();
                var assignmentCategory = paramList["AssignmentCategory"].Split(',').ToList();
                var analystList = paramList["Analysts"].Split(',').ToList();
                var userIdList = paramList["UserIds"].Split(',').ToList();
                var expectedLabels = paramList["Labels"].Split(',').ToList();
                var assignedRestriction = String.Join(",",
                    _qaManager.GetAssignedRestrictionDescriptionForUser(userIdList[1]).ToArray());
                var userNameIdPair = paramList["UserNameIdPair"];
                List<string> restrictedAnalystFromDb, unrestrictedAnalystFromDb;
                List<string> auditDataFromDatabase;
                SearchByAnalystAndNavigateToAppealCategoryAssignment(_qaManager, analystList[0]);
                Random rnd = new Random();

                try
                {
                    #region Verification of Category ID Details

                    StringFormatter.PrintMessageTitle("Verify Contents of Category ID Dropdown");
                    _qaManager.IsDropdownPresentByLabel("Category ID").ShouldBeTrue("Dropdown Present ?");
                    var actualDropDownList = _qaManager.GetSideWindow.GetAvailableDropDownList("Category ID")
                        .Where(s => !s.Equals("")).ToList();

                    actualDropDownList.ShouldCollectionBeEqual(categoryCodesFromDatabase,
                        "Category ID List should match");
                    actualDropDownList.IsInAscendingOrder().ShouldBeTrue("Category IDs should be in ascending order");
                    _qaManager.GetSideWindow.SelectDropDownListValueByLabel("Category ID", actualDropDownList[8],
                        false); //check for type ahead functionality
                    _qaManager.GetSideWindow.SelectedDropDownOptionsByLabel("Category ID")
                        .ShouldBeEqual(actualDropDownList[8],
                            "Verify only single value can be selected in Category ID field");

                    StringFormatter.PrintMessageTitle("Verify category details in the form");
                    var randomlySelectedCategory = actualDropDownList[rnd.Next(actualDropDownList.Count)];
                    _qaManager.GetSideWindow.SelectDropDownListValueByLabel("Category ID", categoryList[0]);
                    _qaManager.IsCategoryDetailsSectionPresent().ShouldBeTrue("Category details should be displayed");
                    var categoryCodeDetailsFromDb = _qaManager.GetCategoryDetailsByCategoryCode(categoryList[0]);

                    for (int i = 0; i < categoryCodeDetailsFromDb.Count; i++)
                    {
                        _qaManager.GetCategoryDetailsValueByRowColumn(1, 1, i + 1).ShouldBeEqual(
                            categoryCodeDetailsFromDb[i][0],
                            "Category ID should match against database");
                        _qaManager.GetCategoryDetailsValueByRowColumn(1, 2, i + 1).ShouldBeEqual(
                            categoryCodeDetailsFromDb[i][1],
                            "Client value should match against database");
                        _qaManager.GetCategoryDetailsValueByRowColumn(1, 3, i + 1).ShouldBeEqual(
                            categoryCodeDetailsFromDb[i][2],
                            "Product value should match against database");
                        _qaManager.GetCategoryDetailsLabel(2, 1, i + 1)
                            .ShouldBeEqual(expectedLabels[0], "Proc Label should match");
                        _qaManager.GetCategoryDetailsValueByRowColumn(2, 1, i + 1).ShouldBeEqual(
                            categoryCodeDetailsFromDb[i][3],
                            "Proc Code value should match against database");
                        _qaManager.GetCategoryDetailsLabel(2, 2, i + 1)
                            .ShouldBeEqual(expectedLabels[1], "Trig Label should match");
                        _qaManager.GetCategoryDetailsValueByRowColumn(2, 2, i + 1).ShouldBeEqual(
                            categoryCodeDetailsFromDb[i][4],
                            "Product value should match against database");
                    }

                    _qaManager.ClickOnCategoryDetailsByLineNo(1);
                    _qaManager.GetMessageForUsersWithClaimAccessRestrictionAssigned().ShouldBeEqual(
                        $"This user has access to the following restrictions: {assignedRestriction}. Do you wish to assign them to the restricted appeals, non-restricted appeals, or both?");
                    _qaManager.GetRestrictionOptions()
                        .ShouldCollectionBeEqual(restrictionOptions, "Options Should Match");

                    StringFormatter.PrintMessage("Verify Popup error message when option is not selected");
                    _qaManager.GetSideWindow.Save();
                    ValidateForPopUpMessage(_qaManager, "Popup error message should be present",
                        "Restriction selection is required.");

                    StringFormatter.PrintMessage(
                        "Verify when cancel is clicked in the form selected category will be deselected");
                    _qaManager.GetSideWindow.Cancel();
                    _qaManager.IsCategoryDetailsSectionPresent().ShouldBeFalse("Details Section should not be present");

                    #endregion

                    #region Verification of Assigning categories to analysts having different restriction options

                    StringFormatter.PrintMessageTitle(
                        "Verify Addition of different category assignment for user with Restriction Access");
                    for (int j = 0; j < restrictionOptions.Count; j++)
                    {
                        _qaManager.GetSideWindow.SelectDropDownListValueByLabel("Category ID", categoryList[j], false);
                        _qaManager.ClickOnCategoryDetailsByLineNo(1);
                        _qaManager.SelectRadioButtonOptionByLabel(restrictionOptions[j]);
                        _qaManager.GetSideWindow.Save();
                        _qaManager.ClickOkCancelOnConfirmationModal(true);
                        _qaManager.WaitForWorking();

                        if (restrictionOptions[j] == "Both")
                        {
                            _qaManager.IsCategoryPresentInRespectiveClaimsAssignment(categoryList[j],
                                    assignmentCategory[j - 1])
                                .ShouldBeTrue("Category Should Be Added");
                            _qaManager.IsCategoryPresentInRespectiveClaimsAssignment(categoryList[j],
                                    assignmentCategory[j - 2])
                                .ShouldBeTrue("Category Should Be Added");

                            StringFormatter.PrintMessage(
                                "Verify Categories That Are Already Assigned To The User In Both Restricted and Non-Restricted Options Will Be Shown Disabled");
                            _qaManager.GetSideWindow.SelectDropDownListValueByLabel("Category ID", categoryList[j],
                                false);
                            _qaManager.IsCategoryDetailsSectionDisabled().ShouldBeTrue("Category Should Be Disabled");

                            StringFormatter.PrintMessage(
                                "Verify Addition Of User To The Analyst list shown Appeal Category Manager");
                            unrestrictedAnalystFromDb =
                                _qaManager.GetAssignedAnalystForACategoryFromDb(restrictionOptions[j - 1],
                                    categoryList[j]);
                            restrictedAnalystFromDb =
                                _qaManager.GetAssignedAnalystForACategoryFromDb(restrictionOptions[j - 2],
                                    categoryList[j]);

                            var appealCategoryManagerPage = _qaManager.OpenAppealCategoryManagerInNewTab();
                            if (!appealCategoryManagerPage.GetSideBarPanelSearch.IsSideBarPanelOpen())
                                appealCategoryManagerPage.GetSideBarPanelSearch.OpenSidebarPanel();
                            appealCategoryManagerPage.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Category ID", categoryList[j]);
                            appealCategoryManagerPage.GetSideBarPanelSearch.ClickOnFindButton();
                            appealCategoryManagerPage.WaitForWorking();
                            appealCategoryManagerPage.ClickOnAppealCategoryCatId(categoryList[j]);
                            appealCategoryManagerPage.IsAnalystPresentInRestrictedAssignedAnalystList(userNameIdPair)
                                .ShouldBeTrue("Analyst should be added in restricted group");
                            appealCategoryManagerPage.IsAnalystPresentInNonRestrictedAssignedAnalystList(userNameIdPair)
                                .ShouldBeTrue("Analyst should be added in non-restricted group");
                            appealCategoryManagerPage.GetRestrictedAssignedAnalystList()
                                .ShouldCollectionBeEquivalent(restrictedAnalystFromDb,
                                    "Analyst list should match against database");
                            appealCategoryManagerPage.GetNonRestrictedAssignedAnalystList()
                                .ShouldCollectionBeEquivalent(unrestrictedAnalystFromDb,
                                    "Analyst list should match against database");

                            StringFormatter.PrintMessage("Verify Audit In Appeal Category Audit History");
                            auditDataFromDatabase =
                                appealCategoryManagerPage.GetLatestAppealCategoryAuditHistoryFromDatabase(
                                    categoryList[j]);
                            appealCategoryManagerPage.ClickAppealCategoryAuditHistoryIcon();
                            appealCategoryManagerPage.GetAuditRowCount()
                                .ShouldBeGreater(0, "Audit should be present");
                            appealCategoryManagerPage.GetAnalaystInAuditSection()
                                .AssertIsContained(userIdList[1], "Audit Should Be correct");
                            appealCategoryManagerPage.GetAnalaystInAuditSection(true)
                                .AssertIsContained(userIdList[1], "Audit Should Be correct");
                            appealCategoryManagerPage.GetLatestAuditHistoryInAppealCategoryManager()
                                .ShouldCollectionBeEquivalent(auditDataFromDatabase,
                                    "Audit Data Should Match Against Database");

                            appealCategoryManagerPage.SwitchTab(automatedBase.CurrentPage.CurrentWindowHandle);

                            for (int k = 1; k <= 2; k++)
                            {
                                _qaManager.ClickOnDeleteIconByOptionandLineNo(assignmentCategory[j - k], 1);
                                _qaManager.ClickOkCancelOnConfirmationModal(true);
                                _qaManager.IsCategoryPresentInRespectiveClaimsAssignment(categoryList[j],
                                    assignmentCategory[j - k]).ShouldBeFalse("Category Should Be Deleted");
                            }

                            _qaManager.SwitchTab(automatedBase.CurrentPage.CurrentWindowHandle);
                            appealCategoryManagerPage.RefreshPage(false);
                            appealCategoryManagerPage.ClickOnAppealCategoryAnalystDetailsIcon();
                            appealCategoryManagerPage.IsAnalystPresentInNonRestrictedAssignedAnalystList(userNameIdPair)
                                .ShouldBeFalse("Analyst should be deleted from non-restricted analyst group");
                            appealCategoryManagerPage.IsAnalystPresentInRestrictedAssignedAnalystList(userNameIdPair)
                                .ShouldBeFalse("Analyst Should Be Deleted from restricted analyst group");
                            auditDataFromDatabase =
                                appealCategoryManagerPage.GetLatestAppealCategoryAuditHistoryFromDatabase(
                                    categoryList[j]);
                            appealCategoryManagerPage.ClickAppealCategoryAuditHistoryIcon();
                            appealCategoryManagerPage.GetAnalaystInAuditSection().ShouldNotBeTheSameAs(userIdList[1],
                                "Non-restricted analysts in Audit Should Be correct");
                            appealCategoryManagerPage.GetAnalaystInAuditSection(true)
                                .ShouldNotBeTheSameAs(userIdList[1], "Restricted analyst in Audit Should Be correct");
                            appealCategoryManagerPage.GetLatestAuditHistoryInAppealCategoryManager()
                                .ShouldCollectionBeEquivalent(auditDataFromDatabase,
                                    "Latest Audit Data Should Match Against Database");
                            appealCategoryManagerPage.SwitchTab(automatedBase.CurrentPage.CurrentWindowHandle);
                            _qaManager.CloseAnyTabIfExist();
                        }

                        else
                        {
                            _qaManager.IsCategoryPresentInRespectiveClaimsAssignment(categoryList[j],
                                    assignmentCategory[j])
                                .ShouldBeTrue("Category Should Be Added");

                            StringFormatter.PrintMessage(
                                "Verify Addition Of User To The Analyst list shown in Appeal Category Manager");
                            var appealCategoryManagerPage = _qaManager.OpenAppealCategoryManagerInNewTab();
                            if(!appealCategoryManagerPage.GetSideBarPanelSearch.IsSideBarPanelOpen())
                                appealCategoryManagerPage.GetSideBarPanelSearch.OpenSidebarPanel();
                            appealCategoryManagerPage.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Category ID", categoryList[j]);
                            appealCategoryManagerPage.GetSideBarPanelSearch.ClickOnFindButton();
                            appealCategoryManagerPage.WaitForWorking();
                            appealCategoryManagerPage.ClickOnAppealCategoryCatId(categoryList[j]);
                            auditDataFromDatabase =
                                appealCategoryManagerPage.GetLatestAppealCategoryAuditHistoryFromDatabase(
                                    categoryList[j]);

                            if (restrictionOptions[j] == "Restricted")
                            {
                                restrictedAnalystFromDb =
                                    _qaManager.GetAssignedAnalystForACategoryFromDb(restrictionOptions[j],
                                        categoryList[j]);
                                appealCategoryManagerPage
                                    .IsAnalystPresentInRestrictedAssignedAnalystList(userNameIdPair)
                                    .ShouldBeTrue("Analyst should be added");
                                appealCategoryManagerPage.GetRestrictedAssignedAnalystList()
                                    .ShouldCollectionBeEquivalent(restrictedAnalystFromDb,
                                        "Analyst list should match against database");
                            }
                            else
                            {
                                unrestrictedAnalystFromDb =
                                    _qaManager.GetAssignedAnalystForACategoryFromDb(restrictionOptions[j],
                                        categoryList[j]);
                                appealCategoryManagerPage
                                    .IsAnalystPresentInNonRestrictedAssignedAnalystList(userNameIdPair)
                                    .ShouldBeTrue("Analyst Should Be Deleted");
                                appealCategoryManagerPage.GetNonRestrictedAssignedAnalystList()
                                    .ShouldCollectionBeEquivalent(unrestrictedAnalystFromDb,
                                        "Unrestricted Analyst list should match against database");
                            }

                            StringFormatter.PrintMessage("Verify Audit In Appeal Category Audit History");
                            appealCategoryManagerPage.ClickAppealCategoryAuditHistoryIcon();
                            appealCategoryManagerPage.GetAuditRowCount()
                                .ShouldBeGreater(0, "Audit should be written");
                            if (restrictionOptions[j] == "Restricted")
                                appealCategoryManagerPage.GetAnalaystInAuditSection(true)
                                    .AssertIsContained(userIdList[1], "User should be in restricted user in the audit");
                            else
                                appealCategoryManagerPage.GetAnalaystInAuditSection()
                                    .AssertIsContained(userIdList[1], "Audit Should Be correct");
                            appealCategoryManagerPage.GetLatestAuditHistoryInAppealCategoryManager()
                                .ShouldCollectionBeEquivalent(auditDataFromDatabase, "Audit Data Should Match From Db");
                            appealCategoryManagerPage.SwitchTab(automatedBase.CurrentPage.CurrentWindowHandle);

                            StringFormatter.PrintMessage("Verify Deletion of Category Assignment");
                            _qaManager.ClickOnDeleteIconByOptionandLineNo(assignmentCategory[j], 1);

                            _qaManager.ClickOkCancelOnConfirmationModal(true);
                            _qaManager.IsCategoryPresentInRespectiveClaimsAssignment(categoryList[j],
                                    assignmentCategory[j])
                                .ShouldBeFalse("Category Should Be Deleted");

                            _qaManager.SwitchTab(automatedBase.CurrentPage.CurrentWindowHandle);
                            appealCategoryManagerPage.RefreshPage(false);
                            appealCategoryManagerPage.ClickOnAppealCategoryAnalystDetailsIcon();

                            if (restrictionOptions[j] == "Restricted")
                                appealCategoryManagerPage
                                    .IsAnalystPresentInRestrictedAssignedAnalystList(userNameIdPair)
                                    .ShouldBeFalse("Analyst should be deleted");
                            else
                                appealCategoryManagerPage
                                    .IsAnalystPresentInNonRestrictedAssignedAnalystList(userNameIdPair)
                                    .ShouldBeFalse("Analyst Should Be Deleted");

                            auditDataFromDatabase =
                                appealCategoryManagerPage.GetLatestAppealCategoryAuditHistoryFromDatabase(
                                    categoryList[j]);
                            appealCategoryManagerPage.ClickAppealCategoryAuditHistoryIcon();

                            if (restrictionOptions[j] == "Restricted")
                                appealCategoryManagerPage.GetAnalaystInAuditSection(true)
                                    .ShouldNotBeTheSameAs(userIdList[1], "Audit Should Be correct");
                            else
                                appealCategoryManagerPage.GetAnalaystInAuditSection()
                                    .ShouldNotBeTheSameAs(userIdList[1], "Audit Should Be correct");

                            appealCategoryManagerPage.GetLatestAuditHistoryInAppealCategoryManager()
                                .ShouldCollectionBeEquivalent(auditDataFromDatabase, "Audit Data Should Match From Db");
                            appealCategoryManagerPage.SwitchTab(automatedBase.CurrentPage.CurrentWindowHandle);
                            _qaManager.CloseAnyTabIfExist();
                        }
                    }

                    #endregion

                    #region Verification of assigning categories to analyst without any restriction

                    StringFormatter.PrintMessageTitle(
                        "Verify User Without Any Restriction Access will be assigned to the non-restricted claims");
                    _qaManager.ClickOnSidebarIcon();
                    SearchByAnalystAndNavigateToAppealCategoryAssignment(_qaManager, analystList[1]);
                    _qaManager.GetSideWindow.SelectDropDownListValueByLabel("Category ID", categoryList[0], false);
                    _qaManager.ClickOnCategoryDetailsByLineNo(1);
                    _qaManager.IsRestrictionOptionsPresent()
                        .ShouldBeFalse("No Options Should Be Shown if a user does not have any restriction access");

                    StringFormatter.PrintMessage(
                        "Verify Clicking on cancel the message is closed and changes is not saved");
                    _qaManager.GetSideWindow.Save();
                    _qaManager.IsPageErrorPopupModalPresent().ShouldBeTrue("Popup error message should be present");
                    _qaManager.GetPageErrorMessage()
                        .ShouldBeEqual("The analyst will be added to this category. Do you wish to continue?");
                    _qaManager.IsOkButtonPresent().ShouldBeTrue("Ok button should be present");
                    _qaManager.IsCancelLinkPresent().ShouldBeTrue("Cancel Link should be present");
                    _qaManager.ClickOkCancelOnConfirmationModal(false);
                    _qaManager.IsPageErrorPopupModalPresent().ShouldBeFalse("Message should be closed");
                    _qaManager.IsCategoryPresentInRespectiveClaimsAssignment(categoryList[0], assignmentCategory[1])
                        .ShouldBeFalse("Category Should Not Be Added when cancel is clicked");

                    _qaManager.GetSideWindow.Save();
                    _qaManager.ClickOkCancelOnConfirmationModal(true);
                    _qaManager.WaitForWorking();
                    _qaManager.IsCategoryPresentInRespectiveClaimsAssignment(categoryList[0], assignmentCategory[1])
                        .ShouldBeTrue("Category Should Be Added In Non-Restricted Claims");

                    StringFormatter.PrintMessage(
                        "Verify user can enter a new value in the Category ID search to add the next category");
                    _qaManager.GetSideWindow.SelectDropDownListValueByLabel("Category ID", categoryList[1], false);
                    _qaManager.ClickOnCategoryDetailsByLineNo(1);
                    _qaManager.GetSideWindow.Save();
                    _qaManager.ClickOkCancelOnConfirmationModal(true);
                    _qaManager.WaitForWorking();
                    _qaManager.GetCountOfClaimsAssignmentsRecordRowsByLabel(assignmentCategory[1])
                        .ShouldBeEqual(2, "Verify more than one category can be added one after another");

                    _qaManager.ClickOnDeleteIconByOptionandLineNo(assignmentCategory[1], 1);
                    _qaManager.IsPageErrorPopupModalPresent().ShouldBeTrue("Popup error message should be present");
                    _qaManager.GetPageErrorMessage()
                        .ShouldBeEqual("The analyst will be removed from this category. Do you wish to continue?");
                    _qaManager.IsOkButtonPresent().ShouldBeTrue("Ok button should be present");
                    _qaManager.IsCancelLinkPresent().ShouldBeTrue("Cancel Link should be present");
                    _qaManager.ClickOkCancelOnConfirmationModal(false);
                    _qaManager.IsPageErrorPopupModalPresent().ShouldBeFalse("Message should be closed");
                    _qaManager.IsCategoryPresentInRespectiveClaimsAssignment(categoryList[0], assignmentCategory[1])
                        .ShouldBeTrue("Category Should Not Be Deleted when cancel is clicked in popup");
                    for (int i = 0; i < 2; i++)
                    {
                        _qaManager.ClickOnDeleteIconByOptionandLineNo(assignmentCategory[1], 1);
                        _qaManager.ClickOkCancelOnConfirmationModal(true);
                        _qaManager.IsCategoryPresentInRespectiveClaimsAssignment(categoryList[i], assignmentCategory[1])
                            .ShouldBeFalse("Category Should Be Deleted");
                    }

                    #endregion

                }
                finally
                {
                    if (automatedBase.CurrentPage.GetPageHeader() != PageHeaderEnum.QAManager.GetStringValue())
                    {
                        _qaManager.SwitchTab(automatedBase.CurrentPage.CurrentWindowHandle);
                        _qaManager.CloseAnyPopupIfExist();
                    }

                    if (_qaManager.GetCountOfClaimsAssignmentsRecordRowsByLabel(assignmentCategory[0]) > 0)
                    {
                        _qaManager.ClickOnDeleteIconByOptionandLineNo(assignmentCategory[0], 1);
                        _qaManager.ClickOkCancelOnConfirmationModal(true);
                    }

                    if (_qaManager.GetCountOfClaimsAssignmentsRecordRowsByLabel(assignmentCategory[1]) > 0)
                    {
                        _qaManager.ClickOnDeleteIconByOptionandLineNo(assignmentCategory[1], 1);
                        _qaManager.ClickOkCancelOnConfirmationModal(true);
                    }

                    _qaManager.DeleteAppealCategoryAuditFromDatabase(categoryList[0], categoryList[1], categoryList[2],
                        userIdList[0]);
                }
            }
        }

        [Test] //US66372
        public void Verify_navigation_and_security()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                QaManagerPage _qaManager;
                CommonValidations _commonValidations;
                automatedBase.CurrentPage =
                    _qaManager = automatedBase.QuickLaunch.NavigateToAnalystManager();
                _commonValidations = new CommonValidations(automatedBase.CurrentPage);

                _qaManager.IsFilterPanelPresent().ShouldBeTrue("Filter Panel present.");
                _qaManager.IsSideBarIconPresent().ShouldBeTrue("Search Analyst Icon present.");

                _commonValidations.ValidateSecurityAndNavigationOfAPage(HeaderMenu.Manager,
                    new List<string> { SubMenu.QaManager },
                    RoleEnum.OperationsManager.GetStringValue(),
                    new List<string> { PageHeaderEnum.QAManager.GetStringValue() }, automatedBase.Login.LoginAsUserHavingNoAnyAuthority,
                    new[] { "Test4", "Automation4" });

                _qaManager.Logout().LoginAsClientUser();
                _qaManager.IsQaManagerSubMenuPresent().ShouldBeFalse("QA Manager submenu absent for client user.");
            }
        }

        [Test] //US66373 + CAR-2784(CAR-2972) + CAR-2868(CAR-2975)
        public void Verify_display_result_and_filter_search_result()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                QaManagerPage _qaManager;

                automatedBase.CurrentPage =
                    _qaManager = automatedBase.QuickLaunch.NavigateToAnalystManager();
                _qaManager.RefreshPage(); //to verify default value 
                _qaManager.GetSideBarPanelSearch.SelectDropDownListValueByLabel("QA", "Active");
                _qaManager.ClickOnFindButton();

                var actualQAManagerList = _qaManager.GetGridListValueByCol(col: 1);
                var expectedActiveQAManagerList = _qaManager.GetActiveQAManagerListFromDB();

                actualQAManagerList.ShouldCollectionBeEquivalent(expectedActiveQAManagerList,
                    "Default Collection should equal to DataBase value");

                var dateList = _qaManager.GetGridListValueByCol(col: 2);
                var appealQa = _qaManager.GetGridListValueByCol(col: 3);
                var currentDate = DateTime.Now;
                for (var i = 0; i < appealQa.Count; i++)
                {
                    if (appealQa[i] == "No Appeal QA")
                        (Convert.ToDateTime(dateList[i].Split('-')[1]) >= currentDate).ShouldBeTrue(
                            "End Date should be less than or equal to current date for active list");
                }

                var startDate = dateList.Select(x => x.Split('-')[0]).Where(x => !string.IsNullOrEmpty(x)).ToList();
                var endDate = dateList.Select(x => x.Split('-')[0]).Where(x => !string.IsNullOrEmpty(x)).ToList();

                _qaManager.GetGridValueByRowCol(1, 1).DoesNameContainsOnlyFirstWithLastname()
                    .ShouldBeTrue("Is Analyst Name displayed only as <First name> <Last name>?");
                startDate[0].Trim().IsDateInFormat().ShouldBeTrue("Begin Date should show as MM/DD/YYYY");
                endDate[0].Trim().IsDateInFormat().ShouldBeTrue("End Date should show as MM/DD/YYYY");
                _qaManager.GetGridLabelByRowCol(1, 3).ShouldBeEqual("Appeal QA:", "Verification of Appeal QA Label");

                StringFormatter.PrintMessageTitle("Verification of Inactive List for whose begin QA date is empty");
                _qaManager.SelectAnalayst("ui automation_4");
                _qaManager.SelectQaOption("All");
                _qaManager.ClickOnFindButton();
                _qaManager.GetGridValueByRowCol(1, 1).ShouldBeEqual("ui automation_4", "Analyst should be equal");
                _qaManager.GetGridListCount()
                    .ShouldBeEqual(1, "Only One record should display when searched by analyst");

                StringFormatter.PrintMessage(
                    "Verifying whether clicking on the row opens Analyst QA section by default");
                _qaManager.GetGridViewSection.ClickOnGridRowByRow();
                _qaManager.GetSelectedIconName()
                    .ShouldBeEqual("Modify QA settings", "'Modify QA settings icon should be selected'");
                _qaManager.IsSectionWithHeaderNamePresent("Analyst QA")
                    .ShouldBeTrue("Analyst QA section must be shown by default when data row is clicked");

                StringFormatter.PrintMessage(
                    "Verifying whether clicking on 'Modify QA Settings' icon displays 'Analyst QA' section");
                _qaManager.ClickAnalystManagerIconByColNum(1);
                _qaManager.ClickAnalystManagerIconByColNum(3);
                _qaManager.IsSectionWithHeaderNamePresent("Analyst QA")
                    .ShouldBeTrue("Clicking on 'Modify QA Settings' icon should display 'Analyst QA' section");
                _qaManager.IsSectionWithHeaderNamePresent("QA Settings")
                    .ShouldBeTrue("'QA Settings' form should be present");
                _qaManager.IsSectionWithHeaderNamePresent("QA Results")
                    .ShouldBeTrue("'QA Results' form should be present");

                if (_qaManager.GetSideBarPanelSearch.IsSideBarPanelOpen())
                    _qaManager.GetSideBarPanelSearch.ClickOnToggleSidebarPanelButton();
                _qaManager.ClearBeginDate();
                _qaManager.ClearEndDate();
                if (_qaManager.IsDeleteClientInEditSectionPresent())
                    _qaManager.DeleteClientInEditSection();
                //_qaManager.ClickOnQaRadioButton(1);
                _qaManager.SelectAppealQaOptionFromDropdown("No Appeal QA");
                _qaManager.ClickOnSaveButton();
                _qaManager.GetGridValueByRowCol(1, 2).ShouldBeNullorEmpty("Date should be Empty");

                StringFormatter.PrintMessageTitle(
                    "Verification of Inactive List for those whose End QA date has passed");
                _qaManager.GetGridViewSection.ClickOnGridRowByRow();
                _qaManager.SetBeginDate(DateTime.Now.Date.AddDays(-5).ToString("MM/d/yyyy"));
                _qaManager.SetEndDate(DateTime.Now.Date.AddDays(-1).ToString("MM/d/yyyy"));
                _qaManager.SetClient(ClientEnum.SMTST.ToString());
                _qaManager.SetClaimsCount("1", 1);
                _qaManager.SetMaxClaimsCount("5", 1);
                _qaManager.ClickOnSaveButton();
                _qaManager.GetGridValueByRowCol(1, 2).Split('-')[1].Trim().ShouldBeEqual(
                    DateTime.Now.Date.AddDays(-1).ToString("MM/dd/yyyy"), "End Date should Equal be as passed date");


                if (!_qaManager.GetSideBarPanelSearch.IsSideBarPanelOpen())
                    _qaManager.GetSideBarPanelSearch.ClickOnToggleSidebarPanelButton();
                _qaManager.ClickOnClearButton();
                _qaManager.SelectQaOption("Active");
                _qaManager.ClickOnFindButton();
                _qaManager.WaitForWorking();
                actualQAManagerList = _qaManager.GetGridListValueByCol(col: 1);
                expectedActiveQAManagerList = _qaManager.GetActiveQAManagerListFromDB();
                actualQAManagerList.ShouldCollectionBeEquivalent(expectedActiveQAManagerList,
                    "Active List Collection Should be Same");
                _qaManager.SelectAnalayst("All");
                _qaManager.SelectQaOption("All");
                _qaManager.ClickOnFindButton();
                _qaManager.GetGridListValueByCol(col: 1).ShouldCollectionBeEqual(_qaManager.GetActiveInactiveList(),
                    "Verification of All active and inactive QA user records to the database records");

                _qaManager.GetSideBarPanelSearch.SelectDropDownListValueByLabel("QA", "Not active");
                _qaManager.ClickOnFindButton();

                var expectedInactiveQAManagerList = _qaManager.GetInactiveQAManagerListFromDB();
                _qaManager.GetGridListValueByCol(col: 1).ShouldCollectionBeEquivalent(expectedInactiveQAManagerList,
                    "Inactive users search result should match with the DB");
            }
        }

        [Test] //US66374
        public void Verification_and_validation_of_QA_target_parameters_form()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                QaManagerPage _qaManager;

                automatedBase.CurrentPage =
                    _qaManager = automatedBase.QuickLaunch.NavigateToAnalystManager();
                _qaManager.NavigateToAnalystManager();
                _qaManager.SelectAnalayst("ui automation_1");
                _qaManager.ClickOnFindButton();
                var date = _qaManager.GetGridValueByRowCol(1, 2).Split('-');
                var defaultBeginDate = date[0].Trim();
                var defaultEndDate = date[1].Trim();
                var defaultPerc = _qaManager.GetGridValueByRowCol(1, 3);
                //var defaultMax = _qaManager.GetGridValueByRowCol(1, 5);
                var beginDate = DateTime.Now.ToString("MM/d/yyyy");
                var endDate = DateTime.Now.AddDays(10).ToString("MM/d/yyyy");
                try
                {
                    //_qaManager.ClickOnGridRowToOpenSideWindow();
                    _qaManager.GetGridViewSection.ClickOnGridRowByRow();
                    _qaManager.GetDateRangeInputFieldsCount("Claim QA Settings")
                        .ShouldBeEqual(2, "QA date range fields are present has to be true.");
                    _qaManager.SetEndDate(endDate);
                    var actualEndDate = _qaManager.GetEndDate();
                    actualEndDate.ShouldBeEqual(Convert.ToDateTime(endDate).ToString("MM/dd/yyyy"),
                        "End date selection with use of date picker is allowed");
                    actualEndDate.IsDateInFormat().ShouldBeTrue("Due Date is in mm/dd/yyyy format");
                    _qaManager.SetBeginDate(beginDate);
                    var actualBeginDate = _qaManager.GetBeginDate();
                    actualBeginDate.ShouldBeEqual(Convert.ToDateTime(beginDate).ToString("MM/dd/yyyy"),
                        "Begin date selection with use of date picker is allowed");
                    actualBeginDate.IsDateInFormat().ShouldBeTrue("Begin Date is in mm/dd/yyyy format");
                    StringFormatter.PrintMessageTitle("Validation of Input Fileds");
                    _qaManager.SetEndDateWithoutUsingCalender(DateTime.Now.AddDays(-1).ToString("MM/dd/yyyy"));
                    ValidateForPopUpMessage(_qaManager, "Popup Displayed when end date selected occurs before begin date.",
                        "Please enter a valid date range.");
                    _qaManager.ClearBeginDate();
                    _qaManager.SetEndDateWithoutUsingCalender(DateTime.Now.AddDays(-1).ToString("MM/dd/yyyy"));
                    _qaManager.ClickOnSaveButton();
                    ValidateForPopUpMessage(_qaManager, "Popup Displayed when user selects end date only without a begin date.",
                        "Claim QA values are missing. All fields are required.");
                    _qaManager.ClearBeginDate();
                    _qaManager.ClearEndDate();
                    _qaManager.SetClaimsCount("ABCD");
                    ValidateForPopUpMessage(_qaManager, "Popup Displayed when user selects non numeric value in claim",
                        "Only numbers allowed.");
                    _qaManager.SetMaxClaimsCount("ABCD");
                    ValidateForPopUpMessage(_qaManager, "Popup Displayed when user selects non numeric value in max claim",
                        "Only numbers allowed.");
                    _qaManager.SetClaimsCount("111");
                    ValidateForPopUpMessage(_qaManager, "Popup Displayed when user enters value greater than 100 in max claim",
                        "Value cannot be greater than 100.");
                    _qaManager.SetClaimsCount("0");
                    ValidateForPopUpMessage(_qaManager, "Popup Displayed when user enters 0 value in max claim",
                        "Value cannot be less than 1.");
                    _qaManager.SetBeginDateWithoutUsingCalender(DateTime.Now.ToString("MM/d/yyyy"));
                    _qaManager.ClearClaimsCount();
                    _qaManager.ClickOnSaveButton();
                    ValidateForPopUpMessage(_qaManager,
                        "Popup Displayed when user selects begin date but doesnt input claim count.",
                        "Claim QA values are missing. All fields are required.");
                    var prevClaimPercVal = _qaManager.GetClaimPercentageValue();
                    _qaManager.SetClaimsCount("5");
                    _qaManager.GetClaimPercentageValue().ShouldNotBeEqual(prevClaimPercVal,
                        "Claim percentage value calculated automatically once claim count is set.");
                    _qaManager.ClearMaxClaimsCount();
                    _qaManager.ClickOnSaveButton();
                    ValidateForPopUpMessage(_qaManager,
                        "Popup Displayed when user selects begin date but doesnt input maximum claim count.",
                        "Claim QA values are missing. All fields are required.");
                    _qaManager.ClickOnCancelLink();
                    (_qaManager.GetGridValueByRowCol(1, 2).Split('-')[0]).Trim().ShouldBeEqual(defaultBeginDate,
                        "Previous value retained, and unsaved changes were discarded.");
                    _qaManager.GetGridValueByRowCol(1, 2).Split('-')[1].Trim().ShouldBeEqual(defaultEndDate,
                        "Previous value retained, and unsaved changes were discarded.");
                    _qaManager.GetGridValueByRowCol(1, 3)
                        .ShouldBeEqual(defaultPerc, "Previous value retained, and unsaved changes were discarded.");
                    //_qaManager.GetGridValueByRowCol(1, 5)
                    //    .ShouldBeEqual(defaultMax, "Previous value retained, and unsaved changes were discarded.");
                    //_qaManager.ClickOnGridRowToOpenSideWindow();
                    //_qaManager.ClearBeginDate();
                    //_qaManager.ClearEndDate();
                    //_qaManager.ClickOnSaveButton();
                    //_qaManager.IsPageErrorPopupModalPresent()
                    //    .ShouldBeTrue("Validation Popup should be displayed when saved without daterange.");
                    //_qaManager.ClosePageError();
                    //_qaManager.ClickOnCancelLink();
                    //_qaManager.GetGridValueByRowCol(1, 3).ShouldBeNullorEmpty("Date Range should be Empty");
                }

                finally
                {
                    if (!_qaManager.IsEditFormDisplayed())
                        _qaManager.GetGridViewSection.ClickOnGridRowByRow();

                    _qaManager.SetEndDateWithoutUsingCalender(defaultEndDate);
                    _qaManager.SetBeginDateWithoutUsingCalender(defaultBeginDate);
                    while (_qaManager.IsDeleteClientInEditSectionPresent())
                        _qaManager.DeleteClientInEditSection();
                    _qaManager.SetClient(ClientEnum.SMTST.ToString());
                    _qaManager.SetClaimsCount("1");
                    _qaManager.SetMaxClaimsCount("100");
                    _qaManager.ClickOnSaveButton();

                }

            }
        }

        [NonParallelizable]
        [Test, Category("CommonTableDependent")] //US66374
        public void Verification_and_validation_of_QA_target_record_and_claim_flagging_for_terminated_date()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(1))
            {
                QaManagerPage _qaManager;

                automatedBase.CurrentPage =
                    _qaManager = automatedBase.QuickLaunch.NavigateToAnalystManager();
                var TestName = new StackFrame(true).GetMethod().Name;
                var claimSequence = automatedBase.DataHelper.GetSingleTestData(GetType().FullName, TestName,
                    "ClaimSequence", "Value");
                var date = automatedBase.DataHelper.GetSingleTestData(GetType().FullName, TestName, "Date",
                    "Value");
                var analyst = automatedBase.DataHelper.GetSingleTestData(GetType().FullName, TestName, "Analyst",
                    "Value");

                _qaManager.SearchByAnalyst(analyst);
                _qaManager.GetGridViewSection.ClickOnGridRowByRow();

                var newBeginDate = DateTime.Now.AddDays(-1).ToString("MM/dd/yyyy");
                var newEndDate = DateTime.Now.AddDays(5).ToString("MM/dd/yyyy");

                _qaManager.SetEndDateWithoutUsingCalender(newEndDate);
                _qaManager.SetBeginDateWithoutUsingCalender(newBeginDate);

                if (_qaManager.IsPageErrorPopupModalPresent()) _qaManager.ClosePageError();
                if (_qaManager.GetEndDate() != newEndDate)
                    _qaManager.SetEndDateWithoutUsingCalender(newEndDate);

                _qaManager.SetClaimsCount("1");
                _qaManager.SetMaxClaimsCount("8");
                _qaManager.ClickOnSaveButton();

                _qaManager.GetGridValueByRowCol(1, 2).Split('-')[0].Trim()
                    .ShouldBeEqual(newBeginDate, "Changes has been  applied.");

                StringFormatter.PrintMessageTitle("Verification of Flagging of claims when QA date hasnt passed by uiautomation 1 user");
                var claimSearch = _qaManager.NavigateToClaimSearch();
                _qaManager.DeleteQaAuditRecordOfClaimApproveAndChangeClaimStatus(claimSequence, date);
                _qaManager.ResetUserQaCounterToDefault(automatedBase.EnvironmentManager.HciAdminUsername1);
                var claimAction = claimSearch.SearchByClaimSequenceToNavigateToClaimActionPage(claimSequence);
                claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                claimAction.ClickOnApproveButton();
                claimAction.WaitForWorkingAjaxMessage();

                StringFormatter.PrintMessageTitle("Verification of Flagging of claims when QA date hasnt passed by uiautomation 1 user by checking if approved claims show in qa claim search");
                var qaClaimSearch = claimAction.NavigateToQaClaimSearch();
                qaClaimSearch.SelectOutstandingQaClaims();
                qaClaimSearch.SearchByAnalyst(analyst);
                qaClaimSearch.IsClaimSequencePresentInGridPanel(claimSequence)
                    .ShouldBeTrue("Record of QA flagging of claims should be present  when QA date hasn't passed");
                _qaManager.DeleteQaAuditRecordOfClaimApproveAndChangeClaimStatus(claimSequence, date);
                _qaManager.ResetUserQaCounterToDefault(automatedBase.EnvironmentManager.HciAdminUsername1);
                _qaManager = qaClaimSearch.NavigateToAnalystManager();
                _qaManager.RefreshPage();
                _qaManager.SearchByAnalyst(analyst);
                
                _qaManager.GetGridViewSection.ClickOnGridRowByRow();
                var outdatedBeginDate = DateTime.Now.AddDays(-2).ToString("MM/dd/yyyy");
                var outdatedEndDate = DateTime.Now.AddDays(-1).ToString("MM/dd/yyyy");
                
                _qaManager.SetEndDateWithoutUsingCalender(outdatedEndDate);
                _qaManager.SetBeginDateWithoutUsingCalender(outdatedBeginDate);

                if (_qaManager.IsPageErrorPopupModalPresent()) _qaManager.ClosePageError();
                if (_qaManager.GetEndDate() != outdatedEndDate)
                    _qaManager.SetEndDateWithoutUsingCalender(outdatedEndDate);

                _qaManager.SetClaimsCount("1");
                _qaManager.SetMaxClaimsCount("8");
                _qaManager.ClickOnSaveButton();
                _qaManager.GetGridViewSection.ClickOnGridRowByRow();

                claimSearch = _qaManager.NavigateToClaimSearch();
                claimAction = claimSearch.SearchByClaimSequenceToNavigateToClaimActionPage(claimSequence);
                claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                claimAction.ClickOnApproveButton();
                claimAction.WaitForWorkingAjaxMessage();

                StringFormatter.PrintMessageTitle("Verifying Flagging of claims ends when QA date has passed; checking approved claims doesn't show in qa claim search");
                qaClaimSearch = claimAction.NavigateToQaClaimSearch();
                qaClaimSearch.SelectOutstandingQaClaims();
                qaClaimSearch.SearchByAnalyst(analyst);
                qaClaimSearch.IsClaimSequencePresentInGridPanel(claimSequence)
                    .ShouldBeFalse("Record of QA flagging of claims should not be present  when QA date has passed");
                
                _qaManager = qaClaimSearch.NavigateToAnalystManager();
                _qaManager.RefreshPage();
                StringFormatter.PrintMessageTitle("Reset Data in QA manager");
                _qaManager.SearchByAnalyst(analyst);
                
                _qaManager.GetGridViewSection.ClickOnGridRowByRow();
                _qaManager.SetEndDateWithoutUsingCalender(DateTime.Now.AddDays(5).ToString("MM/dd/yyyy"));
                _qaManager.SetBeginDateWithoutUsingCalender(newBeginDate);
                if (_qaManager.IsPageErrorPopupModalPresent()) _qaManager.ClosePageError();
                if (_qaManager.GetEndDate() != DateTime.Now.AddDays(5).ToString("MM/dd/yyyy"))
                    _qaManager.SetEndDateWithoutUsingCalender(DateTime.Now.AddDays(5).ToString("MM/dd/yyyy"));
                _qaManager.SetClaimsCount("1");
                _qaManager.SetMaxClaimsCount("8");
                _qaManager.ClickOnSaveButton();
                
            }
        }

        [Test] //US66375 + CAR-2868 (CAR-2975)
        public void Validation_of_find_analyst_panel_in_sidebar()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                QaManagerPage _qaManager;

                automatedBase.CurrentPage =
                    _qaManager = automatedBase.QuickLaunch.NavigateToAnalystManager();
                var _analystUserList = _qaManager.GetAssignedToList();
                _qaManager.RefreshPage();
                _qaManager.GetValuesOfSideBarLabel()[0].ShouldBeEqual("Analyst", "Label value equals analyst");
                _qaManager.GetAnalystInputField().ShouldBeEqual("All", "Default selection of analyst is All");

                _qaManager.GetValuesOfSideBarLabel()[1].ShouldBeEqual("QA", "Label value equals QA");
                _qaManager.GetSideBarPanelSearch.GetInputValueByLabel("QA")
                    .ShouldBeEqual("All", "Default selection of QA is All");
                //_qaManager.GetAnalystInputField().ShouldBeEqual("All", "Default selection of QA is All");

                var reqDropDownList = _qaManager.GetSideBarPanelSearch.GetAvailableDropDownList("Analyst");
                reqDropDownList.Count.ShouldBeGreater(0, "List of Analyst is greater than zero.");
                reqDropDownList.Contains("All")
                    .ShouldBeTrue(
                        "A value of all displayed at the top of the list, followed by options sorted alphabetically.");
                //reqDropDownList.Contains("All Active").ShouldBeTrue("A value of all displayed at the top of the list, followed by options sorted alphabetically.");
                reqDropDownList.Remove("All");
                //reqDropDownList.Remove("All Active");
                _analystUserList.Sort();
                reqDropDownList.ShouldCollectionBeEqual(_analystUserList,
                    "Validate Assigned To List with the database");
                reqDropDownList.IsInAscendingOrder().ShouldBeTrue("Analyst should be sorted in alphabetical order.");
                reqDropDownList[0].DoesNameContainsOnlyFirstWithLastname()
                    .ShouldBeTrue("Analyst should be in proper format of <firstname> <lastname>");

                var qaDropDownList = _qaManager.GetSideBarPanelSearch.GetAvailableDropDownList("QA");
                qaDropDownList.ShouldCollectionBeEqual(new[] { "All", "Active", "Not active" },
                    "QA dropdown should be correct");
            }
        }

        [Test] //US66662
        public void Validation_of_filter_in_QA_target_results_history()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                QaManagerPage _qaManager;

                automatedBase.CurrentPage =
                    _qaManager = automatedBase.QuickLaunch.NavigateToAnalystManager();
                var TestName = new StackFrame(true).GetMethod().Name;
                var beginDate = automatedBase.DataHelper.GetSingleTestData(GetType().FullName, TestName,
                    "BeginDate", "Value");
                var endDate = automatedBase.DataHelper.GetSingleTestData(GetType().FullName, TestName, "EndDate",
                    "Value");
                var dateForNoResult = automatedBase.DataHelper.GetSingleTestData(GetType().FullName, TestName,
                    "DateForNoResult", "Value");
                var analyst = automatedBase.DataHelper.GetSingleTestData(GetType().FullName, TestName, "Analyst",
                    "Value");
                var qaResultHistoryCount = _qaManager.GetQaTargetResultHistoryList(automatedBase.EnvironmentManager.HciAdminUsername);

                _qaManager.SearchByAnalyst(analyst);
                _qaManager.ClickOnSearchListRow(analyst);
                _qaManager.ClickOnSidebarIcon(true);

                _qaManager.IsQaResultsSectionOpen().ShouldBeTrue("QA Results section should open up");
                _qaManager.IsNoDataMessagePresent()
                    .ShouldBeFalse("There should be history and no data message is present should be false.");
                var qaHistoryCount = _qaManager.GetQaResultsHistoryCount();
                var qaResultListWithDate = _qaManager.GetQaResultListbyCol(1);
                qaResultHistoryCount.ShouldCollectionBeEquivalent(qaResultListWithDate,
                    "Verify default list of QA Target Results History");

                _qaManager.ScrollToFindQAResult();
                _qaManager.GetDateRangeInputFieldsCount("QA Results")
                    .ShouldBeEqual(2, "Verification of count of beginning date and end date");
                _qaManager.SetStartOrEndDateByDateHeaderName(beginDate, "Find QA Result");
                _qaManager.GetBeginOrEndDateByHeaderName("Find QA Result", true).ShouldBeEqual(beginDate,
                    "Is End Date  automatically popuplated when begin date selected?");
                _qaManager.SetStartOrEndDateByDateHeaderName(endDate, "Find QA Result", false);
                _qaManager.ClickOnFindButton();
                _qaManager.GetQaResultsHistoryCount().ShouldBeLess(qaHistoryCount,
                    "Filter applied searched result count should less than default result count");
                _qaManager.SetDateWithoutUsingCalender("Find QA Result", dateForNoResult, true);
                _qaManager.ClickOnFindButton();
                _qaManager.IsNoDataMessagePresent().ShouldBeTrue("Is No Data Found Div Present?");
                _qaManager.GetNoDataMessageText()
                    .ShouldBeEqual("No Data Available", "Verify Message for <No Data Available> ");
                _qaManager.ClickOnCancelQaResultLink();
                qaResultHistoryCount.ShouldCollectionBeEquivalent(qaResultListWithDate,
                    "Verify default list of QA Target Results History when click on Cancel Link");
                _qaManager.IsNoDataMessagePresent()
                    .ShouldBeFalse("Is <No Data found> Div present when click on Cancel Link");
            }
        }

        [Test] //US69437+US69438 + CAR-3088 (CAR-3149)
        public void Validation_of_appeal_QA_parameters()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                QaManagerPage _qaManager;
                automatedBase.CurrentPage =
                    _qaManager = automatedBase.QuickLaunch.NavigateToAnalystManager();
                var TestName = new StackFrame(true).GetMethod().Name;
                var analyst = automatedBase.DataHelper.GetSingleTestData(GetType().FullName, TestName, "Analyst",
                    "Value");
                var beginDate = automatedBase.DataHelper.GetSingleTestData(GetType().FullName, TestName,
                    "BeginDate", "Value");
                var endDate = automatedBase.DataHelper.GetSingleTestData(GetType().FullName, TestName, "EndDate",
                    "Value");
                var claims = automatedBase.DataHelper.GetSingleTestData(GetType().FullName, TestName, "Claims",
                    "Value");
                var totalClaims = automatedBase.DataHelper.GetSingleTestData(GetType().FullName, TestName,
                    "TotalClaims", "Value");
                var qagroup = automatedBase.DataHelper
                    .GetSingleTestData(GetType().FullName, TestName, "QaGroup", "Value").Split(',')
                    .ToList();
                var appeals = automatedBase.DataHelper.GetSingleTestData(GetType().FullName, TestName, "Appeals",
                    "Value");
                var weeklyAppealMax = automatedBase.DataHelper.GetSingleTestData(GetType().FullName, TestName,
                    "WeeklyAppealMax", "Value");
                var reviewappealpercentage = Convert.ToInt32((1 / Convert.ToDecimal(appeals)) * 100);
                var userId = automatedBase.DataHelper.GetSingleTestData(GetType().FullName, TestName, "UserId",
                    "Value");


                _qaManager.SearchByAnalyst(analyst);
                foreach (var qa in qagroup)
                {
                    _qaManager.GetGridViewSection.ClickOnGridRowByRow();
                    //_qaManager.ClickOnGridRowToOpenSideWindow();
                    _qaManager.SetBeginDate(beginDate);
                    _qaManager.SetEndDate(endDate);
                    _qaManager.SetClaimQaDetails(1, claims);
                    _qaManager.SetClaimQaDetails(3, totalClaims);
                    _qaManager.IsAppealQaSettingsHeaderPresent().ShouldBeTrue("Appeal QA Details section should be present.");
                    switch (qa)
                    {
                        case "TQa":
                            /*_qaManager.GetToolTipForAppealQaDetailsRadioButtonLabel("Trainee QA").ShouldBeEqual(
                                "Identify appeals for QA as they are completed by the user regardless of categories.",
                                "Trainee QA label should have tool tip Identify appeals for QA as they are completed by the user regardless of categories.");
                            _qaManager.ClickOnQaRadioButton(2);
                            _qaManager.IsRadioButtonSelected(2).ShouldBeTrue("Trainee QA should be selected.");
                            _qaManager.IsRadioButtonSelected(1).ShouldBeFalse("No Appeal QA should not be selected.");
                            _qaManager.IsRadioButtonSelected(3).ShouldBeFalse("Ongoing QA should not be selected.");*/

                            _qaManager.SelectAppealQaOptionFromDropdown("Trainee QA");
                            _qaManager.GetSelectedQaAppealOption().ShouldBeEqual("Trainee QA",
                                "Trainee QA option should be selected");
                            VerifyClientAndCategoryFields();
                            _qaManager.SetAppealQaDetails(1, "134");
                            ValidateForPopUpMessage(_qaManager,
                                "If value greater than 100 is entered in 1 of every appeals completed",
                                "Value cannot be greater than 100.");
                            _qaManager.SetAppealQaDetails(1, appeals);
                            _qaManager.GetTextValueInAppealsCompleted().ShouldBeEqual(reviewappealpercentage + "%",
                                "1 of every given appeals percentage is calculated automatically.");
                            _qaManager.SetAppealQaDetails(2, "");
                            _qaManager.ClickOnSaveButton();
                            ValidateForPopUpMessage(_qaManager,
                                "If Save is selected and any of the required Validation Notice will display",
                                "Appeal QA values are missing. All fields are required.");
                            _qaManager.ClickOnCancelLink();
                            break;

                        case "OQa":
                            /*_qaManager.GetToolTipForAppealQaDetailsRadioButtonLabel("Ongoing QA").ShouldBeEqual(
                                "Identify appeals for QA that will be added weekly across all categories completed by that user",
                                "Trainee QA label should have tool tip Identify appeals for QA that will be added weekly across all categories completed by that user.");
                            _qaManager.ClickOnQaRadioButton(3);
                            _qaManager.IsRadioButtonSelected(3).ShouldBeTrue("Ongoing QA should be selected.");
                            _qaManager.IsRadioButtonSelected(1).ShouldBeFalse("No Appeal QA should not be selected.");
                            _qaManager.IsRadioButtonSelected(2).ShouldBeFalse("Trainee QA should not be selected.");*/

                            _qaManager.SelectAppealQaOptionFromDropdown("Ongoing QA");
                            _qaManager.GetSelectedQaAppealOption().ShouldBeEqual("Ongoing QA",
                                "Ongoing QA option should be selected");
                            VerifyClientAndCategoryFields();
                            _qaManager.SetAppealQaDetails(1, "253");
                            ValidateForPopUpMessage(_qaManager, "If value greater than 100 is entered in Weekly Appeal Max",
                                "Value cannot be greater than 100.");
                            _qaManager.SetAppealQaDetails(1, weeklyAppealMax);
                            _qaManager.ClickOnSaveButton();
                            break;

                        default:
                            /*_qaManager.GetToolTipForAppealQaDetailsRadioButtonLabel("No Appeal QA").ShouldBeEqual(
                                "No Appeal QA",
                                "Trainee QA label should have tool tip No Appeal QA.");
                            _qaManager.ClickOnQaRadioButton(1);
                            _qaManager.IsRadioButtonSelected(1).ShouldBeTrue("No Appeal QA should be selected.");
                            _qaManager.IsRadioButtonSelected(3).ShouldBeFalse("Ongoing QA should not be selected.");
                            _qaManager.IsRadioButtonSelected(2).ShouldBeFalse("Trainee QA should not be selected.");*/

                            _qaManager.SelectAppealQaOptionFromDropdown("No Appeal QA");
                            _qaManager.GetSelectedQaAppealOption().ShouldBeEqual("No Appeal QA",
                                "No Appeal QA option should be selected");
                            _qaManager.ClickOnCancelLink();
                            break;
                    }
                }

                _qaManager.GetGridValueByRowCol(1, 3)
                    .ShouldBeEqual("Ongoing QA", "Appeal QA value must be updated to Ongoing QA.");

                void VerifyClientAndCategoryFields()
                {
                    StringFormatter.PrintMessage("Verifying the Available Clients transfer component");
                    ClickCheckboxAndVerifyTransferComponent("Clients");

                    StringFormatter.PrintMessage("Verifying the Available Categories transfer component");
                    ClickCheckboxAndVerifyTransferComponent("Categories");

                    StringFormatter.PrintMessage("Unchecking the Specify Clients and Specify Categories checkboxes");
                    _qaManager.GetSideWindow.CheckOrUncheckByLabel("Specify Clients", false);
                    _qaManager.GetSideWindow.CheckOrUncheckByLabel("Specify Categories", false);

                    _qaManager.IsAvailableAssignedClientsComponentPresentByLabel("Available Clients")
                        .ShouldBeFalse("Available Clients transfer component should be hidden once Specify Clients is unchecked");
                    _qaManager.IsAvailableAssignedClientsComponentPresentByLabel("Available Categories")
                        .ShouldBeFalse("Available Categories transfer component should be hidden once Specify Clients is unchecked");
                }

                void ClickCheckboxAndVerifyTransferComponent(string transferComponentLabel)
                {
                    var clientListFromDB = _qaManager.GetCommonSql.GetAssignedClientListForUser(userId);
                    var categoriesFromDB = _qaManager.GetCommonSql.GetAllAssignedCategoriesByUser(userId);

                    StringFormatter.PrintMessage($"Clicking Specify {transferComponentLabel} checkbox");
                    _qaManager.GetSideWindow.CheckOrUncheckByLabel($"Specify {transferComponentLabel}");

                    _qaManager.GetSideWindow.IsCheckboxCheckedByLabel($"Specify {transferComponentLabel}")
                        .ShouldBeTrue($"Specify {transferComponentLabel} checkbox should be checked");
                    _qaManager.IsAvailableAssignedClientsComponentPresentByLabel($"Available {transferComponentLabel}")
                        .ShouldBeTrue($"Available {transferComponentLabel} transfer component should be present when 'Specify {transferComponentLabel}' is checked");

                    //Moving all the Selected Clients to Available Clients column for testing
                    if (_qaManager.GenericGetAvailableAssignedList($"Selected {transferComponentLabel}").Count > 0)
                        _qaManager.GenericTransferAll($"Selected {transferComponentLabel}");

                    StringFormatter.PrintMessage($"Assigning all {transferComponentLabel}");
                    var availableList = _qaManager.GenericGetAvailableAssignedList($"Available {transferComponentLabel}");
                    _qaManager.GenericTransferAll($"Available {transferComponentLabel}");
                    _qaManager.GenericGetAvailableAssignedList($"Available {transferComponentLabel}")
                        .ShouldCollectionBeEmpty($"Available {transferComponentLabel} side should be empty when all {transferComponentLabel} are selected");

                    var assignedList = _qaManager.GenericGetAvailableAssignedList($"Selected {transferComponentLabel}");
                    assignedList
                        .ShouldCollectionBeEqual(availableList, $"All of the {transferComponentLabel} should be present in assigned clients side");

                    StringFormatter.PrintMessage($"Deselecting all {transferComponentLabel}");
                    _qaManager.GenericTransferAll($"Selected {transferComponentLabel}");
                    _qaManager.GenericGetAvailableAssignedList($"Selected {transferComponentLabel}")
                        .ShouldCollectionBeEmpty($"{transferComponentLabel} side should be empty when all the {transferComponentLabel} are deselected");

                    _qaManager.GenericGetAvailableAssignedList($"Available {transferComponentLabel}")
                        .ShouldCollectionBeEqual(availableList, $"All the deselected {transferComponentLabel} should appear in Available {transferComponentLabel} side");

                    StringFormatter.PrintMessage($"Verifying the {transferComponentLabel} options against the Database");
                    availableList.ShouldCollectionBeEqual(transferComponentLabel == "Clients" ? clientListFromDB : categoriesFromDB,
                        $"All the {transferComponentLabel} assigned to the analyst is correctly displayed ");
                }
            }
        }

        [Test] //CAR-2133:CAR-2348
        public void Verify_QA_claim_volumes_by_client_per_analyst()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                QaManagerPage _qaManager;

                automatedBase.CurrentPage =
                    _qaManager = automatedBase.QuickLaunch.NavigateToAnalystManager();
                var TestName = new StackFrame(true).GetMethod().Name;
                var analyst = automatedBase.DataHelper.GetSingleTestData(GetType().FullName, TestName, "Analyst",
                    "Value");
                var clientList = automatedBase.DataHelper
                    .GetSingleTestData(GetType().FullName, TestName, "Client", "Value").Split(';')
                    .ToList();
                var labelList = automatedBase.DataHelper
                    .GetSingleTestData(GetType().FullName, TestName, "Label", "Value").Split(';')
                    .ToList();
                var claimsCount = automatedBase.DataHelper
                    .GetSingleTestData(GetType().FullName, TestName, "ClaimsCount", "Value")
                    .Split(';').ToList();
                var maxClaims = automatedBase.DataHelper
                    .GetSingleTestData(GetType().FullName, TestName, "MaxClaims", "Value")
                    .Split(';').ToList();
                _qaManager.SearchByAnalyst(analyst);
                //_qaManager.ClickOnGridRowToOpenSideWindow();
                _qaManager.GetGridViewSection.ClickOnGridRowByRow();
                _qaManager.GetGridColumnLabelList()
                    .ShouldBeEqual(labelList, "Grid columns with following labels should be shown.");
                while (_qaManager.IsDeleteClientInEditSectionPresent())
                {
                    _qaManager.DeleteClientInEditSection();
                }

                _qaManager.SetClient(clientList[1]);
                _qaManager.SetClient(clientList[0]);
                _qaManager.IsPageErrorPopupModalPresent()
                    .ShouldBeTrue("Client-specific rows must be removed before an 'ALL' client row can be added.");
                _qaManager.ClosePageError();
                _qaManager.SetClaimsCount("2", 1);
                _qaManager.SetMaxClaimsCount("50", 1);
                _qaManager.GetClient().ShouldBeEqual(clientList[1], "Client label should contain client code.");
                var percentOfClaims = Convert.ToInt32(1.0 / Convert.ToInt32(_qaManager.GetClaimsCount()) * 100);
                _qaManager.GetPercentOfClaims().ShouldBeEqual(percentOfClaims.ToString(),
                    "Percent of Claims should contain value for 1 claim reviewed for every claims counts.");
                //_qaManager.SetClient(clientList[0]);
                _qaManager.SetClaimsCount("111", 1);
                _qaManager.IsPageErrorPopupModalPresent().ShouldBeTrue("Value cannot be greater than 100.");
                _qaManager.ClosePageError();
                _qaManager.SetClaimsCount("0", 1);
                _qaManager.IsPageErrorPopupModalPresent().ShouldBeTrue("Value cannot be less than 1.");
                _qaManager.ClosePageError();
                _qaManager.SetMaxClaimsCount("0", 1);
                _qaManager.IsPageErrorPopupModalPresent().ShouldBeTrue("Value cannot be less than 1.");
                _qaManager.ClosePageError();
                _qaManager.SetMaxClaimsCount("1000", 1);
                _qaManager.GetMaxClaimsCount().ShouldBeEqual("100", "Max value should display");
                //_qaManager.ClosePageError();
                _qaManager.SetClaimsCount(claimsCount[0], 1);
                _qaManager.SetMaxClaimsCount(maxClaims[0], 1);
                _qaManager.ClickOnSaveButton();
                //_qaManager.ClickOnGridRowToOpenSideWindow();
                _qaManager.GetGridViewSection.ClickOnGridRowByRow();
                _qaManager.DeleteClientInEditSection();
                _qaManager.SetClient(clientList[0]);
                _qaManager.SetClient(clientList[1]);
                _qaManager.IsPageErrorPopupModalPresent()
                    .ShouldBeTrue("The 'ALL' client row must be removed before a client-specific row can be added.");
                _qaManager.ClosePageError();
                _qaManager.DeleteClientInEditSection();
                _qaManager.SetClient(clientList[1]);
                _qaManager.SetClaimsCount(claimsCount[1], 1);
                _qaManager.SetMaxClaimsCount(maxClaims[1], 1);
                _qaManager.ClickOnSaveButton();
            }
        }

        [Test, Category("SchemaDependent")] //CAR-2390 + CAR-3127(CAR-3086)
        [Retry(3)]
        public void Verify_QA_claims_based_on_client_code_or_all_client()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(1))
            {
                QaManagerPage _qaManager;

                automatedBase.CurrentPage =
                    _qaManager = automatedBase.QuickLaunch.NavigateToAnalystManager();
                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> testData = automatedBase.DataHelper.GetTestData(GetType().FullName,
                    TestName);
                var claSeq1 = testData["ClaimSequence1"];
                var claSeq2 = testData["ClaimSequence2"];
                var date = testData["Date"];
                var analyst = testData["Analyst"];
                var labels = testData["Labels"].Split(',').ToList();
                var toolTips = testData["ToolTips"].Split(',').ToList();

                //_qaManager.Logout().LoginAsHciAdminUser1().NavigateToAnalystManager();

                for (int i = 0; i < 3; i++)
                {
                    _qaManager.SearchByAnalyst(analyst);
                    //_qaManager.ClickOnGridRowToOpenSideWindow();
                    _qaManager.GetGridViewSection.ClickOnGridRowByRow();
                    if (_qaManager.GetSideBarPanelSearch.IsSideBarPanelOpen())
                        _qaManager.GetSideBarPanelSearch.ClickOnToggleSidebarPanelButton();
                    while (_qaManager.IsDeleteClientInEditSectionPresent())
                        _qaManager.DeleteClientInEditSection();

                    #region CAR-3127(CAR-3086)
                    StringFormatter.PrintMessage("Verification of QA, QC radio buttons tooltip");
                    _qaManager.IsQaQcRadioButtonsPresentByLabel(labels[0]).ShouldBeFalse($"Is {labels[0]} present?");
                    _qaManager.IsQaQcRadioButtonsPresentByLabel(labels[1]).ShouldBeFalse($"Is {labels[1]} present?");
                    #endregion

                    if (i == 0)
                    {
                        StringFormatter.PrintMessageTitle($"Only {ClientEnum.SMTST.ToString()} client active");
                        var newBeginDate = DateTime.Now.AddDays(-1).ToString("MM/dd/yyyy");
                        var newEndDate = DateTime.Now.AddDays(5).ToString("MM/dd/yyyy");
                        _qaManager.SetEndDateWithoutUsingCalender(newEndDate);
                        _qaManager.SetBeginDateWithoutUsingCalender(newBeginDate);
                        if (_qaManager.IsPageErrorPopupModalPresent()) _qaManager.ClosePageError();
                        if (_qaManager.GetEndDate() != newEndDate)
                            _qaManager.SetEndDateWithoutUsingCalender(newEndDate);
                    }

                    if (i == 2)
                    {
                        StringFormatter.PrintMessageTitle(
                            "All Client Active and condition should be applied for both clients");
                        _qaManager.SetClient("ALL");
                        _qaManager.SetClaimsCount("1");
                        _qaManager.SetMaxClaimsCount("3");
                    }
                    else
                    {
                        StringFormatter.PrintMessageTitle(
                            $"Both {ClientEnum.SMTST.ToString()},{ClientEnum.RPE.ToString()} clients are active but claim count is different");

                        _qaManager.SetClient(ClientEnum.SMTST.ToString());
                        _qaManager.SetClaimsCount(i == 0 ? "1" : "2");
                        _qaManager.SetMaxClaimsCount("8");

                        if (i == 1)
                        {

                            _qaManager.SetClient(ClientEnum.RPE.ToString());
                            _qaManager.SetClaimsCount("1", 2);
                            _qaManager.SetMaxClaimsCount("8", 2);
                        }
                    }

                    #region CAR-3127(CAR-3086)
                    StringFormatter.PrintMessage("Verification of QA, QC radio buttons tooltip");
                    _qaManager.IsQaQcRadioButtonsPresentByLabel(labels[0]).ShouldBeTrue($"Is {labels[0]} present?");
                    _qaManager.IsQaQcRadioButtonsPresentByLabel(labels[1]).ShouldBeTrue($"Is {labels[1]} present?");
                    _qaManager.GetToolTipInfoOfQaQcButtonsByLabel(labels[0])
                        .ShouldBeEqual(toolTips[0], $"ToolTip for {labels[0]} should match");
                    _qaManager.GetToolTipInfoOfQaQcButtonsByLabel(labels[1])
                        .ShouldBeEqual(toolTips[1], $"ToolTip for {labels[1]} should match");
                    _qaManager.IsQaQcRadioButtonSelectedByLabel(labels[0]).ShouldBeTrue("QA radio button should be default selected");
                    _qaManager.IsQaQcRadioButtonSelectedByLabel(labels[1]).ShouldBeFalse("QC radio button should not be default selected");
                    #endregion

                    _qaManager.ClickOnSaveButton();

                    StringFormatter.PrintMessageTitle("Revert the claim and qa counter data to default value");

                    _qaManager.DeleteQaAuditRecordOfClaimApproveAndChangeClaimStatus(claSeq1, date);
                    _qaManager.DeleteQaAuditRecordOfClaimApproveAndChangeClaimStatus(claSeq2, date);
                    _qaManager.DeleteQaAuditRecordOfClaimApproveAndChangeClaimStatus(claSeq1, date,
                        ClientEnum.RPE.ToString());
                    _qaManager.DeleteQaAuditRecordOfClaimApproveAndChangeClaimStatus(claSeq2, date,
                        ClientEnum.RPE.ToString());
                    _qaManager.ResetUserQaCounterToDefault(automatedBase.EnvironmentManager.HciAdminUsername1);

                    var claimSearch = _qaManager.NavigateToClaimSearch();
                    automatedBase.CurrentPage.RefreshPage();
                    var claimAction = claimSearch.SearchByClaimSequenceToNavigateToClaimActionPage(claSeq1);
                    claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                    claimAction.ClickOnApproveButtonAndNavigateToNewClaimSearchPage(claSeq1);


                    claimSearch.SearchByClaimSequenceToNavigateToClaimActionPage(claSeq2);
                    claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                    claimAction.ClickOnApproveButtonAndNavigateToNewClaimSearchPage(claSeq2);


                    claimSearch.ClickOnSwitchClient().SwitchClientTo(ClientEnum.RPE, true);
                    automatedBase.CurrentPage.NavigateToClaimSearch();
                    automatedBase.CurrentPage.RefreshPage();

                    claimSearch.SearchByClaimSequenceToNavigateToClaimActionPage(claSeq1);
                    claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                    claimAction.ClickOnApproveButtonAndNavigateToNewClaimSearchPage(claSeq1);


                    claimSearch.SearchByClaimSequenceToNavigateToClaimActionPage(claSeq2);
                    claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                    claimAction.ClickOnApproveButtonAndNavigateToNewClaimSearchPage(claSeq2);


                    claimSearch.ClickOnSwitchClient().SwitchClientTo(ClientEnum.SMTST, true);

                    var qaClaimSearch = automatedBase.CurrentPage.NavigateToQaClaimSearch();
                    qaClaimSearch.SelectOutstandingQaClaims();
                    qaClaimSearch.SelectClient(ClientEnum.SMTST.ToString());
                    qaClaimSearch.SearchByAnalyst(analyst);

                    //This portion covers the part of CAR-3127(CAR-3086)
                    switch (i)
                    {
                        case 0:
                            qaClaimSearch.IsClaimSequencePresentInGridPanel(claSeq1)
                                .ShouldBeTrue(
                                    $"Claim Sequence : {claSeq1} should be in qa review claim for {ClientEnum.SMTST.ToString()} client");
                            qaClaimSearch.IsClaimSequencePresentInGridPanel(claSeq2)
                                .ShouldBeTrue(
                                    $"Claim Sequence : {claSeq2} should be in qa review claim for {ClientEnum.SMTST.ToString()} client");

                            qaClaimSearch.SelectClientAndFind(ClientEnum.RPE.ToString());
                            qaClaimSearch.IsClaimSequencePresentInGridPanel(claSeq1)
                                .ShouldBeFalse(
                                    $"Claim Sequence : {claSeq1} should not be in qa review claim for {ClientEnum.RPE.ToString()} client");
                            qaClaimSearch.IsClaimSequencePresentInGridPanel(claSeq2)
                                .ShouldBeFalse(
                                    $"Claim Sequence : {claSeq2} should be in qa review claim for {ClientEnum.RPE.ToString()} client");
                            break;
                        case 1:
                            qaClaimSearch.IsClaimSequencePresentInGridPanel(claSeq1)
                                .ShouldBeFalse(
                                    $"Claim Sequence : {claSeq1} should not be in qa review claim for {ClientEnum.SMTST.ToString()} client");
                            qaClaimSearch.IsClaimSequencePresentInGridPanel(claSeq2)
                                .ShouldBeTrue(
                                    $"Claim Sequence : {claSeq2} should be in qa review claim for {ClientEnum.SMTST.ToString()} client");

                            qaClaimSearch.SelectClientAndFind(ClientEnum.RPE.ToString());
                            qaClaimSearch.IsClaimSequencePresentInGridPanel(claSeq1)
                                .ShouldBeTrue(
                                    $"Claim Sequence : {claSeq1} should be in qa review claim for {ClientEnum.RPE.ToString()} client");
                            qaClaimSearch.IsClaimSequencePresentInGridPanel(claSeq2)
                                .ShouldBeTrue(
                                    $"Claim Sequence : {claSeq2} should be in qa review claim for {ClientEnum.RPE.ToString()} client");
                            break;
                        case 2:
                            qaClaimSearch.IsClaimSequencePresentInGridPanel(claSeq1)
                                .ShouldBeTrue(
                                    $"Claim Sequence : {claSeq1} should be in qa review claim for {ClientEnum.SMTST.ToString()} client");
                            qaClaimSearch.IsClaimSequencePresentInGridPanel(claSeq2)
                                .ShouldBeTrue(
                                    $"Claim Sequence : {claSeq2} should be in qa review claim for {ClientEnum.SMTST.ToString()} client");

                            qaClaimSearch.SelectClientAndFind(ClientEnum.RPE.ToString());
                            qaClaimSearch.IsClaimSequencePresentInGridPanel(claSeq1)
                                .ShouldBeTrue(
                                    $"Claim Sequence : {claSeq1} should be in qa review claim for {ClientEnum.RPE.ToString()} client");
                            qaClaimSearch.IsClaimSequencePresentInGridPanel(claSeq2)
                                .ShouldBeFalse(
                                    $"Claim Sequence : {claSeq2} should not be in qa review claim for {ClientEnum.RPE.ToString()} client");
                            break;
                    }

                    _qaManager = qaClaimSearch.NavigateToAnalystManager();
                }
            }
        }

        [Test]
        public void Verify_Add_PTO_Button() //CAR-2973(CAR-2785)
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                QaManagerPage _qaManager;

                automatedBase.CurrentPage =
                    _qaManager = automatedBase.QuickLaunch.NavigateToAnalystManager();
                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> testData = automatedBase.DataHelper.GetTestData(GetType().FullName,
                    TestName);
                var Analyst = testData["Analyst"];
                var userSeq = testData["Userseq"];
                var errorMessage = testData["Errormessage"].Split(',').ToList();
                List<string> DateList = new List<string>
                {
                    DateTime.Now.AddDays(-1).ToString("MM/dd/yyyy"), DateTime.Now.ToString("MM/dd/yyyy"),
                    DateTime.Now.AddDays(1).ToString("MM/dd/yyyy")
                };

                _qaManager.DeleteAnalystPTO(Analyst);
                _qaManager.SearchByAnalyst(testData["AnalystName"]);
                _qaManager.ClickOnSearchListRow(1);

                StringFormatter.PrintMessage(
                    "Verify passed PTO Date are not displayed by adding PTO date earlier than current date in DB");
                _qaManager.InsertAnalystPTO(userSeq);
                _qaManager.ClickOnPTOIcon();
                _qaManager.NoentriesSectionPresent().ShouldBeTrue("No entries section present");

                StringFormatter.PrintMessage("verify Icons in PTO Section");
                _qaManager.IsPlannedTimeOffFormDisabled().ShouldBeTrue("Planned PTO form should be disabled");
                _qaManager.GetSideWindow.ClickOnEditIcon();
                _qaManager.IsAddNewEntryButtonPresent().ShouldBeTrue("Add New Entry button present?");
                _qaManager.ClickOnAddNewEntryButton();
                _qaManager.IsDaterangePresent().ShouldBeTrue("Date range field is present?");
                _qaManager.IsSaveButtonPresent().ShouldBeTrue("Save button present?");
                _qaManager.IsCancelLinkPresent().ShouldBeTrue("cancel link present?");
                _qaManager.ClickOnSaveButton();
                ValidateDateRangePopupmessage(_qaManager, errorMessage[0]);

                StringFormatter.PrintMessage(
                    "Verify End date gets auto populated after entering begin date and date range validation to enter future date");
                _qaManager.SetPTODateWithoutUsingCalendar(DateList[0]);
                _qaManager.GetEndDate().ShouldBeEqual(DateList[0], "End date copied to begin date?");
                _qaManager.ClickOnSaveButton();
                _qaManager.WaitForWorking();
                ValidateDateRangePopupmessage(_qaManager, errorMessage[1]);

                StringFormatter.PrintMessage(
                    "Validation for PTO being applied for current date");
                AddUpdatePtoDateRange(_qaManager, DateList[1], DateList[0], isSave: false, isactionrequired: false, isnew: false);
                ValidateDateRangePopupmessage(_qaManager, errorMessage[2]);

                StringFormatter.PrintMessage("verify cancel link");
                _qaManager.ClickOnCancelLink();
                _qaManager.WaitForWorking();
                _qaManager.IsPlannedTimeOffFormDisabled().ShouldBeTrue("Planned time off form disabled?");

                StringFormatter.PrintMessage("Verify Addition of new Analyst PTO");
                _qaManager.DeleteAnalystPTO(Analyst);
                AddUpdatePtoDateRange(_qaManager, DateList[1], DateList[2]);
                _qaManager.GetTotalAnalystPTO().ShouldBeEqual(1, "Date range added?");
                _qaManager.IsPlannedTimeOffFormDisabled()
                    .ShouldBeTrue("Planned PTO form should be disabled after save");
                _qaManager.GetAnalystPTO(Analyst)
                    .ShouldBeEqual($"{DateList[1]}-{DateList[2]}", "Analyst date saved correct in DB?");


                StringFormatter.PrintMessage("Verify addition of same date range PTO");
                AddUpdatePtoDateRange(_qaManager, DateList[1], DateList[2], row: 2);
                ValidateDateRangePopupmessage(_qaManager, errorMessage[3]);
                _qaManager.ClickOnCancelLink();
                _qaManager.WaitForWorking();


                StringFormatter.PrintMessage(
                    "Verify update Analyst PTO, verify Same date is allowed in both start and end date");
                AddUpdatePtoDateRange(_qaManager, DateList[2], DateList[2], isnew: false);
                _qaManager.GetAnalystPTO(Analyst)
                    .ShouldBeEqual($"{DateList[2]}-{DateList[2]}", "Analyst date saved correct in DB?");

                StringFormatter.PrintMessage("Verify delete of Analyst PTO");
                _qaManager.GetSideWindow.ClickOnEditIcon();
                _qaManager.DeleteAllPTO();
                _qaManager.ClickOnSaveButton();
                _qaManager.WaitForWorking();
                _qaManager.NoentriesSectionPresent().ShouldBeTrue("No new entry");


                StringFormatter.PrintMessage("Verify addition of multiple PTOs and they are sorted in ascending order");
                _qaManager.AddMultipleAnalystPTO(2);
                _qaManager.GetTotalAnalystPTO().ShouldBeEqual(2, "Date range added?");
                _qaManager.IsAnalystPTOSortedInAscendingOrder().ShouldBeTrue("Analyst Pro Sorted in Ascending Order?");
            }
        }

        [Test] //CAR-3190(CAR-3152)
        [Author("Pujan Aryal")]
        public void Verify_Add_Business_Day_Only_Option_For_QC_In_Claim_QA_Settings()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(3))
            {
                QaManagerPage qaManager = automatedBase.QuickLaunch.NavigateToAnalystManager();
                var TestName = new StackFrame(true).GetMethod().Name;

                IDictionary<string, string> testData = automatedBase.DataHelper.GetTestData(GetType().FullName, TestName);
                var claSeq = testData["ClaimSequence"];
                var claSeqNoOption = testData["ClaimSequenceNoOption"];
                var analyst = testData["Analyst"];
                var date = testData["Date"];
                var tooltipValue = testData["ToolTipValue"];
                var newBeginDate = DateTime.Now.AddDays(-1).ToString("MM/dd/yyyy");
                var newEndDate = DateTime.Now.AddDays(2).ToString("MM/dd/yyyy");

                qaManager.DeleteQaAuditRecordOfClaimApproveAndChangeClaimStatusForQCReview(claSeq, date);
                qaManager.DeleteQaAuditRecordOfClaimApproveAndChangeClaimStatusForQCReview(claSeqNoOption, date);
                qaManager.ResetUserQaCounterToDefault(automatedBase.EnvironmentManager.HciAdminUsername3);

                try
                {
                    StringFormatter.PrintMessage("Verify presence of QC Business Days Only Option and Tooltip Value");
                    qaManager.ActivateAnalystForGivenDateRangeAndTarget(newBeginDate, newEndDate, "1", "5", analyst, true);
                    qaManager.IsQcBusinessDaysOnlyOptionPresent().ShouldBeTrue("QC Business Days Only should be present");
                    qaManager.GetQcBusinessDaysOnlyTooltip().ShouldBeEqual(tooltipValue);
                    qaManager.IsQaBusinessDaysOnlyOptionSelectedByDefault().ShouldBeTrue("QC Business Days Only Option should be selected by default");

                    StringFormatter.PrintMessage("Verify when the option is selected, " +
                                                 "and the user is put on QC review for any client, " +
                                                 "the QC claims will only be added to the QC review queue " +
                                                 "during the the hours of Monday through Friday, " +
                                                 "6am MST - 6pm MST, excluding Cotiviti holidays.");
                    var newClaimSearch = qaManager.NavigateToClaimSearch();
                    ApproveClaimsInClaimActionAndReturnToClaimSearch(newClaimSearch, claSeq);

                    if (IsWeekdayAndTimeBetween6AmAnd6PmMST() && !IsHoliday())
                        qaManager.IsClaimSeqAlreadySelectedForDailyQc(claSeq)
                            .ShouldBeTrue($"Claim sequence: {claSeq} should be selected for QC review.");
                    else
                        qaManager.IsClaimSeqAlreadySelectedForDailyQc(claSeq)
                            .ShouldBeFalse($"Claim sequence: {claSeq} should be selected for QC review.");

                    StringFormatter.PrintMessage("Verify if the option is not selected, QC claims will be selected based on current functionality, regardless of day or time.");
                    automatedBase.CurrentPage.NavigateToAnalystManager();
                    qaManager.SearchByAnalyst(analyst);
                    qaManager.GetGridViewSection.ClickOnGridRowByRow();
                    qaManager.GetSideWindow.CheckOrUncheckByLabel("QC Business Days Only", false);

                    qaManager.ClickOnSaveButton();
                    qaManager.NavigateToClaimSearch();
                    ApproveClaimsInClaimActionAndReturnToClaimSearch(newClaimSearch, claSeqNoOption);
                    qaManager.IsClaimSeqAlreadySelectedForDailyQc(claSeqNoOption)
                        .ShouldBeTrue($"Claim sequence: {claSeqNoOption} should be selected for QC review regardless of day or time.");
                }
                finally
                {
                    StringFormatter.PrintMessageTitle("End of test. Finally block running");

                    StringFormatter.PrintMessage("Checking the analyst options to reinstate the initial conditions");
                    automatedBase.CurrentPage.NavigateToAnalystManager();
                    qaManager.SearchByAnalyst(analyst);
                    qaManager.GetGridViewSection.ClickOnGridRowByRow();
                    qaManager.GetSideWindow.CheckOrUncheckByLabel("QC Business Days Only", true);

                    qaManager.SelectRadioButtonOptionByLabel("QA");
                    qaManager.ClickOnSaveButton();

                    StringFormatter.PrintMessage("Reverting the test data in DB");
                    qaManager.DeleteQaAuditRecordOfClaimApproveAndChangeClaimStatus(claSeq, date);
                    qaManager.DeleteQaAuditRecordOfClaimApproveAndChangeClaimStatus(claSeqNoOption, date);
                    qaManager.ResetUserQaCounterToDefault(automatedBase.EnvironmentManager.HciAdminUsername3);
                }

                #region Local Methods

                bool IsHoliday() => qaManager.IsApprovedDateHoliday(claSeq);

                bool IsWeekdayAndTimeBetween6AmAnd6PmMST()
                {
                    bool isWeekday = false;
                    bool isTimeValid = true;
                    List<string> dayHour = qaManager.GetDayAndHourFromApprovedDate(claSeq);
                    int day = Convert.ToInt32(dayHour[0]);
                    int hour = Convert.ToInt32(dayHour[1]);

                    //Checking the approved datetime for working hours and holidays
                    if (day >= 2 && day <= 6)
                        isWeekday = true;
                    if (hour < 6 || hour > 17)
                        isTimeValid = false;

                    return (isWeekday && isTimeValid);
                }
                #endregion
            }
        }

        #endregion




        #region Private Methods
        private void ApproveClaimsInClaimActionAndReturnToClaimSearch(ClaimSearchPage claimSearch, string claimSequence)
        {
            var newClaimAction = claimSearch.SearchByClaimSequenceToNavigateToClaimActionPage(claimSequence);
            newClaimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
            newClaimAction.ClickOnApproveButton();
            newClaimAction.WaitForWorkingAjaxMessage();
        }

        private void SearchByAnalystAndNavigateToAppealCategoryAssignment(QaManagerPage _qaManager, string analyst)
        {
            _qaManager.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Analyst", analyst);
            _qaManager.GetSideBarPanelSearch.ClickOnFindButton();
            _qaManager.GetGridViewSection.ClickOnGridRowByRow();
            _qaManager.ClickAnalystManagerIconByColNum(2);
            _qaManager.WaitForSpinner();
        }

        private void ValidateForPopUpMessage(QaManagerPage _qaManager, string popUpMessgae, string expectedError)
        {
            _qaManager.WaitToLoadPageErrorPopupModal();
            _qaManager.IsPageErrorPopupModalPresent()
                .ShouldBeTrue(popUpMessgae);
            _qaManager.GetPageErrorMessage()
                .ShouldBeEqual(expectedError,
                    popUpMessgae);
            _qaManager.ClosePageError();
        }

        private void ValidateDateRangePopupmessage(QaManagerPage _qaManager, string errorMessage)
        {
            _qaManager.IsPageErrorPopupModalPresent().ShouldBeTrue("Error Pop up displayed ");
            _qaManager.GetPageErrorMessage().ShouldBeEqual(errorMessage, "Error message correct?");
            _qaManager.ClosePageError();
        }

        private void AddUpdatePtoDateRange(QaManagerPage _qaManager, string startdate, string enddate, int row = 1, bool isnew = true, bool isSave = true, bool isactionrequired = true)
        {
            if (isactionrequired)
            {
                _qaManager.GetSideWindow.ClickOnEditIcon();
            }
            if (isnew)
            {
                _qaManager.ClickOnAddNewEntryButton();
            }
            _qaManager.SetPTODateWithoutUsingCalendar(startdate, row);
            _qaManager.SetPTODateWithoutUsingCalendar(enddate, row, isBegin: false);
            if (isSave)
            {
                _qaManager.ClickOnSaveButton();
                _qaManager.WaitForWorking();
            }


        }
        #endregion
    }
}

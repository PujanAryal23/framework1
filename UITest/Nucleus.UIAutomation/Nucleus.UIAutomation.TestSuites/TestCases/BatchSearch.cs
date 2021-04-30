using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Nucleus.Service.PageServices.Batch;
using Nucleus.Service.Support.Common;
using Nucleus.Service.PageServices.Settings.User;
using Nucleus.Service.Support.Common.Constants;
using Nucleus.Service.Support.Enum;
using Nucleus.UIAutomation.TestSuites.Base;
using NUnit.Framework;
using Nucleus.Service.Support.Environment;
using UIAutomation.Framework.Utils;
using System.Diagnostics;
using System.Globalization;
using Nucleus.Service.Data;
using Nucleus.Service.PageServices.Claim;
using Nucleus.Service.Support.Menu;
using Nucleus.UIAutomation.TestSuites.Common;
using NUnit.Framework.Internal;
using UIAutomation.Framework.Core.Driver;
using static System.Console;
using static System.String;


namespace Nucleus.UIAutomation.TestSuites.TestCases
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class BatchSearch
    {/*
        #region PRIVATE FIELDS

         private BatchSearchPage _batchSearch;
         private ProfileManagerPage _profileManager;
         private ClaimSearchPage _claimSearch;
         private ClaimActionPage _claimAction;
         private BatchSummaryPage _batchSummary;
         private CommonValidations _commonValidation;

         #endregion



         #region OVERRIDE METHODS

         /// <summary>
         /// Override ClassInit to add additional code.
         /// </summary>
         protected override void ClassInit()
         {
             try
             {
                 base.ClassInit();
                 CurrentPage = _batchSearch = QuickLaunch.NavigateToBatchSearch();
                 _commonValidation = new CommonValidations(CurrentPage);
             }
             catch (Exception ex)
             {
                 if (StartFlow != null)
                     StartFlow.Dispose();
                 throw;
             }
         }

         protected override void TestInit()
         {
             base.TestInit();
             CurrentPage = _batchSearch;
         }

         protected override void ClassCleanUp()
         {
             try
             {
                 _batchSearch.CloseDbConnection();
             }

             finally
             {
                 base.ClassCleanUp();
             }
         }

         protected override void TestCleanUp()

         {
             if (string.Compare(UserType.CurrentUserType, UserType.HCIADMIN, StringComparison.OrdinalIgnoreCase) != 0)
             {
                 _batchSearch = _batchSearch.Logout().LoginAsHciAdminUser().NavigateToBatchSearch();
             }

             if (_batchSearch.GetPageHeader() != PageHeaderEnum.BatchSearch.GetStringValue())
             {
                 _batchSearch.ClickOnQuickLaunch().NavigateToBatchSearch();
             }

             _batchSearch.SideBarPanelSearch.OpenSidebarPanel();
             _batchSearch.SideBarPanelSearch.ClickOnClearLink();
             _batchSearch.SideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                 BatchQuickSearchTypeEnum.IncompleteBatches.GetStringValue());
             _batchSearch.SideBarPanelSearch.ClickOnFindButton();
             _batchSearch.WaitForWorkingAjaxMessage();
             base.TestCleanUp();
         }

         #endregion*/
        #region PROTECTED PROPERTIES

        protected string FullyQualifiedClassName
        {
            get { return GetType().FullName; }
        }

        #endregion

        #region TestSuites


        [Test] //TE56
        [Retrying(Times = 3)]
        public void Verify_security_and_navigation_of_Batch_Search_page()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                BatchSearchPage _batchSearch;
                CommonValidations _commonValidation;
                automatedBase.CurrentPage =
                    _batchSearch = automatedBase.QuickLaunch.NavigateToBatchSearch();
                _commonValidation = new CommonValidations(automatedBase.CurrentPage);
                _commonValidation.ValidateSecurityAndNavigationOfAPage(HeaderMenu.Batch,
                    new List<string> { SubMenu.BatchSearch },
                    RoleEnum.ClaimsProcessor.GetStringValue(),
                    new List<string> { PageHeaderEnum.BatchSearch.GetStringValue() },
                    automatedBase.Login.LoginAsUserHavingNoAnyAuthority,
                    new[] { "Test4", "Automation4" }, automatedBase.Login.LoginAsHciAdminUser);
                _batchSearch = automatedBase.CurrentPage.Logout().LoginAsHciAdminUser().NavigateToBatchSearch();

                _batchSearch.SideBarPanelSearch.IsSideBarPanelOpen()
                    .ShouldBeTrue("Side Bar panel should be opened by default");
                _batchSearch.SideBarPanelSearch.ClickOnToggleSidebarPanelButton();
                _batchSearch.SideBarPanelSearch.IsSideBarPanelOpen()
                    .ShouldBeFalse("Sidebar Panel should be hidden when toggle button is clicked.");

            }
        }

        [Test] //TE-93
        public void Verify_Search_Filters_and_their_Default_values()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                BatchSearchPage _batchSearch;
                automatedBase.CurrentPage =
                    _batchSearch = automatedBase.QuickLaunch.NavigateToBatchSearch();
                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestName);
                var expectedBatchSearchFilterList = automatedBase.DataHelper
                    .GetMappingData(FullyQualifiedClassName, "Batch_Search_Filter_List").Values.ToList();
                var expectedQuickSearchOptions = automatedBase.DataHelper
                    .GetMappingData(FullyQualifiedClassName, "quick_search_options_for_batch_search").Values.ToList();

                _batchSearch.ClickOnQuickLaunch().NavigateToBatchSearch();
                ValidateDefaultValuesOfFiltersExceptQuickSearchFilter(_batchSearch);

                _batchSearch.SideBarPanelSearch.GetSearchFiltersList()
                    .ShouldCollectionBeEqual(expectedBatchSearchFilterList,
                        "Batch Search filter should be as expected.");

                StringFormatter.PrintMessageTitle("Validate Quick Search dropdown");
                ValidateQuickSearchDropDownForDefaultValueAndExpectedList(_batchSearch, "Quick Search", expectedQuickSearchOptions);

                _batchSearch.SideBarPanelSearch.GetInputValueByLabel("Batch ID")
                    .ShouldBeNullorEmpty("Default value of Batch ID field should be empty.");
                _batchSearch.SideBarPanelSearch.SetInputFieldByLabel("Batch ID", paramLists["Alphanumeric"]);
                _batchSearch.SideBarPanelSearch.GetLengthOfTheInputFieldByLabel("Batch ID")
                    .ShouldBeEqual(50, "Max char limit of Batch ID field should be 50");

                StringFormatter.PrintMessageTitle("Validate Client Create Date field");
                ValidateDatePickerField(_batchSearch, "Client Create Date", paramLists["Message1"], paramLists["Message2"],
                    paramLists["Message3"]);

                StringFormatter.PrintMessageTitle("Validate Cotiviti Create Date field");
                ValidateDatePickerField(_batchSearch, "Cotiviti Create Date", paramLists["Message1"], paramLists["Message2"],
                    paramLists["Message3"]);

                _batchSearch.SideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                    BatchQuickSearchTypeEnum.ReceivedThisWeek.GetStringValue());
                StringFormatter.PrintMessageTitle("Verify Clear filter");
                _batchSearch.SideBarPanelSearch.ClickOnClearLink();

                ValidateDefaultValuesOfFiltersExceptQuickSearchFilter(_batchSearch);
                _batchSearch.SideBarPanelSearch.GetInputValueByLabel("Quick Search").ShouldBeEqual(
                    BatchQuickSearchTypeEnum.ReceivedThisWeek.GetStringValue(),
                    "Clear button should all filter fields except selected quick search field");


                StringFormatter.PrintMessageTitle("Verify All quick search filter only cannot initiate search");
                _batchSearch.SideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                    BatchQuickSearchTypeEnum.AllBatches.GetStringValue());
                _batchSearch.SideBarPanelSearch.ClickOnFindButton();
                _batchSearch.WaitForWorkingAjaxMessage();
                _batchSearch.IsPageErrorPopupModalPresent()
                    .ShouldBeTrue("User should not be able to search by All Batches only");
                _batchSearch.GetPageErrorMessage().ShouldBeEqual(
                    "Search cannot be initiated without any criteria entered.",
                    "Verify the popup message when search by all batches only");
                _batchSearch.ClosePageError();
                _batchSearch.SideBarPanelSearch.SetInputFieldByLabel("Batch ID", paramLists["BatchId"]);
                _batchSearch.SideBarPanelSearch.ClickOnFindButton();
                _batchSearch.IsPageErrorPopupModalPresent()
                    .ShouldBeFalse("User should be able to search by All Batches with any other criteria selected.");
                _batchSearch.SideBarPanelSearch.ClickOnClearLink();


            }
        }



        [Test] //TE56 
        public void Verify_secondary_details_in_Batch_search_page_and_presence_of_checkbox_against_product_label()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                BatchSearchPage _batchSearch;
                CommonValidations _commonValidation;
                automatedBase.CurrentPage =
                    _batchSearch = automatedBase.QuickLaunch.NavigateToBatchSearch();
                var TestName = new StackFrame(true).GetMethod().Name;
                var batchId1 = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestName,
                    "BatchId1", "Value");
                var batchId2 = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestName,
                    "BatchId2", "Value");
                var _activeProductListForClientDB = _batchSearch.GetActiveProductListForClientDB();

                _batchSearch.SideBarPanelSearch.OpenSidebarPanel();
                _batchSearch.SideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                    BatchQuickSearchTypeEnum.AllBatches.GetStringValue());
                _batchSearch.SideBarPanelSearch.SetInputFieldByLabel("Batch ID", batchId2);
                _batchSearch.SideBarPanelSearch.ClickOnButtonByButtonName("Find");
                _batchSearch.WaitForWorking();

                StringFormatter.PrintMessageTitle(
                    "Verifying product labels,Released and Complete column's label and values from DB ");

                //_batchSearch.GetProductLabels()
                //    .ShouldCollectionBeEqual(_activeProductListForClientDB, "Both the list should match");
                _batchSearch.GetGridViewSection.GetLabelInGridByColRow(3).ShouldBeEqual("Released:");
                _batchSearch.GetGridViewSection.GetValueInGridByColRow(3)
                    .ShouldBeEqual(_batchSearch.GetRelesedUserDateFromDatabase(batchId2));
                _batchSearch.GetGridViewSection.GetLabelInGridByColRow(4).ShouldBeEqual("Complete:");
                _batchSearch.GetGridViewSection.GetValueInGridByColRow(4)
                    .ShouldBeEqual(_batchSearch.GetBatchCompleteDateFromDatabase(batchId2));

                //TE-508
                _batchSearch.GetGridViewSection.GetLabelInGridByColRow(5).ShouldBeEqual("Batch Date:");
                _batchSearch.GetGridViewSection.GetValueInGridByColRow(5)
                    .ShouldBeEqual(_batchSearch.GetBatchDateFromDatabase(batchId2));

                _batchSearch.GetGridViewSection.ClickOnGridByRowCol();
                StringFormatter.PrintMessageTitle(
                    "Verifying secondary details values and checkmark against product from Database ");
                VerifyCheckboxAgainstProductLabel(_batchSearch, batchId2, _activeProductListForClientDB);
                _batchSearch.SideBarPanelSearch.OpenSidebarPanel();
                _batchSearch.SideBarPanelSearch.ClickOnClearLink();

                StringFormatter.PrintMessageTitle(
                    "Verifying secondary details values and checkmark against product from Database ");
                _batchSearch.SideBarPanelSearch.SetInputFieldByLabel("Batch ID", batchId1);
                _batchSearch.SideBarPanelSearch.ClickOnFindButton();
                _batchSearch.WaitForWorking();
                _batchSearch.GetGridViewSection.ClickOnGridByRowCol();
                _batchSearch.GetBatchDetailsHeader().ShouldBeEqual("Batch Details");
                _batchSearch.GetBatchDetailsSecondaryViewValueByLabel("Total Flagged Claims").ShouldBeEqual(
                    _batchSearch.GetTotalClaimsCountInSecondaryDetails(batchId1), "Values should match");
                _batchSearch.GetBatchDetailsSecondaryViewValueByLabel("Total Unreviewed")
                    .ShouldBeEqual(_batchSearch.GetTotalUnreviewedClaimsCountFromDatabase(batchId1));
                _batchSearch.GetBatchDetailsSecondaryViewValueByLabel("Cotiviti Create Date")
                    .ShouldBeEqual(_batchSearch.GetCotivitiAndClientCreateDateFromDatabase(batchId1)[0]);
                VerifyCheckboxAgainstProductLabel(_batchSearch, batchId1, _activeProductListForClientDB);


            }
        }

        [Test] //TE-93
        public void Verify_search_result_for_different_quick_search_options()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                BatchSearchPage _batchSearch;
                CommonValidations _commonValidation;
                automatedBase.CurrentPage =
                    _batchSearch = automatedBase.QuickLaunch.NavigateToBatchSearch();

                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestName);

                var receivedThisWeekBatches = _batchSearch.GetBatchesReceivedThisWeekFromDatabase();
                var incompleteBatches = _batchSearch.GetIncompleteBatchesListromDatabase();

                List<string>[] ValuesFromDB =
                    {receivedThisWeekBatches, incompleteBatches};
                string[] batchStringValue =
                {

                    BatchQuickSearchTypeEnum.ReceivedThisWeek.GetStringValue(),
                    BatchQuickSearchTypeEnum.IncompleteBatches.GetStringValue(),
                };


                for (int j = 0; j < ValuesFromDB.Length; j++)
                {

                    Verify_the_Batch_list_with_database(_batchSearch, batchStringValue[j], ValuesFromDB[j]);
                    _batchSearch.SideBarPanelSearch.SetInputFieldByLabel("Batch ID",
                        paramLists["BatchID" + (j + 1)]);
                    _batchSearch.SideBarPanelSearch.SetInputFieldByLabel("Client Create Date",
                        paramLists["ClientCreateDate" + (j + 1)]);
                    _batchSearch.SideBarPanelSearch.ClickOnHeader();
                    _batchSearch.SideBarPanelSearch.SetInputFieldByLabel("Cotiviti Create Date",
                        paramLists["CotivitiCreateDate" + (j + 1)]);
                    _batchSearch.SideBarPanelSearch.ClickOnFindButton();
                    _batchSearch.WaitForWorkingAjaxMessage();

                    if (_batchSearch.SideBarPanelSearch.GetInputValueByLabel("Quick Search") ==
                        BatchQuickSearchTypeEnum.ReceivedThisWeek.GetStringValue())
                    {
                        _batchSearch.SideBarPanelSearch.IsNoMatchingRecordFoundMessagePresent()
                            .ShouldBeTrue(
                                "Search results displayed for" +
                                batchStringValue[j]); //No data for received this week option
                    }
                    else
                    {
                        _batchSearch.GetGridViewSection.GetGridRowCount()
                            .ShouldBeGreater(0, "Search results displayed for" + batchStringValue[j]);
                    }

                    _batchSearch.SideBarPanelSearch.ClickOnClearLink();

                }

            }
        }

        [Test,Category("OnDemand")] //TE-86+ TE-382 + TE-524 + CAR-2209
        [Order(1)]
        [NonParallelizable]
        public void Verify_batch_release_functionality()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                BatchSearchPage _batchSearch;
                ClaimSearchPage _claimSearch;
                ClaimActionPage _claimAction;
                BatchSummaryPage _batchSummary;

                automatedBase.CurrentPage =
                    _batchSearch = automatedBase.QuickLaunch.NavigateToBatchSearch();
                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestName);
                var claSeq = paramLists["Claseq"].Split(',').ToList();
                var batchId = paramLists["BatchId"];
                var batchSeq = paramLists["BatchSeq"];
                EmailReader emailReader = null;
                try
                {

                    List<string> _activeProductListForClientDB = _batchSearch.GetActiveProductListForClientDB();
                    _batchSearch.RevertBatch(claSeq[0], claSeq[1], claSeq[2], claSeq[3], batchSeq);

                    _batchSearch.IsRoleAssigned<UserProfileSearchPage>(new List<string> { automatedBase.EnvironmentManager.Username },
                        RoleEnum.ClaimsProcessor.GetStringValue()).ShouldBeTrue(
                        $"Is  Release Claims present for current user<{automatedBase.EnvironmentManager.Username}>");

                    //CurrentPage = _profileManager = QuickLaunch.NavigateToProfileManager();
                    //_profileManager.ClickOnPrivileges();
                    //_profileManager.IsAuthorityAssigned(AuthorityAssignedEnum.ReleaseClaims.GetStringValue())
                    //    .ShouldBeTrue("HCIAdmin user should have Release Claims authority assigned");

                    StringFormatter.PrintMessage(
                        "Verify batch release icon is not present if there are no claims to be released");
                    _claimSearch = _batchSearch.NavigateToClaimSearch();
                    _claimAction =
                        _claimSearch.SearchByClaimSequenceToNavigateToClaimActionPage(claSeq[0]);
                    _claimAction.GetBatchIdInClaimDetails()
                        .ShouldBeEqual(batchId, "Batch ID should be match");
                    _claimAction.GetClaimStatus().ShouldBeEqual(ClaimStatusTypeEnum.CotivitiUnreviewed.GetStringValue(),
                        "Claim status should be Cotiviti Unreviewed");
                    _batchSearch = _claimSearch.NavigateToBatchSearch();
                    _batchSearch.SideBarPanelSearch.SetInputFieldByLabel("Batch ID", batchId);
                    _batchSearch.SideBarPanelSearch.ClickOnFindButton();
                    _batchSearch.WaitForWorking();
                    _batchSearch.GetBatchReleaseIconCount().ShouldBeEqual(0,
                        "Batch Release Icon present when there are no claims to be released");
                    _claimSearch = _batchSearch.NavigateToClaimSearch();


                    StringFormatter.PrintMessageTitle("Approving the claims");
                    ApproveClaims(_claimSearch, _claimAction, claSeq[0]);
                    ApproveClaims(_claimSearch, _claimAction, claSeq[2]);
                    ApproveClaims(_claimSearch, _claimAction, claSeq[3]);

                    StringFormatter.PrintMessage(
                        "Verify if there are claims to be released, release icon should be present");
                    _batchSearch = _claimSearch.NavigateToBatchSearch();
                    _batchSearch.SideBarPanelSearch.SetInputFieldByLabel("Batch ID", batchId);
                    _batchSearch.SideBarPanelSearch.ClickOnFindButton();
                    _batchSearch.WaitForWorking();
                    _batchSearch.GetBatchReleaseIconCount()
                        .ShouldBeEqual(1, "Batch Release Icon present when there are claims to be released?");
                    _batchSearch.GetBatchReleaseIconToolTip()
                        .ShouldBeEqual("Release Claims", "Tooltip of the batch release icon");


                    #region CAR-2209

                    StringFormatter.PrintMessage("Total unreviewed COB count before Batch Release");
                    var unreviewedCOBCounts =
                        _batchSearch.GetUnreviewedClaimsCountByProductFromDatabase(batchId, ProductEnum.COB);
                    _batchSearch.GetGridViewSection.ClickOnGridRowByRow();
                    _batchSearch.GetBatchDetailsSecondaryViewValueByLabel(AuthoritiesEnum.COB.ToString())
                        .ShouldBeEqual(unreviewedCOBCounts,
                            $"Total unreviewed COB count should be {unreviewedCOBCounts}");

                    #endregion

                    _batchSearch.ClickOnBatchReleaseIcon();
                    var t = DateTime.UtcNow;
                    var expectedreldateandusername = new List<string>
                    {
                        automatedBase.EnvironmentManager.Username + " " + _batchSearch.CurrentDateTimeInMst(t).AddMinutes(-3)
                            .ToString("MM/dd/yyyy hh:mm tt"),
                        automatedBase.EnvironmentManager.Username + " " + _batchSearch.CurrentDateTimeInMst(t).AddMinutes(-2)
                            .ToString("MM/dd/yyyy hh:mm tt"),
                        automatedBase.EnvironmentManager.Username + " " + _batchSearch.CurrentDateTimeInMst(t).AddMinutes(-1)
                            .ToString("MM/dd/yyyy hh:mm tt"),
                        automatedBase.EnvironmentManager.Username + " " +
                        _batchSearch.CurrentDateTimeInMst(t).ToString("MM/dd/yyyy hh:mm tt"),
                        automatedBase.EnvironmentManager.Username + " " + _batchSearch.CurrentDateTimeInMst(t).AddMinutes(1)
                            .ToString("MM/dd/yyyy hh:mm tt")
                    };

                    var releasedDate = _batchSearch.GetGridViewSection.GetValueInGridByColRow(3);
                    releasedDate.AssertIsContainedInList(expectedreldateandusername,
                        "Release date and user should be updated with the current logged in User and current system date and time");
                    _batchSearch.GetBatchReleaseIconCount().ShouldBeEqual(0,
                        "Batch Release Icon present when there are no claims to be released?");


                    #region CAR-2209

                    StringFormatter.PrintMessage(
                        "Verifying COB count changes after batch is released for Client users");
                    _batchSearch.Logout().LoginAsClientUser().NavigateToBatchSearch();
                    _batchSearch.SideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search", "All Batches");
                    _batchSearch.SideBarPanelSearch.SetInputFieldByLabel("Batch ID", batchId);
                    _batchSearch.SideBarPanelSearch.ClickOnFindButton();
                    _batchSearch.WaitForWorking();
                    _batchSearch.GetGridViewSection.ClickOnGridRowByRow();
                    var unreviewedCOBCountsClient =
                        _batchSearch.GetUnreviewedClaimsCountByProductByClientFromDatabase(batchId, ProductEnum.COB);
                    _batchSearch.GetBatchDetailsSecondaryViewValueByLabel(AuthoritiesEnum.COB.ToString())
                        .ShouldBeEqual(unreviewedCOBCountsClient,
                            $"COB count should be {unreviewedCOBCountsClient} after batch is released");

                    StringFormatter.PrintMessage(
                        "Verifying COB count changes after batch is released for internal users");
                    _batchSearch.Logout().LoginAsHciAdminUser().NavigateToBatchSearch();
                    _batchSearch.SideBarPanelSearch.SetInputFieldByLabel("Batch ID", batchId);
                    _batchSearch.SideBarPanelSearch.ClickOnFindButton();
                    _batchSearch.WaitForWorking();
                    _batchSearch.GetGridViewSection.ClickOnGridRowByRow();
                    unreviewedCOBCounts =
                        _batchSearch.GetUnreviewedClaimsCountByProductFromDatabase(batchId, ProductEnum.COB);
                    _batchSearch.GetBatchDetailsSecondaryViewValueByLabel(AuthoritiesEnum.COB.ToString())
                        .ShouldBeEqual(unreviewedCOBCounts,
                            $"COB count should be {unreviewedCOBCounts} after batch is released");


                    #endregion

                    //TE-382
                    StringFormatter.PrintMessage("Verify Processing History Shows Most Recent Batch Release");
                    _batchSummary = _batchSearch.ClickOnBatchIdAndNavigateToBatchSummaryPage(batchId);
                    _batchSummary.GetValueInProcessingHistoryByRow().ShouldBeEqual(automatedBase.EnvironmentManager.Username,
                        "Release user should be updated with the current logged in user");
                    var releaseDateInProcessingHistory = DateTime.ParseExact(
                        _batchSummary.GetValueInProcessingHistoryByRow(1, 2), "MM/dd/yyyy hh:mm:ss tt",
                        CultureInfo.InvariantCulture);
                    releaseDateInProcessingHistory.AssertDateRange(
                        _batchSearch.CurrentDateTimeInMst(t).AddMinutes(-2).AddSeconds(-3),
                        _batchSearch.CurrentDateTimeInMst(t).AddMinutes(1),
                        "Release Date in processing history should be within the current time.");
                    _claimSearch = _batchSearch.NavigateToClaimSearch();

                    #region CAR-2209

                    StringFormatter.PrintMessage(
                        "Verification that flag will be set to Client Unreviewed when client review flag = 'Y' for top flags");
                    _claimAction =
                        _claimSearch.SearchByClaimSequenceToNavigateToClaimActionPage(claSeq[0], true);
                    _claimAction.IsGetClientReviewFlagTrueOrFalse(claSeq[0])
                        .ShouldBeTrue("For client review flag = Y, status should change to Client Unreviewed");
                    _claimAction.GetClaimStatus().ShouldBeEqual(ClaimStatusTypeEnum.ClientUnreviewed.GetStringValue(),
                        "Claim status should be Client Unreviewed");

                    StringFormatter.PrintMessage(
                        "Verification that flag will be set to Client Unreviewed for multiple client review flag values for top flags");
                    _claimAction.ClickClaimSearchIcon()
                        .SearchByClaimSequenceToNavigateToClaimActionPage(claSeq[2], true);
                    _claimAction.IsGetClientReviewFlagTrueOrFalse(claSeq[2])
                        .ShouldBeFalse(
                            "For multiple client review flag values for top flag, status should change to Client Unreviewed");
                    _claimAction.GetClaimStatus().ShouldBeEqual(ClaimStatusTypeEnum.ClientUnreviewed.GetStringValue(),
                        "Claim status should be Client Unreviewed");


                    StringFormatter.PrintMessage(
                        "Verification that flag will be set to Client Reviewed for client review flag 'N' for top flags");
                    _claimAction =
                        _claimSearch.SearchByClaimSequenceToNavigateToClaimActionPage(claSeq[3], true);
                    _claimAction.IsGetClientReviewFlagTrueOrFalse(claSeq[3], false)
                        .ShouldBeTrue("For client review flag N, status should change to Client Reviewed");
                    _claimAction.GetClaimStatus().ShouldBeEqual(ClaimStatusTypeEnum.ClientReviewed.GetStringValue(),
                        "Claim status should be Client Reviewed");

                    #endregion

                    StringFormatter.PrintMessage(
                        "Verification of presence of batch release icon on approving another claim");
                    _claimAction.ClickClaimSearchIcon()
                        .SearchByClaimSequenceToNavigateToClaimActionPage(claSeq[1], true);
                    _claimAction.GetBatchIdInClaimDetails()
                        .ShouldBeEqual(batchId, "Batch ID should be match");
                    _claimAction.GetClaimStatus().ShouldBeEqual(ClaimStatusTypeEnum.CotivitiUnreviewed.GetStringValue(),
                        "Claim status should be Cotiviti Unreviewed");

                    _claimAction.ClickOnApproveButton();

                    _claimSearch.GetGridViewSection.DoesGridListValueByColHasValue("Cotiviti Reviewed", 8);

                    _batchSearch = _claimSearch.NavigateToBatchSearch();


                    _batchSearch.SideBarPanelSearch.SetInputFieldByLabel("Batch ID", batchId);
                    _batchSearch.SideBarPanelSearch.ClickOnFindButton();
                    _batchSearch.WaitForWorking();
                    _batchSearch.GetBatchReleaseIconCount()
                        .ShouldBeEqual(1, "Batch Release Icon present when there are claims to be released");
                    _batchSearch.ClickOnBatchReleaseIcon();
                    _batchSearch.GetBatchReleaseIconCount().ShouldBeEqual(0,
                        "Batch Release Icon is not present when there are no claims to be released");
                    t = DateTime.UtcNow;
                    var expectedreldateandusername1 = new List<string>
                    {
                        automatedBase.EnvironmentManager.Username + " " + _batchSearch.CurrentDateTimeInMst(t).AddMinutes(-3)
                            .ToString("MM/dd/yyyy hh:mm tt"),
                        automatedBase.EnvironmentManager.Username + " " + _batchSearch.CurrentDateTimeInMst(t).AddMinutes(-2)
                            .ToString("MM/dd/yyyy hh:mm tt"),
                        automatedBase.EnvironmentManager.Username + " " + _batchSearch.CurrentDateTimeInMst(t).AddMinutes(-1)
                            .ToString("MM/dd/yyyy hh:mm tt"),
                        automatedBase.EnvironmentManager.Username + " " +
                        _batchSearch.CurrentDateTimeInMst(t).ToString("MM/dd/yyyy hh:mm tt"),
                        automatedBase.EnvironmentManager.Username + " " + _batchSearch.CurrentDateTimeInMst(t).AddMinutes(1)
                            .ToString("MM/dd/yyyy hh:mm tt")
                    };
                    var releasedDate1 = _batchSearch.GetGridViewSection.GetValueInGridByColRow(3);
                    releasedDate1.AssertIsContainedInList(expectedreldateandusername1,
                        "Release date and user should be updated with the current logged in User and current system date and time");
                    _batchSearch.GetGridViewSection.ClickOnGridByRowCol();
                    VerifyCheckboxAgainstProductLabel(_batchSearch, batchId, _activeProductListForClientDB);
                    //var completeDate = DateTime.ParseExact(_batchSearch.GetGridViewSection.GetValueInGridByColRow(12), "MM/dd/yyyy hh:mm tt", CultureInfo.InvariantCulture);
                    var completeDate = _batchSearch.GetGridViewSection.GetValueInGridByColRow(4);
                    var expectedcompletedateandtime = new List<string>
                    {
                        _batchSearch.CurrentDateTimeInMst(t).AddMinutes(-3).ToString("MM/dd/yyyy hh:mm tt"),
                        _batchSearch.CurrentDateTimeInMst(t).AddMinutes(-2).ToString("MM/dd/yyyy hh:mm tt"),
                        _batchSearch.CurrentDateTimeInMst(t).AddMinutes(-1).ToString("MM/dd/yyyy hh:mm tt"),
                        _batchSearch.CurrentDateTimeInMst(t).ToString("MM/dd/yyyy hh:mm tt")
                    };
                    completeDate.AssertIsContainedInList(expectedcompletedateandtime,
                        "Completed date equals to the current system date and time?");
                    _batchSearch.GetRelToClientFromDatabase(batchId).ShouldBeEqual("T",
                        "Complete date populates when there are no more claims to be released");
                    //t = DateTime.UtcNow;
                    //completeDate.AssertDateRange(_batchSearch.CurrentDateTimeInMst(t).AddMinutes(-1), _batchSearch.CurrentDateTimeInMst(t).AddMinutes(1), "Completed date equals to the current system date and time?");

                    //TE-524
                    StringFormatter.PrintMessage("Verify Email Is sent when batch is released");
                    emailReader = new EmailReader();
                    emailReader.AuthenticateUser(paramLists["Email"], ConfigurationManager.AppSettings["MailPassword"]);
                    emailReader.SelectInbox();
                    int latestMailId = emailReader.GetMailCount();
                    var mailHeader = emailReader.GetMailHeader(latestMailId);
                    WriteLine(mailHeader);
                    Regex.Match(mailHeader, paramLists["Subject"]).Success.ShouldBeTrue("Subject Should Match");

                    mailHeader.Contains(paramLists["EmailFrom"])
                        .ShouldBeTrue("Email From Should Match");

                    string mailDate = Regex.Match(mailHeader, "\\d+\\s\\w+\\s\\w{4}").ToString();
                    mailDate.ShouldBeEqual(_batchSearch.CurrentDateTimeInMst(t).ToString("d MMM yyyy"),
                        "Received Date Should Match");

                }
                finally
                {
                    emailReader?.CloseConnection();
                }

            }
        }

        [Test, Category("Working")] //TE-66
        public void Verify_batch_release_icon_for_user_without_release_claim_authority()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                BatchSearchPage _batchSearch;

                automatedBase.CurrentPage =
                    _batchSearch = automatedBase.QuickLaunch.NavigateToBatchSearch();
                _batchSearch
                    .IsRoleAssigned<UserProfileSearchPage>(
                        new List<string> { automatedBase.EnvironmentManager.HciUserWithNoManageEdit },
                        RoleEnum.ClaimsProcessor.GetStringValue()).ShouldBeFalse(
                        $"Is CV Coder Work List present for current user<{automatedBase.EnvironmentManager.HciUserWithNoManageEdit}>");

                automatedBase.CurrentPage.Logout().LoginAsUserHavingNoManageEditAuthority();
                _batchSearch.NavigateToBatchSearch();
                _batchSearch.GetBatchReleaseIconCount()
                    .ShouldBeEqual(0, "Is Batch Release Icon present for users with no authority");
            }
        }

        [Test] //CAR-728
        public void Verify_find_button_is_disabled_when_search_is_active()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                BatchSearchPage _batchSearch;

                automatedBase.CurrentPage =
                    _batchSearch = automatedBase.QuickLaunch.NavigateToBatchSearch();
                _batchSearch.SideBarPanelSearch.SetInputFieldByLabel("Quick Search", "Incomplete Batches");
                _batchSearch.ClickFindAndCheckIfFindButtonIsDisabled()
                    .ShouldBeTrue("Find Button Should be disabled while the search is active.");
                _batchSearch.WaitForWorkingAjaxMessage();
                _batchSearch.GetGridViewSection.GetGridRowCount()
                    .ShouldBeGreater(0, "Search results should be displayed");
                _batchSearch.CheckIfFindButtonIsEnabled()
                    .ShouldBeTrue("Find Button Should be enabled once the search is complete.");
            }
        }

        [Test, Category("SmokeTestDeployment")] //TANT-99
        public void Verify_Batch_Search_Page_and_presence_of_data()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                BatchSearchPage _batchSearch;

                automatedBase.CurrentPage =
                    _batchSearch = automatedBase.QuickLaunch.NavigateToBatchSearch();
                _batchSearch.GetGridViewSection.GetGridRowCount().ShouldBeGreaterOrEqual
                    (0, "Default search should the most recent active batches");
            }
        }

        [Test] //TE-508
        public void Verify_Sorting_Options_In_Batch_Search()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                BatchSearchPage _batchSearch;

                automatedBase.CurrentPage =
                    _batchSearch = automatedBase.QuickLaunch.NavigateToBatchSearch();
                var TestName = new StackFrame(true).GetMethod().Name;
                var filterOptions =
                    automatedBase.DataHelper.GetMappingData(FullyQualifiedClassName, "Batch_Sorting_options").Values.ToList();
                try
                {
                    _batchSearch.GetGridViewSection.IsFilterOptionIconPresent()
                        .ShouldBeTrue("Filter Icon Option Should Be Present");
                    _batchSearch.GetGridViewSection.GetFilterOptionTooltip()
                        .ShouldBeEqual("Sort Results", "Correct tooltip is displayed");
                    _batchSearch.GetFilterOptionList()
                        .ShouldCollectionBeEqual(filterOptions, "Filter Options Lists Collection Should Equal");
                    _batchSearch.SideBarPanelSearch.ClickOnFindButton();
                    _batchSearch.WaitForWorkingAjaxMessage();
                    StringFormatter.PrintMessage("Verify sort using Batch Id");
                    VerifySortingOptionsInBatchSearch(_batchSearch, 2, 1, filterOptions[0]);
                    StringFormatter.PrintMessage("Verify sort using Batch Date");
                    VerifySortingOptionsInBatchSearch(_batchSearch, 5, 2, filterOptions[1], true);
                }
                finally
                {
                    _batchSearch.ClickOnClearSort();
                    _batchSearch.GetGridViewSection.GetGridListValueByCol(16).IsInDescendingOrder()
                        .ShouldBeTrue("Is default sort applied after clear sort ?");
                }
            }
        }



        #endregion

        #region PRIVATE

        private void ValidateDefaultValuesOfFiltersExceptQuickSearchFilter(BatchSearchPage _batchSearch)
        {
            _batchSearch.SideBarPanelSearch.GetInputValueByLabel("Batch ID")
                .ShouldBeNullorEmpty("Batch ID should be empty");
            _batchSearch.SideBarPanelSearch.GetDateFieldFrom("Client Create Date")
                .ShouldBeNullorEmpty("Default value of Client Create Date");
            _batchSearch.SideBarPanelSearch.GetDateFieldTo("Client Create Date")
                .ShouldBeNullorEmpty("Default value of Client Create Date");
            _batchSearch.SideBarPanelSearch.GetDateFieldFrom("Cotiviti Create Date")
                .ShouldBeNullorEmpty("Default Value of Cotiviti Create Date");
            _batchSearch.SideBarPanelSearch.GetDateFieldTo("Cotiviti Create Date")
                .ShouldBeNullorEmpty("Default Value of Cotiviti Create Date");
            _batchSearch.SideBarPanelSearch.GetDateFieldPlaceholder("Client Create Date", 1)
                .ShouldBeEqual("00/00/0000", "Date range picker for Client Create Date (from) default placeholder value");
            _batchSearch.SideBarPanelSearch.GetDateFieldPlaceholder("Client Create Date", 2)
                .ShouldBeEqual("00/00/0000", "Date range picker forClient Create Date (to) default placeholder value");
            _batchSearch.SideBarPanelSearch.GetDateFieldPlaceholder("Cotiviti Create Date", 1)
                .ShouldBeEqual("00/00/0000", "Date range picker for Cotiviti Create Date (from) default placeholder value");
            _batchSearch.SideBarPanelSearch.GetDateFieldPlaceholder("Cotiviti Create Date", 2)
                .ShouldBeEqual("00/00/0000", "Date range picker for Cotiviti Create Date (to) default placeholder value");

        }

        public void ValidateDatePickerField(BatchSearchPage _batchSearch, string label, string message1, string message2, string message3)
        {
            _batchSearch.SideBarPanelSearch.ClickOnClearLink();

            _batchSearch.SideBarPanelSearch.SetDateFieldTo(label, DateTime.Now.AddDays(1).ToString("MM/d/yyyy"));
            _batchSearch.SideBarPanelSearch.GetFieldErrorIconTooltipMessage(label)
                .ShouldBeEqual(
                    message3,
                    "Field Error Tooltip Message When Date From is empty");
            _batchSearch.IsInvalidInputPresentByLabel(label).ShouldBeTrue($"{label} should be highlighted red for invalid input");
            _batchSearch.GetCountOfInvalidRed().ShouldBeEqual(2,
                $"Both to and from fields of {label} should be highlighted red for invalid input");

            _batchSearch.SideBarPanelSearch.SetDateFieldFrom(label, DateTime.Now.AddDays(1).ToString("MM/d/yyyy"));
            _batchSearch.SideBarPanelSearch.SetDateField(label, "", 2);
            _batchSearch.SideBarPanelSearch.GetFieldErrorIconTooltipMessage(label)
                .ShouldBeEqual(
                    message3,
                    "Field Error Tooltip Message When Date To is empty");
            _batchSearch.IsInvalidInputPresentByLabel(label).ShouldBeTrue($"{label} should be highlighted red for invalid input");
            _batchSearch.GetCountOfInvalidRed().ShouldBeEqual(2,
                $"Both to and from fields of {label} should be highlighted red for invalid input");


            _batchSearch.SideBarPanelSearch.SetInputFieldByLabel(label, DateTime.Now.AddDays(2).ToString("MM/d/yyyy"), sendTabKey: true); //check numeric value can be typed
            _batchSearch.SideBarPanelSearch.GetDateFieldFrom(label).ShouldBeEqual(DateTime.Now.AddDays(2).ToString("MM/dd/yyyy"), label + " Checks numeric value is accepted");

            _batchSearch.SideBarPanelSearch.SetDateFieldFrom(label, DateTime.Now.ToString("MM/d/yyyy"));
            _batchSearch.SideBarPanelSearch.GetDateFieldTo(label).
                ShouldBeEqual(_batchSearch.SideBarPanelSearch.GetDateFieldFrom(label), label + " From value populated in To field as well.");

            _batchSearch.SideBarPanelSearch.SetDateFieldTo(label, DateTime.Now.Subtract(new TimeSpan(24, 0, 0)).ToString("MM/d/yyyy"));
            _batchSearch.GetPageErrorMessage().ShouldBeEqual("Please enter a valid date range.");
            _batchSearch.ClosePageError();
            _batchSearch.SideBarPanelSearch.ClickOnHeader();


            _batchSearch.SideBarPanelSearch.SetDateFieldTo(label, DateTime.Now.AddMonths(3).AddDays(3).ToString("MM/d/yyyy"));
            _batchSearch.SideBarPanelSearch.GetFieldErrorIconTooltipMessage(label)
                .ShouldBeEqual(
                    message1,
                    string.Format("Field Error Tooltip Message When <{0}> range greater than 3 months", label));
            _batchSearch.IsInvalidInputPresentByLabel(label).ShouldBeTrue($"{label} should be highlighted red for invalid input");
            _batchSearch.GetCountOfInvalidRed().ShouldBeEqual(2,
                $"Both to and from fields of {label} should be highlighted red for invalid input");
            _batchSearch.SideBarPanelSearch.ClickOnFindButton();
            _batchSearch.GetPageErrorMessage().ShouldBeEqual(message2, "Verification of popup message for invalid date");
            _batchSearch.ClosePageError();
            _batchSearch.SideBarPanelSearch.ClickOnHeader();

        }



        private void VerifyCheckboxAgainstProductLabel(BatchSearchPage _batchSearch, string batchid, List<string> activeProducts)
        {

            for (int i = 0; i < activeProducts.Count; i++)
            {
                var product = ProductEnum.CV;
                switch (activeProducts[i])
                {
                    case "CV":
                        product = ProductEnum.CV;
                        break;
                    case "FFP":
                        product = ProductEnum.FFP;
                        break;
                    case "FCI":
                        product = ProductEnum.FCI;
                        break;
                    case "DCA":
                        product = ProductEnum.DCA;
                        break;
                    case "COB":
                        product = ProductEnum.COB;
                        break;

                }
                var unreviewedClaimsCountByProductFromDatabase = _batchSearch.GetUnreviewedClaimsCountByProductFromDatabase(batchid, product);
                _batchSearch.GetBatchDetailsSecondaryViewValueByLabel(activeProducts[i]).ShouldBeEqual(unreviewedClaimsCountByProductFromDatabase, "Is Count Equals?");

            }

        }

        private void VerifyIfCheckBoxIsNotPresentThenReleaseIconShouldBePresent()
        {

        }

        private void ValidateQuickSearchDropDownForDefaultValueAndExpectedList(BatchSearchPage _batchSearch, string label, IList<string> collectionToEqual)
        {
            var actualDropDownList = _batchSearch.SideBarPanelSearch.GetAvailableDropDownList(label);
            actualDropDownList.Count.ShouldBeGreater(0, "List of " + label + " is greater than zero.");
            actualDropDownList.RemoveAll(IsNullOrEmpty);
            if (collectionToEqual != null)
                actualDropDownList.ShouldCollectionBeEqual(collectionToEqual, label + " List As Expected");
            _batchSearch.SideBarPanelSearch.GetInputValueByLabel(label)
                .ShouldBeEqual(BatchQuickSearchTypeEnum.IncompleteBatches.GetStringValue(), label + " value defaults to Incomplete Batches");
            _batchSearch.SideBarPanelSearch.SelectDropDownListValueByLabel(label, actualDropDownList[0],
                false); //check for type ahead functionality
            _batchSearch.SideBarPanelSearch.SelectDropDownListValueByLabel(label, actualDropDownList[1]);
            _batchSearch.SideBarPanelSearch.GetInputValueByLabel(label)
                .ShouldBeEqual(actualDropDownList[1], "User can select only a single option");

        }

        private void Verify_the_Batch_list_with_database(BatchSearchPage _batchSearch, string quickSearchMappingOption, List<string> batchList, bool search = true)
        {
            var batchCount = batchList == null ? 0 : batchList.Count;

            if (search)
            {
                _batchSearch.SideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                    quickSearchMappingOption, false);
                _batchSearch.SideBarPanelSearch.ClickOnFindButton();
                _batchSearch.WaitForWorking();
                _batchSearch.IsPageErrorPopupModalPresent().ShouldBeFalse("Search should be initated with quick search option " + quickSearchMappingOption + " only.");
            }
            if (batchCount > 25)
            {
                _batchSearch.ClickOnLoadMore();
                var loadMoreValue = _batchSearch.GetLoadMoreText();
                var numbers = Regex.Split(loadMoreValue, @"\D+").Where(s => s != String.Empty)
                    .Select(m => int.Parse(m.Trim())).ToList();
                numbers[1].ShouldBeEqual(batchCount, quickSearchMappingOption + "batch count match");
                if (Enumerable.Range(1, 49).Contains(numbers[0]))
                {
                    numbers[0].ShouldBeEqual
                    (batchCount,
                        "For " + quickSearchMappingOption +
                        "count less than 50, clicking on Load more should equal the " + quickSearchMappingOption +
                        " count with the database");
                }
                else
                {
                    numbers[0].ShouldBeEqual(50, "The value should equal to 50");
                }
            }
            else
            {
                var rowCount = _batchSearch.GetGridViewSection.GetGridRowCount();
                if (_batchSearch.SideBarPanelSearch.IsNoMatchingRecordFoundMessagePresent())
                {
                    rowCount = 0;
                }
                rowCount.ShouldBeEqual(batchCount, "Batch Counts Should Match");

            }
            if (batchList != null)
                _batchSearch.GetGridViewSection.GetGridListValueByCol(2)
                    .ShouldCollectionBeEqual(batchList.Take(50), quickSearchMappingOption + " list should be equal and list should be sorted by Client Created Date in descending");

        }

        private void VerifySortingOptionsInBatchSearch(BatchSearchPage _batchSearch, int col, int option, string sortoption, bool isDate = false)
        {
            _batchSearch.ClickOnFilterOptionListRow(option);
            if (isDate)
                _batchSearch.GetGridViewSection.GetGridListValueByCol(col).Select(DateTime.Parse).ToList().IsInAscendingOrder()
                    .ShouldBeTrue($"Search result must be sorted by {sortoption} in Ascending");
            else
                _batchSearch.GetGridViewSection.GetGridListValueByCol(col).IsInAscendingOrder()
                .ShouldBeTrue($"Search result must be sorted by {sortoption} in Ascending");
            _batchSearch.ClickOnFilterOptionListRow(option);
            if (isDate)
                _batchSearch.GetGridViewSection.GetGridListValueByCol(col).Select(DateTime.Parse).ToList().IsInDescendingOrder()
                    .ShouldBeTrue($"Search result must be sorted by {sortoption} in Ascending");
            else
                _batchSearch.GetGridViewSection.GetGridListValueByCol(col).IsInDescendingOrder()
                .ShouldBeTrue($"Search result must be sorted by {sortoption} in Descending");
        }

        private void ApproveClaims(ClaimSearchPage _claimSearch, ClaimActionPage _claimAction, string claSeq)
        {
            _claimSearch.SearchByClaimSequenceToNavigateToClaimActionPage(claSeq);
            _claimAction.GetClaimStatus().ShouldBeEqual(ClaimStatusTypeEnum.CotivitiUnreviewed.GetStringValue(),
                "Claim status should be Cotiviti Unreviewed");
            _claimAction.ClickOnApproveButton();
            _claimAction.WaitForCondition(() => _claimSearch.GetPageHeader().Equals(PageHeaderEnum.ClaimSearch.GetStringValue()), 1000);
            _claimAction.WaitForStaticTime(200);
            _claimAction.GetCommonSql.GetClaimStatusFromDatabase(claSeq).ShouldBeEqual("R", "Status should be reviewed");
            _claimAction.GetCommonSql.GetReleaseToClientStatusFromDatabase(claSeq).ShouldBeEqual("F", "Claim should not be released to client");
            //_claimSearch.GetGridViewSection
            //    .DoesGridListValueByColHasValue(ClaimStatusTypeEnum.CotivitiReviewed.GetStringValue(), 8)
            //    .ShouldBeTrue("The claim should be Cotiviti Reviewed", true);
        }



        #endregion



    }
}

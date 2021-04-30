using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Nucleus.Service.Data;
using Nucleus.Service.PageServices.Claim;
using Nucleus.Service.Support.Common.Constants;
using Nucleus.Service.Support.Environment;
using Nucleus.UIAutomation.TestSuites.Base;
using NUnit.Framework;
using Nucleus.Service.Support.Enum;
using UIAutomation.Framework.Utils;
using System.Text.RegularExpressions;

using Nucleus.Service.PageServices.QuickLaunch;
using Nucleus.Service.PageServices.Settings;
using Nucleus.Service.PageServices.Settings.User;
using Nucleus.Service.Support.Menu;
using UIAutomation.Framework.Core.Driver;
using Extensions = Nucleus.Service.Support.Utils.Extensions;
using static System.String;

namespace Nucleus.UIAutomation.TestSuites.TestCases
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class ClaimSearch
    {
        //#region PRIVATE FIELDS

        //private ClaimSearchPage _claimSearch;
        //private ProfileManagerPage _newProfileManager;
        //private ClientSearchPage _clientSearch;
        //private List<string> _smtstPlanList;
        //private List<string> _cvtyPlanList;
        //private List<string> _smtstSubStatusList;
        //private List<string> _cvtySubStatusList;
        //private List<string> _claimsAssignedToUserList;
        //private List<string> _cvtyClaimsAssignedToUserList;
        //private List<string> _lineOfBusinessList;
        //private List<string> _smtstReviewGroup;
        //private List<string> _cvtyReviewGroup;
        //private List<string> _activeProductListForClient;
        //private List<string> _allProductListFromDB;
        //private Int16 _unreviewedClaimsCountInternalUser;
        //private Int16 _pendedClaimsCount;
        //private Int16 _flaggedClaimsCountInternalUser;
        //private List<string> _cvtyBatchList;
        //private List<string> _smtstBatchList;
       

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

        //#region OVERRIDE METHODS

        //protected override void ClassInit()
        //{
        //    try
        //    {
        //        base.ClassInit();
        //        //QuickLaunch.ClickOnSwitchClient().SwitchClientTo(ClientEnum.CVTY);
        //        CurrentPage =  _claimSearch = QuickLaunch.NavigateToClaimSearch();

        //     try
        //        {
        //            RetrieveListFromDatabase();
        //        }
        //        catch (Exception e)
        //        {
        //            Console.WriteLine(e.Message);
        //        }
                
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
        //    base.TestCleanUp();
        //    if ( Compare(UserType.CurrentUserType, UserType.HCIADMIN, StringComparison.OrdinalIgnoreCase) != 0)
        //    {
        //        _claimSearch = CurrentPage.ClickOnQuickLaunch()
        //            .Logout()
        //            .LoginAsHciAdminUser().ClickOnSwitchClient().SwitchClientTo(automatedBase.EnvironmentManager.TestClient)
        //            .NavigateToClaimSearch();
        //    }
        //    else
        //    {
        //         if (!CurrentPage.IsCurrentClientAsExpected(automatedBase.EnvironmentManager.TestClient))
        //        {
        //            CheckTestClientAndSwitch();
        //            _claimSearch.NavigateToClaimSearch();
        //        }
        //        else
        //         {
        //             _claimSearch.GetSideBarPanelSearch.OpenSidebarPanel();
        //            _claimSearch.GetSideBarPanelSearch.ClickOnClearLink();
        //            if(_claimSearch.GetSideBarPanelSearch.GetTopHeaderName() != "Find Claim")
        //                _claimSearch.GetSideBarPanelSearch.ClickOnFilterIcon();
        //            _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
        //                ClaimQuickSearchTypeEnum.AllClaims.GetStringValue());
        //        }
        //    }
        //}

        //protected override void ClassCleanUp()
        //{
        //    try
        //    {
        //        _claimSearch.CloseDbConnection();
                
        //    }

        //    finally
        //    {
        //        base.ClassCleanUp();
        //    }
        //}

        //#endregion

        //#region DBinteraction methods
        //private void RetrieveListFromDatabase()
        //{

        //    _smtstSubStatusList = _claimSearch.GetAssociatedClaimSubStatusForInternalUser(ClientEnum.SMTST.ToString());
        //    _cvtySubStatusList = _claimSearch.GetAssociatedClaimSubStatusForInternalUser(ClientEnum.CVTY.GetStringDisplayValue());
        //    _smtstPlanList = _claimSearch.GetAssociatedPlansForClient(ClientEnum.SMTST.ToString());
        //    _cvtyPlanList = _claimSearch.GetAssociatedPlansForClient(ClientEnum.CVTY.GetStringDisplayValue());
        //    _claimsAssignedToUserList = _claimSearch.GetAssignedToList(ClientEnum.SMTST.ToString());
        //    _cvtyClaimsAssignedToUserList= _claimSearch.GetAssignedToList(ClientEnum.CVTY.GetStringDisplayValue());
        //    _lineOfBusinessList = _claimSearch.GetCommonSql.GetLineOfBusiness();
        //    _smtstReviewGroup = _claimSearch.GetReviewGroup(ClientEnum.SMTST.ToString());
        //    _cvtyReviewGroup = _claimSearch.GetReviewGroup(ClientEnum.SMTST.ToString());
        //    _activeProductListForClient = (List<string>)_claimSearch.GetActiveProductListForClient(ClientEnum.SMTST.ToString());
        //    _allProductListFromDB = _claimSearch.GetCommonSql.GetAllProductList();
        //    _unreviewedClaimsCountInternalUser = Convert.ToInt16(_claimSearch.GetUnreviewedClaimListForInternalUser(ClientEnum.SMTST.ToString(), automatedBase.EnvironmentManager.HciAdminUsername)[0]);
        //    _pendedClaimsCount = Convert.ToInt16(_claimSearch.GetPendedClaimsCount(ClientEnum.SMTST.ToString(), automatedBase.EnvironmentManager.HciAdminUsername)[0]);
        //    //_pendedClaimsCount = Convert.ToInt16(_claimSearch.GetPendedClaimsCount(ClientEnum.SMTST.ToString())[0]);
        //    _flaggedClaimsCountInternalUser = Convert.ToInt16(_claimSearch.GetFlaggedClaimList(ClientEnum.SMTST.ToString())[0]);
        //    _smtstBatchList = _claimSearch.GetAssociatedBatchList(ClientEnum.SMTST.ToString());
        //    _smtstBatchList.Insert(0, "All");
        //    _cvtyBatchList = _claimSearch.GetAssociatedBatchList(ClientEnum.CVTY.GetStringDisplayValue());
        //    _cvtyBatchList.Insert(0, "All");
        //}



        //#endregion

        #region TEST SUITES

        [Test] //TANT-304
        public void Verify_basic_search_filters_for_all_claims()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ClaimSearchPage _claimSearch;
                _claimSearch = automatedBase.CurrentPage.NavigateToClaimSearch();
                _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search", "All Claims");
                _claimSearch.GetSideBarPanelSearch.GetSearchFiltersList()
                    .ShouldCollectionBeEqual(
                        automatedBase.DataHelper.GetMappingData(FullyQualifiedClassName, "claim_search_all_claims").Values.ToList(), "Search Filters",
                        true);
            }
        }

        [Test] // CAR-3145(CAR-3051)
        [NonParallelizable]
        public void Verification_of_Restriction_filter_for_Unreviewed_and_Outstanding_Claims()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ClaimSearchPage _claimSearch;
                string filter = "Restrictions";
                _claimSearch = automatedBase.CurrentPage.NavigateToClaimSearch();

                var assignedRestrictionList = _claimSearch.GetCommonSql.GetAssignedRestrictionDescriptionsForAUserFromDb(automatedBase.EnvironmentManager.Username);
                StringFormatter.PrintMessage("Verification of restriction filter for Unreviewed Claims");
                var assignedRestrictionClaimListForUnreviewedClaims =
                    _claimSearch.GetResultsForSpecificRestrictionFilterForUnreviewedClaimsDb(automatedBase.EnvironmentManager.Username);

                var assignedRestrictionClaimListForOutstandingClaims = 
                    _claimSearch.GetResultsForSpecificRestrictionFilterForOutstandingQADb(automatedBase.EnvironmentManager.Username);
                
                _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search", ClaimQuickSearchTypeEnum.UnreviewedClaims.GetStringValue());
                _claimSearch.GetSideBarPanelSearch.IsSearchInputFieldDisplayedByLabel(filter).ShouldBeTrue("Is Restrictions filter present?");

                StringFormatter.PrintMessage("Verifying if the assigned restrictions match for Unreviewed Claims");
                _claimSearch.GetSideBarPanelSearch
                    .GetAvailableDropDownList(filter).Skip(2)
                    .ShouldCollectionBeEqual(assignedRestrictionList, "Do assigned restrictions match?");
                List<string> defaultValues = _claimSearch.GetSideBarPanelSearch
                    .GetAvailableDropDownList(filter).Take(2).ToList();

                StringFormatter.PrintMessage("Verification for Unreviewed Claims with All restrictions");
                _claimSearch.GetSideBarPanelSearch.ClickOnFindButton();
                _claimSearch.WaitForWorking();
                var loadMoreValue = _claimSearch.GetLoadMoreText();
                var numbers = Regex.Split(loadMoreValue, @"\D+").Where(s => s != Empty).Select(m => int.Parse(m.Trim()))
                    .ToList();
                numbers[1].ShouldBeEqual(Convert.ToInt16(
                        _claimSearch.GetUnreviewedClaimListForInternalUser(ClientEnum.SMTST.ToString(), automatedBase.EnvironmentManager
                            .Username)[0]),
                        "Is Unreviewed Claim count for ALL restriction equal?");

                StringFormatter.PrintMessage("Verification for Unreviewed Claims with No restrictions");
                _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel(filter, defaultValues[1]);
                _claimSearch.GetSideBarPanelSearch.ClickOnFindButton();
                _claimSearch.WaitForWorking();
                loadMoreValue = _claimSearch.GetLoadMoreText();
                numbers = Regex.Split(loadMoreValue, @"\D+").Where(s => s != Empty).Select(m => int.Parse(m.Trim()))
                    .ToList();
                numbers[1].ShouldBeEqual(Convert.ToInt16(
                        _claimSearch.GetCountOfUnreviewedClaimsForNoRestrictionDb(automatedBase.EnvironmentManager
                            .Username)),
                    "Is Unreviewed Claim count for No restriction equal?");


                StringFormatter.PrintMessage("Verification for Unreviewed Claims for assigned restrictions");
                for (int i = 0; i < assignedRestrictionList.Count; i++)
                {
                    StringFormatter.PrintMessage($"Verifying for {assignedRestrictionList[i]} restriction");
                    SearchByRestrictionAndCheckValue(filter, assignedRestrictionList[i],
                        assignedRestrictionClaimListForUnreviewedClaims[i]);
                }

                StringFormatter.PrintMessage("Verification of restriction filter for Outstanding QA Claims");
                _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search", ClaimQuickSearchTypeEnum.OutstandingQCClaims.GetStringValue());
                _claimSearch.GetSideBarPanelSearch.IsSearchInputFieldDisplayedByLabel(filter).ShouldBeTrue("Is Restrictions filter present?");
                _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel(filter).ShouldBeEqual("All","Default value of Restrictions filter should be All");

                StringFormatter.PrintMessage("Verifying for Outstanding QA Claims filter with All restrictions");

                SearchByRestrictionAndCheckValue(filter, defaultValues[0],
                    _claimSearch.GetResultsForAllRestrictionFilterForOutstandingQADb(automatedBase.EnvironmentManager.Username));

                StringFormatter.PrintMessage("Verifying for  Outstanding QA Claims filter with No restrictions");
                SearchByRestrictionAndCheckValue(filter, defaultValues[1],
                    _claimSearch.GetResultsForNoRestrictionFilterForOutstandingQADb(automatedBase.EnvironmentManager.Username));

                StringFormatter.PrintMessage("Verifying for  Outstanding QA Claims filter with assigned restrictions");
                for (int i = 0; i < assignedRestrictionList.Count; i++)
                {
                    StringFormatter.PrintMessage($"Verifying for {assignedRestrictionList[i]} restriction");
                    SearchByRestrictionAndCheckValue(filter, assignedRestrictionList[i],
                        assignedRestrictionClaimListForOutstandingClaims[i]);
                }

                StringFormatter.PrintMessage(
                    "Verify if the user has no restriction available then only default values are displayed in the restriction list");
                _claimSearch.Logout().LoginAsClaimViewRestrictionUser();
                _claimSearch.GetCommonSql.
                    GetAssignedRestrictionDescriptionsForAUserFromDb(automatedBase.EnvironmentManager.HciClaimViewRestrictionUsername).
                    ShouldBeNull("Restriction should not be assigned to this user");
                _claimSearch = automatedBase.CurrentPage.NavigateToClaimSearch();
                _claimSearch.GetSideBarPanelSearch
                    .SelectDropDownListValueByLabel("Quick Search", ClaimQuickSearchTypeEnum.UnreviewedClaims.GetStringValue());
                _claimSearch.GetSideBarPanelSearch.GetAvailableDropDownList(filter)
                    .ShouldBeEqual(defaultValues,
                        "Only default values available when no restriction is assigned ?");

                _claimSearch.GetSideBarPanelSearch
                    .SelectDropDownListValueByLabel("Quick Search", ClaimQuickSearchTypeEnum.OutstandingQCClaims.GetStringValue());
                _claimSearch.GetSideBarPanelSearch.GetAvailableDropDownList(filter)
                    .ShouldBeEqual(defaultValues,
                        "Only default values available when no restriction is assigned ?");

                #region Local Method

                void SearchByRestrictionAndCheckValue(string label, string value, List<string> valueToCompare)
                {
                    if(!_claimSearch.GetSideBarPanelSearch.IsSideBarPanelOpen())
                        _claimSearch.GetSideBarPanelSearch.OpenSidebarPanel();
                    _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel(label, value);
                    _claimSearch.GetSideBarPanelSearch.ClickOnFindButton();
                    _claimSearch.WaitForWorking();
                    if (automatedBase.CurrentPage.GetPageHeader().Equals(PageHeaderEnum.ClaimAction.GetStringValue()))
                    {
                        automatedBase.CurrentPage.CloseAnyPopupIfExist();
                        automatedBase.CurrentPage.ClickOnBrowserBackButton();
                        automatedBase.CurrentPage.WaitForPageToLoadWithSideBarPanel();
                    }
                    loadMoreValue = _claimSearch.GetLoadMoreText();
                    numbers = Regex.Split(loadMoreValue, @"\D+").Where(s => s != Empty).Select(m => int.Parse(m.Trim()))
                        .ToList();
                    if (numbers[0] < numbers[1])
                    {
                        _claimSearch.ClickOnLoadMore();
                    }
                    _claimSearch.GetGridViewSection.GetGridListValueByCol()
                        .ShouldCollectionBeEquivalent(
                            valueToCompare, "Search results should match");
                }

                #endregion
            }
        }

        [Test, NUnit.Framework.Category("SmokeTestDeployment"), NUnit.Framework.Category("ExcludeDailyTest")] //TANT-79
        public void Verify_Claim_Search()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ClaimSearchPage _claimSearch;
                _claimSearch = automatedBase.QuickLaunch.NavigateToClaimSearch();
                _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                    ClaimQuickSearchTypeEnum.UnreviewedClaims.GetStringValue());
                _claimSearch.GetSideBarPanelSearch.ClickOnFindButton();
                _claimSearch.WaitForWorkingAjaxMessage();
                var listBeforeSearch = _claimSearch.GetGridViewSection.GetGridAllRowData();
                var claimstatusDistinctValue = _claimSearch.GetClaimStatus().Distinct().ToList();
                claimstatusDistinctValue.Count.ShouldBeEqual(1, "Distinct Count should equal to one");
                claimstatusDistinctValue[0].ShouldBeEqual
                (ClaimStatusTypeEnum.CotivitiUnreviewed.GetStringValue(),
                    "Claims with status Cotiviti Unreviewed are displayed in the Search results");
                var newClaimAction = _claimSearch.ClickOnClaimSequenceOfSearchResult(1);
                newClaimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                newClaimAction.GetPageHeader().ShouldBeEqual(PageHeaderEnum.ClaimAction.GetStringValue(),
                    "Client Action Page Should Be Opened");
                automatedBase.CurrentPage = _claimSearch = newClaimAction.ClickClaimSearchIcon();
                _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel("Quick Search").ShouldBeEqual(
                    ClaimQuickSearchTypeEnum.UnreviewedClaims.GetStringValue(),
                    "Quick search option should be unreviewed claims.");
                _claimSearch.GetGridViewSection.GetGridAllRowData()
                    .ShouldCollectionBeEqual(listBeforeSearch, "Previous list should be retained");
            }
        }


        [Test]//US50648
        [Retrying(Times = 3)]
        public void Verify_other_claim_number_field_with_searched_validation()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ClaimSearchPage _claimSearch;
                _claimSearch = automatedBase.QuickLaunch.NavigateToClaimSearch();
                
                var TestName = new StackFrame(true).GetMethod().Name;
                var otherClaimNumber = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestName,
                    "OtherClaimNumber", "Value");
                try
                {
                    _claimSearch.IsOtherClaimNumberFieldDisplayed().ShouldBeTrue("Other Claim Number Should displayed");
                    var maxCharacter = new string('a', 25) + new string('1', 26);
                    _claimSearch.SetOtherClaimNoInFindClaimSection(maxCharacter);
                    _claimSearch.GetOtherClaimNoInFindClaimSection()
                        .Length.ShouldBeEqual(50, "Accept only max 50 alphanumeric characters");
                    _claimSearch.ClickonClaimSequenceInFindClaimSectionTextField();
                    _claimSearch.GetOtherClaimNoInFindClaimSection()
                        .ShouldBeNullorEmpty("Other Claim Number Should clear when click away from that field");
                    _claimSearch.SearchByOtherClaimNmber("Test123");
                    _claimSearch.GetNoMatchingRecordFoundMessage()
                        .ShouldBeEqual("No matching records were found.", "No Result Message");
                    _claimSearch.SearchByOtherClaimNmber(otherClaimNumber);
                    _claimSearch.GetSearchResultRowCount()
                        .ShouldBeGreater(1,
                            "Claim Search Result Row Should be displayed when searched by Other Claim Number");
                }
                finally
                {
                    _claimSearch.ClickOnClearLinkOnFindSection();
                }
            }
        }

        [Test] //US69155
        [Retrying(Times = 3)]
        public void Verify_Assigned_To_search_filter_when_All_Claims_Option_is_selected()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ClaimSearchPage _claimSearch;
                _claimSearch = automatedBase.QuickLaunch.NavigateToClaimSearch();
                var TestName = new StackFrame(true).GetMethod().Name;
                var assignedTo = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestName,
                    "AssignedTo", "Value");
                _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel("Quick Search")
                    .ShouldBeEqual(ClaimQuickSearchTypeEnum.AllClaims.GetStringValue(),
                        "Quick Search option defaults to All Claims");
                _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel("Assigned To")
                    .ShouldBeEqual("All", "Assigned To");
                _claimSearch.ClickOnFindButton();
                _claimSearch.IsPageErrorPopupModalPresent().ShouldBeTrue("Page error message should be present");
                _claimSearch.GetPageErrorMessage().ShouldBeEqual(
                    "Search cannot be initiated without any criteria entered.",
                    "Is Error Message equal?");
                _claimSearch.ClosePageError();
                _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Assigned To", assignedTo);
                _claimSearch.ClickOnFindButton();
                _claimSearch.GetSearchResultRowCount()
                    .ShouldBeGreater(1, "Claim Search Result Row Should be displayed when searched by Assigned To");
                _claimSearch.ClickSearchResultRow();
                _claimSearch.GetAssignedTo().ShouldBeEqual(assignedTo, "Assigned To value should match");
            }
        }

        [Test] //TE201
        [Retrying(Times = 3)]
        public void Verify_batchId_search_filter_when_All_claims_option_selected()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ClaimSearchPage _claimSearch;
                _claimSearch = automatedBase.QuickLaunch.NavigateToClaimSearch();
                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestName);
                _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel("Quick Search").ShouldBeEqual(
                    ClaimQuickSearchTypeEnum.AllClaims.GetStringValue(),
                    "Default filter for quick search is All claims");
                _claimSearch.GetSideBarPanelSearch.IsSearchInputFieldDisplayedByLabel("Batch ID")
                    .ShouldBeTrue("Batch search field should be displayed");
                _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel("Batch ID")
                    .ShouldBeNullorEmpty("Default value for BatchId ");
                _claimSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Batch ID", paramLists["BatchId1Search"]);
                _claimSearch.GetSideBarPanelSearch.ClickOnFindButton();
                _claimSearch.WaitForWorkingAjaxMessage();
                _claimSearch.GetSearchResultRowCount().ShouldBeGreater(0, "Claim Search Result Row ");
                _claimSearch.ClickSearchResultRow();
                _claimSearch.GetBatchId().ShouldBeEqual(paramLists["BatchId1"], "Is Correct Batch Id display?");
                _claimSearch.GetGridViewSection.GetGridListValueByCol()
                    .ShouldCollectionBeEquivalent(paramLists["ClaSeq"].Split(',').ToList(),
                        "Is Correct List of Claim Sequence display?");
                _claimSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Batch ID", paramLists["BatchId2Search"]);
                _claimSearch.GetSideBarPanelSearch.ClickOnFindButton();
                _claimSearch.WaitForWorkingAjaxMessage();
                _claimSearch.GetCountOfClaseqFromBatchId(paramLists["BatchId2"])
                    .ShouldBeGreater(2000, "The result set should be greater than 2000");
                _claimSearch.IsPageErrorPopupModalPresent()
                    .ShouldBeTrue(
                        "User should be prompted to limit search criteria for search results more than 2000 lines");
                _claimSearch.GetPageErrorMessage()
                    .ShouldBeEqual("The result set is too large. Please limit your search criteria.",
                        "Verify the popup message when result set yeails more than 2000 lines");
                _claimSearch.ClosePageError();
                StringFormatter.PrintMessage("verify character limit");
                _claimSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Batch ID",
                    Concat(Enumerable.Repeat("*a1@_A", 21)));
                _claimSearch.GetSideBarPanelSearch.GetLengthOfTheInputFieldByLabel("Batch ID")
                    .ShouldBeEqual(100, "Verification of Maximum length of BatchId");
                _claimSearch.GetSideBarPanelSearch.ClickOnFindButton();
                _claimSearch.WaitForWorkingAjaxMessage();
                StringFormatter.PrintMessage("verifying invalid batch id");
                _claimSearch.GetNoMatchingRecordFoundMessage().ShouldBeEqual("No matching records were found.",
                    "Verify Empty Record Message on Sidebar Panel");
            }
        }

        [Test]//CAR-3148(CAR-3047)
        public void Verify_BatchDate_Filter_and_search_result()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ClaimSearchPage _claimSearch;
                _claimSearch = automatedBase.QuickLaunch.NavigateToClaimSearch();
                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestName);
                var batchDate = paramLists["batchDate"];
                var invalidBatchDate = paramLists["invalidBatchDate"];
                _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",ClaimQuickSearchTypeEnum.UnreviewedClaims.GetStringValue());
                _claimSearch.GetSideBarPanelSearch.IsSearchInputFieldDisplayedByLabel("Batch Date").ShouldBeTrue("Is Batch Date filter present?");
                _claimSearch.GetSideBarPanelSearch.GetInputAttributeValueByLabel("Batch Date", "placeholder")
                    .ShouldBeEqual("MM/DD/YYYY", "Placeholder value should match");
                _claimSearch.GetSideBarPanelSearch.SetDateField("Batch Date", batchDate, 1);
                _claimSearch.ClickOnFindButton();
                _claimSearch.WaitForWorking();
                _claimSearch.GetGridViewSection.GetGridListValueByCol()
                    .ShouldCollectionBeEquivalent(_claimSearch.GetUnreviewedClaimsByBatchDate(batchDate, automatedBase.EnvironmentManager.Username),"claim search result correct?");

                StringFormatter.PrintMessage("Verify No Data Found Message for Invalid Batch Date");
                _claimSearch.GetSideBarPanelSearch.ClickOnClearLink();
                _claimSearch.GetSideBarPanelSearch.SetDateField("Batch Date", invalidBatchDate, 1);
                _claimSearch.ClickOnFindButton();
                _claimSearch.WaitForWorking();
                _claimSearch.GetNoMatchingRecordFoundMessage().ShouldBeEqual("No matching records were found.",
                    "Verify Empty Record Message on Sidebar Panel");
            }
        }

        [Test] //TE692
        [Retrying(Times = 3)]
        public void Verify_MemberId_search_filter_when_All_claims_option_selected()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ClaimSearchPage _claimSearch;
                _claimSearch = automatedBase.QuickLaunch.NavigateToClaimSearch();
                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestName);
                var validMemberId = paramLists["MemberId"].Split(',')[0];
                var restrictedMemberId = paramLists["MemberId"].Split(',')[1];


                _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel("Quick Search").ShouldBeEqual(
                    ClaimQuickSearchTypeEnum.AllClaims.GetStringValue(),
                    "Default filter for quick search is All claims");
                _claimSearch.GetSideBarPanelSearch.IsSearchInputFieldDisplayedByLabel("Member ID")
                    .ShouldBeTrue("member ID field should be displayed");
                _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel("Member ID")
                    .ShouldBeNullorEmpty("Default value for BatchId ");
                _claimSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Member ID", validMemberId);
                _claimSearch.GetSideBarPanelSearch.ClickOnFindButton();
                _claimSearch.WaitForWorkingAjaxMessage();
                _claimSearch.GetSearchResultRowCount().ShouldBeGreater(0, "Claim Search Result Row ");
                _claimSearch.GetClaimSeqListOnSearchResult().ShouldCollectionBeEquivalent(
                    _claimSearch.GetClaimSequenceByMemberIdFromDB(validMemberId), "claim sequence value same?");


                StringFormatter.PrintMessage("verify character limit");
                _claimSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Member ID",
                    Concat(Enumerable.Repeat("a1@_A", 21)));
                _claimSearch.GetSideBarPanelSearch.GetLengthOfTheInputFieldByLabel("Member ID")
                    .ShouldBeEqual(100, "Verification of Maximum length of BatchId");
                _claimSearch.GetSideBarPanelSearch.ClickOnFindButton();
                _claimSearch.WaitForWorkingAjaxMessage();
                StringFormatter.PrintMessage("verifying invalid member id");
                _claimSearch.GetNoMatchingRecordFoundMessage().ShouldBeEqual("No matching records were found.",
                    "Verify Empty Record Message on Sidebar Panel");
                _claimSearch.GetSideBarPanelSearch.ClickOnClearLink();

                StringFormatter.PrintMessage("verify Claim Restriction Message for restricted patient ");
                _claimSearch = automatedBase.CurrentPage.Logout().LoginAsClaimViewRestrictionUser()
                    .NavigateToClaimSearch();
                _claimSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Member ID", restrictedMemberId);
                _claimSearch.ClickOnFindButton();
                _claimSearch.IsPageErrorPopupModalPresent()
                    .ShouldBeTrue("Error Message displayed when searching restricted patient");
                _claimSearch.GetPageErrorMessage()
                    .ShouldBeEqual(
                        $"This claim has {_claimSearch.GetPatientRestrictionDescriptionByPatNum(restrictedMemberId)} restriction(s) which is preventing you from viewing it. Please contact Cotiviti Client Services if you feel this is an error.");
                _claimSearch.ClosePageError();
            }
        }

        [Test]
        [Retrying(Times = 3)]
        public void Verify_basic_search_filters_for_pended_quick_Search_option() //US65789 + CAR2304(CAR-2135)
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ClaimSearchPage _claimSearch;
                _claimSearch = automatedBase.QuickLaunch.NavigateToClaimSearch();
                var TestName = new StackFrame(true).GetMethod().Name;
                List<string> _smtstSubStatusList = _claimSearch.GetAssociatedClaimSubStatusForInternalUser(ClientEnum.SMTST.ToString());
                List<string> _smtstPlanList = _claimSearch.GetAssociatedPlansForClient(ClientEnum.SMTST.ToString());
                List<string> _claimsAssignedToUserList = _claimSearch.GetAssignedToList(ClientEnum.SMTST.ToString());
                List<string> _cvtyClaimsAssignedToUserList = _claimSearch.GetAssignedToList(ClientEnum.CVTY.GetStringDisplayValue());
                List<string> _activeProductListForClient = (List<string>)_claimSearch.GetActiveProductListForClient(ClientEnum.SMTST.ToString());
                List<string> _allProductListFromDB = _claimSearch.GetCommonSql.GetAllProductList();
                int _pendedClaimsCount = Convert.ToInt16(_claimSearch.GetPendedClaimsCount(ClientEnum.SMTST.ToString(), automatedBase.EnvironmentManager.HciAdminUsername)[0]);

                var expectedProductTypeList = GetExpectedProductListFromDatabase(_allProductListFromDB);
                _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel("Quick Search")
                    .ShouldBeEqual(ClaimQuickSearchTypeEnum.AllClaims.GetStringValue(),
                        "Quick Search option defaults to All Claims");
                _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                    ClaimQuickSearchTypeEnum.PendedClaims.GetStringValue(), false); //check for type ahead functionality
                Verify_correct_search_filter_options_displayed_for("pended_claims",
                    ClaimQuickSearchTypeEnum.PendedClaims.GetStringValue());
                StringFormatter.PrintMessageTitle("Claim Sub Status field Validation");
                ValidateFieldSupportingMultipleValues("Claim Sub Status", _smtstSubStatusList);
                StringFormatter.PrintMessageTitle("Product field Validation");
                ValidateSingleDropDownForDefaultValueAndExpectedList("Product", expectedProductTypeList);
                StringFormatter.PrintMessageTitle("Plan field Validation");
                ValidateFieldSupportingMultipleValues("Plan", _smtstPlanList);
                StringFormatter.PrintMessageTitle("Assigned To Field Validation");
                ValidateAssignedTo("Assigned To", ClientEnum.SMTST.ToString());
                _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel("Provider Sequence")
                    .ShouldBeNullorEmpty("Provider Sequence should be empty");
                _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel("Member ID")
                    .ShouldBeNullorEmpty("Provider Sequence should be empty");
                _claimSearch.GetSideBarPanelSearch.ClickOnFindButton();
                _claimSearch.WaitForWorking();
                _claimSearch.ClickOnLoadMore();
                var loadMoreValue = _claimSearch.GetLoadMoreText();
                var numbers = Regex.Split(loadMoreValue, @"\D+").Where(s => s != Empty).Select(m => int.Parse(m.Trim()))
                    .ToList();
                _pendedClaimsCount = Convert.ToInt16(_claimSearch.GetPendedClaimsCount(ClientEnum.SMTST.ToString(),
                    automatedBase.EnvironmentManager.HciAdminUsername)[0]);
                numbers[1].ShouldBeEqual(_pendedClaimsCount, "Pended Claims count match");
                if (Enumerable.Range(1, 49).Contains(numbers[0]))
                {
                    numbers[0].ShouldBeEqual
                    (_pendedClaimsCount,
                        "For claim count less than 50, clicking on Load more should equal the Pended Claims count with the database");
                }
                else
                {
                    numbers[0].ShouldBeEqual(50, "The value should equal to 50");
                }

                var list = _claimSearch.GetClaimStatus().Distinct().ToList();
                _smtstSubStatusList.Add("FFP Pend");
                _smtstSubStatusList.Add("CV Pend");
                list.CollectionShouldBeSubsetOf(_smtstSubStatusList,
                    "Pended Claims Sub Status in the Search List should match with the list from the database");
                _smtstSubStatusList.Remove("FFP Pend");
                _smtstSubStatusList.Remove("CV Pend");

                void Verify_correct_search_filter_options_displayed_for(string mappingQuickSearchOptionName, string quickSearchValue)
                {

                    _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search", quickSearchValue);
                    _claimSearch.GetSideBarPanelSearch.GetSearchFiltersList()
                        .ShouldCollectionBeEqual(
                            automatedBase.DataHelper.GetMappingData(FullyQualifiedClassName, mappingQuickSearchOptionName).Values.ToList(), "Search Filters",
                            true);
                }

                List<String> GetExpectedProductListFromDatabase(List<string> allProductList)
                {
                    List<string> expectedProductTypes = new List<string>();

                    for (int i = 0; i < _activeProductListForClient.Count; i++)
                    {
                        if (_activeProductListForClient[i] == "T")
                        {
                            expectedProductTypes.Add(allProductList[i]);
                        }
                    }

                    expectedProductTypes.Insert(0, "All");
                    return expectedProductTypes;
                }

                void ValidateFieldSupportingMultipleValues(string label, IList<string> expectedDropDownList)
                {
                    ValidateMultipleDropDownForDefaultValueAndExpectedList(label, expectedDropDownList);
                    _claimSearch.GetSideBarPanelSearch.SelectSearchDropDownListForMultipleSelectValue(label, expectedDropDownList[0]);
                    _claimSearch.GetSideBarPanelSearch.GetInputAttributeValueByLabel(label, "placeholder")
                        .ShouldBeEqual(expectedDropDownList[0], label + "single value selected");
                    _claimSearch.GetSideBarPanelSearch.SelectSearchDropDownListForMultipleSelectValue(label, expectedDropDownList[expectedDropDownList.Count - 1]);
                    _claimSearch.GetSideBarPanelSearch.GetInputAttributeValueByLabel(label, "placeholder")
                        .ShouldBeEqual("Multiple values selected", label + "multiple value selected");
                    _claimSearch.GetSideBarPanelSearch.SelectSearchDropDownListForMultipleSelectValue(label, "Clear");

                }

                void ValidateMultipleDropDownForDefaultValueAndExpectedList(string label, IList<string> collectionToEqual)
                {
                    var listedOptionsList = _claimSearch.GetSideBarPanelSearch.GetMultiSelectAvailableDropDownList(label);
                    listedOptionsList.Count.ShouldBeGreater(0, "List of " + label + " is greater than zero.");
                    _claimSearch.GetSideBarPanelSearch.GetMultiSelectListedDropDownList(label).Contains("All")
                        .ShouldBeTrue(
                            "A value of all displayed at the top of the list");

                    listedOptionsList.ShouldCollectionBeEqual(collectionToEqual, label + " List As Expected , followed by options sorted alphabetically.");

                    listedOptionsList.IsInAscendingOrder().ShouldBeTrue(label + " should be sorted in alphabetical order.");
                    _claimSearch.GetSideBarPanelSearch.GetInputAttributeValueByLabel(label, "placeholder")
                        .ShouldBeEqual("Select one or more", label + " value defaults to 'select one or more'");
                }

                void ValidateSingleDropDownForDefaultValueAndExpectedList(string label, IList<string> collectionToEqual, bool order = true)
                {
                    var actualDropDownList = _claimSearch.GetSideBarPanelSearch.GetAvailableDropDownList(label);
                    actualDropDownList.Count.ShouldBeGreater(0, "List of " + label + " is greater than zero.");
                    if (collectionToEqual != null)
                        actualDropDownList.ShouldCollectionBeEqual(collectionToEqual, label + " List As Expected");
                    if (order)
                    {
                        actualDropDownList.Remove("All");
                        actualDropDownList.IsInAscendingOrder().ShouldBeTrue(label + " should be sorted in alphabetical order.");
                    }
                    _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel(label).ShouldBeEqual("All", label + " value defaults to All");
                    _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel(label, actualDropDownList[0], false); //check for type ahead functionality
                    _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel(label, actualDropDownList[1]);
                    _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel(label).ShouldBeEqual(actualDropDownList[1], "User can select only a single option");
                    _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel(label, "All");
                }

                void ValidateAssignedTo(string label, string client)
                {
                    _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel(label).ShouldBeEqual("All", label + " value defaults to All");
                    var reqDropDownList = _claimSearch.GetSideBarPanelSearch.GetAvailableDropDownList(label);
                    reqDropDownList.Remove("All");
                    if (client == ClientEnum.SMTST.ToString())
                    {
                        _claimsAssignedToUserList.Sort();
                        reqDropDownList.ShouldCollectionBeEqual(_claimsAssignedToUserList, "Validate Assigned To List with the database");
                    }
                    else if (client == ClientEnum.CVTY.GetStringDisplayValue())
                    {
                        _cvtyClaimsAssignedToUserList.Sort();
                        reqDropDownList.ShouldCollectionBeEqual(_cvtyClaimsAssignedToUserList, "Validate Assigned To List with the database");
                    }
                    reqDropDownList[0].DoesNameContainsOnlyFirstWithLastname()
                        .ShouldBeTrue(label + " should be in proper format of <firstname> <lastname> (user id)");

                }
            }
        }

        [Test]
        [Retrying(Times = 3)]
        public void Verify_that_default_value_and_the_clear_filters_clears_all_filters_claim_search() //US65789+US66263+US68072 + CAR-3145(CAR-3051) + CAR-3148(CAR-3047)
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ClaimSearchPage _claimSearch;
                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestName);
                //page should be initial state to verify default value
                _claimSearch = automatedBase.CurrentPage.ClickOnQuickLaunch().NavigateToClaimSearch();
                StringFormatter.PrintMessageTitle("Verify default input value for Unreviwed Claims");
                _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                    ClaimQuickSearchTypeEnum.UnreviewedClaims.GetStringValue());
                _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel("Product").ShouldBeEqual("All", "Product");
                #region  CAR-3145(CAR-3051)
                _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel("Restrictions").ShouldBeEqual("All", "Restriction");
                #endregion
                _claimSearch.GetSideBarPanelSearch.GetInputAttributeValueByLabel("Plan", "placeholder")
                    .ShouldBeEqual("Select one or more", "Plan");
                _claimSearch.GetSideBarPanelSearch.GetInputAttributeValueByLabel("Line Of Business", "placeholder")
                    .ShouldBeEqual("Select one or more", "Line Of Business");
                _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel("Claim Type")
                    .ShouldBeEqual("All", "Claim Type");
                _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel("Batch ID").ShouldBeEqual("All", "Batch ID");
                _claimSearch.GetSideBarPanelSearch.GetInputAttributeValueByLabel("Review Group", "placeholder")
                    .ShouldBeEqual("Select one or more", "Review Group Provider Sequence");
                _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel("Provider Sequence")
                    .ShouldBeEqual("", "Provider Sequence");
                _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                    ClaimQuickSearchTypeEnum.PendedClaims.GetStringValue());
                _claimSearch.GetSideBarPanelSearch.GetInputAttributeValueByLabel("Claim Sub Status", "placeholder")
                    .ShouldBeEqual("Select one or more", "Claim Sub Status");
                _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel("Product").ShouldBeEqual("All", "Product");
                _claimSearch.GetSideBarPanelSearch.GetInputAttributeValueByLabel("Plan", "placeholder")
                    .ShouldBeEqual("Select one or more", "Plan");
                _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel("Assigned To")
                    .ShouldBeEqual("All", "Assigned To");
                _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel("Provider Sequence")
                    .ShouldBeEqual("", "Provider Sequence");
                _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel("Member ID").ShouldBeEqual("", "Member ID");

                StringFormatter.PrintMessageTitle("Verification of Clear Filter for Unreviewed Claims");
                _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                    ClaimQuickSearchTypeEnum.UnreviewedClaims.GetStringValue());
                _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Claim Status",
                    paramLists["UnreviewedClaimStatus"], false);
                _claimSearch.GetSideBarPanelSearch.SelectSearchDropDownListForMultipleSelectValue("Line Of Business",
                    paramLists["LineofBusiness1"]);
                _claimSearch.GetSideBarPanelSearch.SelectSearchDropDownListForMultipleSelectValue("Line Of Business",
                    paramLists["LineofBusiness2"]);
                _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Product", paramLists["ProductType"],
                    false);
                _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Restrictions", paramLists["Restriction"],
                    false);
                _claimSearch.GetSideBarPanelSearch.SelectSearchDropDownListForMultipleSelectValue("Plan",
                    paramLists["Plan1"]);
                _claimSearch.GetSideBarPanelSearch.SelectSearchDropDownListForMultipleSelectValue("Plan",
                    paramLists["Plan2"]);
                _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Claim Type", paramLists["ClaimType"],
                    false);
                _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Batch ID", paramLists["BatchID"],
                    false);
                #region  CAR-3148(CAR-3047)
                _claimSearch.GetSideBarPanelSearch.SetDateField("Batch Date", "02/25/2011", 1);
                #endregion  
                _claimSearch.GetSideBarPanelSearch.SelectSearchDropDownListForMultipleSelectValue("Review Group",
                    paramLists["ReviewGroup"].Split(',').ToList()[0]);
                _claimSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Provider Sequence",
                    paramLists["ProviderSequence"]);
                _claimSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Member ID", paramLists["MemberId"]);
                _claimSearch.GetSideBarPanelSearch.ClickOnClearLink();

                StringFormatter.PrintMessageTitle(
                    "Verify Clear Filter clears does not clear Quick Search and Claim Status when Unreviewed Claims is selected");
                _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel("Quick Search")
                    .ShouldBeEqual(ClaimQuickSearchTypeEnum.UnreviewedClaims.GetStringValue(), "Quick Search");
                _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel("Claim Status")
                    .ShouldBeEqual(ClaimStatusTypeEnum.CotivitiUnreviewed.GetStringValue(), "Claim Status");

                StringFormatter.PrintMessageTitle(
                    "Verify Clear Filter clears all the rest of the search filters - Unreviewed Claims ");
                _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel("Product").ShouldBeEqual("All", "Product");
                _claimSearch.GetSideBarPanelSearch.GetInputAttributeValueByLabel("Plan", "placeholder")
                    .ShouldBeEqual("Select one or more", "Plan");
                _claimSearch.GetSideBarPanelSearch.GetInputAttributeValueByLabel("Line Of Business", "placeholder")
                    .ShouldBeEqual("Select one or more", "Line Of Business");
                _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel("Claim Type")
                    .ShouldBeEqual("All", "Claim Type");
                _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel("Batch ID").ShouldBeEqual("All", "Batch ID");
                _claimSearch.GetSideBarPanelSearch.GetDateFieldFrom("Batch Date")
                    .ShouldBeNullorEmpty("Batch Date Empty?");
                _claimSearch.GetSideBarPanelSearch.GetInputAttributeValueByLabel("Review Group", "placeholder")
                    .ShouldBeEqual("Select one or more", "Review Group Provider Sequence");
                _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel("Provider Sequence")
                    .ShouldBeEqual("", "Provider Sequence");
                _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel("Member ID").ShouldBeEqual("", "Member ID");


                StringFormatter.PrintMessageTitle("Verification of Clear Filter for Pended Claims");
                _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                    ClaimQuickSearchTypeEnum.PendedClaims.GetStringValue());
                _claimSearch.GetSideBarPanelSearch.SelectSearchDropDownListForMultipleSelectValue("Claim Sub Status",
                    paramLists["PendedClaimSubStatus1"]);
                _claimSearch.GetSideBarPanelSearch.SelectSearchDropDownListForMultipleSelectValue("Claim Sub Status",
                    paramLists["PendedClaimSubStatus2"]);
                _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Product", paramLists["ProductType"]);
                _claimSearch.GetSideBarPanelSearch.SelectSearchDropDownListForMultipleSelectValue("Plan",
                    paramLists["Plan2"]);
                _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Assigned To",
                    paramLists["AssignedTo"], false);
                _claimSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Provider Sequence",
                    paramLists["ProviderSequence"]);
                _claimSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Member ID", paramLists["MemberId"]);
                _claimSearch.GetSideBarPanelSearch.ClickOnClearLink();

                StringFormatter.PrintMessageTitle(
                    "Verify Clear Filter clears does not clear Quick Search and Claim Sub Status when Pended Claims is selected");
                _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel("Quick Search")
                    .ShouldBeEqual(ClaimQuickSearchTypeEnum.PendedClaims.GetStringValue(), "Quick Search");
                _claimSearch.GetSideBarPanelSearch.GetInputAttributeValueByLabel("Claim Sub Status", "placeholder")
                    .ShouldBeEqual("Select one or more", "Claim Sub Status");
                _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel("Product").ShouldBeEqual("All", "Product");
                _claimSearch.GetSideBarPanelSearch.GetInputAttributeValueByLabel("Plan", "placeholder")
                    .ShouldBeEqual("Select one or more", "Plan");
                _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel("Assigned To")
                    .ShouldBeEqual("All", "Assigned To");
                _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel("Provider Sequence")
                    .ShouldBeEqual("", "Provider Sequence");
                _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel("Member ID").ShouldBeEqual("", "member Id");

                StringFormatter.PrintMessageTitle("Verification of Clear Filter for Flagged Claims");
                _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                    ClaimQuickSearchTypeEnum.FlaggedClaims.GetStringValue());
                _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Claim Status",
                    ClaimStatusTypeEnum.CotivitiReviewed.GetStringValue());
                _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Product", paramLists["ProductType"],
                    false);
                _claimSearch.GetSideBarPanelSearch.SelectSearchDropDownListForMultipleSelectValue("Plan",
                    paramLists["Plan1"]);
                _claimSearch.GetSideBarPanelSearch.SelectSearchDropDownListForMultipleSelectValue("Plan",
                    paramLists["Plan2"]);
                _claimSearch.GetSideBarPanelSearch.SelectSearchDropDownListForMultipleSelectValue("Line Of Business",
                    paramLists["LineofBusiness1"]);
                _claimSearch.GetSideBarPanelSearch.SelectSearchDropDownListForMultipleSelectValue("Line Of Business",
                    paramLists["LineofBusiness2"]);
                _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Batch ID",
                    paramLists["BatchIDFlaggedClaim"]);
                _claimSearch.GetSideBarPanelSearch.SelectSearchDropDownListForMultipleSelectValue("Review Group",
                    paramLists["ReviewGroup"].Split(',').ToList()[0]);
                _claimSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Member ID", paramLists["MemberId"]);
                _claimSearch.GetSideBarPanelSearch.ClickOnClearLink();

                StringFormatter.PrintMessageTitle(
                    "Verify Clear Filter clears does not clear Quick Search and Batch ID when Flagged Claims is selected");
                _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel("Quick Search")
                    .ShouldBeEqual(ClaimQuickSearchTypeEnum.FlaggedClaims.GetStringValue());
                _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel("Claim Status")
                    .ShouldBeEqual("All", "Claim Status");
                _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel("Product").ShouldBeEqual("All", "Product");
                _claimSearch.GetSideBarPanelSearch.GetInputAttributeValueByLabel("Plan", "placeholder")
                    .ShouldBeEqual("Select one or more", "Plan");
                _claimSearch.GetSideBarPanelSearch.GetInputAttributeValueByLabel("Line Of Business", "placeholder")
                    .ShouldBeEqual("Select one or more", "Line Of Business");
                _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel("Batch ID")
                    .ShouldBeEqual(paramLists["BatchID"], "Most recent Batch ID");
                _claimSearch.GetSideBarPanelSearch.GetInputAttributeValueByLabel("Review Group", "placeholder")
                    .ShouldBeEqual("Select one or more", "Review Group");
                _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel("Member ID").ShouldBeEqual("", "Member ID");
            }
        }

        [Test]
        [Retrying(Times = 3)]
        public void Verify_basic_search_filters_for_unreviewed_quick_Search_option() //US66263 + CAR-1408, TE-451 + TE-556  +  CAR2304(CAR-2135) + TE-835 + CAR-3052
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ClaimSearchPage _claimSearch;
                _claimSearch = automatedBase.QuickLaunch.NavigateToClaimSearch();
                var TestName = new StackFrame(true).GetMethod().Name;
                List<string> _allProductListFromDB = _claimSearch.GetCommonSql.GetAllProductList();
                List<string> _activeProductListForClient = (List<string>)_claimSearch.GetActiveProductListForClient(ClientEnum.SMTST.ToString());
                var expectedProductTypeList = GetExpectedProductListFromDatabase(_allProductListFromDB);
                List<string> _smtstPlanList = _claimSearch.GetAssociatedPlansForClient(ClientEnum.SMTST.ToString());
                List<string> _lineOfBusinessList = _claimSearch.GetCommonSql.GetLineOfBusiness();
                List<string> _smtstReviewGroup = _claimSearch.GetReviewGroup(ClientEnum.SMTST.ToString());
                List<string> _flagList = _claimSearch.GetAllFlagsFromDatabase();
                int _unreviewedClaimsCountInternalUser = Convert.ToInt16(_claimSearch.GetUnreviewedClaimListForInternalUser(ClientEnum.SMTST.ToString(), automatedBase.EnvironmentManager.HciAdminUsername)[0]);

                var appealStatusList =
                    automatedBase.DataHelper.GetMappingData(FullyQualifiedClassName, "appeal_status").Values.ToList();
                _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel("Quick Search")
                    .ShouldBeEqual(ClaimQuickSearchTypeEnum.AllClaims.GetStringValue(),
                        "Quick Search option defaults to All Claims");
                _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                    ClaimQuickSearchTypeEnum.UnreviewedClaims.GetStringValue());
                Verify_correct_search_filter_options_displayed_for("unreviewed_claims",
                    ClaimQuickSearchTypeEnum.UnreviewedClaims.GetStringValue());
                StringFormatter.PrintMessageTitle("Claim Status field Validation");
                ValidateClaimStatusforUnreviewedClaims("Claim Status");
                StringFormatter.PrintMessageTitle("Product field Validation");
                ValidateSingleDropDownForDefaultValueAndExpectedList("Product", expectedProductTypeList);
                StringFormatter.PrintMessageTitle("Plan field Validation");
                ValidateFieldSupportingMultipleValues("Plan", _smtstPlanList);
                StringFormatter.PrintMessageTitle("Line of Business Field Validation");
                ValidateFieldSupportingMultipleValues("Line Of Business", _lineOfBusinessList);
                StringFormatter.PrintMessageTitle("Claim Type Field Validation");
                ValidateDropdownListWithMappingData("Claim Type", "claim_type");

                #region CAR-3052
                StringFormatter.PrintMessageTitle("Flag Field Validation");
                ValidateFieldSupportingMultipleValues("Flag", _flagList);
                #endregion

                StringFormatter.PrintMessageTitle("Batch ID Validation");
                ValidateBatchIdListWithDatabase("Batch ID");
                StringFormatter.PrintMessage("Batch Date validation");//CAR-3148
                _claimSearch.GetSideBarPanelSearch.GetDateFieldFrom("Batch Date")
                    .ShouldBeNullorEmpty("batch DateTime empty?");
                StringFormatter.PrintMessageTitle("Review Group Validation");
                ValidateFieldSupportingMultipleValues("Review Group", _smtstReviewGroup);
                _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel("Provider Sequence")
                    .ShouldBeNullorEmpty("Provider Sequence should be empty");
                StringFormatter.PrintMessageTitle("Appeal Status Validation");
                ValidateDropdownListWithMappingData("Appeal Status", "appeal_status");

                //TE-556
                StringFormatter.PrintMessage("Verify Proc Code");
                _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel("Proc")
                    .ShouldBeNullorEmpty("Proc should be empty by default");
                _claimSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Proc", Concat(Enumerable.Repeat("A123", 21)));
                _claimSearch.GetSideBarPanelSearch.GetLengthOfTheInputFieldByLabel("Proc")
                    .ShouldBeEqual(5, "Maximum allowed value 5 ?");
                _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel("Member ID")
                    .ShouldBeNullorEmpty("Member Id  should be empty by default");
                _claimSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Member ID",
                    Concat(Enumerable.Repeat("A123", 26)));
                _claimSearch.GetSideBarPanelSearch.GetLengthOfTheInputFieldByLabel("Member ID")
                    .ShouldBeEqual(100, "Maximum allowed value 5 ?");
                _claimSearch.GetSideBarPanelSearch.ClickOnClearLink();



                _claimSearch.GetSideBarPanelSearch.ClickOnFindButton();
                _claimSearch.WaitForWorking();
                _claimSearch.ClickOnLoadMore();
                var loadMoreValue = _claimSearch.GetLoadMoreText();
                var numbers = Regex.Split(loadMoreValue, @"\D+").Where(s => s != Empty).Select(m => int.Parse(m.Trim()))
                    .ToList();
                _unreviewedClaimsCountInternalUser = Convert.ToInt16(
                    _claimSearch.GetUnreviewedClaimListForInternalUser(ClientEnum.SMTST.ToString(),
                        automatedBase.EnvironmentManager.HciAdminUsername)[0]);
                numbers[1].ShouldBeEqual(_unreviewedClaimsCountInternalUser,
                    "Unreviewed Claims count should match with the value from database");
                if (Enumerable.Range(1, 49).Contains(numbers[0]))
                {
                    numbers[0].ShouldBeEqual
                    (_unreviewedClaimsCountInternalUser,
                        "For claim count less than 50, clicking on Load more should equal the Unreviewed Claims count with the database");
                }
                else
                {
                    numbers[0].ShouldBeEqual(50, "The value should equal to 50");
                }

                var appealStatusAbb = new List<string> {"O', 'N', 'T', 'C','M", "O", "N", "T", "C"};
                if (_claimSearch.CurrentPageUrl.Contains("dev"))
                {
                    for (var j = 0; j < appealStatusList.Count; j++)
                    {
                        _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Appeal Status",
                            appealStatusList[j]);
                        _claimSearch.GetSideBarPanelSearch.ClickOnFindButton();
                        _claimSearch.WaitForWorking();
                        var claseqList = _claimSearch.GetGridViewSection.GetGridListValueByCol();
                        var claseqlist = claseqList.Select(x => x.Split('-')[0]).ToList();
                        var classublist = claseqList.Select(x => x.Split('-')[1]).ToList();
                        var newList = claseqlist.Zip(classublist, (s, i) => new {sv = "(" + s, iv = i + ")"}).ToList();
                        var newList1 = newList.Select(x => x.sv + "," + x.iv).ToList();
                        var finallist = Join(",", newList1.Select(x => x));
                        var appealCountList = _claimSearch.GetGridViewSection.GetAppealCountList();
                        _claimSearch.GetAppealCountByClaseqAppealStatus(finallist, appealStatusAbb[j])
                            .ShouldCollectionBeEqual(appealCountList, "Appeal Count Should be equal ");
                    }
                }

                void Verify_correct_search_filter_options_displayed_for(string mappingQuickSearchOptionName, string quickSearchValue)
                {

                    _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search", quickSearchValue);
                    _claimSearch.GetSideBarPanelSearch.GetSearchFiltersList()
                        .ShouldCollectionBeEqual(
                            automatedBase.DataHelper.GetMappingData(FullyQualifiedClassName, mappingQuickSearchOptionName).Values.ToList(), "Search Filters",
                            true);
                }

                void ValidateClaimStatusforUnreviewedClaims(string label)
                {
                    var expectedClaimStatusforUnreviewedClaims = automatedBase.DataHelper.GetMappingData(FullyQualifiedClassName, "unreviewed_claims_status").Values.ToList();
                    var actualDropDownList = _claimSearch.GetSideBarPanelSearch.GetAvailableDropDownList(label);

                    actualDropDownList.ShouldCollectionBeEqual(expectedClaimStatusforUnreviewedClaims, "Claim Status for Unreviewed Claims match");
                    _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel("Claim Status")
                        .ShouldBeEqual(ClaimStatusTypeEnum.CotivitiUnreviewed.GetStringValue(), "Is default Claim Status equal?");

                }

                void ValidateFlagForFlaggedAndUnreviewedClaims(string label)
                {
                    _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel(label).ShouldBeEqual("All", label + " value defaults to All");

                    var actualDropDownList = _claimSearch.GetSideBarPanelSearch.GetAvailableDropDownList(label);
                    _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel(label, actualDropDownList[1], false); //check for type ahead functionality
                    _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel(label, actualDropDownList[2]);
                    _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel(label).ShouldBeEqual(actualDropDownList[2], "User can select only a single option");
                    _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel(label, "All");
                    VerifyFlagListBasedOnProduct();
                }

                void VerifyFlagListBasedOnProduct()
                {
                    var productListAbbreviation = GetExpectedProductListFromDatabase(_claimSearch.GetCommonSql.GetAllProductList(true));
                    var productList = GetExpectedProductListFromDatabase(_allProductListFromDB);
                    var i = 0;
                    foreach (var product in productListAbbreviation)
                    {
                        _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Product", productList[i]);
                        var flagList = _claimSearch.GetSideBarPanelSearch.GetAvailableDropDownList("Flag");
                        flagList.RemoveAt(0);
                        if (product == "All")
                        {
                            flagList.ShouldCollectionBeEqual(_claimSearch.GetAllFlagsFromDatabase(), "Flag List Should Match when All is selected");
                        }
                        else
                        {
                            Enum.TryParse(product, out ProductEnum productName);
                            flagList.ShouldCollectionBeEqual(_claimSearch.GetActiveFlagsForProductFromDatabase(productName.GetStringDisplayValue()), "Flag List Should Match");
                        }
                        _claimSearch.GetSideBarPanelSearch.ClickOnClearLink();
                        i++;
                    }
                }
                void ValidateDropdownListWithMappingData(string label, string mappingOptionName)
                {
                    _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel(label).ShouldBeEqual("All", label + " value defaults to All");
                    var expectedMappingList = automatedBase.DataHelper.GetMappingData(FullyQualifiedClassName, mappingOptionName).Values.ToList();
                    var actualDropDownList = _claimSearch.GetSideBarPanelSearch.GetAvailableDropDownList(label);
                    actualDropDownList.Count.ShouldBeGreater(0, "List of " + label + " is greater than zero.");
                    actualDropDownList.ShouldCollectionBeEqual(expectedMappingList, (label + " List match with" + mappingOptionName));
                }

                void ValidateBatchIdListWithDatabase(string label)
                {
                    _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel(label).ShouldBeEqual("All", label + " value defaults to All");
                    var expectedList = _claimSearch.GetAssociatedBatchList(ClientEnum.SMTST.ToString());
                    expectedList.Insert(0, "All");
                    var actualDropDownList = _claimSearch.GetSideBarPanelSearch.GetAvailableDropDownList(label);
                    actualDropDownList.ShouldCollectionBeEqual(expectedList, ("Batch Id List should equal to database value"));
                }

                List<String> GetExpectedProductListFromDatabase(List<string> allProductList)
                {
                    List<string> expectedProductTypes = new List<string>();

                    for (int i = 0; i < _activeProductListForClient.Count; i++)
                    {
                        if (_activeProductListForClient[i] == "T")
                        {
                            expectedProductTypes.Add(allProductList[i]);
                        }
                    }

                    expectedProductTypes.Insert(0, "All");
                    return expectedProductTypes;
                }

                void ValidateFieldSupportingMultipleValues(string label, IList<string> expectedDropDownList)
                {
                    ValidateMultipleDropDownForDefaultValueAndExpectedList(label, expectedDropDownList);
                    _claimSearch.GetSideBarPanelSearch.SelectSearchDropDownListForMultipleSelectValue(label, expectedDropDownList[0]);
                    _claimSearch.GetSideBarPanelSearch.GetInputAttributeValueByLabel(label, "placeholder")
                        .ShouldBeEqual(expectedDropDownList[0], label + "single value selected");
                    _claimSearch.GetSideBarPanelSearch.SelectSearchDropDownListForMultipleSelectValue(label, expectedDropDownList[expectedDropDownList.Count - 1]);
                    _claimSearch.GetSideBarPanelSearch.GetInputAttributeValueByLabel(label, "placeholder")
                        .ShouldBeEqual("Multiple values selected", label + "multiple value selected");
                    _claimSearch.GetSideBarPanelSearch.SelectSearchDropDownListForMultipleSelectValue(label, "Clear");

                }

                void ValidateMultipleDropDownForDefaultValueAndExpectedList(string label, IList<string> collectionToEqual)
                {
                    var listedOptionsList = _claimSearch.GetSideBarPanelSearch.GetMultiSelectAvailableDropDownList(label);
                    listedOptionsList.Count.ShouldBeGreater(0, "List of " + label + " is greater than zero.");
                    _claimSearch.GetSideBarPanelSearch.GetMultiSelectListedDropDownList(label).Contains("All")
                        .ShouldBeTrue(
                            "A value of all displayed at the top of the list");

                    listedOptionsList.ShouldCollectionBeEqual(collectionToEqual, label + " List As Expected , followed by options sorted alphabetically.");

                    listedOptionsList.IsInAscendingOrder().ShouldBeTrue(label + " should be sorted in alphabetical order.");
                    _claimSearch.GetSideBarPanelSearch.GetInputAttributeValueByLabel(label, "placeholder")
                        .ShouldBeEqual("Select one or more", label + " value defaults to 'select one or more'");
                }

                void ValidateSingleDropDownForDefaultValueAndExpectedList(string label, IList<string> collectionToEqual, bool order = true)
                {
                    var actualDropDownList = _claimSearch.GetSideBarPanelSearch.GetAvailableDropDownList(label);
                    actualDropDownList.Count.ShouldBeGreater(0, "List of " + label + " is greater than zero.");
                    if (collectionToEqual != null)
                        actualDropDownList.ShouldCollectionBeEqual(collectionToEqual, label + " List As Expected");
                    if (order)
                    {
                        actualDropDownList.Remove("All");
                        actualDropDownList.IsInAscendingOrder().ShouldBeTrue(label + " should be sorted in alphabetical order.");
                    }
                    _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel(label).ShouldBeEqual("All", label + " value defaults to All");
                    _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel(label, actualDropDownList[0], false); //check for type ahead functionality
                    _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel(label, actualDropDownList[1]);
                    _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel(label).ShouldBeEqual(actualDropDownList[1], "User can select only a single option");
                    _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel(label, "All");
                }
            }
        }

        [Test] //US68072 + TE-556 + CAR-3052 [CAR-3179]
        [Retrying(Times = 3)]
        public void Verify_search_results_with_extra_search_filters_are_correct_unreviewed_claims() //US66263 +TE-451 +TE-556 + CAR-1716
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ClaimSearchPage _claimSearch;
                _claimSearch = automatedBase.QuickLaunch.NavigateToClaimSearch();
                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestName);
                var clientName = paramLists["ClientName"];
                var flags = paramLists["Flag"].Split(';').ToList();
                var username = paramLists["Uname"];
                var validMemberid = paramLists["MemberId"].Split(',')[0];
                var invalidMemberid = paramLists["MemberId"].Split(',')[1];
                _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel
                    ("Quick Search", ClaimQuickSearchTypeEnum.UnreviewedClaims.GetStringValue());
                _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel
                    ("Product", paramLists["ProductType"]);
                _claimSearch.GetSideBarPanelSearch.SelectSearchDropDownListForMultipleSelectValue
                    ("Line Of Business", paramLists["LineofBusiness"]);
                _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel
                    ("Claim Type", paramLists["ClaimTypeH"]);
                _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel
                    ("Batch ID", paramLists["BatchID"]);
                _claimSearch.GetSideBarPanelSearch.SetInputFieldByLabel
                    ("Provider Sequence", paramLists["ProviderSequence"]);

                _claimSearch.GetSideBarPanelSearch.ClickOnFindButton();
                _claimSearch.WaitForWorking();
                _claimSearch.GetInternalUserClaimSeq().ShouldBeEqual(paramLists["ClaimSeq"], "Claim Sequence");
                _claimSearch.GetPlanName().ShouldBeEqual("Default Plan", "Plan");
                _claimSearch.GetClaimTypeofH().ShouldBeEqual(paramLists["ClaimTypeH"], "Claim Type of H");
                _claimSearch.GetProviderSeq().ShouldBeEqual(paramLists["ProviderSequence"], "Provider Sequence");

                _claimSearch.ClickSearchResultRow();
                _claimSearch.GetBatchId().ShouldBeEqual(paramLists["BatchID"], "Batch ID");
                _claimSearch.GetLineOfBusinessfromClaimDetails()
                    .ShouldBeEqual(paramLists["LineofBusiness"], "Line Of Business");
                _claimSearch.GetSideBarPanelSearch.ClickOnClearLink();
                _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel
                    ("Claim Type", paramLists["ClaimTypeU"]);
                _claimSearch.GetSideBarPanelSearch.ClickOnFindButton();
                _claimSearch.WaitForWorking();
                _claimSearch.GetClaimTypeofU().ShouldBeEqual(paramLists["ClaimTypeU"], "Claim Type of U");
                _claimSearch.GetSideBarPanelSearch.SelectSearchDropDownListForMultipleSelectValue
                    ("Review Group", paramLists["ReviewGroup"].Split(',').ToList()[0]);
                _claimSearch.GetSideBarPanelSearch.ClickOnFindButton();
                _claimSearch.GetNoMatchingRecordFoundMessage()
                    .ShouldBeEqual("No matching records were found.", "Warning message should be displayed");
                _claimSearch.GetSideBarPanelSearch.ClickOnClearLink();

                //TE-451 + TE-556
                StringFormatter.PrintMessage("Verify Results when multiple Review Groups and Proc is selected");
                _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                    ClaimQuickSearchTypeEnum.UnreviewedClaims.GetStringValue());
                _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Claim Status",
                    ClaimStatusTypeEnum.ClientUnreviewed.GetStringValue());
                _claimSearch.GetSideBarPanelSearch.SelectMultipleValuesInMultiSelectDropdownList("Review Group",
                    paramLists["ReviewGroup"].Split(',').ToList());
                _claimSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Proc", paramLists["ValidProc"]);
                _claimSearch.GetSideBarPanelSearch.ClickOnFindButton();
                _claimSearch.WaitForWorking();
                _claimSearch.GetGridViewSection.GetGridListValueByCol().ShouldCollectionBeEqual(
                    _claimSearch.GetUnreviewedClaimForReviewGroupInternaList(paramLists["ValidProc"]),
                    "Results equal?");
                _claimSearch.GetSideBarPanelSearch.ClickOnClearLink();

                StringFormatter.PrintMessage("verify Correct details displayed when searched using member id");
                _claimSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Member ID", validMemberid);
                _claimSearch.GetSideBarPanelSearch.ClickOnFindButton();
                _claimSearch.WaitForWorking();
                _claimSearch.GetMemberId().ShouldBeEqual(validMemberid, "Member id displayed correct?");

                StringFormatter.PrintMessage("verify No Matching record found message displayed for invalid MemberId");
                _claimSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Member ID", invalidMemberid);
                _claimSearch.GetSideBarPanelSearch.ClickOnFindButton();
                _claimSearch.WaitForWorking();
                _claimSearch.GetSideBarPanelSearch.IsNoMatchingRecordFoundMessagePresent()
                    .ShouldBeTrue("Message displayed if no records found?");
                _claimSearch.GetSideBarPanelSearch.ClickOnClearLink();

                StringFormatter.PrintMessage("Verify Search Cannot Be Instantiated With Invalid Proc Code");
                _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                    ClaimQuickSearchTypeEnum.UnreviewedClaims.GetStringValue());
                _claimSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Proc", paramLists["InvalidProc"]);
                _claimSearch.GetSideBarPanelSearch.ClickOnFindButton();
                _claimSearch.WaitForWorking();
                _claimSearch.IsPageErrorPopupModalPresent()
                    .ShouldBeTrue(
                        "Search Should Not Be Initiated with invalid proc code");
                _claimSearch.GetPageErrorMessage().ShouldBeEqual("Selected Proc Code is invalid. Please search again.",
                    "Verify the popup message");
                _claimSearch.ClosePageError();
                _claimSearch.GetSideBarPanelSearch.ClickOnClearLink();

                StringFormatter.PrintMessage(
                    "Verify No Matching Records Found message when no results found for selected proc code");
                _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                    ClaimQuickSearchTypeEnum.UnreviewedClaims.GetStringValue());
                _claimSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Proc", paramLists["Proc"]);
                _claimSearch.GetSideBarPanelSearch.ClickOnFindButton();
                _claimSearch.WaitForWorking();
                _claimSearch.GetNoMatchingRecordFoundMessage()
                    .ShouldBeEqual("No matching records were found.", "Warning message should be displayed");
                _claimSearch.GetSideBarPanelSearch.ClickOnClearLink();



                #region CAR-1716 + CAR-3052

                _claimSearch.IsFlagLabelPresent().ShouldBeTrue("Is Flag Label present above Batch ID?");
                _claimSearch.GetSideBarPanelSearch.ClickOnToggleIcon("Flag");
                _claimSearch.IsFlagDropDownListAscending().ShouldBeTrue("Is Flag dropdown is ascending?");
                _claimSearch.GetSideBarPanelSearch.SelectMultipleValuesInMultiSelectDropdownList("Flag", flags);
                //_claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Flag", flags, false);
                _claimSearch.GetSideBarPanelSearch.ClickOnFindButton();
                _claimSearch.WaitForWorking();
                _claimSearch.CloseAnyPopupIfExist();
                if (_claimSearch.GetGridViewSection.IsLoadMorePresent())
                {
                    var loadMoreValue = _claimSearch.GetLoadMoreText();
                    var numbers = Regex.Split(loadMoreValue, @"\D+").Where(s => s != Empty)
                        .Select(m => int.Parse(m.Trim())).ToList();
                    numbers[1].ToString().ShouldBeEqual(
                        _claimSearch.GetCountOfUnreviewedClaimsForSpecificFlag(clientName, username, $"'{flags[0]}','{flags[1]}'"),
                        "Count of result including claim records where there is at least one active flag matching the search criteria on the claims should match with database");
                }
                else
                {
                    _claimSearch.GetGridViewSection.GetRecordRowsCountFromPage().ShouldBeEqual(
                        _claimSearch.GetCountOfUnreviewedClaimsForSpecificFlag(clientName, username, $"'{flags[0]}','{flags[1]}'"),
                        "Count of result including claim records where there is at least one active flag matching the search criteria on the claims should match with database");
                }

                #endregion
            }
        }

        [Retrying(Times = 2)]
        [Test]
        public void Verify_search_results_with_extra_search_filters_are_correct_pended_claims() //US65789
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ClaimSearchPage _claimSearch;
                _claimSearch = automatedBase.QuickLaunch.NavigateToClaimSearch();
                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestName);
                var validMemberid = paramLists["MemberId"].Split(',')[0];
                var invalidMemberid = paramLists["MemberId"].Split(',')[1];
                _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel
                    ("Quick Search", ClaimQuickSearchTypeEnum.PendedClaims.GetStringValue());
                _claimSearch.GetSideBarPanelSearch.SelectSearchDropDownListForMultipleSelectValue
                    ("Claim Sub Status", paramLists["ClaimSubStatus"]);
                _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel
                    ("Assigned To", paramLists["AssignedTo"]);
                _claimSearch.GetSideBarPanelSearch.SetInputFieldByLabel
                    ("Provider Sequence", paramLists["ProviderSequence"]);
                _claimSearch.GetSideBarPanelSearch.ClickOnFindButton();
                _claimSearch.WaitForWorking();
                _claimSearch.GetInternalUserClaimSeq().ShouldBeEqual(paramLists["ClaimSeq"], "Claim Sequence");
                _claimSearch.GetClaimSubStatus().ShouldBeEqual(paramLists["ClaimSubStatus"], "Claim Sub Status");
                _claimSearch.GetPlanName().ShouldBeEqual("Default Plan", "Plan");
                _claimSearch.GetProviderSeq().ShouldBeEqual(paramLists["ProviderSequence"], "Provider Sequence");
                _claimSearch.ClickSearchResultRow();
                _claimSearch.GetAssignedTo().ShouldBeEqual(paramLists["AssignedTo"], "Assigned To");
                _claimSearch.GetLineOfBusinessfromClaimDetails()
                    .ShouldBeEqual(paramLists["LineofBusiness"], "Line Of Business");
                _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel
                    ("Product", paramLists["ProductType"]);
                _claimSearch.GetSideBarPanelSearch.ClickOnFindButton();
                _claimSearch.GetNoMatchingRecordFoundMessage()
                    .ShouldBeEqual("No matching records were found.", "Warning message should be displayed");
                _claimSearch.GetSideBarPanelSearch.ClickOnClearLink();

                StringFormatter.PrintMessage("verify Correct details displayed when searched using member id");
                _claimSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Member ID", validMemberid);
                _claimSearch.GetSideBarPanelSearch.ClickOnFindButton();
                _claimSearch.WaitForWorking();
                _claimSearch.GetMemberId().ShouldBeEqual(validMemberid, "Member id displayed correct?");

                StringFormatter.PrintMessage("verify No Matching record found message displayed for invalid MemberId");
                _claimSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Member ID", invalidMemberid);
                _claimSearch.GetSideBarPanelSearch.ClickOnHeader();
                _claimSearch.GetSideBarPanelSearch.ClickOnFindButton();
                _claimSearch.WaitForWorking();
                _claimSearch.GetSideBarPanelSearch.IsNoMatchingRecordFoundMessagePresent()
                    .ShouldBeTrue("Message displayed if no records found?");
                _claimSearch.GetSideBarPanelSearch.ClickOnClearLink();
            }
        }

        [Test] //US66943
        [Retrying(Times = 3)]
        public void Verify_users_are_allowed_to_work_through_the_list_of_claim_results_in_Claim_Search()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ClaimSearchPage _claimSearch;
                _claimSearch = automatedBase.QuickLaunch.NavigateToClaimSearch();
                var TestName = new StackFrame(true).GetMethod().Name;
                var paramLists = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestName);
                var claimNo = paramLists["ClaimNo"];
                var auditDate = paramLists["AuditDate"];
                automatedBase.Login = _claimSearch.Logout();
                automatedBase.QuickLaunch = automatedBase.Login.LoginAsHciAdminUser5();
                automatedBase.CheckTestClientAndSwitch();
                _claimSearch = automatedBase.QuickLaunch.NavigateToClaimSearch();
                _claimSearch.UpdateClaimStatusToUnreviewedFromDatabase(claimNo);
                _claimSearch.DeleteClaimAuditOnlyByClaimNo(claimNo, auditDate);
  
                _claimSearch.SetClaimNoInFindClaimSection(claimNo);
                _claimSearch.ClickOnFindButton();
                var searchList = _claimSearch.GetClaimSeqListOnSearchResult();
                searchList.ShouldCollectionBeSorted(true, "Iterations are list with most recent claim at top");
                _claimSearch.IsClaimSearchResultsLocked(automatedBase.EnvironmentManager.HciAdminUsername5, searchList)
                    .ShouldBeFalse("Is any claims in claim search result is locked?");
                var newClaimAction = _claimSearch.ClickOnClaimSequenceOfSearchResult(1);
                newClaimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                newClaimAction.IsCurrentClaimLocked(automatedBase.EnvironmentManager.HciAdminUsername5)
                    .ShouldBeTrue("Current Claim should be locked.");
                newClaimAction.ClickOnApproveButton();
                newClaimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                newClaimAction.GetPageHeader()
                    .ShouldBeEqual("Claim Action",
                        "Page Should redirect to next claim after approving claim.");
                newClaimAction.ClickOnTransferApproveButton();
                newClaimAction.SelectStatusCode("Documentation Requested");
                newClaimAction.ClickOnSaveButton();
                newClaimAction.GetPageHeader()
                    .ShouldBeEqual("Claim Action",
                        "Page Should redirect to next claim after clicking Approve/Transfer claim.");
                newClaimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                newClaimAction.GetCurrentClaimSequence().ShouldBeEqual(searchList[2],
                    "Page should advance to next claim of the claim search list.");
                newClaimAction.ClickOnTransferButton();
                newClaimAction.SelectStatusCode("Documentation Requested");
                newClaimAction.ClickOnSaveButton();
                newClaimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                newClaimAction.GetPageHeader()
                    .ShouldBeEqual("Claim Action",
                        "Page Should redirect to next claim after clicking transfer claim.");
                newClaimAction.GetCurrentClaimSequence().ShouldBeEqual(searchList[3],
                    "Page should advance to next claim of the claim search list.");
                for (var i = 4; i <= searchList.Count(); i++)
                {
                    newClaimAction.ClickOnNextButton();
                    if (i != searchList.Count())
                        newClaimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                }

                newClaimAction.GetPageHeader()
                    .ShouldBeEqual("Claim Search",
                        "Page Should redirect to Claim Search page after all claims in claim search result are iterated.");
                _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel("Claim No").ShouldBeEqual(claimNo,
                    "Search criteria should retained after navigating back to Claim Search page.");
                _claimSearch.GetClaimSeqListOnSearchResult()
                    .ShouldBeEqual(searchList, "Search result should retain.");
                newClaimAction = _claimSearch.ClickOnClaimSequenceOfSearchResult(1);
                newClaimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                _claimSearch.SetClaimNoInFindClaimSection(claimNo);
                _claimSearch.ClickOnFindButton();
                newClaimAction.GetPageHeader()
                    .ShouldBeEqual("Claim Search",
                        "Page Should redirect to Claim Search page when claim is searched with records.");
                newClaimAction = _claimSearch.ClickOnClaimSequenceOfSearchResult(1);
                newClaimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                newClaimAction.SetClaimNoInFindClaimSection("Test123");
                newClaimAction.ClickOnFindButton();
                newClaimAction.GetEmptyMessage()
                    .ShouldBeEqual("No matching records were found.", "No Match Record Message");
                newClaimAction.GetPageHeader()
                    .ShouldBeEqual("Claim Search",
                        "Page Should redirect to Claim Search page when claim is searched without records.");
            }
        }

        [Test] //US67761
        [Retrying(Times = 3)]
        public void Verify_users_without_any_WorkList_authorities_can_still_view_the_Find_Claims_Section_Claim_Search_page()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ClaimSearchPage _claimSearch;
                _claimSearch = automatedBase.QuickLaunch.NavigateToClaimSearch();
                var TestName = new StackFrame(true).GetMethod().Name;
                var claimSequence = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestName,
                    "ClaimSequence", "Value");
                //logout and login as user without any of the claims work list authorities
                StringFormatter.PrintMessage(
                    "Logout and Login as user without any of the claims work list authorities");
                _claimSearch.Logout().LoginAsUserHavingNoAnyAuthority().NavigateToClaimSearch();
                _claimSearch.GetSideBarPanelSearch.IsSideBarPanelOpen()
                    .ShouldBeTrue("Side Bar Panel must be opened by default");
                _claimSearch.GetSideBarPanelSearch.IsWorkListIconPresent().ShouldBeFalse("Is Work List Icon display?");
                _claimSearch.GetSideBarPanelSearch.GetTopHeaderName()
                    .ShouldBeEqual("Find Claim", "Search Panel Header Name Should equal");
                //_claimSearch.GetSideBarPanelSearch.GetSectionHeaderName().ShouldBeEqual("Find Claim", "Search Panel Section Header Name Should equal.");
                _claimSearch.GetSideBarPanelSearch.IsSwitchWorkListIconDisplayed()
                    .ShouldBeFalse("Is Switch work list icon display?");
                _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel("Quick Search")
                    .ShouldBeEqual(ClaimQuickSearchTypeEnum.AllClaims.GetStringValue(),
                        "Default value of Quick Search should be equal");
                _claimSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Claim Sequence", claimSequence);
                var newClaimAction = _claimSearch.ClickOnFindButtonAndNavigateToClaimAction();
                newClaimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                _claimSearch.GetPageHeader()
                    .ShouldBeEqual("Claim Action", "Page should redirect to Claim Action page.");
                newClaimAction.IsNextButtonDisabled().ShouldBeFalse("Is Next button disable?");
                newClaimAction.ClickOnNextButton();
                newClaimAction.GetPageHeader().ShouldBeEqual(PageHeaderEnum.ClaimSearch.GetStringValue(),
                    "Page should redirect to Claim Search page.");
                _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel("Claim Sequence")
                    .ShouldBeEqual(claimSequence, "Search criteria must retain.");
                _claimSearch.GetGridViewSection.GetGridRowCount()
                    .ShouldBeEqual(1, "Claim Search Result row Should retain.");
                _claimSearch.GetClaimSeqListOnSearchResult()[0]
                    .ShouldBeEqual(claimSequence, "Correct search result should display");
            }
        }

        [Test] //US68072 + CAR-1408+TE-451+TE-556 + CAR2304(CAR-2135) + TE-835 + CAR-3052
        [Retrying(Times = 3)]
        public void Verify_basic_search_filters_for_Flagged_Claims_in_Quick_Search_option()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ClaimSearchPage _claimSearch;
                _claimSearch = automatedBase.QuickLaunch.NavigateToClaimSearch();
                var TestName = new StackFrame(true).GetMethod().Name;
                List<string> _lineOfBusinessList = _claimSearch.GetCommonSql.GetLineOfBusiness();
                List<string> _smtstReviewGroup = _claimSearch.GetReviewGroup(ClientEnum.SMTST.ToString());
                List<string> _activeProductListForClient = (List<string>)_claimSearch.GetActiveProductListForClient(ClientEnum.SMTST.ToString());
                List<string> _allProductListFromDB = _claimSearch.GetCommonSql.GetAllProductList();
                List<string> _smtstPlanList = _claimSearch.GetAssociatedPlansForClient(ClientEnum.SMTST.ToString());
                List<string> _flagsList = _claimSearch.GetAllFlagsFromDatabase();
                int _flaggedClaimsCountInternalUser = Convert.ToInt16(_claimSearch.GetFlaggedClaimList(ClientEnum.SMTST.ToString())[0]);
                var expectedProductTypeList = GetExpectedProductListFromDatabase(_allProductListFromDB);
                var claimstatusList = automatedBase.DataHelper.GetMappingData(FullyQualifiedClassName, "flagged_claims_status").Values
                    .ToList();
                var appealStatusList =
                    automatedBase.DataHelper.GetMappingData(FullyQualifiedClassName, "appeal_status").Values.ToList();
                _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel("Quick Search")
                    .ShouldBeEqual(ClaimQuickSearchTypeEnum.AllClaims.GetStringValue(),
                        "Quick Search option defaults to All Claims");
                _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                    ClaimQuickSearchTypeEnum.FlaggedClaims.GetStringValue());
                Verify_correct_search_filter_options_displayed_for("flagged_claims",
                    ClaimQuickSearchTypeEnum.FlaggedClaims.GetStringValue());
                StringFormatter.PrintMessageTitle("Claim Status field Validation");
                ValidateClaimStatusforFlaggedClaims("Claim Status");
                StringFormatter.PrintMessageTitle("Product field Validation");
                ValidateSingleDropDownForDefaultValueAndExpectedList("Product", expectedProductTypeList);
                StringFormatter.PrintMessageTitle("Plan field Validation");
                ValidateFieldSupportingMultipleValues("Plan", _smtstPlanList);
                StringFormatter.PrintMessageTitle("Line of Business Field Validation");
                ValidateFieldSupportingMultipleValues("Line Of Business", _lineOfBusinessList);
                #region CAR-3052
                StringFormatter.PrintMessageTitle("Flag Field Validation");
                ValidateFieldSupportingMultipleValues("Flag", _flagsList);
                #endregion
                StringFormatter.PrintMessageTitle("Batch ID Validation");
                ValidateBatchIDforFlaggedClaims("Batch ID");
                StringFormatter.PrintMessageTitle("Review Group Validation");
                ValidateFieldSupportingMultipleValues("Review Group", _smtstReviewGroup);
                StringFormatter.PrintMessageTitle("Appeal Status");
                ValidateDropdownListWithMappingData("Appeal Status", "appeal_status");

                _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel("Member ID")
                    .ShouldBeNullorEmpty("Member Id  should be empty by default");
                _claimSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Member ID",
                    Concat(Enumerable.Repeat("A123", 26)));
                _claimSearch.GetSideBarPanelSearch.GetLengthOfTheInputFieldByLabel("Member ID")
                    .ShouldBeEqual(100, "Maximum allowed value 5 ?");
                _claimSearch.GetSideBarPanelSearch.ClickOnClearLink();

                //TE-556
                StringFormatter.PrintMessage("Verify Proc Code");
                _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel("Proc")
                    .ShouldBeNullorEmpty("Proc should be empty by default");
                _claimSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Proc", Concat(Enumerable.Repeat("A123", 21)));
                _claimSearch.GetSideBarPanelSearch.GetLengthOfTheInputFieldByLabel("Proc")
                    .ShouldBeEqual(5, "Maximum allowed value 5 ?");
                _claimSearch.GetSideBarPanelSearch.ClickOnClearLink();

                _claimSearch.GetSideBarPanelSearch.ClickOnFindButton();
                _claimSearch.WaitForWorking();
                _claimSearch.ClickOnLoadMore();
                var loadMoreValue = _claimSearch.GetLoadMoreText();
                var numbers = Regex.Split(loadMoreValue, @"\D+").Where(s => s != Empty).Select(m => int.Parse(m.Trim()))
                    .ToList();
                _flaggedClaimsCountInternalUser =
                    Convert.ToInt16(_claimSearch.GetFlaggedClaimList(ClientEnum.SMTST.ToString())[0]);
                numbers[1].ShouldBeEqual(_flaggedClaimsCountInternalUser,
                    "Unreviewed Claims count should match with the value from database");
                if (Enumerable.Range(1, 49).Contains(numbers[0]))
                {
                    numbers[0].ShouldBeEqual
                    (_flaggedClaimsCountInternalUser,
                        "For claim count less than 50, clicking on Load more should equal the Flagged Claims count with the database");
                }
                else
                {
                    numbers[0].ShouldBeEqual(50, "The value should equal to 50");
                }

                var listOfClaimStatus = _claimSearch.GetClaimStatus().Distinct().ToList();
                claimstatusList.Add("Revised Claims");
                claimstatusList.Add("Logic Pend");
                listOfClaimStatus.CollectionShouldBeSubsetOf
                (claimstatusList,
                    "The Claim status of the search results should match with values from Claim Status dropdown ");
                var appealStatusAbb = new List<string> {"O', 'N', 'T', 'C", "O", "N", "T", "C"};
                if (_claimSearch.CurrentPageUrl.Contains("dev"))
                {
                    for (var j = 0; j < appealStatusList.Count; j++)
                    {
                        _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Appeal Status",
                            appealStatusList[j]);
                        _claimSearch.GetSideBarPanelSearch.ClickOnFindButton();
                        _claimSearch.WaitForWorking();
                        var claseqList = _claimSearch.GetGridViewSection.GetGridListValueByCol();
                        var claseqlist = claseqList.Select(x => x.Split('-')[0]).ToList();
                        var classublist = claseqList.Select(x => x.Split('-')[1]).ToList();
                        var newList = claseqlist.Zip(classublist, (s, i) => new {sv = "(" + s, iv = i + ")"}).ToList();
                        var newList1 = newList.Select(x => x.sv + "," + x.iv).ToList();
                        var finallist = Join(",", newList1.Select(x => x));
                        var appealCountList = _claimSearch.GetGridViewSection.GetAppealCountList();
                        _claimSearch.GetAppealCountByClaseqAppealStatus(finallist, appealStatusAbb[j])
                            .ShouldCollectionBeEqual(appealCountList, "Appeal Count Should be equal ");
                    }
                }

                void Verify_correct_search_filter_options_displayed_for(string mappingQuickSearchOptionName, string quickSearchValue)
                {

                    _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search", quickSearchValue);
                    _claimSearch.GetSideBarPanelSearch.GetSearchFiltersList()
                        .ShouldCollectionBeEqual(
                            automatedBase.DataHelper.GetMappingData(FullyQualifiedClassName, mappingQuickSearchOptionName).Values.ToList(), "Search Filters",
                            true);
                }

                void ValidateClaimStatusforFlaggedClaims(string label)
                {
                    _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel(label).ShouldBeEqual("All", label + " value defaults to All");
                    var expectedClaimStatusforFlaggedClaims = automatedBase.DataHelper.GetMappingData(FullyQualifiedClassName, "flagged_claims_status").Values.ToList();
                    var actualDropDownList = _claimSearch.GetSideBarPanelSearch.GetAvailableDropDownList(label);

                    actualDropDownList.ShouldCollectionBeEqual(expectedClaimStatusforFlaggedClaims, "Claim Status for Unreviewed Claims match");
                    _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel(label, actualDropDownList[1], false); //check for type ahead functionality
                    _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel(label, actualDropDownList[2]);
                    _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel(label).ShouldBeEqual(actualDropDownList[2], "User can select only a single option");
                    _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel(label, "All");

                }

                void ValidateFlagForFlaggedAndUnreviewedClaims(string label)
                {
                    _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel(label).ShouldBeEqual("All", label + " value defaults to All");

                    var actualDropDownList = _claimSearch.GetSideBarPanelSearch.GetAvailableDropDownList(label);
                    _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel(label, actualDropDownList[1], false); //check for type ahead functionality
                    _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel(label, actualDropDownList[2]);
                    _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel(label).ShouldBeEqual(actualDropDownList[2], "User can select only a single option");
                    _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel(label, "All");
                    VerifyFlagListBasedOnProduct();
                }

                void VerifyFlagListBasedOnProduct()
                {
                    var productListAbbreviation = GetExpectedProductListFromDatabase(_claimSearch.GetCommonSql.GetAllProductList(true));
                    var productList = GetExpectedProductListFromDatabase(_allProductListFromDB);
                    var i = 0;
                    foreach (var product in productListAbbreviation)
                    {
                        _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Product", productList[i]);
                        var flagList = _claimSearch.GetSideBarPanelSearch.GetAvailableDropDownList("Flag");
                        flagList.RemoveAt(0);
                        if (product == "All")
                        {
                            flagList.ShouldCollectionBeEqual(_claimSearch.GetAllFlagsFromDatabase(), "Flag List Should Match when All is selected");
                        }
                        else
                        {
                            Enum.TryParse(product, out ProductEnum productName);
                            flagList.ShouldCollectionBeEqual(_claimSearch.GetActiveFlagsForProductFromDatabase(productName.GetStringDisplayValue()), "Flag List Should Match");
                        }
                        _claimSearch.GetSideBarPanelSearch.ClickOnClearLink();
                        i++;
                    }
                }
                void ValidateDropdownListWithMappingData(string label, string mappingOptionName)
                {
                    _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel(label).ShouldBeEqual("All", label + " value defaults to All");
                    var expectedMappingList = automatedBase.DataHelper.GetMappingData(FullyQualifiedClassName, mappingOptionName).Values.ToList();
                    var actualDropDownList = _claimSearch.GetSideBarPanelSearch.GetAvailableDropDownList(label);
                    actualDropDownList.Count.ShouldBeGreater(0, "List of " + label + " is greater than zero.");
                    actualDropDownList.ShouldCollectionBeEqual(expectedMappingList, (label + " List match with" + mappingOptionName));
                }

                void ValidateBatchIDforFlaggedClaims(string label)
                {
                    var expecteBatchIDList = _claimSearch.GetAssociatedBatchList(ClientEnum.SMTST.ToString());
                    var actualBatchIDList = _claimSearch.GetSideBarPanelSearch.GetAvailableDropDownList(label);
                    actualBatchIDList.ShouldCollectionBeEqual(expecteBatchIDList, "Batch Id List Shoul Equal");
                    _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel("Batch ID")
                        .ShouldBeEqual(expecteBatchIDList[0], ("The default " + label + "should be" + expecteBatchIDList[0]));
                    _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel(label, actualBatchIDList[1], false); //check for type ahead functionality
                    _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel(label, actualBatchIDList[0]);
                    _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel(label).ShouldBeEqual(actualBatchIDList[0], "User can select only a single option");
                }

                List<String> GetExpectedProductListFromDatabase(List<string> allProductList)
                {
                    List<string> expectedProductTypes = new List<string>();

                    for (int i = 0; i < _activeProductListForClient.Count; i++)
                    {
                        if (_activeProductListForClient[i] == "T")
                        {
                            expectedProductTypes.Add(allProductList[i]);
                        }
                    }

                    expectedProductTypes.Insert(0, "All");
                    return expectedProductTypes;
                }

                void ValidateFieldSupportingMultipleValues(string label, IList<string> expectedDropDownList)
                {
                    ValidateMultipleDropDownForDefaultValueAndExpectedList(label, expectedDropDownList);
                    _claimSearch.GetSideBarPanelSearch.SelectSearchDropDownListForMultipleSelectValue(label, expectedDropDownList[0]);
                    _claimSearch.GetSideBarPanelSearch.GetInputAttributeValueByLabel(label, "placeholder")
                        .ShouldBeEqual(expectedDropDownList[0], label + "single value selected");
                    _claimSearch.GetSideBarPanelSearch.SelectSearchDropDownListForMultipleSelectValue(label, expectedDropDownList[expectedDropDownList.Count - 1]);
                    _claimSearch.GetSideBarPanelSearch.GetInputAttributeValueByLabel(label, "placeholder")
                        .ShouldBeEqual("Multiple values selected", label + "multiple value selected");
                    _claimSearch.GetSideBarPanelSearch.SelectSearchDropDownListForMultipleSelectValue(label, "Clear");

                }

                void ValidateMultipleDropDownForDefaultValueAndExpectedList(string label, IList<string> collectionToEqual)
                {
                    var listedOptionsList = _claimSearch.GetSideBarPanelSearch.GetMultiSelectAvailableDropDownList(label);
                    listedOptionsList.Count.ShouldBeGreater(0, "List of " + label + " is greater than zero.");
                    _claimSearch.GetSideBarPanelSearch.GetMultiSelectListedDropDownList(label).Contains("All")
                        .ShouldBeTrue(
                            "A value of all displayed at the top of the list");

                    listedOptionsList.ShouldCollectionBeEqual(collectionToEqual, label + " List As Expected , followed by options sorted alphabetically.");

                    listedOptionsList.IsInAscendingOrder().ShouldBeTrue(label + " should be sorted in alphabetical order.");
                    _claimSearch.GetSideBarPanelSearch.GetInputAttributeValueByLabel(label, "placeholder")
                        .ShouldBeEqual("Select one or more", label + " value defaults to 'select one or more'");
                }

                void ValidateSingleDropDownForDefaultValueAndExpectedList(string label, IList<string> collectionToEqual, bool order = true)
                {
                    var actualDropDownList = _claimSearch.GetSideBarPanelSearch.GetAvailableDropDownList(label);
                    actualDropDownList.Count.ShouldBeGreater(0, "List of " + label + " is greater than zero.");
                    if (collectionToEqual != null)
                        actualDropDownList.ShouldCollectionBeEqual(collectionToEqual, label + " List As Expected");
                    if (order)
                    {
                        actualDropDownList.Remove("All");
                        actualDropDownList.IsInAscendingOrder().ShouldBeTrue(label + " should be sorted in alphabetical order.");
                    }
                    _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel(label).ShouldBeEqual("All", label + " value defaults to All");
                    _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel(label, actualDropDownList[0], false); //check for type ahead functionality
                    _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel(label, actualDropDownList[1]);
                    _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel(label).ShouldBeEqual(actualDropDownList[1], "User can select only a single option");
                    _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel(label, "All");
                }
            }
        }

        [Test] //US68072 + TE-556 + CAR-1716 + CAR-3052 + CAR-3052 [CAR-3179]
        [Retrying(Times = 3)]
        public void Verify_search_results_with_extra_search_filters_are_correct_flagged_claims()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ClaimSearchPage _claimSearch;
                _claimSearch = automatedBase.QuickLaunch.NavigateToClaimSearch();
                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestName);
                var validMemberid = paramLists["MemberId"].Split(',')[0];
                var invalidMemberid = paramLists["MemberId"].Split(',')[1];
                var clientName = paramLists["ClientName"];
                var flags = paramLists["Flag"].Split(';').ToList();
                var username = paramLists["Uname"];
                _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel
                    ("Quick Search", ClaimQuickSearchTypeEnum.FlaggedClaims.GetStringValue());
                _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel
                    ("Batch ID", paramLists["BatchID"]);
                _claimSearch.GetSideBarPanelSearch.ClickOnFindButton();
                _claimSearch.WaitForWorking();
                _claimSearch.GetInternalUserClaimSeqFlaggedClaims()
                    .ShouldBeEqual(paramLists["ClaimSeq"], "Claim Sequence");
                _claimSearch.ClickSearchResultRow();
                _claimSearch.GetBatchId().ShouldBeEqual(paramLists["BatchID"], "Batch ID");
                _claimSearch.GetLineOfBusinessfromClaimDetails()
                    .ShouldBeEqual(paramLists["LineofBusiness"], "Line Of Business");
                _claimSearch.GetSideBarPanelSearch.ClickOnClearLink();


                StringFormatter.PrintMessage("verify Correct details displayed when searched using member id");
                _claimSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Member ID", validMemberid);
                _claimSearch.GetSideBarPanelSearch.ClickOnFindButton();
                _claimSearch.WaitForWorking();
                _claimSearch.GetMemberId().ShouldBeEqual(validMemberid, "Member id displayed correct?");

                StringFormatter.PrintMessage("verify No Matching record found message displayed for invalid MemberId");
                _claimSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Member ID", invalidMemberid);
                _claimSearch.GetSideBarPanelSearch.ClickOnFindButton();
                _claimSearch.WaitForWorking();
                _claimSearch.GetSideBarPanelSearch.IsNoMatchingRecordFoundMessagePresent()
                    .ShouldBeTrue("Message displayed if no records found?");
                _claimSearch.GetSideBarPanelSearch.ClickOnClearLink();

                //TE-451 + TE-556
                StringFormatter.PrintMessage("Verify Results when multiple Review Groups and Proc is selected");
                _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                    ClaimQuickSearchTypeEnum.FlaggedClaims.GetStringValue());
                _claimSearch.GetSideBarPanelSearch.SelectMultipleValuesInMultiSelectDropdownList("Review Group",
                    paramLists["ReviewGroup"].Split(',').ToList());
                _claimSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Proc", paramLists["ValidProc"]);
                _claimSearch.GetSideBarPanelSearch.ClickOnFindButton();
                _claimSearch.WaitForWorking();
                _claimSearch.GetGridViewSection.GetGridListValueByCol().ShouldCollectionBeEqual(
                    _claimSearch.GetFlaggedClaimForReviewGroupInternaList(paramLists["ValidProc"]), "Results equal?");
                _claimSearch.GetSideBarPanelSearch.ClickOnClearLink();

                StringFormatter.PrintMessage("Verify Search Cannot Be Instantiated With Invalid Proc Code");
                _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                    ClaimQuickSearchTypeEnum.FlaggedClaims.GetStringValue());
                _claimSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Proc", paramLists["InvalidProc"]);
                _claimSearch.GetSideBarPanelSearch.ClickOnFindButton();
                _claimSearch.WaitForWorking();
                _claimSearch.IsPageErrorPopupModalPresent()
                    .ShouldBeTrue(
                        "Search Should Not Be Initiated with invalid proc code");
                _claimSearch.GetPageErrorMessage().ShouldBeEqual("Selected Proc Code is invalid. Please search again.",
                    "Verify the popup message");
                _claimSearch.ClosePageError();
                _claimSearch.GetSideBarPanelSearch.ClickOnClearLink();

                StringFormatter.PrintMessage(
                    "Verify No Matching Records Found message when no results found for selected proc code");
                _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                    ClaimQuickSearchTypeEnum.FlaggedClaims.GetStringValue());
                _claimSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Proc", paramLists["Proc"]);
                _claimSearch.GetSideBarPanelSearch.ClickOnFindButton();
                _claimSearch.WaitForWorking();
                _claimSearch.GetNoMatchingRecordFoundMessage()
                    .ShouldBeEqual("No matching records were found.", "Warning message should be displayed");
                _claimSearch.GetSideBarPanelSearch.ClickOnClearLink();

                #region CAR-1716 + CAR-3052 [CAR-3179]

                _claimSearch.IsFlagLabelPresent().ShouldBeTrue("Is Flag Label present above Batch ID?");
                _claimSearch.GetSideBarPanelSearch.ClickOnToggleIcon("Flag");
                _claimSearch.IsFlagDropDownListAscending().ShouldBeTrue("Is Flag dropdown is ascending?");
                _claimSearch.GetSideBarPanelSearch.SelectMultipleValuesInMultiSelectDropdownList("Flag", flags);
                _claimSearch.GetSideBarPanelSearch.ClickOnFindButton();
                _claimSearch.WaitForWorking();
                _claimSearch.CloseAnyPopupIfExist();

                if (_claimSearch.GetGridViewSection.IsLoadMorePresent())
                {
                    var loadMoreValue = _claimSearch.GetLoadMoreText();
                    var numbers = Regex.Split(loadMoreValue, @"\D+").Where(s => s != Empty)
                        .Select(m => int.Parse(m.Trim())).ToList();
                    numbers[1].ToString().ShouldBeEqual(
                        _claimSearch.GetCountOfFlaggedClaimsByFlagName(clientName, username, $"'{flags[0]}','{flags[1]}'"),
                        "Count of result including claim records where there is at least one active flag matching the search criteria on the claims should match with database");
                }
                else
                {
                    _claimSearch.GetGridViewSection.GetRecordRowsCountFromPage().ShouldBeEqual(
                        _claimSearch.GetCountOfFlaggedClaimsByFlagName(clientName, username, $"'{flags[0]}',{flags[1]}"),
                        "Count of result including claim records where there is at least one active flag matching the search criteria on the claims should match with database");
                }

                #endregion

                #endregion
            }
        }

        [Test]//US66665
        [Retrying(Times = 3)]
        public void Validate_nucleus_worklist_is_ordered_by_claim_due_date_client_received_date_claim_seq_ascendingly()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ClaimSearchPage _claimSearch;
                _claimSearch = automatedBase.QuickLaunch.NavigateToClaimSearch();
                try
                {
                    var newClaimAction =
                        _claimSearch.SearchByClaimSequenceToNavigateToClaimActionPage("1458012-0", true);
                    newClaimAction.ClickWorkListIcon();
                    _claimSearch.CreatePciWorklistForUnreviewedClaim();
                    newClaimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                    var claimSeqList = new List<string> {newClaimAction.GetClaimSequence()};
                    _claimSearch = newClaimAction.ClickClaimSearchIcon();
                    claimSeqList.AddRange(newClaimAction.GetListOfNextClaimsInWorklistSectionDisplayed());
                    var dbClaSeqList = _claimSearch.GetListOfClaimsForPciWorklist(claimSeqList);
                    var dueDateList = dbClaSeqList
                        .Select(list => list[1] != "" ? DateTime.Parse(list[1]) : (DateTime?) null).ToList();
                    var clientRecvDate = dbClaSeqList
                        .Select(list => list[2] != "" ? DateTime.Parse(list[2]) : (DateTime?) null).ToList();
                    var clasSeqList = dbClaSeqList
                        .Select(list => list[0] != "" ? Int64.Parse(list[0].Split('-')[0]) : (Int64?) null).ToList();
                    if (dueDateList.Any(same => same != dueDateList[0]))
                    {
                        _claimSearch.IsListInAscendingOrder(dueDateList)
                            .ShouldBeTrue("List should be sorted in ascending order of due date.");
                    }
                    else if (clientRecvDate.Any(same => same != clientRecvDate[0]))
                    {
                        _claimSearch.IsListInAscendingOrder(clientRecvDate)
                            .ShouldBeTrue("List should be soreted in ascending order of client received date.");
                    }
                    else
                    {
                        _claimSearch.IsListInAscendingOrder(clasSeqList)
                            .ShouldBeTrue("List should be soreted in ascending order of claim sequence value.");
                    }

                    clasSeqList.ShouldCollectionBeEqual(
                        claimSeqList.Select(list => list != null ? Int64.Parse(list.Split('-')[0]) : (Int64?) null)
                            .ToList(),
                        "The worklist order should be sorted in the basis of due date, client received date and claseq in  ascending order.");
                }
                finally
                {
                    if (!_claimSearch.GetSideBarPanelSearch.GetTopHeaderName().Equals("Find Claim"))
                        _claimSearch.ClickOnFindClaimIcon();
                    _claimSearch.GetSideBarPanelSearch.ClickOnClearLink();
                }
            }
        }

        [Test]//US68334+CAR-1395
        [Retrying(Times = 3)]
        public void Verify_Search_list_sorting_for_Internal_Users()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ClaimSearchPage _claimSearch;
                _claimSearch = automatedBase.QuickLaunch.NavigateToClaimSearch();
                var TestName = new StackFrame(true).GetMethod().Name;
                var filterOptions =
                    automatedBase.DataHelper.GetMappingData(FullyQualifiedClassName, "claim_sorting_option_list_internal").Values
                        .ToList();
                var claimNum = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestName,
                    "ClaimNo", "Value");
                var claimNoForAppealCount = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestName,
                    "ClaimNoForAppealCount", "Value");
                try
                {
                    _claimSearch.IsFilterOptionPresent().ShouldBeTrue("Is Filter Option Icon Present?");
                    _claimSearch.GetFilterOptionList()
                        .ShouldCollectionBeEqual(filterOptions, "Filter Options Lists Collection Should Equal");
                    _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                        ClaimQuickSearchTypeEnum.AllClaims.GetStringValue());
                    _claimSearch.SearchByClaimNmber(claimNum);
                    _claimSearch.GetSearchResultCount().ShouldBeGreater(0, "Search Result must be listed.");
                    var expectedDescendingSortedClaimSequence =
                        _claimSearch.GetClaimSequenceByClaimNoInDescendingOrder(claimNum);
                    var expectedAscendingSortedClaimSequence =
                        _claimSearch.GetClaimSequenceByClaimNoInAscendingOrder(claimNum);
                    _claimSearch.GetClaimSeqListOnSearchResult().ShouldCollectionBeEqual(
                        expectedDescendingSortedClaimSequence,
                        "Default sorting of claim search result should be by claim sequence in descending order.");
                    StringFormatter.PrintMessageTitle("Validation of Sorting by Claim Sequence");
                    _claimSearch.ClickOnFilterOptionListRow(1);
                    _claimSearch.GetClaimSeqListOnSearchResult().ShouldCollectionBeEqual(
                        expectedAscendingSortedClaimSequence,
                        "Claim Sequence and Claim sub should be sorted in Ascending Order");
                    _claimSearch.ClickOnFilterOptionListRow(1);
                    _claimSearch.GetClaimSeqListOnSearchResult().ShouldCollectionBeEqual(
                        expectedDescendingSortedClaimSequence,
                        "Claim Sequence and Claim sub should be sorted in Descending Order");
                    ValidateClaimSearchRowSorted(8, 7, "Claim Status");
                    _claimSearch.GetSideBarPanelSearch.ClickOnClearLink();
                    _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                        ClaimQuickSearchTypeEnum.PendedClaims.GetStringValue());
                    _claimSearch.GetSideBarPanelSearch.SelectSearchDropDownListForMultipleSelectValue(
                        "Claim Sub Status",
                        "Revised Claims");
                    _claimSearch.GetSideBarPanelSearch.ClickOnFindButton();
                    _claimSearch.WaitForWorkingAjaxMessage();
                    ValidateClaimSearchRowSorted(3, 2, "Flags");
                    ValidateClaimSearchRowSorted(4, 3, "Form Type");
                    ValidateClaimSearchRowSorted(5, 4, "Provider Sequence");
                    ValidateClaimSearchRowSorted(7, 6, "Savings");
                    ValidateClaimSearchRowSorted(6, 5, "Member ID");

                    #region CAR-1395

                    if (_claimSearch.CurrentPageUrl.Contains("dev") || _claimSearch.CurrentPageUrl.Contains("qa"))
                    {
                        _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                            ClaimQuickSearchTypeEnum.AllClaims.GetStringValue());
                        _claimSearch.SearchByClaimNmber(claimNoForAppealCount);
                        _claimSearch.ClickOnFilterOptionListRow(8);
                        _claimSearch.GetSearchResultListByCol(9).IsInAscendingOrder()
                            .ShouldBeTrue("Is Appeal Count in ascending order?");
                        _claimSearch.ClickOnFilterOptionListRow(8);
                        _claimSearch.GetSearchResultListByCol(9).IsInDescendingOrder()
                            .ShouldBeTrue("Is Appeal Count in descending order?");
                    }

                    #endregion

                    _claimSearch.SearchByClaimNmber(claimNum);
                    _claimSearch.ClickOnFilterOptionListRow(9);
                    StringFormatter.PrintMessage("Clicked on Clear Sort.");
                    _claimSearch.GetClaimSeqListOnSearchResult().ShouldCollectionBeEqual(
                        expectedDescendingSortedClaimSequence,
                        "All sorting is cleared and search list is sorted by Claim Sequence which is default sort.");
                }
                finally
                {
                    _claimSearch.CaptureScreenShot("Failed reason for Failing claimSearch Test");
                    _claimSearch.ClickOnFilterOptionListRow(9);
                    StringFormatter.PrintMessage("Clicked on Clear Sort.");

                }

                void ValidateClaimSearchRowSorted(int col, int sortOptionRow, string colName)
                {
                    _claimSearch.ClickOnFilterOptionListRow(sortOptionRow);
                    switch (colName)
                    {
                        case "Claim Sequence":
                            _claimSearch.IsListClaimSeqSortedInAscendingOrder(col)
                           .ShouldBeTrue(Format("{0} Should sorted in Ascending Order", colName));
                            _claimSearch.ClickOnFilterOptionListRow(sortOptionRow);
                            _claimSearch.IsListClaimSeqSortedInDescendingOrder(col)
                                .ShouldBeTrue(Format("{0} Should sorted in Descending Order", colName));
                            break;

                        case "Savings":
                            _claimSearch.IsListInAscendingOrderBySavings(2)
                           .ShouldBeTrue(Format("{0} Should sorted in Ascending Order", colName));
                            _claimSearch.ClickOnFilterOptionListRow(sortOptionRow);
                            _claimSearch.IsListInADecendingOrderBySavings(2)
                                .ShouldBeTrue(Format("{0} Should sorted in Descending Order", colName));
                            break;
                        case "Provider Sequence":
                            _claimSearch.IsProvSeqinAscendingOrder()
                           .ShouldBeTrue(Format("{0} Should sorted in Ascending Order", colName));
                            _claimSearch.ClickOnFilterOptionListRow(sortOptionRow);
                            _claimSearch.IsProvSeqinDescendingOrder()
                                .ShouldBeTrue(Format("{0} Should sorted in Descending Order", colName));
                            break;
                        case "Form Type":
                            _claimSearch.IsClaimSearchListInReferenceToProvSeqInAcseningOrder().ShouldBeTrue(Format("{0} Should sorted in Ascending Order", colName));
                            _claimSearch.ClickOnFilterOptionListRow(sortOptionRow);
                            _claimSearch.IsClaimSearchListInReferenceToProvSeqInDescendingOrder()
                                .ShouldBeTrue($"{colName} Should sorted in Descending Order");
                            break;
                        case "Member ID":
                            _claimSearch.IsClaimSearchResultListForMemIdInAcseningOrder().IsInAscendingOrder().ShouldBeTrue(Format("{0} Should sorted in Ascending Order", colName));
                            _claimSearch.ClickOnFilterOptionListRow(sortOptionRow);
                            _claimSearch.IsClaimSearchResultListForMemIdInAcseningOrder().IsInDescendingOrder()
                                .ShouldBeTrue($"{colName} Should sorted in Descending Order");
                            break;
                        default:
                            _claimSearch.IsListStringSortedInAscendingOrder(col)
                           .ShouldBeTrue($"{colName} Should sorted in Ascending Order");
                            _claimSearch.ClickOnFilterOptionListRow(sortOptionRow);
                            _claimSearch.IsListStringSortedInDescendingOrder(col)
                                .ShouldBeTrue($"{colName} Should sorted Descending Order");
                            break;
                    }
                }
            }
        }

        [Test]//US68396
        [Retrying(Times = 3)]
        public void Verify_Sort_order_retains_when_return_to_claim_search()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ClaimSearchPage _claimSearch;
                _claimSearch = automatedBase.QuickLaunch.NavigateToClaimSearch();
                var TestName = new StackFrame(true).GetMethod().Name;
                var claimNum = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestName,
                    "ClaimNo", "Value");
                _claimSearch.SearchByClaimNmber(claimNum.Split(',')[0]);
                StringFormatter.PrintMessage("Sort by Claim  Status");
                try
                {
                    _claimSearch.ClickOnFilterOptionListRow(6);
                    _claimSearch.IsListInAscendingOrderBySavings(2)
                        .ShouldBeTrue("Is Search Result Sorted in ascending Order by Saving?");
                    var newClaimAction = _claimSearch.ClickOnClaimSequenceOfSearchResult(1);
                    newClaimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                    newClaimAction.GetPageHeader().ShouldBeEqual(PageHeaderEnum.ClaimAction.GetStringValue(),
                        "Page Should Navigate to Claim Action");
                    newClaimAction.ClickClaimSearchIcon();
                    _claimSearch.GetPageHeader().ShouldBeEqual(PageHeaderEnum.ClaimSearch.GetStringValue(),
                        "Page Should navigate back to Claim Search page.");
                    _claimSearch.IsListInAscendingOrderBySavings(2)
                        .ShouldBeTrue("Is Saving sorting retained after return back to Claim Search page?");
                    _claimSearch.SearchByClaimNmber(claimNum.Split(',')[1]);
                    _claimSearch.IsListInAscendingOrderBySavings(2)
                        .ShouldBeTrue("Is Saving sorting retained after next search?");
                }
                finally
                {
                    _claimSearch.ClickOnFilterOptionListRow(8);
                    StringFormatter.PrintMessage("Clicked on Clear Sort.");
                }
            }
        }

        [Test, NUnit.Framework.Category("AppealDependent")] //US68645
        [Retrying(Times = 3)]
        public void Verify_correct_dropdown_values_are_displayed_even_after_client_switch_through_Appeal()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ClaimSearchPage _claimSearch;
                _claimSearch = automatedBase.QuickLaunch.NavigateToClaimSearch();
                var TestName = new StackFrame(true).GetMethod().Name;
                List<string> _smtstSubStatusList = _claimSearch.GetAssociatedClaimSubStatusForInternalUser(ClientEnum.SMTST.ToString());
                List<string> _smtstPlanList = _claimSearch.GetAssociatedPlansForClient(ClientEnum.SMTST.ToString());
                List<string> _cvtyPlanList = _claimSearch.GetAssociatedPlansForClient(ClientEnum.CVTY.GetStringDisplayValue());
                List<string> _claimsAssignedToUserList = _claimSearch.GetAssignedToList(ClientEnum.SMTST.ToString());
                List<string> _cvtyClaimsAssignedToUserList = _claimSearch.GetAssignedToList(ClientEnum.CVTY.GetStringDisplayValue());
                List<string> _smtstReviewGroup = _claimSearch.GetReviewGroup(ClientEnum.SMTST.ToString());
                List<string> _cvtyReviewGroup = _claimSearch.GetReviewGroup(ClientEnum.SMTST.ToString());
                List<string> _smtstBatchList = _claimSearch.GetAssociatedBatchList(ClientEnum.SMTST.ToString());
                _smtstBatchList.Insert(0, "All");
                List<string> _cvtyBatchList = _claimSearch.GetAssociatedBatchList(ClientEnum.CVTY.GetStringDisplayValue());
                _cvtyBatchList.Insert(0, "All");

                var cvtySubStatusList =
                    _claimSearch.GetAssociatedClaimSubStatusForInternalUserWithDCIInactive(
                        ClientEnum.CVTY.GetStringDisplayValue());
                _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                    ClaimQuickSearchTypeEnum.UnreviewedClaims.GetStringValue());
                _claimSearch.GetSideBarPanelSearch.GetAvailableDropDownList("Batch ID")
                    .ShouldCollectionBeEqual(_smtstBatchList,
                        "Correct Batch ID list should be displayed for SMTST client.");
                _claimSearch.GetSideBarPanelSearch.GetMultiSelectAvailableDropDownList("Review Group")
                    .ShouldCollectionBeEqual(_smtstReviewGroup,
                        "Correct Review Group list should be displayed for SMTST client.");
                _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                    ClaimQuickSearchTypeEnum.PendedClaims.GetStringValue());
                ValidateAssignedTo("Assigned To", ClientEnum.SMTST.ToString());
                _claimSearch.GetSideBarPanelSearch.GetMultiSelectAvailableDropDownList("Plan")
                    .ShouldCollectionBeEqual(_smtstPlanList, "Correct Plan list should be displayed for SMTST client.");
                _claimSearch.GetSideBarPanelSearch.GetMultiSelectAvailableDropDownList("Claim Sub Status")
                    .ShouldCollectionBeEqual(_smtstSubStatusList,
                        "Correct Claim Sub Status list should be displayed for SMTST client.");
                var newAppealSearch = automatedBase.QuickLaunch.NavigateToAppealSearch();
                _claimSearch.MouseOutAppealMenu();
                _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                    AppealQuickSearchTypeEnum.AllAppeals.GetStringValue());
                newAppealSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Client",
                    ClientEnum.CVTY.GetStringDisplayValue());
                newAppealSearch.ClickOnAdvancedSearchFilterIcon(true);
                newAppealSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Status", "New");
                newAppealSearch.ClickOnFindButton();
                var newAppealAction = newAppealSearch.ClickOnAppealSequenceToNavigateAppealActionPageByRow(2);
                newAppealAction.CloseAnyPopupIfExist();
                newAppealAction.IsDefaultTestClientForEmberPage(ClientEnum.CVTY);
                automatedBase.CurrentPage = _claimSearch = newAppealAction.NavigateToClaimSearch();
                StringFormatter.PrintMessageTitle("Validation of Correct dropdown values after client switch.");
                _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                    ClaimQuickSearchTypeEnum.UnreviewedClaims.GetStringValue());
                _claimSearch.GetSideBarPanelSearch.GetAvailableDropDownList("Batch ID")
                    .ShouldCollectionBeEqual(_cvtyBatchList,
                        "Correct Batch ID list should be displayed for CVTY client.");
                _claimSearch.GetSideBarPanelSearch.GetMultiSelectAvailableDropDownList("Review Group")
                    .ShouldCollectionBeEqual(_cvtyReviewGroup,
                        "Correct Review Group list should be displayed for CVTY client.");
                _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                    ClaimQuickSearchTypeEnum.PendedClaims.GetStringValue());
                ValidateAssignedTo("Assigned To", ClientEnum.CVTY.GetStringDisplayValue());
                _claimSearch.GetSideBarPanelSearch.GetMultiSelectAvailableDropDownList("Plan")
                    .ShouldCollectionBeEqual(_cvtyPlanList, "Correct Plan list should be displayed for CVTY client.");
                _claimSearch.GetSideBarPanelSearch.GetMultiSelectAvailableDropDownList("Claim Sub Status")
                    .ShouldCollectionBeEqual(cvtySubStatusList,
                        "Correct Claim Sub Status list should be displayed for CVTY client.");

                void ValidateAssignedTo(string label, string client)
                {
                    _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel(label).ShouldBeEqual("All", label + " value defaults to All");
                    var reqDropDownList = _claimSearch.GetSideBarPanelSearch.GetAvailableDropDownList(label);
                    reqDropDownList.Remove("All");
                    if (client == ClientEnum.SMTST.ToString())
                    {
                        _claimsAssignedToUserList.Sort();
                        reqDropDownList.ShouldCollectionBeEqual(_claimsAssignedToUserList, "Validate Assigned To List with the database");
                    }
                    else if (client == ClientEnum.CVTY.GetStringDisplayValue())
                    {
                        _cvtyClaimsAssignedToUserList.Sort();
                        reqDropDownList.ShouldCollectionBeEqual(_cvtyClaimsAssignedToUserList, "Validate Assigned To List with the database");
                    }
                    reqDropDownList[0].DoesNameContainsOnlyFirstWithLastname()
                        .ShouldBeTrue(label + " should be in proper format of <firstname> <lastname> (user id)");

                }
            }
        }

        
        [Test] //US69154
        //[Retry(3)]
        public void Verify_previously_viewed_claims_are_listed_in_find_claim_section()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ClaimSearchPage _claimSearch;
                _claimSearch = automatedBase.QuickLaunch.NavigateToClaimSearch();
                var TestName = new StackFrame(true).GetMethod().Name;

                automatedBase.CurrentPage.ClickOnQuickLaunch();
                automatedBase.QuickLaunch.NavigateToClaimSearch();

                var claimSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestName,
                    "ClaimSequence", "Value").Split(';');
                StringFormatter.PrintMessage("Previously Viewed Claims Verification");
                foreach (var claseq in claimSeq)
                {
                    var newClaimAction = _claimSearch.SearchByClaimSequenceToNavigateToClaimActionPage(claseq);
                    newClaimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                    _claimSearch = newClaimAction.ClickClaimSearchIcon();
                }

                var claseqList = claimSeq.Reverse().ToList();
                claseqList.RemoveAt(3);
                var previouslyViewedClaseq = _claimSearch.GetPreviouslyViewedClaimList();
                previouslyViewedClaseq.ShouldNotContain(claimSeq[0], "Only 3 recently viewed claims should be listed");
                previouslyViewedClaseq.ShouldCollectionBeEqual(claseqList,
                    "3 most recently viewed claims, with most recent on top should be listed in this section");
                var _claimAction = _claimSearch.ClickOnPreviouslyViewedClaimSequenceLinkByClaseq(claseqList[1]);
                _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                _claimAction.GetCurrentClaimSequence()
                    .ShouldBeEqual(claseqList[1],
                        "When Return to claim link for Claseq: " + claseqList[1] +
                        " is clicked , page should redirect to claim action page for that claim: " + claseqList[1]);
                _claimAction.ClickClaimSearchIcon();
            }
        }

        [Test]//US69158
        [Retrying(Times = 1)]
        [NonParallelizable]
        public void Validate_claim_lock_is_present_when_claim_is_viewed_by_other_user()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ClaimSearchPage _claimSearch;
                _claimSearch = automatedBase.QuickLaunch.NavigateToClaimSearch();
                var TestName = new StackFrame(true).GetMethod().Name;
                var claimSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestName,
                    "ClaimSequence", "Value");
                var claimNo = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestName,
                    "ClaimNo", "Value");
                try
                {
                    automatedBase.Login = _claimSearch.Logout();
                    automatedBase.QuickLaunch = automatedBase.Login.LoginAsHciAdminUser5();
                    automatedBase.CheckTestClientAndSwitch();
                    automatedBase.CurrentPage = _claimSearch = automatedBase.QuickLaunch.NavigateToClaimSearch();
                    var newClaimAction = _claimSearch.SearchByClaimSequenceToNavigateToClaimActionPage(claimSeq);
                    newClaimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                    automatedBase.CurrentPage = newClaimAction;
                    StringFormatter.PrintMessage(
                        Format("Open claim aciton page for claim sequence: {0} with direct URL ", claimSeq));
                    var newClaimActionUrl = automatedBase.CurrentPage.CurrentPageUrl;
                    newClaimAction =
                        _claimSearch.SwitchToOpenNewClaimActionByUrl(newClaimActionUrl,
                            automatedBase.EnvironmentManager.HciAdminUsername1);
                    newClaimAction.IsClaimLocked()
                        .ShouldBeTrue("Claim should be locked when it is in view mode by another user");
                    newClaimAction.GetLockIConTooltip()
                        .ShouldBeEqual(
                            "This claim has been opened in view mode. It is currently locked by Test Automation5 (ui_automation5).",
                            "Is Lock Message Equal?");
                    newClaimAction.IsAddAppealIconDisabled()
                        .ShouldBeTrue(
                            "Create Appeal Should disabled for claims that has appeal whose status is not complete or closed");
                    newClaimAction.GetToolTipMessageDisabledCreateAppealIcon()
                        .ShouldBeEqual(
                            "Claim is currently locked by Test Automation5 (ui_automation5). An appeal cannot be opened while the claim is being edited by another user.",
                            "Tooltip Message on add appeal icon when locked by another user.");
                    newClaimAction.ClickClaimSearchIcon();
                    if (!_claimSearch.GetSideBarPanelSearch.GetTopHeaderName().Equals("Find Claim"))
                        _claimSearch.ClickOnFindClaimIcon();
                    StringFormatter.PrintMessage(
                        Format("Validate lock on claim sequence: {0} by searching from new claim search page ",
                            claimSeq));
                    _claimSearch.SearchByClaimSequence(claimSeq);
                    newClaimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                    newClaimAction.IsClaimLocked()
                        .ShouldBeTrue("Claim should be locked when it is in view mode by another user");
                    newClaimAction.GetLockIConTooltip()
                        .ShouldBeEqual(
                            "This claim has been opened in view mode. It is currently locked by Test Automation5 (ui_automation5).",
                            "Is Lock Message Equal?");
                    newClaimAction.ClickClaimSearchIcon();
                    StringFormatter.PrintMessage(
                        Format("Validate lock on claim sequence: {0} when returninig to new claim search page ",
                            claimSeq));
                    _claimSearch.GetGridViewSection.GetTitleOfListIconPresentInGridForClassName("lock")
                        .ShouldBeEqual(
                            "This claim has been opened in view mode. It is currently locked by Test Automation5 (ui_automation5).",
                            "Is Lock Message Equal?");
                    _claimSearch.SearchByClaimNmber(claimNo);
                    _claimSearch.IsClaimLockPresentForClaimSequence(claimSeq)
                        .ShouldBeTrue("Claim should be locked when it is in view mode by another user");
                }
                finally
                {
                    automatedBase.CurrentPage =
                        _claimSearch =
                            automatedBase.CurrentPage.ClickOnQuickLaunch().Logout().LoginAsHciAdminUser().NavigateToClaimSearch();
                }
            }
        }

        

        [Test]//US69625 //US69703 // CAR-2366 | CAR-2386
        [Retrying(Times = 3)]
        public void Verify_default_for_real_time_clients()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ClaimSearchPage _claimSearch;
                ClientSearchPage _clientSearch;
                _claimSearch = automatedBase.QuickLaunch.NavigateToClaimSearch();
                automatedBase.CurrentPage = _claimSearch.ClickOnQuickLaunch().ClickOnSwitchClient()
                    .SwitchClientTo(ClientEnum.CVTY);
                for (var i = 0; i < 2; i++)
                {
                    _clientSearch = _claimSearch.NavigateToClientSearch();
                    _clientSearch.SearchByClientCodeToNavigateToClientProfileViewPage(
                        ClientEnum.CVTY.GetStringDisplayValue());
                    _clientSearch.GetSideWindow.ClickOnEditIcon();
                    _clientSearch.GetSideWindow.SelectDropDownListValueByLabel(
                        GeneralTabEnum.ProcessingType.GetStringValue(),
                        i == 0 ? ProcessingType.PR.GetStringValue() : ProcessingType.R.GetStringValue());
                    _clientSearch.GetSideWindow.Save(waitForWorkingMessage: true);

                    _claimSearch = automatedBase.CurrentPage.NavigateToClaimSearch();
                    try
                    {
                        _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                            ClaimQuickSearchTypeEnum.UnreviewedClaims.GetStringValue());
                        _claimSearch.ClickOnFindButton();
                        _claimSearch.GetGridViewSection.GetLabelInGridByColRow(3, 1).ShouldBeEqual("Due:",
                            "Due Date column should be shown on Claim Search for PCA Real-time clients.");
                        _claimSearch.IsListDateSortedInAscendingOrder(3)
                            .ShouldBeTrue(
                                "Due Date Should sorted in ascending Order, claims with oldest date/time should be shown at the top of the list");
                        _claimSearch.ClickOnFilterOption();
                        _claimSearch.ClickOnFilterOptionListRow(1);
                        _claimSearch.IsListClaimSeqSortedInAscendingOrder(2)
                            .ShouldBeTrue("Claim search result should be sorted by Claim Sequence in ascending order.");
                        _claimSearch.ClickOnFilterOption();
                        _claimSearch.ClickOnFilterOptionListRow(9);
                        StringFormatter.PrintMessage("Clicked on Clear Sort.");
                        _claimSearch.IsListDateSortedInAscendingOrder(3)
                            .ShouldBeTrue("Claim Search result should be sorted by Due Date in ascending Order");
                    }
                    finally
                    {
                        _claimSearch.ClickOnFilterOptionListRow(9);
                        StringFormatter.PrintMessage("Clicked on Clear Sort.");
                        _clientSearch = _claimSearch.NavigateToClientSearch();
                        _clientSearch.SearchByClientCodeToNavigateToClientProfileViewPage(
                            ClientEnum.CVTY.GetStringDisplayValue());
                        _clientSearch.GetSideWindow.ClickOnEditIcon();
                        _clientSearch.GetSideWindow.SelectDropDownListValueByLabel(
                            GeneralTabEnum.ProcessingType.GetStringValue(), ProcessingType.R.GetStringValue());
                        _clientSearch.GetSideWindow.Save(waitForWorkingMessage: true);

                        _claimSearch = automatedBase.CurrentPage.NavigateToClaimSearch();
                    }
                }
            }
        }

        [Test, NUnit.Framework.Category("ClaimViewRestriction")]//CAR-506,CAR-495
        [Retrying(Times = 3)]
        public void Verify_claim_should_display_for_restricted_claim_for_all_user_type()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ClaimSearchPage _claimSearch;
                _claimSearch = automatedBase.QuickLaunch.NavigateToClaimSearch();
                var TestName = new StackFrame(true).GetMethod().Name;
                var claimSequence = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestName,
                    "ClaimSequence", "Value");
                automatedBase.CurrentPage = _claimSearch = automatedBase.CurrentPage.NavigateToClaimSearch();
                _claimSearch.GetCommonSql
                    .IsClaimAllowByClaimSequenceAndUserId(claimSequence,
                        automatedBase.EnvironmentManager.HciClaimViewRestrictionUsername)
                    .ShouldBeFalse(Format("{0} is in restricted group", claimSequence));
                var url = Format("app/#/clients/{0}/claims/{1}/{2}/lines/claim_lines?claimSearchType=findClaims",
                    ClientEnum.SMTST, claimSequence.Split('-')[0], claimSequence.Split('-')[1]);
                try
                {
                    var currentPage = _claimSearch.VisitAndReturnPageByUrlFoAuthorizedPage<ClaimActionPage>(url);
                    currentPage.GetClaimSequence().ShouldBeEqual(claimSequence, "Is Claim Action Page Open?");
                    currentPage.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                    currentPage.ClickClaimSearchIcon();
                    currentPage.ClickSearchIcon();
                    _claimSearch.SearchByClaimSequence(claimSequence);
                    currentPage.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                    currentPage.GetClaimSequence().ShouldBeEqual(claimSequence);
                }
                finally
                {
                    automatedBase.CurrentPage = _claimSearch = _claimSearch.NavigateToClaimSearch();
                }
            }
        }

        [Test] //TE264
        [Retrying(Times = 3)]
        public void Verify_Auto_trim_Off_spaces_for_fields()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ClaimSearchPage _claimSearch;
                _claimSearch = automatedBase.QuickLaunch.NavigateToClaimSearch();
                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestName);
                StringFormatter.PrintMessage("Verify extra space is removed from fields");
                /*  out of scope for Selenium
                _claimSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Claim Sequence",paramLists["ClaimSequence"]); 
                _claimSearch.GetSideBarPanelSearch.GetInputValueByLabel("Claim Sequence").Length.ShouldBeEqual(paramLists["Claim Sequence"].Length, "extra spaces removed?");
                _claimSearch.GetSideBarPanelSearch.ClickOnClearLink();*/
                _claimSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Claim No",
                    Concat(paramLists["ClaimNo"], "      "));
                _claimSearch.GetSideBarPanelSearch.ClickOnFindButton();
                _claimSearch.WaitForWorkingAjaxMessage();
                _claimSearch.GetSearchResultRowCount()
                    .ShouldBeGreater(0, "Claim Search Result Row  for search with claim number");
                _claimSearch.ClickSearchResultRow();
                _claimSearch.GetClaimNo().ShouldBeEqual(paramLists["ClaimNo"]);
                _claimSearch.GetSideBarPanelSearch.ClickOnClearLink();
                _claimSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Other Claim Number",
                    Concat(paramLists["OtherClaimNumber"], "  "));
                _claimSearch.GetSideBarPanelSearch.ClickOnFindButton();
                _claimSearch.WaitForWorkingAjaxMessage();
                _claimSearch.GetSearchResultRowCount()
                    .ShouldBeGreater(0, "Claim Search Result Row for search with other claim number ");
                _claimSearch.GetSideBarPanelSearch.ClickOnClearLink();
                _claimSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Batch ID",
                    paramLists["BatchIDSearch"] + "   ");
                _claimSearch.GetSideBarPanelSearch.ClickOnFindButton();
                _claimSearch.WaitForWorkingAjaxMessage();
                _claimSearch.GetSearchResultRowCount()
                    .ShouldBeGreater(0, "Claim Search Result Row for search with batch id  ");
                _claimSearch.ClickSearchResultRow();
                _claimSearch.GetBatchId().ShouldBeEqual(paramLists["BatchID"]);
                _claimSearch.GetSideBarPanelSearch.ClickOnClearLink();
            }
        }

        [Test] //CAR-766
        [Retrying(Times = 3)]
        public void Verify_full_facility_name_displays_on_search_result()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ClaimSearchPage _claimSearch;
                _claimSearch = automatedBase.QuickLaunch.NavigateToClaimSearch();
                _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                    ClaimQuickSearchTypeEnum.FlaggedClaims.GetStringValue());
                _claimSearch.GetSideBarPanelSearch.ClickOnFindButton();
                _claimSearch.WaitForWorkingAjaxMessage();
                var prvName =
                    _claimSearch.GetCommonSql.GetFullPrvNameForGivenClaimSeq(
                        _claimSearch.GetGridViewSection.GetValueInGridByColRow());
                _claimSearch.ClickOnClaimSearchResultByRow(1);
                _claimSearch.GetProviderNameInProviderdetailsByRowCol(row: 2, col: 4).ShouldBeEqual(prvName,
                    "Full Facility Name/ Provider name for given claim seq is present and should be equal to " +
                    prvName);
                /* sorting covered in US68301 */
                _claimSearch.GetSideBarPanelSearch.ClickOnClearLink();
            }
        }

        [Test, NUnit.Framework.Category("AppealDependent"),NUnit.Framework.Category("Working")]//CAR-1395(CAR-836)
        public void Verify_appeal_badge_icon_in_primary_and_appeal_status_in_secondary_detail()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ClaimSearchPage _claimSearch;
                _claimSearch = automatedBase.QuickLaunch.NavigateToClaimSearch();
                var TestName = new StackFrame(true).GetMethod().Name;
                var claSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestName,
                    "ClaimSequence", "Value");
                var claimNo = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestName, "ClaimNo",
                    "Value");

                _claimSearch.SearchByClaimNmber(claimNo);

                Convert.ToInt32(_claimSearch.GetGridViewSection.GetValueInGridByColRow(9, 2))
                    .ShouldBeEqual(1, "Verify Appeal Count for having only one appeal");
                var appealCount = Convert.ToInt32(_claimSearch.GetGridViewSection.GetValueInGridByColRow(9, 4));
                appealCount.ShouldBeGreater(1, "Is Appeal Badge Icon Present for having appeal greater than one?");

                _claimSearch.GetGridViewSection.GetToolTipInGridByColRow(9, 4)
                    .ShouldBeEqual(Format("This claim has {0} appeal(s).", appealCount),
                        "Verify Tooltip message of appeal badge icon");

                _claimSearch.GetGridViewSection.GetColorInGridByColRow(9, 4)
                    .AssertIsContained("rgb(194, 194, 194)", "Is Appeal Badge Icon in Grey Color");

                var newClaimAction = _claimSearch.ClickOnClaimSequenceByClaimSequence(claSeq);
                newClaimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                newClaimAction.ClickOnViewAppealIcon();
                var status = newClaimAction.GetListOfAppealStatusInRows();
                _claimSearch = newClaimAction.ClickClaimSearchIcon();
                _claimSearch.ClickOnClaimSearchResultByRow(4);
                _claimSearch.GetClaimDetailsValueByLabel("Appeal Status:")
                    .ShouldBeEqual(status[0], "Verify Appeal Status should be recent one for multiple Appeals");
                _claimSearch.ClickOnClaimSearchResultByRow(1);
                _claimSearch.GetClaimDetailsValueByLabel("Appeal Status:")
                    .ShouldBeEqual("None", "Verify Appeal Status for not having appeal");
            }
        }

        [Test]//TE-527 + TE-672+TE-795
        [Retrying(Times = 3)]
        public void Validate_export_icon_and_exported_Excel_Value()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ClaimSearchPage _claimSearch;
                _claimSearch = automatedBase.QuickLaunch.NavigateToClaimSearch();
                var TestName = new StackFrame(true).GetMethod().Name;
                var expectedHeaders = automatedBase.DataHelper.GetMappingData(FullyQualifiedClassName, "Claim_export_headers").Values
                    .ToList();
                var parameterList = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestName);
                var sheetname = parameterList["sheetName"];
                string lob = parameterList["LoB"];
                var expectedDataList = _claimSearch.GetExcelDataList(automatedBase.EnvironmentManager.Username);
                var fileName = "";

                try
                {

                    _claimSearch.IsExportIconPresent().ShouldBeTrue("Export Icon Present?");
                    _claimSearch.IsExportIconDisabled().ShouldBeTrue("Is Export Icon disabled?");
                    _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search", "Flagged Claims");
                    _claimSearch.GetSideBarPanelSearch.SelectSearchDropDownListForMultipleSelectValue(
                        "Line Of Business",
                        lob);
                    _claimSearch.GetSideBarPanelSearch.ClickOnHeader();
                    _claimSearch.ClickOnFindButton();
                    _claimSearch.IsExportIconEnabled().ShouldBeTrue("Is Export Icon enabled?");

                    _claimSearch.ClickOnExportIcon();
                    _claimSearch.IsPageErrorPopupModalPresent()
                        .ShouldBeTrue("Is Confirmation Model Displayed after clicking on export?");

                    StringFormatter.PrintMessage("verify on clicking cancel in confirmation model , nothing happens");
                    _claimSearch.ClickOkCancelOnConfirmationModal(false);
                    _claimSearch.IsPageErrorPopupModalPresent()
                        .ShouldBeFalse("Is Confirmation model displayed after clicking cancel?");

                    StringFormatter.PrintMessage("verify export of claim search");
                    _claimSearch.ClickOnExportIcon();
                    _claimSearch.ClickOkCancelOnConfirmationModal(true);
                    SiteDriver.WaitToLoadNew(3000);

                    fileName = _claimSearch.GoToDownloadPageAndGetFileName();


                    ExcelReader.ReadExcelSheetValue(fileName, sheetname, 3, 3, out List<string> headerList,
                        out List<List<string>> excelExportList, out string clientname, true);

                    StringFormatter.PrintMessage("verify client name and header values");
                    expectedHeaders.ShouldCollectionBeEqual(headerList, "headers equal?");
                    clientname.Trim().ShouldBeEqual(Extensions.GetStringValue(ClientEnum.SMTST));
                    StringFormatter.PrintMessage("verify values correct?");


                    for (int i = 0; i < expectedDataList.Count - 1; i++)
                    {
                        excelExportList[i][0].ShouldBeEqual(expectedDataList[i][1],
                            "Correct Claim Sequence values should be exported");
                        excelExportList[i][1].ShouldBeEqual(expectedDataList[i][2],
                            "Correct Claim Number values should be exported");
                        excelExportList[i][2].ShouldBeEqual(expectedDataList[i][3],
                            "Correct Received Date values should be exported");
                        excelExportList[i][3].ShouldBeEqual(expectedDataList[i][4],
                            "Correct Form Type values should be exported");
                        excelExportList[i][4].ShouldBeEqual(expectedDataList[i][5],
                            "Correct Provider Name values should be exported");
                        excelExportList[i][5].ShouldBeEqual(expectedDataList[i][6],
                            "Correct Provider Sequence values should be exported");
                        excelExportList[i][6].ShouldBeEqual(expectedDataList[i][7],
                            "Correct Member Id values should be exported");
                        excelExportList[i][7].ShouldBeEqual(expectedDataList[i][8],
                            "Correct Flag values should be exported");
                        excelExportList[i][7].IsFlagInCorrectFormat().ShouldBeTrue("Flags in correct format?");
                        excelExportList[i][8].ShouldBeEqual(double.Parse(expectedDataList[i][9]).ToString(),
                            "Correct Savings values should be exported");
                        excelExportList[i][9].ShouldBeEqual(expectedDataList[i][10],
                            "Correct Batch ID values should be exported");
                        excelExportList[i][10].ShouldBeEqual(expectedDataList[i][11],
                            "Correct Line Of Business values should be exported");
                        excelExportList[i][11].ShouldBeEqual(expectedDataList[i][12],
                            "Correct Claim Status values should be exported");
                    }


                    _claimSearch.GetClaimExportAuditListFromDB(automatedBase.EnvironmentManager.Username).ShouldContain(
                        "/api/clients/SMTST/ClaimSearchResults/DownloadXLS/", "Claim search download audit present?");
                }
                finally
                {
                    _claimSearch.CloseAnyPopupIfExist();
                    if (!IsNullOrEmpty(fileName))
                        ExcelReader.DeleteExcelFileIfAlreadyExists(fileName);

                }
            }
        }

      

        [Test] //TE-718
        [Retrying(Times = 1)]
        public void Verify_Outstanding_QA_Claims_Quick_Search_Option()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ClaimSearchPage _claimSearch;
                _claimSearch = automatedBase.QuickLaunch.NavigateToClaimSearch();
                var quickSearchOptions = automatedBase.DataHelper.GetMappingData(FullyQualifiedClassName, "outstanding_qa_claims")
                    .Values.ToList();

                var qaReadyClaimsFromDatabase = _claimSearch.GetListOfQAReadyClaimsFromDatabase();

                _claimSearch.IsRoleAssigned<ClaimSearchPage>(new List<string> {automatedBase.EnvironmentManager.HciAdminUsername2},
                    RoleEnum.QAAnalyst.GetStringValue()).ShouldBeTrue(
                    $"Is CV QA Audit present for current user<{automatedBase.EnvironmentManager.HciAdminUsername2}>");

                StringFormatter.PrintMessage("Verify Outstanding QA Claim Search Option");
                _claimSearch.GetSideBarPanelSearch.GetAvailableDropDownList("Quick Search").ShouldContain(
                    ClaimQuickSearchTypeEnum.OutstandingQCClaims.GetStringValue(),
                    "Quick Search Option should contain Outstanding QA Claim option for users with CV QA audit authority");
                _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                    ClaimQuickSearchTypeEnum.OutstandingQCClaims.GetStringValue());
                _claimSearch.GetSideBarPanelSearch.ClickOnFindButton();
                _claimSearch.WaitForWorking();
                _claimSearch.GetGridViewSection.GetGridListValueByCol()
                    .ShouldCollectionBeEquivalent(qaReadyClaimsFromDatabase, "Data Should Match");

                StringFormatter.PrintMessage("Verify additional Filters Are not shown");
                _claimSearch.GetSideBarPanelSearch.GetSearchFiltersList().ShouldCollectionBeEqual(quickSearchOptions,
                    "Search Filters Should Match");

                StringFormatter.PrintMessage(
                    "Verify Outstanding QA Claim Search Option Is Not Present For User Not Having CV QA Audit Authority");
                _claimSearch.Logout().LoginAsUserHavingNoAnyAuthority().NavigateToClaimSearch();
                _claimSearch.GetSideBarPanelSearch.GetAvailableDropDownList("Quick Search").ShouldNotContain(
                    ClaimQuickSearchTypeEnum.OutstandingQCClaims.GetStringValue(),
                    "Quick Search Option should not contain Outstanding QA Claim option");
            }
        }

        [NonParallelizable]
        [Test] //TE-820
        public void Verify_Previous_Search_Is_Cleared_In_Claim_Search_Page()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ClaimSearchPage _claimSearch;
                _claimSearch = automatedBase.QuickLaunch.NavigateToClaimSearch();
                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestName);
                _claimSearch.DeleteLogicNoteFromDatabase(paramLists["ClaimSequence"]);

                _claimSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Batch ID", paramLists["BatchId"]);
                _claimSearch.GetSideBarPanelSearch.ClickOnFindButton();
                _claimSearch.WaitForWorking();

                var newClaimAction = _claimSearch.ClickOnClaimSequenceOfSearchResult(1);
                newClaimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                var claseq = newClaimAction.GetClaimSequence();
                newClaimAction.ClickOnNextButton();
                newClaimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                newClaimAction.GetClaimSequence()
                    .ShouldNotBeEqual(claseq, "Navigation successful to next claim in the worklist");
                newClaimAction.ClickClaimSearchIcon();
                _claimSearch.GetSideBarPanelSearch.ClickOnClearLink();
                _claimSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Claim Sequence", paramLists["ClaimSequence"]);
                _claimSearch.GetSideBarPanelSearch.ClickOnFindButton();
                _claimSearch.WaitForWorking();
                newClaimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();

                newClaimAction.ClickOnNextButton();
                automatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.ClaimSearch.GetStringValue(),
                    "Navigate to Claim Search Page");

                //Create Logic
                _claimSearch.SearchByClaimSequence(paramLists["ClaimSequence"]);
                newClaimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                newClaimAction.ClickOnAddLogicIconByRow(1);
                newClaimAction.AddLogicMessageTextarea(paramLists["Message"]);
                newClaimAction.GetSideWindow.Save(waitForWorkingMessage: true);
                automatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.ClaimSearch.GetStringValue(),
                    "Navigate to Claim Search Page", true);

            }
        }

        [Test] //TE-927
        [Retrying(Times = 3)]
        public void Verify_Validation_Message_When_Searched_Using_Only_Clasub()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ClaimSearchPage _claimSearch;
                _claimSearch = automatedBase.QuickLaunch.NavigateToClaimSearch();
                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestName);
                _claimSearch.SetClaimSequenceInFindClaimSection(paramLists["Clasub"]);
                _claimSearch.ClickOnFindButton();
                _claimSearch.IsPageErrorPopupModalPresent().ShouldBeTrue("error pop up present?");
                _claimSearch.GetPageErrorMessage().ShouldBeEqual("Search cannot be initiated with ClaSub only.");
                _claimSearch.ClosePageError();
            }
        }

        [Test] //TE-986
        [Retrying(Times = 3)]
        public void Verify_Flag_status_In_Secondary_Details()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ClaimSearchPage _claimSearch;
                _claimSearch = automatedBase.QuickLaunch.NavigateToClaimSearch();
                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestName);
                var claseq = paramLists["claseq"];
                var noFlagClaseq = paramLists["NoFlagClaseq"];
                _claimSearch.GetCommonSql.GetAllActiveProductsAbbrvForClient(ClientEnum.SMTST.ToString())
                    .ShouldContain(ProductEnum.COB.ToString(), "Is COB product active for the client ?");
                _claimSearch.ResetFlagStatusOfClaim(claseq);
                _claimSearch.GetSideBarPanelSearch.OpenSidebarPanel();
                _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                    ClaimQuickSearchTypeEnum.UnreviewedClaims.GetStringValue());
                _claimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Product",
                    ProductEnum.COB.GetStringValue());
                _claimSearch.GetSideBarPanelSearch.ClickOnFindButton();
                _claimSearch.WaitForWorking();
                _claimSearch.ClickOnClaimSearchResultByClaseq(claseq);
                _claimSearch.GetClaimDetailsValueByLabel("COB:").ShouldBeEqual(
                    _claimSearch.GetFlagStatusForInternalUser(claseq.Split('-')[0], claseq.Split('-')[1]),
                    "correct status displayed when COB Flags are Unreviewed");
                var claimAction = _claimSearch.ClickOnClaimSequenceByClaimSequence(claseq);
                claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                claimAction.WaitForPageToLoad();
                claimAction.ClickOnApproveButton();
                claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                claimAction.ClickWorkListIcon();
                claimAction.ClickSearchIcon();
                claimAction.GetSideBarPanelSearch.ClickOnClearLink();
                claimAction.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                    ClaimQuickSearchTypeEnum.AllClaims.GetStringValue());
                claimAction.SearchByClaimSequence(claseq);
                claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                claimAction.ClickClaimSearchIcon();
                _claimSearch.ClickOnClaimSearchResultByClaseq(claseq);
                _claimSearch.GetClaimDetailsValueByLabel("COB:").ShouldBeEqual(
                    _claimSearch.GetFlagStatusForInternalUser(claseq.Split('-')[0], claseq.Split('-')[1]),
                    "correct status displayed when COB Flags are reviewed");

                _claimSearch.SearchByClaimSequence(noFlagClaseq);
                claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
                claimAction.ClickClaimSearchIcon();
                _claimSearch.ClickOnClaimSearchResultByClaseq(noFlagClaseq);
                _claimSearch.GetClaimDetailsValueByLabel("COB:").ShouldBeEqual(
                    _claimSearch.GetFlagStatusForInternalUser(noFlagClaseq.Split('-')[0], noFlagClaseq.Split('-')[1]),
                    "correct status displayed when COB Flags are reviewed");
            }
        }
    }
}

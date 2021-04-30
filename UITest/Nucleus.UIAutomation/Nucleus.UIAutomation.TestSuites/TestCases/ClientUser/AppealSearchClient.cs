using System;
using System.Collections.Generic;
using System.Linq;
using Nucleus.Service.Support.Enum;
using Nucleus.UIAutomation.TestSuites.Base;
using Nucleus.Service.PageServices.Appeal;
using Nucleus.Service.Support.Common.Constants;
using NUnit.Framework;
using Nucleus.Service.Data;
using System.Diagnostics;
using System.IO;
using UIAutomation.Framework.Utils;
using System.Text.RegularExpressions;
using Nucleus.Service.Support.Environment;
using UIAutomation.Framework.Core.Driver;

namespace Nucleus.UIAutomation.TestSuites.TestCases.ClientUser
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class AppealSearchClient
    {
        #region Old Sequential Code Commented Out

        /*#region PRIVATE FIELDS


        private AppealSearchPage _appealSearch;
        private AppealActionPage _appealAction;
        private AppealSummaryPage _appealSummary;
        private List<string> _assignedClientList;
        private List<string> _lineOfBusinessList;
        #endregion

        #region PROTECTED PROPERTIES

        protected override string FullyQualifiedClassName
        {
            get { return GetType().FullName; }
        }

        #endregion

        #region OVERRIDE METHODS

        protected override void ClassInit()
        {
            try
            {
                base.ClassInit();
                automatedBase.CurrentPage = _appealSearch = automatedBase.QuickLaunch.NavigateToAppealSearch();

                try
                {
                    RetrieveListFromDatabase();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

            }
            catch (Exception)
            {
                if (StartFlow != null)
                    StartFlow.Dispose();
                throw;
            }
        }

        protected override void TestInit()
        {
            base.TestInit();
            automatedBase.CurrentPage = _appealSearch;
        }

        protected override void TestCleanUp()
        {
            _appealSearch.CloseAnyTabIfExist();
            if (string.Compare(UserType.CurrentUserType, UserType.CLIENT, StringComparison.OrdinalIgnoreCase) != 0)
            {
                automatedBase.CurrentPage = automatedBase.QuickLaunch = automatedBase.CurrentPage.Logout().LoginAsClientUser();
                _appealSearch = automatedBase.CurrentPage.NavigateToAppealSearch();
            }

            if (automatedBase.CurrentPage.Equals(typeof(AppealActionPage)))
            {
                automatedBase.CurrentPage = _appealSearch = _appealAction.ClickOnSearchIconToNavigateAppealSearchPage();
            }
            else if (!automatedBase.CurrentPage.Equals(typeof(AppealSearchPage)))
            {
                _appealSearch = automatedBase.CurrentPage.ClickOnautomatedBase.QuickLaunch().NavigateToAppealSearch();
            }
            _appealSearch.ClearAll();
            _appealSearch.SelectMyAppeals();
            base.TestCleanUp();

        }

        #region DBinteraction methods
        private void RetrieveListFromDatabase()
        {
           
            _assignedClientList = _appealSearch.GetAssignedClientList(EnvironmentManager.ClientUserName);
            _assignedClientList.Insert(0, "All");
            _lineOfBusinessList = _appealSearch.GetCommonSql.GetLineOfBusiness();
            _appealSearch.CloseDbConnection();
        }



        #endregion


        #endregion*/

        #endregion

        protected string FullyQualifiedClassName
        {
            get { return GetType().FullName; }
        }

        #region TEST SUITES

        [Test] //CAR-2992(CAR-2962) + CAR-3121(CAR-3063)
        public void Verify_appeal_level_count_should_be_unique_to_that_client()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                AppealSearchPage _appealSearch = automatedBase.CurrentPage.NavigateToAppealSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var claimSequence = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "ClaimSequence", "Value");
                _appealSearch.SearchByClaimSequence(claimSequence, ClientEnum.SMTST.ToString());

                #region CAR-3121(CAR-3063)

                _appealSearch.GetSearchResultListByCol(6).Distinct().ShouldCollectionBeEquivalent(
                    new List<string> {"A", "MRR"}, "List should contain MRR that have been created on that claim.");
                _appealSearch.IsAppealLevelBadgeValuePresentForMRRAppealType().ShouldBeTrue(
                    "Badge should show the count of appeal types of appeal and MRR that have been created on that claim.");

                #endregion

                _appealSearch.GetAppealLevelBadgeValue().Max().ShouldBeEqual(
                    _appealSearch.GetAppealLevel(claimSequence, ClientEnum.SMTST),
                    $"Is Correct Appeal Level display for {ClientEnum.SMTST}");
                _appealSearch.SearchByClaimSequence(claimSequence, ClientEnum.RPE.ToString());
                _appealSearch.GetAppealLevelBadgeValue().Max().ShouldBeEqual(
                    _appealSearch.GetAppealLevel(claimSequence, ClientEnum.RPE),
                    $"Is Correct Appeal Level display for {ClientEnum.RPE}");
            }
        }

        [Test, Category("SmokeTestDeployment")] //TANT-93
        public void Verify_Appeal_Search_Find_Panel_Icon_And_Sort_Options()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                AppealSearchPage _appealSearch = automatedBase.CurrentPage.NavigateToAppealSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var filterOptions =
                    automatedBase.DataHelper.GetMappingData(FullyQualifiedClassName, "Verify_search_list_sorting")
                        .Values.ToList();

                StringFormatter.PrintMessage("Verify open/close of Find Appeals Panel");
                _appealSearch.GetSideBarPanelSearch.IsSideBarPanelOpen()
                    .ShouldBeTrue(" By default Find Appeals panel displayed ?");
                _appealSearch.GetSideBarPanelSearch.ClickOnToggleSidebarPanelButton();
                _appealSearch.GetSideBarPanelSearch.IsSideBarPanelOpen()
                    .ShouldBeFalse(" Find Appeals panel displayed ?");
                _appealSearch.GetSideBarPanelSearch.ClickOnToggleSidebarPanelButton();
                _appealSearch.GetSideBarPanelSearch.IsSideBarPanelOpen()
                    .ShouldBeTrue(" Find Appeals panel displayed ?");

                StringFormatter.PrintMessage("Verify Sort Option List");
                _appealSearch.GetGridViewSection.IsFilterOptionIconPresent()
                    .ShouldBeTrue("Is Filter Option Icon Present ?");
                _appealSearch.GetGridViewSection.GetFilterOptionTooltip()
                    .ShouldBeEqual("Sort Results", "Correct tooltip is displayed");
                _appealSearch.GetGridViewSection.GetFilterOptionList()
                    .ShouldCollectionBeEqual(filterOptions, "Filter Options Lists Collection Should Be Equal");
            }
        }

        [Test, Category("SmokeTestDeployment"), Category("AppealDependent")] //TANT-93
        public void Verify_Appeal_Search_Results_And_Navigation()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                AppealSearchPage _appealSearch = automatedBase.CurrentPage.NavigateToAppealSearch();

                StringFormatter.PrintMessage("Verify Results For All Appeals Quick Search Options");
                _appealSearch.GetSideBarPanelSearch.ClickOnAdvancedSearchFilterIcon(true);
                _appealSearch.SelectAllAppeals();
                _appealSearch.SelectClientSmtst();
                _appealSearch.SelectSearchDropDownListValue("Status",
                    AppealStatusEnum.Complete.GetStringDisplayValue());
                _appealSearch.ClickOnFindButton();
                _appealSearch.WaitForWorkingAjaxMessage();
                _appealSearch.GetGridViewSection.GetGridRowCount()
                    .ShouldBeGreater(0, "Search Result should be returned.");
                var appealSummary = _appealSearch.ClickOnAppealSequence(1);
                appealSummary.GetPageHeader().ShouldBeEqual(PageHeaderEnum.AppealSummary.GetStringValue(),
                    "Page Header should be Appeal Summary");
                automatedBase.CurrentPage = _appealSearch = appealSummary.ClickOnSearchIconToNavigateAppealSearchPage();
            }
        }

        [Test,Category("SmokeTestDeployment"), Category("EdgeNonCompatible")] //US67260 //TANT-93
        public void Verify_Appeal_letter_download_page()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                AppealSearchPage _appealSearch = automatedBase.CurrentPage.NavigateToAppealSearch();

                _appealSearch.ClickOnAdvancedSearchFilterIcon(true);
                _appealSearch.SelectAllAppeals();
                _appealSearch.SelectClientSmtst();
                _appealSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Status",
                    AppealStatusEnum.Complete.GetStringDisplayValue());
                _appealSearch.ClickOnFindButtonAndWait();

                if (automatedBase.CurrentPage.GetPageHeader() != PageHeaderEnum.AppealSearch.GetStringValue())
                {
                    _appealSearch.ClickOnBrowserBackButton();
                }

                var appealLetter = _appealSearch.ClickOnAppealLetter(1);
                appealLetter.GetAppealLetterPageHeader().ShouldBeEqual(PageHeaderEnum.AppealLetter.GetStringValue(),
                    "Appeal Letter Popup", true);

                appealLetter.ClickOnClaimNoAndGetPageHeader().ShouldBeEqual(
                    PageHeaderEnum.ClaimAction.GetStringValue(),
                    "Claim Action Tab should display when click on claim Number");
                appealLetter.SwitchWindowByTitle(PageTitleEnum.AppealLetter.GetStringValue());

                var fileName = _appealSearch.ClickOnDownloadPDFAndGetFileName(appealLetter);
                _appealSearch.WaitForFileExists(@"C:/Users/uiautomation/Downloads/" + fileName);
                File.Exists(@"C:/Users/uiautomation/Downloads/" + fileName)
                    .ShouldBeTrue("Appeal Letter should be downloaded");
            }
        }


        [Test] //US45660
        public void Verify_client_side_search_filters_and_result_display()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                AppealSearchPage _appealSearch = automatedBase.CurrentPage.NavigateToAppealSearch();
                AppealSummaryPage _appealSummary;

                var _assignedClientList = _appealSearch.GetAssignedClientList(automatedBase.EnvironmentManager.ClientUserName);
                _assignedClientList.Insert(0, "All");

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var expectedQuickSearchList = automatedBase.DataHelper
                    .GetMappingData(FullyQualifiedClassName, "Quick_Search_List").Values.ToList();
                var expectedClientSearchList = _assignedClientList;
                var expectedApealStatusList = automatedBase.DataHelper
                    .GetMappingData(FullyQualifiedClassName, "appeal_status").Values.ToList();
                var expectedAppealTypeList = automatedBase.DataHelper
                    .GetMappingData(FullyQualifiedClassName, "Appeal_type").Values.ToList();
                var expectedAppealPriorityList = automatedBase.DataHelper
                    .GetMappingData(FullyQualifiedClassName, "Appeal_priority").Values.ToList();
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var client = ClientEnum.SMTST.ToString();

                automatedBase.CurrentPage = _appealSearch =
                    _appealSearch.ClickOnQuickLaunch().NavigateToAppealSearch();
                _appealSearch.GetPageHeader()
                    .ShouldBeEqual("Appeal Search", "Successfully navigated to Appeal search.");
                _appealSearch.IsFindAppealSectionPresent()
                    .ShouldBeTrue("Find appeal section is present in new appeal search page");
                _appealSearch.IsSearchListSectionPresent()
                    .ShouldBeTrue("Search List  section is present in new appeal search page");
                _appealSearch.ClickOnSearchButton();
                _appealSearch.WaitToOpenCloseWorkList();
                _appealSearch.IsFindAppealSectionPresent()
                    .ShouldBeFalse("Find Section Should Closed when click on search Icon First Time");
                _appealSearch.ClickOnSearchButton();
                _appealSearch.WaitToOpenCloseWorkList(false);
                _appealSearch.IsFindAppealSectionPresent()
                    .ShouldBeTrue("Find Section Should Displayed when click on search Icon Second Time");
                _appealSearch.GetQuickSearchFilterValue().ShouldBeEqual(paramLists["QuickSearchDefault"],
                    "Quick Search default filter equals  " + paramLists["QuickSearchDefault"]);

                _appealSearch.GetDropDownList(1)
                    .ShouldCollectionBeEqual(expectedQuickSearchList, "Quick Search  List As Expected");
                var searchResult = _appealSearch.GetAppealSearchResult();
                _appealSearch.SelectAllAppeals();
                _appealSearch.GetPageHeader()
                    .ShouldBeEqual("Appeal Search",
                        "Search wasn't auto executed after selection of 'all appeals ' in  quick search filter.");
                _appealSearch.GetAppealSearchResult().ShouldCollectionBeEqual(searchResult,
                    "Search wasn't auto executed after selection of 'all appeals ' in  quick search filter.");

                _appealSearch.SelectMyAppeals();
                _appealSearch.GetPageHeader()
                    .ShouldBeEqual("Appeal Search",
                        "Search wasn't auto executed after selection of 'my appeals ' in quick search filter.");
                _appealSearch.GetAppealSearchResult().ShouldCollectionBeEqual(searchResult,
                    "Search wasn't auto executed after selection of 'all appeals ' in  quick search filter.");

                _appealSearch.ClickOnFindButton();
                _appealSearch.WaitForWorkingAjaxMessage();
                var searchlistforMyappeals = _appealSearch.GetAppealSearchResult();
                searchlistforMyappeals.ShouldNotBeEqual(searchResult,
                    "Selection of my appeals filters down to appeals by user");
                _appealSearch.SelectLast30Days();
                _appealSearch.GetPageHeader()
                    .ShouldBeEqual("Appeal Search",
                        "Search wasn't auto executed after selection of 'last 30 days appeal ' in  quick search filter.");
                _appealSearch.GetAppealSearchResult().ShouldCollectionBeEqual(searchlistforMyappeals,
                    "Search wasn't auto executed after selection of 'all appeals ' in  quick search filter.");
                _appealSearch.ClickOnAdvancedSearchFilterIcon(true);
                _appealSearch.IsCreateDatePresent()
                    .ShouldBeFalse("Create Date Should not display when Last 30 Days Selected");
                _appealSearch.ClickOnAdvancedSearchFilterIcon(false);


                _appealSearch.ClickOnFindButton();
                _appealSearch.WaitForWorkingAjaxMessage();
                var searchlistforLast30Days = _appealSearch.GetAppealSearchResult();
                searchlistforLast30Days.ShouldNotBeEqual(searchlistforMyappeals,
                    "Selection of last 30 days appeals filters down to same");
                _appealSearch.GetSearchInputField(4)
                    .ShouldBeEqual(client, "Client Search default filter equals " + client);
                _appealSearch.GetDropDownList(4)
                    .ShouldCollectionBeEqual(expectedClientSearchList, "Client Search  List As Expected");

                _appealSearch.SelectSearchDropDownListValue("Client", client, 4);
                _appealSearch.GetSearchInputField(4)
                    .ShouldBeEqual(client, "Client Search bears type-ahead functionality");
                _appealSearch.GetSearchInputField(2).ShouldBeEqual("", "Appeal Sequence");
                _appealSearch.SetAppealSequence("1");
                _appealSearch.GetSearchInputField(3).ShouldBeEqual("", "Appeal No");
                _appealSearch.SetSearchInputField("Appeal No", "abc1", 3);
                _appealSearch.GetSearchInputField(5).ShouldBeEqual("", "Claim Sequence");
                _appealSearch.SetSearchInputField("ClaimSequence", "012", 5);
                _appealSearch.GetAlternateClaimNoLabel(6)
                    .ShouldBeEqual("Claim No", "Alternate Claim No title as set by client");
                _appealSearch.GetSearchInputField(6).ShouldBeEqual("", "Claim No");
                _appealSearch.SetSearchInputField("Claim No", paramLists["50CharacterClaimNo"], 6);
                _appealSearch.GetSearchInputField(6).Count()
                    .ShouldBeEqual(50, "Claim No should not greater than 50 character");

                _appealSearch.GetSearchInputField(8).ShouldBeEqual("All", "Appeal Status");
                _appealSearch.GetDropDownList(8)
                    .ShouldCollectionBeEqual(expectedApealStatusList, "Appeal Status List As Expected");
                _appealSearch.SelectSearchDropDownListValue("Status", paramLists["Status"], 8);
                _appealSearch.GetSearchInputField(9).ShouldBeEqual("All", "Appeal Type");
                _appealSearch.GetDropDownList(9)
                    .ShouldCollectionBeEqual(expectedAppealTypeList, "Appeal Type List As Expected");
                _appealSearch.SelectSearchDropDownListValue("Type", paramLists["Type"], 9);
                _appealSearch.GetSearchInputField(10).ShouldBeEqual("All", "Appeal Priority");
                _appealSearch.GetDropDownList(10).ShouldCollectionBeEqual(expectedAppealPriorityList,
                    "Appeal Priority List As Expected");
                _appealSearch.SelectSearchDropDownListValue("Priority", paramLists["Priority"], 10);
                _appealSearch.ClickOnFindButton();
                _appealSearch.WaitForWorkingAjaxMessage();
                _appealSearch.GetNoMatchingRecordFoundMessage()
                    .ShouldBeEqual("No matching records were found.", "No matching record found Message");
                _appealSearch.ClearAll();

                StringFormatter.PrintMessageTitle("Verify Clear Filter clears all the search filters");
                _appealSearch.GetSearchInputField(1).ShouldBeEqual("Last 30 Days", "Quick Search");
                _appealSearch.GetSearchInputField(2).ShouldBeEqual("", "Appeal Sequence");
                _appealSearch.GetSearchInputField(3).ShouldBeEqual("", "Appeal No");
                _appealSearch.GetSearchInputField(4).ShouldBeEqual(ClientEnum.SMTST.ToString(), "Client");
                _appealSearch.GetSearchInputField(5).ShouldBeEqual("", "Claim Sequence");
                _appealSearch.GetSearchInputField(6).ShouldBeEqual("", "Claim No");
                _appealSearch.GetSearchInputField(8).ShouldBeEqual("All", "Status");
                _appealSearch.GetSearchInputField(9).ShouldBeEqual("All", "Type");
                _appealSearch.GetSearchInputField(10).ShouldBeEqual("All", "Priority");
                _appealSearch.SelectAllAppeals();
                _appealSearch.SelectSearchDropDownListValue("Client", ClientEnum.SMTST.ToString(), 4);
                _appealSearch.SetAppealSequence(paramLists["AppealSeq"]);
                _appealSearch.SetSearchInputField("Appeal No", paramLists["AppealNo"], 3);
                automatedBase.CurrentPage =
                    _appealSummary =
                        _appealSearch.SearchByClaimSequence(paramLists["ClaimSequence"]); //search without cla sub

                _appealSearch.WaitForWorkingAjaxMessage();
                _appealSummary.GetPageHeader().ShouldBeEqual("Appeal Summary", "Navigated to appeal summary page");
                _appealSummary.GetAppealSequenceOnHeader().ShouldBeEqual(paramLists["AppealSeq"],
                    "Exact value required for Appeal Sequence .");
                _appealSummary.GetAppealDetails(2, 1).ShouldBeEqual(paramLists["AppealNo"],
                    "Appeal Number same as when appeal was created");
                automatedBase.CurrentPage =
                    _appealSearch = _appealSummary.ClickOnSearchIconToNavigateAppealSearchPage();
                _appealSearch.GetPreviouslyViewedAppealSeq().ShouldBeEqual(paramLists["AppealSeq"],
                    "Previously viewed Appeal Sequence is present.");
                _appealSearch.IsReturnToAppealSectionPresent()
                    .ShouldBeTrue("return to previously viewed appeal section is present");
                _appealSummary = _appealSearch.ReturnToPreviouslyViewedAppeal();
                _appealSearch.WaitForWorkingAjaxMessage();
                _appealSummary.GetPageHeader().ShouldBeEqual("Appeal Summary", "Navigated to appeal summary page");
                automatedBase.CurrentPage = _appealSummary.ClickOnSearchIconToNavigateAppealSearchPage();
            }
        }



        [Test] //US45660 borken to US47081
        public void Verify_client_side_advance_search_filters()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                AppealSearchPage _appealSearch = automatedBase.CurrentPage.NavigateToAppealSearch();
                var _lineOfBusinessList = _appealSearch.GetCommonSql.GetLineOfBusiness();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var expectedProductTypeList = automatedBase.DataHelper
                    .GetMappingData(FullyQualifiedClassName, "product_type").Values.ToList();
                var expectedPlanTypeList = automatedBase.DataHelper.GetMappingData(FullyQualifiedClassName, "plan_type")
                    .Values.ToList();
                var expectedApealLevelList = automatedBase.DataHelper
                    .GetMappingData(FullyQualifiedClassName, "appeal_level").Values.ToList();
                IDictionary<string, string> paramLists = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestExtensions.TestName);
                
                _appealSearch.GetPageHeader()
                    .ShouldBeEqual("Appeal Search", "Successfully navigated to Appeal search.");
                _appealSearch.IsSearchListSectionPresent()
                    .ShouldBeTrue("Search List  section is present in new appeal search page");
                _appealSearch.ClickOnAdvancedSearchFilterIcon(true);
                _appealSearch.GetSearchInputField(11).ShouldBeEqual("All", "Product value defaults to All");
                _appealSearch.GetDropDownList(11)
                    .ShouldCollectionBeEqual(expectedProductTypeList, "Product List As Expected");
                _appealSearch.IsDropDownSorted(11).ShouldBeTrue("Product sorted in alphabetical order");
                _appealSearch.SelectSearchDropDownListValue("Client", ClientEnum.SMTST.ToString(), 4);
                _appealSearch.GetSearchInputLabel(12)
                    .ShouldBeEqual("Plan", "On Specific Client selection Plan is visible");
                _appealSearch.IsSearchInputLabelForDropDownFieldDispalyed("Plan")
                    .ShouldBeTrue("On  Specific Client selection Plan is visible");
                _appealSearch.IsSearchInputFieldDisabled(17).ShouldBeFalse(
                    "On Specific Client selection Provider Sequence's disabled state should be false?");
                _appealSearch.GetSearchInputField(17).ShouldBeEqual(String.Empty,
                    "Provider Sequence value defaults to blank on client selection");
                _appealSearch.SelectSearchDropDownListValue("Client", "All", 4);
                _appealSearch.IsSearchInputLabelForDropDownFieldDispalyed("Plan")
                    .ShouldBeFalse("On non Specific Client selection Plan visibility should be false");
                _appealSearch.IsSearchInputFieldDispalyed(17)
                    .ShouldBeFalse("On Non Specific Client selection State is hidden");
                _appealSearch.IsSearchInputFieldDisabled(15)
                    .ShouldBeTrue("On Specific Client selection Provider Sequence should be disabled?");
                _appealSearch.GetSearchInputFieldPlaceholder(15).ShouldBeEqual("Please Select Client",
                    " Provider Sequence ask to select client");
                _appealSearch.SelectSearchDropDownListValue("Client", ClientEnum.SMTST.ToString(),
                    4); //re select specific client
                _appealSearch.SetSearchInputField("Provider sequence input", paramLists["ProviderSeq"], 17);

                _appealSearch.GetMultiSelectListedDropDownList(12)
                    .ShouldCollectionBeEqual(expectedPlanTypeList, "Plan List As Expected");
                _appealSearch.IsMultiSelectListedDropDownSorted(12)
                    .ShouldBeTrue("Plan sorted in alphabetical order");

                _appealSearch.GetSearchInputFieldPlaceholder(13).ShouldBeEqual("Select one or more",
                    "LOB value defaults to select one or more");
                _appealSearch.GetMultiSelectAvailableDropDownList(13)
                    .ShouldCollectionBeEqual(_lineOfBusinessList, "LOB List As Expected");
                _appealSearch.IsMultiSelectAvailableDropDownSorted(13)
                    .ShouldBeTrue("LOB sorted in alphabetical order");
                _appealSearch.SelectSearchDropDownListForMultipleSelectValue("Line of Business",
                    paramLists["LineOfBusiness1"], 13);
                _appealSearch.GetSearchInputFieldPlaceholder(13).ShouldBeEqual(paramLists["LineOfBusiness1"],
                    "LOB multiple value selected");
                _appealSearch.SelectSearchDropDownListForMultipleSelectValue("Line of Business",
                    paramLists["LineOfBusiness2"], 13);
                _appealSearch.GetSearchInputFieldPlaceholder(13)
                    .ShouldBeEqual("Multiple values selected", "LOB multiple value selected");


                _appealSearch.GetSearchInputField(18).ShouldBeEqual("All", "Appeal level value defaults to All");
                _appealSearch.GetDropDownList(18)
                    .ShouldCollectionBeEqual(expectedApealLevelList, "Appeal level List As Expected");
                _appealSearch.IsDropDownSorted(18).ShouldBeTrue("Appeal level sorted in alphabetical order");
                _appealSearch.SetSearchInputField("Appeal level", paramLists["AppealLevel"], 18);
                _appealSearch.GetSearchInputField(18)
                    .ShouldBeEqual(paramLists["AppealLevel"], "Appeal level selection ");

                _appealSearch.SelectAllAppeals();
                _appealSearch.SetSearchInputField("ClaimSequence", paramLists["ClaimSequence"], 5);
                ValidateDateRangePickerBehaviour("Due Date", 14);
                ValidateDateRangePickerBehaviour("Create Date", 15);
                ValidateDateRangePickerBehaviour("Complete Date", 16);


                void ValidateDateRangePickerBehaviour(string fieldName, int row)
                {
                    _appealSearch.GetDateFieldPlaceholder(row, 1).ShouldBeEqual("00/00/0000", "Date range picker for " + fieldName + " (from) default value");
                    _appealSearch.GetDateFieldPlaceholder(row, 2).ShouldBeEqual("00/00/0000", "Date range picker for " + fieldName + " (to) default value");
                    _appealSearch.SetDateField(row, 1, DateTime.Now.ToString("MM/d/yyyy"), fieldName); //check numeric value can be typed
                    _appealSearch.GetDateFieldFrom(row).ShouldBeEqual(DateTime.Now.ToString("MM/d/yyyy"), fieldName + " Checks numeric value can be inputted");
                    _appealSearch.SetDateField(row, 1, "", fieldName);
                    _appealSearch.SetDateFieldFrom(row, DateTime.Now.ToString("MM/d/yyyy"), fieldName);
                    _appealSearch.GetDateFieldTo(row).ShouldBeEqual(_appealSearch.GetDateFieldFrom(row), fieldName + " From value populated in To field as well.");
                    _appealSearch.SetDateField(row, 2, "", fieldName);
                    _appealSearch.SetDateFieldTo(row, DateTime.Now.AddMonths(3).AddDays(2).ToString("MM/d/yyyy"), fieldName);
                    ValidateFieldErrorMessageForDateRange(_appealSearch, fieldName, row,
                        "Search cannot be initiated for date ranges greater than 3 months.", false);
                }
            }
        }
        
        [Test] //us47369(US45660's child story
        public void Verify_search_list_sorting()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                AppealSearchPage _appealSearch = automatedBase.CurrentPage.NavigateToAppealSearch();

                _appealSearch.GetPageHeader()
                    .ShouldBeEqual("Appeal Search", "Successfully navigated to Appeal search.");
                _appealSearch.IsSearchListSectionPresent()
                    .ShouldBeTrue("Search List  section is present in new appeal search page");
                try
                {
                    _appealSearch.ClickOnAdvancedSearchFilterIcon(true);
                    _appealSearch.SelectAllAppeals();
                    _appealSearch.SelectSearchDropDownListValue("Appeal Level", "All");
                    _appealSearch.SetDateFieldFrom(14, "02/1/2016", "Create Date");
                    _appealSearch.SetDateFieldTo(14, "03/1/2016", "Create Date");
                    _appealSearch.SendEnterKeysOnClientTextField();
                    _appealSearch.WaitForWorkingAjaxMessage();
                    _appealSearch.GetAppealSearchResultRowCount()
                        .ShouldBeGreater(0, "Appeal Search Result found when hit <Enter> key");

                    _appealSearch.IsListDateSortedInAscendingOrder(2)
                        .ShouldBeTrue("Due Date should Sorted in Ascending Order by default");

                    ValidateAppealSearchRowSorted(7, 2, "Creator");
                    ValidateAppealSearchRowSorted(6, 3, "AppealType");
                    _appealSearch.ClickOnFilterOptionListRow(4);
                    _appealSearch.IsListDateSortedInAscendingOrder(9)
                        .ShouldBeTrue("Record Should Be Sorted Ascending Order By Completed Date");
                    _appealSearch.ClickOnFilterOptionListRow(4);
                    _appealSearch.IsListDateSortedInDescendingOrder(9)
                        .ShouldBeTrue("Record Should Be Sorted Descending Order By Completed Date");
                    ValidateAppealSearchRowSorted(8, 6, "Status");
                    _appealSearch.ClickOnFilterOptionListRow(5);
                    _appealSearch.GetUrgentList()
                        .IsInAscendingOrder()
                        .ShouldBeTrue("Appeal Search Row should sorted by Priority");
                    _appealSearch.ClickOnFilterOptionListRow(5);
                    _appealSearch.GetUrgentList()
                        .IsInDescendingOrder()
                        .ShouldBeTrue("Appeal Search Row should sorted by Priority");

                    //new added search filter sort by create date
                    _appealSearch.ClickOnFilterOptionListRow(1);
                    _appealSearch.ClickOnSearchListRow(1);
                    var previousCreateDate = _appealSearch.GetAppealDetailsContentValue(3, 1);
                    for (var i = 2; i < 5; i++)
                    {
                        _appealSearch.ClickOnSearchListRow(i);
                        var currentCreateDate = _appealSearch.GetAppealDetailsContentValue(3, 1);
                        (Convert.ToDateTime(currentCreateDate).Date >= Convert.ToDateTime(previousCreateDate).Date)
                            .ShouldBeTrue(string.Format("Date:<{0} should greater than or equal to Date:{1}",
                                currentCreateDate, previousCreateDate));
                        previousCreateDate = currentCreateDate;
                    }

                    _appealSearch.ClickOnFilterOptionListRow(1);
                    _appealSearch.ClickOnSearchListRow(1);
                    previousCreateDate = _appealSearch.GetAppealDetailsContentValue(3, 1);

                    for (var i = 2; i < 5; i++)
                    {
                        _appealSearch.ClickOnSearchListRow(i);
                        var currentCreateDate = _appealSearch.GetAppealDetailsContentValue(3, 1);
                        (Convert.ToDateTime(previousCreateDate).Date >= Convert.ToDateTime(currentCreateDate).Date)
                            .ShouldBeTrue(string.Format("Date:<{0} should less than or equal to Date:{1}",
                                currentCreateDate, previousCreateDate));
                        previousCreateDate = currentCreateDate;

                    }
                    _appealSearch.ClickOnFilterOptionListRow(6);

                }
                finally
                {
                    if (!_appealSearch.IsFindAppealSectionPresent())
                        _appealSearch.ClickOnSearchButton();
                    _appealSearch.ClickOnFilterOptionListRow(6);
                    _appealSearch.ClickOnAdvancedSearchFilterIcon(false);
                    _appealSearch.ClearAll();
                }

                void ValidateAppealSearchRowSorted(int col, int sortOptionRow, string colName)
                {
                    _appealSearch.ClickOnFilterOptionListRow(sortOptionRow);
                    _appealSearch.IsListStringSortedInAscendingOrder(col)
                        .ShouldBeTrue(string.Format("{0} Should sorted in Ascending Order", colName));
                    _appealSearch.ClickOnFilterOptionListRow(sortOptionRow);
                    _appealSearch.IsListStringSortedInDescendingOrder(col)
                        .ShouldBeTrue(string.Format("{0} Should sorted in Descending Order", colName));
                }
            }
        }


        [Test] //us47369(us45660's child story)
        public void Verify_appeal_details_section_for_client_user()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                AppealSearchPage _appealSearch = automatedBase.CurrentPage.NavigateToAppealSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestExtensions.TestName);

                _appealSearch.GetPageHeader()
                    .ShouldBeEqual("Appeal Search", "Successfully navigated to Appeal search.");
                _appealSearch.SelectAllAppeals();
                _appealSearch.SelectSearchDropDownListValue("Client", ClientEnum.SMTST.ToString(), 4);
                _appealSearch.SetClaimSequence(paramLists["ClaimSequence"]);
                _appealSearch.ClickOnFindButton();
                _appealSearch.WaitForWorkingAjaxMessage();

                _appealSearch.ClickOnSearchListRow(1);
                _appealSearch.WaitForWorkingAjaxMessage();
                if (_appealSearch.IsFindAppealSectionPresent())
                    _appealSearch.ClickOnSearchButton();
                _appealSearch.IsAppealDetailSectionOpen().ShouldBeTrue("Appeal details is Open");
                _appealSearch.GetAppealDetailsLabel(1, 1)
                    .ShouldBeEqual("Product:", "Appeal details product label present");
                _appealSearch.GetAppealDetailsContentValue(1, 1)
                    .ShouldBeEqual(paramLists["Product"], "Appeal details product value present");
                _appealSearch.GetAppealDetailsLabel(1, 2)
                    .ShouldBeEqual("Claim Seq:", "Appeal details Claim Seq label present");
                _appealSearch.GetAppealDetailsContentValue(1, 2)
                    .ShouldBeEqual(paramLists["ClaimSequence"], "Appeal details Claim Seq value present");
                //_appealSearch.GetAppealDetailsLabel(1, 3)
                //    .ShouldEqual("Claim No:", "Appeal details Claim No: label present");
                //_appealSearch.GetAppealDetailsContentValue(1, 3)
                //    .ShouldEqual(paramLists["ClaimNo"], "Appeal details Claim No value present");
                _appealSearch.GetAppealDetailsLabel(2, 1)
                    .ShouldBeEqual("Claim Count:", "Appeal details ClaimCount label present");
                _appealSearch.GetAppealDetailsContentValue(2, 1)
                    .ShouldBeEqual(paramLists["ClaimCount"], "Appeal details ClaimCount value present");
                _appealSearch.GetAppealDetailsLabel(2, 2)
                    .ShouldBeEqual("Prov Seq:", "Appeal details Prov Seq label present");
                _appealSearch.GetAppealDetailsContentValue(2, 2)
                    .ShouldBeEqual(paramLists["ProviderSequence"], "Appeal details Prov Seq value present");
                _appealSearch.GetAppealDetailsLabel(3, 1)
                    .ShouldBeEqual("Create Date:", "Appeal details Create Date label present");
                VerifyThatDateIsInCorrectFormat( _appealSearch.GetAppealDetailsContentValue(3, 1), "Create Date");
                _appealSearch.ClickOnSearchListRow(1);
                if (!_appealSearch.IsFindAppealSectionPresent())
                    _appealSearch.ClickOnSearchButton();
            }
        }


        [Test] //us47369(us45660's child story) + CAR-3121(CAR-3063)
        public void Verify_client_side_search_result_display()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                AppealSearchPage _appealSearch = automatedBase.CurrentPage.NavigateToAppealSearch();
                AppealSummaryPage _appealSummary;

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName,
                    TestExtensions.TestName);

                AppealLetterPage appealLetter = null;
                _appealSearch.GetPageHeader()
                    .ShouldBeEqual("Appeal Search", "Successfully navigated to Appeal search.");
                _appealSearch.SelectAllAppeals();

                _appealSearch.SelectSearchDropDownListValue("Client", ClientEnum.SMTST.ToString(), 4);
                _appealSearch.SetClaimSequence(paramLists["ClaimSequence"]);
                _appealSearch.ClickOnFindButton();
                _appealSearch.WaitForWorkingAjaxMessage();

                _appealSearch.GetAdjustValueByRow(4).ShouldBeEqual("A", "Adjust Icon Should Be Present");
                _appealSearch.GetDenyValueByRow(3).ShouldBeEqual("D", "Deny Icon Should Be Present");
                _appealSearch.GetPayValueByRow(2).ShouldBeEqual("P", "Pay Icon Should Be Present");
                _appealSearch.GetNoDocsValueByRow(1).ShouldBeEqual("N", "No Docs Icon Should Be sPresent");
                _appealSearch.GetSearchResultByRowCol(1, 5)
                    .ShouldBeEqual(ClientEnum.SMTST.ToString(), "Client Code Should display");
                _appealSearch.GetSearchResultByRowCol(2, 6).ShouldBeEqual("A", "Appeal Type Should display");
                _appealSearch.GetSearchResultByRowCol(1, 6).ShouldBeEqual("RR", "Appeal Type Should display");
                VerifyThatDateIsInCorrectFormat( _appealSearch.GetSearchResultByRowCol(1, 2), "Due Date");
                _appealSearch.IsBlackColorDueDatePresent(1).ShouldBeTrue("Black Color Due Date Present");
                VerifyThatNameIsInCorrectFormat(_appealSearch.GetSearchResultByRowCol(1, 7),
                    "Appeal record creater name Should be in <First Name><Last Name>");
                VerifyThatDateIsInCorrectFormat( _appealSearch.GetSearchResultByRowCol(1, 9),
                    "Appeal record completed Date");
                _appealSearch.GetSearchResultByRowCol(1, 8)
                    .ShouldBeEqual("Complete", "Status Should display Complete");
                _appealSearch.GetSearchResultByRowCol(3, 8).ShouldBeEqual("Closed", "Status Should display Closed");
                _appealSearch.IsAppealLetterIconPrsentByRow(3)
                    .ShouldBeTrue("Appeal level Icon should be Present for status Closed");
                appealLetter = _appealSearch.ClickOnAppealLetter(1);
                appealLetter.PageTitle.ShouldBeEqual(appealLetter.CurrentPageTitle, "Appeal Letter Popup");
                automatedBase.CurrentPage =
                    _appealSearch = appealLetter.CloseLetterPopUpPageAndBackToAppealSearch();

                _appealSearch.GetSearchResultByRowCol(1, 3).ShouldNotBeEmpty("Appeal Sequence Should be displayed");
                automatedBase.CurrentPage = _appealSummary = _appealSearch.ClickOnAppealSequence(1);
                _appealSummary.GetPageHeader()
                    .ShouldBeEqual("Appeal Summary", "Appeal Summary Page open for complete status");
                automatedBase.CurrentPage =
                    _appealSearch = _appealSummary.ClickOnSearchIconToNavigateAppealSearchPage();
                _appealSearch.IsAppealLockIconPresent(paramLists["appealSeq"])
                    .ShouldBeFalse("Appeal Should not be Locked");
                _appealSearch.SelectMyAppeals();
                _appealSearch.SelectSearchDropDownListValue("Status", paramLists["StatusNew"], 8);
                _appealSearch.ClickOnFindButton();
                _appealSearch.WaitForWorkingAjaxMessage();

                _appealSearch.GetSearchResultByRowCol(1, 8).ShouldBeEqual("New", "Status Should display New");
                _appealSearch.IsUrgentIconPresentByAppealSeq(paramLists["NonUrgentAppealSeq"]).ShouldBeFalse("Urgent Icon should not display");
                _appealSearch.IsUrgentIconPresentByAppealSeq(paramLists["UrgentAppealSeq"]).ShouldBeTrue("Urgent Icon should be displayed");
                
                _appealSearch.SelectSearchDropDownListValue("Status", paramLists["StatusDocwait"], 8);
                _appealSearch.ClickOnFindButton();
                _appealSearch.WaitForWorkingAjaxMessage();

                _appealSearch.GetSearchResultByRowCol(1, 8)
                    .ShouldBeEqual("Documents Waiting", "Status Should display Documents Waiting");
                _appealSearch.SelectSearchDropDownListValue("Status", paramLists["StatusOpen"], 8);
                _appealSearch.ClickOnFindButton();
                _appealSearch.WaitForWorkingAjaxMessage();

                _appealSearch.GetSearchResultByRowCol(1, 8).ShouldBeEqual("Open", "Status Should display as Open");

                #region CAR-3121(CAR-3063)

                StringFormatter.PrintMessage("Verify Search Result when Medical Record Review is selected");
                _appealSearch.ClickOnClearLink();
                _appealSearch.ClickOnAdvancedSearchFilterIcon(true);
                _appealSearch.SelectClientSmtst();
                _appealSearch.SelectAllAppeals();
                _appealSearch.SelectDropDownListbyInputLabel("Type", "Medical Record Review");
                _appealSearch.GetSideBarPanelSearch.SetDateField("Create Date", "09/20/2020", 1);
                _appealSearch.GetSideBarPanelSearch.SetDateField("Create Date", "09/26/2020", 2);
                _appealSearch.ClickOnFindButton();
                _appealSearch.WaitForWorkingAjaxMessage();
                _appealSearch.GetSearchResultListByCol(3).ShouldCollectionBeEquivalent(
                    _appealSearch.GetMedicalRecordReviewAppealsFromDatabase(),
                    "Results should yield appeal records that are of the Medical Record Review type");

                #endregion
                
            }
        }


        [Test]//US47136
        public void Verify_Search_Validation_for_minimum_set_of_filters()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                AppealSearchPage _appealSearch = automatedBase.CurrentPage.NavigateToAppealSearch();

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var product = paramLists["Product"];
                var client = ClientEnum.SMTST.ToString();
                var status = paramLists["Status"];
                var type = paramLists["Type"];
                var priority = paramLists["Priority"];
                var appealLevel = paramLists["AppealLevel"];
                _appealSearch.SelectAllAppeals();

                _appealSearch.ClickOnAdvancedSearchFilterIcon(true);
                ValidateFieldErrorMessageForComboBox(_appealSearch, "Client", client,
                    "Search cannot be initiated with Client only. A Date Range, Status, Appeal Seq, Appeal #, Claim Seq, or Provider Seq is also required.");
                _appealSearch.SelectSearchDropDownListValue("Client",
                    "All"); //select all client so no error message on client


                ValidateFieldErrorMessageForComboBox(_appealSearch, "Product", product,
                    "Search cannot be initiated with Product only. A date range, Appeal Seq, Claim Seq, Appeal #, or Provider Seq search criteria is required.");

                ValidateFieldErrorMessageForComboBox(_appealSearch, "Status", status,
                    "Search cannot be initiated for all appeals in a status of  Closed. Please select a different Status, a Date Range, or Provider Seq.");

                ValidateFieldErrorMessageForComboBox(_appealSearch, "Type", type,
                    "Search cannot be initiated with Type only. A date range, Client, Status, or Provider Seq search criteria is required.");

                ValidateFieldErrorMessageForComboBox(_appealSearch, "Priority", priority,
                    "Search cannot be initiated with Priority only. A date range, Client, Status, or Provider Seq search criteria is required.");

                ValidateFieldErrorMessageForComboBox(_appealSearch, "Appeal Level", appealLevel,
                    "Search cannot be initiated with Appeal Level only. A date range, Category, Status, or Provider Seq search criteria is required.");
            }
        }

        [Test,Category("AppealDependent")]// TE-800 + TE-889
        public void verify_Appeal_Letter_content_In_Claim_Summary() 

        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                AppealSearchPage _appealSearch = automatedBase.CurrentPage.NavigateToAppealSearch();
                AppealSummaryPage _appealSummary;

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);

                const string reviewDisclaimer =
                    "Cotiviti, Inc. Dental Consultant has reviewed the claim(s) listed below in response to your request for consideration of reimbursement. Our review considered the following documents:There were no document types associated with this request.The results for each line are stated below.";

                const string appealLetterClosing =
                    "A copy of the criteria used to make this determination is available upon request. If you desire a reconsideration of this determination, you or your representative have the right to submit additional documentation, written comments, or any other relevant information prior to submission. This information along with a reconsideration request can be submitted to the insurance provider within 180 days. For any questions please contact Cotiviti, Inc.";
                const string fourthParagraph =
                    "A written response to a reconsideration request will be provided within 30 calendar days of receiving the required documentation. Requests can be expedited in the case of urgent health care services. Expedited responses will be provided within 72 hours. You have the right to request an external appeal review, a peer to peer consultation or a emergency care review by contacting the insurance provider. You may also contact Cotiviti directly by calling the toll free number above.";
                const string contactDetail =

                    "Cotiviti, Inc | Payment Accuracy Division" +
                    "10897 South Riverfront Parkway, Suite 200" +
                    "South Jordan, UT 84095" +
                    "Toll-free: (877) 554-5006" +
                    "Fax: (801) 285-5801" +
                    "Monday-Friday 6:00 am - 6:00 pm MST";

                var footerNote = new List<string>
                {
                    "Reviewed by: Test Automation", "|", "Analyst", "|", "Cotiviti",
                    "clientservices@cotiviti.com", "|", "direct: 801.285.5800", "|", "fax: 801.285.5801",
                };

                _appealSearch.SelectAllAppeals();
                _appealSummary = _appealSearch.SearchByAppealSequence(paramLists["AppealSeq"]);
                var appealLetter = _appealSummary.ClickAppealLetter();
                appealLetter.GetAppealLetterPageHeader().ShouldBeEqual("Appeal Letter", "verify correct header");
                appealLetter.GetReviewDisclaimer().Replace("\r\n", "")
                    .ShouldBeEqual(reviewDisclaimer, "Disclaimer correct?");
                appealLetter.GetAppealLetterClosing().Replace("\r\n", "").ShouldBeEqual(
                    appealLetterClosing + contactDetail + fourthParagraph, "validate appeal letter closing paragraph");
                appealLetter.GetFooterDetail().ShouldCollectionBeEqual(footerNote, "Verification of Footer Note");
                _appealSummary = appealLetter.CloseLetterPopUpPageAndBackToAppealSummary();
                _appealSummary.ClickOnSearchIconToNavigateQAAppealSearchPage();

            }
        }

        /// <summary>
        /// For appeals currently being viewed by another user , an appeal lock will be present in the new appeal search result grid for the same appeal
        /// </summary>
        [Test] //US47692
        public void Verify_appeal_lock_is_present_in_appeal_search_result_for_in_view_appeals()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                AppealSearchPage _appealSearch = automatedBase.CurrentPage.NavigateToAppealSearch();
                AppealSummaryPage _appealSummary;

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var claimSeq = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "ClaimSequence", "Value");
                var lockBy = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName,
                    TestExtensions.TestName, "LockedBy", "Value");
                _appealSearch.SelectAllAppeals();
                _appealSearch.SelectSearchDropDownListValue("Client", ClientEnum.SMTST.ToString(), 4);
                _appealSearch.SetClaimSequence(claimSeq);
                _appealSearch.ClickOnFindButton();
                _appealSearch.WaitForWorkingAjaxMessage();
                var appealSequence = _appealSearch.GetSearchResultByRowCol(1, 3);
                
                try
                {
                    automatedBase.CurrentPage = _appealSummary = _appealSearch.ClickOnAppealSequence(1);
                    var pageUrl = automatedBase.CurrentPage.CurrentPageUrl;
                    _appealSummary.ClickOnSearchIconToNavigateAppealSearchPage();
                    _appealSearch
                        .IsAppealLockIconPresentForAppealSummaryViewAndGetLockIconToolTipMessage(claimSeq,
                            appealSequence, pageUrl)
                        .ShouldBeEqual(string.Format("This appeal is locked by {0}", lockBy),
                            "Appeal Icon is locked and Tooltip Message");
                }

                finally
                {
                    _appealSearch.CloseAnyTabOfAppealSummaryIfExists();
                    _appealSearch.ClearAll();
                    _appealSearch.SelectAllAppeals();
                }
            }
        }

        [Test] //TE-279
        public void Verify_that_users_are_allowed_to_search_by_product_and_status_for_all_appeals()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                AppealSearchPage _appealSearch = automatedBase.CurrentPage.NavigateToAppealSearch();

                _appealSearch.ClickOnQuickLaunch().NavigateToAppealSearch();
                _appealSearch.SelectAllAppeals();
                _appealSearch.ClickOnAdvancedSearchFilterIcon(true);
                _appealSearch.SelectSearchDropDownListValue("Client", "All");
                _appealSearch.SelectSearchDropDownListValue("Product", ProductEnum.DCA.GetStringValue());
                _appealSearch.SelectSearchDropDownListValue("Status",
                    AppealStatusEnum.Open.GetStringDisplayValue());
                _appealSearch.ClickOnFindButton();
                _appealSearch.WaitForWorkingAjaxMessage();
                _appealSearch.IsPageErrorPopupModalPresent().ShouldBeFalse(
                    "User should be allowed to search for appeals when any product and any status other than Closed is selected");
                _appealSearch.GetGridViewSection.GetGridRowCount().ShouldBeGreater(0, "Search should return data");

                StringFormatter.PrintMessage(
                    "Validate Closed appeals can't be search with All Appeals and Product only");
                ValidateFieldErrorMessageForComboBox(_appealSearch, "Status", AppealStatusEnum.Closed.GetStringDisplayValue(),
                    "Search cannot be initiated for all appeals in a status of  Closed. Please select a different Status, a Date Range, or Provider Seq.");

                _appealSearch.SelectSearchDropDownListValue("Product", ProductEnum.DCA.GetStringValue());
                StringFormatter.PrintMessage(
                    "Validate  appeals can't be search with All Appeals, Product and any other search criteria other than none Closed status appeal");
                ValidateFieldErrorMessageForComboBox(_appealSearch, "Client", ClientEnum.SMTST.ToString(),
                    "Search cannot be initiated with Client only. A Date Range, Status, Appeal Seq, Appeal #, Claim Seq, or Provider Seq is also required.");
            }
        }

        [Test]//TE-887
        public void Verify_MentorAppeal_Opens_In_Appeal_Summary()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                AppealSearchPage _appealSearch = automatedBase.CurrentPage.NavigateToAppealSearch();
                AppealSummaryPage _appealSummary;

                _appealSearch.GetSideBarPanelSearch.ClickOnAdvancedSearchFilterIcon(true);
                _appealSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search", "All Appeals");
                _appealSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Status",
                    AppealStatusEnum.MentorReview.GetStringDisplayValue());
                _appealSearch.SelectSMTST();
                _appealSearch.ClickOnFindButton();
                _appealSearch.WaitForWorkingAjaxMessage();
                _appealSearch.GetGridViewSection.GetValueInGridByColRow(8)
                    .ShouldBeEqual("Mentor", "correct appeal status displayed in primary data?");
                _appealSummary = _appealSearch.ClickOnAppealSequenceByStatusOrAppealToGoAppealSummaryPage("Mentor");
                automatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.AppealSummary.GetStringValue(),
                    $"Is {PageHeaderEnum.AppealAction.GetStringValue()} page open?");
                _appealSummary.GetStatusValue().ShouldBeEqual(AppealStatusEnum.MentorReview.GetStringDisplayValue(),
                    "Status Should Match");
                _appealSummary.ClickOnSearchIconToNavigateAppealSearchPage();
            }
        }


        [Test, Category("Working")] // TE-580
        public void Verify_original_sort_order_when_working_from_appeal_to_appeal_for_clientUser()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                AppealSearchPage _appealSearch = automatedBase.CurrentPage.NavigateToAppealSearch();
                AppealSummaryPage _appealSummary;

                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var appealSequence = paramLists["AppealSeq"];
                var createDateFrom = paramLists["DueDate"].Split(',')[0];
                var createDateTo = paramLists["DueDate"].Split(',')[0];

                _appealSearch.UpdateAppealStatusToComplete(appealSequence);
                if (!_appealSearch.IsAdvancedSearchFilterIconSelected())
                    _appealSearch.ClickOnAdvancedSearchFilterIconForMyAppeal();
                _appealSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search", "All Appeals");
                _appealSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Status", paramLists["Status"]);
                //_appealSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Client", paramLists["Client"]);
                _appealSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Type", paramLists["Type"]);
                _appealSearch.GetSideBarPanelSearch.SetDateField("Due Date", createDateFrom, 1);
                _appealSearch.GetSideBarPanelSearch.SetDateField("Due Date", createDateTo, 2);

                _appealSearch.ClickOnFindButton();
                _appealSearch.WaitForWorkingAjaxMessage();
                _appealSearch.ClickOnFilterOptionListRow(1);
                var initialAppealList = _appealSearch.GetSearchResultListByCol(3);

                _appealSearch.IsListStringSortedInAscendingOrder(4)
                    .ShouldBeTrue("Search result list is sorted in ascending order of claim no should be true");
                _appealSummary = _appealSearch.ClickOnAppealSequence(1);
                var totalCount = initialAppealList.Count;
                var lastAppeal = initialAppealList[totalCount - 1];

                foreach (var value in initialAppealList)
                {
                    var appealSeq = _appealSummary.GetAppealSequenceOnHeader();
                    appealSeq.ShouldBeEqual(value,
                        "Appeal Seq should be on order to the appeal seq in Appeal Search page's sorted order");
                    _appealSummary.ClosedAppeal();

                }
                _appealSearch.GetPageHeader().ShouldBeEqual(PageHeaderEnum.AppealSearch.GetStringValue(),
                    "Appeal search page should be retained");
            }
        }

        [Test] //CAR-728
        public void Verify_find_button_is_disabled_when_search_is_active()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                AppealSearchPage _appealSearch = automatedBase.CurrentPage.NavigateToAppealSearch();

                _appealSearch.SelectDropDownListbyInputLabel("Quick Search", "My Appeals");
                _appealSearch.ClickFindAndCheckIfFindButtonIsDisabled()
                    .ShouldBeTrue("Find Button Should be disabled while the search is active.");
                _appealSearch.WaitForWorkingAjaxMessage();
                _appealSearch.GetGridViewSection.GetGridRowCount()
                    .ShouldBeGreater(0, "Search results should be displayed");
                _appealSearch.CheckIfFindButtonIsEnabled()
                    .ShouldBeTrue("Find button should be enabled once the results are displayed");
            }
        }




        #endregion

        

        private void ValidateFieldErrorMessageForDateRange(AppealSearchPage _appealSearch, string fieldName, int row, string message, bool setTheDate)
        {
            if (setTheDate)
            {
                _appealSearch.SetDateFieldFrom(row, DateTime.Now.ToString("MM/d/yyyy"), fieldName);
                _appealSearch.SetDateFieldTo(row, DateTime.Now.AddMonths(3).AddDays(1).ToString("MM/d/yyyy"), fieldName);
            }

            _appealSearch.GetInvalidInputToolTipByLabel(fieldName)
                .ShouldBeEqual(
                    message,
                    string.Format("Field Error Tooltip Message When <{0}> range greater than 3 months", fieldName));
            _appealSearch.ClickOnFindButton();
            _appealSearch.GetPageErrorMessage()
                    .ShouldBeEqual("Invalid or missing data must be resolved before search can be initiated.",
                        "Popup Message  when attempting to search when exclamation icon present");
            _appealSearch.ClosePageError();
            _appealSearch.SetDateField(row, 1, "", fieldName);//clear the from row
        }

        private void VerifyThatDateIsInCorrectFormat(string date, string message)
        {
            Regex.IsMatch(date, @"^(0[1-9]|1[0-2])\/(0[1-9]|1\d|2\d|3[01])\/(19|20)\d{2}$").ShouldBeTrue(message + " '" + date + "' is in format MM/DD/YYYY");
        }
        //<First Name> <Last Name>
        private void VerifyThatNameIsInCorrectFormat(string name, string message)
        {
            Regex.IsMatch(name, @"^(\S+ )+\S+$").ShouldBeTrue(message + " '" + name + "' is in format XXX XXX ");
        }


        private void ValidateFieldErrorMessageForComboBox(AppealSearchPage _appealSearch, string fieldName,
            string value, string message)
        {
            _appealSearch.SelectSearchDropDownListValue(fieldName, value);
            _appealSearch.GetInvalidInputToolTipByLabel(fieldName)
                .ShouldBeEqual(
                    message,
                    string.Format("Field Error Tooltip Message When <{0}> is selected only", fieldName));
            _appealSearch.ClickOnFindButton();
            _appealSearch.GetPageErrorMessage()
                    .ShouldBeEqual("Invalid or missing data must be resolved before search can be initiated.",
                        "Popup Message  when attempting to search when exclamation icon present");
            _appealSearch.ClosePageError();
            _appealSearch.ClearAll();
        }

        

      
    }
}

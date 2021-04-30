using System;
using static System.String;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Nucleus.Service.Data;
using Nucleus.Service.PageServices.Provider;
using Nucleus.Service.Support.Common.Constants;
using Nucleus.Service.Support.Enum;
using Nucleus.UIAutomation.TestSuites.Base;
using NUnit.Framework;
using UIAutomation.Framework.Utils;
using static System.Console;
using Extensions = Nucleus.Service.Support.Utils.Extensions;

namespace Nucleus.UIAutomation.TestSuites.TestCases.ClientUser
{
    public class ProviderClaimSearchClient : AutomatedBaseClient
    {
        #region PRIVATE FIELDS

        private ProviderClaimSearchPage _providerClaimSearch;
        private ProviderActionPage _providerAction;
        private ProviderSearchPage _providerSearch;
        private string _providerSequence = string.Empty;

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
        protected override void ClassInit()
        {
            try
            {
                base.ClassInit();
                _providerSequence = DataHelper.GetSingleTestData(FullyQualifiedClassName, "ClassInit", "ProviderSequence", "Value");
                _providerSearch = QuickLaunch.NavigateToProviderSearch();

            }
            catch (Exception)
            {
                if (StartFlow != null)
                    StartFlow.Dispose();
                throw;
            }
        }

        #region OVERRIDE METHODS

        protected override void TestCleanUp()
        {
            if (_providerClaimSearch.IsPageErrorPopupModalPresent())
                _providerClaimSearch.ClosePageError();

            if (string.Compare(UserType.CurrentUserType, UserType.CLIENT, StringComparison.OrdinalIgnoreCase) != 0)
            {
                CurrentPage = QuickLaunch = CurrentPage.Logout().LoginAsClientUser();
                _providerSearch = QuickLaunch.NavigateToProviderSearch();

            }
            if (_providerSearch.GetPageHeader() != PageHeaderEnum.ProviderSearch.GetStringValue())
            {
                CurrentPage.NavigateToProviderSearch();
            }
            _providerSearch.GetSideBarPanelSearch.OpenSidebarPanel();
            _providerSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search", ProviderEnum.AllProviders.GetStringValue());
            base.TestCleanUp();
        }

        #endregion


        #region TEST SUITES

        [Test] //(CAR-1795) CAR-1459
        public void Navigation_and_template_for_client_users()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            var providerSequence = DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                "ProviderSequence", "Value");

            var listOfHistoryOptions = DataHelper.GetMappingData(FullyQualifiedClassName, "History_Options").Select(s => s.Value).ToList();
            _providerSearch.SearchByProviderSequence(providerSequence);
            _providerAction = _providerSearch.ClickOnFirstProviderSeqToNavigateToProviderActionPage();

            _providerAction.GetAvailableHistoryOptions().ShouldCollectionBeEqual(listOfHistoryOptions, "Are the History Options as expected ?");

            StringFormatter.PrintMessageTitle("Verification of navigation to the Provider Claim Search");
            _providerClaimSearch = _providerAction.ClickProviderClaimSearchInHxToNavigateToProviderClaimSearchPage();
            _providerClaimSearch.GetPageHeader().ShouldBeEqual(Extensions.GetStringValue(PageHeaderEnum.ProviderClaimSearch),
                "Is user navigated to 'Provider Claim Search' page once selected from the HX dropdown in Provider Action Page");

            _providerClaimSearch.GetProviderSequenceFromHeader().ShouldBeEqual(providerSequence, "Provider Sequence is getting displayed correctly next to the page header");

            _providerClaimSearch.GetSideBarPanelSearch.IsSideBarPanelOpen().ShouldBeTrue("Sidebar panel should be opened by default");
            _providerClaimSearch.GetSideBarPanelSearch.GetTopHeaderName().ShouldBeEqual("Find Provider Claims", "Is the sidebar panel header name correct?");

            _providerClaimSearch.GetSideBarPanelSearch.ClickOnToggleSidebarPanelButton();
            _providerClaimSearch.GetSideBarPanelSearch.IsSideBarPanelOpen().ShouldBeFalse("Clicking on 'Find Provider Claims' icon will close the sidebar panel");

            StringFormatter.PrintMessageTitle("Verification navigating back to the Provider Action Page by clicking the Search icon");
            _providerAction = _providerClaimSearch.ClickOnSearchIconToReturnToProviderActionPage();
            _providerAction.GetPageHeader().ShouldBeEqual(Extensions.GetStringValue(PageHeaderEnum.ProviderAction),
                "Clicking on 'Search icon in Provider Claim Search page should return to the Provider Action page for the provider'");
            _providerAction.GetProviderSequence().ShouldBeEqual(providerSequence,
                "The opened Provider Action should be for the same provider for which the Provider Claim Search page was opened");
        }

        [Test] //CAR 1460
        public void Verify_quick_search_filters_options()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            var expectedQuickSearchOptions = DataHelper
                .GetMappingData(FullyQualifiedClassName, "quick_search_options").Values.ToList();
            IDictionary<string, string> paramLists =
                DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);

            var errorMessageSearchWithoutAnyCriteria = paramLists["Message1"];

            _providerSearch.SearchByProviderSequence(_providerSequence);
            _providerAction = _providerSearch.ClickOnFirstProviderSeqToNavigateToProviderActionPage();
            _providerClaimSearch = _providerAction.ClickProviderClaimSearchInHxToNavigateToProviderClaimSearchPage();

            StringFormatter.PrintMessageTitle("Validate Quick Search dropdown");
            ValidateQuickSearchDropDownForDefaultValueAndExpectedList("Quick Search", expectedQuickSearchOptions);

            Verify_correct_search_filter_options_displayed_for("last_12_months", ProviderClaimSearchEnum.Last12Months.GetStringValue());

            _providerClaimSearch.GetSideBarPanelSearch.ClickOnFindButton();
            _providerClaimSearch.IsPageErrorPopupModalPresent().ShouldBeTrue("Page error pop up should be present");
            _providerClaimSearch.GetPageErrorMessage().ShouldBeEqual(errorMessageSearchWithoutAnyCriteria, "Search cannot be initiated without any criteria entered.");
            _providerClaimSearch.ClosePageError();

            var activeFlagList = _providerClaimSearch.GetSideBarPanelSearch.GetAvailableDropDownList("Flag");
            activeFlagList.RemoveAt(0);
            _providerClaimSearch.GetActiveFlagsFromDatabase().ShouldBeEqual(activeFlagList, "Flag list should match");

            _providerClaimSearch.GetSideBarPanelSearch.GetInputValueByLabel("Proc Code").ShouldBeNullorEmpty("Default value of Proc Code field should be empty.");
            _providerClaimSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Proc Code", Concat(Enumerable.Repeat("33334", 5)));
            _providerClaimSearch.GetSideBarPanelSearch.GetLengthOfTheInputFieldByLabel("Proc Code")
                .ShouldBeEqual(5, "Max char limit of Proc Code field should be 5");


            _providerClaimSearch.GetSideBarPanelSearch.GetInputValueByLabel("Dx Code").ShouldBeNullorEmpty("Default value of Dx Code field should be empty.");
            _providerClaimSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Dx Code", "123ABC.def123y");
            _providerClaimSearch.GetSideBarPanelSearch.GetLengthOfTheInputFieldByLabel("Dx Code")
                .ShouldBeEqual(12, "Max char limit of Dx Code field should be 12");


            _providerClaimSearch.GetSideBarPanelSearch.GetInputValueByLabel("Modifier").ShouldBeNullorEmpty("Default value of Modifier field should be empty.");
            _providerClaimSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Modifier", Concat(Enumerable.Repeat("a1234", 2)));
            _providerClaimSearch.GetSideBarPanelSearch.GetLengthOfTheInputFieldByLabel("Modifier")
                .ShouldBeEqual(2, "Max char limit of Modifier field should be 2");

            _providerClaimSearch.GetSideBarPanelSearch.GetInputValueByLabel("Patient Seq").ShouldBeNullorEmpty("Default value of Patient Seq field should be empty.");
            _providerClaimSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Patient Seq", Concat(Enumerable.Repeat("33334", 5)));
            _providerClaimSearch.GetSideBarPanelSearch.GetInputValueByLabel("Patient #").ShouldBeNullorEmpty("Default value of Patient # field should be empty.");
            _providerClaimSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Patient #", Concat(Enumerable.Repeat("a1234", 10)));
            var dropDownListForGender = _providerClaimSearch.GetSideBarPanelSearch.GetAvailableDropDownList("Patient Gender");
            dropDownListForGender.RemoveAt(0);
            dropDownListForGender.ShouldCollectionBeEqual(DataHelper.GetMappingData(FullyQualifiedClassName, "gender_option").Values.ToList(), "Gender list should match");
            _providerClaimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Patient Gender", "Male");

            _providerClaimSearch.GetSideBarPanelSearch.ClickOnClearLinkCssSelector();
            _providerClaimSearch.GetSideBarPanelSearch.GetInputValueByLabel("Quick Search")
                .Equals(ProviderClaimSearchEnum.Last12Months.GetStringValue());

            _providerClaimSearch.GetSideBarPanelSearch.GetInputAttributeValueByLabel("Flag", "placeholder").ShouldBeEqual("Select Flag", "Flag");
            _providerClaimSearch.GetSideBarPanelSearch.GetInputValueByLabel("Proc Code").ShouldBeNullorEmpty("Default value of Proc Code field should be empty.");
            _providerClaimSearch.GetSideBarPanelSearch.GetInputValueByLabel("Dx Code").ShouldBeNullorEmpty("Default value of Dx Code field should be empty.");
            _providerClaimSearch.GetSideBarPanelSearch.GetInputValueByLabel("Modifier").ShouldBeNullorEmpty("Default value of Modifier field should be empty.");
            _providerClaimSearch.GetSideBarPanelSearch.GetInputValueByLabel("Patient Seq").ShouldBeNullorEmpty("Default value of Patient Seq field should be empty.");
            _providerClaimSearch.GetSideBarPanelSearch.GetInputValueByLabel("Patient #").ShouldBeNullorEmpty("Default value of Patient # field should be empty.");
            _providerClaimSearch.GetSideBarPanelSearch.GetInputAttributeValueByLabel("Patient Gender", "placeholder")
                .ShouldBeEqual("Type to filter", "Patient Gender");


            _providerClaimSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Proc Code", paramLists["InvalidProcCode"]);
            _providerClaimSearch.GetSideBarPanelSearch.ClickOnFindButton();
            _providerClaimSearch.IsPageErrorPopupModalPresent().ShouldBeTrue("Page error pop up should be present");
            _providerClaimSearch.GetPageErrorMessage().ShouldBeEqual("Proc Code " + paramLists["InvalidProcCode"] + " is not a valid value.");
            _providerClaimSearch.ClosePageError();
            _providerClaimSearch.GetSideBarPanelSearch.ClickOnClearLinkCssSelector();

            _providerClaimSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Dx Code", paramLists["InvalidDxCode"]);
            _providerClaimSearch.GetSideBarPanelSearch.ClickOnFindButton();
            _providerClaimSearch.IsPageErrorPopupModalPresent().ShouldBeTrue("Page error pop up should be present");
            _providerClaimSearch.GetPageErrorMessage().ShouldBeEqual("Dx Code " + paramLists["InvalidDxCode"] + " is not a valid value.");
            _providerClaimSearch.ClosePageError();
            _providerClaimSearch.GetSideBarPanelSearch.ClickOnClearLinkCssSelector();


            StringFormatter.PrintMessageTitle("Validate fields for All Claims Quick Search option");
            _providerClaimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search", ProviderClaimSearchEnum.AllClaims.GetStringValue());
            Verify_correct_search_filter_options_displayed_for("all_claims", ProviderClaimSearchEnum.AllClaims.GetStringValue());
            _providerClaimSearch.GetSideBarPanelSearch.ClickOnFindButton();
            _providerClaimSearch.IsPageErrorPopupModalPresent().ShouldBeTrue("Page error pop up should be present");
            _providerClaimSearch.GetPageErrorMessage().ShouldBeEqual(errorMessageSearchWithoutAnyCriteria, "Search cannot be initiated without any criteria entered.");
            _providerClaimSearch.ClosePageError();
            _providerClaimSearch.GetSideBarPanelSearch.GetInputValueByLabel("Claim Seq").ShouldBeNullorEmpty("Default value of Claim Seq field should be empty.");
            _providerClaimSearch.GetSideBarPanelSearch.GetInputValueByLabel("Claim No").ShouldBeNullorEmpty("Default value of Claim No field should be empty.");

            StringFormatter.PrintMessageTitle("Validate Begin DOS-End DOS field");
            ValidateDatePickerField("Begin DOS - End DOS", paramLists["Message1"], paramLists["Message2"], paramLists["Message3"]);

            _providerClaimSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Claim Seq", Concat(Enumerable.Repeat("33334", 5)));
            _providerClaimSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Claim No", Concat(Enumerable.Repeat("a1234", 10)));
            _providerClaimSearch.GetSideBarPanelSearch.ClickOnClearLinkCssSelector();
            _providerClaimSearch.GetSideBarPanelSearch.GetInputValueByLabel("Claim Seq").ShouldBeNullorEmpty("Default value of Proc Code field should be empty.");
            _providerClaimSearch.GetSideBarPanelSearch.GetInputValueByLabel("Claim No").ShouldBeNullorEmpty("Default value of Dx Code field should be empty.");
        }

        [Test] //TE-559
        public void Verify_Sorting_Of_Provider_Claim_Search_For_Client()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            IDictionary<string, string> paramLists = DataHelper.GetTestData(FullyQualifiedClassName,
                TestExtensions.TestName);
            var filterOptions =
                DataHelper.GetMappingData(FullyQualifiedClassName, "Sorting_Options").Values.ToList();
            var prvSeq = paramLists["ProviderSequence"];
            var beginDOS = paramLists["BeginDOS"];
            var endDos = paramLists["EndDOS"];
           
           
                
                _providerSearch.SearchByProviderSequence(paramLists["ProviderSequence"]);
                _providerAction = _providerSearch.ClickOnFirstProviderSeqToNavigateToProviderActionPage();
                _providerClaimSearch =
                    _providerAction.ClickProviderClaimSearchInHxToNavigateToProviderClaimSearchPage();
                var expectedListInAscending =
                    _providerClaimSearch.GetClaimsearcResultInAscendingOrder(prvSeq, beginDOS, endDos);
                var expectedListInDescending =
                    _providerClaimSearch.GetClaimsearcResultInDescendingOrder(prvSeq, beginDOS, endDos);
            _providerClaimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search", "All Claims");
                _providerClaimSearch.GetSideBarPanelSearch.SetDateField("Begin DOS - End DOS",
                    beginDOS, 1);
                _providerClaimSearch.GetSideBarPanelSearch.SetDateField("Begin DOS - End DOS",
                    endDos, 2);
                _providerClaimSearch.GetSideBarPanelSearch.ClickOnFindButton();
                _providerClaimSearch.WaitForWorking();


                StringFormatter.PrintMessage("verify sorting options");
                _providerClaimSearch.IsFilterOptionPresent().ShouldBeTrue("Is Filter Option Icon Present?");
                _providerClaimSearch.GetFilterOptionTooltip()
                    .ShouldBeEqual("Sort Results", "Correct tooltip is displayed");
                _providerClaimSearch.GetFilterOptionList()
                    .ShouldCollectionBeEqual(filterOptions, "Filter Options Lists Collection Should Equal");

                StringFormatter.PrintMessage("verify default sorting by claim sequence");

                _providerClaimSearch.GetGridViewSection.GetGridListValueByCol(2).ShouldCollectionBeEqual(
                    expectedListInAscending.Select(x => x[1]).ToList(), "Claim sequence in ascending order?");
                _providerClaimSearch.GetGridViewSection.GetGridListValueByCol(1).ShouldCollectionBeEqual(expectedListInAscending.Select(x => x[0]).ToList(), "Line Number in ascending order?");
                _providerClaimSearch.ClickOnFilterOptionListRow(1);
                _providerClaimSearch.GetGridViewSection.GetGridListValueByCol(2).ShouldCollectionBeEqual(
                    expectedListInDescending.Select(x => x[1]).ToList(), "Claim sequence in descending order?");
                _providerClaimSearch.GetGridViewSection.GetGridListValueByCol(1).ShouldCollectionBeEqual(expectedListInDescending.Select(x => x[0]).ToList(), "Line Number in descending order?");


                VerifySortingOptionsInProviderClaimSearch(5, 2, filterOptions[1]);
                VerifySortingOptionsInProviderClaimSearch(6, 3, filterOptions[2]);
                VerifySortingOptionsInProviderClaimSearch(7, 4, filterOptions[3]);
                VerifySortingOptionsInProviderClaimSearch(8, 5, filterOptions[4]);
                VerifySortingOptionsInProviderClaimSearch(9, 6, filterOptions[5]);
                VerifySortingOptionsInProviderClaimSearch(10, 7, filterOptions[6]);
                _providerClaimSearch.clickOnClearSort();
                _providerClaimSearch.GetGridViewSection.GetGridListValueByCol().ShouldCollectionBeEqual(
                    expectedListInAscending.Select(x => x[1]).ToList(), "Claim sequence in ascending order?");
                _providerClaimSearch.GetGridViewSection.GetGridListValueByCol(1).ShouldCollectionBeEqual(expectedListInAscending.Select(x => x[0]).ToList(), "Line Number in ascending order?");

           

        }

        [Test] //(CAR-1462,TE-591)
        public void Verify_search_results_secondary_data_for_client()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            var providerSequence = DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                "ProviderSequence", "Value");
            var claSeq = DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                "ClaimSequence", "Value");

            var providerSequence1 = DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                "ProviderSequence1", "Value");
            var claSeq1 = DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                "ClaimSequence1", "Value");
            _providerSearch.SearchByProviderSequence(providerSequence);
            _providerAction = _providerSearch.ClickOnFirstProviderSeqToNavigateToProviderActionPage();
            _providerClaimSearch = _providerAction.ClickProviderClaimSearchInHxToNavigateToProviderClaimSearchPage();

            var claimLineDetailsSecondaryDataList = _providerClaimSearch.GetSecondaryDetailsForClaimLineData(providerSequence, claSeq, "1");
            var claimno = claimLineDetailsSecondaryDataList[0];
            var procCodeDescription = claimLineDetailsSecondaryDataList[13];
            var revCodeDescription = claimLineDetailsSecondaryDataList[14];
            var adjStatus = claimLineDetailsSecondaryDataList[27];
            var modifierDescription = "";
            var dxCodeDescription = "";
            var dxCodes = "";
            int i;
            for (i = 1; i < 13; i++)
            {
                if (claimLineDetailsSecondaryDataList[i] != "" && i < 4)
                {
                    modifierDescription = i == 1 ? Concat(modifierDescription, claimLineDetailsSecondaryDataList[i + 14]) : Concat(modifierDescription, "\r\n", claimLineDetailsSecondaryDataList[i + 14]);
                }
                if (claimLineDetailsSecondaryDataList[i] != "" && i > 3)
                {
                    dxCodeDescription = i == 4 ? Concat(dxCodeDescription, claimLineDetailsSecondaryDataList[i + 14]) : Concat(dxCodeDescription, "\r\n", claimLineDetailsSecondaryDataList[i + 14]);
                }
            }
            if (!_providerClaimSearch.GetSideBarPanelSearch.IsSideBarPanelOpen())
            {
                _providerClaimSearch.GetSideBarPanelSearch.OpenSidebarPanel();
            }
            _providerClaimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search", ProviderClaimSearchEnum.AllClaims.GetStringValue());
            _providerClaimSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Claim Seq", claSeq);
            _providerClaimSearch.GetSideBarPanelSearch.ClickOnFindButton();
            _providerClaimSearch.GetGridViewSection.ClickOnGridRowByRowInClaimSearch();
            _providerClaimSearch.IsLabelPresent("Claim No:");
            _providerClaimSearch.IsLabelPresent("Adj Stats:");
            _providerClaimSearch.IsLabelPresent("Proc Code:");
            _providerClaimSearch.IsLabelPresent("Rev Code:");
            _providerClaimSearch.IsLabelPresent("Modifiers");
            _providerClaimSearch.IsLabelPresent("Dx Codes");
            _providerClaimSearch.GetClaimDetailsValueByLabel("Claim No:").ShouldBeEqual(claimno,
                $"Claim No value should be equal to the alt claim no. {claimno}");
            _providerClaimSearch.GetClaimDetailsValueByLabel("Adj Status:").ShouldBeEqual(adjStatus,
                $"Adj Status value should be equal to {adjStatus}");
            _providerClaimSearch.GetProcCodeValueByLabel("Proc Code:").ShouldBeEqual(procCodeDescription,
                $"Proc Code value should be equal to {procCodeDescription}");
            _providerClaimSearch.GetClaimDetailsValueByLabel("Rev Code:").ShouldBeEqual(revCodeDescription,
                $"Rev Code value should be equal to {revCodeDescription}");
            _providerClaimSearch.GetModifierValueByLabel("Modifiers")
                .ShouldBeEqual(modifierDescription, $"Modifiers List should be equal to {modifierDescription}");
            for (int j = 2; j <= 10; j++)
            {
                dxCodes = j == 2 ? Concat(dxCodes, _providerClaimSearch.GetDxCodesValueByLabel("Dx Codes", j)) : Concat(dxCodes, "\r\n", _providerClaimSearch.GetDxCodesValueByLabel("Dx Codes", j));
            }

            dxCodes.ShouldBeEqual(dxCodeDescription, $"Dx Code value should be equal to {dxCodeDescription}");
            _providerClaimSearch.GetGridViewSection.IsGridRowSelected().ShouldBeTrue("Selected claim line record should be highlighted.");

            _providerClaimSearch.ClickOnSearchIconToReturnToProviderActionPage().ClickOnSearchIconAtHeaderReturnToProviderSearchPage();
            _providerSearch.SearchByProviderSequence(providerSequence1);
            _providerAction = _providerSearch.ClickOnFirstProviderSeqToNavigateToProviderActionPage();
            _providerClaimSearch = _providerAction.ClickProviderClaimSearchInHxToNavigateToProviderClaimSearchPage();
            _providerClaimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search", ProviderClaimSearchEnum.AllClaims.GetStringValue());
            _providerClaimSearch.GetSideBarPanelSearch.SetInputFieldByLabel("Claim Seq", claSeq1);
            _providerClaimSearch.GetSideBarPanelSearch.ClickOnFindButton();
            _providerClaimSearch.GetGridViewSection.ClickOnGridRowByRowInClaimSearch();
            _providerClaimSearch.GetDxCodesValueByLabel("Dx Codes",2).AssertIsContained(
                "No description available", "No description available when Code Description is not available");
            _providerClaimSearch.GetClaimDetailsValueByLabel("Rev Code:").AssertIsContained(
                "No description available", "No description available when Code Description is not available");
            _providerClaimSearch.GetModifierValueByLabel("Modifiers").AssertIsContained(
                "No description available", "No description available when Code Description is not available");

        }

        [Test] //CAR-1461 (CAR-1856)
        public void Verify_search_results_in_provider_claim_search_for_client_users()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            IDictionary<string, string> paramLists = DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
            var providerSequence = paramLists["ProviderSequence"];
            var beginDOS = paramLists["DOS"];
            var endDOS = paramLists["EDOS"];

            StringFormatter.PrintMessage("Navigating to the Provider Claim Search page");
            _providerSearch.ClearAll();
            _providerClaimSearch = _providerSearch.SearchByProviderSequenceAndNavigateToProviderClaimSearchPage(providerSequence);

            StringFormatter.PrintMessage("Performing a search based on Begin DOS and End DOS");
            _providerClaimSearch.SearchByBeginDOSAndEndDOS(beginDOS, endDOS);

            StringFormatter.PrintMessageTitle("Verification of Search Result in the grid");
            _providerClaimSearch.GetGridViewSection.GetGridListValueByCol(2)
                .ShouldCollectionBeSorted(false, "The default search result is sorted in ascending order of claim sequence values");

            StringFormatter.PrintMessage("Verification of 'Load More' button when search result is greater than 25");
            var countOfExpectedResultFromDB = _providerClaimSearch.GetCountOfProviderClaimSearchResultByBeginDOSEndDOS(providerSequence, beginDOS, endDOS);
            var loadMoreValue = _providerClaimSearch.GetPagination.GetLoadMoreText();

            _providerClaimSearch.GetGridViewSection.GetGridRowCount().ShouldBeEqual(25, "Search Grid should show up to 25 records at once");

            loadMoreValue.ShouldBeEqual($"Viewing 25 of {countOfExpectedResultFromDB} (Load More)", "Verification of Load More text");
            _providerClaimSearch.GetPagination.ClickOnLoadMore();
            loadMoreValue = _providerClaimSearch.GetPagination.GetLoadMoreText();
            var numbers = Regex.Split(loadMoreValue, @"\D+").Where(s => s != Empty).Select(m => int.Parse(m.Trim())).ToList();
            numbers[0].ShouldBeGreater(25, "Load More Text Count should Updated");
            numbers[1].ShouldBeEqual(countOfExpectedResultFromDB, "Total Count Should be same");
            _providerClaimSearch.GetGridViewSection.IsVerticalScrollBarPresentInGridSection().ShouldBeTrue("Vertical Scrollbar should Present");

            ValidateColumnFieldsOfSearchResult(paramLists);

            _providerClaimSearch.ClickOnClaimSeqInGridToOpenClaimActionPopUp(1, 2);
            CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.ClaimAction.GetStringValue(), "Claim Sequence is a hyperlink that, when selected, will show the associated " +
                                                                      "Claim Action page in pop up view");
            _providerClaimSearch.CloseAnyPopupIfExist();

            StringFormatter.PrintMessage("Verifying whether search is retained ");
            var countOfRecords = _providerClaimSearch.GetGridViewSection.GetGridRowCount();

            _providerClaimSearch.ClickOnSearchIconToReturnToProviderActionPage().ClickProviderClaimSearchInHxToNavigateToProviderClaimSearchPage();

            _providerClaimSearch.GetGridViewSection.GetGridRowCount().ShouldBeEqual(countOfRecords, "Search filters should be retained when " +
                                                                                                    "switching between Provider Action and Provider Claim Search");

            StringFormatter.PrintMessageTitle("Verification of search result for a Claseq with multiple Clasubs");
            _providerSearch = _providerClaimSearch.NavigateToProviderSearchFromProviderClaimSearchPage();
            _providerClaimSearch = _providerSearch.SearchByProviderSequenceAndNavigateToProviderClaimSearchPage(paramLists["ProviderSequenceForDifferentClasub"]);

            _providerClaimSearch.SearchByClaimSequence(paramLists["ClaseqWithDifferentClasub"]);
            var listOfClaseqClasubFromDB = _providerClaimSearch.GetClaimSeqForMultipleClaSub(paramLists["ProviderSequenceForDifferentClasub"], paramLists["ClaseqWithDifferentClasub"]);

            _providerClaimSearch.GetGridViewSection.GetGridListValueByCol(2).ShouldCollectionBeEqual(listOfClaseqClasubFromDB,
                "Claseq with multiple clasub should be displayed correctly");
        }
        #endregion

        #region PRIVATE

        private void ValidateColumnFieldsOfSearchResult(IDictionary<string, string> testData)
        {
            StringFormatter.PrintMessage("Verification of the Line Number");
            var lineNoListFromGrid = _providerClaimSearch.GetGridViewSection.GetGridListValueByCol(1);
            lineNoListFromGrid.Where(x => Regex.Match(x, @"^[\d]+$").Success).ToList().ShouldCollectionBeEqual(lineNoListFromGrid,
                "Line Numbers should be listed in proper format");

            StringFormatter.PrintMessage("Verification of the Claseq and Clasub");
            var claseqClasubListFromGrid = _providerClaimSearch.GetGridViewSection.GetGridListValueByCol();
            claseqClasubListFromGrid.Where(x => Regex.Match(x, @"^[\d]+-[\d]+$").Success).ToList().ShouldCollectionBeEqual(claseqClasubListFromGrid,
                "Claseq and clasub should be listed in proper format as 'claseq - clasub'");

            StringFormatter.PrintMessage("Verification of Patient Name");
            var patientNameListFromGrid = _providerClaimSearch.GetGridViewSection.GetGridListValueByCol(3);
            patientNameListFromGrid.Where(x => Regex.Match(x, @"^[A-Za-z]+, [A-Za-z]+$").Success).ToList().ShouldCollectionBeEqual(patientNameListFromGrid,
                "Patient Name should be listed in proper format as <Last Name>, <First Name>");

            StringFormatter.PrintMessage("Verification of Patient Age");
            var patientAgeListFromGrid = _providerClaimSearch.GetGridViewSection.GetGridListValueByCol(4);
            patientAgeListFromGrid.Where(x => Regex.Match(x, @"^[\d]+(\s)*y [\d]+(\s)*m$").Success).ToList().ShouldCollectionBeEqual(patientAgeListFromGrid,
                "Patient age should be listed in proper format as <years> y <months> m");

            StringFormatter.PrintMessage("Verification of Patient Gender");
            _providerClaimSearch.GetGridViewSection.GetLabelInGridByColRow(5, 1).ShouldBeEqual("Gen:", "Gender label should be 'Gen'");
            var patientGenderListFromGrid = _providerClaimSearch.GetGridViewSection.GetGridListValueByCol(5);
            patientGenderListFromGrid.Where(x => Regex.Match(x, @"^[MFU]$").Success).ToList().ShouldCollectionBeEqual(patientGenderListFromGrid,
                "Patient gender should be listed correctly as either M, F or U");

            StringFormatter.PrintMessage("Verification of Diagnosis Code");
            _providerClaimSearch.GetGridViewSection.GetLabelInGridByColRow(6).ShouldBeEqual("Dx1:", "Diagnosis Code should be 'Dx1'");
            var diagnosisCodeListFromGrid = _providerClaimSearch.GetGridViewSection.GetGridListValueByCol(6);
            diagnosisCodeListFromGrid.Where(x => Regex.Match(x, @"^\d+.\d+$").Success).ToList().ShouldCollectionBeEqual(diagnosisCodeListFromGrid,
                "Diagnosis Code should be listed correctly with decimal points");

            StringFormatter.PrintMessage("Verification of Begin DOS");
            _providerClaimSearch.GetGridViewSection.GetLabelInGridByColRow(7).ShouldBeEqual("DOS:", "Begin DOS label should be 'DOS'");
            var beginDOSListFromGrid = _providerClaimSearch.GetGridViewSection.GetGridListValueByCol(7);
            beginDOSListFromGrid.Where(x => Regex.Match(x, @"^[\d]{2}/[\d]{2}/[\d]{4}$").Success).ToList().ShouldCollectionBeEqual(beginDOSListFromGrid,
                "Diagnosis Code should be listed correctly with decimal points");

            StringFormatter.PrintMessage("Verification of Procedure Code");
            var procCodeListFromGrid = _providerClaimSearch.GetGridViewSection.GetGridListValueByCol(9);
            procCodeListFromGrid.Where(x => x != "").Where(x => Regex.Match(x, @"^[\S]{5}$").Success).ToList().ShouldCollectionBeEqual(procCodeListFromGrid.Where(x => x != ""),
                "Proc Code should be listed correctly as 5 digits");

            StringFormatter.PrintMessage("Verification of Paid Column");
            _providerClaimSearch.GetGridViewSection.GetLabelInGridByColRow(10).ShouldBeEqual("Paid:", "Paid label should be 'Paid'");
            var paidListFromGrid = _providerClaimSearch.GetGridViewSection.GetGridListValueByCol(10);
            paidListFromGrid.Select(s => s.TrimStart('$'))
                .All(trig => _providerClaimSearch.IsValueCurrency(trig)).ShouldBeTrue("Paid Value should be in currency format");

            StringFormatter.PrintMessage("Verification of Modifier Field");
            _providerClaimSearch.GetSideBarPanelSearch.ClickOnClearLinkCssSelector();
            _providerClaimSearch.SearchByClaimSequence(testData["ClaseqHavingModifierDetails"]);

            _providerClaimSearch.GetGridViewSection.GetLabelInGridByColRow(8).ShouldBeEqual("M1:", "Modifier label should be M1");
            _providerClaimSearch.GetGridViewSection.GetValueInGridByColRow(8).ShouldBeEqual(testData["ExpectedModifier"], $"Modifier value should be {testData["ExpectedModifier"]}");

            StringFormatter.PrintMessage("Verification of Flag details");
            _providerSearch = _providerClaimSearch.NavigateToProviderSearchFromProviderClaimSearchPage();

            _providerClaimSearch = _providerSearch.SearchByProviderSequenceAndNavigateToProviderClaimSearchPage(testData["ProviderSequenceHavingFlagDetails"]);

            _providerClaimSearch.SearchByClaimSequence(testData["ClaimSeqHavingFlagDetails"]);
            _providerClaimSearch.GetGridViewSection.GetValueInGridByColRow(11).ShouldBeEqual(testData["ExpectedFlag"],
                "Top Flag should be displayed for the line");
        }
        private void ValidateQuickSearchDropDownForDefaultValueAndExpectedList(string label, IList<string> collectionToEqual)
        {
            var actualDropDownList = _providerClaimSearch.GetSideBarPanelSearch.GetAvailableDropDownList(label);
            actualDropDownList.Count.ShouldBeGreater(0, "List of " + label + " is greater than zero.");
            actualDropDownList.RemoveAll(IsNullOrEmpty);
            if (collectionToEqual != null)
                actualDropDownList.ShouldCollectionBeEqual(collectionToEqual, label + " List As Expected");
            _providerClaimSearch.GetSideBarPanelSearch.GetInputValueByLabel(label)
                .ShouldBeEqual(ProviderClaimSearchEnum.Last12Months.GetStringValue(), label + " value defaults to Last 12 Months");
            _providerClaimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel(label, actualDropDownList[1],
                false); //check for type ahead functionality
            _providerClaimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel(label, actualDropDownList[0]);
            _providerClaimSearch.GetSideBarPanelSearch.GetInputValueByLabel(label)
                .ShouldBeEqual(actualDropDownList[0], "User can select only a single option");

        }

        private void VerifySortingOptionsInProviderClaimSearch(int col, int option, string sortoption)
        {
            StringFormatter.PrintMessageTitle("Validation of Sorting by " + sortoption);
            _providerClaimSearch.ClickOnFilterOptionListRow(option);

            switch (sortoption)
            {
                case "Paid":
                    _providerClaimSearch.IsListInAscendingOrderByPaid()
                        .ShouldBeTrue("Search result must be sorted by " + sortoption + " in Ascending");
                    _providerClaimSearch.ClickOnFilterOptionListRow(option);
                    _providerClaimSearch.IsListInDescendingOrderByPaid()
                        .ShouldBeTrue("Search result must be sorted by " + sortoption + " in Descending");
                    break;
                default:
                    _providerClaimSearch.GetGridViewSection.GetGridListValueByCol(col).IsInAscendingOrder()
                        .ShouldBeTrue("Search result must be sorted by " + sortoption + " in Ascending");
                    _providerClaimSearch.ClickOnFilterOptionListRow(option);
                    _providerClaimSearch.GetGridViewSection.GetGridListValueByCol(col).IsInDescendingOrder()
                        .ShouldBeTrue("Search result must be sorted by " + sortoption + " in Descending");
                    break;
            }
        }


        private void Verify_correct_search_filter_options_displayed_for(string mappingQuickSearchOptionName, string quickSearchValue)
        {

            _providerClaimSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search", quickSearchValue);
            _providerClaimSearch.GetSideBarPanelSearch.GetSearchFiltersList()
                .ShouldCollectionBeEqual(
                    DataHelper.GetMappingData(FullyQualifiedClassName, mappingQuickSearchOptionName).Values.ToList(), "Search Filters",
                    true);

        }

        public void ValidateDatePickerField(string label, string message1, string message2, string message3)
        {
            _providerClaimSearch.GetSideBarPanelSearch.SetDateFieldTo(label, DateTime.Now.AddDays(1).ToString("MM/d/yyyy"));
            _providerClaimSearch.GetSideBarPanelSearch.ClickOnFindButton();
            _providerClaimSearch.IsPageErrorPopupModalPresent().ShouldBeTrue("Page error pop up should be present");
            _providerClaimSearch.GetPageErrorMessage().ShouldBeEqual(message1);
            _providerClaimSearch.ClosePageError();
            _providerClaimSearch.GetSideBarPanelSearch.ClickOnClearLinkCssSelector();


            _providerClaimSearch.GetSideBarPanelSearch.SetDateFieldFrom(label, DateTime.Now.ToString("MM/d/yyyy"));
            _providerClaimSearch.GetSideBarPanelSearch.GetDateFieldTo(label).
                ShouldBeEqual(_providerClaimSearch.GetSideBarPanelSearch.GetDateFieldFrom(label), label + " From value populated in To field as well.");

            _providerClaimSearch.GetSideBarPanelSearch.SetDateFieldTo(label, DateTime.Now.Subtract(new TimeSpan(24, 0, 0)).ToString("MM/d/yyyy"));
            _providerClaimSearch.GetPageErrorMessage().ShouldBeEqual(message2, "Please enter a valid date range.");
            _providerClaimSearch.ClosePageError();

            _providerClaimSearch.GetSideBarPanelSearch.SetDateFieldTo(label, DateTime.Now.AddMonths(6).AddDays(3).ToString("MM/d/yyyy"));
            _providerClaimSearch.GetSideBarPanelSearch.ClickOnFindButton();
            _providerClaimSearch.IsPageErrorPopupModalPresent().ShouldBeTrue("Page error pop up should be present");
            _providerClaimSearch.GetPageErrorMessage().ShouldBeEqual(message3);
            _providerClaimSearch.ClosePageError();
            _providerClaimSearch.GetSideBarPanelSearch.ClickOnClearLinkCssSelector();

        }

        #endregion


    }
}

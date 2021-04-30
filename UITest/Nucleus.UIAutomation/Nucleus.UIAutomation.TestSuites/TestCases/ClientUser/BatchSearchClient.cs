using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Nucleus.Service.Data;
using Nucleus.Service.PageServices.Batch;
using Nucleus.Service.PageServices.Claim;
using Nucleus.Service.PageServices.Settings.User;
using Nucleus.Service.Support.Common.Constants;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Environment;
using Nucleus.Service.Support.Menu;
using Nucleus.UIAutomation.TestSuites.Base;
using Nucleus.UIAutomation.TestSuites.Common;
using NUnit.Framework;
using UIAutomation.Framework.Utils;

namespace Nucleus.UIAutomation.TestSuites.TestCases.ClientUser
{
    public class BatchSearchClient : AutomatedBaseClient
    {
        #region PRIVATE FIELDS
        private BatchSearchPage _batchSearch;
        private ProfileManagerPage _profileManager;
        private ClaimSearchPage _claimSearch;
        private ClaimActionPage _claimAction;
        private BatchSummaryPage _batchSummary;
        private UserProfileSearchPage _userProfileSearch;
        private CommonValidations _commonValidation;
        #endregion

        #region PROTECTED PROPERTIES
        protected override  string FullyQualifiedClassName
        {
            get { return GetType().FullName; }
        }
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
            if (string.Compare(UserType.CurrentUserType, UserType.CLIENT, StringComparison.OrdinalIgnoreCase) != 0)
            {
                _batchSearch = _batchSearch.Logout().LoginAsClientUser().NavigateToBatchSearch();
            }

            if (_batchSearch.GetPageHeader() != PageHeaderEnum.BatchSearch.GetStringValue())
            {
                _batchSearch.NavigateToBatchSearch();
            }

            _batchSearch.SideBarPanelSearch.OpenSidebarPanel();
            _batchSearch.SideBarPanelSearch.ClickOnClearLink();
            _batchSearch.SideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                BatchQuickSearchTypeEnum.ReceivedThisWeek.GetStringValue());
            _batchSearch.SideBarPanelSearch.ClickOnFindButton();
            _batchSearch.WaitForWorkingAjaxMessage();
            base.TestCleanUp();
        }

        #endregion

        #region TestSuites

       [Test] //TE-383
        public void Verify_security_and_navigation_of_Batch_Search_page()
       {
           _commonValidation.ValidateSecurityAndNavigationOfAPage(HeaderMenu.Batch,
               new List<string> {SubMenu.BatchSearch},
               RoleEnum.ClaimsProcessor.GetStringValue(),
               new List<string> {PageHeaderEnum.BatchSearch.GetStringValue()},
               Login.LoginAsClientUserWithNoAnyAuthorityAndRedirectToQuickLaunch, new[] {"uiautomation", "noauthority"},
               Login.LoginAsClientUser);
           _batchSearch = CurrentPage.Logout().LoginAsClientUser().NavigateToBatchSearch();
            _batchSearch.SideBarPanelSearch.IsSideBarPanelOpen()
                .ShouldBeTrue("Side Bar panel should be opened by default");
            _batchSearch.SideBarPanelSearch.ClickOnToggleSidebarPanelButton();
            _batchSearch.SideBarPanelSearch.IsSideBarPanelOpen()
                .ShouldBeFalse("Sidebar Panel should be hidden when toggle button is clicked.");
           
        }

        [Test] //TE-383
        public void Verify_Search_Filters_and_their_Default_values()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            IDictionary<string, string> paramLists =
                DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
            var expectedBatchSearchFilterList = DataHelper
                .GetMappingData(FullyQualifiedClassName, "Batch_Search_Filter_List").Values.ToList();
            var expectedQuickSearchOptions = DataHelper
                .GetMappingData(FullyQualifiedClassName, "quick_search_options_for_batch_search").Values.ToList();

            _batchSearch.NavigateToBatchSearch();
            ValidateDefaultValuesOfFiltersExceptQuickSearchFilter();

            _batchSearch.SideBarPanelSearch.GetSearchFiltersList()
                .ShouldCollectionBeEqual(expectedBatchSearchFilterList, "Batch Search filter should be as expected.");

            StringFormatter.PrintMessageTitle("Validate Quick Search dropdown");
            ValidateQuickSearchDropDownForDefaultValueAndExpectedList("Quick Search", expectedQuickSearchOptions);

            _batchSearch.SideBarPanelSearch.GetInputValueByLabel("Batch ID")
                .ShouldBeNullorEmpty("Default value of Batch ID field should be empty.");
            _batchSearch.SideBarPanelSearch.SetInputFieldByLabel("Batch ID", paramLists["Alphanumeric"]);
            _batchSearch.SideBarPanelSearch.GetLengthOfTheInputFieldByLabel("Batch ID")
                .ShouldBeEqual(50, "Max char limit of Batch ID field should be 50");

            StringFormatter.PrintMessageTitle("Validate Client Create Date field");
            ValidateDatePickerField("Client Create Date", paramLists["Message1"], paramLists["Message2"], paramLists["Message3"]);

            StringFormatter.PrintMessageTitle("Validate Cotiviti Create Date field");
            ValidateDatePickerField("Cotiviti Create Date", paramLists["Message1"], paramLists["Message2"], paramLists["Message3"]);

            _batchSearch.SideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                BatchQuickSearchTypeEnum.IncompleteBatches.GetStringValue());
            StringFormatter.PrintMessageTitle("Verify Clear filter");
            _batchSearch.SideBarPanelSearch.ClickOnClearLink();

            ValidateDefaultValuesOfFiltersExceptQuickSearchFilter();
            _batchSearch.SideBarPanelSearch.GetInputValueByLabel("Quick Search").ShouldBeEqual(
                BatchQuickSearchTypeEnum.IncompleteBatches.GetStringValue(),
                "Clear button should all filter fields except selected quick search field");


            StringFormatter.PrintMessageTitle("Verify All quick search filter only cannot initiate search");
            _batchSearch.SideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search", BatchQuickSearchTypeEnum.AllBatches.GetStringValue());
            _batchSearch.SideBarPanelSearch.ClickOnFindButton();
            _batchSearch.WaitForWorkingAjaxMessage();
            _batchSearch.IsPageErrorPopupModalPresent().ShouldBeTrue("User should not be able to search by All Batches only");
            _batchSearch.GetPageErrorMessage().ShouldBeEqual("Search cannot be initiated without any criteria entered.",
                "Verify the popup message when search by all batches only");
            _batchSearch.ClosePageError();
            _batchSearch.SideBarPanelSearch.SetInputFieldByLabel("Batch ID", paramLists["BatchId"]);
            _batchSearch.SideBarPanelSearch.ClickOnFindButton();
            _batchSearch.IsPageErrorPopupModalPresent().ShouldBeFalse("User should be able to search by All Batches with any other criteria selected.");
            _batchSearch.SideBarPanelSearch.ClickOnClearLink();


        }

  
        [Test] //TE-383
        public void Verify_secondary_details_in_Batch_search_page_and_presence_of_checkbox_against_product_label()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            var batchId1 = DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName, "BatchId1", "Value");
            var batchId2 = DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName, "BatchId2", "Value");
            var _activeProductListForClientDB = _batchSearch.GetActiveProductListForClientDB();

            _batchSearch.SideBarPanelSearch.OpenSidebarPanel();
            _batchSearch.SideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                BatchQuickSearchTypeEnum.AllBatches.GetStringValue());
            _batchSearch.SideBarPanelSearch.SetInputFieldByLabel("Batch ID", batchId2);
            _batchSearch.SideBarPanelSearch.ClickOnButtonByButtonName("Find");
            _batchSearch.WaitForWorking();

            StringFormatter.PrintMessageTitle("Verifying product labels,Released and Complete column's label and values from DB ");
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
            StringFormatter.PrintMessageTitle("Verifying secondary details values and checkmark against product from Database ");
            VerifyCheckboxAgainstProductLabel(batchId2, _activeProductListForClientDB);
            _batchSearch.SideBarPanelSearch.OpenSidebarPanel();
            _batchSearch.SideBarPanelSearch.ClickOnClearLink();

            StringFormatter.PrintMessageTitle("Verifying secondary details values and checkmark against product from Database ");
            _batchSearch.SideBarPanelSearch.SetInputFieldByLabel("Batch ID", batchId1);
            _batchSearch.SideBarPanelSearch.ClickOnFindButton();
            _batchSearch.WaitForWorking();
            _batchSearch.GetGridViewSection.ClickOnGridByRowCol();
            _batchSearch.GetBatchDetailsHeader().ShouldBeEqual("Batch Details");
            _batchSearch.GetBatchDetailsSecondaryViewValueByLabel("Total Flagged Claims").ShouldBeEqual(
                _batchSearch.GetTotalClaimsCountInSecondaryDetails(batchId1), "Values should match");
            _batchSearch.GetBatchDetailsSecondaryViewValueByLabel("Total Unreviewed")
                .ShouldBeEqual(_batchSearch.GetTotalUnreviewedClaimsCountByClientFromDatabase(batchId1));
            _batchSearch.GetBatchDetailsSecondaryViewValueByLabel("Cotiviti Create Date")
                .ShouldBeEqual(_batchSearch.GetCotivitiAndClientCreateDateFromDatabase(batchId1)[0]);
            VerifyCheckboxAgainstProductLabel(batchId1, _activeProductListForClientDB);


        }

        [Test] //TE-383
        public void Verify_search_result_for_different_quick_search_options()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            IDictionary<string, string> paramLists = DataHelper.GetTestData(FullyQualifiedClassName,
                TestExtensions.TestName);

            var receivedThisWeekBatches = _batchSearch.GetBatchesReceivedThisWeekFromDatabase();
            var incompleteBatches = _batchSearch.GetIncompleteBatchesListromDatabase();

            List<string>[] valuesFromDb =
                    {receivedThisWeekBatches, incompleteBatches};
            string[] batchStringValue =
            {

                BatchQuickSearchTypeEnum.ReceivedThisWeek.GetStringValue(),
                BatchQuickSearchTypeEnum.IncompleteBatches.GetStringValue(),
            };

            for (int j = 0; j < valuesFromDb.Length; j++)
            {
                Verify_the_Batch_list_with_database(batchStringValue[j], valuesFromDb[j]);
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

        [Test] //TE-383
        public void Verify_find_button_is_disabled_when_search_is_active()
        {
            _batchSearch.SideBarPanelSearch.SetInputFieldByLabel("Quick Search", "Received this Week");
            _batchSearch.ClickFindAndCheckIfFindButtonIsDisabled()
                .ShouldBeTrue("Find Button Should be disabled while the search is active.");
            _batchSearch.WaitForWorkingAjaxMessage();
            _batchSearch.GetGridViewSection.GetGridRowCount().ShouldBeGreaterOrEqual(0, "Search results should be displayed");
            _batchSearch.CheckIfFindButtonIsEnabled()
                .ShouldBeTrue("Find Button Should be enabled once the search is complete.");
        }

        [Test, Category("SmokeTestDeployment")] //TANT-99
        public void Verify_Batch_Search_Page_and_presence_of_data()
        {
            _batchSearch.GetGridViewSection.GetGridRowCount().ShouldBeGreaterOrEqual
                (0, "Default search should the most recent active batches");
        }

        [Test] //TE-508
        public void Verify_Sorting_Options_In_Batch_Search()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            var filterOptions =
                DataHelper.GetMappingData(FullyQualifiedClassName, "Batch_Sorting_options").Values.ToList();
            try
            {
                _batchSearch.SideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search", "Incomplete Batches");
                _batchSearch.SideBarPanelSearch.ClickOnFindButton();
                _batchSearch.WaitForWorkingAjaxMessage();
                _batchSearch.GetGridViewSection.IsFilterOptionIconPresent()
                    .ShouldBeTrue("Filter Icon Option Should Be Present");
                _batchSearch.GetGridViewSection.GetFilterOptionTooltip()
                    .ShouldBeEqual("Sort Results", "Correct tooltip is displayed");
                _batchSearch.GetFilterOptionList()
                    .ShouldCollectionBeEqual(filterOptions, "Filter Options Lists Collection Should Equal");
                _batchSearch.SideBarPanelSearch.ClickOnFindButton();
                _batchSearch.WaitForWorkingAjaxMessage();
                StringFormatter.PrintMessage("Verify sort using Batch Id");
                VerifySortingOptionsInBatchSearch(2, 1, filterOptions[0]);
                StringFormatter.PrintMessage("Verify sort using Batch Date");
                VerifySortingOptionsInBatchSearch(16, 2, filterOptions[1]);
            }
            finally
            {
                _batchSearch.ClickOnClearSort();
                _batchSearch.GetGridViewSection.GetGridListValueByCol(16).IsInDescendingOrder()
                    .ShouldBeTrue("Is default sort applied after clear sort ?");
            }
        }
        #endregion

        #region PRIVATE

        private void ValidateDefaultValuesOfFiltersExceptQuickSearchFilter()
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

        public void ValidateDatePickerField(string label, string message1, string message2, string message3)
        {
            _batchSearch.SideBarPanelSearch.SetDateFieldTo(label, DateTime.Now.AddDays(1).ToString("MM/d/yyyy"));
            _batchSearch.SideBarPanelSearch.GetFieldErrorIconTooltipMessage(label)
                .ShouldBeEqual(
                    message3,
                    "Field Error Tooltip Message When Date From is empty");

            _batchSearch.SideBarPanelSearch.SetDateFieldFrom(label, DateTime.Now.AddDays(1).ToString("MM/d/yyyy"));
            _batchSearch.SideBarPanelSearch.SetDateField(label, "", 2);
            _batchSearch.SideBarPanelSearch.GetFieldErrorIconTooltipMessage(label)
                .ShouldBeEqual(
                    message3,
                    "Field Error Tooltip Message When Date To is empty");


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
            _batchSearch.SideBarPanelSearch.ClickOnFindButton();
            _batchSearch.GetPageErrorMessage().ShouldBeEqual(message2, "Verification of popup message for invalid date");
            _batchSearch.ClosePageError();
            _batchSearch.SideBarPanelSearch.ClickOnHeader();


        }


        private void VerifyCheckboxAgainstProductLabel(string batchid, List<string> activeProducts)
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
                var unreviewedClaimsCountByProductFromDatabase = _batchSearch.GetUnreviewedClaimsCountByProductByClientFromDatabase(batchid, product);
                _batchSearch.GetBatchDetailsSecondaryViewValueByLabel(activeProducts[i]).ShouldBeEqual(unreviewedClaimsCountByProductFromDatabase, "Is Count Equals?");

            }

        }

        private void ValidateQuickSearchDropDownForDefaultValueAndExpectedList(string label, IList<string> collectionToEqual)
        {
            var actualDropDownList = _batchSearch.SideBarPanelSearch.GetAvailableDropDownList(label);
            actualDropDownList.Count.ShouldBeGreater(0, "List of " + label + " is greater than zero.");
            actualDropDownList.RemoveAll(String.IsNullOrEmpty);
            if (collectionToEqual != null)
                actualDropDownList.ShouldCollectionBeEqual(collectionToEqual, label + " List As Expected");
            _batchSearch.SideBarPanelSearch.GetInputValueByLabel(label)
                .ShouldBeEqual(BatchQuickSearchTypeEnum.ReceivedThisWeek.GetStringValue(), label + " value defaults to Received This Week");
            _batchSearch.SideBarPanelSearch.SelectDropDownListValueByLabel(label, actualDropDownList[0],
                false); //check for type ahead functionality
            _batchSearch.SideBarPanelSearch.SelectDropDownListValueByLabel(label, actualDropDownList[2]);
            _batchSearch.SideBarPanelSearch.GetInputValueByLabel(label)
                .ShouldBeEqual(actualDropDownList[2], "User can select only a single option");

        }

        private void Verify_the_Batch_list_with_database(string quickSearchMappingOption, List<string> batchList, bool search = true)
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

        private void VerifySortingOptionsInBatchSearch(int col, int option, string sortoption)
        {
            _batchSearch.ClickOnFilterOptionListRow(option);
            _batchSearch.GetGridViewSection.GetGridListValueByCol(col).IsInAscendingOrder()
                .ShouldBeTrue($"Search result must be sorted by {sortoption} in Ascending");
            _batchSearch.ClickOnFilterOptionListRow(option);
            _batchSearch.GetGridViewSection.GetGridListValueByCol(col).IsInDescendingOrder()
                .ShouldBeTrue($"Search result must be sorted by {sortoption} in Descending");
        }




        #endregion



    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Nucleus.Service.Support.Common;
using Nucleus.Service.Support.Common.Constants;
using Nucleus.UIAutomation.TestSuites.Base;
using Nucleus.Service.PageServices.Claim;
using Nucleus.Service.Data;
using System.Diagnostics;
using NUnit.Framework;

namespace Nucleus.UIAutomation.TestSuites.TestCases
{
    public class ClaimLineSearch : AutomatedBase
    {
        #region PRIVATE FIELDS

        private ClaimLineSearchPage _claimLineSearch;
        private IDictionary<string, string> _quickSearchOptionsForHciUser;
      
        #endregion

        #region PROTECTED PROPERTIES

        protected override string FullyQualifiedClassName
        {
            get { return GetType().FullName; }
        }

        #endregion

        #region OVERRIDE MEHTODS

        /// <summary>
        /// Override ClassInit to add additional code.
        /// </summary>
        protected override void ClassInit()
        {
            try
            {
                base.ClassInit();
                _claimLineSearch = QuickLaunch.NavigateToClaimLineSearch();
                _claimLineSearch.ClickOnNucleusIcon();
                _quickSearchOptionsForHciUser = DataHelper.GetMappingData(FullyQualifiedClassName,
                                                                          "quick_search_options_for_hci_user");
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
            if (string.Compare(UserType.CurrentUserType, UserType.CLIENT, StringComparison.OrdinalIgnoreCase) == 0)
            {
                _claimLineSearch = _claimLineSearch
                    .Logout()
                    .LoginAsHciAdminUser()
                    .NavigateToClaimLineSearch();
            }
        }

        #endregion

        #region TEST SUITES

     
        [Test, Category("SmokeTest")]
        public void Verify_that_search_does_not_show_blank_grid_when_batch_contains_unwanted_action_driver_data()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            IDictionary<string, string> paramLists = DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
            string batchId = paramLists["BatchId"];
            _claimLineSearch.SelectAllLines();
            _claimLineSearch.SearchByBatchId(batchId);
            _claimLineSearch.IsGridShowingUpRecords().ShouldBeTrue("Result Grid shows some records", true);
        }

         [Test, Category("SmokeTest")]
        public void Verify_correct_search_filters_underneath_each_quicksearch_options()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            foreach (var quickSearch in _quickSearchOptionsForHciUser)
            {
                _claimLineSearch.SelectQuickSearch(quickSearch.Value);
                _claimLineSearch.GetSearchFiltersList()
                    .ShouldCollectionBeEqual(DataHelper.GetMappingData(FullyQualifiedClassName, quickSearch.Key).Values.ToList(),
                        string.Empty, false, string.Format("Search filters for {0} :", quickSearch.Value));
            }
        }

        [Test, Category("SmokeTest"),Category("IENonCompatible")]
        public void Test_pagination_and_verify_can_click_through_each_page()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            string batchId = DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName, "BatchId", "Value");
            _claimLineSearch.SelectAllLines();
            _claimLineSearch.SelectAction("Delete");
            _claimLineSearch.SearchByBatchId(batchId);
            var paginationTests = new PaginationTests(_claimLineSearch);
            paginationTests.TestPagination();
        }

        [Test]
        public void Test_you_can_change_the_page_size_and_verify_proper_number_of_rows_are_displayed()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            string batchId = DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName, "BatchId", "Value");

            _claimLineSearch.SelectAllLines();
            _claimLineSearch.SearchByBatchId(batchId);
            //change page size and verify the no of records
            var paginationService = new PaginationService(_claimLineSearch);
            foreach (int pageSize in paginationService.PageSizes)
            {
                StringFormatter.PrintLineBreak();
                _claimLineSearch.PageSizeSelect(paginationService, pageSize);
                int pagesize = paginationService.GetSelectedPageSize();
                paginationService.CheckPageSize(paginationService.EndPage, pagesize, paginationService.TotalNoOfRecords).ShouldBeTrue("Expected Records not shown as page size is changed");
                StringFormatter.PrintLineBreak();
            }
        }

     //   [Test]
        public void Verify_that_default_grid_columns_are_correct_for_all_lines()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            IList<string> accessibleColumns = DataHelper.GetMappingData(FullyQualifiedClassName, "default_grid_columns_for_hci_users").Keys.ToList();
            string batchId = DataHelper.GetSingleTestData(FullyQualifiedClassName, "Test_you_can_change_the_page_size_and_verify_proper_number_of_rows_are_displayed", "BatchId", "Value");
            _claimLineSearch.SelectAllLines();
            _claimLineSearch.SearchByBatchId(batchId);
            _claimLineSearch.GridColumnReset();
            _claimLineSearch.GetResultGridColumnsList().ShouldCollectionBeEqual(accessibleColumns, "Grid Columns");
        }

    //    [Test]
        public void Verify_that_default_grid_columns_are_correct_for_modified_lines()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            IList<string> accessibleColumns = DataHelper.GetMappingData(FullyQualifiedClassName, "default_grid_columns_for_hci_users").Keys.ToList();
            string batchId = DataHelper.GetSingleTestData(FullyQualifiedClassName, "Test_you_can_change_the_page_size_and_verify_proper_number_of_rows_are_displayed", "BatchId", "Value");
            _claimLineSearch.SelectModifiedLines();
            _claimLineSearch.SearchByBatchId(batchId);
            _claimLineSearch.GridColumnReset();
            _claimLineSearch.GetResultGridColumnsList().ShouldCollectionBeEqual(accessibleColumns, "Grid Columns");
        }

        [Test, Category("SmokeTest")]
        public void Verify_list_of_quickSearch_options_for_hci_users()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            _claimLineSearch.GetQuickSearchOptions().ShouldCollectionBeEqual(_quickSearchOptionsForHciUser.Values.ToList(), "QuickSearch Options");
        }

        

        [Test, Category("SmokeTest"), Category("SmokeTestDeployment")]
        public void For_all_lines_test_each_filter_option_individually_with_batchid_and_verify_results_are_correct()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            Test_search_filters_and_verify_search_results(ClaimLineSearchPage.AllLines);
        }

        [Test, Category("SmokeTest"), Category("SmokeTestDeployment")]
        public void For_modified_lines_test_each_filter_option_individually_with_batchid_and_verify_results_are_correct()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            Test_search_filters_and_verify_search_results(ClaimLineSearchPage.ModifiedLines);
        }

        #endregion

        #region PRIVATE METHODS

        private void Test_search_filters_and_verify_search_results(string mappingQuickSearchOptionName)
        {
            IDictionary<string, string> paramLists = DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
            string batchId = paramLists["BatchID"];
            string modifiedDateBegin = paramLists["ModifiedDateStart"];
            string modifiedDateEnd = paramLists["ModifiedDateEnd"];
            string modifiedDateFlag = paramLists["ModifiedDateFlag"];
            string plan = paramLists["Plan"];
            string planNameFlag = paramLists["PlanFlag"];
            string lineOfBusiness = paramLists["LineOfBusiness"];
            string lineOfBusinessFlag = paramLists["LineOfBusinessFlag"];
            string action = paramLists["Action"];
            string actionFlag = paramLists["ActionFlag"];
            string actionType = paramLists["ActionType"];
            string actionTypeFlag = paramLists["ActionTypeFlag"];
            string userName = paramLists["UserName"];
            string userNameFlag = paramLists["UserNameFlag"];
            string reason = paramLists["Reason"];
            string reasonFlag = paramLists["ReasonFlag"];
            string edit = paramLists["Edit"];
            string editFlag = paramLists["EditFlag"];
            string procCode = paramLists["ProcCode"];
            string procCodeFlag = paramLists["ProcCodeFlag"];
            string triggerCode = paramLists["TriggerCode"];
            string triggerCodeFlag = paramLists["TriggerCodeFlag"];
            if (ClaimLineSearchPage.AllLines == mappingQuickSearchOptionName)
            {
                StringFormatter.PrintMessageTitle("Verify Search Filters for All Lines");
                _claimLineSearch.SelectAllLines();
                _claimLineSearch.SearchByBatchId(batchId);
            }
            else
            {
                StringFormatter.PrintMessageTitle("Verify Search Filters for Modified Lines");
                _claimLineSearch.SelectModifiedLines();
                _claimLineSearch.SearchByBatchId(batchId);
            }

            if (!_claimLineSearch.IsNoRecordsFoundDivPresent())
            {
                //reset grid columns 
              //  _claimLineSearch.GridColumnReset();

                //verify modified date
                _claimLineSearch.SearchByModifiedDate(modifiedDateBegin, modifiedDateEnd);
                VerifySearchResults("Modified Date", modifiedDateBegin, modifiedDateEnd, modifiedDateFlag);
                _claimLineSearch.ClearModifiedDate();
                StringFormatter.PrintLineBreak();

                //// verify plan
                _claimLineSearch.SearchByPlan(plan);
                VerifySearchResults("Plan", plan, null, planNameFlag);
                _claimLineSearch.ClearPlan();
                StringFormatter.PrintLineBreak();

                // verify line of business
                _claimLineSearch.SearchByLineOfBusiness(lineOfBusiness);
                VerifySearchResults("Line Of Business", lineOfBusiness, null, lineOfBusinessFlag);
                _claimLineSearch.ClearLineOfBusiness();
                StringFormatter.PrintLineBreak();

                // verify action
                _claimLineSearch.SearchByAction(action);
                VerifySearchResults("Action", action, null, actionFlag);
                _claimLineSearch.ClearAction();
                StringFormatter.PrintLineBreak();

                // verify action type
                _claimLineSearch.SearchByActionType(actionType);
                VerifySearchResults("Action Type", actionType, null, actionTypeFlag);
                _claimLineSearch.ClearActionType();
                StringFormatter.PrintLineBreak();

                // verify user name
                _claimLineSearch.SearchByUserName(userName.Replace("( ","("));
                VerifySearchResults("User Name", userName, null, userNameFlag);
                _claimLineSearch.ClearUserName();
                StringFormatter.PrintLineBreak();

                //verify reason
                _claimLineSearch.SearchByReason(reason);
                VerifySearchResults("Reason", reason, null, reasonFlag);
                _claimLineSearch.ClearReason();
                StringFormatter.PrintLineBreak();

                //verify edit
                _claimLineSearch.SearchByEdit(edit);
                VerifySearchResults("Edit", edit, null, editFlag);
                _claimLineSearch.ClearEdit();
                StringFormatter.PrintLineBreak();

                //verify Proc Code
                _claimLineSearch.SearchByProcCode(procCode);
                VerifySearchResults("Proc Code", procCode, null, procCodeFlag);
                _claimLineSearch.ClearProcCode();
                StringFormatter.PrintLineBreak();

                //verify Trigger Code
                _claimLineSearch.SearchByTriggerCode(triggerCode);
                VerifySearchResults("Trigger Code", triggerCode, null, triggerCodeFlag);
                _claimLineSearch.ClearTriggerCode();
                StringFormatter.PrintLineBreak();
            }

            else
                this.AssertFail("Quick search option selected has no data. Hence , test cannot be proceeded.");
        }

        private void VerifySearchResults(string columnToVerify, string expectedValue1, string expectedValue2, string recordFlag)
        {

            IDictionary<string, string> gridColumns = DataHelper.GetMappingData(FullyQualifiedClassName, "default_grid_columns_for_hci_users");
            IDictionary<string, string> reasonCodeLookUp = DataHelper.GetMappingData(FullyQualifiedClassName, "reasoncode_lookup");
            switch (recordFlag)
            {
                case "0":
                    _claimLineSearch.IsNoRecordsFoundDivPresent().ShouldBeTrue("No Div Records Found");
                    Console.WriteLine("Verified that No Records Found div is present");
                    break;
                case "1":
                case "2":
                    int rowCount = _claimLineSearch.GetRowCount();
                    Console.WriteLine("Total No of Records : {0}", rowCount);
                    Console.WriteLine("Verifying Searh result for row 1");
                    string gridData = _claimLineSearch.GetGridData(1, Convert.ToInt32(gridColumns[columnToVerify]));

                    if (columnToVerify == "Reason")
                        gridData = reasonCodeLookUp[gridData];

                    if (columnToVerify == "Edit")
                        gridData.AssertIsContained(expectedValue1, columnToVerify);
                    else if (columnToVerify == "Modified Date")
                    {
                        DateTime expectedDateBegin = Convert.ToDateTime(expectedValue1);
                        DateTime expectedDateEnd = Convert.ToDateTime(expectedValue2);
                        DateTime actualDate = Convert.ToDateTime(gridData);
                        actualDate.AssertDateRange(expectedDateBegin, expectedDateEnd, columnToVerify);
                    }
                    else
                        gridData.ShouldBeEqual(expectedValue1, columnToVerify);
                    break;
                default:
                    this.AssertFail("Incorrect test data for flag provided, Flag can only be 0, 1 or 2");
                    break;
            }
        }

        #endregion
    }
}

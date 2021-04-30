using Nucleus.Service.Data;
using Nucleus.Service.PageServices.Appeal;
using Nucleus.Service.PageServices.QA;
using Nucleus.Service.PageServices.Settings.User;
using Nucleus.Service.Support.Common.Constants;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Environment;
using Nucleus.UIAutomation.TestSuites.Base;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Nucleus.Service.SqlScriptObjects.Common_SQL;
using NUnit.Framework.Constraints;
using NUnit.Framework.Internal;
using UIAutomation.Framework.Utils;


namespace Nucleus.UIAutomation.TestSuites.TestCases
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class QaAppealSearch
    {

        /* #region PRIVATE FIELDS

         private UserProfileSearchPage _newUserProfileSearch;
         private ProfileManagerPage _profileManager;
         private QaAppealSearchPage _qaAppealSearch;
         private AppealSummaryPage _appealSummary;
         private QaManagerPage _qaManager;
         private List<string> _analystUserList;
         private List<string> _assignedClientList;
         private List<string> _activeCategoryist;


         #endregion

         #region OVERRIDE METHODS

         protected override void ClassInit()
         {
             try
             {
                 base.ClassInit();
                 _qaAppealSearch = QuickLaunch.NavigateToQaAppealSearch();      

             }
             catch (Exception e)
             {
                 if(StartFlow !=null)
                     StartFlow.Dispose();
                 throw;
             }
         }

         protected override void TestCleanUp()
         {
             base.TestCleanUp();
             if (string.Compare(UserType.CurrentUserType, UserType.HCIADMIN, StringComparison.OrdinalIgnoreCase) != 0)
             {
                 _qaAppealSearch = _qaAppealSearch.Logout().LoginAsHciAdminUser().NavigateToQaAppealSearch();
             }
             else if (CurrentPage.GetPageHeader() != PageHeaderEnum.QaAppealSearch.GetStringValue())
             {
                 CurrentPage.ClickOnQuickLaunch().NavigateToQaAppealSearch();
             }
             else
             {
                 _qaAppealSearch.GetSideBarPanelSearch.OpenSidebarPanel();
                 _qaAppealSearch.ClickOnClearButton();
                 _qaAppealSearch.SelectOutstandingQaAppeals();
                 _qaAppealSearch.ClickOnFindButton();
             }
         }



         #endregion

       */

        #region PROTECTED PROPERTIES
        protected string FullyQualifiedClassName
        {
            get { return GetType().FullName; }
        }

        #endregion

        #region TEST SUITES

        [Test] //CAR-3050 (CAR-3123)
        public void Verify_export_functionality_in_qa_appeal_search()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                string downloadedExcelWithDefaultFilter = string.Empty;
                string downloadedExcelWithFilteredData = string.Empty;

                try
                {
                    QaAppealSearchPage _qaAppealSearch;
                    automatedBase.CurrentPage = _qaAppealSearch = automatedBase.QuickLaunch.NavigateToQaAppealSearch();

                    var TestName = new StackFrame(true).GetMethod().Name;
                    var paramList = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestName);
                    var headersList = paramList["Headers"].Split(';').ToList();
                    var userId = paramList["userId"];
                    var categoryToFilterWith = paramList["category"];

                    StringFormatter.PrintMessageTitle(
                        "Exporting the Outstanding QA Appeals which is the default view for this QA Appeal Search page");
                    _qaAppealSearch.ClickOnExportIcon();

                    var outstandingQaAppealResultFromDB =
                        _qaAppealSearch.GetOutstandingQaAppealsGridResultFromDB("where qra.reviewed = 'F'");
                    downloadedExcelWithDefaultFilter = _qaAppealSearch.GetFileName();

                    StringFormatter.PrintMessage("Verifying exported filename");
                    bool isFileNameFormatCorrect = $"QA Appeals{DateTime.Now.ToString("_MM_dd_yyyy")}" ==
                                                   downloadedExcelWithDefaultFilter.Split('.')[0].Trim();

                    isFileNameFormatCorrect.ShouldBeTrue($"Exported filename {downloadedExcelWithDefaultFilter} should be in correct format");

                    int initialAuditSeq = _qaAppealSearch.GetAuditSeqForExportFromDB(userId, DateTime.Now.ToString("MM/dd/yyyy"));

                    StringFormatter.PrintMessage("Verifying the excel headers and data against the DB");
                    VerifyExcelData(outstandingQaAppealResultFromDB, downloadedExcelWithDefaultFilter);

                    StringFormatter.PrintMessageTitle("Exporting the Outstanding QA Appeals after applying search");
                    _qaAppealSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Category",
                        categoryToFilterWith);
                    _qaAppealSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                        QaAppealQuickSearchTypeEnum.AllQaAppeals.GetStringValue());
                    _qaAppealSearch.ClickOnFindButton();

                    _qaAppealSearch.ClickOnExportIcon();

                    var filteredQaAppealResultFromDB =
                        _qaAppealSearch.GetOutstandingQaAppealsGridResultFromDB("where a.category_code = 'E/M'");
                    downloadedExcelWithFilteredData = _qaAppealSearch.GetFileName();

                    VerifyExcelData(filteredQaAppealResultFromDB, downloadedExcelWithFilteredData);

                    StringFormatter.PrintMessageTitle("Verify audit record");
                    int auditSeqAfterSecondExport =
                        _qaAppealSearch.GetAuditSeqForExportFromDB(userId, DateTime.Now.ToString("MM/dd/yyyy"));

                    auditSeqAfterSecondExport.ShouldBeGreater(initialAuditSeq,
                        "Audit should be written after excel export");
                    
                    #region LOCAL METHOD
                    void VerifyExcelData(List<List<string>> qaAppealDataFromDB, string fileName)
                    {
                        ExcelReader.ReadExcelSheetValue(fileName, "QaAppealSearchResults", 3, 2,
                            out var headerListFromExcel,
                            out var excelExportList, out _, skipMergedCells: false);

                        // To handle excess blank rows stored in excelExportList
                        if (excelExportList.Count > qaAppealDataFromDB.Count)
                        {
                            excelExportList = excelExportList.Take(excelExportList.Count - 1).ToList();
                        }

                        headerListFromExcel.RemoveAt(headerListFromExcel.Count - 1);

                        excelExportList.Select(list =>
                        {
                            list.RemoveAt(list.Count - 1);
                            for (int count = 0; count < list.Count; count++)
                            {
                                list[count] = list[count].Trim();
                            }

                            return list;
                        }).ToList();

                        headerListFromExcel.ShouldCollectionBeEqual(headersList,
                            "The headers in the exported excel file should be correct");

                        StringFormatter.PrintMessage("Verify data values in the excel file");

                        bool isDataCorrect = true;

                        //Verifying the list items since lists cannot be compared directly
                        foreach (var dataRowInExcel in excelExportList)
                            isDataCorrect = isDataCorrect &&
                                            qaAppealDataFromDB.Any(c => c.SequenceEqual(dataRowInExcel));

                        isDataCorrect.ShouldBeTrue("Cell data in the exported Excel file should be correct");
                        excelExportList.Count.ShouldBeEqual(qaAppealDataFromDB.Count,
                            "The counts of the the data row in excel should be equal to the DB records");

                        ExcelReader.DeleteExcelFileIfAlreadyExists(fileName);
                    }
                    #endregion
                }

                finally
                {
                    StringFormatter.PrintMessageTitle("Finally Block");
                    ExcelReader.DeleteExcelFileIfAlreadyExists(downloadedExcelWithDefaultFilter);
                    ExcelReader.DeleteExcelFileIfAlreadyExists(downloadedExcelWithFilteredData);
                }
            }
        }

        [Test, Category("SmokeTestDeployment")] //TANT-106
        public void Verify_QA_Appeal_Search_Pannel_And_Navigation_TO_Appeal_Summary()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                QaAppealSearchPage _qaAppealSearch;
                AppealSummaryPage _appealSummary;
                automatedBase.CurrentPage =
                            _qaAppealSearch = automatedBase.QuickLaunch.NavigateToQaAppealSearch();
                var sortOption = automatedBase.DataHelper
                    .GetMappingData(FullyQualifiedClassName, "QAAppealSearch_Sort_Options").Values.ToList();

                StringFormatter.PrintMessage("verify  Open or Close the QA Appeal Search panel");
                _qaAppealSearch.GetSideBarPanelSearch.IsSideBarPanelOpen()
                    .ShouldBeTrue("QA Appeal Search Panel on sidebar is open by default when user lands on page.");
                _qaAppealSearch.GetSideBarPanelSearch.ClickOnToggleSidebarPanelButton();
                _qaAppealSearch.GetSideBarPanelSearch.IsSideBarPanelOpen()
                    .ShouldBeFalse("QA Appeal Search Panel on sidebar is hidden when toggle button is clicked.");

                StringFormatter.PrintMessage("Verify sort option list");
                _qaAppealSearch.IsSortIconPresent().ShouldBeTrue("Sort Icon should  be present");
                _qaAppealSearch.GetFilterOptionList()
                    .ShouldCollectionBeEqual(sortOption, "The sorting options should match");
                if (_qaAppealSearch.CurrentPageUrl.Contains("dev."))
                {
                    if(!_qaAppealSearch.GetSideBarPanelSearch.IsSideBarPanelOpen())
                        _qaAppealSearch.GetSideBarPanelSearch.ClickOnToggleSidebarPanelButton();
                    StringFormatter.PrintMessage("Verify navigation to Appeal Summary page ");
                    _qaAppealSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Client",ClientEnum.SMTST.ToString());
                    _qaAppealSearch.ClickOnFindButton();
                    _qaAppealSearch.WaitForWorking();
                    var appealSeq = _qaAppealSearch.GetGridViewSection.GetValueInGridByColRow(3);
                    _appealSummary = _qaAppealSearch.ClickOnAppealSequenceAndNavigateToAppealSummaryPage(appealSeq);
                    _appealSummary.GetAppealSequenceOnHeader()
                        .ShouldBeEqual(appealSeq, "Correct Appeal summary page opened?");
                    _appealSummary.GetWindowHandlesCount().ShouldBeEqual(1,
                        "Appeal Summary Page should be opened in the same page and not in a" +
                        "separate pop out page");
                    _qaAppealSearch = _appealSummary.ClickOnSearchIconToNavigateQAAppealSearchPage();
                    _qaAppealSearch.GetPageHeader()
                        .ShouldBeEqual(PageHeaderEnum.QaAppealSearch.GetStringValue(),
                            "Verify page header after landing to Qa Appeal search");


                    StringFormatter.PrintMessage("Verify Error displayed while deleting Appeal");
                    _qaAppealSearch.GetGridViewSection.IsDeleteIconPresentInRecordForRow(5);
                    Console.Write(_qaAppealSearch.GetGridViewSection.GetValueInGridByColRow(3));
                    _qaAppealSearch.GetGridViewSection.ClickOnDeleteIcon();
                    _qaAppealSearch.GetPageErrorMessage()
                        .ShouldBeEqual(
                            "The selected QA appeal will be permanently deleted. Click Ok to proceed or Cancel.");
                    _qaAppealSearch.ClickOkCancelOnConfirmationModal(false);
                }

            }
        }



        //[Test] //US69386
        //public void Verify_navigation_and_security()
        //{
        //    TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
        //    _profileManager = QuickLaunch.NavigateToProfileManager();
        //    _profileManager.ClickOnPrivileges();
        //    _profileManager.IsAuthorityAssigned("QA Manager").ShouldBeTrue("'QA Manager' authority is assigned");
        //    _profileManager.NavigateToQaAppealSearch();
        //    _qaAppealSearch.IsQaAppealSearchSubMenuPresent().ShouldBeTrue("QA Appeal Search Sub Menu should Present for authorized user");
        //    _qaAppealSearch.GetPageInsideTitle().ShouldBeEqual(PageHeaderEnum.QaAppealSearch.GetStringValue(),
        //        "Is Correct Page Title Displayed inside the page?");

        //    _qaAppealSearch.Logout().LoginAsUserHavingNoManageCategoryAuthority(); //uiautomation3
        //    _profileManager = QuickLaunch.NavigateToProfileManager();
        //    _profileManager.ClickOnPrivileges();
        //    _profileManager.IsAuthorityAssigned("QA Manager").ShouldBeTrue("'QA Manager' authority is assigned with read only");
        //    _qaAppealSearch.IsQaAppealSearchSubMenuPresent().ShouldBeFalse("QA Appeal Search Menu unavailable for Read Only Authority");

        //    _profileManager.ClickOnQuickLaunch();
        //    _qaAppealSearch.Logout().LoginAsUserHavingNoAnyAuthority();//uiautomation4
        //    _qaAppealSearch.IsQaAppealSearchSubMenuPresent().ShouldBeFalse("QA Appeal Search submenu absent for unauthorized user.");

        //}

        [Test] //US69388 + CAR-933(CAR-908) + CAR-3205(CAR-3236)
        public void Verify_search_grid_display()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                QaAppealSearchPage _qaAppealSearch;
                AppealSummaryPage _appealSummary;
                automatedBase.CurrentPage =
                    _qaAppealSearch = automatedBase.QuickLaunch.NavigateToQaAppealSearch();
                var TestName = new StackFrame().GetMethod().Name;

                #region CAR 908

                //_qaAppealSearch.SelectAllQaAppeals();
                //_qaAppealSearch.GetSideBarPanelSearch.SetDateFieldFrom("QA Review Date", "05/30/2019");
                //_qaAppealSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Client",
                //    ClientEnum.SMTST.ToString());
                //_qaAppealSearch.ClickOnFindButton();

                //_qaAppealSearch.GetGridViewSection.IsLabelPresentByCol(4)
                //    .ShouldBeFalse("Label should not be displayed for Appeal Category");
                //_qaAppealSearch.GetGridViewSection.IsLabelPresentByCol(8)
                //    .ShouldBeFalse("Label should not be displayed for QA Score");
                //var scoreBefore = _qaAppealSearch.GetGridViewSection.GetValueInGridByColRow(8);
                //var scoreAfter = "789";
                //scoreAfter = scoreBefore == scoreAfter ? "900" : scoreAfter;
                //_appealSummary =
                //    _qaAppealSearch.ClickOnAppealSequenceAndNavigateToAppealSummaryPage(
                //        _qaAppealSearch.GetGridViewSection.GetValueInGridByColRow(3));
                //var category = _appealSummary.GetAppealDetails(1, 4);

                //_appealSummary.ClickMoreOption();
                //var appealProcessingHx = _appealSummary.ClickAppealProcessingHx();
                //var initialQaDoneUser = appealProcessingHx.GetInitialQADoneUser();
                //appealProcessingHx.CloseAppealProcessingHistoryAndBackToAppealSummary();
                //_appealSummary.ClickOnCompleteQAIcon(true);
                //scoreBefore.ShouldBeEqual(_appealSummary.GetEditAppealInputValueByLabel("QA Score"));
                //_appealSummary.SetEditAppealInputValueByLabel("QA Score", scoreAfter);
                //_appealSummary.ClickOnSaveButtonOnEditAppeal();

                //_qaAppealSearch.GetGridViewSection.GetValueInGridByColRow(4)
                //    .ShouldBeEqual(category, "Correct Appeal Category must be displayed");

                //_qaAppealSearch.GetGridViewSection.GetValueInGridByColRow(7).ShouldBeEqual(
                //    initialQaDoneUser.Substring(0, initialQaDoneUser.LastIndexOf('(') - 1),
                //    "QA Reviewer Name must be name of person that completed the initial appeal QA review");

                //_qaAppealSearch.GetGridViewSection.GetValueInGridByColRow(8)
                //    .ShouldBeEqual(scoreAfter, "QA Score must be most updated QA score");

                //_qaAppealSearch.GetGridViewSection.GetLabelInGridByColRow(7)
                //    .ShouldBeEqual("Reviewer:",
                //        "QA Reviewer Name section should show the label 'QA Reviewer Name:'");

                //_qaAppealSearch.GetGridViewSection.GetValueInGridByColRow(7).DoesNameContainsOnlyFirstWithLastname()
                //    .ShouldBeTrue("QA Reviewer Name should be present in <FirstName> <LastName> format");

                #endregion

                _qaAppealSearch.ClickOnClearButton();
                _qaAppealSearch.SelectAllQaAppeals();
                //_qaAppealSearch.SelectOutstandingQaAppeals();
                //_qaAppealSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Client", ClientEnum.SMTST.ToString());
                _qaAppealSearch.GetSideBarPanelSearch.SetDateFieldFrom("QA Review Date", "10/1/2017");
                _qaAppealSearch.GetSideBarPanelSearch.SetDateFieldTo("QA Review Date", "12/31/2017");
                _qaAppealSearch.ClickOnFindButton();

                _qaAppealSearch.GetGridViewSection.GetGridListValueByCol(5)
                    .IsInAscendingOrder()
                    .ShouldBeTrue("Default order of appeals should be in ascending order of Complete Date");

                _qaAppealSearch.GetGridViewSection.IsLabelPresentByCol(2)
                    .ShouldBeFalse("Label should not be displayed for Client Code");

                _qaAppealSearch.GetGridViewSection.GetValueInGridByColRow(2)
                    .Length.ShouldBeGreater(0,
                        "Client code should be present and character length should be greater than zero");

                _qaAppealSearch.GetGridViewSection.IsLabelPresentByCol(3)
                    .ShouldBeFalse("Label should not be displayed for Appeal Sequence");

                _qaAppealSearch.GetGridViewSection.GetValueInGridByColRow(3).Length
                    .ShouldBeGreater(0,
                        "Appeal Sequence should be present and character length should be greater than zero");

                _qaAppealSearch.GetGridViewSection.IsClickableLink(3)
                    .ShouldBeTrue("Appeal Sequence shoule be a clickable link");


                _qaAppealSearch.GetGridViewSection.GetLabelInGridByColRow(5)
                    .ShouldBeEqual("Completed:",
                        "Completed Date section should show the label 'Completed Date:'");

                _qaAppealSearch.GetGridViewSection.GetValueInGridByColRow(5).IsDateInFormat()
                    .ShouldBeTrue("Is Completed Date in correct format");

                #region CAR-3205(CAR-3236)
                var appealSeq = _qaAppealSearch.GetGridViewSection.GetValueInGridByColRow(3);
                _qaAppealSearch.GetGridViewSection.GetValueInGridByColRow(5).ShouldBeEqual(
                    _qaAppealSearch.GetAppealCompletedDateFromDb(appealSeq),
                    "QA appeal search should show the actual completed date of the appeal rather than the date that the appeal was inserted into the QA_REVIEW table.");
                #endregion

                _qaAppealSearch.GetGridViewSection.IsLabelPresentByCol(6)
                    .ShouldBeFalse("Label should not be displayed for Analyst Name");

                _qaAppealSearch.GetGridViewSection.GetValueInGridByColRow(6).DoesNameContainsOnlyFirstWithLastname()
                    .ShouldBeTrue("Analyst Name should be present in <FirstName> <LastName> format");

                _qaAppealSearch.GetGridViewSection.GetLabelInGridByColRow(9)
                    .ShouldBeEqual("Reviewed:",
                        "QA Review Date section should show the label 'QA Review Date:'");



                var list = _qaAppealSearch.GetGridViewSection.GetGridListValueByCol(9);
                list.RemoveAll(String.IsNullOrEmpty);
                list[0].IsDateInFormat()
                    .ShouldBeTrue("Is QA Review Date in correct format");

                //_qaAppealSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Client",
                //    ClientEnum.SMTST.ToString());
                //_qaAppealSearch.ClickOnFindButton();

                //_qaAppealSearch.ClearFilterSearch();
                //_qaAppealSearch.SelectOutstandingQaAppeals();
                //_qaAppealSearch.ClickOnFindButton();
                //var resultCOunt = _qaAppealSearch.GetGridViewSection.GetGridRowCount();
                //var loadMoreValue = _qaAppealSearch.GetLoadMoreText();
                //var numbers = Regex.Split(loadMoreValue, @"\D+").Where(s => s != String.Empty)
                //    .Select(m => int.Parse(m.Trim())).ToList();
                //var count = numbers[1] % 25 == 0 ? numbers[1] / 25 - 1 : numbers[1] / 25;
                //for (var i = 0; i < count; i++)
                //{
                //    _qaAppealSearch.ClickOnLoadMore();
                //}

                //_qaAppealSearch.IsVerticalScrollBarPresentInResultList()
                //    .ShouldBeTrue("Scrollbar Should display in results showns when results loaded are more than 25.");
                //if (count > 0)
                //    _qaAppealSearch.GetGridViewSection.GetGridRowCount().ShouldBeGreater(resultCOunt,
                //        "Additional results should be shown when user clicks on load more link.");
            }
        }

        [Test] //US69389
        public void Verify_sort_option_and_sorted_search_results_for_QA_Appeal_Search()

        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                QaAppealSearchPage _qaAppealSearch;
                automatedBase.CurrentPage =
                    _qaAppealSearch = automatedBase.QuickLaunch.NavigateToQaAppealSearch();

                _qaAppealSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel
                    ("Quick Search", QaAppealQuickSearchTypeEnum.OutstandingQaAppeals.GetStringValue());
                _qaAppealSearch.ClickOnFindButton();

                _qaAppealSearch.IsListStringSortedInAscendingOrder(5)
                    .ShouldBeTrue("Completed Date should be sorted in Ascending Order by default");
                ValidateQAAppealSearchRowSorted(_qaAppealSearch, 2, 1, "Client");
                ValidateQAAppealSearchRowSorted(_qaAppealSearch, 4, 2, "Category");
                ValidateQAAppealSearchRowSorted(_qaAppealSearch, 6, 3, "Analyst");
                ValidateQAAppealSearchRowSorted(_qaAppealSearch, 7, 4, "Reviewer");
                _qaAppealSearch.ClickOnFilterOptionListRow(5);
                _qaAppealSearch.IsListStringSortedInAscendingOrder(5)
                    .ShouldBeTrue("Completed Date should be sorted in Ascending Order by default");
            }
        }


        [Test] //US69387 + CAR-2349 + TE-994 + CAR-3134
        public void Validation_of_filter_panel_and_expected_plus_default_value_of_filter_panel_input_fields()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                QaAppealSearchPage _qaAppealSearch;
                UserProfileSearchPage _newUserProfileSearch;
                List<string> _assignedClientList;
                List<string> _activeCategoryist;
                List<string> _analystUserList;
                automatedBase.CurrentPage =
                            _qaAppealSearch = automatedBase.QuickLaunch.NavigateToQaAppealSearch();
                var expectedQuickSearchList =
                    automatedBase.DataHelper.GetMappingData(FullyQualifiedClassName, "QuickSearch").Values.ToList();
                List<string> defaultValues = automatedBase.DataHelper
                    .GetMappingData(FullyQualifiedClassName, "QAAppealSearchRestriction").Values.ToList();
                List<string> appealResult = automatedBase.DataHelper
                    .GetMappingData(FullyQualifiedClassName, "AppealResult").Values.ToList();
                _newUserProfileSearch = automatedBase.CurrentPage.NavigateToNewUserProfileSearch();
                _newUserProfileSearch.SearchUserByNameOrId(new List<string> { automatedBase.EnvironmentManager.Username }, true);
                _newUserProfileSearch.ClickOnGridRowByUserIdToOpenUserSettingSideView(automatedBase.EnvironmentManager.Username);
                _newUserProfileSearch.ClickOnUserSettingTabByTabName(UserSettingsTabEnum.Clients.GetStringValue());


                var availableRestrictionList = _newUserProfileSearch.GetAvailableAssignedList(3, false);
                if (availableRestrictionList.Count != 0)
                {
                    availableRestrictionList.Reverse();
                    defaultValues.AddRange(availableRestrictionList);
                }

                _qaAppealSearch = _newUserProfileSearch.NavigateToQaAppealSearch();
                try
                {
                    _analystUserList = _qaAppealSearch.GetAnalystList();
                    _activeCategoryist = _qaAppealSearch.GetAllActiveCategory();

                    StringFormatter.PrintMessageTitle(
                        "Verifying Default value behaviour for Outstanding QA Appeal search Filter input panel fields");
                    var filterList = automatedBase.DataHelper.GetMappingData(FullyQualifiedClassName, "Outstanding_QA_Appeals")
                        .Values.ToList();
                    Verify_correct_search_filter_options_displayed_for(_qaAppealSearch, "Outstanding_QA_Appeals",
                        QaAppealQuickSearchTypeEnum.OutstandingQaAppeals.GetStringValue(), filterList);
                    ValidateDefaultValueForQaAppealSearch(_qaAppealSearch, QaAppealQuickSearchTypeEnum.OutstandingQaAppeals
                        .GetStringValue());
                    var qsDropDownList = _qaAppealSearch.GetSideBarPanelSearch.GetAvailableDropDownList("Quick Search");
                    qsDropDownList.Count.ShouldBeGreater(0, "List of Quick Search is greater than zero.");
                    qsDropDownList.ShouldCollectionBeEqual(expectedQuickSearchList, "Quick Search List is As Expected");
                    _qaAppealSearch.GetSideBarPanelSearch.GetInputValueByLabelPlaceholder("Analyst")
                        .ShouldBeEqual("Select Analyst", "Placeholder value of Analyst is as expected");
                    ValidateDropDownForDefaultValueAndExpectedList(_qaAppealSearch, "Analyst", _analystUserList);
                    _analystUserList[0].DoesNameContainsOnlyFirstWithLastname()
                        .ShouldBeTrue("Analyst should be in proper format of <firstname> <lastname>");

                    #region TE-994

                    _qaAppealSearch.GetSideBarPanelSearch.GetInputValueByLabelPlaceholder("Category")
                        .ShouldBeEqual("Select Category", "Placeholder value of Category is as expected");
                    ValidateDropDownForDefaultValueAndExpectedList(_qaAppealSearch, "Category", _activeCategoryist);

                    #endregion


                    _qaAppealSearch.GetSideBarPanelSearch.GetInputValueByLabelPlaceholder("Client")
                        .ShouldBeEqual("Select Client", "Placeholder value of Client is as expected");
                    _assignedClientList =
                        _qaAppealSearch.GetAssignedClientList(automatedBase.EnvironmentManager.HciAdminUsername);
                    
                    #region CAR-3137
                    _assignedClientList.Insert(0, "All Centene");
                    #endregion
                    
                    ValidateDropDownForDefaultValueAndExpectedList(_qaAppealSearch, "Client", _assignedClientList);
                    Verify_dropdownlist_list_for_Restriction(_qaAppealSearch, "Restrictions", defaultValues);

                    #region CAR-3134
                    ValidateDropDownForDefaultValueAndExpectedList(_qaAppealSearch, "Appeal Result", appealResult,
                        true);
                    #endregion
                    
                    StringFormatter.PrintMessageTitle(
                        "Verifying Default value behaviour for additional filters for All QA Appeal search Filter input panel fields");
                    filterList = automatedBase.DataHelper.GetMappingData(FullyQualifiedClassName, "All_QA_Appeals")
                       .Values.ToList();
                    Verify_correct_search_filter_options_displayed_for(_qaAppealSearch, "All_QA_Appeals",
                        QaAppealQuickSearchTypeEnum.AllQaAppeals.GetStringValue(), filterList);
                    _qaAppealSearch.GetSideBarPanelSearch.GetInputValueByLabelPlaceholder("QA Reviewer")
                        .ShouldBeEqual("Select QA Reviewer", "Placeholder value of QA Reviewer is as expected");
                    ValidateDropDownForDefaultValueAndExpectedList(_qaAppealSearch, "QA Reviewer", _analystUserList);
                    _analystUserList[0].DoesNameContainsOnlyFirstWithLastname()
                        .ShouldBeTrue("QA Reviewer should be in proper format of <firstname> <lastname>");
                    ValidateQaAppealSearchDateFields(_qaAppealSearch, "Completed Date");
                    ValidateQaAppealSearchDateFields(_qaAppealSearch, "QA Review Date");
                    Verify_dropdownlist_list_for_Restriction(_qaAppealSearch, "Restrictions", defaultValues);
                    ValidateDropDownForDefaultValueAndExpectedList(_qaAppealSearch, "Appeal Result", appealResult,
                        true);
                    _qaAppealSearch.ClickOnClearButton();
                    StringFormatter.PrintMessageTitle(
                        "Validate For default value of QA Appeal Search filter panel fields, advanced filter remains open and Quick Search defaults to all");
                    //default values for all should retain but quick search value defaults to all
                    ValidateDefaultValueForQaAppealSearch(_qaAppealSearch, QaAppealQuickSearchTypeEnum.AllQaAppeals.GetStringValue());
                    //Removing this part as a part of CAR-2349
                    //StringFormatter.PrintMessageTitle("Verify Validation fail message");
                    //_qaAppealSearch.GetSideBarPanelSearch.ClickOnClearLink();
                    //_qaAppealSearch.GetSideBarPanelSearch.ClickOnFindButton();
                    //_qaAppealSearch.IsPageErrorPopupModalPresent().ShouldBeTrue("Page error message should be present");
                    //_qaAppealSearch.GetPageErrorMessage()
                    //    .ShouldBeEqual("Search cannot be initiated without any criteria entered.");
                    //_qaAppealSearch.ClosePageError();
                }

                finally
                {
                    automatedBase.CurrentPage = _qaAppealSearch.ClickOnQuickLaunch().NavigateToQaAppealSearch();
                }
            }
        }

        [Test] //CAR-2349
        public void
            Verify_search_results_for_restriction_filter_and_restriction_filter_for_when_there_are_no_restriction_assigned()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                QaAppealSearchPage _qaAppealSearch;
                UserProfileSearchPage _newUserProfileSearch;
                automatedBase.CurrentPage =
                    _qaAppealSearch = automatedBase.QuickLaunch.NavigateToQaAppealSearch();

                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestName);
                List<string> defaultValues = automatedBase.DataHelper
                    .GetMappingData(FullyQualifiedClassName, "QAAppealSearchRestriction").Values.ToList();

                try
                {
                    var _assignedRestrictionAppeaList =
                        _qaAppealSearch.GetSearchResultsForAssignedRestriction(paramLists["UserId"]);
                    _qaAppealSearch.CloseDbConnection();

                    _newUserProfileSearch = automatedBase.CurrentPage.NavigateToNewUserProfileSearch();
                    _newUserProfileSearch.SearchUserByNameOrId(new List<string> { automatedBase.EnvironmentManager.Username }, true);
                    _newUserProfileSearch.ClickOnGridRowByUserIdToOpenUserSettingSideView(automatedBase.EnvironmentManager.Username);
                    _newUserProfileSearch.ClickOnUserSettingTabByTabName(UserSettingsTabEnum.Clients.GetStringValue());


                    var availableRestrictionList = _newUserProfileSearch.GetAvailableAssignedList(3, false);

                    _newUserProfileSearch.SearchUserByNameOrId(
                        new List<string> { automatedBase.EnvironmentManager.HciClaimViewRestrictionUsername }, true);
                    _newUserProfileSearch.ClickOnGridRowByUserIdToOpenUserSettingSideView(automatedBase.EnvironmentManager
                        .HciClaimViewRestrictionUsername);
                    _newUserProfileSearch.ClickOnUserSettingTabByTabName(UserSettingsTabEnum.Clients.GetStringValue());
                    _newUserProfileSearch.GetAvailableAssignedList(3, false)
                        .ShouldCollectionBeEmpty("Restriction should not be assigned");

                    if (availableRestrictionList.Count != 0)
                    {
                        defaultValues.AddRange(availableRestrictionList);
                    }

                    _qaAppealSearch = _newUserProfileSearch.NavigateToQaAppealSearch();


                    //For Outstanding QA appeals
                    _qaAppealSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Analyst",
                        paramLists["Analyst"]);
                    _qaAppealSearch.GetSideBarPanelSearch.ClickOnFindButton();
                    _qaAppealSearch.WaitForWorking();
                    _qaAppealSearch.GetGridViewSection.GetGridListValueByCol(3)
                        .ShouldCollectionBeEquivalent(
                            _qaAppealSearch
                                .GetSearchResultsForAllRestrictionFilteForOutstandingQA(paramLists["UserId"]),
                            "Search results match?");


                    _qaAppealSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Restrictions",
                        defaultValues[1]);
                    _qaAppealSearch.GetSideBarPanelSearch.ClickOnFindButton();
                    _qaAppealSearch.WaitForWorking();
                    _qaAppealSearch.GetGridViewSection.GetGridListValueByCol(3)
                        .ShouldCollectionBeEquivalent(
                            _qaAppealSearch
                                .GetSearchResultsForNoRestrictionFilterForOutstandingQA(paramLists["UserId"]),
                            "Search results match?");

                    for (int i = 0; i < availableRestrictionList.Count; i++)
                    {
                        _qaAppealSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Restrictions",
                            availableRestrictionList[i]);
                        _qaAppealSearch.ClickonFindButton();

                        _qaAppealSearch.GetGridViewSection.GetGridListValueByCol(3)
                            .ShouldCollectionBeEquivalent(_assignedRestrictionAppeaList[i],
                                "Search results match?");
                    }

                    _qaAppealSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Restrictions", "All");
                    _qaAppealSearch.GetSideBarPanelSearch.ClickOnClearLink();

                    //For All QA appeals
                    _qaAppealSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                        QaAppealQuickSearchTypeEnum.AllQaAppeals.GetStringValue());
                    _qaAppealSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Analyst",
                        paramLists["Analyst"]);

                    _qaAppealSearch.GetSideBarPanelSearch.ClickOnFindButton();
                    _qaAppealSearch.WaitForWorking();
                    _qaAppealSearch.GetGridViewSection.GetGridListValueByCol(3)
                        .ShouldCollectionBeEquivalent(
                            _qaAppealSearch.GetSearchResultsForAllRestrictionFilter(paramLists["UserId"]),
                            "Search results match");


                    _qaAppealSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Restrictions",
                        defaultValues[1]);
                    _qaAppealSearch.GetSideBarPanelSearch.ClickOnFindButton();
                    _qaAppealSearch.WaitForWorking();
                    _qaAppealSearch.GetGridViewSection.GetGridListValueByCol(3)
                        .ShouldCollectionBeEquivalent(
                            _qaAppealSearch.GetSearchResultsForNoRestrictionFilter(paramLists["UserId"]),
                            "Search results match");

                    for (int i = 0; i < availableRestrictionList.Count; i++)
                    {
                        _qaAppealSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Restrictions",
                            availableRestrictionList[i]);
                        _qaAppealSearch.ClickonFindButton();
                        _qaAppealSearch.GetGridViewSection.GetGridListValueByCol(3)
                            .ShouldCollectionBeEquivalent(_assignedRestrictionAppeaList[i],
                                "Search results match?");
                    }

                    _qaAppealSearch.GetSideBarPanelSearch.ClickOnClearLink();

                    StringFormatter.PrintMessage(
                        "Verify if the user has no restriction available then only default values are displayed in the restriction list");
                    _qaAppealSearch.Logout().LoginAsClaimViewRestrictionUser();

                    _qaAppealSearch = automatedBase.CurrentPage.NavigateToQaAppealSearch();
                    _qaAppealSearch.GetSideBarPanelSearch.GetAvailableDropDownList("Restrictions")
                        .ShouldBeEqual(defaultValues.GetRange(0, availableRestrictionList.Count),
                            "Only default values available when no restriction is assigned ?");

                }
                finally
                {
                    _qaAppealSearch.UpdateQAReviewToNULLAndUpdateCompletedDateToday(
                        paramLists["AppealSeq2"] + "," + paramLists["AppealSeq3"]);
                }
            }
        }

        [Test] //US69439
        public void Verification_of_assignment_of_completed_appeals_to_QA_Manager()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(3))
            {
                QaAppealSearchPage _qaAppealSearch;
                QaManagerPage _qaManager;
                automatedBase.CurrentPage =
                    _qaAppealSearch = automatedBase.QuickLaunch.NavigateToQaAppealSearch();
                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestName);

                var createdAppealSeq = paramLists["CreatedAppealSeq"].Split(',');
                _qaAppealSearch.SetAppealStatusToNewAndDeleteFromQAAppeal(paramLists["CreatedAppealSeq"],
                    automatedBase.EnvironmentManager.HciAdminUsername3);
                var expectedAppealList = new List<string> { createdAppealSeq[1], createdAppealSeq[3] };
                var notExpectedAppealList = new List<string>
                {
                    createdAppealSeq[0],
                    createdAppealSeq[2],
                    createdAppealSeq[4],
                    createdAppealSeq[5]
                };

                _qaManager = _qaAppealSearch.NavigateToAnalystManager();
                var currentUserFullName = _qaAppealSearch.GetLoggedInUserFullName();
                _qaManager.SearchByAnalyst(currentUserFullName);

                _qaManager.UpdateQAAppealDetails("2", "2");

                var appealSearch = _qaManager.NavigateToAppealSearch();
                appealSearch.CompleteAppeals(createdAppealSeq);

                _qaAppealSearch = automatedBase.QuickLaunch.NavigateToQaAppealSearch();
                _qaAppealSearch.SearchByAnalyst(currentUserFullName);

                var allQAList = _qaAppealSearch.GetGridViewSection.GetGridListValueByCol(3);

                expectedAppealList.CollectionShouldBeSubsetOf(allQAList,
                    "Expected Appeals Should display in QA Appeal Search");
                notExpectedAppealList.CollectionShouldNotBeSubsetOf(allQAList,
                    "Not Expected  List should not display in QA Appeal Search");
                //CAR-929 starts here 
                _qaAppealSearch.GetSideBarPanelSearch.OpenSidebarPanel();
                _qaAppealSearch.GetSideBarPanelSearch.ClickOnClearLink();
                _qaAppealSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                    paramLists["QuickSearch"]);
                //_qaAppealSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Analyst", paramLists["Analyst"]);
                _qaAppealSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Client", paramLists["Client"]);
                _qaAppealSearch.GetSideBarPanelSearch.SetDateFieldFrom("QA Review Date", paramLists["QAReviewDateTo"]);
                _qaAppealSearch.GetSideBarPanelSearch.SetDateFieldTo("QA Review Date", paramLists["QAReviewDateFrom"]);
                _qaAppealSearch.ClickOnFindButton();
                _qaAppealSearch.WaitForWorkingAjaxMessage();
                _qaAppealSearch.GetGridViewSection.GetTitleOfListIconPresentInGridForClassName("small_icon")
                    .ShouldBeEqual("QA Done", "Done Appeals should show a check box instead of X");
                _qaAppealSearch.GetSideBarPanelSearch.ClickOnClearLink();
                _qaAppealSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Analyst", "ui automation_3");
                _qaAppealSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Client", paramLists["Client"]);
                _qaAppealSearch.ClickOnFindButton();
                _qaAppealSearch.WaitForWorkingAjaxMessage();
                if (_qaAppealSearch.GetSideBarPanelSearch.IsNoMatchingRecordFoundMessagePresent())
                {
                    throw new Exception("No Result Found");
                }

                _qaAppealSearch.GetGridViewSection.ClickOnDeleteIconByRowValue(expectedAppealList[0]);
                _qaAppealSearch.GetPageErrorMessage()
                    .ShouldBeEqual(
                        "The selected QA appeal will be permanently deleted. Click Ok to proceed or Cancel.");
                _qaAppealSearch.ClickOkCancelOnConfirmationModal(false);
                var appealSeqList = _qaAppealSearch.GetGridViewSection.GetGridListValueByColAndSort(3);
                appealSeqList.ShouldContain(expectedAppealList[0], "The appeal should not be deleted.");
                Console.Write(expectedAppealList[0]);
                _qaAppealSearch.GetGridViewSection.ClickOnDeleteIconByRowValue(expectedAppealList[0]);
                _qaAppealSearch.ClickOkCancelOnConfirmationModal(true);
                var appealSeqListAfterDelete = _qaAppealSearch.GetGridViewSection.GetGridListValueByColAndSort(3);
                appealSeqListAfterDelete.ShouldNotContain(expectedAppealList[0], "The appeal should be deleted.");



            }
        }

        [Test, Category("AppealDependent")] //US69390+US69385,CAR-850+CAR-931
        public void Verification_of_Appeal_QA_Done_button_and_WorkList()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                QaAppealSearchPage _qaAppealSearch;
                AppealSummaryPage _appealSummary;
                automatedBase.CurrentPage =
                    _qaAppealSearch = automatedBase.QuickLaunch.NavigateToQaAppealSearch();
                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestName);
                var completedDateMultiple = paramLists["CompletedDateMultiple"];
                var completedDateSingle = paramLists["CompletedDateSingle"];
                //Revert appeal sequences to REVIEWED = 'F'
                try
                {
                    automatedBase.CurrentPage.IsRoleAssigned<UserProfileSearchPage>(
                            new List<string> { automatedBase.EnvironmentManager.HciAdminUsername3 },
                            RoleEnum.QAAnalyst.GetStringValue())
                        .ShouldBeTrue(
                            $"Is {AuthorityAssignedEnum.QaManager.GetStringValue()}  present for current user<{automatedBase.EnvironmentManager.HciAdminUsername3}>");

                    automatedBase.CurrentPage.IsAvailableAssignedRowPresent(1, RoleEnum.OperationsManager.GetStringValue())
                        .ShouldBeFalse(
                            $"Is Appeal Manager present for current user<{automatedBase.EnvironmentManager.HciAdminUsername3}>");


                    _qaAppealSearch.RevertQAReviewComplete(paramLists["AppealSequences"],
                        automatedBase.EnvironmentManager.HciAdminUsername3);
                    _qaAppealSearch.RevertQAReviewComplete(paramLists["AppealSequences"],
                        automatedBase.EnvironmentManager.HciUserWithManageEditAuthority);
                    _qaAppealSearch.Logout().LoginAsHciAdminUser3();
                    _qaAppealSearch.NavigateToQaAppealSearch();
                    var currentUserFullName = _qaAppealSearch.GetLoggedInUserFullName();
                    _qaAppealSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                        QaAppealQuickSearchTypeEnum.AllQaAppeals.GetStringValue());
                    _qaAppealSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Analyst",
                        currentUserFullName);
                    _qaAppealSearch.GetSideBarPanelSearch.SetDateFieldFrom("Completed Date", completedDateMultiple);
                    _qaAppealSearch.GetSideBarPanelSearch.ClickOnFindButton();

                    _qaAppealSearch.WaitForWorking();
                    _qaAppealSearch.GetGridViewSection.GetGridListValueByCol(3).Count
                        .ShouldBeGreater(0, "List should be greater than 0");
                    var appealCount = _qaAppealSearch.GetGridViewSection.GetGridListValueByCol(3).Count;
                    var appealSequence = _qaAppealSearch.GetGridViewSection.GetGridListValueByCol(3);
                    _appealSummary =
                        _qaAppealSearch.ClickOnAppealSequenceAndNavigateToAppealSummaryPage(fromGrid: true);
                    for (var i = 0; i < appealCount - 1; i++)
                    {
                        _appealSummary.IsQaCompleteIconInAppealSummaryEnabled()
                            .ShouldBeTrue("QA Complete Icon should be enabled");
                        _appealSummary.ClickOnAppealQaPassIconAndWaitForNextAppeal();
                        _appealSummary.GetAppealSequenceOnHeader().ShouldBeEqual(appealSequence[i + 1],
                            "After QA Done, page should automatically navigates to next appeal in the list");
                    }

                    #region car-931 starts here

                    _appealSummary.ClickOnCompleteQAIcon(true);
                    _appealSummary.IsEditAppealFormDisplayed()
                        .ShouldBeTrue(
                            "Record QA Results form is displayed when complete qa icon is clicked should be true");
                    _appealSummary.SetEditAppealInputValueByLabel("QA Score", "asdd122", true);
                    _appealSummary.IsPageErrorPopupModalPresent().ShouldBeTrue(
                        "Page error pop is displayed when non numeric input is provided in QA score shoud be true");
                    _appealSummary.GetPageErrorMessage()
                        .ShouldBeEqual("Only numbers allowed.", "Error message should be equal");
                    _appealSummary.ClosePageError();
                    _appealSummary.SetEditAppealInputValueByLabel("QA Score", "1234");
                    _appealSummary.GetEditAppealInputValueByLabel("QA Score").Length.ShouldBeEqual(3,
                        "QA score should only accept 3 digits, and length should equal to 3");
                    var notes = new string('a', 4502);
                    _appealSummary.SetLengthyNoteInRecordQaResults(notes);
                    _appealSummary.GetNoteInRecordQaResult().Length
                        .ShouldBeEqual(4500, "Character limit should be 4500 or lesss");
                    const string testNote = "Test note for record qa results";
                    _appealSummary.SetNoteInRecordQaResult(testNote);
                    _appealSummary.SetEditAppealInputValueByLabel("QA Score", "1");
                    _appealSummary.ClickOnSaveButtonOnEditAppeal();

                    var reviewer = _qaAppealSearch.GetSearchResultListByCol(7);
                    var reviewedDate = _qaAppealSearch.GetSearchResultListByCol(9);

                    _qaAppealSearch.GetPageHeader().ShouldBeEqual(PageHeaderEnum.QaAppealSearch.GetStringValue(),
                        "User should taken back to QA Appeal search after last claim of list is approved");
                    _qaAppealSearch.ClickOnAppealSequenceAndNavigateToAppealSummaryPage(fromGrid: true,
                        row: appealCount);
                    _appealSummary.IsQaCompleteIconInAppealSummaryEnabled()
                        .ShouldBeTrue("QA Complete Icon should be enabled");
                    _appealSummary.ClickOnCompleteQAIcon(true);
                    _appealSummary.GetNoteInRecordQaResult()
                        .ShouldBeEqual(testNote, "Note should be saved and should be equal to expected");

                    StringFormatter.PrintMessage(
                        "Verify Clicking on Cancel button will revert all values to previously saved");
                    _appealSummary.SetEditAppealInputValueByLabel("QA Score", "2");
                    _appealSummary.SetNoteInRecordQaResult("updated cancel note");
                    _appealSummary.ClickOnCancelLinkOnEditAppeal();
                    _appealSummary.ClickOnCompleteQAIcon(true);
                    _appealSummary.GetEditAppealInputValueByLabel("QA Score").ShouldBeEqual("1",
                        "Previous QA score should be retained and should be equal to expected");
                    _appealSummary.SetEditAppealInputValueByLabel("QA Score", "3");
                    _appealSummary.SetNoteInRecordQaResult("@update note1");
                    _appealSummary.ClickOnSaveButtonOnEditAppeal();

                    var newReviewer = _qaAppealSearch.GetSearchResultListByCol(7);
                    var newReviewedDate = _qaAppealSearch.GetSearchResultListByCol(9);
                    newReviewer.ShouldCollectionBeEqual(reviewer,
                        "Reviewer list should be equal as original reviewer should not be overwritten.");
                    newReviewedDate.ShouldCollectionBeEqual(reviewedDate,
                        "Reviewed Date list should be equal as original reviewed date should not be overwritten.");
                    _qaAppealSearch.ClickOnAppealSequenceAndNavigateToAppealSummaryPage(fromGrid: true,
                        row: appealCount);
                    StringFormatter.PrintMessage("Verify values in Audit Records");
                    AppealProcessingHistoryPage appealProcessingHistory = _appealSummary.ClickAppealProcessingHx();
                    appealProcessingHistory.GetAppealAuditGridTableDataValue(2, 3)
                        .ShouldBeEqual("QAUpdate",
                            "Closed Appeal Record should added in Appeal Processing Hisory Page");

                    #endregion

                    appealProcessingHistory.GetAppealAuditGridTableDataValue(2, 3)
                        .ShouldBeEqual("QAUpdate",
                            "Closed Appeal Record should added in Appeal Processing Hisory Page");
                    appealProcessingHistory.GetAppealAuditGridTableDataValue(4, 3)
                        .ShouldBeEqual("QADone", "Closed Appeal Record should added in Appeal Processing Hisory Page");
                    appealProcessingHistory.GetAppealAuditGridTableDataValue(4, 2).Split('(')[0].Trim()
                        .ShouldBeEqual(currentUserFullName, "Modified By Value should be Consistent to UserName");
                    appealProcessingHistory.GetAppealAuditGridTableDataValue(4, 1).Split(' ')[0].ShouldBeEqual(
                        DateTime.Now.ToString("M/d/yyyy"), "Modified Date in Audit Records should be consistent");
                    appealProcessingHistory.CloseAppealProcessingHistoryPageToAppealSummaryPage();
                    _qaAppealSearch = _appealSummary.ClickOnSearchIconToNavigateQAAppealSearchPage();
                    StringFormatter.PrintMessage(
                        "Verify for only a single search result the user should be navigated back to the QA Appeal search page");
                    _qaAppealSearch.UpdateQAReviewToNULLForSingleAppeal(paramLists["AppealSequence"],
                        automatedBase.EnvironmentManager.HciAdminUsername3);
                    _qaAppealSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Analyst",
                        currentUserFullName);
                    _qaAppealSearch.GetSideBarPanelSearch.SetDateFieldFrom("Completed Date", completedDateSingle);
                    _qaAppealSearch.GetSideBarPanelSearch.ClickOnFindButton();
                    _qaAppealSearch.WaitForWorking();
                    _qaAppealSearch.ClickOnAppealSequenceAndNavigateToAppealSummaryPage(paramLists["AppealSequence"]);
                    _appealSummary.IsQaCompleteIconInAppealSummaryEnabled()
                        .ShouldBeTrue("QA Complete Icon should be enabled");
                    _appealSummary.ClickOnCompleteQAIcon();
                    _qaAppealSearch.GetPageHeader().ShouldBeEqual(PageHeaderEnum.QaAppealSearch.GetStringValue(),
                        "User should be taken back to QA Appeal search after last appeal of list is approved");
                    _qaAppealSearch.ClickOnAppealSequenceAndNavigateToAppealSummaryPage(paramLists["AppealSequence"]);
                    _appealSummary.ClickOnSearchIconToNavigateQAAppealSearchPage();
                    _qaAppealSearch.GetPageHeader().ShouldBeEqual(PageHeaderEnum.QaAppealSearch.GetStringValue(),
                        "User should be taken back to QA Appeal search after clicking on Return to Appeal Search Page");
                    //CAR 850 starts here ...
                    _qaAppealSearch.RevertQAReviewComplete(paramLists["AppealSequences"],
                        automatedBase.EnvironmentManager.HciAdminUsername3);
                    _qaAppealSearch.Logout().LoginAsHciUserWithManageEditAuthority();
                    //CurrentPage = _profileManager = QuickLaunch.NavigateToProfileManager();
                    //_profileManager.ClickOnPrivileges();
                    //_profileManager.IsAuthorityAssigned(AuthorityAssignedEnum.QaManager.GetStringValue())
                    //    .ShouldBeTrue("User should have Qa Manager authority assigned");
                    //_profileManager.IsAuthorityAssigned(AuthorityAssignedEnum.AppealManager.GetStringValue())
                    //    .ShouldBeFalse("User should not have Appeal Manager authority assigned");
                    _qaAppealSearch.NavigateToQaAppealSearch();
                    _qaAppealSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                        QaAppealQuickSearchTypeEnum.AllQaAppeals.GetStringValue());
                    _qaAppealSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Analyst",
                        currentUserFullName);
                    _qaAppealSearch.GetSideBarPanelSearch.SetDateFieldFrom("Completed Date", completedDateMultiple);
                    _qaAppealSearch.GetSideBarPanelSearch.ClickOnFindButton();
                    _qaAppealSearch.WaitForWorking();
                    _qaAppealSearch.GetGridViewSection.GetGridListValueByCol(3).Count
                        .ShouldBeGreater(0, "List should be greater than 0");
                    appealCount = _qaAppealSearch.GetGridViewSection.GetGridListValueByCol(3).Count;
                    appealSequence = _qaAppealSearch.GetGridViewSection.GetGridListValueByCol(3);
                    _appealSummary =
                        _qaAppealSearch.ClickOnAppealSequenceAndNavigateToAppealSummaryPage(fromGrid: true, row: 1);
                    for (var i = 0; i < appealCount - 1; i++)
                    {
                        _appealSummary.IsQaCompleteIconInAppealSummaryEnabled()
                            .ShouldBeTrue("QA Complete Icon should be enabled");
                        _appealSummary.ClickOnAppealQaPassIconAndWaitForNextAppeal();
                        _appealSummary.GetAppealSequenceOnHeader().ShouldBeEqual(appealSequence[i + 1],
                            "After QA Done, page should automatically navigates to next appeal in the list");
                    }

                    _appealSummary.ClickOnCompleteQAIcon();
                    _qaAppealSearch.GetPageHeader().ShouldBeEqual(PageHeaderEnum.QaAppealSearch.GetStringValue(),
                        "User should taken back to QA Appeal search after last claim of list is approved");
                }
                finally
                {
                    _qaAppealSearch.UpdateQAReviewToReviewed(paramLists["AppealSequences"]);
                }
            }
        }

        [Test] //US69387 + CAR-2349 +TE-994
        public void Verify_that_the_clear_filters_clears_all_filters()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                QaAppealSearchPage _qaAppealSearch;
                automatedBase.CurrentPage =
                    _qaAppealSearch = automatedBase.QuickLaunch.NavigateToQaAppealSearch();
                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestName);
                List<string> restrictionList =
                    _qaAppealSearch.GetSideBarPanelSearch.GetAvailableDropDownList("Restrictions");
                var r = new Random();
                var restrictionlistNext = r.Next(0, restrictionList.Count - 1);

                StringFormatter.PrintMessageTitle(
                    "Validate Clear button clears all filter option for Outstanding QA Appeals Quick Search option");
                _qaAppealSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Analyst", paramLists["Analyst"]);
                _qaAppealSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Category",
                    paramLists["Category"]);
                _qaAppealSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Client", paramLists["Client"]);
                _qaAppealSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Restrictions",
                    restrictionList[restrictionlistNext]);
                _qaAppealSearch.GetSideBarPanelSearch.ClickOnClearLink();
                ValidateDefaultValueForQaAppealSearch(_qaAppealSearch, QaAppealQuickSearchTypeEnum.OutstandingQaAppeals
                    .GetStringValue());
                StringFormatter.PrintMessageTitle(
                    "Validate Clear button clears all filter option for All QA Appeals Quick Search option");
                _qaAppealSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                    QaAppealQuickSearchTypeEnum.AllQaAppeals.GetStringValue());
                _qaAppealSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Analyst", paramLists["Analyst"]);
                _qaAppealSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Category",
                    paramLists["Category"]);
                _qaAppealSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Client", paramLists["Client"]);
                _qaAppealSearch.GetSideBarPanelSearch.SetDateFieldFrom("Completed Date",
                    DateTime.Now.ToString("MM/d/yyyy"));
                _qaAppealSearch.GetSideBarPanelSearch.SetDateFieldTo("Completed Date",
                    DateTime.Now.AddDays(7).ToString("MM/d/yyyy"));
                _qaAppealSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("QA Reviewer",
                    paramLists["Reviewer"]);
                _qaAppealSearch.GetSideBarPanelSearch.SetDateFieldFrom("QA Review Date",
                    DateTime.Now.ToString("MM/d/yyyy"));
                _qaAppealSearch.GetSideBarPanelSearch.SetDateFieldTo("QA Review Date",
                    DateTime.Now.AddDays(7).ToString("MM/d/yyyy"));
                _qaAppealSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Restrictions",
                    restrictionList[restrictionlistNext]);
                _qaAppealSearch.GetSideBarPanelSearch.ClickOnClearLink();
                ValidateDefaultValueForQaAppealSearch(_qaAppealSearch, QaAppealQuickSearchTypeEnum.AllQaAppeals.GetStringValue());
            }
        }

        [Test] //CAR-728
        public void Verify_find_button_is_disabled_when_search_is_active()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                QaAppealSearchPage _qaAppealSearch;
                automatedBase.CurrentPage =
                    _qaAppealSearch = automatedBase.QuickLaunch.NavigateToQaAppealSearch();
                _qaAppealSearch.GetSideBarPanelSearch.OpenSidebarPanel();
                _qaAppealSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                    "Outstanding QA Appeals");
                _qaAppealSearch.ClickFindAndCheckIfFindButtonIsDisabled()
                    .ShouldBeTrue("Find Button Should be disabled while the search is active.");
                _qaAppealSearch.WaitForWorkingAjaxMessage();
                _qaAppealSearch.GetGridViewSection.GetGridRowCount()
                    .ShouldBeGreater(0, "Search results should be displayed");
                _qaAppealSearch.CheckIfFindButtonIsEnabled()
                    .ShouldBeTrue("Find Button Should be enabled once the search is complete.");
            }
        }

        [Test] //CAR-1011 (CAR-2305)
        public void Verify_QA_record_is_deleted_when_appeal_is_deleted()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                QaAppealSearchPage _qaAppealSearch;
                automatedBase.CurrentPage =
                    _qaAppealSearch = automatedBase.QuickLaunch.NavigateToQaAppealSearch();
                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestName);
                var claSeq = paramLists["ClaimSequence"];
                var reasonCode = paramLists["ReasonCode"];
                var _appealCreator = automatedBase.CurrentPage.Logout().LoginAsHciAdminUser1().NavigateToAppealCreator();

                try
                {
                    StringFormatter.PrintMessageTitle("Creating an appeal by uiautomation_1");
                    _appealCreator.SearchByClaimSequence(claSeq);
                    if (_appealCreator.IsClaimLocked() && _appealCreator.GetPageHeader() == "Claim Search")
                    {
                        DeletePreviousAppeals(claSeq);
                        _appealCreator.SearchByClaimSequence(claSeq);
                    }

                    _appealCreator.CreateAppealForClaimSequence(ProductEnum.CV.GetStringValue(), false, "testDocID");
                    StringFormatter.PrintMessage("Completing the appeal so that it appears in the QA Appeal List");
                    var _appealSearch = _appealCreator.NavigateToAppealSearch();
                    _appealSearch.SelectSMTST();
                    var _appealAction = _appealSearch.SearchByClaimSequenceToGoAppealAction(claSeq);
                    var appealSeq = _appealAction.GetAppealSequence();
                    _appealAction.CompleteAppeals(null, reasonCode, "Test Rationale");
                    StringFormatter.PrintMessage(
                        "Verifying whether newly created appeal is listed under QA Appeal Search results");
                    _qaAppealSearch = automatedBase.CurrentPage.NavigateToQaAppealSearch();
                    SearchForAppealInQaAppealSearch();
                    _qaAppealSearch.GetGridViewSection.GetGridListValueByCol(3).Contains(appealSeq)
                        .ShouldBeTrue(
                            "Newly created Appeal should be listed in the search results in QA Appeal Search");
                    StringFormatter.PrintMessage("Deleting the newly created appeal");
                    var _appealManager = _qaAppealSearch.NavigateToAppealManager();
                    _appealManager.DeleteAppealByAppealSeq(appealSeq);
                    StringFormatter.PrintMessageTitle(
                        "Verifying whether the deleted appeal record is deleted from the QA Appeal Search as well");
                    _qaAppealSearch = _appealManager.NavigateToQaAppealSearch();
                    SearchForAppealInQaAppealSearch();
                    _qaAppealSearch.GetGridViewSection.GetGridListValueByCol(3).Contains(appealSeq)
                        .ShouldBeFalse(
                            "Deleted Appeal should not be present in the search results in QA Appeal Search");

                    #region LOCAL METHODS

                    void SearchForAppealInQaAppealSearch()
                    {
                        var dateCompleted = _qaAppealSearch.CurrentDateTimeInMst(null).Date.ToString("MM/dd/yyyy");

                        _qaAppealSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                            QaAppealQuickSearchTypeEnum.AllQaAppeals.GetStringValue());
                        _qaAppealSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Client",
                            ClientEnum.SMTST.ToString());
                        _qaAppealSearch.GetSideBarPanelSearch.SetDateField("Completed Date", dateCompleted, 1);
                        _qaAppealSearch.GetSideBarPanelSearch.SetDateField("Completed Date", dateCompleted, 2);
                        _qaAppealSearch.ClickOnFindButton();
                    }

                    void DeletePreviousAppeals(string claimSeq)
                    {
                        if (automatedBase.CurrentPage.GetPageHeader() != "Appeal Manager")
                            automatedBase.CurrentPage.NavigateToAppealManager().DeleteAppealsAssociatedWithClaim(claimSeq);

                        automatedBase.CurrentPage.NavigateToAppealCreator();
                    }

                    #endregion
                }

                finally
                {
                    var _appealManager = automatedBase.CurrentPage.NavigateToAppealManager();
                    _appealManager.DeleteAppealsAssociatedWithClaim(claSeq);
                }
            }
        }


        [Test] //TE-994
        public void Verify_Search_Result_For_Search_By_Category()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                QaAppealSearchPage _qaAppealSearch;
                automatedBase.CurrentPage =
                    _qaAppealSearch = automatedBase.QuickLaunch.NavigateToQaAppealSearch();
                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestName);
                var category_code = paramLists["Category"];
                _qaAppealSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Category", category_code);
                _qaAppealSearch.ClickonFindButton();
                _qaAppealSearch.WaitForWorking();
                _qaAppealSearch.GetGridViewSection.GetGridListValueByCol(3).ShouldCollectionBeEquivalent(
                    _qaAppealSearch.GetOutStandingQAAppealByCategory(category_code), "Search Results Equal");
                _qaAppealSearch.GetSideBarPanelSearch.ClickOnClearLink();

                _qaAppealSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                    QaAppealQuickSearchTypeEnum.AllQaAppeals.GetStringValue());
                _qaAppealSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Category", category_code);
                _qaAppealSearch.ClickonFindButton();
                _qaAppealSearch.WaitForWorking();
                _qaAppealSearch.GetGridViewSection.GetGridListValueByCol(3)
                    .ShouldCollectionBeEquivalent(_qaAppealSearch.GetAllQAAppealByCategory(category_code),
                        "Search Results Equal");

                _qaAppealSearch.GetSideBarPanelSearch.ClickOnClearLink();

            }
        }

        [Test] //CAR-3134 (CAR-3280)
        [Author("Pujan Aryal")]
        public void Verify_Search_Result_For_Search_By_Appeal_Result()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                QaAppealSearchPage _qaAppealSearch = automatedBase.QuickLaunch.NavigateToQaAppealSearch();
                Dictionary<string, string> appealResults = new Dictionary<string, string>()
                {
                    [AppealResult.Pay.GetStringDisplayValue()]    = AppealResult.Pay.GetStringValue(),
                    [AppealResult.Deny.GetStringDisplayValue()]   = AppealResult.Deny.GetStringValue(),
                    [AppealResult.Adjust.GetStringDisplayValue()] = AppealResult.Adjust.GetStringValue(),
                    [AppealResult.NoDocs.GetStringDisplayValue()] = AppealResult.NoDocs.GetStringValue()

                };

                //Outstanding QA Appeals
                StringFormatter.PrintMessage("Verify Search Result For Outstanding QA Appeals");
                foreach (KeyValuePair<string, string> appealResult in appealResults)
                {
                    _qaAppealSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Client", ClientEnum.SMTST.ToString());
                    _qaAppealSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Appeal Result", appealResult.Key);
                    _qaAppealSearch.ClickonFindButton();
                    _qaAppealSearch.WaitForWorking();

                    if (_qaAppealSearch.GetGridViewSection.IsLoadMorePresent())
                        _qaAppealSearch.GetGridViewSection.ClickLoadMoreIterativelyToShowAllRecords();

                    _qaAppealSearch.GetAppealResultTypeInGridViewSection().Distinct().Count()
                        .ShouldBeEqual(1, "Appeal Result Count should be equal");
                    _qaAppealSearch.GetAppealResultTypeInGridViewSection().Distinct()
                        .ShouldBeEqual(new List<string> { appealResult.Value },$"Result should show data for {appealResult.Key} appeal result");
                    
                    StringFormatter.PrintMessage("Verifying the search result with DB");
                    _qaAppealSearch.GetGridViewSection.GetGridListValueByCol(3).ShouldCollectionBeEquivalent(
                        _qaAppealSearch.GetOutStandingQAAppealByAppealResult(appealResult.Value), $"Search Result should yield outstanding QA Appeals data for appeal result {appealResult.Key}");
                    _qaAppealSearch.GetSideBarPanelSearch.ClickOnClearLink();

                }

                //All Appeals
                StringFormatter.PrintMessage("Verify Search Result For All QA Appeals");
                foreach (KeyValuePair<string, string> appealResult in appealResults)
                {
                    _qaAppealSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                        QaAppealQuickSearchTypeEnum.AllQaAppeals.GetStringValue());
                    _qaAppealSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Client", ClientEnum.SMTST.ToString());
                    _qaAppealSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Appeal Result", appealResult.Key);
                    _qaAppealSearch.ClickonFindButton();
                    _qaAppealSearch.WaitForWorking();
                    if (_qaAppealSearch.GetGridViewSection.IsLoadMorePresent())
                        _qaAppealSearch.GetGridViewSection.ClickLoadMoreIterativelyToShowAllRecords();
                    _qaAppealSearch.GetAppealResultTypeInGridViewSection().Distinct().Count()
                        .ShouldBeEqual(1, "Appeal Result Count should be equal");
                    _qaAppealSearch.GetAppealResultTypeInGridViewSection().Distinct().ShouldBeEqual(new List<string> { appealResult.Value },$"Result should show data for {appealResult.Key} appeal result");

                    StringFormatter.PrintMessage("Verifying the search result with DB");
                    _qaAppealSearch.GetGridViewSection.GetGridListValueByCol(3).ShouldCollectionBeEquivalent
                        (_qaAppealSearch.GetAllQAAppealsByAppealResult(appealResult.Value), $"Search Result should yield all QA Appeals data for appeal result {appealResult.Key}");
                    _qaAppealSearch.GetSideBarPanelSearch.ClickOnClearLink();

                }
            }
        }

        [Test, Category("UatTest")] //CAR-3281 [CAR-3137]
        [Author("Shyam Bhattarai")]
        public void Verify_All_Centene_Option_In_Client_Filter()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                QaAppealSearchPage _qaAppealSearch;
                automatedBase.CurrentPage = _qaAppealSearch = automatedBase.QuickLaunch.NavigateToQaAppealSearch();

                StringFormatter.PrintMessage("Verify 'All Centene' client option for Outstanding QA Appeals");
                _qaAppealSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Client", "All Centene");
                _qaAppealSearch.ClickonFindButton();

                if (_qaAppealSearch.GetGridViewSection.IsLoadMorePresent())
                    _qaAppealSearch.GetGridViewSection.ClickLoadMoreIterativelyToShowAllRecords();

                StringFormatter.PrintMessage("Verifying the search result with DB for Outstanding QA Appeals");
                _qaAppealSearch.GetGridViewSection.GetGridListValueByCol(3)
                    .ShouldCollectionBeEquivalent(
                        _qaAppealSearch.GetQaAppealsForAllCenteneClient("uiautomation", false),
                        "Search result should display correct result for Outstanding QA Appeals for All Centene clients");
                _qaAppealSearch.GetSideBarPanelSearch.ClickOnClearLink();

                StringFormatter.PrintMessage("Verifying the search result with DB for All QA Appeals");
                _qaAppealSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search", 
                    QaAppealQuickSearchTypeEnum.AllQaAppeals.GetStringValue());

                _qaAppealSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Client", "All Centene");
                _qaAppealSearch.ClickonFindButton();

                if (_qaAppealSearch.GetGridViewSection.IsLoadMorePresent())
                    _qaAppealSearch.GetGridViewSection.ClickLoadMoreIterativelyToShowAllRecords();

                StringFormatter.PrintMessage("Verifying the search result with DB for All QA Appeals");
                _qaAppealSearch.GetGridViewSection.GetGridListValueByCol(3)
                    .ShouldCollectionBeEquivalent(_qaAppealSearch.GetQaAppealsForAllCenteneClient(automatedBase.EnvironmentManager.Username),
                        "Search result should display correct result for All QA Appeals for All Centene clients");
            }
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        //private void ValidateListSorted(int col, string sortOption, string colName)
        //{
        //    _qaAppealSearch.ClickOnFilterOptionList(sortOption);
        //    _qaAppealSearch.GetGridViewSection.GetGridListValueByCol(col).IsInAscendingOrder()
        //        .ShouldBeTrue(string.Format("The list is sorted in ascending order of {0}", colName));

        //    _qaAppealSearch.ClickOnFilterOptionList(sortOption);
        //    _qaAppealSearch.GetGridViewSection.GetGridListValueByCol(col).IsInDescendingOrder()
        //        .ShouldBeTrue(string.Format("The list is sorted in descending order of {0}", colName));
        //}

        private void Verify_correct_search_filter_options_displayed_for(QaAppealSearchPage _qaAppealSearch, string mappingQuickSearchOptionName, string quickSearchValue, List<string> filertList)
        {

            _qaAppealSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search", quickSearchValue);
            _qaAppealSearch.GetSideBarPanelSearch.GetSearchFiltersList()
                .ShouldCollectionBeEqual(filertList
                    , "Search Filters",
                    true);

        }

        private void ValidateQaAppealSearchDateFields(QaAppealSearchPage _qaAppealSearch, string fieldName)
        {
            _qaAppealSearch.GetDateRangeInputFieldsCount(fieldName)
                .ShouldBeEqual(2, "A beginning and end date range filters are present");
            var beginDate = DateTime.Now.AddDays(-10).ToString("MM/d/yyyy");
            var endDate = DateTime.Now.AddMonths(3).ToString("MM/d/yyyy");
            _qaAppealSearch.GetSideBarPanelSearch.SetDateFieldFrom(fieldName, beginDate);
            var approveDate = _qaAppealSearch.GetSideBarPanelSearch.GetDateFieldTo(fieldName);
            approveDate.ShouldBeEqual(Convert.ToDateTime(beginDate).ToString("MM/dd/yyyy"),
                "Date populated in the beginning date is automatically populated in the end date field");
            approveDate.IsDateInFormat()
                .ShouldBeTrue("Date should be in MM/DD/YYYY format");
            _qaAppealSearch.GetSideBarPanelSearch.SetDateFieldTo(fieldName, endDate);
            _qaAppealSearch.ClickOnFindButton(false);
            ValidateForPopUpMessage(_qaAppealSearch, "Popup Displayed when date range exceeds 3 months",
                "Date range selected must not exceed 3 months.");
        }

        private void ValidateForPopUpMessage(QaAppealSearchPage _qaAppealSearch, string popUpMessgae, string expectedError)
        {
            _qaAppealSearch.WaitToLoadPageErrorPopupModal();
            _qaAppealSearch.IsPageErrorPopupModalPresent()
                .ShouldBeTrue(popUpMessgae);
            _qaAppealSearch.GetPageErrorMessage()
                .ShouldBeEqual(expectedError,
                    popUpMessgae);
            _qaAppealSearch.ClosePageError();
        }


        private void ValidateDefaultValueForQaAppealSearch(QaAppealSearchPage _qaAppealSearch, string quickSearchValue)
        {
            if (quickSearchValue == QaAppealQuickSearchTypeEnum.AllQaAppeals.GetStringValue())
            {
                StringFormatter.PrintMessageTitle("Validate For default value of QA Claim Search filter panel fields");
                _qaAppealSearch.GetSideBarPanelSearch.GetInputValueByLabel("Quick Search")
                    .ShouldBeEqual(quickSearchValue, "Default value is as expected");
                _qaAppealSearch.GetSideBarPanelSearch.GetInputValueByLabel("Analyst")
                    .ShouldBeNullorEmpty("Analyst value defaults to blank ");
                _qaAppealSearch.GetSideBarPanelSearch.GetInputValueByLabel("Category")
                    .ShouldBeNullorEmpty("Analyst value defaults to blank ");
                _qaAppealSearch.GetSideBarPanelSearch.GetInputValueByLabel("Client")
                    .ShouldBeNullorEmpty("Client value defaults to blank ");
                _qaAppealSearch.GetSideBarPanelSearch.GetInputValueByLabel("QA Reviewer")
                    .ShouldBeNullorEmpty("QA Reviewer value defaults to blank ");
                _qaAppealSearch.GetSideBarPanelSearch.GetDateFieldTo("Completed Date")
                    .ShouldBeNullorEmpty("Approve End Date field value defaults to blank ");
                _qaAppealSearch.GetSideBarPanelSearch.GetDateFieldFrom("Completed Date")
                    .ShouldBeNullorEmpty("Approve From Date field value defaults to blank ");
                _qaAppealSearch.GetSideBarPanelSearch.GetDateFieldFrom("QA Review Date")
                    .ShouldBeNullorEmpty("QA Review From Date field value defaults to blank ");
                _qaAppealSearch.GetSideBarPanelSearch.GetDateFieldTo("QA Review Date")
                    .ShouldBeNullorEmpty("QA Review End Date field value defaults to blank ");
                _qaAppealSearch.GetSideBarPanelSearch.GetInputValueByLabel("Restrictions")
                    .ShouldBeEqual("All", "Restrictions dropdown should default to All");
                _qaAppealSearch.GetSideBarPanelSearch.GetInputValueByLabel("Appeal Result").ShouldBeNullorEmpty("Appeal result value defaults to blank ");
            }
            else
            {
                StringFormatter.PrintMessageTitle("Validate For default value of Outstanding QA Appeal Search filter panel fields");
                _qaAppealSearch.GetSideBarPanelSearch.GetInputValueByLabel("Quick Search")
                    .ShouldBeEqual(quickSearchValue, "Default value is as expected");
                _qaAppealSearch.GetSideBarPanelSearch.GetInputValueByLabel("Analyst")
                    .ShouldBeNullorEmpty("Analyst value defaults to blank ");
                _qaAppealSearch.GetSideBarPanelSearch.GetInputValueByLabel("Category")
                    .ShouldBeNullorEmpty("Analyst value defaults to blank ");
                _qaAppealSearch.GetSideBarPanelSearch.GetInputValueByLabel("Client")
                    .ShouldBeNullorEmpty("Client value defaults to blank ");
                _qaAppealSearch.GetSideBarPanelSearch.GetInputValueByLabel("Restrictions")
                    .ShouldBeEqual("All", "Restrictions dropdown should default to All");
                _qaAppealSearch.GetSideBarPanelSearch.GetInputValueByLabel("Appeal Result").ShouldBeNullorEmpty("Appeal result value defaults to blank ");
            }
        }

        private void ValidateDropDownForDefaultValueAndExpectedList(QaAppealSearchPage _qaAppealSearch, string label, IList<string> collectionToEqual, bool qAResult = false)
        {
            var actualDropDownList = _qaAppealSearch.GetSideBarPanelSearch.GetAvailableDropDownList(label);
            actualDropDownList.Count.ShouldBeGreater(0, "List of " + label + " is greater than zero.");
            if (collectionToEqual != null)
            {
                actualDropDownList.RemoveAll(item => item == "");
                actualDropDownList.ShouldCollectionBeEqual(collectionToEqual, label + " List is As Expected");
            }
            if (!qAResult) actualDropDownList.IsInAscendingOrder().ShouldBeTrue(label + " should be sorted in alphabetical order.");
            _qaAppealSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel(label, actualDropDownList[0], false); //check for type ahead functionality
            _qaAppealSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel(label, actualDropDownList[1]);
            _qaAppealSearch.GetSideBarPanelSearch.GetInputValueByLabel(label).ShouldBeEqual(actualDropDownList[1], "User can select only a single option");
        }

        private void ValidateQAAppealSearchRowSorted(QaAppealSearchPage _qaAppealSearch, int col, int sortOptionRow, string colName)
        {
            _qaAppealSearch.ClickOnFilterOptionListRow(sortOptionRow);
            _qaAppealSearch.IsListStringSortedInAscendingOrder(col)
                .ShouldBeTrue(string.Format("{0} Should be sorted in Ascending Order", colName));
            _qaAppealSearch.ClickOnFilterOptionListRow(sortOptionRow);
            _qaAppealSearch.IsListStringSortedInDescendingOrder(col)
                .ShouldBeTrue(string.Format("{0} Should be sorted in Descending Order", colName));
        }

        private void Verify_dropdownlist_list_for_Restriction(QaAppealSearchPage _qaAppealSearch, string label, List<string> availableRestriction)
        {
            var actualDropDownList = _qaAppealSearch.GetSideBarPanelSearch.GetAvailableDropDownList(label);
            actualDropDownList.ShouldCollectionBeEquivalent(availableRestriction, "Available restrictions should match");
            _qaAppealSearch.GetSideBarPanelSearch.GetInputValueByLabel("Restriction")
                .ShouldBeEqual("All");
            _qaAppealSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel(label, actualDropDownList[1], false); //check for type ahead functionality
            _qaAppealSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel(label, actualDropDownList[0]);
        }
        #endregion
    }
}

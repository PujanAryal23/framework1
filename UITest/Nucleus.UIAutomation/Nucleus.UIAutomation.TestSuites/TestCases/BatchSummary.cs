using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Nucleus.Service.Data;
using Nucleus.Service.PageServices.Batch;
using Nucleus.Service.PageServices.Settings.User;
using Nucleus.Service.Support.Common.Constants;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Utils;
using Nucleus.UIAutomation.TestSuites.Base;
using NUnit.Framework;

namespace Nucleus.UIAutomation.TestSuites.TestCases
{
    public class BatchSummary : NewAutomatedBase
    {
        #region PRIVATE PROPERTIES

        private BatchSummaryPage _batchSummary;
        private BatchSearchPage _batchSearch;
        private ProfileManagerPage _profileManager;

        string[] _quadrantTitle =
        {
            "Return Files", "Stats by Product", "Processing History"
        };

        #endregion



        #region PROTECTED PROPERTIES

        protected override string FullyQualifiedClassName
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
                _batchSummary.CloseDbConnection();
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

            if (CurrentPage.GetPageHeader() == PageHeaderEnum.BatchSummary.GetStringValue())
            {
                CurrentPage = _batchSearch = _batchSummary.ClickOnBackButtonAndNavigateBackToBatchSearchPage();
            }

            if (_batchSearch.GetPageHeader() != PageHeaderEnum.BatchSearch.GetStringValue())
            {
                _batchSearch.ClickOnQuickLaunch().NavigateToBatchSearch();
            }

            if (!_batchSearch.SideBarPanelSearch.IsSideBarPanelOpen())
                _batchSearch.SideBarPanelSearch.ClickOnToggleSidebarPanelButton();
            _batchSearch.SideBarPanelSearch.ClickOnClearLink();
            _batchSearch.SideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                BatchQuickSearchTypeEnum.IncompleteBatches.GetStringValue());
            _batchSearch.SideBarPanelSearch.ClickOnFindButton();
            _batchSearch.WaitForWorkingAjaxMessage();
            base.TestCleanUp();
        }

        #endregion

        #region TEST SUITES

        [Test] //TE-57+TE-155
        public void Verify_batch_information_in_batch_summary_page_in_q1_quadrant()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            IDictionary<string, string> paramLists =
                DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
            var batchID = paramLists["BatchID"];
            var expectedFirstRowHeaderLabelList = paramLists["FirstRowHeaderLabel"].Split(';').ToList();
            var expectedFirstColumnHeaderLabelList = paramLists["FirstColumnHeaderLabel"].Split(';').ToList();

            string[] batchLabel =
            {
                "Batch Date", "Review Date", "Received Date","Received FTP File", "Processing Started", "Processing Completed"
            };

            SearchByBatchID(batchID);
            CurrentPage = _batchSummary =
                _batchSearch.ClickOnBatchIdAndNavigateToBatchSummaryPage(batchID);
            _batchSummary.GetPageHeader().ShouldBeEqual(PageHeaderEnum.BatchSummary.GetStringValue(),
                "Page Header should be equal");
            var batchInfo = _batchSummary.GetBatchInformationInQaQuadrant(batchID).ToList();
            _batchSummary.GetValuesByLabelListInQ1Qudrant(batchLabel)
                .ShouldCollectionBeEqual(batchInfo[0], "Is Correct Value in Q1 Top Quadrant?");

            _batchSummary.GetBatchIdValue().ShouldBeEqual(batchID,
                "Correct Batch ID value should be displayed next to page header.");
            _batchSummary.IsBackIconPresent()
                .ShouldBeTrue("Is Magnifying Glass icon present in the header to the right of Q1?");

            _batchSummary.GetFirstRowHeaderLabel().ShouldCollectionBeEqual(expectedFirstRowHeaderLabelList,
                "First Row label should be equal");
            _batchSummary.GetColumnHeaderLabel().ShouldCollectionBeEqual(expectedFirstColumnHeaderLabelList,
                "First Column label should be equal");
            _batchSummary.GetBatchInformationRightDivValue().ShouldCollectionBeEqual(batchInfo[1],
                "Correct Values displayed for Q1 Bottom Batch Information");

            StringFormatter.PrintMessageTitle("Verify back button and search results are retained");
            CurrentPage = _batchSearch = _batchSummary.ClickOnBackButtonAndNavigateBackToBatchSearchPage();
            _batchSearch.GetPageHeader()
                .ShouldBeEqual(PageHeaderEnum.BatchSearch.GetStringValue(),
                    "Page should navigate back to Batch Search page");
            _batchSearch.SideBarPanelSearch.IsSideBarPanelOpen().ShouldBeTrue("Is Side bar panel open?");
            _batchSearch.SideBarPanelSearch.GetInputValueByLabel("Batch ID")
                .ShouldBeEqual(batchID, "Input Value Should retain");
            _batchSearch.GetGridViewSection.GetGridRowCount()
                .ShouldBeEqual(1, "Searched Grid Row Count Should retain");
            _batchSearch.GetGridViewSection.GetValueInGridByColRow().ShouldBeEqual(batchID,
                "Verify Searched Grid Batch Id Correct after coming back from Batch Summary to Batch Search Page");

        }

        [Test,Category("Acceptance")] //TE-57 +TE-86 + CAR-3252 (CAR-3313)
        public void Verify_Stats_by_Product_in_batch_summary_page_in_q3_quadrant()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            var paramList = DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
            var batchID = paramList["BatchID"];
            var expectedDataLabelList = paramList["DataLabels"].Split(';').ToList();
            var expectedDollarLabelList = paramList["DollarLabels"].Split(';').ToList();

            SearchByBatchID(batchID);

            CurrentPage = _batchSummary =
                _batchSearch.ClickOnBatchIdAndNavigateToBatchSummaryPage(batchID);
            var expectedActiveProductList = _batchSummary.GetActiveProductListForClientDB();
            var expectedDataValueList = _batchSummary.GetDataValueListByActiveProduct(batchID);
            var expectedDollarValueList = _batchSummary.GetDollarValueListByActiveProduct(batchID);

            _batchSummary.GetQuadrantTitle(1)
                .ShouldBeEqual(_quadrantTitle[1], "Quadrant Header should be equal");
            _batchSummary.IsDollarValueIconPresent()
                .ShouldBeTrue("Is Dollar value icon in right side of Stats by Product quadrant present?");
            _batchSummary.IsCountValueIconPresent()
                .ShouldBeTrue("Is Dollar value icon in right side of Stats by Product quadrant present?");
            _batchSummary.IsDataValueIconSelected()
                .ShouldBeTrue("Default view of Stats By Product container should be data values of batch");
            _batchSummary.IsDollarValueIconSelected().ShouldBeFalse("Is Dollar value Icon selected?");
            _batchSummary.GetActiveProductList().ShouldCollectionBeEqual(expectedActiveProductList,
                "Stats of Active product assigned to current Client should only be displayed.");

            StringFormatter.PrintMessage("Validation of Product Statistics Data Container Labels for batch");
            _batchSummary.GetStatsByProductFirstColumnLabel()
                .ShouldCollectionBeEqual(expectedDataLabelList.GetRange(0, 3), "First Column label.");
            _batchSummary.GetStatsByProductLabelByRow()
                .ShouldCollectionBeEqual(expectedDataLabelList.GetRange(3, 6), "Cotiviti Data point labels");
            _batchSummary.GetStatsByProductLabelByRow(2)
                .ShouldCollectionBeEqual(expectedDataLabelList.GetRange(9, 4), "Client data point labels");
            _batchSummary.GetStatsByProductLabelByRow(3)
                .ShouldCollectionBeEqual(expectedDataLabelList.GetRange(13, 2), "Total Flagged labels");

            StringFormatter.PrintMessage("Verify Data Values");

            for (int i = 0; i < expectedActiveProductList.Count; i++)
            {
                _batchSummary.GetStatsByProductList(expectedActiveProductList[i])
                    .ShouldCollectionBeEqual(expectedDataValueList[i],
                        "Data Values for the batch by product: " + expectedActiveProductList[i]);

            }

            StringFormatter.PrintMessage("Verify Dollar Values");
            _batchSummary.ClickOnDollarValueIcon();
            _batchSummary.IsDollarValueIconSelected().ShouldBeTrue("Is Dollar value Icon selected?");
            _batchSummary.IsDataValueIconSelected().ShouldBeFalse("Is Data Value Icon selected");
            for (int i = 0; i < expectedActiveProductList.Count; i++)
            {
                _batchSummary.GetStatsByProductList(expectedActiveProductList[i])
                    .ShouldCollectionBeEqual(expectedDollarValueList[i],
                        "Dollar Values for the batch by product: " + expectedActiveProductList[i]);
            }
            StringFormatter.PrintMessage("Validation of Product Statistics Dollar C Labels");
            _batchSummary.GetStatsByProductFirstColumnLabel()
                .ShouldCollectionBeEqual(expectedDollarLabelList.GetRange(0, 3), "First Column label.");
            _batchSummary.GetStatsByProductLabelByRow()
                .ShouldCollectionBeEqual(expectedDollarLabelList.GetRange(3, 6), "Cotiviti Data point labels");
            _batchSummary.GetStatsByProductLabelByRow(2)
                .ShouldCollectionBeEqual(expectedDollarLabelList.GetRange(9, 4), "Client data point labels");
            _batchSummary.GetStatsByProductLabelByRow(3)
                .ShouldCollectionBeEqual(expectedDollarLabelList.GetRange(13, 2), "Total Flagged labels");
        }


        [Test] //TE-57+TE-155
        public void Verify_batch_processing_history_in_batch_summary_page_in_q4_quadrant()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            var batchID = DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                "BatchID", "Value");

            SearchByBatchID(batchID);
            CurrentPage = _batchSummary =
                _batchSearch.ClickOnBatchIdAndNavigateToBatchSummaryPage(batchID);
            _batchSummary.GetQuadrantTitle(2)
                .ShouldBeEqual(_quadrantTitle[2], "Quadrant Header should be equal");
            var expectedList = _batchSummary.GetBatchReleaseListFromDatabase(batchID);

            _batchSummary.GetLabelInProcessingHistoryByRow()
                .ShouldBeEqual("Release User:", "Release User label should be correct");
            _batchSummary.GetLabelInProcessingHistoryByRow(1, 2)
                .ShouldBeEqual("Release Date:", "Release Date label should be correct");
            _batchSummary.GetProcessingHistoryListByColumn()
                .ShouldCollectionBeEqual(expectedList.Select(x => x[0]).ToList(),
                    "Is Released User List Collection Equal?");
            _batchSummary.GetProcessingHistoryListByColumn(2)
                .ShouldCollectionBeEqual(expectedList.Select(x => x[1]).ToList(),
                    "Is Released Date List Collection Equal?");
        }

        [Test] //TE-57+TE-155
        public void Verify_return_files_in_batch_summary_page_in_q2_quadrant()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            var batchID = DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                "BatchID", "Value");
            var batchSeq = DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName,
                "BatchSeq", "Value");

            SearchByBatchID(batchID);
            CurrentPage = _batchSummary =
                _batchSearch.ClickOnBatchIdAndNavigateToBatchSummaryPage(batchID);
            _batchSummary.GetQuadrantTitle().ShouldBeEqual(_quadrantTitle[0], "Quadrant Header should be equal");
            var returnFilesCount = _batchSummary.GetReturnFilesCount(batchSeq);
            if (returnFilesCount <= 1)
            {
                _batchSummary.IsEmptyMessagePresentInReturnFiles()
                    .ShouldBeTrue("Data should not be displayed when return file count is 0");
                _batchSummary.GetEmptyMessagePresentInReturnFiles()
                    .ShouldBeEqual("No Data Available", "Verify Empty Message");
            }
        }


        #endregion

        #region PRIVATE METHODS

        private void SearchByBatchID(string batchID)
        {
            _batchSearch.SideBarPanelSearch.SelectDropDownListValueByLabel("Quick Search",
                BatchQuickSearchTypeEnum.AllBatches.GetStringValue());
            _batchSearch.SideBarPanelSearch.SetInputFieldByLabel("Batch ID", batchID);
            _batchSearch.SideBarPanelSearch.ClickOnFindButton();
            _batchSearch.WaitForWorkingAjaxMessage();
        }

        #endregion
    }
}

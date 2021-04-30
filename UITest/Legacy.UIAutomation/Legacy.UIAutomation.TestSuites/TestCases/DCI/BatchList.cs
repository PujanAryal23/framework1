using System;
using System.Collections.Generic;
using System.Linq;
using Legacy.Service.Data;
using Legacy.Service.PageServices.Product;
using Legacy.Service.Support.Enum;
using Legacy.Service.Support.Utils;
using NUnit.Framework;
using Legacy.UIAutomation.TestSuites.Base;
using System.Diagnostics;

namespace Legacy.UIAutomation.TestSuites.TestCases.DCI
{
    [Category("DCI")]
    public class BatchList : AutomatedBase
    {
        #region PRIVATE PROPERTIES

        private BatchListPage _batchList;
        private BatchStatisticsReportPage _batchStatisticsReport;
        private FlaggedClaimPage _flaggedClaim;

        #endregion

        #region PROTECTED PROPERTIES

        protected override string FullyQualifiedClassName
        {
            get
            {
                return GetType().FullName;
            }
        }

        protected override ProductEnum TestProduct
        {
            get
            {
                return ProductEnum.DCI;
            }

        }

        #endregion

        #region OVERRIDE METHODS

        protected override void FixtureSetUp()
        {
            base.FixtureSetUp();
            CurrentPage = _batchList = LoginPage.Login().GoToProductPage(ProductEnum.DCI).NavigateToBatchListPage();
        }

        protected override void TestInit()
        {
            base.TestInit();
            CurrentPage = _batchList;
        }

        protected override void TestCleanUp()
        {
            base.TestCleanUp();
            if (!CurrentPage.Equals(typeof(BatchListPage)))
            {
                CurrentPage.ClickOnBackButton();
                CurrentPage = _batchList;
                Console.WriteLine("Navigated back to Batch List Page");
            }
        }

        #endregion

        #region TEST SUITES

        [Test]
        public void Verify_that_click_on_stats_link_takes_to_Batch_Statistics_Report_page()
        {
            CurrentPage = _batchStatisticsReport = _batchList.ClickOnPageLink("1").ClickOnStatsLink();
            try
            {
                _batchStatisticsReport.CurrentPageTitle.ShouldEqual(_batchStatisticsReport.PageTitle, "PageTitle", "Page Title Mismatch Error");
            }
            catch (AssertionException ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Test]
        public void Verify_presence_of_different_tables_in_Batch_Statistics_Report_Page()
        {
            IList<IList<string>> expectedList = new List<IList<string>>()
                                                    {
                                                        DataHelper.GetMappingData(FullyQualifiedClassName, "Statistical Report Headers").Values.ToList(),
                                                        DataHelper.GetMappingData(FullyQualifiedClassName, "Batch Dates Headers").Values.ToList(),
                                                        DataHelper.GetMappingData(FullyQualifiedClassName, "Return Files Headers").Values.ToList(),
                                                        DataHelper.GetMappingData(FullyQualifiedClassName, "Processing History Headers").Values.ToList(),
                                                        DataHelper.GetMappingData(FullyQualifiedClassName, "Statistical Report Rows Title").Values.ToList(),
                                                        DataHelper.GetMappingData(FullyQualifiedClassName, "Batch Dates Rows Title").Values.ToList()
                                                    };

            IList<string> messageList = new List<string> { "Statistical Report Table Headers", "Batch Dates Table Headers", "Return Files Table Headers", "Processing History Table Headers", "Statistical Report Table Rows Title", "Batch Dates Table Rows Title" };

            CurrentPage = _batchStatisticsReport = _batchList.ClickOnStatsLink();
            try
            {
                for (int i = 0; i < 4; i++)
                {
                    int tableIndex = i + 1;
                    _batchStatisticsReport.GetHeadersOfTable(tableIndex.ToString()).ShouldCollectionEqual(expectedList[i], messageList[i]);
                    if (i < 2)
                    {
                        Console.WriteLine();
                        _batchStatisticsReport.GetRowTitleOfTable(tableIndex.ToString()).ShouldCollectionEqual(
                            expectedList[i + 4], messageList[i + 4]);
                    }
                    Console.WriteLine();
                }
            }
            catch (AssertionException ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Test]
        public void Verify_that_click_on_claim_status_link_navigate_to_claim_history_popup()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            IDictionary<string, string> param = DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
            string batchId = param["BatchId"];
            string claimSeq = param["ClaimSequence"];
            string pageNumber = param["PageNumber"];
            CurrentPage = _flaggedClaim = _batchList.ClickOnPageLink(pageNumber).ClickOnBatchIdLink(batchId);
            Console.WriteLine("Navigated to Flagged Claim Page");
            ClaimHistoryPage claimHistory = null;
            try
            {
                claimHistory = _flaggedClaim.ClickOnClaimStatusOfClaimSequence(claimSeq);
                claimHistory.CurrentPageTitle.ShouldEqual(claimHistory.PageTitle, "PageTitle", "Page Title Mismatch Error");
            }
            catch (AssertionException ex)
            {
                Assert.Fail(ex.Message);
            }
            finally
            {
                if (claimHistory != null)
                {
                    CurrentPage = claimHistory.CloseClaimHistoryPopup(string.Format(PageTitleEnum.FlaggedClaims.GetStringValue(), ProductEnum.DCI.GetStringValue()));
                }
            }
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Legacy.Service.PageServices.Product;
using Legacy.UIAutomation.TestSuites.Base;
using NUnit.Framework;
using Legacy.Service.Data;
using Legacy.Service.Support.Common.Constants;
using Legacy.Service.Support.Enum;

namespace Legacy.UIAutomation.TestSuites.TestCases.PCI
{
    [Category("PCI")]
    public class SearchCleared : AutomatedBase
    {
        #region PRIVATE PROPERTIES

        private SearchClearedPage _searchCleared;
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
                return ProductEnum.PCI;
            }

        }

        #endregion

        #region OVERRIDE METHODS

        protected override void FixtureSetUp()
        {
            base.FixtureSetUp();
            _searchCleared = LoginPage.Login().GoToProductPage(ProductEnum.PCI).NavigateToSearchClearedPage();
        }

        protected override void TestInit()
        {
            base.TestInit();
            CurrentPage = _searchCleared;
        }

        #endregion

        #region TEST SUITES

        [Test]
        public void Search_by_a_given_batch_id_and_verify_the_release_date_column_matches_the_batch_id()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            IDictionary<string, string> param = DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
            string batchId = param["BatchId"];
            var expectedDate = Convert.ToDateTime(param["ReleaseDate"]);
            StringFormatter.PrintMessageTitle("Search Operation For Release Date");
            _searchCleared.SearchByBatchId(batchId);
            _searchCleared.ClickAscendingArrowOfReleaseDateColumn();
            _searchCleared.GetReleaseDateOfFirstRow().ShouldEqual(expectedDate, "Release Date", "Mismatch Release Date");
            StringFormatter.PrintLineBreak();
        }

        #endregion
    }
}

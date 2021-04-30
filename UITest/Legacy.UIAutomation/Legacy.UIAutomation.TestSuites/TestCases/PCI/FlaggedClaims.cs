using System;
using System.Collections.Generic;
using System.Diagnostics;
using Legacy.Service.Data;
using Legacy.Service.PageServices.Product;
using Legacy.Service.Support.Utils;
using Legacy.UIAutomation.TestSuites.Base;
using NUnit.Framework;
using Legacy.Service.Support.Enum;

namespace Legacy.UIAutomation.TestSuites.TestCases.PCI
{
    [Category("PCI")]
    public class FlaggedClaims : AutomatedBase
    {
        #region PRIVATE PROPERTIES

        private BatchListPage _batchList;
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
                return ProductEnum.PCI;
            }

        }

        #endregion

        #region OVERRIDE METHODS

        protected override void FixtureSetUp()
        {
            base.FixtureSetUp();
            CurrentPage = _batchList = LoginPage.Login().GoToPhysicianClaimInsight().NavigateToBatchListPage();
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
                CurrentPage.NavigateToBackPage();
                CurrentPage = _batchList;
                Console.WriteLine("Navigated back to Batch List Page");
            }
        }

        #endregion

        #region TEST SUITES

        [Test]
        public void Verify_clicked_on_ascending_and_descending_arrow_sort_Claim_Sequence()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            IDictionary<string, string> param = DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
            string batchId = param["BatchId"];
            CurrentPage = _flaggedClaim = _batchList.ClickOnBatchIdLink(batchId);
            Console.WriteLine("Navigated to Flagged Claim Page");
            
            _flaggedClaim.ClickClaimNoDescendingArrow();
            var claimNumbersDescending = _flaggedClaim.GetColumnValue(2);
            claimNumbersDescending.IsInDescendingOrder().ShouldBeTrue("Claim numbers are in descending order");
            
            _flaggedClaim.ClickClaimNoAscendingArrow();
            var claimNumbersAscending = _flaggedClaim.GetColumnValue(2);
            claimNumbersAscending.IsInAscendingOrder().ShouldBeTrue("Claim numbers are in ascending order");
        }

        #endregion
    }
}

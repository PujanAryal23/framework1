using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Legacy.Service.Data;
using Legacy.Service.PageServices.Product;
using Legacy.Service.Support.Common.Constants;
using Legacy.Service.Support.Enum;
using Legacy.Service.Support.Utils;
using Legacy.UIAutomation.TestSuites.Base;
using NUnit.Framework;

namespace Legacy.UIAutomation.TestSuites.TestCases.DCI
{
    [Category("DCI")]
    public class DocClaimList : AutomatedBase
    {
        #region PRIVATE PROPERTIES

        private DocClaimListPage _docClaimList;

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
            ProductPage = LoginPage.Login().GoToProductPage(TestProduct);
        }

        protected override void TestInit()
        {
            base.TestInit();
            CurrentPage = ProductPage;
        }

        protected override void TestCleanUp()
        {
            if (CurrentPage.Equals(typeof(DocClaimListPage)))
                ProductPage = _docClaimList.ClickOnBack(TestProduct);
            base.TestCleanUp();
        }

        #endregion

        #region TEST SUITES

        [Test]
        public void Verify_ClaimSummary_page_open_when_clicked_on_claimSequence_of_Documents_Required_page()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            ClaimSummaryPage claimSummary = null;
            string claimSeq = DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName, "ClaimSequence");
            CurrentPage = _docClaimList = ProductPage.NavigateToDocClaimListPage(DocumentTypeEnum.DocRequired);
            try
            {
                CurrentPage = claimSummary = _docClaimList.ClickClaimSequence(claimSeq);
                claimSummary
                    .CurrentPageTitle
                    .ShouldEqual(claimSummary.PageTitle, "Page Title", "Page Title Mismatch");
                string pageUrl = string.Concat(claimSummary.BaseUrl, ProductPageUrlEnum.ClaimSummaryInfo.GetStringValue());
                claimSummary.CurrentPageUrl.Contains(pageUrl).ShouldBeTrue(string.Format("Page Url '{0}' Contains '{1}'", claimSummary.CurrentPageUrl, pageUrl));
            }
            finally
            {
                if(claimSummary != null)
                    CurrentPage = _docClaimList = claimSummary.GoBackToDocClaimList();
            }
        }

        [Test]
        public void Verify_ClaimSummary_page_open_when_clicked_on_claimSequence_of_Documents_Requested_page()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            ClaimSummaryPage claimSummary = null;
            string claimSeq = DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName, "ClaimSequence");
            CurrentPage = _docClaimList = ProductPage.NavigateToDocClaimListPage(DocumentTypeEnum.DocRequested);
            try
            {
                CurrentPage = claimSummary = _docClaimList.ClickClaimSequence(claimSeq);
                claimSummary
                    .CurrentPageTitle
                    .ShouldEqual(claimSummary.PageTitle, "Page Title", "Page Title Mismatch");
                string pageUrl = string.Concat(claimSummary.BaseUrl, ProductPageUrlEnum.ClaimSummaryInfo.GetStringValue());
                claimSummary.CurrentPageUrl.Contains(pageUrl).ShouldBeTrue(string.Format("Page Url '{0}' Contains '{1}'", claimSummary.CurrentPageUrl, pageUrl));
            }
            finally
            {
                if (claimSummary != null)
                    CurrentPage = _docClaimList = claimSummary.GoBackToDocClaimList();
            }
        }

        [Test]
        public void Verify_ClaimSummary_page_open_when_clicked_on_claimSequence_of_Documents_Received_page()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            ClaimSummaryPage claimSummary = null;
            string claimSeq = DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName, "ClaimSequence");
            CurrentPage = _docClaimList = ProductPage.NavigateToDocClaimListPage(DocumentTypeEnum.DocReceived);
            try
            {
                CurrentPage = claimSummary = _docClaimList.ClickClaimSequence(claimSeq);
                claimSummary
                    .CurrentPageTitle
                    .ShouldEqual(claimSummary.PageTitle, "Page Title", "Page Title Mismatch");
                string pageUrl = string.Concat(claimSummary.BaseUrl, ProductPageUrlEnum.ClaimSummaryInfo.GetStringValue());
                claimSummary.CurrentPageUrl.Contains(pageUrl).ShouldBeTrue(string.Format("Page Url '{0}' Contains '{1}'", claimSummary.CurrentPageUrl, pageUrl));
            }
            finally
            {
                if (claimSummary != null)
                    CurrentPage = _docClaimList = claimSummary.GoBackToDocClaimList();
            }
        }


        #endregion
    }
}

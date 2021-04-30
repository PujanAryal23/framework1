using System.Diagnostics;
using Legacy.UIAutomation.TestSuites.Base;
using Legacy.Service.PageServices.Product;
using NUnit.Framework;
using Legacy.Service.Support.Enum;
using Legacy.Service.Support.Common.Constants;

namespace Legacy.UIAutomation.TestSuites.TestCases.PCI
{
    [Category("PCI")]
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
                return ProductEnum.PCI;
            }

        }

        #endregion

        #region OVERRIDE METHODS

        protected override void FixtureSetUp()
        {
            base.FixtureSetUp();
            ProductPage = LoginPage.Login().GoToProductPage(ProductEnum.PCI);
        }

        protected override void TestInit()
        {
            base.TestInit();
            CurrentPage = ProductPage;
        }

        protected override void TestCleanUp()
        {
            if (CurrentPage.Equals(typeof(DocClaimListPage)))
                ProductPage = _docClaimList.ClickOnBack(ProductEnum.PCI);
            base.TestCleanUp();
        }

        #endregion

        #region TEST SUITES

        [Test]
        public void Verify_Documents_Required_page_open_when_clicked_on_Docs_Required_button_And_verify_click_on_back_button_takes_to_product_page()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            const DocumentTypeEnum docType = DocumentTypeEnum.DocRequired;
            NavigateToAndVerifyDocumentsAndProductPagesOpen(docType);
        }

        [Test]
        public void Verify_Documents_Requested_page_open_when_clicked_on_Docs_Requested_button_And_verify_click_on_back_button_takes_to_product_page()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            const DocumentTypeEnum docType = DocumentTypeEnum.DocRequested;
            NavigateToAndVerifyDocumentsAndProductPagesOpen(docType);
        }

        [Test]
        public void Verify_Documents_DocReceived_page_open_when_clicked_on_Docs_DocReceived_button_And_verify_click_on_back_button_takes_to_product_page()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            const DocumentTypeEnum docType = DocumentTypeEnum.DocReceived;
            NavigateToAndVerifyDocumentsAndProductPagesOpen(docType);
        }


        #endregion

        #region PRIVATE METHODS

        /// <summary>
        /// Navigate to Documents Claim List Page and verify the respective page opens.
        /// Click on back button and verify Product page opens
        /// </summary>
        /// <param name="docType">A type of documents claim list page.</param>
        private void NavigateToAndVerifyDocumentsAndProductPagesOpen(DocumentTypeEnum docType)
        {
            StringFormatter.PrintMessageTitle("Documents Page");
            CurrentPage = _docClaimList = ProductPage.NavigateToDocClaimListPage(docType);
            _docClaimList.CurrentPageTitle.ShouldEqual(_docClaimList.PageTitle, "Page Title ");
            _docClaimList.CurrentPageUrl.AssertIsContained(_docClaimList.PageUrl, "Page Url ");
            StringFormatter.PrintLineBreak();
            StringFormatter.PrintMessageTitle("Product Page");
            CurrentPage = ProductPage = _docClaimList.ClickOnBack(ProductEnum.PCI);
            ProductPage.CurrentPageTitle.ShouldEqual(ProductPage.PageTitle, "Page Title ");
            ProductPage.CurrentPageUrl.AssertIsContained(ProductPage.PageUrl, "Page Url ");
            StringFormatter.PrintLineBreak();
        }

        #endregion
    }
}

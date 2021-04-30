using System;
using System.Collections.Generic;
using System.Diagnostics;
using Legacy.Service.Data;
using Legacy.Service.PageServices.Common;
using Legacy.Service.Support.Enum;
using Legacy.Service.Support.Utils;
using Legacy.UIAutomation.TestSuites.Base;
using Legacy.Service.PageServices.Product;
using NUnit.Framework;

namespace Legacy.UIAutomation.TestSuites.TestCases.PCI
{
    [Category("PCI")]
    public class SearchPended : AutomatedBase
    {
        #region PRIVATE PROPERTIES

        private SearchPendedPage _searchPended;

        private string _calendarPopupHandle = null;
       

        private bool _hitSearchButton;

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
            try
            {

                base.TestInit();
                CurrentPage = _searchPended = ProductPage.NavigateToSearchPendedPage().GetWindowHandle(out OriginalWindowHandle);
            }
            catch (Exception ex)
            {
            }
        }

        protected override void TestCleanUp()
        {
            if (CurrentPage.Equals(typeof(SearchPendedPage)))
                ProductPage = _searchPended.ClickOnBack(ProductEnum.PCI);
            base.TestCleanUp();
        }

        #endregion

        #region TEST SUITES

        [Test]
        public void Verify_clicking_on_back_button_navigate_to_product_page()
        {
            var product = ProductEnum.PCI.GetStringValue();
            CurrentPage = ProductPage = _searchPended.ClickOnBack(ProductEnum.PCI);
            ProductPage
                .CurrentPageTitle
                .ShouldEqual(product, string.Format("Navigated to {0}", product), string.Format("Unable to navigate {0}.", product));
        }

       

        #endregion

        #region PRIVATE METHODS

       

        #endregion
    }
}

using System;
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
    public class LogicRequests : AutomatedBase
    {
        #region PRIVATE PROPERTIES

        private LogicRequestsPage _logicRequests;

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
            CurrentPage = _logicRequests = ProductPage.NavigateToLogicRequestsPage().GetWindowHandle(out OriginalWindowHandle);
        }

        protected override void TestCleanUp()
        {
            if (CurrentPage.Equals(typeof(LogicRequestsPage)))
                ProductPage = _logicRequests.ClickOnBack(ProductEnum.PCI);
            base.TestCleanUp();
        }

        #endregion

        #region TEST SUITES

        [Test]
        public void Verify_can_navigate_To_logic_requests_page()
        {
            _logicRequests
                .CurrentPageTitle
                .ShouldEqual(
                string.Format(PageTitleEnum.LogicRequests.GetStringValue(), ProductEnum.PCI.GetStringValue()),
                "PageTitle", "Page Title Mismatch Error");
        }

        [Test]
        public void Verify_clicking_on_back_button_navigate_to_product_page()
        {
            var product = ProductEnum.PCI.GetStringValue();
            CurrentPage = ProductPage = _logicRequests.ClickOnBack(ProductEnum.PCI);
            ProductPage
                .CurrentPageTitle
                .ShouldEqual(product, string.Format("Navigated to {0}", product), string.Format("Unable to navigate {0}.", product));
        }

        [Test]
        public void Verify_Notify_Client_popup_appears_when_Notify_Client_button_clicked_and_closed_when_Close_button_is_clicked()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            bool isNotifyClientPopupPresent = true;
            string notifyClientPopupHandle = null;
            NotifyClientPage notifyClient = null;
            try
            {
                notifyClient = _logicRequests.NavigateToNotifyClient();
                notifyClient
                    .GetWindowHandle(out notifyClientPopupHandle)
                    .CurrentPageTitle
                    .ShouldEqual(PageTitleEnum.NotifyClient.GetStringValue(), "Page Title");

                notifyClient
                    .CurrentPageUrl
                    .ShouldEqual(string.Concat(notifyClient.BaseUrl, ProductPageUrlEnum.NotifyClient.GetStringValue()), "Page Url");

                _logicRequests = notifyClient.ClickCloseButton<LogicRequestsPage>();
                isNotifyClientPopupPresent = _logicRequests.IsPopupPresentWithHandleName(notifyClientPopupHandle);
                isNotifyClientPopupPresent.ShouldBeFalse("Notify Client Popup Present");
            }
            catch (Exception exception)
            {
                this.AssertFail(exception.Message);
            }
            finally
            {
                if (notifyClient != null && isNotifyClientPopupPresent)
                    _logicRequests.ClosePopupAndSwitchToOriginalHandle(notifyClientPopupHandle,OriginalWindowHandle);
            }
        }

      
        #endregion


    }
}

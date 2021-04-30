using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Legacy.Service.Data;
using Legacy.Service.PageServices.Common;
using Legacy.Service.PageServices.Product;
using Legacy.Service.Support.Enum;
using Legacy.Service.Support.Utils;
using Legacy.UIAutomation.TestSuites.Base;
using NUnit.Framework;

namespace Legacy.UIAutomation.TestSuites.TestCases.DCI
{
    [Category("DCI")]
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
            CurrentPage = _logicRequests = ProductPage.NavigateToLogicRequestsPage().GetWindowHandle(out OriginalWindowHandle);
        }

        protected override void TestCleanUp()
        {
            if (CurrentPage.Equals(typeof(LogicRequestsPage)))
                ProductPage = _logicRequests.ClickOnBack(TestProduct);
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
                string.Format(PageTitleEnum.LogicRequests.GetStringValue(), TestProduct.GetStringValue()),
                "PageTitle", "Page Title Mismatch Error");
            _logicRequests.CurrentPageUrl
                .Contains(_logicRequests.PageUrl)
                .ShouldBeTrue(
                string.Format("Page Url '{0}' contains '{1}'", _logicRequests.CurrentPageUrl, _logicRequests.PageUrl));
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
                    _logicRequests.ClosePopupAndSwitchToOriginalHandle(notifyClientPopupHandle, OriginalWindowHandle);
            }
        }

        [Test]
        public void Verify_that_choosing_a_date_from_calendar_enters_the_date_in_the_textfield_client_receive_date()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            var date = DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
            DateTime expectedClientBeginDate = DateTime.Parse(date["ClientReceiveBeginDate"]);
            DateTime expectedClientEndDate = DateTime.Parse(date["ClientReceiveEndDate"]);
            DateTime actualDate;
            
                /* ------------------------------------------ *Client Receive Begin Date* ----------------------------------------------- */
                _logicRequests.ClickOnClientReceiveDate();
                _logicRequests.ClickOnDate(expectedClientBeginDate);
                _logicRequests.GetClientReceiveDate(out actualDate); 
                actualDate.ToShortDateString().ShouldEqual(expectedClientBeginDate.ToShortDateString(), "Client Receive Begin Date ");
                /* ----------------------------------------- --------------------------------------- ------------------------------------ */

                /* ------------------------------------------- *Client Receive End Date* ------------------------------------------------ */
                _logicRequests.ClickOnClientReceiveDate(false);
                _logicRequests.ClickOnDate(expectedClientEndDate);
                _logicRequests.GetClientReceiveDate(out actualDate,false); 
                actualDate.ToShortDateString().ShouldEqual(expectedClientEndDate.ToShortDateString(), "Client Receive End Date ");
                /* ----------------------------------------- --------------------------------------- ------------------------------------ */
           
        }
       
        [Test]
        public void Verify_that_choosing_a_date_from_calendar_enters_the_date_in_the_textfield_logic_date()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            var date = DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
            DateTime expectedLogicBeginDate = DateTime.Parse(date["LogicBeginDate"]);
            DateTime expectedLogicEndDate = DateTime.Parse(date["LogicEndDate"]);
            DateTime actualDate;
            
                /* ----------------------------------------------- *Logic Begin Date* --------------------------------------------------- */
                _logicRequests.ClickOnLogicDate();
                _logicRequests.ClickOnDate(expectedLogicBeginDate);
                _logicRequests.GetLogicDate(out actualDate);
                actualDate.ToShortDateString().ShouldEqual(expectedLogicBeginDate.ToShortDateString(), "Logic Begin Date ");
                /* ----------------------------------------- --------------------------------------- ------------------------------------ */

                /* ------------------------------------------------ *Logic End Date* ---------------------------------------------------- */
                 _logicRequests.ClickOnLogicDate(false);
                _logicRequests.ClickOnDate(expectedLogicEndDate);
                _logicRequests.GetLogicDate(out actualDate, false);
                actualDate.ToShortDateString().ShouldEqual(expectedLogicEndDate.ToShortDateString(), "Logic End Date ");
                /* ----------------------------------------- --------------------------------------- ------------------------------------ */
           
        }

        #endregion


    }
}

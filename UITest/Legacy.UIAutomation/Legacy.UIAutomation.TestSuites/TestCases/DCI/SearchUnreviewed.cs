using System;
using System.Collections.Generic;
using System.Diagnostics;
using Legacy.Service.Data;
using Legacy.Service.PageServices.Common;
using Legacy.Service.PageServices.Product;
using Legacy.Service.Support.Utils;
using Legacy.UIAutomation.TestSuites.Base;
using NUnit.Framework;
using Legacy.Service.Support.Enum;

namespace Legacy.UIAutomation.TestSuites.TestCases.DCI
{
    [Category("DCI")]
    public class SearchUnreviewed : AutomatedBase
    {
        #region PRIVATE PROPERTIES

        private SearchUnreviewedPage _searchUnreviewed;

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
                return ProductEnum.DCI;
            }

        }

        #endregion

        #region OVERRIDE METHODS

        protected override void FixtureSetUp()
        {
            base.FixtureSetUp();
            ProductPage = LoginPage.Login().GoToProductPage(ProductEnum.DCI);
        }

        protected override void TestInit()
        {
            base.TestInit();
            CurrentPage = _searchUnreviewed = ProductPage.NavigateToSearchUnreviewedPage().GetWindowHandle(out OriginalWindowHandle);
        }

        protected override void TestCleanUp()
        {
            if (CurrentPage.Equals(typeof(SearchUnreviewedPage)))
                ProductPage = _searchUnreviewed.ClickOnBack(ProductEnum.DCI);
            base.TestCleanUp();
        }

        #endregion

        #region TEST SUITES

        [Test]
        public void Verify_clicking_on_back_button_navigate_to_product_page()
        {
            var product = ProductEnum.DCI_Full_Form.GetStringValue();
            CurrentPage = ProductPage = _searchUnreviewed.ClickOnBack(ProductEnum.DCI);
            ProductPage
                .CurrentPageTitle
                .ShouldEqual(product, string.Format("Navigated to {0}", product), string.Format("Unable to navigate {0}.", product));
        }

        [Test]
        public void Verify_that_choosing_a_date_from_calendar_enters_the_date_in_the_textfield_from_and_to_client_received_date_and_also_verify_search_results()
        {
         
           
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            var date = DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
            DateTime expectedFromClientReceivedDate = DateTime.Parse(date["FromClientReceivedDate"]);
            DateTime expectedToClientReceivedDate = DateTime.Parse(date["ToClientReceivedDate"]);


            try
            {
                /* ------------------------------------------------ *From Client Received Date* ------------------------------------------------------- */
                ClickOnCalAndGetDateFromTextField(expectedFromClientReceivedDate)
                .ToShortDateString().ShouldEqual(expectedFromClientReceivedDate.ToShortDateString(), "From Client Recieved Date");
                /* ----------------------------------------- --------------------------------------- ------------------------------------ */

                /* ------------------------------------------------ *To Client Received Date* ------------------------------------------------------- */
                ClickOnCalAndGetDateFromTextField(expectedToClientReceivedDate, false)
                .ToShortDateString().ShouldEqual(expectedToClientReceivedDate.ToShortDateString(), "To Client Recieved Date");
                /* ----------------------------------------- --------------------------------------- ------------------------------------ */

                _searchUnreviewed = _searchUnreviewed.ClickSearchButon(out _hitSearchButton);
                _searchUnreviewed.IsNomatchingRecordsFound().ShouldBeFalse("-- No matching records were found --");

                IList<DateTime> actualSearchResults = _searchUnreviewed.GetRecordsList(8, 19);

                foreach (var searchResult in actualSearchResults)
                    searchResult.AssertDateRange(expectedFromClientReceivedDate, expectedToClientReceivedDate, "Client Received Date ");

            }
            catch (Exception exception)
            {
                this.AssertFail(exception.Message);
            }
            finally
            {
               
                if (_hitSearchButton)
                {
                    _searchUnreviewed = (SearchUnreviewedPage)_searchUnreviewed.GoBack();
                    _hitSearchButton = false;
                }
            }
        }

        #endregion

        #region PRIVATE METHODS

        private DateTime ClickOnCalAndGetDateFromTextField(DateTime expectedClientReceivedDate, bool iSFrom = true)
        {
            DateTime actualDate;
            _searchUnreviewed.ClickOnDateCal(iSFrom);
            _searchUnreviewed.ClickOnDate(expectedClientReceivedDate);
            _searchUnreviewed.GetClienRecvDate(out actualDate,iSFrom);
            Console.Out.WriteLine("Selected a Date : {0}",actualDate.ToShortDateString());

            return actualDate;
        }

       

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Legacy.Service.Data;
using Legacy.Service.PageServices.Common;
using Legacy.Service.PageServices.Product;
using Legacy.Service.Support.Enum;
using Legacy.UIAutomation.TestSuites.Base;
using NUnit.Framework;

namespace Legacy.UIAutomation.TestSuites.TestCases.DCI
{
    [Category("DCI")]
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
                ProductPage = _searchPended.ClickOnBack(TestProduct);
            base.TestCleanUp();
        }

        #endregion

        #region TEST SUITES

        [Test]
        public void Verify_that_choosing_a_date_from_calendar_enters_the_date_in_the_textfield_client_received_date_and_also_verify_search_results()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            var date = DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
            DateTime expectedClientReceivedDateBegin = DateTime.Parse(date["ClientReceivedDateBegin"]);
            DateTime expectedClientReceivedDateEnd = DateTime.Parse(date["ClientReceivedDateEnd"]);
            var expectedData = Convert.ToInt32(date["ClientReceivedDateData"]);

            try
            {
                ClickOnCalAndGetDateFromTextField(expectedClientReceivedDateBegin, true, true).ToShortDateString().ShouldEqual(expectedClientReceivedDateBegin.ToShortDateString(), "Client Recieved Date Begin");
                ClickOnCalAndGetDateFromTextField(expectedClientReceivedDateEnd, false, true).ToShortDateString().ShouldEqual(expectedClientReceivedDateEnd.ToShortDateString(), "From Client Recieved Date End");

                _searchPended = _searchPended.ClickSearchButon(out _hitSearchButton);
                switch (expectedData)
                {
                    case 0:
                        _searchPended.IsNomatchingRecordsFound().ShouldBeTrue("-- No matching records were found --");
                        break;

                    case 1: 
                    case 2:
                         IList<DateTime> actualClientRecvDate = 
                         _searchPended.GetRecordsList(6, 19);
                        foreach (var searchResult in actualClientRecvDate)
                            searchResult.AssertDateRange(expectedClientReceivedDateBegin, expectedClientReceivedDateEnd, "Client Received Date ");
                        break;
                }
            }
            catch (Exception exception)
            {
                this.AssertFail(exception.Message);
            }
            finally
            {
               
                if (_hitSearchButton)
                {
                    _searchPended = (SearchPendedPage)_searchPended.GoBack();
                    _hitSearchButton = false;
                }

            }
        }

        [Test]
        public void Verify_that_choosing_a_date_from_calendar_enters_the_date_in_the_textfield_pended_date_and_also_verify_search_results()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            var date = DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
            DateTime expectedPendedDateBegin = DateTime.Parse(date["PendedDateBegin"]);
            DateTime expectedPendedDateEnd = DateTime.Parse(date["PendedDateEnd"]);
            var expectedData = Convert.ToInt32(date["PendedDateData"]);

            try
            {
                ClickOnCalAndGetDateFromTextField(expectedPendedDateBegin, true, false).ToShortDateString().ShouldEqual(expectedPendedDateBegin.ToShortDateString(), "Pended Date Begin");
                ClickOnCalAndGetDateFromTextField(expectedPendedDateEnd, false, false).ToShortDateString().ShouldEqual(expectedPendedDateEnd.ToShortDateString(), "Pended Date End");

                _searchPended = _searchPended.ClickSearchButon(out _hitSearchButton);

                switch (expectedData)
                {
                    case 0:
                        _searchPended.IsNomatchingRecordsFound().ShouldBeTrue("-- No matching records were found --");
                        break;

                    case 1:
                    case 2:
                    IList<DateTime> actualPendedDate = _searchPended.GetRecordsList(7, 19);
                    foreach (var searchResult in actualPendedDate)
                        searchResult.AssertDateRange(expectedPendedDateBegin, expectedPendedDateEnd, "Pended Date ");
                    break;
                }
            }
            catch (Exception exception)
            {
                this.AssertFail(exception.Message);
            }
            finally
            {
                
                if (_hitSearchButton)
                {
                    _searchPended = (SearchPendedPage)_searchPended.GoBack();
                    _hitSearchButton = false;
                }

            }
        }

        #endregion

        #region PRIVATE METHODS

        private DateTime ClickOnCalAndGetDateFromTextField(DateTime expectedDate, bool iSBegin, bool isClientRecvDate)
        {
            DateTime actualDate;

            if (isClientRecvDate)
                _searchPended.ClickOnClientRecvCal(iSBegin);
            else
            _searchPended.ClickOnPendedCal(iSBegin);
            _searchPended.ClickOnDate(expectedDate);
           
           
            _searchPended = isClientRecvDate
                                ? _searchPended.GetClienRecvDate(out actualDate, iSBegin)
                                : _searchPended.GetPendedDate(out actualDate, iSBegin);
            Console.Out.WriteLine("Selected a Date : {0}", actualDate.ToShortDateString());

            return actualDate;
        }

      

        #endregion
    }
}

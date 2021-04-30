using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Legacy.Service.Data;
using Legacy.Service.PageServices.Common;
using Legacy.Service.PageServices.Product;
using Legacy.Service.Support.Enum;
using NUnit.Framework;
using Legacy.UIAutomation.TestSuites.Base;

namespace Legacy.UIAutomation.TestSuites.TestCases.DCI
{
    [Category("DCI")]
    public class ModifiedEdits : AutomatedBase
    {
        #region PRIVATE PROPERTIES

        private ModifiedEditsPage _modifiedEdits;

        private string _calendarPopupHandle = null;
       

        private bool _hitSearchButton;

        #endregion

        #region PROTECTED PROPERTIES

        protected override string FullyQualifiedClassName
        {
            get { return GetType().FullName; }
        }

        protected override ProductEnum TestProduct
        {
            get { return ProductEnum.DCI; }

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
            CurrentPage =
                _modifiedEdits = ProductPage.NavigateToModifiedEditsPage().GetWindowHandle(out OriginalWindowHandle);
        }

        protected override void TestCleanUp()
        {
            if (CurrentPage.Equals(typeof (ModifiedEditsPage)))
                ProductPage = _modifiedEdits.ClickOnBack(TestProduct);
            base.TestCleanUp();
        }

        #endregion

        #region TEST SUITES

        [Test]
        public void Verify_that_choosing_a_date_from_calendar_enters_the_date_in_the_textfield_modified_date_and_also_verify_search_results()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            var date = DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
            DateTime expectedModBegDate = DateTime.Parse(date["ModBegDate"]);
            DateTime expectedModEndDate = DateTime.Parse(date["ModEndDate"]);
            var dateDataExpected = Convert.ToInt32(date["ModDateData"]);

            try
            {
                ClickOnCalAndGetDateFromTextField(expectedModBegDate).ToShortDateString().ShouldEqual(
                    expectedModBegDate.ToShortDateString(), "Modified Edits Date Begin");
                ClickOnCalAndGetDateFromTextField(expectedModEndDate, false).ToShortDateString().ShouldEqual(
                    expectedModEndDate.ToShortDateString(), "Modified Edits Date End");

                _modifiedEdits = _modifiedEdits.ClickSearchButon(out _hitSearchButton);
                switch (dateDataExpected)
                {
                    case 0:
                        _modifiedEdits.IsNomatchingRecordsFound().ShouldBeTrue("-- No matching records were found --");
                        break;

                    case 1:
                    case 2:
                        IList<DateTime> actualSearchResults;
                        _modifiedEdits.GetRecordsList(out actualSearchResults, 11, 2);
                        foreach (var searchResult in actualSearchResults)
                            searchResult.AssertDateRange(expectedModBegDate, expectedModEndDate, "Modified Edits Date ");
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
                    _modifiedEdits = (ModifiedEditsPage) _modifiedEdits.GoBack();
                    _hitSearchButton = false;
                }

            }
        }

        #endregion

        #region PRIVATE METHODS

        private DateTime ClickOnCalAndGetDateFromTextField(DateTime expectedModEditsDate, bool iSFrom = true)
        {
            DateTime actualDate;
             _modifiedEdits.ClickOnDateCal(iSFrom);
            
            _modifiedEdits.ClickOnDate(expectedModEditsDate);
            _modifiedEdits.GetModEditDate(
                    out actualDate, iSFrom);
            Console.Out.WriteLine("Selected a Date : {0}", actualDate.ToShortDateString());

            return actualDate;
        }

       
        #endregion
    }
}

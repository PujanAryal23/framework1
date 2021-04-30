using System;
using System.Diagnostics;
using Legacy.Service.Data;
using Legacy.Service.PageServices.Common;
using Legacy.UIAutomation.TestSuites.Base;
using Legacy.Service.PageServices.Product;
using NUnit.Framework;
using Legacy.Service.Support.Enum;

namespace Legacy.UIAutomation.TestSuites.TestCases.PCI
{
    [Category("PCI")]
    public class SearchProduct : AutomatedBase
    {
        #region PRIVATE PROPERTIES

        private SearchProductPage _searchProduct;

        private string _calendarPopupHandle = null;
      

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
            CurrentPage = _searchProduct = ProductPage.NavigateToSearchProductPage()
                                               .GetWindowHandle(out OriginalWindowHandle);
        }

        protected override void TestCleanUp()
        {
            if (CurrentPage.Equals(typeof(SearchProductPage)))
                ProductPage = _searchProduct.ClickOnBack(ProductEnum.PCI);
            OriginalWindowHandle = null;
            base.TestCleanUp();
        }

        #endregion

        #region TEST SUITES

        [Test]
        public void Verify_that_choosing_a_date_from_calendar_enters_the_date_in_the_textfield_patient_dob()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            var date = DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
            DateTime expectedPatientDob = DateTime.Parse(date["PatientDOB"]);

            try
            {
                /* ------------------------------------------------ *Patient DOB* ------------------------------------------------------- */
                ClickOnCalAndGetDateFromTextField(expectedPatientDob, false, true).ToShortDateString().ShouldEqual(expectedPatientDob.ToShortDateString(), "Patient DOB ");
                /* ----------------------------------------- --------------------------------------- ------------------------------------ */

            }
            catch (Exception exception)
            {
                this.AssertFail(exception.Message);
            }
           
        }

        [Test]
        public void Verify_that_choosing_a_date_from_calendar_enters_the_date_in_the_textfield_begin_and_end_dos()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            var date = DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
            DateTime expectedBeginDos = DateTime.Parse(date["BeginDOS"]);
            DateTime expectedEndDos = DateTime.Parse(date["EndDOS"]);

            try
            {
                /* ------------------------------------------------ *Begin DOS* ------------------------------------------------------- */

                ClickOnCalAndGetDateFromTextField(expectedBeginDos, true, false).ToShortDateString().ShouldEqual(expectedBeginDos.ToShortDateString(), "Begin DOS");


                /* ----------------------------------------- --------------------------------------- ------------------------------------ */

                /* ------------------------------------------------ *End DOS* ------------------------------------------------------- */
                ClickOnCalAndGetDateFromTextField(expectedEndDos, false, false).ToShortDateString().ShouldEqual(expectedEndDos.ToShortDateString(), "End DOS");
                /* ----------------------------------------- --------------------------------------- ------------------------------------ */

            }
            catch (Exception exception)
            {
                this.AssertFail(exception.Message);
            }
           
        }
        #endregion

        #region PRIVATE METHODS

        private DateTime ClickOnCalAndGetDateFromTextField(DateTime expectedDate, bool iSBegin, bool isPatientDob)
        {
            DateTime actualDate;

            if (isPatientDob) 
                _searchProduct.ClickOnPatientDobCal();
            
            else _searchProduct.ClickOnDosCal(iSBegin);

           
            _searchProduct.ClickOnDate(expectedDate);
            _searchProduct = isPatientDob
                                ? _searchProduct.GetPatientDob(out actualDate)
                                : _searchProduct.GetDos(out actualDate, iSBegin);
            Console.Out.WriteLine("Selected a Date : {0}", actualDate.ToShortDateString());

            return actualDate;
        }

      

        #endregion
    }
}

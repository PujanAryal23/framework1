using System;
using System.Collections.Generic;
using System.Diagnostics;
using Legacy.Service.Data;
using Legacy.Service.PageServices.Common;
using Legacy.Service.PageServices.Product;
using Legacy.Service.Support.Enum;
using Legacy.UIAutomation.TestSuites.Base;
using NUnit.Framework;

namespace Legacy.UIAutomation.TestSuites.TestCases.PCI
{
    [Category("PCI")]
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
            //ProductPage = LoginPage.Login().GoToProductPage(ProductEnum.PCI);
        }

        protected override void TestInit()
        {
            base.TestInit();
            CurrentPage = _modifiedEdits = ProductPage.NavigateToModifiedEditsPage().GetWindowHandle(out OriginalWindowHandle);
        }

        protected override void TestCleanUp()
        {
            if (CurrentPage.Equals(typeof(ModifiedEditsPage)))
                ProductPage = _modifiedEdits.ClickOnBack(ProductEnum.PCI);
            base.TestCleanUp();
        }

        #endregion

        #region TEST SUITES

        

        #endregion

        #region PRIVATE METHODS

        private DateTime ClickOnCalAndGetDateFromTextField(DateTime expectedModEditsDate, bool iSFrom = true)
        {
            DateTime actualDate;
           _modifiedEdits.ClickOnDateCal(iSFrom);
           
            _modifiedEdits.GetModEditDate(out actualDate, iSFrom);
            Console.Out.WriteLine("Selected a Date : {0}", actualDate.ToShortDateString());

            return actualDate;
        }

       

        #endregion
    }
}

using System;
using Legacy.Service.PageServices.Product;
using Legacy.Service.Support.Enum;
using Legacy.UIAutomation.TestSuites.Base;
using NUnit.Framework;

namespace Legacy.UIAutomation.TestSuites.TestCases.PCI
{
    [Category("PCI")]
    public class PhysicianClaimInsight : AutomatedBase
    {
        #region PRIVATE PROPERTIES

        private PhysicianClaimInsightPage _pciFlow;
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
            //CurrentPage = _pciFlow = LoginPage.Login().GoToPhysicianClaimInsight();
            //Console.WriteLine("Navigated to Physician Claim Insight");
        }

        protected override void TestInit()
        {
            base.TestInit();
            CurrentPage = _pciFlow;
        }

        protected override void TestCleanUp()
        {
            base.TestCleanUp();
            if (!CurrentPage.Equals(typeof(PhysicianClaimInsightPage)))
            {
                CurrentPage = _pciFlow = CurrentPage.NavigateToPhysicianClaimInsightPage();
                Console.WriteLine("Navigated back to Physician Claim Insight");
            }
        }

        #endregion

        
    }
}



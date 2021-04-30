using Legacy.Service.PageServices.Product;
using Legacy.Service.Support.Enum;
using Legacy.UIAutomation.TestSuites.Base;
using NUnit.Framework;

namespace Legacy.UIAutomation.TestSuites.TestCases.FCI
{
    [Category("FCI")]
    public class FacilityClaimInsight : AutomatedBase
    {
        private FacilityClaimInsightPage _facilityClaimInsight;

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
                return ProductEnum.FCI;
            }

        }

        #endregion

        
        protected override void FixtureSetUp()
        {
            base.FixtureSetUp();
            _facilityClaimInsight = (FacilityClaimInsightPage)LoginPage.Login().GoToProductPage(ProductEnum.FCI);
        }

        protected override void TestInit()
        {
            base.TestInit();
            CurrentPage = _facilityClaimInsight;
        }

        protected override void TestCleanUp()
        {
            base.TestCleanUp();
            if (!CurrentPage.Equals(typeof(FacilityClaimInsightPage)))
            {
                CurrentPage = _facilityClaimInsight = (FacilityClaimInsightPage)CurrentPage.GoBack();
            }
        }

        [Test]
        public void Verify_can_navigate_to_facility_claim_insight_page()
        {
            _facilityClaimInsight.CurrentPageTitle.ShouldEqual(_facilityClaimInsight.PageTitle, "PageTitle", "Page Title Mismatch Error");
        }
    }
}



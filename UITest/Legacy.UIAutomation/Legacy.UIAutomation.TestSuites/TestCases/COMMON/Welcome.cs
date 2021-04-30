using System;
using Legacy.Service.PageServices.ATS;
using Legacy.Service.PageServices.FFP;
using Legacy.Service.PageServices.Invoicing;
using Legacy.Service.PageServices.Negotiation;
using Legacy.Service.PageServices.Password;
using Legacy.Service.PageServices.Product;
using Legacy.Service.PageServices.Rationale;
using Legacy.Service.PageServices.Reports;
using Legacy.Service.PageServices.Welcome;
using Legacy.Service.PageServices.Search;
using Legacy.Service.Support.Enum;
using Legacy.Service.Support.Utils;
using Legacy.UIAutomation.TestSuites.Base;
using NUnit.Framework;

namespace Legacy.UIAutomation.TestSuites.TestCases.COMMON
{
    public class Welcome : AutomatedBase
    {
        private WelcomePage _welcomePage;
        private DentalClaimInsightPage  _dciPage;
        private FacilityClaimInsightPage _fciPage;
        private PhysicianClaimInsightPage _pciPage;
        private SearchPage _searchPage;
        private NegotiationPage _negotiationPage;
        private InvoicingPage _invoicePage;
        private ChangePasswordPage _passwordPage;
        private ReportsAndEventsPage _reportsPage;
        private AtsPage _atsPage;
        private RuleRationalePage _rationalePage;
        private FraudFinderProPage _ffpPage;
         

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
                return ProductEnum.COMMON;
            }

        }

        protected override void FixtureSetUp()
        {
            base.FixtureSetUp();
            BasePage = _welcomePage = LoginPage.Login();
        }

        protected override void TestInit()
        {
            base.TestInit();
            BasePage = _welcomePage;
        }

        protected override void TestCleanUp()
        {
            base.TestCleanUp();
            if (!BasePage.Equals(typeof(WelcomePage)))
            {
                BasePage = _welcomePage = (WelcomePage)BasePage.GoBack();
            }
        }

        [Test]
        public void Verify_Can_Navigate_To_Pci()
        {
            BasePage = _pciPage = (PhysicianClaimInsightPage) _welcomePage.GoToProductPage(ProductEnum.PCI);
            Console.WriteLine("Navigated to Physician Insight Page");
            _pciPage.CurrentPageTitle.ShouldEqual(PageTitleEnum.PhysicianClaimInsight.GetStringValue(), "PageTitle", "Page Title Mismatch Error");
        }

        [Test]
        public void Verify_Can_Navigate_To_Fci()
        {
            BasePage = _fciPage = (FacilityClaimInsightPage) _welcomePage.GoToProductPage(ProductEnum.FCI);
             Console.WriteLine("Navigated to Facility Insight Page");
            _fciPage.CurrentPageTitle.ShouldEqual(PageTitleEnum.FacilityClaimInsight.GetStringValue(), "PageTitle", "Page Title Mismatch Error");
        }

        [Test]
        public void Verify_Can_Navigate_To_Dci()
        {
            BasePage = _dciPage = (DentalClaimInsightPage) _welcomePage.GoToProductPage(ProductEnum.DCI);
            Console.WriteLine("Navigated to DentalClaim Insight Page");
            _dciPage.CurrentPageTitle.ShouldEqual(PageTitleEnum.DentalClaimInsight.GetStringValue(), "PageTitle", "Page Title Mismatch Error");
        }

        [Test]
        public void Verify_Can_Navigate_To_Search_Page()
        {
            BasePage = _searchPage = _welcomePage.ClickOnSearchButtonToNavigateSearchPage();
            Console.WriteLine("Navigated to Search Page");
            _searchPage.CurrentPageTitle.ShouldEqual(PageTitleEnum.Search.GetStringValue(), "PageTitle", "Page Title Mismatch Error");
        }

        [Test]
        public void Verify_Can_Navigate_To_Negotiation_Page()
        {
            BasePage = _negotiationPage = _welcomePage.ClickOnNegotiationButtonToNavigateNegotiationPage();
            Console.WriteLine("Navigated to Negotiation Page");
            _negotiationPage.CurrentPageTitle.ShouldEqual(PageTitleEnum.Negotiation.GetStringValue(), "PageTitle", "Page Title Mismatch Error");
        }

        [Test]
        public void Verify_Can_Navigate_To_Invoice_Page()
        {
            BasePage = _invoicePage = _welcomePage.ClickOnInvoiceButtonToNavigateInvoicePage();
            Console.WriteLine("Navigated to Invoice Page");
            _invoicePage.CurrentPageTitle.ShouldEqual(PageTitleEnum.Invoicing.GetStringValue(), "PageTitle", "Page Title Mismatch Error");
        }

        [Test]
        public void Verify_Can_Navigate_To_Reports_Page()
        {
            BasePage = _reportsPage = _welcomePage.ClickOnReportsButtonToNavigateReportsPage();
            Console.WriteLine("Navigated to Reports And Events Page");
            _reportsPage.CurrentPageTitle.ShouldEqual(PageTitleEnum.Reports.GetStringValue(), "PageTitle", "Page Title Mismatch Error");
        }

        [Test]
        public void Verify_Can_Navigate_To_Ats_Page()
        {
            BasePage = _atsPage = _welcomePage.ClickOnAtsButtonToNavigateAtsPage();
            Console.WriteLine("Navigated to ATS Page");
            _atsPage.CurrentPageTitle.ShouldEqual(PageTitleEnum.Ats.GetStringValue(), "PageTitle", "Page Title Mismatch Error");
        }

        [Test]
        public void Verify_Can_Navigate_To_Password_Page()
        {
            BasePage = _passwordPage = _welcomePage.ClickOnPasswordButtonToNavigateChangePasswordPage();
            Console.WriteLine("Navigated to Change Password Page");
            _passwordPage.CurrentPageTitle.ShouldEqual(PageTitleEnum.ChangePassword.GetStringValue(), "PageTitle", "Page Title Mismatch Error");
        }

        [Test]
        public void Verify_Can_Navigate_To_Rationale_Page()
        {
            BasePage = _rationalePage = _welcomePage.ClickOnRationaleButtonToNavigateRationalePage();
            Console.WriteLine("Navigated to Rationale Page");
            _rationalePage.CurrentPageTitle.ShouldEqual(PageTitleEnum.Rationale.GetStringValue(), "PageTitle", "Page Title Mismatch Error");
        }

        [Test]
        public void Verify_Can_Navigate_To_Ffp()
        {
            BasePage = _ffpPage = _welcomePage.GoToFraudFinderPro();
            Console.WriteLine("Navigated to FraudFinder Pro Page");
            _ffpPage.CurrentPageTitle.ShouldEqual(PageTitleEnum.FraudFinderPro.GetStringValue(), "PageTitle", "Page Title Mismatch Error");
        }

        
    } 
}

using System;
using Legacy.Service.PageObjects.FFP;
using Legacy.Service.PageObjects.Login;
using Legacy.Service.PageObjects.Negotiation;
using Legacy.Service.PageObjects.Password;
using Legacy.Service.PageObjects.Product;
using Legacy.Service.PageObjects.Rationale;
using Legacy.Service.PageObjects.Reports;
using Legacy.Service.PageObjects.Welcome;
using Legacy.Service.PageObjects.ATS;
using Legacy.Service.PageObjects.Invoicing;
using Legacy.Service.PageObjects.Search;
using Legacy.Service.PageServices.Base;
using Legacy.Service.PageServices.FFP;
using Legacy.Service.PageServices.Invoicing;
using Legacy.Service.PageServices.Login;
using Legacy.Service.PageServices.ATS;
using Legacy.Service.PageServices.Negotiation;
using Legacy.Service.PageServices.Password;
using Legacy.Service.PageServices.Rationale;
using Legacy.Service.PageServices.Reports;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Core.Navigation;
using Legacy.Service.PageServices.Product;
using Legacy.Service.PageServices.Search;
using Legacy.Service.Support.Enum;
using UIAutomation.Framework.Elements;

namespace Legacy.Service.PageServices.Welcome
{
    public class WelcomePage : BasePageService//, IDisposable
    {
        private readonly WelcomePageObjects _welcomePage;

        public WelcomePage(INavigator navigator, WelcomePageObjects welcomePage)
            : base(navigator, welcomePage)
        {
            // Just for performance!
            _welcomePage = (WelcomePageObjects)PageObject;
        }


        public ProductPage GoToProductPage(ProductEnum product)
        {
            switch (product)
            {
                case ProductEnum.DCI:
                    StartLegacy.PreAuthorizationProduct = "Dental";
                    return GoToDentalClaimInsight();

                case ProductEnum.FCI:
                    return GoToFacilityClaimInsight();

                default:
                    StartLegacy.PreAuthorizationProduct = "Medical";
                    return GoToPhysicianClaimInsight();
            }
        }

        public PhysicianClaimInsightPage GoToPhysicianClaimInsight()
        {
            var pciPage = Navigator.Navigate<PhysicianClaimInsightPageObjects>
           (() => _welcomePage.PhysicianClaimInsight.Click());
            return new PhysicianClaimInsightPage(Navigator, pciPage);
        }

        public FacilityClaimInsightPage GoToFacilityClaimInsight()
        {
            var fciPage = Navigator.Navigate<FacilityClaimInsightPageObjects>
            (() => _welcomePage.FacilityClaimInsight.Click());
            return new FacilityClaimInsightPage(Navigator, fciPage);
        }

        public DentalClaimInsightPage GoToDentalClaimInsight()
        {
            var dciPage = Navigator.Navigate<DentalClaimInsightPageObjects>
            (() => _welcomePage.DentalClaimInsight.Click());
            return new DentalClaimInsightPage(Navigator, dciPage);
        }


        public FraudFinderProPage GoToFraudFinderPro()
        {
            var ffpPage = Navigator.Navigate<FraudFinderProPageObjects>
            (() => _welcomePage.FraudFinderPro.Click());
            return new FraudFinderProPage(Navigator, ffpPage);
        }


        public SearchPage ClickOnSearchButtonToNavigateSearchPage()
        {
            var searchPage = Navigator.Navigate<SearchPageObjects>
            (() => _welcomePage.SearchBtn.Click());
            return new SearchPage(Navigator, searchPage);
        }

        public NegotiationPage ClickOnNegotiationButtonToNavigateNegotiationPage()
        {
            var negotiationPage = Navigator.Navigate<NegotiationPageObjects>
            (() => SiteDriver.FindElement<ImageButton>(WelcomePageObjects.NegotiationButtonXPath, How.XPath).Click()
            
            );
            return new NegotiationPage(Navigator, negotiationPage);
        }

        public RuleRationalePage ClickOnRationaleButtonToNavigateRationalePage()
        {
            var rationalePage = Navigator.Navigate<RationalePageObjects>
            (() => _welcomePage.RationaleBtn.Click());
            return new RuleRationalePage(Navigator, rationalePage);
        }

        public  ChangePasswordPage ClickOnPasswordButtonToNavigateChangePasswordPage()
        {
            var passwordPage = Navigator.Navigate<ChangePasswordPageObjects>
            (() => _welcomePage.PasswordBtn.Click());
            return new ChangePasswordPage(Navigator, passwordPage);
        }

        public InvoicingPage ClickOnInvoiceButtonToNavigateInvoicePage()
        {
            var invoicingPage = Navigator.Navigate<InvoicingPageObjects>
            (() => _welcomePage.InvoicingBtn.Click());
            return new InvoicingPage(Navigator, invoicingPage);
        }

        public ReportsAndEventsPage ClickOnReportsButtonToNavigateReportsPage()
        {
            var reportsAndEventsPage = Navigator.Navigate<ReportsAndEventsPageObjects>
            (() => _welcomePage.ReportsAndEventsBtn.Click());
            return new ReportsAndEventsPage(Navigator, reportsAndEventsPage);
        }


        public AtsPage ClickOnAtsButtonToNavigateAtsPage()
        {
            var atsPage = Navigator.Navigate<AtsPageObjects>
            (() => _welcomePage.AtsBtn.Click());
            return new AtsPage(Navigator, atsPage);
        }

        public LoginPage Logout()
        {
            var loginPage = Navigator.Navigate<LoginPageObjects>
                (() => _welcomePage.LogOutBtn.Click());
            Console.WriteLine("Logged out from Welcome Page");
            return new LoginPage(Navigator, loginPage);
        }
    }
}

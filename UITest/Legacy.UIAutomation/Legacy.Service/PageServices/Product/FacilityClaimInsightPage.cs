using Legacy.Service.PageObjects;
using Legacy.Service.PageObjects.Product;
using Legacy.Service.PageObjects.Welcome;
using Legacy.Service.PageServices.Base;
using Legacy.Service.PageServices.Default;
using Legacy.Service.PageServices.Welcome;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Core.Navigation;

namespace Legacy.Service.PageServices.Product
{
    public class FacilityClaimInsightPage : ProductPage
    {
        private readonly FacilityClaimInsightPageObjects _fciPage;

        public FacilityClaimInsightPage(INavigator navigator, FacilityClaimInsightPageObjects pciPage)
            : base(navigator, pciPage)
        {
            // Just for performance!
            _fciPage = (FacilityClaimInsightPageObjects)PageObject;
        }

        public override IPageService GoBack()
        {
            var welcomePage = Navigator.Navigate<WelcomePageObjects>(() => _fciPage.BackBtn.Click());
            return new WelcomePage(Navigator, welcomePage);
        }
    }
}

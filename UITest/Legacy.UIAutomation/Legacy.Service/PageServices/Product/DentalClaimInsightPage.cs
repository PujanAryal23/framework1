using Legacy.Service.PageObjects.Product;
using Legacy.Service.PageObjects.Welcome;
using Legacy.Service.PageServices.Base;
using Legacy.Service.PageServices.Welcome;
using UIAutomation.Framework.Core.Navigation;

namespace Legacy.Service.PageServices.Product
{
    public class DentalClaimInsightPage : ProductPage
    {
        private readonly DentalClaimInsightPageObjects _dciPage;

        public DentalClaimInsightPage(INavigator navigator, DentalClaimInsightPageObjects dciPage)
            : base(navigator, dciPage)
        {
            _dciPage = (DentalClaimInsightPageObjects)PageObject;
        }

        public override IPageService GoBack()
        {
            var welcomePage = Navigator.Navigate<WelcomePageObjects>(() => _dciPage.BackBtn.Click());
            return new WelcomePage(Navigator, welcomePage);
        }
    }
}

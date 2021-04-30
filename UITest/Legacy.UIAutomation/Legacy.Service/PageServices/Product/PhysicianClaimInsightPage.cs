using System;
using Legacy.Service.PageObjects.Product;
using Legacy.Service.PageObjects.Welcome;
using Legacy.Service.PageServices.Base;
using Legacy.Service.PageServices.Welcome;
using UIAutomation.Framework.Core.Navigation;

namespace Legacy.Service.PageServices.Product
{
    public class PhysicianClaimInsightPage : ProductPage
    {
        private readonly PhysicianClaimInsightPageObjects _pciPage;

        public PhysicianClaimInsightPage(INavigator navigator, PhysicianClaimInsightPageObjects pciPage)
            : base(navigator, pciPage)
        {
            _pciPage = (PhysicianClaimInsightPageObjects)PageObject;
        }

        public override IPageService GoBack()
        {
            var welcomePage = Navigator.Navigate<WelcomePageObjects>(() => _pciPage.BackBtn.Click());
            return new WelcomePage(Navigator, welcomePage);
        }
    }
}

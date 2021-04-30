using Legacy.Service.PageObjects;
using Legacy.Service.PageObjects.Welcome;
using Legacy.Service.PageServices.Base;
using Legacy.Service.PageServices.Welcome;
using UIAutomation.Framework.Core.Navigation;
using Legacy.Service.PageObjects.Negotiation;
using Legacy.Service.PageServices.Default;

namespace Legacy.Service.PageServices.Negotiation
{
    public class NegotiationPage : DefaultPage
    {
        private readonly NegotiationPageObjects _negotiationPage;

        public NegotiationPage(INavigator navigator, NegotiationPageObjects negotiationPage)
            : base(navigator, negotiationPage)
        {
            // Just for performance!
            _negotiationPage = (NegotiationPageObjects)PageObject;
        }

        public override IPageService GoBack()
        {
            var welcomePage = Navigator.Navigate<WelcomePageObjects>(() => _negotiationPage.BackBtn.Click());
            return new WelcomePage(Navigator, welcomePage);
        }
    }

   
}

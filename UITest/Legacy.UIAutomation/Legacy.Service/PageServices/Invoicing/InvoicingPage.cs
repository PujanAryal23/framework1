using Legacy.Service.PageObjects;
using Legacy.Service.PageObjects.Welcome;
using Legacy.Service.PageServices.Base;
using Legacy.Service.PageServices.Welcome;
using UIAutomation.Framework.Core.Navigation;
using Legacy.Service.PageObjects.Invoicing;

using Legacy.Service.PageServices.Default;

namespace Legacy.Service.PageServices.Invoicing
{
    public class InvoicingPage : DefaultPage
    {
        private readonly InvoicingPageObjects _invoice;

        public InvoicingPage(INavigator navigator, InvoicingPageObjects invoice)
            : base(navigator, invoice)
        {
            // Just for performance!
            _invoice = (InvoicingPageObjects)PageObject;
        }

        public override IPageService GoBack()
        {
            var welcomePage = Navigator.Navigate<WelcomePageObjects>(() => _invoice.BackBtn.Click());
            return new WelcomePage(Navigator, welcomePage);
        }
    }


}

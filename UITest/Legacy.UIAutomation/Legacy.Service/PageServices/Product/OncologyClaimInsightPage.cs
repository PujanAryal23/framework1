using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Legacy.Service.PageObjects;
using Legacy.Service.PageObjects.Product;
using Legacy.Service.PageObjects.Welcome;
using Legacy.Service.PageServices.Base;
using Legacy.Service.PageServices.Default;
using Legacy.Service.PageServices.Welcome;
using UIAutomation.Framework.Core.Navigation;

namespace Legacy.Service.PageServices.Product
{
    public class OncologyClaimInsightPage : DefaultPage
    {
        private readonly OncologyClaimInsightPageObjects _ociPage;

        public OncologyClaimInsightPage(INavigator navigator, OncologyClaimInsightPageObjects pciPage)
            : base(navigator, pciPage)
        {
            // Just for performance!
            _ociPage = (OncologyClaimInsightPageObjects)PageObject;
        }

        public override IPageService GoBack()
        {
            var welcomePage = Navigator.Navigate<WelcomePageObjects>(() => _ociPage.BackBtn.Click());
            return new WelcomePage(Navigator, welcomePage);
        }


    }
}

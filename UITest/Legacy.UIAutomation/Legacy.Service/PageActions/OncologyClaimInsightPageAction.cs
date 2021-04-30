using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Legacy.Service.PageObjects;
using UIAutomation.Framework.Core.Navigation;

namespace Legacy.Service.PageActions
{
    public class OncologyClaimInsightPageAction : BasePageAction
    {
        private readonly OncologyClaimInsightPage _ociPage;

        public OncologyClaimInsightPageAction(INavigator navigator, OncologyClaimInsightPage pciPage)
            : base(navigator, pciPage)
        {
            // Just for performance!
            _ociPage = (OncologyClaimInsightPage)_pageObject;
        }

        public override IPageAction GoBack()
        {
            var welcomePage = Navigator.Navigate<WelcomePage>(() => _ociPage.BackBtn.Click());
            return new WelcomePageAction(Navigator, welcomePage);
        }


    }
}

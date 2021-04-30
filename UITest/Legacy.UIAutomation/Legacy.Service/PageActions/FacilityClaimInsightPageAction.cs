using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Legacy.Service.PageObjects;
using UIAutomation.Framework.Core.Navigation;

namespace Legacy.Service.PageActions
{
    public class FacilityClaimInsightPageAction : BasePageAction
    {
        private readonly FacilityClaimInsightPage _fciPage;

        public FacilityClaimInsightPageAction(INavigator navigator, FacilityClaimInsightPage pciPage)
            : base(navigator, pciPage)
        {
            // Just for performance!
            _fciPage = (FacilityClaimInsightPage)_pageObject;
        }

        public override IPageAction GoBack()
        {
            var welcomePage = Navigator.Navigate<WelcomePage>(() => _fciPage.BackBtn.Click());
            return new WelcomePageAction(Navigator, welcomePage);
        }

       
    }
}

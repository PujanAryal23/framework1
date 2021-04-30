using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Legacy.Service.PageObjects;
using UIAutomation.Framework.Core.Navigation;

namespace Legacy.Service.PageActions
{
    public class PhysicianClaimInsightPageAction : BasePageAction
    {
        private readonly PhysicianClaimInsightPage _pciPage;

        public PhysicianClaimInsightPageAction(INavigator navigator, PhysicianClaimInsightPage pciPage)
            : base(navigator, pciPage)
        {
            // Just for performance!
            _pciPage = (PhysicianClaimInsightPage)_pageObject;
        }

        public override IPageAction GoBack()
        {
            var welcomePage = Navigator.Navigate<WelcomePage>(() => _pciPage.BackBtn.Click());
            return new WelcomePageAction(Navigator, welcomePage);
        }

        public BatchListPageAction NavigateToBatchList()
        {
            var batchListPage = Navigator.Navigate<BatchListPage>(() => _pciPage.BatchListBtn.Click());
            return new BatchListPageAction(Navigator, batchListPage);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Legacy.Service.PageObjects.Product;
using Legacy.Service.PageServices.Base;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Core.Navigation;

namespace Legacy.Service.PageServices.Product
{
    public class RevDescriptionPage : BasePageService
    {
        private RevDescriptionPageObjects _revDescriptionPage;

        public RevDescriptionPage(INavigator navigator, RevDescriptionPageObjects revDescriptionPage)
            : base(navigator, revDescriptionPage)
        {
            _revDescriptionPage = (RevDescriptionPageObjects)PageObject;
        }

        public ClaimSummaryPage CloseRevDesc(string windowHandle)
        {
            
            SiteDriver.CloseWindowAndSwitchTo(windowHandle);
            
            SiteDriver.SwitchFrame("View");
            return new ClaimSummaryPage(Navigator, new ClaimSummaryPageObjects());
        }
    }
}

using Legacy.Service.PageServices.Base;
using Legacy.Service.Support;
using Legacy.Service.Support.Enum;
using Legacy.Service.Support.Utils;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Core.Navigation;
using Legacy.Service.PageObjects.Product;

namespace Legacy.Service.PageServices.Product
{
    public class ProviderAppealPage : BasePageService
    {
        private ProviderAppealPageObjects _providerAppealPage;

        public ProviderAppealPage(INavigator navigator, ProviderAppealPageObjects providerAppealPage)
            : base(navigator, providerAppealPage)
        {
            _providerAppealPage = (ProviderAppealPageObjects)PageObject;
        }

        /// <summary>
        /// CLose appeal popup
        /// </summary>
        /// <param name="pageTitle"></param>
        /// <returns></returns>
        public ClaimSummaryPage CloseProviderAppealPopup(string pageTitle)
        {
            var claimSummaryPage = Navigator.Navigate<ClaimSummaryPageObjects>(() =>
                                                {
                                                    SiteDriver.CloseWindow();
                                                    SiteDriver.SwitchWindowByTitle(pageTitle);
                                                });
            SiteDriver.SwitchFrame("View");
            return new ClaimSummaryPage(Navigator, claimSummaryPage);
        }
    }
}

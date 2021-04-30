using Legacy.Service.PageServices.Base;
using Legacy.Service.Support;
using Legacy.Service.Support.Enum;
using Legacy.Service.Support.Utils;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Core.Navigation;
using Legacy.Service.PageObjects.Product;

namespace Legacy.Service.PageServices.Product
{
    public class ClaimHistoryPage : BasePageService
    {
        private ClaimHistoryPageObjects _claimHistoryPage;

        public ClaimHistoryPage(INavigator navigator, ClaimHistoryPageObjects claimHistoryPage)
            : base(navigator, claimHistoryPage)
        {
            _claimHistoryPage = (ClaimHistoryPageObjects)PageObject;
        }

        /// <summary>
        /// CLose claim history page
        /// </summary>
        /// <param name="pageTitle"></param>
        /// <returns></returns>
        public FlaggedClaimPage CloseClaimHistoryPopup(string pageTitle)
        {
            var flaggedClaimPage = Navigator.Navigate<FlaggedClaimPageObjects>(() =>
            {
                SiteDriver.CloseWindow();
                SiteDriver.SwitchWindowByTitle(pageTitle);
            });
            return new FlaggedClaimPage(Navigator, flaggedClaimPage);
        }
    }
}

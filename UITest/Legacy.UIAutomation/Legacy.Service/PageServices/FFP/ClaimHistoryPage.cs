using Legacy.Service.PageServices.Base;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Core.Navigation;
using Legacy.Service.PageObjects.FFP;

namespace Legacy.Service.PageServices.FFP
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

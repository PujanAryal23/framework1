using System;
using Legacy.Service.PageObjects.Product;
using Legacy.Service.PageServices.Base;
using Legacy.Service.Support.Enum;
using Legacy.Service.Support.Utils;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Core.Navigation;

namespace Legacy.Service.PageServices.Product
{
    public class PreAuthorizationSearchPage : BasePageService
    {
        private readonly PreAuthorizationSearchPageObjects _preAuthorizationSearchPage;

        public PreAuthorizationSearchPage(INavigator navigator, PreAuthorizationSearchPageObjects preAuthorizationSearch)
            : base(navigator, preAuthorizationSearch)
        {
            _preAuthorizationSearchPage = (PreAuthorizationSearchPageObjects)PageObject;
        }

        /// <summary>
        /// Get preauthorization search  window handle
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        public PreAuthorizationSearchPage GetWindowHandle(out string handle)
        {
            return GetCurrentWindowHandle<PreAuthorizationSearchPage>(_preAuthorizationSearchPage,out handle);
        }

        /// <summary>
        /// Get title label
        /// </summary>
        /// <returns></returns>
        public string GetTitleLabel()
        {
            return _preAuthorizationSearchPage.TitleLabel.Text;
        }


        /// <summary>
        /// Click close button of patient history page
        /// </summary>
        /// <returns></returns>
        public ClaimSummaryPage ClickCloseButton()
        {
            var claimSummary = Navigator.Navigate<ClaimSummaryPageObjects>(() =>
            {
                _preAuthorizationSearchPage.CloseButton.Click();
                Console.WriteLine("Clicked Close Button of PreAuthorization Page.");
                SiteDriver.SwitchWindowByTitle(PageTitleEnum.ClaimSummary.GetStringValue());
            });
            SiteDriver.CloseFrameAndSwitchTo("View");
            return new ClaimSummaryPage(Navigator, claimSummary);
        }

       
    }
}

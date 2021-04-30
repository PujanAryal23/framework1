using System;
using Legacy.Service.PageObjects.Product;
using Legacy.Service.PageServices.Base;
using Legacy.Service.Support.Enum;
using Legacy.Service.Support.Utils;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Core.Navigation;
using UIAutomation.Framework.Elements;

namespace Legacy.Service.PageServices.Product
{
    public class OriginalDataPage : BasePageService
    {
        private readonly OriginalDataPageObjects _originalDataPage;

        public OriginalDataPage(INavigator navigator, OriginalDataPageObjects originalData)
            : base(navigator, originalData)
        {
            _originalDataPage = (OriginalDataPageObjects)PageObject;
        }



        /// <summary>
        /// Get original data  window handle
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        public OriginalDataPage GetWindowHandle(out string handle)
        {
            return GetCurrentWindowHandle<OriginalDataPage>(_originalDataPage,out handle);
        }

        /// <summary>
        /// Get title label
        /// </summary>
        /// <returns></returns>
        public string GetTitleLabel()
        {
            SiteDriver.SwitchFrame("Header");
            return _originalDataPage.TitleLabel.Text;
        }


        /// <summary>
        /// Click close button of popup
        /// </summary>
        /// <returns></returns>
        public ClaimSummaryPage ClickCloseButton()
        {
            var claimSummary = Navigator.Navigate<ClaimSummaryPageObjects>(() =>
            {
                _originalDataPage.CloseButton.Click();
                Console.WriteLine("Clicked Close Button of Original Data Page.");
                SiteDriver.SwitchWindowByTitle(PageTitleEnum.ClaimSummary.GetStringValue());
            });
            SiteDriver.CloseFrameAndSwitchTo("View");
            return new ClaimSummaryPage(Navigator, claimSummary);
        }

        /// <summary>
        /// Switch frame to header
        /// </summary>
        /// <returns></returns>
        public void SwitchFrameToHeader()
        {
            SiteDriver.SwitchFrame("Header");
        }



    }
}

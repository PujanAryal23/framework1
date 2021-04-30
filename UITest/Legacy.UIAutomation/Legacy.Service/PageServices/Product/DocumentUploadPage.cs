using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Legacy.Service.PageObjects.Product;
using Legacy.Service.PageServices.Base;
using Legacy.Service.Support;
using Legacy.Service.Support.Enum;
using Legacy.Service.Support.Utils;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Core.Navigation;

namespace Legacy.Service.PageServices.Product
{
    public class DocumentUploadPage : BasePageService
    {
        private readonly DocumentUploadPageObjects _documentUploadPage;

        public DocumentUploadPage(INavigator navigator, DocumentUploadPageObjects documentUploadPage)
            : base(navigator, documentUploadPage)
        {
            _documentUploadPage = (DocumentUploadPageObjects)PageObject;
        }

        /// <summary>
        /// Get document upload window handle
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        public DocumentUploadPage GetWindowHandle(out string handle)
        {
            return GetCurrentWindowHandle<DocumentUploadPage>(_documentUploadPage,out handle);
        }

        /// <summary>
        /// Click close button of document upload page
        /// </summary>
        /// <returns></returns>
        public ClaimSummaryPage ClickCloseButton()
        {
            var claimSummary = Navigator.Navigate<ClaimSummaryPageObjects>(() =>
                                                {
                                                    _documentUploadPage.CloseButton.Click();
                                                    Console.WriteLine("Clicked Close Button of Document Upload Page.");
                                                    SiteDriver.SwitchWindowByTitle(string.Format(PageTitleEnum.ClaimSummary.GetStringValue(), ProductTitle));
                                                });
            SiteDriver.CloseFrameAndSwitchTo("View");
            return new ClaimSummaryPage(Navigator, claimSummary);
        }
    }
}

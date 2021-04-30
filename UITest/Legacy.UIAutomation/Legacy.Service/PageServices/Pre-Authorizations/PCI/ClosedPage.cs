using System;
using Legacy.Service.PageObjects.Pre_Authorizations;
using Legacy.Service.PageObjects.Pre_Authorizations.PCI;
using Legacy.Service.PageServices.Default;
using Legacy.Service.Support.Enum;
using Legacy.Service.Support.Utils;
using UIAutomation.Framework.Core.Navigation;
using Legacy.Service.PageServices.Common;
using UIAutomation.Framework.Elements;

namespace Legacy.Service.PageServices.Pre_Authorizations.PCI
{
    public class ClosedPage : DefaultPage
    {
        #region PRIVATE FIELDS

        private ClosedPageObjects _closedPage;

        #endregion

        #region CONSTRUCTOR

        public ClosedPage(INavigator navigator, ClosedPageObjects closedPage)
            : base(navigator, closedPage)
        {
            _closedPage = (ClosedPageObjects)PageObject;
        }

        #endregion

        #region PUBLIC METHODS

        /// <summary>
        /// Go one step back
        /// </summary>
        /// <returns></returns>
        public override Base.IPageService GoBack()
        {
            var preAuthorizationPage = Navigator.Navigate<PreAuthPageObjects>(() => _closedPage.BackButton.Click());
            return new PreAuthPage(Navigator, preAuthorizationPage);
        }

        /// <summary>
        /// Go back to same page
        /// </summary>
        /// <returns></returns>
        public ClosedPage GoBackToSamePage()
        {
            _closedPage = Navigator.Navigate<ClosedPageObjects>(() => _closedPage.BackButton.Click());
            return new ClosedPage(Navigator, _closedPage); 
        }

        /// <summary>
        /// Click on Search Button
        /// </summary>
        /// <param name="hitSearchButton"></param>
        /// <returns></returns>
        public ClosedPage ClickSearchButton(out bool hitSearchButton)
        {
            hitSearchButton = false;
            _closedPage = Navigator.Navigate<ClosedPageObjects>(() =>
            {
                _closedPage.SearchButton.Click();
                Console.Out.WriteLine("Click Search button.");
            });
            hitSearchButton = true;
            return new ClosedPage(Navigator, _closedPage);
        }

       

        /// <summary>
        /// Get recv date
        /// </summary>
        /// <param name="recvDate"></param>
        /// <param name="iSFrom"></param>
        /// <returns></returns>
        public ClosedPage GetClienRecvDate(out DateTime recvDate, bool iSFrom = true)
        {
            TextField recvDateTxt = iSFrom
                                        ? _closedPage.ReceivedFromTxt
                                        : _closedPage.ReceivedToTxt;
            return GetDate<ClosedPage>(_closedPage, recvDateTxt, out recvDate);
        }

        #endregion
    }
}

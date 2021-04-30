using System;
using Legacy.Service.PageObjects.Pre_Authorizations;
using Legacy.Service.PageObjects.Pre_Authorizations.DCI;
using Legacy.Service.PageServices.Default;
using UIAutomation.Framework.Core.Navigation;

namespace Legacy.Service.PageServices.Pre_Authorizations.DCI
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

        #endregion
    }
}

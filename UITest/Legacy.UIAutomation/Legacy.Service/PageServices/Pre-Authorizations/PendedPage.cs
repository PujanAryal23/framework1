using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Legacy.Service.PageObjects.Pre_Authorizations;
using Legacy.Service.PageServices.Default;
using UIAutomation.Framework.Core.Navigation;

namespace Legacy.Service.PageServices.Pre_Authorizations
{
    public class PendedPage : DefaultPage
    {
        #region PRIVATE FIELDS

        private PendedPageObjects _pendedPage;

        #endregion

        #region CONSTRUCTOR

        public PendedPage(INavigator navigator, PendedPageObjects pendedPage)
            : base(navigator, pendedPage)
        {
            _pendedPage = (PendedPageObjects)PageObject;
        }

        #endregion

        #region PUBLIC METHODS

        /// <summary>
        /// Go one step back
        /// </summary>
        /// <returns></returns>
        public override Base.IPageService GoBack()
        {
            var preAuthorizationPage = Navigator.Navigate<PreAuthPageObjects>(() => _pendedPage.BackButton.Click());
            return new PreAuthPage(Navigator, preAuthorizationPage);
        }

        #endregion
    }
}

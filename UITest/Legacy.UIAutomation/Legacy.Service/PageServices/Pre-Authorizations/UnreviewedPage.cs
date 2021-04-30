using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Legacy.Service.PageObjects.Pre_Authorizations;
using Legacy.Service.PageServices.Default;
using UIAutomation.Framework.Core.Navigation;

namespace Legacy.Service.PageServices.Pre_Authorizations
{
    public class UnreviewedPage : DefaultPage
    {
        #region PRIVATE FIELDS

        private UnreviewedPageObjects _unreviewedPage;

        #endregion

        #region CONSTRUCTOR

        public UnreviewedPage(INavigator navigator, UnreviewedPageObjects unreviewedPage)
            : base(navigator, unreviewedPage)
        {
            _unreviewedPage = (UnreviewedPageObjects)PageObject;
        }

        #endregion

        #region PUBLIC METHODS

        /// <summary>
        /// Go one step back
        /// </summary>
        /// <returns></returns>
        public override Base.IPageService GoBack()
        {
            var preAuthorizationPage = Navigator.Navigate<PreAuthPageObjects>(() => _unreviewedPage.BackButton.Click());
            return new PreAuthPage(Navigator, preAuthorizationPage);
        }

        #endregion
    }
}

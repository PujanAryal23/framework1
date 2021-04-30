using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Legacy.Service.PageObjects.Pre_Authorizations;
using Legacy.Service.PageObjects.Pre_Authorizations.DCI;
using Legacy.Service.PageServices.Default;
using UIAutomation.Framework.Core.Navigation;

namespace Legacy.Service.PageServices.Pre_Authorizations.DCI
{
    public class ConsultCompletePage : DefaultPage
    {
         #region PRIVATE FIELDS

        private ConsultCompletePageObjects _consultCompletePage;

        #endregion

        #region CONSTRUCTOR

        public ConsultCompletePage(INavigator navigator, ConsultCompletePageObjects consultCompletePage)
            : base(navigator, consultCompletePage)
        {
            _consultCompletePage = (ConsultCompletePageObjects)PageObject;
        }

        #endregion

        #region PUBLIC METHODS

        /// <summary>
        /// Go one step back
        /// </summary>
        /// <returns></returns>
        public override Base.IPageService GoBack()
        {
            var preAuthorizationPage = Navigator.Navigate<PreAuthPageObjects>(() => _consultCompletePage.BackButton.Click());
            return new PreAuthPage(Navigator, preAuthorizationPage);
        }

        #endregion
    }
}

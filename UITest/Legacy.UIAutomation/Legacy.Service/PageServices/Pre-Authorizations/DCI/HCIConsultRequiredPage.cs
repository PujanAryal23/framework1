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
    public class HCIConsultRequiredPage : DefaultPage
    {
         #region PRIVATE FIELDS

        private HCIConsultRequiredPageObjects _hciConsultRequiredPage;

        #endregion

        #region CONSTRUCTOR

        public HCIConsultRequiredPage(INavigator navigator, HCIConsultRequiredPageObjects hciConsultRequiredPage)
            : base(navigator, hciConsultRequiredPage)
        {
            _hciConsultRequiredPage = (HCIConsultRequiredPageObjects)PageObject;
        }

        #endregion

        #region PUBLIC METHODS

        /// <summary>
        /// Go one step back
        /// </summary>
        /// <returns></returns>
        public override Base.IPageService GoBack()
        {
            var preAuthorizationPage = Navigator.Navigate<PreAuthPageObjects>(() => _hciConsultRequiredPage.BackButton.Click());
            return new PreAuthPage(Navigator, preAuthorizationPage);
        }

        #endregion
    }
}

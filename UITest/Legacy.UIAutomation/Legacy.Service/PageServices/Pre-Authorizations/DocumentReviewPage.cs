using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Legacy.Service.PageObjects.Pre_Authorizations;
using Legacy.Service.PageServices.Default;
using UIAutomation.Framework.Core.Navigation;

namespace Legacy.Service.PageServices.Pre_Authorizations
{
    public class DocumentReviewPage : DefaultPage
    {
        #region PRIVATE FIELDS

        private DocumentReviewPageObjects _documentReviewPage;

        #endregion

        #region CONSTRUCTOR

        public DocumentReviewPage(INavigator navigator, DocumentReviewPageObjects documentReviewPage)
            : base(navigator, documentReviewPage)
        {
            _documentReviewPage = (DocumentReviewPageObjects)PageObject;
        }

        #endregion

        #region PUBLIC METHODS

        /// <summary>
        /// Go one step back
        /// </summary>
        /// <returns></returns>
        public override Base.IPageService GoBack()
        {
            var preAuthorizationPage = Navigator.Navigate<PreAuthPageObjects>(() => _documentReviewPage.BackButton.Click());
            return new PreAuthPage(Navigator, preAuthorizationPage);
        }

        #endregion
    }
}

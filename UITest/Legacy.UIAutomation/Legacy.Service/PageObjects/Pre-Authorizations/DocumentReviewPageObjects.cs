using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Legacy.Service.PageObjects.Default;
using Legacy.Service.Support.Enum;
using Legacy.Service.Support.Utils;

namespace Legacy.Service.PageObjects.Pre_Authorizations
{
    public class DocumentReviewPageObjects : DefaultPageObjects
    {
        #region OVERRIDE PROPERTIES

        public override string PageTitle
        {
            get { return PageTitleEnum.DocumentReview.GetStringValue(); }
        }

        #endregion

        #region CONSTRUCTOR

        public DocumentReviewPageObjects()
            : base(string.Format(PageUrlEnum.ListPage.GetStringValue(), StartLegacy.PreAuthorizationProduct))
        {

        }

        #endregion
    }
}

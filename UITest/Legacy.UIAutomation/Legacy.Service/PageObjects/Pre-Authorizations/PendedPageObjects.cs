using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Legacy.Service.PageObjects.Default;
using Legacy.Service.Support.Enum;
using Legacy.Service.Support.Utils;

namespace Legacy.Service.PageObjects.Pre_Authorizations
{
    public class PendedPageObjects : DefaultPageObjects
    {
        #region OVERRIDE PROPERTIES

        public override string PageTitle
        {
            get { return PageTitleEnum.MedicalPended.GetStringValue(); }
        }

        #endregion

        #region CONSTRUCTOR

        public PendedPageObjects()
            : base(string.Format(PageUrlEnum.ListPage.GetStringValue(), StartLegacy.PreAuthorizationProduct))
        {

        }

        #endregion
    }
}

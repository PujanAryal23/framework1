using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Legacy.Service.PageObjects.Default;
using Legacy.Service.Support.Enum;
using Legacy.Service.Support.Utils;

namespace Legacy.Service.PageObjects.Pre_Authorizations
{
    public class UnreviewedPageObjects : DefaultPageObjects
    {
        #region OVERRIDE PROPERTIES

        public override string PageTitle
        {
            get
            {
                if(StartLegacy.Product == ProductEnum.PCI)
                    return string.Format(PageTitleEnum.Unreviewed.GetStringValue(), "");
                return string.Format(PageTitleEnum.Unreviewed.GetStringValue(), StartLegacy.PreAuthorizationProduct + " ");
            }
        }

        #endregion

        #region CONSTRUCTOR

        public UnreviewedPageObjects()
            : base(string.Format(PageUrlEnum.ListPage.GetStringValue(), StartLegacy.PreAuthorizationProduct))
        {

        }

        #endregion
    }
}

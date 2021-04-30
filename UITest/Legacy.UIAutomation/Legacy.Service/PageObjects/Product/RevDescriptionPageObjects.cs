using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Legacy.Service.Support.Utils;
using UIAutomation.Framework.Core.Base;
using Legacy.Service.Support.Enum;

namespace Legacy.Service.PageObjects.Product
{
    public class RevDescriptionPageObjects : PageBase
    {
         #region OVERRIDE PROPERTIES

        public override string PageTitle
        {
            get { return PageTitleEnum.RevDesc.GetStringValue(); }
        }

        #endregion

        #region CONSTRUCTOR

        public RevDescriptionPageObjects()
            : base(ProductPageUrlEnum.RevDesc.GetStringValue())
        {

        }

        #endregion
    }
}

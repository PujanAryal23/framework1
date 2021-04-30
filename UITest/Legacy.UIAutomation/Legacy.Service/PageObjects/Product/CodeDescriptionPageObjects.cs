using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Legacy.Service.Support;
using Legacy.Service.Support.Enum;
using Legacy.Service.Support.Utils;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Base;
using UIAutomation.Framework.Elements;

namespace Legacy.Service.PageObjects.Product
{
    public class CodeDescriptionPageObjects : PageBase
    {
        #region PUBLIC PROPERTIES

        public const string CodeDescLabelId = "lblCodeDescription";

        #endregion

        #region PAGEOBJECTS PROPERTIES

        [FindsBy(How = How.Id, Using = CodeDescLabelId)]
        public TextLabel CodeDescLabel;

        #endregion

        #region OVERRIDE PROPERTIES

        public override string PageTitle
        {
            get { return PageTitleEnum.CodeDesc.GetStringValue(); }
        }

        #endregion

        #region CONSTRUCTOR

        public CodeDescriptionPageObjects()
            : base(ProductPageUrlEnum.CodeDesc.GetStringValue())
        {

        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Legacy.Service.Support.Enum;
using Legacy.Service.Support.Utils;
using UIAutomation.Framework.Core.Base;
using UIAutomation.Framework.Elements;
using UIAutomation.Framework.Common;

namespace Legacy.Service.PageObjects.Product
{
    public class NotifyClientPageObjects : PageBase
    {
        #region PRIVATE/PUBLIC PROPERTIES

        private const string CloseButtonXPath = ".//img[@alt='Close window']";

        #endregion

        #region PAGEOBJECT PROPERTIES

        [FindsBy(How = How.XPath, Using = CloseButtonXPath)]
        public ImageButton CloseButton;

        #endregion

        #region OVERRIDE PROPERTIES

        public override string PageTitle
        {
            get {return PageTitleEnum.NotifyClient.GetStringValue(); }
        }

        #endregion

         #region CONSTRUCTOR

        public NotifyClientPageObjects()
            : base(ProductPageUrlEnum.NotifyClient.GetStringValue())
        {

        }

        #endregion


    }
}

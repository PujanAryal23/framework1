using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Legacy.Service.PageObjects.Default;
using Legacy.Service.Support.Enum;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Elements;
using UIAutomation.Framework.Utils;

namespace Legacy.Service.PageObjects.Search
{
    public class SearchPageObjects : DefaultPageObjects
    {
       

        #region PRIVATE/PUBLIC PROPERTIES

       

        #endregion

        #region PAGEOBJECTS PROPERTIES

        [FindsBy(How = How.XPath, Using = "//img[contains(@src,'_Images/Btn_Back.jpg')]")]
        public ImageButton BackBtn;

        
        #endregion

        #region OVERRIDE PROPERTIES

        public override string PageTitle
        {
            get { return string.Format(PageTitleEnum.Search.GetStringValue(), StartLegacy.Product.GetStringValue()); }
        }

        #endregion

        #region CONSTRUCTOR

        public SearchPageObjects()
            : base(ProductPageUrlEnum.Search.GetStringValue())
        {
        }

        #endregion
    }
}

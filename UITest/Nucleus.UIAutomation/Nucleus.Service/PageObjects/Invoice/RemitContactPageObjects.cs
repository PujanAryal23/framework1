
using Nucleus.Service.Support.Utils;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Base;
using Nucleus.Service.Support.Enum;
using UIAutomation.Framework.Elements;

namespace Nucleus.Service.PageObjects.Invoice
{
    public class RemitContactPageObjects : PageBase
    {
        #region PRIVATE FIELDS

       
        private const string RemitDivCssLocator = "div.Remit";
        private const string ContactDivCssLocator = "div.Contact";
        

        #endregion

        #region PAGEOBJECTS PROPERTIES

        [FindsBy(How = How.CssSelector, Using = RemitDivCssLocator)]
        public TextField RemitDiv;
        [FindsBy(How = How.CssSelector, Using = ContactDivCssLocator)]
        public TextField ContactDiv;

        

        #endregion

        #region OVERRIDE PROPERTIES

        public override string PageTitle
        {
            get { return PageTitleEnum.RemitContact.GetStringValue(); }
        }

        #endregion

        #region CONSTRUCTOR

        public RemitContactPageObjects()
            : base(PageUrlEnum.RemitContact.GetStringValue())
        {
        }

        #endregion
    }
}

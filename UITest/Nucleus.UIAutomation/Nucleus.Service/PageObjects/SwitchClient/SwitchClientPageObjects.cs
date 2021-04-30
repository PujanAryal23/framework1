using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Base;
using UIAutomation.Framework.Elements;

namespace Nucleus.Service.PageObjects.SwitchClient
{
    public class SwitchClientPageObjects : NewPageBase
    {
        #region PUBLIC FIELDS

        #region ID

        public const string SwitchClientXPathTemplate = "//li[span[text()='{0}']]/a[text()='{1}']";

        public const string SwitchClientTemplateCore = "ul:has(span:contains({0})) span:contains({1})";

        // public const string SwitchClientControlXPath = "//div[@class='swc-ControlWrapper']";

        public const string SwitchClientListCssSelector =
            "section.sidebar_content ul li.no_link:not([class*=no_value]) span";

        #endregion

        #endregion
     
        #region CONSTRUCTOR


        #endregion
    }
}

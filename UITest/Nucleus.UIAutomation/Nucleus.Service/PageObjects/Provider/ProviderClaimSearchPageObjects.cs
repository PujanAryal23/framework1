using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nucleus.Service.PageObjects.Default;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Utils;

namespace Nucleus.Service.PageObjects.Provider
{
    public class ProviderClaimSearchPageObjects : NewDefaultPageObjects
    {
        #region CONSTRUCTOR 
        public ProviderClaimSearchPageObjects() : base(PageUrlEnum.ProviderClaimSearch.GetStringValue())
        { }
        #endregion

        #region PROTECTED PROPERTIES
        public override string PageTitle
        {
            get { return PageTitleEnum.ProviderClaimSearch.GetStringValue(); }
        }

        #endregion

        #region CSS/XPathLocators

        public const string SearchIconToReturnToProviderActionCssSelector = "span.search_icon";
        public const string FilterOptionsListCssLocator = "li.appeal_search_filter_options";
        public const string FilterOptionListByCss = "li.appeal_search_filter_options>ul>li>span";
        public const string FilterOptionValueByCss = "li.appeal_search_filter_options>ul>li:nth-of-type({0})>span";

        public const string ClaimLineDetailsLabelXPath = "//label[text()='{0}']";

        public const string ProcCodeValueByLabelXpath = "//label[text()='{0}']/../div";

        public const string ModifiersDxCodesLabelXPath = "//li[text()='{0}']";

        public const string ClaimLineDetailsValueByLabelXPath = "//label[text()='{0}']/../span";

        public const string DxCodesValueByLabelXPath = "//li[text()='{0}']/..//li[{1}]//div";

        public const string ModifiersValueByLabelXpath = "//li[text()='{0}']/..//li//span";

        public const string ProviderSequenceFromHeaderCssLocator = ".header_data_point:has(label:contains(\"Provider Seq\")) span";
        public const string BeginDateInSidebarCssLocator = "div.range_date_input_component>input:nth-of-type(1)";
        public const string EndDateInSidebarCssLocator = "div.range_date_input_component>input:nth-of-type(2)";

        #endregion
    }
}

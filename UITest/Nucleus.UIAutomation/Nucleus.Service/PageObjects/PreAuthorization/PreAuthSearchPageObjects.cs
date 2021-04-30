using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nucleus.Service.PageObjects.Default;
using Nucleus.Service.Support.Enum;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Elements;
using UIAutomation.Framework.Utils;


namespace Nucleus.Service.PageObjects.PreAuthorization
{
    public class PreAuthSearchPageObjects : NewDefaultPageObjects
    {
        #region PRIVATE/PUBLIC FIELDS

        public const string PreAuthSearchResultListCssTemplate = "ul.component_item_list>div>ul>li:nth-of-type({0})>span";
        public const string AuthListOnSearchResult = ".component_item_list li.action_link span";
        public const string FilterOptionsListCssLocator = "li.appeal_search_filter_options >ul>li";
        public const string FilterOptionsIconCssLocator = "li.appeal_search_filter_options";
        public const string PreAuthDetailsLabelXPath =
            "//section[contains(@class,'column_40')]/section[contains(@class,'component_content')]/div/ul[contains(@class,'component_item_row')]/li/label[contains(@title,'{0}')]";
        public const string PreAuthDetailsValueXPath = "//label[contains(@title,'{0}')]/../span";
        public const string SideBarIconCssLocator = "span.sidebar_icon";
        public const string LockIconByAuthSeqCssLocator = "ul.component_item_row:has(li:nth-child(2) span.data_point_value:contains({0})) li.lock";
        public const string UpperRightQuadrantLabelCssLocator = "#top_section section.component_right>section>div>label";

        #endregion

        #region CONSTRUCTOR

        public PreAuthSearchPageObjects()
            : base(PageUrlEnum.PreAuthSearch.GetStringValue())
        {
        }

        #endregion

        #region XPATH/CSS LOCATORS
        public const string FindXPath = "//button[text()='Find']";
        public const string LockIconCssSelector = "li.lock";

        public const string LastUnlockedAuthSeqXPath =
            "((//section[contains(@class,'component_left')]//section[contains(@class,'component_content')]/ul/ul/div/ul/li/ul/li[not(contains(@class,'lock'))])[last()]/../..)/following-sibling::li[1]/span";

        #endregion

        #region PROTECTED PROPERTIES

        public override string PageTitle
        {
            get { return PageTitleEnum.PreAuthorization.GetStringValue(); }
        }

        #endregion


    }
}

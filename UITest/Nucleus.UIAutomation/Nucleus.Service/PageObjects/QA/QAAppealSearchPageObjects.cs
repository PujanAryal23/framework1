using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nucleus.Service.PageObjects.Default;
using Nucleus.Service.Support.Enum;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Elements;
using UIAutomation.Framework.Utils;

namespace Nucleus.Service.PageObjects.QA
{

    public class QaAppealSearchPageObjects : NewDefaultPageObjects
    {

        #region PRIVATE/PUBLIC FIELDS
        
        
        


        #region Css/XPATH
        public const string PageInsideTitleCssLocator = "label.page_title";
        public const string FilterOptionsListCssLocator = "ul.option_list>li";
        public const string FilterOptionsIconCssLocator = "span.filters";
        public const string QAAppealSearchResultListCssTemplate =
            "ul.component_item_list>div>ul>li:nth-of-type({0}) span";
        public const string FilterCssSelectorInactive = "li[class*='not_selected'] span.filters";
        public const string CompletedDateFrom = "//label[text()='Completed Date']/../input[1]";
        public const string CompletedDateTo = "//label[text()='Completed Date']/../input[2]";
        public const string FilterListValueCssTemplate = "//span[text() = '{0}']";
        //public const string LoadMoreCssLocator = "span.load_more_data";
        public const string DisabledFindButtonCssLocator = "div.is_disabled>button.work_button";
        public const string FindButtonCssLocator = "button.work_button";
        public const string QaAppealSearchResultSectionCssTemplate =
            "section.search_list:has( label:contains(QA Appeal Search)) section.component_content";
        public const string QaAppealResutGridCheckForVerticalScrollBar = "section.search_list section.component_content ";
        public const string ExportIconCssLocator = "span[title=Export]";
        public const string AppealResultGridViewSectionCSSSelector = "ul.component_item_list>.component_item>ul>li:nth-of-type(1)>ul>li:nth-child(2)";


        #endregion

        #endregion

        #region PAGEOBJECTS PROPERTIES

        //[FindsBy(How = How.CssSelector, Using = FilterOptionsIconCssLocator)]
        //public Link SortFilterIcon;

        #endregion

        #region PROTECTED PROPERTIES

        public override string PageTitle
        {
         get { return PageTitleEnum.QAAppealSearch.GetStringValue(); }
        }

        #endregion

        #region CONSTRUCTOR

        public QaAppealSearchPageObjects()
            : base(PageUrlEnum.QaClaimSearch.GetStringValue())
        {
        }
        #endregion
        
    }
}

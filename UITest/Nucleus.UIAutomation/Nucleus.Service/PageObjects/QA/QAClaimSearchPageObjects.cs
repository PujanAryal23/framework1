using Nucleus.Service.PageObjects.Default;
using Nucleus.Service.Support.Enum;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Elements;
using UIAutomation.Framework.Utils;

namespace Nucleus.Service.PageObjects.QA
{
    public class QaClaimSearchPageObjects : NewDefaultPageObjects
    {
        #region PRIVATE/PUBLIC FIELDS

        #region Link Text

        public const string SortByClientLinkText = "Sort By Client";
        public const string SortByAnalystLinkText = "Sort By Analyst";
        public const string ClearSortLinkText = "Clear Sort";

        #endregion

        #region Css
        public const string DisabledFindButtonCssLocator = "div.is_disabled>button.work_button";
        public const string FindButtonCssLocator = "button.work_button";
        public const string PageInsideTitleCssLocator = "label.page_title";
        public const string SearchAnalystCssLocator = "span.sidebar_icon";
        public const string FilterPanelCssLocator = "label.component_title";
        public const string OkConfirmationCssSelector = "div#confirmation_links > div#complete_link";
        public const string CancelConfirmationCssSelector = "div#confirmation_links > span.span_link.modal_close";
        public const string SearchResultListCountCssSelector = "div.component_item";
        public const string ClearFilterClass = "div.form_buttons  span.span_link";
        public const string InputFieldByLabelCssTemplate = "div:has(>label:contains({0})) input";
        public const string EmptySearchResultCssTemplate = "section.search_list  p.empty_message";
        public const string ToggleFindAnalystPanelCssLocator = "span.sidebar_icon.toolbar_icon";
        public const string FilterCssSelector = "span.filters";
        public const string FilterOptionListCssLocator = "ul.option_list>li>span";
        public const string FilterListValueCssTemplate = "//span[text()='{0}']";
        public const string NoQaResultIconCssSelector="ul.component_item_list>div>ul>li>ul>li:not([class*=qa_pass]):not([class*=qa_fail])";
        public const string QaPassResultIconCssSelector = "ul.component_item_list>div>ul>li>ul>li.qa_pass";
        public const string QaFailResultIconCssSelector = "ul.component_item_list>div>ul>li>ul>li.qa_fail";
        public const string ClaseqByQaPassResultIconJQuery = "ul.component_item_list>div>ul:has(li.qa_pass)>li:nth-of-type(3)>span";
        public const string ClaseqByQaFailResultIconJQuery = "ul.component_item_list>div>ul:has(li.qa_fail)>li:nth-of-type(3)>span";
        public const string ClaseqByNoQaResultIconJQuery = "ul.component_item_list>div>ul:has(>li>ul>li:not([class*=qa_pass]):not([class*=qa_fail]):not([class*=claim_restrictions_small]))>li:nth-of-type(3)>span";
        public const string AwaitingReviewListIconPresentInGridCssTemplate = "ul.component_item_list>div>ul li.small_icon:not(.qa_pass):not(.qa_fail):not(.claim_restrictions_small)";
        public const string ClaimActionStatusCssSelector = "div.column_28>div";
        public const string RestrictionIconCssSelector = "li.claim_restrictions_small.position_left.null:not(.blank_badge)";


        #endregion

        #region XPath

        public const string InputFieldXPathTemplate = "//div[label[text()='{0}']]//input";
        public const string DropDrownInputFieldListValueXPathTemplate = "//div[label[text()='{0}']]/section/ul/li[text()='{1}']";
        public const string DropDownInputListByLabelXPathTemplate = "//label[text()='{0}']/../section//ul//li";
        public const string ClearCancelXPathTemplate = "//span[text()='{0}']";
        public const string GridForClaimSequenceXPathTemplate = "//ul[li[span[text()='{0}']]]";
        public const string QaResultIconByClaseqXPathTemplate = "//span[text()='{0}']/../../li[1]//li";
        public const string LastClaimSequenceOfGridXPath =
            "//ul[contains(@class,'component_item_list')]/div[last()]//li[3]/span";

        #endregion

        #endregion

        #region PAGEOBJECTS PROPERTIES

        //[FindsBy(How = How.CssSelector, Using = FilterCssSelector)]
        //public Link SortFilterIcon;

        #endregion

        #region PROTECTED PROPERTIES

        public override string PageTitle
        {
            get { return PageTitleEnum.QAClaimSearch.GetStringValue(); }
        }

        #endregion

        #region CONSTRUCTOR

        public QaClaimSearchPageObjects()
            : base(PageUrlEnum.QaClaimSearch.GetStringValue())
        {
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nucleus.Service.PageObjects.Default;
using Nucleus.Service.Support.Enum;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Elements;
using UIAutomation.Framework.Utils;



namespace Nucleus.Service.PageObjects.Provider
{
    public class ProviderSearchPageObjects : NewDefaultPageObjects
    {
        #region Css
        public const string DisabledFindButtonCssLocator = "div.is_disabled>button.work_button";
        public const string PageInsideTitleCssLocator = "label.page_title";
        public const string SideBarIconCssLocator = "span.sidebar_icon";
        public const string ButtonXPathTemplate = "//button[text()='{0}']";
        public const string FindButtonCssLocator = "button.work_button";
        public const string ClearFilterClass = "div.form_buttons  span.span_link";
        public const string ProviderSequenceValueXPath =
            "//li[label[contains(text(),'Prov Seq')]]/span";
        public const string LockIconByProviderSequenceXPathTemplate =
            "//ul[contains(@class,'component_item_list')]//ul[li/span[text()='{0}']]//li[contains(@class,'lock')]";
        public const string DashboardIconXPath = "//a[text()='Dashboard']";

        public const string ReviewIconByRowCssSelector = "ul.component_item_list>div:nth-of-type({0})>ul>li:nth-of-type(1)>ul>li:nth-of-type(4).eyeball.small_icon";
        public const string SearchResultScoreCssSelector = "ul.component_item_list>div:nth-of-type({0})>ul>li:nth-of-type(1)>ul>li:nth-of-type(3)";
        public const string ProviderScoreListCssSelector = "ul.component_item_list>div>ul>li>ul>li:nth-of-type(3)>span";

       
        public const string ScoreIconByRowXpath = "//ul[contains(@class,component_item_list)]/div[{0}]/ul/li[1]/ul/li[2]";
        public const string GridScoreLocatorXpathTemplate = " //ul[contains(@class,component_item_list)]/div[{0}]/ul/li[1]/ul/li[3]/span";
        public const string SelectedProviderSearchResultRowXpath = "//span[text()= '{0}']/../../..";
        public const string ProviderDetailsValueByLabelCssLocator = "li:has(>label:contains({0}))>span";
        public const string SearchListByProviderSeqSelectorTemplate = "//span[text()= '{0}']/../..";
        public const string ProviderDetailsHeaderCssSector = "section.column_40 div.component_header_left>label";
        public const string ProviderNameListCssSector = ".component_item_list ul.component_item_row>li:nth-of-type(2)>span";
        //public const string LoadMoreCssLocator = "span.load_more_data";

        public const string FilterOptionsListCssLocator = "li.appeal_search_filter_options >ul>li";
        public const string FilterOptionsIconCssLocator = "li.appeal_search_filter_options";
        public const string ReviewIconListCssTemplate = "ul.component_item_list>div>ul>li>ul>li:nth-of-type(4)";
        public const string ProviderProfileIconListCssTemplate = "ul.component_item_list>div:nth-of-type(-n+5)>ul>li:nth-of-type(8).provider_icon_small";
        public const string MedawareIconCssTemplate = "ul.component_item_list>div:nth-of-type({0})>ul>li.provider_icon_small.alerted";

        public const string ProviderHistoryIconCssLocator = "span.patient_claim_history.icon.toolbar_icon";
        public const string NotesIconCssLocator = "span.notes";
        public const string NoteListCssLocator = "div.note_row";
        public const string NotesContainerXpathLocator = "//label[text()='Provider Notes']/../../../section[2]/ul[contains(@class,component_item_list)]";
        public const string ProviderNoteRowAttributeCssLocatorTemplate = "div.note_row>ul>li:nth-of-type({0})>span";
        public const string CarrotIconCssSelector = "div.note_row>ul>li>ul>li>span";
        public const string CarrotIconRowwiseCssLocatorTemplate = "div.note_row:nth-of-type({0})>ul>li:nth-of-type(1)>ul>li>span";
        public const string CarrotDownIconCssSelector = "span.small_caret_down_icon";
        public const string ProviderNoNotesCssSelector = "p.empty_message";
        public const string ProviderLockIconCSSSelector = "span.medium_icon.locked";
        [FindsBy(How = How.CssSelector, Using = ReviewIconByRowCssSelector)]
        public ImageButton ReviewIcon;
        public const string ProviderSearchExportIconCssSelector = ".print_icon";
        public const string DisabledExportIconCss = "li.is_disabled>span.print_icon";
        public const string EnabledExportIconCss = "li.is_active>span.print_icon";

        //[FindsBy(How = How.CssSelector, Using = PageHeaderCssLocator)]
        //public TextLabel PageHeader;

        //[FindsBy(How = How.XPath, Using = DashboardIconXPath)]
        //public ImageButton DashboardIcon;
        #endregion

        #region PROTECTED PROPERTIES
        public override string PageTitle
        {
            get { return PageTitleEnum.ProviderSearch.GetStringValue(); }
        }
        #endregion

        #region CONSTRUCTOR

        public ProviderSearchPageObjects()
            : base(PageUrlEnum.ProviderSearch.GetStringValue())
        {
        }

        #endregion
    }
}

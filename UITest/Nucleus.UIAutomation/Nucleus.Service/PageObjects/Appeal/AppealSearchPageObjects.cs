using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nucleus.Service.PageObjects.Default;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Utils;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Elements;

namespace Nucleus.Service.PageObjects.Appeal
{
    public class AppealSearchPageObjects:NewDefaultPageObjects
    {
        #region CONSTANT

        #endregion
        
        #region TEMPLATE

        public const string InputFieldLabeXPathTemplate = "//label[text()='{0}']";
        public const string SearchInputFieldCssTemplate =
            "section.component_sidebar_panel span>form>div:nth-of-type({0}) input";

        public const string SearchInputFieldXpathTemplate = "//label[text()='{0}']/../section/div/input";
        public const string SearchInputListValueXPathTemplateGeneric =
           "//label[text()='{0}']/../section//ul//li[text()='{1}']";

        public const string DisabledSearchInputFieldCssTemplate =
            "section.component_sidebar_panel span>form>div:nth-of-type({0}) input[disabled='']";
        public const string SearchLabelCssTemplate =
            "section.component_sidebar_panel span>form>div:nth-of-type({0}) label";

        public const string SearchInputLabelXpathTemplate =
            "//section[contains(@class,'component_sidebar_panel')]/span/form/div/label[text()='{0}']";//"[not(ancestor::span[@class='required_asterisk'])]";
        public const string DropDownSearchInputLabelXpathTemplate =
            "//section[contains(@class,'component_sidebar_panel')]/span/form/div/div/label[text()='{0}']";
        public const string SearchInputListValueXPathTemplate =
            "//section[contains(@class,'component_sidebar_panel')]/span/form/div[{0}]//section/ul//li[text()='{1}']";

        public const string QuickSearchListValueXPathTemplate =
            "//section[contains(@class,'component_sidebar_panel')]/span/form/div[1]//section/ul/li[text()='{0}']";

        public const string AppealSequenceValueCssTemplate =
            "ul.component_item_list>div:nth-of-type({0}) li.action_link>span";
        public const string ClientSelectValueXPathTemplate =
            "//section[contains(@class,'component_sidebar_panel')]/span/form/div[3]/div//section/ul//li[text()='{0}']";

        public const string LockIconByAppealSequenceXPathTemplate =
            "//ul[contains(@class,'component_item_list')]//ul[li/span[text()='{0}']]//li[contains(@class,'lock')]";

        public const string UrgerIconCssTemplate = "ul.component_item_list>div:nth-of-type({0}) li.field_error";
        public const string UrgentIconByAppealSeqCssTemplate = "div.component_item:has(span:contains({0}))div.component_item:has(li.field_error)";

        public const string AppealSearchResultCssTemplate =
            "ul.component_item_list>div:nth-of-type({0})>ul>li:nth-of-type({1}) span";
 
        public const string DropDownToggleIconCssTemplate = "section.component_sidebar_panel span>form>div:nth-of-type({0}) section.select_wrap span.select_toggle";
        public const string DropDownToggleValueCssTemplate = "section.component_sidebar_panel span>form>div:nth-of-type({0}) section.select_wrap >ul>li.option";
        public const string MultiDropDownToggleIconCssTemplate = "section.component_sidebar_panel span>form>div:nth-of-type({0}) section.multi_select_wrap span.select_toggle";
        public const string MultiSelectListedDropDownToggleValueCssTemplate = "section.component_sidebar_panel span>form>div:nth-of-type({0}) section.multi_select_wrap >ul >section.list_options> li.option";
        public const string MultiSelectAvailableDropDownToggleValueCssTemplate = "section.component_sidebar_panel span>form>div:nth-of-type({0}) section.multi_select_wrap >ul >section.available_options> li.option";

        public const string AppealSearchResultListCssTemplate =
            "ul.component_item_list>div>ul>li:nth-of-type({0}) span";

        public const string AdjustCssTemplate = "ul.component_item_list>div:nth-of-type({0}) li.appeal_adjust>span";
        public const string PayCssTemplate = "ul.component_item_list>div:nth-of-type({0}) li.appeal_pay>span";
        public const string DenyCssTemplate = "ul.component_item_list>div:nth-of-type({0}) li.appeal_deny>span";
        public const string NoDocsCssTemplate = "ul.component_item_list>div:nth-of-type({0}) li.value_badge>span";
        public const string AppealLetterIconCssTemplate = "ul.component_item_list>div:nth-of-type({0}) li.appeal_letter";

        public const string ApealLetterIconByAppealLevelXPathTemplate =
            "//ul[li[contains(@class,'secondary_badge')][span[text()='{0}']]]//li[contains(@class,'appeal_letter')]";

        public const string ApealLetterIconForEmptyAppealLevelXPathTemplate =
            "//ul[contains(@class,'component_item_row ')]/li[12]/span[text()='']/ancestor::ul[contains(@class,'component_item_row ')]//li[contains(@class,'appeal_letter')]";
        public const string BoldRedColorDueDateCssTemplate = "li.due_date_before_current_date";
        public const string NonBoldOnlyRedColorDueDateCssTemplate = "li.due_date_equal_current_date";
        public const string OrangeColorDueDateCssTemplate = "li.due_date_for_tomorrow";

        public const string BlackColorDueDateXPathTemplate =
            "//ul[contains(@class,'component_item_list')]/div[{0}]/ul/li[2][not(contains(@class,'due_date_before_current_date')) and not(contains(@class,'due_date_equal_current_date')) and not(contains(@class,'due_date_for_tomorrow'))]";

        public const string AppealSearchListRowSelectorTemplate = "ul.component_item_list div:nth-of-type({0})>ul";
        public const string AppealDetailsHeaderXpath =
            "//section[contains(@class,'search_list')]/section/div/label[text() = 'Appeal Details']";

        public const string AppealDetailContentLabelXpathTemplate =
            "(//section[contains(@class,'search_list')]/section/div/label[text() = 'Appeal Details']/../../../section[2]/div/ul[{0}]/li/label)[{1}]";
        public const string AppealDetailContentValueXpathTemplate =
            "(//section[contains(@class,'search_list')]/section/div/label[text() = 'Appeal Details']/../../../section[2]/div/ul[{0}]/li/span)[{1}]";

        #endregion

        #region ID

        public const string PageErrorPopupModelId = "nucleus_modal_wrap";
        public const string PageErrorCloseId = "nucleus_modal_close";
        public const string PageErrorMessageId = "nucleus_modal_content";
       
        #endregion

        #region XPATH
        public const string SearchIconInActiveXpath = "//li[span[contains(@class,'sidebar_icon' )] and contains(@class,'not_selected')]";
        public const string AppealSequenceByStatusOrAppealXPathTemplate =
           "//ul[contains(@class,'component_item_list')]/div/ul[li/span[text()='{0}']]/li[contains(@class,'action_link')]/span";

        public const string AssignedToByAppealSequenceXPathTemplate =
            "//ul[contains(@class,'component_item_list')]/div/ul[li/span[text()='{0}']]/li[8]";

        //public const string FieldErrorIconByLabelXpathTemplate =
        //    "//label[text()='{0}']/..//span[contains(@class,'field_error')]";


        #endregion
        #region Class
        public const string AppealLevelBadgeCssTemplate = ".secondary_badge span";
        public const string AppealLevelBadgeValueForMRRAppealType = "//span[text()='MRR']/../following-sibling::li[contains(@class,'secondary_badge')]/span";

        public const string UnlockAppealSequenceListWithStatusNew =
            "ul:not(:has(li.lock)):has(span:contains(New)) li.action_link>span";//jquery
        //public const string LoadMoreCssLocator = "div.load_more_data span";
        public const string AppealSequenceListCssSelector = "li.action_link >span";
        public const string UrgentListCssLocator =
            "ul.component_item_list>div>ul>li:nth-of-type(1)>ul>li:nth-of-type(1)";
        public const string FilterOptionsListCssLocator = "li.appeal_search_filter_options >ul>li";
        public const string FilterOptionsIconCssLocator = "li.appeal_search_filter_options";
        public const string NoMatchingRecordFoundCssSelector = "p.empty_search_results_message";
        //public const string FieldErrorIconCssLocator = "span.field_error:not([style*='none'])";
        public const string AdvancedSearchIconCssLocator = "span.advanced_filter_icon ";


        public const string AppealSequenceInputFieldCssLocator =
            "section.component_sidebar_panel span>form>div:nth-of-type(2) input";
        public const string FindButtonCssLocator = "button.work_button";
        public const string DisabledFindButtonCssLocator = "div.is_disabled>button.work_button";
        public const string EnabledFindButtonCssLocator = "div.is_enabled>button.work_button";

        public const string FindAppealSectionCssLocator = "section.component_sidebar.is_slider:not(.is_hidden)";
        public const string QuickSearchCssLocator =
            "section.component_sidebar_panel span>form>div:nth-of-type(1) input";
        public const string ClaimSequenceInputFieldCssLocator =
            "section.component_sidebar_panel span>form>div:nth-of-type(5) input";
        public const string ClaimSequenceInputFieldInCotivitiUserCssLocator =
            "section.component_sidebar_panel span>form>div:nth-of-type(4) input";
        public const string ClientSelectCssLocator =
            "section.component_sidebar_panel span>form>div:nth-of-type(3) input";
        public const string ClearFilterClass = "div.form_buttons  span.span_link";
        public const string SearchIconCssLocator = "span.sidebar_icon.toolbar_icon";
        public const string SearchListSectionCssLocator = "section.component_left.search_list";
        public const string SearchResultEmptyMessageCssLocator = "section.component_left.search_list section.component_content p.empty_message";
        public const string AppealSearchResultCssLocator = "ul.component_item_list div ul.component_item_row";
        public const string PreviouslyViewedAppealSequenceCssLocator = " ul.worklist_list li span:nth-of-type(1)";
        public const string ReturnToAppealCssLocator = " ul.worklist_list li span:nth-of-type(1)";
        public const string PreviouslyViewedAppealLinkCssLocator = " ul.worklist_list li";
        public const string AppealSearchQuickLaunchTile = "div.MainContent a[href='/app/#/clients/SMTST/appeals']";
        public const string ClearCancelXPathTemplate = "//span[text()='{0}']";
        public const string AppealSearchResultByRowColCssTemplate =
           "ul.component_item_list>div:nth-of-type({1})>ul>li:nth-of-type({0}) span";

       
        #endregion

       
        #region ID



        #endregion

        #region XPATH


        #endregion


        #region PROTECTED PROPERTIES

        public override string PageTitle
        {
            get { return PageTitleEnum.AppealSearch.GetStringValue(); }
        }

        
        #endregion

        #region CONSTRUCTOR

        public AppealSearchPageObjects()
            : base(PageUrlEnum.AppealSearch.GetStringValue())
        {
        }

        #endregion
    }
}

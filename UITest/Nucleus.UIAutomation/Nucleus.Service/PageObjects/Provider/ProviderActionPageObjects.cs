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
    public class ProviderActionPageObjects : NewDefaultPageObjects
    {
        #region Private Properties

        #region Constants

        #region TEMPLATE

        

        #endregion

        #region ID


        public const string PageErrorPopupModelId = "nucleus_modal_wrap";
        private const string PageErrorCloseId = "nucleus_modal_close";
        #endregion

        #region XPATH
        
        public const string ConditionExposureButtonActiveXpath = "//li[@title='Condition Exposure']";

        public const string AddCIUReferralIconWithLabelXpath =
            "//label[@title='CIU Referral']/../../li[contains(@class, 'add_icon')]";

        //public const string RedRequiredExclamationPointXpath = "//label[text()='{0}']/span[contains(@class,'field_error')]";

        public const string RequiredAsteriskXpath = "//label[text()={0}]/span[contains(@class, 'required_asterisk') and contains(@style, 'display: none')]";

        public const string SelectedPatternCategoryOptionsXpath = "//section[@class = 'selected_options']/li";

        #endregion

        #endregion

        #endregion

        #region Provider Condtions

        public const string ConditionIdListInProviderConditionsCssLocator =
            "div.listed_provider_condition li.action_link >span";
        public const string TriggeredDaeInProviderConditionsCssLocator =
            "div.listed_provider_condition >li>ul:nth-of-type(1)>li:nth-of-type(2)>span";

        #endregion

        #region PAGEOBJECT PROPERTIES

        public const string ProviderSequenceCssSelector = "div.action_provider_details>ul>div:nth-of-type(1)>div";

        public const string BasicProviderDetailsValue = "//div[contains(@class,'action_provider_details')]/ul/div/div";

        public const string BasicProviderDetailsLabel = "//div[contains(@class,'action_provider_details')]/ul/div/label";

        public const string GoogleSearchIconCssSelector = "div[title = 'Google search']";

        public const string GoogleSearchInputCssSelector = "input.gLFyf.gsfi";

        public const string GooglePopupPageTitle = "head:nth-of-type(1)>meta+title";

        #region condition detail 

        public static string EllipsesOnConditionDetailRowXPathTemplate =
            "//section[contains(@class,'condition_audits')]/section[2]/ul/li[{0}]//ul[contains(@class,'ellipse')]/li";
        #endregion 

        #region code selection manage code of concern

        public static string CodeSelectionLabelXPath="//div[contains(@class,'search_input_component')]/../../li[1]/header";
        public static string CodesOfConcernLabelXPath = "//h2[text()='Codes of Concern']";
        public static string CodesOfConcernByCodeXPathTemplate = "//div[text()='{0}']";
        public static string CodesOfConcernCheckBoxByCodeXPathTemplate = "//div[text()='{0}']/../span";
        public static string CodesOfConcern5ColumnCssLocator = "//h2[text()='Codes of Concern']/../../../ul[last()]";

        public static string ConditionIdInManageCodeOfConcernByConditionIdXPathTemplate =
                "//section[contains(@class,'action_provider_conditions ')][.//label[text()='Manage Codes of Concern']]//li[contains(@class,'is_editing ')][*//span[text()='{0}']]";

        public static string CodeOfConcernInCodeSelectionCssLocator =
            "li.selected_codes_of_concern div.component_checkbox >div";

        public static string CancelLinkOnManageCodeOfConcern =
            "section.action_provider_conditions:nth-of-type(2) span.span_link";

        #endregion
        #region CIU Referral

        public const string CIUReferralCreatedDeteCssTemplate =
            "li.ciu_referrals >ul:nth-of-type(2)>li>ul:nth-of-type(1)>li:nth-of-type(2)>span";

        public const string CIUReferralDetailByRowLabelCssTemplate =
            //"//li[contains(@class,'ciu_referrals ')]/ul[2]/li[{0}]//label[text()='{1}']/../span";
            "li.ciu_referrals>ul:nth-of-type(2)>li:nth-of-type({0}) li:contains({1})>span";//can find element when run using jquery

        public const string ProviderDetailSectionCssSelector =
            "div#top_section>div:nth-of-type(2)>div:nth-of-type(2)>div";

        public const string ProviderDetailSelectionByRowCssSelector = ProviderDetailSectionCssSelector + ">section>ul.component_item_list >li:nth-of-type({0})";

        public const string ProviderDetailExpandedSectionByRowCssSelector = ProviderDetailSelectionByRowCssSelector + ".is_expanded";

        public const string AddressHyperlinkInProviderDetailSectionCssSelector = ".clipped.span_link";

        public const string DeleteCIUReferralIconCssTemplate =
            "li.ciu_referrals >ul:nth-of-type(2)>li:nth-of-type({0}) li.small_delete_icon";

        public const string CIUReferralRecordRowCssSelector = "li.ciu_referrals >ul:nth-of-type(2)>li";

        public const string AddCIUReferralRecordCssSelector = "li.add_icon";

        public const string CreateCIUReferralFormXPath = "//header[.//text()[contains(.,'Create CIU')]]/../../..";

        public const string CreateCIUReferralInputCssTemplate = "li:contains({0})>input";

        public const string SaveCIUReferralButtonCssSelector = "section#provider_entity_details button.work_button";

        public const string CancelCIUReferralLinkCssSelector = "section#provider_entity_details div.form_buttons span.span_link";

        public const string PatternCategoryInputCssSelector = "div:has(>label:contains(Pattern Category))>section>input";

        public const string PatternCategoryValueCssSelector =
            "div:has(>label:contains(Pattern Category)) li:contains({0})";

        public const string NoCiuReferralCssSelector = "span[title=\"There are no CIU referral details available.\"]";

        public const string InputCIUReferralCssTemplate = "div:has(>label:contains({0})) input , li:has(>label:contains({0})) input";
        public const string CiuReferralInputFieldXpathTempalte = "//label[text()[contains(.,'{0}')]]/..//input";
        public const string ProviderDetailSectionToggleCssSelector = "section#provider_entity_details:has(>ul)";

        public const string Jquery = "$('section.selected_options li:nth-of-type({0})').text()";

        public const string CiuReferralRecordRowCssSelector = "li.ciu_referrals >ul:nth-of-type(2)>li";

        #endregion

        #region Template
        public const string ProviderExposureCountValueCssTemplate =
           "div.listed_exposure>div>ul:nth-of-type({0})>div:nth-of-type({1})>div>span:nth-of-type(1)";
        public const string ProviderExposureAvgValueCssTemplate =
            "div.listed_exposure>div>ul:nth-of-type({0})>div:nth-of-type({1})>div>span:nth-of-type(2)";

        public const string ProviderDetailByLabelXPathTemplate = "//div[label[text()[contains(.,'{0}')]]]/div";
        #endregion


        #region ID



        #endregion

        #region XPATH

        public const string ProviderExposureSectionCssSelector = "div#top_section>div:nth-of-type(1)>div:nth-of-type(2)";

        public const string SelectedCodeOfConcernDivCssTemplate =
            "li.selected_codes_of_concern div.toggle_select>span.active";

        public const string ActiveRetriggerPeriodValueCssLocator = "li.active.time_selector";
        public const string QuickNoActionXIconCssLocator = "span.small_delete_icon";
        public const string EditActionConditionCssSelector = "li.is_active > span.toolbar_icon.edit_icon";
        public const string NextActionCssSelector = "li.is_active > span.toolbar_icon.next.icon";
        public const string DisabledEditActionConditionCssSelector = "li.is_disabled > span.toolbar_icon.edit_icon";
        public const string ProvCondListEditIconXpathSelector = "(//li[@title= 'Action Condition']/span)[{0}]";//"[title='Action Condition'] span:nth-of-type({0})";
        public const string PrividerName = "div.provider_title>label:nth-of-type(2)";
        public const string QuadrantTitleNameCssLocator = "section.component_header label.component_title";
        public const string ToolbarButtonsClass = ".provider_action >.component_left > .component_header > .component_header_right";

        public const string ComponentActionProviderConditionCssLocator = "section.component.action_provider_conditions";

        public const string Top10ProcCodesIconCssLocator = ".top_codes";

        public const string Top10ProcCodesByOptionCssLocator = ".top_codes + ul > li:nth-of-type({0}) span";
        public const string Top10ProcCodesRow = "//section[contains(@id,'provider_entity_details')]/ul/div[li[not(contains(@class,'ciu_referrals'))]]";

        public const string Top10ProcCodeProcedureValueByRowCssLocator = "#provider_entity_details > ul > div:nth-of-type({0}) > li:nth-of-type(1) > span";
        public const string Top10ProcCodeShortDescValueByRowCssLocator = "#provider_entity_details > ul > div:nth-of-type({0}) > li:nth-of-type(2) > span";
        public const string Top10ProcCodeCountLabelByRowCssLocator = "#provider_entity_details > ul > div:nth-of-type({0}) > li:nth-of-type(3) > label";
        public const string Top10ProcCodeCountValueByRowCssLocator = "#provider_entity_details > ul > div:nth-of-type({0}) > li:nth-of-type(3) > span";
        public const string Top10ProcCodeBilledLabelByRowCssLocator = "#provider_entity_details > ul > div:nth-of-type({0}) > li:nth-of-type(4) > label";
        public const string Top10ProcCodeBilledValueByRowCssLocator = "#provider_entity_details > ul > div:nth-of-type({0}) > li:nth-of-type(4) > span";
        public const string Top10ProcCodePaidLabelByRowCssLocator = "#provider_entity_details > ul > div:nth-of-type({0}) > li:nth-of-type(5) > label";
        public const string Top10ProcCodePaidValueByRowCssLocator = "#provider_entity_details > ul > div:nth-of-type({0}) > li:nth-of-type(5) > span";

        public const string Top10ProcCodeCountValueCssLocator = "#provider_entity_details > ul > div > li:nth-of-type(3) > span";
        public const string Top10ProcCodeBilledValueCssLocator = "#provider_entity_details > ul > div > li:nth-of-type(4) > span";
        public const string Top10ProcCodePaidValueCssLocator = "#provider_entity_details > ul > div > li:nth-of-type(5) > span";

        public const string ProviderDetailsIconCssLocator = ".provider_details_info";

        public const string ConditionExposureIconCssLocator = ".condition_exposure";

        public const string ConditionExposureEmptyMessage = ".listed_exposure p.empty_message";

        public const string ConditionExposureLabelTemplate =
            "section.listed_exposure > section + section > div > ul:nth-of-type({0}) > div:nth-of-type({1}) > header";

        public const string ConditionExposureValueTemplate =
            "section.listed_exposure > section + section > div > ul:nth-of-type({0}) > div:nth-of-type({1}) > div > span";

        public const string ConditionExposureDetailsSectionCssSelector =
            "section.listed_exposure > section + section>div";


        
        public const string SelectCodeOfConcernForActioningCssLocatorTemplate =
            "section.action_provider_conditions > section:nth-of-type(2) > ul > div:nth-of-type({0}) > li";

        public const string ReasonCodeComboBoxXPath = "//div[contains(@class,'reason_code')]/section/div/input";

        public const string ReasonCodeDropDownXPath = "//div[contains(@class,'reason_code')]/section/div/span";

        public const string ReasonCodeListOptionsXPath = "//div[contains(@class,'reason_code')]/section/ul/li";

        public const string ActionListOptionsXPath = "//label[text()='Action']/../section/ul/li";
        
        public const string ReasonCodeListOptionSelectorsXPathTemplate = "//div[contains(@class,'reason_code')]/section/ul/li[text()='{0}']";
        public const string FirstReasonCodeOptionSelectorsXPath = "//div[contains(@class,'reason_code')]/section/ul/li[2]";

        public const string ActionComboInputValueXPathTemplate =
            "//ul[contains(@class,'select_options')]/li[text()='{0}']";
        public const string ActionComboInputXpathSelector = "//label[text()='Action']/../section//input";
        public const string ActionComboInputToggleXpathSelector = "//label[text()='Action']/../section//span[contains(@class,'select_toggle')]";
        public const string ActionComboBoxXPath = "//section[contains(@class,'action_provider_form')]//div[label[text()[contains(.,'Action')]]]//input";

        public const string ActionSelectedOptionCssSelector =
            "section.action_provider_form > form > ul:nth-of-type(2) > li:nth-of-type(1) > div > section > ul > li.is_active";

        public const string ReasonCodeSelectedOptionCssSelector = "div.reason_code > section > ul > li.is_active";

        public const string ActionXPathTemplate = "//div[contains(@class,'select_component')]/section/ul/li[text()='{0}']";

        public const string RetriggerTimePeriodDisabledCssSelector = "div.time_span_selector.is_disabled ul.disabled.time_selectors";
        public const string RetriggerTimePeriodEnabledCssSelector = "ul:not(.disabled) >  li.time_selector";

        public const string VisibleToClientCheckBoxXpath = "//div[text()='Visible To Client']";
        public const string VisibleToClientCheckBoxCheckedXpath =
            "//div[text()='Visible To Client']/../span[contains(@class,'active')]";

        public const string FirstCodeofConcernCheckBoxCheckedCssLocator =
            "li.selected_codes_of_concern span.active";

        public const string CancelActionCssSelector = "section.action_provider_conditions div.form_buttons span.span_link";

        

        public const string SearchConditionCssSelector = "span.add_icon[title='Create User Specified Condition']";

        public const string SearchConditionInputBoxCssSelector = "div.search_form > input";

        public const string SearchConditionSmallCssSelector = "div.small_icon.search_icon";

        public const string SearchConditionFirstResultXPath = "//h2[text()='Search Results']/../li/ul";

        public const string SelectedConditionFirstXPath = "//h2[text()='Selected Conditions']/../li/ul/li/span";

        public const string SelectedConditionByCodeXPath =
            "//h2[text()='Selected Conditions']/../li[ul/li/span[contains(@title,'{0}')]]";

        public const string SelectedConditionCssSelector = "section.action_provider_form li.is_selectable";

        public const string BackLinkCssSelector = "span.browser_back";

        public const string ValidationNoticeModalPopupId = "nucleus_modal_wrap";

        public const string ValidationNoticePopupContentDivId = "nucleus_modal_content";

        public const string ValidationNoticePopupCloseId = "nucleus_modal_close";

        public const string BasicProviderDetailsDivCssSelector = "div.action_provider_details";

        public const string ProviderExposureCssSelector = "div.listed_exposure";

        public const string ProviderExposureDataItemsCssSelector = "div.listed_exposure span.data_bar_item";

        public const string ProviderDetailsId = "provider_entity_details";

        public const string QuickNoActionIconCssSelector = "li[title='Quick No Action']>span";

        public const string FilterConditionsIconCssSelector = "span.filters";

        public const string FilterConditionsOptionTemplateXPath = "//li[contains(@class,'condition_options')]/ul/li[{0}]/span";

        public const string FilterConditionsOptionListXPath = "//li[contains(@class,'condition_options')]/ul";

        public const string ConditionClientActionXPath = "//label[text()='Client Action:']/../span";

        public const string ConditionIdInProviderConditionsCssSelectorTemplate =
            "section.action_provider_conditions > section.component_content > ul > div:nth-of-type({0}) > li > ul:nth-of-type(1) > li:nth-of-type(3)";
        

        public const string ProviderConditionXPathTemplate =
            "//li[ul/li/span[text()[contains(.,'{0}')]]]";

        public const string ProviderConditionCssSelector = "section.action_provider_conditions > section.component_content > ul > div > li";

        public const string ProviderConditionCssSelectorByRowTemplate = "section.action_provider_conditions > section.component_content > ul > div:nth-of-type({0}) > li";

        public const string PrivderCodntionSelectorByConditionIdXPathTemplate =
            "//li[ul/li/span[text()[contains(.,'{0}')]]]";
        public const string ProviderConditionConditionCodeCssSelectorByRowTemplate = "section.action_provider_conditions > section.component_content > ul > div:nth-of-type({0}) > li>ul:nth-of-type(1)>li:nth-of-type(3)>span";
        public const string ProviderConditionConditionCodeDescriptionXPath = "//li[span[text()[contains(.,'Description')]]]";

        public const string ProviderCondtionIdPopupContentCssTemplate = "div.content li:nth-of-type({0})";

        public const string ProviderConditionLeftColumnValuesForFieldGivenAndRowTemplate =
            "(//label[text()[contains(.,'{0}')]]/../span)[{1}]";

        public const string ActionRequiredBadgeIconCssSelector = "div.listed_provider_condition:nth-of-type({0}) li.action_required.has_badge";
        public const string ConditionDetailsIconCssSelector = "a.condition_details";
        public const string ConditionDetailsSectionXPathLocator =
            "//section[contains(@class,'condition_audits')]/section[2]/ul";
        public const string ConditionDetailsEmptyConditionNoteXPathLocator =
            "//section[contains(@class,'condition_audits')]/section[2]/ul/p[1]";

        public const string ConditionDetailsConditionNoteXpathLocator =
            "//label[text()[contains(.,'Condition Note')]]/../span";
        public const string ConditionDetailsEmptyAuditRecordXPathLocator =
            "//section[contains(@class,'condition_audits')]/section[2]/ul/p[2]";
        public const string ConditionDetailsRowXPathTemplate =
            "//section[contains(@class,'condition_audits')]/section[2]/ul/li[{0}]";
        public const string ConditionDetailsRowsXPathTemplate =
            "//section[contains(@class,'condition_audits')]/section[2]/ul/li";
        public const string ConditionDetailsRowSelectorXPathTemplate =
            "//section[contains(@class,'condition_audits')]/section[2]/ul/li[{0}]/ul[{1}]";
        public const string ConditionDetailsFirstSecondRowSelectorXPathTemplate =
            "//section[contains(@class,'condition_audits')]/section[2]/ul/li[{0}]/ul[position()<3]";
        public const string ConditionDetailsCodeOfConcernXPathTemplate =
            "//section[contains(@class,'condition_audits')]/section[2]/ul/li[{0}]/ul[last()]";
        public const string ConditionDetailsAssociatedProcCodeListXPathTemplate =
            "//section[contains(@class,'condition_audits')]/section[2]/ul/li[{0}]/ul[last()]//ul/li";

        public const string ConditionDetailsActionDateValueXPathTemplate =
            "//section[contains(@class,'condition_audits')]/section[2]/ul/li[{0}]/ul[1]/li[2]/span";

        public const string ConditionDetailsReasonLabelXPathTemplate =
            "//section[contains(@class,'condition_audits')]/section[2]/ul/li[{0}]/ul[1]/li[3]/label";
        public const string ActionCondtionInputFieldByLabelXPathTemplate = "//div[label[contains(., '{0}')]]//input";

        public const string ProviderFlaggingLabelCssSelector = "div.provider_flagging_controls > div > div";
        public const string ProviderFlaggingCheckBoxCssSelector = "div.provider_flagging_controls > div > span.icon.checkbox";
        public const string OkConfirmationCssSelector = "div#confirmation_links > div#complete_link";
        public const string CancelConfirmationCssSelector = "div#confirmation_links > span.span_link.modal_close";

        public const string ExclamationProfileIndicatorCssSelector = "div.icon.profile_indicator.alerted";
        public const string ProfileIndicatorCssSelector = "div.icon.profile_indicator";
        public const string ProfileReviewIndicatorCssSelector = "div.icon.toolbar_icon.eyeball ";
        public const string ConfirmationPopupModalId = "nucleus_modal_wrap";

        public const string ConfirmationPopupModalMessageCssSelector =
            "div#nucleus_modal_wrap > div#nucleus_modal_content_wrap > #nucleus_modal_content";

        public const string SmallAddNoteIconCssSelector = " span.small_add_notes_icon";
        public const string SmallNoteIconCssSelector = "span.small_icon.alert_notes_icon";
        public const string SmallViewNoteIconCssSelector = "span.small_notes_icon";

        public const string AddNoteIconCssSelector = "span.add_notes";
        public const string NoteIconCssSelector = "ul.toolbar>li:nth-of-type(1)>span.add_notes, ul.toolbar>li:nth-of-type(1)>span.notes";
        public const string SearchIconCssSelector = "ul.toolbar>li:nth-of-type(3)>span.search_icon";
        public const string ViewNoteIconCssSelector = "span.notes";
        public const string AddNoteSectionCssSelector = "section.note_component";


        public const string ConditioninSelectConditionsForActioningColumnCssSelector =
           "li.component_item.is_selectable.is_editing";

        public const string ConditionIdInSelectConditionsForActioningColumnCssSelectorTemplate =
            "div:nth-of-type({0}) > li.component_item.is_selectable.is_active > ul:nth-of-type(1) > li:nth-of-type(2)";

        public const string ConditionFromEditProviderActionCssTemplate = "div > section:not(.is_hidden):nth-of-type({0}) ul:nth-of-type({1}) li.action_link>span";

        public const string ManageCodesofConcernColumnCssSelector =
            "div#bottom_section>div > section:nth-of-type(2)";

        public const string TriggerDateCssSelector = "ul.single_item_row>li:nth-of-type(1)>span";

        public const string ConditionTitleCssSelector = "ul.single_item_row>li:nth-of-type(2)>span";

        public const string ConditionDescriptionCss = "section.action_provider_conditions p.empty_message";

        public const string AllCodesofConcernCheckCssSelector = "section.action_provider_conditions >section:nth-of-type(2)>ul>li>section li.selected_codes_of_concern  span";
        public const string SingleCodesofConcernCheckBoxCssTemplate = "li.selected_codes_of_concern li:nth-of-type({0}) span";

        public const string ActionAllConditionsCheckBoxCssSelector = "section.action_provider_form span.checkbox";

        public const string SelectAllCodesofConcernCheckBoxCssSelector = "li.apply_all_checkbox span.checkbox";

       
        public const string ReTriggerPeriodInputCssSelector = "input.manual_input";

        public const string DecisionRationaleCssSelector = ".cke_wysiwyg_frame.cke_reset";

        public const string SaveActionButtonCssSelector =
            "section.action_provider_form form div.form_buttons button.work_button";
        //"section.action_provider_conditions div.form_buttons > button.work_button";
        public const string DecisionRationaleFormattingOptionCssSelector = "a.cke_button";

       public const string AllCodesofConcernCssSelector =
            "section.action_provider_conditions >section:nth-of-type(2)>ul>li>section li.selected_codes_of_concern li span ";

       public const string CodesofConcernRowCssTemplate = "div#bottom_section>div>section:nth-of-type(2)>section:nth-of-type(2) >ul:nth-of-type(1)>li:nth-of-type({0})";

       public const string CodesofConcernRowCssLocator = "div#bottom_section>div>section:nth-of-type(2)>section:nth-of-type(2) >ul:nth-of-type(1)>li";
       public const string CodesofConcernRowCheckboxCssLocator = "div#bottom_section>div>section:nth-of-type(2)>section:nth-of-type(2) >ul:nth-of-type(1)>li span.checkbox";

       // public const string AdjacentCheckBoxofCodeofConcernCss =
        // " #bottom_section div:nth-of-type(1) section:nth-of-type(2) section:nth-of-type(2) ul:nth-of-type(2)  li div span";

        public const string MoreOptionsCssSelector = ".filters.icon.toolbar_icon";

        public const string AllTrigerredConditionsCssSelector =
            "ul.is_visible.option_list > .ember-view.list_option.is_active.not_selected:nth-of-type(3)";
        public const string ManageCodesofConcernColumnScrollDivCssSelector =
            "div#bottom_section>div> section:nth-of-type(2)>section:nth-of-type(2)";
        

        public const string ListCodeofColumCssLocator = "section.action_provider_conditions >section:nth-of-type(2)>ul>li>section ul:nth-of-type(2)";

        public const string EmptyMessageOnUserAddedContionCssLocator = "ul.no_listed_columns p.empty_message";

        public const string SearchResultSectionCssLocator = "ul.no_listed_columns";

        public const string SearchResultValueCssLocator = "ul.no_listed_columns span";

        public const string UserAddedConditionSectionCssLocator =
            "div#bottom_section>div section:nth-of-type(2)>section:nth-of-type(2) section.form_component";

        public const string ClearUserAddedConditionSectionCssLocator =
            "div#bottom_section>div section:nth-of-type(2)>section:nth-of-type(2) >section button.work_button";

        public const string MatchingCondtionListCssLocator = "form>ul:nth-of-type(2)>li>ul>li";

        public const string NewConditiontionCodeSectionCssLocator =
            "div#bottom_section>div > div >section:nth-of-type(2)>section:nth-of-type(2)>ul";

        public const string NewAddedCodeCheckboxCssTemplate =
            "section.action_provider_conditions >section:nth-of-type(2)>ul>li>section ul>li:nth-of-type({0}) span";

        public const string SelectAllXPath = "//div[text()='Select All']";

        public const string NewAddedCodeCssLocator =
            "section.action_provider_conditions >section:nth-of-type(2)>ul>li>section ul>li span";

        public const string ClearButtonXPath = "//button[text()='Clear']";
        public const string CancelButtonOnUserAddedCondition = " //span[text()='Cancel']";

        public const string GenerateRationaleXPath = "//button[text()='Generate Rationale']";
        public const string GenerateRationaleDisabledXPath = "//button[contains(@class,'is_disabled') and text()='Generate Rationale' ]";

        public const string GenerateRationaleSectionCssLocator = "section.rationale_modal";

        public const string BillingSummaryCssLocator =
            "section.rationale_modal>ul:nth-of-type(1) >li:nth-of-type(2) input";

        public const string GenerateRationaleFormInputFieldXPathTemplate =
            "//ul//div[label[text()[contains(.,'{0}')]]]/input";

        public const string GenerateRationaleFormInputFieldByTabNoXPathTemplate =
            "(//div[label]/input)[{0}]";
        public const string GenerateRationaleFormSelectFieldByTabNoXPathTemplate =
            "(//div/section/div/input)[{0}]";
        public const string RefreshContentCssLocator = "section.rationale_modal  button:nth-of-type(1)";
        public const string FinishCssLocator = "section.rationale_modal  button:nth-of-type(1)";
        public const string CancelLinkOnGenerateRationaleCssLocator = "section.rationale_modal span.span_link";

        public const string GenerateRationaleInputFieldXPathTemplate =
            "//div[label[text()='{0}']]/input";
        public const string GenerateRationaleSelectFieldXPathTemplate =
            "//div[label[text()='{0}']]/section/div/input";
        public const string GenerateRationaleFrameInputXPathTemplate =
            "//div[label[text()='{0}']]/div/div/div/iframe";

        public const string GeneratedRationaleIframCssLocator =
            "section.rationale_modal >ul:nth-of-type(2) iframe.cke_wysiwyg_frame ";

        public const string GeneratedRationaleNoteFieldPTagXPathTemplate = "//body[//*[text()[contains(.,'PROVIDER')]]]/p[{0}]";
         public const string GeneratedRationaleNoteFieldXPathTemplate = "//body[//*[text()[contains(.,'{0}')]]]";

        public const string MatchingConditionListTemplate = "//ul[li[span[text()[contains(.,'{0}')]]]]";

        public const string LicenseStatusActiveListXPathTemplate =
            "//div[label[text()[contains(.,'License Status')]]]/section/ul/li[text()='{0}']";
        public const string LicenseStatusActiveXPathTemplate = "//div[label[text()[contains(.,'License Status')]]]/section/ul/li[contains(@class, 'is_active')]";
        public const string LicenseStatusListXapth = "//div[label[text()[contains(.,'License Status')]]]/section/ul/li";
        public const string LicenseStatusDropDownXpath =
            "//div[label[text()[contains(.,'License Status')]]]//div[contains(@class,'select_input')]/span";
        public const string DecisionRationaleCotivitiUserCssSelector = "div.rationale-pane";
        public const string DecisionRationaleHyperlinkCssSelector = "a[href*='{0}']";
        public const string ConditionNotesToolTipXpathLocator = "//a[@title='View Condition Notes']";
        public const string ConditionNotesListCssSelector = "section.component_item_list>div>ul";
        public const string EmptyNoteMessageCssLocator = "section>p.empty_message";
        public const string ConditionNoteCaretIconByRowCssTemplate =
            "section.component_item_list>div:nth-of-type({0})>ul>li span.small_caret_right_icon";
        public const string ConditionNoteRecordsByRowColCssTemplate =
            "section.component_item_list>div:nth-of-type({0}).note_row>ul>li:nth-of-type({1}) span";
        public const string VisibleToClientIconInConditionNoteByRowCssTemplate =
           "section.component_item_list>div:nth-of-type({0})>ul>li.small_check_ok_icon";
        public const string ConditionNotesTextCssTemplate =
            "section.component_item_list >div:nth-of-type({0})>li>span";
        public const string ConditionNoteCarrotDownIconByRowCssTemplate =
            "section.component_item_list>div:nth-of-type({0})>ul>li span.small_caret_down_icon";

        public const string LockIconXpath = "//span[contains(@class,'locked')]";
        public const string ScoutCaseSelectedXpath = "//div[text()[contains(.,'Open Scout Case')]]/../../div[contains(@class,'selected')]";
        public const string ScoutCaseXpath = "//div[text()[contains(.,'Open Scout Case')]]/../span";
         public const string ConditionNoteTextCssTemplateByDate = "section.component_item_list >div:has( li>span:contains({0}))>li>span";

        public const string ConditionNotesDetailsValueTextCssTemplate = " section.component_item_list >div:nth-of-type({0}) li:nth-of-type({1}) span";

        public const string ProviderScoreWidgetXPath = "//div[@class='provider_score span_link']";
        public const string ProviderProfileWidgetXPath = "//div[contains(@class, 'profile_indicator')]";

        public const string ProviderClaimHistoryCssSelector = "span.patient_claim_history";

        public const string QuickNoActionAllConditionsCssSelector = "section.action_provider_conditions span.delete_all";

        public const string GoogleMapCssSelector = "div.map_modal";

        public const string CloseGoogleMapPopupCssSelector = "div#nucleus_modal_close";

        public const string ProviderExposureVisitAvgValueCssLocator =
            "div#visit_stat>div>span:nth-of-type(2)";

        public const string ClaimHistoryOptionsCssSelectorByRow = "span[title='History Options']+ul>li:nth-of-type({0})>span";
        public const string ClaimHistoryOptionsCssSelector = "span[title='History Options'] +ul>li span";
        public const string ClaimHistoryIconSelectedCssSelector = "li.is_active.is_selected[title='History Options']";
        public const string SelectConditionDropdownCssSelector = ".action_provider_conditions:nth-of-type(2) div.select_input>span";
        //public const string SelectConditionDropdownCssSelector = ".select_component:has(li.option.is_active:contains({0}))";
        public const string SelectConditionOptionFromDropdownCssSelector = "ul.is_visible.select_options li.option:contains({0})";
        public const string InputFieldForBeginProcCodeCssSelector = "input[placeholder = 'Begin Code']";
        public const string InputFieldForEndProcCodeCssSelector = "input[placeholder = 'End Code']";
        public const string SearchButtonInUserSpecifiedConditionFormCssSelector = ".form_buttons .work_button:contains(Search)";
        public const string AddButtonInUserSpecifiedConditionFormCssSelector = ".form_buttons .work_button:contains(Add)";
        public const string ClearButtonInUserSpecifiedConditionFormCssSelector = ".form_buttons .work_button:contains(Clear)";
        public const string FlagAllCodesCheckBoxXPath = "//div[contains(text(),'Flag all codes')]/../span";
        public const string CancelLinkCssSelector = ".form_buttons .span_link:contains(Cancel)";
        public const string GetUserSpecifiedConditionList = "//li[contains(text(),'Select Condition')]/../li";

        public const string GetTextIfAddOrSearchButtonXPath = "//button[contains(text(),'Clear')]/preceding-sibling::button";
        public const string ValueOfUserSpecifiedSearchResultsCssSelector = "ul:has(h2.component_list_header:contains(\"Search Results\"))>li>ul>li>span";
        public const string UserSpecifiedSearchResultsCssSelector = "h2.component_list_header:contains(\"Search Results\")";
        public const string UserSpecifiedMatchingConditionCssSelector = "h2:contains(\"Matching Conditions\")";
        public const string SeletableUserSpecifiedSearchResultCssSelector = "ul:has(h2.component_list_header:contains(Search Results))>li>ul";
        public const string UserSpecifiedMatchingConditionsRecordsCssSelector = "li:has(h2:contains(Matching Conditions))>ul>li>ul>li>span";
        public const string UserSpecifiedConditionInUserSpecifiedConditionSectionCssSelector = "ul.single_item_row li.action_link>span";
        public const string UserSpecifiedProcCodeCheckboxCssSelector = "ul.single_item_row+section.action_provider_conditions div.component_checkbox span.checkbox";
        public const string ActionConditionsSelectedUserSpecifiedCondition = "section.action_provider_form li.action_link>span";
        public const string ActionConditionsSelectedConditionsCssSelector = "h2:contains(Selected Conditions)";
        public const string SelectedProcCodeCssSelector = "ul.single_item_row+section.action_provider_conditions div.component_checkbox div";
        public const string UserSpecifiedConditionFlagAllCodesMessageCssSelector = "form>p.empty_message ";
        public const string SelectedSpecifiedConditionCssSelector = "li.is_active.is_editing:nth-of-type(1)>ul>li>span";

        public const string UserSpecifiedConditionFormCssSelector =
            "div#bottom_section>div section:nth-of-type(2)>section:nth-of-type(2)>div>section";

        public const string ActionConditionsFormSectionCssSelector = "section.action_provider_conditions.action_provider_form";

        public const string SelectedUserSpecifiedConditionComponent = "div#bottom_section>div section:nth-of-type(2)>section:nth-of-type(2)>ul";
        public const string SelectedUserSpecifiedConditionFromDropdown =
            "ul.form_row.no_listed_columns div.select_component ul.is_hidden.select_options>li.is_active";

        public const string RangeOfCodesContainerCssSelector = "section.action_provider_conditions.form_component:not(.action_provider_form)";
        public const string RangeOfCodesCountXpathSelector = "(//section[contains(@class,'action_provider_conditions form_component')]//ul)[1]/li";
        

        //li[contains(text(),'Select Condition')]/../../div/span
        public const string CreateUserSearchForConditionsIconCssSelector = "span[title='Create User Specified Condition']";

        public const string CancelLinkInActionConditionCssSelector =
            "section.action_provider_form  div.form_buttons span.span_link";

        public const string FirstProviderConditionEditIcon = "li[title='Action Condition']>span.small_edit_icon";

        public const string TopSectionCollapsed = "div#top_section.collapsed";
        

        #region
        public const string ActionInputDisabledCssLocator = "//label[text()='Action']/../section//input[@disabled]";
        public const string ReasonCodeInputDisabledCssLocator = "//div[contains(@class,'reason_code')]/section/div/input[@disabled]";
        public const string ActionAllConditionsDisabledCssLocator = "section.action_provider_form span.checkbox.is_disabled";
        public const string VisibleToClientCheckboxDisabledCssLocator = "//div[text()='Visible To Client']/..//span[contains(@class,'is_disabled')]";
        public const string SaveButtonDisabledCssLocator = "section.action_provider_form form div.form_buttons button.work_button.is_disabled";




        #endregion

        #endregion

        #endregion

        #region PROTECTED PROPERTIES

        public override string PageTitle
        {
            get { return PageTitleEnum.ProviderAction.GetStringValue(); }
        }

        //[FindsBy(How = How.XPath, Using = GenerateRationaleXPath)]
        //public ImageButton GenerateRationale;

        //[FindsBy(How = How.XPath, Using = ConditionNotesToolTipXpathLocator)]
        //public Link ConditionNotesNoteIcon;

        //[FindsBy(How = How.CssSelector, Using = EditActionConditionCssSelector)]
        //public ImageButton EditActionCondition;

        //[FindsBy(How = How.CssSelector, Using = NextActionCssSelector)]
        //public ImageButton NextAction;

        //[FindsBy(How = How.CssSelector, Using = QuickNoActionXIconCssLocator)]
        //public ImageButton QuickNoActionXIcon;

        //[FindsBy(How = How.CssSelector, Using = CancelActionCssSelector)]
        //public ImageButton CancelActionCondition;

        //[FindsBy(How = How.CssSelector, Using = SaveActionNoteIconButtonCssSelector)]
        //public ImageButton SaveActionCondition;

        //[FindsBy(How = How.CssSelector, Using = ClearUserAddedConditionSectionCssLocator)]
        //public ImageButton ClearUserAddedCondition;

        //[FindsBy(How = How.Id, Using = PageErrorCloseId)]
        //public ImageButton PageErrorCloseButton;

        //[FindsBy(How = How.CssSelector, Using = BackLinkCssSelector)]
        //public Link BackLink;

        //[FindsBy(How = How.XPath, Using = ActionComboBoxXPath)]
        //public SelectComboBox ActionComboBox;

        //[FindsBy(How = How.XPath, Using = ReasonCodeComboBoxXPath)]
        //public SelectComboBox ReasonCodeComboBox;

        //[FindsBy(How = How.Id, Using = ValidationNoticePopupContentDivId)]
        //public TextLabel ValidationNoticeModalPopupContentDiv;

        //[FindsBy(How = How.Id, Using = ValidationNoticePopupCloseId)]
        //public TextLabel ValidationNoticeModalClose;

        //[FindsBy(How = How.CssSelector, Using = SearchConditionCssSelector)]
        //public TextLabel SearchCondition;

        //[FindsBy(How = How.CssSelector, Using = SearchConditionInputBoxCssSelector)]
        //public TextField SearchConditionInput;

        //[FindsBy(How = How.CssSelector, Using = ProviderFlaggingCheckBoxCssSelector)]
        //public CheckBox ProviderFlaggingCheckBox;

        //[FindsBy(How = How.CssSelector, Using = OkConfirmationCssSelector)]
        //public Link OkConfirmationLink;

        //[FindsBy(How = How.CssSelector, Using = CancelConfirmationCssSelector)]
        //public Link CancelConfirmationLink;


        //[FindsBy(How = How.CssSelector, Using = SmallNoteIconCssSelector)]
        //public Link SmallNoteIcon;

        //[FindsBy(How = How.CssSelector, Using = NoteIconCssSelector)]
        //public Link NoteIcon;

        //[FindsBy(How = How.CssSelector, Using = SearchIconCssSelector)]
        //public Link SearchIcon;

        //[FindsBy(How = How.XPath, Using = ProviderScoreWidgetXPath)]
        //public Link ProviderScorecardWidget;

        //[FindsBy(How = How.XPath, Using = ProviderProfileWidgetXPath)]
        //public Link ProviderProfileWidget;


        //[FindsBy(How = How.CssSelector, Using = ProfileReviewIndicatorCssSelector)]
        //public Link ProviderProfileReviewIcon;

        //[FindsBy(How = How.CssSelector, Using = ProviderClaimHistoryCssSelector)]
        //public Link ProviderClaimHistoryIcon;

        //[FindsBy(How = How.CssSelector, Using = ProviderClaimHistoryCssSelector)]
        //public Link ProviderClaimHistoryOptions;
        #endregion

        #region CONSTRUCTOR

        public ProviderActionPageObjects()
            : base(PageUrlEnum.FraudOps.GetStringValue())
        {
        }

        #endregion
    }
}

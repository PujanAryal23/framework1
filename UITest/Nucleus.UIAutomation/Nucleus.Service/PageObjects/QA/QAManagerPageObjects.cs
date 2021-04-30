using Nucleus.Service.PageObjects.Default;
using Nucleus.Service.Support.Enum;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Elements;
using UIAutomation.Framework.Utils;

namespace Nucleus.Service.PageObjects.QA
{
    public class QaManagerPageObjects : NewDefaultPageObjects
    {
        #region PRIVATE/PUBLIC FIELDS
        public const string PageInsideTitleCssLocator = "label.page_title";
        public const string SideBarIconCssLocator = "span.sidebar_icon";
        public const string FilterPanelCssLocator = "label.component_title";

        public const string AddPTOIconCssLocator = "a.icon.pto";

        public const string DeleteIconListCssLocator="form>ul span.small_delete_icon ";
        public const string DeleteIconByRowCssTemplate = "form>ul:nth-of-type({0}) span.small_delete_icon ";
        public const string DateInputByRowColCssTemplate = "form>ul:nth-of-type({0}) div>input:nth-of-type({1})";

        public const string PlannedTimeOffForm =
            "section.component_header:has(.toolbar_icon.pto) ~ section section.settings_form.read_only";

        public const string ModifyPlannedTimeOffForm =
            "span.form_header_left:contains(Planned Time Off)~span:has(.small_edit_icon)";

        public const string NoentriesMessage = ".empty_message ";
        public const string TimeOffDateRange = "ul div.range_date_input_component";

        public const string AnalystOTPStartOrEndDate =
            "ul div.range_date_input_component input.datepicker_input_component:nth-of-type(1) ";

        public const string AppealQASettingsFormSectionCssSelector = "section.appeal_qa.form_component";
        public const string AppealQaDropdownCssSelector = "section.appeal_qa.form_component div.select_input input";
        public const string QaOptionInAppealQASettingsCssSelector = "ul.is_visible.select_options li:contains({0})";

        public const string CheckBoxOptionsInAppealQaSettingsXPath = "//div[text()='{0}']/preceding-sibling::span[contains(@class,'checkbox')]";


        #region Grid

        public const string EditIconByAnalystJqueryTemplate =
            "ul:has(>li>span:contains({0})) span.small_edit_icon";
        public const string EditIconByRowCssTemplate = "ul.component_item_list>div:nth-of-type({0}) span.small_edit_icon";
        public const string EditIconListCssSelector = "ul.component_item_list>div span.small_edit_icon";
        public const string ValueInGridByRowColumnCssTemplate =
            "ul.component_item_list>div:nth-of-type({0})>ul>li:nth-of-type({1})>span";
        public const string ListValueInGridByColumnCssTemplate =
            "section:has(label:contains({0})) ul.component_item_list>div>ul>li:nth-of-type({1})>span";
        public const string LabelInGridByRowColumnCssTemplate =
            "ul.component_item_list>div:nth-of-type({0})>ul>li:nth-of-type({1})>label";
        public const string GridListCssSelector = "section:has(label:contains({0})) ul.component_item_list>div>ul";
        public const string InactiveGridCssLocator = "ul.component_item_list>div>ul>li.is_disabled";
        public const string BeginDateInEditSectionCssLocator = "div.range_date_input_component>input:nth-of-type(1)";
        public const string EndDateInEditSectionCssLocator = "div.range_date_input_component>input:nth-of-type(2)";
        public const string BeginOrEndDateByHeaderNameCssLocator = "form:has(label:contains({0})) div.range_date_input_component>input:nth-of-type({1})";
        public const string ClaimsCountInEditSectionCssLocator = "tbody>tr:nth-of-type({0})>td:nth-of-type(1)>div>input";
        public const string MaxClaimsCountInEditSectionCssLocator = "tbody>tr:nth-of-type({0})>td:nth-of-type(3)>div>input";
        public const string ButtonXPathTemplate = "//button[text()='{0}']";
        public const string ClientDropDownCssSelector =
            "li.component_sentence_input>div.select_component>section>div>input";
        public const string ClientDropDownValueCssLocator =
            "ul.select_options.is_visible>li:contains({0})";
        public const string GridColumnLabelCssLocator = "thead>tr>th";
        public const string ClientInEditSectionCssLocator = "tbody>tr>th";
        public const string PercentClaimsInEditSectionCssLocator = "table>tbody>tr>td:nth-of-type(2)";
        public const string DeleteClientIneditSectionCssLocator =
            "li.qa_client_settings table tr:nth-of-type({0}) li[title = 'Remove']";

        #endregion  


        #region Css template

        public const string GetNameOfSelectedIconCssTemplate = "section.component_header li.is_selected";
        public const string SectionHeaderPresenceCssTemplate = ".component_header label:contains({0})";
        public const string QaResultsDetailsListValueByLabelJQueryTemplate =
            "section.form_component:has(label:contains(Find QA Result)) + ul.component_item_list li:has(>label:contains({0}))>span";
        public const string QaTargetHistoryDetailsValueByLabelCssTemplate = ".qa_target_result_list div:nth-of-type({0})>ul>li:has(>label:contains({1}))>span";//jquery
        public const string QaResultsDetailsValueByLabelCssTemplate = "section.form_component:has(label:contains(Find QA Result)) + ul.component_item_list>div:nth-of-type({0}) li:has(>label:contains({1}))>span";
        public const string QaTargetHistoryDetailsValueByRowColCssTemplate = ".qa_target_result_list div:nth-of-type({0})>ul>li:nth-of-type({1}) span";
        public const string QaTargetHistoryDetailsResultListCssTemplate = ".qa_target_result_list div>ul>li:nth-of-type({0}) span";
        public const string QaResultsDetailsListCssTemplate = "div section.form_component:has(label:contains(Find QA Result)) + ul>div li:nth-of-type({0}) span";
        public const string QaTargetHistoryDetailsLabelByRowColCssTemplate = ".qa_target_result_list div:nth-of-type({0})>ul>li:nth-of-type({1}) label";
        public const string ClaimPercentageCssLocator = "tbody>tr>td:nth-of-type(2)";
        public const string EditIconCssLocator = "span.small_edit_icon";
        public const string EditFormCssLocator = "section.form_component";
        public const string HeaderOfEditFormCssLocator = "section.form_component header.form_header";
        public const string SearchResultListCountCssSelector = "div.component_item";
        public const string ClearFilterCssSelector = "div.form_buttons  span.span_link";
        public const string AnalystInputFieldCssLocator =
            "section.component_sidebar_panel span input";
        public const string NoDataMessageLeftComponentCssSelector = ".search_list p.empty_message";
        public const string AppealFormCssLocator = "section.form_component";
        public const string QaResultsHistoryCountCssSelector  = "div section.form_component:has(label:contains(Find QA Result)) + ul>div";
        
        public const string FindQaResultButtonCssSelector  = ".form_component .form_buttons .work_button ";
        public const string CancelQaResultButtonCssSelector  = ".form_component:has(label:contains(Find QA Result)) .span_link";


        #endregion
        #region TEMPLATE
        public const string QaParametersInputFieldCssLocatorWithJquery = "label:has(>span:contains('1 of every'))~div:nth-of-type({0})>input"; //jQuery
        public const string EditIconByRowCssLocator = "ul.component_item_list>div:nth-of-type({0}) span.small_edit_icon";
        public const string InputFieldByLabelCssTemplate = "div:has(>label:contains({0})) input";
        public const string SearchListByTextInRowSelectorTemplate = "//span[text()[contains(.,'{0}')]]/../..";
        public const string SearchListRowSelectorTemplate = "ul.component_item_list div:nth-of-type({0})>ul";
        public const string QaTargetHistoryDetailsHeaderXpath =
          "//section[contains(@class,'search_list')]/section/div/label[text() = 'QA Target Results History']";

        public const string QaResultsFieldCssLocator = "div section.form_component:has(label:contains(Find QA Result))";
        public const string QaTargetHistoryFilterFieldValueCssLocator =
            "div.range_date_input_component input:nth-of-type({0})";

        public const string ClaimQaDetailsInputFieldCssLocator =
            "section.form_component tbody>tr>td:nth-of-type({0})>div>input"; //"div.basic_input_component:nth-of-type({0})>input";

        public const string AppealQaDetailsInputFieldCssLocator =
            "li.component_sentence_input>div.basic_input_component:nth-of-type({0})>input";//"div.qa_review_frequency:nth-of-type(1)>div:nth-of-type({0})>input";


        #endregion


        #region XPath

        public const string InputFieldXPathTemplate = "//label[text()='{0}']/../div/input";
        public const string InputFieldXPathTemplateForQaResultDate = "//div[label[text()='Date Range']]//input[{0}]";
        public const string InputFieldForQaResultDateXPath =
            @"(//label[text()='{0}']/../parent::div[contains(@class,'component_header')]/following-sibling::section)[1]//input[contains(@class,'datepicker')]";
        public const string DropDrownInputFieldListValueXPathTemplate =
            "//div[label[text()='{0}']]/section/ul/li[text()='{1}']";

        public const string DropDownInputListByLabelXPathTemplate =
            "//label[text()='{0}']/../section//ul//li";
        //public const string ClearCancelXPathTemplate = "//span[text()='{0}']";
        public const string LabelXPathTemplate = "//span[text()='{0}']";
        public const string FormHeaderLabelXPathTemplate = "//label[text()='{0}']";
        public const string QaRadioButtonXPathTemplate = "//li[not(contains(@class,'qa_client_settings'))]//span[text()= '']/../../span[{0}]";
        //"//span[text()= '{0}']/../../span/span";
        public const string QaRadioButtonLabelXPathTemplate = "//label/span[text()= '{0}']";
        public const string AppealsCompletedXPathTemplate = "//label[contains(text(), 'appeals completed = ')]/span";

        public const string ClaimQaDetailQaQcRadioButtonsXPathTemplate =
            "//label/span[text()='{0}']/../preceding-sibling::span[1]";

        public const string QCBusinessDaysOnlyXpath = "//div[contains(@class,'qc_weekday_check')]//div[text()='QC Business Days Only']";
        public const string QCBusinessDaysOnlyTooltipXpath = "//div[contains(@class,'qc_weekday_check')]//span";

        public const string ClaimsQaDetailQaQcLabelXPathTemplate = "//label/span[text()='{0}']";
        #endregion

        #region Analyst Category Assignments

        public const string AnalystManagerFormIconsCssSelectorTemplate = "li[title = '{0}']";
        public const string AnalystManagerFormCssSelector =
            "section.component_left:nth-of-type(1) section.component_content>section";
        public const string AnalystManagerFormTitleCssSelector = "section.component_left label.component_title";
        public const string AnalystManagerFormLocatorCssTemplate = "section.component_left:has(section:has(div:has(label:contains({0}))))";
        public const string AnalystManagerIconCssSelectorTemplate = "ul.modern_ui>li:nth-of-type({0})>a";
        public const string AnalystManagerLabelCssSelectorTemplate = "label:contains({0})";
        public const string DropdownCssSelectorTemplate = "label:contains({0})+section>div>input";
        public const string DownCaretIconCssSelectorTemplate = "div:has(label:contains({0}))>div.small_caret_down_icon";

        public const string RightCaretIconCssSelectorTemplate =
            "div:has(label:contains({0}))>div.small_caret_right_icon";

        public const string ClaimsAssignmentsNoRecordsCssSelectorTemplate =
            "section.component_left div.component_sub_header+ul.component_item_list:nth-of-type({0})>p";


        public const string ClaimsAssignmentsValueListCssTemplate =
            "div:has(div:has(label:contains({0})))+ul>div:nth-of-type({1})>ul>li>span";

        public const string ClaimsAssignmentsRecordsRowCssTemplate = "div:has(div:has(label:contains({0})))+ul>div";

        public const string ClaimsAssignmentRecordLabelSelectorTemplate =
            "div:has(div:has(label:contains({0})))+ul>div:nth-of-type({1})>ul:nth-of-type({2})>li>label";

        public const string ClaimsAssignmentParticularRecordValueListSelectorTemplate =
            "div:has(div:has(label:contains({0})))+ul>div>ul:nth-of-type({1})>li:nth-of-type({2})>span";

        public const string DeleteIconSelectorTemplate =
            "div:has(div:has(label:contains({0})))+ul>div:nth-of-type({1}) li[title=Delete]";
        public const string DropDownToggleIconCssTemplate = "section.form_component span.select_toggle";
        public const string DropDownToggleValueCssTemplate = "section.form_component ul.select_options li";
        public const string CategoryCodeDetailsCssSelector = "li.form_item div";
        public const string CategoryCodeDetailsLabelByRowColumnCssTemplate =
            "li.form_item div:nth-of-type({2}) ul:nth-of-type({0})>li:nth-of-type({1})>label";
        public const string CategoryCodeDetailsValueByRowColumnCssTemplate =
            "li.form_item div:nth-of-type({2}) ul:nth-of-type({0})>li:nth-of-type({1})>span";
        public const string CategoryCodeDetailsByLineNoCssTemplate =
            "li.form_item div:nth-of-type({0})";
        public const string MessageForUsersWithRestrictionAssigned =
            "form ul:nth-of-type(3) h2";
        public const string RestrictionOptionsByCssSelector =
            "label.radio_button_label>span:nth-of-type(1)";
        public const string RadioButtonByLabelXPathTemplate =
            "//span[text()='{0}']/../preceding::span[2]";
        public const string CategoryAssignmentByCategoryXPathTemplate =
            "//label[text()='{0}']/../../../ul[{2}]/div/ul/li/span[text()='{1}']";
        public const string DeleteIconByAssignementAndLineNo =
            "ul:nth-of-type({1}) div:nth-of-type({0}) span.small_delete_icon";


        #endregion
        #endregion

        #region PAGEOBJECTS PROPERTIES

        #endregion

        #region PROTECTED PROPERTIES

        public override string PageTitle
        {
            get { return PageTitleEnum.QAManager.GetStringValue(); }
        }

        #endregion

        #region CONSTRUCTOR

        public QaManagerPageObjects()
            : base(PageUrlEnum.QaManager.GetStringValue())
        {
        }

        #endregion
    }
}

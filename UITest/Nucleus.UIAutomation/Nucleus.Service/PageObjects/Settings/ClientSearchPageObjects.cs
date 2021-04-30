using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Nucleus.Service.PageObjects.Default;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Utils;

namespace Nucleus.Service.PageObjects.Settings
{
    public class ClientSearchPageObjects : NewDefaultPageObjects
    {
        #region CONSTRUCTOR
        public ClientSearchPageObjects()
            : base(PageUrlEnum.ClientSearch.GetStringValue())
        {
        }
        #endregion

        #region PROTECTED PROPERTIES
        public override string PageTitle
        {
            get { return PageTitleEnum.ClientSearch.GetStringValue(); }
        }
        #endregion

        #region public method

        #region ClientSettings

        public const string FormHeaderLabelCssLocator = "section.form_component >ul:nth-of-type(1) header label";
        public const string HeaderHelpIconXPathTemplate = "//h2[text()='{0}']/following-sibling::span";
        public const string ModifiedByWithDateCSSLocator = "li:has(label[title=\"Last Modified by\"]) span";
        public const string ClientSettingsSidePaneHeaderCSSLocator = "section.component_left .component_title";
        public const string NonAppealedFlagsLabelCssLocator = "label:contains(Non-Appealed Flags)";
        public const string NonAppealedFlagsDivXPathLocator = "//div[contains(@class,'list_transfer_component')]//label[text()='Non-Appealed Flags']";


        #region product/Appeals

        public const string AppealDueDatesTableColumnNameXPath = "//div[*/h2[text()='Appeal Due Date Calculation']]/table/thead//th";
        public const string TextAreaByLabelXPathTemplate = "//div[*/span[text()='{0}']]//textarea";
        public const string DivByLabelXPathTemplate = "//div[*/span[text()='{0}']] | //li[*/span[text()='{0}']]";
        public const string RadioButtonByLabelXPathTemplate =
            "//div[*/span[text()='{0}']]/span[{1}]";

        public const string RadioButtonByLabelPresentXPathTemplate = "//div[*/span[text()='{0}']]/span[contains(@class,'radio_button')]";

        public const string ProductListXPath = "//div[h2[text()='Product Status']]//div/label[1]";

        public const string AppealDueDatesInputByLabelXPathTemplate =
            "//div[contains(@class,'settings_sub_section')][*//h2[text()='Appeal Due Date Calculation']]//tr[th[text()='{0}']]/td//input";

        public const string AppealDueDatesInputByLabelAndColumnXPathTemplate =
            "//div[contains(@class,'settings_sub_section')][*//h2[text()='Appeal Due Date Calculation']]//tr[th[text()='{0}']]/td[{1}]//input";

        public const string AppealDueDatesAllInputTextBoxXPath =
            "//div[contains(@class,'settings_sub_section')][*//h2[text()='Appeal Due Date Calculation']]//input";

        public const string AppealDueDateProductRowXPath =
            "//div[*/h2[text()='Appeal Due Date Calculation']]/table/tbody/tr";

        public const string MRRRecordRequestExpirationLabelCssSelectorTemplate =
            "label:contains({0})";

        public const string MRRRecordRequestExpirationInputBoxCssSelector = "li:has(label:contains(MRR Record Request Expiration))>div.numeric_input_component>input";

        public const string FWAVRecordRequestExpirationInputBoxCssSelector =
            "li:has(label:contains(FWAV Record Request Expiration)) div.basic_input_component input";
        #endregion

        #region CONFIGURATION TAB

        public const string InfoHelpIconByLabelCSSLocator = "label:has(span:contains({0})) .info_icon";
        public const string InputTextBoxCSSLocator = "li:has(span:contains({0})) input";
        public const string DropdownListConfigSettingsCSSLocator = "li:has(span:contains({0})) .select_options li";

        public const string DropdownValueConfigSettingsCSSLocator =
            "//ul[contains(@class,'is_visible') and contains(@class,'select_options')]//li[text()='{0}']";
        #endregion

        #region Security Tab

        public const string DaysTextCssSelector = "label>span:contains(days)";
        public const string DefaultTextForClientUserAccessCssSelector = "li.component_flat_input  ul.is_hidden>li";
        public const string SecurityTabOptionsByLabelCssSelector = "span:contains({0})";
        public const string SecurityTabInputFieldsXpath = "//section[contains(@class,'settings_form')]//input";
        public const string SecurityTabWhiteListingHeaderCssSelector = "h2.component_list_header";
        public const string YesNoRadioBuutonByTextCssSelector = "section.settings_form label:contains({0})";
        public const string IpTextAreaCssLocator = "div.form_text.is_enabled>textarea";
        public const string TextAreaByLabelXPath = "//span[contains(text(),'{0}')]/../../../following-sibling::li//textarea";
        public const string ClientSettingsFormCssSelector = "section.settings_form";
        public const string ExternalIDPLogoutURLSectionXpath = "//h2[contains(text(),'External IDP Logout URL')]";

        #endregion

        #region WORKFLOW TAB

        public const string CompleteBatchWithPendedClaimsHelpIconCssLocator = "li:has(h2:contains({0})) .info_icon";
        public const string FieldRowWhenAutomatedBatchReleaseIsYesCssLocator = "li:has(label:contains({0}))";
        public const string InputFieldByLabelCssTemplate = "li:has(label:contains({0})) input";
        public const string DropDownInputListForTimePickerByLabelAndValueCssLocator = "//label[text()='{0}']/..//ul//li[text()='{1}']";
        public const string ProductListForWorklistTabCssSelector = "li.form_item:nth-of-type(4) label>span";
        #endregion

        public const string AllRadioButtonXPath =
            "//section[contains(@class,'settings_form')]//span[contains(@class,'component_radio_button')]";
        ////section[contains(@class,'form_component')]//span[contains(@class,'component_radio_button')]

        public const string AllTextAreaXPath =
            "//section[contains(@class,'settings_form')]//textarea";

        public const string AllTextBoxXPath =
            "//section[contains(@class,'settings_form')]//input";

        public const string StatusRadioButtonXPathTemplate =
            "//div[*/span[contains(text(),'Status')]]//div[contains(@class,'radio_button_group')]/span[{0}]";

        public const string StatusRadioButtonLabelXPathTemplate =
            "//div[*/span[contains(text(),'Status')]]//div[contains(@class,'radio_button_group')]/label[{0}]";

        public const string ClientSettingsTabListCssLocator = "div.option_button_selector >div";
        public const string SelectedClientSettingTabCssLocator = "div.option_button_selector >div.is_selected";

        public const string ClientSettingTabXPath =
            "//div[contains(@class,'option_button_selector')]/div[text()='{0}']";

        
        #endregion

        public const string InactiveIcon = "li.inactive_icon";
        public const string CheckMarkIconCssSelectorByLabel = "//label[text()='{0}:']/../following-sibling::li[1]/ul/li";
        public const string AppealDueDateCalculationTypeCheckMarkByProductandLabel = "//div[text()='{0}']/preceding-sibling::span[contains(@class, 'active')]";
        public const string FindButtonCssLocator = "button.work_button";
        public const string DisabledFindButtonCssLocator = "div.is_disabled>button.work_button";
        public const string FilterOptionsListCssLocator = "li.appeal_search_filter_options";
        public const string FilterOptionValueByCss = "li.appeal_search_filter_options>ul>li:nth-of-type({0})>span";
        public const string FilterOptionListByCss = "li.appeal_search_filter_options>ul>li>span";
        public const string ClientDetailHeaderByCSS = "section.column_50:not(.search_list) .component_header_left label";
        public const string DetailsLabelByCss = "(//label[text() = 'Client Details']/../../../section[2]/div/ul[{0}]/li/label)[{1}]";
        public const string ClientSecondaryDetailsValueByCss =
         "//label[text()='Client Details']/../../../section[2]/div/ul/li/label[contains(@title, '{0}')]/../span";

        public const string GridRowByClientNameXpathTemplate = "//ul/div/ul/li/span[text()='{0}']";

        #region Custom Fields

        public const string CustomFieldLabelXpathSelector = "//label[contains(text(),'Custom Fields')]";
        public const string HeaderLabelCssLocator = "div.custom_fields thead tr th";
        public const string AvailableFieldsXpathLocator = "//label[contains(normalize-space(),'Available Fields')]";
        public const string AvailableFieldEnableDropdownXpathSelector = "//div[contains(@class, 'custom_fields')]//..//section//input";
        public const string PHISelectorSelectedCheckBoxCssSelector = "div.custom_fields span.checkbox.active";
        public const string PHISelectorCheckboxByRowCssSelector = "div.custom_fields tbody tr:nth-of-type({0}) span.checkbox";
        public const string PHISelectorCheckboxCssSelector = "div.custom_fields span.checkbox";
        public const string AvailableListCssSelector = "div.custom_fields tbody tr";
        public const string OrderColumnCssSelector = "div.custom_fields tbody tr th";
        public const string CustomFieldsLabelCssSelector =
            "div.custom_fields tbody tr td:nth-of-type(2) input";

        public const string CustomFieldLabelByRowCssSelector = "div.custom_fields tbody tr:nth-of-type({0}) input";
        public const string AvailableFieldsDropdownValueListXpathSelector =
            "//div[contains(@class, 'custom_fields')]//..//section//li[text]";

        public const string EmptyMessageCssSelector = "p.empty_message";
        public const string MoveUpCaretIconCssSelector = "div.custom_fields tbody tr:nth-of-type({0}) span.half_caret_up_icon";
        public const string MoveDownCaretIconCssSelector = "div.custom_fields tbody tr:nth-of-type({0}) span.half_caret_down_icon ";
        public const string DeleteIconCssSelector = "div.custom_fields tbody tr:nth-of-type({0}) span.small_delete_icon";

        public const string CustomFieldColumnCssSelector =
            "div.custom_fields tbody tr td:nth-of-type({0})";
        public const string CustomFieldLabelListCssSelector = "div.custom_fields tbody tr input";
        public const string AllDeleteIconCssSelector = "div.custom_fields tbody tr span.small_delete_icon";
        public const string SettingsSectionCSSSeletor = ".settings_form ";
            

        #endregion

        #region File upload
        public const string CIWUploadSectionXpath = "//label[text()='CIW file']//ancestor::div";
        public const string CIWLastModifiedByXpath = "//label[contains(@title,'Last CIW Modified')]/../span";
        public const string ChooseFileButtonDisabledCssSelector = "input.file_upload_field:disabled";
        public const string ClientLogoUploadSectionXpath = "//label[text()='Client Logo']//ancestor::div";
        public const string ChooseFileButtonByCSS = "input[multiple].file_upload_field:disabled";
        public const string ClientLogolastModifiedByXpath = "//label[contains(@title,'Last Logo Modified')]/../span";

        #endregion

        #region Interop

        public const string RadioButtonByLabelOnInterop =
            "(//li[label[text()='{0}']]//span[contains(@class,'component_radio_button')])";

        #endregion

        public const string TimePickerInputFieldByLabelXPathTemplate =
            "(//*[label[text()='{0}']]//div[contains(@class,'timepicker_component')]//input)[{1}]";

        public const string TimePickerListByLabelXpathTemplate =
            "(//*[label[text()='{0}']]//div[contains(@class,'timepicker_component')]//ul[contains(@class,'is_visible')][1]/li)";


        public const string TimePickerValueByLabelXpathTemplate =
            "(//*[label[text()='{0}']]//div[contains(@class,'timepicker_component')]//ul[contains(@class,'is_visible')][1]/li[text()='{1}'])";

        public const string AppealDueDateCalculationCheckbox =
            //"//div[contains(@class,settings_sub_section)]/table/tbody/tr[1]/td/div/div[text()='Business']";
            "//div[text()='{0}']/preceding-sibling::span";

        public const string AppealCheckboxchecked =
            " div.settings_sub_section table tbody tr:nth-of-type({0}) td div:contains({1}) span.active";

        public const string AppealCalculationType =
            "div.component_checkbox div:contains({0})";

        public const string GetAppealDueDays =
            "div.settings_sub_section table tbody th:contains({0})~td:nth-of-type(1) input";

        public const string AppealDueDateSettingsTooltip = "h2:contains(Appeal Due Date Calculation)~span";

        public const string ExcludeHolidaysLabel = "//label[text()='Exclude holidays:']";

        public const string GetExcludeHolidayOptions = "//label[text()='Exclude holidays:']/following-sibling::div/div";

        public const string ExcludeHolidaysOptionCheckMarkByCotivitiOrClient = "//div[text()='{0}']/preceding-sibling::span[contains(@class, 'active')]";

        public const string ExcludeHolidayOptionCheckBox =
            "div.component_checkbox div:contains({0})";

        public const string TimeUnitCssSelectorTemplate = "li:has(label:contains({0}))>label:nth-of-type(2)";

        #endregion
    }
}

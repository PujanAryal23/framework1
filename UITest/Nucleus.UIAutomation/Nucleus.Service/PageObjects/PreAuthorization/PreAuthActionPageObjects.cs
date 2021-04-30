using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using Nucleus.Service.PageObjects.Default;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Utils;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Elements;

namespace Nucleus.Service.PageObjects.PreAuthorization
{
    public class PreAuthActionPageObjects : NewDefaultPageObjects
    {
        #region PRIVATE FIELDS

        #endregion

        #region CONSTRUCTOR

        public PreAuthActionPageObjects() : base(PageUrlEnum.PreAuthAction.GetStringValue())
        { }

        #endregion

        #region PROTECTED PROPERTIES
        public override string PageTitle
        {
            get { return PageTitleEnum.PreAuthorization.GetStringValue(); }
        }

        #endregion

        #region CSS/XPathLocators

        public const string FlagDetailValueByLabelAndFlag = "li:Contains({0})~li label:Contains({1})~span";
        public const string FlagAuditHistoryByLinNoFlagLabelXPathTemplate =
            "//div[*/li[contains(@class,'numeric_badge')][span[text()='{0}']]/following-sibling::li/span[text()='{1}']]/ul[2]//label[@title='{2}']/following-sibling::span";

        public const string AddFlagSectionCssLocator = "section.add_flag_form";
        public const string MessageBelowSelectedLinesXPath = "//h2[text()='Selected Lines']/following-sibling::p";
        
        public const string LinesListOnSelectLinesXPath = "//section[*/div[text()='Select All Lines']]/ul/li";

        public const string LinesListOnSelectedLinesXPath = "//section[h2[text()='Selected Lines']]/li";
        public const string ValuesListOnLinesOnSelectLinesXPathTemplate = "//section[*/div[text()='Select All Lines']]/ul/li{1}/ul/li[{0}]";
        public const string ValuesListOnLinesOnSelectedLinesXPathTemplate = "//section[h2[text()='Selected Lines']]/li/ul/li[{0}]";


        public static string SelectSearchResultByAuthSeqCssLocator = "div.component_item:has(li.component_data_point) span:contains(\"{0}\")";
        public const string UpperLeftQuadrantValueByLabel = "div.component_table_item:has(label.data_point_label:contains(\"{0}\")) div.data_point_value";
        public const string ValueByLabelXpath = "//label[text()='{0}']/../span";
        public const string ProcCodenDatenFlagCssLocator = "div#bottom_section>div>div>section>section.component_content>ul>div>ul:nth-child(1)>li.{0}>span";

        public const string LineItemValueListByRowColumnXPath =
            "//section[*/label[text()='Line Items']]/following-sibling::section//div/ul[{0}]/li[{1}]/span";

        public const string LineItemValueByDivRowColumnXPath =
            "//section[*/label[text()='Line Items']]/following-sibling::section//div[{0}]/ul[{1}]/li[{2}]/span";
        public const string QuadrantHeaderTitleCssLocator =
            "div#top_section label.component_title,label.page_title, #bottom_section label.component_title";

        public const string AddIconStateCssLocator = "li[title='Add Flag']";
        public const string ApproveIconStateCssLocator = "li[title = 'Approve']";
        public const string ApproveIconCssLocator = "span.approve";
        public const string AddFlagIconCssLocator = ".component_header_right .add_icon";
        public const string HistoryIconCssLocator = "span.patient_claim_history";
        public const string NextIconCssLocator = "span.next";
        public const string NextIconDisabledCssLocator = "li.is_disabled span.next";
        public const string MoreOptionsIconCssSelector = "span[title='more options']";
        public const string ReturnToPreAuthSearchCssLocator = "span.search_icon";
        
        public const string FlagLineNumberXPath =
            "//section[*/label[text()='Flagged Lines']]/following-sibling::section//*[contains(@class,'numeric_badge')]";

        public const string FlagLineLevelHeaderDetailValueXPathTemplate =
            "//section[*/label[text()='Flagged Lines']]/following-sibling::section//div[{0}]/li/ul[{1}]/li[{2}]/span";

        public const string FlagLineLevelHeaderDetailLabelXPathTemplate =
            "//section[*/label[text()='Flagged Lines']]/following-sibling::section//div[{0}]/li/ul[{1}]/li[{2}]/label";

        public const string ClientDataSourceValueXPathTemplate =
            "//section[*/label[text()='Flagged Lines']]/following-sibling::section//div[{0}]/li/div[{1}]//li[contains(@class,'client_flag_data_source ')]/li[1]/span";
        
        public const string FlagRowDetailValueXPathTemplate =
            "//section[*/label[text()='Flagged Lines']]/following-sibling::section//div[{0}]/li/div[{1}]//ul[contains(@class,'component_item_row')]/li[{2}]/span";
        
        public const string FlagRowDetailLabelXPathTemplate =
            "//section[*/label[text()='Flagged Lines']]/following-sibling::section//div[{0}]/li/div[{1}]//ul[contains(@class,'component_item_row')]/li[{2}]/label";

        public const string FlagRowEditIconXPathTemplate =
            "//section[*/label[text()='Flagged Lines']]/following-sibling::section//div[{0}]/li/div[{1}]//span[contains(@class,'small_edit_icon')]";

        public const string FlagRowLogicIconXPathTemplate =
            "//section[*/label[text()='Flagged Lines']]/following-sibling::section//div[{0}]/li/div[{1}]//span[contains(@class,'verisk_logic')]";

        public const string FlagRowAddLogicIconXPathTemplate =
            "//section[*/label[text()='Flagged Lines']]/following-sibling::section//div[{0}]/li/div[{1}]//span[contains(@class,'add_logic')]";

        public const string FlagRowXPathTemplate =
            "//section[*/label[text()='Flagged Lines']]/following-sibling::section//div[{0}]/li/div[{1}]/ul";

        public const string FlaggedLineRowDivByLineNoXPathTemplate =
            "//section[*/label[text()='Flagged Lines']]/following-sibling::section//div[*//li[contains(@class,'numeric_badge')]/span[text()='{0}']]";

        public const string ProcCodeXPathTemplate =
            "//section[*/label[text()='Flagged Lines']]/following-sibling::section//div[{0}]/li/ul[1]/li[3]/span";

        public const string FlagXPathTemplate =
            "//section[*/label[text()='Flagged Lines']]/following-sibling::section//div[{0}]/li/div[{1}]//ul[contains(@class,'component_item_row')]/li[{2}]/span";

        public const string FlagListByDivXPathTemplate =
            "//section[*/label[text()='Flagged Lines']]/following-sibling::section//div[{0}]/li/div//ul[contains(@class,'component_item_row')]/li[{1}]/span";

        public const string FlagBoldOrNormalColorXPathTemplate =
            "//section[*//label[text()='Flagged Lines']]//li[contains(@class,'{0}')]";

        public const string LineNoCssSelector = "div#bottom_section>div>div>section>section.component_content>ul>div>ul>li.is_count";

        public const string ProcCodeDescriptionCssSelector = "li.column_50> span";

        public const string LineDetailsProcCodeXPath =
            "//section[*/label[text()='Line Items']]/following-sibling::section//div[{0}]/ul[1]/li[4]/span";

        public const string EditFlagIconCssLocator = "li[title='Edit Flag']";

        public const string EnabledEditFlagIconsCssLocator = "li[title='Edit Flag'].is_enabled";

        public const string EditFlagIconByFlagLineNoAndRowXPathTemplate =
            "//section[*/label[text()='Flagged Lines']]/following-sibling::section//div[{0}]/li/div[{1}]//ul[contains(@class,'component_item_row')]//li[@title='Edit Flag']";

        public const string EditFlagFormSectionCssLocator = "section.form_component:has(header label:contains(Edit Flag))";

        public const string EditFlagFormSectionHeaderCssLocator = "#bottom_section section.form_component header span.form_header_left";

        public const string FlaggedLinesSectionActiveIconsByIconName = "li[class*=\"is_active\"]:has(span:contains(\"{0}\"))";

        public const string LineItemsOrFlagAuditHistoryCssSelectorByTitle = "a[title='{0}']";

        public const string FlagAuditRecordsCssSelector = "#bottom_section section.component_right>section.component_content>div";

        public const string FlagAuditHistoryNoAuditRecordMessageCssLocator = "#bottom_section section.component_right>section.component_content>p";

        public const string FlagAuditRecordLineNumberFlagSelectionCssSelectorTemplate = "#bottom_section section.component_right>section.component_content>div>ul>li:nth-of-type({0})";

        public const string FlagAuditHistoryModDateCssSelectorTemplate =
            "#bottom_section section.component_right>section.component_content>div:nth-of-type({0})>ul.component_nested_row>ul>li>label[title='Mod Date']+span";

        public const string FlagAuditHistoryActionsCssSelectorTemplate = "#bottom_section section.component_right>section.component_content>div:nth-of-type({0})>ul.component_nested_row";

        public const string FlagAuditHistoryRecordLabelCssSelectorTemplate = "#bottom_section section.component_right>section.component_content>div:nth-of-type({0})>ul:nth-of-type({1})>ul label";
        
        public const string FlagAuditHistoryRecordValuesListCssSelectorTemplate = "#bottom_section section.component_right>section.component_content>div:nth-of-type({0})>ul.component_nested_row:nth-of-type({1})>ul>li>span";

        public const string FlagAuditHistoryRecordValueCssSelectorTemplate =
            "#bottom_section section.component_right>section.component_content>div.component_item:has(li.badge:contains({0}))>ul:nth-of-type({1})>ul:nth-of-type({2})>li:nth-of-type({3})>span";

        public const string FlaggedLineReasonCodeToggleCssSelector = "div.reason_code span.select_toggle";

        public const string FlaggedLineReasonCodeInputFieldXPath = "//div[contains(@class,'reason_code')]//input";

        public const string FlaggedLineReasonCodesXPathTemplate =
            "//div[contains(@class,'reason_code')]/section/ul/li[{0}]";

        public const string FlaggedLineDeleteRestoreIconXPathTemplate = "//li[@title='{0}']/span";

        public const string SaveButtonByCss = "#bottom_section section button.work_button";
        public const string SaveButtomonlineByXpath = ".component_item_list div li div .work_button";

        public const string CancelLinkXPath = "#bottom_section section span.span_link";

        public const string EditFlagToopTipByFlagNoCssLocator = "li.component_item:has(li.component_data_point.badge>span:contains({0})) li[title=\"Edit Flag\"] span";

        public const string GetCountOfDeletedFlagsCssLocator = "ul.is_deleted";

        public const string VisibleToClientCheckboxCssLocator = "section.form_component:has(div:contains(\"Visible To Client\")) span.icon.checkbox";

        public const string EditNotesCssLocator = "body.cke_editable>p";
        public const string EditIconByLineNoAndFlagNameCssLocator = "section:has(label.component_title:contains(\"Flagged Lines\")) div:has(li.badge span:contains(\"{0}\")) ul.component_item_row:has(li.component_data_point>span:contains(\"{1}\")) span.small_edit_icon";

        public const string HeaderOfBottomRightSectionCssLocator = "#bottom_section  section.component_right div.component_header_left>label";

        public const string PreAuthDetailValueByTitleCssSelector = "label[title = '{0}']+div";

        public const string FlagAuditHistoryNotesLabelCssSelectorByLineNumAndRow = "#bottom_section section.component_right>section.component_content>div:nth-of-type({0})>ul:nth-of-type({1})>ul label[title='Notes']";

        public const string DisabledApproveIconCssSelector = "li.is_disabled>span.approve";

        public const string MoreOptionCssLocator = "li[title='more options'] > span";
        public const string MoreOptionListCssLocator = "li[title='More Options'] span.span_link";
        public const string PreAuthProcessingHistoryLinkXpath = "//span[text()='Pre-Auth Processing History']";
        public const string UsernameCssSelector = "div#client_meta>div#welcome_user";

        public const string LastDivOfLineItemsCssSelector = "div#bottom_section section.component_right ul.component_item_list>div:last-of-type";

        public const string LastDivOfFlagAuditHistoryCssSelector = "div#bottom_section section.component_right section.component_content>div:last-of-type";

        public const string LastDivOfFlaggedLinesCssSelector = "div#bottom_section section.component_left section.component_content>ul>div:last-of-type";
        public const string ReplyButtonByCss = "#bottom_section li button.work_button:nth-of-type(1)";


        #region Pre-Auth Transfer

        public const string TransferIconCssLocator = "li.is_active span.transfer";
        public const string TransferPreAuthFormCssSelector = "section.form_component:has(label:contains(Transfer Pre-Auth))";
        public const string TransferPreAuthNoteCssSelector = "div.form_text:has(label.form_label) textarea";
        public const string StatusDropdownIconInTransferFormCssSelector = ".form_component .select_component:has(label:contains(Status)) span.select_toggle";
        public const string AllStatusInStatusDropdownInPreAuthTransferForm = "ul.is_visible.select_options.type_ahead li";
        public const string StatusDropdownInputFieldInTransferFormCssSelector = ".form_component .select_component:has(label:contains(Status)) input";
        #endregion 
        public const string TransferIconStatusCssSelector = "li[title='Transfer']";

        public const string PatientPreAuthHistoryRowCssSelector = "table.nucleus_table > tbody > tr.dynamic_row";

        public const string PatientPreAuthHistoryLineDosSpecialtyCssSelector =
            "table.nucleus_table>tbody>tr.dynamic_row:nth-child({0})>td:nth-child({1})>span";

        public const string PatientPreAuthHistoryFlagCssSelector =
            "table.nucleus_table>tbody>tr.dynamic_row:nth-child({0})>td:nth-child(15)>span>a";

        public const string PateintPreAuthHistoryFlagPopUpXPathTemplate = "//span[text()='Flag - DUNL']/../a";

        public const string PatientPreAuthHistoryScenarioCurrencyCssSelector =
            "table.nucleus_table>tbody>tr.dynamic_row:nth-child({0})>td:nth-child({1})";

        public const string PatientPreAuthHistoryProcCssSelector =
            "table.nucleus_table>tbody>tr.dynamic_row:nth-child({0})>td>a";

        public const string PatientPreAuthHistorySummaryCssSelector = "table.nucleus_table > tbody > tr.summary_row";

        public const string PatientPreAuthHistoryProviderNameCssSelector =
            "table.nucleus_table>tbody>tr.summary_row>td>div:nth-child(1)>span>span";

        public const string PatientPreAuthHistoryAuthSeqCssSelector =
            "table.nucleus_table>tbody>tr.summary_row>td>div:nth-child(2)>span>a";

        public const string PatientPreAuthHistoryStatusCssSelector = 
            "table.nucleus_table>tbody>tr.summary_row>td>div:nth-child(3)>span:nth-child(2)";

        public const string PatientPreAuthHistoryCurrencyCssSelector = 
            "table.nucleus_table>tbody>tr.summary_row>td:nth-child({0})";



        public const string LockIconCssSelector = "span.medium_icon.locked";
        public const string EditDentalRecordCssLocator = "li[title='Edit Record']";


        #region Upload Document
        public const string UpperRightQuadrantTitleCssLocator = "#top_section section.component_right>section>div>label";

        public const string UploadDocumentFormHeader = "span.form_header_left>label";
        public const string addedDocumentList = "div.listed_document >ul:nth-of-type({0}) li";

        public const string UploadDocumentFormCssSelector = "section.form_component div.uploader";

        public const string AddIconCssSelector = "span.form_header_right span.add_icon";

        public const string CancelLinkCssSelector = "ul.document_list span.span_link";

        public const string PreAuthStatusCssSelector = "label[title='Status']+div";

        public const string PreAuthDocumentDeleteIconCssLocator =
            "div.listed_document:nth-of-type(1) >ul:nth-of-type(1)>li:not([style*='none'])  span.small_delete_icon";

        public const string PreAuthDocumentNameCssTemplate = "div.listed_document ul>li:nth-of-type({0})>span";
        #endregion

        public const string TNHyperlinkXpath = "//div[@id='bottom_section']//div//section[contains(@class, 'component_right')]//section[2]//ul//ul[3]//li[contains(@class,'action_link')]/span";

        public const string TNHyperlinkByXpath = "//div[@id='bottom_section']//div//section[contains(@class, 'component_right')]//section[2]//ul//div[{0}]//ul[3]//li[contains(@class,'action_link')]/span";
        public const string DisabledSaveButtonByCss = "#bottom_section section button.is_disabled.work_button ";
        public const string QuadrantTwoCancelButtonByCss = "#bottom_section section span.span_link ";
        public const string DeletedFlagByCssSelector = "ul.is_deleted li:has(span[title=\" {0} \"])";
        public const string AllFlagsInFlagAuditHistory = "#bottom_section section.component_right>section.component_content div>ul.component_item_row>li:nth-of-type(2)>span";


        public const string ProcCodeDescriptionByLineNumberCssSelector = "div.column_100:nth-of-type({0})> li > ul:nth-of-type(1) > li:nth-of-type(4)>span";
        public const string ProcCodeByLineNumberCssSelector = "div.column_100:nth-of-type({0})> li > ul:nth-of-type(1) > li:nth-of-type(3)>span";
        public const string LineNumberCssSelector = "div.column_100> li:nth-of-type({0}) > ul:nth-of-type(1) > li:nth-of-type(1)>span";
        public const string EobMessageByLineNoCssSelector = "div.column_100:nth-of-type({0})> li > div li:nth-of-type(3)";

        public const string NoDataAvailableMessageXpath =
            "//section[contains(@class,'component_content')]//p[text()='No Data Available']";

        public const string FlagAuditByActionAndUserTypeXpath =
            "//span[@title='{1}']/../../../ul[2]//li[3]//span[text()='Internal']/../../../ul[2]/li/span[text()='{0}']";

        public const string EditIconInLineItemsQuadrantByLineNo = @"section:contains(Line Items)~section>ul.component_item_list div:nth-of-type({0}) ul span.small_edit_icon";
        public const string EditLineSectionInLineItemsQuadrantByLineNo = @"section:contains(Line Items)~section>ul.component_item_list div:nth-of-type({0}) section.form_component ul label:contains({1})~input";
        public const string SaveButtoninEditLineSectionInLineItemsByLineNo = @"section:contains(Line Items)~section>ul.component_item_list div:nth-of-type({0}) section.form_component button.work_button";
        public const string CancelButtoninEditLineSectionInLineItemsByLineNo = @"section:contains(Line Items)~section>ul.component_item_list div:nth-of-type({0}) section.form_component span.span_link:contains(Cancel)";
        public const string QuickDeleteIconCssSelector = "span.delete_all";
        public const string QuickRestoreIconCssSelector = "span.restore_all";
        public const string FlagLinesCssSelector = "div#bottom_section section.component_left ul.component_item_list>div>li>div>ul";

        public const string FlagsOfFlagLineCssSelector =
            "div#bottom_section section.component_left ul.component_item_list>div>li>div>ul>ul>li:nth-of-type(3)>span";
        #endregion

        #region NewLogicManager
        public const string AddLogicIconCssSelectorByLineNumberAndRow = "div#bottom_section section.component_left section.component_content>ul>div:nth-of-type({0})>li>div:nth-of-type({1}) span.add_logic";
        public const string ClientLogicIconCssSelectorByLineNumAndRow = "div#bottom_section section.component_left section.component_content>ul>div:nth-of-type({0})>li>div:nth-of-type({1}) span.client_logic.is_active.small_icon";
        public const string LogicFormCssSelector = "div.flag_logics_messages+section textarea";
        public const string AssignedToValueCssSelector = "div.flag_logics_messages +section label[title='Assigned to'] + span";
        //public const string LogicFormCssLocator = "ul.flag_logics";
        public const string LogicFormByLineNumAndByRowCssTemplate = "div#bottom_section section.component_left ul.component_item_list>div:nth-of-type({0})>li>div:nth-of-type({1}) div.flag_logics_messages+section textarea";
        public const string LogicMessageCssSelector = "div.logic_message";
        public const string LogicMessageFormLabelCssSelector = "ul.flag_logics>section span.form_header_left>label";
        public const string LogicMessageTextareaCssLocator = "ul.flag_logics textarea";
        public const string LeftMessaageSectionCssLocator = "div.flag_logics_messages div.logic_message.left";
        public const string RightMessaageSectionCssLocatorByRow = "div.logic_message_row:nth-of-type({0}) div.logic_message.right";
        //public const string ClientLogicIconByRowCssTemplate = "div.flagged_line:nth-of-type({0}) .client_logic";
        public const string InternalLogicIconByRowCssTemplate = "div#bottom_section section.component_left section.component_content>ul>div:nth-of-type({0})>li>div:nth-of-type({1}) .internal_logic";
        public const string LogicIconWithLogicByLineNumAndRowCssTemplate = "div#bottom_section section.component_left section.component_content>ul>div:nth-of-type({0})>li>div:nth-of-type({1}) .client_logic,div#bottom_section section.component_left section.component_content>ul>div:nth-of-type({0})>li>div:nth-of-type({1}) .internal_logic";
        public const string ButtonXPathTemplate = "//button[text()='{0}']";
        public const string AddLogicIconCssLocator = "span.is_active.add_logic.icon.small_icon";
        public const string ClientLogicIconCssLocator = "span.is_active.client_logic.icon.small_icon";
        public const string CotivitiLogicIconCssLocator = "span.is_active.verisk_logic.icon.small_icon";
        #endregion

        #region Notes
        public const string AddNotesIndicatorCssSelector = "span.icon.toolbar_icon.add_notes";
        public const string ViewNotesIndicatorCssSelector = "span.icon.toolbar_icon.notes";
        public const string NotesLinkXPath = "//span[text()='Notes']";
        public const string NoteRecordsByRowColCssTemplate =
            "ul.component_item_list>div:nth-of-type({1}).note_row>ul>li:nth-of-type({0}) span";
        public const string NotePencilIconByRowCssTemplate =
           "ul.component_item_list>div:nth-of-type({0})>ul>li span.small_edit_icon";
        public const string NotePencilIconByNameCssTemplate =
            "div.note_row:has(span:contains({0})) span.small_edit_icon";
        public const string NoteCarrotIconByRowCssTemplate =
            "ul.component_item_list>div:nth-of-type({0})>ul>li span.small_caret_right_icon";
        public const string NoteCarrotIconByNameCssTemplate =
           "ul.component_item_list>div.note_row:has(span:contains({0}))>ul>li span.small_caret_right_icon";
        public const string NoteCarrotDownIconByRowCssTemplate =
            "ul.component_item_list>div:nth-of-type({0}) ul li li[title='Expand']";
        public const string NoteCarrotDownIconByNameCssTemplate =
         "ul.component_item_list>div.note_row:has(span:contains({0}))>ul>li span.small_caret_down_icon";
        public const string VisibleToClientIconInNoteContainerByRowCssTemplate =
           "ul.component_item_list>div:nth-of-type({0})>ul>li.small_check_ok_icon";
        public const string VisibleToClientIconInNoteRowByNameCssTemplate =
         "ul.component_item_list>div.note_row:has(span:contains({0}))>ul>li.small_check_ok_icon";
        public const string VisibleToClientIconInNoteRowCss =
        "ul.component_item_list>div.note_row .small_check_ok_icon";
        public const string NoteContainerCssLocator = "section.note_component";
        public const string NotesListCssLocator =
           "ul.component_item_list>div.note_row";
        public const string NotesEditFormCssLocator =
        "ul.component_item_list>div.note_row>div.note_edit_form";
        public const string NotesSectionWithScrollBarCssLocator =
           "ul.note_list";
        public const string NotesTextAreaByRowBarCssLocator =
           "div:nth-of-type({0}).note_row .cke_wysiwyg_frame";
        public const string NotesEditFormByRowXpath =
        "//div[{0}][contains(@class,'note_row')]/div[contains(@class,'note_edit_form')]";
        public const string NotesEditFormByNameCssSelector =
        "div.note_row:has(span:contains({0}))>div.note_edit_form";
        public const string NoteVisibleToClientEnabledCssLocator =
        "ul.component_item_list>div:nth-of-type({0}).note_row>div.note_edit_form>div.form_text.is_enabled>div.component_checkbox.is_enabled";
        public const string NotesEditFormSaveButtonCssLocator = "ul.note_list div.form_buttons .work_button";
        public const string NotesEditFormSaveButtonByNameCssLocator = "div.note_row:has(span:contains({0})) button.work_button";
        public const string NotesEditFormCancelButtonCssLocator = "div:nth-of-type({0}).note_row span.span_link";
        public const string NotesEditFormCancelButtonByNameCssLocator = "div.note_row:has(span:contains({0})) span.span_link";
        public const string VisibleToClientCheckBoxInNoteEditorByRowCssLocator = "div.component_checkbox span.checkbox";
        public const string NOtesIconByCss = "section:has(label:contains(Pre-Auth Action)) div ul li span.notes";
        public const string NotesAddIconCssSelector =
            "section:has(label:contains(Pre-Auth Action)) div ul li span.add_notes";
        public const string NotesIconDisabled = "li.add_icon.is_disabled";
        public const string AddNoteFormCssSelector = ".note_list > ul.component_group";
        public const string TypeInputXPath = "//label[text()='Type']/..//input";
        public const string TypeListValueXPathTemplate = "//label[text()='Type']/../section/ul/li[text()='{0}']";
        public const string SubTypeInputXPath = "//label[text()='Sub Type']/..//input";
        public const string SubTypeListValueXPathTemplate = "//label[text()='Sub Type']/../section/ul/li[text()='{0}']";
        public const string AddNotesTextAreaCssLocator = ".note_component .cke_wysiwyg_frame";
        public const string VisibleToClientCssLocator = ".note_component .checkbox";
        public const string CheckedVisibleToClientXpath = "//section[contains(@class,'form_component')][not(contains(@style,'none'))]//div[text()='Visible To Client']/following-sibling::span[contains(@class,'active')]";
        public const string AddNotesFirstNameLastNameCssLocator = "//section[contains(@class,'note_component')]//span[text()='Name']/../../../div[4]";
        public const string AddNoteSaveButtonCssSelector = ".note_component .work_button";
        public const string AddNoteCancelLinkCssSelector = ".note_component .span_link";
        public const string InputFieldInNotesHeaderByLabelCssTemplate = @"section:has(>ul>div>header:contains(Notes)) div:has(>label:contains({0})) input";
        public const string NoteTypeDropDownInputListByLabelXPathTemplate =
            "//span[text()='{0}']/../following-sibling::div/section//ul//li";
        public const string NoNotesMessageCssLocator = ".note_component .empty_message";
        public const string VisibleToClientCheckBoxInNoteEditorByNameCssLocator = "div.note_row:has(span:contains({0})) .checkbox";
        public const string SavedNotesDetailsByCss = "section.note_component ul div.note_row:nth-of-type({0}) li:nth-of-type({1})";
        public const string PreauthNoteSectionAddNoteByCss = "section.note_component ul div li:nth-of-type(1)";
        public const string SavedNotesDetailsByCol = "section.note_component ul div:nth-of-type({0}) ul li:nth-of-type({1})";
        public const string SelectedVisibleToClientCheckBoxInNoteEditorByRowCssLocator =
            "ul.component_item_list>div:nth-of-type({0}).note_row div.component_checkbox.selected";
        public const string SelectedVisibleToClientCheckBoxInNoteEditorByNameCssLocator =
            "ul.component_item_list>div.note_row:has(span:contains({0})) div.component_checkbox.selected";
        public const string RedBadgeNotesIndicatorCssSelector = "span.notes.icon.toolbar_icon>span.icon_badge";
        public const string GetPreAuthNoteType = ".note_component label:contains(Type)~li span";
        public const string GetPreauthNoteUserName= "//span[text()='Name']/../parent::div";


        #endregion

    }
}

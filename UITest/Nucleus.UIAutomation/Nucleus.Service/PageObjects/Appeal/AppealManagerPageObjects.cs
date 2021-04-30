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
    public class AppealManagerPageObjects : NewDefaultPageObjects
    {
        #region CONSTANT
        #endregion

        #region Appeal Details

        public const string AppealDetailsValueByLabelCssLocator = "li:has(>label:contains({0}))>span";//jquery
        public const string FlagDetailsInAppealDetailsCssLocator = "li:has(>label:contains(Product))+li>span";//jquery
        #endregion

        #region Appeal Search Result
        public const string AppealSearchResultCssLocator = "ul.component_item_list div ul.component_item_row";
        //public const string LoadMoreCssLocator = "div.load_more_data span";
        public const string AppealSequenceListCssSelector = "li.action_link >span";
        public const string UrgentListCssLocator =
            "ul.component_item_list>div>ul>li:nth-of-type(2)>ul>li:nth-of-type(1)";
        public const string FilterOptionsListCssLocator = "li.appeal_search_filter_options >ul>li";
        public const string FilterOptionsIconCssLocator = "li.appeal_search_filter_options";
        public const string AppealSearchResultListCssTemplate =
            "ul.component_item_list>div>ul>li:nth-of-type({0}) span";
        public const string AppealSearchResultByRowColCssTemplate =
           "ul.component_item_list>div:nth-of-type({1})>ul>li:nth-of-type({0}) span";

        public const string AppealSequenceInAppealSearchResultByAppealSequenceCssTemplate =
            "span:contains({0})";
        public const string LockIconByAppealSequenceCssTemplate ="ul:has(>li>span:contains({0})) li.lock";//jquery

        public const string BlackColorDueDateByAppealSequenceCssTemplate =
            "ul:has(>li>span:contains({0}))>li:nth-of-type(3):not(.due_date_equal_current_date, .due_date_for_tomorrow, .due_date_before_current_date)";//jquery

        public const string BoldRedColorDueDateCssTemplate = "li.due_date_before_current_date";
        public const string NonBoldOnlyRedColorDueDateCssTemplate = "li.due_date_equal_current_date";
        public const string OrangeColorDueDateCssTemplate = "li.due_date_for_tomorrow";
        public const string DisabledEditIconByAppealSequenceCssLocator = "small_edit_icon is_disabled";

        //public const string FieldErrorByLabelCssTemplate = "div:has(>label:contains({0})) span.field_error";
        public const string AppealLevelBadgeCssTemplate = ".secondary_badge span";
        public const string GreyAppealLevelBadgeCssTemplate = ".secondary_badge";

        #endregion

        #region XPath
        #region FindAppeal

        public const string InputFieldInEditAppealByLabelCssTemplate = @"section:has(>ul>header:contains(Edit Appeal Information)) div:has(>label:contains({0})) input";

        public const string InputFieldXPathTemplate = "//div[label[text()='{0}']]//input";

        public const string DropDrownInputFieldListValueXPathTemplate =
            "//div[label[text()='{0}']]/section/ul/li[text()='{1}']";

        public const string DropDownInputListByLabelXPathTemplate =
            "//label[text()='{0}']/../section//ul//li";

        public const string ButtonXPathTemplate = "//button[text()='{0}']";
        public const string ClearCancelXPathTemplate = "//span[text()='{0}']";
        #endregion
        #endregion

        public const string DueDateInEditAppealFormSectionXpath = "//span[contains(@class,'form_header_left')]/../../..//div[label[text()='Due Date']]/input";

        #region Css
        public const string PageHeaderCssLocator = "label.page_title";
        public const string EditIconCssLocator = "span.small_edit_icon";
        public const string EditIconByRowCssLocator = "div.component_item:nth-of-type({0}) span.small_edit_icon";
        public const string DeleteIconCssLocator = "span.small_delete_icon";
        public const string EditFormCssLocator = "section.form_component";
        public const string FindButtonCssLocator = "button.work_button";
        public const string DisabledFindButtonCssLocator = "div.is_disabled>button.work_button";
        public const string OkConfirmationCssSelector = "div#confirmation_links > div#complete_link";
        public const string CancelConfirmationCssSelector = "div#confirmation_links > span.span_link.modal_close";
        public const string SearchResultListCountCssSelector = ".component_item_list div.component_item";
        public const string ClearFilterClass = "div.form_buttons  span.span_link";
        public const string NoDataMessageCssSelector = ".appeal_secondary_view section p.empty_message";
        public const string NoDataMessageLeftComponentCssSelector = ".search_list p.empty_message";
        public const string HeaderEditIconCssLocator = "span.edit_icon";
        public const string SideBarIconCssLocator = "span.sidebar_icon";
        public const string SelectAllCheckBoxCssLocator = "div.component_checkbox";
        public const string EnabledEditAppealsConditionCssSelector = "li.is_active > span.toolbar_icon.edit_icon";
        public const string DisabledEditAppealsConditionCssSelector = "li.is_disabled > span.toolbar_icon.edit_icon";
        public const string SelectedAppealListXPathTemplate =
            "//section[h2[text()='{0}']]/ul/ul[contains(@class,'is_selectable is_active')]";
        public const string UnSelectedAppealListXPath=
            "//section[h2[text()='Select Appeals']]/ul/ul[not(contains(@class,'is_selectable is_active'))]";
        public const string SelectedDropDownListCssLocator = "section.multi_select_wrap >ul>section.selected_options>li";
        public const string MoveAppealsToQACssLocator = "span.add_qa";
        #endregion

        #region Template

        public const string DeleteIconTemplateCssLocator = "div.component_item:nth-of-type({0}) span.small_delete_icon";
        public const string SearchListRowSelectorTemplate = "ul.component_item_list div:nth-of-type({0})>ul";
        public const string SearchListByAppealSeqSelectorTemplate = "//span[text()= '{0}']/../..";
        public const string SelectAppealInSelectedAppealsColumnCssLocator = 
            "section.component_content:has(h2:contains(Selected Appeals)) span:contains({0})";

        public const string GetPrimaryReviewerOrAssignedToIn3colTemplate =
            "//span[text()= '{0}']/../../../ul/li/label[@title='{1}']/../span";
        public const string SearchResultCssTemplate =
            "ul.component_item_list>div:nth-of-type({0})>ul>li:nth-of-type({1}) span";
        public const string SearchResultByAppealSeqXpathTemplate =
            "//span[text()= '{0}']/../../li[{1}]/span"; //col: 3:date, 4: appeal seq, 5:client, 6:appeal type, 7:code, 8:assigned to, 9:code, 10:code and sequentially 11:status
        public const string AppealDetailsHeaderXpath =
           "//section[contains(@class,'search_list')]/section/div/label[text() = 'Appeal Details']";
        
        public const string AppealDetailContentLabelXpathTemplate =
            "(//section[contains(@class,'search_list')]/section/div/label[text() = 'Appeal Details']/../../../section[2]/div/ul[{0}]/li/label)[{1}]";
        public const string AppealDetailContentValueXpathTemplate =
            "(//section[contains(@class,'search_list')]/section/div/label[text() = 'Appeal Details']/../../../section[2]/div/ul[{0}]/li/span)[{1}]";

        public const string AppealDetailValueByLableXpathTempalte = "//label[contains(@title,'{0}')]";
        public const string AppealListRowEditIconCssTemplate = "ul:has(>li>span:contains({0})) span.small_edit_icon";//jquery
        public const string DisabledEditIconCssTemplate = "ul:has(>li>span:contains({0})) li:has(>span.small_edit_icon.is_disabled)";//jquery

        public const string ClientNotesDivCssLocator = "ul.allow_scroll ";

        #region Upload Section
        public const string AppealDocumentUploadEnabledXPath =
            "//li[span[contains(@class,'add_icon' )] and contains(@class,'is_active')]";
        public const string AppealDocumentUploadDisabledXPath =
            "//li[span[contains(@class,'add_icon' )] and contains(@class,'is_disabled')]";
        public const string FileTypeAvailableValueXPathTemplate =
            "//section[contains(@class,'multi_select_wrap')]/ul/section[contains(@class,'available_options')]/li[text()='{0}']";
        public const string FileToUploadSection = "//section[contains(@class, 'appeal_upload_form')]/div/ul[3]";
        public const string AppealDocumentsDivCssLocator = "div.listed_document";
        public const string ListsofAppealDocumentsCssLocator = "div.listed_document >ul:nth-of-type(1) >li:nth-of-type(4) span ";
        public const string AppealDocumentUploadSectionCssLocator = "section.appeal_upload_form ";
        public const string AppealDocumentUploadFileBrowseCssLocator = "section.appeal_upload_form input.file_upload_field";

        public const string AppealManagerUploaderFieldLabelCssLocator =
            "div.basic_input_component.uploader  div:nth-of-type({0}) label ";

        public const string AppealManagerUploaderFieldValueCssLocator =
            "div.basic_input_component.uploader  div:nth-of-type({0}) input ";

        public const string FileTypeCssLocator = "section.multi_select_wrap >input";
        public const string FileTypeToggleIconCssLocator = "section.multi_select_wrap >span";
        public const string FileTypeSelectedValueListCssLocator = "section.multi_select_wrap >ul>section.selected_options>li";
        public const string FileTypeValueListCssLocator = "section.multi_select_wrap >ul>section.available_options>li";
        public const string CancelAppealUploadButtonCssLocator = "section.appeal_upload_form span.span_link";
        public const string AddDocumentButtonCssLocator = "section.appeal_upload_form button.work_button.basic_button_component";
        public const string DisabledAddDocumentButtonCssLocator = "section.appeal_upload_form button.work_button.basic_button_component.is_disabled";
        public const string SaveAppealUploadButtonCssLocator = "section.appeal_upload_form div.form_buttons button.work_button";
        public const string FileToUploadDetailsCssLocator = "section.appeal_upload_form ul li:nth-of-type({0})  li:nth-of-type({1})> span";
        public const string DeleteFileDocumentInAppealSummaryCssLocator = "div.listed_document:nth-of-type({0}) >ul:nth-of-type(1) span.small_delete_icon ";
        public const string DeleteIconForFileListCssLocator = "div.listed_document >ul:nth-of-type(1) span.small_delete_icon";
        public const string DeleteIconDisabledForFileListCssLocator = "div.listed_document >ul:nth-of-type(1) span.small_delete_icon.is_disabled";
        public const string AppealDocumentsListAttributeValueCssTemplate = "div.listed_document:nth-of-type({0}) >ul:nth-of-type({1}) >li:nth-of-type({2}) span ";


        public const string InputFieldByLabelXpathTemplate = "//div[label[text()='{0}']]//input";

        public const string DropDownInputListValueByLabelAndValueXPathTemplate = "//label[text()='{0}']/../section//ul//li[text()='{1}']";


        #endregion

        #endregion

        #region Class

        public const string PageErrorPopupModelId = "nucleus_modal_wrap";
        private const string PageErrorCloseId = "nucleus_modal_close";
        public const string PageErrorMessageId = "nucleus_modal_content";
        #endregion


        #region PROTECTED PROPERTIES

        public override string PageTitle
        {
            get { return PageTitleEnum.AppealManager.GetStringValue(); }
        }


        #endregion

        #region CONSTRUCTOR

        public AppealManagerPageObjects()
            : base(PageUrlEnum.AppealManager.GetStringValue())
        {
        }

        #endregion
    }
}

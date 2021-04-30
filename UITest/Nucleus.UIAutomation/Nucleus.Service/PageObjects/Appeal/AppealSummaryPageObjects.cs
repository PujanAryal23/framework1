using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nucleus.Service.PageObjects.Default;
using Nucleus.Service.Support.Enum;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Elements;
using UIAutomation.Framework.Utils;

namespace Nucleus.Service.PageObjects.Appeal
{
    public class AppealSummaryPageObjects : NewDefaultPageObjects
    {

        #region Appeal Detail

        public const string AppealDetailValueXPthTemplate =
            "//section[contains(@class,'appeal_details ')] //label[text()='{0}']/../div";
        #endregion

        #region TEMPLATE

        public const string LabelXPath = "//label[text()='{0}']";
        public const string EditFormCssLocator = "section.appeal_details>section.form_component";

        public const string EditAppealInputFieldByLabelXPathTemplate = "//div[label[text()='{0}']]//input";

        public const string DisabledEditAppealInputFieldByLabelXPathTemplate =
            "//div[label[text()='{0}']]//input[@disabled]";

        public const string RecordQaResultNoteCssSelector = "textarea.text_editor";
        public const string EditAppealDropDownListValueByLabelXpathTemplate =
            "//div[label[text()='{0}']]//ul/li[text()='{1}']";
        public const string EditAppealDropDownListByLabelXpathTemplate =
            "//div[label[text()='{0}']]//ul/li";
        public const string EditAppealInformationValueCssTemplate =
            "section.appeal_details >section>ul>div:nth-of-type({0}) input";

        public const string AppealDetailsCssTemplate =
            "section.appeal_details >div>ul:nth-of-type({0})>div:nth-of-type({1})>div";

        public const string ClaimLineDivCssTemplate =
            "section.appeal_summary_lines >section:nth-of-type(2) li.component_item div:nth-of-type({0})>ul";

        public const string GreyAppealLevelCssTemplate =
            "section.appeal_summary_lines >section:nth-of-type(2) li.component_item div:nth-of-type({0})>ul> ul:nth-of-type(2)>li:nth-of-type({1}).secondary_badge>span ";

        public const string ClainNoListCssLcoator =
            "section.appeal_summary_lines >section:nth-of-type(2) li.component_item div>ul> ul:nth-of-type(1)>li:nth-of-type(1)>span";

        public const string ClaimLineFlagListValueCssTemplate =
            "section.appeal_summary_lines >section:nth-of-type(2) li.component_item div:nth-of-type({0})>ul> ul:nth-of-type(2)>div.listed_top_flags";

        public const string ClaimGroupRowValueXPathTemplate =
            "//section[contains(@class,'appeal_summary_lines')]//label[text()[contains(.,'{0}')]]/../span";

        public const string ClaimGroupRowLabelXPathTemplate =
            "//section[contains(@class,'appeal_summary_lines')]//label[text()[contains(.,'{0}')]]";

        public const string ClaimLinesRowColumnValueCssTemplate =
            "section.appeal_summary_lines >section:nth-of-type(2) li.component_item div:nth-of-type({0})>ul> ul:nth-of-type({1})>li:nth-of-type({2})>span";

        public const string ClaimLinesRowColumnCssTemplate =
           "section.appeal_summary_lines >section:nth-of-type(2) li.component_item div:nth-of-type({0})>ul> ul:nth-of-type({1})>li:nth-of-type({2})";

        public const string ClaimSequenceValueCssTemplate =
            "section.appeal_summary_lines>section:nth-of-type({0})>ul>div>li>ul>li:nth-of-type(1)>span";
        public const string AppealLineValueForLabelXpathTempalte = "//label[text()[contains(.,'{0}')]]/../span";

        public const string FirstLineOfAppealLineDataCssLocator =
            "div.has_appeal_line:nth-of-type({0})>ul>ul:nth-of-type(1)";//ul:nth-of-type is first line

        public const string AppealLineRowCssTemplate =
            "section.appeal_summary_lines >section>ul>div div.has_appeal_line:nth-of-type({0})>ul";
        public const string AppealLineDetailsValueCssTemplate =
            "section.appeal_line_details >section>ul:nth-of-type(1)>li>ul:nth-of-type({0}) li:nth-of-type({1})>span";

        public const string AppealSummaryValueCssTemplate =
            "section.appeal_details>div>ul:nth-of-type({0})>div:nth-of-type({1})>div";


        public const string AppealDocumentsListAttributeValueCssTemplate = "div.listed_document:nth-of-type({0}) >ul:nth-of-type({1}) >li:nth-of-type({2}) span ";
        public const string AppealLineDetailsAuditByLabelXpathTemplate = "//label[@title='{0}']/../span";
        public const string LabelElementPresentXpathTemplate = "//label[@title='{0}']";
        public const string AppealLineDetailsAuditByLabelForDivXpathTemplate = "//label[@title='{0}']/../div | //label[@title='{0}']/../span";
        public const string PreviousAppealListXPath = "//label[@title ='Previous Appeals']/../../li/span[@title != '']";
        public const string PreviousAppealSelectionXPath = "//label[@title ='Previous Appeals']/../../li/span[@title = '{0}']";
        public const string AppealSummaryPageTitleCss = "label.page_title";
        public const string LockIconCssSelector = "span.icon.medium_icon.locked";
        public const string LetterBodyForDCICssLocator = "div#appeal_letter_wrap";//"div#letterBody";
        #endregion

        #region ID


        #endregion

        #region XPATH
        public const string EditEnabledIconXPath =
            "//li[span[contains(@class,'edit_icon')] and not(contains(@class,'is_disabled'))]";
        public const string EditDisabledIconXPath =
            "//li[span[contains(@class,'edit_icon')] and contains(@class,'is_disabled')]";

        public const string AppealTypeXPath = "//div[label[text()='Appeal Type']]/div";
        public const string StatusValueXPath = "//div[label[text()='Status']]/div";
        public const string AppealLetterEnabledXPath =
            "//li[span[contains(@class,'letter_icon' )] and contains(@class,'is_active')]";
        public const string AppealLetterDisabledXPath =
            "//li[span[contains(@class,'letter_icon' )] and contains(@class,'is_disabled')]";
        public const string PreviousAppealValueXPath =
            "//section[contains(@class,'appeal_line_details ')] //li[contains(@class,'action_link')]";

        //public const string AppealNoteEnabledXPath =
        //    "//li[span[contains(@class,'notes' )] and contains(@class,'is_active')]";
        public const string AppealProcessingHxLinkXpath = "//span[text()='Appeal Processing History']";
        public const string AppealDocumentUploadEnabledXPath =
            "//li[span[contains(@class,'add_icon' )] and contains(@class,'is_active')]";
        public const string AppealDocumentUploadDisabledXPath =
            "//li[span[contains(@class,'add_icon' )] and contains(@class,'is_disabled')]";
        public const string FileTypeAvailableValueXPathTemplate =
            "//section[contains(@class,'multi_select_wrap')]/ul/section[contains(@class,'available_options')]/li[text()='{0}']";
        public const string FileToUploadSection = "//section[contains(@class, 'upload_form')]/div/ul[3]";
        //public const string ClaimSequenceXpath = "//section[contains(@class,'appeal_summary_lines ')] //li/label[text()='Claim Sequence:']/../span";
        public const string DiplayRationaleLinkXpath = "//span[@title= 'Display Rationale and Summary']";

        public const string ActiveFirstAppealLineXpath =
            "//div[contains(@class,'has_appeal_line')]/ul[contains(@class,'is_active')]";
        public const string QaCompleteInAppealSummaryEnabledIconXpathSelector = "//span[contains(@class,'qa_pass')]/..";
        
        public const string QaCompleteInAppealSummaryIconCssSelector = "span.qa_pass";

        #endregion
        #region Class

        public const string SaveButtonEditAppeal = "section.appeal_details  button.work_button";
        public const string CancelLinkEditAppeal = "section.appeal_details  span.span_link";
        public const string EditIconCssLocator = "span.edit_icon";
        public const string AppealLinesSectionCssLocator = "section.appeal_summary_lines";
        
        public const string ClosedAppealDisabledIconCssLocator =
            "section.claim_action div.component_header_right>ul>li:nth-of-type(2).is_disabled>span.approve";

        public const string ClosedAppealEnabledIconCssLocator =
            "section.claim_action div.component_header_right>ul>li:nth-of-type(2).is_active >span.approve";

        public const string NoPreviousAppealXPath =
            "//ul[li/label[text()[contains(.,'Previous Appeals')]]]/li[2]/span";

        public const string AppealDetailsCssLocator = "section.appeal_details";
        public const string AppealSequenceValueHeaderSectionCssLocator = "section.claim_action >div>div>li>span";
        public const string SearchIconCssLocator = "span.search_icon";

        public const string AppealLineDetailsSectionCssLocator = "section.appeal_line_details";
        public const string AppealLineDetailsEmptyMessageCssLocator = "section.appeal_line_details p.empty_message";

        public const string MoreOptionCssLocator = "li[title='More Options'] > span";
        public const string AppealDocumentsDivCssLocator = "div.listed_document";
        public const string ListsofAppealDocumentsCssLocator = "div.listed_document >ul:nth-of-type(1) >li:nth-of-type(4) span ";
        public const string AppealDocumentUploadSectionCssLocator = "section.appeal_upload_form,section.upload_form ";
        public const string AppealDocumentUploadFileBrowseCssLocator = "section.appeal_upload_form,section.upload_form input.file_upload_field";

        public const string AppealSummaryUploaderFieldLabelCssLocator =
            "div.basic_input_component.uploader  div:nth-of-type({0}) label ";

        public const string AppealSummaryUploaderFieldValueCssLocator =
            "div.basic_input_component.uploader  div:nth-of-type({0}) input ";

        public const string FileTypeCssLocator = "section.multi_select_wrap >input";
        public const string FileTypeToggleIconCssLocator = "section.multi_select_wrap >span";
        public const string FileTypeSelectedValueListCssLocator = "section.multi_select_wrap >ul>section.selected_options>li";
        public const string FileTypeValueListCssLocator = "section.multi_select_wrap >ul>section.available_options>li";
        public const string CancelAppealUploadButtonCssLocator = "section.upload_form,section.appeal_upload_form span.span_link";
        public const string AddDocumentButtonCssLocator = "section.appeal_upload_form,section.upload_form button.work_button.basic_button_component";
        public const string DisabledAddDocumentButtonCssLocator = "section.upload_form button.work_button.basic_button_component.is_disabled";
        public const string SaveAppealUploadButtonCssLocator = "section.upload_form div.form_buttons button.work_button";
        public const string FileToUploadDetailsCssLocator = "section.upload_form ul li:nth-of-type({0})  li:nth-of-type({1})> span";
        public const string DeleteFileDocumentInAppealSummaryCssLocator = "div.listed_document:nth-of-type({0}) >ul:nth-of-type(1) span.small_delete_icon ";
        public const string DeleteIconForFileListCssLocator = "div.listed_document >ul:nth-of-type(1) span.small_delete_icon";
        public const string DeleteIconDisabledForFileListCssLocator = "div.listed_document >ul:nth-of-type(1) span.small_delete_icon.is_disabled";

        #region Appeal Email
        public const string EmailIconCssLocator = "a.appeal_email_icon";
        public const string AppealEmailFormCssLocator = "section.appeal_email_form";
        public const string ToInputFieldCssLocator="section.appeal_email_form>form>ul:nth-of-type(1)>div:nth-of-type(1) input";
        public const string AdditionalCCInputFieldCssLocator = "section.appeal_email_form>form>ul:nth-of-type(1)>div:nth-of-type(4) input";
        public const string EmailValueXPath = "//li[label[text()='Email:']]/span";
        public const string ClientCCValueXPath = "//li[label[text()='Client CC:']]/span";
        public const string TextMessageCssLocator = "section.appeal_email_form >form>ul:nth-of-type(2)";
        public const string CancelAppealEmailLinkCssLocator = "section.appeal_email_form div.form_buttons>span>span.span_link";
        public const string SendEmailButtonXPath = "//button[text()='Send Email']";
        #endregion
        #endregion

        #region PROTECTED PROPERTIES

        public override string PageTitle
        {
            get { return PageTitleEnum.AppealSummary.GetStringValue(); }
        }

        
        #endregion

        #region CONSTRUCTOR

        public AppealSummaryPageObjects()
            : base(PageUrlEnum.AppealSummary.GetStringValue())
        {
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Nucleus.Service.PageObjects.Default;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Utils;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Elements;

namespace Nucleus.Service.PageObjects.Appeal
{
    public class AppealActionPageObjects : NewDefaultPageObjects
    {

        public const string WorkingAjaxMessageCssLocator = "div.small_loading";
        #region AppealInformation

        public const string EditAppealInputFieldByLabelXPathTemplate = "//section[contains(@class,'form_component')]//div[label[text()='{0}']]//input";

        public const string EditAppealDropDownListValueByLabelXpathTemplate =
            "//div[label[text()='{0}']]//ul/li[text()='{1}']";

        public const string AppealStatusDropDownValueByXpath = "//div[label[text()='Status']]//ul/li";

        public const string EditAppealInputDivByLabelXPathTemplate =
            "//header[.//text()[contains(.,'Edit Appeal')]]/../..//label[text()[contains(.,'{0}')]]/..";
        #endregion

        #region Adjust Appeal Line

        public const string EditIconAdjustAppealLineByFlagCssTemplate =
            "div.appeal-line-flag:has(span:contains({0})) span.small_edit_icon";//jquery

        public const string DeleteIconAdjustAppealLineByFlagCssTemplate =
            "div.appeal-line-flag:has(span:contains({0})) span.small_delete_icon";//jquery

        public const string RestoreIconAdjustAppealLineByFlagCssTemplate =
            "div.appeal-line-flag:has(span:contains({0})) span.quick_restore";//jquery

        public const string SwitchIconAdjustAppealLineByFlagCssTemplate =
            "div.appeal-line-flag:has(span:contains({0})) span.small_switch_icon:not(.is_disabled)";//jquery

        public const string AdjustAppealLineHeaderCssSelector = "header:contains(Adjust Appeal Line)";//jquery

        public const string EditFlagLineByFlagCssTemplate = "div.appeal-line-flag:has(span:contains({0}))";
        public const string EditFlagLineByDeletedFlagCssTemplate = "div.appeal-line-flag.is_deleted:has(span:contains({0}))";

        public const string EditFlagLineValueListByFlagCssTemplate =
            "div.appeal-line-flag:has(span:contains({0})) li>span";

        public const string EditFlagLineLabelListByFlagCssTemplate =
            "div.appeal-line-flag:has(span:contains({0})) li>label";

        public const string FlagListAdjustAppealLineCssLocator = "div.appeal-line-flag li.edit_flag>span";

        public const string EditIconAdjustAppealLineByRowCssTemplate =
            "div.appeal-line-flag:nth-of-type({0}) span.small_edit_icon:not(.is_disabled)";
        public const string DeleteIconAdjustAppealLineByRowCssTemplate =
            "div.appeal-line-flag:nth-of-type({0}) span.small_delete_icon";
        public const string RestoreIconAdjustAppealLineByRowCssTemplate =
            "div.appeal-line-flag:nth-of-type({0}) span.small_restore_icon";
        public const string SwitchIconAdjustAppealLineByRowCssTemplate =
            "div.appeal-line-flag:nth-of-type({0}) span.small_switch_icon";

        public const string EditFlagLineByDeletedRowCssTemplate = "div.appeal-line-flag.is_deleted:nth-of-type({0})";

        public const string EditFlagLineCssLocator = "div.appeal-line-flag";

        //public const string EditFlagReasonCodeXPath =
        //    "//section[contains(@class,'flag_line_edits')]//label[text()='Reason Code']/..//input";

        //public const string ReasonCodeEditFlagXPathTemplate =
        //    "//section[contains(@class,'flag_line_edits')]//label[text()='Reason Code']/..//li[text()='{0}']";

        public const string ReasonCodeListEditFlagCssLocator =
            "section.flag_line_edits div:has(label:contains(Reason)) ul>li";

        public const string CancelLinkEditFlagCssLocator = "section.flag_line_edits span.span_link";

        public const string AppealLineCancelLinkCssLocator = "div.form_buttons  span.span_link:contains(Cancel)";

        public const string AppealLineCancelLinkXPathTemplate = "//section[contains(@class,'appeal_claims')]//span[text()='Cancel']";

        public const string AppealLineCancelLinkByLinNoXPathTemplate = "//li[contains(@class,'badge')]//span[text()={0}]//..//..//..//span[text()='Cancel']";

        public const string EditFlagDivCssLocator = "section.flag_line_edits";

        public const string LineDeleteCssLocator = "span.line_delete";
        public const string LineRestoreCssLocator = "span.line_restore";
        public const string LineSwitchCssLocator = "span.line_switch";

        public const string FlagLinesXpathTemplate = "//li[contains(@class,'badge')]//span[text()={0}]//..//..//..//div[contains(@class,'appeal-flags')]//div[not(contains(@class, 'is_deleted'))]";
        public const string DeletedFlagLinesXpathTemplate = "//div[contains(@class,'appeal-flags')]//div[(contains(@class, 'is_deleted'))]";

        public const string AdjustAppealLineByLineNoXpathTemplate = "//li[contains(@class,'badge')]//span[text()={0}]//..//..//span[text()='Adjust']";



        #endregion

        #region TEMPLATE

        #endregion
        #region DisabledIcon

        public const string LockedIconCssSelector = "span.medium_icon.locked";
        public const string DisabledEditIconCssSelector = "span.small_edit_icon.is_disabled";
        public const string DisabledNoteIconCssSelector = "li.is_disabled>span.notes";
        public const string DisabledLetterIconCssSelector = "li.is_disabled>span.letter_icon";
        public const string DisabledAppealDraftIconCssSelector = "li.is_disabled>span.save_draft";
        public const string DisabledAppealDocumentCssSelector = "li.is_disabled>span.has_documents";
        public const string DisabledTextAreaByLabelXPathTemplate =
            "//div[label[text()[contains(.,'{0}')]]]//textarea[@disabled]";
        public const string DisabledInputFieldByLabelXPathTemplate =
            "//div[label[text()[contains(.,'{0}')]]]//input[@disabled]";

        public const string DisabledVisibleToClientCheckBoxCssLocator = "span.checkbox.is_disabled";
        public const string SearchIconXPath = "//li[span[contains(@class,'search_icon')]]";
        public const string DisabledSaveDraftButton = "//div[button[text()='Save Draft'] and contains(@class,'is_disabled')]";

        #endregion
        public const string EnabledLetterIconCssSelector = "li.is_active>span.letter_icon";
        public const string EnabledAppealDraftIconCssSelector = "li.is_active>span.save_draft";
        public const string EnabledAppealDocumentCssSelector = "li.is_active>span.has_documents";

        public const string AppealEllipsisByLabelXPathTemplate = "//li[label[text()[contains(.,'{0}')]]]";
        public const string AppealInfoByLabelXPathTemplate = "//li[label[text()[contains(.,'{0}')]]]/span";
        public const string InputFieldByLabelXPathTemplate = "//div[label[text()='{0}']]//input";

        public const string EditAppealDropDownListByLabelXpathTemplate =
            "//div[label[text()='{0}']]//ul/li";

        public const string FlagByLineNoXPathTemplate =
            "//ul[li[contains(@class,'badge')]/span[text()='{0}']]//li[not(contains(@class,'is_deleted'))]/span[text()='{1}']";

        public const string ClaimSequenceValueCssTemplate =
            "section.appeal_claims >div:nth-of-type({0})>li>ul>li:nth-of-type(1)>span";
        public const string ProviderValueCssTemplate =
            "section.appeal_claims >div:nth-of-type({0})>li>ul>li:nth-of-type(2)>span";
        public const string SpecialtyValueCssTemplate =
            "section.appeal_claims >div:nth-of-type({0})>li>ul>li:nth-of-type(3)>span";
       
        public const string AppealRowValueCssTemplate = "div.appeal_row >ul:nth-of-type({0})>li:nth-of-type({1})>span";
        public const string LineNoCssTemplate = "section.appeal_claims>div:nth-of-type({0}) >li>ul:nth-of-type({1}) li.numeric_badge >span";

        public const string FlagListValueCssTemplate =
            "section.appeal_claims>div:nth-of-type({0}) >li>ul:nth-of-type({1}) div.listed_top_flags";

        public const string DateOfServiceCssTemplate =
            "section.appeal_claims>div:nth-of-type({0}) >li>ul:nth-of-type({1}) li.line_date >span";
        public const string RevCodeCssTemplate =
            "section.appeal_claims>div:nth-of-type({0}) >li>ul:nth-of-type({1}) li:nth-of-type(6)>span";
        public const string ModifierCssTemplate =
            "section.appeal_claims>div:nth-of-type({0}) >li>ul:nth-of-type({1}) li:nth-of-type(7)>span";
        public const string ProcCodeCssTemplate =
            "section.appeal_claims>div:nth-of-type({0}) >li>ul:nth-of-type({1}) li:nth-of-type(8)>span";
        public const string ProcDescriptionCssTemplate =
            "section.appeal_claims>div:nth-of-type({0}) >li>ul:nth-of-type({1}) li:nth-of-type(9)>span";
        public const string UnitsCssTemplate =
            "section.appeal_claims>div:nth-of-type({0}) >li>ul:nth-of-type({1}) li:nth-of-type(10)>span";
        public const string SourceCssTemplate =
            "section.appeal_claims>div:nth-of-type({0}) >li>ul:nth-of-type({1}) li:nth-of-type(11)>span";
        public const string SugPaidCssTemplate =
            "section.appeal_claims>div:nth-of-type({0}) >li>ul:nth-of-type({1}) li:nth-of-type(12)>span";
        public const string TrigCodeCssTemplate =
            "section.appeal_claims>div:nth-of-type({0}) >li>ul:nth-of-type({1}) li:nth-of-type(13)>span";
        public const string TrigSpecCssTemplate =
            "section.appeal_claims>div:nth-of-type({0}) >li>ul:nth-of-type({1}) li:nth-of-type(14)>span";
        public const string TrigDOSCssTemplate =
            "section.appeal_claims>div:nth-of-type({0}) >li>ul:nth-of-type({1}) li:nth-of-type(15)>span";
        public const string AppealLevelCssTemplate =
            "section.appeal_claims>div:nth-of-type({0}) >li>ul:nth-of-type({1}) li.notification_badge >span";//notification_badge means red

        public const string DenyIconCssTemplate =
            "section.appeal_claims>div:nth-of-type({0}) >li>ul:nth-of-type({1}) li.appeal_deny";
        public const string AllPayIconCssTemplate =
            "section.appeal_claims>div:nth-of-type(2) >li>ul li.appeal_pay.value_badge";
        public const string AllDenyIconCssTemplate =
            "section.appeal_claims>div:nth-of-type(2) >li>ul li.appeal_deny.value_badge";
        public const string PayIconCssTemplate =
           "section.appeal_claims>div:nth-of-type({0}) >li>ul:nth-of-type({1}) li.appeal_pay";
        public const string AdjustIconCssTemplate =
           "section.appeal_claims>div:nth-of-type({0}) >li>ul:nth-of-type({1}) li.appeal_adjust";
        public const string NoDocsIconCssTemplate =
           "section.appeal_claims>div:nth-of-type({0}) >li>ul:nth-of-type({1}) li.appeal_no_docs";
        public const string AppealHelpIconCssTemplate =
            "section.appeal_claims>div:nth-of-type({0}) >li>ul:nth-of-type({1}) li:nth-of-type(17)>span";
        public const string AppealResultTypeCssTemplate =
            "section.appeal_claims>div:nth-of-type({0}) >li>ul:nth-of-type({1}) ul.option_list";
        public const string AppealResultTypeEllipsesCssTemplate =
            "section.appeal_claims>div:nth-of-type({0}) >li>ul:nth-of-type({1}) span.ellipses";



        public const string AdjustIconXPathTemplateByFlagAndClaimSequence =
            "//li[contains(@class,'component_item')][*//span[text()='{0}']]//ul[contains(@class,'appeal_action_line')][*//span[text()='{1}']]//li[contains(@class,'appeal_adjust')]";

        public const string ReasonCodeListXpathTemplate =
            "//div[contains(@class,'reason_code')]//ul/li[text()='{0}']";

        public const string EditAppealLineTextAreaXpathTemplate = "//div[label[text()='{0}']]/textarea";
        public const string EditAppealLineIframeEditorXpathTemplate = "//div[label[text()='{0}']]//iframe";
        public const string EditAppealLineIframeEditorByRowXpathTemplate = "//ul[contains(@class,'component_nested_row ')][{0}]//div[label[text()='{1}']]//iframe";

        public const string AppealLineListXpathTemplate = "//ul[contains(@class,'component_nested_row ')]";
        public const string EditAppealLineIframeEditorByAppealLineNoAndAppealLineClaSeqXpathTemplate =
            "(//li[contains(@class,'component_item')]//span[text()='{0}']/../label[text()='Claim Sequence:']//ancestor::li//div[label[text()='{1}']]//iframe)[{2}]";//"(//div[label[text()='{0}']]//iframe)[{1}]";

        //public const string ExclamationIconXpathTemplate = "//label[text()='{0}']/span[2][not(contains(@style,'none'))]";
        public const string AppealDocumentFileNameCssLocator = "div.listed_document li.action_link span:nth-of-type({0})";
        public const string AppealDocumentInfoCssLocator = "div.listed_document:nth-of-type({0})  ul:nth-of-type({1}) li:nth-of-type({2}) span";

        public const string AppealLineDeleteIconCssLocator =
            "ul:nth-of-type({0}) ul.appeal_action_line  span.small_delete_icon";

        public const string EnabledAppealLineDeleteIconsCssSelector =
            "ul.appeal_action_line  span.small_delete_icon.is_active";

        public const string AppealLineDeleteIconListElementsCssLocator = "ul:nth-of-type({0}) ul.appeal_action_line>li:nth-of-type(1)";
        public const string AppealLineListXpath = "//ul[ul[contains(@class,'appeal_action_line')]]";

        public const string AppealLetterClaimLineXpathTemplate =
            "(//div[contains(@class, 'appeal-letter ')]/section/section[2]//li[contains(text(),'Line')])[{0}]";

        public const string AppealLineFormCssLocator = "section.appeal_claims section.form_component";

        public const string AppealLineFormByLineCssLocator =
            "section.appeal_claims li.component_item>ul:nth-of-type({0})>section.form_component";

        public const string AppealActionHeaderForMRRAppealTypeCssSelector =
            ".appeal_action_header li>span:contains(Medical Record Review)";


        #region ID



        #endregion

        #region XPATH

        public const string SaveButtonXPath = "//button[text()='Save']";
        public const string AppealProcessingHxLinkXpath = "//span[text()='Appeal Processing History']";
        public const string AppealHxLinkXpath = "//span[text()='Appeal History']";
        public const string SaveDraftButtonXPath = "//li[ul[contains(@class,'appeal_claim')]]/ul[{0}]//button[text()='Save Draft']";

        public const string CancelSaveDraftXPath =
            "//div[button[text()='Save Draft']]//span[contains(@class,'span_link')]";

        public const string CopyAllButtonXpath =
            "//li[ul[contains(@class,'appeal_claim')]]/ul[{0}]//button[text()='Copy All']";
        public const string AppealDocumentIconXpath = "//li[contains(@title,'View Appeal Documents')]";
        public const string AppealDocuemntSectionXpath = "//label[text()='Appeal Documents']/../../..";
        public const string AppealLetterSectionXpath = "//label[text()='Appeal Letter']/../../..";

        public const string ConsultantRationalesInputXPath =
            "//div[label[starts-with(normalize-space(),'Consultant Rationales')]]//input";

        public const string ConsultantRationalesListXPath =
            "//div[label[starts-with(normalize-space(),'Consultant Rationales')]]//ul/li";

        public const string ConsultantRationalesValueXPath =
            "//div[label[starts-with(normalize-space(),'Consultant Rationales')]]//ul/li[text()='{0}']";

        public const string AppealLetterFooterXpath =
            "//div[contains(@class, 'appeal-letter ')]/section/section[2]/ul/ul[contains(text(),'Please let')]";

        #endregion

        #region CSS


        public const string DashboardButtonCssLocator = "a.dashboard";
        public const string HelpButtonCssLocator = "a.help_center";


        public const string ReasonCodeInputCssLocator = "div.reason_code input";
        public const string ReasonCodeListCssLocator = "div.reason_code ul:not(.is_hidden)>li";

        public const string CancelLinkCssLocator = "div.form_buttons  span.span_link";
        public const string EditAppealHeaderValueCssLocator = "header.form_header";
        public const string EditAppealLineSectionCssLocator = "section.form_component ";
        public const string SearchIconCssLocator = "span.search_icon";

        public const string RecordReviewLabelCssLocator = "li.appeal_type_header >span";
        public const string AppealSeqCssLocator = "li.appeal_action_seq >span";
        public const string EditIconCssLocator = "span.small_edit_icon.is_active";
        public const string AppealDocumentSectionCssLocator = "section.component_left.search_list";
        public const string AppealDocumentListCssLocator = "div.listed_document";
        public const string AppealDocumentListByDateCssLocator = "div.listed_document>ul> li:nth-of-type(4)";
        public const string ClaimSequenceValueListCssLocator =
           "section.appeal_claims >div:nth-of-type(2)>li>ul>li>span";

        public const string MoreOptionsListCssLocator = "li.appeal_search_filter_options >ul >li>span";

        public const string AppealLetterSectionCssLocator =  "div.appeal-letter>section";

        public const string RedInvalidInActionResultCssLocator =
            "div.radio_button_group>span.invalid";

        //private

        public const string VisibleToClientCssLocator = "span.icon.checkbox ";
       // public const string addNotesIconCssLocator = "span.add_notes.icon";
       // public const string notesIconCssLocator = "span.notes.icon";
        public const string documentsIconCssLocator = "span.has_documents.icon";
        public const string approveIconCssLocator = "span.approve.icon";
        public const string savedraftIconCssLocator = "span.save_draft.icon";
        public const string LetterIconIconCssLocator = "span.letter_icon.icon";
        
        public const string MoreOptionCssLocator = "li[title='More Options'] > span";
        #endregion

        #region Appeal Email

        public const string EnabledDisabledAppealEmailXPath = "//li[span[contains(@class,'appeal_email_icon')]]";
        public const string EmailIconCssLocator = "span.appeal_email_icon";
        public const string AppealEmailFormCssLocator = "section.appeal_email_form";
        public const string ToInputFieldCssLocator = "section.appeal_email_form>form>ul:nth-of-type(1)>div:nth-of-type(1) input";
        public const string AdditionalCCInputFieldCssLocator = "section.appeal_email_form>form>ul:nth-of-type(1)>div:nth-of-type(4) input";
        public const string EmailValueXPath = "//li[label[text()='Email:']]/span";
        public const string ClientCCValueXPath = "//li[label[text()='Client CC:']]/span";
        public const string TextMessageCssLocator = "section.appeal_email_form >form>ul:nth-of-type(2)";
        public const string CancelAppealEmailLinkCssLocator = "section.appeal_email_form div.form_buttons>span>span.span_link";
        public const string SendEmailButtonXPath = "//button[text()='Send Email']";
        #endregion

        #region Appeal Mail

        public const string LetterBodyCssLocator = "div.appeal-letter section.component_content";
        public const string LetterBodyForDCICssLocator = "div#letterBody";
        public const string RefreshIconCssLocator = "span.restore_all";
        public const string GetDocumentTypeInAppealLetter = ".appeal-letter ul:nth-of-type(1) li ul li";
        public const string DocumentTypeInLetterBody = "div#review_disclaimer ul li";

        #endregion

        #region Appeal Help

        public const string AppealHelpSectionXPath = "//label[text()='Appeal Help']/../../../../section";
        public const string AppealHelpfulHintsTextXPath = "//label[text()='Appeal Helpful Hints']/../../div[1]";
        public const string AppealRationaleDocumentXPath = "//label[text()='Appeal Rationale Document']/../../div[2]";
        public const string GetAppealHelpfulHintsTextXPath = "//label[text()='Appeal Helpful Hints']/../../div[1]//div//iframe";
        public const string GetAppealHelpfulHintsTextNoDataXPath = "//label[text()='Appeal Helpful Hints']/../../div[1]//p";
        public const string AppealRationaleDocumentFileNameXPath = "//label[text()='Appeal Rationale Document']/../../div[2]//div//li[2]//span";
        public const string AppealRationaleDocumentFileNameNoDataXPath = "//label[text()='Appeal Rationale Document']/../../div[2]//p";

        #endregion


        #region PROTECTED PROPERTIES

        public override string PageTitle
        {
            get { return PageTitleEnum.AppealSummary.GetStringValue(); }
        }


        #endregion

        #region CONSTRUCTOR

        public AppealActionPageObjects()
            : base(PageUrlEnum.AppealSummary.GetStringValue())
        {
        }

        #endregion
    }
}

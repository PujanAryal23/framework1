using System;
using System.Drawing;
using Nucleus.Service.PageObjects.Default;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Utils;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Base;
using UIAutomation.Framework.Elements;
using UIAutomation.Framework.Core.Driver;

namespace Nucleus.Service.PageObjects.Claim
{
    public class ClaimActionPageObjects : NewDefaultPageObjects
    {
        #region public/PUBLIC FIELDS

        public const string DentalHistoryLinkXPathTemplate = "//label[text()='{0}']/../span";
        public const string SpinnerCssLocator = "div.spinner_wrap, div.loading_spinner";
        public const string PageHeaderCssLocator = "label.page_title";
        public const string ClaimRestrictionIconToolbarCssLocator = "span.claim_restrictions.toolbar_icon";
        public const string ClaimRestrictionIndicatorIconToolbarCssLocator = "div.claim_worker_indicator.{0}";
        public const string PatientHistoryIconCssLocator = "span.patient_claim_history";
        public const string PatientHistoryPatientSeqCssLocator = "label.patseq_title";
        public const string PopUpCssSelector = ".SubNavPopupContent";

        #region claim flag audit history

        public const string ClaimFlagAuditHistoryFlagSelectorXPathTemplate =
            "//section[contains(@class,'claim_lines')]//div[contains(@class,'component_item')]/ul/li/span[text()='{0}']"; 
        public const string ClaimFlagAuditHistoryIconCssLocator = "a.audit_icon";
       

        public const string ClaimFlagAuditHistoryDetailCssTemplate =
            "section.claim_lines section.component_content>div:has(span:contains({0})):has(li.numeric_badge>span:contains({3})) div.flag_audits div:nth-of-type({1}) li:has(label:contains({2}))>span";//jquery

        public const string ClaimFlagAuditHistoryHeaderDetailCssTemplate =
            "div.flag_details:has(span:contains({0})) li:has(label:contains({1}))>span";//jquery

        public const string FlagOnClaimFlagAuditHistoryCssTemplate =
            "section.claim_lines section.component_content>div:nth-of-type({0})>ul>li.edit_flag>span";

        public const string LineNoOnClaimFlagAuditHistoryCssTemplate =
            "section.claim_lines section.component_content>div:nth-of-type({0})>ul>li.numeric_badge>span";

        public const string ClaimFlagAuditHistoryDetailListCssTemplate =
            "section.claim_lines section.component_content>div:has(span:contains({0})):nth-of-type(1) li:has(label:contains({1}))>span";//jquery

        public const string ClaimFlagAuditHistoryRowDivCsLocator =
            "section.claim_lines section.component_content>div:has(li.numeric_badge>span:contains({1})):has(li.edit_flag >span:contains({0}))";

        public const string FlagLinkByFlagCssTemplate = "div.line_modifiers li.edit_flag span:contains({0})";
        #endregion

        #region SidebarPannel

        public const string EmptyMessageOnAdditionalLockedClaims =
            "//header[text()='Additional Locked Claims']/../p[contains(@class,'empty_message')]";
        #endregion

        #region addflag

        public const string TriggerClaimLineOnAddFlagCssTemplate =
            "li.trigger_claim:has(span:contains({0}))>li:has(li.numeric_badge>span:contains({1}))";//jquery

        public const string CheckBoxCssLocator = "span.checkbox";

        #endregion

        #region flaggedlines

        public const string sourcevalueByFlag =
            ".line_modifiers .component_nested_row:contains({0}) ul:contains(Source:)";
        public const string FlaggedLineDetailsCssSelectorTemplate =
            "div.flagged_line:nth-of-type({0})>li>ul:nth-of-type({1})>li:nth-of-type({2})>span";
        public const string LineDetailsDataPointValuesCssSelectorByDivNumAndTitle = "li.line_detail>div:nth-of-type({0})>ul>ul>li>label[title = '{1}']+span";
        public const string LineDetailsSugModifierByPreceedingSiblingCssSelector = "li.line_detail>div:nth-of-type({0}) li:has(label:contains({1}))+ li>label";
        public const string LineDetailLabelByDivNumAndRowAndColCssSelector = "li.line_detail>div:nth-of-type({0}) ul.component_item_row:nth-of-type({1}) li:nth-of-type({2}) label";
        public const string SelectedLineSelectedClaimLinesCssSelectorTemplate = "section.add_flags section.component_content_section>li:nth-of-type({0})";

        public const string DataPointsLabelForLineDetailsCssSelector =
            "li.line_detail>div:nth-of-type({0})>ul>ul:nth-of-type({1})>li:nth-of-type({2})>span";

        public const string LineDetailDataPointLabelCssSelectorByLabel = "li.line_detail>ul>li>label:contains({0})";
        public const string LineDetailDataPointValueCssSelectorByLabel = "li.line_detail>ul>li>label:contains({0})+span";
        public const string FlagLineManualSourceXPathTemplate =
            "//div[contains(@class,'flagged_line')]//div//ul[*//li[contains(@class,'edit_flag')]/span[normalize-space()='{0}']]//li/span[text()='Manual/CL']";

        public const string FlagLineEditFlagSourceXpathTemplate = "//div[contains(@class,'flagged_line')][{0}]//div//ul[*//li[contains(@class,'edit_flag')]/span[normalize-space()='{1}']]//li[label[text()='S:']]/span";

        public const string FlagLineSelectedLinesIndexCssSelector = "section.component_content_section>li.is_selectable.is_active li:first-child>span";
       
        public const string DentalDataValueListXPathTemplate =
            "//div[contains(@class,'{0}')][1] //label[{1}]/following-sibling::span";

        public const string DentalDataInputValueListXPathTemplate =
            "//div[contains(@class,'claim_line')][1] //label[{0}]/following-sibling::input";

        public const string DataPointsLabelForFlaggedLineCssSelectorTemplate = "div.flagged_line:nth-of-type(1)>li>ul>li>label[title='{0}']";
        public const string DataPointsValueForFlaggedLineCssSelectorTemplate = "div.flagged_line:nth-of-type({0})>li>ul>li>label[title='{1}'] + span";

        public const string DeleteIconOnFlagByLineNoAndRowCssTemplate =
            "div.flagged_line:nth-of-type({0}) div:nth-of-type({1}) span.small_delete_icon.is_active";

        public const string RestoreIconOnFlagLineNoAndRowCssTemplate = "div.flagged_line:nth-of-type({0})>li>div:nth-of-type({1}) span.small_restore_icon.is_active";
        public const string SelectFlagCodeDivInFlaggedLinesCssTemplate = "(//div[contains(@class, 'flagged_line')]//div[contains(@class, 'line_modifiers')])[{0}]";

        public const string GetFlagCodeFromFlaggedLines = "(//div[contains(@class, 'flagged_line')]//div[contains(@class, 'line_modifiers')]/" +
                                                          "ul[contains(@class,'component_nested_row')]/ul/li[3]/span)[{0}]";

        public const string DeleteRestoreIconOnFlagLinesByRowCssLocator =
            "div.flagged_line:nth-of-type({0}) >li>ul:nth-of-type(2) li:nth-of-type(2)>span";
        public const string DeleteIconOnFlagLinesByRowCssLocator =
            "div.flagged_line:nth-of-type({0}) >li>ul:nth-of-type(2) span.small_delete_icon";

        public const string RestoreIconOnFlaggedLinesByRowCssLocator =
            "div.flagged_line:nth-of-type({0}) >li>ul:nth-of-type(2) span.small_restore_icon.is_active";

        public const string FlagLineDivCssTemplate = "div.line_modifiers:has(span:contains({0}))";//jquery

        public const string FlagLineDivCssSelector =
            "section.flagged_lines>section.component_content>ul>div.flagged_line:nth-of-type({0})>li>div.line_modifiers>ul>ul";
        //section.flagged_lines>section.component_content>ul>div.flagged_line:nth-of-type(1) div.line_modifiers:nth-of-type(2)>ul>ul
        #endregion

        public const string FlaggedLineLineCssSelector = "section.flagged_lines>section.component_content>ul>div.flagged_line:nth-of-type({0})>li>ul:nth-of-type({1})";
        public const string SwitchIconByCurrentLineNoAndTriggerClaimAndTriggerLineNoXPathLocator =
                "//div[contains(@class,'flagged_line ')]/li[*[li[1][span[text()='{0}']]]]/div[contains(@class,'line_modifiers')][*/ul[li[7][span[text()='{1}']] and li[8][span[text()='{2}']]]]//span[contains(@class,'small_switch_icon')  and not(contains(@class,'is_disabled'))]";

        public const string SwitchIconByLineNumberAndRowNum = "div.flagged_line:nth-of-type({0}) div.line_modifiers:nth-of-type({1}) li[title = 'Switch Flag']";

        public const string DisabledSwitchIconByCurrentLineNoAndTriggerClaimAndTriggerLineNoXPathLocator =
            "//div[contains(@class,'flagged_line ')]/li[*[li[1][span[text()='{0}']]]]/div[contains(@class,'line_modifiers')][*/ul[li[7][span[text()='{1}']] and li[8][span[text()='{2}']]]]//span[contains(@class,'small_switch_icon')  and contains(@class,'is_disabled')]";

        public const string SwitchIconByCurrentLineNoAndFlagAndTriggerLineNoXPathLocator =
            "//div[contains(@class,'flagged_line ')]/li[*[li[1][span[text()='{0}']]]]/div[contains(@class,'line_modifiers')][*/ul[li[3][span[text()='{1}']] and li[8][span[text()='{2}']]]]//span[contains(@class,'small_switch_icon')  and not(contains(@class,'is_disabled'))]";

        public const string DisabledSwitchIconByCurrentLineNoAndFlagAndTriggerLineNoXPathLocator =
            "//div[contains(@class,'flagged_line ')]/li[*[li[1][span[text()='{0}']]]]/div[contains(@class,'line_modifiers')][*/ul[li[3][span[text()='{1}']] and li[8][span[text()='{2}']]]]//span[contains(@class,'small_switch_icon')  and contains(@class,'is_disabled')]";
        


        public const string HeaderListCssLocator = "section:not(.is_hidden)>section> div.component_header_left";

        public const string EditAllFlagClaimSectionCssLocator = "section.claim_flag_edits:not([style*='none'])";
        public const string EditFlagClaimSectionCssLocator = "section.flag_line_edits:not([style*='none'])";

        public const string VisibleToClientAllFLagsCssSelector =
            "section.claim_flag_edits:not([style*='none']) ul div.component_checkbox";

        public const string VisibleToClientLineFlagCssSelector =
            "section.line_flag_edits:not([style*='none']) ul div.component_checkbox";

        public const string VisibleToClientFlagLineCssSelector =
            "section.flag_line_edits:not([style*='none']) ul li div.component_checkbox";

        public const string SourceByFlagXPath =
            "//div[contains(@class,'line_modifiers ')]/ul/ul[li[3]/span[text()='{0}']]/li[4]/span";
        public const string TrigDOSXPath = "//div[contains(@class,'line_modifiers ')]/ul/ul/li[2]/span[text()!='']";
        public const string ClaimLineDetailsValueByLineNoXPathTemplate =
            "//div[contains(@class,'claim_line')][ul[1]/li[1]/span[text()='{0}']]/ul[{1}]/li[{2}]/span";

        public const string ClearCssLocator = "div.current_viewing_searchlist > div span.span_link";
        public const string ClaimDetailsCssLocator = "section.claim_action";
       
        public const string MoreOptionCssLocator = "li[title='More Options'] > span";
        public const string MoreOptionListCssLocator = "li[title='More Options'] span.span_link";

        public const string PatientClaimHistoryIconCssLocator = "span.patient_claim_history.icon";
        public const string OriginalClaimDataLinkXpath = "//span[text()='Original Claim Data']";
        public const string ClaimProcessingHistoryLinkXpath = "//span[text()='Claim Processing Hx']";
        public const string InvoiceLinkXpath = "//span[text()='Invoice Data']";

        public const string HciRunLabelCssLocator = "label[title='HCIRUN']";
        public const string HciRunValueCssLocator = "label[title='HCIRUN'] + div";
        public const string HciVoidValueCssLocator = "label[title='HCIVOID'] + div";
        public const string HciProvSpecValueCssLocator = "label[title='PROV_SPEC'] + div";
        public const string HciProvNameValueCssLocator = "label[title='PROV_NAME'] + div";

        public const string CreateCssLocator = "div.current_viewing_searchlist > div > button";
        public const string AppealSequenceCssTemplate = "span[title='{0}']";
        public const string AppealSequenceStatusXPathTemplate = "//span[text()='{0}']/../../li[3]/span";
        public const string AppealStatusByRowOfAppealXPathTemplate = "//label[text()='Appeals']/../../../section[2]//ul/li[1]//li[3]/span";
        public const string AppealTypeByRowOfAppealXPathTemplate = "//label[text()='Appeals']/../../../section[2]//ul/li[{0}]//li[2]/span";
        public const string AppealLetterXPathTemplate = "//span[text()='{0}']/../../li[4]/span";
        public const string AppealSequencesXPath = "//label[text()='Appeals']/../../../section[2]/ul/li";
        public const string AppealSequenceXPath = "//label[text()='Appeals']/../../../section[2]//span";

        public const string AppealSequenceXpathByRow =
            "//label[text()='Appeals']/../../../section[2]/ul/li[{0}]//span";

        public const string EOBMessageCssLocator = "div.line_modifiers:has(span:contains({0}))>ul>ul>li:nth-of-type(3)";//jquery

        public const string TrigClaNoXpathSelectorInEobMsg =
            "//ul[..//span[text()='{0}']]//div//div/ul/ul/li[3]//a[contains(@class,'popup_link')]";

        public const string ProcDescFlaggedLinesCssLocator = "div.flagged_line > li > ul > li:nth-of-type(4)";
        public const string ProcCodeFlaggedLinesCssLocator = "div.flagged_line:nth-of-type({0}) > li > ul > li:nth-of-type(3)";
        public const string AppealSequenceListXpath = "//label[text()='Appeals']/../../../section[2]//ul/li[1]/span";
        public const string AppealStatusListXpath = "//label[text()='Appeals']/../../../section[2]//ul/li[3]/span";
        public const string AppealTypeListXpath = "//label[text()='Appeals']/../../../section[2]//ul/li[2]/span";

        public const string FlagLineWithTriggerClaimXPath = "//ul[@class='single_row full_row' and li[a[contains(text(),'{0}')]]]";

        public const string ProcDescFlaggedLineOfFlaggedLinesDivXPath =
            "//div[1]/ul[contains(@class,'component_nested_row')]/ul/li[6]";
        public const string ProcDescClaimLinesDivCssLocator =
            "section.claim_lines ul.component_item_row > li:nth-of-type(5) > span";
        public const string ProcDescClaimDollarDetailsDivCssLocator =
            "section.claim_dollar_details ul.component_item_row > li:nth-of-type(5) > span";

        public const string ApproveCssLocator = "li.is_active > span.approve.toolbar_icon";
        public const string PosFlaggedLinesCssLocator =
            "div.flagged_line > li >ul >li:nth-of-type(5) > span";

        public const string SpecialtyOnFlaggedLinesByValueXPathLocator =
            "//div[contains(@class,'flagged_line ')]//li[span[text()='{0}'] and span[@title='{1}']]";
        public const string TriggerClaimCssSelector =
            "ul.component_item_list >div:nth-of-type(1) >li >div:nth-of-type(1) >ul > ul >li:nth-of-type(7)";
        public const string TriggerClaimsXPath =
            "div.line_modifiers > ul > ul >li:nth-of-type(7):not(.display_empty) > span";
        public const string FirstFlagDetailsCssSelector = "div.flagged_line >li:nth-of-type(1) >div:nth-of-type(1)";
        public const string PosFlagDetailsCssLocator =
            "section.claim_lines section.component_content>div  > ul:nth-of-type(3) >li ";
        public const string PosLineDetailsCssLocator = "div.line_modifiers >li>div>ul>ul:nth-of-type(3) >li>span";
        public const string PosClaimLinesCssSelector =
            "div.claim_line > ul:nth-of-type(2) >li:nth-of-type(2) > span";
        public const string TriggerCodeXPath = "//div[contains(@class,'line_modifiers ')]/ul/ul/li[5]/span[text()!='']";
        public const string RevenueCodeCssLocator = "div.flagged_line  >li >ul >li:nth-of-type(6).action_link > span";

        public const string ProviderSequenceTextLabelCss = "section.claim_action  label[title='Provider Seq'] + div";
        public const string LineDetailsSectionCss = "ul.component_item_list > div:nth-of-type(1) >li >ul";
        public const string FlagDetailsSectionCss = "div.flagged_line  > li > div > ul";
        public const string RightQuadrantLabelXPath = "//div[section[contains(@class,'flagged_lines')]]/div//section/section[contains(@class,'component_header')]//label";
        public const string DxCodeNotPresentDivCssLocator = "section.component_content > p.empty_message";
        public const string ClaimLinesCssLocator = "a.claim_lines_restore";
        public const string RestoreFlagsIconCssLocator = "li.is_active span.icon.toolbar_icon.restore_all";
        public const string TooltipId = "tool_tip_wrap";
        public const string DeleteButtonOfEditAllFlagSectionCssLocator = "span.line_delete.small_toolbar_icon";
        public const string RestoreButtonOfEditAllFlagSectionCssLocator = "span.line_restore.small_toolbar_icon";
        public const string DeleteButtonOnEditFormLineLevel = "ul.component_item_list span.line_delete.small_toolbar_icon";
        public const string RestoreButtonOnEditFormLineLevel = "ul.component_item_list span.line_restore.small_toolbar_icon";
        public const string SaveButtonCssLocator = "section:not([style*='none']) >ul button.work_button";
        public const string FlagDetailsTriggerClaimsList = "span.fot-trigger-claim";
        public const string FlagDeatailsTrigerClaimsTemplate = "span.fot-trigger-claim:nth-of-type({0})";
        public const string FlagLineDescRowColWiseCssLocator =
            "div.flagged_line  > li > div:nth-of-type({0}) >ul> ul>li:nth-of-type({1})";

        public const string TriggerAltclaimNoOnFlagLinesCssLocator =
            "div.flagged_line:nth-of-type({0}) div.line_modifiers:nth-of-type({1}) li.edit_flag+li>a";

        public const string SaveButtonEditFlagCssLocator =
            "section.flag_line_edits >ul:nth-of-type(2) >div > div > button";

        public const string EditReasonCodeDropDownCssLocator =
            "section.component_content section:not([style*='none'])  ul > div > div.large-select > section > div.select_input > span";
        public const string EditReasonCodeCssLocator = "section.component_content section:not([style*='none'])  ul > div > div.large-select > section > div.select_input > input";
        public const string EditReasonCodeOptionsCssLocator = "section.component_content section:not([style*='none'])  ul > div > div.large-select > section > div.select_input + ul > li";
        public const string EditReasonCodeSelectorXPath = "//section[contains(@class,'flag_line_edits') or contains(@class,'line_flag_edits') or contains(@class,'claim_flag_edits') or contains(@class,'claim_line_edits')][not(contains(@style,'none'))] //li[text()='{0}']";

        public const string EditReasonCodeLabelCssSelector =
            "section.component_content section:not([style*='none'])  ul > div > div.large-select >label";

        public const string EditReasonCodeInEditAllFlagCssLocator =
            "section.claim_flag_edits  input";
        public const string EditReasonCodeSelectorInEditAllFlagXPath =
            "//section[contains(@class,'claim_flag_edits')]/ul/div[3]/div/section/ul/li[text()='{0}']";

        public const string DashboardButtonCssLocator = "a.dashboard";
        public const string HelpButtonCssLocator = "a.help_center";
        public const string FlaggedLineCssLocator = "div.flagged_line  > li > div > ul";
        public const string PageErrorCloseId = "nucleus_modal_close";
        public const string SaveEditButtonCssLocator = "section.add_flag_form button.work_button";
        public const string SaveSugButtonCssLocator= "section.flag_line_edits button.work_button";
        public const string PageErrorModalPopupContentDivId = "nucleus_modal_content";
        public const string DollarIconCssLocator = "a.toolbar_icon.dollars";
        public const string WorkListCssLocator = "span.sidebar_icon";
        public const string ClaimSearchIconCssLocator = "span.search_icon";
        public const string ClaimSequenceXPath = "//input[@placeholder='Claim Sequence']";
        public const string FindXPath = "//button[text()='Find']";
        public const string SearchIconCssLocator = "span.icon.filter_icon";
        //public const string ClaimSeqCssLocator = "div.data_point_value";
        public const string ClaimSeqCssLocator = "//div[label[contains(text(),'Claim Seq')] or label[contains(text(),'Bill Seq')]]/div[contains(@class,'data_point_value')]";
        public const string PatientSeqXPath = "//div[label[contains(text(),'Patient Seq')]]/div[contains(@class,'data_point_value')]//span";

        public const string ClaimSequenceValueInClaimActionCssLocator =
            "section.claim_action  label[title='Claim Seq'] + div";
        public const string SugUnitInputFieldCssLocator = "div.basic_input_component.field.numeric_input_component > input[placeholder='Sug Units']:not(:disabled)";
        public const string SugPaidInputFieldCssLocator = "div.basic_input_component.field.numeric_input_component >input[placeholder='Sug Paid']:not(:disabled)";
        public const string SugCodeInputFieldCssLocator = "div.basic_input_component > input[placeholder='Sug Code']:not(:disabled)";
        public const string SystemDeletedFlagLineCssLocator = "ul.is_deleted > ul";
        public const string SystemDeletedFlagLineClientXPath = "//ul[@class='full_row']/li[@class='flag full_row nested_row deleted']/section[ul/li[2]/a[@class='popup_link line_flag_should_not_be_worked']]";
        public const string ViewSystemDeletedFlagIconCssLocator = "li.is_active > span.system_deleted.toolbar_icon";
        public const string EditFlagsIconCssLocator = "li.is_active > span.icon.toolbar_icon.edit_icon";
        public const string DosTextLabelCssLocator = "section.flagged_lines div.flagged_line ul.component_item_row > li:nth-of-type(2)";

        public const string AllDosValuesCssLocator = "section.flagged_lines div.flagged_line ul.component_item_row > li.line_date:nth-of-type(2)";
        public const string TrigClaimLinkXPathTemplate = "//div[contains(@class,'line_modifiers ')]/ul/ul/li[7][not(contains(@class,'display_empty'))] /span[text()!='']";
        public const string EditFlagsAreaCssLocator = "section[style='']";
        public const string FlagRowCssLocator = "div.flagged_line  > li > div > ul";
        public const string DeletedFlagRowCssSelector = "ul.is_deleted";
        public const string FieldErrorXPath = "//span[text()='Reason code required']";
        public const string FlaggedLinesCssLocator = "div.line_modifiers > ul:not([class*='is_deleted']) > ul";
        public const string DeletedFlaggedLinesCssLocator = "div.line_modifiers > ul.is_deleted > ul";
        public const string LineDeleteCssLocator = "section.flag_line_edits >ul> li >ul > div >ul > li > span.line_delete ";
        public const string LineRestoreCssLocator = "section.flag_line_edits >ul> li >ul > div >ul > li > span.line_restore ";
        public const string PageErrorPopupModelId = "nucleus_modal_wrap";
        public const string FirstDxCodeCssLocator = "span.dxcodelink.first>a";
        public const string FlagDetailsDivCssLocator = "section.flag_details";
        public const string FlagAuditTrailInfoCssLocator = "section.audit_line.full_row";
        public const string WorkingAjaxMessageCssLocator = "div.small_loading";
        public const string FlagListForLineCssLocator = "div.flagged_line:nth-of-type({0}) div.line_modifiers > ul:not([class*='is_deleted']) > ul li.edit_flag  span";
        public const string FlagListWithDeletedFlagForLineCssLocator = "div.flagged_line:nth-of-type({0}) div.line_modifiers:not([style*='none'])  li.edit_flag  span";

        public const string EditAllFlagsOnLineCssLocator =
            "div.flagged_line > li > ul:nth-of-type(2)  span.small_edit_icon.is_active";

        public const string DxCodeByRowCssTemplate =
            "ul.component_item_list >li:nth-of-type({0}) >ul >li:nth-of-type(2)>span";

        public const string Q2LeftHeaderCssLocator = "section.claim_secondary_view  div.component_header_left>label";

        public const string VersionTextId = "build_version";
        public const string DeleteAllFlagsIconCssLocator = "section.flagged_lines >section> div:nth-of-type(2) >ul>li:nth-of-type(1)";

        public const string ApproveClaimIconCssLocator =
            "section.flagged_lines >section> div:nth-of-type(2) >ul>li:nth-of-type(2)";
        public const string NextIconXPath =
            "//li[span[@class='next is_enabled toolbar_icon icon']]";
        public const string EditIconCssLocator =
            "section.flagged_lines >section> div:nth-of-type(2) >ul>li:nth-of-type(3)";

        public const string FlagAddIconCssLocator =
            "section.flagged_lines >section> div:nth-of-type(2) >ul>li:nth-of-type(4)";
        public const string TransferIconCssLocator =
           "section.flagged_lines >section> div:nth-of-type(2) >ul>li:nth-of-type(5)";
        
        public const string DeleteAllFlagsCssLocator = "div.lines li.is_active > span.icon.toolbar_icon.delete_all";
        public const string DeleteAllFlagsCssLocatorDisabled = "div.lines li.is_disabled> span.icon.toolbar_icon.delete_all";
        public const string DeleteLineIconCssLocator = "div.flagged_line > li > ul:nth-of-type(2)  span.small_delete_icon.is_active";
        public const string DeleteFlagIconCssLocator = "div.flagged_line > li  > div span.small_delete_icon.is_active";

        public const string RestoreLineIconCssLocator =
            "div.flagged_line > li > ul:nth-of-type(2)  span.small_restore_icon.is_active";
        public const string RestoreFlagIconCssLocator = "div.line_modifiers span.small_restore.is_active";
        public const string PageErrorModalPopupId = "nucleus_modal_wrap";
        public const string FlagLevelEditIconXPathTemplate = "//div[contains(@class,'flagged_line')]/li/div/ul/ul[li[3]/span[text()='{0}']]/li[1]/ul/li[1]";
        public const string SavingsXPath = "//div[contains(@class,'flagged_line ')]/li[div/ul/ul/li[3]/span[text()='{0}']]/ul[2]/li[label[contains(text(),'Savings')]]/span";
        public const string SugPaidAmountXPath = "//div[contains(@class,'flagged_line ')]/li[div/ul/ul/li[3]/span[text()='{0}']]/ul[2]/li[label[contains(text(),'Sug Paid')]]/span";
        public const string FlagLevelUnitXPathTemplate = "//ul[@class='full_row']/li[@class='flag full_row nested_row']/section/ul[li[3]/a[text()='{0}']]/li[10]/span[2]";
        public const string ClaimLevelUnitXPathTemplate = "//div[contains(@class,'flagged_line')]/li/div/ul/ul[li[3]/span[text()='{0}']]/li[10]/span";
        public const string DisabledAddDocumensCssLocator = "li.is_disabled > span.add_documents.icon.toolbar_icon";
        public const string DisabledDeleteAllCssLocator = "li.is_disabled > span.icon.toolbar_icon.delete_all";
        public const string DisabledRestoreAllCssLocator = "li.is_disabled > span.icon.toolbar_icon.restore_all";
        public const string DisabledAddCssLocator = "li.is_disabled > span.icon.toolbar_icon.add_icon";
        public const string AddIconCssLocator = "span.icon.toolbar_icon.add_icon";
        public const string DisabledEditAllCssLocator = "li.is_disabled > span.icon.toolbar_icon.edit_icon";
        public const string DisabledApproveNextCssLocator = "li.is_disabled > span.transfer_approve.icon.toolbar_icon";
        public const string DisabledApproveCssLocator = "li.is_disabled > span.approve.toolbar_icon.icon";
        public const string DisabledNextCssLocator = "li.is_disabled > span.next.toolbar_icon.icon";
        public const string DisabledTransferCssLocator = "li.is_disabled > span.transfer.icon.toolbar_icon";
        public const string LockIconCssSelector = "span.icon.medium_icon.locked";

        public const string AddAppealIconCssSelector = "a.add_appeal";
        public const string DisabledAddAppealIconCssSelector = "a.add_appeal.disabled";
        public const string EditFlagDetailSectionCssLocator = "section.flag_details li.edit_flag.line_flag_cannot_be_worked";
        public const string FlagDetailLabelCssLocatorTemplate = "div.flag_details.is_active > ul > li:has(label:contains({0}))";
        public const string FlagDetailLabelCssLocatorByPreceedingLabel = "div.flag_details.is_active > ul > li:has(label:contains({0}))+ li>label";
        public const string FlagDetailDataPointValueCssLocator = "div.flag_details.is_selectable.is_active > ul:nth-of-type({0})>li:nth-of-type({1})>span";
        public const string CustomizationLabelCssLocator = "div.ember-view.component_item.flag_details.column_100.is_selectable.is_active > ul > li:nth-of-type(5) >label.data_point_label";
        public const string CustomizationValueCssLocator = "div.ember-view.component_item.flag_details.column_100.is_selectable.is_active > ul > li:nth-of-type(5) >span.data_point_value";
        public const string BackButtonXPath = "//a[@class='icon toolbar_icon browser_back']";
        public const string NextButtonCss = "span.next.toolbar_icon.icon";
        public const string ProcCodePopupLinkCssLocator = "div.flagged_line  >li >ul >li:nth-of-type(3).action_link > span";
        public const string ProcCodePopuLinkXPathTemaplate = ".//a[@popupname={0}]";
        public const string ClaimLineProcCodePopupLinkXPathTemaplate = "//div[contains(@class,'flagged_line ')]/li/ul/li[3]/span[text()='{0}']";
        public const string WorkListControlId = "worklist_component";
        public const string WorkListControlCssLocator = "section:not(.is_hidden).is_slider";
        public const string WorkListHeaderCssLocator = "section.is_slider:not(.is_hidden) label.component_title";
        public const string CloseWorkListBtnCssLocator = "section.is_slider:not(.is_hidden) span.close_sidebar";
       

        public const string WorkListOptionSelectorDropDownCssLocator = "span.options.toolbar_icon";
        public const string WorkListOptionsXPathTemplate = "//ul[@class='is_visible option_list']//span[text()='{0}']";
        public const string FindIconCssLocator = "span.icon.filter_icon";
        public const string WorkListIconCssLocator = "span.icon.list_icon";
        public const string ClaimSequenceLabelCssLocator = "div.current_viewing_searchlist > div > label";
        public const string ClaimNoLabelCssLocator = "div.current_viewing_searchlist > div:nth-of-type(2) > label";
        public const string ClaimNoInputXPath = "//input[@placeholder='Claim No']";
        public const string TransferCssSelector = "span.transfer.icon.toolbar_icon";
        public const string TransferApproveCssSelector = "span.transfer_approve.toolbar_icon.icon";

        public const string NextClaimInWorkListQueueXPath =
            "//section[@class='worklist_section worklist_queue']/ul/li[1]/div/span";
        public const string FlaggedLineRowXPath =
            "//div[@id='flagged_lines']/ul/div[1]/li/ul[1]/li[@class='flag full_row nested_row']"; //First Flag of first claim line
        public const string FlaggedLineDeletedRowXPath =
            "//div[@id='flagged_lines']/ul/div[1]/li/ul[1]/li[@class='flag full_row nested_row deleted']"; //First Flag of first claim line
        public const string SecondRestoreButtonXPath = "//div[@id='flagged_lines']/ul/div[1]/li/ul[2]/li[@class='flag full_row nested_row deleted']/section/ul/li[1]/span[2]";
        public const string DisabledSwitchButtonXPath = "//div[@id='flagged_lines']/ul/div[1]/li/ul[1]/li[contains(@class,'flag full_row nested_row')]/section/ul/li[1]/span[contains(@class,'is_disabled')]";  //Gives first switch button
        public const string RestoreButtonXPath = "//div[@id='flagged_lines']/ul/div[1]/li/ul[1]/li[@class='flag full_row nested_row deleted']/section/ul/li[1]/span[2]";
        public const string DeleteButtonXPath = "//div[@id='flagged_lines']/ul/div[1]/li/ul[1]/li[@class='flag full_row nested_row']/section/ul/li[1]/span[2]";
        public const string SwitchButtonXPath = "//div[@id='flagged_lines']/ul/div[1]/li/ul[1]/li[contains(@class,'flag full_row nested_row')]/section/ul/li[1]/span[contains(@class,'icon small_icon small_switch_icon')]";
        public const string TransferDropDownBtnCssLocator = "span.icon.toolbar_icon.chevrons.inactive";
        public const string TransferOptionXPath = "//ul[@class='option_list']/li[1]/span";
        public const string FlaggedLineDeletedByLineNoAndFlaggedRow = "div.flagged_line:nth-of-type({0})>li>div:nth-of-type({1})>ul.is_deleted";
        public const string FlaggedLineNotDeletedByLineNoAndFlaggedRow = "div.flagged_line:nth-of-type({0})>li>div:nth-of-type({1})>ul:not([class*='is_deleted'])";

        public const string TransferOptionMovedToLeftXPath =
            "//li[@title='Transfer']/span";
        public const string LogicIconMovedToLeftXPath = "li[title=Logics] span.is_active";
        public const string TransferApproveOptionXPath = "//ul[@class='option_list']/li[3]/span";
        public const string TransferDropDownOptionsDivCssLocator = "ul.option_list";
        public const string TransferClaimWidgetCssLocator = "section.transfer_form";
        public const string TransferClaimWidgetHeaderCssLocator = "section.transfer_form >ul:nth-of-type(1) > div:nth-of-type(1) >header";
        public const string StatusComboBoxCssSelector = "section.form_component div:has(label:contains(Status))>section>div.select_input";
        //public const string StatusComboBoxCssSelector = "//section[contains(@class,'flagged_lines')]/section[2]/section[2]/ul/div[2]/div/section/div/input";
        public const string StatusCodeCssTemplate = "div:has(label:contains(Status))>section>ul>li:contains({0})";
        //public const string StatusCodeCssTemplate = "//section[contains(@class,'flagged_lines')]/section[2]/section[2]/ul/div[2]/div/section/ul/li[text()='{0}']";
        public const string StatusOptionListCssSelector = "div:has(label:contains(Status))>section>ul>li";
        //public const string StatusOptionListCssSelector = "//label[text()='Status']/../section/ul/li";
        public const string CancelWidgetCssSelector = "section:not([style*='none']) >ul span.span_link";
        public const string FlagLevelCancelLinkCssLocator = "section.add_flag_form span.span_link";

        public const string ReturnToClaimXPathTemplate = "//li[span[text()='{0}']]/span[1]";
        public const string NextClaimsInWorklistXPath = "//header[text()='Next Claims in Work List']/../parent::section";
        public const string NextClaimsInWorklistSectionHeaderXPath = "//header[text()='Next Claims in Work List']";
        public const string ClaimListInNextClaimsInWorklistXPath = "//header[text()='Next Claims in Work List']/parent::span/ul/li";
        public const string NoDataTextInNextClaimsInWorklistXPath = "//header[text()='Next Claims in Work List']/parent::span/p";
        public const string AdditionalLockedClaimsSectionXPath = "//header[text()='Additional Locked Claims']/../parent::section";
        public const string AdditionalLockedClaimsSectionHeaderXPath = "//header[text()='Additional Locked Claims']";
        public const string PreviouslyViewedClaimsSectionXPath = "//header[text()='Previously Viewed Claims']/../parent::section";
        public const string PreviouslyViewedClaimsSectionHeaderXPath = "//header[text()='Previously Viewed Claims']";
        public const string AddLogicIconCssLocator = "span.is_active.add_logic.icon.small_icon";
        public const string ClientLogicIconCssLocator = "span.is_active.client_logic.icon.small_icon";
        public const string CotivitiLogicIconCssLocator = "span.is_active.internal_logic.icon.small_icon";
        public const string LockedAddLogicIconCssLocator = "span.is_disabled.add_logic.icon.small_icon";
        public const string LockedCotivitiLogicIconCssLocator = "span.is_disabled.internal_logic.icon.small_icon";
        public const string LockedClientLogicIconCssLocator = "span.is_disabled.client_logic.icon.small_icon";
        

        public const string FlaggedLinesProcDescriptionWithEllipsisCssSelector = "div.flagged_line>li>ul>li:nth-of-type(4)";
        public const string FlaggedLinesFlagDescriptionWithEllipsisCssSelector = "div.line_modifiers > ul > ul > li:nth-of-type(6) > span:not([title=''] )";
        public const string ProviderDescriptionInFlagDetailsRowWithEllipsisXPath = "//li[@class='flag_provider data_point' and @title]";
        public const string HciRunFieldWithEllipsisCssSelector = "div.component_item > ul:nth-of-type(3) > div:nth-of-type(1) div:not([title=''] )";
        public const string HciVoidFieldWithEllipsisCssSelector = "div.component_item > ul:nth-of-type(3) > div:nth-of-type(2) div:not([title=''] )";
        public const string HdrPayornameFieldWithEllipsisCssSelector = "div.component_item > ul:nth-of-type(3) > div:nth-of-type(3) div:not([title=''] )";
        public const string HdrPhoneFieldWithEllipsisCssSelector = "div.component_item > ul:nth-of-type(3) > div:nth-of-type(4) div:not([title=''] )";
        public const string HdrProvTypeFieldWithEllipsisXPath = "//div[@class='row last']/div[5]/div[@class='row_item_value clipped' and @title]";
        public const string HdrSpecFieldWithEllipsisXPath = "//div[@class='row last']/div[6]/div[@class='row_item_value clipped' and @title]";
        public const string SpecialtyWithEllipsisCssSelector = "div.component_item > ul:nth-of-type(2) > div:nth-of-type(2) div:not([title=''] )";
        public const string ProviderNameWithEllipsisCssSelector = "div.component_item > ul:nth-of-type(2) > div:nth-of-type(3) div:not([title=''] )";
        public const string ClaimLinesProcDEscriptionWithEllipsisCssSelector = "div.claim_line > ul:nth-of-type(1) > li:nth-of-type(5) >span:not([title=''] )";
        public const string ClaimDollarDetailsProcDescriptionWithEllipsisCssLocator = "div.claim_dollar_detail > ul:nth-of-type(1) > li:nth-of-type(5) >span";
        public const string NewClaimActionDataHolderDivCssLocator = ".claim";
        public const string PlanSelectOptionXPath = "//div[contains(@class,'pci_worklist')]/div[3]/div/section/input";
        public const string PlanDropDownXPath = "//div[contains(@class,'pci_worklist')]/div[3]/div/section/span";

        public const string ClaimStatusDropDownXPath =
            "//div[contains(@class,'pci_worklist')]/div[1]/div/section/div/span";

        public const string ClaimStatusFCIDropDownXPath =
            "//div[contains(@class,'fci_worklist')]/div[1]/div/section/div/span";

        public const string ClaimStatusXPath = "//div[contains(@class,'pci_worklist')]/div[1]/div/section/div/input";
        public const string ClaimStatusSelectOptionXPath = "//div[contains(@class,'pci_worklist')]/div[1]/div/section/div/input";
        public const string ClaimSubStatusDropDownXPath = "//div[contains(@class,'pci_worklist')]/div[2]/div/section/span";
        public const string ClaimSubStatusTextXPath = "//div[contains(@class,'pci_worklist')]/div[2]/div/section/input";
        public const string ClaimSubStatusSelectOptionXPathTemplate = "//div[contains(@class,'pci_worklist')]/div[2]/div/section/ul/section/li[text()='{0}']";
        public const string ClaimTypeDropDownXPath = "//div[contains(@class,'pci_worklist')]/div[4]/div/section/div/span";
        public const string ClaimTypeXPath = "//div[contains(@class,'pci_worklist')]/div[5]/div/section/div/input";
        public const string ClaimTypeListOptionsXPath = "//div[contains(@class,'pci_worklist')]/div[4]/div/section/ul/li";
        public const string ClaimTypeListOptionsSelectorXPathTemplate =
            "//div[contains(@class,'pci_worklist')]/div[5]/div/section/ul/li[text()='{0}']";

        public const string TriggerClaimLineAdjustedPaidXPath = "//li[@class='data_point adj_paid hide_excessive_text']/span[2]";
        public const string FlagDropDownAddFlagSectionXPath= "//section[contains(@class,'add_flag_form')]//div[label[text()='Flag']]//input";
        public const string FlagSrcDropDownAddFlagSectionXPath =
            "//section[contains(@class,'add_flag_form')]//div[label[text()='Flag Source']]//input";
        public const string FlagListOptionsAddFlagSectionXPathTemplate = "//section[contains(@class,'add_flag_form')]//div[label[text()='Flag']]//ul/li[text()='{0}']";
        public const string FlagListOptionsInFlagDropdownInAddFlagForm = "//section[contains(@class,'add_flag_form')]//div[label[text()='Flag']]//li[contains(@class,'option')]";
        public const string FlagSrcDropDownAddFlagSectionXPathTemplate = "//section[contains(@class,'add_flag_form')]//div[label[text()='Flag Source']]//ul/li[text()='{0}']";

        public const string FlagDropDownXPath = "//div[contains(@class,'pci_worklist')]/div[6]/div/section/div/span";
        public const string FLagListOptionsXPath = "//div[contains(@class,'pci_worklist')]/div[6]/div/section/ul/li";
        public const string FLagListOptionsSelectorXPathTemplate = "//div[contains(@class,'pci_worklist')]/div[7]/div/section/ul/li[text()='{0}']";
        public const string FlagXPath = "//div[contains(@class,'pci_worklist')]/div[7]/div/section/div/input";
        public const string BatchIdDropDownXPath = "//div[contains(@class,'pci_worklist')]/div[7]/div/section/div/span";
        public const string BatchIdDropDownXPathOtherPage = "//div/div[7]/div/section/div/span";
        public const string BatchIdXPath = "//div[contains(@class,'pci_worklist')]/div[8]/div/section/div/input";
        public const string BatchListOptionsXPath = "//div[contains(@class,'pci_worklist')]/div[7]/div/section/ul/li";
        public const string BatchListOptionsSelectorXPathTemplate = "//div[contains(@class,'pci_worklist')]/div[8]/div/section/ul/li[text()='{0}']";
        public const string BatchListOptionsSelectorXPathTemplateOtherPage = "//div/div[7]/div/section/ul/li[text()='{0}']";
        public const string TriggerClaimLineInputXPath = "//input[@placeholder='Trigger Claim Line']";

        public const string AllowedOnClaimDetailsByLineBadgeXPathTemplate =
            "//div[contains(@class,'claim_dollar_detail')][ul[1]/li[1]/span[text()='{0}']]/ul[2]/li[3]/span";
        public const string BilledOnClaimDetailsByLineBadgeXPathTemplate =
            "//div[contains(@class,'claim_dollar_detail')][ul[1]/li[1]/span[text()='{0}']]/ul[2]/li[2]/span";
        public const string AdjPaidOnClaimsDetailsByLineBadgeXPathTemplate =
            "//div[contains(@class,'claim_dollar_detail')][ul[1]/li[1]/span[text()='{0}']]/ul[2]/li[4]/span";
        public const string ClaimDollarDetailsIconCssLocator = "a.dollars";
        public const string ClaimDollarDetailsByLineWithLabelXPathTemplate =
            "//div[contains(@class,'claim_dollar_detail')][{0}]//label[text()[contains(.,'{1}')]]/../span";

        public const string UnitsOnFlagLineCssTemplate = "div.flagged_line:nth-of-type({0})>li>ul>li:nth-of-type(8)>span";
        public const string FlagLineCssTemplate = "div.flagged_line:nth-of-type({0})>li>ul>li.numeric_badge>span";
        public const string ClaimDetailsLastRowCssLocator = "section.claim_action ul:nth-of-type(3) >div:last-of-type >label"; //to check no ciu icon is present next to prov name
        public const string DentalHistoryCssSelector = "div.flagged_line>li>ul:nth-of-type(1)>li>span[title={0}]";

        #region ModifierExplanationFlagDetails

        public const string ModifierExplanationOfFirstFlagRowCssLocator =
            "div.line_modifiers:nth-of-type(1) > ul > ul > li:nth-of-type(9) > span";

        public const string ModifierNumberFlagDetailsXPath =
            "//div[@id='flag_detail']/ul/li/section/section/div/ul/li[@class='line_mod data_point']/span[@class='grid_value clipped']";

        public const string ModifierDescriptionFlagDetailsXPath =
            "//div[@id='flag_detail']/ul/li/section/section/div/ul/li[@class='line_mod_description data_point']/span[@class='grid_value clipped']";
        #endregion

        #region ModifierExplanationLineDetails
        public const string ModifierExplanationFlaggedLinesCssLocator =
            "div.flagged_line:nth-of-type(1) > li > ul > li:nth-of-type(7) > span";

        public const string ModifierNumberLinesDetailsXPath =
            "//div[@id='line_detail']/ul/li/section/div/ul/li[@class='line_mod data_point']/span[@class='grid_value clipped']";

        public const string ModifierDescriptionLinesDetailsXPath = 
            "//div[@id='line_detail']/ul/li/section/div/ul/li[@class='line_mod_description data_point']/span[@class='grid_value clipped']";

       
        #endregion

        public const string PlanOptionXPathTemplate =
            "//ul[@class=' is_visible select_options']/section[@class='available_options']/li[text()='{0}']";

        public const string ClaimStatusOptionXPathTemplate =
            "//div[contains(@class,'pci_worklist')]/div[1]/div/section/ul/li[text()='{0}']";

        public const string ClaimStatusFCIOptionXPathTemplate =
            "//div[contains(@class,'fci_worklist')]/div[1]/div/section/ul/li[text()='{0}']";

        public const string RightComponentHeaderSectionCssLocator = "section.claim_secondary_view label.component_title";
        public const string ViewAppealIconCssLocator = "span.icon.toolbar_icon.appeals";
        public const string ViewDxCodesIconCssLocator = "span.icon.toolbar_icon.dx_codes";
        public const string NoAppealsMessageSectionCssLocator = "p.empty_message";
        public const string ViewAppealIconWithBadgeCssLocator = "span.icon.toolbar_icon.appeals > span.icon_badge";
        public const string TriggerClaimDataCssLocator = "li.flag_seq.data_point > a";
        public const string DxCodeLabelCssLocator = "li.line_dx.data_point > span.grid_label";
        public const string DxCodeDataCssLocator = "li.line_dx.data_point > span.dxcodelink.first > a.popup_link";
        public const string DxVersionDataCssLoctor = "li.line_dx_indicator.data_point > span.grid_value.clipped";
        public const string DxDescriptionDataCssLocator = "li.line_dx_description.data_point > span.grid_value.clipped";
        public const string EmptyFlagMessageCssLocator = "section.flagged_lines p.empty_message";
        public const string FirstEnabledSwitchButtonCssLocator = "span.icon.small_icon.small_switch_icon.is_active";
        public const string QaAuditGreenIconCssSelector = "span.qa_mode";
        public const string QaAuditRedIconCssSelector = "span.qa_lock_mode";
        public const string QaPassIconCssLocator = "span.qa_pass";
        public const string QaFailIconCssLocator = "a.qa_fail";
        //public const string ClaimStatusDivInClaimActionCssLocator = "section.claim_action > div > ul > div:nth-of-type(7) > div";
        public const string ClaimStatusDivInClaimActionCssLocator = "section.claim_action  ul:nth-of-type(1) div:nth-of-type(7) > div";
        public const string BatchIdDivInClaimActionXPath = "//div[label[text()='Batch ID']]/div";
        public const string ProviderNameTriggerClaimLineXPath = "//div[@class='claim_info']/section/div[2]/span[2]";
        public const string TriggerClaimLineWithDosXPathTemplate =
            "//section[@class='add_trigger bottom_section']/div[{0} and section/div[1]/span/a[text()='{1}'] or section/div[1]/span[text()='{1}']]/ul[{2} and li[text()='{2}']]/li[text()='{3}']";
        public const string TriggerClaimLineDosValueXPathTemplate =
            "//section[@class='add_trigger bottom_section']/div[{0}]/section/div[1]/span/a | //section[@class='add_trigger bottom_section']/div[{0}]/ul[{1}]/li[1] | //section[@class='add_trigger bottom_section']/div[{0}]/ul[{1}]/li[2]";
        public const string AddFlagDivId = "section.add_flag_form";
        public const string FirstClaimLineToAddFlagCssSelectorTemplate = "section.available_claim_lines > section:nth-of-type(2) >ul >li:nth-of-type({0})";
        public const string ClaimActionPageTitleCss = "label.page_title";
        public const string AddNotesIndicatorCssSelector = "span.icon.toolbar_icon.add_notes";
        public const string ViewNotesIndicatorCssSelector = "span.icon.toolbar_icon.notes"; 
        public const string RedBadgeNotesIndicatorCssSelector = "span.notes.icon.toolbar_icon>span.icon_badge";
        public const string InputFieldInNotesHeaderByLabelCssTemplate = @"section:has(>ul>div>header:contains(Notes)) div:has(>label:contains({0})) input";
        public const string NoteTypeDropDownInputListByLabelXPathTemplate =
            "//span[text()='{0}']/../following-sibling::div/section//ul//li";
        public const string NoteRecordsListByColCssTemplate =
        "ul.component_item_list>div.note_row>ul>li:nth-of-type({0}) span";
        public const string NoteRecordsByRowColCssTemplate =
            "ul.component_item_list>div:nth-of-type({1}).note_row>ul>li:nth-of-type({0}) span";
        public const string NoteRecordsByRowColAttributeCssTemplate =
           "ul.component_item_list>div:nth-of-type({0}).note_row>ul>li:nth-of-type({1})";
        public const string NotePencilIconByRowCssTemplate =
            "ul.component_item_list>div:nth-of-type({0})>ul>li span.small_edit_icon";
        public const string NotePencilIconByNameCssTemplate =
            "div.note_row:has(span:contains({0})) span.small_edit_icon";
        public const string NoteCarrotIconByRowCssTemplate =
            "ul.component_item_list>div:nth-of-type({0})>ul>li span.small_caret_right_icon";
        public const string NoteCarrotIconByNameCssTemplate =
           "ul.component_item_list>div.note_row:has(span:contains({0}))>ul>li span.small_caret_right_icon";
        public const string NoteCarrotDownIconByRowCssTemplate =
            "ul.component_item_list>div:nth-of-type({0})>ul>li span.small_caret_down_icon";
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
        public const string NotesEditFormSaveButtonCssLocator = "div:nth-of-type({0}).note_row button.work_button";
        public const string NotesEditFormSaveButtonByNameCssLocator = "div.note_row:has(span:contains({0})) button.work_button";
        public const string NotesEditFormCancelButtonCssLocator = "div:nth-of-type({0}).note_row span.span_link";
        public const string NotesEditFormCancelButtonByNameCssLocator = "div.note_row:has(span:contains({0})) span.span_link";
        public const string VisibleToClientCheckBoxInNoteEditorByRowCssLocator = "div:nth-of-type({0}).note_row .checkbox";
        public const string NotesAddIconCssSelector = "li.add_icon";
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
       // public const string AddNoteExclamationXpath = "//section[contains(@class,'note_component')]//label[text()='Notes']//span[2]";
        public const string SubTypeEnabledXPath = "//label[text()='Sub Type' and contains(@class,'is_enabled')]";
        public const string SubTypeDisabledXPath = "//label[text()='Sub Type' and contains(@class,'is_disabled')]";
        public const string AddIconDisabledCssLocator = "li[title = 'Add Note'].is_disabled";
        public const string NameLabelXPath = "//span[text()='Name']/../..";
        public const string VisibleToClientCheckBoxInNoteEditorByNameCssLocator = "div.note_row:has(span:contains({0})) .checkbox";
        public const string SelectedVisibleToClientCheckBoxInNoteEditorByRowCssLocator =
        "ul.component_item_list>div:nth-of-type({0}).note_row div.component_checkbox.selected";
        public const string SelectedVisibleToClientCheckBoxInNoteEditorByNameCssLocator =
        "ul.component_item_list>div.note_row:has(span:contains({0})) div.component_checkbox.selected";
        //public const string EditNoteExclamationByNameCssLocator=
        //"div.note_row:has(span:contains({0})) label:contains(Notes) span.field_error";
        public const string NoNotesMessageCssLocator = ".note_component .empty_message";
        public const string ClaimLineValueRowAndLabelXPathTemplate =
            "(//div[contains(@class, 'claim_line')]//label[text()='{1}']/../span)[{0}]";
        public const string ProviderDetailsIconBadgeCssLocator =
            "span.entity_details.icon > span.icon_badge"; 
        public const string ProviderDetailsIconCssSelector =
             "span.entity_details.icon";

        public const string NoDataAvailableXPath = "//p[text()='No Data Available']/../../section[@class='component_content ember-view']/p";

        public const string DataPointsOnContainerViewXPath = "//section[contains(@class,'claim_secondary_view')]/section[div/label[text()='Provider Details']]/following-sibling::div/section/ul/li[{0}]/ul/span";
        public const string ProviderDetailsViewDivXPath =
            "//section[contains(@class,'claim_secondary_view')]/section[div/label[text()='Provider Details']]/following-sibling::div/section";

        public const string ContainerViewVerticalRowLabelXPath =
            "//section[contains(@class,'claim_secondary_view')]/section[div/label[text()='Provider Details']]/following-sibling::div/section/ul/li[{0}]/ul/li[1]/label";
        public const string ContainerViewVerticalRowLabelListXPath =
            "//section[contains(@class,'claim_secondary_view')]/section[div/label[text()='Provider Details']]/following-sibling::div/section/ul//ul/li[1]/label";

        public const string ContainerViewVerticalRowValueXPath =
            "//section[contains(@class,'claim_secondary_view')]/section[div/label[text()='Provider Details']]/following-sibling::div/section/ul/li[{0}]/ul/li[1]/span";
        public const string ContainerViewVerticalRowSecondValueXPath =
            "//section[contains(@class,'claim_secondary_view')]/section[div/label[text()='Provider Details']]/following-sibling::div/section/ul/li[{0}]/ul/li[1]/span[2]";

        public const string ContainerViewVerticalRowBadgeXPath = "//ul[contains(@class,'accordion')]/li[{0}]/header/span[{1}]";
        public const string ContainerViewVerticalRowCountXPath = "//section[contains(@class,'claim_secondary_view')]/section[div/label[text()='Provider Details']]/following-sibling::div/section/ul/li";

        public const string ContainerViewVerticalRowCountWithBatchXPath = "//section[@id='provider_entity_details']/ul/li[not(contains(@class,'ciu_referrals'))]";

        public const string DataPointListCountForVerticalRowXPath =
            "//section[@id='provider_entity_details']/ul/li[{0}]/ul[@class='accordion_items is_visible']/li";

        public const string CollapsedContainerViewVerticalRowXPath =
            "//section[contains(@class,'claim_secondary_view')]/section[div/label[text()='Provider Details']]/following-sibling::div/section/ul/li[{0}][contains(@class,'is_expanded')]";
                    public const string DataPointListCountForSecondVerticalRowXPath =
            "//section[@id='provider_entity_details']/ul/li[{0}]/ul[@class='accordion_items is_visible']";

        public const string BadgeIconInContainerViewVerticalRowXPath =
            "//li[{0}]/header/span[@class='detail_item_count badge notification_badge']";

        public const string BadgeIconInContainerViewFirstVerticalRowCountXPath = "//header[@class='has_content accordion_header active']/span[@class='detail_item_count badge notification_badge']";
        public const string NotesLinkXPath = "//span[text()='Notes']";

        public const string ProviderSpecialtyValueCssSelector = "section.claim_action  label[title='Specialty'] + div";
        public const string ProviderNameValueCssSelector = "section.claim_action > div > ul:nth-of-type(2) > div:nth-of-type(3) > div";
        public const string ProviderZipValueCssSelector = "section.claim_action  label[title='Zip'] + div";
        public const string ProviderTinValueCssSelector = "section.claim_action  label[title='TIN'] + div";

        public const string ProviderSearchNotesIconCssSelector = "span.claim_provider_search_notes_icon";


        public const string IsExClamationIconPresentCssSelector =
            "section.claim_action span.icon.small_icon.field_error";

        public const string RightQuadrantLabelClaimDollarCss = "section.claim_dollar_details label.component_title";

        public const string CancelEditFlagLinkCssLocator = "section.flag_line_edits  span.span_link";
        
        public const string OkConfirmationCssSelector = "div#confirmation_links > div#complete_link";
        public const string CancelConfirmationCssSelector = "div#confirmation_links > span.modal_close";

        public const string SelectTriggerClaimLineXpathByRowAndColTemplate =
            "//header[.//text()='Select Trigger Claim/Line']/../ul/li/li/ul[{0}]/li[{1}]";

        public const string DocumentsLinkCssLocator = "span.has_documents , span.add_documents";
        public const string AddDocumentIconCssSelector = "section.component_right span.icon.toolbar_icon.add_documents";
        public const string ViewDocumentIconCssSelector = "section.component_right span.icon.toolbar_icon.has_documents";
        public const string RedBadgeDocumentIconCssSelector = "span.has_documents.icon.toolbar_icon>span.icon_badge";
        public const string UploadDocumentCssSelector = "section.upload_form";
        public const string UploadDocumentFormCssSelector = "section.upload_form div.uploader";
        public const string UploadDocumentSaveButtonCssSelector = "section.upload_form button.work_button";
        public const string UploadDocumentCancelButtonCssSelector = "section.upload_form span.span_link";
        public const string ClaimDocumentNameCssTemplate = "div.listed_document ul>li:nth-of-type({0})>span";

        public const string ClaimDocumentDeleteIconCssLocator =
            "div.listed_document:nth-of-type(1) >ul:nth-of-type(1)>li:not([style*='none'])  span.small_delete_icon";
        public const string ClaimDocumentSectionWithScrollBarCssLocator = "ul.document_list ";

        public const string ErrorMessageOkButtonCssSelector = "div.work_button";

        public const string FirstFlagLinkCssLocator = "li.edit_flag>span";

        public const string FlagAuditHistoryNoteEditIconXPath = "//span[text()='{0}']/../../../ul[4]/li[2]/ul/li/span";

        public const string VisibleToClientCheckBoxCssSelector =
            "section.claim_lines>section.component_content>div>div>div>div>ul>div>div>span.icon.checkbox";

        public const string ClaimFlagAuditHistoryValueByLabelXPath = "//ul[*//span[text()='{0}']]//label[text()='{1}:']/../span";

        public const string IFrameNoteXPathTemplateByLabel =
            "//section[contains(@class,'claim_lines')]//label[text()='{0}']/..//iframe";

        public const string SaveClaimFlagAuditHistoryXPath =
            "//section[contains(@class,'claim_lines')]//button[text()='Save']";

        public const string CancelClaimFlagAuditHistoryXPath =
            "//section[contains(@class,'claim_lines')]//span[text()='Cancel']";

        public const string EditFlagNoteXPath = "//div[contains(@class,'form_component')]";

        public const string GetFlagDetailsByLabelXPath = "//label[text()='{0}:']/../span";

        public const string CustomFieldsLabelCssSelector =
            "section.claim_action.component_content ul:nth-of-type(3) label";

        public const string DefaultSecondaryView = "section.claim_secondary_view label.component_title";
        #region ClaimQA

        public const string ClaimQaPassCssSelector = "span.qa_pass";
        public const string ClaimQaFailCssSelector = "a.qa_fail";
        public const string QaDoneCssSelector = "span.qa_lock_mode";
        public const string ClaimQaEditFlagCssTemplate = "div.claim_line:nth-of-type({0}) span.small_edit_icon";
        public const string ClaimQaRowValueByColCssTemplate = "div.claim_line:nth-of-type({0})>ul>li:nth-of-type({1})>span";
        public const string ClaimQaRowValueListCssTemplate = "div.claim_line>ul>li:nth-of-type({0})>span";
        public const string ClaimQaNoteIconByRowCssTemplate = "div.claim_line:nth-of-type({0}) li.hasnote";
        public const string RecordQaErrorDropdownXPath = "//ul[contains(@class,'claim_qa_details ')]//label[text()='{0}']/..//input";
        public const string ClaimQaSaveButtonCssSelector = "ul>li>div>button";
        public const string ClaimQaCancelLinkCssSelector = "ul>li>div>span>span";
        public const string RecordQaErrorDropdownListValueXPath = "//ul[contains(@class,'claim_qa_details ')]//label[text()='{0}']/..//li[text()='{1}']";
        public const string RecordQaErrorDropdownListXPath = "//ul[contains(@class,'claim_qa_details ')]//label[text()='{0}']/..//li[contains(@class,'option')]";
        public const string CancelButtonOnRecordQaErrorCssTemplate = "div.claim_line:nth-of-type({0}) button";
        public const string RecordQaErrorSectionCssTemplate = "div.claim_line:nth-of-type({0}) >section.form_component";
        public const string OverrideNotesCssTemplate =
            "div.claim_line:nth-of-type({0}) textarea";
        #endregion

        #region Claim Flag Notes
        public const string ClaimFlagNotesIconCssLocator = "a.notes";

        public const string GetFlagValueandLineFlagNotesSection =
            "div.flag_audits:nth-of-type({0})>div>ul.component_item_row:nth-of-type(1)>li:nth-of-type({1})";
        public const string EnabledClaimFlagNotesIconCssLocator = "a.notes.active";

        public const string FlagDetailLabel =
            "section.claim_lines section.component_content>div:has(span:contains({0})) UL.component_nested_row  li.component_data_point label.data_point_label";

        public const string FlagNotesDetailsByFlagandLabel =
            "section.claim_lines section.component_content>div:has(span:contains({0})) li.component_data_point:has(label:contains({1}))>span";

        public const string NOFlagMessageByFlag =
            "section.claim_lines section.component_content>div:has(span:contains({0})) div>label span.data_point_label";

        #endregion

        #region FlagDetails



        public const string SugCodeLinkOnFlagDetailsCssTemplate =
            "div.flag_details span:contains({0})";//jquery

        public const string FlagDetailsTextByLabel = "li:has(label:contains({0}))";

        public const string FlagLineByFlagCssTemplate =
            "div.line_modifiers:not([style*=none]):has( span:contains({0}))>ul";//jquery

        public const string FlagDetailsDefenseRationaleXpath = "//label[text()='Defense Rationale:']/../span";
        public const string FlagDetailsContainersCssLocator = "section>div.flag_details";
        public const string FlagDetailsCVPFlagLineNumberCssLocator = "div.flag_details>ul>li.is_count>span";
        public const string FlagDetailsCVPFlagEditCssLocator = "div.flag_details>ul>li.edit_flag>span";
        public const string LineNoOfSelectedFlaggedLineXpath = @"//ul[contains(@class,'is_active')]/../../ul/li";
        public const string EditOfSelectedFlaggedLineCssLocator = @"ul.is_active li.edit_flag>span";

        public const string FlagDetailsLabelCssSelectorTemplate =
            "section>div.flag_details>ul:nth-of-type(2)>ul>li:nth-of-type({0})>label";

        public const string FlagDetails2ndRowValueCssSelecorTemplate =
            "section>div.flag_details>ul:nth-of-type(2)>ul>li:nth-of-type({0})>span";
        public const string FlagDetails1stRowValueCssSelecorTemplate =
            "section>div.flag_details>ul:nth-of-type(1)>li:nth-of-type({0})>span";
        public const string RuleIdCssLocator = "section>div.flag_details>ul:nth-of-type(1)>li:nth-of-type(3)>label";


        public const string AvailableFlagList = "div.line_modifiers li.edit_flag span";
        #endregion
        #region FlagAudit

        public const string FlagAuditDetailsCssTemplate =
            "section.claim_lines section.component_content>div>div.flag_audits ul:nth-of-type(1)>ul:nth-of-type({0})>li:nth-of-type({1})>span";

        public const string DeletedFlagRowInFlagAudtiDetailCssLocator = "section.claim_lines section.component_content>div>ul.is_deleted";

        public const string FlagAuditListCssLocator = "div.flag_audits >div";
        #endregion

        #region DXCODES

        public const string FirstOriginalDxDateCssLocator = "section.claim_secondary_view > section.component_content > ul > li > ul > li:nth-of-type(1)";

        public const string DxCodeListXpathLocator =
            "//section[contains(@class,'claim_secondary_view')]//li[contains(@class,'component_item')]";

        #endregion

        #region CLAIMLINES

        
        public const string SelectAllLinesToggleIconCssSelector = "section.component_content>div.toggle_select>span";
        public const string ClaimLinesCssSelector = "section.available_claim_lines div.toggle_select+ul >li.is_active";
        public const string SelectedClaimLineCssSelector = "ul.component_item_list>li.is_selectable.is_active li.is_enabled>span";
        public const string ClaimLineIndexCssSelector =
            "div.claim_line>ul.component_item_row>li.is_count>span";
        public const string TopFlagOnClaimLineCssTemplate = "div.claim_line:nth-of-type({0}) li.edit_flag>span";
        public const string TopFlagListOnClaimLineCssTemplate = "div.claim_line li.edit_flag>span";
        public const string FirstEndDosOnClaimLineCssLocator = "div.claim_line > ul > li:nth-of-type(3) > span";
        public const string ProcCodeClaimLineCssLocator = "div.claim_line > ul:nth-of-type({0}) >li:nth-of-type(4) > span";
        public const string ProcDescriptionClaimLineCssLocator = "div.claim_line > ul:nth-of-type({0}) >li:nth-of-type(5) > span";
        public const string ClaimLineCount = "section.claim_lines div.claim_line";
        public const string DentalDataPointLabelForClaimLineCssTemplate = "div.claim_line:nth-of-type(1)>ul>li>label[title='{0}']";

        public const string DentalDataPointValueForClaimLineCssTemplate = "div.claim_line:nth-of-type({0})>ul>li>label[title='{1}']+ span";


        public const string ClaimLineViewLinkCssTemplate = "li[title='{0}']>a";
        public const string ClaimLineDataPointsTitleCssSelector = "div.claim_line>ul:nth-of-type(2)>li>label";
        public const string ClaimLineDentalEditIconCssSelector = "(//li[@title='Edit Record'])[1]/span";
        public const string EditClaimLineRegionCssSelector = "div>section.form_component";
        public const string EditClaimLineComponentsXPath = "(//ul[contains(@class,'component_group')])/div";
        public const string EditClaimLineInputFieldXPathTemplate = "//div[label[contains(normalize-space(),'{0}')]]/input";
        public const string EditDentalFieldSaveButtonCssLocator = "ul.component_item_row>div.component_item>div.form_buttons>button";
        public const string EditDentalFieldCancelLinkCssLocator = "ul.component_item_row>div.component_item>div.form_buttons>span>span";
        public const string ClaimLineNoByLabelAndValueCssLocator = "div.claim_line:has(li:has(label[title=\'{0}\'])>span:contains({1}))>ul.component_item_row>li.is_count>span";
        public const string ClaimLineEditIconXpath = "(//li[@title='Edit Record'])[1]/span";
        public const string ClaimLineEditNotesIconXPath = "//div[contains(@class,'flag_audits')]/div[1]/ul[1]/ul[4]/li[2]/ul[1]/li[1]/span[1]";
        public const string ClaimLineEditNotesRegionCssSelector = "div>div.note_edit_form";

        public const string EditAllFlagOnTheLineIconCssSelector= "li[title='Edit all Flags on the Line']>span";

        public const string EditAllFlagOnTheLineIconByFlagLineNoCssSelector =
            "div.flagged_line:nth-of-type({0}) li[title='Edit all Flags on the Line']>span";
        public const string EditAllFlagOnTheLineSectionCssSelector = "section.line_flag_edits:not([style*='none'])";

        public const string DeleteIconOnEditAllFlagOnTheLineCssSelector =
            "section.line_flag_edits li.is_active span.line_delete";
        public const string RestoreIconOnEditAllFlagOnTheLineCssSelector =
            "section.line_flag_edits li.is_active span.line_restore";


        #endregion

        #region WORKLIST


        public const string ClaimSequenceInWorkListQueueXPath = "//section[@class='section.worlist_section.worklist-queue']/ul/li";

        public const string FiltersLabelCssLocator =
            "div.current_viewing_searchlist>div>div>label";
        public const string InputReviewGroupXPath = "//div[@id='worklist_filters']/div[3]/section/div/input";
        public const string ReviewGroupXPath = "//div[@id='worklist_filters']/div[3]/section/ul/li";
        public const string FiltersSelectedOptionCssLocator = "div.selected_option.pointer";

        public const string ClaimSearchPanelSectionCssLocator = "section.component_sidebar.column_undefined.is_slider";

        public const string ClaimSearchPanelDisabledSectionCssLocator = "section.component_sidebar.column_undefined.is_slider.is_hidden";
        //public const string EmptySearchListMessageSectionCssLocator =
        //  "section.search_list.component_left p.empty_message";

        public const string EmptyMessageCssLocator = "p.empty_search_results_message";
        public const string ClearLinkOnFindClaimSectionCssLocator = "div.current_viewing_searchlist span.span_link";
        public const string AlternateClaimNoInFindPanelLabelCssLocator =
            "section.component_sidebar.column_undefined.is_slider div.basic_input_component:nth-of-type(3)>label";
        public const string SearchlistComponentItemLabelTemplate = "ul.component_item_list li:nth-of-type({0}) ul.component_item_row > li:nth-of-type({1}) label";
        public const string SearchlistComponentItemValueTemplate = "ul.component_item_list li:nth-of-type({0}) ul.component_item_row > li:nth-of-type({1}) span";
        public const string AppealLevelBadgeOnSearchResultTemplate = "ul.component_item_list li:nth-of-type({0}) ul.component_item_row li.secondary_badge span";
        public const string ClaimActionLinkOnSearchResultTemplate = "ul.component_item_list li:nth-of-type({0}) ul.component_item_row li.action_link span";
        public const string ClaimActionsListOnSearchResult = ".component_item_list li.action_link span";


        public const string ClaimNoXPath = "//input[@placeholder='Claim No']";
        public const string WorklistFlagsCssSelector = "div.line_modifiers li.edit_flag>span";
        public const string DentalProfilerIconCssSelector = "span.warning_icon";
        #endregion

        #region MPR / PS EDIT

        public const string TriggerLineForEditMprXPath = "//ul[li[span[text()='MPR']]]/li[8]/span";
        public const string TriggerLineForEditXPathTemplate = "//ul[li[span[text()='{0}']]]/li[8]/span";

        public const string PsEditXPathTemplate = "//li[ul[1]/li[1]/span[text()='{0}']]/div/ul/ul/li[3]/span[text()='PS']";
        public const string LineActionXPathTemplate = "//li[ul[1]/li[1]/span[text()='{0}']]/div/ul/ul[li[3]/span[text()='PS']]/li/ul/li/span";

        public const string FlagLineWithMPrEditXPath = "//ul[ul/li[span[text()='MPR']]]";
        public const string MprEditDelOrRestoreActionXPath = "//ul[li/span[text()='MPR']]/li/ul/li[2]";
        public const string FlagLineWithPsEditXPathTemplate = "//li[ul[1]/li[1]/span[text()='{0}']]/div/ul[ul/li[3]/span[text()='PS']]";

        #endregion

       
        #endregion

        #region SWITCH EDIT XPATHS

        public const string EditLineForSwitchXPath =
            ".//li[@class='flag full_row nested_row']/section/ul[li/span[@class='icon small_icon small_switch_icon is_enabled']]";
        public const string SwitchEditXPath = "//ul[li/ul/li/span[contains(@class,'small_switch_icon is_active')]]/li[3]/span";
        public const string TriggerLineValueXPath = "//ul[li/ul/li/span[contains(@class,'small_switch_icon is_active')]]/li[8]/span";
        public const string CurrentLineNumForSwitchXPath = "//li[div/ul/ul/li/ul/li/span[contains(@class,'small_switch_icon is_active')]]/ul[1]/li[1]/span";

        public const string FlagLineSwitchedFromAnotherLineXPathLocator = "//ul[contains(@class,'component_nested_row') and not(contains(@class,'is_deleted'))]/ul[li[3]/span[text()='{0}']]/li[8]/span[text()='{1}']";
        public const string FlagLineSwitchedFromAnotherLineDifferentClaimXPathLocator = "//ul[contains(@class,'component_nested_row') and not(contains(@class,'is_deleted'))]/ul[li/span[text()='{0}']and li/span[text()='{1}']]/li[8]/span[text()='{2}']";
        public const string SwitchedFromTriggerLineXPath =
            "//ul[contains(@class,'is_active')]/ul[li[3]/span[text()='{0}']]/li[8]/span";
        public const string SwitchedFromDifferentClaimTriggerLineXPath = "//ul[li/span[text()='{0}'] and li/span[text()='{1}']]/li[8]/span";
        public const string NoteTextInAuditTrialCssLocator = "div.flag_audits  ul > ul:nth-of-type(4) > li:nth-of-type(2) > span";
        public const string ModifiedDateTextInAuditTrialCssLocator = "div.flag_audits  ul > ul:nth-of-type(1) > li:nth-of-type(1) > span";
        public const string ActionTypeInAuditTrialCssLocator = "div.flag_audits  ul > ul:nth-of-type(2) > li:nth-of-type(2) > span";
        public const string ActionTextInAuditTrialCssLocator = "div.flag_audits  ul > ul:nth-of-type(2) > li:nth-of-type(1) > span";
        public const string CodeInAuditTrialCssLocator = "div.flag_audits  ul > ul:nth-of-type(3) > li:nth-of-type(1) > span";
        public const string ModifiedByInAuditTrialCssLocator = "div.flag_audits  ul > ul:nth-of-type(1) > li:nth-of-type(2) > span";

        public const string FlagLineSwitchedToAnotherLineXPathLocator =
            "//ul[contains(@class,'is_deleted')]/ul[li[3]/span[text()='{0}']]/li[8]/span[text()='{1}']";

        public const string SwitchedToTriggerLineXPath =
            "//ul[contains(@class,'is_active is_deleted')]/ul[li[3]/span[text()='{0}']]/li[8]/span";

        public const string SwitchedToClaimSequenceXPath =
            "//ul[contains(@class,'is_deleted')]/ul/li/span[text()='{0}']";

        public const string SwitchedFromClaimSequenceXPath =
            "//ul[contains(@class,'component_nested_row')]/ul/li/span[text()='{0}']";


        public const string SpecificLineAndEditClientXPathTemplate =
            "//li[ul[1]/li[1]/span[text()='{0}']]/div/ul[ul/li[2]/span[text()='{1}']]";

        public const string SpecificLineAndEditXPathTemplate =
            "//li[ul[1]/li[1]/span[text()='{0}']]/div/ul[ul/li[{2}]/span[text()='{1}']]"; 

        public const string DeleteAllFlagsOnLineXPathTemplate = "//li[ul[1]/li[1]/span[text()='{0}']]/ul[2]/li/ul/li[2]/span";

        public const string  DeletedLineForSpecificLineAndEditXPathTemplate =
            "//li[ul[1]/li[1]/span[text()='{0}']]/div/ul[contains(@class,'is_deleted')]/ul/li[3]/span[text()='{1}']";

        public const string RestoreDeletedFlagForSpecificLineAndEditXPathTemplate = "//li[ul[1]/li[1]/span[text()='{0}']]/div/ul[contains(@class,'is_deleted')]/ul[li[3]/span[text()='{1}']]/li[1]/ul/li[2]/span[contains(@class,'small_restore')]";

        public const string RestoreDeletedFlagForSpecificLineAndEditClientXPathTemplate = "//li[ul[1]/li[1]/span[text()='{0}']]/div/ul[contains(@class,'is_deleted')]/ul[li[2]/span[text()='{1}']]/li[1]/ul/li[2]/span[contains(@class,'small_restore')]";

        public const string DeletedLineForClientSpecificLineAndEditXPathTemplate =
            "//li[ul[1]/li[1]/span[text()='{0}']]/div/ul[contains(@class,'is_deleted')]/ul/li[2]/span[text()='{1}']";

        public const string DeleteButtonOfGivenLineEditFlagClientTemplate =
            "//li[ul[1]/li[1]/span[text()='{0}']]/div/ul/ul/li/ul/li[2][../../../li/span[text()='{1}']]";

        public const string EditButtonOfGivenLineEditFlagClientTemplate =
            "//li[ul[1]/li[1]/span[text()='{0}']]/div/ul/ul/li/ul/li[1][../../../li/span[text()='{1}']]";

        public const string EditFlagNoteIframeXPath = "//section[contains(@class,'flag_line_edits')]//label[text()='Note']/..//iframe";

        public const string EditCountTriggeredByOtherLineXPathTemplate =
            "//li[ul[1]/li[1]/span[text()='{0}']]/div/ul[not(contains(@class,'is_deleted'))]/ul/li[3]/span[text()='{1}']";
        public const string DisabledSwitchIconXPathTemplate =
            "//li[ul[1]/li[1]/span[text()='{0}']]/div/ul/ul/li/ul/li[3][contains(@class,'is_disabled')]";

        public const string TriggerLineValueByFlagXPathTemplate =
            "//div[contains(@class,'flagged_line ')]/li/div/ul[not(contains(@class,'is_deleted')) and ..//span[text()='{0}']]/ul/li[8]/span";

        public const string SwitchIconByFlagXPathTemplate =
            "//div[contains(@class,'flagged_line ')]/li/div/ul[not(contains(@class,'is_deleted')) and ..//span[text()='{0}']] //span[@class='small_switch_icon is_active icon small_icon']";

        public const string UnitsValueByLineFlagXPathTemplate =
            "//div[contains(@class,'flagged_line ')][{0}]/li/div/ul[not(contains(@class,'is_deleted')) and ..//span[text()='{1}']]/ul/li[10]/span";

        public const string UnitsValueByFlagXPathTemplate =
            "//div[contains(@class,'flagged_line ')]/li/div/ul[not(contains(@class,'is_deleted')) and ..//span[text()='{0}']]/ul/li[10]/span";

        public const string LogicIconByFlagXPathTemplate =
            "//div[contains(@class,'flagged_line ')]/li/div/ul[not(contains(@class,'is_deleted')) and ..//span[text()='{0}']]/ul/li/ul/li[4]/span";

        public const string SwitchIconInEditFlagCssSelector =
            "section.flag_line_edits>ul>li>ul>div>ul>li:nth-of-type(3)>span";

        public const string EnabledEditIconByLineNoAndRowCssSelector =
            "div.flagged_line:nth-of-type({0}) div.line_modifiers:nth-of-type({1}) span.small_edit_icon.is_active";

        public const string DisabledEditIconByLineNoAndRowCssSelector =
            "div.flagged_line:nth-of-type({0}) div.line_modifiers:nth-of-type({1}) span.small_edit_icon.is_disabled";

        public const string IconSelectorByLineNumFlagAndIconNameXPathTemplate = "//div[contains(@class,'flagged_line ')][{0}] //span[contains(@title,'{1}')]/../..//span[contains(@class,'{2}')]";

        public const string EnabledDeleteIconByLineNoAndRowCssSelector =
            "div.flagged_line:nth-of-type({0}) div.line_modifiers:nth-of-type({1}) span.small_delete_icon.is_active";

        public const string DisabledDeleteIconByLineNoAndRowCssSelector =
            "div.flagged_line:nth-of-type({0}) div.line_modifiers:nth-of-type({1}) span.small_delete_icon.is_disabled";

        public const string DisabledRestoreIconByLineNoAndRowCssSelector =
            "div.flagged_line:nth-of-type({0}) div.line_modifiers:nth-of-type({1}) span.small_restore_icon.is_disabled";

        public const string ImageIdSectionCssSelector = ".component_table_row:has(label:contains(Image ID))";

        public const string DisabledDeleteIconbyLineNumber = "div.flagged_line:nth-of-type({0})>li>ul:nth-of-type(2) span.small_delete_icon.is_disabled";

        public const string DisabledEditIconByLineNumber = "div.flagged_line:nth-of-type({0})>li>ul:nth-of-type(2) span.small_edit_icon.is_disabled";

        public const string EnabledDeleteIconByLineNumber = "div.flagged_line:nth-of-type({0})>li>ul:nth-of-type(2) span.small_delete_icon.is_active";

        public const string EnabledEditIconByLineNumber = "div.flagged_line:nth-of-type({0})>li>ul:nth-of-type(2) span.small_edit_icon.is_active";

        public const string ClaimLineIconInClaimLinesCssSelector = "a[title=\"Claim Lines\"]";
        #endregion

        #region DCIWORKLIST

        public const string DciWorklistFlagsCssSelector = "div.line_modifiers li.edit_flag>span";
        public const string DciWorkListFiltersCssSelector = "div.current_viewing_searchlist label";
        public const string DciClaimStatusCssSelector = "div.current_viewing_searchlist label + section>ul.is_visible>li";
        #endregion

        #region CIU Referral

        public const string AddCiuButtonCssLocator = "li.ciu_referrals li.has_badge";

        public const string CiuReferralInputFieldXpathTempalte = "//label[text()[contains(.,'{0}')]]/..//input";
        public const string SearchInputListValueCssTemplate =
            "//section[contains(@class,'ciu_referral_form ')]//ul//li[text()='{0}']";

        public const string SearchInputFieldXpathTemplate = "//label[text()='{0}']/../section/input";

        public const string SearchInputListValueXPathTemplateGeneric =
            "//label[text()='{0}']/../section//ul//li[text()='{1}']";

        public const string SaveCiuButtonXpath = "//button[text()='Save']";
       

        public const string CiuReferralRecordRowCssSelector = "li.ciu_referrals >ul:nth-of-type(2)>li";

        public const string DeleteReferralButtonCssTemplate =
           "li.ciu_referrals ul.component_item_list li:nth-of-type({0}) li.has_badge";

        public const string CiuReferralCreatedDeteCssTemplate =
            "li.ciu_referrals >ul:nth-of-type(2)>li>ul:nth-of-type(1)>li:nth-of-type(2)>span";

        public const string CiuReferralDetailByRowLabelCssTemplate =
            //"//li[contains(@class,'Ciu_referrals ')]/ul[2]/li[{0}]//label[text()='{1}']/../span";
            "li.ciu_referrals>ul:nth-of-type(2)>li:nth-of-type({0}) li:contains({1})>span";//can find element when run using jquery

        public const string ProviderDetailSectionCssSelector =
            "div#top_section>div:nth-of-type(2)>div:nth-of-type(2)>div";

        public const string DeleteCiuReferralIconCssTemplate =
            "li.ciu_referrals >ul:nth-of-type(2)>li:nth-of-type({0}) li.small_delete_icon";



        public const string CreateCiuReferralFormXPath = "//header[.//text()[contains(.,'Create CIU')]]/../../..";

        public const string CreateCiuReferralInputCssTemplate = "li:contains({0})>input";


        public const string CancelCiuReferralLinkCssSelector = @"//label[text()=""Create CIU Referral""]/ancestor::section[contains(@class,'form_component')]//span[text()=""Cancel""]";

        public const string PatternCategoryInputCssSelector = "div:has(>label:contains(Pattern Category))>section>input";

        public const string PatternCategoryValueCssSelector =
            "div:has(>label:contains(Pattern Category)) li:contains({0})";

        public const string NoCiuReferralCssSelector = "span[title=\"There are no CIU referral details available.\"]";

        public const string InputCiuReferralCssTemplate = "div:has(>label:contains({0})) input , li:has(>label:contains({0})) input";
        //public const string FieldErrorXpathTemplateForCiu =
        //    "//header//label[text()='Create CIU Referral']/ancestor::section[contains(@class,'form_component')]//span[contains(@class,'field_error')]";
        //public const string CiuReferralFieldErrorForLabelXpathTemplate = "//label[text()[contains(.,'{0}')]]/span[contains(@class,'field_error')]";
        //public const string CiuReferralFieldErrorForLabelCssTemplate = "li:has( label:contains({0})) span.field_error";

        #endregion

        #region NewLogicManager
        public const string AddLogicIconCssSelectorByRow = "div.flagged_line:nth-of-type({0}) div.line_modifiers:nth-of-type(1) span.add_logic.is_active.small_icon";
        public const string ClientLogicIconCssSelectorByRow = "div.flagged_line:nth-of-type(1) div.line_modifiers:nth-of-type({0}) span.client_logic.is_active.small_icon";
        public const string LogicFormXpathTemplate = "//label[text()='{0}']";
        public const string AssignedToValueCssSelector = "div.logic_message_row+section label[title='Assigned to'] + span";
        public const string LogicFormCssLocator = "ul.flag_logics";
        public const string LogicFormByRowCssTemplate = "div.flagged_line:nth-of-type({0}) ul.flag_logics";
        public const string LogicMessageCssSelector = "div.logic_message";
        public const string LogicMessageFormLabelCssSelector = "ul.flag_logics>section span.form_header_left>label";
        public const string LogicMessageTextareaCssLocator = "ul.flag_logics textarea";
        public const string LeftMessaageSectionCssLocator = "div.flag_logics_messages div.logic_message.left";
        public const string RightMessaageSectionCssLocator = "div.flag_logics_messages div.logic_message.right";
        public const string ClientLogicIconByRowCssTemplate = "div.flagged_line:nth-of-type({0}) .client_logic";
        public const string InternalLogicIconByRowCssTemplate = "div.flagged_line:nth-of-type({0}) .internal_logic";
        public const string LogicIconWithLogicByRowCssTemplate = "div.flagged_line:nth-of-type({0}) .internal_logic,div.flagged_line:nth-of-type({0}) .client_logic";
        public const string ButtonXPathTemplate = "//button[text()='{0}']";
        #endregion

        #region DxCodesLineDetails
        public const string DxCodesInClaimLineDetailsCssSelector = "div.line_modifiers ul.component_item_list>li.component_item";
        public const string DxCodesLabelInClaimLineDetailsByRowCssTemplate = "div.line_modifiers ul.component_item_list>li.component_item:nth-of-type({0}) li>label";
        public const string DxCodesDescriptionValuesInClaimLineDetailsByRowColumnCssTemplate = "div.line_modifiers ul.component_item_list>li.component_item:nth-of-type({0}) ul li:nth-of-type({1})>span";
        public const string DxCodesInClaimLineDetailsByLabelCssTemplate = "div.line_modifiers ul.component_item_list>li.component_item label[title = '{0}']";
        #endregion

        public const string InputFieldByLabelCssTemplate = "div.radio_button_group:has(span:contains({0})),li.form_item:has(label:contains({0})) input,div.component_item:has(label:contains({0})) input";
        public const string DropDownInputListByLabelXPathTemplate = "//*/section[contains(@class, 'form_component')]//label[text()='{0}']/../section//ul//li";



        #region PROTECTED PROPERTIES

        public override string PageTitle
        {
            get { return PageTitleEnum.ClaimAction.GetStringValue(); }
        }

        #endregion

        #region CONSTRUCTOR

        public ClaimActionPageObjects()
            : base(PageUrlEnum.ClaimAction.GetStringValue())
        {
        }

        #endregion
    }
}

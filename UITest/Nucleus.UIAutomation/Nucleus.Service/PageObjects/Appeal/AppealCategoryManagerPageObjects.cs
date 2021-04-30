using System.Drawing;
using Microsoft.SqlServer.Server;
using Nucleus.Service.PageObjects.Default;
using Nucleus.Service.Support.Enum;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Base;
using UIAutomation.Framework.Elements;
using UIAutomation.Framework.Utils;

namespace Nucleus.Service.PageObjects.Appeal
{
    public class AppealCategoryManagerPageObjects : NewDefaultPageObjects
    {
        #region PRIVATE/PUBLIC FIELDS

        public const string DentalAppealCategoryRowJQueryTemplate =
            "ul.component_item_row:has(span[title=DCA]):has(span[title={0}])";

        public const string AppealCategoryAnalystDetailsIconCssSelector = "a.appeal_category_analyst_details_icon";
        public const string AppealCategoryAuditHistoryIconCssSelector = "a.appeal_category_audit_history_icon";
        public const string AppealCategoryAssignedAnalystXPath = "//label[contains(text(),'{0}')]/../span";

        public const string AppealCategoryAssinedNResAnalystCssTemplate =
            "ul.component_item_list  div:nth-of-type(2) li.analyst_full_name>span";
        public const string AppealCategoryAssinedResAnalystCssTemplate =
            "ul.component_item_list  div:nth-of-type(4) li.analyst_full_name>span";

        public const string AppealCategoryAssinedNResAnalystXpathTemplate =
            "//label[text()='Analysts (restricted)']/../../preceding-sibling::div//span[@class='data_point_value']";
        public const string AppealCategoryAssinedResAnalystXpathTemplate =
            "//label[text()='Analysts (restricted)']/../../following-sibling::div//span[@class='data_point_value']";

        public const string AppealCategoryAssignedNResAnalystByNameXpathTemplate =
            "//label[text()='Analysts (restricted)']/../../preceding-sibling::div//span[@title='{0}']";
        public const string AppealCategoryAssinedResAnalystByNameXpathTemplate =
            "//label[text()='Analysts (restricted)']/../../following-sibling::div//span[@title='{0}']";


        public const string PageInsideTitleCssLocator = "label.page_title";
        public const string AppealCategoryRowCssLocator = "section.search_list ul.component_item_list > li > ul";
        public const string AppealCategoryOrderOfLastRowWithOutFlagXPath = "//ul[contains(@class,'component_item_list')]/li[ul/li[5]/span[text()='']][last()]/ul/li[6]/span";

        public const string AppealCategoryLastOrderNumberCssSelector =
            "section.search_list ul.component_item_list>li:nth-last-child(2) label[title='Order']+span";
        public const string AppealCategoryCatIdCssLocator = "section.search_list ul.component_item_list > li>ul>li:nth-of-type(2) label[title='{0}']";
        public const string AppealCategoryRowCssTemplate = "section.search_list ul.component_item_list > li:nth-of-type({0}) > ul";
        public const string AppealCategoryRowExcludingDciJqueryTemplate = "section.search_list ul.component_item_list > li>ul>li:nth-of-type(4) span:not([title=\"DCA\"])";
        public const string EditIconEnabledCssLocator = "span.small_edit_icon.is_active";
        public const string DeleteIconEnabledCssLocator = "span.small_delete_icon.is_active";
        public const string EditIconDisabledCssLocator = "span.small_edit_icon.is_disabled";
        public const string DeleteIconDisabledCssLocator = "span.small_delete_icon.is_disabled";
        public const string CategoryLabelCssTemplate = "section.search_list ul.component_item_list > li:nth-of-type({0}) > ul > li:nth-of-type(2) > label";
        public const string CategoryLabelExcludingDentalJqueryTemplate = "section.search_list ul.component_item_list > li:has(>ul>li:nth-of-type(4) span:not([title=\"DCA\"])) ul>li:nth-of-type(2) > label";

        public const string ClientLabelCssTemplate = "section.search_list ul.component_item_list > li:nth-of-type({0}) > ul > li:nth-of-type(3) > label";
        public const string ClientValueCssTemplate = "section.search_list ul.component_item_list > li:nth-of-type({0}) > ul > li:nth-of-type(3) > span";
        public const string ClientValueExcludingDentalJqueryTemplate = "section.search_list ul.component_item_list > li:has(>ul>li:nth-of-type(4) span:not([title=\"DCA\"])) ul>li:nth-of-type(3) > span";

        public const string FlagLabelCssLocator = "section.search_list ul.component_item_list > li:nth-of-type({0}) > ul > li:nth-of-type(5) > label";
        public const string FlagValueCssTemplate = "section.search_list ul.component_item_list > li:nth-of-type({0}) > ul > li:nth-of-type(5) > span";

        public const string OrderLabelCssLocator = "section.search_list ul.component_item_list > li:nth-of-type({0}) > ul > li:nth-of-type(6) > label";
        public const string OrderValueCssTemplate = "section.search_list ul.component_item_list > li:nth-of-type({0}) > ul > li:nth-of-type(6) > span";
        public const string OrderValueExcludingDentalJqueryTemplate = "section.search_list ul.component_item_list > li:has(>ul>li:nth-of-type(4) span:not([title=\"DCA\"])) ul>li:nth-of-type(5) > span";
        public const string ProductLabelCssLocator = "section.search_list ul.component_item_list > li:nth-of-type({0}) > ul > li:nth-of-type(4) > label";
        public const string ProductValueCssTemplate = "section.search_list ul.component_item_list > li:nth-of-type({0}) > ul > li:nth-of-type(4) > span";
        public const string ProductValueExcludingDentalJqueryTemplate = "section.search_list ul.component_item_list > li:has(>ul>li:nth-of-type(4) span:not([title=\"DCA\"])) ul>li:nth-of-type(4) > span";

        public const string ProductValueExcludingDciCssTemplate =
            "section.search_list ul.component_item_list > li > ul > li:nth-of-type(4) > span:not([title=\"DCA\"])";
        public const string ProcLabelCssLocator = "section.search_list ul.component_item_list > li:nth-of-type({0}) > ul > li:nth-of-type(7) > label";
        public const string ProcValueCssTemplate = "section.search_list ul.component_item_list > li:nth-of-type({0}) > ul > li:nth-of-type(7) > span";
        public const string ProcValueExcludingDciJqueryTemplate = "section.search_list ul.component_item_list > li:has(>ul>li:nth-of-type(4) span:not([title=\"DCA\"])) ul>li:nth-of-type(7) > span";
        public const string TrigLabelCssLocator = "section.search_list ul.component_item_list > li:nth-of-type({0}) > ul > li:nth-of-type(8) > label";
        public const string TrigValueCssTemplate = "section.search_list ul.component_item_list > li:nth-of-type({0}) > ul > li:nth-of-type(8) > span";
        public const string TrigValueExcludingDciJqueryTemplate = "section.search_list ul.component_item_list > li:has(>ul>li:nth-of-type(4) span:not([title=\"DCA\"])) ul>li:nth-of-type(8) > span";
       
        public const string CategoryValueCssTemplate = "section.search_list ul.component_item_list > li:nth-of-type({0}) > ul > li:nth-of-type(2) > span";
        public const string EditIconCssTemplate = "li:nth-of-type({0}) >ul>li>ul> li>span.small_edit_icon.is_active";
        public const string DeleteIconCssTemplate = "li:nth-of-type({0}) >ul>li>ul> li>span.small_delete_icon.is_active";
        public const string AppealCategoryRowColValueCssTemplate = "section.search_list ul.component_item_list > li:nth-of-type({0}) > ul>li:nth-of-type({1}) span";
        public const string AppealCategoryRowColValueCssTemplatewLabel = "section.search_list ul.component_item_list > li:nth-of-type({0}) > ul>li:nth-of-type({1})";
        public const string ProductValueCssLocator = "section.search_list ul.component_item_list > li > ul > li:nth-of-type(4) > span";
        public const string TrigValueCssLocator = "section.search_list ul.component_item_list > li > ul > li:nth-of-type(8) > span";
        public const string ProcValueCssLocator = "section.search_list ul.component_item_list > li > ul > li:nth-of-type(7) > span";
        
        public const string OrderValueCssLocator = "section.search_list ul.component_item_list > li > ul > li:nth-of-type(6) > span";
        public const string CategoryLabelCssLocator = "section.search_list ul.component_item_list > li > ul > li:nth-of-type(2) > label";
        public const string CategoryLabelCssLocatorByRow = "section.search_list ul.component_item_list > li:nth-of-type({0}) > ul > li:nth-of-type(2) > label";

        public const string WorkingAjaxMessageCssLocator = "div.small_loading";
        public const string AuditRowCssLocator = "div.flag_line";
        public const string AuditTrailContainerCssLocator = "section.component_right.line_details>section:nth-of-type(2)>div";
        public const string AuditRowEachValueCssSelector = "div.flag_line li";
        public const string LastModifiedDateCssTemplate =
            "div.flag_line:nth-of-type({0})>ul> ul:nth-of-type(1)>li:nth-of-type(1)>label";
        public const string CategoryIDAuditSectionCssLocator =
            "section.component_right.line_details>section:nth-of-type(2)>div>ul>li>label";
        public const string CategoryOrderAuditSectionCssLocator =
            "div.flag_line:nth-of-type(1)>ul> ul:nth-of-type(1)>li:nth-of-type(3)>span";
        public const string ProductValueAuditSectionCssLocator =
           "div.flag_line:nth-of-type(1)>ul> ul:nth-of-type(1)>li:nth-of-type(2)>span";
        public const string NotesValueAuditSectionCssLocator =
            "div.flag_line:nth-of-type(1)>ul> li:nth-of-type(1)>span:nth-of-type(2)";
        public const string AnalystOnAuditSectionCssLocator =
            "div.flag_line:nth-of-type(1)>ul> ul:nth-of-type(2)>li:nth-of-type(1)>span";
        public const string LatestAuditDataInAppealCategoryAuditHistory =
            "div.flag_line:nth-of-type(1)>ul> ul:nth-of-type(1)>li:nth-of-type(1)>label,div.flag_line:nth-of-type(1)>ul> ul>li>span,div.flag_line:nth-of-type(1)>ul>li:nth-of-type(1)>span:nth-of-type(2)";
        public const string LastModifiedUserAuditSectionCssLocator =
           "div.flag_line:nth-of-type(1)>ul> ul:nth-of-type(1)>li:nth-of-type(6)>span";
        public const string ProcCodeAuditSectionCssLocator =
           "div.flag_line:nth-of-type(1)>ul> ul:nth-of-type(1)>li:nth-of-type(4)>span";
        public const string TrigProcAuditSectionCssLocator =
           "div.flag_line:nth-of-type(1)>ul> ul:nth-of-type(1)>li:nth-of-type(5)>span";
        public const string UserIDAuditSectionCssLocator =
           "div.flag_line:nth-of-type(1)>ul> ul:nth-of-type(2)>li:nth-of-type(1)>span";

        public const string AddCategoryFormCssSelector = "section.form_component";
        public const string AddCategoryCodeCssLocator = ".add_icon";
        public const string AddCategoryCodeEnabledCssLocator = "li.is_active a.add_icon";
        public const string AddCategoryCodeDisabledCssLocator = "li.is_disabled a.add_icon";
        public const string SearchCategoryCodeCssLocator = ".sidebar_icon";

        public const string CategoryIdAddCategorySectionCssLocator =
            "section.form_component >form label:contains(Category ID)+input";
        //"section.form_component >form>ul:nth-of-type(2)>div:nth-of-type(1) input";

        public const string ClientAddCategorySectionCssLocator =
           "section.form_component >form>ul:nth-of-type(2)>div:nth-of-type(2) input";

        public const string FlagFromAddCategorySectionCssSelector =
            "section.form_component >form>ul:nth-of-type(3)>div:has(label:contains(Flag)) input";

        public const string FlagDropDownListXPathTemplate = "//label[text()='Flag']/../section//ul//li[text()='{0}']";
        public const string AvalilableFlagDropDownListXPath = "//label[text()='Flag']/../section//ul//li";
        public const string ProcCodeFromAddCategorySectionCssLocator =
            "section.form_component >form label:contains(Proc Code From)+input";
        public const string ProcCodeToAddCategorySectionCssLocator =
            "section.form_component >form label:contains(Proc Code To)+input";

        public const string ProductAddCategorySectionCssLocator =
            "section.form_component >form div:has(label:contains(Product))>section input";
        //"section.form_component >form>ul:nth-of-type(3)>div:nth-of-type(1) input";

        public const string ProductOnAddSectionXpathTemplate =
            "//section[contains(@class,'form_component')]/form/ul[2]/div[1]/div/section/ul/li[text()='{0}']";
            //"//section[contains(@class,'form_component')]/form/ul[3]/div[1]/div/section/ul/li[text()='{0}']";
        public const string TrigProcFromAddCategorySectionCssLocator =
            "section.form_component >form label:contains(Trig Proc From)+input";
        public const string TrigProcTodAddCategorySectionCssLocator =
            "section.form_component >form label:contains(Trig Proc To)+input";
        public const string CategoryOrderAddCategorySectionCssLocator =
            "section.form_component >form>ul:nth-of-type(4)>div:nth-of-type(1) input";
        public const string CategoryOrderListValueOnAddSectionFieldCssLocator =
            "section.form_component >form>ul:nth-of-type(4)>div:nth-of-type(1) ul li";
        public const string CategoryOrderOnAddSectionXpathTemplate = "//section[contains(@class,'form_component')]/form/ul[4]/div/div/section/ul/li[text()='{0}']";
        public const string AllClearFlagOptionsInAddCategorySectionCssTemplate = "ul.is_visible li:contains({0})";

        public const string AnalaystListValueOnAddSectionFieldCssLocator =
            "section.form_component >form>ul:nth-of-type(5)>div:nth-of-type(1) ul li";

        public const string AnalaystRestrictedListValueOnAddSectionFieldCssLocator =
            "section.form_component >form>ul:nth-of-type(6)>div:nth-of-type(1) ul li";

        public const string AnalaystListValueOnAddSectionFieldForDCICssLocator =
            "section.form_component >form>ul:nth-of-type(3)>div:nth-of-type(1) ul li";

        public const string ClientListValueOnAddSectionFieldCssLocator =
            "section.form_component >form>ul:nth-of-type(2)>div:nth-of-type(2) ul li";
        public const string ClientOnAddSectionXpathTemplate = "//section[contains(@class,'form_component')]/form/ul[2]/div[2]/div/section/ul/li[text()='{0}']";

        public const string AnalystAddCategorySectionForDCICssLocator = "section.form_component >form>ul:nth-of-type(3)>div input";

        public const string AnalystAddCategorySectionCssLocator =
            "section.form_component >form>ul:nth-of-type(5)>div input";

        public const string AnalystRestrictedClaimsAddCategorySectionCssLocator =
            "section.form_component >form>ul:nth-of-type(6)>div input";

        public const string AnalystOnAddForDCISectionXpathTemplate =
            "//section[contains(@class,'form_component')]/form/ul[3]/div/section/ul/li[contains(text(),'{0}')]";

        public const string AnalystOnAddSectionXpathTemplate =
            "//section[contains(@class,'form_component')]/form/ul[5]/div/section/ul/li[contains(text(),'{0}')]";


        public const string AnalystRestrictedClaimsOnAddSectionXpathTemplate =
            "//section[contains(@class,'form_component')]/form/ul[6]/div/section/ul/li[contains(text(),'{0}')]";

        public const string NoteAddCategorySectionCssLocator = ".cke_editable_themed.cke_contents_ltr";
        public const string savebuttonCssLocator = "section.form_component >form>ul:nth-of-type(7) button";
        public const string FindCategoryCodesSectionCssLocator = "section.component_sidebar.is_slider:not(.is_hidden)";
        public const string SearchFieldFindSectionCssTemplate =
            "section.component_sidebar_panel span>div>div:nth-of-type({0}) input";
        public const string SearchFieldListValueFindSectionXPathTemplate =
            "//section[contains(@class,'component_sidebar_panel')]/span/div/div[{0}]/section/ul/li[text()='{1}']";
        public const string SearchFieldListFindSectionCssTemplate =
            "section.component_sidebar_panel span>div>div:nth-of-type({0}) ul>li";

        public const string SearchFieldListFindSectionDropDownValueCssTemplate =
            "section.component_sidebar_panel span>div>div:nth-of-type({0}) ul>li:nth-of-type({1})";
        public const string FindButtonCssLocator = ".work_button";
        public const string SaveButtonInEditCssLocator = "form:has(label:contains(Edit)) .work_button";
        public const string SaveButtonInAddFormsCssLocator = "form:has(label:contains(Add)) .work_button";
        public const string SaveButtonCssSelector = "section.form_component div.form_buttons button.work_button";
        public const string ClearButtonCssLocator = "section.component_sidebar_panel span.span_link";
        public const string EmptySearchMessageCssLocator = ".empty_search_results_message";
        public const string PageErrorPopupModelId = "nucleus_modal_wrap";
        private const string PageErrorCloseId = "nucleus_modal_close";
        public const string PageErrorMessageId = "nucleus_modal_content";
        public const string FilterIconCssLocator = "span.filters.icon.toolbar_icon";
        public const string FilterOptionListCssLocator = "ul.option_list>li>span";
        public const string FilterOptionCssTemplate = "ul.option_list>li:nth-of-type({0})>span";
        public const string FilterOptionSectionCssLocator = "ul.option_list.is_visible";
        public const string EditFormXPathByLabel = "//label[text()='{0}']/../../..";
        public const string EditFormCssLocator = "section.component_nested_form";

        public const string ClientOnEditSectionFieldCssLocator =
            "section.component_nested_form>form>ul:nth-of-type(2) div:nth-of-type(1) input";

        public const string CategoryIdOnEditSectionFieldCssLocator =
            "section.component_nested_form>form>ul:nth-of-type(2)> div:nth-of-type(1) input";
        public const string ProcCodeFromOnEditSectionFieldCssLocator =
           "section.component_nested_form>form>ul:nth-of-type(2)> div:nth-of-type(1) input";
        public const string ProcCodeToOnEditSectionFieldCssLocator =
           "section.component_nested_form>form>ul:nth-of-type(2)> div:nth-of-type(2) input";
        public const string CategoryOrderOnEditSectionFieldCssLocator =
           "section.component_nested_form>form>ul:nth-of-type(2) ul div.basic_input_component input";
        public const string FlagOnEditSectionFieldCssLocator =
            "section.component_nested_form>form>ul div:has(label:contains(Flag))> section>input";
        public const string TrigProcFromOnEditSectionFieldCssLocator =
           "section.component_nested_form>form>ul:nth-of-type(2)> div:nth-of-type(3) input";
        public const string TrigProcToOnEditSectionFieldCssLocator =
           "section.component_nested_form>form>ul:nth-of-type(2)> div:nth-of-type(4) input";
        public const string AnalystOnEditSectionFieldCssLocator =
           "section.component_nested_form>form>ul:nth-of-type(4)> div:nth-of-type(1) input";

        public const string AnalystOnEditSectionForRestrictedAnalystsFieldCssLocator =
            "section.component_nested_form>form>ul:nth-of-type(5)> div:nth-of-type(1) input";

        public const string CategoryOrderListValueOnEditSectionFieldCssLocator =
            "section.component_nested_form>form>ul:nth-of-type(2)> div:nth-of-type(2) input";

        public const string AnalystListValueOnEditSectionFieldCssLocator =
            "section.component_nested_form>form>ul:nth-of-type(4)> div:nth-of-type(1) ul li";

        public const string AnalystListValueOnEditSectionForRestrictedAnalystsFieldCssLocator =
            "section.component_nested_form>form>ul:nth-of-type(5)> div:nth-of-type(1) ul li";

        //public const string ProductListValueOnEditSectionFieldCssLocator =
          //  "section.component_nested_form>form>ul:nth-of-type(2)> div:nth-of-type(2) ul li";
          public const string LastLiOfPrimaryViewCssSelector = "section.component_content>ul>li:last-of-type";
        public const string SelectedAnalystValueCssLoctor =
            "ul.component_item_row:has(label:contains({0})) li.selected_analyst_name >span.data_point_value";
        //"li.selected_analyst_name >span.data_point_value";

        public const string SelectedAnalystValueFromRestrictedAnalystSectionCssLoctor =
            "ul.component_item_row:has(label:contains(Analyst (non-restricted claims))) li.selected_analyst_name >span.data_point_value";


        public const string CategoryOrderOnEditSectionXpathTemplate =
            "//section[contains(@class,'component_nested_form')]/form/ul[2]/div[2]/div/section/ul/li[text()='{0}']";
       // public const string ProductOnEditSectionXpathTemplate =
         //  "//section[contains(@class,'component_nested_form')]/form/ul[2]/div[2]/div/section/ul/li[text()='{0}']";
        public const string AnalystOnEditSectionXpathTemplate =
           "//section[contains(@class,'component_nested_form')]/form/ul[4]/div/section/ul/li[contains(text(),'{0}')]";
        public const string AnalystWithRestrictedAccessOnEditSectionXpathTemplate =
            "//section[contains(@class,'component_nested_form')]/form/ul[5]/div/section/ul/li[contains(text(),'{0}')]";
        public const string ClientOnEditSectionXpathTemplate =
           "//section[contains(@class,'component_nested_form')]/form/ul[2]/div[1]/div/section/ul/li[text()='{0}']";

        public const string DeleteAnalystCssTemplate = "ul.component_item_row:has(label:contains(\"{0}\")) ul.multi_select_row:nth-of-type({1}) span.small_delete_icon";

        public const string MoveUpAnalystByRowCssTemplate = "ul.multi_select_row:nth-of-type({0}) span.half_caret_up_icon";
        public const string MoveDownAnalystByRowCssTemplate = "ul.multi_select_row:nth-of-type({0}) span.half_caret_down_icon";
       // public const string MoveUpFollowingAnalystCssTemplate = "//li[contains(@class,'selected_analyst_name')]/span[text()='{0}']/../..//li//span[contains(@class,'half_caret_up_icon')]";
       // public const string MoveDownFollowingAnalystCssTemplate = "//li[contains(@class,'selected_analyst_name')]/span[text()='{0}']/../..//li//span[contains(@class,'half_caret_down_icon')]";
        public const string MoveUpFollowingAnalystCssTemplate = "form:has( label:contains({0})) ul.multi_select_row:has(li>span:contains({1})) li>span.half_caret_up_icon";
        public const string MoveDownFollowingAnalystCssTemplate = "form:has( label:contains({0})) ul.multi_select_row:has(li>span:contains({1})) li>span.half_caret_down_icon";
        public const string DeleteAnalystListCssSelector = "ul.component_item_row:has(label:contains({0})) ul.multi_select_row span.small_delete_icon";
        public const string DeleteRestrictedAccessAnalystListCssSelector = "ul.component_item_row:has(label:contains({0})) ul.multi_select_row span.small_delete_icon";

        public const string CancelButtonCssLocator = "form span.span_link";
        public const string DisabledCancelButtonCssLocator = "form span.is_disabled";

        public const string AddNewCategoryCodeSectionXPath =
            "//section[form/ul/div/header[.//text()='Add Appeal Category']]";

        public const string AppealRowCategoryByClientProductProcCodeTrigCodeXPathTemplate =
            "//ul[contains(@class,'component_item_list')]/li [ul/li[3]/span[text()='{0}']] [ul/li[4]/span[text()='{1}']] [ul/li[7]/span[text()='{2}']] [ul/li[8]/span[text()='{3}']]";

        public const string GetCategoryIdByProductProcCodeTrigCodePresent =
            "//ul[contains(@class,'component_item_list')]/li  [ul/li[3]/span[text()='{0}']] [ul/li[4]/span[text()='{1}']] [ul/li[7]/span[text()='{2}']] [ul/li[8]/span[text()='{3}']]//li[2]//label";
        public const string DeleteIconByClientProductProcCodeTrigCodeXPathTemplate =
            "//ul[contains(@class,'component_item_list')]/li  [ul/li[3]/span[text()='{0}']] [ul/li[4]/span[text()='{1}']] [ul/li[7]/span[text()='{2}']] [ul/li[8]/span[text()='{3}']]/ul/li/ul/li/span[contains(@class,'small_delete_icon is_active icon')]";

	    public const string EditIconByClientProductProcCodeTrigCodeXPathTemplate =
			"//ul[contains(@class,'component_item_list')]/li  [ul/li[3]/span[text()='{0}']] [ul/li[4]/span[text()='{1}']] [ul/li[7]/span[text()='{2}']] [ul/li[8]/span[text()='{3}']]/ul/li/ul/li/span[contains(@class,'small_edit_icon')]";
        public const string OrderVaueByClientProductProcCodeTrigCodeXPathTemplate =
            "//ul[contains(@class,'component_item_list')]/li  [ul/li[3]/span[text()='{0}']] [ul/li[4]/span[text()='{1}']] [ul/li[7]/span[text()='{2}']] [ul/li[8]/span[text()='{3}']]/ul/li[6]/span";

		public const string OkConfirmationCssSelector = "div#confirmation_links > div#complete_link";
        public const string CancelConfirmationCssSelector = "div#confirmation_links > span.span_link.modal_close";
        public const string EditAppealCategoryHeaderCssSelector = "span.form_header_left>label";

        public const string AssignedAnalystByType =
            "section.component_right > section.component_content  div.component_sub_header:Contains({0})~div";
        public const string AnalystAssignmentHeaderCssLocator = "section.component_right > section.component_header>div>label";
        public const string AnalystAssignmentSubHeaderCsslocator = "section.component_right > section.component_content  div.component_sub_header";
        public const string AnalystAssignmentListItems =
            "section.component_right > section.component_content > ul.component_item_list > div.component_item  li.analyst_full_name > span";

        public const string EditIconOfOrderZeroXpath = "//li[label[text()='Order:'] and span[text()='1']]/..//span[contains(@class,'small_edit_icon')]";
        public const string EditIconOfOrderNonNaOrZeroXpath = "//li[label[text()='Order:'] and span[not(text()='0') and not(text()='NA')]]/..//span[contains(@class,'small_edit_icon')]";

        public const string EditFormLabelCSSLocator  = "section.form_component label:contains(\"{0}\")";
        public const string AnalystRestrictedOnAuditSectionCssLocator = "div.flag_line:nth-of-type(1)>ul> ul:nth-of-type(3)>li:nth-of-type(1)>span";
        public const string RestrictedAnalystAssignmentListItems =
            "section.component_right > section.component_content > ul.component_item_list > div.component_item:nth-of-type({0})  li.analyst_full_name > span";

        public const string InputFieldByLabelNameInAddCategoryCSSLocator = "section.form_component div:has(label.form_label:contains(\"{0}\")) input";

        public const string GetAppealCategoryRowCountByProductCSSLocator = "ul.component_item_row span.data_point_value:contains({0})";

        public const string GetSelectedAppealCategoryLabelByColumnXpath =
            "//li[contains(@class,'component_item') and contains(@class,'is_active')]/ul/li[{0}]/label";

        public const string EditIconByCategoryIdXPathTemplate =
            "//label[text()='UITEST']/../../li//span[contains(@class,'small_edit_icon is_active')]";
        public const string EditIconByCategoryOrderXPathTemplate =
            "//span[text()='{0}']/../../li//span[contains(@class,'small_edit_icon is_active')]";
        //public const string LoadMoreCssLocator = "div.load_more_data span";
        #endregion

        #region PROTECTED PROPERTIES

        public override string PageTitle
        {
            get { return PageTitleEnum.AppealCategoryManager.GetStringValue(); }
        }

        #endregion

        #region CONSTRUCTOR

        public AppealCategoryManagerPageObjects()
            : base(PageUrlEnum.AppealCategoryManager.GetStringValue())
        {
        }

        #endregion
    }
}

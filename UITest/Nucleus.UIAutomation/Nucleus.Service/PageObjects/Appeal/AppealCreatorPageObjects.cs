using Nucleus.Service.PageObjects.Default;
using Nucleus.Service.Support.Enum;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Elements;
using UIAutomation.Framework.Utils;

namespace Nucleus.Service.PageObjects.Appeal
{
    public class AppealCreatorPageObjects : NewDefaultPageObjects
    {
        #region PRIVATE FIELDS

        #region TEMPLATE

        public const string SelectedClaimLinesByHeaderXpathTemplate= "//h2[text()='{0}']/following-sibling::ul//ul[contains(@class,'is_active')]";

        public const string AppealMenu = ".//div[@id='master_navigation']/ul/li/header[text()='Appeal']";
        public const string FileTypeAvailableValueXPathTemplate =
            "//section[contains(@class,'multi_select_wrap')]/ul/section[contains(@class,'available_options')]/li[text()='{0}']";
        public const string CliamLinesSectionByProCodeXPathTemplate =
            "//section[h2[text()='{0}']]/ul/div/li/div[ul/ul[1]/li[4]/span[text()='{1}']]/ul";

        public const string ClaimLinesNotHavingFlagXPathTemplate =
            "//section[h2[text()='{0}']]/ul/div/li/div[ul/ul[2]/div[@title='']][{1}]/ul";
        public const string ClaimSequenceXPathTemplate =
            "//section[h2[text()='{0}']]/ul/div/li/ul/li[1]/span";

        public const string LinesNoXPathTemplate =
            "//section[h2[text()='{0}']]/ul/div/li[ul/li/span[text()='{1}']]/div/ul/ul/li[1]/span";
        public const string ClearLinkOnAddClaimSectionXPath = "//ul/div/span[text()='Clear']";
        public const string ClaimLevelXPathTemplate = "//section[h2[text()='{0}']]/ul/div[{1}]/li/ul";
        //public const string ClaimLineXPathTemplate = "//section[h2[text()='Select Appeal Lines']]/ul/div/li/div[{0}]";
        public const string SelectAllCheckBoxXPath = "//div[div[text()='Select All Lines']]/span";
        public const string OkConfirmationCssSelector = "div#confirmation_links > div#complete_link";
        public const string CancelConfirmationCssSelector = "div#confirmation_links > span.span_link.modal_close";
        public const string ClaimLineHavingZeroBillSectionXPathTemplate =
            "//section[h2[text()='{0}']]/ul/div/li/div[ul/ul[2]/li[3]/span[text()='$0.00']]/ul{1}";
        public const string ClaimLineNotHavingZeroBillSectionCountXPathTemplate =
           "//section[h2[text()='{0}']]/ul/div[{2}]/li/div[ul/ul[2]/li[3]/span[text()!='$0.00']]/ul{1}";
        public const string ClaimLineNotHavingZeroBillAllSectionCountXPathTemplate =
           "//section[h2[text()='{0}']]/ul/div/li/div[ul/ul[2]/li[3]/span[text()!='$0.00']]/ul{1}";
        public const string ClaimLineNotHavingZeroBillSectionXPathTemplate =
           "//section[h2[text()='{0}']]/ul/div[{3}]/li/div[ul/ul[2]/li[3]/span[text()!='$0.00']][{1}]/ul{2}";
        public const string ClaimSequenceTemplate = "ul.component_nested_row";
        public const string AppealCreatorColumnContainerTemplate = "section.appeal_creator >section:nth-of-type({0})";

        public const string AppealCreatorColumnHeaderTemplate =
            "section.appeal_creator >section:nth-of-type({0}) h2.component_list_header";
        public const string SearchlistComponentItemLabelTemplate = "ul.component_item_list li:nth-of-type({0}) ul.component_item_row > li:nth-of-type({1}) label";
        public const string SearchlistComponentItemValueTemplate = "ul.component_item_list li:nth-of-type({0}) ul.component_item_row > li:nth-of-type({1}) span";
        public const string AddAppealOnClaimSearchTemplate = "ul.component_item_list li:nth-of-type({0}) ul.component_item_row  li.add_appeals";
        public const string AddAppealOnClaimByClaimSequenceTemplate = "ul.component_item_row:has( li>span.data_point_value:contains({0})) li.add_appeals";
        public const string AddAppealEnabledOnClaimSearchTemplate = "ul.component_item_list li:nth-of-type({0}) ul.component_item_row  li.add_appeals.is_enabled";
        public const string AddAppealDisabledOnClaimSearchTemplate = "ul.component_item_list li:nth-of-type({0}) ul.component_item_row  li.add_appeals.is_disabled";
        public const string LockTooltipOnClaimSearchPageOfAppealCreator = "ul.component_item_list li:nth-of-type({0}) ul.component_item_row li.lock.position_left";
        public const string AppealLevelBadgeOnSearchResultTemplate = "ul.component_item_list li:nth-of-type({0}) ul.component_item_row li.secondary_badge span";
        public const string AppealLevelBadgeByClaimSequenceOnSearchResultTemplate = "ul.component_item_row:has( li>span.data_point_value:contains({0})) li.secondary_badge span";
        public const string ClaimActionLinkOnSearchResultTemplate = "ul.component_item_list li:nth-of-type({0}) ul.component_item_row li.action_link span";
        public const string ClaimActionLinkByClaseqOnSearchResultTemplate = "ul.component_item_row:has( li>span.data_point_value:contains({0}))li.action_link span";
        public const string SearchlistComponentItemValueByClaimSeqTemplate = "//span[text()='{0}']/../../li[{1}]/span";
        public const string SearchlistClaimSeqTemplate = "//span[text()='{0}']";

        public const string CreateAppealFormInputXpathTempalte = "//label[text()[contains(.,'{0}')]]/../input";
        public const string AppealCreatorHeaderLocatorTemplate = "div.component_header li:nth-of-type({0}) span";
        #endregion

        public const string EnableClaimSequenceCssLocator =
            "ul.component_item_list >li:not([class*='disable']) li.action_link";
        public const string EnableCreateAppealIconCssLocator =
            "ul.component_item_list >li:not([class*='disable']) li.add_appeals";

        public const string SearchlistComponentRowDisabled = "ul.component_item_list li.is_disabled:nth-of-type(1)";
        public const string ClaimNoInAddClaimSectionCssLocator =
            "section.add_claim_appeal_form>ul:nth-of-type(2)>li li.no_link>span";
        public const string ClaimSequenceInAddClaimSectionCssLocator =
            "section.add_claim_appeal_form>ul:nth-of-type(2)>li li.action_link>span";
        public const string EmptyMessageCssLocator = "p.empty_search_results_message";
        public const string ClearLinkOnFindClaimSectionCssLocator = "div.current_viewing_searchlist span.span_link";
        public const string FindClaimSectionCssLocator = "section.is_slider:not([class*='is_hidden'])";
        public const string DescriptionCssLocator =
            "div.basic_input_component.uploader >ul:nth-of-type(1)>div:nth-of-type(3) input";
        public const string FileTypeCssLocator = "section.multi_select_wrap >input";
        public const string FileTypeToggleIconCssLocator = "section.multi_select_wrap >span";
        public const string FileTypeValueListCssLocator = "section.multi_select_wrap >ul>section.available_options>li";
        public const string FileTypeSelectedValueListCssLocator = "section.multi_select_wrap >ul>section.selected_options>li";
        public const string DocumentIDCssLocator =
            "section.appeal_create_form >ul:nth-of-type(1)>div:nth-of-type(4) input";
        public const string CloseIconOnFineClaimSectionCssSelector = "span.close_sidebar";
        public const string AddClaimRowSectionCssLocator = "section.add_claim_appeal_form>ul:nth-of-type(2)>li";
        public const string SearchIconCssLocator = "span.sidebar_icon";
        public const string AppealCreateFormHeaderCssLocator =
            "section.appeal_creator >section:nth-of-type(3) section.appeal_create_form> ul  header:nth-of-type(1)";

        public const string CancelAppealCreateButtonCssLocator = "section.appeal_creator >section:nth-of-type(3) section.appeal_create_form div.form_buttons span.span_link";

        public const string CreateAppealFormInputCssSelector =
            "section.appeal_create_form >ul:nth-of-type(1)  >div:nth-of-type({0}) input";
        public const string CreateAppealFormLabelCssSelector =
            "section.appeal_create_form >ul:nth-of-type(1)  >div:nth-of-type({0}) label:not(span)";
        public const string CreateAppealFormPhoneExtInputCssSelector =
            "section.appeal_create_form >ul:nth-of-type(1)  >div:nth-of-type({0}) input:nth-of-type({1})";
        public const string CreateAppealFormPhoneExtLabelCssSelector =
            "section.appeal_create_form >ul:nth-of-type(1)  >div:nth-of-type({0}) label:nth-of-type({1})";
        public const string ExclamationIconCssTemplate =
            "section.appeal_create_form >ul:nth-of-type(1)  >div:nth-of-type({0}) span.small_icon.field_error";
        public const string DisableCreateAppealIconTemplate = "//span[text()='{0}']/../../li/ul/li[contains(@class,'is_disabled')]";


        #region SelectAppealLines Css

        public const string AppealCreatorHeaderCssSelector = ".appeal_creator_header";
        public const string ClaimSeqLabelCssLocator =
            "section.appeal_creator >section:nth-of-type(1) >section>section>ul ul li:nth-of-type(1) label.data_point_label";
        public const string ClaimSeqValueCssLocator =
            "section.appeal_creator >section:nth-of-type(1) >section>section>ul ul li:nth-of-type(1) span.data_point_value";
        public const string ClaimNoLabelCssLocator =
            "section.appeal_creator >section:nth-of-type(1) >section>section>ul ul li:nth-of-type(2) label.data_point_label";
        public const string ClaimNoValueCssLocator =
            "section.appeal_creator >section:nth-of-type(1) >section>section>ul ul li:nth-of-type(2) span.data_point_value";
        public const string ClaimLinesCountCssLocator =
          "section.appeal_creator >section:nth-of-type(1) >section>section>ul li>div";
        public const string ClaimLineCssLocator =
          "section.appeal_creator >section:nth-of-type(1) >section>section>ul li>div:nth-of-type({0})> ul";

        public const string LineNoIconCssLocator =
            "section.appeal_creator >section:nth-of-type(1) >section>section>ul li div:nth-of-type({0}) li.has_badge:not(.line_appeal_count) span";
        public const string DateofServiceCssLocator =
            "section.appeal_creator >section:nth-of-type(1) >section>section>ul li div:nth-of-type({0}) ul> ul:nth-of-type(1) li:nth-of-type(3) span.data_point_value";
        public const string ProcCodeCssLocator =
            "section.appeal_creator >section:nth-of-type(1) >section>section>ul li div:nth-of-type({0}) ul> ul:nth-of-type(1) li:nth-of-type(4) span.data_point_value";
        public const string ProcDescriptionCssLocator =
            "section.appeal_creator >section:nth-of-type(1) >section>section>ul li div:nth-of-type({0}) ul> ul:nth-of-type(1) li:nth-of-type(5) span.data_point_value";
        public const string AppealLevelCssLocator =
            "section.appeal_creator >section:nth-of-type(1) >section>section>ul li div:nth-of-type({0})  ul> ul:nth-of-type(2) li:nth-of-type(1)";
        public const string AppealLevelIconCssLocator =
            "section.appeal_creator >section:nth-of-type(1) >section>section>ul li div:nth-of-type({0}) li.line_appeal_count span";
        public const string BilledAmountCssLocator =
            "section.appeal_creator >section:nth-of-type(1) >section>section>ul li div:nth-of-type({0}) ul> ul:nth-of-type(2) li:nth-of-type(3) span.data_point_value";
        public const string RevCodeCssLocator =
            "section.appeal_creator >section:nth-of-type(1) >section>section>ul li div:nth-of-type({0}) ul> ul:nth-of-type(2) li:nth-of-type(4) span.data_point_value";
        public const string FlagDivCssLocator =
            "section.appeal_creator >section:nth-of-type(1) >section>section>ul li div:nth-of-type({0}) ul> ul:nth-of-type(2) div.listed_top_flags";
        public const string FlagCssLocator =
            "section.appeal_creator >section:nth-of-type(1) >section>section>ul li div:nth-of-type({0}) ul> ul:nth-of-type(2) div.listed_top_flags span";
        public const string BoldRedFlagCssLocator =
            "section.appeal_creator >section:nth-of-type(1) >section>section>ul li div ul> ul:nth-of-type(2) div.listed_top_flags li.line_flag_must_be_worked";
        public const string RedFlagCssLocator =
            "section.appeal_creator >section:nth-of-type(1) >section>section>ul li div ul> ul:nth-of-type(2) div.listed_top_flags li.line_flag_can_be_worked ";
        public const string GrayFlagCssLocator =
            "section.appeal_creator >section:nth-of-type(1) >section>section>ul li div ul> ul:nth-of-type(2) div.listed_top_flags li.line_flag_cannot_be_worked";



        #endregion

        #region ID

        public const string AppealCreatorQuicklaunchTile = "qlAppealCreator";

        //private const string ProviderAndPatientLblId = "ctl00_MainContentPlaceHolder_headerSubNavLbl";
        //private const string ClaimsGridTableId = "ctl00_MainContentPlaceHolder_ClaimsGridControl_ClaimsGrid_ctl00";
        public const string ProductComboInputValueXPathTemplate =
            "//ul[contains(@class,'select_options')]/li[text()='{0}']";
        public const string ProductComboInputCssSelector = "div.select_input>input";
        public const string ProductComboInputToggleCssSelector = "div.select_input span.select_toggle";
        public const string ProductComboListCssSelector = "ul.select_options>li";
        public const string ProductComboListSelectCssSelector = "ul.select_options>li:nth-of-type({0})";
        public const string SelectedProductCssSelector = "ul.select_options>li.is_active";
        public const string ProductComboInputDisabledCssSelector =
            "div.select_input>input";
        public const string AppealRadioBoxInputXPath = "//span[text()='A']";
        //public const string AppealRadioBoxCss = "span[title = 'Record Review']";
        public const string RecordReviewRadioBoxInputXPath = "//span[text()='R']";
        public const string DentalAppealTypeRadioBoxInputXpath = "//span[text()='D']";
        public const string AppealTypeBoxDisabledXPathTemplate = "//span[text()='{0}']/../../span[contains(@class,'is_disabled')]";
        public const string AppealTypeBoxSelectionStatusXPath = "//span[contains(@class,'is_active')]/span[text()='{0}']";
                                                                //"//span[text()='{0}']/../../span[contains(@class,'is_active')]"; 
        public const string MedicalRecordReviewAppealTypeXPath = "//span[text()='M']/..";
        public const string AppealTypeBoxPresentXpath = "//span[contains(@class,'component_radio_button')]/span[text()='{0}']";
        public const string UrgentCheckBoxSelectionStatusXPath = "//div[contains(@class,'selected') and div[text()='Urgent']]/span[contains(@class,'active')]";
        public const string UrgentCheckBoxDisabledsXPath = "//div[div[text()='Urgent']]/span[contains(@class,'is_disabled')]";
        public const string UrgentCheckBoxInpuXPath = "//div[div[text()='Urgent']]/span";
        public const string SaveBtnXPath = "//button[text()='Save']";
        //private const string DocUploaderBtnId = "ctl00_MainContentPlaceHolder_DocUploaderBtn";

        public const string ClaimSequenceXPath = "//input[@placeholder='Claim Sequence' or @placeholder='Bill Sequence']";
        public const string ClaimNoXPath = "//input[@placeholder='Claim No']";
        public const string FindXPath = "//button[text()='Find']";

        #endregion
        #region class

        public const string flagIocnAddSectionCssLocator = "li.active_appeal_flags ";
        public const string LockIconCssLocator = "li.lock";
        public const string DisableCreateAppealIconCssLocator = "li.add_appeals.is_disabled";
        public const string ProviderNameLabelCssLocator = "li.header_data_point:nth-of-type(1)> label.data_point_label";
        public const string ProviderNameValueCssLocator = "li.header_data_point:nth-of-type(1)> span.data_point_value";
        public const string PatientSequenceLabelCssLocator = "li.header_data_point:nth-of-type(2)> label.data_point_label";
        public const string PatientSequenceValueCssLocator = "li.header_data_point:nth-of-type(2)> span.data_point_value";
        public const string AddAppealBtnOnClaimSearchPageOfAppealCreator = "li.add_appeals";
        public const string ClaimSearchPanelSectionCssLocator = "section.component_sidebar.column_undefined.is_slider";

        public const string ClaimSearchPanelDisabledSectionCssLocator = "section.component_sidebar.column_undefined.is_slider.is_hidden";

        public const string AlternateClaimNoInFindPanelLabelCssLocator =
            "section.component_sidebar.is_slider div.basic_input_component:nth-of-type(2)>label";
        public const string EmptySearchListMessageSectionCssLocator =
            "section.search_list.component_left p.empty_message";

        public const string SearchIconButtonInAppealCreatorLeftComponent =
            "section.component_left.search_list span.toolbar_icon";

        public const string RecentlyAddedAppealPanelCssLocator =
            "section.component_sidebar.column_undefined.is_slider .component_sidebar_panel:not(.filter_panel)";
        public const string RecentlyAddedAppealListCssLocator =
            "section.component_sidebar.column_undefined.is_slider .component_sidebar_panel:not(.filter_panel) ul.worklist_list li";
        public const string ClaimActionsListOnSearchResult = ".component_item_list li.action_link span";

        public const string AppealDocumentUploadFileBrowseCssLocator = "section.appeal_creator input.file_upload_field";

        public const string AppealCreatorUploaderFieldLabelCssLocator =
            "div.basic_input_component.uploader  div:nth-of-type({0}) label ";

        public const string AppealCreatorUploaderFieldValueCssLocator =
            "div.basic_input_component.uploader  div:nth-of-type({0}) input ";
        public const string RecentlyAddedAppealCssSelectorTemplate = "ul.worklist_list li:nth-of-type({0}) span.item_link";
        public const string RecentlyAddedAppealJsSelectorTemplate = "span:contains({0})";//to be used with a js selector

        #endregion

        #region Xpath

        public const string RecentlyAddedAppealXPathTemplate = "//span[text()='{0}']";

        public const string ClaimSequenceHavingLockInAddClaimSectionXPath =
            "//ul[li/ul/li[contains(@class,'lock')]]/li[2]/span";

        public const string ClaimLineHavingLockInAddClaimSectionXPath = "//ul[li/ul/li[contains(@class,'lock')]]";

        public const string AddDocumentButtonXpath = "//button[text()='Add File']";
        public const string FileToUploadSectionXpath = "//header[.//text()='Files To Upload']/../li/ul";
        public const string FileToUploadDetailsXpath = "//ul[header[.//text()='Files To Upload']]/li[{0}]//li[{1}]/span";
        public const string FieldInputXpath = "//label[text()='{0}']/../input";
        #endregion

        #endregion

        #region CONSTRUCTOR

        public AppealCreatorPageObjects()
            : base(PageUrlEnum.AppealCreator.GetStringValue())
        {
        }

        #endregion
    }
}

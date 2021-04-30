using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nucleus.Service.PageObjects.Default;
using Nucleus.Service.PageServices.Base.Default;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Utils;

namespace Nucleus.Service.PageObjects.Settings.User
{
    public class UserProfileSearchPageObjects : NewDefaultPageObjects
    {
        #region CONSTRUCTOR
        public UserProfileSearchPageObjects()
            : base(PageUrlEnum.UserProfileSearch.GetStringValue())
        {
        }
        #endregion

        #region PROTECTED PROPERTIES
        public override string PageTitle
        {
            get { return PageTitleEnum.UserProfileSearch.GetStringValue(); }
        }
        #endregion

        #region public methods

        #region New User Account

        public const string SelectOptionToBeAssignedByNameXPath = "//h2[text()='{0}']/..//span[text()='{1}']";
        public const string CreateNewUserTab = "div[title=\"{0}\"]";
        public const string ExtNoCssLocator = "input[placeholder='ext']";
        public const string NextButtonCssLocator = "ul:not([hidden]) button.right.work_button";
        public const string PreviousButtonCssLocator = "ul:not([hidden]) button.secondary_button";
        public const string CancelLinkCssLocator =
            "ul:not([hidden]) button.secondary_button:contains(Cancel) , ul:not([hidden]) span>span.span_link:contains(Cancel)";
        public const string NewUserAccountFormXPath =
            "//label[text()='New User Account']/../../following-sibling::section";
        public const string LabelXPathByLabel = "//label[text()='{0}']";
        public const string FormHeaderByHeaderNameXPath = "//ul[not(@style='display: none;')]//span[contains(@class,'form_header')]//label[text()='{0}']";
        public const string NewUserAccountTabsCssSelector = "div.option_button_selector>div";
        public const string ProfileHeadersCssSelector = "div.settings_sub_section>h2";
        public const string OptionsInAvailableOrAssignedByRow = "(//h2[text()='{0}']/.. /ul/ul/div[{1}]//span)[2]";
        public const string HeaderDescriptionCssLocator = "div.list_transfer_component > h2.component_list_header";
        public const string CreateUserButtonXPath = "//section[@class='form_component settings_form']//button[text()='Create User']";
        public const string CreateUserIconCss = ".add_icon";
        public const string SummaryLabelXPath = "//label[@title='{0}']";
        public const string SummaryValueXPath = "//label[@title='{0}']/../span";
        public const string FormHeaderXPath = "//span/label[text()='Profile']";

        #endregion

        public const string LoadMoreCssLocator = "span.load_more_data";
        public const string ActiveUserCssLocator = "li.active_icon";
        public const string InactiveuserCssLocator = "li.inactive_icon";
        public const string FrozenUserCssLocator = "li.frozen_icon";
        //public const string FieldErrorIconByLabelXpathTemplate =
        //    "//label[text()='{0}']/..//span[contains(@class,'field_error')]";    
        public const string LockedUserCssLocator = "li.lock";
        //public const string LinkableLoadMoreCssLocator = "span.load_more_data.span_link";
        public const string IconValueListCssLocator = "ul.component_item_list>div>ul>li>ul>li:nth-of-type(1)";
        public const string AddUserIconByCss = "li[title='Create New User']";
        public const string AddUserPopupByCss = "iframe[name=CreateNewUserWindow]";
        public const string CreateUserTitleBarByCss = "title:contains(Create New User)";
        public const string NextButtonId = "#ctl00_MainContentPlaceHolder_NextBtn";
        public const string UserNameInGridByXpath =
            "//ul[contains(@class,'component_item_row ') and  li/span[text()='{0}']]/li[2]";
        public const string AvailabeOptionByLabelOptionTemplateXpath = "//span[contains(@class,'InputSectionTitle') and contains(.,'{0}')]/..//span[text()='{1}']";
        public const string AvailableAcessListXpathTemplate =
            "//span[contains(@class,'InputSectionTitle') and contains(.,'{0}')]/..//div[contains(@id,'Available')]//li";
        public const string RadAjaxPanelDivCssLocator = "div:not([style*='none'])>div.raDiv";
        public const string GetAllArrowsByLabelXpathTemplate = "(//span[contains(@class,'InputSectionTitle') and contains(.,'{0}')]/..//span[contains(@class,'rlbButtonTL')])[{1}]";
        public const string AvailableRolesPrivilegesXPath = "//h2[text()='Available Roles']/.. /ul/ul/div//span[contains(@class,'data_point_value')]";
        public const string AssignedRolesPrivilegesXPath = "//h2[text()='Assigned Roles']/.. /ul/ul/div//span[contains(@class,'data_point_value')]";
        public const string InfoIconAtSideOfAvailableRolesCssSelector = "div.left_list:has(h2:contains(Available Roles)) span.info_icon";
        public const string UserProfileLinksOnSearchResult = ".component_item_list li.action_link span";
        public const string FilterOptionsListCssLocator = "li.appeal_search_filter_options";
        public const string FilterOptionValueByCss = "li.appeal_search_filter_options>ul>li:nth-of-type({0})>span";
        public const string FilterOptionListByCss = "li.appeal_search_filter_options>ul>li>span";

        #region User Settings Side View

        public const string AllTextBoxXPath = "//section[contains(@class,'settings_form')]//li[contains(@class,'form_item')]//input";
        public const string UserSettingsTabListCssLocator = "div.option_button_selector >div";
        public const string SelectedUserSettingTabCssLocator = "div.option_button_selector >div.is_selected";
        public const string UserSettingTabXPathByTabName = "//div[contains(@class,'option_button_selector')]/div[text()='{0}']";
        public const string UserSettingsFormFieldCssLocator = ".form_component.settings_form";
        public const string InputFieldByLabelCssLocator = "li:has(label:contains({0})) input";
        public const string LabelXPath = "//section[contains(@class,'form_component')]//label[text()='{0}']";
        public const string PhoneFaxInputByLabel = "//label[text()='{0}']/..//li[contains(@class,'main-phone-input')]/input";
        public const string PhoneFaxExtInputByLabel = "//label[text()='{0}']/..//li[contains(@class,'phone-extension-input')]/input";
        public const string DropDownOptionsListValueByLabel = "li:has(label:contains({0})) li.option:contains({1})";
        public const string DropDownOptionListByLabel = "li:has(label:contains({0})) li";

        public const string UserSettingsContainerCssSelector = "div.option_button_selector> div.option_button";

        public const string UserSettingsContainerTitleCssSelector = "section.settings_form.form_component>ul:nth-of-type({0})>li>header>span>label";

        public const string UserSettingsContainerByLabelXPath = "//div[contains(text(), '{0}')]";

        public const string InfoHelpIconByLabelCssLocator = "label:has(span:contains({0})) .info_icon";
        public const string HeaderinfoHelpIconByLabelCssLocator = "span:contains({0})~span>.info_icon";
        public const string TabinfoHelpIconByLabelCssLocator = "div[title={0}] span.info_icon";

        public const string EditIconCssLocator = "ul:not([hidden]) span.small_edit_icon";

        public const string RadioButtonByLabelXPathTemplate =
            "//div[*/span[text()='{0}']]/span[{1}]";

        public const string RadioButtonByLabelPresentXPathTemplate = "//div[*/span[text()='{0}']]/span[contains(@class,'radio_button')]";

        public const string NotificationLabelXpathLocator =
            "//header[*/label[text()='Notifications']]/../..//div//label/span";

        public const string ClientListLabelXPathTemplate =
            "//ul[contains(@class,'settings_form_body')][5]//div[contains(@class,'list_transfer')][1]//div[contains(@class,'{0}')]/h2";
       
        

        public const string ModifySettingsXPathTemplate = "//label[text()='{0}']/../..//span[@title='Modify Settings']";

        public const string SelectDeselectAllCssSelector = "div:has(h2:contains({0}))>li>span";

       

        public const string ExtUserIdByClientNameXPathTemplate = "//span[text()='{0}']/../../..//input";

        public const string SaveButtonXPathTemplate = "//button[text()='Save']";

        public const string CancelButtonXPathTemplate = "//span[text()='Cancel']";

        public const string LastModifiedXPathTemplate = "//label[text()='Last Modified by:']/../span";

        public const string ShowAnswerCssSelector =
            "li.component_flat_input:nth-of-type({0}) li.unhide_link span";

        public const string TooltipXPathTemplate =
            "//span[text()='{0}']/../span[contains(@class,'info_icon')]";
        

        #endregion

        public const string LeftFormHeaderCssSelector =
            "div.full_content section.component_left:nth-of-type(3) div.component_header_left label";

        #endregion
    }
}

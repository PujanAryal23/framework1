using System.Net;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Base;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Elements;

namespace Nucleus.Service.PageObjects.Default
{
    public class NewDefaultPageObjects : NewPageBase
    {

        #region PUBLIC FIELDS

        public const string NucleusHeaderCssLocator = "div.verisk_user>section:nth-of-type(1)";
        public const string ClientNucleusHeaderCssLocator = "div.client_user>section:nth-of-type(1)";
        public const string UnAuthorizedMessageCssSelector = "//p[contains(@class,'return_link')]/../p[1]";
        public const string ReturnToTheLastPageLinkCssSelector = "//p[contains(@class,'return_link')]//a";
        public const string NucleusFooterId = "footer";
        public const string NucleusLogoId = "nucleus_logo";
        public const string LandingImageCssLocator = "div.landing_image";
        #region VALUES

        public const string DocUploaderWindowName = "NucleusPopup";

        #endregion

        #region XPath
        public static string SubMenuCategoryListXPathTemplate = "//li/header[text()='{0}']/../ul/li/span | //li/header[text()='Appeal']/../ul/li/a";
        public const string ServerErrorPageXpathTemplate = "//h1[contains(text(),\"Server Error\")]";
        #endregion

        #region CssLocator

        public const string SpinnerCssLocator = ":not(.is_hidden)>div.spinner_wrap,:not(.is_hidden)>.loading_spinner";
        public const string PageHeaderCssLocator = "label.page_title,label.PageTitle";
        public const string CheckCurrentClientFromClaimUrl = "li a[href*='{0}']";
        //public const string currentClientCssLocator = "div.current_client>input";

        public const string PopupPageTitleCssLocator = ".PopupPageTitle";
        public const string LoggedInUserFullNameToolTip =
            "div#welcome_user";

        public const string DownloadCIWIconCssSelector = "div.ciw_button";
        public const string EditSettingsIcon = "li[title = 'Edit Settings Manager']";//"span.edit_settings";
        public const string CurrentClientCssSelector = "div.current_client>input";
        #endregion

        #region ID

        public const string ClientSmallBrandingImgId = "ctl00_ClientSmallBrandingImg"; //retro
        public const string ClientSmallBrandingCssLocator = "div#client_logo>img"; //ember
        public const string SwitchClientCssSelector = "div.current_client>input,div.current_client>label";
        public const string SwitchClientCaretIconCssSelector = ".client_select .select_toggle";
        public const string SwitchClientListValueXPathTemplate = "//div[contains(@class,'client_options')]//ul[li[label[text()='{0}']]]";
        public const string SwitchClientListCssLocator = "div.client_options>div:nth-of-type(2) li:nth-of-type(1)>label";
        public const string SwitchClientListOptionsCssLocator = "div.client_options";

        public const string MostRecentHeaderXpath = "//div[@class='switch_client_header']/label[contains(text(),'Most Recent')]";
        public const string AllClientsHeaderXpath = "//div[@class='switch_client_header']/label[contains(text(),'All Clients')]";
        public const string MostRecentClientsCodesCss = "div.switch_client_header:has(label:contains({0})) ~ ul>li:nth-of-type({1})";
        public const string LogoImageTemplate = "logo_{0}.png";
        public const string UserMenuId = "user_menu";
        public const string ManagerTab = "ul>li.top_nav:nth-of-type(8)";
        public const string ManagerTabByCss = "ul>li.top_nav:nth-of-type(8):contains(Manager)";
        public const string ManagerTabSubMenu = "ul>li.top_nav:nth-of-type(8):contains(Manager)>ul li";
        public const string UserMenuDivCssLocator = "div#user_menu ul.main_nav";
        public const string UserGreetingMenuLabel = "li.top_nav>header";
        public const string UserGreetingMenuList = "ul.main_nav>li>span";
        public const string QuickLinksOptionsCssSelector = "ul.main_nav>li>ul>li>span,ul.main_nav>li>ul>li>a";
        public const string TopNavMenuOptions = "div#master_navigation>ul>li>header";
        public const string CotivitiLinksOptionsCssSelector = "ul.main_nav>li>ul>li>ul>li>a";
        public const string LogOutButtonXPath = "//span[text()='Sign Out'] | //a[text()='Sign Out']";
        public const string HelpCenterXPath = "//span[text()='Help Center']";
        public const string DocumentLinkId = "ctl00_MainContentPlaceHolder_DocUploaderBtn";
        public const string QuickLaunchButtonXPath = "//span[text()='My Profile'] | //a[text()='My Profile']";
        public const string DashboardIconXPath = "//span[text()='Dashboard']";
        public const string MyProfileIconXPath = "//span[text()='My Profile']";
        public const string QuickLinksXPath = "//span[contains(text(),'Quicklinks')]";
        public const string CotivitiLinksCssSelector = "ul.main_nav>li:nth-of-type(2)>ul>li:nth-of-type(1)>span";
        public const string CMSLinkCssSelector = "ul.main_nav>li:nth-of-type(2)>ul>li:nth-of-type(2)>a";
        public const string CotivitiLinksByLinkNameXPath = "//a[contains(text(),'{0}')]";

        #endregion

        #region PopupModal

        public const string OkConfirmationCssSelector = "div#confirmation_links > div#complete_link";
        public const string CancelConfirmationCssSelector = "div#confirmation_links > span.span_link.modal_close";
        public const string PageErrorPopupModelId = "nucleus_modal_wrap";
        public const string PageErrorCloseId = "nucleus_modal_close";
        public const string PageErrorMessageId = "nucleus_modal_content";
        public const string WorkingAjaxMessageCssLocator = "div.loading_spinner>div.loader"; //:not(.is_hidden)>div.small_loading
        #endregion

        #region Invalid Input

        public const string InvalidInputByLabelXPathTemplate =
            "//label[text()='{0}']/..//input[contains(@class,'invalid')] | //span[text()='{0}']/../..//input[contains(@class,'invalid')] | //label[text()='{0}']/following-sibling::div//input[contains(@class,'invalid')]";

        public const string NoteDivCssLocator =
            "div.cke_browser_webkit.invalid";

        public const string NoteDivByLabelXPathTemplate =
            "//label[text()='{0}']/following-sibling::*[contains(@class,'cke_browser_webkit') and contains(@class,'invalid')] ";

        public const string AllInvalidCssLocator =
            ".invalid";

        public const string InvalidDropdownInputFieldCssSelector = "ul:not([style*=none]) div.select_component:has(label:contains({0})) input";

        public const string TransferComponentByLabelCssSelector = ".list_transfer_component div:has(*:contains({0})) ul.invalid";
        public const string DisplayedTransferComponentByLabelCssSelector = ".list_transfer_component div:has(*:contains({0})) ";

        public const string DeselectAllLinkInTransferComponentCssSelector = "span[title='<< Deselect All']";
        public const string SelectAllLinkInTransferComponentCssSelector = "span[title='Select All >>']";

        #endregion

        #region Available/Assigned 


        public const string AvailableAssignedRowXPathTemplte =
            "//div[h2[text()='{0}']]//span[text()='{1}']";

        public const string AvailableAssignedListCssLocator = "div.column_50:has(h2:contains({0})) ul.component_item_row span.data_point_value";

        public const string AllAssignedListXPath = "//div[h2[text()='{0}']]//span";
        //   ".list_transfer_component >div.column_50:contains({0}) ul.component_item_list div ul li";
        public const string AllAvailableListCssLocator =
            ".list_transfer_component >div.left_list:contains({0}) ul.component_item_list div ul li";

        public const string AvailableAssignedClientsDivCssSelectorTemplate = "div:has(h2:contains({0}))>ul>ul>div:has(*:contains({1}))";

        #endregion

        #endregion

        #region CONSTRUCTOR

        public NewDefaultPageObjects(string pageUrl)
            : base(pageUrl)
        {
        }

        public NewDefaultPageObjects(string baseUrl, string pageUrl)
            : base(baseUrl, pageUrl)
        {

        }

        public NewDefaultPageObjects()
        {

        }

        #endregion
    }
}

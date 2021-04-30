using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nucleus.Service.PageObjects.Default;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Utils;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Elements;

namespace Nucleus.Service.PageObjects.Settings.User
{
    public class OldUserProfileSearchPageObjects : NewDefaultPageObjects
    {
        #region PRIVATE/PUBLIC PROPERTIES

        public const string RadAjaxPanelDivCssLocator = "div:not([style*='none'])>div.raDiv";

        public const string CreateUserFrameName = "CreateNewUserWindow";

        public const string PopUpErrorDivId = "RadWindowWrapper_ctl00_MainContentPlaceHolder_PopErrorsWindow";
        public const string ErrorLabelId = "ctl00_MainContentPlaceHolder_ErrorLabel";
        public const string AddUserId = "ctl00_MainContentPlaceHolder_CreateNewUserBtn";
        public const string FirstnameId = "ctl00_MainContentPlaceHolder_FirstNameTxt";
        public const string LastnameId = "ctl00_MainContentPlaceHolder_LastNameTxt";
        public const string EmailAddressId = "ctl00_MainContentPlaceHolder_EmailTxt";
        public const string PhoneId = "ctl00_MainContentPlaceHolder_PhoneTxt";
        public const string UserId = "BackButton";
        public const string PasswordId = "ctl00_MainContentPlaceHolder_PasswordBox";
        public const string ConfirmPasswordId = "ctl00_MainContentPlaceHolder_PasswordVerifyBox";
        public const string UserTypeId = "ctl00_MainContentPlaceHolder_UserTypeCombo_Input";
        public const string UserStatusId = "ctl00_MainContentPlaceHolder_StatusCombo_Input";
        public const string NextButtonId = "ctl00_MainContentPlaceHolder_NextBtn";
        public const string RightArrowButtonCssSelector = "a.rlbButton.rlbNoButtonText.rlbTransferFrom";
        public const string LeftArrowButtonCssSelector = "a.rlbButton.rlbTransferTo.rlbNoButtonText";
        public const string DefaultClientId = "ctl00_MainContentPlaceHolder_DefaultClientBox_Input";
        public const string DashboardViewDropdownOptionsListXpath =
            "//div[@id='ctl00_MainContentPlaceHolder_DefaultDashboardProductCombo_DropDown']/div/ul/li";
        public const string DashboardViewDropdownOptionsXpath =
            "//div[@id='ctl00_MainContentPlaceHolder_DefaultDashboardProductCombo_DropDown']/div/ul/li[text()='{0}']";
        public const string DashboardViewToggleIcon = "ctl00_MainContentPlaceHolder_DefaultDashboardProductCombo_Arrow";
        public const string DefaultDashboardView = "DefaultDashboardLabel";
        public const string DashboardProductType = "ctl00_MainContentPlaceHolder_DefaultDashboardProductCombo_Input";
        public const string DashboardInPreferencesSectionXpath = "//select[@class='ListboxPad']/option[4]";

        public const string AuthorityToggleIcon
            = "//div[@id='ctl00_MainContentPlaceHolder_AssignedAuthoritiesOuterBox']//li//label[text()='{0}']/..//div//td[2]/a";

        public const string SelectDashboardAuthorityXpathTemplate =
            "//div[@class='rcbSlide']/div//ul[@class='rcbList']/li[text()='{0}']";
        public const string SelectReportsAuthorityXpathTemplate =
            "//div[@id='ctl00_MainContentPlaceHolder_AssignedAuthoritiesOuterBox_i1_AssignedCombo_DropDown']/div/ul/li[text()='{0}']";
        public const string AvailableAuthorityall= "//div[@class='rcbSlide']/div//ul[@class='rcbList']/li";


        public const string AssignedAuthorityType = "ctl00_MainContentPlaceHolder_AssignedAuthoritiesOuterBox_i0_AssignedCombo_Input";
        public const string AssignedAuthorityRowXpath = "//div[@id='ctl00_MainContentPlaceHolder_AssignedAuthoritiesOuterBox']/div/ul/li";
        public const string WizardBackButtonXpath = "//a[@class='WizardBackButton']";
        public const string CloseWindowCssSelector = "a.rwCloseButton";
        public const string SearchButtonId = "ctl00_MainContentPlaceHolder_SearchBtn";

        public const string UserTypeXPathTemplate =
            "//div[@id='ctl00_MainContentPlaceHolder_UserTypeCombo_DropDown']/div/ul/li[text()='{0}']";

        public const string UserStatusXPathTemplate = 
            "//div[@id='ctl00_MainContentPlaceHolder_StatusCombo_DropDown']/div/ul/li[text()='{0}']";

        public const string AvailableClientXPathTemplate = "//span[text()='{0}']";

        public const string DefaultClientXPathTemplate =
            "//div[@id='ctl00_MainContentPlaceHolder_DefaultClientBox_DropDown']/div/ul/li[text()='{0}']";

        public const string PreferencesId = "ctl00_MainContentPlaceHolder_ListBoxPrefSummary";

        public const string IsRequestInProgress = "ctl00_MainContentPlaceHolder_RadAjaxManager1._isRequestInProgress";
        public const string TblGridHeaderId = "ctl00_MainContentPlaceHolder_RadGrid1_ctl00";
        public const string FirstRowUserNameCssLocator = "#ctl00_MainContentPlaceHolder_RadGrid1_ctl00__0 > td:nth-child(2)>a ";
        public const string UserNameByUserIDXpath = "//td[(text()='{0}')]/../td/a";

        public const string AvailabeOptionByLabelOptionTemplateXpath="//span[contains(@class,'InputSectionTitle') and contains(.,'{0}')]/..//span[text()='{1}']";
        public const string GetAllArrowsByLabelXpathTemplate = "(//span[contains(@class,'InputSectionTitle') and contains(.,'{0}')]/..//span[contains(@class,'rlbButtonTL')])[{1}]";

        public const string AvailableAcessListXpathTemplate =
                "//span[contains(@class,'InputSectionTitle') and contains(.,'{0}')]/..//div[contains(@id,'Available')]//li"
            ;
        public const string AssignedAccessListXpathTemplate =
                "//span[contains(@class,'InputSectionTitle') and contains(.,'{0}')]/..//div[contains(@id,'Assigned')]//li"
            ;

        public const string UserPreferenceDefaultPage = "div [id='ctl00_MainContentPlaceHolder_LandingPageCombo_DropDown'] ul li";
        public const string UserpreferenceDefaultPageById = "ctl00_MainContentPlaceHolder_LandingPageCombo_Input";
        public const string UserPreferenceDefaultPageById = "ctl00_MainContentPlaceHolder_LandingPageCombo_Arrow";
        public const string AddUserPopUpTitle = "span.InputSectionTitle";

        public const string GetListValuesByLabelXpath =
            "//span[contains(@class,'InputSectionTitle') and contains(.,'{0}')]/..//li";

        public const string LabelNameXPathSelector = "//label[text() = '{0}']";
        public const string SpanLabelNameXPathSelector = "//span[text() = '{0}']";

        public const string AssignedRoleXpathTemplate =
            "//div[@id='ctl00_MainContentPlaceHolder_AssignedRolesBox']//li/span";

        public const string AssignedPrivilegeXpathTemplate =
            "//div[@id='ctl00_MainContentPlaceHolder_AssignedAuthoritiesOuterBox']//li//label[text()='{0}']/..";

        public const string AvailableRolesPrivilegesCssSelector =
            "div#ctl00_MainContentPlaceHolder_AvailableRolesBox>div>ul.rlbList>li>span";
        #endregion

        #region PAGEOBJECTS PROPERTIES

        //[FindsBy(How = How.Id, Using = PopUpErrorDivId)] 
        //public Div PopupErrorDiv;

        //[FindsBy(How = How.Id, Using = ErrorLabelId)] 
        //public TextLabel ErrorLabel;

        //[FindsBy(How = How.CssSelector, Using = RightArrowButtonCssSelector)]
        //public ImageButton RightArrowButton;

        //[FindsBy(How = How.CssSelector, Using = LeftArrowButtonCssSelector)]
        //public ImageButton LeftArrowButton;

        //[FindsBy(How = How.Id, Using = DefaultClientId)]
        //public InputButton DefaultClientInput;

        //[FindsBy(How = How.Id, Using = AddUserId)]
        //public Link AddUserLink;

        //[FindsBy(How = How.XPath, Using = WizardBackButtonXpath)]
        //public Link BackButton;
       
        //[FindsBy(How = How.Id, Using = SearchButtonId)]
        //public Link SearchButton;

        //[FindsBy(How = How.Id, Using = FirstnameId)]
        //public InputButton FirstnameInput;

        //[FindsBy(How = How.Id, Using = LastnameId)]
        //public InputButton LastnameInput;

        //[FindsBy(How = How.Id, Using = EmailAddressId)]
        //public InputButton EmailAddressInput;

        //[FindsBy(How = How.Id, Using = PhoneId)]
        //public InputButton PhoneInput;

        //[FindsBy(How = How.Id, Using = UserId)]
        //public InputButton UserIdInput;

        //[FindsBy(How = How.Id, Using = PasswordId)]
        //public InputButton PasswordInput;

        //[FindsBy(How = How.Id, Using = ConfirmPasswordId)]
        //public InputButton ConfirmPasswordInput;

        //[FindsBy(How = How.Id, Using = UserTypeId)]
        //public InputButton UserTypeInput;

        //[FindsBy(How = How.Id, Using = UserStatusId)]
        //public InputButton UserStatusInput;

        //[FindsBy(How = How.Id, Using = NextButtonId)]
        //public CustomButton NextButton;

        //[FindsBy(How = How.CssSelector, Using = CloseWindowCssSelector)]
        //public Link CloseWindow;

        //[FindsBy(How = How.Id, Using = TblGridHeaderId)]
        //public Table TblGridHeader;

        //[FindsBy(How = How.CssSelector, Using = FirstRowUserNameCssLocator)]
        //public Link UserNameLink;
       
        #endregion

        #region PROTECTED PROPERTIES

        public override string PageTitle
        {
            get { return PageTitleEnum.CreateNewUser.GetStringValue(); }
        }

        #endregion

       #region CONSTRUCTOR

       public OldUserProfileSearchPageObjects()
            : base(PageUrlEnum.OldUserProfileSearch.GetStringValue())
        {
        }

        #endregion
    }
}

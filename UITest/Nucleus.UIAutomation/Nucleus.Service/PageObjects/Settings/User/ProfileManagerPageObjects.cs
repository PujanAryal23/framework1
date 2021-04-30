using Nucleus.Service.PageObjects.Default;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Utils;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Elements;

namespace Nucleus.Service.PageObjects.Settings.User
{
    public class ProfileManagerPageObjects : NewDefaultPageObjects
    {
		#region PRIVATE/PUBLIC FIELDS

        public const string ProfileTabId = "ctl00_MainContentPlaceHolder_ProfileTab";
        public const string PrivilegesTabId = "ctl00_MainContentPlaceHolder_PrivilegesTab";
        public const string SecurityTabId = "ctl00_MainContentPlaceHolder_SecurityTab";
	    public const string RestrictionsTabId = "ctl00_MainContentPlaceHolder_RestrictionsTab";
		public const string NewPasswordTextBoxId = "ctl00_MainContentPlaceHolder_NewPasswordTxt";
        public const string ConfirmNewPasswordTextBoxId = "ctl00_MainContentPlaceHolder_ConfirmNewPasswordTxt";
        public const string SecurityQuestionTextBoxId = "ctl00_MainContentPlaceHolder_Question{0}Combo_Input";
        public const string SecurityQuestionArrowCaretId = "ctl00_MainContentPlaceHolder_Question{0}Combo_Arrow";
        
        public const string SecurityQuestion1TextBoxId = "ctl00_MainContentPlaceHolder_Question1Combo_Input";
        public const string SecurityQuestion2TextBoxId = "ctl00_MainContentPlaceHolder_Question2Combo_Input";
        public const string SecurityAnswer1TextBoxId = "ctl00_MainContentPlaceHolder_Answer1Txt";
        public const string SecurityAnswer2TextBoxId = "ctl00_MainContentPlaceHolder_Answer2Txt";
        public const string SaveButtonId = "ctl00_MainContentPlaceHolder_SaveBtn";
        public const string CancelButtonId = "ctl00_MainContentPlaceHolder_CancelBtn";
        public const string ErrorModalPopupId = "RadWindowWrapper_ctl00_MainContentPlaceHolder_PopErrorsWindow";
        public const string ErrorMessageXPath = "//span[@id='ctl00_MainContentPlaceHolder_ErrorLabel']";
        public const string PciWorkListInputCssLocator = "li[authorityname=PCI_WORK_LIST] > span > div > table > tbody > tr > td > input";
        public const string FciWorkListInputCssLocator = "li[authorityname=FCI_WORK_LIST] > span > div > table > tbody > tr > td > input";
        public const string CloseButtonClassName = "rwCloseButton";

        public const string UserPreferenceDefaultDashboard = "//li[text() = '{0}']";

        public const string UserPreferenceDefaultPage =
            "div [id='ctl00_MainContentPlaceHolder_LandingPageCombo_DropDown'] ul li";
        public const string PrivilegesAvailableSpanCssLocator =
            "#ctl00_MainContentPlaceHolder_AvailableAuthoritiesBox span.rlbText:contains({0})";
        public const string UserPreferenceDefaultDashboardView = "ctl00_MainContentPlaceHolder_dDefDashboardProduct";
        public const string UserpreferenceDefaultPageById = "ctl00_MainContentPlaceHolder_LandingPageCombo";
        public const string UserpreferenceDefaultPageArrowById = "ctl00_MainContentPlaceHolder_LandingPageCombo_Arrow";
        public const string UserPreferenceDefaultClient = "ctl00_MainContentPlaceHolder_DefaultClientBox";
        public const string AssignedAuthorityLabelXPathTemplate = "//div[@id='ctl00_MainContentPlaceHolder_AssignedAuthoritiesOuterBox']/div/ul/li/span/div/label[text()='{0}']";
        public const string AssignedAuthorityValueXpathTemplate = "//div[@id='ctl00_MainContentPlaceHolder_AssignedAuthoritiesOuterBox']/div/ul/li/span/div/label[text()='{0}']/../..//td[1]/input";
        
        public const string AssignedRoleLabelXPathTemplate = "//div[@id='ctl00_MainContentPlaceHolder_AssignedRolesBox']/div/ul/li/span[text()='{0}']";

        public const string AssignedRoleXpath =
            "//div[@id='ctl00_MainContentPlaceHolder_AssignedRolesBox']/div/ul/li[1]";

        public const string PrivilegesAvailableorAssignedSpanCssLocator =
            "#ctl00_MainContentPlaceHolder_AvailableAuthoritiesBox span.rlbText:contains({0}), #ctl00_MainContentPlaceHolder_AssignedAuthoritiesOuterBox span.rlbText:contains({0})";
        public const string DefaultPage = "ctl00_MainContentPlaceHolder_LandingPageCombo_Input";
        public const string DefaultDashboard = "ctl00_MainContentPlaceHolder_DefaultDashboardProductCombo_Input";
        public const string SelectAssignedAuthorityXpathTemplate =
            "//div[@id='ctl00_MainContentPlaceHolder_AssignedAuthoritiesOuterBox']/div/ul/li/span/div/label[text()='{0}']";

	    public const string SelectAvailableRestrictionXpathTemplate =
		    "//div[@id='ctl00_MainContentPlaceHolder_AvailableRestrictionsBox']/div/ul/li/span[text()='{0}']";
	    public const string SelectAssignedRestrictionXpathTemplate =
			"//div[@id='ctl00_MainContentPlaceHolder_AssignedRestrictionsBox']/div/ul/li/span[text()='{0}']";
	    public const string LeftArrowButtonXPath = "//a[@class='rlbButton rlbNoButtonText rlbTransferTo']";
        public const string LeftArrowButtonInRestrictionXPath = "//div[@id='dClaimsAvail']//a[@class='rlbButton rlbNoButtonText rlbTransferTo']";

        public const string RightArrowInRestrictionButtonXPath =
                                                        "//div[@id='dClaimsAvail']//a[@class='rlbButton rlbNoButtonText rlbTransferFrom']";
        //"//div[@id='dAvailRestrictions']//a[@class='rlbButton rlbNoButtonText rlbTransferFrom']";

        public const string AllRightArrowButtonXPath = "//div[@id='ctl00_MainContentPlaceHolder_AvailableRestrictionsBox']//a[contains(@class,'rlbTransferAllFrom')]";

        public const string AllLeftArrowButtonXPath = "//div[@id='ctl00_MainContentPlaceHolder_AvailableRestrictionsBox']//a[contains(@class,'rlbTransferAllTo')]";
        public const string RadAjaxPanelDivCssLocator = "div:not([style*='none'])>div.raDiv";

        public const string ShowAnswer1Id = "ctl00_MainContentPlaceHolder_ShowAnswer1Txt";
        public const string ShowAnswer2Id = "ctl00_MainContentPlaceHolder_ShowAnswer2Txt";
        public const string HiddenAnswer1Id = "ctl00_MainContentPlaceHolder_HiddenAnswer1Txt";
        public const string HiddenAnswer2Id = "ctl00_MainContentPlaceHolder_HiddenAnswer2Txt";

        public const string QuestionDropDownValueSelectionCssTemplate = "div#ctl00_MainContentPlaceHolder_Question{0}Combo_DropDown ul.rcbList li:contains({1})";
        public const string QuestionDropDownListCssTemplate = "div#ctl00_MainContentPlaceHolder_Question{0}Combo_DropDown ul.rcbList li";
        public const string GetAllArrowsByLabelXpathTemplate = "(//span[contains(@class,'InputSectionTitle') and contains(.,'{0}')]/..//span[contains(@class,'rlbButtonTL')])[{1}]";

        //public const string AssignedAuthorityXpathTemplate = "//div[contains(@class,'stacked_label')]//label[text()='{0}']/../..//div[contains(@class,'RadComboBox RadComboBox_Nucleus')]";
        public const string AssignedAuthorityCssLocator = "span:has(div.stacked_label label:contains({0})) div[class=\"RadComboBox RadComboBox_Nucleus\"] input[class=\"rcbInput radPreventDecorate\"]";
        //public const string AuthorityTypeXpathTemplate = "//form//div[contains(@style,'visibility: visible')]//li[text()='{0}']";
        public const string AuthorityTypeCssLocator = "form div[style*=\"visibility: visible\"][style*=\"overflow: visible\"] li:contains({0})";

        public const string DefaultpagebyCss =
            "div [id='ctl00_MainContentPlaceHolder_LandingPageCombo_DropDown'] ul li[text()='{0}']";
            //"div [id='ctl00_MainContentPlaceHolder_LandingPageCombo_DropDown'] ul li:contains({0})";
        public const string CancelBtnCssLocator = "a.CancelButton";
        public const string SaveBtnCssLocator = "div.button_segment input[value=Save]";

        public const string CompleteYourProfileFrameName = "CompleteYourProfileWindow";
        public const string SecurityQ1Id = "ctl00_MainContentPlaceHolder_Question1Combo_Input";
        public const string SecurityQ2Id = "ctl00_MainContentPlaceHolder_Question2Combo_Input";
        public const string BackButtonCssSelector = "a#ctl00_MainContentPlaceHolder_BackBtn";
        public const string SubscriberIdById = "ctl00_MainContentPlaceHolder_SubscriberIDTxt";
        public const string ClientTypeId = "ctl00_MainContentPlaceHolder_UserTypeCombo_Input";
        public const string FirstNamebyId = "ctl00_MainContentPlaceHolder_FirstNameTxt";
        public const string LastNamebyId = "ctl00_MainContentPlaceHolder_LastNameTxt";
        public const string RightArrowButtonCssSelector = "a.rlbButton.rlbNoButtonText.rlbTransferFrom";
        public const string AvailableClientXPathTemplate = "//span[text()='{0}']";
        public const string WizardBackButtonXpath = "//a[@class='WizardBackButton']";
        public const string CloseWindowCssSelector = "a.rwCloseButton";
        public const string SpanLabelNameXPathSelector = "//span[text() = '{0}']";

        public const string AvailableAcessListXpathTemplate =
            "//span[contains(@class,'InputSectionTitle') and contains(.,'{0}')]/..//div[contains(@id,'Available')]//li";

        public const string AutoDisplayPatientClaimHistoryId = "ctl00_MainContentPlaceHolder_AutoPatientClaimHistory";


        #region  restrictions tab
        public const string ClaimViewSectionTitleCssLocator = "#RestrictedClaimsAccess .InputSectionTitle";
		public const string ClaimRestrictionContainerTitleCssLlocator = "(//span[text()='Restricted Claims Access']/..//label)[{0}]"; //"#Restrictions>div:nth-of-type({0}) label";
        //"#dClaimsAvail label"
        public const string AvailableRestrictionsListCssLocator =
                            "#ctl00_MainContentPlaceHolder_AvailableRestrictionsBox ul>li>span";

        public const string AssignedRestrictionsListCssLocator =
                            "#ctl00_MainContentPlaceHolder_AssignedRestrictionsBox ul>li>span";

        public const string AssignedClientsListXpath = "//div[@id='ctl00_MainContentPlaceHolder_AssignedClientsBox']/div/ul/li/span/div/label";

		#endregion



		#endregion

		#region PROTECTED PROPERTIES

		public override string PageTitle
        {
            get { return PageTitleEnum.ProfileManager.GetStringValue(); }
        }

        #endregion

        #region CONSTRUCTOR

        public ProfileManagerPageObjects()
            : base(PageUrlEnum.ProfileManager.GetStringValue())
        {
        }

        #endregion

        #region PAGEOBJECTS PROPERTIES

        //      [FindsBy(How = How.ClassName, Using = CloseButtonClassName)]
        //      public ImageButton CloseButton;

        //      [FindsBy(How = How.Id, Using = ProfileTabId)]LeftArrowButton  //      public Link ProfileTabLink;

        //      [FindsBy(How = How.Id, Using = PrivilegesTabId)]
        //      public Link PrivilegesTabLink;

        //      [FindsBy(How = How.Id, Using = SecurityTabId)]
        //      public Link SecurityTabLink;

        //   [FindsBy(How = How.Id, Using = RestrictionsTabId)]
        //   public Link RestrictionsTabLink;

        //[FindsBy(How = How.Id, Using = NewPasswordTextBoxId)]
        //      public InputButton NewPasswordTextBox;

        //      [FindsBy(How = How.Id, Using = ConfirmNewPasswordTextBoxId)]
        //      public InputButton ConfirmNewPasswordTextBox;

        //      [FindsBy(How = How.Id, Using = SecurityAnswer1TextBoxId)]
        //      public InputButton SecurityAnswer1TextBox;

        //      [FindsBy(How = How.Id, Using = SecurityAnswer2TextBoxId)]
        //      public InputButton SecurityAnswer2TextBox;

        //      [FindsBy(How = How.Id, Using = SaveButtonId)]
        //      public CustomButton SaveButton;

        //      [FindsBy(How = How.Id, Using = CancelButtonId)]
        //      public CustomButton CancelButton;

        //      [FindsBy(How = How.Id, Using = ErrorModalPopupId)]
        //      public CustomButton ErrorModalPopup;

        //      [FindsBy(How = How.XPath, Using = ErrorMessageXPath)]
        //      public TextLabel ErrorMessage;

        //      [FindsBy(How = How.XPath, Using = LeftArrowButtonXPath)]
        //      public ImageButton LeftArrowButton;

        //   [FindsBy(How = How.XPath, Using = LeftArrowButtonInRestrictionXPath)]
        //   public ImageButton LeftArrowButtonInRestriction;

        //[FindsBy(How = How.XPath, Using = RightArrowInRestrictionButtonXPath)]
        //   public ImageButton RightArrowButtonInRestriction;

        //   [FindsBy(How = How.XPath, Using = AllLeftArrowButtonXPath)]
        //   public ImageButton AllLeftArrowButton;

        //   [FindsBy(How = How.XPath, Using = AllRightArrowButtonXPath)]
        //   public ImageButton AllRightArrowButton;

        //[FindsBy(How = How.Id, Using = ShowAnswer1Id)]
        //      public TextLabel ShowAnswer1;

        //      [FindsBy(How = How.Id, Using = ShowAnswer2Id)]
        //      public TextLabel ShowAnswer2;

        //      [FindsBy(How = How.Id, Using = HiddenAnswer1Id)]
        //      public TextLabel HiddenAnswer1;

        //      [FindsBy(How = How.Id, Using = HiddenAnswer2Id)]
        //      public TextLabel HiddenAnswer2;

        //      [FindsBy(How = How.Id, Using = FirstNamebyId)]
        //      public TextLabel FirstName;
        //      [FindsBy(How = How.Id, Using = LastNamebyId)]
        //      public TextLabel LastName;


        //      [FindsBy(How = How.CssSelector, Using = RightArrowButtonCssSelector)]
        //      public ImageButton RightArrowButton;

        //      [FindsBy(How = How.Id, Using = AutoDisplayPatientClaimHistoryId)]
        //      public CheckBox AutoDisplayPatientClaimHistory;

        #endregion
    }
}

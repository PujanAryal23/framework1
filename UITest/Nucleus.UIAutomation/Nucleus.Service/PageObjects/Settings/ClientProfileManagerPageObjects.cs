using System.Resources;
using Nucleus.Service.PageObjects.Default;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Utils;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Elements;

namespace Nucleus.Service.PageObjects.Settings
{
    public class ClientProfileManagerPageObjects : DefaultPageObjects
    {
        #region PRIVATE/PUBLIC FIELDS

        #region GenericTemplate

        public const string TextValueByLabelXPathTemplate =
            "//label[starts-with(normalize-space(),'{0}')]/following::span[1]/input[1]";

        #endregion

        public const string DentalClaimInsightCheckBoxId =
            "ctl00_MainContentPlaceHolder_ProductListBox_i1_ProductCheckbox";
        public const string AllowAddSwitchFlagsOnClaimActionDropDownId = "ddlAddSwitchFlags_Input";
        public const string AllowAddSwitchFlagsOnClaimActionDropDownListCssLocator = "div#ddlAddSwitchFlags_DropDown li";
        public const string AllowSwitchFlagsOnAppealActionId = "ctl00_MainContentPlaceHolder_chbAppealSwitch";
        public const string YesAutoGenerateAppealEmailCssSelector =
            "input#ctl00_MainContentPlaceHolder_AutoGenerateAppealEmail_Yes";
        public const string NoAutoGenerateAppealEmailCssSelector =
            "input#ctl00_MainContentPlaceHolder_AutoGenerateAppealEmail_No";
        public const string AppealEmailCCCssLocator = "textarea#ctl00_MainContentPlaceHolder_AppealEmailTextBox";
        public const string AllowBillableAppealsCssTemplate =
            "div.billableAppeals>div:nth-of-type(1)>input:nth-of-type({0})";
        private const string PrivilegesTabId = "ctl00_MainContentPlaceHolder_PrivilegesTab";
        private const string SecurityTabId = "SecurityLI";
        private const string NewPasswordTextBoxId = "ctl00_MainContentPlaceHolder_NewPasswordTxt";
        private const string ConfirmNewPasswordTextBoxId = "ctl00_MainContentPlaceHolder_ConfirmNewPasswordTxt";
        private const string SaveButtonId = "ctl00_MainContentPlaceHolder_SaveBtn";
        private const string ErrorModalPopupId = "RadWindowWrapper_ctl00_MainContentPlaceHolder_PopErrorsWindow";
        private const string ErrorMessageXPath = "//span[@id='ctl00_MainContentPlaceHolder_ErrorLabel']";
        public const string PciWorkListInputCssLocator = "li[authorityname=PCI_WORK_LIST] > span > div > table > tbody > tr > td > input";
        public const string FciWorkListInputCssLocator = "li[authorityname=FCI_WORK_LIST] > span > div > table > tbody > tr > td > input";
        private const string PasswordDurationInputBoxId = "ctl00_MainContentPlaceHolder_PasswordDurationBox";
        private const string SecurityTabSaveButtonId = "ctl00_MainContentPlaceHolder_SaveButton";
        private const string ProductSettingsTabId = "ProductLI";
        private const string GeneralInformationTabId = "GeneralLI";
        private const string InteropSettingTabId = "InteropLI";

        private const string EnableQuickDeleteXpath = "//div[@class='billableAppeals']/div[3]";
        public const string PhiAccessibilityDropDownSelectorCssLocator= "div#ctl00_MainContentPlaceHolder_PHIAccessibility table";
        public const string PhiAccessibilityDropdownValuesXPathTemplate = "//div[contains(@id,'ctl00_MainContentPlaceHolder_PHIAccessibility_DropDown')]/div/ul/li[text()='{0}']";

        public const string PhiAccesibilitySelectedOptionValueId =
            "ctl00_MainContentPlaceHolder_PHIAccessibility_ClientState";
        private const string EnableQuickDeleteYesOptionId =
            "ctl00_MainContentPlaceHolder_AllowClientUserQuickDelete_Yes";
        private const string EnableQuickDeleteNoOptionId = "ctl00_MainContentPlaceHolder_AllowClientUserQuickDelete_No";


        public const string AlertIconXPath =
            "//span[@id='ctl00_MainContentPlaceHolder_RangeValidator1']";
        public const string AlertIcon2XPath =
            "//span[@id='ctl00_MainContentPlaceHolder_RequiredFieldValidator1']";

        public const string AlertMessageXPath = "//div[@class='RadToolTip RadToolTip_Nucleus rtVisibleCallout ' and contains(@style,'visibility: visible')]";

       
        private const string ModalCloseButtonId = "nucleus_modal_close";

        private const string DisplayExternalDocumentIDId = "ctl00_MainContentPlaceHolder_GeneralInfoCheckboxes_15";

        public const string CotivitiUploadAppealDocumentId =
            "ctl00_MainContentPlaceHolder_AllowVHuploadsappealdocuments";

        public const string ClientProcessAppealsiwthCotivitiHealthId =
            "ctl00_MainContentPlaceHolder_GeneralInfoCheckboxes_14";

        public const string AllowClientUserstoModifyAutoReviewedFlagsXpath = "//table[@id='ctl00_MainContentPlaceHolder_GeneralInfoCheckboxes']/tbody/tr[12]/td/span/input";

        public const string ClientUsesClaimLogicsXpath =
            // "//table[@id='ctl00_MainContentPlaceHolder_GeneralInfoCheckboxes']/tbody/tr[6]/td/span/input";
            "//label[text()[contains(.,'Client Uses Claim Logics')]]/../input";

        public const string ClientCodeById = "ctl00_MainContentPlaceHolder_ClientCodeTextBox";
        public const string CancelButtonById = "ctl00_MainContentPlaceHolder_CancelButton";
        public const string CloseClientAppealsId = "ctl00_MainContentPlaceHolder_CloseClientAppeals";
        public const string ScoutCaseTrackerXpath = "//label[text()[contains(.,'Scout Case Tracker')]]/../input";
        public const string EnableProviderFlaggingId = "ctl00_MainContentPlaceHolder_ProviderFlaggingEnabledCheckbox";
        public const string ProviderFlaggingTextId = "ctl00_MainContentPlaceHolder_ProviderFlaggingTitleTextBox";

        public const string ClientInformationDropdownIDValueXpathTemplate =
            "//label[contains(normalize-space(),'{0}')]/..//input[contains(@class,'rcbInput')]";

        public const string BackButtonXpathSelector = "//a[@id='ctl00_MainContentPlaceHolder_ListButton']";

        public const string EnableIpFilterID = "ctl00_MainContentPlaceHolder_ChbIpFilter";
        public const string IpFilterTextboxId = "ctl00_MainContentPlaceHolder_IpTextBox";
        public const string CIDRTextboxId = "ctl00_MainContentPlaceHolder_CidrTextBox";

        public const string IPFilterValidationId = "ctl00_MainContentPlaceHolder_RequiredFieldValidator_IpTextBox";
        public const string IpFilterRegexValidationCssSelector = "span#ctl00_MainContentPlaceHolder_RequiredFieldValidator_IpTextBox+span";
        public const string IpFilterCustomValidationId = "ctl00_MainContentPlaceHolder_CustomValidatorIp";


        public const string IPFilterTextBoxWrapperId = "ctl00_MainContentPlaceHolder_IpTextBox_wrapper";
        public const string CotivitiUserIpTextboxId = "ctl00_MainContentPlaceHolder_VerscendIpTextBox";
        public const string CotivitiIpFilterValidationId =
            "ctl00_MainContentPlaceHolder_RequiredFieldValidator_VerscendIpTextBox";

        public const string CotivitiIpFilterRegexValidationCssSelector=
            "span#ctl00_MainContentPlaceHolder_RequiredFieldValidator_VerscendIpTextBox+span";//ctl00_MainContentPlaceHolder_CustomValidatorCotivitiIp

        public const string CotivitiIpFilterCustomValidationId =
            "ctl00_MainContentPlaceHolder_CustomValidatorVerscendIp";

        public const string RetrySoftMatchId = "ctl00_MainContentPlaceHolder_Retrysoftmatchafter";
        public const string FailDLQAfterId = "ctl00_MainContentPlaceHolder_FailDLQafter";

        public const string RetrysoftmatchafterRangeValidatorId =
            "ctl00_MainContentPlaceHolder_RetrysoftmatchafterRangeValidator";
        public const string RetrySoftMatchRequiredId = "ctl00_MainContentPlaceHolder_RetrysoftmatchafterRequired";
        public const string FailDLQafterCompareValidatorId = "ctl00_MainContentPlaceHolder_FaildlqafterCompareValidator";

        public const string TurnaroundTimesByProduct =
            "//li[contains(@product,'{0}')]/span/span[position()>1]/input[1]";
        #endregion

        #region PROTECTED PROPERTIES

        public override string PageTitle
        {
            get { return PageTitleEnum.ClientProfileManager.GetStringValue(); }
        }

     

        #endregion

        #region CONSTRUCTOR

        public ClientProfileManagerPageObjects()
            : base(PageUrlEnum.ClientProfileManager.GetStringValue())
        {
        }

        #endregion

        #region PAGEOBJECTS PROPERTIES
        [FindsBy(How = How.Id, Using = PasswordDurationInputBoxId)]
        public Link PasswordDurationInputBox;

         [FindsBy(How = How.Id, Using = ModalCloseButtonId)]
        public Link ModalCloseButton;

        

        [FindsBy(How = How.Id, Using = SecurityTabSaveButtonId)]
        public Link SecurityTabSaveButton;
        
        [FindsBy(How = How.Id, Using = PrivilegesTabId)]
        public Link PrivilegesTabLink;

        [FindsBy(How = How.Id, Using = SecurityTabId)]
        public Link SecurityTabLink;

        [FindsBy(How = How.Id, Using = NewPasswordTextBoxId)]
        public InputButton NewPasswordTextBox;

        [FindsBy(How = How.Id, Using = ConfirmNewPasswordTextBoxId)]
        public InputButton ConfirmNewPasswordTextBox;

        [FindsBy(How = How.Id, Using = SaveButtonId)]
        public CustomButton SaveButton;

        [FindsBy(How = How.Id, Using = ErrorModalPopupId)]
        public CustomButton ErrorModalPopup;

        [FindsBy(How = How.XPath, Using = ErrorMessageXPath)]
        public TextLabel ErrorMessage;

        [FindsBy(How = How.Id, Using = ProductSettingsTabId)]
        public Link ProductSettingsTabLink;

        [FindsBy(How = How.XPath, Using = EnableQuickDeleteXpath)]
        public Div EnableQuickDeleteRadioButton;

        [FindsBy(How = How.Id, Using = EnableQuickDeleteYesOptionId)]
        public RadioButton GetEnableQuickDeleteDefaultOption;

        [FindsBy(How = How.Id, Using = EnableQuickDeleteYesOptionId)]
        public RadioButton EnableQuickDeleteYesOption;

        [FindsBy(How = How.Id, Using = EnableQuickDeleteNoOptionId)]
        public RadioButton EnableQuickDeleteNoOption;

        [FindsBy(How = How.Id, Using = GeneralInformationTabId)]
        public Link GeneralInformationTabLink;

        [FindsBy(How = How.Id, Using = InteropSettingTabId)]
        public Link InteropSettingTabLink;

        [FindsBy(How = How.Id, Using = DisplayExternalDocumentIDId)]
        public CheckBox DisplayExternalDocumentIDCheckBox;

        [FindsBy(How = How.Id, Using = CotivitiUploadAppealDocumentId)]
        public CheckBox CotivitiUploadAppealDocumentCheckBox;

        [FindsBy(How = How.Id, Using = AllowSwitchFlagsOnAppealActionId)]
        public CheckBox AllowSwitchFlagsOnAppealActionCheckBox;

        [FindsBy(How = How.Id, Using = ClientProcessAppealsiwthCotivitiHealthId)]
        public CheckBox ClientProcessAppealsiwthCotivitiHealthCheckBox;

        [FindsBy(How = How.XPath, Using = AllowClientUserstoModifyAutoReviewedFlagsXpath)]
        public CheckBox AllowClientUserstoModifyAutoReviewedFlags;

        [FindsBy(How = How.XPath, Using = ClientUsesClaimLogicsXpath)]
        public CheckBox ClientUsesClaimLogics;

        [FindsBy(How = How.Id, Using = ClientCodeById)]
        public CheckBox ClientCodeBox;

        [FindsBy(How = How.Id, Using = CancelButtonById)]
        public CheckBox CancelButton;



        [FindsBy(How = How.Id, Using = CloseClientAppealsId)]
        public CheckBox CloseClientAppeals;

        [FindsBy(How = How.XPath, Using = ScoutCaseTrackerXpath)]
        public CheckBox ScoutCaseTracker;

        [FindsBy(How = How.Id, Using = EnableProviderFlaggingId)]
        public CheckBox EnableProviderFlagging;

        [FindsBy(How = How.Id, Using = ProviderFlaggingTextId)]
        public InputButton ProviderFlaggingText;

        [FindsBy(How = How.Id, Using = EnableIpFilterID)]
        public CheckBox EnableIpFilterCheckbox;

        [FindsBy(How = How.Id, Using = IpFilterTextboxId)]
        public TextField IpFilterTextbox;

        [FindsBy(How = How.Id, Using = CIDRTextboxId)]
        public TextField CIDRTextbox;

        [FindsBy(How = How.Id, Using = CotivitiUserIpTextboxId)]
        public TextField CotivitiIpTextBox;

        [FindsBy(How = How.Id, Using = RetrySoftMatchId)]
        public InputButton RetrySoftMatchTextbox;
        
        [FindsBy(How = How.Id, Using = FailDLQAfterId)]
        public InputButton FailDLQAfterIdTextbox;

        [FindsBy(How = How.Id, Using = RetrysoftmatchafterRangeValidatorId)]
        public ImageButton RetrysoftmatchafterRangeValidator;

        [FindsBy(How = How.Id, Using = FailDLQafterCompareValidatorId)]
        public ImageButton FailDLQafterCompareValidator;

        [FindsBy(How = How.Id, Using = DentalClaimInsightCheckBoxId)]
        public CheckBox DentalClaimInsightCheckBox;
        
        public static string OptionXpath = "//label[text()='{0}']/../input";

        public static string OptionParentXpath = "//label[text()='{0}']/..";

        #endregion
    }
}

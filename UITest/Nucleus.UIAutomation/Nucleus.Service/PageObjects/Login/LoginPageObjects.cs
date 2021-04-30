using System.Runtime.Remoting.Messaging;
using Nucleus.Service.PageObjects.Default;
using Nucleus.Service.Support.Enum;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Base;
using UIAutomation.Framework.Elements;
using UIAutomation.Framework.Utils;

namespace Nucleus.Service.PageObjects.Login
{
    public class LoginPageObjects : NewDefaultPageObjects//NewPageBase
    {
        #region CONSTANT

        #region ID

        public const string UserIdBoxId = "username";
        public const string PasswordBoxId = "password";
        public const string LoginButtonId = "login_button";
        public const string ForgotPasswordLinkId = "forgot_password_link";
        public const string LoginPrivacyWrapperCssLocator = "section.login_helpers.secure_disclaimer";
        public const string LoginErrorCssSelector = "ul#login_validation";
        public const string LoadingImageXPath = "//div[@id='loginButtonWrapper']/img";
        public const string ForgotPasswordMessageCssSelector = "li#forgot_password_success";
        public const string BrowserSupportNotificationCloseButtonCssSelector = "section:not([style*='none'])>div#work_button";
        public const string browsesrSupportNotificationMessage = "section#browser_support_notification p.support_notice";
        public const string HelpDeskNoticeId = "li#help_desk_notice";
        public const string PasswordResetSuccessfulIcon = "li#forgot_password_success>span";
        public const string UserNameValidationId = "li#validation_username";
        public const string LoginValidationNoticeId = "ul#login_validation>li#login_validation_notice";
        public const string LoginValidationMessageId = "ul#login_validation>li#login_validation_message";
        public const string OktaLoginvalidationByCss = "li#login_validation_notice";

        #endregion

        public const string UsernameRequiredValidationIconCssSelector = "li#validation_username>span";
        public const string EmailNoticeBlockCssSelector = "ul.email_notice li#forgot_password_success";
        public const string OktaLoginUrlInNucleus = "li#login_validation_notice a";
        public const string OktaHeader = ".okta-sign-in-header";

        #region Complete Your Basic Profile

        public const string CompleteYourBasicProfileFormXPath = "//label[contains(text(),'Complete Your Basic Profile')]/../..";
        public const string CompleteYourBasicProfileStepsCssSelector = "div.steps g:has(text:contains({0}))";
        public const string StepWiseFormCssLocator = "div.form:has(div.form_header:has(label:contains({0})))";
        public const string InputFieldLabelsCssSelector = "div.modern_ui:has(div.steps:has(svg:has(g.current))) div.form:not(.hidden) div.form_row label";
        public const string InputFieldsCssSelector = "div.modern_ui:has(div.steps:has(svg:has(g.current))) div.form:not(.hidden) div.form_row input";
        public const string InputFieldByLabelCssSelectors =
            "div.modern_ui:has(div.steps:has(svg:has(g.current))) div.form:not(.hidden)>div.form_row> div:has(label:contains({0}))>input, div.form:not(.hidden)>div.form_row> div>li:has(label:contains({0}))>input";

        public const string PasswordInputFieldCssSelectorTemplate = "div.modern_ui:has(div.steps:has(svg:has(g.current))) div.form:not(.hidden)>div.password>div.form_row:nth-of-type({0}) div:has(label:contains(Password)) input";
        public const string DropDownInputFieldXPathTemplate =
            "//div[contains(@class,'form_row')]//div[contains(@class,'select_component')]/label[text()='{0}']/following-sibling::section//input";
        public const string DropDownInputListByLabelXPathTemplate = "//label[text()='{0}']/../section//ul//li";
        public const string DropDownInputListValueByLabelAndValueXPathTemplate = "//label[text()='{0}']/../section//ul//li[text()='{1}']";
        public const string AnswerInputField = "div.modern_ui:has(div.steps:has(svg:has(g.current))) div.form:not(.hidden)>div.form_row:nth-of-type({0})>div:has(label:contains(Answer))>input";
        public const string AnswerAndPasswordInputFieldCssSelectorTemplate = "div.modern_ui:has(div.steps:has(svg:has(g.current))) div.form:not(.hidden) div.form_row:nth-of-type({0})>div:has(label:contains({1}))>input";
        public const string NextButtonCssLocator = "div.wizard_button.main_button.right";
        public const string PreviousButtonCssLocator = "div.wizard_button.main_button.left";
        public const string DropdownInputByLabelCssSelector = "div.select_component:has(label:contains({0}))>section>div>input";
        public const string PasswordDescriptionCssSelector = "div.password_desc>p";
        public const string FormHeaderCssSelectorTemplate = "label:contains({0})";
        public const string WelcomeNoteCssSelectorTemplate = "div:has(label:contains({0}))>p";
        public const string RemeberMeCheckboxCssSelector = "div.remember_me>input[type = 'checkbox']";

        #endregion

        #region VerifyEmail

        public const string VerifyEmailFormCssSelector = "div.complete_profile.verify_email>div.content";

        public const string VerifyEmailHeaderCssSelector =
            "label:contains(Verify Your Email Address)"; 

        public const string VerifyEmailMessageCssSelector =
            "div.complete_profile.verify_email>div.content>div.form div.form_row>p"; //innerText

        public const string VerifyEmailCurrentEmailLabelCssSelector = "label:contains(Current Email Address:)";

        public const string VerifyEmailCurrentEmailTextBoxCssSelector =
            "div.complete_profile.verify_email>div.content>div.form div.form_row div>input"; //value

        public const string VerifyEmailAddressButton = "div.complete_profile.verify_email div.footer>div.wizard_button";
        #endregion
        #endregion

        #region PROTECTED PROPERTIES

        public override string PageTitle
        {
            get { return PageTitleEnum.Login.GetStringValue(); }
        }

        #endregion

        #region PAGEOBJECT PROPERTIES

        #region ID

        //[FindsBy(How = How.Id, Using = UserIdBoxId)]
        //public TextField UserIdBox;

        //[FindsBy(How = How.Id, Using = PasswordBoxId)]
        //public TextField PasswordBox;

        //[FindsBy(How = How.Id, Using = LoginButtonId)]
        //public ImageButton LoginButton;

        //[FindsBy(How = How.CssSelector, Using = LoginPrivacyWrapperCssLocator)] 
        //public TextLabel LoginPrivacyWrapperTextLabel;

        //[FindsBy(How = How.CssSelector, Using = LoginErrorCssSelector)] 
        //public TextLabel LoginErrorTextLabel;

        //[FindsBy(How = How.Id, Using = ForgotPasswordLinkId)]
        //public Link ForgotPasswordLink;

        //[FindsBy(How = How.CssSelector, Using = ForgotPasswordMessageCssSelector)]
        //public TextLabel ForgotPasswordMessageTextLabel;

        #endregion

        #endregion
        
        #region CONSTRUCTORS

        public LoginPageObjects()
            : base()
        {
        }

        #endregion
    }
}


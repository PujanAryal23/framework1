using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using Nucleus.Service.Data;
using Nucleus.Service.PageServices.Base.Default;
using Nucleus.Service.PageServices.Claim;
using Nucleus.Service.PageServices.Login;
using Nucleus.Service.PageServices.Settings.User;
using Nucleus.Service.Support.Common.Constants;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Environment;
using Nucleus.UIAutomation.TestSuites.Base;
using NUnit.Framework;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Data.AutomationData;
using UIAutomation.Framework.Utils;

namespace Nucleus.UIAutomation.TestSuites.TestCases
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class Login 
    {

        /*#region PRIVATE FIELDS

        private LoginPage _login;
        private const int allowedLoginAttempt = 5;
        private int loginCount = 0;
        private ClaimSearchPage _claimSearch;
        private MyProfilePage _myProfilePage;
        #endregion

        #region OVERRIDE METHODS
        protected override void ClassInit()
        {
            try
            {
                base.ClassInit();
                _login = QuickLaunch.Logout();
                QuickLaunch.WaitForStaticTime(5000);
            }
            catch (Exception)
            {
                if (StartFlow != null)
                    StartFlow.Dispose();
                throw;
            }
        }
        #endregion

        #region PROTECTED PROPERTIES

        protected override string FullyQualifiedClassName
        {
            get
            {
                return GetType().FullName;
            }
        }

        protected override void TestCleanUp()
        {
            base.TestCleanUp();
            if(!_login.IsUserIdBoxPresent())
                _login = QuickLaunch.Logout();

        }

        protected override void ClassCleanUp()
        {
            StringFormatter.PrintMessage("Class cleanup");
            StartFlow.Dispose();
        }

        #endregion*/

        #region PROTECTED PROPERTIES
        protected string FullyQualifiedClassName
        {
            get
            {
                return GetType().FullName;
            }
        }
        #endregion

        #region TEST SUITES

        [NonParallelizable]
        [Test, Category("OnDemand")] //CAR-2997(CAR-2991)
        public void Verify_email_verification_message()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(UserLoginIndex:97))
            {
                LoginPage _login;
                _login = automatedBase.Login;

                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestName);
                var verifyEmailAddressMessage = paramLists["VerifyEmailAddressMessage"];
                var emailAddress = paramLists["EmailAddress"].Split(',').ToList();
                var verifyEmailAddressButtonText = paramLists["VerifyEmailAddressButtonText"];
                try
                {
                    _login.UpdateFlagsValue("0", automatedBase.EnvironmentManager.HciFirstLoginUser);
                    _login.UpdateEmailVerificationFromDb("F", automatedBase.EnvironmentManager.HciFirstLoginUser);
                    _login.LoginAsFirstLoginUser();
                    _login.WaitForVerifyEmailFormToLoad();

                    StringFormatter.PrintMessage("Specification verification");
                    _login.IsVerifyEmailAddressFormPresent().ShouldBeTrue("Is Verify Email Address form present?");
                    _login.IsVerifyEmailHeaderPresent().ShouldBeTrue("Is Verify your Email Address header present?");
                    _login.GetVerifyEmailMessage().Replace("  ", " ")
                        .ShouldBeEqual(verifyEmailAddressMessage.Replace("  ", " "), "Verify email message should match");
                    _login.IsVerifyEmailCurrentEmailAddressLabelPresent().ShouldBeTrue("Is Current Email Address label present?");
                    _login.IsVerifyEmailCurrentEmailTextBoxPresent().ShouldBeTrue("Is Email box present?");
                    _login.GetVerifyEmailCurrentEmailValue()
                        .ShouldBeEqual(emailAddress[0], "Default email address should match");
                    _login.IsVerifyEmailButtonPresent().ShouldBeTrue("Is Verify Email button present?");
                    _login.GetVerifyEmailButtonText()
                        .ShouldBeEqual(verifyEmailAddressButtonText, "Button text should match");

                    StringFormatter.PrintMessage("Verification by empty email address");
                    _login.SetCurrentEmail("");
                    _login.ClickVerifyEmailButton();
                    _login.WaitToLoadPageErrorPopupModal();
                    _login.GetPageErrorMessage().ShouldBeEqual("The email address is invalid.",
                        "Error message should match for empty email address");
                    _login.ClosePageError();

                    StringFormatter.PrintMessage("Verification with existing email address");
                    _login.SetCurrentEmail(emailAddress[1]);
                    var previousEmailAddress = _login.GetVerifyEmailCurrentEmailValue();
                    _login.ClickVerifyEmailButton();
                    _login.WaitToLoadPageErrorPopupModal();
                    _login.GetPageErrorMessage().ShouldBeEqual(
                        "This email address is already associated with another Nucleus account. If you feel this is an error, please contact Client Services for assistance.",
                        "Error message for redundant email address should match");
                    _login.ClosePageError();

                    StringFormatter.PrintMessage("Verification if user logs out before completing info");
                    _login.SwitchTabToLogInAsFirstLoginUser();
                    _login.WaitForVerifyEmailFormToLoad();
                    _login.CloseTab(0, 0);
                    _login.GetVerifyEmailCurrentEmailValue().ShouldNotBeEqual(previousEmailAddress, "Email address should not match on logging out before completing the info");

                    StringFormatter.PrintMessage("Verifying email address");
                    _login.ClickVerifyEmailButton();
                    _login.IsVerifyEmailAddressFormPresent().ShouldBeFalse("Is Verify Email Address form present?");

                    StringFormatter.PrintMessage("Verification that once user has completed the process, user will not be shown the form");
                    automatedBase.CurrentPage.Logout().LoginAsFirstLoginUser();
                    _login.IsVerifyEmailAddressFormPresent().ShouldBeFalse("Verify Email form should not be present");
                }

                finally
                {
                    _login.UpdateEmailVerificationFromDb("T", automatedBase.EnvironmentManager.HciFirstLoginUser);
                }
            }
        }

        [Test, Category("SmokeTestDeployment"), Category("Navigation3")] //TANT - 259
        public void Verify_login_logout()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(UserLoginIndex: 97))
            {
                LoginPage _login;
                _login = automatedBase.Login;

                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestName);
                var userValidation = paramLists["UserNameValidation"];
                var helpDeskNotice = paramLists["HelpNotice"];
                var loginValidationNotice = paramLists["LoginValidationNotice"];
                var loginValidationMessage = paramLists["LoginValidationMessage"];
                var passwordResetSuccessfulMessage = paramLists["PasswordResetSuccessfulMessage"];

                automatedBase.CurrentPage = _login.LoginAsHciAdminUser();
                automatedBase.CurrentPage.CurrentPageUrl.ShouldBeEqual(
                    automatedBase.EnvironmentManager.ApplicationUrl + PageUrlEnum.QuickLaunch.GetStringValue(),
                    "Url should match");
                automatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.QuickLaunch.GetStringValue(),
                    "Upon login, user should be navigated to Nucleus home page");
                automatedBase.CurrentPage.Logout();
                _login.CurrentPageUrl.ShouldBeEqual(automatedBase.EnvironmentManager.ApplicationUrl,
                    "Logout should navigate the user to application login page");
                _login.PageTitle.ShouldBeEqual(PageTitleEnum.Logout.GetStringValue(), "Page title should match");
                _login.IsUserIdBoxPresent().ShouldBeTrue("Is User id box present?");
                _login.IsPasswordBoxPresent().ShouldBeTrue("Is Password box present?");
                _login.IsForgotPasswordLinkPresent().ShouldBeTrue("Is Forgot Password link present?");
                _login.IsSignInButtonPresent().ShouldBeTrue("Is Sign In button present?");
                _login.IsRememberMeCheckboxPresent().ShouldBeTrue("Is Remember Me checkbox present?");
                _login.ClickForgotPasswordLink(false);
                _login.IsUserNameRequiredValidationIconPresent()
                    .ShouldBeTrue("Is User name required validation icon present?");
                _login.GetUserNameValidation().ShouldBeEqual(userValidation, "User name is required");
                _login.GetHelpDeskNotice().ShouldBeEqual(helpDeskNotice, "Help desk information should match");
                _login.SetUserName(automatedBase.EnvironmentManager.HciAdminUsername);
                _login.ClickForgotPasswordLink();
                _login.IsPasswordResetSuccessfulIconPresent()
                    .ShouldBeTrue("Is Password reset successful icon present?");
                _login.GetPasswordResetMessage().Replace("\r\n", " ")
                    .ShouldBeEqual(passwordResetSuccessfulMessage, "Password reset successful message should match");
                _login.ClickLoginButton();
                _login.GetLoginValidationNotice().ShouldBeEqual(loginValidationNotice,
                    "Login Validation notice should be shown");
                _login.GetLoginValidationMessage()
                    .ShouldBeEqual(loginValidationMessage, "Login validation Message should match");
                automatedBase.CurrentPage.ClickOnBrowserBackButton();
                _login.LoginAsHciAdminUser();
                automatedBase.CurrentPage.WaitForPageToLoad();
                automatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.QuickLaunch.GetStringValue(),
                    "Page should land on Nucleus Home after login");
            }
        }


        
       

        [NonParallelizable]
        [Test]
        public void Verify_New_user_login() //CAR-2847(CAR-2420)
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(UserLoginIndex: 97))
            {
                LoginPage _login;
                _login = automatedBase.Login;
                //_login = automatedBase.QuickLaunch.Logout();
                ClaimSearchPage _claimSearch;
                MyProfilePage _myProfilePage;

                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestName);
                var userid = paramLists["UserId"];
                var flagsValue = paramLists["FlagsValue"].Split(',').ToList();
                var personalInformationLabelsList = automatedBase.DataHelper.GetMappingData(FullyQualifiedClassName, "PersonalInformationLabel").Values
                    .ToList();
                var contactInformationLabelsList = automatedBase.DataHelper.GetMappingData(FullyQualifiedClassName, "ContactInformationLabel").Values.ToList();
                var securitySettingsLabelList = automatedBase.DataHelper.GetMappingData(FullyQualifiedClassName, "SecuritySettingsLabel").Values.ToList();
                var changedPasswordList = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestName, "ChangedPasswordList",
                    "Value").Split(';').ToList();
                var questionList = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestName, "QuestionList",
                    "Value").Split(';').ToList();
                var answerList = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestName, "AnswerList",
                    "Value").Split(';').ToList();
                var newPassword = automatedBase.DataHelper.GetSingleTestData(FullyQualifiedClassName, TestName, "NewPassword",
                    "Value");
                var invalidPassswords = paramLists["InvalidPasswords"].Split(',').ToList();
                var invalidPhoneNumbers = paramLists["InvalidPhoneNumbers"].Split(',').ToList();
                var invalidEmailAddress = paramLists["InvalidEmailAddress"].Split(',').ToList();
                var welcomeNoteLabels = paramLists["WelcomeNoteLabels"].Split(',').ToList();
                var passwordRequirementMessage = paramLists["PasswordRequirementMessage"].Split(',').ToList();
                var passwordValidationMessage = paramLists["PasswordValidationMessage"].Split(';').ToList();
                string errorMessage = paramLists["ErrorMessage"];
                string fields = paramLists["Fields"];
                string welcomeNote = paramLists["WelcomeNote"];
                string additionalNote = paramLists["AdditionalDetailNote"];
                var userIdDetails = _login.GetUsersDetailFromDatabaseByUserId(fields, userid);
                List<string> personalInformation = new List<string>(userIdDetails);
                personalInformation.RemoveRange(5, 3);
                List<string> contactInformation = new List<string>(userIdDetails);
                contactInformation.RemoveRange(0, 5);

                try
                {
                    _login.UpdateFlagsValue(flagsValue[1], userid);
                    _claimSearch = _login.LoginAsFirstLoginUser();
                    _claimSearch.WaitForPageToLoad();
                    _login.WaitToLoadCompleteYourBasicProfileForm();
                    _login.IsCompleteYourBasicProfileFormPresent()
                        .ShouldBeTrue("Is Complete your basic profile form present?");

                    StringFormatter.PrintMessage("Verification if the user does not complete the Security Settings portion of the form before the session ends, they will be required to complete this process upon their next log in");
                    _login.IsFormHeaderPresent(CompleteYourProfileStepsEnum.PersonalInformation.GetStringValue()).ShouldBeTrue("Is Personal information form present?");
                    _login.ClickNextButton();
                    _login.IsFormHeaderPresent(CompleteYourProfileStepsEnum.CompleteBasicProfileContactInformation.GetStringValue()).ShouldBeTrue("Is Contact information form present?");
                    _login.ClickNextButton();
                    _login.IsFormHeaderPresent(CompleteYourProfileStepsEnum.SecuritySettings.GetStringValue()).ShouldBeTrue("Is Security Settings form present?");
                    _login.SwitchTabToLogInAsFirstLoginUser();
                    _claimSearch.WaitForPageToLoad();
                    _login.CloseTab(0, 0);
                    _claimSearch.WaitForPageToLoad();
                    _login.WaitToLoadCompleteYourBasicProfileForm(1000);
                    _login.IsFormHeaderPresent(CompleteYourProfileStepsEnum.PersonalInformation.GetStringValue()).ShouldBeTrue("Relogging in after incomplete Security Settings step, user will be have to complete the steps again");

                    #region Personal Information

                    StringFormatter.PrintMessage("Verification of specification");
                    _login.IsStepSelected(CompleteYourProfileStepsEnum.PersonalInformation.GetStringValue()).ShouldBeTrue("Is Personal Information step selected?");
                    _login.IsStepWiseFormPresent(CompleteYourProfileStepsEnum.PersonalInformation.GetStringValue())
                        .ShouldBeTrue("Is Personal Information form present?");
                    _login.IsNextButtonPresent().ShouldBeTrue("Is next button present?");
                    _login.GetInputFieldsValue().ShouldCollectionBeEqual(personalInformation,
                        "Initially filled values should match with database");
                    _login.GetInputFieldLabelList().ShouldCollectionBeEqual(personalInformationLabelsList, "Input labels should match");

                    StringFormatter.PrintMessage("Verification of required fields");
                    foreach (var label in personalInformationLabelsList)
                    {
                        _login.ClearCompleteYourBasicProfileInputField(label);
                    }
                    _login.ClickNextButton();

                    for (var i = 0; i < personalInformationLabelsList.Count; i++)
                    {
                        if (i == 0 || i == 3)
                            _claimSearch.IsInvalidInputPresentByLabel(personalInformationLabelsList[i]).ShouldBeFalse($"Is invalid input present for {personalInformationLabelsList[i]}");
                        else
                            _claimSearch.IsInvalidInputPresentByLabel(personalInformationLabelsList[i]).ShouldBeTrue($"Is invalid input present for {personalInformationLabelsList[i]}");
                    }

                    StringFormatter.PrintMessage("Valid input for Personal Information form validation");
                    _login.FillInputFields(personalInformationLabelsList, personalInformation);
                    _login.ClickNextButton();

                    #endregion
                    #region Contact Information

                    StringFormatter.PrintMessage("Specification verification of Contact Information");
                    _login.IsFormHeaderPresent(CompleteYourProfileStepsEnum.CompleteBasicProfileContactInformation.GetStringValue()).ShouldBeTrue("Is Contact Information form present?");
                    _login.IsStepSelected(CompleteYourProfileStepsEnum.CompleteBasicProfileContactInformation.GetStringValue()).ShouldBeTrue($"Is {CompleteYourProfileStepsEnum.CompleteBasicProfileContactInformation.GetStringValue()} step selected?");
                    _login.IsStepCompleted(CompleteYourProfileStepsEnum.PersonalInformation.GetStringValue()).ShouldBeTrue($"Is {CompleteYourProfileStepsEnum.PersonalInformation.GetStringValue()} step completed?");
                    _login.IsPreviousButtonPresent().ShouldBeTrue("Is previous button present?");
                    _login.IsNextButtonPresent().ShouldBeTrue("Is next button present?");
                    var contactInformationValues = _login.GetInputFieldsValue();
                    var trimmedPhoneNumber = contactInformationValues[0].Replace("-", string.Empty);
                    contactInformationValues.RemoveAt(0);
                    contactInformationValues.Insert(0, trimmedPhoneNumber);

                    StringFormatter.PrintMessage("Contact Information specification verification");
                    contactInformationValues.ShouldCollectionBeEqual(contactInformation, "Initially filled values should match with database");
                    _login.GetInputFieldLabelList().ShouldCollectionBeEqual(contactInformationLabelsList, "Input labels should match");

                    StringFormatter.PrintMessage("Verification of required fields");
                    foreach (var label in contactInformationLabelsList)
                    {
                        _login.ClearCompleteYourBasicProfileInputField(label);
                    }
                    _login.ClickNextButton();

                    for (var i = 0; i < contactInformationLabelsList.Count; i++)
                    {
                        if (i == 1)
                            _claimSearch.IsInvalidInputPresentByLabel(contactInformationLabelsList[i]).ShouldBeFalse($"Is invalid input present for {contactInformationLabelsList[i]}");
                        else
                            _claimSearch.IsInvalidInputPresentByLabel(contactInformationLabelsList[i]).ShouldBeTrue($"Is invalid input present for {contactInformationLabelsList[i]}");
                    }


                    StringFormatter.PrintMessage("Validation of phone number and ext input field");
                    foreach (var invalidPhoneNumber in invalidPhoneNumbers)
                    {
                        PhoneNumberAndContactNumberValidation(contactInformationLabelsList[0], invalidPhoneNumber);
                    }

                    _login.GetSideWindow.SetInputInInputFieldByLabel(contactInformationLabelsList[0], "1234567890", true);
                    _login.GetSideWindow.GetInputValueByLabel(contactInformationLabelsList[0]).ShouldBeEqual("234-567-890", "If a phone number starts with 1, it will get trimmed automatically");
                    _login.GetSideWindow.SetInputInInputFieldByLabel(contactInformationLabelsList[1], "12345678901");
                    _login.GetSideWindow.GetInputValueByLabel(contactInformationLabelsList[1])
                        .ShouldBeEqual("1234567890", "Maximum of 10 digits are allowed in the ext field");

                    StringFormatter.PrintMessage("Validation of email address input field");
                    foreach (var invalidEmail in invalidEmailAddress)
                    {
                        PhoneNumberAndContactNumberValidation(contactInformationLabelsList[2], invalidEmail);

                    }

                    StringFormatter.PrintMessage("Verification of previous button");
                    _login.ClickPreviousButton();
                    _login.IsFormHeaderPresent(CompleteYourProfileStepsEnum.PersonalInformation.GetStringValue()).ShouldBeTrue("Previous button should navigate to Personal information form");
                    _login.GetInputFieldsValue().ShouldCollectionBeEqual(personalInformation, "Previously filled values should remain intact");
                    _login.ClickNextButton();

                    StringFormatter.PrintMessage("Filling the contact information fields with proper values");
                    _login.FillInputFields(contactInformationLabelsList, contactInformation);
                    foreach (var label in contactInformationLabelsList)
                    {
                        _claimSearch.IsInvalidInputPresentByLabel(label)
                            .ShouldBeFalse($"Is invalid input present for {contactInformationLabelsList[0]}");
                    }
                    _login.ClickNextButton();

                    #endregion
                    #region Security Settings
                    StringFormatter.PrintMessage("Specification Verification");
                    _login.IsFormHeaderPresent(CompleteYourProfileStepsEnum.SecuritySettings.GetStringValue()).ShouldBeTrue("Is Security Settings form present?");
                    _login.IsFormHeaderPresent(CompleteYourProfileStepsEnum.ChangePassword.GetStringValue()).ShouldBeTrue("Is Change password section present?");
                    _login.IsStepSelected(CompleteYourProfileStepsEnum.SecuritySettings.GetStringValue()).ShouldBeTrue($"Is {CompleteYourProfileStepsEnum.SecuritySettings.GetStringValue()} step selected?");
                    _login.IsStepCompleted(CompleteYourProfileStepsEnum.CompleteBasicProfileContactInformation.GetStringValue()).ShouldBeTrue($"Is {CompleteYourProfileStepsEnum.CompleteBasicProfileContactInformation.GetStringValue()} step completed?");
                    _login.IsStepCompleted(CompleteYourProfileStepsEnum.PersonalInformation.GetStringValue()).ShouldBeTrue($"Is {CompleteYourProfileStepsEnum.PersonalInformation.GetStringValue()} step completed?");
                    _login.GetInputFieldLabelList().ShouldCollectionBeEqual(securitySettingsLabelList, "Security Settings labels should match");

                    for (int i = 0; i < 6; i++)
                    {
                        if (i == 0 || i == 2)
                            _login.IsDropDownFieldPresentByLabel(securitySettingsLabelList[i]).ShouldBeTrue($"Is {securitySettingsLabelList[i]} dropdown present?");
                        else if (i == 1 || i == 3)
                            _login.IsAnswerAndPasswordInputFieldPresentByRowAndLabel(securitySettingsLabelList[i], i == 1 ? i + 1 : i).ShouldBeTrue($"Is Answer input field for {securitySettingsLabelList[i == 1 ? 1 : 2]} dropdown present?");
                        else
                        {
                            _login.IsAnswerAndPasswordInputFieldPresentByRowAndLabel(securitySettingsLabelList[i], i == 4 ? 1 : 2).ShouldBeTrue($"Is {securitySettingsLabelList[i]} input present?");
                        }
                    }

                    _login.GetPasswordDescription().ShouldCollectionBeEqual(passwordRequirementMessage, "Password requirement message should match");
                    _login.GetAnswerAndPasswordInputByRowAndLabel(securitySettingsLabelList[1]).ShouldBeEqual(answerList[0], "Answer should match");
                    _login.GetAnswerAndPasswordInputByRowAndLabel(securitySettingsLabelList[1], 3).ShouldBeEqual(answerList[1], "Answer should match");
                    _login.IsPreviousButtonPresent().ShouldBeTrue("Is previous button present?");
                    _login.IsNextButtonPresent().ShouldBeTrue("Is next button present?");

                    StringFormatter.PrintMessage("Verification of previous button");
                    _login.ClickPreviousButton();
                    _login.IsFormHeaderPresent(CompleteYourProfileStepsEnum.CompleteBasicProfileContactInformation
                        .GetStringValue()).ShouldBeTrue("Previous button should navigate the form to previous step");
                    contactInformationValues = _login.GetInputFieldsValue();
                    trimmedPhoneNumber = contactInformationValues[0].Replace("-", string.Empty);
                    contactInformationValues.RemoveAt(0);
                    contactInformationValues.Insert(0, trimmedPhoneNumber);
                    contactInformationValues.ShouldCollectionBeEqual(contactInformation, "Initially filled values should match with database");
                    _login.ClickPreviousButton();
                    _login.IsFormHeaderPresent(CompleteYourProfileStepsEnum.PersonalInformation
                        .GetStringValue()).ShouldBeTrue("Previous button should navigate the form to previous step");
                    _login.GetInputFieldsValue().ShouldCollectionBeEqual(personalInformation, "Are previously entered values present?");
                    _login.ClickNextButton();
                    _login.ClickNextButton();
                    _login.IsFormHeaderPresent(CompleteYourProfileStepsEnum.SecuritySettings.GetStringValue()).ShouldBeTrue("Is Security Settings step present?");
                    _login.IsFormHeaderPresent(CompleteYourProfileStepsEnum.ChangePassword.GetStringValue()).ShouldBeTrue("Is Change Password form row present?");

                    StringFormatter.PrintMessage("Verification of required fields");
                    _login.ClearCompleteYourBasicProfileInputField(securitySettingsLabelList[1], true);
                    _login.ClearCompleteYourBasicProfileInputField(securitySettingsLabelList[1], true, 3);

                    _login.ClickNextButton();
                    foreach (var e in securitySettingsLabelList)
                    {
                        _claimSearch.IsInvalidInputPresentByLabel(e)
                           .ShouldBeTrue("Is Invalid input present in security settings fields");
                    }

                    StringFormatter.PrintMessage("Verification of security questions");
                    var questionListsFromDB = _login.GetAvailabeQuestionListFromDb();

                    _login.GetAvailableDropDownList(securitySettingsLabelList[0]).ShouldCollectionBeEquivalent(questionListsFromDB, "Question list should be correct");
                    _login.SelectDropDownListValueByLabel(securitySettingsLabelList[0], questionList[0]);
                    _login.ValidateAnswerInputBoxByRow(securitySettingsLabelList[1]).ShouldBeEqual(100, "Answer Text Field should allow up to 100 alphanumeric or special character values.");
                    _login.SetAnswerAndPasswordInputByRowAndLabel(answerList[0], securitySettingsLabelList[1], 2);

                    StringFormatter.PrintMessage("Same question cannot be selected twice");
                    questionListsFromDB.Remove(questionList[0]);
                    var questionListsFromUi = _login.GetAvailableDropDownList(securitySettingsLabelList[2]);
                    questionListsFromUi.ShouldCollectionBeEquivalent(questionListsFromDB, "Question list should be correct and should not contain the question selected in Security question 1");

                    _login.SelectDropDownListValueByLabel(securitySettingsLabelList[2], questionList[1]);
                    _login.ValidateAnswerInputBoxByRow(securitySettingsLabelList[1], 3).ShouldBeEqual(100, "Answer Text Field should allow up to 100 alphanumeric or special character values.");
                    _login.SetAnswerAndPasswordInputByRowAndLabel(answerList[1], securitySettingsLabelList[1], 3);

                    StringFormatter.PrintMessage("Validation for password of length less than 8 and password and confirm password values mismatch");
                    for (int i = 0; i < invalidPassswords.Count; i++)
                    {
                        PasswordValidation(passwordValidationMessage[i], invalidPassswords[i], 1, securitySettingsLabelList[4]);
                    }

                    StringFormatter.PrintMessageTitle("Verifying that the user is not allowed to input new password which is within the recent 10 last passwords");
                    Random random = new Random();
                    int n = random.Next(0, changedPasswordList.Count);

                    _login.SetAnswerAndPasswordInputByRowAndLabel(changedPasswordList[n], securitySettingsLabelList[4]);
                    _login.SetAnswerAndPasswordInputByRowAndLabel(changedPasswordList[n], securitySettingsLabelList[4], 2);
                    _login.ClickNextButton();
                    _claimSearch.WaitForWorking();
                    _claimSearch.IsPageErrorPopupModalPresent().ShouldBeTrue("Is page error popup present?");
                    _claimSearch.GetPageErrorMessage().ShouldBeEqual(errorMessage,
                        "Password should not match the previous passwords");
                    _claimSearch.ClosePageError();

                    StringFormatter.PrintMessage("Password Validation meeting all the requirements and password and confirm password values should match");
                    _login.SetAnswerAndPasswordInputByRowAndLabel(newPassword, securitySettingsLabelList[4]);
                    _claimSearch.IsInvalidInputPresentByLabel(securitySettingsLabelList[4]).ShouldBeFalse("Is invalid input validation present?");
                    _claimSearch.IsInvalidInputPresentByLabel(securitySettingsLabelList[5]).ShouldBeTrue("Is invalid input validation present?");
                    _login.SetAnswerAndPasswordInputByRowAndLabel(newPassword, securitySettingsLabelList[4], 2);
                    _claimSearch.IsInvalidInputPresentByLabel(securitySettingsLabelList[5]).ShouldBeFalse("Is invalid input validation present?");
                    _login.ClickNextButton();
                    _claimSearch.WaitForWorking();
                    #endregion

                    #region WelcomeForm
                    StringFormatter.PrintMessage("Verification of Thank you form and close button");
                    _login.IsFormHeaderPresent(welcomeNoteLabels[0]).ShouldBeTrue("Is thank you form present?");
                    _login.GetWelcomeNoteByLabel(welcomeNoteLabels[0]).ShouldBeEqual(welcomeNote, "Welcome note should match");
                    _login.IsFormHeaderPresent(welcomeNoteLabels[1]).ShouldBeTrue("Is visit my profile label present?");
                    _login.GetWelcomeNoteByLabel(welcomeNoteLabels[1]).ShouldBeEqual(additionalNote, "Additional detail note should match");
                    _login.ClickNextButton();
                    _claimSearch.WaitForWorking();

                    StringFormatter.PrintMessage("Verification that on completing Complete Your Basic Profile form user will be navigated to default landing page");
                    _claimSearch.GetPageHeader().ShouldBeEqual(PageTitleEnum.ClaimSearch.GetStringDisplayValue(),
                        "On Completion of the form user should be navigated to default landing page");
                    _login.IsCompleteYourBasicProfileFormPresent().ShouldBeFalse("Is Complete your basic profile form present?");

                    #endregion

                    #region Visit My Profile

                    automatedBase.CurrentPage.ClickOnQuickLaunch();
                    _login.UpdateFlagsValue(flagsValue[1], userid);
                    if (_login.GetLatestPasswordCountFromDb(userid) == 11)
                        _login.DeleteLatestPasswordFromDb(userid);
                    while (_login.IsCompleteYourBasicProfileFormPresent() == false)
                    {
                        automatedBase.CurrentPage.RefreshPage(waitSidebar:false);
                    }

                    _login.WaitToLoadCompleteYourBasicProfileForm();
                    _login.ClickNextButton();
                    _login.ClickNextButton();
                    _login.SelectDropDownListValueByLabel(securitySettingsLabelList[0], questionList[0]);
                    automatedBase.CurrentPage.WaitForCondition(() => !_login.IsDropDownListPresenByLabel(securitySettingsLabelList[0]), 2000);
                    _login.SelectDropDownListValueByLabel(securitySettingsLabelList[2], questionList[1]);
                    _login.SetAnswerAndPasswordInputByRowAndLabel(newPassword, securitySettingsLabelList[4]);
                    _login.SetAnswerAndPasswordInputByRowAndLabel(newPassword, securitySettingsLabelList[4], 2);
                    _login.ClickNextButton();
                    _claimSearch.WaitForWorking();
                    _myProfilePage = _login.ClickVisitMyProfile(welcomeNoteLabels[1]);
                    _myProfilePage.WaitForPageToLoad();
                    _myProfilePage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.MyProfile.GetStringValue(),
                        "VisitMyProfile link should navigate the user to My Profile Page");
                    _myProfilePage.ClickOnCancelButton();
                    _claimSearch.WaitForWorking();
                    _claimSearch.GetPageHeader().ShouldBeEqual(PageTitleEnum.ClaimSearch.GetStringDisplayValue(),
                        "Cancel lands the page to default landing page");

                    #endregion
                }
                finally
                {
                    _login.UpdateFlagsValue(flagsValue[0], userid);
                    if (_login.GetLatestPasswordCountFromDb(userid) == 11)
                        _login.DeleteLatestPasswordFromDb(userid);
                }

                void PhoneNumberAndContactNumberValidation(string label, string value)
                {
                    _login.SetCompleteYourBasicProfileInput(label, value);
                    _claimSearch.IsInvalidInputPresentByLabel(label)
                        .ShouldBeTrue("Is invalid input present for wrong email format?");
                }

                void PasswordValidation(string message, string value, int row, string label)
                {
                    StringFormatter.PrintMessage(message);
                    _login.SetAnswerAndPasswordInputByRowAndLabel(value, label, row);
                    _claimSearch.IsInvalidInputPresentByLabel(label).ShouldBeTrue("Invalid input should be present for incorrect password format");
                }
            }
        }

        [Test]
        public void Verify_security_text_appears_below_sign_in_button()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(UserLoginIndex: 97))
            {
                LoginPage _login;
                _login = automatedBase.Login;
                //_login = automatedBase.QuickLaunch.Logout();

                var TestName = new StackFrame(true).GetMethod().Name;
                _login.GetSecurityTextBelowSignIn().ShouldBeEqual(
                    "This secure site is for registered users only. If you would like to learn more, please review our terms of use",
                    "Security Text");
            }
        }

        [Test, Category("SmokeTest")]
        public void Verify_if_wrong_credential_is_entered_then_red_warning_text_is_displayed()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(UserLoginIndex: 97))
            {
                LoginPage _login;
                _login = automatedBase.Login;
                //_login = automatedBase.QuickLaunch.Logout();
                var TestName = new StackFrame(true).GetMethod().Name;
                _login.LoginWithWrongCredential();
                _login.GetLoginErrorText()
                    .ShouldBeEqual(
                        "Your login attempt has failed.\r\nPlease contact the Cotiviti Help Desk at 801.285.5910 or clientservices@cotiviti.com Monday - Friday, 6:00 A.M. - 6:00 P.M. MST",
                        "Login Error Text");
            }
        }

        [Test, Category("SmokeTest")]//US25234
        public void Verify_if_user_exceeds_the_number_of_allowed_attempts_to_login_then_their_account_will_be_locked()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(UserLoginIndex: 97))
            {
                LoginPage _login;
                _login = automatedBase.Login;
                //_login = automatedBase.QuickLaunch.Logout();
                int loginCount = 0;
                const int allowedLoginAttempt = 5;

                var TestName = new StackFrame(true).GetMethod().Name;
                automatedBase.QuickLaunch = _login.LoginTestUser(true);
                StringFormatter.PrintMessage("Login Successful !");
                _login = automatedBase.QuickLaunch.Logout();
                _login.ClickOnCloseButton();
                do
                {
                    _login.LoginTestUserWithWrongPassword();
                    loginCount++;
                } while (loginCount < allowedLoginAttempt);

                _login.LoginTestUser(false);
                _login.GetLoginErrorText()
                    .ShouldBeEqual(
                        "Your login attempt has failed.\r\nPlease contact the Cotiviti Help Desk at 801.285.5910 or clientservices@cotiviti.com Monday - Friday, 6:00 A.M. - 6:00 P.M. MST",
                        "Login Error Text");
                StringFormatter.PrintMessage(" Login Failed! Account is locked for 30 Minuates.");
            }
        }

        [Test, Category("SmokeTest")]//US25234
        public void Verify_that_user_can_unlock_their_own_account_by_using_the_forgot_your_password_process()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(UserLoginIndex: 97))
            {
                LoginPage _login;
                _login = automatedBase.Login;
                //_login = automatedBase.QuickLaunch.Logout();
                var TestName = new StackFrame(true).GetMethod().Name;
                _login.SetUserToChangePassword();
                _login.ClickForgotPasswordLink();
                _login.GetChangePasswordInstruction().ShouldBeEqual(
                    "An email has been sent to the address linked to this account.\r\nPlease follow the instructions on the email to reset your password.",
                    "password Change Instruction");
            }
        }

        [Test]//TE-992 +TE-1156
        public void Verify_SSOUser_Is_Denied_Login_From_Nucleus()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(UserLoginIndex: 97))
            {
                LoginPage _login;
                _login = automatedBase.Login;
                var TestName = new StackFrame(true).GetMethod().Name;
                var paramLists = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestName);
                var username = paramLists["UserName"];
                var password = paramLists["Password"];
                var message = paramLists["Message"].Split(';').ToList();
                var OktaUrl = paramLists["OktaLoginUrl"];
                _login.UpdateSSOSettingFOrUser(username, true);
                for ( int i=0;i<2;i++)
                {
                    
                    StringFormatter.PrintMessage(
                        "Verify behaviour when SSO user tries to login through Nucleus Direct Url");
                    _login.LoginSSOUser(username, password);
                   _login.GetOktaLoginValidationMessage().Trim().ShouldBeEqual(message[i],
                            "Error Message correct?");
                    _login.GetUserAuthenticationAudit(username).ShouldBeEqual("I", "audit added?");

                    StringFormatter.PrintMessage(
                        "Verify failed attempt count does not increase and user is not locked out if a user tries to access application more than 3 times");
                    for (int j = 0; j < 4; j++)
                    {
                        _login.LoginSSOUser(username, password);
                    }

                    _login.GetFailedLoginCountFromDatabase(username)
                        .ShouldBeEqual(0, "Failed login count increased?");

                    StringFormatter.PrintMessage("verify SSO user is not allowed to reset password ");
                    _login.SetUserToChangePassword(username);
                    _login.ClickForgotPasswordLink(true);
                    _login.GetOktaLoginValidationMessage().Trim()
                     .ShouldBeEqual(message[i],
                        "Error Message correct?");
                    _login.UpdateSSOSettingFOrUser(username);
                }
                _login.clickonOktaLinkFromLoginPage();
                _login.GetCurrentUrl().ShouldBeEqual(OktaUrl,"user Navigated to Okta?");
                _login.ClickOnBrowserBackButton();

            }
        }
        
        [Test, Category("ChromeNonCompatible")] //TE-1157
        public void Verify_Browser_Support_Notification_Message_In_FireFox()
        {
            using ( var automatedBase = new NewAutomatedBaseParallelRun(UserLoginIndex: 97))
            {
                LoginPage _login;
                _login = automatedBase.Login;
                var TestName = new StackFrame(true).GetMethod().Name;
                var paramLists = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestName);
                var message = paramLists["BrowserSupportMessage"].Split(';').ToList();
                _login.GetBrowserSupportNotificationMessage().Trim()
                    .ShouldBeEqual(message[0], "browser support message correct?");
                _login.ClickOnCloseButton();
                
            }
        }

        [Test]//TE-425
        public void Verify_login_with_return_urls()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(UserLoginIndex: 97))
            {
                LoginPage _login;
                _login = automatedBase.Login;
                //_login = automatedBase.QuickLaunch.Logout();

                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestName);

                StringFormatter.PrintMessage("Verify redirection to relative URLs for core");
                var url = _login.LoginWithReturnUrl(paramLists["RandomUrl"]);
                url.ToLower().ShouldBeEqual(
                    (automatedBase.EnvironmentManager.ApplicationUrl + PageUrlEnum.QuickLaunch.GetStringValue()).ToLower(),
                    "User should be navigated to Default Page instead of random Return Url");
                automatedBase.CurrentPage.WaitForLogoutIcon();
                automatedBase.CurrentPage.Logout();

                StringFormatter.PrintMessage("Verify redirection to relative URLs for core");
                url = _login.LoginWithReturnUrl(paramLists["CoreUrl"]);
                url.ToLower().ShouldBeEqual(
                    (automatedBase.EnvironmentManager.ApplicationUrl + PageUrlEnum.ClaimAction.GetStringValue()).ToLower(),
                    "User should able to login and redirect to default page");
            }
        }

        #endregion
    }
}

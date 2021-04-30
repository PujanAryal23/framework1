using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Nucleus.Service.Data;
using Nucleus.Service.PageObjects.Appeal;
using Nucleus.Service.PageObjects.Batch;
using Nucleus.Service.PageObjects.Login;
using Nucleus.Service.PageObjects.QuickLaunch;
using Nucleus.Service.PageServices.Base;
using Nucleus.Service.PageServices.QuickLaunch;
using Nucleus.Service.Support.Common.Constants;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Core.Navigation;
using Nucleus.Service.Support.Environment;
using Nucleus.Service.PageServices.Claim;
using Nucleus.Service.PageObjects.Claim;
using Nucleus.Service.PageObjects.Default;
using Nucleus.Service.PageObjects.Provider;
using Nucleus.Service.PageObjects.Settings.User;
using Nucleus.Service.PageServices.Appeal;
using Nucleus.Service.PageServices.Base.Default;
using Nucleus.Service.PageServices.Batch;
using Nucleus.Service.PageServices.Provider;
using Nucleus.Service.PageServices.Settings.User;
using Nucleus.Service.SqlScriptObjects.Common_SQL;
using Nucleus.Service.SqlScriptObjects.User;
using Nucleus.Service.Support.Common;
using Nucleus.Service.Support.Enum;
using OpenQA.Selenium;
using UIAutomation.Framework.Database;
using UIAutomation.Framework.Elements;
using UIAutomation.Framework.Utils;
using static System.String;
using Extensions = Nucleus.Service.Support.Utils.Extensions;


namespace Nucleus.Service.PageServices.Login
{
    public class LoginPage : NewDefaultPage //NewBasePageService
    {
        #region PRIVATE FIELDS

        private readonly LoginPageObjects _nucleusLoginPage;
        private readonly SideBarPanelSearch _sideBarPanelSearch;
        private readonly CommonSQLObjects _commonSqlObjects;
        private readonly GridViewSection _gridViewSection;
        private readonly SideWindow _sideWindow;
        private readonly NewDefaultPage _defaultPage;

        #endregion

        #region CONSRUCTOR

        public LoginPage(INewNavigator navigator, LoginPageObjects nucleusLoginPage, ISiteDriver siteDriver,
            IJavaScriptExecutors javaScriptExecutors, IEnvironmentManager environmentManager,
            IBrowserOptions browserOptions, IOracleStatementExecutor executor)
            : base(navigator, nucleusLoginPage, siteDriver, javaScriptExecutors, environmentManager, browserOptions,
                executor)
        {
            // Just for performance!
            _nucleusLoginPage = (LoginPageObjects) PageObject;
            _sideBarPanelSearch = new SideBarPanelSearch(new CalenderPage(SiteDriver), SiteDriver, JavaScriptExecutor);
            _gridViewSection = new GridViewSection(SiteDriver, JavaScriptExecutor);
            _sideWindow = new SideWindow(new CalenderPage(SiteDriver), SiteDriver, JavaScriptExecutor);
        }

        public SideWindow GetSideWindow
        {
            get { return _sideWindow; }
        }

        public SideBarPanelSearch GetSideBarPanelSearch
        {
            get { return _sideBarPanelSearch; }
        }

        public NewDefaultPage GetDefaultPage
        {
            get { return _defaultPage; }
        }

        public GridViewSection GetGridViewSection
        {
            get { return _gridViewSection; }
        }

        #endregion


        #region PUBLIC METHODS

        public bool IsPasswordBoxPresent() => SiteDriver.IsElementPresent(LoginPageObjects.PasswordBoxId, How.Id);


        public bool IsUserIdBoxPresent()
        {
            return SiteDriver.IsElementPresent(LoginPageObjects.UserIdBoxId, How.Id);
        }

        public bool IsForgotPasswordLinkPresent() =>
            SiteDriver.IsElementPresent(LoginPageObjects.ForgotPasswordLinkId, How.Id);

        public bool IsSignInButtonPresent() => SiteDriver.IsElementPresent(LoginPageObjects.LoginButtonId, How.Id);

        public bool IsRememberMeCheckboxPresent() =>
            SiteDriver.IsElementPresent(LoginPageObjects.RemeberMeCheckboxCssSelector, How.CssSelector);

        public void SetUserName(string userName)
        {
            SiteDriver.FindElement(LoginPageObjects.UserIdBoxId, How.Id).ClearElementField();
            SiteDriver.FindElement(LoginPageObjects.UserIdBoxId, How.Id).SendKeys(userName);
        }

        public void SetPassword(string password)
        {
            SiteDriver.FindElement(LoginPageObjects.PasswordBoxId, How.Id).ClearElementField();
            SiteDriver.FindElement(LoginPageObjects.PasswordBoxId, How.Id).SendKeys(password);
        }

        public void ClickLoginButton() =>
            SiteDriver.FindElement(LoginPageObjects.LoginButtonId, How.Id).Click();


        public void ClickOnCloseButton()
        {
            if (SiteDriver.IsElementPresent(LoginPageObjects.BrowserSupportNotificationCloseButtonCssSelector,
                How.CssSelector))
                SiteDriver.FindElementAndClickOnCheckBox(
                    LoginPageObjects.BrowserSupportNotificationCloseButtonCssSelector, How.CssSelector);

        }

        public string GetUserNameValidation() =>
            JavaScriptExecutor.FindElement(LoginPageObjects.UserNameValidationId).GetAttribute("innerText");

        public string GetHelpDeskNotice() =>
            JavaScriptExecutor.FindElement(LoginPageObjects.HelpDeskNoticeId).GetAttribute("innerText");

        public string GetLoginValidationNotice() =>
            JavaScriptExecutor.FindElement(LoginPageObjects.LoginValidationNoticeId).GetAttribute("innerText");

        public string GetLoginValidationMessage() =>
            JavaScriptExecutor.FindElement(LoginPageObjects.LoginValidationMessageId).GetAttribute("innerText");


        public string GetOktaLoginValidationMessage() =>
            JavaScriptExecutor.FindElement(LoginPageObjects.OktaLoginvalidationByCss).GetAttribute("innerText");

        public string GetPasswordResetMessage() =>
            JavaScriptExecutor.FindElement(LoginPageObjects.EmailNoticeBlockCssSelector).GetAttribute("innerText");

        public string GetSecurityTextBelowSignIn()
        {
            var element = SiteDriver.FindElement(LoginPageObjects.LoginPrivacyWrapperCssLocator, How.CssSelector);
            return element.Text;
        }

        public void LoginWithWrongCredential()
        {
            SiteDriver.FindElement(LoginPageObjects.UserIdBoxId, How.Id).SendKeys("wrongusername");
            SiteDriver.WaitForIe(2000);
            SiteDriver.FindElement(LoginPageObjects.PasswordBoxId, How.Id).SendKeys("wrongpassword");
            SiteDriver.WaitForIe(2000);
            SiteDriver.FindElement(LoginPageObjects.LoginButtonId, How.Id).Click();
            Console.WriteLine("Login with wrong credential: username<wrongusername> password<wrongpassword>");
        }

        public void LoginTestUserWithWrongPassword()
        {
            SiteDriver.FindElement(LoginPageObjects.UserIdBoxId, How.Id).SendKeys("test_login");
            SiteDriver.WaitForIe(2000);
            SiteDriver.FindElement(LoginPageObjects.PasswordBoxId, How.Id).SendKeys("wrongpassword");
            SiteDriver.WaitForIe(2000);
            SiteDriver.FindElement(LoginPageObjects.LoginButtonId, How.Id).Click();
            Console.WriteLine("Login with wrong password<wrongpassword>");
            SiteDriver.WaitForPageToLoad();

        }

        public void LoginSSOUser(string userId,string password)
        {
            SiteDriver.FindElement(LoginPageObjects.UserIdBoxId, How.Id).SendKeys(userId);
            SiteDriver.WaitForIe(2000);
            SiteDriver.FindElement(LoginPageObjects.PasswordBoxId, How.Id).SendKeys(password);
            SiteDriver.WaitForIe(2000);
            SiteDriver.FindElement(LoginPageObjects.LoginButtonId, How.Id).Click();
            Console.WriteLine("Login with wrong password<Test123$>");
            SiteDriver.WaitForPageToLoad();

        }

        public void SetUserToChangePassword(string userName = "userName")
        {
            SiteDriver.FindElement(LoginPageObjects.PasswordBoxId, How.Id).ClearElementField();
            SiteDriver.FindElement(LoginPageObjects.UserIdBoxId, How.Id).ClearElementField();
            SiteDriver.FindElement(LoginPageObjects.UserIdBoxId, How.Id).SendKeys(userName);
            if (!SiteDriver.FindElement(LoginPageObjects.UserIdBoxId, How.Id).Text.Equals(userName))
            {
                SiteDriver.FindElement(LoginPageObjects.UserIdBoxId, How.Id).ClearElementField();
                SiteDriver.FindElement(LoginPageObjects.UserIdBoxId, How.Id).SendKeys(userName);
            }
        }

        public string GetBrowserSupportNotificationMessage()
        {
            return SiteDriver.FindElement(LoginPageObjects.browsesrSupportNotificationMessage, How.CssSelector).Text;
        }

        public void ClickForgotPasswordLink(bool isPasswordResetSuccessful = true, bool isoktaUser = false)
        {
            SiteDriver.FindElement(LoginPageObjects.ForgotPasswordLinkId, How.Id).Click();
            if (isoktaUser)
                SiteDriver.WaitForCondition(() =>
                    SiteDriver.IsElementPresent(LoginPageObjects.OktaLoginvalidationByCss, How.CssSelector));
            else
            {
                if (isPasswordResetSuccessful)
                    SiteDriver.WaitForCondition(() =>
                        SiteDriver.IsElementPresent(LoginPageObjects.EmailNoticeBlockCssSelector, How.CssSelector));
                else
                    SiteDriver.WaitForCondition(() =>
                        SiteDriver.IsElementPresent(LoginPageObjects.UsernameRequiredValidationIconCssSelector,
                            How.CssSelector));
            }
        }
    

    public bool IsUserNameRequiredValidationIconPresent() =>
            SiteDriver.IsElementPresent(LoginPageObjects.UsernameRequiredValidationIconCssSelector, How.CssSelector);

        public bool IsPasswordResetSuccessfulIconPresent() =>
            SiteDriver.IsElementPresent(LoginPageObjects.PasswordResetSuccessfulIcon, How.CssSelector);

        public string GetChangePasswordInstruction()
        {
            SiteDriver.WaitForIe(2000);
            return SiteDriver.FindElement(LoginPageObjects.ForgotPasswordMessageCssSelector, How.CssSelector).Text;
        }

        public string GetLoginErrorText()
        {
            return SiteDriver.FindElement(LoginPageObjects.LoginErrorCssSelector, How.CssSelector).Text;
        }

      

        private void WaitForLoadingImage()
        {
            SiteDriver.WaitForCondition(() => !IsLoadingImagePresent());
        }

        public bool IsLoadingImagePresent()
        {
            return SiteDriver.IsElementPresent(LoginPageObjects.LoadingImageXPath, How.XPath);
        }

        public ClaimSearchPage LoginAsHciAdminUser8()
        {
            var newClaimSearch = Login<ClaimSearchPage>(EnvironmentManager.HciAdminUsername8, EnvironmentManager.HciAdminPassword);
            EnvironmentManager.Username = EnvironmentManager.HciAdminUsername8;
            UserType.CurrentUserType = UserType.HCIADMIN8;
            return newClaimSearch;
        }

        public QuickLaunchPage LoginAsHciAdminUser()
        {
            var quickLaunch = Login(EnvironmentManager.HciAdminUsername, EnvironmentManager.HciAdminPassword);
            EnvironmentManager.Username = EnvironmentManager.HciAdminUsername;
            UserType.CurrentUserType = UserType.HCIADMIN;
            return quickLaunch;
        }

        

        public SuspectProvidersPage LoginWithDefaultSuspectProvidersPage()
        {
            var suspectProvider = Login<SuspectProvidersPage>(EnvironmentManager.HciAdminUserWithSuspectProviderDefaultPage, EnvironmentManager.HciAdminPasswordWithSuspectProviderDefaultPage);
            EnvironmentManager.Username = EnvironmentManager.HciAdminUserWithSuspectProviderDefaultPage;
            UserType.CurrentUserType = UserType.HCIUSER;
            return suspectProvider;

        }

        public ProviderSearchPage LoginWithDefaultProviderSearchPage()
        {
            var providerSearch = Login<ProviderSearchPage>(EnvironmentManager.HciAdminUserWithProviderSearchDefaultPage, EnvironmentManager.HciAdminPasswordWithProviderSearchDefaultPage);
            EnvironmentManager.Username = EnvironmentManager.HciAdminUserWithProviderSearchDefaultPage;
            UserType.CurrentUserType = UserType.HCIUSER;
            return providerSearch;

        }



        public SuspectProvidersPage LoginClientUserWithDefaultSuspectProvidersPage()
        {
            var suspectProvider = Login<SuspectProvidersPage>(EnvironmentManager.ClientUserWithSuspectProviderDefaultPage, EnvironmentManager.ClientPasswordWithSuspectProviderDefaultPage);
            EnvironmentManager.Username = EnvironmentManager.ClientUserWithSuspectProviderDefaultPage;
            UserType.CurrentUserType = UserType.CLIENT2;
            return suspectProvider;

        }

        public ProviderSearchPage LoginClientUserWithDefaultProviderSearchPage()
        {
            var providerSearch = Login<ProviderSearchPage>(EnvironmentManager.ClientUserWithProviderSearchDefaultPage, EnvironmentManager.ClientPasswordWithProviderSearchDefaultPage);
            EnvironmentManager.Username = EnvironmentManager.ClientUserWithProviderSearchDefaultPage;
            UserType.CurrentUserType = UserType.CLIENT2;
            return providerSearch;

        }

        public QuickLaunchPage LoginAsClaimViewRestrictionUser()
        {
            var quickLaunch = Login(EnvironmentManager.HciClaimViewRestrictionUsername, EnvironmentManager.HciAdminPassword);
            EnvironmentManager.Username = EnvironmentManager.HciClaimViewRestrictionUsername;
            UserType.CurrentUserType = UserType.HCICLAIMVIEWRESTRICTION;
            return quickLaunch;
        }
        public QuickLaunchPage LoginAsHciAdminUser1()
        {
            var quickLaunch = Login(EnvironmentManager.HciAdminUsername1, EnvironmentManager.HciAdminPassword);
            EnvironmentManager.Username = EnvironmentManager.HciAdminUsername1;
            UserType.CurrentUserType = UserType.HCIADMIN1;
            return quickLaunch;
        }

        public QuickLaunchPage LoginAsHciAdminUser2()
        {
            var quickLaunch = Login(EnvironmentManager.HciAdminUsername2, EnvironmentManager.HciAdminPassword);
            EnvironmentManager.Username = EnvironmentManager.HciAdminUsername2;
            UserType.CurrentUserType = UserType.HCIADMIN2;
            return quickLaunch;
        }

        public QuickLaunchPage LoginAsHciUserForSecurityCheck()
        {
            var quickLaunch = Login(EnvironmentManager.HCIUserNameForSecurityCheck, EnvironmentManager.HCIPasswordForSecurityCheck);
            EnvironmentManager.Username = EnvironmentManager.HCIUserNameForSecurityCheck;
            UserType.CurrentUserType = UserType.HCIUserNameForSecurityCheck;
            return quickLaunch;
        }

        public QuickLaunchPage LoginAsHciAdminUser3()
        {
            var quickLaunch = Login(EnvironmentManager.HciAdminUsername3, EnvironmentManager.HciAdminPassword);
            EnvironmentManager.Username = EnvironmentManager.HciAdminUsername3;
            UserType.CurrentUserType = UserType.HCIADMIN3;
            return quickLaunch;
        }

        public QuickLaunchPage LoginAsHciAdminUser4()
        {
            var quickLaunch = Login(EnvironmentManager.HciAdminUsername4, EnvironmentManager.HciAdminPassword);
            EnvironmentManager.Username = EnvironmentManager.HciAdminUsername4;
            UserType.CurrentUserType = UserType.HCIADMIN4;
            return quickLaunch;
        }

        public QuickLaunchPage LoginAsHciAdminUserClaim5()
        {
            var quickLaunch = Login(EnvironmentManager.HciAdminUsernameClaim5, EnvironmentManager.HciAdminPassword);
            EnvironmentManager.Username = EnvironmentManager.HciAdminUsernameClaim5;
            UserType.CurrentUserType = UserType.HCIADMINCLAIM5;
            return quickLaunch;
        }

        public QuickLaunchPage LoginAsHciAdminUser5()
        {
            var quickLaunch = Login(EnvironmentManager.HciAdminUsername5, EnvironmentManager.HciAdminPassword);
            EnvironmentManager.Username = EnvironmentManager.HciAdminUsername5;
            UserType.CurrentUserType = UserType.HCIADMIN5;
            return quickLaunch;
        }

        public QuickLaunchPage LoginAsMstrUserwithManagerRole()
        {
            var quickLaunch = Login(EnvironmentManager.MstrtUserWithManageRole, EnvironmentManager.MstrtUserWithManageRolePassword);
            EnvironmentManager.Username = EnvironmentManager.MstrtUserWithManageRole;
            UserType.CurrentUserType = UserType.MstrUserwithManagerRole;
            return quickLaunch;
        }

        public QuickLaunchPage LoginAsClientMstrUserwithManagerRole()
        {
            var quickLaunch = Login(EnvironmentManager.ClientMstrtUserWithManageRole, EnvironmentManager.ClientMstrtUserWithManageRolePassword);
            EnvironmentManager.Username = EnvironmentManager.ClientMstrtUserWithManageRole;
            UserType.CurrentUserType = UserType.ClientMstrUserwithManagerRole;
            return quickLaunch;
        }

        public QuickLaunchPage LoginAsClientUser()
        {
            var quickLaunch = Login(EnvironmentManager.ClientUserName, EnvironmentManager.ClientPassword);
            EnvironmentManager.Username = EnvironmentManager.ClientUserName;
            UserType.CurrentUserType = UserType.CLIENT;
            return quickLaunch;
        }

        public QuickLaunchPage LoginAsClientUser2()
        {
            var quickLaunch = Login(EnvironmentManager.ClientUserHavingFfpEditOfPciFlagsAuthority,
                EnvironmentManager.ClientUserHavingFfpEditOfPciFlagsAuthorityPassword);
            EnvironmentManager.Username = EnvironmentManager.ClientUserHavingFfpEditOfPciFlagsAuthority;
            UserType.CurrentUserType = UserType.CLIENT2;
            return quickLaunch;
        }
   
        public QuickLaunchPage LoginAsClientUser1()
        {
            var quickLaunch = Login(EnvironmentManager.ClientUserName1, EnvironmentManager.ClientPassword);
            EnvironmentManager.Username = EnvironmentManager.ClientUserName1;
            UserType.CurrentUserType = UserType.CLIENT1;
            return quickLaunch;
        }

        public QuickLaunchPage LoginAsClientUser4()
        {
            var quickLaunch = Login(EnvironmentManager.ClientUserName4, EnvironmentManager.ClientPassword);
            EnvironmentManager.Username = EnvironmentManager.ClientUserName4;
            UserType.CurrentUserType = UserType.CLIENT;
            return quickLaunch;
        }

        public QuickLaunchPage LoginAsHciUserHavingManageAppealsReadWriteAuthority()
        {
            var quickLaunch = Login(EnvironmentManager.HciUserWithReadWriteManageAppeals,
                                    EnvironmentManager.HciIUserWithReadWriteManageAppealsPassword);
            EnvironmentManager.Username = EnvironmentManager.HciUserWithReadWriteManageAppeals;
            UserType.CurrentUserType = UserType.HCIUSER;
            return quickLaunch;
        }

        public QuickLaunchPage LoginAsClientUserWithReadWriteManageAppeals()
        {
            var quickLaunch = Login(EnvironmentManager.ClientUserWithReadWriteManageAppeals,
                                    EnvironmentManager.ClientUserWithReadWriteManageAppealsPassword);
            EnvironmentManager.Username = EnvironmentManager.ClientUserWithReadWriteManageAppeals;
            UserType.CurrentUserType = UserType.CLIENT;
            return quickLaunch;
        }

        public QuickLaunchPage LoginAsUserHavingManageAppealRationaleReadOnlyAuthortiy()
        {
            var quickLaunch = Login(EnvironmentManager.HciUserWithNoManageCategory,
                EnvironmentManager.HciUserPasswordWithNoManageCategory);
            EnvironmentManager.Username = EnvironmentManager.HciUserWithNoManageCategory;
            UserType.CurrentUserType = UserType.HCIUSERWITHNOMANAGEEDITRIGHT;
            return quickLaunch;
        }
        
        public QuickLaunchPage LoginAsUserHavingManageAppealsReadOnlyAuthority()
        {
            var quickLaunch = Login(EnvironmentManager.HciUserWithReadOnlyManageAppeals,
                                    EnvironmentManager.HciUserWithReadOnlyManageAppealsPassword);
            EnvironmentManager.Username = EnvironmentManager.HciUserWithReadOnlyManageAppeals;
            UserType.CurrentUserType = UserType.HCIUSER;
            return quickLaunch;
        }

        public QuickLaunchPage LoginAsUserHavingNoManageEditAuthority()
        {
            var quickLaunch = Login(EnvironmentManager.HciUserWithNoManageEdit,
                                    EnvironmentManager.HciUserPasswordWithNoManageEdit);
            EnvironmentManager.Username = EnvironmentManager.HciUserWithNoManageEdit;
            UserType.CurrentUserType = UserType.HCIUSERWITHNOMANAGEEDITRIGHT;
            return quickLaunch;
        }

        public QuickLaunchPage LoginAsClientUserWithoutManageEditsPrivilege()
        {
            var quickLaunch = Login(EnvironmentManager.ClientUserWithoutManageEditsAuthority,
                                    EnvironmentManager.ClientUserWithoutManageEditsAuthorityPassword);
            EnvironmentManager.Username = EnvironmentManager.ClientUserWithoutManageEditsAuthority;
            UserType.CurrentUserType = UserType.ClientUserWithoutManageEditAuthority;
            return quickLaunch;
        }

        public QuickLaunchPage LoginAsHciUserHavingManageCategoryReadWriteAuthority()
        {
            var quickLaunch = Login(EnvironmentManager.HciUserWithReadWriteManageCategory,
                                    EnvironmentManager.HciIUserWithReadWriteManageCategoryPassword);
            EnvironmentManager.Username = EnvironmentManager.HciUserWithReadWriteManageCategory;
            UserType.CurrentUserType = UserType.HCIUSER;
            return quickLaunch;
        }

        public QuickLaunchPage LoginAsUserHavingManageCategoryReadOnlyAuthority()
        {
            var quickLaunch = Login(EnvironmentManager.HciUserWithReadOnlyManageCategory,
                                    EnvironmentManager.HciUserWithReadOnlyManageCategoryPassword);
            EnvironmentManager.Username = EnvironmentManager.HciUserWithReadOnlyManageCategory;
            UserType.CurrentUserType = UserType.HCIUSER;
            return quickLaunch;
        }

        public QuickLaunchPage LoginAsUserHavingNoManageCategoryAuthority()
        {
            var quickLaunch = Login(EnvironmentManager.HciUserWithNoManageCategory,
                                    EnvironmentManager.HciUserPasswordWithNoManageCategory);
            EnvironmentManager.Username = EnvironmentManager.HciUserWithNoManageCategory;
            UserType.CurrentUserType = UserType.HCIUSERWITHNOMANAGEEDITRIGHT;
            return quickLaunch;
        }

        public QuickLaunchPage LoginAsUserHavingNoAnyAuthority()
        {
            var quickLaunch = Login(EnvironmentManager.HciUserWithNoManageAppealAuthority,
                EnvironmentManager.HciUserPasswordWithNoManageCategory);
            EnvironmentManager.Username = EnvironmentManager.HciUserWithNoManageAppealAuthority;
            UserType.CurrentUserType = UserType.HCIUSERWITHNOMANAGEEDITRIGHT;
            return quickLaunch;
        }

        public QuickLaunchPage LoginAsUserHavingNoManageAppealAuthority()
        {
            var quickLaunch = Login(EnvironmentManager.HciUserWithNoManageAppealAuthority,
                                    EnvironmentManager.HciUserPasswordWithNoManageAppealAuthority);
            EnvironmentManager.Username = EnvironmentManager.HciUserWithNoManageCategory;
            UserType.CurrentUserType = UserType.HCIUSERWITHNOMANAGEEDITRIGHT;
            return quickLaunch;
        }

        public QuickLaunchPage LoginAsClientUserHavingFfpEditOfPciFlagsAuthority()
        {
            var quickLaunch = Login(EnvironmentManager.ClientUserHavingFfpEditOfPciFlagsAuthority,
                                    EnvironmentManager.ClientUserHavingFfpEditOfPciFlagsAuthorityPassword);
            EnvironmentManager.Username = EnvironmentManager.ClientUserHavingFfpEditOfPciFlagsAuthority;
            UserType.CurrentUserType = UserType.CLIENT2;
            return quickLaunch; 
        }

        public QuickLaunchPage LoginAsAppealClientUser()
        {
            var quickLaunch = Login(EnvironmentManager.AppealClientUser,
                                    EnvironmentManager.AppealClientUserPassword);
            EnvironmentManager.Username = EnvironmentManager.AppealClientUser;
            UserType.CurrentUserType = UserType.CLIENT;
            return quickLaunch;
        }

        public QuickLaunchPage LoginAsHciUser()
        {
            var quickLaunch = Login(EnvironmentManager.HciUsername,EnvironmentManager.HciPassword);
            EnvironmentManager.Username = EnvironmentManager.HciUsername;
            UserType.CurrentUserType = UserType.HCIUSER;
            return quickLaunch;
        }

        public QuickLaunchPage LoginAsPciTest5User()
        {
            var quickLaunch = Login(EnvironmentManager.PciTest5User, EnvironmentManager.PciTest5Password);
            EnvironmentManager.Username = EnvironmentManager.PciTest5User;
            UserType.CurrentUserType = UserType.PCI5USER;
            return quickLaunch;
        }

        public QuickLaunchPage LoginAsPciTest5ClientUser()
        {
            var quickLaunch = Login(EnvironmentManager.PciTest5ClientUser, EnvironmentManager.PciTest5ClientPassword);
            EnvironmentManager.Username = EnvironmentManager.PciTest5ClientUser;
            UserType.CurrentUserType = UserType.PCI5CLIENTUSER;
            return quickLaunch;
        }

        public ClaimSearchPage LoginAsHciUserForMyProfile()
        {
            var newClaimSearch = Login<ClaimSearchPage>(EnvironmentManager.HCIAdminUsernameForMyProfile, EnvironmentManager.HCIPasswordForUserForMyProfile);
            EnvironmentManager.Username = EnvironmentManager.HCIAdminUsernameForMyProfile;
            UserType.CurrentUserType = UserType.HCIUSERFORMYPROFILE;
            return newClaimSearch;
        }

        public ClaimSearchPage LoginAsClientUserForMyProfile()
        {
            var claimSearchPage = Login<ClaimSearchPage>(EnvironmentManager.ClientUserNameForMyProfile, EnvironmentManager.ClientPasswordForUserForMyProfile);
            EnvironmentManager.Username = EnvironmentManager.ClientUserNameForMyProfile;
            UserType.CurrentUserType = UserType.CLIENTUSERFORMYPROFILE;
            return claimSearchPage;
        }

        public QuickLaunchPage LoginAsHCIUserWithReadOnlyAccessToAllAuthorities()
        {
            var quickLaunch = Login(EnvironmentManager.HCIUserWithReadOnlyAccessToAllAuthorites, EnvironmentManager.HCIUserWithReadOnlyAccessToAllAuthoritesPassword);
            EnvironmentManager.Username = EnvironmentManager.HCIUserWithReadOnlyAccessToAllAuthorites;
            UserType.CurrentUserType = UserType.HCIUSER;
            return quickLaunch;
        }

        public QuickLaunchPage LoginAsClientUserWithReadOnlyAccessToAllAuthorities()
        {
            var quickLaunch = Login(EnvironmentManager.ClientUserWithAllReadOnlyAuthorities, EnvironmentManager.HciAdminPassword);
            EnvironmentManager.Username = EnvironmentManager.ClientUserWithAllReadOnlyAuthorities;
            UserType.CurrentUserType = UserType.CLIENTREADONLY;
            return quickLaunch;
        }

        public QuickLaunchPage LoginTestUser(bool isValidTurn)
        {
            if (isValidTurn)
            {
                var quickLaunch = Login(EnvironmentManager.LoginTestUser,
                    EnvironmentManager.LoginTestPassword);
                EnvironmentManager.Username = EnvironmentManager.LoginTestUser;
                UserType.CurrentUserType = UserType.HCIUSER;
                return quickLaunch;
            }
            else
            {
                SiteDriver.FindElement(LoginPageObjects.LoginButtonId,How.Id).SendKeys(EnvironmentManager.LoginTestUser);
                SiteDriver.FindElement(LoginPageObjects.PasswordBoxId, How.Id).SendKeys(EnvironmentManager.LoginTestPassword);
                SiteDriver.WaitForIe(1000);
                SiteDriver.FindElement(LoginPageObjects.LoginButtonId, How.Id).Click();
                Console.WriteLine("Login with username<{0}>", EnvironmentManager.LoginTestUser);                
                return null;
            }
        }

        public ClaimSearchPage LoginAsFirstLoginUser()
        {
            var newClaimSearch = Login<ClaimSearchPage>(EnvironmentManager.HciFirstLoginUser, EnvironmentManager.HciAdminPassword);
            EnvironmentManager.Username = EnvironmentManager.HciFirstLoginUser;
            UserType.CurrentUserType = UserType.HCILOGINCOMPLETE;
            return newClaimSearch;
        }

        public ClaimSearchPage LoginAsClientFirstLoginUser()
        {
            var newClaimSearch = Login<ClaimSearchPage>(EnvironmentManager.ClientFirstLoginUser, EnvironmentManager.HciAdminPassword);
            EnvironmentManager.Username = EnvironmentManager.ClientFirstLoginUser;
            UserType.CurrentUserType = UserType.CLIENTLOGINCOMPLETE;
            return newClaimSearch;
        }
        private QuickLaunchPage Login(string username, string password)
        {
            EnvironmentManager.UserFullName = null;
            var quickLaunchPage = Navigator.Navigate<QuickLaunchPageObjects>
                (() =>
                     {
                         ClickOnCloseButton();
                         //_nucleusLoginPage.UserIdBox.ClearElementField();
                         SiteDriver.FindElement(LoginPageObjects.UserIdBoxId, How.Id).ClearElementField();
                         //_nucleusLoginPage.UserIdBox.SetText(username);
                         SiteDriver.FindElement(LoginPageObjects.UserIdBoxId, How.Id).SendKeys(username);
                         SiteDriver.WaitForIe(1000);
                         SiteDriver.FindElement(LoginPageObjects.PasswordBoxId, How.Id).ClearElementField();
                         //_nucleusLoginPage.PasswordBox.ClearElementField();
                         SiteDriver.FindElement(LoginPageObjects.PasswordBoxId, How.Id).SendKeys(password);
                         //_nucleusLoginPage.PasswordBox.SetText(password);
                         SiteDriver.WaitForIe(1000);
                         SiteDriver.FindElement(LoginPageObjects.LoginButtonId, How.Id).Click();
                         //_nucleusLoginPage.LoginButton.Click();
                         SiteDriver.WaitForPageToLoad();
                         if (!SiteDriver.Title.Contains("ORA") &&
                             !SiteDriver.Title.Contains("Connection request timed out"))
                         {
                             SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(DefaultPageObjects.LandingImageCssLocator, How.CssSelector));
                             if (!SiteDriver.IsElementPresent(DefaultPageObjects.LandingImageCssLocator, How.CssSelector))
                                 CaptureScreenShot("Nucleus Home Page Issue");
                             SiteDriver.WaitToLoadNew(5000);
                             Console.Out.WriteLine("Login by : {0}.", username);
                             return;
                         }
                        
                        
                         //SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(DefaultPageObjects.PageHeaderCssLocator, How.CssSelector));
                         

                         // Setting the UserFullName as Environmental variable
                         //var pageHeader = SiteDriver.FindElement(DefaultPageObjects.PageHeaderCssLocator,
                         //    How.CssSelector).Text;
                         //if (pageHeader == PageHeaderEnum.MyProfile.GetStringValue())
                         //    EnvironmentManager.UserFullName = GetSideWindow.GetInputValueByLabel("First Name") + " " +
                         //                                      GetSideWindow.GetInputValueByLabel("Last Name");
                         //else
                         //    EnvironmentManager.UserFullName = null;

                        
                         Console.WriteLine("Oracle Issue URL=" + SiteDriver.Url);
                         Console.WriteLine("Oracle Issue Title=" + SiteDriver.Title);
                         //BrowserOptions.SetBrowserOptions(DataHelper.EnviromentVariables);
                         SiteDriver.Open(BrowserOptions.ApplicationUrl);
                         //SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(DefaultPageObjects.PageHeaderCssLocator, How.CssSelector));
                         ClickOnCloseButton();
                         SiteDriver.FindElement(LoginPageObjects.UserIdBoxId, How.Id).SendKeys(username);
                         SiteDriver.WaitForIe(1000);
                         SiteDriver.FindElement(LoginPageObjects.PasswordBoxId, How.Id).SendKeys(password);
                         SiteDriver.WaitForIe(1000);
                         SiteDriver.FindElement(LoginPageObjects.LoginButtonId, How.Id).Click();
                         Console.Out.WriteLine("Login by : {0}.", username);
                         SiteDriver.WaitForPageToLoad();
                         SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(DefaultPageObjects.LandingImageCssLocator, How.CssSelector));
                         SiteDriver.WaitToLoadNew(5000);

                     });
            return new QuickLaunchPage(Navigator, quickLaunchPage, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public T LoginByPage<T>(string username,string userType)
        {
            UserType.CurrentUserType = userType;
            return Login<T>(username, EnvironmentManager.HciAdminPassword);            
        }

        private T Login<T>(string username, string password)
        {
            EnvironmentManager.UserFullName = null;
            var action = new Action(
            () =>
            {
                ClickOnCloseButton();
                SiteDriver.FindElement(LoginPageObjects.UserIdBoxId, How.Id).ClearElementField();
                SiteDriver.FindElement(LoginPageObjects.UserIdBoxId, How.Id).SendKeys(username);
                SiteDriver.WaitForIe(1000);
                SiteDriver.FindElement(LoginPageObjects.PasswordBoxId, How.Id).ClearElementField();
                SiteDriver.FindElement(LoginPageObjects.PasswordBoxId, How.Id).SendKeys(password);
                SiteDriver.WaitForIe(1000);
                SiteDriver.FindElement(LoginPageObjects.LoginButtonId, How.Id).Click();
                SiteDriver.WaitForPageToLoad();
                    //SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(DefaultPageObjects.LandingImageCssLocator, How.CssSelector));
                //if (!SiteDriver.IsElementPresent(DefaultPageObjects.LandingImageCssLocator, How.CssSelector))
                //    EnvironmentManager.CaptureScreenShot("Nucleus Home Page Issue");
                Console.Out.WriteLine("Login by : {0}.", username);
                Console.WriteLine("Get Current Login URL=" + SiteDriver.Url);
                if (!SiteDriver.Title.Contains("ORA") &&
                    !SiteDriver.Title.Contains("Connection request timed out")) return;
                Console.WriteLine("Oracle Issue URL=" + SiteDriver.Url);
                Console.WriteLine("Oracle Issue Title=" + SiteDriver.Title);
               //BrowserOptions.SetBrowserOptions(DataHelper.EnviromentVariables);
                SiteDriver.Open(BrowserOptions.ApplicationUrl);
                ClickOnCloseButton();
                SiteDriver.FindElement(LoginPageObjects.UserIdBoxId, How.Id).SendKeys(username);
                SiteDriver.WaitForIe(1000);
                SiteDriver.FindElement(LoginPageObjects.PasswordBoxId, How.Id).SendKeys(password);
                SiteDriver.WaitForIe(1000);
                SiteDriver.FindElement(LoginPageObjects.LoginButtonId, How.Id).Click();
                Console.Out.WriteLine("Login by : {0}.", username);
                SiteDriver.WaitForPageToLoad();
                SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(DefaultPageObjects.LandingImageCssLocator, How.CssSelector));
            });
            var target = typeof(T);
            if (typeof(ClaimSearchPage) == target)
            {
                PageObject =
                    Navigator.Navigate<ClaimSearchPageObjects>(action);
                SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(SideBarPanelSearch.SideBarPanelSectionCssLocator, How.CssSelector));
            }
            else if (typeof(AppealSearchPage) == target)
            {
                PageObject =
                    Navigator.Navigate<AppealSearchPageObjects>(action);
                SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(SideBarPanelSearch.SideBarPanelSectionCssLocator, How.CssSelector));
            }
            
            else if (typeof(BatchSearchPage) == target)
            {
                PageObject =
                    Navigator.Navigate<BatchSearchPageObjects>(action);
                SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(SideBarPanelSearch.SideBarPanelSectionCssLocator, How.CssSelector));
            }       
            else if (typeof(ProviderSearchPage) == target)
            {
               PageObject =
                    Navigator.Navigate<ProviderSearchPageObjects>(action);
                SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(SideBarPanelSearch.SideBarPanelSectionCssLocator, How.CssSelector));
            }
            else if (typeof(SuspectProvidersPage) == target)
            {
                PageObject =
                    Navigator.Navigate<SuspectProvidersPageObjects>(action);
            }
            else if (typeof(MyProfilePage) == target)
            {
                PageObject = Navigator.Navigate<MyProfilePageObjects>(action);
            }
            else
                PageObject =
                    Navigator.Navigate<QuickLaunchPageObjects>(action);

            SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(DefaultPageObjects.PageHeaderCssLocator, How.CssSelector));
            return (T)Activator.CreateInstance(target, Navigator, PageObject,SiteDriver,JavaScriptExecutor,EnvironmentManager,BrowserOptions,Executor);
        }

        public ClaimSearchPage LoginAsUserWithReadOnlyRetroClaimSearchAuthority()
        {           
            var newClaimSearchPage = Navigator.Navigate<ClaimSearchPageObjects>
                (() =>
                {
                    ClickOnCloseButton();
                    SiteDriver.FindElement(LoginPageObjects.UserIdBoxId, How.Id).SendKeys(EnvironmentManager.HCIUserWithReadOnlyRetroClaimSearchAuthority);
                    SiteDriver.FindElement(LoginPageObjects.PasswordBoxId, How.Id).SendKeys(EnvironmentManager.HCIUserWithReadOnlyRetroClaimSearchAuthorityPassword);
                    SiteDriver.FindElement(LoginPageObjects.LoginButtonId, How.Id).Click();
                    Console.Out.WriteLine("Login by : {0}.", EnvironmentManager.HCIUserWithReadOnlyRetroClaimSearchAuthority);
                });
            EnvironmentManager.Username = EnvironmentManager.HCIUserWithReadOnlyRetroClaimSearchAuthority;
            UserType.CurrentUserType = UserType.HCIUSER;
            SiteDriver.WaitForCondition(() => !SiteDriver.IsElementPresent(ClaimSearchPageObjects.SpinnerCssLocator, How.CssSelector));
            SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(ClaimSearchPageObjects.SideBarPannelCssLocator, How.CssSelector));
            return new ClaimSearchPage(Navigator, newClaimSearchPage, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);            
        }

        public ClaimSearchPage LoginAsClientUserWithNoAnyAuthority()
        {
            var newClaimSearchPage = Navigator.Navigate<ClaimSearchPageObjects>
                (() =>
                {
                    ClickOnCloseButton();
                    SiteDriver.FindElement(LoginPageObjects.UserIdBoxId, How.Id).SendKeys(EnvironmentManager.ClientUserWithNoAnyAuthority);
                    SiteDriver.FindElement(LoginPageObjects.PasswordBoxId, How.Id).SendKeys(EnvironmentManager.ClientUserWithNoAnyAuthorityPassword);
                    SiteDriver.FindElement(LoginPageObjects.LoginButtonId, How.Id).Click();
                    Console.Out.WriteLine("Login by : {0}.", EnvironmentManager.ClientUserWithNoAnyAuthority);
                });
            EnvironmentManager.Username = EnvironmentManager.ClientUserWithNoAnyAuthority;
            UserType.CurrentUserType = UserType.CLIENTWITHNOAUTHORITY;
            SiteDriver.WaitForCondition(() => !SiteDriver.IsElementPresent(ClaimSearchPageObjects.SpinnerCssLocator, How.CssSelector));
            SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(ClaimSearchPageObjects.SideBarPannelCssLocator, How.CssSelector));
            return new ClaimSearchPage(Navigator, newClaimSearchPage, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public QuickLaunchPage LoginAsClientUserWithNoAnyAuthorityAndRedirectToQuickLaunch()
        {
            var quickLaunch = Navigator.Navigate<QuickLaunchPageObjects>
            (() =>
            {
                ClickOnCloseButton();
                SiteDriver.FindElement(LoginPageObjects.UserIdBoxId, How.Id).SendKeys(EnvironmentManager.ClientUserWithNoAnyAuthority);
                SiteDriver.FindElement(LoginPageObjects.PasswordBoxId, How.Id).SendKeys(EnvironmentManager.ClientUserWithNoAnyAuthorityPassword);
                SiteDriver.FindElement(LoginPageObjects.LoginButtonId, How.Id).Click();
                Console.Out.WriteLine("Login by : {0}.", EnvironmentManager.ClientUserWithNoAnyAuthority);
                EnvironmentManager.Username = EnvironmentManager.ClientUserWithNoAnyAuthority;
                UserType.CurrentUserType = UserType.CLIENTWITHNOAUTHORITY;
                SiteDriver.WaitForCondition(() => !SiteDriver.IsElementPresent(ClaimSearchPageObjects.SpinnerCssLocator, How.CssSelector));
                SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(ClaimSearchPageObjects.SideBarPannelCssLocator, How.CssSelector));
                JavaScriptExecutor.ExecuteClick(DefaultPageObjects.QuickLaunchButtonXPath, How.XPath);                
            });
            return new QuickLaunchPage(Navigator, quickLaunch, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public QuickLaunchPage LoginAsHciAdminUserWithUserMaintainenaceAuthorityToUpdatePassword()
        {
            var quickLaunch = Login(EnvironmentManager.HciAdminUserWithUserMaintainenaceAuthorityToUpdatePassword, EnvironmentManager.UpdatePassword);
            EnvironmentManager.Username = EnvironmentManager.HciAdminUserWithUserMaintainenaceAuthorityToUpdatePassword;
            UserType.CurrentUserType = UserType.Adminwithusermaintenanceauthority;
            return quickLaunch;
        }

        public QuickLaunchPage LoginAsHciAdminUserWithUserMaintainenaceAuthorityToUpdatePasswordWithNewPassword()
        {
            var quickLaunch = Login(EnvironmentManager.HciAdminUserWithUserMaintainenaceAuthorityToUpdatePassword, EnvironmentManager.UpdateCurrentPasswordAndSetToNew);
            EnvironmentManager.Username = EnvironmentManager.HciAdminUserWithUserMaintainenaceAuthorityToUpdatePassword;
            UserType.CurrentUserType = UserType.Adminwithusermaintenanceauthority;
            return quickLaunch;
        }

        public QuickLaunchPage LoginasHciAdminUser7()
        {
            var quickLaunch = Login(EnvironmentManager.HciAdminUsername7,
                EnvironmentManager.HciAdminPassword);
            EnvironmentManager.Username = EnvironmentManager.HciAdminUsername7;
            UserType.CurrentUserType = UserType.HCIADMIN;
            return quickLaunch;
        }

        public QuickLaunchPage LoginAsClientUser7()
        {
            var quickLaunch = Login(EnvironmentManager.ClientUsername7,
                EnvironmentManager.ClientPassword);
            EnvironmentManager.Username = EnvironmentManager.ClientUsername7;
            UserType.CurrentUserType = UserType.CLIENT7;
            return quickLaunch;
        }

        public QuickLaunchPage LoginAsHciAdminUserWithReadOnlyUserMaintainenaceAuthorityToUpdatePassword()
        {
            var quickLaunch = Login(EnvironmentManager.HciAdminUserWithReadOnlyUserMaintainenaceAuthorityToUpdatePassword, EnvironmentManager.UpdatePassword);
            EnvironmentManager.Username = EnvironmentManager.HciAdminUserWithReadOnlyUserMaintainenaceAuthorityToUpdatePassword;
            UserType.CurrentUserType = UserType.Adminwithreadonlyusermaintenanceauthority;
            return quickLaunch;
        }

        public QuickLaunchPage LoginAsHciAdminUserWithReadOnlyUserMaintainenaceAuthorityWithNewPassword()
        {
            var quickLaunch = Login(EnvironmentManager.HciAdminUserWithReadOnlyUserMaintainenaceAuthorityToUpdatePassword, EnvironmentManager.UpdateCurrentPasswordAndSetToNew);
            EnvironmentManager.Username = EnvironmentManager.HciAdminUserWithReadOnlyUserMaintainenaceAuthorityToUpdatePassword;
            UserType.CurrentUserType = UserType.Adminwithreadonlyusermaintenanceauthority;
            return quickLaunch;
        }

        public QuickLaunchPage LoginAsHciAdminUserWithReadOnlyUserMaintainenaceAuthorityWithPasswordUpdatedByOtherUser()
        {
            var quickLaunch = Login(EnvironmentManager.HciAdminUserWithReadOnlyUserMaintainenaceAuthorityToUpdatePassword, EnvironmentManager.PasswordUpdatedByOtherUser);
            EnvironmentManager.Username = EnvironmentManager.HciAdminUserWithReadOnlyUserMaintainenaceAuthorityToUpdatePassword;
            UserType.CurrentUserType = UserType.Adminwithreadonlyusermaintenanceauthority;
            return quickLaunch;
        }

        public QuickLaunchPage LoginAsHciAdminUserWithNoUserMaintainenaceAuthorityToUpdatePassword()
        {
            var quickLaunch = Login(EnvironmentManager.HciAdminUserWithNoUserMaintainenaceAuthorityToUpdatePassword, EnvironmentManager.UpdatePassword);
            EnvironmentManager.Username = EnvironmentManager.HciAdminUserWithNoUserMaintainenaceAuthorityToUpdatePassword;
            UserType.CurrentUserType = UserType.Adminwithnousermaintenanceauthority;
            return quickLaunch;
        }

        public QuickLaunchPage LoginAsHciAdminUserWithNoUserMaintainenaceAuthorityWithNewPassword()
        {
            var quickLaunch = Login(EnvironmentManager.HciAdminUserWithNoUserMaintainenaceAuthorityToUpdatePassword, EnvironmentManager.UpdateCurrentPasswordAndSetToNew);
            EnvironmentManager.Username = EnvironmentManager.HciAdminUserWithNoUserMaintainenaceAuthorityToUpdatePassword;
            UserType.CurrentUserType = UserType.Adminwithnousermaintenanceauthority;
            return quickLaunch;
        }

        public QuickLaunchPage LoginAsHciAdminUserWithNoUserMaintainenaceAuthorityWithPasswordUpdatedByOtherUser()
        {
            var quickLaunch = Login(EnvironmentManager.HciAdminUserWithNoUserMaintainenaceAuthorityToUpdatePassword, EnvironmentManager.PasswordUpdatedByOtherUser);
            EnvironmentManager.Username = EnvironmentManager.HciAdminUserWithNoUserMaintainenaceAuthorityToUpdatePassword;
            UserType.CurrentUserType = UserType.Adminwithnousermaintenanceauthority;
            return quickLaunch;
        }

        public QuickLaunchPage LoginAsClientUserWithClaimViewRestriction()
        {
            var quickLaunch = Login(EnvironmentManager.ClientUserWithClaimViewRestriction, EnvironmentManager.HciAdminPassword);
            EnvironmentManager.Username = EnvironmentManager.ClientUserWithClaimViewRestriction;
            UserType.CurrentUserType = UserType.CLIENTCLAIMVIEWRESTRICTION;
            return quickLaunch;
        }

        public QuickLaunchPage LoginAsHciUserWithManageEditAuthority()
        {
            var quickLaunch = Login(EnvironmentManager.HciUserWithManageEditAuthority, EnvironmentManager.HciAdminPassword);
            EnvironmentManager.Username = EnvironmentManager.HciUserWithManageEditAuthority;
            UserType.CurrentUserType = UserType.AdminWithManageEditAuthority;
            return quickLaunch;
        }

        public QuickLaunchPage LoginAsHciUserWithMyDashboardAuthorityAndReadOnlyProductDashboardAuthority()
        {
            var quickLaunch = Login(EnvironmentManager.HciUserWithManageEditAuthority,
                EnvironmentManager.HciAdminPassword);
            EnvironmentManager.Username = EnvironmentManager.HciUserWithManageEditAuthority;
            UserType.CurrentUserType = UserType.AdminWithManageEditAuthority;
            return quickLaunch;
        }

        public QuickLaunchPage LoginAsHciUserWithReadOnlyMyDashboardAndReadWriteProductDashboardAuthority()
        {
            var quickLaunch = Login(EnvironmentManager.HciAdminUserWithUserMaintainenaceAuthorityToUpdatePassword, EnvironmentManager.HciAdminPassword);
            EnvironmentManager.Username = EnvironmentManager.HciAdminUserWithUserMaintainenaceAuthorityToUpdatePassword;
            UserType.CurrentUserType = UserType.Adminwithusermaintenanceauthority;
            return quickLaunch;       
        }

        public QuickLaunchPage LoginAsHciUserWithNoProductDashboardAuthority()
        {
            return LoginAsHciAdminUserWithNoUserMaintainenaceAuthorityToUpdatePassword();
        }

        public string LoginAndDoNotNavigate(string username,string password)
        {
            SiteDriver.FindElement(LoginPageObjects.UserIdBoxId, How.Id).ClearElementField();
            SiteDriver.FindElement(LoginPageObjects.UserIdBoxId, How.Id).SendKeys(username);
            SiteDriver.WaitForIe(1000);
            SiteDriver.FindElement(LoginPageObjects.PasswordBoxId, How.Id).ClearElementField();
            SiteDriver.FindElement(LoginPageObjects.PasswordBoxId, How.Id).SendKeys(EnvironmentManager.ClientUserWithNoAnyAuthorityPassword);
            SiteDriver.WaitForIe(1000);
            SiteDriver.FindElement(LoginPageObjects.LoginButtonId, How.Id).Click();
            return SiteDriver.Url;
        }

        public QuickLaunchPage LoginAsMstrUser1()
        {
            var quickLaunch = Login(EnvironmentManager.MstrtUser1,
                EnvironmentManager.MstrtUser1Password);
            EnvironmentManager.Username = EnvironmentManager.MstrtUser1;
            UserType.CurrentUserType = UserType.MstrUser;
            return quickLaunch;
        }

        public QuickLaunchPage LoginAsClientMstrUser()
        {
            var quickLaunch = Login(EnvironmentManager.ClientMstrtUser,
                EnvironmentManager.ClientMstrUserPassword);
            EnvironmentManager.Username = EnvironmentManager.ClientMstrtUser;
            UserType.CurrentUserType = UserType.ClientMstrUser;
            return quickLaunch;
        }

        public string LoginWithReturnUrl(string Url)
        { 
            SiteDriver.WebDriver.Navigate().GoToUrl(BrowserOptions.ApplicationUrl+"?ReturnUrl="+Url);
            SiteDriver.FindElement(LoginPageObjects.UserIdBoxId, How.Id).ClearElementField();
            SiteDriver.FindElement(LoginPageObjects.UserIdBoxId, How.Id).SendKeys(EnvironmentManager.HciAdminUsername);
            SiteDriver.WaitForIe(1000);
            SiteDriver.FindElement(LoginPageObjects.PasswordBoxId, How.Id).ClearElementField();
            SiteDriver.FindElement(LoginPageObjects.PasswordBoxId, How.Id).SendKeys(EnvironmentManager.HciAdminPassword);
            SiteDriver.WaitForIe(1000);
            SiteDriver.FindElement(LoginPageObjects.LoginButtonId, How.Id).Click();
            return SiteDriver.Url;
        }


        public QuickLaunchPage LoginAsHCIUserwithONLYDCIWorklistauthority()
        {
            var quickLaunch = Login(EnvironmentManager.HCIUserWithOnlyDciWorklistAuthorityAndNoOtherAuthority, EnvironmentManager.HciAdminPassword);
            EnvironmentManager.Username = EnvironmentManager.HCIUserWithOnlyDciWorklistAuthorityAndNoOtherAuthority;
            UserType.CurrentUserType = UserType.HciUserWithDciWorkListAuthority;
            return quickLaunch;           
        }

        public QuickLaunchPage LoginAsClientUserwithONLYDCIWorklistauthority()
        {
            var quickLaunch = Login(EnvironmentManager.ClientUserWithOnlyDciWorklistAuthorityAndNoOtherAuthority, EnvironmentManager.HciAdminPassword);
            EnvironmentManager.Username = EnvironmentManager.ClientUserWithOnlyDciWorklistAuthorityAndNoOtherAuthority;
            UserType.CurrentUserType = UserType.ClientUserWithDciWorkListAuthority;
            return quickLaunch;
        }

        public QuickLaunchPage LoginAsHciUserHCIUserWithoutPCIProductAuthority()
        {
            var quickLaunch = Login(EnvironmentManager.HCIUserWithoutPCIProductAuthority, EnvironmentManager.HciAdminPassword);
            EnvironmentManager.Username = EnvironmentManager.HCIUserWithoutPCIProductAuthority;
            UserType.CurrentUserType = UserType.HCIUserWithoutPCIProductAuthority;
            return quickLaunch;

        }

        public void UpdateFlagsValue(string flagsValue, string userId)
        {
            Executor.ExecuteQuery(Format(UserSqlScriptObjects.UpdateFlagsValueFromDb, flagsValue, userId));
        }

        public int GetFailedLoginCountFromDatabase(string userId)
        {
            return Convert.ToInt32(Executor.GetSingleStringValue(Format(UserSqlScriptObjects.GetFailedLoginCountsFromDatabase, userId)));
        }

        #region Complete Your Basic Profile
        public void DeleteLatestPasswordFromDb(string userId)
        {
            Executor.ExecuteQuery(Format(UserSqlScriptObjects.DeleteLatestPasswordFromDb, userId));
        }
        public bool IsStepSelected(string formHeader) => JavaScriptExecutor
            .FindElement(Format(LoginPageObjects.CompleteYourBasicProfileStepsCssSelector, formHeader))
            .GetAttribute("class").Contains("current");

        public bool IsStepCompleted(string formHeader) => JavaScriptExecutor
            .FindElement(Format(LoginPageObjects.CompleteYourBasicProfileStepsCssSelector, formHeader))
            .GetAttribute("class").Contains("complete");
        public bool IsStepWiseFormPresent(string formHeader) => JavaScriptExecutor
            .IsElementPresent(Format(LoginPageObjects.StepWiseFormCssLocator, formHeader));

        public List<string> GetInputFieldLabelList() =>
            JavaScriptExecutor.FindElements(LoginPageObjects.InputFieldLabelsCssSelector, "Text");

        public bool IsInputFieldPresentByLabel(string label) =>
            JavaScriptExecutor.IsElementPresent(Format(LoginPageObjects.InputFieldByLabelCssSelectors, label));

        public List<string> GetInputFieldsValue() =>
            JavaScriptExecutor.FindElements(LoginPageObjects.InputFieldsCssSelector, "value", true);

        public void SetCompleteYourBasicProfileInput(string label, string value)
        {
            JavaScriptExecutor.FindElement(Format(LoginPageObjects.InputFieldByLabelCssSelectors, label)).ClearElementField();
            JavaScriptExecutor.FindElement(Format(LoginPageObjects.InputFieldByLabelCssSelectors, label)).SendKeys(value);
        }

        public void SetAnswerAndPasswordInputByRowAndLabel(string value, string label, int row = 1)
        {
            JavaScriptExecutor.FindElement(Format(LoginPageObjects.AnswerAndPasswordInputFieldCssSelectorTemplate, row, label)).ClearElementField();
            JavaScriptExecutor.FindElement(Format(LoginPageObjects.AnswerAndPasswordInputFieldCssSelectorTemplate, row, label)).SendKeys(value);
        }
       
        public bool IsAnswerAndPasswordInputFieldPresentByRowAndLabel(string label, int row = 1) =>
            JavaScriptExecutor.IsElementPresent(Format(LoginPageObjects.AnswerAndPasswordInputFieldCssSelectorTemplate, row, label));

        public string GenerateRandomTextBasedOnAcceptedValues(string acceptableValues, int length) =>
            Extensions.GetRandomString(acceptableValues, length);

        public int ValidateAnswerInputBoxByRow(string label, int row = 2)
        {
            var alphanumericValue = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_`~!@#$%^&*()-=+{}|<>?,./:";
            var answer = GenerateRandomTextBasedOnAcceptedValues(alphanumericValue, 101);
            SetAnswerAndPasswordInputByRowAndLabel(answer, label, row);
            return GetAnswerAndPasswordInputByRowAndLabel(label, row).Length;
        }

        public string GetAnswerAndPasswordInputByRowAndLabel(string label, int row = 2) =>
            JavaScriptExecutor.FindElement(Format(LoginPageObjects.AnswerAndPasswordInputFieldCssSelectorTemplate, row, label)).GetAttribute("value");

        public void ClearCompleteYourBasicProfileInputField(string label, bool isAnswer = false, int row = 2 )
        {
            if (isAnswer)
            {
                JavaScriptExecutor.FindElement(Format(LoginPageObjects.AnswerAndPasswordInputFieldCssSelectorTemplate, row, label)).ClearElementField();
            }

            else
            {
                JavaScriptExecutor.FindElement(Format(LoginPageObjects.InputFieldByLabelCssSelectors, label)).ClearElementField();
            }
        }

        public bool IsNextButtonPresent() =>
            SiteDriver.IsElementPresent(LoginPageObjects.NextButtonCssLocator, How.CssSelector);

        public bool IsPreviousButtonPresent() => SiteDriver.IsElementPresent(LoginPageObjects.PreviousButtonCssLocator, How.CssSelector);

        public void ClickNextButton()
        {
            SiteDriver.FindElement(LoginPageObjects.NextButtonCssLocator, How.CssSelector).Click();
            SiteDriver.WaitToLoadNew(500);
        }


        public void ClickPreviousButton()
        {
            SiteDriver.FindElement(LoginPageObjects.PreviousButtonCssLocator, How.CssSelector).Click();
            SiteDriver.WaitToLoadNew(500);
        }

        public List<string> GetPasswordDescription() => SiteDriver.FindElementsAndGetAttribute("innerHTML",LoginPageObjects.PasswordDescriptionCssSelector,How.CssSelector);

        public void WaitToLoadCompleteYourBasicProfileForm(int waitTime = 5000)
        {
            SiteDriver.WaitForCondition(IsCompleteYourBasicProfileFormPresent, waitTime);
            SiteDriver.WaitForCondition(() =>
                SiteDriver.IsElementPresent(LoginPageObjects.NextButtonCssLocator, How.XPath), 2000);
        }
        public bool IsCompleteYourBasicProfileFormPresent()
        {
            SiteDriver.WaitForCondition(() =>
                SiteDriver.IsElementPresent(LoginPageObjects.CompleteYourBasicProfileFormXPath, How.XPath), 2000);
            return SiteDriver.IsElementPresent(LoginPageObjects.CompleteYourBasicProfileFormXPath, How.XPath, 1000);
        }

        public bool IsFormHeaderPresent(string header) =>
            JavaScriptExecutor.IsElementPresent(Format(LoginPageObjects.FormHeaderCssSelectorTemplate, header));

        public bool IsDropDownListPresenByLabel(string label) =>
            SiteDriver.IsElementPresent(Format(LoginPageObjects.DropDownInputListByLabelXPathTemplate, label),
                How.XPath);
        public List<string> GetAvailableDropDownList(string label)
        {
            var element = SiteDriver.FindElement(Format(LoginPageObjects.DropDownInputFieldXPathTemplate, label),How.XPath);
            element.Click();
            Console.WriteLine("Looking for <{0}> List", label);
            SiteDriver.WaitToLoadNew(500);
            var list = JavaScriptExecutor.FindElements(Format(LoginPageObjects.DropDownInputListByLabelXPathTemplate, label), How.XPath, "Text");
            if (list.Count == 0)
            {
                SiteDriver.FindElement(string.Format(LoginPageObjects.DropDownInputFieldXPathTemplate, label), How.XPath).Click();
                SiteDriver.WaitToLoadNew(500);
                list = JavaScriptExecutor.FindElements(string.Format(LoginPageObjects.DropDownInputListByLabelXPathTemplate, label), How.XPath, "Text");
            }
            list.Remove("");
            SiteDriver.FindElement(string.Format(LoginPageObjects.DropDownInputFieldXPathTemplate, label), How.XPath).Click();
            Console.WriteLine("<{0}> Drop down list count is {1} ", label, list.Count);
            return list;
        }

        public string GetDropDownInputFieldByLabel(string label)
        {
            return SiteDriver.FindElement(Format(LoginPageObjects.DropDownInputFieldXPathTemplate, label), How.XPath)
                .GetAttribute("value");

        }

        public void SelectDropDownListValueByLabel(string label, string value, bool directSelect = true)
        {
            SiteDriver.WaitToLoadNew(300);
            var element = SiteDriver.FindElement(Format(LoginPageObjects.DropDownInputFieldXPathTemplate, label), How.XPath);
            if (!GetDropDownInputFieldByLabel(label).Equals(value))
            {
                JavaScriptExecutor.ExecuteClick(Format(LoginPageObjects.DropDownInputFieldXPathTemplate, label),How.XPath);
                SiteDriver.WaitToLoadNew(300);
                try
                {
                    element.ClearElementField();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                if (!directSelect) element.SendKeys(value);
                if (!SiteDriver.IsElementPresent(Format(LoginPageObjects.DropDownInputListValueByLabelAndValueXPathTemplate, label, value), How.XPath)) JavaScriptExecutor.ClickJQuery(Format(LoginPageObjects.DropDownInputFieldXPathTemplate, label));
                SiteDriver.FindElement(Format(LoginPageObjects.DropDownInputListValueByLabelAndValueXPathTemplate, label, value), How.XPath).Click();
                SiteDriver.WaitToLoadNew(300);
            }
            Console.WriteLine("<{0}> Selected in <{1}> ", value, label);
        }

        public bool IsDropDownFieldPresentByLabel(string label) =>
            SiteDriver.IsElementPresent(Format(LoginPageObjects.DropDownInputFieldXPathTemplate, label), How.XPath);

        public ClaimSearchPage SwitchTabToLogInAsFirstLoginUser()
        {
            JavaScriptExecutor.ExecuteScript("window.open()");
            var login = Navigator.Navigate<LoginPageObjects>(() =>
            {
                var handles = SiteDriver.WindowHandles.ToList();
                SiteDriver.SwitchWindow(handles[1]);
                SiteDriver.Open(BrowserOptions.ApplicationUrl);
                SiteDriver.WaitForCondition(
                    () => SiteDriver.SwitchWindowByTitle(PageTitleEnum.Login.GetStringValue()));

            });

            var loginPage = new LoginPage(Navigator, login, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
            return loginPage.LoginAsFirstLoginUser();

        }

        public ClaimSearchPage SwitchTabToLogInAsClientFirstLoginUser()
        {
            JavaScriptExecutor.ExecuteScript("window.open()");
            var login = Navigator.Navigate<LoginPageObjects>(() =>
            {
                var handles = SiteDriver.WindowHandles.ToList();
                SiteDriver.SwitchWindow(handles[1]);
                SiteDriver.Open(BrowserOptions.ApplicationUrl);
                SiteDriver.WaitForCondition(
                    () => SiteDriver.SwitchWindowByTitle(PageTitleEnum.Login.GetStringValue()));

            });

            var loginPage = new LoginPage(Navigator, login, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
            return loginPage.LoginAsClientFirstLoginUser();

        }

        public void CloseTab(int handleToClose, int handleToSwitch)
        {
            while (SiteDriver.WindowHandles.Count != 1)
            {
                SiteDriver.SwitchWindow(SiteDriver.WindowHandles[handleToClose]);
                {
                    SiteDriver.CloseWindow();
                    SiteDriver.SwitchWindow(SiteDriver.WindowHandles[handleToSwitch]);
                }
            }
        }
        
        public string GetWelcomeNoteByLabel(string label) => JavaScriptExecutor
            .FindElement(Format(LoginPageObjects.WelcomeNoteCssSelectorTemplate, label)).GetAttribute("innerHTML");
        
        public MyProfilePage ClickVisitMyProfile(string header)
        {
            var myProfilePage = Navigator.Navigate<MyProfilePageObjects>(() =>
                {
                    JavaScriptExecutor.FindElement(Format(LoginPageObjects.FormHeaderCssSelectorTemplate, header))
                        .Click();
                }
            );
            return new MyProfilePage(Navigator, myProfilePage, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }
        
        public void FillInputFields(List<string> label, List<string> values)
        {
            for (int i = 0; i < label.Count; i++)
            {
                SetCompleteYourBasicProfileInput(label[i], values[i]);
            }

        }

        #endregion

        #region VerifyEmail

        public bool IsVerifyEmailAddressFormPresent() =>
            SiteDriver.IsElementPresent(LoginPageObjects.VerifyEmailFormCssSelector, How.CssSelector);

        public bool IsVerifyEmailHeaderPresent() => JavaScriptExecutor.IsElementPresent(LoginPageObjects.VerifyEmailHeaderCssSelector);
        public string GetVerifyEmailMessage() =>
            SiteDriver.FindElementAndGetAttribute(LoginPageObjects.VerifyEmailMessageCssSelector, How.CssSelector,
                "innerText");

        public bool IsVerifyEmailCurrentEmailAddressLabelPresent() => JavaScriptExecutor.IsElementPresent(LoginPageObjects.VerifyEmailCurrentEmailLabelCssSelector);

        public bool IsVerifyEmailCurrentEmailTextBoxPresent() => SiteDriver.IsElementPresent(LoginPageObjects.VerifyEmailCurrentEmailTextBoxCssSelector, How.CssSelector);

        public string GetVerifyEmailCurrentEmailValue() => SiteDriver.FindElementAndGetAttribute(LoginPageObjects.VerifyEmailCurrentEmailTextBoxCssSelector, How.CssSelector,"value");

        public void SetCurrentEmail(string value)
        {
            var element = SiteDriver.FindElement(LoginPageObjects.VerifyEmailCurrentEmailTextBoxCssSelector, How.CssSelector);
            element.ClearElementField();
            element.SendKeys(value);
        }

        public bool IsVerifyEmailButtonPresent() =>
            SiteDriver.IsElementPresent(LoginPageObjects.VerifyEmailAddressButton, How.CssSelector);

        public void ClickVerifyEmailButton()
        {
            SiteDriver
                .FindElement(LoginPageObjects.VerifyEmailAddressButton, How.CssSelector).Click();
            WaitForWorkingAjaxMessage();
        }

        public string GetVerifyEmailButtonText() =>
            SiteDriver.FindElementAndGetAttribute(LoginPageObjects.VerifyEmailAddressButton, How.CssSelector,
                "outerText");
     
        public void WaitForVerifyEmailFormToLoad(int waitTime = 2000) => SiteDriver.WaitForCondition(IsVerifyEmailAddressFormPresent, waitTime);
        #endregion

        public void WaitToLoadPageErrorPopupModal(int waitTime = 500)
        {
            SiteDriver.WaitForCondition(IsPageErrorPopupModalPresent, waitTime);
        }
        public bool IsPageErrorPopupModalPresent()
        {
            SiteDriver.WaitForCondition(() =>
                SiteDriver.IsElementPresent(DefaultPageObjects.PageErrorPopupModelId, How.Id), 500);
            return SiteDriver.IsElementPresent(DefaultPageObjects.PageErrorPopupModelId, How.Id, 200);
        }
        public string GetPageErrorMessage()
        {
            SiteDriver.WaitForCondition(IsPageErrorPopupModalPresent, 500);
            return SiteDriver.FindElement(DefaultPageObjects.PageErrorMessageId, How.Id).Text;
        }

        public void ClosePageError()
        {
            JavaScriptExecutor.ExecuteClick(DefaultPageObjects.PageErrorCloseId, How.Id);
            SiteDriver.WaitToLoadNew(500);
            Console.WriteLine("Closed the modal popup");
        }

        public bool IsWorkingAjaxMessagePresent()
        {
            return SiteDriver.IsElementPresent(DefaultPageObjects.WorkingAjaxMessageCssLocator, How.CssSelector);
        }
        public void WaitForWorkingAjaxMessage()
        {
            SiteDriver.WaitForCondition(IsWorkingAjaxMessagePresent, 2000);
            SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
            SiteDriver.WaitForPageToLoad();
        }


        public QuickLaunchPage LoginAsJITUser()
        {
            var quickLaunch = Login(EnvironmentManager.JITUsername, EnvironmentManager.JITPassword);
            EnvironmentManager.Username = EnvironmentManager.HciUsername;
            UserType.CurrentUserType = UserType.HCIUSER;
            return quickLaunch;
        }
        #region Database Interaction

        public void UpdateEmailVerificationFromDb(string emailValue, string userId) =>
            Executor.ExecuteQuery(Format(UserSqlScriptObjects.UpdateEmailVerificationFromDb, emailValue, userId));

        public List<string> GetUsersDetailFromDatabaseByUserId(string fields, string userId) 
            {
            var userDetailList = new List<string>();
            var details = Executor.GetCompleteTableWithNullValues(Format(UserSqlScriptObjects.GetDetailsOfUsersFromDbByUserId, fields, userId)).ToList();
            foreach (DataRow row in details)
            {
                userDetailList = row.ItemArray
                    .Select(x => x.ToString()).ToList();

            }
            return userDetailList;
        }

        public List<string> GetAvailabeQuestionListFromDb()
        {
            return Executor.GetTableSingleColumn(UserSqlScriptObjects.GetSecurityQuestionList);
        }

        public int GetLatestPasswordCountFromDb(string userId)
        {
            return Executor.GetCompleteTable(Format(UserSqlScriptObjects.SelectPasswordDetail, userId)).Count();
        }

        public string GetUserAuthenticationAudit(string userName)
        {
            return Executor.GetSingleStringValue(Format(UserSqlScriptObjects.UserAuthenticationAudit, userName));
        }

        public void clickonOktaLinkFromLoginPage()
        {
            var element = SiteDriver.FindElement(LoginPageObjects.OktaLoginUrlInNucleus, How.CssSelector);
                element.Click();
            SiteDriver.WaitForCondition(isOktaHeaderPresent);
            
        }

        public bool isOktaHeaderPresent()
        {
            return SiteDriver.IsElementPresent(LoginPageObjects.OktaHeader, How.CssSelector);
        }

        public void UpdateSSOSettingFOrUser(string username, bool flag = false)
        {
            if (flag)
            {
                Executor.ExecuteQuery(string.Format(UserSqlScriptObjects.UpdateSSOSettingforUser, 'T', username));
            }
            else
            {
                Executor.ExecuteQuery(string.Format(UserSqlScriptObjects.UpdateSSOSettingforUser, 'F', username));
            }


        }

        #endregion
    }

    #endregion


}


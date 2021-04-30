using System;
using System.Security.Policy;
using Nucleus.Service.PageObjects.Settings.User;
using Nucleus.Service.PageServices.Base.Default;
using Nucleus.Service.Support.Common;
using Nucleus.Service.Support.Environment;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Common.Constants;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Core.Navigation;
using UIAutomation.Framework.Elements;
using UIAutomation.Framework.Utils;
using System.Collections.Generic;
using Nucleus.Service.SqlScriptObjects.Settings;
using Nucleus.Service.SqlScriptObjects.User;
using Nucleus.Service.Support.Enum;
using UIAutomation.Framework.Database;
using OpenQA.Selenium;


namespace Nucleus.Service.PageServices.Settings.User
{
    public class OldUserProfileSearchPage : NewDefaultPage
    {
        #region PRIVATE FIELDS

        private OldUserProfileSearchPageObjects _userProfileSearch;
        private SearchFilter _searchFilter;
        private string _currentWindow = string.Empty;
       
        #endregion

        #region CONSTRUCTOR

        public OldUserProfileSearchPage(INewNavigator navigator, OldUserProfileSearchPageObjects userProfileSearch, ISiteDriver siteDriver, IJavaScriptExecutors javaScriptExecutors, IEnvironmentManager environmentManager, IBrowserOptions browserOptions, IOracleStatementExecutor executor)
            : base(navigator, userProfileSearch, siteDriver, javaScriptExecutors, environmentManager, browserOptions, executor)
        {
            _userProfileSearch = (OldUserProfileSearchPageObjects)PageObject;
            _currentWindow = SiteDriver.CurrentWindowHandle;
            
        }

        #endregion

        #region PUBLIC METHODS

        public OldUserProfileSearchPage ClickAddUser()
        {
            var userProfileSearch = Navigator.Navigate<OldUserProfileSearchPageObjects>(() =>
                                                {
                                                    SiteDriver.FindElement(OldUserProfileSearchPageObjects.AddUserId,How.Id).Click();
                                                    Console.WriteLine("Click Add User");
                                                });
            return new OldUserProfileSearchPage(Navigator, userProfileSearch, SiteDriver,JavaScriptExecutor,EnvironmentManager,BrowserOptions,Executor);
        }

        public void ClickOnSearchButton()
        {
            SiteDriver.FindElement(OldUserProfileSearchPageObjects.SearchButtonId, How.Id).Click();
            Console.WriteLine("Clicked on Search Button.");
            WaitForGridToLoad();


        }

        //public ProfileManagerPage ClickOnUserName(string userID)
        //{
        //    SiteDriver.FindElement(string.Format(UserProfileSearchPageObjects.UserNameByUserIDXpath, userID), How.XPath).Click();
        //    Console.WriteLine("Clicked on User Name Link with user ID : {0}", userID);

        //    return new ProfileManagerPage(Navigator, new ProfileManagerPageObjects());
        //}

        //public ProfileManagerPage ClickOnFirstUserName()
        //{
        //    _userProfileSearch.UserNameLink.Click();
        //    Console.WriteLine("Clicked on User Name Link");
        //    return new ProfileManagerPage(Navigator, new ProfileManagerPageObjects());
        //}
        public void InsertFirstName(string firstName)
        {
            SiteDriver.WaitForCondition(() =>
                SiteDriver.IsElementPresent(OldUserProfileSearchPageObjects.NextButtonId, How.Id), 2000);
            SiteDriver.FindElement(OldUserProfileSearchPageObjects.FirstnameId, How.Id).ClearElementField();
            SiteDriver.FindElement(OldUserProfileSearchPageObjects.FirstnameId, How.Id).SendKeys(firstName);
            Console.WriteLine("Inserted First Name: {0}", firstName);
        }

        public void InsertLastName(string lastName)
        {
            SiteDriver.FindElement(OldUserProfileSearchPageObjects.LastnameId, How.Id).ClearElementField();
            SiteDriver.FindElement(OldUserProfileSearchPageObjects.LastnameId, How.Id).SendKeys(lastName);
            Console.WriteLine("Inserted Last Name: {0}", lastName);
        }

        public void InsertUserId(string userId)
        {
            SiteDriver.FindElement(OldUserProfileSearchPageObjects.UserId, How.Id).Click();
            SiteDriver.FindElement(OldUserProfileSearchPageObjects.UserId, How.Id).SendKeys(userId);
            Console.WriteLine("Inserted User Id: {0}", userId);
        }

        public void InsertPhoneNo(string phoneNo)
        {
            SiteDriver.FindElement(OldUserProfileSearchPageObjects.PhoneId, How.Id).Click();
            SiteDriver.FindElement(OldUserProfileSearchPageObjects.PhoneId, How.Id).SendKeys(phoneNo.Substring(0, 1));
            SiteDriver.FindElement(OldUserProfileSearchPageObjects.PhoneId, How.Id).SendKeys(phoneNo.Substring(1));
            Console.WriteLine("Inserted Phone No: {0}", phoneNo);
        }

        public void InsertEmailAddress(string emailAddress)
        {
            SiteDriver.FindElement(OldUserProfileSearchPageObjects.EmailAddressId, How.Id).Click();
            SiteDriver.FindElement(OldUserProfileSearchPageObjects.EmailAddressId, How.Id).SendKeys(emailAddress);
            Console.WriteLine("Inserted Email Address: {0}", emailAddress);
        }

        public void InsertPassword(string password)
        {
            SiteDriver.FindElement(OldUserProfileSearchPageObjects.PasswordId, How.Id).SendKeys(password);
            Console.WriteLine("Inserted Password: {0}", password);
        }

        public void InsertConfirmPassword(string password)
        {
            SiteDriver.FindElement(OldUserProfileSearchPageObjects.ConfirmPasswordId, How.Id).SendKeys(password);
            Console.WriteLine("Inserted Confirm Password: {0}", password);
        }

        public void SelectUserType(string userType)
        {

            SelectAndSearchOnDropDownBy("User Type", OldUserProfileSearchPageObjects.UserTypeId, userType);

            //_userProfileSearch.UserTypeInput.Click();
            //
            //SiteDriver.FindElement(string.Format(UserProfileSearchPageObjects.UserTypeXPathTemplate, userType), How.XPath).Click();
            //_userProfileSearch.UserTypeInput.Escape();
            //Console.WriteLine("Selected User Type: {0}", userType);
        }


        /// <summary>
        /// Select and search by drop down
        /// </summary>
        /// <param name="searchFilterName"></param>
        /// <param name="selector"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private void SelectAndSearchOnDropDownBy(string searchFilterName, string selector, string value, bool clickOnBody = false)
        {
            Console.WriteLine("Searching for  {0} : {1}", searchFilterName, value);
            SiteDriver.FindElement(selector, How.Id).ClearElementField();
            SiteDriver.FindElement(selector, How.Id).SendKeys(value + Keys.Escape);

            if (clickOnBody)
            {

                ClickOnPopUpTitle();
                SiteDriver.WaitToLoadNew(2000);
            }
            WaitForChrome();

        }

        public void ClickOnPopUpTitle()
        {
            SiteDriver.FindElement(OldUserProfileSearchPageObjects.AddUserPopUpTitle, How.CssSelector).Click();
        }
        /// <summary>
        /// Select and search by drop down
        /// </summary>
        /// <param name="searchFilterName"></param>
        /// <param name="selector"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private void SelectOnDropDownBy(string searchFilterName, string selector, string value)
        {
            Console.WriteLine("Searching for  {0} : {1}", searchFilterName, value);
            SiteDriver.FindElement(selector, How.Id).ClearElementField();
            SiteDriver.FindElement(selector, How.Id).SendKeys(value);
            SiteDriver.FindElement<InputButton>(selector, How.Id).ArrowDown();
            SiteDriver.FindElement(selector, How.Id).Click();

        }


        private void WaitForChrome(int milliSecondsTimeout = 000)
        {
            if (string.Compare(BrowserConstants.Chrome, EnvironmentManager.Browser, StringComparison.OrdinalIgnoreCase) == 0)
                SiteDriver.WaitToLoadNew(milliSecondsTimeout);
        }

        public void SelectUserStatus(string userStatus)
        {
            SelectAndSearchOnDropDownBy("User Status", OldUserProfileSearchPageObjects.UserStatusId, userStatus);
        }

        public void ClickNextButton(bool wait = false)
        {
            SiteDriver.WaitForCondition(() =>
                SiteDriver.IsElementPresent(OldUserProfileSearchPageObjects.NextButtonId, How.Id), 3000);
            SiteDriver.FindElement(OldUserProfileSearchPageObjects.NextButtonId, How.Id).Click();
            if (wait)
            {
                SiteDriver.WaitForCondition(() => !SiteDriver.IsElementPresent(OldUserProfileSearchPageObjects.NextButtonId, How.Id), 3000);
                SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(OldUserProfileSearchPageObjects.NextButtonId, How.Id), 5000);
            }

            Console.WriteLine("Clicked Next Button");
        }

        public void SelectAvailableClientAndPrivilege(string client)
        {
            SiteDriver.WaitToLoadNew(3000);
            SiteDriver.WaitForCondition(() =>
                SiteDriver.IsElementPresent(OldUserProfileSearchPageObjects.WizardBackButtonXpath, How.XPath), 3000);
            SiteDriver.FindElement(
                string.Format(OldUserProfileSearchPageObjects.AvailableClientXPathTemplate, client), How.XPath).Click();
            SiteDriver.FindElement(OldUserProfileSearchPageObjects.RightArrowButtonCssSelector, How.CssSelector).Click();
            SiteDriver.WaitForCondition(() => !SiteDriver.IsElementPresent(OldUserProfileSearchPageObjects.RadAjaxPanelDivCssLocator, How.CssSelector));
            Console.WriteLine("Selected Client: {0}", client);
        }

        public void SelectDefaultClient(string client, bool clickElseWhere = false)
        {
            SiteDriver.WaitToLoadNew(3000);
            SiteDriver.WaitForCondition(() =>
                SiteDriver.IsElementPresent(OldUserProfileSearchPageObjects.DefaultClientId, How.Id), 3000);
            SelectAndSearchOnDropDownBy("Default Client", OldUserProfileSearchPageObjects.DefaultClientId, client, clickElseWhere);
        }
        public List<string> GetDefaultPageList(bool sendTabKey = false)
        {
            SiteDriver.FindElement(OldUserProfileSearchPageObjects.UserpreferenceDefaultPageById, How.Id)
                .Click();

            var elements1 = JavaScriptExecutor.FindElements(OldUserProfileSearchPageObjects.UserPreferenceDefaultPage, "Text");
            SiteDriver.FindElement(OldUserProfileSearchPageObjects.UserpreferenceDefaultPageById, How.Id)
                .Click();
            SiteDriver.FindElement(OldUserProfileSearchPageObjects.UserpreferenceDefaultPageById, How.Id)
                .SendKeys(OpenQA.Selenium.Keys.Tab);
            return elements1;
        }

        public void WaitForDefaultPageRefreshWhenClientChanged(string pageName = "Batch Search")
        {
            SiteDriver.WaitForCondition(() =>
                {
                    SiteDriver.FindElement(OldUserProfileSearchPageObjects.UserpreferenceDefaultPageById,
                            How.Id)
                        .Click();
                    var elements1 =
                        JavaScriptExecutor.FindElements(OldUserProfileSearchPageObjects.UserPreferenceDefaultPage);
                    SiteDriver.FindElement(OldUserProfileSearchPageObjects.UserpreferenceDefaultPageById,How.Id).SendKeys(Keys.Tab);
                    return !elements1.Contains(pageName);
                }, 10000
            );
        }

        public void SelectClient(string client)
        {
            SelectOnDropDownBy("Default Client", OldUserProfileSearchPageObjects.DefaultClientId, client);
        }
        public void SwitchToCreateUserFrame()
        {
            SiteDriver.SwitchFrame(UserProfileSearchPageObjects.AddUserPopupByCss);
            SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(UserProfileSearchPageObjects.NextButtonId, How.Id), 5000);
        }

        public string GetErrorLabel()
        {
            return SiteDriver.FindElement(OldUserProfileSearchPageObjects.ErrorLabelId, How.Id).Text;
        }

        public bool IsErrorDivPresent()
        {
            return SiteDriver.FindElement(OldUserProfileSearchPageObjects.PopUpErrorDivId, How.Id).Displayed; 
        }

        public void ClickBackButtonInWizard()
        {
            SiteDriver.FindElement(OldUserProfileSearchPageObjects.WizardBackButtonXpath, How.Id).Click();
        }

        public bool IsDefaultDashboardViewLabelPresent()
        {
            SiteDriver.WaitToLoadNew(3000);
            return SiteDriver.IsElementPresent(OldUserProfileSearchPageObjects.DefaultDashboardView, How.Id);
        }

        public List<string> GetDashboardViewProductType()
        {
            SiteDriver.WaitToLoadNew(3000);
            SiteDriver.WaitForCondition(() =>
                SiteDriver.IsElementPresent(OldUserProfileSearchPageObjects.DashboardProductType, How.Id), 3000);
            JavaScriptExecutor.ExecuteClick(OldUserProfileSearchPageObjects.DashboardViewToggleIcon, How.Id);
            SiteDriver.WaitToLoadNew(200);
            var list = JavaScriptExecutor.FindElements(OldUserProfileSearchPageObjects.DashboardViewDropdownOptionsListXpath, How.XPath, "Text");
            return list;

        }

        public void SetPrivilegeAuthorityType(string authorityLabel, string authorityType)
        {
            SiteDriver.WaitToLoadNew(3000);
            SiteDriver.WaitForCondition(() =>
                SiteDriver.IsElementPresent(string.Format(OldUserProfileSearchPageObjects.AuthorityToggleIcon, authorityLabel), How.XPath), 3000);
            JavaScriptExecutor.ExecuteClick(string.Format(OldUserProfileSearchPageObjects.AuthorityToggleIcon, authorityLabel), How.XPath);
            JavaScriptExecutor.ExecuteClick(string.Format(OldUserProfileSearchPageObjects.SelectDashboardAuthorityXpathTemplate, authorityType), How.XPath);
            Console.WriteLine("Authority Type Selected: <{0}>", authorityType);
        }

        public void SetDefaultDashboardView(string productType)
        {
            JavaScriptExecutor.ExecuteClick(OldUserProfileSearchPageObjects.DashboardViewToggleIcon, How.Id);
            JavaScriptExecutor.ExecuteClick(string.Format(OldUserProfileSearchPageObjects.DashboardViewDropdownOptionsXpath, productType), How.XPath);
            Console.WriteLine("Default Dashboard View Selected: <{0}>", productType);
        }

        public void RemoveAssignedAuthority(string authorityLabel)
        {
            SiteDriver.WaitToLoadNew(3000);
            SiteDriver.WaitForCondition(() =>
                SiteDriver.IsElementPresent(string.Format(OldUserProfileSearchPageObjects.AssignedPrivilegeXpathTemplate, authorityLabel), How.XPath), 3000);
            SiteDriver.FindElement(string.Format(OldUserProfileSearchPageObjects.AssignedPrivilegeXpathTemplate, authorityLabel), How.XPath).Click();
            SiteDriver.FindElement(OldUserProfileSearchPageObjects.LeftArrowButtonCssSelector,How.CssSelector).Click();
            SiteDriver.WaitForCondition(() => !SiteDriver.IsElementPresent(OldUserProfileSearchPageObjects.RadAjaxPanelDivCssLocator, How.CssSelector));
            Console.WriteLine("Removed Dashboard authority");

        }

        public void RemoveAssignedRole()
        {
            SiteDriver.WaitToLoadNew(3000);
            SiteDriver.WaitForCondition(() =>
                SiteDriver.IsElementPresent(OldUserProfileSearchPageObjects.AssignedRoleXpathTemplate, How.XPath), 3000);
            SiteDriver.FindElement(OldUserProfileSearchPageObjects.AssignedRoleXpathTemplate, How.XPath).Click();
            SiteDriver.FindElement(OldUserProfileSearchPageObjects.LeftArrowButtonCssSelector,How.CssSelector).Click();
            SiteDriver.WaitForCondition(() => !SiteDriver.IsElementPresent(OldUserProfileSearchPageObjects.RadAjaxPanelDivCssLocator, How.CssSelector));
            Console.WriteLine("Removed assigned role");

        }

        public string GetAttributeValue()
        {
            return SiteDriver.FindElement(OldUserProfileSearchPageObjects.DashboardProductType, How.Id)
                .GetAttribute("value");
        }

        public bool IsDashboardViewPresentInPreferncesSection()
        {
            SiteDriver.WaitForCondition(() =>
                SiteDriver.IsElementPresent(OldUserProfileSearchPageObjects.PreferencesId, How.Id), 3000);
            return SiteDriver.IsElementPresent(OldUserProfileSearchPageObjects.DashboardInPreferencesSectionXpath, How.XPath);
        }

        public void CloseCreateWindow()
        {

            SiteDriver.SwitchWindow(_currentWindow);
            if (SiteDriver.IsElementPresent(OldUserProfileSearchPageObjects.CloseWindowCssSelector, How.CssSelector))
                SiteDriver.FindElement(OldUserProfileSearchPageObjects.CloseWindowCssSelector, How.CssSelector).Click();
           // _userProfileSearch.CloseWindow.ClickJS();
        }

        public void CloseWarningMessageWindowAndCloseCreateWindow()
        {
            JavaScriptExecutor.CloseFrame(OldUserProfileSearchPageObjects.CreateUserFrameName);
            SiteDriver.FindElement("rwCloseButton", How.ClassName).Click();
            //SiteDriver.SwitchWindow(_currentWindow);
            //SiteDriver.FindElement("rwCloseButton", How.ClassName).Click();
        }

        public void SwitchToPopUpErrorFrame()
        {
            SiteDriver.SwitchFrame("PopErrorsWindow");
        }

        public void WaitForGridToLoad()
        {
            SiteDriver.WaitForAjaxToLoad(OldUserProfileSearchPageObjects.IsRequestInProgress);
        }

        public List<string> GetAvailableRestrictionsListByLabel(string label)
        {
            return JavaScriptExecutor.FindElements(
                string.Format(OldUserProfileSearchPageObjects.AvailableAcessListXpathTemplate, label), How.XPath,
                "Text");

        }
        public List<string> GetAssignedRestrictionsListByLabel(string label)
        {
            return JavaScriptExecutor.FindElements(
                string.Format(OldUserProfileSearchPageObjects.AssignedAccessListXpathTemplate, label), How.XPath,
                "Text");


        }

        public List<string> GetListValueByLabelXpath(string label)
        {
            return JavaScriptExecutor.FindElements(
                string.Format(OldUserProfileSearchPageObjects.GetListValuesByLabelXpath, label), How.XPath,
                "Text");
        }
        public bool IsSpanLabelNamePresent(string label)
        {
            return SiteDriver.IsElementPresent(
                string.Format(OldUserProfileSearchPageObjects.SpanLabelNameXPathSelector, label), How.XPath);
        }


        public bool IsLabelNamePresent(string label)
        {
            return SiteDriver.IsElementPresent(
                string.Format(OldUserProfileSearchPageObjects.LabelNameXPathSelector, label), How.XPath);
        }

        public void MoveOptionFromAvailableToAssigned(string label, string option, bool reverse = false)
        {


            SiteDriver.FindElement(
                string.Format(OldUserProfileSearchPageObjects.AvailabeOptionByLabelOptionTemplateXpath, label, option),
                How.XPath).Click();
            if (reverse)
                SiteDriver.FindElement(string.Format(OldUserProfileSearchPageObjects.GetAllArrowsByLabelXpathTemplate, label, 2), How.XPath).Click();
            else
                SiteDriver.FindElement(string.Format(OldUserProfileSearchPageObjects.GetAllArrowsByLabelXpathTemplate, label, 1), How.XPath).Click();

            SiteDriver.WaitForCondition(() => !SiteDriver.IsElementPresent(OldUserProfileSearchPageObjects.RadAjaxPanelDivCssLocator, How.CssSelector));


        }
        public void MoveAllValuesFromAssignedToAvailableByLabel(string label)
        {
            if ((GetAvailableRestrictionsListByLabel(label).Count != 0) && label == "Restricted Claims Access")
                SiteDriver.FindElement(string.Format(OldUserProfileSearchPageObjects.GetAllArrowsByLabelXpathTemplate, label, 4), How.XPath).Click();
            else
                SiteDriver.FindElement(string.Format(OldUserProfileSearchPageObjects.GetAllArrowsByLabelXpathTemplate, label, 4), How.XPath).Click();
        }

        public void MoveAllValuesFromAvailabelToAssignedByLabel(string label)
        {
            if ((GetAvailableRestrictionsListByLabel(label).Count != 0) && label == "Restricted Claims Access")
                SiteDriver.FindElement(string.Format(OldUserProfileSearchPageObjects.GetAllArrowsByLabelXpathTemplate, label, 3), How.XPath).Click();
            else
                SiteDriver.FindElement(string.Format(OldUserProfileSearchPageObjects.GetAllArrowsByLabelXpathTemplate, label, 3), How.XPath).Click();
            SiteDriver.WaitForCondition(() => !SiteDriver.IsElementPresent(OldUserProfileSearchPageObjects.RadAjaxPanelDivCssLocator, How.CssSelector));

        }

        public void SearchForUserByName(string fname, string lname)
        {
            InsertFirstName(fname);
            InsertLastName(lname);
            ClickOnSearchButton();
        }

        #endregion


        public string[] GetAllAvailableRolesList()
        {
            return JavaScriptExecutor.FindElements(
                OldUserProfileSearchPageObjects.AvailableRolesPrivilegesCssSelector, How.CssSelector, "Text").ToArray();
        }

        public void SelectRandomRolePrivilege()
        {
            MoveAllValuesFromAssignedToAvailableByLabel("Role Privileges");
            var rolePrivilege = GetAllAvailableRolesList();
            var r = new Random();
            var roleIndex = r.Next(0, rolePrivilege.Length - 1);
            MoveOptionFromAvailableToAssigned("Role Privileges", rolePrivilege[roleIndex]);

            //var values = Enum.GetValues(typeof(RoleEnum));
            //var random = new Random();
            //MoveOptionFromAvailableToAssigned("Role Privileges", ((RoleEnum)values.GetValue(random.Next(values.Length))).GetStringValue());
        }


    }
}

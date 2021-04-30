using Nucleus.Service.Support.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Nucleus.Service.PageObjects.Claim;
using Nucleus.Service.PageObjects.Default;
using Nucleus.Service.PageObjects.Settings.User;
using Nucleus.Service.PageServices.Base.Default;
using Nucleus.Service.SqlScriptObjects.Common_SQL;
using UIAutomation.Framework.Core.Navigation;
using UIAutomation.Framework.Database;
using Nucleus.Service.SqlScriptObjects.Settings;
using Nucleus.Service.SqlScriptObjects.User;
using Nucleus.Service.Support.Common.Constants;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Environment;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Elements;
using UIAutomation.Framework.Utils;
using static System.String;
using Extensions = Nucleus.Service.Support.Utils.Extensions;
using static System.String;

namespace Nucleus.Service.PageServices.Settings.User
{
    public class UserProfileSearchPage : NewDefaultPage
    {
        #region PRIVATE FIELDS

        private readonly GridViewSection _gridViewSection;
        private readonly SideBarPanelSearch _sideBarPanelSearch;
        private readonly NewPagination _pagination;
        private readonly string _originalWindow;
        private readonly SideWindow _sideWindow;

        #endregion

        #region CONSTRUCTOR

        public UserProfileSearchPage(INewNavigator navigator, UserProfileSearchPageObjects newuserprofilesearchpage, ISiteDriver siteDriver, IJavaScriptExecutors javaScriptExecutors, IEnvironmentManager environmentManager, IBrowserOptions browserOptions, IOracleStatementExecutor executor)
            : base(navigator, newuserprofilesearchpage, siteDriver, javaScriptExecutors, environmentManager, browserOptions, executor)
        {
            newuserprofilesearchpage = (UserProfileSearchPageObjects)PageObject;
            _gridViewSection = new GridViewSection(SiteDriver,JavaScriptExecutor);
            _sideBarPanelSearch = new SideBarPanelSearch(new CalenderPage(SiteDriver),SiteDriver,JavaScriptExecutor);
            _originalWindow = SiteDriver.CurrentWindowHandle;
            _sideWindow = new SideWindow(new CalenderPage(SiteDriver), SiteDriver, JavaScriptExecutor);
        }

        #endregion

        #region PROPERTIES

        public GridViewSection GetGridViewSection
        {
            get { return _gridViewSection; }
        }
        public NewPagination GetPagination
        {
            get { return _pagination; }
        }
        public SideWindow GetSideWindow
        {
            get { return _sideWindow; }
        }

        public string OriginalWindowHandle
        {
            get { return _originalWindow; }
        }

        public SideBarPanelSearch SideBarPanelSearch
        {
            get { return _sideBarPanelSearch; }
        }

        #endregion



        #region public methods

        #region New User Account

        public List<string> RandomlyClickOptionsInAvailableOrAssigned(List<string> listOfOptions, string availableOrAssignedHeaderLabel,
            int numberOfOptionsToBeClicked)
        {
            var listOfRandomlySelectedOptions = new List<string>();
            var rand = new Random();
            var count = 0;
            List<int> rows = Enumerable.Range(1, listOfOptions.Count).ToList();

            while (count < numberOfOptionsToBeClicked)
            {
                int randIndex = rand.Next(0, rows.Count - 1);

                try
                {
                    listOfRandomlySelectedOptions.Add(GetOptionNameInAvailableOrAssignedByRow(availableOrAssignedHeaderLabel,
                        randIndex + 1));
                    ClickOptionInAvailableOrAssignedByRow(availableOrAssignedHeaderLabel, randIndex + 1);
                    rows.RemoveAt(randIndex);
                }
                catch (Exception e)
                {

                }


                count++;
            }

            return listOfRandomlySelectedOptions;
        }

        public string GetOptionNameInAvailableOrAssignedByRow(string label, int row) =>
            SiteDriver.FindElement(
                Format(UserProfileSearchPageObjects.OptionsInAvailableOrAssignedByRow, label, row),
                How.XPath).Text;

        public void ClickOptionInAvailableOrAssignedByRow(string label, int row) =>
            SiteDriver.FindElement(Format(UserProfileSearchPageObjects.OptionsInAvailableOrAssignedByRow, label, row),
                How.XPath).Click();

        public bool IsCreateNewUserTabSelectedByTabName(string tabName) =>
            SiteDriver.FindElement(Format(UserProfileSearchPageObjects.CreateNewUserTab, tabName), How.CssSelector)
                .GetAttribute("class").Contains("is_selected");

        public void SetDataInClientsTabToCreateNewUser(List<string> clientsToBeAssigned, string defaultClient, List<string> accessToBeAssigned)
        {
            StringFormatter.PrintMessage("Assigning clients to the user");

            foreach (var client in clientsToBeAssigned)
                SiteDriver.FindElement(Format(UserProfileSearchPageObjects.SelectOptionToBeAssignedByNameXPath, "Available Clients", client),
                    How.XPath).Click();

            StringFormatter.PrintMessage("Selecting Default Client");
            GetSideWindow.SelectDropDownListValueByLabel("Default Client", defaultClient);

            StringFormatter.PrintMessage("Assigning accesses to the user");
            foreach (var access in accessToBeAssigned)
                SiteDriver.FindElement(Format(UserProfileSearchPageObjects.SelectOptionToBeAssignedByNameXPath, "Available Access", access),
                    How.XPath).Click();
        }

        public void ClickToMoveOptionsBetweenAvailableAndAssigned(string label, string role) =>
            SiteDriver.FindElement(Format(UserProfileSearchPageObjects.SelectOptionToBeAssignedByNameXPath, label, role),
                How.XPath).Click();

        public bool IsExtNoInputFieldPresent() =>
            SiteDriver.IsElementPresent(UserProfileSearchPageObjects.ExtNoCssLocator, How.CssSelector);


        public void SetExtNo(string extNo)
        {
            SiteDriver.FindElement(UserProfileSearchPageObjects.ExtNoCssLocator, How.CssSelector).SendKeys(extNo);
        }

        public string GetExtNo()
        {
            return SiteDriver.FindElement(UserProfileSearchPageObjects.ExtNoCssLocator, How.CssSelector)
                .GetAttribute("value");
        }

        public string GetExtAttribute(string attribute) =>
            SiteDriver.FindElementAndGetAttribute(UserProfileSearchPageObjects.ExtNoCssLocator, How.CssSelector,
                attribute);


        public bool IsNewUserAccountNextButtonPresent() =>
            SiteDriver.IsElementPresent(UserProfileSearchPageObjects.NextButtonCssLocator, How.CssSelector);


        public void ClickOnNextButton()
        {
            var element = SiteDriver.FindElement(UserProfileSearchPageObjects.NextButtonCssLocator, How.CssSelector);
            SiteDriver.ElementToBeClickable(element);
            SiteDriver.WaitToLoadNew(500);
            element.Click();
            WaitForWorking();
        }

        public void ClickOnPreviousButton()
        {
            SiteDriver.FindElement(UserProfileSearchPageObjects.PreviousButtonCssLocator, How.CssSelector).Click();
        }

        public void ClickOnCancelLink()
        {
            JavaScriptExecutor.FindElement(UserProfileSearchPageObjects.CancelLinkCssLocator).Click();
        }


        public void NavigateToCreateNewUser()
        {
            SiteDriver.FindElement(UserProfileSearchPageObjects.AddUserIconByCss, How.CssSelector).Click();
            WaitForWorkingAjaxMessage();
            if (SideBarPanelSearch.IsSideBarPanelOpen())
            {
                SideBarPanelSearch.ClickOnSideBarPanelIcon();
                WaitForWorkingAjaxMessage();
            }
        }

        public bool IsNewUserAccountFormPresent() =>
            SiteDriver.IsElementPresent(UserProfileSearchPageObjects.NewUserAccountFormXPath, How.XPath);

        public List<string> GetNewUserAccountTabsLabel() => SiteDriver.FindElementsAndGetAttribute("title",
            UserProfileSearchPageObjects.NewUserAccountTabsCssSelector, How.CssSelector);

        public List<string> GetProfileTabHeaders() => JavaScriptExecutor.FindElements(UserProfileSearchPageObjects.ProfileHeadersCssSelector, How.CssSelector, "Text");

        public string GetHeaderDescription()
        {
            return SiteDriver.FindElement(UserProfileSearchPageObjects.HeaderDescriptionCssLocator, How.CssSelector).Text;
        }

        public bool IsSummaryLabelPresent(string label)
        {
            return SiteDriver.IsElementPresent(Format(UserProfileSearchPageObjects.SummaryLabelXPath, label), How.XPath);
        }

        public string GetSummaryLabelValue(string label)
        {
            return SiteDriver.FindElement(Format(UserProfileSearchPageObjects.SummaryValueXPath, label), How.XPath).Text;
        }

        public bool IsPreviousButtonPresent()
        {
            return SiteDriver.IsElementPresent(UserProfileSearchPageObjects.PreviousButtonCssLocator,
                How.CssSelector);
        }

        public bool IsCancelButtonPresent() =>
            JavaScriptExecutor.IsElementPresent(UserProfileSearchPageObjects.CancelLinkCssLocator);


        public bool IsCreateAccountPresent()
        {
            return SiteDriver.IsElementPresent(UserProfileSearchPageObjects.CreateUserButtonXPath, How.XPath);
        }

        public void ClickOnCreateUserButton()
        {
            SiteDriver.FindElement(UserProfileSearchPageObjects.CreateUserButtonXPath, How.XPath).Click();
            WaitForWorking();
        }

        public bool IsCreateUserIconPresent()
        {
            return SiteDriver.IsElementPresent(UserProfileSearchPageObjects.CreateUserIconCss, How.CssSelector);
        }

        public bool IsSSOFlagSetToTrueInDatabase(string userId)
        {
            return Executor.GetSingleStringValue(Format(UserSqlScriptObjects.GetUsesSSOFlagValueFromDb, userId))
                .Equals("T");
        }

        public bool IsFederatedUserSetToTrueInDatabase(string userId)
        {
            return Executor.GetSingleStringValue(Format(UserSqlScriptObjects.GetIsFederatedFlagValueFromDb, userId))
                .Equals("T");
        }

        public List<string> GetExpectedDepartmentValuesFromTable()
        {
            return Executor.GetTableSingleColumn(Format(UserSqlScriptObjects.GetReferenceDepartments));
        }

        public int GetUserDetailsFromDb(string userId)
        {
            return Convert.ToInt32(Executor.GetSingleValue(Format(UserSqlScriptObjects.GetUserDetailsFromDb, userId)));
        }

        public void DeleteUserDetailsFromDb(string userId)
        {
            Executor.ExecuteQuery(Format(UserSqlScriptObjects.DeleteUserDetailsFromDb, userId));
        }

        public bool IsProfileHeaderPresent()
        {
            return SiteDriver.IsElementPresent(UserProfileSearchPageObjects.FormHeaderXPath, How.XPath);
        }

        #endregion

        public bool IsFormHeaderPresentByHeaderName(string formHeader) =>
            SiteDriver.IsElementPresent(Format(UserProfileSearchPageObjects.FormHeaderByHeaderNameXPath, formHeader), How.XPath);

        public string GetSystemDateFromDatabase() =>
            Executor.GetSingleStringValue(CommonSQLObjects.GetSystemDateFromDatabase);

        public bool IsRadioButtonPresentByLabel(string label) =>
            SiteDriver.IsElementPresent(
                Format(UserProfileSearchPageObjects.RadioButtonByLabelPresentXPathTemplate, label), How.XPath);

        public bool IsRadioButtonOnOffByLabel(string label, bool active = true)
        {
            return SiteDriver.FindElementAndGetAttribute(
                Format(UserProfileSearchPageObjects.RadioButtonByLabelXPathTemplate, label, active ? 1 : 2),
                How.XPath, "class").Contains("is_active");
        }


        public void ClickOnRadioButtonByLabel(string label, bool active = true)
        {
            JavaScriptExecutor.ExecuteClick(
                Format(UserProfileSearchPageObjects.RadioButtonByLabelXPathTemplate, label, active ? 1 : 2),
                How.XPath);
        }

        public bool IsRadioButtonDisabled(string label)
        {
           return SiteDriver.FindElement(string.Format(UserProfileSearchPageObjects.RadioButtonByLabelXPathTemplate, label,1), How.XPath).GetAttribute("Class").Contains("is_disabled");
        }

        public string GetInfoHelpTooltipByLabel(string label) =>
            JavaScriptExecutor.FindElement(Format(UserProfileSearchPageObjects.InfoHelpIconByLabelCssLocator, label)).GetAttribute("title");

        public string GetHeaderInfoHelpToolTipByLabel(string label) => JavaScriptExecutor.FindElement(Format(UserProfileSearchPageObjects.HeaderinfoHelpIconByLabelCssLocator, label)).GetAttribute("title");

        public string GetTabInfoHelpToolTipByLabel(string label) => JavaScriptExecutor.FindElement(Format(UserProfileSearchPageObjects.TabinfoHelpIconByLabelCssLocator, label)).GetAttribute("title");


        public void SelectDropDownListValueByLabel(string label, string value)
        {
            SiteDriver.WaitToLoadNew(300);
            var element = JavaScriptExecutor.FindElement(Format(UserProfileSearchPageObjects.InputFieldByLabelCssLocator, label));

            JavaScriptExecutor.ExecuteClick(element);
            SiteDriver.WaitToLoadNew(300);
            try
            {
                element.ClearElementField();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            JavaScriptExecutor.ClickJQuery(Format(UserProfileSearchPageObjects.DropDownOptionsListValueByLabel, label, value));
            SiteDriver.WaitToLoadNew(300);

            Console.WriteLine("<{0}> Selected in <{1}> ", value, label);
        }

        public string GenerateRandomTextBasedOnAcceptedValues(string acceptableValues, int length) =>
            Extensions.GetRandomString(acceptableValues, length);

        public string GetRandomPhoneNumber()
        {
            var randomPhoneNumber = Extensions.GetRandomPhoneNumber();
            while (randomPhoneNumber.StartsWith("0") || randomPhoneNumber.StartsWith("1"))
            {
                randomPhoneNumber = Extensions.GetRandomPhoneNumber();
            }

            return randomPhoneNumber;

        }

        #region User Profile Tab Side View

        public Dictionary<string, string> GetAllFormDataFromAllFieldsInUserProfileTab(List<string> labelList, bool internalUser = true)
        {
            Dictionary<string, string> dictOfValuesWithLabel = new Dictionary<string, string>();

            if (internalUser)
                labelList.Remove(UserProfileEnum.SubscriberID.GetStringValue());
            else
                labelList.Remove(UserProfileEnum.Department.GetStringValue());

            foreach (var label in labelList)
            {
                if (label.Contains("Phone") || label.Equals("Fax"))
                {
                    var phoneFaxNumber = GetInputFromPhoneFaxFields(label);
                    var extNumber = GetInputFromPhoneFaxFields(label, false);

                    dictOfValuesWithLabel.Add(label, $"{phoneFaxNumber};{extNumber}");
                }

                else
                    dictOfValuesWithLabel.Add(label, GetInputTextBoxValueByLabel(label));
            }
            return dictOfValuesWithLabel;
        }

        public void SetInputInPhoneFaxFields(string label, string phoneValue, string extValue)
        {
            var phoneField = SiteDriver.FindElement(Format(UserProfileSearchPageObjects.PhoneFaxInputByLabel, label), How.XPath);
            phoneField.ClearElementField();
            phoneField.SendKeys(phoneValue);

            var extField = SiteDriver.FindElement(Format(UserProfileSearchPageObjects.PhoneFaxExtInputByLabel, label), How.XPath);
            extField.ClearElementField();
            extField.SendKeys(extValue);
        }

        /// <summary>
        /// Returns the phone number if phone = true. Else, returns ext number.
        /// </summary>
        /// <param name="label"></param>
        /// <param name="phone"></param>
        /// <returns></returns>
        public string GetInputFromPhoneFaxFields(string label, bool phone = true)
        {
            var selector = phone
                ? Format(UserProfileSearchPageObjects.PhoneFaxInputByLabel, label)
                : Format(UserProfileSearchPageObjects.PhoneFaxExtInputByLabel, label);

            return SiteDriver.FindElementAndGetAttribute(selector, How.XPath, "value");
        }

        public string GetInputTextBoxValueByLabel(string label) =>
            JavaScriptExecutor.FindElement(Format(UserProfileSearchPageObjects.InputFieldByLabelCssLocator, label)).GetAttribute("value");

        public string GetInputTextBoxTypeByLabel(string label) =>
            JavaScriptExecutor.FindElement(Format(UserProfileSearchPageObjects.InputFieldByLabelCssLocator, label)).GetAttribute("type");

        public List<string> GetDropDownListForUserProfileSettingsByLabel(string label)
        {
            JavaScriptExecutor.FindElement(Format(UserProfileSearchPageObjects.InputFieldByLabelCssLocator, label)).Click();
            var list = JavaScriptExecutor.FindElements(Format(UserProfileSearchPageObjects.DropDownOptionListByLabel, label), "Text");
            JavaScriptExecutor.FindElement(Format(UserProfileSearchPageObjects.InputFieldByLabelCssLocator, label)).Click();
            return list;
        }

        public bool IsLabelPresent(string label) =>
            SiteDriver.IsElementPresent(Format(UserProfileSearchPageObjects.LabelXPath, label), How.XPath);

        public void ClickOnGridRowByUserIdToOpenUserSettingSideView(string userId)
        {
            GetGridViewSection.ClickOnGridRowByValue(userId);
            WaitForWorking();
        }

        public void ClickOnEditIcon()
        {
            SiteDriver.FindElement(UserProfileSearchPageObjects.EditIconCssLocator, How.CssSelector).Click();
            WaitForStaticTime(500);
        }

        public bool IsAllTextBoxDisabled()
        {
            var result = SiteDriver.FindElements(UserProfileSearchPageObjects.AllTextBoxXPath, How.XPath)
                .Select(x => x.Enabled).ToList();
            return result.All(c => c == false) && result.Count > 0;
        }

        public List<string> GetUserSettingsTabList() =>
            JavaScriptExecutor.FindElements(UserProfileSearchPageObjects.UserSettingsTabListCssLocator, How.CssSelector, "Text");

        public string GetSelectedUserSettingTab() =>
             SiteDriver.FindElement(UserProfileSearchPageObjects.SelectedUserSettingTabCssLocator, How.CssSelector).Text;

        public void ClickOnUserSettingTabByTabName(string tabName)
        {
            SiteDriver.FindElement(Format(UserProfileSearchPageObjects.UserSettingTabXPathByTabName, tabName),
                How.XPath).Click();
            WaitForStaticTime(1000);
        }

        public bool IsUserSettingsTabDisabled(string tabName) =>
            SiteDriver.FindElement(Format(UserProfileSearchPageObjects.UserSettingTabXPathByTabName, tabName),
                How.XPath).GetAttribute("class").Contains("is_disabled");

        public bool IsUserSettingsFormDisabled() =>
            JavaScriptExecutor.FindElement(UserProfileSearchPageObjects.UserSettingsFormFieldCssLocator)
                .GetAttribute("class").Contains("read_only");

        public bool IsUserSettingsFormPresent(bool wait = false)
        {
            if (wait)
            {
                SiteDriver.WaitForCondition(() => JavaScriptExecutor.IsElementPresent(UserProfileSearchPageObjects.UserSettingsFormFieldCssLocator), 10000);
            }
            return JavaScriptExecutor.IsElementPresent(UserProfileSearchPageObjects.UserSettingsFormFieldCssLocator);
        }

        public bool IsInputFieldByLabelDisabled(string label) =>
            !JavaScriptExecutor.FindElement(Format(UserProfileSearchPageObjects.InputFieldByLabelCssLocator, label)).Enabled;

        public bool IsInputFieldByLabelPresent(string label) =>
            JavaScriptExecutor.IsElementPresent(string.Format(UserProfileSearchPageObjects.InputFieldByLabelCssLocator, label));

        public string GetPlaceHolderText(string label)
        {
            return JavaScriptExecutor
                .FindElement(string.Format(UserProfileSearchPageObjects.InputFieldByLabelCssLocator, label))
                .GetAttribute("placeholder");
        }


        public void SetInputTextBoxValueByLabel(string label, string value)
        {
            WaitForStaticTime(1000);
            var element =
                JavaScriptExecutor.FindElement(Format(UserProfileSearchPageObjects.InputFieldByLabelCssLocator, label));
            element.ClearElementField();
            element.SendKeys(value);
        }

        #endregion

        #region Roles Tab

        public List<string> GetRoleNamesByUserTypeFromDb(string userType)
        {
            List<string> listOfAvailableRoles = new List<string>();

            switch (userType)
            {
                case "Internal":
                    listOfAvailableRoles = Executor.GetTableSingleColumn(Format(UserSqlScriptObjects.GetRoleNameByUserType, 2));
                    break;
                case "Client":
                    listOfAvailableRoles = Executor.GetTableSingleColumn(Format(UserSqlScriptObjects.GetRoleNameByUserType, 8));
                    break;
                default:
                    listOfAvailableRoles = Executor.GetTableSingleColumn(UserSqlScriptObjects.GetAllRoleName);
                    break;
            }
            return listOfAvailableRoles;
        }

        public List<string> GetRoleDesriptionFromDb(string userType)
        {
            List<string> listOfAvailableRoles = new List<string>();

            switch (userType)
            {
                case "Internal":
                    listOfAvailableRoles = Executor.GetTableSingleColumn(Format(UserSqlScriptObjects.GetRoleDesriptionByUserTypeFromDb, 2));
                    break;
                case "Client":
                    listOfAvailableRoles = Executor.GetTableSingleColumn(Format(UserSqlScriptObjects.GetRoleDesriptionByUserTypeFromDb, 8));
                    break;
                default:
                    listOfAvailableRoles = Executor.GetTableSingleColumn(UserSqlScriptObjects.GetAllRoleDesriptionFromDb);
                    break;
            }
            return listOfAvailableRoles;
        }

        #endregion

        public void CloseDatabaseConnection()
        {
            Executor.CloseConnection();
        }

        public void SearchUserByNameOrId(List<string> name, bool searchByUserId = false, bool searchByFirstNameLastName = false)
        {
            if (searchByUserId)
            {
                _sideBarPanelSearch.OpenSidebarPanel();
                _sideBarPanelSearch.SetInputFieldByLabel("User ID", name[0]);
                _sideBarPanelSearch.ClickOnFindButton();
                WaitForWorking();
            }
            else if (searchByFirstNameLastName)
            {
                _sideBarPanelSearch.OpenSidebarPanel();
                _sideBarPanelSearch.SetInputFieldByLabel("First Name", name[0]);
                _sideBarPanelSearch.SetInputFieldByLabel("Last Name", name[1]);
                _sideBarPanelSearch.ClickOnFindButton();
                WaitForWorking();
            }
        }

        public ProfileManagerPage ClickonUserNameToNavigateProfileManagerPage(string firstname, string lastname = " ")
        {
            var profileManager = Navigator.Navigate<ProfileManagerPageObjects>(() =>
            {
                _sideBarPanelSearch.OpenSidebarPanel();
                _sideBarPanelSearch.SetInputFieldByLabel("First Name", firstname);
                _sideBarPanelSearch.SetInputFieldByLabel("Last Name", lastname);
                _sideBarPanelSearch.ClickOnFindButton();
                WaitForWorking();
                //GetGridViewSection.ClickOnGridByRowCol(1,2);
                ClickOnUserFirstnameandlastname(firstname + " " + lastname);
                SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
                SiteDriver.WaitForPageToLoad();
                SiteDriver.WaitForCondition(
                    () => SiteDriver.IsElementPresent(ProfileManagerPageObjects.BackButtonCssSelector,
                        How.CssSelector));
            });
            return new ProfileManagerPage(Navigator, profileManager, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }

        public void ClickOnUserFirstnameandlastname(string name)
        {
            SiteDriver.FindElement
            (string.Format(UserProfileSearchPageObjects.UserNameInGridByXpath, name
                ), How.XPath).Click();
        }
        
        //public ProfileManagerPage ClickonFisrtUserNameToNavigateProfileManagerPage(string firstname, string lastname = " ")
        //{
        //    var profileManager = Navigator.Navigate<ProfileManagerPageObjects>(() =>
        //    {
        //       _sideBarPanelSearch.OpenSidebarPanel();
        //        _sideBarPanelSearch.SetInputFieldByLabel("First Name", firstname);
        //        _sideBarPanelSearch.SetInputFieldByLabel("Last Name", lastname);
        //        _sideBarPanelSearch.ClickOnFindButton();
        //        WaitForWorking();
        //        GetGridViewSection.ClickOnGridByRowCol(1, 2);
        //        SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
        //        SiteDriver.WaitForPageToLoad();
        //        SiteDriver.WaitForCondition(
        //            () => SiteDriver.IsElementPresent(ProfileManagerPageObjects.BackButtonCssSelector,
        //                How.CssSelector));
        //    });
        //    return new ProfileManagerPage(Navigator, profileManager);
        //}

        //public ProfileManagerPage ClickonUserNameToNavigateProfilemanagereUsingUserId(string userid)
        //{
        //    var profileManager = Navigator.Navigate<ProfileManagerPageObjects>(() =>
        //    {
        //        _sideBarPanelSearch.OpenSidebarPanel();
        //        _sideBarPanelSearch.SetInputFieldByLabel("User ID", userid);
        //        _sideBarPanelSearch.ClickOnFindButton();
        //        WaitForWorking();
        //        GetGridViewSection.ClickOnGridByRowCol(1, 2);
        //        SiteDriver.WaitForCondition(() => !IsWorkingAjaxMessagePresent());
        //        SiteDriver.WaitForPageToLoad();
        //        SiteDriver.WaitForCondition(
        //            () => SiteDriver.IsElementPresent(ProfileManagerPageObjects.BackButtonCssSelector,
        //                How.CssSelector));
        //    });
        //    return new ProfileManagerPage(Navigator, profileManager);
        //}
        
        public List<List<string>> GetPrimaryDataResultFromDatabase(string firstname)
        {
            var infoList = new List<List<string>>();
            var list = Executor.GetCompleteTable(string.Format(UserSqlScriptObjects.PrimaryDataForUserProfileSearch, firstname));

            foreach (DataRow row in list)
            {
                var t = row.ItemArray.Select(x => x.ToString()).ToList();
                infoList.Add(t);
            }

            return infoList;
        }

        public List<string> GetLatestAuditFromRoleAuditTableByUserId(string userId)
        {
            var roleAuditList = new List<List<string>>();
            var list = Executor.GetTableSingleColumn(Format(UserSqlScriptObjects.GetAssignedRolesToUserFromAuditTable,
                userId));

            return list;
        }

        public List<List<string>> GetLatestAuditInfoFromTableByUserId(string userId)
        {
            var roleAuditList = new List<List<string>>();
            var list = Executor.GetCompleteTable(Format(UserSqlScriptObjects.GetLatestAuditFromRoleAuditTableByUserId, userId));

            foreach (DataRow row in list)
            {
                var t = row.ItemArray.Select(x => x.ToString()).ToList();
                roleAuditList.Add(t);
            }

            return roleAuditList;
        }

        //public UserProfileSearchPage NavigatetoCreateUserPage(bool handlePopup = false)
        //{
        //    var userProfileSearch = Navigator.Navigate<UserProfileSearchPageObjects>(() =>
        //        {
        //            ClickOnAddUserIcon();
        //            SiteDriver.WaitForCondition(JavaScriptExecutor.ExecuteJqueryStatus);
        //            SiteDriver.WaitForCondition(() => !SiteDriver.IsElementPresent(NewClaimActionPageObjects.SpinnerCssLocator, How.CssSelector));
        //            SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(NewClaimActionPageObjects.ClaimDetailsCssLocator, How.CssSelector));
        //        }

        //    );
        //    return new UserProfileSearchPage(Navigator, userProfileSearch);
        //}

        //public UserProfileSearchPage ClickAddUserAndNavigatetoCreateUserPage()
        //{
        //    var userProfileSearch = Navigator.Navigate<UserProfileSearchPageObjects>(() =>
        //    {
        //        ClickOnAddUserIcon();
        //        Console.WriteLine("Click Add User");
        //        SiteDriver.SwitchWindowByUrl("CreateNewUser");
        //    });
        //    return new UserProfileSearchPage(Navigator, userProfileSearch);
        //}

        public List<string> GetUserIdForpartialMatchFromDatabase(string firstname, string lastname)
        {
            return Executor.GetTableSingleColumn(string.Format(UserSqlScriptObjects.UserDetailUsingPartialMatch,
                firstname, lastname));
        }

        public List<string> GetUserByActivestatus()
        {
            return Executor.GetTableSingleColumn(String.Format(UserSqlScriptObjects.UsersByActiveStatus));
        }

        public List<string> GetUserByInactivestatus()
        {
            return Executor.GetTableSingleColumn(String.Format(UserSqlScriptObjects.UsersByInactiveStatus));
        }

        public string GetLoadMoreText()
        {
            return GetGridViewSection.GetLoadMoreText();
        }

        public bool IsActivestatusIconPresent()
        {
            return SiteDriver.IsElementPresent(UserProfileSearchPageObjects.ActiveUserCssLocator, How.CssSelector);
        }

        public bool IsActiveIconPresentInAllList()
        {
            var list = SiteDriver.FindElementsAndGetAttributeByClass("active_icon",
                UserProfileSearchPageObjects.IconValueListCssLocator, How.CssSelector);
            return list.All(x => x.Equals(true));
        }

        public bool IsInActiveIconPresentInAllList()
        {
            var list = SiteDriver.FindElementsAndGetAttributeByClass("inactive_icon",
                UserProfileSearchPageObjects.IconValueListCssLocator, How.CssSelector);
            return list.All(x => x.Equals(true));
        }

        public bool IsLockIconPresentInAllList()
        {
            var list = SiteDriver.FindElementsAndGetAttributeByClass("lock",
                UserProfileSearchPageObjects.IconValueListCssLocator, How.CssSelector);
            return list.All(x => x.Equals(true));
        }

        public bool IsFrozenIconPresentInAllList()
        {
            var list = SiteDriver.FindElementsAndGetAttributeByClass("frozen_icon",
                UserProfileSearchPageObjects.IconValueListCssLocator, How.CssSelector);
            return list.All(x => x.Equals(true));
        }

        public bool IsInactivestatusIconPresent()
        {
            return SiteDriver.IsElementPresent(UserProfileSearchPageObjects.InactiveuserCssLocator, How.CssSelector);
        }

        public bool IsLockedstatusIconPresent()
        {
            return SiteDriver.IsElementPresent(UserProfileSearchPageObjects.LockedUserCssLocator, How.CssSelector);
        }

        public bool IsFrozenstatusIconPresent()
        {
            return SiteDriver.IsElementPresent(UserProfileSearchPageObjects.FrozenUserCssLocator, How.CssSelector);
        }

        public bool IsLoadMoreLinkable()
        {
            return GetGridViewSection.IsLoadMorePresent();
        }

        public void UpdateUserStatusToFrozen(string userid)
        {
            Executor.ExecuteQuery(String.Format(UserSqlScriptObjects.UpdateUserToFrozenStatus, userid));
        }

        public void RevertFrozenstatusForUser(string userid)
        {
            Executor.ExecuteQuery(string.Format(UserSqlScriptObjects.RevertUserFrozenStatus, userid));
            Console.WriteLine("Revert user_id: {0} to active state from frozen status", userid);
        }

        public void UpdateUserToLockedStatus(string userid)
        {
            Executor.ExecuteQuery(string.Format(UserSqlScriptObjects.UpdateUserToLockedStatus, userid));
            Console.WriteLine("Set user_id: {0} to locked state", userid);
        }

        public void RevertUserLockedStatus(string userid)
        {
            Executor.ExecuteQuery(string.Format(UserSqlScriptObjects.RevertUserLockedStatus, userid));
            Console.WriteLine("Unlock user_id: {0} to active state", userid);
        }

        public void RevertUserToActiveStatus(string userid)
        {
            Executor.ExecuteQuery(string.Format(UserSqlScriptObjects.RevertUserToActiveStatus, userid));
            Console.WriteLine("Set user_id {0} to active status", userid);
        }

        public bool IsCreateUserPageTitlePresent()
        {
            // return SiteDriver.IsElementPresent(NewUserProfileSearchPageObjects.CreateUserTitleBarByCss, How.CssSelector);
            return JavaScriptExecutor.IsElementPresent(UserProfileSearchPageObjects.CreateUserTitleBarByCss);
        }

        public string GetCreateUserPopUptitle()
        {
            return SiteDriver.FindElement(UserProfileSearchPageObjects.CreateUserTitleBarByCss, How.CssSelector).Text;
        }

        public void HardDeleteUserIfExists(string userid)
        {
            Executor.ExecuteQuery(string.Format(SettingsSqlScriptObject.HardDeleteUserIfExists, userid));
        }

        public List<string> GetAvailableAccessForAccessForInternalUser()
        {
            return
                Executor.GetTableSingleColumn(UserSqlScriptObjects.AvailableAccessForAccessForInternalUser);
        }

        public List<string> GetAvailableAccessForAccessForClientUser()
        {
            return
                Executor.GetTableSingleColumn(UserSqlScriptObjects.AvailableAccessForAccessForClientUser);
        }

        public List<String> GetAvailableAccessListByClientFromDb(string client, int userType)
        {
            return Executor.GetTableSingleColumn(Format(SettingsSqlScriptObject.GetAccessListByClient,
                client, userType), 1);
        }

        public List<String> GetAvailableRestrictionListByClientListFromDb(List<string> client, int userType)
        {
            var clients = "'" + Join("','", client) + "'";
            return Executor.GetTableSingleColumn(Format(SettingsSqlScriptObject.GetAccessListByClientList,
                clients, userType), 1);
        }

        public bool IsCreateUserPopUpPresent()
        {
            try
            {
                SiteDriver.WaitForCondition(() =>
                SiteDriver.SwitchWindowByUrl("CreateNewUser"));
                var isCreateUserPopUp = IsCreateUserPageTitlePresent();
                return isCreateUserPopUp;
            }
            catch (Exception)
            {
                Console.WriteLine("Catch exception and close window");
                SiteDriver.SwitchWindow(_originalWindow);
            }
            return false;
        }

        public void SwitchToNewUserSearchPage(bool createuserPage = false)
        {
            if (createuserPage)
                SiteDriver.CloseWindow();
            SiteDriver.SwitchWindowByTitle(PageTitleEnum.UserProfileSearch.GetStringValue());
        }

        public bool IsAddUserIconPresent()
        {
            return SiteDriver.IsElementPresent(UserProfileSearchPageObjects.AddUserIconByCss, How.CssSelector);
        }

        //public void ClickOnAddUserIcon()
        //{
        //    //JavaScriptExecutor.ExecuteClick(NewUserProfileSearchPageObjects.AddUserIconByCss, How.CssSelector);
        //    SiteDriver.FindElement(NewUserProfileSearchPageObjects.AddUserIconByCss, How.CssSelector).Click();
        //    Console.WriteLine("Clicked on Create user button");
        //    SiteDriver.WaitForCondition(() => SiteDriver.WindowHandles.Count > 1, 500);                  
        //}

        public bool IsNextButtonPresent()
        {
            return SiteDriver.IsElementPresent(UserProfileSearchPageObjects.NextButtonId, How.CssSelector);
        }

        public List<string> GetAvailableAccessListByLabel(string label)
        {
            return JavaScriptExecutor.FindElements(
                string.Format(UserProfileSearchPageObjects.AvailableAcessListXpathTemplate, label), How.XPath,
                "Text");
        }

        public void MoveAllValuesFromAvailabelToAssignedByLabel(string label)
        {
            if ((GetAvailableAccessListByLabel(label).Count != 0) && label == "Restricted Claims Access")
                SiteDriver.FindElement(Format(UserProfileSearchPageObjects.GetAllArrowsByLabelXpathTemplate, label, 3), How.XPath).Click();
            else
                SiteDriver.FindElement(Format(UserProfileSearchPageObjects.GetAllArrowsByLabelXpathTemplate, label, 3), How.XPath).Click();
            SiteDriver.WaitForCondition(() => !SiteDriver.IsElementPresent(UserProfileSearchPageObjects.RadAjaxPanelDivCssLocator, How.CssSelector));
        }

        public void MoveAllValuesFromAssignedToAvailableByLabel(string label)
        {
            if ((GetAvailableAccessListByLabel(label).Count != 0) && label == "Restricted Claims Access")
                SiteDriver.FindElement(Format(UserProfileSearchPageObjects.GetAllArrowsByLabelXpathTemplate, label, 4), How.XPath).Click();
            else
                SiteDriver.FindElement(Format(UserProfileSearchPageObjects.GetAllArrowsByLabelXpathTemplate, label, 4), How.XPath).Click();
        }

        public void SelectRandomRolePrivilege()
        {
            MoveAllValuesFromAssignedToAvailableByLabel("Role Privileges");
            var values = Enum.GetValues(typeof(RoleEnum));
            var random = new Random();
            MoveOptionFromAvailableToAssigned("Role Privileges", ((RoleEnum)values.GetValue(random.Next(values.Length))).GetStringValue());
        }

        public List<string> GetAllAvailableRolesList()
        {
            return JavaScriptExecutor.FindElements(
                UserProfileSearchPageObjects.AvailableRolesPrivilegesXPath, How.XPath, "Text");
        }

        public List<string> GetAllAssignedRolesList() =>
            JavaScriptExecutor.FindElements(UserProfileSearchPageObjects.AssignedRolesPrivilegesXPath, How.XPath, "Text");

        public bool IsInfoIconPresentNextToAvailableRoles() =>
            JavaScriptExecutor.IsElementPresent(UserProfileSearchPageObjects.InfoIconAtSideOfAvailableRolesCssSelector);

        public List<string> GetAllInfoIconDescription()
        {
            var elements = JavaScriptExecutor.FindElements(UserProfileSearchPageObjects.InfoIconAtSideOfAvailableRolesCssSelector, "Attribute:title");
            return elements;
        }

        public void MoveOptionFromAvailableToAssigned(string label, string option, bool reverse = false)
        {
            SiteDriver.FindElement(
                string.Format(UserProfileSearchPageObjects.AvailabeOptionByLabelOptionTemplateXpath, label, option),
                How.XPath).Click();
            if (reverse)
                SiteDriver.FindElement(Format(UserProfileSearchPageObjects.GetAllArrowsByLabelXpathTemplate, label, 2), How.XPath).Click();
            else
                SiteDriver.FindElement(Format(UserProfileSearchPageObjects.GetAllArrowsByLabelXpathTemplate, label, 1), How.XPath).Click();

            SiteDriver.WaitForCondition(() => !SiteDriver.IsElementPresent(UserProfileSearchPageObjects.RadAjaxPanelDivCssLocator, How.CssSelector));
        }

        public int GetSearchResultCount()
        {
            return SiteDriver.FindElementsCount(UserProfileSearchPageObjects.UserProfileLinksOnSearchResult,
                How.CssSelector);
        }

        public void ClickOnFilterOptionListRow(int row)
        {
            ClickOnFilterOption();
            JavaScriptExecutor.ExecuteClick(Format(UserProfileSearchPageObjects.FilterOptionValueByCss, row)
                , How.CssSelector);
            Console.WriteLine("Click on {0} filter option", row);
            SiteDriver.WaitForPageToLoad();
            ClickOnFilterOption();
        }

        public void ClickOnFilterOption()
        {
            JavaScriptExecutor.ExecuteClick(UserProfileSearchPageObjects.FilterOptionsListCssLocator, How.CssSelector);
        }

        public void ClickOnClearSort()
        {
            ClickOnFilterOptionListRow(4);
            Console.WriteLine("Clicked on Clear sort option.");
        }

        public List<string> GetUserSettingsContainerList()
        {
            return JavaScriptExecutor.FindElements(UserProfileSearchPageObjects.UserSettingsContainerCssSelector, How.CssSelector,
                "Text");
        }

        public string GetUserSettingsContainerTitle(int n = 1)
        {
            return SiteDriver.FindElement(Format(UserProfileSearchPageObjects.UserSettingsContainerTitleCssSelector, n), How.CssSelector).Text;
        }

        public void ClickUserSettingsContainerByLabel(string label)
        {
            SiteDriver.FindElement(Format(UserProfileSearchPageObjects.UserSettingsContainerByLabelXPath, label), How.XPath).Click();
        }

        public List<string> GetNotificationslabelList()
        {
            return JavaScriptExecutor.FindElements(UserProfileSearchPageObjects.NotificationLabelXpathLocator, How.XPath,
                "Text");
        }

        public List<String> GetUserAssignedClientList(string userid)
        {
            return Executor.GetTableSingleColumn(Format(UserSqlScriptObjects.UserAssignedClientList, userid));
        }

        public string GetFullNameByUserId(string userid)
        {
            return Executor.GetSingleStringValue(Format(UserSqlScriptObjects.GetFullNameByUserID, userid));
        }

        public string GetClientListLabel(string side = "left_list")
        {
            return SiteDriver.FindElement(
                Format(UserProfileSearchPageObjects.ClientListLabelXPathTemplate, side), How.XPath).Text;
        }



        //public bool IsEditIconPresent(string tab)
        //{
        //    return SiteDriver.IsElementPresent(Format(NewUserProfileSearchPageObjects.ModifySettingsXPathTemplate, tab),
        //        How.XPath);
        //}

        public bool IsEditIconEnabled(string tab)
        {
            var classValue = SiteDriver.FindElementAndGetAttribute(Format(UserProfileSearchPageObjects.ModifySettingsXPathTemplate, tab),
                How.XPath, "class");
            if (classValue.Contains("is_disabled"))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public void ClickEditIconSettings(string tab)
        {
            SiteDriver.FindElement(Format(UserProfileSearchPageObjects.ModifySettingsXPathTemplate, tab), How.XPath).Click();
        }

        public void SelectDeselectAll(string label)
        {
            JavaScriptExecutor.FindElement(Format(UserProfileSearchPageObjects.SelectDeselectAllCssSelector, label)).Click();
        }



        public int SetAndGetLengthOfExtUserIdByClientName(string client)
        {
            var extUserIdElement = SiteDriver.FindElement(
                Format(UserProfileSearchPageObjects.ExtUserIdByClientNameXPathTemplate, client), How.XPath);
            var alphanumericValue = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_";
            var extId = GenerateRandomTextBasedOnAcceptedValues(alphanumericValue, 33);
            extUserIdElement.SendKeys(extId);
            return extUserIdElement.GetAttribute("value").Length;
        }

        public int GetLengthOfTheInputFieldByLabel(string client)
        {
            var element = SiteDriver.FindElement(Format(UserProfileSearchPageObjects.ExtUserIdByClientNameXPathTemplate, client), How.XPath).GetAttribute("value");
            var lengthOfInputField = element.Length;
            return lengthOfInputField;
        }

        //public void SaveorCancelUserSettings(bool Save)
        //{
        //    if (Save)
        //    {
        //        SiteDriver.FindElement(NewUserProfileSearchPageObjects.SaveButtonXPathTemplate, How.XPath)
        //            .Click();
        //    }
        //    else
        //    {
        //        SiteDriver.FindElement(NewUserProfileSearchPageObjects.CancelButtonXPathTemplate, How.XPath)
        //            .Click();
        //    }
        //}

        public string GetLastModifiedBy()
        {
            return SiteDriver
                .FindElement(UserProfileSearchPageObjects.LastModifiedXPathTemplate, How.XPath).Text;
        }

        public void DeleteAssignedClient(string userid, List<string> clientList)
        {
            Executor.ExecuteQuery(Format(UserSqlScriptObjects.DeleteAssignedUserClient, userid, clientList[0], clientList[1]));
        }

        public void RestoreSecurityAnswersInDatabase(string userid)
        {
            Executor.ExecuteQuery(string.Format(UserSqlScriptObjects.RestoreSecurityAnswers, userid));
            Console.WriteLine("Restore Security Answer of : {0} ", userid);
        }

        public void ClickOnShowAnswer(int n = 2)
        {
            SiteDriver.FindElement(Format(UserProfileSearchPageObjects.ShowAnswerCssSelector, n), How.CssSelector).Click();
        }

        public int ValidateAnswerInputBoxByLabel(string label)
        {
            var alphanumericValue = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_`~!@#$%^&*()-=+{}|<>?,./:";
            var answer = GenerateRandomTextBasedOnAcceptedValues(alphanumericValue, 101);
            SetInputTextBoxValueByLabel(label, answer);
            return GetInputTextBoxValueByLabel(label).Length;
        }

        public string GetTooltipmessageByLabel(string label)
        {
            return SiteDriver.FindElement(Format(UserProfileSearchPageObjects.TooltipXPathTemplate, label),
                How.XPath).GetAttribute("title");
        }

        public string GetDefaultClientOfUser(string userid)
        {
            var t = Executor.GetSingleStringValue(Format(UserSqlScriptObjects.GetDefaultClientOfUser, userid));
            return t;
        }

        public List<string> GetAvailabeQuestionListFromDb()
        {
            return Executor.GetTableSingleColumn(UserSqlScriptObjects.GetSecurityQuestionList);
        }

        public List<string> GetAssignedRoleIdsToUserByUserId(string userId) =>
            Executor.GetTableSingleColumn(Format(UserSqlScriptObjects.GetAssignedRoleIdsToUserByUserId, userId));

        public void DeleteLastSavedPasswordByUser(string userid)
        {
            Executor.ExecuteQuery(Format(UserSqlScriptObjects.DeleteLastSavedPasswordByUser, userid));
            Executor.ExecuteQuery(Format(UserSqlScriptObjects.DeleteLastSavedPasswordHistory, userid));
        }

        public int GetLastSavedPasswordByUser(string userid)
        {
            return Convert.ToInt32(Executor.GetSingleStringValue(Format(UserSqlScriptObjects.GetLastSavedPasswordAuditByUser, userid)));
        }



        #region Create New User

        public string GetLeftFormHeader() => SiteDriver
            .FindElement(UserProfileSearchPageObjects.LeftFormHeaderCssSelector, How.CssSelector).Text;

        public int  GetUserCountForEmailAndUserType(string userEmail)
        {
            return (int) Executor.GetSingleValue(
                    Format(SettingsSqlScriptObject.GetUserCountForUserTypeAndEmail, userEmail));
             
        }


        public bool DoesUserAlreadyExistWithSameEmail(string email)
        {
            if (GetUserCountForEmailAndUserType(email) >= 1)
                return true;
            else
            {
                return false;
            }
        }

        #endregion

        #endregion
    }
}

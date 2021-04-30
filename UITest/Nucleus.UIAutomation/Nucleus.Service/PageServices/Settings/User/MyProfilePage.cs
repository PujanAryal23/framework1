using Nucleus.Service.Support.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Nucleus.Service.PageObjects.Claim;
using Nucleus.Service.PageObjects.Default;
using Nucleus.Service.PageObjects.QuickLaunch;
using Nucleus.Service.PageObjects.Settings.User;
using Nucleus.Service.PageServices.Base.Default;
using Nucleus.Service.PageServices.QuickLaunch;
using Nucleus.Service.SqlScriptObjects.Common_SQL;
using UIAutomation.Framework.Core.Navigation;
using UIAutomation.Framework.Database;
using Nucleus.Service.SqlScriptObjects.Settings;
using Nucleus.Service.SqlScriptObjects.User;
using Nucleus.Service.Support.Common.Constants;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Environment;
using OpenQA.Selenium;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Elements;
using UIAutomation.Framework.Utils;
using static System.String;
using Extensions = Nucleus.Service.Support.Utils.Extensions;
using static System.String;

namespace Nucleus.Service.PageServices.Settings.User
{
    public class MyProfilePage : NewDefaultPage
    {

        #region PRIVATE FIELDS

        private readonly GridViewSection _gridViewSection;
        private readonly SideBarPanelSearch _sideBarPanelSearch;
        private readonly NewPagination _pagination;
        private readonly string _originalWindow;
        private readonly SideWindow _sideWindow;

        #endregion

        #region CONSTRUCTOR

        public MyProfilePage(INewNavigator navigator, MyProfilePageObjects myProfilePage, ISiteDriver siteDriver, IJavaScriptExecutors javaScriptExecutors, IEnvironmentManager environmentManager, IBrowserOptions browserOptions, IOracleStatementExecutor executor)
            : base(navigator, myProfilePage, siteDriver, javaScriptExecutors, environmentManager, browserOptions, executor)
        {
            myProfilePage = (MyProfilePageObjects)PageObject;
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

        #region PUBLIC METHODS

        public string GenerateRandomTextBasedOnAcceptedValues(string acceptableValues, int length) => Extensions.GetRandomString(acceptableValues, length);

        public string GetRandomPhoneNumber() => Extensions.GetRandomPhoneNumber();

        public void CloseDatabaseConnection()
        {
            Executor.CloseConnection();
        }

        public void ClickOnSaveButton()
        {
            JavaScriptExecutor.FindElement(MyProfilePageObjects.SaveButtonCssSelector).Click();
            WaitForWorking();
        }

        public void ClickOnCancelButton()
        {
            JavaScriptExecutor.FindElement(MyProfilePageObjects.CancelButtonCssSelector).Click();
            WaitForWorking();
        }

        public string GetExtValueByType(string extOfPhoneFaxOrAltPhone)
        {
            return JavaScriptExecutor.FindElement(Format(MyProfilePageObjects.ExtFieldByPhoneFaxOrAltPhoneCssSelector,
                extOfPhoneFaxOrAltPhone)).GetAttribute("value");
        }

        public void SetExtValueByType(string extOfPhoneFaxOrAltPhone, string value)
        {
            var element = JavaScriptExecutor.FindElement(Format(MyProfilePageObjects.ExtFieldByPhoneFaxOrAltPhoneCssSelector,
                extOfPhoneFaxOrAltPhone));
            element.ClearElementField();
            element.SendKeys(value);
        }

        public void RevertUserInfoInProfileAndPreferences(string userId, bool isInternalUser = true)
        {
            var query = isInternalUser
                ? UserSqlScriptObjects.RevertUserInfoInProfileAndPreferencesForInternalUser
                : UserSqlScriptObjects.RevertUserInfoInProfileAndPreferencesForClientUser;

            Executor.ExecuteQuery(Format(query, userId));
        }

        public List<string> GetExpectedDepartmentValuesFromTable()
        {
            return Executor.GetTableSingleColumn(Format(UserSqlScriptObjects.GetReferenceDepartments));
        }


        /// <summary>
        /// Fills all the data in 'Profile & Preferences' tab in My Profile page. The label for ext fields should be gives in a hyphen-separated manner.
        /// For instance, ext for 'Phone Number' should be passed as 'Phone Number-Ext'.
        /// </summary>
        /// <param name="formDataToBeFilled"></param>
        /// <param name="defaultPage"></param>
        /// <param name="defaultClient"></param>
        /// <param name="isTooltipsChecked"></param>
        /// <param name="isPatientClaimHxChecked"></param>
        public void FillAllInputFieldsInProfileAndPreferencesTabInMyProfile(IDictionary<string, string> formDataToBeFilled, string defaultPage, string defaultClient, bool isTooltipsChecked,
            bool isPatientClaimHxChecked, bool isInternalUser = true, string department="")
        {
            foreach (var key in formDataToBeFilled.Keys)
            {
                var formLabel = key;

                if (formLabel.Contains('-'))
                {
                    formLabel = key.Split('-')[0];
                    SetExtValueByType(formLabel, formDataToBeFilled[key]);
                    continue;
                }
                
                GetSideWindow.SetInputInInputFieldByLabel(formLabel, formDataToBeFilled[key]);
            }

            GetSideWindow.SelectDropDownListValueByLabel("Default Page", defaultPage, false);
            GetSideWindow.SelectDropDownListValueByLabel("Default Client", defaultClient, false);
            GetSideWindow.ClickOnCheckBoxByLabel("Enable Tooltips on Claim Action");

            if (isInternalUser)
            {
                GetSideWindow.SelectDropDownListValueByLabel("Department", department, false);
                GetSideWindow.ClickOnCheckBoxByLabel("Automatically display Patient Claim History on Claim Action");
            }
                
        }          
       
        public string GetUserNameOfClient(string userid)
        {
            return Executor.GetSingleStringValue(Format(UserSqlScriptObjects.GetFullNameByUserID, userid));
        }

        public List<string> GetAssignedRoleIdsToUserByUserId(string userId) =>
            Executor.GetTableSingleColumn(Format(UserSqlScriptObjects.GetAssignedRoleIdsToUserByUserId, userId));

        public List<string> GetAssignedClientListForUser(string userId) =>
            GetCommonSql.GetAssignedClientListForUser(userId);

        public int GetLastSavedPasswordByUser(string userid)
        {
            return Convert.ToInt32(Executor.GetSingleStringValue(Format(UserSqlScriptObjects.GetLastSavedPasswordAuditByUser, userid)));
        }

        public void DeleteLastSavedPasswordByUser(string userid)
        {
            Executor.ExecuteQuery(Format(UserSqlScriptObjects.DeleteLastSavedPasswordByUser, userid));
            Executor.ExecuteQuery(Format(UserSqlScriptObjects.DeleteLastSavedPasswordHistory, userid));
        }

        public int GetLatestPasswordCountFromDb(string userId)
        {
            return Executor.GetCompleteTable(Format(UserSqlScriptObjects.SelectPasswordDetail, userId)).Count();
        }

        public void DeleteLatestPasswordFromDb(string userId)
        {
            Executor.ExecuteQuery(Format(UserSqlScriptObjects.DeleteLatestPasswordFromDb, userId));
        }
        public void ClickOnTabByName(string tabName)
        {
            SiteDriver.FindElement(Format(MyProfilePageObjects.ProfileTabXPath, tabName), How.XPath).Click();
        }

        public bool IsDropDownPresentByLabel(string label)
        {
            return SiteDriver.IsElementPresent(Format(MyProfilePageObjects.DropdownByLabelXPath, label), How.XPath);
        }

        public bool IsTextBoxPresentByLabel(string section, string label)
        {
            return SiteDriver.IsElementPresent(Format(MyProfilePageObjects.TextBoxByLabelXPath, section, label),
                How.XPath);
        }

        public bool IsSectionSelectedByTabName(string tabName)
        {
            return SiteDriver.FindElement(Format(MyProfilePageObjects.ProfileTabXPath, tabName), How.XPath)
                .GetAttribute("class").Contains("is_selected");
        }

        public void SetInputTextBoxValueByLabel(string section, string label, string value)
        {
            WaitForStaticTime(1000);
            var element = section == "Change Password" ? SiteDriver.FindElement(Format(MyProfilePageObjects.TextBoxByLabelXPath, section, label), How.XPath) : SiteDriver.FindElement(Format(MyProfilePageObjects.TextBoxByQuestionLabelXPath, section, label), How.XPath);
            element.ClearElementField();
            element.SendKeys(value);
        }

        public List<string> GetAvailabeQuestionListFromDb()
        {
            return Executor.GetTableSingleColumn(UserSqlScriptObjects.GetSecurityQuestionList);
        }
        
        public List<string> GetDropDownListForUserProfileSettingsByLabel(string label)
        {
            SiteDriver.FindElement(Format(MyProfilePageObjects.DropdownByLabelXPath, label), How.XPath).Click();
            var list = JavaScriptExecutor.FindElements(Format(MyProfilePageObjects.DropdownListByLabelXPath, label), How.XPath, "Text").OrderBy(q=>q).ToList();
            SiteDriver.FindElement(Format(MyProfilePageObjects.DropdownByLabelXPath, label), How.XPath).Click();
            list.Remove("");
            return list;
        }

        public void SelectDropDownListValueByLabel(string label, string value)
        {
            SiteDriver.WaitToLoadNew(300);
            var element = SiteDriver.FindElement(Format(MyProfilePageObjects.DropdownByLabelXPath, label), How.XPath);
            element.Click();
           SiteDriver.WaitToLoadNew(300);
            SiteDriver.FindElement(Format(MyProfilePageObjects.DropdonwValueByTextXPath, label, value), How.XPath).Click();
            SiteDriver.WaitToLoadNew(300);
            Console.WriteLine("<{0}> Selected in <{1}> ", value, label);
        }
        public void SelectDropDownListValueByLabelOption(string label, string value, bool directSelect = true)
        {
            SiteDriver.WaitToLoadNew(300);
            var element = SiteDriver.FindElement(string.Format(MyProfilePageObjects.InputFieldByLabelXpathTemplate, label), How.XPath);
           
                JavaScriptExecutor.ExecuteClick(string.Format(MyProfilePageObjects.InputFieldByLabelXpathTemplate, label), How.XPath);
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
                if (!SiteDriver.IsElementPresent(string.Format(MyProfilePageObjects.DropDownInputListValueByLabelAndValueXPathTemplate, label, value), How.XPath)) JavaScriptExecutor.ClickJQuery(string.Format(MyProfilePageObjects.InputFieldByLabelCssTemplate, label));
                JavaScriptExecutor.ExecuteClick(string.Format(MyProfilePageObjects.DropDownInputListValueByLabelAndValueXPathTemplate, label, value), How.XPath);
                SiteDriver.WaitToLoadNew(300);
                Console.WriteLine("<{0}> Selected in <{1}> ", value, label);
        }

        public int ValidateAnswerInputBoxByLabel(string section, string label)
        {
            const string alphanumericValue = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_`~!@#$%^&*()-=+{}|<>?,./:";
            var answer = GenerateRandomTextBasedOnAcceptedValues(alphanumericValue, 101);
            SetInputTextBoxValueByLabel(section, label, answer);
            return GetInputTextBoxValueByLabel(section, label).Length;
        }     

        public string GetInputTextBoxValueByLabel(string section, string label) =>
            SiteDriver.FindElement(Format(MyProfilePageObjects.TextBoxByQuestionLabelXPath, section, label), How.XPath).GetAttribute("value");

        public void ClickSaveOrCancel(bool save)
        {
            if (save)
            {
                SiteDriver.FindElement(Format(MyProfilePageObjects.ButtonByTextXPath, "Save"), How.XPath)
                    .Click();
                WaitForWorking();
            }
            else
            {
                SiteDriver.FindElement(Format(MyProfilePageObjects.ButtonByTextXPath, "Cancel"), How.XPath)
                    .Click();
                WaitForWorking();
            }
        }

        public string GetDropDownListDefaultValue(string label)
        {
            var element = SiteDriver.FindElementAndGetAttribute(Format(MyProfilePageObjects.DropdownListByLabelXPath, label), How.XPath, "innerHTML");
            return element;
        }     

        #endregion
    }
}

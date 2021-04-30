using System;
using Nucleus.Service.PageServices.Base.Default;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Core.Navigation;
using Nucleus.Service.PageObjects.Settings.User;
using UIAutomation.Framework.Elements;
using Nucleus.Service.Support.Common.Constants;
using Nucleus.Service.Support.Enum;
using System.Collections.Generic;
using Nucleus.Service.SqlScriptObjects.Settings;
using UIAutomation.Framework.Utils;
using UIAutomation.Framework.Database;
using Nucleus.Service.SqlScriptObjects.User;
using System.Linq;
using Nucleus.Service.PageObjects.QuickLaunch;
using Nucleus.Service.PageServices.QuickLaunch;
using Nucleus.Service.Support.Common;
using Nucleus.Service.Support.Environment;

namespace Nucleus.Service.PageServices.Settings.User
{
    public class ProfileManagerPage : NewDefaultPage
    {
        #region PRIVATE FIELDS

        private ProfileManagerPageObjects _profileManagerPage;
        private OldUserProfileSearchPage _userProfileSearch;



        #endregion

        #region PUBLIC PROPERTIES

        public ProfileManagerPageObjects MyProfile
        {
            get { return _profileManagerPage; }
        }

        #endregion

        #region CONSTRUCTOR

        public ProfileManagerPage(INewNavigator navigator, ProfileManagerPageObjects profileManagerPage, ISiteDriver siteDriver, IJavaScriptExecutors javaScriptExecutors, IEnvironmentManager environmentManager, IBrowserOptions browserOptions, IOracleStatementExecutor executor)
            : base(navigator, profileManagerPage, siteDriver, javaScriptExecutors, environmentManager, browserOptions, executor)
        {
            _profileManagerPage = (ProfileManagerPageObjects)PageObject;
            }


        #endregion

        #region Database

        public void CloseDbConnection()
        {
            Executor.CloseConnection();
        }

        public List<string> GetAvailabeRestrictionsList()
        {
            return
                Executor.GetTableSingleColumn(SettingsSqlScriptObject.GetAvailabeRestrictionsList);
        }

        public List<string> GetAvailabeQuestionListFromDb()
        {
            return
                Executor.GetTableSingleColumn(UserSqlScriptObjects.GetSecurityQuestionList);
        }

        public string GetProcessingTypeForClientfromDatabase(string clientcode)
        {
            return Executor.GetSingleStringValue(string.Format(SettingsSqlScriptObject.ProcessingTypeForCleint, clientcode));
        }

        public void RestoreSecurityQuestionAndPasswordInDatabase(string pwd, string salt, string q1Id, string q2Id, string userid)
        {
            Executor.ExecuteQuery(string.Format(UserSqlScriptObjects.ResetUserPasswordAndSecurityQuestion, pwd, salt, q1Id, q2Id, userid));
            Console.WriteLine("Restore Security Question and password  of : {0} to default", userid);
        }

        public void RestoreSecurityAnswersInDatabase(string userid)
        {
            Executor.ExecuteQuery(string.Format(UserSqlScriptObjects.RestoreSecurityAnswers, userid));
            Console.WriteLine("Restore Security Answer of : {0} ", userid);
        }

        public void DeletePasswordHistoryAuditFromDatabase(string userid)
        {
            Executor.ExecuteQueryAsync(string.Format(UserSqlScriptObjects.DeletePasswordHistoryForUser, userid));
            Console.WriteLine("Delete Password History Audit of : {0} ", userid);
        }

        public int GetRestrictedClaimCountForClaimSequence(string claseq, List<string> assignedRestrictions)
        {
            string restrictions = "";
            for (int i = 0; i < assignedRestrictions.Count; i++)
            {
                if (i < assignedRestrictions.Count - 1)
                    restrictions += "'" + assignedRestrictions[i] + "',";
                else restrictions += "'" + assignedRestrictions[i] + "'";
            }
            return
                Convert.ToInt16(Executor.GetSingleValue(string.Format(SettingsSqlScriptObject.RestrictedClaimCountForClaimSequence, claseq.Split('-')[0], claseq.Split('-')[1], restrictions)));


        }
        #endregion
        #region PUBLIC METHODS

        public bool IsUserPrefernceDefaultDashboardViewLabelPresent()
        {
            return SiteDriver.IsElementPresent(ProfileManagerPageObjects.UserPreferenceDefaultDashboardView, How.Id);
        }

        public List<string> GetDashboardViewProductType()
        {
            JavaScriptExecutor.ExecuteClick(OldUserProfileSearchPageObjects.DashboardViewToggleIcon, How.Id);
            SiteDriver.WaitToLoadNew(3000);
            var list = JavaScriptExecutor.FindElements(OldUserProfileSearchPageObjects.DashboardViewDropdownOptionsListXpath, How.XPath, "Text");
            JavaScriptExecutor.ExecuteClick(OldUserProfileSearchPageObjects.DashboardViewToggleIcon, How.Id);
            return list;

        }

        public string GetSubscriberIdFromDatabase(string userid)
        {
            return Executor.GetSingleStringValue(string.Format(SettingsSqlScriptObject.SubscriberIdForUser, userid));
        }

        public void RemoveSubscriberIdFromDatabase(string userid)
        {
            StringFormatter.PrintMessage("Remove subscriber id value");
            Executor.ExecuteQuery(string.Format(SettingsSqlScriptObject.RemoveSubscriberIdValue, userid));
        }

        public string GetSelectedDashboardViewProductType()
        {
            return SiteDriver.FindElement(OldUserProfileSearchPageObjects.DashboardProductType, How.Id)
                .GetAttribute("value");
        }

        public void SetDefaultDashboardViewProductType(string value)
        {
            JavaScriptExecutor.ExecuteClick(OldUserProfileSearchPageObjects.DashboardViewToggleIcon, How.Id);
            JavaScriptExecutor.ExecuteClick(string.Format(OldUserProfileSearchPageObjects.DashboardViewDropdownOptionsXpath, value), How.XPath);
            Console.WriteLine("Default Dashboard View Selected: <{0}>", value);

        }
        public void SetDashboardAuthorityType(string authorityType)
        {
            SiteDriver.WaitToLoadNew(3000);
            SiteDriver.WaitForCondition(() =>
                SiteDriver.IsElementPresent(OldUserProfileSearchPageObjects.AuthorityToggleIcon, How.Id), 3000);
            JavaScriptExecutor.ExecuteClick(OldUserProfileSearchPageObjects.AuthorityToggleIcon, How.Id);
            JavaScriptExecutor.ExecuteClick(string.Format(OldUserProfileSearchPageObjects.SelectDashboardAuthorityXpathTemplate, authorityType), How.XPath);
            Console.WriteLine("Dashboard Authority Type Selected: <{0}>", authorityType);
        }

        public bool IsDashboardViewProductTypeDropdownPresent()
        {
            return SiteDriver.IsElementPresent(OldUserProfileSearchPageObjects.DashboardViewDropdownOptionsListXpath,
                How.XPath);
        }
        public string GetPageErrorMessage()
        {
            string windowHandle = SiteDriver.CurrentWindowHandle;
            SiteDriver.SwitchFrame("PopErrorsWindow");
            string result = SiteDriver.FindElement(ProfileManagerPageObjects.ErrorMessageXPath, How.XPath).Text;
            SiteDriver.SwitchWindow(windowHandle);
            return result;
        }

        public void CloseErrorModal()
        {  SiteDriver.FindElement(ProfileManagerPageObjects.CloseButtonClassName, How.ClassName).Click();
        }

        public bool IsErrorModalPresent()
        {
            return SiteDriver.FindElement(ProfileManagerPageObjects.ErrorModalPopupId, How.Id).Displayed;
        }

        public void ClickSaveButton(bool waitExtra = false)
        {
            JavaScriptExecutor.ExecuteClick(ProfileManagerPageObjects.SaveBtnCssLocator, How.CssSelector);
            //_profileManagerPage.SaveButton.Click();
            SiteDriver.WaitForPageToLoad();
            SiteDriver.WaitToLoadNew(1000);
            Console.WriteLine("Clicked Save Button");
            if (!waitExtra) return;
            Console.WriteLine("Additional wait added");
            SiteDriver.WaitToLoadNew(5000);
        }

        public QuickLaunchPage ClickSaveButtonToSaveProfile(bool waitExtra = false)
        {
            JavaScriptExecutor.ExecuteClick(ProfileManagerPageObjects.SaveBtnCssLocator, How.CssSelector);
            //_profileManagerPage.SaveButton.Click();
            SiteDriver.WaitForPageToLoad();
            SiteDriver.WaitToLoadNew(1000);
            Console.WriteLine("Clicked Save Button");

            return new QuickLaunchPage(Navigator, new QuickLaunchPageObjects(), SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor);
        }
        public void ClickCancelButton()
        {
            JavaScriptExecutor.ExecuteClick(ProfileManagerPageObjects.CancelBtnCssLocator, How.CssSelector);
            SiteDriver.WaitForPageToLoad();
            SiteDriver.WaitToLoadNew(1000);
            Console.WriteLine("Clicked on Cancel Button");
        }
        public bool IsSaveButtonPresent()
        {
            return SiteDriver.IsElementPresent(ProfileManagerPageObjects.SaveButtonId, How.Id);
        }

        public void InsertNewPassword(string password)
        {
            SiteDriver.FindElement(ProfileManagerPageObjects.NewPasswordTextBoxId, How.Id).SendKeys(password);
            Console.WriteLine("Inserted New password:  {0}", password);
        }

        public void InsertConfirmNewPassword(string password)
        {
            SiteDriver.FindElement(ProfileManagerPageObjects.ConfirmNewPasswordTextBoxId, How.Id).SendKeys(password);
            Console.WriteLine("Inserted Confirm New Password: {0}", password);
        }
        public void SelectQuestionFromList(string value, bool question1 = true)
        {
            var questionId = question1 ? 1 : 2;
            SiteDriver.WaitToLoadNew(300);
            var element = JavaScriptExecutor.FindElement('#' + string.Format(ProfileManagerPageObjects.SecurityQuestionTextBoxId, questionId));
            JavaScriptExecutor.FindElement('#' + string.Format(ProfileManagerPageObjects.SecurityQuestionArrowCaretId, questionId)).Click();
            SiteDriver.WaitToLoadNew(300);
            element.ClearElementField();
            if (!JavaScriptExecutor.IsElementPresent(string.Format(ProfileManagerPageObjects.QuestionDropDownValueSelectionCssTemplate, questionId, value))) JavaScriptExecutor.ClickJQuery('#' + string.Format(ProfileManagerPageObjects.QuestionDropDownValueSelectionCssTemplate, questionId, value));
            JavaScriptExecutor.ClickJQuery(string.Format(ProfileManagerPageObjects.QuestionDropDownValueSelectionCssTemplate, questionId, value));
            Console.WriteLine("<{0}> Selected in Question <{1}> ", value, questionId);
        }

        public List<string> GetAvailableQuestionList(bool question1 = true)
        {
            SiteDriver.WaitToLoadNew(1000);
            var questionId = question1 ? 1 : 2;
            var element = JavaScriptExecutor.FindElement('#' + string.Format(ProfileManagerPageObjects.SecurityQuestionArrowCaretId, questionId));
            element.Click();
            Console.WriteLine("Looking for Question <{0}> List", questionId);
            SiteDriver.WaitToLoadNew(500);
            var list = JavaScriptExecutor.FindElements(string.Format(ProfileManagerPageObjects.QuestionDropDownListCssTemplate, questionId), "Text");
            if (list.Count == 0)
            {
                element.Click();
                SiteDriver.WaitToLoadNew(500);
                list = JavaScriptExecutor.FindElements(string.Format(ProfileManagerPageObjects.QuestionDropDownListCssTemplate, questionId), "Text");
            }
            element.Click();
            Console.WriteLine("Question <{0}> Drop down list count is {1} ", questionId, list.Count);
            return list;
        }

        public string GetSelectedSecurityQuestion(bool question1 = true)
        {
            var questionId = question1 ? 1 : 2;
            return JavaScriptExecutor.FindElement(string.Format('#' + ProfileManagerPageObjects.SecurityQuestionTextBoxId, questionId)).GetAttribute("value");

        }

        public string GetSecurityAnswer1()
        {
           return  SiteDriver.FindElementAndGetAttribute(ProfileManagerPageObjects.SecurityAnswer1TextBoxId, How.Id,"value");
        }

        public string GetSecurityAnswer2()
        {
            return SiteDriver.FindElementAndGetAttribute(ProfileManagerPageObjects.SecurityAnswer2TextBoxId, How.Id, "value");
            //if (isEnabled)
            //{
            //    var value = _profileManagerPage.SecurityAnswer2TextBox.GetAttribute("value");
            //    return JavaScriptExecutor.Execute(string.Format("return JSON.parse('{0}').validationText", value)).ToString();
            //}
            //else return _profileManagerPage.HiddenAnswer2.GetAttribute("value");
        }

        public void SetSecurityAnswer1(string value)
        {
            SiteDriver.FindElement(ProfileManagerPageObjects.SecurityAnswer1TextBoxId, How.Id).ClearElementField();
            SiteDriver.FindElement(ProfileManagerPageObjects.SecurityAnswer1TextBoxId, How.Id).SendKeys(value);
            Console.WriteLine("Set Security Answer1: {0}", value);
        }

        public void SetSecurityAnswer2(string value)
        {
            SiteDriver.FindElement(ProfileManagerPageObjects.SecurityAnswer2TextBoxId, How.Id).ClearElementField();
            SiteDriver.FindElement(ProfileManagerPageObjects.SecurityAnswer2TextBoxId, How.Id).SendKeys(value);
            Console.WriteLine("Set Security Answer1: {0}", value);
        }
        public void ClickOnSecurityTab()
        {
            SiteDriver.FindElement(ProfileManagerPageObjects.SecurityTabId, How.Id).Click();
            StringFormatter.PrintMessage("Click on Security Tab");
        }
        public void ClickOnRestrictionsTab()
        {
            SiteDriver.FindElement(ProfileManagerPageObjects.RestrictionsTabId, How.Id).Click();
            StringFormatter.PrintMessage("Click on Restrictions Tab");
        }

        public void ClickOnShowAnswer1()
        {
            SiteDriver.FindElement(ProfileManagerPageObjects.ShowAnswer1Id, How.Id).Click();
            SiteDriver.WaitForCondition(() => GetSecurityAnswer1().Length > 0);
            Console.Write("Click on Show answer of first security question");
        }

        public void ClickOnShowAnswer2()
        {
            SiteDriver.FindElement(ProfileManagerPageObjects.ShowAnswer2Id, How.Id).Click();
            Console.Write("Click on Show answer of second security question");
            SiteDriver.WaitForCondition(() => GetSecurityAnswer2().Length > 0);
        }

        public bool IsShowAnswer1Visible()
        {
            return SiteDriver.IsElementPresent(ProfileManagerPageObjects.ShowAnswer1Id, How.Id);
        }

        public bool IsShowAnswer2Visible()
        {
            return SiteDriver.IsElementPresent(ProfileManagerPageObjects.ShowAnswer2Id, How.Id);
        }

        public bool IsSubscriberIdpresent()
        {
            return SiteDriver.IsElementPresent(ProfileManagerPageObjects.SubscriberIdById, How.Id);
        }

        public bool IsSubscriberIdFieldEnabled()
        {
            return SiteDriver.IsElementEnabled(ProfileManagerPageObjects.SubscriberIdById, How.Id);
        }

        public string GetSubscriberIdValue()
        {
            // return JavaScriptExecutor.FindElement(ProfileManagerPageObjects.SubscriberIdById).Text;
            return SiteDriver.FindElementAndGetAttribute(ProfileManagerPageObjects.SubscriberIdById, How.Id, "value");
        }

        public void SetValueforSubscriberField(string value)
        {
            SiteDriver.FindElement(ProfileManagerPageObjects.SubscriberIdById, How.Id).ClearElementField();
            SiteDriver.FindElementAndSetText(ProfileManagerPageObjects.SubscriberIdById, How.Id, value);
        }

        public void ClearSubscriberIdInputValue()
        {
            SiteDriver.FindElement(ProfileManagerPageObjects.SubscriberIdById, How.Id).ClearElementField();
        }

        public string GetSelectedClientType()
        {
            return SiteDriver.FindElement(ProfileManagerPageObjects.ClientTypeId, How.Id)
                .GetAttribute("value");

        }

        public string GetUserFirstName()
        {
            return SiteDriver.FindElementAndGetAttribute(ProfileManagerPageObjects.FirstNamebyId, How.Id, "value");
        }

        public string GetUserLastName()
        {
            return SiteDriver.FindElementAndGetAttribute(ProfileManagerPageObjects.LastNamebyId, How.Id, "value");
        }








        /// <summary>
        /// Click on Privileges by clicking on Privileges tab
        /// </summary>
        /// <returns>An instance of ProfileManagerPage</returns>
        public void ClickOnProfile()
        {
            SiteDriver.FindElement(ProfileManagerPageObjects.ProfileTabId, How.Id).Click();
            StringFormatter.PrintMessage("Click on Profile Tab");
        }

        /// <summary>
        /// Click on Privileges by clicking on Privileges tab
        /// </summary>
        /// <returns>An instance of ProfileManagerPage</returns>
        public void ClickOnPrivileges()
        {
            SiteDriver.FindElement(ProfileManagerPageObjects.PrivilegesTabId, How.Id).Click();
            StringFormatter.PrintMessage("Click on Privileges Tab");
        }

        /// <summary>
        /// Obtain work list value
        /// </summary>
        /// <param name="product">An enumerated product value</param>
        /// <returns>A work list value</returns>
        public string GetWorkListValue(ProductEnum product)
        {
            string workList = null;
            switch (product)
            {
                case ProductEnum.CV:
                    workList = ProfileManagerPageObjects.PciWorkListInputCssLocator;
                    break;
                case ProductEnum.FCI:
                    workList = ProfileManagerPageObjects.FciWorkListInputCssLocator;
                    break;
            }
            try
            {
                return SiteDriver.FindElement(workList, How.CssSelector).GetAttribute("value");
            }
            catch
            {
                return string.Empty;
            }
        }

        public bool IsReadWriteAssigned(string authority)
        {
            return SiteDriver.FindElementAndGetAttribute(
                string.Format(ProfileManagerPageObjects.AssignedAuthorityValueXpathTemplate, authority), How.XPath,
                "value").Equals("Read/write");

        }

        public bool IsReadOnlyAssgined(string authority)
        {
            return SiteDriver.FindElementAndGetAttribute(
                string.Format(ProfileManagerPageObjects.AssignedAuthorityValueXpathTemplate, authority), How.XPath,
                "value").Equals("Read-only");
        }


        public bool IsAuthorityAssigned(string authority)
        {
            return SiteDriver.IsElementPresent(string.Format(ProfileManagerPageObjects.AssignedAuthorityLabelXPathTemplate, authority),
                How.XPath);
        }

        public bool IsAuthorityAvailableOrAssigned(string authority)
        {
            return JavaScriptExecutor.IsElementPresent(string.Format(ProfileManagerPageObjects.PrivilegesAvailableorAssignedSpanCssLocator, authority));
        }

        public bool IsRoleAssigned(string authority)
        {
            return SiteDriver.IsElementPresent(string.Format(ProfileManagerPageObjects.AssignedRoleLabelXPathTemplate, authority),
                How.XPath);
        }

        public bool AnyRoleIsAssigned()
        {
            return SiteDriver.IsElementPresent(ProfileManagerPageObjects.AssignedRoleXpath, How.XPath);
        }

        public string GetDefaultPagePreference()
        {
            return SiteDriver.FindElement(ProfileManagerPageObjects.DefaultPage, How.Id).GetAttribute("value");
        }

        public string GetDefaultDashboardPreference()
        {
            return SiteDriver.FindElement(ProfileManagerPageObjects.DefaultDashboard, How.Id).GetAttribute("value");
        }

        public void SelectAndRemoveAssignedAuthorityFromList(string authority)
        {
            SiteDriver.FindElement
                (string.Format(ProfileManagerPageObjects.SelectAssignedAuthorityXpathTemplate, authority), How.XPath).Click();
            SiteDriver.FindElement(ProfileManagerPageObjects.LeftArrowButtonXPath,How.XPath).Click();
            SiteDriver.WaitForCondition(() => !SiteDriver.IsElementPresent(ProfileManagerPageObjects.RadAjaxPanelDivCssLocator, How.CssSelector));
            Console.WriteLine("Removed {0} authority", authority);

        }

        public void RefreshPage()
        {
            SiteDriver.Refresh();
        }

        public void SwitchToCompleteProfileUserFrame()
        {
            SiteDriver.SwitchFrame(ProfileManagerPageObjects.CompleteYourProfileFrameName);
            SiteDriver.WaitForCondition(() => SiteDriver.IsElementPresent(ProfileManagerPageObjects.SaveButtonId, How.Id), 5000);
        }

        public string GetSecurityQ1Text()
        {
            return SiteDriver.FindElementAndGetAttribute(ProfileManagerPageObjects.SecurityQ1Id, How.Id, "value");
        }

        public string GetSecurityQ2Text()
        {
            return SiteDriver.FindElementAndGetAttribute(ProfileManagerPageObjects.SecurityQ2Id, How.Id, "value");
        }

        public void CloseCurrentWindowAndSwitchToOriginal()
        {
            var handles = SiteDriver.WindowHandles.ToList();
            SiteDriver.CloseWindowAndSwitchTo(handles[0]);
        }

        //public void MoveAllValuesFromAssignedToAvailableByLabel(string label)
        //{
        //    if (GetAssignedRestrictionsListByLabel(label).Count != 0)
        //        SiteDriver.FindElement(string.Format(UserProfileSearchPageObjects.GetAllArrowsByLabelXpathTemplate, label, 4), How.XPath).Click();
        //}
        public List<string> GetAssignedRestrictionsListByLabel(string label)
        {
            return JavaScriptExecutor.FindElements(
                string.Format(OldUserProfileSearchPageObjects.AssignedAccessListXpathTemplate, label), How.XPath,
                "Text");


        }

        public List<string> GetAvailableRestrictionsListByLabel(string label)
        {
            return JavaScriptExecutor.FindElements(
                string.Format(ProfileManagerPageObjects.AvailableAcessListXpathTemplate, label), How.XPath,
                "Text");

        }


        public void MoveAllValuesFromAvailabelToAssignedByLabel(string label)
        {
            if ((GetAvailableRestrictionsListByLabel(label).Count != 0) && label == "Restricted Claims Access")
                SiteDriver.FindElement(string.Format(ProfileManagerPageObjects.GetAllArrowsByLabelXpathTemplate, label, 3), How.XPath).Click();
            else
                SiteDriver.FindElement(string.Format(ProfileManagerPageObjects.GetAllArrowsByLabelXpathTemplate, label, 3), How.XPath).Click();
            SiteDriver.WaitForCondition(() => !SiteDriver.IsElementPresent(ProfileManagerPageObjects.RadAjaxPanelDivCssLocator, How.CssSelector));

        }

        public bool IsSpanLabelNamePresent(string label)
        {
            return SiteDriver.IsElementPresent(
                string.Format(ProfileManagerPageObjects.SpanLabelNameXPathSelector, label), How.XPath);
        }
        public void SelectAvailableClientAndPrivilege(string client)
        {
            SiteDriver.WaitToLoadNew(3000);
            SiteDriver.WaitForCondition(() =>
                SiteDriver.IsElementPresent(ProfileManagerPageObjects.WizardBackButtonXpath, How.XPath), 3000);
            SiteDriver.FindElement(
                string.Format(ProfileManagerPageObjects.AvailableClientXPathTemplate, client), How.XPath).Click();
            SiteDriver.FindElement(ProfileManagerPageObjects.RightArrowButtonCssSelector, How.ClassName).Click();
            SiteDriver.WaitForCondition(() => !SiteDriver.IsElementPresent(ProfileManagerPageObjects.RadAjaxPanelDivCssLocator, How.CssSelector));
            Console.WriteLine("Selected Client: {0}", client);
        }

        public bool IsAutoDisplayPatientClaimHistoryCheckBoxChecked()
        {
            return SiteDriver.FindElement<CheckBox>(ProfileManagerPageObjects.AutoDisplayPatientClaimHistoryId, How.Id).Checked;

        }


        public void ClickOnAutoDisplayPatientClaimHistoryCheckBox(bool click)
        {
            if (SiteDriver.FindElement(ProfileManagerPageObjects.AutoDisplayPatientClaimHistoryId, How.Id).Selected)
            {
                SiteDriver.FindElement(ProfileManagerPageObjects.AutoDisplayPatientClaimHistoryId, How.Id).Click();
                if (click)
                {
                    SiteDriver.FindElement(ProfileManagerPageObjects.AutoDisplayPatientClaimHistoryId, How.Id).Click();

                }

             }
            else if (click)
            {
                SiteDriver.FindElement(ProfileManagerPageObjects.AutoDisplayPatientClaimHistoryId, How.Id).Click();

            }
        }
        #region Restriction Tab


        public string GetSectionTitleForRestrictedClaimsAccess()
        {
            return SiteDriver.FindElement(ProfileManagerPageObjects.ClaimViewSectionTitleCssLocator, How.CssSelector).Text;
        }


        public string GetRestrictedClaimsAccessContainerTitle(int row = 1)
        {
            var test = SiteDriver.FindElement(string.Format(ProfileManagerPageObjects.ClaimRestrictionContainerTitleCssLlocator, row), How.XPath).Text;
            return test;
        }


        public List<string> GetAvailableRestrictedClaimsAccessListFromSection()
        {
            return
                JavaScriptExecutor.FindElements(ProfileManagerPageObjects.AvailableRestrictionsListCssLocator, How.CssSelector, "Text");
        }

        public List<string> GetAssignedRestrictionsListFromSection()
        {
            return
                JavaScriptExecutor.FindElements(ProfileManagerPageObjects.AssignedRestrictionsListCssLocator, How.CssSelector, "Text");
        }



        public void SelectAndAddAvailableClaimsAccessFromList(string restriction)
        {
            SiteDriver.FindElement
                (string.Format(ProfileManagerPageObjects.SelectAvailableRestrictionXpathTemplate, restriction), How.XPath).Click();
            SiteDriver.FindElement(ProfileManagerPageObjects.RightArrowButtonCssSelector,How.CssSelector).Click();
            SiteDriver.WaitForCondition(() => !SiteDriver.IsElementPresent(ProfileManagerPageObjects.RadAjaxPanelDivCssLocator, How.CssSelector));
            Console.WriteLine("Added {0} restriction", restriction);

        }

        public void SelectAndRemoveAssignedRestrictionFromList(string restriction)
        {
            SiteDriver.FindElement
                (string.Format(ProfileManagerPageObjects.SelectAssignedRestrictionXpathTemplate, restriction), How.XPath).Click();
            SiteDriver.FindElement(ProfileManagerPageObjects.LeftArrowButtonInRestrictionXPath, How.XPath).Click();
            SiteDriver.WaitForCondition(() => !SiteDriver.IsElementPresent(ProfileManagerPageObjects.RadAjaxPanelDivCssLocator, How.CssSelector));
            Console.WriteLine("Removed {0} restriction", restriction);

        }


        public void RemoveAllAssignedRestrictionFromList()
        {
            SiteDriver.FindElement(ProfileManagerPageObjects.AllLeftArrowButtonXPath, How.XPath).Click();
            SiteDriver.WaitForCondition(() => !SiteDriver.IsElementPresent(ProfileManagerPageObjects.RadAjaxPanelDivCssLocator, How.CssSelector));
            Console.WriteLine("Removed All restriction");

        }

        public void AddAllAvailableRestrictionFromList()
        {
            SiteDriver.FindElement(ProfileManagerPageObjects.AllRightArrowButtonXPath, How.XPath).Click();
            SiteDriver.WaitForCondition(() => !SiteDriver.IsElementPresent(ProfileManagerPageObjects.RadAjaxPanelDivCssLocator, How.CssSelector));
            Console.WriteLine("Added All restriction");

        }
        public string GetRestrictionTabTitle()
        {  return SiteDriver.FindElement(ProfileManagerPageObjects.RestrictionsTabId, How.Id).Text;
        }



        public bool IsRespectiveAuthorityAssigned(string authority, string authType = "")
        {
            ClickOnPrivileges();
            switch (authType)
            {
                case "Read-Only":
                    return IsReadOnlyAssgined(authority);


                case "Read-Write":
                    return IsReadWriteAssigned(authority);

                default:
                    return IsAuthorityAssigned(authority);

            }

        }

        public bool IsAuthorityAvailable(string authority)
        {
            return JavaScriptExecutor.IsElementPresent(
                string.Format(ProfileManagerPageObjects.PrivilegesAvailableSpanCssLocator, authority));
        }

        public void SelectDefaultDashboard(string dashboardType)
        {
            ClickOnProfile();
            SiteDriver.FindElement(ProfileManagerPageObjects.UserPreferenceDefaultDashboardView, How.Id)
                .Click();
            SiteDriver.FindElement(string.Format(ProfileManagerPageObjects.UserPreferenceDefaultDashboard, dashboardType), How.XPath).Click();
            ClickSaveButton();
        }

        public void SelectDefaultPage(string PageLabel)
        {
            ClickOnProfile();
            SiteDriver.FindElement(ProfileManagerPageObjects.UserpreferenceDefaultPageById, How.Id)
                .Click();
            SiteDriver.FindElement(string.Format(ProfileManagerPageObjects.UserPreferenceDefaultDashboard, PageLabel), How.XPath).Click();
            ClickSaveButton();
        }

        public void MoveOptionFromAvailableToAssigned(string label, string option, bool reverse = false)
        {
            SiteDriver.FindElement(
                string.Format(OldUserProfileSearchPageObjects.AvailabeOptionByLabelOptionTemplateXpath, label, option),
                How.XPath).Click();
            if (reverse)
                SiteDriver.FindElement(string.Format(ProfileManagerPageObjects.GetAllArrowsByLabelXpathTemplate, label, 2), How.XPath).Click();
            else
                SiteDriver.FindElement(string.Format(ProfileManagerPageObjects.GetAllArrowsByLabelXpathTemplate, label, 1), How.XPath).Click();
        }

        public void SetAuthorityTypeFromDropDown(string label, string authorityType)
        {
            JavaScriptExecutor.FindElement(string.Format(ProfileManagerPageObjects.AssignedAuthorityCssLocator, label)).Click();
            WaitForWorking();
            WaitForStaticTime(500);
            JavaScriptExecutor.ClickJQuery(string.Format(ProfileManagerPageObjects.AuthorityTypeCssLocator, authorityType));
        }

        public bool IsPrivilegeAssignedFromDatabase(string username, string privilege)
        {
            return Executor.GetSingleValue(string.Format(SettingsSqlScriptObject.GetAuthorityCountByPrivilege, username,
                 privilege)) == 1;
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

        public bool IsAnyRoleAssignedToUserFromDatabase(string username)
        {
            return Executor.GetSingleValue(string.Format(SettingsSqlScriptObject.GetRoleCountForUser, username
                   )) > 0;
        }
        public bool IsRoleAssignedFromDatabase(string username, string role)
        {

            return Executor.GetSingleValue(string.Format(SettingsSqlScriptObject.GetAuthorityCountByRole, username, role)) == 1;
        }



        public void ClickOnBackButton()
        {
            SiteDriver.FindElement(ProfileManagerPageObjects.BackButtonCssSelector, How.CssSelector).Click();
        }

        public void MoveAllValuesFromAssignedToAvailableByLabel(string label)
        {
            SiteDriver.FindElement(string.Format(OldUserProfileSearchPageObjects.GetAllArrowsByLabelXpathTemplate, label, 4), How.XPath).Click();
        }
        public string[] GetAllAvailableRolesList()
        {
            return JavaScriptExecutor.FindElements(
                OldUserProfileSearchPageObjects.AvailableRolesPrivilegesCssSelector, How.CssSelector, "Text").ToArray();
        }

        public void SelectRandomRolePrivilege()
        {
            MoveAllValuesFromAssignedToAvailableByLabel("Role Privileges");
            string[] rolePrivilege = GetAllAvailableRolesList();
            Random r = new Random();
            int roleIndex = r.Next(0, rolePrivilege.Length - 1);
            MoveOptionFromAvailableToAssigned("Role Privileges", rolePrivilege[roleIndex]);

        }



        public void SelectDefaultClient(string client)
        {

            SiteDriver.FindElement(ProfileManagerPageObjects.UserPreferenceDefaultClient, How.Id)
                .Click();
            SiteDriver.FindElement(string.Format(ProfileManagerPageObjects.UserPreferenceDefaultDashboard, client), How.XPath).Click();

        }

        public List<string> GetDefaultPageList()
        {
            SiteDriver.WaitToLoadNew(1000);

            SiteDriver.FindElement(ProfileManagerPageObjects.UserpreferenceDefaultPageArrowById, How.Id).Click();
            var elements1 = JavaScriptExecutor.FindElements(ProfileManagerPageObjects.UserPreferenceDefaultPage, "Text");
            SiteDriver.FindElement(ProfileManagerPageObjects.UserpreferenceDefaultPageArrowById, How.Id).Click();


            return elements1;
        }

        public List<string> GetAssignedClientsList()
        {
            var clientList = SiteDriver.FindDisplayedElementsText(ProfileManagerPageObjects.AssignedClientsListXpath,
                How.XPath);
            var clientCodeList = new List<string>();
            foreach (var clients in clientList)
            {
                clientCodeList.Add(clients.Split(' ')[0]);
            }
            return clientCodeList;
        }

        #endregion


        #endregion
    }
}


using Nucleus.Service.PageObjects.Settings;
using Nucleus.Service.PageServices.Base.Default;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Nucleus.Service.PageObjects.Batch;
using Nucleus.Service.PageObjects.Default;
using Nucleus.Service.PageObjects.Settings.User;
using Nucleus.Service.PageServices.Settings.User;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Core.Navigation;
using UIAutomation.Framework.Database;
using UIAutomation.Framework.Utils;
using UIAutomation.Framework.Elements;
using Nucleus.Service.SqlScriptObjects.MicroStrategy;
using Nucleus.Service.Support.Common;
using Nucleus.Service.Support.Environment;


namespace Nucleus.Service.PageServices.Settings
{
    public class MicrostrategyMaintenancePage : NewDefaultPage
    {

        #region PRIVATE
        private readonly MicrostrategyMaintenancePageObjects _microstrategyMaintenance;
        private readonly GridViewSection _gridViewSection;
        private readonly SideWindow _sideWindow;


        #endregion

        #region CONSTRUCTOR
        public MicrostrategyMaintenancePage(INewNavigator navigator, MicrostrategyMaintenancePageObjects microstrategyMaintainance, ISiteDriver siteDriver, IJavaScriptExecutors javaScriptExecutors, IEnvironmentManager environmentManager, IBrowserOptions browserOptions, IOracleStatementExecutor executor) : 
            base(navigator, microstrategyMaintainance, siteDriver, javaScriptExecutors, environmentManager, browserOptions, executor)
        {
            _microstrategyMaintenance = (MicrostrategyMaintenancePageObjects)PageObject;
            _gridViewSection = new GridViewSection(SiteDriver,JavaScriptExecutor);
            _sideWindow = new SideWindow(new CalenderPage(SiteDriver), SiteDriver, JavaScriptExecutor);


        }
        #endregion

        #region PUBLIC

        public GridViewSection GetGridViewSection
        {
            get { return _gridViewSection; }
        }

        public SideWindow SideWindow
        {
            get { return _sideWindow; }
        }



        public string GetQuadrantTitle(int i = 0)
        {
            var value = JavaScriptExecutor.FindElements(MicrostrategyMaintenancePageObjects.QuadrantTitlesCssTemplate, How.CssSelector,
                "Text");
            return value[i];
        }

        public string GetValueInGridByRowCol(string quadrantName, int col = 2, int row = 1)
        {

            return SiteDriver.FindElement(
                string.Format(MicrostrategyMaintenancePageObjects.ValueInGridByQuadrantRowColumnXpath, quadrantName, row, col), How.XPath).Text;

        }
        public string GetLabelInGridByColRow(string quadrantName, int col = 2, int row = 1)
        {
            return SiteDriver.FindElement(
                string.Format(MicrostrategyMaintenancePageObjects.LabelInGridByQuadrantRowColumnXpath, quadrantName, row, col), How.XPath).Text.Split(':')[0];
        }

        public List<string> GetReportsGridListValueByCol(int col = 2)
        {
            var t = JavaScriptExecutor.FindElements(
                string.Format(MicrostrategyMaintenancePageObjects.ListValueInGridByQuadrantAndColumnXpath,
                    "Microstrategy Reports", col), How.XPath, "Text");
            return t;
        }

        public void ClickOnAddIconByQuadrantName(string label)
        {
            SiteDriver.FindElement(
                string.Format(MicrostrategyMaintenancePageObjects.AddIconByQuadrantNameXpath, label), How.XPath).Click();
            Console.WriteLine("Clicked on {0} add icon.", label);
        }

        public int GetGridRowCountByQuadrantName(string label)
        {
            return SiteDriver.FindElementsCount(
                string.Format(MicrostrategyMaintenancePageObjects.GridRowByQuadrantHeaderXpath,
                    label), How.XPath);
        }

        public bool IsAddUserGroup()
        {
            return SiteDriver
                .FindElementAndGetAttribute(MicrostrategyMaintenancePageObjects.AddReportIconXpath, How.XPath, "class")
                .Contains("is_disabled");
        }

        public bool IsAddReportIconEnabled()
        {
            return SiteDriver
                .FindElementAndGetAttribute(MicrostrategyMaintenancePageObjects.AddReportIconXpath, How.XPath, "class")
                .Contains("is_active");
        }

        public bool IsAddMicrostrategyReportFormDisplayed()
        {
            return SiteDriver.IsElementPresent(MicrostrategyMaintenancePageObjects.AddMicrostrategyReportsFormXpath,
                How.XPath);
        }

        public bool IsEditMicrostrategyReportFormDisplayed()
        {
            return SiteDriver.IsElementPresent(MicrostrategyMaintenancePageObjects.EditMicrostrategyReportsFormXpath,
                 How.XPath);

        }

        public bool IsInputFieldPresentInAddReport(string label)
        {
            return JavaScriptExecutor.IsElementPresent(string.Format(MicrostrategyMaintenancePageObjects.InputFieldByLabelXpath, label));
        }

        public string GetInputFieldValueByLabel(string label)
        {


            var dd = JavaScriptExecutor.FindElement(
                    string.Format(MicrostrategyMaintenancePageObjects.InputFieldByLabelXpath, label))
                .GetAttribute("value");
            return dd;


            //return SiteDriver.FindElement(string.Format(MicrostrategyMaintenancePageObjects.InputFieldByLabelXpath, label),How.XPath).Text;


        }

        public void SetInputFieldByLabel(string label, string value)
        {

            JavaScriptExecutor.FindElement(string.Format(MicrostrategyMaintenancePageObjects.InputFieldByLabelXpath, label)).ClearElementField();
            JavaScriptExecutor.FindElement(string.Format(MicrostrategyMaintenancePageObjects.InputFieldByLabelXpath, label)).SendKeys(value);
            Console.WriteLine("{0} value set in {1} field", value, label);
        }

        public string GetValueOfInputFieldByLabel(string label)
        {

            return SiteDriver.FindElement(string.Format(MicrostrategyMaintenancePageObjects.InputFieldByLabelXpath, label), How.XPath).GetAttribute("value");

        }



        public void ClickonAddMicrostrategyReportSaveButton()
        {
            SiteDriver.FindElement(string.Format(MicrostrategyMaintenancePageObjects.SaveButtonByQuadrantHeaderXpath, "Microstrategy Reports"), How.XPath).Click();
        }

        public bool IsEditIconPresentInByRow(string quadrantName, int row = 1)
        {
            return SiteDriver.IsElementPresent(
                string.Format(MicrostrategyMaintenancePageObjects.EditRecordIconByQuadrantRowXpath,
                    quadrantName, row), How.XPath);
        }
        public void ClickonMicrostrategyReportSaveButton()
        {
            var element =
                SiteDriver.FindElement(
                    string.Format(MicrostrategyMaintenancePageObjects.SaveButtonByQuadrantHeaderXpath,
                        "Microstrategy Reports"), How.XPath);
            SiteDriver.ElementToBeClickable(element);
            SiteDriver.WaitToLoadNew(500);
            element.Click();
            WaitForWorkingAjaxMessage();
            Console.WriteLine("Save button in Microstrategy Reports form clicked");
        }


        public bool IsDeleteIconPresentByRow(string quadrantName, int row = 1)
        {
            return SiteDriver.IsElementPresent(
                string.Format(MicrostrategyMaintenancePageObjects.DeleteRecordIconByQuadrantRow,
                    quadrantName, row), How.XPath);
        }

        public bool IsEditIconPresentByValueInRow(string value)
        {
            return JavaScriptExecutor.IsElementPresent(
                string.Format(MicrostrategyMaintenancePageObjects.EditIconByValueInRowCssLocator, value));
        }

        public bool IsDeleteIconPresentByValueInRow(string value)
        {
            return JavaScriptExecutor.IsElementPresent(
                string.Format(MicrostrategyMaintenancePageObjects.DeleteIconByValueInRowCssLocator, value));

        }

        public void ClickOnEditIconByRowValue(string value)
        {
            JavaScriptExecutor.ClickJQuery(
                string.Format(MicrostrategyMaintenancePageObjects.EditIconByValueInRowCssLocator, value));

        }

        public void ClickOnDeleteIconByRowValue(string value)
        {
            JavaScriptExecutor.ClickJQuery(
                string.Format(MicrostrategyMaintenancePageObjects.DeleteIconByValueInRowCssLocator, value));
        }

        public string GetReportFormHeader()
        {
            return SiteDriver
                .FindElement(MicrostrategyMaintenancePageObjects.ReportFormHeaderXpath, How.XPath).Text;
        }
        public List<string> GetAvailableDropDownList(string label)
        {
            return _sideWindow.GetAvailableDropDownList(label);
        }


        public List<string> GetMultiSelectListedDropDownList(string label)
        {
            //SiteDriver.FindElement(
            //    string.Format(MicrostrategyMaintenancePageObjects.MultiDropDownToggleIconXpathTemplate, label),
            //    How.XPath).Click();
            //var list = SiteDriver.FindElements(string.Format(MicrostrategyMaintenancePageObjects.MultiSelectListedDropDownToggleValueXpathTemplate, label), How.XPath, "Text");
            //JavaScriptExecutor.ExecuteMouseOut(string.Format(MicrostrategyMaintenancePageObjects.MultiDropDownToggleIconXpathTemplate, label), How.XPath);
            ////return list;
            return _sideWindow.GetMultiSelectListedDropDownList(label);
        }

        public void SelectSearchDropDownListForMultipleSelectValue(string label, string value)
        {
            _sideWindow.SelectSearchDropDownListForMultipleSelectValue("Username", value);
        }

        public void SelectMultipleValuesInMultiSelectDropdownList(string label, List<string> list)
        {
            _sideWindow.SelectMultipleValuesInMultiSelectDropdownList(label, list);
        }
        public string GetInputAttributeValueByLabel(string label, string attribute)
        {
            return
                _sideWindow.GetInputAttributeValueByLabel(label, attribute);
        }

        public void ClickOnDeleteIconByFullName(string fullName)
        {
            JavaScriptExecutor.ExecuteClick(string.Format(MicrostrategyMaintenancePageObjects.DeleteIconByFullNameXpathTemplate, fullName), How.XPath);
        }



        public void ClickOkCancelOnConfirmationModal(bool confirmation)
        {

            if (confirmation)
            {
                JavaScriptExecutor.ExecuteClick(DefaultPageObjects.OkConfirmationCssSelector, How.CssSelector);


                // JavaScriptExecutor.ExecuteClick(DefaultPageObjects.OkConfirmationCssSelector, How.CssSelector);

                WaitForWorking();
                Console.WriteLine("Ok Button is Clicked");

            }
            else
            {
                JavaScriptExecutor.ExecuteClick(DefaultPageObjects.CancelConfirmationCssSelector, How.CssSelector);
                WaitForWorking();
                Console.WriteLine("Cancel Button is Clicked");

            }

        }

        public void ClearInputField(string label)
        {
            SiteDriver.FindElement(string.Format(SideBarPanelSearch.InputFieldByLabelXpathTemplate, label), How.XPath).ClearElementField();
        }

        public void ClickOnEditIconByRowCol(string quadrantName, int row = 1)
        {


            JavaScriptExecutor.ExecuteClick(string.Format(MicrostrategyMaintenancePageObjects.EditRecordIconByQuadrantRowXpath, quadrantName, row), How.XPath);
            SiteDriver.WaitForCondition(IsEditMicrostrategyReportFormDisplayed);
            Console.WriteLine("Click On Small Edit Icon");

        }

        public void ClickonCancelButton()
        {
            SiteDriver.FindElement(MicrostrategyMaintenancePageObjects.CancelButtonXpath, How.XPath).Click();
            Console.WriteLine("Cancel button clicked");
            WaitForWorkingAjaxMessage();
        }

        public bool IsRowPresentByValue(string value)
        {
            return JavaScriptExecutor.IsElementPresent(string.Format(MicrostrategyMaintenancePageObjects.GridRowByValue, value));
        }

        public void ClickonSaveAndCloseButton()
        {
            SiteDriver.FindElement(MicrostrategyMaintenancePageObjects.SaveAndCloseButtonXpath, How.XPath).Click();
        }

        public string GetGridColValueByValue(string value, int col = 2)
        {
            string string1 = JavaScriptExecutor
                 .FindElement(
                     string.Format(MicrostrategyMaintenancePageObjects.GridColValueByUniqueRowValue, value, col)).Text;
            return string1;
        }

        public Int32 GetLengthOfTheInputFieldByLabel(string label)
        {
            var element = JavaScriptExecutor.FindElement(string.Format(MicrostrategyMaintenancePageObjects.InputFieldByLabelXpath, label)).GetAttribute("value");
            var lengthOfInputField = element.Length;
            return lengthOfInputField;
        }

        public bool IsEmptyMessagePresent()
        {
            return SiteDriver.IsElementPresent(MicrostrategyMaintenancePageObjects.EmptyMessageCssSelector, How.CssSelector);
        }

        public string GetEmptyMessage()
        {
            return SiteDriver.FindElement(MicrostrategyMaintenancePageObjects.EmptyMessageCssSelector,
                How.CssSelector).Text;
        }

        public ProfileManagerPage ClickOnUserIdAndNavigateToProfileManager(string name)
        {
            var profilePage = Navigator.Navigate<ProfileManagerPageObjects>(() =>
            {
                SiteDriver.FindElement(string.Format(MicrostrategyMaintenancePageObjects.FullNameLinkXpath, name), How.XPath
                    ).Click();
                //JavaScriptExecutor.ClickJQuery(string.Format(MicrostrategyMaintenancePageObjects.FullNameLinkXpath, name));
                SiteDriver.WaitForCondition(JavaScriptExecutor.ExecuteJqueryStatus);
                Console.WriteLine("Clicked on Full Name " + name);
                SiteDriver.WaitForCondition(() => SiteDriver.SwitchWindowByUrl("ProfileManager"));
                SiteDriver.WaitForCondition(
                    () => SiteDriver.IsElementPresent(DefaultPageObjects.PageHeaderCssLocator, How.CssSelector));

            });

            return new ProfileManagerPage(Navigator, profilePage, SiteDriver, JavaScriptExecutor, EnvironmentManager, BrowserOptions, Executor
            );

        }



        public List<string> GetGridLabelListByRow(string quadrantName, int row = 1)
        {
            return JavaScriptExecutor.FindElements(
                string.Format(MicrostrategyMaintenancePageObjects.GridLabelListByRowXpath, quadrantName, row), How.XPath, "Text").Select(x => x.Split(':')[0]).ToList();
        }

        public List<string> GetGridValueListByRow(string quadrantName, int row = 1)
        {
            return JavaScriptExecutor.FindElements(
                string.Format(MicrostrategyMaintenancePageObjects.GridValueListByRowXpath, quadrantName, row), How.XPath, "Text");
        }

        public void MouseOutFromDropdown(string label)
        {
            _sideWindow.MouseOutFromDropdown(label);

        }

        #region SQL


        public string GetMstrReportsCountFromDatabase()
        {
            return Executor.GetSingleStringValue(MicrostrategySqlScriptObjects.MstrReportsCount);
        }

        public string GetPasswordValueFromDataBase(string groupname, string username)
        {
            return Executor.GetSingleStringValue(string.Format(MicrostrategySqlScriptObjects.PasswordForUserGroup, groupname, username));
        }

        public List<string> GetMstrReportsDetailFromDatabase()
        {
            var result = Executor.GetCompleteTable(MicrostrategySqlScriptObjects.MstrReports);
            var dataRows = result as DataRow[] ?? result.ToArray();
            return dataRows[0].ItemArray.Select(x => x.ToString()).ToList();
        }

        public List<List<string>> GetUserDetailsFromDatabase()
        {
            var infoList = new List<List<string>>();
            var batchInfo = Executor
                .GetCompleteTable(string.Format(MicrostrategySqlScriptObjects.ExistingUserGroupDetailsFromDatabase));
            foreach (DataRow row in batchInfo)
            {
                var t = row.ItemArray.Select(x => x.ToString()).ToList();
                infoList.Add(t);
            }
            return infoList;
        }

        public List<string> GetRoleList()
        {
            return Executor.GetTableSingleColumn(MicrostrategySqlScriptObjects.RoleList).ToList();
        }
        public List<string> GetMstrUserIdsPerGroupName(string groupName)
        {
            return Executor.GetTableSingleColumn(string.Format(MicrostrategySqlScriptObjects.MstrUserIDsPerGroupName, groupName));

        }

        public List<string> GetMstrReportsByReportName(string reportName)
        {
            var result = Executor.GetCompleteTable(string.Format(MicrostrategySqlScriptObjects.GetMstrReportsByReportName,
                reportName));
            var dataRows = result as DataRow[] ?? result.ToArray();
            return dataRows[0].ItemArray.Select(x => x.ToString().Replace("  ", " ")).ToList();
        }

        public List<string> GetUserNameList()
        {
            return Executor.GetTableSingleColumn(MicrostrategySqlScriptObjects.GetUserNameDropDownList);

        }

        public void HardDeleteMstrReportByReportName(string reportName)
        {
            Executor.ExecuteQuery(string.Format(MicrostrategySqlScriptObjects.DeleteMstrReportByReportName, reportName));
        }
        #endregion
        #endregion

        #region first Quadrant

        public bool IsAddIconPresent(string select)
        {
            return SiteDriver.IsElementPresent(string.Format(MicrostrategyMaintenancePageObjects.AddIconXpath, select), How.XPath);
        }


        public bool IsAddIconPresentQuadrantName(string quadrantName)
        {
            return SiteDriver.IsElementPresent(string.Format(MicrostrategyMaintenancePageObjects.AddIconByQuadrantNameXpath, quadrantName), How.XPath);
        }

        public void SelectNucleusUser(string label, string text)
        {
            _sideWindow.SelectDropDownListValueByLabel(label, text);

        }




        public bool IsAddMicrostrategyAddUserGroupFormDisplayed()
        {
            return SiteDriver.IsElementPresent(MicrostrategyMaintenancePageObjects.AddMicrostrategyuserGroupFormByXpath,
                How.XPath);
        }
        public bool IsEditMicrostrategyAddUserGroupFormDisplayed()
        {
            return SiteDriver.IsElementPresent(MicrostrategyMaintenancePageObjects.EditMicrostrategyuserGroupFormByXpath,
                How.XPath);
        }
        public string GetAddUserGroupFormHeader()
        {
            return SiteDriver
                .FindElement(MicrostrategyMaintenancePageObjects.AddMicrostrategyuserGroupFormByXpath, How.XPath).Text;
        }
        public bool IsInputFieldPresentInAddusergroup(string inputfield)
        {
            return JavaScriptExecutor.IsElementPresent(string.Format(MicrostrategyMaintenancePageObjects.InputFieldByLabelXpath, inputfield));
        }
        public bool IsAddReportIconDisabled()
        {
            return SiteDriver
                .FindElementAndGetAttribute(MicrostrategyMaintenancePageObjects.AddReportIconXpath, How.XPath, "class")
                .Contains("is_disabled");
        }

        public bool IsAddusergroupiconDisabled()
        {
            return SiteDriver
                .FindElementAndGetAttribute(MicrostrategyMaintenancePageObjects.AddUsergrpupIconXpath, How.XPath, "class")
                .Contains("is_disabled");
        }

        public bool IsAddMicrostrategyUserGroupFormDisplayed()
        {

            return SiteDriver.IsElementPresent(MicrostrategyMaintenancePageObjects.AddMicrostrategyuserGroupFormByXpath,
                How.XPath);
        }
        public string GetUserGroupFormHeader()
        {
            return SiteDriver
                .FindElement(MicrostrategyMaintenancePageObjects.UsergroupFormHeaderByXpath, How.XPath).Text;
        }

        public string CountOfuserGrpups()
        {

            return Executor.GetSingleStringValue(MicrostrategySqlScriptObjects.CountofUserGroups);
        }

        public string GetUserGroupValuesPerRowCol(int row, int col)
        {

            return JavaScriptExecutor.FindElement(string.Format(MicrostrategyMaintenancePageObjects.GridvalueForUserGroupRowByXpath, row, col)).Text;
        }



        public string GetDetailsOfAddedUserGroup(int i)
        {
            return SiteDriver.GetElementText(string.Format(MicrostrategyMaintenancePageObjects.DetailsOfAddedUserByCss, i), How.CssSelector);
        }

        public void ClickAndRemoveAddedUserGroup()
        {

            JavaScriptExecutor.ClickJQuery(MicrostrategyMaintenancePageObjects.RemoveAddedUserByCss);
            Console.WriteLine("Button Clicked");
        }

        public void ClickOnAddGroupButton()
        {
            JavaScriptExecutor.ClickJQuery(MicrostrategyMaintenancePageObjects.AddGroupButtonByCss);
            Console.WriteLine("Button Clicked");
        }

        public void ClickOnSaveButton()
        {
            SiteDriver.FindElement(MicrostrategyMaintenancePageObjects.SaveButtonByXpath, How.XPath).Click();
            Console.WriteLine("Button Clicked");
        }
        public void ClickOnSaveandCloseButton()
        {
            SiteDriver.FindElement(MicrostrategyMaintenancePageObjects.SaveandCloseButtonByXpath, How.XPath).Click();
            Console.WriteLine("Button Clicked");
        }

        public void ClickOnUpdatePassword()
        {
            SiteDriver.FindElementAndClickOnCheckBox(MicrostrategyMaintenancePageObjects.UpdatePasswordCheeckBoxBycss, How.CssSelector);
            Console.WriteLine("Button Clicked");
        }

        public List<String> GetdetailsOfExistingUserGroup(int i)
        {
            // return SiteDriver.FindElement(string.Format(MicrostrategyMaintenancePageObjects.DetailsOfExisitingUserGroupByCss, i), How.CssSelector).Text;\
            return JavaScriptExecutor.FindElements(string.Format(MicrostrategyMaintenancePageObjects.DetailsOfExisitingUserGroupByCss, i), How.CssSelector, "Text");
        }


        public int GetCreateOrEditedUserGroup(string groupname, string username)
        {
            return (int)Executor.GetSingleValue(string.Format(MicrostrategySqlScriptObjects.CreatedOrEditedUsergroup, groupname, username));
        }

        public int GetUserGroupDataFromDatabase(string groupname, string username)
        {
            return (int)Executor.GetSingleValue(string.Format(MicrostrategySqlScriptObjects.UserGrouptoDelete, groupname, username));
        }



        /*  public void GetDetailsOfExistingUserGroupFromDatabase()
          {
              Executor.GetCompleteTable(MicrostrategySqlScriptObjects.ExistingUserGroupDetailsFromDatabase).ToList();
          }*/

        public bool GetUserGroupRecordStatusFromDatabase(string groupname, string username)
        {
            string check = Executor.GetSingleStringValue(string.Format(MicrostrategySqlScriptObjects.MstrGroupstatus,
                groupname, username));

            return (check == "F") ? false : true;



        }

        public void DeleteUserGroup(string groupname, string username)
        {
            Executor.ExecuteQuery(string.Format(MicrostrategySqlScriptObjects.DeletmstrUserGroupValue, groupname, username));
        }


        #endregion

        #region second quadrant 

        public List<string> GetNucleusUserGridListValueByCol(int col = 2)
        {
            var t = JavaScriptExecutor.FindElements(string.Format(MicrostrategyMaintenancePageObjects.ListValueInGridByQuadrantAndColumnXpath, "Nucleus User", col), How.XPath, "Text");
            return JavaScriptExecutor.FindElements(string.Format(MicrostrategyMaintenancePageObjects.ListValueInGridByQuadrantAndColumnXpath, "Nucleus User", col), How.XPath, "Text");
        }
        public bool IsAddIconDisabled(string quadrantName)
        {
            return SiteDriver.IsElementPresent(string.Format(MicrostrategyMaintenancePageObjects.AddIconDisabledXpath, quadrantName), How.XPath);
        }

        public void ClickOnMstrUserGroupRow(string quadrantName, int row)
        {
            JavaScriptExecutor.ClickJQuery(string.Format(MicrostrategyMaintenancePageObjects.GridRowByQuadrantNameXpathTemplate, quadrantName, row
                    ));
        }

        public List<string> GetGridValueByQuadrantName(string quadrantName, int col = 2)
        {
            var userIdList = JavaScriptExecutor.FindElements(
                string.Format(MicrostrategyMaintenancePageObjects.GridValueByColumnXpathTemplate, quadrantName, col), How.XPath, "Text");
            var sortedList = userIdList.OrderBy(x => x).ToList();
            return sortedList;
        }

        public List<string> GetGridValueInfirstQuadrant(int col = 3)
        {
            string quadrantName = "User Group";
            var userIdList = JavaScriptExecutor.FindElements(
             string.Format(MicrostrategyMaintenancePageObjects.GridValueByColumnXpathTemplate, quadrantName, col), How.XPath, "Text");

            return userIdList;
        }

        public bool IsAddFormPresent()
        {
            return SiteDriver.IsElementPresent(MicrostrategyMaintenancePageObjects.AddFormCssSelector,
                How.CssSelector);
        }
        public void ClickOnGridRowByValue(string value)
        {
            JavaScriptExecutor.ClickJQuery(string.Format(MicrostrategyMaintenancePageObjects.GridRowByValue, value));
        }
        #endregion
    }
}

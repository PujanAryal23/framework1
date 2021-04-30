using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nucleus.Service.Data;
using Nucleus.Service.PageObjects.Appeal;
using Nucleus.Service.PageObjects.Default;
using Nucleus.Service.PageObjects.Settings;
using Nucleus.Service.PageServices.Base.Default;
using Nucleus.Service.SqlScriptObjects.Common_SQL;
using Nucleus.Service.SqlScriptObjects.EditSettingsManager;
using Nucleus.Service.SqlScriptObjects.RoleManager;
using Nucleus.Service.SqlScriptObjects.Settings;
using Nucleus.Service.SqlScriptObjects.User;
using Nucleus.Service.Support.Common;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Environment;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Core.Navigation;
using UIAutomation.Framework.Database;
using UIAutomation.Framework.Elements;
using UIAutomation.Framework.Utils;
using static System.String;
using static System.Console;


namespace Nucleus.Service.PageServices.Settings
{
    public class RoleManagerPage : NewDefaultPage
    {
        #region PRIVATE FIELDS

        private RoleManagerPageObject _roleManagerPage;
        private SideBarPanelSearch _sideBarPanelSearch;
        private GridViewSection _gridViewSection;
        private SideWindow _sideWindow;


        #endregion

        #region PUBLIC PROPERTIES

        public RoleManagerPageObject RoleManager
        {
            get { return _roleManagerPage; }
        }

        #endregion

        #region CONSTRUCTOR

        public RoleManagerPage(INewNavigator navigator, RoleManagerPageObject roleManagerPage, ISiteDriver siteDriver, IJavaScriptExecutors javaScriptExecutors, IEnvironmentManager environmentManager, IBrowserOptions browserOptions, IOracleStatementExecutor executor)
            : base(navigator, roleManagerPage, siteDriver, javaScriptExecutors, environmentManager, browserOptions, executor)
        {

            _roleManagerPage = (RoleManagerPageObject)PageObject;
            _sideBarPanelSearch = new SideBarPanelSearch(new CalenderPage(SiteDriver), SiteDriver, JavaScriptExecutor);
            _gridViewSection = new GridViewSection(SiteDriver, JavaScriptExecutor);
            _sideWindow = new SideWindow(new CalenderPage(SiteDriver), SiteDriver, JavaScriptExecutor);

        }
        #endregion

        #region PROPERTIES

        public GridViewSection GetGridViewSection
        {
            get { return _gridViewSection; }
        }

        public SideWindow GetSideWindow
        {
            get { return _sideWindow; }
        }

        public SideBarPanelSearch SideBarPanelSearch
        {
            get { return _sideBarPanelSearch; }
        }

        #endregion

        #region Database
        public void CloseDatabaseConnection()
        {
            Executor.CloseConnection();
        }

        #endregion

        #region Public Methods

        public void ClickEditIconByRow(int row = 1) =>
            SiteDriver.FindElement(Format(RoleManagerPageObject.EditIconByRowCssSelector, row), How.CssSelector).Click();

        public void ClickDeleteIconByRow(int row = 1) =>
            JavaScriptExecutor.ExecuteClick(Format(RoleManagerPageObject.DeleteIconByRowCssSelector, row), How.CssSelector);

        public void SelectAvailableAuthoritiesByRow(int row = 1) =>
            SiteDriver.FindElement(Format(RoleManagerPageObject.AvailableAuthoritiesByRowXPath, row), How.XPath).Click();

        public void ClickAuthorityOptionByName(string availableOrAssignedAuthority, string authorityName)
        {
            if (availableOrAssignedAuthority == CreateNewRoleFormEnum.AvailableAuthorities.GetStringValue())
                JavaScriptExecutor.ExecuteClick(Format(RoleManagerPageObject.AuthorityByNameXPath,
                    CreateNewRoleFormEnum.AvailableAuthorities.GetStringValue(), authorityName), How.XPath);

            else if (availableOrAssignedAuthority == CreateNewRoleFormEnum.AssignedAuthorities.GetStringValue())
                JavaScriptExecutor.ExecuteClick(Format(RoleManagerPageObject.AuthorityByNameXPath,
                    CreateNewRoleFormEnum.AssignedAuthorities.GetStringValue(), authorityName), How.XPath);

            else
                throw new ArgumentException("Invalid arguments");
        }

        public void DeselectAssignedAuthoritiesByRow(int row = 1) =>
            JavaScriptExecutor.ExecuteClick(Format(RoleManagerPageObject.AssignedAuthoritiesByRowCssSelector, row), How.CssSelector);

        public void ClickEditIconByRoleNameAndUserType(string roleName, string userType) =>
            SiteDriver.FindElement(
                Format(RoleManagerPageObject.EditIconByRoleNameAndUserTypeXPath, roleName, userType), How.XPath).Click();

        public void SelectCreateNewRoleAvailableAuthorities(int row = 1) =>
            SiteDriver.FindElement(
                    Format(RoleManagerPageObject.CreateNewRoleAvailableAuthoritiesByRowCssSelector, row),
                    How.CssSelector)
                .Click();

        public void SetRoleDescription(string value)
        {
            JavaScriptExecutor.FindElement(RoleManagerPageObject.RoleDescriptionInputFieldCssSelector)
                .ClearElementField();
            JavaScriptExecutor.FindElement(RoleManagerPageObject.RoleDescriptionInputFieldCssSelector)
                .SendKeys(value);
        }

        public List<string> GetAssignedAuthoritiesList()
        {
            return JavaScriptExecutor.FindElements(RoleManagerPageObject.AssignedAuthoritiesXPath, How.XPath, "Text");
        }

        public List<string> GetAvailableAuthoritiesList()
        {
            return JavaScriptExecutor.FindElements(RoleManagerPageObject.AvailableAuthoritiesXPath, How.XPath,
                "Text");
        }

        public List<string> GetAvailableAuthoritiesListForUserTypeFromDB(string userType)
        {
            switch (userType.ToLower())
            {
                case "internal":
                    return Executor.GetTableSingleColumn(RoleManagerSqlScriptObject
                        .GetAvailableAuthoritiesListForInternalUserFromDB);
                    break;

                case "client":
                    return Executor.GetTableSingleColumn(RoleManagerSqlScriptObject
                        .GetAvailableAuthoritiesListForClientUserFromDB);
                    break;

                default:
                    throw new InvalidEnumArgumentException("Invalid User Type");
            }
        }

        public bool IsAddRoleIconPresent() =>
            SiteDriver.IsElementPresent(RoleManagerPageObject.AddIconCssSelector, How.CssSelector);

        public void ClickAddRoleIcon() =>
            SiteDriver.FindElement(RoleManagerPageObject.AddIconCssSelector, How.CssSelector).Click();

        public string GetAddRoleIconTooltip() =>
            SiteDriver.FindElement(RoleManagerPageObject.AddIconCssSelector, How.CssSelector).GetAttribute("title");

        public bool IsCreateNewFormPresent() =>
            JavaScriptExecutor.IsElementPresent(RoleManagerPageObject.CreateNewRoleFormCssSelector);

        public void ClickSelectAll() =>
            JavaScriptExecutor.FindElement(RoleManagerPageObject.SelectAllCssSelector).Click();

        public void ClickDeselectAll() =>
            JavaScriptExecutor.FindElement(RoleManagerPageObject.DeselectAllCssSelector).Click();

        public void CreateNewRole(string roleName, string userType, string roleDescription, List<string> authorities)
        {
            if (!IsCreateNewFormPresent())
                ClickAddRoleIcon();
            GetSideWindow.SetInputInInputFieldByLabel(CreateNewRoleFormEnum.RoleName.GetStringValue(), roleName);
            GetSideWindow.SelectDropDownValue(CreateNewRoleFormEnum.UserType.GetStringValue(), userType);
            GetSideWindow.SetInputInInputFieldByLabel(CreateNewRoleFormEnum.RoleDescription.GetStringValue(), roleDescription);
            if (authorities != null)
                foreach (var authority in authorities)
                    ClickAuthorityOptionByName(CreateNewRoleFormEnum.AvailableAuthorities.GetStringValue(), authority);
            GetSideWindow.Save(waitForWorkingMessage: true);
        }

        public void DeleteRoleByRoleNameAndUserType(string roleName, string roleType)
        {
            WriteLine($"Deleting role : '{roleName}' for {roleType} user");
            var columnName = roleType == "Internal" ? "internal_role_id" : "client_role_id";
            Executor.ExecuteQuery(Format(RoleManagerSqlScriptObject.DeleteRoleByRoleNameFromDB, columnName, roleName));
        }

        public void InputDataInCreateNewRoleFormByLabel(string label, string value)
        {
            var element = JavaScriptExecutor.FindElement(Format(RoleManagerPageObject.CreateNewRoleFormInputFieldByLabelCssSelector, label));
            element.ClearElementField();
            element.SendKeys(value);
        }

        public void ClearRoleDescription() => JavaScriptExecutor.FindElement(RoleManagerPageObject.RoleDescriptionInputFieldCssSelector).ClearElementField();

        public string GetInputDataByLabelInCreateNewRoleForm(string label)
        {
            return JavaScriptExecutor.GetText(Format(RoleManagerPageObject.CreateNewRoleFormInputFieldByLabelCssSelector, label));
        }

        public bool IsEditIconPresent(int row = 1)
        {
            return SiteDriver.IsElementPresent(Format(RoleManagerPageObject.EditIconByRowCssSelector, row),
                How.CssSelector);
        }

        public List<string> GetAllRoleNameListFromDb()
        {
            var list = Executor.GetTableSingleColumn(UserSqlScriptObjects.GetAllRoleName);
            return list;
        }

        public List<string> GetAllRoleNameValueList()
        {
            return JavaScriptExecutor.FindElements(RoleManagerPageObject.RoleNameValueCssSelector, How.CssSelector, "Text").ToList();
        }

        public List<string> GetAllUserTypeValueList()
        {
            return JavaScriptExecutor.FindElements(RoleManagerPageObject.UserTypeValueCssSelector, How.CssSelector, "Text")
                .ToList();
        }

        public IEnumerable<DataRow> GetAllRoleNameAndUserTypeFromDB()
        {
            var dataValues = Executor.GetCompleteTable(RoleManagerSqlScriptObject.GetRoleNameAndUserTypeFromDB);
            return dataValues;
        }

        public List<string> GetRoleNameValueByUserTypeFromDb(string type)
        {
            List<string> list = new List<string>();
            switch (type)
            {
                case "Internal":
                    list = Executor.GetTableSingleColumn(Format(UserSqlScriptObjects.GetRoleNameByUserType, 2));
                    break;
                case "Client":
                    list = Executor.GetTableSingleColumn(Format(UserSqlScriptObjects.GetRoleNameByUserType, 8));
                    break;
                default:
                    list = Executor.GetTableSingleColumn(UserSqlScriptObjects.GetAllRoleName);
                    break;
            }
            return list;
        }

        public List<string> GetRoleAuditForCreateAndModifyFromDB(string action)
        {
            List<string> dataValues;
            switch (action)
            {
                case "Modify":
                    dataValues = Executor.GetTableSingleColumn(Format(RoleManagerSqlScriptObject.GetRoleAuditForCreateAndModify, "E")).ToList();
                    break;
                case "Create":
                default:
                    dataValues = Executor.GetTableSingleColumn(Format(RoleManagerSqlScriptObject.GetRoleAuditForCreateAndModify, "I")).ToList();
                    break;
            }
            return dataValues;
        }
        #endregion


    }
}

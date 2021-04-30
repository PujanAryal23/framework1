using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Nucleus.Service.Data;
using Nucleus.Service.PageServices.Batch;
using Nucleus.Service.PageServices.Claim;
using Nucleus.Service.PageServices.Settings;
using Nucleus.Service.PageServices.Settings.User;
using Nucleus.Service.Support.Common.Constants;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Environment;
using Nucleus.Service.Support.Menu;
using Nucleus.UIAutomation.TestSuites.Base;
using Nucleus.UIAutomation.TestSuites.Common;
using NUnit.Framework;
using UIAutomation.Framework.Utils;


namespace Nucleus.UIAutomation.TestSuites.TestCases
{
    public class RoleManager : NewAutomatedBase
    {
        #region Private

        private RoleManagerPage _roleManager;
        private CommonValidations _commonValidations;
        private ProfileManagerPage _profileManager;

        #endregion

        #region DBinteraction methods
        private void RetrieveListFromDatabase()
        {

        }
        #endregion

        #region PROTECTED PROPERTIES
        protected override string FullyQualifiedClassName
        {
            get { return GetType().FullName; }
        }
        #endregion

        #region OVERRIDE

        protected override void ClassInit()
        {
            try
            {
                base.ClassInit();
                CurrentPage = _roleManager = CurrentPage.NavigateToRoleManager();
                _commonValidations = new CommonValidations(CurrentPage);
                RetrieveListFromDatabase();
            }
            catch (Exception)
            {
                if (StartFlow != null)
                    StartFlow.Dispose();
                throw;
            }
        }


        protected override void TestInit()
        {
            _roleManager.CloseDatabaseConnection();
            base.TestInit();

        }

        protected override void ClassCleanUp()
        {
            base.ClassCleanUp();
        }

        protected override void TestCleanUp()
        {
            base.TestCleanUp();
            if (string.Compare(UserType.CurrentUserType, UserType.HCIADMIN, StringComparison.OrdinalIgnoreCase) != 0)
            {
                _roleManager = _roleManager.Logout().LoginAsHciAdminUser().NavigateToRoleManager();
            }

            if (_roleManager.GetPageHeader() != Extensions.GetStringValue(PageHeaderEnum.RoleManager))
            {
                _roleManager.ClickOnQuickLaunch().NavigateToNewUserProfileSearch();
            }

            if(!_roleManager.SideBarPanelSearch.IsSideBarPanelOpen())
                _roleManager.SideBarPanelSearch.ClickOnToggleSidebarPanelButton();
        }
        #endregion

        #region TestSuites

        [Test] //CAR-2752(CAR-2225)
        public void Verify_primary_grid_display()
        {
            _roleManager.SideBarPanelSearch.SelectDropDownListValueByLabel(CreateNewRoleFormEnum.UserType.GetStringValue(), "All");

            var allRoleNamesInGrid = _roleManager.GetGridViewSection.GetGridListValueByCol();
            var allUserTypesInGrid = _roleManager.GetGridViewSection.GetGridListValueByCol(3);
            
            _roleManager.GetGridViewSection.GetLabelInGridByColRow()
                .ShouldBeEqual("Role Name:", "'Role Name' label should be shown in the grid");
            _roleManager.GetGridViewSection.GetLabelInGridByColRow(3)
                .ShouldBeEqual("User Type:", "'User Type' label should be shown in the grid");

            allRoleNamesInGrid.IsInAscendingOrder()
                .ShouldBeTrue("Role Name should be in alphabetical order");
            allUserTypesInGrid.Distinct()
                .ShouldCollectionBeEquivalent(new List<string> { "Internal", "Client" }, "User Type should be Same");

            var roleNameAndUserTypesFromDB = _roleManager.GetAllRoleNameAndUserTypeFromDB();
            var listOfRoleNames = roleNameAndUserTypesFromDB.Select(x => x.ItemArray[0]).ToList();
            var listOfUserTypes = roleNameAndUserTypesFromDB.Select(x => x.ItemArray[1]).ToList();

            allRoleNamesInGrid.ShouldCollectionBeEquivalent(listOfRoleNames, "Are all the roles shown in the grid present in the DB");
            allUserTypesInGrid.ShouldCollectionBeEquivalent(listOfUserTypes, "Are all the user types shown in the grid present in the DB");
        }

        [Test] //CAR-2227(CAR-2754)+CAR-2815(CAR-2743)
        public void Verify_edit_functionality()
         {
            const string userType = "Internal";
            const string roleName = "Test Internal Role";           
            try
            {
                StringFormatter.PrintMessage("Deleting the test role");
                _roleManager.DeleteRoleByRoleNameAndUserType("Test Internal Role", userType);
                StringFormatter.PrintMessage("Verification if edit icon is present");
                _roleManager.GetSideWindow.SelectDropDownListValueByLabel(CreateNewRoleFormEnum.UserType.GetStringValue(), "Internal");
                _roleManager.GetGridViewSection.IsEnabledEditIconPresent().ShouldBeTrue("Is Edit Icon present?");               
                #region Form Validation
                _roleManager.ClickEditIconByRoleNameAndUserType("TestAutomationInternal", userType);
                _roleManager.ClickDeselectAll();
                _roleManager.GetSideWindow.IsDropDownPresentByLabel(CreateNewRoleFormEnum.UserType.GetStringValue())
                    .ShouldBeFalse("User Type field is not editable, so should not be present in the edit form");
               
                var roleDescription = _roleManager.GetSideWindow.GetInputValueByLabel(CreateNewRoleFormEnum.RoleDescription.GetStringValue());

                StringFormatter.PrintMessage("Validation of form with empty Role Description and assigned authorities");
                _roleManager.GetSideWindow.SetInputInInputFieldByLabel(CreateNewRoleFormEnum.RoleDescription.GetStringValue(), "");
                _roleManager.GetSideWindow.Save();
                _roleManager.IsPageErrorPopupModalPresent()
                    .ShouldBeTrue("Error popup should be present when trying to save without any role description");
                _roleManager.ClosePageError();
                _roleManager.IsInvalidInputPresentOnTransferComponentByLabel(CreateNewRoleFormEnum.AssignedAuthorities.GetStringValue())
                    .ShouldBeTrue($"Is invalid highlight shown in {CreateNewRoleFormEnum.AssignedAuthorities.GetStringValue()}?");
                _roleManager.IsInvalidInputPresentByLabel(CreateNewRoleFormEnum.RoleDescription.GetStringValue())
                    .ShouldBeTrue($"Is invalid highlight shown in {CreateNewRoleFormEnum.RoleDescription.GetStringValue()}?");

                StringFormatter.PrintMessage($"Validation of form with empty {CreateNewRoleFormEnum.AssignedAuthorities.GetStringValue()}");
                _roleManager.SetRoleDescription(roleDescription);
                _roleManager.GetSideWindow.Save();
                _roleManager.IsPageErrorPopupModalPresent()
                     .ShouldBeTrue("Error popup should be present when trying to save without any authority assigned");
                _roleManager.ClosePageError();
                _roleManager.IsInvalidInputPresentOnTransferComponentByLabel(CreateNewRoleFormEnum.AssignedAuthorities.GetStringValue())
                    .ShouldBeTrue($"Is invalid highlight shown in {CreateNewRoleFormEnum.AssignedAuthorities.GetStringValue()}?");

                StringFormatter.PrintMessage("Validation of form with empty Role Description");
                _roleManager.SelectAvailableAuthoritiesByRow();
                _roleManager.GetSideWindow.SetInputInInputFieldByLabel(CreateNewRoleFormEnum.RoleDescription.GetStringValue(), "");
                _roleManager.GetSideWindow.Save();
                _roleManager.IsPageErrorPopupModalPresent()
                    .ShouldBeTrue("Error popup should be present when trying to save without any modification");
                _roleManager.ClosePageError();
                _roleManager.IsInvalidInputPresentByLabel(CreateNewRoleFormEnum.RoleDescription.GetStringValue())
                    .ShouldBeTrue("Is invalid highlight shown in ?");
                _roleManager.GetSideWindow.Cancel();

                #endregion
                #region EditRole
                StringFormatter.PrintMessage("Creation of new role");
                 roleDescription = "Test Role for Internal Users";
                _roleManager.CreateNewRole(roleName, userType, roleDescription,
                     new List<string> { "Role Test Authority Internal User Type #2" });
                StringFormatter.PrintMessage("Verification that existing saved values are present");
                 _roleManager.GetGridViewSection.GetGridListValueByCol().ShouldContain(roleName,
                     "Existing saved Role Name should be present");
                 _roleManager.ClickEditIconByRoleNameAndUserType(roleName, userType);
                 _roleManager.GetSideWindow.GetInputValueByLabel(CreateNewRoleFormEnum.RoleDescription.GetStringValue()).ShouldBeEqual(roleDescription,
                     "Existing saved Role Description should be present");
                StringFormatter.PrintMessage("Verification that available and assigned authorities should be distinct");
                 _roleManager.GetAssignedAuthoritiesList().CollectionShouldNotBeSubsetOf(_roleManager.GetAvailableAuthoritiesList(),
                     "Assigned Authorities should not be listed in available authority");
                StringFormatter.PrintMessage("Verification that available authorities list doesn't contain the authorities assigned to other roles");
                 var availableAuthoritiesFromForm = _roleManager.GetAvailableAuthoritiesList();
                 var availableAuthoritiesFromDB = _roleManager.GetAvailableAuthoritiesListForUserTypeFromDB(userType);
                 availableAuthoritiesFromDB.ShouldCollectionBeEquivalent(availableAuthoritiesFromForm,
                     $"{CreateNewRoleFormEnum.AvailableAuthorities.GetStringValue()} list should be correct");
                 var editedRoleDescription = "Edited Role Description";
                 var previousAssignedAuthorities = _roleManager.GetAssignedAuthoritiesList();
                 var previousAvailableAuthorities = _roleManager.GetAvailableAuthoritiesList();
                 var previousRoleDescription = _roleManager.GetSideWindow.GetInputValueByLabel(CreateNewRoleFormEnum.RoleDescription.GetStringValue());
                StringFormatter.PrintMessage("Verification of Cancel Functionality");
                 EditRole(editedRoleDescription, 1);
                var modifiedAssignedAuthorities = _roleManager.GetAssignedAuthoritiesList();
                 var modifiedAvailableAuthorities = _roleManager.GetAvailableAuthoritiesList();
                _roleManager.GetSideWindow.Cancel();
                 _roleManager.ClickEditIconByRoleNameAndUserType(roleName, userType);
                 _roleManager.GetSideWindow.GetInputValueByLabel(CreateNewRoleFormEnum.RoleDescription.GetStringValue())
                     .ShouldBeEqual(previousRoleDescription, $"Cancel should not save any modification to " +
                                                             $"{CreateNewRoleFormEnum.RoleDescription.GetStringValue()}");
                 _roleManager.GetAssignedAuthoritiesList().ShouldCollectionBeNotEqual(modifiedAssignedAuthorities,
                     $"Cancel should not save any modification to {CreateNewRoleFormEnum.AssignedAuthorities.GetStringValue()}");
                 _roleManager.GetAvailableAuthoritiesList().ShouldCollectionBeNotEqual(modifiedAvailableAuthorities, 
                     $"Cancel should not save any modification to {CreateNewRoleFormEnum.AvailableAuthorities.GetStringValue()}");
                StringFormatter.PrintMessage("Verification of Save functionality");
                 EditRole(editedRoleDescription, 1);
                 _roleManager.GetSideWindow.Save();
                 _roleManager.ClickEditIconByRoleNameAndUserType(roleName, userType);
                 _roleManager.GetSideWindow.GetInputValueByLabel(CreateNewRoleFormEnum.RoleDescription.GetStringValue())
                     .ShouldBeEqual(editedRoleDescription, 
                         $"Updated role description should be displayed in {CreateNewRoleFormEnum.RoleDescription.GetStringValue()}");
                 _roleManager.GetAssignedAuthoritiesList()
                     .ShouldCollectionBeEqual(modifiedAssignedAuthorities, "Updated assigned authority should be displayed");
                 _roleManager.GetAvailableAuthoritiesList().ShouldCollectionBeEqual(modifiedAvailableAuthorities, 
                     "Updated available authority should be displayed");
                //Verify role audit creation after modify
                var t = DateTime.UtcNow;
                Convert.ToDateTime(_roleManager.GetRoleAuditForCreateAndModifyFromDB("Modify")[0]).AssertDateRange(CurrentPage.CurrentDateTimeInMst(t).AddMinutes(-3), CurrentPage.CurrentDateTimeInMst(t).AddMinutes(3), "An audit should be created for the role modification.");

               #endregion
            }
            finally
            {
                if (_roleManager.IsPageErrorPopupModalPresent())
                    _roleManager.ClosePageError();
                StringFormatter.PrintMessage("Deleting the test role");
                 _roleManager.DeleteRoleByRoleNameAndUserType(roleName, userType);
                 _roleManager.RefreshPage();
            }
            void EditRole(string description, int row)
            {
                _roleManager.SetRoleDescription(description);
                _roleManager.ClickDeselectAll();
                _roleManager.SelectAvailableAuthoritiesByRow(row);
            }
         }

        [Test] //CAR-2224 [CAR-2751] + CAR-2815(CAR-2743)
        public void Verify_create_new_user_role()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            IDictionary<string, string> paramLists = DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
            var userTypes = paramLists["UserTypes"].Split(';').ToList();
            try
            {
                StringFormatter.PrintMessage("Deleting the test role");
                _roleManager.DeleteRoleByRoleNameAndUserType("Test Internal Role For Creating New User", userTypes[0]);
                StringFormatter.PrintMessageTitle("Verify clicking the 'Add Role' icon");
                _roleManager.IsAddRoleIconPresent().ShouldBeTrue("Add Icon should be present");
                _roleManager.GetAddRoleIconTooltip().ShouldBeEqual("Create New Role", "Is tooltip as expected?");
                _roleManager.ClickAddRoleIcon();
                _roleManager.IsCreateNewFormPresent()
                    .ShouldBeTrue("'Create New Role' form should be present when 'Add Role' icon is clicked");
                #region FORM VALIDATION TESTS
                StringFormatter.PrintMessageTitle("Form Validation Tests");
                _roleManager.GetSideWindow.IsAsertiskPresent(CreateNewRoleFormEnum.RoleName.GetStringValue())
                    .ShouldBeTrue($"'{CreateNewRoleFormEnum.RoleName.GetStringValue()}' should have an asterisk sign");
                _roleManager.GetSideWindow.SetInputInInputFieldByLabel(CreateNewRoleFormEnum.RoleName.GetStringValue(),
                    _roleManager.RandomString(101));
                _roleManager.GetSideWindow.GetInputValueByLabel(CreateNewRoleFormEnum.RoleName.GetStringValue()).Length
                    .ShouldBeEqual(100,
                        $"'{CreateNewRoleFormEnum.RoleName.GetStringValue()}' accepts only 100 characters");
                var userTypeListInDropdown =
                    _roleManager.GetSideWindow.GetDropDownList(CreateNewRoleFormEnum.UserType.GetStringValue());
                userTypeListInDropdown.RemoveAt(0);
                userTypeListInDropdown.ShouldCollectionBeEqual(userTypes,
                    $"'{CreateNewRoleFormEnum.UserType.GetStringValue()}' " +
                    "dropdown values should be correct");
                _roleManager.GetSideWindow.IsAsertiskPresent(CreateNewRoleFormEnum.UserType.GetStringValue())
                    .ShouldBeTrue($"'{CreateNewRoleFormEnum.UserType.GetStringValue()}' should have an asterisk sign");
                _roleManager.GetSideWindow.SetInputInInputFieldByLabel(
                    CreateNewRoleFormEnum.RoleDescription.GetStringValue(),
                    _roleManager.RandomString(256));
                _roleManager.GetSideWindow.GetInputValueByLabel(CreateNewRoleFormEnum.RoleDescription.GetStringValue())
                    .Length
                    .ShouldBeEqual(255,
                        $"'{CreateNewRoleFormEnum.RoleDescription.GetStringValue()}' accepts only 255 characters");
                _roleManager.GetSideWindow.IsAsertiskPresent(CreateNewRoleFormEnum.RoleDescription.GetStringValue())
                    .ShouldBeTrue(
                        $"'{CreateNewRoleFormEnum.RoleDescription.GetStringValue()}' should have an asterisk sign");
                StringFormatter.PrintMessageTitle("Validation of Save button");
                _roleManager.GetSideWindow.Cancel();
                _roleManager.ClickAddRoleIcon();
                _roleManager.GetSideWindow.Save();
                _roleManager.IsPageErrorPopupModalPresent()
                    .ShouldBeTrue("Page error should pop up when required fields are not entered");
                _roleManager.GetPageErrorMessage().ShouldBeEqual("Some required values are missing.",
                    "Error popup should display correct error message");
                _roleManager.ClosePageError();
                _roleManager.IsInvalidInputPresentByLabel(CreateNewRoleFormEnum.RoleName.GetStringValue())
                    .ShouldBeTrue("Is Invalid Input present for Role Name field?");
                _roleManager.IsInvalidInputPresentByLabel(CreateNewRoleFormEnum.UserType.GetStringValue())
                    .ShouldBeTrue("Is Invalid Input present for User Type field?");
                _roleManager.IsInvalidInputPresentByLabel(CreateNewRoleFormEnum.RoleDescription.GetStringValue())
                    .ShouldBeTrue("Is Invalid Input present for Role Description field?");
                _roleManager.GetCountOfInvalidRed()
                    .ShouldBeEqual(4, "All required fields should show the red highlight when no input is provided");
                StringFormatter.PrintMessageTitle(
                    $"Validation of the '{CreateNewRoleFormEnum.AvailableAuthorities.GetStringValue()}'" +
                    $"and '{CreateNewRoleFormEnum.AssignedAuthorities.GetStringValue()}' fields");
                foreach (var userType in userTypes)
                {
                    _roleManager.GetSideWindow.SelectDropDownValue(CreateNewRoleFormEnum.UserType.GetStringValue(), userType);
                    var availableAuthoritiesFromForm = _roleManager.GetAvailableAuthoritiesList();
                    var availableAuthoritiesFromDB = _roleManager.GetAvailableAuthoritiesListForUserTypeFromDB(userType);
                    availableAuthoritiesFromDB.ShouldCollectionBeEquivalent(availableAuthoritiesFromForm,
                        $"{CreateNewRoleFormEnum.AssignedAuthorities.GetStringValue()} list should be correct");
                    _roleManager.GetAssignedAuthoritiesList().Count.ShouldBeEqual(0,
                        $"The '{CreateNewRoleFormEnum.AssignedAuthorities.GetStringValue()}' list should be empty before any authority is assigned.");
                    _roleManager.ClickSelectAll();
                    _roleManager.GetAssignedAuthoritiesList().ShouldCollectionBeEquivalent(availableAuthoritiesFromForm, "");
                    _roleManager.GetAvailableAuthoritiesList().Count.ShouldBeEqual(0, "Available Authorities should be correct");
                    _roleManager.ClickDeselectAll();
                    _roleManager.GetAvailableAuthoritiesList().ShouldCollectionBeEquivalent(availableAuthoritiesFromForm, "Available Authorities should be correct after desselect selection");
                    _roleManager.GetAssignedAuthoritiesList().Count.ShouldBeEqual(0, "Assigned Authorities should be correct");
                }                 
                StringFormatter.PrintMessageTitle("Validating creating a new role without entering required fields");
                VerifySavingFormWithoutEnteringRequiredFields();
                    #endregion
                #region CREATE NEW ROLE TESTS
                StringFormatter.PrintMessageTitle("Verifying creating a role");
                _roleManager.CreateNewRole("Test Internal Role For Creating New User", userTypes[0], "Test Role for Internal Users",
                    new List<string> { "Role Test Authority Internal User Type #1" });
                //Verifying a newly created role in audit
                var t = DateTime.UtcNow;
                Convert.ToDateTime(_roleManager.GetRoleAuditForCreateAndModifyFromDB("Create")[0]).AssertDateRange(CurrentPage.CurrentDateTimeInMst(t).AddMinutes(-3), CurrentPage.CurrentDateTimeInMst(t).AddMinutes(3), "An audit should be created for the role creation.");
                _roleManager.IsPageErrorPopupModalPresent()
                    .ShouldBeFalse("New role should be successfully created for valid inputs");
                StringFormatter.PrintMessage(
                    "Verifying whether newly created role is present appears in the Role Manager grid");
                var rowNum = _roleManager.GetGridViewSection.GetGridRowNumberByColAndLabel("Test Internal Role For Creating New User");
                _roleManager.GetGridViewSection.GetValueInGridByColRow(row: rowNum).ShouldBeEqual("Test Internal Role For Creating New User",
                    "Newly created role should appear in the grid");
                _roleManager.GetGridViewSection.GetValueInGridByColRow(3, rowNum).ShouldBeEqual(userTypes[0],
                    "Newly created role should have correct user type displayed in the grid");
                StringFormatter.PrintMessageTitle(
                    "Verifying user should not be allowed to create another role with the same name");
                _roleManager.CreateNewRole("Test Internal Role For Creating New User", userTypes[0], "New role description",
                    new List<string> { "Role Test Authority Internal User Type #2" });
                _roleManager.IsPageErrorPopupModalPresent()
                    .ShouldBeTrue("Creation of new role with an existing role's name is not allowed");
                _roleManager.GetPageErrorMessage()
                    .ShouldBeEqual("The selected Role Name already exists. Please enter a new Role Name.",
                        "Error message should be displayed correctly");
                _roleManager.ClosePageError();
                #endregion
            }
            finally
            {
                if(_roleManager.IsPageErrorPopupModalPresent())
                    _roleManager.ClosePageError();
                if (_roleManager.IsCreateNewFormPresent())
                    _roleManager.GetSideWindow.Cancel();
                StringFormatter.PrintMessage("Finally, deleting the test role");
                _roleManager.DeleteRoleByRoleNameAndUserType("Test Internal Role For Creating New User", userTypes[0]);
                _roleManager.RefreshPage();
            }
        }

        [Test] //CAR-2226[CAR-2753]
        public void Verify_search_existing_user_roles()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            List<string> userTypeList = DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName, "UserType", "Value").Split(';').ToList();
            var list = _roleManager.GetAllRoleNameListFromDb();
            var roleNameListByUserType = _roleManager.GetRoleNameValueByUserTypeFromDb(userTypeList[2]);
            try
            {
                StringFormatter.PrintMessageTitle("Verify finding existing user roles");
                _roleManager.SideBarPanelSearch.GetAvailableDropDownList("User Type")
                    .ShouldCollectionBeEquivalent(userTypeList, "The search should auto-execute on landing");
                _roleManager.SideBarPanelSearch.IsSideBarPanelOpen().ShouldBeTrue("Sidebar panel should be present and open");
                _roleManager.GetGridViewSection.GetGridListValueByCol().ShouldCollectionBeEqual(list, "The following role names should be present.");
                _roleManager.SideBarPanelSearch.SelectDropDownListValueByLabel("User Type", userTypeList[2]);
                _roleManager.GetGridViewSection.GetGridListValueByCol(3).Distinct().All(x=> x == "Client")
                    .ShouldBeTrue("Search should be performed using the User Type.");
                    
                _roleManager.GetGridViewSection.GetGridListValueByCol().ShouldCollectionBeEqual(roleNameListByUserType, 
                        "Search executed should consist only the role name assigned to the user type.");
            }

            finally
            {
                if (_roleManager.IsPageErrorPopupModalPresent())
                    _roleManager.ClosePageError();
            }
        }
        #endregion

        #region PUBLIC METHODS

        public void VerifySavingFormWithoutEnteringRequiredFields()
        {
            StringFormatter.PrintMessage($"Entering all data except {CreateNewRoleFormEnum.RoleName.GetStringValue()}");
            _roleManager.CreateNewRole("", "Internal", "Role created for Test Purpose",
                new List<string> { "Role Test Authority Internal User Type #1", "Role Test Authority Internal User Type #2" });
            _roleManager.IsPageErrorPopupModalPresent().ShouldBeTrue("Name of the role must be specified");
            _roleManager.GetPageErrorMessage().ShouldBeEqual("Some required values are missing.");
            _roleManager.ClosePageError();
            _roleManager.IsInvalidInputPresentByLabel(CreateNewRoleFormEnum.RoleName.GetStringValue()).
                ShouldBeTrue("Invalid highlight should be shown");
            _roleManager.GetSideWindow.Cancel();

            StringFormatter.PrintMessage($"Entering all data except {CreateNewRoleFormEnum.UserType.GetStringValue()}");
            _roleManager.CreateNewRole("Test Role Name", "Please Select", "Role created for Test Purpose", null);
            _roleManager.IsPageErrorPopupModalPresent().ShouldBeTrue("User type should be specified");
            _roleManager.GetPageErrorMessage().ShouldBeEqual("Some required values are missing.");
            _roleManager.ClosePageError();
            _roleManager.IsInvalidInputPresentByLabel(CreateNewRoleFormEnum.UserType.GetStringValue()).
                ShouldBeTrue("Invalid highlight should be shown");
            _roleManager.GetSideWindow.Cancel();

            StringFormatter.PrintMessage($"Entering all data except {CreateNewRoleFormEnum.RoleDescription.GetStringValue()}");
            _roleManager.CreateNewRole("Test Role Name", "Internal", "",
                new List<string> { "Role Test Authority Internal User Type #1", "Role Test Authority Internal User Type #2" });
            _roleManager.IsPageErrorPopupModalPresent().ShouldBeTrue("Role description should be specified");
            _roleManager.GetPageErrorMessage().ShouldBeEqual("Some required values are missing.");
            _roleManager.ClosePageError();
            _roleManager.IsInvalidInputPresentByLabel(CreateNewRoleFormEnum.RoleDescription.GetStringValue())
                .ShouldBeTrue("Invalid highlight should be shown");
            _roleManager.GetSideWindow.Cancel();

            StringFormatter.PrintMessage($"Entering all data except value in {CreateNewRoleFormEnum.AssignedAuthorities.GetStringValue()}");
            _roleManager.CreateNewRole("Test Role Name", "Internal", "Role created for Test Purpose", null);
            _roleManager.IsPageErrorPopupModalPresent().ShouldBeTrue("At least one authority should be assigned");
            _roleManager.GetPageErrorMessage().ShouldBeEqual("Some required values are missing.");
            _roleManager.ClosePageError();
            _roleManager.IsInvalidInputPresentOnTransferComponentByLabel(CreateNewRoleFormEnum.AssignedAuthorities.GetStringValue())
                .ShouldBeTrue("Invalid highlight should be shown");
            _roleManager.GetSideWindow.Cancel();
        }

        #endregion
    }
}

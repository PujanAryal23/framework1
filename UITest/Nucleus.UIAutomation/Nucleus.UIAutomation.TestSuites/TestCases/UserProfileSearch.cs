using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using Nucleus.Service.Data;
using Nucleus.Service.PageServices.Batch;
using Nucleus.Service.PageServices.Claim;
using Nucleus.Service.PageServices.Login;
using Nucleus.Service.PageServices.Settings.User;
using Nucleus.Service.Support.Common.Constants;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Environment;
using Nucleus.Service.Support.Menu;
using Nucleus.UIAutomation.TestSuites.Base;
using Nucleus.UIAutomation.TestSuites.Common;
using NUnit.Framework;
using UIAutomation.Framework.Utils;
using static System.String;


namespace Nucleus.UIAutomation.TestSuites.TestCases
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class UserProfileSearch 
    {
        #region Private

        //private UserProfileSearchPage _userProfileSearch;
        //private CommonValidations _commonValidations;
        
       // private ProfileManagerPage _profileManager;
       // private LoginPage _login;

        private readonly List<string> _profileLabelList = new List<string>
        {
            "First Name", "Last Name", "Phone", "Phone_ext", "Email Address", "User ID", "User Type", "Status",
            "Password", "Confirm Password"
        };

        private readonly List<string> _profileValueList = new List<string>
        {
            "Test", "Test", "231-456-7890", "01", "cotiviti@cotiviti.com", "uiautomation_Test", "Internal", "Active",
            "TestPassword123$", "TestPassword123$"
        };

        private readonly List<List<string>> _profileDetailInfoList;

        public UserProfileSearch()
        {
            _profileDetailInfoList = new List<List<string>>
            {
                _profileLabelList,_profileValueList
            };
        }

        #endregion

        #region PROTECTED PROPERTIES
        //protected  string GetType().FullName
        //{
        //    get { return GetType().FullName; }
        //}
        #endregion

        #region DBinteraction methods
        private void RetrieveListFromDatabase()
        {

        }
        #endregion

        #region OVERRIDE
        /*
        protected override void ClassInit()
        {
            try
            {
                base.ClassInit();
                CurrentPage = _userProfileSearch = CurrentPage.NavigateToNewUserProfileSearch();
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
            _userProfileSearch.CloseDatabaseConnection();
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
                _userProfileSearch = _userProfileSearch.Logout().LoginAsHciAdminUser().NavigateToNewUserProfileSearch();
            }

            if (_userProfileSearch.GetPageHeader() != Extensions.GetStringValue(PageHeaderEnum.UserProfileSearch))
            {
                _userProfileSearch.ClickOnQuickLaunch().NavigateToNewUserProfileSearch();
            }
            _userProfileSearch.SideBarPanelSearch.OpenSidebarPanel();
            _userProfileSearch.SideBarPanelSearch.ClickOnClearLink();

            if(_userProfileSearch.IsUserSettingsFormPresent())
                CurrentPage.RefreshPage();

        }*/
        #endregion

        #region TestSuites


        [Test] //CAR-2843(CAR-2797)
        public void Verify_validation_clients_assignment_tab()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                UserProfileSearchPage _userProfileSearch;
                automatedBase.CurrentPage =
                    _userProfileSearch = automatedBase.QuickLaunch.NavigateToNewUserProfileSearch();
                var userType = "Internal";
                _userProfileSearch.NavigateToCreateNewUser();
                FillProfileTab(_userProfileSearch);
                _userProfileSearch.CaptureScreenShot("Failed reason for Verify_validation_clients_assignment_tab Test");
                _userProfileSearch.ClickOnNextButton();
                _userProfileSearch.IsCreateNewUserTabSelectedByTabName(NewUserAccountTabEnum.Clients.GetStringValue())
                    .ShouldBeTrue("Tab is correctly highlighted");
                try
                {
                    var expectedClientList = _userProfileSearch.GetCommonSql.GetClientFromDatabaseByClientStatus("T");
                    var random = new Random();
                    for (var j = 0; j < 2; j++)
                    {
                        var expectedAccessList =
                            _userProfileSearch.GetAvailableAccessListByClientFromDb(ClientEnum.SMTST.ToString(),
                                userType == "Internal" ? 2 : 8);
                        if (j == 1)
                        {
                            #region Verification of Client Assignment


                            var randomClient1 = expectedClientList[random.Next(expectedClientList.Count)];
                            var randomClient2 = "";
                            while (true)
                            {
                                randomClient2 = expectedClientList[random.Next(expectedClientList.Count)];
                                if (randomClient1 != randomClient2)
                                    break;
                            }

                            Empty_validation_For_Clients_Tab(_userProfileSearch);

                            _userProfileSearch.GetAvailableAssignedList(2)
                                .ShouldCollectionBeEqual(expectedClientList,
                                    "Is Client List are Equal and is in alphabetical order");
                            _userProfileSearch.ClickOnAvailableAssignedRow(2, randomClient1);

                            _userProfileSearch.GetAvailableAssignedList(2).ShouldNotContain(randomClient1,
                                "Assigned Client should not present in Available Clients?");

                            _userProfileSearch.SetAndGetLengthOfExtUserIdByClientName(randomClient1).ShouldBeEqual(32,
                                "Maximum number of characters that are allowed in the Ext User Id is 32.");


                            _userProfileSearch.ClickOnAvailableAssignedRow(2, randomClient1, false);

                            _userProfileSearch.GetAvailableAssignedList(2, false)
                                .ShouldCollectionBeEmpty("Deselected Clients should not present");
                            _userProfileSearch.ClickOnAvailableAssignedRow(2, randomClient1);

                            _userProfileSearch.ClickOnAvailableAssignedRow(2, randomClient2);
                            var selectedClientList = new List<string> {randomClient1, randomClient2};
                            selectedClientList.Sort();
                            _userProfileSearch.GetAvailableAssignedList(2, false)
                                .ShouldCollectionBeEqual(selectedClientList, "Is Selected Client Equal?");

                            _userProfileSearch.GetSideWindow.GetDropDownList("Default Client", true)
                                .ShouldCollectionBeEqual(selectedClientList,
                                    "Is Default Client List shows assigned Clients");

                            _userProfileSearch.SelectDeselectAll("Available Clients");
                            _userProfileSearch.GetAvailableAssignedList(2, false)
                                .ShouldCollectionBeEqual(expectedClientList, "Is All Client Selected?");

                            _userProfileSearch.GetSideWindow.GetDropDownList("Default Client", true)
                                .ShouldCollectionBeEqual(expectedClientList,
                                    "Is Default Client List shows All assigned Clients");

                            _userProfileSearch.GetAvailableAssignedList(2)
                                .ShouldCollectionBeEmpty("Is Available Client Empty?");
                            _userProfileSearch.SelectDeselectAll("Assigned Clients");
                            _userProfileSearch.ClickOnAvailableAssignedRow(2, ClientEnum.SMTST.ToString());

                            _userProfileSearch.GetSideWindow.SelectDropDownValue("Default Client",
                                ClientEnum.SMTST.ToString());



                            #endregion

                        }

                        #region Verification of Restricted Claim access

                        var access = expectedAccessList[random.Next(expectedAccessList.Count)];


                        _userProfileSearch.GetAvailableAssignedList(3).ShouldCollectionBeEqual(expectedAccessList,
                            "Is Available Access List Equals?");
                        _userProfileSearch.GetAvailableAssignedList(3, false)
                            .ShouldCollectionBeEmpty("Is Assigned Restriction Empty?");

                        _userProfileSearch.ClickOnAvailableAssignedRow(3, access);

                        _userProfileSearch.GetAvailableAssignedList(3, false)
                            .ShouldCollectionBeEqual(new List<string> {access},
                                "Is Assigned Restriction Equals?");
                        _userProfileSearch.GetAvailableAssignedList(3).ShouldNotContain(access,
                            "Assigned Restriction should not present in Available Access?");

                        _userProfileSearch.ClickOnAvailableAssignedRow(3, access, false);
                        _userProfileSearch.GetAvailableAssignedList(3, false)
                            .ShouldCollectionBeEmpty("Is Assigned Restriction Empty?");

                        _userProfileSearch.SelectDeselectAll("Available Access");
                        _userProfileSearch.GetAvailableAssignedList(3, false).ShouldCollectionBeEqual(
                            expectedAccessList,
                            "Is Available Access List Equals?");
                        _userProfileSearch.GetAvailableAssignedList(3)
                            .ShouldCollectionBeEmpty("Is Assigned Restriction Empty?");

                        _userProfileSearch.SelectDeselectAll("Assigned Access");
                        _userProfileSearch.GetAvailableAssignedList(3, false)
                            .ShouldCollectionBeEmpty("Is Assigned Restriction Empty?");

                        _userProfileSearch.ClickOnAvailableAssignedRow(3, access);

                        #endregion

                        if (j == 1)
                        {
                            _userProfileSearch.ClickOnNextButton();
                            _userProfileSearch
                                .IsCreateNewUserTabSelectedByTabName(NewUserAccountTabEnum.RolesNotifications
                                    .GetStringValue())
                                .ShouldBeTrue("Tab is correctly highlighted");
                            _userProfileSearch.ClickOnPreviousButton();
                            _userProfileSearch
                                .IsCreateNewUserTabSelectedByTabName(NewUserAccountTabEnum.Clients.GetStringValue())
                                .ShouldBeTrue("Tab is correctly highlighted");
                            continue;
                        }

                        _userProfileSearch.ClickOnAvailableAssignedRow(3, access, false);
                        userType = userType == "Internal" ? "Client" : "Internal";
                        _userProfileSearch.ClickOnPreviousButton();
                        _userProfileSearch
                            .IsCreateNewUserTabSelectedByTabName(NewUserAccountTabEnum.Profile.GetStringValue())
                            .ShouldBeTrue("Tab is correctly highlighted");
                        GetValueListOfProfileTab(_userProfileSearch)
                            .ShouldCollectionBeEqual(_profileDetailInfoList[1], "Are previous values Equals?");
                        _userProfileSearch.GetSideWindow.SelectDropDownValue("User Type", userType);
                        _userProfileSearch.ClickOnNextButton();
                        _userProfileSearch
                            .IsCreateNewUserTabSelectedByTabName(NewUserAccountTabEnum.Clients.GetStringValue())
                            .ShouldBeTrue("Tab is correctly highlighted");
                    }

                    _userProfileSearch.ClickOnCancelLink();
                    _userProfileSearch.GetSideWindow.IsSideWindowBlockPresent()
                        .ShouldBeFalse("Is New User Account Form open when clicking on cancel link?");
                    _userProfileSearch.NavigateToCreateNewUser();
                    GetValueListOfProfileTab(_userProfileSearch).ShouldCollectionBeAllNullOrEmpty("All Input field should be empty");


                }
                finally
                {
                    _userProfileSearch.SideBarPanelSearch.OpenSidebarPanel();
                }


            }
        }

        private void Empty_validation_For_Clients_Tab(UserProfileSearchPage _userProfileSearch)
        {
            _userProfileSearch.ClickOnNextButton();
            _userProfileSearch.IsInvalidInputPresentOnTransferComponentByLabel("Assigned Clients")
                .ShouldBeTrue("Is Red border Present in Assigned Clients?");
            _userProfileSearch.IsInvalidInputPresentByLabel("Default Client")
                .ShouldBeTrue("Is Read boarder Present in Default Client?");
            _userProfileSearch.IsInputFieldByLabelDisabled("Default Client")
                .ShouldBeTrue("Is Default Client disabled?");
            _userProfileSearch.IsInvalidInputPresentOnTransferComponentByLabel("Assigned Access")
                .ShouldBeFalse("Is Red border Present in Assigned Access?");
        }


        [Test] //CAR-2798 [CAR-2844]
        public void Verification_of_roles_notifications_tab_in_create_user()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                UserProfileSearchPage _userProfileSearch;
                automatedBase.CurrentPage =
                    _userProfileSearch = automatedBase.QuickLaunch.NavigateToNewUserProfileSearch();
                var clientList = new List<string> {ClientEnum.SMTST.ToString()};
                string defaultClient = ClientEnum.SMTST.ToString();
                var accessToBeAssigned = new List<string> {RestrictionGroup.AllUser.GetStringValue()};
                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(GetType().FullName, TestName);
                var defaultPagesForAllRoles = paramLists["DefaultPageForAllRoles"].Split(';').ToList();
                var roleWithManageAppeals = paramLists["RoleWithManageAppeals"];
                var roleWithProviderMaintenanceAndSuspectProvider =
                    paramLists["RoleWithProviderMaintenanceAndSuspectProvider"];
                var roleWithQuickLaunch = paramLists["RoleWithQuickLaunch"];
                var roleWithoutAnyDefaultPageSpecificRole = paramLists["RoleWithoutAnyDefaultPageSpecificRole"];
                var userTypes = new List<string> {"Internal", "Client"};
                var roleAndDefaultPage = new Dictionary<string, string>
                {
                    [roleWithManageAppeals] = PageHeaderEnum.AppealSearch.GetStringValue(),
                    [roleWithProviderMaintenanceAndSuspectProvider] =
                        $"{PageHeaderEnum.ProviderSearch.GetStringValue()};{PageHeaderEnum.SuspectProviders.GetStringValue()}",
                    [roleWithQuickLaunch] = PageHeaderEnum.QuickLaunch.GetStringValue()
                };
                _userProfileSearch.NavigateToCreateNewUser();
                if (_userProfileSearch.SideBarPanelSearch.IsSideBarPanelOpen())
                    _userProfileSearch.SideBarPanelSearch.ClickOnToggleSidebarPanelButton();
                FillProfileTab(_userProfileSearch);
                _userProfileSearch.ClickOnNextButton();
                _userProfileSearch.SetDataInClientsTabToCreateNewUser(clientList, defaultClient, accessToBeAssigned);
                _userProfileSearch.ClickOnNextButton();
                _userProfileSearch
                    .IsCreateNewUserTabSelectedByTabName(NewUserAccountTabEnum.RolesNotifications.GetStringValue())
                    .ShouldBeTrue("Tab is correctly highlighted");
                foreach (var userType in userTypes)
                {
                    var availableRolesInForm = _userProfileSearch.GetAllAvailableRolesList();
                    var assignedRolesInForm = _userProfileSearch.GetAllAssignedRolesList();
                    var availableRolesFromDatabase = _userProfileSearch.GetRoleNamesByUserTypeFromDb(userType);
                    var listOfInfoIconDescription =
                        _userProfileSearch.GetAllInfoIconDescription().Select(x => x.Trim()).ToList();
                    var listOfRoleDescriptionFromDb = _userProfileSearch.GetRoleDesriptionFromDb(userType)
                        .Select(x => x.Trim()).ToList();
                    StringFormatter.PrintMessageTitle("Verifying required red highlight over input fields");
                    _userProfileSearch.ClickOnNextButton();
                    _userProfileSearch
                        .IsInvalidInputPresentOnTransferComponentByLabel(
                            NewUserRolesEnum.AssignedRoles.GetStringValue())
                        .ShouldBeTrue("At least a role needs to be assigned");
                    _userProfileSearch.IsInvalidDropdownInputFieldPresent(NewUserRolesEnum.DefaultPage.GetStringValue())
                        .ShouldBeTrue($"{NewUserRolesEnum.DefaultPage.GetStringValue()} needs to be selected");
                    _userProfileSearch.SelectDeselectAll(NewUserRolesEnum.AvailableRoles.GetStringValue());
                    _userProfileSearch.ClickOnNextButton();
                    _userProfileSearch.IsInvalidDropdownInputFieldPresent(NewUserRolesEnum.DefaultPage.GetStringValue())
                        .ShouldBeTrue($"{NewUserRolesEnum.DefaultPage.GetStringValue()} needs to be selected");
                    _userProfileSearch.SelectDeselectAll(NewUserRolesEnum.AssignedRoles.GetStringValue());

                    #region Verify Available and Assigned To Fields

                    StringFormatter.PrintMessageTitle("Verifying the Available and Assigned roles fields");
                    _userProfileSearch.IsInfoIconPresentNextToAvailableRoles()
                        .ShouldBeTrue("Info Icon should be present next to the available roles");
                    listOfInfoIconDescription.ShouldCollectionBeEquivalent(listOfRoleDescriptionFromDb,
                        "Information icon will be shown to the left of each role, displaying the Role Description text in a tool tip");
                    availableRolesInForm
                        .ShouldCollectionBeSorted(false,
                            $"{NewUserRolesEnum.AvailableRoles.GetStringValue()} list should be sorted alphabetically");
                    assignedRolesInForm
                        .ShouldCollectionBeEmpty(
                            $"{NewUserRolesEnum.AssignedRoles.GetStringValue()} should be empty at the start");
                    availableRolesFromDatabase
                        .ShouldCollectionBeEquivalent(availableRolesInForm,
                            "A list of available roles for the user type will be listed");
                    StringFormatter.PrintMessage("Randomly assigning 5 roles to the user");
                    var randomlyAssignedRoles = _userProfileSearch.RandomlyClickOptionsInAvailableOrAssigned(
                        availableRolesInForm,
                        NewUserRolesEnum.AvailableRoles.GetStringValue(), 5);
                    randomlyAssignedRoles.ShouldCollectionBeEquivalent(_userProfileSearch.GetAllAssignedRolesList(),
                        $"Roles should be correctly displayed in the '{NewUserRolesEnum.AssignedRoles.GetStringValue()}'");
                    _userProfileSearch.GetAllAssignedRolesList().ShouldCollectionBeSorted(false,
                        $"Roles should be in alphabetic order in '{NewUserRolesEnum.AvailableRoles.GetStringValue()}'");
                    StringFormatter.PrintMessage(
                        $"Verifying deselecting the roles from '{NewUserRolesEnum.AssignedRoles.GetStringValue()}'");
                    _userProfileSearch.RandomlyClickOptionsInAvailableOrAssigned(randomlyAssignedRoles,
                        NewUserRolesEnum.AssignedRoles.GetStringValue(), 5);
                    _userProfileSearch.GetAllAssignedRolesList()
                        .ShouldCollectionBeEmpty("All the assigned roles can be deselected by clicking on them");
                    StringFormatter.PrintMessage(
                        "Verifying the 'Select All' and 'Deselect All' buttons in the roles selection component");
                    _userProfileSearch.SelectDeselectAll(NewUserRolesEnum.AvailableRoles.GetStringValue());
                    _userProfileSearch.GetAllAvailableRolesList().ShouldCollectionBeEmpty(
                        $"'{NewUserRolesEnum.AvailableRoles.GetStringValue()}' should be empty once all roles are selected to be assigned");
                    _userProfileSearch.SelectDeselectAll(NewUserRolesEnum.AssignedRoles.GetStringValue());
                    _userProfileSearch.GetAllAssignedRolesList().ShouldCollectionBeEmpty(
                        $"'{NewUserRolesEnum.AssignedRoles.GetStringValue()}' should be empty once all roles are deselected");
                    _userProfileSearch.GetAllAvailableRolesList().ShouldCollectionBeEqual(availableRolesInForm,
                        "All roles should be restored to" +
                        $"{NewUserRolesEnum.AvailableRoles.GetStringValue()} when Deselect All is clicked");

                    #endregion

                    #region Verify Default Page Field

                    StringFormatter.PrintMessage(
                        $"Verifying the  {NewUserRolesEnum.DefaultPage.GetStringValue()} field");
                    _userProfileSearch.IsInputFieldByLabelDisabled(NewUserRolesEnum.DefaultPage.GetStringValue())
                        .ShouldBeTrue(
                            $"{NewUserRolesEnum.DefaultPage.GetStringValue()} should be disabled when no roles are assigned");
                    _userProfileSearch.ClickOnAvailableAssignedRow(1, roleWithoutAnyDefaultPageSpecificRole);
                    _userProfileSearch.IsInputFieldByLabelDisabled(NewUserRolesEnum.DefaultPage.GetStringValue())
                        .ShouldBeFalse(
                            $"{NewUserRolesEnum.DefaultPage.GetStringValue()} should be enabled once a role is assigned");
                    _userProfileSearch.GetSideWindow.GetDropDownList(NewUserRolesEnum
                            .DefaultPage.GetStringValue()).Where(x => x != "").ToList()
                        .ShouldCollectionBeEqual(defaultPagesForAllRoles,
                            $"'{NewUserRolesEnum.DefaultPage.GetStringValue()}' shows " +
                            $"{defaultPagesForAllRoles[0]} and {defaultPagesForAllRoles[1]} as default page for any non-default page specific role");
                    StringFormatter.PrintMessage(
                        $"Verifying contents of {NewUserRolesEnum.DefaultPage.GetStringValue()} dropdown");
                    foreach (var role in roleAndDefaultPage.Keys)
                    {
                        _userProfileSearch.ClickOnAvailableAssignedRow(1, role);
                        if (role == roleWithProviderMaintenanceAndSuspectProvider)
                        {
                            _userProfileSearch.GetSideWindow.GetDropDownList(NewUserRolesEnum
                                .DefaultPage.GetStringValue()).ShouldContain(roleAndDefaultPage[role].Split(';')[0],
                                $"The {roleAndDefaultPage[role].Split(';')[0]} page should be added to the " +
                                $"{NewUserRolesEnum.DefaultPage.GetStringValue()} list");
                            _userProfileSearch.GetSideWindow.GetDropDownList(NewUserRolesEnum
                                .DefaultPage.GetStringValue()).ShouldContain(roleAndDefaultPage[role].Split(';')[1],
                                $"The {roleAndDefaultPage[role].Split(';')[1]} page should be added to the " +
                                $"{NewUserRolesEnum.DefaultPage.GetStringValue()} list");
                            continue;
                        }

                        _userProfileSearch.GetSideWindow.GetDropDownList(NewUserRolesEnum
                            .DefaultPage.GetStringValue()).ShouldContain(roleAndDefaultPage[role],
                            $"The {roleAndDefaultPage[role]} page should be added to the {NewUserRolesEnum.DefaultPage.GetStringValue()} list");
                    }

                    _userProfileSearch.GetSideWindow.GetDropDownList(NewUserRolesEnum.DefaultPage.GetStringValue())
                        .ShouldCollectionBeSorted(false,
                            $"{NewUserRolesEnum.DefaultPage.GetStringValue()} is sorted alphabetically");
                    _userProfileSearch.SelectDropDownListValueByLabel(NewUserRolesEnum.DefaultPage.GetStringValue(),
                        roleAndDefaultPage[roleWithManageAppeals]);

                    #endregion

                    #region Verify Notifications Field

                    StringFormatter.PrintMessageTitle("Verification of the 'Notifications' options");
                    foreach (NewUserNotificationsEnum notificationOption in Enum.GetValues(
                        typeof(NewUserNotificationsEnum)))
                    {
                        var option = notificationOption.GetStringValue();
                        if (option == "Batch complete/return file process cannot be initiated" &&
                            userType == userTypes[1])
                            continue;
                        _userProfileSearch.IsRadioButtonPresentByLabel(option)
                            .ShouldBeTrue($"Radio button is available for {option}");
                        _userProfileSearch.IsRadioButtonOnOffByLabel(option, false).ShouldBeTrue(
                            $"Radio button for '{option}' option" +
                            "should default to 'No' ");
                        _userProfileSearch.ClickOnRadioButtonByLabel(option);
                    }

                    #endregion

                    #region Verify Switching Between Tabs

                    StringFormatter.PrintMessageTitle(
                        "Verification of navigating between other tabs using Previous and Next buttons");
                    var originallyAssignedRoles = _userProfileSearch.GetAllAssignedRolesList();
                    var originallyAvailableRoles = _userProfileSearch.GetAllAvailableRolesList();
                    var originalDefaultPage =
                        _userProfileSearch.GetSideWindow.GetDropDownInputFieldByLabel(NewUserRolesEnum.DefaultPage
                            .GetStringValue());
                    StringFormatter.PrintMessage("Clicking on 'Next' button");
                    _userProfileSearch.ClickOnNextButton();
                    _userProfileSearch.IsFormHeaderPresentByHeaderName("Summary")
                        .ShouldBeTrue("Clicking on 'Next' should direct to Profile Summary");
                    _userProfileSearch.ClickOnPreviousButton();
                    _userProfileSearch.IsFormHeaderPresentByHeaderName("Roles").ShouldBeTrue(
                        "Clicking on 'Previous' from Summary tab " +
                        "should direct to Roles tab");
                    StringFormatter.PrintMessageTitle(
                        "Verifying whether data remains unchanged when navigating between tabs");
                    while (!_userProfileSearch.IsFormHeaderPresentByHeaderName("Profile"))
                        _userProfileSearch.ClickOnPreviousButton();
                    while (!_userProfileSearch.IsFormHeaderPresentByHeaderName("Roles"))
                        _userProfileSearch.ClickOnNextButton();
                    VerifyDataIsUnchanged(originallyAvailableRoles, originallyAssignedRoles, originalDefaultPage,
                        userType);

                    #endregion

                    // Click Cancel and perform validations for 'Client' user type
                    if (userType == userTypes[0])
                    {
                        StringFormatter.PrintMessage("Verifying clicking on Cancel button");
                        _userProfileSearch.ClickOnCancelLink();
                        _userProfileSearch.IsNewUserAccountFormPresent()
                            .ShouldBeFalse("Clicking Cancel should close the add new user form");
                        _userProfileSearch.NavigateToCreateNewUser();
                        if (_userProfileSearch.SideBarPanelSearch.IsSideBarPanelOpen())
                            _userProfileSearch.SideBarPanelSearch.ClickOnToggleSidebarPanelButton();
                        StringFormatter.PrintMessage("Changing the user type to Client");
                        FillProfileTab(_userProfileSearch);
                        _userProfileSearch.SelectDropDownListValueByLabel(UserInformationEnum.UserType.GetStringValue(),
                            userTypes[1]);
                        _userProfileSearch.ClickOnNextButton();
                        _userProfileSearch.SetDataInClientsTabToCreateNewUser(clientList, defaultClient,
                            accessToBeAssigned);
                        _userProfileSearch.ClickOnNextButton();
                        _userProfileSearch
                            .IsCreateNewUserTabSelectedByTabName(
                                NewUserAccountTabEnum.RolesNotifications.GetStringValue())
                            .ShouldBeTrue("Tab is correctly highlighted");
                    }
                }

                #region Local Methods

                void VerifyDataIsUnchanged(IList<string> originallyAvailableRoles,
                    IList<string> originallyAssignedRoles, string originalDefaultPage,
                    string userType)
                {
                    _userProfileSearch.GetAllAvailableRolesList().ShouldCollectionBeEqual(originallyAvailableRoles,
                        "Available Roles should remain unchanged");
                    _userProfileSearch.GetAllAssignedRolesList().ShouldCollectionBeEqual(originallyAssignedRoles,
                        "Assigned Roles should remain unchanged");
                    _userProfileSearch.GetSideWindow
                        .GetDropDownInputFieldByLabel(NewUserRolesEnum.DefaultPage.GetStringValue())
                        .ShouldBeEqual(originalDefaultPage,
                            $"{NewUserRolesEnum.DefaultPage.GetStringValue()} should remain unchanged");
                    foreach (NewUserNotificationsEnum notificationOption in Enum.GetValues(
                        typeof(NewUserNotificationsEnum)))
                    {
                        var option = notificationOption.GetStringValue();
                        if (option == "Batch complete/return file process cannot be initiated" &&
                            userType == userTypes[1])
                            continue;
                        _userProfileSearch.IsRadioButtonOnOffByLabel(option)
                            .ShouldBeTrue($"Radio button value for '{option}' should be unchanged ");
                    }
                }

                #endregion

            }
        }

        [Test,Category("OnDemand")] //CAR-2842(CAR-2152) +CAR-3012(CAR-2996) + TE-973
        [NonParallelizable]
        public void Create_new_nucleus_user_account()
        {

            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {

                UserProfileSearchPage _userProfileSearch;
                automatedBase.CurrentPage =
                    _userProfileSearch = automatedBase.QuickLaunch.NavigateToNewUserProfileSearch();
                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(GetType().FullName, TestName);
                var tabLabelsList = automatedBase.DataHelper.GetMappingData(GetType().FullName, "New_User_Account_Tab_Title")
                    .Values
                    .ToList();
                var profileHeaders = automatedBase.DataHelper.GetMappingData(GetType().FullName, "ProfileHeaders").Values
                    .ToList();
                var expectedUserTypes = paramLists["UserTypes"].Split(',').ToList();
                var expectedStatus = paramLists["Status"].Split(',').ToList();
                ;
                string passwordTooltip = paramLists["PasswordToolTip"];
                string userId = paramLists["UserId"];
                string email = paramLists["Email"];
                var invalidPassswords = paramLists["InvalidPasswords"].Split(',').ToList();
                var newProfileLabelList = new List<string>(_profileLabelList);
                _userProfileSearch.NavigateToCreateNewUser();

                try
                {
                    #region Specification Verification

                    StringFormatter.PrintMessage("Specification Verification");
                    _userProfileSearch.IsNewUserAccountFormPresent().ShouldBeTrue("Is New User Account form present?");
                    _userProfileSearch.IsCreateNewUserTabSelectedByTabName(NewUserAccountTabEnum.Profile.ToString())
                        .ShouldBeTrue("Is Profile tab selected?");
                    _userProfileSearch.IsLabelPresent(NewUserAccountTabEnum.Profile.ToString())
                        .ShouldBeTrue("Is Profile label shown on the selecting Profile tab?");
                    _userProfileSearch.GetNewUserAccountTabsLabel()
                        .ShouldCollectionBeEqual(tabLabelsList, $"{tabLabelsList} tabs should be present");
                    _userProfileSearch.GetProfileTabHeaders()
                        .ShouldCollectionBeEquivalent(profileHeaders, $"{profileHeaders} headers should be present");
                    newProfileLabelList.Remove("Phone_ext");

                    StringFormatter.PrintMessage("Verification of presence of input fields");
                    foreach (var label in newProfileLabelList)
                    {
                        _userProfileSearch.GetSideWindow
                            .IsInputFieldPresentByLabel(label)
                            .ShouldBeTrue($"{label} input field present?");
                    }

                    _userProfileSearch.IsExtNoInputFieldPresent().ShouldBeTrue("Is Ext input present?");

                    StringFormatter.PrintMessage("Verification of dropdown values");
                    var userTypeDropDownValues =
                        _userProfileSearch.GetSideWindow.GetDropDownList(_profileLabelList[6], true);

                    userTypeDropDownValues.ShouldCollectionBeEqual(expectedUserTypes,
                        $"Are {userTypeDropDownValues[0]}, {userTypeDropDownValues[1]} user types present?");
                    var statusDropDownList =
                        _userProfileSearch.GetSideWindow.GetDropDownList(_profileLabelList[7], true);
                    statusDropDownList.ShouldCollectionBeEqual(expectedStatus,
                        $"Are {expectedStatus[0]}, {expectedStatus[1]} status present?");

                    StringFormatter.PrintMessage("Verification of display value and tooltip message");
                    _userProfileSearch.GetExtAttribute("placeholder")
                        .ShouldBeEqual("ext", "Display value should be ext");
                    _userProfileSearch.GetInfoHelpTooltipByLabel(newProfileLabelList[7])
                        .ShouldBeEqual(passwordTooltip, "Tooltip should match");

                    StringFormatter.PrintMessage("Verification of presence of next and cancel button");
                    _userProfileSearch.IsNewUserAccountNextButtonPresent().ShouldBeTrue("Is next button present?");
                    _userProfileSearch.IsCancelButtonPresent().ShouldBeTrue("Is cancel button present?");

                    #endregion

                    StringFormatter.PrintMessage("Empty fields validation");
                    _userProfileSearch.ClickOnNextButton();
                    newProfileLabelList.Remove(newProfileLabelList.Last());
                    foreach (var label in newProfileLabelList)
                    {
                        _userProfileSearch.IsInvalidInputPresentByLabel(label)
                            .ShouldBeTrue($"Is invalid input present for {label}?");
                    }

                    _userProfileSearch.GetExtAttribute("class").ShouldNotContain("invalid",
                        "Ext is not required field, so invalid input should not be present in ext");

                    #region PhoneNumberValidation

                    StringFormatter.PrintMessage("Verification that if a number starts with '1', it gets omitted");
                    _userProfileSearch.GetSideWindow.SetInputInInputFieldByLabel(_profileLabelList[2], "1234567890",
                        true);
                    _userProfileSearch.GetSideWindow.GetInputValueByLabel(_profileLabelList[2])
                        .ShouldBeEqual("234-567-890", "Initital 1 automatically gets trimmed");

                    StringFormatter.PrintMessage("Verification of max length of ext");
                    _userProfileSearch.SetExtNo("12345678901");
                    _userProfileSearch.GetExtNo()
                        .ShouldBeEqual("1234567890", "Maximum of 10 digits are allowed in ext");

                    StringFormatter.PrintMessage("Verification that phone number should be in XXX-XXX-XXXX format");
                    FillProfileTab(_userProfileSearch);
                    _userProfileSearch.GetSideWindow.SetInputInInputFieldByLabel(_profileLabelList[2], "23456789");
                    _userProfileSearch.ClickOnNextButton();
                    _userProfileSearch.IsInvalidInputPresentByLabel(_profileLabelList[2])
                        .ShouldBeTrue("Invalid input should be present for incorrect phone number format");
                    _userProfileSearch.GetSideWindow.SetInputInInputFieldByLabel(_profileLabelList[2],
                        _profileValueList[2], true);
                    _userProfileSearch.IsInvalidInputPresentByLabel(_profileLabelList[2])
                        .ShouldBeFalse("Is Invalid input present?");
                    _userProfileSearch.GetSideWindow.GetInputValueByLabel(_profileLabelList[2])
                        .ShouldBeEqual(_profileValueList[2], "Phone number should be in XXX-XXX-XXXX format");

                    #endregion

                    StringFormatter.PrintMessage("Email format validation");
                    _userProfileSearch.GetSideWindow.SetInputInInputFieldByLabel(_profileLabelList[4], "cotiviti");
                    _userProfileSearch.ClickOnNextButton();
                    _userProfileSearch.IsInvalidInputPresentByLabel(_profileLabelList[4])
                        .ShouldBeTrue("Invalid input should be present for incorrect email format");

                    #region CAR-3012(CAR-2996)

                    StringFormatter.PrintMessage(
                        "Verify pop up error message when existing email is already in user for same usertype");
                    bool doesuserWithEmailExists = _userProfileSearch.DoesUserAlreadyExistWithSameEmail(email);
                    _userProfileSearch.GetSideWindow.SetInputInInputFieldByLabel(_profileLabelList[4], email);
                    _userProfileSearch.ClickOnNextButton();
                    _userProfileSearch.WaitForWorking();
                    if (doesuserWithEmailExists)
                    {
                        _userProfileSearch.IsPageErrorPopupModalPresent()
                            .ShouldBeTrue("Invalid input should be present for incorrect email format");
                        _userProfileSearch.GetPageErrorMessage().ShouldBeEqual(
                            "An account already exists for a user with the same email address. Please verify the email address.",
                            "Error message correct?");
                        _userProfileSearch.ClosePageError();
                    }

                    _userProfileSearch.GetSideWindow.SetInputInInputFieldByLabel(_profileLabelList[4],
                        "cotiviti@cotiviti");
                    _userProfileSearch.IsInvalidInputPresentByLabel(_profileLabelList[4])
                        .ShouldBeTrue("Invalid input should be present for incorrect email format");
                    _userProfileSearch.GetSideWindow.SetInputInInputFieldByLabel(_profileLabelList[4],
                        _profileValueList[4]);
                    _userProfileSearch.IsInvalidInputPresentByLabel(_profileLabelList[4])
                        .ShouldBeFalse("Invalid input should be present for incorrect email format");

                    #endregion

                    #region Password Validation

                    StringFormatter.PrintMessage(
                        "Validation for password of length less than 8 and password and confirm password values mismatch");
                    _userProfileSearch.GetSideWindow.SetInputInInputFieldByLabel(_profileLabelList[8],
                        invalidPassswords[0]);
                    _userProfileSearch.IsInvalidInputPresentByLabel(_profileLabelList[8])
                        .ShouldBeTrue("Invalid input should be present for incorrect password format");
                    _userProfileSearch.IsInvalidInputPresentByLabel(_profileLabelList[9])
                        .ShouldBeTrue("Invalid input should be present in confirm password for password mismatch");

                    PasswordValidation(
                        "Validation for password meeting the length requirement without one special character",
                        invalidPassswords[1]);
                    PasswordValidation(
                        "Validation for password meeting the length and one special character requirement without one numeric character",
                        invalidPassswords[2]);
                    PasswordValidation(
                        "Validation for password meeting the length, special character and numeric number requirement without one uppercase value",
                        invalidPassswords[3]);
                    PasswordValidation(
                        "Validation for password meeting the length, special character, numeric number and uppercase requirement without one lowercase value",
                        invalidPassswords[4]);

                    StringFormatter.PrintMessage(
                        "Password Validation meeting all the requirements and password and confirm password values should match");
                    _userProfileSearch.GetSideWindow.SetInputInInputFieldByLabel(_profileLabelList[8],
                        _profileValueList[8]);
                    _userProfileSearch.IsInvalidInputPresentByLabel(_profileLabelList[8])
                        .ShouldBeFalse("Is invalid input validation present?");
                    _userProfileSearch.IsInvalidInputPresentByLabel(_profileLabelList[9])
                        .ShouldBeFalse("Is invalid input validation present?");

                    #endregion

                    StringFormatter.PrintMessage("Verification of Next button with new userId");
                    _userProfileSearch.ClickOnNextButton();
                    _userProfileSearch.IsLabelPresent(NewUserAccountTabEnum.Clients.ToString())
                        .ShouldBeTrue("User should be navigated to Clients tab");
                    _userProfileSearch.ClickOnPreviousButton();

                    StringFormatter.PrintMessage("Verification of Next button with existing userId");
                    _userProfileSearch.GetSideWindow.SetInputInInputFieldByLabel(_profileLabelList[5], userId);
                    _userProfileSearch.ClickOnNextButton();
                    _userProfileSearch.GetPageErrorMessage().ShouldBeEqual("This User ID is already in use.",
                        "Error pop up should be present");
                    _userProfileSearch.ClosePageError();

                    StringFormatter.PrintMessage("Verification of Cancel link");
                    _userProfileSearch.ClickOnCancelLink();
                    _userProfileSearch.IsNewUserAccountFormPresent().ShouldBeFalse("Is New user account form present?");

                    StringFormatter.PrintMessage(
                        "Verification that on clicking cancel link the previously set values are not set");
                    _userProfileSearch.NavigateToCreateNewUser();
                    _userProfileSearch.SideBarPanelSearch.ClickOnSideBarPanelIcon();
                    newProfileLabelList.Add(_profileLabelList.Last());
                    foreach (var label in newProfileLabelList)
                    {
                        _userProfileSearch.GetSideWindow.GetInputValueByLabel(label)
                            .ShouldBeNullorEmpty(
                                $"Previously set {label} values should not be present after clicking cancel link");
                    }

                    _userProfileSearch.GetExtNo()
                        .ShouldBeNullorEmpty(
                            "Previously set Ext number should not be present after clicking cancel link");
                }
                finally
                {
                    _userProfileSearch.SideBarPanelSearch.OpenSidebarPanel();
                }

                void PasswordValidation(string message, string value)
                {
                    StringFormatter.PrintMessage(message);
                    _userProfileSearch.GetSideWindow.SetInputInInputFieldByLabel(_profileLabelList[8], value);
                    _userProfileSearch.IsInvalidInputPresentByLabel(_profileLabelList[8])
                        .ShouldBeTrue("Invalid input should be present for incorrect password format");
                }
            }
        }
        [NonParallelizable]
        [Test, Category("OnDemand")] //CAR-2741 [CAR-2845]
        public void Verify_Profile_Summary_and_Creation()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                UserProfileSearchPage _userProfileSearch;
                automatedBase.CurrentPage =
                    _userProfileSearch = automatedBase.QuickLaunch.NavigateToNewUserProfileSearch();
                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(GetType().FullName, TestName);
                var summaryHeaderDescription = paramLists["SummaryHeaderDescription"];
                var userCreationMessage = paramLists["UserCreationMessage"];
                var defaultPage = paramLists["DefaultPage"];
                var summaryLabelList = paramLists["SummaryLabelList"].Split(';').ToList();
                var summaryvalueList = paramLists["SummaryvalueList"].Split(';').ToList();
                var clientList = paramLists["ClientList"].Split(';').ToList();
                var roleList = paramLists["RoleList"].Split(';').ToList();
                var restrictionList = paramLists["RestrictionList"].Split(';').ToList();
                var notificationsList = paramLists["NotificationsList"].Split(';').ToList();
                var notificationsValueList = paramLists["NotificationsValueList"].Split(';').ToList();
                _userProfileSearch.DeleteUserDetailsFromDb(summaryvalueList[1]);
                _userProfileSearch.GetUserDetailsFromDb(summaryvalueList[1])
                    .ShouldBeEqual(0, "Users should not be available in the database.");
                roleList.Sort();
                try
                {
                    StringFormatter.PrintMessageTitle(
                        "Verification whether all the fields are displaying correctly in the Profile Summary form");
                    Fill_User_Profile_Clients_Roles(_userProfileSearch,clientList, roleList, restrictionList, defaultPage,
                        notificationsList, notificationsValueList);
                    _userProfileSearch.GetHeaderDescription().ShouldBeEqual(summaryHeaderDescription,
                        "Header Description for summary should contain following message.");
                    _userProfileSearch.IsPreviousButtonPresent()
                        .ShouldBeTrue("Previous Button Should be present in the bottom left of form.");
                    _userProfileSearch.IsCancelButtonPresent()
                        .ShouldBeTrue("Cancel Button should be present right to the previous button.");
                    _userProfileSearch.IsCreateAccountPresent()
                        .ShouldBeTrue("Create User should be present in the bottom right of the form.");

                    var i = 0;
                    foreach (var label in summaryLabelList)
                    {
                        _userProfileSearch.IsSummaryLabelPresent(label)
                            .ShouldBeTrue($"{label} label should be present.");
                        _userProfileSearch.GetSummaryLabelValue(label).ShouldBeEqual(summaryvalueList[i],
                            $"{label} should contain the {summaryvalueList[i]}");
                        i++;
                    }

                    _userProfileSearch.ClickOnPreviousButton();

                    StringFormatter.PrintMessageTitle(
                        "Verification whether data is being retained when navigating between tabs");
                    Verify_Roles_And_Notifications_Values_Are_Saved(_userProfileSearch,roleList, defaultPage, notificationsList,
                        notificationsValueList);
                    _userProfileSearch.ClickOnNextButton();
                    _userProfileSearch.ClickOnCancelLink();

                    StringFormatter.PrintMessageTitle("Verifying creating a new user");
                    Fill_User_Profile_Clients_Roles(_userProfileSearch,clientList, roleList, restrictionList, defaultPage,
                        notificationsList, notificationsValueList);
                    _userProfileSearch.ClickOnCreateUserButton();

                    StringFormatter.PrintMessage("Verifying whether newly created user is added to the database");
                    _userProfileSearch.GetUserDetailsFromDb(summaryvalueList[1])
                        .ShouldBeEqual(1, "Users should be added to the database.");
                    _userProfileSearch.IsPageErrorPopupModalPresent()
                        .ShouldBeTrue("A Confirmation message should be shown.");
                    _userProfileSearch.GetPageErrorMessage().ShouldBeEqual(userCreationMessage,
                        "Message should contain the information that a new user has been created.");
                    _userProfileSearch.ClickOkCancelOnConfirmationModal(true);
                    _userProfileSearch.IsPageErrorPopupModalPresent().ShouldBeFalse("Message popup should be closed.");
                    _userProfileSearch.IsProfileHeaderPresent().ShouldBeTrue("Create New User form should be shown.");
                    _userProfileSearch.DeleteUserDetailsFromDb(summaryvalueList[1]);

                    StringFormatter.PrintMessage("Verifying clicking cancel in the confirmation popup");
                    Fill_User_Profile_Clients_Roles(_userProfileSearch,clientList, roleList, restrictionList, defaultPage,
                        notificationsList, notificationsValueList);
                    _userProfileSearch.ClickOnCreateUserButton();
                    _userProfileSearch.ClickOkCancelOnConfirmationModal(false);
                    _userProfileSearch.IsPageErrorPopupModalPresent().ShouldBeFalse("Message popup should be closed.");
                    _userProfileSearch.IsNewUserAccountFormPresent().ShouldBeFalse(
                        "Create New User Form should not be present after cancel is clicked" +
                        "in the confirmation message");

                    StringFormatter.PrintMessageTitle("Searching for the newly created user");
                    if (!_userProfileSearch.SideBarPanelSearch.IsSideBarPanelOpen())
                    {
                        _userProfileSearch.SideBarPanelSearch.ClickOnSideBarPanelIcon();
                    }

                    _userProfileSearch.SideBarPanelSearch.SetInputFieldByLabel("User ID", summaryvalueList[1]);
                    _userProfileSearch.SideBarPanelSearch.ClickOnFindButton();
                    _userProfileSearch.WaitForWorkingAjaxMessageForBothDisplayAndHide();
                    _userProfileSearch.GetGridViewSection.GetValueInGridByColRow(3)
                        .ShouldBeEqual(summaryvalueList[1], "Search");
                }
                finally
                {
                    StringFormatter.PrintMessageTitle("Finally Block. Deleting the created user from the database.");
                    _userProfileSearch.DeleteUserDetailsFromDb(summaryvalueList[1]);
                }
            }
        }
        [NonParallelizable]
        [Test, Category("OnDemand")] //TE-993 +TE-1011
        public void Verify_SSO_User_Creation()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                UserProfileSearchPage _userProfileSearch;
                automatedBase.CurrentPage =
                    _userProfileSearch = automatedBase.QuickLaunch.NavigateToNewUserProfileSearch();
                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(GetType().FullName, TestName);
                var summaryHeaderDescription = paramLists["SummaryHeaderDescription"];
                var userCreationMessage = paramLists["UserCreationMessage"];
                var defaultPage = paramLists["DefaultPage"];
                var summaryLabelList = paramLists["SummaryLabelList"].Split(';').ToList();
                var summaryvalueList = paramLists["SummaryvalueList"].Split(';').ToList();
                var clientList = paramLists["ClientList"].Split(';').ToList();
                var roleList = paramLists["RoleList"].Split(';').ToList();
                var restrictionList = paramLists["RestrictionList"].Split(';').ToList();
                var notificationsList = paramLists["NotificationsList"].Split(';').ToList();
                var notificationsValueList = paramLists["NotificationsValueList"].Split(';').ToList();
                _userProfileSearch.DeleteUserDetailsFromDb(summaryvalueList[1]);
                _userProfileSearch.GetUserDetailsFromDb(summaryvalueList[1])
                    .ShouldBeEqual(0, "Users should not be available in the database.");
                roleList.Sort();
                try
                {
                    for (int i = 0; i < 2; i++)
                    {
                        _userProfileSearch.NavigateToCreateNewUser();
                        _userProfileSearch.RefreshPage();
                        if (_userProfileSearch.SideBarPanelSearch.IsSideBarPanelOpen())
                        {
                            _userProfileSearch.SideBarPanelSearch.ClickOnSideBarPanelIcon();
                        }
                        StringFormatter.PrintMessage("Verification of user id field");
                        _userProfileSearch.GetSideWindow.SetInputInInputFieldByLabel("User ID", string.Concat(Enumerable.Repeat("a1@_A", 21)));
                        _userProfileSearch.GetSideWindow.GetInputFieldText("User ID").Length.ShouldBeEqual(100, string.Format("Verification of Maximum length of {0} Filter"));
                       
                        StringFormatter.PrintMessage("Verification of SSO User Radio Button Options");
                        _userProfileSearch.GetSideWindow
                            .IsInputFieldPresentByLabel("SSO User")
                            .ShouldBeTrue("SSO User input field present?");
                        _userProfileSearch
                            .IsRadioButtonPresentByLabel("SSO User")
                            .ShouldBeTrue("SSO User radio button should display");
                        _userProfileSearch.IsRadioButtonOnOffByLabel("SSO User")
                            .ShouldBeFalse("By Default SSO User should be set to no");
                        _userProfileSearch.GetSideWindow
                            .IsInputFieldPresentByLabel("Is Federated")
                            .ShouldBeTrue("Is Federated input field present?");

                        _userProfileSearch
                            .IsRadioButtonPresentByLabel("Is Federated")
                            .ShouldBeTrue("Is Federeated radio button should display");
                        _userProfileSearch.IsRadioButtonOnOffByLabel("Is Federated")
                            .ShouldBeFalse("By Default Is Federated should be set to no");
                        _userProfileSearch.IsRadioButtonDisabled("Is Federated").ShouldBeTrue("true?");

                        StringFormatter.PrintMessage("Verify Password Field Disabled when SSO User is selected");
                        _userProfileSearch.ClickOnRadioButtonByLabel("SSO User");
                        _userProfileSearch.IsRadioButtonDisabled("Is Federated")
                            .ShouldBeFalse("Is federated radio button enabled?");


                        if (i == 0)
                            _userProfileSearch.ClickOnRadioButtonByLabel("Is Federated");

                        _userProfileSearch.IsInputFieldByLabelDisabled("Password")
                            .ShouldBeTrue("Password Field Should Be Disabled");
                        _userProfileSearch.IsInputFieldByLabelDisabled("Confirm Password")
                            .ShouldBeTrue("Confirm Password Field Should Be Disabled");
                        _userProfileSearch.GetInfoHelpTooltipByLabel("Password").ShouldBeEqual(
                            "The password for this user will be handled through the organization Identity Provider",
                            "Tootltip value should match.");


                        StringFormatter.PrintMessageTitle("Verifying creating a new user");
                        Fill_User_Profile_Clients_Roles(_userProfileSearch,clientList, roleList, restrictionList, defaultPage,
                            notificationsList, notificationsValueList, true);
                        var j = 0;
                        if (i == 1)
                            summaryvalueList[7] = "NO";
                        foreach (var label in summaryLabelList)
                        {
                            _userProfileSearch.GetSummaryLabelValue(label).ShouldBeEqual(summaryvalueList[j],
                                $"{label} should contain the {summaryvalueList[j]}");
                            j++;
                        }

                        _userProfileSearch.ClickOnCreateUserButton();


                        StringFormatter.PrintMessage("Verifying whether newly created user is added to the database");
                        _userProfileSearch.GetUserDetailsFromDb(summaryvalueList[1])
                            .ShouldBeEqual(1, "Users should be added to the database.");
                        _userProfileSearch.IsPageErrorPopupModalPresent()
                            .ShouldBeTrue("A Confirmation message should be shown.");
                        _userProfileSearch.GetPageErrorMessage().ShouldBeEqual(userCreationMessage,
                            "Message should contain the information that a new user has been created.");
                        _userProfileSearch.ClickOkCancelOnConfirmationModal(true);
                        _userProfileSearch.IsPageErrorPopupModalPresent()
                            .ShouldBeFalse("Message popup should be closed.");
                        _userProfileSearch.IsProfileHeaderPresent()
                            .ShouldBeTrue("Create New User form should be shown.");

                        StringFormatter.PrintMessage("Verify Uses_SSO flag is set to true in database");
                        _userProfileSearch.IsSSOFlagSetToTrueInDatabase(summaryvalueList[1])
                            .ShouldBeTrue("SSO Flag should be set to true in database");
                        if (i == 0)
                            _userProfileSearch.IsFederatedUserSetToTrueInDatabase(summaryvalueList[1])
                                .ShouldBeTrue("Is Federated Flag should be set to true in database");
                        else
                            _userProfileSearch.IsFederatedUserSetToTrueInDatabase(summaryvalueList[1])
                                .ShouldBeFalse("Is Federated Flag should be set to false in database");

                        StringFormatter.PrintMessageTitle("Searching for the newly created user");
                        if (!_userProfileSearch.SideBarPanelSearch.IsSideBarPanelOpen())
                        {
                            _userProfileSearch.SideBarPanelSearch.ClickOnSideBarPanelIcon();
                        }

                        _userProfileSearch.SideBarPanelSearch.SetInputFieldByLabel("User ID", summaryvalueList[1]);
                        _userProfileSearch.SideBarPanelSearch.ClickOnFindButton();
                        _userProfileSearch.WaitForWorkingAjaxMessageForBothDisplayAndHide();
                        _userProfileSearch.GetGridViewSection.GetValueInGridByColRow(3)
                            .ShouldBeEqual(summaryvalueList[1], "Search");
                        _userProfileSearch.GetGridViewSection.ClickOnGridRowByRow(1);

                        StringFormatter.PrintMessage("Verify Updation of SSO and Is Federated in user settings");
                        _userProfileSearch.ClickOnEditIcon();
                        _userProfileSearch
                            .IsRadioButtonPresentByLabel("Is Federated")
                            .ShouldBeTrue("Is Federated radio button should display");
                        _userProfileSearch
                            .IsRadioButtonPresentByLabel("SSO User")
                            .ShouldBeTrue("SSO User radio button should display");

                        _userProfileSearch.IsRadioButtonOnOffByLabel("SSO User")
                            .ShouldBeTrue("SSO User should be set to yes");

                        if (i == 1)
                            _userProfileSearch.IsRadioButtonOnOffByLabel("Is Federated")
                                .ShouldBeFalse("Is Federated should be set to no");
                        else
                            _userProfileSearch.IsRadioButtonOnOffByLabel("Is Federated")
                                .ShouldBeTrue("Is Federated should be set to yes");


                        _userProfileSearch.SetInputTextBoxValueByLabel(UserProfileEnum.JobTitle.GetStringValue(),
                            "Test");
                        if (_userProfileSearch.IsRadioButtonOnOffByLabel("Is Federated"))
                        {
                            _userProfileSearch.ClickOnRadioButtonByLabel("Is Federated", false);
                            _userProfileSearch.GetSideWindow.Save();
                            _userProfileSearch.IsFederatedUserSetToTrueInDatabase(summaryvalueList[1])
                                .ShouldBeFalse("Is Federated Flag should be set to false in database");
                        }
                        else
                        {
                            _userProfileSearch.ClickOnRadioButtonByLabel("Is Federated");
                            _userProfileSearch.GetSideWindow.Save();
                            _userProfileSearch.IsFederatedUserSetToTrueInDatabase(summaryvalueList[1])
                                .ShouldBeTrue("Is Federated Flag should be set to true in database");
                        }

                        _userProfileSearch.ClickOnEditIcon();
                        _userProfileSearch.ClickOnRadioButtonByLabel("SSO User", false);
                        _userProfileSearch.GetSideWindow.Save();
                        _userProfileSearch.IsSSOFlagSetToTrueInDatabase(summaryvalueList[1])
                            .ShouldBeFalse("SSO Flag should be set to true in database");
                        _userProfileSearch.IsFederatedUserSetToTrueInDatabase(summaryvalueList[1])
                            .ShouldBeFalse("Is Federated Flag should be set to false in database");
                        _userProfileSearch.DeleteUserDetailsFromDb(summaryvalueList[1]);
                    }
                }
                finally
                {
                    StringFormatter.PrintMessageTitle("Finally Block. Deleting the created user from the database.");
                    _userProfileSearch.DeleteUserDetailsFromDb(summaryvalueList[1]);
                }
            }
        }

        [Test] //CAR-2149 [CAR-2811]
        public void Verify_Roles_Tab()
        {

            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                UserProfileSearchPage _userProfileSearch;
                automatedBase.CurrentPage =
                    _userProfileSearch = automatedBase.QuickLaunch.NavigateToNewUserProfileSearch();
                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(GetType().FullName, TestName);
                var userId = paramLists["userId"].Split(',').ToList();
                var userTypes = paramLists["userTypes"].Split(',').ToList();
                var roleForInternalUser = paramLists["RoleForInternal"];
                var roleForClientUser = paramLists["RoleForClient"];
                var modifiedByUserSeq =
                    _userProfileSearch.GetCommonSql.GetUserSeqForCurrentlyLoggedInUser(automatedBase.EnvironmentManager.Username);
                try
                {
                    StringFormatter.PrintMessageTitle
                        ($"Verifying navigating to the {UserSettingsTabEnum.RolesAuthorities.GetStringValue()} tab");
                    _userProfileSearch.SearchUserByNameOrId(userId, true);
                    _userProfileSearch.GetGridViewSection.ClickOnGridRowByRow();
                    _userProfileSearch.ClickOnUserSettingTabByTabName(
                        UserSettingsTabEnum.RolesAuthorities.GetStringValue());
                    if (_userProfileSearch.GetAllAssignedRolesList().Count != 0)
                    {
                        if (_userProfileSearch.IsUserSettingsFormDisabled())
                            _userProfileSearch.ClickOnEditIcon();
                        _userProfileSearch.SelectDeselectAll(RolesEnum.AssignedRoles.GetStringValue());
                        _userProfileSearch.GetSideWindow.Save(waitForWorkingMessage: true);
                    }

                    for (int count = 0; count < 2; count++)
                    {
                        var currentUserType = _userProfileSearch.GetGridViewSection.GetValueInGridByColRow(6);

                        StringFormatter.PrintMessage
                            ($"Verification of {RolesEnum.AvailableRoles.GetStringValue()} and {RolesEnum.AssignedRoles.GetStringValue()} lists");
                        _userProfileSearch.ClickOnEditIcon();
                        _userProfileSearch.IsUserSettingsFormDisabled()
                            .ShouldBeFalse
                                ($"{UserSettingsTabEnum.RolesAuthorities.GetStringValue()} form should be enabled after clicking on edit icon");
                        var availableRolesInForm = _userProfileSearch.GetAllAvailableRolesList();
                        var assignedRolesInForm = _userProfileSearch.GetAllAssignedRolesList();
                        var availableRolesFromDatabase =
                            _userProfileSearch.GetRoleNamesByUserTypeFromDb(currentUserType);
                        var listOfInfoIconDescription = _userProfileSearch.GetAllInfoIconDescription()
                            .Select(x => x.Trim()).ToList();
                        var listOfRoleDescriptionFromDB = _userProfileSearch.GetRoleDesriptionFromDb(currentUserType)
                            .Select(x => x.Trim()).ToList();
                        _userProfileSearch.IsInfoIconPresentNextToAvailableRoles()
                            .ShouldBeTrue("Info Icon should be present next to the available roles");
                        listOfInfoIconDescription.ShouldCollectionBeEquivalent(listOfRoleDescriptionFromDB,
                            "Information icon will be shown to the left of each role, displaying the Role Description text in a tool tip");
                        availableRolesInForm
                            .ShouldCollectionBeSorted(false,
                                $"{RolesEnum.AvailableRoles.GetStringValue()} list should be sorted alphabetically");
                        assignedRolesInForm
                            .ShouldCollectionBeEmpty(
                                $"{RolesEnum.AssignedRoles.GetStringValue()} should be empty at the start");
                        availableRolesFromDatabase
                            .ShouldCollectionBeEquivalent(availableRolesInForm,
                                $"'{RolesEnum.AvailableRoles.GetStringValue()}' should display all the applicable roles which are not assigned " +
                                $"to {currentUserType} user");
                        StringFormatter.PrintMessageTitle("Verifying role assignment, save and cancel buttons");
                        var testRole = currentUserType == "Internal" ? roleForInternalUser : roleForClientUser;
                        _userProfileSearch.ClickOnAvailableAssignedRow(1, testRole);
                        _userProfileSearch.GetSideWindow.Save(waitForWorkingMessage: true);
                        _userProfileSearch.IsUserSettingsFormDisabled()
                            .ShouldBeTrue("Form should revert to read only view");
                        StringFormatter.PrintMessage("Verifying whether changes are saved after clicking Save button");
                        _userProfileSearch.ClickOnEditIcon();
                        _userProfileSearch.GetAllAvailableRolesList()
                            .ShouldCollectionBeEquivalent(
                                availableRolesFromDatabase.Where(x => x != testRole).ToList(),
                                $"Role should be moved from '{RolesEnum.AvailableRoles.GetStringValue()}' to '{RolesEnum.AssignedRoles.GetStringValue()}' ");
                        _userProfileSearch.GetAllAssignedRolesList()
                            .ShouldCollectionBeEquivalent(new List<string> {testRole},
                                $"'{RolesEnum.AssignedRoles.GetStringValue()}' should update after a role is assigned");
                        StringFormatter.PrintMessage("Verifying deselecting a particular role from the list");
                        _userProfileSearch
                            .ClickOnAvailableAssignedRow(1, testRole, false);
                        _userProfileSearch.GetSideWindow.Save(waitForWorkingMessage: true);

                        _userProfileSearch.ClickOnEditIcon();
                        _userProfileSearch.GetAllAvailableRolesList().ShouldCollectionBeEquivalent(
                            availableRolesFromDatabase,
                            $"Role from {RolesEnum.AssignedRoles.GetStringValue()} can be moved back to " +
                            $"{RolesEnum.AvailableRoles.GetStringValue()} list");

                        StringFormatter.PrintMessage("Verifying 'Select All' and 'Deselect All' buttons");
                        _userProfileSearch.SelectDeselectAll(RolesEnum.AvailableRoles.GetStringValue());
                        _userProfileSearch.GetSideWindow.Save(waitForWorkingMessage: true);
                        if (_userProfileSearch.IsPageErrorPopupModalPresent())
                        {
                            _userProfileSearch.ClosePageError();
                        }

                        _userProfileSearch.ClickOnUserSettingTabByTabName(
                            UserSettingsTabEnum.Preferences.GetStringValue());
                        _userProfileSearch.SelectDropDownListValueByLabel(
                            UserPreferencesEnum.DefaultDashboard.GetStringValue(), "CV Dashboard");
                        _userProfileSearch.GetSideWindow.Save(waitForWorkingMessage: true);
                        _userProfileSearch.ClickOnUserSettingTabByTabName(UserSettingsTabEnum.RolesAuthorities
                            .GetStringValue());
                        VerifyAuditInformation(_userProfileSearch.GetAllAssignedRolesList());
                        _userProfileSearch.ClickOnEditIcon();
                        _userProfileSearch.GetAllAssignedRolesList().ShouldCollectionBeEquivalent(
                            availableRolesFromDatabase,
                            $"All roles should move from {RolesEnum.AvailableRoles.GetStringValue()} to {RolesEnum.AssignedRoles.GetStringValue()}" +
                            "when 'Select All' button is clicked");
                        _userProfileSearch.GetAllAvailableRolesList()
                            .ShouldCollectionBeEmpty(
                                $"{RolesEnum.AvailableRoles.GetStringValue()} should be empty as all roles are moved to {RolesEnum.AssignedRoles.GetStringValue()}");
                        _userProfileSearch.SelectDeselectAll(RolesEnum.AssignedRoles.GetStringValue());
                        _userProfileSearch.GetSideWindow.Save(waitForWorkingMessage: true);
                        _userProfileSearch.ClickOnEditIcon();
                        _userProfileSearch.GetAllAvailableRolesList().ShouldCollectionBeEquivalent(
                            availableRolesFromDatabase,
                            $"All roles should move from {RolesEnum.AssignedRoles.GetStringValue()} to {RolesEnum.AvailableRoles.GetStringValue()}" +
                            "when 'Deselect All' button is clicked");
                        _userProfileSearch.GetAllAssignedRolesList()
                            .ShouldCollectionBeEmpty
                                ($"{RolesEnum.AssignedRoles.GetStringValue()} should be empty as all roles are moved to {RolesEnum.AvailableRoles.GetStringValue()}");
                        StringFormatter.PrintMessage("Verifying the Cancel button");
                        _userProfileSearch.SelectDeselectAll(RolesEnum.AvailableRoles.GetStringValue());
                        _userProfileSearch.GetSideWindow.Cancel();
                        _userProfileSearch.IsUserSettingsFormDisabled()
                            .ShouldBeTrue("Form should revert back to read only view");
                        _userProfileSearch.ClickOnEditIcon();
                        _userProfileSearch.GetAllAvailableRolesList().ShouldCollectionBeEquivalent(
                            availableRolesFromDatabase,
                            $"All roles should remain unchanged in the {RolesEnum.AvailableRoles.GetStringValue()} when Cancel is clicked");
                        _userProfileSearch.GetAllAssignedRolesList().ShouldCollectionBeEmpty(
                            $"All roles should remain unchanged in the {RolesEnum.AssignedRoles.GetStringValue()} when Cancel is clicked");
                        if (count == 0)
                        {
                            StringFormatter.PrintMessage("Changing the user type");
                            userTypes.Remove(currentUserType);
                            _userProfileSearch.ClickOnUserSettingTabByTabName(
                                UserSettingsTabEnum.Profile.GetStringValue());
                            _userProfileSearch.SelectDropDownListValueByLabel(UserProfileEnum.UserType.GetStringValue(),
                                userTypes[0] == "Internal" ? "Internal" : userTypes[0]);
                            _userProfileSearch.GetSideWindow.Save(waitForWorkingMessage: true);
                            _userProfileSearch.ClickOnUserSettingTabByTabName(UserSettingsTabEnum.RolesAuthorities
                                .GetStringValue());
                        }
                    }

                    #region LOCAL METHODS

                    void VerifyAuditInformation(List<string> assignedAuthorities)
                    {
                        var assignedRolesListInDB =
                            _userProfileSearch.GetLatestAuditFromRoleAuditTableByUserId(userId[0]);
                        var auditDetailsFromDB = _userProfileSearch.GetLatestAuditInfoFromTableByUserId(userId[0]);
                        var sysdate = DateTime.ParseExact(_userProfileSearch.GetSystemDateFromDatabase(),
                            "MM/dd/yyyy hh:mm:ss tt",
                            CultureInfo.InvariantCulture);
                        assignedRolesListInDB.ShouldCollectionBeEquivalent(assignedAuthorities,
                            "Authorities which are assigned to the user" +
                            "should be recorded in the audit table");
                        auditDetailsFromDB[0][0]
                            .ShouldBeEqual(userId[0], "User id should be stored as part of the audit");
                        auditDetailsFromDB[0][2].ShouldBeEqual(modifiedByUserSeq,
                            "UserSeq of the user who modified the role assignment should be stored as part of the audit");
                        Convert.ToDateTime(auditDetailsFromDB[0][3], CultureInfo.InvariantCulture)
                            .AssertDateRange(sysdate.AddMinutes(-2), sysdate.AddMinutes(2),
                                "'Modified Date' should store the last modified date in the audit table");
                    }

                    #endregion
                }

                finally
                {
                    if (_userProfileSearch.IsPageErrorPopupModalPresent())
                    {
                        _userProfileSearch.ClosePageError();
                        _userProfileSearch.GetSideWindow.Cancel();
                    }

                    bool isCorrectUserSelected =
                        _userProfileSearch.GetGridViewSection.GetValueInGridByColRow(3) == userId[0];

                    _userProfileSearch.ClickOnUserSettingTabByTabName(UserSettingsTabEnum.RolesAuthorities
                        .GetStringValue());

                    if ((_userProfileSearch.GetAllAssignedRolesList().Count != 0) && isCorrectUserSelected)
                    {
                        if (_userProfileSearch.IsUserSettingsFormDisabled())
                            _userProfileSearch.ClickOnEditIcon();

                        _userProfileSearch.SelectDeselectAll(RolesEnum.AssignedRoles.GetStringValue());
                        _userProfileSearch.GetSideWindow.Save(waitForWorkingMessage: true);
                    }
                }
            }
        }

        [Test] //CAR-2812(2732)
        public void Verify_access_save_cancel_under_clients_tab()
        {

            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                UserProfileSearchPage _userProfileSearch;
                automatedBase.CurrentPage =
                    _userProfileSearch = automatedBase.QuickLaunch.NavigateToNewUserProfileSearch();
                var TestName = new StackFrame(true).GetMethod().Name;
                var userId = automatedBase.DataHelper.GetSingleTestData(GetType().FullName, TestName, "UserId",
                    "Value");
                var userType = "Internal";
                try
                {

                    _userProfileSearch.SearchUserByNameOrId(new List<string> {userId}, true);
                    _userProfileSearch.ClickOnGridRowByUserIdToOpenUserSettingSideView(userId);
                    _userProfileSearch.ClickOnUserSettingTabByTabName(UserSettingsTabEnum.Clients.GetStringValue());

                    #region RevertSection

                    if (_userProfileSearch.SideBarPanelSearch.IsSideBarPanelOpen())
                        _userProfileSearch.SideBarPanelSearch.ClickOnToggleSidebarPanelButton();
                    _userProfileSearch.ClickEditIconSettings("Clients");

                    if (_userProfileSearch.GetAvailableAssignedList(3, false).Count > 0 || _userProfileSearch
                            .GetAvailableAssignedList(2, false).Contains(ClientEnum.CVTY.GetStringDisplayValue()))
                    {
                        _userProfileSearch.SelectDeselectAll("Assigned Access");
                        _userProfileSearch.ClickOnAvailableAssignedRow(2, ClientEnum.CVTY.GetStringDisplayValue(),
                            false);
                        _userProfileSearch.GetSideWindow.Save(waitForWorkingMessage: true);
                        _userProfileSearch.ClickEditIconSettings("Clients");
                    }

                    _userProfileSearch.ClickOnUserSettingTabByTabName(UserSettingsTabEnum.Profile.GetStringValue());

                    _userProfileSearch.SelectDropDownListValueByLabel(UserProfileEnum.UserType.GetStringValue(),
                        userType);
                    _userProfileSearch.GetSideWindow.Save(waitForWorkingMessage: true);
                    _userProfileSearch.ClickOnUserSettingTabByTabName(UserSettingsTabEnum.Clients.GetStringValue());
                    _userProfileSearch.ClickEditIconSettings("Clients");

                    #endregion



                    for (var i = 0; i < 2; i++)
                    {
                        var expectedAccessList =
                            _userProfileSearch.GetAvailableAccessListByClientFromDb(ClientEnum.SMTST.ToString(),
                                userType == "Internal" ? 2 : 8);
                        _userProfileSearch.GetAvailableAccessListByClientFromDb(ClientEnum.SMTST.ToString(),
                            userType == "HCI" ? 2 : 8);
                        var random = new Random();
                        var access = expectedAccessList[random.Next(expectedAccessList.Count)];

                        #region Verification of available access, click and save functionality

                        _userProfileSearch.GetAvailableAssignedList(3).ShouldCollectionBeEqual(expectedAccessList,
                            "Is Available Access List Equals?");

                        _userProfileSearch.ClickOnAvailableAssignedRow(3, access);

                        _userProfileSearch.GetAvailableAssignedList(3, false)
                            .ShouldCollectionBeEqual(new List<string> {access},
                                "Is Assigned Restriction Equals?");
                        _userProfileSearch.GetAvailableAssignedList(3).ShouldNotContain(access,
                            "Assigned Restriction should not present in Available Access?");

                        _userProfileSearch.ClickOnAvailableAssignedRow(3, access, false);
                        _userProfileSearch.GetAvailableAssignedList(3, false)
                            .ShouldCollectionBeEmpty("Is Assigned Restriction Empty?");

                        _userProfileSearch.SelectDeselectAll("Available Access");
                        _userProfileSearch.GetAvailableAssignedList(3, false).ShouldCollectionBeEqual(
                            expectedAccessList,
                            "Is Available Access List Equals?");
                        _userProfileSearch.GetAvailableAssignedList(3)
                            .ShouldCollectionBeEmpty("Is Assigned Restriction Empty?");

                        _userProfileSearch.SelectDeselectAll("Assigned Access");
                        _userProfileSearch.GetAvailableAssignedList(3, false)
                            .ShouldCollectionBeEmpty("Is Assigned Restriction Empty?");

                        _userProfileSearch.SelectDeselectAll("Available Access");
                        _userProfileSearch.ClickOnAvailableAssignedRow(3, access, false);
                        _userProfileSearch.GetAvailableAssignedList(3, true)
                            .ShouldCollectionBeEqual(new List<string> {access},
                                "Is Assigned Access Equals?");

                        _userProfileSearch.GetAvailableAssignedList(3, false).ShouldNotContain(access,
                            "Assigned Access should not present in Available Access?");

                        _userProfileSearch.GetSideWindow.Save(waitForWorkingMessage: true);
                        _userProfileSearch.RefreshPage();
                        _userProfileSearch.SearchUserByNameOrId(new List<string> {userId}, true);
                        _userProfileSearch.ClickOnGridRowByUserIdToOpenUserSettingSideView(userId);
                        _userProfileSearch.ClickOnUserSettingTabByTabName(UserSettingsTabEnum.Clients.GetStringValue());
                        _userProfileSearch.ClickEditIconSettings("Clients");

                        _userProfileSearch.GetAvailableAssignedList(3)
                            .ShouldCollectionBeEqual(new List<string> {access},
                                "Is Assigned Access Equals?");


                        #endregion

                        #region Verification of Cancellation and change the usertype and add the  another client

                        var preAvailableAccess = _userProfileSearch.GetAvailableAssignedList(3);
                        var preAssignedAccess = _userProfileSearch.GetAvailableAssignedList(3, false);
                        _userProfileSearch.SelectDeselectAll("Assigned Access");
                        _userProfileSearch.GetSideWindow.Cancel();
                        _userProfileSearch.IsUserSettingsFormDisabled()
                            .ShouldBeTrue("Form Should be disabled after cancellation");
                        _userProfileSearch.ClickEditIconSettings("Clients");
                        _userProfileSearch.GetAvailableAssignedList(3)
                            .ShouldCollectionBeEqual(preAvailableAccess, "Available Access List should retain");
                        _userProfileSearch.GetAvailableAssignedList(3, false)
                            .ShouldCollectionBeEqual(preAssignedAccess, "Assigned Access List should retain");
                        _userProfileSearch.SelectDeselectAll("Assigned Access");
                        _userProfileSearch.GetSideWindow.Save(waitForWorkingMessage: true);
                        _userProfileSearch.ClickEditIconSettings("Clients");
                        _userProfileSearch.ClickOnAvailableAssignedRow(2, ClientEnum.CVTY.GetStringDisplayValue());
                        var newRestrictionAfterAdditionClient =
                            _userProfileSearch.GetAvailableRestrictionListByClientListFromDb(
                                new List<string> {ClientEnum.SMTST.ToString(), ClientEnum.CVTY.GetStringDisplayValue()},
                                userType == "Internal" ? 2 : 8);
                        newRestrictionAfterAdditionClient.ShouldCollectionBeNotEqual(expectedAccessList,
                            "Added Client Access should not be same as previous");
                        if (i != 0) continue;
                        userType = userType == "Internal" ? "Client" : "Internal";
                        _userProfileSearch.ClickOnUserSettingTabByTabName(
                            UserSettingsTabEnum.Profile.GetStringValue());
                        _userProfileSearch.SelectDropDownListValueByLabel(UserProfileEnum.UserType.GetStringValue(),
                            userType);
                        _userProfileSearch.ClickOnUserSettingTabByTabName(
                            UserSettingsTabEnum.Clients.GetStringValue());
                        _userProfileSearch.ClickOnAvailableAssignedRow(2, ClientEnum.CVTY.GetStringDisplayValue(),
                            false);
                        _userProfileSearch.GetSideWindow.Save(waitForWorkingMessage: true);
                        _userProfileSearch.ClickEditIconSettings("Clients");

                        #endregion


                    }




                }
                finally
                {
                    if (_userProfileSearch.IsUserSettingsFormDisabled())
                        _userProfileSearch.ClickEditIconSettings("Clients");
                    if (_userProfileSearch.GetAvailableAssignedList(3, false).Count > 0 || _userProfileSearch
                            .GetAvailableAssignedList(3, false).Contains(ClientEnum.CVTY.GetStringDisplayValue()))
                    {
                        _userProfileSearch.SelectDeselectAll("Assigned Access");
                        _userProfileSearch.ClickOnAvailableAssignedRow(2, ClientEnum.CVTY.GetStringDisplayValue(),
                            false);
                        _userProfileSearch.GetSideWindow.Save(waitForWorkingMessage: true);
                        _userProfileSearch.ClickEditIconSettings("Clients");
                    }

                    if (userType == "Internal")
                    {
                        _userProfileSearch.ClickOnUserSettingTabByTabName(
                            UserSettingsTabEnum.Profile.GetStringValue());
                        _userProfileSearch.SelectDropDownListValueByLabel(UserProfileEnum.UserType.GetStringValue(),
                            "Client");
                        _userProfileSearch.ClickOnUserSettingTabByTabName(
                            UserSettingsTabEnum.Clients.GetStringValue());
                        _userProfileSearch.GetSideWindow.Save(waitForWorkingMessage: true);
                    }

                }
            }
        }
        [NonParallelizable]
        [Test, Category("OnDemand")] //CAR-2147 (CAR-2451)
        public void Verify_save_and_cancel_in_user_settings_preferences_side_view()
        {
            


            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                UserProfileSearchPage _userProfileSearch;
                LoginPage _login;
                
                automatedBase.CurrentPage =
                    _userProfileSearch = automatedBase.QuickLaunch.NavigateToNewUserProfileSearch();

                var TestName = new StackFrame().GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(GetType().FullName, TestName);
                var userId = paramLists["userId"].Split(',').ToList();
                var tooltip = new List<string>
                {
                    "Indicates desired default page upon login.",
                    "Indicates desired default client upon login.",
                    "Indicates desired default dashboard view.",
                    "Enables/disables the automatic pop up of Patient Claim History from Claim Action.",
                    "Enables/disables tooltips on Claim Action."
                };

                var defaultPageListForNoAccessAuthority = new List<string>
                {
                    "Claim Search", "My Profile", "Nucleus Home"
                };

                var defaultPageListForAllAccessAuthority = new List<string>
                {
                    "Appeal Search", "Claim Search", "My Profile", "Nucleus Home", "Provider Search",
                    "Suspect Providers"
                };

                var defaultDashboardWithAccessAuthority = new List<string>
                {
                    "My Dashboard", "COB Dashboard", "CV Dashboard", "FFP Dashboard", "Microstrategy"
                };

                var defaultUserPreferencesValue = new List<object>
                {
                    PageHeaderEnum.QuickLaunch.GetStringValue(), ClientEnum.SMTST.ToString(), "CV Dashboard", true, true
                };

                var newUserPreferencesValue = new List<object>
                {
                    "Nucleus Home", ClientEnum.TTREE.ToString(), "FFP Dashboard", false, false
                };

                try
                {
                    _userProfileSearch.SearchUserByNameOrId(new List<string> {userId[0]}, true);
                    _userProfileSearch.ClickOnGridRowByUserIdToOpenUserSettingSideView(userId[0]);
                    _userProfileSearch.ClickOnUserSettingTabByTabName(UserSettingsTabEnum.Preferences.GetStringValue());

                    if (_userProfileSearch.SideBarPanelSearch.IsSideBarPanelOpen())
                        _userProfileSearch.SideBarPanelSearch.ClickOnToggleSidebarPanelButton();

                    StringFormatter.PrintMessageTitle("Verifying whether form is in disabled state when first viewed");
                    _userProfileSearch.IsUserSettingsFormDisabled()
                        .ShouldBeTrue("User Settings form should be disabled at first");
                    _userProfileSearch.GetSelectedUserSettingTab().ShouldBeEqual(
                        UserSettingsTabEnum.Preferences.GetStringValue(),
                        $"'{UserSettingsTabEnum.Preferences.GetStringValue()}' tab should be selected");
                    _userProfileSearch.IsAllTextBoxDisabled().ShouldBeTrue(
                        $"All text boxes should be disabled in the '{UserSettingsTabEnum.Preferences.GetStringValue()}' tab");

                    _userProfileSearch.ClickOnEditIcon();

                    StringFormatter.PrintMessageTitle(
                        "Verification of dashboard page option disabled for those user who does not have any dashboard authority");

                    _userProfileSearch
                        .IsInputFieldByLabelDisabled(UserPreferencesEnum.DefaultDashboard.GetStringValue())
                        .ShouldBeTrue(
                            $"{UserPreferencesEnum.DefaultDashboard.GetStringValue()} should disabled for no dashboard authority present");
                    _userProfileSearch.GetPlaceHolderText(UserPreferencesEnum.DefaultDashboard.GetStringValue())
                        .ShouldBeEqual("No dashboard access assigned.", "Is disabled Message Equals?");


                    var userPreferencesLabelList = Enum.GetValues(typeof(UserPreferencesEnum))
                        .Cast<UserPreferencesEnum>()
                        .Select(x => x.GetStringValue()).ToList();
                    for (var i = 0; i < tooltip.Count; i++)
                    {
                        _userProfileSearch.GetInfoHelpTooltipByLabel(userPreferencesLabelList[i])
                            .ShouldBeEqual(tooltip[i], "Is Tooltip on info Icon Equals?");
                    }

                    StringFormatter.PrintMessageTitle(
                        "Verification of Default Page for that user who do not have any other page privilage");

                    _userProfileSearch
                        .GetDropDownListForUserProfileSettingsByLabel(UserPreferencesEnum.DefaultPage.GetStringValue())
                        .Where(x => x != "").ToList()
                        .ShouldCollectionBeEqual(defaultPageListForNoAccessAuthority,
                            $"Is {UserPreferencesEnum.DefaultPage.GetStringValue()} contains only no authority need authority list");

                    _userProfileSearch.SideBarPanelSearch.OpenSidebarPanel();

                    _userProfileSearch.SearchUserByNameOrId(new List<string> {userId[1]}, true);
                    _userProfileSearch.ClickOnGridRowByUserIdToOpenUserSettingSideView(userId[1]);

                    _userProfileSearch.ClickOnEditIcon();
                    _userProfileSearch.IsUserSettingsFormDisabled()
                        .ShouldBeFalse("Form should be enabled once the edit icon is clicked.");

                    StringFormatter.PrintMessageTitle(
                        "Verification of Default Page, Defafult Client and Defult Dashboard for having all authority");
                    _userProfileSearch
                        .GetDropDownListForUserProfileSettingsByLabel(UserPreferencesEnum.DefaultPage.GetStringValue())
                        .Where(x => x != "").ToList()
                        .ShouldCollectionBeEqual(defaultPageListForAllAccessAuthority,
                            "Is all landing page display?");

                    _userProfileSearch
                        .GetDropDownListForUserProfileSettingsByLabel(
                            UserPreferencesEnum.DefaultClient.GetStringValue())
                        .ShouldCollectionBeEquivalent(
                            _userProfileSearch.GetCommonSql.GetAssignedClientListForUser(automatedBase.EnvironmentManager.Username),
                            "Is assigned client display?");

                    var actualDefaultDashboardList = _userProfileSearch
                        .GetDropDownListForUserProfileSettingsByLabel(UserPreferencesEnum.DefaultDashboard
                            .GetStringValue());
                    actualDefaultDashboardList.RemoveAt(0);
                    actualDefaultDashboardList.ShouldCollectionBeEqual(defaultDashboardWithAccessAuthority,
                        "Is assigned dashboard display?");

                    _userProfileSearch
                        .IsRadioButtonPresentByLabel(UserPreferencesEnum.AutoDisplayPtClHx.GetStringValue())
                        .ShouldBeTrue(
                            $"{UserPreferencesEnum.EnableClAction.GetStringValue()} radio button should display");

                    _userProfileSearch.IsRadioButtonPresentByLabel(UserPreferencesEnum.EnableClAction.GetStringValue())
                        .ShouldBeTrue(
                            $"{UserPreferencesEnum.EnableClAction.GetStringValue()} radio button should display");

                    StringFormatter.PrintMessageTitle("Verification of Cancel functionality");

                    _userProfileSearch.SideBarPanelSearch.OpenSidebarPanel();

                    _userProfileSearch.SearchUserByNameOrId(new List<string> {userId[2]}, true);
                    _userProfileSearch.ClickOnGridRowByUserIdToOpenUserSettingSideView(userId[2]);

                    _userProfileSearch.ClickOnEditIcon();

                    _userProfileSearch.ClickOnRadioButtonByLabel(UserPreferencesEnum.AutoDisplayPtClHx.GetStringValue(),
                        Convert.ToBoolean(newUserPreferencesValue[3]));
                    _userProfileSearch.ClickOnRadioButtonByLabel(UserPreferencesEnum.EnableClAction.GetStringValue(),
                        Convert.ToBoolean(newUserPreferencesValue[4]));
                    _userProfileSearch.SelectDropDownListValueByLabel(UserPreferencesEnum.DefaultPage.GetStringValue(),
                        newUserPreferencesValue[0].ToString());
                    _userProfileSearch.SelectDropDownListValueByLabel(
                        UserPreferencesEnum.DefaultClient.GetStringValue(),
                        newUserPreferencesValue[1].ToString());
                    _userProfileSearch.SelectDropDownListValueByLabel(
                        UserPreferencesEnum.DefaultDashboard.GetStringValue(), newUserPreferencesValue[2].ToString());

                    _userProfileSearch.GetSideWindow.Cancel();

                    StringFormatter.PrintMessageTitle("Verification of Save functionality");

                    _userProfileSearch.ClickOnEditIcon();

                    _userProfileSearch
                        .IsRadioButtonOnOffByLabel(UserPreferencesEnum.AutoDisplayPtClHx.GetStringValue(),
                            Convert.ToBoolean(defaultUserPreferencesValue[3]))
                        .ShouldBeTrue($"{UserPreferencesEnum.AutoDisplayPtClHx.GetStringValue()}");
                    _userProfileSearch
                        .IsRadioButtonOnOffByLabel(UserPreferencesEnum.EnableClAction.GetStringValue(),
                            Convert.ToBoolean(defaultUserPreferencesValue[4]))
                        .ShouldBeTrue($"{UserPreferencesEnum.AutoDisplayPtClHx.GetStringValue()}");

                    _userProfileSearch.GetInputTextBoxValueByLabel(UserPreferencesEnum.DefaultPage.GetStringValue())
                        .ShouldBeEqual(defaultUserPreferencesValue[0].ToString(),
                            $"{UserPreferencesEnum.DefaultPage.GetStringValue()}");
                    _userProfileSearch.GetInputTextBoxValueByLabel(UserPreferencesEnum.DefaultClient.GetStringValue())
                        .ShouldBeEqual(defaultUserPreferencesValue[1].ToString(),
                            $"{UserPreferencesEnum.DefaultClient.GetStringValue()}");
                    _userProfileSearch
                        .GetInputTextBoxValueByLabel(UserPreferencesEnum.DefaultDashboard.GetStringValue())
                        .ShouldBeEqual(defaultUserPreferencesValue[2].ToString(),
                            $"{UserPreferencesEnum.DefaultDashboard.GetStringValue()}");

                    _userProfileSearch.ClickOnUserSettingTabByTabName(UserSettingsTabEnum.Profile.GetStringValue());
                    _userProfileSearch.GetSideWindow.SelectDropDownListByIndex(UserProfileEnum.Status.GetStringValue(),
                        1);
                    _userProfileSearch.ClickOnUserSettingTabByTabName(UserSettingsTabEnum.Preferences.GetStringValue());

                    _userProfileSearch.ClickOnRadioButtonByLabel(UserPreferencesEnum.AutoDisplayPtClHx.GetStringValue(),
                        Convert.ToBoolean(newUserPreferencesValue[3]));
                    _userProfileSearch.ClickOnRadioButtonByLabel(UserPreferencesEnum.EnableClAction.GetStringValue(),
                        Convert.ToBoolean(newUserPreferencesValue[4]));
                    _userProfileSearch.SelectDropDownListValueByLabel(UserPreferencesEnum.DefaultPage.GetStringValue(),
                        newUserPreferencesValue[0].ToString());
                    _userProfileSearch.SelectDropDownListValueByLabel(
                        UserPreferencesEnum.DefaultClient.GetStringValue(),
                        newUserPreferencesValue[1].ToString());
                    _userProfileSearch.SelectDropDownListValueByLabel(
                        UserPreferencesEnum.DefaultDashboard.GetStringValue(), newUserPreferencesValue[2].ToString());

                    _userProfileSearch.GetSideWindow.Save(waitForWorkingMessage: true);
                    _userProfileSearch.RefreshPage();

                    _userProfileSearch.SideBarPanelSearch.OpenSidebarPanel();

                    _userProfileSearch.SearchUserByNameOrId(new List<string> {userId[2]}, true);
                    _userProfileSearch.ClickOnGridRowByUserIdToOpenUserSettingSideView(userId[2]);
                    _userProfileSearch.ClickOnUserSettingTabByTabName(UserSettingsTabEnum.Preferences.GetStringValue());

                    _userProfileSearch.ClickOnEditIcon();

                    StringFormatter.PrintMessageTitle("Verification of changed data");

                    _userProfileSearch
                        .IsRadioButtonOnOffByLabel(UserPreferencesEnum.AutoDisplayPtClHx.GetStringValue(),
                            Convert.ToBoolean(newUserPreferencesValue[3]))
                        .ShouldBeTrue($"{UserPreferencesEnum.AutoDisplayPtClHx.GetStringValue()}");
                    _userProfileSearch
                        .IsRadioButtonOnOffByLabel(UserPreferencesEnum.EnableClAction.GetStringValue(),
                            Convert.ToBoolean(newUserPreferencesValue[4]))
                        .ShouldBeTrue($"{UserPreferencesEnum.AutoDisplayPtClHx.GetStringValue()}");

                    _userProfileSearch.GetInputTextBoxValueByLabel(UserPreferencesEnum.DefaultPage.GetStringValue())
                        .ShouldBeEqual(newUserPreferencesValue[0].ToString(),
                            $"{UserPreferencesEnum.DefaultPage.GetStringValue()}");

                    _userProfileSearch.GetInputTextBoxValueByLabel(UserPreferencesEnum.DefaultClient.GetStringValue())
                        .ShouldBeEqual(newUserPreferencesValue[1].ToString(),
                            $"{UserPreferencesEnum.DefaultClient.GetStringValue()}");
                    _userProfileSearch
                        .GetInputTextBoxValueByLabel(UserPreferencesEnum.DefaultDashboard.GetStringValue())
                        .ShouldBeEqual(newUserPreferencesValue[2].ToString(),
                            $"{UserPreferencesEnum.DefaultDashboard.GetStringValue()}");

                    _login = _userProfileSearch.Logout();

                    _login.LoginByPage<ClaimSearchPage>(userId[0], UserType.CLIENT);
                    automatedBase.CurrentPage.IsDashboardIconPresent()
                        .ShouldBeFalse("Is Dashboard Icon Present for no dashboard authority user");

                    automatedBase.CurrentPage.Logout()
                        .LoginByPage<BatchSearchPage>(userId[2], UserType.CLIENT);
                    automatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(newUserPreferencesValue[0].ToString(),
                        "Landing Page should be batch search page.");
                    automatedBase.CurrentPage.GetCurrentClient().ShouldBeEqual(ClientEnum.TTREE.GetStringValue(),
                        "Default Client Code should Updated");

                    var dashboard = _userProfileSearch.NavigateToDashboard();
                    if (automatedBase.CurrentPage.IsPageErrorPopupModalPresent())
                        automatedBase.CurrentPage.ClosePageError();
                    dashboard.IsContainerHeaderClaimsOverviewFFPPresent().ShouldBeTrue("Is FFP Dashboard Page Opened?");
                }
                finally
                {
                    if (automatedBase.CurrentPage.CurrentPageTitle == PageTitleEnum.Login.GetStringValue())
                    { 
                        _userProfileSearch = automatedBase.Login.LoginAsHciAdminUser().NavigateToNewUserProfileSearch();
                    }
                    else if (string.Compare(UserType.CurrentUserType, UserType.HCIADMIN,
                                 StringComparison.OrdinalIgnoreCase) != 0)
                    {
                        if (automatedBase.CurrentPage.IsPageErrorPopupModalPresent())
                            automatedBase.CurrentPage.ClosePageError();
                        _userProfileSearch =
                            automatedBase.CurrentPage.Logout().LoginAsHciAdminUser().NavigateToNewUserProfileSearch();
                    }

                    StringFormatter.PrintMessageTitle("Revert the data such that there will be no failure in next run");

                    if (_userProfileSearch.GetSideWindow.IsSaveButtonPresent())
                        _userProfileSearch.GetSideWindow.Cancel();

                    _userProfileSearch.SideBarPanelSearch.OpenSidebarPanel();

                    _userProfileSearch.SearchUserByNameOrId(new List<string> {userId[2]}, true);
                    _userProfileSearch.ClickOnGridRowByUserIdToOpenUserSettingSideView(userId[2]);
                    _userProfileSearch.ClickOnUserSettingTabByTabName(UserSettingsTabEnum.Preferences.GetStringValue());
                    _userProfileSearch.ClickOnEditIcon();

                    _userProfileSearch.ClickOnRadioButtonByLabel(UserPreferencesEnum.AutoDisplayPtClHx.GetStringValue(),
                        Convert.ToBoolean(defaultUserPreferencesValue[3]));
                    _userProfileSearch.ClickOnRadioButtonByLabel(UserPreferencesEnum.EnableClAction.GetStringValue(),
                        Convert.ToBoolean(defaultUserPreferencesValue[4]));
                    _userProfileSearch.SelectDropDownListValueByLabel(UserPreferencesEnum.DefaultPage.GetStringValue(),
                        defaultUserPreferencesValue[0].ToString());
                    _userProfileSearch.SelectDropDownListValueByLabel(
                        UserPreferencesEnum.DefaultClient.GetStringValue(), defaultUserPreferencesValue[1].ToString());
                    _userProfileSearch.SelectDropDownListValueByLabel(
                        UserPreferencesEnum.DefaultDashboard.GetStringValue(),
                        defaultUserPreferencesValue[2].ToString());

                    _userProfileSearch.GetSideWindow.Save(waitForWorkingMessage: true);
                }
            }
        }

        [Test] //TE-373
        public void Verify_security_and_navigation_of_the_new_User_Profile_search_page()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                UserProfileSearchPage _userProfileSearch;
                LoginPage login;
                CommonValidations _commonValidations = new CommonValidations(automatedBase.CurrentPage);
                string _userMaintenance = RoleEnum.NucleusAdmin.GetStringValue();

                automatedBase.CurrentPage =
                    _userProfileSearch = automatedBase.QuickLaunch.NavigateToNewUserProfileSearch();
                StringFormatter.PrintMessage("Verify Find User Profile Panel Open By Default");
                _userProfileSearch.SideBarPanelSearch.IsSideBarPanelOpen()
                    .ShouldBeTrue(" By default Find User Profile panel displayed ?");
                StringFormatter.PrintMessage("Verify Sort and Find User Profiles Panel Control Icon");
                _userProfileSearch.GetGridViewSection.IsFilterOptionIconPresent()
                    .ShouldBeTrue("Is Filter Option Icon Present ?");
                _userProfileSearch.GetGridViewSection.IsSideBarIconPresent()
                    .ShouldBeTrue("Is Side Bar Icon Present ?");
                _userProfileSearch.SideBarPanelSearch.ClickOnToggleSidebarPanelButton();
                _userProfileSearch.SideBarPanelSearch.IsSideBarPanelOpen().ShouldBeFalse("Sidebar open after toggle ?");
                _commonValidations.ValidateSecurityAndNavigationOfAPage(HeaderMenu.Settings,
                    new List<string> {SubMenu.User, SubMenu.NewUserProfileSearch},
                    _userMaintenance,

                    new List<string> {PageHeaderEnum.UserProfileSearch.GetStringValue()},
                    automatedBase.Login.LoginAsUserHavingNoAnyAuthority, new[] {"Test4", "Automation4"});
            }
            }
        

        [Test, Category("SmokeTestDeployment")] //CAR-2146 (CAR-2387) +TE-949
        public void Verify_security_and_navigation_of_new_user_settings_side_view()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                UserProfileSearchPage _userProfileSearch;

                automatedBase.CurrentPage =
                    _userProfileSearch = automatedBase.QuickLaunch.NavigateToNewUserProfileSearch();

                StringFormatter.PrintMessage("Verify Find User Profile Panel Open By Default");
                _userProfileSearch.SideBarPanelSearch.IsSideBarPanelOpen()
                    .ShouldBeTrue(" By default Find User Profile panel displayed ?");
                StringFormatter.PrintMessage("Verify Sort and Find User Profiles Panel Control Icon");
                _userProfileSearch.GetGridViewSection.IsFilterOptionIconPresent()
                    .ShouldBeTrue("Is Filter Option Icon Present ?");
                _userProfileSearch.GetGridViewSection.IsSideBarIconPresent()
                    .ShouldBeTrue("Is Side Bar Icon Present ?");
                _userProfileSearch.SideBarPanelSearch.ClickOnToggleSidebarPanelButton();
                _userProfileSearch.SideBarPanelSearch.IsSideBarPanelOpen().ShouldBeFalse("Sidebar open after toggle ?");

                StringFormatter.PrintMessageTitle(
                    "Verifying security of the User Settings side view when viewed by a user with read only access to 'User Maintenance'");
                _userProfileSearch = _userProfileSearch.Logout().LoginAsHCIUserWithReadOnlyAccessToAllAuthorities()
                    .NavigateToNewUserProfileSearch();
                _userProfileSearch.IsAddUserIconPresent()
                    .ShouldBeFalse(
                        "Add userIcon should not be displayed for user with Nucleus Admin Readonly Authority"); // TE-949
                _userProfileSearch.SearchUserByNameOrId(new List<string> {automatedBase.EnvironmentManager.Username}, true);
                _userProfileSearch.ClickOnGridRowByUserIdToOpenUserSettingSideView(automatedBase.EnvironmentManager.Username);
                //_userProfileSearch.GetSideWindow.IsEditIconDisabled().ShouldBeTrue("Edit icon should be disabled for user with readonly" +
                //                                                                      "privilege for Product Management authority");
                foreach (UserSettingsTabEnum tab in Enum.GetValues(typeof(UserSettingsTabEnum)))
                {
                    _userProfileSearch.ClickOnUserSettingTabByTabName(tab.GetStringValue());
                    _userProfileSearch.GetSideWindow.IsEditDisabled().ShouldBeTrue(
                        "Edit Icon should be disabled for user with readonly privilege for Product Management authority.");
                }

            }
        }

        [Test] //CAR-2146 (CAR-2387) +CAR-3036(CAR -2959)
        public void Validate_form_fields_of_user_profile_settings_side_view()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                UserProfileSearchPage _userProfileSearch;
                LoginPage _login;
                automatedBase.CurrentPage =
                    _userProfileSearch = automatedBase.QuickLaunch.NavigateToNewUserProfileSearch();
                var TestName = new StackFrame().GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(GetType().FullName, TestName);
                var userId = paramLists["userId"];
                var expectedUserTypes = paramLists["UserTypes"].Split(';').ToList();
                var validEmailAddress = paramLists["ValidEmailAddress"];
                var invalidEmailAddresses = paramLists["InvalidEmailAddresses"].Split(';').ToList();
                var profileTabName = UserSettingsTabEnum.Profile.GetStringValue();
                var expectedDepartments = _userProfileSearch.GetExpectedDepartmentValuesFromTable();
                var InternalUserId = paramLists["InternalUserId"];

                try
                {
                    _userProfileSearch.SearchUserByNameOrId(new List<string> {userId}, true);
                    _userProfileSearch.ClickOnGridRowByUserIdToOpenUserSettingSideView(userId);

                    if (_userProfileSearch.SideBarPanelSearch.IsSideBarPanelOpen())
                        _userProfileSearch.SideBarPanelSearch.ClickOnToggleSidebarPanelButton();

                    StringFormatter.PrintMessageTitle("Verifying whether form is in disabled state when first viewed");
                    _userProfileSearch.IsUserSettingsFormDisabled()
                        .ShouldBeTrue("User Settings form should be disabled at first");
                    _userProfileSearch.GetSelectedUserSettingTab().ShouldBeEqual(profileTabName,
                        $"'{UserSettingsTabEnum.Profile.GetStringValue()}' tab should be selected");
                    _userProfileSearch.IsAllTextBoxDisabled()
                        .ShouldBeTrue($"All text boxes should be disabled in the '{profileTabName}' tab");

                    StringFormatter.PrintMessage(
                        "Verification whether all the required labels and input fields are present");
                    var profileOptions = Enum.GetValues(typeof(UserProfileEnum)).Cast<UserProfileEnum>()
                        .Select(x => x.GetStringValue()).ToList();

                    foreach (var option in profileOptions)
                    {
                        if (option == UserProfileEnum.SubscriberID.GetStringValue() ||
                            option == UserProfileEnum.Department.GetStringValue())
                            continue; // Subscriber ID , department will be tested separately below
                        _userProfileSearch.IsLabelPresent(option).ShouldBeTrue($"'{option}' label should be present");
                        _userProfileSearch.IsInputFieldByLabelPresent(option)
                            .ShouldBeTrue($"'Input field for '{option}' should be present");
                    }

                    _userProfileSearch.GetSideWindow.ClickOnEditIcon();
                    _userProfileSearch.IsUserSettingsFormDisabled()
                        .ShouldBeFalse("Form should be enabled once the edit icon is clicked.");

                    StringFormatter.PrintMessageTitle("Validating the 'User Information' input fields");

                    _userProfileSearch.IsInputFieldByLabelDisabled(UserProfileEnum.UserId.GetStringValue())
                        .ShouldBeTrue($"'{UserProfileEnum.UserId.GetStringValue()}' should not be an editable field");

                    _userProfileSearch
                        .GetDropDownListForUserProfileSettingsByLabel(UserProfileEnum.UserType.GetStringValue())
                        .ShouldCollectionBeEqual(expectedUserTypes,
                            $"'{UserProfileEnum.UserType.GetStringValue()}' should have correct dropdown values");

                    var statusOptions = Enum.GetValues(typeof(UserProfileStatusEnum)).Cast<UserProfileStatusEnum>()
                        .Select(x => x.GetStringValue()).ToList();
                    _userProfileSearch
                        .GetDropDownListForUserProfileSettingsByLabel(UserProfileEnum.Status.GetStringValue())
                        .ShouldCollectionBeEqual(statusOptions,
                            $"'{UserProfileEnum.Status.GetStringValue()}' should have correct dropdown values");

                    StringFormatter.PrintMessageTitle("Validating the 'Personal Information' input fields");
                    var alphanumericValue = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~";

                    ValidateInputFieldByLabel(UserProfileEnum.Prefix.GetStringValue(), alphanumericValue, 15, false);
                    ValidateInputFieldByLabel(UserProfileEnum.FirstName.GetStringValue(), alphanumericValue, 50);
                    ValidateInputFieldByLabel(UserProfileEnum.LastName.GetStringValue(), alphanumericValue, 50);
                    ValidateInputFieldByLabel(UserProfileEnum.Suffix.GetStringValue(), alphanumericValue, 15, false);
                    ValidateInputFieldByLabel(UserProfileEnum.Credentials.GetStringValue(), alphanumericValue, 100,
                        false);
                    ValidateInputFieldByLabel(UserProfileEnum.JobTitle.GetStringValue(), alphanumericValue, 50);

                    _userProfileSearch.SelectDropDownListValueByLabel(UserProfileEnum.UserType.GetStringValue(),
                        "Internal");
                    _userProfileSearch.IsInputFieldByLabelPresent(UserProfileEnum.SubscriberID.GetStringValue())
                        .ShouldBeFalse(
                            $"{UserProfileEnum.SubscriberID.GetStringValue()} should not be shown for internal users");

                    _userProfileSearch.SelectDropDownListValueByLabel(UserProfileEnum.UserType.GetStringValue(),
                        "Client");
                    _userProfileSearch.IsInputFieldByLabelPresent(UserProfileEnum.SubscriberID.GetStringValue())
                        .ShouldBeTrue(
                            $"{UserProfileEnum.SubscriberID.GetStringValue()} should be shown only for client users");
                    ValidateInputFieldByLabel(UserProfileEnum.SubscriberID.GetStringValue(), alphanumericValue, 100,
                        false);

                    StringFormatter.PrintMessageTitle("Validating the 'Contact Information' input fields");
                    var unallowedValues = "ABCDEFGHIJKLMNOPQRSTUVWXYZ!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~";
                    var allowedNumericValues = "0123456789";

                    ValidatePhoneFaxInputFieldByLabel(UserProfileEnum.Phone.GetStringValue(), allowedNumericValues, 10);
                    ValidatePhoneFaxInputFieldByLabel(UserProfileEnum.Fax.GetStringValue(), allowedNumericValues, 10,
                        false);
                    ValidatePhoneFaxInputFieldByLabel(UserProfileEnum.AltPhone.GetStringValue(), allowedNumericValues,
                        10, false);

                    StringFormatter.PrintMessage("Validating the 'Email Address' field");


                    ValidateEmailInputField(validEmailAddress);

                    foreach (var invalidEmailAddress in invalidEmailAddresses)
                    {
                        ValidateEmailInputField(invalidEmailAddress, false);
                    }

                    // CAR-3036+CAR-3057
                    StringFormatter.PrintMessage("Verify Department field for Internal User Only");
                    var dept = UserProfileEnum.Department.GetStringValue();
                    _userProfileSearch.SearchUserByNameOrId(new List<string> {InternalUserId}, true);
                    _userProfileSearch.ClickOnGridRowByUserIdToOpenUserSettingSideView(InternalUserId);
                    _userProfileSearch.GetSideWindow.ClickOnEditIcon();

                    for (var i = 0; i < 2; i++)
                    {

                        _userProfileSearch.ClickOnRadioButtonByLabel("SSO User",
                            !_userProfileSearch.IsRadioButtonOnOffByLabel("SSO User"));


                        _userProfileSearch.IsLabelPresent(dept).ShouldBeTrue("Department label should be present");
                        _userProfileSearch.IsInputFieldByLabelPresent(dept)
                            .ShouldBeTrue($"'Input field for {dept} should be present for internal users");
                        var departmentlabel =
                            _userProfileSearch.GetDropDownListForUserProfileSettingsByLabel(dept);
                        departmentlabel.RemoveAt(0);
                        departmentlabel.ShouldCollectionBeEquivalent(expectedDepartments,
                            $"'{dept}' should have correct dropdown values");

                    }



                    _userProfileSearch.SelectDropDownListValueByLabel(UserProfileEnum.UserType.GetStringValue(),
                        "Client");
                    _userProfileSearch.IsInputFieldByLabelPresent(dept)
                        .ShouldBeFalse($"'Input field for {dept} should not be present for client users");
                }
                finally
                {
                    if (!_userProfileSearch.IsUserSettingsFormDisabled())
                        _userProfileSearch.GetSideWindow.Cancel();
                }

                #region Local Methods

                void ClickOnProfileTab() =>
                    _userProfileSearch.ClickOnUserSettingTabByTabName(UserSettingsTabEnum.Profile.GetStringValue());

                void ValidateEmailInputField(string emailAddress, bool isValidEmail = true)
                {
                    var label = UserProfileEnum.EmailAddress.GetStringValue();

                    _userProfileSearch.SetInputTextBoxValueByLabel(label, emailAddress);
                    //ClickOnProfileTab();

                    if (isValidEmail)
                        _userProfileSearch.IsInvalidInputPresentByLabel(label)
                            .ShouldBeFalse($"{label} should accept email address in the form of XXX@XXX.XXX");

                    else
                        _userProfileSearch.IsInvalidInputPresentByLabel(label).ShouldBeTrue(
                            $"{label} Error highlight should be shown in the text input field when un invalid email is entered");
                }

                void ValidateInputFieldByLabel(string label, string acceptedValues, int lengthAllowed,
                    bool requiredField = true)
                {
                    var inputText =
                        _userProfileSearch.GenerateRandomTextBasedOnAcceptedValues(acceptedValues, lengthAllowed + 1);

                    _userProfileSearch.SetInputTextBoxValueByLabel(label, inputText);
                    //ClickOnProfileTab();
                    _userProfileSearch.GetInputTextBoxValueByLabel(label)
                        .ShouldBeEqual(inputText.Substring(0, lengthAllowed));

                    if (requiredField)
                    {
                        _userProfileSearch.SetInputTextBoxValueByLabel(label, "");
                        //ClickOnProfileTab();
                        _userProfileSearch.IsInvalidInputPresentByLabel(label)
                            .ShouldBeTrue($"'{label}' should be a required field");
                    }

                    else
                    {
                        _userProfileSearch.SetInputTextBoxValueByLabel(label, "");
                        //ClickOnProfileTab();
                        _userProfileSearch.IsInvalidInputPresentByLabel(label)
                            .ShouldBeFalse($"'{label}' should not be a required field");
                    }
                }

                void ValidatePhoneFaxInputFieldByLabel(string label, string values, int length,
                    bool requiredField = true)
                {
                    var inputTextForPhoneFax =
                        _userProfileSearch.GenerateRandomTextBasedOnAcceptedValues(values, length + 1);
                    var inputTextForExt =
                        _userProfileSearch.GenerateRandomTextBasedOnAcceptedValues(values, length + 1);
                    while (inputTextForPhoneFax.StartsWith("1"))
                    {
                        inputTextForPhoneFax =
                            _userProfileSearch.GenerateRandomTextBasedOnAcceptedValues(values, length + 1);
                    }


                    _userProfileSearch.SetInputInPhoneFaxFields(label, inputTextForPhoneFax, inputTextForExt);
                    //ClickOnProfileTab();

                    var phoneFaxTextFromForm = _userProfileSearch.GetInputFromPhoneFaxFields(label);
                    var extTextFromForm = _userProfileSearch.GetInputFromPhoneFaxFields(label, false);
                    var phoneFaxRegexWithoutHyphen = phoneFaxTextFromForm.Replace("-", "");

                    Regex phoneFaxRegex = new Regex("^\\d{3}-\\d{3}-\\d{4}$");
                    phoneFaxRegex.IsMatch(phoneFaxTextFromForm)
                        .ShouldBeTrue("Entered phone should be in correct format");
                    phoneFaxRegexWithoutHyphen.ShouldBeEqual(inputTextForPhoneFax.Substring(0, length),
                        $"'{label}' field should accept only {length} numbers");

                    extTextFromForm.ShouldBeEqual(inputTextForExt.Substring(0, length),
                        $"Ext should only allow {length} numbers");

                    _userProfileSearch.SetInputInPhoneFaxFields(label, "", "");
                    //ClickOnProfileTab();

                    if (requiredField)
                        _userProfileSearch.IsInvalidInputPresentByLabel(label)
                            .ShouldBeTrue($"'{label}' should be a required field");

                    else
                        _userProfileSearch.IsInvalidInputPresentByLabel(label)
                            .ShouldBeFalse($"'{label}' should not be a required field");

                }

                #endregion
            }
        }

        [Test] //CAR-2146 (CAR-2387)
        [NonParallelizable]
        public void Verify_save_and_cancel_in_user_settings_side_view()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                UserProfileSearchPage _userProfileSearch;
                LoginPage _login;
                automatedBase.CurrentPage =
                    _userProfileSearch = automatedBase.QuickLaunch.NavigateToNewUserProfileSearch();
                var TestName = new StackFrame().GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(GetType().FullName, TestName);
                var userId = paramLists["userId"];

                try
                {
                    //CurrentPage.RefreshPage();


                    _userProfileSearch.SearchUserByNameOrId(new List<string> {userId}, true);
                    _userProfileSearch.ClickOnGridRowByUserIdToOpenUserSettingSideView(userId);
                    _userProfileSearch.ClickOnUserSettingTabByTabName(UserSettingsTabEnum.Profile.GetStringValue());
                    _userProfileSearch.GetSideWindow.ClickOnEditIcon();

                    if (_userProfileSearch.SideBarPanelSearch.IsSideBarPanelOpen())
                        _userProfileSearch.SideBarPanelSearch.ClickOnToggleSidebarPanelButton();

                    #region Verifying Cancel

                    StringFormatter.PrintMessageTitle("Verifying the Cancel button");
                    var profileOptions = Enum.GetValues(typeof(UserProfileEnum)).Cast<UserProfileEnum>()
                        .Select(x => x.GetStringValue()).ToList();

                    var initialFormDataBeforeCancelling =
                        _userProfileSearch.GetAllFormDataFromAllFieldsInUserProfileTab(profileOptions);

                    Dictionary<string, string> personalInformation = new Dictionary<string, string>
                    {
                        [UserProfileEnum.Prefix.GetStringValue()] =
                            paramLists["PersonalInfoToSave"].Split(';').ToList()[0],
                        [UserProfileEnum.FirstName.GetStringValue()] =
                            paramLists["PersonalInfoToSave"].Split(';').ToList()[1],
                        [UserProfileEnum.LastName.GetStringValue()] =
                            paramLists["PersonalInfoToSave"].Split(';').ToList()[2],
                        [UserProfileEnum.Suffix.GetStringValue()] =
                            paramLists["PersonalInfoToSave"].Split(';').ToList()[3],
                        [UserProfileEnum.Credentials.GetStringValue()] =
                            paramLists["PersonalInfoToSave"].Split(';').ToList()[4],
                        [UserProfileEnum.JobTitle.GetStringValue()] =
                            paramLists["PersonalInfoToSave"].Split(';').ToList()[5]
                    };

                    FillForm(profileOptions, personalInformation, true);

                    _userProfileSearch.GetSideWindow.Cancel();
                    _userProfileSearch.WaitForWorking();

                    _userProfileSearch.GetSideWindow.ClickOnEditIcon();
                    _userProfileSearch.GetAllFormDataFromAllFieldsInUserProfileTab(profileOptions)
                        .ShouldCollectionBeEqual(initialFormDataBeforeCancelling,
                            "Form data should not be saved when Cancel button is clicked");

                    #endregion

                    #region Verifying Save

                    StringFormatter.PrintMessageTitle("Verifying Save button functionality");
                    FillForm(profileOptions, personalInformation, true);
                    var dataEnteredToSave =
                        _userProfileSearch.GetAllFormDataFromAllFieldsInUserProfileTab(profileOptions, false);
                    _userProfileSearch.GetSideWindow.Save(waitForWorkingMessage: true);

                    _userProfileSearch.GetAllFormDataFromAllFieldsInUserProfileTab(profileOptions)
                        .ShouldCollectionBeEqual(dataEnteredToSave,
                            "Data should be saved correctly when save is clicked");

                    #endregion

                    #region Verifying Update

                    StringFormatter.PrintMessageTitle("Revert the data to the default value");
                    _userProfileSearch.GetSideWindow.ClickOnEditIcon();
                    FillForm(profileOptions, personalInformation);

                    //var dataToUpdate =
                    //    _userProfileSearch.GetAllFormDataFromAllFieldsInUserProfileTab(profileOptions);
                    _userProfileSearch.GetSideWindow.Save(waitForWorkingMessage: true);

                    _userProfileSearch.GetSideWindow.ClickOnEditIcon();

                    //_userProfileSearch.GetAllFormDataFromAllFieldsInUserProfileTab(profileOptions)
                    //    .ShouldCollectionBeEqual(dataToUpdate,
                    //        "Data should be updated correctly when changes are made");

                    #endregion

                    #region Verifying Subscriber ID field

                    StringFormatter.PrintMessageTitle(
                        "Verification of saving and updating the Subscriber field for client user type");

                    var subscriberID =
                        _userProfileSearch.GenerateRandomTextBasedOnAcceptedValues("abcdefghijklmnopqrstuvwxyz", 100);
                    _userProfileSearch.SelectDropDownListValueByLabel(UserProfileEnum.UserType.GetStringValue(),
                        "Client");
                    _userProfileSearch.SetInputTextBoxValueByLabel(UserProfileEnum.SubscriberID.GetStringValue(),
                        subscriberID);
                    _userProfileSearch.GetSideWindow.Save(waitForWorkingMessage: true);

                    _userProfileSearch.GetSideWindow.ClickOnEditIcon();

                    _userProfileSearch.GetInputTextBoxValueByLabel(UserProfileEnum.SubscriberID.GetStringValue())
                        .ShouldBeEqual(subscriberID, "Subscriber field should updated");

                    #endregion

                    #region LOCAL METHODS

                    void FillForm(List<string> labelList, Dictionary<string, string> personalInformationDictionary,
                        bool enterRandomPersonalInfo = false)
                    {
                        //Entering all dropdown fields and Contact Information
                        foreach (var label in labelList)
                        {
                            switch (label)
                            {
                                case "User ID":
                                    break;

                                case "User Type":
                                    var userTypeToBeEntered =
                                        _userProfileSearch.GetInputTextBoxValueByLabel(label) == "Internal"
                                            ? "Client"
                                            : "Internal";
                                    _userProfileSearch.SelectDropDownListValueByLabel(label,
                                        userTypeToBeEntered);
                                    break;

                                case "Status":
                                    var availableStatusList =
                                        _userProfileSearch.GetDropDownListForUserProfileSettingsByLabel(label);
                                    availableStatusList.Remove(UserProfileStatusEnum.Locked.GetStringValue());
                                    availableStatusList.Remove(UserProfileStatusEnum.Frozen.GetStringValue());
                                    var random = new Random();

                                    var nextIndex = random.Next(availableStatusList.Count);

                                    _userProfileSearch.SelectDropDownListValueByLabel(label,
                                        availableStatusList[nextIndex]);
                                    break;

                                case "Email Address":
                                    var emailAddressToBeEntered =
                                        _userProfileSearch.GenerateRandomTextBasedOnAcceptedValues(
                                            "abcdefghijklmnopqrstuvwxyz", 5) +
                                        "@test.com";
                                    _userProfileSearch.SetInputTextBoxValueByLabel(
                                        UserProfileEnum.EmailAddress.GetStringValue(),
                                        emailAddressToBeEntered);
                                    break;
                            }
                        }

                        if (!enterRandomPersonalInfo)
                            foreach (KeyValuePair<string, string> labelAndValue in personalInformationDictionary)
                                _userProfileSearch.SetInputTextBoxValueByLabel(labelAndValue.Key, labelAndValue.Value);
                        else
                        {
                            var alphanumericValue =
                                "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~";

                            foreach (KeyValuePair<string, string> labelAndValue in personalInformationDictionary)
                                _userProfileSearch.SetInputTextBoxValueByLabel(labelAndValue.Key,
                                    _userProfileSearch.GenerateRandomTextBasedOnAcceptedValues(alphanumericValue, 100));
                        }

                        _userProfileSearch.SetInputInPhoneFaxFields(UserProfileEnum.Phone.GetStringValue(),
                            _userProfileSearch.GetRandomPhoneNumber(),
                            _userProfileSearch.GetRandomPhoneNumber());

                        _userProfileSearch.SetInputInPhoneFaxFields(UserProfileEnum.Fax.GetStringValue(),
                            _userProfileSearch.GetRandomPhoneNumber(),
                            _userProfileSearch.GetRandomPhoneNumber());

                        _userProfileSearch.SetInputInPhoneFaxFields(UserProfileEnum.AltPhone.GetStringValue(),
                            _userProfileSearch.GetRandomPhoneNumber(),
                            _userProfileSearch.GetRandomPhoneNumber());
                    }
                }

                #endregion

                finally
                {
                    if (!_userProfileSearch.IsUserSettingsFormDisabled())
                        _userProfileSearch.GetSideWindow.Cancel();
                    _userProfileSearch.RevertUserToActiveStatus(userId);
                }
            }
        }

        [Test] //TE-486
        public void Verify_Primary_Details_In_New_User_Profile_search()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                UserProfileSearchPage _userProfileSearch;
                LoginPage _login;
                automatedBase.CurrentPage =
                    _userProfileSearch = automatedBase.QuickLaunch.NavigateToNewUserProfileSearch();
                var TestName = new StackFrame().GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(GetType().FullName, TestName);
                StringFormatter.PrintMessage("Verify Primary data");
                var expectedList = _userProfileSearch.GetPrimaryDataResultFromDatabase(paramLists["FirstName"]);
                var firstName = paramLists["PartialMatch"].Split(',').ToList()[0];
                var lastName = paramLists["PartialMatch"].Split(',').ToList()[1];
                _userProfileSearch.SideBarPanelSearch.SetInputFieldByLabel("First Name", paramLists["FirstName"]);
                _userProfileSearch.SideBarPanelSearch.SelectDropDownListValueByLabel("User Type", "Client");
                _userProfileSearch.SideBarPanelSearch.SelectDropDownListValueByLabel("User Account Status", "Active");
                _userProfileSearch.SideBarPanelSearch.ClickOnFindButton();
                _userProfileSearch.WaitForWorkingAjaxMessage();
                _userProfileSearch.GetGridViewSection.GetGridRowCount().ShouldBeGreater(0, "Results Displayed?");

                StringFormatter.PrintMessage("Verify Labels in Grid");
                ValidateLabelsInGrid(_userProfileSearch);
                StringFormatter.PrintMessage("Verify Values in Grid");
                _userProfileSearch.GetGridViewSection.GetGridListValueByCol(2)
                    .ShouldCollectionBeEqual(expectedList.Select(x => x[0]).ToList(),
                        "Username Validated and sorted by first name lastname by default");
                _userProfileSearch.GetGridViewSection.GetGridListValueByCol(3)
                    .ShouldCollectionBeEqual(expectedList.Select(x => x[1]).ToList(), "UserId Validated?");
                _userProfileSearch.GetGridViewSection.GetGridListValueByCol(5)
                    .ShouldCollectionBeEqual(expectedList.Select(x => x[3]).ToList(), "email Validated?");
                _userProfileSearch.GetGridViewSection.GetGridListValueByCol(6)
                    .ShouldCollectionBeEqual(expectedList.Select(x => x[4]).ToList(), "UserType Validated?");
                StringFormatter.PrintMessage("Verify phone number in correct format");
                var phoneNumberDisplayed = _userProfileSearch.GetGridViewSection.GetValueInGridByColRow(4);
                phoneNumberDisplayed.IsPhoneNumberInCorrectFormat()
                    .ShouldBeTrue("Verify phone number format");
                phoneNumberDisplayed = Regex.Replace(phoneNumberDisplayed, @"[()-]", "").Remove(3, 1);
                phoneNumberDisplayed.ShouldBeEqual(expectedList[0][2], "Phone Number Value Verified");
                _userProfileSearch.SideBarPanelSearch.ClickOnClearLink();

                StringFormatter.PrintMessage("Verify exact match for user id");
                _userProfileSearch.SideBarPanelSearch.SetInputFieldByLabel("User ID", paramLists["User_id"]);
                _userProfileSearch.SideBarPanelSearch.ClickOnFindButton();
                _userProfileSearch.WaitForWorkingAjaxMessage();
                _userProfileSearch.GetGridViewSection.GetGridRowCount()
                    .ShouldBeEqual(1, "User Id should Result into exact match");
                _userProfileSearch.GetGridViewSection.GetValueInGridByColRow(3)
                    .ShouldBeEqual(paramLists["User_id"], "User Id value same?");
                _userProfileSearch.SideBarPanelSearch.ClickOnClearLink();


                StringFormatter.PrintMessage("Verify partial match for first and last name");
                _userProfileSearch.SideBarPanelSearch.SetInputFieldByLabel("First Name", firstName);
                _userProfileSearch.SideBarPanelSearch.SetInputFieldByLabel("Last Name", lastName);
                _userProfileSearch.SideBarPanelSearch.ClickOnFindButton();
                _userProfileSearch.WaitForWorkingAjaxMessage();
                _userProfileSearch.GetGridViewSection.GetGridListValueByCol(3).ShouldCollectionBeEqual(
                    _userProfileSearch.GetUserIdForpartialMatchFromDatabase(firstName, lastName), "UserId Correct?");
                _userProfileSearch.SideBarPanelSearch.ClickOnClearLink();

            }
        }

        [Test] //TE-486
        public void Verify_User_Status_Icon_and_Filter_Results_Based_On_UserStatus()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                UserProfileSearchPage _userProfileSearch;
                LoginPage _login;
                automatedBase.CurrentPage =
                    _userProfileSearch = automatedBase.QuickLaunch.NavigateToNewUserProfileSearch();
                var TestName = new StackFrame().GetMethod().Name;
                var paramLists = automatedBase.DataHelper.GetTestData(GetType().FullName, TestName);
                var expectedUserAccountStatus =
                    automatedBase.DataHelper.GetMappingData(GetType().FullName, "User_Account_Status").Values.ToList();
                var activeUserFromDB = _userProfileSearch.GetUserByActivestatus();
                var inactiveUserFromDB = _userProfileSearch.GetUserByInactivestatus();


                StringFormatter.PrintMessage("Verify Active Account status icon displayed");

                _userProfileSearch.SideBarPanelSearch.SelectDropDownListValueByLabel("User Account Status",
                    expectedUserAccountStatus[1]);
                _userProfileSearch.SideBarPanelSearch.SetInputFieldByLabel("User ID", activeUserFromDB[0]);
                _userProfileSearch.SideBarPanelSearch.ClickOnFindButton();
                _userProfileSearch.WaitForWorkingAjaxMessage();
                _userProfileSearch.IsActiveIconPresentInAllList().ShouldBeTrue("Active status Should be present");
                _userProfileSearch.GetGridViewSection.GetValueInGridByColRow(3)
                    .ShouldBeEqual(activeUserFromDB[0], "Active User count correct?");

                StringFormatter.PrintMessage("Verify Inactive Account status icon displayed");

                _userProfileSearch.SideBarPanelSearch.SelectDropDownListValueByLabel("User Account Status",
                    expectedUserAccountStatus[2]);
                _userProfileSearch.SideBarPanelSearch.SetInputFieldByLabel("User ID", inactiveUserFromDB[0]);
                _userProfileSearch.SideBarPanelSearch.ClickOnFindButton();
                _userProfileSearch.WaitForWorkingAjaxMessage();
                _userProfileSearch.IsInActiveIconPresentInAllList().ShouldBeTrue("Inactive status Should be present");
                _userProfileSearch.GetGridViewSection.GetValueInGridByColRow(3)
                    .ShouldBeEqual(inactiveUserFromDB[0], "Active User count correct?");
                _userProfileSearch.SideBarPanelSearch.ClickOnClearLink();

                StringFormatter.PrintMessage("Verify Frozen Account Status icon displayed?");

                _userProfileSearch.RevertUserToActiveStatus(paramLists["UserId"]);
                _userProfileSearch.UpdateUserStatusToFrozen(paramLists["UserId"]);
                _userProfileSearch.SideBarPanelSearch.SetInputFieldByLabel("User ID", paramLists["UserId"]);
                _userProfileSearch.SideBarPanelSearch.SelectDropDownListValueByLabel("User Account Status", "Frozen");
                _userProfileSearch.SideBarPanelSearch.ClickOnFindButton();
                _userProfileSearch.WaitForWorkingAjaxMessage();
                _userProfileSearch.GetGridViewSection.GetValueInGridByColRow(3)
                    .ShouldBeEqual(paramLists["UserId"], "user id for Frozen user value equal?");
                _userProfileSearch.IsFrozenIconPresentInAllList().ShouldBeTrue("Frozen icon Present?");
                _userProfileSearch.SideBarPanelSearch.ClickOnClearLink();
                _userProfileSearch.RevertFrozenstatusForUser(paramLists["UserId"]);

                StringFormatter.PrintMessage("Verify Locked Account Status icon displayed?");

                _userProfileSearch.UpdateUserToLockedStatus(paramLists["UserId"]);
                _userProfileSearch.SideBarPanelSearch.SetInputFieldByLabel("User ID", paramLists["UserId"]);
                _userProfileSearch.SideBarPanelSearch.SelectDropDownListValueByLabel("User Account Status", "Locked");
                _userProfileSearch.SideBarPanelSearch.ClickOnFindButton();
                _userProfileSearch.WaitForWorkingAjaxMessage();
                _userProfileSearch.GetGridViewSection.GetValueInGridByColRow(3)
                    .ShouldBeEqual(paramLists["UserId"], "user id for Locked user value equal?");
                _userProfileSearch.IsLockIconPresentInAllList().ShouldBeTrue("Locked icon Present?");
                _userProfileSearch.RevertUserLockedStatus(paramLists["UserId"]);
                _userProfileSearch.SideBarPanelSearch.ClickOnClearLink();

            }
        }

        //[Test]//TE-486
        //public void Verify_Navigation_To_User_Profile_Search_Page_On_clicking_On_UserName()
        //{
        //    TestExtensions.TestName = new StackFrame().GetMethod().Name;
        //    var paramLists = DataHelper.GetTestData(GetType().FullName, TestExtensions.TestName);
        //    StringFormatter.PrintMessage("Verify navigation to user profile on clicking on user name");
        //    _profileManager = _userProfileSearch.ClickonUserNameToNavigateProfileManagerPage(paramLists["FirstName"], paramLists["LastName"]);
        //    _profileManager.GetPageHeader().ShouldBeEqual(PageHeaderEnum.MyProfile.GetStringValue(), "Is Page Header User Profile ?");
        //    _profileManager.GetUserFirstName().ShouldBeEqual(paramLists["FirstName"], "Firstname should be equal.");
        //    _profileManager.GetUserLastName().ShouldBeEqual(paramLists["LastName"], "LastName should be equal");
        //    _profileManager.ClickCancelButton();
        //    CurrentPage.GetCurrentUrl().ShouldBeEqual(
        //        EnvironmentManager.Instance.ApplicationUrl + PageUrlEnum.UserProfileSearch.GetStringValue(),
        //        "Clicking on Cancel should return to New User Profile Search page.");



        //}

        [Test] //TE-485 +CAR-2861 + CAR-3004 [CAR-3011]+CV-8700
        public void Verify_New_UserProfile_Search_Filters()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                UserProfileSearchPage _userProfileSearch;
                LoginPage _login;
                automatedBase.CurrentPage =
                    _userProfileSearch = automatedBase.QuickLaunch.NavigateToNewUserProfileSearch();
                var TestName = new StackFrame().GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(GetType().FullName, TestName);
                var expectedUserType = automatedBase.DataHelper.GetMappingData(GetType().FullName, "User_Type").Values.ToList();
                var expectedUserAccountStatus = automatedBase.DataHelper
                    .GetMappingData(GetType().FullName, "User_Account_Status")
                    .Values.ToList();
                var filterOptionList =
                    automatedBase.DataHelper.GetMappingData(GetType().FullName, "Filter_Options_List").Values.ToList();

                var invalidEmails = paramLists["InvalidEmails"].Split(';').ToList();

                StringFormatter.PrintMessage("Verify Search Filter Labels");
                _userProfileSearch.SideBarPanelSearch.GetSearchFiltersList()
                    .ShouldCollectionBeEqual(filterOptionList, "Search Filters", true);
                StringFormatter.PrintMessage("Verify First Name Filter Option");
                ValidateInputFieldForDefaultValueAndInputLength(_userProfileSearch,filterOptionList[0], 100);
                StringFormatter.PrintMessage("Verify Last Name Filter Option");
                ValidateInputFieldForDefaultValueAndInputLength(_userProfileSearch,filterOptionList[1], 100);
                StringFormatter.PrintMessage("Verify User Id Filter Option");
                ValidateInputFieldForDefaultValueAndInputLength(_userProfileSearch, filterOptionList[2], 100);
                StringFormatter.PrintMessage("Verify User Type Filter Option");
                ValidateSingleDropDownForDefaultValueAndExpectedList(_userProfileSearch,filterOptionList[3], expectedUserType, false);
                StringFormatter.PrintMessage("Verify User Type Filter Option");
                ValidateSingleDropDownForDefaultValueAndExpectedList(_userProfileSearch,filterOptionList[5], expectedUserAccountStatus,
                    false);
                _userProfileSearch.SideBarPanelSearch.ClickOnClearLink();

                StringFormatter.PrintMessage(
                    "Verify Clear Link clear all the selected options and set back to default values");
                _userProfileSearch.SideBarPanelSearch.SetInputFieldByLabel("First Name", paramLists["FirstName"]);
                _userProfileSearch.SideBarPanelSearch.SetInputFieldByLabel("Last Name", paramLists["LastName"]);
                _userProfileSearch.SideBarPanelSearch.SetInputFieldByLabel("User ID", paramLists["UserId"]);
                _userProfileSearch.SideBarPanelSearch.SelectDropDownListValueByLabel("User Type", expectedUserType[1]);
                _userProfileSearch.SideBarPanelSearch.SetInputFieldByLabel("Email Address",
                    paramLists["EmailLowerCase"]);
                _userProfileSearch.SideBarPanelSearch.SelectDropDownListValueByLabel("User Account Status",
                    expectedUserAccountStatus[1]);
                _userProfileSearch.SideBarPanelSearch.ClickOnClearLink();
                ValidateDefaultValuesOfSearchFilters(_userProfileSearch);

                _userProfileSearch.SideBarPanelSearch.SetInputFieldByLabel("First Name", paramLists["FirstName"]);
                _userProfileSearch.SideBarPanelSearch.SetInputFieldByLabel("Last Name", paramLists["LastName"]);
                _userProfileSearch.SideBarPanelSearch.ClickOnFindButton();
                _userProfileSearch.WaitForWorkingAjaxMessage();
                _userProfileSearch.ClickOnUserFirstnameandlastname(
                    paramLists["FirstName"] + " " + paramLists["LastName"]);
                _userProfileSearch.IsUserSettingsFormPresent(true)
                    .ShouldBeTrue("Is User Setting Form displayed instead of popup");
                _userProfileSearch.GetWindowHandlesCount().ShouldBeEqual(1, "There should not any retro popup");

                //CAR - 3004[CAR - 3011]
                StringFormatter.PrintMessage("Verifying the Email Filter");
                automatedBase.CurrentPage.RefreshPage();
                _userProfileSearch.SideBarPanelSearch.OpenSidebarPanel();
                _userProfileSearch.SideBarPanelSearch.ClickOnClearLink();
                VerifyEmailAddressField(invalidEmails, filterOptionList[4], _userProfileSearch);

                StringFormatter.PrintMessage("Verify searching for a user by Email Address");
                _userProfileSearch.SideBarPanelSearch.SetInputFieldByLabel(filterOptionList[4],
                    paramLists["EmailLowerCase"]);
                _userProfileSearch.SideBarPanelSearch.ClickOnFindButton();
                _userProfileSearch.WaitForWorkingAjaxMessage();

                _userProfileSearch.GetGridViewSection.GetGridRowCount()
                    .ShouldBeEqual(1, "Searching by email address should return a unique user");
                _userProfileSearch.GetGridViewSection.GetValueInGridByColRow(3).ShouldBeEqual(paramLists["UserId"],
                    "The user returned after the search should be correct");

                StringFormatter.PrintMessage("Verify searching for a user by mixed case Email Address");
                _userProfileSearch.SideBarPanelSearch.SetInputFieldByLabel(filterOptionList[4],
                    "UIAUTOMATION_1@automationuser.com");
                _userProfileSearch.SideBarPanelSearch.ClickOnFindButton();
                _userProfileSearch.WaitForWorkingAjaxMessage();

                _userProfileSearch.GetGridViewSection.GetGridRowCount()
                    .ShouldBeEqual(1, "Searching by email address should return a unique user");
                _userProfileSearch.GetGridViewSection.GetValueInGridByColRow(3).ShouldBeEqual(paramLists["UserId2"],
                    "Search should yield user records with the matching email address value regardless of letter case");
            }
        }

        [Test] //TE-486
        public void Verify_Load_More_Icon_Functionality()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                UserProfileSearchPage _userProfileSearch;
                LoginPage _login;
                automatedBase.CurrentPage =
                    _userProfileSearch = automatedBase.QuickLaunch.NavigateToNewUserProfileSearch();
                automatedBase.CurrentPage.RefreshPage();
                _userProfileSearch.SideBarPanelSearch.SetInputFieldByLabel("First Name", "test");
                _userProfileSearch.SideBarPanelSearch.ClickOnFindButton();
                _userProfileSearch.WaitForWorkingAjaxMessage();
                var rowCount = _userProfileSearch.GetGridViewSection.GetGridRowCount();
                rowCount.ShouldBeEqual(25, "Maximum 25 results displayed initially?");
                _userProfileSearch.GetGridViewSection.IsLoadMorePresent().ShouldBeTrue("Is load more icon present?");

                var loadMoreValue = _userProfileSearch.GetLoadMoreText();
                var numbers = Regex.Split(loadMoreValue, @"\D+").Where(s => s != String.Empty)
                    .Select(m => int.Parse(m.Trim())).ToList();
                var scrollCount = numbers[1] / 25;

                for (int i = 1; i < scrollCount; i++)
                {
                    _userProfileSearch.GetGridViewSection.ClickLoadMore();
                    rowCount += 25;
                    _userProfileSearch.GetGridViewSection.GetGridRowCount()
                        .ShouldBeEqual(rowCount, "25 more results added?");
                }

                if (_userProfileSearch.IsLoadMoreLinkable())
                {
                    _userProfileSearch.GetGridViewSection.ClickLoadMore();
                    _userProfileSearch.GetGridViewSection.GetGridRowCount().ShouldBeEqual(numbers[1]);
                }

                _userProfileSearch.SideBarPanelSearch.ClickOnClearLink();
            }
        }

        [Test] //TE-485
        public void Verify_users_are_not_allowed_to_search_without_entering_firstname_lastname_or_userid()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                UserProfileSearchPage _userProfileSearch;
                LoginPage _login;
                automatedBase.CurrentPage =
                    _userProfileSearch = automatedBase.QuickLaunch.NavigateToNewUserProfileSearch();
                var TestName = new StackFrame().GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(GetType().FullName, TestName);
                StringFormatter.PrintMessageTitle(
                    "Verify Search cannot be completed without First Name, Last Name or User ID field");
                _userProfileSearch.SideBarPanelSearch.ClickOnFindButton();
                _userProfileSearch.WaitForWorkingAjaxMessage();
                _userProfileSearch.IsPageErrorPopupModalPresent()
                    .ShouldBeTrue(
                        "User should not be able to search without entering First Name, Last Name or User ID field");
                _userProfileSearch.GetPageErrorMessage().ShouldBeEqual("At least one search criteria is required.",
                    "Verify the popup message");
                _userProfileSearch.ClosePageError();

                StringFormatter.PrintMessageTitle("Verify Search can be completed with First Name only");
                VerifySearchCanBeCompleted(_userProfileSearch,"First Name", paramLists["FirstName"]);

                StringFormatter.PrintMessageTitle("Verify Search can be completed with Last Name only");
                VerifySearchCanBeCompleted(_userProfileSearch,"Last Name", paramLists["LastName"]);

                StringFormatter.PrintMessageTitle("Verify Search can be completed with User Id only");
                VerifySearchCanBeCompleted(_userProfileSearch,"User ID", paramLists["UserId"]);

                StringFormatter.PrintMessageTitle("Verify Search cannot be completed with user type only");
                ValidateFieldErrorMessageForComboBox(_userProfileSearch,"User Type", "Internal");

                StringFormatter.PrintMessageTitle("Verify Search cannot be completed with User Account Status only");
                ValidateFieldErrorMessageForComboBox(_userProfileSearch,"User Account Status", "Locked");
            }
        }

        //[Test] //TE-488
        //public void Verify_add_User_Popup_Functionality()
        //{
        //    try
        //    {
        //        _userProfileSearch.IsAddUserIconPresent().ShouldBeTrue("Add User Icon Present?");
        //        _userProfileSearch.ClickOnAddUserIcon();
        //        _userProfileSearch.IsCreateUserPopUpPresent().ShouldBeTrue("Is Create new user popup opened?");
        //        _userProfileSearch.IsNextButtonPresent().ShouldBeTrue("Next icon present?");
        //        _userProfileSearch.SwitchToNewUserSearchPage(true);
        //        CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.OldUserProfileSearch.GetStringValue());

        //    }
        //    finally
        //    {
        //        _userProfileSearch.CloseAnyPopupIfExist();
        //    }
        //}

        [Test] //TE-487
        public void Verify_Sorting_Options_In_New_User_Profile_Search()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                UserProfileSearchPage _userProfileSearch;
                LoginPage _login;
                automatedBase.CurrentPage =
                    _userProfileSearch = automatedBase.QuickLaunch.NavigateToNewUserProfileSearch();
                var TestName = new StackFrame(true).GetMethod().Name;
                var filterOptions =
                    automatedBase.DataHelper.GetMappingData(GetType().FullName, "User_Profile_Sorting_options").Values.ToList();
                var firstName = automatedBase.DataHelper.GetSingleTestData(GetType().FullName, TestName,
                    "FirstName",
                    "Value");
                try
                {
                    _userProfileSearch.GetGridViewSection.IsFilterOptionIconPresent()
                        .ShouldBeTrue("Filter Icon Option Should Be Present");
                    _userProfileSearch.GetGridViewSection.GetFilterOptionTooltip()
                        .ShouldBeEqual("Sort Results", "Correct tooltip is displayed");
                    _userProfileSearch.GetGridViewSection.GetFilterOptionList()
                        .ShouldCollectionBeEqual(filterOptions, "Filter Options Lists Collection Should Equal");
                    StringFormatter.PrintMessage("Verify default sorting");
                    _userProfileSearch.SideBarPanelSearch.SetInputFieldByLabel("First Name", firstName);
                    _userProfileSearch.SideBarPanelSearch.ClickOnFindButton();
                    _userProfileSearch.WaitForWorkingAjaxMessage();
                    _userProfileSearch.GetGridViewSection.GetGridRowCount()
                        .ShouldBeGreater(0, "Search Result Should Be Listed");
                    _userProfileSearch.GetGridViewSection.GetGridListValueByCol().IsInAscendingOrder()
                        .ShouldBeTrue("By Default Result Should Be Sorted by user name in ascending order");
                    _userProfileSearch.ClickOnFilterOptionListRow(1);
                    _userProfileSearch.GetGridViewSection.GetGridListValueByCol().IsInDescendingOrder()
                        .ShouldBeTrue("Search result must be sorted by User Name in Descending");
                    _userProfileSearch.ClickOnFilterOptionListRow(1);
                    _userProfileSearch.GetGridViewSection.GetGridListValueByCol().IsInAscendingOrder()
                        .ShouldBeTrue("Search result must be sorted by User Name in Ascending");
                    StringFormatter.PrintMessage("Verify other sort options");
                    VerifySortingOptionsInUserProfileSearch(_userProfileSearch,3, 2, filterOptions[1]);
                    VerifySortingOptionsInUserProfileSearch(_userProfileSearch,6, 3, filterOptions[2]);

                    _userProfileSearch.ClickOnClearSort();
                    _userProfileSearch.GetGridViewSection.GetGridListValueByCol().IsInAscendingOrder()
                        .ShouldBeTrue("Is default sort applied after clear sort ?");
                }
                finally
                {
                    _userProfileSearch.ClickOnClearSort();
                }
            }
        }

        [Test, Category("SmokeTest")] //CAR-2388(CAR-2145) + TANT-196
        public void Verify_User_Profile_Settings_container_tabs()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                UserProfileSearchPage _userProfileSearch;
                LoginPage _login;
                automatedBase.CurrentPage =
                    _userProfileSearch = automatedBase.QuickLaunch.NavigateToNewUserProfileSearch();
                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> testData = automatedBase.DataHelper.GetTestData(GetType().FullName,
                    TestName);
                var firstName = testData["FirstName"];
                var labelList = testData["ContainerList"].Split(';').ToList();
                var headerList = testData["HeaderList"].Split(';').ToList();
                _userProfileSearch.SideBarPanelSearch.SetInputFieldByLabel("First Name", firstName);
                _userProfileSearch.SideBarPanelSearch.ClickOnFindButton();
                _userProfileSearch.WaitForWorking();
                _userProfileSearch.GetGridViewSection.ClickOnGridRowByRow();
                _userProfileSearch.WaitForWorking();
                _userProfileSearch.GetUserSettingsContainerList().ShouldCollectionBeEqual(labelList,
                    "UserSettings Container should consist of following 6 containers.");
                _userProfileSearch.GetSideWindow.ClickOnEditIcon();

                for (var i = 0; i < labelList.Count; i++)
                {
                    _userProfileSearch.ClickUserSettingsContainerByLabel(labelList[i]);
                    _userProfileSearch.IsUserSettingsFormDisabled().ShouldBeFalse(
                        $"The '{labelList[i]}' form should be enabled once edit icon" +
                        $"is clicked in another tab");
                    _userProfileSearch.GetUserSettingsContainerTitle(i + 1).ShouldBeEqual(headerList[i],
                        $"{labelList[i]} section should have title {headerList[i]}.");
                }
            }
        }

        [Test] //CAR-2504(2151)
        public void Verify_Notifications_tab()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                UserProfileSearchPage _userProfileSearch;
                LoginPage _login;
                automatedBase.CurrentPage =
                    _userProfileSearch = automatedBase.QuickLaunch.NavigateToNewUserProfileSearch();
                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists = automatedBase.DataHelper.GetTestData(GetType().FullName,
                    TestName);
                var userId = paramLists["UserId"].Split(',').ToList();

                var notificationsLabelList = Enum.GetValues(typeof(NotificationsEnum)).Cast<NotificationsEnum>()
                    .Select(x => x.GetStringValue()).ToList();
                try
                {
                    _userProfileSearch.SearchUserByNameOrId(userId, true);
                    _userProfileSearch.GetGridViewSection.ClickOnGridRowByRow();
                    _userProfileSearch.ClickOnUserSettingTabByTabName("Notifications");
                    _userProfileSearch.IsUserSettingsFormDisabled()
                        .ShouldBeTrue("User settings form should be disabled.");
                    _userProfileSearch.GetSelectedUserSettingTab().ShouldBeEqual(
                        UserSettingsTabEnum.Notifications.GetStringValue(),
                        $"'{UserSettingsTabEnum.Notifications.GetStringValue()}' tab should be selected");

                    _userProfileSearch.GetNotificationslabelList()
                        .ShouldBeEqual(notificationsLabelList, "List should match");
                    foreach (var label in notificationsLabelList)
                    {
                        _userProfileSearch.IsRadioButtonPresentByLabel(label)
                            .ShouldBeTrue("Radio button should be present in very label");
                    }

                    StringFormatter.PrintMessage(
                        "All the radio buttons are set to their default value of NO for the user in case the test breaks in the middle");

                    if (_userProfileSearch.IsRadioButtonOnOffByLabel(notificationsLabelList[0]))
                    {
                        _userProfileSearch.ClickOnEditIcon();
                        for (int i = 0; i < notificationsLabelList.Count; i++)
                        {
                            _userProfileSearch.ClickOnRadioButtonByLabel(notificationsLabelList[i], false);
                        }

                        _userProfileSearch.GetSideWindow.Save(waitForWorkingMessage: true);
                    }

                    //Test starts here

                    _userProfileSearch.ClickOnEditIcon();
                    foreach (var label in notificationsLabelList)
                    {
                        _userProfileSearch.ClickOnRadioButtonByLabel(label);
                    }

                    _userProfileSearch.GetSideWindow.Cancel();
                    foreach (var label in notificationsLabelList)
                    {
                        _userProfileSearch.IsRadioButtonOnOffByLabel(label, false)
                            .ShouldBeTrue("Changes should not be saved");
                    }

                    _userProfileSearch.ClickOnEditIcon();
                    foreach (var label in notificationsLabelList)
                    {
                        _userProfileSearch.ClickOnRadioButtonByLabel(label);
                    }

                    _userProfileSearch.GetSideWindow.Save(waitForWorkingMessage: true);

                    foreach (var label in notificationsLabelList)
                    {
                        _userProfileSearch.IsRadioButtonOnOffByLabel(label).ShouldBeTrue("Changes should be saved");
                    }
                }

                finally
                {
                    StringFormatter.PrintMessage(
                        "All the radio buttons are set to their default value of NO for the user ");

                    if (_userProfileSearch.IsRadioButtonOnOffByLabel(notificationsLabelList[0]))
                    {
                        _userProfileSearch.ClickOnEditIcon();
                        for (int i = 0; i < notificationsLabelList.Count; i++)
                        {
                            _userProfileSearch.ClickOnRadioButtonByLabel(notificationsLabelList[i], false);
                        }

                        _userProfileSearch.GetSideWindow.Save(waitForWorkingMessage: true);
                    }
                }

            }
        }

        [Test] //CAR-2150
        public void Verify_new_user_settings_management_client_assignments()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                UserProfileSearchPage _userProfileSearch;
                LoginPage _login;
                automatedBase.CurrentPage =
                    _userProfileSearch = automatedBase.QuickLaunch.NavigateToNewUserProfileSearch();
                var TestName = new StackFrame(true).GetMethod().Name;
                var userId = automatedBase.DataHelper.GetSingleTestData(GetType().FullName, TestName, "UserId",
                    "Value");
                var clientList = automatedBase.DataHelper.GetSingleTestData(GetType().FullName, TestName,
                    "ClientList",
                    "Value").Split(';').ToList();
                List<string> userAssignedClientList = _userProfileSearch.GetUserAssignedClientList(userId);
                List<string> availableClientList =
                    _userProfileSearch.GetCommonSql.GetClientFromDatabaseByClientStatus("T");
                List<string> totalClientList = new List<string>();


                totalClientList = availableClientList;
                availableClientList = availableClientList.Except(userAssignedClientList).ToList();
                availableClientList.Sort();
                totalClientList.Sort();

                try
                {
                    _userProfileSearch.IsRoleAssigned<UserProfileSearchPage>(new List<string> {userId},
                        RoleEnum.NucleusAdmin.GetStringValue(), isDefaultPageUserProfile: true).ShouldBeTrue(
                        $"Is User Maintenance present for current user<{userId}>");


                    _userProfileSearch.SideBarPanelSearch.OpenSidebarPanel();
                    _userProfileSearch.SideBarPanelSearch.SetInputFieldByLabel("User ID", userId);
                    _userProfileSearch.SideBarPanelSearch.ClickOnFindButton();
                    _userProfileSearch.GetGridViewSection.ClickOnGridRowByRow();
                    _userProfileSearch.ClickOnUserSettingTabByTabName(UserSettingsTabEnum.Clients.GetStringValue());
                    _userProfileSearch.GetClientListLabel().ShouldBeEqual("Available Clients",
                        "Available clients should be shown in the left container.");
                    _userProfileSearch.GetClientListLabel("right_list").ShouldBeEqual("Assigned Clients",
                        "Assigned clients should be shown in the right container.");
                    _userProfileSearch.GetAvailableAssignedList().ShouldCollectionBeEqual(availableClientList,
                        "Available clients should be shown in the section with Available Clients.");
                    _userProfileSearch.GetAvailableAssignedList().IsInAscendingOrder()
                        .ShouldBeTrue("Available Client List should be sorted in ascending order");
                    _userProfileSearch.GetAvailableAssignedList(2, false).ShouldCollectionBeEqual(
                        userAssignedClientList,
                        "Assigned clients should be shown in the section with Assigned Clients.");
                    _userProfileSearch.IsEditIconEnabled("Clients").ShouldBeTrue(
                        "Users with User Maintenance(read/write) authority should be able to modify client assignments.");
                    foreach (string client in clientList)
                    {
                        _userProfileSearch.ClickEditIconSettings("Clients");
                        _userProfileSearch.ClickOnAvailableAssignedRow(2, client);
                        _userProfileSearch.SetAndGetLengthOfExtUserIdByClientName(client).ShouldBeEqual(32,
                            "Maximum number of characters that are allowed in the Ext User Id is 32.");
                        _userProfileSearch.GetSideWindow.Save(waitForWorkingMessage: true);
                        _userProfileSearch.RefreshPage();
                        _userProfileSearch.SideBarPanelSearch.SetInputFieldByLabel("User ID", userId);
                        _userProfileSearch.SideBarPanelSearch.ClickOnFindButton();
                        _userProfileSearch.GetGridViewSection.ClickOnGridRowByRow();
                        _userProfileSearch.ClickOnUserSettingTabByTabName(UserSettingsTabEnum.Clients.GetStringValue());
                        if (!userAssignedClientList.Contains(client))
                        {
                            userAssignedClientList.Add(client);
                        }

                        userAssignedClientList.Sort();
                        _userProfileSearch.GetAvailableAssignedList(2, false).ShouldCollectionBeEqual(
                            userAssignedClientList,
                            "Assigned clients should be shown in the section with Assigned Clients.");
                    }

                    _userProfileSearch.Logout().LoginAsHciUserWithManageEditAuthority()
                        .NavigateToNewUserProfileSearch();
                    _userProfileSearch.ClickOnSwitchClient().GetSwitchClientList()
                        .ShouldCollectionBeEqual(userAssignedClientList,
                            "Assigned clients should be shown in the switch client list.");
                    _userProfileSearch.ClickOnSwitchClient();
                    _userProfileSearch.SideBarPanelSearch.SetInputFieldByLabel("User ID", userId);
                    _userProfileSearch.SideBarPanelSearch.ClickOnFindButton();
                    _userProfileSearch.GetGridViewSection.ClickOnGridRowByRow();
                    _userProfileSearch.ClickOnUserSettingTabByTabName(UserSettingsTabEnum.Clients.GetStringValue());
                    _userProfileSearch.ClickEditIconSettings("Clients");
                    _userProfileSearch.SelectDeselectAll("Available Clients");
                    _userProfileSearch.GetAvailableAssignedList(2, false).ShouldCollectionBeEqual(totalClientList,
                        "Every available clients should be shown in the section with Assigned Clients.");
                    _userProfileSearch.SelectDeselectAll("Assigned Clients");
                    _userProfileSearch.GetAvailableAssignedList().ShouldCollectionBeEqual(availableClientList,
                        "Available clients should be shown in the section with Available Clients.");
                    _userProfileSearch.GetAvailableAssignedList(2, false)[0].ShouldCollectionBeEqual(
                        _userProfileSearch.GetDefaultClientOfUser(userId),
                        "Deselecting All should move all client to Available Client box except for default Client");
                    _userProfileSearch.GetSideWindow.Save(waitForWorkingMessage: true);
                }
                finally
                {
                    _userProfileSearch.DeleteAssignedClient(userId, clientList);
                    _userProfileSearch.SideBarPanelSearch.ClickOnClearLink();
                }
            }
        }

        [Test] //CAR-2148+TE-960
        public void Verify_new_user_settings_management_security_settings()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                UserProfileSearchPage _userProfileSearch;
                LoginPage _login;
                automatedBase.CurrentPage =
                    _userProfileSearch = automatedBase.QuickLaunch.NavigateToNewUserProfileSearch();
                var TestName = new StackFrame(true).GetMethod().Name;
                var userId = automatedBase.DataHelper.GetSingleTestData(GetType().FullName, TestName, "UserId",
                    "Value");
                var changedPasswordList = automatedBase.DataHelper.GetSingleTestData(GetType().FullName, TestName,
                    "ChangedPasswordList",
                    "Value").Split(';').ToList();
                var questionList = automatedBase.DataHelper.GetSingleTestData(GetType().FullName, TestName,
                    "QuestionList",
                    "Value").Split(';').ToList();
                var answerList = automatedBase.DataHelper.GetSingleTestData(GetType().FullName, TestName,
                    "AnswerList",
                    "Value").Split(';').ToList();
                var newPassword = automatedBase.DataHelper.GetSingleTestData(GetType().FullName, TestName,
                    "NewPassword",
                    "Value");

                string fullName = _userProfileSearch.GetFullNameByUserId("uiautomation");

                try
                {
                    StringFormatter.PrintMessageTitle(
                        "Verifying whether the user with read/write privilege to User Maintenance will see the edit icon");
                    _userProfileSearch.SearchUserByNameOrId(new List<string>() {userId}, true);
                    _userProfileSearch.ClickOnGridRowByUserIdToOpenUserSettingSideView(userId);

                    _userProfileSearch.ClickOnUserSettingTabByTabName(UserSettingsTabEnum.Security.GetStringValue());
                    _userProfileSearch.GetSideWindow.IsEditIconDisabled().ShouldBeFalse(
                        "Users with User Maintenance(read/write) authority should be able to modify Security settings.");

                    StringFormatter.PrintMessageTitle(
                        "Verifying whether the user audit increases each time a new password is set");
                    int countOfUserAudits = 0;
                    foreach (string password in changedPasswordList)
                    {
                        _userProfileSearch.ClickOnEditIcon();
                        _userProfileSearch.SetInputTextBoxValueByLabel("New Password", password);
                        _userProfileSearch.SetInputTextBoxValueByLabel("Confirm New Password", password);
                        _userProfileSearch.GetSideWindow.Save(waitForWorkingMessage: true);

                        countOfUserAudits++;
                        _userProfileSearch.GetLastSavedPasswordByUser(userId).ShouldBeEqual(countOfUserAudits,
                            "Audit record should be created for password change.");
                    }

                    StringFormatter.PrintMessage(
                        $"Form validation for the '{SecurityEnum.SecurityQuestions.GetStringValue()}' input fields");
                    _userProfileSearch.ClickOnEditIcon();

                    var questionListsFromDb = _userProfileSearch.GetAvailabeQuestionListFromDb();

                    var allQuestionsInSecurityQ1Dropdown =
                        _userProfileSearch.GetDropDownListForUserProfileSettingsByLabel(SecurityEnum.SecurityQuestion1
                            .GetStringValue());

                    //TE-960
                    allQuestionsInSecurityQ1Dropdown.CollectionShouldBeSubsetOf(questionListsFromDb,
                        "Questions list should be correct");

                    _userProfileSearch.SelectDropDownListValueByLabel(SecurityEnum.SecurityQuestion1.GetStringValue(),
                        questionList[0]);
                    _userProfileSearch.GetDropDownListForUserProfileSettingsByLabel(SecurityEnum.SecurityQuestion2
                        .GetStringValue()).ShouldNotContain(questionList[0],
                        "Questions list in Security Question 2 should not contain question already selected in Security Question 1");
                    _userProfileSearch.ValidateAnswerInputBoxByLabel(SecurityEnum.SecurityAnswer1.GetStringValue())
                        .ShouldBeEqual(100,
                            "Answer Text Field should allow up to 100 alphanumeric or special character values.");

                    _userProfileSearch.SetInputTextBoxValueByLabel(SecurityEnum.SecurityAnswer1.GetStringValue(),
                        answerList[0]);
                    _userProfileSearch.ClickOnShowAnswer();
                    _userProfileSearch.GetInputTextBoxValueByLabel(SecurityEnum.SecurityAnswer1.GetStringValue())
                        .ShouldBeEqual(answerList[0],
                            "Answer should be shown after clicking the Show Answer link.");
                    questionListsFromDb.Remove(questionList[0]);
                    var allQuestionsInSecurityQ2Dropdown =
                        _userProfileSearch.GetDropDownListForUserProfileSettingsByLabel(SecurityEnum.SecurityQuestion2
                            .GetStringValue());

                    allQuestionsInSecurityQ2Dropdown.ShouldCollectionBeEquivalent(questionListsFromDb,
                        "Questions list should be correct");

                    _userProfileSearch.SelectDropDownListValueByLabel(SecurityEnum.SecurityQuestion2.GetStringValue(),
                        questionList[1]);
                    _userProfileSearch.GetDropDownListForUserProfileSettingsByLabel(SecurityEnum.SecurityQuestion1
                        .GetStringValue()).ShouldNotContain(questionList[1],
                        "Questions list in Security Question 1 should not contain question already selected in Security Question 2");
                    _userProfileSearch.ValidateAnswerInputBoxByLabel(SecurityEnum.SecurityAnswer2.GetStringValue())
                        .ShouldBeEqual(100,
                            "Answer Text Field should allow up to 100 alphanumeric or special character values.");

                    _userProfileSearch.SetInputTextBoxValueByLabel(SecurityEnum.SecurityAnswer2.GetStringValue(),
                        answerList[1]);
                    _userProfileSearch.ClickOnShowAnswer(4);
                    _userProfileSearch.GetInputTextBoxValueByLabel(SecurityEnum.SecurityAnswer2.GetStringValue())
                        .ShouldBeEqual(answerList[1],
                            "Answer should be shown after clicking the Show Answer link.");

                    StringFormatter.PrintMessage($"Validation of the '{SecurityEnum.ChangePassword.GetStringValue()}'");
                    _userProfileSearch.GetInfoHelpTooltipByLabel(SecurityEnum.NewPassword.GetStringValue())
                        .ShouldBeEqual(
                            "Cannot use one of the last 10 saved passwords. Selected password must contain: at least 8 characters, at least 1 special character, at least 1 numeric character, at least 1 uppercase character, at least 1 lowercase character.");
                    _userProfileSearch.SetInputTextBoxValueByLabel(SecurityEnum.NewPassword.GetStringValue(), "test");
                    _userProfileSearch.GetSideWindow.Save();
                    _userProfileSearch.IsPageErrorPopupModalPresent().ShouldBeTrue("Error messages should be shown.");
                    _userProfileSearch.GetPageErrorMessage().ShouldBeEqual(
                        "Password must contain:\r\n- at least 8 characters\r\n- at least 1 special character\r\n- at least 1 numeric character\r\n- at least 1 uppercase character\r\n- at least 1 lowercase character",
                        "Error messages should be shown if the password doesn't meet the required criteria.");
                    _userProfileSearch.ClosePageError();


                    StringFormatter.PrintMessageTitle(
                        "Verifying that the user is not allowed to input new password which is within the recent 10 last passwords");
                    Random random = new Random();
                    int n = random.Next(0, changedPasswordList.Count);

                    _userProfileSearch.SetInputTextBoxValueByLabel(SecurityEnum.NewPassword.GetStringValue(),
                        changedPasswordList[n]);
                    _userProfileSearch.SetInputTextBoxValueByLabel(SecurityEnum.ConfirmNewPassword.GetStringValue(),
                        changedPasswordList[n]);
                    _userProfileSearch.GetSideWindow.Save();
                    _userProfileSearch.IsPageErrorPopupModalPresent().ShouldBeTrue("Error messages should be shown.");
                    _userProfileSearch.GetPageErrorMessage().ShouldBeEqual(
                        "Password must not match any of the last 10 saved passwords.",
                        "Error message should be shown if the new password matches last 10 passwords.");
                    _userProfileSearch.ClosePageError();

                    StringFormatter.PrintMessage("Verifying saving a valid password");
                    _userProfileSearch.SetInputTextBoxValueByLabel(SecurityEnum.NewPassword.GetStringValue(),
                        newPassword);
                    _userProfileSearch.SetInputTextBoxValueByLabel(SecurityEnum.ConfirmNewPassword.GetStringValue(),
                        newPassword);
                    _userProfileSearch.GetSideWindow.Save();
                    _userProfileSearch.GetLastModifiedBy()
                        .ShouldBeEqual(fullName + " " + DateTime.Now.ToString("MM/dd/yyyy"),
                            "Changes made should be created in the audit record.");
                    _userProfileSearch.Logout().LoginAsHciUserForSecurityCheck().NavigateToNewUserProfileSearch();
                }

                finally
                {
                    _userProfileSearch.DeleteLastSavedPasswordByUser(userId);

                    if (_userProfileSearch.IsPageErrorPopupModalPresent())
                    {
                        _userProfileSearch.ClosePageError();
                    }

                    if (!_userProfileSearch.SideBarPanelSearch.IsSideBarPanelOpen())
                    {
                        _userProfileSearch.SideBarPanelSearch.ClickOnToggleSidebarPanelButton();
                        _userProfileSearch.SideBarPanelSearch.ClickOnClearLink();
                    }
                }
            }
        }

        [Test] //TE-987
        public void Verify_Disabled_Fields_For_SSO_User()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                UserProfileSearchPage _userProfileSearch;
                LoginPage _login;
                automatedBase.CurrentPage =
                    _userProfileSearch = automatedBase.QuickLaunch.NavigateToNewUserProfileSearch();
                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> testData = automatedBase.DataHelper.GetTestData(GetType().FullName,
                    TestName);
                var user_id = testData["UserID"];
                _userProfileSearch.SearchUserByNameOrId(new List<string>() {user_id}, true);
                _userProfileSearch.ClickOnGridRowByUserIdToOpenUserSettingSideView(user_id);
                var fields = testData["Fields"].Split(',').ToList();

                if (_userProfileSearch.IsRadioButtonOnOffByLabel("SSO User"))
                {
                    _userProfileSearch.ClickOnEditIcon();
                    _userProfileSearch.GetHeaderInfoHelpToolTipByLabel(UserSettingsTabEnum.Profile.GetStringValue())
                        .ShouldBeEqual("The disabled fields are managed by the organization portal");
                    _userProfileSearch.IsUserSettingsTabDisabled(UserSettingsTabEnum.Security.GetStringValue())
                        .ShouldBeTrue("settings tab should be true for sso user");
                    _userProfileSearch.GetTabInfoHelpToolTipByLabel(UserSettingsTabEnum.Security.ToString())
                        .ShouldBeEqual(
                            "The password for this user will be handled through the organization Identity Provider");
                    foreach (var field in fields)
                    {
                        _userProfileSearch.IsInputFieldByLabelDisabled(field)
                            .ShouldBeTrue($"{field} should be disabled for SSO User");
                    }
                }



            }
        }

        [Test,Category("Acceptance")]//CV-8700
        public void Verify_functionality_For_User_With_Longer_User_Id()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun(UserLoginIndex: 97))
            {
                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> testData = automatedBase.DataHelper.GetTestData(GetType().FullName,
                    TestName);
                LoginPage _login = automatedBase.Login;
                var email = testData["EmailID"];
                var job = testData["JobTitle"].Split(',').ToList();
                var name = testData["Username"];
                automatedBase.CurrentPage = _login.LoginAsJITUser();
                UserProfileSearchPage _userProfile = null;
                try
                {
                    StringFormatter.PrintMessage("Verify no error occurs while updating user profile for users that has long user id");
                    _userProfile=automatedBase.CurrentPage.SearchByUserIdToNavigateToUserSettingsForm(email);
                    _userProfile.GetSideWindow.ClickOnEditIcon();
                    _userProfile.SetInputTextBoxValueByLabel(
                        UserProfileEnum.JobTitle.GetStringValue(), job[0]);
                    _userProfile.GetSideWindow.Save();
                    _userProfile.GetLastModifiedBy().ShouldBeEqual($"{name} {DateTime.Now.Date.ToString("MM/dd/yyyy")}","modified value correct?");
                }
                finally
                {
                    _userProfile.GetSideWindow.ClickOnEditIcon();
                    _userProfile.SetInputTextBoxValueByLabel(
                        UserProfileEnum.JobTitle.GetStringValue(), job[1]);
                    _userProfile.GetSideWindow.Save();
                }
                
            }
        }

        #endregion

        #region private methods

        private void VerifyEmailAddressField(List<string> invalidEmailAddresses, string label,UserProfileSearchPage _userProfileSearch)
        {
            foreach (var email in invalidEmailAddresses)
            {
                _userProfileSearch.SideBarPanelSearch.SetInputFieldByLabel(label, email);
                _userProfileSearch.SideBarPanelSearch.ClickOnFindButton();
                _userProfileSearch.IsInvalidInputPresentByLabel(label)
                    .ShouldBeTrue("Input field should be highlighted red on performing search by invalid email address");
            }
        }

        private void FillProfileTab(UserProfileSearchPage _userProfileSearch,List<List<string>> profileList = null, bool ssoUser = false)
        {
            var data = new List<List<string>>(_profileDetailInfoList);
            if (profileList != null)
                data = new List<List<string>>(profileList);
            if (ssoUser)
            {
                data[1][5] = "uiautomation_TestSSO";
                data[1][4] = "cotivitiSSO@cotiviti.com";
            }

            for (var i = 0; i < data[0].Count; i++)
            {
                if (i == 6 || i == 7)
                {
                    _userProfileSearch.GetSideWindow.SelectDropDownValue(data[0][i], data[1][i]);
                    continue;
                }
                if (i == 3)
                {
                    _userProfileSearch.SetExtNo(data[1][i]);
                    continue;
                }

                if (i == 8 || i == 9)
                {
                    if(!ssoUser)
                        _userProfileSearch.GetSideWindow.SetInputInInputFieldByLabel(data[0][i], data[1][i]);
                    continue;
                }
                _userProfileSearch.GetSideWindow.SetInputInInputFieldByLabel(data[0][i], data[1][i]);
            }
        }

        private List<string> GetValueListOfProfileTab(UserProfileSearchPage _userProfileSearch)
        {
            var valueList = new List<string>();
            var data = new List<List<string>>(_profileDetailInfoList);

            for (var i = 0; i < data[0].Count; i++)
            {
                valueList.Add(i == 3
                    ? _userProfileSearch.GetExtNo()
                    : _userProfileSearch.GetSideWindow.GetInputValueByLabel(data[0][i]));
            }

            return valueList;

        }
        private void ValidateLabelsInGrid(UserProfileSearchPage _userProfileSearch)
        {
            _userProfileSearch.GetGridViewSection.IsLabelPresentByCol(2).ShouldBeFalse("Is Label present for Username?");
            _userProfileSearch.GetGridViewSection.IsLabelPresentByCol(3).ShouldBeFalse("Is Label present for User_Id?");
            _userProfileSearch.GetGridViewSection.IsLabelPresentByCol(4).ShouldBeTrue("Is Label Present for Phone Number?");
            _userProfileSearch.GetGridViewSection.GetLabelInGridByColRow(4).ShouldBeEqual("Phone:");
            _userProfileSearch.GetGridViewSection.IsLabelPresentByCol(5).ShouldBeFalse("Is Label present for Email?");
            _userProfileSearch.GetGridViewSection.IsLabelPresentByCol(6).ShouldBeFalse("Is Label present for User Type?");
        }

        private void ValidateDefaultValuesOfSearchFilters(UserProfileSearchPage _userProfileSearch)
        {
            _userProfileSearch.SideBarPanelSearch.GetInputValueByLabel("First Name")
                .ShouldBeNullorEmpty("First Name Should be set to default value");
            _userProfileSearch.SideBarPanelSearch.GetInputValueByLabel("Last Name")
                .ShouldBeNullorEmpty("Last Name Should be set to default value");
            _userProfileSearch.SideBarPanelSearch.GetInputValueByLabel("User ID")
                .ShouldBeNullorEmpty("User ID Should be set to default value");
            _userProfileSearch.SideBarPanelSearch.GetInputValueByLabel("User Type")
                .ShouldBeEqual("All", "Client Status should default to all");
            _userProfileSearch.SideBarPanelSearch.GetInputValueByLabel("Email Address")
                .ShouldBeNullorEmpty("Email Address should be set to default value");
            _userProfileSearch.SideBarPanelSearch.GetInputValueByLabel("User Account Status")
                .ShouldBeEqual("All", "User Account Status should default to all");
        }

        private void ValidateSingleDropDownForDefaultValueAndExpectedList(UserProfileSearchPage _userProfileSearch, string label, IList<string> collectionToEqual, 
            bool order = true)
        {
            var actualDropDownList = _userProfileSearch.SideBarPanelSearch.GetAvailableDropDownList(label);
            actualDropDownList.Count.ShouldBeGreater(0, "List of " + label + " is greater than zero.");
            if (collectionToEqual != null)
                actualDropDownList.ShouldCollectionBeEqual(collectionToEqual, label + " List As Expected");
            if (order)
            {
                actualDropDownList.Remove("All");
                actualDropDownList.IsInAscendingOrder()
                    .ShouldBeTrue(label + " should be sorted in alphabetical order.");
            }
            _userProfileSearch.SideBarPanelSearch.GetInputValueByLabel(label).ShouldBeEqual("All", label + " value defaults to All");
            _userProfileSearch.SideBarPanelSearch.SelectDropDownListValueByLabel(label, actualDropDownList[0], false); //check for type ahead functionality
            _userProfileSearch.SideBarPanelSearch.SelectDropDownListValueByLabel(label, actualDropDownList[1]);
            _userProfileSearch.SideBarPanelSearch.GetInputValueByLabel(label).ShouldBeEqual(actualDropDownList[1], "User can select only a single option");
            _userProfileSearch.SideBarPanelSearch.SelectDropDownListValueByLabel(label, "All");
        }
          
        private void ValidateInputFieldForDefaultValueAndInputLength(UserProfileSearchPage _userProfileSearch,string label,int allowedCharacters)
        {
            _userProfileSearch.SideBarPanelSearch.GetInputValueByLabel(label).ShouldBeNullorEmpty(string.Format("By Default {0} should be empty",label));
            _userProfileSearch.SideBarPanelSearch.SetInputFieldByLabel(label, string.Concat(Enumerable.Repeat("a1@_A", 21)));
            _userProfileSearch.SideBarPanelSearch.GetLengthOfTheInputFieldByLabel(label).ShouldBeEqual(allowedCharacters, string.Format("Verification of Maximum length of {0} Filter",label));
        }

        public void ValidateResultForInputStatus(UserProfileSearchPage _userProfileSearch,int count, string filterOptions)
        {
            if (count > 0)
            {
                _userProfileSearch.GetGridViewSection.GetGridRowCount()
                    .ShouldBeEqual(count, filterOptions + "User count correct ? ");
            }
            else
            {
                _userProfileSearch.SideBarPanelSearch.IsNoMatchingRecordFoundMessagePresent().ShouldBeTrue("No data found");
            }
        }

        private void ValidateFieldErrorMessageForComboBox(UserProfileSearchPage _userProfileSearch,string fieldName , string value)
        {
            _userProfileSearch.SideBarPanelSearch.SelectDropDownListValueByLabel(fieldName, value);
            _userProfileSearch.GetInvalidInputToolTipByLabel(fieldName).
                ShouldBeEqual(string.Format("Search cannot be initiated with {0} only. First Name, Last Name or User ID is required", fieldName));
            _userProfileSearch.SideBarPanelSearch.ClickOnFindButton();
            _userProfileSearch.WaitForWorkingAjaxMessage();
            _userProfileSearch.IsPageErrorPopupModalPresent()
                .ShouldBeTrue("User should not be able to search without entering First Name, Last Name or User ID field");
            _userProfileSearch.GetPageErrorMessage().ShouldBeEqual("At least one search criteria is required.",
                "Verify the popup message");
            _userProfileSearch.ClosePageError();
            _userProfileSearch.SideBarPanelSearch.ClickOnClearLink();
        }

        private void VerifySearchCanBeCompleted(UserProfileSearchPage _userProfileSearch,string fieldName,string value)
        {
            _userProfileSearch.SideBarPanelSearch.SetInputFieldByLabel(fieldName, value);
            _userProfileSearch.SideBarPanelSearch.ClickOnFindButton();
            _userProfileSearch.WaitForWorkingAjaxMessage();
            _userProfileSearch.IsPageErrorPopupModalPresent()
                .ShouldBeFalse("User should be able to search");
            _userProfileSearch.GetGridViewSection.GetGridRowCount().ShouldBeGreater(0, "Search should return data");
            _userProfileSearch.SideBarPanelSearch.ClickOnClearLink();
        }

        private void VerifySortingOptionsInUserProfileSearch(UserProfileSearchPage _userProfileSearch,int col, int option, string sortoption)
        {
            StringFormatter.PrintMessageTitle("Validation of Sorting");
            _userProfileSearch.ClickOnFilterOptionListRow(option);
            _userProfileSearch.GetGridViewSection.GetGridListValueByCol(col).IsInAscendingOrder()
                .ShouldBeTrue($"Search result must be sorted by {sortoption} in Ascending");
            _userProfileSearch.ClickOnFilterOptionListRow(option);
            _userProfileSearch.GetGridViewSection.GetGridListValueByCol(col).IsInDescendingOrder()
                .ShouldBeTrue($"Search result must be sorted by {sortoption} in Descending");
        }

        private void Fill_User_Profile_Clients_Roles(UserProfileSearchPage _userProfileSearch,List<string> clientList, List<string> roleList, List<string> restrictionList, string defaultPage, List<string> notificationsList, List<string> notificationsValueList, bool SSOUser = false)
        {
            var i = 0;
            _userProfileSearch.NavigateToCreateNewUser();
            if (_userProfileSearch.SideBarPanelSearch.IsSideBarPanelOpen())
            {
                _userProfileSearch.SideBarPanelSearch.ClickOnSideBarPanelIcon();
            }
            FillProfileTab(_userProfileSearch,ssoUser: SSOUser);
            _userProfileSearch.ClickOnNextButton();

            foreach (var client in clientList)
            {
                _userProfileSearch.ClickOnAvailableAssignedRow(2, client);
            }
            _userProfileSearch.GetSideWindow.SelectDropDownValue("Default Client", clientList[0]);

            foreach (var restriction in restrictionList)
            {
                _userProfileSearch.ClickOnAvailableAssignedRow(3, restriction);
            }
            _userProfileSearch.ClickOnNextButton();
            foreach (var role in roleList)
            {
                _userProfileSearch.ClickOnAvailableAssignedRow(1, role);
            }
            _userProfileSearch.GetSideWindow.SelectDropDownValue("Default Page", defaultPage);
            foreach (var notification in notificationsList)
            {
                _userProfileSearch.ClickOnRadioButtonByLabel(notification, bool.Parse(notificationsValueList[i]));
                i++;
            }
            _userProfileSearch.ClickOnNextButton();
        }

        private List<string> GetAssignedRoles(UserProfileSearchPage _userProfileSearch)
        {
            return _userProfileSearch.GetAllAssignedRolesList();
        }

        private void Verify_Roles_And_Notifications_Values_Are_Saved(UserProfileSearchPage _userProfileSearch,List<string> roleList, string defaultPage, List<string> notificationsLabelList, List<string> notificationsRadioValue)
        {
            GetAssignedRoles(_userProfileSearch).ShouldCollectionBeEqual(roleList, "Assigned Role values must be saved while clicking the previous button.");
            _userProfileSearch.GetSideWindow.GetDropDownListDefaultValue("Default Page").ShouldBeEqual(defaultPage, "Default Page value should be saved.");
            var i = 0;
            foreach (var label in notificationsLabelList)
            {
                _userProfileSearch.IsRadioButtonPresentByLabel(label)
                    .ShouldBeTrue("Radio button should be present in very label");
                _userProfileSearch.IsRadioButtonOnOffByLabel(label).ShouldBeEqual(bool.Parse(notificationsRadioValue[i]), "Notification values should be saved.");
                i++;
            }
        }

        #endregion
    }
}


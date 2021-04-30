using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using Nucleus.Service.Data;
using Nucleus.Service.PageServices.Login;
using Nucleus.Service.PageServices.QuickLaunch;
using Nucleus.Service.PageServices.Settings.User;
using Nucleus.Service.Support.Common.Constants;
using Nucleus.Service.Support.Enum;
using Nucleus.UIAutomation.TestSuites.Base;
using Nucleus.UIAutomation.TestSuites.Common;
using NUnit.Framework;
using UIAutomation.Framework.Utils;
using static System.String;

namespace Nucleus.UIAutomation.TestSuites.TestCases.ClientUser
{
    public class MyProfileClient : AutomatedBaseClient
    {
        #region Private

        private MyProfilePage _myProfilePage;
        private CommonValidations _commonValidations;
        private LoginPage _login;
        private QuickLaunchPage _quickLaunch;
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
                CurrentPage = _myProfilePage = CurrentPage.NavigateToMyProfilePage();
                //CurrentPage = _myProfilePage = CurrentPage.NavigateToMyProfilePageViaUrl();

                _commonValidations = new CommonValidations(CurrentPage);
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
            _myProfilePage.CloseDatabaseConnection();
            base.TestInit();
        }

        protected override void ClassCleanUp()
        {
            base.ClassCleanUp();
        }

        protected override void TestCleanUp()
        {
            base.TestCleanUp();
            if (Compare(UserType.CurrentUserType, UserType.CLIENT, StringComparison.OrdinalIgnoreCase) != 0)
            {
                _myProfilePage = _myProfilePage.Logout().LoginAsClientUser().NavigateToMyProfilePage();
            }

            if (_myProfilePage.GetPageHeader() != Extensions.GetStringValue(PageHeaderEnum.CoreMyProfile))
            {
                _myProfilePage.ClickOnQuickLaunch().NavigateToMyProfilePage();
            }
        }
        #endregion

        #region TestSuites

        [Test, Category("SmokeTestDeployment")] //TANT-257 + CAR-2906 [CAR-2910]
        public void Verify_my_profile_has_dashboard_option_for_client_users()
        {
            var fullName = CurrentPage.GetLoggedInUserFullName();
            var dropdownLabel = "Default Dashboard";
            var dashboardOptions = new List<string>
                {"Coding Validation", "Coordination of Benefits", "FraudFinder Pro"};
            _myProfilePage.IsDropDownPresentByLabel(dropdownLabel)
                .ShouldBeTrue($"Default Dashboard dropdown should be present for '{fullName}'");
            _myProfilePage.GetDropDownListDefaultValue(dropdownLabel).ShouldBeEqual("Coding Validation",
                $"Default dashboard for {fullName} should be 'Coding Validation'");
            _myProfilePage.GetDropDownListForUserProfileSettingsByLabel(dropdownLabel)
                .ShouldCollectionBeEquivalent(dashboardOptions, "Expected dashboard options should be correct");
        }

        [Test] //CAR-2906 [CAR-2910]
        public void Verify_my_profile_shows_default_dashboard_option_for_client_users()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            IDictionary<string, string> paramLists = DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);

            var userList = paramLists["UserList"].Split(';').ToList();
            var dashboardOptions = paramLists["DashboardOptions"].Split(';').ToList();

            //Tuple containing user info containing <user description, roles assigned, roles unassigned>
            IDictionary<string, Tuple<string, string, string>> userListWithDescription = new Dictionary<string, Tuple<string, string, string>>
            {
                [userList[0]] = Tuple.Create("User with none of the dashboard roles assigned", "", "Claims Processor"),
                [userList[1]] = Tuple.Create("User with all of the dashboards roles assigned", "Claims Processor", ""),
            };

            StringFormatter.PrintMessageTitle("Checking whether the users have the correct roles assigned to them");
            CurrentPage.Logout().LoginAsHciAdminUser();

            int loopCounter = 0;
            foreach (KeyValuePair<string, Tuple<string, string, string>> user in userListWithDescription)
            {
                var userId = userList[loopCounter];
                var userInfo = user.Value.Item1;
                var listOfAssignedRoles = user.Value.Item2.Split(';').ToList();
                var listOfUnassignedRoles = user.Value.Item3.Split(';').ToList();
                
                foreach (var role in listOfAssignedRoles)
                {
                    if (role != Empty)
                        if (CurrentPage.GetPageHeader() == PageHeaderEnum.UserProfileSearch.GetStringValue() &&
                            CurrentPage.GetGridViewSection.GetValueInGridByColRow(col: 3) == userId)
                        {
                            CurrentPage.IsAvailableAssignedRowPresent(1, role)
                                .ShouldBeTrue($"'{userId}' which is a {userInfo} should have the role '{role}' assigned");
                        }
                        else
                        {
                            CurrentPage.IsRoleAssigned<UserProfileSearch>(new List<string> { userId }, role)
                                .ShouldBeTrue($"'{userId}' which is a {userInfo} should have the role '{role}' assigned");
                        }
                }

                foreach (var unassignedRole in listOfUnassignedRoles)
                    if (unassignedRole != Empty)
                        if (CurrentPage.GetPageHeader() == PageHeaderEnum.UserProfileSearch.GetStringValue() &&
                            CurrentPage.GetGridViewSection.GetValueInGridByColRow(col: 3) == userId)
                        {
                            CurrentPage.IsAvailableAssignedRowPresent(1, unassignedRole)
                                .ShouldBeFalse($"'{userId}' which is a {userInfo} should not be assigned with '{unassignedRole}' role");
                        }
                        else
                        {
                            CurrentPage.IsRoleAssigned<UserProfileSearch>(new List<string> { userId }, unassignedRole)
                                .ShouldBeFalse($"'{userId}' which is a {userInfo} should not be assigned with '{unassignedRole}' role");
                        }

                loopCounter++;
            }

            StringFormatter.PrintMessageTitle("Verifying whether correct dashboard options are getting displayed in 'My Profile' page for the users");
            foreach (var user in userList)
            {
                var dropdownLabel = "Default Dashboard";

                switch (user)
                {
                    case "uiautomation_cl_dciwrklst":
                        var _myProfile = CurrentPage.Logout().LoginAsClientUserwithONLYDCIWorklistauthority().NavigateToMyProfilePage();
                        _myProfile.IsDropDownPresentByLabel(dropdownLabel)
                            .ShouldBeFalse($"Default Dashboard dropdown should not be present for {user}");
                        break;

                    case "uiautomation_cl":
                        _myProfile = CurrentPage.Logout().LoginAsClientUser().NavigateToMyProfilePage();
                        _myProfile.IsDropDownPresentByLabel(dropdownLabel)
                            .ShouldBeTrue($"Default Dashboard dropdown should be present for {user}");
                        _myProfile.GetDropDownListDefaultValue(dropdownLabel).ShouldBeEqual(ProductEnum.CV.GetStringValue(),
                            $"Default dashboard for {user} should be 'My Dashboard'");
                        _myProfile.GetDropDownListForUserProfileSettingsByLabel(dropdownLabel)
                            .ShouldCollectionBeEquivalent(dashboardOptions, "Expected dashboard options should be correct");
                        break;
                }
            }
        }

        [Test] //CAR-2848 [CAR-2153]
        public void Verify_input_fields_in_profile_and_preferences_in_my_profile_page_for_client_users()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            IDictionary<string, string> paramLists = DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
            var myProfileClientUserId = paramLists["MyProfileClientUserId"];
            var originalPrefix = paramLists["Prefix"];
            var originalFirstName = paramLists["FirstName"];
            var originalLastName = paramLists["LastName"];
            var originalSuffix = paramLists["Suffix"];
            var originalJobTitle = paramLists["JobTitle"];
            var originalCredentials = paramLists["Credentials"];
            var originalSubscriberId = paramLists["SubscriberId"];
            var originalPhoneNo = paramLists["PhoneNo"];
            var originalExt = paramLists["Ext"];
            var originalFax = paramLists["Fax"];
            var originalFaxExt = paramLists["FaxExt"];
            var originalAltPhoneNo = paramLists["AltPhoneNo"];
            var originalAltPhoneExt = paramLists["AltPhoneExt"];
            var originalEmailAddress = paramLists["EmailAddress"];
            var originalDefaultPage = paramLists["DefaultPage"];
            var originalDefaultClient = paramLists["DefaultClient"];
            var validEmailAddress = paramLists["ValidEmailAddress"];
            var invalidEmailAddress = paramLists["InvalidEmailAddresses"].Split(';').ToList();

            IDictionary<string, string> formData = new Dictionary<string, string>()
            {
                ["Prefix"] = originalPrefix,
                ["First Name"] = originalFirstName,
                ["Last Name"] = originalLastName,
                ["Suffix"] = originalSuffix,
                ["Job Title"] = originalJobTitle,
                ["Credentials"] = originalCredentials,
                ["Subscriber ID"] = originalSubscriberId,
                ["Phone Number"] = originalPhoneNo,
                ["Phone Number-Ext"] = originalExt,
                ["Fax"] = originalFax,
                ["Fax-Ext"] = originalFaxExt,
                ["Alt Phone"] = originalAltPhoneNo,
                ["Alt Phone-Ext"] = originalAltPhoneExt,
                ["Email Address"] = originalEmailAddress,
            };

            StringFormatter.PrintMessageTitle("Checking and bringing back the test data to the original state");
            if (_myProfilePage.GetAssignedRoleIdsToUserByUserId(myProfileClientUserId) != null ||
                _myProfilePage.GetAssignedClientListForUser(myProfileClientUserId)[0] != ClientEnum.SMTST.ToString())
            {
                RevertAssignedRolesAndClients(myProfileClientUserId);
            }

            CurrentPage.Logout().LoginAsClientUserForMyProfile();
            _myProfilePage = _myProfilePage.NavigateToMyProfilePage();
            _myProfilePage.GetPageHeader().ShouldBeEqual("My Profile", "Correct page header should be displayed.");

            StringFormatter.PrintMessageTitle("Verification of 'Personal Information' section");
            _myProfilePage.GetSideWindow.IsInputFieldPresentByLabel("First Name")
                .ShouldBeTrue("'First Name' input field should be present");
            _myProfilePage.GetSideWindow.IsInputFieldPresentByLabel("Last Name")
                .ShouldBeTrue("'Last Name' input field should be present");
            _myProfilePage.GetSideWindow.IsInputFieldPresentByLabel("Suffix")
                .ShouldBeTrue("'Suffix' input field should be present");
            _myProfilePage.GetSideWindow.IsInputFieldPresentByLabel("Job Title")
                .ShouldBeTrue("'Job Title' input field should be present");
            _myProfilePage.GetSideWindow.IsInputFieldPresentByLabel("Credentials")
                .ShouldBeTrue("'Credentials' input field should be present");
            _myProfilePage.GetSideWindow.IsInputFieldPresentByLabel("Subscriber ID")
                .ShouldBeTrue("'Subscriber ID' input field should be present for Client user");

            StringFormatter.PrintMessage("Verification of existing values in 'Personal Information'");
            _myProfilePage.GetSideWindow.GetInputValueByLabel("Prefix")
                .ShouldBeEqual(originalPrefix, "'Prefix' should show pre-existing value");
            _myProfilePage.GetSideWindow.GetInputValueByLabel("First Name").ShouldBeEqual(originalFirstName,
                "'First Name' should show pre-existing value");
            _myProfilePage.GetSideWindow.GetInputValueByLabel("Last Name")
                .ShouldBeEqual(originalLastName, "'Last Name' should show pre-existing value");
            _myProfilePage.GetSideWindow.GetInputValueByLabel("Suffix")
                .ShouldBeEqual(originalSuffix, "'Suffix' should show pre-existing value");
            _myProfilePage.GetSideWindow.GetInputValueByLabel("Job Title")
                .ShouldBeEqual(originalJobTitle, "'Job Title' should show pre-existing value");
            _myProfilePage.GetSideWindow.GetInputValueByLabel("Credentials").ShouldBeEqual(originalCredentials,
                "'Credentials' should show pre-existing value");
            _myProfilePage.GetSideWindow.GetInputValueByLabel("Subscriber ID").ShouldBeEqual(originalSubscriberId,
                "'Subscriber ID' should show pre-existing value");

            StringFormatter.PrintMessage("Verification of existing values in 'Contact Information'");
            _myProfilePage.GetSideWindow.GetInputValueByLabel("Phone Number").ShouldBeEqual(originalPhoneNo,
                "'Phone Number' should show pre-existing value");
            _myProfilePage.GetExtValueByType("Phone Number")
                .ShouldBeEqual(originalExt, "'Ext'(Phone) should show pre-existing value");
            _myProfilePage.GetSideWindow.GetInputValueByLabel("Fax")
                .ShouldBeEqual(originalFax, "'Fax' should show pre-existing value");
            _myProfilePage.GetExtValueByType("Fax")
                .ShouldBeEqual(originalFaxExt, "'Ext'(Fax) should show pre-existing value");
            _myProfilePage.GetSideWindow.GetInputValueByLabel("Alt Phone").ShouldBeEqual(originalAltPhoneNo,
                "'Alt Phone' should show pre-existing value");
            _myProfilePage.GetExtValueByType("Alt Phone").ShouldBeEqual(originalAltPhoneExt,
                "'Ext'(Alt Phone) should show pre-existing value");
            _myProfilePage.GetSideWindow.GetInputValueByLabel("Email Address").ShouldBeEqual(originalEmailAddress,
                "'Email Address' should show pre-existing value");

            StringFormatter.PrintMessage("Verification of existing values in 'User Preferences'");
            _myProfilePage.GetSideWindow.GetDropDownInputFieldByLabel("Default Page")
                .ShouldBeEqual(originalDefaultPage);
            _myProfilePage.GetSideWindow.GetDropDownInputFieldByLabel("Default Client")
                .ShouldBeEqual(originalDefaultClient);
            _myProfilePage.GetSideWindow.GetDropDownList("Default Client").Count
                .ShouldBeEqual(1, "When a user has just 1 client assigned then no other options will be shown");

            StringFormatter.PrintMessageTitle("Verification of checkbox fields");
            _myProfilePage.GetSideWindow.IsCheckboxCheckedByLabel("Enable Tooltips on Claim Action")
                .ShouldBeTrue("'Enable Tooltips on Claim Action' should be checked by default");
          
            StringFormatter.PrintMessageTitle("Verification of input fields under Contact Information");
            SetAllPersonalAndContactInformationToBlank();
            _myProfilePage.ClickOnSaveButton();
            _myProfilePage.IsPageErrorPopupModalPresent()
                .ShouldBeTrue("Error message pops up when required fields are left unfilled");
            _myProfilePage.GetPageErrorMessage()
                .ShouldBeEqual("Invalid or missing data must be resolved before the record can be saved.");
            _myProfilePage.ClosePageError();

            _myProfilePage.IsInvalidInputPresentByLabel("First Name")
                .ShouldBeTrue("'First Name' should be a required field");
            _myProfilePage.IsInvalidInputPresentByLabel("Last Name")
                .ShouldBeTrue("'Last Name' should be a required field");
            _myProfilePage.IsInvalidInputPresentByLabel("Job Title")
                .ShouldBeTrue("'Job Title' should be a required field");
            _myProfilePage.IsInvalidInputPresentByLabel("Phone Number")
                .ShouldBeTrue("'Phone Number' should be a required field");
            _myProfilePage.IsInvalidInputPresentByLabel("Email Address")
                .ShouldBeTrue("'Email Address' should be a required field");
            _myProfilePage.GetCountOfInvalidRed().ShouldBeEqual(6, "Required fields should be highlighted");

            StringFormatter.PrintMessageTitle("Validating Cancel button");
            _myProfilePage.ClickOnCancelButton();
            CurrentPage.GetPageHeader().ShouldBeEqual(originalDefaultPage,
                "User should be navigated to default landing page.");
            _myProfilePage = CurrentPage.NavigateToMyProfilePage();
            VerifyDataInTheForm(formData, originalDefaultPage, originalDefaultClient, true)
                .ShouldBeTrue("Form data should be unchanged when Cancel button is clicked");

            StringFormatter.PrintMessageTitle("Validating Phone/Alt Phone/Fax Numbers");
            var allowedNumericValues = "23456789";
            ValidatePhoneFaxInputFieldByLabel("Phone Number", allowedNumericValues, 10);
            ValidatePhoneFaxInputFieldByLabel("Fax", allowedNumericValues, 10);
            ValidatePhoneFaxInputFieldByLabel("Alt Phone", allowedNumericValues, 10);

            StringFormatter.PrintMessageTitle("Validating Email Address field");
            ValidateEmailInputField(validEmailAddress);


            foreach (var invalidEmail in invalidEmailAddress)
            {
                ValidateEmailInputField(invalidEmail, false);
            }

            #region LOCAL METHODS

            void SetAllPersonalAndContactInformationToBlank()
            {
                StringFormatter.PrintMessage("Setting input fields in Contact Information as empty");
                _myProfilePage.GetSideWindow.SetInputInInputFieldByLabel("Phone Number", "");
                _myProfilePage.SetExtValueByType("Phone Number", "");
                _myProfilePage.GetSideWindow.SetInputInInputFieldByLabel("Fax", "");
                _myProfilePage.SetExtValueByType("Fax", "");
                _myProfilePage.GetSideWindow.SetInputInInputFieldByLabel("Alt Phone", "");
                _myProfilePage.SetExtValueByType("Alt Phone", "");
                _myProfilePage.GetSideWindow.SetInputInInputFieldByLabel("Email Address", "");

                StringFormatter.PrintMessage("Setting input fields in Personal Information as empty");
                _myProfilePage.GetSideWindow.SetInputInInputFieldByLabel("Prefix", "");
                _myProfilePage.GetSideWindow.SetInputInInputFieldByLabel("First Name", "");
                _myProfilePage.GetSideWindow.SetInputInInputFieldByLabel("Last Name", "");
                _myProfilePage.GetSideWindow.SetInputInInputFieldByLabel("Suffix", "");
                _myProfilePage.GetSideWindow.SetInputInInputFieldByLabel("Job Title", "");
                _myProfilePage.GetSideWindow.SetInputInInputFieldByLabel("Credentials", "");
            }

            void ValidatePhoneFaxInputFieldByLabel(string label, string values, int length, bool requiredField = true)
            {
                var inputTextForPhoneFax =
                    _myProfilePage.GenerateRandomTextBasedOnAcceptedValues(values, length + 1);
                var inputTextForExt = _myProfilePage.GenerateRandomTextBasedOnAcceptedValues(values, length + 1);

                _myProfilePage.GetSideWindow.SetInputInInputFieldByLabel(label, inputTextForPhoneFax);
                _myProfilePage.SetExtValueByType(label, inputTextForExt);

                var phoneFaxTextFromForm = _myProfilePage.GetSideWindow.GetDropDownInputFieldByLabel(label);
                var extTextFromForm = _myProfilePage.GetExtValueByType(label);
                var phoneFaxRegexWithoutHyphen = phoneFaxTextFromForm.Replace("-", "");

                Regex phoneFaxRegex = new Regex("^\\d{3}-\\d{3}-\\d{4}$");
                phoneFaxRegex.IsMatch(phoneFaxTextFromForm)
                    .ShouldBeTrue("Entered phone should be in correct format");
                phoneFaxRegexWithoutHyphen.ShouldBeEqual(inputTextForPhoneFax.Substring(0, length),
                    $"'{label}' field should accept only {length} numbers");

                extTextFromForm.ShouldBeEqual(inputTextForExt.Substring(0, length),
                    $"{label} - Ext should only allow {length} numbers");
            }

            void ValidateEmailInputField(string emailAddress, bool isValidEmail = true)
            {
                var label = "Email Address";

                _myProfilePage.GetSideWindow.SetInputInInputFieldByLabel(label, emailAddress);

                if (isValidEmail)
                    _myProfilePage.IsInvalidInputPresentByLabel(label)
                        .ShouldBeFalse($"{label} should accept email address in the form of XXX@XXX.XXX");

                else
                    _myProfilePage.IsInvalidInputPresentByLabel(label).ShouldBeTrue(
                        $"{label} Error highlight should be shown in the text input field when un invalid email is entered");
            }

            #endregion


        }

        [Test] //CAR-2848 [CAR-2153]
        public void Verify_save_in_profile_and_preferences_in_my_profile_page_for_client_users()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            IDictionary<string, string> paramLists = DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
            var myProfileClientUserId = paramLists["MyProfileClientUserId"];
            var originalPrefix = paramLists["Prefix"];
            var originalFirstName = paramLists["FirstName"];
            var originalLastName = paramLists["LastName"];
            var originalSuffix = paramLists["Suffix"];
            var originalJobTitle = paramLists["JobTitle"];
            var originalCredentials = paramLists["Credentials"];
            var originalSubscriberId = paramLists["SubscriberId"];
            var originalEmailAddress = paramLists["EmailAddress"];
            var originalDefaultClient = paramLists["DefaultClient"];
            var allDefaultPages = paramLists["AllDefaultPages"].Split(';').ToList();

            StringFormatter.PrintMessageTitle("Checking and bringing back the test data to the original state");
            if (_myProfilePage.GetAssignedRoleIdsToUserByUserId(myProfileClientUserId) != null ||
                _myProfilePage.GetAssignedClientListForUser(myProfileClientUserId)[0] != ClientEnum.SMTST.ToString())
            {
                RevertAssignedRolesAndClients(myProfileClientUserId);
            }
            
            try
            {
                StringFormatter.PrintMessageTitle("Verification of the 'Default Client' and 'Default Page' dropdown fields");
                var _newUserProfileSearch = CurrentPage.Logout().LoginAsHciAdminUser().NavigateToNewUserProfileSearch();

                _newUserProfileSearch.SearchUserByNameOrId(new List<string> { myProfileClientUserId }, true);
                _newUserProfileSearch.GetGridViewSection.ClickOnGridRowByRow();
                _newUserProfileSearch.ClickOnUserSettingTabByTabName(UserSettingsTabEnum.Clients.GetStringValue());
                _newUserProfileSearch.ClickOnEditIcon();
                _newUserProfileSearch.SelectDeselectAll("Available Clients");
                var assignedClientList = _newUserProfileSearch.GetAvailableAssignedList(2, false);

                _newUserProfileSearch.ClickOnUserSettingTabByTabName(
                    UserSettingsTabEnum.RolesAuthorities.GetStringValue());
                _newUserProfileSearch.SelectDeselectAll("Available Roles");
                _newUserProfileSearch.GetSideWindow.Save(waitForWorkingMessage: true);
                if (_newUserProfileSearch.IsPageErrorPopupModalPresent())
                {
                    _newUserProfileSearch.ClosePageError();
                }
                _newUserProfileSearch.ClickOnUserSettingTabByTabName(UserSettingsTabEnum.Preferences.GetStringValue());
                _newUserProfileSearch.SelectDropDownListValueByLabel(UserPreferencesEnum.DefaultDashboard.GetStringValue(), "CV Dashboard");
                _newUserProfileSearch.GetSideWindow.Save(waitForWorkingMessage: true);


                _myProfilePage = _newUserProfileSearch.Logout().LoginAsClientUserForMyProfile()
                    .NavigateToMyProfilePage();
                var clientList = _myProfilePage.GetSideWindow.GetDropDownList("Default Client");
                var defaultPageList = _myProfilePage.GetSideWindow.GetDropDownList("Default Page");

                StringFormatter.PrintMessage("Verifying the department dropdown field should not be present for client user");
                _myProfilePage.IsDropDownPresentByLabel(UserProfileEnum.Department.GetStringValue())
                    .ShouldBeFalse("Department dropdown should be present in My Profile page of an internal user");

                _myProfilePage.GetSideWindow.GetDropDownInputFieldByLabel("Default Client").ShouldBeEqual(
                    originalDefaultClient,
                    "Default client should remain the same even though new clients are added");

                var clientCodesInDefaultClientDropdown =
                    clientList.Select(client => client.Split('-')[0].Trim()).ToList();

                clientCodesInDefaultClientDropdown
                    .ShouldCollectionBeEquivalent(assignedClientList,
                        "All the assigned clients should be present in the default client dropdown");

                defaultPageList.ShouldCollectionBeEquivalent(allDefaultPages,
                    "All the pages should be available once all roles are assigned to the user");


                StringFormatter.PrintMessageTitle("Verifying updating the form data and saving the changes");
                FillFormDataAndVerify();

                #region LOCAL METHODS

                void FillFormDataAndVerify()
                {
                    IDictionary<string, string> formDataToBeFilled = new Dictionary<string, string>()
                    {
                        ["Prefix"] = originalPrefix + "updated",
                        ["First Name"] = originalFirstName + "updated",
                        ["Last Name"] = originalLastName + "updated",
                        ["Suffix"] = originalSuffix + "updated",
                        ["Job Title"] = originalJobTitle + "updated",
                        ["Credentials"] = originalCredentials + "updated",
                        ["Subscriber ID"] = originalSubscriberId + "updated",

                        ["Phone Number"] = _myProfilePage.GenerateRandomTextBasedOnAcceptedValues("23456789", 10),
                        ["Phone Number-Ext"] = _myProfilePage.GenerateRandomTextBasedOnAcceptedValues("23456789", 10),
                        ["Fax"] = _myProfilePage.GenerateRandomTextBasedOnAcceptedValues("23456789", 10),
                        ["Fax-Ext"] = _myProfilePage.GenerateRandomTextBasedOnAcceptedValues("23456789", 10),
                        ["Alt Phone"] = _myProfilePage.GenerateRandomTextBasedOnAcceptedValues("23456789", 10),
                        ["Alt Phone-Ext"] = _myProfilePage.GenerateRandomTextBasedOnAcceptedValues("23456789", 10),
                        ["Email Address"] = originalEmailAddress.Replace(".com", ".org")

                    };
                    var updatedDefaultPage = "Appeal Search";
                    var updatedDefaultClient = "TTREE - TallTree Administrator";

                    _myProfilePage
                        .FillAllInputFieldsInProfileAndPreferencesTabInMyProfile(formDataToBeFilled, updatedDefaultPage,
                            updatedDefaultClient, false, false, isInternalUser: false);

                    _myProfilePage.ClickOnSaveButton();

                    CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.AppealSearch.GetStringValue(),
                        "User should be taken to the default landing page");

                    _myProfilePage.NavigateToMyProfilePage();
                    VerifyDataInTheForm(formDataToBeFilled, updatedDefaultPage, updatedDefaultClient, false)
                        .ShouldBeTrue("Updated data should be saved successfully");

                    StringFormatter.PrintMessageTitle("Verifying saved data is reflected in User Profile Search page");
                    _newUserProfileSearch = _myProfilePage.Logout().LoginAsHciAdminUser().NavigateToNewUserProfileSearch();
                    _newUserProfileSearch.SearchUserByNameOrId(new List<string> { myProfileClientUserId }, true);
                    _newUserProfileSearch.GetGridViewSection.ClickOnGridRowByRow();
                    _newUserProfileSearch.ClickOnUserSettingTabByTabName(UserSettingsTabEnum.Profile.GetStringValue());
                    _newUserProfileSearch.ClickOnEditIcon();

                    foreach (var key in formDataToBeFilled.Keys)
                    {
                        var label = key;
                        var phoneNumberFields = new List<string> { "Phone Number", "Fax", "Alt Phone" };

                        if (phoneNumberFields.Contains(label))
                        {
                            label = label == "Phone Number" ? "Phone" : label;

                            _newUserProfileSearch.GetInputFromPhoneFaxFields(label).Replace("-", "").ShouldBeEqual(formDataToBeFilled[key].Replace("-", ""),
                                $"Data is correctly reflected in {label}");

                            continue;
                        }

                        if (label.Contains("-"))
                        {
                            switch (label.Split('-')[0])
                            {
                                case "Phone Number":
                                    _newUserProfileSearch.GetInputFromPhoneFaxFields("Phone", false).ShouldBeEqual(formDataToBeFilled[key],
                                    $"Data is correctly reflected in {label}");
                                    continue;

                                case "Fax":
                                    _newUserProfileSearch.GetInputFromPhoneFaxFields("Fax", false).ShouldBeEqual(formDataToBeFilled[key],
                                        $"Data is correctly reflected in {label}");
                                    continue;

                                case "Alt Phone":
                                    _newUserProfileSearch.GetInputFromPhoneFaxFields("Alt Phone", false).ShouldBeEqual(formDataToBeFilled[key],
                                        $"Data is correctly reflected in {label}");
                                    continue;

                            }
                        }
                        _newUserProfileSearch.GetSideWindow.GetInputValueByLabel(label).ShouldBeEqual(formDataToBeFilled[label], $"Data is correctly reflected in {label}");
                    }

                    _newUserProfileSearch.ClickOnUserSettingTabByTabName(UserSettingsTabEnum.Preferences.GetStringValue());
                    _newUserProfileSearch.GetInputTextBoxValueByLabel("Default Page").ShouldBeEqual(updatedDefaultPage,
                        "Default Page should show the updated value");

                    _newUserProfileSearch.GetInputTextBoxValueByLabel("Default Client")
                        .ShouldBeEqual(updatedDefaultClient.Split('-')[0].Trim(),
                            "Default Client should show the updated value");

                    _newUserProfileSearch.IsRadioButtonOnOffByLabel("Enable Claim Action Tooltips")
                        .ShouldBeFalse("'Enable Claim Action Tooltips' should be set to NO");
                }

                #endregion
            }

            finally
            {
                StringFormatter.PrintMessageTitle("Finally Block");

                if (_myProfilePage.GetAssignedRoleIdsToUserByUserId(myProfileClientUserId) != null ||
                    _myProfilePage.GetAssignedClientListForUser(myProfileClientUserId)[0] != ClientEnum.SMTST.ToString())
                {
                    RevertAssignedRolesAndClients(myProfileClientUserId);
                }
            }

        }

        #endregion

        #region PUBLIC METHODS

        void RevertAssignedRolesAndClients(string userId)
        {
            StringFormatter.PrintMessage("Reverting the profile and preferences info for the user in DB");
            _myProfilePage.RevertUserInfoInProfileAndPreferences(userId, false);

            StringFormatter.PrintMessage("Reverting the assigned roles and clients for the user");
            if (Compare(UserType.CurrentUserType, UserType.HCIADMIN, StringComparison.OrdinalIgnoreCase) != 0)
                _myProfilePage.Logout().LoginAsHciAdminUser();

            var _newUserProfileSearch = CurrentPage.NavigateToNewUserProfileSearch(false);
            _newUserProfileSearch.SearchUserByNameOrId(new List<string>() { userId }, true);
            _newUserProfileSearch.GetGridViewSection.ClickOnGridRowByRow();
            _newUserProfileSearch.ClickOnUserSettingTabByTabName("Clients");
            _newUserProfileSearch.ClickOnEditIcon();
            _newUserProfileSearch.SelectDeselectAll("Assigned Clients");

            _newUserProfileSearch.ClickOnUserSettingTabByTabName(UserSettingsTabEnum.RolesAuthorities.GetStringValue());
            _newUserProfileSearch.SelectDeselectAll("Assigned Roles");
            _newUserProfileSearch.GetSideWindow.Save(waitForWorkingMessage: true);
        }

        bool VerifyDataInTheForm(IDictionary<string, string> expectedFormData, string defaultPage, string defaultClient, bool isTooltipsChecked)
        {
            bool isDataCorrect = true;

            foreach (var key in expectedFormData.Keys)
            {
                var formLabel = key;

                if (formLabel.Contains('-'))
                    formLabel = key.Split('-')[0];

                isDataCorrect = isDataCorrect && _myProfilePage.GetSideWindow.GetInputValueByLabel(formLabel).Replace("-", "")
                                    .Equals(expectedFormData[formLabel].Replace("-", ""));
            }

            isDataCorrect = isDataCorrect
                            &&
                            _myProfilePage.GetSideWindow.GetDropDownInputFieldByLabel("Default Page")
                                .Equals(defaultPage)
                            &&
                            _myProfilePage.GetSideWindow.GetDropDownInputFieldByLabel("Default Client")
                                .Equals(defaultClient)
                            &&
                            (_myProfilePage.GetSideWindow.IsCheckboxCheckedByLabel("Enable Tooltips on Claim Action") == isTooltipsChecked);
            return isDataCorrect;
        }

        [Test, Category("OnDemand")] //CAR-2836 [CAR-2489]
        public void Verify_profile_security_settings_for_client_user()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            var userId = DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName, "UserId",
                "Value");
            var changedPasswordList = DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName, "ChangedPasswordList",
                "Value").Split(';').ToList();
            var questionList = DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName, "QuestionList",
                "Value").Split(';').ToList();
            var answerList = DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName, "AnswerList",
                "Value").Split(';').ToList();
            var newPassword = DataHelper.GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName, "NewPassword",
                "Value");
            var fullName = _myProfilePage.GetUserNameOfClient("dontusethis_internal");
            _myProfilePage.DeleteLastSavedPasswordByUser(userId);
            var questionListsFromDb = _myProfilePage.GetAvailabeQuestionListFromDb();
            try
            {
                CurrentPage.Logout().LoginAsClientUserForMyProfile();
                _myProfilePage.NavigateToMyProfilePage();
                _myProfilePage.ClickOnTabByName("Security Settings");
                _myProfilePage.IsSectionSelectedByTabName("Security Settings")
                    .ShouldBeTrue("Security Settings tab should be selected and underlined.");

                StringFormatter.PrintMessageTitle("Verification of input fields");
                _myProfilePage.IsDropDownPresentByLabel("Security Question 1")
                    .ShouldBeTrue("Security Question 1 dropdown should be present.");
                _myProfilePage.IsDropDownPresentByLabel("Security Question 2")
                    .ShouldBeTrue("Security Question 2 dropdown should be present.");
                _myProfilePage.IsTextBoxPresentByLabel("Change Password", "Password")
                    .ShouldBeTrue("Password Text Box should be present.");
                _myProfilePage.IsTextBoxPresentByLabel("Change Password", "Confirm Password")
                    .ShouldBeTrue("Confirm Password Text Box should be present.");

                StringFormatter.PrintMessage("Clicking Cancel changes will be discarded and user will be navigated to the default landing page.");
                _myProfilePage.ClickOnCancelButton();
                CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.ClaimSearch.GetStringValue(), "When cancel is clicked" +
                                                                                                       "user should be directed to Claim Search which is the default page for the user");
                CurrentPage.NavigateToMyProfilePage();
                _myProfilePage.ClickOnTabByName("Security Settings");

                StringFormatter.PrintMessage($"Form validation for the '{SecurityEnum.SecurityQuestions.GetStringValue()}' input fields");
                var allQuestionsInSecurityQ1Dropdown =
                    _myProfilePage.GetDropDownListForUserProfileSettingsByLabel(SecurityEnum.SecurityQuestion1
                        .GetStringValue());
                var questionListsFromDbExceptSecurityQ2 = questionListsFromDb.ToList();
                var questionListsFromDbExceptSecurityQ1 = questionListsFromDb.ToList();

                questionListsFromDbExceptSecurityQ2.Remove(_myProfilePage.GetDropDownListDefaultValue(SecurityEnum.SecurityQuestion2.GetStringValue()));
                allQuestionsInSecurityQ1Dropdown.ShouldCollectionBeEquivalent(questionListsFromDbExceptSecurityQ2, "Questions list should be correct");
                _myProfilePage.SelectDropDownListValueByLabel(SecurityEnum.SecurityQuestion1.GetStringValue(), questionList[0]);
                _myProfilePage.ValidateAnswerInputBoxByLabel("Security Questions", SecurityEnum.SecurityQuestion1.GetStringValue()).ShouldBeEqual(100, "Answer Text Field should allow up to 100 alphanumeric or special character values.");
                _myProfilePage.SetInputTextBoxValueByLabel("Security Questions", SecurityEnum.SecurityQuestion1.GetStringValue(), answerList[0]);
                _myProfilePage.GetInputTextBoxValueByLabel("Security Questions", SecurityEnum.SecurityQuestion1.GetStringValue()).ShouldBeEqual(answerList[0], "Answer should be shown after clicking the Show Answer link.");
                questionListsFromDbExceptSecurityQ1.Remove(_myProfilePage.GetDropDownListDefaultValue(SecurityEnum.SecurityQuestion1.GetStringValue()));

                var allQuestionsInSecurityQ2Dropdown =
                    _myProfilePage.GetDropDownListForUserProfileSettingsByLabel(SecurityEnum.SecurityQuestion2
                        .GetStringValue());

                allQuestionsInSecurityQ2Dropdown.ShouldCollectionBeEquivalent(questionListsFromDbExceptSecurityQ1, "Questions list should be correct");
                _myProfilePage.SelectDropDownListValueByLabel(SecurityEnum.SecurityQuestion2.GetStringValue(), questionList[1]);
                _myProfilePage.ValidateAnswerInputBoxByLabel("Security Questions", SecurityEnum.SecurityQuestion2.GetStringValue()).ShouldBeEqual(100, "Answer Text Field should allow up to 100 alphanumeric or special character values.");
                _myProfilePage.SetInputTextBoxValueByLabel("Security Questions", SecurityEnum.SecurityQuestion2.GetStringValue(), answerList[1]);
                _myProfilePage.GetInputTextBoxValueByLabel("Security Questions", SecurityEnum.SecurityQuestion2.GetStringValue()).ShouldBeEqual(answerList[1], "Answer should be shown after clicking the Show Answer link.");
                
                StringFormatter.PrintMessageTitle("Changing the password 10 times");
                var countOfUserAudits = 0;
                foreach (var password in changedPasswordList)
                {
                    _myProfilePage.SetInputTextBoxValueByLabel("Change Password", "Password", password);
                    _myProfilePage.SetInputTextBoxValueByLabel("Change Password", "Confirm Password", password);
                    _myProfilePage.ClickOnSaveButton();
                    countOfUserAudits++;
                    CurrentPage.NavigateToMyProfilePage();
                    _myProfilePage.GetLastSavedPasswordByUser(userId).ShouldBeEqual(countOfUserAudits, "Audit record should be created for password change.");
                    _myProfilePage.ClickOnTabByName("Security Settings");
                }

                StringFormatter.PrintMessageTitle("Verifying that the user is not allowed to set a password within the 10 last changed passwords");
                _myProfilePage.SetInputTextBoxValueByLabel("Change Password", "Password", changedPasswordList[0]);
                _myProfilePage.SetInputTextBoxValueByLabel("Change Password", "Confirm Password", changedPasswordList[0]);
                _myProfilePage.ClickSaveOrCancel(true);
                _myProfilePage.IsPageErrorPopupModalPresent()
                    .ShouldBeTrue("User should not be allowed to enter password within the last 10 passwords");
                _myProfilePage.GetPageErrorMessage()
                    .ShouldBeEqual("Password must not match any of the last 10 saved passwords.");
                _myProfilePage.ClosePageError();
            }

            finally
            {
                StringFormatter.PrintMessageTitle("Running finally block");
                if (_myProfilePage.IsPageErrorPopupModalPresent())
                    _myProfilePage.ClosePageError();
                
                if (CurrentPage.GetPageHeader() != PageHeaderEnum.MyProfile.GetStringValue())
                {
                    CurrentPage.NavigateToMyProfilePage();
                    _myProfilePage.ClickOnTabByName("Security Settings");
                }

                StringFormatter.PrintMessage("Setting the password to original");
                _myProfilePage.SetInputTextBoxValueByLabel("Change Password", "Password", newPassword);
                _myProfilePage.SetInputTextBoxValueByLabel("Change Password", "Confirm Password", newPassword);
                _myProfilePage.ClickOnSaveButton();
                _myProfilePage.DeleteLastSavedPasswordByUser(userId);
                
            }
        }
        #endregion
    }
}

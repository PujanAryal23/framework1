using System;
using System.Collections.Generic;
using Nucleus.Service.Data;
using Nucleus.Service.PageServices.Claim;
using Nucleus.Service.PageServices.Settings;
using Nucleus.Service.PageServices.Settings.User;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Environment;
using Nucleus.UIAutomation.TestSuites.Base;
using NUnit.Framework;
using System.Diagnostics;
using Nucleus.Service.Support.Common.Constants;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using Nucleus.Service.PageServices.Provider;
using UIAutomation.Framework.Core.Driver;
using Extensions = Nucleus.Service.Support.Utils.Extensions;
using Nucleus.Service.PageServices.Base.Default;
using Nucleus.Service.PageServices.QuickLaunch;
using Nucleus.Service.Support.Menu;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Utils;
using System.Configuration;
using System.Drawing;
using System.Globalization;
using Nucleus.Service.Support.Common;

namespace Nucleus.UIAutomation.TestSuites.TestCases
{
    public class Settings : NewAutomatedBase
    {
        #region PRIVATE FIELDS

        private ClientSearchPage _clientSearch;
        private ClaimActionPage _claimAction;
        private ClaimSearchPage _claimSearch;
        private ProfileManagerPage _profileManager;
        private OldUserProfileSearchPage _userProfileSearch;
        private MaintenanceNoticesPage _maintenanceNotices;
        private QuickLaunchPage _quickLaunch;
        private UserProfileSearchPage _newUserProfileSearchPage;
        private SuspectProvidersPage _suspectProviders;

        #endregion

        #region PROTECTED PROPERTIES

        protected override string FullyQualifiedClassName
        {
            get { return GetType().FullName; }
        }

        #endregion

        #region OVERRIDE METHODS

        /// <summary>
        /// Override ClassInit to add additional code.
        /// </summary>
        protected override void ClassInit()
        {
            try
            {
                base.ClassInit();
            }
            catch (Exception)
            {
                if (StartFlow != null)
                    StartFlow.Dispose();
                throw;
            }
        }
        protected override void ClassCleanUp()
        {
            try
            {
                if (_profileManager != null)
                    _profileManager.CloseDbConnection();
            }

            finally
            {
                base.ClassCleanUp();
            }
        }

        protected override void TestCleanUp()
        {
            if (string.Compare(UserType.CurrentUserType, UserType.HCIADMIN, StringComparison.OrdinalIgnoreCase) != 0)
            {
                QuickLaunch = CurrentPage.Logout().LoginAsHciAdminUser();
            }
            else 
            {
                CurrentPage.ClickOnQuickLaunch();
            }
            base.TestCleanUp();
        }

        #endregion

        
        #region TEST SUITES

    
        [Test, Category("SmokeTest")]
        public void Navigate_to_client_search_through_client_search_menu()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            StringFormatter.PrintMessageTitle("Verify the title of Page");
            CurrentPage = _clientSearch = QuickLaunch.NavigateToClientSearch();
            _clientSearch.PageTitle.ShouldBeEqual(_clientSearch.CurrentPageTitle, "PageTitle");
            StringFormatter.PrintLineBreak();
        }



        [Test]//US68635
        public void Verify_Client_Setting_Allow_Client_Users_to_Modify_Autoreviewed_Flags_should_always_be_enabled()
        {
            CurrentPage = _clientSearch = QuickLaunch.NavigateToClientSearch();
            _clientSearch.SearchByClientCodeToNavigateToClientProfileViewPage(ClientEnum.SMTST.ToString(), 
                ClientSettingsTabEnum.Configuration.GetStringValue());
            
            StringFormatter.PrintMessageTitle($"Client setting '{ConfigurationSettingsEnum.ClientsCanModifyAAflags.GetStringValue()}' should always be enabled " +
                $"regardless of the state of the '{ConfigurationSettingsEnum.ReverseFlag.GetStringValue()}' setting. ");

            var allowSwitchNonReverseFlagLabel = ConfigurationSettingsEnum.NonReverseFlag.GetStringValue();
            var clientsCanModifyAAFlagsLabel = ConfigurationSettingsEnum.ClientsCanModifyAAflags.GetStringValue();

            _clientSearch.SelectDropDownListForClientSettingsByLabel(allowSwitchNonReverseFlagLabel, "YES");
            _clientSearch.IsRadioButtonOnOffByLabel(clientsCanModifyAAFlagsLabel)
                .ShouldBeTrue($"'{clientsCanModifyAAFlagsLabel}' should always be enabled");
            
            _clientSearch.SelectDropDownListForClientSettingsByLabel(allowSwitchNonReverseFlagLabel, "NO");
            _clientSearch.IsRadioButtonOnOffByLabel(clientsCanModifyAAFlagsLabel)
                .ShouldBeTrue($"'{clientsCanModifyAAFlagsLabel}' should always be enabled");

            _clientSearch.SelectDropDownListForClientSettingsByLabel(allowSwitchNonReverseFlagLabel, "YES - only to client unreviewed claims");
            _clientSearch.IsRadioButtonOnOffByLabel(clientsCanModifyAAFlagsLabel)
                .ShouldBeTrue($"'{clientsCanModifyAAFlagsLabel}' should always be enabled");
        }

       

        //[Test, Category("SmokeTest")]
        public void Verify_that_restriction_message_is_displayed_if_user_creates_or_edits_a_password_on_the_profile_manager_page()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            ProfileManagerPage profileManager = QuickLaunch.NavigateToProfileManager();
            CurrentPage = profileManager;
            profileManager.ClickOnSecurityTab();
            profileManager.InsertNewPassword("testpassword");
            profileManager.InsertConfirmNewPassword("testpassword");
            profileManager.ClickSaveButton();
            profileManager.IsErrorModalPresent().ShouldBeTrue("Password Restriction Modal is present.");
            profileManager.GetPageErrorMessage().ShouldBeEqual("Passwords must meet the following criteria:\r\n*Must contain at least 8 characters\r\n*Must contain at least 1 special character\r\n*Must contain at least 1 numeric character\r\n*Must contain at least 1 uppercase character\r\n*Must contain at least 1 lowercase character", "Password Restriction Message");
        }

        //[Test, Category("SmokeTest")]
        public void Verify_that_password_used_message_is_displayed_if_user_save_previously_used_password_on_the_profile_manager_page()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            ProfileManagerPage profileManager = QuickLaunch.NavigateToProfileManager();
            CurrentPage = profileManager;
            profileManager.ClickOnSecurityTab();
            profileManager.InsertNewPassword("Test123$");
            profileManager.InsertConfirmNewPassword("Test123$");
            profileManager.ClickSaveButton();
            bool isErrorModalPresent = profileManager.IsErrorModalPresent();
            isErrorModalPresent.ShouldBeTrue("Password Restriction Modal is present.");
            profileManager.GetPageErrorMessage().ShouldBeEqual("This password has previously been used.", "Password Restriction Message");
            if (isErrorModalPresent) profileManager.CloseErrorModal();

        }

       

       
       


     
        [Test, Category("SmokeTest")]//US25238+TANT-192
        public void Verify_that_if_enable_quick_delete_is_selected_to_yes_option_the_quick_delete_icons_at_header_line_and_flag_level_will_all_be_displayed_and_operate_as_usual_for_Cotiviti_user()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            IDictionary<string, string> paramLists = DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
            string claimSequence = paramLists["ClaimSequence"];

            CurrentPage = _clientSearch = QuickLaunch.NavigateToClientSearch();

            _clientSearch.SearchByClientCodeToNavigateToClientProfileViewPage(ClientEnum.SMTST.ToString(),
                ClientSettingsTabEnum.Configuration.GetStringValue());

            _clientSearch.IsRadioButtonOnOffByLabel(ConfigurationSettingsEnum.QuickDeleteFlag.GetStringValue())
                .ShouldBeTrue($"{ConfigurationSettingsEnum.QuickDeleteFlag.GetStringValue()} should checked");


            _claimAction = CurrentPage.NavigateToClaimSearch()
                .SearchByClaimSequenceToNavigateToClaimActionPage(claimSequence);

            _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
            _claimAction.IsDeleteAllFlagsPresent().ShouldBeTrue("Delete Icon at header is present ");
            _claimAction.IsDeleteFlagsIconPresent().ShouldBeTrue("Delete Icon at flag is present");
            _claimAction.IsDeleteLineIconPresent().ShouldBeTrue("Delete Icon at line is present");

        }

        [Test]//US25238+TANT-192
        public void Verify_that_if_enable_quick_delete_is_selected_to_no_option_the_quick_delete_icons_at_header_line_and_flag_level_will_not_be_affected_for_Cotiviti_user()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            IDictionary<string, string> paramLists = DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
            string claimSequence = paramLists["ClaimSequence"];

            CurrentPage = _clientSearch = QuickLaunch.NavigateToClientSearch();

            _clientSearch.SearchByClientCodeToNavigateToClientProfileViewPage(ClientEnum.TTREE.ToString(),
                ClientSettingsTabEnum.Configuration.GetStringValue());

            _clientSearch.IsRadioButtonOnOffByLabel(ConfigurationSettingsEnum.QuickDeleteFlag.GetStringValue(),false)
                .ShouldBeTrue($"{ConfigurationSettingsEnum.QuickDeleteFlag.GetStringValue()} should not checked");


            CurrentPage.ClickOnSwitchClient().SwitchClientTo(ClientEnum.TTREE,true);

            _claimSearch = QuickLaunch.NavigateToClaimSearch();
            _claimAction = _claimSearch.SearchByClaimSequenceToNavigateToClaimActionPage(claimSequence);
            _claimAction.HandleAutomaticallyOpenedPatientClaimHistoryPopup();
            _claimAction.IsDeleteAllFlagsPresent().ShouldBeTrue("Delete Icon at header is present ");
            _claimAction.IsDeleteLineIconPresent().ShouldBeTrue("Delete Icon at line is present");
            _claimAction.IsDeleteFlagsIconPresent().ShouldBeTrue("Delete Icon at flag is present");

        }

       

        

      
       


        // Please make sure to run this test till the end while debugging or running in case of any failure during the test
        [Test,  Category("OnDemand")] //TE-178 + TE-682
        public void Verify_Enable_Ip_Filter_Setting_functionality()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            var remoteIp = DataHelper
                .GetSingleTestData(FullyQualifiedClassName, TestExtensions.TestName, "RemoteIp", "Value").Split(';')
                .ToList();
            var auditDataListForLimitAccessByClientIPAddress = DataHelper.GetMappingData(FullyQualifiedClassName, "LimitAccessByClientIPAddressAuditData").Values.ToList();
            var auditDataListForCotivitiAccessByIpAddress = DataHelper.GetMappingData(FullyQualifiedClassName, "CotivitiAccessByIPAddressAuditData").Values.ToList();
            var auditDataListForlistOfIPAddresses = DataHelper.GetMappingData(FullyQualifiedClassName, "IPListAuditData").Values.ToList();

            var testserver = ConfigurationManager.AppSettings["TestServer"];
            var isRemoteTestServer = !string.IsNullOrEmpty(testserver);
            var localIp = GetLocalIPAddress();
            var cotivitiIPList = new List<string>();

            var label = SecurityTabEnum.LimitAccessByClientIPAddress.GetStringValue();
            var _newClientSearch = CurrentPage.NavigateToClientSearch();

            try
            {
                _newClientSearch.SearchByClientCodeToNavigateToClientProfileViewPage(ClientEnum.SMTST.ToString(),
                    ClientSettingsTabEnum.Security.GetStringValue());

                StringFormatter.PrintMessage("Verify Error Message When IP Field is blank");
                _newClientSearch.ClickOnRadioButtonByLabel(label);
                _newClientSearch.SetEmptyIp();
                _newClientSearch.GetSideWindow.SaveWithoutJs();
                _newClientSearch.IsPageErrorPopupModalPresent()
                    .ShouldBeTrue("Page Error popUp model should be present");
                _newClientSearch.GetPageErrorMessage().ShouldBeEqual("Comma separated valid IP and CIDR (e.g. 127.0.0.1,127.0.0.2/32) is required before the record can be saved.");
                _newClientSearch.ClosePageError();

                StringFormatter.PrintMessage("Validating Default range is 32");
                _newClientSearch.SetIp(remoteIp[0]);
                _newClientSearch.GetSideWindow.SaveWithoutJs();

                //_newClientSearch.RefreshPage();

                StringFormatter.PrintMessage("Verify Audit For Limit Access by client IP address");
                var auditDataForLimitAccessByClientIPAddressFromDb = _newClientSearch.GetClientSettingAuditFromDatabase(ClientEnum.SMTST.ToString(), 1);
                var lastModDate = Convert.ToDateTime(auditDataForLimitAccessByClientIPAddressFromDb[0][2]).ToString("MM/dd/yyyy hh:mm tt").Trim();
                var systemdate = DateTime.ParseExact(_newClientSearch.GetCommonSql.GetSystemDateFromDB(), "MM/dd/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);
                DateTime.ParseExact(lastModDate, "MM/dd/yyyy hh:mm tt", CultureInfo.InvariantCulture).AssertDateRange(systemdate.AddMinutes(-2),
                    systemdate.AddMinutes(1), "Last modified date should match");
                auditDataForLimitAccessByClientIPAddressFromDb[0].RemoveAt(2);
                auditDataForLimitAccessByClientIPAddressFromDb[0].ShouldCollectionBeEqual(auditDataListForLimitAccessByClientIPAddress, "Audit data should match");

                _newClientSearch.SearchByClientCodeToNavigateToClientProfileViewPage(ClientEnum.SMTST.ToString(),
                    ClientSettingsTabEnum.Security.GetStringValue());
                
                _newClientSearch.GetValueOfTextBox(label).ShouldBeEqual($"{remoteIp[0]}/32",
                    "When CIDR range is not entered then default CIDR should be 32");

                if (isRemoteTestServer)
                {
                    _newClientSearch.SetMultipleIpAddresses(remoteIp);
                    auditDataListForlistOfIPAddresses[4] = $"{remoteIp}/32";
                }
                else
                {
                    _newClientSearch.SetIp(localIp);
                    auditDataListForlistOfIPAddresses[4] = $"{localIp}/32";

                }

                _newClientSearch.GetSideWindow.SaveWithoutJs();

                StringFormatter.PrintMessage("Verify Audit For List Of IP Addresses");
                var auditDataForListOfIPAddressesFromDb = _newClientSearch.GetClientSettingAuditFromDatabase(ClientEnum.SMTST.ToString(), 1);
                lastModDate = Convert.ToDateTime(auditDataForListOfIPAddressesFromDb[0][2]).ToString("MM/dd/yyyy hh:mm tt").Trim();
                systemdate = DateTime.ParseExact(_newClientSearch.GetCommonSql.GetSystemDateFromDB(), "MM/dd/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);
                DateTime.ParseExact(lastModDate, "MM/dd/yyyy hh:mm tt", CultureInfo.InvariantCulture).AssertDateRange(systemdate.AddMinutes(-2),
                    systemdate.AddMinutes(1), "Last modified date should match");
                auditDataForListOfIPAddressesFromDb[0].RemoveAt(2);
                auditDataForListOfIPAddressesFromDb[0].ShouldCollectionBeEqual(auditDataListForlistOfIPAddresses, "Audit data should match");

                CurrentPage = CurrentPage.Logout().LoginAsClientUser();
                CurrentPage.PageTitle.ShouldBeEqual(PageTitleEnum.QuickLaunch.GetStringValue(),
                    "Login Successful as client user when logged in machine's ip is white listed");
                CurrentPage = _newClientSearch.Logout().LoginAsHciAdminUser();
                CurrentPage.PageTitle.ShouldBeEqual(PageTitleEnum.QuickLaunch.GetStringValue(),
                    "Login Successful as internal user when logged in machine's ip is white listed");

                _newClientSearch = CurrentPage.NavigateToClientSearch();
                _newClientSearch.SearchByClientCodeToNavigateToClientProfileViewPage
                    (ClientEnum.SMTST.ToString(), ClientSettingsTabEnum.Security.GetStringValue());

                StringFormatter.PrintMessage(
                    "Validating saved ips against database");
                var ipWithAndWithoutRange = new List<string>() {"121.0.0.1", "127.0.0.1/31"};

                _newClientSearch.SetMultipleIpAddresses(ipWithAndWithoutRange);
                _newClientSearch.GetSideWindow.Save(waitForWorkingMessage:true);
                _newClientSearch.GetSideWindow.ClickOnEditIcon();
                _newClientSearch.GetSideWindow.Cancel();
                _newClientSearch.GetSideWindow.ClickOnEditIcon();
                _newClientSearch.GetIpList(label).ShouldCollectionBeEqual(
                    _newClientSearch.GetWhiteListedIpFromDatabase(),
                    "Client Ip list should be equal to database values");

                Regex.IsMatch(_newClientSearch.GetIpList(label)[0],
                        @"^((((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)(\/([0-9]?|[1-2][0-9]?|3[0-2]?))?)?\,?)*)*$")
                    .ShouldBeTrue("IP is in format 127.0.0.1/32");

                StringFormatter.PrintMessageTitle(
                    "Verifying both internal and client user whether inside cotiviti's network or not, can't access Nucleus application when their ips are not white listed ");
                var login = CurrentPage.Logout();
                var url = login.LoginAndDoNotNavigate(EnvironmentManager.HciAdminUsername,
                    EnvironmentManager.HciAdminPassword);

                StringFormatter.PrintMessage("Verifying restricted Ip page for internal user");
                ValidateUrlIsRestricted(url);
                _newClientSearch.ClickOnBrowserBackButton();

                StringFormatter.PrintMessage("Verifying restricted Ip page for client user");
                url = login.LoginAndDoNotNavigate(EnvironmentManager.ClientUserName,
                    EnvironmentManager.ClientPassword);
                ValidateUrlIsRestricted(url);

                _newClientSearch.UpdateEnableIpFilterSettingToFalse(); //Revert Restrictions
                CurrentPage.ClickOnBrowserBackButton();
                login.LoginAndDoNotNavigate(EnvironmentManager.HciAdminUsername,
                    EnvironmentManager.HciAdminPassword);

                StringFormatter.PrintMessage(
                    "Verifying Cotiviti Users can't access Nucleus application when Allow All Cotiviti user setting is set false");
                _newClientSearch = CurrentPage.NavigateToClientSearch();
                _newClientSearch.SearchByClientCodeToNavigateToClientProfileViewPage(ClientEnum.SMTST.ToString(),
                    ClientSettingsTabEnum.Security.GetStringValue());

                _newClientSearch.ClickOnRadioButtonByLabel(label);
                _newClientSearch.SetIp("127.0.0.1");
                _newClientSearch.ClickOnRadioButtonByLabel(SecurityTabEnum.CotivitiAccessByIPAddress.GetStringValue(), false);
                _newClientSearch.GetSideWindow.SaveWithoutJs();

                StringFormatter.PrintMessage("Verify Audit For Cotiviti Access by IP address");
                var auditDataForCotivitiAccessByIPAddressFromDb = _newClientSearch.GetClientSettingAuditFromDatabase(ClientEnum.SMTST.ToString(), 3);
                lastModDate = Convert.ToDateTime(auditDataForCotivitiAccessByIPAddressFromDb[2][2]).ToString("MM/dd/yyyy hh:mm tt").Trim();
                systemdate = DateTime.ParseExact(_newClientSearch.GetCommonSql.GetSystemDateFromDB(), "MM/dd/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);
                DateTime.ParseExact(lastModDate, "MM/dd/yyyy hh:mm tt", CultureInfo.InvariantCulture).AssertDateRange(systemdate.AddMinutes(-2),
                    systemdate.AddMinutes(1), "Last modified date should match");
                auditDataForCotivitiAccessByIPAddressFromDb[2].RemoveAt(2);
                auditDataForCotivitiAccessByIPAddressFromDb[2].ShouldCollectionBeEqual(auditDataListForCotivitiAccessByIpAddress, "Audit data should match");

                login = _newClientSearch.Logout();
                url = login.LoginAndDoNotNavigate(EnvironmentManager.HciAdminUsername,
                    EnvironmentManager.HciAdminPassword);
                ValidateUrlIsRestricted(url);

                _newClientSearch.UpdateEnableIpFilterSettingToFalse();
                _newClientSearch.ClickOnBrowserBackButton();
                CurrentPage = Login.LoginAsHciAdminUser();
            }

           finally
            {
                _newClientSearch.UpdateEnableIpFilterSettingToFalse();
                if (_newClientSearch.GetPageHeader().Equals(PageTitleEnum.Login.GetStringValue()))
                {
                    QuickLaunch = Login.LoginAsHciAdminUser();
                }
            }
        }



        


        [Test]//TE-289
        public void Verify_microstrategy_option_to_be_displayed_in_Default_Dashboard_View_dropdown()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            var expectedDashboardViewProductTypeList = DataHelper
                .GetMappingData(FullyQualifiedClassName, "all_dashboard_view_product_type_list").Values.ToList();

            const string rolePrivilegesLabel = "Role Privileges";
            const string authorityPrivilegesLabel = "Authority Privileges";

            var values = Enum.GetValues(typeof(RoleEnum));
            var random = new Random();
            _profileManager = CurrentPage.NavigateToProfileManager();

            StringFormatter.PrintMessageTitle("Verify Display of Dashboard Dropdown when both role and authority privilage are not assigned.");
            _profileManager.ClickOnPrivileges();
            _profileManager.MoveAllValuesFromAssignedToAvailableByLabel(authorityPrivilegesLabel);
            _profileManager.MoveAllValuesFromAssignedToAvailableByLabel(rolePrivilegesLabel);
            _profileManager.ClickOnProfile();
            _profileManager.IsDashboardViewProductTypeDropdownPresent().ShouldBeFalse("Is Default Dashboard View displayed? ");

            StringFormatter.PrintMessageTitle("Verify Display of Dashboard Dropdown when only authority privilage is assigned.");
            _profileManager.ClickOnPrivileges();
            _profileManager.MoveOptionFromAvailableToAssigned(authorityPrivilegesLabel, AuthorityAssignedEnum.Reports.GetStringValue());
            _profileManager.ClickOnProfile();
            _profileManager.IsDashboardViewProductTypeDropdownPresent().ShouldBeFalse("Is Default Dashboard View displayed? ");

            StringFormatter.PrintMessageTitle("Verify Display of Dashboard Dropdown when only role privilage is assigned.");
            _profileManager.ClickOnPrivileges();
            _profileManager.MoveAllValuesFromAssignedToAvailableByLabel(authorityPrivilegesLabel);
            _profileManager.MoveOptionFromAvailableToAssigned(rolePrivilegesLabel, ((RoleEnum)values.GetValue(random.Next(values.Length))).GetStringValue());
            _profileManager.ClickOnProfile();
            _profileManager.IsDashboardViewProductTypeDropdownPresent().ShouldBeFalse("Is Default Dashboard View displayed? ");

            StringFormatter.PrintMessageTitle("Verify Display of Dashboard Dropdown when Report authority and any role privilage are assigned.");
            _profileManager.ClickOnPrivileges();
            _profileManager.MoveOptionFromAvailableToAssigned(authorityPrivilegesLabel, AuthorityAssignedEnum.Reports.GetStringValue());
            _profileManager.ClickOnProfile();
            _profileManager.IsDashboardViewProductTypeDropdownPresent().ShouldBeTrue("Is Default Dashboard View displayed? ");
            var dashboardViewProductType=_profileManager.GetDashboardViewProductType();
            dashboardViewProductType.ShouldContain("Microstrategy Dashboard",
                "Microstrategy Dashboard option must be listed in Default Dashboard View dropdown when Reports authority and any one Role is assigned to user");
            dashboardViewProductType.Count.ShouldBeEqual(2, "Only Microstrategy Dashboard must be listed");

            StringFormatter.PrintMessageTitle("Verify Display of Dashboard Dropdown when any role privilage and MyDashboard authority privilage is assigned.");
            _profileManager.ClickOnPrivileges();
            _profileManager.MoveAllValuesFromAssignedToAvailableByLabel(rolePrivilegesLabel);
            _profileManager.MoveAllValuesFromAssignedToAvailableByLabel(authorityPrivilegesLabel);
            _profileManager.MoveOptionFromAvailableToAssigned(rolePrivilegesLabel, ((RoleEnum)values.GetValue(random.Next(values.Length))).GetStringValue());
            _profileManager.MoveOptionFromAvailableToAssigned(authorityPrivilegesLabel, AuthorityAssignedEnum.MyDashboard.GetStringValue());
            _profileManager.ClickOnProfile();
            _profileManager.IsDashboardViewProductTypeDropdownPresent().ShouldBeTrue("Is Default Dashboard View displayed?");
            _profileManager.GetDashboardViewProductType().ShouldNotContain("Microstrategy Dashboard",
                "Is Microstrategy Dashboard option present in Default Dashboard View dropdown when Reports authority is not assigned even though any Role is assigned to user?");

            StringFormatter.PrintMessageTitle("Verify Display of Dashboard Dropdown when any role privilage and Product authority privilage is assigned.");
            _profileManager.ClickOnPrivileges();
            _profileManager.MoveAllValuesFromAssignedToAvailableByLabel(authorityPrivilegesLabel);
            _profileManager.MoveOptionFromAvailableToAssigned(authorityPrivilegesLabel,AuthorityAssignedEnum.ProductDashboards.GetStringValue());
            _profileManager.ClickOnProfile();
            _profileManager.IsDashboardViewProductTypeDropdownPresent().ShouldBeTrue("Is Default Dashboard View displayed?");
            _profileManager.GetDashboardViewProductType().ShouldNotContain("Microstrategy Dashboard",
                "Is Microstrategy Dashboard option present in Default Dashboard View dropdown when Reports authority is not assigned even though any Role is assigned to user?");

            StringFormatter.PrintMessageTitle("Verify Display of Dashboard Dropdown when all role and authority privilages are assigned.");
            expectedDashboardViewProductTypeList.Insert(0, "");
            _newUserProfileSearchPage = _profileManager.NavigateToNewUserProfileSearch();
            CurrentPage = _profileManager =
                _newUserProfileSearchPage.ClickonUserNameToNavigateProfileManagerPage("Mstr", "Automation");
            _profileManager.ClickOnPrivileges();
            _newUserProfileSearchPage.SelectRandomRolePrivilege();
            _profileManager.ClickOnProfile();
            _profileManager.GetDashboardViewProductType()
                .ShouldCollectionBeEqual(expectedDashboardViewProductTypeList,
                    "Microstrategy option is listed in Dashboard View Product List");
            _profileManager.ClickOnPrivileges();
            _newUserProfileSearchPage.MoveAllValuesFromAvailabelToAssignedByLabel("Role Privileges");
            _profileManager.ClickOnProfile();
            _profileManager.GetDashboardViewProductType()
                .ShouldCollectionBeEqual(expectedDashboardViewProductTypeList,
                    "Microstrategy option is listed in Dashboard View Product List");
        }
        #endregion

       
        [Test] //TE-433

        public void Verify_Default_Page_SuspectProvider_page()
        {
            StringFormatter.PrintMessage("Verify default page set");
            var _newUserProfileSearch = CurrentPage.NavigateToNewUserProfileSearch();
            _newUserProfileSearch.SearchUserByNameOrId(new List<string> { EnvironmentManager.HciAdminUserWithSuspectProviderDefaultPage }, true);
            _newUserProfileSearch.ClickOnGridRowByUserIdToOpenUserSettingSideView(EnvironmentManager.HciAdminUserWithSuspectProviderDefaultPage);
            _newUserProfileSearch.ClickOnUserSettingTabByTabName(UserSettingsTabEnum.Preferences.GetStringValue());
            _newUserProfileSearch.GetInputTextBoxValueByLabel(UserPreferencesEnum.DefaultPage
                    .GetStringValue()).ShouldBeEqual(PageHeaderEnum.SuspectProviders.GetStringValue(),
                "default page set to suspect Provider?");

            _newUserProfileSearch.SearchUserByNameOrId(new List<string> { EnvironmentManager.ClientUserWithSuspectProviderDefaultPage }, true);
            _newUserProfileSearch.ClickOnGridRowByUserIdToOpenUserSettingSideView(EnvironmentManager.ClientUserWithSuspectProviderDefaultPage);
            _newUserProfileSearch.ClickOnUserSettingTabByTabName(UserSettingsTabEnum.Preferences.GetStringValue());
            _newUserProfileSearch.GetInputTextBoxValueByLabel(UserPreferencesEnum.DefaultPage
                .GetStringValue()).ShouldBeEqual(PageHeaderEnum.SuspectProviders.GetStringValue(),
                "default page set to suspect Provider?");

            QuickLaunch.Logout().LoginWithDefaultSuspectProvidersPage();
            CurrentPage.WaitForPageToLoad();
            var suspectProvider = EnvironmentManager.ApplicationUrl +
                                  PageUrlEnum.SuspectProviders.GetStringValue();
            CurrentPage.CurrentPageUrl.ShouldBeEqual(suspectProvider, "Page Url Should be Suspect Provider");
            CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.SuspectProviders.GetStringValue(),"Page Header should match");

            QuickLaunch.Logout().LoginClientUserWithDefaultSuspectProvidersPage();
            CurrentPage.WaitForPageToLoad();
            
            CurrentPage.CurrentPageUrl.ShouldBeEqual(suspectProvider, "Page Url Should be Suspect Provider");
            CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.SuspectProviders.GetStringValue(), "Page Header should match");


        }

        [Test] //TE-433
        public void Verify_Default_Page_ProviderSearch_page()
        {
            StringFormatter.PrintMessage("Verify default page set");
            var _newUserProfileSearch = CurrentPage.NavigateToNewUserProfileSearch();
            _newUserProfileSearch.SearchUserByNameOrId(new List<string> { EnvironmentManager.HciAdminUserWithProviderSearchDefaultPage }, true);
            _newUserProfileSearch.ClickOnGridRowByUserIdToOpenUserSettingSideView(EnvironmentManager.HciAdminUserWithProviderSearchDefaultPage);
            _newUserProfileSearch.ClickOnUserSettingTabByTabName(UserSettingsTabEnum.Preferences.GetStringValue());
            _newUserProfileSearch.GetInputTextBoxValueByLabel(UserPreferencesEnum.DefaultPage
                    .GetStringValue()).ShouldBeEqual(PageHeaderEnum.ProviderSearch.GetStringValue(),
                "default page set to Provider?");

            _newUserProfileSearch.SearchUserByNameOrId(new List<string> { EnvironmentManager.ClientUserWithProviderSearchDefaultPage }, true);
            _newUserProfileSearch.ClickOnGridRowByUserIdToOpenUserSettingSideView(EnvironmentManager.ClientUserWithProviderSearchDefaultPage);
            _newUserProfileSearch.ClickOnUserSettingTabByTabName(UserSettingsTabEnum.Preferences.GetStringValue());
            _newUserProfileSearch.GetInputTextBoxValueByLabel(UserPreferencesEnum.DefaultPage
                    .GetStringValue()).ShouldBeEqual(PageHeaderEnum.ProviderSearch.GetStringValue(),
                "default page set to Provider?");

            QuickLaunch.Logout().LoginWithDefaultProviderSearchPage();
            CurrentPage.WaitForPageToLoad();
            var provider = EnvironmentManager.ApplicationUrl + PageUrlEnum.ProviderSearch.GetStringValue();
            CurrentPage.CurrentPageUrl.ShouldBeEqual(provider, "Page Url Should be Suspect Provider");
            CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.ProviderSearch.GetStringValue(), "Page Header should match");
            
            QuickLaunch.Logout().LoginClientUserWithDefaultProviderSearchPage();
            CurrentPage.WaitForPageToLoad();
            
            CurrentPage.CurrentPageUrl.ShouldBeEqual(provider, "Page Url Should be Provider Search ");
            CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.ProviderSearch.GetStringValue(), "Page Header should match");




        }

        [Test, Category("SmokeTest")] //CAR-2902(CAR-2744) + CAR-2914(CAR-2837)
        public void Verify_User_Name_Greeting_Menu_And_Dropdown()
        {
            var dropDownListOptions = Enum.GetValues(typeof(UserGreetingMenuEnum))
                .Cast<UserGreetingMenuEnum>().Select(x => x.GetStringValue()).ToList();
            var quickLinksMenuOptions = Enum.GetValues(typeof(QuickLinksEnum))
                .Cast<QuickLinksEnum>().Select(x => x.GetStringValue()).ToList();
            var cotivitiLinksMenuOptions = Enum.GetValues(typeof(CotivitiLinksEnum))
                .Cast<CotivitiLinksEnum>().Select(x => x.GetStringValue()).ToList();
            CurrentPage.IsUserNameGreetingMenuPresent().ShouldBeTrue("User name Greeting Menu Should be present in the right corner of the page");
            StringFormatter.PrintMessage("Verify Dropdown label");
            CurrentPage.GetUserNameGreetingMenuDropdownLabel().ShouldBeEqual($"Hi, {CurrentPage.GetLoggedInUserFullName().Split(' ')[0]}");
            StringFormatter.PrintMessage("Verify Presence of Drop Down Options");
            CurrentPage.GetUserNameGreetingMenuDropdownOptions().ShouldCollectionBeEqual(dropDownListOptions,"Drop Down list Options Should Match");
            CurrentPage.GetQuickLinksMenuOptions().ShouldCollectionBeEqual(quickLinksMenuOptions, "Quick links list should match");
            CurrentPage.GetCotivitiLinksMenuOptions().ShouldCollectionBeEqual(cotivitiLinksMenuOptions,"Cotiviti links list should match");
            StringFormatter.PrintMessage("Verify Only Claim Search Menu is visible and Basic dropdown is visible to users without any privilege");
            CurrentPage.Logout().LoginAsUserHavingNoAnyAuthority();
            dropDownListOptions.Remove("Dashboard");
            CurrentPage.GetTopNavigationMenuOptions().ShouldCollectionBeEqual(new List<string>(){"Claim"},"Only Claim Search Menu should be present for user without any authority");
            CurrentPage.GetUserNameGreetingMenuDropdownOptions().ShouldCollectionBeEqual(dropDownListOptions,"Basic dropdown option present for users without any authority");
            StringFormatter.PrintMessage("Verify Cotiviti and Client Flagged providers are not present for user without Provider Maintenance authority");
            CurrentPage.MouseOverQuickLinks();
            CurrentPage.GetQuickLinksMenuOptions().ShouldNotContain(QuickLinksEnum.CotivitiFlaggedProviders.GetStringValue(),$"List should not contain {QuickLinksEnum.CotivitiFlaggedProviders.GetStringValue()}");
            CurrentPage.GetQuickLinksMenuOptions().ShouldNotContain(QuickLinksEnum.ClientFlaggedProviders.GetStringValue(), $"List should not contain {QuickLinksEnum.ClientFlaggedProviders.GetStringValue()}");



        }






        #region private fields

        private void ValidateDefaultPageBasedOnDefaultClient(string client)
        {
            _userProfileSearch.SelectDefaultClient(client);
            _userProfileSearch.WaitForDefaultPageRefreshWhenClientChanged();
            _userProfileSearch.GetDefaultPageList(true).ShouldNotContain("Batch Search", "Batch search option should not present in Default page dropdown?");
        }

        private void ValidateAuthority(bool authorityPresent, string privilege, bool rolePresent, string role)
        {
            if (authorityPresent)
                _profileManager.IsAuthorityAssigned(privilege)
                    .ShouldBeTrue(string.Format("{0} authority must be provided for current user", privilege));
            else
            {
                _profileManager.IsAuthorityAssigned(privilege)
                    .ShouldBeFalse(string.Format("{0} authority must not be provided for current user", privilege));
            }

            if (rolePresent)
                _profileManager.IsRoleAssigned(role)
                    .ShouldBeTrue(string.Format("{0} role must be provided for current user", role));
            else
            {
                _profileManager.IsRoleAssigned(role)
                    .ShouldBeFalse(string.Format("{0} role must not be provided for current user", role));

            }

        }

        private static void ValidateUrlIsRestricted(string url)
        {
            if (url.ToLower().Contains("dev.nucleus") || url.ToLower().Contains("dev3.nucleus") || url.ToLower().Contains("dev1.nucleus") ||
                url.ToLower().Contains("qa.nucleus"))
            {
                url.ToLower().Contains("authentications/create")
                    .ShouldBeTrue("Is Create Authentication page opened?");
            }
            else
            {
                url.ToLower().Contains("/errors/restrictedip")
                    .ShouldBeTrue("Is restricted page opened?");
            }
        }
        //private void UpdatePasswordAndSecurityQuestionAndVefiy(string[] secQuestion, string pwd, string message)
        //{
        //    UpdateQuestionAndPassword(secQuestion, pwd);
        //    _profileManager.ClickSaveButton();
        //    _profileManager.IsPageErrorPopupModalPresent().ShouldBeFalse("Password should be updated successfully");
        //    CurrentPage.GetPageHeader()
        //        .ShouldBeEqual(Extensions.GetStringValue(PageHeaderEnum.QuickLaunch),
        //            string.Format("Password should be successfully updated when {0}", message));
        //    _profileManager = QuickLaunch.NavigateToProfileManager();
        //    _profileManager.ClickOnSecurityTab();
        //    _profileManager.GetSelectedSecurityQuestion()
        //        .ShouldBeEqual(secQuestion[0],
        //            string.Format("Security Question 1 should be successfully updated when {0}", message));
        //    _profileManager.GetSelectedSecurityQuestion(false)
        //        .ShouldBeEqual(secQuestion[1],
        //            string.Format("Security Question 2 should be successfully updated when {0}", message));

        //}

        private void UpdateQuestionAndPassword(string[] secQuestion, string pwd)
        {
            _profileManager.SelectQuestionFromList(secQuestion[0]);
            _profileManager.SetSecurityAnswer1("k");
            _profileManager.SelectQuestionFromList(secQuestion[1], false);
            _profileManager.SetSecurityAnswer2("k");
            _profileManager.InsertNewPassword(pwd);
            _profileManager.InsertConfirmNewPassword(pwd);
        }


        private void FillUserDetails(OldUserProfileSearchPage userProfileSearch, List<string> UserDetails)
        {
            userProfileSearch.InsertFirstName(UserDetails[0]);
            userProfileSearch.InsertLastName(UserDetails[1]);
            userProfileSearch.InsertPhoneNo(UserDetails[2]);
            userProfileSearch.InsertEmailAddress(UserDetails[3]);
            userProfileSearch.InsertUserId(UserDetails[4]);
            userProfileSearch.SelectUserType(UserDetails[5]);
            userProfileSearch.InsertPassword(UserDetails[6]);
            userProfileSearch.InsertConfirmPassword(UserDetails[7]);
            userProfileSearch.SelectUserStatus(UserDetails[8]);
        }

        //private void VerifyBatchMenuAndSubMenuArePresent()
        //{
        //    StringFormatter.PrintMessageTitle("Verify Batch Menu and Sub Menu are accessible through retro page");
        //    QuickLaunch.GetHeaderMenuText(5).ShouldBeEqual(HeaderMenu.Batch);
        //    Mouseover.MouseOverBatchMenu();         
        //    QuickLaunch.GetSubMenuOption(5, 1).ShouldBeEqual(SubMenu.BatchSearch, "1st submenu");
            
        //    StringFormatter.PrintMessageTitle("Verify Batch Menu and Sub Menu are accessible through core page");
        //    var appealSearch = QuickLaunch.NavigateToAppealSearch();

        //    appealSearch.WaitForPageToLoadWithSideBarPanel();

        //    appealSearch.GetSideBarPanelSearch.ClickOnSideBarHeader();

        //    appealSearch.GetHeaderMenuText(5).ShouldBeEqual(HeaderMenu.Batch);
        //    Mouseover.MouseOverBatchMenu();
        //    appealSearch.GetSubMenuOption(5, 1).ShouldBeEqual(SubMenu.BatchSearch, "1st submenu");
        //    QuickLaunch = appealSearch.ClickOnQuickLaunch();
        //}

        private void UpdateAndCheckProcessingTypeOfClient(ProcessingType type)
        {
            QuickLaunch.GetCommonSql.UpdateProcessingTypeOfClient(type.ToString(), ClientEnum.SMTST.ToString());
            CurrentPage = _clientSearch = QuickLaunch.NavigateToClientSearch();

            _clientSearch.SearchByClientCodeToNavigateToClientProfileViewPage(ClientEnum.SMTST.ToString(), ClientSettingsTabEnum.General.GetStringValue());

            _clientSearch.GetSideWindow.GetDropDownInputFieldByLabel(GeneralTabEnum.ProcessingType.GetStringValue())
                .ShouldBeEqual(type.GetStringValue(), $"For SMTST client the Processing Type equals to '{type.GetStringValue()}'");
        }

        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        //public void SelectRandomRolePrivilege()
        //{
        //    string[] rolePrivilege = _userProfileSearch.GetAllAvailableRolesList();
        //    Random r = new Random();
        //    int roleIndex = r.Next(0, rolePrivilege.Length - 1);
        //    _userProfileSearch.MoveOptionFromAvailableToAssigned("Role Privileges", rolePrivilege[roleIndex]);

        //}

        //public void SelectAllRolePrivilege()
        //{  }
        


        #endregion
    }
}



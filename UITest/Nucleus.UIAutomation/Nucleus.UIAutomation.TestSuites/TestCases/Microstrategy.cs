using Nucleus.UIAutomation.TestSuites.Base;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nucleus.Service.PageServices.Microstrategy;
using NUnit.Framework;
using System.Diagnostics;
using Nucleus.Service.Data;
using Nucleus.Service.PageObjects.Settings.User;
using Nucleus.Service.PageServices.Dashboard;
using Nucleus.Service.PageServices.Settings.User;
using Nucleus.Service.Support.Common.Constants;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Environment;
using Nucleus.Service.Support.Menu;
using Nucleus.Service.Support.Utils;
using Nucleus.UIAutomation.TestSuites.Common;

using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Core.Navigation;
using Nucleus.Service.PageObjects.Microstrategy;
using Nucleus.Service.PageServices.QuickLaunch;

namespace Nucleus.UIAutomation.TestSuites.TestCases
{
    public class Microstrategy : NewAutomatedBase

    {
        #region PRIVATE FIELDS

        private MicrostrategyPage _microstrategy;
        private MicrostrategyReportPage _microstrategyhome;
        private CommonValidations _commonValidation;
        private readonly string _reportPrivilege = AuthorityAssignedEnum.Reports.GetStringValue();
        //private readonly string _executiveRole = RoleEnum.Executive.GetStringValue();
        //private readonly string _managerRole = RoleEnum.Manager.GetStringValue();
        private readonly string _appealCategory = RoleEnum.AppealCategoryReadOnlyOnly.GetStringValue();
        private readonly string _clinicalValidation = RoleEnum.ClinicalValidationReadOnly.GetStringValue();
        //private readonly string _directorRole = RoleEnum.Director.GetStringValue();
        private readonly string _analystRole = RoleEnum.ClaimsProcessor.GetStringValue();
        private ProfileManagerPage _profileManager;
        private DashboardPage _dashboard;
        private UserProfileSearchPage _userProfileSearch;


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
                UserLoginIndex = 6;
                base.ClassInit();
                //CurrentPage = _microstrategy = QuickLaunch.NavigateToMicrostrategy();

            }
            catch (Exception)
            {
                if (StartFlow != null)
                    StartFlow.Dispose();
                throw;
            }
        }

        protected override void TestCleanUp()
        {
            if (string.Compare(UserType.CurrentUserType, UserType.MstrUser, StringComparison.OrdinalIgnoreCase) != 0)
            {

                QuickLaunch = _microstrategy.Logout().LoginAsMstrUser1();

            }
            else 
            {
                CurrentPage.ClickOnQuickLaunch();
            }
            base.TestCleanUp();

        }

        protected override void ClassCleanUp()
        {
            //if (CurrentPage.IsQuickLaunchIconPresent())
            //    _microstrategy.ClickOnQuickLaunch();
            base.ClassCleanUp();
        }

        #endregion

        #region TEST SUITES

        //[Test]//TE-239
        //public void Verify_iframe_test()
        //{
        //    _microstrategyhome = QuickLaunch.ClickOnDashboardIconToNavigateToMicrostrategyHome();
        //    _microstrategyhome.SwitchToIframe();
        //    _microstrategyhome.ClickOnQuickLaunch();
        //}

        [Test]//TE-239
        public void Verify_default_navigation_to_microstrategy_and_microstrategy_option_in_dashboard_menu()
        {

            CurrentPage.Logout().LoginAsHciAdminUser();
            _userProfileSearch = CurrentPage.NavigateToNewUserProfileSearch();
            CurrentPage.IsRoleAssigned<UserProfileSearchPage>(new List<string> { EnvironmentManager.MstrtUser1 }, RoleEnum.MicroStrategy.GetStringValue(),isDefaultPageUserProfile:true).ShouldBeTrue(
                $"Is MicroStrategy present for user<{EnvironmentManager.MstrtUser1}>");
            _userProfileSearch.ClickOnUserSettingTabByTabName(UserSettingsTabEnum.Preferences.GetStringValue());
            _userProfileSearch.GetInputTextBoxValueByLabel(UserPreferencesEnum.DefaultDashboard.GetStringValue())
                .ShouldBeEqual("Microstrategy","Is Default Dashboard is Microstrategy?");

            _userProfileSearch.ClickOnUserSettingTabByTabName(UserSettingsTabEnum.RolesAuthorities.GetStringValue());
            _userProfileSearch.ClickOnEditIcon();
            CurrentPage.IsAvailableAssignedRowPresent(1, RoleEnum.ClaimsProcessor.GetStringValue(),false).ShouldBeTrue(
                $"Claim Processor(All Dashboard) should not present for current user<{EnvironmentManager.MstrtUser1}>");

            _userProfileSearch.ClickOnUserSettingTabByTabName(UserSettingsTabEnum.Preferences.GetStringValue());
            _userProfileSearch.GetDropDownListForUserProfileSettingsByLabel(UserPreferencesEnum.DefaultDashboard
                    .GetStringValue()).ShouldCollectionBeEqual(new List<string>{
                "","Microstrategy"
            }, "Is only Microstrategy present?");



            CurrentPage.IsRoleAssigned<UserProfileSearchPage>(new List<string> { EnvironmentManager.HciAdminUsername }, RoleEnum.MicroStrategy.GetStringValue()).ShouldBeTrue(
                $"Is MicroStrategy present for user<{EnvironmentManager.HciAdminUsername}>");

            _userProfileSearch.ClickOnUserSettingTabByTabName(UserSettingsTabEnum.Preferences.GetStringValue());
            _userProfileSearch.ClickOnEditIcon();
            "Microstrategy".AssertIsContainedInList(
                _userProfileSearch.GetDropDownListForUserProfileSettingsByLabel(UserPreferencesEnum.DefaultDashboard
                    .GetStringValue()), "Is Default Dashboard is Microstrategy?");

           
            _dashboard = CurrentPage.NavigateToDashboard();
            _dashboard.IsMicrostrategyOptionPresentInDashboardMenu().ShouldBeTrue("Is microstrategy option present?");
            _microstrategy = _dashboard.SelectMicrostrategyFilterOptions();
            _microstrategy.GetTitle().ShouldBeEqual(PageTitleEnum.Microstrategy.GetStringValue(), "Is User navigated to Microstrategy page?");

            CurrentPage.Logout().LoginAsMstrUser1();

            //_profileManager = QuickLaunch.NavigateToProfileManager();
            //_profileManager.IsRespectiveAuthorityAssigned(_reportPrivilege, "Read-Write")
            //    .ShouldBeTrue("Reports r/w authority is present for current user:" + EnvironmentManager.Username);
            //_profileManager.AnyRoleIsAssigned().ShouldBeTrue("Any Role should be assigned for current user:" + EnvironmentManager.Username);
            //_profileManager.ClickOnProfile();
            //_profileManager.GetDashboardViewProductType().ShouldContain("Microstrategy Dashboard",
            //    "Microstrategy Dashboard option must be listed in Default Dashboard View list");
            ////_profileManager.SetDefaultDashboardViewProductType("Microstrategy Dashboard");
            ////_profileManager.ClickSaveButton();
            _microstrategyhome = QuickLaunch.ClickOnDashboardIconToNavigateToMicrostrategyHome();
            _microstrategyhome.IsMicrostrategyOptionPresentInDashboardMenu().ShouldBeTrue("Is microstrategy option present?");
            _microstrategyhome.GetTitle().ShouldBeEqual(PageTitleEnum.Microstrategy.GetStringValue(),
                "When Microstrateggy is selected for default Dashboard View then user should navigate to Microstragy upon clicking dashboard icon");

            ////login as user with default dashboard view is other than Microstrategy
            //_profileManager = _microstrategyhome.Logout().LoginAsHciAdminUser().NavigateToProfileManager();
            //_profileManager.IsRespectiveAuthorityAssigned(_reportPrivilege, "Read-Write")
            //    .ShouldBeTrue("Reports r/w authority is present for current user:" + EnvironmentManager.Username);
            ////_profileManager.IsRoleAssigned(_executiveRole)
            ////    .ShouldBeTrue("Executive authority is present for current user:" + EnvironmentManager.Username);
            //_profileManager.ClickOnProfile();
            //_profileManager.GetDashboardViewProductType().ShouldContain("Microstrategy Dashboard",
            //    "Microstrategy Dashboard option must be listed in Default Dashboard View list");




            //login as user without Reports privilege and Any Role (uiautomation7)
            //_dashboard = CurrentPage.Logout().LoginAsHciUserWithManageEditAuthority().NavigateToDashboard();
            //_dashboard.IsMicrostrategyOptionPresentInDashboardMenu().ShouldBeFalse(
            //    "Microstrategy option should not be present for user without Reports Privilege");

          
        }
        [Test,Category("Working")]//TE-239
        public void Verify_Correct_Details_Displayed_And_Navigation_To_Microstrategy_Home()
        {         
           TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            IDictionary<string, string> paramLists = DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
            var columnHeader = paramLists["LabelName"].Split(',').ToList();

            try
            {


                _userProfileSearch = QuickLaunch.Logout().LoginAsMstrUserwithManagerRole().NavigateToNewUserProfileSearch();

                CurrentPage.IsRoleAssigned<UserProfileSearchPage>(new List<string> { EnvironmentManager.MstrtUserWithManageRole }, RoleEnum.MicroStrategy.GetStringValue(), isDefaultPageUserProfile: true).ShouldBeTrue(
                    $"Is MicroStrategy present for user<{EnvironmentManager.MstrtUser1}>");

                _dashboard = _profileManager.NavigateToDashboard();

                _microstrategy = _dashboard.NavigateToMicrostrategyWithMultipleReports();


                StringFormatter.PrintMessage("Verify headers displayed");

                _microstrategy.GetGridViewSection.GetLabelInGridByColRow(1, 1)
                    .ShouldBeEqual(columnHeader[0], "reportname header value verified");
                _microstrategy.GetGridViewSection.GetLabelInGridByColRow(2, 1)
                    .ShouldBeEqual(columnHeader[1], "Role header value verified");
                _microstrategy.GetGridViewSection.GetLabelInGridByColRow(3, 1)
                    .ShouldBeEqual(columnHeader[2], "Created By header value verified");
                _microstrategy.GetGridViewSection.GetLabelInGridByColRow(4, 1)
                    .ShouldBeEqual(columnHeader[3], "Create date header value verified");

                _microstrategy.GetGridViewSection.GetGridRowCount()
                    .ShouldBeEqual(_microstrategy.GetCountOfReportsAssigendtoUser(EnvironmentManager.Username));
                StringFormatter.PrintMessage("Verify correct report name displayed");
                var expectedList = _microstrategy.GetReportInfoFromDatabase(EnvironmentManager.Username);

                _microstrategy.GetGridViewSection.GetGridListValueByCol(1)
                    .ShouldCollectionBeEqual(expectedList.Select(x => x[0]).ToList(), "Report Name equal?");
                _microstrategy.GetGridViewSection.GetGridListValueByCol(2)
                    .ShouldCollectionBeEqual(expectedList.Select(x => x[1]).ToList(), "Role equal?");
                _microstrategy.GetGridViewSection.GetGridListValueByCol(3)
                    .ShouldCollectionBeEqual(expectedList.Select(x => x[2]).ToList(), "created by equal?");
                _microstrategy.GetGridViewSection.GetGridListValueByCol(4)
                    .ShouldCollectionBeEqual(expectedList.Select(x => x[3]).ToList(), "created date equal?");



                StringFormatter.PrintMessage("Verify navigation to Microstratergy home");
                _microstrategyhome = _microstrategy.ClickOnViewReportsAndNavigatetohome();
                _microstrategyhome.IsSelectProductOptionpresent().ShouldBeTrue("ReportHome option displayed?");
                StringFormatter.PrintMessage("verify navigation back to the microstratergy page");
                _microstrategyhome.ClickOnMicrostrategyDashboardTONavigateToMicrostratergyPage();
                StringFormatter.PrintMessage("Navigated back to microstratergy page");

                _profileManager = QuickLaunch.Logout().LoginAsUserHavingManageAppealRationaleReadOnlyAuthortiy()
                    .NavigateToProfileManager();
                _profileManager.ClickOnPrivileges();
                _profileManager.IsRoleAssigned(_appealCategory).ShouldBeTrue("provider Relations Role Assigned?");
                _profileManager.IsRoleAssigned(_clinicalValidation).ShouldBeTrue("Quality Assurance Role Assigned?");
                _dashboard = _profileManager.NavigateToDashboard();
                _microstrategy = _dashboard.NavigateToMicrostrategyWithMultipleReports();



                if (_microstrategy.checkRolesToReportsMappingfromDatabase(_appealCategory, _clinicalValidation))
                {
                    _microstrategy.IsMstrNoDataMessageAvailable()
                        .ShouldBeFalse("Message Displayed when reports are not assigned to the user");
                    _microstrategy.UpdateReportToRoleMapping(_appealCategory, _clinicalValidation,'F');
                    _microstrategy.Reload();
                }

                _microstrategy.IsMstrNoDataMessageAvailable()
                    .ShouldBeTrue("Message Displayed when reports are not assigned to the user");

            }
            finally
            {
                _microstrategy.UpdateReportToRoleMapping(_appealCategory, _clinicalValidation);
            }

        }

        //[Test] //TE-529
        //public void Verify_Switch_Client_Functionality_For_MSTR_Page()
        //{
        //    try
        //    {
        //        CurrentPage.Logout().LoginAsHciAdminUser();
        //        var mostRecentClientData =
        //            QuickLaunch.GetMostRecentClientDetailsFromDatabase(EnvironmentManager.Username);
        //        var allClientData = QuickLaunch.GetAllClientDetailsFromDatabase(EnvironmentManager.Username);
        //        _profileManager = CurrentPage.NavigateToProfileManager();
        //        var defaultPage = _profileManager.GetDefaultPagePreference();
        //        _dashboard = _profileManager.NavigateToDashboard();
        //        _microstrategy = _dashboard.NavigateToMicrostrategyWithMultipleReports();

        //        StringFormatter.PrintMessage("Verify Switch Client Icon in Core Page");
        //        _microstrategy.IsSwitchClientIconPresent()
        //            .ShouldBeTrue("Switch Client Icon Should Be Present in core page");
        //        _microstrategy.SwitchClientSection.ClickOnSwitchClient();

        //        StringFormatter.PrintMessage("Verify Presence of Switch Client Window");
        //        _microstrategy.SwitchClientSection.IsSwitchClientSideWindowPresent()
        //            .ShouldBeTrue("Switch Client Side Window Should Be Opened");

        //        _microstrategy.SwitchClientSection.ClickOnCloseButton();
        //        _microstrategy.SwitchClientSection.IsSwitchClientSideWindowPresent()
        //            .ShouldBeFalse("Switch Client Side Window Should Be Closed");

        //        _microstrategy.SwitchClientSection.ClickOnSwitchClient();
        //        _microstrategy.SwitchClientSection.IsMostRecentPageHeaderPresent()
        //            .ShouldBeTrue("Most Recent Page Header Should Be Present");
        //        _microstrategy.SwitchClientSection.GetMostRecentClientCodes()
        //            .ShouldCollectionBeEqual(mostRecentClientData.Select(x => x[0]).ToList(),
        //                "Client Codes Should Match");
        //        _microstrategy.SwitchClientSection.GetMostRecentClientNames()
        //            .ShouldCollectionBeEqual(mostRecentClientData.Select(x => x[1]).ToList(),
        //                "Client Names Should Match");
        //        _microstrategy.SwitchClientSection.IsAllClientsPageHeaderPresent()
        //            .ShouldBeTrue("All Clients Page Header Should Be Present");
        //        _microstrategy.SwitchClientSection.GetAllClientCodes()
        //            .ShouldCollectionBeEqual(allClientData.Select(x => x[0]).ToList(), "Client Codes Should Match");
        //        _microstrategy.SwitchClientSection.GetAllClientNames()
        //            .ShouldCollectionBeEqual(allClientData.Select(x => x[1]).ToList(), "Client Names Should Match");

        //        _microstrategy.SwitchClientSection.SwitchClientTo(ClientEnum.TTREE);
        //        _microstrategy.WaitForPageToLoad();
        //        CurrentPage.GetPageHeader().ShouldBeEqual(defaultPage,
        //            "User should be navigated to default page after client switch");
        //        _dashboard = CurrentPage.NavigateToDashboard();
        //        _microstrategy = _dashboard.NavigateToMicrostrategyWithMultipleReports();
        //        _microstrategy.WaitForPageToLoad();
        //        CurrentPage.IsClientLogoPresent(ClientEnum.TTREE,true);


        //        StringFormatter.PrintMessage("Verify Navigation to Default Core Page After Client Switch");
        //        QuickLaunch.Logout().LoginWithDefaultSuspectProvidersPage();
        //        CurrentPage.WaitForPageToLoad();
        //        _profileManager = CurrentPage.NavigateToProfileManager();
        //        var defaultCorePage = _profileManager.GetDefaultPagePreference();
        //        _dashboard = CurrentPage.NavigateToDashboard();
        //        _microstrategy = _dashboard.NavigateToMicrostrategyWithMultipleReports();
        //        _microstrategy.SwitchClientSection.ClickOnSwitchClient();
        //        _microstrategy.SwitchClientSection.SwitchClientTo(ClientEnum.TTREE);
        //        CurrentPage.GetPageHeader().ShouldBeEqual(defaultCorePage,
        //            "User should be navigated to default page after client switch");
        //        CurrentPage.IsClientLogoPresent(ClientEnum.TTREE);
        //    }
        //    finally
        //    {
        //        CurrentPage.Logout().LoginAsMstrUser1();
        //    }
        //}




        #endregion


        #region PRIVATE METHODS 

       

        #endregion

    }
}

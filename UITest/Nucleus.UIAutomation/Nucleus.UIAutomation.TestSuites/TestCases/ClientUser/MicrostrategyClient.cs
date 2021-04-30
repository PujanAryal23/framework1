using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Nucleus.Service.Data;
using Nucleus.Service.PageServices.Dashboard;
using Nucleus.Service.PageServices.Microstrategy;
using Nucleus.Service.PageServices.Settings.User;
using Nucleus.Service.Support.Common.Constants;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Environment;
using Nucleus.Service.Support.Utils;
using Nucleus.UIAutomation.TestSuites.Base;
using Nucleus.UIAutomation.TestSuites.Common;
using NUnit.Framework;
using UIAutomation.Framework.Core.Driver;

namespace Nucleus.UIAutomation.TestSuites.TestCases.ClientUser
{
    public class MicrostrategyClient: AutomatedBaseClient
    {
        #region PRIVATE FIELDS

        private MicrostrategyPage _microstrategy;
        private MicrostrategyReportPage _microstrategyhome;
        private CommonValidations _commonValidation;
        private readonly string _reportPrivilege = AuthorityAssignedEnum.Reports.GetStringValue();
        //private readonly string _executiveRole = RoleEnum.Executive.GetStringValue();
        private ProfileManagerPage _profileManager;
        private DashboardPage _dashboard;
        //private readonly string _managerRole = RoleEnum.Manager.GetStringValue();
        //private readonly string _directorRole = RoleEnum.Director.GetStringValue();
        //private readonly string _providerRelations = RoleEnum.ProviderRelations.GetStringValue();
        //private readonly string _qualityAssurance = RoleEnum.QualityAssurance.GetStringValue();
        //private readonly string _analystRole = RoleEnum.Analyst.GetStringValue();
        


        #endregion

        #region OVERRIDE METHODS
        protected override void ClassInit()
        {
            try
            {
                UserLoginIndex = 3;
                base.ClassInit();

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
            base.TestInit();
           // CurrentPage = _microstrategy;
        }

        protected override void TestCleanUp()
        {
            if (string.Compare(UserType.CurrentUserType, UserType.ClientMstrUser, StringComparison.OrdinalIgnoreCase) != 0)
            {

                QuickLaunch = _microstrategy.Logout().LoginAsClientMstrUser();

            }
            else 
            {
                CurrentPage.ClickOnQuickLaunch();
            }
            base.TestCleanUp();
        }
        #endregion

        #region PROTECTED PROPERTIES

        protected override string FullyQualifiedClassName
        {
            get
            {
                return GetType().FullName;
            }
        }
        #endregion

        #region TEST SUITES

        [Test]//TE-239 + //TE-289
        public void Verify_default_navigation_to_microstrategy_and_microstrategy_option_in_dashboard_menu()
        {
            _profileManager = QuickLaunch.NavigateToProfileManager();
            _profileManager.ClickOnProfile();
            _profileManager.IsAnyRoleAssignedToUserFromDatabase(EnvironmentManager.ClientMstrtUser).ShouldBeTrue("Is role assigned?");
            _profileManager.IsPrivilegeAssignedFromDatabase(EnvironmentManager.ClientMstrtUser, _reportPrivilege).ShouldBeTrue("Is Report Privilege assigned?");
            _profileManager.GetDashboardViewProductType().ShouldContain("Microstrategy Dashboard",
                "Microstrategy Dashboard option must be listed in Default Dashboard View list");
            _profileManager.GetSelectedDashboardViewProductType().ShouldBeEqual("Microstrategy Dashboard");
            _microstrategy = _profileManager.ClickOnDashboardIconToNavigateToMicrostrategy();
            _microstrategy.IsMicrostrategyOptionPresentInDashboardMenu().ShouldBeTrue("Is microstrategy option present?");
            _microstrategy.GetTitle().ShouldBeEqual(PageTitleEnum.Microstrategy.GetStringValue(),
                 "When Microstrategy is selected for default Dashboard View then user should navigate to Microstrategy upon clicking dashboard icon");

            //login as user with default dashboard view is other than Microstrategy
            _profileManager = _microstrategy.Logout().LoginAsClientUser().NavigateToProfileManager();
            _profileManager.IsAnyRoleAssignedToUserFromDatabase(EnvironmentManager.ClientUserName).ShouldBeTrue("Is role assigned?"); 
            _profileManager.IsPrivilegeAssignedFromDatabase(EnvironmentManager.ClientUserName, _reportPrivilege).ShouldBeTrue("Is Report Privilege assigned?"); ;
            _profileManager.ClickOnProfile();
            _profileManager.GetDashboardViewProductType().ShouldContain("Microstrategy Dashboard",
                "Microstrategy Dashboard option must be listed in Default Dashboard View list");
            _dashboard = _profileManager.NavigateToDashboard();
            _dashboard.IsMicrostrategyOptionPresentInDashboardMenu().ShouldBeTrue("Is microstrategy option present?");
            _microstrategy = _dashboard.SelectMicrostrategyFilterOptions();
            _microstrategy.GetTitle().ShouldBeEqual(PageTitleEnum.Microstrategy.GetStringValue(), "Is User navigated to Microstrategy page?");
        }

        [Test,Category("Working")]
        public void Verify_Correct_Details_Displayed_And_Navigation_To_Microstrategy_Home_For_Client()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            IDictionary<string, string> paramLists = DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
            var columnHeader = paramLists["LabelName"].Split(',').ToList();
            
            _profileManager = QuickLaunch.Logout().LoginAsClientMstrUserwithManagerRole().NavigateToProfileManager();
            //_profileManager.IsRoleAssignedFromDatabase(EnvironmentManager.ClientMstrtUserWithManageRole,_managerRole).ShouldBeTrue("Manage Role Assigned?");
            //_profileManager.IsRoleAssignedFromDatabase(EnvironmentManager.ClientMstrtUserWithManageRole, _directorRole).ShouldBeTrue("Director Role Assigned?");
            _dashboard = _profileManager.NavigateToDashboard();
            _microstrategy = _dashboard.NavigateToMicrostrategyWithMultipleReports();
            var expectedList = _microstrategy.GetReportInfoFromDatabase(EnvironmentManager.Username);

           _microstrategy.GetGridViewSection.GetLabelInGridByColRow(1, 1).ShouldBeEqual(columnHeader[0], "reportname header value verified");
            _microstrategy.GetGridViewSection.GetLabelInGridByColRow(2, 1).ShouldBeEqual(columnHeader[1], "Role header value verified");
            _microstrategy.GetGridViewSection.GetLabelInGridByColRow(3, 1).ShouldBeEqual(columnHeader[2], "Created By header value verified");
            _microstrategy.GetGridViewSection.GetLabelInGridByColRow(4, 1).ShouldBeEqual(columnHeader[3], "Create date header value verified");

            _microstrategy.GetGridViewSection.GetGridRowCount().ShouldBeEqual(_microstrategy.GetCountOfReportsAssigendtoUser(EnvironmentManager.Username));
            StringFormatter.PrintMessage("Verify correct report name displayed");
            
            _microstrategy.GetGridViewSection.GetGridListValueByCol(1).ShouldCollectionBeEqual(expectedList.Select(x => x[0]).ToList(), "Report Name equal?");
            _microstrategy.GetGridViewSection.GetGridListValueByCol(2).ShouldCollectionBeEqual(expectedList.Select(x => x[1]).ToList(), "Role equal?");
            _microstrategy.GetGridViewSection.GetGridListValueByCol(3).ShouldCollectionBeEqual(expectedList.Select(x => x[2]).ToList(), "created by equal?");
            _microstrategy.GetGridViewSection.GetGridListValueByCol(4).ShouldCollectionBeEqual(expectedList.Select(x => x[3]).ToList(), "created date equal?");

            StringFormatter.PrintMessage("Verify navigation to Microstratergy home");
            _microstrategyhome = _microstrategy.ClickOnViewReportsAndNavigatetohome();
            _microstrategyhome.IsSelectProductOptionpresent().ShouldBeTrue("ReportHome option displayed?");
            StringFormatter.PrintMessage("verify navigation back to the microstratergy page");
            _microstrategyhome.ClickOnMicrostrategyDashboardTONavigateToMicrostratergyPage();
            StringFormatter.PrintMessage("Navigated back to microstratergy page");

            _profileManager = QuickLaunch.Logout().LoginAsClientUserHavingFfpEditOfPciFlagsAuthority().NavigateToProfileManager();
           
            //_profileManager.IsRoleAssigned(_providerRelations).ShouldBeTrue("provider Relations Role Assigned?");
            //_profileManager.IsRoleAssigned(_qualityAssurance).ShouldBeTrue("Quality Assurance Role Assigned?");
            _dashboard = _profileManager.NavigateToDashboard();
            _microstrategy = _dashboard.NavigateToMicrostrategyWithMultipleReports();
            //if (_microstrategy.checkRolesToReportsMappingfromDatabase(_providerRelations, _qualityAssurance))
            //{
            //    _microstrategy.IsMstrNoDataMessageAvailable().ShouldBeFalse("Message Displayed when reports are not assigned to the user");
            //    _microstrategy.UpdateReportToRoleMapping(_providerRelations, _qualityAssurance);
            //    SiteDriver.Reload();
            //}
            _microstrategy.IsMstrNoDataMessageAvailable().ShouldBeTrue("Message Displayed when reports are not assigned to the user");



        }


        //[Test] //TE-529
        public void Verify_Switch_Client_Functionality_For_MSTR_Page()
        {
            try
            {
                QuickLaunch.Logout().LoginAsClientUser();
                var mostRecentClientData =
                    QuickLaunch.GetMostRecentClientDetailsFromDatabase(EnvironmentManager.Username);
                var allClientData = QuickLaunch.GetAllClientDetailsFromDatabase(EnvironmentManager.Username);
                _profileManager = QuickLaunch.NavigateToProfileManager();
                var defaultPage = _profileManager.GetDefaultPagePreference();
                _dashboard = _profileManager.NavigateToDashboard();
                _microstrategy = _dashboard.NavigateToMicrostrategyWithMultipleReports();

                StringFormatter.PrintMessage("Verify Switch Client Icon in Core Page");
                _microstrategy.IsSwitchClientIconPresent()
                    .ShouldBeTrue("Switch Client Icon Should Be Present in core page");
                _microstrategy.SwitchClientSection.ClickOnSwitchClient();

                StringFormatter.PrintMessage("Verify Presence of Switch Client Window");
                _microstrategy.SwitchClientSection.IsSwitchClientSideWindowPresent()
                    .ShouldBeTrue("Switch Client Side Window Should Be Opened");

                _microstrategy.SwitchClientSection.ClickOnCloseButton();
                _microstrategy.SwitchClientSection.IsSwitchClientSideWindowPresent()
                    .ShouldBeFalse("Switch Client Side Window Should Be Closed");

                _microstrategy.SwitchClientSection.ClickOnSwitchClient();
                _microstrategy.SwitchClientSection.IsMostRecentPageHeaderPresent()
                    .ShouldBeTrue("Most Recent Page Header Should Be Present");
                _microstrategy.SwitchClientSection.GetMostRecentClientCodes()
                    .ShouldCollectionBeEqual(mostRecentClientData.Select(x => x[0]).ToList(),
                        "Client Codes Should Match");
                _microstrategy.SwitchClientSection.GetMostRecentClientNames()
                    .ShouldCollectionBeEqual(mostRecentClientData.Select(x => x[1]).ToList(),
                        "Client Names Should Match");
                _microstrategy.SwitchClientSection.IsAllClientsPageHeaderPresent()
                    .ShouldBeTrue("All Clients Page Header Should Be Present");
                _microstrategy.SwitchClientSection.GetAllClientCodes()
                    .ShouldCollectionBeEqual(allClientData.Select(x => x[0]).ToList(), "Client Codes Should Match");
                _microstrategy.SwitchClientSection.GetAllClientNames()
                    .ShouldCollectionBeEqual(allClientData.Select(x => x[1]).ToList(), "Client Names Should Match");

                _microstrategy.SwitchClientSection.SwitchClientTo(ClientEnum.TTREE);
                _microstrategy.WaitForPageToLoad();
                CurrentPage.GetPageHeader().ShouldBeEqual(defaultPage,
                    "User should be navigated to default page after client switch");
                _dashboard = CurrentPage.NavigateToDashboard();
                _microstrategy = _dashboard.NavigateToMicrostrategyWithMultipleReports();
                _microstrategy.WaitForPageToLoad();
                CurrentPage.IsClientLogoPresent(ClientEnum.TTREE,true);


                StringFormatter.PrintMessage("Verify Navigation to Default Core Page After Client Switch");
                QuickLaunch.Logout().LoginClientUserWithDefaultSuspectProvidersPage();
                CurrentPage.WaitForPageToLoad();
                _profileManager = CurrentPage.NavigateToProfileManager();
                var defaultCorePage = _profileManager.GetDefaultPagePreference();
                _dashboard = CurrentPage.NavigateToDashboard();
                _microstrategy = _dashboard.NavigateToMicrostrategyWithMultipleReports();
                _microstrategy.SwitchClientSection.ClickOnSwitchClient();
                _microstrategy.SwitchClientSection.SwitchClientTo(ClientEnum.TTREE);
                CurrentPage.GetPageHeader().ShouldBeEqual(defaultCorePage,
                    "User should be navigated to default page after client switch");
                CurrentPage.IsClientLogoPresent(ClientEnum.TTREE);
            }
            finally
            {
                CurrentPage.Logout().LoginAsClientMstrUser();
            }
        }


        #endregion


        #region PRIVATE METHODS 



        #endregion
    }
}

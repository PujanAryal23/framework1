using System.Diagnostics;
using Nucleus.Service.Data;
using Nucleus.Service.PageServices.Dashboard;
using Nucleus.Service.Support.Common.Constants;
using Nucleus.Service.Support.Menu;
using Nucleus.UIAutomation.TestSuites.Base;
using NUnit.Framework;
using System;
using Nucleus.Service.Support.Environment;
using Nucleus.Service.Support.Enum;
using System.Linq;
using Nucleus.Service.PageServices.Settings.User;
using System.Configuration;
using Nucleus.Service.PageServices.ChromeDownLoad;
using Nucleus.Service.PageServices.QuickLaunch;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Nucleus.Service.PageServices.Appeal;
using Nucleus.Service.PageServices.Base.Default;
using Nucleus.Service.PageServices.Claim;
using Nucleus.Service.PageServices.HelpCenter;
using Nucleus.Service.PageServices.PreAuthorization;
using Nucleus.Service.PageServices.Provider;
using Nucleus.Service.PageServices.SwitchClient;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Utils;


namespace Nucleus.UIAutomation.TestSuites.TestCases
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class NucleusHome
    {
        //private DashboardPage _dashboard;
        //private ProfileManagerPage _profileManager;
        //private ClaimActionPage _claimAction;
        //private ProviderSearchPage _providerSearch;
        //private SwitchClientPage _switchClientPage;

        //private ClaimSearchPage _claimSearch;
        //private AppealSearchPage _appealSearch;

        //private AppealActionPage _appealAction;
        //private PreAuthSearchPage _newPreAuthSearchPage;
        //private PreAuthActionPage _newPreAuthActionPage;
        //private ProviderActionPage _providerActionPage;
        //private UserProfileSearchPage _newUserProfileSearch;
        
        //#region OVERRIDE METHODS

        //protected override string FullyQualifiedClassName
        //{
        //    get { return GetType().FullName; }
        //}

        ///// <summary>
        ///// Override ClassInit to add additional code.
        ///// </summary>
        //protected override void ClassInit()
        //{
        //    try
        //    {
        //        base.ClassInit();


        //    }
        //    catch (Exception)
        //    {
        //        if (StartFlow != null)
        //            StartFlow.Dispose();
        //        throw;
        //    }
        //}

        //protected override void TestCleanUp()
        //{
        //    base.TestCleanUp();
        //    if (!automatedBase.CurrentPage.IsCurrentClientAsExpected(automatedBase.EnvironmentManager.TestClient))
        //    {
        //        CheckTestClientAndSwitch();
        //    }

        //    if (string.Compare(UserType.CurrentUserType, UserType.HCIADMIN, StringComparison.OrdinalIgnoreCase) != 0)
        //    {
        //        automatedBase.QuickLaunch = automatedBase.QuickLaunch.Logout().LoginAsHciAdminUser();
        //        CheckTestClientAndSwitch();
        //    }
        //    else 
        //    {
        //        automatedBase.CurrentPage.ClickOnQuickLaunch();
        //    }
        //}

        //#endregion

        #region TEST SUITES

        [Test, Category("SmokeTestDeployment")] //TANT-258 + CAR-2427[CAR-2901]
        public void Verify_switch_client_for_internal_users()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                automatedBase.CurrentPage.IsSwitchClientIconPresent().ShouldBeTrue("Switch Client should be present");

                var _newClaimSearch = automatedBase.CurrentPage.NavigateToClaimSearch();

                _newClaimSearch.SwitchClientTo(ClientEnum.TTREE);
                _newClaimSearch.WaitForPageToLoad();

                automatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.QuickLaunch.GetStringValue(),
                    "User should be navigated to the default landing page after switching the client");
                automatedBase.CurrentPage.GetCurrentClient().ShouldBeEqual(ClientEnum.TTREE.GetStringValue(),
                    "Client code in switch client dropdown should display the current client after the switch");
            }
        }

        [Test]//CAR-2899(CAR-438)
        public void Verify_message_and_link_on_Nucleus_Home()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                var linkValueList = new List<string> {"Go to My Profile", "Go to Help Center"};

                automatedBase.QuickLaunch.IsGoToLinkPresent(linkValueList[0]).ShouldBeTrue($"Is {linkValueList[0]} Button Present?");
                automatedBase.QuickLaunch.ClickOnGoToLinkPresent<MyProfilePage>(linkValueList[0]);
                automatedBase.QuickLaunch.GetPageHeader().ShouldBeEqual(PageHeaderEnum.MyProfile.GetStringValue(),
                    $"Is {linkValueList[0]} redirects to correct page?");
                automatedBase.QuickLaunch.ClickOnLogo();

                automatedBase.QuickLaunch.IsGoToLinkPresent(linkValueList[1]).ShouldBeTrue($"Is {linkValueList[1]} Button Present?");
                automatedBase.QuickLaunch.ClickOnGoToLinkPresent<HelpCenterPage>(linkValueList[1]);
                automatedBase.QuickLaunch.GetPageHeader().ShouldBeEqual(PageHeaderEnum.HelpCenter.GetStringValue(),
                    $"Is {linkValueList[1]} redirects to correct page?");
                automatedBase.QuickLaunch.ClickOnLogo();
            }

        }

        [Test, Category("SmokeTest")]
        public void Navigate_to_quick_launch_page()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                Console.Out.WriteLine(automatedBase.EnvironmentManager.ApplicationUrl);
            }
        }

        [Test] //TE-610
        public void Verify_correct_Client_Code_Displayed_In_Multiple_Tabs()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ProviderSearchPage _providerSearch;
                try
                {
                    automatedBase.CurrentPage =
                        _providerSearch =
                            automatedBase.QuickLaunch.NavigateToProviderSearch(); //Navigate to any Client Specific Url

                    StringFormatter.PrintMessage("Verify that user can work with different client in multiple tabs ");
                    _providerSearch.IsDefaultTestClientForEmberPage(ClientEnum.SMTST)
                        .ShouldBeTrue("Client  smtst present?");
                    var url = automatedBase.CurrentPage.PageUrl.Replace("SMTST", "TTREE");
                    var newDuplicatePage = _providerSearch.SwitchTabAndNavigateToProviderSearchPage(url);

                    newDuplicatePage.IsDefaultTestClientForEmberPage(ClientEnum.TTREE)
                        .ShouldBeTrue("Client  TTREE present?");
                    newDuplicatePage.SwitchTab(newDuplicatePage.GetCurrentWindowHandle());
                    _providerSearch.RefreshPage();
                    _providerSearch.IsDefaultTestClientForEmberPage(ClientEnum.SMTST)
                        .ShouldBeTrue("Is Client  SMTST present? Is Previous client retained?");

                    StringFormatter.PrintMessage(
                        "Verify  behavior on navigation from core page with client specific Url to core page without client specific url. If Clientcode is not specified in url, cookie set in the client will be used");
                    var dashboardPage = _providerSearch.NavigateToDashboard();
                    dashboardPage.IsDefaultTestClientForEmberPage(ClientEnum.SMTST)
                        .ShouldBeTrue("client retained to SMTST in Dashboard  page");
                    newDuplicatePage.SwitchTab(newDuplicatePage.GetCurrentWindowHandle());
                    _providerSearch.RefreshPage();
                    automatedBase.CurrentPage = dashboardPage = newDuplicatePage.NavigateToDashboard();
                    dashboardPage.IsDefaultTestClientForEmberPage(ClientEnum.TTREE)
                        .ShouldBeTrue("Previous Client retained in Dashboard page?");



                }
                finally
                {
                    automatedBase.CurrentPage.CloseAnyTabIfExist();
                    automatedBase.CurrentPage.ClickOnSwitchClient().SwitchClientTo(ClientEnum.SMTST);
                }
            }



        }

        [Test, Category("SchemaDependent")] //TE-610
        [NonParallelizable]
        public void Verify_Release_All_Locks_On_Logout()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                 
                 ClaimActionPage _claimAction;
                 ProviderSearchPage _providerSearch;
               

                 ClaimSearchPage _claimSearch;
                 AppealSearchPage _appealSearch;

                 AppealActionPage _appealAction;
                 PreAuthSearchPage _newPreAuthSearchPage;
                 PreAuthActionPage _newPreAuthActionPage;
                 ProviderActionPage _providerActionPage;
                
                var TestName = new StackFrame(true).GetMethod().Name;
                var paramList = automatedBase.DataHelper.GetTestData(GetType().FullName, TestName);
                var prvseq = paramList["providerseq"];
                var appealseq = paramList["appealseq"];
                var claseq = paramList["claimseq"].Split('-')[0];
                var clasub = paramList["claimseq"].Split('-')[1];
                var preauthseq = paramList["preauthseq"];

                try
                {
                    automatedBase.QuickLaunch.Logout().LoginasHciAdminUser7();
                    var userID = automatedBase.EnvironmentManager.Username;
                    _providerSearch = automatedBase.QuickLaunch.NavigateToProviderSearch();
                    _providerSearch.SearchByProviderSequence(prvseq);
                    _providerActionPage =
                        _providerSearch.ClickOnProviderSequenceAndNavigateToProviderActionPage(prvseq);

                    var newFirstTab = _providerSearch.SwitchTabAndNavigateToQuickLaunchPage();
                    newFirstTab.ClickOnSwitchClient().SwitchClientTo(ClientEnum.TTREE);
                    _appealSearch = newFirstTab.NavigateToAppealSearch();
                    _appealAction =
                        _appealSearch.SearchByAppealSequenceToNavigateToAppealActionForClientSwitch(appealseq);


                    var secondTab = _appealSearch.SwitchTabAndNavigateToQuickLaunchPage(false, 2);
                    _claimSearch = secondTab.NavigateToClaimSearch();
                    _claimAction =
                        _claimSearch.SearchByClaimSequenceToNavigateToClaimActionForClientSwitch(claseq, true);

                    var thirdtab = _claimAction.SwitchTabAndNavigateToQuickLaunchPage(false, 3);
                    thirdtab.ClickOnSwitchClient().SwitchClientTo(ClientEnum.PEHP);
                    _newPreAuthSearchPage = thirdtab.NavigateToPreAuthSearch();
                    _newPreAuthActionPage =
                        _newPreAuthSearchPage.SearchByAuthSequenceAndNavigateToAuthActionAndHandlePopup(preauthseq,
                            false);

                    var duplicateTab = _newPreAuthActionPage.SwitchTabAndNavigateToQuickLaunchPage(false, 4);


                    _providerActionPage.IsProviderLocked(ClientEnum.SMTST.ToString(), prvseq, userID)
                        .ShouldBeTrue("Provider lock retained on multiple client switch?");
                    _claimSearch.IsClaimLocked(ClientEnum.TTREE.GetStringDisplayValue(), claseq, clasub, userID)
                        .ShouldBeTrue("claim lock retained on multiple client switch?");
                    _appealAction.IsAppealLocked(appealseq, userID)
                        .ShouldBeTrue("Appeal lock retained on multiple client switch?");
                    _newPreAuthActionPage.IsPreAuthLocked(ClientEnum.PEHP.ToString(), preauthseq, userID)
                        .ShouldBeTrue("preauth lock retained on multiple client switch?");

                    duplicateTab.Logout().LoginAsHciAdminUser();
                    _providerActionPage.GetLockedProviderCountByUser(ClientEnum.SMTST.ToString(), userID)
                        .ShouldBeEqual(0, "provider locked released?");
                    _appealAction.GetAppealLockCountByUser(userID).ShouldBeEqual(0, "Appeal lock released on logout?");
                    _claimAction.GetLockedClaimCountByUser(userID, ClientEnum.TTREE.GetStringDisplayValue())
                        .ShouldBeEqual(0, "claim lock release on log out?");
                    _newPreAuthActionPage.GetLockedPreauthCountByUser(userID, ClientEnum.PEHP.ToString())
                        .ShouldBeEqual(0, "preauth lock release on logout?");

                }
                finally
                {
                    automatedBase.CurrentPage.CloseAnyTabIfExist();


                }
            }
        }

        [Test] //TE-762
        [NonParallelizable]
        public void Verify_Remove_Lock_Functionality()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ClaimActionPage _claimAction;
                ClaimSearchPage _claimSearch;
               
                automatedBase.QuickLaunch = automatedBase.CurrentPage.Logout().LoginasHciAdminUser7();
                StringFormatter.PrintMessage("Generate FCI worklist");
                _claimAction = automatedBase.QuickLaunch.NavigateToFciClaimsWorkList(true);
                automatedBase.QuickLaunch =
                    _claimAction.SwitchTabAndOpenQuickLaunchPageCloseFirstTab(automatedBase.Login.LoginasHciAdminUser7);
                StringFormatter.PrintMessage("Generate CV worklist");
                _claimAction = automatedBase.QuickLaunch.NavigateToCVClaimsWorkList(true);

                _claimAction.GetSideBarPanelSearch.OpenSidebarPanel();
                _claimAction.WaitForAdditionalLockedClaims();
                var lockedClaimsList = _claimAction.GetSideBarPanelSearch.GetAddtionallockedClaims();
                var claseq = lockedClaimsList.FirstOrDefault();

                StringFormatter.PrintMessage("Verify Remove lock functionality in additional locked claims section");
                _claimAction
                    .IsClaimSearchResultsLocked(automatedBase.EnvironmentManager.HciAdminUsername7, lockedClaimsList)
                    .ShouldBeTrue("claims of Fci worklist should be locked");
                _claimAction.GetSideBarPanelSearch.ClickOnRemoveLock(claseq);
                _claimAction.GetSideBarPanelSearch.GetAddtionallockedClaims().Count
                    .ShouldBeEqual(lockedClaimsList.Count - 1, "Addition Locked Claims should be decreased");
                _claimAction.IsClaimLocked(ClientEnum.SMTST.ToString(), claseq.Split('-')[0], claseq.Split('-')[1],
                    automatedBase.EnvironmentManager.HciAdminUsername7).ShouldBeFalse($"Is claim {claseq} locked?");

                StringFormatter.PrintMessage(
                    " Verify clicking on search icon, only removes the lock of the current worklist and locks of previous worklist is retained");
                _claimSearch = _claimAction.ClickClaimSearchIcon();
                _claimSearch.GetSideBarPanelSearch.OpenSidebarPanel();
                _claimAction.WaitForAdditionalLockedClaims();
                _claimSearch.GetSideBarPanelSearch.GetAddtionallockedClaims().FirstOrDefault()
                    .ShouldBeEqual(lockedClaimsList[1], "FCI Claim retained?");
            }




        }


        [Test] //TE-522 + CAR-2427 [CAR-2901]
        public void Verify_Switch_Client_Functionality_For_Core_Pages()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                UserProfileSearchPage _newUserProfileSearch;
                try
                {

                    var mostRecentClientData =
                        automatedBase.QuickLaunch.GetMostRecentClientDetailsFromDatabase(automatedBase
                            .EnvironmentManager.Username);
                    var allClientData =
                        automatedBase.QuickLaunch.GetAllClientDetailsFromDatabase(automatedBase.EnvironmentManager
                            .Username);

                    automatedBase.CurrentPage.IsCurrentClientAsExpected(ClientEnum.SMTST)
                        .ShouldBeTrue("User's default client should be shown");
                    _newUserProfileSearch = automatedBase.CurrentPage.NavigateToNewUserProfileSearch();
                    _newUserProfileSearch.SearchUserByNameOrId(
                        new List<string> {automatedBase.EnvironmentManager.Username}, true);
                    _newUserProfileSearch.ClickOnGridRowByUserIdToOpenUserSettingSideView(automatedBase
                        .EnvironmentManager.Username);
                    _newUserProfileSearch.ClickOnUserSettingTabByTabName(
                        UserSettingsTabEnum.Preferences.GetStringValue());

                    var defaultPage =
                        _newUserProfileSearch.GetInputTextBoxValueByLabel(
                            UserPreferencesEnum.DefaultPage.GetStringValue());
                    var _claimSearchPage = _newUserProfileSearch.NavigateToClaimSearch();

                    StringFormatter.PrintMessage("Verify Switch Client Dropdown in Core Page");
                    _claimSearchPage.IsSwitchClientIconPresent()
                        .ShouldBeTrue("Switch Client Dropdown Should Be Present in core page");

                    _claimSearchPage.ClickOnSwitchClient();
                    StringFormatter.PrintMessage("Verifying the 'Most Recent' sub-list");
                    _claimSearchPage.IsMostRecentPageHeaderPresent()
                        .ShouldBeTrue("'Most Recent' Page Header Should Be Present");
                    _claimSearchPage.GetClientCodesOrClientNames(true, false)
                        .ShouldCollectionBeEqual(mostRecentClientData.Select(x => x[0]).ToList(),
                            "Client codes should match in 'Most Recent' sub-list");
                    _claimSearchPage.GetClientCodesOrClientNames(true, true)
                        .ShouldCollectionBeEqual(mostRecentClientData.Select(x => x[1]).ToList(),
                            "Client names should match in 'All Clients' sub-list");

                    StringFormatter.PrintMessage("Verifying the 'All Clients' sub-list");
                    _claimSearchPage.IsAllClientsPageHeaderPresent()
                        .ShouldBeTrue("'All Clients' Page Header Should Be Present");
                    _claimSearchPage.GetClientCodesOrClientNames(false, false)
                        .ShouldCollectionBeEqual(allClientData.Select(x => x[0]).ToList(),
                            "Client Codes Should Match in 'All Clients' sub-list");
                    _claimSearchPage.GetClientCodesOrClientNames(false, true)
                        .ShouldCollectionBeEqual(allClientData.Select(x => x[1]).ToList(),
                            "Client Names Should Match in 'All Clients' sub-list");

                    StringFormatter.PrintMessageTitle("Verifying switching the client");
                    _claimSearchPage.TypeAheadInSwitchClientPage(ClientEnum.TTREE);
                    _claimSearchPage.GetClientCodesOrClientNames(false, false)
                        .ShouldCollectionBeEqual(new List<string>() {ClientEnum.TTREE.ToString()},
                            "Switch Client dropdown should support typeahead");

                    _claimSearchPage.SwitchClientTo(ClientEnum.TTREE);
                    _claimSearchPage.WaitForPageToLoad();
                    automatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(defaultPage,
                        "User should be navigated to default page after client switch");
                    _claimSearchPage = automatedBase.CurrentPage.NavigateToClaimSearch();
                    _claimSearchPage.WaitForPageToLoad();
                    automatedBase.CurrentPage.CurrentPageUrl.Contains(ClientEnum.TTREE.ToString())
                        .ShouldBeTrue("Client should be changed");

                    automatedBase.CurrentPage.ClickOnQuickLaunch();

                    StringFormatter.PrintMessage("Verify Navigation to Default Core Page After Client Switch");
                    automatedBase.QuickLaunch.Logout().LoginWithDefaultSuspectProvidersPage();
                    automatedBase.CurrentPage.WaitForPageToLoad();
                    _newUserProfileSearch = automatedBase.CurrentPage.NavigateToNewUserProfileSearch();
                    _newUserProfileSearch.SearchUserByNameOrId(
                        new List<string> {automatedBase.EnvironmentManager.Username}, true);
                    _newUserProfileSearch.ClickOnGridRowByUserIdToOpenUserSettingSideView(automatedBase
                        .EnvironmentManager.Username);
                    _newUserProfileSearch.ClickOnUserSettingTabByTabName(
                        UserSettingsTabEnum.Preferences.GetStringValue());
                    var defaultCorePage =
                        _newUserProfileSearch.GetInputTextBoxValueByLabel(UserPreferencesEnum.DefaultPage
                            .GetStringValue());

                    _claimSearchPage = _newUserProfileSearch.NavigateToClaimSearch();
                    _claimSearchPage.ClickOnSwitchClient();
                    _claimSearchPage.SwitchClientTo(ClientEnum.TTREE);
                    automatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(defaultCorePage,
                        "User should be navigated to default page after client switch");
                    automatedBase.CurrentPage.CurrentPageUrl.Contains(ClientEnum.TTREE.ToString())
                        .ShouldBeTrue("Client should be changed");

                    StringFormatter.PrintMessageTitle("Log in as user which has only one assigned client to it");
                    automatedBase.CurrentPage.Logout().LoginAsHciAdminUser2();
                    automatedBase.CurrentPage.GetCurrentClient().ShouldBeEqual("Selenium Test Client",
                        "The default client for the user should be SMTST");
                    automatedBase.CurrentPage.IsSwitchClientCaretSignPresent()
                        .ShouldBeFalse(
                            "Switch client field not be a dropdown when there is only one client that is assigned");

                }

                finally

                {
                    if (string.Compare(UserType.CurrentUserType, UserType.HCIADMIN,
                        StringComparison.OrdinalIgnoreCase) != 0)
                        automatedBase.CurrentPage.Logout().LoginAsHciAdminUser();
                }
            }
        
        }

       
        [Test]
        public void Verify_that_CV_QC_Qudit_worklist_menu_is_shown_for_user_with_CV_QC_Authority()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                var TestName = new StackFrame(true).GetMethod().Name;
                bool pciQaAuditAuthorizedFlag = Convert.ToBoolean(automatedBase.DataHelper.GetSingleTestData(
                    GetType().FullName,
                    TestName, "CVQCAuditFlag", "Value"));

                if (pciQaAuditAuthorizedFlag)
                    automatedBase.CurrentPage.IsSecondarySubMenuOptionPresent(HeaderMenu.Claim, SubMenu.ClaimWorkList,
                            SubMenu.CVQCWorkList)
                        .ShouldBeTrue("Is CV QA SubMenu Present");
            }
        }

        [Test] //CAR-2993(CAR-2980)
        public void Verify_ManagerTab_And_Submenu()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists = automatedBase.DataHelper.GetTestData(GetType().FullName,
                    TestName);
                var subMenuList = paramLists["Submenu"].Split(',').ToList();
                automatedBase.QuickLaunch.IsManagerMenuPresent().ShouldBeTrue("Manager Header Menu present?");
                automatedBase.QuickLaunch.GetManagerSubMenu()
                    .ShouldCollectionBeEqual(subMenuList, "Submenu List correct?");

                StringFormatter.PrintMessage("Verify Manager Menu Is Only Visible For Internal Users");
                automatedBase.QuickLaunch.Logout().LoginAsClientUser();
                automatedBase.CurrentPage.IsManagerMenuPresent().ShouldBeFalse("Manager Header Menu present?");

                StringFormatter.PrintMessage(
                    "Verify user requires Appeal Manager authority (Ops Manager role) or QA Manager authority (QA Analyst role) will see this tab");
                automatedBase.QuickLaunch.Logout().LoginWithDefaultProviderSearchPage();
                automatedBase.CurrentPage.IsManagerMenuPresent().ShouldBeFalse("Manager Header Menu present?");
            }
        }

        [Test] //US28488 and US67762
        public void Verify_Dashboard_icon_for_Cotiviti_users_with_Dashboard_authority_and_navigation_to_Dashboard_page()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                DashboardPage _dashboard;
                var TestName = new StackFrame(true).GetMethod().Name;
                var expectedDashboardProductTypeList = automatedBase.DataHelper
                    .GetMappingData(GetType().FullName, "Dashboard_Product_Type_List").Values.ToList();
                expectedDashboardProductTypeList.Insert(0, "");


                StringFormatter.PrintMessage("Verification of authority for user with readonly privilege");
                automatedBase.QuickLaunch.IsRoleAssigned<UserProfileSearchPage>(
                    new List<string> {automatedBase.EnvironmentManager.Username},
                    RoleEnum.ClaimsProcessor.GetStringValue()).ShouldBeTrue(
                    $"Is Product Dashboards present for current user {automatedBase.EnvironmentManager.Username}");
                automatedBase.QuickLaunch.IsRoleAssigned<QuickLaunchPage>(
                    new List<string> {automatedBase.EnvironmentManager.ClientUserName},
                    RoleEnum.ClaimsProcessor.GetStringValue()).ShouldBeTrue(
                    $"Is Product Dashboards present for current user {automatedBase.EnvironmentManager.ClientUserName}");


                automatedBase.QuickLaunch.IsDashboardIconPresent().ShouldBeTrue("Is Dashboard icon present?");

                _dashboard = automatedBase.QuickLaunch.NavigateToDashboard();
                _dashboard.CurrentPageTitle.ShouldBeEqual(PageHeaderEnum.Dashboard.GetStringValue(),
                    "Is Dashboard page title Equals?");

                automatedBase.QuickLaunch.Logout().LoginAsClientUser();
                automatedBase.QuickLaunch.IsDashboardIconPresent().ShouldBeTrue("Is Dashboard icon present?");

                _dashboard = automatedBase.QuickLaunch.NavigateToDashboard();
                _dashboard.CurrentPageTitle.ShouldBeEqual(PageHeaderEnum.Dashboard.GetStringValue(),
                    "Is Dashboard page title Equals?");
            }
        }

        [Test] //US28488 and US67762
        public void Verify_Dashboard_icon_for_Cotiviti_users_without_Dashboard_authority()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                StringFormatter.PrintMessage("Verification of authority for user with readonly privilege");
                automatedBase.QuickLaunch.IsRoleAssigned<UserProfileSearchPage>(
                    new List<string> {automatedBase.EnvironmentManager.HciUserWithNoManageAppealAuthority},
                    RoleEnum.ClaimsProcessor.GetStringValue()).ShouldBeFalse(
                    $"Is Product Dashboards present for current user {automatedBase.EnvironmentManager.HciUserWithNoManageAppealAuthority}");
                automatedBase.QuickLaunch.IsRoleAssigned<UserProfileSearchPage>(
                    new List<string> {automatedBase.EnvironmentManager.ClientUserWithNoAnyAuthority},
                    RoleEnum.ClaimsProcessor.GetStringValue()).ShouldBeFalse(
                    $"Is Product Dashboards present for current user {automatedBase.EnvironmentManager.ClientUserWithNoAnyAuthority}");



                StringFormatter.PrintMessage("The logged in User <uiautomation4> does not have Dashboard authority");
                automatedBase.QuickLaunch.Logout().LoginAsUserHavingNoAnyAuthority(); //uiautomation4
                automatedBase.QuickLaunch.IsDashboardIconPresent().ShouldBeFalse("Is Dashboard icon present?");


                StringFormatter.PrintMessage("The logged in User uiautomation_cli does not have Dashboard authority");
                automatedBase.QuickLaunch.Logout().LoginAsClientUserWithNoAnyAuthority()
                    .ClickOnQuickLaunch(); //uiautomation_cli
                automatedBase.QuickLaunch.IsDashboardIconPresent().ShouldBeFalse("Is Dashboard icon present?");

            }
        }

        //[Test] //US67762
        //public void Verify_dashboard_icon_for_users_with_Read_only_authority()
        //{
        //    TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
        //    var expectedDashboardProductTypeList = automatedBase.DataHelper
        //        .GetMappingData(FullyQualifiedClassName,
        //            "Dashboard_Product_Type_List_For_Users_with_read_Only_Authority").Values.ToList();
        //    expectedDashboardProductTypeList.Insert(0, "");

        //    automatedBase.QuickLaunch.Logout().LoginAsHCIUserWithReadOnlyAccessToAllAuthorities();
        //    automatedBase.QuickLaunch.IsDashboardIconPresent().ShouldBeTrue("Is Dashboard icon present?");
        //    automatedBase.QuickLaunch.ClickOnCustomizeLink();
        //    automatedBase.QuickLaunch.IsDashboardPresentInQuickLaunchSetting()
        //        .ShouldBeTrue("Is Dashboard Icon present in Quick Launch Setting?");
        //    automatedBase.QuickLaunch.ClickOnCloseCustomize();
        //    _dashboard = automatedBase.QuickLaunch.NavigateToDashboard();
        //    _dashboard.CurrentPageTitle.ShouldBeEqual(PageHeaderEnum.Dashboard.GetStringValue(),
        //        "Is Dashboard page title Equals?");
        //    _profileManager = _dashboard.NavigateToProfileManager();
        //    _profileManager.IsUserPrefernceDefaultDashboardViewLabelPresent().ShouldBeTrue
        //        ("Dashboard view should be present in the Preferences section in New User Profile Summary");
        //    _profileManager.GetDashboardViewProductType().ShouldCollectionBeEqual(expectedDashboardProductTypeList,
        //        "Default Dashboard View Dropdown list");
        //    _profileManager.IsAuthorityAssigned("Product Dashboards")
        //        .ShouldBeTrue("Is <Dashboard> authority assigned?");
        //    _profileManager.IsReadOnlyAssgined("Product Dashboards")
        //        .ShouldBeTrue("Read-only authority should be assigned");
        //}

        [Test] //US67762
        public void Verify_top_Navigation_Bar_when_logged_in_User_with_Read_Write_authority()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                StringFormatter.PrintMessageTitle(
                    "Verification of tabs and icons in the top navigation of the application when logged in as a user with " +
                    "Read/Write authority");
                var expectedTopNavigationMenuOptions = automatedBase.DataHelper
                    .GetMappingData(GetType().FullName, "TopNavigationMenuOptions").Values.ToList();

                automatedBase.QuickLaunch.GetMainNavigationMenuTabList().ShouldCollectionBeEqual(
                    expectedTopNavigationMenuOptions,
                    "All the tabs should be present");
            }

            /*automatedBase.QuickLaunch.IsHelpIconPresent().ShouldBeTrue("Help Icon should be present");
            automatedBase.QuickLaunch.IsDashboardIconPresent().ShouldBeTrue("Dashboard Icon should be present");
            automatedBase.QuickLaunch.IsSwitchClientIconPresent().ShouldBeTrue("Switch Client option should be present");
            automatedBase.QuickLaunch.IsQuickLaunchIconPresent().ShouldBeTrue("Quick Launch Icon should be present");*/
        }



        //[Test] //US67763
        //public void Verify_navigation_Claim_Search_page_without_Retro_Claim_Search_authority()
        //{
        //    StringFormatter.PrintMessageTitle("Verify Claim Search for not having Authority of the Retro Claim Search");
        //    var claimSearch = automatedBase.EnvironmentManager.Instance.ApplicationUrl + PageUrlEnum.NewClaimSearch.GetStringValue();
        //    automatedBase.QuickLaunch.Logout().LoginAsUserHavingNoAnyAuthority();
        //    automatedBase.QuickLaunch.ClickOnCustomizeLink();
        //    automatedBase.QuickLaunch.IsClaimSearchPresentInQuickLaunchSetting()
        //        .ShouldBeFalse("Is Claim Search present in the Quick Launch Settings?");
        //    automatedBase.QuickLaunch.IsFlaggedClaimsPresentInQuickLaunchSetting()
        //        .ShouldBeFalse("Is Flagged Claims present in the Quick Launch Settings?");
        //    automatedBase.QuickLaunch.IsPendedClaimPresentInQuickLaunchSetting()
        //        .ShouldBeFalse("Is Pended Claims present in the Quick Launch Settings?");
        //    automatedBase.QuickLaunch.IsUnreviewedClaimsPresentInQuickLaunchSetting()
        //        .ShouldBeFalse("Is Unreviewed Claims present in the Quick Launch Settings?");
        //    automatedBase.QuickLaunch.IsUnreviewedFFPClaimsPresentInQuickLaunchSetting()
        //        .ShouldBeFalse("Is Unreviewed FFP Claims present in the Quick Launch Settings?");
        //    automatedBase.QuickLaunch.ClickOnCloseCustomize();
        //    Mouseover.MouseOverClaimMenu();
        //    automatedBase.QuickLaunch.GetSubMenuOption(1, 1).ShouldBeEqual(SubMenu.NewClaimSearch, "1st submenu");
        //    automatedBase.QuickLaunch.GetAllSubMenuListByHeaderMenu(HeaderMenu.Claim).ShouldNotContain(SubMenu.ClaimSearch,
        //        "Is Retro Claim Search option present in the submenu list?");

        //    var url = PageUrlEnum.ClaimSearch.GetStringValue();
        //    var title = automatedBase.QuickLaunch.SwitchToNavigateRetroClaimSearchViaUrlAndGetTitle(url);
        //    title.ShouldBeEqual(string.Format("User {0} is not authorized to access claim search.",
        //            automatedBase.EnvironmentManager.Instance.HciUserWithNoManageAppealAuthority),
        //        "Page should redirect to unauthorized yellow page if trying to navigate via URL");

        //    automatedBase.QuickLaunch.NavigateToClaimSearch();
        //    automatedBase.CurrentPage.CurrentPageUrl.ShouldBeEqual(claimSearch, "Page Url Should be New Claim Search");
        //}

        [Test, Category("IENonCompatible"), Category("SmokeTestDeployment")] //US69708 CAR-2900(CAR-2745)
        public void Verify_download_CIW_Doc()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                DashboardPage _dashboard;
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var url = automatedBase.QuickLaunch.CurrentPageUrl;
                const string expectedFileName = "SMTST.xlsx";
                string fileName;
                try
                {

                    StringFormatter.PrintMessage("Verify download CIW for Client with no doc");
                    automatedBase.QuickLaunch.ClickOnSwitchClient()
                        .SwitchClientTo(ClientEnum.LOADT); //switch to client with no CIW doc
                    automatedBase.QuickLaunch.IsCIWIconDisplayed()
                        .ShouldBeTrue("Is Download CIW Document Icon present?");
                    automatedBase.QuickLaunch.ClickOnCIWIcon();
                    automatedBase.QuickLaunch.IsPageErrorPopupModalPresent()
                        .ShouldBeTrue("Page error popup should be displayed");
                    automatedBase.QuickLaunch.GetPageErrorMessage()
                        .ShouldBeEqual(string.Format("No CIW document for {0} is available for download.",
                            ClientEnum.LOADT));
                    automatedBase.QuickLaunch.ClosePageError();

                    StringFormatter.PrintMessage("Verify download CIW from Core project");
                    automatedBase.CurrentPage = _dashboard = automatedBase.QuickLaunch.NavigateToDashboard();
                    _dashboard.IsCIWIconDisplayed()
                        .ShouldBeTrue("CIW Icon must be displayed form Core project as well ");
                    automatedBase.CurrentPage = automatedBase.QuickLaunch = _dashboard.ClickOnQuickLaunch();

                    StringFormatter.PrintMessage("Verify download CIW for client with doc");
                    automatedBase.QuickLaunch.ClickOnSwitchClient().SwitchClientTo(ClientEnum.SMTST);
                    automatedBase.QuickLaunch.IsCIWIconDisplayed()
                        .ShouldBeTrue("Is Download CIW Document Icon present?");
                    automatedBase.QuickLaunch.GetCIWIconTooltip()
                        .ShouldBeEqual("Download CIW", "Is Correct tooltip is display?");

                    StringFormatter.PrintMessage("Verification for Cancel");
                    automatedBase.QuickLaunch.ClickOnCIWIcon();
                    automatedBase.QuickLaunch.IsPageErrorPopupModalPresent()
                        .ShouldBeTrue("Confirmation pop up present on clicking CIW Download");
                    automatedBase.QuickLaunch.GetPageErrorMessage()
                        .ShouldBeEqual("This clients' CIW file will be downloaded. Do you wish to continue?");
                    automatedBase.QuickLaunch.ClickOkCancelOnConfirmationModal(false);
                    automatedBase.QuickLaunch.IsPageErrorPopupModalPresent()
                        .ShouldBeFalse("is confirmation pop up present after cancel is clicked");


                    StringFormatter.PrintMessage("Verification of downloaded file");
                    fileName = automatedBase.QuickLaunch.ClickOnClickOnCIWIconAndGetFileName();
                    var actualfileName = Regex.Replace(fileName, @"\d", "").Trim();
                    actualfileName = actualfileName.Replace("()", "").Replace(" ", "");
                    actualfileName.ShouldBeEqual(expectedFileName, "Is Download Filename Corrected");
                    automatedBase.QuickLaunch.WaitForFileExists(@"C:/Users/uiautomation/Downloads/" + fileName);
                    File.Exists(@"C:/Users/uiautomation/Downloads/" + fileName)
                        .ShouldBeTrue("CIW document should be downloaded");
                }
                finally
                {
                    automatedBase.QuickLaunch = automatedBase.CurrentPage.ClickBrowserBackButton<QuickLaunchPage>(url);
                    ExcelReader.DeleteFileIfAlreadyExists(expectedFileName);
                }
            }
        }

        



        [Test, Category("Working")]
        public void Verify_Microstrategy_dashboard_option_for_user_with_Report()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                automatedBase.QuickLaunch.Logout().LoginAsMstrUser1(); //
                automatedBase.QuickLaunch.Logout().LoginAsMstrUser1(); //
            }
        }

        
        [Test] //CAR 1493(CAR-1608)
        public void Verify_user_with_ONLY_DCIWorklist_authority_and_no_other_worklist_authority_can_select_DCI_Claims_option_from_worklist_menu()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ClaimActionPage _claimAction;
                automatedBase.QuickLaunch.Logout().LoginAsHCIUserwithONLYDCIWorklistauthority();
                _claimAction = automatedBase.QuickLaunch.NavigateToDciClaimsWorkList(true);
                var claseq = _claimAction.GetClaimSequence();
                _claimAction.GetFlagCountByClaimSeqAndProductFromDatabase(claseq, "D").ShouldBeGreaterOrEqual(1,
                    "claims with at least one active DCA flag should be presented in work list");
                _claimAction.IsPageErrorPopupModalPresent()
                    .ShouldBeFalse("There should not be any popup when navigate to DCA Work List");
            }
        }

        [Test] //CAR-2914(CAR-2837)
        public void Verify_different_Cotiviti_Links_In_User_Greeting_Dropdown_Menu()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                var TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(GetType().FullName, TestName);
                var quickLinksMenuOptions = Enum.GetValues(typeof(QuickLinksEnum))
                    .Cast<QuickLinksEnum>().Select(x => x.GetStringValue()).ToList();
                var cotivitiLinksMenuOptions = Enum.GetValues(typeof(CotivitiLinksEnum))
                    .Cast<CotivitiLinksEnum>().Select(x => x.GetStringValue()).ToList();
                var i = 0;
                quickLinksMenuOptions.Remove(QuickLinksEnum.CotivitiLinks.GetStringValue());
                foreach (var link in quickLinksMenuOptions)
                {
                    automatedBase.CurrentPage.MouseOverQuickLinks();
                    if (link == QuickLinksEnum.CMS.GetStringValue())
                        ClickOnALinkAndGetUrl(() => automatedBase.CurrentPage.ClickOnCotivitiLinksByLinkName(link),automatedBase)
                            .ShouldBeEqual(paramLists["CmsDatabaseUrl"], "Url should match");
                    else
                    {
                        var providerSearchPage = automatedBase.CurrentPage
                            .ClickOnProviderSearchLinkAndNavigateToProviderSearchPage(link);
                        providerSearchPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.ProviderSearch.GetStringValue(),
                            "User Should Be Navigated to Provider Search Page");
                        if (link == QuickLinksEnum.CotivitiFlaggedProviders.GetStringValue())
                            providerSearchPage.GetSideBarPanelSearch.GetInputValueByLabel("Quick Search")
                                .ShouldBeEqual(ProviderEnum.CotivitiFlaggedProviders.GetStringValue(),
                                    $"Quick search type should be {link}");
                        else
                            providerSearchPage.GetSideBarPanelSearch.GetInputValueByLabel("Quick Search")
                                .ShouldBeEqual(ProviderEnum.ClientFlaggedProviders.GetStringValue(),
                                    $"Quick search type should be {link}");
                        automatedBase.CurrentPage.ClickOnQuickLaunch();

                    }

                }

                StringFormatter.PrintMessage("Verify Cotiviti Links");
                foreach (var link in cotivitiLinksMenuOptions)
                {
                    automatedBase.CurrentPage.MouseOverCotivitiLinks();
                    ClickOnALinkAndGetUrl(() => automatedBase.CurrentPage.ClickOnCotivitiLinksByLinkName(link),automatedBase)
                        .ShouldBeEqual(paramLists["CotivitiLinksUrls"].Split(',')[i], "Url should match");
                    i++;
                }
            }
        }


        #endregion

        #region private methods
        private string ClickOnALinkAndGetUrl(Action action,NewAutomatedBaseParallelRun automatedBase)
        {
            action();
            automatedBase.CurrentPage.SwitchToLastWindow();
            var url = automatedBase.CurrentPage.GetCurrentUrl();
            automatedBase.CurrentPage.SwitchTab("0");
            automatedBase.CurrentPage.CloseAnyTabIfExist();
            automatedBase.CurrentPage.ClickOnPageFooter();
            return url;
        }

        private void VerifyPageHeader(Func<bool> tilePresent, Action clickOnTile, PageHeaderEnum enumValue, NewAutomatedBaseParallelRun automatedBase)
        {
            tilePresent().ShouldBeTrue(string.Format("Is {0} Tile Present?", enumValue.GetStringValue()));
            clickOnTile();
            automatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(enumValue.GetStringValue(),
                "Is Page Header Equals?");
        }

        private void VerifyPageHeaderForOtherPages(Func<bool> tilePresent, Action clickOnTile, PageUrlEnum pageUrl, NewAutomatedBaseParallelRun automatedBase)
        {
            try
            {
                tilePresent().ShouldBeTrue(string.Format("Is {0} Tile Present?", pageUrl));
                clickOnTile();
                automatedBase.CurrentPage.WaitForStaticTime(7000);
                automatedBase.CurrentPage.SwitchWindow(pageUrl.GetStringValue());
                automatedBase.CurrentPage.GetCurrentUrl().AssertIsContained(pageUrl.GetStringValue(), "Is Url Correct contains?");
            }
            finally
            {
                automatedBase.CurrentPage.CloseAnyTabIfExist();
            }
        }

        #endregion
    }
}



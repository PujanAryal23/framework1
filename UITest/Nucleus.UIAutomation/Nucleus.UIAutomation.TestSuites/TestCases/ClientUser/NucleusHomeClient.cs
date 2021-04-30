using System.Diagnostics;
using Nucleus.Service.Data;
using Nucleus.Service.PageServices.Dashboard;
using Nucleus.Service.PageServices.Settings.User;
using Nucleus.Service.Support.Common.Constants;
using Nucleus.Service.Support.Menu;
using Nucleus.UIAutomation.TestSuites.Base;
using NUnit.Framework;
using System;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Environment;
using System.Linq;
using Nucleus.Service.PageServices.Claim;
using Nucleus.Service.Support.Utils;
using UIAutomation.Framework.Core.Driver;
using System.Collections.Generic;
using Nucleus.Service.PageServices.Appeal;
using Nucleus.Service.PageServices.Base.Default;
using Nucleus.Service.PageServices.HelpCenter;
using Nucleus.Service.PageServices.PreAuthorization;
using Nucleus.Service.PageServices.Provider;
using Org.BouncyCastle.Asn1.Cms;

namespace Nucleus.UIAutomation.TestSuites.TestCases.ClientUser
{

    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class NucleusHomeClient
    {
        //private DashboardPage _dashboard;        
        //private ProfileManagerPage _profileManager;
        //private ClaimActionPage _claimAction;
        //private ProviderSearchPage _providerSearch;
        //private AppealSearchPage _appealSearch;
        //private AppealActionPage _appealAction;
        //private PreAuthSearchPage _newPreAuthSearch;
        //private PreAuthActionPage _newPreAuthAction;
        //private ClaimSearchPage _claimSearch;
        //private ProviderActionPage _providerActionPage;
        //private UserProfileSearchPage _newUserProfileSearch;

        //#region OVERRIDE METHODS

        protected string FullyQualifiedClassName
        {
            get { return GetType().FullName; }
        }

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

        //    if (string.Compare(UserType.CurrentUserType, UserType.CLIENT, StringComparison.OrdinalIgnoreCase) != 0)
        //    {
        //        automatedBase.QuickLaunch = automatedBase.CurrentPage.Logout().LoginAsClientUser();
        //    }
        //    else 
        //    {
        //        automatedBase.CurrentPage.ClickOnQuickLaunch();
        //    }
        //    base.TestCleanUp();
        //    CheckTestClientAndSwitch();
        //}

        //#endregion

        #region TEST SUITES

        [Test, Category("SmokeTestDeployment")] //TANT-258 + CAR-2427[CAR-2901]
        public void Verify_switch_client_for_client_users()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
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
        [Order(4)]
        public void Verify_message_and_link_on_Nucleus_Home()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                var linkValueList = new List<string> { "Go to My Profile", "Go to Help Center" };

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

        [Test] //TE-610
        public void Verify_correct_Client_Code_Displayed_In_Multiple_Tabs()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                ProviderSearchPage _providerSearch;
                try
                {
                    automatedBase.CurrentPage =
                        _providerSearch =
                            automatedBase.QuickLaunch.NavigateToProviderSearch(); //Navigate to any Client Specific Url

                    StringFormatter.PrintMessage("Verify that user can work with different client in multiple tabs ");
                    _providerSearch.IsDefaultTestClientForEmberPage(ClientEnum.SMTST)
                        .ShouldBeTrue("Client logo for smtst present?");
                    var url = automatedBase.CurrentPage.PageUrl.Replace(ClientEnum.SMTST.ToString(), ClientEnum.TTREE.ToString());
                    var newDuplicatePage = _providerSearch.SwitchTabAndNavigateToProviderSearchPage(url);

                    newDuplicatePage.IsDefaultTestClientForEmberPage(ClientEnum.TTREE)
                        .ShouldBeTrue("Client logo for TTREE present?");
                    newDuplicatePage.SwitchTab(automatedBase.CurrentPage.CurrentWindowHandle);
                    _providerSearch.RefreshPage();
                    _providerSearch.IsDefaultTestClientForEmberPage(ClientEnum.SMTST)
                        .ShouldBeTrue("Is Client logo for SMTST present? Is Previous client retained?");

                    StringFormatter.PrintMessage(
                        "Verify  behavior on navigation from core page with client specific Url to core page without client specific url. If Clientcode is not specified in url, cookie set in the client will be used");
                    var dashboardPage = _providerSearch.NavigateToDashboard();
                    dashboardPage.IsDefaultTestClientForEmberPage(ClientEnum.SMTST)
                        .ShouldBeTrue("client retained to SMTST in Dashboard  page");
                    newDuplicatePage.SwitchTab(dashboardPage.CurrentWindowHandle);
                    _providerSearch.RefreshPage();
                    automatedBase.CurrentPage = dashboardPage = newDuplicatePage.NavigateToDashboard();
                    dashboardPage.IsDefaultTestClientForEmberPage(ClientEnum.TTREE)
                        .ShouldBeTrue("Previous Client retained in Dashboard page?");


                    //StringFormatter.PrintMessage(
                    //    "Verify  behavior on navigation from core page to Retro page. Retro pages use client set in cookie");
                    //Enum.TryParse(_providerSearch.GetClientCookie(), out ClientEnum currentClientCookie);
                    //currentClientCookie.ShouldBeEqual(ClientEnum.TTREE, "Is client cookie set to TTREE?");

                    //StringFormatter.PrintMessage(" Verify TTREE is used as the current client while navigating to retro page");
                    //dashboardPage.SwitchTab(SiteDriver.CurrentWindowHandle);
                    //dashboardPage.ClickOnQuickLaunch();
                    //automatedBase.CurrentPage.IsClientLogoPresentForRetroPage(currentClientCookie)
                    //    .ShouldBeTrue("client retained while navigating to retro page?");
                    //automatedBase.CurrentPage.SwitchTab(SiteDriver.CurrentWindowHandle);
                    //automatedBase.CurrentPage.ClickOnQuickLaunch();
                    //automatedBase.CurrentPage.IsClientLogoPresentForRetroPage(currentClientCookie)
                    //    .ShouldBeTrue("client changes according to cookie?");
                }
                finally
                {
                    automatedBase.CurrentPage.CloseAnyTabIfExist();
                }
            }



        }
        
        [Test] //TE-522 + CAR-2427 [CAR-2901]
        [NonParallelizable]
        public void Verify_Switch_Client_Functionality_For_Core_Pages()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                UserProfileSearchPage _newUserProfileSearch;
                var mostRecentClientData =
                        automatedBase.QuickLaunch.GetMostRecentClientDetailsFromDatabase(automatedBase.EnvironmentManager.Username);
                var allClientData =
                    automatedBase.QuickLaunch.GetAllClientDetailsFromDatabase(automatedBase.EnvironmentManager.Username);
                var testClientUser = automatedBase.EnvironmentManager.Username;

                automatedBase.CurrentPage.IsCurrentClientAsExpected(ClientEnum.SMTST)
                    .ShouldBeTrue("User's default client should be shown");

                automatedBase.CurrentPage.Logout().LoginAsHciAdminUser();
                _newUserProfileSearch = automatedBase.CurrentPage.NavigateToNewUserProfileSearch();
                _newUserProfileSearch.SearchUserByNameOrId(new List<string> { testClientUser }, true);
                _newUserProfileSearch.ClickOnGridRowByUserIdToOpenUserSettingSideView(testClientUser);
                _newUserProfileSearch.ClickOnUserSettingTabByTabName(
                    UserSettingsTabEnum.Preferences.GetStringValue());

                var defaultPage =
                    _newUserProfileSearch.GetInputTextBoxValueByLabel(
                        UserPreferencesEnum.DefaultPage.GetStringValue());

                _newUserProfileSearch.SearchUserByNameOrId
                    (new List<string> { automatedBase.EnvironmentManager.ClientUserWithSuspectProviderDefaultPage }, true);
                _newUserProfileSearch.ClickOnGridRowByUserIdToOpenUserSettingSideView(automatedBase.EnvironmentManager
                    .ClientUserWithSuspectProviderDefaultPage);
                _newUserProfileSearch.ClickOnUserSettingTabByTabName(
                    UserSettingsTabEnum.Preferences.GetStringValue());

                var defaultCorePage =
                    _newUserProfileSearch.GetInputTextBoxValueByLabel(UserPreferencesEnum.DefaultPage
                        .GetStringValue());
                var _claimSearchPage =
                    automatedBase.CurrentPage.Logout().LoginAsClientUser().NavigateToClaimSearch();

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
                        "'Client Codes Should Match in 'All Clients' sub-list");
                _claimSearchPage.GetClientCodesOrClientNames(false, true)
                    .ShouldCollectionBeEqual(allClientData.Select(x => x[1]).ToList(),
                        "Client Names Should Match in 'All Clients' sub-list");

                StringFormatter.PrintMessageTitle("Verifying switching the client");
                _claimSearchPage.TypeAheadInSwitchClientPage(ClientEnum.TTREE);
                _claimSearchPage.GetClientCodesOrClientNames(false, false)
                    .ShouldCollectionBeEqual(new List<string>() { ClientEnum.TTREE.ToString() },
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
                automatedBase.QuickLaunch.Logout().LoginClientUserWithDefaultSuspectProvidersPage();
                _claimSearchPage = automatedBase.QuickLaunch.NavigateToClaimSearch();
                _claimSearchPage.ClickOnSwitchClient();
                _claimSearchPage.SwitchClientTo(ClientEnum.TTREE);
                automatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(defaultCorePage,
                    "User should be navigated to default page after client switch");
                automatedBase.CurrentPage.CurrentPageUrl.Contains(ClientEnum.TTREE.ToString())
                    .ShouldBeTrue("Client should be changed");

                StringFormatter.PrintMessageTitle("Log in as user which has only one assigned client to it");
                automatedBase.CurrentPage.Logout().LoginAsClientUserWithoutManageEditsPrivilege();
                automatedBase.CurrentPage.GetCurrentClient().ShouldBeEqual("Selenium Test Client",
                    "The default client for the user should be SMTST");
                automatedBase.CurrentPage.IsSwitchClientCaretSignPresent()
                    .ShouldBeFalse(
                        "Switch client field not be a dropdown when there is only one client that is assigned");

            }
        }

        [Test, Category("SchemaDependent")]  //TE-610
        public void Verify_Release_All_Locks_On_Logout_For_Client_User()
        {
            using (var automatedBase = new AutomatedBaseClientParallel(3))
            {
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                ProviderSearchPage _providerSearch;
                ProviderActionPage _providerActionPage;
                AppealSearchPage _appealSearch;
                AppealActionPage _appealAction;
                ClaimSearchPage _claimSearch;
                ClaimActionPage _claimAction;
                PreAuthSearchPage _newPreAuthSearch;
                PreAuthActionPage _newPreAuthAction;
                var paramList = automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var prvseq = paramList["providerseq"];
                var appealseq = paramList["appealseq"];
                var claseq = paramList["claimseq"].Split('-')[0];
                var clasub = paramList["claimseq"].Split('-')[1];
                var preAuthSeq = paramList["preauthseq"];

                try
                {
                    var userID = automatedBase.EnvironmentManager.Username;
                    _providerSearch = automatedBase.QuickLaunch.NavigateToProviderSearch();
                    _providerSearch.SearchByProviderSequence(prvseq);
                    _providerActionPage =
                        _providerSearch.ClickOnProviderSequenceAndNavigateToProviderActionPage(prvseq);

                    var newFirstTab = _providerSearch.SwitchTabAndNavigateToQuickLaunchPage();
                    newFirstTab.ClickOnSwitchClient().SwitchClientTo(ClientEnum.TTREE);
                    _appealSearch = newFirstTab.NavigateToAppealSearch();
                    _appealSearch.SelectAllAppeals();
                    _appealSearch.GetSideBarPanelSearch.SelectDropDownListValueByLabel("Client", "TTREE");
                    _appealAction =
                        _appealSearch.SearchByAppealSequenceToNavigateToAppealSummaryForClientSwitch(appealseq);


                    var secondTab = _appealSearch.SwitchTabAndNavigateToQuickLaunchPage(false, 2);
                    _claimSearch = secondTab.NavigateToClaimSearch();
                    _claimAction =
                        _claimSearch.SearchByClaimSequenceToNavigateToClaimActionForClientSwitch(claseq, true);

                    var thirdTab = _claimAction.SwitchTabAndNavigateToQuickLaunchPage(false, 3);
                    thirdTab.ClickOnSwitchClient().SwitchClientTo(ClientEnum.PEHP);
                    _newPreAuthSearch = thirdTab.NavigateToPreAuthSearch();
                    _newPreAuthAction =
                        _newPreAuthSearch.SearchByAuthSequenceAndNavigateToAuthActionAndHandlePopup(preAuthSeq, false);

                    var duplicateTab = _newPreAuthAction.SwitchTabAndNavigateToQuickLaunchPage(false, 4);


                    _providerActionPage.IsProviderLocked(ClientEnum.SMTST.ToString(), prvseq, userID)
                        .ShouldBeTrue("Provider lock retained on multiple client switch?");
                    _claimSearch.IsClaimLocked(ClientEnum.TTREE.GetStringDisplayValue(), claseq, clasub, userID)
                        .ShouldBeTrue("claim lock retained on multiple client switch?");
                    _appealAction.IsAppealLocked(appealseq, userID)
                        .ShouldBeTrue("Appeal lock retained on multiple client switch?");
                    _newPreAuthAction.IsPreAuthLocked(ClientEnum.PEHP.ToString(), preAuthSeq, userID)
                        .ShouldBeTrue("preauth lock retained on multiple client switch?");

                    duplicateTab.Logout().LoginAsClientUser();
                    _providerActionPage.GetLockedProviderCountByUser(ClientEnum.SMTST.ToString(), userID)
                        .ShouldBeEqual(0, "provider locked released?");
                    _appealAction.GetAppealLockCountByUser(userID).ShouldBeEqual(0, "Appeal lock released on logout?");
                    _claimAction.GetLockedClaimCountByUser(userID, ClientEnum.TTREE.GetStringDisplayValue())
                        .ShouldBeEqual(0, "claim lock release on log out?");
                    _newPreAuthAction.GetLockedPreauthCountByUser(userID, ClientEnum.PEHP.ToString())
                        .ShouldBeEqual(0, "preauth lock release on logout?");

                }
                finally
                {
                    automatedBase.CurrentPage.CloseAnyPopupIfExist();

                }
            }

        }

        [Test] //TE-762
        [NonParallelizable]
        public void Verify_Remove_Lock_Functionality() // TE-762
        {
            using (var automatedBase = new AutomatedBaseClientParallel(3))
            {
                ClaimActionPage _claimAction;
                ClaimSearchPage _claimSearch;
                StringFormatter.PrintMessage("Generate FCI worklist");
                _claimAction = automatedBase.QuickLaunch.NavigateToFciClaimsWorkList(true);
                automatedBase.QuickLaunch =
                    _claimAction.SwitchTabAndOpenQuickLaunchPageCloseFirstTab(automatedBase.Login.LoginAsClientMstrUser);
                StringFormatter.PrintMessage("Generate CV worklist");
                _claimAction = automatedBase.QuickLaunch.NavigateToCVClaimsWorkList(true);

                _claimAction.GetSideBarPanelSearch.OpenSidebarPanel();
                _claimAction.WaitForAdditionalLockedClaims();
                var lockedClaimsList = _claimAction.GetSideBarPanelSearch.GetAddtionallockedClaims();
                var claseq = lockedClaimsList.FirstOrDefault();

                StringFormatter.PrintMessage("Verify Remove lock functionality in additional locked claims section");
                _claimAction.IsClaimSearchResultsLocked(automatedBase.EnvironmentManager.ClientMstrtUser, lockedClaimsList)
                    .ShouldBeTrue("claims of Fci worklist should be locked");
                _claimAction.GetSideBarPanelSearch.ClickOnRemoveLock(claseq);
                _claimAction.GetSideBarPanelSearch.GetAddtionallockedClaims().Count
                    .ShouldBeEqual(lockedClaimsList.Count - 1);
                _claimAction.IsClaimLocked(ClientEnum.SMTST.ToString(), claseq.Split('-')[0], claseq.Split('-')[1],
                    automatedBase.EnvironmentManager.ClientMstrtUser).ShouldBeFalse($"Is claim {claseq} locked?");

                StringFormatter.PrintMessage(
                    " Verify clicking on search icon, only removes the lock of the current worklist and locks of previous worklist is retained");
                _claimSearch = _claimAction.ClickClaimSearchIcon();
                _claimSearch.GetSideBarPanelSearch.OpenSidebarPanel();
                _claimAction.WaitForAdditionalLockedClaims();
                _claimSearch.GetSideBarPanelSearch.GetAddtionallockedClaims().FirstOrDefault()
                    .ShouldBeEqual(lockedClaimsList[1], "FCI Claim retained?");
            }
        }


        //[Test]//US28488 and US67762
        //public void Verify_Dashboard_icon_for_users_with_Dashboard_authority_and_navigation_to_Dashboard_page_client_user()
        //{
        //    TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
        //    var expectedDashboardProductTypeList = DataHelper
        //        .GetMappingData(FullyQualifiedClassName, "Dashboard_Product_Type_List").Values.ToList();
        //    expectedDashboardProductTypeList.Insert(0, "");

        //    automatedBase.QuickLaunch.IsDashboardIconPresent().ShouldBeTrue("Is Dashboard icon present?");

        //    _dashboard = automatedBase.QuickLaunch.NavigateToDashboard();
        //    _dashboard.CurrentPageTitle.ShouldBeEqual(PageHeaderEnum.Dashboard.GetStringValue(), "Is Dashboard page title Equals?");
        //    _profileManager = _dashboard.NavigateToProfileManager();
        //    _profileManager.IsUserPrefernceDefaultDashboardViewLabelPresent().ShouldBeTrue
        //            ("Dashboard view should be present in the Preferences section in New User Profile Summary");
        //    _profileManager.GetDashboardViewProductType().ShouldCollectionBeEqual(expectedDashboardProductTypeList, "Default Dashboard View Dropdown list");
        //    _profileManager.IsAuthorityAssigned("Product Dashboards").ShouldBeTrue("Is <Dashboard> authority assigned?");

        //}


        //[Test] //US28488 and US67762
        //public void Verify_Dashboard_icon_for_client_users_without_Dashboard_authority()
        //{
        //    StringFormatter.PrintMessage("The logged in User uiautomation_cli does not have Dashboard authority");
        //    automatedBase.QuickLaunch.Logout().LoginAsClientUserWithNoAnyAuthority().ClickOnQuickLaunch();//uiautomation_cli
        //    automatedBase.QuickLaunch.IsDashboardIconPresent().ShouldBeFalse("Is Dashboard icon present?");

        //    //_profileManager = automatedBase.QuickLaunch.NavigateToProfileManager();
        //    //_profileManager.IsUserPrefernceDefaultDashboardViewLabelPresent().ShouldBeFalse
        //    //    ("Is Dashboard view present in the Preferences section in New User Profile Summary?");
        //}

        //[Test]//US67762
        //public void Verify_dashboard_icon_for_users_with_Read_only_authority()
        //{
        //    TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
        //    var expectedDashboardProductTypeList = DataHelper
        //        .GetMappingData(FullyQualifiedClassName, "Dashboard_Product_Type_List").Values.ToList();
        //    expectedDashboardProductTypeList.Insert(0, "");

        //    automatedBase.QuickLaunch.Logout().LoginAsClientUserWithReadOnlyAccessToAllAuthorities();
        //    automatedBase.QuickLaunch.IsDashboardIconPresent().ShouldBeTrue("Is Dashboard icon present?");

        //    _dashboard = automatedBase.QuickLaunch.NavigateToDashboard();
        //    _dashboard.CurrentPageTitle.ShouldBeEqual(PageHeaderEnum.Dashboard.GetStringValue(), "Is Dashboard page title Equals?");
        //    _profileManager = _dashboard.NavigateToProfileManager();
        //    _profileManager.IsUserPrefernceDefaultDashboardViewLabelPresent().ShouldBeTrue
        //        ("Dashboard view should be present in the Preferences section in New User Profile Summary");
        //    _profileManager.GetDashboardViewProductType().ShouldCollectionBeEqual(expectedDashboardProductTypeList, "Default Dashboard View Dropdown list");
        //    _profileManager.IsAuthorityAssigned("Product Dashboards").ShouldBeTrue("Is <Dashboard> authority assigned?");
        //    _profileManager.IsReadOnlyAssgined("Product Dashboards").ShouldBeTrue("Read-only authority should be assigned");
        //}



        //[Test]//US67763
        //public void Verify_default_page_as_New_ClaimSearch_page_for_users_with_default_page_Claim_Search_in_User_Preference_client_user()
        //{

        //    StringFormatter.PrintMessageTitle("Verify the default page preference as Claim Search navigates to the New Claim search page");
        //    automatedBase.QuickLaunch.Logout().LoginAsClientUserWithNoAnyAuthority();
        //    var claimSearch = EnvironmentManager.ApplicationUrl + PageUrlEnum.NewClaimSearch.GetStringValue();
        //    automatedBase.CurrentPage.CurrentPageUrl.ShouldBeEqual(claimSearch, "Page Url Should be New Claim Search");
        //    var profileManager = automatedBase.QuickLaunch.NavigateToProfileManager();
        //    profileManager.GetDefaultPagePreference().
        //        ShouldBeEqual(PageHeaderEnum.ClaimSearch.GetStringValue(), "Default page preference should be 'Claim Search'");
        //    automatedBase.QuickLaunch = profileManager.ClickOnQuickLaunch();

        //}

        [Test] //CAR 1493(CAR-1608)
        [Order(3)]
        public void Verify_client_user_with_ONLY_DCIWorklist_authority_and_no_other_worklist_authority_can_select_DCI_Claims_option_from_worklist_menu()
        {
            using (var automatedBase = new AutomatedBaseClientParallel(100))
            {
                ClaimActionPage _claimAction;
                automatedBase.Login.LoginAsClientUserwithONLYDCIWorklistauthority();
                _claimAction = automatedBase.CurrentPage.NavigateToDciClaimsWorkList();
                var claseq = _claimAction.GetClaimSequence();
                _claimAction.GetFlagCountByClaimSeqAndProductFromDatabase(claseq, "D").ShouldBeGreaterOrEqual(1,
                    "claims with at least one active DCA flag should be presented in work list");
                _claimAction.IsPageErrorPopupModalPresent()
                    .ShouldBeFalse("There should not be any popup when navigate to DCA Work List");
                _claimAction.ClickClaimSearchIcon();
            }
        }

        [Test] //CAR-2914(CAR-2837)
        [Order(2)]
        public void Verify_different_Cotiviti_Links_In_User_Greeting_Dropdown_Menu()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                IDictionary<string, string> paramLists =
                    automatedBase.DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
                var quickLinksMenuOptions = Enum.GetValues(typeof(QuickLinksEnum))
                    .Cast<QuickLinksEnum>().Select(x => x.GetStringValue()).ToList();
                var cotivitiLinksMenuOptions = Enum.GetValues(typeof(CotivitiLinksEnum))
                    .Cast<CotivitiLinksEnum>().Select(x => x.GetStringValue()).ToList();
                var i = 0;
                quickLinksMenuOptions.Remove(QuickLinksEnum.CotivitiLinks.GetStringValue());
                quickLinksMenuOptions.Remove(QuickLinksEnum.CotivitiFlaggedProviders.GetStringValue());
                foreach (var link in quickLinksMenuOptions)
                {
                    automatedBase.CurrentPage.MouseOverQuickLinks();
                    if (link == QuickLinksEnum.CMS.GetStringValue())
                        ClickOnALinkAndGetUrl(() => automatedBase.CurrentPage.ClickOnCotivitiLinksByLinkName(link), automatedBase.CurrentPage)
                            .ShouldBeEqual(paramLists["CmsDatabaseUrl"], "Url should match");
                    else
                    {
                        var providerSearchPage = automatedBase.CurrentPage
                            .ClickOnProviderSearchLinkAndNavigateToProviderSearchPage(link);
                        providerSearchPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.ProviderSearch.GetStringValue(),
                            "User Should Be Navigated to Provider Search Page");
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
                    ClickOnALinkAndGetUrl(() => automatedBase.CurrentPage.ClickOnCotivitiLinksByLinkName(link), automatedBase.CurrentPage)
                        .ShouldBeEqual(paramLists["CotivitiLinksUrls"].Split(',')[i], "Url should match");
                    i++;
                }
            }
        }


        #endregion

        #region private methods
        private string ClickOnALinkAndGetUrl(Action action, NewDefaultPage CurrentPage)
        {
            action();
            CurrentPage.SwitchToLastWindow();
            var url = CurrentPage.GetCurrentUrl();
            CurrentPage.SwitchTab("0");
            CurrentPage.CloseAnyTabIfExist();
            CurrentPage.ClickOnPageFooter();
            return url;
        }

        private void VerifyPageHeader(Func<bool> tilePresent, Action clickOnTile, PageHeaderEnum enumValue, NewDefaultPage CurrentPage)
        {
            tilePresent().ShouldBeTrue(string.Format("Is {0} tile present ? ", enumValue.GetStringValue()));
            clickOnTile();
            CurrentPage.GetPageHeader().ShouldBeEqual(enumValue.GetStringValue(), "Is Page Header Equal ?");
        }

        //private void VerifyPageHeaderForOtherPages(Func<bool> tilePresent,Action clickOnTile,PageUrlEnum pageUrl)
        //{
        //    try
        //    {
        //        tilePresent().ShouldBeTrue(string.Format("Is {0} tile present ? ", pageUrl));
        //        clickOnTile();
        //        automatedBase.CurrentPage.WaitForStaticTime(7000);
        //        automatedBase.CurrentPage.SwitchWindowByUrl(pageUrl.GetStringValue());
        //        automatedBase.CurrentPage.GetCurrentUrl().AssertIsContained(pageUrl.GetStringValue(),"Is Url Correct ?");
        //    }
        //    finally
        //    {
        //        automatedBase.CurrentPage.CloseAnyTabIfExist();
        //    }
        //}
        #endregion
    }
}

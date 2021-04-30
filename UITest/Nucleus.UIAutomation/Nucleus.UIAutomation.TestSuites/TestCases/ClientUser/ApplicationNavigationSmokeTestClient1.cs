using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Nucleus.Service.PageObjects.PreAuthorization;
using Nucleus.Service.PageServices.Appeal;
using Nucleus.Service.PageServices.Batch;
using Nucleus.Service.PageServices.Claim;
using Nucleus.Service.PageServices.Dashboard;
using Nucleus.Service.PageServices.Microstrategy;
using Nucleus.Service.PageServices.PreAuthorization;
using Nucleus.Service.Support.Common.Constants;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Environment;
using Nucleus.Service.Support.Utils;
using Nucleus.UIAutomation.TestSuites.Base;
using NUnit.Framework;
using UIAutomation.Framework.Core.Navigation;

namespace Nucleus.UIAutomation.TestSuites.TestCases.ClientUser
{
    public class ApplicationNavigationSmokeTestClient1 : AutomatedBaseClient
    {
        #region PRIVATE FIELDS

        private NewClaimActionPage _newClaimAction;
        private AppealCreatorPage _appealCreator;
        private NewAppealActionPage _newAppealAction;
        private NewAppealSearchPage _newAppealSearch;
        private PreAuthSearchPage _preAuthSearchPage;
        private NewBatchSearchPage _newBatchSearch;
        private DashboardPage _dashboard;

        #endregion

        #region PROTECTED FIELDS

        #endregion

        #region OVERRIDE
        protected override void ClassInit()
        {
            try
            {
                UserLoginIndex = 2;
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

            }

            finally
            {
                base.ClassCleanUp();
            }
        }

        protected override void TestCleanUp()
        {
            base.TestCleanUp();
            if (!CurrentPage.IsCurrentClientAsExpected(EnvironmentManager.Instance.TestClient))
            {
                CheckTestClientAndSwitch();
            }

            if (string.Compare(UserType.CurrentUserType, UserType.CLIENT4, StringComparison.OrdinalIgnoreCase) != 0)
            {
                QuickLaunch = QuickLaunch.Logout().LoginAsClientUser4();
                CheckTestClientAndSwitch();
            }
            else if (!CurrentPage.GetPageHeader().Equals(PageHeaderEnum.QuickLaunch.GetStringValue()))
            {
                CurrentPage.ClickOnQuickLaunch();

            }
        }
        #endregion

        #region TEST SUITES

        //Tant-73
        [Test, Category("SmokeTestDeployment"), Category("Navigation1")]
        public void Verify_Quick_launch_icon_Switch_client_icon_and_Help_presence_in_Quick_launch_page()
        {
            CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.QuickLaunch.GetStringValue());
            Verify_Quick_Launch_Switch_Client_Help_Icons(false, true, true, () => CurrentPage.ClickOnQuickLaunch());
        }

        [Test, Category("SmokeTestDeployment"), Category("Navigation1")]
        public void Verify_Quick_launch_icon_Switch_client_icon_and_Help_presence_in_Help_center_page()
        {
            QuickLaunch.NavigateToHelpCenter();
            CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.HelpCenter.GetStringValue());
            Verify_Quick_Launch_Switch_Client_Help_Icons(true, true, false, () => CurrentPage.NavigateToHelpCenter());
        }

        //TANT-113,Tant-73
        [Test, Category("SmokeTestDeployment"), Category("Navigation1")]
        public void Verify_navigation_to_Claim_Search_page()
        {
            QuickLaunch.NavigateToNewClaimSearch();
            CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.ClaimSearch.GetStringValue());
            Verify_Quick_Launch_Switch_Client_Help_Icons(true, true, true, () => CurrentPage.NavigateToNewClaimSearch(),true);
        }

        

        [Test, Category("SmokeTestDeployment"), Category("Navigation1")]
        public void Verify_navigation_to_new_Logic_Search_page()
        {
            QuickLaunch.NavigateToNewLogicSearch();
            CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.LogicSearch.GetStringValue());
            Verify_Quick_Launch_Switch_Client_Help_Icons(true, true, true, () => CurrentPage.NavigateToNewLogicSearch(),true);
        }

        [Test, Category("SmokeTestDeployment"), Category("Navigation1")]
        public void Verify_navigation_to_PCI_Claims_page()
        {
            QuickLaunch.NavigateToPciClaimsWorkList(true);
            CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.ClaimAction.GetStringValue());
            Verify_Quick_Launch_Switch_Client_Help_Icons(true, true, true, () => CurrentPage.NavigateToPciClaimsWorkList(),true);

        }

        [Test, Category("SmokeTestDeployment"), Category("Navigation1")]
        public void Verify_navigation_to_FCI_Claims_page()
        {
            QuickLaunch.NavigateToFciClaimsWorkList(true);
            CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.ClaimAction.GetStringValue());
            Verify_Quick_Launch_Switch_Client_Help_Icons(true, true, true, () => CurrentPage.NavigateToFciClaimsWorkList(),true);
        }

        [Test, Category("SmokeTestDeployment"), Category("Navigation1")]
        public void Verify_navigation_to_FFP_Claims_page()
        {
            QuickLaunch.NavigateToFfpClaimsWorkList(true);
            CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.ClaimAction.GetStringValue());
            Verify_Quick_Launch_Switch_Client_Help_Icons(true, true, true, () => CurrentPage.NavigateToFfpClaimsWorkList(),true);
        }

        [Test, Category("SmokeTestDeployment"), Category("Navigation1")]
        public void Verify_navigation_to_DCI_Claims_page()
        {
            QuickLaunch.NavigateToDciClaimsWorkList(true);
            CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.ClaimAction.GetStringValue());
            Verify_Quick_Launch_Switch_Client_Help_Icons(true, true, true, () => CurrentPage.NavigateToDciClaimsWorkList(),true);
        }

        [Test, Category("SmokeTestDeployment"), Category("Navigation1")]
        public void Verify_navigation_to_Pre_Authorization_Search_page()
        {
            QuickLaunch.NavigateToPreAuthSearch();
            CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.PreAuthSearch.GetStringValue());
            Verify_Quick_Launch_Switch_Client_Help_Icons(true, true, true, () => CurrentPage.NavigateToPreAuthSearch(),true);
        }

        [Test, Category("SmokeTestDeployment"), Category("Navigation1")]
        public void Verify_navigation_to_Pre_Authorization_Action_page()
        {
            NaviateToPreAuthActionPageClient(1,2);
            CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.PreAuthAction.GetStringValue());
            Verify_Quick_Launch_Switch_Client_Help_Icons(true, true, true, () => NaviateToPreAuthActionPageClient(1,2),true);
        }

        [Test, Category("SmokeTestDeployment"), Category("Navigation1")]
        public void Verify_navigation_to_Pre_Authorization_Creator_page()
        {
            QuickLaunch.NavigateToPreAuthCreatorPage();
            CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.PreAuthCreator.GetStringValue());
            Verify_Quick_Launch_Switch_Client_Help_Icons(true, true, true, () => CurrentPage.NavigateToPreAuthSearch(),true);
        }

        [Test, Category("SmokeTestDeployment"), Category("Navigation1")]
        public void Verify_navigation_to_New_Provider_Search_page()
        {
            QuickLaunch.NavigateToNewProviderSearch();
            CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.ProviderSearch.GetStringValue());
            Verify_Quick_Launch_Switch_Client_Help_Icons(true, true, true, () => CurrentPage.NavigateToNewProviderSearch(),true);
        }

        [Test, Category("SmokeTestDeployment"), Category("Navigation1")]
        public void Verify_navigation_to_Suspect_Providers_page()
        {
            QuickLaunch.NavigateToSuspectProviders();
            CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.SuspectProviders.GetStringValue());
            Verify_Quick_Launch_Switch_Client_Help_Icons(true, true, true, () => CurrentPage.NavigateToSuspectProviders(),true);
        }

        [Test, Category("SmokeTestDeployment"), Category("Navigation1")]
        public void Verify_navigation_to_Suspect_Providers_WorkList_page()
        {
            QuickLaunch.NavigateToSuspectProvidersWorkList();
            CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.NewProviderAction.GetStringValue());
            Verify_Quick_Launch_Switch_Client_Help_Icons(true, true, true, () => CurrentPage.NavigateToSuspectProvidersWorkList(),true);
        }

      
        #endregion

        #region RPIVATE METHODS
        private void Verify_Quick_Launch_Switch_Client_Help_Icons(bool isQuickLaunch, bool isSwitchClientIcon,
            bool isHelpIcon, Action navigateToPage, bool isCorePage=false)
        {
            if (isQuickLaunch)
            {
                CurrentPage.IsQuickLaunchIconPresent().ShouldBeTrue("Quick lunch icon should be present");
                CurrentPage.ClickOnQuickLaunch();
                CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.QuickLaunch.GetStringValue(), "Page should be navigated to Quick Launch page");
                navigateToPage();
            }

            if (isSwitchClientIcon)
            {
                CurrentPage.IsSwitchClientIconPresent().ShouldBeTrue("Switch client icon should be present");
                CurrentPage.IsClientLogoPresentForRetroPage(ClientEnum.SMTST).ShouldBeFalse("Default client logo is not present");
                if (isCorePage)
                    CurrentPage.ClickOnSwitchClient().SwitchClientTo(ClientEnum.TTREE, true);
                else
                    CurrentPage.ClickOnSwitchClient().SwitchClientTo(ClientEnum.TTREE);
                CurrentPage.IsClientLogoPresentForRetroPage(ClientEnum.TTREE).ShouldBeTrue("Client should be changed",true);
                CurrentPage.ClickOnSwitchClient().SwitchClientTo(ClientEnum.SMTST);

                CurrentPage.IsClientLogoPresentForRetroPage(ClientEnum.SMTST).ShouldBeFalse("Default client logo is not present");
                navigateToPage();
            }

            else
                CurrentPage.IsSwitchClientIconPresent().ShouldBeFalse("Switch client icon should not be present");

            if (!isHelpIcon) return;
            CurrentPage.IsHelpCenterIconPresent().ShouldBeTrue("Quick lunch icon should be present");
            CurrentPage.NavigateToHelpCenter();
            CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.HelpCenter.GetStringValue(), "Help center page should be presented");
            navigateToPage();

        }

        public AppealCreatorPage NavigateToAppealCreatorPage()
        {
            _appealCreator = QuickLaunch.NavigateToAppealCreator();
            _appealCreator.SearchByClaimNoAndStayOnSearch("1");
            return _appealCreator.ClickOnFirstEnableCreateAppealIcon();
        }

        public PreAuthActionPage NaviateToPreAuthActionPageClient(int row,int col)
        {
            _preAuthSearchPage = QuickLaunch.NavigateToPreAuthSearch();
            return  _preAuthSearchPage.NaviateToPreAuthActionPageClient(row, col);
        }

        public AppealSummaryPage NavigateToAppealSummaryPage()
        {
            _newAppealSearch = QuickLaunch.NavigateToNewAppealSearch();
            return _newAppealSearch.NavigateToAppealSummaryPageBySideBarPannel();
        }


        public NewBatchSummaryPage NavigateToNewBatchSummaryPage()
        {
            _newBatchSearch = QuickLaunch.NavigateToNewBatchSearch();
            return _newBatchSearch.ClickOnBatchByRowCol(1,2);
        }

        public AppealsDetailPage NavigateToAppealsDetailPageViaDashboard()
        {
            _dashboard = QuickLaunch.NavigateToDashboard();
            return _dashboard.ClickOnAppealsDetailExpandIcon();
        }

        public ClaimsDetailPage NavigateToClaimsDetailPageViaDashboard()
        {
            _dashboard = QuickLaunch.NavigateToDashboard();
            return _dashboard.ClickOnClaimsDetailExpandIcon();
        }
        
        public DashboardLogicRequestsDetailsPage NavigateToLogicRequestsDetailPageViaDashboard()
        {
            _dashboard = QuickLaunch.NavigateToDashboard();
            return _dashboard.ClickOnLogicRequestsDetailClientExpandIcon();
        }
        
        public MicrostrategyPage NavigateToMicrostrategyPage()
        {
            _dashboard = QuickLaunch.NavigateToDashboard();
            return _dashboard.NavigateToMicrostrategyWithMultipleReports();
        }

        #endregion
    }
}

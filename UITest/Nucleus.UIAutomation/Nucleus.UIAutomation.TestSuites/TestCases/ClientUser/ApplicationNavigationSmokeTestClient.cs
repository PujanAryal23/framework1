using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Nucleus.Service.PageObjects.PreAuthorization;
using Nucleus.Service.PageServices.Appeal;
using Nucleus.Service.PageServices.Base.Default;
using Nucleus.Service.PageServices.Batch;
using Nucleus.Service.PageServices.Claim;
using Nucleus.Service.PageServices.Dashboard;
using Nucleus.Service.PageServices.Microstrategy;
using Nucleus.Service.PageServices.PreAuthorization;
using Nucleus.Service.PageServices.QuickLaunch;
using Nucleus.Service.Support.Common.Constants;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Environment;
using Nucleus.Service.Support.Utils;
using Nucleus.UIAutomation.TestSuites.Base;
using NUnit.Framework;
using NUnit.Framework.Internal;
using UIAutomation.Framework.Core.Navigation;

namespace Nucleus.UIAutomation.TestSuites.TestCases.ClientUser
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class ApplicationNavigationSmokeTestClient 
    {
        /*#region PRIVATE FIELDS

        private ClaimActionPage _claimAction;
        private AppealCreatorPage _appealCreator;
        private AppealActionPage _appealAction;
        private AppealSearchPage _appealSearch;
        private PreAuthSearchPage _preAuthSearchPage;
        private BatchSearchPage _batchSearch;
        private DashboardPage _dashboard;
        private COBClaimsDetailPage _cobClaimsDetail;
        #endregion

        #region PROTECTED FIELDS

        #endregion

        #region OVERRIDE
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

            }

            finally
            {
                base.ClassCleanUp();
            }
        }

        protected override void TestCleanUp()
        {
            if (CurrentPage.IsPageErrorPopupModalPresent())
                CurrentPage.ClosePageError();
            base.TestCleanUp();
            if (!CurrentPage.IsCurrentClientAsExpected(EnvironmentManager.TestClient))
            {
                CheckTestClientAndSwitch();
            }

            if (string.Compare(UserType.CurrentUserType, UserType.CLIENT, StringComparison.OrdinalIgnoreCase) != 0)
            {
                QuickLaunch = QuickLaunch.Logout().LoginAsClientUser();
                CheckTestClientAndSwitch();
            }
            CurrentPage.ClickOnLogo();
            //    else if (!CurrentPage.GetPageHeader().Equals(PageHeaderEnum.QuickLaunch.GetStringValue()))
            //    {
            //        CurrentPage.ClickOnQuickLaunch();

            //    }
        }
        #endregion
        */
        #region TEST SUITES

        //Tant-73
        //[Test, Category("SmokeTestDeployment"), Category("Navigation1")]
        //public void Verify_Quick_launch_icon_Switch_client_icon_and_Help_presence_in_Quick_launch_page()
        //{
        //    CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.QuickLaunch.GetStringValue());
        //    Verify_Quick_Launch_Switch_Client_Help_Icons(() => CurrentPage.ClickOnQuickLaunch());
        //}

        [Test, Category("SmokeTestDeployment"), Category("Navigation3")] //TANT-260
        public void Navigate_to_COB_DashBoardPage_client()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                DashboardPage _dashboard;
                var TestName = new StackFrame().GetMethod().Name;
                automatedBase.CurrentPage = _dashboard = automatedBase.QuickLaunch.NavigateToCOBDashboard();
                automatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageTitleEnum.Dashboard.GetStringValue());
                _dashboard.IsCobPresentInContainerHeaderWidgetOverviewByComponentTitle(DashboardOverviewTitlesEnum
                        .ClaimsOverview.GetStringValue())
                    .ShouldBeTrue("Is COB present in Claims Overview Widget?");
                _dashboard.IsCobPresentInContainerHeaderWidgetOverviewByComponentTitle(DashboardOverviewTitlesEnum
                        .AppealsOverview.GetStringValue())
                    .ShouldBeTrue("Is COB present in Appeals Overview Widget?");
                Verify_Quick_Launch_Switch_Client_Help_Icons(() => automatedBase.CurrentPage.NavigateToCOBDashboard(), automatedBase.CurrentPage,
                    true, false);
                automatedBase.CurrentPage.NavigateToCVDashboard();
            }
        }

        [Test, Category("SmokeTestDeployment"), Category("Navigation3")] //TANT-262
        public void Navigate_to_COB_Claims_Detail_Page_client()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                var TestName = new StackFrame().GetMethod().Name;
                COBClaimsDetailPage _cobClaimsDetail;
                _cobClaimsDetail = automatedBase.QuickLaunch.NavigateToCobClaimsDetailPage();
                automatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageTitleEnum.DashboardClaimsDetail.GetStringValue());
                _cobClaimsDetail.IsCobPresentInContainerHeaderWidgetOverviewByComponentTitle("Totals by Client")
                    .ShouldBeTrue("Is COB present in Totals by Client widget?");
                Verify_Quick_Launch_Switch_Client_Help_Icons(() => automatedBase.CurrentPage.NavigateToCobClaimsDetailPage(), automatedBase.CurrentPage, true,
                    false);
                automatedBase.CurrentPage.NavigateToCVDashboard();
            }
        }

        [Test, Category("SmokeTestDeployment"), Category("Navigation3")] //TANT-261
        public void Navigate_to_COB_Appeals_Detail_Page_client()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                var TestName = new StackFrame().GetMethod().Name;
                automatedBase.QuickLaunch.NavigateToCobAppealsDetailPage();
                automatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageTitleEnum.COBAppealsDetail.GetStringValue());
                Verify_Quick_Launch_Switch_Client_Help_Icons(() => automatedBase.CurrentPage.NavigateToCobAppealsDetailPage(), automatedBase.CurrentPage, true,
                    false);
                automatedBase.CurrentPage.NavigateToCVDashboard();
            }
        }

        [Test, Category("SmokeTestDeployment"), Category("Navigation1")]
        public void Navigate_to_help_center_page_through_help_center_button_client()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                var TestName = new StackFrame().GetMethod().Name;
                automatedBase.QuickLaunch.NavigateToHelpCenter();
                automatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.HelpCenter.GetStringValue());
            }
        }

        //TANT-113,Tant-73
        [Test, Category("SmokeTestDeployment"), Category("Navigation1")]
        public void Verify_navigation_to_Claim_Search_page()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                var TestName = new StackFrame().GetMethod().Name;
                automatedBase.QuickLaunch.NavigateToClaimSearch();
                automatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.ClaimSearch.GetStringValue());
                Verify_Quick_Launch_Switch_Client_Help_Icons(() => automatedBase.CurrentPage.NavigateToClaimSearch(), automatedBase.CurrentPage, true, true);
            }
        }



        [Test, Category("SmokeTestDeployment"), Category("Navigation1")]
        public void Verify_navigation_to_Logic_Search_page()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                var TestName = new StackFrame().GetMethod().Name;
                automatedBase.QuickLaunch.NavigateToLogicSearch();
                automatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.LogicSearch.GetStringValue());
                Verify_Quick_Launch_Switch_Client_Help_Icons(() => automatedBase.CurrentPage.NavigateToLogicSearch(), automatedBase.CurrentPage, true);
            }
        }

        //[Test, Category("SmokeTestDeployment"), Category("Navigation1")]
        //public void Verify_navigation_to_PCI_Claims_page()
        //{
        //    QuickLaunch.NavigateToPciClaimsWorkList(true);
        //    CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.ClaimAction.GetStringValue());
        //    Verify_Quick_Launch_Switch_Client_Help_Icons(false, true, false, () => CurrentPage.NavigateToPciClaimsWorkList(),true);

        //}

        //[Test, Category("SmokeTestDeployment"), Category("Navigation1")]
        //public void Verify_navigation_to_FCI_Claims_page()
        //{
        //    QuickLaunch.NavigateToFciClaimsWorkList(true);
        //    CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.ClaimAction.GetStringValue());
        //    Verify_Quick_Launch_Switch_Client_Help_Icons(false, true, false, () => CurrentPage.NavigateToFciClaimsWorkList(),true);
        //}

        //[Test, Category("SmokeTestDeployment"), Category("Navigation1")]
        //public void Verify_navigation_to_FFP_Claims_page()
        //{
        //    QuickLaunch.NavigateToFfpClaimsWorkList(true);
        //    CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.ClaimAction.GetStringValue());
        //    Verify_Quick_Launch_Switch_Client_Help_Icons(false, true, false, () => CurrentPage.NavigateToFfpClaimsWorkList(),true);
        //}

        [Test, Category("SmokeTestDeployment")]
        public void Verify_navigation_to_DCI_Claims_page()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                var TestName = new StackFrame().GetMethod().Name;

                automatedBase.QuickLaunch.NavigateToDciClaimsWorkList(true);
                automatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.ClaimAction.GetStringValue());
                Verify_Quick_Launch_Switch_Client_Help_Icons(
                    () => automatedBase.CurrentPage.NavigateToDciClaimsWorkList(), automatedBase.CurrentPage, true);
            }
        }

        [Test, Category("SmokeTestDeployment"), Category("Navigation1")]
        public void Verify_navigation_to_Pre_Authorization_Search_page()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                var TestName = new StackFrame().GetMethod().Name;
                automatedBase.QuickLaunch.NavigateToPreAuthSearch();
                automatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.PreAuthSearch.GetStringValue());
                Verify_Quick_Launch_Switch_Client_Help_Icons(() => automatedBase.CurrentPage.NavigateToPreAuthSearch(), automatedBase.CurrentPage, true);
            }
        }

        [Test, Category("SmokeTestDeployment"), Category("Navigation1")]
        public void Verify_navigation_to_Pre_Authorization_Action_page()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                var TestName = new StackFrame().GetMethod().Name;
                NaviateToPreAuthActionPageClient(automatedBase.QuickLaunch,1, 2);
                automatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.PreAuthAction.GetStringValue());
                Verify_Quick_Launch_Switch_Client_Help_Icons(() => NaviateToPreAuthActionPageClient(automatedBase.QuickLaunch,1, 2), automatedBase.CurrentPage, true);
            }
        }

        [Test, Category("SmokeTestDeployment"), Category("Navigation1")]
        public void Verify_navigation_to_Pre_Authorization_Creator_page()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                var TestName = new StackFrame().GetMethod().Name;
                automatedBase.QuickLaunch.NavigateToPreAuthCreatorPage();
                automatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.PreAuthCreator.GetStringValue());
                Verify_Quick_Launch_Switch_Client_Help_Icons(() => automatedBase.CurrentPage.NavigateToPreAuthSearch(), automatedBase.CurrentPage, true);
            }
        }

        [Test, Category("SmokeTestDeployment"), Category("Navigation1")]
        public void Verify_navigation_to_New_Provider_Search_page()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                var TestName = new StackFrame().GetMethod().Name;
                automatedBase.QuickLaunch.NavigateToProviderSearch();
                automatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.ProviderSearch.GetStringValue());
                Verify_Quick_Launch_Switch_Client_Help_Icons(() => automatedBase.CurrentPage.NavigateToProviderSearch(), automatedBase.CurrentPage, true);
            }
        }

        [Test, Category("SmokeTestDeployment"), Category("Navigation1")]
        public void Verify_navigation_to_Suspect_Providers_page()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                var TestName = new StackFrame().GetMethod().Name;
                automatedBase.QuickLaunch.NavigateToSuspectProviders();
                automatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.SuspectProviders.GetStringValue());
                Verify_Quick_Launch_Switch_Client_Help_Icons(() => automatedBase.CurrentPage.NavigateToSuspectProviders(), automatedBase.CurrentPage, true);
            }
        }

        [Test, Category("SmokeTestDeployment"), Category("Navigation1")]
        public void Verify_navigation_to_Suspect_Providers_WorkList_page()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                var TestName = new StackFrame().GetMethod().Name;
                automatedBase.QuickLaunch.NavigateToSuspectProvidersWorkList();
                automatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.ProviderAction.GetStringValue());
                Verify_Quick_Launch_Switch_Client_Help_Icons(() => automatedBase.CurrentPage.NavigateToSuspectProvidersWorkList(), automatedBase.CurrentPage,
                    true);
            }
        }

        [Test, Category("SmokeTestDeployment"), Category("Navigation1")]
        public void Verify_navigation_to_Appeal_search_page()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                var TestName = new StackFrame().GetMethod().Name;
                automatedBase.QuickLaunch.NavigateToAppealSearch();
                automatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.AppealSearch.GetStringValue());
                Verify_Quick_Launch_Switch_Client_Help_Icons(() => automatedBase.CurrentPage.NavigateToAppealSearch(), automatedBase.CurrentPage, true, true);
            }
        }

        [Test, Category("SmokeTestDeployment"), Category("Navigation1")]
        public void Verify_navigation_to_Appeal_Creator_page()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                var TestName = new StackFrame().GetMethod().Name;
                NavigateToAppealCreatorPage(automatedBase.QuickLaunch);
                automatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.AppealCreator.GetStringValue());
                Verify_Quick_Launch_Switch_Client_Help_Icons(() => NavigateToAppealCreatorPage(automatedBase.QuickLaunch), automatedBase.CurrentPage, true);
            }
        }

        [Test, Category("SmokeTestDeployment"), Category("AppealDependent")]
        public void Verify_navigation_to_Appeal_Summary_page()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                var TestName = new StackFrame().GetMethod().Name;
                NavigateToAppealSummaryPage(automatedBase.QuickLaunch);
                automatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.AppealSummary.GetStringValue());
                Verify_Quick_Launch_Switch_Client_Help_Icons(() => NavigateToAppealSummaryPage(automatedBase.QuickLaunch), automatedBase.CurrentPage, true);
            }
        }

        [Test, Category("SmokeTestDeployment")]
        public void Verify_navigation_to_Batch_Search_page()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                var TestName = new StackFrame().GetMethod().Name;
                automatedBase.QuickLaunch.NavigateToBatchSearch();
                automatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.BatchSearch.GetStringValue());
                Verify_Quick_Launch_Switch_Client_Help_Icons(() => automatedBase.CurrentPage.NavigateToBatchSearch(), automatedBase.CurrentPage, true);
            }
        }

        [Test, Category("SmokeTestDeployment")]
        public void Verify_navigation_to_Batch_Summary_page()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                var TestName = new StackFrame().GetMethod().Name;
                NavigateToBatchSummaryPage(automatedBase.QuickLaunch);
                automatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.BatchSummary.GetStringValue());
                Verify_Quick_Launch_Switch_Client_Help_Icons(() => NavigateToBatchSummaryPage(automatedBase.QuickLaunch), automatedBase.CurrentPage, true);
            }
        }

        [Test, Category("SmokeTestDeployment")]
        public void Verify_navigation_to_Dashboard()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                var TestName = new StackFrame().GetMethod().Name;
                automatedBase.CurrentPage.NavigateToDashboard();
                automatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.Dashboard.GetStringValue());
                Verify_Quick_Launch_Switch_Client_Help_Icons(() => automatedBase.CurrentPage.NavigateToDashboard(), automatedBase.CurrentPage, true);
            }
        }

        [Test, Category("SmokeTestDeployment")]
        public void Verify_navigation_to_Appeal_Details_via_Dashboard()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                var TestName = new StackFrame().GetMethod().Name;
                NavigateToAppealsDetailPageViaDashboard(automatedBase.QuickLaunch);
                automatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.DashboardAppealsDetail.GetStringValue());
                Verify_Quick_Launch_Switch_Client_Help_Icons(() => NavigateToAppealsDetailPageViaDashboard(automatedBase.QuickLaunch), automatedBase.CurrentPage, true);
            }
        }

        [Test, Category("SmokeTestDeployment")]
        public void Verify_navigation_to_Claims_Details_via_Dashboard()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                var TestName = new StackFrame().GetMethod().Name;
                NavigateToClaimsDetailPageViaDashboard(automatedBase.QuickLaunch);
                automatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.DashboardClaimsDetail.GetStringValue());
                Verify_Quick_Launch_Switch_Client_Help_Icons(() => NavigateToClaimsDetailPageViaDashboard(automatedBase.QuickLaunch), automatedBase.CurrentPage, true);
            }
        }

        [Test, Category("SmokeTestDeployment")]
        public void Verify_navigation_to_Logic_requests_Details_via_Dashboard()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                var TestName = new StackFrame().GetMethod().Name;
                NavigateToLogicRequestsDetailPageViaDashboard(automatedBase.QuickLaunch);
                automatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.DashboardLogicRequestsDetail.GetStringValue());
                Verify_Quick_Launch_Switch_Client_Help_Icons(() => NavigateToLogicRequestsDetailPageViaDashboard(automatedBase.QuickLaunch), automatedBase.CurrentPage,
                    true);
            }
        }

        [Test, Category("SmokeTestDeployment"), Category("Working")]
        public void Verify_navigation_to_Microstrategy()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                var TestName = new StackFrame().GetMethod().Name;
                NavigateToMicrostrategyPage(automatedBase.QuickLaunch);
                automatedBase.CurrentPage.GetTitle().ShouldBeEqual(PageTitleEnum.Microstrategy.GetStringValue());
                Verify_Quick_Launch_Switch_Client_Help_Icons(() => NavigateToMicrostrategyPage(automatedBase.QuickLaunch), automatedBase.CurrentPage);
            }
        }

        [Test, Category("SmokeTestDeployment")]
        public void Verify_navigation_to_Invoice_Search()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                var TestName = new StackFrame().GetMethod().Name;
                automatedBase.QuickLaunch.NavigateToInvoiceSearch();
                automatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.InvoiceSearch.GetStringValue());
                Verify_Quick_Launch_Switch_Client_Help_Icons(() => automatedBase.CurrentPage.NavigateToInvoiceSearch(), automatedBase.CurrentPage, true);
            }
        }

        [Test, Category("SmokeTestDeployment")]
        public void Verify_navigation_to_My_Profile()
        {
            using (var automatedBase = new AutomatedBaseClientParallel())
            {
                var TestName = new StackFrame().GetMethod().Name;
                automatedBase.QuickLaunch.NavigateToMyProfilePage();
                automatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.MyProfile.GetStringValue());
                Verify_Quick_Launch_Switch_Client_Help_Icons(() => automatedBase.CurrentPage.NavigateToMyProfilePage(), automatedBase.CurrentPage);
            }
        }

        #endregion

        #region RPIVATE METHODS
        private void Verify_Quick_Launch_Switch_Client_Help_Icons(Action navigateToPage, NewDefaultPage CurrentPage, bool isCorePage=false, bool needToSwitch=false)
        {
            
                CurrentPage.IsSwitchClientIconPresent().ShouldBeTrue("Switch client icon should be present");
            if (needToSwitch)
            {
                CurrentPage.GetCurrentClient().ShouldBeEqual(ClientEnum.SMTST.GetStringValue(), "Default client logo is not present");
                if (isCorePage)
                   
                        CurrentPage.ClickOnSwitchClient().SwitchClientTo(ClientEnum.TTREE);
                else
                    CurrentPage.ClickOnSwitchClient().SwitchClientTo(ClientEnum.TTREE);
                CurrentPage.GetCurrentClient().ShouldBeEqual(ClientEnum.TTREE.GetStringValue(), "Client should be changed", true);
                CurrentPage.ClickOnSwitchClient().SwitchClientTo(ClientEnum.SMTST);
                CurrentPage.GetCurrentClient().ShouldBeEqual(ClientEnum.SMTST.GetStringValue(), "Default client logo is not present");
                navigateToPage();
            }
            CurrentPage.IsHelpCenterIconPresent().ShouldBeTrue("Help center icon should be present");
            CurrentPage.NavigateToHelpCenter();
            CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.HelpCenter.GetStringValue(), "Help center page should be presented");
            navigateToPage();

        }

        public AppealCreatorPage NavigateToAppealCreatorPage(QuickLaunchPage QuickLaunch)
        {
            AppealCreatorPage _appealCreator;
            _appealCreator = QuickLaunch.NavigateToAppealCreator();
            _appealCreator.SearchByClaimNoAndStayOnSearch("1");
            return _appealCreator.ClickOnFirstEnableCreateAppealIcon();
        }

        public PreAuthActionPage NaviateToPreAuthActionPageClient(QuickLaunchPage QuickLaunch,int row,int col)
        {
            PreAuthSearchPage _preAuthSearchPage;
            _preAuthSearchPage = QuickLaunch.NavigateToPreAuthSearch();
            return  _preAuthSearchPage.NaviateToPreAuthActionPageClient(row, col);
        }

        public AppealSummaryPage NavigateToAppealSummaryPage(QuickLaunchPage QuickLaunch)
        {
            AppealSearchPage _appealSearch;
            _appealSearch = QuickLaunch.NavigateToAppealSearch();
            return _appealSearch.NavigateToAppealSummaryPageBySideBarPannel();
        }


        public BatchSummaryPage NavigateToBatchSummaryPage(QuickLaunchPage QuickLaunch)
        {
            BatchSearchPage _batchSearch;
            _batchSearch = QuickLaunch.NavigateToBatchSearch();
            return _batchSearch.ClickOnBatchByRowCol(1,2);
        }

        public AppealsDetailPage NavigateToAppealsDetailPageViaDashboard(QuickLaunchPage QuickLaunch)
        {
            DashboardPage _dashboard;
            _dashboard = QuickLaunch.NavigateToDashboard();
            return _dashboard.ClickOnAppealsDetailExpandIcon();
        }

        public ClaimsDetailPage NavigateToClaimsDetailPageViaDashboard(QuickLaunchPage QuickLaunch)
        {
            DashboardPage _dashboard;
            _dashboard = QuickLaunch.NavigateToDashboard();
            return _dashboard.ClickOnClaimsDetailExpandIcon();
        }
        
        public DashboardLogicRequestsDetailsPage NavigateToLogicRequestsDetailPageViaDashboard(QuickLaunchPage QuickLaunch)
        {
            DashboardPage _dashboard;
            _dashboard = QuickLaunch.NavigateToDashboard();
            return _dashboard.ClickOnLogicRequestsDetailClientExpandIcon();
        }
        
        public MicrostrategyPage NavigateToMicrostrategyPage(QuickLaunchPage QuickLaunch)
        {
            DashboardPage _dashboard;
            _dashboard = QuickLaunch.NavigateToDashboard();
            return _dashboard.NavigateToMicrostrategyWithMultipleReports();
        }

        #endregion
    }
}

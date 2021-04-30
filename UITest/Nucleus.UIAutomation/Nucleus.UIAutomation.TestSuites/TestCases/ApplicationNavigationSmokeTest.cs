using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nucleus.Service.PageObjects.Settings.User;
using Nucleus.Service.PageServices.Appeal;
using Nucleus.Service.PageServices.Base.Default;
using Nucleus.Service.PageServices.Batch;
using Nucleus.Service.PageServices.Claim;
using Nucleus.Service.PageServices.Dashboard;
using Nucleus.Service.PageServices.PreAuthorization;
using Nucleus.Service.PageServices.QuickLaunch;
using Nucleus.Service.PageServices.Settings;
using Nucleus.Service.PageServices.Settings.User;
using Nucleus.Service.Support.Common.Constants;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Environment;
using Nucleus.Service.Support.Utils;
using Nucleus.UIAutomation.TestSuites.Base;
using NUnit.Framework;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Core.Navigation;

namespace Nucleus.UIAutomation.TestSuites.TestCases
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class ApplicationNavigationSmokeTest
    {
        /*#region PRIVATE FIELDS

        private ClaimActionPage _claimAction;
        private AppealCreatorPage _appealCreator;
        private AppealActionPage _appealAction;
        private AppealSearchPage _appealSearch;
        private PreAuthSearchPage _preAuthSearchPage;
        private BatchSearchPage _batchSearch;
        private DashboardPage _dashboard;
        private ClientSearchPage _clientSearch;
        private EditSettingsManagerPage _editSettingsManagerPage;
        private RoleManagerPage _roleManagerPage;
        private UserProfileSearchPage _userProfileSearch;
        private AppealManagerPage _appealManager;
        private COBClaimsDetailPage _claimsDetail;

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
            if(CurrentPage.IsPageErrorPopupModalPresent())
                CurrentPage.ClosePageError();
            base.TestCleanUp();
            if (!CurrentPage.IsCurrentClientAsExpected(EnvironmentManager.TestClient))
            {
                CheckTestClientAndSwitch();
            }

            if (string.Compare(UserType.CurrentUserType, UserType.HCIADMIN, StringComparison.OrdinalIgnoreCase) != 0)
            {
                QuickLaunch = QuickLaunch.Logout().LoginAsHciAdminUser();
                CheckTestClientAndSwitch();
            }
            //else if (!CurrentPage.GetPageHeader().Equals(PageHeaderEnum.QuickLaunch.GetStringValue()))
            //{
            //    CurrentPage.ClickOnQuickLaunch();

            //}
            CurrentPage.ClickOnLogo();

        }
        #endregion*/

        #region TEST SUITES
        //TANT-73
        //[Test, Category("SmokeTestDeployment"), Category("Navigation1")]
        //public void Verify_Quick_launch_icon_Switch_client_icon_and_Help_presence_in_Quick_launch_page()
        //{
        //    CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.QuickLaunch.GetStringValue());
        //    Verify_Quick_Launch_Switch_Client_Help_Icons(() => CurrentPage.ClickOnQuickLaunch());
        //}

        [Test, Category("SmokeTestDeployment"), Category("Navigation3")] //TANT-260
        public void Navigate_to_COB_DashBoardPage()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                DashboardPage _dashboard;
                _dashboard = automatedBase.QuickLaunch.NavigateToCOBDashboard();
                automatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageTitleEnum.Dashboard.GetStringValue());
                _dashboard.IsCobPresentInContainerHeaderWidgetOverviewByComponentTitle(DashboardOverviewTitlesEnum
                        .ClaimsOverview.GetStringValue())
                    .ShouldBeTrue("Is COB present in Claims Overview Widget?");
                _dashboard.IsCobPresentInContainerHeaderWidgetOverviewByComponentTitle(DashboardOverviewTitlesEnum
                        .AppealsOverview.GetStringValue())
                    .ShouldBeTrue("Is COB present in Appeals Overview Widget?");
                Verify_Quick_Launch_Switch_Client_Help_Icons(automatedBase.CurrentPage,() => automatedBase.CurrentPage.NavigateToCOBDashboard(), false, true,
                    false);
                automatedBase.CurrentPage.NavigateToCVDashboard();

            }
        }

        [Test, Category("SmokeTestDeployment"), Category("Navigation3")] //TANT-262
        public void Navigate_to_COB_Claims_Detail_Page()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                COBClaimsDetailPage _claimsDetail;
                _claimsDetail = automatedBase.QuickLaunch.NavigateToCobClaimsDetailPage();
                automatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageTitleEnum.DashboardClaimsDetail.GetStringValue());
                _claimsDetail
                    .IsCobPresentInContainerHeaderWidgetOverviewByComponentTitle(COBClaimsDetailEnum.TotalsByFlag
                        .GetStringValue())
                    .ShouldBeTrue("Is COB present in Totals by Flag widget?");
                _claimsDetail
                    .IsCobPresentInContainerHeaderWidgetOverviewByComponentTitle(COBClaimsDetailEnum.TotalsByBatch
                        .GetStringValue())
                    .ShouldBeTrue("Is COB present in Totals by Batch widget?");
                _claimsDetail
                    .IsCobPresentInContainerHeaderWidgetOverviewByComponentTitle(COBClaimsDetailEnum.TotalsByClient
                        .GetStringValue())
                    .ShouldBeTrue("Is COB present in Totals by Client widget?");
                Verify_Quick_Launch_Switch_Client_Help_Icons(automatedBase.CurrentPage,() => automatedBase.CurrentPage.NavigateToCobClaimsDetailPage(), false,
                    true, false);
                automatedBase.CurrentPage.NavigateToCVDashboard();
            }
        }

        [Test, Category("SmokeTestDeployment"), Category("Navigation3")] //TANT-261
        public void Navigate_to_COB_Appeals_Detail_Page()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                automatedBase.QuickLaunch.NavigateToCobAppealsDetailPage();
                automatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageTitleEnum.COBAppealsDetail.GetStringValue());
                Verify_Quick_Launch_Switch_Client_Help_Icons(automatedBase.CurrentPage,() => automatedBase.CurrentPage.NavigateToCobAppealsDetailPage(), false,
                    true, false);
                automatedBase.CurrentPage.NavigateToCVDashboard();
            }
        }

        [Test, Category("SmokeTestDeployment"), Category("Navigation1")] //CAR-2902(CAR-2744)
        public void Navigate_to_help_center_page_through_help_center_button()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                automatedBase.QuickLaunch.NavigateToHelpCenter();
                automatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.HelpCenter.GetStringValue());
            }
        }
        //TANT-113 //TANT-73

        [Test, Category("SmokeTestDeployment"), Category("Navigation1")]
        public void Verify_navigation_to_Appeal_search_page()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                automatedBase.QuickLaunch.NavigateToAppealSearch();
                automatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.AppealSearch.GetStringValue());
                Verify_Quick_Launch_Switch_Client_Help_Icons(automatedBase.CurrentPage,() => automatedBase.CurrentPage.NavigateToAppealSearch(), false, true,
                    true);
            }
        }

        [Test, Category("SmokeTestDeployment"), Category("Navigation1")]
        public void Verify_navigation_to_Appeal_Creator_page()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                NavigateToAppealCreatorPage(automatedBase.QuickLaunch);
                automatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.AppealCreator.GetStringValue());
                Verify_Quick_Launch_Switch_Client_Help_Icons(automatedBase.CurrentPage,() => NavigateToAppealCreatorPage(automatedBase.QuickLaunch), false, true);
            }
        }

        [Test, Category("SmokeTestDeployment"), Category("Navigation3"), Category("AppealDependent")] //+TANT-95
        public void Verify_navigation_to_a_My_Appeals_page()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealActionPage _appealAction;
                AppealSearchPage _appealSearch;
                _appealAction = automatedBase.QuickLaunch.NavigateToMyAppeals(false);
                try
                {
                    
                   
                    Console.WriteLine(_appealAction.GetPageHeader());
                    _appealAction.GetWindowHandlesCount().ShouldBeEqual(2,
                        "There should be two window one is appeal action and another is claim action popup");
                    var claimActionPopup =
                        _appealAction.SwitchClaimActionPopup(PageUrlEnum.ClaimAction.GetStringValue());
                    claimActionPopup.GetPageHeader().ShouldBeEqual(PageHeaderEnum.ClaimAction.GetStringValue(),
                        "Claim Action Popup should appears when navigate to my appeal");
                    _appealAction.CloseAnyTabIfExist();

                    automatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.AppealAction.GetStringValue());
                    Verify_Quick_Launch_Switch_Client_Help_Icons(automatedBase.CurrentPage,() => automatedBase.CurrentPage.NavigateToMyAppeals(), true, true);
                }
                finally
                {
                    automatedBase.CurrentPage.CloseAnyTabIfExist();
                    automatedBase.CurrentPage = _appealSearch = _appealAction.ClickOnSearchIconToNavigateAppealSearchPage();
                }

            }
        }

        [Test, Category("SmokeTestDeployment"), Category("Navigation1")]
        public void Verify_navigation_to_Appeals_Manager_page()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                automatedBase.QuickLaunch.NavigateToAppealManager();
                automatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.AppealManager.GetStringValue());
                Verify_Quick_Launch_Switch_Client_Help_Icons(automatedBase.CurrentPage,() => automatedBase.CurrentPage.NavigateToAppealManager(), false, true);
            }
        }

        [Test, Category("SmokeTestDeployment"), Category("Navigation3")]
        public void Verify_navigation_to_Appeal_Rationale_Manager_page()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                automatedBase.QuickLaunch.NavigateToAppealRationaleManager();
                automatedBase.CurrentPage.GetPageHeader()
                    .ShouldBeEqual(PageHeaderEnum.AppealRationaleManager.GetStringValue());
                Verify_Quick_Launch_Switch_Client_Help_Icons(automatedBase.CurrentPage,
                    () => automatedBase.CurrentPage.NavigateToAppealRationaleManager(), false, true);
            }
        }

        [Test, Category("SmokeTestDeployment"), Category("Navigation1")]
        public void Verify_navigation_to_Appeal_Category_Manager_page()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                automatedBase.QuickLaunch.NavigateToAppealCategoryManager();
                automatedBase.CurrentPage.GetPageHeader()
                    .ShouldBeEqual(PageHeaderEnum.AppealCategoryManager.GetStringValue());
                Verify_Quick_Launch_Switch_Client_Help_Icons(automatedBase.CurrentPage,
                    () => automatedBase.CurrentPage.NavigateToAppealCategoryManager(), false, true);
            }
        }

        [Test, Category("SmokeTestDeployment"), Category("Navigation1"), Category("AppealDependent")]
        public void Verify_navigation_to_Appeal_Summary_page()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                NavigateToAppealSummaryPage(automatedBase.QuickLaunch);
                automatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.AppealSummary.GetStringValue());
                Verify_Quick_Launch_Switch_Client_Help_Icons(automatedBase.CurrentPage,() => NavigateToAppealSummaryPage(automatedBase.QuickLaunch), false, true);
            }
        }


        [Test, Category("SmokeTestDeployment"), Category("Navigation1")]
        public void Verify_navigation_to_Batch_Search_page()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                automatedBase.QuickLaunch.NavigateToBatchSearch();
                automatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.BatchSearch.GetStringValue());
                Verify_Quick_Launch_Switch_Client_Help_Icons(automatedBase.CurrentPage,() => automatedBase.CurrentPage.NavigateToBatchSearch(),
                    false, true);
            }
        }

        [Test, Category("SmokeTestDeployment"), Category("Navigation1")]
        public void Verify_navigation_to_Batch_Summary_page()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                NavigateToBatchSummaryPage(automatedBase.QuickLaunch);
                automatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.BatchSummary.GetStringValue());
                Verify_Quick_Launch_Switch_Client_Help_Icons(automatedBase.CurrentPage,() => NavigateToBatchSummaryPage(automatedBase.QuickLaunch), false, true);
            }
        }

        [Test, Category("SmokeTestDeployment"), Category("Navigation1")]
        public void Verify_navigation_to_Claim_Search_page()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                automatedBase.QuickLaunch.NavigateToClaimSearch();
                automatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.ClaimSearch.GetStringValue());
                Verify_Quick_Launch_Switch_Client_Help_Icons(automatedBase.CurrentPage,() => automatedBase.CurrentPage.NavigateToClaimSearch(),
                    false, true, true);
            }
        }

        [Test, Category("SmokeTestDeployment"), Category("Navigation1")] //CAR-2902(CAR-2744)
        public void Verify_navigation_to_Dashboard()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                var dashboard = automatedBase.CurrentPage.NavigateToDashboard();
                dashboard.IsContainerHeaderClaimsOverviewPCIPresent().ShouldBeTrue("Is CV Dashboard Page Opened?");
                Verify_Quick_Launch_Switch_Client_Help_Icons(automatedBase.CurrentPage,() => automatedBase.CurrentPage.NavigateToDashboard(),
                    false, true);
            }
        }

        [Test, Category("SmokeTestDeployment"), Category("Navigation3")]
        public void Verify_navigation_to_Appeal_Details_via_Dashboard()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                NavigateToAppealsDetailPageViaDashboard(automatedBase.QuickLaunch);
                automatedBase.CurrentPage.GetPageHeader()
                    .ShouldBeEqual(PageHeaderEnum.DashboardAppealsDetail.GetStringValue());
                Verify_Quick_Launch_Switch_Client_Help_Icons(automatedBase.CurrentPage,() => NavigateToAppealsDetailPageViaDashboard(automatedBase.QuickLaunch), false,
                    true);
            }
        }

        [Test, Category("SmokeTestDeployment"), Category("Navigation3")]
        public void Verify_navigation_to_Claims_Details_via_Dashboard()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                NavigateToClaimsDetailPageViaDashboard(automatedBase.QuickLaunch);
                automatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.DashboardClaimsDetail.GetStringValue());
                Verify_Quick_Launch_Switch_Client_Help_Icons(automatedBase.CurrentPage,() => NavigateToClaimsDetailPageViaDashboard(automatedBase.QuickLaunch), false,
                    true);
            }
        }

        [Test, Category("SmokeTestDeployment"), Category("Navigation2")]
        public void Verify_navigation_to_Logic_requests_Details_via_Dashboard()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                automatedBase.CurrentPage.RefreshPage(false, 0, true);
                NavigateToLogicRequestsDetailPageViaDashboard(automatedBase.QuickLaunch);
                automatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.DashboardLogicRequestsDetail.GetStringValue());
                Verify_Quick_Launch_Switch_Client_Help_Icons(automatedBase.CurrentPage,() => NavigateToLogicRequestsDetailPageViaDashboard(automatedBase.QuickLaunch),
                    false, true);
            }
        }

        [Test, Category("SmokeTestDeployment"), Category("Navigation2")]
        public void Verify_navigation_to_FFP_Dashboard()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                var dashboard = automatedBase.CurrentPage.NavigateToFFPDashboard();
                dashboard.IsContainerHeaderClaimsOverviewFFPPresent().ShouldBeTrue("Is FFP Dashboard Page Opened?");
                Verify_Quick_Launch_Switch_Client_Help_Icons(automatedBase.CurrentPage,() => automatedBase.CurrentPage.NavigateToFFPDashboard(), false, true);
            }
        }

        [Test, Category("SmokeTestDeployment"), Category("Navigation2")]
        public void Verify_navigation_to_FFP_Claims_Detail_Dashboard()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                NavigateToFFPClaimsDetailDashboard(automatedBase.QuickLaunch);
                automatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.DashboardFFPClaimsDetail.GetStringValue());
                Verify_Quick_Launch_Switch_Client_Help_Icons(automatedBase.CurrentPage,() => automatedBase.CurrentPage.NavigateToFFPDashboard(), false, true);
            }
        }


        [Test, Category("SmokeTestDeployment"), Category("Navigation2")]
        public void Verify_navigation_to_My_Dashboard()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                var dashboard = automatedBase.CurrentPage.NavigateToMyDashboard();
                dashboard.IsMydashboardShownInRightCornerForWidget().ShouldBeTrue("Is My Dashboard Page Opened?");
                Verify_Quick_Launch_Switch_Client_Help_Icons(automatedBase.CurrentPage,() => automatedBase.CurrentPage.NavigateToMyDashboard(), false, true);
            }
        }


        [Test, Category("SmokeTestDeployment"), Category("Navigation2")]
        public void Verify_navigation_to_Logic_Search_page()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                automatedBase.QuickLaunch.NavigateToLogicSearch();
                automatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.LogicSearch.GetStringValue());
                Verify_Quick_Launch_Switch_Client_Help_Icons(automatedBase.CurrentPage,() => automatedBase.CurrentPage.NavigateToLogicSearch(), false, true);
            }
        }

        //[Test, Category("SmokeTestDeployment"), Category("Navigation1")]
        //public void Verify_navigation_to_PCI_Claims_page()
        //{
        //    QuickLaunch.NavigateToPciClaimsWorkList(true);
        //    CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.ClaimAction.GetStringValue());
        //    Verify_Quick_Launch_Switch_Client_Help_Icons(false, true, false, () => CurrentPage.NavigateToPciClaimsWorkList(true),false,true);
        //}

        //[Test, Category("SmokeTestDeployment"), Category("Navigation2")]
        //public void Verify_navigation_to_PCI_RN_Claims_page()
        //{
        //    QuickLaunch.NavigateToPciRnWorkList(true);
        //    CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.ClaimAction.GetStringValue());
        //    Verify_Quick_Launch_Switch_Client_Help_Icons(false, true, false, () => CurrentPage.NavigateToPciRnWorkList(true),false,true);
        //}

        //[Test, Category("SmokeTestDeployment"), Category("Navigation2")]
        //public void Verify_navigation_to_PCI_Coder_Claims_page()
        //{
        //    QuickLaunch.NavigateToPciCodersClaim(true);
        //    CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.ClaimAction.GetStringValue());
        //    Verify_Quick_Launch_Switch_Client_Help_Icons(false, true, false, () => CurrentPage.NavigateToPciCodersClaim(true),false,true);
        //}

        [Test, Category("SmokeTestDeployment"), Category("Navigation2")]
        public void Verify_navigation_to_PCI_QA_Claims_page()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                automatedBase.QuickLaunch.NavigateToCvQcWorkList(true);
                automatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.ClaimAction.GetStringValue());
                Verify_Quick_Launch_Switch_Client_Help_Icons(automatedBase.CurrentPage,() => automatedBase.CurrentPage.NavigateToCvQcWorkList(true), false,
                    true);
            }
        }

        //[Test, Category("SmokeTestDeployment"), Category("Navigation3")]
        //public void Verify_navigation_to_FCI_Claims_page()
        //{
        //    QuickLaunch.NavigateToFciClaimsWorkList(true);
        //    CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.ClaimAction.GetStringValue());
        //    Verify_Quick_Launch_Switch_Client_Help_Icons(false, true, false, () => CurrentPage.NavigateToFciClaimsWorkList(true), false, true);
        //}

        //[Test, Category("SmokeTestDeployment"), Category("Navigation2")]
        //public void Verify_navigation_to_FFP_Claims_page()
        //{
        //    QuickLaunch.NavigateToFfpClaimsWorkList(true);
        //    CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.ClaimAction.GetStringValue());
        //    Verify_Quick_Launch_Switch_Client_Help_Icons(false, true, false, () => CurrentPage.NavigateToFfpClaimsWorkList(true),false,true);
        //}

        //[Test, Category("SmokeTestDeployment")]
        //public void Verify_navigation_to_DCI_Claims_page()
        //{
        //    QuickLaunch.NavigateToDciClaimsWorkList(true);
        //    CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.ClaimAction.GetStringValue());
        //    Verify_Quick_Launch_Switch_Client_Help_Icons(false, true, false, () => CurrentPage.NavigateToDciClaimsWorkList(true),false,true);
        //}


        [Test, Category("SmokeTestDeployment"), Category("Navigation2")]
        public void Verify_navigation_to_Pre_Authorization_Search_page()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                automatedBase.QuickLaunch.NavigateToPreAuthSearch();
                automatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.PreAuthSearch.GetStringValue());
                Verify_Quick_Launch_Switch_Client_Help_Icons(automatedBase.CurrentPage,() => automatedBase.CurrentPage.NavigateToPreAuthSearch(), false, true);
            }
        }

        [Test, Category("SmokeTestDeployment"), Category("Navigation2")]
        public void Verify_navigation_to_Pre_Authorization_Action_page()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                NaviateToPreAuthActionPage(automatedBase.QuickLaunch,1, 2);
                automatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.PreAuthAction.GetStringValue());
                Verify_Quick_Launch_Switch_Client_Help_Icons(automatedBase.CurrentPage,() => NaviateToPreAuthActionPage(automatedBase.QuickLaunch, 1, 2), false, true);
            }
        }

        [Test, Category("SmokeTestDeployment"), Category("Navigation2")]
        public void Verify_navigation_to_New_Provider_Search_page()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                automatedBase.QuickLaunch.NavigateToProviderSearch();
                automatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.ProviderSearch.GetStringValue());
                Verify_Quick_Launch_Switch_Client_Help_Icons(automatedBase.CurrentPage,() => automatedBase.CurrentPage.NavigateToProviderSearch(), false, true);
            }
        }

        [Test, Category("SmokeTestDeployment")]
        public void Verify_navigation_to_Suspect_Providers_page()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                automatedBase.QuickLaunch.NavigateToSuspectProviders();
                automatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.SuspectProviders.GetStringValue());
                Verify_Quick_Launch_Switch_Client_Help_Icons(automatedBase.CurrentPage,() => automatedBase.CurrentPage.NavigateToSuspectProviders(), false,
                    true);
            }
        }

        [Test, Category("SmokeTestDeployment")]
        public void Verify_navigation_to_Suspect_Providers_WorkList_page()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                automatedBase.QuickLaunch.NavigateToSuspectProvidersWorkList();
                automatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.ProviderAction.GetStringValue());
                Verify_Quick_Launch_Switch_Client_Help_Icons(automatedBase.CurrentPage,() => automatedBase.CurrentPage.NavigateToSuspectProvidersWorkList(),
                    false, true);
            }
        }


        [Test, Category("SmokeTestDeployment")]
        public void Verify_navigation_to_Repository()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                automatedBase.QuickLaunch.NavigateToRepository();
                automatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.Repository.GetStringValue());
                Verify_Quick_Launch_Switch_Client_Help_Icons(automatedBase.CurrentPage,() => automatedBase.CurrentPage.NavigateToRepository(), false, true);
            }
        }

        [Test, Category("SmokeTestDeployment")]
        public void Verify_navigation_to_Reference_Manager()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                automatedBase.QuickLaunch.NavigateToReferenceManager();
                automatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.ReferenceManager.GetStringValue());
                Verify_Quick_Launch_Switch_Client_Help_Icons(automatedBase.CurrentPage,() => automatedBase.CurrentPage.NavigateToReferenceManager(), false,
                    true);
            }
        }

        [Test, Category("SmokeTestDeployment")]
        public void Verify_navigation_to_Invoice_Search()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                automatedBase.QuickLaunch.NavigateToInvoiceSearch();
                automatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.InvoiceSearch.GetStringValue());
                Verify_Quick_Launch_Switch_Client_Help_Icons(automatedBase.CurrentPage,() => automatedBase.CurrentPage.NavigateToInvoiceSearch(), false, true);
            }
        }

        [Test, Category("SmokeTestDeployment")]
        public void Verify_navigation_to_Analyst_Manager()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                automatedBase.QuickLaunch.NavigateToAnalystManager();
                automatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.QAManager.GetStringValue());
                Verify_Quick_Launch_Switch_Client_Help_Icons(automatedBase.CurrentPage,() => automatedBase.CurrentPage.NavigateToAnalystManager(), false, true);
            }
        }

        [Test, Category("SmokeTestDeployment")]
        public void Verify_navigation_to_QA_Claim_Search()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                automatedBase.QuickLaunch.NavigateToQaClaimSearch();
                automatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.QaClaimSearch.GetStringValue());
                Verify_Quick_Launch_Switch_Client_Help_Icons(automatedBase.CurrentPage,() => automatedBase.CurrentPage.NavigateToQaClaimSearch(), false, true);
            }
        }

        [Test, Category("SmokeTestDeployment")]
        public void Verify_navigation_to_QA_Appeal_Search()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                automatedBase.QuickLaunch.NavigateToQaAppealSearch();
                automatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.QaAppealSearch.GetStringValue());
                Verify_Quick_Launch_Switch_Client_Help_Icons(automatedBase.CurrentPage,() => automatedBase.CurrentPage.NavigateToQaAppealSearch(), false, true);
            }
        }

        [Test, Category("SmokeTestDeployment")] // TANT 195
        public void Verify_navigation_to_Client_Search_And_Edit_Setting_Manager()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ClientSearchPage _clientSearch;
                _clientSearch = automatedBase.QuickLaunch.NavigateToClientSearch();
                automatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.ClientSearch.GetStringValue());
                _clientSearch.SearchByClientCodeToNavigateToClientProfileViewPage(ClientEnum.SMTST.ToString());
                var tabList = _clientSearch.GetClientSettingsTabList().Take(6);
                foreach (var tab in tabList)
                {
                    _clientSearch.ClickOnClientSettingTabByTabName(tab);

                    _clientSearch.IsClientSettingsSectionDisabled().ShouldBeTrue($"Is {tab} setting section disabled?");
                    _clientSearch.GetSideWindow.ClickOnEditIcon();
                    _clientSearch.IsClientSettingsSectionDisabled()
                        .ShouldBeFalse($"Is {tab} setting section disabled?");
                    _clientSearch.GetSideWindow.Cancel();
                    if (tab == ClientSettingsTabEnum.Custom.GetStringValue())
                        _clientSearch.WaitForWorking();
                    _clientSearch.IsClientSettingsSectionDisabled().ShouldBeTrue($"Is {tab} setting section disabled?");

                }

                StringFormatter.PrintMessage(
                    "Verify navigation to Edit settings Manager Page from New client search page");
                _clientSearch.ClickOnEditSettingsIcon();
                automatedBase.CurrentPage.WaitForPageToLoadWithSideBarPanel();
                automatedBase.CurrentPage.GetPageHeader()
                    .ShouldBeEqual(PageHeaderEnum.EditSettingsManager.GetStringValue());
                Verify_Quick_Launch_Switch_Client_Help_Icons(automatedBase.CurrentPage,() =>
                    automatedBase.CurrentPage.NavigateToEditSettingsManager());

                automatedBase.CurrentPage.ClickOnReturnToClientSearch();
                automatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.ClientSearch.GetStringValue());
                Verify_Quick_Launch_Switch_Client_Help_Icons(automatedBase.CurrentPage,() => automatedBase.CurrentPage.NavigateToClientSearch());
                automatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.ClientSearch.GetStringValue());

            }
        }



        [Test, Category("SmokeTestDeployment")] //CAR-2902(CAR-2744)
        public void Verify_navigation_to_My_Profile()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                automatedBase.QuickLaunch.NavigateToMyProfilePage();
                automatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.MyProfile.GetStringValue());
                Verify_Quick_Launch_Switch_Client_Help_Icons(automatedBase.CurrentPage,() => automatedBase.CurrentPage.NavigateToMyProfilePage());
            }
        }

        [Test, Category("SmokeTestDeployment")]
        public void Verify_navigation_to_User_Profile_Search()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                automatedBase.QuickLaunch.NavigateToNewUserProfileSearch();
                automatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.UserProfileSearch.GetStringValue());
                Verify_Quick_Launch_Switch_Client_Help_Icons(automatedBase.CurrentPage,() => automatedBase.CurrentPage.NavigateToNewUserProfileSearch());
            }
        }

        [Test, Category("SmokeTestDeployment"), Category("Navigation3")]
        public void Verify_navigation_to_Maintenance_Banners()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                automatedBase.QuickLaunch.NavigateToMaintenanceNotices();
                automatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.MaintenanceBanners.GetStringValue());
                Verify_Quick_Launch_Switch_Client_Help_Icons(automatedBase.CurrentPage,() => automatedBase.CurrentPage.NavigateToMaintenanceNotices(), false,
                    true);
            }
        }

        [Test, Category("SmokeTestDeployment")]
        public void Verify_navigation_to_Microstrategy()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                automatedBase.QuickLaunch.NavigateToMicrostrategyMaintenance();
                automatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.MicrostrategyMaintenance.GetStringValue());
                Verify_Quick_Launch_Switch_Client_Help_Icons(automatedBase.CurrentPage,() => automatedBase.CurrentPage.NavigateToMicrostrategyMaintenance(),
                    isCorePage: true);
            }
        }

        [Test, Category("SmokeTestDeployment")] //CAR-2214(CAR-2730)+ TANT-248
        public void Verify_navigation_to_Role_Manager()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                RoleManagerPage _roleManagerPage;
                _roleManagerPage = automatedBase.QuickLaunch.NavigateToRoleManager();
                _roleManagerPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.RoleManager.GetStringValue(),
                    "User should be able to navigate to the 'Role Manager' page");
                _roleManagerPage.SideBarPanelSearch.GetTopHeaderName()
                    .ShouldBeEqual("Filter Roles", "Side bar panel should be open"); //Find Role Managers
                _roleManagerPage.SideBarPanelSearch.ClickOnToggleSidebarPanelButton();
                _roleManagerPage.SideBarPanelSearch.IsSideBarPanelOpen().ShouldBeFalse("Is Side bar panel open?");
                Verify_Quick_Launch_Switch_Client_Help_Icons(automatedBase.CurrentPage,() => automatedBase.CurrentPage.NavigateToRoleManager(),
                    isCorePage: true);
            }
        }

        [Test, Category("SmokeTestDeployment")] //TANT-243
        public void Verify_navigation_to_Client_Search_And_Edit_Setting_Manager_for_Inactive_Clients()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                ClientSearchPage _clientSearch;
                _clientSearch = automatedBase.QuickLaunch.NavigateToClientSearch();
                _clientSearch.SideBarPanelSearch.SetInputFieldByLabel("Status", "Inactive", sendTabKey: true);
                _clientSearch.SideBarPanelSearch.ClickOnFindButton();
                _clientSearch.WaitForWorking();
                if (_clientSearch.GetGridViewSection.GetGridRowCount() > 0)
                {
                    _clientSearch.GetGridViewSection.ClickOnGridRowByRow();
                    var tabList = _clientSearch.GetClientSettingsTabList().Take(5);
                    foreach (var tab in tabList)
                    {
                        _clientSearch.ClickOnClientSettingTabByTabName(tab);

                        _clientSearch.IsClientSettingsSectionDisabled()
                            .ShouldBeTrue($"Is {tab} setting section disabled?");
                        _clientSearch.GetSideWindow.ClickOnEditIcon();
                        _clientSearch.IsClientSettingsSectionDisabled()
                            .ShouldBeFalse($"Is {tab} setting section disabled?");
                        _clientSearch.GetSideWindow.Cancel();
                        if (tab == ClientSettingsTabEnum.Custom.GetStringValue())
                            _clientSearch.WaitForWorking();
                        _clientSearch.IsClientSettingsSectionDisabled()
                            .ShouldBeTrue($"Is {tab} setting section disabled?");

                    }

                    StringFormatter.PrintMessage(
                        "Verify navigation to Edit settings Manager Page from New client search page");
                    _clientSearch.ClickOnEditSettingsIcon();
                    automatedBase.CurrentPage.WaitForPageToLoadWithSideBarPanel();
                    automatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.EditSettingsManager.GetStringValue());
                    Verify_Quick_Launch_Switch_Client_Help_Icons(automatedBase.CurrentPage,() =>
                        automatedBase.CurrentPage.NavigateToEditSettingsManager(isInactive: true));
                }
            }
        }

        [Test, Category("SmokeTestDeployment")] //TANT-250
        public void Verify_navigation_to_user_settings_form()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                UserProfileSearchPage _userProfileSearch;
                _userProfileSearch =
                    automatedBase.CurrentPage.SearchByUserIdToNavigateToUserSettingsForm(automatedBase.EnvironmentManager.HciAdminUsername);
                _userProfileSearch.GetLeftFormHeader().ShouldBeEqual(PageHeaderEnum.UserSettings.GetStringValue(),
                    "User settings form should be present");
                var tablist = Enum.GetValues(typeof(UserSettingsTabEnum)).Cast<UserSettingsTabEnum>()
                    .Select(x => x.GetStringValue()).ToList();
                _userProfileSearch.GetUserSettingsTabList().ShouldCollectionBeEqual(tablist, "Tab list should match");
                foreach (var tab in tablist)
                {
                    _userProfileSearch.ClickOnUserSettingTabByTabName(tab);
                    _userProfileSearch.IsUserSettingsFormDisabled().ShouldBeTrue($"Is {tab} form disabled?");
                    _userProfileSearch.ClickEditIconSettings(tab);
                    _userProfileSearch.GetSideWindow.IsSaveButtonPresent().ShouldBeTrue("Is Save button present?");
                    _userProfileSearch.IsCancelButtonPresent().ShouldBeTrue("Is Cancel button present?");
                    if (tab.Equals(tablist[0]))
                    {
                        foreach (var tabs in tablist)
                        {
                            _userProfileSearch.ClickOnUserSettingTabByTabName(tabs);
                            _userProfileSearch.IsUserSettingsFormDisabled().ShouldBeFalse($"Is {tab} form disabled?");
                        }

                        _userProfileSearch.ClickOnCancelLink();
                        foreach (var label in tablist)
                        {
                            _userProfileSearch.ClickOnUserSettingTabByTabName(label);
                            _userProfileSearch.IsUserSettingsFormDisabled().ShouldBeTrue($"Is {tab} form disabled?");
                        }
                    }
                    else
                    {
                        _userProfileSearch.IsUserSettingsFormDisabled().ShouldBeFalse($"Is {tab} form disabled?");
                        _userProfileSearch.ClickOnCancelLink();
                        _userProfileSearch.IsUserSettingsFormDisabled().ShouldBeTrue($"Is {tab} form disabled?");
                    }

                }


            }
        }

        [Test, Category("SmokeTestDeployment")] //TANT-244
        public void Verify_Navigation_To_Create_New_User_Account_form()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                UserProfileSearchPage _userProfileSearch;
                var tablist = Enum.GetValues(typeof(NewUserAccountTabEnum)).Cast<NewUserAccountTabEnum>()
                    .Select(x => x.GetStringValue()).ToList();
                _userProfileSearch = automatedBase.CurrentPage.NavigateToCreateNewUserAccount();
                _userProfileSearch.GetUserSettingsTabList()
                    .ShouldCollectionBeEqual(tablist, $"{tablist} tabs should match");
                _userProfileSearch.GetLeftFormHeader().ShouldBeEqual(PageHeaderEnum.CreateUser.GetStringValue(),
                    "New User Account form should be present");
                _userProfileSearch.IsCreateNewUserTabSelectedByTabName(tablist[0])
                    .ShouldBeTrue($"{tablist[0]} tab should be selected");
                _userProfileSearch.IsNewUserAccountNextButtonPresent().ShouldBeTrue("Is Next Button present?");
                _userProfileSearch.IsCancelButtonPresent().ShouldBeTrue("Is Cancel Button present?");
            }
        }

        [Test, Category("SmokeTestDeployment"), Category("AppealDependent")] //TANT-245
        public void Verify_Move_Appeals_to_QA_in_Appeal_Manager_page()
        {
            using (var automatedBase = new NewAutomatedBaseParallelRun())
            {
                AppealManagerPage _appealManager;
                _appealManager = automatedBase.CurrentPage.NavigateToAppealManager();
                _appealManager.IsMoveAppealsToQaPresent().ShouldBeTrue("Is Move Appeals to QA button present?");
                _appealManager.IsModifyAppealsIconDisabled().ShouldBeTrue("Is Move Appeals to QA button disabled?");
                _appealManager.GetSideBarPanelSearch.ClickOnDropDownInputList("Quick Search");
                var filterList = _appealManager.GetSideBarPanelSearch.GetAvailableDropDownList("Quick Search");
                foreach (var filter in filterList)
                {
                    _appealManager.GetSideBarPanelSearch.SetInputFieldByLabel("Quick Search", filter);
                    _appealManager.GetSideBarPanelSearch.ClickOnFindButton();
                    _appealManager.WaitForPageToLoad();
                    if (_appealManager.GetGridViewSection.GetGridRowCount() > 0)
                        _appealManager.IsModifyAppealsIconDisabled()
                            .ShouldBeFalse("Is Move Appeals to QA button disabled?");
                }
            }
        }

        #endregion

        #region PRIVATE METHODS

        private void Verify_Quick_Launch_Switch_Client_Help_Icons(NewDefaultPage CurrentPage, Action navigateToPage, bool popup = false, bool isCorePage=false,bool needToSwitch=false)
        {
                CurrentPage.IsSwitchClientIconPresent().ShouldBeTrue("Switch client icon should be present");

                if (needToSwitch)
                {
                    CurrentPage.GetCurrentClient().ShouldBeEqual(ClientEnum.SMTST.GetStringValue(), "Default client logo is not present");
                    if (isCorePage)
                        if (popup)
                            CurrentPage.ClickOnSwitchClient().SwitchClientTo(ClientEnum.TTREE,  true);
                        else
                            CurrentPage.ClickOnSwitchClient().SwitchClientTo(ClientEnum.TTREE);
                    else
                        CurrentPage.ClickOnSwitchClient().SwitchClientTo(ClientEnum.TTREE);
                    CurrentPage.GetCurrentClient().ShouldBeEqual(ClientEnum.TTREE.GetStringValue(), "Client should be changed", true);
                    CurrentPage.ClickOnSwitchClient().SwitchClientTo(ClientEnum.SMTST);
                    CurrentPage.GetCurrentClient().ShouldBeEqual(ClientEnum.SMTST.GetStringValue(), "Default client logo is not present");
                navigateToPage();
                }
                CurrentPage.IsHelpCenterIconPresent().ShouldBeTrue("Help center icon should be present");
                if (popup)
                    CurrentPage.NavigateToHelpCenter(true);
                else
                    CurrentPage.NavigateToHelpCenter();

                CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.HelpCenter.GetStringValue(), "Help center page should be presented");
                navigateToPage();
        }

        public PreAuthActionPage NaviateToPreAuthActionPage(QuickLaunchPage QuickLaunch, int row, int col)
        {
            PreAuthSearchPage _preAuthSearchPage = QuickLaunch.NavigateToPreAuthSearch();
            return _preAuthSearchPage.NaviateToPreAuthActionPage(row, col);
        }

        public AppealSummaryPage NavigateToAppealSummaryPage(QuickLaunchPage QuickLaunch)
        {
            AppealSearchPage _appealSearch = QuickLaunch.NavigateToAppealSearch();
            return _appealSearch.NavigateToAppealSummaryPageBySideBarPannel(true);
        }

        public AppealCreatorPage NavigateToAppealCreatorPage(QuickLaunchPage QuickLaunch)
        {
            AppealCreatorPage _appealCreator= QuickLaunch.NavigateToAppealCreator();
            _appealCreator.SearchByClaimNoAndStayOnSearch("1");
            return _appealCreator.ClickOnFirstEnableCreateAppealIcon();
        }

        public BatchSummaryPage NavigateToBatchSummaryPage(QuickLaunchPage QuickLaunch)
        {
           BatchSearchPage _batchSearch = QuickLaunch.NavigateToBatchSearch();
            return _batchSearch.ClickOnBatchByRowCol(1, 2);
        }
        
        public AppealsDetailPage NavigateToAppealsDetailPageViaDashboard(QuickLaunchPage QuickLaunch)
        {
           DashboardPage _dashboard = QuickLaunch.NavigateToDashboard();
            return _dashboard.ClickOnAppealsDetailExpandIcon();
        }

        public ClaimsDetailPage NavigateToClaimsDetailPageViaDashboard(QuickLaunchPage QuickLaunch)
        {
            DashboardPage _dashboard = QuickLaunch.NavigateToDashboard();
            return _dashboard.ClickOnClaimsDetailExpandIcon();
        }


        public DashboardLogicRequestsDetailsPage NavigateToLogicRequestsDetailPageViaDashboard(QuickLaunchPage QuickLaunch)
        {
            DashboardPage _dashboard = QuickLaunch.NavigateToDashboard();
            return _dashboard.ClickOnLogicRequestsDetailClientExpandIcon();
        }

        public ClaimsDetailPage NavigateToFFPClaimsDetailDashboard(QuickLaunchPage QuickLaunch)
        {
            DashboardPage _dashboard = QuickLaunch.NavigateToFFPDashboard();
            return _dashboard.ClickOnFFPClaimsDetailExpandIcon();
        }

        
    
        #endregion

    }
}

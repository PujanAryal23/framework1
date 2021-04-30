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
    public class ApplicationNavigationSmokeTestParallels
    {
        #region PRIVATE FIELDS

     
       

        #endregion

        #region PROTECTED FIELDS

        #endregion

        #region OVERRIDE
      
        #endregion

        #region TEST SUITES
        //TANT-73
        //[Test, Category("SmokeTestDeployment"), Category("Navigation1")]
        //public void Verify_Quick_launch_icon_Switch_client_icon_and_Help_presence_in_Quick_launch_page()
        //{
        //    newAutomatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.newAutomatedBase.QuickLaunch.GetStringValue());
        //    Verify_Quick_Launch_Switch_Client_Help_Icons(newAutomatedBase.CurrentPage,() => newAutomatedBase.CurrentPage.ClickOnnewAutomatedBase.QuickLaunch());
        //}

        [Test, Category("SmokeTestDeployment"), Category("Navigation3")] //TANT-260
        public void Navigate_to_COB_DashBoardPage()
        {
            using (var newAutomatedBase = new NewAutomatedBaseParallelRun())
            {
                var _dashboard = newAutomatedBase.QuickLaunch.NavigateToCOBDashboard();

                _dashboard.GetPageHeader().ShouldBeEqual(PageTitleEnum.Dashboard.GetStringValue());
                _dashboard.IsCobPresentInContainerHeaderWidgetOverviewByComponentTitle(DashboardOverviewTitlesEnum.ClaimsOverview.GetStringValue())
                    .ShouldBeTrue("Is COB present in Claims Overview Widget?");
                _dashboard.IsCobPresentInContainerHeaderWidgetOverviewByComponentTitle(DashboardOverviewTitlesEnum.AppealsOverview.GetStringValue())
                    .ShouldBeTrue("Is COB present in Appeals Overview Widget?");
                Verify_Quick_Launch_Switch_Client_Help_Icons(newAutomatedBase.CurrentPage,
                    () => _dashboard.NavigateToCOBDashboard(), false, true, false);


            }
           
        }

        

        [Test, Category("SmokeTestDeployment"),Category("Navigation3")] //TANT-262
        public void Navigate_to_COB_Claims_Detail_Page()
        {
            using (var newAutomatedBase = new NewAutomatedBaseParallelRun())
            {
                var _claimsDetail= newAutomatedBase.QuickLaunch.NavigateToCobClaimsDetailPage();


                _claimsDetail.GetPageHeader().ShouldBeEqual(PageTitleEnum.DashboardClaimsDetail.GetStringValue());
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
                Verify_Quick_Launch_Switch_Client_Help_Icons(newAutomatedBase.CurrentPage,
                    () => newAutomatedBase.CurrentPage.NavigateToCobClaimsDetailPage(), false,
                    true, false);
            }
        }

        [Test, Category("SmokeTestDeployment"), Category("Navigation3")] //TANT-261
        public void Navigate_to_COB_Appeals_Detail_Page()
        {
            using (var newAutomatedBase = new NewAutomatedBaseParallelRun())
            {
                newAutomatedBase.QuickLaunch.NavigateToCobAppealsDetailPage();
                newAutomatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageTitleEnum.COBAppealsDetail.GetStringValue());
                Verify_Quick_Launch_Switch_Client_Help_Icons(newAutomatedBase.CurrentPage,() => newAutomatedBase.CurrentPage.NavigateToCobAppealsDetailPage(), false, true, false);
                newAutomatedBase.CurrentPage.NavigateToCVDashboard();
            }
        }

        [Test, Category("SmokeTestDeployment"), Category("Navigation1")] //CAR-2902(CAR-2744)
        public void Navigate_to_help_center_page_through_help_center_button()
        {
            using (var newAutomatedBase = new NewAutomatedBaseParallelRun())
            {
                newAutomatedBase.QuickLaunch.NavigateToHelpCenter();
                newAutomatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.HelpCenter.GetStringValue());
            }
        }
        //TANT-113 //TANT-73

        [Test, Category("SmokeTestDeployment"), Category("Navigation1")]
        public void Verify_navigation_to_Appeal_search_page()
        {
            using (var newAutomatedBase = new NewAutomatedBaseParallelRun())
            {
                newAutomatedBase.QuickLaunch.NavigateToAppealSearch();
                newAutomatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.AppealSearch.GetStringValue());
                Verify_Quick_Launch_Switch_Client_Help_Icons(newAutomatedBase.CurrentPage,() => newAutomatedBase.CurrentPage.NavigateToAppealSearch(), false, true, true);
            }
        }

        [Test, Category("SmokeTestDeployment"), Category("Navigation1")]
        public void Verify_navigation_to_Appeal_Creator_page()
        {
            using (var newAutomatedBase = new NewAutomatedBaseParallelRun())
            {
                NavigateToAppealCreatorPage(newAutomatedBase.CurrentPage);
                newAutomatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.AppealCreator.GetStringValue());
                Verify_Quick_Launch_Switch_Client_Help_Icons(newAutomatedBase.CurrentPage,() => NavigateToAppealCreatorPage(newAutomatedBase.CurrentPage), false, true);
            }
        }

        [Test, Category("SmokeTestDeployment"), Category("Navigation3"), Category("AppealDependent")] //+TANT-95
        public void Verify_navigation_to_a_My_Appeals_page()
        {
            using (var newAutomatedBase = new NewAutomatedBaseParallelRun())
            {
                var _appealAction = newAutomatedBase.QuickLaunch.NavigateToMyAppeals(false);
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

                    newAutomatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.AppealAction.GetStringValue());
                    Verify_Quick_Launch_Switch_Client_Help_Icons(newAutomatedBase.CurrentPage,() => newAutomatedBase.CurrentPage.NavigateToMyAppeals(), true, true);
                }
                finally
                {
                    newAutomatedBase.CurrentPage.CloseAnyTabIfExist();
                   _appealAction.ClickOnSearchIconToNavigateAppealSearchPage();
                }

            }
        }

        [Test, Category("SmokeTestDeployment"), Category("Navigation1")]
        public void Verify_navigation_to_Appeals_Manager_page()
        {
            using (var newAutomatedBase = new NewAutomatedBaseParallelRun())
            {
                newAutomatedBase.QuickLaunch.NavigateToAppealManager();
                newAutomatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.AppealManager.GetStringValue());
                Verify_Quick_Launch_Switch_Client_Help_Icons(newAutomatedBase.CurrentPage,() => newAutomatedBase.CurrentPage.NavigateToAppealManager(), false, true);
            }
        }

        [Test, Category("SmokeTestDeployment"), Category("Navigation3")]
        public void Verify_navigation_to_Appeal_Rationale_Manager_page()
        {
            using (var newAutomatedBase = new NewAutomatedBaseParallelRun())
            {
                newAutomatedBase.QuickLaunch.NavigateToAppealRationaleManager();
                newAutomatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.AppealRationaleManager.GetStringValue());
                Verify_Quick_Launch_Switch_Client_Help_Icons(newAutomatedBase.CurrentPage,() => newAutomatedBase.CurrentPage.NavigateToAppealRationaleManager(), false, true);
            }
        }

        [Test, Category("SmokeTestDeployment"), Category("Navigation1")]
        public void Verify_navigation_to_Appeal_Category_Manager_page()
        {
            using (var newAutomatedBase = new NewAutomatedBaseParallelRun())
            {
                newAutomatedBase.QuickLaunch.NavigateToAppealCategoryManager();
                newAutomatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.AppealCategoryManager.GetStringValue());
                Verify_Quick_Launch_Switch_Client_Help_Icons(newAutomatedBase.CurrentPage,() => newAutomatedBase.CurrentPage.NavigateToAppealCategoryManager(), false, true);
            }
        }

        [Test, Category("SmokeTestDeployment"), Category("Navigation1"), Category("AppealDependent")]
        public void Verify_navigation_to_Appeal_Summary_page()
        {
            using (var newAutomatedBase = new NewAutomatedBaseParallelRun())
            {
                NavigateToAppealSummaryPage(newAutomatedBase.CurrentPage);
                newAutomatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.AppealSummary.GetStringValue());
                Verify_Quick_Launch_Switch_Client_Help_Icons(newAutomatedBase.CurrentPage,() => NavigateToAppealSummaryPage(newAutomatedBase.CurrentPage), false, true);
            }
        }


        [Test, Category("SmokeTestDeployment"), Category("Navigation1")]
        public void Verify_navigation_to_Batch_Search_page()
        {
            using (var newAutomatedBase = new NewAutomatedBaseParallelRun())
            {
                newAutomatedBase.QuickLaunch.NavigateToBatchSearch();
                newAutomatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.BatchSearch.GetStringValue());
                Verify_Quick_Launch_Switch_Client_Help_Icons(newAutomatedBase.CurrentPage,() => newAutomatedBase.CurrentPage.NavigateToBatchSearch(), false, true);
            }
        }

        [Test, Category("SmokeTestDeployment"), Category("Navigation1")]
        public void Verify_navigation_to_Batch_Summary_page()
        {
            using (var newAutomatedBase = new NewAutomatedBaseParallelRun())
            {
                NavigateToBatchSummaryPage(newAutomatedBase.CurrentPage);
                newAutomatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.BatchSummary.GetStringValue());
                Verify_Quick_Launch_Switch_Client_Help_Icons(newAutomatedBase.CurrentPage,() => NavigateToBatchSummaryPage(newAutomatedBase.CurrentPage), false, true);
            }
        }

        [Test, Category("SmokeTestDeployment"), Category("Navigation1")]
        public void Verify_navigation_to_Claim_Search_page()
        {
            using (var newAutomatedBase = new NewAutomatedBaseParallelRun())
            {
                newAutomatedBase.QuickLaunch.NavigateToClaimSearch();
                newAutomatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.ClaimSearch.GetStringValue());
                Verify_Quick_Launch_Switch_Client_Help_Icons(newAutomatedBase.CurrentPage,() => newAutomatedBase.CurrentPage.NavigateToClaimSearch(), false, true, true);
            }
        }

        [Test, Category("SmokeTestDeployment"), Category("Navigation1")] //CAR-2902(CAR-2744)
        public void Verify_navigation_to_Dashboard()
        {
            using (var newAutomatedBase = new NewAutomatedBaseParallelRun())
            {
                var dashboard = newAutomatedBase.CurrentPage.NavigateToDashboard();
                dashboard.IsContainerHeaderClaimsOverviewPCIPresent().ShouldBeTrue("Is CV Dashboard Page Opened?");
                Verify_Quick_Launch_Switch_Client_Help_Icons(newAutomatedBase.CurrentPage,() => newAutomatedBase.CurrentPage.NavigateToDashboard(), false, true);
            }
        }

        [Test, Category("SmokeTestDeployment"), Category("Navigation3")]
        public void Verify_navigation_to_Appeal_Details_via_Dashboard()
        {
            using (var newAutomatedBase = new NewAutomatedBaseParallelRun())
            {
                NavigateToAppealsDetailPageViaDashboard(newAutomatedBase.CurrentPage);
                newAutomatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.DashboardAppealsDetail.GetStringValue());
                Verify_Quick_Launch_Switch_Client_Help_Icons(newAutomatedBase.CurrentPage,() => NavigateToAppealsDetailPageViaDashboard(newAutomatedBase.CurrentPage), false, true);
            }
        }

        [Test, Category("SmokeTestDeployment"), Category("Navigation3")]
        public void Verify_navigation_to_Claims_Details_via_Dashboard()
        {
            using (var newAutomatedBase = new NewAutomatedBaseParallelRun())
            {
                NavigateToClaimsDetailPageViaDashboard(newAutomatedBase.CurrentPage);
                newAutomatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.DashboardClaimsDetail.GetStringValue());
                Verify_Quick_Launch_Switch_Client_Help_Icons(newAutomatedBase.CurrentPage,() => NavigateToClaimsDetailPageViaDashboard(newAutomatedBase.CurrentPage), false, true);
            }
        }

        [Test, Category("SmokeTestDeployment"), Category("Navigation2")]
        public void Verify_navigation_to_Logic_requests_Details_via_Dashboard()
        {
            using (var newAutomatedBase = new NewAutomatedBaseParallelRun())
            {
                newAutomatedBase.CurrentPage.RefreshPage(false, 0, true);
                NavigateToLogicRequestsDetailPageViaDashboard(newAutomatedBase.CurrentPage);
                newAutomatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.DashboardLogicRequestsDetail.GetStringValue());
                Verify_Quick_Launch_Switch_Client_Help_Icons(newAutomatedBase.CurrentPage,() => NavigateToLogicRequestsDetailPageViaDashboard(newAutomatedBase.CurrentPage), false, true);
            }
        }

        [Test, Category("SmokeTestDeployment"), Category("Navigation2")]
        public void Verify_navigation_to_FFP_Dashboard()
        {
            using (var newAutomatedBase = new NewAutomatedBaseParallelRun())
            {
                var dashboard = newAutomatedBase.CurrentPage.NavigateToFFPDashboard();
                dashboard.IsContainerHeaderClaimsOverviewFFPPresent().ShouldBeTrue("Is FFP Dashboard Page Opened?");
                Verify_Quick_Launch_Switch_Client_Help_Icons(newAutomatedBase.CurrentPage,() => newAutomatedBase.CurrentPage.NavigateToFFPDashboard(), false, true);
            }
        }

        [Test, Category("SmokeTestDeployment"), Category("Navigation2")]
        public void Verify_navigation_to_FFP_Claims_Detail_Dashboard()
        {
            using (var newAutomatedBase = new NewAutomatedBaseParallelRun())
            {
                NavigateToFFPClaimsDetailDashboard(newAutomatedBase.CurrentPage);
                newAutomatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.DashboardFFPClaimsDetail.GetStringValue());
                Verify_Quick_Launch_Switch_Client_Help_Icons(newAutomatedBase.CurrentPage,() => newAutomatedBase.CurrentPage.NavigateToFFPDashboard(), false, true);
            }
        }


        [Test, Category("SmokeTestDeployment"), Category("Navigation2")]
        public void Verify_navigation_to_My_Dashboard()
        {
            using (var newAutomatedBase = new NewAutomatedBaseParallelRun())
            {
                var dashboard = newAutomatedBase.CurrentPage.NavigateToMyDashboard();
                dashboard.IsMydashboardShownInRightCornerForWidget().ShouldBeTrue("Is My Dashboard Page Opened?");
                Verify_Quick_Launch_Switch_Client_Help_Icons(newAutomatedBase.CurrentPage,() => newAutomatedBase.CurrentPage.NavigateToMyDashboard(), false, true);
            }
        }


        [Test, Category("SmokeTestDeployment"), Category("Navigation2")]
        public void Verify_navigation_to_Logic_Search_page()
        {
            using (var newAutomatedBase = new NewAutomatedBaseParallelRun())
            {
                newAutomatedBase.QuickLaunch.NavigateToLogicSearch();
                newAutomatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.LogicSearch.GetStringValue());
                Verify_Quick_Launch_Switch_Client_Help_Icons(newAutomatedBase.CurrentPage,() => newAutomatedBase.CurrentPage.NavigateToLogicSearch(), false, true);
            }
        }


        [Test, Category("SmokeTestDeployment"), Category("Navigation2")]
        public void Verify_navigation_to_PCI_QA_Claims_page()
        {
            using (var newAutomatedBase = new NewAutomatedBaseParallelRun())
            {
                newAutomatedBase.QuickLaunch.NavigateToCvQcWorkList(true);
                newAutomatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.ClaimAction.GetStringValue());
                Verify_Quick_Launch_Switch_Client_Help_Icons(newAutomatedBase.CurrentPage,() => newAutomatedBase.CurrentPage.NavigateToCvQcWorkList(true), false, true);
            }
        }



        [Test, Category("SmokeTestDeployment"), Category("Navigation2")]
        public void Verify_navigation_to_Pre_Authorization_Search_page()
        {
            using (var newAutomatedBase = new NewAutomatedBaseParallelRun())
            {
                newAutomatedBase.QuickLaunch.NavigateToPreAuthSearch();
                newAutomatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.PreAuthSearch.GetStringValue());
                Verify_Quick_Launch_Switch_Client_Help_Icons(newAutomatedBase.CurrentPage,() => newAutomatedBase.CurrentPage.NavigateToPreAuthSearch(), false, true);
            }
        }

        [Test, Category("SmokeTestDeployment"), Category("Navigation2")]
        public void Verify_navigation_to_Pre_Authorization_Action_page()
        {
            using (var newAutomatedBase = new NewAutomatedBaseParallelRun())
            {
                NaviateToPreAuthActionPage(newAutomatedBase.CurrentPage,1, 2);
                newAutomatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.PreAuthAction.GetStringValue());
                Verify_Quick_Launch_Switch_Client_Help_Icons(newAutomatedBase.CurrentPage,() => NaviateToPreAuthActionPage(newAutomatedBase.CurrentPage,1, 2), false, true);
            }
        }

        [Test, Category("SmokeTestDeployment"), Category("Navigation2")]
        public void Verify_navigation_to_New_Provider_Search_page()
        {
            using (var newAutomatedBase = new NewAutomatedBaseParallelRun())
            {
                newAutomatedBase.QuickLaunch.NavigateToProviderSearch();
                newAutomatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.ProviderSearch.GetStringValue());
                Verify_Quick_Launch_Switch_Client_Help_Icons(newAutomatedBase.CurrentPage,() => newAutomatedBase.CurrentPage.NavigateToProviderSearch(), false, true);
            }
        }

        [Test, Category("SmokeTestDeployment")]
        public void Verify_navigation_to_Suspect_Providers_page()
        {
            using (var newAutomatedBase = new NewAutomatedBaseParallelRun())
            {
                newAutomatedBase.QuickLaunch.NavigateToSuspectProviders();
                newAutomatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.SuspectProviders.GetStringValue());
                Verify_Quick_Launch_Switch_Client_Help_Icons(newAutomatedBase.CurrentPage,() => newAutomatedBase.CurrentPage.NavigateToSuspectProviders(), false, true);
            }
        }

        [Test, Category("SmokeTestDeployment")]
        public void Verify_navigation_to_Suspect_Providers_WorkList_page()
        {
            using (var newAutomatedBase = new NewAutomatedBaseParallelRun())
            {
                newAutomatedBase.QuickLaunch.NavigateToSuspectProvidersWorkList();
                newAutomatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.ProviderAction.GetStringValue());
                Verify_Quick_Launch_Switch_Client_Help_Icons(newAutomatedBase.CurrentPage,() => newAutomatedBase.CurrentPage.NavigateToSuspectProvidersWorkList(), false, true);
            }
        }


        [Test, Category("SmokeTestDeployment")]
        public void Verify_navigation_to_Repository()
        {
            using (var newAutomatedBase = new NewAutomatedBaseParallelRun())
            {
                newAutomatedBase.QuickLaunch.NavigateToRepository();
                newAutomatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.Repository.GetStringValue());
                Verify_Quick_Launch_Switch_Client_Help_Icons(newAutomatedBase.CurrentPage,() => newAutomatedBase.CurrentPage.NavigateToRepository(), false, true);
            }
        }

        [Test, Category("SmokeTestDeployment")]
        public void Verify_navigation_to_Reference_Manager()
        {
            using (var newAutomatedBase = new NewAutomatedBaseParallelRun())
            {
                newAutomatedBase.QuickLaunch.NavigateToReferenceManager();
                newAutomatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.ReferenceManager.GetStringValue());
                Verify_Quick_Launch_Switch_Client_Help_Icons(newAutomatedBase.CurrentPage,() => newAutomatedBase.CurrentPage.NavigateToReferenceManager(), false, true);
            }
        }

        [Test, Category("SmokeTestDeployment")]
        public void Verify_navigation_to_Invoice_Search()
        {
            using (var newAutomatedBase = new NewAutomatedBaseParallelRun())
            {
                newAutomatedBase.QuickLaunch.NavigateToInvoiceSearch();
                newAutomatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.InvoiceSearch.GetStringValue());
                Verify_Quick_Launch_Switch_Client_Help_Icons(newAutomatedBase.CurrentPage,() => newAutomatedBase.CurrentPage.NavigateToInvoiceSearch(), false, true);
            }
        }

        [Test, Category("SmokeTestDeployment")]
        public void Verify_navigation_to_QA_Manager()
        {
            using (var newAutomatedBase = new NewAutomatedBaseParallelRun())
            {
                newAutomatedBase.QuickLaunch.NavigateToAnalystManager();
                newAutomatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.QAManager.GetStringValue());
                Verify_Quick_Launch_Switch_Client_Help_Icons(newAutomatedBase.CurrentPage,() => newAutomatedBase.CurrentPage.NavigateToAnalystManager(), false, true);
            }
        }

        [Test, Category("SmokeTestDeployment")]
        public void Verify_navigation_to_QA_Claim_Search()
        {
            using (var newAutomatedBase = new NewAutomatedBaseParallelRun())
            {
                newAutomatedBase.QuickLaunch.NavigateToQaClaimSearch();
                newAutomatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.QaClaimSearch.GetStringValue());
                Verify_Quick_Launch_Switch_Client_Help_Icons(newAutomatedBase.CurrentPage,() => newAutomatedBase.CurrentPage.NavigateToQaClaimSearch(), false, true);
            }
        }

        [Test, Category("SmokeTestDeployment")]
        public void Verify_navigation_to_QA_Appeal_Search()
        {
            using (var newAutomatedBase = new NewAutomatedBaseParallelRun())
            {
                newAutomatedBase.QuickLaunch.NavigateToQaAppealSearch();
                newAutomatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.QaAppealSearch.GetStringValue());
                Verify_Quick_Launch_Switch_Client_Help_Icons(newAutomatedBase.CurrentPage,() => newAutomatedBase.CurrentPage.NavigateToQaAppealSearch(), false, true);
            }
        }

        [Test, Category("SmokeTestDeployment")] // TANT 195
        public void Verify_navigation_to_Client_Search_And_Edit_Setting_Manager()
        {
            using (var newAutomatedBase = new NewAutomatedBaseParallelRun())
            {
                var _clientSearch = newAutomatedBase.QuickLaunch.NavigateToClientSearch();
                newAutomatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.ClientSearch.GetStringValue());
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
                newAutomatedBase.CurrentPage.WaitForPageToLoadWithSideBarPanel();
                newAutomatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.EditSettingsManager.GetStringValue());
                Verify_Quick_Launch_Switch_Client_Help_Icons(newAutomatedBase.CurrentPage,() => newAutomatedBase.CurrentPage.NavigateToEditSettingsManager());

                newAutomatedBase.CurrentPage.ClickOnReturnToClientSearch();
                newAutomatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.ClientSearch.GetStringValue());
                Verify_Quick_Launch_Switch_Client_Help_Icons(newAutomatedBase.CurrentPage,() => newAutomatedBase.CurrentPage.NavigateToClientSearch());
                newAutomatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.ClientSearch.GetStringValue());

            }

        }

        [Test, Category("SmokeTestDeployment")] //CAR-2902(CAR-2744)
        public void Verify_navigation_to_My_Profile()
        {
            using (var newAutomatedBase = new NewAutomatedBaseParallelRun())
            {
                newAutomatedBase.QuickLaunch.NavigateToMyProfilePage();
                newAutomatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.MyProfile.GetStringValue());
                Verify_Quick_Launch_Switch_Client_Help_Icons(newAutomatedBase.CurrentPage,() => newAutomatedBase.CurrentPage.NavigateToMyProfilePage());
            }
        }

        [Test, Category("SmokeTestDeployment")]
        public void Verify_navigation_to_User_Profile_Search()
        {
            using (var newAutomatedBase = new NewAutomatedBaseParallelRun())
            {
                newAutomatedBase.QuickLaunch.NavigateToNewUserProfileSearch();
                newAutomatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.UserProfileSearch.GetStringValue());
                Verify_Quick_Launch_Switch_Client_Help_Icons(newAutomatedBase.CurrentPage,() => newAutomatedBase.CurrentPage.NavigateToNewUserProfileSearch());
            }
        }

        [Test, Category("SmokeTestDeployment"), Category("Navigation3")]
        public void Verify_navigation_to_Maintenance_Banners()
        {
            using (var newAutomatedBase = new NewAutomatedBaseParallelRun())
            {
                newAutomatedBase.QuickLaunch.NavigateToMaintenanceNotices();
                newAutomatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.MaintenanceBanners.GetStringValue());
                Verify_Quick_Launch_Switch_Client_Help_Icons(newAutomatedBase.CurrentPage,() => newAutomatedBase.CurrentPage.NavigateToMaintenanceNotices(), false, true);
            }
        }

        [Test, Category("SmokeTestDeployment")]
        public void Verify_navigation_to_Microstrategy()
        {
            using (var newAutomatedBase = new NewAutomatedBaseParallelRun())
            {
                newAutomatedBase.QuickLaunch.NavigateToMicrostrategyMaintenance();
                newAutomatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.MicrostrategyMaintenance.GetStringValue());
                Verify_Quick_Launch_Switch_Client_Help_Icons(newAutomatedBase.CurrentPage,() => newAutomatedBase.CurrentPage.NavigateToMicrostrategyMaintenance(), isCorePage: true);
            }
        }

        [Test, Category("SmokeTestDeployment")] //CAR-2214(CAR-2730)+ TANT-248
        public void Verify_navigation_to_Role_Manager()
        {
            using (var newAutomatedBase = new NewAutomatedBaseParallelRun())
            {
                var _roleManagerPage = newAutomatedBase.QuickLaunch.NavigateToRoleManager();
                _roleManagerPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.RoleManager.GetStringValue(),
                    "User should be able to navigate to the 'Role Manager' page");
                _roleManagerPage.SideBarPanelSearch.GetTopHeaderName()
                    .ShouldBeEqual("Filter Roles", "Side bar panel should be open"); //Find Role Managers
                _roleManagerPage.SideBarPanelSearch.ClickOnToggleSidebarPanelButton();
                _roleManagerPage.SideBarPanelSearch.IsSideBarPanelOpen().ShouldBeFalse("Is Side bar panel open?");
                Verify_Quick_Launch_Switch_Client_Help_Icons(newAutomatedBase.CurrentPage,() => newAutomatedBase.CurrentPage.NavigateToRoleManager(), isCorePage: true);
            }
        }

        [Test, Category("SmokeTestDeployment")] //TANT-243
        public void Verify_navigation_to_Client_Search_And_Edit_Setting_Manager_for_Inactive_Clients()
        {
            using (var newAutomatedBase = new NewAutomatedBaseParallelRun())
            {
                var _clientSearch = newAutomatedBase.QuickLaunch.NavigateToClientSearch();
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
                    newAutomatedBase.CurrentPage.WaitForPageToLoadWithSideBarPanel();
                    newAutomatedBase.CurrentPage.GetPageHeader().ShouldBeEqual(PageHeaderEnum.EditSettingsManager.GetStringValue());
                    Verify_Quick_Launch_Switch_Client_Help_Icons(newAutomatedBase.CurrentPage,() =>
                        newAutomatedBase.CurrentPage.NavigateToEditSettingsManager(isInactive: true));
                }
            }
        }

        [Test, Category("SmokeTestDeployment")] //TANT-250

        public void Verify_navigation_to_user_settings_form()
        {
            using (var newAutomatedBase = new NewAutomatedBaseParallelRun())
            {
                var _userProfileSearch =
                    newAutomatedBase.CurrentPage.SearchByUserIdToNavigateToUserSettingsForm(EnvironmentManager.Instance.HciAdminUsername);
                _userProfileSearch.GetLeftFormHeader().ShouldBeEqual(PageHeaderEnum.UserSettings.GetStringValue(),
                    "User settings form should be present");
                var tablist = Enum.GetValues(typeof(UserSettingsTabEnum)).Cast<UserSettingsTabEnum>()
                    .Select(x => x.GetStringValue()).ToList();
                _userProfileSearch.GetUserSettingsTabList()
                    .ShouldCollectionBeEqual(tablist, "Tab list should match");
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
                            _userProfileSearch.IsUserSettingsFormDisabled()
                                .ShouldBeFalse($"Is {tab} form disabled?");
                        }

                        _userProfileSearch.ClickOnCancelLink();
                        foreach (var label in tablist)
                        {
                            _userProfileSearch.ClickOnUserSettingTabByTabName(label);
                            _userProfileSearch.IsUserSettingsFormDisabled()
                                .ShouldBeTrue($"Is {tab} form disabled?");
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

        [

        Test, Category("SmokeTestDeployment")] //TANT-244
        public void Verify_Navigation_To_Create_New_User_Account_form()
        {
            using (var newAutomatedBase = new NewAutomatedBaseParallelRun())
            {
                var tablist = Enum.GetValues(typeof(NewUserAccountTabEnum)).Cast<NewUserAccountTabEnum>().Select(x => x.GetStringValue()).ToList();
                var _userProfileSearch = newAutomatedBase.CurrentPage.NavigateToCreateNewUserAccount();
                _userProfileSearch.GetUserSettingsTabList().ShouldCollectionBeEqual(tablist, $"{tablist} tabs should match");
                _userProfileSearch.GetLeftFormHeader().ShouldBeEqual(PageHeaderEnum.CreateUser.GetStringValue(), "New User Account form should be present");
                _userProfileSearch.IsCreateNewUserTabSelectedByTabName(tablist[0]).ShouldBeTrue($"{tablist[0]} tab should be selected");
                _userProfileSearch.IsNewUserAccountNextButtonPresent().ShouldBeTrue("Is Next Button present?");
                _userProfileSearch.IsCancelButtonPresent().ShouldBeTrue("Is Cancel Button present?");
            }
        }

        [Test, Category("SmokeTestDeployment"), Category("AppealDependent")] //TANT-245
        public void Verify_Move_Appeals_to_QA_in_Appeal_Manager_page()
        {
            using (var newAutomatedBase = new NewAutomatedBaseParallelRun())
            {
                var _appealManager = newAutomatedBase.CurrentPage.NavigateToAppealManager();
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

        private void Verify_Quick_Launch_Switch_Client_Help_Icons(NewDefaultPage CurrentPage,Action navigateToPage, bool popup = false, bool isCorePage=false,bool needToSwitch=false)
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
                if (CurrentPage.IsWorkingAjaxMessagePresent())
                    CurrentPage.RefreshPage(false);
                navigateToPage();
        }

        public PreAuthActionPage NaviateToPreAuthActionPage(NewDefaultPage CurrentPage, int row, int col)
        {
            return CurrentPage.NavigateToPreAuthSearch().NaviateToPreAuthActionPage(row, col);
        }

        public AppealSummaryPage NavigateToAppealSummaryPage(NewDefaultPage CurrentPage)
        {
            return CurrentPage.NavigateToAppealSearch().NavigateToAppealSummaryPageBySideBarPannel(true);
        }

        public AppealCreatorPage NavigateToAppealCreatorPage(NewDefaultPage CurrentPage)
        {
            return CurrentPage.NavigateToAppealCreator().SearchByClaimNoAndStayOnSearch("1").ClickOnFirstEnableCreateAppealIcon();
        }

        public BatchSummaryPage NavigateToBatchSummaryPage(NewDefaultPage CurrentPage)
        {
            return CurrentPage.NavigateToBatchSearch().ClickOnBatchByRowCol(1, 2);
        }

        public AppealsDetailPage NavigateToAppealsDetailPageViaDashboard(NewDefaultPage CurrentPage)
        {
            
            return CurrentPage.NavigateToDashboard().ClickOnAppealsDetailExpandIcon();
        }

        public ClaimsDetailPage NavigateToClaimsDetailPageViaDashboard(NewDefaultPage CurrentPage)
        {
            return CurrentPage.NavigateToDashboard().ClickOnClaimsDetailExpandIcon();
        }


        public DashboardLogicRequestsDetailsPage NavigateToLogicRequestsDetailPageViaDashboard(NewDefaultPage CurrentPage)
        {
            return CurrentPage.NavigateToDashboard().ClickOnLogicRequestsDetailClientExpandIcon();
        }

        public ClaimsDetailPage NavigateToFFPClaimsDetailDashboard(NewDefaultPage CurrentPage)
        {
            return CurrentPage.NavigateToDashboard().ClickOnFFPClaimsDetailExpandIcon();
        }



        #endregion

    }
}

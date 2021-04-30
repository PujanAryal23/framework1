using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Nucleus.Service.Data;
using Nucleus.Service.PageServices.ChromeDownLoad;
using Nucleus.Service.PageServices.Dashboard;
using Nucleus.Service.PageServices.Settings.User;
using Nucleus.Service.SqlScriptObjects.Common_SQL;
using Nucleus.Service.Support.Common.Constants;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Environment;
using Nucleus.Service.Support.Utils;
using Nucleus.UIAutomation.TestSuites.Base;
using NUnit.Framework;
using UIAutomation.Framework.Utils;
using Extensions = Nucleus.Service.Support.Utils.Extensions;


namespace Nucleus.UIAutomation.TestSuites.TestCases
{
    class DashboardClaimsDetail : NewAutomatedBase
    {
        #region PRIVATE FIELDS

        private DashboardPage _dashboard;
        private readonly String _day = DateTime.Now.ToString("ddd");
        private ClaimsDetailPage _claimsDetailPage;
        private List<List<String>> _allCotivitiExpectedLists = new List<List<string>>();
        private List<String> _expectedClaimByClientList;
        private List<string> _expectedRealTimeClientList;
        private ChromeDownLoadPage chromeDownLoad;
        private UserProfileSearchPage _userProfileSearch;

        #endregion

        #region OVERRIDE METHODS

        protected override string FullyQualifiedClassName
        {
            get
            {
                return GetType().FullName;
            }
        }

        /// <summary>
        /// Override ClassInit to add additional code.
        /// </summary>
        protected override void ClassInit()
        {
            try
            {
                base.ClassInit();
                _dashboard = QuickLaunch.NavigateToDashboard();
                _claimsDetailPage = _dashboard.ClickOnClaimsDetailExpandIcon();
                AssignedExpectedList();

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
            base.TestCleanUp();
            if (string.Compare(UserType.CurrentUserType, UserType.HCIADMIN, StringComparison.OrdinalIgnoreCase) != 0)
            {
                QuickLaunch.Logout().LoginAsHciAdminUser().ClickOnSwitchClient().SwitchClientTo(EnvironmentManager.TestClient);
                _dashboard = QuickLaunch.NavigateToDashboard();
                _claimsDetailPage = _dashboard.ClickOnClaimsDetailExpandIcon();

            }
        }

        protected override void ClassCleanUp()
        {
            try
            {
                _claimsDetailPage.CloseConnection();
            }

            finally
            {
                base.ClassCleanUp();
            }
        }

        private void AssignedExpectedList()
        {
            _allCotivitiExpectedLists = _claimsDetailPage.GetAllCotivitiExpectedLists(EnvironmentManager.HciAdminUsername);
            _expectedClaimByClientList = _allCotivitiExpectedLists[0];
            _expectedRealTimeClientList =
                _claimsDetailPage.GetAllRealTimeExpectedLists(EnvironmentManager.HciAdminUsername)[0];
        }


        #endregion

        #region TEST SUITES

       
        //US29055 contains all story
        [Test]
        public void Verify_header_and_column_headers_in_dashboard_claims_detail_and_verify_client_values_sum_to_top_row_total_value()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            IList<string> expectedFirstRowColumnHeadersList = DataHelper.GetMappingData(FullyQualifiedClassName, "1st_row_column_headers").Values.ToList();
            IList<string> expectedSecondRowColumnHeadersList = DataHelper.GetMappingData(FullyQualifiedClassName, "2nd_row_column_headers").Values.ToList();
            _claimsDetailPage.GetClaimsDetailHeader().ShouldBeEqual("Claims by Client", "Claims Detail Header");

            expectedFirstRowColumnHeadersList.ShouldCollectionBeEqual(_claimsDetailPage.GetFirstRowColumnHeaders(), "1st row column headers are equal:");
            expectedSecondRowColumnHeadersList.ShouldCollectionBeEqual(_claimsDetailPage.GetSecondRowColumnHeaders(), "2nd row column headers are equal:");
            _claimsDetailPage.DoesLastUpdatedTimeAppearInUpperRightCornerOfThePage().ShouldBeTrue("Last updated time appears in upper right corner");


            StringFormatter.PrintMessageTitle("Verifying client values sum to top row total values");
                for (int i = 1; i <= _claimsDetailPage.GetFirstRowColumnHeaderCount(); i++)
                    _claimsDetailPage.GetHeaderValue(i).ShouldBeEqual(_claimsDetailPage.GetEachColumnSumValue(i, _claimsDetailPage.GetClaimsDetailHeader()), "Total sum equals to top value in column " + i);
      }

        [Test, Category("SmokeTest")]//US69671
        public void Verify_active_assigned_clients_appear_and_inactive_clients_do_not_appear_in_list_and_no_column_has_null_value_in_Claims_by_Client()
        {

            //TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            //IDictionary<string, string> paramLists = DataHelper.GetTestData(FullyQualifiedClassName, TestExtensions.TestName);
            //string allActiveClients = paramLists["ActiveClients"];
            //string allInActiveClients = paramLists["InActiveClients"];

            //var allInActiveClientsList = allInActiveClients.Split(';').ToList();

            string widget = "Claims by Client";// _claimsDetailPage.GetClaimsDetailHeader();
            StringFormatter.PrintMessageTitle("Verifying active clients appear and inactive clients donot appear in the list.");
            _claimsDetailPage.GetActiveClientList(widget).ShouldCollectionBeEqual(_expectedClaimByClientList, "Claims by Active Client List and should be sorted");
            //    TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            //    foreach (var activeClient in activeClientsList)
            //    {
            //       allActiveClientsList.Contains(activeClient).ShouldBeTrue("Active Client " + activeClient + " Appears in list?");
            //    }
            //    foreach (var allInactiveClient in allInActiveClientsList)
            //    {
            //        activeClientsList.Contains(allInactiveClient).ShouldBeFalse("InActive client " + allInactiveClient + " Appears in list?");
            //    }

            StringFormatter.PrintMessageTitle("Verifying values in column");
                if (_claimsDetailPage.GetAllColumnsValues(widget))
                    Console.WriteLine("No column has null value");
                else
                {
                    Assert.Fail("Column has null value.");
                }
            

        }
       

        [Test]
        public void Verify_clicking_on_Download_PDF_button_downloads_pdf_document()
        {
            
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
                var currentTime = DateTime.Now;
                string fileName = string.Format(@"DASHBOARD_CLAIMSSUMMARY_{0}{1}{2}{3}{4}.pdf", currentTime.Month,
                    currentTime.Day, currentTime.Year, currentTime.Hour, currentTime.Minute);
                string testserver = ConfigurationManager.AppSettings["TestServer"],
                    testserver2 = ConfigurationManager.AppSettings["TestNode"];
                bool isRemoteTestServer = !string.IsNullOrEmpty(testserver);
                string userName = isRemoteTestServer
                    ? ConfigurationManager.AppSettings["ProxyUserName"]
                    : System.Environment.UserName;

                string filelocation = string.Empty;

                if (isRemoteTestServer)
                {
                    //testserver = @"\\" + testserver;
                    testserver2 = @"\\" + testserver2;
                }


                
                _claimsDetailPage.IsSubMenuDownloadPDFPresent().ShouldBeTrue("Download to PDF submenu present");
                var url=_claimsDetailPage.CurrentPageUrl;
                _claimsDetailPage.ClickOnDownloadPDF();
                if (EnvironmentManager.Browser.Equals("IE", StringComparison.InvariantCulture))
                {
                    fileName = string.Format(@"C:\Users\{0}\Downloads\{1}", userName, fileName);
                    IEDownloadHelper.ClickSaveButtonToDownloadFile(isRemoteTestServer, testserver2, fileName);
                    filelocation = isRemoteTestServer ? fileName.Substring(2) : fileName;
                    Console.Out.WriteLine("File downloaded at {0}", filelocation);
                }
                else
                {
                    ChromeDownLoadPage chromeDownLoad = _claimsDetailPage.NavigateToChromeDownLoadPage();
                    filelocation = chromeDownLoad.GetFileLocation();
                    filelocation = filelocation.Replace("%20", " ");
                    filelocation = isRemoteTestServer ? filelocation.Substring(2).Replace("/", @"\") : filelocation;
                    Console.Out.WriteLine("File downloaded at {0}", filelocation);
                    _claimsDetailPage = chromeDownLoad.ClickBrowserBackButton<ClaimsDetailPage>(url);
                }
          
        
        }

        [Test] //TANT-22
        public void Verify_header_and_column_headers_and_client_list_in_dashboard_claims_detail_in_Real_Time_Claims_widget()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            IList<string> expectedRowColumnHeadersList = DataHelper
                .GetMappingData(FullyQualifiedClassName, "real_time_claims_column_header").Values.ToList();
            const string widget = "Real Time Claims";

            _claimsDetailPage.GetClaimsDetailContainerHeader()
                .ShouldBeEqual(widget, "Correct container header is displayed");
            _claimsDetailPage.GetRealTimeClaimsColumnHeaders().ShouldCollectionBeEqual(expectedRowColumnHeadersList,
                " Column headers are equal");
           
            StringFormatter.PrintMessageTitle("Verifying active real time and cvp batch clients list.");
            _claimsDetailPage.GetActiveClientList(widget).ShouldCollectionBeEqual(_expectedRealTimeClientList,
                "Real Time and CVP Batch Clients that are assigned to current user should be displayed.");
           _claimsDetailPage.IsListInAscedingOrder( _claimsDetailPage.GetActiveClientList(widget))
                .ShouldBeTrue("Client code should be sorted in ascending order");

            StringFormatter.PrintMessageTitle("Verifying values in column");
            if (_claimsDetailPage.GetAllRealTimeColumnsValues(widget))
                Console.WriteLine("No column has null value");
            else
            {
                Assert.Fail("Column has null value.");
            }
        }
       

        [Test] //TANT-22 +TE-840 + CAR-3003(CAR-2937)
        public void Verify_data_points_of_PCI_Claims_Details_in_Real_Time_Client_widget_for_Internal_user()
        {
            var headers = new List<string>
                {"Overdue", "< 30 Mins", "< 1 Hr", "< 2 Hr", "< 4 Hr", "< 6 Hr", "Total"};

            _claimsDetailPage.GetClaimsDetailRealTimeClaimsDataHeader().ShouldCollectionBeEqual(headers, "Headers should match");

            foreach (var realTimeClient in _expectedRealTimeClientList)
            {
                StringFormatter.PrintMessage("Clicking on the caret sign to expand the claim restriction section");
                _claimsDetailPage.IsClaimRestrictionSectionPresentForRealTimeClient(realTimeClient)
                    .ShouldBeFalse("Caret section is collapsed by default");

                _claimsDetailPage.ClickOnCaretSignByRealTimeClient(realTimeClient);
                _claimsDetailPage.IsClaimRestrictionSectionPresentForRealTimeClient(realTimeClient)
                    .ShouldBeTrue("Clicking on the caret sign should expand the claim restriction section");

                StringFormatter.PrintMessage("Verifying the order of the restrictions for the real time client");
                SortingAlgorithm.IsInAscendingOrder(_claimsDetailPage.GetClaimRestrictionNamesByRealTimeClient(realTimeClient)).ShouldBeTrue("Is Restricted list sorted in alphabetical order?");

                var claimRestrictionDataFromUI =
                    _claimsDetailPage.GetClaimCountsByRestrictionInRealTimeClientByClientName(realTimeClient);
                var overallUnreviewedClaimCountsFromUI =
                    _claimsDetailPage.GetClaimsCountForEveryDueTimeForRealTimeClientCssSelector(realTimeClient);

                var overallUnreviewedClaimCountsFromDB =
                    _claimsDetailPage.GetUnreviewedClaimDataForRealTimeClientFromDB(realTimeClient);
                var claimRestrictionDataFromUIFromDB =
                    _claimsDetailPage.GetRestrictedClaimDataForRealTimeClientFromDB(realTimeClient);

                StringFormatter.PrintMessageTitle($"Verifying the overall unreviewed claim counts with DB for {realTimeClient}");
                overallUnreviewedClaimCountsFromUI.ShouldCollectionBeEqual(overallUnreviewedClaimCountsFromDB,
                    "Overall unreviewed claim counts should match with DB");

                StringFormatter.PrintMessageTitle($"Verifying the claim restriction counts in caret section with DB for {realTimeClient}");
                int i = 0;
                foreach (var restrictedClaimCount in claimRestrictionDataFromUIFromDB)
                {
                    restrictedClaimCount
                        .ShouldCollectionBeEqual(claimRestrictionDataFromUI.GetRange(8 * i, 8),
                            "Count of the restricted claims should match with DB");
                    i++;
                }
            }
        }

        [Test, Category("IENonCompatible")] //TE-41
        public void Verify_clicking_on_Download_PDF_button_downloads_pdf_document_in_Real_Time_Client_Widget()
        {
            var url="";
            try
            {
                const string expectedFileName = "DASHBOARD_REALTIMECLAIMS";
                TestExtensions.TestName = new StackFrame(true).GetMethod().Name;

                
                _claimsDetailPage.IsRealTimeSubMenuDownloadPDFPresent().ShouldBeTrue("Download to PDF submenu present");
                 url = _claimsDetailPage.CurrentPageUrl;
                _claimsDetailPage.ClickOnRealTimeClaimsDownloadPDF();

                 chromeDownLoad = _claimsDetailPage.NavigateToChromeDownLoadPage();
                chromeDownLoad.GetFileName().AssertIsContained(expectedFileName, "Is Expected FileName Present?");
                var filelocation = chromeDownLoad.GetFileLocation();


                filelocation.AssertIsContained("api/dashboardSummary/exportRealtimeClaims/PCI", "Is FileLocation Correct?");
                Console.Out.WriteLine("File downloaded at {0}", filelocation);
            }
            finally

            {
                if(chromeDownLoad!=null)
                    _claimsDetailPage = chromeDownLoad.ClickBrowserBackButton<ClaimsDetailPage>(url);
            }



        }

        [Test, Category("Working")]
        public void Verify_Count_Of_Claims_Awaiting_QA_Review_In_Dashboard()
        {
            // Story blocked as of response from PO
            try
            {
                var clientList = _claimsDetailPage.GetClientListByWidget("Claims by Client");
                foreach (var client in clientList)
                {
                    _claimsDetailPage.GetAwaitingQAClaimsForAPIByClient(client);
                    
                }
            }
            catch (Exception e)
            {
                
            }
            


        }

        [Test] //TE-872
        public void Verify_Refreshing_On_Claims_Detail_Page_User_Remains_On_Same_Page_for_user_with_MyDashboard_as_default_preference()
        {
            _userProfileSearch=CurrentPage.ClickOnQuickLaunch().NavigateToNewUserProfileSearch();
            _userProfileSearch.SearchUserByNameOrId(new List<string>{EnvironmentManager.HciAdminUserWithSuspectProviderDefaultPage},true);
            _userProfileSearch.ClickOnGridRowByUserIdToOpenUserSettingSideView(EnvironmentManager.HciAdminUserWithSuspectProviderDefaultPage);
            _userProfileSearch.ClickOnUserSettingTabByTabName(Extensions.GetStringValue(UserSettingsTabEnum.Preferences));
            _userProfileSearch.GetInputTextBoxValueByLabel(Extensions.GetStringValue(UserPreferencesEnum.DefaultDashboard)).ShouldBeEqual("My Dashboard", "Default Dashboard Should Match");

            CurrentPage.Logout().LoginWithDefaultSuspectProvidersPage();

            CurrentPage.NavigateToCVDashboard();

            //var profileManager = CurrentPage.NavigateToProfileManager();
            //profileManager.GetDefaultDashboardPreference().ShouldBeEqual("My Dashboard","Default Dashboard Should Match");
            //CurrentPage.NavigateToCVDashboard();

            _dashboard.ClickOnClaimsDetailExpandIcon();
            CurrentPage.RefreshPage(false);
            CurrentPage.IsPageErrorPopupModalPresent().ShouldBeFalse("Error Popup Should Not Be Present");
            CurrentPage.GetPageHeader().ShouldBeEqual("Dashboard - Claims Detail", "Page Header Should Match");

            CurrentPage.NavigateToFFPDashboard();
            _dashboard.ClickOnFFPClaimsDetailExpandIcon();
            CurrentPage.RefreshPage(false);
            CurrentPage.IsPageErrorPopupModalPresent().ShouldBeFalse("Error Popup Should Not Be Present");
            CurrentPage.GetPageHeader().ShouldBeEqual("Dashboard - FFP Claims Detail", "Page Header Should Match");

        }

        [Test] //CAR-3264(CAR-3049)
        [Author("Pujan Aryal")]
        public void Verify_Real_Time_QC_Claims_Tab_In_Claims_Detail_Page()
        {
            TestExtensions.TestName = new StackFrame(true).GetMethod().Name;
            IList<string> expectedRowColumnHeadersList = DataHelper
                .GetMappingData(FullyQualifiedClassName, "real_time_claims_column_header").Values.ToList();
            const string widget = "Real Time QC Claims";

            StringFormatter.PrintMessage("Verify Real Time QC Claims tab is present");
            _claimsDetailPage.SwitchWidgetByTabName(widget);
            _claimsDetailPage.GetClaimsDetailContainerHeader()
                .ShouldBeEqual(widget, "Is view switched to Real Time QC Claims ?");

            StringFormatter.PrintMessage("Verify columns across the top are as expected");
            _claimsDetailPage.GetRealTimeClaimsColumnHeaders().ShouldCollectionBeEqual(expectedRowColumnHeadersList,
                " Column headers are equal");

            StringFormatter.PrintMessageTitle("Verify clients list are as expected.");
            _claimsDetailPage.GetActiveClientList(widget).ShouldCollectionBeEqual(_expectedRealTimeClientList,
                "A list of all the clients with processing type of Real-time, PCA Real time, PCA batch that are assigned to current user should be displayed.");
            _claimsDetailPage.IsListInAscedingOrder(_claimsDetailPage.GetActiveClientList(widget))
                .ShouldBeTrue("Client code should be sorted in ascending order");

            StringFormatter.PrintMessageTitle("Verify claims count data are as expected.");
            foreach (var realTimeClient in _expectedRealTimeClientList)
            {
                var overallClaimCountsWaitingForQCReviewFromDB =
                    _claimsDetailPage.GetOverallClaimCountsWaitingForQCReviewFromDB(realTimeClient);
                _claimsDetailPage.GetClaimsCountForEveryDueTimeForRealTimeClientCssSelector(realTimeClient).
                    ShouldCollectionBeEqual(overallClaimCountsWaitingForQCReviewFromDB, "For each client, counts of" +
                        " claims that are waiting for QC review should be shown in the corresponding columns, " +
                        "calculating the claim due date/time from the current due date/time.");
            }
        }

        #endregion
    }
}
